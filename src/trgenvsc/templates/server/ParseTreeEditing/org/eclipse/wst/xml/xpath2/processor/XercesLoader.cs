//using System;

///// <summary>
/////*****************************************************************************
///// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
///// All rights reserved. This program and the accompanying materials
///// are made available under the terms of the Eclipse Public License 2.0
///// which accompanies this distribution, and is available at
///// https://www.eclipse.org/legal/epl-2.0/
///// 
///// SPDX-License-Identifier: EPL-2.0
///// 
///// Contributors:
/////     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
/////     Mukul Gandhi - bug 276134 - improvements to schema aware primitive type support
/////                                 for attribute/element nodes  
/////     Jesper Steen Moller - Fixed namespace awareness
/////     David Carver  - bug 281186 - implementation of fn:id and fn:idref.  Correct
/////                                  loading of grammars if non-validating.
/////     Mukul Gandhi - bug 280798 -  PsychoPath support for JDK 1.4
///// ******************************************************************************
///// </summary>

//namespace org.eclipse.wst.xml.xpath2.processor
//{

//	using org.w3c.dom;

//	using org.xml.sax;

//	/// <summary>
//	/// Xerces loader class. The loading is always namespace aware.
//	/// </summary>
//	public class XercesLoader : DOMLoader
//	{

//		private const string NONVALIDATING_LOAD_DTD_GRAMMAR = "http://apache.org/xml/features/nonvalidating/load-dtd-grammar";

//		public const string NAMESPACES_FEATURE = "http://xml.org/sax/features/namespaces";

//		public const string VALIDATION_FEATURE = "http://xml.org/sax/features/validation";

//		public const string SCHEMA_VALIDATION_FEATURE = "http://apache.org/xml/features/validation/schema";

//		public const string SCHEMA_FULL_CHECKING_FEATURE = "http://apache.org/xml/features/validation/schema-full-checking";

//		public const string DYNAMIC_VALIDATION_FEATURE = "http://apache.org/xml/features/validation/dynamic";

//		public const string LOAD_EXTERNAL_DTD_FEATURE = "http://apache.org/xml/features/nonvalidating/load-external-dtd";

//		public const string JAXP_SCHEMA_LANGUAGE = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";
//		public const string W3C_XML_SCHEMA = "http://www.w3.org/2001/XMLSchema";

//		public const string DOCUMENT_IMPLEMENTATION_PROPERTY = "http://apache.org/xml/properties/dom/document-class-name";
//		public const string DOCUMENT_PSVI_IMPLEMENTATION = "org.apache.xerces.dom.PSVIDocumentImpl";

//		internal bool _validating;

//		internal Schema _schema = null;

//		/// <summary>
//		/// Constructor for Xerces loader.
//		/// </summary>
//		public XercesLoader()
//		{
//			_validating = false;
//		}

//		/// <summary>
//		/// @since 1.1
//		/// </summary>
//		public XercesLoader(Schema schema)
//		{
//			_validating = false;
//			_schema = schema;
//		}

//		/// <summary>
//		/// The Xerces loader loads the XML document
//		/// </summary>
//		/// <param name="in">
//		///            is the input stream. </param>
//		/// <exception cref="DOMLoaderException">
//		///             DOM loader exception. </exception>
//		/// <returns> The loaded document. </returns>
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public Document load(InputStream in) throws DOMLoaderException
//		public virtual Document load(System.IO.Stream @in)
//		{

//			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();

//			factory.NamespaceAware = true;
//			factory.setAttribute(SCHEMA_VALIDATION_FEATURE, Convert.ToBoolean(_validating));
//			factory.setAttribute(LOAD_EXTERNAL_DTD_FEATURE, true);
//			factory.setAttribute(NONVALIDATING_LOAD_DTD_GRAMMAR, true);
//			factory.setAttribute(DOCUMENT_IMPLEMENTATION_PROPERTY, DOCUMENT_PSVI_IMPLEMENTATION);

//			if (_schema != null)
//			{
//			  factory.Schema = _schema;
//			}
//			else
//			{
//			  factory.Validating = _validating;
//			}

//			try
//			{
//				DocumentBuilder builder = factory.newDocumentBuilder();

//				if (_validating)
//				{
//					builder.ErrorHandler = new ErrorHandlerAnonymousInnerClass(this);
//				}
//				return builder.parse(@in);
//			}
//			catch (SAXException e)
//			{
//				//throw new DOMLoaderException("SAX exception: " + e.getMessage());
//				Console.WriteLine(e.ToString());
//				Console.Write(e.StackTrace);
//			}
//			catch (ParserConfigurationException e)
//			{
//				throw new DOMLoaderException("Parser configuration exception: " + e.Message);
//			}
//			catch (IOException e)
//			{
//				throw new DOMLoaderException("IO exception: " + e.Message);
//			}

//			return null;

//		}

//		private class ErrorHandlerAnonymousInnerClass : ErrorHandler
//		{
//			private readonly XercesLoader outerInstance;

//			public ErrorHandlerAnonymousInnerClass(XercesLoader outerInstance)
//			{
//				this.outerInstance = outerInstance;
//			}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void fatalError(SAXParseException e) throws SAXException
//			public virtual void fatalError(SAXParseException e)
//			{
//				throw e;
//			}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void error(SAXParseException e) throws SAXParseException
//			public virtual void error(SAXParseException e)
//			{
//				throw e;
//			}

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void warning(SAXParseException e) throws SAXParseException
//			public virtual void warning(SAXParseException e)
//			{
//				throw e; // XXX
//			}
//		}

//		/// <summary>
//		/// Set validating boolean.
//		/// </summary>
//		/// <param name="x">
//		///            is the value to set the validating boolean to. </param>
//		public virtual void set_validating(bool x)
//		{
//			_validating = x;
//		}
//	}

//}