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
 * $Id: ElemForEach.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using NodeSorter = org.apache.xalan.transformer.NodeSorter;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using IntStack = org.apache.xml.utils.IntStack;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;


	/// <summary>
	/// Implement xsl:for-each.
	/// <pre>
	/// <!ELEMENT xsl:for-each
	///  (#PCDATA
	///   %instructions;
	///   %result-elements;
	///   | xsl:sort)
	/// >
	/// 
	/// <!ATTLIST xsl:for-each
	///   select %expr; #REQUIRED
	///   %space-att;
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.for-each">for-each in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemForEach : ElemTemplateElement, ExpressionOwner
	{
		internal new const long serialVersionUID = 6018140636363583690L;
	  /// <summary>
	  /// Set true to request some basic status reports </summary>
	  internal const bool DEBUG = false;

	  /// <summary>
	  /// This is set by an "xalan-doc-cache-off" pi, or the old "xalan:doc-cache-off" pi.
	  /// The old form of the PI only works for XML parsers that are not namespace aware.
	  /// It tells the engine that
	  /// documents created in the location paths executed by this element
	  /// will not be reparsed. It's set by StylesheetHandler during
	  /// construction. Note that this feature applies _only_ to xsl:for-each
	  /// elements in its current incarnation; a more general cache management
	  /// solution is desperately needed.
	  /// </summary>
	  public bool m_doc_cache_off = false;

	  /// <summary>
	  /// Construct a element representing xsl:for-each.
	  /// </summary>
	  public ElemForEach()
	  {
	  }

	  /// <summary>
	  /// The "select" expression.
	  /// @serial
	  /// </summary>
	  protected internal Expression m_selectExpression = null;


	  /// <summary>
	  /// Used to fix bug#16889
	  /// Store XPath away for later processing.
	  /// </summary>
	  protected internal XPath m_xpath = null;

	  /// <summary>
	  /// Set the "select" attribute.
	  /// </summary>
	  /// <param name="xpath"> The XPath expression for the "select" attribute. </param>
	  public virtual XPath Select
	  {
		  set
		  {
			m_selectExpression = value.Expression;
    
			// The following line is part of the codes added to fix bug#16889
			// Store value which will be needed when firing Selected Event
			m_xpath = value;
		  }
		  get
		  {
			return m_selectExpression;
		  }
	  }


	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
	  /// NEEDSDOC <param name="sroot">
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {

		base.compose(sroot);

		int length = SortElemCount;

		for (int i = 0; i < length; i++)
		{
		  getSortElem(i).compose(sroot);
		}

		ArrayList vnames = sroot.ComposeState.VariableNames;

		if (null != m_selectExpression)
		{
		  m_selectExpression.fixupVariables(vnames, sroot.ComposeState.GlobalsSize);
		}
		else
		{
		  m_selectExpression = StylesheetRoot.m_selectDefault.Expression;
		}
	  }

	  /// <summary>
	  /// This after the template's children have been composed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCompose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void endCompose(StylesheetRoot sroot)
	  {
		int length = SortElemCount;

		for (int i = 0; i < length; i++)
		{
		  getSortElem(i).endCompose(sroot);
		}

		base.endCompose(sroot);
	  }


	  //  /**
	  //   * This function is called after everything else has been
	  //   * recomposed, and allows the template to set remaining
	  //   * values that may be based on some other property that
	  //   * depends on recomposition.
	  //   *
	  //   * @throws TransformerException
	  //   */
	  //  public void compose() throws TransformerException
	  //  {
	  //
	  //    if (null == m_selectExpression)
	  //    {
	  //      m_selectExpression =
	  //        getStylesheetRoot().m_selectDefault.getExpression();
	  //    }
	  //  }

	  /// <summary>
	  /// Vector containing the xsl:sort elements associated with this element.
	  ///  @serial
	  /// </summary>
	  protected internal ArrayList m_sortElems = null;

	  /// <summary>
	  /// Get the count xsl:sort elements associated with this element. </summary>
	  /// <returns> The number of xsl:sort elements. </returns>
	  public virtual int SortElemCount
	  {
		  get
		  {
			return (m_sortElems == null) ? 0 : m_sortElems.Count;
		  }
	  }

	  /// <summary>
	  /// Get a xsl:sort element associated with this element.
	  /// </summary>
	  /// <param name="i"> Index of xsl:sort element to get
	  /// </param>
	  /// <returns> xsl:sort element at given index </returns>
	  public virtual ElemSort getSortElem(int i)
	  {
		return (ElemSort) m_sortElems[i];
	  }

	  /// <summary>
	  /// Set a xsl:sort element associated with this element.
	  /// </summary>
	  /// <param name="sortElem"> xsl:sort element to set </param>
	  public virtual ElemSort SortElem
	  {
		  set
		  {
    
			if (null == m_sortElems)
			{
			  m_sortElems = new ArrayList();
			}
    
			m_sortElems.Add(value);
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
			return Constants.ELEMNAME_FOREACH;
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
			return Constants.ELEMNAME_FOREACH_STRING;
		  }
	  }

	  /// <summary>
	  /// Execute the xsl:for-each transformation
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		transformer.pushCurrentTemplateRuleIsNull(true);
		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this); //trigger for-each element event
		}

		try
		{
		  transformSelectedNodes(transformer);
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }
		  transformer.popCurrentTemplateRuleIsNull();
		}
	  }

	  /// <summary>
	  /// Get template element associated with this
	  /// 
	  /// </summary>
	  /// <returns> template element associated with this (itself) </returns>
	  protected internal virtual ElemTemplateElement TemplateMatch
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Sort given nodes
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state for the sort. </param>
	  /// <param name="keys"> Vector of sort keyx </param>
	  /// <param name="sourceNodes"> Iterator of nodes to sort
	  /// </param>
	  /// <returns> iterator of sorted nodes
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator sortNodes(org.apache.xpath.XPathContext xctxt, java.util.Vector keys, org.apache.xml.dtm.DTMIterator sourceNodes) throws javax.xml.transform.TransformerException
	  public virtual DTMIterator sortNodes(XPathContext xctxt, ArrayList keys, DTMIterator sourceNodes)
	  {

		NodeSorter sorter = new NodeSorter(xctxt);
		sourceNodes.ShouldCacheNodes = true;
		sourceNodes.runTo(-1);
		xctxt.pushContextNodeList(sourceNodes);

		try
		{
		  sorter.sort(sourceNodes, keys, xctxt);
		  sourceNodes.CurrentPos = 0;
		}
		finally
		{
		  xctxt.popContextNodeList();
		}

		return sourceNodes;
	  }

	  /// <summary>
	  /// Perform a query if needed, and call transformNode for each child.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> Thrown in a variety of circumstances.
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void transformSelectedNodes(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public virtual void transformSelectedNodes(TransformerImpl transformer)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xpath.XPathContext xctxt = transformer.getXPathContext();
		XPathContext xctxt = transformer.XPathContext;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int sourceNode = xctxt.getCurrentNode();
		int sourceNode = xctxt.CurrentNode;
		DTMIterator sourceNodes = m_selectExpression.asIterator(xctxt, sourceNode);

		try
		{

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

			// The original code, which is broken for bug#16889,
			// which fails to get the original select expression in the select event. 
			/*  transformer.getTraceManager().fireSelectedEvent(
			 *    sourceNode,
			 *            this,
			 *            "select",
			 *            new XPath(m_selectExpression),
			 *            new org.apache.xpath.objects.XNodeSet(sourceNodes));
			 */ 

			// The following code fixes bug#16889
			// Solution: Store away XPath in setSelect(Xath), and use it here.
			// Pass m_xath, which the current node is associated with, onto the TraceManager.

			Expression expr = m_xpath.Expression;
			org.apache.xpath.objects.XObject xObject = expr.execute(xctxt);
			int current = xctxt.CurrentNode;
			transformer.TraceManager.fireSelectedEvent(current, this, "select", m_xpath, xObject);
		}



		  xctxt.pushCurrentNode(DTM.NULL);

		  IntStack currentNodes = xctxt.CurrentNodeStack;

		  xctxt.pushCurrentExpressionNode(DTM.NULL);

		  IntStack currentExpressionNodes = xctxt.CurrentExpressionNodeStack;

		  xctxt.pushSAXLocatorNull();
		  xctxt.pushContextNodeList(sourceNodes);
		  transformer.pushElemTemplateElement(null);

		  // pushParams(transformer, xctxt);
		  // Should be able to get this from the iterator but there must be a bug.
		  DTM dtm = xctxt.getDTM(sourceNode);
		  int docID = sourceNode & DTMManager.IDENT_DTM_DEFAULT;
		  int child;

		  while (DTM.NULL != (child = sourceNodes.nextNode()))
		  {
			currentNodes.Top = child;
			currentExpressionNodes.Top = child;

			if ((child & DTMManager.IDENT_DTM_DEFAULT) != docID)
			{
			  dtm = xctxt.getDTM(child);
			  docID = child & DTMManager.IDENT_DTM_DEFAULT;
			}

			//final int exNodeType = dtm.getExpandedTypeID(child);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = dtm.getNodeType(child);
			int nodeType = dtm.getNodeType(child);

			// Fire a trace event for the template.
			if (transformer.Debug)
			{
			   transformer.TraceManager.fireTraceEvent(this);
			}

			// And execute the child templates.
			// Loop through the children of the template, calling execute on 
			// each of them.
			for (ElemTemplateElement t = this.m_firstChild; t != null; t = t.m_nextSibling)
			{
			  xctxt.SAXLocator = t;
			  transformer.CurrentElement = t;
			  t.execute(transformer);
			}

			if (transformer.Debug)
			{
			 // We need to make sure an old current element is not 
			  // on the stack.  See TransformerImpl#getElementCallstack.
			  transformer.CurrentElement = null;
			  transformer.TraceManager.fireTraceEndEvent(this);
			}


			 // KLUGE: Implement <?xalan:doc_cache_off?> 
			 // ASSUMPTION: This will be set only when the XPath was indeed
			 // a call to the Document() function. Calling it in other
			 // situations is likely to fry Xalan.
			 //
			 // %REVIEW% We need a MUCH cleaner solution -- one that will
			 // handle cleaning up after document() and getDTM() in other
			// contexts. The whole SourceTreeManager mechanism should probably
			 // be moved into DTMManager rather than being explicitly invoked in
			 // FuncDocument and here.
			 if (m_doc_cache_off)
			 {
			   if (DEBUG)
			   {
				 Console.WriteLine("JJK***** CACHE RELEASE *****\n" + "\tdtm=" + dtm.DocumentBaseURI);
			   }
			  // NOTE: This will work because this is _NOT_ a shared DTM, and thus has
			  // only a single Document node. If it could ever be an RTF or other
			 // shared DTM, this would require substantial rework.
			   xctxt.SourceTreeManager.removeDocumentFromCache(dtm.Document);
			   xctxt.release(dtm,false);
			 }
		  }
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireSelectedEndEvent(sourceNode, this, "select", new XPath(m_selectExpression), new org.apache.xpath.objects.XNodeSet(sourceNodes));
		  }

		  xctxt.popSAXLocator();
		  xctxt.popContextNodeList();
		  transformer.popElemTemplateElement();
		  xctxt.popCurrentExpressionNode();
		  xctxt.popCurrentNode();
		  sourceNodes.detach();
		}
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// <!ELEMENT xsl:apply-templates (xsl:sort|xsl:with-param)*>
	  /// <!ATTLIST xsl:apply-templates
	  ///   select %expr; "node()"
	  ///   mode %qname; #IMPLIED
	  /// >
	  /// </summary>
	  /// <param name="newChild"> Child to add to child list
	  /// </param>
	  /// <returns> Child just added to child list </returns>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		int type = ((ElemTemplateElement) newChild).XSLToken;

		if (Constants.ELEMNAME_SORT == type)
		{
		  SortElem = (ElemSort) newChild;

		  return newChild;
		}
		else
		{
		  return base.appendChild(newChild);
		}
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  public override void callChildVisitors(XSLTVisitor visitor, bool callAttributes)
	  {
		  if (callAttributes && (null != m_selectExpression))
		  {
			  m_selectExpression.callVisitors(this, visitor);
		  }

		int length = SortElemCount;

		for (int i = 0; i < length; i++)
		{
		  getSortElem(i).callVisitors(visitor);
		}

		base.callChildVisitors(visitor, callAttributes);
	  }

	  /// <seealso cref="ExpressionOwner.getExpression()"/>
	  public virtual Expression Expression
	  {
		  get
		  {
			return m_selectExpression;
		  }
		  set
		  {
			  value.exprSetParent(this);
			  m_selectExpression = value;
		  }
	  }


	  /*
	   * to keep the binary compatibility, assign a default value for newly added
	   * globel varialbe m_xpath during deserialization of an object which was 
	   * serialized using an older version
	   */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream os) throws IOException, ClassNotFoundException
	   private void readObject(ObjectInputStream os)
	   {
			   os.defaultReadObject();
			   m_xpath = null;
	   }
	}

}