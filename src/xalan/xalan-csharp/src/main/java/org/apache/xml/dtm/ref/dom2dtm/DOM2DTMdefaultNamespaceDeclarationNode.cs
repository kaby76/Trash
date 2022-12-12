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
 * $Id: DOM2DTMdefaultNamespaceDeclarationNode.java 1225427 2011-12-29 04:33:32Z mrglavas $
 */

namespace org.apache.xml.dtm.@ref.dom2dtm
{
	using DTMException = org.apache.xml.dtm.DTMException;

	using Attr = org.w3c.dom.Attr;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using TypeInfo = org.w3c.dom.TypeInfo;
	using UserDataHandler = org.w3c.dom.UserDataHandler;
	using DOMException = org.w3c.dom.DOMException;

	/// <summary>
	/// This is a kluge to let us shove a declaration for xml: into the
	/// DOM2DTM model.  Basically, it creates a proxy node in DOM space to
	/// carry the additional information. This is _NOT_ a full DOM
	/// implementation, and shouldn't be one since it sits alongside the
	/// DOM rather than becoming part of the DOM model.
	/// 
	/// (This used to be an internal class within DOM2DTM. Moved out because
	/// I need to perform an instanceof operation on it to support a temporary
	/// workaround in DTMManagerDefault.)
	/// 
	/// %REVIEW% What if the DOM2DTM was built around a DocumentFragment and
	/// there isn't a single root element? I think this fails that case...
	/// 
	/// %REVIEW% An alternative solution would be to create the node _only_
	/// in DTM space, but given how DOM2DTM is currently written I think
	/// this is simplest.
	/// 
	/// </summary>
	public class DOM2DTMdefaultNamespaceDeclarationNode : Attr, TypeInfo
	{
	  internal const string NOT_SUPPORTED_ERR = "Unsupported operation on pseudonode";

	  internal Element pseudoparent;
	  internal string prefix, uri, nodename;
	  internal int handle;
	  internal DOM2DTMdefaultNamespaceDeclarationNode(Element pseudoparent, string prefix, string uri, int handle)
	  {
		this.pseudoparent = pseudoparent;
		this.prefix = prefix;
		this.uri = uri;
		this.handle = handle;
		this.nodename = "xmlns:" + prefix;
	  }
	  public virtual string NodeName
	  {
		  get
		  {
			  return nodename;
		  }
	  }
	  public virtual string Name
	  {
		  get
		  {
			  return nodename;
		  }
	  }
	  public virtual string NamespaceURI
	  {
		  get
		  {
			  return "http://www.w3.org/2000/xmlns/";
		  }
	  }
	  public virtual string Prefix
	  {
		  get
		  {
			  return prefix;
		  }
		  set
		  {
			  throw new DTMException(NOT_SUPPORTED_ERR);
		  }
	  }
	  public virtual string LocalName
	  {
		  get
		  {
			  return prefix;
		  }
	  }
	  public virtual string NodeValue
	  {
		  get
		  {
			  return uri;
		  }
		  set
		  {
			  throw new DTMException(NOT_SUPPORTED_ERR);
		  }
	  }
	  public virtual string StringValue
	  {
		  get
		  {
			  return uri;
		  }
		  set
		  {
			  throw new DTMException(NOT_SUPPORTED_ERR);
		  }
	  }
	  public virtual Element OwnerElement
	  {
		  get
		  {
			  return pseudoparent;
		  }
	  }

	  public virtual bool isSupported(string feature, string version)
	  {
		  return false;
	  }
	  public virtual bool hasChildNodes()
	  {
		  return false;
	  }
	  public virtual bool hasAttributes()
	  {
		  return false;
	  }
	  public virtual Node ParentNode
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual Node FirstChild
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual Node LastChild
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual Node PreviousSibling
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual Node NextSibling
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual bool Specified
	  {
		  get
		  {
			  return false;
		  }
	  }
	  public virtual void normalize()
	  {
		  return;
	  }
	  public virtual NodeList ChildNodes
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual NamedNodeMap Attributes
	  {
		  get
		  {
			  return null;
		  }
	  }
	  public virtual short NodeType
	  {
		  get
		  {
			  return Node.ATTRIBUTE_NODE;
		  }
	  }
	  public virtual Node insertBefore(Node a, Node b)
	  {
		  throw new DTMException(NOT_SUPPORTED_ERR);
	  }
	  public virtual Node replaceChild(Node a, Node b)
	  {
		  throw new DTMException(NOT_SUPPORTED_ERR);
	  }
	  public virtual Node appendChild(Node a)
	  {
		  throw new DTMException(NOT_SUPPORTED_ERR);
	  }
	  public virtual Node removeChild(Node a)
	  {
		  throw new DTMException(NOT_SUPPORTED_ERR);
	  }
	  public virtual Document OwnerDocument
	  {
		  get
		  {
			  return pseudoparent.getOwnerDocument();
		  }
	  }
	  public virtual Node cloneNode(bool deep)
	  {
		  throw new DTMException(NOT_SUPPORTED_ERR);
	  }

