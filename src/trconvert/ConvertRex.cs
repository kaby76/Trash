using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;

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
        static int Validate(UnvParseTreeNode t)
        {
            var ty = t.GetType();
            int count = 0;
            if (t.ParentNode == null)
            {
                var nnn = t as UnvParseTreeElement;
                var k = nnn.GetText();
                System.Console.WriteLine("yo " + k);
                count++;
            }
            foreach (var c in t.Children)
            {
                count += Validate(c);
            }
            return count;
        }
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

            var grammar_name = Path.GetFileNameWithoutExtension( Path.GetFileName(ffn));

            var engine = new org.eclipse.wst.xml.xpath2.processor.Engine();
            var ate = new ParseTreeEditing.UnvParseTreeDOM.ConvertToDOM();
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//syntaxDefinition",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.InsertBefore(nodes.First(), "grammar " + grammar_name + ";" + Environment.NewLine);
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var nodes = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/RuleDef",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(nodes, ":");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n2 = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/*[last()]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.InsertAfter(n2, ";");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n3 = engine.parseExpression(
                        @"//lexicalDefinition/Tokens",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(n3, "// LEXER RULES");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n4 = engine.parseExpression(
                        @"//lexicalDefinition/lexicalProduction/RuleDef",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.Replace(n4, ":");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n5 = engine.parseExpression(
                        @"//lexicalDefinition/lexicalProduction/*[last()]",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                TreeEdits.InsertAfter(n5, ";");
            }
            using (var dynamicContext = ate.Try(trees, parser))
            {
                var n6 = engine.parseExpression(
                        @"//lexicalDefinition/lexicalProduction/name/Name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                var n7 = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/name/Name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                var mapping = new Dictionary<string, string>();
                foreach (var n in n6)
                {
                    var s = n.GetText();
                    if (s == null || s.Length == 0) continue;
                    mapping[s] = Char.ToUpper(s[0]) + s.Substring(1);
                }
                foreach (var n in n7)
                {
                    var s = n.GetText();
                    if (s == null || s.Length == 0) continue;
                    var v = Char.ToLower(s[0]);
                    var v2 = s.Substring(1);
                    mapping[s] = v + v2;
                }
                var n8 = engine.parseExpression(
                        @"//syntaxDefinition/syntaxProduction/name/Name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var n in n8)
                {
                    var nnn = n as UnvParseTreeElement;
                    var k = nnn.GetText();
                    mapping.TryGetValue(k, out string v);
                    if (v == null) continue;
                    TreeEdits.Replace(nnn, v);
                }
                var n9 = engine.parseExpression(
                        @"//syntaxProduction//nameOrString/name/Name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue)).ToList();
                foreach (var n in n9)
                {
                    var nnn = n as UnvParseTreeElement;
                    var k = nnn.GetText();
                    mapping.TryGetValue(k, out string v);
                    if (v == null) continue;
                    TreeEdits.Replace(nnn, v);
                }
                var n10 = engine.parseExpression(
                        @"//lexicalProduction/name/Name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var n in n10)
                {
                    var k = n.GetText();
                    mapping.TryGetValue(k, out string v);
                    if (v == null) continue;
                    TreeEdits.Replace(n, v);
                }
                var n11 = engine.parseExpression(
                        @"//lexicalProduction//lexicalPrimary/name/Name",
                        new StaticContextBuilder()).evaluate(
                        dynamicContext, new object[] { dynamicContext.Document })
                    .Select(x => (x.NativeValue as ParseTreeEditing.UnvParseTreeDOM.UnvParseTreeElement)).ToList();
                foreach (var n in n11)
                {
                    var k = n.GetText();
                    mapping.TryGetValue(k, out string v);
                    if (v == null) continue;
                    TreeEdits.Replace(n, v);
                }
            }
        }
    }
}

