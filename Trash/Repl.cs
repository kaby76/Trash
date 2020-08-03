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

    class Repl
    {
        List<string> History { get; set; } = new List<string>();
        const string PreviousHistoryFfn = ".trash.rc";
        Dictionary<string, string> Aliases { get; set; } = new Dictionary<string, string>();
        Stack<Tuple<string, string, IParseTree>> stack = new Stack<Tuple<string, string, IParseTree>>();

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
                if (tree.read() != null)
                {
                    History.Add(line);
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
                else if (tree.import_() != null)
                {
                    History.Add(line);
                    var import = tree.import_();
                    var type = import.type()?.GetText();
                    var f = import.ffn().GetText();
                    f = f.Substring(1, f.Length - 2);
                    if (type == "antlr3")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        LanguageServer.Antlr3Import.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                    else if (type == "antlr2")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        LanguageServer.Antlr2Import.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                    else if (type == "bison")
                    {
                        var ii = System.IO.File.ReadAllText(f);
                        Dictionary<string, string> res = new Dictionary<string, string>();
                        LanguageServer.BisonImport.Try(f, ii, ref res);
                        System.Console.Write(res.First().Value);
                    }
                }
                else if (tree.history() != null)
                {
                    History.Add(line);
                    System.Console.WriteLine();
                    for (int i = 0; i < History.Count; ++i)
                    {
                        var h = History[i];
                        System.Console.WriteLine(i + " " + h);
                    }
                }
                else if (tree.quit() != null)
                {
                    History.Add(line);
                    return false;
                }
                else if (tree.empty() != null)
                {
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
                else if (tree.alias() != null)
                {
                    History.Add(line);
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
                else if (tree.find() != null)
                {
                    History.Add(line);
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
                else if (tree.print() != null)
                {
                    History.Add(line);
                    var top = stack.Peek();
                    System.Console.WriteLine();
                    System.Console.WriteLine(top.Item1);
                    System.Console.WriteLine(top.Item2);
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

    }

}
