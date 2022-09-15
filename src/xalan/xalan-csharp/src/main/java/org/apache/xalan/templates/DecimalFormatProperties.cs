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
 * $Id: DecimalFormatProperties.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using QName = org.apache.xml.utils.QName;

	/// <summary>
	/// Implement xsl:decimal-format.
	/// <pre>
	/// <!ELEMENT xsl:decimal-format EMPTY>
	/// <!ATTLIST xsl:decimal-format
	///   name %qname; #IMPLIED
	///   decimal-separator %char; "."
	///   grouping-separator %char; ","
	///   infinity CDATA "Infinity"
	///   minus-sign %char; "-"
	///   NaN CDATA "NaN"
	///   percent %char; "%"
	///   per-mille %char; "&#x2030;"
	///   zero-digit %char; "0"
	///   digit %char; "#"
	///   pattern-separator %char; ";"
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.format-number">format-number in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class DecimalFormatProperties : ElemTemplateElement
	{
		internal new const long serialVersionUID = -6559409339256269446L;

	  /// <summary>
	  /// An instance of DecimalFormatSymbols for this element.
	  ///  @serial       
	  /// </summary>
	  internal DecimalFormatSymbols m_dfs;

	  /// <summary>
	  /// Constructor DecimalFormatProperties
	  /// 
	  /// </summary>
	  public DecimalFormatProperties(int docOrderNumber)
	  {

		m_dfs = new DecimalFormatSymbols();

		// Set default values, they can be overiden if necessary.  
		m_dfs.setInfinity(Constants.ATTRVAL_INFINITY);
		m_dfs.setNaN(Constants.ATTRVAL_NAN);

		m_docOrderNumber = docOrderNumber;
	  }

	  /// <summary>
	  /// Return the decimal format Symbols for this element.
	  /// <para>The xsl:decimal-format element declares a decimal-format,
	  /// which controls the interpretation of a format pattern used by
	  /// the format-number function. If there is a name attribute, then
	  /// the element declares a named decimal-format; otherwise, it
	  /// declares the default decimal-format. The value of the name
	  /// attribute is a QName, which is expanded as described in [2.4 Qualified Names].
	  /// It is an error to declare either the default decimal-format or a
	  /// decimal-format with a given name more than once (even with different
	  /// import precedence), unless it is declared every time with the same
	  /// value for all attributes (taking into account any default values).</para>
	  /// <para>The other attributes on xsl:decimal-format correspond to the
	  /// methods on the JDK 1.1 DecimalFormatSymbols class. For each get/set
	  /// method pair there is an attribute defined for the xsl:decimal-format
	  /// element.</para>
	  /// </summary>
	  /// <returns> the decimal format Symbols for this element. </returns>
	  public virtual DecimalFormatSymbols DecimalFormatSymbols
	  {
		  get
		  {
			return m_dfs;
		  }
	  }

	  /// <summary>
	  /// If there is a name attribute, then the element declares a named
	  /// decimal-format; otherwise, it declares the default decimal-format.
	  /// @serial
	  /// </summary>
	  private QName m_qname = null;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// If there is a name attribute, then the element declares a named
	  /// decimal-format; otherwise, it declares the default decimal-format.
	  /// </summary>
	  /// <param name="qname"> The name to set as the "name" attribute. </param>
	  public virtual QName Name
	  {
		  set
		  {
			m_qname = value;
		  }
		  get
		  {
    
			if (m_qname == null)
			{
			  return new QName("");
			}
			else
			{
			  return m_qname;
			}
		  }
	  }


	  /// <summary>
	  /// Set the "decimal-separator" attribute.
	  /// decimal-separator specifies the character used for the decimal sign;
	  /// the default value is the period character (.).
	  /// </summary>
	  /// <param name="ds"> Character to set as decimal separator  </param>
	  public virtual char DecimalSeparator
	  {
		  set
		  {
			m_dfs.setDecimalSeparator(value);
		  }
		  get
		  {
			return m_dfs.getDecimalSeparator();
		  }
	  }


	  /// <summary>
	  /// Set the "grouping-separator" attribute.
	  /// grouping-separator specifies the character used as a grouping
	  /// (e.g. thousands) separator; the default value is the comma character (,).
	  /// </summary>
	  /// <param name="gs"> Character to use a grouping separator  </param>
	  public virtual char GroupingSeparator
	  {
		  set
		  {
			m_dfs.setGroupingSeparator(value);
		  }
		  get
		  {
			return m_dfs.getGroupingSeparator();
		  }
	  }


	  /// <summary>
	  /// Set the "infinity" attribute.
	  /// infinity specifies the string used to represent infinity;
	  /// the default value is the string Infinity.
	  /// </summary>
	  /// <param name="inf"> String to use as the "infinity" attribute. </param>
	  public virtual string Infinity
	  {
		  set
		  {
			m_dfs.setInfinity(value);
		  }
		  get
		  {
			return m_dfs.getInfinity();
		  }
	  }


	  /// <summary>
	  /// Set the "minus-sign" attribute.
	  /// minus-sign specifies the character used as the default minus sign; the
	  /// default value is the hyphen-minus character (-, #x2D).
	  /// </summary>
	  /// <param name="v"> Character to use as minus sign </param>
	  public virtual char MinusSign
	  {
		  set
		  {
			m_dfs.setMinusSign(value);
		  }
		  get
		  {
			return m_dfs.getMinusSign();
		  }
	  }


	  /// <summary>
	  /// Set the "NaN" attribute.
	  /// NaN specifies the string used to represent the NaN value;
	  /// the default value is the string NaN.
	  /// </summary>
	  /// <param name="v"> String to use as the "NaN" attribute. </param>
	  public virtual string NaN
	  {
		  set
		  {
			m_dfs.setNaN(value);
		  }
		  get
		  {
			return m_dfs.getNaN();
		  }
	  }


	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> the element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_DECIMALFORMAT_STRING;
		  }
	  }

	  /// <summary>
	  /// Set the "percent" attribute.
	  /// percent specifies the character used as a percent sign; the default
	  /// value is the percent character (%).
	  /// </summary>
	  /// <param name="v"> Character to use as percent  </param>
	  public virtual char Percent
	  {
		  set
		  {
			m_dfs.setPercent(value);
		  }
		  get
		  {
			return m_dfs.getPercent();
		  }
	  }


	  /// <summary>
	  /// Set the "per-mille" attribute.
	  /// per-mille specifies the character used as a per mille sign; the default
	  /// value is the Unicode per-mille character (#x2030).
	  /// </summary>
	  /// <param name="v"> Character to use as per-mille </param>
	  public virtual char PerMille
	  {
		  set
		  {
			m_dfs.setPerMill(value);
		  }
		  get
		  {
			return m_dfs.getPerMill();
		  }
	  }


	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_DECIMALFORMAT;
		  }
	  }

	  /// <summary>
	  /// Set the "zero-digit" attribute.
	  /// zero-digit specifies the character used as the digit zero; the default
	  /// value is the digit zero (0).
	  /// </summary>
	  /// <param name="v"> Character to use as the digit zero </param>
	  public virtual char ZeroDigit
	  {
		  set
		  {
			m_dfs.setZeroDigit(value);
		  }
		  get
		  {
			return m_dfs.getZeroDigit();
		  }
	  }


	  /// <summary>
	  /// Set the "digit" attribute.
	  /// digit specifies the character used for a digit in the format pattern;
	  /// the default value is the number sign character (#).
	  /// </summary>
	  /// <param name="v"> Character to use for a digit in format pattern </param>
	  public virtual char Digit
	  {
		  set
		  {
			m_dfs.setDigit(value);
		  }
		  get
		  {
			return m_dfs.getDigit();
		  }
	  }


	  /// <summary>
	  /// Set the "pattern-separator" attribute.
	  /// pattern-separator specifies the character used to separate positive
	  /// and negative sub patterns in a pattern; the default value is the
	  /// semi-colon character (;).
	  /// </summary>
	  /// <param name="v"> Character to use as a pattern separator </param>
	  public virtual char PatternSeparator
	  {
		  set
		  {
			m_dfs.setPatternSeparator(value);
		  }
		  get
		  {
			return m_dfs.getPatternSeparator();
		  }
	  }


	  /// <summary>
	  /// This function is called to recompose() all of the decimal format properties elements.
	  /// </summary>
	  /// <param name="root"> Stylesheet root </param>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeDecimalFormats(this);
	  }

	}

}