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
 * $Id: TreeWalker.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

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
	/// @xsl.usage advanced
	/// </summary>

	public class TreeWalker
	{

	  /// <summary>
	  /// Local reference to a ContentHandler </summary>
	  private ContentHandler m_contentHandler = null;

	  // ARGHH!!  JAXP Uses Xerces without setting the namespace processing to ON!
	  // DOM2Helper m_dh = new DOM2Helper();

	  /// <summary>
	  /// DomHelper for this TreeWalker </summary>
	  protected internal DOMHelper m_dh;

			/// <summary>
			/// Locator object for this TreeWalker </summary>
			private LocatorImpl m_locator = new LocatorImpl();

	  /// <summary>
	  /// Get the ContentHandler used for the tree walk.
	  /// </summary>
	  /// <returns> the ContentHandler used for the tree walk </returns>
	  public virtual ContentHandler ContentHandler
	  {
		  get
		  {
			return m_contentHandler;
		  }
		  set
		  {
			m_contentHandler = value;
		  }
	  }


			/// <summary>
			/// Constructor. </summary>
			/// <param name="contentHandler"> The implemention of the </param>
			/// <param name="systemId"> System identifier for the document.
			/// contentHandler operation (toXMLString, digest, ...) </param>
	  public TreeWalker(ContentHandler contentHandler, DOMHelper dh, string systemId)
	  {
		this.m_contentHandler = contentHandler;
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
		m_dh = dh;
	  }

	  /// <summary>
	  /// Constructor. </summary>
	  /// <param name="contentHandler"> The implemention of the
	  /// contentHandler operation (toXMLString, digest, ...) </param>
	  public TreeWalker(ContentHandler contentHandler, DOMHelper dh)
	  {
		this.m_contentHandler = contentHandler;
		m_contentHandler.setDocumentLocator(m_locator);
		try
		{
		   // Bug see Bugzilla  26741
		  m_locator.setSystemId(System.getProperty("user.dir") + File.separator + "dummy.xsl");
		}
		catch (SecurityException)
		{ // user.dir not accessible from applet
		}
		m_dh = dh;
	  }

	  /// <summary>
	  /// Constructor. </summary>
	  /// <param name="contentHandler"> The implemention of the
	  /// contentHandler operation (toXMLString, digest, ...) </param>
	  public TreeWalker(ContentHandler contentHandler)
	  {
		this.m_contentHandler = contentHandler;
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
	  public virtual void traverse(Node pos)
	  {
			this.m_contentHandler.startDocument();

			traverseFragment(pos);

			this.m_contentHandler.endDocument();
	  }

	  /// <summary>
	  /// Perform a pre-order traversal non-recursive style.  
	  /// 
	  /// In contrast to the traverse() method this method will not issue 
	  /// startDocument() and endDocument() events to the SAX listener.
	  /// </summary>
	  /// <param name="pos"> Node in the tree where to start traversal
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void traverseFragment(org.w3c.dom.Node pos) throws org.xml.sax.SAXException
	  public virtual void traverseFragment(Node pos)
	  {
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
	  public virtual void traverse(Node pos, Node top)
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
		if (m_contentHandler is org.apache.xml.dtm.@ref.dom2dtm.DOM2DTM.CharacterNodeHandler)
		{
		  ((org.apache.xml.dtm.@ref.dom2dtm.DOM2DTM.CharacterNodeHandler)m_contentHandler).characters(node);
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
	  protected internal virtual void startNode(Node node)
	  {

		if (m_contentHandler is NodeConsumer)
		{
		  ((NodeConsumer) m_contentHandler).OriginatingNode = node;
		}

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
		  NamedNodeMap atts = ((Element) node).getAttributes();
		  int nAttrs = atts.getLength();
		  // System.out.println("TreeWalker#startNode: "+node.getNodeName());

		  for (int i = 0; i < nAttrs; i++)
		  {
			Node attr = atts.item(i);
			string attrName = attr.getNodeName();

			// System.out.println("TreeWalker#startNode: attr["+i+"] = "+attrName+", "+attr.getNodeValue());
			if (attrName.Equals("xmlns") || attrName.StartsWith("xmlns:", StringComparison.Ordinal))
			{
			  // System.out.println("TreeWalker#startNode: attr["+i+"] = "+attrName+", "+attr.getNodeValue());
			  int index;
			  // Use "" instead of null, as Xerces likes "" for the 
			  // name of the default namespace.  Fix attributed 
			  // to "Steven Murray" <smurray@ebt.com>.
			  string prefix = (index = attrName.IndexOf(":", StringComparison.Ordinal)) < 0 ? "" : attrName.Substring(index + 1);

			  this.m_contentHandler.startPrefixMapping(prefix, attr.getNodeValue());
			}

		  }

		  // System.out.println("m_dh.getNamespaceOfNode(node): "+m_dh.getNamespaceOfNode(node));
		  // System.out.println("m_dh.getLocalNameOfNode(node): "+m_dh.getLocalNameOfNode(node));
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
	  protected internal virtual void endNode(Node node)
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

		  NamedNodeMap atts = ((Element) node).getAttributes();
		  int nAttrs = atts.getLength();

		  for (int i = 0; i < nAttrs; i++)
		  {
			Node attr = atts.item(i);
			string attrName = attr.getNodeName();

			if (attrName.Equals("xmlns") || attrName.StartsWith("xmlns:", StringComparison.Ordinal))
			{
			  int index;
			  // Use "" instead of null, as Xerces likes "" for the 
			  // name of the default namespace.  Fix attributed 
			  // to "Steven Murray" <smurray@ebt.com>.
			  string prefix = (index = attrName.IndexOf(":", StringComparison.Ordinal)) < 0 ? "" : attrName.Substring(index + 1);

			  this.m_contentHandler.endPrefixMapping(prefix);
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