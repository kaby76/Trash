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
 * $Id: ElemIf.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Implement xsl:if.
	/// <pre>
	/// <!ELEMENT xsl:if %template;>
	/// <!ATTLIST xsl:if
	///   test %expr; #REQUIRED
	///   %space-att;
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Conditional-Processing-with-xsl:if">XXX in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemIf : ElemTemplateElement
	{
		internal new const long serialVersionUID = 2158774632427453022L;

	  /// <summary>
	  /// The xsl:if element must have a test attribute, which specifies an expression.
	  /// @serial
	  /// </summary>
	  private XPath m_test = null;

	  /// <summary>
	  /// Set the "test" attribute.
	  /// The xsl:if element must have a test attribute, which specifies an expression.
	  /// </summary>
	  /// <param name="v"> test attribute to set </param>
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
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
	  /// <param name="sroot"> The root stylesheet.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
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
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_IF;
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
			return Constants.ELEMNAME_IF_STRING;
		  }
	  }

	  /// <summary>
	  /// Conditionally execute a sub-template.
	  /// The expression is evaluated and the resulting object is converted
	  /// to a boolean as if by a call to the boolean function. If the result
	  /// is true, then the content template is instantiated; otherwise, nothing
	  /// is created.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		XPathContext xctxt = transformer.XPathContext;
		int sourceNode = xctxt.CurrentNode;

		if (transformer.Debug)
		{
		  XObject test = m_test.execute(xctxt, sourceNode, this);

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireSelectedEvent(sourceNode, this, "test", m_test, test);
		  }

		  // xsl:for-each now fires one trace event + one for every
		  // iteration; changing xsl:if to fire one regardless of true/false

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEvent(this);
		  }

		  if (test.@bool())
		  {
			transformer.executeChildTemplates(this, true);
		  }

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }

		  // I don't think we want this.  -sb
		  //  if (transformer.getDebug())
		  //    transformer.getTraceManager().fireSelectedEvent(sourceNode, this,
		  //            "endTest", m_test, test);
		}
		else if (m_test.@bool(xctxt, sourceNode, this))
		{
		  transformer.executeChildTemplates(this, true);
		}

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