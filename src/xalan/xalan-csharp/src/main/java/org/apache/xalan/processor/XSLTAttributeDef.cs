using System;
using System.Collections;
using System.Text;

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
 * $Id: XSLTAttributeDef.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using AVT = org.apache.xalan.templates.AVT;
	using Constants = org.apache.xalan.templates.Constants;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using QName = org.apache.xml.utils.QName;
	using StringToIntTable = org.apache.xml.utils.StringToIntTable;
	using StringVector = org.apache.xml.utils.StringVector;
	using XML11Char = org.apache.xml.utils.XML11Char;
	using XPath = org.apache.xpath.XPath;


	/// <summary>
	/// This class defines an attribute for an element in a XSLT stylesheet,
	/// is meant to reflect the structure defined in http://www.w3.org/TR/xslt#dtd, and the
	/// mapping between Xalan classes and the markup attributes in the element.
	/// </summary>
	public class XSLTAttributeDef
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			m_errorType = WARNING;
		}

	   // How to handle invalid values for this attribute 
	   internal const int FATAL = 0;
	   internal const int ERROR = 1;
	   internal const int WARNING = 2;


	  /// <summary>
	  /// Construct an instance of XSLTAttributeDef.
	  /// </summary>
	  /// <param name="namespace"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="type"> One of T_CDATA, T_URL, T_AVT, T_PATTERN, T_EXPR, T_CHAR,
	  /// T_NUMBER, T_YESNO, T_QNAME, T_QNAMES, T_ENUM, T_SIMPLEPATTERNLIST,
	  /// T_NMTOKEN, T_STRINGLIST, T_PREFIX_URLLIST, T_ENUM_OR_PQNAME, T_NCNAME. </param>
	  /// <param name="required"> true if this is attribute is required by the XSLT specification. </param>
	  /// <param name="supportsAVT"> true if this attribute supports AVT's. </param>
	  /// <param name="errorType"> the type of error to issue if validation fails.  One of FATAL, ERROR, WARNING.  </param>
	  internal XSLTAttributeDef(string @namespace, string name, int type, bool required, bool supportsAVT, int errorType)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		this.m_namespace = @namespace;
		this.m_name = name;
		this.m_type = type;
		this.m_required = required;
		this.m_supportsAVT = supportsAVT;
		this.m_errorType = errorType;
	  }

	  /// <summary>
	  /// Construct an instance of XSLTAttributeDef.
	  /// </summary>
	  /// <param name="namespace"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="type"> One of T_CDATA, T_URL, T_AVT, T_PATTERN, T_EXPR,
	  /// T_CHAR, T_NUMBER, T_YESNO, T_QNAME, T_QNAMES, T_ENUM,
	  /// T_SIMPLEPATTERNLIST, T_NMTOKEN, T_STRINGLIST, T_PREFIX_URLLIST, 
	  /// T_ENUM_OR_PQNAME, T_NCNAME. </param>
	  /// <param name="supportsAVT"> true if this attribute supports AVT's. </param>
	  /// <param name="errorType"> the type of error to issue if validation fails.  One of FATAL, ERROR, WARNING. </param>
	  /// <param name="defaultVal"> The default value for this attribute. </param>
	  internal XSLTAttributeDef(string @namespace, string name, int type, bool supportsAVT, int errorType, string defaultVal)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }

		this.m_namespace = @namespace;
		this.m_name = name;
		this.m_type = type;
		this.m_required = false;
		this.m_supportsAVT = supportsAVT;
		this.m_errorType = errorType;
		this.m_default = defaultVal;
	  }

	  /// <summary>
	  /// Construct an instance of XSLTAttributeDef that uses two
	  /// enumerated values.
	  /// </summary>
	  /// <param name="namespace"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="required"> true if this attribute is required by the XSLT specification. </param>
	  /// <param name="supportsAVT"> true if this attribute supports AVT's. </param>
	  /// <param name="prefixedQNameValAllowed"> If true, the type is T_ENUM_OR_PQNAME </param>
	  /// <param name="errorType"> the type of error to issue if validation fails.  One of FATAL, ERROR, WARNING. </param>
	  /// <param name="k1"> The XSLT name of the enumerated value. </param>
	  /// <param name="v1"> An integer representation of k1. </param>
	  /// <param name="k2"> The XSLT name of the enumerated value. </param>
	  /// <param name="v2"> An integer representation of k2. </param>
	  internal XSLTAttributeDef(string @namespace, string name, bool required, bool supportsAVT, bool prefixedQNameValAllowed, int errorType, string k1, int v1, string k2, int v2)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }

		this.m_namespace = @namespace;
		this.m_name = name;
		this.m_type = prefixedQNameValAllowed ? T_ENUM_OR_PQNAME : T_ENUM;
		this.m_required = required;
		this.m_supportsAVT = supportsAVT;
		this.m_errorType = errorType;
		m_enums = new StringToIntTable(2);

		m_enums.put(k1, v1);
		m_enums.put(k2, v2);
	  }

	  /// <summary>
	  /// Construct an instance of XSLTAttributeDef that uses three
	  /// enumerated values.
	  /// </summary>
	  /// <param name="namespace"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="required"> true if this attribute is required by the XSLT specification. </param>
	  /// <param name="supportsAVT"> true if this attribute supports AVT's. </param>
	  /// <param name="prefixedQNameValAllowed"> If true, the type is T_ENUM_OR_PQNAME </param>
	  /// <param name="errorType"> the type of error to issue if validation fails.  One of FATAL, ERROR, WARNING.    * </param>
	  /// <param name="k1"> The XSLT name of the enumerated value. </param>
	  /// <param name="v1"> An integer representation of k1. </param>
	  /// <param name="k2"> The XSLT name of the enumerated value. </param>
	  /// <param name="v2"> An integer representation of k2. </param>
	  /// <param name="k3"> The XSLT name of the enumerated value. </param>
	  /// <param name="v3"> An integer representation of k3. </param>
	  internal XSLTAttributeDef(string @namespace, string name, bool required, bool supportsAVT, bool prefixedQNameValAllowed, int errorType, string k1, int v1, string k2, int v2, string k3, int v3)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }

		this.m_namespace = @namespace;
		this.m_name = name;
		this.m_type = prefixedQNameValAllowed ? T_ENUM_OR_PQNAME : T_ENUM;
		this.m_required = required;
		this.m_supportsAVT = supportsAVT;
		this.m_errorType = errorType;
		m_enums = new StringToIntTable(3);

		m_enums.put(k1, v1);
		m_enums.put(k2, v2);
		m_enums.put(k3, v3);
	  }

	  /// <summary>
	  /// Construct an instance of XSLTAttributeDef that uses three
	  /// enumerated values.
	  /// </summary>
	  /// <param name="namespace"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="required"> true if this attribute is required by the XSLT specification. </param>
	  /// <param name="supportsAVT"> true if this attribute supports AVT's. </param>
	  /// <param name="prefixedQNameValAllowed"> If true, the type is T_ENUM_OR_PQNAME </param> </param>
	  /// <param name="errorType"> the type of error to issue if validation fails.  One of FATAL, ERROR, WARNING.    * <param name="k1"> The XSLT name of the enumerated value. </param>
	  /// <param name="v1"> An integer representation of k1. </param>
	  /// <param name="k2"> The XSLT name of the enumerated value. </param>
	  /// <param name="v2"> An integer representation of k2. </param>
	  /// <param name="k3"> The XSLT name of the enumerated value. </param>
	  /// <param name="v3"> An integer representation of k3. </param>
	  /// <param name="k4"> The XSLT name of the enumerated value. </param>
	  /// <param name="v4"> An integer representation of k4. </param>
	  internal XSLTAttributeDef(string @namespace, string name, bool required, bool supportsAVT, bool prefixedQNameValAllowed, int errorType, string k1, int v1, string k2, int v2, string k3, int v3, string k4, int v4)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }

		this.m_namespace = @namespace;
		this.m_name = name;
		this.m_type = prefixedQNameValAllowed ? T_ENUM_OR_PQNAME : T_ENUM;
		this.m_required = required;
		this.m_supportsAVT = supportsAVT;
		this.m_errorType = errorType;
		m_enums = new StringToIntTable(4);

		m_enums.put(k1, v1);
		m_enums.put(k2, v2);
		m_enums.put(k3, v3);
		m_enums.put(k4, v4);
	  }

	  /// <summary>
	  /// Type values that represent XSLT attribute types. </summary>
	  internal const int T_CDATA = 1, T_URL = 2, T_AVT = 3, T_PATTERN = 4, T_EXPR = 5, T_CHAR = 6, T_NUMBER = 7, T_YESNO = 8, T_QNAME = 9, T_QNAMES = 10, T_ENUM = 11, T_SIMPLEPATTERNLIST = 12, T_NMTOKEN = 13, T_STRINGLIST = 14, T_PREFIX_URLLIST = 15, T_ENUM_OR_PQNAME = 16, T_NCNAME = 17, T_AVT_QNAME = 18, T_QNAMES_RESOLVE_NULL = 19, T_PREFIXLIST = 20;

	  /// <summary>
	  /// Representation for an attribute in a foreign namespace. </summary>
	  internal static readonly XSLTAttributeDef m_foreignAttr = new XSLTAttributeDef("*", "*", XSLTAttributeDef.T_CDATA,false, false, WARNING);

	  /// <summary>
	  /// Method name that objects may implement if they wish to have forein attributes set. </summary>
	  internal const string S_FOREIGNATTR_SETTER = "setForeignAttr";

	  /// <summary>
	  /// The allowed namespace for this element.
	  /// </summary>
	  private string m_namespace;

	  /// <summary>
	  /// Get the allowed namespace for this attribute.
	  /// </summary>
	  /// <returns> The allowed namespace for this attribute, which may be null, or may be "*". </returns>
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
	  /// Get the name of this attribute.
	  /// </summary>
	  /// <returns> non-null reference to the name of this attribute, which may be "*". </returns>
	  internal virtual string Name
	  {
		  get
		  {
			return m_name;
		  }
	  }

	  /// <summary>
	  /// The type of this attribute value.
	  /// </summary>
	  private int m_type;

	  /// <summary>
	  /// Get the type of this attribute value.
	  /// </summary>
	  /// <returns> One of T_CDATA, T_URL, T_AVT, T_PATTERN, T_EXPR, T_CHAR,
	  /// T_NUMBER, T_YESNO, T_QNAME, T_QNAMES, T_ENUM, T_SIMPLEPATTERNLIST,
	  /// T_NMTOKEN, T_STRINGLIST, T_PREFIX_URLLIST, T_ENUM_OR_PQNAME. </returns>
	  internal virtual int Type
	  {
		  get
		  {
			return m_type;
		  }
	  }

	  /// <summary>
	  /// If this element is of type T_ENUM, this will contain
	  /// a map from the attribute string to the Xalan integer
	  /// value.
	  /// </summary>
	  private StringToIntTable m_enums;

	  /// <summary>
	  /// If this element is of type T_ENUM, this will return
	  /// a map from the attribute string to the Xalan integer
	  /// value. </summary>
	  /// <param name="key"> The XSLT attribute value.
	  /// </param>
	  /// <returns> The integer representation of the enumerated value for this attribute. </returns>
	  /// <exception cref="Throws"> NullPointerException if m_enums is null. </exception>
	  private int getEnum(string key)
	  {
		return m_enums.get(key);
	  }

	 /// <summary>
	 /// If this element is of type T_ENUM, this will return
	 /// an array of strings - the values in the enumeration
	 /// </summary>
	 /// <returns> An array of the enumerated values permitted for this attribute.
	 /// </returns>
	 /// <exception cref="Throws"> NullPointerException if m_enums is null. </exception>
	  private string[] EnumNames
	  {
		  get
		  {
			return m_enums.keys();
		  }
	  }

	  /// <summary>
	  /// The default value for this attribute.
	  /// </summary>
	  private string m_default;

	  /// <summary>
	  /// Get the default value for this attribute.
	  /// </summary>
	  /// <returns> The default value for this attribute, or null. </returns>
	  internal virtual string Default
	  {
		  get
		  {
			return m_default;
		  }
		  set
		  {
			m_default = value;
		  }
	  }


	  /// <summary>
	  /// If true, this is a required attribute.
	  /// </summary>
	  private bool m_required;

	  /// <summary>
	  /// Get whether or not this is a required attribute.
	  /// </summary>
	  /// <returns> true if this is a required attribute. </returns>
	  internal virtual bool Required
	  {
		  get
		  {
			return m_required;
		  }
	  }

	  /// <summary>
	  /// If true, this is attribute supports AVT's.
	  /// </summary>
	  private bool m_supportsAVT;

	  /// <summary>
	  /// Get whether or not this attribute supports AVT's.
	  /// </summary>
	  /// <returns> true if this attribute supports AVT's. </returns>
	  internal virtual bool SupportsAVT
	  {
		  get
		  {
			return m_supportsAVT;
		  }
	  }

	  internal int m_errorType;

	  /// <summary>
	  /// Get the type of error message to use if the attribute value is invalid.
	  /// </summary>
	  /// <returns> one of XSLAttributeDef.FATAL, XSLAttributeDef.ERROR, XSLAttributeDef.WARNING </returns>
	  internal virtual int ErrorType
	  {
		  get
		  {
			return m_errorType;
		  }
	  }
	  /// <summary>
	  /// String that should represent the setter method which which
	  /// may be used on objects to set a value that represents this attribute  
	  /// </summary>
	  internal string m_setterString = null;

	  /// <summary>
	  /// Return a string that should represent the setter method.
	  /// The setter method name will be created algorithmically the
	  /// first time this method is accessed, and then cached for return
	  /// by subsequent invocations of this method.
	  /// </summary>
	  /// <returns> String that should represent the setter method which which
	  /// may be used on objects to set a value that represents this attribute,
	  /// of null if no setter method should be called. </returns>
	  public virtual string SetterMethodName
	  {
		  get
		  {
    
			if (null == m_setterString)
			{
			  if (m_foreignAttr == this)
			  {
				return S_FOREIGNATTR_SETTER;
			  }
			  else if (m_name.Equals("*"))
			  {
				m_setterString = "addLiteralResultAttribute";
    
				return m_setterString;
			  }
    
			  StringBuilder outBuf = new StringBuilder();
    
			  outBuf.Append("set");
    
			  if ((!string.ReferenceEquals(m_namespace, null)) && m_namespace.Equals(Constants.S_XMLNAMESPACEURI))
			  {
				outBuf.Append("Xml");
			  }
    
			  int n = m_name.Length;
    
			  for (int i = 0; i < n; i++)
			  {
				char c = m_name[i];
    
				if ('-' == c)
				{
				  i++;
    
				  c = m_name[i];
				  c = char.ToUpper(c);
				}
				else if (0 == i)
				{
				  c = char.ToUpper(c);
				}
    
				outBuf.Append(c);
			  }
    
			  m_setterString = outBuf.ToString();
			}
    
			return m_setterString;
		  }
	  }

	  /// <summary>
	  /// Process an attribute string of type T_AVT into
	  /// a AVT value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> Should be an Attribute Value Template string.
	  /// </param>
	  /// <returns> An AVT object that may be used to evaluate the Attribute Value Template.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> which will wrap a
	  /// <seealso cref="javax.xml.transform.TransformerException"/>, if there is a syntax error
	  /// in the attribute value template string. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: org.apache.xalan.templates.AVT processAVT(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual AVT processAVT(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		try
		{
		  AVT avt = new AVT(handler, uri, name, rawName, value, owner);

		  return avt;
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_CDATA into
	  /// a String value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> non-null string reference.
	  /// </param>
	  /// <returns> The value argument.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException."> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processCDATA(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processCDATA(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {
		  if (SupportsAVT)
		  {
			try
			{
			  AVT avt = new AVT(handler, uri, name, rawName, value, owner);
			  return avt;
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
		  }
		  else
		  {
			return value;
		  }
	  }

	  /// <summary>
	  /// Process an attribute string of type T_CHAR into
	  /// a Character value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> Should be a string with a length of 1.
	  /// </param>
	  /// <returns> Character object.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the string is not a length of 1. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processCHAR(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processCHAR(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {
		if (SupportsAVT)
		{
			try
			{
			  AVT avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If an AVT wasn't used, validate the value
			  if ((avt.Simple) && (value.Length != 1))
			  {
				  handleError(handler, XSLTErrorResources.INVALID_TCHAR, new object[] {name, value},null);
				return null;
			  }
			  return avt;
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
		}
		else
		{
			if (value.Length != 1)
			{
				handleError(handler, XSLTErrorResources.INVALID_TCHAR, new object[] {name, value},null);
				return null;
			}

			return new char?(value[0]);
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_ENUM into a int value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> non-null string that represents an enumerated value that is
	  /// valid for this element. </param>
	  /// <param name="owner">
	  /// </param>
	  /// <returns> An Integer representation of the enumerated value if this attribute does not support
	  ///         AVT.  Otherwise, and AVT is returned. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processENUM(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processENUM(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		AVT avt = null;
		if (SupportsAVT)
		{
			try
			{
			  avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If this attribute used an avt, then we can't validate at this time.
			  if (!avt.Simple)
			  {
				  return avt;
			  }
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
		}

		int retVal = this.getEnum(value);

		if (retVal == StringToIntTable.INVALID_KEY)
		{
		   StringBuilder enumNamesList = ListOfEnums;
		   handleError(handler, XSLTErrorResources.INVALID_ENUM,new object[]{name, value, enumNamesList.ToString()},null);
		   return null;
		}

		if (SupportsAVT)
		{
			return avt;
		}
		else
		{
			return new int?(retVal);
		}

	  }

	  /// <summary>
	  /// Process an attribute string of that is either an enumerated value or a qname-but-not-ncname.
	  /// Returns an AVT, if this attribute support AVT; otherwise returns int or qname.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> non-null string that represents an enumerated value that is
	  /// valid for this element. </param>
	  /// <param name="owner">
	  /// </param>
	  /// <returns> AVT if attribute supports AVT. An Integer representation of the enumerated value if
	  ///         attribute does not support AVT and an enumerated value was used.  Otherwise a qname
	  ///         is returned. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processENUM_OR_PQNAME(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processENUM_OR_PQNAME(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		object objToReturn = null;

		if (SupportsAVT)
		{
			try
			{
			  AVT avt = new AVT(handler, uri, name, rawName, value, owner);
			  if (!avt.Simple)
			  {
				  return avt;
			  }
			  else
			  {
				  objToReturn = avt;
			  }
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
		}

		// An avt wasn't used.
		  int key = this.getEnum(value);

		if (key != StringToIntTable.INVALID_KEY)
		{
			if (objToReturn == null)
			{
				objToReturn = new int?(key);
			}
		}

		// enum not used.  Validate qname-but-not-ncname.
		else
		{
			try
			{
				QName qname = new QName(value, handler, true);
				if (objToReturn == null)
				{
					objToReturn = qname;
				}

				if (string.ReferenceEquals(qname.Prefix, null))
				{
				   StringBuilder enumNamesList = ListOfEnums;

					enumNamesList.Append(" <qname-but-not-ncname>");
				   handleError(handler,XSLTErrorResources.INVALID_ENUM,new object[]{name, value, enumNamesList.ToString()},null);
				   return null;

				}
			}
			catch (System.ArgumentException ie)
			{
			   StringBuilder enumNamesList = ListOfEnums;
			   enumNamesList.Append(" <qname-but-not-ncname>");

			   handleError(handler,XSLTErrorResources.INVALID_ENUM,new object[]{name, value, enumNamesList.ToString()},ie);
			   return null;

			}
			catch (Exception re)
			{
			   StringBuilder enumNamesList = ListOfEnums;
			   enumNamesList.Append(" <qname-but-not-ncname>");

			   handleError(handler,XSLTErrorResources.INVALID_ENUM,new object[]{name, value, enumNamesList.ToString()},re);
			   return null;
			}
		}

		  return objToReturn;
	  }

	  /// <summary>
	  /// Process an attribute string of type T_EXPR into
	  /// an XPath value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> An XSLT expression string.
	  /// </param>
	  /// <returns> an XPath object that may be used for evaluation.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the expression
	  /// string contains a syntax error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processEXPR(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processEXPR(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		try
		{
		  XPath expr = handler.createXPath(value, owner);

		  return expr;
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_NMTOKEN into
	  /// a String value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A NMTOKEN string.
	  /// </param>
	  /// <returns> the value argument or an AVT if this attribute supports AVTs.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the value is not a valid nmtoken </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processNMTOKEN(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processNMTOKEN(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		  if (SupportsAVT)
		  {
			try
			{
			  AVT avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If an AVT wasn't used, validate the value
			  if ((avt.Simple) && (!XML11Char.isXML11ValidNmtoken(value)))
			  {
				handleError(handler,XSLTErrorResources.INVALID_NMTOKEN, new object[] {name, value},null);
				return null;
			  }
			  return avt;
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
		  }
		  else
		  {
			  if (!XML11Char.isXML11ValidNmtoken(value))
			  {
				handleError(handler,XSLTErrorResources.INVALID_NMTOKEN, new object[] {name, value},null);
				return null;
			  }
		  }
		return value;
	  }

	  /// <summary>
	  /// Process an attribute string of type T_PATTERN into
	  /// an XPath match pattern value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A match pattern string.
	  /// </param>
	  /// <returns> An XPath pattern that may be used to evaluate the XPath.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the match pattern
	  /// string contains a syntax error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processPATTERN(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processPATTERN(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		try
		{
		  XPath pattern = handler.createMatchPatternXPath(value, owner);

		  return pattern;
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_NUMBER into
	  /// a double value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A string that can be parsed into a double value. </param>
	  /// <param name="number">
	  /// </param>
	  /// <returns> A Double object.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/>
	  /// if the string does not contain a parsable number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processNUMBER(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processNUMBER(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {


		if (SupportsAVT)
		{
			double? val;
			AVT avt = null;
			try
			{
			  avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If this attribute used an avt, then we can't validate at this time.
			  if (avt.Simple)
			  {
				  val = Convert.ToDouble(value);
			  }
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
			catch (System.FormatException nfe)
			{
				 handleError(handler,XSLTErrorResources.INVALID_NUMBER, new object[] {name, value}, nfe);
				return null;
			}
			return avt;

		}
		else
		{
			try
			{
			  return Convert.ToDouble(value);
			}
			catch (System.FormatException nfe)
			{
				handleError(handler,XSLTErrorResources.INVALID_NUMBER, new object[] {name, value}, nfe);
				return null;
			}
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_QNAME into a QName value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A string that represents a potentially prefix qualified name. </param>
	  /// <param name="owner">
	  /// </param>
	  /// <returns> A QName object if this attribute does not support AVT's.  Otherwise, an AVT
	  ///         is returned.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the string contains a prefix that can not be
	  /// resolved, or the string contains syntax that is invalid for a qualified name. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processQNAME(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processQNAME(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		 try
		 {
				 QName qname = new QName(value, handler, true);
			  return qname;
		 }
			catch (System.ArgumentException ie)
			{
				// thrown by QName constructor
				handleError(handler,XSLTErrorResources.INVALID_QNAME, new object[] {name, value},ie);
				return null;
			}
			catch (Exception re)
			{
				// thrown by QName constructor
				handleError(handler,XSLTErrorResources.INVALID_QNAME, new object[] {name, value},re);
				return null;
			}
	  }


	  /// <summary>
	  /// Process an attribute string of type T_QNAME into a QName value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A string that represents a potentially prefix qualified name. </param>
	  /// <param name="owner">
	  /// </param>
	  /// <returns> An AVT is returned.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the string contains a prefix that can not be
	  /// resolved, or the string contains syntax that is invalid for a qualified name. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processAVT_QNAME(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processAVT_QNAME(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		   AVT avt = null;
		   try
		   {
			  avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If an AVT wasn't used, validate the value
			  if (avt.Simple)
			  {
				 int indexOfNSSep = value.IndexOf(':');

				 if (indexOfNSSep >= 0)
				 {
					  string prefix = value.Substring(0, indexOfNSSep);
					  if (!XML11Char.isXML11ValidNCName(prefix))
					  {
						 handleError(handler,XSLTErrorResources.INVALID_QNAME,new object[]{name, value},null);
						 return null;
					  }
				 }

				 string localName = (indexOfNSSep < 0) ? value : value.Substring(indexOfNSSep + 1);

				 if ((string.ReferenceEquals(localName, null)) || (localName.Length == 0) || (!XML11Char.isXML11ValidNCName(localName)))
				 {
						 handleError(handler,XSLTErrorResources.INVALID_QNAME,new object[]{name, value},null);
						 return null;
				 }
			  }
		   }
			catch (TransformerException te)
			{
			   // thrown by AVT constructor
			  throw new org.xml.sax.SAXException(te);
			}

		return avt;
	  }

	  /// <summary>
	  /// Process an attribute string of type NCName into a String
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A string that represents a potentially prefix qualified name. </param>
	  /// <param name="owner">
	  /// </param>
	  /// <returns> A String object if this attribute does not support AVT's.  Otherwise, an AVT
	  ///         is returned.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the string contains a prefix that can not be
	  /// resolved, or the string contains syntax that is invalid for a NCName. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processNCNAME(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processNCNAME(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		if (SupportsAVT)
		{
			AVT avt = null;
			try
			{
			  avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If an AVT wasn't used, validate the value
			  if ((avt.Simple) && (!XML11Char.isXML11ValidNCName(value)))
			  {
				 handleError(handler,XSLTErrorResources.INVALID_NCNAME,new object[] {name, value},null);
				 return null;
			  }
			  return avt;
			}
			catch (TransformerException te)
			{
			   // thrown by AVT constructor
			  throw new org.xml.sax.SAXException(te);
			}

		}
		else
		{
			if (!XML11Char.isXML11ValidNCName(value))
			{
				handleError(handler,XSLTErrorResources.INVALID_NCNAME,new object[] {name, value},null);
				return null;
			}
			return value;
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_QNAMES into a vector of QNames where
	  /// the specification requires that non-prefixed elements not be placed in a
	  /// namespace.  (See section 2.4 of XSLT 1.0.)
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A whitespace delimited list of qualified names.
	  /// </param>
	  /// <returns> a Vector of QName objects.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the one of the qualified name strings
	  /// contains a prefix that can not be
	  /// resolved, or a qualified name contains syntax that is invalid for a qualified name. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: java.util.Vector processQNAMES(StylesheetHandler handler, String uri, String name, String rawName, String value) throws org.xml.sax.SAXException
	  internal virtual ArrayList processQNAMES(StylesheetHandler handler, string uri, string name, string rawName, string value)
	  {

		StringTokenizer tokenizer = new StringTokenizer(value, " \t\n\r\f");
		int nQNames = tokenizer.countTokens();
		ArrayList qnames = new ArrayList(nQNames);

		for (int i = 0; i < nQNames; i++)
		{
		  // Fix from Alexander Rudnev
		  qnames.Add(new QName(tokenizer.nextToken(), handler));
		}

		return qnames;
	  }

	 /// <summary>
	 /// Process an attribute string of type T_QNAMES_RESOLVE_NULL into a vector
	 /// of QNames where the specification requires non-prefixed elements to be
	 /// placed in the default namespace.  (See section 16 of XSLT 1.0; the
	 /// <em>only</em> time that this will get called is for the
	 /// <code>cdata-section-elements</code> attribute on <code>xsl:output</code>.
	 /// </summary>
	 /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	 /// <param name="uri"> The Namespace URI, or an empty string. </param>
	 /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	 /// <param name="rawName"> The qualified name (with prefix). </param>
	 /// <param name="value"> A whitespace delimited list of qualified names.
	 /// </param>
	 /// <returns> a Vector of QName objects.
	 /// </returns>
	 /// <exception cref="org.xml.sax.SAXException"> if the one of the qualified name strings
	 /// contains a prefix that can not be resolved, or a qualified name contains
	 /// syntax that is invalid for a qualified name. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: final java.util.Vector processQNAMESRNU(StylesheetHandler handler, String uri, String name, String rawName, String value) throws org.xml.sax.SAXException
	  internal ArrayList processQNAMESRNU(StylesheetHandler handler, string uri, string name, string rawName, string value)
	  {

		StringTokenizer tokenizer = new StringTokenizer(value, " \t\n\r\f");
		int nQNames = tokenizer.countTokens();
		ArrayList qnames = new ArrayList(nQNames);

		string defaultURI = handler.getNamespaceForPrefix("");
		for (int i = 0; i < nQNames; i++)
		{
		  string tok = tokenizer.nextToken();
		  if (tok.IndexOf(':') == -1)
		  {
			qnames.Add(new QName(defaultURI,tok));
		  }
		  else
		  {
			qnames.Add(new QName(tok, handler));
		  }
		}
		return qnames;
	  }

	  /// <summary>
	  /// Process an attribute string of type T_SIMPLEPATTERNLIST into
	  /// a vector of XPath match patterns.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A whitespace delimited list of simple match patterns.
	  /// </param>
	  /// <returns> A Vector of XPath objects.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if one of the match pattern
	  /// strings contains a syntax error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: java.util.Vector processSIMPLEPATTERNLIST(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual ArrayList processSIMPLEPATTERNLIST(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		try
		{
		  StringTokenizer tokenizer = new StringTokenizer(value, " \t\n\r\f");
		  int nPatterns = tokenizer.countTokens();
		  ArrayList patterns = new ArrayList(nPatterns);

		  for (int i = 0; i < nPatterns; i++)
		  {
			XPath pattern = handler.createMatchPatternXPath(tokenizer.nextToken(), owner);

			patterns.Add(pattern);
		  }

		  return patterns;
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Process an attribute string of type T_STRINGLIST into
	  /// a vector of XPath match patterns.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> a whitespace delimited list of string values.
	  /// </param>
	  /// <returns> A StringVector of the tokenized strings. </returns>
	  internal virtual StringVector processSTRINGLIST(StylesheetHandler handler, string uri, string name, string rawName, string value)
	  {

		StringTokenizer tokenizer = new StringTokenizer(value, " \t\n\r\f");
		int nStrings = tokenizer.countTokens();
		StringVector strings = new StringVector(nStrings);

		for (int i = 0; i < nStrings; i++)
		{
		  strings.addElement(tokenizer.nextToken());
		}

		return strings;
	  }

	  /// <summary>
	  /// Process an attribute string of type T_URLLIST into
	  /// a vector of prefixes that may be resolved to URLs.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A list of whitespace delimited prefixes.
	  /// </param>
	  /// <returns> A vector of strings that may be resolved to URLs.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if one of the prefixes can not be resolved. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: org.apache.xml.utils.StringVector processPREFIX_URLLIST(StylesheetHandler handler, String uri, String name, String rawName, String value) throws org.xml.sax.SAXException
	  internal virtual StringVector processPREFIX_URLLIST(StylesheetHandler handler, string uri, string name, string rawName, string value)
	  {

		StringTokenizer tokenizer = new StringTokenizer(value, " \t\n\r\f");
		int nStrings = tokenizer.countTokens();
		StringVector strings = new StringVector(nStrings);

		for (int i = 0; i < nStrings; i++)
		{
		  string prefix = tokenizer.nextToken();
		  string url = handler.getNamespaceForPrefix(prefix);

		  if (!string.ReferenceEquals(url, null))
		  {
			strings.addElement(url);
		  }
		  else
		  {
			throw new org.xml.sax.SAXException(XSLMessages.createMessage(XSLTErrorResources.ER_CANT_RESOLVE_NSPREFIX, new object[] {prefix}));
		  }

		}

		return strings;
	  }

	  /// <summary>
	  /// Process an attribute string of type T_PREFIXLIST into
	  /// a vector of prefixes that may be resolved to URLs.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A list of whitespace delimited prefixes.
	  /// </param>
	  /// <returns> A vector of strings that may be resolved to URLs.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if one of the prefixes can not be resolved. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: org.apache.xml.utils.StringVector processPREFIX_LIST(StylesheetHandler handler, String uri, String name, String rawName, String value) throws org.xml.sax.SAXException
	   internal virtual StringVector processPREFIX_LIST(StylesheetHandler handler, string uri, string name, string rawName, string value)
	   {

		 StringTokenizer tokenizer = new StringTokenizer(value, " \t\n\r\f");
		 int nStrings = tokenizer.countTokens();
		 StringVector strings = new StringVector(nStrings);

		 for (int i = 0; i < nStrings; i++)
		 {
		   string prefix = tokenizer.nextToken();
		   string url = handler.getNamespaceForPrefix(prefix);
		   if (prefix.Equals(Constants.ATTRVAL_DEFAULT_PREFIX) || !string.ReferenceEquals(url, null))
		   {
			 strings.addElement(prefix);
		   }
		   else
		   {
			 throw new org.xml.sax.SAXException(XSLMessages.createMessage(XSLTErrorResources.ER_CANT_RESOLVE_NSPREFIX, new object[] {prefix}));
		   }

		 }

		 return strings;
	   }


	  /// <summary>
	  /// Process an attribute string of type T_URL into
	  /// a URL value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> non-null string that conforms to the URL syntax.
	  /// </param>
	  /// <returns> The non-absolutized URL argument, in other words, the value argument.  If this 
	  ///         attribute supports AVT, an AVT is returned.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the URL does not conform to the URL syntax. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processURL(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processURL(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		if (SupportsAVT)
		{
			try
			{
			  AVT avt = new AVT(handler, uri, name, rawName, value, owner);

			  // If an AVT wasn't used, validate the value
			 // if (avt.getSimpleString() != null) {
				   // TODO: syntax check URL value.
					// return SystemIDResolver.getAbsoluteURI(value, 
					//                                         handler.getBaseIdentifier());
			  //}	
			  return avt;
			}
			catch (TransformerException te)
			{
			  throw new org.xml.sax.SAXException(te);
			}
		}
		 else
		 {
		// TODO: syntax check URL value.
		// return SystemIDResolver.getAbsoluteURI(value, 
		//                                         handler.getBaseIdentifier());

			return value;
		 }
	  }

	  /// <summary>
	  /// Process an attribute string of type T_YESNO into
	  /// a Boolean value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> A string that should be "yes" or "no".
	  /// </param>
	  /// <returns> Boolean object representation of the value.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private System.Nullable<bool> processYESNO(StylesheetHandler handler, String uri, String name, String rawName, String value) throws org.xml.sax.SAXException
	  private bool? processYESNO(StylesheetHandler handler, string uri, string name, string rawName, string value)
	  {

		// Is this already checked somewhere else?  -sb
		if (!(value.Equals("yes") || value.Equals("no")))
		{
		  handleError(handler, XSLTErrorResources.INVALID_BOOLEAN, new object[] {name, value}, null);
		  return null;
		}

		 return new bool?(value.Equals("yes") ? true : false);
	  }

	  /// <summary>
	  /// Process an attribute value.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="name"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="value"> The unprocessed string value of the attribute.
	  /// </param>
	  /// <returns> The processed Object representation of the attribute.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the attribute value can not be processed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: Object processValue(StylesheetHandler handler, String uri, String name, String rawName, String value, org.apache.xalan.templates.ElemTemplateElement owner) throws org.xml.sax.SAXException
	  internal virtual object processValue(StylesheetHandler handler, string uri, string name, string rawName, string value, ElemTemplateElement owner)
	  {

		int type = Type;
		object processedValue = null;

		switch (type)
		{
		case T_AVT :
		  processedValue = processAVT(handler, uri, name, rawName, value, owner);
		  break;
		case T_CDATA :
		  processedValue = processCDATA(handler, uri, name, rawName, value, owner);
		  break;
		case T_CHAR :
		  processedValue = processCHAR(handler, uri, name, rawName, value, owner);
		  break;
		case T_ENUM :
		  processedValue = processENUM(handler, uri, name, rawName, value, owner);
		  break;
		case T_EXPR :
		  processedValue = processEXPR(handler, uri, name, rawName, value, owner);
		  break;
		case T_NMTOKEN :
		  processedValue = processNMTOKEN(handler, uri, name, rawName, value, owner);
		  break;
		case T_PATTERN :
		  processedValue = processPATTERN(handler, uri, name, rawName, value, owner);
		  break;
		case T_NUMBER :
		  processedValue = processNUMBER(handler, uri, name, rawName, value, owner);
		  break;
		case T_QNAME :
		  processedValue = processQNAME(handler, uri, name, rawName, value, owner);
		  break;
		case T_QNAMES :
		  processedValue = processQNAMES(handler, uri, name, rawName, value);
		  break;
		case T_QNAMES_RESOLVE_NULL:
		  processedValue = processQNAMESRNU(handler, uri, name, rawName, value);
		  break;
		case T_SIMPLEPATTERNLIST :
		  processedValue = processSIMPLEPATTERNLIST(handler, uri, name, rawName, value, owner);
		  break;
		case T_URL :
		  processedValue = processURL(handler, uri, name, rawName, value, owner);
		  break;
		case T_YESNO :
		  processedValue = processYESNO(handler, uri, name, rawName, value);
		  break;
		case T_STRINGLIST :
		  processedValue = processSTRINGLIST(handler, uri, name, rawName, value);
		  break;
		case T_PREFIX_URLLIST :
		  processedValue = processPREFIX_URLLIST(handler, uri, name, rawName, value);
		  break;
		case T_ENUM_OR_PQNAME :
			processedValue = processENUM_OR_PQNAME(handler, uri, name, rawName, value, owner);
			break;
		case T_NCNAME :
			processedValue = processNCNAME(handler, uri, name, rawName, value, owner);
			break;
		case T_AVT_QNAME :
			processedValue = processAVT_QNAME(handler, uri, name, rawName, value, owner);
			break;
		case T_PREFIXLIST :
		  processedValue = processPREFIX_LIST(handler, uri, name, rawName, value);
		  break;

		default :
		}

		return processedValue;
	  }

	  /// <summary>
	  /// Set the default value of an attribute.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="elem"> The object on which the property will be set.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> wraps an invocation exception if the
	  /// setter method can not be invoked on the object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void setDefAttrValue(StylesheetHandler handler, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  internal virtual void setDefAttrValue(StylesheetHandler handler, ElemTemplateElement elem)
	  {
		setAttrValue(handler, this.Namespace, this.Name, this.Name, this.Default, elem);
	  }

	  /// <summary>
	  /// Get the primative type for the class, if there
	  /// is one.  If the class is a Double, for instance,
	  /// this will return double.class.  If the class is not one
	  /// of the 9 primative types, it will return the same
	  /// class that was passed in.
	  /// </summary>
	  /// <param name="obj"> The object which will be resolved to a primative class object if possible.
	  /// </param>
	  /// <returns> The most primative class representation possible for the object, never null. </returns>
	  private Type getPrimativeClass(object obj)
	  {

		if (obj is XPath)
		{
		  return typeof(XPath);
		}

		Type cl = obj.GetType();

		if (cl == typeof(Double))
		{
		  cl = typeof(double);
		}

		if (cl == typeof(Float))
		{
		  cl = typeof(float);
		}
		else if (cl == typeof(Boolean))
		{
		  cl = typeof(bool);
		}
		else if (cl == typeof(Byte))
		{
		  cl = typeof(sbyte);
		}
		else if (cl == typeof(Character))
		{
		  cl = typeof(char);
		}
		else if (cl == typeof(Short))
		{
		  cl = typeof(short);
		}
		else if (cl == typeof(Integer))
		{
		  cl = typeof(int);
		}
		else if (cl == typeof(Long))
		{
		  cl = typeof(long);
		}

		return cl;
	  }

	  /// <summary>
	  /// StringBuffer containing comma delimited list of valid values for ENUM type.
	  /// Used to build error message.
	  /// </summary>
	  private StringBuilder ListOfEnums
	  {
		  get
		  {
			 StringBuilder enumNamesList = new StringBuilder();
			 string[] enumValues = this.EnumNames;
    
			 for (int i = 0; i < enumValues.Length; i++)
			 {
				if (i > 0)
				{
				   enumNamesList.Append(' ');
				}
				enumNamesList.Append(enumValues[i]);
			 }
			return enumNamesList;
		  }
	  }

	  /// <summary>
	  /// Set a value on an attribute.
	  /// </summary>
	  /// <param name="handler"> non-null reference to current StylesheetHandler that is constructing the Templates. </param>
	  /// <param name="attrUri"> The Namespace URI of the attribute, or an empty string. </param>
	  /// <param name="attrLocalName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="attrRawName"> The raw name of the attribute, including possible prefix. </param>
	  /// <param name="attrValue"> The attribute's value. </param>
	  /// <param name="elem"> The object that should contain a property that represents the attribute.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: boolean setAttrValue(StylesheetHandler handler, String attrUri, String attrLocalName, String attrRawName, String attrValue, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  internal virtual bool setAttrValue(StylesheetHandler handler, string attrUri, string attrLocalName, string attrRawName, string attrValue, ElemTemplateElement elem)
	  {
		if (attrRawName.Equals("xmlns") || attrRawName.StartsWith("xmlns:", StringComparison.Ordinal))
		{
		  return true;
		}

		string setterString = SetterMethodName;

		// If this is null, then it is a foreign namespace and we 
		// do not process it.
		if (null != setterString)
		{
		  try
		  {
			System.Reflection.MethodInfo meth;
			object[] args;

			if (setterString.Equals(S_FOREIGNATTR_SETTER))
			{
			  // workaround for possible crimson bug
			  if (string.ReferenceEquals(attrUri, null))
			  {
				  attrUri = "";
			  }
			  // First try to match with the primative value.
			  Type sclass = attrUri.GetType();
			  Type[] argTypes = new Type[]{sclass, sclass, sclass, sclass};

			  meth = elem.GetType().GetMethod(setterString, argTypes);

			  args = new object[]{attrUri, attrLocalName, attrRawName, attrValue};
			}
			else
			{
			  object value = processValue(handler, attrUri, attrLocalName, attrRawName, attrValue, elem);
			  // If a warning was issued because the value for this attribute was
			  // invalid, then the value will be null.  Just return
			  if (null == value)
			  {
				  return false;
			  }

			  // First try to match with the primative value.
			  Type[] argTypes = new Type[]{getPrimativeClass(value)};

			  try
			  {
				meth = elem.GetType().GetMethod(setterString, argTypes);
			  }
			  catch (NoSuchMethodException)
			  {
				Type cl = ((object) value).GetType();

				// If this doesn't work, try it with the non-primative value;
				argTypes[0] = cl;
				meth = elem.GetType().GetMethod(setterString, argTypes);
			  }

			  args = new object[]{value};
			}

			meth.invoke(elem, args);
		  }
		  catch (NoSuchMethodException nsme)
		  {
			if (!setterString.Equals(S_FOREIGNATTR_SETTER))
			{
			  handler.error(XSLTErrorResources.ER_FAILED_CALLING_METHOD, new object[]{setterString}, nsme); //"Failed calling " + setterString + " method!", nsme);
			  return false;
			}
		  }
		  catch (IllegalAccessException iae)
		  {
			handler.error(XSLTErrorResources.ER_FAILED_CALLING_METHOD, new object[]{setterString}, iae); //"Failed calling " + setterString + " method!", iae);
			return false;
		  }
		  catch (InvocationTargetException nsme)
		  {
			handleError(handler, XSLTErrorResources.WG_ILLEGAL_ATTRIBUTE_VALUE, new object[]{Constants.ATTRNAME_NAME, Name}, nsme);
			return false;
		  }
		}

		return true;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void handleError(StylesheetHandler handler, String msg, Object [] args, Exception exc) throws org.xml.sax.SAXException
	  private void handleError(StylesheetHandler handler, string msg, object[] args, Exception exc)
	  {
		switch (ErrorType)
		{
			case (FATAL):
			case (ERROR):
					handler.error(msg, args, exc);
					break;
			case (WARNING):
					handler.warn(msg, args);
				goto default;
			default:
				break;
		}
	  }
	}

}