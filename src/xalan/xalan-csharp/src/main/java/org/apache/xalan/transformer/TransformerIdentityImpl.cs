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
 * $Id: TransformerIdentityImpl.java 575747 2007-09-14 16:28:37Z kcormier $
 */
namespace org.apache.xalan.transformer
{



	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using OutputProperties = org.apache.xalan.templates.OutputProperties;
	using Serializer = org.apache.xml.serializer.Serializer;
	using SerializerFactory = org.apache.xml.serializer.SerializerFactory;
	using Method = org.apache.xml.serializer.Method;
	using DOMBuilder = org.apache.xml.utils.DOMBuilder;
	using XMLReaderManager = org.apache.xml.utils.XMLReaderManager;

	using Document = org.w3c.dom.Document;
	using DocumentFragment = org.w3c.dom.DocumentFragment;
	using Node = org.w3c.dom.Node;

	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using XMLReader = org.xml.sax.XMLReader;
	using DeclHandler = org.xml.sax.ext.DeclHandler;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// This class implements an identity transformer for
	/// <seealso cref="javax.xml.transform.sax.SAXTransformerFactory#newTransformerHandler()"/>
	/// and <seealso cref="javax.xml.transform.TransformerFactory#newTransformer()"/>.  It
	/// simply feeds SAX events directly to a serializer ContentHandler, if the
	/// result is a stream.  If the result is a DOM, it will send the events to
	/// <seealso cref="org.apache.xml.utils.DOMBuilder"/>.  If the result is another
	/// content handler, it will simply pass the events on.
	/// </summary>
	public class TransformerIdentityImpl : Transformer, TransformerHandler, DeclHandler
	{

	  /// <summary>
	  /// Constructor TransformerIdentityImpl creates an identity transform.
	  /// 
	  /// </summary>
	  public TransformerIdentityImpl(bool isSecureProcessing)
	  {
		m_outputFormat = new OutputProperties(Method.XML);
		m_isSecureProcessing = isSecureProcessing;
	  }

	  /// <summary>
	  /// Constructor TransformerIdentityImpl creates an identity transform.
	  /// 
	  /// </summary>
	  public TransformerIdentityImpl() : this(false)
	  {
	  }

