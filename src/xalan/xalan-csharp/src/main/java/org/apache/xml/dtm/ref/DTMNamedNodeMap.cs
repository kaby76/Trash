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
 * $Id: DTMNamedNodeMap.java 1225427 2011-12-29 04:33:32Z mrglavas $
 */
namespace org.apache.xml.dtm.@ref
{

	using DOMException = org.w3c.dom.DOMException;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// DTMNamedNodeMap is a quickie (as opposed to quick) implementation of the DOM's
	/// NamedNodeMap interface, intended to support DTMProxy's getAttributes()
	/// call.
	/// <para>
	/// ***** Note: this does _not_ current attempt to cache any of the data;
	/// if you ask for attribute 27 and then 28, you'll have to rescan the first
	/// 27. It should probably at least keep track of the last one retrieved,
	/// and possibly buffer the whole array.
	/// </para>
	/// <para>
	/// ***** Also note that there's no fastpath for the by-name query; we search
	/// linearly until we find it or fail to find it. Again, that could be
	/// optimized at some cost in object creation/storage.
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	public class DTMNamedNodeMap : NamedNodeMap
	{

	  /// <summary>
	  /// The DTM for this node. </summary>
	  internal DTM dtm;

	  /// <summary>
	  /// The DTM element handle. </summary>
	  internal int element;

	  /// <summary>
	  /// The number of nodes in this map. </summary>
	  internal short m_count = -1;

	  /// <summary>
	  /// Create a getAttributes NamedNodeMap for a given DTM element node
	  /// </summary>
	  /// <param name="dtm"> The DTM Reference, must be non-null. </param>
	  /// <param name="element"> The DTM element handle. </param>
	  public DTMNamedNodeMap(DTM dtm, int element)
	  {
		this.dtm = dtm;
		this.element = element;
	  }

	  /// <summary>
	  /// Return the number of Attributes on this Element
	  /// </summary>
	  /// <returns> The number of nodes in this map. </returns>
	  public virtual int Length
	  {
		  get
		  {
    
			if (m_count == -1)
			{
			  short count = 0;
    
			  for (int n = dtm.getFirstAttribute(element); n != -1; n = dtm.getNextAttribute(n))
			  {
				++count;
			  }
    
			  m_count = count;
			}
    
			return (int) m_count;
		  }
	  }

	  /// <summary>
	  /// Retrieves a node specified by name. </summary>
	  /// <param name="name"> The <code>nodeName</code> of a node to retrieve. </param>
	  /// <returns> A <code>Node</code> (of any type) with the specified
	  ///   <code>nodeName</code>, or <code>null</code> if it does not identify
	  ///   any node in this map. </returns>
	  public virtual Node getNamedItem(string name)
	  {

		for (int n = dtm.getFirstAttribute(element); n != org.apache.xml.dtm.DTM_Fields.NULL; n = dtm.getNextAttribute(n))
		{
		  if (dtm.getNodeName(n).Equals(name))
		  {
			return dtm.getNode(n);
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Returns the <code>index</code>th item in the map. If <code>index</code>
	  /// is greater than or equal to the number of nodes in this map, this
	  /// returns <code>null</code>. </summary>
	  /// <param name="i"> The index of the requested item. </param>
	  /// <returns> The node at the <code>index</code>th position in the map, or
	  ///   <code>null</code> if that is not a valid index. </returns>
	  public virtual Node item(int i)
	  {

		int count = 0;

		for (int n = dtm.getFirstAttribute(element); n != -1; n = dtm.getNextAttribute(n))
		{
		  if (count == i)
		  {
			return dtm.getNode(n);
		  }
		  else
		  {
			++count;
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Adds a node using its <code>nodeName</code> attribute. If a node with
	  /// that name is already present in this map, it is replaced by the new
	  /// one.
	  /// <br>As the <code>nodeName</code> attribute is used to derive the name
	  /// which the node must be stored under, multiple nodes of certain types
	  /// (those that have a "special" string value) cannot be stored as the
	  /// names would clash. This is seen as preferable to allowing nodes to be
	  /// aliased. </summary>
	  /// <param name="newNode"> node to store in this map. The node will later be
	  ///   accessible using the value of its <code>nodeName</code> attribute.
	  /// </param>
	  /// <returns> If the new <code>Node</code> replaces an existing node the
	  ///   replaced <code>Node</code> is returned, otherwise <code>null</code>
	  ///   is returned. </returns>
	  /// <exception cref="DOMException">
	  ///   WRONG_DOCUMENT_ERR: Raised if <code>arg</code> was created from a
	  ///   different document than the one that created this map.
	  ///   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this map is readonly.
	  ///   <br>INUSE_ATTRIBUTE_ERR: Raised if <code>arg</code> is an
	  ///   <code>Attr</code> that is already an attribute of another
	  ///   <code>Element</code> object. The DOM user must explicitly clone
	  ///   <code>Attr</code> nodes to re-use them in other elements. </exception>
	  public virtual Node setNamedItem(Node newNode)
	  {
		throw new DTMException(DTMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// <summary>
	  /// Removes a node specified by name. When this map contains the attributes
	  /// attached to an element, if the removed attribute is known to have a
	  /// default value, an attribute immediately appears containing the
	  /// default value as well as the corresponding namespace URI, local name,
	  /// and prefix when applicable. </summary>
	  /// <param name="name"> The <code>nodeName</code> of the node to remove.
	  /// </param>
	  /// <returns> The node removed from this map if a node with such a name
	  ///   exists. </returns>
	  /// <exception cref="DOMException">
	  ///   NOT_FOUND_ERR: Raised if there is no node named <code>name</code> in
	  ///   this map.
	  ///   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this map is readonly. </exception>
	  public virtual Node removeNamedItem(string name)
	  {
		throw new DTMException(DTMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// <summary>
	  /// Retrieves a node specified by local name and namespace URI. HTML-only
	  /// DOM implementations do not need to implement this method. </summary>
	  /// <param name="namespaceURI"> The namespace URI of the node to retrieve. </param>
	  /// <param name="localName"> The local name of the node to retrieve.
	  /// </param>
	  /// <returns> A <code>Node</code> (of any type) with the specified local
	  ///   name and namespace URI, or <code>null</code> if they do not
	  ///   identify any node in this map.
	  /// @since DOM Level 2 </returns>
	  public virtual Node getNamedItemNS(string namespaceURI, string localName)
	  {
		   Node retNode = null;
		   for (int n = dtm.getFirstAttribute(element); n != org.apache.xml.dtm.DTM_Fields.NULL; n = dtm.getNextAttribute(n))
		   {
			 if (localName.Equals(dtm.getLocalName(n)))
			 {
			   string nsURI = dtm.getNamespaceURI(n);
			   if ((string.ReferenceEquals(namespaceURI, null) && string.ReferenceEquals(nsURI, null)) || (!string.ReferenceEquals(namespaceURI, null) && namespaceURI.Equals(nsURI)))
			   {
				 retNode = dtm.getNode(n);
				 break;
			   }
			 }
		   }
		   return retNode;
	  }

	  /// <summary>
	  /// Adds a node using its <code>namespaceURI</code> and
	  /// <code>localName</code>. If a node with that namespace URI and that
	  /// local name is already present in this map, it is replaced by the new
	  /// one.
	  /// <br>HTML-only DOM implementations do not need to implement this method. </summary>
	  /// <param name="arg"> A node to store in this map. The node will later be
	  ///   accessible using the value of its <code>namespaceURI</code> and
	  ///   <code>localName</code> attributes.
	  /// </param>
	  /// <returns> If the new <code>Node</code> replaces an existing node the
	  ///   replaced <code>Node</code> is returned, otherwise <code>null</code>
	  ///   is returned. </returns>
	  /// <exception cref="DOMException">
	  ///   WRONG_DOCUMENT_ERR: Raised if <code>arg</code> was created from a
	  ///   different document than the one that created this map.
	  ///   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this map is readonly.
	  ///   <br>INUSE_ATTRIBUTE_ERR: Raised if <code>arg</code> is an
	  ///   <code>Attr</code> that is already an attribute of another
	  ///   <code>Element</code> object. The DOM user must explicitly clone
	  ///   <code>Attr</code> nodes to re-use them in other elements.
	  /// @since DOM Level 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node setNamedItemNS(org.w3c.dom.Node arg) throws org.w3c.dom.DOMException
	  public virtual Node setNamedItemNS(Node arg)
	  {
		throw new DTMException(DTMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// <summary>
	  /// Removes a node specified by local name and namespace URI. A removed
	  /// attribute may be known to have a default value when this map contains
	  /// the attributes attached to an element, as returned by the attributes
	  /// attribute of the <code>Node</code> interface. If so, an attribute
	  /// immediately appears containing the default value as well as the
	  /// corresponding namespace URI, local name, and prefix when applicable.
	  /// <br>HTML-only DOM implementations do not need to implement this method.
	  /// </summary>
	  /// <param name="namespaceURI"> The namespace URI of the node to remove. </param>
	  /// <param name="localName"> The local name of the node to remove.
	  /// </param>
	  /// <returns> The node removed from this map if a node with such a local
	  ///   name and namespace URI exists. </returns>
	  /// <exception cref="DOMException">
	  ///   NOT_FOUND_ERR: Raised if there is no node with the specified
	  ///   <code>namespaceURI</code> and <code>localName</code> in this map.
	  ///   <br>NO_MODIFICATION_ALLOWED_ERR: Raised if this map is readonly.
	  /// @since DOM Level 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node removeNamedItemNS(String namespaceURI, String localName) throws org.w3c.dom.DOMException
	  public virtual Node removeNamedItemNS(string namespaceURI, string localName)
	  {
		throw new DTMException(DTMException.NO_MODIFICATION_ALLOWED_ERR);
	  }

	  /// <summary>
	  /// Simple implementation of DOMException.
	  /// @xsl.usage internal
	  /// </summary>
	  public class DTMException : DOMException
	  {
			  internal const long serialVersionUID = -8290238117162437678L;
		/// <summary>
		/// Constructs a DOM/DTM exception.
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="message"> </param>
		public DTMException(short code, string message) : base(code, message)
		{
		}

		/// <summary>
		/// Constructor DTMException
		/// 
		/// </summary>
		/// <param name="code"> </param>
		public DTMException(short code) : base(code, "")
		{
		}
	  }
	}

}