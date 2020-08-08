namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using Microsoft.CodeAnalysis;
    using org.eclipse.wst.xml.xpath2.api;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Workspaces;

    class Repl
    {
        List<string> History { get; set; } = new List<string>();
        const string PreviousHistoryFfn = ".trash.rc";
        Dictionary<string, string> Aliases { get; set; } = new Dictionary<string, string>();
        Stack<Document> stack = new Stack<Document>();

        public Repl()
        {
        }

        void RedoAliases()
        {
            for (int i = 0; i < History.Count; ++i)
            {
                string input = History[i];
                RecallAliases(input);
            }
        }

        void HistoryAdd(string input)
        {
            History.Add(input);
            HistoryWrite();
        }

        void HistoryRead()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!System.IO.File.Exists(home + Path.DirectorySeparatorChar + PreviousHistoryFfn)) return;
            var history = System.IO.File.ReadAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn);
            History = history.ToList();
            RedoAliases();
        }

        void HistoryWrite()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            System.IO.File.WriteAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn, History);
        }

        public void Run()
        {
            string input;
            HistoryRead();
            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
            } while (Execute(input));
            HistoryWrite();
        }

        public bool Execute(string line)
        {
            try
            {
                var input = line + ";";
                var str = new AntlrInputStream(input);
                var lexer = new ReplLexer(str);
                var tokens = new CommonTokenStream(lexer);
                var parser = new ReplParser(tokens);
                var tree = parser.cmd();
                if (false) ;
                else if (tree.alias() != null)
                {
                    HistoryAdd(line);
                    var alias = tree.alias();
                    var id = alias.id();
                    var sl = alias.StringLiteral();
                    if (sl != null)
                    {
                        Aliases[id.GetText()] = sl.GetText().Substring(1, sl.GetText().Length - 2);
                    }
                    else if (alias.id_keyword() != null)
                    {
                        var id_keyword = alias.id_keyword();
                        Aliases[id.GetText()] = id_keyword.GetText();
                    }
                    else if (alias.id() == null)
                    {
                        System.Console.WriteLine();
                        foreach (var p in Aliases)
                        {
                            System.Console.WriteLine(p.Key + " = " + p.Value);
                        }
                    }
                }
                else if (tree.analyze() != null)
                {
                    HistoryAdd(line);
                    var doc = stack.Peek();
                    AnalyzeDoc(doc);
                }
                else if (tree.anything() != null)
                {
                    var anything = tree.anything();
                    if (Aliases.ContainsKey(anything.id().GetText()))
                    {
                        var cmd = Aliases[anything.id().GetText()];
                        var rest = anything.rest()?.children.Select(c => c.GetText());
                        var rs = rest != null ? String.Join(" ", rest) : "";
                        cmd = cmd + " " + rs;
                        return Execute(cmd);
                    }
                    else
                    {
                        System.Console.WriteLine("Unknown command");
                    }
                }
                else if (tree.bang() != null)
                {
                    var bang = tree.bang();
                    if (bang.@int() != null)
                    {
                        var snum = bang.@int().GetText();
                        var num = Int32.Parse(snum);
                        return Execute(History[num]);
                    }
                    else if (bang.BANG().Length > 1)
                    {
                        return Execute(History.Last());
                    }
                    else if (bang.id_keyword() != null)
                    {
                        var s = bang.id_keyword().GetText();
                        for (int i = History.Count - 1; i >= 0; --i)
                        {
                            if (History[i].StartsWith(s))
                            {
                                return Execute(History[i]);
                            }
                        }
                        System.Console.WriteLine("No previous command starts with " + s);
                    }
                }
                else if (tree.convert() != null)
                {
                    HistoryAdd(line);
                    var import = tree.convert();
                    var type = import.type()?.GetText();
                    var doc = stack.Peek();
                    var f = doc.FullPath;
                    if (type == "antlr3")
                    {
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        var imp = new LanguageServer.Antlr3Import();
                        imp.Try(doc.FullPath, doc.Code, ref res);
                        stack.Pop();
                        foreach (var r in res)
                        {
                            var new_doc = CheckDoc(r.Key);
                            new_doc.Code = r.Value;
                            stack.Push(new_doc);
                        }
                        System.Console.Write(res.First().Value);
                    }
                    else if (type == "antlr2")
                    {
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        var imp = new LanguageServer.Antlr2Import();
                        imp.Try(doc.FullPath, doc.Code, ref res);
                        foreach (var r in res)
                        {
                            var new_doc = CheckDoc(r.Key);
                            new_doc.Code = r.Value;
                            stack.Push(new_doc);
                        }
                        System.Console.Write(res.First().Value);
                    }
                    else if (type == "bison")
                    {
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        var imp = new LanguageServer.BisonImport();
                        imp.Try(doc.FullPath, doc.Code, ref res);
                        foreach (var r in res)
                        {
                            var new_doc = CheckDoc(r.Key);
                            new_doc.Code = r.Value;
                            stack.Push(new_doc);
                        }
                        System.Console.Write(res.First().Value);
                    }
                }
                else if (tree.dot() != null)
                {
                    if (stack.Any())
                    {
                        var doc = stack.Peek();
                        TerminalNodeImpl x = TreeEdits.LeftMostToken(doc.GetParseTree());
                        var ts = x.Payload.TokenSource;
                        System.Console.WriteLine();
                        System.Console.WriteLine(
                            TreeOutput.OutputTree(
                                doc.GetParseTree(),
                                ts as Lexer,
                                null).ToString());
                    }
                }
                else if (tree.find() != null)
                {
                    HistoryAdd(line);
                    var find = tree.find();
                    var expr = find.StringLiteral().GetText();
                    expr = expr.Substring(1, expr.Length - 2);
                    var doc = stack.Peek();
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var atree = doc.GetParseTree();
                    ANTLRv4Lexer alexer = new ANTLRv4Lexer(new AntlrInputStream(""));
                    CommonTokenStream cts = new CommonTokenStream(alexer);
                    ANTLRv4Parser aparser = new ANTLRv4Parser(cts);
                    AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser);
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToArray();
                    foreach (var node in nodes)
                    {
                        TerminalNodeImpl x = TreeEdits.LeftMostToken(node);
                        var ts = x.Payload.TokenSource;
                        System.Console.WriteLine();
                        System.Console.WriteLine(
                            TreeOutput.OutputTree(
                                node,
                                ts as Lexer,
                                null).ToString());
                    }
                }
                else if (tree.fold() != null)
                {
                }
                else if (tree.empty() != null)
                {
                }
                else if (tree.history() != null)
                {
                    HistoryAdd(line);
                    System.Console.WriteLine();
                    for (int i = 0; i < History.Count; ++i)
                    {
                        var h = History[i];
                        System.Console.WriteLine(i + " " + h);
                    }
                }
                else if (tree.load() != null)
                {
                    HistoryAdd(line);
                    var r = tree.load();
                    var f = r.ffn().GetText();
                    f = f.Substring(1, f.Length - 2);
                    var doc = CheckDoc(f);
                    stack.Push(doc);
                }
                else if (tree.parse() != null)
                {
                    HistoryAdd(line);
                    var r = tree.parse();
                    var doc = stack.Peek();
                    ParseDoc(doc, r.type()?.GetText());
                }
                else if (tree.print() != null)
                {
                    HistoryAdd(line);
                    var doc = stack.Peek();
                    System.Console.WriteLine();
                    System.Console.WriteLine(doc.FullPath);
                    System.Console.WriteLine(doc.Code);
                }
                else if (tree.quit() != null)
                {
                    HistoryAdd(line);
                    return false;
                }
                else if (tree.rotate() != null)
                {
                    HistoryAdd(line);
                    var docs = stack.ToList();
                    var last = docs.Last();
                    docs.RemoveAt(docs.Count() - 1);
                    stack = new Stack<Document>();
                    foreach (var doc in docs) stack.Push(doc);
                    stack.Push(last);
                }
                else if (tree.stack() != null)
                {
                    HistoryAdd(line);
                    var docs = stack.ToList();
                    foreach (var doc in docs)
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine(doc.FullPath);
                    }
                }
                else if (tree.unfold() != null)
                {
                    HistoryAdd(line);
                    var unfold = tree.unfold();
                    var expr = unfold.StringLiteral().GetText();
                    expr = expr.Substring(1, expr.Length - 2);
                    var doc = stack.Peek();
                    var aparser = ParsingResultsFactory.Create(doc).Parser;
                    var atree = doc.GetParseTree();
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(
                        atree, aparser);
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    var res = LanguageServer.Transform.Unfold(nodes, doc);
                    doc.Code = res.First().Value;
                    doc = CheckDoc(doc.FullPath);
                    stack.Pop();
                    stack.Push(doc);
                }
            }
            catch
            {
                System.Console.WriteLine("Err");
            }
            return true;
        }

        public bool RecallAliases(string line)
        {
            try
            {
                var input = line + ";";
                var str = new AntlrInputStream(input);
                var lexer = new ReplLexer(str);
                var tokens = new CommonTokenStream(lexer);
                var parser = new ReplParser(tokens);
                var tree = parser.cmd();
                if (tree.alias() != null)
                {
                    var alias = tree.alias();
                    var id = alias.id();
                    var sl = alias.StringLiteral();
                    if (sl != null)
                    {
                        Aliases[id.GetText()] = sl.GetText().Substring(1, sl.GetText().Length - 2);
                    }
                    else if (alias.id_keyword() != null)
                    {
                        var id_keyword = alias.id_keyword();
                        Aliases[id.GetText()] = id_keyword.GetText();
                    }
                }
                else if (tree.anything() != null)
                {
                    var anything = tree.anything();
                    if (Aliases.ContainsKey(anything.id().GetText()))
                    {
                        var cmd = Aliases[anything.id().GetText()];
                        var rest = anything.rest()?.children.Select(c => c.GetText());
                        var rs = String.Join(" ", rest);
                        cmd = cmd + rs;
                        RecallAliases(cmd);
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        public Document CheckDoc(string path)
        {
            string file_name = path;
            Document document = Workspaces.Workspace.Instance.FindDocument(file_name);
            if (document == null)
            {
                document = new Workspaces.Document(file_name);
                try
                {   // Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(file_name))
                    {
                        // Read the stream to a string, and write the string to the console.
                        string str = sr.ReadToEnd();
                        document.Code = str;
                    }
                }
                catch (IOException)
                {
                }
                Project project = Workspaces.Workspace.Instance.FindProject("Misc");
                if (project == null)
                {
                    project = new Project("Misc", "Misc", "Misc");
                    Workspaces.Workspace.Instance.AddChild(project);
                }
                project.AddDocument(document);
            }
            return document;
        }

        public void AnalyzeDoc(Document document)
        {
            _ = ParsingResultsFactory.Create(document);
            var results = LanguageServer.Analysis.PerformAnalysis(document);
            
            foreach (var r in results)
            {
                System.Console.WriteLine(r.Document + " " + r.Severify + " " + r.Start + " " + r.Message);
                System.Console.WriteLine();
            }
        }

        public void ParseDoc(Document document, string grammar)
        {
            document.Changed = true;
            document.ParseAs = grammar;
            _ = ParsingResultsFactory.Create(document);
            _ = new LanguageServer.Module().Compile();
        }
    }
}