	  /// <summary>
	  /// Enables the user of the TransformerHandler to set the
	  /// to set the Result for the transformation.
	  /// </summary>
	  /// <param name="result"> A Result instance, should not be null.
	  /// </param>
	  /// <exception cref="IllegalArgumentException"> if result is invalid for some reason. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setResult(javax.xml.transform.Result result) throws IllegalArgumentException
	  public virtual Result Result
	  {
		  set
		  {
			if (null == value)
			{
			  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_RESULT_NULL, null)); //"Result should not be null");
			}
			m_result = value;
		  }
	  }

	  /// <summary>
	  /// Set the base ID (URI or system ID) from where relative
	  /// URLs will be resolved. </summary>
	  /// <param name="systemID"> Base URI for the source tree. </param>
	  public virtual string SystemId
	  {
		  set
		  {
			m_systemID = value;
		  }
		  get
		  {
			return m_systemID;
		  }
	  }


	  /// <summary>
	  /// Get the Transformer associated with this handler, which
	  /// is needed in order to set parameters and output properties.
	  /// </summary>
	  /// <returns> non-null reference to the transformer. </returns>
	  public virtual Transformer Transformer
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Reset the status of the transformer.
	  /// </summary>
	  public virtual void reset()
	  {
		m_flushedStartDoc = false;
		m_foundFirstElement = false;
		m_outputStream = null;
		clearParameters();
		m_result = null;
		m_resultContentHandler = null;
		m_resultDeclHandler = null;
		m_resultDTDHandler = null;
		m_resultLexicalHandler = null;
		m_serializer = null;
		m_systemID = null;
		m_URIResolver = null;
		m_outputFormat = new OutputProperties(Method.XML);
	  }

	  /// <summary>
	  /// Create a result ContentHandler from a Result object, based
	  /// on the current OutputProperties.
	  /// </summary>
	  /// <param name="outputTarget"> Where the transform result should go,
	  /// should not be null.
	  /// </param>
	  /// <returns> A valid ContentHandler that will create the
	  /// result tree when it is fed SAX events.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void createResultContentHandler(javax.xml.transform.Result outputTarget) throws javax.xml.transform.TransformerException
	  private void createResultContentHandler(Result outputTarget)
	  {

		if (outputTarget is SAXResult)
		{
		  SAXResult saxResult = (SAXResult) outputTarget;

		  m_resultContentHandler = saxResult.Handler;
		  m_resultLexicalHandler = saxResult.LexicalHandler;

		  if (m_resultContentHandler is Serializer)
		  {

			// Dubious but needed, I think.
			m_serializer = (Serializer) m_resultContentHandler;
		  }
		}
		else if (outputTarget is DOMResult)
		{
		  DOMResult domResult = (DOMResult) outputTarget;
		  Node outputNode = domResult.Node;
		  Node nextSibling = domResult.NextSibling;
		  Document doc;
		  short type;

		  if (null != outputNode)
		  {
			type = outputNode.NodeType;
			doc = (Node.DOCUMENT_NODE == type) ? (Document) outputNode : outputNode.OwnerDocument;
		  }
		  else
		  {
			try
			{
			  DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();

			  dbf.NamespaceAware = true;

			  if (m_isSecureProcessing)
			  {
				try
				{
				  dbf.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
				}
				catch (ParserConfigurationException)
				{
				}
			  }

			  DocumentBuilder db = dbf.newDocumentBuilder();

			  doc = db.newDocument();
			}
			catch (ParserConfigurationException pce)
			{
			  throw new TransformerException(pce);
			}

			outputNode = doc;
			type = outputNode.NodeType;

			((DOMResult) outputTarget).Node = outputNode;
		  }

		  DOMBuilder domBuilder = (Node.DOCUMENT_FRAGMENT_NODE == type) ? new DOMBuilder(doc, (DocumentFragment) outputNode) : new DOMBuilder(doc, outputNode);

		  if (nextSibling != null)
		  {
			domBuilder.NextSibling = nextSibling;
		  }

		  m_resultContentHandler = domBuilder;
		  m_resultLexicalHandler = domBuilder;
		}
		else if (outputTarget is StreamResult)
		{
		  StreamResult sresult = (StreamResult) outputTarget;

		  try
		  {
			Serializer serializer = SerializerFactory.getSerializer(m_outputFormat.Properties);

			m_serializer = serializer;

			if (null != sresult.Writer)
			{
			  serializer.Writer = sresult.Writer;
			}
			else if (null != sresult.OutputStream)
			{
			  serializer.OutputStream = sresult.OutputStream;
			}
			else if (null != sresult.SystemId)
			{
			  string fileURL = sresult.SystemId;

			  if (fileURL.StartsWith("file:///", StringComparison.Ordinal))
			  {
				if (fileURL.Substring(8).IndexOf(":", StringComparison.Ordinal) > 0)
				{
				  fileURL = fileURL.Substring(8);
				}
				else
				{
				  fileURL = fileURL.Substring(7);
				}
			  }
			  else if (fileURL.StartsWith("file:/", StringComparison.Ordinal))
			  {
				if (fileURL.Substring(6).IndexOf(":", StringComparison.Ordinal) > 0)
				{
				  fileURL = fileURL.Substring(6);
				}
				else
				{
				  fileURL = fileURL.Substring(5);
				}
			  }

			  m_outputStream = new System.IO.FileStream(fileURL, System.IO.FileMode.Create, System.IO.FileAccess.Write);
			  serializer.OutputStream = m_outputStream;
			}
			else
			{
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_OUTPUT_SPECIFIED, null)); //"No output specified!");
			}

			m_resultContentHandler = serializer.asContentHandler();
		  }
		  catch (IOException ioe)
		  {
			throw new TransformerException(ioe);
		  }
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, new object[]{outputTarget.GetType().FullName})); //"Can't transform to a Result of type "
										// + outputTarget.getClass().getName()
										// + "!");
		}

		if (m_resultContentHandler is DTDHandler)
		{
		  m_resultDTDHandler = (DTDHandler) m_resultContentHandler;
		}

		if (m_resultContentHandler is DeclHandler)
		{
		  m_resultDeclHandler = (DeclHandler) m_resultContentHandler;
		}

		if (m_resultContentHandler is LexicalHandler)
		{
		  m_resultLexicalHandler = (LexicalHandler) m_resultContentHandler;
		}
	  }

	  /// <summary>
	  /// Process the source tree to the output result. </summary>
	  /// <param name="source">  The input for the source tree.
	  /// </param>
	  /// <param name="outputTarget"> The output target.
	  /// </param>
	  /// <exception cref="TransformerException"> If an unrecoverable error occurs
	  /// during the course of the transformation. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(javax.xml.transform.Source source, javax.xml.transform.Result outputTarget) throws javax.xml.transform.TransformerException
	  public virtual void transform(Source source, Result outputTarget)
	  {

		createResultContentHandler(outputTarget);

		/*
		 * According to JAXP1.2, new SAXSource()/StreamSource()
		 * should create an empty input tree, with a default root node. 
		 * new DOMSource()creates an empty document using DocumentBuilder.
		 * newDocument(); Use DocumentBuilder.newDocument() for all 3 situations,
		 * since there is no clear spec. how to create an empty tree when
		 * both SAXSource() and StreamSource() are used.
		 */
		if ((source is StreamSource && source.SystemId == null && ((StreamSource)source).InputStream == null && ((StreamSource)source).Reader == null) || (source is SAXSource && ((SAXSource)source).InputSource == null && ((SAXSource)source).XMLReader == null) || (source is DOMSource && ((DOMSource)source).Node == null))
		{
		  try
		  {
			DocumentBuilderFactory builderF = DocumentBuilderFactory.newInstance();
			DocumentBuilder builder = builderF.newDocumentBuilder();
			string systemID = source.SystemId;
			source = new DOMSource(builder.newDocument());

			// Copy system ID from original, empty Source to new Source
			if (!string.ReferenceEquals(systemID, null))
			{
			  source.SystemId = systemID;
			}
		  }
		  catch (ParserConfigurationException e)
		  {
			throw new TransformerException(e.Message);
		  }
		}

		try
		{
		  if (source is DOMSource)
		  {
			DOMSource dsource = (DOMSource) source;

			m_systemID = dsource.SystemId;

			Node dNode = dsource.Node;

			if (null != dNode)
			{
			  try
			  {
				if (dNode.NodeType == Node.ATTRIBUTE_NODE)
				{
				  this.startDocument();
				}
				try
				{
				  if (dNode.NodeType == Node.ATTRIBUTE_NODE)
				  {
					string data = dNode.NodeValue;
					char[] chars = data.ToCharArray();
					characters(chars, 0, chars.Length);
				  }
				  else
				  {
					org.apache.xml.serializer.TreeWalker walker;
					walker = new org.apache.xml.serializer.TreeWalker(this, m_systemID);
					walker.traverse(dNode);
				  }
				}
				finally
				{
				  if (dNode.NodeType == Node.ATTRIBUTE_NODE)
				  {
					this.endDocument();
				  }
				}
			  }
			  catch (SAXException se)
			  {
				throw new TransformerException(se);
			  }

			  return;
			}
			else
			{
			  string messageStr = XSLMessages.createMessage(XSLTErrorResources.ER_ILLEGAL_DOMSOURCE_INPUT, null);

			  throw new System.ArgumentException(messageStr);
			}
		  }

		  InputSource xmlSource = SAXSource.sourceToInputSource(source);

		  if (null == xmlSource)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_TRANSFORM_SOURCE_TYPE, new object[]{source.GetType().FullName})); //"Can't transform a Source of type "
										   //+ source.getClass().getName() + "!");
		  }

		  if (null != xmlSource.SystemId)
		  {
			m_systemID = xmlSource.SystemId;
		  }

		  XMLReader reader = null;
		  bool managedReader = false;

		  try
		  {
			if (source is SAXSource)
			{
			  reader = ((SAXSource) source).XMLReader;
			}

			if (null == reader)
			{
			  try
			  {
				reader = XMLReaderManager.Instance.XMLReader;
				managedReader = true;
			  }
			  catch (SAXException se)
			  {
				throw new TransformerException(se);
			  }
			}
			else
			{
			  try
			  {
				reader.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
			  }
			  catch (SAXException)
			  {
				// We don't care.
			  }
			}

			// Get the input content handler, which will handle the 
			// parse events and create the source tree. 
			ContentHandler inputHandler = this;

			reader.ContentHandler = inputHandler;

			if (inputHandler is DTDHandler)
			{
			  reader.DTDHandler = (DTDHandler) inputHandler;
			}

			try
			{
			  if (inputHandler is LexicalHandler)
			  {
				reader.setProperty("http://xml.org/sax/properties/lexical-handler", inputHandler);
			  }

			  if (inputHandler is DeclHandler)
			  {
				reader.setProperty("http://xml.org/sax/properties/declaration-handler", inputHandler);
			  }
			}
			catch (SAXException)
			{
			}

			try
			{
			  if (inputHandler is LexicalHandler)
			  {
				reader.setProperty("http://xml.org/sax/handlers/LexicalHandler", inputHandler);
			  }

			  if (inputHandler is DeclHandler)
			  {
				reader.setProperty("http://xml.org/sax/handlers/DeclHandler", inputHandler);
			  }
			}
			catch (org.xml.sax.SAXNotRecognizedException)
			{
			}

			reader.parse(xmlSource);
		  }
		  catch (org.apache.xml.utils.WrappedRuntimeException wre)
		  {
			Exception throwable = wre.Exception;

			while (throwable is org.apache.xml.utils.WrappedRuntimeException)
			{
			  throwable = ((org.apache.xml.utils.WrappedRuntimeException) throwable).Exception;
			}

			throw new TransformerException(wre.Exception);
		  }
		  catch (SAXException se)
		  {
			throw new TransformerException(se);
		  }
		  catch (IOException ioe)
		  {
			throw new TransformerException(ioe);
		  }
		  finally
		  {
			if (managedReader)
			{
			  XMLReaderManager.Instance.releaseXMLReader(reader);
			}
		  }
		}
		finally
		{
		  if (null != m_outputStream)
		  {
			try
			{
			  m_outputStream.Close();
			}
			catch (IOException)
			{
			}
			m_outputStream = null;
		  }
		}
	  }

	  /// <summary>
	  /// Add a parameter for the transformation.
	  /// 
	  /// <para>Pass a qualified name as a two-part string, the namespace URI
	  /// enclosed in curly braces ({}), followed by the local name. If the
	  /// name has a null URL, the String only contain the local name. An
	  /// application can safely check for a non-null URI by testing to see if the first
	  /// character of the name is a '{' character.</para>
	  /// <para>For example, if a URI and local name were obtained from an element
	  /// defined with &lt;xyz:foo xmlns:xyz="http://xyz.foo.com/yada/baz.html"/&gt;,
	  /// then the qualified name would be "{http://xyz.foo.com/yada/baz.html}foo". Note that
	  /// no prefix is used.</para>
	  /// </summary>
	  /// <param name="name"> The name of the parameter, which may begin with a namespace URI
	  /// in curly braces ({}). </param>
	  /// <param name="value"> The value object.  This can be any valid Java object. It is
	  /// up to the processor to provide the proper object coersion or to simply
	  /// pass the object on for use in an extension. </param>
	  public virtual void setParameter(string name, object value)
	  {
		if (value == null)
		{
		  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_SET_PARAM_VALUE, new object[]{name}));
		}

		if (null == m_params)
		{
		  m_params = new Hashtable();
		}

		m_params[name] = value;
	  }

	  /// <summary>
	  /// Get a parameter that was explicitly set with setParameter
	  /// or setParameters.
	  /// 
	  /// <para>This method does not return a default parameter value, which
	  /// cannot be determined until the node context is evaluated during
	  /// the transformation process.
	  /// 
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="name"> Name of the parameter. </param>
	  /// <returns> A parameter that has been set with setParameter. </returns>
	  public virtual object getParameter(string name)
	  {

		if (null == m_params)
		{
		  return null;
		}

		return m_params[name];
	  }

	  /// <summary>
	  /// Clear all parameters set with setParameter.
	  /// </summary>
	  public virtual void clearParameters()
	  {

		if (null == m_params)
		{
		  return;
		}

		m_params.Clear();
	  }

	  /// <summary>
	  /// Set an object that will be used to resolve URIs used in
	  /// document().
	  /// 
	  /// <para>If the resolver argument is null, the URIResolver value will
	  /// be cleared, and the default behavior will be used.</para>
	  /// </summary>
	  /// <param name="resolver"> An object that implements the URIResolver interface,
	  /// or null. </param>
	  public virtual URIResolver URIResolver
	  {
		  set
		  {
			m_URIResolver = value;
		  }
		  get
		  {
			return m_URIResolver;
		  }
	  }


	  /// <summary>
	  /// Set the output properties for the transformation.  These
	  /// properties will override properties set in the Templates
	  /// with xsl:output.
	  /// 
	  /// <para>If argument to this function is null, any properties
	  /// previously set are removed, and the value will revert to the value
	  /// defined in the templates object.</para>
	  /// 
	  /// <para>Pass a qualified property key name as a two-part string, the namespace URI
	  /// enclosed in curly braces ({}), followed by the local name. If the
	  /// name has a null URL, the String only contain the local name. An
	  /// application can safely check for a non-null URI by testing to see if the first
	  /// character of the name is a '{' character.</para>
	  /// <para>For example, if a URI and local name were obtained from an element
	  /// defined with &lt;xyz:foo xmlns:xyz="http://xyz.foo.com/yada/baz.html"/&gt;,
	  /// then the qualified name would be "{http://xyz.foo.com/yada/baz.html}foo". Note that
	  /// no prefix is used.</para>
	  /// </summary>
	  /// <param name="oformat"> A set of output properties that will be
	  /// used to override any of the same properties in affect
	  /// for the transformation.
	  /// </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  /// <seealso cref= java.util.Properties
	  /// </seealso>
	  /// <exception cref="IllegalArgumentException"> if any of the argument keys are not
	  /// recognized and are not namespace qualified. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOutputProperties(java.util.Properties oformat) throws IllegalArgumentException
	  public virtual Properties OutputProperties
	  {
		  set
		  {
    
			if (null != value)
			{
    
			  // See if an *explicit* method was set.
			  string method = (string) value.get(OutputKeys.METHOD);
    
			  if (null != method)
			  {
				m_outputFormat = new OutputProperties(method);
			  }
			  else
			  {
				m_outputFormat = new OutputProperties();
			  }
    
			  m_outputFormat.copyFrom(value);
			}
			else
			{
			  // if value is null JAXP says that any props previously set are removed
			  // and we are to revert back to those in the templates object (i.e. Stylesheet).
			  m_outputFormat = null;
			}
		  }
		  get
		  {
			return (Properties) m_outputFormat.Properties.clone();
		  }
	  }


	  /// <summary>
	  /// Set an output property that will be in effect for the
	  /// transformation.
	  /// 
	  /// <para>Pass a qualified property name as a two-part string, the namespace URI
	  /// enclosed in curly braces ({}), followed by the local name. If the
	  /// name has a null URL, the String only contain the local name. An
	  /// application can safely check for a non-null URI by testing to see if the first
	  /// character of the name is a '{' character.</para>
	  /// <para>For example, if a URI and local name were obtained from an element
	  /// defined with &lt;xyz:foo xmlns:xyz="http://xyz.foo.com/yada/baz.html"/&gt;,
	  /// then the qualified name would be "{http://xyz.foo.com/yada/baz.html}foo". Note that
	  /// no prefix is used.</para>
	  /// 
	  /// <para>The Properties object that was passed to <seealso cref="#setOutputProperties"/> won't
	  /// be effected by calling this method.</para>
	  /// </summary>
	  /// <param name="name"> A non-null String that specifies an output
	  /// property name, which may be namespace qualified. </param>
	  /// <param name="value"> The non-null string value of the output property.
	  /// </param>
	  /// <exception cref="IllegalArgumentException"> If the property is not supported, and is
	  /// not qualified with a namespace.
	  /// </exception>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOutputProperty(String name, String value) throws IllegalArgumentException
	  public virtual void setOutputProperty(string name, string value)
	  {

		if (!OutputProperties.isLegalPropertyKey(name))
		{
		  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, new object[]{name})); //"output property not recognized: "
		}
											 //+ name);

		m_outputFormat.setProperty(name, value);
	  }

	  /// <summary>
	  /// Get an output property that is in effect for the
	  /// transformation.  The property specified may be a property
	  /// that was set with setOutputProperty, or it may be a
	  /// property specified in the stylesheet.
	  /// </summary>
	  /// <param name="name"> A non-null String that specifies an output
	  /// property name, which may be namespace qualified.
	  /// </param>
	  /// <returns> The string value of the output property, or null
	  /// if no property was found.
	  /// </returns>
	  /// <exception cref="IllegalArgumentException"> If the property is not supported.
	  /// </exception>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOutputProperty(String name) throws IllegalArgumentException
	  public virtual string getOutputProperty(string name)
	  {

		string value = null;
		OutputProperties props = m_outputFormat;

		value = props.getProperty(name);

		if (null == value)
		{
		  if (!OutputProperties.isLegalPropertyKey(name))
		  {
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, new object[]{name})); //"output property not recognized: "
		  }
											  // + name);
		}

		return value;
	  }

	  /// <summary>
	  /// Set the error event listener in effect for the transformation.
	  /// </summary>
	  /// <param name="listener"> The new error listener. </param>
	  /// <exception cref="IllegalArgumentException"> if listener is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setErrorListener(javax.xml.transform.ErrorListener listener) throws IllegalArgumentException
	  public virtual ErrorListener ErrorListener
	  {
		  set
		  {
			  if (value == null)
			  {
				throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_NULL_ERROR_HANDLER, null));
			  }
			  else
			  {
				m_errorListener = value;
			  }
		  }
		  get
		  {
			return m_errorListener;
		  }
	  }


	  ////////////////////////////////////////////////////////////////////
	  // Default implementation of DTDHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive notification of a notation declaration.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass if they wish to keep track of the notations
	  /// declared in a document.</para>
	  /// </summary>
	  /// <param name="name"> The notation name. </param>
	  /// <param name="publicId"> The notation public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The notation system identifier. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.DTDHandler#notationDecl
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notationDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
	  public virtual void notationDecl(string name, string publicId, string systemId)
	  {
		if (null != m_resultDTDHandler)
		{
		  m_resultDTDHandler.notationDecl(name, publicId, systemId);
		}
	  }

	  /// <summary>
	  /// Receive notification of an unparsed entity declaration.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to keep track of the unparsed entities
	  /// declared in a document.</para>
	  /// </summary>
	  /// <param name="name"> The entity name. </param>
	  /// <param name="publicId"> The entity public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The entity system identifier. </param>
	  /// <param name="notationName"> The name of the associated notation. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unparsedEntityDecl(String name, String publicId, String systemId, String notationName) throws org.xml.sax.SAXException
	  public virtual void unparsedEntityDecl(string name, string publicId, string systemId, string notationName)
	  {

		if (null != m_resultDTDHandler)
		{
		  m_resultDTDHandler.unparsedEntityDecl(name, publicId, systemId, notationName);
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Default implementation of ContentHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive a Locator object for document events.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass if they wish to store the locator for use
	  /// with other document events.</para>
	  /// </summary>
	  /// <param name="locator"> A locator for all SAX document events. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator </seealso>
	  /// <seealso cref= org.xml.sax.Locator </seealso>
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
			try
			{
			  if (null == m_resultContentHandler)
			  {
				createResultContentHandler(m_result);
			  }
			}
			catch (TransformerException te)
			{
			  throw new org.apache.xml.utils.WrappedRuntimeException(te);
			}
    
			m_resultContentHandler.DocumentLocator = value;
		  }
	  }

	  /// <summary>
	  /// Receive notification of the beginning of the document.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the beginning
	  /// of a document (such as allocating the root node of a tree or
	  /// creating an output file).</para>
	  /// </summary>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#startDocument
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
	  public virtual void startDocument()
	  {

		try
		{
		  if (null == m_resultContentHandler)
		  {
			createResultContentHandler(m_result);
		  }
		}
		catch (TransformerException te)
		{
		  throw new SAXException(te.Message, te);
		}

		// Reset for multiple transforms with this transformer.
		m_flushedStartDoc = false;
		m_foundFirstElement = false;
	  }

	  internal bool m_flushedStartDoc = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected final void flushStartDoc() throws org.xml.sax.SAXException
	  protected internal void flushStartDoc()
	  {
		if (!m_flushedStartDoc)
		{
		  if (m_resultContentHandler == null)
		  {
			try
			{
			  createResultContentHandler(m_result);
			}
			catch (TransformerException te)
			{
				throw new SAXException(te);
			}
		  }
		  m_resultContentHandler.startDocument();
		  m_flushedStartDoc = true;
		}
	  }

	  /// <summary>
	  /// Receive notification of the end of the document.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the end
	  /// of a document (such as finalising a tree or closing an output
	  /// file).</para>
	  /// </summary>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#endDocument
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
	  public virtual void endDocument()
	  {
		flushStartDoc();
		m_resultContentHandler.endDocument();
	  }

	  /// <summary>
	  /// Receive notification of the start of a Namespace mapping.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each Namespace prefix scope (such as storing the prefix mapping).</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <param name="uri"> The Namespace URI mapped to the prefix. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
	  public virtual void startPrefixMapping(string prefix, string uri)
	  {
		flushStartDoc();
		m_resultContentHandler.startPrefixMapping(prefix, uri);
	  }

	  /// <summary>
	  /// Receive notification of the end of a Namespace mapping.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the end of
	  /// each prefix mapping.</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
	  public virtual void endPrefixMapping(string prefix)
	  {
		flushStartDoc();
		m_resultContentHandler.endPrefixMapping(prefix);
	  }

	  /// <summary>
	  /// Receive notification of the start of an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each element (such as allocating a new tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="qName"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  /// <param name="attributes"> The specified or defaulted attributes. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#startElement
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public virtual void startElement(string uri, string localName, string qName, Attributes attributes)
	  {

		if (!m_foundFirstElement && null != m_serializer)
		{
		  m_foundFirstElement = true;

		  Serializer newSerializer;

		  try
		  {
			newSerializer = SerializerSwitcher.switchSerializerIfHTML(uri, localName, m_outputFormat.Properties, m_serializer);
		  }
		  catch (TransformerException te)
		  {
			throw new SAXException(te);
		  }

		  if (newSerializer != m_serializer)
		  {
			try
			{
			  m_resultContentHandler = newSerializer.asContentHandler();
			}
			catch (IOException ioe) // why?
			{
			  throw new SAXException(ioe);
			}

			if (m_resultContentHandler is DTDHandler)
			{
			  m_resultDTDHandler = (DTDHandler) m_resultContentHandler;
			}

			if (m_resultContentHandler is LexicalHandler)
			{
			  m_resultLexicalHandler = (LexicalHandler) m_resultContentHandler;
			}

			m_serializer = newSerializer;
		  }
		}
		flushStartDoc();
		m_resultContentHandler.startElement(uri, localName, qName, attributes);
	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the end of
	  /// each element (such as finalising a tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI or if Namespace
	  ///        processing is not being performed. </param>
	  /// <param name="localName"> The local name (without prefix), or the
	  ///        empty string if Namespace processing is not being
	  ///        performed. </param>
	  /// <param name="qName"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#endElement
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
	  public virtual void endElement(string uri, string localName, string qName)
	  {
		m_resultContentHandler.endElement(uri, localName, qName);
	  }

	  /// <summary>
	  /// Receive notification of character data inside an element.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method to take specific actions for each chunk of character data
	  /// (such as adding the data to a node or buffer, or printing it to
	  /// a file).</para>
	  /// </summary>
	  /// <param name="ch"> The characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#characters
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {
		flushStartDoc();
		m_resultContentHandler.characters(ch, start, length);
	  }

	  /// <summary>
	  /// Receive notification of ignorable whitespace in element content.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method to take specific actions for each chunk of ignorable
	  /// whitespace (such as adding data to a node or buffer, or printing
	  /// it to a file).</para>
	  /// </summary>
	  /// <param name="ch"> The whitespace characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void ignorableWhitespace(char[] ch, int start, int length)
	  {
		m_resultContentHandler.ignorableWhitespace(ch, start, length);
	  }

	  /// <summary>
	  /// Receive notification of a processing instruction.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions for each
	  /// processing instruction, such as setting status variables or
	  /// invoking other methods.</para>
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///             none is supplied. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#processingInstruction
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {
		flushStartDoc();
		m_resultContentHandler.processingInstruction(target, data);
	  }

	  /// <summary>
	  /// Receive notification of a skipped entity.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions for each
	  /// processing instruction, such as setting status variables or
	  /// invoking other methods.</para>
	  /// </summary>
	  /// <param name="name"> The name of the skipped entity. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#processingInstruction
	  /// </seealso>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
	  public virtual void skippedEntity(string name)
	  {
		flushStartDoc();
		m_resultContentHandler.skippedEntity(name);
	  }

	  /// <summary>
	  /// Report the start of DTD declarations, if any.
	  /// 
	  /// <para>Any declarations are assumed to be in the internal subset
	  /// unless otherwise indicated by a <seealso cref="#startEntity startEntity"/>
	  /// event.</para>
	  /// 
	  /// <para>Note that the start/endDTD events will appear within
	  /// the start/endDocument events from ContentHandler and
	  /// before the first startElement event.</para>
	  /// </summary>
	  /// <param name="name"> The document type name. </param>
	  /// <param name="publicId"> The declared public identifier for the
	  ///        external DTD subset, or null if none was declared. </param>
	  /// <param name="systemId"> The declared system identifier for the
	  ///        external DTD subset, or null if none was declared. </param>
	  /// <exception cref="SAXException"> The application may raise an
	  ///            exception. </exception>
	  /// <seealso cref= #endDTD </seealso>
	  /// <seealso cref= #startEntity </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
	  public virtual void startDTD(string name, string publicId, string systemId)
	  {
		flushStartDoc();
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.startDTD(name, publicId, systemId);
		}
	  }

	  /// <summary>
	  /// Report the end of DTD declarations.
	  /// </summary>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref= #startDTD </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
	  public virtual void endDTD()
	  {
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.endDTD();
		}
	  }

	  /// <summary>
	  /// Report the beginning of an entity in content.
	  /// 
	  /// <para><strong>NOTE:</entity> entity references in attribute
	  /// values -- and the start and end of the document entity --
	  /// are never reported.</para>
	  /// 
	  /// <para>The start and end of the external DTD subset are reported
	  /// using the pseudo-name "[dtd]".  All other events must be
	  /// properly nested within start/end entity events.</para>
	  /// 
	  /// <para>Note that skipped entities will be reported through the
	  /// <seealso cref="org.xml.sax.ContentHandler#skippedEntity skippedEntity"/>
	  /// event, which is part of the ContentHandler interface.</para>
	  /// </summary>
	  /// <param name="name"> The name of the entity.  If it is a parameter
	  ///        entity, the name will begin with '%'. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref= #endEntity </seealso>
	  /// <seealso cref= org.xml.sax.ext.DeclHandler#internalEntityDecl </seealso>
	  /// <seealso cref= org.xml.sax.ext.DeclHandler#externalEntityDecl </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startEntity(String name) throws org.xml.sax.SAXException
	  public virtual void startEntity(string name)
	  {
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.startEntity(name);
		}
	  }

	  /// <summary>
	  /// Report the end of an entity.
	  /// </summary>
	  /// <param name="name"> The name of the entity that is ending. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref= #startEntity </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endEntity(String name) throws org.xml.sax.SAXException
	  public virtual void endEntity(string name)
	  {
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.endEntity(name);
		}
	  }

	  /// <summary>
	  /// Report the start of a CDATA section.
	  /// 
	  /// <para>The contents of the CDATA section will be reported through
	  /// the regular {@link org.xml.sax.ContentHandler#characters
	  /// characters} event.</para>
	  /// </summary>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref= #endCDATA </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
	  public virtual void startCDATA()
	  {
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.startCDATA();
		}
	  }

	  /// <summary>
	  /// Report the end of a CDATA section.
	  /// </summary>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
	  /// <seealso cref= #startCDATA </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
	  public virtual void endCDATA()
	  {
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.endCDATA();
		}
	  }

	  /// <summary>
	  /// Report an XML comment anywhere in the document.
	  /// 
	  /// <para>This callback will be used for comments inside or outside the
	  /// document element, including comments in the external DTD
	  /// subset (if read).</para>
	  /// </summary>
	  /// <param name="ch"> An array holding the characters in the comment. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void comment(char[] ch, int start, int length)
	  {
		flushStartDoc();
		if (null != m_resultLexicalHandler)
		{
		  m_resultLexicalHandler.comment(ch, start, length);
		}
	  }

	  // Implement DeclHandler

	  /// <summary>
	  /// Report an element type declaration.
	  ///   
	  /// <para>The content model will consist of the string "EMPTY", the
	  /// string "ANY", or a parenthesised group, optionally followed
	  /// by an occurrence indicator.  The model will be normalized so
	  /// that all whitespace is removed,and will include the enclosing
	  /// parentheses.</para>
	  /// </summary>
	  /// <param name="name"> The element type name. </param>
	  /// <param name="model"> The content model as a normalized string. </param>
	  /// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void elementDecl(String name, String model) throws org.xml.sax.SAXException
		public virtual void elementDecl(string name, string model)
		{
							if (null != m_resultDeclHandler)
							{
									m_resultDeclHandler.elementDecl(name, model);
							}
		}


		/// <summary>
		/// Report an attribute type declaration.
		/// 
		/// <para>Only the effective (first) declaration for an attribute will
		/// be reported.  The type will be one of the strings "CDATA",
		/// "ID", "IDREF", "IDREFS", "NMTOKEN", "NMTOKENS", "ENTITY",
		/// "ENTITIES", or "NOTATION", or a parenthesized token group with 
		/// the separator "|" and all whitespace removed.</para>
		/// </summary>
		/// <param name="eName"> The name of the associated element. </param>
		/// <param name="aName"> The name of the attribute. </param>
		/// <param name="type"> A string representing the attribute type. </param>
		/// <param name="valueDefault"> A string representing the attribute default
		///        ("#IMPLIED", "#REQUIRED", or "#FIXED") or null if
		///        none of these applies. </param>
		/// <param name="value"> A string representing the attribute's default value,
		///        or null if there is none. </param>
		/// <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void attributeDecl(String eName, String aName, String type, String valueDefault, String value) throws org.xml.sax.SAXException
		public virtual void attributeDecl(string eName, string aName, string type, string valueDefault, string value)
		{
		  if (null != m_resultDeclHandler)
		  {
									m_resultDeclHandler.attributeDecl(eName, aName, type, valueDefault, value);
		  }
		}


		/// <summary>
		/// Report an internal entity declaration.
		/// 
		/// <para>Only the effective (first) declaration for each entity
		/// will be reported.</para>
		/// </summary>
		/// <param name="name"> The name of the entity.  If it is a parameter
		///        entity, the name will begin with '%'. </param>
		/// <param name="value"> The replacement text of the entity. </param>
		/// <exception cref="SAXException"> The application may raise an exception. </exception>
		/// <seealso cref= #externalEntityDecl </seealso>
		/// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void internalEntityDecl(String name, String value) throws org.xml.sax.SAXException
		public virtual void internalEntityDecl(string name, string value)
		{
		  if (null != m_resultDeclHandler)
		  {
									m_resultDeclHandler.internalEntityDecl(name, value);
		  }
		}


		/// <summary>
		/// Report a parsed external entity declaration.
		/// 
		/// <para>Only the effective (first) declaration for each entity
		/// will be reported.</para>
		/// </summary>
		/// <param name="name"> The name of the entity.  If it is a parameter
		///        entity, the name will begin with '%'. </param>
		/// <param name="publicId"> The declared public identifier of the entity, or
		///        null if none was declared. </param>
		/// <param name="systemId"> The declared system identifier of the entity. </param>
		/// <exception cref="SAXException"> The application may raise an exception. </exception>
		/// <seealso cref= #internalEntityDecl </seealso>
		/// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void externalEntityDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void externalEntityDecl(string name, string publicId, string systemId)
		{
		  if (null != m_resultDeclHandler)
		  {
									m_resultDeclHandler.externalEntityDecl(name, publicId, systemId);
		  }
		}

	  /// <summary>
	  /// This is null unless we own the stream.
	  /// </summary>
	  private System.IO.FileStream m_outputStream = null;

	  /// <summary>
	  /// The content handler where result events will be sent. </summary>
	  private ContentHandler m_resultContentHandler;

	  /// <summary>
	  /// The lexical handler where result events will be sent. </summary>
	  private LexicalHandler m_resultLexicalHandler;

	  /// <summary>
	  /// The DTD handler where result events will be sent. </summary>
	  private DTDHandler m_resultDTDHandler;

	  /// <summary>
	  /// The Decl handler where result events will be sent. </summary>
	  private DeclHandler m_resultDeclHandler;

	  /// <summary>
	  /// The Serializer, which may or may not be null. </summary>
	  private Serializer m_serializer;

	  /// <summary>
	  /// The Result object. </summary>
	  private Result m_result;

	  /// <summary>
	  /// The system ID, which is unused, but must be returned to fullfill the
	  ///  TransformerHandler interface.
	  /// </summary>
	  private string m_systemID;

	  /// <summary>
	  /// The parameters, which is unused, but must be returned to fullfill the
	  ///  Transformer interface.
	  /// </summary>
	  private Hashtable m_params;

	  /// <summary>
	  /// The error listener for TrAX errors and warnings. </summary>
	  private ErrorListener m_errorListener = new org.apache.xml.utils.DefaultErrorHandler(false);

	  /// <summary>
	  /// The URIResolver, which is unused, but must be returned to fullfill the
	  ///  TransformerHandler interface.
	  /// </summary>
	  internal URIResolver m_URIResolver;

	  /// <summary>
	  /// The output properties. </summary>
	  private OutputProperties m_outputFormat;

	  /// <summary>
	  /// Flag to set if we've found the first element, so we can tell if we have 
	  ///  to check to see if we should create an HTML serializer.      
	  /// </summary>
	  internal bool m_foundFirstElement;

	  /// <summary>
	  /// State of the secure processing feature.
	  /// </summary>
	  private bool m_isSecureProcessing = false;
	}

}