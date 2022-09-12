using System;

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: ExsltDynamic.java 468639 2006-10-28 06:52:33Z minchau $
 */
namespace org.apache.xalan.lib
{


	using ExpressionContext = org.apache.xalan.extensions.ExpressionContext;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using NodeSet = org.apache.xpath.NodeSet;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using Text = org.w3c.dom.Text;

	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;

	/// <summary>
	/// This class contains EXSLT dynamic extension functions.
	/// 
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:dyn="http://exslt.org/dynamic"
	/// </pre>
	/// The documentation for each function has been copied from the relevant
	/// EXSLT Implementer page.
	/// </summary>
	/// <seealso cref= <a href="http://www.exslt.org/">EXSLT</a>
	/// 
	/// @xsl.usage general </seealso>
	public class ExsltDynamic : ExsltBase
	{

	   public const string EXSL_URI = "http://exslt.org/common";

	  /// <summary>
	  /// The dyn:max function calculates the maximum value for the nodes passed as 
	  /// the first argument, where the value of each node is calculated dynamically 
	  /// using an XPath expression passed as a string as the second argument. 
	  /// <para>
	  /// The expressions are evaluated relative to the nodes passed as the first argument.
	  /// In other words, the value for each node is calculated by evaluating the XPath 
	  /// expression with all context information being the same as that for the call to 
	  /// the dyn:max function itself, except for the following:
	  /// </para>
	  /// <para>
	  /// <ul>
	  ///  <li>the context node is the node whose value is being calculated.</li>
	  ///  <li>the context position is the position of the node within the node set passed as 
	  ///   the first argument to the dyn:max function, arranged in document order.</li>
	  ///  <li>the context size is the number of nodes passed as the first argument to the 
	  ///   dyn:max function.</li>
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// The dyn:max function returns the maximum of these values, calculated in exactly 
	  /// the same way as for math:max. 
	  /// </para>
	  /// <para>
	  /// If the expression string passed as the second argument is an invalid XPath 
	  /// expression (including an empty string), this function returns NaN. 
	  /// </para>
	  /// <para>
	  /// This function must take a second argument. To calculate the maximum of a set of 
	  /// nodes based on their string values, you should use the math:max function.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="myContext"> The ExpressionContext passed by the extension processor </param>
	  /// <param name="nl"> The node set </param>
	  /// <param name="expr"> The expression string
	  /// </param>
	  /// <returns> The maximum evaluation value </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double max(org.apache.xalan.extensions.ExpressionContext myContext, org.w3c.dom.NodeList nl, String expr) throws org.xml.sax.SAXNotSupportedException
	  public static double max(ExpressionContext myContext, NodeList nl, string expr)
	  {

		XPathContext xctxt = null;
		if (myContext is XPathContext.XPathExpressionContext)
		{
		  xctxt = ((XPathContext.XPathExpressionContext) myContext).XPathContext;
		}
		else
		{
		  throw new SAXNotSupportedException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_CONTEXT_PASSED, new object[]{myContext}));
		}

		if (string.ReferenceEquals(expr, null) || expr.Length == 0)
		{
		  return Double.NaN;
		}

		NodeSetDTM contextNodes = new NodeSetDTM(nl, xctxt);
		xctxt.pushContextNodeList(contextNodes);

		double maxValue = - double.MaxValue;
		for (int i = 0; i < contextNodes.Length; i++)
		{
		  int contextNode = contextNodes.item(i);
		  xctxt.pushCurrentNode(contextNode);

		  double result = 0;
		  try
		  {
			XPath dynamicXPath = new XPath(expr, xctxt.SAXLocator, xctxt.NamespaceContext, XPath.SELECT);
			result = dynamicXPath.execute(xctxt, contextNode, xctxt.NamespaceContext).num();
		  }
		  catch (TransformerException)
		  {
			xctxt.popCurrentNode();
			xctxt.popContextNodeList();
			return Double.NaN;
		  }

		  xctxt.popCurrentNode();

		  if (result > maxValue)
		  {
			  maxValue = result;
		  }
		}

