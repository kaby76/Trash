using System;
using Antlr4.Runtime;
using xpath.org.eclipse.wst.xml.xpath2.processor.@internal;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2013 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     Bug 338494    - prohibiting xpath expressions starting with / or // to be parsed. 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

    using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
    using XPathExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathExpr;
    using XPath31Parser = xpath.org.eclipse.wst.xml.xpath2.processor.@internal.XPath31Parser;
    using XPath31Lexer = xpath.org.eclipse.wst.xml.xpath2.processor.@internal.XPath31Lexer;

    /// <summary>
    /// JFlexCupParser parses the xpath expression
    /// </summary>
    public class InternalXPathParser
    {

        /// <summary>
        /// Tries to parse the xpath expression
        /// </summary>
        /// <param name="xpath">
        ///            is the xpath string. </param>
        /// <param name="isRootlessAccess">
        ///            if 'true' then PsychoPath engine can't parse xpath expressions starting with / or //. </param>
        /// <exception cref="XPathParserException."> </exception>
        /// <returns> the xpath value.
        /// @since 2.0 </returns>
        public virtual XPath parse(string xpath, bool isRootlessAccess)
        {
            var lexer = new XPath31Lexer(new AntlrInputStream(xpath));
            try
            {
                lexer.RemoveErrorListeners();
                var lexer_error_listener = new ErrorListener<int>(null, lexer, 3);
                lexer.AddErrorListener(lexer_error_listener);
                var tokens = new CommonTokenStream(lexer);
                var parser = new XPath31Parser(tokens);
                parser.RemoveErrorListeners();
                var parser_error_listener = new ErrorListener<IToken>(parser, lexer, 3);
                parser.AddErrorListener(parser_error_listener);
                XPath31Parser.XpathContext parse_tree = parser.xpath();
                if (lexer_error_listener.had_error || parser_error_listener.had_error)
                    throw new XPathParserException("ANTLR parser error");
                var sb = new OutputParseTree().OutputTree(parse_tree, tokens, parser, true);
                // Compute AST representation of XPath expression.
                var visitor = XPathBuilderVisitor.INSTANCE;
                var xPath2 = (XPath)visitor.VisitXpath(parse_tree);
                xPath2.Expression = xpath;
                var o = new org.eclipse.wst.xml.xpath2.processor.@internal.OutputXPathExpression();
                var sb2 = o.OutputTree(xPath2).ToString();
                //System.Console.WriteLine("==============================");
                //System.Console.WriteLine("IR for expression \"" + xpath + "\"");
                //System.Console.WriteLine("Expression \"" + xpath + "\"");
                //System.Console.WriteLine(sb2.ToString());
                if (isRootlessAccess)
                {
                }
                return xPath2;
            }
            catch (StaticError e)
            {
                throw new XPathParserException(e.code(), e.Message);
            }
            catch (Exception e)
            {
                throw new XPathParserException(e.Message);
            }
        }

        private class DefaultVisitorAnonymousInnerClass : DefaultVisitor
        {
            private readonly InternalXPathParser outerInstance;

            public DefaultVisitorAnonymousInnerClass(InternalXPathParser outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public override object visit(XPathExpr e)
            {
                if (e.slashes() > 0)
                {
                    throw new XPathParserException("Access to root node is not allowed (set by caller)");
                }
                do
                {
                    e.expr().accept(this); // check the single step (may have filter with root access)
                    e = e.next(); // follow singly linked list of the path, it's all relative past the first one
                } while (e != null);
                return null;
            }
        }
    }

}