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
 * $Id: DOMBuilder.java 1225373 2011-12-28 22:59:38Z mrglavas $
 */
namespace org.apache.xml.utils
{

	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using Document = org.w3c.dom.Document;
	using DocumentFragment = org.w3c.dom.DocumentFragment;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using Text = org.w3c.dom.Text;
	using CDATASection = org.w3c.dom.CDATASection;

	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;
	/// <summary>
	/// This class takes SAX events (in addition to some extra events
	/// that SAX doesn't handle yet) and adds the result to a document
	/// or document fragment.
	/// @xsl.usage general
	/// </summary>
	public class DOMBuilder : ContentHandler, LexicalHandler
	{

	  /// <summary>
	  /// Root document </summary>
	  public Document m_doc;

	  /// <summary>
	  /// Current node </summary>
	  protected internal Node m_currentNode = null;

	  /// <summary>
	  /// The root node </summary>
	  protected internal Node m_root = null;

	  /// <summary>
	  /// The next sibling node </summary>
	  protected internal Node m_nextSibling = null;

	  /// <summary>
	  /// First node of document fragment or null if not a DocumentFragment </summary>
	  public DocumentFragment m_docFrag = null;

	  /// <summary>
	  /// Vector of element nodes </summary>
	  protected internal System.Collections.Stack m_elemStack = new System.Collections.Stack();

	  /// <summary>
	  /// Namespace support </summary>
	  protected internal ArrayList m_prefixMappings = new ArrayList();

	  /// <summary>
	  /// DOMBuilder instance constructor... it will add the DOM nodes
	  /// to the document fragment.
	  /// </summary>
	  /// <param name="doc"> Root document </param>
	  /// <param name="node"> Current node </param>
	  public DOMBuilder(Document doc, Node node)
	  {
		m_doc = doc;
		m_currentNode = m_root = node;

		if (node is Element)
		{
		  m_elemStack.Push(node);
		}
	  }

	  /// <summary>
	  /// DOMBuilder instance constructor... it will add the DOM nodes
	  /// to the document fragment.
	  /// </summary>
	  /// <param name="doc"> Root document </param>
	  /// <param name="docFrag"> Document fragment </param>
	  public DOMBuilder(Document doc, DocumentFragment docFrag)
	  {
		m_doc = doc;
		m_docFrag = docFrag;
	  }

	  /// <summary>
	  /// DOMBuilder instance constructor... it will add the DOM nodes
	  /// to the document.
	  /// </summary>
	  /// <param name="doc"> Root document </param>
	  public DOMBuilder(Document doc)
	  {
		m_doc = doc;
	  }

	  /// <summary>
	  /// Get the root document or DocumentFragment of the DOM being created.
	  /// </summary>
	  /// <returns> The root document or document fragment if not null </returns>
	  public virtual Node RootDocument
	  {
		  get
		  {
			return (null != m_docFrag) ? (Node) m_docFrag : (Node) m_doc;
		  }
	  }

	  /// <summary>
	  /// Get the root node of the DOM tree.
	  /// </summary>
	  public virtual Node RootNode
	  {
		  get
		  {
			return m_root;
		  }
	  }

	  /// <summary>
	  /// Get the node currently being processed.
	  /// </summary>
	  /// <returns> the current node being processed </returns>
	  public virtual Node CurrentNode
	  {
		  get
		  {
			return m_currentNode;
		  }
	  }

	  /// <summary>
	  /// Set the next sibling node, which is where the result nodes 
	  /// should be inserted before.
	  /// </summary>
	  /// <param name="nextSibling"> the next sibling node. </param>
	  public virtual Node NextSibling
	  {
		  set
		  {
			m_nextSibling = value;
		  }
		  get
		  {
			return m_nextSibling;
		  }
	  }


