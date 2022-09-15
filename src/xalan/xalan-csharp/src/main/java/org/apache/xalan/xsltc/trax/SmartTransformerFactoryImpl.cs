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
 * $Id: SmartTransformerFactoryImpl.java 468653 2006-10-28 07:07:05Z minchau $
 */


namespace org.apache.xalan.xsltc.trax
{

	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using XMLFilter = org.xml.sax.XMLFilter;

	/// <summary>
	/// Implementation of a transformer factory that uses an XSLTC 
	/// transformer factory for the creation of Templates objects
	/// and uses the Xalan processor transformer factory for the
	/// creation of Transformer objects.  
	/// @author G. Todd Miller 
	/// </summary>
	public class SmartTransformerFactoryImpl : SAXTransformerFactory
	{
		/// <summary>
		/// <para>Name of class as a constant to use for debugging.</para>
		/// </summary>
		private const string CLASS_NAME = "SmartTransformerFactoryImpl";

		private SAXTransformerFactory _xsltcFactory = null;
		private SAXTransformerFactory _xalanFactory = null;
		private SAXTransformerFactory _currFactory = null;
		private ErrorListener _errorlistener = null;
		private URIResolver _uriresolver = null;

		/// <summary>
		/// <para>State of secure processing feature.</para>
		/// </summary>
		private bool featureSecureProcessing = false;

		/// <summary>
		/// implementation of the SmartTransformerFactory. This factory
		/// uses org.apache.xalan.xsltc.trax.TransformerFactory
		/// to return Templates objects; and uses 
		/// org.apache.xalan.processor.TransformerFactory
		/// to return Transformer objects.  
		/// </summary>
		public SmartTransformerFactoryImpl()
		{
		}

		private void createXSLTCTransformerFactory()
		{
		_xsltcFactory = new TransformerFactoryImpl();
		_currFactory = _xsltcFactory;
		}

