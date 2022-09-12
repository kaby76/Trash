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
 * $Id: ElemApplyTemplates.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{


	using StackGuard = org.apache.xalan.transformer.StackGuard;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using IntStack = org.apache.xml.utils.IntStack;
	using QName = org.apache.xml.utils.QName;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Implement xsl:apply-templates.
	/// <pre>
	/// &amp;!ELEMENT xsl:apply-templates (xsl:sort|xsl:with-param)*>
	/// &amp;!ATTLIST xsl:apply-templates
	///   select %expr; "node()"
	///   mode %qname; #IMPLIED
	/// &amp;
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Applying-Template-Rules">section-Applying-Template-Rules in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemApplyTemplates : ElemCallTemplate
	{
		internal new const long serialVersionUID = 2903125371542621004L;

	  /// <summary>
	  /// mode %qname; #IMPLIED
	  /// @serial
	  /// </summary>
	  private QName m_mode = null;

	  /// <summary>
	  /// Set the mode attribute for this element.
	  /// </summary>
	  /// <param name="mode"> reference, which may be null, to the <a href="http://www.w3.org/TR/xslt#modes">current mode</a>. </param>
	  public virtual QName Mode
	  {
		  set
		  {
			m_mode = value;
		  }
		  get
		  {
			return m_mode;
		  }
	  }


	  /// <summary>
	  /// Tells if this belongs to a default template,
	  /// in which case it will act different with
	  /// regard to processing modes. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#built-in-rule">built-in-rule in XSLT Specification</a>
	  /// @serial </seealso>
	  private bool m_isDefaultTemplate = false;

	//  /**
	//   * List of namespace/localname IDs, for identification of xsl:with-param to 
	//   * xsl:params.  Initialized in the compose() method.
	//   */
	//  private int[] m_paramIDs;

	  /// <summary>
	  /// Set if this belongs to a default template,
	  /// in which case it will act different with
	  /// regard to processing modes. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#built-in-rule">built-in-rule in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="b"> boolean value to set. </param>
	  public virtual bool IsDefaultTemplate
	  {
		  set
		  {
			m_isDefaultTemplate = value;
		  }
	  }

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> Token ID for this element types </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_APPLY_TEMPLATES;
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
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> Element name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_APPLY_TEMPLATES_STRING;
		  }
	  }

	  /// <summary>
	  /// Apply the context node to the matching templates. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Applying-Template-Rules">section-Applying-Template-Rules in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		transformer.pushCurrentTemplateRuleIsNull(false);

		bool pushMode = false;

		try
		{
		  // %REVIEW% Do we need this check??
		  //      if (null != sourceNode)
		  //      {
		  // boolean needToTurnOffInfiniteLoopCheck = false;
		  QName mode = transformer.Mode;

		  if (!m_isDefaultTemplate)
		  {
			if (((null == mode) && (null != m_mode)) || ((null != mode) && !mode.Equals(m_mode)))
			{
			  pushMode = true;

			  transformer.pushMode(m_mode);
			}
		  }
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEvent(this);
		  }

		  transformSelectedNodes(transformer);
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }

		  if (pushMode)
		  {
			transformer.popMode();
		  }

		  transformer.popCurrentTemplateRuleIsNull();
		}
	  }


	  /// <summary>
	  /// Perform a query if needed, and call transformNode for each child.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> Thrown in a variety of circumstances.
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transformSelectedNodes(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void transformSelectedNodes(TransformerImpl transformer)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xpath.XPathContext xctxt = transformer.getXPathContext();
		XPathContext xctxt = transformer.XPathContext;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int sourceNode = xctxt.getCurrentNode();
		int sourceNode = xctxt.CurrentNode;
		DTMIterator sourceNodes = m_selectExpression.asIterator(xctxt, sourceNode);
		VariableStack vars = xctxt.VarStack;
		int nParams = ParamElemCount;
		int thisframe = vars.StackFrame;
		StackGuard guard = transformer.StackGuard;
		bool check = (guard.RecursionLimit > -1) ? true : false;

		bool pushContextNodeListFlag = false;

		try
		{

				xctxt.pushCurrentNode(org.apache.xml.dtm.DTM_Fields.NULL);
				xctxt.pushCurrentExpressionNode(org.apache.xml.dtm.DTM_Fields.NULL);
				xctxt.pushSAXLocatorNull();
				transformer.pushElemTemplateElement(null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector keys = (m_sortElems == null) ? null : transformer.processSortKeys(this, sourceNode);
		  ArrayList keys = (m_sortElems == null) ? null : transformer.processSortKeys(this, sourceNode);

		  // Sort if we need to.
		  if (null != keys)
		  {
			sourceNodes = sortNodes(xctxt, keys, sourceNodes);
		  }

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireSelectedEvent(sourceNode, this, "select", new XPath(m_selectExpression), new org.apache.xpath.objects.XNodeSet(sourceNodes));
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.serializer.SerializationHandler rth = transformer.getSerializationHandler();
		  SerializationHandler rth = transformer.SerializationHandler;
	//      ContentHandler chandler = rth.getContentHandler();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StylesheetRoot sroot = transformer.getStylesheet();
		  StylesheetRoot sroot = transformer.Stylesheet;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TemplateList tl = sroot.getTemplateListComposed();
		  TemplateList tl = sroot.TemplateListComposed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean quiet = transformer.getQuietConflictWarnings();
		  bool quiet = transformer.QuietConflictWarnings;

		  // Should be able to get this from the iterator but there must be a bug.
		  DTM dtm = xctxt.getDTM(sourceNode);

		  int argsFrame = -1;
		  if (nParams > 0)
		  {
			// This code will create a section on the stack that is all the 
			// evaluated arguments.  These will be copied into the real params 
			// section of each called template.
			argsFrame = vars.link(nParams);
			vars.StackFrame = thisframe;

			for (int i = 0; i < nParams; i++)
			{
			  ElemWithParam ewp = m_paramElems[i];
			  if (transformer.Debug)
			  {
				transformer.TraceManager.fireTraceEvent(ewp);
			  }
			  XObject obj = ewp.getValue(transformer, sourceNode);
			  if (transformer.Debug)
			  {
				transformer.TraceManager.fireTraceEndEvent(ewp);
			  }

			  vars.setLocalVariable(i, obj, argsFrame);
			}
			vars.StackFrame = argsFrame;
		  }

		  xctxt.pushContextNodeList(sourceNodes);
		  pushContextNodeListFlag = true;

		  IntStack currentNodes = xctxt.CurrentNodeStack;

		  IntStack currentExpressionNodes = xctxt.CurrentExpressionNodeStack;

		  // pushParams(transformer, xctxt);

		  int child;
		  while (org.apache.xml.dtm.DTM_Fields.NULL != (child = sourceNodes.nextNode()))
		  {
			currentNodes.Top = child;
			currentExpressionNodes.Top = child;

			if (xctxt.getDTM(child) != dtm)
			{
			  dtm = xctxt.getDTM(child);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int exNodeType = dtm.getExpandedTypeID(child);
			int exNodeType = dtm.getExpandedTypeID(child);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = dtm.getNodeType(child);
			int nodeType = dtm.getNodeType(child);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.utils.QName mode = transformer.getMode();
			QName mode = transformer.Mode;

			ElemTemplate template = tl.getTemplateFast(xctxt, child, exNodeType, mode, -1, quiet, dtm);

			// If that didn't locate a node, fall back to a default template rule.
			// See http://www.w3.org/TR/xslt#built-in-rule.
			if (null == template)
			{
			  switch (nodeType)
			  {
			  case org.apache.xml.dtm.DTM_Fields.DOCUMENT_FRAGMENT_NODE :
			  case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
				template = sroot.DefaultRule;
				// %OPT% direct faster?
				break;
			  case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
			  case org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE :
			  case org.apache.xml.dtm.DTM_Fields.TEXT_NODE :
				// if(rth.m_elemIsPending || rth.m_docPending)
				//  rth.flushPending(true);
				transformer.pushPairCurrentMatched(sroot.DefaultTextRule, child);
				transformer.CurrentElement = sroot.DefaultTextRule;
				// dtm.dispatchCharactersEvents(child, chandler, false);
				dtm.dispatchCharactersEvents(child, rth, false);
				transformer.popCurrentMatched();
				continue;
			  case org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE :
				template = sroot.DefaultRootRule;
				break;
			  default :

				// No default rules for processing instructions and the like.
				continue;
			  }
			}
			else
			{
				transformer.CurrentElement = template;
			}

			transformer.pushPairCurrentMatched(template, child);
			if (check)
			{
				guard.checkForInfinateLoop();
			}

			int currentFrameBottom; // See comment with unlink, below
			if (template.m_frameSize > 0)
			{
			  xctxt.pushRTFContext();
			  currentFrameBottom = vars.StackFrame; // See comment with unlink, below
			  vars.link(template.m_frameSize);
			  // You can't do the check for nParams here, otherwise the 
			  // xsl:params might not be nulled.
			  if (template.m_inArgsSize > 0)
			  {
				int paramIndex = 0;
				for (ElemTemplateElement elem = template.FirstChildElem; null != elem; elem = elem.NextSiblingElem)
				{
				  if (Constants.ELEMNAME_PARAMVARIABLE == elem.XSLToken)
				  {
					ElemParam ep = (ElemParam)elem;

					int i;
					for (i = 0; i < nParams; i++)
					{
					  ElemWithParam ewp = m_paramElems[i];
					  if (ewp.m_qnameID == ep.m_qnameID)
					  {
						XObject obj = vars.getLocalVariable(i, argsFrame);
						vars.setLocalVariable(paramIndex, obj);
						break;
					  }
					}
					if (i == nParams)
					{
					  vars.setLocalVariable(paramIndex, null);
					}
				  }
				  else
				  {
					break;
				  }
				  paramIndex++;
				}

			  }
			}
			else
			{
				currentFrameBottom = 0;
			}

			// Fire a trace event for the template.
			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEvent(template);
			}

			// And execute the child templates.
			// Loop through the children of the template, calling execute on 
			// each of them.
			for (ElemTemplateElement t = template.m_firstChild; t != null; t = t.m_nextSibling)
			{
			  xctxt.SAXLocator = t;
			  try
			  {
				  transformer.pushElemTemplateElement(t);
				  t.execute(transformer);
			  }
			  finally
			  {
				  transformer.popElemTemplateElement();
			  }
			}

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEndEvent(template);
			}

			if (template.m_frameSize > 0)
			{
			  // See Frank Weiss bug around 03/19/2002 (no Bugzilla report yet).
			  // While unlink will restore to the proper place, the real position 
			  // may have been changed for xsl:with-param, so that variables 
			  // can be accessed.  
			  // of right now.
			  // More:
			  // When we entered this function, the current 
			  // frame buffer (cfb) index in the variable stack may 
			  // have been manually set.  If we just call 
			  // unlink(), however, it will restore the cfb to the 
			  // previous link index from the link stack, rather than 
			  // the manually set cfb.  So, 
			  // the only safe solution is to restore it back 
			  // to the same position it was on entry, since we're 
			  // really not working in a stack context here. (Bug4218)
			  vars.unlink(currentFrameBottom);
			  xctxt.popRTFContext();
			}

			transformer.popCurrentMatched();

		  } // end while (DTM.NULL != (child = sourceNodes.nextNode()))
		}
		catch (SAXException se)
		{
		  transformer.ErrorListener.fatalError(new TransformerException(se));
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireSelectedEndEvent(sourceNode, this, "select", new XPath(m_selectExpression), new org.apache.xpath.objects.XNodeSet(sourceNodes));
		  }

		  // Unlink to the original stack frame  
		  if (nParams > 0)
		  {
			vars.unlink(thisframe);
		  }
		  xctxt.popSAXLocator();
		  if (pushContextNodeListFlag)
		  {
			  xctxt.popContextNodeList();
		  }
		  transformer.popElemTemplateElement();
		  xctxt.popCurrentExpressionNode();
		  xctxt.popCurrentNode();
		  sourceNodes.detach();
		}
	  }

	}

}