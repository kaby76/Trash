using System;
using System.Collections;
using System.IO;
using System.Text;

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
 * $Id: TransformerImpl.java 1225432 2011-12-29 05:01:46Z mrglavas $
 */

namespace org.apache.xalan.xsltc.trax
{


	using DOM = org.apache.xalan.xsltc.DOM;
	using DOMCache = org.apache.xalan.xsltc.DOMCache;
	using StripFilter = org.apache.xalan.xsltc.StripFilter;
	using Translet = org.apache.xalan.xsltc.Translet;
	using TransletException = org.apache.xalan.xsltc.TransletException;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using DOMWSFilter = org.apache.xalan.xsltc.dom.DOMWSFilter;
	using SAXImpl = org.apache.xalan.xsltc.dom.SAXImpl;
	using XSLTCDTMManager = org.apache.xalan.xsltc.dom.XSLTCDTMManager;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;
	using TransletOutputHandlerFactory = org.apache.xalan.xsltc.runtime.output.TransletOutputHandlerFactory;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using OutputPropertiesFactory = org.apache.xml.serializer.OutputPropertiesFactory;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using XMLReaderManager = org.apache.xml.utils.XMLReaderManager;
	using ContentHandler = org.xml.sax.ContentHandler;
	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using XMLReader = org.xml.sax.XMLReader;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// @author Morten Jorgensen
	/// @author G. Todd Miller
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class TransformerImpl : Transformer, DOMCache, ErrorListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			_errorListener = this;
		}

		private const string EMPTY_STRING = "";
		private const string NO_STRING = "no";
		private const string YES_STRING = "yes";
		private const string XML_STRING = "xml";

		private const string LEXICAL_HANDLER_PROPERTY = "http://xml.org/sax/properties/lexical-handler";
		private const string NAMESPACE_FEATURE = "http://xml.org/sax/features/namespaces";

		/// <summary>
		/// A reference to the translet or null if the identity transform.
		/// </summary>
		private AbstractTranslet _translet = null;

		/// <summary>
		/// The output method of this transformation.
		/// </summary>
		private string _method = null;

		/// <summary>
		/// The output encoding of this transformation.
		/// </summary>
		private string _encoding = null;

		/// <summary>
		/// The systemId set in input source.
		/// </summary>
		private string _sourceSystemId = null;

		/// <summary>
		/// An error listener for runtime errors.
		/// </summary>
		private ErrorListener _errorListener;

		/// <summary>
		/// A reference to a URI resolver for calls to document().
		/// </summary>
		private URIResolver _uriResolver = null;

		/// <summary>
		/// Output properties of this transformer instance.
		/// </summary>
		private Properties _properties, _propertiesClone;

		/// <summary>
		/// A reference to an output handler factory.
		/// </summary>
		private TransletOutputHandlerFactory _tohFactory = null;

		/// <summary>
		/// A reference to a internal DOM represenation of the input.
		/// </summary>
		private DOM _dom = null;

		/// <summary>
		/// Number of indent spaces to add when indentation is on.
		/// </summary>
		private int _indentNumber;

		/// <summary>
		/// A reference to the transformer factory that this templates
		/// object belongs to.
		/// </summary>
		private TransformerFactoryImpl _tfactory = null;

		/// <summary>
		/// A reference to the output stream, if we create one in our code.
		/// </summary>
		private Stream _ostream = null;

		/// <summary>
		/// A reference to the XSLTCDTMManager which is used to build the DOM/DTM
		/// for this transformer.
		/// </summary>
		private XSLTCDTMManager _dtmManager = null;

		/// <summary>
		/// A reference to an object that creates and caches XMLReader objects.
		/// </summary>
		private XMLReaderManager _readerManager = XMLReaderManager.Instance;

		/// <summary>
		/// A flag indicating whether we use incremental building of the DTM.
		/// </summary>
		//private boolean _isIncremental = false;

		/// <summary>
		/// A flag indicating whether this transformer implements the identity 
		/// transform.
		/// </summary>
		private bool _isIdentity = false;

		/// <summary>
		/// State of the secure processing feature.
		/// </summary>
		private bool _isSecureProcessing = false;

		/// <summary>
		/// A hashtable to store parameters for the identity transform. These
		/// are not needed during the transformation, but we must keep track of 
		/// them to be fully complaint with the JAXP API.
		/// </summary>
		private Hashtable _parameters = null;

		/// <summary>
		/// This class wraps an ErrorListener into a MessageHandler in order to
		/// capture messages reported via xsl:message.
		/// </summary>
		internal class MessageHandler : org.apache.xalan.xsltc.runtime.MessageHandler
		{
		internal ErrorListener _errorListener;

		public MessageHandler(ErrorListener errorListener)
		{
			_errorListener = errorListener;
		}

		public override void displayMessage(string msg)
		{
			if (_errorListener == null)
			{
			Console.Error.WriteLine(msg);
			}
			else
			{
			try
			{
				_errorListener.warning(new TransformerException(msg));
			}
			catch (TransformerException)
			{
				// ignored 
			}
			}
		}
		}

		protected internal TransformerImpl(Properties outputProperties, int indentNumber, TransformerFactoryImpl tfactory) : this(null, outputProperties, indentNumber, tfactory)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		_isIdentity = true;
		// _properties.put(OutputKeys.METHOD, "xml");
		}

		protected internal TransformerImpl(Translet translet, Properties outputProperties, int indentNumber, TransformerFactoryImpl tfactory)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		_translet = (AbstractTranslet) translet;
		_properties = createOutputProperties(outputProperties);
		_propertiesClone = (Properties) _properties.clone();
		_indentNumber = indentNumber;
		_tfactory = tfactory;
		//_isIncremental = tfactory._incremental;
		}

		/// <summary>
		/// Return the state of the secure processing feature.
		/// </summary>
		public bool SecureProcessing
		{
			get
			{
				return _isSecureProcessing;
			}
			set
			{
				_isSecureProcessing = value;
			}
		}


		/// <summary>
		/// Returns the translet wrapped inside this Transformer or
		/// null if this is the identity transform.
		/// </summary>
		protected internal AbstractTranslet Translet
		{
			get
			{
			return _translet;
			}
		}

		public bool Identity
		{
			get
			{
			return _isIdentity;
			}
		}

		/// <summary>
		/// Implements JAXP's Transformer.transform()
		/// </summary>
		/// <param name="source"> Contains the input XML document </param>
		/// <param name="result"> Will contain the output from the transformation </param>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void transform(javax.xml.transform.Source source, javax.xml.transform.Result result) throws javax.xml.transform.TransformerException
		public void transform(Source source, Result result)
		{
		if (!_isIdentity)
		{
			if (_translet == null)
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_NO_TRANSLET_ERR);
			throw new TransformerException(err.ToString());
			}
			// Pass output properties to the translet
			transferOutputProperties(_translet);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.serializer.SerializationHandler toHandler = getOutputHandler(result);
		SerializationHandler toHandler = getOutputHandler(result);
		if (toHandler == null)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_NO_HANDLER_ERR);
			throw new TransformerException(err.ToString());
		}

		if (_uriResolver != null && !_isIdentity)
		{
			_translet.DOMCache = this;
		}

		// Pass output properties to handler if identity
		if (_isIdentity)
		{
			transferOutputProperties(toHandler);
		}

		transform(source, toHandler, _encoding);

		if (result is DOMResult)
		{
			((DOMResult)result).setNode(_tohFactory.Node);
		}
		}

		/// <summary>
		/// Create an output handler for the transformation output based on 
		/// the type and contents of the TrAX Result object passed to the 
		/// transform() method. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler getOutputHandler(javax.xml.transform.Result result) throws javax.xml.transform.TransformerException
		public SerializationHandler getOutputHandler(Result result)
		{
		// Get output method using get() to ignore defaults 
		_method = (string) _properties.get(OutputKeys.METHOD);

		// Get encoding using getProperty() to use defaults
		_encoding = (string) _properties.getProperty(OutputKeys.ENCODING);

		_tohFactory = TransletOutputHandlerFactory.newInstance();
		_tohFactory.Encoding = _encoding;
		if (!string.ReferenceEquals(_method, null))
		{
			_tohFactory.OutputMethod = _method;
		}

		// Set indentation number in the factory
		if (_indentNumber >= 0)
		{
			_tohFactory.IndentNumber = _indentNumber;
		}

		// Return the content handler for this Result object
		try
		{
			// Result object could be SAXResult, DOMResult, or StreamResult 
			if (result is SAXResult)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.sax.SAXResult target = (javax.xml.transform.sax.SAXResult)result;
					SAXResult target = (SAXResult)result;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.ContentHandler handler = target.getHandler();
					ContentHandler handler = target.getHandler();

			_tohFactory.Handler = handler;

					/// <summary>
					/// Fix for bug 24414
					/// If the lexicalHandler is set then we need to get that
					/// for obtaining the lexical information 
					/// </summary>
					LexicalHandler lexicalHandler = target.getLexicalHandler();

					if (lexicalHandler != null)
					{
				_tohFactory.LexicalHandler = lexicalHandler;
					}

			_tohFactory.OutputType = TransletOutputHandlerFactory.SAX;
			return _tohFactory.SerializationHandler;
			}
			else if (result is DOMResult)
			{
			_tohFactory.Node = ((DOMResult) result).getNode();
			_tohFactory.NextSibling = ((DOMResult) result).getNextSibling();
			_tohFactory.OutputType = TransletOutputHandlerFactory.DOM;
			return _tohFactory.SerializationHandler;
			}
			else if (result is StreamResult)
			{
			// Get StreamResult
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.stream.StreamResult target = (javax.xml.transform.stream.StreamResult) result;
			StreamResult target = (StreamResult) result;

			// StreamResult may have been created with a java.io.File,
			// java.io.Writer, java.io.OutputStream or just a String
			// systemId. 

			_tohFactory.OutputType = TransletOutputHandlerFactory.STREAM;

			// try to get a Writer from Result object
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = target.getWriter();
			Writer writer = target.getWriter();
			if (writer != null)
			{
				_tohFactory.Writer = writer;
				return _tohFactory.SerializationHandler;
			}

			// or try to get an OutputStream from Result object
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.OutputStream ostream = target.getOutputStream();
			Stream ostream = target.getOutputStream();
			if (ostream != null)
			{
				_tohFactory.OutputStream = ostream;
				return _tohFactory.SerializationHandler;
			}

			// or try to get just a systemId string from Result object
			string systemId = result.getSystemId();
			if (string.ReferenceEquals(systemId, null))
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_NO_RESULT_ERR);
						throw new TransformerException(err.ToString());
			}

			// System Id may be in one of several forms, (1) a uri
			// that starts with 'file:', (2) uri that starts with 'http:'
			// or (3) just a filename on the local system.
			URL url = null;
			if (systemId.StartsWith("file:", StringComparison.Ordinal))
			{
						url = new URL(systemId);
				_tohFactory.OutputStream = _ostream = new FileStream(url.getFile(), FileMode.Create, FileAccess.Write);
				return _tohFactory.SerializationHandler;
			}
					else if (systemId.StartsWith("http:", StringComparison.Ordinal))
					{
						url = new URL(systemId);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.net.URLConnection connection = url.openConnection();
						URLConnection connection = url.openConnection();
				_tohFactory.OutputStream = _ostream = connection.getOutputStream();
				return _tohFactory.SerializationHandler;
					}
					else
					{
						// system id is just a filename
						url = (new File(systemId)).toURL();
				_tohFactory.OutputStream = _ostream = new FileStream(url.getFile(), FileMode.Create, FileAccess.Write);
				return _tohFactory.SerializationHandler;
					}
			}
		}
			// If we cannot write to the location specified by the SystemId
			catch (UnknownServiceException e)
			{
				throw new TransformerException(e);
			}
			catch (ParserConfigurationException e)
			{
				throw new TransformerException(e);
			}
			// If we cannot create the file specified by the SystemId
			catch (IOException e)
			{
				throw new TransformerException(e);
			}
		return null;
		}

		/// <summary>
		/// Set the internal DOM that will be used for the next transformation
		/// </summary>
		protected internal DOM DOM
		{
			set
			{
			_dom = value;
			}
		}

		/// <summary>
		/// Builds an internal DOM from a TrAX Source object
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.apache.xalan.xsltc.DOM getDOM(javax.xml.transform.Source source) throws javax.xml.transform.TransformerException
		private DOM getDOM(Source source)
		{
			try
			{
				DOM dom = null;

				if (source != null)
				{
					DTMWSFilter wsfilter;
					if (_translet != null && _translet is StripFilter)
					{
						wsfilter = new DOMWSFilter(_translet);
					}
					 else
					 {
						wsfilter = null;
					 }

					 bool hasIdCall = (_translet != null) ? _translet.hasIdCall() : false;

					 if (_dtmManager == null)
					 {
						 _dtmManager = (XSLTCDTMManager)System.Activator.CreateInstance(_tfactory.DTMManagerClass);
					 }
					 dom = (DOM)_dtmManager.getDTM(source, false, wsfilter, true, false, false, 0, hasIdCall);
				}
				else if (_dom != null)
				{
					 dom = _dom;
					 _dom = null; // use only once, so reset to 'null'
				}
				else
				{
					 return null;
				}

				if (!_isIdentity)
				{
					// Give the translet the opportunity to make a prepass of
					// the document, in case it can extract useful information early
					_translet.prepassDocument(dom);
				}

				return dom;

			}
			catch (Exception e)
			{
				if (_errorListener != null)
				{
					postErrorToListener(e.Message);
				}
				throw new TransformerException(e);
			}
		}

		/// <summary>
		/// Returns the <seealso cref="org.apache.xalan.xsltc.trax.TransformerFactoryImpl"/>
		/// object that create this <code>Transformer</code>.
		/// </summary>
		protected internal TransformerFactoryImpl TransformerFactory
		{
			get
			{
				return _tfactory;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="org.apache.xalan.xsltc.runtime.output.TransletOutputHandlerFactory"/>
		/// object that create the <code>TransletOutputHandler</code>.
		/// </summary>
		protected internal TransletOutputHandlerFactory TransletOutputHandlerFactory
		{
			get
			{
				return _tohFactory;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transformIdentity(javax.xml.transform.Source source, org.apache.xml.serializer.SerializationHandler handler) throws Exception
		private void transformIdentity(Source source, SerializationHandler handler)
		{
			// Get systemId from source
			if (source != null)
			{
				_sourceSystemId = source.getSystemId();
			}

			if (source is StreamSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.stream.StreamSource stream = (javax.xml.transform.stream.StreamSource) source;
				StreamSource stream = (StreamSource) source;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream streamInput = stream.getInputStream();
				Stream streamInput = stream.getInputStream();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Reader streamReader = stream.getReader();
				Reader streamReader = stream.getReader();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.XMLReader reader = _readerManager.getXMLReader();
				XMLReader reader = _readerManager.XMLReader;

				try
				{
					// Hook up reader and output handler 
					try
					{
						reader.setProperty(LEXICAL_HANDLER_PROPERTY, handler);
					}
					catch (SAXException)
					{
						// Falls through
					}
					reader.setContentHandler(handler);

					// Create input source from source
					InputSource input;
					if (streamInput != null)
					{
						input = new InputSource(streamInput);
						input.setSystemId(_sourceSystemId);
					}
					else if (streamReader != null)
					{
						input = new InputSource(streamReader);
						input.setSystemId(_sourceSystemId);
					}
					else if (!string.ReferenceEquals(_sourceSystemId, null))
					{
						input = new InputSource(_sourceSystemId);
					}
					else
					{
						ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_NO_SOURCE_ERR);
						throw new TransformerException(err.ToString());
					}

					// Start pushing SAX events
					reader.parse(input);
				}
				finally
				{
					_readerManager.releaseXMLReader(reader);
				}
			}
			else if (source is SAXSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.sax.SAXSource sax = (javax.xml.transform.sax.SAXSource) source;
				SAXSource sax = (SAXSource) source;
				XMLReader reader = sax.getXMLReader();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.InputSource input = sax.getInputSource();
				InputSource input = sax.getInputSource();
				bool userReader = true;

				try
				{
					// Create a reader if not set by user
					if (reader == null)
					{
						reader = _readerManager.XMLReader;
						userReader = false;
					}

					// Hook up reader and output handler 
					try
					{
						reader.setProperty(LEXICAL_HANDLER_PROPERTY, handler);
					}
					catch (SAXException)
					{
						// Falls through
					}
					reader.setContentHandler(handler);

					// Start pushing SAX events
					reader.parse(input);
				}
				finally
				{
					if (!userReader)
					{
						_readerManager.releaseXMLReader(reader);
					}
				}
			}
			else if (source is DOMSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.dom.DOMSource domsrc = (javax.xml.transform.dom.DOMSource) source;
				DOMSource domsrc = (DOMSource) source;
				(new DOM2TO(domsrc.getNode(), handler)).parse();
			}
			else if (source is XSLTCSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.DOM dom = ((XSLTCSource) source).getDOM(null, _translet);
				DOM dom = ((XSLTCSource) source).getDOM(null, _translet);
				((SAXImpl)dom).copy(handler);
			}
			else
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_NO_SOURCE_ERR);
				throw new TransformerException(err.ToString());
			}
		}

		/// <summary>
		/// Internal transformation method - uses the internal APIs of XSLTC
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void transform(javax.xml.transform.Source source, org.apache.xml.serializer.SerializationHandler handler, String encoding) throws javax.xml.transform.TransformerException
		private void transform(Source source, SerializationHandler handler, string encoding)
		{
		try
		{
				/*
				 * According to JAXP1.2, new SAXSource()/StreamSource()
				 * should create an empty input tree, with a default root node. 
				 * new DOMSource()creates an empty document using DocumentBuilder.
				 * newDocument(); Use DocumentBuilder.newDocument() for all 3 
				 * situations, since there is no clear spec. how to create 
				 * an empty tree when both SAXSource() and StreamSource() are used.
				 */
				if ((source is StreamSource && source.getSystemId() == null && ((StreamSource)source).getInputStream() == null && ((StreamSource)source).getReader() == null) || (source is SAXSource && ((SAXSource)source).getInputSource() == null && ((SAXSource)source).getXMLReader() == null) || (source is DOMSource && ((DOMSource)source).getNode() == null))
				{
							DocumentBuilderFactory builderF = DocumentBuilderFactory.newInstance();
							DocumentBuilder builder = builderF.newDocumentBuilder();
							string systemID = source.getSystemId();
							source = new DOMSource(builder.newDocument());

							// Copy system ID from original, empty Source to new
							if (!string.ReferenceEquals(systemID, null))
							{
							  source.setSystemId(systemID);
							}
				}
			if (_isIdentity)
			{
			transformIdentity(source, handler);
			}
			else
			{
			_translet.transform(getDOM(source), handler);
			}
		}
		catch (TransletException e)
		{
			if (_errorListener != null)
			{
				postErrorToListener(e.Message);
			}
			throw new TransformerException(e);
		}
		catch (Exception e)
		{
			if (_errorListener != null)
			{
				postErrorToListener(e.Message);
			}
			throw new TransformerException(e);
		}
		catch (Exception e)
		{
			if (_errorListener != null)
			{
				postErrorToListener(e.Message);
			}
			throw new TransformerException(e);
		}
		finally
		{
				_dtmManager = null;
		}

		// If we create an output stream for the Result, we need to close it after the transformation.
		if (_ostream != null)
		{
			try
			{
				_ostream.Close();
			}
			catch (IOException)
			{
			}
			_ostream = null;
		}
		}

		/// <summary>
		/// Implements JAXP's Transformer.getErrorListener()
		/// Get the error event handler in effect for the transformation.
		/// </summary>
		/// <returns> The error event handler currently in effect </returns>
		public ErrorListener ErrorListener
		{
			get
			{
			return _errorListener;
			}
			set
			{
				if (value == null)
				{
				ErrorMsg err = new ErrorMsg(ErrorMsg.ERROR_LISTENER_NULL_ERR, "Transformer");
					throw new System.ArgumentException(err.ToString());
				}
				_errorListener = value;
    
			// Register a message handler to report xsl:messages
			if (_translet != null)
			{
				_translet.MessageHandler = new MessageHandler(_errorListener);
			}
			}
		}


		/// <summary>
		/// Inform TrAX error listener of an error
		/// </summary>
		private void postErrorToListener(string message)
		{
			try
			{
				_errorListener.error(new TransformerException(message));
			}
		catch (TransformerException)
		{
				// ignored - transformation cannot be continued
		}
		}

		/// <summary>
		/// Inform TrAX error listener of a warning
		/// </summary>
		private void postWarningToListener(string message)
		{
			try
			{
				_errorListener.warning(new TransformerException(message));
			}
		catch (TransformerException)
		{
				// ignored - transformation cannot be continued
		}
		}

		/// <summary>
		/// The translet stores all CDATA sections set in the <xsl:output> element
		/// in a Hashtable. This method will re-construct the whitespace separated
		/// list of elements given in the <xsl:output> element.
		/// </summary>
		private string makeCDATAString(Hashtable cdata)
		{
		// Return a 'null' string if no CDATA section elements were specified
		if (cdata == null)
		{
			return null;
		}

		StringBuilder result = new StringBuilder();

		// Get an enumeration of all the elements in the hashtable
		System.Collections.IEnumerator elements = cdata.keys();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		if (elements.hasMoreElements())
		{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			result.Append((string)elements.nextElement());
			while (elements.MoveNext())
			{
			string element = (string)elements.Current;
			result.Append(' ');
			result.Append(element);
			}
		}

		return (result.ToString());
		}

		/// <summary>
		/// Implements JAXP's Transformer.getOutputProperties().
		/// Returns a copy of the output properties for the transformation. This is
		/// a set of layered properties. The first layer contains properties set by
		/// calls to setOutputProperty() and setOutputProperties() on this class,
		/// and the output settings defined in the stylesheet's <xsl:output>
		/// element makes up the second level, while the default XSLT output
		/// settings are returned on the third level.
		/// </summary>
		/// <returns> Properties in effect for this Transformer </returns>
		public Properties OutputProperties
		{
			get
			{
			return (Properties) _properties.clone();
			}
			set
			{
			if (value != null)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.Enumeration names = value.propertyNames();
				System.Collections.IEnumerator names = value.propertyNames();
    
				while (names.MoveNext())
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String name = (String) names.Current;
				string name = (string) names.Current;
    
				// Ignore lower layer value
				if (isDefaultProperty(name, value))
				{
					continue;
				}
    
				if (validOutputProperty(name))
				{
					_properties.setProperty(name, value.getProperty(name));
				}
				else
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_UNKNOWN_PROP_ERR, name);
					throw new System.ArgumentException(err.ToString());
				}
				}
			}
			else
			{
				_properties = _propertiesClone;
			}
			}
		}

		/// <summary>
		/// Implements JAXP's Transformer.getOutputProperty().
		/// Get an output property that is in effect for the transformation. The
		/// property specified may be a property that was set with setOutputProperty,
		/// or it may be a property specified in the stylesheet.
		/// </summary>
		/// <param name="name"> A non-null string that contains the name of the property </param>
		/// <exception cref="IllegalArgumentException"> if the property name is not known </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getOutputProperty(String name) throws IllegalArgumentException
		public string getOutputProperty(string name)
		{
		if (!validOutputProperty(name))
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_UNKNOWN_PROP_ERR, name);
			throw new System.ArgumentException(err.ToString());
		}
		return _properties.getProperty(name);
		}


		/// <summary>
		/// Implements JAXP's Transformer.setOutputProperty().
		/// Get an output property that is in effect for the transformation. The
		/// property specified may be a property that was set with 
		/// setOutputProperty(), or it may be a property specified in the stylesheet.
		/// </summary>
		/// <param name="name"> The name of the property to set </param>
		/// <param name="value"> The value to assign to the property </param>
		/// <exception cref="IllegalArgumentException"> Never, errors are ignored </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setOutputProperty(String name, String value) throws IllegalArgumentException
		public void setOutputProperty(string name, string value)
		{
		if (!validOutputProperty(name))
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_UNKNOWN_PROP_ERR, name);
			throw new System.ArgumentException(err.ToString());
		}
		_properties.setProperty(name, value);
		}

		/// <summary>
		/// Internal method to pass any properties to the translet prior to
		/// initiating the transformation
		/// </summary>
		private void transferOutputProperties(AbstractTranslet translet)
		{
		// Return right now if no properties are set
		if (_properties == null)
		{
			return;
		}

		// Get a list of all the defined properties
		System.Collections.IEnumerator names = _properties.propertyNames();
		while (names.MoveNext())
		{
			// Note the use of get() instead of getProperty()
			string name = (string) names.Current;
			string value = (string) _properties.get(name);

			// Ignore default properties
			if (string.ReferenceEquals(value, null))
			{
				continue;
			}

			// Pass property value to translet - override previous setting
			if (name.Equals(OutputKeys.ENCODING))
			{
			translet._encoding = value;
			}
			else if (name.Equals(OutputKeys.METHOD))
			{
			translet._method = value;
			}
			else if (name.Equals(OutputKeys.DOCTYPE_PUBLIC))
			{
			translet._doctypePublic = value;
			}
			else if (name.Equals(OutputKeys.DOCTYPE_SYSTEM))
			{
			translet._doctypeSystem = value;
			}
			else if (name.Equals(OutputKeys.MEDIA_TYPE))
			{
			translet._mediaType = value;
			}
			else if (name.Equals(OutputKeys.STANDALONE))
			{
			translet._standalone = value;
			}
			else if (name.Equals(OutputKeys.VERSION))
			{
			translet._version = value;
			}
			else if (name.Equals(OutputKeys.OMIT_XML_DECLARATION))
			{
			translet._omitHeader = (!string.ReferenceEquals(value, null) && value.ToLower().Equals("yes"));
			}
			else if (name.Equals(OutputKeys.INDENT))
			{
			translet._indent = (!string.ReferenceEquals(value, null) && value.ToLower().Equals("yes"));
			}
			else if (name.Equals(OutputKeys.CDATA_SECTION_ELEMENTS))
			{
			if (!string.ReferenceEquals(value, null))
			{
				translet._cdata = null; // clear previous setting
				StringTokenizer e = new StringTokenizer(value);
				while (e.hasMoreTokens())
				{
				translet.addCdataElement(e.nextToken());
				}
			}
			}
		}
		}

		/// <summary>
		/// This method is used to pass any properties to the output handler
		/// when running the identity transform.
		/// </summary>
		public void transferOutputProperties(SerializationHandler handler)
		{
		// Return right now if no properties are set
		if (_properties == null)
		{
			return;
		}

		string doctypePublic = null;
		string doctypeSystem = null;

		// Get a list of all the defined properties
		System.Collections.IEnumerator names = _properties.propertyNames();
		while (names.MoveNext())
		{
			// Note the use of get() instead of getProperty()
			string name = (string) names.Current;
			string value = (string) _properties.get(name);

			// Ignore default properties
			if (string.ReferenceEquals(value, null))
			{
				continue;
			}

			// Pass property value to translet - override previous setting
			if (name.Equals(OutputKeys.DOCTYPE_PUBLIC))
			{
			doctypePublic = value;
			}
			else if (name.Equals(OutputKeys.DOCTYPE_SYSTEM))
			{
			doctypeSystem = value;
			}
			else if (name.Equals(OutputKeys.MEDIA_TYPE))
			{
			handler.MediaType = value;
			}
			else if (name.Equals(OutputKeys.STANDALONE))
			{
			handler.Standalone = value;
			}
			else if (name.Equals(OutputKeys.VERSION))
			{
			handler.Version = value;
			}
			else if (name.Equals(OutputKeys.OMIT_XML_DECLARATION))
			{
			handler.OmitXMLDeclaration = !string.ReferenceEquals(value, null) && value.ToLower().Equals("yes");
			}
			else if (name.Equals(OutputKeys.INDENT))
			{
			handler.Indent = !string.ReferenceEquals(value, null) && value.ToLower().Equals("yes");
			}
			else if (name.Equals(OutputKeys.CDATA_SECTION_ELEMENTS))
			{
			if (!string.ReferenceEquals(value, null))
			{
				StringTokenizer e = new StringTokenizer(value);
						ArrayList uriAndLocalNames = null;
				while (e.hasMoreTokens())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String token = e.nextToken();
				string token = e.nextToken();

							// look for the last colon, as the String may be
							// something like "http://abc.com:local"
							int lastcolon = token.LastIndexOf(':');
							string uri;
							string localName;
							if (lastcolon > 0)
							{
								uri = token.Substring(0, lastcolon);
								localName = token.Substring(lastcolon + 1);
							}
							else
							{
								// no colon at all, lets hope this is the
								// local name itself then
								uri = null;
								localName = token;
							}

							if (uriAndLocalNames == null)
							{
								uriAndLocalNames = new ArrayList();
							}
							// add the uri/localName as a pair, in that order
							uriAndLocalNames.Add(uri);
							uriAndLocalNames.Add(localName);
				}
						handler.CdataSectionElements = uriAndLocalNames;
			}
			}
		}

		// Call setDoctype() if needed
		if (!string.ReferenceEquals(doctypePublic, null) || !string.ReferenceEquals(doctypeSystem, null))
		{
			handler.setDoctype(doctypeSystem, doctypePublic);
		}
		}

		/// <summary>
		/// Internal method to create the initial set of properties. There
		/// are two layers of properties: the default layer and the base layer.
		/// The latter contains properties defined in the stylesheet or by
		/// the user using this API.
		/// </summary>
		private Properties createOutputProperties(Properties outputProperties)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Properties defaults = new java.util.Properties();
		Properties defaults = new Properties();
		setDefaults(defaults, "xml");

		// Copy propeties set in stylesheet to base
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Properties super = new java.util.Properties(defaults);
		Properties @base = new Properties(defaults);
		if (outputProperties != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration names = outputProperties.propertyNames();
			System.Collections.IEnumerator names = outputProperties.propertyNames();
			while (names.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = (String) names.Current;
			string name = (string) names.Current;
			@base.setProperty(name, outputProperties.getProperty(name));
			}
		}
		else
		{
			@base.setProperty(OutputKeys.ENCODING, _translet._encoding);
			if (!string.ReferenceEquals(_translet._method, null))
			{
				@base.setProperty(OutputKeys.METHOD, _translet._method);
			}
		}

		// Update defaults based on output method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String method = super.getProperty(javax.xml.transform.OutputKeys.METHOD);
		string method = @base.getProperty(OutputKeys.METHOD);
		if (!string.ReferenceEquals(method, null))
		{
			if (method.Equals("html"))
			{
				setDefaults(defaults,"html");
			}
			else if (method.Equals("text"))
			{
				setDefaults(defaults,"text");
			}
		}

		return @base;
		}

		/// <summary>
		/// Internal method to get the default properties from the
		/// serializer factory and set them on the property object. </summary>
		/// <param name="props"> a java.util.Property object on which the properties are set. </param>
		/// <param name="method"> The output method type, one of "xml", "text", "html" ... </param>
		private void setDefaults(Properties props, string method)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Properties method_props = org.apache.xml.serializer.OutputPropertiesFactory.getDefaultMethodProperties(method);
			Properties method_props = OutputPropertiesFactory.getDefaultMethodProperties(method);
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration names = method_props.propertyNames();
				System.Collections.IEnumerator names = method_props.propertyNames();
				while (names.MoveNext())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = (String)names.Current;
					string name = (string)names.Current;
					props.setProperty(name, method_props.getProperty(name));
				}
			}
		}
		/// <summary>
		/// Verifies if a given output property name is a property defined in
		/// the JAXP 1.1 / TrAX spec
		/// </summary>
		private bool validOutputProperty(string name)
		{
		return (name.Equals(OutputKeys.ENCODING) || name.Equals(OutputKeys.METHOD) || name.Equals(OutputKeys.INDENT) || name.Equals(OutputKeys.DOCTYPE_PUBLIC) || name.Equals(OutputKeys.DOCTYPE_SYSTEM) || name.Equals(OutputKeys.CDATA_SECTION_ELEMENTS) || name.Equals(OutputKeys.MEDIA_TYPE) || name.Equals(OutputKeys.OMIT_XML_DECLARATION) || name.Equals(OutputKeys.STANDALONE) || name.Equals(OutputKeys.VERSION) || name[0] == '{');
		}

		/// <summary>
		/// Checks if a given output property is default (2nd layer only)
		/// </summary>
		private bool isDefaultProperty(string name, Properties properties)
		{
		return (properties.get(name) == null);
		}

		/// <summary>
		/// Implements JAXP's Transformer.setParameter()
		/// Add a parameter for the transformation. The parameter is simply passed
		/// on to the translet - no validation is performed - so any unused
		/// parameters are quitely ignored by the translet.
		/// </summary>
		/// <param name="name"> The name of the parameter </param>
		/// <param name="value"> The value to assign to the parameter </param>
		public void setParameter(string name, object value)
		{

			if (value == null)
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, name);
				throw new System.ArgumentException(err.ToString());
			}

		if (_isIdentity)
		{
			if (_parameters == null)
			{
			_parameters = new Hashtable();
			}
			_parameters.put(name, value);
		}
		else
		{
			_translet.addParameter(name, value);
		}
		}

		/// <summary>
		/// Implements JAXP's Transformer.clearParameters()
		/// Clear all parameters set with setParameter. Clears the translet's
		/// parameter stack.
		/// </summary>
		public void clearParameters()
		{
		if (_isIdentity && _parameters != null)
		{
			_parameters.clear();
		}
		else
		{
			_translet.clearParameters();
		}
		}

		/// <summary>
		/// Implements JAXP's Transformer.getParameter()
		/// Returns the value of a given parameter. Note that the translet will not
		/// keep values for parameters that were not defined in the stylesheet.
		/// </summary>
		/// <param name="name"> The name of the parameter </param>
		/// <returns> An object that contains the value assigned to the parameter </returns>
		public object getParameter(string name)
		{
		if (_isIdentity)
		{
			return (_parameters != null) ? _parameters.get(name) : null;
		}
		else
		{
			return _translet.getParameter(name);
		}
		}

		/// <summary>
		/// Implements JAXP's Transformer.getURIResolver()
		/// Set the object currently used to resolve URIs used in document().
		/// </summary>
		/// <returns>  The URLResolver object currently in use </returns>
		public URIResolver URIResolver
		{
			get
			{
			return _uriResolver;
			}
			set
			{
			_uriResolver = value;
			}
		}


		/// <summary>
		/// This class should only be used as a DOMCache for the translet if the
		/// URIResolver has been set.
		/// 
		/// The method implements XSLTC's DOMCache interface, which is used to
		/// plug in an external document loader into a translet. This method acts
		/// as an adapter between TrAX's URIResolver interface and XSLTC's
		/// DOMCache interface. This approach is simple, but removes the
		/// possibility of using external document caches with XSLTC.
		/// </summary>
		/// <param name="baseURI"> The base URI used by the document call. </param>
		/// <param name="href"> The href argument passed to the document function. </param>
		/// <param name="translet"> A reference to the translet requesting the document </param>
		public DOM retrieveDocument(string baseURI, string href, Translet translet)
		{
		try
		{
				// Argument to document function was: document('');
				if (href.Length == 0)
				{
					href = baseURI;
				}

				/*
				 *  Fix for bug 24188
				 *  Incase the _uriResolver.resolve(href,base) is null
				 *  try to still  retrieve the document before returning null 
				 *  and throwing the FileNotFoundException in
				 *  org.apache.xalan.xsltc.dom.LoadDocument
				 *
				 */
				Source resolvedSource = _uriResolver.resolve(href, baseURI);
				if (resolvedSource == null)
				{
					StreamSource streamSource = new StreamSource(SystemIDResolver.getAbsoluteURI(href, baseURI));
					return getDOM(streamSource);
				}

				return getDOM(resolvedSource);
		}
		catch (TransformerException e)
		{
			if (_errorListener != null)
			{
			postErrorToListener("File not found: " + e.Message);
			}
			return (null);
		}
		}

		/// <summary>
		/// Receive notification of a recoverable error. 
		/// The transformer must continue to provide normal parsing events after
		/// invoking this method. It should still be possible for the application
		/// to process the document through to the end.
		/// </summary>
		/// <param name="e"> The warning information encapsulated in a transformer 
		/// exception. </param>
		/// <exception cref="TransformerException"> if the application chooses to discontinue
		/// the transformation (always does in our case). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.TransformerException e) throws javax.xml.transform.TransformerException
		public void error(TransformerException e)
		{
			Exception wrapped = e.getException();
			if (wrapped != null)
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.ERROR_PLUS_WRAPPED_MSG, e.getMessageAndLocation(), wrapped.Message));
			}
			else
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.ERROR_MSG, e.getMessageAndLocation()));
			}
			throw e;
		}

		/// <summary>
		/// Receive notification of a non-recoverable error. 
		/// The application must assume that the transformation cannot continue
		/// after the Transformer has invoked this method, and should continue
		/// (if at all) only to collect addition error messages. In fact,
		/// Transformers are free to stop reporting events once this method has
		/// been invoked.
		/// </summary>
		/// <param name="e"> The warning information encapsulated in a transformer
		/// exception. </param>
		/// <exception cref="TransformerException"> if the application chooses to discontinue
		/// the transformation (always does in our case). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void fatalError(javax.xml.transform.TransformerException e) throws javax.xml.transform.TransformerException
		public void fatalError(TransformerException e)
		{
			Exception wrapped = e.getException();
			if (wrapped != null)
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, e.getMessageAndLocation(), wrapped.Message));
			}
			else
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.FATAL_ERR_MSG, e.getMessageAndLocation()));
			}
			throw e;
		}

		/// <summary>
		/// Receive notification of a warning.
		/// Transformers can use this method to report conditions that are not
		/// errors or fatal errors. The default behaviour is to take no action.
		/// After invoking this method, the Transformer must continue with the
		/// transformation. It should still be possible for the application to
		/// process the document through to the end.
		/// </summary>
		/// <param name="e"> The warning information encapsulated in a transformer
		/// exception. </param>
		/// <exception cref="TransformerException"> if the application chooses to discontinue
		/// the transformation (never does in our case). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warning(javax.xml.transform.TransformerException e) throws javax.xml.transform.TransformerException
		public void warning(TransformerException e)
		{
			Exception wrapped = e.getException();
			if (wrapped != null)
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.WARNING_PLUS_WRAPPED_MSG, e.getMessageAndLocation(), wrapped.Message));
			}
			else
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.WARNING_MSG, e.getMessageAndLocation()));
			}
		}

		/// <summary>
		/// This method resets  the Transformer to its original configuration
		/// Transformer code is reset to the same state it was when it was
		/// created
		/// @since 1.5
		/// </summary>
		public void reset()
		{

			_method = null;
			_encoding = null;
			_sourceSystemId = null;
			_errorListener = this;
			_uriResolver = null;
			_dom = null;
			_parameters = null;
			_indentNumber = 0;
			OutputProperties = null;

		}
	}

}