		/// <summary>
		/// Non-DOM method, part of the temporary kluge
		/// %REVIEW% This would be a pruning problem, but since it will always be
		/// added to the root element and we prune on elements, we shouldn't have 
		/// to worry.
		/// </summary>
		public virtual int HandleOfNode
		{
			get
			{
				return handle;
			}
		}

		//RAMESH: PENDING=> Add proper implementation for the below DOM L3 additions

		/// <seealso cref="org.w3c.dom.TypeInfo.getTypeName()"/>
		public virtual string TypeName
		{
			get
			{
				return null;
			}
		}

		/// <seealso cref="org.w3c.dom.TypeInfo.getTypeNamespace()"/>
		public virtual string TypeNamespace
		{
			get
			{
				return null;
			}
		}

		/// <seealso cref="or.gw3c.dom.TypeInfo.isDerivedFrom(String,String,int)"/>
		public virtual bool isDerivedFrom(string ns, string localName, int derivationMethod)
		{
			return false;
		}

		public virtual TypeInfo SchemaTypeInfo
		{
			get
			{
				return this;
			}
		}

		public virtual bool Id
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Associate an object to a key on this node. The object can later be
		/// retrieved from this node by calling <code>getUserData</code> with the
		/// same key. </summary>
		/// <param name="key"> The key to associate the object to. </param>
		/// <param name="data"> The object to associate to the given key, or
		///   <code>null</code> to remove any existing association to that key. </param>
		/// <param name="handler"> The handler to associate to that key, or
		///   <code>null</code>. </param>
		/// <returns> Returns the <code>DOMObject</code> previously associated to
		///   the given key on this node, or <code>null</code> if there was none.
		/// @since DOM Level 3 </returns>
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
			if (arg.getNodeType() != NodeType)
			{
				return false;
			}
			// in theory nodeName can't be null but better be careful
			// who knows what other implementations may be doing?...
			if (string.ReferenceEquals(NodeName, null))
			{
				if (arg.getNodeName() != null)
				{
					return false;
				}
			}
			else if (!NodeName.Equals(arg.getNodeName()))
			{
				return false;
			}

			if (string.ReferenceEquals(LocalName, null))
			{
				if (arg.getLocalName() != null)
				{
					return false;
				}
			}
			else if (!LocalName.Equals(arg.getLocalName()))
			{
				return false;
			}

			if (string.ReferenceEquals(NamespaceURI, null))
			{
				if (arg.getNamespaceURI() != null)
				{
					return false;
				}
			}
			else if (!NamespaceURI.Equals(arg.getNamespaceURI()))
			{
				return false;
			}

			if (string.ReferenceEquals(Prefix, null))
			{
				if (arg.getPrefix() != null)
				{
					return false;
				}
			}
			else if (!Prefix.Equals(arg.getPrefix()))
			{
				return false;
			}

			if (string.ReferenceEquals(NodeValue, null))
			{
				if (arg.getNodeValue() != null)
				{
					return false;
				}
			}
			else if (!NodeValue.Equals(arg.getNodeValue()))
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
		/// DOM Level 3 - Experimental:
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
						int length = map.getLength();
						for (int i = 0;i < length;i++)
						{
							Node attr = map.item(i);
							string attrPrefix = attr.getPrefix();
							string value = attr.getNodeValue();
							@namespace = attr.getNamespaceURI();
							if (!string.ReferenceEquals(@namespace, null) && @namespace.Equals("http://www.w3.org/2000/xmlns/"))
							{
								// at this point we are dealing with DOM Level 2 nodes only
								if (string.ReferenceEquals(specifiedPrefix, null) && attr.getNodeName().Equals("xmlns"))
								{
									// default namespace
									return value;
								}
								else if (!string.ReferenceEquals(attrPrefix, null) && attrPrefix.Equals("xmlns") && attr.getLocalName().Equals(specifiedPrefix))
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
					if (this.OwnerElement.getNodeType() == Node.ELEMENT_NODE)
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
		///  DOM Level 3: Experimental
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

		/// 
		/// <summary>
		/// DOM Level 3 - Experimental:
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
					if (this.OwnerElement.getNodeType() == Node.ELEMENT_NODE)
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setTextContent(String textContent) throws org.w3c.dom.DOMException
		public virtual string TextContent
		{
			set
			{
				NodeValue = value;
			}
			get
			{
				return NodeValue; // overriden in some subclasses
			}
		}


		/// <summary>
		/// Compares a node with this node with regard to their position in the
		/// document. </summary>
		/// <param name="other"> The node to compare against this node. </param>
		/// <returns> Returns how the given node is positioned relatively to this
		///   node.
		/// @since DOM Level 3 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
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
	}


}