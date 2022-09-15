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
 * $Id: ElemWithParam.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using XRTreeFrag = org.apache.xpath.objects.XRTreeFrag;
	using XString = org.apache.xpath.objects.XString;

	/// <summary>
	/// Implement xsl:with-param.  xsl:with-param is allowed within
	/// both xsl:call-template and xsl:apply-templates.
	/// <pre>
	/// <!ELEMENT xsl:with-param %template;>
	/// <!ATTLIST xsl:with-param
	///   name %qname; #REQUIRED
	///   select %expr; #IMPLIED
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.element-with-param">element-with-param in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemWithParam : ElemTemplateElement
	{
		internal new const long serialVersionUID = -1070355175864326257L;
	  /// <summary>
	  /// This is the index to the stack frame being called, <emph>not</emph> the 
	  /// stack frame that contains this element.
	  /// </summary>
	  internal int m_index;

	  /// <summary>
	  /// The "select" attribute, which specifies the value of the
	  /// argument, if element content is not specified.
	  /// @serial
	  /// </summary>
	  private XPath m_selectPattern = null;

	  /// <summary>
	  /// Set the "select" attribute.
	  /// The "select" attribute specifies the value of the
	  /// argument, if element content is not specified.
	  /// </summary>
	  /// <param name="v"> Value to set for the "select" attribute.  </param>
	  public virtual XPath Select
	  {
		  set
		  {
			m_selectPattern = value;
		  }
		  get
		  {
			return m_selectPattern;
		  }
	  }


	  /// <summary>
	  /// The required name attribute specifies the name of the
	  /// parameter (the variable the value of whose binding is
	  /// to be replaced). The value of the name attribute is a QName,
	  /// which is expanded as described in [2.4 Qualified Names].
	  /// @serial
	  /// </summary>
	  private QName m_qname = null;

	  internal int m_qnameID;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// DJD
	  /// </summary>
	  /// <param name="v"> Value to set for the "name" attribute. </param>
	  public virtual QName Name
	  {
		  set
		  {
			m_qname = value;
		  }
		  get
		  {
			return m_qname;
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
			return Constants.ELEMNAME_WITHPARAM;
		  }
	  }


	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> the node name. </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_WITHPARAM_STRING;
		  }
	  }

	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		// See if we can reduce an RTF to a select with a string expression.
		if (null == m_selectPattern && sroot.Optimizer)
		{
		  XPath newSelect = ElemVariable.rewriteChildToExpression(this);
		  if (null != newSelect)
		  {
			m_selectPattern = newSelect;
		  }
		}
		m_qnameID = sroot.ComposeState.getQNameID(m_qname);
		base.compose(sroot);

		ArrayList vnames = sroot.ComposeState.VariableNames;
		if (null != m_selectPattern)
		{
		  m_selectPattern.fixupVariables(vnames, sroot.ComposeState.GlobalsSize);
		}

		// m_index must be resolved by ElemApplyTemplates and ElemCallTemplate!
	  }

	  /// <summary>
	  /// Set the parent as an ElemTemplateElement.
	  /// </summary>
	  /// <param name="p"> This node's parent as an ElemTemplateElement </param>
	  public override ElemTemplateElement ParentElem
	  {
		  set
		  {
			base.ParentElem = value;
			value.m_hasVariableDecl = true;
		  }
	  }

	  /// <summary>
	  /// Get the XObject representation of the variable.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="sourceNode"> non-null reference to the <a href="http://www.w3.org/TR/xslt#dt-current-node">current source node</a>.
	  /// </param>
	  /// <returns> the XObject representation of the variable.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getValue(org.apache.xalan.transformer.TransformerImpl transformer, int sourceNode) throws javax.xml.transform.TransformerException
	  public virtual XObject getValue(TransformerImpl transformer, int sourceNode)
	  {

		XObject var;
		XPathContext xctxt = transformer.XPathContext;

		xctxt.pushCurrentNode(sourceNode);

		try
		{
		  if (null != m_selectPattern)
		  {
			var = m_selectPattern.execute(xctxt, sourceNode, this);

			var.allowDetachToRelease(false);

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireSelectedEvent(sourceNode, this, "select", m_selectPattern, var);
			}
		  }
		  else if (null == FirstChildElem)
		  {
			var = XString.EMPTYSTRING;
		  }
		  else
		  {

			// Use result tree fragment
			int df = transformer.transformToRTF(this);

			var = new XRTreeFrag(df, xctxt, this);
		  }
		}
		finally
		{
		  xctxt.popCurrentNode();
		}

		return var;
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
	  {
		  if (callAttrs && (null != m_selectPattern))
		  {
			  m_selectPattern.Expression.callVisitors(m_selectPattern, visitor);
		  }
		base.callChildVisitors(visitor, callAttrs);
	  }

	  /// <summary>
	  /// Add a child to the child list. If the select attribute
	  /// is present, an error will be raised.
	  /// </summary>
	  /// <param name="elem"> New element to append to this element's children list
	  /// </param>
	  /// <returns> null if the select attribute was present, otherwise the 
	  /// child just added to the child list  </returns>
	  public override ElemTemplateElement appendChild(ElemTemplateElement elem)
	  {
		// cannot have content and select
		if (m_selectPattern != null)
		{
		  error(XSLTErrorResources.ER_CANT_HAVE_CONTENT_AND_SELECT, new object[]{"xsl:" + this.NodeName});
		  return null;
		}
		return base.appendChild(elem);
	  }


	}

}