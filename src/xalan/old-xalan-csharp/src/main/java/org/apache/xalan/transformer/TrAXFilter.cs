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
 * $Id: TrAXFilter.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using EntityResolver = org.xml.sax.EntityResolver;
	using InputSource = org.xml.sax.InputSource;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLFilterImpl = org.xml.sax.helpers.XMLFilterImpl;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;


	public class TrAXFilter : XMLFilterImpl
	{
	  private Templates m_templates;
	  private TransformerImpl m_transformer;

	  /// <summary>
	  /// Construct an empty XML filter, with no parent.
	  /// 
	  /// <para>This filter will have no parent: you must assign a parent
	  /// before you start a parse or do any configuration with
	  /// setFeature or setProperty.</para>
	  /// </summary>
	  /// <seealso cref= org.xml.sax.XMLReader#setFeature </seealso>
	  /// <seealso cref= org.xml.sax.XMLReader#setProperty </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TrAXFilter(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
	  public TrAXFilter(Templates templates)
	  {
		m_templates = templates;
		m_transformer = (TransformerImpl)templates.newTransformer();
	  }

	  /// <summary>
	  /// Return the Transformer object used for this XML filter.
	  /// </summary>
	  public virtual TransformerImpl Transformer
	  {
		  get
		  {
			return m_transformer;
		  }
	  }

	  /// <summary>
	  /// Set the parent reader.
	  /// 
	  /// <para>This is the <seealso cref="org.xml.sax.XMLReader XMLReader"/> from which 
	  /// this filter will obtain its events and to which it will pass its 
	  /// configuration requests.  The parent may itself be another filter.</para>
	  /// 
	  /// <para>If there is no parent reader set, any attempt to parse
	  /// or to set or get a feature or property will fail.</para>
	  /// </summary>
	  /// <param name="parent"> The parent XML reader. </param>
	  /// <exception cref="java.lang.NullPointerException"> If the parent is null. </exception>
	  public virtual XMLReader Parent
	  {
		  set
		  {
			base.Parent = value;
    
			if (null != value.ContentHandler)
			{
			  this.ContentHandler = value.ContentHandler;
			}
    
			// Not really sure if we should do this here, but 
			// it seems safer in case someone calls parse() on 
			// the value.
			setupParse();
		  }
	  }

	  /// <summary>
	  /// Parse a document.
	  /// </summary>
	  /// <param name="input"> The input source for the document entity. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <exception cref="java.io.IOException"> An IO exception from the parser,
	  ///            possibly from a byte stream or character stream
	  ///            supplied by the application. </exception>
	  /// <seealso cref= org.xml.sax.XMLReader#parse(org.xml.sax.InputSource) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(org.xml.sax.InputSource input) throws org.xml.sax.SAXException, java.io.IOException
	  public virtual void parse(InputSource input)
	  {
		if (null == Parent)
		{
		  XMLReader reader = null;

		  // Use JAXP1.1 ( if possible )
		  try
		  {
			  javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();
			  factory.NamespaceAware = true;

			  if (m_transformer.Stylesheet.SecureProcessing)
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

		  XMLReader parent;
		  if (reader == null)
		  {
			  parent = XMLReaderFactory.createXMLReader();
		  }
		  else
		  {
			  parent = reader;
		  }
		  try
		  {
			parent.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
		  }
		  catch (org.xml.sax.SAXException)
		  {
		  }
		  // setParent calls setupParse...
		  Parent = parent;
		}
		else
		{
		  // Make sure everything is set up.
		  setupParse();
		}
		if (null == m_transformer.ContentHandler)
		{
		  throw new org.xml.sax.SAXException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_CALL_PARSE, null)); //"parse can not be called if the ContentHandler has not been set!");
		}

		Parent.parse(input);
		Exception e = m_transformer.ExceptionThrown;
		if (null != e)
		{
		  if (e is org.xml.sax.SAXException)
		  {
			throw (org.xml.sax.SAXException)e;
		  }
		  else
		  {
			throw new org.xml.sax.SAXException(e);
		  }
		}
	  }

	  /// <summary>
	  /// Parse a document.
	  /// </summary>
	  /// <param name="systemId"> The system identifier as a fully-qualified URI. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <exception cref="java.io.IOException"> An IO exception from the parser,
	  ///            possibly from a byte stream or character stream
	  ///            supplied by the application. </exception>
	  /// <seealso cref= org.xml.sax.XMLReader#parse(java.lang.String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(String systemId) throws org.xml.sax.SAXException, java.io.IOException
	  public virtual void parse(string systemId)
	  {
		parse(new InputSource(systemId));
	  }


	  /// <summary>
	  /// Set up before a parse.
	  /// 
	  /// <para>Before every parse, check whether the parent is
	  /// non-null, and re-register the filter for all of the 
	  /// events.</para>
	  /// </summary>
	  private void setupParse()
	  {
		XMLReader p = Parent;
		if (p == null)
		{
		  throw new System.NullReferenceException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_PARENT_FOR_FILTER, null)); //"No parent for filter");
		}

		ContentHandler ch = m_transformer.InputContentHandler;
	//    if(ch instanceof SourceTreeHandler)
	//      ((SourceTreeHandler)ch).setUseMultiThreading(true);
		p.ContentHandler = ch;
		p.EntityResolver = this;
		p.DTDHandler = this;
		p.ErrorHandler = this;
	  }

	  /// <summary>
	  /// Set the content event handler.
	  /// </summary>
	  /// <param name="handler"> The new content handler. </param>
	  /// <exception cref="java.lang.NullPointerException"> If the handler
	  ///            is null. </exception>
	  /// <seealso cref= org.xml.sax.XMLReader#setContentHandler </seealso>
	  public virtual ContentHandler ContentHandler
	  {
		  set
		  {
			m_transformer.ContentHandler = value;
			// super.setContentHandler(m_transformer.getResultTreeHandler());
		  }
	  }

	  public virtual ErrorListener ErrorListener
	  {
		  set
		  {
			m_transformer.ErrorListener = value;
		  }
	  }

	}

}