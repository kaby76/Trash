namespace LanguageServer
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using Microsoft.CodeAnalysis;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Document = Workspaces.Document;

    public class NullableValue
    {
        public enum Value
        {
            Top = 0,
            Empty = 1,
            NonEmpty = 2,
            Bottom = 3
        };
        public int V { get; set; }
        public void Add(int v)
        {
            V |= v;
        }
        public void Add(Value v)
        {
            V |= (int)v;
        }
        public override bool Equals(object obj)
        {
            if (obj is NullableValue nv)
                return this.V == nv.V;
            else return false;
        }
        public override string ToString()
        {
            switch (this.V)
            {
                case 0: return NullableValue.Value.Top.ToString();
                case 1: return NullableValue.Value.Empty.ToString();
                case 2: return NullableValue.Value.NonEmpty.ToString();
                case 3: return NullableValue.Value.Bottom.ToString();
                default: return "ERROR";
            };
        }
    }

    public class NodeEnumerator : IEnumerable<IParseTree>
    {
        IParseTree _root;
        public NodeEnumerator(IParseTree root) { _root = root; }

        public IEnumerator<IParseTree> GetEnumerator()
        {
            Stack<IParseTree> stack = new Stack<IParseTree>();
            stack.Push(_root);
            while (stack.Any())
            {
                var n = stack.Pop();
                yield return n;
                if (n is TerminalNodeImpl term)
                {
                }
                else
                {
                    for (int i = n.ChildCount - 1; i >= 0; --i)
                    {
                        var c = n.GetChild(i);
                        stack.Push(c);
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public class Analysis
    {
        public static void ShowCycles(int pos, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            ParsingResults pd_parser = ParsingResultsFactory.Create(document) as ParsingResults;
            if (pd_parser == null)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }
            var egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Find all other grammars by walking dependencies (import, vocab, file names).
            HashSet<string> read_files = new HashSet<string>
            {
                document.FullPath
            };
            Dictionary<Workspaces.Document, List<TerminalNodeImpl>> every_damn_literal =
                new Dictionary<Workspaces.Document, List<TerminalNodeImpl>>();
            for (; ; )
            {
                int before_count = read_files.Count;
                foreach (string f in read_files)
                {
                    List<string> additional = ParsingResults.InverseImports.Where(
                        t => t.Value.Contains(f)).Select(
                        t => t.Key).ToList();
                    read_files = read_files.Union(additional).ToHashSet();
                }
                foreach (string f in read_files)
                {
                    var additional = ParsingResults.InverseImports.Where(
                        t => t.Key == f).Select(
                        t => t.Value);
                    foreach (var t in additional)
                    {
                        read_files = read_files.Union(t).ToHashSet();
                    }
                }
                int after_count = read_files.Count;
                if (after_count == before_count)
                {
                    break;
                }
            }

            // Construct graph of symbol usage.
            Transform.TableOfRules table = new Transform.TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();
            Digraph<string> graph = new Digraph<string>();
            foreach (Transform.TableOfRules.Row r in table.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                graph.AddVertex(r.LHS);
            }
            foreach (Transform.TableOfRules.Row r in table.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                List<string> j = r.RHS;
                //j.Reverse();
                foreach (string rhs in j)
                {
                    Transform.TableOfRules.Row sym = table.rules.Where(t => t.LHS == rhs).FirstOrDefault();
                    if (!sym.is_parser_rule)
                    {
                        continue;
                    }
                    DirectedEdge<string> e = new DirectedEdge<string>() { From = r.LHS, To = rhs };
                    graph.AddEdge(e);
                }
            }
            List<string> starts = new List<string>();
            List<string> parser_lhs_rules = new List<string>();
            foreach (Transform.TableOfRules.Row r in table.rules)
            {
                if (r.is_parser_rule)
                {
                    parser_lhs_rules.Add(r.LHS);
                    if (r.is_start)
                    {
                        starts.Add(r.LHS);
                    }
                }
            }

            IParseTree rule = null;
            IParseTree it = pd_parser.AllNodes.Where(n =>
            {
                if (!(n is ANTLRv4Parser.ParserRuleSpecContext || n is ANTLRv4Parser.LexerRuleSpecContext))
                    return false;
                Interval source_interval = n.SourceInterval;
                int a = source_interval.a;
                int b = source_interval.b;
                IToken ta = pd_parser.TokStream.Get(a);
                IToken tb = pd_parser.TokStream.Get(b);
                var start = ta.StartIndex;
                var stop = tb.StopIndex + 1;
                return start <= pos && pos < stop;
            }).FirstOrDefault();
            rule = it;
            var k = (ANTLRv4Parser.ParserRuleSpecContext)rule;
            var tarjan = new TarjanSCC<string, DirectedEdge<string>>(graph);
            List<string> ordered = new List<string>();
            var sccs = tarjan.Compute();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Cycles in " + document.FullPath);
            var done = new List<IEnumerable<string>>();
            foreach (var scc in sccs)
            {
                if (scc.Value.Count() <= 1) continue;
                if (!done.Contains(scc.Value))
                {
                    foreach (var s in scc.Value)
                    {
                        sb.Append(" ");
                        sb.Append(s);
                    }
                    sb.AppendLine();
                    sb.AppendLine();
                    done.Add(scc.Value);
                }
            }

            //var scc = sccs[k.RULE_REF().ToString()];
            //foreach (var v in scc)
            //{
            //    ordered.Add(v);
            //}
        }

        public static List<DiagnosticInfo> PerformAnalysis(Document document, System.Collections.Generic.IEnumerable<string> start_rules)
        {
            List<DiagnosticInfo> result = new List<DiagnosticInfo>();

            // Check if initial file is a grammar.
            ParsingResults pd_parser = ParsingResultsFactory.Create(document) as ParsingResults;
            if (pd_parser == null)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }
            var egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Find all other grammars by walking dependencies (import, vocab, file names).
            HashSet<string> read_files = new HashSet<string>
            {
                document.FullPath
            };
            Dictionary<Workspaces.Document, List<TerminalNodeImpl>> every_damn_literal =
                new Dictionary<Workspaces.Document, List<TerminalNodeImpl>>();
            for (; ; )
            {
                int before_count = read_files.Count;
                foreach (string f in read_files)
                {
                    List<string> additional = ParsingResults.InverseImports.Where(
                        t => t.Value.Contains(f)).Select(
                        t => t.Key).ToList();
                    read_files = read_files.Union(additional).ToHashSet();
                }
                foreach (string f in read_files)
                {
                    var additional = ParsingResults.InverseImports.Where(
                        t => t.Key == f).Select(
                        t => t.Value);
                    foreach (var t in additional)
                    {
                        read_files = read_files.Union(t).ToHashSet();
                    }
                }
                int after_count = read_files.Count;
                if (after_count == before_count)
                {
                    break;
                }
            }

            {
                if (pd_parser.AllNodes != null)
                {
                    int[] histogram = new int[pd_parser.Map.Length];
                    var fun = pd_parser.Classify;
                    IEnumerable<IParseTree> it = pd_parser.AllNodes.Where(n => n is TerminalNodeImpl);
                    foreach (var n in it)
                    {
                        var t = n as TerminalNodeImpl;
                        int i = -1;
                        try
                        {
                            i = pd_parser.Classify(pd_parser, pd_parser.Attributes, t);
                            if (i >= 0)
                            {
                                histogram[i]++;
                            }
                        }
                        catch (Exception) { }
                    }
                    for (int j = 0; j < histogram.Length; ++j)
                    {
                        if (histogram[j] == 0) continue;
                        string i = histogram[j] + " occurrences of " + pd_parser.Map[j];
                        result.Add(
                            new DiagnosticInfo()
                            {
                                Document = document.FullPath,
                                Severify = DiagnosticInfo.Severity.Info,
                                Start = 0,
                                End = 0,
                                Message = i
                            });
                    }
                }
            }


            //IParseTree rule = null;
            //var tarjan = new TarjanSCC<string, DirectedEdge<string>>(graph);
            //List<string> ordered = new List<string>();
            //var sccs = tarjan.Compute();
            //foreach (var scc in sccs)
            //{
            //    if (scc.Value.Count() <= 1) continue;
            //    var k = scc.Key;
            //    var v = scc.Value;
            //    string i = "Participates in cycle " +
            //        string.Join(" => ", scc.Value);
            //    var (start, end) = table.rules.Where(r => r.LHS == k).Select(r =>
            //    {
            //        var lmt = TreeEdits.LeftMostToken(r.rule);
            //        var source_interval = lmt.SourceInterval;
            //        int a = source_interval.a;
            //        int b = source_interval.b;
            //        IToken ta = pd_parser.TokStream.Get(a);
            //        IToken tb = pd_parser.TokStream.Get(b);
            //        var st = ta.StartIndex;
            //        var ed = tb.StopIndex + 1;
            //        return (st, ed);
            //    }).FirstOrDefault();
            //    result.Add(
            //        new DiagnosticInfo()
            //        {
            //            Document = document.FullPath,
            //            Severify = DiagnosticInfo.Severity.Info,
            //            Start = start,
            //            End = end,
            //            Message = i
            //        });
            //}


            // Check for useless lexer tokens.
            List<string> unused = new List<string>();
            var pt = pd_parser.ParseTree;
            var l1 = TreeEdits.FindTopDown(pt, (in IParseTree t, out bool c) =>
            {
                c = true;
                if (t is ANTLRv4Parser.LexerRuleSpecContext)
                {
                    c = false;
                    return t;
                }
                return null;
            }).ToList();
            foreach (var t in l1)
            {
                var r1 = t as ANTLRv4Parser.LexerRuleSpecContext;
                var lhs = r1.TOKEN_REF();
                var k = lhs.GetText();
                var source_interval = lhs.SourceInterval;
                int a = source_interval.a;
                int b = source_interval.b;
                IToken ta = pd_parser.TokStream.Get(a);
                IToken tb = pd_parser.TokStream.Get(b);
                var st = ta.StartIndex;
                var ed = tb.StopIndex + 1;
                var refs = new Module().FindRefsAndDefs(st, document);
                if (refs.Count() <= 1)
                {
                    string i = "Lexer rule " + k + " unused in parser grammar rules. Consider removing.";
                    result.Add(
                        new DiagnosticInfo()
                        {
                            Document = document.FullPath,
                            Severify = DiagnosticInfo.Severity.Info,
                            Start = st,
                            End = ed,
                            Message = i
                        });
                }
            }

            // Check for literals or sets that contain \uXXXX with further characters past that
            // aren't other escapes. In other words, it gets confusing when you write '\u123456'.
            // Did you mean '\u{123456}' or did you mean '\u1234' '56'?
            var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                {
                    var nodes = engine.parseExpression(
                        @"//STRING_LITERAL",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                    .ToArray();
                    foreach (var n in nodes)
                    {
                        var t = n as TerminalNodeImpl;
                        var text = t.Payload.Text;
                        for (int i = 0; i < text.Length; ++i)
                        {
                            if (text[i] == '\\')
                            {
                                if (i + 1 >= text.Length) continue;
                                if (text[i + 1] == 'u')
                                {
                                    if (i + 2 >= text.Length) continue;
                                    if (text[i + 2] == '{') continue;
                                    if (i + 6 >= text.Length) continue;
                                    if (text[i + 6] == '\\') continue;
                                    else if (text[i + 6] == '\'') continue;
                                    else
                                    {
                                        // String containing Unicode 4-byte quantity, but it's questionable.
                                        string m = "Literal " + text + " looks unusual. Consider moving \\u's to end, or changing to code point \\u{XXXXXX} syntax.";
                                        result.Add(
                                            new DiagnosticInfo()
                                            {
                                                Document = document.FullPath,
                                                Severify = DiagnosticInfo.Severity.Info,
                                                Start = t.Payload.StartIndex,
                                                End = t.Payload.StopIndex,
                                                Message = m
                                            });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                {
                    var nodes = engine.parseExpression(
                        @"//LEXER_CHAR_SET",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                    .ToArray();
                    foreach (var n in nodes)
                    {
                        var t = n as TerminalNodeImpl;
                        var text = t.Payload.Text;
                        for (int i = 0; i < text.Length; ++i)
                        {
                            if (text[i] == '\\')
                            {
                                if (i + 1 >= text.Length) continue;
                                if (text[i + 1] == 'u')
                                {
                                    if (i + 2 >= text.Length) continue;
                                    if (text[i + 2] == '{') continue;
                                    if (i + 6 >= text.Length) continue;
                                    if (text[i + 6] == '\\') continue;
                                    else if (text[i + 6] == ']') continue;
                                    else if (text[i + 6] == '-') continue;
                                    else
                                    {
                                        // String containing Unicode 4-byte quantity, but it's questionable.
                                        string m = "Literal " + text + " looks unusual. Consider moving \\u's to end, or changing to code point \\u{XXXXXX} syntax.";
                                        result.Add(
                                            new DiagnosticInfo()
                                            {
                                                Document = document.FullPath,
                                                Severify = DiagnosticInfo.Severity.Info,
                                                Start = t.Payload.StartIndex,
                                                End = t.Payload.StopIndex,
                                                Message = m
                                            });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                {
                    var nodes = engine.parseExpression(
                        @"//parserRuleSpec",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement))
                    .ToArray();
                    foreach (var n in nodes)
                    {
                        var elements = engine.parseExpression(
                            @"ruleBlock/ruleAltList/labeledAlt/alternative/element[1]/atom/terminal/TOKEN_REF",
                                new StaticContextBuilder()).evaluate(dynamicContext, new object[] { n })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl)
                            .Select(x => x.GetText())
                            .ToArray();
                        // If there are more than 1 of any name, flag rule.
                        if (elements.Distinct().Count() != elements.Count())
                        {
                            var rule_ref = (n.AntlrIParseTree as ANTLRv4Parser.ParserRuleSpecContext)?.RULE_REF() as TerminalNodeImpl; ;
                            string name = rule_ref.GetText();
                            string m = "Rule " + name + " contains some common left factors. Consider grouping.";
                            result.Add(
                                new DiagnosticInfo()
                                {
                                    Document = document.FullPath,
                                    Severify = DiagnosticInfo.Severity.Info,
                                    Start = rule_ref.Payload.StartIndex,
                                    End = rule_ref.Payload.StopIndex,
                                    Message = m
                                });
                        }
                    }
                }
            }

            {
                ParsingResults pd = pd_parser;
                //var pt = pd.ParseTree;

                List<ANTLRv4Parser.AltListContext> altlists;
                List<ANTLRv4Parser.ElementContext> elements;
                List<ANTLRv4Parser.AltListContext> altlists2;
                List<ANTLRv4Parser.ElementContext> elements2;
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();

                    altlists = engine.parseExpression(
                        "//(altList | labeledAlt)/alternative/element/ebnf[not(child::blockSuffix)]/block/altList[not(@ChildCount > 1)]",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as ANTLRv4Parser.AltListContext).ToList();
                    altlists2 = engine.parseExpression(
                        "//(altList | labeledAlt)[not(@ChildCount > 1)]/alternative[not(@ChildCount > 1)]/element/ebnf[not(child::blockSuffix)]/block/altList[@ChildCount > 1]",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as ANTLRv4Parser.AltListContext).ToList();
                    elements = altlists.Select(t => t.Parent.Parent.Parent as ANTLRv4Parser.ElementContext).ToList();
                    elements2 = altlists2.Select(t => t.Parent.Parent.Parent as ANTLRv4Parser.ElementContext).ToList();
                }

                for (int j = 0; j < altlists.Count; ++j)
                {
                    // Remove {altlist}/../../.. (an element), which is the "i'th" child.
                    var altlist = altlists[j];
                    var element = elements[j];
                    var parent_alternative = element?.Parent as ANTLRv4Parser.AlternativeContext;
                    IParseTree p = null;
                    for (p = element; p != null; p = p.Parent)
                    {
                        if (p as ANTLRv4Parser.ParserRuleSpecContext != null) break;
                    }
                    var rule_ref = (p as ANTLRv4Parser.ParserRuleSpecContext)?.RULE_REF() as TerminalNodeImpl; ;
                    string name = rule_ref.GetText();
                    string m = "Rule " + name + " contains useless parentheses. ";
                    result.Add(
                        new DiagnosticInfo()
                        {
                            Document = document.FullPath,
                            Severify = DiagnosticInfo.Severity.Info,
                            Start = rule_ref.Payload.StartIndex,
                            End = rule_ref.Payload.StopIndex,
                            Message = m
                        });
                    //int i = 0;
                    //for (; i < parent_alternative.ChildCount;)
                    //{
                    //    if (parent_alternative.children[i] == element)
                    //        break;
                    //    ++i;
                    //}
                    //parent_alternative.children.RemoveAt(i);
                    //parent_alternative.children.Insert(i, altlist);
                }
                for (int j = 0; j < altlists2.Count; ++j)
                {
                    // Remove {altlist}/../../.. (an element), which is the "i'th" child.
                    var altlist = altlists2[j];
                    var element = elements2[j];
                    var parent_alternative = element?.Parent as ANTLRv4Parser.AlternativeContext;
                    IParseTree p = null;
                    for (p = element; p != null; p = p.Parent)
                    {
                        if (p as ANTLRv4Parser.ParserRuleSpecContext != null) break;
                    }
                    var rule_ref = (p as ANTLRv4Parser.ParserRuleSpecContext)?.RULE_REF() as TerminalNodeImpl; ;
                    string name = rule_ref.GetText();
                    string m = "Rule " + name + " contains useless parentheses. ";
                    result.Add(
                        new DiagnosticInfo()
                        {
                            Document = document.FullPath,
                            Severify = DiagnosticInfo.Severity.Info,
                            Start = rule_ref.Payload.StartIndex,
                            End = rule_ref.Payload.StopIndex,
                            Message = m
                        });
                    //int i = 0;
                    //for (; i < parent_alternative.ChildCount;)
                    //{
                    //    if (parent_alternative.children[i] == element)
                    //        break;
                    //    ++i;
                    //}
                    //parent_alternative.children.RemoveAt(i);
                    //parent_alternative.children.Insert(i, altlist);
                }
            }

            {
                ParsingResults pd = pd_parser;
                var d = Nullable(pd.ParseTree);
                foreach (var pair in d)
                {
                    var name = pair.Key;
                    var val = pair.Value;
                    string m = "Rule " + name + " is " + val;
                    result.Add(
                        new DiagnosticInfo()
                        {
                            Document = document.FullPath,
                            Severify = DiagnosticInfo.Severity.Info,
                            //Start = term.Payload.StartIndex,
                            //End = term.Payload.StopIndex,
                            Message = m
                        });
                }
            }

            {
                var ag = new AntlrGraph();
                ParsingResults pd = pd_parser;
                var res = ag.Visit(pd.ParseTree);
                Find(result, document, res, ag.Rules);
                FindInterrule(result, document, start_rules, res, ag.Rules);
                //System.Console.WriteLine(res.ToString());
                FindReachable(result, document, ag.Rules, start_rules);
            }

            return result;
        }

        private static void FindReachable(List<DiagnosticInfo> result, Document document, Dictionary<string, Digraph<string, SymbolEdge>> rules, IEnumerable<string> start_rules)
        {
            // Convert.
            Digraph<string, DirectedEdge<string>> g = new Digraph<string, DirectedEdge<string>>();
            foreach (var v in rules)
            {
                var sym = v.Key;
                var rule_graph = v.Value;
                Stack<string> stack = new Stack<string>();
                HashSet<string> visited = new HashSet<string>();
                foreach (var x in rule_graph.StartVertices)
                    stack.Push(x);
                g.AddVertex(sym);
                while (stack.Any())
                {
                    var x = stack.Pop();
                    visited.Add(x);
                    if (rule_graph.EndVertices.Contains(x))
                    {
                        continue;
                    }
                    foreach (var e in rule_graph.SuccessorEdges(x))
                    {
                        var sym_to = e._symbol;
                        if (sym_to != "" && sym_to != null)
                            g.AddEdge(new DirectedEdge<string>() { From = sym, To = sym_to });
                        if (visited.Contains(e.To)) continue;
                        stack.Push(e.To);
                    }
                }
            }

            // Perform reachability and report all symbols not reachable.
            HashSet<string> visited_reachable = new HashSet<string>();
            var dfs = new DepthFirstOrder<string, DirectedEdge<string>>(g, start_rules);
            foreach (var v in dfs)
            {
                visited_reachable.Add(v);
            }
            foreach (var v in g.Vertices)
            {
                if (visited_reachable.Contains(v)) continue;
                string m = v + " is unreachable from the start symbol.";
                result.Add(
                    new DiagnosticInfo()
                    {
                        Document = document.FullPath,
                        Severify = DiagnosticInfo.Severity.Info,
                        //Start = term.Payload.StartIndex,
                        //End = term.Payload.StopIndex,
                        Message = m
                    });
            }

        }

        private static void Find(List<DiagnosticInfo> result, Document document, Digraph<string, SymbolEdge> res, Dictionary<string, Digraph<string, SymbolEdge>> rules)
        {
            foreach (var v in rules)
            {
                bool ok = false;
                var sym = v.Key;
                Stack<string> stack = new Stack<string>();
                HashSet<string> visited = new HashSet<string>();
                foreach (var x in v.Value.StartVertices)
                    stack.Push(x);
                while (stack.Any())
                {
                    var x = stack.Pop();
                    visited.Add(x);
                    if (v.Value.EndVertices.Contains(x))
                    {
                        ok = true;
                        break;
                    }
                    foreach (var e in res.SuccessorEdges(x))
                    {
                        if (e._symbol == sym) continue;
                        if (visited.Contains(e.To)) continue;
                        stack.Push(e.To);
                    }
                }
                if (!ok)
                {
                    var name = sym;
                    string m = "Rule " + name + " is malformed. Infinite recursion.";
                    result.Add(
                        new DiagnosticInfo()
                        {
                            Document = document.FullPath,
                            Severify = DiagnosticInfo.Severity.Info,
                            //Start = term.Payload.StartIndex,
                            //End = term.Payload.StopIndex,
                            Message = m
                        });
                }
            }
        }

        private static void FindInterrule(List<DiagnosticInfo> result, Document document, IEnumerable<string> start_rules, Digraph<string, SymbolEdge> res, Dictionary<string, Digraph<string, SymbolEdge>> rules)
        {
            Digraph<string> interrule_graph = new Digraph<string>();
            Dictionary<string, HashSet<string>> symbols_used = new Dictionary<string, HashSet<string>>();

            Algorithms.Utils.MultiMap<string, List<SymbolEdge>> paths = new Algorithms.Utils.MultiMap<string, List<SymbolEdge>>();
            foreach (var v in rules)
            {
                // Find all paths from start to end for current rule.
                var sym = v.Key;
                var graph = v.Value;
                HashSet<SymbolEdge> visited = new HashSet<SymbolEdge>();
                Stack<List<SymbolEdge>> stack = new Stack<List<SymbolEdge>>();
                Dictionary<SymbolEdge, EdgeClassifier.Classification> classify = new Dictionary<SymbolEdge, EdgeClassifier.Classification>();
                var xxx = graph.StartVertices.First();
                EdgeClassifier.Classify(graph, xxx, ref classify);
                foreach (var x in v.Value.StartVertices)
                {
                    foreach (var e in res.SuccessorEdges(x))
                    {
                        stack.Push(new List<SymbolEdge>() { e });
                    }
                }

                while (stack.Any())
                {
                    var x = stack.Pop();
                    var xl = x.Last();
                    var xlv = xl.To;
                    visited.Add(xl);
                    if (v.Value.EndVertices.Contains(xl.To))
                    {
                        paths.Add(v.Key, x);
                        continue;
                    }
                    foreach (var e in res.SuccessorEdges(xlv))
                    {
                        if (classify[e] == EdgeClassifier.Classification.Back)
                        {
                            continue;
                        }
                        var new_path = x.ToList();
                        new_path.Add(e);
                        stack.Push(new_path);
                    }
                }

                interrule_graph.AddVertex(v.Key);
            }

            /*
            foreach (var pair in paths)
            {
                string k = pair.Key;
                List<List<SymbolEdge>> v = pair.Value;
                System.Console.WriteLine();
                System.Console.WriteLine(k);
                foreach (var l in v)
                {
                    foreach (var p in l)
                    {
                        System.Console.Write(p);
                    }
                    System.Console.WriteLine();
                }
            }
            */
        
            // For each rule, determine if all paths contain
            // a common symbol. Make note of those symbols.
            // Create a graph of these symbol usages.
            foreach (var r in rules)
            {
                var ps = paths[r.Key];
                bool first = true;
                string a = r.Key;
                symbols_used[a] = new HashSet<string>();
                foreach (List<SymbolEdge> p in ps)
                {
                    if (first)
                    {
                        first = false;
                        foreach (SymbolEdge q in p)
                        {
                            if (q._symbol != null)
                            {
                                symbols_used[a].Add(q._symbol);
                            }
                        }
                    }
                    else
                    {
                        var other_list = new HashSet<string>();
                        foreach (SymbolEdge q in p)
                        {
                            if (q._symbol != null)
                            {
                                other_list.Add(q._symbol);
                            }
                        }
                        var rr = symbols_used[a].Intersect(other_list);
                        symbols_used[a] = rr.ToHashSet();
                    }
                }
                if (!symbols_used[a].Any())
                    continue;

                foreach (var s in symbols_used[a])
                {
                    interrule_graph.AddEdge(new DirectedEdge<string>() { From = a, To = s });
                }
            }

            // Find cylces.
            var tarjan = new TarjanSCC<string, DirectedEdge<string>>(interrule_graph);
            IDictionary<string, IEnumerable<string>> sccs = tarjan.Compute();
            foreach (var pair in sccs)
            {
                var k = pair.Key;
                var v = pair.Value;
                var x = v.ToList();
                if (x.Count > 1)
                {
                    var name = String.Join("->", x);
                    string m = "Rule " + name + " are malformed. They contain a cycle that cannot terminate.";
                    result.Add(
                        new DiagnosticInfo()
                        {
                            Document = document.FullPath,
                            Severify = DiagnosticInfo.Severity.Info,
                                //Start = term.Payload.StartIndex,
                                //End = term.Payload.StopIndex,
                                Message = m
                        });
                }
            }
        }

        static void Update(Stack<IParseTree> stack, IParseTree parent, NullableValue to, NullableValue from)
        {
            var before = to.V;
            var after = from.V | before;
            if (!before.Equals(after))
            {
                to.V |= from.V;
                if (parent != null && !stack.Contains(parent)) stack.Push(parent);
            }
        }

        static void Assign(Stack<IParseTree> stack, IParseTree parent, NullableValue to, NullableValue from)
        {
            to.V = from.V;
            if (parent != null && !stack.Contains(parent)) stack.Push(parent);
        }

        static Dictionary<string, NullableValue> Nullable(IParseTree root)
        {
            Stack<IParseTree> stack = new Stack<IParseTree>();
            Dictionary<string, NullableValue> nullable_rule_ref = new Dictionary<string, NullableValue>();
            Dictionary<IParseTree, NullableValue> nullable = new Dictionary<IParseTree, NullableValue>();
            foreach (var v in new NodeEnumerator(root))
            {
                nullable[v] = new NullableValue() { V = (int)NullableValue.Value.Top };
                if (v is ANTLRv4Parser.ParserRuleSpecContext)
                {
                    var s = (v as ANTLRv4Parser.ParserRuleSpecContext).RULE_REF().GetText();
                    nullable_rule_ref[s] = new NullableValue() { V = (int)NullableValue.Value.Top };
                }
                else if (v is ANTLRv4Parser.LexerRuleSpecContext)
                {
                    var s = (v as ANTLRv4Parser.LexerRuleSpecContext).TOKEN_REF().GetText();
                    nullable_rule_ref[s] = new NullableValue() { V = (int)NullableValue.Value.Top };
                }
            }
            foreach (var v in new NodeEnumerator(root))
            {
                if (v.ChildCount == 0) stack.Push(v);
            }
            while (stack.Any())
            {
                var v = stack.Pop();
                if (v is TerminalNodeImpl tni)
                {
                    if (tni.Symbol.Type == ANTLRv4Parser.RULE_REF
                        || tni.Symbol.Type == ANTLRv4Parser.TOKEN_REF)
                    {
                        var s = v.GetText();
                        if (nullable_rule_ref.ContainsKey(s))
                            Assign(stack, v.Parent, nullable[v], nullable_rule_ref[s]);
                        else
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.NonEmpty });
                    }
                    else
                    {
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.NonEmpty });
                    }
                }
                else if (v is ANTLRv4Parser.GrammarSpecContext
                    || v is ANTLRv4Parser.GrammarDeclContext
                    || v is ANTLRv4Parser.GrammarTypeContext
                    || v is ANTLRv4Parser.PrequelConstructContext
                    || v is ANTLRv4Parser.OptionsSpecContext
                    || v is ANTLRv4Parser.OptionContext
                    || v is ANTLRv4Parser.OptionValueContext
                    || v is ANTLRv4Parser.DelegateGrammarsContext
                    || v is ANTLRv4Parser.DelegateGrammarContext
                    || v is ANTLRv4Parser.TokensSpecContext
                    || v is ANTLRv4Parser.ChannelsSpecContext
                    || v is ANTLRv4Parser.IdListContext
                    || v is ANTLRv4Parser.Action_Context
                    || v is ANTLRv4Parser.ActionScopeNameContext
                    || v is ANTLRv4Parser.ActionBlockContext
                    || v is ANTLRv4Parser.ArgActionBlockContext
                    || v is ANTLRv4Parser.ModeSpecContext
                    || v is ANTLRv4Parser.RulesContext
                    || v is ANTLRv4Parser.ExceptionGroupContext
                    || v is ANTLRv4Parser.ExceptionHandlerContext
                    || v is ANTLRv4Parser.FinallyClauseContext
                    || v is ANTLRv4Parser.RulePrequelContext
                    || v is ANTLRv4Parser.RuleReturnsContext
                    || v is ANTLRv4Parser.ThrowsSpecContext
                    || v is ANTLRv4Parser.LocalsSpecContext
                    || v is ANTLRv4Parser.RuleActionContext
                    || v is ANTLRv4Parser.RuleModifiersContext
                    || v is ANTLRv4Parser.RuleModifierContext
                    || v is ANTLRv4Parser.LexerAtomContext
                    || v is ANTLRv4Parser.AtomContext
                    || v is ANTLRv4Parser.NotSetContext
                    || v is ANTLRv4Parser.SetElementContext
                    || v is ANTLRv4Parser.CharacterRangeContext
                    || v is ANTLRv4Parser.TerminalContext
                    || v is ANTLRv4Parser.ElementOptionsContext
                    || v is ANTLRv4Parser.ElementOptionContext
                    || v is ANTLRv4Parser.IdentifierContext
                    || v is ANTLRv4Parser.LexerCommandsContext
                    || v is ANTLRv4Parser.LexerCommandContext
                    || v is ANTLRv4Parser.LexerCommandNameContext
                    || v is ANTLRv4Parser.LexerCommandExprContext)
                    Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.NonEmpty });

                else if (v is ANTLRv4Parser.RuleSpecContext)
                {
                    Assign(stack, v.Parent, nullable[v], nullable[v.GetChild(0)]);
                }
                else if (v is ANTLRv4Parser.ParserRuleSpecContext prsc)
                {
                    var b = nullable[prsc.ruleBlock()];
                    Assign(stack, v.Parent, nullable[v], b);
                    if (!nullable_rule_ref.ContainsKey(prsc.RULE_REF().GetText()))
                        nullable_rule_ref[prsc.RULE_REF().GetText()] = new NullableValue() { V = (int)NullableValue.Value.NonEmpty };
                    Assign(stack, v.Parent, nullable_rule_ref[prsc.RULE_REF().GetText()], b);
                }
                else if (v is ANTLRv4Parser.RuleBlockContext rbc)
                {
                    var b = nullable[rbc.ruleAltList()];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.RuleAltListContext ralc)
                {
                    bool first = true;
                    var d = new NullableValue();
                    for (int i = 0; i < ralc.ChildCount; ++i)
                    {
                        var c = ralc.GetChild(i);
                        if (!(c is ANTLRv4Parser.LabeledAltContext)) continue;
                        var b = nullable[c];
                        if (first)
                            d.V = b.V;
                        else
                            d.Add(b.V);
                        first = false;
                    }
                    Assign(stack, v.Parent, nullable[v], d);
                }
                else if (v is ANTLRv4Parser.LabeledAltContext lac)
                {
                    var b = nullable[lac.alternative()];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.LexerRuleSpecContext lrsc)
                {
                    var b = nullable[lrsc.lexerRuleBlock()];
                    Assign(stack, v.Parent, nullable[v], b);
                    if (!nullable_rule_ref.ContainsKey(lrsc.TOKEN_REF().GetText()))
                        nullable_rule_ref[lrsc.TOKEN_REF().GetText()] = new NullableValue() { V = (int)NullableValue.Value.NonEmpty };
                    Assign(stack, v.Parent, nullable_rule_ref[lrsc.TOKEN_REF().GetText()], b);
                }
                else if (v is ANTLRv4Parser.LexerRuleBlockContext lrbc)
                {
                    var b = nullable[lrbc.lexerAltList()];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.LexerAltListContext lalc)
                {
                    bool first = true;
                    var d = new NullableValue();
                    for (int i = 0; i < lalc.ChildCount; ++i)
                    {
                        var c = lalc.GetChild(i);
                        if (!(c is ANTLRv4Parser.LexerAltContext)) continue;
                        var b = nullable[c];
                        if (first)
                            d.V = b.V;
                        else
                            d.Add(b.V);
                        first = false;
                    }
                    Assign(stack, v.Parent, nullable[v], d);
                }
                else if (v is ANTLRv4Parser.LexerAltContext lac2)
                {
                    if (lac2.ChildCount == 0)
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                    else
                    {
                        var b = nullable[lac2.lexerElements()];
                        Assign(stack, v.Parent, nullable[v], b);
                    }
                }
                else if (v is ANTLRv4Parser.LexerElementsContext lec)
                {
                    if (lec.ChildCount == 0)
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                    else
                    {
                        bool first = true;
                        var d = new NullableValue();
                        for (int i = 0; i < lec.ChildCount; ++i)
                        {
                            var c = lec.GetChild(i);
                            if (!(c is ANTLRv4Parser.LexerElementContext)) continue;
                            var b = nullable[c];
                            if (first)
                                d.V = b.V;
                            else
                                d.Add(b.V);
                            first = false;
                        }
                        if (0 != (d.V & (int)NullableValue.Value.NonEmpty))
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.NonEmpty });
                        else if (0 != (nullable[v].V & (int)NullableValue.Value.Empty))
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                        else
                            Assign(stack, v.Parent, nullable[v], new NullableValue());
                    }
                }
                else if (v is ANTLRv4Parser.LexerElementContext lec2)
                {
                    var suffix = lec2.ebnfSuffix();
                    bool opt = suffix != null && suffix.GetText()[0] != '+';
                    if (lec2.labeledLexerElement() != null)
                    {
                        if (opt)
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                        else
                            Assign(stack, v.Parent, nullable[v], nullable[lec2.labeledLexerElement()]);
                    }
                    else if (lec2.lexerAtom() != null)
                    {
                        if (opt)
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                        else
                            Assign(stack, v.Parent, nullable[v], nullable[lec2.lexerAtom()]);
                    }
                    else if (lec2.lexerBlock() != null)
                    {
                        if (opt)
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                        else
                            Assign(stack, v.Parent, nullable[v], nullable[lec2.lexerBlock()]);
                    }
                    else
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                }
                else if (v is ANTLRv4Parser.LabeledLexerElementContext llec)
                {
                    var c1 = llec.lexerAtom();
                    var c2 = llec.lexerBlock();
                    if (c1 != null)
                        Assign(stack, v.Parent, nullable[v], nullable[c1]);
                    else if (c2 != null)
                        Assign(stack, v.Parent, nullable[v], nullable[c2]);
                    else
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                }
                else if (v is ANTLRv4Parser.LexerBlockContext lbc)
                {
                    var b = nullable[lbc.lexerAltList()];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.AltListContext alc)
                {
                    bool first = true;
                    var d = new NullableValue();
                    for (int i = 0; i < alc.ChildCount; ++i)
                    {
                        var c = alc.GetChild(i);
                        if (!(c is ANTLRv4Parser.AlternativeContext)) continue;
                        var b = nullable[c];
                        if (first)
                            d.V = b.V;
                        else
                            d.Add(b.V);
                        first = false;
                    }
                    Assign(stack, v.Parent, nullable[v], d);
                }
                else if (v is ANTLRv4Parser.AlternativeContext ac)
                {
                    if (ac.ChildCount == 0)
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                    else
                    {
                        bool first = true;
                        var d = new NullableValue();
                        for (int i = 0; i < ac.ChildCount; ++i)
                        {
                            var c = ac.GetChild(i);
                            if (!(c is ANTLRv4Parser.ElementContext)) continue;
                            var b = nullable[c];
                            if (first)
                                d.V = b.V;
                            else
                                d.Add(b.V);
                            first = false;
                        }
                        if (0 != (d.V & (int)NullableValue.Value.NonEmpty))
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.NonEmpty });
                        else if (0 != (d.V & (int)NullableValue.Value.Empty))
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                    }
                }
                else if (v is ANTLRv4Parser.ElementContext ec)
                {
                    var suffix = ec.ebnfSuffix();
                    bool opt = suffix != null && suffix.GetText()[0] != '+';
                    if (ec.labeledElement() != null)
                    {
                        if (opt)
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                        else
                            Assign(stack, v.Parent, nullable[v], nullable[ec.labeledElement()]);
                    }
                    else if (ec.atom() != null)
                    {
                        if (opt)
                            Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                        else
                            Assign(stack, v.Parent, nullable[v], nullable[ec.atom()]);
                    }
                    else if (ec.ebnf() != null)
                    {
                        var b = nullable[ec.ebnf()];
                        Assign(stack, v.Parent, nullable[v], b);
                    }
                    else
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                }
                else if (v is ANTLRv4Parser.LabeledElementContext lec3)
                {
                    var c1 = lec3.atom();
                    var c2 = lec3.block();
                    if (c1 != null)
                        Assign(stack, v.Parent, nullable[v], nullable[c1]);
                    else if (c2 != null)
                        Assign(stack, v.Parent, nullable[v], nullable[c2]);
                    else
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                }
                else if (v is ANTLRv4Parser.EbnfContext ebnf)
                {
                    var suffix = ebnf.blockSuffix();
                    bool opt = suffix != null && suffix.GetText()[0] != '+';
                    if (ebnf.block() != null)
                    {
                        var b = nullable[ebnf.block()];
                        Assign(stack, v.Parent, nullable[v], b);
                    }
                    else
                        Assign(stack, v.Parent, nullable[v], new NullableValue() { V = (int)NullableValue.Value.Empty });
                }
                else if (v is ANTLRv4Parser.BlockSuffixContext)
                {
                    var b = nullable[v.GetChild(0)];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.EbnfSuffixContext ebnfsuffix)
                {
                    var a = ebnfsuffix.PLUS() != null;
                    var b = new NullableValue() { V = (int)(a ? NullableValue.Value.NonEmpty : NullableValue.Value.Empty) };
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.BlockSetContext bsc)
                {
                    bool first = true;
                    var d = new NullableValue();
                    for (int i = 0; i < bsc.ChildCount; ++i)
                    {
                        var c = bsc.GetChild(i);
                        if (!(c is ANTLRv4Parser.SetElementContext)) continue;
                        var b = nullable[c];
                        if (first)
                            d.V = b.V;
                        else
                            d.Add(b.V);
                        first = false;
                    }
                    Assign(stack, v.Parent, nullable[v], d);
                }
                else if (v is ANTLRv4Parser.BlockContext bc)
                {
                    var b = nullable[bc.altList()];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else if (v is ANTLRv4Parser.RulerefContext rrc)
                {
                    if (!nullable_rule_ref.ContainsKey(rrc.RULE_REF().GetText()))
                        nullable_rule_ref[rrc.RULE_REF().GetText()] = new NullableValue() { V = (int)NullableValue.Value.NonEmpty };
                    var b = nullable_rule_ref[rrc.RULE_REF().GetText()];
                    Assign(stack, v.Parent, nullable[v], b);
                }
                else
                    throw new Exception();
            }
            return nullable_rule_ref;
        }
    }

}
