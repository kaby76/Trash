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
 * $Id: TransformerHandlerImpl.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{

	using StripFilter = org.apache.xalan.xsltc.StripFilter;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using DOMWSFilter = org.apache.xalan.xsltc.dom.DOMWSFilter;
	using SAXImpl = org.apache.xalan.xsltc.dom.SAXImpl;
	using XSLTCDTMManager = org.apache.xalan.xsltc.dom.XSLTCDTMManager;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using DeclHandler = org.xml.sax.ext.DeclHandler;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;

	/// <summary>
	/// Implementation of a JAXP1.1 TransformerHandler
	/// @author Morten Jorgensen
	/// </summary>
	public class TransformerHandlerImpl : TransformerHandler, DeclHandler
	{

		private TransformerImpl _transformer;
		private AbstractTranslet _translet = null;
		private string _systemId;
		private SAXImpl _dom = null;
		private ContentHandler _handler = null;
		private LexicalHandler _lexHandler = null;
		private DTDHandler _dtdHandler = null;
		private DeclHandler _declHandler = null;
		private Result _result = null;
		private Locator _locator = null;

		private bool _done = false; // Set in endDocument()

		/// <summary>
		/// A flag indicating whether this transformer handler implements the 
		/// identity transform.
		/// </summary>
		private bool _isIdentity = false;

		/// <summary>
		/// Cosntructor - pass in reference to a TransformerImpl object
		/// </summary>
		public TransformerHandlerImpl(TransformerImpl transformer)
		{
		// Save the reference to the transformer
		_transformer = transformer;

		if (transformer.Identity)
		{
			// Set initial handler to the empty handler
			_handler = new DefaultHandler();
			_isIdentity = true;
		}
		else
		{
			// Get a reference to the translet wrapped inside the transformer
			_translet = _transformer.Translet;
		}
		}

		/// <summary>
		/// Implements javax.xml.transform.sax.TransformerHandler.getSystemId()
		/// Get the base ID (URI or system ID) from where relative URLs will be
		/// resolved. </summary>
		/// <returns> The systemID that was set with setSystemId(String id) </returns>
		public virtual string SystemId
		{
			get
			{
			return _systemId;
			}
			set
			{
			_systemId = value;
			}
		}


		/// <summary>
		/// Implements javax.xml.transform.sax.TransformerHandler.getTransformer()
		/// Get the Transformer associated with this handler, which is needed in
		/// order to set parameters and output properties. </summary>
		/// <returns> The Transformer object </returns>
		public virtual Transformer Transformer
		{
			get
			{
			return _transformer;
			}
		}

		/// <summary>
		/// Implements javax.xml.transform.sax.TransformerHandler.setResult()
		/// Enables the user of the TransformerHandler to set the to set the Result
		/// for the transformation. </summary>
		/// <param name="result"> A Result instance, should not be null </param>
		/// <exception cref="IllegalArgumentException"> if result is invalid for some reason </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setResult(javax.xml.transform.Result result) throws IllegalArgumentException
		public virtual Result Result
		{
			set
			{
			_result = value;
    
			if (null == value)
			{
			   ErrorMsg err = new ErrorMsg(ErrorMsg.ER_RESULT_NULL);
			   throw new System.ArgumentException(err.ToString()); //"result should not be null");
			}
    
			if (_isIdentity)
			{
				try
				{
				// Connect this object with output system directly
				SerializationHandler outputHandler = _transformer.getOutputHandler(value);
				_transformer.transferOutputProperties(outputHandler);
    
				_handler = outputHandler;
				_lexHandler = outputHandler;
				}
				catch (TransformerException)
				{
				_result = null;
				}
			}
			else if (_done)
			{
				// Run the transformation now, if not already done
				try
				{
				_transformer.DOM = _dom;
				_transformer.transform(null, _result);
				}
				catch (TransformerException e)
				{
				// What the hell are we supposed to do with this???
				throw new System.ArgumentException(e.Message);
				}
			}
			}
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.characters()
		/// Receive notification of character data.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public virtual void characters(char[] ch, int start, int length)
		{
		_handler.characters(ch, start, length);
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.startDocument()
		/// Receive notification of the beginning of a document.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public virtual void startDocument()
		{
		// Make sure setResult() was called before the first SAX event
		if (_result == null)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_SET_RESULT_ERR);
			throw new SAXException(err.ToString());
		}

			if (!_isIdentity)
			{
				bool hasIdCall = (_translet != null) ? _translet.hasIdCall() : false;
				XSLTCDTMManager dtmManager = null;

				// Create an internal DOM (not W3C) and get SAX2 input handler
				try
				{
					dtmManager = (XSLTCDTMManager)System.Activator.CreateInstance(_transformer.TransformerFactory.DTMManagerClass);
				}
				catch (Exception e)
				{
					throw new SAXException(e);
				}

				DTMWSFilter wsFilter;
				if (_translet != null && _translet is StripFilter)
				{
					wsFilter = new DOMWSFilter(_translet);
				}
				else
				{
					wsFilter = null;
				}

				// Construct the DTM using the SAX events that come through
				_dom = (SAXImpl)dtmManager.getDTM(null, false, wsFilter, true, false, hasIdCall);

				_handler = _dom.Builder;
				_lexHandler = (LexicalHandler) _handler;
				_dtdHandler = (DTDHandler) _handler;
				_declHandler = (DeclHandler) _handler;


				// Set document URI
				_dom.DocumentURI = _systemId;

				if (_locator != null)
				{
					_handler.setDocumentLocator(_locator);
				}
			}

		// Proxy call
		_handler.startDocument();
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.endDocument()
		/// Receive notification of the end of a document.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public virtual void endDocument()
		{
		// Signal to the DOMBuilder that the document is complete
		_handler.endDocument();

		if (!_isIdentity)
		{
			// Run the transformation now if we have a reference to a Result object
			if (_result != null)
			{
			try
			{
				_transformer.DOM = _dom;
				_transformer.transform(null, _result);
			}
			catch (TransformerException e)
			{
				throw new SAXException(e);
			}
			}
			// Signal that the internal DOM is built (see 'setResult()').
			_done = true;

			// Set this DOM as the transformer's DOM
			_transformer.DOM = _dom;
		}
		if (_isIdentity && _result is DOMResult)
		{
			((DOMResult)_result).setNode(_transformer.TransletOutputHandlerFactory.Node);
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.startElement()
		/// Receive notification of the beginning of an element.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qname, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
		public virtual void startElement(string uri, string localName, string qname, Attributes attributes)
		{
		_handler.startElement(uri, localName, qname, attributes);
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.endElement()
		/// Receive notification of the end of an element.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String namespaceURI, String localName, String qname) throws org.xml.sax.SAXException
		public virtual void endElement(string namespaceURI, string localName, string qname)
		{
		_handler.endElement(namespaceURI, localName, qname);
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.processingInstruction()
		/// Receive notification of a processing instruction.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public virtual void processingInstruction(string target, string data)
		{
		_handler.processingInstruction(target, data);
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.startCDATA()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public virtual void startCDATA()
		{
		if (_lexHandler != null)
		{
			_lexHandler.startCDATA();
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.endCDATA()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public virtual void endCDATA()
		{
		if (_lexHandler != null)
		{
			_lexHandler.endCDATA();
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.comment()
		/// Receieve notification of a comment
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public virtual void comment(char[] ch, int start, int length)
		{
		if (_lexHandler != null)
		{
			_lexHandler.comment(ch, start, length);
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.ignorableWhitespace()
		/// Receive notification of ignorable whitespace in element
		/// content. Similar to characters(char[], int, int).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(char[] ch, int start, int length) throws org.xml.sax.SAXException
		public virtual void ignorableWhitespace(char[] ch, int start, int length)
		{
		_handler.ignorableWhitespace(ch, start, length);
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.setDocumentLocator()
		/// Receive an object for locating the origin of SAX document events. 
		/// </summary>
		public virtual Locator DocumentLocator
		{
			set
			{
				_locator = value;
    
				if (_handler != null)
				{
					_handler.setDocumentLocator(value);
				}
			}
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.skippedEntity()
		/// Receive notification of a skipped entity.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
		public virtual void skippedEntity(string name)
		{
		_handler.skippedEntity(name);
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.startPrefixMapping()
		/// Begin the scope of a prefix-URI Namespace mapping.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public virtual void startPrefixMapping(string prefix, string uri)
		{
		_handler.startPrefixMapping(prefix, uri);
		}

		/// <summary>
		/// Implements org.xml.sax.ContentHandler.endPrefixMapping()
		/// End the scope of a prefix-URI Namespace mapping.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
		public virtual void endPrefixMapping(string prefix)
		{
		_handler.endPrefixMapping(prefix);
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.startDTD()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void startDTD(string name, string publicId, string systemId)
		{
		if (_lexHandler != null)
		{
			_lexHandler.startDTD(name, publicId, systemId);
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.endDTD()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public virtual void endDTD()
		{
		if (_lexHandler != null)
		{
			_lexHandler.endDTD();
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.startEntity()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startEntity(String name) throws org.xml.sax.SAXException
		public virtual void startEntity(string name)
		{
		if (_lexHandler != null)
		{
			_lexHandler.startEntity(name);
		}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.LexicalHandler.endEntity()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endEntity(String name) throws org.xml.sax.SAXException
		public virtual void endEntity(string name)
		{
		if (_lexHandler != null)
		{
			_lexHandler.endEntity(name);
		}
		}

		/// <summary>
		/// Implements org.xml.sax.DTDHandler.unparsedEntityDecl()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void unparsedEntityDecl(String name, String publicId, String systemId, String notationName) throws org.xml.sax.SAXException
		public virtual void unparsedEntityDecl(string name, string publicId, string systemId, string notationName)
		{
			if (_dtdHandler != null)
			{
			_dtdHandler.unparsedEntityDecl(name, publicId, systemId, notationName);
			}
		}

		/// <summary>
		/// Implements org.xml.sax.DTDHandler.notationDecl()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void notationDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void notationDecl(string name, string publicId, string systemId)
		{
			if (_dtdHandler != null)
			{
			_dtdHandler.notationDecl(name, publicId, systemId);
			}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.DeclHandler.attributeDecl()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void attributeDecl(String eName, String aName, String type, String valueDefault, String value) throws org.xml.sax.SAXException
		public virtual void attributeDecl(string eName, string aName, string type, string valueDefault, string value)
		{
			if (_declHandler != null)
			{
			_declHandler.attributeDecl(eName, aName, type, valueDefault, value);
			}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.DeclHandler.elementDecl()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void elementDecl(String name, String model) throws org.xml.sax.SAXException
		public virtual void elementDecl(string name, string model)
		{
			if (_declHandler != null)
			{
			_declHandler.elementDecl(name, model);
			}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.DeclHandler.externalEntityDecl()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void externalEntityDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void externalEntityDecl(string name, string publicId, string systemId)
		{
			if (_declHandler != null)
			{
			_declHandler.externalEntityDecl(name, publicId, systemId);
			}
		}

		/// <summary>
		/// Implements org.xml.sax.ext.DeclHandler.externalEntityDecl()
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void internalEntityDecl(String name, String value) throws org.xml.sax.SAXException
		public virtual void internalEntityDecl(string name, string value)
		{
			if (_declHandler != null)
			{
			_declHandler.internalEntityDecl(name, value);
			}
		}
	}

}