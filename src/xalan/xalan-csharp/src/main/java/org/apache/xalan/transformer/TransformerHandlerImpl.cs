using System;
using System.Threading;

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
 * $Id: TransformerHandlerImpl.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using IncrementalSAXSource_Filter = org.apache.xml.dtm.@ref.IncrementalSAXSource_Filter;
	using SAX2DTM = org.apache.xml.dtm.@ref.sax2dtm.SAX2DTM;
	using XPathContext = org.apache.xpath.XPathContext;

	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using EntityResolver = org.xml.sax.EntityResolver;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;
	using DeclHandler = org.xml.sax.ext.DeclHandler;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;


	/// <summary>
	/// A TransformerHandler
	/// listens for SAX ContentHandler parse events and transforms
	/// them to a Result.
	/// </summary>
	public class TransformerHandlerImpl : EntityResolver, DTDHandler, ContentHandler, ErrorHandler, LexicalHandler, TransformerHandler, DeclHandler
	{
		/// <summary>
		/// The flag for the setting of the optimize feature;
		/// </summary>
		private readonly bool m_optimizer;

		/// <summary>
		/// The flag for the setting of the incremental feature;
		/// </summary>
		private readonly bool m_incremental;

		/// <summary>
		/// The flag for the setting of the source_location feature;
		/// </summary>
		private readonly bool m_source_location;

	  private bool m_insideParse = false;

	  ////////////////////////////////////////////////////////////////////
	  // Constructors.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Construct a TransformerHandlerImpl.
	  /// </summary>
	  /// <param name="transformer"> Non-null reference to the Xalan transformer impl. </param>
	  /// <param name="doFragment"> True if the result should be a document fragement. </param>
	  /// <param name="baseSystemID">  The system ID to use as the base for relative URLs. </param>
	  public TransformerHandlerImpl(TransformerImpl transformer, bool doFragment, string baseSystemID) : base()
	  {


		m_transformer = transformer;
		m_baseSystemID = baseSystemID;

		XPathContext xctxt = transformer.XPathContext;
		DTM dtm = xctxt.getDTM(null, true, transformer, true, true);

		m_dtm = dtm;
		dtm.DocumentBaseURI = baseSystemID;

		m_contentHandler = dtm.ContentHandler;
		m_dtdHandler = dtm.DTDHandler;
		m_entityResolver = dtm.EntityResolver;
		m_errorHandler = dtm.ErrorHandler;
		m_lexicalHandler = dtm.LexicalHandler;
		m_incremental = transformer.Incremental;
		m_optimizer = transformer.Optimize;
		m_source_location = transformer.Source_location;
	  }

	  /// <summary>
	  /// Do what needs to be done to shut down the CoRoutine management.
	  /// </summary>
	  protected internal virtual void clearCoRoutine()
	  {
		clearCoRoutine(null);
	  }

	  /// <summary>
	  /// Do what needs to be done to shut down the CoRoutine management.
	  /// </summary>
	  protected internal virtual void clearCoRoutine(SAXException ex)
	  {
		if (null != ex)
		{
		  m_transformer.ExceptionThrown = ex;
		}

		if (m_dtm is SAX2DTM)
		{
		  if (DEBUG)
		  {
			Console.Error.WriteLine("In clearCoRoutine...");
		  }
		  try
		  {
			SAX2DTM sax2dtm = ((SAX2DTM)m_dtm);
			if (null != m_contentHandler && m_contentHandler is IncrementalSAXSource_Filter)
			{
			  IncrementalSAXSource_Filter sp = (IncrementalSAXSource_Filter)m_contentHandler;
			  // This should now be all that's needed.
			  sp.deliverMoreNodes(false);
			}

			sax2dtm.clearCoRoutine(true);
			m_contentHandler = null;
			m_dtdHandler = null;
			m_entityResolver = null;
			m_errorHandler = null;
			m_lexicalHandler = null;
		  }
		  catch (Exception throwable)
		  {
			Console.WriteLine(throwable.ToString());
			Console.Write(throwable.StackTrace);
		  }

		  if (DEBUG)
		  {
			Console.Error.WriteLine("...exiting clearCoRoutine");
		  }
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of javax.xml.transform.sax.TransformerHandler.
	  ////////////////////////////////////////////////////////////////////

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
			  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_RESULT_NULL, null)); //"result should not be null");
			}
    
			try
			{
		//      ContentHandler handler =
		//        m_transformer.createResultContentHandler(value);
		//      m_transformer.setContentHandler(handler);
				SerializationHandler xoh = m_transformer.createSerializationHandler(value);
				m_transformer.SerializationHandler = xoh;
			}
			catch (javax.xml.transform.TransformerException)
			{
			  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_RESULT_COULD_NOT_BE_SET, null)); //"result could not be set");
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
			m_baseSystemID = value;
			m_dtm.DocumentBaseURI = value;
		  }
		  get
		  {
			return m_baseSystemID;
		  }
	  }


	  /// <summary>
	  /// Get the Transformer associated with this handler, which
	  /// is needed in order to set parameters and output properties.
	  /// </summary>
	  /// <returns> The Transformer associated with this handler </returns>
	  public virtual Transformer Transformer
	  {
		  get
		  {
			return m_transformer;
		  }
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of org.xml.sax.EntityResolver.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Filter an external entity resolution.
	  /// </summary>
	  /// <param name="publicId"> The entity's public identifier, or null. </param>
	  /// <param name="systemId"> The entity's system identifier. </param>
	  /// <returns> A new InputSource or null for the default.
	  /// </returns>
	  /// <exception cref="IOException"> </exception>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <exception cref="java.io.IOException"> The client may throw an
	  ///            I/O-related exception while obtaining the
	  ///            new InputSource. </exception>
	  /// <seealso cref= org.xml.sax.EntityResolver#resolveEntity </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.InputSource resolveEntity(String publicId, String systemId) throws org.xml.sax.SAXException, java.io.IOException
	  public virtual InputSource resolveEntity(string publicId, string systemId)
	  {

		if (m_entityResolver != null)
		{
		  return m_entityResolver.resolveEntity(publicId, systemId);
		}
		else
		{
		  return null;
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of org.xml.sax.DTDHandler.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Filter a notation declaration event.
	  /// </summary>
	  /// <param name="name"> The notation name. </param>
	  /// <param name="publicId"> The notation's public identifier, or null. </param>
	  /// <param name="systemId"> The notation's system identifier, or null. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.DTDHandler#notationDecl </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notationDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
	  public virtual void notationDecl(string name, string publicId, string systemId)
	  {

		if (m_dtdHandler != null)
		{
		  m_dtdHandler.notationDecl(name, publicId, systemId);
		}
	  }

	  /// <summary>
	  /// Filter an unparsed entity declaration event.
	  /// </summary>
	  /// <param name="name"> The entity name. </param>
	  /// <param name="publicId"> The entity's public identifier, or null. </param>
	  /// <param name="systemId"> The entity's system identifier, or null. </param>
	  /// <param name="notationName"> The name of the associated notation. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unparsedEntityDecl(String name, String publicId, String systemId, String notationName) throws org.xml.sax.SAXException
	  public virtual void unparsedEntityDecl(string name, string publicId, string systemId, string notationName)
	  {

		if (m_dtdHandler != null)
		{
		  m_dtdHandler.unparsedEntityDecl(name, publicId, systemId, notationName);
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of org.xml.sax.ContentHandler.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Filter a new document locator event.
	  /// </summary>
	  /// <param name="locator"> The document locator. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator </seealso>
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
    
			if (DEBUG)
			{
			  Console.WriteLine("TransformerHandlerImpl#setDocumentLocator: " + value.SystemId);
			}
    
			this.m_locator = value;
    
			if (null == m_baseSystemID)
			{
			  SystemId = value.SystemId;
			}
    
			if (m_contentHandler != null)
			{
			  m_contentHandler.DocumentLocator = value;
			}
		  }
	  }

	  /// <summary>
	  /// Filter a start document event.
	  /// </summary>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#startDocument </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
	  public virtual void startDocument()
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#startDocument");
		}

		m_insideParse = true;

	   // Thread listener = new Thread(m_transformer);

		if (m_contentHandler != null)
		{
		  //m_transformer.setTransformThread(listener);
		  if (m_incremental)
		  {
			m_transformer.SourceTreeDocForThread = m_dtm.Document;

			int cpriority = Thread.CurrentThread.Priority;

			// runTransformThread is equivalent with the 2.0.1 code,
			// except that the Thread may come from a pool.
			org.apache.xalan.transformer.TransformerImpl.runTransformThread(cpriority);
		  }

		  // This is now done _last_, because IncrementalSAXSource_Filter
		  // will immediately go into a "wait until events are requested"
		  // pause. I believe that will close our timing window.
		  // %REVIEW%
		  m_contentHandler.startDocument();
		}

	   //listener.setDaemon(false);
	   //listener.start();

	  }

	  /// <summary>
	  /// Filter an end document event.
	  /// </summary>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#endDocument </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
	  public virtual void endDocument()
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#endDocument");
		}

		m_insideParse = false;

		if (m_contentHandler != null)
		{
		  m_contentHandler.endDocument();
		}

		if (m_incremental)
		{
		  m_transformer.waitTransformThread();
		}
		else
		{
		  m_transformer.SourceTreeDocForThread = m_dtm.Document;
		  m_transformer.run();
		}
	   /* Thread transformThread = m_transformer.getTransformThread();
	
	    if (null != transformThread)
	    {
	      try
	      {
	
	        // This should wait until the transformThread is considered not alive.
	        transformThread.join();
	
	        if (!m_transformer.hasTransformThreadErrorCatcher())
	        {
	          Exception e = m_transformer.getExceptionThrown();
	
	          if (null != e)
	            throw new org.xml.sax.SAXException(e);
	        }
	
	        m_transformer.setTransformThread(null);
	      }
	      catch (InterruptedException ie){}
	    }*/
	  }

	  /// <summary>
	  /// Filter a start Namespace prefix mapping event.
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix. </param>
	  /// <param name="uri"> The Namespace URI. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
	  public virtual void startPrefixMapping(string prefix, string uri)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#startPrefixMapping: " + prefix + ", " + uri);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.startPrefixMapping(prefix, uri);
		}
	  }

	  /// <summary>
	  /// Filter an end Namespace prefix mapping event.
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
	  public virtual void endPrefixMapping(string prefix)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#endPrefixMapping: " + prefix);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.endPrefixMapping(prefix);
		}
	  }

	  /// <summary>
	  /// Filter a start element event.
	  /// </summary>
	  /// <param name="uri"> The element's Namespace URI, or the empty string. </param>
	  /// <param name="localName"> The element's local name, or the empty string. </param>
	  /// <param name="qName"> The element's qualified (prefixed) name, or the empty
	  ///        string. </param>
	  /// <param name="atts"> The element's attributes. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#startElement </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
	  public virtual void startElement(string uri, string localName, string qName, Attributes atts)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#startElement: " + qName);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.startElement(uri, localName, qName, atts);
		}
	  }

	  /// <summary>
	  /// Filter an end element event.
	  /// </summary>
	  /// <param name="uri"> The element's Namespace URI, or the empty string. </param>
	  /// <param name="localName"> The element's local name, or the empty string. </param>
	  /// <param name="qName"> The element's qualified (prefixed) name, or the empty
	  ///        string. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#endElement </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String uri, String localName, String qName) throws org.xml.sax.SAXException
	  public virtual void endElement(string uri, string localName, string qName)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#endElement: " + qName);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.endElement(uri, localName, qName);
		}
	  }

	  /// <summary>
	  /// Filter a character data event.
	  /// </summary>
	  /// <param name="ch"> An array of characters. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#characters </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#characters: " + start + ", " + length);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.characters(ch, start, length);
		}
	  }

	  /// <summary>
	  /// Filter an ignorable whitespace event.
	  /// </summary>
	  /// <param name="ch"> An array of characters. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void ignorableWhitespace(char[] ch, int start, int length)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#ignorableWhitespace: " + start + ", " + length);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.ignorableWhitespace(ch, start, length);
		}
	  }

	  /// <summary>
	  /// Filter a processing instruction event.
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The text following the target. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#processingInstruction </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#processingInstruction: " + target + ", " + data);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.processingInstruction(target, data);
		}
	  }

	  /// <summary>
	  /// Filter a skipped entity event.
	  /// </summary>
	  /// <param name="name"> The name of the skipped entity. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#skippedEntity </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
	  public virtual void skippedEntity(string name)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#skippedEntity: " + name);
		}

		if (m_contentHandler != null)
		{
		  m_contentHandler.skippedEntity(name);
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of org.xml.sax.ErrorHandler.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Filter a warning event.
	  /// </summary>
	  /// <param name="e"> The nwarning as an exception. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ErrorHandler#warning </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
	  public virtual void warning(SAXParseException e)
	  {
		// This is not great, but we really would rather have the error 
		// handler be the error listener if it is a error handler.  Coroutine's fatalError 
		// can't really be configured, so I think this is the best thing right now 
		// for error reporting.  Possibly another JAXP 1.1 hole.  -sb
		javax.xml.transform.ErrorListener errorListener = m_transformer.ErrorListener;
		if (errorListener is ErrorHandler)
		{
		  ((ErrorHandler)errorListener).warning(e);
		}
		else
		{
		  try
		  {
			errorListener.warning(new javax.xml.transform.TransformerException(e));
		  }
		  catch (javax.xml.transform.TransformerException)
		  {
			throw e;
		  }
		}
	  }

	  /// <summary>
	  /// Filter an error event.
	  /// </summary>
	  /// <param name="e"> The error as an exception. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ErrorHandler#error </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
	  public virtual void error(SAXParseException e)
	  {
		// %REVIEW% I don't think this should be called.  -sb
		// clearCoRoutine(e);

		// This is not great, but we really would rather have the error 
		// handler be the error listener if it is a error handler.  Coroutine's fatalError 
		// can't really be configured, so I think this is the best thing right now 
		// for error reporting.  Possibly another JAXP 1.1 hole.  -sb
		javax.xml.transform.ErrorListener errorListener = m_transformer.ErrorListener;
		if (errorListener is ErrorHandler)
		{
		  ((ErrorHandler)errorListener).error(e);
		  if (null != m_errorHandler)
		  {
			m_errorHandler.error(e); // may not be called.
		  }
		}
		else
		{
		  try
		  {
			errorListener.error(new javax.xml.transform.TransformerException(e));
			if (null != m_errorHandler)
			{
			  m_errorHandler.error(e); // may not be called.
			}
		  }
		  catch (javax.xml.transform.TransformerException)
		  {
			throw e;
		  }
		}
	  }

	  /// <summary>
	  /// Filter a fatal error event.
	  /// </summary>
	  /// <param name="e"> The error as an exception. </param>
	  /// <exception cref="SAXException"> The client may throw
	  ///            an exception during processing. </exception>
	  /// <seealso cref= org.xml.sax.ErrorHandler#fatalError </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
	  public virtual void fatalError(SAXParseException e)
	  {
		if (null != m_errorHandler)
		{
		  try
		  {
			m_errorHandler.fatalError(e);
		  }
		  catch (SAXParseException)
		  {
			// ignore
		  }
		  // clearCoRoutine(e);
		}

		// This is not great, but we really would rather have the error 
		// handler be the error listener if it is a error handler.  Coroutine's fatalError 
		// can't really be configured, so I think this is the best thing right now 
		// for error reporting.  Possibly another JAXP 1.1 hole.  -sb
		javax.xml.transform.ErrorListener errorListener = m_transformer.ErrorListener;

		if (errorListener is ErrorHandler)
		{
		  ((ErrorHandler)errorListener).fatalError(e);
		  if (null != m_errorHandler)
		  {
			m_errorHandler.fatalError(e); // may not be called.
		  }
		}
		else
		{
		  try
		  {
			errorListener.fatalError(new javax.xml.transform.TransformerException(e));
			if (null != m_errorHandler)
			{
			  m_errorHandler.fatalError(e); // may not be called.
			}
		  }
		  catch (javax.xml.transform.TransformerException)
		  {
			throw e;
		  }
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of org.xml.sax.ext.LexicalHandler.
	  ////////////////////////////////////////////////////////////////////

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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#startDTD: " + name + ", " + publicId + ", " + systemId);
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.startDTD(name, publicId, systemId);
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#endDTD");
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.endDTD();
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#startEntity: " + name);
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.startEntity(name);
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#endEntity: " + name);
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.endEntity(name);
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#startCDATA");
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.startCDATA();
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#endCDATA");
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.endCDATA();
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#comment: " + start + ", " + length);
		}

		if (null != m_lexicalHandler)
		{
		  m_lexicalHandler.comment(ch, start, length);
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of org.xml.sax.ext.DeclHandler.
	  ////////////////////////////////////////////////////////////////////

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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#elementDecl: " + name + ", " + model);
		}

		if (null != m_declHandler)
		{
		  m_declHandler.elementDecl(name, model);
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#attributeDecl: " + eName + ", " + aName + ", etc...");
		}

		if (null != m_declHandler)
		{
		  m_declHandler.attributeDecl(eName, aName, type, valueDefault, value);
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#internalEntityDecl: " + name + ", " + value);
		}

		if (null != m_declHandler)
		{
		  m_declHandler.internalEntityDecl(name, value);
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

		if (DEBUG)
		{
		  Console.WriteLine("TransformerHandlerImpl#externalEntityDecl: " + name + ", " + publicId + ", " + systemId);
		}

		if (null != m_declHandler)
		{
		  m_declHandler.externalEntityDecl(name, publicId, systemId);
		}
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Internal state.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Set to true for diagnostics output. </summary>
	  private static bool DEBUG = false;

	  /// <summary>
	  /// The transformer this will use to transform a
	  /// source tree into a result tree.
	  /// </summary>
	  private TransformerImpl m_transformer;

	  /// <summary>
	  /// The system ID to use as a base for relative URLs. </summary>
	  private string m_baseSystemID;

	  /// <summary>
	  /// The result for the transformation. </summary>
	  private Result m_result = null;

	  /// <summary>
	  /// The locator for this TransformerHandler. </summary>
	  private Locator m_locator = null;

	  /// <summary>
	  /// The entity resolver to aggregate to. </summary>
	  private EntityResolver m_entityResolver = null;

	  /// <summary>
	  /// The DTD handler to aggregate to. </summary>
	  private DTDHandler m_dtdHandler = null;

	  /// <summary>
	  /// The content handler to aggregate to. </summary>
	  private ContentHandler m_contentHandler = null;

	  /// <summary>
	  /// The error handler to aggregate to. </summary>
	  private ErrorHandler m_errorHandler = null;

	  /// <summary>
	  /// The lexical handler to aggregate to. </summary>
	  private LexicalHandler m_lexicalHandler = null;

	  /// <summary>
	  /// The decl handler to aggregate to. </summary>
	  private DeclHandler m_declHandler = null;

	  /// <summary>
	  /// The Document Table Instance we are transforming. </summary>
	  internal DTM m_dtm;
	}

}