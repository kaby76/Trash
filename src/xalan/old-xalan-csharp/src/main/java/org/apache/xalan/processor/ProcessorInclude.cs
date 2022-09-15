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
 * $Id: ProcessorInclude.java 469349 2006-10-31 03:06:50Z minchau $
 */
namespace org.apache.xalan.processor
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using TreeWalker = org.apache.xml.utils.TreeWalker;

	using Node = org.w3c.dom.Node;

	using Attributes = org.xml.sax.Attributes;
	using InputSource = org.xml.sax.InputSource;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// TransformerFactory class for xsl:include markup. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#include">include in XSLT Specification</a>
	/// 
	/// @xsl.usage internal </seealso>
	[Serializable]
	public class ProcessorInclude : XSLTElementProcessor
	{
		internal new const long serialVersionUID = -4570078731972673481L;

	  /// <summary>
	  /// The base URL of the XSL document.
	  /// @serial
	  /// </summary>
	  private string m_href = null;

	  /// <summary>
	  /// Get the base identifier with which this stylesheet is associated.
	  /// </summary>
	  /// <returns> non-null reference to the href attribute string, or 
	  ///         null if setHref has not been called. </returns>
	  public virtual string Href
	  {
		  get
		  {
			return m_href;
		  }
		  set
		  {
			// Validate?
			m_href = value;
		  }
	  }


	  /// <summary>
	  /// Get the stylesheet type associated with an included stylesheet
	  /// </summary>
	  /// <returns> the type of the stylesheet </returns>
	  protected internal virtual int StylesheetType
	  {
		  get
		  {
			return StylesheetHandler.STYPE_INCLUDE;
		  }
	  }

	  /// <summary>
	  /// Get the error number associated with this type of stylesheet including itself
	  /// </summary>
	  /// <returns> the appropriate error number </returns>
	  protected internal virtual string StylesheetInclErr
	  {
		  get
		  {
			return XSLTErrorResources.ER_STYLESHEET_INCLUDES_ITSELF;
		  }
	  }

	  /// <summary>
	  /// Receive notification of the start of an xsl:include element.
	  /// </summary>
	  /// <param name="handler"> The calling StylesheetHandler/TemplatesBuilder. </param>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="rawName"> The raw XML 1.0 name (with prefix), or the
	  ///        empty string if raw names are not available. </param>
	  /// <param name="attributes"> The attributes attached to the element.  If
	  ///        there are no attributes, it shall be an empty
	  ///        Attributes object.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {


		setPropertiesFromAttributes(handler, rawName, attributes, this);

		try
		{

		  // Get the Source from the user's URIResolver (if any).
		  Source sourceFromURIResolver = getSourceFromUriResolver(handler);
		  // Get the system ID of the included/imported stylesheet module
		  string hrefUrl = getBaseURIOfIncludedStylesheet(handler, sourceFromURIResolver);

		  if (handler.importStackContains(hrefUrl))
		  {
			throw new org.xml.sax.SAXException(XSLMessages.createMessage(StylesheetInclErr, new object[]{hrefUrl})); //"(StylesheetHandler) "+hrefUrl+" is directly or indirectly importing itself!");
		  }

		  // Push the system ID and corresponding Source
		  // on some stacks for later retrieval during parse() time.
		  handler.pushImportURL(hrefUrl);
		  handler.pushImportSource(sourceFromURIResolver);

		  int savedStylesheetType = handler.StylesheetType;

		  handler.StylesheetType = this.StylesheetType;
		  handler.pushNewNamespaceSupport();

		  try
		  {
			parse(handler, uri, localName, rawName, attributes);
		  }
		  finally
		  {
			handler.StylesheetType = savedStylesheetType;
			handler.popImportURL();
			handler.popImportSource();
			handler.popNamespaceSupport();
		  }
		}
		catch (TransformerException te)
		{
		  handler.error(te.Message, te);
		}
	  }

	  /// <summary>
	  /// Set off a new parse for an included or imported stylesheet.  This will 
	  /// set the <seealso cref="StylesheetHandler"/> to a new state, and recurse in with 
	  /// a new set of parse events.  Once this function returns, the state of 
	  /// the StylesheetHandler should be restored.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, which should be the XSLT namespace. </param>
	  /// <param name="localName"> The local name (without prefix), which should be "include" or "import". </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="attributes"> The list of attributes on the xsl:include or xsl:import element.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void parse(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  protected internal virtual void parse(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {
		TransformerFactoryImpl processor = handler.StylesheetProcessor;
		URIResolver uriresolver = processor.URIResolver;

		try
		{
		  Source source = null;

		  // The base identifier, an aboslute URI
		  // that is associated with the included/imported
		  // stylesheet module is known in this method,
		  // so this method does the pushing of the
		  // base ID onto the stack.

		  if (null != uriresolver)
		  {
			// There is a user provided URI resolver.
			// At the startElement() call we would
			// have tried to obtain a Source from it
			// which we now retrieve
			source = handler.peekSourceFromURIResolver();

			if (null != source && source is DOMSource)
			{
			  Node node = ((DOMSource)source).Node;

			  // There is a user provided URI resolver.
			  // At the startElement() call we would
			  // have already pushed the system ID, obtained
			  // from either the source.getSystemId(), if non-null
			  // or from SystemIDResolver.getAbsoluteURI() as a backup
			  // which we now retrieve.
			  string systemId = handler.peekImportURL();

			  // Push the absolute URI of the included/imported
			  // stylesheet module onto the stack.
			  if (!string.ReferenceEquals(systemId, null))
			  {
				  handler.pushBaseIndentifier(systemId);
			  }

			  TreeWalker walker = new TreeWalker(handler, new org.apache.xml.utils.DOM2Helper(), systemId);

			  try
			  {
				walker.traverse(node);
			  }
			  catch (org.xml.sax.SAXException se)
			  {
				throw new TransformerException(se);
			  }
			  if (!string.ReferenceEquals(systemId, null))
			  {
				handler.popBaseIndentifier();
			  }
			  return;
			}
		  }

		  if (null == source)
		  {
			string absURL = SystemIDResolver.getAbsoluteURI(Href, handler.BaseIdentifier);

			source = new StreamSource(absURL);
		  }

		  // possible callback to a class that over-rides this method.
		  source = processSource(handler, source);

		  XMLReader reader = null;

		  if (source is SAXSource)
		  {
			SAXSource saxSource = (SAXSource)source;
			reader = saxSource.XMLReader; // may be null
		  }

		  InputSource inputSource = SAXSource.sourceToInputSource(source);

		  if (null == reader)
		  {
			// Use JAXP1.1 ( if possible )
			try
			{
			  javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();
			  factory.NamespaceAware = true;

			  if (handler.StylesheetProcessor.SecureProcessing)
			  {
				try
				{
				  factory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
				}
				catch (org.xml.sax.SAXException)
				{
				}
			  }

			  javax.xml.parsers.SAXParser jaxpParser = factory.newSAXParser();
			  reader = jaxpParser.XMLReader;

			}
			catch (javax.xml.parsers.ParserConfigurationException ex)
			{
			  throw new org.xml.sax.SAXException(ex);
			}
			catch (javax.xml.parsers.FactoryConfigurationError ex1)
			{
				throw new org.xml.sax.SAXException(ex1.ToString());
			}
			catch (System.MissingMethodException)
			{
			}
			catch (AbstractMethodError)
			{
			}
		  }
		  if (null == reader)
		  {
			reader = XMLReaderFactory.createXMLReader();
		  }

		  if (null != reader)
		  {
			reader.ContentHandler = handler;

			// Push the absolute URI of the included/imported
			// stylesheet module onto the stack.
			handler.pushBaseIndentifier(inputSource.SystemId);

			try
			{
			  reader.parse(inputSource);
			}
			finally
			{
			  handler.popBaseIndentifier();
			}
		  }
		}
		catch (IOException ioe)
		{
		  handler.error(XSLTErrorResources.ER_IOEXCEPTION, new object[]{Href}, ioe);
		}
		catch (TransformerException te)
		{
		  handler.error(te.Message, te);
		}
	  }

	  /// <summary>
	  /// This method does nothing, but a class that extends this class could
	  /// over-ride it and do some processing of the source. </summary>
	  /// <param name="handler"> The calling StylesheetHandler/TemplatesBuilder. </param>
	  /// <param name="source"> The source of the included stylesheet. </param>
	  /// <returns> the same or an equivalent source to what was passed in. </returns>
	  protected internal virtual Source processSource(StylesheetHandler handler, Source source)
	  {
		  return source;
	  }

	  /// <summary>
	  /// Get the Source object for the included or imported stylesheet module
	  /// obtained from the user's URIResolver, if there is no user provided 
	  /// URIResolver null is returned.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private javax.xml.transform.Source getSourceFromUriResolver(StylesheetHandler handler) throws javax.xml.transform.TransformerException
	  private Source getSourceFromUriResolver(StylesheetHandler handler)
	  {
			Source s = null;
				TransformerFactoryImpl processor = handler.StylesheetProcessor;
				URIResolver uriresolver = processor.URIResolver;
				if (uriresolver != null)
				{
					string href = Href;
					string @base = handler.BaseIdentifier;
					s = uriresolver.resolve(href,@base);
				}

			return s;
	  }

		/// <summary>
		/// Get the base URI of the included or imported stylesheet,
		/// if the user provided a URIResolver, then get the Source
		/// object for the stylsheet from it, and get the systemId 
		/// from that Source object, otherwise try to recover by
		/// using the SysteIDResolver to figure out the base URI. </summary>
		/// <param name="handler"> The handler that processes the stylesheet as SAX events,
		/// and maintains state </param>
		/// <param name="s"> The Source object from a URIResolver, for the included stylesheet module,
		/// so this will be null if there is no URIResolver set. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getBaseURIOfIncludedStylesheet(StylesheetHandler handler, javax.xml.transform.Source s) throws javax.xml.transform.TransformerException
		private string getBaseURIOfIncludedStylesheet(StylesheetHandler handler, Source s)
		{



			string baseURI;
			string idFromUriResolverSource;
			if (s != null && !string.ReferenceEquals((idFromUriResolverSource = s.SystemId), null))
			{
				// We have a Source obtained from a users's URIResolver,
				// and the system ID is set on it, so return that as the base URI
				baseURI = idFromUriResolverSource;
			}
			else
			{
				// The user did not provide a URIResolver, or it did not 
				// return a Source for the included stylesheet module, or
				// the Source has no system ID set, so we fall back to using
				// the system ID Resolver to take the href and base
				// to generate the baseURI of the included stylesheet.
				baseURI = SystemIDResolver.getAbsoluteURI(Href, handler.BaseIdentifier);
			}

			return baseURI;
		}
	}

}