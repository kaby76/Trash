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
// $Id: XPathExpressionImpl.java 1225277 2011-12-28 18:50:56Z mrglavas $

namespace org.apache.xpath.jaxp
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using DOMImplementation = org.w3c.dom.DOMImplementation;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;
	using InputSource = org.xml.sax.InputSource;

	/// <summary>
	/// The XPathExpression interface encapsulates a (compiled) XPath expression.
	/// 
	/// @version $Revision: 1225277 $
	/// @author  Ramesh Mandava
	/// </summary>
	public class XPathExpressionImpl : javax.xml.xpath.XPathExpression
	{

		private XPathFunctionResolver functionResolver;
		private XPathVariableResolver variableResolver;
		private JAXPPrefixResolver prefixResolver;
		private org.apache.xpath.XPath xpath;

		// By default Extension Functions are allowed in XPath Expressions. If
		// Secure Processing Feature is set on XPathFactory then the invocation of
		// extensions function need to throw XPathFunctionException
		private bool featureSecureProcessing = false;

		/// <summary>
		/// Protected constructor to prevent direct instantiation; use compile()
		/// from the context.
		/// </summary>
		protected internal XPathExpressionImpl()
		{
		};

		protected internal XPathExpressionImpl(org.apache.xpath.XPath xpath, JAXPPrefixResolver prefixResolver, XPathFunctionResolver functionResolver, XPathVariableResolver variableResolver)
		{
			this.xpath = xpath;
			this.prefixResolver = prefixResolver;
			this.functionResolver = functionResolver;
			this.variableResolver = variableResolver;
			this.featureSecureProcessing = false;
		};

		protected internal XPathExpressionImpl(org.apache.xpath.XPath xpath, JAXPPrefixResolver prefixResolver, XPathFunctionResolver functionResolver, XPathVariableResolver variableResolver, bool featureSecureProcessing)
		{
			this.xpath = xpath;
			this.prefixResolver = prefixResolver;
			this.functionResolver = functionResolver;
			this.variableResolver = variableResolver;
			this.featureSecureProcessing = featureSecureProcessing;
		};

		public virtual org.apache.xpath.XPath XPath
		{
			set
			{
				this.xpath = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object eval(Object item, javax.xml.namespace.QName returnType) throws javax.xml.transform.TransformerException
		public virtual object eval(object item, QName returnType)
		{
			XObject resultObject = eval(item);
			return getResultAsType(resultObject, returnType);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.apache.xpath.objects.XObject eval(Object contextItem) throws javax.xml.transform.TransformerException
		private XObject eval(object contextItem)
		{
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

			xpathSupport.VarStack = new JAXPVariableStack(variableResolver);
			XObject xobj = null;

			Node contextNode = (Node)contextItem;
			// We always need to have a ContextNode with Xalan XPath implementation
			// To allow simple expression evaluation like 1+1 we are setting 
			// dummy Document as Context Node
			if (contextNode == null)
			{
				  contextNode = DummyDocument;
			}

			xobj = xpath.execute(xpathSupport, contextNode, prefixResolver);
			return xobj;
		}


		/// <summary>
		/// <para>Evaluate the compiled XPath expression in the specified context and
		///  return the result as the specified type.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec
		/// for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If <code>returnType</code> is not one of the types defined 
		/// in <seealso cref="XPathConstants"/>,
		/// then an <code>IllegalArgumentException</code> is thrown.</para>
		/// 
		/// <para>If a <code>null</code> value is provided for
		/// <code>item</code>, an empty document will be used for the
		/// context.
		/// If <code>returnType</code> is <code>null</code>, then a 
		/// <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="item"> The starting context (node or node list, for example). </param>
		/// <param name="returnType"> The desired return type.
		/// </param>
		/// <returns> The <code>Object</code> that is the result of evaluating the
		/// expression and converting the result to
		///   <code>returnType</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If the expression cannot be evaluated. </exception>
		/// <exception cref="IllegalArgumentException"> If <code>returnType</code> is not one
		/// of the types defined in <seealso cref="XPathConstants"/>. </exception>
		/// <exception cref="NullPointerException"> If  <code>returnType</code> is
		/// <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object evaluate(Object item, javax.xml.namespace.QName returnType) throws javax.xml.xpath.XPathExpressionException
		public virtual object evaluate(object item, QName returnType)
		{
			//Validating parameters to enforce constraints defined by JAXP spec
			if (returnType == null)
			{
			   //Throwing NullPointerException as defined in spec
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"returnType"});
				throw new System.NullReferenceException(fmsg);
			}
			// Checking if requested returnType is supported. returnType need to be
			// defined in XPathConstants 
			if (!isSupported(returnType))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNSUPPORTED_RETURN_TYPE, new object[] {returnType.ToString()});
				throw new System.ArgumentException(fmsg);
			}
			try
			{
				return eval(item, returnType);
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

		/// <summary>
		/// <para>Evaluate the compiled XPath expression in the specified context and
		/// return the result as a <code>String</code>.</para>
		/// 
		/// <para>This method calls <seealso cref="#evaluate(Object item, QName returnType)"/>
		/// with a <code>returnType</code> of
		/// <seealso cref="XPathConstants#STRING"/>.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec
		///  for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If a <code>null</code> value is provided for
		/// <code>item</code>, an empty document will be used for the
		/// context.
		/// 
		/// </para>
		/// </summary>
		/// <param name="item"> The starting context (node or node list, for example).
		/// </param>
		/// <returns> The <code>String</code> that is the result of evaluating the
		/// expression and converting the result to a
		///   <code>String</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If the expression cannot be evaluated. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String evaluate(Object item) throws javax.xml.xpath.XPathExpressionException
		public virtual string evaluate(object item)
		{
			return (string)this.evaluate(item, XPathConstants.STRING);
		}



		internal static DocumentBuilderFactory dbf = null;
		internal static DocumentBuilder db = null;
		internal static Document d = null;

		/// <summary>
		/// <para>Evaluate the compiled XPath expression in the context of the 
		/// specified <code>InputSource</code> and return the result as the
		///  specified type.</para>
		/// 
		/// <para>This method builds a data model for the <seealso cref="InputSource"/> and calls
		/// <seealso cref="#evaluate(Object item, QName returnType)"/> on the resulting 
		/// document object.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec
		///  for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If <code>returnType</code> is not one of the types defined in 
		/// <seealso cref="XPathConstants"/>,
		/// then an <code>IllegalArgumentException</code> is thrown.</para>
		/// 
		/// <para>If <code>source</code> or <code>returnType</code> is <code>null</code>,
		/// then a <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="source"> The <code>InputSource</code> of the document to evaluate
		/// over. </param>
		/// <param name="returnType"> The desired return type.
		/// </param>
		/// <returns> The <code>Object</code> that is the result of evaluating the
		/// expression and converting the result to
		///   <code>returnType</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If the expression cannot be evaluated. </exception>
		/// <exception cref="IllegalArgumentException"> If <code>returnType</code> is not one
		/// of the types defined in <seealso cref="XPathConstants"/>. </exception>
		/// <exception cref="NullPointerException"> If  <code>source</code> or 
		/// <code>returnType</code> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object evaluate(org.xml.sax.InputSource source, javax.xml.namespace.QName returnType) throws javax.xml.xpath.XPathExpressionException
		public virtual object evaluate(InputSource source, QName returnType)
		{
			if ((source == null) || (returnType == null))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, null);
				throw new System.NullReferenceException(fmsg);
			}
			// Checking if requested returnType is supported. returnType need to be
			// defined in XPathConstants 
			if (!isSupported(returnType))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNSUPPORTED_RETURN_TYPE, new object[] {returnType.ToString()});
				throw new System.ArgumentException(fmsg);
			}
			try
			{
				if (dbf == null)
				{
					dbf = DocumentBuilderFactory.newInstance();
					dbf.NamespaceAware = true;
					dbf.Validating = false;
				}
				db = dbf.newDocumentBuilder();
				Document document = db.parse(source);
				return eval(document, returnType);
			}
			catch (Exception e)
			{
				throw new XPathExpressionException(e);
			}
		}

		/// <summary>
		/// <para>Evaluate the compiled XPath expression in the context of the specified <code>InputSource</code> and return the result as a
		/// <code>String</code>.</para>
		/// 
		/// <para>This method calls <seealso cref="#evaluate(InputSource source, QName returnType)"/> with a <code>returnType</code> of
		/// <seealso cref="XPathConstants#STRING"/>.</para>
		/// 
		/// <para>See "Evaluation of XPath Expressions" section of JAXP 1.3 spec
		/// for context item evaluation,
		/// variable, function and QName resolution and return type conversion.</para>
		/// 
		/// <para>If <code>source</code> is <code>null</code>, then a <code>NullPointerException</code> is thrown.</para>
		/// </summary>
		/// <param name="source"> The <code>InputSource</code> of the document to evaluate over.
		/// </param>
		/// <returns> The <code>String</code> that is the result of evaluating the expression and converting the result to a
		///   <code>String</code>.
		/// </returns>
		/// <exception cref="XPathExpressionException"> If the expression cannot be evaluated. </exception>
		/// <exception cref="NullPointerException"> If  <code>source</code> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String evaluate(org.xml.sax.InputSource source) throws javax.xml.xpath.XPathExpressionException
		public virtual string evaluate(InputSource source)
		{
			return (string)this.evaluate(source, XPathConstants.STRING);
		}

		private bool isSupported(QName returnType)
		{
			// XPathConstants.STRING
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
			// If isSupported check is already done then the execution path 
			// shouldn't come here. Being defensive
			string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNSUPPORTED_RETURN_TYPE, new object[] {returnType.ToString()});
			throw new System.ArgumentException(fmsg);
		 }


		private static Document DummyDocument
		{
			get
			{
				try
				{
					if (dbf == null)
					{
						dbf = DocumentBuilderFactory.newInstance();
						dbf.NamespaceAware = true;
						dbf.Validating = false;
					}
					db = dbf.newDocumentBuilder();
    
					DOMImplementation dim = db.DOMImplementation;
					d = dim.createDocument("http://java.sun.com/jaxp/xpath", "dummyroot", null);
					return d;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				return null;
			}
		}




	}

}