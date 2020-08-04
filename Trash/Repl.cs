namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using Microsoft.CodeAnalysis;
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
        Stack<Tuple<string, string, IParseTree>> stack = new Stack<Tuple<string, string, IParseTree>>();

        void HistoryAdd(string input)
        {
            History.Add(input);
            WriteHistory();
        }

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

        void ReadHistory()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!System.IO.File.Exists(home + Path.DirectorySeparatorChar + PreviousHistoryFfn)) return;
            var history = System.IO.File.ReadAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn);
            History = history.ToList();
            RedoAliases();
        }

        void WriteHistory()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            System.IO.File.WriteAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn, History);
        }

        public void Run()
        {
            string input;
            ReadHistory();
            do
            {
                Console.Write("> ");
                input = Console.ReadLine();
            } while (Execute(input));
            WriteHistory();
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
                else if (tree.anything() != null)
                {
                    var anything = tree.anything();
                    if (Aliases.ContainsKey(anything.id().GetText()))
                    {
                        var cmd = Aliases[anything.id().GetText()];
                        var rest = anything.rest()?.children.Select(c => c.GetText());
                        var rs = rest != null ? String.Join(" ", rest) : "";
                        cmd = cmd + rs;
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
                    else if (bang.id() != null)
                    {
                        var s = bang.id().GetText();
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
                else if (tree.dot() != null)
                {
                    if (stack.Any())
                    {
                        var t = stack.Peek();
                        TerminalNodeImpl x = TreeEdits.LeftMostToken(t.Item3);
                        var ts = x.Payload.TokenSource;
                        System.Console.WriteLine();
                        System.Console.WriteLine(
                            TreeOutput.OutputTree(
                                t.Item3,
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
                    var top = stack.Peek();
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var atree = top.Item3;
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
                else if (tree.import_() != null)
                {
                    HistoryAdd(line);
                    var import = tree.import_();
                    var type = import.type()?.GetText();
                    var f = import.ffn().GetText();
                    f = f.Substring(1, f.Length - 2);
                    if (type == "antlr3")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        var imp = new LanguageServer.Antlr3Import();
                        imp.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                    else if (type == "antlr2")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        var imp = new LanguageServer.Antlr2Import();
                        imp.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                    else if (type == "bison")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        var imp = new LanguageServer.BisonImport();
                        imp.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                }
                else if (tree.print() != null)
                {
                    HistoryAdd(line);
                    var top = stack.Peek();
                    System.Console.WriteLine();
                    System.Console.WriteLine(top.Item1);
                    System.Console.WriteLine(top.Item2);
                }
                else if (tree.quit() != null)
                {
                    HistoryAdd(line);
                    return false;
                }
                else if (tree.read() != null)
                {
                    HistoryAdd(line);
                    var r = tree.read();
                    var f = r.ffn().GetText();
                    f = f.Substring(1, f.Length - 2);
                    var ii = System.IO.File.ReadAllText(f);
                    var s1 = new AntlrInputStream(ii);
                    var l1 = new ANTLRv4Lexer(s1);
                    var t1 = new CommonTokenStream(l1);
                    var p1 = new ANTLRv4Parser(t1);
                    var tr = p1.grammarSpec();
                    stack.Push(new Tuple<string, string, IParseTree>(f, ii, tr));
                }
                else if (tree.unfold() != null)
                {
                    HistoryAdd(line);
                    var unfold = tree.unfold();
                    var expr = unfold.StringLiteral().GetText();
                    expr = expr.Substring(1, expr.Length - 2);
                    var top = stack.Peek();
                    var document = CheckDoc(top.Item1);
                    var aparser = ParserDetailsFactory.Create(document).Parser;
                    var atree = document.GetParseTree();
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(
                        atree, aparser);
                    var nodes = engine.parseExpression(expr,
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    var res = LanguageServer.Transform.Unfold(nodes, document);
                    document.Code = res.First().Value;
                    document = CheckDoc(document.FullPath);
                    stack.Pop();
                    stack.Push(new Tuple<string, string, IParseTree>(top.Item1,
                        document.Code, document.GetParseTree()));
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
                if (tree.read() != null)
                {
                }
                else if (tree.import_() != null)
                {
                }
                else if (tree.history() != null)
                {
                }
                else if (tree.quit() != null)
                {
                }
                else if (tree.empty() != null)
                {
                }
                else if (tree.bang() != null)
                {
                }
                else if (tree.alias() != null)
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

        public static Document CheckDoc(string path)
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
            document.Changed = true;
            _ = ParserDetailsFactory.Create(document);
            _ = LanguageServer.Module.Compile();
            return document;
        }
    }
}
