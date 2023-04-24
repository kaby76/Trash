using org.w3c.dom;

namespace LanguageServer
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using ParseTreeEditing.UnvParseTreeDOM;
    using System.Data;

    public class ConvertAntlr3
    {
        public ConvertAntlr3()
        {
        }

        public void Try(UnvParseTreeNode[] trees,
            Parser parser,
            Lexer lexer,
            string ffn,
            string out_type = "antlr4")
        {
            var error_file_name = ffn;
            error_file_name = error_file_name.EndsWith(".g3")
                ? (error_file_name.Substring(0, error_file_name.Length - 3) + ".txt")
                : error_file_name;
            error_file_name = error_file_name.EndsWith(".g")
                ? (error_file_name.Substring(0, error_file_name.Length - 2) + ".txt")
                : error_file_name;

            var new_ffn = ffn;
            new_ffn = new_ffn.EndsWith(".g3")
                ? (new_ffn.Substring(0, new_ffn.Length - 3) + ".g4")
                : new_ffn;
            new_ffn = new_ffn.EndsWith(".g")
                ? (new_ffn.Substring(0, new_ffn.Length - 2) + ".g4")
                : new_ffn;

            Dictionary<string, string> results = new Dictionary<string, string>();
            var now = DateTime.Now.ToString();
            var errors = new StringBuilder();

            // Transforms derived from two sources:
            // https://github.com/senseidb/sensei/pull/23
            //var (text_before, other) = TreeEdits.TextToLeftOfLeaves(tokens, tree);

            // Remove unused options at top of grammar def.
            var engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (var dynamicContext = ate.Try(trees, parser))
            {
                // Allow language, tokenVocab, TokenLabelType, superClass
                var nodes = engine.parseExpression(
                        @"//grammarDef/optionsSpec
                            /option
                                [id_
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'output'
                                        or text() = 'backtrack'
                                        or text() = 'memoize'
                                        or text() = 'ASTLabelType'
                                        or text() = 'rewrite'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
                var options = engine.parseExpression(
                        @"//grammarDef/optionsSpec",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var os in options)
                {
                    var count = os.Children.Count();
                    if (count == 3) TreeEdits.Delete(os);
                }
            }

            // Fix options in the beginning of rules.
            // See https://theantlrguy.atlassian.net/wiki/spaces/ANTLR3/pages/2687029/Rule+and+subrule+options
            using (var dynamicContext = ate.Try(trees, parser))
            {
                // Allow language, tokenVocab, TokenLabelType, superClass
                var nodes = engine.parseExpression(
                        @"//rule_/optionsSpec
                            /option
                                [id_
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'output'
                                        or text() = 'backtrack'
                                        or text() = 'memoize'
                                        or text() = 'ASTLabelType'
                                        or text() = 'rewrite'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
                var options = engine.parseExpression(
                        @"//rule_/optionsSpec",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var os in options)
                {
                    var count = os.Children.Count();
                    if (count == 3) TreeEdits.Delete(os);
                }
            }

            // Use new tokens {} syntax
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//tokensSpec
                            /tokenSpec
                                /SEMI",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var node in nodes)
                {
                    if (node == nodes.Last())
                    {
                        // Delete tha last ";" in tokens list--change in syntax.
                        TreeEdits.Delete(node);
                        continue;
                    }

                    // Replace all remaining ";" with ",". 
                    TreeEdits.Replace(node, ",");
                }

                var equals = engine.parseExpression(
                    @"//tokensSpec
                            /tokenSpec[EQUAL]",
                    new StaticContextBuilder()).evaluate(
                    dynamicContext, new object[] { dynamicContext.Document }
                ).Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                // Antlr4 doesn't support assignment of lexer tokens.
                // Rewrite these as plain lexer rules.
                if (equals.Any())
                {
                    var new_lexer_rules = equals.Select(t =>
                    {
                        var lhsc = t.Children.ElementAt(0);
                        var lhs = lhsc.GetText();
                        var rhsc = t.Children.ElementAt(2);
                        var rhs = rhsc.GetText();
                        return new Tuple<string, string>(lhs, rhs);
                    }).ToList();
                    foreach (var e in equals)
                    {
                        // Order of delete important because we are using indices.
                        // Nuke "value".
                        var v = e.Children.ElementAt(2);
                        TreeEdits.Delete(v);
                        // Nuke "=".
                        var z = e.Children.ElementAt(1);
                        TreeEdits.Delete(z);
                    }
                    // Look for first lexer rule.
                    var last_rule = engine.parseExpression(
                          @"//rule_[id_/TOKEN_REF]",
                          new StaticContextBuilder()).evaluate(
                          dynamicContext, new object[] { dynamicContext.Document }
                          ).Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).FirstOrDefault();
                    if (last_rule != null)
                    {
                        var par = last_rule.ParentNode;
                        Node last = par;
                        last = TreeEdits.InsertBefore(last, Environment.NewLine + Environment.NewLine);
                        last = TreeEdits.InsertAfter(last, "// Token string literals converted to explicit lexer rules." + Environment.NewLine);
                        last = TreeEdits.InsertAfter(last, "// Reorder these rules accordingly." + Environment.NewLine + Environment.NewLine);
                        foreach (var p in new_lexer_rules)
                        {
                            var lhs = p.Item1;
                            var rhs = p.Item2;
                            var rule = lhs + ": " + rhs + ";" + Environment.NewLine;
                            last = TreeEdits.InsertAfter(last, rule);
                        }
                        last = TreeEdits.InsertAfter(last, "//" + Environment.NewLine + Environment.NewLine);
                    }
                }
            }

            // Note-- @rulecatch does not exist in Antlr4!
            // Remove unnecessary rulecatch block (use BailErrorStrategy instead)

            // Remove unsupported rewrite syntax and AST operators
            using (var dynamicContext = ate.Try(trees, parser))
            {
                // Note, for this operation, we are just deleting everything,
                // no conversion to a visitor.
                var n1 = engine.parseExpression(
                        @"//atom
                            /(ROOT | BANG)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (n1.Any())
                {
                    foreach (var n in n1) TreeEdits.Delete(n);
                }

                var n2 = engine.parseExpression(
                        @"//terminal_
                            /(ROOT | BANG)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (n2.Any())
                {
                    foreach (var n in n2) TreeEdits.Delete(n);
                }

                var n3 = engine.parseExpression(
                        @"//rewrite",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (n3.Any())
                {
                    foreach (var n in n3) TreeEdits.Delete(n);
                }

                var n4 = engine.parseExpression(
                        @"//rule_/BANG",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var n in n4) TreeEdits.Delete(n);
            }

            // Scopes are not in Antlr4 (equivalent are locals).
            // For now nuke.
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var rule_scope_spec = engine.parseExpression(
                        @"//rule_/ruleScopeSpec",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var n in rule_scope_spec) TreeEdits.Delete(rule_scope_spec);
            }

            // labels in lexer rules are not supported in ANTLR 4.
            // Nuke.
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var enos = engine.parseExpression(
                        @"//rule_[id/TOKEN_REF]
                            /altList
                                //elementNoOptionSpec
                                    [EQUAL or PEQ]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (enos.Any())
                {
                    foreach (var n in enos)
                    {
                        TreeEdits.Delete(n.ChildNodes.item(1) as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement);
                        TreeEdits.Delete(n.ChildNodes.item(0) as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement);
                    }
                }
            }

            // fragment rule cannot contain an action or command.
            // Nuke.
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var ab = engine.parseExpression(
                        @"//rule_[FRAGMENT]
                             /altList
                                 //elementNoOptionSpec
                                    /actionBlock[not(QM)]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (ab.Any())
                {
                    foreach (var n in ab) TreeEdits.Delete(n);
                }
            }

            // Remove syntactic predicates (unnecessary and unsupported in ANTLR 4)
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n2 = engine.parseExpression(
                        @"//ebnf
                            [SEMPREDOP]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
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
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n1 = engine.parseExpression(
                        @"//elementNoOptionSpec
                            [(actionBlock and QM)]
                                /SEMPREDOP",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                if (n1.Any())
                {
                    foreach (var n in n1) TreeEdits.Delete(n);
                }
            }

            // Remove unsupported option k=n (where n=1, 2, 3, ...)
            // Replace "( options { greedy=false; } : a | b | c )*" with
            // "(a | b | c)*?"
            using (var dynamicContext = ate.Try(trees, parser))
            {
                // Find k=n
                var k = engine.parseExpression(
                        @"//optionsSpec[not(../@id = 'grammarDef')]
                            /option
                                [id_
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'k'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                var k2 = engine.parseExpression(
                        @"//optionsSpec
                            /option
                                [id_
                                    /(TOKEN_REF | RULE_REF)
                                        [text() = 'k'
                                        ]]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                // greedy=false.
                var greedy = engine.parseExpression(
                        @"//optionsSpec[not(../@id = 'grammarDef')]
                            /option
                                [id_/(TOKEN_REF | RULE_REF)[text() = 'greedy']
                                and 
                                optionValue/id_/RULE_REF[text() = 'false']]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                var greedyOptionSpec = greedy.Select(t => t.ParentNode).ToList();
                var optionsSpec = engine.parseExpression(
                        @"//optionsSpec[not(../@id = 'grammarDef')]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement));
                // Nuke options.
                foreach (var tree in trees)
                {
                    TreeEdits.Delete(tree, (in UnvParseTreeNode n, out bool c) =>
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
                        var ss = UnvParseTreeElement.Reconstruct(os);
                        if (greedyOptionSpec.Contains(os) && os.ParentNode.LocalName == "block")
                        {
                            var block = os.ParentNode;
                            var block_parent = block.ParentNode as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement;
                            if (block_parent.LocalName == "ebnf")
                            {
                                while (block_parent.Children.Count() > 1)
                                {
                                    var l = block_parent.Children.Last();
                                    TreeEdits.Delete(l);
                                }
                                TreeEdits.InsertAfter(block_parent.Children.Last(), "*?");
                            }
                        }

                        if (os.Children.Count() == 3)
                        {
                            if (os.ParentNode.LocalName == "Block")
                            {
                                for (int i = 0; i < os.ParentNode.ChildNodes.Length; i++)
                                {
                                    if (os.ParentNode.ChildNodes.item(i).LocalName == "COLON")
                                        TreeEdits.Delete((UnvParseTreeNode)os.ParentNode.ChildNodes.item(i));
                                }
                            }
                            TreeEdits.Delete(os);
                        }
                    }

                    // Rewrite remaining action blocks that contain input, etc.
                    // input was renamed to _input in ANTLR 4.
                    // Use the channel lexer command.

                    // Antlr4 cannot perform '~' of a lexer alt list or a lexer symbol
                    // that isn't a set.
                    // Rewrite lexer rules foobar : ~(a | b | c), a : [...]; b : [...]; c : [...];
                    // Unfold all lexer symbols on RHS inside ~-operator.
                }
            }
        }
    }
}
