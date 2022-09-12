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
 * $Id: AttList.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer.utils
{

	using Attr = org.w3c.dom.Attr;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	using Attributes = org.xml.sax.Attributes;

	/// <summary>
	/// Wraps a DOM attribute list in a SAX Attributes.
	/// 
	/// This class is a copy of the one in org.apache.xml.utils. 
	/// It exists to cut the serializers dependancy on that package.
	/// A minor changes from that package are:
	/// DOMHelper reference changed to DOM2Helper, class is not "public"
	/// 
	/// This class is not a public API, it is only public because it is 
	/// used in org.apache.xml.serializer.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public sealed class AttList : Attributes
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
	  internal DOM2Helper m_dh;

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
	  public AttList(NamedNodeMap attrs, DOM2Helper dh)
	  {

		m_attrs = attrs;
		m_lastIndex = m_attrs.Length - 1;
		m_dh = dh;
	  }

	  /// <summary>
	  /// Get the number of attribute nodes in the list 
	  /// 
	  /// </summary>
	  /// <returns> number of attribute nodes </returns>
	  public int Length
	  {
		  get
		  {
			return m_attrs.Length;
		  }
	  }

	  /// <summary>
	  /// Look up an attribute's Namespace URI by index.
	  /// </summary>
	  /// <param name="index"> The attribute index (zero-based). </param>
	  /// <returns> The Namespace URI, or the empty string if none
	  ///         is available, or null if the index is out of
	  ///         range. </returns>
	  public string getURI(int index)
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
	  public string getLocalName(int index)
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
	  public string getQName(int i)
	  {
		return ((Attr) m_attrs.item(i)).Name;
	  }

	  /// <summary>
	  /// Get the attribute's node type by index
	  /// 
	  /// </summary>
	  /// <param name="i"> The attribute index (zero-based)
	  /// </param>
	  /// <returns> the attribute's node type </returns>
	  public string getType(int i)
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
	  public string getValue(int i)
	  {
		return ((Attr) m_attrs.item(i)).Value;
	  }

	  /// <summary>
	  /// Get the attribute's node type by name
	  /// 
	  /// </summary>
	  /// <param name="name"> Attribute name
	  /// </param>
	  /// <returns> the attribute's node type </returns>
	  public string getType(string name)
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
	  public string getType(string uri, string localName)
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
	  public string getValue(string name)
	  {
		Attr attr = ((Attr) m_attrs.getNamedItem(name));
		return (null != attr) ? attr.Value : null;
	  }

	  /// <summary>
	  /// Look up an attribute's value by Namespace name.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty String if the
	  ///        name has no Namespace URI. </param>
	  /// <param name="localName"> The local name of the attribute. </param>
	  /// <returns> The attribute value as a string, or null if the
	  ///         attribute is not in the list. </returns>
	  public string getValue(string uri, string localName)
	  {
			Node a = m_attrs.getNamedItemNS(uri,localName);
			return (a == null) ? null : a.NodeValue;
	  }

	  /// <summary>
	  /// Look up the index of an attribute by Namespace name.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if
	  ///        the name has no Namespace URI. </param>
	  /// <param name="localPart"> The attribute's local name. </param>
	  /// <returns> The index of the attribute, or -1 if it does not
	  ///         appear in the list. </returns>
	  public int getIndex(string uri, string localPart)
	  {
		for (int i = m_attrs.Length - 1;i >= 0;--i)
		{
		  Node a = m_attrs.item(i);
		  string u = a.NamespaceURI;
		  if ((string.ReferenceEquals(u, null) ? string.ReferenceEquals(uri, null) : u.Equals(uri)) && a.LocalName.Equals(localPart))
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
	  public int getIndex(string qName)
	  {
		for (int i = m_attrs.Length - 1;i >= 0;--i)
		{
		  Node a = m_attrs.item(i);
		  if (a.NodeName.Equals(qName))
		  {
		return i;
		  }
		}
		return -1;
	  }
	}


}