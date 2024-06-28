using Antlr4.Runtime;
using org.eclipse.wst.xml.xpath2.processor.util;
using org.w3c.dom;
using ParseTreeEditing.UnvParseTreeDOM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trash
{
    internal class ConvertAntlr4
    {
        public static void ToKocmanLLK(UnvParseTreeNode[] trees,
            Parser parser,
            Lexer lexer,
            string ffn)
        {
            var engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//(grammarDecl | optionsSpec | channelsSpec | options_)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }
            Node pp = null;
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//rules",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                pp = TreeEdits.InsertBefore(nodes.First(), "%%" + Environment.NewLine);
            }
            List<string> tokens;
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//lexerRuleSpec[not(.//lexerCommand/lexerCommandName/identifier/RULE_REF = 'skip')]/TOKEN_REF",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                tokens = nodes.Select(n => n.GetText()).ToList();
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//lexerRuleSpec",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }
            foreach (var t in tokens)
            {
                TreeEdits.InsertBefore(pp, "%token " + t + Environment.NewLine);
            }
        }

        public static void ToPegjs(UnvParseTreeNode[] trees,
            Parser parser,
            Lexer lexer,
            string ffn)
        {
            var engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//(grammarDecl | optionsSpec | channelsSpec | options_)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//(lexerRuleSpec/COLON | parserRuleSpec/COLON)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(nodes, "=");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//lexerRuleSpec/FRAGMENT",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//OR",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(nodes, "/");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//(lexerRuleSpec/SEMI | parserRuleSpec/SEMI)",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//ebnfSuffix[STAR and QUESTION]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var node in nodes)
                {
                    var n = node.Children.Last();
                    TreeEdits.Delete(n);
                }
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//lexerRuleSpec/lexerRuleBlock/lexerAltList/lexerAlt/lexerCommands/lexerCommand/lexerCommandName/identifier/RULE_REF[text()='skip']",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                bool got_intertoken = nodes.Any();
                List<string> intertoken_rule_names = new List<string>();
                foreach (var node in nodes)
                {
                    var rule = node
                            .ParentNode
                            .ParentNode
                            .ParentNode
                            .ParentNode
                            .ParentNode
                            .ParentNode
                            .ParentNode
                            .ParentNode
                        as UnvParseTreeElement;
                    var name_node = rule.Children.FirstOrDefault();
                    // Rename only if not whitespace.
                    var name = name_node.GetText();
                    intertoken_rule_names.Add(name);
                    var p = node
                            .ParentNode
                            .ParentNode
                            .ParentNode
                            .ParentNode
                        as UnvParseTreeElement;
                    var ch_list = p.Children.ToList();
                    foreach (var c in ch_list) TreeEdits.Delete(c);
                }
                if (got_intertoken)
                {
                    var inserts = engine.parseExpression(
                            @"//parserRuleSpec/ruleBlock//TOKEN_REF",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    foreach (var i in inserts) TreeEdits.InsertBefore(i, " _? ");
                    var inserts2 = engine.parseExpression(
                            @"//parserRuleSpec/ruleBlock//STRING_LITERAL",
                            new StaticContextBuilder()).evaluate(
                            dynamicContext, new object[] { dynamicContext.Document })
                        .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                    foreach (var i in inserts2) TreeEdits.InsertBefore(i, " _? ");
                    var last_rule = engine.parseExpression(
                        @"//lexerRuleSpec/TOKEN_REF",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document }
                    ).Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).FirstOrDefault();
                    if (last_rule != null)
                    {
                        var par = last_rule.ParentNode;
                        Node last = par;
                        last = TreeEdits.InsertBefore(last,
                            Environment.NewLine + Environment.NewLine
                            + "_ = "
                            + string.Join(" / ", intertoken_rule_names)
                            + Environment.NewLine + Environment.NewLine);
                    }
                }
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//notSet[setElement/LEXER_CHAR_SET]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var node in nodes)
                {
                    TreeEdits.Delete(node.Children.First());
                    var set = node.Children.First().GetText();
                    set = set.Insert(1, "^");
                    TreeEdits.Replace(node.Children.First(), set);
                }
            }
        }
    }
}
