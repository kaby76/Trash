using System;
using System.Collections;

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
 * $Id: Extensions.java 468639 2006-10-28 06:52:33Z minchau $
 */
namespace org.apache.xalan.lib
{



	using ExpressionContext = org.apache.xalan.extensions.ExpressionContext;
	using EnvironmentCheck = org.apache.xalan.xslt.EnvironmentCheck;
	using NodeSet = org.apache.xpath.NodeSet;
	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	using Document = org.w3c.dom.Document;
	using DocumentFragment = org.w3c.dom.DocumentFragment;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using Text = org.w3c.dom.Text;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;

	/// <summary>
	/// This class contains many of the Xalan-supplied extensions.
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:xalan="http://xml.apache.org/xalan"
	/// </pre>
	/// @xsl.usage general
	/// </summary>
	public class Extensions
	{
	  /// <summary>
	  /// Constructor Extensions
	  /// 
	  /// </summary>
	  private Extensions()
	  {
	  } // Make sure class cannot be instantiated

	  /// <summary>
	  /// This method is an extension that implements as a Xalan extension
	  /// the node-set function also found in xt and saxon.
	  /// If the argument is a Result Tree Fragment, then <code>nodeset</code>
	  /// returns a node-set consisting of a single root node as described in
	  /// section 11.1 of the XSLT 1.0 Recommendation.  If the argument is a
	  /// node-set, <code>nodeset</code> returns a node-set.  If the argument
	  /// is a string, number, or boolean, then <code>nodeset</code> returns
	  /// a node-set consisting of a single root node with a single text node
	  /// child that is the result of calling the XPath string() function on the
	  /// passed parameter.  If the argument is anything else, then a node-set
	  /// is returned consisting of a single root node with a single text node
	  /// child that is the result of calling the java <code>toString()</code>
	  /// method on the passed argument.
	  /// Most of the
	  /// actual work here is done in <code>MethodResolver</code> and
	  /// <code>XRTreeFrag</code>. </summary>
	  /// <param name="myProcessor"> Context passed by the extension processor </param>
	  /// <param name="rtf"> Argument in the stylesheet to the nodeset extension function
	  /// 
	  /// NEEDSDOC ($objectName$) @return </param>
	  public static NodeSet nodeset(ExpressionContext myProcessor, object rtf)
	  {

		string textNodeValue;

		if (rtf is NodeIterator)
		{
		  return new NodeSet((NodeIterator) rtf);
		}
		else
		{
		  if (rtf is string)
		  {
			textNodeValue = (string) rtf;
		  }
		  else if (rtf is bool?)
		  {
			textNodeValue = (new XBoolean(((bool?) rtf).Value)).str();
		  }
		  else if (rtf is double?)
		  {
			textNodeValue = (new XNumber(((double?) rtf).Value)).str();
		  }
		  else
		  {
			textNodeValue = rtf.ToString();
		  }

		  // This no longer will work right since the DTM.
		  // Document myDoc = myProcessor.getContextNode().getOwnerDocument();
		  try
		  {
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder db = dbf.newDocumentBuilder();
			Document myDoc = db.newDocument();

			Text textNode = myDoc.createTextNode(textNodeValue);
			DocumentFragment docFrag = myDoc.createDocumentFragment();

			docFrag.appendChild(textNode);

			return new NodeSet(docFrag);
		  }
		  catch (ParserConfigurationException pce)
		  {
			throw new org.apache.xml.utils.WrappedRuntimeException(pce);
		  }
		}
	  }

	  /// <summary>
	  /// Returns the intersection of two node-sets.
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set </param>
	  /// <param name="nl2"> NodeList for second node-set </param>
	  /// <returns> a NodeList containing the nodes in nl1 that are also in nl2
	  /// 
	  /// Note: The usage of this extension function in the xalan namespace 
	  /// is deprecated. Please use the same function in the EXSLT sets extension
	  /// (http://exslt.org/sets). </returns>
	  public static NodeList intersection(NodeList nl1, NodeList nl2)
	  {
		return ExsltSets.intersection(nl1, nl2);
	  }

	  /// <summary>
	  /// Returns the difference between two node-sets.
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set </param>
	  /// <param name="nl2"> NodeList for second node-set </param>
	  /// <returns> a NodeList containing the nodes in nl1 that are not in nl2
	  /// 
	  /// Note: The usage of this extension function in the xalan namespace 
	  /// is deprecated. Please use the same function in the EXSLT sets extension
	  /// (http://exslt.org/sets). </returns>
	  public static NodeList difference(NodeList nl1, NodeList nl2)
	  {
		return ExsltSets.difference(nl1, nl2);
	  }