		private void createXalanTransformerFactory()
		{
		 const string xalanMessage = "org.apache.xalan.xsltc.trax.SmartTransformerFactoryImpl " + "could not create an " + "org.apache.xalan.processor.TransformerFactoryImpl.";
		// try to create instance of Xalan factory...	
		try
		{
				Type xalanFactClass = ObjectFactory.findProviderClass("org.apache.xalan.processor.TransformerFactoryImpl", ObjectFactory.findClassLoader(), true);
			_xalanFactory = (SAXTransformerFactory) System.Activator.CreateInstance(xalanFactClass);
		}
		catch (ClassNotFoundException)
		{
			Console.Error.WriteLine(xalanMessage);
		}
		 catch (InstantiationException)
		 {
			Console.Error.WriteLine(xalanMessage);
		 }
		 catch (IllegalAccessException)
		 {
			Console.Error.WriteLine(xalanMessage);
		 }
		_currFactory = _xalanFactory;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setErrorListener(javax.xml.transform.ErrorListener listener) throws IllegalArgumentException
		public virtual ErrorListener ErrorListener
		{
			set
			{
			_errorlistener = value;
			}
			get
			{
			return _errorlistener;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object getAttribute(String name) throws IllegalArgumentException
		public virtual object getAttribute(string name)
		{
		// GTM: NB: 'debug' should change to something more unique... 
		if ((name.Equals("translet-name")) || (name.Equals("debug")))
		{
			if (_xsltcFactory == null)
			{
					createXSLTCTransformerFactory();
			}
				return _xsltcFactory.getAttribute(name);
		}
			else
			{
			if (_xalanFactory == null)
			{
				createXalanTransformerFactory();
			}
			return _xalanFactory.getAttribute(name);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setAttribute(String name, Object value) throws IllegalArgumentException
		public virtual void setAttribute(string name, object value)
		{
		// GTM: NB: 'debug' should change to something more unique... 
		if ((name.Equals("translet-name")) || (name.Equals("debug")))
		{
			if (_xsltcFactory == null)
			{
					createXSLTCTransformerFactory();
			}
				_xsltcFactory.setAttribute(name, value);
		}
			else
			{
			if (_xalanFactory == null)
			{
				createXalanTransformerFactory();
			}
			_xalanFactory.setAttribute(name, value);
			}
		}

		/// <summary>
		/// <para>Set a feature for this <code>SmartTransformerFactory</code> and <code>Transformer</code>s
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
			featureSecureProcessing = value;
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
			string[] features = new string[] {DOMSource.FEATURE, DOMResult.FEATURE, SAXSource.FEATURE, SAXResult.FEATURE, StreamSource.FEATURE, StreamResult.FEATURE};

		// feature name cannot be null
		if (string.ReferenceEquals(name, null))
		{
				ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_GET_FEATURE_NULL_NAME);
				throw new System.NullReferenceException(err.ToString());
		}

		// Inefficient, but it really does not matter in a function like this
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
			return featureSecureProcessing;
		}

		// unknown feature
			return false;
		}

		public virtual URIResolver URIResolver
		{
			get
			{
			return _uriresolver;
			}
			set
			{
			_uriresolver = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Source getAssociatedStylesheet(javax.xml.transform.Source source, String media, String title, String charset) throws javax.xml.transform.TransformerConfigurationException
		public virtual Source getAssociatedStylesheet(Source source, string media, string title, string charset)
		{
		if (_currFactory == null)
		{
				createXSLTCTransformerFactory();
		}
		return _currFactory.getAssociatedStylesheet(source, media, title, charset);
		}

		/// <summary>
		/// Create a Transformer object that copies the input document to the
		/// result. Uses the org.apache.xalan.processor.TransformerFactory. </summary>
		/// <returns> A Transformer object. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Transformer newTransformer() throws javax.xml.transform.TransformerConfigurationException
		public virtual Transformer newTransformer()
		{
		if (_xalanFactory == null)
		{
				createXalanTransformerFactory();
		}
		if (_errorlistener != null)
		{
			_xalanFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xalanFactory.setURIResolver(_uriresolver);
		}
		 _currFactory = _xalanFactory;
		return _currFactory.newTransformer();
		}

		/// <summary>
		/// Create a Transformer object that from the input stylesheet 
		/// Uses the org.apache.xalan.processor.TransformerFactory. </summary>
		/// <param name="source"> the stylesheet. </param>
		/// <returns> A Transformer object. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Transformer newTransformer(javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
		public virtual Transformer newTransformer(Source source)
		{
			if (_xalanFactory == null)
			{
				createXalanTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xalanFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xalanFactory.setURIResolver(_uriresolver);
		}
		 _currFactory = _xalanFactory;
		return _currFactory.newTransformer(source);
		}

		/// <summary>
		/// Create a Templates object that from the input stylesheet 
		/// Uses the org.apache.xalan.xsltc.trax.TransformerFactory. </summary>
		/// <param name="source"> the stylesheet. </param>
		/// <returns> A Templates object. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.Templates newTemplates(javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
		public virtual Templates newTemplates(Source source)
		{
			if (_xsltcFactory == null)
			{
				createXSLTCTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xsltcFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xsltcFactory.setURIResolver(_uriresolver);
		}
		 _currFactory = _xsltcFactory;
		return _currFactory.newTemplates(source);
		}

		/// <summary>
		/// Get a TemplatesHandler object that can process SAX ContentHandler
		/// events into a Templates object. Uses the
		/// org.apache.xalan.xsltc.trax.TransformerFactory.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TemplatesHandler newTemplatesHandler() throws javax.xml.transform.TransformerConfigurationException
		public virtual TemplatesHandler newTemplatesHandler()
		{
			if (_xsltcFactory == null)
			{
				createXSLTCTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xsltcFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xsltcFactory.setURIResolver(_uriresolver);
		}
		return _xsltcFactory.newTemplatesHandler();
		}

		/// <summary>
		/// Get a TransformerHandler object that can process SAX ContentHandler
		/// events based on a copy transformer. 
		/// Uses org.apache.xalan.processor.TransformerFactory. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler() throws javax.xml.transform.TransformerConfigurationException
		public virtual TransformerHandler newTransformerHandler()
		{
			if (_xalanFactory == null)
			{
				createXalanTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xalanFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xalanFactory.setURIResolver(_uriresolver);
		}
		return _xalanFactory.newTransformerHandler();
		}

		/// <summary>
		/// Get a TransformerHandler object that can process SAX ContentHandler
		/// events based on a transformer specified by the stylesheet Source. 
		/// Uses org.apache.xalan.processor.TransformerFactory. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler(javax.xml.transform.Source src) throws javax.xml.transform.TransformerConfigurationException
		public virtual TransformerHandler newTransformerHandler(Source src)
		{
			if (_xalanFactory == null)
			{
				createXalanTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xalanFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xalanFactory.setURIResolver(_uriresolver);
		}
		return _xalanFactory.newTransformerHandler(src);
		}


		/// <summary>
		/// Get a TransformerHandler object that can process SAX ContentHandler
		/// events based on a transformer specified by the stylesheet Source. 
		/// Uses org.apache.xalan.xsltc.trax.TransformerFactory. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public javax.xml.transform.sax.TransformerHandler newTransformerHandler(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
		public virtual TransformerHandler newTransformerHandler(Templates templates)
		{
			if (_xsltcFactory == null)
			{
				createXSLTCTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xsltcFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xsltcFactory.setURIResolver(_uriresolver);
		}
			return _xsltcFactory.newTransformerHandler(templates);
		}


		/// <summary>
		/// Create an XMLFilter that uses the given source as the
		/// transformation instructions. Uses
		/// org.apache.xalan.xsltc.trax.TransformerFactory.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.xml.sax.XMLFilter newXMLFilter(javax.xml.transform.Source src) throws javax.xml.transform.TransformerConfigurationException
		public virtual XMLFilter newXMLFilter(Source src)
		{
			if (_xsltcFactory == null)
			{
				createXSLTCTransformerFactory();
			}
		if (_errorlistener != null)
		{
			_xsltcFactory.setErrorListener(_errorlistener);
		}
		if (_uriresolver != null)
		{
			_xsltcFactory.setURIResolver(_uriresolver);
		}
		Templates templates = _xsltcFactory.newTemplates(src);
		if (templates == null)
		{
			return null;
		}
		return newXMLFilter(templates);
		}

		/*
		 * Create an XMLFilter that uses the given source as the
		 * transformation instructions. Uses
		 * org.apache.xalan.xsltc.trax.TransformerFactory.
		 */
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
				if (_xsltcFactory == null)
				{
					createXSLTCTransformerFactory();
				}
			ErrorListener errorListener = _xsltcFactory.getErrorListener();
				if (errorListener != null)
				{
					try
					{
						errorListener.fatalError(e1);
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
	}

}