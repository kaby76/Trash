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
// $Id: XPathImpl.java 1225280 2011-12-28 18:52:55Z mrglavas $

namespace org.apache.xpath.jaxp
{


	using DTM = org.apache.xml.dtm.DTM;
	using org.apache.xpath;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XSLMessages = org.apache.xalan.res.XSLMessages;

	using Node = org.w3c.dom.Node;
	using DOMImplementation = org.w3c.dom.DOMImplementation;
	using Document = org.w3c.dom.Document;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;


	/// <summary>
	/// The XPathImpl class provides implementation for the methods defined  in
	/// javax.xml.xpath.XPath interface. This provide simple access to the results
	/// of an XPath expression.
	/// 
	/// 
	/// @version $Revision: 1225280 $
	/// @author  Ramesh Mandava
	/// </summary>
	public class XPathImpl : javax.xml.xpath.XPath
	{

		// Private variables
		private XPathVariableResolver variableResolver;
		private XPathFunctionResolver functionResolver;
		private XPathVariableResolver origVariableResolver;
		private XPathFunctionResolver origFunctionResolver;
		private NamespaceContext namespaceContext = null;
		private JAXPPrefixResolver prefixResolver;
		// By default Extension Functions are allowed in XPath Expressions. If 
		// Secure Processing Feature is set on XPathFactory then the invocation of
		// extensions function need to throw XPathFunctionException
		private bool featureSecureProcessing = false;

		internal XPathImpl(XPathVariableResolver vr, XPathFunctionResolver fr)
		{
			this.origVariableResolver = this.variableResolver = vr;
			this.origFunctionResolver = this.functionResolver = fr;
		}

		internal XPathImpl(XPathVariableResolver vr, XPathFunctionResolver fr, bool featureSecureProcessing)
		{
			this.origVariableResolver = this.variableResolver = vr;
			this.origFunctionResolver = this.functionResolver = fr;
			this.featureSecureProcessing = featureSecureProcessing;
		}

