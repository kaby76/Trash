//using System.Collections.Generic;
//using Antlr4.Runtime.Tree;
//using org.eclipse.wst.xml.xpath2.api;

//namespace ConsoleApp1
//{
//    using Antlr4.Runtime;
//    using AntlrTreeEditing.AntlrDOM;
//    using org.eclipse.wst.xml.xpath2.processor;
//    using org.eclipse.wst.xml.xpath2.processor.util;
//    using System;
//    using System.Linq;
//    using CTree;
//    using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;

//    class Program
//    {
//        static void Main(string[] args)
//        {
//            {
//                org.eclipse.wst.xml.xpath2.processor.Engine engine = new Engine();
//                var input = System.IO.File.ReadAllText("../AntlrDOM/ANTLRv4Parser.g4");
//                var (tree, parser, lexer) = AntlrDOM.Parse.Try(input);
//                AntlrDynamicContext dynamicContext = AntlrDOM.ConvertToDOM.Try(tree, parser);


//                {
//                    var c1 = new CTree.Class1(parser, lexer, new Dictionary<string, IParseTree>());
//                    var c2 = c1.CreateTree("( ruleAltList ( labeledAlt ( alternative )))");
//                }


//                // Tests.
//                {
//                    var expression = engine.parseExpression("//ruleSpec", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    if (rs.size() != 65) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//atom", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    if (rs.size() != 244) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//ASSIGN", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    if (rs.size() != 1) throw new Exception();
//                }
//                {
//                    // Selects ( atom ( terminal ...))
//                    var expression = engine.parseExpression("//terminal/ancestor::atom", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    if (rs.size() != 113) throw new Exception();
//                    var first = rs.first();
//                    var ant = first.NativeValue as AntlrDOM.AntlrElement;
//                    if (!(ant.AntlrIParseTree is ANTLRv4Parser.AtomContext)) throw new Exception();
//                }
//                {
//                    // Selects all nodes.
//                    var expression = engine.parseExpression("//*", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    if (rs.size() != 2078) throw new Exception();
//                }

//                {
//                    var expression = engine.parseExpression("//*[@Start]", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                }

//                {
//                    var expression = engine.parseExpression("//*[@Start]//atom", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                }

//                {
//                    var expression = engine.parseExpression("//*[@Start='222']", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    int num = rs.size();
//                    OutputResultSet(expression, rs, parser);
//                    if (num != 7) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//*[@End='222']", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    int num = rs.size();
//                    OutputResultSet(expression, rs, parser);
//                    if (num != 3) throw new Exception();
//                }
//                {
//                    var expression =
//                        engine.parseExpression("//*[@Start='222' and @End='222']", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 3) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//ruleSpec//OR", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 52) throw new Exception();
//                    // atom and ruleref nodes in the parse tree.
//                }
//                if (false)
//                {
//                    // Not working.
//                    var expression = engine.parseExpression("//ruleSpec//'|'", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 51) throw new Exception();
//                    // atom and ruleref nodes in the parse tree.
//                }

//                {
//                    var expression = engine.parseExpression("//OR", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 52) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//*[not(self::OR)]", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 2078 - 52) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//*[text()]", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    int num = rs.size();
//                    if (num != 605) throw new Exception();
//                }
//                {
//                    var expression = engine.parseExpression("//*[text()='RBRACE']", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 3) throw new Exception();
//                }

