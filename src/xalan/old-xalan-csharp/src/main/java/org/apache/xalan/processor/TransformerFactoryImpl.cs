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
 * $Id: TransformerFactoryImpl.java 1581058 2014-03-24 20:55:14Z ggregory $
 */
namespace org.apache.xalan.processor
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TrAXFilter = org.apache.xalan.transformer.TrAXFilter;
	using TransformerIdentityImpl = org.apache.xalan.transformer.TransformerIdentityImpl;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XalanProperties = org.apache.xalan.transformer.XalanProperties;
	using StopParseException = org.apache.xml.utils.StopParseException;
	using StylesheetPIHandler = org.apache.xml.utils.StylesheetPIHandler;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using TreeWalker = org.apache.xml.utils.TreeWalker;
	using Node = org.w3c.dom.Node;
	using InputSource = org.xml.sax.InputSource;
	using XMLFilter = org.xml.sax.XMLFilter;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// The TransformerFactoryImpl, which implements the TRaX TransformerFactory
	/// interface, processes XSLT stylesheets into a Templates object
	/// (a StylesheetRoot).
	/// </summary>
	public class TransformerFactoryImpl : SAXTransformerFactory
	{
	  /// <summary>
	  /// The path/filename of the property file: XSLTInfo.properties  
	  /// Maintenance note: see also
	  /// <code>org.apache.xpath.functions.FuncSystemProperty.XSLT_PROPERTIES</code>
	  /// </summary>
	  public const string XSLT_PROPERTIES = "org/apache/xalan/res/XSLTInfo.properties";

	  /// <summary>
	  /// <para>State of secure processing feature.</para>
	  /// </summary>
	  private bool m_isSecureProcessing = false;

	  /// <summary>
	  /// Constructor TransformerFactoryImpl
	  /// 
	  /// </summary>
	  public TransformerFactoryImpl()
	  {
	  }

	  /// <summary>
	  /// Static string to be used for incremental feature </summary>
	  public const string FEATURE_INCREMENTAL = "http://xml.apache.org/xalan/features/incremental";

	  /// <summary>
	  /// Static string to be used for optimize feature </summary>
	  public const string FEATURE_OPTIMIZE = "http://xml.apache.org/xalan/features/optimize";

	  /// <summary>
	  /// Static string to be used for source_location feature </summary>
	  public const string FEATURE_SOURCE_LOCATION = XalanProperties.SOURCE_LOCATION;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.Templates processFromNode(org.w3c.dom.Node node) throws javax.xml.transform.TransformerConfigurationException
	  public virtual Templates processFromNode(Node node)
	  {

		try
		{
		  TemplatesHandler builder = newTemplatesHandler();
		  TreeWalker walker = new TreeWalker(builder, new org.apache.xml.utils.DOM2Helper(), builder.SystemId);

		  walker.traverse(node);

		  return builder.Templates;
		}
		catch (org.xml.sax.SAXException se)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(new TransformerException(se));
			}
			catch (TransformerConfigurationException ex)
			{
			  throw ex;
			}
			catch (TransformerException ex)
			{
			  throw new TransformerConfigurationException(ex);
			}

			return null;
		  }
		  else
		  {

			// Should remove this later... but right now diagnostics from 
			// TransformerConfigurationException are not good.
			// se.printStackTrace();
			throw new TransformerConfigurationException(XSLMessages.createMessage(XSLTErrorResources.ER_PROCESSFROMNODE_FAILED, null), se);
			//"processFromNode failed", se);
		  }
		}
		catch (TransformerConfigurationException tce)
		{
		  // Assume it's already been reported to the error listener.
		  throw tce;
		}
	   /* catch (TransformerException tce)
	    {
	      // Assume it's already been reported to the error listener.
	      throw new TransformerConfigurationException(tce.getMessage(), tce);
	    }*/
		catch (Exception e)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(new TransformerException(e));
			}
			catch (TransformerConfigurationException ex)
			{
			  throw ex;
			}
			catch (TransformerException ex)
			{
			  throw new TransformerConfigurationException(ex);
			}

			return null;
		  }
		  else
		  {
			// Should remove this later... but right now diagnostics from 
			// TransformerConfigurationException are not good.
			// se.printStackTrace();
			throw new TransformerConfigurationException(XSLMessages.createMessage(XSLTErrorResources.ER_PROCESSFROMNODE_FAILED, null), e); //"processFromNode failed",
														//e);
		  }
		}
	  }

	  /// <summary>
	  /// The systemID that was specified in
	  /// processFromNode(Node node, String systemID).
	  /// </summary>
	  private string m_DOMsystemID = null;

	  /// <summary>
	  /// The systemID that was specified in
	  /// processFromNode(Node node, String systemID).
	  /// </summary>
	  /// <returns> The systemID, or null. </returns>
	  internal virtual string DOMsystemID
	  {
		  get
		  {
			return m_DOMsystemID;
		  }
	  }

	  /// <summary>
	  /// Process the stylesheet from a DOM tree, if the
	  /// processor supports the "http://xml.org/trax/features/dom/input"
	  /// feature.
	  /// </summary>
	  /// <param name="node"> A DOM tree which must contain
	  /// valid transform instructions that this processor understands. </param>
	  /// <param name="systemID"> The systemID from where xsl:includes and xsl:imports
	  /// should be resolved from.
	  /// </param>
	  /// <returns> A Templates object capable of being used for transformation purposes.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: javax.xml.transform.Templates processFromNode(org.w3c.dom.Node node, String systemID) throws javax.xml.transform.TransformerConfigurationException
	  internal virtual Templates processFromNode(Node node, string systemID)
	  {

		m_DOMsystemID = systemID;

		return processFromNode(node);
	  }

	  /// <summary>
	  /// Get InputSource specification(s) that are associated with the
	  /// given document specified in the source param,
	  /// via the xml-stylesheet processing instruction
	  /// (see http://www.w3.org/TR/xml-stylesheet/), and that matches
	  /// the given criteria.  Note that it is possible to return several stylesheets
	  /// that match the criteria, in which case they are applied as if they were
	  /// a list of imports or cascades.
	  /// 
	  /// <para>Note that DOM2 has it's own mechanism for discovering stylesheets.
	  /// Therefore, there isn't a DOM version of this method.</para>
	  /// 
	  /// </summary>
	  /// <param name="source"> The XML source that is to be searched. </param>
	  /// <param name="media"> The media attribute to be matched.  May be null, in which
	  ///              case the prefered templates will be used (i.e. alternate = no). </param>
	  /// <param name="title"> The value of the title attribute to match.  May be null. </param>
	  /// <param name="charset"> The value of the charset attribute to match.  May be null.
	  /// </param>
	  /// <returns> A Source object capable of being used to create a Templates object.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.Source getAssociatedStylesheet(javax.xml.transform.Source source, String media, String title, String charset) throws javax.xml.transform.TransformerConfigurationException
	  public virtual Source getAssociatedStylesheet(Source source, string media, string title, string charset)
	  {

		string baseID;
		InputSource isource = null;
		Node node = null;
		XMLReader reader = null;

		if (source is DOMSource)
		{
		  DOMSource dsource = (DOMSource) source;

		  node = dsource.Node;
		  baseID = dsource.SystemId;
		}
		else
		{
		  isource = SAXSource.sourceToInputSource(source);
		  baseID = isource.SystemId;
		}

		// What I try to do here is parse until the first startElement
		// is found, then throw a special exception in order to terminate 
		// the parse.
		StylesheetPIHandler handler = new StylesheetPIHandler(baseID, media, title, charset);

		// Use URIResolver. Patch from Dmitri Ilyin 
		if (m_uriResolver != null)
		{
		  handler.URIResolver = m_uriResolver;
		}

		try
		{
		  if (null != node)
		  {
			TreeWalker walker = new TreeWalker(handler, new org.apache.xml.utils.DOM2Helper(), baseID);

			walker.traverse(node);
		  }
		  else
		  {

			// Use JAXP1.1 ( if possible )
			try
			{
			  javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();

			  factory.NamespaceAware = true;

			  if (m_isSecureProcessing)
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

			if (null == reader)
			{
			  reader = XMLReaderFactory.createXMLReader();
			}

			if (m_isSecureProcessing)
			{
				reader.setFeature("http://xml.org/sax/features/external-general-entities",false);
			}
			// Need to set options!
			reader.ContentHandler = handler;
			reader.parse(isource);
		  }
		}
		catch (StopParseException)
		{

		  // OK, good.
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerConfigurationException("getAssociatedStylesheets failed", se);
		}
		catch (IOException ioe)
		{
		  throw new TransformerConfigurationException("getAssociatedStylesheets failed", ioe);
		}

		return handler.AssociatedStylesheet;
	  }

	  /// <summary>
	  /// Create a new Transformer object that performs a copy
	  /// of the source to the result.
	  /// </summary>
	  /// <returns> A Transformer object that may be used to perform a transformation
	  /// in a single thread, never null.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> May throw this during
	  ///            the parse when it is constructing the
	  ///            Templates object and fails. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.sax.TemplatesHandler newTemplatesHandler() throws javax.xml.transform.TransformerConfigurationException
	  public virtual TemplatesHandler newTemplatesHandler()
	  {
		return new StylesheetHandler(this);
	  }

	  /// <summary>
	  /// <para>Set a feature for this <code>TransformerFactory</code> and <code>Transformer</code>s
	  /// or <code>Template</code>s created by this factory.</para>
	  /// 
	  /// <para>
	  /// Feature names are fully qualified <seealso cref="java.net.URI"/>s.
	  /// Implementations may define their own features.
	  /// An <seealso cref="TransformerConfigurationException"/> is thrown if this <code>TransformerFactory</code> or the
	  /// <code>Transformer</code>s or <code>Template</code>s it creates cannot support the feature.
	  /// It is possible for an <code>TransformerFactory</code> to expose a feature value but be unable to change its state.
	  /// </para>
	  /// 
	  /// <para>See <seealso cref="javax.xml.transform.TransformerFactory"/> for full documentation of specific features.</para>
	  /// </summary>
	  /// <param name="name"> Feature name. </param>
	  /// <param name="value"> Is feature state <code>true</code> or <code>false</code>.
	  /// </param>
	  /// <exception cref="TransformerConfigurationException"> if this <code>TransformerFactory</code>
	  ///   or the <code>Transformer</code>s or <code>Template</code>s it creates cannot support this feature. </exception>
	  /// <exception cref="NullPointerException"> If the <code>name</code> parameter is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFeature(String name, boolean value) throws javax.xml.transform.TransformerConfigurationException
	  public virtual void setFeature(string name, bool value)
	  {

		  // feature name cannot be null
		  if (string.ReferenceEquals(name, null))
		  {
			  throw new System.NullReferenceException(XSLMessages.createMessage(XSLTErrorResources.ER_SET_FEATURE_NULL_NAME, null));
		  }

		  // secure processing?
		  if (name.Equals(XMLConstants.FEATURE_SECURE_PROCESSING))
		  {
			  m_isSecureProcessing = value;
		  }
		  // This implementation does not support the setting of a feature other than
		  // the secure processing feature.
		  else
		  {
		  throw new TransformerConfigurationException(XSLMessages.createMessage(XSLTErrorResources.ER_UNSUPPORTED_FEATURE, new object[] {name}));
		  }
	  }

	  /// <summary>
	  /// Look up the value of a feature.
	  /// <para>The feature name is any fully-qualified URI.  It is
	  /// possible for an TransformerFactory to recognize a feature name but
	  /// to be unable to return its value; this is especially true
	  /// in the case of an adapter for a SAX1 Parser, which has
	  /// no way of knowing whether the underlying parser is
	  /// validating, for example.</para>
	  /// </summary>
	  /// <param name="name"> The feature name, which is a fully-qualified URI. </param>
	  /// <returns> The current state of the feature (true or false). </returns>
	  public virtual bool getFeature(string name)
	  {

		// feature name cannot be null
		if (string.ReferenceEquals(name, null))
		{
			throw new System.NullReferenceException(XSLMessages.createMessage(XSLTErrorResources.ER_GET_FEATURE_NULL_NAME, null));
		}

		// Try first with identity comparison, which 
		// will be faster.
		if ((DOMResult.FEATURE == name) || (DOMSource.FEATURE == name) || (SAXResult.FEATURE == name) || (SAXSource.FEATURE == name) || (StreamResult.FEATURE == name) || (StreamSource.FEATURE == name) || (SAXTransformerFactory.FEATURE == name) || (SAXTransformerFactory.FEATURE_XMLFILTER == name))
		{
		  return true;
		}
		else if ((DOMResult.FEATURE.Equals(name)) || (DOMSource.FEATURE.Equals(name)) || (SAXResult.FEATURE.Equals(name)) || (SAXSource.FEATURE.Equals(name)) || (StreamResult.FEATURE.Equals(name)) || (StreamSource.FEATURE.Equals(name)) || (SAXTransformerFactory.FEATURE.Equals(name)) || (SAXTransformerFactory.FEATURE_XMLFILTER.Equals(name)))
		{
		  return true;
		}
		// secure processing?
		else if (name.Equals(XMLConstants.FEATURE_SECURE_PROCESSING))
		{
		  return m_isSecureProcessing;
		}
		else
		{
		  // unknown feature
		  return false;
		}
	  }

	  /// <summary>
	  /// Flag set by FEATURE_OPTIMIZE.
	  /// This feature specifies whether to Optimize stylesheet processing. By
	  /// default it is set to true.
	  /// </summary>
	  private bool m_optimize = true;

	  /// <summary>
	  /// Flag set by FEATURE_SOURCE_LOCATION.
	  /// This feature specifies whether the transformation phase should
	  /// keep track of line and column numbers for the input source
	  /// document. Note that this works only when that
	  /// information is available from the source -- in other words, if you
	  /// pass in a DOM, there's little we can do for you.
	  /// 
	  /// The default is false. Setting it true may significantly
	  /// increase storage cost per node. 
	  /// </summary>
	  private bool m_source_location = false;

	  /// <summary>
	  /// Flag set by FEATURE_INCREMENTAL.
	  /// This feature specifies whether to produce output incrementally, rather than
	  /// waiting to finish parsing the input before generating any output. By 
	  /// default this attribute is set to false. 
	  /// </summary>
	  private bool m_incremental = false;

	  /// <summary>
	  /// Allows the user to set specific attributes on the underlying
	  /// implementation.
	  /// </summary>
	  /// <param name="name"> The name of the attribute. </param>
	  /// <param name="value"> The value of the attribute; Boolean or String="true"|"false"
	  /// </param>
	  /// <exception cref="IllegalArgumentException"> thrown if the underlying
	  /// implementation doesn't recognize the attribute. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setAttribute(String name, Object value) throws IllegalArgumentException
	  public virtual void setAttribute(string name, object value)
	  {
		if (name.Equals(FEATURE_INCREMENTAL))
		{
		  if (value is bool?)
		  {
			// Accept a Boolean object..
			m_incremental = ((bool?)value).Value;
		  }
		  else if (value is string)
		  {
			// .. or a String object
			m_incremental = (new bool?((string)value));
		  }
		  else
		  {
			// Give a more meaningful error message
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_BAD_VALUE, new object[]{name, value})); //name + " bad value " + value);
		  }
		}
		else if (name.Equals(FEATURE_OPTIMIZE))
		{
		  if (value is bool?)
		  {
			// Accept a Boolean object..
			m_optimize = ((bool?)value).Value;
		  }
		  else if (value is string)
		  {
			// .. or a String object
			m_optimize = (new bool?((string)value));
		  }
		  else
		  {
			// Give a more meaningful error message
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_BAD_VALUE, new object[]{name, value})); //name + " bad value " + value);
		  }
		}

		// Custom Xalan feature: annotate DTM with SAX source locator fields.
		// This gets used during SAX2DTM instantiation. 
		//
		// %REVIEW% Should the name of this field really be in XalanProperties?
		// %REVIEW% I hate that it's a global static, but didn't want to change APIs yet.
		else if (name.Equals(FEATURE_SOURCE_LOCATION))
		{
		  if (value is bool?)
		  {
			// Accept a Boolean object..
			m_source_location = ((bool?)value).Value;
		  }
		  else if (value is string)
		  {
			// .. or a String object
			m_source_location = (new bool?((string)value));
		  }
		  else
		  {
			// Give a more meaningful error message
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_BAD_VALUE, new object[]{name, value})); //name + " bad value " + value);
		  }
		}

		else
		{
		  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_NOT_SUPPORTED, new object[]{name})); //name + "not supported");
		}
	  }

	  /// <summary>
	  /// Allows the user to retrieve specific attributes on the underlying
	  /// implementation.
	  /// </summary>
	  /// <param name="name"> The name of the attribute. </param>
	  /// <returns> value The value of the attribute.
	  /// </returns>
	  /// <exception cref="IllegalArgumentException"> thrown if the underlying
	  /// implementation doesn't recognize the attribute. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object getAttribute(String name) throws IllegalArgumentException
	  public virtual object getAttribute(string name)
	  {
		if (name.Equals(FEATURE_INCREMENTAL))
		{
		  return m_incremental ? true : false;
		}
		else if (name.Equals(FEATURE_OPTIMIZE))
		{
		  return m_optimize ? true : false;
		}
		else if (name.Equals(FEATURE_SOURCE_LOCATION))
		{
		  return m_source_location ? true : false;
		}
		else
		{
		  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_ATTRIB_VALUE_NOT_RECOGNIZED, new object[]{name})); //name + " attribute not recognized");
		}
	  }

	  /// <summary>
	  /// Create an XMLFilter that uses the given source as the
	  /// transformation instructions.
	  /// </summary>
	  /// <param name="src"> The source of the transformation instructions.
	  /// </param>
	  /// <returns> An XMLFilter object, or null if this feature is not supported.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.XMLFilter newXMLFilter(javax.xml.transform.Source src) throws javax.xml.transform.TransformerConfigurationException
	  public virtual XMLFilter newXMLFilter(Source src)
	  {

		Templates templates = newTemplates(src);
		if (templates == null)
		{
			return null;
		}

		return newXMLFilter(templates);
	  }

	  /// <summary>
	  /// Create an XMLFilter that uses the given source as the
	  /// transformation instructions.
	  /// </summary>
	  /// <param name="templates"> non-null reference to Templates object.
	  /// </param>
	  /// <returns> An XMLFilter object, or null if this feature is not supported.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.XMLFilter newXMLFilter(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
	  public virtual XMLFilter newXMLFilter(Templates templates)
	  {
		try
		{
		  return new TrAXFilter(templates);
		}
		catch (TransformerConfigurationException ex)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(ex);
			  return null;
			}
			catch (TransformerConfigurationException ex1)
			{
			  throw ex1;
			}
			catch (TransformerException ex1)
			{
			  throw new TransformerConfigurationException(ex1);
			}
		  }
		  throw ex;
		}
	  }

	  /// <summary>
	  /// Get a TransformerHandler object that can process SAX
	  /// ContentHandler events into a Result, based on the transformation
	  /// instructions specified by the argument.
	  /// </summary>
	  /// <param name="src"> The source of the transformation instructions.
	  /// </param>
	  /// <returns> TransformerHandler ready to transform SAX events.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler(javax.xml.transform.Source src) throws javax.xml.transform.TransformerConfigurationException
	  public virtual TransformerHandler newTransformerHandler(Source src)
	  {

		Templates templates = newTemplates(src);
		if (templates == null)
		{
			return null;
		}

		return newTransformerHandler(templates);
	  }

	  /// <summary>
	  /// Get a TransformerHandler object that can process SAX
	  /// ContentHandler events into a Result, based on the Templates argument.
	  /// </summary>
	  /// <param name="templates"> The source of the transformation instructions.
	  /// </param>
	  /// <returns> TransformerHandler ready to transform SAX events. </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
	  public virtual TransformerHandler newTransformerHandler(Templates templates)
	  {
		try
		{
		  TransformerImpl transformer = (TransformerImpl) templates.newTransformer();
		  transformer.URIResolver = m_uriResolver;
		  TransformerHandler th = (TransformerHandler) transformer.getInputContentHandler(true);

		  return th;
		}
		catch (TransformerConfigurationException ex)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(ex);
			  return null;
			}
			catch (TransformerConfigurationException ex1)
			{
			  throw ex1;
			}
			catch (TransformerException ex1)
			{
			  throw new TransformerConfigurationException(ex1);
			}
		  }

		  throw ex;
		}

	  }

	//  /** The identity transform string, for support of newTransformerHandler()
	//   *  and newTransformer().  */
	//  private static final String identityTransform =
	//    "<xsl:stylesheet " + "xmlns:xsl='http://www.w3.org/1999/XSL/Transform' "
	//    + "version='1.0'>" + "<xsl:template match='/|node()'>"
	//    + "<xsl:copy-of select='.'/>" + "</xsl:template>" + "</xsl:stylesheet>";
	//
	//  /** The identity transform Templates, built from identityTransform, 
	//   *  for support of newTransformerHandler() and newTransformer().  */
	//  private static Templates m_identityTemplate = null;

	  /// <summary>
	  /// Get a TransformerHandler object that can process SAX
	  /// ContentHandler events into a Result.
	  /// </summary>
	  /// <returns> TransformerHandler ready to transform SAX events.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler() throws javax.xml.transform.TransformerConfigurationException
	  public virtual TransformerHandler newTransformerHandler()
	  {
		return new TransformerIdentityImpl(m_isSecureProcessing);
	  }

	  /// <summary>
	  /// Process the source into a Transformer object.  Care must
	  /// be given to know that this object can not be used concurrently
	  /// in multiple threads.
	  /// </summary>
	  /// <param name="source"> An object that holds a URL, input stream, etc.
	  /// </param>
	  /// <returns> A Transformer object capable of
	  /// being used for transformation purposes in a single thread.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> May throw this during the parse when it
	  ///            is constructing the Templates object and fails. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.Transformer newTransformer(javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
	  public virtual Transformer newTransformer(Source source)
	  {
		try
		{
		  Templates tmpl = newTemplates(source);
		  /* this can happen if an ErrorListener is present and it doesn't
		     throw any exception in fatalError. 
		     The spec says: "a Transformer must use this interface
		     instead of throwing an exception" - the newTemplates() does
		     that, and returns null.
		  */
		  if (tmpl == null)
		  {
			  return null;
		  }
		  Transformer transformer = tmpl.newTransformer();
		  transformer.URIResolver = m_uriResolver;
		  return transformer;
		}
		catch (TransformerConfigurationException ex)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(ex);
			  return null;
			}
			catch (TransformerConfigurationException ex1)
			{
			  throw ex1;
			}
			catch (TransformerException ex1)
			{
			  throw new TransformerConfigurationException(ex1);
			}
		  }
		  throw ex;
		}
	  }

	  /// <summary>
	  /// Create a new Transformer object that performs a copy
	  /// of the source to the result.
	  /// </summary>
	  /// <returns> A Transformer object capable of
	  /// being used for transformation purposes in a single thread.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> May throw this during
	  ///            the parse when it is constructing the
	  ///            Templates object and it fails. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.Transformer newTransformer() throws javax.xml.transform.TransformerConfigurationException
	  public virtual Transformer newTransformer()
	  {
		  return new TransformerIdentityImpl(m_isSecureProcessing);
	  }

	  /// <summary>
	  /// Process the source into a Templates object, which is likely
	  /// a compiled representation of the source. This Templates object
	  /// may then be used concurrently across multiple threads.  Creating
	  /// a Templates object allows the TransformerFactory to do detailed
	  /// performance optimization of transformation instructions, without
	  /// penalizing runtime transformation.
	  /// </summary>
	  /// <param name="source"> An object that holds a URL, input stream, etc. </param>
	  /// <returns> A Templates object capable of being used for transformation purposes.
	  /// </returns>
	  /// <exception cref="TransformerConfigurationException"> May throw this during the parse when it
	  ///            is constructing the Templates object and fails. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.Templates newTemplates(javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
	  public virtual Templates newTemplates(Source source)
	  {

		string baseID = source.SystemId;

		if (null != baseID)
		{
		   baseID = SystemIDResolver.getAbsoluteURI(baseID);
		}


		if (source is DOMSource)
		{
		  DOMSource dsource = (DOMSource) source;
		  Node node = dsource.Node;

		  if (null != node)
		  {
			return processFromNode(node, baseID);
		  }
		  else
		  {
			string messageStr = XSLMessages.createMessage(XSLTErrorResources.ER_ILLEGAL_DOMSOURCE_INPUT, null);

			throw new System.ArgumentException(messageStr);
		  }
		}

		TemplatesHandler builder = newTemplatesHandler();
		builder.SystemId = baseID;

		try
		{
		  InputSource isource = SAXSource.sourceToInputSource(source);
		  isource.SystemId = baseID;
		  XMLReader reader = null;

		  if (source is SAXSource)
		  {
			reader = ((SAXSource) source).XMLReader;
		  }

		  if (null == reader)
		  {

			// Use JAXP1.1 ( if possible )
			try
			{
			  javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();

			  factory.NamespaceAware = true;

			  if (m_isSecureProcessing)
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

		  // If you set the namespaces to true, we'll end up getting double 
		  // xmlns attributes.  Needs to be fixed.  -sb
		  // reader.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
		  reader.ContentHandler = builder;
		  reader.parse(isource);
		}
		catch (org.xml.sax.SAXException se)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(new TransformerException(se));
			}
			catch (TransformerConfigurationException ex1)
			{
			  throw ex1;
			}
			catch (TransformerException ex1)
			{
			  throw new TransformerConfigurationException(ex1);
			}
		  }
		  else
		  {
			throw new TransformerConfigurationException(se.Message, se);
		  }
		}
		catch (Exception e)
		{
		  if (m_errorListener != null)
		  {
			try
			{
			  m_errorListener.fatalError(new TransformerException(e));
			  return null;
			}
			catch (TransformerConfigurationException ex1)
			{
			  throw ex1;
			}
			catch (TransformerException ex1)
			{
			  throw new TransformerConfigurationException(ex1);
			}
		  }
		  else
		  {
			throw new TransformerConfigurationException(e.Message, e);
		  }
		}

		return builder.Templates;
	  }

	  /// <summary>
	  /// The object that implements the URIResolver interface,
	  /// or null.
	  /// </summary>
	  internal URIResolver m_uriResolver;

	  /// <summary>
	  /// Set an object that will be used to resolve URIs used in
	  /// xsl:import, etc.  This will be used as the default for the
	  /// transformation. </summary>
	  /// <param name="resolver"> An object that implements the URIResolver interface,
	  /// or null. </param>
	  public virtual URIResolver URIResolver
	  {
		  set
		  {
			m_uriResolver = value;
		  }
		  get
		  {
			return m_uriResolver;
		  }
	  }


	  /// <summary>
	  /// The error listener. </summary>
	  private ErrorListener m_errorListener = new org.apache.xml.utils.DefaultErrorHandler(false);

	  /// <summary>
	  /// Get the error listener in effect for the TransformerFactory.
	  /// </summary>
	  /// <returns> A non-null reference to an error listener. </returns>
	  public virtual ErrorListener ErrorListener
	  {
		  get
		  {
			return m_errorListener;
		  }
		  set
		  {
    
			if (null == value)
			{
			  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_ERRORLISTENER, null));
			}
			  // "ErrorListener");
    
			m_errorListener = value;
		  }
	  }


	  /// <summary>
	  /// Return the state of the secure processing feature.
	  /// </summary>
	  /// <returns> state of the secure processing feature. </returns>
	  public virtual bool SecureProcessing
	  {
		  get
		  {
			return m_isSecureProcessing;
		  }
	  }
	}

}