	  /// <summary>
	  /// Return null since there is no Writer for this class.
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual java.io.Writer Writer
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// <summary>
	  /// Append a node to the current container.
	  /// </summary>
	  /// <param name="newNode"> New node to append </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void append(org.w3c.dom.Node newNode) throws org.xml.sax.SAXException
	  protected internal virtual void append(Node newNode)
	  {

		Node currentNode = m_currentNode;

		if (null != currentNode)
		{
		  if (currentNode == m_root && m_nextSibling != null)
		  {
			currentNode.insertBefore(newNode, m_nextSibling);
		  }
		  else
		  {
			currentNode.appendChild(newNode);
		  }

		  // System.out.println(newNode.getNodeName());
		}
		else if (null != m_docFrag)
		{
		  if (m_nextSibling != null)
		  {
			m_docFrag.insertBefore(newNode, m_nextSibling);
		  }
		  else
		  {
			m_docFrag.appendChild(newNode);
		  }
		}
		else
		{
		  bool ok = true;
		  short type = newNode.getNodeType();

		  if (type == Node.TEXT_NODE)
		  {
			string data = newNode.getNodeValue();

			if ((null != data) && (data.Trim().Length > 0))
			{
			  throw new org.xml.sax.SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_CANT_OUTPUT_TEXT_BEFORE_DOC, null)); //"Warning: can't output text before document element!  Ignoring...");
			}

			ok = false;
		  }
		  else if (type == Node.ELEMENT_NODE)
		  {
			if (m_doc.getDocumentElement() != null)
			{
			  ok = false;

			  throw new org.xml.sax.SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_CANT_HAVE_MORE_THAN_ONE_ROOT, null)); //"Can't have more than one root on a DOM!");
			}
		  }

		  if (ok)
		  {
			if (m_nextSibling != null)
			{
			  m_doc.insertBefore(newNode, m_nextSibling);
			}
			else
			{
			  m_doc.appendChild(newNode);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Receive an object for locating the origin of SAX document events.
	  /// 
	  /// <para>SAX parsers are strongly encouraged (though not absolutely
	  /// required) to supply a locator: if it does so, it must supply
	  /// the locator to the application by invoking this method before
	  /// invoking any of the other methods in the ContentHandler
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
	  /// application should not attempt to use it at any other time.</para>
	  /// </summary>
	  /// <param name="locator"> An object that can return the location of
	  ///                any SAX document event. </param>
	  /// <seealso cref="org.xml.sax.Locator"/>
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
    
			// No action for the moment.
		  }
	  }

	  /// <summary>
	  /// Receive notification of the beginning of a document.
	  /// 
	  /// <para>The SAX parser will invoke this method only once, before any
	  /// other methods in this interface or in DTDHandler (except for
	  /// setDocumentLocator).</para>
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
	  public virtual void startDocument()
	  {

		// No action for the moment.
	  }

	  /// <summary>
	  /// Receive notification of the end of a document.
	  /// 
	  /// <para>The SAX parser will invoke this method only once, and it will
	  /// be the last method invoked during the parse.  The parser shall
	  /// not invoke this method until it has either abandoned parsing
	  /// (because of an unrecoverable error) or reached the end of
	  /// input.</para>
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
	  public virtual void endDocument()
	  {

		// No action for the moment.
	  }

	  /// <summary>
	  /// Receive notification of the beginning of an element.
	  /// 
	  /// <para>The Parser will invoke this method at the beginning of every
	  /// element in the XML document; there will be a corresponding
	  /// endElement() event for every startElement() event (even when the
	  /// element is empty). All of the element's content will be
	  /// reported, in order, before the corresponding endElement()
	  /// event.</para>
	  /// 
	  /// <para>If the element name has a namespace prefix, the prefix will
	  /// still be attached.  Note that the attribute list provided will
	  /// contain only attributes with explicit values (specified or
	  /// defaulted): #IMPLIED attributes will be omitted.</para>
	  /// 
	  /// </summary>
	  /// <param name="ns"> The namespace of the node </param>
	  /// <param name="localName"> The local part of the qualified name </param>
	  /// <param name="name"> The element name. </param>
	  /// <param name="atts"> The attributes attached to the element, if any. </param>
	  /// <seealso cref=".endElement"/>
	  /// <seealso cref="org.xml.sax.Attributes"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(String ns, String localName, String name, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
	  public virtual void startElement(string ns, string localName, string name, Attributes atts)
	  {

		Element elem;

		// Note that the namespace-aware call must be used to correctly
		// construct a Level 2 DOM, even for non-namespaced nodes.
		if ((null == ns) || (ns.Length == 0))
		{
		  elem = m_doc.createElementNS(null,name);
		}
		else
		{
		  elem = m_doc.createElementNS(ns, name);
		}

		append(elem);

		try
		{
		  int nAtts = atts.getLength();

		  if (0 != nAtts)
		  {
			for (int i = 0; i < nAtts; i++)
			{

			  //System.out.println("type " + atts.getType(i) + " name " + atts.getLocalName(i) );
			  // First handle a possible ID attribute
			  if (atts.getType(i).equalsIgnoreCase("ID"))
			  {
				setIDAttribute(atts.getValue(i), elem);
			  }

			  string attrNS = atts.getURI(i);

			  if ("".Equals(attrNS))
			  {
				attrNS = null; // DOM represents no-namespace as null
			  }

			  // System.out.println("attrNS: "+attrNS+", localName: "+atts.getQName(i)
			  //                   +", qname: "+atts.getQName(i)+", value: "+atts.getValue(i));
			  // Crimson won't let us set an xmlns: attribute on the DOM.
			  string attrQName = atts.getQName(i);

			  // In SAX, xmlns[:] attributes have an empty namespace, while in DOM they 
			  // should have the xmlns namespace
			  if (attrQName.StartsWith("xmlns:", StringComparison.Ordinal) || attrQName.Equals("xmlns"))
			  {
				attrNS = "http://www.w3.org/2000/xmlns/";
			  }

			  // ALWAYS use the DOM Level 2 call!
			  elem.setAttributeNS(attrNS,attrQName, atts.getValue(i));
			}
		  }

		  /*
		   * Adding namespace nodes to the DOM tree;
		   */
		  int nDecls = m_prefixMappings.Count;

		  string prefix, declURL;

		  for (int i = 0; i < nDecls; i += 2)
		  {
			prefix = (string) m_prefixMappings[i];

			if (string.ReferenceEquals(prefix, null))
			{
			  continue;
			}

			declURL = (string) m_prefixMappings[i + 1];

			elem.setAttributeNS("http://www.w3.org/2000/xmlns/", prefix, declURL);
		  }

		  m_prefixMappings.Clear();

		  // append(elem);

		  m_elemStack.Push(elem);

		  m_currentNode = elem;

		  // append(elem);
		}
		catch (Exception de)
		{
		  // de.printStackTrace();
		  throw new org.xml.sax.SAXException(de);
		}

	  }

	  /// 
	  /// 
	  /// 
	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// 
	  /// <para>The SAX parser will invoke this method at the end of every
	  /// element in the XML document; there will be a corresponding
	  /// startElement() event for every endElement() event (even when the
	  /// element is empty).</para>
	  /// 
	  /// <para>If the element name has a namespace prefix, the prefix will
	  /// still be attached to the name.</para>
	  /// 
	  /// </summary>
	  /// <param name="ns"> the namespace of the element </param>
	  /// <param name="localName"> The local part of the qualified name of the element </param>
	  /// <param name="name"> The element name </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endElement(String ns, String localName, String name) throws org.xml.sax.SAXException
	  public virtual void endElement(string ns, string localName, string name)
	  {
		m_elemStack.Pop();
		m_currentNode = m_elemStack.Count == 0 ? null : (Node)m_elemStack.Peek();
	  }

	  /// <summary>
	  /// Set an ID string to node association in the ID table.
	  /// </summary>
	  /// <param name="id"> The ID string. </param>
	  /// <param name="elem"> The associated ID. </param>
	  public virtual void setIDAttribute(string id, Element elem)
	  {

		// Do nothing. This method is meant to be overiden.
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
	  /// <param name="ch"> The characters from the XML document. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <seealso cref=".ignorableWhitespace"/>
	  /// <seealso cref="org.xml.sax.Locator"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {
		if (OutsideDocElem && org.apache.xml.utils.XMLCharacterRecognizer.isWhiteSpace(ch, start, length))
		{
		  return; // avoid DOM006 Hierarchy request error
		}

		if (m_inCData)
		{
		  cdata(ch, start, length);

		  return;
		}

		string s = new string(ch, start, length);
		Node childNode;
		childNode = m_currentNode != null ? m_currentNode.getLastChild(): null;
		if (childNode != null && childNode.getNodeType() == Node.TEXT_NODE)
		{
		   ((Text)childNode).appendData(s);
		}
		else
		{
		   Text text = m_doc.createTextNode(s);
		   append(text);
		}
	  }

	  /// <summary>
	  /// If available, when the disable-output-escaping attribute is used,
	  /// output raw text without escaping.  A PI will be inserted in front
	  /// of the node with the name "lotusxsl-next-is-raw" and a value of
	  /// "formatter-to-dom".
	  /// </summary>
	  /// <param name="ch"> Array containing the characters </param>
	  /// <param name="start"> Index to start of characters in the array </param>
	  /// <param name="length"> Number of characters in the array </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void charactersRaw(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void charactersRaw(char[] ch, int start, int length)
	  {
		if (OutsideDocElem && org.apache.xml.utils.XMLCharacterRecognizer.isWhiteSpace(ch, start, length))
		{
		  return; // avoid DOM006 Hierarchy request error
		}


		string s = new string(ch, start, length);

		append(m_doc.createProcessingInstruction("xslt-next-is-raw", "formatter-to-dom"));
		append(m_doc.createTextNode(s));
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
	  /// <seealso cref=".endEntity"/>
	  /// <seealso cref="org.xml.sax.ext.DeclHandler.internalEntityDecl"/>
	  /// <seealso cref="org.xml.sax.ext.DeclHandler.externalEntityDecl"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startEntity(String name) throws org.xml.sax.SAXException
	  public virtual void startEntity(string name)
	  {

		// Almost certainly the wrong behavior...
		// entityReference(name);
	  }

	  /// <summary>
	  /// Report the end of an entity.
	  /// </summary>
	  /// <param name="name"> The name of the entity that is ending. </param>
	  /// <seealso cref=".startEntity"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endEntity(String name) throws org.xml.sax.SAXException
	  public virtual void endEntity(string name)
	  {
	  }

	  /// <summary>
	  /// Receive notivication of a entityReference.
	  /// </summary>
	  /// <param name="name"> name of the entity reference </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void entityReference(String name) throws org.xml.sax.SAXException
	  public virtual void entityReference(string name)
	  {
		append(m_doc.createEntityReference(name));
	  }

	  /// <summary>
	  /// Receive notification of ignorable whitespace in element content.
	  /// 
	  /// <para>Validating Parsers must use this method to report each chunk
	  /// of ignorable whitespace (see the W3C XML 1.0 recommendation,
	  /// section 2.10): non-validating parsers may also use this method
	  /// if they are capable of parsing and using content models.</para>
	  /// 
	  /// <para>SAX parsers may return all contiguous whitespace in a single
	  /// chunk, or they may split it into several chunks; however, all of
	  /// the characters in any single event must come from the same
	  /// external entity, so that the Locator provides useful
	  /// information.</para>
	  /// 
	  /// <para>The application must not attempt to read from the array
	  /// outside of the specified range.</para>
	  /// </summary>
	  /// <param name="ch"> The characters from the XML document. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  /// <seealso cref=".characters"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void ignorableWhitespace(char[] ch, int start, int length)
	  {
		if (OutsideDocElem)
		{
		  return; // avoid DOM006 Hierarchy request error
		}

		string s = new string(ch, start, length);

		append(m_doc.createTextNode(s));
	  }

	  /// <summary>
	  /// Tell if the current node is outside the document element.
	  /// </summary>
	  /// <returns> true if the current node is outside the document element. </returns>
	   private bool OutsideDocElem
	   {
		   get
		   {
			  return (null == m_docFrag) && m_elemStack.Count == 0 && (null == m_currentNode || m_currentNode.getNodeType() == Node.DOCUMENT_NODE);
		   }
	   }

	  /// <summary>
	  /// Receive notification of a processing instruction.
	  /// 
	  /// <para>The Parser will invoke this method once for each processing
	  /// instruction found: note that processing instructions may occur
	  /// before or after the main document element.</para>
	  /// 
	  /// <para>A SAX parser should never report an XML declaration (XML 1.0,
	  /// section 2.8) or a text declaration (XML 1.0, section 4.3.1)
	  /// using this method.</para>
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///        none was supplied. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {
		append(m_doc.createProcessingInstruction(target, data));
	  }

	  /// <summary>
	  /// Report an XML comment anywhere in the document.
	  /// 
	  /// This callback will be used for comments inside or outside the
	  /// document element, including comments in the external DTD
	  /// subset (if read).
	  /// </summary>
	  /// <param name="ch"> An array holding the characters in the comment. </param>
	  /// <param name="start"> The starting position in the array. </param>
	  /// <param name="length"> The number of characters to use from the array. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void comment(char[] ch, int start, int length)
	  {
		append(m_doc.createComment(new string(ch, start, length)));
	  }

	  /// <summary>
	  /// Flag indicating that we are processing a CData section </summary>
	  protected internal bool m_inCData = false;

	  /// <summary>
	  /// Report the start of a CDATA section.
	  /// </summary>
	  /// <seealso cref=".endCDATA"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
	  public virtual void startCDATA()
	  {
		m_inCData = true;
		append(m_doc.createCDATASection(""));
	  }

	  /// <summary>
	  /// Report the end of a CDATA section.
	  /// </summary>
	  /// <seealso cref=".startCDATA"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
	  public virtual void endCDATA()
	  {
		m_inCData = false;
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
	  /// <seealso cref=".ignorableWhitespace"/>
	  /// <seealso cref="org.xml.sax.Locator"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void cdata(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void cdata(char[] ch, int start, int length)
	  {
		if (OutsideDocElem && org.apache.xml.utils.XMLCharacterRecognizer.isWhiteSpace(ch, start, length))
		{
		  return; // avoid DOM006 Hierarchy request error
		}

		string s = new string(ch, start, length);

		CDATASection section = (CDATASection) m_currentNode.getLastChild();
		section.appendData(s);
	  }

	  /// <summary>
	  /// Report the start of DTD declarations, if any.
	  /// 
	  /// Any declarations are assumed to be in the internal subset
	  /// unless otherwise indicated.
	  /// </summary>
	  /// <param name="name"> The document type name. </param>
	  /// <param name="publicId"> The declared public identifier for the
	  ///        external DTD subset, or null if none was declared. </param>
	  /// <param name="systemId"> The declared system identifier for the
	  ///        external DTD subset, or null if none was declared. </param>
	  /// <seealso cref=".endDTD"/>
	  /// <seealso cref=".startEntity"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
	  public virtual void startDTD(string name, string publicId, string systemId)
	  {

		// Do nothing for now.
	  }

	  /// <summary>
	  /// Report the end of DTD declarations.
	  /// </summary>
	  /// <seealso cref=".startDTD"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
	  public virtual void endDTD()
	  {

		// Do nothing for now.
	  }

	  /// <summary>
	  /// Begin the scope of a prefix-URI Namespace mapping.
	  /// 
	  /// <para>The information from this event is not necessary for
	  /// normal Namespace processing: the SAX XML reader will
	  /// automatically replace prefixes for element and attribute
	  /// names when the http://xml.org/sax/features/namespaces
	  /// feature is true (the default).</para>
	  /// 
	  /// <para>There are cases, however, when applications need to
	  /// use prefixes in character data or in attribute values,
	  /// where they cannot safely be expanded automatically; the
	  /// start/endPrefixMapping event supplies the information
	  /// to the application to expand prefixes in those contexts
	  /// itself, if necessary.</para>
	  /// 
	  /// <para>Note that start/endPrefixMapping events are not
	  /// guaranteed to be properly nested relative to each-other:
	  /// all startPrefixMapping events will occur before the
	  /// corresponding startElement event, and all endPrefixMapping
	  /// events will occur after the corresponding endElement event,
	  /// but their order is not guaranteed.</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <param name="uri"> The Namespace URI the prefix is mapped to. </param>
	  /// <seealso cref=".endPrefixMapping"/>
	  /// <seealso cref=".startElement"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
	  public virtual void startPrefixMapping(string prefix, string uri)
	  {
			  if (null == prefix || prefix.Length == 0)
			  {
				prefix = "xmlns";
			  }
			  else
			  {
				  prefix = "xmlns:" + prefix;
			  }
			  m_prefixMappings.Add(prefix);
			  m_prefixMappings.Add(uri);
	  }

	  /// <summary>
	  /// End the scope of a prefix-URI mapping.
	  /// 
	  /// <para>See startPrefixMapping for details.  This event will
	  /// always occur after the corresponding endElement event,
	  /// but the order of endPrefixMapping events is not otherwise
	  /// guaranteed.</para>
	  /// </summary>
	  /// <param name="prefix"> The prefix that was being mapping. </param>
	  /// <seealso cref=".startPrefixMapping"/>
	  /// <seealso cref=".endElement"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
	  public virtual void endPrefixMapping(string prefix)
	  {
	  }

	  /// <summary>
	  /// Receive notification of a skipped entity.
	  /// 
	  /// <para>The Parser will invoke this method once for each entity
	  /// skipped.  Non-validating processors may skip entities if they
	  /// have not seen the declarations (because, for example, the
	  /// entity was declared in an external DTD subset).  All processors
	  /// may skip external entities, depending on the values of the
	  /// http://xml.org/sax/features/external-general-entities and the
	  /// http://xml.org/sax/features/external-parameter-entities
	  /// properties.</para>
	  /// </summary>
	  /// <param name="name"> The name of the skipped entity.  If it is a
	  ///        parameter entity, the name will begin with '%'. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
	  public virtual void skippedEntity(string name)
	  {
	  }
	}

}