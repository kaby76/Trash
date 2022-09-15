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
 * $Id: ElemLiteralResult.java 1225375 2011-12-28 23:03:43Z mrglavas $
 */
namespace org.apache.xalan.templates
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using StringVector = org.apache.xml.utils.StringVector;
	using XPathContext = org.apache.xpath.XPathContext;
	using Attr = org.w3c.dom.Attr;
	using DOMException = org.w3c.dom.DOMException;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using TypeInfo = org.w3c.dom.TypeInfo;
	using UserDataHandler = org.w3c.dom.UserDataHandler;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Implement a Literal Result Element. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#literal-result-element">literal-result-element in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemLiteralResult : ElemUse
	{
		internal new const long serialVersionUID = -8703409074421657260L;

		/// <summary>
		/// The return value as Empty String. </summary>
		private const string EMPTYSTRING = "";

	  /// <summary>
	  /// Tells if this element represents a root element
	  /// that is also the stylesheet element.
	  /// TODO: This should be a derived class.
	  /// @serial
	  /// </summary>
	  private bool isLiteralResultAsStylesheet = false;

	  /// <summary>
	  /// Set whether this element represents a root element
	  /// that is also the stylesheet element.
	  /// 
	  /// </summary>
	  /// <param name="b"> boolean flag indicating whether this element
	  /// represents a root element that is also the stylesheet element. </param>
	  public virtual bool IsLiteralResultAsStylesheet
	  {
		  set
		  {
			isLiteralResultAsStylesheet = value;
		  }
		  get
		  {
			return isLiteralResultAsStylesheet;
		  }
	  }


	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);
		StylesheetRoot.ComposeState cstate = sroot.getComposeState();
		ArrayList vnames = cstate.VariableNames;
		if (null != m_avts)
		{
		  int nAttrs = m_avts.Count;

		  for (int i = (nAttrs - 1); i >= 0; i--)
		  {
			AVT avt = (AVT) m_avts[i];
			avt.fixupVariables(vnames, cstate.GlobalsSize);
		  }
		}
	  }

	  /// <summary>
	  /// The created element node will have the attribute nodes
	  /// that were present on the element node in the stylesheet tree,
	  /// other than attributes with names in the XSLT namespace.
	  /// @serial
	  /// </summary>
	  private IList m_avts = null;

	  /// <summary>
	  /// List of attributes with the XSLT namespace.
	  ///  @serial 
	  /// </summary>
	  private IList m_xslAttr = null;

	  /// <summary>
	  /// Set a literal result attribute (AVTs only).
	  /// </summary>
	  /// <param name="avt"> literal result attribute to add (AVT only) </param>
	  public virtual void addLiteralResultAttribute(AVT avt)
	  {

		if (null == m_avts)
		{
		  m_avts = new ArrayList();
		}

		m_avts.Add(avt);
	  }

	  /// <summary>
	  /// Set a literal result attribute (used for xsl attributes).
	  /// </summary>
	  /// <param name="att"> literal result attribute to add </param>
	  public virtual void addLiteralResultAttribute(string att)
	  {

		if (null == m_xslAttr)
		{
		  m_xslAttr = new ArrayList();
		}

		m_xslAttr.Add(att);
	  }

	  /// <summary>
	  /// Set the "xml:space" attribute.
	  /// A text node is preserved if an ancestor element of the text node
	  /// has an xml:space attribute with a value of preserve, and
	  /// no closer ancestor element has xml:space with a value of default. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#strip">strip in XSLT Specification</a> </seealso>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Creating-Text">section-Creating-Text in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="avt">  Enumerated value, either Constants.ATTRVAL_PRESERVE 
	  /// or Constants.ATTRVAL_STRIP. </param>
	  public virtual AVT XmlSpace
	  {
		  set
		  {
			// This function is a bit-o-hack, I guess...
			addLiteralResultAttribute(value);
			string val = value.SimpleString;
			if (val.Equals("default"))
			{
			  base.XmlSpace = Constants.ATTRVAL_STRIP;
			}
			else if (val.Equals("preserve"))
			{
			  base.XmlSpace = Constants.ATTRVAL_PRESERVE;
			}
			// else maybe it's a real AVT, so we can't resolve it at this time.
		  }
	  }

	  /// <summary>
	  /// Get a literal result attribute by name.
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of attribute node to get </param>
	  /// <param name="localName"> Local part of qualified name of attribute node to get
	  /// </param>
	  /// <returns> literal result attribute (AVT) </returns>
	  public virtual AVT getLiteralResultAttributeNS(string namespaceURI, string localName)
	  {

		if (null != m_avts)
		{
		  int nAttrs = m_avts.Count;

		  for (int i = (nAttrs - 1); i >= 0; i--)
		  {
			AVT avt = (AVT) m_avts[i];

			if (avt.Name.Equals(localName) && avt.URI.Equals(namespaceURI))
			{
			  return avt;
			}
		  } // end for
		}

		return null;
	  }

	  /// <summary>
	  /// Return the raw value of the attribute.
	  /// </summary>
	  /// <param name="namespaceURI"> Namespace URI of attribute node to get </param>
	  /// <param name="localName"> Local part of qualified name of attribute node to get
	  /// </param>
	  /// <returns> The Attr value as a string, or the empty string if that attribute 
	  /// does not have a specified or default value </returns>
	  public override string getAttributeNS(string namespaceURI, string localName)
	  {

		AVT avt = getLiteralResultAttributeNS(namespaceURI, localName);

		if ((null != avt))
		{
		  return avt.SimpleString;
		}

		return EMPTYSTRING;
	  }

	  /// <summary>
	  /// Get a literal result attribute by name. The name is namespaceURI:localname  
	  /// if namespace is not null.
	  /// </summary>
	  /// <param name="name"> Name of literal result attribute to get
	  /// </param>
	  /// <returns> literal result attribute (AVT) </returns>
	  public virtual AVT getLiteralResultAttribute(string name)
	  {

		if (null != m_avts)
		{
		  int nAttrs = m_avts.Count;
		  string @namespace = null;
		  for (int i = (nAttrs - 1); i >= 0; i--)
		  {
			AVT avt = (AVT) m_avts[i];
			@namespace = avt.URI;

			if ((!string.ReferenceEquals(@namespace, null) && (@namespace.Length != 0) && (@namespace + ":" + avt.Name).Equals(name)) || ((string.ReferenceEquals(@namespace, null) || @namespace.Length == 0) && avt.RawName.Equals(name)))
			{
			  return avt;
			}
		  } // end for
		}

		return null;
	  }

	  /// <summary>
	  /// Return the raw value of the attribute.
	  /// </summary>
	  /// <param name="namespaceURI">:localName or localName if the namespaceURI is null of 
	  /// the attribute to get
	  /// </param>
	  /// <returns> The Attr value as a string, or the empty string if that attribute 
	  /// does not have a specified or default value </returns>
	  public override string getAttribute(string rawName)
	  {

		AVT avt = getLiteralResultAttribute(rawName);

		if ((null != avt))
		{
		  return avt.SimpleString;
		}

		return EMPTYSTRING;
	  }

	  /// <summary>
	  /// Get whether or not the passed URL is flagged by
	  /// the "extension-element-prefixes" or "exclude-result-prefixes"
	  /// properties. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="prefix"> non-null reference to prefix that might be excluded.(not currently used) </param>
	  /// <param name="uri"> reference to namespace that prefix maps to
	  /// </param>
	  /// <returns> true if the prefix should normally be excluded. </returns>
	  public override bool containsExcludeResultPrefix(string prefix, string uri)
	  {
		if (string.ReferenceEquals(uri, null) || (null == m_excludeResultPrefixes && null == m_ExtensionElementURIs))
		{
		  return base.containsExcludeResultPrefix(prefix, uri);
		}

		if (prefix.Length == 0)
		{
		  prefix = Constants.ATTRVAL_DEFAULT_PREFIX;
		}

		// This loop is ok here because this code only runs during
		// stylesheet compile time.    
			if (m_excludeResultPrefixes != null)
			{
				for (int i = 0; i < m_excludeResultPrefixes.size(); i++)
				{
					if (uri.Equals(getNamespaceForPrefix(m_excludeResultPrefixes.elementAt(i))))
					{
						return true;
					}
				}
			}

			// JJK Bugzilla 1133: Also check locally-scoped extensions
		if (m_ExtensionElementURIs != null && m_ExtensionElementURIs.contains(uri))
		{
		   return true;
		}

			return base.containsExcludeResultPrefix(prefix, uri);
	  }

	  /// <summary>
	  /// Augment resolvePrefixTables, resolving the namespace aliases once
	  /// the superclass has resolved the tables.
	  /// </summary>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resolvePrefixTables() throws javax.xml.transform.TransformerException
	  public override void resolvePrefixTables()
	  {

		base.resolvePrefixTables();

		StylesheetRoot stylesheet = StylesheetRoot;

		if ((null != m_namespace) && (m_namespace.Length > 0))
		{
		  NamespaceAlias nsa = stylesheet.getNamespaceAliasComposed(m_namespace);

		  if (null != nsa)
		  {
			m_namespace = nsa.ResultNamespace;

			// String resultPrefix = nsa.getResultPrefix();
			string resultPrefix = nsa.StylesheetPrefix; // As per xsl WG, Mike Kay

			if ((null != resultPrefix) && (resultPrefix.Length > 0))
			{
			  m_rawName = resultPrefix + ":" + m_localName;
			}
			else
			{
			  m_rawName = m_localName;
			}
		  }
		}

		if (null != m_avts)
		{
		  int n = m_avts.Count;

		  for (int i = 0; i < n; i++)
		  {
			AVT avt = (AVT) m_avts[i];

			// Should this stuff be a method on AVT?
			string ns = avt.URI;

			if ((null != ns) && (ns.Length > 0))
			{
			  NamespaceAlias nsa = stylesheet.getNamespaceAliasComposed(m_namespace); // %REVIEW% ns?

			  if (null != nsa)
			  {
				string @namespace = nsa.ResultNamespace;

				// String resultPrefix = nsa.getResultPrefix();
				string resultPrefix = nsa.StylesheetPrefix; // As per XSL WG
				string rawName = avt.Name;

				if ((null != resultPrefix) && (resultPrefix.Length > 0))
				{
				  rawName = resultPrefix + ":" + rawName;
				}

				avt.URI = @namespace;
				avt.RawName = rawName;
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  /// Return whether we need to check namespace prefixes
	  /// against the exclude result prefixes or extensions lists.
	  /// Note that this will create a new prefix table if one
	  /// has not been created already.
	  /// 
	  /// NEEDSDOC ($objectName$) @return
	  /// </summary>
	  internal override bool needToCheckExclude()
	  {
		if (null == m_excludeResultPrefixes && null == PrefixTable && m_ExtensionElementURIs == null) // JJK Bugzilla 1133
		{
		  return false;
		}
		else
		{

		  // Create a new prefix table if one has not already been created.
		  if (null == PrefixTable)
		  {
			PrefixTable = new ArrayList();
		  }

		  return true;
		}
	  }

	  /// <summary>
	  /// The namespace of the element to be created.
	  /// @serial
	  /// </summary>
	  private string m_namespace;

	  /// <summary>
	  /// Set the namespace URI of the result element to be created.
	  /// Note that after resolvePrefixTables has been called, this will
	  /// return the aliased result namespace, not the original stylesheet
	  /// namespace.
	  /// </summary>
	  /// <param name="ns"> The Namespace URI, or the empty string if the
	  ///        element has no Namespace URI. </param>
	  public virtual string Namespace
	  {
		  set
		  {
			if (null == value) // defensive, shouldn't have to do this.
			{
			  value = "";
			}
			m_namespace = value;
		  }
		  get
		  {
			return m_namespace;
		  }
	  }


	  /// <summary>
	  /// The local name of the element to be created.
	  /// @serial
	  /// </summary>
	  private string m_localName;

	  /// <summary>
	  /// Set the local name of the LRE.
	  /// </summary>
	  /// <param name="localName"> The local name (without prefix) of the result element
	  ///                  to be created. </param>
	  public virtual string LocalName
	  {
		  set
		  {
			m_localName = value;
		  }
		  get
		  {
			return m_localName;
		  }
	  }


	  /// <summary>
	  /// The raw name of the element to be created.
	  /// @serial
	  /// </summary>
	  private string m_rawName;

	  /// <summary>
	  /// Set the raw name of the LRE.
	  /// </summary>
	  /// <param name="rawName"> The qualified name (with prefix), or the
	  ///        empty string if qualified names are not available. </param>
	  public virtual string RawName
	  {
		  set
		  {
			m_rawName = value;
		  }
		  get
		  {
			return m_rawName;
		  }
	  }


	 /// <summary>
	 /// Get the prefix part of the raw name of the Literal Result Element.
	 /// </summary>
	 /// <returns> The prefix, or the empty string if noprefix was provided. </returns>
	  public override string Prefix
	  {
		  get
		  {
				int len = m_rawName.Length - m_localName.Length - 1;
			return (len > 0) ? m_rawName.Substring(0,len) : "";
		  }
		  set
		  {
						  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
		  }
	  }


	  /// <summary>
	  /// The "extension-element-prefixes" property, actually contains URIs.
	  /// @serial
	  /// </summary>
	  private StringVector m_ExtensionElementURIs;

	  /// <summary>
	  /// Set the "extension-element-prefixes" property. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Vector of URIs (not prefixes) to set as the "extension-element-prefixes" property </param>
	  public virtual StringVector ExtensionElementPrefixes
	  {
		  set
		  {
			m_ExtensionElementURIs = value;
		  }
	  }

	  /// <seealso cref= org.w3c.dom.Node
	  /// </seealso>
	  /// <returns> NamedNodeMap </returns>
	  public override NamedNodeMap Attributes
	  {
		  get
		  {
				return new LiteralElementAttributes(this);
		  }
	  }

	  public class LiteralElementAttributes : NamedNodeMap
	  {
		  private readonly ElemLiteralResult outerInstance;

			  internal int m_count = -1;

			  /// <summary>
			  /// Construct a NameNodeMap.
			  /// 
			  /// </summary>
			  public LiteralElementAttributes(ElemLiteralResult outerInstance)
			  {
				  this.outerInstance = outerInstance;
			  }

			  /// <summary>
			  /// Return the number of Attributes on this Element
			  /// </summary>
			  /// <returns> The number of nodes in this map. The range of valid child 
			  /// node indices is <code>0</code> to <code>length-1</code> inclusive </returns>
			  public virtual int Length
			  {
				  get
				  {
					if (m_count == -1)
					{
					   if (null != outerInstance.m_avts)
					   {
						   m_count = outerInstance.m_avts.Count;
					   }
					   else
					   {
						   m_count = 0;
					   }
					}
					return m_count;
				  }
			  }

			  /// <summary>
			  /// Retrieves a node specified by name. </summary>
			  /// <param name="name"> The <code>nodeName</code> of a node to retrieve. </param>
			  /// <returns> A <code>Node</code> (of any type) with the specified
			  ///   <code>nodeName</code>, or <code>null</code> if it does not 
			  ///   identify any node in this map. </returns>
			  public virtual Node getNamedItem(string name)
			  {
					if (Length == 0)
					{
						return null;
					}
					string uri = null;
					string localName = name;
					int index = name.IndexOf(":", StringComparison.Ordinal);
					if (-1 != index)
					{
							 uri = name.Substring(0, index);
							 localName = name.Substring(index + 1);
					}
					Node retNode = null;
					IEnumerator eum = outerInstance.m_avts.GetEnumerator();
					while (eum.MoveNext())
					{
							AVT avt = (AVT) eum.Current;
							if (localName.Equals(avt.Name))
							{
							  string nsURI = avt.URI;
							  if ((string.ReferenceEquals(uri, null) && string.ReferenceEquals(nsURI, null)) || (!string.ReferenceEquals(uri, null) && uri.Equals(nsURI)))
							  {
								retNode = new Attribute(outerInstance, avt, outerInstance);
								break;
							  }
							}
					}
					return retNode;
			  }

			  /// <summary>
			  /// Retrieves a node specified by local name and namespace URI. </summary>
			  /// <param name="namespaceURI"> Namespace URI of attribute node to get </param>
			  /// <param name="localName"> Local part of qualified name of attribute node to 
			  /// get </param>
			  /// <returns> A <code>Node</code> (of any type) with the specified
			  ///   <code>nodeName</code>, or <code>null</code> if it does not 
			  ///   identify any node in this map. </returns>
			  public virtual Node getNamedItemNS(string namespaceURI, string localName)
			  {
					  if (Length == 0)
					  {
						  return null;
					  }
					  Node retNode = null;
					  IEnumerator eum = outerInstance.m_avts.GetEnumerator();
					  while (eum.MoveNext())
					  {
						AVT avt = (AVT) eum.Current;
						if (localName.Equals(avt.Name))
						{
						  string nsURI = avt.URI;
						  if ((string.ReferenceEquals(namespaceURI, null) && string.ReferenceEquals(nsURI, null)) || (!string.ReferenceEquals(namespaceURI, null) && namespaceURI.Equals(nsURI)))
						  {
							retNode = new Attribute(outerInstance, avt, outerInstance);
							break;
						  }
						}
					  }
					  return retNode;
			  }

			  /// <summary>
			  /// Returns the <code>index</code>th item in the map. If <code>index
			  /// </code> is greater than or equal to the number of nodes in this 
			  /// map, this returns <code>null</code>. </summary>
			  /// <param name="i"> The index of the requested item. </param>
			  /// <returns> The node at the <code>index</code>th position in the map, 
			  ///   or <code>null</code> if that is not a valid index. </returns>
			  public virtual Node item(int i)
			  {
					if (Length == 0 || i >= outerInstance.m_avts.Count)
					{
						return null;
					}
					else
					{
						return new Attribute(outerInstance, ((AVT)outerInstance.m_avts[i]), outerInstance);
					}
			  }

			  /// <seealso cref= org.w3c.dom.NamedNodeMap
			  /// </seealso>
			  /// <param name="name"> of the node to remove
			  /// </param>
			  /// <returns> The node removed from this map if a node with such 
			  /// a name exists. 
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node removeNamedItem(String name) throws org.w3c.dom.DOMException
			  public virtual Node removeNamedItem(string name)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }

			  /// <seealso cref= org.w3c.dom.NamedNodeMap
			  /// </seealso>
			  /// <param name="namespaceURI"> Namespace URI of the node to remove </param>
			  /// <param name="localName"> Local part of qualified name of the node to remove
			  /// </param>
			  /// <returns> The node removed from this map if a node with such a local
			  ///  name and namespace URI exists
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node removeNamedItemNS(String namespaceURI, String localName) throws org.w3c.dom.DOMException
			  public virtual Node removeNamedItemNS(string namespaceURI, string localName)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }

			  /// <summary>
			  /// Unimplemented. See org.w3c.dom.NamedNodeMap
			  /// </summary>
			  /// <param name="A"> node to store in this map
			  /// </param>
			  /// <returns> If the new Node replaces an existing node the replaced 
			  /// Node is returned, otherwise null is returned
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node setNamedItem(org.w3c.dom.Node arg) throws org.w3c.dom.DOMException
			  public virtual Node setNamedItem(Node arg)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }

			  /// <summary>
			  /// Unimplemented. See org.w3c.dom.NamedNodeMap
			  /// </summary>
			  /// <param name="A"> node to store in this map
			  /// </param>
			  /// <returns> If the new Node replaces an existing node the replaced 
			  /// Node is returned, otherwise null is returned
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node setNamedItemNS(org.w3c.dom.Node arg) throws org.w3c.dom.DOMException
			  public virtual Node setNamedItemNS(Node arg)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }
	  }

	  public class Attribute : Attr
	  {
		  private readonly ElemLiteralResult outerInstance;

			  internal AVT m_attribute;
			  internal Element m_owner = null;
			  /// <summary>
			  /// Construct a Attr.
			  /// 
			  /// </summary>
			  public Attribute(ElemLiteralResult outerInstance, AVT avt, Element elem)
			  {
				  this.outerInstance = outerInstance;
					m_attribute = avt;
					m_owner = elem;
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <param name="newChild"> New node to append to the list of this node's 
			  /// children
			  /// 
			  /// </param>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node appendChild(org.w3c.dom.Node newChild) throws org.w3c.dom.DOMException
			  public virtual Node appendChild(Node newChild)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <param name="deep"> Flag indicating whether to clone deep 
			  /// (clone member variables)
			  /// </param>
			  /// <returns> Returns a duplicate of this node </returns>
			  public virtual Node cloneNode(bool deep)
			  {
					  return new Attribute(outerInstance, m_attribute, m_owner);
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> null </returns>
			  public virtual NamedNodeMap Attributes
			  {
				  get
				  {
					return null;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> a NodeList containing no nodes.  </returns>
			  public virtual NodeList ChildNodes
			  {
				  get
				  {
						  return new NodeListAnonymousInnerClass(this);
				  }
			  }

			  private class NodeListAnonymousInnerClass : NodeList
			  {
				  private readonly Attribute outerInstance;

				  public NodeListAnonymousInnerClass(Attribute outerInstance)
				  {
					  this.outerInstance = outerInstance;
				  }

				  public virtual int Length
				  {
					  get
					  {
							  return 0;
					  }
				  }
				  public virtual Node item(int index)
				  {
						  return null;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> null </returns>
			  public virtual Node FirstChild
			  {
				  get
				  {
						  return null;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> null </returns>
			  public virtual Node LastChild
			  {
				  get
				  {
						  return null;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> the local part of the qualified name of this node </returns>
			  public virtual string LocalName
			  {
				  get
				  {
						  return m_attribute.Name;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> The namespace URI of this node, or null if it is 
			  /// unspecified </returns>
			  public virtual string NamespaceURI
			  {
				  get
				  {
						  string uri = m_attribute.URI;
						  return (uri.Length == 0)?null:uri;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> null </returns>
			  public virtual Node NextSibling
			  {
				  get
				  {
						return null;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> The name of the attribute </returns>
			  public virtual string NodeName
			  {
				  get
				  {
						  string uri = m_attribute.URI;
						  string localName = LocalName;
						  return (uri.Length == 0)?localName:uri + ":" + localName;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> The node is an Attr </returns>
			  public virtual short NodeType
			  {
				  get
				  {
						  return ATTRIBUTE_NODE;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> The value of the attribute
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getNodeValue() throws org.w3c.dom.DOMException
			  public virtual string NodeValue
			  {
				  get
				  {
						  return m_attribute.SimpleString;
				  }
				  set
				  {
						  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> null </returns>
			  public virtual Document OwnerDocument
			  {
				  get
				  {
					return m_owner.OwnerDocument;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> the containing element node </returns>
			  public virtual Node ParentNode
			  {
				  get
				  {
						  return m_owner;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> The namespace prefix of this node, or null if it is 
			  /// unspecified </returns>
			  public virtual string Prefix
			  {
				  get
				  {
						  string uri = m_attribute.URI;
						  string rawName = m_attribute.RawName;
						  return (uri.Length == 0)? null:rawName.Substring(0, rawName.IndexOf(":", StringComparison.Ordinal));
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> null </returns>
			  public virtual Node PreviousSibling
			  {
				  get
				  {
						  return null;
				  }
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> false </returns>
			  public virtual bool hasAttributes()
			  {
					  return false;
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> false </returns>
			  public virtual bool hasChildNodes()
			  {
					  return false;
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <param name="newChild"> New child node to insert </param>
			  /// <param name="refChild"> Insert in front of this child
			  /// </param>
			  /// <returns> null
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node insertBefore(org.w3c.dom.Node newChild, org.w3c.dom.Node refChild) throws org.w3c.dom.DOMException
			  public virtual Node insertBefore(Node newChild, Node refChild)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <returns> Returns <code>false</code>
			  /// @since DOM Level 2 </returns>
			  public virtual bool isSupported(string feature, string version)
			  {
				return false;
			  }

			  /// <seealso cref= org.w3c.dom.Node </seealso>
			  public virtual void normalize()
			  {
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <param name="oldChild"> Child to be removed
			  /// </param>
			  /// <returns> null
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node removeChild(org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
			  public virtual Node removeChild(Node oldChild)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }

			  /// <seealso cref= org.w3c.dom.Node
			  /// </seealso>
			  /// <param name="newChild"> Replace existing child with this one </param>
			  /// <param name="oldChild"> Existing child to be replaced
			  /// </param>
			  /// <returns> null
			  /// </returns>
			  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node replaceChild(org.w3c.dom.Node newChild, org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
			  public virtual Node replaceChild(Node newChild, Node oldChild)
			  {
					  outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
					  return null;
			  }



			  /// 
			  /// <returns> The name of this attribute </returns>
			  public virtual string Name
			  {
				  get
				  {
						  return m_attribute.Name;
				  }
			  }

			  /// 
			  /// <returns> The value of this attribute returned as string </returns>
			  public virtual string Value
			  {
				  get
				  {
						  return m_attribute.SimpleString;
				  }
				  set
				  {
					outerInstance.throwDOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR, XSLTErrorResources.NO_MODIFICATION_ALLOWED_ERR);
				  }
			  }

			  /// 
			  /// <returns> The Element node this attribute is attached to 
			  /// or null if this attribute is not in use </returns>
			  public virtual Element OwnerElement
			  {
				  get
				  {
						  return m_owner;
				  }
			  }

			  /// 
			  /// <returns> true </returns>
			  public virtual bool Specified
			  {
				  get
				  {
						  return true;
				  }
			  }


		   public virtual TypeInfo SchemaTypeInfo
		   {
			   get
			   {
				   return null;
			   }
		   }

			public virtual bool Id
			{
				get
				{
					return false;
				}
			}

			public virtual object setUserData(string key, object data, UserDataHandler handler)
			{
				return OwnerDocument.setUserData(key, data, handler);
			}

			public virtual object getUserData(string key)
			{
				return OwnerDocument.getUserData(key);
			}

			public virtual object getFeature(string feature, string version)
			{
				return isSupported(feature, version) ? this : null;
			}

			  public virtual bool isEqualNode(Node arg)
			  {
				  return arg == this;
			  }

			  public virtual string lookupNamespaceURI(string specifiedPrefix)
			  {
					 return null;
			  }

			  public virtual bool isDefaultNamespace(string namespaceURI)
			  {
					return false;
			  }

		  public virtual string lookupPrefix(string namespaceURI)
		  {
				return null;
		  }

			public virtual bool isSameNode(Node other)
			{
				// we do not use any wrapper so the answer is obvious
				return this == other;
			}

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
						return NodeValue; // overriden in some subclasses
				}
			}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public short compareDocumentPosition(org.w3c.dom.Node other) throws org.w3c.dom.DOMException
			  public virtual short compareDocumentPosition(Node other)
			  {
					return 0;
			  }

			  public virtual string BaseURI
			  {
				  get
				  {
						return null;
				  }
			  }
	  }

	  /// <summary>
	  /// Get an "extension-element-prefix" property. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="i"> Index of URI ("extension-element-prefix" property) to get
	  /// </param>
	  /// <returns> URI at given index ("extension-element-prefix" property)
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getExtensionElementPrefix(int i) throws ArrayIndexOutOfBoundsException
	  public virtual string getExtensionElementPrefix(int i)
	  {

		if (null == m_ExtensionElementURIs)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return m_ExtensionElementURIs.elementAt(i);
	  }

	  /// <summary>
	  /// Get the number of "extension-element-prefixes" Strings. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	  /// </seealso>
	  /// <returns> the number of "extension-element-prefixes" Strings </returns>
	  public virtual int ExtensionElementPrefixCount
	  {
		  get
		  {
			return (null != m_ExtensionElementURIs) ? m_ExtensionElementURIs.size() : 0;
		  }
	  }

	  /// <summary>
	  /// Find out if the given "extension-element-prefix" property is defined. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="uri"> The URI to find
	  /// </param>
	  /// <returns> True if the given URI is found </returns>
	  public virtual bool containsExtensionElementURI(string uri)
	  {

		if (null == m_ExtensionElementURIs)
		{
		  return false;
		}

		return m_ExtensionElementURIs.contains(uri);
	  }

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_LITERALRESULT;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
    
			// TODO: Need prefix.
			return m_rawName;
		  }
	  }

	  /// <summary>
	  /// The XSLT version as specified by this element.
	  /// @serial
	  /// </summary>
	  private string m_version;

	  /// <summary>
	  /// Set the "version" property. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#forwards">forwards in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Version property value to set </param>
	  public virtual string Version
	  {
		  set
		  {
			m_version = value;
		  }
		  get
		  {
			return m_version;
		  }
	  }


	  /// <summary>
	  /// The "exclude-result-prefixes" property.
	  /// @serial
	  /// </summary>
	  private StringVector m_excludeResultPrefixes;

	  /// <summary>
	  /// Set the "exclude-result-prefixes" property.
	  /// The designation of a namespace as an excluded namespace is
	  /// effective within the subtree of the stylesheet rooted at
	  /// the element bearing the exclude-result-prefixes or
	  /// xsl:exclude-result-prefixes attribute; a subtree rooted
	  /// at an xsl:stylesheet element does not include any stylesheets
	  /// imported or included by children of that xsl:stylesheet element. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#literal-result-element">literal-result-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> vector of prefixes that are resolvable to strings. </param>
	  public virtual StringVector ExcludeResultPrefixes
	  {
		  set
		  {
			m_excludeResultPrefixes = value;
		  }
	  }

	  /// <summary>
	  /// Tell if the result namespace decl should be excluded.  Should be called before
	  /// namespace aliasing (I think).
	  /// </summary>
	  /// <param name="prefix"> Prefix of namespace to check </param>
	  /// <param name="uri"> URI of namespace to check
	  /// </param>
	  /// <returns> True if the given namespace should be excluded
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean excludeResultNSDecl(String prefix, String uri) throws javax.xml.transform.TransformerException
	  private bool excludeResultNSDecl(string prefix, string uri)
	  {

		if (null != m_excludeResultPrefixes)
		{
		  return containsExcludeResultPrefix(prefix, uri);
		}

		return false;
	  }

	  /// <summary>
	  /// Copy a Literal Result Element into the Result tree, copy the
	  /// non-excluded namespace attributes, copy the attributes not
	  /// of the XSLT namespace, and execute the children of the LRE. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#literal-result-element">literal-result-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
		public override void execute(TransformerImpl transformer)
		{
			SerializationHandler rhandler = transformer.SerializationHandler;

			try
			{
				if (transformer.Debug)
				{
					// flush any buffered pending processing before
					// the trace event.
					rhandler.flushPending();
					transformer.TraceManager.fireTraceEvent(this);
				}

				// JJK Bugzilla 3464, test namespace85 -- make sure LRE's
				// namespace is asserted even if default, since xsl:element
				// may have changed the context.
				rhandler.startPrefixMapping(Prefix, Namespace);

				// Add namespace declarations.
				executeNSDecls(transformer);
				rhandler.startElement(Namespace, LocalName, RawName);
			}
			catch (SAXException se)
			{
				throw new TransformerException(se);
			}

			/*
			 * If we make it to here we have done a successful startElement()
			 * we will do an endElement() call for balance, no matter what happens
			 * in the middle.  
			 */

			// tException remembers if we had an exception "in the middle"
			TransformerException tException = null;
			try
			{

				// Process any possible attributes from xsl:use-attribute-sets first
				base.execute(transformer);

				//xsl:version, excludeResultPrefixes???
				// Process the list of avts next
				if (null != m_avts)
				{
					int nAttrs = m_avts.Count;

					for (int i = (nAttrs - 1); i >= 0; i--)
					{
						AVT avt = (AVT) m_avts[i];
						XPathContext xctxt = transformer.XPathContext;
						int sourceNode = xctxt.CurrentNode;
						string stringedValue = avt.evaluate(xctxt, sourceNode, this);

						if (null != stringedValue)
						{

							// Important Note: I'm not going to check for excluded namespace 
							// prefixes here.  It seems like it's too expensive, and I'm not 
							// even sure this is right.  But I could be wrong, so this needs 
							// to be tested against other implementations.

							rhandler.addAttribute(avt.URI, avt.Name, avt.RawName, "CDATA", stringedValue, false);
						}
					} // end for
				}

				// Now process all the elements in this subtree
				// TODO: Process m_extensionElementPrefixes && m_attributeSetsNames
				transformer.executeChildTemplates(this, true);
			}
			catch (TransformerException te)
			{
				// thrown in finally to prevent original exception consumed by subsequent exceptions
				tException = te;
			}
			catch (SAXException se)
			{
				tException = new TransformerException(se);
			}

			try
			{
				/* we need to do this endElement() to balance the
				 * successful startElement() call even if 
				 * there was an exception in the middle.
				 * Otherwise an exception in the middle could cause a system to hang.
				 */
				if (transformer.Debug)
				{
					// flush any buffered pending processing before
					// the trace event.
					//rhandler.flushPending();
					transformer.TraceManager.fireTraceEndEvent(this);
				}
				rhandler.endElement(Namespace, LocalName, RawName);
			}
			catch (SAXException se)
			{
				/* we did call endElement(). If thee was an exception
				 * in the middle throw that one, otherwise if there
				 * was an exception from endElement() throw that one.
				 */
				if (tException != null)
				{
					throw tException;
				}
				else
				{
					throw new TransformerException(se);
				}
			}

			/* If an exception was thrown in the middle but not with startElement() or
			 * or endElement() then its time to let it percolate.
			 */ 
			if (tException != null)
			{
				throw tException;
			}

			unexecuteNSDecls(transformer);

			// JJK Bugzilla 3464, test namespace85 -- balance explicit start.
			try
			{
				rhandler.endPrefixMapping(Prefix);
			}
			catch (SAXException se)
			{
				throw new TransformerException(se);
			}
		}

	  /// <summary>
	  /// Compiling templates requires that we be able to list the AVTs
	  /// ADDED 9/5/2000 to support compilation experiment
	  /// </summary>
	  /// <returns> an Enumeration of the literal result attributes associated
	  /// with this element. </returns>
	  public virtual IEnumerator enumerateLiteralResultAttributes()
	  {
		return (null == m_avts) ? null : m_avts.GetEnumerator();
	  }

		/// <summary>
		/// Accept a visitor and call the appropriate method 
		/// for this class.
		/// </summary>
		/// <param name="visitor"> The visitor whose appropriate method will be called. </param>
		/// <returns> true if the children of the object should be visited. </returns>
		protected internal override bool accept(XSLTVisitor visitor)
		{
		  return visitor.visitLiteralResultElement(this);
		}

		/// <summary>
		/// Call the children visitors. </summary>
		/// <param name="visitor"> The visitor whose appropriate method will be called. </param>
		protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
		{
		  if (callAttrs && null != m_avts)
		  {
			int nAttrs = m_avts.Count;

			for (int i = (nAttrs - 1); i >= 0; i--)
			{
			  AVT avt = (AVT) m_avts[i];
			  avt.callVisitors(visitor);
			}
		  }
		  base.callChildVisitors(visitor, callAttrs);
		}

		/// <summary>
		/// Throw a DOMException
		/// </summary>
		/// <param name="msg"> key of the error that occured. </param>
		public virtual void throwDOMException(short code, string msg)
		{

		  string themsg = XSLMessages.createMessage(msg, null);

		  throw new DOMException(code, themsg);
		}

	}

}