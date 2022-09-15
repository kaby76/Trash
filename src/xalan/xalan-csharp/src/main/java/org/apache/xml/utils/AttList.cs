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
 * $Id: AttList.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{
	using Attr = org.w3c.dom.Attr;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// Wraps a DOM attribute list in a SAX Attributes.
	/// @xsl.usage internal
	/// </summary>
	public class AttList : Attributes
	{

	  /// <summary>
	  /// List of attribute nodes </summary>
	  internal NamedNodeMap m_attrs;

	  /// <summary>
	  /// Index of last attribute node </summary>
	  internal int m_lastIndex;

	  // ARGHH!!  JAXP Uses Xerces without setting the namespace processing to ON!
	  // DOM2Helper m_dh = new DOM2Helper();

	  /// <summary>
	  /// Local reference to DOMHelper </summary>
	  internal DOMHelper m_dh;

	//  /**
	//   * Constructor AttList
	//   *
	//   *
	//   * @param attrs List of attributes this will contain
	//   */
	//  public AttList(NamedNodeMap attrs)
	//  {
	//
	//    m_attrs = attrs;
	//    m_lastIndex = m_attrs.getLength() - 1;
	//    m_dh = new DOM2Helper();
	//  }

	  /// <summary>
	  /// Constructor AttList
	  /// 
	  /// </summary>
	  /// <param name="attrs"> List of attributes this will contain </param>
	  /// <param name="dh"> DOMHelper  </param>
	  public AttList(NamedNodeMap attrs, DOMHelper dh)
	  {

		m_attrs = attrs;
		m_lastIndex = m_attrs.getLength() - 1;
		m_dh = dh;
	  }

	  /// <summary>
	  /// Get the number of attribute nodes in the list 
	  /// 
	  /// </summary>
	  /// <returns> number of attribute nodes </returns>
	  public virtual int Length
	  {
		  get
		  {
			return m_attrs.getLength();
		  }
	  }

	  /// <summary>
	  /// Look up an attribute's Namespace URI by index.
	  /// </summary>
	  /// <param name="index"> The attribute index (zero-based). </param>
	  /// <returns> The Namespace URI, or the empty string if none
	  ///         is available, or null if the index is out of
	  ///         range. </returns>
	  public virtual string getURI(int index)
	  {
		string ns = m_dh.getNamespaceOfNode(((Attr) m_attrs.item(index)));
		if (null == ns)
		{
		  ns = "";
		}
		return ns;
	  }

	  /// <summary>
	  /// Look up an attribute's local name by index.
	  /// </summary>
	  /// <param name="index"> The attribute index (zero-based). </param>
	  /// <returns> The local name, or the empty string if Namespace
	  ///         processing is not being performed, or null
	  ///         if the index is out of range. </returns>
	  public virtual string getLocalName(int index)
	  {
		return m_dh.getLocalNameOfNode(((Attr) m_attrs.item(index)));
	  }

	  /// <summary>
	  /// Look up an attribute's qualified name by index.
	  /// 
	  /// </summary>
	  /// <param name="i"> The attribute index (zero-based).
	  /// </param>
	  /// <returns> The attribute's qualified name </returns>
	  public virtual string getQName(int i)
	  {
		return ((Attr) m_attrs.item(i)).getName();
	  }

	  /// <summary>
	  /// Get the attribute's node type by index
	  /// 
	  /// </summary>
	  /// <param name="i"> The attribute index (zero-based)
	  /// </param>
	  /// <returns> the attribute's node type </returns>
	  public virtual string getType(int i)
	  {
		return "CDATA"; // for the moment
	  }

	  /// <summary>
	  /// Get the attribute's node value by index
	  /// 
	  /// </summary>
	  /// <param name="i"> The attribute index (zero-based)
	  /// </param>
	  /// <returns> the attribute's node value </returns>
	  public virtual string getValue(int i)
	  {
		return ((Attr) m_attrs.item(i)).getValue();
	  }

	  /// <summary>
	  /// Get the attribute's node type by name
	  /// 
	  /// </summary>
	  /// <param name="name"> Attribute name
	  /// </param>
	  /// <returns> the attribute's node type </returns>
	  public virtual string getType(string name)
	  {
		return "CDATA"; // for the moment
	  }

	  /// <summary>
	  /// Look up an attribute's type by Namespace name.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty String if the
	  ///        name has no Namespace URI. </param>
	  /// <param name="localName"> The local name of the attribute. </param>
	  /// <returns> The attribute type as a string, or null if the
	  ///         attribute is not in the list or if Namespace
	  ///         processing is not being performed. </returns>
	  public virtual string getType(string uri, string localName)
	  {
		return "CDATA"; // for the moment
	  }

	  /// <summary>
	  /// Look up an attribute's value by name.
	  /// 
	  /// </summary>
	  /// <param name="name"> The attribute node's name
	  /// </param>
	  /// <returns> The attribute node's value </returns>
	  public virtual string getValue(string name)
	  {
		Attr attr = ((Attr) m_attrs.getNamedItem(name));
		return (null != attr) ? attr.getValue() : null;
	  }

	  /// <summary>
	  /// Look up an attribute's value by Namespace name.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty String if the
	  ///        name has no Namespace URI. </param>
	  /// <param name="localName"> The local name of the attribute. </param>
	  /// <returns> The attribute value as a string, or null if the
	  ///         attribute is not in the list. </returns>
	  public virtual string getValue(string uri, string localName)
	  {
			Node a = m_attrs.getNamedItemNS(uri,localName);
			return (a == null) ? null : a.getNodeValue();
	  }

	  /// <summary>
	  /// Look up the index of an attribute by Namespace name.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if
	  ///        the name has no Namespace URI. </param>
	  /// <param name="localPart"> The attribute's local name. </param>
	  /// <returns> The index of the attribute, or -1 if it does not
	  ///         appear in the list. </returns>
	  public virtual int getIndex(string uri, string localPart)
	  {
		for (int i = m_attrs.getLength() - 1;i >= 0;--i)
		{
		  Node a = m_attrs.item(i);
		  string u = a.getNamespaceURI();
		  if ((string.ReferenceEquals(u, null) ? string.ReferenceEquals(uri, null) : u.Equals(uri)) && a.getLocalName().Equals(localPart))
		  {
		return i;
		  }
		}
		return -1;
	  }

	  /// <summary>
	  /// Look up the index of an attribute by raw XML 1.0 name.
	  /// </summary>
	  /// <param name="qName"> The qualified (prefixed) name. </param>
	  /// <returns> The index of the attribute, or -1 if it does not
	  ///         appear in the list. </returns>
	  public virtual int getIndex(string qName)
	  {
		for (int i = m_attrs.getLength() - 1;i >= 0;--i)
		{
		  Node a = m_attrs.item(i);
		  if (a.getNodeName().Equals(qName))
		  {
		return i;
		  }
		}
		return -1;
	  }
	}


}