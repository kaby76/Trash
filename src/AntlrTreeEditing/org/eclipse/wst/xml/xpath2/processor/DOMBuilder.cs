/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2009 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
///     Jesper Steen Moller - Doc fixes
/// ******************************************************************************
/// </summary>

using System.IO;
using javax.xml.parsers;

namespace org.eclipse.wst.xml.xpath2.processor
{

	using org.w3c.dom;
    using SAXParseException = org.xml.sax.SAXParseException;
    using SAXException = org.xml.sax.SAXException;
	using ParserConfigurationException = org.xml.sax.ParserConfigurationException;
	using DocumentBuilderFactory = javax.xml.parsers.DocumentBuilderFactor;
	using DocumentBuilder = javax.xml.parsers.DocumentBuilder;

	/// <summary>
	/// The DOM builder loads an DOM from an InputStream. The loading is always namespace aware.
	/// </summary>
	public class DOMBuilder : DOMLoader
	{
		internal bool _validating;
		internal bool _namespace_aware;

		/// <summary>
		/// Constructor for DOM builder.
		/// </summary>
		public DOMBuilder()
		{
			_validating = false;
		}

		/// <summary>
		/// Loads The XML document.
		/// </summary>
		/// <param name="in">
		///            is the input stream. </param>
		/// <exception cref="DOMLoaderException">
		///             DOM loader exception. </exception>
		/// <returns> The loaded document. </returns>
		// XXX: fix error reporting
		public virtual Document load(System.IO.Stream @in)
		{

			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();

			factory.NamespaceAware = true;
			factory.Validating = _validating;

			try
			{
				DocumentBuilder builder = factory.newDocumentBuilder();

				// if(_validating) {
				builder.ErrorHandler = new ErrorHandlerAnonymousInnerClass(this);
				// }
				return builder.parse(@in);
			}
			catch (SAXException e)
			{
				throw new DOMLoaderException("SAX exception: " + e.Message);
			}
			catch (ParserConfigurationException e)
			{
				throw new DOMLoaderException("Parser configuration exception: " + e.Message);
			}
			catch (IOException e)
			{
				throw new DOMLoaderException("IO exception: " + e.Message);
			}
		}

        public class ErrorHandlerAnonymousInnerClass //: ErrorHandler
		{
			private readonly DOMBuilder outerInstance;

			public ErrorHandlerAnonymousInnerClass(DOMBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(SAXParseException e) throws SAXException
			public virtual void fatalError(SAXParseException e)
			{
				throw e;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(SAXParseException e) throws SAXParseException
			public virtual void error(SAXParseException e)
			{
				throw e;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(SAXParseException e) throws SAXParseException
			public virtual void warning(SAXParseException e)
			{
				throw e; // XXX
			}
		}

		/// <summary>
		/// Set validating boolean.
		/// </summary>
		/// <param name="x">
		///            is the value to set the validating boolean to. </param>
		public virtual void set_validating(bool x)
		{
			_validating = x;
		}
	}

}