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
 * $Id: XPathNamespaceImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */


namespace org.apache.xpath.domapi
{
	using Attr = org.w3c.dom.Attr;
	using DOMException = org.w3c.dom.DOMException;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using XPathNamespace = org.w3c.dom.xpath.XPathNamespace;

	using UserDataHandler = org.w3c.dom.UserDataHandler;

	/// 
	/// 
	/// <summary>
	/// The <code>XPathNamespace</code> interface is returned by 
	/// <code>XPathResult</code> interfaces to represent the XPath namespace node 
	/// type that DOM lacks. There is no public constructor for this node type. 
	/// Attempts to place it into a hierarchy or a NamedNodeMap result in a 
	/// <code>DOMException</code> with the code <code>HIERARCHY_REQUEST_ERR</code>
	/// . This node is read only, so methods or setting of attributes that would 
	/// mutate the node result in a DOMException with the code 
	/// <code>NO_MODIFICATION_ALLOWED_ERR</code>.
	/// <para>The core specification describes attributes of the <code>Node</code> 
	/// interface that are different for different node node types but does not 
	/// describe <code>XPATH_NAMESPACE_NODE</code>, so here is a description of 
	/// those attributes for this node type. All attributes of <code>Node</code> 
	/// not described in this section have a <code>null</code> or 
	/// <code>false</code> value.
	/// </para>
	/// <para><code>ownerDocument</code> matches the <code>ownerDocument</code> of the 
	/// <code>ownerElement</code> even if the element is later adopted.
	/// </para>
	/// <para><code>prefix</code> is the prefix of the namespace represented by the 
	/// node.
	/// </para>
	/// <para><code>nodeName</code> is the same as <code>prefix</code>.
	/// </para>
	/// <para><code>nodeType</code> is equal to <code>XPATH_NAMESPACE_NODE</code>.
	/// </para>
	/// <para><code>namespaceURI</code> is the namespace URI of the namespace 
	/// represented by the node.
	/// </para>
	/// <para><code>adoptNode</code>, <code>cloneNode</code>, and 
	/// <code>importNode</code> fail on this node type by raising a 
	/// <code>DOMException</code> with the code <code>NOT_SUPPORTED_ERR</code>.In 
	/// future versions of the XPath specification, the definition of a namespace 
	/// node may be changed incomatibly, in which case incompatible changes to 
	/// field values may be required to implement versions beyond XPath 1.0.
	/// </para>
	/// <para>See also the <a href='http://www.w3.org/TR/2004/NOTE-DOM-Level-3-XPath-20040226'>Document Object Model (DOM) Level 3 XPath Specification</a>.
	/// 
	/// This implementation wraps the DOM attribute node that contained the 
	/// namespace declaration.
	/// @xsl.usage internal
	/// </para>
	/// </summary>

	internal class XPathNamespaceImpl : XPathNamespace
	{

		// Node that XPathNamespaceImpl wraps
		private readonly Node m_attributeNode;

		/// <summary>
		/// Constructor for XPathNamespaceImpl.
		/// </summary>
		internal XPathNamespaceImpl(Node node)
		{
			m_attributeNode = node;
		}

