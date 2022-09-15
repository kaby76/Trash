using System.Collections;
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
 * $Id: TemplatesHandlerImpl.java 577935 2007-09-20 21:35:20Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{

	using CompilerException = org.apache.xalan.xsltc.compiler.CompilerException;
	using Parser = org.apache.xalan.xsltc.compiler.Parser;
	using SourceLoader = org.apache.xalan.xsltc.compiler.SourceLoader;
	using Stylesheet = org.apache.xalan.xsltc.compiler.Stylesheet;
	using SyntaxTreeNode = org.apache.xalan.xsltc.compiler.SyntaxTreeNode;
	using XSLTC = org.apache.xalan.xsltc.compiler.XSLTC;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;

	using ContentHandler = org.xml.sax.ContentHandler;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// Implementation of a JAXP1.1 TemplatesHandler
	/// @author Morten Jorgensen
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class TemplatesHandlerImpl : ContentHandler, TemplatesHandler, SourceLoader
	{
		/// <summary>
		/// System ID for this stylesheet.
		/// </summary>
		private string _systemId;

		/// <summary>
		/// Number of spaces to add for output indentation.
		/// </summary>
		private int _indentNumber;

		/// <summary>
		/// This URIResolver is passed to all Transformers.
		/// </summary>
		private URIResolver _uriResolver = null;

		/// <summary>
		/// A reference to the transformer factory that this templates
		/// object belongs to.
		/// </summary>
		private TransformerFactoryImpl _tfactory = null;

		/// <summary>
		/// A reference to XSLTC's parser object.
		/// </summary>
		private Parser _parser = null;

		/// <summary>
		/// The created Templates object.
		/// </summary>
		private TemplatesImpl _templates = null;

		/// <summary>
		/// Default constructor
		/// </summary>
		protected internal TemplatesHandlerImpl(int indentNumber, TransformerFactoryImpl tfactory)
		{
		_indentNumber = indentNumber;
		_tfactory = tfactory;

			// Instantiate XSLTC and get reference to parser object
			XSLTC xsltc = new XSLTC();
			if (tfactory.getFeature(XMLConstants.FEATURE_SECURE_PROCESSING))
			{
				xsltc.SecureProcessing = true;
			}

			if ("true".Equals(tfactory.getAttribute(TransformerFactoryImpl.ENABLE_INLINING)))
			{
				xsltc.TemplateInlining = true;
			}
			else
			{
				xsltc.TemplateInlining = false;
			}

			_parser = xsltc.Parser;
		}

		/// <summary>
		/// Implements javax.xml.transform.sax.TemplatesHandler.getSystemId()
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
		/// Store URIResolver needed for Transformers.
		/// </summary>
		public virtual URIResolver URIResolver
		{
			set
			{
			_uriResolver = value;
			}
		}

		/// <summary>
		/// Implements javax.xml.transform.sax.TemplatesHandler.getTemplates()
		/// When a TemplatesHandler object is used as a ContentHandler or
		/// DocumentHandler for the parsing of transformation instructions, it
		/// creates a Templates object, which the caller can get once the SAX
		/// events have been completed. </summary>
		/// <returns> The Templates object that was created during the SAX event
		///         process, or null if no Templates object has been created. </returns>
		public virtual Templates Templates
		{
			get
			{
				return _templates;
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
			// A _uriResolver must be set if this method is called
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Source source = _uriResolver.resolve(href, context);
			Source source = _uriResolver.resolve(href, context);
			if (source != null)
			{
			return Util.getInputSource(xsltc, source);
			}
		}
		catch (TransformerException)
		{
			// Falls through
		}
		return null;
		}

		// -- ContentHandler --------------------------------------------------

		/// <summary>
		/// Re-initialize parser and forward SAX2 event.
		/// </summary>
		public virtual void startDocument()
		{
			XSLTC xsltc = _parser.XSLTC;
			xsltc.init(); // calls _parser.init()
			xsltc.OutputType = XSLTC.BYTEARRAY_OUTPUT;
			_parser.startDocument();
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public virtual void endDocument()
		{
			_parser.endDocument();

			// create the templates
			try
			{
				XSLTC xsltc = _parser.XSLTC;

				// Set the translet class name if not already set
				string transletName;
				if (!string.ReferenceEquals(_systemId, null))
				{
					transletName = Util.baseName(_systemId);
				}
				else
				{
					transletName = (string)_tfactory.getAttribute("translet-name");
				}
				xsltc.ClassName = transletName;

				// Get java-legal class name from XSLTC module
				transletName = xsltc.ClassName;

				Stylesheet stylesheet = null;
				SyntaxTreeNode root = _parser.DocumentRoot;

				// Compile the translet - this is where the work is done!
				if (!_parser.errorsFound() && root != null)
				{
					// Create a Stylesheet element from the root node
					stylesheet = _parser.makeStylesheet(root);
					stylesheet.SystemId = _systemId;
					stylesheet.ParentStylesheet = null;

					if (xsltc.TemplateInlining)
					{
					   stylesheet.TemplateInlining = true;
					}
					else
					{
					   stylesheet.TemplateInlining = false;
					}

					// Set a document loader (for xsl:include/import) if defined
					if (_uriResolver != null)
					{
						stylesheet.SourceLoader = this;
					}

					_parser.CurrentStylesheet = stylesheet;

					// Set it as top-level in the XSLTC object
					xsltc.Stylesheet = stylesheet;

					// Create AST under the Stylesheet element
					_parser.createAST(stylesheet);
				}

				// Generate the bytecodes and output the translet class(es)
				if (!_parser.errorsFound() && stylesheet != null)
				{
					stylesheet.MultiDocument = xsltc.MultiDocument;
					stylesheet.HasIdCall = xsltc.hasIdCall();

					// Class synchronization is needed for BCEL
					lock (xsltc.GetType())
					{
						stylesheet.translate();
					}
				}

				if (!_parser.errorsFound())
				{
					// Check that the transformation went well before returning
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[][] bytecodes = xsltc.getBytecodes();
					sbyte[][] bytecodes = xsltc.Bytecodes;
					if (bytecodes != null)
					{
						_templates = new TemplatesImpl(xsltc.Bytecodes, transletName, _parser.OutputProperties, _indentNumber, _tfactory);

						// Set URIResolver on templates object
						if (_uriResolver != null)
						{
							_templates.URIResolver = _uriResolver;
						}
					}
				}
				else
				{
					StringBuilder errorMessage = new StringBuilder();
					ArrayList errors = _parser.Errors;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = errors.size();
					int count = errors.Count;
					for (int i = 0; i < count; i++)
					{
						if (errorMessage.Length > 0)
						{
							errorMessage.Append('\n');
						}
						errorMessage.Append(errors[i].ToString());
					}
					throw new SAXException(ErrorMsg.JAXP_COMPILE_ERR, new TransformerException(errorMessage.ToString()));
				}
			}
			catch (CompilerException e)
			{
				throw new SAXException(ErrorMsg.JAXP_COMPILE_ERR, e);
			}
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void startPrefixMapping(string prefix, string uri)
		{
			_parser.startPrefixMapping(prefix, uri);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void endPrefixMapping(string prefix)
		{
			_parser.endPrefixMapping(prefix);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String uri, String localname, String qname, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
		public virtual void startElement(string uri, string localname, string qname, Attributes attributes)
		{
			_parser.startElement(uri, localname, qname, attributes);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void endElement(string uri, string localname, string qname)
		{
			_parser.endElement(uri, localname, qname);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void characters(char[] ch, int start, int length)
		{
			_parser.characters(ch, start, length);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void processingInstruction(string name, string value)
		{
			_parser.processingInstruction(name, value);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void ignorableWhitespace(char[] ch, int start, int length)
		{
			_parser.ignorableWhitespace(ch, start, length);
		}

		/// <summary>
		/// Just forward SAX2 event to parser object.
		/// </summary>
		public virtual void skippedEntity(string name)
		{
			_parser.skippedEntity(name);
		}

		/// <summary>
		/// Set internal system Id and forward SAX2 event to parser object.
		/// </summary>
		public virtual Locator DocumentLocator
		{
			set
			{
				SystemId = value.getSystemId();
				_parser.DocumentLocator = value;
			}
		}
	}



}