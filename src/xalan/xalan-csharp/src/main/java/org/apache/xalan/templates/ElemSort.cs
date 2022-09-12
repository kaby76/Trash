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
 * $Id: ElemSort.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using XPath = org.apache.xpath.XPath;

	using DOMException = org.w3c.dom.DOMException;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Implement xsl:sort.
	/// <pre>
	/// <!ELEMENT xsl:sort EMPTY>
	/// <!ATTLIST xsl:sort
	///   select %expr; "."
	///   lang %avt; #IMPLIED
	///   data-type %avt; "text"
	///   order %avt; "ascending"
	///   case-order %avt; #IMPLIED
	/// >
	/// <!-- xsl:sort cannot occur after any other elements or
	/// any non-whitespace character -->
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#sorting">sorting in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemSort : ElemTemplateElement
	{
		internal new const long serialVersionUID = -4991510257335851938L;

	  /// <summary>
	  /// xsl:sort has a select attribute whose value is an expression.
	  /// @serial
	  /// </summary>
	  private XPath m_selectExpression = null;

	  /// <summary>
	  /// Set the "select" attribute.
	  /// xsl:sort has a select attribute whose value is an expression.
	  /// For each node to be processed, the expression is evaluated
	  /// with that node as the current node and with the complete
	  /// list of nodes being processed in unsorted order as the current
	  /// node list. The resulting object is converted to a string as if
	  /// by a call to the string function; this string is used as the
	  /// sort key for that node. The default value of the select attribute
	  /// is ., which will cause the string-value of the current node to
	  /// be used as the sort key.
	  /// </summary>
	  /// <param name="v"> Value to set for the "select" attribute </param>
	  public virtual XPath Select
	  {
		  set
		  {
    
			if (value.PatternString.IndexOf("{", StringComparison.Ordinal) < 0)
			{
			  m_selectExpression = value;
			}
			else
			{
			  error(XSLTErrorResources.ER_NO_CURLYBRACE, null);
			}
		  }
		  get
		  {
			return m_selectExpression;
		  }
	  }


	  /// <summary>
	  /// lang specifies the language of the sort keys.
	  /// @serial
	  /// </summary>
	  private AVT m_lang_avt = null;

	  /// <summary>
	  /// Set the "lang" attribute.
	  /// lang specifies the language of the sort keys; it has the same
	  /// range of values as xml:lang [XML]; if no lang value is
	  /// specified, the language should be determined from the system environment.
	  /// </summary>
	  /// <param name="v"> The value to set for the "lang" attribute </param>
	  public virtual AVT Lang
	  {
		  set
		  {
			m_lang_avt = value;
		  }
		  get
		  {
			return m_lang_avt;
		  }
	  }


	  /// <summary>
	  /// data-type specifies the data type of the
	  /// strings to be sorted.
	  /// @serial
	  /// </summary>
	  private AVT m_dataType_avt = null;

	  /// <summary>
	  /// Set the "data-type" attribute.
	  /// <code>data-type</code> specifies the data type of the
	  /// strings; the following values are allowed:
	  /// <ul>
	  /// <li>
	  /// <code>text</code> specifies that the sort keys should be
	  /// sorted lexicographically in the culturally correct manner for the
	  /// language specified by <code>lang</code>.
	  /// </li>
	  /// <li>
	  /// <code>number</code> specifies that the sort keys should be
	  /// converted to numbers and then sorted according to the numeric value;
	  /// the sort key is converted to a number as if by a call to the
	  /// <b><a href="http://www.w3.org/TR/xpath#function-number">number</a></b> function; the <code>lang</code>
	  /// attribute is ignored.
	  /// </li>
	  /// <li>
	  /// A <a href="http://www.w3.org/TR/REC-xml-names#NT-QName">QName</a> with a prefix
	  /// is expanded into an <a href="http://www.w3.org/TR/xpath#dt-expanded-name">expanded-name</a> as described
	  /// in <a href="#qname">[<b>2.4 Qualified Names</b>]</a>; the expanded-name identifies the data-type;
	  /// the behavior in this case is not specified by this document.
	  /// </li>
	  /// </ul>
	  /// <para>The default value is <code>text</code>.</para>
	  /// <blockquote>
	  /// <b>NOTE: </b>The XSL Working Group plans that future versions of XSLT will
	  /// leverage XML Schemas to define further values for this
	  /// attribute.</blockquote>
	  /// </summary>
	  /// <param name="v"> Value to set for the "data-type" attribute </param>
	  public virtual AVT DataType
	  {
		  set
		  {
			m_dataType_avt = value;
		  }
		  get
		  {
			return m_dataType_avt;
		  }
	  }


	  /// <summary>
	  /// order specifies whether the strings should be sorted in ascending
	  /// or descending order.
	  /// @serial
	  /// </summary>
	  private AVT m_order_avt = null;

	  /// <summary>
	  /// Set the "order" attribute.
	  /// order specifies whether the strings should be sorted in ascending
	  /// or descending order; ascending specifies ascending order; descending
	  /// specifies descending order; the default is ascending.
	  /// </summary>
	  /// <param name="v"> The value to set for the "order" attribute </param>
	  public virtual AVT Order
	  {
		  set
		  {
			m_order_avt = value;
		  }
		  get
		  {
			return m_order_avt;
		  }
	  }


	  /// <summary>
	  /// case-order has the value upper-first or lower-first.
	  /// The default value is language dependent.
	  /// @serial
	  /// </summary>
	  private AVT m_caseorder_avt = null;

	  /// <summary>
	  /// Set the "case-order" attribute.
	  /// case-order has the value upper-first or lower-first; this applies
	  /// when data-type="text", and specifies that upper-case letters should
	  /// sort before lower-case letters or vice-versa respectively.
	  /// For example, if lang="en", then A a B b are sorted with
	  /// case-order="upper-first" and a A b B are sorted with case-order="lower-first".
	  /// The default value is language dependent.
	  /// </summary>
	  /// <param name="v"> The value to set for the "case-order" attribute
	  /// 
	  /// @serial </param>
	  public virtual AVT CaseOrder
	  {
		  set
		  {
			m_caseorder_avt = value;
		  }
		  get
		  {
			return m_caseorder_avt;
		  }
	  }


	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID of the element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_SORT;
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
			return Constants.ELEMNAME_SORT_STRING;
		  }
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// </summary>
	  /// <param name="newChild"> Child to add to the child list
	  /// </param>
	  /// <returns> Child just added to the child list
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node appendChild(org.w3c.dom.Node newChild) throws org.w3c.dom.DOMException
	  public override Node appendChild(Node newChild)
	  {

		error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
		return null;
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
		if (null != m_caseorder_avt)
		{
		  m_caseorder_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_dataType_avt)
		{
		  m_dataType_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_lang_avt)
		{
		  m_lang_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_order_avt)
		{
		  m_order_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_selectExpression)
		{
		  m_selectExpression.fixupVariables(vnames, cstate.GlobalsSize);
		}
	  }
	}

}