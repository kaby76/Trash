using Trash.Commands;

namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Utils;
    using Workspaces;

    public class Repl
    {
        List<string> History { get; set; } = new List<string>();
        const string PreviousHistoryFfn = ".trash.rc";
        public Dictionary<string, string> Aliases { get; set; } = new Dictionary<string, string>();
        public Utils.StackQueue<Document> stack = new Utils.StackQueue<Document>();
        string script_file = null;
        int current_line_index = 0;
        string[] lines = null;
        public int QuietAfter = 10;
        public readonly Docs _docs;
        public Workspace _workspace { get; private set; } = new Workspace();

        public Repl(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-script":
                        script_file = args[i+1];
                        break;
                    default:
                        break;
                }
            }

            // Clean up left overs from "run" commands.
            var path = Path.GetTempPath();
            var regex = new Regex("Antlrvsix[0-9]+$");
            foreach (string dir in Directory.GetDirectories(path))
            {
                if (regex.IsMatch(dir))
                {
                    Directory.Delete(dir, true);
                }
            }

            _docs = new Docs(this);
        }

        void RedoAliases()
        {
            RecallAliases();
        }

        void HistoryAdd(string input)
        {
            History.Add(input);
            HistoryWrite();
        }

        void HistoryRead()
        {
            RedoAliases();
        }

        void HistoryWrite()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            System.IO.File.WriteAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn, History);
        }

        public string GetArg(ReplParser.ArgContext arg)
        {
            if (arg == null)
                return null;
            if (arg.StringLiteral() != null)
            {
                var sl = arg.StringLiteral().GetText();
                sl = sl.Substring(1, sl.Length - 2);
                return sl;
            }
            if (arg.stuff() != null)
            {
                var a = arg.stuff().GetText();
                return a;
            }
            return null;
        }

        public void Execute(string line)
        {
            try
            {
                var input = line + ";";
                var str = new AntlrInputStream(input);
                var lexer = new ReplLexer(str);
                lexer.RemoveErrorListeners();
                var llistener = new ErrorListener<int>(0);
                lexer.AddErrorListener(llistener);
                var tokens = new CommonTokenStream(lexer);
                var parser = new ReplParser(tokens);
                var listener = new ErrorListener<IToken>(2);
                parser.AddErrorListener(listener);
                var btree = parser.cmds();
                if (llistener.had_error) throw new Exception("command syntax error");
                if (listener.had_error) throw new Exception("command syntax error");
                foreach (var tree in btree.cmd())
                {
                    if (false) ;
                    else if (tree.alias() is ReplParser.AliasContext x_alias)
                    {
                        new CAlias().Execute(this, x_alias);
                    }
                    else if (tree.analyze() is ReplParser.AnalyzeContext x_analyze)
                    {
                        new CAnalyze().Execute(this, x_analyze);
                    }
                    else if (tree.anything() != null)
                    {
                        var anything = tree.anything();
                        if (Aliases.ContainsKey(anything.id().GetText()))
                        {
                            var cmd = Aliases[anything.id().GetText()];
                            var stuff = anything.stuff();
                            var rest = stuff.Select(s => s.GetText()).ToList();
                            var rs = rest != null ? String.Join(" ", rest) : "";
                            cmd = cmd + " " + rs;
                            Execute(cmd);
                            return;
                        }
                        else
                        {
                            System.Console.Error.WriteLine("Unknown command");
                        }
                    }
                    else if (tree.bang() != null)
                    {
                        var bang = tree.bang();
                        if (bang.@int() != null)
                        {
                            var snum = bang.@int().GetText();
                            var num = Int32.Parse(snum);
                            var recall = History[num];
                            System.Console.Error.WriteLine(recall);
                            Execute(recall);
                            return;
                        }
                        else if (bang.BANG().Length > 1)
                        {
                            var recall = History.Last();
                            System.Console.Error.WriteLine(recall);
                            Execute(recall);
                            return;
                        }
                        else if (bang.id_keyword() != null)
                        {
                            var s = bang.id_keyword().GetText();
                            for (int i = History.Count - 1; i >= 0; --i)
                            {
                                if (History[i].StartsWith(s))
                                {
                                    var recall = History[i];
                                    System.Console.Error.WriteLine(recall);
                                    Execute(recall);
                                    return;
                                }
                            }
                            System.Console.Error.WriteLine("No previous command starts with " + s);
                        }
                    }
                    else if (tree.cd() is ReplParser.CdContext x_cd)
                    {
                        new CCd().Execute(this, x_cd);
                    }
                    else if (tree.combine() is ReplParser.CombineContext x_combine)
                    {
                        new CCombine().Execute(this, x_combine);
                    }
                    else if (tree.convert() is ReplParser.ConvertContext x_convert)
                    {
                        new CConvert().Execute(this, x_convert);
                    }
                    else if (tree.delabel() is ReplParser.DelabelContext x_delabel)
                    {
                        new CDelabel().Execute(this, x_delabel);
                    }
                    else if (tree.delete() is ReplParser.DeleteContext x_delete)
                    {
                        new CDelete().Execute(this, x_delete);
                    }
                    else if (tree.dot() is ReplParser.DotContext x_dot)
                    {
                        new CDot().Execute(this, x_dot);
                    }
                    else if (tree.empty() != null)
                    {
                        throw new Repl.DoNotAddToHistory(); // Don't add to history.
                    }
                    else if (tree.find() is ReplParser.FindContext x_find)
                    {
                        new CFind().Execute(this, x_find);
                    }
                    else if (tree.fold() != null)
                    {
                        var c = tree.fold();
                        var expr = GetArg(c.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            var results = LanguageServer.Transform.Fold(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.foldlit() != null)
                    {
                        var c = tree.foldlit();
                        var expr = GetArg(c.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                            var results = LanguageServer.Transform.Foldlit(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.group() != null)
                    {
                        var group = tree.group();
                        var expr = GetArg(group.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            var results = LanguageServer.Transform.Group(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.has() != null)
                    {
                        var c = tree.has();
                        var graph = c.GRAPH() != null;
                        var expr = GetArg(c.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            List<IParseTree> nodes = null;
                            if (expr != null)
                            {
                                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            }
                            if (c.DR() != null)
                            {
                                var result = LanguageServer.Transform.HasDirectRec(doc, nodes);
                                foreach (var r in result)
                                {
                                    System.Console.WriteLine(r);
                                }
                            }
                            else if (c.IR() != null)
                            {
                                var result = LanguageServer.Transform.HasIndirectRec(nodes, graph, doc);
                                foreach (var r in result)
                                {
                                    System.Console.WriteLine(r);
                                }
                            }
                            else throw new Exception("unknown check");
                        }
                    }
                    else if (tree.help() is ReplParser.HelpContext x)
                    {
                        new CHelp().Execute(this, x);
                    }
                    else if (tree.history() != null)
                    {
                        System.Console.WriteLine();
                        for (int i = 0; i < History.Count; ++i)
                        {
                            var h = History[i];
                            System.Console.WriteLine(i + " " + h);
                        }
                    }
                    else if (tree.kleene() != null)
                    {
                        var c = tree.kleene();
                        var expr = GetArg(c.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            List<IParseTree> nodes = null;
                            if (expr != null)
                            {
                                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            }
                            var results = LanguageServer.Transform.ConvertRecursionToKleeneOperator(doc, nodes);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.ls() != null)
                    {
                        var c = tree.ls();
                        var expr = GetArg(c.arg());
                        var cwd = Directory.GetCurrentDirectory();
                        List<string> dirs = new List<string>(Directory.EnumerateDirectories(cwd));
                        foreach (var dir in dirs)
                        {
                            Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}/");
                        }
                        if (expr == null) expr = "*";
                        string[] files = Directory.GetFiles(cwd, expr);
                        foreach (var f in files)
                        {
                            Console.WriteLine($"{f.Substring(f.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                        }
                    }
                    else if (tree.mvsr() != null)
                    {
                        var c = tree.mvsr();
                        var expr = c.StringLiteral().GetText();
                        expr = expr.Substring(1, expr.Length - 2);
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            var results = LanguageServer.Transform.MoveStartRuleToTop(doc, nodes);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.parse() != null)
                    {
                        var r = tree.parse();
                        var doc = stack.Peek();
                        _docs.ParseDoc(doc, QuietAfter, r.type()?.GetText());
                    }
                    else if (tree.pop() != null)
                    {
                        _ = stack.Pop();
                    }
                    else if (tree.print() != null)
                    {
                        var doc = stack.Peek();
                        System.Console.Error.WriteLine();
                        System.Console.Error.WriteLine(doc.FullPath);
                        System.Console.WriteLine(doc.Code);
                    }
                    else if (tree.pwd() != null)
                    {
                        var cwd = Directory.GetCurrentDirectory();
                        System.Console.Error.WriteLine(cwd);
                    }
                    else if (tree.quit() != null)
                    {
                        HistoryAdd(line);
                        throw new Repl.Quit();
                    }
                    else if (tree.read() != null)
                    {
                        var r = tree.read();
                        var f = GetArg(r.arg());
                        string here = null;
                        if (!f.Contains("."))
                        {
                            here = f;
                        }
                        if (f != null)
                        {
                            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), f);
                            if (files.Count() != 1)
                                throw new Exception("Ambiguous match for '" + f + "'.");
                            var doc = _docs.ReadDoc(files.First());
                            stack.Push(doc);
                        }
                        else if (here != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (; ; )
                            {
                                string cl;
                                if (script_file != null)
                                {
                                    cl = lines[current_line_index++];
                                }
                                else
                                {
                                    cl = Console.ReadLine();
                                }
                                if (cl == here)
                                    break;
                                sb.AppendLine(cl);
                            }
                            var s = sb.ToString();
                            var doc = _docs.CreateDoc("temp" + current_line_index + ".g4", s);
                            stack.Push(doc);
                        }
                        else throw new Exception("not sure what to read.");
                    }
                    else if (tree.rename() != null)
                    {
                        var rename = tree.rename();
                        var to_sym = rename.StringLiteral()[1].GetText();
                        to_sym = to_sym.Substring(1, to_sym.Length - 2);
                        var doc = stack.Peek();
                        var expr = rename.StringLiteral()[0].GetText();
                        expr = expr.Substring(1, expr.Length - 2);
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                            var results = LanguageServer.Transform.Rename(nodes, to_sym, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.reorder() != null)
                    {
                        Dictionary<string, string> results = new Dictionary<string, string>();
                        var c = tree.reorder();
                        var doc = stack.Peek();
                        string expr = null;
                        if (c.modes() != null)
                        {
                            results = LanguageServer.Transform.SortModes(doc);
                        }
                        else
                        {
                            LspAntlr.ReorderType order = default;
                            if (c.alpha() != null)
                                order = LspAntlr.ReorderType.Alphabetically;
                            else if (c.bfs() != null)
                            {
                                order = LspAntlr.ReorderType.BFS;
                                expr = c.bfs().StringLiteral().GetText();
                            }
                            else if (c.dfs() != null)
                            {
                                order = LspAntlr.ReorderType.DFS;
                                expr = c.dfs().StringLiteral().GetText();
                            }
                            else
                                throw new Exception("unknown sorting type");
                            List<IParseTree> nodes = null;
                            if (expr != null)
                            {
                                expr = expr.Substring(1, expr.Length - 2);
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                }
                            }
                            results = LanguageServer.Transform.ReorderParserRules(doc, order, nodes);
                        }
                        EnactEdits(results);
                    }
                    else if (tree.rotate() != null)
                    {
                        var top = stack.Pop();
                        var docs = stack.ToList();
                        docs.Reverse();
                        stack = new StackQueue<Document>();
                        stack.Push(top);
                        foreach (var doc in docs) stack.Push(doc);
                    }
                    else if (tree.rr() != null)
                    {
                        var c = tree.rr();
                        var expr = c.StringLiteral().GetText();
                        expr = expr.Substring(1, expr.Length - 2);
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            var results = LanguageServer.Transform.ToRightRecursion(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.run() != null)
                    {
                        var c = tree.run();
                        var g = new Grun(this);
                        var p = c.arg();
                        var parameters = p.Select(a => GetArg(a)).ToArray();
                        g.Run(parameters);
                    }
                    else if (tree.rup() != null)
                    {
                        var rup = tree.rup();
                        var expr = rup.StringLiteral()?.GetText();
                        expr = expr?.Substring(1, expr.Length - 2);
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        List<IParseTree> nodes = null;
                        if (expr != null)
                        {
                            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                            {
                                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            }
                        }
                        var results = LanguageServer.Transform.RemoveUselessParentheses(doc, nodes);
                        EnactEdits(results);
                    }
                    else if (tree.set() != null)
                    {
                        var c = tree.set();
                        var id = c.id_keyword().GetText();
                        var v1 = c.StringLiteral()?.GetText();
                        var v2 = c.INT()?.GetText();
                        if (id.ToLower() == "quietafter")
                        {
                            var v = int.Parse(v2);
                            QuietAfter = v;
                        }
                    }
                    else if (tree.split() != null)
                    {
                        var r = tree.split();
                        var doc = stack.Peek();
                        var results = LanguageServer.Transform.SplitGrammar(doc);
                        EnactEdits(results);
                    }
                    else if (tree.stack() != null)
                    {
                        var docs = stack.ToList();
                        foreach (var doc in docs)
                        {
                            System.Console.WriteLine();
                            System.Console.WriteLine(doc.FullPath);
                        }
                    }
                    else if (tree.ulliteral() != null)
                    {
                        var ulliteral = tree.ulliteral();
                        var expr = ulliteral.StringLiteral()?.GetText();
                        expr = expr?.Substring(1, expr.Length - 2);
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        if (expr != null)
                        {
                            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                            {
                                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                var nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                                var results = LanguageServer.Transform.UpperLowerCaseLiteral(nodes, doc);
                                EnactEdits(results);
                            }
                        }
                        else
                        {
                            var results = LanguageServer.Transform.UpperLowerCaseLiteral(null, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.unalias() != null)
                    {
                        var alias = tree.unalias();
                        var id = alias.id();
                        Aliases.Remove(id.GetText());
                    }
                    else if (tree.unfold() != null)
                    {
                        var unfold = tree.unfold();
                        var expr = GetArg(unfold.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                            var results = LanguageServer.Transform.Unfold(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.ungroup() != null)
                    {
                        var c = tree.ungroup();
                        var expr = GetArg(c.arg());
                        var doc = stack.Peek();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            var results = LanguageServer.Transform.Ungroup(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.write() != null)
                    {
                        var r = tree.write();
                        var doc = stack.Peek();
                        _docs.WriteDoc(doc);
                    }
                }
                HistoryAdd(line);
            }
            catch (Repl.DoNotAddToHistory)
            {
            }
            catch (Repl.Quit)
            {
                throw;
            }
            catch (Exception eeks)
            {
                System.Console.Error.WriteLine(eeks.Message);
            }
        }

        public void Execute()
        {
            HistoryRead();
            string input;
            ICharStream str;
            if (script_file != null)
            {
                // Read commands from script, and read a grammar file (Antlr4) from stdin.
                var st = System.IO.File.ReadAllText(script_file);
                str = new AntlrInputStream(st);
            }
            else
            {
                var stdin = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
                str = new MyInputStream(stdin);
            }
            { 
                var lexer = new ReplLexer(str);
                lexer.RemoveErrorListeners();
                var llistener = new ErrorListener<int>(0);
                lexer.AddErrorListener(llistener);
                var tokens = new MyUnbufferedTokenStream(lexer);
                int last = str.Index;
                for (; ; )
                {
                    if (str.Index == str.Size)
                    {
                        Console.Error.Write("> ");
                        if (last != str.Index)
                        {
                            // Copy everything from last to current index as string
                            // and enter into history.
                            var h = str.GetText(new Antlr4.Runtime.Misc.Interval(last, str.Index - 1));
                            h = h.Replace("\n", "").Replace("\r", "");
                            if (h != "")
                                HistoryAdd(h);
                            last = str.Index;
                        }
                    }

                    {
                        var parser = new ReplParser(tokens);
                        var listener = new ErrorListener<IToken>(2);
                        parser.AddErrorListener(listener);
                        parser.ErrorHandler = new MyBailErrorStrategy(tokens);
                        var tree = parser.cmd();

                        if (llistener.had_error) throw new Exception("command syntax error");
                        if (listener.had_error) throw new Exception("command syntax error");

                        try
                        {
                            if (false) ;
                            else if (tree.alias() is ReplParser.AliasContext x_alias)
                            {
                                new CAlias().Execute(this, x_alias);
                            }
                            else if (tree.analyze() is ReplParser.AnalyzeContext x_analyze)
                            {
                                new CAnalyze().Execute(this, x_analyze);
                            }
                            else if (tree.anything() != null)
                            {
                                var anything = tree.anything();
                                if (Aliases.ContainsKey(anything.id().GetText()))
                                {
                                    var cmd = Aliases[anything.id().GetText()];
                                    ReplParser.StuffContext[] stuff = anything.stuff();
                                    var rest = stuff.Select(s => s.GetText()).ToList();
                                    var rs = rest != null ? String.Join(" ", rest) : "";
                                    cmd = cmd + " " + rs;
                                    Execute(cmd);
                                    last = str.Index;
                                }
                                else
                                {
                                    System.Console.Error.WriteLine("Unknown command");
                                }
                            }
                            else if (tree.bang() != null)
                            {
                                var bang = tree.bang();
                                if (bang.@int() != null)
                                {
                                    var snum = bang.@int().GetText();
                                    var num = Int32.Parse(snum);
                                    var recall = History[num];
                                    System.Console.Error.WriteLine(recall);
                                    Execute(recall);
                                    throw new Repl.DoNotAddToHistory();
                                }
                                else if (bang.BANG().Length > 1)
                                {
                                    var recall = History.Last();
                                    System.Console.Error.WriteLine(recall);
                                    Execute(recall);
                                    throw new Repl.DoNotAddToHistory();
                                }
                                else if (bang.id_keyword() != null)
                                {
                                    var s = bang.id_keyword().GetText();
                                    for (int i = History.Count - 1; i >= 0; --i)
                                    {
                                        if (History[i].StartsWith(s))
                                        {
                                            var recall = History[i];
                                            System.Console.Error.WriteLine(recall);
                                            Execute(recall);
                                            throw new Repl.DoNotAddToHistory();
                                        }
                                    }
                                    System.Console.Error.WriteLine("No previous command starts with " + s);
                                }
                            }
                            else if (tree.cd() is ReplParser.CdContext x_cd)
                            {
                                new CCd().Execute(this, x_cd);
                            }
                            else if (tree.combine() is ReplParser.CombineContext x_combine)
                            {
                                new CCombine().Execute(this, x_combine);
                            }
                            else if (tree.convert() is ReplParser.ConvertContext x_convert)
                            {
                                new CConvert().Execute(this, x_convert);
                            }
                            else if (tree.delabel() is ReplParser.DelabelContext x_delabel)
                            {
                                new CDelabel().Execute(this, x_delabel);
                            }
                            else if (tree.delete() is ReplParser.DeleteContext x_delete)
                            {
                                new CDelete().Execute(this, x_delete);
                            }
                            else if (tree.dot() is ReplParser.DotContext x_dot)
                            {
                                new CDot().Execute(this, x_dot);
                            }
                            else if (tree.empty() != null)
                            {
                                throw new Repl.DoNotAddToHistory(); // Don't add to history.
                            }
                            else if (tree.find() is ReplParser.FindContext x_find)
                            {
                                new CFind().Execute(this, x_find);
                            }
                            else if (tree.fold() != null)
                            {
                                var c = tree.fold();
                                var expr = GetArg(c.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    var results = LanguageServer.Transform.Fold(nodes, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.foldlit() != null)
                            {
                                var c = tree.foldlit();
                                var expr = GetArg(c.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                                    var results = LanguageServer.Transform.Foldlit(nodes, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.group() != null)
                            {
                                var group = tree.group();
                                var expr = GetArg(group.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    var results = LanguageServer.Transform.Group(nodes, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.has() != null)
                            {
                                var c = tree.has();
                                var graph = c.GRAPH() != null;
                                var expr = GetArg(c.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    List<IParseTree> nodes = null;
                                    if (expr != null)
                                    {
                                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                        nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    }
                                    if (c.DR() != null)
                                    {
                                        var result = LanguageServer.Transform.HasDirectRec(doc, nodes);
                                        foreach (var r in result)
                                        {
                                            System.Console.WriteLine(r);
                                        }
                                    }
                                    else if (c.IR() != null)
                                    {
                                        var result = LanguageServer.Transform.HasIndirectRec(nodes, graph, doc);
                                        foreach (var r in result)
                                        {
                                            System.Console.WriteLine(r);
                                        }
                                    }
                                    else throw new Exception("unknown check");
                                }
                            }
                            else if (tree.help() is ReplParser.HelpContext x)
                            {
                                new CHelp().Execute(this, x);
                            }
                            else if (tree.history() != null)
                            {
                                System.Console.WriteLine();
                                for (int i = 0; i < History.Count; ++i)
                                {
                                    var h = History[i];
                                    System.Console.WriteLine(i + " " + h);
                                }
                            }
                            else if (tree.kleene() != null)
                            {
                                var c = tree.kleene();
                                var expr = GetArg(c.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    List<IParseTree> nodes = null;
                                    if (expr != null)
                                    {
                                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                        nodes = engine.parseExpression(expr,
                                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    }
                                    var results = LanguageServer.Transform.ConvertRecursionToKleeneOperator(doc, nodes);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.ls() != null)
                            {
                                var path = ".";
                                var c = tree.ls();
                                var expr = GetArg(c.arg());
                                if (expr != null)
                                {
                                    path = expr;
                                }
                                List<string> dirs = new List<string>(Directory.EnumerateDirectories(path));
                                foreach (var dir in dirs)
                                {
                                    Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}/");
                                }
                                if (expr == null) expr = "*";
                                string[] files = Directory.GetFiles(path, expr);
                                foreach (var f in files)
                                {
                                    Console.WriteLine($"{f.Substring(f.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                                }
                            }
                            else if (tree.mvsr() != null)
                            {
                                var c = tree.mvsr();
                                var expr = c.StringLiteral().GetText();
                                expr = expr.Substring(1, expr.Length - 2);
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    var results = LanguageServer.Transform.MoveStartRuleToTop(doc, nodes);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.parse() != null)
                            {
                                var r = tree.parse();
                                var doc = stack.Peek();
                                _docs.ParseDoc(doc, QuietAfter, r.type()?.GetText());
                            }
                            else if (tree.pop() != null)
                            {
                                _ = stack.Pop();
                            }
                            else if (tree.print() != null)
                            {
                                var doc = stack.Peek();
                                System.Console.Error.WriteLine();
                                System.Console.Error.WriteLine(doc.FullPath);
                                System.Console.WriteLine(doc.Code);
                            }
                            else if (tree.pwd() != null)
                            {
                                var cwd = Directory.GetCurrentDirectory();
                                System.Console.Error.WriteLine(cwd);
                            }
                            else if (tree.quit() != null)
                            {
                                return;
                            }
                            else if (tree.read() != null)
                            {
                                var r = tree.read();
                                var f = GetArg(r.arg());
                                string here = null;
                                if (!f.Contains("."))
                                {
                                    here = f;
                                }
                                if (f != null)
                                {
                                    var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), f);
                                    if (files.Count() != 1)
                                        throw new Exception("Ambiguous match for '" + f + "'.");
                                    var doc = _docs.ReadDoc(files.First());
                                    stack.Push(doc);
                                }
                                else if (here != null)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    for (; ; )
                                    {
                                        string cl;
                                        if (script_file != null)
                                        {
                                            cl = lines[current_line_index++];
                                        }
                                        else
                                        {
                                            cl = Console.ReadLine();
                                        }
                                        if (cl == here)
                                            break;
                                        sb.AppendLine(cl);
                                    }
                                    var s = sb.ToString();
                                    var doc = _docs.CreateDoc("temp" + current_line_index + ".g4", s);
                                    stack.Push(doc);
                                }
                                else throw new Exception("not sure what to read.");
                            }
                            else if (tree.rename() != null)
                            {
                                var rename = tree.rename();
                                var to_sym = rename.StringLiteral()[1].GetText();
                                to_sym = to_sym.Substring(1, to_sym.Length - 2);
                                var doc = stack.Peek();
                                var expr = rename.StringLiteral()[0].GetText();
                                expr = expr.Substring(1, expr.Length - 2);
                                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                                    var results = LanguageServer.Transform.Rename(nodes, to_sym, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.reorder() != null)
                            {
                                Dictionary<string, string> results = new Dictionary<string, string>();
                                var c = tree.reorder();
                                var doc = stack.Peek();
                                string expr = null;
                                if (c.modes() != null)
                                {
                                    results = LanguageServer.Transform.SortModes(doc);
                                }
                                else
                                {
                                    LspAntlr.ReorderType order = default;
                                    if (c.alpha() != null)
                                        order = LspAntlr.ReorderType.Alphabetically;
                                    else if (c.bfs() != null)
                                    {
                                        order = LspAntlr.ReorderType.BFS;
                                        expr = c.bfs().StringLiteral().GetText();
                                    }
                                    else if (c.dfs() != null)
                                    {
                                        order = LspAntlr.ReorderType.DFS;
                                        expr = c.dfs().StringLiteral().GetText();
                                    }
                                    else
                                        throw new Exception("unknown sorting type");
                                    List<IParseTree> nodes = null;
                                    if (expr != null)
                                    {
                                        expr = expr.Substring(1, expr.Length - 2);
                                        var pr = ParsingResultsFactory.Create(doc);
                                        var aparser = pr.Parser;
                                        var atree = pr.ParseTree;
                                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                        {
                                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                            nodes = engine.parseExpression(expr,
                                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                        }
                                    }
                                    results = LanguageServer.Transform.ReorderParserRules(doc, order, nodes);
                                }
                                EnactEdits(results);
                            }
                            else if (tree.rotate() != null)
                            {
                                var top = stack.Pop();
                                var docs = stack.ToList();
                                docs.Reverse();
                                stack = new StackQueue<Document>();
                                stack.Push(top);
                                foreach (var doc in docs) stack.Push(doc);
                            }
                            else if (tree.rr() != null)
                            {
                                var c = tree.rr();
                                var expr = c.StringLiteral().GetText();
                                expr = expr.Substring(1, expr.Length - 2);
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    var results = LanguageServer.Transform.ToRightRecursion(nodes, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.run() != null)
                            {
                                var c = tree.run();
                                var g = new Grun(this);
                                var p = c.arg();
                                var parameters = p.Select(a => GetArg(a)).ToArray();
                                g.Run(parameters);
                            }
                            else if (tree.rup() != null)
                            {
                                var rup = tree.rup();
                                var expr = rup.StringLiteral()?.GetText();
                                expr = expr?.Substring(1, expr.Length - 2);
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                List<IParseTree> nodes = null;
                                if (expr != null)
                                {
                                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                    {
                                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                        nodes = engine.parseExpression(expr,
                                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    }
                                }
                                var results = LanguageServer.Transform.RemoveUselessParentheses(doc, nodes);
                                EnactEdits(results);
                            }
                            else if (tree.set() != null)
                            {
                                var c = tree.set();
                                var id = c.id_keyword().GetText();
                                var v1 = c.StringLiteral()?.GetText();
                                var v2 = c.INT()?.GetText();
                                if (id.ToLower() == "quietafter")
                                {
                                    var v = int.Parse(v2);
                                    QuietAfter = v;
                                }
                            }
                            else if (tree.split() != null)
                            {
                                var r = tree.split();
                                var doc = stack.Peek();
                                var results = LanguageServer.Transform.SplitGrammar(doc);
                                EnactEdits(results);
                            }
                            else if (tree.stack() != null)
                            {
                                var docs = stack.ToList();
                                foreach (var doc in docs)
                                {
                                    System.Console.WriteLine();
                                    System.Console.WriteLine(doc.FullPath);
                                }
                            }
                            else if (tree.ulliteral() != null)
                            {
                                var ulliteral = tree.ulliteral();
                                var expr = ulliteral.StringLiteral()?.GetText();
                                expr = expr?.Substring(1, expr.Length - 2);
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                if (expr != null)
                                {
                                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                    {
                                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                        var nodes = engine.parseExpression(expr,
                                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                                        var results = LanguageServer.Transform.UpperLowerCaseLiteral(nodes, doc);
                                        EnactEdits(results);
                                    }
                                }
                                else
                                {
                                    var results = LanguageServer.Transform.UpperLowerCaseLiteral(null, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.unalias() != null)
                            {
                                var alias = tree.unalias();
                                var id = alias.id();
                                Aliases.Remove(id.GetText());
                            }
                            else if (tree.unfold() != null)
                            {
                                var unfold = tree.unfold();
                                var expr = GetArg(unfold.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                                    var results = LanguageServer.Transform.Unfold(nodes, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.ungroup() != null)
                            {
                                var c = tree.ungroup();
                                var expr = GetArg(c.arg());
                                var doc = stack.Peek();
                                var pr = ParsingResultsFactory.Create(doc);
                                var aparser = pr.Parser;
                                var atree = pr.ParseTree;
                                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                                {
                                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                    var nodes = engine.parseExpression(expr,
                                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                    var results = LanguageServer.Transform.Ungroup(nodes, doc);
                                    EnactEdits(results);
                                }
                            }
                            else if (tree.workspace() != null)
                            {
                                _workspace = new Workspace();
                            }
                            else if (tree.write() != null)
                            {
                                var r = tree.write();
                                var doc = stack.Peek();
                                _docs.WriteDoc(doc);
                            }
                            HistoryWrite();
                        }
                        catch (Repl.DoNotAddToHistory)
                        {
                        }
                        catch (Repl.Quit)
                        {
                            return;
                        }
                        catch (Exception eeks)
                        {
                            System.Console.Error.WriteLine(eeks.Message);
                        }
                        finally
                        {
                            // Reset the input buffer and start all over from
                            // scratch.
                            while (str.Index < str.Size && !(str.LA(1) == '\n' || str.LA(1) == '\r'))
                                str.Consume();
                            while (str.Index < str.Size && (str.LA(1) == '\n' || str.LA(1) == '\r'))
                                str.Consume();
                            lexer = new ReplLexer(str);
                            lexer.RemoveErrorListeners();
                            lexer.AddErrorListener(new ErrorListener<int>(0));
                            tokens = new MyUnbufferedTokenStream(lexer);
                        }
                    }
                }
            }
        }

        private class Quit : Exception
        {
            public Quit() { }
        }

        private class DoNotAddToHistory : Exception
        {
            public DoNotAddToHistory() { }
        }

        public void EnactEdits(Dictionary<string, string> results)
        {
            if (results.Count > 0)
            {
                stack.Pop();
                foreach (var res in results)
                {
                    if (res.Value == null) continue;
                    var new_doc = _docs.CreateDoc(res.Key, res.Value);
                    stack.Push(new_doc);
                    _docs.ParseDoc(new_doc, QuietAfter);
                }
            }
            else
            {
                System.Console.Error.WriteLine("no changes");
            }
        }

        public void RecallAliases()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!System.IO.File.Exists(home + Path.DirectorySeparatorChar + PreviousHistoryFfn)) return;
            var history = System.IO.File.ReadAllLines(home + Path.DirectorySeparatorChar + PreviousHistoryFfn);
            History = history.ToList();
            var st = string.Join(Environment.NewLine, history);
            var str = new AntlrInputStream(st);
            var lexer = new ReplLexer(str);
            lexer.RemoveErrorListeners();
            var llistener = new ErrorListener<int>(0);
            lexer.AddErrorListener(llistener);
            if (false)
            {
                for (; ; )
                {
                    var tok = lexer.NextToken();
                    System.Console.WriteLine(tok);
                    if (tok.Type == -1)
                        break;
                }
            }

            var tokens = new UnbufferedTokenStream(lexer);
            int last = str.Index;
            for (; ; )
            {
                try
                {
                    var parser = new ReplParser(tokens);
                    var listener = new ErrorListener<IToken>(2);
                    parser.AddErrorListener(listener);
                    parser.ErrorHandler = new MyBailErrorStrategy(tokens);
                    var tree = parser.cmd();

                    if (llistener.had_error) throw new Exception("command syntax error");
                    if (listener.had_error) throw new Exception("command syntax error");

                    if (tree.alias() != null)
                    {
                        var alias = tree.alias();
                        var id = alias.ID()?.GetText();
                        var sl = alias.StringLiteral()?.GetText();
                        var id_keyword = alias.id_keyword()?.GetText();
                        if (id != null && sl != null)
                        {
                            Aliases[id] = sl.Substring(1, sl.Length - 2);
                        }
                        else if (id != null && id_keyword != null)
                        {
                            Aliases[id] = id_keyword;
                        }
                    }
                    else if (tree.unalias() != null)
                    {
                        var alias = tree.unalias();
                        var id = alias.id();
                        Aliases.Remove(id.GetText());
                    }
                    else if (tree.anything() != null)
                    {
                        var anything = tree.anything();
                        if (Aliases.ContainsKey(anything.id().GetText()))
                        {
                            var cmd = Aliases[anything.id().GetText()];
                            var stuff = anything.stuff();
                            var rest = stuff.Select(s => s.GetText()).ToList();
                            var rs = rest != null ? String.Join(" ", rest) : "";
                            cmd = cmd + rs;
                            //RecallAliases(cmd);
                        }
                    }
                }
                catch (Repl.DoNotAddToHistory)
                {
                }
                catch (Exception eeks)
                {
                }
                finally
                {
                    // Reset the input buffer and start all over from
                    // scratch.
                    while (str.Index < str.Size && !(str.LA(1) == '\n' || str.LA(1) == '\r'))
                        str.Consume();
                    while (str.Index < str.Size && (str.LA(1) == '\n' || str.LA(1) == '\r'))
                        str.Consume();
                    lexer = new ReplLexer(str);
                    lexer.RemoveErrorListeners();
                    lexer.AddErrorListener(new ErrorListener<int>(0));
                    tokens = new UnbufferedTokenStream(lexer);
                }
                if (str.Index == str.Size)
                    break;
            }
        }
    }
}
