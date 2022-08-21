namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using org.eclipse.wst.xml.xpath2.processor.util;

    public class ConvertAntlr3
    {
        public ConvertAntlr3() { }

        public Dictionary<string, string> Try(string ffn, string input, string out_type = "antlr4")
        {
            var error_file_name = ffn;
            error_file_name = error_file_name.EndsWith(".g3")
                ? (error_file_name.Substring(0, error_file_name.Length - 3) + ".txt") : error_file_name;
            error_file_name = error_file_name.EndsWith(".g")
                ? (error_file_name.Substring(0, error_file_name.Length - 2) + ".txt") : error_file_name;

            var new_ffn = ffn;
            new_ffn = new_ffn.EndsWith(".g3")
                ? (new_ffn.Substring(0, new_ffn.Length - 3) + ".g4") : new_ffn;
            new_ffn = new_ffn.EndsWith(".g")
                ? (new_ffn.Substring(0, new_ffn.Length - 2) + ".g4") : new_ffn;

            Dictionary<string, string> results = new Dictionary<string, string>();
            var now = DateTime.Now.ToString();
            var errors = new StringBuilder();
            var str = new AntlrInputStream(input);
            var lexer = new ANTLRv3Lexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ANTLRv3Parser(tokens);
            var elistener = new ErrorListener<IToken>(parser, lexer, 0);
            parser.AddErrorListener(elistener);
            var tree = parser.grammarDef();
            
            if (elistener.had_error)
            {
                results.Add(error_file_name, errors.ToString());
                return results;
            }
            else
            {
                errors.AppendLine("File " + ffn + " parsed successfully.");
                errors.AppendLine("Date: " + now);
            }

            // Transforms derived from two sources:
            // https://github.com/senseidb/sensei/pull/23
            var (text_before, other) = TreeEdits.TextToLeftOfLeaves(tokens, tree);

            // Remove unused options at top of grammar def.
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                // Allow language, tokenVocab, TokenLabelType, superClass
                var nodes = engine.parseExpression(
                        @"//grammarDef/optionsSpec
                            /option
                                [id
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'output'
                                        or text() = 'backtrack'
                                        or text() = 'memoize'
                                        or text() = 'ASTLabelType'
                                        or text() = 'rewrite'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                TreeEdits.Delete(nodes);
                var options = engine.parseExpression(
                        @"//grammarDef/optionsSpec",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var os in options)
                {
                    if (os.ChildCount == 3) TreeEdits.Delete(os);
                }
            }

            // Fix options in the beginning of rules.
            // See https://theantlrguy.atlassian.net/wiki/spaces/ANTLR3/pages/2687029/Rule+and+subrule+options
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                // Allow language, tokenVocab, TokenLabelType, superClass
                var nodes = engine.parseExpression(
                        @"//rule_/optionsSpec
                            /option
                                [id
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'output'
                                        or text() = 'backtrack'
                                        or text() = 'memoize'
                                        or text() = 'ASTLabelType'
                                        or text() = 'rewrite'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                TreeEdits.Delete(nodes);
                var options = engine.parseExpression(
                        @"//rule_/optionsSpec",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var os in options)
                {
                    if (os.ChildCount == 3) TreeEdits.Delete(os);
                }
            }

            // Use new tokens {} syntax
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var nodes = engine.parseExpression(
                        @"//tokensSpec
                            /tokenSpec
                                /SEMI",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (nodes.Any())
                {
                    // Delete tha last ";" in tokens list--change in syntax.
                    var last = nodes.Last();
                    TreeEdits.Delete(last);
                    // Replace all remaining ";" with ",". 
                    TreeEdits.Replace(tree, (in IParseTree n, out bool c) =>
                    {
                        c = true;
                        if (!nodes.Contains(n) && n != last) return null;
                        var t = n as TerminalNodeImpl;
                        var new_sym = new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.COMMA)
                        { Line = -1, Column = -1, Text = "," });
                        text_before.TryGetValue(t, out string v);
                        if (v != null)
                            text_before.Add(new_sym, v);
                        return new_sym;
                    });
                }
                var equals = engine.parseExpression(
                        @"//tokensSpec
                            /tokenSpec[EQUAL]",
                        new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document }
                    ).Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                // Antlr4 doesn't support assignment of lexer tokens.
                // Rewrite these as plain lexer rules.
                if (equals.Any())
                {
                    var new_lexer_rules = equals.Select(t =>
                    {
                        var lhs = (t as ANTLRv3Parser.TokenSpecContext).TOKEN_REF().GetText();
                        var rhs = t.GetChild(2).GetText();
                        return new Tuple<string, string>(lhs, rhs);
                    }).ToList();
                    foreach (var e in equals)
                    {
                        // Nuke "=".
                        TreeEdits.Delete(e.GetChild(1));
                        // Nuke "value".
                        TreeEdits.Delete(e.GetChild(1));
                    }
                    // Look for last lexer rule.
                    var last_rule = engine.parseExpression(
                          @"//rule_[id/TOKEN_REF]",
                          new StaticContextBuilder()).evaluate(
                          dynamicContext, new object[] { dynamicContext.Document }
                          ).Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).LastOrDefault();
                    if (last_rule != null)
                    {
                        var par = last_rule.Parent as ParserRuleContext;
                        foreach (var p in new_lexer_rules)
                        {
                            var lhs = p.Item1;
                            var rhs = p.Item2;
                            var env = new Dictionary<string, object>();
                            env.Add("lhs", new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.STRING_LITERAL) { Line = -1, Column = -1, Text = lhs }));
                            text_before[env["lhs"] as TerminalNodeImpl] = System.Environment.NewLine + System.Environment.NewLine;
                            env.Add("colon", new TerminalNodeImpl(new CommonToken(ANTLRv3Lexer.COLON) { Line = -1, Column = -1, Text = ":" }));
                            env.Add("rhs", new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.STRING_LITERAL) { Line = -1, Column = -1, Text = rhs }));
                            env.Add("semi", new TerminalNodeImpl(new CommonToken(ANTLRv4Lexer.SEMI) { Line = -1, Column = -1, Text = ";" }));
                            var construct = new CTree.Class1(parser, env);
                            var res = construct.CreateTree(
                                    "( rule_ {lhs} {colon} " +
                                    "   ( altList " +
                                    "      ( alternative " +
                                    "         ( element " +
                                    "            ( elementNoOptionSpec" +
                                    "               ( atom " +
                                    "                  {rhs} " +
                                    "   )  )  )  )  )" +
                                    "   {semi} " +
                                    ")");
                            par.AddChild(res as RuleContext);
                        }
                    }
                }
            }

            // Note-- @rulecatch does not exist in Antlr4!
            // Remove unnecessary rulecatch block (use BailErrorStrategy instead)

            // Remove unsupported rewrite syntax and AST operators
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                // Note, for this operation, we are just deleting everything,
                // no conversion to a visitor.
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var n1 = engine.parseExpression(
                        @"//atom
                            /(ROOT | BANG)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (n1.Any())
                {
                    foreach (var n in n1) TreeEdits.Delete(n);
                }
                var n2 = engine.parseExpression(
                        @"//terminal_
                            /(ROOT | BANG)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (n2.Any())
                {
                    foreach (var n in n2) TreeEdits.Delete(n);
                }
                var n3 = engine.parseExpression(
                        @"//rewrite",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (n3.Any())
                {
                    foreach (var n in n3) TreeEdits.Delete(n);
                }
                var n4 = engine.parseExpression(
                        @"//rule_/BANG",
                        new StaticContextBuilder()).evaluate(
                     dynamicContext, new object[] { dynamicContext.Document })
                     .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var n in n4) TreeEdits.Delete(n);
            }

            // Scopes are not in Antlr4 (equivalent are locals).
            // For now nuke.
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var rule_scope_spec = engine.parseExpression(
                        @"//rule_/ruleScopeSpec",
                        new StaticContextBuilder()).evaluate(
                     dynamicContext, new object[] { dynamicContext.Document })
                     .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                foreach (var n in rule_scope_spec) TreeEdits.Delete(rule_scope_spec);
            }

            // labels in lexer rules are not supported in ANTLR 4.
            // Nuke.
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var enos = engine.parseExpression(
                        @"//rule_[id/TOKEN_REF]
                            /altList
                                //elementNoOptionSpec
                                    [EQUAL or PEQ]",
                        new StaticContextBuilder()).evaluate(
                     dynamicContext, new object[] { dynamicContext.Document })
                     .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (enos.Any())
                {
                    foreach (var n in enos)
                    {
                        TreeEdits.Delete(n.GetChild(1));
                        TreeEdits.Delete(n.GetChild(0));
                    }
                }
            }

            // fragment rule cannot contain an action or command.
            // Nuke.
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var ab = engine.parseExpression(
                        @"//rule_[FRAGMENT]
                             /altList
                                 //elementNoOptionSpec
                                    /actionBlock[not(QM)]",
                        new StaticContextBuilder()).evaluate(
                     dynamicContext, new object[] { dynamicContext.Document })
                     .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (ab.Any())
                {
                    foreach (var n in ab) TreeEdits.Delete(n);
                }
            }

            // Remove syntactic predicates (unnecessary and unsupported in ANTLR 4)
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var n2 = engine.parseExpression(
                        @"//ebnf
                            [SEMPREDOP]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (n2.Any())
                {
                    foreach (var n in n2) TreeEdits.Delete(n);
                }
            }

            // Use the channel lexer command
            // Use locals[] instead of scope{}
            // Fix constructor name and invalid escape sequence
            // Create lexer rules for implicitly defined tokens
            // Use non-greedy matching in lexer rule

            // Semantic predicates do not need to be explicitly gated in ANTLR 4
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                var n1 = engine.parseExpression(
                        @"//elementNoOptionSpec
                            [(actionBlock and QM)]
                                /SEMPREDOP",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                if (n1.Any())
                {
                    foreach (var n in n1) TreeEdits.Delete(n);
                }
            }

            // Remove unsupported option k=3 (1, 2, 4, 5, ...)
            // Replace "( options { greedy=false; } : a | b | c )*" with
            // "(a | b | c)*?"
            using (AntlrTreeEditing.AntlrDOM.AntlrDynamicContext dynamicContext =
                    new AntlrTreeEditing.AntlrDOM.ConvertToDOM().Try(tree, parser))
            {
                org.eclipse.wst.xml.xpath2.processor.Engine engine =
                    new org.eclipse.wst.xml.xpath2.processor.Engine();
                // k=3 (1, 2, 4, 5, ...)
                var k = engine.parseExpression(
                        @"//optionsSpec[not(../@id = 'grammarDef')]
                            /option
                                [id
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'k'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                // greedy=false.
                var greedy = engine.parseExpression(
                        @"//optionsSpec[not(../@id = 'grammarDef')]
                            /option
                                [id/(TOKEN_REF | RULE_REF)[text() = 'greedy']
                                and 
                                optionValue/id/RULE_REF[text() = 'false']]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree).ToList();
                var greedyOptionSpec = greedy.Select(t => t.Parent).ToList();
                var optionsSpec = engine.parseExpression(
                       @"//optionsSpec[not(../@id = 'grammarDef')]",
                       new StaticContextBuilder()).evaluate(
                       dynamicContext, new object[] { dynamicContext.Document })
                   .Select(x => (x.NativeValue as AntlrTreeEditing.AntlrDOM.AntlrElement).AntlrIParseTree);
                // Nuke options.
                TreeEdits.Delete(tree, (in IParseTree n, out bool c) =>
                {
                    c = true;
                    if (k.Contains(n) || greedy.Contains(n))
                    {
                        return n;
                    }
                    return null;
                });
                foreach (var os in optionsSpec)
                {
                    if (greedyOptionSpec.Contains(os) && os.Parent is ANTLRv3Parser.BlockContext)
                    {
                        var block = os.Parent;
                        var block_parent = block.Parent;
                        if (block_parent is ANTLRv3Parser.EbnfContext ebnf)
                        {
                            while (ebnf.ChildCount > 1)
                            {
                                ebnf.children.RemoveAt(1);
                            }

                            ebnf.AddChild(new TerminalNodeImpl(new CommonToken(ANTLRv3Parser.STAR)
                            { Line = -1, Column = -1, Text = "*" }));
                            ebnf.AddChild(new TerminalNodeImpl(new CommonToken(ANTLRv3Parser.QM)
                            { Line = -1, Column = -1, Text = "?" }));
                        }
                    }
                    if (os.ChildCount == 3)
                    {
                        if (os.Parent is ANTLRv3Parser.BlockContext bc)
                        {
                            TreeEdits.Delete(bc.COLON());
                        }
                        TreeEdits.Delete(os);
                    }
                }
            }

            // Rewrite remaining action blocks that contain input, etc.
            // input was renamed to _input in ANTLR 4.
            // Use the channel lexer command.
            {
            }

            // Antlr4 cannot perform '~' of a lexer alt list or a lexer symbol
            // that isn't a set.
            // Rewrite lexer rules foobar : ~(a | b | c), a : [...]; b : [...]; c : [...];
            // Unfold all lexer symbols on RHS inside ~-operator.


            StringBuilder sb = new StringBuilder();
            TreeEdits.Reconstruct(sb, tree, text_before);
            var new_code = sb.ToString();
            results.Add(new_ffn, new_code);
            results.Add(ffn.Replace(".g", ".txt"), errors.ToString());

            return results;
        }
    }
}
