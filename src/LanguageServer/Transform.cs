namespace LanguageServer
{
    using Algorithms;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;
    using Antlr4.Runtime.Tree;
    using AntlrTreeEditing.AntlrDOM;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using Domemtech.Symtab;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Document = Workspaces.Document;
    using NWayDiff;
    using org.w3c.dom;
    using Microsoft.CodeAnalysis.Operations;
    using System.Collections.Immutable;
    using System.Collections;
    using System.Text.RegularExpressions;

    public class Transform
    {
        public class LiteralsGrammar : ANTLRv4ParserBaseListener
        {
            public List<TerminalNodeImpl> Literals = new List<TerminalNodeImpl>();

            public LiteralsGrammar()
            {
            }

            public override void EnterTerminal([NotNull] ANTLRv4Parser.TerminalContext context)
            {
                TerminalNodeImpl first = context.GetChild(0) as TerminalNodeImpl;
                if (first.Symbol.Type == ANTLRv4Parser.STRING_LITERAL)
                {
                    Literals.Add(first);
                }
            }
        }

        public class FindFirstRule : ANTLRv4ParserBaseListener
        {
            public IParseTree First = null;
            public IParseTree Last = null;

            public FindFirstRule() { }

            public override void EnterRules([NotNull] ANTLRv4Parser.RulesContext context)
            {
                ANTLRv4Parser.RuleSpecContext[] rule_spec = context.ruleSpec();
                if (rule_spec == null)
                {
                    return;
                }

                First = rule_spec[0];
            }
        }

        public class FindFirstMode : ANTLRv4ParserBaseListener
        {
            public IParseTree First = null;
            public IParseTree Last = null;

            public FindFirstMode() { }


            public override void EnterModeSpec([NotNull] ANTLRv4Parser.ModeSpecContext context)
            {
                First = context;
            }
        }

        public class FindOptions : ANTLRv4ParserBaseListener
        {
            public IParseTree OptionsSpec = null;
            public List<IParseTree> Options = new List<IParseTree>();

            public override void EnterOption([NotNull] ANTLRv4Parser.OptionContext context)
            {
                Options.Add(context);
                base.EnterOption(context);
            }

            public override void EnterOptionsSpec([NotNull] ANTLRv4Parser.OptionsSpecContext context)
            {
                OptionsSpec = context;
                base.EnterOptionsSpec(context);
            }
        }

        public class ExtractRules : ANTLRv4ParserBaseListener
        {
            public List<ANTLRv4Parser.ParserRuleSpecContext> ParserRules = new List<ANTLRv4Parser.ParserRuleSpecContext>();
            public List<ANTLRv4Parser.LexerRuleSpecContext> LexerRules = new List<ANTLRv4Parser.LexerRuleSpecContext>();
            public List<IParseTree> Rules = new List<IParseTree>();
            public List<ITerminalNode> LhsSymbol = new List<ITerminalNode>();
            public Dictionary<ITerminalNode, List<ITerminalNode>> RhsSymbols = new Dictionary<ITerminalNode, List<ITerminalNode>>();
            private ITerminalNode current_nonterminal;

            public override void EnterParserRuleSpec([NotNull] ANTLRv4Parser.ParserRuleSpecContext context)
            {
                ParserRules.Add(context);
                Rules.Add(context);
                ITerminalNode rule_ref = context.RULE_REF();
                LhsSymbol.Add(rule_ref);
                current_nonterminal = rule_ref;
                RhsSymbols[current_nonterminal] = new List<ITerminalNode>();
            }

            public override void EnterLexerRuleSpec([NotNull] ANTLRv4Parser.LexerRuleSpecContext context)
            {
                LexerRules.Add(context);
                Rules.Add(context);
                ITerminalNode token_ref = context.TOKEN_REF();
                LhsSymbol.Add(token_ref);
                current_nonterminal = token_ref;
                RhsSymbols[current_nonterminal] = new List<ITerminalNode>();
            }

            public override void EnterRuleref([NotNull] ANTLRv4Parser.RulerefContext context)
            {
                RhsSymbols[current_nonterminal].Add(context.GetChild(0) as ITerminalNode);
            }
        }

        public class ExtractModes : ANTLRv4ParserBaseListener
        {
            public List<ANTLRv4Parser.ModeSpecContext> Modes = new List<ANTLRv4Parser.ModeSpecContext>();

            public override void EnterModeSpec([NotNull] ANTLRv4Parser.ModeSpecContext context)
            {
                Modes.Add(context);
            }
        }

        public class FindCalls : CSharpSyntaxWalker
        {
            public List<string> Invocations = new List<string>();

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                var t = node.GetText().ToString();
                if (t.IndexOf("context.") == -1)
                {
                    Invocations.Add(t);
                }
                base.VisitInvocationExpression(node);
            }
        }

        public static Dictionary<string, SyntaxTree> ReadCsharpSource(Document document)
        {
            Dictionary<string, SyntaxTree> trees = new Dictionary<string, SyntaxTree>();
            string g4_file_path = document.FullPath;
            string current_dir = Path.GetDirectoryName(g4_file_path);
            if (current_dir == null)
            {
                return trees;
            }
            foreach (string f in Directory.EnumerateFiles(current_dir))
            {
                if (Path.GetExtension(f).ToLower() != ".cs")
                {
                    continue;
                }

                string file_name = f;
                string suffix = Path.GetExtension(file_name);
                if (suffix != ".cs")
                {
                    continue;
                }

                try
                {
                    string ffn = file_name;
                    StreamReader sr = new StreamReader(ffn);
                    string code = sr.ReadToEnd();
                    SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
                    trees[ffn] = tree;
                }
                catch (Exception)
                {
                }
            }
            return trees;
        }

        public class TableOfRules
        {
            public class Row
            {
                public IParseTree rule;
                public string LHS;
                public List<string> RHS;
                public int start_index;
                public int end_index;
                public bool is_start;
                public bool is_used;
                public bool is_parser_rule;
            }

            public List<Row> rules = new List<Row>();
            public Dictionary<string, int> nt_to_index = new Dictionary<string, int>();
            public ExtractRules listener;
            private readonly ParsingResults pd_parser;
            private readonly Document document;
            private readonly Dictionary<string, SyntaxTree> trees;

            public TableOfRules(ParsingResults p, Document d)
            {
                pd_parser = p;
                document = d;
                trees = ReadCsharpSource(document);
            }

            public TableOfRules() { }

            public void ReadRules(IParseTree tree = null)
            {
                tree = tree == null ? pd_parser.ParseTree : tree;
                // Get rules, lhs, rhs.
                listener = new ExtractRules();
                ParseTreeWalker.Default.Walk(listener, tree);
                List<ITerminalNode> nonterminals = listener.LhsSymbol;
                Dictionary<ITerminalNode, List<ITerminalNode>> rhs = listener.RhsSymbols;
                for (int i = 0; i < listener.Rules.Count; ++i)
                {
                    rules.Add(new Row()
                    {
                        rule = listener.Rules[i],
                        LHS = nonterminals[i].GetText(),
                        is_parser_rule = char.IsLower(nonterminals[i].GetText()[0]),
                        RHS = rhs[nonterminals[i]].Select(t => t.GetText()).ToList(),
                    });
                }
                for (int i = 0; i < rules.Count; ++i)
                {
                    string t = rules[i].LHS;
                    nt_to_index[t] = i;
                }
            }

            public void FindPartitions()
            {
                FindFirstRule find_first_rule = new FindFirstRule();
                ParseTreeWalker.Default.Walk(find_first_rule, pd_parser.ParseTree);
                IParseTree first_rule = find_first_rule.First;
                if (first_rule == null)
                {
                    return;
                }

                string old_code = document.Code;
                for (int i = 0; i < rules.Count; ++i)
                {
                    IParseTree rule = rules[i].rule;
                    // Find range indices for rule including comments. Note, start index is inclusive; end
                    // index is exclusive. We make the assumption
                    // that the preceeding whitespace and comments are grouped with a rule all the way
                    // from the end a previous non-whitespace or comment, such as options, headers, or rule.
                    Interval token_interval = rule.SourceInterval;
                    int end = token_interval.b;
                    Antlr4.Runtime.IToken end_tok = pd_parser.TokStream.Get(end);
                    Antlr4.Runtime.IToken last = end_tok;
                    int end_ind = old_code.Length <= last.StopIndex ? last.StopIndex : last.StopIndex + 1;
                    for (int j = end_ind; j < old_code.Length; j++)
                    {
                        if (old_code[j] == '\r')
                        {
                            if (j + 1 < old_code.Length && old_code[j + 1] == '\n')
                            {
                                end_ind = j + 2;
                            }
                            else
                            {
                                end_ind = j + 1;
                            }

                            break;
                        }
                        end_ind = j;
                    }
                    int start = token_interval.a;
                    Antlr4.Runtime.IToken start_tok = pd_parser.TokStream.Get(start);
                    int start_ind = start_tok.StartIndex;
                    rules[i].start_index = start_ind;
                }
                for (int i = 0; i < rules.Count; ++i)
                {
                    if (i > 0)
                    {
                        rules[i-1].end_index = rules[i].start_index-1;
                    }
                }
                rules[rules.Count-1].end_index = old_code.Length-1;
            }

            public void FindModePartitions()
            {
                FindFirstMode find_first_mode = new FindFirstMode();
                ParseTreeWalker.Default.Walk(find_first_mode, pd_parser.ParseTree);
                IParseTree first_rule = find_first_mode.First;
                if (first_rule == null)
                {
                    return;
                }

                string old_code = document.Code;
                for (int i = 0; i < rules.Count; ++i)
                {
                    IParseTree rule = rules[i].rule;
                    // Find range indices for rule including comments. Note, start index is inclusive; end
                    // index is exclusive. We make the assumption
                    // that the preceeding whitespace and comments are grouped with a rule all the way
                    // from the end a previous non-whitespace or comment, such as options, headers, or rule.
                    Interval token_interval = rule.SourceInterval;
                    int end = token_interval.b;
                    Antlr4.Runtime.IToken end_tok = pd_parser.TokStream.Get(end);
                    Antlr4.Runtime.IToken last = end_tok;
                    int end_ind = old_code.Length <= last.StopIndex ? last.StopIndex : last.StopIndex + 1;
                    for (int j = end_ind; j < old_code.Length; j++)
                    {
                        if (old_code[j] == '\r')
                        {
                            if (j + 1 < old_code.Length && old_code[j + 1] == '\n')
                            {
                                end_ind = j + 2;
                            }
                            else
                            {
                                end_ind = j + 1;
                            }

                            break;
                        }
                        end_ind = j;
                    }
                    int start = token_interval.a;
                    Antlr4.Runtime.IToken start_tok = pd_parser.TokStream.Get(start);
                    int start_ind = start_tok.StartIndex;
                    rules[i].start_index = start_ind;
                    rules[i].end_index = end_ind;
                }
                for (int i = 0; i < rules.Count; ++i)
                {
                    if (i > 0)
                    {
                        rules[i].start_index = rules[i - 1].end_index;
                    }
                }
                for (int i = 0; i < rules.Count; ++i)
                {
                    for (int j = rules[i].start_index; j < rules[i].end_index; ++j)
                    {
                        if (old_code[j] == '\r')
                        {
                            if (j + 1 < rules[i].end_index && old_code[j + 1] == '\n')
                            {
                                ;
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }

            public void FindStartRules(List<IParseTree> identified_start_rules = null)
            {
                for (int i = 0; i < rules.Count; ++i)
                {
                    for (int j = 0; j < rules[i].RHS.Count; ++j)
                    {
                        string rhs_symbol = rules[i].RHS[j];
                        if (nt_to_index.ContainsKey(rhs_symbol))
                            rules[nt_to_index[rules[i].RHS[j]]].is_used = true;
                    }
                }
                try
                {
                    if (identified_start_rules == null)
                    {
                        foreach (KeyValuePair<string, SyntaxTree> kvp in trees)
                        {
                            string file_name = kvp.Key;
                            SyntaxTree tree = kvp.Value;
                            CompilationUnitSyntax root = (CompilationUnitSyntax)tree.GetRoot();
                            if (root == null)
                            {
                                continue;
                            }
                            FindCalls syntax_walker = new FindCalls();
                            syntax_walker.Visit(root);
                            for (int i = 0; i < rules.Count; ++i)
                            {
                                string nt_name = rules[i].LHS;
                                string call = "." + nt_name + "()";
                                foreach (string j in syntax_walker.Invocations)
                                {
                                    if (j.Contains(call))
                                    {
                                        rules[i].is_used = true;
                                        rules[i].is_start = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var p in identified_start_rules)
                        {
                            if (p is TerminalNodeImpl)
                            {
                                var id = p.GetText();
                                if (nt_to_index.ContainsKey(id))
                                {
                                    rules[nt_to_index[id]].is_start = true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public class TableOfModes
        {
            public class Row
            {
                public ANTLRv4Parser.ModeSpecContext mode;
                public string name;
                public int start_index;
                public int end_index;
            }

            public List<Row> modes = new List<Row>();
            public Dictionary<string, int> name_to_index = new Dictionary<string, int>();
            public ExtractModes listener;
            private readonly ParsingResults pd_parser;
            private readonly Document document;

            public TableOfModes(ParsingResults p, Document d)
            {
                pd_parser = p;
                document = d;
            }

            public void ReadModes()
            {
                // Get modes.
                listener = new ExtractModes();
                ParseTreeWalker.Default.Walk(listener, pd_parser.ParseTree);
                for (int i = 0; i < listener.Modes.Count; ++i)
                {
                    modes.Add(new Row()
                    {
                        mode = listener.Modes[i],
                        name = listener.Modes[i].identifier().GetText(),
                    });
                }
                for (int i = 0; i < modes.Count; ++i)
                {
                    string t = modes[i].name;
                    name_to_index[t] = i;
                }
            }

            public void FindPartitions()
            {
                FindFirstMode find_first_mode = new FindFirstMode();
                ParseTreeWalker.Default.Walk(find_first_mode, pd_parser.ParseTree);
                IParseTree first_rule = find_first_mode.First;
                if (first_rule == null)
                {
                    return;
                }

                string old_code = document.Code;
                for (int i = 0; i < modes.Count; ++i)
                {
                    var mode = modes[i].mode;
                    // Find range indices for modes including comments. Note, start index is inclusive; end
                    // index is exclusive. We make the assumption
                    // that the preceeding whitespace and comments are grouped with a rule all the way
                    // from the end a previous non-whitespace or comment, such as options, headers, or rule.
                    Interval token_interval = mode.SourceInterval;
                    int end = token_interval.b;
                    Antlr4.Runtime.IToken end_tok = pd_parser.TokStream.Get(end);
                    Antlr4.Runtime.IToken last = end_tok;
                    int end_ind = old_code.Length <= last.StopIndex ? last.StopIndex : last.StopIndex + 1;
                    for (int j = end_ind; j < old_code.Length; j++)
                    {
                        if (old_code[j] == '\r')
                        {
                            if (j + 1 < old_code.Length && old_code[j + 1] == '\n')
                            {
                                end_ind = j + 2;
                            }
                            else
                            {
                                end_ind = j + 1;
                            }

                            break;
                        }
                        end_ind = j;
                    }
                    int start = token_interval.a;
                    Antlr4.Runtime.IToken start_tok = pd_parser.TokStream.Get(start);
                    int start_ind = start_tok.StartIndex;
                    modes[i].start_index = start_ind;
                    modes[i].end_index = end_ind;
                }
                for (int i = 0; i < modes.Count; ++i)
                {
                    if (i > 0)
                    {
                        modes[i].start_index = modes[i - 1].end_index;
                    }
                }
                for (int i = 0; i < modes.Count; ++i)
                {
                    for (int j = modes[i].start_index; j < modes[i].end_index; ++j)
                    {
                        if (old_code[j] == '\r')
                        {
                            if (j + 1 < modes[i].end_index && old_code[j + 1] == '\n')
                            {
                                ;
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
        }

        public static bool IsOverlapping(int x1, int x2, int y1, int y2)
        {
            return Math.Max(x1, y1) <= Math.Min(x2, y2);
        }

        public static bool IsContainedBy(int x1, int x2, int y1, int y2)
        {
            return y1 >= x1 && y2 <= x2;
        }

        public static void Output(StringBuilder sb, CommonTokenStream stream, IParseTree tree)
        {
            if (tree as TerminalNodeImpl != null)
            {
                TerminalNodeImpl tok = tree as TerminalNodeImpl;
                if (tok.Symbol.Type == TokenConstants.EOF)
                    return;
                sb.Append(" " + tok.GetText());
            }
            else
            {
                for (int i = 0; i < tree.ChildCount; ++i)
                {
                    var c = tree.GetChild(i);
                    Output(sb, stream, c);
                }
            }
        }

        public static Dictionary<string, string> ReplaceLiterals(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Find rewrite rules, i.e., string literal to string symbol name.
            Dictionary<string, string> subs = new Dictionary<string, string>();

            var stack = new Stack<Document>();
            var workspace = document.Workspace;
            stack.Push(document);
            while (stack.Any())
            {
                var doc = stack.Pop();
                if (!(ParsingResultsFactory.Create(doc) is ParsingResults pd_doc))
                    continue;

                foreach (var c in pd_doc.Imports)
                {
                    Workspaces.Document d = workspace.FindDocument(c);
                    if (d == null)
                    {
                        continue;
                    }
                    stack.Push(d);
                }

                // Find literals in lexer rules.
                var (tree, parser, lexer) = (pd_doc.ParseTree, pd_doc.Parser, pd_doc.Lexer);
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var dom_literals = engine.parseExpression(
                            @"//ruleSpec
                        /lexerRuleSpec
                            /lexerRuleBlock
                                /lexerAltList[not(@ChildCount > 1)]
                                    /lexerAlt
                                        /lexerElements[not(@ChildCount > 1)]
                                            /lexerElement[not(@ChildCount > 1)]
                                                /lexerAtom
                                                    /terminal[not(@ChildCount > 1)]
                                                        /STRING_LITERAL",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToArray();
                    if (dom_literals.Length == 0) continue;
                    var old_names = dom_literals.Select(x => x.AntlrIParseTree.GetText()).ToList();
                    var new_names = engine.parseExpression(
                            "../../../../../../../../TOKEN_REF",
                            new StaticContextBuilder()).evaluate(dynamicContext, dom_literals)
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree.GetText()).ToArray();
                    for (int i = 0; i < old_names.Count; ++i) subs.Add(old_names[i], new_names[i]);
                }
            }

            // Find string literals in parser and combined grammars and substitute.

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            TreeEdits.Replace(pd_parser.ParseTree,
                (in IParseTree n, out bool c) =>
                {
                    c = true;
                    if (!(n is TerminalNodeImpl))
                    {
                        return null;
                    }
                    var t = n as TerminalNodeImpl;
                    if (t.Payload.Type != ANTLRv4Lexer.STRING_LITERAL)
                    {
                        return null;
                    }
                    bool no = false;
                    // Make sure this literal does not appear in lexer rule.
                    for (IRuleNode p = t.Parent; p != null; p = p.Parent)
                    {
                        if (p is ANTLRv4Parser.LexerRuleSpecContext)
                        {
                            no = true;
                            break;
                        }
                    }
                    if (no)
                    {
                        return null;
                    }
                    subs.TryGetValue(t.GetText(), out string value);
                    if (value == null)
                    {
                        return null;
                    }
                    var token = new CommonToken(ANTLRv4Lexer.TOKEN_REF) { Line = -1, Column = -1, Text = value };
                    var new_sym = new TerminalNodeImpl(token);
                    text_before.TryGetValue(t, out string v);
                    if (v != null)
                        text_before.Add(new_sym, v);
                    return new_sym;
                });
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        public static Dictionary<string, string> Foldlit(List<IParseTree> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is Antlr4ParsingResults pd_parser))
                return result;
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Verify the defs for all LHS symbols.
            // Find rewrite rules, i.e., string literal to string symbol name.
            Dictionary<string, string> subs = new Dictionary<string, string>();
            var stack = new Stack<Document>();
            var workspace = document.Workspace;
            var docs = new HashSet<Document>();
            stack.Push(document);
            while (stack.Any())
            {
                var doc = stack.Pop();
                if (!(ParsingResultsFactory.Create(doc) is ParsingResults pd_doc)) continue;
                if (docs.Contains(doc)) continue;
                docs.Add(doc);
                foreach (var c in pd_doc.Imports)
                {
                    Workspaces.Document d = workspace.FindDocument(c);
                    if (d == null)
                    {
                        continue;
                    }
                    stack.Push(d);
                }
                _ = Antlr4ParsingResults.InverseImports.TryGetValue(doc.FullPath, out HashSet<string> inverse);
                if (inverse != null)
                {
                    foreach (var c in inverse)
                    {
                        Workspaces.Document d = workspace.FindDocument(c);
                        if (d == null)
                        {
                            continue;
                        }
                        stack.Push(d);
                    }
                }
            }
            // Grab lexer or combined grammar.
            Document lexer_or_combined = null;
            IParseTree tree = null;
            Parser parser = null;
            Lexer lexer = null;
            foreach (var d in docs)
            {
                ExtractGrammarType x = new ExtractGrammarType();
                ParseTreeWalker.Default.Walk(x, d.ParseTree);
                bool got = x.Type == ExtractGrammarType.GrammarType.Combined
                    || x.Type == ExtractGrammarType.GrammarType.Lexer;
                if (got) {
                    var xxxx = ParsingResultsFactory.Create(d) as ParsingResults;
                    lexer_or_combined = d;
                    parser = xxxx.Parser;
                    lexer = xxxx.Lexer;
                    tree = d.ParseTree;
                    break;
                }
            }
            if (lexer_or_combined == null)
            {
                throw new LanguageServerException("Need a lexer or combined grammar. Where is it?");
            }
            // Filter lexer rules for criteria for an acceptable replacement.
            List<string> old_names = new List<string>();
            List<string> new_names = new List<string>();
            var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
            AntlrElement[] dom_literals;
            if (nodes == null)
            {
                var pr = ParsingResultsFactory.Create(lexer_or_combined);
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    nodes = engine.parseExpression(
                        @"//lexerRuleSpec/TOKEN_REF",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                }
            }
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                dom_literals = engine.parseExpression(
                        @"//lexerRuleSpec
                            /lexerRuleBlock
                                /lexerAltList[not(@ChildCount > 1)]
                                    /lexerAlt
                                        /lexerElements[not(@ChildCount > 1)]
                                            /lexerElement[not(@ChildCount > 1)]
                                                /lexerAtom
                                                    /terminal[not(@ChildCount > 1)]
                                                        /STRING_LITERAL",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToArray();
                if (dom_literals.Length == 0) return result;

                var allowable_lexer_rules = engine.parseExpression(
                        "../../../../../../../../TOKEN_REF",
                        new StaticContextBuilder()).evaluate(dynamicContext, dom_literals)
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                for (int i = 0; i < allowable_lexer_rules.Count; ++i)
                {
                    var l = allowable_lexer_rules[i];
                    var t = l.GetText();
                    if (nodes.Contains(l))
                    {
                        if (!subs.ContainsKey(dom_literals[i].AntlrIParseTree.GetText()))
                            subs.Add(dom_literals[i].AntlrIParseTree.GetText(), t);
                        else
                            System.Console.Error.WriteLine("Warning: multiple rules for RHS string "
                                + dom_literals[i].AntlrIParseTree.GetText() + ". Rule "
                                + t + " ignored, using "
                                + subs[dom_literals[i].AntlrIParseTree.GetText()]
                                + " instead.");
                    }
                }
            }

            // Compute a list of files that are related to each other.
            if (!docs.Any())
            {
                throw new LanguageServerException("Got no docs for the request!");
            }
            foreach (var doc in docs)
            {
                if (!(ParsingResultsFactory.Create(doc) is ParsingResults pd_doc))
                    continue;

                // Get all intertoken text immediately for source reconstruction.
                var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_doc.TokStream, pd_doc.ParseTree);

                // Find string literals in parser and combined grammars and substitute.
                TreeEdits.Replace(pd_doc.ParseTree,
                    (in IParseTree n, out bool c) =>
                    {
                        c = true;
                        if (!(n is TerminalNodeImpl))
                        {
                            return null;
                        }
                        var t = n as TerminalNodeImpl;
                        if (t.Payload.Type != ANTLRv4Lexer.STRING_LITERAL)
                        {
                            return null;
                        }
                        bool no = false;
                        // Make sure this literal does not appear in lexer rule.
                        for (IRuleNode p = t.Parent; p != null; p = p.Parent)
                        {
                            if (p is ANTLRv4Parser.LexerRuleSpecContext)
                            {
                                no = true;
                                break;
                            }
                        }
                        if (no)
                        {
                            return null;
                        }
                        subs.TryGetValue(t.GetText(), out string value);
                        if (value == null)
                        {
                            return null;
                        }
                        var token = new CommonToken(ANTLRv4Lexer.TOKEN_REF) { Line = -1, Column = -1, Text = value };
                        var new_sym = new TerminalNodeImpl(token);
                        text_before.TryGetValue(t, out string v);
                        if (v != null)
                            text_before.Add(new_sym, v);
                        return new_sym;
                    });
                StringBuilder sb = new StringBuilder();
                TreeEdits.Reconstruct(sb, pd_doc.ParseTree, text_before);
                var new_code = sb.ToString();
                if (new_code != pd_doc.Code)
                {
                    result[doc.FullPath] = new_code;
                }
            }
            return result;
        }

        public static Dictionary<string, string> RemoveUselessParserProductions(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if lexer grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }
            ExtractGrammarType lp = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp, pd_parser.ParseTree);
            bool is_lexer = lp.Type == ExtractGrammarType.GrammarType.Lexer;
            if (is_lexer)
            {
                throw new LanguageServerException("A parser or combined grammar file is not selected. Please select one first.");
            }

            // Consider only the target grammar.
            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            List<IParseTree> deletions = new List<IParseTree>();
            foreach (TableOfRules.Row r in table.rules)
            {
                if (r.is_parser_rule && r.is_used == false)
                {
                    deletions.Add(r.rule);
                }
            }
            if (! deletions.Any())
            {
                return result;
            }
            
            ANTLRv4Parser.RulesContext rules;

            {
                var first = deletions.First();
                var rule = first as ANTLRv4Parser.ParserRuleSpecContext;
                var rs = rule.Parent as ANTLRv4Parser.RuleSpecContext;
                rules = rs.Parent as ANTLRv4Parser.RulesContext;
            }

            for (int i = rules.ChildCount - 1; i >= 0; --i)
            {
                if (!deletions.Select(r => r.Parent as ANTLRv4Parser.RuleSpecContext).Contains(rules.children[i]))
                    continue;
                var rule = rules.children[i];
                var first_token = TreeEdits.LeftMostToken(rule);
                var prior_text = text_before[first_token];
                var last_token = TreeEdits.RightMostToken(rule);
                var next_token = TreeEdits.NextToken(last_token);
                if (next_token == null) continue; // EOF
                var next_token_prior_text = text_before[next_token];
                text_before[next_token] = prior_text + next_token_prior_text;
            }

            for (int i = rules.ChildCount - 1; i >= 0; --i)
            {
                if (deletions.Select(r => r.Parent as ANTLRv4Parser.RuleSpecContext).Contains(rules.children[i]))
                {
                    rules.children.RemoveAt(i);
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> ReorderParserRules(Document document, LspAntlr.ReorderType type, List<IParseTree> identified_start_rules = null)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if lexer grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType lp = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp, pd_parser.ParseTree);
            bool is_lexer = lp.Type == ExtractGrammarType.GrammarType.Lexer;
            if (is_lexer)
            {
                throw new LanguageServerException("A parser or combined grammar file is not selected. Reordering only applies to grammars that contain parser rules; lexer rules cannot be reordered. Please select one first.");
            }

            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules(identified_start_rules);

            // Find new order of rules.
            string old_code = document.Code;
            List<IParseTree> reorder = new List<IParseTree>();
            if (type == LspAntlr.ReorderType.Alphabetically)
            {
                List<string> ordered = table.rules
                    .Where(r => r.is_parser_rule)
                    .Select(r => r.LHS)
                    .OrderBy(r => r).ToList();
                foreach (string s in ordered)
                {
                    TableOfRules.Row row = table.rules[table.nt_to_index[s]];
                    reorder.Add(row.rule);
                }
            }
            else
            {
                Digraph<string> graph = new Digraph<string>();
                foreach (TableOfRules.Row r in table.rules)
                {
                    if (!r.is_parser_rule)
                    {
                        continue;
                    }
                    graph.AddVertex(r.LHS);
                }
                foreach (TableOfRules.Row r in table.rules)
                {
                    if (!r.is_parser_rule)
                    {
                        continue;
                    }
                    List<string> j = r.RHS;
                    //j.Reverse();
                    foreach (string rhs in j)
                    {
                        TableOfRules.Row sym = table.rules.Where(t => t.LHS == rhs).FirstOrDefault();
                        if (!sym.is_parser_rule)
                        {
                            continue;
                        }

                        DirectedEdge<string> e = new DirectedEdge<string>() { From = r.LHS, To = rhs };
                        graph.AddEdge(e);
                    }
                }
                List<string> starts = new List<string>();
                foreach (TableOfRules.Row r in table.rules)
                {
                    if (r.is_parser_rule && r.is_start)
                    {
                        starts.Add(r.LHS);
                    }
                }
                List<string> ordered;
                if (type == LspAntlr.ReorderType.DFS)
                {
                    Algorithms.DepthFirstOrder<string, DirectedEdge<string>> sort = new DepthFirstOrder<string, DirectedEdge<string>>(graph, starts);
                    ordered = sort.ToList();
                }
                else if (type == LspAntlr.ReorderType.BFS)
                {
                    Algorithms.BreadthFirstOrder<string, DirectedEdge<string>> sort = new BreadthFirstOrder<string, DirectedEdge<string>>(graph, starts);
                    ordered = sort.ToList();
                }
                else
                {
                    return result;
                }
                foreach (string s in ordered)
                {
                    TableOfRules.Row row = table.rules[table.nt_to_index[s]];
                    reorder.Add(row.rule);
                }
            }
            foreach (TableOfRules.Row r in table.rules)
            {
                if (r.is_parser_rule) continue;
                reorder.Add(r.rule);
            }
            bool has_new_order = false;
            for (int i = 0; i < table.rules.Count; ++i)
            {
                TableOfRules.Row r = table.rules[i];
                if (reorder[i] != r.rule)
                {
                    has_new_order = true;
                    break;
                }
            }
            if (!has_new_order)
            {
                // No changes, no error.
                return result;
            }

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            ANTLRv4Parser.RulesContext rules;
            {
                var first = reorder.First();
                var rule = first as ANTLRv4Parser.ParserRuleSpecContext;
                if (rule != null)
                {
                    var rs = rule.Parent as ANTLRv4Parser.RuleSpecContext;
                    rules = rs.Parent as ANTLRv4Parser.RulesContext;
                    rules.children = reorder.Select(t => t.Parent).ToArray();
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> SplitGrammar(Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType lp = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp, pd_parser.ParseTree);
            if (lp.Type != ExtractGrammarType.GrammarType.Combined)
            {
                throw new LanguageServerException("A combined grammar file is not selected. Please select one first.");
            }

            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();

            string old_code = document.Code;
            {
                // Create a parser and lexer grammar.
                StringBuilder sb_parser = new StringBuilder();
                StringBuilder sb_lexer = new StringBuilder();
                if (!(pd_parser.ParseTree is ANTLRv4Parser.GrammarSpecContext root))
                {
                    return result;
                }

                ANTLRv4Parser.GrammarDeclContext grammar_type_tree = root.grammarDecl();
                ANTLRv4Parser.IdentifierContext id = grammar_type_tree.identifier();
                ITerminalNode semi_tree = grammar_type_tree.SEMI();
                ANTLRv4Parser.RulesContext rules_tree = root.rules();
                string pre = old_code.Substring(0, pd_parser.TokStream.Get(grammar_type_tree.SourceInterval.a).StartIndex - 0);
                sb_parser.Append(pre);
                sb_lexer.Append(pre);
                sb_parser.Append("parser grammar " + id.GetText() + "Parser;" + Environment.NewLine);
                sb_lexer.Append("lexer grammar " + id.GetText() + "Lexer;" + Environment.NewLine);
                int x1 = pd_parser.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                int x2 = pd_parser.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                string n1 = old_code.Substring(x1, x2 - x1);
                sb_parser.Append(n1);
                sb_lexer.Append(n1);
                sb_parser.AppendLine("options { tokenVocab=" + id.GetText() + "Lexer; }");
                int end = 0;
                for (int i = 0; i < table.rules.Count; ++i)
                {
                    TableOfRules.Row r = table.rules[i];
                    // Partition rule symbols.
                    if (r.is_parser_rule)
                    {
                        string n2 = old_code.Substring(r.start_index, r.end_index - r.start_index + 1);
                        sb_parser.Append(n2);
                    }
                    else
                    {
                        string n2 = old_code.Substring(r.start_index, r.end_index - r.start_index + 1);
                        sb_lexer.Append(n2);
                    }
                    end = r.end_index + 1;
                }
                if (end < old_code.Length)
                {
                    string rest = old_code.Substring(end);
                    sb_parser.Append(rest);
                    sb_lexer.Append(rest);
                }
                string g4_file_path = document.FullPath;
                string current_dir = Path.GetDirectoryName(g4_file_path);
                if (current_dir == null)
                {
                    return result;
                }
                string orig_name = Path.GetFileNameWithoutExtension(g4_file_path);
                string new_code_parser = sb_parser.ToString();
                string new_parser_ffn = current_dir + Path.DirectorySeparatorChar
                    + orig_name + "Parser.g4";
                string new_lexer_ffn = current_dir + Path.DirectorySeparatorChar
                    + orig_name + "Lexer.g4";
                string new_code_lexer = sb_lexer.ToString();
                new_code_lexer = CleanupNewlines(new_code_lexer);
                new_code_parser = CleanupNewlines(new_code_parser);
                result.Add(new_parser_ffn, new_code_parser);
                result.Add(new_lexer_ffn, new_code_lexer);
                result.Add(g4_file_path, null);
            }

            return result;
        }

        private static string CleanupNewlines(string str)
        {
            Regex regex1 = new Regex("(?<![\r])[\n]");
            return regex1.Replace(str, "\r\n");
        }

        public static Dictionary<string, string> CombineGrammars(Document document1, Document document2)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!(ParsingResultsFactory.Create(document1) is ParsingResults pd_parser1))
                throw new LanguageServerException("Please select two grammar files.");
            if (!(ParsingResultsFactory.Create(document2) is ParsingResults pd_parser2))
                throw new LanguageServerException("Please select two grammar files.");

            // Make sure the two files are lexer and parser, no particular order.
            ExtractGrammarType lp1 = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp1, pd_parser1.ParseTree);
            if (!(lp1.Type == ExtractGrammarType.GrammarType.Parser || lp1.Type == ExtractGrammarType.GrammarType.Lexer))
            {
                throw new LanguageServerException("Grammar "
                    + document1.FullPath + " is not a parser grammar, nor lexer grammar.");
            }
            ExtractGrammarType lp2 = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp2, pd_parser2.ParseTree);
            if (!(lp2.Type == ExtractGrammarType.GrammarType.Parser || lp2.Type == ExtractGrammarType.GrammarType.Lexer))
            {
                throw new LanguageServerException("Grammar "
                    + document2.FullPath + " is not a parser grammar, nor lexer grammar.");
            }

            // Make sure parser is doc1, lexer is doc2.
            if (lp1.Type == ExtractGrammarType.GrammarType.Lexer)
            {
                // Swap.
                var s = document1;
                document1 = document2;
                document2 = s;
                var s1 = pd_parser1;
                pd_parser1 = pd_parser2;
                pd_parser2 = s1;
            }

            TableOfRules table1 = new TableOfRules(pd_parser1, document1);
            table1.ReadRules();
            table1.FindPartitions();
            table1.FindStartRules();

            string old_code1 = document1.Code;

            TableOfRules table2 = new TableOfRules(pd_parser2, document2);
            table2.ReadRules();
            table2.FindPartitions();
            table2.FindStartRules();

            string old_code2 = document2.Code;

            // Look for tokenVocab.
            FindOptions find_options = new FindOptions();
            ParseTreeWalker.Default.Walk(find_options, pd_parser1.ParseTree);
            ANTLRv4Parser.OptionContext tokenVocab = null;
            foreach (var o in find_options.Options)
            {
                var oo = o as ANTLRv4Parser.OptionContext;
                if (oo.identifier() != null && oo.identifier().GetText() == "tokenVocab")
                {
                    tokenVocab = oo;
                }
            }
            bool remove_options_spec = tokenVocab != null && find_options.Options.Count == 1;
            bool rewrite_options_spec = tokenVocab != null;

            // Create a combined parser grammar.
            StringBuilder sb_parser = new StringBuilder();
            if (!(pd_parser1.ParseTree is ANTLRv4Parser.GrammarSpecContext root))
            {
                return null;
            }

            ANTLRv4Parser.GrammarDeclContext grammar_type_tree = root.grammarDecl();
            ANTLRv4Parser.IdentifierContext id = grammar_type_tree.identifier();
            ITerminalNode semi_tree = grammar_type_tree.SEMI();
            ANTLRv4Parser.RulesContext rules_tree = root.rules();
            string pre = old_code1.Substring(0, pd_parser1.TokStream.Get(grammar_type_tree.SourceInterval.a).StartIndex - 0);
            sb_parser.Append(pre);
            sb_parser.Append("grammar " + id.GetText().Replace("Parser", "") + ";" + Environment.NewLine);

            if (!(remove_options_spec || rewrite_options_spec))
            {
                int x1 = pd_parser1.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                int x2 = pd_parser1.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                string n1 = old_code1.Substring(x1, x2 - x1);
                sb_parser.Append(n1);
            }
            else if (remove_options_spec)
            {
                int x1 = pd_parser1.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                int x2 = pd_parser1.TokStream.Get(find_options.OptionsSpec.SourceInterval.a).StartIndex;
                int x3 = pd_parser1.TokStream.Get(find_options.OptionsSpec.SourceInterval.b).StopIndex + 1;
                int x4 = pd_parser1.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                string n1 = old_code1.Substring(x1, x2 - x1);
                sb_parser.Append(n1);
                string n3 = old_code1.Substring(x3, x4 - x3);
                sb_parser.Append(n3);
            }
            else if (rewrite_options_spec)
            {
                int x1 = pd_parser1.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                int x2 = 0;
                int x3 = 0;
                foreach (var o in find_options.Options)
                {
                    var oo = o as ANTLRv4Parser.OptionContext;
                    if (oo.identifier() != null && oo.identifier().GetText() == "tokenVocab")
                    {
                        x2 = pd_parser1.TokStream.Get(oo.SourceInterval.a).StartIndex;
                        int j;
                        for (j = oo.SourceInterval.b + 1; ; j++)
                        {
                            if (pd_parser1.TokStream.Get(j).Text == ";")
                            {
                                j++;
                                break;
                            }
                        }
                        x3 = pd_parser1.TokStream.Get(j).StopIndex + 1;
                        break;
                    }
                }
                int x4 = pd_parser1.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                string n1 = old_code1.Substring(x1, x2 - x1);
                sb_parser.Append(n1);
                string n2 = old_code1.Substring(x2, x3 - x2);
                sb_parser.Append(n2);
                string n4 = old_code1.Substring(x3, x4 - x3);
                sb_parser.Append(n4);
            }
            int end = 0;
            for (int i = 0; i < table1.rules.Count; ++i)
            {
                TableOfRules.Row r = table1.rules[i];
                if (r.is_parser_rule)
                {
                    string n2 = old_code1.Substring(r.start_index, r.end_index - r.start_index);
                    sb_parser.Append(n2);
                }
                end = r.end_index + 1;
            }
            if (end < old_code1.Length)
            {
                string rest = old_code1.Substring(end);
                sb_parser.Append(rest);
            }
            end = 0;
            string lexer_old_code = document2.Code;
            for (int i = 0; i < table2.rules.Count; ++i)
            {
                TableOfRules.Row r = table2.rules[i];
                if (!r.is_parser_rule)
                {
                    string n2 = lexer_old_code.Substring(r.start_index, r.end_index - r.start_index);
                    sb_parser.Append(n2);
                }
                end = r.end_index + 1;
            }
            if (end < lexer_old_code.Length)
            {
                string rest = lexer_old_code.Substring(end);
                sb_parser.Append(rest);
            }
            string g4_file_path = document1.FullPath;
            string current_dir = Path.GetDirectoryName(g4_file_path);
            if (current_dir == null)
            {
                return null;
            }

            string orig_name = Path.GetFileName(g4_file_path);
            string new_name = orig_name.Replace("Parser.g4", "");
            string new_code_parser = sb_parser.ToString();
            string new_parser_ffn = current_dir + Path.DirectorySeparatorChar
                + new_name + ".g4";
            result.Add(new_parser_ffn, new_code_parser);
            result.Add(pd_parser1.FullFileName, null);
            result.Add(pd_parser2.FullFileName, null);
            return result;
        }

        public static Dictionary<string, string> SplitCombineGrammars(Document document, bool split)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if lexer grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType lp = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp, pd_parser.ParseTree);
            if (split && lp.Type != ExtractGrammarType.GrammarType.Combined)
            {
                throw new LanguageServerException("A combined grammar file is not selected. Please select one first.");
            }
            if ((!split) && lp.Type != ExtractGrammarType.GrammarType.Parser)
            {
                throw new LanguageServerException("A split grammar file is not selected. Please select one first.");
            }

            var workspace = document.Workspace;
            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();

            string old_code = document.Code;
            if (split)
            {
                // Create a parser and lexer grammar.
                StringBuilder sb_parser = new StringBuilder();
                StringBuilder sb_lexer = new StringBuilder();
                if (!(pd_parser.ParseTree is ANTLRv4Parser.GrammarSpecContext root))
                {
                    return null;
                }

                ANTLRv4Parser.GrammarDeclContext grammar_type_tree = root.grammarDecl();
                ANTLRv4Parser.IdentifierContext id = grammar_type_tree.identifier();
                ITerminalNode semi_tree = grammar_type_tree.SEMI();
                ANTLRv4Parser.RulesContext rules_tree = root.rules();
                string pre = old_code.Substring(0, pd_parser.TokStream.Get(grammar_type_tree.SourceInterval.a).StartIndex - 0);
                sb_parser.Append(pre);
                sb_lexer.Append(pre);
                sb_parser.Append("parser grammar " + id.GetText() + "Parser;" + Environment.NewLine);
                sb_lexer.Append("lexer grammar " + id.GetText() + "Lexer;" + Environment.NewLine);
                int x1 = pd_parser.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                int x2 = pd_parser.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                string n1 = old_code.Substring(x1, x2 - x1);
                sb_parser.Append(n1);
                sb_lexer.Append(n1);
                sb_parser.AppendLine("options { tokenVocab=" + id.GetText() + "Lexer; }");
                int end = 0;
                for (int i = 0; i < table.rules.Count; ++i)
                {
                    TableOfRules.Row r = table.rules[i];
                    // Partition rule symbols.
                    if (r.is_parser_rule)
                    {
                        string n2 = old_code.Substring(r.start_index, r.end_index - r.start_index);
                        sb_parser.Append(n2);
                    }
                    else
                    {
                        string n2 = old_code.Substring(r.start_index, r.end_index - r.start_index);
                        sb_lexer.Append(n2);
                    }
                    end = r.end_index + 1;
                }
                if (end < old_code.Length)
                {
                    string rest = old_code.Substring(end);
                    sb_parser.Append(rest);
                    sb_lexer.Append(rest);
                }
                string g4_file_path = document.FullPath;
                string current_dir = Path.GetDirectoryName(g4_file_path);
                if (current_dir == null)
                {
                    return null;
                }
                string orig_name = Path.GetFileNameWithoutExtension(g4_file_path);
                string new_code_parser = sb_parser.ToString();
                string new_parser_ffn = current_dir + Path.DirectorySeparatorChar
                    + orig_name + "Parser.g4";
                string new_lexer_ffn = current_dir + Path.DirectorySeparatorChar
                    + orig_name + "Lexer.g4";
                string new_code_lexer = sb_lexer.ToString();
                result.Add(new_parser_ffn, new_code_parser);
                result.Add(new_lexer_ffn, new_code_lexer);
                result.Add(g4_file_path, null);
            }
            else
            {
                // Parse grammar.
                HashSet<string> read_files = new HashSet<string>
                {
                    document.FullPath
                };
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
                    int after_count = read_files.Count;
                    if (after_count == before_count)
                    {
                        break;
                    }
                }
                List<ParsingResults> grammars = new List<ParsingResults>();
                foreach (string f in read_files)
                {
                    Workspaces.Document d = workspace.FindDocument(f);
                    if (d == null)
                    {
                        continue;
                    }
                    ParsingResults x = ParsingResultsFactory.Create(d) as ParsingResults;
                    grammars.Add(x);
                }

                // I'm going to have to assume two grammars, one lexer and one parser grammar each.
                if (grammars.Count != 2)
                {
                    return null;
                }

                // Read now lexer grammar. The parser grammar was already read.
                ParsingResults pd_lexer = grammars[1];
                Workspaces.Document ldocument = workspace.FindDocument(pd_lexer.FullFileName);
                TableOfRules lexer_table = new TableOfRules(pd_lexer, ldocument);
                lexer_table.ReadRules();
                lexer_table.FindPartitions();
                lexer_table.FindStartRules();

                // Look for tokenVocab.
                FindOptions find_options = new FindOptions();
                ParseTreeWalker.Default.Walk(find_options, pd_parser.ParseTree);
                ANTLRv4Parser.OptionContext tokenVocab = null;
                foreach (var o in find_options.Options)
                {
                    var oo = o as ANTLRv4Parser.OptionContext;
                    if (oo.identifier() != null && oo.identifier().GetText() == "tokenVocab")
                    {
                        tokenVocab = oo;
                    }
                }
                bool remove_options_spec = tokenVocab != null && find_options.Options.Count == 1;
                bool rewrite_options_spec = tokenVocab != null;

                // Create a combined parser grammar.
                StringBuilder sb_parser = new StringBuilder();
                if (!(pd_parser.ParseTree is ANTLRv4Parser.GrammarSpecContext root))
                {
                    return null;
                }

                ANTLRv4Parser.GrammarDeclContext grammar_type_tree = root.grammarDecl();
                ANTLRv4Parser.IdentifierContext id = grammar_type_tree.identifier();
                ITerminalNode semi_tree = grammar_type_tree.SEMI();
                ANTLRv4Parser.RulesContext rules_tree = root.rules();
                string pre = old_code.Substring(0, pd_parser.TokStream.Get(grammar_type_tree.SourceInterval.a).StartIndex - 0);
                sb_parser.Append(pre);
                sb_parser.Append("grammar " + id.GetText().Replace("Parser", "") + ";" + Environment.NewLine);

                if (!(remove_options_spec || rewrite_options_spec))
                {
                    int x1 = pd_parser.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                    int x2 = pd_parser.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                    string n1 = old_code.Substring(x1, x2 - x1);
                    sb_parser.Append(n1);
                }
                else if (remove_options_spec)
                {
                    int x1 = pd_parser.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                    int x2 = pd_parser.TokStream.Get(find_options.OptionsSpec.SourceInterval.a).StartIndex;
                    int x3 = pd_parser.TokStream.Get(find_options.OptionsSpec.SourceInterval.b).StopIndex + 1;
                    int x4 = pd_parser.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                    string n1 = old_code.Substring(x1, x2 - x1);
                    sb_parser.Append(n1);
                    string n3 = old_code.Substring(x3, x4 - x3);
                    sb_parser.Append(n3);
                }
                else if (rewrite_options_spec)
                {
                    int x1 = pd_parser.TokStream.Get(semi_tree.SourceInterval.b).StopIndex + 1;
                    int x2 = 0;
                    int x3 = 0;
                    foreach (var o in find_options.Options)
                    {
                        var oo = o as ANTLRv4Parser.OptionContext;
                        if (oo.identifier() != null && oo.identifier().GetText() == "tokenVocab")
                        {
                            x2 = pd_parser.TokStream.Get(oo.SourceInterval.a).StartIndex;
                            int j;
                            for (j = oo.SourceInterval.b + 1; ; j++)
                            {
                                if (pd_parser.TokStream.Get(j).Text == ";")
                                {
                                    j++;
                                    break;
                                }
                            }
                            x3 = pd_parser.TokStream.Get(j).StopIndex + 1;
                            break;
                        }
                    }
                    int x4 = pd_parser.TokStream.Get(rules_tree.SourceInterval.a).StartIndex;
                    string n1 = old_code.Substring(x1, x2 - x1);
                    sb_parser.Append(n1);
                    string n2 = old_code.Substring(x2, x3 - x2);
                    sb_parser.Append(n2);
                    string n4 = old_code.Substring(x3, x4 - x3);
                    sb_parser.Append(n4);
                }
                int end = 0;
                for (int i = 0; i < table.rules.Count; ++i)
                {
                    TableOfRules.Row r = table.rules[i];
                    if (r.is_parser_rule)
                    {
                        string n2 = old_code.Substring(r.start_index, r.end_index - r.start_index);
                        sb_parser.Append(n2);
                    }
                    end = r.end_index + 1;
                }
                if (end < old_code.Length)
                {
                    string rest = old_code.Substring(end);
                    sb_parser.Append(rest);
                }
                end = 0;
                string lexer_old_code = ldocument.Code;
                for (int i = 0; i < lexer_table.rules.Count; ++i)
                {
                    TableOfRules.Row r = lexer_table.rules[i];
                    if (!r.is_parser_rule)
                    {
                        string n2 = lexer_old_code.Substring(r.start_index, r.end_index - r.start_index);
                        sb_parser.Append(n2);
                    }
                    end = r.end_index + 1;
                }
                if (end < lexer_old_code.Length)
                {
                    string rest = lexer_old_code.Substring(end);
                    sb_parser.Append(rest);
                }
                string g4_file_path = document.FullPath;
                string current_dir = Path.GetDirectoryName(g4_file_path);
                if (current_dir == null)
                {
                    return null;
                }

                string orig_name = Path.GetFileName(g4_file_path);
                string new_name = orig_name.Replace("Parser.g4", "");
                string new_code_parser = sb_parser.ToString();
                string new_parser_ffn = current_dir + Path.DirectorySeparatorChar
                    + new_name + ".g4";
                result.Add(new_parser_ffn, new_code_parser);
                result.Add(pd_parser.FullFileName, null);
                result.Add(pd_lexer.FullFileName, null);
            }

            return result;
        }

        private static bool HasDirectLeftRecursion(IParseTree rule)
        {
            if (rule is ANTLRv4Parser.ParserRuleSpecContext r)
            {
                var lhs = r.RULE_REF();
                var rb = r.ruleBlock();
                if (rb == null) return false;
                var ral = rb.ruleAltList();
                foreach (var la in ral.labeledAlt())
                {
                    if (la
                        .alternative()?
                        .element()?
                        .FirstOrDefault()?
                        .atom()?
                        .ruleref()?
                        .GetChild(0) is TerminalNodeImpl t1 && t1.GetText() == lhs.GetText())
                    {
                        return true;
                    }

                    if (la
                        .alternative()?
                        .element()?
                        .FirstOrDefault()?
                        .labeledElement()?
                        .atom()?
                        .ruleref()?
                        .GetChild(0) is TerminalNodeImpl t2 && t2.GetText() == lhs.GetText())
                    {
                        return true;
                    }
                }
            }
            else if (rule is ANTLRv4Parser.LexerRuleSpecContext r2)
            {
                var lhs = r2.TOKEN_REF();
                var rb = r2.lexerRuleBlock();
                if (rb == null) return false;
                var lal = rb.lexerAltList();
                foreach (var la in lal.lexerAlt())
                {
                    if (la
                        .lexerElements()?
                        .lexerElement()?
                        .FirstOrDefault()?
                        .lexerAtom()?
                        .terminal()?
                        .GetChild(0) is TerminalNodeImpl t1 && t1.GetText() == lhs.GetText())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool HasDirectRightRecursion(IParseTree rule)
        {
            if (rule is ANTLRv4Parser.ParserRuleSpecContext r)
            {
                var lhs = r.RULE_REF();
                var rb = r.ruleBlock();
                if (rb == null) return false;
                var ral = rb.ruleAltList();
                foreach (var la in ral.labeledAlt())
                {
                    if (la
                        .alternative()?
                        .element()?
                        .LastOrDefault()?
                        .atom()?
                        .ruleref()?
                        .GetChild(0) is TerminalNodeImpl t1 && t1.GetText() == lhs.GetText())
                    {
                        return true;
                    }
                    if (la
                        .alternative()?
                        .element()?
                        .LastOrDefault()?
                        .labeledElement()?
                        .atom()?
                        .ruleref()?
                        .GetChild(0) is TerminalNodeImpl t2 && t2.GetText() == lhs.GetText())
                    {
                        return true;
                    }
                }
            }
            else if (rule is ANTLRv4Parser.LexerRuleSpecContext r2)
            {
                var lhs = r2.TOKEN_REF();
                var rb = r2.lexerRuleBlock();
                if (rb == null) return false;
                var lal = rb.lexerAltList();
                foreach (var la in lal.lexerAlt())
                {
                    if (la
                        .lexerElements()?
                        .lexerElement()?
                        .LastOrDefault()?
                        .lexerAtom()?
                        .terminal()?
                        .GetChild(0) is TerminalNodeImpl t1 && t1.GetText() == lhs.GetText())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static List<Domemtech.Symtab.ISymbol> GetDef(IParseTree lhs, ParsingResults pd_parser)
        {
            List<Domemtech.Symtab.ISymbol> result = new List<Domemtech.Symtab.ISymbol>();
            var term = lhs as TerminalNodeImpl;
            pd_parser.Attributes.TryGetValue(term, out IList<CombinedScopeSymbol> list_value);
            if (list_value == null) return result;
            if (list_value.Count > 1) return result;
            var value = list_value.First();
            if (value == null) return result;
            Domemtech.Symtab.ISymbol sym = value as Domemtech.Symtab.ISymbol;
            if (sym == null) return result;
            result = new List<Domemtech.Symtab.ISymbol>() { sym };
            if (sym is RefSymbol) result = sym.resolve();
            return result;
        }

        private static List<TerminalNodeImpl> ChaseAlts(IParseTree node)
        {
            List<TerminalNodeImpl> result = new List<TerminalNodeImpl>();
            if (node is ANTLRv4Parser.GrammarSpecContext) { }
            else if (node is ANTLRv4Parser.GrammarDeclContext) { }
            else if (node is ANTLRv4Parser.GrammarTypeContext) { }
            else if (node is ANTLRv4Parser.PrequelConstructContext) { }
            else if (node is ANTLRv4Parser.OptionsSpecContext) { }
            else if (node is ANTLRv4Parser.OptionContext) { }
            else if (node is ANTLRv4Parser.OptionValueContext) { }
            else if (node is ANTLRv4Parser.DelegateGrammarsContext) { }
            else if (node is ANTLRv4Parser.DelegateGrammarContext) { }
            else if (node is ANTLRv4Parser.TokensSpecContext) { }
            else if (node is ANTLRv4Parser.ChannelsSpecContext) { }
            else if (node is ANTLRv4Parser.IdListContext) { }
            else if (node is ANTLRv4Parser.Action_Context) { }
            else if (node is ANTLRv4Parser.ActionScopeNameContext) { }
            else if (node is ANTLRv4Parser.ActionBlockContext) { }
            else if (node is ANTLRv4Parser.ArgActionBlockContext) { }
            else if (node is ANTLRv4Parser.ModeSpecContext) { }
            else if (node is ANTLRv4Parser.RulesContext) { }
            else if (node is ANTLRv4Parser.RuleSpecContext) { }
            else if (node is ANTLRv4Parser.ParserRuleSpecContext) { }
            else if (node is ANTLRv4Parser.ExceptionGroupContext) { }
            else if (node is ANTLRv4Parser.ExceptionHandlerContext) { }
            else if (node is ANTLRv4Parser.FinallyClauseContext) { }
            else if (node is ANTLRv4Parser.RulePrequelContext) { }
            else if (node is ANTLRv4Parser.RuleReturnsContext) { }
            else if (node is ANTLRv4Parser.ThrowsSpecContext) { }
            else if (node is ANTLRv4Parser.LocalsSpecContext) { }
            else if (node is ANTLRv4Parser.RuleActionContext) { }
            else if (node is ANTLRv4Parser.RuleModifiersContext) { }
            else if (node is ANTLRv4Parser.RuleModifierContext) { }
            else if (node is ANTLRv4Parser.RuleBlockContext n_rbc)
            {
                var c = n_rbc.ruleAltList();
                result.AddRange(ChaseAlts(c));
            }
            else if (node is ANTLRv4Parser.RuleAltListContext n_ralc)
            {
                var c = n_ralc.labeledAlt();
                foreach (var a in c)
                {
                    result.AddRange(ChaseAlts(a));
                }
            }
            else if (node is ANTLRv4Parser.LabeledAltContext n_lac)
            {
                var c = n_lac.alternative();
                result.AddRange(ChaseAlts(c));
            }
            else if (node is ANTLRv4Parser.LexerRuleSpecContext) { }
            else if (node is ANTLRv4Parser.LexerRuleBlockContext) { }
            else if (node is ANTLRv4Parser.LexerAltListContext) { }
            else if (node is ANTLRv4Parser.LexerAltContext) { }
            else if (node is ANTLRv4Parser.LexerElementsContext) { }
            else if (node is ANTLRv4Parser.LexerElementContext) { }
            else if (node is ANTLRv4Parser.LabeledLexerElementContext) { }
            else if (node is ANTLRv4Parser.LexerBlockContext) { }
            else if (node is ANTLRv4Parser.LexerCommandsContext) { }
            else if (node is ANTLRv4Parser.LexerCommandContext) { }
            else if (node is ANTLRv4Parser.LexerCommandNameContext) { }
            else if (node is ANTLRv4Parser.LexerCommandExprContext) { }
            else if (node is ANTLRv4Parser.AltListContext n_alc)
            {
                var c = n_alc.alternative();
                foreach (var a in c)
                {
                    result.AddRange(ChaseAlts(a));
                }
            }
            else if (node is ANTLRv4Parser.AlternativeContext n_ac)
            {
                var c = n_ac.element();
                if (c != null)
                {
                    result.AddRange(ChaseAlts(c[0]));
                }
            }
            else if (node is ANTLRv4Parser.ElementContext n_ec)
            {
                var le = n_ec.labeledElement();
                if (le != null)
                {
                    result.AddRange(ChaseAlts(le));
                    return result;
                }
                var a = n_ec.atom();
                if (a != null)
                {
                    result.AddRange(ChaseAlts(a));
                    return result;
                }
                var e = n_ec.ebnf();
                if (e != null)
                {
                    result.AddRange(ChaseAlts(e));
                    return result;
                }
            }
            else if (node is ANTLRv4Parser.LabeledElementContext n_lec)
            {
                var a = n_lec.atom();
                if (a != null)
                {
                    result.AddRange(ChaseAlts(a));
                    return result;
                }
                var b = n_lec.block();
                if (b != null)
                {
                    result.AddRange(ChaseAlts(b));
                    return result;
                }
            }
            else if (node is ANTLRv4Parser.EbnfContext n_ebnf_c)
            {
                var b = n_ebnf_c.block();
                if (b != null)
                {
                    result.AddRange(ChaseAlts(b));
                    return result;
                }
            }
            else if (node is ANTLRv4Parser.BlockSuffixContext) { }
            else if (node is ANTLRv4Parser.EbnfSuffixContext) { }
            else if (node is ANTLRv4Parser.LexerAtomContext) { }
            else if (node is ANTLRv4Parser.AtomContext n_atom_c)
            {
                var t = n_atom_c.terminal();
                if (t != null)
                {
                    result.AddRange(ChaseAlts(t));
                    return result;
                }
                var r = n_atom_c.ruleref();
                if (r != null)
                {
                    result.AddRange(ChaseAlts(r));
                    return result;
                }
            }
            else if (node is ANTLRv4Parser.NotSetContext) { }
            else if (node is ANTLRv4Parser.BlockSetContext) { }
            else if (node is ANTLRv4Parser.SetElementContext) { }
            else if (node is ANTLRv4Parser.BlockContext n_bc)
            {
                var altList = n_bc.altList();
                result.AddRange(ChaseAlts(altList));
            }
            else if (node is ANTLRv4Parser.RulerefContext n_rrc)
            {
                var rr = n_rrc.RULE_REF();
                result.Add(rr as TerminalNodeImpl);
            }
            else if (node is ANTLRv4Parser.CharacterRangeContext) { }
            else if (node is ANTLRv4Parser.TerminalContext) { }
            else if (node is ANTLRv4Parser.ElementOptionsContext) { }
            else if (node is ANTLRv4Parser.ElementOptionContext) { }
            else if (node is ANTLRv4Parser.IdentifierContext n_ic)
            {
                var rr = n_ic.RULE_REF();
                result.Add(rr as TerminalNodeImpl);
            }
            else throw new Exception("Invalid case.");

            return result;
        }

        private static IEnumerable<TerminalNodeImpl> RHS(TerminalNodeImpl def, Dictionary<TerminalNodeImpl, List<TerminalNodeImpl>> cache)
        {
            cache.TryGetValue(def, out List<TerminalNodeImpl> value);
            if (value != null) return value;
            var result = new List<TerminalNodeImpl>();
            if (def.Parent is ANTLRv4Parser.ParserRuleSpecContext p1)
            {
                // Gather all RULE_REF's in ruleBlock alts.
                //var stack = new Stack<IParseTree>();
                //stack.Push(p1.ruleBlock());
                //while (stack.Any())
                //{
                //    var n = stack.Pop();
                //    if (n is TerminalNodeImpl t)
                //    {
                //        result.Add(t);
                //    }
                //    else
                //    {
                //        for (int i = n.ChildCount -1; i >= 0; i--)
                //        {
                //            stack.Push(n.GetChild(i));
                //        }
                //    }
                //}
                result = ChaseAlts(p1.ruleBlock());
            }
            cache[def] = result;
            return result;
        }

        private static List<string> HasIndirectLeftRecursion(Document document, bool gen_dot, IEnumerable<IParseTree> nodes = null)
        {
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            var workspace = document.Workspace;
            var pr = ParsingResultsFactory.Create(document);
            var aparser = pr.Parser;
            var atree = pr.ParseTree;
            List<TerminalNodeImpl> all_nodes;
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                all_nodes = engine.parseExpression(
                    @"//parserRuleSpec/RULE_REF",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as TerminalNodeImpl).ToList();
            }

            Digraph<Domemtech.Symtab.ISymbol> graph = new Digraph<Domemtech.Symtab.ISymbol>();

            // Get LHS defs and refs, then perform transitive closure of all uses, all rules and their symbols.
            HashSet<Domemtech.Symtab.ISymbol> visited = new HashSet<Domemtech.Symtab.ISymbol>();
            Stack<Domemtech.Symtab.ISymbol> stack = new Stack<Domemtech.Symtab.ISymbol>();

            foreach (var node in all_nodes)
            {
                var defs = GetDef(node, pd_parser);
                foreach (var def in defs) { stack.Push(def); }
            }
            Dictionary<TerminalNodeImpl, List<TerminalNodeImpl>> cache = new Dictionary<TerminalNodeImpl, List<TerminalNodeImpl>>();
            while (stack.Any())
            {
                var def = stack.Pop();
                if (visited.Contains(def))
                    continue;
                visited.Add(def);

                // Navigate to RHS symbols and get defs. Draw edges between those defs and this def.

                string def_file = def.file;
                if (def_file == null)
                    continue;

                Document doc = workspace.FindDocument(def_file);
                if (doc == null)
                    continue;

                if (!(ParsingResultsFactory.Create(doc) is ParsingResults pd))
                    continue;

                var tree = pd.ParseTree;
                if (tree == null)
                    continue;

                var lhs = def.Token;
                if (lhs == null)
                    continue;

                TerminalNodeImpl lhs_term = TreeEdits.Find(lhs.First(), tree);
                if (lhs_term == null)
                    continue;

                if (!(lhs_term.Parent is ANTLRv4Parser.ParserRuleSpecContext || lhs_term.Parent is ANTLRv4Parser.LexerRuleSpecContext))
                    continue;

                graph.AddVertex(def);

                var rhs_symbols = RHS(lhs_term, cache);
                // Only consider first rhs symbol for now.
                if (rhs_symbols.Any())
                {
                    foreach (var rhs_sym in rhs_symbols)
                    {
                        var more_defs = GetDef(rhs_sym, pd);
                        foreach (Domemtech.Symtab.ISymbol def2 in more_defs)
                        {
                            string def_file2 = def2.file;
                            if (def_file2 == null)
                                continue;

                            Document doc2 = workspace.FindDocument(def_file2);
                            if (doc2 == null)
                                continue;

                            if (!(ParsingResultsFactory.Create(doc2) is ParsingResults pd2))
                                continue;

                            var tree2 = pd2.ParseTree;
                            if (tree2 == null)
                                continue;

                            var lhs2 = def2.Token;
                            if (lhs2 == null)
                                continue;

                            TerminalNodeImpl lhs_term2 = TreeEdits.Find(lhs2.First(), tree2);
                            if (lhs_term2 == null)
                                continue;

                            if (!(lhs_term2.Parent is ANTLRv4Parser.ParserRuleSpecContext || lhs_term.Parent is ANTLRv4Parser.LexerRuleSpecContext))
                                continue;

                            graph.AddVertex(def2);
                            DirectedEdge<Domemtech.Symtab.ISymbol> e = new DirectedEdge<Domemtech.Symtab.ISymbol>() { From = def, To = def2 };
                            graph.AddEdge(e);

                            stack.Push(def2);
                        }
                    }
                }
            }

            // Check rule and graph.
            var tarjan = new TarjanSCC<Domemtech.Symtab.ISymbol, DirectedEdge<Domemtech.Symtab.ISymbol>>(graph);
            List<Domemtech.Symtab.ISymbol> ordered = new List<Domemtech.Symtab.ISymbol>();
            IDictionary<Domemtech.Symtab.ISymbol, IEnumerable<Domemtech.Symtab.ISymbol>> sccs = tarjan.Compute();
            var cycles = sccs.Select(t => t.Value).Distinct().Where(t => t.Count() > 1);
            List<string> result = new List<string>();
            if (!cycles.Any())
            {
                return result;
            }

            if (gen_dot)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("digraph IndirectCycles {");

                foreach (var cycle in cycles)
                {
                    foreach (var c in cycle)
                    {
                        var edges = graph.SuccessorEdges(c);
                        foreach (var edge in edges)
                        {
                            if (cycle.Contains(edge.To))
                            {
                                sb.AppendLine(edge.From.Name + " -> " + edge.To.Name + ";");
                            }
                        }
                    }
                }

                // Output all other lines.

                sb.AppendLine("}");
                result = new List<string>() { sb.ToString() };
            }
            else
            {

                if (nodes == null)
                {
                    foreach (KeyValuePair<Domemtech.Symtab.ISymbol, IEnumerable<Domemtech.Symtab.ISymbol>> scc in sccs)
                    {
                        if (scc.Value.Count() > 1)
                        {
                            var s = string.Join(" ", scc.Value.Select(t => t.Name).OrderBy(t => t));
                            if (!result.Contains(s)) result.Add(s);
                        }
                    }
                }
                else
                {
                    foreach (var node in nodes)
                    {
                        var defs = GetDef(node, pd_parser);

                        foreach (var def in defs)
                        {
                            var scc = sccs[def];
                            if (scc.Count() > 1)
                            {
                                StringBuilder sb = new StringBuilder();
                                var s = string.Join(" ", scc.Select(t => t.Name).OrderBy(t => t));
                                result.Add(def.Name + " => " + sb.ToString());
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static (IParseTree, IParseTree) GenerateReplacementRules(string new_symbol_name, IParseTree rule, Dictionary<TerminalNodeImpl, string> text_before)
        {
            ANTLRv4Parser.ParserRuleSpecContext new_a_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
            {
                var r = rule as ANTLRv4Parser.ParserRuleSpecContext;
                var lhs = r.RULE_REF()?.GetText();
                {
                    TreeEdits.CopyTreeRecursive(r.RULE_REF(), new_a_rule, text_before);
                }
                // Now have "A"
                {
                    var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                    var new_colon = new TerminalNodeImpl(token2);
                    new_a_rule.AddChild(new_colon);
                    new_colon.Parent = new_a_rule;
                }
                // Now have "A :"
                ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                {
                    ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(new_a_rule, 0);
                    new_a_rule.AddChild(new_rule_block_context);
                    new_rule_block_context.Parent = new_a_rule;
                    new_rule_block_context.AddChild(rule_alt_list);
                    rule_alt_list.Parent = new_rule_block_context;
                }
                // Now have "A : <rb <ral> >"
                {
                    var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                    var new_semi = new TerminalNodeImpl(token3);
                    new_a_rule.AddChild(new_semi);
                    new_semi.Parent = new_a_rule;
                }
                // Now have "A : <rb <ral> > ;"
                {
                    TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
                }
                // Now have "A : <rb <ral> > ; <eg>"
                bool first = true;
                foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(rule))
                {
                    ANTLRv4Parser.ElementContext e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                    if (lhs == e?.GetText())
                    {
                        // skip alts that have direct left recursion.
                        continue;
                    }
                    if (lhs == e?.atom()?.GetText())
                    {
                        continue;
                    }

                    {
                        if (!first)
                        {
                            var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                            var new_or = new TerminalNodeImpl(token4);
                            rule_alt_list.AddChild(new_or);
                            new_or.Parent = rule_alt_list;
                        }
                        first = false;
                    }
                    ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                    rule_alt_list.AddChild(l_alt);
                    l_alt.Parent = rule_alt_list;
                    // Create new alt "beta A'".
                    ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                    l_alt.AddChild(new_alt);
                    new_alt.Parent = l_alt;
                    foreach (var element in alt.element())
                    {
                        TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                    }
                    var token = new CommonToken(ANTLRv4Lexer.RULE_REF) { Line = -1, Column = -1, Text = new_symbol_name };
                    var new_rule_ref = new TerminalNodeImpl(token);
                    var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                    new_ruleref.AddChild(new_rule_ref);
                    new_rule_ref.Parent = new_ruleref;
                    var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                    new_atom.AddChild(new_ruleref);
                    new_ruleref.Parent = new_atom;
                    var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                    new_element.AddChild(new_atom);
                    new_atom.Parent = new_element;
                    new_alt.AddChild(new_element);
                    new_element.Parent = new_alt;
                }
            }
            // Now have "A : beta1 A' | beta2 A' | ... ; <eg>"

            ANTLRv4Parser.ParserRuleSpecContext new_ap_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
            {
                var r = rule as ANTLRv4Parser.ParserRuleSpecContext;
                var lhs = r.RULE_REF()?.GetText();
                {
                    var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = new_symbol_name };
                    var new_rule_ref = new TerminalNodeImpl(token);
                    text_before[new_rule_ref] = System.Environment.NewLine + System.Environment.NewLine;
                    new_ap_rule.AddChild(new_rule_ref);
                    new_rule_ref.Parent = new_ap_rule;
                }
                // Now have "A'"
                {
                    var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                    var new_colon = new TerminalNodeImpl(token2);
                    new_ap_rule.AddChild(new_colon);
                    new_colon.Parent = new_ap_rule;
                }
                // Now have "A' :"
                ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                {
                    ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(new_a_rule, 0);
                    new_ap_rule.AddChild(new_rule_block_context);
                    new_rule_block_context.Parent = new_ap_rule;
                    new_rule_block_context.AddChild(rule_alt_list);
                    rule_alt_list.Parent = new_rule_block_context;
                }
                // Now have "A' : <rb <ral> >"
                {
                    var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                    var new_semi = new TerminalNodeImpl(token3);
                    new_ap_rule.AddChild(new_semi);
                    new_semi.Parent = new_ap_rule;
                }
                // Now have "A : <rb <ral> > ;"
                {
                    TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
                }
                // Now have "A' : <rb <ral> > ; <eg>"
                bool first = true;
                foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(rule))
                {
                    ANTLRv4Parser.ElementContext e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                    if (lhs != e?.GetText())
                    {
                        continue;
                    }
                    if (lhs != e?.atom()?.GetText())
                    {
                        continue;
                    }

                    {
                        if (!first)
                        {
                            var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                            var new_or = new TerminalNodeImpl(token4);
                            rule_alt_list.AddChild(new_or);
                            new_or.Parent = rule_alt_list;
                        }
                        first = false;
                    }
                    ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                    rule_alt_list.AddChild(l_alt);
                    l_alt.Parent = rule_alt_list;
                    // Create new alt "alpha A'".
                    ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                    l_alt.AddChild(new_alt);
                    new_alt.Parent = l_alt;
                    bool first2 = true;
                    foreach (var element in alt.element())
                    {
                        if (first2)
                        {
                            first2 = false;
                            continue;
                        }
                        TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                    }
                    var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = new_symbol_name };
                    var new_rule_ref = new TerminalNodeImpl(token);
                    var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                    new_ruleref.AddChild(new_rule_ref);
                    new_rule_ref.Parent = new_ruleref;
                    var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                    new_atom.AddChild(new_ruleref);
                    new_ruleref.Parent = new_atom;
                    var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                    new_element.AddChild(new_atom);
                    new_atom.Parent = new_element;
                    new_alt.AddChild(new_element);
                    new_element.Parent = new_alt;
                }
                {
                    if (!first)
                    {
                        var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                        var new_or = new TerminalNodeImpl(token4);
                        rule_alt_list.AddChild(new_or);
                        new_or.Parent = rule_alt_list;
                    }
#pragma warning disable IDE0059
                    first = false;
#pragma warning restore IDE0059
                    ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                    rule_alt_list.AddChild(l_alt);
                    l_alt.Parent = rule_alt_list;
                    // Create new empty alt.
                    ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                    l_alt.AddChild(new_alt);
                    new_alt.Parent = l_alt;
                }
            }
            // Now have "A' : alpha1 A' | alpha2 A' | ... ;"

            return ((IParseTree)new_a_rule, (IParseTree)new_ap_rule);
        }

        private static IParseTree ReplaceWithKleeneRules(Parser parser, bool has_direct_left_recursion, bool has_direct_right_recursion, IParseTree rule, Dictionary<TerminalNodeImpl, string> text_before)
        {
            // Left recursion:
            // A -> A b1 | A b2 | ... | a1 | a2 | ... ;
            // => A ->  (a1 | a2 | ... ) (b1 | b2 | ...)*;
            // Note, A on RHS cannot have any postfix operators.
            //
            // A -> A? b1 | A? b2 | ... | a1 | a2 | ...;
            // A -> A b1 | b1 | A b2 | b2 | ... | a1 | a2 | ...;
            // A -> b1 | b2 | ... | a1 | a2 | ... | A b1 | a b2 | ...; 
            // A -> ( a1 | a2 | ... | b1 | b2 | ... ) (b1 | b2 | ...)* ;
            // A on RHS must only be "A?".
            //
            // Right recursion:
            // Convert A -> b1 A | b2 A | ... | a1 | a2 | ... ;
            // into A ->   (b1 | b2 | ...)* (a1 | a2 | ... )
            //
            // A -> a1 | a2 | ... | b1 A? | b2 A? | ...;
            // A -> (b1 | b2 | ...)* (a1 | a2 | ... | b1 | b2 | ...)
            // A on RHS must only be "A?".
            //
            // First, set up the new rule, less the RHS rewrite.
            var new_a_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
            string lhs = "";
            IParseTree colon;
            if (rule is ANTLRv4Parser.ParserRuleSpecContext r)
            {
                //parserRuleSpec: ruleModifiers? RULE_REF argActionBlock? ruleReturns? throwsSpec? localsSpec? rulePrequel* COLON ruleBlock SEMI exceptionGroup;
                if (r.ruleModifiers() != null) TreeEdits.CopyTreeRecursive(r.ruleModifiers(), new_a_rule, text_before);
                TreeEdits.CopyTreeRecursive(r.RULE_REF(), new_a_rule, text_before);
                lhs = r.RULE_REF()?.GetText();
                if (r.argActionBlock() != null) TreeEdits.CopyTreeRecursive(r.argActionBlock(), new_a_rule, text_before);
                if (r.ruleReturns() != null) TreeEdits.CopyTreeRecursive(r.ruleReturns(), new_a_rule, text_before);
                if (r.throwsSpec() != null) TreeEdits.CopyTreeRecursive(r.throwsSpec(), new_a_rule, text_before);
                if (r.localsSpec() != null) TreeEdits.CopyTreeRecursive(r.localsSpec(), new_a_rule, text_before);
                foreach (var rp in r.rulePrequel()) TreeEdits.CopyTreeRecursive(rp, new_a_rule, text_before);
                colon = TreeEdits.CopyTreeRecursive(r.COLON(), new_a_rule, text_before);
                TreeEdits.CopyTreeRecursive(r.SEMI(), new_a_rule, text_before);
                if (r.exceptionGroup() != null) TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
            }
            else if (rule is ANTLRv4Parser.LexerRuleSpecContext lr)
            {
                if (lr.FRAGMENT() != null) TreeEdits.CopyTreeRecursive(lr.FRAGMENT(), new_a_rule, text_before);
                TreeEdits.CopyTreeRecursive(lr.TOKEN_REF(), new_a_rule, text_before);
                lhs = lr.TOKEN_REF()?.GetText();
                colon = TreeEdits.CopyTreeRecursive(lr.COLON(), new_a_rule, text_before);
                TreeEdits.CopyTreeRecursive(lr.SEMI(), new_a_rule, text_before);
            }
            else throw new Exception();

            // Partition the alts into two collections, one
            // contains the "alphas" and the other the "betas", which I
            // describe above.
            List<IParseTree> alphas = new List<IParseTree>();
            List<IParseTree> betas = new List<IParseTree>();
            bool q_operator = false;
            foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(rule))
            {
                ANTLRv4Parser.ElementContext e = null;
                if (has_direct_left_recursion && !has_direct_right_recursion)
                {
                    e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                }
                else
                {
                    e = EnumeratorOfRHS(alt)?.LastOrDefault();
                }
                if (lhs == e?.GetText())
                {
                    TreeEdits.Delete(e);
                    betas.Add(alt);
                }
                else if (lhs == e?.atom().GetText())
                {
                    if (e?.ebnfSuffix()?.GetText() != "?")
                        return null;
                    TreeEdits.Delete(e);
                    betas.Add(alt);
                    q_operator = true;
                }
                else
                {
                    alphas.Add(alt);
                }
            }
            // Determine if alpha list equals beta list.
            bool equal_alpha_beta = true;
            if (alphas.Count == betas.Count)
            {
                for (int jj = 0; jj < alphas.Count; ++jj)
                {
                    var r1 = alphas[jj]?.GetText()?.Trim();
                    var r2 = betas[jj]?.GetText()?.Trim();
                    if (r1 != r2)
                    {
                        equal_alpha_beta = false;
                        break;
                    }
                }
            }
            else
            {
                equal_alpha_beta = false;
            }
            // Finish up RHS
            if (equal_alpha_beta)
            {
                var container = Make("+", alphas, text_before, parser);
                TreeEdits.InsertAfter(colon, container);
            }
            else
            {
                if (has_direct_left_recursion && !has_direct_right_recursion)
                {
                    var a_container = q_operator
                            ? Make("", alphas.Concat(betas).ToList(), text_before, parser)
                            : Make("", alphas, text_before, parser);
                    var b_container = Make("*", betas, text_before, parser);
                    TreeEdits.InsertAfter(colon, b_container);
                    TreeEdits.InsertAfter(colon, a_container);
                }
                else
                {
                    var a_container = q_operator
                      ? Make("", alphas.Concat(betas).ToList(), text_before, parser)
                      : Make("", alphas, text_before, parser);
                    var b_container = Make("*", betas, text_before, parser);
                    TreeEdits.InsertAfter(colon, a_container);
                    TreeEdits.InsertAfter(colon, b_container);
                }
            }
            return (IParseTree)new_a_rule;
        }

        private static IParseTree Make(string closure_op, List<IParseTree> syms, Dictionary<TerminalNodeImpl, string> text_before, Parser parser)
        {
            var container = new ANTLRv4Parser.BlockContext(null, 0);
            bool first = true;
            bool parens = RequiresParens(syms, parser);
            var lp = new TerminalNodeImpl(new CommonToken(ANTLRv4Parser.LPAREN) { Line = -1, Column = -1, Text = "(" });
            var rp = new TerminalNodeImpl(new CommonToken(ANTLRv4Parser.RPAREN) { Line = -1, Column = -1, Text = ")" });
            var bar = new TerminalNodeImpl(new CommonToken(ANTLRv4Parser.OR) { Line = -1, Column = -1, Text = "|" });
            if (parens) TreeEdits.CopyTreeRecursive(lp, container, text_before);
            IParseTree last = null;
            for (int jj = 0; jj < syms.Count; ++jj)
            {
                if (!first)
                {
                    TreeEdits.CopyTreeRecursive(bar, container, text_before);
                    last = TreeEdits.CopyTreeRecursive(syms[jj], container, text_before);
                }
                else
                {
                    last = TreeEdits.CopyTreeRecursive(syms[jj], container, text_before);
                    first = false;
                }
            }
            if (parens) last = TreeEdits.CopyTreeRecursive(rp, container, text_before);
            var a1 = TreeEdits.InsertAfter(last, closure_op);
            text_before[a1] = "";
            return container;
        }

        private static bool RequiresParens(List<IParseTree> alphas, Parser p)
        {
            if (alphas.Count == 0) return false;
            if (alphas.Count > 1) return true;
            var v = alphas.First();
            var s = v.ToStringTree(p);
            if (v is ANTLRv4Parser.AlternativeContext && v.ChildCount > 1)
                return true;
            return false;
        }

        public static Dictionary<string, string> ConvertRecursionToKleeneOperator(Document document, List<IParseTree> nodes = null)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            if (nodes != null)
            {
                var updated = new HashSet<IParseTree>();
                foreach (var node in nodes)
                {
                    bool got = false;
                    // Make sure the node points to somewhere in a rule, and replace it 
                    // with the node in the LHS of the rule, if not already point to that node.
                    for (var p = node; p != null; p = p.Parent)
                    {
                        if (p is ANTLRv4Parser.ParserRuleSpecContext prsc)
                        {
                            updated.Add(prsc.RULE_REF());
                            got = true;
                            break;
                        }
                        else if (p is ANTLRv4Parser.LexerRuleSpecContext lrsc)
                        {
                            updated.Add(lrsc.TOKEN_REF());
                            got = true;
                            break;
                        }
                    }
                    if (!got)
                    {
                        throw new LanguageServerException("XPath spec yielded a 'node' that doesn't point do a parser or lexer rule.");
                    }
                }
                nodes = updated.ToList();
            }
            else
            {
                var pr = ParsingResultsFactory.Create(document);
                var aparser = pr.Parser;
                var atree = pr.ParseTree;
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    nodes = engine.parseExpression(
                        @"(//parserRuleSpec/RULE_REF | //lexerRuleSpec/TOKEN_REF)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                }
            }

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            for (int i = 0; i < nodes.Count(); ++i)
            {
                var node = nodes[i];
                if (!(node is TerminalNodeImpl && (node.Parent is ANTLRv4Parser.ParserRuleSpecContext || node.Parent is ANTLRv4Parser.LexerRuleSpecContext)))
                    throw new LanguageServerException("Node for Kleene EBNF transform must be the LHS symbol.");

                for (; node != null; node = node.Parent)
                {
                    if ((node is ANTLRv4Parser.ParserRuleSpecContext || node is ANTLRv4Parser.LexerRuleSpecContext))
                        break;
                }
                if (node == null)
                    throw new LanguageServerException("A rule is not selected. Please select one first.");

                // We are now at the rule that the user identified to eliminate direct
                // left recursion.
                // Check if the rule has direct left recursion.

                bool has_direct_left_recursion = HasDirectLeftRecursion(node);
                bool has_direct_right_recursion = HasDirectRightRecursion(node);
                if (!(has_direct_left_recursion || has_direct_right_recursion))
                {
                    continue;
                }
                else if (has_direct_left_recursion && has_direct_right_recursion)
                {
                    continue;
                }

                // Has direct recursion.
                node = ReplaceWithKleeneRules(pd_parser.Parser, has_direct_left_recursion, has_direct_right_recursion, node, text_before);
                if (node != null)
                {
                    TreeEdits.Replace(pd_parser.ParseTree,
                        (in IParseTree x, out bool c) =>
                        {
                            if (x is ANTLRv4Parser.ParserRuleSpecContext)
                            {
                                var y = x as ANTLRv4Parser.ParserRuleSpecContext;
                                var name = y.RULE_REF()?.GetText();
                                if (name == Lhs(node).GetText())
                                {
                                    var leaf = TreeEdits.Frontier(y).First();
                                    var leaf_replace = TreeEdits.Frontier(node).First();
                                    text_before[leaf_replace] = text_before[leaf];
                                    c = false;
                                    return node;
                                }
                            }
                            else if (x is ANTLRv4Parser.LexerRuleSpecContext)
                            {
                                var y = x as ANTLRv4Parser.LexerRuleSpecContext;
                                var name = y.TOKEN_REF()?.GetText();
                                if (name == Lhs(node).GetText())
                                {
                                    var leaf = TreeEdits.Frontier(y).First();
                                    var leaf_replace = TreeEdits.Frontier(node).First();
                                    text_before[leaf_replace] = text_before[leaf];
                                    c = false;
                                    return node;
                                }
                            }
                            c = true;
                            return null;
                        });
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        private static string GenerateNewName(IParseTree rule, ParsingResults pd_parser)
        {
            if (!(rule is ANTLRv4Parser.ParserRuleSpecContext r))
                return null;

            var b = r.RULE_REF().GetText();
            var list = pd_parser.AllNodes.Where(n =>
            {
                return (n is ANTLRv4Parser.ParserRuleSpecContext || n is ANTLRv4Parser.LexerRuleSpecContext);
            }).Select(n =>
            {
                if (n is ANTLRv4Parser.ParserRuleSpecContext)
                {
                    var z = n as ANTLRv4Parser.ParserRuleSpecContext;
                    var lhs = z.RULE_REF();
                    return lhs.GetText();
                }
                return "";
            }).ToList();
            int gnum = 1;
            for (; ; )
            {
                if (!list.Contains(b + gnum.ToString()))
                    break;
                gnum++;
            }
            return b + gnum.ToString();
        }

        public static List<string> HasDirectRec(Document document, IEnumerable<IParseTree> nodes = null)
        {
            List<string> result = new List<string>();
            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (!(node is TerminalNodeImpl && node.Parent is ANTLRv4Parser.ParserRuleSpecContext
                        || node is TerminalNodeImpl && node.Parent is ANTLRv4Parser.LexerRuleSpecContext))
                        throw new LanguageServerException("Node for query must be the LHS symbol.");
                }
            }
            else
            {
                var pr = ParsingResultsFactory.Create(document);
                var aparser = pr.Parser;
                var atree = pr.ParseTree;
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(atree, aparser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    nodes = engine.parseExpression(
                        @"//parserRuleSpec/RULE_REF",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                }
            }
            foreach (var node in nodes)
            {
                bool left = HasDirectLeftRecursion(node.Parent);
                bool right = HasDirectRightRecursion(node.Parent);
                if (left || right)
                {
                    string t = node.GetText() + " => " +
                        ((left ? "left" : "") + " " + (right ? "right" : "")).Trim();
                    result.Add(t);
                }
            }
            return result;
        }

        public static List<string> HasIndirectRec(IEnumerable<IParseTree> nodes, bool gen_dot, Document document)
        {
            List<string> result = new List<string>();
            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            result = HasIndirectLeftRecursion(document, gen_dot, nodes);
            return result;
        }

        public static Dictionary<string, string> ToRightRecursion(IEnumerable<IParseTree> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            if (!nodes.Any())
            {
                throw new LanguageServerException("XPath spec for LHS symbol empty.");
            }
            if (nodes.Count() > 1)
            {
                throw new LanguageServerException("XPath spec for LHS symbol specifies more than one node.");
            }

            var node = nodes.First();

            if (!(node is TerminalNodeImpl && node.Parent is ANTLRv4Parser.ParserRuleSpecContext))
                throw new LanguageServerException("Node for transform must be the LHS symbol.");

            var rule = node;
            for (; rule != null; rule = rule.Parent)
            {
                if ((rule is ANTLRv4Parser.ParserRuleSpecContext || rule is ANTLRv4Parser.LexerRuleSpecContext))
                    break;
            }
            if (rule == null)
                throw new LanguageServerException("A parser rule is not selected. Please select one first.");
            
            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();
            Digraph<string> graph = new Digraph<string>();
            foreach (TableOfRules.Row r in table.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                graph.AddVertex(r.LHS);
            }
            foreach (TableOfRules.Row r in table.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                List<string> j = r.RHS;
                //j.Reverse();
                foreach (string rhs in j)
                {
                    TableOfRules.Row sym = table.rules.Where(t => t.LHS == rhs).FirstOrDefault();
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
            foreach (TableOfRules.Row r in table.rules)
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

            var k = (ANTLRv4Parser.ParserRuleSpecContext)rule;
            var tarjan = new TarjanSCC<string, DirectedEdge<string>>(graph);
            List<string> ordered = new List<string>();
            var sccs = tarjan.Compute();

            // We are only going to note in "ordered" rules that are going to change.
            var scc = sccs[k.RULE_REF().ToString()];
            foreach (var v in scc)
            {
                ordered.Add(v);
            }

            // We are now at the rule that the user identified to eliminate indirect
            // left recursion.
            // Check if the rule participates in indirect left recursion.

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            var rules_tree = TreeEdits.FindTopDown(pd_parser.ParseTree,
                (in IParseTree t, out bool c) =>
                {
                    if (t is ANTLRv4Parser.RulesContext)
                    {
                        c = false;
                        return t;
                    }
                    c = true;
                    return null;
                }).First() as ANTLRv4Parser.RulesContext;

            // Keep a list of rules that are part of the SCC.
            Dictionary<string, IParseTree> rules = new Dictionary<string, IParseTree>();
            foreach (string s in ordered)
            {
                var ai = table.rules.Where(r => r.LHS == s).First();
                var air = (ANTLRv4Parser.ParserRuleSpecContext)ai.rule;
                rules[s] = TreeEdits.CopyTreeRecursive(air, null, text_before);
            }
            for (int i = 0; i < ordered.Count; ++i)
            {
                var ai = ordered[i];
                var ai_tree = rules[ai];
                ANTLRv4Parser.ParserRuleSpecContext new_a_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                var r = ai_tree as ANTLRv4Parser.ParserRuleSpecContext;
                var lhs = r.RULE_REF()?.GetText();
                TreeEdits.CopyTreeRecursive(r.RULE_REF(), new_a_rule, text_before);
                // Now have "A"
                {
                    var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                    var new_colon = new TerminalNodeImpl(token2);
                    new_a_rule.AddChild(new_colon);
                    new_colon.Parent = new_a_rule;
                }
                // Now have "A :"
                ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                {
                    ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(new_a_rule, 0);
                    new_a_rule.AddChild(new_rule_block_context);
                    new_rule_block_context.Parent = new_a_rule;
                    new_rule_block_context.AddChild(rule_alt_list);
                    rule_alt_list.Parent = new_rule_block_context;
                }
                // Now have "A : <rb <ral> >"
                {
                    var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                    var new_semi = new TerminalNodeImpl(token3);
                    new_a_rule.AddChild(new_semi);
                    new_semi.Parent = new_a_rule;
                }
                // Now have "A : <rb <ral> > ;"
                {
                    TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
                }
                // Now have "A : <rb <ral> > ; <eg>"
                for (int j = 0; j < i; ++j)
                {
                    var aj = ordered[j];
                    var aj_tree = rules[aj];
                    var new_alts = new List<ANTLRv4Parser.AlternativeContext>();
                    foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(ai_tree))
                    {
                        ANTLRv4Parser.ElementContext e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                        if (!(aj == e?.GetText() || aj == e?.atom()?.GetText()))
                        {
                            // Leave alt unchanged.
                            new_alts.Add(alt);
                            continue;
                        }

                        // Substitute Aj into Ai.
                        // Example:
                        // s : a A | B;
                        // a : a C | s D | ;
                        // ts order of symbols = [s, a].
                        // i = 1, j = 0.
                        // => a : a C | a A D | B D | ;

                        foreach (ANTLRv4Parser.AlternativeContext alt2 in EnumeratorOfAlts(aj_tree))
                        {
                            ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                            foreach (var element in alt2.element())
                            {
                                TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                            }
                            bool first = true;
                            foreach (var element in alt.element())
                            {
                                if (first)
                                {
                                    first = false;
                                    continue;
                                }
                                TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                            }
                            new_alts.Add(new_alt);
                        }
                    }
                    {
                        bool first = true;
                        foreach (var new_alt in new_alts)
                        {
                            if (!first)
                            {
                                var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                                var new_or = new TerminalNodeImpl(token4);
                                rule_alt_list.AddChild(new_or);
                                new_or.Parent = rule_alt_list;
                            }
                            first = false;
                            ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                            rule_alt_list.AddChild(l_alt);
                            l_alt.Parent = rule_alt_list;
                            l_alt.AddChild(new_alt);
                            new_alt.Parent = l_alt;
                        }
                    }
                    rules[ai] = new_a_rule;
                }

                // Check if the rule ai has direct left recursion.
                bool has_direct_left_recursion = HasDirectLeftRecursion(rules[ai]);
                if (!has_direct_left_recursion)
                {
                    continue;
                }

                // Has direct left recursion.

                // Replace rule with two new rules.
                //
                // Original rule:
                // A
                //   : A a1
                //   | A a2
                //   | A a3
                //   | B1
                //   | B2
                //   ...
                //   ;
                // Note a1, a2, a3 ... cannot be empty sequences.
                // B1, B2, ... cannot start with A.
                //
                // New rules.
                //
                // A
                //   : B1 A'
                //   | B2 A'
                //   | ...
                //   ;
                // A'
                //   : a1 A'
                //   | a2 A'
                //   | ...
                //   | (empty)
                //   ;
                //

                string generated_name = GenerateNewName(rules[ai], pd_parser);
                var (fixed_rule, new_rule) = GenerateReplacementRules(generated_name, rules[ai], text_before);
                rules[ai] = fixed_rule;
                rules[generated_name] = new_rule;
            }


            {
                // Replace original rule.
                IParseTree last = null;
                int last_index = -1;
                TreeEdits.Replace(pd_parser.ParseTree,
                    (in IParseTree x, out bool c) =>
                    {
                        c = true;
                        if (x is ANTLRv4Parser.ParserRuleSpecContext)
                        {
                            var y = x as ANTLRv4Parser.ParserRuleSpecContext;
                            var name = y.RULE_REF()?.GetText();
                            rules.TryGetValue(name, out IParseTree replacement);
                            if (replacement != null)
                            {
                                last_index = rules_tree.children.FindIndex(
                                    w =>
                                    {
                                        var xxxx = w as ANTLRv4Parser.RuleSpecContext;
                                        var yyyy = xxxx.parserRuleSpec();
                                        if (yyyy == null)
                                            return false;
                                        return y == yyyy;
                                    });
                                last = replacement;
                                return replacement;
                            }
                        }
                        return null;
                    });


                // Add in new rules.
                last_index++;
                foreach (var r in rules)
                {
                    var key = r.Key;
                    var value = r.Value;
                    var prc = value as ANTLRv4Parser.ParserRuleSpecContext;
                    if (!ordered.Contains(key))
                    {
                        var rule_spec = new ANTLRv4Parser.RuleSpecContext(null, 0);
                        rule_spec.AddChild(prc);
                        prc.Parent = rule_spec;
                        rules_tree.children.Insert(last_index++, rule_spec);
                        rule_spec.Parent = rules_tree;
                    }
                }
                StringBuilder sb = new StringBuilder();
                TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
                var new_code = sb.ToString();
                if (new_code != pd_parser.Code)
                {
                    result[document.FullPath] = new_code;
                }
            }
            return result;
        }

        public static Dictionary<string, string> EliminateDirectLeftRecursion(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Assume cursor positioned at the rule that contains left recursion.
            // Find rule.
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
                var st = ta.StartIndex;
                var sp = tb.StopIndex + 1;
                return st <= start && start < sp;
            }).FirstOrDefault();
            rule = it ?? throw new LanguageServerException("A parser rule is not selected. Please select one first.");

            // We are now at the rule that the user identified to eliminate direct
            // left recursion.
            // Check if the rule has direct left recursion.

            bool has_direct_left_recursion = HasDirectLeftRecursion(rule);
            if (!has_direct_left_recursion)
            {
                throw new LanguageServerException("Parser rule selected does not have direct left recursion. Please select another rule.");
            }

            // Has direct left recursion.

            // Replace rule with two new rules.
            //
            // Original rule:
            // A
            //   : A a1
            //   | A a2
            //   | A a3
            //   | B1
            //   | B2
            //   ...
            //   ;
            // Note a1, a2, a3 ... cannot be empty sequences.
            // B1, B2, ... cannot start with A.
            //
            // New rules.
            //
            // A
            //   : B1 A'
            //   | B2 A'
            //   | ...
            //   ;
            // A'
            //   : a1 A'
            //   | a2 A'
            //   | ...
            //   | (empty)
            //   ;
            //

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            
            string generated_name = GenerateNewName(rule, pd_parser);

            {
                ANTLRv4Parser.ParserRuleSpecContext new_a_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                {
                    var r = rule as ANTLRv4Parser.ParserRuleSpecContext;
                    var old_lhs = r.RULE_REF();
                    var lhs = old_lhs.GetText();
                    {
                        var new_lhs = TreeEdits.CopyTreeRecursive(old_lhs, new_a_rule, text_before);
                    }
                    // Now have "A"
                    {
                        var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                        var new_colon = new TerminalNodeImpl(token2);
                        new_a_rule.AddChild(new_colon);
                        new_colon.Parent = new_a_rule;
                    }
                    // Now have "A :"
                    ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                    {
                        ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(new_a_rule, 0);
                        new_a_rule.AddChild(new_rule_block_context);
                        new_rule_block_context.Parent = new_a_rule;
                        new_rule_block_context.AddChild(rule_alt_list);
                        rule_alt_list.Parent = new_rule_block_context;
                    }
                    // Now have "A : <rb <ral> >"
                    {
                        var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                        var new_semi = new TerminalNodeImpl(token3);
                        new_a_rule.AddChild(new_semi);
                        new_semi.Parent = new_a_rule;
                    }
                    // Now have "A : <rb <ral> > ;"
                    {
                        TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
                    }
                    // Now have "A : <rb <ral> > ; <eg>"
                    bool first = true;
                    foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(rule))
                    {
                        ANTLRv4Parser.ElementContext e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                        if (lhs == e?.GetText())
                        {
                            // skip alts that have direct left recursion.
                            continue;
                        }
                        {
                            if (!first)
                            {
                                var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                                var new_or = new TerminalNodeImpl(token4);
                                rule_alt_list.AddChild(new_or);
                                new_or.Parent = rule_alt_list;
                            }
                            first = false;
                        }
                        ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                        rule_alt_list.AddChild(l_alt);
                        l_alt.Parent = rule_alt_list;
                        // Create new alt "beta A'".
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                        foreach (var element in alt.element())
                        {
                            TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                        }
                        var token = new CommonToken(ANTLRv4Lexer.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                        new_ruleref.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ruleref;
                        var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                        new_atom.AddChild(new_ruleref);
                        new_ruleref.Parent = new_atom;
                        var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                        new_element.AddChild(new_atom);
                        new_atom.Parent = new_element;
                        new_alt.AddChild(new_element);
                        new_element.Parent = new_alt;
                    }
                }
                // Now have "A : beta1 A' | beta2 A' | ... ; <eg>"

                ANTLRv4Parser.ParserRuleSpecContext new_ap_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                {
                    var r = rule as ANTLRv4Parser.ParserRuleSpecContext;
                    var lhs = r.RULE_REF()?.GetText();
                    {
                        var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        text_before.Add(new_rule_ref,
                            System.Environment.NewLine + System.Environment.NewLine);
                        new_ap_rule.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ap_rule;
                    }
                    // Now have "A'"
                    {
                        var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                        var new_colon = new TerminalNodeImpl(token2);
                        new_ap_rule.AddChild(new_colon);
                        new_colon.Parent = new_ap_rule;
                    }
                    // Now have "A' :"
                    ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                    {
                        ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(new_a_rule, 0);
                        new_ap_rule.AddChild(new_rule_block_context);
                        new_rule_block_context.Parent = new_ap_rule;
                        new_rule_block_context.AddChild(rule_alt_list);
                        rule_alt_list.Parent = new_rule_block_context;
                    }
                    // Now have "A' : <rb <ral> >"
                    {
                        var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                        var new_semi = new TerminalNodeImpl(token3);
                        new_ap_rule.AddChild(new_semi);
                        new_semi.Parent = new_ap_rule;
                    }
                    // Now have "A : <rb <ral> > ;"
                    {
                        TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
                    }
                    // Now have "A' : <rb <ral> > ; <eg>"
                    bool first = true;
                    foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(rule))
                    {
                        ANTLRv4Parser.ElementContext e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                        if (lhs != e?.GetText())
                        {
                            // skip alts that DO NOT have direct left recursion.
                            continue;
                        }
                        {
                            if (!first)
                            {
                                var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                                var new_or = new TerminalNodeImpl(token4);
                                rule_alt_list.AddChild(new_or);
                                new_or.Parent = rule_alt_list;
                            }
                            first = false;
                        }
                        ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                        rule_alt_list.AddChild(l_alt);
                        l_alt.Parent = rule_alt_list;
                        // Create new alt "alpha A'".
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                        bool first2 = true;
                        foreach (var element in alt.element())
                        {
                            if (first2)
                            {
                                first2 = false;
                                continue;
                            }
                            TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                        }
                        var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                        new_ruleref.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ruleref;
                        var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                        new_atom.AddChild(new_ruleref);
                        new_ruleref.Parent = new_atom;
                        var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                        new_element.AddChild(new_atom);
                        new_atom.Parent = new_element;
                        new_alt.AddChild(new_element);
                        new_element.Parent = new_alt;
                    }
                    {
                        if (!first)
                        {
                            var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                            var new_or = new TerminalNodeImpl(token4);
                            rule_alt_list.AddChild(new_or);
                            new_or.Parent = rule_alt_list;
                        }
                        first = false;
                        ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                        rule_alt_list.AddChild(l_alt);
                        l_alt.Parent = rule_alt_list;
                        // Create new empty alt.
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                    }
                }
                // Now have "A' : alpha1 A' | alpha2 A' | ... ;"

                // Replace rule and add new rule afterwards.
                {
                    var rs = rule.Parent;
                    var rule_spec = rs as ANTLRv4Parser.RuleSpecContext;
                    var r = rule_spec.Parent;
                    var rules = r as ANTLRv4Parser.RulesContext;
                    TreeEdits.Replace(rule_spec,
                        (in IParseTree n, out bool c) =>
                        {
                            if (n == rule)
                            {
                                c = false;
                                return new_a_rule;
                            }
                            c = true;
                            return null;
                        });
                    var new_rs = new ANTLRv4Parser.RuleSpecContext(null, 0);
                    new_rs.AddChild(new_ap_rule);
                    new_ap_rule.Parent = new_rs;
                    int i = 0;
                    for (; i < rules.ChildCount; ++i)
                    {
                        if (rules.GetChild(i) == rule_spec)
                            break;
                    }
                    rules.children.Insert(i + 1, new_rs);
                    new_rs.Parent = rules;
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        public static Dictionary<string, string> EliminateIndirectLeftRecursion(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
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
            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();
            Digraph<string> graph = new Digraph<string>();
            foreach (TableOfRules.Row r in table.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                graph.AddVertex(r.LHS);
            }
            foreach (TableOfRules.Row r in table.rules)
            {
                if (!r.is_parser_rule)
                {
                    continue;
                }
                List<string> j = r.RHS;
                //j.Reverse();
                foreach (string rhs in j)
                {
                    TableOfRules.Row sym = table.rules.Where(t => t.LHS == rhs).FirstOrDefault();
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
            foreach (TableOfRules.Row r in table.rules)
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

            // We are only going to eliminate the indirect recursion of what is being pointed to.
            // Check rule and graph.
            // Assume cursor positioned at the rule that participates in indirect left recursion.
            // Find rule.
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
                var st = ta.StartIndex;
                var sp = tb.StopIndex + 1;
                return st <= start && start < sp;
            }).FirstOrDefault();
            if (it == null)
            {
                return result;
            }
            rule = it;
            var k = (ANTLRv4Parser.ParserRuleSpecContext)rule;
            var tarjan = new TarjanSCC<string, DirectedEdge<string>>(graph);
            List<string> ordered = new List<string>();
            var sccs = tarjan.Compute();

            // We are only going to note in "ordered" rules that are going to change.
            var scc = sccs[k.RULE_REF().ToString()];
            foreach (var v in scc)
            {
                ordered.Add(v);
            }

            // We are now at the rule that the user identified to eliminate indirect
            // left recursion.
            // Check if the rule participates in indirect left recursion.

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            var rules_tree = TreeEdits.FindTopDown(pd_parser.ParseTree,
                (in IParseTree t, out bool c) =>
                {
                    if (t is ANTLRv4Parser.RulesContext)
                    {
                        c = false;
                        return t;
                    }
                    c = true;
                    return null;
                }).First() as ANTLRv4Parser.RulesContext;

            // Keep a list of rules that are part of the SCC.
            Dictionary<string, IParseTree> rules = new Dictionary<string, IParseTree>();
            foreach (string s in ordered)
            {
                var ai = table.rules.Where(r => r.LHS == s).First();
                var air = (ANTLRv4Parser.ParserRuleSpecContext)ai.rule;
                rules[s] = TreeEdits.CopyTreeRecursive(air, null, text_before);
            }
            for (int i = 0; i < ordered.Count; ++i)
            {
                var ai = ordered[i];
                var ai_tree = rules[ai];
                ANTLRv4Parser.ParserRuleSpecContext new_a_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                var r = ai_tree as ANTLRv4Parser.ParserRuleSpecContext;
                var lhs = r.RULE_REF()?.GetText();
                TreeEdits.CopyTreeRecursive(r.RULE_REF(), new_a_rule, text_before);
                // Now have "A"
                {
                    var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                    var new_colon = new TerminalNodeImpl(token2);
                    new_a_rule.AddChild(new_colon);
                    new_colon.Parent = new_a_rule;
                }
                // Now have "A :"
                ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                {
                    ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(new_a_rule, 0);
                    new_a_rule.AddChild(new_rule_block_context);
                    new_rule_block_context.Parent = new_a_rule;
                    new_rule_block_context.AddChild(rule_alt_list);
                    rule_alt_list.Parent = new_rule_block_context;
                }
                // Now have "A : <rb <ral> >"
                {
                    var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                    var new_semi = new TerminalNodeImpl(token3);
                    new_a_rule.AddChild(new_semi);
                    new_semi.Parent = new_a_rule;
                }
                // Now have "A : <rb <ral> > ;"
                {
                    TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_a_rule, text_before);
                }
                // Now have "A : <rb <ral> > ; <eg>"
                for (int j = 0; j < i; ++j)
                {
                    var aj = ordered[j];
                    var aj_tree = rules[aj];
                    var new_alts = new List<ANTLRv4Parser.AlternativeContext>();
                    foreach (ANTLRv4Parser.AlternativeContext alt in EnumeratorOfAlts(ai_tree))
                    {
                        ANTLRv4Parser.ElementContext e = EnumeratorOfRHS(alt)?.FirstOrDefault();
                        if (aj != e?.GetText())
                        {
                            // Leave alt unchanged.
                            new_alts.Add(alt);
                            continue;
                        }
                        
                        // Substitute Aj into Ai.
                        // Example:
                        // s : a A | B;
                        // a : a C | s D | ;
                        // ts order of symbols = [s, a].
                        // i = 1, j = 0.
                        // => a : a C | a A D | B D | ;

                        foreach (ANTLRv4Parser.AlternativeContext alt2 in EnumeratorOfAlts(aj_tree))
                        {
                            ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                            foreach (var element in alt2.element())
                            {
                                TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                            }
                            bool first = true;
                            foreach (var element in alt.element())
                            {
                                if (first)
                                {
                                    first = false;
                                    continue;
                                }
                                TreeEdits.CopyTreeRecursive(element, new_alt, text_before);
                            }
                            new_alts.Add(new_alt);
                        }
                    }
                    {
                        bool first = true;
                        foreach (var new_alt in new_alts)
                        {
                            if (!first)
                            {
                                var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                                var new_or = new TerminalNodeImpl(token4);
                                rule_alt_list.AddChild(new_or);
                                new_or.Parent = rule_alt_list;
                            }
                            first = false;
                            ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                            rule_alt_list.AddChild(l_alt);
                            l_alt.Parent = rule_alt_list;
                            l_alt.AddChild(new_alt);
                            new_alt.Parent = l_alt;
                        }
                    }
                    rules[ai] = new_a_rule;
                }

                // Check if the rule ai has direct left recursion.
                bool has_direct_left_recursion = HasDirectLeftRecursion(rules[ai]);
                if (!has_direct_left_recursion)
                {
                    continue;
                }

                // Has direct left recursion.

                // Replace rule with two new rules.
                //
                // Original rule:
                // A
                //   : A a1
                //   | A a2
                //   | A a3
                //   | B1
                //   | B2
                //   ...
                //   ;
                // Note a1, a2, a3 ... cannot be empty sequences.
                // B1, B2, ... cannot start with A.
                //
                // New rules.
                //
                // A
                //   : B1 A'
                //   | B2 A'
                //   | ...
                //   ;
                // A'
                //   : a1 A'
                //   | a2 A'
                //   | ...
                //   | (empty)
                //   ;
                //

                string generated_name = GenerateNewName(rules[ai], pd_parser);
                var (fixed_rule, new_rule) = GenerateReplacementRules(generated_name, rules[ai], text_before);
                rules[ai] = fixed_rule;
                rules[generated_name] = new_rule;
            }


            {
                // Replace original rule.
                IParseTree last = null;
                int last_index = -1;
                TreeEdits.Replace(pd_parser.ParseTree,
                    (in IParseTree x, out bool c) =>
                    {
                        c = true;
                        if (x is ANTLRv4Parser.ParserRuleSpecContext)
                        {
                            var y = x as ANTLRv4Parser.ParserRuleSpecContext;
                            var name = y.RULE_REF()?.GetText();
                            rules.TryGetValue(name, out IParseTree replacement);
                            if (replacement != null)
                            {
                                last_index = rules_tree.children.FindIndex(
                                    w =>
                                    {
                                        var xxxx = w as ANTLRv4Parser.RuleSpecContext;
                                        var yyyy = xxxx.parserRuleSpec();
                                        if (yyyy == null)
                                            return false;
                                        return y == yyyy;
                                    });
                                last = replacement;
                                return replacement;
                            }
                        }
                        return null;
                    });


                // Add in new rules.
                last_index++;
                foreach (var r in rules)
                {
                    var key = r.Key;
                    var value = r.Value;
                    var prc = value as ANTLRv4Parser.ParserRuleSpecContext;
                    if (!ordered.Contains(key))
                    {
                        var rule_spec = new ANTLRv4Parser.RuleSpecContext(null, 0);
                        rule_spec.AddChild(prc);
                        prc.Parent = rule_spec;
                        rules_tree.children.Insert(last_index++, rule_spec);
                        rule_spec.Parent = rules_tree;
                    }
                }
                StringBuilder sb = new StringBuilder();
                TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
                var new_code = sb.ToString();
                if (new_code != pd_parser.Code)
                {
                    result[document.FullPath] = new_code;
                }
            }
            return result;
        }

        public static TerminalNodeImpl Lhs(IParseTree ai)
        {
            if (ai is ANTLRv4Parser.ParserRuleSpecContext r)
            {
                var lhs = r.RULE_REF();
                return (TerminalNodeImpl)lhs;
            }
            else if (ai is ANTLRv4Parser.LexerRuleSpecContext r2)
            {
                ITerminalNode lhs = r2.TOKEN_REF();
                return (TerminalNodeImpl)lhs;
            }
            else return null;
        }

        public static IEnumerable<ANTLRv4Parser.AlternativeContext> EnumeratorOfAlts(IParseTree ai)
        {
            var r = ai as ANTLRv4Parser.ParserRuleSpecContext;
            ANTLRv4Parser.RuleBlockContext rhs = r.ruleBlock();
            ANTLRv4Parser.RuleAltListContext rule_alt_list = rhs.ruleAltList();
            foreach (ANTLRv4Parser.LabeledAltContext l_alt in rule_alt_list.labeledAlt())
            {
                ANTLRv4Parser.AlternativeContext alt = l_alt.alternative();
                yield return alt;
            }
        }

        public static IEnumerable<ANTLRv4Parser.LexerAltContext> EnumeratorOfLexerAlts(IParseTree ai)
        {
            var r = ai as ANTLRv4Parser.LexerRuleSpecContext;
            var rhs = r.lexerRuleBlock();
            var rule_alt_list = rhs.lexerAltList();
            var list = rule_alt_list.lexerAlt();
            foreach (var l_alt in list)
            {
                yield return l_alt;
            }
        }

        public static IEnumerable<ANTLRv4Parser.ElementContext> EnumeratorOfRHS(ANTLRv4Parser.AlternativeContext alt)
        {
            foreach (ANTLRv4Parser.ElementContext element in alt.element())
            {
                yield return element;
            }
        }

        public static IEnumerable<ANTLRv4Parser.LexerAtomContext> EnumeratorOfLexerRHS(ANTLRv4Parser.LexerAltContext alt)
        {
            var elements = alt.lexerElements();
            foreach (var element in elements.lexerElement())
            {
                var atom = element.lexerAtom();
                {
                    yield return atom;
                }
            }
        }

        public static Dictionary<string, string> EliminateAntlrKeywordsInRules(Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            // Check if initial file is a parser or combined grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                ;
            if (!is_grammar)
            {
                throw new LanguageServerException("A parser or combined grammar file is not selected. Please select one first.");
            }

            TableOfRules table = new TableOfRules(pd_parser, document);
            table.ReadRules();
            table.FindPartitions();
            table.FindStartRules();

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            TreeEdits.Replace(pd_parser.ParseTree,
                (in IParseTree n, out bool c) =>
                {
                    c = true;
                    if (!(n is TerminalNodeImpl))
                    {
                        return null;
                    }
                    var t = n as TerminalNodeImpl;
                    var r = t.GetText();
                    if (t.Symbol.Type == ANTLRv4Lexer.RULE_REF)
                    {
                        if (r == "options"
                            || r == "grammar"
                            || r == "tokenVocab"
                            || r == "lexer"
                            || r == "parser"
                            || r == "rule")
                        {
                            var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = r + "_nonterminal" };
                            var new_rule_ref = new TerminalNodeImpl(token);
                            text_before.Add(new_rule_ref, text_before[t]);
                        }
                    }
                    return null;
                });
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> AddLexerRulesForStringLiterals(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                ;
            if (!is_grammar)
            {
                throw new LanguageServerException("A parser or combined grammar file is not selected. Please select one first.");
            }

            var workspace = document.Workspace;

            // Find all other grammars by walking dependencies (import, vocab, file names).
            HashSet<string> read_files = new HashSet<string>
            {
                document.FullPath
            };
            Dictionary<Workspaces.Document, List<TerminalNodeImpl>> every_damn_literal =
                new Dictionary<Workspaces.Document, List<TerminalNodeImpl>>();

            // Find rewrite rules, i.e., lexer rule "<TOKEN_REF> : <string literal>"
            Dictionary<string, string> subs = new Dictionary<string, string>();
            foreach (string f in read_files)
            {
                Workspaces.Document whatever_document = workspace.FindDocument(f);
                if (whatever_document == null)
                {
                    continue;
                }
                ParsingResults pd_whatever = ParsingResultsFactory.Create(whatever_document) as ParsingResults;

                // Find literals in grammars.
                LiteralsGrammar lp_whatever = new LiteralsGrammar();
                ParseTreeWalker.Default.Walk(lp_whatever, pd_whatever.ParseTree);
                List<TerminalNodeImpl> list_literals = lp_whatever.Literals;
                foreach (TerminalNodeImpl lexer_literal in list_literals)
                {
                    string old_name = lexer_literal.GetText();
                    // Given candidate, walk up tree to find lexer_rule.
                    /*
                        ( ruleSpec
                          ( lexerRuleSpec
                            ( OFF_CHANNEL text=\r\n\r\n
                            )
                            ( OFF_CHANNEL text=...
                            )
                            (OFF_CHANNEL text =\r\n\r\n
                            )
                            (OFF_CHANNEL text =...
                            )
                            (OFF_CHANNEL text =\r\n\r\n
                            )
                            (DEFAULT_TOKEN_CHANNEL i = 995 txt = NONASSOC tt = 1
                            )
                            (OFF_CHANNEL text =\r\n\t
                            )
                            (DEFAULT_TOKEN_CHANNEL i = 997 txt =: tt = 29
                            )
                            (lexerRuleBlock
                              (lexerAltList
                                (lexerAlt
                                  (lexerElements
                                    (lexerElement
                                      (lexerAtom
                                        (terminal
                                          (OFF_CHANNEL text =
                                          )
                                          (DEFAULT_TOKEN_CHANNEL i = 999 txt = '%binary' tt = 8
                            ))))))))
                            (OFF_CHANNEL text =\r\n\t
                            )
                            (DEFAULT_TOKEN_CHANNEL i = 1001 txt =; tt = 32
                        ) ) )

                     * Make sure it fits the structure of the tree shown above.
                     * 
                     */
                    IRuleNode p1 = lexer_literal.Parent;
                    if (p1.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p1 is ANTLRv4Parser.TerminalContext))
                    {
                        continue;
                    }

                    IRuleNode p2 = p1.Parent;
                    if (p2.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p2 is ANTLRv4Parser.LexerAtomContext))
                    {
                        continue;
                    }

                    IRuleNode p3 = p2.Parent;
                    if (p3.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p3 is ANTLRv4Parser.LexerElementContext))
                    {
                        continue;
                    }

                    IRuleNode p4 = p3.Parent;
                    if (p4.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p4 is ANTLRv4Parser.LexerElementsContext))
                    {
                        continue;
                    }

                    IRuleNode p5 = p4.Parent;
                    if (p5.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p5 is ANTLRv4Parser.LexerAltContext))
                    {
                        continue;
                    }

                    IRuleNode p6 = p5.Parent;
                    if (p6.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p6 is ANTLRv4Parser.LexerAltListContext))
                    {
                        continue;
                    }

                    IRuleNode p7 = p6.Parent;
                    if (p7.ChildCount != 1)
                    {
                        continue;
                    }

                    if (!(p7 is ANTLRv4Parser.LexerRuleBlockContext))
                    {
                        continue;
                    }

                    IRuleNode p8 = p7.Parent;
                    if (p8.ChildCount != 4)
                    {
                        continue;
                    }

                    if (!(p8 is ANTLRv4Parser.LexerRuleSpecContext))
                    {
                        continue;
                    }

                    IParseTree alt = p8.GetChild(0);
                    string new_name = alt.GetText();
                    subs.Add(old_name, new_name);
                }
            }

            // Determine where to put any new rules.
            string where_to_stuff = null;
            foreach (string f in read_files)
            {
                Workspaces.Document whatever_document = workspace.FindDocument(f);
                if (whatever_document == null)
                {
                    continue;
                }
                ParsingResults pd_whatever = ParsingResultsFactory.Create(whatever_document) as ParsingResults;
                ExtractGrammarType x1 = new ExtractGrammarType();
                ParseTreeWalker.Default.Walk(x1, pd_whatever.ParseTree);
                bool is_right_grammar =
                           x1.Type == ExtractGrammarType.GrammarType.Combined
                           || x1.Type == ExtractGrammarType.GrammarType.Lexer;
                if (!is_right_grammar)
                    continue;
                if (where_to_stuff != null)
                    return null;
                where_to_stuff = f;
            }

            Dictionary<string, Dictionary<TerminalNodeImpl, string>> text_before = new Dictionary<string, Dictionary<TerminalNodeImpl, string>>();
            // Find string literals in parser and combined grammars and substitute.
            foreach (string f in read_files)
            {
                Workspaces.Document whatever_document = workspace.FindDocument(f);
                if (whatever_document == null)
                {
                    continue;
                }
                ParsingResults pd_whatever = ParsingResultsFactory.Create(whatever_document) as ParsingResults;
                // Get all intertoken text immediately for source reconstruction.
                var (tb, other) = TreeEdits.TextToLeftOfLeaves(pd_whatever.TokStream, pd_whatever.ParseTree);
                text_before[f] = tb;
            }
            Dictionary<string, string> new_subs = new Dictionary<string, string>();
            foreach (string f in read_files)
            {
                Workspaces.Document whatever_document = workspace.FindDocument(f);
                if (whatever_document == null)
                {
                    continue;
                }
                ParsingResults pd_whatever = ParsingResultsFactory.Create(whatever_document) as ParsingResults;
                TreeEdits.Replace(pd_parser.ParseTree,
                (in IParseTree n, out bool c) =>
                {
                    c = true;
                    if (!(n is TerminalNodeImpl))
                    {
                        return null;
                    }
                    var t = n as TerminalNodeImpl;
                    if (t.Payload.Type != ANTLRv4Lexer.STRING_LITERAL)
                    {
                        return null;
                    }
                    bool no = false;
                    // Make sure this literal does not appear in lexer rule
                    // because we are going to create a new lexer rule for the literal.
                    for (IRuleNode p = t.Parent; p != null; p = p.Parent)
                    {
                        if (p is ANTLRv4Parser.LexerRuleSpecContext)
                        {
                            no = true;
                            break;
                        }
                    }
                    if (no)
                    {
                        return null;
                    }
                    var literal = t.GetText();
                    subs.TryGetValue(literal, out string value);
                    if (value == null)
                    {
                        string now = DateTime.Now.ToString()
                            .Replace("/", "_")
                            .Replace(":", "_")
                            .Replace(" ", "_");
                        var new_r = "GENERATED_" + now;
                        if (subs.ContainsValue(new_r))
                        {
                            for (int i = 0; ; ++i)
                            {
                                if (!subs.ContainsValue(new_r + "_" + i))
                                {
                                    new_r = new_r + "_" + i;
                                    break;
                                }
                            }
                        }
                        subs[literal] = new_r;
                        new_subs[literal] = new_r;
                        value = new_r;
                    }
                    var token = new CommonToken(ANTLRv4Parser.TOKEN_REF) { Line = -1, Column = -1, Text = value };
                    return new TerminalNodeImpl(token);
                });
                StringBuilder sb = new StringBuilder();
                TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before[f]);
                var new_code = sb.ToString();
                if (new_code != pd_parser.Code)
                {
                    result[document.FullPath] = new_code;
                }
            }
            if (new_subs.Count > 0)
            {
                Workspaces.Document whatever_document = workspace.FindDocument(where_to_stuff);
                ParsingResults pd_whatever = ParsingResultsFactory.Create(whatever_document) as ParsingResults;
                string old_code = pd_whatever.Code;
                if (result.TryGetValue(where_to_stuff, out string other))
                    old_code = other;
                var rules = TreeEdits.FindTopDown(pd_whatever.ParseTree,
                    (in IParseTree t, out bool c) =>
                    {
                        if (t is ANTLRv4Parser.RulesContext)
                        {
                            c = false;
                            return t;
                        }
                        c = true;
                        return null;
                    }).First() as ANTLRv4Parser.RulesContext;
                foreach (var pair in new_subs)
                {
                    var literal = pair.Key;
                    var gen_name = pair.Value;
                    ANTLRv4Parser.LexerRuleSpecContext new_a_rule = new ANTLRv4Parser.LexerRuleSpecContext(null, 0);
                    {
                        var token = new CommonToken(ANTLRv4Parser.TOKEN_REF) { Line = -1, Column = -1, Text = gen_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        text_before[where_to_stuff].Add(new_rule_ref,
                            System.Environment.NewLine + System.Environment.NewLine);
                        new_a_rule.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_a_rule;
                        // Now have "A"
                        {
                            var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                            var new_colon = new TerminalNodeImpl(token2);
                            new_a_rule.AddChild(new_colon);
                            new_colon.Parent = new_a_rule;
                        }
                        // Now have "A :"
                        ANTLRv4Parser.LexerAltListContext lexer_alt_list = new ANTLRv4Parser.LexerAltListContext(null, 0);
                        {
                            ANTLRv4Parser.LexerRuleBlockContext new_rule_block_context = new ANTLRv4Parser.LexerRuleBlockContext(new_a_rule, 0);
                            new_a_rule.AddChild(new_rule_block_context);
                            new_rule_block_context.Parent = new_a_rule;
                            new_rule_block_context.AddChild(lexer_alt_list);
                            lexer_alt_list.Parent = new_rule_block_context;
                        }
                        ANTLRv4Parser.LexerAltContext lexer_alt = new ANTLRv4Parser.LexerAltContext(null, 0);
                        lexer_alt_list.AddChild(lexer_alt);
                        lexer_alt.Parent = lexer_alt_list;
                        ANTLRv4Parser.LexerElementsContext lexer_elements = new ANTLRv4Parser.LexerElementsContext(null, 0);
                        lexer_alt.AddChild(lexer_elements);
                        lexer_elements.Parent = lexer_alt;
                        ANTLRv4Parser.LexerElementContext lexer_element = new ANTLRv4Parser.LexerElementContext(null, 0);
                        lexer_elements.AddChild(lexer_element);
                        lexer_element.Parent = lexer_elements;
                        ANTLRv4Parser.LexerAtomContext lexer_atom = new ANTLRv4Parser.LexerAtomContext(null, 0);
                        lexer_element.AddChild(lexer_atom);
                        lexer_atom.Parent = lexer_element;
                        ANTLRv4Parser.TerminalContext terminal = new ANTLRv4Parser.TerminalContext(null, 0);
                        lexer_atom.AddChild(terminal);
                        terminal.Parent = lexer_atom;
                        {
                            var t = new CommonToken(ANTLRv4Parser.STRING_LITERAL) { Line = -1, Column = -1, Text = literal };
                            var sl = new TerminalNodeImpl(t);
                            terminal.AddChild(sl);
                            sl.Parent = terminal;
                        }
                        {
                            var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                            var new_semi = new TerminalNodeImpl(token3);
                            new_a_rule.AddChild(new_semi);
                            new_semi.Parent = new_a_rule;
                        }
                        // Now have "A : 'string-literal' ;"
                        var rule_spec = new ANTLRv4Parser.RuleSpecContext(null, 0);
                        rule_spec.AddChild(new_a_rule);
                        new_a_rule.Parent = rule_spec;
                        rules.AddChild(rule_spec);
                        rule_spec.Parent = rules;
                    }
                }
                StringBuilder sb = new StringBuilder();
                TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before[where_to_stuff]);
                var new_code = sb.ToString();
                if (new_code != pd_parser.Code)
                {
                    result[document.FullPath] = new_code;
                }
            }

            return result;
        }

        public static Dictionary<string, string> SortModes(Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if lexer grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType lp = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(lp, pd_parser.ParseTree);
            if (lp.Type != ExtractGrammarType.GrammarType.Lexer)
                throw new LanguageServerException("A lexer grammar file is not selected. Please select one first.");
            TableOfModes table = new TableOfModes(pd_parser, document);
            table.ReadModes();
            table.FindPartitions();

            // Find new order of modes.
            string old_code = document.Code;
            List<Pair<int, int>> reorder = new List<Pair<int, int>>();
            {
                List<string> ordered = table.modes
                    .Select(r => r.name)
                    .OrderBy(r => r).ToList();
                foreach (string s in ordered)
                {
                    TableOfModes.Row row = table.modes[table.name_to_index[s]];
                    reorder.Add(new Pair<int, int>(row.start_index, row.end_index));
                }
            }

            StringBuilder sb = new StringBuilder();
            int previous = 0;
            if (table.modes.Count == 0) return result;
            {
                int index_start = table.modes[0].start_index;
                int len = 0;
                string pre = old_code.Substring(previous, index_start - previous);
                sb.Append(pre);
                previous = index_start + len;
            }
            foreach (Pair<int, int> l in reorder)
            {
                int index_start = l.a;
                int len = l.b - l.a;
                string add = old_code.Substring(index_start, len);
                sb.Append(add);
            }
            //string rest = old_code.Substring(previous);
            //sb.Append(rest);
            string new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> Unfold(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var workspace = document.Workspace;

            // Check cursor position. It is either the LHS symbol of a rule,
            // which means the user wants to unroll all applied occurrences of the rule
            // or it is on a symbol in the RHS of the rule, which means the
            // user wants to unroll this specific applied occurrence of the rule.

            var defs = new Module().GetDefsLeaf(document);
            bool is_cursor_on_def = false;
            TerminalNodeImpl def = null;
            bool is_cursor_on_ref = false;
            IEnumerable<TerminalNodeImpl> refs = null;
            foreach (var d in defs)
            {
                bool a = IsContainedBy(d.Symbol.StartIndex, d.Symbol.StopIndex + 1, start, end);
                if (!a)
                    continue;

                is_cursor_on_def = true;
                // This means that user wants to unfold all occurrences on RHS,
                // not a specific instance.
                // This means that user wants to unfold a specific
                // instance of a RHS symbol.
                def = d;
                break;
            }
            {
                refs = new Module().GetRefsLeaf(document);
                refs = refs
                    .Where(r =>
                    {
                        pd_parser.Attributes.TryGetValue(r, out IList<CombinedScopeSymbol> list_value);
                        if (list_value == null) return false;
                        if (list_value.Count > 1) return false;
                        var value = list_value.First();
                        if (value == null) return false;
                        Domemtech.Symtab.ISymbol sym = value as Domemtech.Symtab.ISymbol;
                        if (sym == null) return false;
                        List<Domemtech.Symtab.ISymbol> list_of_syms = new List<Domemtech.Symtab.ISymbol>() { sym };
                        if (sym is RefSymbol) list_of_syms = sym.resolve();
                        if (list_of_syms.Count > 1) return false;
                        var s = list_of_syms.First();
                        if (!(s is NonterminalSymbol)) return false;
                        string def_file = s.file;
                        if (def_file == null) return false;
                        Workspaces.Document def_document = workspace.FindDocument(def_file);
                        if (def_document == null) return false;
                        ParsingResults def_pd = ParsingResultsFactory.Create(def_document);
                        if (def_pd == null) return false;
                        return true;
                    }).ToList();
                refs = refs
                    .Where(r =>
                    {
                        // Pick refs that are for def or overlap [start, end].
                        if (is_cursor_on_def)
                        {
                            pd_parser.Attributes.TryGetValue(r, out IList<CombinedScopeSymbol> list_value);
                            if (list_value == null) return false;
                            if (list_value.Count > 1) return false;
                            var value = list_value.First();
                            if (value == null) return false;
                            Domemtech.Symtab.ISymbol sym = value as Domemtech.Symtab.ISymbol;
                            if (sym == null) return false;
                            List<Domemtech.Symtab.ISymbol> list_of_syms = new List<Domemtech.Symtab.ISymbol>() { sym };
                            if (sym is RefSymbol) list_of_syms = sym.resolve();
                            if (list_of_syms.Count > 1) return false;
                            var s = list_of_syms.First();
                            if (s.Token.First().InputStream.SourceName != def.Symbol.InputStream.SourceName) return false;
                            if (s.Token.First().TokenIndex != def.Symbol.TokenIndex) return false;
                            return true;
                        }
                        else
                        {
                            IToken ta = pd_parser.TokStream.Get(r.SourceInterval.a);
                            var st = ta.StartIndex;
                            var ed = ta.StopIndex + 1;
                            bool a = IsOverlapping(st, ed, start, end);
                            return a;
                        }
                    }).ToList();
                if (!refs.Any())
                {
                    return result;
                }
                is_cursor_on_ref = true;
            }

            if (!(is_cursor_on_def || is_cursor_on_ref))
            {
                throw new LanguageServerException("Please position the cursor on either a LHS symbol (which means "
                                                  + " to replace all RHS occurrences of the symbol), or on a RHS symbol (which means"
                                                  + " to replace the specific RHS occurrence of the symbol, then try again.");
            }


            if (!refs.Any())
            {
                // You can't replace a symbol if there's no use.
                // Note that there's always one use as long as there's a
                // definition.
                throw new LanguageServerException("There is no use of the symbol "
                    //       + sym.name + ". Position the cursor to another symbol and try again."
                    );
            }

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            // Substitute RHS into all applied occurrences.
            foreach (var re in refs)
            {
                pd_parser.Attributes.TryGetValue(re, out IList<CombinedScopeSymbol> list_value);
                if (list_value == null) continue;
                if (list_value.Count > 1) continue;
                var value = list_value.First();
                if (value == null) continue;
                Domemtech.Symtab.ISymbol sym = value as Domemtech.Symtab.ISymbol;
                if (sym == null) continue;
                List<Domemtech.Symtab.ISymbol> list_of_syms = new List<Domemtech.Symtab.ISymbol>() { sym };
                if (sym is RefSymbol) list_of_syms = sym.resolve();
                if (list_of_syms.Count > 1) continue;
                var x = list_of_syms.First();
                if (!(x is NonterminalSymbol)) continue;
                // Find rule based on token for defining occurrence.
                var def_token = x.Token;
                var def_leaf = pd_parser.AllNodes.Where(
                    z =>
                    {
                        var z2 = z as TerminalNodeImpl;
                        if (z2 == null) return false;
                        return z2.Symbol?.TokenIndex == def_token.First().TokenIndex;
                    }).FirstOrDefault();
                IParseTree rule;
                for (rule = def_leaf; rule != null; rule = rule.Parent)
                {
                    if (rule is ANTLRv4Parser.ParserRuleSpecContext)
                        break;
                }
                // For symbol rule.RULE_REF(), replace occurrence in parse tree
                // with modified RHS list.
                var parser_rule = rule as ANTLRv4Parser.ParserRuleSpecContext;
                if (re.Symbol.InputStream.SourceName != pd_parser.FullFileName) continue;
                var td = document;
                ParsingResults pd = ParsingResultsFactory.Create(td) as ParsingResults;
                var rhs = parser_rule.ruleBlock();
                var pt = pd.ParseTree;
                var sym_pt = LanguageServer.Util.Find(start, td);
                if (sym_pt == null) throw new Exception("Inexplicably can't find document symbol in DoFold.");

                var s = new Module().GetDocumentSymbol(re.Symbol.StartIndex, td);
                if (s == null) throw new Exception("Inexplicably can't find document symbol in DoFold.");

                TreeEdits.Replace(pt,
                (in IParseTree t, out bool c) =>
                {
                    c = true;
                    if (!(t is ANTLRv4Parser.ElementContext))
                        return null;
                    var u = t as ANTLRv4Parser.ElementContext;
                    var id = u.atom()?.ruleref()?.RULE_REF();
                    if (id == null) return null;
                    if (id.GetText() != s.name) return null;
                    if (!(id is TerminalNodeImpl)) return null;
                    var tni = id as TerminalNodeImpl;
                    if (tni.Payload.StartIndex == re.Symbol.StartIndex
                        && tni.Payload.StopIndex == re.Symbol.StopIndex)
                    {
                        var element_p = t;
                        var alternative_p = t.Parent;
                        var element = element_p as ANTLRv4Parser.ElementContext;
                        var alternative = alternative_p as ANTLRv4Parser.AlternativeContext;
                        var ebnf_suffix = element.ebnfSuffix();

                        bool modified = false;
                        int i = 0;
                        for (; i < alternative.ChildCount; ++i)
                        {
                            if (alternative.children[i] == element)
                            {
                                modified = true;
                                break;
                            }
                        }
                        if (!modified) return null;

                        var env = new Dictionary<string, object>();
                        if (ebnf_suffix != null)
                            env.Add("suffix", TreeEdits.CopyTreeRecursive(ebnf_suffix, null, text_before));
                        env.Add("lparen", new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" }));
                        env.Add("rparen", new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" }));
                        var ruleAltList = rhs.ruleAltList();
                        bool first = true;
                        List<IParseTree> rhses = new List<IParseTree>();
                        foreach (var labeledAlt in ruleAltList.labeledAlt())
                        {
                            if (!first)
                            {
                                var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                                var new_or = new TerminalNodeImpl(token4);
                                rhses.Add(new_or);
                            }
                            first = false;
                            var a = labeledAlt.alternative();
                            rhses.Add(TreeEdits.CopyTreeRecursive(a, null, text_before));
                        }
                        env.Add("rhses", rhses);

                        var construct = new CTree.Class1(pd_parser.Parser, env);
                        var res = construct.CreateTree(
                                "( element ( ebnf ( block {lparen} ( altList {rhses} ) {rparen}) {suffix}))")
                            as ANTLRv4Parser.ElementContext;

                        return res;
                    }
                    return null;
                });
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        public static void Unfold(List<TerminalNodeImpl> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Check cursor position. It is either the LHS symbol of a rule,
            // which means the user wants to unroll all applied occurrences of the rule
            // or it is on a symbol in the RHS of the rule, which means the
            // user wants to unroll this specific applied occurrence of the rule.

            Dictionary<TerminalNodeImpl, IParseTree> defs = new Dictionary<TerminalNodeImpl, IParseTree>();

            foreach (var n in nodes)
            {
                var d = GetDef(n, pd_parser);
                if (d == null || d.Count == 0 || d.Count > 1) continue;
                var def = d.First();
                string def_file = def.file;
                if (def_file == null) continue;
                var workspace = document.Workspace;
                Document doc2 = workspace.FindDocument(def_file);
                if (doc2 == null) continue;
                if (!(ParsingResultsFactory.Create(doc2) is ParsingResults pd2)) continue;
                var tree = pd2.ParseTree;
                if (tree == null) continue;
                var lhs = def.Token;
                if (lhs == null) continue;
                TerminalNodeImpl lhs_term = TreeEdits.Find(lhs.First(), tree);
                if (lhs_term == null) continue;
                var parent = lhs_term.Parent;
                if (parent == null) continue;
                var p = parent as ParserRuleContext;
                if (p.RuleIndex == ANTLRv4Parser.RULE_parserRuleSpec)
                {
                    for (int i = 0; i < p.ChildCount; ++i)
                    {
                        var c = p.GetChild(i) as ParserRuleContext;
                        if (c != null && c.RuleIndex == ANTLRv4Parser.RULE_ruleBlock)
                        {
                            defs[n] = c;
                            break;
                        }
                    }
                }
                else if (p.RuleIndex == ANTLRv4Parser.RULE_lexerRuleSpec)
                {
                    for (int i = 0; i < p.ChildCount; ++i)
                    {
                        var c = p.GetChild(i) as ParserRuleContext;
                        if (c != null && c.RuleIndex == ANTLRv4Parser.RULE_lexerRuleBlock)
                        {
                            defs[n] = c;
                            break;
                        }
                    }
                }
            }

            if (defs.Count == 0)
            {
                throw new LanguageServerException("Please position the cursor on either a LHS symbol (which means"
                                                  + " to replace all RHS occurrences of the symbol), or on a RHS symbol (which means"
                                                  + " to replace the specific RHS occurrence of the symbol, then try again.");
            }

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            // Substitute RHS into all applied occurrences.
            foreach (var re in nodes)
            {
                var def = defs[re];
                TreeEdits.Replace(re, def);
            }
        }

        public static Dictionary<string, string> Unfold(
            List<TerminalNodeImpl> replace_these,
            List<IParseTree> all_sources,
            Parser parser,
            Lexer Lexer)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, IParseTree> rhs_replacement = new Dictionary<string, IParseTree>();
            foreach (var @ref in replace_these)
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ate = new AntlrTreeEditing.AntlrDOM.ConvertToDOM();
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = ate.Try(all_sources, parser))
                {
                    var nodes = engine.parseExpression(
                        @"//lexerRuleSpec[TOKEN_REF/text() = '" + @ref.GetText() + "']",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                    if (nodes.Count > 1 || nodes.Count == 0)
                        continue;

                }
            }
            return result;
        }

        static int fold_number = 0;

        public static Dictionary<string, string> Fold(List<IParseTree> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            if (!nodes.Any())
            {
                throw new LanguageServerException("XPath spec for LHS symbol empty.");
            }
            if (nodes.Count() > 1)
            {
                throw new LanguageServerException("XPath spec for LHS symbol specifies more than one node.");
            }

            var node = nodes.First();

            // Check cursor position. Many things can happen here, but we have to try
            // and make some sense of what the user is pointing out.
            // Check if node is the LHS symbol of a rule,
            // which means the user wants to fold all occurrences of the rule

            if (!(node is TerminalNodeImpl && node.Parent is ANTLRv4Parser.ParserRuleSpecContext))
                throw new LanguageServerException("Node for fold transform must be the LHS symbol.");

            TerminalNodeImpl def = null;
            TerminalNodeImpl sym_start = null;
            TerminalNodeImpl sym_end = null;
            def = sym_end = sym_start = node as TerminalNodeImpl;

            // Go up tree to find a common parent.
            List<IParseTree> lhs_path = new List<IParseTree>();
            List<IParseTree> rhs_path = new List<IParseTree>();
            for (var p = (IParseTree)sym_start; p != null; p = p.Parent) lhs_path.Insert(0, p);
            for (var p = (IParseTree)sym_end; p != null; p = p.Parent) rhs_path.Insert(0, p);

            int i = 0;
            for (; ; )
            {
                if (lhs_path[i] != rhs_path[i])
                {
                    --i;
                    break;
                }
                ++i;
                if (i >= lhs_path.Count || i > rhs_path.Count)
                {
                    --i;
                    break;
                }
            }
            if (i < 0)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            if (lhs_path[i] is ANTLRv4Parser.ParserRuleSpecContext)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            if (lhs_path[i] is ANTLRv4Parser.RuleAltListContext)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            int j = i;
            for (; j >= 0; --j)
            {
                if (lhs_path[j] is ANTLRv4Parser.ParserRuleSpecContext)
                    break;
            }
            if (j < 0)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            if (lhs_path[j] is ANTLRv4Parser.ParserRuleSpecContext
                && j + 2 == lhs_path.Count
                && sym_start != sym_end)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }

            bool replace_all = sym_start == sym_end && sym_start.Parent is ANTLRv4Parser.ParserRuleSpecContext;

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            if (replace_all)
            {
                // grab lhs of rule.
                var the_rule = lhs_path[j];
                var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var RHS = engine.parseExpression(
                            @"//parserRuleSpec[RULE_REF/text() = '" + def.GetText() + @"']
                            /ruleBlock
                                /ruleAltList",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).First();
                    var Possible = engine.parseExpression(
                            @"//parserRuleSpec
                            /ruleBlock
                                //altList",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();

                    foreach (var p in Possible)
                    {
                        if (p.GetText() == RHS.GetText())
                        { }
                    }
                }

                // Find complete RHS elsewhere and use LHS symbol if there's a match.
                var rule = lhs_path[j] as ANTLRv4Parser.ParserRuleSpecContext;
                ParsingResults pd = pd_parser;
                var pt = pd.ParseTree;
                var replace_name = rule.RULE_REF().GetText();
                var rule_block = rule.ruleBlock();
                var rule_alt_list = rule_block.ruleAltList();
                var str = rule_alt_list.GetText();
                foreach (var replace_this in TreeEdits.FindTopDown(pt,
                    (in IParseTree t, out bool c) =>
                    {
                        if (str == t.GetText())
                        {
                            for (var p = t; p != null; p = p.Parent)
                            {
                                if (p == rule)
                                {
                                    c = false;
                                    return null;
                                }
                            }
                            c = false;
                            return t;
                        }
                        c = true;
                        return null;
                    }))
                {
                    // Replace entire block.
                    // Create a new block with one symbol.
                    var new_block = new ANTLRv4Parser.BlockContext(null, 0);
                    {
                        var lparen_token = new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" };
                        var new_lparen = new TerminalNodeImpl(lparen_token);
                        new_block.AddChild(new_lparen);
                        new_lparen.Parent = new_block;
                        text_before.Add(new_lparen, " ");
                        ANTLRv4Parser.AltListContext l_alt = new ANTLRv4Parser.AltListContext(null, 0);
                        new_block.AddChild(l_alt);
                        l_alt.Parent = new_block;
                        var rparen_token = new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" };
                        var new_rparen = new TerminalNodeImpl(rparen_token);
                        new_block.AddChild(new_rparen);
                        new_rparen.Parent = new_block;
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                        var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                        new_alt.AddChild(new_element);
                        new_element.Parent = new_alt;
                        var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                        new_element.AddChild(new_atom);
                        new_atom.Parent = new_element;
                        var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                        new_atom.AddChild(new_ruleref);
                        new_ruleref.Parent = new_atom;
                        var token = new CommonToken(ANTLRv4Lexer.RULE_REF) { Line = -1, Column = -1, Text = replace_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        new_ruleref.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ruleref;

                        TreeEdits.Replace(pt,
                            (in IParseTree t, out bool c) =>
                            {
                                c = true;
                                if (t != replace_this)
                                    return null;
                                c = false;
                                return new_block;
                            });
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        public static Dictionary<string, string> Fold(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Check cursor position. Many things can happen here, but we have to try
            // and make some sense of what the user is pointing out.
            // It is either the LHS symbol of a rule,
            // which means the user wants to fold all occurrences of the rule
            // RHS or it is a selection of symbols in the RHS of the rule, which means the
            // user wants to fold this specific sequence and then create a new rule.

            var defs = new Module().GetDefsLeaf(document);
            bool is_cursor_on_def = false;
            TerminalNodeImpl def = null;
            foreach (var d in defs)
            {
                bool a = IsContainedBy(d.Symbol.StartIndex, d.Symbol.StopIndex + 1, start, end);
                if (!a)
                    continue;

                is_cursor_on_def = true;
                // This means that user wants to unfold all occurrences on RHS,
                // not a specific instance.
                // This means that user wants to unfold a specific
                // instance of a RHS symbol.
                def = d;
                break;
            }

            TerminalNodeImpl sym_start = null;
            TerminalNodeImpl sym_end = null;
            if (is_cursor_on_def)
            {
                // Selection is a single point.
                sym_end = sym_start = LanguageServer.Util.Find(start, document);
            }
            else
            {
                // Selection is of a list of characters. Go up the tree to find an
                // exact match.
                if (start >= end)
                {
                    var temp = end;
                    end = start;
                    start = temp;
                }
                sym_start = LanguageServer.Util.Find(start, document);
                sym_end = LanguageServer.Util.Find(end - 1, document);
            }
            if (sym_end == null || sym_start == null)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            // Go up tree to find a common parent.
            List<IParseTree> lhs_path = new List<IParseTree>();
            List<IParseTree> rhs_path = new List<IParseTree>();
            for (var p = (IParseTree)sym_start; p != null; p = p.Parent) lhs_path.Insert(0, p);
            for (var p = (IParseTree)sym_end; p != null; p = p.Parent) rhs_path.Insert(0, p);

            int i = 0;
            for (; ; )
            {
                if (lhs_path[i] != rhs_path[i])
                {
                    --i;
                    break;
                }
                ++i;
                if (i >= lhs_path.Count || i > rhs_path.Count)
                {
                    --i;
                    break;
                }
            }
            if (i < 0)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            if (lhs_path[i] is ANTLRv4Parser.ParserRuleSpecContext)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            if (lhs_path[i] is ANTLRv4Parser.RuleAltListContext)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            int j = i;
            for (; j >= 0; --j)
            {
                if (lhs_path[j] is ANTLRv4Parser.ParserRuleSpecContext)
                    break;
            }
            if (j < 0)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }
            if (lhs_path[j] is ANTLRv4Parser.ParserRuleSpecContext
                && j + 2 == lhs_path.Count
                && sym_start != sym_end)
            {
                throw new LanguageServerException("Please define a span within the RHS of a rule, or just the LHS symbol, then try again.");
            }

            bool replace_all = sym_start == sym_end && sym_start.Parent is ANTLRv4Parser.ParserRuleSpecContext;

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            if (replace_all)
            {
                // grab lhs of rule.
                var the_rule = lhs_path[j];
                var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
                using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                {
                    org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                    var RHS = engine.parseExpression(
                            @"//parserRuleSpec[RULE_REF/text() = '" + def.GetText() + @"']
                            /ruleBlock
                                /ruleAltList",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).First();
                    var Possible = engine.parseExpression(
                            @"//parserRuleSpec
                            /ruleBlock
                                //altList",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();

                    foreach (var p in Possible)
                    {
                        if (p.GetText() == RHS.GetText())
                        { }
                    }
                }
                // Find complete RHS elsewhere and use LHS symbol if there's a match.
                var rule = lhs_path[j] as ANTLRv4Parser.ParserRuleSpecContext;
                ParsingResults pd = pd_parser;
                var pt = pd.ParseTree;
                var replace_name = rule.RULE_REF().GetText();
                var rule_block = rule.ruleBlock();
                var rule_alt_list = rule_block.ruleAltList();
                var str = rule_alt_list.GetText();
                foreach (var replace_this in TreeEdits.FindTopDown(pt,
                    (in IParseTree t, out bool c) =>
                    {
                        if (str == t.GetText())
                        {
                            for (var p = t; p != null; p = p.Parent)
                            {
                                if (p == rule)
                                {
                                    c = false;
                                    return null;
                                }
                            }
                            c = false;
                            return t;
                        }
                        c = true;
                        return null;
                    }))
                {
                    // Replace entire block.
                    // Create a new block with one symbol.
                    var new_block = new ANTLRv4Parser.BlockContext(null, 0);
                    {
                        var lparen_token = new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" };
                        var new_lparen = new TerminalNodeImpl(lparen_token);
                        new_block.AddChild(new_lparen);
                        new_lparen.Parent = new_block;
                        text_before.Add(new_lparen, " ");
                        ANTLRv4Parser.AltListContext l_alt = new ANTLRv4Parser.AltListContext(null, 0);
                        new_block.AddChild(l_alt);
                        l_alt.Parent = new_block;
                        var rparen_token = new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" };
                        var new_rparen = new TerminalNodeImpl(rparen_token);
                        new_block.AddChild(new_rparen);
                        new_rparen.Parent = new_block;
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                        var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                        new_alt.AddChild(new_element);
                        new_element.Parent = new_alt;
                        var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                        new_element.AddChild(new_atom);
                        new_atom.Parent = new_element;
                        var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                        new_atom.AddChild(new_ruleref);
                        new_ruleref.Parent = new_atom;
                        var token = new CommonToken(ANTLRv4Lexer.RULE_REF) { Line = -1, Column = -1, Text = replace_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        new_ruleref.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ruleref;

                        TreeEdits.Replace(pt,
                            (in IParseTree t, out bool c) =>
                            {
                                c = true;
                                if (t != replace_this)
                                    return null;
                                c = false;
                                return new_block;
                            });
                    }
                }
            }
            else
            {
                var rule = lhs_path[j] as ANTLRv4Parser.ParserRuleSpecContext;
                ParsingResults pd = pd_parser;
                var pt = pd.ParseTree;
                var replace_this = lhs_path[i];
                string generated_name = "fold" + fold_number++;
                if (replace_this is ANTLRv4Parser.BlockContext
                    && replace_this.Parent is ANTLRv4Parser.EbnfContext)
                {
                    // Replace entire block.
                    // Create a new block with one symbol.
                    var new_block = new ANTLRv4Parser.BlockContext(null, 0);
                    {
                        var lparen_token = new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" };
                        var new_lparen = new TerminalNodeImpl(lparen_token);
                        new_block.AddChild(new_lparen);
                        new_lparen.Parent = new_block;
                        text_before.Add(new_lparen, " ");
                        ANTLRv4Parser.AltListContext l_alt = new ANTLRv4Parser.AltListContext(null, 0);
                        new_block.AddChild(l_alt);
                        l_alt.Parent = new_block;
                        var rparen_token = new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" };
                        var new_rparen = new TerminalNodeImpl(rparen_token);
                        new_block.AddChild(new_rparen);
                        new_rparen.Parent = new_block;
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                        var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                        new_alt.AddChild(new_element);
                        new_element.Parent = new_alt;
                        var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                        new_element.AddChild(new_atom);
                        new_atom.Parent = new_element;
                        var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                        new_atom.AddChild(new_ruleref);
                        new_ruleref.Parent = new_atom;
                        var token = new CommonToken(ANTLRv4Lexer.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        new_ruleref.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ruleref;

                        TreeEdits.Replace(pt,
                            (in IParseTree t, out bool c) =>
                            {
                                c = true;
                                if (t != replace_this)
                                    return null;
                                c = false;
                                return new_block;
                            });
                    }

                    // Now create a new rule.
                    var rule_spec = new ANTLRv4Parser.RuleSpecContext(null, 0);
                    {
                        ANTLRv4Parser.ParserRuleSpecContext new_ap_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                        rule_spec.AddChild(new_ap_rule);
                        new_ap_rule.Parent = rule_spec;
                        ANTLRv4Parser.ParserRuleSpecContext r = rule;
                        var lhs = generated_name;
                        {
                            var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                            var new_rule_ref = new TerminalNodeImpl(token);
                            text_before.Add(new_rule_ref,
                                System.Environment.NewLine + System.Environment.NewLine);
                            new_ap_rule.AddChild(new_rule_ref);
                            new_rule_ref.Parent = new_ap_rule;
                        }
                        // Now have "A'"
                        {
                            var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                            var new_colon = new TerminalNodeImpl(token2);
                            new_ap_rule.AddChild(new_colon);
                            new_colon.Parent = new_ap_rule;
                        }
                        // Now have "A' :"
                        ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                        {
                            ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(null, 0);
                            new_ap_rule.AddChild(new_rule_block_context);
                            new_rule_block_context.Parent = new_ap_rule;
                            new_rule_block_context.AddChild(rule_alt_list);
                            rule_alt_list.Parent = new_rule_block_context;
                        }
                        // Now have "A' : <rb <ral> >"
                        {
                            var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                            var new_semi = new TerminalNodeImpl(token3);
                            new_ap_rule.AddChild(new_semi);
                            new_semi.Parent = new_ap_rule;
                        }
                        // Now have "A : <rb <ral> > ;"
                        {
                            TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_ap_rule, text_before);
                        }
                        // Now have "A' : <rb <ral> > ; <eg>"
                        {
                            ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                            rule_alt_list.AddChild(l_alt);
                            l_alt.Parent = rule_alt_list;
                            // Create new alt "alpha A'".
                            ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                            l_alt.AddChild(new_alt);
                            new_alt.Parent = l_alt;
                            var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                            new_alt.AddChild(new_element);
                            new_element.Parent = new_alt;
                            var new_ebnf = new ANTLRv4Parser.EbnfContext(null, 0);
                            new_element.AddChild(new_ebnf);
                            new_ebnf.Parent = new_element;
                            TreeEdits.CopyTreeRecursive(replace_this, new_ebnf, text_before);
                        }

                        // Now have "A' : ... ;"
                        TreeEdits.InsertAfter(pt, (t) =>
                        {
                            if (t != rule.Parent)
                                return null;
                            return rule_spec;
                        });
                    }
                }
                else if (replace_this is ANTLRv4Parser.BlockContext
                    && replace_this is ANTLRv4Parser.AlternativeContext)
                {

                }
                else if (replace_this is ANTLRv4Parser.BlockContext
                    && replace_this is ANTLRv4Parser.EbnfContext)
                {

                }
                else if (replace_this is TerminalNodeImpl)
                {
                    // Replace the symbol on the RHS with another symbol.
                    var old_sym = replace_this as TerminalNodeImpl;
                    {
                        var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        text_before.Add(new_rule_ref, text_before[old_sym]);
                        TreeEdits.Replace(pt,
                            (in IParseTree t, out bool c) =>
                            {
                                c = true;
                                if (t != replace_this)
                                    return null;
                                c = false;
                                return new_rule_ref;
                            });
                    }

                    // Now create a new rule.
                    var rule_spec = new ANTLRv4Parser.RuleSpecContext(null, 0);
                    {
                        ANTLRv4Parser.ParserRuleSpecContext new_ap_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                        rule_spec.AddChild(new_ap_rule);
                        new_ap_rule.Parent = rule_spec;
                        ANTLRv4Parser.ParserRuleSpecContext r = rule;
                        var lhs = generated_name;
                        {
                            var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                            var new_rule_ref = new TerminalNodeImpl(token);
                            text_before.Add(new_rule_ref,
                                System.Environment.NewLine + System.Environment.NewLine);
                            new_ap_rule.AddChild(new_rule_ref);
                            new_rule_ref.Parent = new_ap_rule;
                        }
                        // Now have "A'"
                        {
                            var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                            var new_colon = new TerminalNodeImpl(token2);
                            new_ap_rule.AddChild(new_colon);
                            new_colon.Parent = new_ap_rule;
                        }
                        // Now have "A' :"
                        ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                        {
                            ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(null, 0);
                            new_ap_rule.AddChild(new_rule_block_context);
                            new_rule_block_context.Parent = new_ap_rule;
                            new_rule_block_context.AddChild(rule_alt_list);
                            rule_alt_list.Parent = new_rule_block_context;
                        }
                        // Now have "A' : <rb <ral> >"
                        {
                            var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                            var new_semi = new TerminalNodeImpl(token3);
                            new_ap_rule.AddChild(new_semi);
                            new_semi.Parent = new_ap_rule;
                        }
                        // Now have "A : <rb <ral> > ;"
                        {
                            TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_ap_rule, text_before);
                        }
                        // Now have "A' : <rb <ral> > ; <eg>"
                        {
                            ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                            rule_alt_list.AddChild(l_alt);
                            l_alt.Parent = rule_alt_list;
                            // Create new alt "alpha A'".
                            ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                            l_alt.AddChild(new_alt);
                            new_alt.Parent = l_alt;
                            var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                            new_alt.AddChild(new_element);
                            new_element.Parent = new_alt;
                            var new_ebnf = new ANTLRv4Parser.EbnfContext(null, 0);
                            new_element.AddChild(new_ebnf);
                            new_ebnf.Parent = new_element;
                            TreeEdits.CopyTreeRecursive(replace_this, new_ebnf, text_before);
                        }

                        // Now have "A' : ... ;"
                        TreeEdits.InsertAfter(pt, (t) =>
                        {
                            if (t != rule.Parent)
                                return null;
                            return rule_spec;
                        });
                    }
                }
                else if (replace_this is ANTLRv4Parser.AltListContext)
                {
                    // The only possible parent is a block, so just replace the
                    // altlist with new altlist with just the symbol.
                    {
                        ANTLRv4Parser.AltListContext l_alt = new ANTLRv4Parser.AltListContext(null, 0);
                        ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                        l_alt.AddChild(new_alt);
                        new_alt.Parent = l_alt;
                        var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                        new_alt.AddChild(new_element);
                        new_element.Parent = new_alt;
                        var new_atom = new ANTLRv4Parser.AtomContext(null, 0);
                        new_element.AddChild(new_atom);
                        new_atom.Parent = new_element;
                        var new_ruleref = new ANTLRv4Parser.RulerefContext(null, 0);
                        new_atom.AddChild(new_ruleref);
                        new_ruleref.Parent = new_atom;
                        var token = new CommonToken(ANTLRv4Lexer.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                        var new_rule_ref = new TerminalNodeImpl(token);
                        new_ruleref.AddChild(new_rule_ref);
                        new_rule_ref.Parent = new_ruleref;
                        TreeEdits.Replace(pt,
                            (in IParseTree t, out bool c) =>
                            {
                                c = true;
                                if (t != replace_this)
                                    return null;
                                c = false;
                                return l_alt;
                            });
                    }

                    // Now create a new rule.
                    var rule_spec = new ANTLRv4Parser.RuleSpecContext(null, 0);
                    {
                        ANTLRv4Parser.ParserRuleSpecContext new_ap_rule = new ANTLRv4Parser.ParserRuleSpecContext(null, 0);
                        rule_spec.AddChild(new_ap_rule);
                        new_ap_rule.Parent = rule_spec;
                        ANTLRv4Parser.ParserRuleSpecContext r = rule;
                        var lhs = generated_name;
                        {
                            var token = new CommonToken(ANTLRv4Parser.RULE_REF) { Line = -1, Column = -1, Text = generated_name };
                            var new_rule_ref = new TerminalNodeImpl(token);
                            text_before.Add(new_rule_ref,
                                System.Environment.NewLine + System.Environment.NewLine);
                            new_ap_rule.AddChild(new_rule_ref);
                            new_rule_ref.Parent = new_ap_rule;
                        }
                        // Now have "A'"
                        {
                            var token2 = new CommonToken(ANTLRv4Parser.COLON) { Line = -1, Column = -1, Text = ":" };
                            var new_colon = new TerminalNodeImpl(token2);
                            new_ap_rule.AddChild(new_colon);
                            new_colon.Parent = new_ap_rule;
                        }
                        // Now have "A' :"
                        ANTLRv4Parser.RuleAltListContext rule_alt_list = new ANTLRv4Parser.RuleAltListContext(null, 0);
                        {
                            ANTLRv4Parser.RuleBlockContext new_rule_block_context = new ANTLRv4Parser.RuleBlockContext(null, 0);
                            new_ap_rule.AddChild(new_rule_block_context);
                            new_rule_block_context.Parent = new_ap_rule;
                            new_rule_block_context.AddChild(rule_alt_list);
                            rule_alt_list.Parent = new_rule_block_context;
                        }
                        // Now have "A' : <rb <ral> >"
                        {
                            var token3 = new CommonToken(ANTLRv4Parser.SEMI) { Line = -1, Column = -1, Text = ";" };
                            var new_semi = new TerminalNodeImpl(token3);
                            new_ap_rule.AddChild(new_semi);
                            new_semi.Parent = new_ap_rule;
                        }
                        // Now have "A : <rb <ral> > ;"
                        {
                            TreeEdits.CopyTreeRecursive(r.exceptionGroup(), new_ap_rule, text_before);
                        }
                        // Now have "A' : <rb <ral> > ; <eg>"
                        {
                            ANTLRv4Parser.LabeledAltContext l_alt = new ANTLRv4Parser.LabeledAltContext(rule_alt_list, 0);
                            rule_alt_list.AddChild(l_alt);
                            l_alt.Parent = rule_alt_list;
                            // Create new alt "alpha A'".
                            ANTLRv4Parser.AlternativeContext new_alt = new ANTLRv4Parser.AlternativeContext(null, 0);
                            l_alt.AddChild(new_alt);
                            new_alt.Parent = l_alt;
                            var new_element = new ANTLRv4Parser.ElementContext(null, 0);
                            new_alt.AddChild(new_element);
                            new_element.Parent = new_alt;
                            var new_ebnf = new ANTLRv4Parser.EbnfContext(null, 0);
                            new_element.AddChild(new_ebnf);
                            new_ebnf.Parent = new_element;
                            TreeEdits.CopyTreeRecursive(replace_this, new_ebnf, text_before);
                        }

                        // Now have "A' : ... ;"
                        TreeEdits.InsertAfter(pt, (t) =>
                        {
                            if (t != rule.Parent)
                                return null;
                            return rule_spec;
                        });
                    }
                }
                else
#pragma warning disable CS0642 // Possible mistaken empty statement
                    ;
#pragma warning restore CS0642 // Possible mistaken empty statement
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        public static Dictionary<string, string> RemoveUselessParentheses(Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                || egt.Type == ExtractGrammarType.GrammarType.Combined
                || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }
            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            var old_code = pd_parser.Code;
            {
                ParsingResults pd = pd_parser;
                var pt = pd.ParseTree;
                var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
                {
                    List<IParseTree> parens1 = new List<IParseTree>();
                    using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                    {
                        org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                        // When can I remove "()"'s?
                        // 1) If the enclosed element in the parentheses is just one item, regardless
                        // if there is a suffix. E.g., (a)+, or (a), (x y z (a) b c).
                        // 2) If there is more than one element, then the parentheses can't be followed
                        // by a ?-, *-, +-operator. So, "(a b)+" is not equal to "a b+".
                        parens1 = engine.parseExpression(
                            @"//block
                            [
                               altList[count(alternative) = 1 and alternative[count(element) = 1]]
                            or altList[count(alternative) = 1 and alternative[count(element) > 1 and not(./../../../blockSuffix)]]
                            or altList[count(alternative) > 1 and alternative[count(element) = 1 and not(./../../../blockSuffix) and not(ancestor::alternative[count(element) > 1]) ]]
                            ]
                            /(LPAREN | RPAREN)",
                          new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                            .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();

                        // Delete parentheses, and adjust the "before_text" for
                        // correct off-channel tokens for the remaining subtree.
                        foreach (var p in parens1)
                        {
                            var term = p as TerminalNodeImpl;
                            if (term.GetText() == "(")
                            {
                                var save = text_before[term];
                                var parent = p.Parent;
                                TerminalNodeImpl after = TreeEdits.NextToken(term);
                                TreeEdits.Delete(p);
                                if (after != null)
                                {
                                    text_before[after] = save;
                                }
                            }
                            else
                            {
                                TreeEdits.Delete(p);
                            }
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != old_code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> ReplacePriorization(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
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

            bool replace_all = true;

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            if (replace_all)
            {
                ParsingResults pd = pd_parser;
                var pt = pd.ParseTree;
                foreach (var replace_this in TreeEdits.FindTopDown(pt,
                    (in IParseTree t, out bool c) =>
                    {
                        c = true;
                        if (t is ANTLRv4Parser.BlockContext)
                        {
                            var block = t as ANTLRv4Parser.BlockContext;
                            var p = block.Parent;
                            if (p is ANTLRv4Parser.EbnfContext)
                            {
                                var ebnf = p as ANTLRv4Parser.EbnfContext;
                                if (ebnf.blockSuffix() != null)
                                {
                                    return null;
                                }
                            }
                            var pp = p.Parent; // element
                            var ppp = pp.Parent; // alternative
                            var pppp = ppp.Parent; // altList or labeledAlt
                            if (pppp is ANTLRv4Parser.AltListContext)
                            {
                                if (pppp.ChildCount != 1)
                                    return null;
                            }
                            if (pppp is ANTLRv4Parser.LabeledAltContext)
                            {
                                if (pppp.ChildCount != 1)
                                    return null;
                            }
                            if (block.COLON() != null)
                            {
                                return null;
                            }
                            var alt_list = block.altList();
                            if (alt_list.ChildCount > 1 && ppp.ChildCount > 1)
                            {
                                return null;
                            }
                            return t;
                        }
                        if (t is ANTLRv4Parser.RuleBlockContext)
                        {

                        }
                        return null;
                    }))
                {
                    // Remove block by hoisting all parts of altList of the block into an element.
                    var block = replace_this as ANTLRv4Parser.BlockContext;
                    var ebnf = block?.Parent as ANTLRv4Parser.EbnfContext;
                    var element = ebnf?.Parent as ANTLRv4Parser.ElementContext;
                    var parent_alternative = element?.Parent as ANTLRv4Parser.AlternativeContext;
                    int i = 0;
                    for (; i < parent_alternative.ChildCount;)
                    {
                        if (parent_alternative.children[i] == element)
                            break;
                        ++i;
                    }
                    parent_alternative.children.RemoveAt(i);
                    var alt_list = block?.altList();
                    var alternatives = alt_list?.alternative();
                    if (alternatives.Length > 1)
                    {
                        IParseTree rule_alt_list_p = block;
                        for (; rule_alt_list_p != null; rule_alt_list_p = rule_alt_list_p.Parent)
                        {
                            if (rule_alt_list_p is ANTLRv4Parser.RuleAltListContext)
                                break;
                        }
                        var rule_alt_list = rule_alt_list_p as ANTLRv4Parser.RuleAltListContext;
                        bool first = true;
                        foreach (var alternative in alternatives)
                        {
                            if (!first)
                            {
                                var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                                var new_or = new TerminalNodeImpl(token4);
                                rule_alt_list.AddChild(new_or);
                                new_or.Parent = rule_alt_list;
                            }
                            first = false;
                            var labeled_alt = new ANTLRv4Parser.LabeledAltContext(null, 0);
                            TreeEdits.CopyTreeRecursive(alternative, labeled_alt, text_before);
                            rule_alt_list.AddChild(labeled_alt);
                            labeled_alt.Parent = rule_alt_list;
                        }
                    }
                    else
                    {
                        var alternative = alt_list?.GetChild(0) as ANTLRv4Parser.AlternativeContext;
                        foreach (var e in alternative.element())
                        {
                            var copy = TreeEdits.CopyTreeRecursive(e, null, text_before) as ANTLRv4Parser.ElementContext;
                            parent_alternative.children.Insert(i, copy);
                            copy.Parent = parent_alternative;
                            i++;
                        }
                    }
                }
            }
            else
            {
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }

            return result;
        }

        public static Dictionary<string, string> UpperLowerCaseLiteral(int start, int end, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Find keyword-like literals in lexer rules.
            List<TerminalNodeImpl> to_check_literals;
            List<ANTLRv4Parser.LexerElementsContext> elements;
            var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var dom_literals = engine.parseExpression(
                        @"//lexerRuleSpec
                        /lexerRuleBlock
                            /lexerAltList[not(@ChildCount > 1)]
                                /lexerAlt
                                    /lexerElements[not(@ChildCount > 1)]
                                        /lexerElement[not(@ChildCount > 1)]
                                            /lexerAtom
                                                /terminal[not(@ChildCount > 1)]
                                                    /STRING_LITERAL",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToArray();
                elements = engine.parseExpression(
                        "../../../..",
                        new StaticContextBuilder()).evaluate(dynamicContext, dom_literals)
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree as ANTLRv4Parser.LexerElementsContext).ToList();
                to_check_literals = dom_literals.Select(x => x.AntlrIParseTree as TerminalNodeImpl).ToList();
            }
            List<IParseTree> subs_elems = new List<IParseTree>();
            List<IParseTree> subs_lit = new List<IParseTree>();
            // Make up translation map. Note if the symbol is outside the range [start, end],
            // then don't add an entry for it.
            for (int i = 0; i < to_check_literals.Count; ++i)
            {
                var lexer_literal = to_check_literals[i];
                var elems = elements[i];
                var ok = true;
                var ll = lexer_literal;
                var tok = pd_parser.TokStream.Get(ll.SourceInterval.a);
                if (!IsOverlapping(tok.StartIndex, tok.StopIndex + 1, start, end))
                {
                    continue;
                }
                var s = ll.GetText();
                s = s.Substring(1).Substring(0, s.Length - 2);
                foreach (var cc in s)
                {
                    if (!char.IsLetterOrDigit(cc))
                    {
                        ok = false;
                        break;
                    }
                }
                if (!ok)
                {
                    continue;
                }
                subs_lit.Add(ll);
                subs_elems.Add(elems);
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            TreeEdits.Replace(pd_parser.ParseTree,
                (in IParseTree n, out bool c) =>
                {
                    c = true;
                    if (!subs_elems.Contains(n))
                        return null;
                    int i = subs_elems.IndexOf(n);
                    c = false;

                    // This node is the lexerElements node. Go through all leaves and convert
                    // a string literal to a list of lexer char sets.
                    // Then, construct a lexerElements node with the replacement sets.
                    var new_lexerelements = new ANTLRv4Parser.LexerElementsContext(null, 0);

                    var literal = subs_lit[i];
                    var s = literal.GetText();
                    s = s.Substring(1).Substring(0, s.Length - 2);
                    foreach (var cc in s)
                    {
                        var token = new CommonToken(ANTLRv4Lexer.LEXER_CHAR_SET)
                        {
                            Line = -1,
                            Column = -1,
                            Text =
                                "[" + char.ToLower(cc) + char.ToUpper(cc) + "]"
                        };
                        var set = new TerminalNodeImpl(token);
                        var construct = new CTree.Class1(pd_parser.Parser,
                            new Dictionary<string, object>()
                                {{"id", set}});
                        var la = construct.CreateTree("( lexerElement ( lexerAtom {id} ))") as ANTLRv4Parser.LexerElementContext;
                        new_lexerelements.AddChild(la);
                        la.Parent = new_lexerelements;
                    }
                    return new_lexerelements;
                });
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> UpperLowerCaseLiteral(List<TerminalNodeImpl> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            if (nodes != null)
            {
                foreach (var n in nodes)
                {
                    if (!(n is TerminalNodeImpl foo))
                    {
                        throw new LanguageServerException("Unexpected type of node--should be for a STRING_LITERAL terminal node");
                    }
                    if (foo.Symbol.Type != ANTLRv4Lexer.STRING_LITERAL)
                    {
                        throw new LanguageServerException("Unexpected type of node--should be for a STRING_LITERAL terminal node");
                    }
                }
            }

            // Find keyword-like literals in lexer rules.
            List<TerminalNodeImpl> to_check_lexer_rule_spec;
            var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var dom_lexer_rule_spec = engine.parseExpression(
                        @"
//lexerRuleSpec//STRING_LITERAL",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToArray();
                to_check_lexer_rule_spec = dom_lexer_rule_spec.Select(x => x.AntlrIParseTree as TerminalNodeImpl).ToList();
            }
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            for (int i = 0; i < to_check_lexer_rule_spec.Count; ++i)
            {
                var str = to_check_lexer_rule_spec[i];
                if (nodes != null && nodes.Any() && !nodes.Contains(str)) continue;
                if (str.Parent is ANTLRv4Parser.CharacterRangeContext) continue;
                if (str.Parent is ANTLRv4Parser.OptionValueContext) continue;
                if (str.Parent is ANTLRv4Parser.ElementOptionContext) continue;
                bool add_parens = false;
                for (IParseTree p = str; p != null; p = p.Parent)
                {
                    if (p is ANTLRv4Parser.LexerElementContext le)
                    {
                        if (le.ebnfSuffix() != null)
                        {
                            add_parens = true;
                        }
                        break;
                    }
                }
                StringBuilder rep_sb = new StringBuilder();
                var s_orig = str.GetText();
                // Strip single quotes from string literal.
                var s = s_orig.Substring(1).Substring(0, s_orig.Length - 2);
                // Make new string of upper and lowercase chars, and symbols.
                if (add_parens)
                {
                    rep_sb.Append("(");
                }
                for (int j = 0; j < s.Length; ++j)
                {
                    var cc = s[j];
                    if (Char.IsLetter(cc))
                    {
                        string rep = (Char.ToUpper(cc) == Char.ToLower(cc)) ? cc.ToString() : (Char.ToLower(cc).ToString() + Char.ToUpper(cc).ToString());
                        rep_sb.Append(" [" + rep + "]");
                    }
                    else if (cc == '[')
                    {
                        rep_sb.Append(@" [\[]");
                    }
                    else if (cc == ']')
                    {
                        rep_sb.Append(@" [\]]");
                    }
                    else if (cc == '-')
                    {
                        rep_sb.Append(@" [\-]");
                    }
                    else if (cc == '\\')
                    {
                        cc = s[++j];
                        rep_sb.Append(@" [\" + cc + "]");
                    }
                    else
                    {
                        rep_sb.Append(@" [" + cc + @"]");
                    }
                }
                if (add_parens)
                {
                    rep_sb.Append(" )");
                }
                var token = new CommonToken(ANTLRv4Lexer.STRING_LITERAL)
                {
                    Line = -1,
                    Column = -1,
                    Text = rep_sb.ToString()
                };
                var string_literal = new TerminalNodeImpl(token);
                var construct = new CTree.Class1(pd_parser.Parser,
                    new Dictionary<string, object>()
                        {{"id", string_literal}});
                TreeEdits.Replace(str, string_literal);
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> UnUpperLowerCaseLiteral(List<TerminalNodeImpl> nodes, bool uppercase, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Find keyword-like literals in lexer rules.
            List<ANTLRv4Parser.LexerRuleSpecContext> to_check_lexer_rule_spec;
            var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                var dom_literals = engine.parseExpression(
                        @"//lexerRuleSpec
[lexerRuleBlock/lexerAltList/lexerAlt/lexerElements/lexerElement/lexerAtom/LEXER_CHAR_SET 
and not(lexerRuleBlock//ebnfSuffix)
]",
                        new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement)).ToArray();
                to_check_lexer_rule_spec = dom_literals.Select(x => x.AntlrIParseTree as ANTLRv4Parser.LexerRuleSpecContext).ToList();
            }
            List<IParseTree> subs_elems = new List<IParseTree>();
            List<IParseTree> subs_lit = new List<IParseTree>();
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            for (int i = 0; i < to_check_lexer_rule_spec.Count; ++i)
            {
                var lexer_rule_spec = to_check_lexer_rule_spec[i];
                if (lexer_rule_spec.FRAGMENT() != null) continue;
                var lexer_elements = lexer_rule_spec?.lexerRuleBlock()?.lexerAltList()?.lexerAlt()[0]?.lexerElements();
                var lexer_element = lexer_elements?.lexerElement();
                // Verify each lexer element is correct.
                bool ok = true;
                foreach (var element in lexer_element)
                {
                    var st = element.lexerAtom()?.LEXER_CHAR_SET()?.GetText();
                    if (st == null || !(st.Length == 4 || st.Length == 3))
                    {
                        ok = false;
                        break;
                    }
                    for (int ii = 1; ii < st.Length - 1; ++ii)
                    {
                        if (ii == 1) continue;
                        if (st[ii] != Char.ToLower(st[ii-1]) && st[ii] != Char.ToUpper(st[ii-1]))
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (!ok) break;
                }
                if (!ok) continue;
                StringBuilder s = new StringBuilder();
                s.Append("'");
                foreach (var element in lexer_element)
                {
                    var st = element.lexerAtom().LEXER_CHAR_SET().GetText();
                    // Assume first char in set suffices.
                    var c = st[1];
                    c = uppercase ? Char.ToUpper(c) : Char.ToLower(c);
                    s.Append(c);
                }
                s.Append("'");
                var token = new CommonToken(ANTLRv4Lexer.STRING_LITERAL)
                {
                    Line = -1,
                    Column = -1,
                    Text = s.ToString()
                };
                var string_literal = new TerminalNodeImpl(token);
                var construct = new CTree.Class1(pd_parser.Parser,
                    new Dictionary<string, object>()
                        {{"id", string_literal}});
                var les = construct.CreateTree("( lexerElements ( lexerElement ( lexerAtom {id} )))");
                TreeEdits.Replace(lexer_elements, les);
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> Rename(List<TerminalNodeImpl> nodes, string new_text, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var node = nodes.First();
            var index = node.Symbol.StartIndex;
            IEnumerable<Location> locations = new Module().FindRefsAndDefs(index, document);
            var x1 = locations.Select(r => r.Uri).ToList();
            var documents = x1.Distinct().ToList();
            foreach (Document f in documents)
            {
                string fn = f.FullPath;
                IOrderedEnumerable<Location> per_file_changes = locations.Where(z => z.Uri == f).OrderBy(q => q.Range.Start.Value);
                StringBuilder sb = new StringBuilder();
                int previous = 0;
                string code = f.Code;
                foreach (Location l in per_file_changes)
                {
                    Document d = l.Uri;
                    string xx = d.FullPath;
                    var r = l.Range;
                    string pre = code.Substring(previous, r.Start.Value - previous);
                    sb.Append(pre);
                    sb.Append(new_text);
                    previous = r.End.Value + 1;
                }
                string rest = code.Substring(previous);
                sb.Append(rest);
                string new_code = sb.ToString();
                if (new_code != f.Code)
                {
                    result.Add(document.FullPath, new_code);
                }
            }
            return result;
        }

        class LocationComparer : IEqualityComparer<Location>
        {
            public new bool Equals(object x, object y)
            {
                if (x == null || y == null)
                    return false;
                var x1 = x as Location;
                var y1 = y as Location;
                if (x1 == null || y1 == null)
                    return false;
                if (x1.Uri?.FullPath != y1.Uri?.FullPath)
                    return false;
                if (x1.Range.Start.Value != y1.Range.Start.Value)
                    return false;
                if (x1.Range.End.Value != y1.Range.End.Value)
                    return false;
                return true;
            }

            public bool Equals(Location x1, Location y1)
            {
                if (x1 == null || y1 == null)
                    return false;
                if (x1.Uri?.FullPath != y1.Uri?.FullPath)
                    return false;
                if (x1.Range.Start.Value != y1.Range.Start.Value)
                    return false;
                if (x1.Range.End.Value != y1.Range.End.Value)
                    return false;
                return true;
            }

            public int GetHashCode(object obj)
            {
                var o1 = obj as Location;
                if (o1 == null)
                    return obj.GetHashCode();
                var hc = o1.Range.Start.Value;
                return hc;
            }

            public int GetHashCode(Location obj)
            {
                var o1 = obj as Location;
                if (o1 == null)
                    return obj.GetHashCode();
                var hc = o1.Range.Start.Value;
                return hc;
            }
        }

        public static Dictionary<string, string> Rename(Dictionary<string, string> rename_list, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            Dictionary<string, IEnumerable<Location>> locs_to_change = new Dictionary<string, IEnumerable<Location>>();
            HashSet<Location> hs = new HashSet<Location>();
            foreach (var pair in rename_list)
            {
                var old_name = pair.Key;
                var new_name = pair.Value;
                IEnumerable<Location> locs = new Module().FindRefsAndDefs(old_name, document);
                // Save rename locations for later application.
                locs_to_change[old_name] = locs;
                var nhs = hs.Union(locs, new LocationComparer()).ToHashSet();
                hs = nhs;
            }
            var all_locs = hs.ToList();
            var sorted_all_locs = all_locs.OrderByDescending(p => p.Range.Start.Value).ToList();
            var x1 = sorted_all_locs.Select(r => r.Uri).ToList();
            var documents = x1.Distinct().ToList();
            foreach (Document f in documents)
            {
                string fn = f.FullPath;
                IOrderedEnumerable<Location> per_file_changes = sorted_all_locs.Where(z => z.Uri == f).OrderBy(q => q.Range.Start.Value);
                StringBuilder sb = new StringBuilder();
                int previous = 0;
                string code = f.Code;
                foreach (Location l in per_file_changes)
                {
                    Document d = l.Uri;
                    string xx = d.FullPath;
                    var r = l.Range;
                    string old = code.Substring(r.Start.Value, 1 + r.End.Value - r.Start.Value);
                    string pre = code.Substring(previous, r.Start.Value - previous);
                    sb.Append(pre);
                    sb.Append(rename_list[old]);
                    previous = r.End.Value + 1;
                }
                string rest = code.Substring(previous);
                sb.Append(rest);
                string new_code = sb.ToString();
                if (new_code != f.Code)
                {
                    result.Add(fn, new_code);
                }
            }
            return result;
        }

        public static Dictionary<string, string> Delete(List<IParseTree> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            TreeEdits.Delete(nodes);

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            StringBuilder sb = new StringBuilder();

            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> InsertBefore(List<IParseTree> nodes, string text, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            foreach (var node in nodes)
            {
                var leaf = TreeEdits.Frontier(node).First();
                var inserted_node = TreeEdits.InsertBefore(node, text);
                // Nuke intertoken after text to next leaf node.
                text_before[inserted_node] = text_before[leaf];
                text_before[leaf] = " ";
            }

            StringBuilder sb = new StringBuilder();

            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> InsertAfter(List<IParseTree> nodes, string text, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            foreach (var node in nodes)
            {
                var leaf = TreeEdits.Frontier(node).First();
                var inserted_node = TreeEdits.InsertAfter(node, text);
                // Nuke intertoken after text to next leaf node.
                text_before[inserted_node] = " ";
            }

            StringBuilder sb = new StringBuilder();

            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> Replace(List<IParseTree> nodes, string text, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            foreach (var node in nodes)
            {
                var leaf = TreeEdits.Frontier(node).First();
                var inserted_node = TreeEdits.Replace(node, text);
                // Nuke intertoken after text to next leaf node.
                text_before[inserted_node] = text_before[leaf];
                text_before[leaf] = " ";
            }

            StringBuilder sb = new StringBuilder();

            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> MoveAfter(IEnumerable<IParseTree> from_list, IParseTree to, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            TreeEdits.MoveAfter(from_list, to);
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> MoveBefore(IEnumerable<IParseTree> from_list, IParseTree to, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);
            TreeEdits.MoveBefore(from_list, to);
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        class MyEqualityComparer : IEqualityComparer<IParseTree>
        {
            public bool Equals(IParseTree b1, IParseTree b2)
            {
                // Compare two parse trees. The node types should be the same
                // and if a leaf, the text the same.
                if (Object.ReferenceEquals(b1, null) && Object.ReferenceEquals(b2, null))
                {
                    return true;
                }
                if (Object.ReferenceEquals(b1, null))
                {
                    return false;
                }
                if (Object.ReferenceEquals(b2, null))
                {
                    return false;
                }
                if (!(b2 is IParseTree))
                {
                    return false;
                }

                {
                    var t1 = b1.GetType();
                    var t2 = b2.GetType();
                    if (!t1.Equals(t2))
                    {
                        return false;
                    }
                }
                var stack1 = new Stack<IParseTree>();
                var stack2 = new Stack<IParseTree>();
                stack1.Push(b1);
                stack2.Push(b2 as AttributedParseTreeNode);
                while (stack1.Any())
                {
                    var n1 = stack1.Pop();
                    var n2 = stack2.Pop();
                    var t1 = n1?.GetType();
                    var t2 = n2?.GetType();
                    if (!t1.Equals(t2))
                    {
                        return false;
                    }
                    if (n1 is TerminalNodeImpl)
                    {
                        var l1 = n1 as TerminalNodeImpl;
                        var l2 = n2 as TerminalNodeImpl;
                        if (l1.GetText() != l2.GetText())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (n1.ChildCount != n2.ChildCount)
                        {
                            return false;
                        }
                        for (int i = 0; i < n1.ChildCount; ++i)
                            stack1.Push(n1.GetChild(i));
                        for (int i = 0; i < n2.ChildCount; ++i)
                            stack2.Push(n2.GetChild(i));
                    }
                }
                {
                    return true;
                }
            }

            public int GetHashCode(IParseTree bx)
            {
                return 1;
            }

            //public static bool operator ==(AttributedParseTreeNode lhs, AttributedParseTreeNode rhs)
            //{
            //    // Check for null.
            //    if (Object.ReferenceEquals(lhs, null))
            //    {
            //        if (Object.ReferenceEquals(rhs, null))
            //        {
            //            // null == null = true.
            //            return true;
            //        }

            //        // Only the left side is null.
            //        return false;
            //    }
            //    // Equals handles the case of null on right side.
            //    return lhs.Equals(rhs);
            //}
            //public static bool operator !=(AttributedParseTreeNode lhs, AttributedParseTreeNode rhs)
            //{
            //    return !(lhs == rhs);
            //}
        }

        public static Dictionary<string, string> Group(List<IParseTree> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");

            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            // For all nodes that are an altList of some type,
            // apply the n-way merge for the whole collection.
            foreach (var node in nodes)
            {
                if (node is ANTLRv4Parser.RuleAltListContext altList1)
                {
                    var las = altList1.labeledAlt();

                    // Place in array of "strings" for n-way merge.
                    List<List<IParseTree>> exprs = new List<List<IParseTree>>();
                    for (int i = 0; i < las.Length; ++i)
                    {
                        // Make sure every labeledAlt is "simple".
                        var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var elements = engine.parseExpression(
                                    @"./alternative
                                        /element",
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { (las[i] as ObserverParserRuleContext).Observers.First() as AntlrNode })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            exprs.Add(elements);
                        }
                    }
                    Difdef<IParseTree> difdef = new Difdef<IParseTree>(exprs.Count, new MyEqualityComparer());
                    for (int x = 0; x < exprs.Count; ++x)
                    {
                        difdef.set_up_sequece(x, exprs[x]);
                    }
                    var diff = difdef.merge();

                    // Create new node to replace this ANTLRv4Parser.RuleAltListContext node.
                    {
                        var construct = new CTree.Class1(pd_parser.Parser, new Dictionary<string, object>());
                        var res = construct.CreateTree(
                            "( ruleAltList ( labeledAlt ( alternative ) ) )")
                                    as ANTLRv4Parser.RuleAltListContext;
                        var alternative = res.labeledAlt()[0].alternative();

                        int i = 0;
                        for (; i < diff.lines.Count;)
                        {
                            // Start at i. Find anchor at j.
                            int j = i;
                            for (; j < diff.lines.Count; ++j)
                            {
                                if (diff.lines[j].mask != diff.mask)
                                    break;
                            }

                            if (i != j)
                            {
                                // Bracket similar all between [i, j).
                                // These all have a full mask, no "or-ing".
                                for (int k = i; k < j; ++k)
                                    TreeEdits.AddChildren(alternative, new List<IParseTree>() { diff.lines[k].text as ANTLRv4Parser.ElementContext });

                                // Slide index i up.
                                i = j;
                                if (i == diff.lines.Count)
                                    break;
                            }

                            // Start at i, disimilar.
                            // Find anchor at j.
                            for (; j < diff.lines.Count; ++j)
                            {
                                if (diff.lines[j].mask == diff.mask)
                                    break;
                            }

                            if (i != j)
                            {
                                // Bracket disimilar all between [i, j).

                                // Now add parentheses and "|" for everything in between j+1 and l-1
                                // Create block.
                                var element_block = construct.CreateTree(
                                    "( element ( ebnf ( block ) ) )")
                                    as ANTLRv4Parser.ElementContext;
                                TreeEdits.AddChildren(alternative, new List<IParseTree>() { element_block });
                                var block = element_block.ebnf().block();
                                TreeEdits.AddChildren(block, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" }) });
                                var altList = construct.CreateTree(
                                    "( altList ( alternative ) )")
                                    as ANTLRv4Parser.AltListContext;
                                var sub_alternative = altList.alternative()[0];
                                TreeEdits.AddChildren(block, new List<IParseTree>() { altList });
                                bool firstfirst = true;
                                for (int f = 0; f < diff.dimension; ++f)
                                {
                                    if (!firstfirst)
                                    {
                                        TreeEdits.AddChildren(altList, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" }) });
                                        sub_alternative = construct.CreateTree(
                                            "( alternative )")
                                            as ANTLRv4Parser.AlternativeContext;
                                        TreeEdits.AddChildren(altList, new List<IParseTree>() { sub_alternative });
                                    }
                                    for (int k = i; k < j; ++k)
                                    {
                                        if ((diff.lines[k].mask & (1 << f)) != 0)
                                        {
                                            firstfirst = false;
                                            TreeEdits.AddChildren(sub_alternative,
                                                new List<IParseTree>() { diff.lines[k].text
                                            as ANTLRv4Parser.ElementContext });
                                        }
                                    }
                                }
                                TreeEdits.AddChildren(block, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" }) });

                                // Slide index i up.
                                i = j;
                                if (i == diff.lines.Count)
                                    break;
                            }
                        }

                        TreeEdits.Replace(altList1, res);
                    }
                }
                else if (node is ANTLRv4Parser.LexerAltListContext altList2)
                {
                    var @as = altList2.lexerAlt();
                    // Place in array of "strings" for n-way merge.
                    List<List<IParseTree>> exprs = new List<List<IParseTree>>();
                    for (int i = 0; i < @as.Length; ++i)
                    {
                        var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var elements = engine.parseExpression(
                                    @"./lexerElements/lexerElement",
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { (@as[i] as ObserverParserRuleContext).Observers.First() as AntlrNode })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            exprs.Add(elements);
                        }
                    }
                    Difdef<IParseTree> difdef = new Difdef<IParseTree>(exprs.Count, new MyEqualityComparer());
                    for (int x = 0; x < exprs.Count; ++x)
                    {
                        difdef.set_up_sequece(x, exprs[x]);
                    }
                    var diff = difdef.merge();

                    {
                        var construct = new CTree.Class1(pd_parser.Parser, new Dictionary<string, object>());
                        var res = construct.CreateTree(
                            "( lexerAltList ( lexerAlt ( lexerElements ) ) )")
                                    as ANTLRv4Parser.LexerAltListContext;
                        var alternative = res.lexerAlt()[0].lexerElements();

                        int i = 0;
                        for (; i < diff.lines.Count;)
                        {
                            // Start at i. Find anchor at j.
                            int j = i;
                            for (; j < diff.lines.Count; ++j)
                            {
                                if (diff.lines[j].mask != diff.mask)
                                    break;
                            }

                            if (i != j)
                            {
                                // Bracket similar all between [i, j).
                                // These all have a full mask, no "or-ing".
                                for (int k = i; k < j; ++k)
                                    TreeEdits.AddChildren(alternative, new List<IParseTree>() { diff.lines[k].text as ANTLRv4Parser.ElementContext });

                                // Slide index i up.
                                i = j;
                                if (i == diff.lines.Count)
                                    break;
                            }

                            // Start at i, disimilar.
                            // Find anchor at j.
                            for (; j < diff.lines.Count; ++j)
                            {
                                if (diff.lines[j].mask == diff.mask)
                                    break;
                            }

                            if (i != j)
                            {
                                // Bracket disimilar all between [i, j).

                                // Now add parentheses and "|" for everything in between j+1 and l-1
                                // Create block.
                                var element_block = construct.CreateTree(
                                    "( element ( ebnf ( block ) ) )")
                                    as ANTLRv4Parser.ElementContext;
                                TreeEdits.AddChildren(alternative, new List<IParseTree>() { element_block });
                                var block = element_block.ebnf().block();
                                TreeEdits.AddChildren(block, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" }) });
                                var altList = construct.CreateTree(
                                    "( altList ( alternative ) )")
                                    as ANTLRv4Parser.AltListContext;
                                var sub_alternative = altList.alternative()[0];
                                TreeEdits.AddChildren(block, new List<IParseTree>() { altList });
                                bool firstfirst = true;
                                for (int f = 0; f < diff.dimension; ++f)
                                {
                                    if (!firstfirst)
                                    {
                                        TreeEdits.AddChildren(altList, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" }) });
                                        sub_alternative = construct.CreateTree(
                                            "( alternative )")
                                            as ANTLRv4Parser.AlternativeContext;
                                        TreeEdits.AddChildren(altList, new List<IParseTree>() { sub_alternative });
                                    }
                                    for (int k = i; k < j; ++k)
                                    {
                                        if ((diff.lines[k].mask & (1 << f)) != 0)
                                        {
                                            firstfirst = false;
                                            TreeEdits.AddChildren(sub_alternative,
                                                new List<IParseTree>() { diff.lines[k].text
                                    as ANTLRv4Parser.ElementContext });
                                        }
                                    }
                                }
                                TreeEdits.AddChildren(block, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" }) });

                                // Slide index i up.
                                i = j;
                                if (i == diff.lines.Count)
                                    break;
                            }
                        }

                        TreeEdits.Replace(altList2, res);
                    }
                }
                else if (node is ANTLRv4Parser.AltListContext altList3)
                {
                    var @as = altList3.alternative();
                    // Place in array of "strings" for n-way merge.
                    List<List<IParseTree>> exprs = new List<List<IParseTree>>();
                    for (int i = 0; i < @as.Length; ++i)
                    {
                        var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
                        using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext = new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
                        {
                            org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                            var elements = engine.parseExpression(
                                    @"./element",
                                    new StaticContextBuilder()).evaluate(dynamicContext, new object[] { (@as[i] as ObserverParserRuleContext).Observers.First() as AntlrNode })
                                .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                            exprs.Add(elements);
                        }
                    }
                    Difdef<IParseTree> difdef = new Difdef<IParseTree>(exprs.Count, new MyEqualityComparer());
                    for (int x = 0; x < exprs.Count; ++x)
                    {
                        difdef.set_up_sequece(x, exprs[x]);
                    }
                    var diff = difdef.merge();

                    // Create new node to replace this ANTLRv4Parser.RuleAltListContext node.
                    {
                        var construct = new CTree.Class1(pd_parser.Parser, new Dictionary<string, object>());
                        var res = construct.CreateTree(
                                           "( ruleAltList ( labeledAlt ( alternative ) ) )")
                              as ANTLRv4Parser.RuleAltListContext;
                        var alternative = res.labeledAlt()[0].alternative();

                        int i = 0;
                        for (; i < diff.lines.Count;)
                        {
                            // Start at i. Find anchor at j.
                            int j = i;
                            for (; j < diff.lines.Count; ++j)
                            {
                                if (diff.lines[j].mask != diff.mask)
                                    break;
                            }

                            if (i != j)
                            {
                                // Bracket similar all between [i, j).
                                // These all have a full mask, no "or-ing".
                                for (int k = i; k < j; ++k)
                                    TreeEdits.AddChildren(alternative, new List<IParseTree>() { diff.lines[k].text as ANTLRv4Parser.ElementContext });

                                // Slide index i up.
                                i = j;
                                if (i == diff.lines.Count)
                                    break;
                            }

                            // Start at i, disimilar.
                            // Find anchor at j.
                            for (; j < diff.lines.Count; ++j)
                            {
                                if (diff.lines[j].mask == diff.mask)
                                    break;
                            }

                            if (i != j)
                            {
                                // Bracket disimilar all between [i, j).

                                // Now add parentheses and "|" for everything in between j+1 and l-1
                                // Create block.
                                var element_block = construct.CreateTree(
                                    "( element ( ebnf ( block ) ) )")
                                            as ANTLRv4Parser.ElementContext;
                                TreeEdits.AddChildren(alternative, new List<IParseTree>() { element_block });
                                var block = element_block.ebnf().block();
                                TreeEdits.AddChildren(block, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.LPAREN) { Line = -1, Column = -1, Text = "(" }) });
                                var altList = construct.CreateTree(
                                    "( altList ( alternative ) )")
                                          as ANTLRv4Parser.AltListContext;
                                var sub_alternative = altList.alternative()[0];
                                TreeEdits.AddChildren(block, new List<IParseTree>() { altList });
                                bool firstfirst = true;
                                for (int f = 0; f < diff.dimension; ++f)
                                {
                                    if (!firstfirst)
                                    {
                                        TreeEdits.AddChildren(altList, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" }) });
                                        sub_alternative = construct.CreateTree(
                                            "( alternative )")
                                            as ANTLRv4Parser.AlternativeContext;
                                        TreeEdits.AddChildren(altList, new List<IParseTree>() { sub_alternative });
                                    }
                                    for (int k = i; k < j; ++k)
                                    {
                                        if ((diff.lines[k].mask & (1 << f)) != 0)
                                        {
                                            firstfirst = false;
                                            TreeEdits.AddChildren(sub_alternative,
                                                new List<IParseTree>() { diff.lines[k].text as ANTLRv4Parser.ElementContext });
                                        }
                                    }
                                }
                                TreeEdits.AddChildren(block, new List<IParseTree>() { new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.RPAREN) { Line = -1, Column = -1, Text = ")" }) });

                                // Slide index i up.
                                i = j;
                                if (i == diff.lines.Count)
                                    break;
                            }
                        }

                        TreeEdits.Replace(altList3, res);
                    }
                }
            }
            
            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> Ungroup(List<IParseTree> nodes, Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            if (nodes != null)
            {
                foreach (var n in nodes)
                {
                    if (!(n is ANTLRv4Parser.ElementContext))
                        throw new Exception("Node isn't an Antlr4 ElementContext type");
                }
            }

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            // Ungroup each element which directly contains ()'s.
            foreach (var n in nodes)
            {
                var element = n as ANTLRv4Parser.ElementContext;
                if (element == null) continue;
                var alt_list = element.ebnf()?.block()?.altList();
                if (alt_list == null) continue;

                var alternative_parent = n.Parent as ANTLRv4Parser.AlternativeContext;
                if (alternative_parent == null) continue;
                var labeled_alt_parent = alternative_parent.Parent as ANTLRv4Parser.LabeledAltContext;
                if (labeled_alt_parent == null) continue;
                var rule_alt_list_parent = labeled_alt_parent?.Parent as ANTLRv4Parser.RuleAltListContext;
                if (rule_alt_list_parent == null) continue;

                var construct = new CTree.Class1(pd_parser.Parser, new Dictionary<string, object>());

                {
                    var new_labeled_alt_parent = construct.CreateTree(
                        "( labeledAlt )") as ANTLRv4Parser.LabeledAltContext;
                    TreeEdits.Replace(labeled_alt_parent, new_labeled_alt_parent);

                    bool first = true;
                    foreach (var alternative in alt_list.alternative())
                    {
                        if (!first)
                        {
                            var token4 = new CommonToken(ANTLRv4Lexer.OR) { Line = -1, Column = -1, Text = "|" };
                            var new_or = new TerminalNodeImpl(token4);
                            new_labeled_alt_parent.AddChild(new_or);
                            new_or.Parent = new_labeled_alt_parent;
                        }
                        first = false;

                        // Make a copy of all elements in this alternative before/after this element.
                        ANTLRv4Parser.AlternativeContext new_alternative_parent = TreeEdits.CopyTreeRecursive(alternative_parent, null, text_before) as ANTLRv4Parser.AlternativeContext;
                        // Nuke the corresponding child for element.
                        int i = 0;
                        for (i = 0; i < alternative_parent.children.Count; ++i)
                        {
                            if (alternative_parent.children[i] == element) break;
                        }
                        TreeEdits.Delete(new_alternative_parent.children[i]);
                        var copy = TreeEdits.CopyTreeRecursive(alternative, null, text_before) as ANTLRv4Parser.AlternativeContext;
                        new_alternative_parent.children.Insert(i, copy);
                        copy.Parent = new_alternative_parent;
                        new_labeled_alt_parent.AddChild(new_alternative_parent);
                        new_alternative_parent.Parent = new_labeled_alt_parent;
                    }
                }
            }
            StringBuilder sb = new StringBuilder();

            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static Dictionary<string, string> Delabel(Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            // Get all intertoken text immediately for source reconstruction.
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(pd_parser.TokStream, pd_parser.ParseTree);

            var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                {
                    var nodes = engine.parseExpression(
                            @"//labeledAlt/(POUND | identifier)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//labeledLexerElement/(identifier | ASSIGN | PLUS_ASSIGN)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//labeledElement/(identifier | ASSIGN | PLUS_ASSIGN)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }

        public static IParseTree NextSib(IParseTree p)
        {
            if (p == null) return null;
            var parent = p.Parent;
            if (parent == null) return null;
            for (int i = 0; i < parent.ChildCount; ++i)
            {
                var c = parent.GetChild(i);
                if (c == p && i != parent.ChildCount - 1)
                {
                    return parent.GetChild(i + 1);
                }
            }
            return null;
        }

        public static Dictionary<string, string> Strip(Document document)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // Check if initial file is a grammar.
            if (!(ParsingResultsFactory.Create(document) is ParsingResults pd_parser))
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            ExtractGrammarType egt = new ExtractGrammarType();
            ParseTreeWalker.Default.Walk(egt, pd_parser.ParseTree);
            bool is_grammar = egt.Type == ExtractGrammarType.GrammarType.Parser
                              || egt.Type == ExtractGrammarType.GrammarType.Combined
                              || egt.Type == ExtractGrammarType.GrammarType.Lexer;
            if (!is_grammar)
            {
                throw new LanguageServerException("A grammar file is not selected. Please select one first.");
            }

            var text_before = new Dictionary<TerminalNodeImpl, string>();

            var (tree, parser, lexer) = (pd_parser.ParseTree, pd_parser.Parser, pd_parser.Lexer);
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
                {
                    var nodes = engine.parseExpression(
                            @"//DOC_COMMENT",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//labeledAlt/(POUND | identifier)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//labeledLexerElement/(identifier | ASSIGN | PLUS_ASSIGN)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//labeledElement/(identifier | ASSIGN | PLUS_ASSIGN)",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//rulePrequel",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//ruleReturns",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//exceptionGroup",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//throwsSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//prequelConstruct",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//elementOptions",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//actionBlock",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes)
                    {
                        // Get next sibling.
                        var sib = NextSib(n);
                        if (sib != null)
                        {
                            if (sib is TerminalNodeImpl t)
                            {
                                if (t.Symbol.Type == ANTLRv4Parser.QUESTION)
                                {
                                    TreeEdits.Delete(sib);
                                }
                            }
                        }
                        TreeEdits.Delete(n);
                    }
                }
                {
                    var nodes = engine.parseExpression(
                            @"//argActionBlock",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    foreach (var n in nodes) TreeEdits.Delete(n);
                }
                {
                    var nodes = engine.parseExpression(
                            @"//parserRuleSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    var l1 = nodes.Select(n => TreeEdits.LeftMostToken(n)).ToList();
                    foreach (var l in l1) text_before[l] = "\n";
                }
                {
                    var nodes = engine.parseExpression(
                            @"//lexerRuleSpec",
                            new StaticContextBuilder()).evaluate(dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree)
                        .ToArray();
                    var l1 = nodes.Select(n => TreeEdits.LeftMostToken(n)).ToList();
                    foreach (var l in l1) text_before[l] = "\n";
                }
            }

            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, pd_parser.ParseTree, text_before);
            var new_code = sb.ToString();
            if (new_code != pd_parser.Code)
            {
                result.Add(document.FullPath, new_code);
            }
            return result;
        }
    }
}
