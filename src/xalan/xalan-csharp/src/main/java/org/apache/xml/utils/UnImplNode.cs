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
 * $Id: UnImplNode.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

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
	/// To be subclassed by classes that wish to fake being nodes.
	/// @xsl.usage internal
	/// </summary>
	public class UnImplNode : Node, Element, NodeList, Document
	{

	  /// <summary>
	  /// Constructor UnImplNode
	  /// 
	  /// </summary>
	  public UnImplNode()
	  {
	  }

	  /// <summary>
	  /// Throw an error.
	  /// </summary>
	  /// <param name="msg"> Message Key for the error </param>
	  public virtual void error(string msg)
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Console.WriteLine("DOM ERROR! class: " + this.GetType().FullName);

		throw new Exception(XMLMessages.createXMLMessage(msg, null));
	  }

	  /// <summary>
	  /// Throw an error.
	  /// </summary>
	  /// <param name="msg"> Message Key for the error </param>
	  /// <param name="args"> Array of arguments to be used in the error message </param>
	  public virtual void error(string msg, object[] args)
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Console.WriteLine("DOM ERROR! class: " + this.GetType().FullName);

		throw new Exception(XMLMessages.createXMLMessage(msg, args)); //"UnImplNode error: "+msg);
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="newChild"> New node to append to the list of this node's children
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node appendChild(org.w3c.dom.Node newChild) throws org.w3c.dom.DOMException
	  public virtual Node appendChild(Node newChild)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"appendChild not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> false </returns>
	  public virtual bool hasChildNodes()
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"hasChildNodes not supported!");

		return false;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> 0 </returns>
	  public virtual short NodeType
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getNodeType not supported!");
    
			return 0;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Node ParentNode
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getParentNode not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual NodeList ChildNodes
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getChildNodes not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Node FirstChild
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getFirstChild not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Node LastChild
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getLastChild not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Node NextSibling
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getNextSibling not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.NodeList
	  /// </summary>
	  /// <returns> 0 </returns>
	  public virtual int Length
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getLength not supported!");
    
			return 0;
		  }
	  } // getLength():int

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.NodeList
	  /// </summary>
	  /// <param name="index"> index of a child of this node in its list of children
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Node item(int index)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"item not supported!");

		return null;
	  } // item(int):Node

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Document OwnerDocument
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getOwnerDocument not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual string TagName
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getTagName not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual string NodeName
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getNodeName not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node </summary>
	  public virtual void normalize()
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"normalize not supported!");
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="name"> Name of the element
	  /// </param>
	  /// <returns> null </returns>
	  public virtual NodeList getElementsByTagName(string name)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getElementsByTagName not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="oldAttr"> Attribute to be removed from this node's list of attributes
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Attr removeAttributeNode(org.w3c.dom.Attr oldAttr) throws org.w3c.dom.DOMException
	  public virtual Attr removeAttributeNode(Attr oldAttr)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"removeAttributeNode not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="newAttr"> Attribute node to be added to this node's list of attributes
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Attr setAttributeNode(org.w3c.dom.Attr newAttr) throws org.w3c.dom.DOMException
	  public virtual Attr setAttributeNode(Attr newAttr)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setAttributeNode not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// 
	  /// </summary>
	  /// <param name="name"> Name of an attribute
	  /// </param>
	  /// <returns> false </returns>
	  public virtual bool hasAttribute(string name)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"hasAttribute not supported!");

		return false;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// 
	  /// </summary>
	  /// <param name="name"> </param>
	  /// <param name="x">
	  /// </param>
	  /// <returns> false </returns>
	  public virtual bool hasAttributeNS(string name, string x)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"hasAttributeNS not supported!");

		return false;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// 
	  /// </summary>
	  /// <param name="name"> Attribute node name
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Attr getAttributeNode(string name)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getAttributeNode not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="name"> Attribute node name to remove from list of attributes
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void removeAttribute(String name) throws org.w3c.dom.DOMException
	  public virtual void removeAttribute(string name)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"removeAttribute not supported!");
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="name"> Name of attribute to set </param>
	  /// <param name="value"> Value of attribute
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setAttribute(String name, String value) throws org.w3c.dom.DOMException
	  public virtual void setAttribute(string name, string value)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setAttribute not supported!");
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="name"> Name of attribute to get
	  /// </param>
	  /// <returns> null </returns>
	  public virtual string getAttribute(string name)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getAttribute not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. Introduced in DOM Level 2.
	  /// </summary>
	  /// <returns> false </returns>
	  public virtual bool hasAttributes()
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"hasAttributes not supported!");

		return false;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of the element </param>
	  /// <param name="localName"> Local part of qualified name of the element
	  /// </param>
	  /// <returns> null </returns>
	  public virtual NodeList getElementsByTagNameNS(string namespaceURI, string localName)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getElementsByTagNameNS not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="newAttr"> Attribute to set
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Attr setAttributeNodeNS(org.w3c.dom.Attr newAttr) throws org.w3c.dom.DOMException
	  public virtual Attr setAttributeNodeNS(Attr newAttr)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setAttributeNodeNS not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of attribute node to get </param>
	  /// <param name="localName"> Local part of qualified name of attribute node to get
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Attr getAttributeNodeNS(string namespaceURI, string localName)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getAttributeNodeNS not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of attribute node to remove </param>
	  /// <param name="localName"> Local part of qualified name of attribute node to remove
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void removeAttributeNS(String namespaceURI, String localName) throws org.w3c.dom.DOMException
	  public virtual void removeAttributeNS(string namespaceURI, string localName)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"removeAttributeNS not supported!");
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of attribute node to set </param>
	  /// NEEDSDOC <param name="qualifiedName"> </param>
	  /// <param name="value"> value of attribute
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setAttributeNS(String namespaceURI, String qualifiedName, String value) throws org.w3c.dom.DOMException
	  public virtual void setAttributeNS(string namespaceURI, string qualifiedName, string value)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setAttributeNS not supported!");
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Element
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of attribute node to get </param>
	  /// <param name="localName"> Local part of qualified name of attribute node to get
	  /// </param>
	  /// <returns> null </returns>
	  public virtual string getAttributeNS(string namespaceURI, string localName)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getAttributeNS not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Node PreviousSibling
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getPreviousSibling not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="deep"> Flag indicating whether to clone deep (clone member variables)
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Node cloneNode(bool deep)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"cloneNode not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getNodeValue() throws org.w3c.dom.DOMException
	  public virtual string NodeValue
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getNodeValue not supported!");
    
			return null;
		  }
		  set
		  {
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setNodeValue not supported!");
		  }
	  }


	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="value"> </param>
	  /// <returns> value Node value
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>

	  // public String getValue ()
	  // {      
	  //  error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getValue not supported!");
	  //  return null;
	  // } 

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="value"> Value to set this node to
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setValue(String value) throws org.w3c.dom.DOMException
	  public virtual string Value
	  {
		  set
		  {
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setValue not supported!");
		  }
	  }

	  /// <summary>
	  ///  Returns the name of this attribute.
	  /// </summary>
	  /// <returns> the name of this attribute. </returns>

	  // public String getName()
	  // {
	  //  return this.getNodeName();
	  // }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Element OwnerElement
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getOwnerElement not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> False </returns>
	  public virtual bool Specified
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setValue not supported!");
    
			return false;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual NamedNodeMap Attributes
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getAttributes not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="newChild"> New child node to insert </param>
	  /// <param name="refChild"> Insert in front of this child
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node insertBefore(org.w3c.dom.Node newChild, org.w3c.dom.Node refChild) throws org.w3c.dom.DOMException
	  public virtual Node insertBefore(Node newChild, Node refChild)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"insertBefore not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="newChild"> Replace existing child with this one </param>
	  /// <param name="oldChild"> Existing child to be replaced
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node replaceChild(org.w3c.dom.Node newChild, org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
	  public virtual Node replaceChild(Node newChild, Node oldChild)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"replaceChild not supported!");

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="oldChild"> Child to be removed
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node removeChild(org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
	  public virtual Node removeChild(Node oldChild)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"replaceChild not supported!");

		return null;
	  }

	  /// <summary>
	  /// Tests whether the DOM implementation implements a specific feature and
	  /// that feature is supported by this node. </summary>
	  /// <param name="feature"> The name of the feature to test. This is the same name
	  ///   which can be passed to the method <code>hasFeature</code> on
	  ///   <code>DOMImplementation</code>. </param>
	  /// <param name="version"> This is the version number of the feature to test. In
	  ///   Level 2, version 1, this is the string "2.0". If the version is not
	  ///   specified, supporting any version of the feature will cause the
	  ///   method to return <code>true</code>.
	  /// </param>
	  /// <returns> Returns <code>false</code>
	  /// @since DOM Level 2 </returns>
	  public virtual bool isSupported(string feature, string version)
	  {
		return false;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual string NamespaceURI
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getNamespaceURI not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual string Prefix
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getPrefix not supported!");
    
			return null;
		  }
		  set
		  {
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"setPrefix not supported!");
		  }
	  }


	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual string LocalName
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED); //"getLocalName not supported!");
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual DocumentType Doctype
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual DOMImplementation Implementation
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual Element DocumentElement
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="tagName"> Element tag name
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Element createElement(String tagName) throws org.w3c.dom.DOMException
	  public virtual Element createElement(string tagName)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <returns> null </returns>
	  public virtual DocumentFragment createDocumentFragment()
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="data"> Data for text node
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Text createTextNode(string data)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="data"> Data for comment
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Comment createComment(string data)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="data"> Data for CDATA section
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.CDATASection createCDATASection(String data) throws org.w3c.dom.DOMException
	  public virtual CDATASection createCDATASection(string data)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="target"> Target for Processing instruction </param>
	  /// <param name="data"> Data for Processing instruction
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.ProcessingInstruction createProcessingInstruction(String target, String data) throws org.w3c.dom.DOMException
	  public virtual ProcessingInstruction createProcessingInstruction(string target, string data)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="name"> Attribute name
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Attr createAttribute(String name) throws org.w3c.dom.DOMException
	  public virtual Attr createAttribute(string name)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="name"> Entity Reference name
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.EntityReference createEntityReference(String name) throws org.w3c.dom.DOMException
	  public virtual EntityReference createEntityReference(string name)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="importedNode"> The node to import. </param>
	  /// <param name="deep">         If <code>true</code>, recursively import the subtree under
	  ///   the specified node; if <code>false</code>, import only the node
	  ///   itself, as explained above. This has no effect on <code>Attr</code>
	  ///   , <code>EntityReference</code>, and <code>Notation</code> nodes.
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node importNode(org.w3c.dom.Node importedNode, boolean deep) throws org.w3c.dom.DOMException
	  public virtual Node importNode(Node importedNode, bool deep)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI for the element </param>
	  /// <param name="qualifiedName"> Qualified name of the element
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Element createElementNS(String namespaceURI, String qualifiedName) throws org.w3c.dom.DOMException
	  public virtual Element createElementNS(string namespaceURI, string qualifiedName)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of the attribute </param>
	  /// <param name="qualifiedName"> Qualified name of the attribute
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Attr createAttributeNS(String namespaceURI, String qualifiedName) throws org.w3c.dom.DOMException
	  public virtual Attr createAttributeNS(string namespaceURI, string qualifiedName)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Document
	  /// </summary>
	  /// <param name="elementId"> ID of the element to get
	  /// </param>
	  /// <returns> null </returns>
	  public virtual Element getElementById(string elementId)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Set Node data
	  /// 
	  /// </summary>
	  /// <param name="data"> data to set for this node
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setData(String data) throws org.w3c.dom.DOMException
	  public virtual string Data
	  {
		  set
		  {
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
		  }
	  }

	  /// <summary>
	  /// Unimplemented.
	  /// </summary>
	  /// <param name="offset"> Start offset of substring to extract. </param>
	  /// <param name="count"> The length of the substring to extract.
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String substringData(int offset, int count) throws org.w3c.dom.DOMException
	  public virtual string substringData(int offset, int count)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// Unimplemented.
	  /// </summary>
	  /// <param name="arg"> String data to append
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void appendData(String arg) throws org.w3c.dom.DOMException
	  public virtual void appendData(string arg)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
	  }

	  /// <summary>
	  /// Unimplemented.
	  /// </summary>
	  /// <param name="offset"> Start offset of substring to insert. </param>
	  /// NEEDSDOC <param name="arg">
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void insertData(int offset, String arg) throws org.w3c.dom.DOMException
	  public virtual void insertData(int offset, string arg)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
	  }

	  /// <summary>
	  /// Unimplemented.
	  /// </summary>
	  /// <param name="offset"> Start offset of substring to delete. </param>
	  /// <param name="count"> The length of the substring to delete.
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void deleteData(int offset, int count) throws org.w3c.dom.DOMException
	  public virtual void deleteData(int offset, int count)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
	  }

	  /// <summary>
	  /// Unimplemented.
	  /// </summary>
	  /// <param name="offset"> Start offset of substring to replace. </param>
	  /// <param name="count"> The length of the substring to replace. </param>
	  /// <param name="arg"> substring to replace with
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void replaceData(int offset, int count, String arg) throws org.w3c.dom.DOMException
	  public virtual void replaceData(int offset, int count, string arg)
	  {
		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
	  }

	  /// <summary>
	  /// Unimplemented.
	  /// </summary>
	  /// <param name="offset"> Offset into text to split
	  /// </param>
	  /// <returns> null, unimplemented
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Text splitText(int offset) throws org.w3c.dom.DOMException
	  public virtual Text splitText(int offset)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
	  }

	  /// <summary>
	  /// NEEDSDOC Method adoptNode 
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="source">
	  /// 
	  /// NEEDSDOC (adoptNode) @return
	  /// </param>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node adoptNode(org.w3c.dom.Node source) throws org.w3c.dom.DOMException
	  public virtual Node adoptNode(Node source)
	  {

		error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);

		return null;
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
	  /// NEEDSDOC ($objectName$) @return
	  /// </para>
	  /// </summary>
	  public virtual string InputEncoding
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
    
			return null;
		  }
		  set
		  {
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
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
	  /// NEEDSDOC ($objectName$) @return
	  /// </para>
	  /// </summary>
	  public virtual bool StrictErrorChecking
	  {
		  get
		  {
    
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
    
			return false;
		  }
		  set
		  {
			error(XMLErrorResources.ER_FUNCTION_NOT_SUPPORTED);
		  }
	  }


		// RAMESH : Pending proper implementation of DOM Level 3    
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

		/// 
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

		/// <summary>
		/// DOM Level 3
		/// Renaming node
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
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
		public virtual bool WhitespaceInElementContent
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
		public virtual void setIdAttribute(bool id)
		{
			//PENDING
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

		/// <summary>
		/// Method getSchemaTypeInfo. </summary>
		/// <returns> TypeInfo </returns>
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