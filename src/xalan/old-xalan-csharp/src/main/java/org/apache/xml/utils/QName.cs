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
 * $Id: QName.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{


	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using Element = org.w3c.dom.Element;

	/// <summary>
	/// Class to represent a qualified name: "The name of an internal XSLT object,
	/// specifically a named template (see [7 Named Templates]), a mode (see [6.7 Modes]),
	/// an attribute set (see [8.1.4 Named Attribute Sets]), a key (see [14.2 Keys]),
	/// a locale (see [14.3 Number Formatting]), a variable or a parameter (see
	/// [12 Variables and Parameters]) is specified as a QName. If it has a prefix,
	/// then the prefix is expanded into a URI reference using the namespace declarations
	/// in effect on the attribute in which the name occurs. The expanded name
	/// consisting of the local part of the name and the possibly null URI reference
	/// is used as the name of the object. The default namespace is not used for
	/// unprefixed names."
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class QName
	{
		internal const long serialVersionUID = 467434581652829920L;

	  /// <summary>
	  /// The local name.
	  /// @serial
	  /// </summary>
	  protected internal string _localName;

	  /// <summary>
	  /// The namespace URI.
	  /// @serial
	  /// </summary>
	  protected internal string _namespaceURI;

	  /// <summary>
	  /// The namespace prefix.
	  /// @serial
	  /// </summary>
	  protected internal string _prefix;

	  /// <summary>
	  /// The XML namespace.
	  /// </summary>
	  public const string S_XMLNAMESPACEURI = "http://www.w3.org/XML/1998/namespace";

	  /// <summary>
	  /// The cached hashcode, which is calculated at construction time.
	  /// @serial
	  /// </summary>
	  private int m_hashCode;

	  /// <summary>
	  /// Constructs an empty QName.
	  /// 20001019: Try making this public, to support Serializable? -- JKESS
	  /// </summary>
	  public QName()
	  {
	  }

	  /// <summary>
	  /// Constructs a new QName with the specified namespace URI and
	  /// local name.
	  /// </summary>
	  /// <param name="namespaceURI"> The namespace URI if known, or null </param>
	  /// <param name="localName"> The local name </param>
	  public QName(string namespaceURI, string localName) : this(namespaceURI, localName, false)
	  {
	  }

	  /// <summary>
	  /// Constructs a new QName with the specified namespace URI and
	  /// local name.
	  /// </summary>
	  /// <param name="namespaceURI"> The namespace URI if known, or null </param>
	  /// <param name="localName"> The local name </param>
	  /// <param name="validate"> If true the new QName will be validated and an IllegalArgumentException will
	  ///                 be thrown if it is invalid. </param>
	  public QName(string namespaceURI, string localName, bool validate)
	  {

		// This check was already here.  So, for now, I will not add it to the validation
		// that is done when the validate parameter is true.
		if (string.ReferenceEquals(localName, null))
		{
		  throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_NULL, null)); //"Argument 'localName' is null");
		}

		if (validate)
		{
			if (!XML11Char.isXML11ValidNCName(localName))
			{
				throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_INVALID,null)); //"Argument 'localName' not a valid NCName");
			}
		}

		_namespaceURI = namespaceURI;
		_localName = localName;
		m_hashCode = ToString().GetHashCode();
	  }

	  /// <summary>
	  /// Constructs a new QName with the specified namespace URI, prefix
	  /// and local name.
	  /// </summary>
	  /// <param name="namespaceURI"> The namespace URI if known, or null </param>
	  /// <param name="prefix"> The namespace prefix is known, or null </param>
	  /// <param name="localName"> The local name
	  ///  </param>
	  public QName(string namespaceURI, string prefix, string localName) : this(namespaceURI, prefix, localName, false)
	  {
	  }

	 /// <summary>
	 /// Constructs a new QName with the specified namespace URI, prefix
	 /// and local name.
	 /// </summary>
	 /// <param name="namespaceURI"> The namespace URI if known, or null </param>
	 /// <param name="prefix"> The namespace prefix is known, or null </param>
	 /// <param name="localName"> The local name </param>
	 /// <param name="validate"> If true the new QName will be validated and an IllegalArgumentException will
	 ///                 be thrown if it is invalid. </param>
	  public QName(string namespaceURI, string prefix, string localName, bool validate)
	  {

		// This check was already here.  So, for now, I will not add it to the validation
		// that is done when the validate parameter is true.
		if (string.ReferenceEquals(localName, null))
		{
		  throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_NULL, null)); //"Argument 'localName' is null");
		}

		if (validate)
		{
			if (!XML11Char.isXML11ValidNCName(localName))
			{
				throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_INVALID,null)); //"Argument 'localName' not a valid NCName");
			}

			if ((null != prefix) && (!XML11Char.isXML11ValidNCName(prefix)))
			{
				throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_PREFIX_INVALID,null)); //"Argument 'prefix' not a valid NCName");
			}

		}
		_namespaceURI = namespaceURI;
		_prefix = prefix;
		_localName = localName;
		m_hashCode = ToString().GetHashCode();
	  }

	  /// <summary>
	  /// Construct a QName from a string, without namespace resolution.  Good
	  /// for a few odd cases.
	  /// </summary>
	  /// <param name="localName"> Local part of qualified name
	  ///  </param>
	  public QName(string localName) : this(localName, false)
	  {
	  }

	  /// <summary>
	  /// Construct a QName from a string, without namespace resolution.  Good
	  /// for a few odd cases.
	  /// </summary>
	  /// <param name="localName"> Local part of qualified name </param>
	  /// <param name="validate"> If true the new QName will be validated and an IllegalArgumentException will
	  ///                 be thrown if it is invalid. </param>
	  public QName(string localName, bool validate)
	  {

		// This check was already here.  So, for now, I will not add it to the validation
		// that is done when the validate parameter is true.
		if (string.ReferenceEquals(localName, null))
		{
		  throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_NULL, null)); //"Argument 'localName' is null");
		}

		if (validate)
		{
			if (!XML11Char.isXML11ValidNCName(localName))
			{
				throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_INVALID,null)); //"Argument 'localName' not a valid NCName");
			}
		}
		_namespaceURI = null;
		_localName = localName;
		m_hashCode = ToString().GetHashCode();
	  }

	  /// <summary>
	  /// Construct a QName from a string, resolving the prefix
	  /// using the given namespace stack. The default namespace is
	  /// not resolved.
	  /// </summary>
	  /// <param name="qname"> Qualified name to resolve </param>
	  /// <param name="namespaces"> Namespace stack to use to resolve namespace </param>
	  public QName(string qname, Stack namespaces) : this(qname, namespaces, false)
	  {
	  }

	  /// <summary>
	  /// Construct a QName from a string, resolving the prefix
	  /// using the given namespace stack. The default namespace is
	  /// not resolved.
	  /// </summary>
	  /// <param name="qname"> Qualified name to resolve </param>
	  /// <param name="namespaces"> Namespace stack to use to resolve namespace </param>
	  /// <param name="validate"> If true the new QName will be validated and an IllegalArgumentException will
	  ///                 be thrown if it is invalid. </param>
	  public QName(string qname, Stack namespaces, bool validate)
	  {

		string @namespace = null;
		string prefix = null;
		int indexOfNSSep = qname.IndexOf(':');

		if (indexOfNSSep > 0)
		{
		  prefix = qname.Substring(0, indexOfNSSep);

		  if (prefix.Equals("xml"))
		  {
			@namespace = S_XMLNAMESPACEURI;
		  }
		  // Do we want this?
		  else if (prefix.Equals("xmlns"))
		  {
			return;
		  }
		  else
		  {
			int depth = namespaces.Count;

			for (int i = depth - 1; i >= 0; i--)
			{
			  NameSpace ns = (NameSpace) namespaces.elementAt(i);

			  while (null != ns)
			  {
				if ((null != ns.m_prefix) && prefix.Equals(ns.m_prefix))
				{
				  @namespace = ns.m_uri;
				  i = -1;

				  break;
				}

				ns = ns.m_next;
			  }
			}
		  }

		  if (null == @namespace)
		  {
			throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_PREFIX_MUST_RESOLVE, new object[]{prefix})); //"Prefix must resolve to a namespace: "+prefix);
		  }
		}

		_localName = (indexOfNSSep < 0) ? qname : qname.Substring(indexOfNSSep + 1);

		if (validate)
		{
			if ((string.ReferenceEquals(_localName, null)) || (!XML11Char.isXML11ValidNCName(_localName)))
			{
			   throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_INVALID,null)); //"Argument 'localName' not a valid NCName");
			}
		}
		_namespaceURI = @namespace;
		_prefix = prefix;
		m_hashCode = ToString().GetHashCode();
	  }

	  /// <summary>
	  /// Construct a QName from a string, resolving the prefix
	  /// using the given namespace context and prefix resolver. 
	  /// The default namespace is not resolved.
	  /// </summary>
	  /// <param name="qname"> Qualified name to resolve </param>
	  /// <param name="namespaceContext"> Namespace Context to use </param>
	  /// <param name="resolver"> Prefix resolver for this context </param>
	  public QName(string qname, Element namespaceContext, PrefixResolver resolver) : this(qname, namespaceContext, resolver, false)
	  {
	  }

	  /// <summary>
	  /// Construct a QName from a string, resolving the prefix
	  /// using the given namespace context and prefix resolver. 
	  /// The default namespace is not resolved.
	  /// </summary>
	  /// <param name="qname"> Qualified name to resolve </param>
	  /// <param name="namespaceContext"> Namespace Context to use </param>
	  /// <param name="resolver"> Prefix resolver for this context </param>
	  /// <param name="validate"> If true the new QName will be validated and an IllegalArgumentException will
	  ///                 be thrown if it is invalid. </param>
	  public QName(string qname, Element namespaceContext, PrefixResolver resolver, bool validate)
	  {

		_namespaceURI = null;

		int indexOfNSSep = qname.IndexOf(':');

		if (indexOfNSSep > 0)
		{
		  if (null != namespaceContext)
		  {
			string prefix = qname.Substring(0, indexOfNSSep);

			_prefix = prefix;

			if (prefix.Equals("xml"))
			{
			  _namespaceURI = S_XMLNAMESPACEURI;
			}

			// Do we want this?
			else if (prefix.Equals("xmlns"))
			{
			  return;
			}
			else
			{
			  _namespaceURI = resolver.getNamespaceForPrefix(prefix, namespaceContext);
			}

			if (null == _namespaceURI)
			{
			  throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_PREFIX_MUST_RESOLVE, new object[]{prefix})); //"Prefix must resolve to a namespace: "+prefix);
			}
		  }
		  else
		  {

			// TODO: error or warning...
		  }
		}

		_localName = (indexOfNSSep < 0) ? qname : qname.Substring(indexOfNSSep + 1);

		if (validate)
		{
			if ((string.ReferenceEquals(_localName, null)) || (!XML11Char.isXML11ValidNCName(_localName)))
			{
			   throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_INVALID,null)); //"Argument 'localName' not a valid NCName");
			}
		}

		m_hashCode = ToString().GetHashCode();
	  }


	  /// <summary>
	  /// Construct a QName from a string, resolving the prefix
	  /// using the given namespace stack. The default namespace is
	  /// not resolved.
	  /// </summary>
	  /// <param name="qname"> Qualified name to resolve </param>
	  /// <param name="resolver"> Prefix resolver for this context </param>
	  public QName(string qname, PrefixResolver resolver) : this(qname, resolver, false)
	  {
	  }

	  /// <summary>
	  /// Construct a QName from a string, resolving the prefix
	  /// using the given namespace stack. The default namespace is
	  /// not resolved.
	  /// </summary>
	  /// <param name="qname"> Qualified name to resolve </param>
	  /// <param name="resolver"> Prefix resolver for this context </param>
	  /// <param name="validate"> If true the new QName will be validated and an IllegalArgumentException will
	  ///                 be thrown if it is invalid. </param>
	  public QName(string qname, PrefixResolver resolver, bool validate)
	  {

		string prefix = null;
		_namespaceURI = null;

		int indexOfNSSep = qname.IndexOf(':');

		if (indexOfNSSep > 0)
		{
		  prefix = qname.Substring(0, indexOfNSSep);

		  if (prefix.Equals("xml"))
		  {
			_namespaceURI = S_XMLNAMESPACEURI;
		  }
		  else
		  {
			_namespaceURI = resolver.getNamespaceForPrefix(prefix);
		  }

		  if (null == _namespaceURI)
		  {
			throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_PREFIX_MUST_RESOLVE, new object[]{prefix})); //"Prefix must resolve to a namespace: "+prefix);
		  }
		  _localName = qname.Substring(indexOfNSSep + 1);
		}
		else if (indexOfNSSep == 0)
		{
		  throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_NAME_CANT_START_WITH_COLON, null));
		}
		else
		{
		  _localName = qname;
		}

		if (validate)
		{
			if ((string.ReferenceEquals(_localName, null)) || (!XML11Char.isXML11ValidNCName(_localName)))
			{
			   throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ARG_LOCALNAME_INVALID,null)); //"Argument 'localName' not a valid NCName");
			}
		}


		m_hashCode = ToString().GetHashCode();
		_prefix = prefix;
	  }

	  /// <summary>
	  /// Returns the namespace URI. Returns null if the namespace URI
	  /// is not known.
	  /// </summary>
	  /// <returns> The namespace URI, or null </returns>
	  public virtual string NamespaceURI
	  {
		  get
		  {
			return _namespaceURI;
		  }
	  }

	  /// <summary>
	  /// Returns the namespace prefix. Returns null if the namespace
	  /// prefix is not known.
	  /// </summary>
	  /// <returns> The namespace prefix, or null </returns>
	  public virtual string Prefix
	  {
		  get
		  {
			return _prefix;
		  }
	  }

	  /// <summary>
	  /// Returns the local part of the qualified name.
	  /// </summary>
	  /// <returns> The local part of the qualified name </returns>
	  public virtual string LocalName
	  {
		  get
		  {
			return _localName;
		  }
	  }

	  /// <summary>
	  /// Return the string representation of the qualified name, using the 
	  /// prefix if available, or the '{ns}foo' notation if not. Performs
	  /// string concatenation, so beware of performance issues.
	  /// </summary>
	  /// <returns> the string representation of the namespace </returns>
	  public override string ToString()
	  {

		return !string.ReferenceEquals(_prefix, null) ? (_prefix + ":" + _localName) : (!string.ReferenceEquals(_namespaceURI, null) ? ("{" + _namespaceURI + "}" + _localName) : _localName);
	  }

	  /// <summary>
	  /// Return the string representation of the qualified name using the 
	  /// the '{ns}foo' notation. Performs
	  /// string concatenation, so beware of performance issues.
	  /// </summary>
	  /// <returns> the string representation of the namespace </returns>
	  public virtual string toNamespacedString()
	  {

		return (!string.ReferenceEquals(_namespaceURI, null) ? ("{" + _namespaceURI + "}" + _localName) : _localName);
	  }


	  /// <summary>
	  /// Get the namespace of the qualified name.
	  /// </summary>
	  /// <returns> the namespace URI of the qualified name </returns>
	  public virtual string Namespace
	  {
		  get
		  {
			return NamespaceURI;
		  }
	  }

	  /// <summary>
	  /// Get the local part of the qualified name.
	  /// </summary>
	  /// <returns> the local part of the qualified name </returns>
	  public virtual string LocalPart
	  {
		  get
		  {
			return LocalName;
		  }
	  }

	  /// <summary>
	  /// Return the cached hashcode of the qualified name.
	  /// </summary>
	  /// <returns> the cached hashcode of the qualified name </returns>
	  public override int GetHashCode()
	  {
		return m_hashCode;
	  }

	  /// <summary>
	  /// Override equals and agree that we're equal if
	  /// the passed object is a string and it matches
	  /// the name of the arg.
	  /// </summary>
	  /// <param name="ns"> Namespace URI to compare to </param>
	  /// <param name="localPart"> Local part of qualified name to compare to 
	  /// </param>
	  /// <returns> True if the local name and uri match  </returns>
	  public virtual bool Equals(string ns, string localPart)
	  {

		string thisnamespace = NamespaceURI;

		return LocalName.Equals(localPart) && (((null != thisnamespace) && (null != ns)) ? thisnamespace.Equals(ns) : ((null == thisnamespace) && (null == ns)));
	  }

	  /// <summary>
	  /// Override equals and agree that we're equal if
	  /// the passed object is a QName and it matches
	  /// the name of the arg.
	  /// </summary>
	  /// <returns> True if the qualified names are equal </returns>
	  public override bool Equals(object @object)
	  {

		if (@object == this)
		{
		  return true;
		}

		if (@object is QName)
		{
		  QName qname = (QName) @object;
		  string thisnamespace = NamespaceURI;
		  string thatnamespace = qname.NamespaceURI;

		  return LocalName.Equals(qname.LocalName) && (((null != thisnamespace) && (null != thatnamespace)) ? thisnamespace.Equals(thatnamespace) : ((null == thisnamespace) && (null == thatnamespace)));
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Given a string, create and return a QName object  
	  /// 
	  /// </summary>
	  /// <param name="name"> String to use to create QName
	  /// </param>
	  /// <returns> a QName object </returns>
	  public static QName getQNameFromString(string name)
	  {

		StringTokenizer tokenizer = new StringTokenizer(name, "{}", false);
		QName qname;
		string s1 = tokenizer.nextToken();
		string s2 = tokenizer.hasMoreTokens() ? tokenizer.nextToken() : null;

		if (null == s2)
		{
		  qname = new QName(null, s1);
		}
		else
		{
		  qname = new QName(s1, s2);
		}

		return qname;
	  }

	  /// <summary>
	  /// This function tells if a raw attribute name is a
	  /// xmlns attribute.
	  /// </summary>
	  /// <param name="attRawName"> Raw name of attribute
	  /// </param>
	  /// <returns> True if the attribute starts with or is equal to xmlns  </returns>
	  public static bool isXMLNSDecl(string attRawName)
	  {

		return (attRawName.StartsWith("xmlns", StringComparison.Ordinal) && (attRawName.Equals("xmlns") || attRawName.StartsWith("xmlns:", StringComparison.Ordinal)));
	  }

	  /// <summary>
	  /// This function tells if a raw attribute name is a
	  /// xmlns attribute.
	  /// </summary>
	  /// <param name="attRawName"> Raw name of attribute
	  /// </param>
	  /// <returns> Prefix of attribute </returns>
	  public static string getPrefixFromXMLNSDecl(string attRawName)
	  {

		int index = attRawName.IndexOf(':');

		return (index >= 0) ? attRawName.Substring(index + 1) : "";
	  }

	  /// <summary>
	  /// Returns the local name of the given node.
	  /// </summary>
	  /// <param name="qname"> Input name
	  /// </param>
	  /// <returns> Local part of the name if prefixed, or the given name if not </returns>
	  public static string getLocalPart(string qname)
	  {

		int index = qname.IndexOf(':');

		return (index < 0) ? qname : qname.Substring(index + 1);
	  }

	  /// <summary>
	  /// Returns the local name of the given node.
	  /// </summary>
	  /// <param name="qname"> Input name 
	  /// </param>
	  /// <returns> Prefix of name or empty string if none there    </returns>
	  public static string getPrefixPart(string qname)
	  {

		int index = qname.IndexOf(':');

		return (index >= 0) ? qname.Substring(0, index) : "";
	  }
	}

}