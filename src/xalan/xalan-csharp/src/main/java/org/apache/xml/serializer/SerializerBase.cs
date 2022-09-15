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
 * $Id: SerializerBase.java 471981 2006-11-07 04:28:00Z minchau $
 */
namespace org.apache.xml.serializer
{


	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;


	/// <summary>
	/// This class acts as a base class for the XML "serializers"
	/// and the stream serializers.
	/// It contains a number of common fields and methods.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public abstract class SerializerBase : SerializationHandler, SerializerConstants
	{
		public abstract java.util.Properties OutputFormat {get;}
		public abstract void setOutputFormat(java.util.Properties format);
		public abstract java.io.Writer Writer {get;}
		public abstract void setWriter(java.io.Writer writer);
		public abstract Stream OutputStream {get;}
		public abstract void setOutputStream(Stream output);
		public abstract void setCdataSectionElements(ArrayList URI_and_localNames);
		public abstract void addUniqueAttribute(string qName, string value, int flags);
		public abstract bool startPrefixMapping(string prefix, string uri, bool shouldFlush);
		public abstract void startElement(string qName);
		public abstract void startElement(string uri, string localName, string qName);
		public abstract void endElement(string elemName);
		public abstract void characters(string chars);
		public abstract void flushPending();
		public abstract bool setEscaping(bool escape);
		public abstract void serialize(org.w3c.dom.Node node);
		public abstract void setContentHandler(ContentHandler ch);
		internal SerializerBase()
		{
			return;
		}

		/// <summary>
		/// The name of the package that this class is in.
		/// <para>
		/// Not a public API.
		/// </para>
		/// </summary>
		public static readonly string PKG_NAME;

		/// <summary>
		/// The same as the name of the package that this class is in
		/// except that '.' are replaced with '/'.
		/// <para>
		/// Not a public API.
		/// </para>
		/// </summary>
		public static readonly string PKG_PATH;

		static SerializerBase()
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			string fullyQualifiedName = typeof(SerializerBase).FullName;
			int lastDot = fullyQualifiedName.LastIndexOf('.');
			if (lastDot < 0)
			{
				PKG_NAME = "";
			}
			else
			{
				PKG_NAME = fullyQualifiedName.Substring(0, lastDot);
			}

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < PKG_NAME.Length; i++)
			{
				char ch = PKG_NAME[i];
				if (ch == '.')
				{
					sb.Append('/');
				}
				else
				{
					sb.Append(ch);
				}
			}
			PKG_PATH = sb.ToString();
		}



		/// <summary>
		/// To fire off the end element trace event </summary>
		/// <param name="name"> Name of element </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireEndElem(String name) throws org.xml.sax.SAXException
		protected internal virtual void fireEndElem(string name)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_ENDELEMENT,name, (Attributes)null);
			}
		}

		/// <summary>
		/// Report the characters trace event </summary>
		/// <param name="chars">  content of characters </param>
		/// <param name="start">  starting index of characters to output </param>
		/// <param name="length">  number of characters to output </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireCharEvent(char[] chars, int start, int length) throws org.xml.sax.SAXException
		protected internal virtual void fireCharEvent(char[] chars, int start, int length)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_CHARACTERS, chars, start,length);
			}
		}

		/// <summary>
		/// true if we still need to call startDocumentInternal() 
		/// </summary>
		protected internal bool m_needToCallStartDocument = true;

		/// <summary>
		/// True if a trailing "]]>" still needs to be written to be
		/// written out. Used to merge adjacent CDATA sections
		/// </summary>
		protected internal bool m_cdataTagOpen = false;

		/// <summary>
		/// All the attributes of the current element, collected from
		/// startPrefixMapping() calls, or addAddtribute() calls, or 
		/// from the SAX attributes in a startElement() call.
		/// </summary>
		protected internal AttributesImplSerializer m_attributes = new AttributesImplSerializer();

		/// <summary>
		/// Tells if we're in an EntityRef event.
		/// </summary>
		protected internal bool m_inEntityRef = false;

		/// <summary>
		/// This flag is set while receiving events from the external DTD </summary>
		protected internal bool m_inExternalDTD = false;

		/// <summary>
		/// The System ID for the doc type.
		/// </summary>
		protected internal string m_doctypeSystem;

		/// <summary>
		/// The public ID for the doc type.
		/// </summary>
		protected internal string m_doctypePublic;

		/// <summary>
		/// Flag to tell that we need to add the doctype decl, which we can't do
		/// until the first element is encountered.
		/// </summary>
		internal bool m_needToOutputDocTypeDecl = true;

		/// <summary>
		/// Tells if we should write the XML declaration.
		/// </summary>
		protected internal bool m_shouldNotWriteXMLHeader = false;

		/// <summary>
		/// The standalone value for the doctype.
		/// </summary>
		private string m_standalone;

		/// <summary>
		/// True if standalone was specified.
		/// </summary>
		protected internal bool m_standaloneWasSpecified = false;

		/// <summary>
		/// Flag to tell if indenting (pretty-printing) is on.
		/// </summary>
		protected internal bool m_doIndent = false;
		/// <summary>
		/// Amount to indent.
		/// </summary>
		protected internal int m_indentAmount = 0;

		/// <summary>
		/// Tells the XML version, for writing out to the XML decl.
		/// </summary>
		protected internal string m_version = null;

		/// <summary>
		/// The mediatype.  Not used right now.
		/// </summary>
		protected internal string m_mediatype;

		/// <summary>
		/// The transformer that was around when this output handler was created (if
		/// any).
		/// </summary>
		private Transformer m_transformer;

		/// <summary>
		/// Namespace support, that keeps track of currently defined 
		/// prefix/uri mappings. As processed elements come and go, so do
		/// the associated mappings for that element.
		/// </summary>
		protected internal NamespaceMappings m_prefixMap;

		/// <summary>
		/// Handle for firing generate events.  This interface may be implemented
		/// by the referenced transformer object.
		/// </summary>
		protected internal SerializerTrace m_tracer;

		protected internal SourceLocator m_sourceLocator;


		/// <summary>
		/// The writer to send output to. This field is only used in the ToStream
		/// serializers, but exists here just so that the fireStartDoc() and
		/// other fire... methods can flush this writer when tracing.
		/// </summary>
		protected internal java.io.Writer m_writer = null;

		/// <summary>
		/// A reference to "stack frame" corresponding to
		/// the current element. Such a frame is pushed at a startElement()
		/// and popped at an endElement(). This frame contains information about
		/// the element, such as its namespace URI. 
		/// </summary>
		protected internal ElemContext m_elemContext = new ElemContext();

		/// <summary>
		/// A utility buffer for converting Strings passed to
		/// character() methods to character arrays.
		/// Reusing this buffer means not creating a new character array
		/// everytime and it runs faster.
		/// </summary>
		protected internal char[] m_charsBuff = new char[60];

		/// <summary>
		/// A utility buffer for converting Strings passed to
		/// attribute methods to character arrays.
		/// Reusing this buffer means not creating a new character array
		/// everytime and it runs faster.
		/// </summary>
		protected internal char[] m_attrBuff = new char[30];

		/// <summary>
		/// Receive notification of a comment.
		/// </summary>
		/// <seealso cref="ExtendedLexicalHandler.comment(String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(String data) throws org.xml.sax.SAXException
		public virtual void comment(string data)
		{
			m_docIsEmpty = false;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = data.length();
			int length = data.Length;
			if (length > m_charsBuff.Length)
			{
				m_charsBuff = new char[length * 2 + 1];
			}
			data.CopyTo(0, m_charsBuff, 0, length - 0);
			comment(m_charsBuff, 0, length);
		}

		/// <summary>
		/// If at runtime, when the qname of the attribute is
		/// known, another prefix is specified for the attribute, then we can
		/// patch or hack the name with this method. For
		/// a qname of the form "ns?:otherprefix:name", this function patches the
		/// qname by simply ignoring "otherprefix".
		/// TODO: This method is a HACK! We do not have access to the
		/// XML file, it sometimes generates a NS prefix of the form "ns?" for
		/// an attribute.
		/// </summary>
		protected internal virtual string patchName(string qname)
		{


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastColon = qname.lastIndexOf(':');
			int lastColon = qname.LastIndexOf(':');

			if (lastColon > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int firstColon = qname.indexOf(':');
				int firstColon = qname.IndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = qname.substring(0, firstColon);
				string prefix = qname.Substring(0, firstColon);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = qname.substring(lastColon + 1);
				string localName = qname.Substring(lastColon + 1);

			// If uri is "" then ignore prefix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = m_prefixMap.lookupNamespace(prefix);
				string uri = m_prefixMap.lookupNamespace(prefix);
				if (!string.ReferenceEquals(uri, null) && uri.Length == 0)
				{
					return localName;
				}
				else if (firstColon != lastColon)
				{
					return prefix + ':' + localName;
				}
			}
			return qname;
		}

		/// <summary>
		/// Returns the local name of a qualified name. If the name has no prefix,
		/// then it works as the identity (SAX2). </summary>
		/// <param name="qname"> the qualified name </param>
		/// <returns> the name, but excluding any prefix and colon. </returns>
		protected internal static string getLocalName(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = qname.lastIndexOf(':');
			int col = qname.LastIndexOf(':');
			return (col > 0) ? qname.Substring(col + 1) : qname;
		}

		/// <summary>
		/// Receive an object for locating the origin of SAX document events.
		/// </summary>
		/// <param name="locator"> An object that can return the location of any SAX document
		/// event.
		/// 
		/// Receive an object for locating the origin of SAX document events.
		/// 
		/// <para>SAX parsers are strongly encouraged (though not absolutely
		/// required) to supply a locator: if it does so, it must supply
		/// the locator to the application by invoking this method before
		/// invoking any of the other methods in the DocumentHandler
		/// interface.</para>
		/// 
		/// <para>The locator allows the application to determine the end
		/// position of any document-related event, even if the parser is
		/// not reporting an error.  Typically, the application will
		/// use this information for reporting its own errors (such as
		/// character content that does not match an application's
		/// business rules).  The information returned by the locator
		/// is probably not sufficient for use with a search engine.</para>
		/// 
		/// <para>Note that the locator will return correct information only
		/// during the invocation of the events in this interface.  The
		/// application should not attempt to use it at any other time.</para> </param>
		public virtual Locator DocumentLocator
		{
			set
			{
				return;
    
				// I don't do anything with this yet.
			}
		}

		/// <summary>
		/// Adds the given attribute to the set of collected attributes , but only if
		/// there is a currently open element.
		/// 
		/// An element is currently open if a startElement() notification has
		/// occured but the start of the element has not yet been written to the
		/// output.  In the stream case this means that we have not yet been forced
		/// to close the elements opening tag by another notification, such as a
		/// character notification.
		/// </summary>
		/// <param name="uri"> the URI of the attribute </param>
		/// <param name="localName"> the local name of the attribute </param>
		/// <param name="rawName">    the qualified name of the attribute </param>
		/// <param name="type"> the type of the attribute (probably CDATA) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <param name="XSLAttribute"> true if this attribute is coming from an xsl:attriute element </param>
		/// <seealso cref="ExtendedContentHandler.addAttribute(String, String, String, String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value, boolean XSLAttribute) throws org.xml.sax.SAXException
		public virtual void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
			if (m_elemContext.m_startTagOpen)
			{
				addAttributeAlways(uri, localName, rawName, type, value, XSLAttribute);
			}

		}

		/// <summary>
		/// Adds the given attribute to the set of attributes, even if there is
		/// no currently open element. This is useful if a SAX startPrefixMapping()
		/// should need to add an attribute before the element name is seen.
		/// </summary>
		/// <param name="uri"> the URI of the attribute </param>
		/// <param name="localName"> the local name of the attribute </param>
		/// <param name="rawName">   the qualified name of the attribute </param>
		/// <param name="type"> the type of the attribute (probably CDATA) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <param name="XSLAttribute"> true if this attribute is coming from an xsl:attribute element </param>
		/// <returns> true if the attribute was added, 
		/// false if an existing value was replaced. </returns>
		public virtual bool addAttributeAlways(string uri, string localName, string rawName, string type, string value, bool XSLAttribute)
		{
			bool was_added;
	//            final int index =
	//                (localName == null || uri == null) ?
	//                m_attributes.getIndex(rawName):m_attributes.getIndex(uri, localName);        
				int index;
	//            if (localName == null || uri == null){
	//                index = m_attributes.getIndex(rawName);
	//            }
	//            else {
	//                index = m_attributes.getIndex(uri, localName);
	//            }
				if (string.ReferenceEquals(localName, null) || string.ReferenceEquals(uri, null) || uri.Length == 0)
				{
					index = m_attributes.getIndex(rawName);
				}
				else
				{
					index = m_attributes.getIndex(uri,localName);
				}
				if (index >= 0)
				{
					/* We've seen the attribute before.
					 * We may have a null uri or localName, but all
					 * we really want to re-set is the value anyway.
					 */
					m_attributes.setValue(index,value);
					was_added = false;
				}
				else
				{
					// the attribute doesn't exist yet, create it
					m_attributes.addAttribute(uri, localName, rawName, type, value);
					was_added = true;
				}
				return was_added;

		}


		/// <summary>
		///  Adds  the given attribute to the set of collected attributes, 
		/// but only if there is a currently open element.
		/// </summary>
		/// <param name="name"> the attribute's qualified name </param>
		/// <param name="value"> the value of the attribute </param>
		public virtual void addAttribute(string name, in string value)
		{
			if (m_elemContext.m_startTagOpen)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String patchedName = patchName(name);
				string patchedName = patchName(name);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = getLocalName(patchedName);
				string localName = getLocalName(patchedName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = getNamespaceURI(patchedName, false);
				string uri = getNamespaceURI(patchedName, false);

				addAttributeAlways(uri,localName, patchedName, "CDATA", value, false);
			}
		}

		/// <summary>
		/// Adds the given xsl:attribute to the set of collected attributes, 
		/// but only if there is a currently open element.
		/// </summary>
		/// <param name="name"> the attribute's qualified name (prefix:localName) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <param name="uri"> the URI that the prefix of the name points to </param>
		public virtual void addXSLAttribute(string name, in string value, in string uri)
		{
			if (m_elemContext.m_startTagOpen)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String patchedName = patchName(name);
				string patchedName = patchName(name);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localName = getLocalName(patchedName);
				string localName = getLocalName(patchedName);

				addAttributeAlways(uri,localName, patchedName, "CDATA", value, true);
			}
		}

		/// <summary>
		/// Add the given attributes to the currently collected ones. These
		/// attributes are always added, regardless of whether on not an element
		/// is currently open. </summary>
		/// <param name="atts"> List of attributes to add to this list </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addAttributes(org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public virtual void addAttributes(Attributes atts)
		{

			int nAtts = atts.getLength();

			for (int i = 0; i < nAtts; i++)
			{
				string uri = atts.getURI(i);

				if (null == uri)
				{
					uri = "";
				}

				addAttributeAlways(uri, atts.getLocalName(i), atts.getQName(i), atts.getType(i), atts.getValue(i), false);

			}
		}

		/// <summary>
		/// Return a <seealso cref="ContentHandler"/> interface into this serializer.
		/// If the serializer does not support the <seealso cref="ContentHandler"/>
		/// interface, it should return null.
		/// </summary>
		/// <returns> A <seealso cref="ContentHandler"/> interface into this serializer,
		///  or null if the serializer is not SAX 2 capable </returns>
		/// <exception cref="IOException"> An I/O exception occured </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.xml.sax.ContentHandler asContentHandler() throws java.io.IOException
		public virtual ContentHandler asContentHandler()
		{
			return this;
		}

		/// <summary>
		/// Report the end of an entity.
		/// </summary>
		/// <param name="name"> The name of the entity that is ending. </param>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
		/// <seealso cref=".startEntity"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endEntity(String name) throws org.xml.sax.SAXException
		public virtual void endEntity(string name)
		{
			if (name.Equals("[dtd]"))
			{
				m_inExternalDTD = false;
			}
			m_inEntityRef = false;

			if (m_tracer != null)
			{
				this.fireEndEntity(name);
			}
		}

		/// <summary>
		/// Flush and close the underlying java.io.Writer. This method applies to
		/// ToStream serializers, not ToSAXHandler serializers. </summary>
		/// <seealso cref="ToStream"/>
		public virtual void close()
		{
			// do nothing (base behavior)
		}

		/// <summary>
		/// Initialize global variables
		/// </summary>
		protected internal virtual void initCDATA()
		{
			// CDATA stack
			//        _cdataStack = new Stack();
			//        _cdataStack.push(new Integer(-1)); // push dummy value
		}

		/// <summary>
		/// Returns the character encoding to be used in the output document. </summary>
		/// <returns> the character encoding to be used in the output document. </returns>
		public virtual string Encoding
		{
			get
			{
				return getOutputProperty(OutputKeys.ENCODING);
			}
			set
			{
				setOutputProperty(OutputKeys.ENCODING,value);
			}
		}


		/// <summary>
		/// Sets the value coming from the xsl:output omit-xml-declaration stylesheet attribute </summary>
		/// <param name="b"> true if the XML declaration is to be omitted from the output
		/// document. </param>
		public virtual bool OmitXMLDeclaration
		{
			set
			{
				string val = value ? "yes":"no";
				setOutputProperty(OutputKeys.OMIT_XML_DECLARATION,val);
			}
			get
			{
				return m_shouldNotWriteXMLHeader;
			}
		}



		/// <summary>
		/// Returns the previously set value of the value to be used as the public
		/// identifier in the document type declaration (DTD).
		/// </summary>
		/// <returns> the public identifier to be used in the DOCTYPE declaration in the
		/// output document. </returns>
		public virtual string DoctypePublic
		{
			get
			{
				return m_doctypePublic;
			}
			set
			{
				setOutputProperty(OutputKeys.DOCTYPE_PUBLIC, value);
			}
		}



		/// <summary>
		/// Returns the previously set value of the value to be used
		/// as the system identifier in the document type declaration (DTD). </summary>
		/// <returns> the system identifier to be used in the DOCTYPE declaration in
		/// the output document.
		///  </returns>
		public virtual string DoctypeSystem
		{
			get
			{
				return m_doctypeSystem;
			}
			set
			{
				setOutputProperty(OutputKeys.DOCTYPE_SYSTEM, value);
			}
		}


		/// <summary>
		/// Set the value coming from the xsl:output doctype-public and doctype-system stylesheet properties </summary>
		/// <param name="doctypeSystem"> the system identifier to be used in the DOCTYPE
		/// declaration in the output document. </param>
		/// <param name="doctypePublic"> the public identifier to be used in the DOCTYPE
		/// declaration in the output document. </param>
		public virtual void setDoctype(string doctypeSystem, string doctypePublic)
		{
			setOutputProperty(OutputKeys.DOCTYPE_SYSTEM, doctypeSystem);
			setOutputProperty(OutputKeys.DOCTYPE_PUBLIC, doctypePublic);
		}

		/// <summary>
		/// Sets the value coming from the xsl:output standalone stylesheet attribute. </summary>
		/// <param name="standalone"> a value of "yes" indicates that the
		/// <code>standalone</code> delaration is to be included in the output
		/// document. This method remembers if the value was explicitly set using
		/// this method, verses if the value is the default value. </param>
		public virtual string Standalone
		{
			set
			{
				setOutputProperty(OutputKeys.STANDALONE, value);
			}
			get
			{
				return m_standalone;
			}
		}
		/// <summary>
		/// Sets the XSL standalone attribute, but does not remember if this is a
		/// default or explicite setting. </summary>
		/// <param name="standalone"> "yes" | "no" </param>
		protected internal virtual string StandaloneInternal
		{
			set
			{
				if ("yes".Equals(value))
				{
					m_standalone = "yes";
				}
				else
				{
					m_standalone = "no";
				}
    
			}
		}


		/// <returns> true if the output document should be indented to visually
		/// indicate its structure. </returns>
		public virtual bool Indent
		{
			get
			{
				return m_doIndent;
			}
			set
			{
				string val = value ? "yes":"no";
				setOutputProperty(OutputKeys.INDENT,val);
			}
		}
		/// <summary>
		/// Gets the mediatype the media-type or MIME type associated with the output
		/// document. </summary>
		/// <returns> the mediatype the media-type or MIME type associated with the
		/// output document. </returns>
		public virtual string MediaType
		{
			get
			{
				return m_mediatype;
			}
			set
			{
				setOutputProperty(OutputKeys.MEDIA_TYPE,value);
			}
		}

		/// <summary>
		/// Gets the version of the output format. </summary>
		/// <returns> the version of the output format. </returns>
		public virtual string Version
		{
			get
			{
				return m_version;
			}
			set
			{
				setOutputProperty(OutputKeys.VERSION, value);
			}
		}



		/// <returns> the number of spaces to indent for each indentation level. </returns>
		public virtual int IndentAmount
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
		/// This method is used when a prefix/uri namespace mapping
		/// is indicated after the element was started with a 
		/// startElement() and before and endElement().
		/// startPrefixMapping(prefix,uri) would be used before the
		/// startElement() call. </summary>
		/// <param name="uri"> the URI of the namespace </param>
		/// <param name="prefix"> the prefix associated with the given URI.
		/// </param>
		/// <seealso cref="ExtendedContentHandler.namespaceAfterStartElement(String, String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void namespaceAfterStartElement(String uri, String prefix) throws org.xml.sax.SAXException
		public virtual void namespaceAfterStartElement(string uri, string prefix)
		{
			// default behavior is to do nothing
		}

		/// <summary>
		/// Return a <seealso cref="DOMSerializer"/> interface into this serializer. If the
		/// serializer does not support the <seealso cref="DOMSerializer"/> interface, it should
		/// return null.
		/// </summary>
		/// <returns> A <seealso cref="DOMSerializer"/> interface into this serializer,  or null
		/// if the serializer is not DOM capable </returns>
		/// <exception cref="IOException"> An I/O exception occured </exception>
		/// <seealso cref="Serializer.asDOMSerializer()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public DOMSerializer asDOMSerializer() throws java.io.IOException
		public virtual DOMSerializer asDOMSerializer()
		{
			return this;
		}

		/// <summary>
		/// Tell if two strings are equal, without worry if the first string is null.
		/// </summary>
		/// <param name="p"> String reference, which may be null. </param>
		/// <param name="t"> String reference, which may be null.
		/// </param>
		/// <returns> true if strings are equal. </returns>
		private static bool subPartMatch(string p, string t)
		{
			return (string.ReferenceEquals(p, t)) || ((null != p) && (p.Equals(t)));
		}

		/// <summary>
		/// Returns the local name of a qualified name. 
		/// If the name has no prefix,
		/// then it works as the identity (SAX2). 
		/// </summary>
		/// <param name="qname"> a qualified name </param>
		/// <returns> returns the prefix of the qualified name,
		/// or null if there is no prefix. </returns>
		protected internal static string getPrefixPart(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = qname.indexOf(':');
			int col = qname.IndexOf(':');
			return (col > 0) ? qname.Substring(0, col) : null;
			//return (col > 0) ? qname.substring(0,col) : "";
		}

		/// <summary>
		/// Some users of the serializer may need the current namespace mappings </summary>
		/// <returns> the current namespace mappings (prefix/uri) </returns>
		/// <seealso cref="ExtendedContentHandler.getNamespaceMappings()"/>
		public virtual NamespaceMappings NamespaceMappings
		{
			get
			{
				return m_prefixMap;
			}
			set
			{
				m_prefixMap = value;
			}
		}

		/// <summary>
		/// Returns the prefix currently pointing to the given URI (if any). </summary>
		/// <param name="namespaceURI"> the uri of the namespace in question </param>
		/// <returns> a prefix pointing to the given URI (if any). </returns>
		/// <seealso cref="ExtendedContentHandler.getPrefix(String)"/>
		public virtual string getPrefix(string namespaceURI)
		{
			string prefix = m_prefixMap.lookupPrefix(namespaceURI);
			return prefix;
		}

		/// <summary>
		/// Returns the URI of an element or attribute. Note that default namespaces
		/// do not apply directly to attributes. </summary>
		/// <param name="qname"> a qualified name </param>
		/// <param name="isElement"> true if the qualified name is the name of 
		/// an element. </param>
		/// <returns> returns the namespace URI associated with the qualified name. </returns>
		public virtual string getNamespaceURI(string qname, bool isElement)
		{
			string uri = EMPTYSTRING;
			int col = qname.LastIndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = (col > 0) ? qname.substring(0, col) : EMPTYSTRING;
			string prefix = (col > 0) ? qname.Substring(0, col) : EMPTYSTRING;

			if (!EMPTYSTRING.Equals(prefix) || isElement)
			{
				if (m_prefixMap != null)
				{
					uri = m_prefixMap.lookupNamespace(prefix);
					if (string.ReferenceEquals(uri, null) && !prefix.Equals(XMLNS_PREFIX))
					{
						throw new Exception(Utils.messages.createMessage(MsgKey.ER_NAMESPACE_PREFIX, new object[] {qname.Substring(0, col)}));
					}
				}
			}
			return uri;
		}

		/// <summary>
		/// Returns the URI of prefix (if any)
		/// </summary>
		/// <param name="prefix"> the prefix whose URI is searched for </param>
		/// <returns> the namespace URI currently associated with the
		/// prefix, null if the prefix is undefined. </returns>
		public virtual string getNamespaceURIFromPrefix(string prefix)
		{
			string uri = null;
			if (m_prefixMap != null)
			{
				uri = m_prefixMap.lookupNamespace(prefix);
			}
			return uri;
		}

		/// <summary>
		/// Entity reference event.
		/// </summary>
		/// <param name="name"> Name of entity
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void entityReference(String name) throws org.xml.sax.SAXException
		public virtual void entityReference(string name)
		{

			flushPending();

			startEntity(name);
			endEntity(name);

			if (m_tracer != null)
			{
				fireEntityReference(name);
			}
		}

		/// <summary>
		/// Sets the transformer associated with this serializer </summary>
		/// <param name="t"> the transformer associated with this serializer. </param>
		/// <seealso cref="SerializationHandler.setTransformer(Transformer)"/>
		public virtual Transformer Transformer
		{
			set
			{
				m_transformer = value;
    
				// If this transformer object implements the SerializerTrace interface
				// then assign m_tracer to the transformer object so it can be used
				// to fire trace events.
				if ((m_transformer is SerializerTrace) && (((SerializerTrace) m_transformer).hasTraceListeners()))
				{
				   m_tracer = (SerializerTrace) m_transformer;
				}
				else
				{
				   m_tracer = null;
				}
			}
			get
			{
				return m_transformer;
			}
		}

		/// <summary>
		/// This method gets the nodes value as a String and uses that String as if
		/// it were an input character notification. </summary>
		/// <param name="node"> the Node to serialize </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		public virtual void characters(org.w3c.dom.Node node)
		{
			flushPending();
			string data = node.getNodeValue();
			if (!string.ReferenceEquals(data, null))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = data.length();
				int length = data.Length;
				if (length > m_charsBuff.Length)
				{
					m_charsBuff = new char[length * 2 + 1];
				}
				data.CopyTo(0, m_charsBuff, 0, length - 0);
				characters(m_charsBuff, 0, length);
			}
		}


		/// <seealso cref="org.xml.sax.ErrorHandler.error(SAXParseException)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException exc) throws org.xml.sax.SAXException
		public virtual void error(SAXParseException exc)
		{
		}

		/// <seealso cref="org.xml.sax.ErrorHandler.fatalError(SAXParseException)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException exc) throws org.xml.sax.SAXException
		public virtual void fatalError(SAXParseException exc)
		{

		  m_elemContext.m_startTagOpen = false;

		}

		/// <seealso cref="org.xml.sax.ErrorHandler.warning(SAXParseException)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException exc) throws org.xml.sax.SAXException
		public virtual void warning(SAXParseException exc)
		{
		}

		/// <summary>
		/// To fire off start entity trace event </summary>
		/// <param name="name"> Name of entity </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireStartEntity(String name) throws org.xml.sax.SAXException
		protected internal virtual void fireStartEntity(string name)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_ENTITYREF, name);
			}
		}

		/// <summary>
		/// Report the characters event </summary>
		/// <param name="chars">  content of characters </param>
		/// <param name="start">  starting index of characters to output </param>
		/// <param name="length">  number of characters to output </param>
	//    protected void fireCharEvent(char[] chars, int start, int length)
	//        throws org.xml.sax.SAXException
	//    {
	//        if (m_tracer != null)
	//            m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_CHARACTERS, chars, start,length);     	        	    	
	//    }
	//        

		/// <summary>
		/// This method is only used internally when flushing the writer from the
		/// various fire...() trace events.  Due to the writer being wrapped with 
		/// SerializerTraceWriter it may cause the flush of these trace events:
		/// EVENTTYPE_OUTPUT_PSEUDO_CHARACTERS 
		/// EVENTTYPE_OUTPUT_CHARACTERS
		/// which trace the output written to the output stream.
		/// 
		/// </summary>
		private void flushMyWriter()
		{
			if (m_writer != null)
			{
				try
				{
					m_writer.flush();
				}
				catch (IOException)
				{

				}
			}
		}
		/// <summary>
		/// Report the CDATA trace event </summary>
		/// <param name="chars">  content of CDATA </param>
		/// <param name="start">  starting index of characters to output </param>
		/// <param name="length">  number of characters to output </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireCDATAEvent(char[] chars, int start, int length) throws org.xml.sax.SAXException
		protected internal virtual void fireCDATAEvent(char[] chars, int start, int length)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_CDATA, chars, start,length);
			}
		}

		/// <summary>
		/// Report the comment trace event </summary>
		/// <param name="chars">  content of comment </param>
		/// <param name="start">  starting index of comment to output </param>
		/// <param name="length">  number of characters to output </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireCommentEvent(char[] chars, int start, int length) throws org.xml.sax.SAXException
		protected internal virtual void fireCommentEvent(char[] chars, int start, int length)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_COMMENT, new string(chars, start, length));
			}
		}


		/// <summary>
		/// To fire off end entity trace event </summary>
		/// <param name="name"> Name of entity </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void fireEndEntity(String name) throws org.xml.sax.SAXException
		public virtual void fireEndEntity(string name)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
			}
			// we do not need to handle this.
		}

		/// <summary>
		/// To fire off start document trace  event
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireStartDoc() throws org.xml.sax.SAXException
		 protected internal virtual void fireStartDoc()
		 {
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_STARTDOCUMENT);
			}
		 }


		/// <summary>
		/// To fire off end document trace event
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireEndDoc() throws org.xml.sax.SAXException
		protected internal virtual void fireEndDoc()
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_ENDDOCUMENT);
			}
		}

		/// <summary>
		/// Report the start element trace event. This trace method needs to be
		/// called just before the attributes are cleared.
		/// </summary>
		/// <param name="elemName"> the qualified name of the element
		///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireStartElem(String elemName) throws org.xml.sax.SAXException
		protected internal virtual void fireStartElem(string elemName)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_STARTELEMENT, elemName, m_attributes);
			}
		}


		/// <summary>
		/// To fire off the end element event </summary>
		/// <param name="name"> Name of element </param>
	//    protected void fireEndElem(String name)
	//        throws org.xml.sax.SAXException
	//    {
	//        if (m_tracer != null)
	//            m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_ENDELEMENT,name, (Attributes)null);     	        	    	
	//    }    


		/// <summary>
		/// To fire off the PI trace event </summary>
		/// <param name="name"> Name of PI </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireEscapingEvent(String name, String data) throws org.xml.sax.SAXException
		protected internal virtual void fireEscapingEvent(string name, string data)
		{

			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_PI,name, data);
			}
		}


		/// <summary>
		/// To fire off the entity reference trace event </summary>
		/// <param name="name"> Name of entity reference </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void fireEntityReference(String name) throws org.xml.sax.SAXException
		protected internal virtual void fireEntityReference(string name)
		{
			if (m_tracer != null)
			{
				flushMyWriter();
				m_tracer.fireGenerateEvent(SerializerTrace.EVENTTYPE_ENTITYREF,name, (Attributes)null);
			}
		}

		/// <summary>
		/// Receive notification of the beginning of a document.
		/// This method is never a self generated call, 
		/// but only called externally.
		/// 
		/// <para>The SAX parser will invoke this method only once, before any
		/// other methods in this interface or in DTDHandler (except for
		/// setDocumentLocator).</para>
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public virtual void startDocument()
		{

			// if we do get called with startDocument(), handle it right away       
			startDocumentInternal();
			m_needToCallStartDocument = false;
			return;
		}

		/// <summary>
		/// This method handles what needs to be done at a startDocument() call,
		/// whether from an external caller, or internally called in the 
		/// serializer.  For historical reasons the serializer is flexible to
		/// startDocument() not always being called.
		/// Even if no external call is
		/// made into startDocument() this method will always be called as a self
		/// generated internal startDocument, it handles what needs to be done at a
		/// startDocument() call.
		/// 
		/// This method exists just to make sure that startDocument() is only ever
		/// called from an external caller, which in principle is just a matter of
		/// style.
		/// </summary>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void startDocumentInternal() throws org.xml.sax.SAXException
		protected internal virtual void startDocumentInternal()
		{
			if (m_tracer != null)
			{
				this.fireStartDoc();
			}
		}
		/// <summary>
		/// This method is used to set the source locator, which might be used to
		/// generated an error message. </summary>
		/// <param name="locator"> the source locator
		/// </param>
		/// <seealso cref="ExtendedContentHandler.setSourceLocator(javax.xml.transform.SourceLocator)"/>
		public virtual SourceLocator SourceLocator
		{
			set
			{
				m_sourceLocator = value;
			}
		}



		public virtual bool reset()
		{
			resetSerializerBase();
			return true;
		}

		/// <summary>
		/// Reset all of the fields owned by SerializerBase
		/// 
		/// </summary>
		private void resetSerializerBase()
		{
			this.m_attributes.clear();
			this.m_CdataElems = null;
			this.m_cdataTagOpen = false;
			this.m_docIsEmpty = true;
			this.m_doctypePublic = null;
			this.m_doctypeSystem = null;
			this.m_doIndent = false;
			this.m_elemContext = new ElemContext();
			this.m_indentAmount = 0;
			this.m_inEntityRef = false;
			this.m_inExternalDTD = false;
			this.m_mediatype = null;
			this.m_needToCallStartDocument = true;
			this.m_needToOutputDocTypeDecl = false;
			if (m_OutputProps != null)
			{
				this.m_OutputProps.Clear();
			}
			if (m_OutputPropsDefault != null)
			{
				this.m_OutputPropsDefault.Clear();
			}
			if (this.m_prefixMap != null)
			{
				this.m_prefixMap.reset();
			}
			this.m_shouldNotWriteXMLHeader = false;
			this.m_sourceLocator = null;
			this.m_standalone = null;
			this.m_standaloneWasSpecified = false;
			this.m_StringOfCDATASections = null;
			this.m_tracer = null;
			this.m_transformer = null;
			this.m_version = null;
			// don't set writer to null, so that it might be re-used
			//this.m_writer = null;
		}

		/// <summary>
		/// Returns true if the serializer is used for temporary output rather than
		/// final output.
		/// 
		/// This concept is made clear in the XSLT 2.0 draft.
		/// </summary>
		internal bool inTemporaryOutputState()
		{
			/* This is a hack. We should really be letting the serializer know
			 * that it is in temporary output state with an explicit call, but
			 * from a pragmatic point of view (for now anyways) having no output
			 * encoding at all, not even the default UTF-8 indicates that the serializer
			 * is being used for temporary RTF.
			 */ 
			return (string.ReferenceEquals(Encoding, null));

		}

		/// <summary>
		/// This method adds an attribute the the current element,
		/// but should not be used for an xsl:attribute child. </summary>
		/// <seealso cref="ExtendedContentHandler.addAttribute(java.lang.String, java.lang.String, java.lang.String, java.lang.String, java.lang.String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value) throws org.xml.sax.SAXException
		public virtual void addAttribute(string uri, string localName, string rawName, string type, string value)
		{
			if (m_elemContext.m_startTagOpen)
			{
				addAttributeAlways(uri, localName, rawName, type, value, false);
			}
		}

		/// <seealso cref="org.xml.sax.DTDHandler.notationDecl(java.lang.String, java.lang.String, java.lang.String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void notationDecl(String arg0, String arg1, String arg2) throws org.xml.sax.SAXException
		public virtual void notationDecl(string arg0, string arg1, string arg2)
		{
			// This method just provides a definition to satisfy the interface
			// A particular sub-class of SerializerBase provides the implementation (if desired)        
		}

		/// <seealso cref="org.xml.sax.DTDHandler.unparsedEntityDecl(java.lang.String, java.lang.String, java.lang.String, java.lang.String)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void unparsedEntityDecl(String arg0, String arg1, String arg2, String arg3) throws org.xml.sax.SAXException
		public virtual void unparsedEntityDecl(string arg0, string arg1, string arg2, string arg3)
		{
			// This method just provides a definition to satisfy the interface
			// A particular sub-class of SerializerBase provides the implementation (if desired)        
		}

		/// <summary>
		/// If set to false the serializer does not expand DTD entities,
		/// but leaves them as is, the default value is true.
		/// </summary>
		public virtual bool DTDEntityExpansion
		{
			set
			{
				// This method just provides a definition to satisfy the interface
				// A particular sub-class of SerializerBase provides the implementation (if desired)        
			}
		}


		/// <summary>
		/// The CDATA section names stored in a whitespace separateed list with
		/// each element being a word of the form "{uri}localName" This list
		/// comes from the cdata-section-elements attribute.
		/// 
		/// This field replaces m_cdataSectionElements Vector.
		/// </summary>
		protected internal string m_StringOfCDATASections = null;

		internal bool m_docIsEmpty = true;
		internal virtual void initCdataElems(string s)
		{
			if (!string.ReferenceEquals(s, null))
			{
				int max = s.Length;

				// true if we are in the middle of a pair of curly braces that delimit a URI
				bool inCurly = false;

				// true if we found a URI but haven't yet processed the local name 
				bool foundURI = false;

				StringBuilder buf = new StringBuilder();
				string uri = null;
				string localName = null;

				// parse through string, breaking on whitespaces.  I do this instead
				// of a tokenizer so I can track whitespace inside of curly brackets,
				// which theoretically shouldn't happen if they contain legal URLs.


				for (int i = 0; i < max; i++)
				{

					char c = s[i];

					if (char.IsWhiteSpace(c))
					{
						if (!inCurly)
						{
							if (buf.Length > 0)
							{
								localName = buf.ToString();
								if (!foundURI)
								{
									uri = "";
								}
								addCDATAElement(uri,localName);
								buf.Length = 0;
								foundURI = false;
							}
							continue;
						}
						else
						{
							buf.Append(c); // add whitespace to the URI
						}
					}
					else if ('{' == c) // starting a URI
					{
						inCurly = true;
					}
					else if ('}' == c)
					{
						// we just ended a URI, add the URI to the vector
						foundURI = true;
						uri = buf.ToString();
						buf.Length = 0;
						inCurly = false;
					}
					else
					{
						// append non-whitespace, non-curly to current URI or localName being gathered.                    
						buf.Append(c);
					}

				}

				if (buf.Length > 0)
				{
					// We have one last localName to process.
					localName = buf.ToString();
					if (!foundURI)
					{
						uri = "";
					}
					addCDATAElement(uri,localName);
				}
			}
		}
		protected internal Hashtable m_CdataElems = null;
		private void addCDATAElement(string uri, string localName)
		{
			if (m_CdataElems == null)
			{
				m_CdataElems = new Hashtable();
			}

			Hashtable h = (Hashtable) m_CdataElems[localName];
			if (h == null)
			{
				h = new Hashtable();
				m_CdataElems[localName] = h;
			}
			h[uri] = uri;

		}


		/// <summary>
		/// Return true if nothing has been sent to this result tree yet.
		/// <para>
		/// This is not a public API.
		/// 
		/// @xsl.usage internal
		/// </para>
		/// </summary>
		public virtual bool documentIsEmpty()
		{
			// If we haven't called startDocument() yet, then this document is empty
			return m_docIsEmpty && (m_elemContext.m_currentElemDepth == 0);
		}

		/// <summary>
		/// Return true if the current element in m_elemContext
		/// is a CDATA section.
		/// CDATA sections are specified in the <xsl:output> attribute
		/// cdata-section-names or in the JAXP equivalent property.
		/// In any case the format of the value of such a property is:
		/// <pre>
		/// "{uri1}localName1 {uri2}localName2 . . . "
		/// </pre>
		/// 
		/// <para>
		/// This method is not a public API, but is only used internally by the serializer.
		/// </para>
		/// </summary>
		protected internal virtual bool CdataSection
		{
			get
			{
    
				bool b = false;
    
				if (null != m_StringOfCDATASections)
				{
					if (string.ReferenceEquals(m_elemContext.m_elementLocalName, null))
					{
						string localName = getLocalName(m_elemContext.m_elementName);
						m_elemContext.m_elementLocalName = localName;
					}
    
					if (string.ReferenceEquals(m_elemContext.m_elementURI, null))
					{
    
						m_elemContext.m_elementURI = ElementURI;
					}
					else if (m_elemContext.m_elementURI.Length == 0)
					{
						if (string.ReferenceEquals(m_elemContext.m_elementName, null))
						{
							m_elemContext.m_elementName = m_elemContext.m_elementLocalName;
							// leave URI as "", meaning in no namespace
						}
						else if (m_elemContext.m_elementLocalName.Length < m_elemContext.m_elementName.Length)
						{
							// We were told the URI was "", yet the name has a prefix since the name is longer than the localname.
							// So we will fix that incorrect information here.
							m_elemContext.m_elementURI = ElementURI;
						}
					}
    
					Hashtable h = (Hashtable) m_CdataElems[m_elemContext.m_elementLocalName];
					if (h != null)
					{
						object obj = h[m_elemContext.m_elementURI];
						if (obj != null)
						{
							b = true;
						}
					}
    
				}
				return b;
			}
		}

		/// <summary>
		/// Before this call m_elementContext.m_elementURI is null,
		/// which means it is not yet known. After this call it
		/// is non-null, but possibly "" meaning that it is in the
		/// default namespace.
		/// </summary>
		/// <returns> The URI of the element, never null, but possibly "". </returns>
		private string ElementURI
		{
			get
			{
				string uri = null;
				// At this point in processing we have received all the
				// namespace mappings
				// As we still don't know the elements namespace,
				// we now figure it out.
    
				string prefix = getPrefixPart(m_elemContext.m_elementName);
    
				if (string.ReferenceEquals(prefix, null))
				{
					// no prefix so lookup the URI of the default namespace
					uri = m_prefixMap.lookupNamespace("");
				}
				else
				{
					uri = m_prefixMap.lookupNamespace(prefix);
				}
				if (string.ReferenceEquals(uri, null))
				{
					// We didn't find the namespace for the
					// prefix ... ouch, that shouldn't happen.
					// This is a hack, we really don't know
					// the namespace
					uri = EMPTYSTRING;
				}
    
				return uri;
			}
		}


		/// <summary>
		/// Get the value of an output property,
		/// the explicit value, if any, otherwise the
		/// default value, if any, otherwise null.
		/// </summary>
		public virtual string getOutputProperty(string name)
		{
			string val = getOutputPropertyNonDefault(name);
			// If no explicit value, try to get the default value
			if (string.ReferenceEquals(val, null))
			{
				val = getOutputPropertyDefault(name);
			}
			return val;

		}
		/// <summary>
		/// Get the value of an output property, 
		/// not the default value. If there is a default
		/// value, but no non-default value this method
		/// will return null.
		/// <para>
		/// 
		/// </para>
		/// </summary>
		public virtual string getOutputPropertyNonDefault(string name)
		{
			return getProp(name,false);
		}

		/// <summary>
		/// Return a <seealso cref="DOM3Serializer"/> interface into this serializer. If the
		/// serializer does not support the <seealso cref="DOM3Serializer"/> interface, it should
		/// return null.
		/// </summary>
		/// <returns> A <seealso cref="DOM3Serializer"/> interface into this serializer,  or null
		/// if the serializer is not DOM capable </returns>
		/// <exception cref="IOException"> An I/O exception occured </exception>
		/// <seealso cref="org.apache.xml.serializer.Serializer.asDOM3Serializer()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object asDOM3Serializer() throws java.io.IOException
		public virtual object asDOM3Serializer()
		{
			return new org.apache.xml.serializer.dom3.DOM3SerializerImpl(this);
		}
		/// <summary>
		/// Get the default value of an xsl:output property,
		/// which would be null only if no default value exists
		/// for the property.
		/// </summary>
		public virtual string getOutputPropertyDefault(string name)
		{
			return getProp(name, true);
		}

		/// <summary>
		/// Set the value for the output property, typically from
		/// an xsl:output element, but this does not change what
		/// the default value is.
		/// </summary>
		public virtual void setOutputProperty(string name, string val)
		{
			setProp(name,val,false);

		}

		/// <summary>
		/// Set the default value for an output property, but this does
		/// not impact any explicitly set value.
		/// </summary>
		public virtual void setOutputPropertyDefault(string name, string val)
		{
			setProp(name,val,true);

		}

		/// <summary>
		/// A mapping of keys to explicitly set values, for example if 
		/// and <xsl:output/> has an "encoding" attribute, this
		/// map will have what that attribute maps to.
		/// </summary>
		private Hashtable m_OutputProps;
		/// <summary>
		/// A mapping of keys to default values, for example if
		/// the default value of the encoding is "UTF-8" then this
		/// map will have that "encoding" maps to "UTF-8".
		/// </summary>
		private Hashtable m_OutputPropsDefault;

		internal virtual ISet<object> OutputPropDefaultKeys
		{
			get
			{
				return m_OutputPropsDefault.Keys;
			}
		}
		internal virtual ISet<object> OutputPropKeys
		{
			get
			{
				return m_OutputProps.Keys;
			}
		}

		private string getProp(string name, bool defaultVal)
		{
			if (m_OutputProps == null)
			{
				m_OutputProps = new Hashtable();
				m_OutputPropsDefault = new Hashtable();
			}

			string val;
			if (defaultVal)
			{
				val = (string) m_OutputPropsDefault[name];
			}
			else
			{
				val = (string) m_OutputProps[name];
			}

			return val;

		}
		/// 
		/// <param name="name"> The name of the property, e.g. "{http://myprop}indent-tabs" or "indent". </param>
		/// <param name="val"> The value of the property, e.g. "4" </param>
		/// <param name="defaultVal"> true if this is a default value being set for the property as 
		/// opposed to a user define on, set say explicitly in the stylesheet or via JAXP </param>
		internal virtual void setProp(string name, string val, bool defaultVal)
		{
			if (m_OutputProps == null)
			{
				m_OutputProps = new Hashtable();
				m_OutputPropsDefault = new Hashtable();
			}

			if (defaultVal)
			{
				m_OutputPropsDefault[name] = val;
			}
			else
			{
				if (OutputKeys.CDATA_SECTION_ELEMENTS.Equals(name) && !string.ReferenceEquals(val, null))
				{
					initCdataElems(val);
					string oldVal = (string) m_OutputProps[name];
					string newVal;
					if (string.ReferenceEquals(oldVal, null))
					{
						newVal = oldVal + ' ' + val;
					}
					else
					{
						newVal = val;
					}
					m_OutputProps[name] = newVal;
				}
				else
				{
					m_OutputProps[name] = val;
				}
			}


		}

		/// <summary>
		/// Get the first char of the local name </summary>
		/// <param name="name"> Either a local name, or a local name
		/// preceeded by a uri enclosed in curly braces. </param>
		internal static char getFirstCharLocName(string name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char first;
			char first;
			int i = name.IndexOf('}');
			if (i < 0)
			{
				first = name[0];
			}
			else
			{
				first = name[i + 1];
			}
			return first;
		}
	}



}