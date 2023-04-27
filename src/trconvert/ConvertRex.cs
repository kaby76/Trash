namespace Trash
{
    using Antlr4.Runtime;
    using org.eclipse.wst.xml.xpath2.processor.util;
    using org.w3c.dom;
    using ParseTreeEditing.UnvParseTreeDOM;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ConvertRex
    {
        public static void ToAntlr4(UnvParseTreeNode[] trees,
            Parser parser,
            Lexer lexer,
            string ffn)
        {
            //syntaxDefinition/syntaxProduction/Name = lhs
            //    /RuleDef = '::='
            //    ()? => same
            //    ()* => same
            //    '' => same
            //    $ => EOF
            // /* ws: definition */ -> hidden?
            //
            // <?TOKENS?> => separates lexer rules from parser rules.
            // #x4000 ... => \u4000.
            var engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/RuleDef",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(nodes, ":");
                var n2 = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/*[last()]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.InsertAfter(n2, ";");
                var n3 = engine.parseExpression(
                        @"//lexicalDefinition/Tokens",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(n3);
                var n4 = engine.parseExpression(
                        @"//lexicalDefinition/lexicalProduction/RuleDef",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(n4, ":");
                var n5 = engine.parseExpression(
                        @"//lexicalDefinition/lexicalProduction/*[last()]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.InsertAfter(n5, ";");

                var n6 = engine.parseExpression(
                        @"//lexicalDefinition/lexicalProduction/name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                var n7 = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                var mapping = new Dictionary<string, string>();

            }

        }
    }
}

