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
 * $Id: DTMNodeProxy.java 889881 2009-12-12 03:47:15Z zongaro $
 */
namespace org.apache.xml.dtm.@ref
{

	using NodeSet = org.apache.xpath.NodeSet;

	using Attr = org.w3c.dom.Attr;
	using CDATASection = org.w3c.dom.CDATASection;
	using Comment = org.w3c.dom.Comment;
	using DOMException = org.w3c.dom.DOMException;
	using DOMImplementation = org.w3c.dom.DOMImplementation;
	using Document = org.w3c.dom.Document;
	using DocumentFragment = org.w3c.dom.DocumentFragment;
	using DocumentType = org.w3c.dom.DocumentType;
	using Element = org.w3c.dom.Element;
	using EntityReference = org.w3c.dom.EntityReference;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using ProcessingInstruction = org.w3c.dom.ProcessingInstruction;
	using Text = org.w3c.dom.Text;

	using UserDataHandler = org.w3c.dom.UserDataHandler;
	using DOMConfiguration = org.w3c.dom.DOMConfiguration;
	using TypeInfo = org.w3c.dom.TypeInfo;

	/// <summary>
	/// <code>DTMNodeProxy</code> presents a DOM Node API front-end to the DTM model.
	/// <para>
	/// It does _not_ attempt to address the "node identity" question; no effort
	/// is made to prevent the creation of multiple proxies referring to a single
	/// DTM node. Users can create a mechanism for managing this, or relinquish the
	/// use of "==" and use the .sameNodeAs() mechanism, which is under
	/// consideration for future versions of the DOM.
	/// </para>
	/// <para>
	/// DTMNodeProxy may be subclassed further to present specific DOM node types.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= org.w3c.dom
	/// @xsl.usage internal </seealso>
	public class DTMNodeProxy : Node, Document, Text, Element, Attr, ProcessingInstruction, Comment, DocumentFragment
	{

	  /// <summary>
	  /// The DTM for this node. </summary>
	  public DTM dtm;

	  /// <summary>
	  /// The DTM node handle. </summary>
	  internal int node;

	  /// <summary>
	  /// The return value as Empty String. </summary>
	  private const string EMPTYSTRING = "";

	  /// <summary>
	  /// The DOMImplementation object </summary>
	  internal static readonly DOMImplementation implementation = new DTMNodeProxyImplementation();

	  /// <summary>
	  /// Create a DTMNodeProxy Node representing a specific Node in a DTM
	  /// </summary>
	  /// <param name="dtm"> The DTM Reference, must be non-null. </param>
	  /// <param name="node"> The DTM node handle. </param>
	  public DTMNodeProxy(DTM dtm, int node)
	  {
		this.dtm = dtm;
		this.node = node;
	  }

	  /// <summary>
	  /// NON-DOM: Return the DTM model
	  /// </summary>
	  /// <returns> The DTM that this proxy is a representative for. </returns>
	  public DTM DTM
	  {
		  get
		  {
			return dtm;
		  }
	  }

	  /// <summary>
	  /// NON-DOM: Return the DTM node number
	  /// </summary>
	  /// <returns> The DTM node handle. </returns>
	  public int DTMNodeNumber
	  {
		  get
		  {
			return node;
		  }
	  }

