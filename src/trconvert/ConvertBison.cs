using Antlr4.Runtime;
using org.eclipse.wst.xml.xpath2.processor.util;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Linq;

namespace Trash
{
    internal class ConvertBison
    {
        public static void ToAntlr4(UnvParseTreeNode[] trees,
           Parser parser,
           Lexer lexer,
           string ffn)
        {
            // https://github.com/senseidb/sensei/pull/23

            // Remove unused options at top of grammar def.
            var engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//input_/prologue_declarations",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }

            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//input_/PercentPercent",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }

            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//input_/epilogue_opt",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }

            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//BRACED_CODE",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Delete(nodes);
            }
        }
    }
}
