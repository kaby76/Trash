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
 * $Id: XSLTElementDef.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{


	using Constants = org.apache.xalan.templates.Constants;
	using QName = org.apache.xml.utils.QName;

	/// <summary>
	/// This class defines the allowed structure for an element in a XSLT stylesheet,
	/// is meant to reflect the structure defined in http://www.w3.org/TR/xslt#dtd, and the
	/// mapping between Xalan classes and the markup elements in the XSLT instance.
	/// This actually represents both text nodes and elements.
	/// </summary>
	public class XSLTElementDef
	{

	  /// <summary>
	  /// Construct an instance of XSLTElementDef.  This must be followed by a
	  /// call to build().
	  /// </summary>
	  internal XSLTElementDef()
	  {
	  }

	  /// <summary>
	  /// Construct an instance of XSLTElementDef.
	  /// </summary>
	  /// <param name="namespace">  The Namespace URI, "*", or null. </param>
	  /// <param name="name"> The local name (without prefix), "*", or null. </param>
	  /// <param name="nameAlias"> A potential alias for the name, or null. </param>
	  /// <param name="elements"> An array of allowed child element defs, or null. </param>
	  /// <param name="attributes"> An array of allowed attribute defs, or null. </param>
	  /// <param name="contentHandler"> The element processor for this element. </param>
	  /// <param name="classObject"> The class of the object that this element def should produce. </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject)
	  {
		build(@namespace, name, nameAlias, elements, attributes, contentHandler, classObject);
		if ((null != @namespace) && (@namespace.Equals(Constants.S_XSLNAMESPACEURL) || @namespace.Equals(Constants.S_BUILTIN_EXTENSIONS_URL) || @namespace.Equals(Constants.S_BUILTIN_OLD_EXTENSIONS_URL)))
		{
		  schema.addAvailableElement(new QName(@namespace, name));
		  if (null != nameAlias)
		  {
			schema.addAvailableElement(new QName(@namespace, nameAlias));
		  }
		}
	  }

		/// <summary>
		/// Construct an instance of XSLTElementDef.
		/// </summary>
		/// <param name="namespace">  The Namespace URI, "*", or null. </param>
		/// <param name="name"> The local name (without prefix), "*", or null. </param>
		/// <param name="nameAlias"> A potential alias for the name, or null. </param>
		/// <param name="elements"> An array of allowed child element defs, or null. </param>
		/// <param name="attributes"> An array of allowed attribute defs, or null. </param>
		/// <param name="contentHandler"> The element processor for this element. </param>
		/// <param name="classObject"> The class of the object that this element def should produce. </param>
		/// <param name="has_required"> true if this element has required elements by the XSLT specification. </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject, bool has_required)
	  {
			this.m_has_required = has_required;
		build(@namespace, name, nameAlias, elements, attributes, contentHandler, classObject);
		if ((null != @namespace) && (@namespace.Equals(Constants.S_XSLNAMESPACEURL) || @namespace.Equals(Constants.S_BUILTIN_EXTENSIONS_URL) || @namespace.Equals(Constants.S_BUILTIN_OLD_EXTENSIONS_URL)))
		{
		  schema.addAvailableElement(new QName(@namespace, name));
		  if (null != nameAlias)
		  {
			schema.addAvailableElement(new QName(@namespace, nameAlias));
		  }
		}

	  }

		/// <summary>
		/// Construct an instance of XSLTElementDef.
		/// </summary>
		/// <param name="namespace">  The Namespace URI, "*", or null. </param>
		/// <param name="name"> The local name (without prefix), "*", or null. </param>
		/// <param name="nameAlias"> A potential alias for the name, or null. </param>
		/// <param name="elements"> An array of allowed child element defs, or null. </param>
		/// <param name="attributes"> An array of allowed attribute defs, or null. </param>
		/// <param name="contentHandler"> The element processor for this element. </param>
		/// <param name="classObject"> The class of the object that this element def should produce. </param>
		/// <param name="has_required"> true if this element has required elements by the XSLT specification. </param>
		/// <param name="required"> true if this element is required by the XSLT specification. </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject, bool has_required, bool required) : this(schema, @namespace, name, nameAlias, elements, attributes, contentHandler, classObject, has_required)
	  {
			this.m_required = required;
	  }

		/// <summary>
		/// Construct an instance of XSLTElementDef.
		/// </summary>
		/// <param name="namespace">  The Namespace URI, "*", or null. </param>
		/// <param name="name"> The local name (without prefix), "*", or null. </param>
		/// <param name="nameAlias"> A potential alias for the name, or null. </param>
		/// <param name="elements"> An array of allowed child element defs, or null. </param>
		/// <param name="attributes"> An array of allowed attribute defs, or null. </param>
		/// <param name="contentHandler"> The element processor for this element. </param>
		/// <param name="classObject"> The class of the object that this element def should produce. </param>
		/// <param name="has_required"> true if this element has required elements by the XSLT specification. </param>
		/// <param name="required"> true if this element is required by the XSLT specification. </param>
		/// <param name="order"> the order this element should appear according to the XSLT specification. </param>
		/// <param name="multiAllowed"> whether this element is allowed more than once </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject, bool has_required, bool required, int order, bool multiAllowed) : this(schema, @namespace, name, nameAlias, elements, attributes, contentHandler, classObject, has_required, required)
	  {
			this.m_order = order;
			this.m_multiAllowed = multiAllowed;
	  }

		/// <summary>
		/// Construct an instance of XSLTElementDef.
		/// </summary>
		/// <param name="namespace">  The Namespace URI, "*", or null. </param>
		/// <param name="name"> The local name (without prefix), "*", or null. </param>
		/// <param name="nameAlias"> A potential alias for the name, or null. </param>
		/// <param name="elements"> An array of allowed child element defs, or null. </param>
		/// <param name="attributes"> An array of allowed attribute defs, or null. </param>
		/// <param name="contentHandler"> The element processor for this element. </param>
		/// <param name="classObject"> The class of the object that this element def should produce. </param>
		/// <param name="has_required"> true if this element has required elements by the XSLT specification. </param>
		/// <param name="required"> true if this element is required by the XSLT specification. </param>
		/// <param name="has_order"> whether this element has ordered child elements </param>
		/// <param name="order"> the order this element should appear according to the XSLT specification. </param>
		/// <param name="multiAllowed"> whether this element is allowed more than once </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject, bool has_required, bool required, bool has_order, int order, bool multiAllowed) : this(schema, @namespace, name, nameAlias, elements, attributes, contentHandler, classObject, has_required, required)
	  {
			this.m_order = order;
			this.m_multiAllowed = multiAllowed;
		this.m_isOrdered = has_order;
	  }

		/// <summary>
		/// Construct an instance of XSLTElementDef.
		/// </summary>
		/// <param name="namespace">  The Namespace URI, "*", or null. </param>
		/// <param name="name"> The local name (without prefix), "*", or null. </param>
		/// <param name="nameAlias"> A potential alias for the name, or null. </param>
		/// <param name="elements"> An array of allowed child element defs, or null. </param>
		/// <param name="attributes"> An array of allowed attribute defs, or null. </param>
		/// <param name="contentHandler"> The element processor for this element. </param>
		/// <param name="classObject"> The class of the object that this element def should produce. </param>
		/// <param name="has_order"> whether this element has ordered child elements </param>
		/// <param name="order"> the order this element should appear according to the XSLT specification. </param>
		/// <param name="multiAllowed"> whether this element is allowed more than once </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject, bool has_order, int order, bool multiAllowed) : this(schema, @namespace, name, nameAlias, elements, attributes, contentHandler, classObject, order, multiAllowed)
	  {
			this.m_isOrdered = has_order;
	  }

		/// <summary>
		/// Construct an instance of XSLTElementDef.
		/// </summary>
		/// <param name="namespace">  The Namespace URI, "*", or null. </param>
		/// <param name="name"> The local name (without prefix), "*", or null. </param>
		/// <param name="nameAlias"> A potential alias for the name, or null. </param>
		/// <param name="elements"> An array of allowed child element defs, or null. </param>
		/// <param name="attributes"> An array of allowed attribute defs, or null. </param>
		/// <param name="contentHandler"> The element processor for this element. </param>
		/// <param name="classObject"> The class of the object that this element def should produce. </param>
		/// <param name="order"> the order this element should appear according to the XSLT specification. </param>
		/// <param name="multiAllowed"> whether this element is allowed more than once </param>
	  internal XSLTElementDef(XSLTSchema schema, string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject, int order, bool multiAllowed) : this(schema, @namespace, name, nameAlias, elements, attributes, contentHandler, classObject)
	  {
		this.m_order = order;
			this.m_multiAllowed = multiAllowed;
	  }

	  /// <summary>
	  /// Construct an instance of XSLTElementDef that represents text.
	  /// </summary>
	  /// <param name="classObject"> The class of the object that this element def should produce. </param>
	  /// <param name="contentHandler"> The element processor for this element. </param>
	  /// <param name="type"> Content type, one of T_ELEMENT, T_PCDATA, or T_ANY. </param>
	  internal XSLTElementDef(Type classObject, XSLTElementProcessor contentHandler, int type)
	  {

		this.m_classObject = classObject;
		this.m_type = type;

		ElementProcessor = contentHandler;
	  }

	  /// <summary>
	  /// Construct an instance of XSLTElementDef.
	  /// </summary>
	  /// <param name="namespace">  The Namespace URI, "*", or null. </param>
	  /// <param name="name"> The local name (without prefix), "*", or null. </param>
	  /// <param name="nameAlias"> A potential alias for the name, or null. </param>
	  /// <param name="elements"> An array of allowed child element defs, or null. </param>
	  /// <param name="attributes"> An array of allowed attribute defs, or null. </param>
	  /// <param name="contentHandler"> The element processor for this element. </param>
	  /// <param name="classObject"> The class of the object that this element def should produce. </param>
	  internal virtual void build(string @namespace, string name, string nameAlias, XSLTElementDef[] elements, XSLTAttributeDef[] attributes, XSLTElementProcessor contentHandler, Type classObject)
	  {

		this.m_namespace = @namespace;
		this.m_name = name;
		this.m_nameAlias = nameAlias;
		this.m_elements = elements;
		this.m_attributes = attributes;

		ElementProcessor = contentHandler;

		this.m_classObject = classObject;

			if (hasRequired() && m_elements != null)
			{
				int n = m_elements.Length;
				for (int i = 0; i < n; i++)
				{
					XSLTElementDef def = m_elements[i];

					if (def != null && def.Required)
					{
						if (m_requiredFound == null)
						{
							m_requiredFound = new Hashtable();
						}
						m_requiredFound[def.Name] = "xsl:" + def.Name;
					}
				}
			}
	  }

	  /// <summary>
	  /// Tell if two objects are equal, when either one may be null.
	  /// If both are null, they are considered equal.
	  /// </summary>
	  /// <param name="obj1"> A reference to the first object, or null. </param>
	  /// <param name="obj2"> A reference to the second object, or null.
	  /// </param>
	  /// <returns> true if the to objects are equal by both being null or 
	  /// because obj2.equals(obj1) returns true. </returns>
	  private static bool equalsMayBeNull(object obj1, object obj2)
	  {
		return (obj2 == obj1) || ((null != obj1) && (null != obj2) && obj2.Equals(obj1));
	  }

	  /// <summary>
	  /// Tell if the two string refs are equal,
	  /// equality being defined as:
	  /// 1) Both strings are null.
	  /// 2) One string is null and the other is empty.
	  /// 3) Both strings are non-null, and equal.
	  /// </summary>
	  /// <param name="s1"> A reference to the first string, or null. </param>
	  /// <param name="s2"> A reference to the second string, or null.
	  /// </param>
	  /// <returns> true if Both strings are null, or if 
	  /// one string is null and the other is empty, or if 
	  /// both strings are non-null, and equal because 
	  /// s1.equals(s2) returns true. </returns>
	  private static bool equalsMayBeNullOrZeroLen(string s1, string s2)
	  {

		int len1 = (string.ReferenceEquals(s1, null)) ? 0 : s1.Length;
		int len2 = (string.ReferenceEquals(s2, null)) ? 0 : s2.Length;

		return (len1 != len2) ? false : (len1 == 0) ? true : s1.Equals(s2);
	  }

	  /// <summary>
	  /// Content type enumerations </summary>
	  internal const int T_ELEMENT = 1, T_PCDATA = 2, T_ANY = 3;

	  /// <summary>
	  /// The type of this element.
	  /// </summary>
	  private int m_type = T_ELEMENT;

	  /// <summary>
	  /// Get the type of this element.
	  /// </summary>
	  /// <returns> Content type, one of T_ELEMENT, T_PCDATA, or T_ANY. </returns>
	  internal virtual int Type
	  {
		  get
		  {
			return m_type;
		  }
		  set
		  {
			m_type = value;
		  }
	  }


	  /// <summary>
	  /// The allowed namespace for this element.
	  /// </summary>
	  private string m_namespace;

	  /// <summary>
	  /// Get the allowed namespace for this element.
	  /// </summary>
	  /// <returns> The Namespace URI, "*", or null. </returns>
	  internal virtual string Namespace
	  {
		  get
		  {
			return m_namespace;
		  }
	  }

	  /// <summary>
	  /// The name of this element.
	  /// </summary>
	  private string m_name;

	  /// <summary>
	  /// Get the local name of this element.
	  /// </summary>
	  /// <returns> The local name of this element, "*", or null. </returns>
	  internal virtual string Name
	  {
		  get
		  {
			return m_name;
		  }
	  }

	  /// <summary>
	  /// The name of this element.
	  /// </summary>
	  private string m_nameAlias;

	  /// <summary>
	  /// Get the name of this element.
	  /// </summary>
	  /// <returns> A potential alias for the name, or null. </returns>
	  internal virtual string NameAlias
	  {
		  get
		  {
			return m_nameAlias;
		  }
	  }

	  /// <summary>
	  /// The allowed elements for this type.
	  /// </summary>
	  private XSLTElementDef[] m_elements;

	  /// <summary>
	  /// Get the allowed elements for this type.
	  /// </summary>
	  /// <returns> An array of allowed child element defs, or null.
	  /// @xsl.usage internal </returns>
	  public virtual XSLTElementDef[] Elements
	  {
		  get
		  {
			return m_elements;
		  }
		  set
		  {
			m_elements = value;
		  }
	  }


	  /// <summary>
	  /// Tell if the namespace URI and local name match this
	  /// element. </summary>
	  /// <param name="uri"> The namespace uri, which may be null. </param>
	  /// <param name="localName"> The local name of an element, which may be null.
	  /// </param>
	  /// <returns> true if the uri and local name arguments are considered 
	  /// to match the uri and local name of this element def. </returns>
	  private bool QNameEquals(string uri, string localName)
	  {

		return (equalsMayBeNullOrZeroLen(m_namespace, uri) && (equalsMayBeNullOrZeroLen(m_name, localName) || equalsMayBeNullOrZeroLen(m_nameAlias, localName)));
	  }

	  /// <summary>
	  /// Given a namespace URI, and a local name, get the processor
	  /// for the element, or return null if not allowed.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing.
	  /// </param>
	  /// <returns> The element processor that matches the arguments, or null. </returns>
	  internal virtual XSLTElementProcessor getProcessorFor(string uri, string localName)
	  {

		XSLTElementProcessor elemDef = null; // return value

		if (null == m_elements)
		{
		  return null;
		}

		int n = m_elements.Length;
		int order = -1;
			bool multiAllowed = true;
		for (int i = 0; i < n; i++)
		{
		  XSLTElementDef def = m_elements[i];

		  // A "*" signals that the element allows literal result
		  // elements, so just assign the def, and continue to  
		  // see if anything else matches.
		  if (def.m_name.Equals("*"))
		  {

			// Don't allow xsl elements
			if (!equalsMayBeNullOrZeroLen(uri, Constants.S_XSLNAMESPACEURL))
			{
			  elemDef = def.m_elementProcessor;
					  order = def.Order;
						multiAllowed = def.MultiAllowed;
			}
		  }
				else if (def.QNameEquals(uri, localName))
				{
					if (def.Required)
					{
						this.setRequiredFound(def.Name, true);
					}
					order = def.Order;
					multiAllowed = def.MultiAllowed;
					elemDef = def.m_elementProcessor;
					break;
				}
		}

			if (elemDef != null && this.Ordered)
			{
				int lastOrder = LastOrder;
				if (order > lastOrder)
				{
					LastOrder = order;
				}
				else if (order == lastOrder && !multiAllowed)
				{
					return null;
				}
				else if (order < lastOrder && order > 0)
				{
					return null;
				}
			}

		return elemDef;
	  }

	  /// <summary>
	  /// Given an unknown element, get the processor
	  /// for the element.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing.
	  /// </param>
	  /// <returns> normally a <seealso cref="ProcessorUnknown"/> reference. </returns>
	  /// <seealso cref= ProcessorUnknown </seealso>
	  internal virtual XSLTElementProcessor getProcessorForUnknown(string uri, string localName)
	  {

		// XSLTElementProcessor lreDef = null; // return value
		if (null == m_elements)
		{
		  return null;
		}

		int n = m_elements.Length;

		for (int i = 0; i < n; i++)
		{
		  XSLTElementDef def = m_elements[i];

		  if (def.m_name.Equals("unknown") && uri.Length > 0)
		  {
			return def.m_elementProcessor;
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// The allowed attributes for this type.
	  /// </summary>
	  private XSLTAttributeDef[] m_attributes;

	  /// <summary>
	  /// Get the allowed attributes for this type.
	  /// </summary>
	  /// <returns> An array of allowed attribute defs, or null. </returns>
	  internal virtual XSLTAttributeDef[] Attributes
	  {
		  get
		  {
			return m_attributes;
		  }
	  }

	  /// <summary>
	  /// Given a namespace URI, and a local name, return the element's
	  /// attribute definition, if it has one.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing.
	  /// </param>
	  /// <returns> The attribute def that matches the arguments, or null. </returns>
	  internal virtual XSLTAttributeDef getAttributeDef(string uri, string localName)
	  {

		XSLTAttributeDef defaultDef = null;
		XSLTAttributeDef[] attrDefs = Attributes;
		int nAttrDefs = attrDefs.Length;

		for (int k = 0; k < nAttrDefs; k++)
		{
		  XSLTAttributeDef attrDef = attrDefs[k];
		  string uriDef = attrDef.Namespace;
		  string nameDef = attrDef.Name;

		  if (nameDef.Equals("*") && (equalsMayBeNullOrZeroLen(uri, uriDef) || (!string.ReferenceEquals(uriDef, null) && uriDef.Equals("*") && !string.ReferenceEquals(uri, null) && uri.Length > 0)))
		  {
			return attrDef;
		  }
		  else if (nameDef.Equals("*") && (string.ReferenceEquals(uriDef, null)))
		  {

			// In this case, all attributes are legal, so return 
			// this as the last resort.
			defaultDef = attrDef;
		  }
		  else if (equalsMayBeNullOrZeroLen(uri, uriDef) && localName.Equals(nameDef))
		  {
			return attrDef;
		  }
		}

		if (null == defaultDef)
		{
		  if (uri.Length > 0 && !equalsMayBeNullOrZeroLen(uri, Constants.S_XSLNAMESPACEURL))
		  {
			return XSLTAttributeDef.m_foreignAttr;
		  }
		}

		return defaultDef;
	  }

	  /// <summary>
	  /// If non-null, the ContentHandler/TransformerFactory for this element.
	  /// </summary>
	  private XSLTElementProcessor m_elementProcessor;

	  /// <summary>
	  /// Return the XSLTElementProcessor for this element.
	  /// </summary>
	  /// <returns> The element processor for this element.
	  /// @xsl.usage internal </returns>
	  public virtual XSLTElementProcessor ElementProcessor
	  {
		  get
		  {
			return m_elementProcessor;
		  }
		  set
		  {
    
			if (value != null)
			{
			  m_elementProcessor = value;
    
			  m_elementProcessor.ElemDef = this;
			}
		  }
	  }


	  /// <summary>
	  /// If non-null, the class object that should in instantiated for
	  /// a Xalan instance of this element.
	  /// </summary>
	  private Type m_classObject;

	  /// <summary>
	  /// Return the class object that should in instantiated for
	  /// a Xalan instance of this element.
	  /// </summary>
	  /// <returns> The class of the object that this element def should produce, or null. </returns>
	  internal virtual Type ClassObject
	  {
		  get
		  {
			return m_classObject;
		  }
	  }

		/// <summary>
		/// If true, this has a required element.
		/// </summary>
	  private bool m_has_required = false;

	  /// <summary>
	  /// Get whether or not this has a required element.
	  /// </summary>
	  /// <returns> true if this this has a required element. </returns>
	  internal virtual bool hasRequired()
	  {
		return m_has_required;
	  }

		/// <summary>
		/// If true, this is a required element.
		/// </summary>
	  private bool m_required = false;

	  /// <summary>
	  /// Get whether or not this is a required element.
	  /// </summary>
	  /// <returns> true if this is a required element. </returns>
	  internal virtual bool Required
	  {
		  get
		  {
			return m_required;
		  }
	  }

		internal Hashtable m_requiredFound;

		/// <summary>
		/// Set this required element found.
		///  
		/// </summary>
	  internal virtual void setRequiredFound(string elem, bool found)
	  {
	   if (m_requiredFound[elem] != null)
	   {
			 m_requiredFound.Remove(elem);
	   }
	  }

		/// <summary>
		/// Get whether all required elements were found.
		/// </summary>
		/// <returns> true if all required elements were found. </returns>
	  internal virtual bool RequiredFound
	  {
		  get
		  {
				if (m_requiredFound == null)
				{
					return true;
				}
			return m_requiredFound.Count == 0;
		  }
	  }

		/// <summary>
		/// Get required elements that were not found.
		/// </summary>
		/// <returns> required elements that were not found. </returns>
	  internal virtual string RequiredElem
	  {
		  get
		  {
				if (m_requiredFound == null)
				{
					return null;
				}
				System.Collections.IEnumerator elems = m_requiredFound.Values.GetEnumerator();
				string s = "";
				bool first = true;
				while (elems.MoveNext())
				{
					if (first)
					{
						first = false;
					}
					else
					{
					 s = s + ", ";
					}
					s = s + (string)elems.Current;
				}
			return s;
		  }
	  }

		internal bool m_isOrdered = false;

		/// <summary>
		/// Get whether this element requires ordered children.
		/// </summary>
		/// <returns> true if this element requires ordered children. </returns>
	  internal virtual bool Ordered
	  {
		  get
		  {
				/*if (!m_CheckedOrdered)
				{
					m_CheckedOrdered = true;
					m_isOrdered = false;
					if (null == m_elements)
						return false;
		
					int n = m_elements.length;
		
					for (int i = 0; i < n; i++)
					{
						if (m_elements[i].getOrder() > 0)
						{
							m_isOrdered = true;
							return true;
						}
					}
					return false;
				}
				else*/
					return m_isOrdered;
		  }
	  }

		/// <summary>
		/// the order that this element should appear, or -1 if not ordered
		/// </summary>
	  private int m_order = -1;

		/// <summary>
		/// Get the order that this element should appear .
		/// </summary>
		/// <returns> the order that this element should appear. </returns>
	  internal virtual int Order
	  {
		  get
		  {
			return m_order;
		  }
	  }

		/// <summary>
		/// the highest order of child elements have appeared so far, 
		/// or -1 if not ordered
		/// </summary>
	  private int m_lastOrder = -1;

		/// <summary>
		/// Get the highest order of child elements have appeared so far .
		/// </summary>
		/// <returns> the highest order of child elements have appeared so far. </returns>
	  internal virtual int LastOrder
	  {
		  get
		  {
			return m_lastOrder;
		  }
		  set
		  {
			m_lastOrder = value;
		  }
	  }


		/// <summary>
		/// True if this element can appear multiple times
		/// </summary>
	  private bool m_multiAllowed = true;

		/// <summary>
		/// Get whether this element can appear multiple times
		/// </summary>
		/// <returns> true if this element can appear multiple times </returns>
	  internal virtual bool MultiAllowed
	  {
		  get
		  {
			return m_multiAllowed;
		  }
	  }
	}

}