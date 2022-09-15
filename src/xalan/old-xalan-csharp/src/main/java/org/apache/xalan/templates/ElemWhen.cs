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
 * $Id: ElemWhen.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XPath = org.apache.xpath.XPath;

	/// <summary>
	/// Implement xsl:when.
	/// <pre>
	/// <!ELEMENT xsl:when %template;>
	/// <!ATTLIST xsl:when
	///   test %expr; #REQUIRED
	///   %space-att;
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Conditional-Processing-with-xsl:choose">XXX in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemWhen : ElemTemplateElement
	{
		internal new const long serialVersionUID = 5984065730262071360L;

	  /// <summary>
	  /// Each xsl:when element has a single attribute, test,
	  /// which specifies an expression.
	  /// @serial
	  /// </summary>
	  private XPath m_test;

	  /// <summary>
	  /// Set the "test" attribute.
	  /// Each xsl:when element has a single attribute, test,
	  /// which specifies an expression.
	  /// </summary>
	  /// <param name="v"> Value to set for the "test" attribute. </param>
	  public virtual XPath Test
	  {
		  set
		  {
			m_test = value;
		  }
		  get
		  {
			return m_test;
		  }
	  }


	  /// <summary>
	  /// Get an integer representation of the element type.
	  /// </summary>
	  /// <returns> An integer representation of the element, defined in the
	  ///     Constants class. </returns>
	  /// <seealso cref= org.apache.xalan.templates.Constants </seealso>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_WHEN;
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
		ArrayList vnames = sroot.getComposeState().VariableNames;
		if (null != m_test)
		{
		  m_test.fixupVariables(vnames, sroot.getComposeState().GlobalsSize);
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
			return Constants.ELEMNAME_WHEN_STRING;
		  }
	  }

	  /// <summary>
	  /// Constructor ElemWhen
	  /// 
	  /// </summary>
	  public ElemWhen()
	  {
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
	  {
		  if (callAttrs)
		  {
			  m_test.Expression.callVisitors(m_test, visitor);
		  }
		base.callChildVisitors(visitor, callAttrs);
	  }

	}

}