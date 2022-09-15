using System;
using System.Collections;
using System.Collections.Generic;
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
 * $Id: ToStream.java 1225444 2011-12-29 05:52:39Z mrglavas $
 */
namespace org.apache.xml.serializer
{


	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using WrappedRuntimeException = org.apache.xml.serializer.utils.WrappedRuntimeException;
	using Node = org.w3c.dom.Node;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This abstract class is a base class for other stream 
	/// serializers (xml, html, text ...) that write output to a stream.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public abstract class ToStream : SerializerBase
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			m_lineSepLen = m_lineSep.Length;
		}


		private const string COMMENT_BEGIN = "<!--";
		private const string COMMENT_END = "-->";

		/// <summary>
		/// Stack to keep track of disabling output escaping. </summary>
		protected internal BoolStack m_disableOutputEscapingStates = new BoolStack();


		/// <summary>
		/// The encoding information associated with this serializer.
		/// Although initially there is no encoding,
		/// there is a dummy EncodingInfo object that will say
		/// that every character is in the encoding. This is useful
		/// for a serializer that is in temporary output state and has
		/// no associated encoding. A serializer in final output state
		/// will have an encoding, and will worry about whether 
		/// single chars or surrogate pairs of high/low chars form
		/// characters in the output encoding. 
		/// </summary>
		internal EncodingInfo m_encodingInfo = new EncodingInfo(null,null, '\u0000');

		/// <summary>
		/// Stack to keep track of whether or not we need to
		/// preserve whitespace.
		/// 
		/// Used to push/pop values used for the field m_ispreserve, but
		/// m_ispreserve is only relevant if m_doIndent is true.
		/// If m_doIndent is false this field has no impact.
		/// 
		/// </summary>
		protected internal BoolStack m_preserves = new BoolStack();

		/// <summary>
		/// State flag to tell if preservation of whitespace
		/// is important. 
		/// 
		/// Used only in shouldIndent() but only if m_doIndent is true.
		/// If m_doIndent is false this flag has no impact.
		/// 
		/// </summary>
		protected internal bool m_ispreserve = false;

		/// <summary>
		/// State flag that tells if the previous node processed
		/// was text, so we can tell if we should preserve whitespace.
		/// 
		/// Used in endDocument() and shouldIndent() but
		/// only if m_doIndent is true. 
		/// If m_doIndent is false this flag has no impact.
		/// </summary>
		protected internal bool m_isprevtext = false;

		private static readonly char[] s_systemLineSep;
		static ToStream()
		{
			s_systemLineSep = SecuritySupport.getSystemProperty("line.separator").ToCharArray();
		}

		/// <summary>
		/// The system line separator for writing out line breaks.
		/// The default value is from the system property,
		/// but this value can be set through the xsl:output
		/// extension attribute xalan:line-separator.
		/// </summary>
		protected internal char[] m_lineSep = s_systemLineSep;


		/// <summary>
		/// True if the the system line separator is to be used.
		/// </summary>
		protected internal bool m_lineSepUse = true;

		/// <summary>
		/// The length of the line seperator, since the write is done
		/// one character at a time.
		/// </summary>
		protected internal int m_lineSepLen;

		/// <summary>
		/// Map that tells which characters should have special treatment, and it
		///  provides character to entity name lookup.
		/// </summary>
		protected internal CharInfo m_charInfo;

		/// <summary>
		/// True if we control the buffer, and we should flush the output on endDocument. </summary>
		internal bool m_shouldFlush = true;

		/// <summary>
		/// Add space before '/>' for XHTML.
		/// </summary>
		protected internal bool m_spaceBeforeClose = false;

		/// <summary>
		/// Flag to signal that a newline should be added.
		/// 
		/// Used only in indent() which is called only if m_doIndent is true.
		/// If m_doIndent is false this flag has no impact.
		/// </summary>
		internal bool m_startNewLine;

		/// <summary>
		/// Tells if we're in an internal document type subset.
		/// </summary>
		protected internal bool m_inDoctype = false;

		/// <summary>
		/// Flag to quickly tell if the encoding is UTF8.
		/// </summary>
		internal bool m_isUTF8 = false;


		/// <summary>
		/// remembers if we are in between the startCDATA() and endCDATA() callbacks
		/// </summary>
		protected internal bool m_cdataStartCalled = false;

		/// <summary>
		/// If this flag is true DTD entity references are not left as-is,
		/// which is exiting older behavior.
		/// </summary>
		private bool m_expandDTDEntities = true;


		/// <summary>
		/// Default constructor
		/// </summary>
		public ToStream()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// This helper method to writes out "]]>" when closing a CDATA section.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void closeCDATA() throws org.xml.sax.SAXException
		protected internal virtual void closeCDATA()
		{
			try
			{
				m_writer.write(CDATA_DELIMITER_CLOSE);
				// write out a CDATA section closing "]]>"
				m_cdataTagOpen = false; // Remember that we have done so.
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}
		}

		/// <summary>
		/// Serializes the DOM node. Throws an exception only if an I/O
		/// exception occured while serializing.
		/// </summary>
		/// <param name="node"> Node to serialize. </param>
		/// <exception cref="IOException"> An I/O exception occured while serializing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
		public override void serialize(Node node)
		{

			try
			{
				TreeWalker walker = new TreeWalker(this);

				walker.traverse(node);
			}
			catch (SAXException se)
			{
				throw new WrappedRuntimeException(se);
			}
		}

		/// <summary>
		/// Taken from XSLTC 
		/// </summary>
		protected internal bool m_escaping = true;

		/// <summary>
		/// Flush the formatter's result stream.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final void flushWriter() throws org.xml.sax.SAXException
		protected internal void flushWriter()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			Writer writer = m_writer;
			if (null != writer)
			{
				try
				{
					if (writer is WriterToUTF8Buffered)
					{
						if (m_shouldFlush)
						{
							 ((WriterToUTF8Buffered) writer).flush();
						}
						else
						{
							 ((WriterToUTF8Buffered) writer).flushBuffer();
						}
					}
					if (writer is WriterToASCI)
					{
						if (m_shouldFlush)
						{
							writer.flush();
						}
					}
					else
					{
						// Flush always. 
						// Not a great thing if the writer was created 
						// by this class, but don't have a choice.
						writer.flush();
					}
				}
				catch (IOException ioe)
				{
					throw new SAXException(ioe);
				}
			}
		}

		internal Stream m_outputStream;
		/// <summary>
		/// Get the output stream where the events will be serialized to.
		/// </summary>
		/// <returns> reference to the result stream, or null of only a writer was
		/// set. </returns>
		public override Stream OutputStream
		{
			get
			{
				return m_outputStream;
			}
			set
			{
				setOutputStreamInternal(value, true);
			}
		}

		// Implement DeclHandler

		/// <summary>
		///   Report an element type declaration.
		/// 
		///   <para>The content model will consist of the string "EMPTY", the
		///   string "ANY", or a parenthesised group, optionally followed
		///   by an occurrence indicator.  The model will be normalized so
		///   that all whitespace is removed,and will include the enclosing
		///   parentheses.</para>
		/// </summary>
		///   <param name="name"> The element type name. </param>
		///   <param name="model"> The content model as a normalized string. </param>
		///   <exception cref="SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void elementDecl(String name, String model) throws org.xml.sax.SAXException
		public virtual void elementDecl(string name, string model)
		{
			// Do not inline external DTD
			if (m_inExternalDTD)
			{
				return;
			}
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				DTDprolog();

				writer.write("<!ELEMENT ");
				writer.write(name);
				writer.write(' ');
				writer.write(model);
				writer.write('>');
				writer.write(m_lineSep, 0, m_lineSepLen);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
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
		/// <seealso cref=".externalEntityDecl"/>
		/// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void internalEntityDecl(String name, String value) throws org.xml.sax.SAXException
		public virtual void internalEntityDecl(string name, string value)
		{
			// Do not inline external DTD
			if (m_inExternalDTD)
			{
				return;
			}
			try
			{
				DTDprolog();
				outputEntityDecl(name, value);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

		}

		/// <summary>
		/// Output the doc type declaration.
		/// </summary>
		/// <param name="name"> non-null reference to document type name. </param>
		/// NEEDSDOC <param name="value">
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void outputEntityDecl(String name, String value) throws java.io.IOException
		internal virtual void outputEntityDecl(string name, string value)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			Writer writer = m_writer;
			writer.write("<!ENTITY ");
			writer.write(name);
			writer.write(" \"");
			writer.write(value);
			writer.write("\">");
			writer.write(m_lineSep, 0, m_lineSepLen);
		}

		/// <summary>
		/// Output a system-dependent line break.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected final void outputLineSep() throws java.io.IOException
		protected internal void outputLineSep()
		{

			m_writer.write(m_lineSep, 0, m_lineSepLen);
		}

		internal override void setProp(string name, string val, bool defaultVal)
		{
			if (!string.ReferenceEquals(val, null))
			{


				char first = getFirstCharLocName(name);
				switch (first)
				{
				case 'c':
					if (OutputKeys.CDATA_SECTION_ELEMENTS.Equals(name))
					{
						string cdataSectionNames = val;
						addCdataSectionElements(cdataSectionNames);
					}
					break;
				case 'd':
					if (OutputKeys.DOCTYPE_SYSTEM.Equals(name))
					{
						this.m_doctypeSystem = val;
					}
					else if (OutputKeys.DOCTYPE_PUBLIC.Equals(name))
					{
						this.m_doctypePublic = val;
						if (val.StartsWith("-//W3C//DTD XHTML", StringComparison.Ordinal))
						{
							m_spaceBeforeClose = true;
						}
					}
					break;
				case 'e':
					string newEncoding = val;
					if (OutputKeys.ENCODING.Equals(name))
					{
						string possible_encoding = Encodings.getMimeEncoding(val);
						if (!string.ReferenceEquals(possible_encoding, null))
						{
							// if the encoding is being set, try to get the
							// preferred
							// mime-name and set it too.
							base.setProp("mime-name", possible_encoding, defaultVal);
						}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String oldExplicitEncoding = getOutputPropertyNonDefault(javax.xml.transform.OutputKeys.ENCODING);
						string oldExplicitEncoding = getOutputPropertyNonDefault(OutputKeys.ENCODING);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String oldDefaultEncoding = getOutputPropertyDefault(javax.xml.transform.OutputKeys.ENCODING);
						string oldDefaultEncoding = getOutputPropertyDefault(OutputKeys.ENCODING);
						if ((defaultVal && (string.ReferenceEquals(oldDefaultEncoding, null) || !oldDefaultEncoding.Equals(newEncoding, StringComparison.OrdinalIgnoreCase))) || (!defaultVal && (string.ReferenceEquals(oldExplicitEncoding, null) || !oldExplicitEncoding.Equals(newEncoding, StringComparison.OrdinalIgnoreCase))))
						{
						   // We are trying to change the default or the non-default setting of the encoding to a different value
						   // from what it was

						   EncodingInfo encodingInfo = Encodings.getEncodingInfo(newEncoding);
						   if (!string.ReferenceEquals(newEncoding, null) && string.ReferenceEquals(encodingInfo.name, null))
						   {
							// We tried to get an EncodingInfo for Object for the given
							// encoding, but it came back with an internall null name
							// so the encoding is not supported by the JDK, issue a message.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String msg = org.apache.xml.serializer.utils.Utils.messages.createMessage(org.apache.xml.serializer.utils.MsgKey.ER_ENCODING_NOT_SUPPORTED,new Object[]{ newEncoding });
							string msg = Utils.messages.createMessage(MsgKey.ER_ENCODING_NOT_SUPPORTED,new object[]{newEncoding});

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String msg2 = "Warning: encoding \"" + newEncoding + "\" not supported, using " + Encodings.DEFAULT_MIME_ENCODING;
							string msg2 = "Warning: encoding \"" + newEncoding + "\" not supported, using " + Encodings.DEFAULT_MIME_ENCODING;
							try
							{
									// Prepare to issue the warning message
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.Transformer tran = super.getTransformer();
									Transformer tran = base.Transformer;
									if (tran != null)
									{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.ErrorListener errHandler = tran.getErrorListener();
										ErrorListener errHandler = tran.getErrorListener();
										// Issue the warning message
										if (null != errHandler && m_sourceLocator != null)
										{
											errHandler.warning(new TransformerException(msg, m_sourceLocator));
											errHandler.warning(new TransformerException(msg2, m_sourceLocator));
										}
										else
										{
											Console.WriteLine(msg);
											Console.WriteLine(msg2);
										}
									}
									else
									{
										Console.WriteLine(msg);
										Console.WriteLine(msg2);
									}
							}
								catch (Exception)
								{
								}

								// We said we are using UTF-8, so use it
								newEncoding = Encodings.DEFAULT_MIME_ENCODING;
								val = Encodings.DEFAULT_MIME_ENCODING; // to store the modified value into the properties a little later
								encodingInfo = Encodings.getEncodingInfo(newEncoding);

						   }
						   // The encoding was good, or was forced to UTF-8 above


						   // If there is already a non-default set encoding and we 
						   // are trying to set the default encoding, skip the this block
						   // as the non-default value is already the one to use.
						   if (defaultVal == false || string.ReferenceEquals(oldExplicitEncoding, null))
						   {
							   m_encodingInfo = encodingInfo;
							   if (!string.ReferenceEquals(newEncoding, null))
							   {
								   m_isUTF8 = newEncoding.Equals(Encodings.DEFAULT_MIME_ENCODING);
							   }

							   // if there was a previously set OutputStream
							   Stream os = Stream;
							   if (os != null)
							   {
								   Writer w = Writer;

								   // If the writer was previously set, but
								   // set by the user, or if the new encoding is the same
								   // as the old encoding, skip this block
								   string oldEncoding = getOutputProperty(OutputKeys.ENCODING);
								   if ((w == null || !m_writer_set_by_user) && !newEncoding.Equals(oldEncoding, StringComparison.OrdinalIgnoreCase))
								   {
									   // Make the change of encoding in our internal
									   // table, then call setOutputStreamInternal
									   // which will stomp on the old Writer (if any)
									   // with a new Writer with the new encoding.
									   base.setProp(name, val, defaultVal);
									   setOutputStreamInternal(os,false);
								   }
							   }
						   }
						}
					}
					break;
				case 'i':
					if (OutputPropertiesFactory.S_KEY_INDENT_AMOUNT.Equals(name))
					{
						IndentAmount = int.Parse(val);
					}
					else if (OutputKeys.INDENT.Equals(name))
					{
						bool b = "yes".Equals(val) ? true : false;
						m_doIndent = b;
					}

					break;
				case 'l':
					if (OutputPropertiesFactory.S_KEY_LINE_SEPARATOR.Equals(name))
					{
						m_lineSep = val.ToCharArray();
						m_lineSepLen = m_lineSep.Length;
					}

					break;
				case 'm':
					if (OutputKeys.MEDIA_TYPE.Equals(name))
					{
						m_mediatype = val;
					}
					break;
				case 'o':
					if (OutputKeys.OMIT_XML_DECLARATION.Equals(name))
					{
						bool b = "yes".Equals(val) ? true : false;
						this.m_shouldNotWriteXMLHeader = b;
					}
					break;
				case 's':
					// if standalone was explicitly specified
					if (OutputKeys.STANDALONE.Equals(name))
					{
						if (defaultVal)
						{
							StandaloneInternal = val;
						}
						else
						{
							m_standaloneWasSpecified = true;
							StandaloneInternal = val;
						}
					}

					break;
				case 'v':
					if (OutputKeys.VERSION.Equals(name))
					{
						m_version = val;
					}
					break;
				default:
					break;

				}
				base.setProp(name, val, defaultVal);
			}
		}
		/// <summary>
		/// Specifies an output format for this serializer. It the
		/// serializer has already been associated with an output format,
		/// it will switch to the new format. This method should not be
		/// called while the serializer is in the process of serializing
		/// a document.
		/// </summary>
		/// <param name="format"> The output format to use </param>
		public override Properties OutputFormat
		{
			set
			{
    
				bool shouldFlush = m_shouldFlush;
    
				if (value != null)
				{
					// Set the default values first,
					// and the non-default values after that, 
					// just in case there is some unexpected
					// residual values left over from over-ridden default values
					System.Collections.IEnumerator propNames;
					propNames = value.propertyNames();
					while (propNames.MoveNext())
					{
						string key = (string) propNames.Current;
						// Get the value, possibly a default value
						string value = value.getProperty(key);
						// Get the non-default value (if any).
						string explicitValue = (string) value.get(key);
						if (string.ReferenceEquals(explicitValue, null) && !string.ReferenceEquals(value, null))
						{
							// This is a default value
							this.setOutputPropertyDefault(key,value);
						}
						if (!string.ReferenceEquals(explicitValue, null))
						{
							// This is an explicit non-default value
							this.setOutputProperty(key,explicitValue);
						}
					}
				}
    
				// Access this only from the Hashtable level... we don't want to 
				// get default properties.
				string entitiesFileName = (string) value.get(OutputPropertiesFactory.S_KEY_ENTITIES);
    
				if (null != entitiesFileName)
				{
    
					string method = (string) value.get(OutputKeys.METHOD);
    
					m_charInfo = CharInfo.getCharInfo(entitiesFileName, method);
				}
    
    
    
    
				m_shouldFlush = shouldFlush;
			}
			get
			{
				Properties def = new Properties();
				{
					ISet<object> s = OutputPropDefaultKeys;
					System.Collections.IEnumerator i = s.GetEnumerator();
					while (i.MoveNext())
					{
						string key = (string) i.Current;
						string val = getOutputPropertyDefault(key);
						def.put(key, val);
					}
				}
    
				Properties props = new Properties(def);
				{
					ISet<object> s = OutputPropKeys;
					System.Collections.IEnumerator i = s.GetEnumerator();
					while (i.MoveNext())
					{
						string key = (string) i.Current;
						string val = getOutputPropertyNonDefault(key);
						if (!string.ReferenceEquals(val, null))
						{
							props.put(key, val);
						}
					}
				}
				return props;
			}
		}


		/// <summary>
		/// Specifies a writer to which the document should be serialized.
		/// This method should not be called while the serializer is in
		/// the process of serializing a document.
		/// </summary>
		/// <param name="writer"> The output writer stream </param>
		public override Writer Writer
		{
			set
			{
				setWriterInternal(value, true);
			}
			get
			{
				return m_writer;
			}
		}

		private bool m_writer_set_by_user;
		private void setWriterInternal(Writer writer, bool setByUser)
		{

			m_writer_set_by_user = setByUser;
			m_writer = writer;
			// if we are tracing events we need to trace what
			// characters are written to the output writer.
			if (m_tracer != null)
			{
				bool noTracerYet = true;
				Writer w2 = m_writer;
				while (w2 is WriterChain)
				{
					if (w2 is SerializerTraceWriter)
					{
						noTracerYet = false;
						break;
					}
					w2 = ((WriterChain)w2).Writer;
				}
				if (noTracerYet)
				{
					m_writer = new SerializerTraceWriter(m_writer, m_tracer);
				}
			}
		}

		/// <summary>
		/// Set if the operating systems end-of-line line separator should
		/// be used when serializing.  If set false NL character 
		/// (decimal 10) is left alone, otherwise the new-line will be replaced on
		/// output with the systems line separator. For example on UNIX this is
		/// NL, while on Windows it is two characters, CR NL, where CR is the
		/// carriage-return (decimal 13).
		/// </summary>
		/// <param name="use_sytem_line_break"> True if an input NL is replaced with the 
		/// operating systems end-of-line separator. </param>
		/// <returns> The previously set value of the serializer. </returns>
		public virtual bool setLineSepUse(bool use_sytem_line_break)
		{
			bool oldValue = m_lineSepUse;
			m_lineSepUse = use_sytem_line_break;
			return oldValue;
		}


		private void setOutputStreamInternal(Stream output, bool setByUser)
		{
			m_outputStream = output;
			string encoding = getOutputProperty(OutputKeys.ENCODING);
			if (Encodings.DEFAULT_MIME_ENCODING.Equals(encoding, StringComparison.OrdinalIgnoreCase))
			{
				// We wrap the OutputStream with a writer, but
				// not one set by the user
				setWriterInternal(new WriterToUTF8Buffered(output), false);
			}
			else if ("WINDOWS-1250".Equals(encoding) || "US-ASCII".Equals(encoding) || "ASCII".Equals(encoding))
			{
				setWriterInternal(new WriterToASCI(output), false);
			}
			else if (!string.ReferenceEquals(encoding, null))
			{
				Writer osw = null;
					try
					{
						osw = Encodings.getWriter(output, encoding);
					}
					catch (UnsupportedEncodingException)
					{
						osw = null;
					}


				if (osw == null)
				{
					Console.WriteLine("Warning: encoding \"" + encoding + "\" not supported" + ", using " + Encodings.DEFAULT_MIME_ENCODING);

					encoding = Encodings.DEFAULT_MIME_ENCODING;
					Encoding = encoding;
					try
					{
						osw = Encodings.getWriter(output, encoding);
					}
					catch (UnsupportedEncodingException e)
					{
						// We can't really get here, UTF-8 is always supported
						// This try-catch exists to make the compiler happy
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				setWriterInternal(osw,false);
			}
			else
			{
				// don't have any encoding, but we have an OutputStream
				Writer osw = new StreamWriter(output);
				setWriterInternal(osw,false);
			}
		}

		/// <seealso cref="SerializationHandler.setEscaping(boolean)"/>
		public override bool setEscaping(bool escape)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = m_escaping;
			bool temp = m_escaping;
			m_escaping = escape;
			return temp;

		}


		/// <summary>
		/// Might print a newline character and the indentation amount
		/// of the given depth.
		/// </summary>
		/// <param name="depth"> the indentation depth (element nesting depth)
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> if an error occurs during writing. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void indent(int depth) throws java.io.IOException
		protected internal virtual void indent(int depth)
		{

			if (m_startNewLine)
			{
				outputLineSep();
			}
			/* For m_indentAmount > 0 this extra test might be slower
			 * but Xalan's default value is 0, so this extra test
			 * will run faster in that situation.
			 */
			if (m_indentAmount > 0)
			{
				printSpace(depth * m_indentAmount);
			}

		}

		/// <summary>
		/// Indent at the current element nesting depth. </summary>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void indent() throws java.io.IOException
		protected internal virtual void indent()
		{
			indent(m_elemContext.m_currentElemDepth);
		}
		/// <summary>
		/// Prints <var>n</var> spaces. </summary>
		/// <param name="n">         Number of spaces to print.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> if an error occurs when writing. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void printSpace(int n) throws java.io.IOException
		private void printSpace(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			Writer writer = m_writer;
			for (int i = 0; i < n; i++)
			{
				writer.write(' ');
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void attributeDecl(String eName, String aName, String type, String valueDefault, String value) throws org.xml.sax.SAXException
		public virtual void attributeDecl(string eName, string aName, string type, string valueDefault, string value)
		{
			// Do not inline external DTD
			if (m_inExternalDTD)
			{
				return;
			}
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				DTDprolog();

				writer.write("<!ATTLIST ");
				writer.write(eName);
				writer.write(' ');

				writer.write(aName);
				writer.write(' ');
				writer.write(type);
				if (!string.ReferenceEquals(valueDefault, null))
				{
					writer.write(' ');
					writer.write(valueDefault);
				}

				//writer.write(" ");
				//writer.write(value);
				writer.write('>');
				writer.write(m_lineSep, 0, m_lineSepLen);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
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
		/// <seealso cref=".internalEntityDecl"/>
		/// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void externalEntityDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void externalEntityDecl(string name, string publicId, string systemId)
		{
			try
			{
				DTDprolog();

				m_writer.write("<!ENTITY ");
				m_writer.write(name);
				if (!string.ReferenceEquals(publicId, null))
				{
					m_writer.write(" PUBLIC \"");
					m_writer.write(publicId);

				}
				else
				{
					m_writer.write(" SYSTEM \"");
					m_writer.write(systemId);
				}
				m_writer.write("\" >");
				m_writer.write(m_lineSep, 0, m_lineSepLen);
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

		}

		/// <summary>
		/// Tell if this character can be written without escaping.
		/// </summary>
		protected internal virtual bool escapingNotNeeded(char ch)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean ret;
			bool ret;
			if (ch < (char)127)
			{
				// This is the old/fast code here, but is this 
				// correct for all encodings?
				if (ch >= CharInfo.S_SPACE || (CharInfo.S_LINEFEED == ch || CharInfo.S_CARRIAGERETURN == ch || CharInfo.S_HORIZONAL_TAB == ch))
				{
					ret = true;
				}
				else
				{
					ret = false;
				}
			}
			else
			{
				ret = m_encodingInfo.isInEncoding(ch);
			}
			return ret;
		}

		/// <summary>
		/// Once a surrogate has been detected, write out the pair of
		/// characters if it is in the encoding, or if there is no
		/// encoding, otherwise write out an entity reference
		/// of the value of the unicode code point of the character
		/// represented by the high/low surrogate pair.
		/// <para>
		/// An exception is thrown if there is no low surrogate in the pair,
		/// because the array ends unexpectely, or if the low char is there
		/// but its value is such that it is not a low surrogate.
		/// 
		/// </para>
		/// </summary>
		/// <param name="c"> the first (high) part of the surrogate, which
		/// must be confirmed before calling this method. </param>
		/// <param name="ch"> Character array. </param>
		/// <param name="i"> position Where the surrogate was detected. </param>
		/// <param name="end"> The end index of the significant characters. </param>
		/// <returns> 0 if the pair of characters was written out as-is,
		/// the unicode code point of the character represented by
		/// the surrogate pair if an entity reference with that value
		/// was written out. 
		/// </returns>
		/// <exception cref="IOException"> </exception>
		/// <exception cref="org.xml.sax.SAXException"> if invalid UTF-16 surrogate detected. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int writeUTF16Surrogate(char c, char ch[], int i, int end) throws java.io.IOException
		protected internal virtual int writeUTF16Surrogate(char c, char[] ch, int i, int end)
		{
			int codePoint = 0;
			if (i + 1 >= end)
			{
				throw new IOException(Utils.messages.createMessage(MsgKey.ER_INVALID_UTF16_SURROGATE, new object[] {Convert.ToString((int) c, 16)}));
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char high = c;
			char high = c;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char low = ch[i+1];
			char low = ch[i + 1];
			if (!Encodings.isLowUTF16Surrogate(low))
			{
				throw new IOException(Utils.messages.createMessage(MsgKey.ER_INVALID_UTF16_SURROGATE, new object[] {Convert.ToString((int) c, 16) + " " + Convert.ToString(low, 16)}));
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			Writer writer = m_writer;

			// If we make it to here we have a valid high, low surrogate pair
			if (m_encodingInfo.isInEncoding(c,low))
			{
				// If the character formed by the surrogate pair
				// is in the encoding, so just write it out
				writer.write(ch,i,2);
			}
			else
			{
				// Don't know what to do with this char, it is
				// not in the encoding and not a high char in
				// a surrogate pair, so write out as an entity ref
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String encoding = getEncoding();
				string encoding = Encoding;
				if (!string.ReferenceEquals(encoding, null))
				{
					/* The output encoding is known, 
					 * so somthing is wrong.
					  */
					codePoint = Encodings.toCodePoint(high, low);
					// not in the encoding, so write out a character reference
					writer.write('&');
					writer.write('#');
					writer.write(Convert.ToString(codePoint));
					writer.write(';');
				}
				else
				{
					/* The output encoding is not known,
					 * so just write it out as-is.
					 */
					writer.write(ch, i, 2);
				}
			}
			// non-zero only if character reference was written out.
			return codePoint;
		}

		/// <summary>
		/// Handle one of the default entities, return false if it
		/// is not a default entity.
		/// </summary>
		/// <param name="ch"> character to be escaped. </param>
		/// <param name="i"> index into character array. </param>
		/// <param name="chars"> non-null reference to character array. </param>
		/// <param name="len"> length of chars. </param>
		/// <param name="fromTextNode"> true if the characters being processed
		/// are from a text node, false if they are from an attribute value </param>
		/// <param name="escLF"> true if the linefeed should be escaped.
		/// </param>
		/// <returns> i+1 if the character was written, else i.
		/// </returns>
		/// <exception cref="java.io.IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: int accumDefaultEntity(java.io.Writer writer, char ch, int i, char[] chars, int len, boolean fromTextNode, boolean escLF) throws java.io.IOException
		internal virtual int accumDefaultEntity(Writer writer, char ch, int i, char[] chars, int len, bool fromTextNode, bool escLF)
		{

			if (!escLF && CharInfo.S_LINEFEED == ch)
			{
				writer.write(m_lineSep, 0, m_lineSepLen);
			}
			else
			{
				// if this is text node character and a special one of those,
				// or if this is a character from attribute value and a special one of those
				if ((fromTextNode && m_charInfo.shouldMapTextChar(ch)) || (!fromTextNode && m_charInfo.shouldMapAttrChar(ch)))
				{
					string outputStringForChar = m_charInfo.getOutputStringForChar(ch);

					if (null != outputStringForChar)
					{
						writer.write(outputStringForChar);
					}
					else
					{
						return i;
					}
				}
				else
				{
					return i;
				}
			}

			return i + 1;

		}
		/// <summary>
		/// Normalize the characters, but don't escape.
		/// </summary>
		/// <param name="ch"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array. </param>
		/// <param name="isCData"> true if a CDATA block should be built around the characters. </param>
		/// <param name="useSystemLineSeparator"> true if the operating systems 
		/// end-of-line separator should be output rather than a new-line character.
		/// </param>
		/// <exception cref="IOException"> </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void writeNormalizedChars(char ch[], int start, int length, boolean isCData, boolean useSystemLineSeparator) throws IOException, org.xml.sax.SAXException
		internal virtual void writeNormalizedChars(char[] ch, int start, int length, bool isCData, bool useSystemLineSeparator)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			Writer writer = m_writer;
			int end = start + length;

			for (int i = start; i < end; i++)
			{
				char c = ch[i];

				if (CharInfo.S_LINEFEED == c && useSystemLineSeparator)
				{
					writer.write(m_lineSep, 0, m_lineSepLen);
				}
				else if (isCData && (!escapingNotNeeded(c)))
				{
					//                if (i != 0)
					if (m_cdataTagOpen)
					{
						closeCDATA();
					}

					// This needs to go into a function... 
					if (Encodings.isHighUTF16Surrogate(c))
					{
						writeUTF16Surrogate(c, ch, i, end);
						i++; // process two input characters
					}
					else
					{
						writer.write("&#");

						string intStr = Convert.ToString((int) c);

						writer.write(intStr);
						writer.write(';');
					}

					//                if ((i != 0) && (i < (end - 1)))
					//                if (!m_cdataTagOpen && (i < (end - 1)))
					//                {
					//                    writer.write(CDATA_DELIMITER_OPEN);
					//                    m_cdataTagOpen = true;
					//                }
				}
				else if (isCData && ((i < (end - 2)) && (']' == c) && (']' == ch[i + 1]) && ('>' == ch[i + 2])))
				{
					writer.write(CDATA_CONTINUE);

					i += 2;
				}
				else
				{
					if (escapingNotNeeded(c))
					{
						if (isCData && !m_cdataTagOpen)
						{
							writer.write(CDATA_DELIMITER_OPEN);
							m_cdataTagOpen = true;
						}
						writer.write(c);
					}

					// This needs to go into a function... 
					else if (Encodings.isHighUTF16Surrogate(c))
					{
						if (m_cdataTagOpen)
						{
							closeCDATA();
						}
						writeUTF16Surrogate(c, ch, i, end);
						i++; // process two input characters
					}
					else
					{
						if (m_cdataTagOpen)
						{
							closeCDATA();
						}
						writer.write("&#");

						string intStr = Convert.ToString((int) c);

						writer.write(intStr);
						writer.write(';');
					}
				}
			}

		}

		/// <summary>
		/// Ends an un-escaping section.
		/// </summary>
		/// <seealso cref=".startNonEscaping"
		////>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endNonEscaping() throws org.xml.sax.SAXException
		public virtual void endNonEscaping()
		{
			m_disableOutputEscapingStates.pop();
		}

		/// <summary>
		/// Starts an un-escaping section. All characters printed within an un-
		/// escaping section are printed as is, without escaping special characters
		/// into entity references. Only XML and HTML serializers need to support
		/// this method.
		/// <para> The contents of the un-escaping section will be delivered through the
		/// regular <tt>characters</tt> event.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startNonEscaping() throws org.xml.sax.SAXException
		public virtual void startNonEscaping()
		{
			m_disableOutputEscapingStates.push(true);
		}

		/// <summary>
		/// Receive notification of cdata.
		/// 
		/// <para>The Parser will call this method to report each chunk of
		/// character data.  SAX parsers may return all contiguous character
		/// data in a single chunk, or they may split it into several
		/// chunks; however, all of the characters in any single event
		/// must come from the same external entity, so that the Locator
		/// provides useful information.</para>
		/// 
		/// <para>The application must not attempt to read from the array
		/// outside of the specified range.</para>
		/// 
		/// <para>Note that some parsers will report whitespace using the
		/// ignorableWhitespace() method rather than this one (validating
		/// parsers must do so).</para>
		/// </summary>
		/// <param name="ch"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref=".ignorableWhitespace"/>
		/// <seealso cref="org.xml.sax.Locator"
		////>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void cdata(char ch[], int start, final int length) throws org.xml.sax.SAXException
		protected internal virtual void cdata(char[] ch, int start, in int length)
		{

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int old_start = start;
				int old_start = start;
				if (m_elemContext.m_startTagOpen)
				{
					closeStartTag();
					m_elemContext.m_startTagOpen = false;
				}
				m_ispreserve = true;

				if (shouldIndent())
				{
					indent();
				}

				bool writeCDataBrackets = (((length >= 1) && escapingNotNeeded(ch[start])));

				/* Write out the CDATA opening delimiter only if
				 * we are supposed to, and if we are not already in
				 * the middle of a CDATA section  
				 */
				if (writeCDataBrackets && !m_cdataTagOpen)
				{
					m_writer.write(CDATA_DELIMITER_OPEN);
					m_cdataTagOpen = true;
				}

				// writer.write(ch, start, length);
				if (EscapingDisabled)
				{
					charactersRaw(ch, start, length);
				}
				else
				{
					writeNormalizedChars(ch, start, length, true, m_lineSepUse);
				}

				/* used to always write out CDATA closing delimiter here,
				 * but now we delay, so that we can merge CDATA sections on output.    
				 * need to write closing delimiter later
				 */
				if (writeCDataBrackets)
				{
					/* if the CDATA section ends with ] don't leave it open
					 * as there is a chance that an adjacent CDATA sections
					 * starts with ]>.  
					 * We don't want to merge ]] with > , or ] with ]> 
					 */
					if (ch[start + length - 1] == ']')
					{
						closeCDATA();
					}
				}

				// time to fire off CDATA event
				if (m_tracer != null)
				{
					base.fireCDATAEvent(ch, old_start, length);
				}
			}
			catch (IOException ioe)
			{
				throw new SAXException(Utils.messages.createMessage(MsgKey.ER_OIERROR, null), ioe);
				//"IO error", ioe);
			}
		}

		/// <summary>
		/// Tell if the character escaping should be disabled for the current state.
		/// </summary>
		/// <returns> true if the character escaping should be disabled. </returns>
		private bool EscapingDisabled
		{
			get
			{
				return m_disableOutputEscapingStates.peekOrFalse();
			}
		}

		/// <summary>
		/// If available, when the disable-output-escaping attribute is used,
		/// output raw text without escaping.
		/// </summary>
		/// <param name="ch"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void charactersRaw(char ch[], int start, int length) throws org.xml.sax.SAXException
		protected internal virtual void charactersRaw(char[] ch, int start, int length)
		{

			if (m_inEntityRef)
			{
				return;
			}
			try
			{
				if (m_elemContext.m_startTagOpen)
				{
					closeStartTag();
					m_elemContext.m_startTagOpen = false;
				}

				m_ispreserve = true;

				m_writer.write(ch, start, length);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

		}

		/// <summary>
		/// Receive notification of character data.
		/// 
		/// <para>The Parser will call this method to report each chunk of
		/// character data.  SAX parsers may return all contiguous character
		/// data in a single chunk, or they may split it into several
		/// chunks; however, all of the characters in any single event
		/// must come from the same external entity, so that the Locator
		/// provides useful information.</para>
		/// 
		/// <para>The application must not attempt to read from the array
		/// outside of the specified range.</para>
		/// 
		/// <para>Note that some parsers will report whitespace using the
		/// ignorableWhitespace() method rather than this one (validating
		/// parsers must do so).</para>
		/// </summary>
		/// <param name="chars"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref=".ignorableWhitespace"/>
		/// <seealso cref="org.xml.sax.Locator"
		////>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(final char chars[], final int start, final int length) throws org.xml.sax.SAXException
		public virtual void characters(in char[] chars, in int start, in int length)
		{
			// It does not make sense to continue with rest of the method if the number of 
			// characters to read from array is 0.
			// Section 7.6.1 of XSLT 1.0 (http://www.w3.org/TR/xslt#value-of) suggest no text node
			// is created if string is empty.	
			if (length == 0 || (m_inEntityRef && !m_expandDTDEntities))
			{
				return;
			}

			m_docIsEmpty = false;

			if (m_elemContext.m_startTagOpen)
			{
				closeStartTag();
				m_elemContext.m_startTagOpen = false;
			}
			else if (m_needToCallStartDocument)
			{
				startDocumentInternal();
			}

			if (m_cdataStartCalled || m_elemContext.m_isCdataSection)
			{
				/* either due to startCDATA() being called or due to 
				 * cdata-section-elements atribute, we need this as cdata
				 */
				cdata(chars, start, length);

				return;
			}

			if (m_cdataTagOpen)
			{
				closeCDATA();
			}

			if (m_disableOutputEscapingStates.peekOrFalse() || (!m_escaping))
			{
				charactersRaw(chars, start, length);

				// time to fire off characters generation event
				if (m_tracer != null)
				{
					base.fireCharEvent(chars, start, length);
				}

				return;
			}

			if (m_elemContext.m_startTagOpen)
			{
				closeStartTag();
				m_elemContext.m_startTagOpen = false;
			}


			try
			{
				int i;
				int startClean;

				// skip any leading whitspace 
				// don't go off the end and use a hand inlined version
				// of isWhitespace(ch)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = start + length;
				int end = start + length;
				int lastDirtyCharProcessed = start - 1; // last non-clean character that was processed
														// that was processed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				bool isAllWhitespace = true;

				// process any leading whitspace
				i = start;
				while (i < end && isAllWhitespace)
				{
					char ch1 = chars[i];

					if (m_charInfo.shouldMapTextChar(ch1))
					{
						// The character is supposed to be replaced by a String
						// so write out the clean whitespace characters accumulated
						// so far
						// then the String.
						writeOutCleanChars(chars, i, lastDirtyCharProcessed);
						string outputStringForChar = m_charInfo.getOutputStringForChar(ch1);
						writer.write(outputStringForChar);
						// We can't say that everything we are writing out is
						// all whitespace, we just wrote out a String.
						isAllWhitespace = false;
						lastDirtyCharProcessed = i; // mark the last non-clean
						// character processed
						i++;
					}
					else
					{
						// The character is clean, but is it a whitespace ?
						switch (ch1)
						{
						// TODO: Any other whitespace to consider?
						case CharInfo.S_SPACE:
							// Just accumulate the clean whitespace
							i++;
							break;
						case CharInfo.S_LINEFEED:
							lastDirtyCharProcessed = processLineFeed(chars, i, lastDirtyCharProcessed, writer);
							i++;
							break;
						case CharInfo.S_CARRIAGERETURN:
							writeOutCleanChars(chars, i, lastDirtyCharProcessed);
							writer.write("&#13;");
							lastDirtyCharProcessed = i;
							i++;
							break;
						case CharInfo.S_HORIZONAL_TAB:
							// Just accumulate the clean whitespace
							i++;
							break;
						default:
							// The character was clean, but not a whitespace
							// so break the loop to continue with this character
							// (we don't increment index i !!)
							isAllWhitespace = false;
							break;
						}
					}
				}

				/* If there is some non-whitespace, mark that we may need
				 * to preserve this. This is only important if we have indentation on.
				 */            
				if (i < end || !isAllWhitespace)
				{
					m_ispreserve = true;
				}


				for (; i < end; i++)
				{
					char ch = chars[i];

					if (m_charInfo.shouldMapTextChar(ch))
					{
						// The character is supposed to be replaced by a String
						// e.g.   '&'  -->  "&amp;"
						// e.g.   '<'  -->  "&lt;"
						writeOutCleanChars(chars, i, lastDirtyCharProcessed);
						string outputStringForChar = m_charInfo.getOutputStringForChar(ch);
						writer.write(outputStringForChar);
						lastDirtyCharProcessed = i;
					}
					else
					{
						if (ch <= (char)0x1F)
						{
							// Range 0x00 through 0x1F inclusive
							//
							// This covers the non-whitespace control characters
							// in the range 0x1 to 0x1F inclusive.
							// It also covers the whitespace control characters in the same way:
							// 0x9   TAB
							// 0xA   NEW LINE
							// 0xD   CARRIAGE RETURN
							//
							// We also cover 0x0 ... It isn't valid
							// but we will output "&#0;" 

							// The default will handle this just fine, but this
							// is a little performance boost to handle the more
							// common TAB, NEW-LINE, CARRIAGE-RETURN
							switch (ch)
							{

							case CharInfo.S_HORIZONAL_TAB:
								// Leave whitespace TAB as a real character
								break;
							case CharInfo.S_LINEFEED:
								lastDirtyCharProcessed = processLineFeed(chars, i, lastDirtyCharProcessed, writer);
								break;
							case CharInfo.S_CARRIAGERETURN:
								writeOutCleanChars(chars, i, lastDirtyCharProcessed);
								writer.write("&#13;");
								lastDirtyCharProcessed = i;
								// Leave whitespace carriage return as a real character
								break;
							default:
								writeOutCleanChars(chars, i, lastDirtyCharProcessed);
								writer.write("&#");
								writer.write(Convert.ToString(ch));
								writer.write(';');
								lastDirtyCharProcessed = i;
								break;

							}
						}
						else if (ch < (char)0x7F)
						{
							// Range 0x20 through 0x7E inclusive
							// Normal ASCII chars, do nothing, just add it to
							// the clean characters

						}
						else if (ch <= (char)0x9F)
						{
							// Range 0x7F through 0x9F inclusive
							// More control characters, including NEL (0x85)
							writeOutCleanChars(chars, i, lastDirtyCharProcessed);
							writer.write("&#");
							writer.write(Convert.ToString(ch));
							writer.write(';');
							lastDirtyCharProcessed = i;
						}
						else if (ch == CharInfo.S_LINE_SEPARATOR)
						{
							// LINE SEPARATOR
							writeOutCleanChars(chars, i, lastDirtyCharProcessed);
							writer.write("&#8232;");
							lastDirtyCharProcessed = i;
						}
						else if (m_encodingInfo.isInEncoding(ch))
						{
							// If the character is in the encoding, and
							// not in the normal ASCII range, we also
							// just leave it get added on to the clean characters

						}
						else
						{
							// This is a fallback plan, we should never get here
							// but if the character wasn't previously handled
							// (i.e. isn't in the encoding, etc.) then what
							// should we do?  We choose to write out an entity
							writeOutCleanChars(chars, i, lastDirtyCharProcessed);
							writer.write("&#");
							writer.write(Convert.ToString(ch));
							writer.write(';');
							lastDirtyCharProcessed = i;
						}
					}
				}

				// we've reached the end. Any clean characters at the
				// end of the array than need to be written out?
				startClean = lastDirtyCharProcessed + 1;
				if (i > startClean)
				{
					int lengthClean = i - startClean;
					m_writer.write(chars, startClean, lengthClean);
				}

				// For indentation purposes, mark that we've just writen text out
				m_isprevtext = true;
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

			// time to fire off characters generation event
			if (m_tracer != null)
			{
				base.fireCharEvent(chars, start, length);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int processLineFeed(final char[] chars, int i, int lastProcessed, final java.io.Writer writer) throws java.io.IOException
		private int processLineFeed(in char[] chars, int i, int lastProcessed, in Writer writer)
		{
			if (!m_lineSepUse || (m_lineSepLen == 1 && m_lineSep[0] == CharInfo.S_LINEFEED))
			{
				// We are leaving the new-line alone, and it is just
				// being added to the 'clean' characters,
				// so the last dirty character processed remains unchanged
			}
			else
			{
				writeOutCleanChars(chars, i, lastProcessed);
				writer.write(m_lineSep, 0, m_lineSepLen);
				lastProcessed = i;
			}
			return lastProcessed;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void writeOutCleanChars(final char[] chars, int i, int lastProcessed) throws java.io.IOException
		private void writeOutCleanChars(in char[] chars, int i, int lastProcessed)
		{
			int startClean;
			startClean = lastProcessed + 1;
			if (startClean < i)
			{
				int lengthClean = i - startClean;
				m_writer.write(chars, startClean, lengthClean);
			}
		}
		/// <summary>
		/// This method checks if a given character is between C0 or C1 range
		/// of Control characters.
		/// This method is added to support Control Characters for XML 1.1
		/// If a given character is TAB (0x09), LF (0x0A) or CR (0x0D), this method
		/// return false. Since they are whitespace characters, no special processing is needed.
		/// </summary>
		/// <param name="ch"> </param>
		/// <returns> boolean </returns>
		private static bool isCharacterInC0orC1Range(char ch)
		{
			if (ch == (char)0x09 || ch == (char)0x0A || ch == (char)0x0D)
			{
				return false;
			}
			else
			{
				return (ch >= (char)0x7F && ch <= (char)0x9F) || (ch >= (char)0x01 && ch <= (char)0x1F);
			}
		}
		/// <summary>
		/// This method checks if a given character either NEL (0x85) or LSEP (0x2028)
		/// These are new end of line charcters added in XML 1.1.  These characters must be
		/// written as Numeric Character References (NCR) in XML 1.1 output document.
		/// </summary>
		/// <param name="ch"> </param>
		/// <returns> boolean </returns>
		private static bool isNELorLSEPCharacter(char ch)
		{
			return (ch == (char)0x85 || ch == (char)0x2028);
		}
		/// <summary>
		/// Process a dirty character and any preeceding clean characters
		/// that were not yet processed. </summary>
		/// <param name="chars"> array of characters being processed </param>
		/// <param name="end"> one (1) beyond the last character 
		/// in chars to be processed </param>
		/// <param name="i"> the index of the dirty character </param>
		/// <param name="ch"> the character in chars[i] </param>
		/// <param name="lastDirty"> the last dirty character previous to i </param>
		/// <param name="fromTextNode"> true if the characters being processed are
		/// from a text node, false if they are from an attribute value. </param>
		/// <returns> the index of the last character processed </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int processDirty(char[] chars, int end, int i, char ch, int lastDirty, boolean fromTextNode) throws java.io.IOException
		private int processDirty(char[] chars, int end, int i, char ch, int lastDirty, bool fromTextNode)
		{
			int startClean = lastDirty + 1;
			// if we have some clean characters accumulated
			// process them before the dirty one.                   
			if (i > startClean)
			{
				int lengthClean = i - startClean;
				m_writer.write(chars, startClean, lengthClean);
			}

			// process the "dirty" character
			if (CharInfo.S_LINEFEED == ch && fromTextNode)
			{
				m_writer.write(m_lineSep, 0, m_lineSepLen);
			}
			else
			{
				startClean = accumDefaultEscape(m_writer, (char)ch, i, chars, end, fromTextNode, false);
				i = startClean - 1;
			}
			// Return the index of the last character that we just processed 
			// which is a dirty character.
			return i;
		}

		/// <summary>
		/// Receive notification of character data.
		/// </summary>
		/// <param name="s"> The string of characters to process.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(String s) throws org.xml.sax.SAXException
		public override void characters(string s)
		{
			if (m_inEntityRef && !m_expandDTDEntities)
			{
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = s.length();
			int length = s.Length;
			if (length > m_charsBuff.Length)
			{
				m_charsBuff = new char[length * 2 + 1];
			}
			s.CopyTo(0, m_charsBuff, 0, length - 0);
			characters(m_charsBuff, 0, length);
		}

		/// <summary>
		/// Escape and writer.write a character.
		/// </summary>
		/// <param name="ch"> character to be escaped. </param>
		/// <param name="i"> index into character array. </param>
		/// <param name="chars"> non-null reference to character array. </param>
		/// <param name="len"> length of chars. </param>
		/// <param name="fromTextNode"> true if the characters being processed are
		/// from a text node, false if the characters being processed are from
		/// an attribute value. </param>
		/// <param name="escLF"> true if the linefeed should be escaped.
		/// </param>
		/// <returns> i+1 if a character was written, i+2 if two characters
		/// were written out, else return i.
		/// </returns>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private int accumDefaultEscape(java.io.Writer writer, char ch, int i, char[] chars, int len, boolean fromTextNode, boolean escLF) throws java.io.IOException
		private int accumDefaultEscape(Writer writer, char ch, int i, char[] chars, int len, bool fromTextNode, bool escLF)
		{

			int pos = accumDefaultEntity(writer, ch, i, chars, len, fromTextNode, escLF);

			if (i == pos)
			{
				if (Encodings.isHighUTF16Surrogate(ch))
				{

					// Should be the UTF-16 low surrogate of the hig/low pair.
					char next;
					// Unicode code point formed from the high/low pair.
					int codePoint = 0;

					if (i + 1 >= len)
					{
						throw new IOException(Utils.messages.createMessage(MsgKey.ER_INVALID_UTF16_SURROGATE, new object[] {Convert.ToString(ch, 16)}));
						//"Invalid UTF-16 surrogate detected: "

						//+Integer.toHexString(ch)+ " ?");
					}
					else
					{
						next = chars[++i];

						if (!(Encodings.isLowUTF16Surrogate(next)))
						{
							throw new IOException(Utils.messages.createMessage(MsgKey.ER_INVALID_UTF16_SURROGATE, new object[] {Convert.ToString(ch, 16) + " " + Convert.ToString(next, 16)}));
						}
						//"Invalid UTF-16 surrogate detected: "

						//+Integer.toHexString(ch)+" "+Integer.toHexString(next));
						codePoint = Encodings.toCodePoint(ch,next);
					}

					writer.write("&#");
					writer.write(Convert.ToString(codePoint));
					writer.write(';');
					pos += 2; // count the two characters that went into writing out this entity
				}
				else
				{
					/*  This if check is added to support control characters in XML 1.1.
					 *  If a character is a Control Character within C0 and C1 range, it is desirable
					 *  to write it out as Numeric Character Reference(NCR) regardless of XML Version
					 *  being used for output document.
					 */ 
					if (isCharacterInC0orC1Range(ch) || isNELorLSEPCharacter(ch))
					{
						writer.write("&#");
						writer.write(Convert.ToString(ch));
						writer.write(';');
					}
					else if ((!escapingNotNeeded(ch) || ((fromTextNode && m_charInfo.shouldMapTextChar(ch)) || (!fromTextNode && m_charInfo.shouldMapAttrChar(ch)))) && m_elemContext.m_currentElemDepth > 0)
					{
						writer.write("&#");
						writer.write(Convert.ToString(ch));
						writer.write(';');
					}
					else
					{
						writer.write(ch);
					}
					pos++; // count the single character that was processed
				}

			}
			return pos;
		}

		/// <summary>
		/// Receive notification of the beginning of an element, although this is a
		/// SAX method additional namespace or attribute information can occur before
		/// or after this call, that is associated with this element.
		/// 
		/// </summary>
		/// <param name="namespaceURI"> The Namespace URI, or the empty string if the
		///        element has no Namespace URI or if Namespace
		///        processing is not being performed. </param>
		/// <param name="localName"> The local name (without prefix), or the
		///        empty string if Namespace processing is not being
		///        performed. </param>
		/// <param name="name"> The element type name. </param>
		/// <param name="atts"> The attributes attached to the element, if any. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref="org.xml.sax.ContentHandler.startElement"/>
		/// <seealso cref="org.xml.sax.ContentHandler.endElement"/>
		/// <seealso cref="org.xml.sax.AttributeList"
		////>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String name, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public virtual void startElement(string namespaceURI, string localName, string name, Attributes atts)
		{
			if (m_inEntityRef)
			{
				return;
			}

			if (m_needToCallStartDocument)
			{
				startDocumentInternal();
				m_needToCallStartDocument = false;
				m_docIsEmpty = false;
			}
			else if (m_cdataTagOpen)
			{
				closeCDATA();
			}
			try
			{
				if (m_needToOutputDocTypeDecl)
				{
					if (null != DoctypeSystem)
					{
						outputDocTypeDecl(name, true);
					}
					m_needToOutputDocTypeDecl = false;
				}

				/* before we over-write the current elementLocalName etc.
				 * lets close out the old one (if we still need to)
				 */
				if (m_elemContext.m_startTagOpen)
				{
					closeStartTag();
					m_elemContext.m_startTagOpen = false;
				}

				if (!string.ReferenceEquals(namespaceURI, null))
				{
					ensurePrefixIsDeclared(namespaceURI, name);
				}

				m_ispreserve = false;

				if (shouldIndent() && m_startNewLine)
				{
					indent();
				}

				m_startNewLine = true;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				writer.write('<');
				writer.write(name);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

			// process the attributes now, because after this SAX call they might be gone
			if (atts != null)
			{
				addAttributes(atts);
			}

			m_elemContext = m_elemContext.push(namespaceURI,localName,name);
			m_isprevtext = false;

			if (m_tracer != null)
			{
				firePseudoAttributes();
			}
		}

		/// <summary>
		/// Receive notification of the beginning of an element, additional
		/// namespace or attribute information can occur before or after this call,
		/// that is associated with this element.
		///  
		/// </summary>
		/// <param name="elementNamespaceURI"> The Namespace URI, or the empty string if the
		///        element has no Namespace URI or if Namespace
		///        processing is not being performed. </param>
		/// <param name="elementLocalName"> The local name (without prefix), or the
		///        empty string if Namespace processing is not being
		///        performed. </param>
		/// <param name="elementName"> The element type name. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref="org.xml.sax.ContentHandler.startElement"/>
		/// <seealso cref="org.xml.sax.ContentHandler.endElement"/>
		/// <seealso cref="org.xml.sax.AttributeList"
		////>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String elementNamespaceURI, String elementLocalName, String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementNamespaceURI, string elementLocalName, string elementName)
		{
			startElement(elementNamespaceURI, elementLocalName, elementName, null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String elementName) throws org.xml.sax.SAXException
		public override void startElement(string elementName)
		{
			startElement(null, null, elementName, null);
		}

		/// <summary>
		/// Output the doc type declaration.
		/// </summary>
		/// <param name="name"> non-null reference to document type name. </param>
		/// NEEDSDOC <param name="closeDecl">
		/// </param>
		/// <exception cref="java.io.IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void outputDocTypeDecl(String name, boolean closeDecl) throws org.xml.sax.SAXException
		internal virtual void outputDocTypeDecl(string name, bool closeDecl)
		{
			if (m_cdataTagOpen)
			{
				closeCDATA();
			}
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				writer.write("<!DOCTYPE ");
				writer.write(name);

				string doctypePublic = DoctypePublic;
				if (null != doctypePublic)
				{
					writer.write(" PUBLIC \"");
					writer.write(doctypePublic);
					writer.write('\"');
				}

				string doctypeSystem = DoctypeSystem;
				if (null != doctypeSystem)
				{
					if (null == doctypePublic)
					{
						writer.write(" SYSTEM \"");
					}
					else
					{
						writer.write(" \"");
					}

					writer.write(doctypeSystem);

					if (closeDecl)
					{
						writer.write("\">");
						writer.write(m_lineSep, 0, m_lineSepLen);
						closeDecl = false; // done closing
					}
					else
					{
						writer.write('\"');
					}
				}
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}
		}

		/// <summary>
		/// Process the attributes, which means to write out the currently
		/// collected attributes to the writer. The attributes are not
		/// cleared by this method
		/// </summary>
		/// <param name="writer"> the writer to write processed attributes to. </param>
		/// <param name="nAttrs"> the number of attributes in m_attributes 
		/// to be processed
		/// </param>
		/// <exception cref="java.io.IOException"> </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processAttributes(java.io.Writer writer, int nAttrs) throws IOException, org.xml.sax.SAXException
		public virtual void processAttributes(Writer writer, int nAttrs)
		{
				/* real SAX attributes are not passed in, so process the 
				 * attributes that were collected after the startElement call.
				 * _attribVector is a "cheap" list for Stream serializer output
				 * accumulated over a series of calls to attribute(name,value)
				 */

				string encoding = Encoding;
				for (int i = 0; i < nAttrs; i++)
				{
					// elementAt is JDK 1.1.8
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = m_attributes.getQName(i);
					string name = m_attributes.getQName(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = m_attributes.getValue(i);
					string value = m_attributes.getValue(i);
					writer.write(' ');
					writer.write(name);
					writer.write("=\"");
					writeAttrString(writer, value, encoding);
					writer.write('\"');
				}
		}

		/// <summary>
		/// Returns the specified <var>string</var> after substituting <VAR>specials</VAR>,
		/// and UTF-16 surrogates for chracter references <CODE>&amp;#xnn</CODE>.
		/// </summary>
		/// <param name="string">      String to convert to XML format. </param>
		/// <param name="encoding">    CURRENTLY NOT IMPLEMENTED.
		/// </param>
		/// <exception cref="java.io.IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void writeAttrString(java.io.Writer writer, String string, String encoding) throws java.io.IOException
		public virtual void writeAttrString(Writer writer, string @string, string encoding)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = string.length();
			int len = @string.Length;
			if (len > m_attrBuff.Length)
			{
			   m_attrBuff = new char[len * 2 + 1];
			}
			@string.CopyTo(0, m_attrBuff, 0, len - 0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] stringChars = m_attrBuff;
			char[] stringChars = m_attrBuff;

			for (int i = 0; i < len; i++)
			{
				char ch = stringChars[i];

				if (m_charInfo.shouldMapAttrChar(ch))
				{
					// The character is supposed to be replaced by a String
					// e.g.   '&'  -->  "&amp;"
					// e.g.   '<'  -->  "&lt;"
					accumDefaultEscape(writer, ch, i, stringChars, len, false, true);
				}
				else
				{
					if ((char)0x0 <= ch && ch <= (char)0x1F)
					{
						// Range 0x00 through 0x1F inclusive
						// This covers the non-whitespace control characters
						// in the range 0x1 to 0x1F inclusive.
						// It also covers the whitespace control characters in the same way:
						// 0x9   TAB
						// 0xA   NEW LINE
						// 0xD   CARRIAGE RETURN
						//
						// We also cover 0x0 ... It isn't valid
						// but we will output "&#0;" 

						// The default will handle this just fine, but this
						// is a little performance boost to handle the more
						// common TAB, NEW-LINE, CARRIAGE-RETURN
						switch (ch)
						{

						case CharInfo.S_HORIZONAL_TAB:
							writer.write("&#9;");
							break;
						case CharInfo.S_LINEFEED:
							writer.write("&#10;");
							break;
						case CharInfo.S_CARRIAGERETURN:
							writer.write("&#13;");
							break;
						default:
							writer.write("&#");
							writer.write(Convert.ToString(ch));
							writer.write(';');
							break;

						}
					}
					else if (ch < (char)0x7F)
					{
						// Range 0x20 through 0x7E inclusive
						// Normal ASCII chars
							writer.write(ch);
					}
					else if (ch <= (char)0x9F)
					{
						// Range 0x7F through 0x9F inclusive
						// More control characters
						writer.write("&#");
						writer.write(Convert.ToString(ch));
						writer.write(';');
					}
					else if (ch == CharInfo.S_LINE_SEPARATOR)
					{
						// LINE SEPARATOR
						writer.write("&#8232;");
					}
					else if (m_encodingInfo.isInEncoding(ch))
					{
						// If the character is in the encoding, and
						// not in the normal ASCII range, we also
						// just write it out
						writer.write(ch);
					}
					else
					{
						// This is a fallback plan, we should never get here
						// but if the character wasn't previously handled
						// (i.e. isn't in the encoding, etc.) then what
						// should we do?  We choose to write out a character ref
						writer.write("&#");
						writer.write(Convert.ToString(ch));
						writer.write(';');
					}

				}
			}
		}

		/// <summary>
		/// Receive notification of the end of an element.
		/// 
		/// </summary>
		/// <param name="namespaceURI"> The Namespace URI, or the empty string if the
		///        element has no Namespace URI or if Namespace
		///        processing is not being performed. </param>
		/// <param name="localName"> The local name (without prefix), or the
		///        empty string if Namespace processing is not being
		///        performed. </param>
		/// <param name="name"> The element type name </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String namespaceURI, String localName, String name) throws org.xml.sax.SAXException
		public virtual void endElement(string namespaceURI, string localName, string name)
		{
			if (m_inEntityRef)
			{
				return;
			}

			// namespaces declared at the current depth are no longer valid
			// so get rid of them    
			m_prefixMap.popNamespaces(m_elemContext.m_currentElemDepth, null);

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				if (m_elemContext.m_startTagOpen)
				{
					if (m_tracer != null)
					{
						base.fireStartElem(m_elemContext.m_elementName);
					}
					int nAttrs = m_attributes.getLength();
					if (nAttrs > 0)
					{
						processAttributes(m_writer, nAttrs);
						// clear attributes object for re-use with next element
						m_attributes.clear();
					}
					if (m_spaceBeforeClose)
					{
						writer.write(" />");
					}
					else
					{
						writer.write("/>");
					}
					/* don't need to pop cdataSectionState because
					 * this element ended so quickly that we didn't get
					 * to push the state.
					 */

				}
				else
				{
					if (m_cdataTagOpen)
					{
						closeCDATA();
					}

					if (shouldIndent())
					{
						indent(m_elemContext.m_currentElemDepth - 1);
					}
					writer.write('<');
					writer.write('/');
					writer.write(name);
					writer.write('>');
				}
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

			if (!m_elemContext.m_startTagOpen && m_doIndent)
			{
				m_ispreserve = m_preserves.Empty ? false : m_preserves.pop();
			}

			m_isprevtext = false;

			// fire off the end element event
			if (m_tracer != null)
			{
				base.fireEndElem(name);
			}
			m_elemContext = m_elemContext.m_prev;
		}

		/// <summary>
		/// Receive notification of the end of an element. </summary>
		/// <param name="name"> The element type name </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///     wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String name) throws org.xml.sax.SAXException
		public override void endElement(string name)
		{
			endElement(null, null, name);
		}

		/// <summary>
		/// Begin the scope of a prefix-URI Namespace mapping
		/// just before another element is about to start.
		/// This call will close any open tags so that the prefix mapping
		/// will not apply to the current element, but the up comming child.
		/// </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.startPrefixMapping"
		////>
		/// <param name="prefix"> The Namespace prefix being declared. </param>
		/// <param name="uri"> The Namespace URI the prefix is mapped to.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> The client may throw
		///            an exception during processing.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
		public virtual void startPrefixMapping(string prefix, string uri)
		{
			// the "true" causes the flush of any open tags
			startPrefixMapping(prefix, uri, true);
		}

		/// <summary>
		/// Handle a prefix/uri mapping, which is associated with a startElement()
		/// that is soon to follow. Need to close any open start tag to make
		/// sure than any name space attributes due to this event are associated wih
		/// the up comming element, not the current one. </summary>
		/// <seealso cref="ExtendedContentHandler.startPrefixMapping"
		////>
		/// <param name="prefix"> The Namespace prefix being declared. </param>
		/// <param name="uri"> The Namespace URI the prefix is mapped to. </param>
		/// <param name="shouldFlush"> true if any open tags need to be closed first, this
		/// will impact which element the mapping applies to (open parent, or its up
		/// comming child) </param>
		/// <returns> returns true if the call made a change to the current 
		/// namespace information, false if it did not change anything, e.g. if the
		/// prefix/namespace mapping was already in scope from before.
		/// </returns>
		/// <exception cref="org.xml.sax.SAXException"> The client may throw
		///            an exception during processing.
		/// 
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException
		public override bool startPrefixMapping(string prefix, string uri, bool shouldFlush)
		{

			/* Remember the mapping, and at what depth it was declared
			 * This is one greater than the current depth because these
			 * mappings will apply to the next depth. This is in
			 * consideration that startElement() will soon be called
			 */

			bool pushed;
			int pushDepth;
			if (shouldFlush)
			{
				flushPending();
				// the prefix mapping applies to the child element (one deeper)
				pushDepth = m_elemContext.m_currentElemDepth + 1;
			}
			else
			{
				// the prefix mapping applies to the current element
				pushDepth = m_elemContext.m_currentElemDepth;
			}
			pushed = m_prefixMap.pushNamespace(prefix, uri, pushDepth);

			if (pushed)
			{
				/* Brian M.: don't know if we really needto do this. The
				 * callers of this object should have injected both
				 * startPrefixMapping and the attributes.  We are 
				 * just covering our butt here.
				 */
				string name;
				if (EMPTYSTRING.Equals(prefix))
				{
					name = "xmlns";
					addAttributeAlways(XMLNS_URI, name, name, "CDATA", uri, false);
				}
				else
				{
					if (!EMPTYSTRING.Equals(uri))
					{ // that maps ns1 prefix to "" URI
						// hack for XSLTC attribset16 test
						name = "xmlns:" + prefix;

						/* for something like xmlns:abc="w3.pretend.org"
						 *  the      uri is the value, that is why we pass it in the
						 * value, or 5th slot of addAttributeAlways()
						 */
						addAttributeAlways(XMLNS_URI, prefix, name, "CDATA", uri, false);
					}
				}
			}
			return pushed;
		}

		/// <summary>
		/// Receive notification of an XML comment anywhere in the document. This
		/// callback will be used for comments inside or outside the document
		/// element, including comments in the external DTD subset (if read). </summary>
		/// <param name="ch"> An array holding the characters in the comment. </param>
		/// <param name="start"> The starting position in the array. </param>
		/// <param name="length"> The number of characters to use from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws org.xml.sax.SAXException
		public virtual void comment(char[] ch, int start, int length)
		{

			int start_old = start;
			if (m_inEntityRef)
			{
				return;
			}
			if (m_elemContext.m_startTagOpen)
			{
				closeStartTag();
				m_elemContext.m_startTagOpen = false;
			}
			else if (m_needToCallStartDocument)
			{
				startDocumentInternal();
				m_needToCallStartDocument = false;
			}

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int limit = start + length;
				int limit = start + length;
				bool wasDash = false;
				if (m_cdataTagOpen)
				{
					closeCDATA();
				}

				if (shouldIndent())
				{
					indent();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				writer.write(COMMENT_BEGIN);
				// Detect occurrences of two consecutive dashes, handle as necessary.
				for (int i = start; i < limit; i++)
				{
					if (wasDash && ch[i] == '-')
					{
						writer.write(ch, start, i - start);
						writer.write(" -");
						start = i + 1;
					}
					wasDash = (ch[i] == '-');
				}

				// if we have some chars in the comment
				if (length > 0)
				{
					// Output the remaining characters (if any)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int remainingChars = (limit - start);
					int remainingChars = (limit - start);
					if (remainingChars > 0)
					{
						writer.write(ch, start, remainingChars);
					}
					// Protect comment end from a single trailing dash
					if (ch[limit - 1] == '-')
					{
						writer.write(' ');
					}
				}
				writer.write(COMMENT_END);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

			/*
			 * Don't write out any indentation whitespace now,
			 * because there may be non-whitespace text after this.
			 * 
			 * Simply mark that at this point if we do decide
			 * to indent that we should 
			 * add a newline on the end of the current line before
			 * the indentation at the start of the next line.
			 */ 
			m_startNewLine = true;
			// time to generate comment event
			if (m_tracer != null)
			{
				base.fireCommentEvent(ch, start_old,length);
			}
		}

		/// <summary>
		/// Report the end of a CDATA section. </summary>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception.
		/// </exception>
		///  <seealso cref=".startCDATA"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
		public virtual void endCDATA()
		{
			if (m_cdataTagOpen)
			{
				closeCDATA();
			}
			m_cdataStartCalled = false;
		}

		/// <summary>
		/// Report the end of DTD declarations. </summary>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
		/// <seealso cref=".startDTD"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public virtual void endDTD()
		{
			try
			{
				if (m_needToOutputDocTypeDecl)
				{
					outputDocTypeDecl(m_elemContext.m_elementName, false);
					m_needToOutputDocTypeDecl = false;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				Writer writer = m_writer;
				if (!m_inDoctype)
				{
					writer.write("]>");
				}
				else
				{
					writer.write('>');
				}

				writer.write(m_lineSep, 0, m_lineSepLen);
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}

		}

		/// <summary>
		/// End the scope of a prefix-URI Namespace mapping. </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.endPrefixMapping"
		////>
		/// <param name="prefix"> The prefix that was being mapping. </param>
		/// <exception cref="org.xml.sax.SAXException"> The client may throw
		///            an exception during processing. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
		public virtual void endPrefixMapping(string prefix)
		{ // do nothing
		}

		/// <summary>
		/// Receive notification of ignorable whitespace in element content.
		/// 
		/// Not sure how to get this invoked quite yet.
		/// </summary>
		/// <param name="ch"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref=".characters"
		////>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws org.xml.sax.SAXException
		public virtual void ignorableWhitespace(char[] ch, int start, int length)
		{

			if (0 == length)
			{
				return;
			}
			characters(ch, start, length);
		}

		/// <summary>
		/// Receive notification of a skipped entity. </summary>
		/// <seealso cref="org.xml.sax.ContentHandler.skippedEntity"
		////>
		/// <param name="name"> The name of the skipped entity.  If it is a
		///       parameter                   entity, the name will begin with '%',
		/// and if it is the external DTD subset, it will be the string
		/// "[dtd]". </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly wrapping
		/// another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
		public virtual void skippedEntity(string name)
		{ // TODO: Should handle
		}

		/// <summary>
		/// Report the start of a CDATA section.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
		/// <seealso cref=".endCDATA"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
		public virtual void startCDATA()
		{
			m_cdataStartCalled = true;
		}

		/// <summary>
		/// Report the beginning of an entity.
		/// 
		/// The start and end of the document entity are not reported.
		/// The start and end of the external DTD subset are reported
		/// using the pseudo-name "[dtd]".  All other events must be
		/// properly nested within start/end entity events.
		/// </summary>
		/// <param name="name"> The name of the entity.  If it is a parameter
		///        entity, the name will begin with '%'. </param>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
		/// <seealso cref=".endEntity"/>
		/// <seealso cref="org.xml.sax.ext.DeclHandler.internalEntityDecl"/>
		/// <seealso cref="org.xml.sax.ext.DeclHandler.externalEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startEntity(String name) throws org.xml.sax.SAXException
		public virtual void startEntity(string name)
		{
			if (name.Equals("[dtd]"))
			{
				m_inExternalDTD = true;
			}

			if (!m_expandDTDEntities && !m_inExternalDTD)
			{
				/* Only leave the entity as-is if
				 * we've been told not to expand them
				 * and this is not the magic [dtd] name.
				 */
				startNonEscaping();
				characters("&" + name + ';');
				endNonEscaping();
			}

			m_inEntityRef = true;
		}

		/// <summary>
		/// For the enclosing elements starting tag write out
		/// out any attributes followed by ">"
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void closeStartTag() throws org.xml.sax.SAXException
		protected internal virtual void closeStartTag()
		{

			if (m_elemContext.m_startTagOpen)
			{

				try
				{
					if (m_tracer != null)
					{
						base.fireStartElem(m_elemContext.m_elementName);
					}
					int nAttrs = m_attributes.getLength();
					if (nAttrs > 0)
					{
						processAttributes(m_writer, nAttrs);
						// clear attributes object for re-use with next element
						m_attributes.clear();
					}
					m_writer.write('>');
				}
				catch (IOException e)
				{
					throw new SAXException(e);
				}

				/* whether Xalan or XSLTC, we have the prefix mappings now, so
				 * lets determine if the current element is specified in the cdata-
				 * section-elements list.
				 */
				if (m_CdataElems != null)
				{
					m_elemContext.m_isCdataSection = CdataSection;
				}

				if (m_doIndent)
				{
					m_isprevtext = false;
					m_preserves.push(m_ispreserve);
				}
			}

		}

		/// <summary>
		/// Report the start of DTD declarations, if any.
		/// 
		/// Any declarations are assumed to be in the internal subset unless
		/// otherwise indicated.
		/// </summary>
		/// <param name="name"> The document type name. </param>
		/// <param name="publicId"> The declared public identifier for the
		///        external DTD subset, or null if none was declared. </param>
		/// <param name="systemId"> The declared system identifier for the
		///        external DTD subset, or null if none was declared. </param>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an
		///            exception. </exception>
		/// <seealso cref=".endDTD"/>
		/// <seealso cref=".startEntity"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void startDTD(string name, string publicId, string systemId)
		{
			DoctypeSystem = systemId;
			DoctypePublic = publicId;

			m_elemContext.m_elementName = name;
			m_inDoctype = true;
		}

		/// <summary>
		/// Returns the m_indentAmount. </summary>
		/// <returns> int </returns>
		public override int IndentAmount
		{
			get
			{
				return m_indentAmount;
			}
			set
			{
				this.m_indentAmount = value;
			}
		}


		/// <summary>
		/// Tell if, based on space preservation constraints and the doIndent property,
		/// if an indent should occur.
		/// </summary>
		/// <returns> True if an indent should occur. </returns>
		protected internal virtual bool shouldIndent()
		{
			return m_doIndent && (!m_ispreserve && !m_isprevtext) && m_elemContext.m_currentElemDepth > 0;
		}

		/// <summary>
		/// Searches for the list of qname properties with the specified key in the
		/// property list. If the key is not found in this property list, the default
		/// property list, and its defaults, recursively, are then checked. The
		/// method returns <code>null</code> if the property is not found.
		/// </summary>
		/// <param name="key">   the property key. </param>
		/// <param name="props"> the list of properties to search in.
		/// 
		/// Sets the vector of local-name/URI pairs of the cdata section elements
		/// specified in the cdata-section-elements property.
		/// 
		/// This method is essentially a copy of getQNameProperties() from
		/// OutputProperties. Eventually this method should go away and a call
		/// to setCdataSectionElements(Vector v) should be made directly. </param>
		private void setCdataSectionElements(string key, Properties props)
		{

			string s = props.getProperty(key);

			if (null != s)
			{
				// Vector of URI/LocalName pairs
				ArrayList v = new ArrayList();
				int l = s.Length;
				bool inCurly = false;
				StringBuilder buf = new StringBuilder();

				// parse through string, breaking on whitespaces.  I do this instead
				// of a tokenizer so I can track whitespace inside of curly brackets,
				// which theoretically shouldn't happen if they contain legal URLs.
				for (int i = 0; i < l; i++)
				{
					char c = s[i];

					if (char.IsWhiteSpace(c))
					{
						if (!inCurly)
						{
							if (buf.Length > 0)
							{
								addCdataSectionElement(buf.ToString(), v);
								buf.Length = 0;
							}
							continue;
						}
					}
					else if ('{' == c)
					{
						inCurly = true;
					}
					else if ('}' == c)
					{
						inCurly = false;
					}

					buf.Append(c);
				}

				if (buf.Length > 0)
				{
					addCdataSectionElement(buf.ToString(), v);
					buf.Length = 0;
				}
				// call the official, public method to set the collected names
				CdataSectionElements = v;
			}

		}

		/// <summary>
		/// Adds a URI/LocalName pair of strings to the list.
		/// </summary>
		/// <param name="URI_and_localName"> String of the form "{uri}local" or "local" 
		/// </param>
		/// <returns> a QName object </returns>
		private void addCdataSectionElement(string URI_and_localName, ArrayList v)
		{

			StringTokenizer tokenizer = new StringTokenizer(URI_and_localName, "{}", false);
			string s1 = tokenizer.nextToken();
			string s2 = tokenizer.hasMoreTokens() ? tokenizer.nextToken() : null;

			if (null == s2)
			{
				// add null URI and the local name
				v.Add(null);
				v.Add(s1);
			}
			else
			{
				// add URI, then local name
				v.Add(s1);
				v.Add(s2);
			}
		}

		/// <summary>
		/// Remembers the cdata sections specified in the cdata-section-elements.
		/// The "official way to set URI and localName pairs. 
		/// This method should be used by both Xalan and XSLTC.
		/// </summary>
		/// <param name="URI_and_localNames"> a vector of pairs of Strings (URI/local) </param>
		public override void setCdataSectionElements(ArrayList URI_and_localNames)
		{
			// convert to the new way.
			if (URI_and_localNames != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = URI_and_localNames.size() - 1;
				int len = URI_and_localNames.Count - 1;
				if (len > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < len; i += 2)
					{
						// whitspace separated "{uri1}local1 {uri2}local2 ..."
						if (i != 0)
						{
							sb.Append(' ');
						}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = (String) URI_and_localNames.elementAt(i);
						string uri = (string) URI_and_localNames[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = (String) URI_and_localNames.elementAt(i + 1);
						string localName = (string) URI_and_localNames[i + 1];
						if (!string.ReferenceEquals(uri, null))
						{
							// If there is no URI don't put this in, just the localName then.
							sb.Append('{');
							sb.Append(uri);
							sb.Append('}');
						}
						sb.Append(localName);
					}
					m_StringOfCDATASections = sb.ToString();
				}
			}
			initCdataElems(m_StringOfCDATASections);
		}

		/// <summary>
		/// Makes sure that the namespace URI for the given qualified attribute name
		/// is declared. </summary>
		/// <param name="ns"> the namespace URI </param>
		/// <param name="rawName"> the qualified name </param>
		/// <returns> returns null if no action is taken, otherwise it returns the
		/// prefix used in declaring the namespace. </returns>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected String ensureAttributesNamespaceIsDeclared(String ns, String localName, String rawName) throws org.xml.sax.SAXException
		protected internal virtual string ensureAttributesNamespaceIsDeclared(string ns, string localName, string rawName)
		{

			if (!string.ReferenceEquals(ns, null) && ns.Length > 0)
			{

				// extract the prefix in front of the raw name
				int index = 0;
				string prefixFromRawName = (index = rawName.IndexOf(":", StringComparison.Ordinal)) < 0 ? "" : rawName.Substring(0, index);

				if (index > 0)
				{
					// we have a prefix, lets see if it maps to a namespace 
					string uri = m_prefixMap.lookupNamespace(prefixFromRawName);
					if (!string.ReferenceEquals(uri, null) && uri.Equals(ns))
					{
						// the prefix in the raw name is already maps to the given namespace uri
						// so we don't need to do anything
						return null;
					}
					else
					{
						// The uri does not map to the prefix in the raw name,
						// so lets make the mapping.
						this.startPrefixMapping(prefixFromRawName, ns, false);
						this.addAttribute("http://www.w3.org/2000/xmlns/", prefixFromRawName, "xmlns:" + prefixFromRawName, "CDATA", ns, false);
						return prefixFromRawName;
					}
				}
				else
				{
					// we don't have a prefix in the raw name.
					// Does the URI map to a prefix already?
					string prefix = m_prefixMap.lookupPrefix(ns);
					if (string.ReferenceEquals(prefix, null))
					{
						// uri is not associated with a prefix,
						// so lets generate a new prefix to use
						prefix = m_prefixMap.generateNextPrefix();
						this.startPrefixMapping(prefix, ns, false);
						this.addAttribute("http://www.w3.org/2000/xmlns/", prefix, "xmlns:" + prefix, "CDATA", ns, false);
					}

					return prefix;

				}
			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void ensurePrefixIsDeclared(String ns, String rawName) throws org.xml.sax.SAXException
		internal virtual void ensurePrefixIsDeclared(string ns, string rawName)
		{

			if (!string.ReferenceEquals(ns, null) && ns.Length > 0)
			{
				int index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean no_prefix = ((index = rawName.indexOf(":")) < 0);
				bool no_prefix = ((index = rawName.IndexOf(":", StringComparison.Ordinal)) < 0);
				string prefix = (no_prefix) ? "" : rawName.Substring(0, index);

				if (null != prefix)
				{
					string foundURI = m_prefixMap.lookupNamespace(prefix);

					if ((null == foundURI) || !foundURI.Equals(ns))
					{
						this.startPrefixMapping(prefix, ns);

						// Bugzilla1133: Generate attribute as well as namespace event.
						// SAX does expect both.

						this.addAttributeAlways("http://www.w3.org/2000/xmlns/", no_prefix ? "xmlns" : prefix, no_prefix ? "xmlns" : ("xmlns:" + prefix), "CDATA", ns, false);
					}

				}
			}
		}

		/// <summary>
		/// This method flushes any pending events, which can be startDocument()
		/// closing the opening tag of an element, or closing an open CDATA section.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void flushPending() throws org.xml.sax.SAXException
		public override void flushPending()
		{
				if (m_needToCallStartDocument)
				{
					startDocumentInternal();
					m_needToCallStartDocument = false;
				}
				if (m_elemContext.m_startTagOpen)
				{
					closeStartTag();
					m_elemContext.m_startTagOpen = false;
				}

				if (m_cdataTagOpen)
				{
					closeCDATA();
					m_cdataTagOpen = false;
				}
				if (m_writer != null)
				{
					try
					{
						m_writer.flush();
					}
					catch (IOException)
					{
						// what? me worry?
					}
				}
		}

		public override ContentHandler ContentHandler
		{
			set
			{
				// this method is really only useful in the ToSAXHandler classes but it is
				// in the interface.  If the method defined here is ever called
				// we are probably in trouble.
			}
		}

		/// <summary>
		/// Adds the given attribute to the set of attributes, even if there is
		/// no currently open element. This is useful if a SAX startPrefixMapping()
		/// should need to add an attribute before the element name is seen.
		/// 
		/// This method is a copy of its super classes method, except that some
		/// tracing of events is done.  This is so the tracing is only done for
		/// stream serializers, not for SAX ones.
		/// </summary>
		/// <param name="uri"> the URI of the attribute </param>
		/// <param name="localName"> the local name of the attribute </param>
		/// <param name="rawName">   the qualified name of the attribute </param>
		/// <param name="type"> the type of the attribute (probably CDATA) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <param name="xslAttribute"> true if this attribute is coming from an xsl:attribute element. </param>
		/// <returns> true if the attribute value was added, 
		/// false if the attribute already existed and the value was
		/// replaced with the new value. </returns>
		public override bool addAttributeAlways(string uri, string localName, string rawName, string type, string value, bool xslAttribute)
		{
			bool was_added;
			int index;
			if (string.ReferenceEquals(uri, null) || string.ReferenceEquals(localName, null) || uri.Length == 0)
			{
				index = m_attributes.getIndex(rawName);
			}
			else
			{
				index = m_attributes.getIndex(uri, localName);
			}

			if (index >= 0)
			{
				string old_value = null;
				if (m_tracer != null)
				{
					old_value = m_attributes.getValue(index);
					if (value.Equals(old_value))
					{
						old_value = null;
					}
				}

				/* We've seen the attribute before.
				 * We may have a null uri or localName, but all we really
				 * want to re-set is the value anyway.
				 */
				m_attributes.setValue(index, value);
				was_added = false;
				if (!string.ReferenceEquals(old_value, null))
				{
					firePseudoAttributes();
				}

			}
			else
			{
				// the attribute doesn't exist yet, create it
				if (xslAttribute)
				{
					/*
					 * This attribute is from an xsl:attribute element so we take some care in
					 * adding it, e.g.
					 *   <elem1  foo:attr1="1" xmlns:foo="uri1">
					 *       <xsl:attribute name="foo:attr2">2</xsl:attribute>
					 *   </elem1>
					 * 
					 * We are adding attr1 and attr2 both as attributes of elem1,
					 * and this code is adding attr2 (the xsl:attribute ).
					 * We could have a collision with the prefix like in the example above.
					 */

					// In the example above, is there a prefix like foo ?
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colonIndex = rawName.indexOf(':');
					int colonIndex = rawName.IndexOf(':');
					if (colonIndex > 0)
					{
						string prefix = rawName.Substring(0, colonIndex);
						NamespaceMappings.MappingRecord existing_mapping = m_prefixMap.getMappingFromPrefix(prefix);

						/* Before adding this attribute (foo:attr2),
						 * is the prefix for it (foo) already mapped at the current depth?
						 */
						if (existing_mapping != null && existing_mapping.m_declarationDepth == m_elemContext.m_currentElemDepth && !existing_mapping.m_uri.Equals(uri))
						{
							/*
							 * There is an existing mapping of this prefix,
							 * it differs from the one we need,
							 * and unfortunately it is at the current depth so we 
							 * can not over-ride it.
							 */

							/*
							 * Are we lucky enough that an existing other prefix maps to this URI ?
							 */
							prefix = m_prefixMap.lookupPrefix(uri);
							if (string.ReferenceEquals(prefix, null))
							{
								/* Unfortunately there is no existing prefix that happens to map to ours,
								 * so to avoid a prefix collision we must generated a new prefix to use. 
								 * This is OK because the prefix URI mapping
								 * defined in the xsl:attribute is short in scope, 
								 * just the xsl:attribute element itself, 
								 * and at this point in serialization the body of the
								 * xsl:attribute, if any, is just a String. Right?
								 *   . . . I sure hope so - Brian M. 
								 */
								prefix = m_prefixMap.generateNextPrefix();
							}

							rawName = prefix + ':' + localName;
						}
					}

					try
					{
						/* This is our last chance to make sure the namespace for this
						 * attribute is declared, especially if we just generated an alternate
						 * prefix to avoid a collision (the new prefix/rawName will go out of scope
						 * soon and be lost ...  last chance here.
						 */
						string prefixUsed = ensureAttributesNamespaceIsDeclared(uri, localName, rawName);
					}
					catch (SAXException e)
					{
						// TODO Auto-generated catch block
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
				m_attributes.addAttribute(uri, localName, rawName, type, value);
				was_added = true;
				if (m_tracer != null)
				{
					firePseudoAttributes();
				}
			}
			return was_added;
		}

		/// <summary>
		/// To fire off the pseudo characters of attributes, as they currently
		/// exist. This method should be called everytime an attribute is added,
		/// or when an attribute value is changed, or an element is created.
		/// </summary>

		protected internal virtual void firePseudoAttributes()
		{
			if (m_tracer != null)
			{
				try
				{
					// flush out the "<elemName" if not already flushed
					m_writer.flush();

					// make a StringBuffer to write the name="value" pairs to.
					StringBuilder sb = new StringBuilder();
					int nAttrs = m_attributes.getLength();
					if (nAttrs > 0)
					{
						// make a writer that internally appends to the same
						// StringBuffer
						Writer writer = new ToStream.WritertoStringBuffer(sb);

						processAttributes(writer, nAttrs);
						// Don't clear the attributes! 
						// We only want to see what would be written out
						// at this point, we don't want to loose them.
					}
					sb.Append('>'); // the potential > after the attributes.
					// convert the StringBuffer to a char array and
					// emit the trace event that these characters "might"
					// be written                
					char[] ch = sb.ToString().ToCharArray();
					m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_OUTPUT_PSEUDO_CHARACTERS, ch, 0, ch.Length);
				}
				catch (IOException)
				{
					// ignore ?
				}
				catch (SAXException)
				{
					// ignore ?
				}
			}
		}

		/// <summary>
		/// This inner class is used only to collect attribute values
		/// written by the method writeAttrString() into a string buffer.
		/// In this manner trace events, and the real writing of attributes will use
		/// the same code.
		/// </summary>
		private class WritertoStringBuffer : Writer
		{
			internal readonly StringBuilder m_stringbuf;
			/// <seealso cref="java.io.Writer.write(char[], int, int)"/>
			internal WritertoStringBuffer(StringBuilder sb)
			{
				m_stringbuf = sb;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void write(char[] arg0, int arg1, int arg2) throws java.io.IOException
			public virtual void write(char[] arg0, int arg1, int arg2)
			{
				m_stringbuf.Append(arg0, arg1, arg2);
			}
			/// <seealso cref="java.io.Writer.flush()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void flush() throws java.io.IOException
			public virtual void flush()
			{
			}
			/// <seealso cref="java.io.Writer.close()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void close() throws java.io.IOException
			public virtual void close()
			{
			}

			public virtual void write(int i)
			{
				m_stringbuf.Append((char) i);
			}

			public virtual void write(string s)
			{
				m_stringbuf.Append(s);
			}
		}

		/// <seealso cref="SerializationHandler.setTransformer(Transformer)"/>
		public override Transformer Transformer
		{
			set
			{
				base.Transformer = value;
				if (m_tracer != null && !(m_writer is SerializerTraceWriter))
				{
					setWriterInternal(new SerializerTraceWriter(m_writer, m_tracer), false);
				}
    
    
			}
		}
		/// <summary>
		/// Try's to reset the super class and reset this class for 
		/// re-use, so that you don't need to create a new serializer 
		/// (mostly for performance reasons).
		/// </summary>
		/// <returns> true if the class was successfuly reset. </returns>
		public override bool reset()
		{
			bool wasReset = false;
			if (base.reset())
			{
				resetToStream();
				wasReset = true;
			}
			return wasReset;
		}

		/// <summary>
		/// Reset all of the fields owned by ToStream class
		/// 
		/// </summary>
		private void resetToStream()
		{
			 this.m_cdataStartCalled = false;
			 /* The stream is being reset. It is one of
			  * ToXMLStream, ToHTMLStream ... and this type can't be changed
			  * so neither should m_charInfo which is associated with the
			  * type of Stream. Just leave m_charInfo as-is for the next re-use.
			  * 
			  */
			 // this.m_charInfo = null; // don't set to null 
			 this.m_disableOutputEscapingStates.clear();
			 // this.m_encodingInfo = null; // don't set to null

			 this.m_escaping = true;
			 // Leave m_format alone for now - Brian M.
			 // this.m_format = null;
			 this.m_expandDTDEntities = true;
			 this.m_inDoctype = false;
			 this.m_ispreserve = false;
			 this.m_isprevtext = false;
			 this.m_isUTF8 = false; //  ?? used anywhere ??
			 this.m_lineSep = s_systemLineSep;
			 this.m_lineSepLen = s_systemLineSep.Length;
			 this.m_lineSepUse = true;
			 // this.m_outputStream = null; // Don't reset it may be re-used
			 this.m_preserves.clear();
			 this.m_shouldFlush = true;
			 this.m_spaceBeforeClose = false;
			 this.m_startNewLine = false;
			 this.m_writer_set_by_user = false;
		}

		/// <summary>
		/// Sets the character encoding coming from the xsl:output encoding stylesheet attribute. </summary>
		/// <param name="encoding"> the character encoding </param>
		 public override string Encoding
		 {
			 set
			 {
				 setOutputProperty(OutputKeys.ENCODING,value);
			 }
		 }

		/// <summary>
		/// Simple stack for boolean values.
		/// 
		/// This class is a copy of the one in org.apache.xml.utils. 
		/// It exists to cut the serializers dependancy on that package.
		/// A minor changes from that package are:
		/// doesn't implement Clonable
		/// 
		/// @xsl.usage internal
		/// </summary>
		internal sealed class BoolStack
		{

		  /// <summary>
		  /// Array of boolean values </summary>
		  internal bool[] m_values;

		  /// <summary>
		  /// Array size allocated </summary>
		  internal int m_allocatedSize;

		  /// <summary>
		  /// Index into the array of booleans </summary>
		  internal int m_index;

		  /// <summary>
		  /// Default constructor.  Note that the default
		  /// block size is very small, for small lists.
		  /// </summary>
		  public BoolStack() : this(32)
		  {
		  }

		  /// <summary>
		  /// Construct a IntVector, using the given block size.
		  /// </summary>
		  /// <param name="size"> array size to allocate </param>
		  public BoolStack(int size)
		  {

			m_allocatedSize = size;
			m_values = new bool[size];
			m_index = -1;
		  }

		  /// <summary>
		  /// Get the length of the list.
		  /// </summary>
		  /// <returns> Current length of the list </returns>
		  public int size()
		  {
			return m_index + 1;
		  }

		  /// <summary>
		  /// Clears the stack.
		  /// 
		  /// </summary>
		  public void clear()
		  {
			m_index = -1;
		  }

		  /// <summary>
		  /// Pushes an item onto the top of this stack.
		  /// 
		  /// </summary>
		  /// <param name="val"> the boolean to be pushed onto this stack. </param>
		  /// <returns>  the <code>item</code> argument. </returns>
		  public bool push(bool val)
		  {

			if (m_index == m_allocatedSize - 1)
			{
			  grow();
			}

			return (m_values[++m_index] = val);
		  }

		  /// <summary>
		  /// Removes the object at the top of this stack and returns that
		  /// object as the value of this function.
		  /// </summary>
		  /// <returns>     The object at the top of this stack. </returns>
		  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
		  public bool pop()
		  {
			return m_values[m_index--];
		  }

		  /// <summary>
		  /// Removes the object at the top of this stack and returns the
		  /// next object at the top as the value of this function.
		  /// 
		  /// </summary>
		  /// <returns> Next object to the top or false if none there </returns>
		  public bool popAndTop()
		  {

			m_index--;

			return (m_index >= 0) ? m_values[m_index] : false;
		  }

		  /// <summary>
		  /// Set the item at the top of this stack  
		  /// 
		  /// </summary>
		  /// <param name="b"> Object to set at the top of this stack </param>
		  public bool Top
		  {
			  set
			  {
				m_values[m_index] = value;
			  }
		  }

		  /// <summary>
		  /// Looks at the object at the top of this stack without removing it
		  /// from the stack.
		  /// </summary>
		  /// <returns>     the object at the top of this stack. </returns>
		  /// <exception cref="EmptyStackException">  if this stack is empty. </exception>
		  public bool peek()
		  {
			return m_values[m_index];
		  }

		  /// <summary>
		  /// Looks at the object at the top of this stack without removing it
		  /// from the stack.  If the stack is empty, it returns false.
		  /// </summary>
		  /// <returns>     the object at the top of this stack. </returns>
		  public bool peekOrFalse()
		  {
			return (m_index > -1) ? m_values[m_index] : false;
		  }

		  /// <summary>
		  /// Looks at the object at the top of this stack without removing it
		  /// from the stack.  If the stack is empty, it returns true.
		  /// </summary>
		  /// <returns>     the object at the top of this stack. </returns>
		  public bool peekOrTrue()
		  {
			return (m_index > -1) ? m_values[m_index] : true;
		  }

		  /// <summary>
		  /// Tests if this stack is empty.
		  /// </summary>
		  /// <returns>  <code>true</code> if this stack is empty;
		  ///          <code>false</code> otherwise. </returns>
		  public bool Empty
		  {
			  get
			  {
				return (m_index == -1);
			  }
		  }

		  /// <summary>
		  /// Grows the size of the stack
		  /// 
		  /// </summary>
		  internal void grow()
		  {

			m_allocatedSize *= 2;

			bool[] newVector = new bool[m_allocatedSize];

			Array.Copy(m_values, 0, newVector, 0, m_index + 1);

			m_values = newVector;
		  }
		}

		// Implement DTDHandler
		/// <summary>
		/// If this method is called, the serializer is used as a
		/// DTDHandler, which changes behavior how the serializer 
		/// handles document entities. </summary>
		/// <seealso cref="org.xml.sax.DTDHandler.notationDecl(java.lang.String, java.lang.String, java.lang.String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void notationDecl(String name, String pubID, String sysID) throws org.xml.sax.SAXException
		public override void notationDecl(string name, string pubID, string sysID)
		{
			// TODO Auto-generated method stub
			try
			{
				DTDprolog();

				m_writer.write("<!NOTATION ");
				m_writer.write(name);
				if (!string.ReferenceEquals(pubID, null))
				{
					m_writer.write(" PUBLIC \"");
					m_writer.write(pubID);

				}
				else
				{
					m_writer.write(" SYSTEM \"");
					m_writer.write(sysID);
				}
				m_writer.write("\" >");
				m_writer.write(m_lineSep, 0, m_lineSepLen);
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/// <summary>
		/// If this method is called, the serializer is used as a
		/// DTDHandler, which changes behavior how the serializer 
		/// handles document entities. </summary>
		/// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl(java.lang.String, java.lang.String, java.lang.String, java.lang.String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void unparsedEntityDecl(String name, String pubID, String sysID, String notationName) throws org.xml.sax.SAXException
		public override void unparsedEntityDecl(string name, string pubID, string sysID, string notationName)
		{
			// TODO Auto-generated method stub
			try
			{
				DTDprolog();

				m_writer.write("<!ENTITY ");
				m_writer.write(name);
				if (!string.ReferenceEquals(pubID, null))
				{
					m_writer.write(" PUBLIC \"");
					m_writer.write(pubID);

				}
				else
				{
					m_writer.write(" SYSTEM \"");
					m_writer.write(sysID);
				}
				m_writer.write("\" NDATA ");
				m_writer.write(notationName);
				m_writer.write(" >");
				m_writer.write(m_lineSep, 0, m_lineSepLen);
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/// <summary>
		/// A private helper method to output the </summary>
		/// <exception cref="SAXException"> </exception>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void DTDprolog() throws SAXException, java.io.IOException
		private void DTDprolog()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			Writer writer = m_writer;
			if (m_needToOutputDocTypeDecl)
			{
				outputDocTypeDecl(m_elemContext.m_elementName, false);
				m_needToOutputDocTypeDecl = false;
			}
			if (m_inDoctype)
			{
				writer.write(" [");
				writer.write(m_lineSep, 0, m_lineSepLen);
				m_inDoctype = false;
			}
		}

		/// <summary>
		/// If set to false the serializer does not expand DTD entities,
		/// but leaves them as is, the default value is true;
		/// </summary>
		public override bool DTDEntityExpansion
		{
			set
			{
				m_expandDTDEntities = value;
			}
		}

		/// <summary>
		/// Sets the end of line characters to be used during serialization </summary>
		/// <param name="eolChars"> A character array corresponding to the characters to be used. </param>
		public virtual char[] NewLine
		{
			set
			{
				m_lineSep = value;
				m_lineSepLen = value.Length;
			}
		}

		/// <summary>
		/// Remembers the cdata sections specified in the cdata-section-elements by appending the given
		/// cdata section elements to the list. This method can be called multiple times, but once an
		/// element is put in the list of cdata section elements it can not be removed.
		/// This method should be used by both Xalan and XSLTC.
		/// </summary>
		/// <param name="URI_and_localNames"> a whitespace separated list of element names, each element
		/// is a URI in curly braces (optional) and a local name. An example of such a parameter is:
		/// "{http://company.com}price {myURI2}book chapter" </param>
		public virtual void addCdataSectionElements(string URI_and_localNames)
		{
			if (!string.ReferenceEquals(URI_and_localNames, null))
			{
				initCdataElems(URI_and_localNames);
			}
			if (string.ReferenceEquals(m_StringOfCDATASections, null))
			{
				m_StringOfCDATASections = URI_and_localNames;
			}
			else
			{
				m_StringOfCDATASections += (" " + URI_and_localNames);
			}
		}
	}

}