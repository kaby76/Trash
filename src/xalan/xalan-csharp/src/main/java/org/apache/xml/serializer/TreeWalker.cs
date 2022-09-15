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
 * $Id: TreeWalker.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	using AttList = org.apache.xml.serializer.utils.AttList;
	using DOM2Helper = org.apache.xml.serializer.utils.DOM2Helper;
	using Comment = org.w3c.dom.Comment;
	using Element = org.w3c.dom.Element;
	using EntityReference = org.w3c.dom.EntityReference;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using ProcessingInstruction = org.w3c.dom.ProcessingInstruction;
	using Text = org.w3c.dom.Text;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;
	using LocatorImpl = org.xml.sax.helpers.LocatorImpl;


	/// <summary>
	/// This class does a pre-order walk of the DOM tree, calling a ContentHandler
	/// interface as it goes.
	/// 
	/// This class is a copy of the one in org.apache.xml.utils. 
	/// It exists to cut the serializers dependancy on that package.
	/// 
	/// @xsl.usage internal
	/// </summary>

	public sealed class TreeWalker
	{

	  /// <summary>
	  /// Local reference to a ContentHandler </summary>
	  private readonly ContentHandler m_contentHandler;
	  /// <summary>
	  /// If m_contentHandler is a SerializationHandler, then this is 
	  /// a reference to the same object. 
	  /// </summary>
	  private readonly SerializationHandler m_Serializer;

	  // ARGHH!!  JAXP Uses Xerces without setting the namespace processing to ON!
	  // DOM2Helper m_dh = new DOM2Helper();

	  /// <summary>
	  /// DomHelper for this TreeWalker </summary>
	  protected internal readonly DOM2Helper m_dh;

	  /// <summary>
	  /// Locator object for this TreeWalker </summary>
	  private readonly LocatorImpl m_locator = new LocatorImpl();

	  /// <summary>
	  /// Get the ContentHandler used for the tree walk.
	  /// </summary>
	  /// <returns> the ContentHandler used for the tree walk </returns>
	  public ContentHandler ContentHandler
	  {
		  get
		  {
			return m_contentHandler;
		  }
	  }

	  public TreeWalker(ContentHandler ch) : this(ch,null)
	  {
	  }
	  /// <summary>
	  /// Constructor. </summary>
	  /// <param name="contentHandler"> The implemention of the
	  /// contentHandler operation (toXMLString, digest, ...) </param>
	  public TreeWalker(ContentHandler contentHandler, string systemId)
	  {
		  // Set the content handler
		  m_contentHandler = contentHandler;
		  if (m_contentHandler is SerializationHandler)
		  {
			  m_Serializer = (SerializationHandler) m_contentHandler;
		  }
		  else
		  {
			  m_Serializer = null;
		  }

		  // Set the system ID, if it is given
		  m_contentHandler.setDocumentLocator(m_locator);
		  if (!string.ReferenceEquals(systemId, null))
		  {
			  m_locator.setSystemId(systemId);
		  }
		  else
		  {
			  try
			  {
				// Bug see Bugzilla  26741
				m_locator.setSystemId(System.getProperty("user.dir") + File.separator + "dummy.xsl");
			  }
			   catch (SecurityException)
			   { // user.dir not accessible from applet
			   }
		  }

		  // Set the document locator  
					if (m_contentHandler != null)
					{
							m_contentHandler.setDocumentLocator(m_locator);
					}
					try
					{
					   // Bug see Bugzilla  26741
					  m_locator.setSystemId(System.getProperty("user.dir") + File.separator + "dummy.xsl");
					}
					catch (SecurityException)
					{ // user.dir not accessible from applet

					}
		m_dh = new DOM2Helper();
	  }

	  /// <summary>
	  /// Perform a pre-order traversal non-recursive style.  
	  /// 
	  /// Note that TreeWalker assumes that the subtree is intended to represent 
	  /// a complete (though not necessarily well-formed) document and, during a 
	  /// traversal, startDocument and endDocument will always be issued to the 
	  /// SAX listener.
	  /// </summary>
	  /// <param name="pos"> Node in the tree where to start traversal
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void traverse(org.w3c.dom.Node pos) throws org.xml.sax.SAXException
	  public void traverse(Node pos)
	  {

		this.m_contentHandler.startDocument();

		Node top = pos;

		while (null != pos)
		{
		  startNode(pos);

		  Node nextNode = pos.getFirstChild();

		  while (null == nextNode)
		  {
			endNode(pos);

			if (top.Equals(pos))
			{
			  break;
			}

			nextNode = pos.getNextSibling();

			if (null == nextNode)
			{
			  pos = pos.getParentNode();

			  if ((null == pos) || (top.Equals(pos)))
			  {
				if (null != pos)
				{
				  endNode(pos);
				}

				nextNode = null;

				break;
			  }
			}
		  }

		  pos = nextNode;
		}
		this.m_contentHandler.endDocument();
	  }

	  /// <summary>
	  /// Perform a pre-order traversal non-recursive style.
	  /// 
	  /// Note that TreeWalker assumes that the subtree is intended to represent 
	  /// a complete (though not necessarily well-formed) document and, during a 
	  /// traversal, startDocument and endDocument will always be issued to the 
	  /// SAX listener.
	  /// </summary>
	  /// <param name="pos"> Node in the tree where to start traversal </param>
	  /// <param name="top"> Node in the tree where to end traversal
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void traverse(org.w3c.dom.Node pos, org.w3c.dom.Node top) throws org.xml.sax.SAXException
	  public void traverse(Node pos, Node top)
	  {

		this.m_contentHandler.startDocument();

		while (null != pos)
		{
		  startNode(pos);

		  Node nextNode = pos.getFirstChild();

		  while (null == nextNode)
		  {
			endNode(pos);

			if ((null != top) && top.Equals(pos))
			{
			  break;
			}

			nextNode = pos.getNextSibling();

			if (null == nextNode)
			{
			  pos = pos.getParentNode();

			  if ((null == pos) || ((null != top) && top.Equals(pos)))
			  {
				nextNode = null;

				break;
			  }
			}
		  }

		  pos = nextNode;
		}
		this.m_contentHandler.endDocument();
	  }

	  /// <summary>
	  /// Flag indicating whether following text to be processed is raw text </summary>
	  internal bool nextIsRaw = false;

	  /// <summary>
	  /// Optimized dispatch of characters.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private final void dispatachChars(org.w3c.dom.Node node) throws org.xml.sax.SAXException
	  private void dispatachChars(Node node)
	  {
		if (m_Serializer != null)
		{
		  this.m_Serializer.characters(node);
		}
		else
		{
		  string data = ((Text) node).getData();
		  this.m_contentHandler.characters(data.ToCharArray(), 0, data.Length);
		}
	  }

	  /// <summary>
	  /// Start processing given node
	  /// 
	  /// </summary>
	  /// <param name="node"> Node to process
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void startNode(org.w3c.dom.Node node) throws org.xml.sax.SAXException
	  protected internal void startNode(Node node)
	  {

	//   TODO: <REVIEW>
	//    A Serializer implements ContentHandler, but not NodeConsumer
	//    so drop this reference to NodeConsumer which would otherwise
	//    pull in all sorts of things
	//    if (m_contentHandler instanceof NodeConsumer)
	//    {
	//      ((NodeConsumer) m_contentHandler).setOriginatingNode(node);
	//    }
	//    TODO: </REVIEW>

					if (node is Locator)
					{
							Locator loc = (Locator)node;
							m_locator.setColumnNumber(loc.getColumnNumber());
							m_locator.setLineNumber(loc.getLineNumber());
							m_locator.setPublicId(loc.getPublicId());
							m_locator.setSystemId(loc.getSystemId());
					}
					else
					{
							m_locator.setColumnNumber(0);
		  m_locator.setLineNumber(0);
					}

		switch (node.getNodeType())
		{
		case Node.COMMENT_NODE :
		{
		  string data = ((Comment) node).getData();

		  if (m_contentHandler is LexicalHandler)
		  {
			LexicalHandler lh = ((LexicalHandler) this.m_contentHandler);

			lh.comment(data.ToCharArray(), 0, data.Length);
		  }
		}
		break;
		case Node.DOCUMENT_FRAGMENT_NODE :

		  // ??;
		  break;
		case Node.DOCUMENT_NODE :

		  break;
		case Node.ELEMENT_NODE :
		  Element elem_node = (Element) node;
		  {
			  // Make sure the namespace node
			  // for the element itself is declared
			  // to the ContentHandler
			  string uri = elem_node.getNamespaceURI();
			  if (!string.ReferenceEquals(uri, null))
			  {
				  string prefix = elem_node.getPrefix();
				  if (string.ReferenceEquals(prefix, null))
				  {
					prefix = "";
				  }
				  this.m_contentHandler.startPrefixMapping(prefix,uri);
			  }
		  }
		  NamedNodeMap atts = elem_node.getAttributes();
		  int nAttrs = atts.getLength();
		  // System.out.println("TreeWalker#startNode: "+node.getNodeName());


		  // Make sure the namespace node of
		  // each attribute is declared to the ContentHandler
		  for (int i = 0; i < nAttrs; i++)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node attr = atts.item(i);
			Node attr = atts.item(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String attrName = attr.getNodeName();
			string attrName = attr.getNodeName();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = attrName.indexOf(':');
			int colon = attrName.IndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix;
			string prefix;

			// System.out.println("TreeWalker#startNode: attr["+i+"] = "+attrName+", "+attr.getNodeValue());
			if (attrName.Equals("xmlns") || attrName.StartsWith("xmlns:", StringComparison.Ordinal))
			{
			  // Use "" instead of null, as Xerces likes "" for the 
			  // name of the default namespace.  Fix attributed 
			  // to "Steven Murray" <smurray@ebt.com>.
			  if (colon < 0)
			  {
				prefix = "";
			  }
			  else
			  {
				prefix = attrName.Substring(colon + 1);
			  }

			  this.m_contentHandler.startPrefixMapping(prefix, attr.getNodeValue());
			}
			else if (colon > 0)
			{
				prefix = attrName.Substring(0, colon);
				string uri = attr.getNamespaceURI();
				if (!string.ReferenceEquals(uri, null))
				{
					this.m_contentHandler.startPrefixMapping(prefix,uri);
				}
			}
		  }

		  string ns = m_dh.getNamespaceOfNode(node);
		  if (null == ns)
		  {
			ns = "";
		  }
		  this.m_contentHandler.startElement(ns, m_dh.getLocalNameOfNode(node), node.getNodeName(), new AttList(atts, m_dh));
		  break;
		case Node.PROCESSING_INSTRUCTION_NODE :
		{
		  ProcessingInstruction pi = (ProcessingInstruction) node;
		  string name = pi.getNodeName();

		  // String data = pi.getData();
		  if (name.Equals("xslt-next-is-raw"))
		  {
			nextIsRaw = true;
		  }
		  else
		  {
			this.m_contentHandler.processingInstruction(pi.getNodeName(), pi.getData());
		  }
		}
		break;
		case Node.CDATA_SECTION_NODE :
		{
		  bool isLexH = (m_contentHandler is LexicalHandler);
		  LexicalHandler lh = isLexH ? ((LexicalHandler) this.m_contentHandler) : null;

		  if (isLexH)
		  {
			lh.startCDATA();
		  }

		  dispatachChars(node);

		  {
			if (isLexH)
			{
			  lh.endCDATA();
			}
		  }
		}
		break;
		case Node.TEXT_NODE :
		{
		  //String data = ((Text) node).getData();

		  if (nextIsRaw)
		  {
			nextIsRaw = false;

			m_contentHandler.processingInstruction(javax.xml.transform.Result.PI_DISABLE_OUTPUT_ESCAPING, "");
			dispatachChars(node);
			m_contentHandler.processingInstruction(javax.xml.transform.Result.PI_ENABLE_OUTPUT_ESCAPING, "");
		  }
		  else
		  {
			dispatachChars(node);
		  }
		}
		break;
		case Node.ENTITY_REFERENCE_NODE :
		{
		  EntityReference eref = (EntityReference) node;

		  if (m_contentHandler is LexicalHandler)
		  {
			((LexicalHandler) this.m_contentHandler).startEntity(eref.getNodeName());
		  }
		  else
		  {

			// warning("Can not output entity to a pure SAX ContentHandler");
		  }
		}
		break;
		default :
		}
	  }

	  /// <summary>
	  /// End processing of given node 
	  /// 
	  /// </summary>
	  /// <param name="node"> Node we just finished processing
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void endNode(org.w3c.dom.Node node) throws org.xml.sax.SAXException
	  protected internal void endNode(Node node)
	  {

		switch (node.getNodeType())
		{
		case Node.DOCUMENT_NODE :
		  break;

		case Node.ELEMENT_NODE :
		  string ns = m_dh.getNamespaceOfNode(node);
		  if (null == ns)
		  {
			ns = "";
		  }
		  this.m_contentHandler.endElement(ns, m_dh.getLocalNameOfNode(node), node.getNodeName());

		  if (m_Serializer == null)
		  {
		  // Don't bother with endPrefixMapping calls if the ContentHandler is a
		  // SerializationHandler because SerializationHandler's ignore the
		  // endPrefixMapping() calls anyways. . . .  This is an optimization.    
		  Element elem_node = (Element) node;
		  NamedNodeMap atts = elem_node.getAttributes();
		  int nAttrs = atts.getLength();

		  // do the endPrefixMapping calls in reverse order 
		  // of the startPrefixMapping calls
		  for (int i = (nAttrs - 1); 0 <= i; i--)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node attr = atts.item(i);
			Node attr = atts.item(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String attrName = attr.getNodeName();
			string attrName = attr.getNodeName();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = attrName.indexOf(':');
			int colon = attrName.IndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix;
			string prefix;

			if (attrName.Equals("xmlns") || attrName.StartsWith("xmlns:", StringComparison.Ordinal))
			{
			  // Use "" instead of null, as Xerces likes "" for the 
			  // name of the default namespace.  Fix attributed 
			  // to "Steven Murray" <smurray@ebt.com>.
			  if (colon < 0)
			  {
				prefix = "";
			  }
			  else
			  {
				prefix = attrName.Substring(colon + 1);
			  }

			  this.m_contentHandler.endPrefixMapping(prefix);
			}
			else if (colon > 0)
			{
				prefix = attrName.Substring(0, colon);
				this.m_contentHandler.endPrefixMapping(prefix);
			}
		  }
		  {
			  string uri = elem_node.getNamespaceURI();
			  if (!string.ReferenceEquals(uri, null))
			  {
				  string prefix = elem_node.getPrefix();
				  if (string.ReferenceEquals(prefix, null))
				  {
					prefix = "";
				  }
				  this.m_contentHandler.endPrefixMapping(prefix);
			  }
		  }
		  }
		  break;
		case Node.CDATA_SECTION_NODE :
		  break;
		case Node.ENTITY_REFERENCE_NODE :
		{
		  EntityReference eref = (EntityReference) node;

		  if (m_contentHandler is LexicalHandler)
		  {
			LexicalHandler lh = ((LexicalHandler) this.m_contentHandler);

			lh.endEntity(eref.getNodeName());
		  }
		}
		break;
		default :
		}
	  }
	} //TreeWalker


}