	  /// <summary>
	  /// Test for equality based on node number.
	  /// </summary>
	  /// <param name="node"> A DTM node proxy reference.
	  /// </param>
	  /// <returns> true if the given node has the same handle as this node. </returns>
	  public bool Equals(Node node)
	  {

		try
		{
		  DTMNodeProxy dtmp = (DTMNodeProxy) node;

		  // return (dtmp.node == this.node);
		  // Patch attributed to Gary L Peskin <garyp@firstech.com>
		  return (dtmp.node == this.node) && (dtmp.dtm == this.dtm);
		}
		catch (System.InvalidCastException)
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Test for equality based on node number.
	  /// </summary>
	  /// <param name="node"> A DTM node proxy reference.
	  /// </param>
	  /// <returns> true if the given node has the same handle as this node. </returns>
	  public sealed override bool Equals(object node)
	  {

		try
		{

		  // DTMNodeProxy dtmp = (DTMNodeProxy)node;
		  // return (dtmp.node == this.node);
		  // Patch attributed to Gary L Peskin <garyp@firstech.com>
		  return Equals((Node) node);
		}
		catch (System.InvalidCastException)
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// FUTURE DOM: Test node identity, in lieu of Node==Node
	  /// </summary>
	  /// <param name="other">
	  /// </param>
	  /// <returns> true if the given node has the same handle as this node. </returns>
	  public bool sameNodeAs(Node other)
	  {

		if (!(other is DTMNodeProxy))
		{
		  return false;
		}

		DTMNodeProxy that = (DTMNodeProxy) other;

		return this.dtm == that.dtm && this.node == that.node;
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public string NodeName
	  {
		  get
		  {
			return dtm.getNodeName(node);
		  }
	  }

	  /// <summary>
	  /// A PI's "target" states what processor channel the PI's data
	  /// should be directed to. It is defined differently in HTML and XML.
	  /// <para>
	  /// In XML, a PI's "target" is the first (whitespace-delimited) token
	  /// following the "<?" token that begins the PI.
	  /// </para>
	  /// <para>
	  /// In HTML, target is always null.
	  /// </para>
	  /// <para>
	  /// Note that getNodeName is aliased to getTarget.
	  /// 
	  /// 
	  /// </para>
	  /// </summary>
	  public string Target
	  {
		  get
		  {
			return dtm.getNodeName(node);
		  }
	  } // getTarget():String

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node as of DOM Level 2 </seealso>
	  public string LocalName
	  {
		  get
		  {
			return dtm.getLocalName(node);
		  }
	  }

	  /// <returns> The prefix for this node. </returns>
	  /// <seealso cref= org.w3c.dom.Node as of DOM Level 2 </seealso>
	  public string Prefix
	  {
		  get
		  {
			return dtm.getPrefix(node);
		  }
		  set
		  {
			throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
		  }
	  }


	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node as of DOM Level 2 </seealso>
	  public string NamespaceURI
	  {
		  get
		  {
			return dtm.getNamespaceURI(node);
		  }
	  }

	  /// <summary>
	  /// Ask whether we support a given DOM feature.
	  /// In fact, we do not _fully_ support any DOM feature -- we're a
	  /// read-only subset -- so arguably we should always return false.
	  /// Or we could say that we support DOM Core Level 2 but all nodes
	  /// are read-only. Unclear which answer is least misleading.
	  /// 
	  /// NON-DOM method. This was present in early drafts of DOM Level 2,
	  /// but was renamed isSupported. It's present here only because it's
	  /// cheap, harmless, and might help some poor fool who is still trying
	  /// to use an early Working Draft of the DOM.
	  /// </summary>
	  /// <param name="feature"> </param>
	  /// <param name="version">
	  /// </param>
	  /// <returns> false </returns>
	  public bool supports(string feature, string version)
	  {
		return implementation.hasFeature(feature,version);
		//throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// <summary>
	  /// Ask whether we support a given DOM feature.
	  /// In fact, we do not _fully_ support any DOM feature -- we're a
	  /// read-only subset -- so arguably we should always return false.
	  /// </summary>
	  /// <param name="feature"> </param>
	  /// <param name="version">
	  /// </param>
	  /// <returns> false </returns>
	  /// <seealso cref= org.w3c.dom.Node as of DOM Level 2 </seealso>
	  public bool isSupported(string feature, string version)
	  {
		return implementation.hasFeature(feature,version);
		// throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// 
	  /// 
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Node </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final String getNodeValue() throws org.w3c.dom.DOMException
	  public string NodeValue
	  {
		  get
		  {
			return dtm.getNodeValue(node);
		  }
		  set
		  {
			throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
		  }
	  }

	  /// <returns> The string value of the node
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final String getStringValue() throws org.w3c.dom.DOMException
	  public string StringValue
	  {
		  get
		  {
			  return dtm.getStringValue(node).ToString();
		  }
	  }


	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public short NodeType
	  {
		  get
		  {
			return (short) dtm.getNodeType(node);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Node ParentNode
	  {
		  get
		  {
    
			if (NodeType == Node.ATTRIBUTE_NODE)
			{
			  return null;
			}
    
			int newnode = dtm.getParent(node);
    
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : dtm.getNode(newnode);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Node OwnerNode
	  {
		  get
		  {
    
			int newnode = dtm.getParent(node);
    
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : dtm.getNode(newnode);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public NodeList ChildNodes
	  {
		  get
		  {
    
			// Annoyingly, AxisIterators do not currently implement DTMIterator, so
			// we can't just wap DTMNodeList around an Axis.CHILD iterator.
			// Instead, we've created a special-case operating mode for that object.
			return new DTMChildIterNodeList(dtm,node);
    
			// throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Node FirstChild
	  {
		  get
		  {
    
			int newnode = dtm.getFirstChild(node);
    
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : dtm.getNode(newnode);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Node LastChild
	  {
		  get
		  {
    
			int newnode = dtm.getLastChild(node);
    
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : dtm.getNode(newnode);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Node PreviousSibling
	  {
		  get
		  {
    
			int newnode = dtm.getPreviousSibling(node);
    
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : dtm.getNode(newnode);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Node NextSibling
	  {
		  get
		  {
    
			// Attr's Next is defined at DTM level, but not at DOM level.
			if (dtm.getNodeType(node) == Node.ATTRIBUTE_NODE)
			{
			  return null;
			}
    
			int newnode = dtm.getNextSibling(node);
    
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : dtm.getNode(newnode);
		  }
	  }

	  // DTMNamedNodeMap m_attrs;

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public NamedNodeMap Attributes
	  {
		  get
		  {
    
			return new DTMNamedNodeMap(dtm, node);
		  }
	  }

	  /// <summary>
	  /// Method hasAttribute
	  /// 
	  /// </summary>
	  /// <param name="name">
	  /// 
	  ///  </param>
	  public virtual bool hasAttribute(string name)
	  {
		return org.apache.xml.dtm.DTM_Fields.NULL != dtm.getAttributeNode(node,null,name);
	  }

	  /// <summary>
	  /// Method hasAttributeNS
	  /// 
	  /// </summary>
	  /// <param name="namespaceURI"> </param>
	  /// <param name="localName">
	  /// 
	  ///  </param>
	  public virtual bool hasAttributeNS(string namespaceURI, string localName)
	  {
		return org.apache.xml.dtm.DTM_Fields.NULL != dtm.getAttributeNode(node,namespaceURI,localName);
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public Document OwnerDocument
	  {
		  get
		  {
			  // Note that this uses the DOM-compatable version of the call
			return (Document)(dtm.getNode(dtm.getOwnerDocument(node)));
		  }
	  }

	  /// 
	  /// <param name="newChild"> </param>
	  /// <param name="refChild">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Node -- DTMNodeProxy is read-only </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Node insertBefore(org.w3c.dom.Node newChild, org.w3c.dom.Node refChild) throws org.w3c.dom.DOMException
	  public Node insertBefore(Node newChild, Node refChild)
	  {
		throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// 
	  /// <param name="newChild"> </param>
	  /// <param name="oldChild">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Node -- DTMNodeProxy is read-only </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Node replaceChild(org.w3c.dom.Node newChild, org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
	  public Node replaceChild(Node newChild, Node oldChild)
	  {
		throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// 
	  /// <param name="oldChild">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Node -- DTMNodeProxy is read-only </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Node removeChild(org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
	  public Node removeChild(Node oldChild)
	  {
		throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// 
	  /// <param name="newChild">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Node -- DTMNodeProxy is read-only </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Node appendChild(org.w3c.dom.Node newChild) throws org.w3c.dom.DOMException
	  public Node appendChild(Node newChild)
	  {
		throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Node </seealso>
	  public bool hasChildNodes()
	  {
		return (org.apache.xml.dtm.DTM_Fields.NULL != dtm.getFirstChild(node));
	  }

	  /// 
	  /// <param name="deep">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Node -- DTMNodeProxy is read-only </seealso>
	  public Node cloneNode(bool deep)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public DocumentType Doctype
	  {
		  get
		  {
			return null;
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public DOMImplementation Implementation
	  {
		  get
		  {
			return implementation;
		  }
	  }

	  /// <summary>
	  /// This is a bit of a problem in DTM, since a DTM may be a Document
	  /// Fragment and hence not have a clear-cut Document Element. We can
	  /// make it work in the well-formed cases but would that be confusing for others?
	  /// 
	  /// </summary>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public Element DocumentElement
	  {
		  get
		  {
				int dochandle = dtm.Document;
				int elementhandle = org.apache.xml.dtm.DTM_Fields.NULL;
				for (int kidhandle = dtm.getFirstChild(dochandle); kidhandle != org.apache.xml.dtm.DTM_Fields.NULL; kidhandle = dtm.getNextSibling(kidhandle))
				{
					switch (dtm.getNodeType(kidhandle))
					{
					case Node.ELEMENT_NODE:
						if (elementhandle != org.apache.xml.dtm.DTM_Fields.NULL)
						{
							elementhandle = org.apache.xml.dtm.DTM_Fields.NULL; // More than one; ill-formed.
							kidhandle = dtm.getLastChild(dochandle); // End loop
						}
						else
						{
							elementhandle = kidhandle;
						}
						break;
    
					// These are harmless; document is still wellformed
					case Node.COMMENT_NODE:
					case Node.PROCESSING_INSTRUCTION_NODE:
					case Node.DOCUMENT_TYPE_NODE:
						break;
    
					default:
						elementhandle = org.apache.xml.dtm.DTM_Fields.NULL; // ill-formed
						kidhandle = dtm.getLastChild(dochandle); // End loop
						break;
					}
				}
				if (elementhandle == org.apache.xml.dtm.DTM_Fields.NULL)
				{
					throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
				}
				else
				{
					return (Element)(dtm.getNode(elementhandle));
				}
		  }
	  }

	  /// 
	  /// <param name="tagName">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Element createElement(String tagName) throws org.w3c.dom.DOMException
	  public Element createElement(string tagName)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public DocumentFragment createDocumentFragment()
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="data">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public Text createTextNode(string data)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="data">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public Comment createComment(string data)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="data">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.CDATASection createCDATASection(String data) throws org.w3c.dom.DOMException
	  public CDATASection createCDATASection(string data)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="target"> </param>
	  /// <param name="data">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.ProcessingInstruction createProcessingInstruction(String target, String data) throws org.w3c.dom.DOMException
	  public ProcessingInstruction createProcessingInstruction(string target, string data)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="name">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Attr createAttribute(String name) throws org.w3c.dom.DOMException
	  public Attr createAttribute(string name)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="name">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.EntityReference createEntityReference(String name) throws org.w3c.dom.DOMException
	  public EntityReference createEntityReference(string name)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="tagname">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Document </seealso>
	  public NodeList getElementsByTagName(string tagname)
	  {
		   ArrayList listVector = new ArrayList();
		   Node retNode = dtm.getNode(node);
		   if (retNode != null)
		   {
			 bool isTagNameWildCard = "*".Equals(tagname);
			 if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == retNode.NodeType)
			 {
			   NodeList nodeList = retNode.ChildNodes;
			   for (int i = 0; i < nodeList.Length; i++)
			   {
				 traverseChildren(listVector, nodeList.item(i), tagname, isTagNameWildCard);
			   }
			 }
			 else if (org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE == retNode.NodeType)
			 {
			   traverseChildren(listVector, dtm.getNode(node), tagname, isTagNameWildCard);
			 }
		   }
		   int size = listVector.Count;
		   NodeSet nodeSet = new NodeSet(size);
		   for (int i = 0; i < size; i++)
		   {
			 nodeSet.addNode((Node) listVector[i]);
		   }
		   return (NodeList) nodeSet;
	  }
	  /// 
	  /// <param name="listVector"> </param>
	  /// <param name="tempNode"> </param>
	  /// <param name="tagname"> </param>
	  /// <param name="isTagNameWildCard">
	  /// 
	  /// 
	  /// Private method to be used for recursive iterations to obtain elements by tag name. </param>
	  private void traverseChildren(ArrayList listVector, Node tempNode, string tagname, bool isTagNameWildCard)
	  {
		if (tempNode == null)
		{
		  return;
		}
		else
		{
		  if (tempNode.NodeType == org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE && (isTagNameWildCard || tempNode.NodeName.Equals(tagname)))
		  {
			listVector.Add(tempNode);
		  }
		  if (tempNode.hasChildNodes())
		  {
			NodeList nodeList = tempNode.ChildNodes;
			for (int i = 0; i < nodeList.Length; i++)
			{
			  traverseChildren(listVector, nodeList.item(i), tagname, isTagNameWildCard);
			}
		  }
		}
	  }

	  /// 
	  /// <param name="importedNode"> </param>
	  /// <param name="deep">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document as of DOM Level 2 -- DTMNodeProxy is read-only </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Node importNode(org.w3c.dom.Node importedNode, boolean deep) throws org.w3c.dom.DOMException
	  public Node importNode(Node importedNode, bool deep)
	  {
		throw new DTMDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="qualifiedName">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document as of DOM Level 2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Element createElementNS(String namespaceURI, String qualifiedName) throws org.w3c.dom.DOMException
	  public Element createElementNS(string namespaceURI, string qualifiedName)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="qualifiedName">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Document as of DOM Level 2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Attr createAttributeNS(String namespaceURI, String qualifiedName) throws org.w3c.dom.DOMException
	  public Attr createAttributeNS(string namespaceURI, string qualifiedName)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="localName">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Document as of DOM Level 2 </seealso>
	  public NodeList getElementsByTagNameNS(string namespaceURI, string localName)
	  {
		ArrayList listVector = new ArrayList();
		Node retNode = dtm.getNode(node);
		if (retNode != null)
		{
		  bool isNamespaceURIWildCard = "*".Equals(namespaceURI);
		  bool isLocalNameWildCard = "*".Equals(localName);
		  if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == retNode.NodeType)
		  {
			NodeList nodeList = retNode.ChildNodes;
			for (int i = 0; i < nodeList.Length; i++)
			{
			  traverseChildren(listVector, nodeList.item(i), namespaceURI, localName, isNamespaceURIWildCard, isLocalNameWildCard);
			}
		  }
		  else if (org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE == retNode.NodeType)
		  {
			traverseChildren(listVector, dtm.getNode(node), namespaceURI, localName, isNamespaceURIWildCard, isLocalNameWildCard);
		  }
		}
		int size = listVector.Count;
		NodeSet nodeSet = new NodeSet(size);
		for (int i = 0; i < size; i++)
		{
		  nodeSet.addNode((Node)listVector[i]);
		}
		return (NodeList) nodeSet;
	  }
	  /// 
	  /// <param name="listVector"> </param>
	  /// <param name="tempNode"> </param>
	  /// <param name="namespaceURI"> </param>
	  /// <param name="localname"> </param>
	  /// <param name="isNamespaceURIWildCard"> </param>
	  /// <param name="isLocalNameWildCard">
	  /// 
	  /// Private method to be used for recursive iterations to obtain elements by tag name 
	  /// and namespaceURI. </param>
	  private void traverseChildren(ArrayList listVector, Node tempNode, string namespaceURI, string localname, bool isNamespaceURIWildCard, bool isLocalNameWildCard)
	  {
		if (tempNode == null)
		{
		  return;
		}
		else
		{
		  if (tempNode.NodeType == org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE && (isLocalNameWildCard || tempNode.LocalName.Equals(localname)))
		  {
			string nsURI = tempNode.NamespaceURI;
			if ((string.ReferenceEquals(namespaceURI, null) && string.ReferenceEquals(nsURI, null)) || isNamespaceURIWildCard || (!string.ReferenceEquals(namespaceURI, null) && namespaceURI.Equals(nsURI)))
			{
			  listVector.Add(tempNode);
			}
		  }
		  if (tempNode.hasChildNodes())
		  {
			NodeList nl = tempNode.ChildNodes;
			for (int i = 0; i < nl.Length; i++)
			{
			  traverseChildren(listVector, nl.item(i), namespaceURI, localname, isNamespaceURIWildCard, isLocalNameWildCard);
			}
		  }
		}
	  }
	  /// 
	  /// <param name="elementId">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Document as of DOM Level 2 </seealso>
	  public Element getElementById(string elementId)
	  {
		   return (Element) dtm.getNode(dtm.getElementById(elementId));
	  }

	  /// 
	  /// <param name="offset">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Text </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Text splitText(int offset) throws org.w3c.dom.DOMException
	  public Text splitText(int offset)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// 
	  /// 
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final String getData() throws org.w3c.dom.DOMException
	  public string Data
	  {
		  get
		  {
			return dtm.getNodeValue(node);
		  }
		  set
		  {
			throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
	  }


	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
	  public int Length
	  {
		  get
		  {
			// %OPT% This should do something smarter?
			return dtm.getNodeValue(node).Length;
		  }
	  }

	  /// 
	  /// <param name="offset"> </param>
	  /// <param name="count">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final String substringData(int offset, int count) throws org.w3c.dom.DOMException
	  public string substringData(int offset, int count)
	  {
		return Data.Substring(offset, count);
	  }

	  /// 
	  /// <param name="arg">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void appendData(String arg) throws org.w3c.dom.DOMException
	  public void appendData(string arg)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="offset"> </param>
	  /// <param name="arg">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void insertData(int offset, String arg) throws org.w3c.dom.DOMException
	  public void insertData(int offset, string arg)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="offset"> </param>
	  /// <param name="count">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void deleteData(int offset, int count) throws org.w3c.dom.DOMException
	  public void deleteData(int offset, int count)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="offset"> </param>
	  /// <param name="count"> </param>
	  /// <param name="arg">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.CharacterData </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void replaceData(int offset, int count, String arg) throws org.w3c.dom.DOMException
	  public void replaceData(int offset, int count, string arg)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Element </seealso>
	  public string TagName
	  {
		  get
		  {
			return dtm.getNodeName(node);
		  }
	  }

	  /// 
	  /// <param name="name">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
	  public string getAttribute(string name)
	  {

		DTMNamedNodeMap map = new DTMNamedNodeMap(dtm, node);
		Node node = map.getNamedItem(name);
		return (null == node) ? EMPTYSTRING : node.NodeValue;
	  }

	  /// 
	  /// <param name="name"> </param>
	  /// <param name="value">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setAttribute(String name, String value) throws org.w3c.dom.DOMException
	  public void setAttribute(string name, string value)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="name">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void removeAttribute(String name) throws org.w3c.dom.DOMException
	  public void removeAttribute(string name)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="name">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
	  public Attr getAttributeNode(string name)
	  {

		DTMNamedNodeMap map = new DTMNamedNodeMap(dtm, node);
		return (Attr)map.getNamedItem(name);
	  }

	  /// 
	  /// <param name="newAttr">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Attr setAttributeNode(org.w3c.dom.Attr newAttr) throws org.w3c.dom.DOMException
	  public Attr setAttributeNode(Attr newAttr)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="oldAttr">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Attr removeAttributeNode(org.w3c.dom.Attr oldAttr) throws org.w3c.dom.DOMException
	  public Attr removeAttributeNode(Attr oldAttr)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// <summary>
	  /// Introduced in DOM Level 2.
	  /// 
	  /// 
	  /// </summary>
	  public virtual bool hasAttributes()
	  {
		return org.apache.xml.dtm.DTM_Fields.NULL != dtm.getFirstAttribute(node);
	  }

	  /// <seealso cref= org.w3c.dom.Element </seealso>
	  public void normalize()
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="localName">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
	  public string getAttributeNS(string namespaceURI, string localName)
	  {
		   Node retNode = null;
		   int n = dtm.getAttributeNode(node,namespaceURI,localName);
		   if (n != org.apache.xml.dtm.DTM_Fields.NULL)
		   {
				   retNode = dtm.getNode(n);
		   }
		   return (null == retNode) ? EMPTYSTRING : retNode.NodeValue;
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="qualifiedName"> </param>
	  /// <param name="value">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void setAttributeNS(String namespaceURI, String qualifiedName, String value) throws org.w3c.dom.DOMException
	  public void setAttributeNS(string namespaceURI, string qualifiedName, string value)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="localName">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void removeAttributeNS(String namespaceURI, String localName) throws org.w3c.dom.DOMException
	  public void removeAttributeNS(string namespaceURI, string localName)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// <param name="namespaceURI"> </param>
	  /// <param name="localName">
	  /// 
	  /// </param>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
	  public Attr getAttributeNodeNS(string namespaceURI, string localName)
	  {
		   Attr retAttr = null;
		   int n = dtm.getAttributeNode(node,namespaceURI,localName);
		   if (n != org.apache.xml.dtm.DTM_Fields.NULL)
		   {
				   retAttr = (Attr) dtm.getNode(n);
		   }
		   return retAttr;
	  }

	  /// 
	  /// <param name="newAttr">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
	  /// <seealso cref= org.w3c.dom.Element </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.w3c.dom.Attr setAttributeNodeNS(org.w3c.dom.Attr newAttr) throws org.w3c.dom.DOMException
	  public Attr setAttributeNodeNS(Attr newAttr)
	  {
		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Attr </seealso>
	  public string Name
	  {
		  get
		  {
			return dtm.getNodeName(node);
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Attr </seealso>
	  public bool Specified
	  {
		  get
		  {
			// We really don't know which attributes might have come from the
			// source document versus from the DTD. Treat them all as having
			// been provided by the user.
			// %REVIEW% if/when we become aware of DTDs/schemae.
			return true;
		  }
	  }

	  /// 
	  /// 
	  /// <seealso cref= org.w3c.dom.Attr </seealso>
	  public string Value
	  {
		  get
		  {
			return dtm.getNodeValue(node);
		  }
		  set
		  {
			throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
	  }


	  /// <summary>
	  /// Get the owner element of an attribute.
	  /// 
	  /// </summary>
	  /// <seealso cref= org.w3c.dom.Attr as of DOM Level 2 </seealso>
	  public Element OwnerElement
	  {
		  get
		  {
			if (NodeType != Node.ATTRIBUTE_NODE)
			{
			  return null;
			}
			// In XPath and DTM data models, unlike DOM, an Attr's parent is its
			// owner element.
			int newnode = dtm.getParent(node);
			return (newnode == org.apache.xml.dtm.DTM_Fields.NULL) ? null : (Element)(dtm.getNode(newnode));
		  }
	  }

	  /// <summary>
	  /// NEEDSDOC Method adoptNode 
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="source">
	  /// 
	  /// 
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node adoptNode(org.w3c.dom.Node source) throws org.w3c.dom.DOMException
	  public virtual Node adoptNode(Node source)
	  {

		throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
	  }

	  /// <summary>
	  /// <para>Based on the <a
	  /// href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407'>Document
	  /// Object Model (DOM) Level 3 Core Specification of 07 April 2004.</a>.
	  /// </para>
	  /// <para>
	  /// An attribute specifying, as part of the XML declaration, the encoding
	  /// of this document. This is <code>null</code> when unspecified.
	  /// @since DOM Level 3
	  /// 
	  /// 
	  /// </para>
	  /// </summary>
	  public virtual string InputEncoding
	  {
		  get
		  {
    
			throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
	  }

	  /// <summary>
	  /// <para>Based on the <a
	  /// href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407'>Document
	  /// Object Model (DOM) Level 3 Core Specification of 07 April 2004.</a>.
	  /// </para>
	  /// <para>
	  /// An attribute specifying whether errors checking is enforced or not.
	  /// When set to <code>false</code>, the implementation is free to not
	  /// test every possible error case normally defined on DOM operations,
	  /// and not raise any <code>DOMException</code>. In case of error, the
	  /// behavior is undefined. This attribute is <code>true</code> by
	  /// defaults.
	  /// @since DOM Level 3
	  /// 
	  /// 
	  /// </para>
	  /// </summary>
	  public virtual bool StrictErrorChecking
	  {
		  get
		  {
    
			throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
		  set
		  {
			throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
	  }


	  /// <summary>
	  /// Inner class to support getDOMImplementation.
	  /// </summary>
	  internal class DTMNodeProxyImplementation : DOMImplementation
	  {
		public virtual DocumentType createDocumentType(string qualifiedName, string publicId, string systemId)
		{
		  throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		}
		public virtual Document createDocument(string namespaceURI, string qualfiedName, DocumentType doctype)
		{
		  // Could create a DTM... but why, when it'd have to be permanantly empty?
		  throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		}
		/// <summary>
		/// Ask whether we support a given DOM feature.
		/// 
		/// In fact, we do not _fully_ support any DOM feature -- we're a
		/// read-only subset -- so arguably we should always return false.
		/// On the other hand, it may be more practically useful to return
		/// true and simply treat the whole DOM as read-only, failing on the
		/// methods we can't support. I'm not sure which would be more useful
		/// to the caller.
		/// </summary>
		public virtual bool hasFeature(string feature, string version)
		{
		  if (("CORE".Equals(feature.ToUpper()) || "XML".Equals(feature.ToUpper())) && ("1.0".Equals(version) || "2.0".Equals(version)))
		  {
			return true;
		  }
		  return false;
		}

		/// <summary>
		///  This method returns a specialized object which implements the
		/// specialized APIs of the specified feature and version. The
		/// specialized object may also be obtained by using binding-specific
		/// casting methods but is not necessarily expected to, as discussed in Mixed DOM implementations
		/// . </summary>
		/// <param name="feature"> The name of the feature requested (case-insensitive). </param>
		/// <param name="version">  This is the version number of the feature to test. If
		///   the version is <code>null</code> or the empty string, supporting
		///   any version of the feature will cause the method to return an
		///   object that supports at least one version of the feature. </param>
		/// <returns>  Returns an object which implements the specialized APIs of
		///   the specified feature and version, if any, or <code>null</code> if
		///   there is no object which implements interfaces associated with that
		///   feature. If the <code>DOMObject</code> returned by this method
		///   implements the <code>Node</code> interface, it must delegate to the
		///   primary core <code>Node</code> and not return results inconsistent
		///   with the primary core <code>Node</code> such as attributes,
		///   childNodes, etc.
		/// @since DOM Level 3 </returns>
		public virtual object getFeature(string feature, string version)
		{
			// we don't have any alternate node, either this node does the job
			// or we don't have anything that does
			//return hasFeature(feature, version) ? this : null;
			return null; //PENDING
		}

	  }


		//RAMESH : Pending proper implementation of DOM Level 3

		public virtual object setUserData(string key, object data, UserDataHandler handler)
		{
			return OwnerDocument.setUserData(key, data, handler);
		}

		/// <summary>
		/// Retrieves the object associated to a key on a this node. The object
		/// must first have been set to this node by calling
		/// <code>setUserData</code> with the same key. </summary>
		/// <param name="key"> The key the object is associated to. </param>
		/// <returns> Returns the <code>DOMObject</code> associated to the given key
		///   on this node, or <code>null</code> if there was none.
		/// @since DOM Level 3 </returns>
		public virtual object getUserData(string key)
		{
			return OwnerDocument.getUserData(key);
		}

		/// <summary>
		///  This method returns a specialized object which implements the
		/// specialized APIs of the specified feature and version. The
		/// specialized object may also be obtained by using binding-specific
		/// casting methods but is not necessarily expected to, as discussed in Mixed DOM implementations. </summary>
		/// <param name="feature"> The name of the feature requested (case-insensitive). </param>
		/// <param name="version">  This is the version number of the feature to test. If
		///   the version is <code>null</code> or the empty string, supporting
		///   any version of the feature will cause the method to return an
		///   object that supports at least one version of the feature. </param>
		/// <returns>  Returns an object which implements the specialized APIs of
		///   the specified feature and version, if any, or <code>null</code> if
		///   there is no object which implements interfaces associated with that
		///   feature. If the <code>DOMObject</code> returned by this method
		///   implements the <code>Node</code> interface, it must delegate to the
		///   primary core <code>Node</code> and not return results inconsistent
		///   with the primary core <code>Node</code> such as attributes,
		///   childNodes, etc.
		/// @since DOM Level 3 </returns>
		public virtual object getFeature(string feature, string version)
		{
			// we don't have any alternate node, either this node does the job
			// or we don't have anything that does
			return isSupported(feature, version) ? this : null;
		}

		/// <summary>
		/// Tests whether two nodes are equal.
		/// <br>This method tests for equality of nodes, not sameness (i.e.,
		/// whether the two nodes are references to the same object) which can be
		/// tested with <code>Node.isSameNode</code>. All nodes that are the same
		/// will also be equal, though the reverse may not be true.
		/// <br>Two nodes are equal if and only if the following conditions are
		/// satisfied: The two nodes are of the same type.The following string
		/// attributes are equal: <code>nodeName</code>, <code>localName</code>,
		/// <code>namespaceURI</code>, <code>prefix</code>, <code>nodeValue</code>
		/// , <code>baseURI</code>. This is: they are both <code>null</code>, or
		/// they have the same length and are character for character identical.
		/// The <code>attributes</code> <code>NamedNodeMaps</code> are equal.
		/// This is: they are both <code>null</code>, or they have the same
		/// length and for each node that exists in one map there is a node that
		/// exists in the other map and is equal, although not necessarily at the
		/// same index.The <code>childNodes</code> <code>NodeLists</code> are
		/// equal. This is: they are both <code>null</code>, or they have the
		/// same length and contain equal nodes at the same index. This is true
		/// for <code>Attr</code> nodes as for any other type of node. Note that
		/// normalization can affect equality; to avoid this, nodes should be
		/// normalized before being compared.
		/// <br>For two <code>DocumentType</code> nodes to be equal, the following
		/// conditions must also be satisfied: The following string attributes
		/// are equal: <code>publicId</code>, <code>systemId</code>,
		/// <code>internalSubset</code>.The <code>entities</code>
		/// <code>NamedNodeMaps</code> are equal.The <code>notations</code>
		/// <code>NamedNodeMaps</code> are equal.
		/// <br>On the other hand, the following do not affect equality: the
		/// <code>ownerDocument</code> attribute, the <code>specified</code>
		/// attribute for <code>Attr</code> nodes, the
		/// <code>isWhitespaceInElementContent</code> attribute for
		/// <code>Text</code> nodes, as well as any user data or event listeners
		/// registered on the nodes. </summary>
		/// <param name="arg"> The node to compare equality with. </param>
		/// <param name="deep"> If <code>true</code>, recursively compare the subtrees; if
		///   <code>false</code>, compare only the nodes themselves (and its
		///   attributes, if it is an <code>Element</code>). </param>
		/// <returns> If the nodes, and possibly subtrees are equal,
		///   <code>true</code> otherwise <code>false</code>.
		/// @since DOM Level 3 </returns>
		public virtual bool isEqualNode(Node arg)
		{
			if (arg == this)
			{
				return true;
			}
			if (arg.NodeType != NodeType)
			{
				return false;
			}
			// in theory nodeName can't be null but better be careful
			// who knows what other implementations may be doing?...
			if (string.ReferenceEquals(NodeName, null))
			{
				if (arg.NodeName != null)
				{
					return false;
				}
			}
			else if (!NodeName.Equals(arg.NodeName))
			{
				return false;
			}

			if (string.ReferenceEquals(LocalName, null))
			{
				if (arg.LocalName != null)
				{
					return false;
				}
			}
			else if (!LocalName.Equals(arg.LocalName))
			{
				return false;
			}

			if (string.ReferenceEquals(NamespaceURI, null))
			{
				if (arg.NamespaceURI != null)
				{
					return false;
				}
			}
			else if (!NamespaceURI.Equals(arg.NamespaceURI))
			{
				return false;
			}

			if (string.ReferenceEquals(Prefix, null))
			{
				if (arg.Prefix != null)
				{
					return false;
				}
			}
			else if (!Prefix.Equals(arg.Prefix))
			{
				return false;
			}

			if (string.ReferenceEquals(NodeValue, null))
			{
				if (arg.NodeValue != null)
				{
					return false;
				}
			}
			else if (!NodeValue.Equals(arg.NodeValue))
			{
				return false;
			}
		/*
		    if (getBaseURI() == null) {
		        if (((NodeImpl) arg).getBaseURI() != null) {
		            return false;
		        }
		    }
		    else if (!getBaseURI().equals(((NodeImpl) arg).getBaseURI())) {
		        return false;
		    }
	*/
			return true;
		}

		/// <summary>
		/// DOM Level 3:
		/// Look up the namespace URI associated to the given prefix, starting from this node.
		/// Use lookupNamespaceURI(null) to lookup the default namespace
		/// </summary>
		/// <param name="namespaceURI"> </param>
		/// <returns> th URI for the namespace
		/// @since DOM Level 3 </returns>
		public virtual string lookupNamespaceURI(string specifiedPrefix)
		{
			short type = this.NodeType;
			switch (type)
			{
			case Node.ELEMENT_NODE :
			{

					string @namespace = this.NamespaceURI;
					string prefix = this.Prefix;
					if (!string.ReferenceEquals(@namespace, null))
					{
						// REVISIT: is it possible that prefix is empty string?
						if (string.ReferenceEquals(specifiedPrefix, null) && string.ReferenceEquals(prefix, specifiedPrefix))
						{
							// looking for default namespace
							return @namespace;
						}
						else if (!string.ReferenceEquals(prefix, null) && prefix.Equals(specifiedPrefix))
						{
							// non default namespace
							return @namespace;
						}
					}
					if (this.hasAttributes())
					{
						NamedNodeMap map = this.Attributes;
						int length = map.Length;
						for (int i = 0;i < length;i++)
						{
							Node attr = map.item(i);
							string attrPrefix = attr.Prefix;
							string value = attr.NodeValue;
							@namespace = attr.NamespaceURI;
							if (!string.ReferenceEquals(@namespace, null) && @namespace.Equals("http://www.w3.org/2000/xmlns/"))
							{
								// at this point we are dealing with DOM Level 2 nodes only
								if (string.ReferenceEquals(specifiedPrefix, null) && attr.NodeName.Equals("xmlns"))
								{
									// default namespace
									return value;
								}
								else if (!string.ReferenceEquals(attrPrefix, null) && attrPrefix.Equals("xmlns") && attr.LocalName.Equals(specifiedPrefix))
								{
					 // non default namespace
									return value;
								}
							}
						}
					}
			/*
					NodeImpl ancestor = (NodeImpl)getElementAncestor(this);
					if (ancestor != null) {
						return ancestor.lookupNamespaceURI(specifiedPrefix);
					}
			*/
					return null;
			}
	/*
	        case Node.DOCUMENT_NODE : {
	                return((NodeImpl)((Document)this).getDocumentElement()).lookupNamespaceURI(specifiedPrefix) ;
	            }
	*/
			case Node.ENTITY_NODE :
			case Node.NOTATION_NODE:
			case Node.DOCUMENT_FRAGMENT_NODE:
			case Node.DOCUMENT_TYPE_NODE:
				// type is unknown
				return null;
			case Node.ATTRIBUTE_NODE:
			{
					if (this.OwnerElement.NodeType == Node.ELEMENT_NODE)
					{
						return OwnerElement.lookupNamespaceURI(specifiedPrefix);
					}
					return null;
			}
			default:
			{
		   /*
					NodeImpl ancestor = (NodeImpl)getElementAncestor(this);
					if (ancestor != null) {
						return ancestor.lookupNamespaceURI(specifiedPrefix);
					}
				 */
					return null;
			}

			}
		}

		/// <summary>
		///  DOM Level 3:
		///  This method checks if the specified <code>namespaceURI</code> is the
		///  default namespace or not. </summary>
		///  <param name="namespaceURI"> The namespace URI to look for. </param>
		///  <returns>  <code>true</code> if the specified <code>namespaceURI</code>
		///   is the default namespace, <code>false</code> otherwise.
		/// @since DOM Level 3 </returns>
		public virtual bool isDefaultNamespace(string namespaceURI)
		{
		   /*
		    // REVISIT: remove casts when DOM L3 becomes REC.
		    short type = this.getNodeType();
		    switch (type) {
		    case Node.ELEMENT_NODE: {
		        String namespace = this.getNamespaceURI();
		        String prefix = this.getPrefix();
	
		        // REVISIT: is it possible that prefix is empty string?
		        if (prefix == null || prefix.length() == 0) {
		            if (namespaceURI == null) {
		                return (namespace == namespaceURI);
		            }
		            return namespaceURI.equals(namespace);
		        }
		        if (this.hasAttributes()) {
		            ElementImpl elem = (ElementImpl)this;
		            NodeImpl attr = (NodeImpl)elem.getAttributeNodeNS("http://www.w3.org/2000/xmlns/", "xmlns");
		            if (attr != null) {
		                String value = attr.getNodeValue();
		                if (namespaceURI == null) {
		                    return (namespace == value);
		                }
		                return namespaceURI.equals(value);
		            }
		        }
	
		        NodeImpl ancestor = (NodeImpl)getElementAncestor(this);
		        if (ancestor != null) {
		            return ancestor.isDefaultNamespace(namespaceURI);
		        }
		        return false;
		    }
		    case Node.DOCUMENT_NODE:{
		            return((NodeImpl)((Document)this).getDocumentElement()).isDefaultNamespace(namespaceURI);
		        }
	
		    case Node.ENTITY_NODE :
		      case Node.NOTATION_NODE:
		    case Node.DOCUMENT_FRAGMENT_NODE:
		    case Node.DOCUMENT_TYPE_NODE:
		        // type is unknown
		        return false;
		    case Node.ATTRIBUTE_NODE:{
		            if (this.ownerNode.getNodeType() == Node.ELEMENT_NODE) {
		                return ownerNode.isDefaultNamespace(namespaceURI);
	
		            }
		            return false;
		        }
		    default:{  
		            NodeImpl ancestor = (NodeImpl)getElementAncestor(this);
		            if (ancestor != null) {
		                return ancestor.isDefaultNamespace(namespaceURI);
		            }
		            return false;
		        }
	
		    }
	*/
			return false;
		}

		/// <summary>
		/// DOM Level 3:
		/// Look up the prefix associated to the given namespace URI, starting from this node.
		/// </summary>
		/// <param name="namespaceURI"> </param>
		/// <returns> the prefix for the namespace </returns>
		public virtual string lookupPrefix(string namespaceURI)
		{

			// REVISIT: When Namespaces 1.1 comes out this may not be true
			// Prefix can't be bound to null namespace
			if (string.ReferenceEquals(namespaceURI, null))
			{
				return null;
			}

			short type = this.NodeType;

			switch (type)
			{
	/*
	        case Node.ELEMENT_NODE: {
	
	                String namespace = this.getNamespaceURI(); // to flip out children
	                return lookupNamespacePrefix(namespaceURI, (ElementImpl)this);
	            }
	
	        case Node.DOCUMENT_NODE:{
	                return((NodeImpl)((Document)this).getDocumentElement()).lookupPrefix(namespaceURI);
	            }
	*/
			case Node.ENTITY_NODE :
			case Node.NOTATION_NODE:
			case Node.DOCUMENT_FRAGMENT_NODE:
			case Node.DOCUMENT_TYPE_NODE:
				// type is unknown
				return null;
			case Node.ATTRIBUTE_NODE:
			{
					if (this.OwnerElement.NodeType == Node.ELEMENT_NODE)
					{
						return OwnerElement.lookupPrefix(namespaceURI);

					}
					return null;
			}
			default:
			{
	/*
	                NodeImpl ancestor = (NodeImpl)getElementAncestor(this);
	                if (ancestor != null) {
	                    return ancestor.lookupPrefix(namespaceURI);
	                }
	*/
					return null;
			}
			}
		}

		/// <summary>
		/// Returns whether this node is the same node as the given one.
		/// <br>This method provides a way to determine whether two
		/// <code>Node</code> references returned by the implementation reference
		/// the same object. When two <code>Node</code> references are references
		/// to the same object, even if through a proxy, the references may be
		/// used completely interchangably, such that all attributes have the
		/// same values and calling the same DOM method on either reference
		/// always has exactly the same effect. </summary>
		/// <param name="other"> The node to test against. </param>
		/// <returns> Returns <code>true</code> if the nodes are the same,
		///   <code>false</code> otherwise.
		/// @since DOM Level 3 </returns>
		public virtual bool isSameNode(Node other)
		{
			// we do not use any wrapper so the answer is obvious
			return this == other;
		}

		/// <summary>
		/// This attribute returns the text content of this node and its
		/// descendants. When it is defined to be null, setting it has no effect.
		/// When set, any possible children this node may have are removed and
		/// replaced by a single <code>Text</code> node containing the string
		/// this attribute is set to. On getting, no serialization is performed,
		/// the returned string does not contain any markup. No whitespace
		/// normalization is performed, the returned string does not contain the
		/// element content whitespaces . Similarly, on setting, no parsing is
		/// performed either, the input string is taken as pure textual content.
		/// <br>The string returned is made of the text content of this node
		/// depending on its type, as defined below:
		/// <table border='1'>
		/// <tr>
		/// <th>Node type</th>
		/// <th>Content</th>
		/// </tr>
		/// <tr>
		/// <td valign='top' rowspan='1' colspan='1'>
		/// ELEMENT_NODE, ENTITY_NODE, ENTITY_REFERENCE_NODE,
		/// DOCUMENT_FRAGMENT_NODE</td>
		/// <td valign='top' rowspan='1' colspan='1'>concatenation of the <code>textContent</code>
		/// attribute value of every child node, excluding COMMENT_NODE and
		/// PROCESSING_INSTRUCTION_NODE nodes</td>
		/// </tr>
		/// <tr>
		/// <td valign='top' rowspan='1' colspan='1'>ATTRIBUTE_NODE, TEXT_NODE,
		/// CDATA_SECTION_NODE, COMMENT_NODE, PROCESSING_INSTRUCTION_NODE</td>
		/// <td valign='top' rowspan='1' colspan='1'>
		/// <code>nodeValue</code></td>
		/// </tr>
		/// <tr>
		/// <td valign='top' rowspan='1' colspan='1'>DOCUMENT_NODE, DOCUMENT_TYPE_NODE, NOTATION_NODE</td>
		/// <td valign='top' rowspan='1' colspan='1'>
		/// null</td>
		/// </tr>
		/// </table> </summary>
		/// <exception cref="DOMException">
		///   NO_MODIFICATION_ALLOWED_ERR: Raised when the node is readonly. </exception>
		/// <exception cref="DOMException">
		///   DOMSTRING_SIZE_ERR: Raised when it would return more characters than
		///   fit in a <code>DOMString</code> variable on the implementation
		///   platform.
		/// @since DOM Level 3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setTextContent(String textContent) throws org.w3c.dom.DOMException
		public virtual string TextContent
		{
			set
			{
				NodeValue = value;
			}
			get
			{
				return dtm.getStringValue(node).ToString();
			}
		}


		/// <summary>
		/// Compares a node with this node with regard to their position in the
		/// document. </summary>
		/// <param name="other"> The node to compare against this node. </param>
		/// <returns> Returns how the given node is positioned relatively to this
		///   node.
		/// @since DOM Level 3 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short compareDocumentPosition(org.w3c.dom.Node other) throws org.w3c.dom.DOMException
		public virtual short compareDocumentPosition(Node other)
		{
			return 0;
		}

		/// <summary>
		/// The absolute base URI of this node or <code>null</code> if undefined.
		/// This value is computed according to . However, when the
		/// <code>Document</code> supports the feature "HTML" , the base URI is
		/// computed using first the value of the href attribute of the HTML BASE
		/// element if any, and the value of the <code>documentURI</code>
		/// attribute from the <code>Document</code> interface otherwise.
		/// <br> When the node is an <code>Element</code>, a <code>Document</code>
		/// or a a <code>ProcessingInstruction</code>, this attribute represents
		/// the properties [base URI] defined in . When the node is a
		/// <code>Notation</code>, an <code>Entity</code>, or an
		/// <code>EntityReference</code>, this attribute represents the
		/// properties [declaration base URI] in the . How will this be affected
		/// by resolution of relative namespace URIs issue?It's not.Should this
		/// only be on Document, Element, ProcessingInstruction, Entity, and
		/// Notation nodes, according to the infoset? If not, what is it equal to
		/// on other nodes? Null? An empty string? I think it should be the
		/// parent's.No.Should this be read-only and computed or and actual
		/// read-write attribute?Read-only and computed (F2F 19 Jun 2000 and
		/// teleconference 30 May 2001).If the base HTML element is not yet
		/// attached to a document, does the insert change the Document.baseURI?
		/// Yes. (F2F 26 Sep 2001)
		/// @since DOM Level 3
		/// </summary>
		public virtual string BaseURI
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// DOM Level 3
		/// Renaming node
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node renameNode(org.w3c.dom.Node n, String namespaceURI, String name) throws org.w3c.dom.DOMException
		public virtual Node renameNode(Node n, string namespaceURI, string name)
		{
			return n;
		}

		/// <summary>
		///  DOM Level 3
		///  Normalize document.
		/// </summary>
		public virtual void normalizeDocument()
		{

		}

		/// <summary>
		///  The configuration used when <code>Document.normalizeDocument</code> is
		/// invoked.
		/// @since DOM Level 3
		/// </summary>
		public virtual DOMConfiguration DomConfig
		{
			get
			{
			   return null;
			}
		}

		/// <summary>
		/// DOM Level 3 feature: documentURI </summary>
		protected internal string fDocumentURI;

		/// <summary>
		/// DOM Level 3
		/// </summary>
		public virtual string DocumentURI
		{
			set
			{
    
				fDocumentURI = value;
			}
			get
			{
				return fDocumentURI;
			}
		}


		/// <summary>
		/// DOM Level 3 feature: Document actualEncoding </summary>
		protected internal string actualEncoding;

		/// <summary>
		/// DOM Level 3
		/// An attribute specifying the actual encoding of this document. This is
		/// <code>null</code> otherwise.
		/// <br> This attribute represents the property [character encoding scheme]
		/// defined in .
		/// @since DOM Level 3
		/// </summary>
		public virtual string ActualEncoding
		{
			get
			{
				return actualEncoding;
			}
			set
			{
				actualEncoding = value;
			}
		}


	   /// <summary>
	   /// DOM Level 3
	   /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Text replaceWholeText(String content) throws org.w3c.dom.DOMException
		public virtual Text replaceWholeText(string content)
		{
	/*
	
	        if (needsSyncData()) {
	            synchronizeData();
	        }
	
	        // make sure we can make the replacement
	        if (!canModify(nextSibling)) {
	            throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR,
	                DOMMessageFormatter.formatMessage(DOMMessageFormatter.DOM_DOMAIN, "NO_MODIFICATION_ALLOWED_ERR", null));
	        }
	
	        Node parent = this.getParentNode();
	        if (content == null || content.length() == 0) {
	            // remove current node
	            if (parent !=null) { // check if node in the tree
	                parent.removeChild(this);
	                return null;
	            }
	        }
	        Text currentNode = null;
	        if (isReadOnly()){
	            Text newNode = this.ownerDocument().createTextNode(content);
	            if (parent !=null) { // check if node in the tree
	                parent.insertBefore(newNode, this);
	                parent.removeChild(this);
	                currentNode = newNode;
	            } else {
	                return newNode;
	            }
	        }  else {
	            this.setData(content);
	            currentNode = this;
	        }
	        Node sibling =  currentNode.getNextSibling();
	        while ( sibling !=null) {
	            parent.removeChild(sibling);
	            sibling = currentNode.getNextSibling();
	        }
	
	        return currentNode;
	*/
			return null; //Pending
		}

		/// <summary>
		/// DOM Level 3
		/// Returns all text of <code>Text</code> nodes logically-adjacent text
		/// nodes to this node, concatenated in document order.
		/// @since DOM Level 3
		/// </summary>
		public virtual string WholeText
		{
			get
			{
    
		/*
		        if (needsSyncData()) {
		            synchronizeData();
		        }
		        if (nextSibling == null) {
		            return data;
		        }
		        StringBuffer buffer = new StringBuffer();
		        if (data != null && data.length() != 0) {
		            buffer.append(data);
		        }
		        getWholeText(nextSibling, buffer);
		        return buffer.toString();
		*/
				return null; // PENDING
			}
		}

		/// <summary>
		/// DOM Level 3
		/// Returns whether this text node contains whitespace in element content,
		/// often abusively called "ignorable whitespace".
		/// </summary>
		public virtual bool ElementContentWhitespace
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// NON-DOM: set the type of this attribute to be ID type.
		/// </summary>
		/// <param name="id"> </param>
		public virtual bool IdAttribute
		{
			set
			{
				//PENDING
			}
		}

		/// <summary>
		/// DOM Level 3: register the given attribute node as an ID attribute
		/// </summary>
		public virtual void setIdAttribute(string name, bool makeId)
		{
			//PENDING
		}


		/// <summary>
		/// DOM Level 3: register the given attribute node as an ID attribute
		/// </summary>
		public virtual void setIdAttributeNode(Attr at, bool makeId)
		{
			//PENDING
		}

		/// <summary>
		/// DOM Level 3: register the given attribute node as an ID attribute
		/// </summary>
		public virtual void setIdAttributeNS(string namespaceURI, string localName, bool makeId)
		{
			//PENDING
		}

		public virtual TypeInfo SchemaTypeInfo
		{
			get
			{
			  return null; //PENDING
			}
		}

		public virtual bool Id
		{
			get
			{
				return false; //PENDING
			}
		}


		private string xmlEncoding;

		public virtual string XmlEncoding
		{
			get
			{
				return xmlEncoding;
			}
			set
			{
				this.xmlEncoding = value;
			}
		}


		private bool xmlStandalone;

		public virtual bool XmlStandalone
		{
			get
			{
				return xmlStandalone;
			}
			set
			{
				this.xmlStandalone = value;
			}
		}


		private string xmlVersion;

		public virtual string XmlVersion
		{
			get
			{
				return xmlVersion;
			}
			set
			{
				this.xmlVersion = value;
			}
		}

	}

}