		/// <seealso cref="org.apache.xalan.dom3.xpath.XPathNamespace.getOwnerElement()"/>
		public virtual Element OwnerElement
		{
			get
			{
				return ((Attr)m_attributeNode).getOwnerElement();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getNodeName()"/>
		public virtual string NodeName
		{
			get
			{
				return "#namespace";
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getNodeValue()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getNodeValue() throws org.w3c.dom.DOMException
		public virtual string NodeValue
		{
			get
			{
				return m_attributeNode.getNodeValue();
			}
			set
			{
			}
		}


		/// <seealso cref="org.w3c.dom.Node.getNodeType()"/>
		public virtual short NodeType
		{
			get
			{
				return XPathNamespace.XPATH_NAMESPACE_NODE;
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getParentNode()"/>
		public virtual Node ParentNode
		{
			get
			{
				return m_attributeNode.getParentNode();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getChildNodes()"/>
		public virtual NodeList ChildNodes
		{
			get
			{
				return m_attributeNode.getChildNodes();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getFirstChild()"/>
		public virtual Node FirstChild
		{
			get
			{
				return m_attributeNode.getFirstChild();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getLastChild()"/>
		public virtual Node LastChild
		{
			get
			{
				return m_attributeNode.getLastChild();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getPreviousSibling()"/>
		public virtual Node PreviousSibling
		{
			get
			{
				return m_attributeNode.getPreviousSibling();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getNextSibling()"/>
		public virtual Node NextSibling
		{
			get
			{
				return m_attributeNode.getNextSibling();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getAttributes()"/>
		public virtual NamedNodeMap Attributes
		{
			get
			{
				return m_attributeNode.getAttributes();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getOwnerDocument()"/>
		public virtual Document OwnerDocument
		{
			get
			{
				return m_attributeNode.getOwnerDocument();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.insertBefore(Node, Node)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node insertBefore(org.w3c.dom.Node arg0, org.w3c.dom.Node arg1) throws org.w3c.dom.DOMException
		public virtual Node insertBefore(Node arg0, Node arg1)
		{
			return null;
		}

		/// <seealso cref="org.w3c.dom.Node.replaceChild(Node, Node)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node replaceChild(org.w3c.dom.Node arg0, org.w3c.dom.Node arg1) throws org.w3c.dom.DOMException
		public virtual Node replaceChild(Node arg0, Node arg1)
		{
			return null;
		}

		/// <seealso cref="org.w3c.dom.Node.removeChild(Node)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node removeChild(org.w3c.dom.Node arg0) throws org.w3c.dom.DOMException
		public virtual Node removeChild(Node arg0)
		{
			return null;
		}

		/// <seealso cref="org.w3c.dom.Node.appendChild(Node)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node appendChild(org.w3c.dom.Node arg0) throws org.w3c.dom.DOMException
		public virtual Node appendChild(Node arg0)
		{
			return null;
		}

		/// <seealso cref="org.w3c.dom.Node.hasChildNodes()"/>
		public virtual bool hasChildNodes()
		{
			return false;
		}

		/// <seealso cref="org.w3c.dom.Node.cloneNode(boolean)"/>
		public virtual Node cloneNode(bool arg0)
		{
			throw new DOMException(DOMException.NOT_SUPPORTED_ERR,null);
		}

		/// <seealso cref="org.w3c.dom.Node.normalize()"/>
		public virtual void normalize()
		{
			m_attributeNode.normalize();
		}

		/// <seealso cref="org.w3c.dom.Node.isSupported(String, String)"/>
		public virtual bool isSupported(string arg0, string arg1)
		{
			return m_attributeNode.isSupported(arg0, arg1);
		}

		/// <seealso cref="org.w3c.dom.Node.getNamespaceURI()"/>
		public virtual string NamespaceURI
		{
			get
			{
    
				// For namespace node, the namespaceURI is the namespace URI
				// of the namespace represented by the node.
				return m_attributeNode.getNodeValue();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.getPrefix()"/>
		public virtual string Prefix
		{
			get
			{
				return m_attributeNode.getPrefix();
			}
			set
			{
			}
		}


		/// <seealso cref="org.w3c.dom.Node.getLocalName()"/>
		public virtual string LocalName
		{
			get
			{
    
				// For namespace node, the local name is the same as the prefix
				return m_attributeNode.getPrefix();
			}
		}

		/// <seealso cref="org.w3c.dom.Node.hasAttributes()"/>
		public virtual bool hasAttributes()
		{
			return m_attributeNode.hasAttributes();
		}

		public virtual string BaseURI
		{
			get
			{
				return null;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public short compareDocumentPosition(org.w3c.dom.Node other) throws org.w3c.dom.DOMException
		public virtual short compareDocumentPosition(Node other)
		{
			return 0;
		}

		private string textContent;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getTextContent() throws org.w3c.dom.DOMException
		public virtual string TextContent
		{
			get
			{
				return textContent;
			}
			set
			{
				this.textContent = value;
			}
		}


		public virtual bool isSameNode(Node other)
		{
			return false;
		}

		public virtual string lookupPrefix(string namespaceURI)
		{
			return ""; //PENDING
		}

		public virtual bool isDefaultNamespace(string namespaceURI)
		{
			return false;
		}

		public virtual string lookupNamespaceURI(string prefix)
		{
			return null;
		}

		public virtual bool isEqualNode(Node arg)
		{
			return false;
		}

		public virtual object getFeature(string feature, string version)
		{
			return null; //PENDING
		}

		public virtual object setUserData(string key, object data, UserDataHandler handler)
		{
			return null; //PENDING
		}

		public virtual object getUserData(string key)
		{
			return null;
		}
	}

}