namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Workspaces;
    using Utils;

    class Repl
    {
        List<string> History { get; set; } = new List<string>();
        const string PreviousHistoryFfn = ".trash.rc";
        Dictionary<string, string> Aliases { get; set; } = new Dictionary<string, string>();
        Utils.StackQueue<Document> stack = new Utils.StackQueue<Document>();

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
                    else if (tree.cd() != null)
                    {
                        var c = tree.cd();
                        var expr = c.StringLiteral()?.GetText();
                        expr = expr?.Substring(1, expr.Length - 2);
                        if (expr != null)
                        {
                            Directory.SetCurrentDirectory(expr);
                        }
                        else
                        {
                            expr = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                            Directory.SetCurrentDirectory(expr);
                        }
                    }
                    else if (tree.combine() != null)
                    {
                        var r = tree.combine();
                        var doc1 = stack.PeekTop(0);
                        var doc2 = stack.PeekTop(1);
                        var results = LanguageServer.Transform.CombineGrammars(doc1, doc2);
                        EnactEdits(results);
                    }
                    else if (tree.convert() != null)
                    {
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
                        ParseDoc(stack.Peek());
                    }
                    else if (tree.delete() != null)
                    {
                        var delete = tree.delete();
                        var expr = delete.StringLiteral().GetText();
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
                            var results = LanguageServer.Transform.Delete(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.dot() != null)
                    {
                        if (stack.Any())
                        {
                            var doc = stack.Peek();
                            var pr = ParsingResultsFactory.Create(doc);
                            var pt = pr.ParseTree;
                            TerminalNodeImpl x = TreeEdits.LeftMostToken(pt);
                            var ts = x.Payload.TokenSource;
                            System.Console.WriteLine();
                            System.Console.WriteLine(
                                TreeOutput.OutputTree(
                                    pt,
                                    ts as Lexer,
                                    null).ToString());
                        }
                    }
                    else if (tree.find() != null)
                    {
                        var find = tree.find();
                        var expr = find.StringLiteral().GetText();
                        expr = expr.Substring(1, expr.Length - 2);
                        var doc = stack.Peek();
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        var pr = ParsingResultsFactory.Create(doc);
                        var aparser = pr.Parser;
                        var atree = pr.ParseTree;
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                        {
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
                    }
                    else if (tree.fold() != null)
                    {
                        var c = tree.fold();
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
                            var results = LanguageServer.Transform.Fold(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.foldlit() != null)
                    {
                        var c = tree.foldlit();
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
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                            var results = LanguageServer.Transform.Foldlit(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.empty() != null)
                    {
                        continue; // Don't add to history.
                    }
                    else if (tree.has() != null)
                    {
                        List<Tuple<string, bool>> result = new List<Tuple<string, bool>>();
                        var c = tree.has();
                        var l_or_r = c.LEFT() == null ? "right" : "left";
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
                            if (c.DR() != null)
                            {
                                result = LanguageServer.Transform.HasDirectRec(nodes, l_or_r, doc);
                            }
                            else if (c.IR() != null)
                            {
                                result = LanguageServer.Transform.HasIndirectRec(nodes, l_or_r, doc);
                            }
                            else throw new Exception("unknown check");
                            foreach (var r in result)
                            {
                                System.Console.WriteLine(r.Item1 + " " + r.Item2);
                            }
                        }
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
                            var results = LanguageServer.Transform.ConvertRecursionToKleeneOperator(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.ls() != null)
                    {
                        var c = tree.ls();
                        var expr = c.StringLiteral()?.GetText();
                        expr = expr?.Substring(1, expr.Length - 2);
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
                        ParseDoc(doc, r.type()?.GetText());
                    }
                    else if (tree.pop() != null)
                    {
                        _ = stack.Pop();
                    }
                    else if (tree.print() != null)
                    {
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
                    else if (tree.read() != null)
                    {
                        var r = tree.read();
                        var f = r.ffn().GetText();
                        f = f.Substring(1, f.Length - 2);
                        var doc = CheckDoc(f);
                        stack.Push(doc);
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
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try( atree, aparser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var nodes = engine.parseExpression(expr,
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            var results = LanguageServer.Transform.ToRightRecursion(nodes, doc);
                            EnactEdits(results);
                        }
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
                        if (expr != null)
                        {
                            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = AntlrTreeEditing.AntlrDOM.ConvertToDOM.Try(atree, aparser))
                            {
                                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                                var nodes = engine.parseExpression(expr,
                                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                                var results = LanguageServer.Transform.RemoveUselessParentheses(nodes, doc);
                                EnactEdits(results);
                            }
                        }
                        else
                        {
                            var results = LanguageServer.Transform.RemoveUselessParentheses(doc);
                            EnactEdits(results);
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
                        var expr = unfold.StringLiteral().GetText();
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
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
                            var results = LanguageServer.Transform.Unfold(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.unify() != null)
                    {
                        var unify = tree.unify();
                        var expr = unify.StringLiteral().GetText();
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
                            var results = LanguageServer.Transform.Unify(nodes, doc);
                            EnactEdits(results);
                        }
                    }
                    else if (tree.write() != null)
                    {
                        var r = tree.write();
                        var doc = stack.Peek();
                        WriteDoc(doc);
                    }
                }
                HistoryAdd(line);
            }
            catch (Exception eeks)
            {
                System.Console.WriteLine(eeks.Message);
            }
            return true;
        }

        public void EnactEdits(Dictionary<string, string> results)
        {
            if (results.Count > 0)
            {
                stack.Pop();
                foreach (var res in results)
                {
                    if (res.Value == null) continue;
                    var new_doc = CreateDoc(res.Key);
                    new_doc.Code = res.Value;
                    stack.Push(new_doc);
                    ParseDoc(new_doc);
                }
            }
            else
            {
                System.Console.WriteLine("no changes");
            }
        }

        public bool RecallAliases(string line)
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
                parser.RemoveErrorListeners();
                var listener = new ErrorListener<IToken>(0);
                parser.AddErrorListener(listener);
                var tree = parser.cmd();
                if (llistener.had_error) throw new Exception("command syntax error");
                if (listener.had_error) throw new Exception("command syntax error");
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

        public Document CreateDoc(string path)
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
                catch (IOException eeks)
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
                catch (IOException eeks)
                {
                    throw eeks;
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

        public void WriteDoc(Document document)
        {
            if (document != null)
            {
                var file_name = document.FullPath;
                try
                {   // Open the text file using a stream reader.
                    using (StreamWriter sw = new StreamWriter(file_name))
                    {
                        sw.Write(document.Code);
                    }
                    System.Console.WriteLine("Written to file " + file_name);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Failed to write file " + e.Message);
                }
            }
        }

        public void AnalyzeDoc(Document document)
        {
            _ = ParsingResultsFactory.Create(document);
            var results = LanguageServer.Analysis.PerformAnalysis(document);
            
            foreach (var r in results)
            {
                System.Console.Write(r.Document + " " + r.Severify + " " + r.Start + " " + r.Message);
                System.Console.WriteLine();
            }
        }

        public void ParseDoc(Document document, string grammar = null)
        {
            document.Changed = true;
            document.ParseAs = grammar;
            _ = ParsingResultsFactory.Create(document);
            _ = new LanguageServer.Module().Compile();
        }
    }
}