	  /// <summary>
	  /// Returns node-set containing distinct string values.
	  /// </summary>
	  /// <param name="nl"> NodeList for node-set </param>
	  /// <returns> a NodeList with nodes from nl containing distinct string values.
	  /// In other words, if more than one node in nl contains the same string value,
	  /// only include the first such node found.
	  /// 
	  /// Note: The usage of this extension function in the xalan namespace 
	  /// is deprecated. Please use the same function in the EXSLT sets extension
	  /// (http://exslt.org/sets). </returns>
	  public static NodeList distinct(NodeList nl)
	  {
		return ExsltSets.distinct(nl);
	  }

	  /// <summary>
	  /// Returns true if both node-sets contain the same set of nodes.
	  /// </summary>
	  /// <param name="nl1"> NodeList for first node-set </param>
	  /// <param name="nl2"> NodeList for second node-set </param>
	  /// <returns> true if nl1 and nl2 contain exactly the same set of nodes. </returns>
	  public static bool hasSameNodes(NodeList nl1, NodeList nl2)
	  {

		NodeSet ns1 = new NodeSet(nl1);
		NodeSet ns2 = new NodeSet(nl2);

		if (ns1.Length != ns2.Length)
		{
		  return false;
		}

		for (int i = 0; i < ns1.Length; i++)
		{
		  Node n = ns1.elementAt(i);

		  if (!ns2.contains(n))
		  {
			return false;
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// Returns the result of evaluating the argument as a string containing
	  /// an XPath expression.  Used where the XPath expression is not known until
	  /// run-time.  The expression is evaluated as if the run-time value of the
	  /// argument appeared in place of the evaluate function call at compile time.
	  /// </summary>
	  /// <param name="myContext"> an <code>ExpressionContext</code> passed in by the
	  ///                  extension mechanism.  This must be an XPathContext. </param>
	  /// <param name="xpathExpr"> The XPath expression to be evaluated. </param>
	  /// <returns> the XObject resulting from evaluating the XPath
	  /// </returns>
	  /// <exception cref="SAXNotSupportedException">
	  /// 
	  /// Note: The usage of this extension function in the xalan namespace 
	  /// is deprecated. Please use the same function in the EXSLT dynamic extension
	  /// (http://exslt.org/dynamic). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.xpath.objects.XObject evaluate(org.apache.xalan.extensions.ExpressionContext myContext, String xpathExpr) throws org.xml.sax.SAXNotSupportedException
	  public static XObject evaluate(ExpressionContext myContext, string xpathExpr)
	  {
		return ExsltDynamic.evaluate(myContext, xpathExpr);
	  }

	  /// <summary>
	  /// Returns a NodeSet containing one text node for each token in the first argument.
	  /// Delimiters are specified in the second argument.
	  /// Tokens are determined by a call to <code>StringTokenizer</code>.
	  /// If the first argument is an empty string or contains only delimiters, the result
	  /// will be an empty NodeSet.
	  /// 
	  /// Contributed to XalanJ1 by <a href="mailto:benoit.cerrina@writeme.com">Benoit Cerrina</a>.
	  /// </summary>
	  /// <param name="toTokenize"> The string to be split into text tokens. </param>
	  /// <param name="delims"> The delimiters to use. </param>
	  /// <returns> a NodeSet as described above. </returns>
	  public static NodeList tokenize(string toTokenize, string delims)
	  {

		Document doc = DocumentHolder.m_doc;


		StringTokenizer lTokenizer = new StringTokenizer(toTokenize, delims);
		NodeSet resultSet = new NodeSet();

		lock (doc)
		{
		  while (lTokenizer.hasMoreTokens())
		  {
			resultSet.addNode(doc.createTextNode(lTokenizer.nextToken()));
		  }
		}

		return resultSet;
	  }

	  /// <summary>
	  /// Returns a NodeSet containing one text node for each token in the first argument.
	  /// Delimiters are whitespace.  That is, the delimiters that are used are tab (&#x09),
	  /// linefeed (&#x0A), return (&#x0D), and space (&#x20).
	  /// Tokens are determined by a call to <code>StringTokenizer</code>.
	  /// If the first argument is an empty string or contains only delimiters, the result
	  /// will be an empty NodeSet.
	  /// 
	  /// Contributed to XalanJ1 by <a href="mailto:benoit.cerrina@writeme.com">Benoit Cerrina</a>.
	  /// </summary>
	  /// <param name="toTokenize"> The string to be split into text tokens. </param>
	  /// <returns> a NodeSet as described above. </returns>
	  public static NodeList tokenize(string toTokenize)
	  {
		return tokenize(toTokenize, " \t\n\r");
	  }

	  /// <summary>
	  /// Return a Node of basic debugging information from the 
	  /// EnvironmentCheck utility about the Java environment.
	  /// 
	  /// <para>Simply calls the <seealso cref="org.apache.xalan.xslt.EnvironmentCheck"/>
	  /// utility to grab info about the Java environment and CLASSPATH, 
	  /// etc., and then returns the resulting Node.  Stylesheets can 
	  /// then maniuplate this data or simply xsl:copy-of the Node.  Note 
	  /// that we first attempt to load the more advanced 
	  /// org.apache.env.Which utility by reflection; only if that fails 
	  /// to we still use the internal version.  Which is available from 
	  /// <a href="http://xml.apache.org/commons/">http://xml.apache.org/commons/</a>.</para>
	  /// 
	  /// <para>We throw a WrappedRuntimeException in the unlikely case 
	  /// that reading information from the environment throws us an 
	  /// exception. (Is this really the best thing to do?)</para>
	  /// </summary>
	  /// <param name="myContext"> an <code>ExpressionContext</code> passed in by the
	  ///                  extension mechanism.  This must be an XPathContext. </param>
	  /// <returns> a Node as described above. </returns>
	  public static Node checkEnvironment(ExpressionContext myContext)
	  {

		Document factoryDocument;
		try
		{
		  DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
		  DocumentBuilder db = dbf.newDocumentBuilder();
		  factoryDocument = db.newDocument();
		}
		catch (ParserConfigurationException pce)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(pce);
		}

		Node resultNode = null;
		try
		{
		  // First use reflection to try to load Which, which is a 
		  //  better version of EnvironmentCheck
		  resultNode = checkEnvironmentUsingWhich(myContext, factoryDocument);

		  if (null != resultNode)
		  {
			return resultNode;
		  }

		  // If reflection failed, fallback to our internal EnvironmentCheck
		  EnvironmentCheck envChecker = new EnvironmentCheck();
		  Hashtable h = envChecker.EnvironmentHash;
		  resultNode = factoryDocument.createElement("checkEnvironmentExtension");
		  envChecker.appendEnvironmentReport(resultNode, factoryDocument, h);
		  envChecker = null;
		}
		catch (Exception e)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(e);
		}

		return resultNode;
	  }

	  /// <summary>
	  /// Private worker method to attempt to use org.apache.env.Which.
	  /// </summary>
	  /// <param name="myContext"> an <code>ExpressionContext</code> passed in by the
	  ///                  extension mechanism.  This must be an XPathContext. </param>
	  /// <param name="factoryDocument"> providing createElement services, etc. </param>
	  /// <returns> a Node with environment info; null if any error </returns>
	  private static Node checkEnvironmentUsingWhich(ExpressionContext myContext, Document factoryDocument)
	  {
		const string WHICH_CLASSNAME = "org.apache.env.Which";
		const string WHICH_METHODNAME = "which";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class WHICH_METHOD_ARGS[] = { java.util.Hashtable.class, java.lang.String.class, java.lang.String.class };
		Type[] WHICH_METHOD_ARGS = new Type[] {typeof(Hashtable), typeof(string), typeof(string)};
		try
		{
		  // Use reflection to try to find xml-commons utility 'Which'
		  Type clazz = ObjectFactory.findProviderClass(WHICH_CLASSNAME, ObjectFactory.findClassLoader(), true);
		  if (null == clazz)
		  {
			return null;
		  }

		  // Fully qualify names since this is the only method they're used in
		  System.Reflection.MethodInfo method = clazz.GetMethod(WHICH_METHODNAME, WHICH_METHOD_ARGS);
		  Hashtable report = new Hashtable();

		  // Call the method with our Hashtable, common options, and ignore return value
		  object[] methodArgs = new object[] {report, "XmlCommons;Xalan;Xerces;Crimson;Ant", ""};
		  object returnValue = method.invoke(null, methodArgs);

		  // Create a parent to hold the report and append hash to it
		  Node resultNode = factoryDocument.createElement("checkEnvironmentExtension");
		  org.apache.xml.utils.Hashtree2Node.appendHashToNode(report, "whichReport", resultNode, factoryDocument);

		  return resultNode;
		}
		catch (Exception)
		{
		  // Simply return null; no need to report error
		  return null;
		}
	  }

		/// <summary>
		/// This class is not loaded until first referenced (see Java Language
		/// Specification by Gosling/Joy/Steele, section 12.4.1)
		/// 
		/// The static members are created when this class is first referenced, as a
		/// lazy initialization not needing checking against null or any
		/// synchronization.
		/// 
		/// </summary>
		private class DocumentHolder
		{
			// Reuse the Document object to reduce memory usage.
			internal static readonly Document m_doc;
			static DocumentHolder()
			{
				try
				{
					m_doc = DocumentBuilderFactory.newInstance().newDocumentBuilder().newDocument();
				}

				catch (ParserConfigurationException pce)
				{
					  throw new org.apache.xml.utils.WrappedRuntimeException(pce);
				}

			}
		}
	}

}