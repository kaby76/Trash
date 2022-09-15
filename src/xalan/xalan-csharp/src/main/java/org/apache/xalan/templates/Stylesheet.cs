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
 * $Id: Stylesheet.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{


	using DTM = org.apache.xml.dtm.DTM;
	using QName = org.apache.xml.utils.QName;
	using StringVector = org.apache.xml.utils.StringVector;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;

	/// <summary>
	/// Represents a stylesheet element.
	/// <para>All properties in this class have a fixed form of bean-style property
	/// accessors for all properties that represent XSL attributes or elements.
	/// These properties have setter method names accessed generically by the
	/// processor, and so these names must be fixed according to the system
	/// defined in the <a href="XSLTAttributeDef#getSetterMethodName">getSetterMethodName</a>
	/// function.</para>
	/// <para><pre>
	/// <!ENTITY % top-level "
	///  (xsl:import*,
	///   (xsl:include
	///   | xsl:strip-space
	///   | xsl:preserve-space
	///   | xsl:output
	///   | xsl:key
	///   | xsl:decimal-format
	///   | xsl:attribute-set
	///   | xsl:variable
	///   | xsl:param
	///   | xsl:template
	///   | xsl:namespace-alias
	///   %non-xsl-top-level;)*)
	/// ">
	/// 
	/// <!ENTITY % top-level-atts '
	///   extension-element-prefixes CDATA #IMPLIED
	///   exclude-result-prefixes CDATA #IMPLIED
	///   id ID #IMPLIED
	///   version NMTOKEN #REQUIRED
	///   xmlns:xsl CDATA #FIXED "http://www.w3.org/1999/XSL/Transform"
	///   %space-att;
	/// '>
	/// 
	/// <!ELEMENT xsl:stylesheet %top-level;>
	/// <!ATTLIST xsl:stylesheet %top-level-atts;>
	/// 
	/// <!ELEMENT xsl:transform %top-level;>
	/// <!ATTLIST xsl:transform %top-level-atts;>
	/// 
	/// </para></pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Stylesheet-Structure">section-Stylesheet-Structure in XSLT Specification</a>"/>
	[Serializable]
	public class Stylesheet : ElemTemplateElement // , Document
	{
		internal new const long serialVersionUID = 2085337282743043776L;

	  /// <summary>
	  /// Constructor for a Stylesheet. </summary>
	  /// <param name="parent">  The including or importing stylesheet. </param>
	  public Stylesheet(Stylesheet parent)
	  {

		if (null != parent)
		{
		  m_stylesheetParent = parent;
		  m_stylesheetRoot = parent.StylesheetRoot;
		}
	  }

	  /// <summary>
	  /// Get the owning stylesheet.  This looks up the
	  /// inheritance chain until it calls getStylesheet
	  /// on a Stylesheet object, which will return itself.
	  /// </summary>
	  /// <returns> The owning stylesheet, itself. </returns>
	  public override Stylesheet Stylesheet
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Tell if this can be cast to a StylesheetComposed, meaning, you
	  /// can ask questions from getXXXComposed functions.
	  /// </summary>
	  /// <returns> False if this is not a StylesheetComposed </returns>
	  public virtual bool AggregatedType
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Tell if this is the root of the stylesheet tree.
	  /// </summary>
	  /// <returns> False is this is not the root of the stylesheet tree. </returns>
	  public virtual bool Root
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Extension to be used when serializing to disk.
	  /// </summary>
	  public const string STYLESHEET_EXT = ".lxc";

	  /// <summary>
	  /// Read the stylesheet from a serialization stream.
	  /// </summary>
	  /// <param name="stream"> Input stream to read from
	  /// </param>
	  /// <exception cref="IOException"> </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream stream) throws IOException, javax.xml.transform.TransformerException
	  private void readObject(ObjectInputStream stream)
	  {

		// System.out.println("Reading Stylesheet");
		try
		{
		  stream.defaultReadObject();
		}
		catch (ClassNotFoundException cnfe)
		{
		  throw new TransformerException(cnfe);
		}

		// System.out.println("Done reading Stylesheet");
	  }

	  /// <summary>
	  /// Write out the given output stream 
	  /// 
	  /// </summary>
	  /// <param name="stream"> The output stream to write out
	  /// </param>
	  /// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream stream) throws java.io.IOException
	  private void writeObject(ObjectOutputStream stream)
	  {

		// System.out.println("Writing Stylesheet");
		stream.defaultWriteObject();

		// System.out.println("Done writing Stylesheet");
	  }

	  //============== XSLT Properties =================

	  /// <summary>
	  /// The "xmlns:xsl" property.
	  /// @serial
	  /// </summary>
	  private string m_XmlnsXsl;

	  /// <summary>
	  /// Set the "xmlns:xsl" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.xslt-namespace">xslt-namespace in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> The value to be set for the "xmlns:xsl" property. </param>
	  public virtual string XmlnsXsl
	  {
		  set
		  {
			m_XmlnsXsl = value;
		  }
		  get
		  {
			return m_XmlnsXsl;
		  }
	  }

	  /// <summary>
	  /// Get the "xmlns:xsl" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.xslt-namespace">xslt-namespace in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// The "extension-element-prefixes" property, actually contains URIs.
	  /// @serial
	  /// </summary>
	  private StringVector m_ExtensionElementURIs;

	  /// <summary>
	  /// Set the "extension-element-prefixes" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.extension-element">extension-element in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> The value to be set for the "extension-element-prefixes" 
	  /// property: a vector of extension element URIs. </param>
	  public virtual StringVector ExtensionElementPrefixes
	  {
		  set
		  {
			m_ExtensionElementURIs = value;
		  }
	  }

	  /// <summary>
	  /// Get and "extension-element-prefix" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.extension-element">extension-element in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of extension element URI in list 
	  /// </param>
	  /// <returns> The extension element URI at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
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
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.extension-element">extension-element in XSLT Specification</a>"
	  ////>
	  /// <returns> Number of URIs in the list </returns>
	  public virtual int ExtensionElementPrefixCount
	  {
		  get
		  {
			return (null != m_ExtensionElementURIs) ? m_ExtensionElementURIs.size() : 0;
		  }
	  }

	  /// <summary>
	  /// Find out if this contains a given "extension-element-prefix" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.extension-element">extension-element in XSLT Specification</a>"
	  ////>
	  /// <param name="uri"> URI of extension element to look for
	  /// </param>
	  /// <returns> True if the given URI was found in the list  </returns>
	  public virtual bool containsExtensionElementURI(string uri)
	  {

		if (null == m_ExtensionElementURIs)
		{
		  return false;
		}

		return m_ExtensionElementURIs.contains(uri);
	  }

	  /// <summary>
	  /// The "exclude-result-prefixes" property.
	  /// @serial
	  /// </summary>
	  private StringVector m_ExcludeResultPrefixs;

	  /// <summary>
	  /// Set the "exclude-result-prefixes" property.
	  /// The designation of a namespace as an excluded namespace is
	  /// effective within the subtree of the stylesheet rooted at
	  /// the element bearing the exclude-result-prefixes or
	  /// xsl:exclude-result-prefixes attribute; a subtree rooted
	  /// at an xsl:stylesheet element does not include any stylesheets
	  /// imported or included by children of that xsl:stylesheet element. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> A StringVector of prefixes to exclude  </param>
	  public virtual StringVector ExcludeResultPrefixes
	  {
		  set
		  {
			m_ExcludeResultPrefixs = value;
		  }
	  }

	  /// <summary>
	  /// Get an "exclude-result-prefix" property.
	  /// The designation of a namespace as an excluded namespace is
	  /// effective within the subtree of the stylesheet rooted at
	  /// the element bearing the exclude-result-prefixes or
	  /// xsl:exclude-result-prefixes attribute; a subtree rooted
	  /// at an xsl:stylesheet element does not include any stylesheets
	  /// imported or included by children of that xsl:stylesheet element. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of prefix to get in list 
	  /// </param>
	  /// <returns> Prefix to be excluded at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getExcludeResultPrefix(int i) throws ArrayIndexOutOfBoundsException
	  public virtual string getExcludeResultPrefix(int i)
	  {

		if (null == m_ExcludeResultPrefixs)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return m_ExcludeResultPrefixs.elementAt(i);
	  }

	  /// <summary>
	  /// Get the number of "exclude-result-prefixes" Strings. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <returns> The number of prefix strings to be excluded.  </returns>
	  public virtual int ExcludeResultPrefixCount
	  {
		  get
		  {
			return (null != m_ExcludeResultPrefixs) ? m_ExcludeResultPrefixs.size() : 0;
		  }
	  }

	  /// <summary>
	  /// Get whether or not the passed prefix is contained flagged by
	  /// the "exclude-result-prefixes" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <param name="prefix"> non-null reference to prefix that might be excluded. </param>
	  /// <param name="uri"> reference to namespace that prefix maps to
	  /// </param>
	  /// <returns> true if the prefix should normally be excluded.> </returns>
	  public override bool containsExcludeResultPrefix(string prefix, string uri)
	  {

		if (null == m_ExcludeResultPrefixs || string.ReferenceEquals(uri, null))
		{
		  return false;
		}

		// This loop is ok here because this code only runs during
		// stylesheet compile time.
		for (int i = 0; i < m_ExcludeResultPrefixs.size(); i++)
		{
		  if (uri.Equals(getNamespaceForPrefix(m_ExcludeResultPrefixs.elementAt(i))))
		  {
			return true;
		  }
		}

		return false;

	  /*  if (prefix.length() == 0)
	      prefix = Constants.ATTRVAL_DEFAULT_PREFIX;
	
	    return m_ExcludeResultPrefixs.contains(prefix); */
	  }

	  /// <summary>
	  /// The "id" property.
	  /// @serial
	  /// </summary>
	  private string m_Id;

	  /// <summary>
	  /// Set the "id" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Embedding-Stylesheets">section-Embedding-Stylesheets in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Value for the "id" property. </param>
	  public virtual string Id
	  {
		  set
		  {
			m_Id = value;
		  }
		  get
		  {
			return m_Id;
		  }
	  }

	  /// <summary>
	  /// Get the "id" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Embedding-Stylesheets">section-Embedding-Stylesheets in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// The "version" property.
	  /// @serial
	  /// </summary>
	  private string m_Version;

	  /// <summary>
	  /// Whether or not the stylesheet is in "Forward Compatibility Mode" 
	  /// @serial
	  /// </summary>
	  private bool m_isCompatibleMode = false;

	  /// <summary>
	  /// Set the "version" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.forwards">forwards in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Value for the "version" property. </param>
	  public virtual string Version
	  {
		  set
		  {
			m_Version = value;
			m_isCompatibleMode = (Convert.ToDouble(value) > Constants.XSLTVERSUPPORTED);
		  }
		  get
		  {
			return m_Version;
		  }
	  }

	  /// <summary>
	  /// Get whether or not the stylesheet is in "Forward Compatibility Mode"
	  /// </summary>
	  /// <returns> true if in forward compatible mode, false otherwise </returns>
	  public virtual bool CompatibleMode
	  {
		  get
		  {
			  return m_isCompatibleMode;
		  }
	  }

	  /// <summary>
	  /// Get the "version" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.forwards">forwards in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// The "xsl:import" list.
	  /// @serial
	  /// </summary>
	  private ArrayList m_imports;

	  /// <summary>
	  /// Add a stylesheet to the "import" list. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Stylesheet to add to the import list </param>
	  public virtual StylesheetComposed Import
	  {
		  set
		  {
    
			if (null == m_imports)
			{
			  m_imports = new ArrayList();
			}
    
			// I'm going to insert the elements in backwards order,
			// so I can walk them 0 to n.
			m_imports.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get a stylesheet from the "import" list. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of the stylesheet to get
	  /// </param>
	  /// <returns> The stylesheet at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public StylesheetComposed getImport(int i) throws ArrayIndexOutOfBoundsException
	  public virtual StylesheetComposed getImport(int i)
	  {

		if (null == m_imports)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (StylesheetComposed) m_imports[i];
	  }

	  /// <summary>
	  /// Get the number of imported stylesheets. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of imported stylesheets. </returns>
	  public virtual int ImportCount
	  {
		  get
		  {
			return (null != m_imports) ? m_imports.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:include" properties.
	  /// @serial
	  /// </summary>
	  private ArrayList m_includes;

	  /// <summary>
	  /// Add a stylesheet to the "include" list. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.include">include in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Stylesheet to add to the "include" list   </param>
	  public virtual Stylesheet Include
	  {
		  set
		  {
    
			if (null == m_includes)
			{
			  m_includes = new ArrayList();
			}
    
			m_includes.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get the stylesheet at the given in index in "include" list </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.include">include in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of stylesheet to get
	  /// </param>
	  /// <returns> Stylesheet at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Stylesheet getInclude(int i) throws ArrayIndexOutOfBoundsException
	  public virtual Stylesheet getInclude(int i)
	  {

		if (null == m_includes)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (Stylesheet) m_includes[i];
	  }

	  /// <summary>
	  /// Get the number of included stylesheets. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of included stylesheets. </returns>
	  public virtual int IncludeCount
	  {
		  get
		  {
			return (null != m_includes) ? m_includes.Count : 0;
		  }
	  }

	  /// <summary>
	  /// Table of tables of element decimal-format. </summary>
	  /// <seealso cref="DecimalFormatProperties"
	  /// @serial/>
	  internal System.Collections.Stack m_DecimalFormatDeclarations;

	  /// <summary>
	  /// Process the xsl:decimal-format element.
	  /// </summary>
	  /// <param name="edf"> Decimal-format element to push into stack   </param>
	  public virtual DecimalFormatProperties DecimalFormat
	  {
		  set
		  {
    
			if (null == m_DecimalFormatDeclarations)
			{
			  m_DecimalFormatDeclarations = new System.Collections.Stack();
			}
    
			// Elements are pushed in by order of importance
			// so that when recomposed, they get overiden properly.
			m_DecimalFormatDeclarations.Push(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:decimal-format" property.
	  /// </summary>
	  /// <seealso cref="DecimalFormatProperties"/>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.format-number">format-number in XSLT Specification</a>"
	  ////>
	  /// <param name="name"> The qualified name of the decimal format property. </param>
	  /// <returns> null if not found, otherwise a DecimalFormatProperties
	  /// object, from which you can get a DecimalFormatSymbols object. </returns>
	  public virtual DecimalFormatProperties getDecimalFormat(QName name)
	  {

		if (null == m_DecimalFormatDeclarations)
		{
		  return null;
		}

		int n = DecimalFormatCount;

		for (int i = (n - 1); i >= 0; i++)
		{
		  DecimalFormatProperties dfp = getDecimalFormat(i);

		  if (dfp.Name.Equals(name))
		  {
			return dfp;
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Get an "xsl:decimal-format" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.format-number">format-number in XSLT Specification</a>"/>
	  /// <seealso cref="DecimalFormatProperties"
	  ////>
	  /// <param name="i"> Index of decimal-format property in stack
	  /// </param>
	  /// <returns> The decimal-format property at the given index 
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public DecimalFormatProperties getDecimalFormat(int i) throws ArrayIndexOutOfBoundsException
	  public virtual DecimalFormatProperties getDecimalFormat(int i)
	  {

		if (null == m_DecimalFormatDeclarations)
		{
		  throw new System.IndexOutOfRangeException();
		}

//JAVA TO C# CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
		return (DecimalFormatProperties) m_DecimalFormatDeclarations.elementAt(i);
	  }

	  /// <summary>
	  /// Get the number of xsl:decimal-format declarations. </summary>
	  /// <seealso cref="DecimalFormatProperties"
	  ////>
	  /// <returns> the number of xsl:decimal-format declarations. </returns>
	  public virtual int DecimalFormatCount
	  {
		  get
		  {
			return (null != m_DecimalFormatDeclarations) ? m_DecimalFormatDeclarations.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:strip-space" properties,
	  /// A lookup table of all space stripping elements.
	  /// @serial
	  /// </summary>
	  private ArrayList m_whitespaceStrippingElements;

	  /// <summary>
	  /// Set the "xsl:strip-space" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="wsi"> WhiteSpaceInfo element to add to list  </param>
	  public virtual WhiteSpaceInfo StripSpaces
	  {
		  set
		  {
    
			if (null == m_whitespaceStrippingElements)
			{
			  m_whitespaceStrippingElements = new ArrayList();
			}
    
			m_whitespaceStrippingElements.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:strip-space" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of WhiteSpaceInfo to get
	  /// </param>
	  /// <returns> WhiteSpaceInfo at given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public WhiteSpaceInfo getStripSpace(int i) throws ArrayIndexOutOfBoundsException
	  public virtual WhiteSpaceInfo getStripSpace(int i)
	  {

		if (null == m_whitespaceStrippingElements)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (WhiteSpaceInfo) m_whitespaceStrippingElements[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:strip-space" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:strip-space" properties. </returns>
	  public virtual int StripSpaceCount
	  {
		  get
		  {
			return (null != m_whitespaceStrippingElements) ? m_whitespaceStrippingElements.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:preserve-space" property,
	  /// A lookup table of all space preserving elements.
	  /// @serial
	  /// </summary>
	  private ArrayList m_whitespacePreservingElements;

	  /// <summary>
	  /// Set the "xsl:preserve-space" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="wsi"> WhiteSpaceInfo element to add to list </param>
	  public virtual WhiteSpaceInfo PreserveSpaces
	  {
		  set
		  {
    
			if (null == m_whitespacePreservingElements)
			{
			  m_whitespacePreservingElements = new ArrayList();
			}
    
			m_whitespacePreservingElements.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get a "xsl:preserve-space" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of WhiteSpaceInfo to get
	  /// </param>
	  /// <returns> WhiteSpaceInfo at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public WhiteSpaceInfo getPreserveSpace(int i) throws ArrayIndexOutOfBoundsException
	  public virtual WhiteSpaceInfo getPreserveSpace(int i)
	  {

		if (null == m_whitespacePreservingElements)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (WhiteSpaceInfo) m_whitespacePreservingElements[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:preserve-space" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:preserve-space" properties. </returns>
	  public virtual int PreserveSpaceCount
	  {
		  get
		  {
			return (null != m_whitespacePreservingElements) ? m_whitespacePreservingElements.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:output" properties.  This is a vector of OutputProperties objects.
	  /// @serial
	  /// </summary>
	  private ArrayList m_output;

	  /// <summary>
	  /// Set the "xsl:output" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.output">output in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> non-null reference to the OutputProperties object to be 
	  ///          added to the collection. </param>
	  public virtual OutputProperties Output
	  {
		  set
		  {
			if (null == m_output)
			{
			  m_output = new ArrayList();
			}
    
			m_output.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:output" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.output">output in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of OutputFormatExtended to get
	  /// </param>
	  /// <returns> non-null reference to an OutputProperties object.
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public OutputProperties getOutput(int i) throws ArrayIndexOutOfBoundsException
	  public virtual OutputProperties getOutput(int i)
	  {

		if (null == m_output)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (OutputProperties) m_output[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:output" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.output">output in XSLT Specification</a>"
	  ////>
	  /// <returns> The number of OutputProperties objects contained in this stylesheet. </returns>
	  public virtual int OutputCount
	  {
		  get
		  {
			return (null != m_output) ? m_output.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:key" property.
	  /// @serial
	  /// </summary>
	  private ArrayList m_keyDeclarations;

	  /// <summary>
	  /// Set the "xsl:key" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.key">key in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> KeyDeclaration element to add to the list of key declarations  </param>
	  public virtual KeyDeclaration Key
	  {
		  set
		  {
    
			if (null == m_keyDeclarations)
			{
			  m_keyDeclarations = new ArrayList();
			}
    
			m_keyDeclarations.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:key" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.key">key in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of KeyDeclaration element to get
	  /// </param>
	  /// <returns> KeyDeclaration element at given index in list 
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public KeyDeclaration getKey(int i) throws ArrayIndexOutOfBoundsException
	  public virtual KeyDeclaration getKey(int i)
	  {

		if (null == m_keyDeclarations)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (KeyDeclaration) m_keyDeclarations[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:key" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.key">key in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:key" properties. </returns>
	  public virtual int KeyCount
	  {
		  get
		  {
			return (null != m_keyDeclarations) ? m_keyDeclarations.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:attribute-set" property.
	  /// @serial
	  /// </summary>
	  private ArrayList m_attributeSets;

	  /// <summary>
	  /// Set the "xsl:attribute-set" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.attribute-sets">attribute-sets in XSLT Specification</a>"
	  ////>
	  /// <param name="attrSet"> ElemAttributeSet to add to the list of attribute sets </param>
	  public virtual ElemAttributeSet AttributeSet
	  {
		  set
		  {
    
			if (null == m_attributeSets)
			{
			  m_attributeSets = new ArrayList();
			}
    
			m_attributeSets.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:attribute-set" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.attribute-sets">attribute-sets in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of ElemAttributeSet to get in list
	  /// </param>
	  /// <returns> ElemAttributeSet at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemAttributeSet getAttributeSet(int i) throws ArrayIndexOutOfBoundsException
	  public virtual ElemAttributeSet getAttributeSet(int i)
	  {

		if (null == m_attributeSets)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (ElemAttributeSet) m_attributeSets[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:attribute-set" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.attribute-sets">attribute-sets in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:attribute-set" properties. </returns>
	  public virtual int AttributeSetCount
	  {
		  get
		  {
			return (null != m_attributeSets) ? m_attributeSets.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:variable" and "xsl:param" properties.
	  /// @serial
	  /// </summary>
	  private ArrayList m_topLevelVariables;

	  /// <summary>
	  /// Set the "xsl:variable" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> ElemVariable object to add to list of top level variables </param>
	  public virtual ElemVariable Variable
	  {
		  set
		  {
    
			if (null == m_topLevelVariables)
			{
			  m_topLevelVariables = new ArrayList();
			}
    
			m_topLevelVariables.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:variable" or "xsl:param" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="qname"> non-null reference to the qualified name of the variable.
	  /// </param>
	  /// <returns> The ElemVariable with the given name in the list or null </returns>
	  public virtual ElemVariable getVariableOrParam(QName qname)
	  {

		if (null != m_topLevelVariables)
		{
		  int n = VariableOrParamCount;

		  for (int i = 0; i < n; i++)
		  {
			ElemVariable var = (ElemVariable) getVariableOrParam(i);

			if (var.Name.Equals(qname))
			{
			  return var;
			}
		  }
		}

		return null;
	  }


	  /// <summary>
	  /// Get an "xsl:variable" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="qname"> Qualified name of the xsl:variable to get 
	  /// </param>
	  /// <returns> reference to the variable named by qname, or null if not found. </returns>
	  public virtual ElemVariable getVariable(QName qname)
	  {

		if (null != m_topLevelVariables)
		{
		  int n = VariableOrParamCount;

		  for (int i = 0; i < n; i++)
		  {
			ElemVariable var = getVariableOrParam(i);
			if ((var.XSLToken == Constants.ELEMNAME_VARIABLE) && (var.Name.Equals(qname)))
			{
			  return var;
			}
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Get an "xsl:variable" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of variable to get in the list
	  /// </param>
	  /// <returns> ElemVariable at the given index in the list 
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemVariable getVariableOrParam(int i) throws ArrayIndexOutOfBoundsException
	  public virtual ElemVariable getVariableOrParam(int i)
	  {

		if (null == m_topLevelVariables)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (ElemVariable) m_topLevelVariables[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:variable" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:variable" properties. </returns>
	  public virtual int VariableOrParamCount
	  {
		  get
		  {
			return (null != m_topLevelVariables) ? m_topLevelVariables.Count : 0;
		  }
	  }

	  /// <summary>
	  /// Set an "xsl:param" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> A non-null ElemParam reference. </param>
	  public virtual ElemParam Param
	  {
		  set
		  {
			Variable = value;
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:param" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="qname"> non-null reference to qualified name of the parameter.
	  /// </param>
	  /// <returns> ElemParam with the given name in the list or null </returns>
	  public virtual ElemParam getParam(QName qname)
	  {

		if (null != m_topLevelVariables)
		{
		  int n = VariableOrParamCount;

		  for (int i = 0; i < n; i++)
		  {
			ElemVariable var = getVariableOrParam(i);
			if ((var.XSLToken == Constants.ELEMNAME_PARAMVARIABLE) && (var.Name.Equals(qname)))
			{
			  return (ElemParam)var;
			}
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// The "xsl:template" properties.
	  /// @serial
	  /// </summary>
	  private ArrayList m_templates;

	  /// <summary>
	  /// Set an "xsl:template" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> ElemTemplate to add to list of templates </param>
	  public virtual ElemTemplate Template
	  {
		  set
		  {
    
			if (null == m_templates)
			{
			  m_templates = new ArrayList();
			}
    
			m_templates.Add(value);
			value.Stylesheet = this;
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:template" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of ElemTemplate in the list to get
	  /// </param>
	  /// <returns> ElemTemplate at the given index in the list
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemTemplate getTemplate(int i) throws javax.xml.transform.TransformerException
	  public virtual ElemTemplate getTemplate(int i)
	  {

		if (null == m_templates)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (ElemTemplate) m_templates[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:template" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:template" properties. </returns>
	  public virtual int TemplateCount
	  {
		  get
		  {
			return (null != m_templates) ? m_templates.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "xsl:namespace-alias" properties.
	  /// @serial
	  /// </summary>
	  private ArrayList m_prefix_aliases;

	  /// <summary>
	  /// Set the "xsl:namespace-alias" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <param name="na"> NamespaceAlias elemeent to add to the list </param>
	  public virtual NamespaceAlias NamespaceAlias
	  {
		  set
		  {
    
			if (m_prefix_aliases == null)
			{
			  m_prefix_aliases = new ArrayList();
			}
    
			m_prefix_aliases.Add(value);
		  }
	  }

	  /// <summary>
	  /// Get an "xsl:namespace-alias" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <param name="i"> Index of NamespaceAlias element to get from the list 
	  /// </param>
	  /// <returns> NamespaceAlias element at the given index in the list
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public NamespaceAlias getNamespaceAlias(int i) throws ArrayIndexOutOfBoundsException
	  public virtual NamespaceAlias getNamespaceAlias(int i)
	  {

		if (null == m_prefix_aliases)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (NamespaceAlias) m_prefix_aliases[i];
	  }

	  /// <summary>
	  /// Get the number of "xsl:namespace-alias" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <returns> the number of "xsl:namespace-alias" properties. </returns>
	  public virtual int NamespaceAliasCount
	  {
		  get
		  {
			return (null != m_prefix_aliases) ? m_prefix_aliases.Count : 0;
		  }
	  }

	  /// <summary>
	  /// The "non-xsl-top-level" properties.
	  /// @serial
	  /// </summary>
	  private Hashtable m_NonXslTopLevel;

	  /// <summary>
	  /// Set found a non-xslt element. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.stylesheet-element">stylesheet-element in XSLT Specification</a>"
	  ////>
	  /// <param name="name"> Qualified name of the element </param>
	  /// <param name="obj"> The element object </param>
	  public virtual void setNonXslTopLevel(QName name, object obj)
	  {

		if (null == m_NonXslTopLevel)
		{
		  m_NonXslTopLevel = new Hashtable();
		}

		m_NonXslTopLevel[name] = obj;
	  }

	  /// <summary>
	  /// Get a non-xslt element. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.stylesheet-element">stylesheet-element in XSLT Specification</a>"
	  ////>
	  /// <param name="name"> Qualified name of the element to get
	  /// </param>
	  /// <returns> The object associate with the given name  </returns>
	  public virtual object getNonXslTopLevel(QName name)
	  {
		return (null != m_NonXslTopLevel) ? m_NonXslTopLevel[name] : null;
	  }

	  // =========== End top-level XSLT properties ===========

	  /// <summary>
	  /// The base URL of the XSL document.
	  /// @serial
	  /// </summary>
	  private string m_href = null;

	  /// <summary>
	  /// The doctype-public element.
	  ///  @serial          
	  /// </summary>
	  private string m_publicId;

	  /// <summary>
	  /// The doctype-system element.
	  ///  @serial          
	  /// </summary>
	  private string m_systemId;

	  /// <summary>
	  /// Get the base identifier with which this stylesheet is associated.
	  /// </summary>
	  /// <returns> the base identifier with which this stylesheet is associated. </returns>
	  public virtual string Href
	  {
		  get
		  {
			return m_href;
		  }
		  set
		  {
			m_href = value;
		  }
	  }


	  /// <summary>
	  /// Set the location information for this element.
	  /// </summary>
	  /// <param name="locator"> SourceLocator object with location information   </param>
	  public override SourceLocator LocaterInfo
	  {
		  set
		  {
    
			if (null != value)
			{
			  m_publicId = value.getPublicId();
			  m_systemId = value.getSystemId();
    
			  if (null != m_systemId)
			  {
				try
				{
				  m_href = SystemIDResolver.getAbsoluteURI(m_systemId, null);
				}
				catch (TransformerException)
				{
    
				  // Ignore this for right now
				}
			  }
    
			  base.LocaterInfo = value;
			}
		  }
	  }

	  /// <summary>
	  /// The root of the stylesheet, where all the tables common
	  /// to all stylesheets are kept.
	  /// @serial
	  /// </summary>
	  private StylesheetRoot m_stylesheetRoot;

	  /// <summary>
	  /// Get the root of the stylesheet, where all the tables common
	  /// to all stylesheets are kept.
	  /// </summary>
	  /// <returns> the root of the stylesheet </returns>
	  public override StylesheetRoot StylesheetRoot
	  {
		  get
		  {
			return m_stylesheetRoot;
		  }
		  set
		  {
			m_stylesheetRoot = value;
		  }
	  }


	  /// <summary>
	  /// The parent of the stylesheet.  This will be null if this
	  /// is the root stylesheet.
	  /// @serial
	  /// </summary>
	  private Stylesheet m_stylesheetParent;

	  /// <summary>
	  /// Get the parent of the stylesheet.  This will be null if this
	  /// is the root stylesheet.
	  /// </summary>
	  /// <returns> the parent of the stylesheet. </returns>
	  public virtual Stylesheet StylesheetParent
	  {
		  get
		  {
			return m_stylesheetParent;
		  }
		  set
		  {
			m_stylesheetParent = value;
		  }
	  }


	  /// <summary>
	  /// Get the owning aggregated stylesheet, or this
	  /// stylesheet if it is aggregated.
	  /// </summary>
	  /// <returns> the owning aggregated stylesheet or itself </returns>
	  public override StylesheetComposed StylesheetComposed
	  {
		  get
		  {
    
			Stylesheet sheet = this;
    
			while (!sheet.AggregatedType)
			{
			  sheet = sheet.StylesheetParent;
			}
    
			return (StylesheetComposed) sheet;
		  }
	  }

	  /// <summary>
	  /// Get the type of the node.  We'll pretend we're a Document.
	  /// </summary>
	  /// <returns> the type of the node: document node. </returns>
	  public override short NodeType
	  {
		  get
		  {
			return DTM.DOCUMENT_NODE;
		  }
	  }

	  /// <summary>
	  /// Get an integer representation of the element type.
	  /// </summary>
	  /// <returns> An integer representation of the element, defined in the
	  ///     Constants class. </returns>
	  /// <seealso cref="org.apache.xalan.templates.Constants"/>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_STYLESHEET;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The node name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_STYLESHEET_STRING;
		  }
	  }

	  /// <summary>
	  /// Replace an "xsl:template" property.
	  /// This is a hook for CompilingStylesheetHandler, to allow
	  /// us to access a template, compile it, instantiate it,
	  /// and replace the original with the compiled instance.
	  /// ADDED 9/5/2000 to support compilation experiment
	  /// </summary>
	  /// <param name="v"> Compiled template to replace with </param>
	  /// <param name="i"> Index of template to be replaced
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void replaceTemplate(ElemTemplate v, int i) throws javax.xml.transform.TransformerException
	  public virtual void replaceTemplate(ElemTemplate v, int i)
	  {

		if (null == m_templates)
		{
		  throw new System.IndexOutOfRangeException();
		}

		replaceChild(v, (ElemTemplateElement)m_templates[i]);
		m_templates[i] = v;
		v.Stylesheet = this;
	  }

		/// <summary>
		/// Call the children visitors. </summary>
		/// <param name="visitor"> The visitor whose appropriate method will be called. </param>
		protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
		{
		  int s = ImportCount;
		  for (int j = 0; j < s; j++)
		  {
			  getImport(j).callVisitors(visitor);
		  }

		  s = IncludeCount;
		  for (int j = 0; j < s; j++)
		  {
			  getInclude(j).callVisitors(visitor);
		  }

		  s = OutputCount;
		  for (int j = 0; j < s; j++)
		  {
			visitor.visitTopLevelInstruction(getOutput(j));
		  }

		  // Next, add in the attribute-set elements

		  s = AttributeSetCount;
		  for (int j = 0; j < s; j++)
		  {
			  ElemAttributeSet attrSet = getAttributeSet(j);
			if (visitor.visitTopLevelInstruction(attrSet))
			{
			  attrSet.callChildVisitors(visitor);
			}
		  }
		  // Now the decimal-formats

		  s = DecimalFormatCount;
		  for (int j = 0; j < s; j++)
		  {
			visitor.visitTopLevelInstruction(getDecimalFormat(j));
		  }

		  // Now the keys

		  s = KeyCount;
		  for (int j = 0; j < s; j++)
		  {
			visitor.visitTopLevelInstruction(getKey(j));
		  }

		  // And the namespace aliases

		  s = NamespaceAliasCount;
		  for (int j = 0; j < s; j++)
		  {
			visitor.visitTopLevelInstruction(getNamespaceAlias(j));
		  }

		  // Next comes the templates

		  s = TemplateCount;
		  for (int j = 0; j < s; j++)
		  {
			try
			{
			  ElemTemplate template = getTemplate(j);
			  if (visitor.visitTopLevelInstruction(template))
			  {
				template.callChildVisitors(visitor);
			  }
			}
			catch (TransformerException te)
			{
			  throw new org.apache.xml.utils.WrappedRuntimeException(te);
			}
		  }

		  // Then, the variables

		  s = VariableOrParamCount;
		  for (int j = 0; j < s; j++)
		  {
			  ElemVariable var = getVariableOrParam(j);
			if (visitor.visitTopLevelVariableOrParamDecl(var))
			{
			  var.callChildVisitors(visitor);
			}
		  }

		  // And lastly the whitespace preserving and stripping elements

		  s = StripSpaceCount;
		  for (int j = 0; j < s; j++)
		  {
			visitor.visitTopLevelInstruction(getStripSpace(j));
		  }

		  s = PreserveSpaceCount;
		  for (int j = 0; j < s; j++)
		  {
			visitor.visitTopLevelInstruction(getPreserveSpace(j));
		  }

		  if (null != m_NonXslTopLevel)
		  {
			  System.Collections.IEnumerator elements = m_NonXslTopLevel.Values.GetEnumerator();
			  while (elements.MoveNext())
			  {
				ElemTemplateElement elem = (ElemTemplateElement)elements.Current;
			  if (visitor.visitTopLevelInstruction(elem))
			  {
				elem.callChildVisitors(visitor);
			  }

			  }
		  }
		}


	  /// <summary>
	  /// Accept a visitor and call the appropriate method 
	  /// for this class.
	  /// </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  /// <returns> true if the children of the object should be visited. </returns>
	  protected internal override bool accept(XSLTVisitor visitor)
	  {
		  return visitor.visitStylesheet(this);
	  }


	}

}