		xctxt.popContextNodeList();
		return maxValue;

	  }

	  /// <summary>
	  /// The dyn:min function calculates the minimum value for the nodes passed as the 
	  /// first argument, where the value of each node is calculated dynamically using 
	  /// an XPath expression passed as a string as the second argument. 
	  /// <para>
	  /// The expressions are evaluated relative to the nodes passed as the first argument. 
	  /// In other words, the value for each node is calculated by evaluating the XPath 
	  /// expression with all context information being the same as that for the call to 
	  /// the dyn:min function itself, except for the following: 
	  /// </para>
	  /// <para>
	  /// <ul>
	  ///  <li>the context node is the node whose value is being calculated.</li>
	  ///  <li>the context position is the position of the node within the node set passed 
	  ///    as the first argument to the dyn:min function, arranged in document order.</li>
	  ///  <li>the context size is the number of nodes passed as the first argument to the 
	  ///    dyn:min function.</li>
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// The dyn:min function returns the minimum of these values, calculated in exactly 
	  /// the same way as for math:min. 
	  /// </para>
	  /// <para>
	  /// If the expression string passed as the second argument is an invalid XPath expression 
	  /// (including an empty string), this function returns NaN. 
	  /// </para>
	  /// <para>
	  /// This function must take a second argument. To calculate the minimum of a set of 
	  /// nodes based on their string values, you should use the math:min function.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="myContext"> The ExpressionContext passed by the extension processor </param>
	  /// <param name="nl"> The node set </param>
	  /// <param name="expr"> The expression string
	  /// </param>
	  /// <returns> The minimum evaluation value </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double min(org.apache.xalan.extensions.ExpressionContext myContext, org.w3c.dom.NodeList nl, String expr) throws org.xml.sax.SAXNotSupportedException
	  public static double min(ExpressionContext myContext, NodeList nl, string expr)
	  {

		XPathContext xctxt = null;
		if (myContext is XPathContext.XPathExpressionContext)
		{
		  xctxt = ((XPathContext.XPathExpressionContext) myContext).XPathContext;
		}
		else
		{
		  throw new SAXNotSupportedException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_CONTEXT_PASSED, new object[]{myContext}));
		}

		if (string.ReferenceEquals(expr, null) || expr.Length == 0)
		{
		  return Double.NaN;
		}

		NodeSetDTM contextNodes = new NodeSetDTM(nl, xctxt);
		xctxt.pushContextNodeList(contextNodes);

		double minValue = double.MaxValue;
		for (int i = 0; i < nl.Length; i++)
		{
		  int contextNode = contextNodes.item(i);
		  xctxt.pushCurrentNode(contextNode);

		  double result = 0;
		  try
		  {
			XPath dynamicXPath = new XPath(expr, xctxt.SAXLocator, xctxt.NamespaceContext, XPath.SELECT);
			result = dynamicXPath.execute(xctxt, contextNode, xctxt.NamespaceContext).num();
		  }
		  catch (TransformerException)
		  {
			xctxt.popCurrentNode();
			xctxt.popContextNodeList();
			return Double.NaN;
		  }

		  xctxt.popCurrentNode();

		  if (result < minValue)
		  {
			  minValue = result;
		  }
		}

		xctxt.popContextNodeList();
		return minValue;

	  }

	  /// <summary>
	  /// The dyn:sum function calculates the sum for the nodes passed as the first argument, 
	  /// where the value of each node is calculated dynamically using an XPath expression 
	  /// passed as a string as the second argument. 
	  /// <para>
	  /// The expressions are evaluated relative to the nodes passed as the first argument. 
	  /// In other words, the value for each node is calculated by evaluating the XPath 
	  /// expression with all context information being the same as that for the call to 
	  /// the dyn:sum function itself, except for the following: 
	  /// </para>
	  /// <para>
	  /// <ul>
	  ///  <li>the context node is the node whose value is being calculated.</li>
	  ///  <li>the context position is the position of the node within the node set passed as 
	  ///    the first argument to the dyn:sum function, arranged in document order.</li>
	  ///  <li>the context size is the number of nodes passed as the first argument to the 
	  ///    dyn:sum function.</li>
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// The dyn:sum function returns the sumimum of these values, calculated in exactly 
	  /// the same way as for sum. 
	  /// </para>
	  /// <para>
	  /// If the expression string passed as the second argument is an invalid XPath 
	  /// expression (including an empty string), this function returns NaN. 
	  /// </para>
	  /// <para>
	  /// This function must take a second argument. To calculate the sumimum of a set of 
	  /// nodes based on their string values, you should use the sum function.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="myContext"> The ExpressionContext passed by the extension processor </param>
	  /// <param name="nl"> The node set </param>
	  /// <param name="expr"> The expression string
	  /// </param>
	  /// <returns> The sum of the evaluation value on each node </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double sum(org.apache.xalan.extensions.ExpressionContext myContext, org.w3c.dom.NodeList nl, String expr) throws org.xml.sax.SAXNotSupportedException
	  public static double sum(ExpressionContext myContext, NodeList nl, string expr)
	  {
		XPathContext xctxt = null;
		if (myContext is XPathContext.XPathExpressionContext)
		{
		  xctxt = ((XPathContext.XPathExpressionContext) myContext).XPathContext;
		}
		else
		{
		  throw new SAXNotSupportedException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_CONTEXT_PASSED, new object[]{myContext}));
		}

		if (string.ReferenceEquals(expr, null) || expr.Length == 0)
		{
		  return Double.NaN;
		}

		NodeSetDTM contextNodes = new NodeSetDTM(nl, xctxt);
		xctxt.pushContextNodeList(contextNodes);

		double sum = 0;
		for (int i = 0; i < nl.Length; i++)
		{
		  int contextNode = contextNodes.item(i);
		  xctxt.pushCurrentNode(contextNode);

		  double result = 0;
		  try
		  {
			XPath dynamicXPath = new XPath(expr, xctxt.SAXLocator, xctxt.NamespaceContext, XPath.SELECT);
			result = dynamicXPath.execute(xctxt, contextNode, xctxt.NamespaceContext).num();
		  }
		  catch (TransformerException)
		  {
			xctxt.popCurrentNode();
			xctxt.popContextNodeList();
			return Double.NaN;
		  }

		  xctxt.popCurrentNode();

		  sum = sum + result;

		}

		xctxt.popContextNodeList();
		return sum;
	  }

	  /// <summary>
	  /// The dyn:map function evaluates the expression passed as the second argument for 
	  /// each of the nodes passed as the first argument, and returns a node set of those values. 
	  /// <para>
	  /// The expressions are evaluated relative to the nodes passed as the first argument. 
	  /// In other words, the value for each node is calculated by evaluating the XPath 
	  /// expression with all context information being the same as that for the call to 
	  /// the dyn:map function itself, except for the following:
	  /// </para>
	  /// <para>
	  /// <ul>
	  ///  <li>The context node is the node whose value is being calculated.</li>
	  ///  <li>the context position is the position of the node within the node set passed
	  ///    as the first argument to the dyn:map function, arranged in document order.</li>
	  ///  <li>the context size is the number of nodes passed as the first argument to the 
	  ///    dyn:map function.</li>
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// If the expression string passed as the second argument is an invalid XPath 
	  /// expression (including an empty string), this function returns an empty node set. 
	  /// </para>
	  /// <para>
	  /// If the XPath expression evaluates as a node set, the dyn:map function returns 
	  /// the union of the node sets returned by evaluating the expression for each of the 
	  /// nodes in the first argument. Note that this may mean that the node set resulting 
	  /// from the call to the dyn:map function contains a different number of nodes from 
	  /// the number in the node set passed as the first argument to the function. 
	  /// </para>
	  /// <para>
	  /// If the XPath expression evaluates as a number, the dyn:map function returns a 
	  /// node set containing one exsl:number element (namespace http://exslt.org/common) 
	  /// for each node in the node set passed as the first argument to the dyn:map function, 
	  /// in document order. The string value of each exsl:number element is the same as 
	  /// the result of converting the number resulting from evaluating the expression to 
	  /// a string as with the number function, with the exception that Infinity results 
	  /// in an exsl:number holding the highest number the implementation can store, and 
	  /// -Infinity results in an exsl:number holding the lowest number the implementation 
	  /// can store. 
	  /// </para>
	  /// <para>
	  /// If the XPath expression evaluates as a boolean, the dyn:map function returns a 
	  /// node set containing one exsl:boolean element (namespace http://exslt.org/common) 
	  /// for each node in the node set passed as the first argument to the dyn:map function, 
	  /// in document order. The string value of each exsl:boolean element is 'true' if the 
	  /// expression evaluates as true for the node, and '' if the expression evaluates as 
	  /// false. 
	  /// </para>
	  /// <para>
	  /// Otherwise, the dyn:map function returns a node set containing one exsl:string 
	  /// element (namespace http://exslt.org/common) for each node in the node set passed 
	  /// as the first argument to the dyn:map function, in document order. The string 
	  /// value of each exsl:string element is the same as the result of converting the 
	  /// result of evaluating the expression for the relevant node to a string as with 
	  /// the string function.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="myContext"> The ExpressionContext passed by the extension processor </param>
	  /// <param name="nl"> The node set </param>
	  /// <param name="expr"> The expression string
	  /// </param>
	  /// <returns> The node set after evaluation </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.w3c.dom.NodeList map(org.apache.xalan.extensions.ExpressionContext myContext, org.w3c.dom.NodeList nl, String expr) throws org.xml.sax.SAXNotSupportedException
	  public static NodeList map(ExpressionContext myContext, NodeList nl, string expr)
	  {
		XPathContext xctxt = null;
		Document lDoc = null;

		if (myContext is XPathContext.XPathExpressionContext)
		{
		  xctxt = ((XPathContext.XPathExpressionContext) myContext).XPathContext;
		}
		else
		{
		  throw new SAXNotSupportedException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_CONTEXT_PASSED, new object[]{myContext}));
		}

		if (string.ReferenceEquals(expr, null) || expr.Length == 0)
		{
		  return new NodeSet();
		}

		NodeSetDTM contextNodes = new NodeSetDTM(nl, xctxt);
		xctxt.pushContextNodeList(contextNodes);

		NodeSet resultSet = new NodeSet();
		resultSet.ShouldCacheNodes = true;

		for (int i = 0; i < nl.Length; i++)
		{
		  int contextNode = contextNodes.item(i);
		  xctxt.pushCurrentNode(contextNode);

		  XObject @object = null;
		  try
		  {
			XPath dynamicXPath = new XPath(expr, xctxt.SAXLocator, xctxt.NamespaceContext, XPath.SELECT);
			@object = dynamicXPath.execute(xctxt, contextNode, xctxt.NamespaceContext);

			if (@object is XNodeSet)
			{
			  NodeList nodelist = null;
			  nodelist = ((XNodeSet)@object).nodelist();

			  for (int k = 0; k < nodelist.Length; k++)
			  {
				Node n = nodelist.item(k);
				if (!resultSet.contains(n))
				{
				  resultSet.addNode(n);
				}
			  }
			}
			else
			{
		  if (lDoc == null)
		  {
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				dbf.NamespaceAware = true;
				DocumentBuilder db = dbf.newDocumentBuilder();
				lDoc = db.newDocument();
		  }

			  Element element = null;
			  if (@object is XNumber)
			  {
				element = lDoc.createElementNS(EXSL_URI, "exsl:number");
			  }
			  else if (@object is XBoolean)
			  {
				element = lDoc.createElementNS(EXSL_URI, "exsl:boolean");
			  }
			  else
			  {
				element = lDoc.createElementNS(EXSL_URI, "exsl:string");
			  }

			  Text textNode = lDoc.createTextNode(@object.str());
			  element.appendChild(textNode);
			  resultSet.addNode(element);
			}
		  }
		  catch (Exception)
		  {
			xctxt.popCurrentNode();
			xctxt.popContextNodeList();
			return new NodeSet();
		  }

		  xctxt.popCurrentNode();

		}

		xctxt.popContextNodeList();
		return resultSet;
	  }

	  /// <summary>
	  /// The dyn:evaluate function evaluates a string as an XPath expression and returns 
	  /// the resulting value, which might be a boolean, number, string, node set, result 
	  /// tree fragment or external object. The sole argument is the string to be evaluated.
	  /// <para>
	  /// If the expression string passed as the second argument is an invalid XPath 
	  /// expression (including an empty string), this function returns an empty node set. 
	  /// </para>
	  /// <para>
	  /// You should only use this function if the expression must be constructed dynamically, 
	  /// otherwise it is much more efficient to use the expression literally.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="myContext"> The ExpressionContext passed by the extension processor </param>
	  /// <param name="xpathExpr"> The XPath expression string
	  /// </param>
	  /// <returns> The evaluation result  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.xpath.objects.XObject evaluate(org.apache.xalan.extensions.ExpressionContext myContext, String xpathExpr) throws org.xml.sax.SAXNotSupportedException
	  public static XObject evaluate(ExpressionContext myContext, string xpathExpr)
	  {
		if (myContext is XPathContext.XPathExpressionContext)
		{
		  XPathContext xctxt = null;
		  try
		  {
			xctxt = ((XPathContext.XPathExpressionContext) myContext).XPathContext;
			XPath dynamicXPath = new XPath(xpathExpr, xctxt.SAXLocator, xctxt.NamespaceContext, XPath.SELECT);

			return dynamicXPath.execute(xctxt, myContext.ContextNode, xctxt.NamespaceContext);
		  }
		  catch (TransformerException)
		  {
			return new XNodeSet(xctxt.DTMManager);
		  }
		}
		else
		{
		  throw new SAXNotSupportedException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_CONTEXT_PASSED, new object[]{myContext})); //"Invalid context passed to evaluate "
		}
	  }

	  /// <summary>
	  /// The dyn:closure function creates a node set resulting from transitive closure of 
	  /// evaluating the expression passed as the second argument on each of the nodes passed 
	  /// as the first argument, then on the node set resulting from that and so on until no 
	  /// more nodes are found. For example: 
	  /// <pre>
	  ///  dyn:closure(., '*')
	  /// </pre>
	  /// returns all the descendant elements of the node (its element children, their 
	  /// children, their children's children and so on). 
	  /// <para>
	  /// The expression is thus evaluated several times, each with a different node set 
	  /// acting as the context of the expression. The first time the expression is 
	  /// evaluated, the context node set is the first argument passed to the dyn:closure 
	  /// function. In other words, the node set for each node is calculated by evaluating 
	  /// the XPath expression with all context information being the same as that for 
	  /// the call to the dyn:closure function itself, except for the following:
	  /// </para>
	  /// <para>
	  /// <ul>
	  ///  <li>the context node is the node whose value is being calculated.</li>
	  ///  <li>the context position is the position of the node within the node set passed 
	  ///    as the first argument to the dyn:closure function, arranged in document order.</li>
	  ///  <li>the context size is the number of nodes passed as the first argument to the 
	  ///    dyn:closure function.</li>
	  ///  <li>the current node is the node whose value is being calculated.</li>
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// The result for a particular iteration is the union of the node sets resulting 
	  /// from evaluting the expression for each of the nodes in the source node set for 
	  /// that iteration. This result is then used as the source node set for the next 
	  /// iteration, and so on. The result of the function as a whole is the union of 
	  /// the node sets generated by each iteration. 
	  /// </para>
	  /// <para>
	  /// If the expression string passed as the second argument is an invalid XPath 
	  /// expression (including an empty string) or an expression that does not return a 
	  /// node set, this function returns an empty node set.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="myContext"> The ExpressionContext passed by the extension processor </param>
	  /// <param name="nl"> The node set </param>
	  /// <param name="expr"> The expression string
	  /// </param>
	  /// <returns> The node set after evaluation </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.w3c.dom.NodeList closure(org.apache.xalan.extensions.ExpressionContext myContext, org.w3c.dom.NodeList nl, String expr) throws org.xml.sax.SAXNotSupportedException
	  public static NodeList closure(ExpressionContext myContext, NodeList nl, string expr)
	  {
		XPathContext xctxt = null;
		if (myContext is XPathContext.XPathExpressionContext)
		{
		  xctxt = ((XPathContext.XPathExpressionContext) myContext).XPathContext;
		}
		else
		{
		  throw new SAXNotSupportedException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_CONTEXT_PASSED, new object[]{myContext}));
		}

		if (string.ReferenceEquals(expr, null) || expr.Length == 0)
		{
		  return new NodeSet();
		}

		NodeSet closureSet = new NodeSet();
		closureSet.ShouldCacheNodes = true;

		NodeList iterationList = nl;
		do
		{

		  NodeSet iterationSet = new NodeSet();

		  NodeSetDTM contextNodes = new NodeSetDTM(iterationList, xctxt);
		  xctxt.pushContextNodeList(contextNodes);

		  for (int i = 0; i < iterationList.Length; i++)
		  {
			int contextNode = contextNodes.item(i);
			xctxt.pushCurrentNode(contextNode);

			XObject @object = null;
			try
			{
			  XPath dynamicXPath = new XPath(expr, xctxt.SAXLocator, xctxt.NamespaceContext, XPath.SELECT);
			  @object = dynamicXPath.execute(xctxt, contextNode, xctxt.NamespaceContext);

			  if (@object is XNodeSet)
			  {
				NodeList nodelist = null;
				nodelist = ((XNodeSet)@object).nodelist();

				for (int k = 0; k < nodelist.Length; k++)
				{
				  Node n = nodelist.item(k);
				  if (!iterationSet.contains(n))
				  {
					iterationSet.addNode(n);
				  }
				}
			  }
			  else
			  {
				xctxt.popCurrentNode();
				xctxt.popContextNodeList();
				return new NodeSet();
			  }
			}
			catch (TransformerException)
			{
			  xctxt.popCurrentNode();
			  xctxt.popContextNodeList();
			  return new NodeSet();
			}

			xctxt.popCurrentNode();

		  }

		  xctxt.popContextNodeList();

		  iterationList = iterationSet;

		  for (int i = 0; i < iterationList.Length; i++)
		  {
			Node n = iterationList.item(i);
			if (!closureSet.contains(n))
			{
			  closureSet.addNode(n);
			}
		  }

		} while (iterationList.Length > 0);

		return closureSet;

	  }

	}

}