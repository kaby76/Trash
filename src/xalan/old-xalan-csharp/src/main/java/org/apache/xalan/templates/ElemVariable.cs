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
 * $Id: ElemVariable.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using XRTreeFrag = org.apache.xpath.objects.XRTreeFrag;
	using XRTreeFragSelectWrapper = org.apache.xpath.objects.XRTreeFragSelectWrapper;
	using XString = org.apache.xpath.objects.XString;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	/// <summary>
	/// Implement xsl:variable.
	/// <pre>
	/// <!ELEMENT xsl:variable %template;>
	/// <!ATTLIST xsl:variable
	///   name %qname; #REQUIRED
	///   select %expr; #IMPLIED
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#variables">variables in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemVariable : ElemTemplateElement
	{
		internal new const long serialVersionUID = 9111131075322790061L;

	  /// <summary>
	  /// Constructor ElemVariable
	  /// 
	  /// </summary>
	  public ElemVariable()
	  {
	  }

	  /// <summary>
	  /// This is the index into the stack frame.
	  /// </summary>
	  protected internal int m_index;

	  /// <summary>
	  /// The stack frame size for this variable if it is a global variable 
	  /// that declares an RTF, which is equal to the maximum number 
	  /// of variables that can be declared in the variable at one time.
	  /// </summary>
	  internal int m_frameSize = -1;


	  /// <summary>
	  /// Sets the relative position of this variable within the stack frame (if local)
	  /// or the global area (if global).  Note that this should be called only for
	  /// global variables since the local position is computed in the compose() method.
	  /// </summary>
	  public virtual int Index
	  {
		  set
		  {
			m_index = value;
		  }
		  get
		  {
			return m_index;
		  }
	  }


	  /// <summary>
	  /// The value of the "select" attribute.
	  /// @serial
	  /// </summary>
	  private XPath m_selectPattern;

	  /// <summary>
	  /// Set the "select" attribute.
	  /// If the variable-binding element has a select attribute,
	  /// then the value of the attribute must be an expression and
	  /// the value of the variable is the object that results from
	  /// evaluating the expression. In this case, the content
	  /// of the variable must be empty.
	  /// </summary>
	  /// <param name="v"> Value to set for the "select" attribute. </param>
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
	  /// The value of the "name" attribute.
	  /// @serial
	  /// </summary>
	  protected internal QName m_qname;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// Both xsl:variable and xsl:param have a required name
	  /// attribute, which specifies the name of the variable. The
	  /// value of the name attribute is a QName, which is expanded
	  /// as described in [2.4 Qualified Names]. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#qname">qname in XSLT Specification</a>
	  /// </seealso>
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
	  /// Tells if this is a top-level variable or param, or not.
	  /// @serial
	  /// </summary>
	  private bool m_isTopLevel = false;

	  /// <summary>
	  /// Set if this is a top-level variable or param, or not. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#top-level-variables">top-level-variables in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Boolean indicating whether this is a top-level variable
	  /// or param, or not. </param>
	  public virtual bool IsTopLevel
	  {
		  set
		  {
			m_isTopLevel = value;
		  }
		  get
		  {
			return m_isTopLevel;
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
			return Constants.ELEMNAME_VARIABLE;
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
			return Constants.ELEMNAME_VARIABLE_STRING;
		  }
	  }

	  /// <summary>
	  /// Copy constructor.
	  /// </summary>
	  /// <param name="param"> An element created from an xsl:variable
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ElemVariable(ElemVariable param) throws javax.xml.transform.TransformerException
	  public ElemVariable(ElemVariable param)
	  {

		m_selectPattern = param.m_selectPattern;
		m_qname = param.m_qname;
		m_isTopLevel = param.m_isTopLevel;

		// m_value = param.m_value;
		// m_varContext = param.m_varContext;
	  }

	  /// <summary>
	  /// Execute a variable declaration and push it onto the variable stack. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#variables">variables in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		int sourceNode = transformer.XPathContext.CurrentNode;

		XObject @var = getValue(transformer, sourceNode);

		// transformer.getXPathContext().getVarStack().pushVariable(m_qname, var);
		transformer.XPathContext.VarStack.setLocalVariable(m_index, @var);

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getValue(org.apache.xalan.transformer.TransformerImpl transformer, int sourceNode) throws javax.xml.transform.TransformerException
	  public virtual XObject getValue(TransformerImpl transformer, int sourceNode)
	  {

		XObject @var;
		XPathContext xctxt = transformer.XPathContext;

		xctxt.pushCurrentNode(sourceNode);

		try
		{
		  if (null != m_selectPattern)
		  {
			@var = m_selectPattern.execute(xctxt, sourceNode, this);

			@var.allowDetachToRelease(false);

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireSelectedEvent(sourceNode, this, "select", m_selectPattern, @var);
			}
		  }
		  else if (null == FirstChildElem)
		  {
			@var = XString.EMPTYSTRING;
		  }
		  else
		  {

			// Use result tree fragment.
			// Global variables may be deferred (see XUnresolvedVariable) and hence
			// need to be assigned to a different set of DTMs than local variables
			// so they aren't popped off the stack on return from a template.
			int df;

			// Bugzilla 7118: A variable set via an RTF may create local
			// variables during that computation. To keep them from overwriting
			// variables at this level, push a new variable stack.
			////// PROBLEM: This is provoking a variable-used-before-set
			////// problem in parameters. Needs more study.
			try
			{
				//////////xctxt.getVarStack().link(0);
				if (m_parentNode is Stylesheet) // Global variable
				{
					df = transformer.transformToGlobalRTF(this);
				}
				else
				{
					df = transformer.transformToRTF(this);
				}
			}
			finally
			{
				//////////////xctxt.getVarStack().unlink(); 
			}

			@var = new XRTreeFrag(df, xctxt, this);
		  }
		}
		finally
		{
		  xctxt.popCurrentNode();
		}

		return @var;
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
		// See if we can reduce an RTF to a select with a string expression.
		if (null == m_selectPattern && sroot.Optimizer)
		{
		  XPath newSelect = rewriteChildToExpression(this);
		  if (null != newSelect)
		  {
			m_selectPattern = newSelect;
		  }
		}

		StylesheetRoot.ComposeState cstate = sroot.getComposeState();

		// This should be done before addVariableName, so we don't have visibility 
		// to the variable now being defined.
		ArrayList vnames = cstate.VariableNames;
		if (null != m_selectPattern)
		{
		  m_selectPattern.fixupVariables(vnames, cstate.GlobalsSize);
		}

		// Only add the variable if this is not a global.  If it is a global, 
		// it was already added by stylesheet root.
		if (!(m_parentNode is Stylesheet) && m_qname != null)
		{
		  m_index = cstate.addVariableName(m_qname) - cstate.GlobalsSize;
		}
		else if (m_parentNode is Stylesheet)
		{
			// If this is a global, then we need to treat it as if it's a xsl:template, 
			// and count the number of variables it contains.  So we set the count to 
			// zero here.
			cstate.resetStackFrameSize();
		}

		// This has to be done after the addVariableName, so that the variable 
		// pushed won't be immediately popped again in endCompose.
		base.compose(sroot);
	  }

	  /// <summary>
	  /// This after the template's children have been composed.  We have to get 
	  /// the count of how many variables have been declared, so we can do a link 
	  /// and unlink.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCompose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void endCompose(StylesheetRoot sroot)
	  {
		base.endCompose(sroot);
		if (m_parentNode is Stylesheet)
		{
			StylesheetRoot.ComposeState cstate = sroot.getComposeState();
			m_frameSize = cstate.FrameSize;
			cstate.resetStackFrameSize();
		}
	  }



	//  /**
	//   * This after the template's children have been composed.
	//   */
	//  public void endCompose() throws TransformerException
	//  {
	//    super.endCompose();
	//  }


	  /// <summary>
	  /// If the children of a variable is a single xsl:value-of or text literal, 
	  /// it is cheaper to evaluate this as an expression, so try and adapt the 
	  /// child an an expression.
	  /// </summary>
	  /// <param name="varElem"> Should be a ElemParam, ElemVariable, or ElemWithParam.
	  /// </param>
	  /// <returns> An XPath if rewrite is possible, else null.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static org.apache.xpath.XPath rewriteChildToExpression(ElemTemplateElement varElem) throws javax.xml.transform.TransformerException
	  internal static XPath rewriteChildToExpression(ElemTemplateElement varElem)
	  {

		ElemTemplateElement t = varElem.FirstChildElem;

		// Down the line this can be done with multiple string objects using 
		// the concat function.
		if (null != t && null == t.NextSiblingElem)
		{
		  int etype = t.XSLToken;

		  if (Constants.ELEMNAME_VALUEOF == etype)
		  {
			ElemValueOf valueof = (ElemValueOf) t;

			// %TBD% I'm worried about extended attributes here.
			if (valueof.DisableOutputEscaping == false && valueof.DOMBackPointer == null)
			{
			  varElem.m_firstChild = null;

			  return new XPath(new XRTreeFragSelectWrapper(valueof.Select.Expression));
			}
		  }
		  else if (Constants.ELEMNAME_TEXTLITERALRESULT == etype)
		  {
			ElemTextLiteral lit = (ElemTextLiteral) t;

			if (lit.DisableOutputEscaping == false && lit.DOMBackPointer == null)
			{
			  string str = lit.NodeValue;
			  XString xstr = new XString(str);

			  varElem.m_firstChild = null;

			  return new XPath(new XRTreeFragSelectWrapper(xstr));
			}
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// This function is called during recomposition to
	  /// control how this element is composed. </summary>
	  /// <param name="root"> The root stylesheet for this transformation. </param>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeVariables(this);
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
	  /// Accept a visitor and call the appropriate method 
	  /// for this class.
	  /// </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  /// <returns> true if the children of the object should be visited. </returns>
	  protected internal override bool accept(XSLTVisitor visitor)
	  {
		  return visitor.visitVariableOrParamDecl(this);
	  }


	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
	  {
		  if (null != m_selectPattern)
		  {
			  m_selectPattern.Expression.callVisitors(m_selectPattern, visitor);
		  }
		base.callChildVisitors(visitor, callAttrs);
	  }

	  /// <summary>
	  /// Tell if this is a psuedo variable reference, declared by Xalan instead 
	  /// of by the user.
	  /// </summary>
	  public virtual bool PsuedoVar
	  {
		  get
		  {
			  string ns = m_qname.NamespaceURI;
			  if ((null != ns) && ns.Equals(RedundentExprEliminator.PSUEDOVARNAMESPACE))
			  {
				  if (m_qname.LocalName.StartsWith("#", StringComparison.Ordinal))
				  {
					  return true;
				  }
			  }
			  return false;
		  }
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