//                {
//                    // Select first RHS parser rule symbol of rule.
//                    var expression = engine.parseExpression(
//                        "//parserRuleSpec/ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]",
//                        new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 55) throw new Exception();
//                    OutputResultSet(expression, rs, parser);
//                }
//                {
//                    // Select rules that have LHS name = 'atom'.
//                    var expression = engine.parseExpression("//parserRuleSpec[RULE_REF/text() = 'atom']",
//                        new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 1) throw new Exception();
//                    OutputResultSet(expression, rs, parser);
//                }
//                {
//                    // Check for any rules that have direct left recursion!
//                    var expression = engine.parseExpression(
//                        "//parserRuleSpec[RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]/text()]",
//                        new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 1) throw new Exception();
//                }
//                {
//                    // Find parserRuleSpec, picking all, then find RULE_REF under each parserRuleSpec.
//                    var expression = engine.parseExpression("//parserRuleSpec/RULE_REF", new StaticContextBuilder());
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 65) throw new Exception();
//                }
//                {
//                    // Find parserRuleSpec, picking all, then find RULE_REF under each parserRuleSpec.
//                    var e1 = engine.parseExpression("//parserRuleSpec", new StaticContextBuilder());
//                    object[] c1 = new object[] {dynamicContext.Document};
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    object[] contexts = rs1.Select(t => (object) (t.NativeValue)).ToArray();
//                    var expression = engine.parseExpression("RULE_REF", new StaticContextBuilder());
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 65) throw new Exception();
//                }
//                {
//                    // Find parserRuleSpec, picking all, then find RULE_REF under each parserRuleSpec.
//                    XPath2Expression e1 = engine.parseExpression("//parserRuleSpec", new StaticContextBuilder());
//                    object[] c1 = new object[] {dynamicContext.Document};
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    object[] contexts = rs1.Select(t => (object) (t.NativeValue)).ToArray();
//                    var expression = engine.parseExpression(".", new StaticContextBuilder());
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 65) throw new Exception();
//                }
//                {
//                    // Find parserRuleSpec, picking all, then find RULE_REF under each parserRuleSpec.
//                    var e1 = engine.parseExpression("//parserRuleSpec", new StaticContextBuilder());
//                    object[] c1 = new object[] {dynamicContext.Document};
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    object[] contexts = rs1.Select(t => (object) (t.NativeValue)).ToArray();
//                    var expression = engine.parseExpression("/RULE_REF", new StaticContextBuilder());
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    //if (num != 4) throw new Exception();
//                }
//                {
//                    // Check for any rules that have direct left recursion!
//                    object[] contexts = new object[] {dynamicContext.Document};
//                    var expression = engine.parseExpression(
//                        "//parserRuleSpec[RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]/text()]",
//                        new StaticContextBuilder());
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 1) throw new Exception();
//                }
//                {
//                    // Check for any rules that have direct left recursion!
//                    var e1 = engine.parseExpression("//parserRuleSpec", new StaticContextBuilder());
//                    object[] c1 = new object[] { dynamicContext.Document };
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    object[] contexts = rs1.Select(t => (object)(t.NativeValue)).ToArray();
//                    var expression = engine.parseExpression(
//                        ".[RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]/text()]",
//                        new StaticContextBuilder());
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                    if (num != 1) throw new Exception();
//                }
//                {
//                    // Check for any rules that have direct left recursion!
//                    //               var e1 = engine.parseExpression("//.[node() = altList]", new StaticContextBuilder());
//                    var e1 = engine.parseExpression("//altList | //labeledAlt", new StaticContextBuilder());
//                    object[] c1 = new object[] { dynamicContext.Document };
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    OutputResultSet(e1, rs1, parser);
//                    //object[] contexts = rs1.Select(t => (object)(t.NativeValue)).ToArray();
//                    //var expression = engine.parseExpression(
//                    //    ".[RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]/text()]",
//                    //    new StaticContextBuilder());
//                    //var rs = expression.evaluate(dynamicContext, contexts);
//                    //OutputResultSet(expression, rs, parser);
//                    //int num = rs.size();
//                    //if (num != 1) throw new Exception();
//                }
//                {
//                    // Check for any rules that have direct left recursion!
//                    //               var e1 = engine.parseExpression("//.[node() = altList]", new StaticContextBuilder());
//                    var e1 = engine.parseExpression("(//altList | //labeledAlt)", new StaticContextBuilder());
//                    object[] c1 = new object[] { dynamicContext.Document };
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    OutputResultSet(e1, rs1, parser);
//                    //object[] contexts = rs1.Select(t => (object)(t.NativeValue)).ToArray();
//                    //var expression = engine.parseExpression(
//                    //    ".[RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]/text()]",
//                    //    new StaticContextBuilder());
//                    //var rs = expression.evaluate(dynamicContext, contexts);
//                    //OutputResultSet(expression, rs, parser);
//                    //int num = rs.size();
//                    //if (num != 1) throw new Exception();
//                }
//                {
//                    // Check for any rules that have direct left recursion!
//                    //               var e1 = engine.parseExpression("//.[node() = altList]", new StaticContextBuilder());
//                    var e1 = engine.parseExpression("(//altList | //labeledAlt)[@ChildCount = 1]", new StaticContextBuilder());
//                    object[] c1 = new object[] { dynamicContext.Document };
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    OutputResultSet(e1, rs1, parser);
//                    //object[] contexts = rs1.Select(t => (object)(t.NativeValue)).ToArray();
//                    //var expression = engine.parseExpression(
//                    //    ".[RULE_REF/text() = ruleBlock/ruleAltList/labeledAlt/alternative/*[name()='element'][1]/atom/ruleref/*[1]/text()]",
//                    //    new StaticContextBuilder());
//                    //var rs = expression.evaluate(dynamicContext, contexts);
//                    //OutputResultSet(expression, rs, parser);
//                    //int num = rs.size();
//                    //if (num != 1) throw new Exception();
//                }
//            }
//            {
//                org.eclipse.wst.xml.xpath2.processor.Engine engine = new Engine();
//                var input = System.IO.File.ReadAllText("../AntlrDOM/ANTLRv4Lexer.g4");
//                var (tree, parser, lexer) = AntlrDOM.Parse.Try(input);
//                AntlrDynamicContext dynamicContext = AntlrDOM.ConvertToDOM.Try(tree, parser);
//                {
//                    //  ( rules
//                    //    ( ruleSpec
//                    //      ( lexerRuleSpec
//                    //        ( OFF_CHANNEL text=\r\n\r\n
//                    //        )
//                    //        ( DEFAULT_TOKEN_CHANNEL i=5 txt=A tt=1
//                    //        )
//                    //        ( OFF_CHANNEL text=
//                    //        )
//                    //        ( DEFAULT_TOKEN_CHANNEL i=7 txt=: tt=29
//                    //        )
//                    //        ( lexerRuleBlock
//                    //          ( lexerAltList
//                    //            ( lexerAlt
//                    //              ( lexerElements
//                    //                ( lexerElement
//                    //                  ( lexerAtom
//                    //                    ( terminal
//                    //                      ( OFF_CHANNEL text=
//                    //                      )
//                    //                      ( DEFAULT_TOKEN_CHANNEL i=9 txt='abc' tt=8
//                    //        ) ) ) ) ) ) ) )
//                    //        ( DEFAULT_TOKEN_CHANNEL i=10 txt=; tt=32
//                    //    ) ) )
//                    //    ( ruleSpec
//                    //      ( lexerRuleSpec
//                    //        ( OFF_CHANNEL text=\r\n
//                    //        )
//                    //        ( DEFAULT_TOKEN_CHANNEL i=12 txt=B tt=1
//                    //        )
//                    //        ( OFF_CHANNEL text=
//                    //        )
//                    //        ( DEFAULT_TOKEN_CHANNEL i=14 txt=: tt=29
//                    //        )
//                    //        ( lexerRuleBlock
//                    //          ( lexerAltList
//                    //            ( lexerAlt
//                    //              ( lexerElements
//                    //                ( lexerElement
//                    //                  ( lexerAtom
//                    //                    ( OFF_CHANNEL text=
//                    //                    )
//                    //                    ( DEFAULT_TOKEN_CHANNEL i=16 txt=[Aa] tt=3
//                    //                ) ) )
//                    //                ( lexerElement
//                    //                  ( lexerAtom
//                    //                    ( DEFAULT_TOKEN_CHANNEL i=17 txt=[Bb] tt=3
//                    //                ) ) )
//                    //                ( lexerElement
//                    //                  ( lexerAtom
//                    //                    ( DEFAULT_TOKEN_CHANNEL i=18 txt=[Cc] tt=3
//                    //        ) ) ) ) ) ) )
//                    //        ( DEFAULT_TOKEN_CHANNEL i=19 txt=; tt=32
//                    //  ) ) ) )
//                    // Find lexer rules with RHS symbol that is only keyword chars.
//                    var e1 = engine.parseExpression("//lexerRuleSpec//lexerRuleBlock//STRING_LITERAL", new StaticContextBuilder());
//                    object[] c1 = new object[] { dynamicContext.Document };
//                    var rs1 = e1.evaluate(dynamicContext, c1);