		/// <summary>
		/// <para>Establishes a variable resolver.</para>
		/// </summary>
		/// <param name="resolver"> Variable Resolver </param>
		public virtual XPathVariableResolver XPathVariableResolver
		{
			set
			{
				if (value == null)
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"XPathVariableResolver"});
					throw new System.NullReferenceException(fmsg);
				}
				this.variableResolver = value;
			}
			get
			{
				return variableResolver;
			}
		}


		/// <summary>
		/// <para>Establishes a function resolver.</para>
		/// </summary>
		/// <param name="resolver"> XPath function resolver </param>
		public virtual XPathFunctionResolver XPathFunctionResolver
		{
			set
			{
				if (value == null)
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"XPathFunctionResolver"});
					throw new System.NullReferenceException(fmsg);
				}
				this.functionResolver = value;
			}
			get
			{
				return functionResolver;
			}
		}


		/// <summary>
		/// <para>Establishes a namespace context.</para>
		/// </summary>
		/// <param name="nsContext"> Namespace context to use </param>
		public virtual NamespaceContext NamespaceContext
		{
			set
			{
				if (value == null)
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"NamespaceContext"});
					throw new System.NullReferenceException(fmsg);
				}
				this.namespaceContext = value;
				this.prefixResolver = new JAXPPrefixResolver(value);
			}
			get
			{
				return namespaceContext;
			}
		}


		private static Document d = null;

		private static DocumentBuilder Parser
		{
			get
			{
				try
				{
					// we'd really like to cache those DocumentBuilders, but we can't because:
					// 1. thread safety. parsers are not thread-safe, so at least
					//    we need one instance per a thread.
					// 2. parsers are non-reentrant, so now we are looking at having a
					// pool of parsers.
					// 3. then the class loading issue. The look-up procedure of
					//    DocumentBuilderFactory.newInstance() depends on context class loader
					//    and system properties, which may change during the execution of JVM.
					//
					// so we really have to create a fresh DocumentBuilder every time we need one
					// - KK
					DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
					dbf.NamespaceAware = true;
					dbf.Validating = false;
					return dbf.newDocumentBuilder();
				}
				catch (ParserConfigurationException e)
				{
					// this should never happen with a well-behaving JAXP implementation. 
					throw new Exception(e.ToString());
				}
			}
		}

		private static Document DummyDocument
		{
			get
			{
				// we don't need synchronization here; even if two threads
				// enter this code at the same time, we just waste a little time
				if (d == null)
				{
					DOMImplementation dim = Parser.DOMImplementation;
					d = dim.createDocument("http://java.sun.com/jaxp/xpath", "dummyroot", null);
				}
				return d;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.apache.xpath.objects.XObject eval(String expression, Object contextItem) throws javax.xml.transform.TransformerException
		private XObject eval(string expression, object contextItem)
		{
			org.apache.xpath.XPath xpath = new org.apache.xpath.XPath(expression, null, prefixResolver, org.apache.xpath.XPath.SELECT);
			org.apache.xpath.XPathContext xpathSupport = null;

			// Create an XPathContext that doesn't support pushing and popping of
			// variable resolution scopes.  Sufficient for simple XPath 1.0
			// expressions.
			if (functionResolver != null)
			{
				JAXPExtensionsProvider jep = new JAXPExtensionsProvider(functionResolver, featureSecureProcessing);
				xpathSupport = new org.apache.xpath.XPathContext(jep, false);
			}
			else
			{
				xpathSupport = new org.apache.xpath.XPathContext(false);
			}

			XObject xobj = null;

			xpathSupport.VarStack = new JAXPVariableStack(variableResolver);

			// If item is null, then we will create a a Dummy contextNode
			if (contextItem is Node)
			{
				xobj = xpath.execute(xpathSupport, (Node)contextItem, prefixResolver);
			}
			else
			{
				xobj = xpath.execute(xpathSupport, org.apache.xml.dtm.DTM_Fields.NULL, prefixResolver);
			}

			return xobj;
		}

		/// <summary>
		/// <para>Evaluate an <code>XPath</code> expression in the specified context and return the result as the specified type.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec
		/// for context item evaluation,
		/// variable, function and <code>QName</code> resolution and return type conversion.</para>
		/// 
		/// <para>If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/> (
		/// <seealso cref="XPathConstants#NUMBER NUMBER"/>,
		/// <seealso cref="XPathConstants#STRING STRING"/>,
		/// <seealso cref="XPathConstants#BOOLEAN BOOLEAN"/>,
		/// <seealso cref="XPathConstants#NODE NODE"/> or
		/// <seealso cref="XPathConstants#NODESET NODESET"/>)
		/// then an <code>IllegalArgumentException</code> is thrown.</para>
		/// 
		/// <para>If a <code>null</code> value is provided for
		/// <code>item</code>, an empty document will be used for the
		/// context.
		/// If <code>expression</code> or <code>returnType</code> is <code>null</code>, then a
		/// <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="expression"> The XPath expression. </param>
		/// <param name="item"> The starting context (node or node list, for example). </param>
		/// <param name="returnType"> The desired return type.
		/// </param>
		/// <returns> Result of evaluating an XPath expression as an <code>Object</code> of <code>returnType</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If <code>expression</code> cannot be evaluated. </exception>
		/// <exception cref="IllegalArgumentException"> If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/>. </exception>
		/// <exception cref="NullPointerException"> If <code>expression</code> or <code>returnType</code> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object evaluate(String expression, Object item, javax.xml.namespace.QName returnType) throws javax.xml.xpath.XPathExpressionException
		public virtual object evaluate(string expression, object item, QName returnType)
		{
			if (string.ReferenceEquals(expression, null))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"XPath expression"});
				throw new System.NullReferenceException(fmsg);
			}
			if (returnType == null)
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"returnType"});
				throw new System.NullReferenceException(fmsg);
			}
			// Checking if requested returnType is supported. returnType need to
			// be defined in XPathConstants
			if (!isSupported(returnType))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNSUPPORTED_RETURN_TYPE, new object[] {returnType.ToString()});
				throw new System.ArgumentException(fmsg);
			}

			try
			{

				XObject resultObject = eval(expression, item);
				return getResultAsType(resultObject, returnType);
			}
			catch (System.NullReferenceException npe)
			{
				// If VariableResolver returns null Or if we get 
				// NullPointerException at this stage for some other reason
				// then we have to reurn XPathException 
				throw new XPathExpressionException(npe);
			}
			catch (javax.xml.transform.TransformerException te)
			{
				Exception nestedException = te.Exception;
				if (nestedException is javax.xml.xpath.XPathFunctionException)
				{
					throw (javax.xml.xpath.XPathFunctionException)nestedException;
				}
				else
				{
					// For any other exceptions we need to throw 
					// XPathExpressionException ( as per spec )
					throw new XPathExpressionException(te);
				}
			}

		}

		private bool isSupported(QName returnType)
		{
			if ((returnType.Equals(XPathConstants.STRING)) || (returnType.Equals(XPathConstants.NUMBER)) || (returnType.Equals(XPathConstants.BOOLEAN)) || (returnType.Equals(XPathConstants.NODE)) || (returnType.Equals(XPathConstants.NODESET)))
			{

				return true;
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object getResultAsType(org.apache.xpath.objects.XObject resultObject, javax.xml.namespace.QName returnType) throws javax.xml.transform.TransformerException
		private object getResultAsType(XObject resultObject, QName returnType)
		{
			// XPathConstants.STRING
			if (returnType.Equals(XPathConstants.STRING))
			{
				return resultObject.str();
			}
			// XPathConstants.NUMBER
			if (returnType.Equals(XPathConstants.NUMBER))
			{
				return new double?(resultObject.num());
			}
			// XPathConstants.BOOLEAN
			if (returnType.Equals(XPathConstants.BOOLEAN))
			{
				return resultObject.@bool() ? true : false;
			}
			// XPathConstants.NODESET ---ORdered, UNOrdered???
			if (returnType.Equals(XPathConstants.NODESET))
			{
				return resultObject.nodelist();
			}
			// XPathConstants.NODE
			if (returnType.Equals(XPathConstants.NODE))
			{
				NodeIterator ni = resultObject.nodeset();
				//Return the first node, or null
				return ni.nextNode();
			}
			string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNSUPPORTED_RETURN_TYPE, new object[] {returnType.ToString()});
			throw new System.ArgumentException(fmsg);
		}



		/// <summary>
		/// <para>Evaluate an XPath expression in the specified context and return the result as a <code>String</code>.</para>
		/// 
		/// <para>This method calls <seealso cref="#evaluate(String expression, Object item, QName returnType)"/> with a <code>returnType</code> of
		/// <seealso cref="XPathConstants#STRING"/>.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" of JAXP 1.3 spec 
		/// for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If a <code>null</code> value is provided for
		/// <code>item</code>, an empty document will be used for the
		/// context.
		/// If <code>expression</code> is <code>null</code>, then a <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="expression"> The XPath expression. </param>
		/// <param name="item"> The starting context (node or node list, for example).
		/// </param>
		/// <returns> The <code>String</code> that is the result of evaluating the expression and
		///   converting the result to a <code>String</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If <code>expression</code> cannot be evaluated. </exception>
		/// <exception cref="NullPointerException"> If <code>expression</code> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String evaluate(String expression, Object item) throws javax.xml.xpath.XPathExpressionException
		public virtual string evaluate(string expression, object item)
		{
			return (string)this.evaluate(expression, item, XPathConstants.STRING);
		}

		/// <summary>
		/// <para>Compile an XPath expression for later evaluation.</para>
		/// 
		/// <para>If <code>expression</code> contains any <seealso cref="XPathFunction"/>s,
		/// they must be available via the <seealso cref="XPathFunctionResolver"/>.
		/// An <seealso cref="XPathExpressionException"/> will be thrown if the <code>XPathFunction</code>
		/// cannot be resovled with the <code>XPathFunctionResolver</code>.</para>
		/// 
		/// <para>If <code>expression</code> is <code>null</code>, a <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="expression"> The XPath expression.
		/// </param>
		/// <returns> Compiled XPath expression.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If <code>expression</code> cannot be compiled. </exception>
		/// <exception cref="NullPointerException"> If <code>expression</code> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.xpath.XPathExpression compile(String expression) throws javax.xml.xpath.XPathExpressionException
		public virtual XPathExpression compile(string expression)
		{
			if (string.ReferenceEquals(expression, null))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"XPath expression"});
				throw new System.NullReferenceException(fmsg);
			}
			try
			{
				org.apache.xpath.XPath xpath = new XPath(expression, null, prefixResolver, org.apache.xpath.XPath.SELECT);
				// Can have errorListener
				XPathExpressionImpl ximpl = new XPathExpressionImpl(xpath, prefixResolver, functionResolver, variableResolver, featureSecureProcessing);
				return ximpl;
			}
			catch (javax.xml.transform.TransformerException te)
			{
				throw new XPathExpressionException(te);
			}
		}


		/// <summary>
		/// <para>Evaluate an XPath expression in the context of the specified <code>InputSource</code>
		/// and return the result as the specified type.</para>
		/// 
		/// <para>This method builds a data model for the <seealso cref="InputSource"/> and calls
		/// <seealso cref="#evaluate(String expression, Object item, QName returnType)"/> on the resulting document object.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec 
		/// for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/>,
		/// then an <code>IllegalArgumentException</code> is thrown.</para>
		/// 
		/// <para>If <code>expression</code>, <code>source</code> or <code>returnType</code> is <code>null</code>,
		/// then a <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="expression"> The XPath expression. </param>
		/// <param name="source"> The input source of the document to evaluate over. </param>
		/// <param name="returnType"> The desired return type.
		/// </param>
		/// <returns> The <code>Object</code> that encapsulates the result of evaluating the expression.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If expression cannot be evaluated. </exception>
		/// <exception cref="IllegalArgumentException"> If <code>returnType</code> is not one of the types defined in <seealso cref="XPathConstants"/>. </exception>
		/// <exception cref="NullPointerException"> If <code>expression</code>, <code>source</code> or <code>returnType</code>
		///   is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object evaluate(String expression, org.xml.sax.InputSource source, javax.xml.namespace.QName returnType) throws javax.xml.xpath.XPathExpressionException
		public virtual object evaluate(string expression, InputSource source, QName returnType)
		{
			// Checking validity of different parameters
			if (source == null)
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"source"});
				throw new System.NullReferenceException(fmsg);
			}
			if (string.ReferenceEquals(expression, null))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"XPath expression"});
				throw new System.NullReferenceException(fmsg);
			}
			if (returnType == null)
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"returnType"});
				throw new System.NullReferenceException(fmsg);
			}

			//Checking if requested returnType is supported. 
			//returnType need to be defined in XPathConstants
			if (!isSupported(returnType))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNSUPPORTED_RETURN_TYPE, new object[] {returnType.ToString()});
				throw new System.ArgumentException(fmsg);
			}

			try
			{

				Document document = Parser.parse(source);

				XObject resultObject = eval(expression, document);
				return getResultAsType(resultObject, returnType);
			}
			catch (SAXException e)
			{
				throw new XPathExpressionException(e);
			}
			catch (IOException e)
			{
				throw new XPathExpressionException(e);
			}
			catch (javax.xml.transform.TransformerException te)
			{
				Exception nestedException = te.Exception;
				if (nestedException is javax.xml.xpath.XPathFunctionException)
				{
					throw (javax.xml.xpath.XPathFunctionException)nestedException;
				}
				else
				{
					throw new XPathExpressionException(te);
				}
			}

		}




		/// <summary>
		/// <para>Evaluate an XPath expression in the context of the specified <code>InputSource</code>
		/// and return the result as a <code>String</code>.</para>
		/// 
		/// <para>This method calls <seealso cref="#evaluate(String expression, InputSource source, QName returnType)"/> with a
		/// <code>returnType</code> of <seealso cref="XPathConstants#STRING"/>.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec
		/// for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If <code>expression</code> or <code>source</code> is <code>null</code>,
		/// then a <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="expression"> The XPath expression. </param>
		/// <param name="source"> The <code>InputSource</code> of the document to evaluate over.
		/// </param>
		/// <returns> The <code>String</code> that is the result of evaluating the expression and
		///   converting the result to a <code>String</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If expression cannot be evaluated. </exception>
		/// <exception cref="NullPointerException"> If <code>expression</code> or <code>source</code> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String evaluate(String expression, org.xml.sax.InputSource source) throws javax.xml.xpath.XPathExpressionException
		public virtual string evaluate(string expression, InputSource source)
		{
			return (string)this.evaluate(expression, source, XPathConstants.STRING);
		}

		/// <summary>
		/// <para>Reset this <code>XPath</code> to its original configuration.</para>
		/// 
		/// <para><code>XPath</code> is reset to the same state as when it was created with
		/// <seealso cref="XPathFactory#newXPath()"/>.
		/// <code>reset()</code> is designed to allow the reuse of existing <code>XPath</code>s
		/// thus saving resources associated with the creation of new <code>XPath</code>s.</para>
		/// 
		/// <para>The reset <code>XPath</code> is not guaranteed to have the same
		/// <seealso cref="XPathFunctionResolver"/>, <seealso cref="XPathVariableResolver"/>
		/// or <seealso cref="NamespaceContext"/> <code>Object</code>s, e.g. <seealso cref="Object#equals(Object obj)"/>.
		/// It is guaranteed to have a functionally equal <code>XPathFunctionResolver</code>,
		/// <code>XPathVariableResolver</code>
		/// and <code>NamespaceContext</code>.</para>
		/// </summary>
		public virtual void reset()
		{
			this.variableResolver = this.origVariableResolver;
			this.functionResolver = this.origFunctionResolver;
			this.namespaceContext = null;
		}

	}

}