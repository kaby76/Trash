using System;
using System.Collections;
using System.IO;

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
 * $Id: TransformerFactoryImpl.java 1225273 2011-12-28 18:46:51Z mrglavas $
 */

namespace org.apache.xalan.xsltc.trax
{


	using SourceLoader = org.apache.xalan.xsltc.compiler.SourceLoader;
	using XSLTC = org.apache.xalan.xsltc.compiler.XSLTC;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using XSLTCDTMManager = org.apache.xalan.xsltc.dom.XSLTCDTMManager;
	using StopParseException = org.apache.xml.utils.StopParseException;
	using StylesheetPIHandler = org.apache.xml.utils.StylesheetPIHandler;
	using InputSource = org.xml.sax.InputSource;
	using XMLFilter = org.xml.sax.XMLFilter;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// Implementation of a JAXP1.1 TransformerFactory for Translets.
	/// @author G. Todd Miller 
	/// @author Morten Jorgensen
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class TransformerFactoryImpl : SAXTransformerFactory, SourceLoader, ErrorListener
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			_errorListener = this;
		}

		// Public constants for attributes supported by the XSLTC TransformerFactory.
		public const string TRANSLET_NAME = "translet-name";
		public const string DESTINATION_DIRECTORY = "destination-directory";
		public const string PACKAGE_NAME = "package-name";
		public const string JAR_NAME = "jar-name";
		public const string GENERATE_TRANSLET = "generate-translet";
		public const string AUTO_TRANSLET = "auto-translet";
		public const string USE_CLASSPATH = "use-classpath";
		public const string DEBUG = "debug";
		public const string ENABLE_INLINING = "enable-inlining";
		public const string INDENT_NUMBER = "indent-number";

		/// <summary>
		/// This error listener is used only for this factory and is not passed to
		/// the Templates or Transformer objects that we create.
		/// </summary>
		private ErrorListener _errorListener;

		/// <summary>
		/// This URIResolver is passed to all created Templates and Transformers
		/// </summary>
		private URIResolver _uriResolver = null;

		/// <summary>
		/// As Gregor Samsa awoke one morning from uneasy dreams he found himself
		/// transformed in his bed into a gigantic insect. He was lying on his hard,
		/// as it were armour plated, back, and if he lifted his head a little he
		/// could see his big, brown belly divided into stiff, arched segments, on
		/// top of which the bed quilt could hardly keep in position and was about
		/// to slide off completely. His numerous legs, which were pitifully thin
		/// compared to the rest of his bulk, waved helplessly before his eyes.
		/// "What has happened to me?", he thought. It was no dream....
		/// </summary>
		protected internal const string DEFAULT_TRANSLET_NAME = "GregorSamsa";

		/// <summary>
		/// The class name of the translet
		/// </summary>
		private string _transletName = DEFAULT_TRANSLET_NAME;

		/// <summary>
		/// The destination directory for the translet
		/// </summary>
		private string _destinationDirectory = null;

		/// <summary>
		/// The package name prefix for all generated translet classes
		/// </summary>
		private string _packageName = null;

		/// <summary>
		/// The jar file name which the translet classes are packaged into
		/// </summary>
		private string _jarFileName = null;

		/// <summary>
		/// This Hashtable is used to store parameters for locating
		/// <?xml-stylesheet ...?> processing instructions in XML docs.
		/// </summary>
		private Hashtable _piParams = null;

		/// <summary>
		/// The above hashtable stores objects of this class.
		/// </summary>
		private class PIParamWrapper
		{
		public string _media = null;
		public string _title = null;
		public string _charset = null;

		public PIParamWrapper(string media, string title, string charset)
		{
			_media = media;
			_title = title;
			_charset = charset;
		}
		}

		/// <summary>
		/// Set to <code>true</code> when debugging is enabled.
		/// </summary>
		private bool _debug = false;

		/// <summary>
		/// Set to <code>true</code> when templates are inlined.
		/// </summary>
		private bool _enableInlining = false;

		/// <summary>
		/// Set to <code>true</code> when we want to generate 
		/// translet classes from the stylesheet.
		/// </summary>
		private bool _generateTranslet = false;

		/// <summary>
		/// If this is set to <code>true</code>, we attempt to use translet classes
		/// for transformation if possible without compiling the stylesheet. The
		/// translet class is only used if its timestamp is newer than the timestamp
		/// of the stylesheet.
		/// </summary>
		private bool _autoTranslet = false;

		/// <summary>
		/// If this is set to <code>true</code>, we attempt to load the translet
		/// from the CLASSPATH.
		/// </summary>
		private bool _useClasspath = false;

		/// <summary>
		/// Number of indent spaces when indentation is turned on.
		/// </summary>
		private int _indentNumber = -1;

		/// <summary>
		/// The provider of the XSLTC DTM Manager service.  This is fixed for any
		/// instance of this class.  In order to change service providers, a new
		/// XSLTC <code>TransformerFactory</code> must be instantiated. </summary>
		/// <seealso cref="XSLTCDTMManager.getDTMManagerClass()"/>
		private Type m_DTMManagerClass;

		/// <summary>
		/// <para>State of secure processing feature.</para>
		/// </summary>
		private bool _isSecureProcessing = false;

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// </summary>
		public TransformerFactoryImpl()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			m_DTMManagerClass = XSLTCDTMManager.DTMManagerClass;
		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Set the error event listener for the TransformerFactory, which is used
		/// for the processing of transformation instructions, and not for the
		/// transformation itself.
		/// </summary>
		/// <param name="listener"> The error listener to use with the TransformerFactory </param>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setErrorListener(javax.xml.transform.ErrorListener listener) throws IllegalArgumentException
		public virtual ErrorListener ErrorListener
		{
			set
			{
			if (value == null)
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.ERROR_LISTENER_NULL_ERR, "TransformerFactory");
					throw new System.ArgumentException(err.ToString());
			}
			_errorListener = value;
			}
			get
			{
			return _errorListener;
			}
		}


		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Returns the value set for a TransformerFactory attribute
		/// </summary>
		/// <param name="name"> The attribute name </param>
		/// <returns> An object representing the attribute value </returns>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object getAttribute(String name) throws IllegalArgumentException
		public virtual object getAttribute(string name)
		{
		// Return value for attribute 'translet-name'
		if (name.Equals(TRANSLET_NAME))
		{
			return _transletName;
		}
		else if (name.Equals(GENERATE_TRANSLET))
		{
			return _generateTranslet ? true : false;
		}
		else if (name.Equals(AUTO_TRANSLET))
		{
			return _autoTranslet ? true : false;
		}
		else if (name.Equals(ENABLE_INLINING))
		{
			if (_enableInlining)
			{
			  return true;
			}
			else
			{
			  return false;
			}
		}

		// Throw an exception for all other attributes
		ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_INVALID_ATTR_ERR, name);
		throw new System.ArgumentException(err.ToString());
		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Sets the value for a TransformerFactory attribute.
		/// </summary>
		/// <param name="name"> The attribute name </param>
		/// <param name="value"> An object representing the attribute value </param>
		/// <exception cref="IllegalArgumentException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setAttribute(String name, Object value) throws IllegalArgumentException
		public virtual void setAttribute(string name, object value)
		{
		// Set the default translet name (ie. class name), which will be used
		// for translets that cannot be given a name from their system-id.
		if (name.Equals(TRANSLET_NAME) && value is string)
		{
			_transletName = (string) value;
			return;
		}
		else if (name.Equals(DESTINATION_DIRECTORY) && value is string)
		{
			_destinationDirectory = (string) value;
			return;
		}
		else if (name.Equals(PACKAGE_NAME) && value is string)
		{
			_packageName = (string) value;
			return;
		}
		else if (name.Equals(JAR_NAME) && value is string)
		{
			_jarFileName = (string) value;
			return;
		}
		else if (name.Equals(GENERATE_TRANSLET))
		{
			if (value is Boolean)
			{
			_generateTranslet = ((bool?) value).Value;
			return;
			}
			else if (value is string)
			{
			_generateTranslet = ((string) value).Equals("true", StringComparison.OrdinalIgnoreCase);
			return;
			}
		}
		else if (name.Equals(AUTO_TRANSLET))
		{
			if (value is Boolean)
			{
			_autoTranslet = ((bool?) value).Value;
			return;
			}
			else if (value is string)
			{
			_autoTranslet = ((string) value).Equals("true", StringComparison.OrdinalIgnoreCase);
			return;
			}
		}
		else if (name.Equals(USE_CLASSPATH))
		{
			if (value is Boolean)
			{
			_useClasspath = ((bool?) value).Value;
			return;
			}
			else if (value is string)
			{
			_useClasspath = ((string) value).Equals("true", StringComparison.OrdinalIgnoreCase);
			return;
			}
		}
		else if (name.Equals(DEBUG))
		{
			if (value is Boolean)
			{
			_debug = ((bool?) value).Value;
			return;
			}
			else if (value is string)
			{
			_debug = ((string) value).Equals("true", StringComparison.OrdinalIgnoreCase);
			return;
			}
		}
		else if (name.Equals(ENABLE_INLINING))
		{
			if (value is Boolean)
			{
			_enableInlining = ((bool?) value).Value;
			return;
			}
			else if (value is string)
			{
			_enableInlining = ((string) value).Equals("true", StringComparison.OrdinalIgnoreCase);
			return;
			}
		}
		else if (name.Equals(INDENT_NUMBER))
		{
			if (value is string)
			{
			try
			{
				_indentNumber = int.Parse((string) value);
				return;
			}
			catch (System.FormatException)
			{
				// Falls through
			}
			}
			else if (value is Integer)
			{
			_indentNumber = ((int?) value).Value;
			return;
			}
		}

		// Throw an exception for all other attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg err = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.JAXP_INVALID_ATTR_ERR, name);
		ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_INVALID_ATTR_ERR, name);
		throw new System.ArgumentException(err.ToString());
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setFeature(String name, boolean value) throws javax.xml.transform.TransformerConfigurationException
		public virtual void setFeature(string name, bool value)
		{

		// feature name cannot be null
		if (string.ReferenceEquals(name, null))
		{
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_SET_FEATURE_NULL_NAME);
				throw new System.NullReferenceException(err.ToString());
		}
		// secure processing?
		else if (name.Equals(XMLConstants.FEATURE_SECURE_PROCESSING))
		{
			_isSecureProcessing = value;
			// all done processing feature
			return;
		}
		else
		{
			// unknown feature
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_UNSUPPORTED_FEATURE, name);
				throw new TransformerConfigurationException(err.ToString());
		}
		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Look up the value of a feature (to see if it is supported).
		/// This method must be updated as the various methods and features of this
		/// class are implemented.
		/// </summary>
		/// <param name="name"> The feature name </param>
		/// <returns> 'true' if feature is supported, 'false' if not </returns>
		public virtual bool getFeature(string name)
		{
		// All supported features should be listed here
		string[] features = new string[] {DOMSource.FEATURE, DOMResult.FEATURE, SAXSource.FEATURE, SAXResult.FEATURE, StreamSource.FEATURE, StreamResult.FEATURE, SAXTransformerFactory.FEATURE, SAXTransformerFactory.FEATURE_XMLFILTER};

		// feature name cannot be null
		if (string.ReferenceEquals(name, null))
		{
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_GET_FEATURE_NULL_NAME);
				throw new System.NullReferenceException(err.ToString());
		}

		// Inefficient, but array is small
		for (int i = 0; i < features.Length; i++)
		{
			if (name.Equals(features[i]))
			{
			return true;
			}
		}
		// secure processing?
		if (name.Equals(XMLConstants.FEATURE_SECURE_PROCESSING))
		{
			return _isSecureProcessing;
		}

		// Feature not supported
		return false;
		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Get the object that is used by default during the transformation to
		/// resolve URIs used in document(), xsl:import, or xsl:include.
		/// </summary>
		/// <returns> The URLResolver used for this TransformerFactory and all
		/// Templates and Transformer objects created using this factory </returns>
		public virtual URIResolver URIResolver
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
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Get the stylesheet specification(s) associated via the xml-stylesheet
		/// processing instruction (see http://www.w3.org/TR/xml-stylesheet/) with
		/// the document document specified in the source parameter, and that match
		/// the given criteria.
		/// </summary>
		/// <param name="source"> The XML source document. </param>
		/// <param name="media"> The media attribute to be matched. May be null, in which
		/// case the prefered templates will be used (i.e. alternate = no). </param>
		/// <param name="title"> The value of the title attribute to match. May be null. </param>
		/// <param name="charset"> The value of the charset attribute to match. May be null. </param>
		/// <returns> A Source object suitable for passing to the TransformerFactory. </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Source getAssociatedStylesheet(javax.xml.transform.Source source, String media, String title, String charset) throws javax.xml.transform.TransformerConfigurationException
		public virtual Source getAssociatedStylesheet(Source source, string media, string title, string charset)
		{

			string baseId;
			XMLReader reader = null;
			InputSource isource = null;


			/// <summary>
			/// Fix for bugzilla bug 24187
			/// </summary>
			StylesheetPIHandler _stylesheetPIHandler = new StylesheetPIHandler(null,media,title,charset);

			try
			{

				if (source is DOMSource)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.dom.DOMSource domsrc = (javax.xml.transform.dom.DOMSource) source;
					DOMSource domsrc = (DOMSource) source;
					baseId = domsrc.getSystemId();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node node = domsrc.getNode();
					org.w3c.dom.Node node = domsrc.getNode();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DOM2SAX dom2sax = new DOM2SAX(node);
					DOM2SAX dom2sax = new DOM2SAX(node);

					_stylesheetPIHandler.BaseId = baseId;

					dom2sax.ContentHandler = _stylesheetPIHandler;
					dom2sax.parse();
				}
				else
				{
					isource = SAXSource.sourceToInputSource(source);
					baseId = isource.getSystemId();

					SAXParserFactory factory = SAXParserFactory.newInstance();
					factory.setNamespaceAware(true);

					if (_isSecureProcessing)
					{
						try
						{
							factory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
						}
						catch (org.xml.sax.SAXException)
						{
						}
					}

					SAXParser jaxpParser = factory.newSAXParser();

					reader = jaxpParser.getXMLReader();
					if (reader == null)
					{
						reader = XMLReaderFactory.createXMLReader();
					}

					_stylesheetPIHandler.BaseId = baseId;
					reader.setContentHandler(_stylesheetPIHandler);
					reader.parse(isource);

				}

				if (_uriResolver != null)
				{
					_stylesheetPIHandler.URIResolver = _uriResolver;
				}

			}
			catch (StopParseException)
			{
			  // startElement encountered so do not parse further

			}
			catch (javax.xml.parsers.ParserConfigurationException e)
			{

				 throw new TransformerConfigurationException("getAssociatedStylesheets failed", e);

			}
			catch (org.xml.sax.SAXException se)
			{

				 throw new TransformerConfigurationException("getAssociatedStylesheets failed", se);


			}
			catch (IOException ioe)
			{
			   throw new TransformerConfigurationException("getAssociatedStylesheets failed", ioe);

			}

			 return _stylesheetPIHandler.AssociatedStylesheet;

		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Create a Transformer object that copies the input document to the result.
		/// </summary>
		/// <returns> A Transformer object that simply copies the source to the result. </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Transformer newTransformer() throws javax.xml.transform.TransformerConfigurationException
		public virtual Transformer newTransformer()
		{
		TransformerImpl result = new TransformerImpl(new Properties(), _indentNumber, this);
		if (_uriResolver != null)
		{
			result.URIResolver = _uriResolver;
		}

		if (_isSecureProcessing)
		{
			result.SecureProcessing = true;
		}
		return result;
		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Process the Source into a Templates object, which is a a compiled
		/// representation of the source. Note that this method should not be
		/// used with XSLTC, as the time-consuming compilation is done for each
		/// and every transformation.
		/// </summary>
		/// <returns> A Templates object that can be used to create Transformers. </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Transformer newTransformer(javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
		public virtual Transformer newTransformer(Source source)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Templates templates = newTemplates(source);
		Templates templates = newTemplates(source);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Transformer transformer = templates.newTransformer();
		Transformer transformer = templates.newTransformer();
		if (_uriResolver != null)
		{
			transformer.setURIResolver(_uriResolver);
		}
		return (transformer);
		}

		/// <summary>
		/// Pass warning messages from the compiler to the error listener
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void passWarningsToListener(java.util.Vector messages) throws javax.xml.transform.TransformerException
		private void passWarningsToListener(ArrayList messages)
		{
		if (_errorListener == null || messages == null)
		{
			return;
		}
		// Pass messages to listener, one by one
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = messages.size();
		int count = messages.Count;
		for (int pos = 0; pos < count; pos++)
		{
			ErrorMsg msg = (ErrorMsg)messages[pos];
			// Workaround for the TCK failure ErrorListener.errorTests.error001.
			if (msg.WarningError)
			{
				_errorListener.error(new TransformerConfigurationException(msg.ToString()));
			}
			else
			{
				_errorListener.warning(new TransformerConfigurationException(msg.ToString()));
			}
		}
		}

		/// <summary>
		/// Pass error messages from the compiler to the error listener
		/// </summary>
		private void passErrorsToListener(ArrayList messages)
		{
		try
		{
			if (_errorListener == null || messages == null)
			{
			return;
			}
			// Pass messages to listener, one by one
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = messages.size();
			int count = messages.Count;
			for (int pos = 0; pos < count; pos++)
			{
			string message = messages[pos].ToString();
			_errorListener.error(new TransformerException(message));
			}
		}
		catch (TransformerException)
		{
			// nada
		}
		}

		/// <summary>
		/// javax.xml.transform.sax.TransformerFactory implementation.
		/// Process the Source into a Templates object, which is a a compiled
		/// representation of the source.
		/// </summary>
		/// <param name="source"> The input stylesheet - DOMSource not supported!!! </param>
		/// <returns> A Templates object that can be used to create Transformers. </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Templates newTemplates(javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
		public virtual Templates newTemplates(Source source)
		{
		// If the _useClasspath attribute is true, try to load the translet from
		// the CLASSPATH and create a template object using the loaded
		// translet.
		if (_useClasspath)
		{
			string transletName = getTransletBaseName(source);

			if (!string.ReferenceEquals(_packageName, null))
			{
				transletName = _packageName + "." + transletName;
			}

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class clazz = ObjectFactory.findProviderClass(transletName, ObjectFactory.findClassLoader(), true);
					Type clazz = ObjectFactory.findProviderClass(transletName, ObjectFactory.findClassLoader(), true);
				resetTransientAttributes();

				return new TemplatesImpl(new Type[]{clazz}, transletName, null, _indentNumber, this);
			}
			catch (ClassNotFoundException)
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR, transletName);
				throw new TransformerConfigurationException(err.ToString());
			}
			catch (Exception e)
			{
				ErrorMsg err = new ErrorMsg(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + e.Message);
				throw new TransformerConfigurationException(err.ToString());
			}
		}

		// If _autoTranslet is true, we will try to load the bytecodes
		// from the translet classes without compiling the stylesheet.
		if (_autoTranslet)
		{
			sbyte[][] bytecodes = null;
			string transletClassName = getTransletBaseName(source);

			if (!string.ReferenceEquals(_packageName, null))
			{
				transletClassName = _packageName + "." + transletClassName;
			}

			if (!string.ReferenceEquals(_jarFileName, null))
			{
				bytecodes = getBytecodesFromJar(source, transletClassName);
			}
			else
			{
				bytecodes = getBytecodesFromClasses(source, transletClassName);
			}

			if (bytecodes != null)
			{
				if (_debug)
				{
					  if (!string.ReferenceEquals(_jarFileName, null))
					  {
					Console.Error.WriteLine(new ErrorMsg(ErrorMsg.TRANSFORM_WITH_JAR_STR, transletClassName, _jarFileName));
					  }
					else
					{
						Console.Error.WriteLine(new ErrorMsg(ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, transletClassName));
					}
				}

				// Reset the per-session attributes to their default values
				// after each newTemplates() call.
				resetTransientAttributes();

				return new TemplatesImpl(bytecodes, transletClassName, null, _indentNumber, this);
			}
		}

		// Create and initialize a stylesheet compiler
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.XSLTC xsltc = new org.apache.xalan.xsltc.compiler.XSLTC();
		XSLTC xsltc = new XSLTC();
		if (_debug)
		{
			xsltc.Debug = true;
		}
		if (_enableInlining)
		{
			xsltc.TemplateInlining = true;
		}
		else
		{
			xsltc.TemplateInlining = false;
		}
		if (_isSecureProcessing)
		{
			xsltc.SecureProcessing = true;
		}
		xsltc.init();

		// Set a document loader (for xsl:include/import) if defined
		if (_uriResolver != null)
		{
			xsltc.SourceLoader = this;
		}

		// Pass parameters to the Parser to make sure it locates the correct
		// <?xml-stylesheet ...?> PI in an XML input document
		if ((_piParams != null) && (_piParams[source] != null))
		{
			// Get the parameters for this Source object
			PIParamWrapper p = (PIParamWrapper)_piParams[source];
			// Pass them on to the compiler (which will pass then to the parser)
			if (p != null)
			{
			xsltc.setPIParameters(p._media, p._title, p._charset);
			}
		}

		// Set the attributes for translet generation
		int outputType = XSLTC.BYTEARRAY_OUTPUT;
		if (_generateTranslet || _autoTranslet)
		{
			// Set the translet name
			xsltc.ClassName = getTransletBaseName(source);

			if (!string.ReferenceEquals(_destinationDirectory, null))
			{
				xsltc.DestDirectory = _destinationDirectory;
			}
			else
			{
				string xslName = getStylesheetFileName(source);
				if (!string.ReferenceEquals(xslName, null))
				{
					  File xslFile = new File(xslName);
					string xslDir = xslFile.getParent();

					  if (!string.ReferenceEquals(xslDir, null))
					  {
						xsltc.DestDirectory = xslDir;
					  }
				}
			}

			if (!string.ReferenceEquals(_packageName, null))
			{
				xsltc.PackageName = _packageName;
			}

			if (!string.ReferenceEquals(_jarFileName, null))
			{
				xsltc.JarFileName = _jarFileName;
				outputType = XSLTC.BYTEARRAY_AND_JAR_OUTPUT;
			}
			else
			{
				outputType = XSLTC.BYTEARRAY_AND_FILE_OUTPUT;
			}
		}

		// Compile the stylesheet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.InputSource input = Util.getInputSource(xsltc, source);
		InputSource input = Util.getInputSource(xsltc, source);
		sbyte[][] bytecodes = xsltc.compile(null, input, outputType);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String transletName = xsltc.getClassName();
		string transletName = xsltc.ClassName;

		// Output to the jar file if the jar file name is set.
		if ((_generateTranslet || _autoTranslet) && bytecodes != null && !string.ReferenceEquals(_jarFileName, null))
		{
			try
			{
				xsltc.outputToJar();
			}
			catch (IOException)
			{
			}
		}

		// Reset the per-session attributes to their default values
		// after each newTemplates() call.
		resetTransientAttributes();

		// Pass compiler warnings to the error listener
		if (_errorListener != this)
		{
			try
			{
			passWarningsToListener(xsltc.Warnings);
			}
			catch (TransformerException e)
			{
			throw new TransformerConfigurationException(e);
			}
		}
		else
		{
			xsltc.printWarnings();
		}

		// Check that the transformation went well before returning
		if (bytecodes == null)
		{

			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_COMPILE_ERR);
			TransformerConfigurationException exc = new TransformerConfigurationException(err.ToString());

			// Pass compiler errors to the error listener
			if (_errorListener != null)
			{
				passErrorsToListener(xsltc.Errors);

				// As required by TCK 1.2, send a fatalError to the
				// error listener because compilation of the stylesheet
				// failed and no further processing will be possible.
				try
				{
					_errorListener.fatalError(exc);
				}
				catch (TransformerException)
				{
					// well, we tried.
				}
			}
			else
			{
				xsltc.printErrors();
			}
			throw exc;
		}

		return new TemplatesImpl(bytecodes, transletName, xsltc.OutputProperties, _indentNumber, this);
		}

		/// <summary>
		/// javax.xml.transform.sax.SAXTransformerFactory implementation.
		/// Get a TemplatesHandler object that can process SAX ContentHandler
		/// events into a Templates object.
		/// </summary>
		/// <returns> A TemplatesHandler object that can handle SAX events </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TemplatesHandler newTemplatesHandler() throws javax.xml.transform.TransformerConfigurationException
		public virtual TemplatesHandler newTemplatesHandler()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TemplatesHandlerImpl handler = new TemplatesHandlerImpl(_indentNumber, this);
		TemplatesHandlerImpl handler = new TemplatesHandlerImpl(_indentNumber, this);
		if (_uriResolver != null)
		{
			handler.URIResolver = _uriResolver;
		}
		return handler;
		}

		/// <summary>
		/// javax.xml.transform.sax.SAXTransformerFactory implementation.
		/// Get a TransformerHandler object that can process SAX ContentHandler
		/// events into a Result. This method will return a pure copy transformer.
		/// </summary>
		/// <returns> A TransformerHandler object that can handle SAX events </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler() throws javax.xml.transform.TransformerConfigurationException
		public virtual TransformerHandler newTransformerHandler()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Transformer transformer = newTransformer();
		Transformer transformer = newTransformer();
		if (_uriResolver != null)
		{
			transformer.setURIResolver(_uriResolver);
		}
		return new TransformerHandlerImpl((TransformerImpl) transformer);
		}

		/// <summary>
		/// javax.xml.transform.sax.SAXTransformerFactory implementation.
		/// Get a TransformerHandler object that can process SAX ContentHandler
		/// events into a Result, based on the transformation instructions
		/// specified by the argument.
		/// </summary>
		/// <param name="src"> The source of the transformation instructions. </param>
		/// <returns> A TransformerHandler object that can handle SAX events </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler(javax.xml.transform.Source src) throws javax.xml.transform.TransformerConfigurationException
		public virtual TransformerHandler newTransformerHandler(Source src)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Transformer transformer = newTransformer(src);
		Transformer transformer = newTransformer(src);
		if (_uriResolver != null)
		{
			transformer.setURIResolver(_uriResolver);
		}
		return new TransformerHandlerImpl((TransformerImpl) transformer);
		}

		/// <summary>
		/// javax.xml.transform.sax.SAXTransformerFactory implementation.
		/// Get a TransformerHandler object that can process SAX ContentHandler
		/// events into a Result, based on the transformation instructions
		/// specified by the argument.
		/// </summary>
		/// <param name="templates"> Represents a pre-processed stylesheet </param>
		/// <returns> A TransformerHandler object that can handle SAX events </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
		public virtual TransformerHandler newTransformerHandler(Templates templates)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Transformer transformer = templates.newTransformer();
		Transformer transformer = templates.newTransformer();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TransformerImpl internal = (TransformerImpl)transformer;
		TransformerImpl @internal = (TransformerImpl)transformer;
		return new TransformerHandlerImpl(@internal);
		}

		/// <summary>
		/// javax.xml.transform.sax.SAXTransformerFactory implementation.
		/// Create an XMLFilter that uses the given source as the
		/// transformation instructions.
		/// </summary>
		/// <param name="src"> The source of the transformation instructions. </param>
		/// <returns> An XMLFilter object, or null if this feature is not supported. </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
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
		/// javax.xml.transform.sax.SAXTransformerFactory implementation.
		/// Create an XMLFilter that uses the given source as the
		/// transformation instructions.
		/// </summary>
		/// <param name="templates"> The source of the transformation instructions. </param>
		/// <returns> An XMLFilter object, or null if this feature is not supported. </returns>
		/// <exception cref="TransformerConfigurationException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.xml.sax.XMLFilter newXMLFilter(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
		public virtual XMLFilter newXMLFilter(Templates templates)
		{
		try
		{
				  return new org.apache.xalan.xsltc.trax.TrAXFilter(templates);
		}
		catch (TransformerConfigurationException e1)
		{
				  if (_errorListener != null)
				  {
					try
					{
					  _errorListener.fatalError(e1);
					  return null;
					}
			catch (TransformerException e2)
			{
					  new TransformerConfigurationException(e2);
			}
				  }
				  throw e1;
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
		public virtual void error(TransformerException e)
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
		/// <param name="e"> warning information encapsulated in a transformer
		/// exception. </param>
		/// <exception cref="TransformerException"> if the application chooses to discontinue
		/// the transformation (always does in our case). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void fatalError(javax.xml.transform.TransformerException e) throws javax.xml.transform.TransformerException
		public virtual void fatalError(TransformerException e)
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
		public virtual void warning(TransformerException e)
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
		/// This method implements XSLTC's SourceLoader interface. It is used to
		/// glue a TrAX URIResolver to the XSLTC compiler's Input and Import classes.
		/// </summary>
		/// <param name="href"> The URI of the document to load </param>
		/// <param name="context"> The URI of the currently loaded document </param>
		/// <param name="xsltc"> The compiler that resuests the document </param>
		/// <returns> An InputSource with the loaded document </returns>
		public virtual InputSource loadSource(string href, string context, XSLTC xsltc)
		{
		try
		{
			if (_uriResolver != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Source source = _uriResolver.resolve(href, context);
			Source source = _uriResolver.resolve(href, context);
			if (source != null)
			{
				return Util.getInputSource(xsltc, source);
			}
			}
		}
		catch (TransformerException)
		{
			// Falls through
		}
		return null;
		}

		/// <summary>
		/// Reset the per-session attributes to their default values
		/// </summary>
		private void resetTransientAttributes()
		{
		_transletName = DEFAULT_TRANSLET_NAME;
		_destinationDirectory = null;
		_packageName = null;
		_jarFileName = null;
		}

		/// <summary>
		/// Load the translet classes from local .class files and return
		/// the bytecode array.
		/// </summary>
		/// <param name="source"> The xsl source </param>
		/// <param name="fullClassName"> The full name of the translet </param>
		/// <returns> The bytecode array </returns>
		private sbyte[][] getBytecodesFromClasses(Source source, string fullClassName)
		{
			if (string.ReferenceEquals(fullClassName, null))
			{
				return null;
			}

			string xslFileName = getStylesheetFileName(source);
			File xslFile = null;
			if (!string.ReferenceEquals(xslFileName, null))
			{
				xslFile = new File(xslFileName);
			}

			// Find the base name of the translet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String transletName;
			string transletName;
			int lastDotIndex = fullClassName.LastIndexOf('.');
			if (lastDotIndex > 0)
			{
				transletName = fullClassName.Substring(lastDotIndex + 1);
			}
			else
			{
				transletName = fullClassName;
			}

			// Construct the path name for the translet class file
			string transletPath = fullClassName.Replace('.', '/');
			if (!string.ReferenceEquals(_destinationDirectory, null))
			{
				transletPath = _destinationDirectory + "/" + transletPath + ".class";
			}
			else
			{
				if (xslFile != null && xslFile.getParent() != null)
				{
					transletPath = xslFile.getParent() + "/" + transletPath + ".class";
				}
				else
				{
					transletPath = transletPath + ".class";
				}
			}

			// Return null if the translet class file does not exist.
			File transletFile = new File(transletPath);
			if (!transletFile.exists())
			{
				return null;
			}

			// Compare the timestamps of the translet and the xsl file.
			// If the translet is older than the xsl file, return null 
			// so that the xsl file is used for the transformation and
			// the translet is regenerated.
			if (xslFile != null && xslFile.exists())
			{
				long xslTimestamp = xslFile.lastModified();
				long transletTimestamp = transletFile.lastModified();
				if (transletTimestamp < xslTimestamp)
				{
					return null;
				}
			}

			// Load the translet into a bytecode array.
			System.Collections.IList bytecodes = new ArrayList();
			int fileLength = (int)transletFile.length();
			if (fileLength > 0)
			{
				FileStream input = null;
				try
				{
					input = new FileStream(transletFile, FileMode.Open, FileAccess.Read);
				}
				catch (FileNotFoundException)
				{
					return null;
				}

				sbyte[] bytes = new sbyte[fileLength];
				try
				{
				readFromInputStream(bytes, input, fileLength);
				input.Close();
				}
			catch (IOException)
			{
					return null;
			}

				bytecodes.Add(bytes);
			}
			else
			{
				return null;
			}

			// Find the parent directory of the translet.
			string transletParentDir = transletFile.getParent();
			if (string.ReferenceEquals(transletParentDir, null))
			{
				transletParentDir = System.getProperty("user.dir");
			}

			File transletParentFile = new File(transletParentDir);

			// Find all the auxiliary files which have a name pattern of "transletClass$nnn.class".
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String transletAuxPrefix = transletName + "$";
			string transletAuxPrefix = transletName + "$";
			File[] auxfiles = transletParentFile.listFiles(new FilenameFilterAnonymousInnerClass(this, transletAuxPrefix));

			// Load the auxiliary class files and add them to the bytecode array.
			for (int i = 0; i < auxfiles.Length; i++)
			{
				File auxfile = auxfiles[i];
				int auxlength = (int)auxfile.length();
				if (auxlength > 0)
				{
					FileStream auxinput = null;
					try
					{
						  auxinput = new FileStream(auxfile, FileMode.Open, FileAccess.Read);
					}
					catch (FileNotFoundException)
					{
						  continue;
					}

					sbyte[] bytes = new sbyte[auxlength];

					try
					{
						  readFromInputStream(bytes, auxinput, auxlength);
						  auxinput.Close();
					}
					catch (IOException)
					{
						  continue;
					}

					bytecodes.Add(bytes);
				}
			}

			// Convert the Vector of byte[] to byte[][].
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = bytecodes.size();
			int count = bytecodes.Count;
			if (count > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[][] result = new byte[count][1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: sbyte[][] result = new sbyte[count][1];
				sbyte[][] result = RectangularArrays.RectangularSbyteArray(count, 1);
				for (int i = 0; i < count; i++)
				{
					result[i] = (sbyte[])bytecodes[i];
				}

				return result;
			}
			else
			{
				return null;
			}
		}

		private class FilenameFilterAnonymousInnerClass : FilenameFilter
		{
			private readonly TransformerFactoryImpl outerInstance;

			private string transletAuxPrefix;

			public FilenameFilterAnonymousInnerClass(TransformerFactoryImpl outerInstance, string transletAuxPrefix)
			{
				this.outerInstance = outerInstance;
				this.transletAuxPrefix = transletAuxPrefix;
			}

			public bool accept(File dir, string name)
			{
				return (name.EndsWith(".class", StringComparison.Ordinal) && name.StartsWith(transletAuxPrefix, StringComparison.Ordinal));
			}
		}

		/// <summary>
		/// Load the translet classes from the jar file and return the bytecode.
		/// </summary>
		/// <param name="source"> The xsl source </param>
		/// <param name="fullClassName"> The full name of the translet </param>
		/// <returns> The bytecode array </returns>
		private sbyte[][] getBytecodesFromJar(Source source, string fullClassName)
		{
			string xslFileName = getStylesheetFileName(source);
			File xslFile = null;
			if (!string.ReferenceEquals(xslFileName, null))
			{
				xslFile = new File(xslFileName);
			}

			  // Construct the path for the jar file
			  string jarPath = null;
			  if (!string.ReferenceEquals(_destinationDirectory, null))
			  {
				jarPath = _destinationDirectory + "/" + _jarFileName;
			  }
			  else
			  {
				  if (xslFile != null && xslFile.getParent() != null)
				  {
					jarPath = xslFile.getParent() + "/" + _jarFileName;
				  }
				else
				{
					jarPath = _jarFileName;
				}
			  }

			  // Return null if the jar file does not exist.
			  File file = new File(jarPath);
			  if (!file.exists())
			  {
				return null;
			  }

			 // Compare the timestamps of the jar file and the xsl file. Return null
			 // if the xsl file is newer than the jar file.
			if (xslFile != null && xslFile.exists())
			{
				long xslTimestamp = xslFile.lastModified();
				long transletTimestamp = file.lastModified();
				if (transletTimestamp < xslTimestamp)
				{
					return null;
				}
			}

			  // Create a ZipFile object for the jar file
			  ZipFile jarFile = null;
			  try
			  {
				jarFile = new ZipFile(file);
			  }
			  catch (IOException)
			  {
				return null;
			  }

			  string transletPath = fullClassName.Replace('.', '/');
			  string transletAuxPrefix = transletPath + "$";
			  string transletFullName = transletPath + ".class";

			  System.Collections.IList bytecodes = new ArrayList();

			  // Iterate through all entries in the jar file to find the 
			  // translet and auxiliary classes.
			  System.Collections.IEnumerator entries = jarFile.entries();
			  while (entries.MoveNext())
			  {
				ZipEntry entry = (ZipEntry)entries.Current;
				string entryName = entry.getName();
				if (entry.getSize() > 0 && (entryName.Equals(transletFullName) || (entryName.EndsWith(".class", StringComparison.Ordinal) && entryName.StartsWith(transletAuxPrefix, StringComparison.Ordinal))))
				{
					try
					{
						  Stream input = jarFile.getInputStream(entry);
						  int size = (int)entry.getSize();
						  sbyte[] bytes = new sbyte[size];
						  readFromInputStream(bytes, input, size);
						  input.Close();
						  bytecodes.Add(bytes);
					}
					catch (IOException)
					{
						  return null;
					}
				}
			  }

			// Convert the Vector of byte[] to byte[][].
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = bytecodes.size();
			int count = bytecodes.Count;
			if (count > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[][] result = new byte[count][1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: sbyte[][] result = new sbyte[count][1];
				sbyte[][] result = RectangularArrays.RectangularSbyteArray(count, 1);
				for (int i = 0; i < count; i++)
				{
					result[i] = (sbyte[])bytecodes[i];
				}

				return result;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Read a given number of bytes from the InputStream into a byte array.
		/// </summary>
		/// <param name="bytes"> The byte array to store the input content. </param>
		/// <param name="input"> The input stream. </param>
		/// <param name="size"> The number of bytes to read. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readFromInputStream(byte[] bytes, java.io.InputStream input, int size) throws java.io.IOException
		private void readFromInputStream(sbyte[] bytes, Stream input, int size)
		{
		  int n = 0;
		  int offset = 0;
		  int length = size;
		  while (length > 0 && (n = input.Read(bytes, offset, length)) > 0)
		  {
			  offset = offset + n;
			  length = length - n;
		  }
		}

		/// <summary>
		/// Return the base class name of the translet.
		/// The translet name is resolved using the following rules:
		/// 1. if the _transletName attribute is set and its value is not "GregorSamsa",
		///    then _transletName is returned.
		/// 2. otherwise get the translet name from the base name of the system ID
		/// 3. return "GregorSamsa" if the result from step 2 is null.
		/// </summary>
		/// <param name="source"> The input Source </param>
		/// <returns> The name of the translet class </returns>
		private string getTransletBaseName(Source source)
		{
			string transletBaseName = null;
			if (!_transletName.Equals(DEFAULT_TRANSLET_NAME))
			{
				return _transletName;
			}
			  else
			  {
				string systemId = source.getSystemId();
				if (!string.ReferenceEquals(systemId, null))
				{
				  string baseName = Util.baseName(systemId);
			if (!string.ReferenceEquals(baseName, null))
			{
				baseName = Util.noExtName(baseName);
				transletBaseName = Util.toJavaName(baseName);
			}
				}
			  }

			return (!string.ReferenceEquals(transletBaseName, null)) ? transletBaseName : DEFAULT_TRANSLET_NAME;
		}

		/// <summary>
		///  Return the local file name from the systemId of the Source object
		/// </summary>
		/// <param name="source"> The Source </param>
		/// <returns> The file name in the local filesystem, or null if the
		/// systemId does not represent a local file. </returns>
		private string getStylesheetFileName(Source source)
		{
			string systemId = source.getSystemId();
			  if (!string.ReferenceEquals(systemId, null))
			  {
				File file = new File(systemId);
				if (file.exists())
				{
					return systemId;
				}
				else
				{
					  URL url = null;
				  try
				  {
						url = new URL(systemId);
				  }
				  catch (MalformedURLException)
				  {
						return null;
				  }

				  if ("file".Equals(url.getProtocol()))
				  {
						return url.getFile();
				  }
				  else
				  {
						return null;
				  }
				}
			  }
			  else
			  {
				return null;
			  }
		}

		/// <summary>
		/// Returns the Class object the provides the XSLTC DTM Manager service.
		/// </summary>
		protected internal virtual Type DTMManagerClass
		{
			get
			{
				return m_DTMManagerClass;
			}
		}
	}

}