//                    foreach (var t in rs1)
//                    {
//                        var native = (AntlrElement)t.NativeValue;
//                        var s = native.AntlrIParseTree.GetText();
//                        s = s.Substring(1).Substring(0, s.Length - 2);
//                        bool ok = true;
//                        foreach (var cc in s)
//                        {
//                            if (!char.IsLetterOrDigit(cc))
//                            {
//                                ok = false;
//                                break;
//                            }
//                        }
//                        if (ok) System.Console.WriteLine(native.AntlrIParseTree.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.GetText());
//                    }
//                }
//                {
//                    var e1 = engine.parseExpression("//lexerRuleSpec/lexerRuleBlock/lexerAltList[count(*) = 1]/lexerAlt/lexerElements[count(*) = 1]/lexerElement[count(*) = 1]/lexerAtom/terminal/STRING_LITERAL", new StaticContextBuilder());
//                    object[] c1 = new object[] { dynamicContext.Document };
//                    var rs1 = e1.evaluate(dynamicContext, c1);
//                    object[] contexts = rs1.Select(t => (object)(t.NativeValue)).ToArray();
//                    var expression = engine.parseExpression(
//                        "./../../../../../../../../TOKEN_REF",
//                        new StaticContextBuilder());
//                    var rs = expression.evaluate(dynamicContext, contexts);
//                    OutputResultSet(expression, rs, parser);
//                    int num = rs.size();
//                }
//            }
//        }

//        private static void OutputResultSet(XPath2Expression expression, ResultSequence rs, Parser parser)
//        {
//            System.Console.WriteLine();
//            System.Console.WriteLine("==============================");
//            System.Console.WriteLine("Result set for \"" + expression.Expression + "\"");
//            System.Console.WriteLine("result size " + rs.size());
//            for (var i = rs.iterator(); i.MoveNext();)
//            {
//                var r = i.Current;
//                var node = r.NativeValue as AntlrNode;
//                var iparsetree = node?.AntlrIParseTree;
//                System.Console.WriteLine(
//                    new xpath.org.eclipse.wst.xml.xpath2.processor.@internal.OutputParseTree().OutputTree(iparsetree, parser.InputStream as CommonTokenStream, false).ToString());
//            }
//        }
//    }
//}
