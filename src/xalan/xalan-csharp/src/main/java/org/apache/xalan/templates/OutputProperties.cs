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
 * $Id: OutputProperties.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{



	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using OutputPropertiesFactory = org.apache.xml.serializer.OutputPropertiesFactory;
	using OutputPropertyUtils = org.apache.xml.serializer.OutputPropertyUtils;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using QName = org.apache.xml.utils.QName;

	/// <summary>
	/// This class provides information from xsl:output elements. It is mainly
	/// a wrapper for <seealso cref="java.util.Properties"/>, but can not extend that class
	/// because it must be part of the <seealso cref="org.apache.xalan.templates.ElemTemplateElement"/>
	/// heararchy.
	/// <para>An OutputProperties list can contain another OutputProperties list as
	/// its "defaults"; this second property list is searched if the property key
	/// is not found in the original property list.</para> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#dtd">XSLT DTD</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#output">xsl:output in XSLT Specification</a>
	///  </seealso>
	[Serializable]
	public class OutputProperties : ElemTemplateElement, ICloneable
	{
		internal new const long serialVersionUID = -6975274363881785488L;
	  /// <summary>
	  /// Creates an empty OutputProperties with no default values.
	  /// </summary>
	  public OutputProperties() : this(org.apache.xml.serializer.Method.XML)
	  {
	  }

	  /// <summary>
	  /// Creates an empty OutputProperties with the specified defaults.
	  /// </summary>
	  /// <param name="defaults">   the defaults. </param>
	  public OutputProperties(Properties defaults)
	  {
		m_properties = new Properties(defaults);
	  }

	  /// <summary>
	  /// Creates an empty OutputProperties with the defaults specified by
	  /// a property file.  The method argument is used to construct a string of
	  /// the form output_[method].properties (for instance, output_html.properties).
	  /// The output_xml.properties file is always used as the base.
	  /// <para>At the moment, anything other than 'text', 'xml', and 'html', will
	  /// use the output_xml.properties file.</para>
	  /// </summary>
	  /// <param name="method"> non-null reference to method name. </param>
	  public OutputProperties(string method)
	  {
		m_properties = new Properties(OutputPropertiesFactory.getDefaultMethodProperties(method));
	  }

	  /// <summary>
	  /// Clone this OutputProperties, including a clone of the wrapped Properties
	  /// reference.
	  /// </summary>
	  /// <returns> A new OutputProperties reference, mutation of which should not
	  ///         effect this object. </returns>
	  public virtual object clone()
	  {

		try
		{
		  OutputProperties cloned = (OutputProperties) base.clone();

		  cloned.m_properties = (Properties) cloned.m_properties.clone();

		  return cloned;
		}
		catch (CloneNotSupportedException)
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Set an output property.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setProperty(QName key, string value)
	  {
		setProperty(key.toNamespacedString(), value);
	  }

	  /// <summary>
	  /// Set an output property.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setProperty(string key, string value)
	  {
		if (key.Equals(OutputKeys.METHOD))
		{
		  MethodDefaults = value;
		}

		if (key.StartsWith(OutputPropertiesFactory.S_BUILTIN_OLD_EXTENSIONS_UNIVERSAL, StringComparison.Ordinal))
		{
		  key = OutputPropertiesFactory.S_BUILTIN_EXTENSIONS_UNIVERSAL + key.Substring(OutputPropertiesFactory.S_BUILTIN_OLD_EXTENSIONS_UNIVERSAL_LEN);
		}

		m_properties.put(key, value);
	  }

	  /// <summary>
	  /// Searches for the property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list with the specified key value. </returns>
	  public virtual string getProperty(QName key)
	  {
		return m_properties.getProperty(key.toNamespacedString());
	  }

	  /// <summary>
	  /// Searches for the property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list with the specified key value. </returns>
	  public virtual string getProperty(string key)
	  {
		if (key.StartsWith(OutputPropertiesFactory.S_BUILTIN_OLD_EXTENSIONS_UNIVERSAL, StringComparison.Ordinal))
		{
		  key = OutputPropertiesFactory.S_BUILTIN_EXTENSIONS_UNIVERSAL + key.Substring(OutputPropertiesFactory.S_BUILTIN_OLD_EXTENSIONS_UNIVERSAL_LEN);
		}
		return m_properties.getProperty(key);
	  }

	  /// <summary>
	  /// Set an output property.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setBooleanProperty(QName key, bool value)
	  {
		m_properties.put(key.toNamespacedString(), value ? "yes" : "no");
	  }

	  /// <summary>
	  /// Set an output property.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setBooleanProperty(string key, bool value)
	  {
		m_properties.put(key, value ? "yes" : "no");
	  }

	  /// <summary>
	  /// Searches for the boolean property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>false</code> if the property is not found, or if the value is other
	  /// than "yes".
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a boolean value, or false
	  /// if null or not "yes". </returns>
	  public virtual bool getBooleanProperty(QName key)
	  {
		return getBooleanProperty(key.toNamespacedString());
	  }

	  /// <summary>
	  /// Searches for the boolean property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>false</code> if the property is not found, or if the value is other
	  /// than "yes".
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a boolean value, or false
	  /// if null or not "yes". </returns>
	  public virtual bool getBooleanProperty(string key)
	  {
		return OutputPropertyUtils.getBooleanProperty(key, m_properties);
	  }

	  /// <summary>
	  /// Set an output property.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setIntProperty(QName key, int value)
	  {
		setIntProperty(key.toNamespacedString(), value);
	  }

	  /// <summary>
	  /// Set an output property.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setIntProperty(string key, int value)
	  {
		m_properties.put(key, Convert.ToString(value));
	  }

	  /// <summary>
	  /// Searches for the int property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>false</code> if the property is not found, or if the value is other
	  /// than "yes".
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a int value, or false
	  /// if null or not a number. </returns>
	  public virtual int getIntProperty(QName key)
	  {
		return getIntProperty(key.toNamespacedString());
	  }

	  /// <summary>
	  /// Searches for the int property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>false</code> if the property is not found, or if the value is other
	  /// than "yes".
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a int value, or false
	  /// if null or not a number. </returns>
	  public virtual int getIntProperty(string key)
	  {
		return OutputPropertyUtils.getIntProperty(key, m_properties);
	  }


	  /// <summary>
	  /// Set an output property with a QName value.  The QName will be turned
	  /// into a string with the namespace in curly brackets.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setQNameProperty(QName key, QName value)
	  {
		setQNameProperty(key.toNamespacedString(), value);
	  }

	  /// <summary>
	  /// Reset the default properties based on the method.
	  /// </summary>
	  /// <param name="method"> the method value. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual string MethodDefaults
	  {
		  set
		  {
				string defaultMethod = m_properties.getProperty(OutputKeys.METHOD);
    
				if ((null == defaultMethod) || !defaultMethod.Equals(value) || defaultMethod.Equals("xml"))
				 // bjm - add the next condition as a hack
				 // but it is because both output_xml.properties and
				 // output_unknown.properties have the same value=xml
				 // for their default. Otherwise we end up with
				 // a ToUnknownStream wraping a ToXMLStream even
				 // when the users says value="xml"
				 //
				{
					Properties savedProps = m_properties;
					Properties newDefaults = OutputPropertiesFactory.getDefaultMethodProperties(value);
					m_properties = new Properties(newDefaults);
					copyFrom(savedProps, false);
				}
		  }
	  }


	  /// <summary>
	  /// Set an output property with a QName value.  The QName will be turned
	  /// into a string with the namespace in curly brackets.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="value"> the value corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setQNameProperty(string key, QName value)
	  {
		setProperty(key, value.toNamespacedString());
	  }

	  /// <summary>
	  /// Searches for the qname property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a QName value, or false
	  /// if null or not "yes". </returns>
	  public virtual QName getQNameProperty(QName key)
	  {
		return getQNameProperty(key.toNamespacedString());
	  }

	  /// <summary>
	  /// Searches for the qname property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a QName value, or false
	  /// if null or not "yes". </returns>
	  public virtual QName getQNameProperty(string key)
	  {
		return getQNameProperty(key, m_properties);
	  }

	  /// <summary>
	  /// Searches for the qname property with the specified key in the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <param name="props"> the list of properties to search in. </param>
	  /// <returns>  the value in this property list as a QName value, or false
	  /// if null or not "yes". </returns>
	  public static QName getQNameProperty(string key, Properties props)
	  {

		string s = props.getProperty(key);

		if (null != s)
		{
		  return QName.getQNameFromString(s);
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Set an output property with a QName list value.  The QNames will be turned
	  /// into strings with the namespace in curly brackets.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="v"> non-null list of QNames corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setQNameProperties(QName key, ArrayList v)
	  {
		setQNameProperties(key.toNamespacedString(), v);
	  }

	  /// <summary>
	  /// Set an output property with a QName list value.  The QNames will be turned
	  /// into strings with the namespace in curly brackets.
	  /// </summary>
	  /// <param name="key"> the key to be placed into the property list. </param>
	  /// <param name="v"> non-null list of QNames corresponding to <tt>key</tt>. </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  public virtual void setQNameProperties(string key, ArrayList v)
	  {

		int s = v.Count;

		// Just an initial guess at reasonable tuning parameters
		FastStringBuffer fsb = new FastStringBuffer(9,9);

		for (int i = 0; i < s; i++)
		{
		  QName qname = (QName) v[i];

		  fsb.append(qname.toNamespacedString());
		  // Don't append space after last value
		  if (i < s - 1)
		  {
			fsb.append(' ');
		  }
		}

		m_properties.put(key, fsb.ToString());
	  }

	  /// <summary>
	  /// Searches for the list of qname properties with the specified key in
	  /// the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a vector of QNames, or false
	  /// if null or not "yes". </returns>
	  public virtual ArrayList getQNameProperties(QName key)
	  {
		return getQNameProperties(key.toNamespacedString());
	  }

	  /// <summary>
	  /// Searches for the list of qname properties with the specified key in
	  /// the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <returns>  the value in this property list as a vector of QNames, or false
	  /// if null or not "yes". </returns>
	  public virtual ArrayList getQNameProperties(string key)
	  {
		return getQNameProperties(key, m_properties);
	  }

	  /// <summary>
	  /// Searches for the list of qname properties with the specified key in
	  /// the property list.
	  /// If the key is not found in this property list, the default property list,
	  /// and its defaults, recursively, are then checked. The method returns
	  /// <code>null</code> if the property is not found.
	  /// </summary>
	  /// <param name="key">   the property key. </param>
	  /// <param name="props"> the list of properties to search in. </param>
	  /// <returns>  the value in this property list as a vector of QNames, or false
	  /// if null or not "yes". </returns>
	  public static ArrayList getQNameProperties(string key, Properties props)
	  {

		string s = props.getProperty(key);

		if (null != s)
		{
		  ArrayList v = new ArrayList();
		  int l = s.Length;
		  bool inCurly = false;
		  FastStringBuffer buf = new FastStringBuffer();

		  // parse through string, breaking on whitespaces.  I do this instead 
		  // of a tokenizer so I can track whitespace inside of curly brackets, 
		  // which theoretically shouldn't happen if they contain legal URLs.
		  for (int i = 0; i < l; i++)
		  {
			char c = s[i];

			if (char.IsWhiteSpace(c))
			{
			  if (!inCurly)
			  {
				if (buf.length() > 0)
				{
				  QName qname = QName.getQNameFromString(buf.ToString());
				  v.Add(qname);
				  buf.reset();
				}
				continue;
			  }
			}
			else if ('{' == c)
			{
			  inCurly = true;
			}
			else if ('}' == c)
			{
			  inCurly = false;
			}

			buf.append(c);
		  }

		  if (buf.length() > 0)
		  {
			QName qname = QName.getQNameFromString(buf.ToString());
			v.Add(qname);
			buf.reset();
		  }

		  return v;
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// This function is called to recompose all of the output format extended elements.
	  /// </summary>
	  /// <param name="root"> non-null reference to the stylesheet root object. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void recompose(StylesheetRoot root) throws javax.xml.transform.TransformerException
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeOutput(this);
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

	  }

	  /// <summary>
	  /// Get the Properties object that this class wraps.
	  /// </summary>
	  /// <returns> non-null reference to Properties object. </returns>
	  public virtual Properties Properties
	  {
		  get
		  {
			return m_properties;
		  }
	  }

	  /// <summary>
	  /// Copy the keys and values from the source to this object.  This will
	  /// not copy the default values.  This is meant to be used by going from
	  /// a higher precedence object to a lower precedence object, so that if a
	  /// key already exists, this method will not reset it.
	  /// </summary>
	  /// <param name="src"> non-null reference to the source properties. </param>
	  public virtual void copyFrom(Properties src)
	  {
		copyFrom(src, true);
	  }

	  /// <summary>
	  /// Copy the keys and values from the source to this object.  This will
	  /// not copy the default values.  This is meant to be used by going from
	  /// a higher precedence object to a lower precedence object, so that if a
	  /// key already exists, this method will not reset it.
	  /// </summary>
	  /// <param name="src"> non-null reference to the source properties. </param>
	  /// <param name="shouldResetDefaults"> true if the defaults should be reset based on 
	  ///                            the method property. </param>
	  public virtual void copyFrom(Properties src, bool shouldResetDefaults)
	  {

		System.Collections.IEnumerator keys = src.keys();

		while (keys.MoveNext())
		{
		  string key = (string) keys.Current;

		  if (!isLegalPropertyKey(key))
		  {
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, new object[]{key})); //"output property not recognized: "
		  }

		  object oldValue = m_properties.get(key);
		  if (null == oldValue)
		  {
			string val = (string) src.get(key);

			if (shouldResetDefaults && key.Equals(OutputKeys.METHOD))
			{
			  MethodDefaults = val;
			}

			m_properties.put(key, val);
		  }
		  else if (key.Equals(OutputKeys.CDATA_SECTION_ELEMENTS))
		  {
			m_properties.put(key, (string) oldValue + " " + (string) src.get(key));
		  }
		}
	  }

	  /// <summary>
	  /// Copy the keys and values from the source to this object.  This will
	  /// not copy the default values.  This is meant to be used by going from
	  /// a higher precedence object to a lower precedence object, so that if a
	  /// key already exists, this method will not reset it.
	  /// </summary>
	  /// <param name="opsrc"> non-null reference to an OutputProperties. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copyFrom(OutputProperties opsrc) throws javax.xml.transform.TransformerException
	  public virtual void copyFrom(OutputProperties opsrc)
	  {
	   // Bugzilla 6157: recover from xsl:output statements
		// checkDuplicates(opsrc);
		copyFrom(opsrc.Properties);
	  }

	  /// <summary>
	  /// Report if the key given as an argument is a legal xsl:output key.
	  /// </summary>
	  /// <param name="key"> non-null reference to key name.
	  /// </param>
	  /// <returns> true if key is legal. </returns>
	  public static bool isLegalPropertyKey(string key)
	  {

		return (key.Equals(OutputKeys.CDATA_SECTION_ELEMENTS) || key.Equals(OutputKeys.DOCTYPE_PUBLIC) || key.Equals(OutputKeys.DOCTYPE_SYSTEM) || key.Equals(OutputKeys.ENCODING) || key.Equals(OutputKeys.INDENT) || key.Equals(OutputKeys.MEDIA_TYPE) || key.Equals(OutputKeys.METHOD) || key.Equals(OutputKeys.OMIT_XML_DECLARATION) || key.Equals(OutputKeys.STANDALONE) || key.Equals(OutputKeys.VERSION) || (key.Length > 0) && (key[0] == '{') && (key.LastIndexOf('{') == 0) && (key.IndexOf('}') > 0) && (key.LastIndexOf('}') == key.IndexOf('}')));
	  }

	  /// <summary>
	  /// The output properties.
	  ///  @serial 
	  /// </summary>
	  private Properties m_properties = null;

		/// <summary>
		/// Creates an empty OutputProperties with the defaults specified by
		/// a property file.  The method argument is used to construct a string of
		/// the form output_[method].properties (for instance, output_html.properties).
		/// The output_xml.properties file is always used as the base.
		/// <para>At the moment, anything other than 'text', 'xml', and 'html', will
		/// use the output_xml.properties file.</para>
		/// </summary>
		/// <param name="method"> non-null reference to method name.
		/// </param>
		/// <returns> Properties object that holds the defaults for the given method.
		/// </returns>
		/// @deprecated Use org.apache.xml.serializer.OuputPropertiesFactory.
		/// getDefaultMethodProperties directly. 
		public static Properties getDefaultMethodProperties(string method)
		{
			return OutputPropertiesFactory.getDefaultMethodProperties(method);
		}
	}

}