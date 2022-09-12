using System;
using System.Collections;
using System.Text;

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
 * $Id: ElemNumber.java 1225442 2011-12-29 05:36:43Z mrglavas $
 */
namespace org.apache.xalan.templates
{


	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using CountersTable = org.apache.xalan.transformer.CountersTable;
	using DecimalToRoman = org.apache.xalan.transformer.DecimalToRoman;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using NodeVector = org.apache.xml.utils.NodeVector;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using StringBufferPool = org.apache.xml.utils.StringBufferPool;
	using XResourceBundle = org.apache.xml.utils.res.XResourceBundle;
	using CharArrayWrapper = org.apache.xml.utils.res.CharArrayWrapper;
	using IntArrayWrapper = org.apache.xml.utils.res.IntArrayWrapper;
	using LongArrayWrapper = org.apache.xml.utils.res.LongArrayWrapper;
	using StringArrayWrapper = org.apache.xml.utils.res.StringArrayWrapper;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	using Node = org.w3c.dom.Node;

	using SAXException = org.xml.sax.SAXException;

	// import org.apache.xalan.dtm.*;

	/// <summary>
	/// Implement xsl:number.
	/// <pre>
	/// <!ELEMENT xsl:number EMPTY>
	/// <!ATTLIST xsl:number
	///    level (single|multiple|any) "single"
	///    count %pattern; #IMPLIED
	///    from %pattern; #IMPLIED
	///    value %expr; #IMPLIED
	///    format %avt; '1'
	///    lang %avt; #IMPLIED
	///    letter-value %avt; #IMPLIED
	///    grouping-separator %avt; #IMPLIED
	///    grouping-size %avt; #IMPLIED
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#number">number in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemNumber : ElemTemplateElement
	{
		internal new const long serialVersionUID = 8118472298274407610L;

		/// <summary>
		/// Chars for converting integers into alpha counts. </summary>
		/// <seealso cref= TransformerImpl#int2alphaCount </seealso>
		private CharArrayWrapper m_alphaCountTable = null;

		private class MyPrefixResolver : PrefixResolver
		{
			private readonly ElemNumber outerInstance;


			internal DTM dtm;
			internal int handle;
			internal bool handleNullPrefix;

			/// <summary>
			/// Constructor for MyPrefixResolver. </summary>
			/// <param name="xpathExpressionContext"> </param>
			public MyPrefixResolver(ElemNumber outerInstance, Node xpathExpressionContext, DTM dtm, int handle, bool handleNullPrefix)
			{
				this.outerInstance = outerInstance;
				this.dtm = dtm;
				this.handle = handle;
				this.handleNullPrefix = handleNullPrefix;
			}

			/// <seealso cref= PrefixResolver#getNamespaceForPrefix(String, Node) </seealso>
			public virtual string getNamespaceForPrefix(string prefix)
			{
				return dtm.getNamespaceURI(handle);
			}

			/// <seealso cref= PrefixResolver#getNamespaceForPrefix(String, Node)
			/// this shouldn't get called. </seealso>
			public virtual string getNamespaceForPrefix(string prefix, Node context)
			{
				return getNamespaceForPrefix(prefix);
			}

			/// <seealso cref= PrefixResolver#getBaseIdentifier() </seealso>
			public virtual string BaseIdentifier
			{
				get
				{
					return outerInstance.BaseIdentifier;
				}
			}

			/// <seealso cref= PrefixResolver#handlesNullPrefixes() </seealso>
			public virtual bool handlesNullPrefixes()
			{
				return handleNullPrefix;
			}

		}

	  /// <summary>
	  /// Only nodes are counted that match this pattern.
	  /// @serial
	  /// </summary>
	  private XPath m_countMatchPattern = null;

	  /// <summary>
	  /// Set the "count" attribute.
	  /// The count attribute is a pattern that specifies what nodes
	  /// should be counted at those levels. If count attribute is not
	  /// specified, then it defaults to the pattern that matches any
	  /// node with the same node type as the current node and, if the
	  /// current node has an expanded-name, with the same expanded-name
	  /// as the current node.
	  /// </summary>
	  /// <param name="v"> Value to set for "count" attribute.  </param>
	  public virtual XPath Count
	  {
		  set
		  {
			m_countMatchPattern = value;
		  }
		  get
		  {
			return m_countMatchPattern;
		  }
	  }


	  /// <summary>
	  /// Specifies where to count from.
	  /// For level="single" or level="multiple":
	  /// Only ancestors that are searched are
	  /// those that are descendants of the nearest ancestor that matches
	  /// the from pattern.
	  /// For level="any:
	  /// Only nodes after the first node before the
	  /// current node that match the from pattern are considered.
	  /// @serial
	  /// </summary>
	  private XPath m_fromMatchPattern = null;

	  /// <summary>
	  /// Set the "from" attribute. Specifies where to count from.
	  /// For level="single" or level="multiple":
	  /// Only ancestors that are searched are
	  /// those that are descendants of the nearest ancestor that matches
	  /// the from pattern.
	  /// For level="any:
	  /// Only nodes after the first node before the
	  /// current node that match the from pattern are considered.
	  /// </summary>
	  /// <param name="v"> Value to set for "from" attribute. </param>
	  public virtual XPath From
	  {
		  set
		  {
			m_fromMatchPattern = value;
		  }
		  get
		  {
			return m_fromMatchPattern;
		  }
	  }


	  /// <summary>
	  /// When level="single", it goes up to the first node in the ancestor-or-self axis
	  /// that matches the count pattern, and constructs a list of length one containing
	  /// one plus the number of preceding siblings of that ancestor that match the count
	  /// pattern. If there is no such ancestor, it constructs an empty list. If the from
	  /// attribute is specified, then the only ancestors that are searched are those
	  /// that are descendants of the nearest ancestor that matches the from pattern.
	  /// Preceding siblings has the same meaning here as with the preceding-sibling axis.
	  /// 
	  /// When level="multiple", it constructs a list of all ancestors of the current node
	  /// in document order followed by the element itself; it then selects from the list
	  /// those nodes that match the count pattern; it then maps each node in the list to
	  /// one plus the number of preceding siblings of that node that match the count pattern.
	  /// If the from attribute is specified, then the only ancestors that are searched are
	  /// those that are descendants of the nearest ancestor that matches the from pattern.
	  /// Preceding siblings has the same meaning here as with the preceding-sibling axis.
	  /// 
	  /// When level="any", it constructs a list of length one containing the number of
	  /// nodes that match the count pattern and belong to the set containing the current
	  /// node and all nodes at any level of the document that are before the current node
	  /// in document order, excluding any namespace and attribute nodes (in other words
	  /// the union of the members of the preceding and ancestor-or-self axes). If the
	  /// from attribute is specified, then only nodes after the first node before the
	  /// current node that match the from pattern are considered.
	  /// @serial
	  /// </summary>
	  private int m_level = Constants.NUMBERLEVEL_SINGLE;

	  /// <summary>
	  /// Set the "level" attribute.
	  /// The level attribute specifies what levels of the source tree should
	  /// be considered; it has the values single, multiple or any. The default
	  /// is single.
	  /// </summary>
	  /// <param name="v"> Value to set for "level" attribute. </param>
	  public virtual int Level
	  {
		  set
		  {
			m_level = value;
		  }
		  get
		  {
			return m_level;
		  }
	  }


	  /// <summary>
	  /// The value attribute contains an expression. The expression is evaluated
	  /// and the resulting object is converted to a number as if by a call to the
	  /// number function.
	  /// @serial
	  /// </summary>
	  private XPath m_valueExpr = null;

	  /// <summary>
	  /// Set the "value" attribute.
	  /// The value attribute contains an expression. The expression is evaluated
	  /// and the resulting object is converted to a number as if by a call to the
	  /// number function.
	  /// </summary>
	  /// <param name="v"> Value to set for "value" attribute. </param>
	  public virtual XPath Value
	  {
		  set
		  {
			m_valueExpr = value;
		  }
		  get
		  {
			return m_valueExpr;
		  }
	  }


	  /// <summary>
	  /// The "format" attribute is used to control conversion of a list of
	  /// numbers into a string. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#convert">convert in XSLT Specification</a>
	  /// @serial </seealso>
	  private AVT m_format_avt = null;

	  /// <summary>
	  /// Set the "format" attribute.
	  /// The "format" attribute is used to control conversion of a list of
	  /// numbers into a string. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#convert">convert in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Value to set for "format" attribute. </param>
	  public virtual AVT Format
	  {
		  set
		  {
			m_format_avt = value;
		  }
		  get
		  {
			return m_format_avt;
		  }
	  }


	  /// <summary>
	  /// When numbering with an alphabetic sequence, the lang attribute
	  /// specifies which language's alphabet is to be used.
	  /// @serial
	  /// </summary>
	  private AVT m_lang_avt = null;

	  /// <summary>
	  /// Set the "lang" attribute.
	  /// When numbering with an alphabetic sequence, the lang attribute
	  /// specifies which language's alphabet is to be used; it has the same
	  /// range of values as xml:lang [XML]; if no lang value is specified,
	  /// the language should be determined from the system environment.
	  /// Implementers should document for which languages they support numbering. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#convert">convert in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Value to set for "lang" attribute. </param>
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
	  /// The letter-value attribute disambiguates between numbering
	  /// sequences that use letters.
	  /// @serial
	  /// </summary>
	  private AVT m_lettervalue_avt = null;

	  /// <summary>
	  /// Set the "letter-value" attribute.
	  /// The letter-value attribute disambiguates between numbering sequences
	  /// that use letters. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#convert">convert in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Value to set for "letter-value" attribute. </param>
	  public virtual AVT LetterValue
	  {
		  set
		  {
			m_lettervalue_avt = value;
		  }
		  get
		  {
			return m_lettervalue_avt;
		  }
	  }


	  /// <summary>
	  /// The grouping-separator attribute gives the separator
	  /// used as a grouping (e.g. thousands) separator in decimal
	  /// numbering sequences.
	  /// @serial
	  /// </summary>
	  private AVT m_groupingSeparator_avt = null;

	  /// <summary>
	  /// Set the "grouping-separator" attribute.
	  /// The grouping-separator attribute gives the separator
	  /// used as a grouping (e.g. thousands) separator in decimal
	  /// numbering sequences. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#convert">convert in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Value to set for "grouping-separator" attribute. </param>
	  public virtual AVT GroupingSeparator
	  {
		  set
		  {
			m_groupingSeparator_avt = value;
		  }
		  get
		  {
			return m_groupingSeparator_avt;
		  }
	  }


	  /// <summary>
	  /// The optional grouping-size specifies the size (normally 3) of the grouping.
	  /// @serial
	  /// </summary>
	  private AVT m_groupingSize_avt = null;

	  /// <summary>
	  /// Set the "grouping-size" attribute.
	  /// The optional grouping-size specifies the size (normally 3) of the grouping. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#convert">convert in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v"> Value to set for "grouping-size" attribute. </param>
	  public virtual AVT GroupingSize
	  {
		  set
		  {
			m_groupingSize_avt = value;
		  }
		  get
		  {
			return m_groupingSize_avt;
		  }
	  }


	  /// <summary>
	  /// Shouldn't this be in the transformer?  Big worries about threads...
	  /// </summary>

	  // private XResourceBundle thisBundle;

	  /// <summary>
	  /// Table to help in converting decimals to roman numerals. </summary>
	  /// <seealso cref= org.apache.xalan.transformer.DecimalToRoman </seealso>
	  private static readonly DecimalToRoman[] m_romanConvertTable = new DecimalToRoman[]
	  {
		  new DecimalToRoman(1000, "M", 900, "CM"),
		  new DecimalToRoman(500, "D", 400, "CD"),
		  new DecimalToRoman(100L, "C", 90L, "XC"),
		  new DecimalToRoman(50L, "L", 40L, "XL"),
		  new DecimalToRoman(10L, "X", 9L, "IX"),
		  new DecimalToRoman(5L, "V", 4L, "IV"),
		  new DecimalToRoman(1L, "I", 1L, "I")
	  };

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
		if (null != m_countMatchPattern)
		{
		  m_countMatchPattern.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_format_avt)
		{
		  m_format_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_fromMatchPattern)
		{
		  m_fromMatchPattern.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_groupingSeparator_avt)
		{
		  m_groupingSeparator_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_groupingSize_avt)
		{
		  m_groupingSize_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_lang_avt)
		{
		  m_lang_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_lettervalue_avt)
		{
		  m_lettervalue_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_valueExpr)
		{
		  m_valueExpr.fixupVariables(vnames, cstate.GlobalsSize);
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
			return Constants.ELEMNAME_NUMBER;
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
			return Constants.ELEMNAME_NUMBER_STRING;
		  }
	  }

	  /// <summary>
	  /// Execute an xsl:number instruction. The xsl:number element is
	  /// used to insert a formatted number into the result tree.
	  /// </summary>
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
		string countString = getCountString(transformer, sourceNode);

		try
		{
		  transformer.ResultTreeHandler.characters(countString.ToCharArray(), 0, countString.Length);
		}
		catch (SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }
		}
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// </summary>
	  /// <param name="newChild"> Child to add to child list
	  /// </param>
	  /// <returns> Child just added to child list
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
		return null;
	  }

	  /// <summary>
	  /// Given a 'from' pattern (ala xsl:number), a match pattern
	  /// and a context, find the first ancestor that matches the
	  /// pattern (including the context handed in).
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state for this. </param>
	  /// <param name="fromMatchPattern"> The ancestor must match this pattern. </param>
	  /// <param name="countMatchPattern"> The ancestor must also match this pattern. </param>
	  /// <param name="context"> The node that "." expresses. </param>
	  /// <param name="namespaceContext"> The context in which namespaces in the
	  /// queries are supposed to be expanded.
	  /// </param>
	  /// <returns> the first ancestor that matches the given pattern
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int findAncestor(org.apache.xpath.XPathContext xctxt, org.apache.xpath.XPath fromMatchPattern, org.apache.xpath.XPath countMatchPattern, int context, ElemNumber namespaceContext) throws javax.xml.transform.TransformerException
	  internal virtual int findAncestor(XPathContext xctxt, XPath fromMatchPattern, XPath countMatchPattern, int context, ElemNumber namespaceContext)
	  {
		DTM dtm = xctxt.getDTM(context);
		while (org.apache.xml.dtm.DTM_Fields.NULL != context)
		{
		  if (null != fromMatchPattern)
		  {
			if (fromMatchPattern.getMatchScore(xctxt, context) != XPath.MATCH_SCORE_NONE)
			{

			  //context = null;
			  break;
			}
		  }

		  if (null != countMatchPattern)
		  {
			if (countMatchPattern.getMatchScore(xctxt, context) != XPath.MATCH_SCORE_NONE)
			{
			  break;
			}
		  }

		  context = dtm.getParent(context);
		}

		return context;
	  }

	  /// <summary>
	  /// Given a 'from' pattern (ala xsl:number), a match pattern
	  /// and a context, find the first ancestor that matches the
	  /// pattern (including the context handed in). </summary>
	  /// <param name="xctxt"> The XPath runtime state for this. </param>
	  /// <param name="fromMatchPattern"> The ancestor must match this pattern. </param>
	  /// <param name="countMatchPattern"> The ancestor must also match this pattern. </param>
	  /// <param name="context"> The node that "." expresses. </param>
	  /// <param name="namespaceContext"> The context in which namespaces in the
	  /// queries are supposed to be expanded.
	  /// </param>
	  /// <returns> the first preceding, ancestor or self node that 
	  /// matches the given pattern
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int findPrecedingOrAncestorOrSelf(org.apache.xpath.XPathContext xctxt, org.apache.xpath.XPath fromMatchPattern, org.apache.xpath.XPath countMatchPattern, int context, ElemNumber namespaceContext) throws javax.xml.transform.TransformerException
	  private int findPrecedingOrAncestorOrSelf(XPathContext xctxt, XPath fromMatchPattern, XPath countMatchPattern, int context, ElemNumber namespaceContext)
	  {
		DTM dtm = xctxt.getDTM(context);
		while (org.apache.xml.dtm.DTM_Fields.NULL != context)
		{
		  if (null != fromMatchPattern)
		  {
			if (fromMatchPattern.getMatchScore(xctxt, context) != XPath.MATCH_SCORE_NONE)
			{
			  context = org.apache.xml.dtm.DTM_Fields.NULL;

			  break;
			}
		  }

		  if (null != countMatchPattern)
		  {
			if (countMatchPattern.getMatchScore(xctxt, context) != XPath.MATCH_SCORE_NONE)
			{
			  break;
			}
		  }

		  int prevSibling = dtm.getPreviousSibling(context);

		  if (org.apache.xml.dtm.DTM_Fields.NULL == prevSibling)
		  {
			context = dtm.getParent(context);
		  }
		  else
		  {

			// Now go down the chain of children of this sibling 
			context = dtm.getLastChild(prevSibling);

			if (context == org.apache.xml.dtm.DTM_Fields.NULL)
			{
			  context = prevSibling;
			}
		  }
		}

		return context;
	  }

	  /// <summary>
	  /// Get the count match pattern, or a default value.
	  /// </summary>
	  /// <param name="support"> The XPath runtime state for this. </param>
	  /// <param name="contextNode"> The node that "." expresses.
	  /// </param>
	  /// <returns> the count match pattern, or a default value. 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: org.apache.xpath.XPath getCountMatchPattern(org.apache.xpath.XPathContext support, int contextNode) throws javax.xml.transform.TransformerException
	  internal virtual XPath getCountMatchPattern(XPathContext support, int contextNode)
	  {

		XPath countMatchPattern = m_countMatchPattern;
		DTM dtm = support.getDTM(contextNode);
		if (null == countMatchPattern)
		{
		  switch (dtm.getNodeType(contextNode))
		  {
		  case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
			MyPrefixResolver resolver;

			if (string.ReferenceEquals(dtm.getNamespaceURI(contextNode), null))
			{
				 resolver = new MyPrefixResolver(this, dtm.getNode(contextNode), dtm,contextNode, false);
			}
			else
			{
				resolver = new MyPrefixResolver(this, dtm.getNode(contextNode), dtm,contextNode, true);
			}

			countMatchPattern = new XPath(dtm.getNodeName(contextNode), this, resolver, XPath.MATCH, support.ErrorListener);
			break;

		  case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :

			// countMatchPattern = m_stylesheet.createMatchPattern("@"+contextNode.getNodeName(), this);
			countMatchPattern = new XPath("@" + dtm.getNodeName(contextNode), this, this, XPath.MATCH, support.ErrorListener);
			break;
		  case org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE :
		  case org.apache.xml.dtm.DTM_Fields.TEXT_NODE :

			// countMatchPattern = m_stylesheet.createMatchPattern("text()", this);
			countMatchPattern = new XPath("text()", this, this, XPath.MATCH, support.ErrorListener);
			break;
		  case org.apache.xml.dtm.DTM_Fields.COMMENT_NODE :

			// countMatchPattern = m_stylesheet.createMatchPattern("comment()", this);
			countMatchPattern = new XPath("comment()", this, this, XPath.MATCH, support.ErrorListener);
			break;
		  case org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE :

			// countMatchPattern = m_stylesheet.createMatchPattern("/", this);
			countMatchPattern = new XPath("/", this, this, XPath.MATCH, support.ErrorListener);
			break;
		  case org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE :

			// countMatchPattern = m_stylesheet.createMatchPattern("pi("+contextNode.getNodeName()+")", this);
			countMatchPattern = new XPath("pi(" + dtm.getNodeName(contextNode) + ")", this, this, XPath.MATCH, support.ErrorListener);
			break;
		  default :
			countMatchPattern = null;
		break;
		  }
		}

		return countMatchPattern;
	  }

	  /// <summary>
	  /// Given an XML source node, get the count according to the
	  /// parameters set up by the xsl:number attributes. </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="sourceNode"> The source node being counted.
	  /// </param>
	  /// <returns> The count of nodes
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String getCountString(org.apache.xalan.transformer.TransformerImpl transformer, int sourceNode) throws javax.xml.transform.TransformerException
	  internal virtual string getCountString(TransformerImpl transformer, int sourceNode)
	  {

		long[] list = null;
		XPathContext xctxt = transformer.XPathContext;
		CountersTable ctable = transformer.CountersTable;

		if (null != m_valueExpr)
		{
		  XObject countObj = m_valueExpr.execute(xctxt, sourceNode, this);
		  //According to Errata E24
		  double d_count = Math.Floor(countObj.num() + 0.5);
		  if (double.IsNaN(d_count))
		  {
			  return "NaN";
		  }
		  else if (d_count < 0 && double.IsInfinity(d_count))
		  {
			  return "-Infinity";
		  }
		  else if (double.IsInfinity(d_count))
		  {
			  return "Infinity";
		  }
		  else if (d_count == 0)
		  {
			  return "0";
		  }
		  else
		  {
				  long count = (long)d_count;
				  list = new long[1];
				  list[0] = count;
		  }
		}
		else
		{
		  if (Constants.NUMBERLEVEL_ANY == m_level)
		  {
			list = new long[1];
			list[0] = ctable.countNode(xctxt, this, sourceNode);
		  }
		  else
		  {
			NodeVector ancestors = getMatchingAncestors(xctxt, sourceNode, Constants.NUMBERLEVEL_SINGLE == m_level);
			int lastIndex = ancestors.size() - 1;

			if (lastIndex >= 0)
			{
			  list = new long[lastIndex + 1];

			  for (int i = lastIndex; i >= 0; i--)
			  {
				int target = ancestors.elementAt(i);

				list[lastIndex - i] = ctable.countNode(xctxt, this, target);
			  }
			}
		  }
		}

		return (null != list) ? formatNumberList(transformer, list, sourceNode) : "";
	  }

	  /// <summary>
	  /// Get the previous node to be counted.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state for this. </param>
	  /// <param name="pos"> The current node
	  /// </param>
	  /// <returns> the previous node to be counted.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getPreviousNode(org.apache.xpath.XPathContext xctxt, int pos) throws javax.xml.transform.TransformerException
	  public virtual int getPreviousNode(XPathContext xctxt, int pos)
	  {

		XPath countMatchPattern = getCountMatchPattern(xctxt, pos);
		DTM dtm = xctxt.getDTM(pos);

		if (Constants.NUMBERLEVEL_ANY == m_level)
		{
		  XPath fromMatchPattern = m_fromMatchPattern;

		  // Do a backwards document-order walk 'till a node is found that matches 
		  // the 'from' pattern, or a node is found that matches the 'count' pattern, 
		  // or the top of the tree is found.
		  while (org.apache.xml.dtm.DTM_Fields.NULL != pos)
		  {

			// Get the previous sibling, if there is no previous sibling, 
			// then count the parent, but if there is a previous sibling, 
			// dive down to the lowest right-hand (last) child of that sibling.
			int next = dtm.getPreviousSibling(pos);

			if (org.apache.xml.dtm.DTM_Fields.NULL == next)
			{
			  next = dtm.getParent(pos);

			  if ((org.apache.xml.dtm.DTM_Fields.NULL != next) && ((((null != fromMatchPattern) && (fromMatchPattern.getMatchScore(xctxt, next) != XPath.MATCH_SCORE_NONE))) || (dtm.getNodeType(next) == org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE)))
			  {
				pos = org.apache.xml.dtm.DTM_Fields.NULL; // return null from function.

				break; // from while loop
			  }
			}
			else
			{

			  // dive down to the lowest right child.
			  int child = next;

			  while (org.apache.xml.dtm.DTM_Fields.NULL != child)
			  {
				child = dtm.getLastChild(next);

				if (org.apache.xml.dtm.DTM_Fields.NULL != child)
				{
				  next = child;
				}
			  }
			}

			pos = next;

			if ((org.apache.xml.dtm.DTM_Fields.NULL != pos) && ((null == countMatchPattern) || (countMatchPattern.getMatchScore(xctxt, pos) != XPath.MATCH_SCORE_NONE)))
			{
			  break;
			}
		  }
		}
		else // NUMBERLEVEL_MULTI or NUMBERLEVEL_SINGLE
		{
		  while (org.apache.xml.dtm.DTM_Fields.NULL != pos)
		  {
			pos = dtm.getPreviousSibling(pos);

			if ((org.apache.xml.dtm.DTM_Fields.NULL != pos) && ((null == countMatchPattern) || (countMatchPattern.getMatchScore(xctxt, pos) != XPath.MATCH_SCORE_NONE)))
			{
			  break;
			}
		  }
		}

		return pos;
	  }

	  /// <summary>
	  /// Get the target node that will be counted..
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state for this. </param>
	  /// <param name="sourceNode"> non-null reference to the <a href="http://www.w3.org/TR/xslt#dt-current-node">current source node</a>.
	  /// </param>
	  /// <returns> the target node that will be counted
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getTargetNode(org.apache.xpath.XPathContext xctxt, int sourceNode) throws javax.xml.transform.TransformerException
	  public virtual int getTargetNode(XPathContext xctxt, int sourceNode)
	  {

		int target = org.apache.xml.dtm.DTM_Fields.NULL;
		XPath countMatchPattern = getCountMatchPattern(xctxt, sourceNode);

		if (Constants.NUMBERLEVEL_ANY == m_level)
		{
		  target = findPrecedingOrAncestorOrSelf(xctxt, m_fromMatchPattern, countMatchPattern, sourceNode, this);
		}
		else
		{
		  target = findAncestor(xctxt, m_fromMatchPattern, countMatchPattern, sourceNode, this);
		}

		return target;
	  }

	  /// <summary>
	  /// Get the ancestors, up to the root, that match the
	  /// pattern.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state for this. </param>
	  /// <param name="node"> Count this node and it's ancestors. </param>
	  /// <param name="stopAtFirstFound"> Flag indicating to stop after the
	  /// first node is found (difference between level = single
	  /// or multiple) </param>
	  /// <returns> The number of ancestors that match the pattern.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: org.apache.xml.utils.NodeVector getMatchingAncestors(org.apache.xpath.XPathContext xctxt, int node, boolean stopAtFirstFound) throws javax.xml.transform.TransformerException
	  internal virtual NodeVector getMatchingAncestors(XPathContext xctxt, int node, bool stopAtFirstFound)
	  {

		NodeSetDTM ancestors = new NodeSetDTM(xctxt.DTMManager);
		XPath countMatchPattern = getCountMatchPattern(xctxt, node);
		DTM dtm = xctxt.getDTM(node);

		while (org.apache.xml.dtm.DTM_Fields.NULL != node)
		{
		  if ((null != m_fromMatchPattern) && (m_fromMatchPattern.getMatchScore(xctxt, node) != XPath.MATCH_SCORE_NONE))
		  {

			// The following if statement gives level="single" different 
			// behavior from level="multiple", which seems incorrect according 
			// to the XSLT spec.  For now we are leaving this in to replicate 
			// the same behavior in XT, but, for all intents and purposes we 
			// think this is a bug, or there is something about level="single" 
			// that we still don't understand.
			if (!stopAtFirstFound)
			{
			  break;
			}
		  }

		  if (null == countMatchPattern)
		  {
			Console.WriteLine("Programmers error! countMatchPattern should never be null!");
		  }

		  if (countMatchPattern.getMatchScore(xctxt, node) != XPath.MATCH_SCORE_NONE)
		  {
			ancestors.addElement(node);

			if (stopAtFirstFound)
			{
			  break;
			}
		  }

		  node = dtm.getParent(node);
		}

		return ancestors;
	  } // end getMatchingAncestors method

	  /// <summary>
	  /// Get the locale we should be using.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="contextNode"> The node that "." expresses.
	  /// </param>
	  /// <returns> The locale to use. May be specified by "lang" attribute,
	  /// but if not, use default locale on the system. 
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: java.util.Locale getLocale(org.apache.xalan.transformer.TransformerImpl transformer, int contextNode) throws javax.xml.transform.TransformerException
	  internal virtual Locale getLocale(TransformerImpl transformer, int contextNode)
	  {

		Locale locale = null;

		if (null != m_lang_avt)
		{
		  XPathContext xctxt = transformer.XPathContext;
		  string langValue = m_lang_avt.evaluate(xctxt, contextNode, this);

		  if (null != langValue)
		  {

			// Not really sure what to do about the country code, so I use the
			// default from the system.
			// TODO: fix xml:lang handling.
			locale = new Locale(langValue.ToUpper(), "");

			//Locale.getDefault().getDisplayCountry());
			if (null == locale)
			{
			  transformer.MsgMgr.warn(this, null, xctxt.getDTM(contextNode).getNode(contextNode), XSLTErrorResources.WG_LOCALE_NOT_FOUND, new object[]{langValue}); //"Warning: Could not find locale for xml:lang="+langValue);

			  locale = Locale.Default;
			}
		  }
		}
		else
		{
		  locale = Locale.Default;
		}

		return locale;
	  }

	  /// <summary>
	  /// Get the number formatter to be used the format the numbers
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="contextNode"> The node that "." expresses.
	  /// </param>
	  /// ($objectName$) <returns> The number formatter to be used
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.text.DecimalFormat getNumberFormatter(org.apache.xalan.transformer.TransformerImpl transformer, int contextNode) throws javax.xml.transform.TransformerException
	  private DecimalFormat getNumberFormatter(TransformerImpl transformer, int contextNode)
	  {
		// Patch from Steven Serocki
		// Maybe we really want to do the clone in getLocale() and return  
		// a clone of the default Locale??
		Locale locale = (Locale)getLocale(transformer, contextNode).clone();

		// Helper to format local specific numbers to strings.
		DecimalFormat formatter = null;

		//synchronized (locale)
		//{
		//     formatter = (DecimalFormat) NumberFormat.getNumberInstance(locale);
		//}

		string digitGroupSepValue = (null != m_groupingSeparator_avt) ? m_groupingSeparator_avt.evaluate(transformer.XPathContext, contextNode, this) : null;


		// Validate grouping separator if an AVT was used; otherwise this was 
		// validated statically in XSLTAttributeDef.java.
		if ((!string.ReferenceEquals(digitGroupSepValue, null)) && (!m_groupingSeparator_avt.Simple) && (digitGroupSepValue.Length != 1))
		{
				transformer.MsgMgr.warn(this, XSLTErrorResources.WG_ILLEGAL_ATTRIBUTE_VALUE, new object[]{Constants.ATTRNAME_NAME, m_groupingSeparator_avt.Name});
		}


		string nDigitsPerGroupValue = (null != m_groupingSize_avt) ? m_groupingSize_avt.evaluate(transformer.XPathContext, contextNode, this) : null;

		// TODO: Handle digit-group attributes
		if ((null != digitGroupSepValue) && (null != nDigitsPerGroupValue) && (digitGroupSepValue.Length > 0))
		{
			// Ignore if separation value is empty string
		  try
		  {
			formatter = (DecimalFormat) NumberFormat.getNumberInstance(locale);
			formatter.GroupingSize = Convert.ToInt32(nDigitsPerGroupValue);

			DecimalFormatSymbols symbols = formatter.DecimalFormatSymbols;
			symbols.GroupingSeparator = digitGroupSepValue[0];
			formatter.DecimalFormatSymbols = symbols;
			formatter.GroupingUsed = true;
		  }
		  catch (System.FormatException)
		  {
			formatter.GroupingUsed = false;
		  }
		}

		return formatter;
	  }

	  /// <summary>
	  /// Format a vector of numbers into a formatted string.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="list"> Array of one or more long integer numbers. </param>
	  /// <param name="contextNode"> The node that "." expresses. </param>
	  /// <returns> String that represents list according to
	  /// %conversion-atts; attributes.
	  /// TODO: Optimize formatNumberList so that it caches the last count and
	  /// reuses that info for the next count.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String formatNumberList(org.apache.xalan.transformer.TransformerImpl transformer, long[] list, int contextNode) throws javax.xml.transform.TransformerException
	  internal virtual string formatNumberList(TransformerImpl transformer, long[] list, int contextNode)
	  {

		string numStr;
		FastStringBuffer formattedNumber = StringBufferPool.get();

		try
		{
		  int nNumbers = list.Length, numberWidth = 1;
		  char numberType = '1';
		  string formatToken , lastSepString = null, formatTokenString = null;

		  // If a seperator hasn't been specified, then use "."  
		  // as a default separator. 
		  // For instance: [2][1][5] with a format value of "1 "
		  // should format to "2.1.5 " (I think).
		  // Otherwise, use the seperator specified in the format string.
		  // For instance: [2][1][5] with a format value of "01-001. "
		  // should format to "02-001-005 ".
		  string lastSep = ".";
		  bool isFirstToken = true; // true if first token
		  string formatValue = (null != m_format_avt) ? m_format_avt.evaluate(transformer.XPathContext, contextNode, this) : null;

		  if (null == formatValue)
		  {
			formatValue = "1";
		  }

		  NumberFormatStringTokenizer formatTokenizer = new NumberFormatStringTokenizer(this, formatValue);

		  // int sepCount = 0;                  // keep track of seperators
		  // Loop through all the numbers in the list.
		  for (int i = 0; i < nNumbers; i++)
		  {

			// Loop to the next digit, letter, or separator.
			if (formatTokenizer.hasMoreTokens())
			{
			  formatToken = formatTokenizer.nextToken();

			  // If the first character of this token is a character or digit, then 
			  // it is a number format directive.
			  if (char.IsLetterOrDigit(formatToken[formatToken.Length - 1]))
			  {
				numberWidth = formatToken.Length;
				numberType = formatToken[numberWidth - 1];
			  }

			  // If there is a number format directive ahead, 
			  // then append the formatToken.
			  else if (formatTokenizer.LetterOrDigitAhead)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer formatTokenStringBuffer = new StringBuffer(formatToken);
				StringBuilder formatTokenStringBuffer = new StringBuilder(formatToken);

				// Append the formatToken string...
				// For instance [2][1][5] with a format value of "1--1. "
				// should format to "2--1--5. " (I guess).
				while (formatTokenizer.nextIsSep())
				{
				  formatToken = formatTokenizer.nextToken();
				  formatTokenStringBuffer.Append(formatToken);
				}
				formatTokenString = formatTokenStringBuffer.ToString();

				// Record this separator, so it can be used as the 
				// next separator, if the next is the last.
				// For instance: [2][1][5] with a format value of "1-1 "
				// should format to "2-1-5 ".
				if (!isFirstToken)
				{
				  lastSep = formatTokenString;
				}

				// Since we know the next is a number or digit, we get it now.
				formatToken = formatTokenizer.nextToken();
				numberWidth = formatToken.Length;
				numberType = formatToken[numberWidth - 1];
			  }
			  else // only separators left
			  {

				// Set up the string for the trailing characters after 
				// the last number is formatted (i.e. after the loop).
				lastSepString = formatToken;

				// And append any remaining characters to the lastSepString.
				while (formatTokenizer.hasMoreTokens())
				{
				  formatToken = formatTokenizer.nextToken();
				  lastSepString += formatToken;
				}
			  } // else
			} // end if(formatTokenizer.hasMoreTokens())

			// if this is the first token and there was a prefix
			// append the prefix else, append the separator
			// For instance, [2][1][5] with a format value of "(1-1.) "
			// should format to "(2-1-5.) " (I guess).
			if (null != formatTokenString && isFirstToken)
			{
			  formattedNumber.append(formatTokenString);
			}
			else if (null != lastSep && !isFirstToken)
			{
			  formattedNumber.append(lastSep);
			}

			getFormattedNumber(transformer, contextNode, numberType, numberWidth, list[i], formattedNumber);

			isFirstToken = false; // After the first pass, this should be false
		  } // end for loop

		  // Check to see if we finished up the format string...
		  // Skip past all remaining letters or digits
		  while (formatTokenizer.LetterOrDigitAhead)
		  {
			formatTokenizer.nextToken();
		  }

		  if (!string.ReferenceEquals(lastSepString, null))
		  {
			formattedNumber.append(lastSepString);
		  }

		  while (formatTokenizer.hasMoreTokens())
		  {
			formatToken = formatTokenizer.nextToken();

			formattedNumber.append(formatToken);
		  }

		  numStr = formattedNumber.ToString();
		}
		finally
		{
		  StringBufferPool.free(formattedNumber);
		}

		return numStr;
	  } // end formatNumberList method

	  /*
	  * Get Formatted number
	  */

	  /// <summary>
	  /// Format the given number and store it in the given buffer 
	  /// 
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="contextNode"> The node that "." expresses. </param>
	  /// <param name="numberType"> Type to format to </param>
	  /// <param name="numberWidth"> Maximum length of formatted number </param>
	  /// <param name="listElement"> Number to format </param>
	  /// <param name="formattedNumber"> Buffer to store formatted number
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void getFormattedNumber(org.apache.xalan.transformer.TransformerImpl transformer, int contextNode, char numberType, int numberWidth, long listElement, org.apache.xml.utils.FastStringBuffer formattedNumber) throws javax.xml.transform.TransformerException
	  private void getFormattedNumber(TransformerImpl transformer, int contextNode, char numberType, int numberWidth, long listElement, FastStringBuffer formattedNumber)
	  {


		string letterVal = (m_lettervalue_avt != null) ? m_lettervalue_avt.evaluate(transformer.XPathContext, contextNode, this) : null;

		/// <summary>
		/// Wrapper of Chars for converting integers into alpha counts.
		/// </summary>
		CharArrayWrapper alphaCountTable = null;

		XResourceBundle thisBundle = null;

		switch (numberType)
		{
		case 'A' :
			if (null == m_alphaCountTable)
			{
					thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, getLocale(transformer, contextNode));
					m_alphaCountTable = (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET);
			}
		  int2alphaCount(listElement, m_alphaCountTable, formattedNumber);
		  break;
		case 'a' :
			if (null == m_alphaCountTable)
			{
					thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, getLocale(transformer, contextNode));
					m_alphaCountTable = (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET);
			}
		  FastStringBuffer stringBuf = StringBufferPool.get();

		  try
		  {
			int2alphaCount(listElement, m_alphaCountTable, stringBuf);
			formattedNumber.append(stringBuf.ToString().ToLower(getLocale(transformer, contextNode)));
		  }
		  finally
		  {
			StringBufferPool.free(stringBuf);
		  }
		  break;
		case 'I' :
		  formattedNumber.append(long2roman(listElement, true));
		  break;
		case 'i' :
		  formattedNumber.append(long2roman(listElement, true).ToLower(getLocale(transformer, contextNode)));
		  break;
		case 0x3042 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("ja", "JP", "HA"));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			formattedNumber.append(int2singlealphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET)));
		  }

		  break;
		}
		case 0x3044 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("ja", "JP", "HI"));

		  if ((!string.ReferenceEquals(letterVal, null)) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			formattedNumber.append(int2singlealphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET)));
		  }

		  break;
		}
		case 0x30A2 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("ja", "JP", "A"));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			formattedNumber.append(int2singlealphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET)));
		  }

		  break;
		}
		case 0x30A4 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("ja", "JP", "I"));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			formattedNumber.append(int2singlealphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET)));
		  }

		  break;
		}
		case 0x4E00 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("zh", "CN"));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		case 0x58F9 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("zh", "TW"));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		case 0x0E51 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("th", ""));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		case 0x05D0 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("he", ""));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		case 0x10D0 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("ka", ""));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		case 0x03B1 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("el", ""));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		case 0x0430 :
		{

		  thisBundle = (XResourceBundle) XResourceBundle.loadResourceBundle(XResourceBundle.LANG_BUNDLE_NAME, new Locale("cy", ""));

		  if (!string.ReferenceEquals(letterVal, null) && letterVal.Equals(Constants.ATTRVAL_TRADITIONAL))
		  {
			formattedNumber.append(tradAlphaCount(listElement, thisBundle));
		  }
		  else //if (m_lettervalue_avt != null && m_lettervalue_avt.equals(Constants.ATTRVAL_ALPHABETIC))
		  {
			int2alphaCount(listElement, (CharArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_ALPHABET), formattedNumber);
		  }

		  break;
		}
		default : // "1"
		  DecimalFormat formatter = getNumberFormatter(transformer, contextNode);
		  string padString = formatter == null ? 0.ToString() : formatter.format(0);
		  string numString = formatter == null ? listElement.ToString() : formatter.format(listElement);
		  int nPadding = numberWidth - numString.Length;

		  for (int k = 0; k < nPadding; k++)
		  {
			formattedNumber.append(padString);
		  }

		  formattedNumber.append(numString);
	  break;
		}
	  }

	  /// <summary>
	  /// Get a string value for zero, which is not really defined by the 1.0 spec, 
	  /// thought I think it might be cleared up by the erreta.
	  /// </summary>
	   internal virtual string ZeroString
	   {
		   get
		   {
			 return "" + 0;
		   }
	   }

	  /// <summary>
	  /// Convert a long integer into alphabetic counting, in other words
	  /// count using the sequence A B C ... Z.
	  /// </summary>
	  /// <param name="val"> Value to convert -- must be greater than zero. </param>
	  /// <param name="table"> a table containing one character for each digit in the radix </param>
	  /// <returns> String representing alpha count of number. </returns>
	  /// <seealso cref= TransformerImpl#DecimalToRoman
	  /// 
	  /// Note that the radix of the conversion is inferred from the size
	  /// of the table. </seealso>
	  protected internal virtual string int2singlealphaCount(long val, CharArrayWrapper table)
	  {

		int radix = table.Length;

		// TODO:  throw error on out of range input
		if (val > radix)
		{
		  return ZeroString;
		}
		else
		{
		  return (new char?(table.getChar((int)val - 1))).ToString(); // index into table is off one, starts at 0
		}
	  }

	  /// <summary>
	  /// Convert a long integer into alphabetic counting, in other words
	  /// count using the sequence A B C ... Z AA AB AC.... etc.
	  /// </summary>
	  /// <param name="val"> Value to convert -- must be greater than zero. </param>
	  /// <param name="table"> a table containing one character for each digit in the radix </param>
	  /// <param name="aTable"> Array of alpha characters representing numbers </param>
	  /// <param name="stringBuf"> Buffer where to save the string representing alpha count of number.
	  /// </param>
	  /// <seealso cref= TransformerImpl#DecimalToRoman
	  /// 
	  /// Note that the radix of the conversion is inferred from the size
	  /// of the table. </seealso>
	  protected internal virtual void int2alphaCount(long val, CharArrayWrapper aTable, FastStringBuffer stringBuf)
	  {

		int radix = aTable.Length;
		char[] table = new char[radix];

		// start table at 1, add last char at index 0. Reason explained above and below.
		int i;

		for (i = 0; i < radix - 1; i++)
		{
		  table[i + 1] = aTable.getChar(i);
		}

		table[0] = aTable.getChar(i);

		// Create a buffer to hold the result
		// TODO:  size of the table can be detereined by computing
		// logs of the radix.  For now, we fake it.
		char[] buf = new char[100];

		//some languages go left to right(ie. english), right to left (ie. Hebrew),
		//top to bottom (ie.Japanese), etc... Handle them differently
		//String orientation = thisBundle.getString(org.apache.xml.utils.res.XResourceBundle.LANG_ORIENTATION);
		// next character to set in the buffer
		int charPos;

		charPos = buf.Length - 1; // work backward through buf[]

		// index in table of the last character that we stored
		int lookupIndex = 1; // start off with anything other than zero to make correction work

		//                                          Correction number
		//
		//  Correction can take on exactly two values:
		//
		//          0       if the next character is to be emitted is usual
		//
		//      radix - 1
		//                  if the next char to be emitted should be one less than
		//                  you would expect
		//                  
		// For example, consider radix 10, where 1="A" and 10="J"
		//
		// In this scheme, we count: A, B, C ...   H, I, J (not A0 and certainly
		// not AJ), A1
		//
		// So, how do we keep from emitting AJ for 10?  After correctly emitting the
		// J, lookupIndex is zero.  We now compute a correction number of 9 (radix-1).
		// In the following line, we'll compute (val+correction) % radix, which is,
		// (val+9)/10.  By this time, val is 1, so we compute (1+9) % 10, which
		// is 10 % 10 or zero.  So, we'll prepare to emit "JJ", but then we'll
		// later suppress the leading J as representing zero (in the mod system,
		// it can represent either 10 or zero).  In summary, the correction value of
		// "radix-1" acts like "-1" when run through the mod operator, but with the
		// desireable characteristic that it never produces a negative number.
		long correction = 0;

		// TODO:  throw error on out of range input
		do
		{

		  // most of the correction calculation is explained above,  the reason for the
		  // term after the "|| " is that it correctly propagates carries across
		  // multiple columns.
		  correction = ((lookupIndex == 0) || (correction != 0 && lookupIndex == radix - 1)) ? (radix - 1) : 0;

		  // index in "table" of the next char to emit
		  lookupIndex = (int)(val + correction) % radix;

		  // shift input by one "column"
		  val = (val / radix);

		  // if the next value we'd put out would be a leading zero, we're done.
		  if (lookupIndex == 0 && val == 0)
		  {
			break;
		  }

		  // put out the next character of output
		  buf[charPos--] = table[lookupIndex]; // left to right or top to bottom
		} while (val > 0);

		stringBuf.append(buf, charPos + 1, (buf.Length - charPos - 1));
	  }

	  /// <summary>
	  /// Convert a long integer into traditional alphabetic counting, in other words
	  /// count using the traditional numbering.
	  /// </summary>
	  /// <param name="val"> Value to convert -- must be greater than zero. </param>
	  /// <param name="thisBundle"> Resource bundle to use
	  /// </param>
	  /// <returns> String representing alpha count of number. </returns>
	  /// <seealso cref= XSLProcessor#DecimalToRoman
	  /// 
	  /// Note that the radix of the conversion is inferred from the size
	  /// of the table. </seealso>
	  protected internal virtual string tradAlphaCount(long val, XResourceBundle thisBundle)
	  {

		// if this number is larger than the largest number we can represent, error!
		if (val > long.MaxValue)
		{
		  this.error(XSLTErrorResources.ER_NUMBER_TOO_BIG);
		  return XSLTErrorResources.ERROR_STRING;
		}
		char[] table = null;

		// index in table of the last character that we stored
		int lookupIndex = 1; // start off with anything other than zero to make correction work

		// Create a buffer to hold the result
		// TODO:  size of the table can be detereined by computing
		// logs of the radix.  For now, we fake it.
		char[] buf = new char[100];

		//some languages go left to right(ie. english), right to left (ie. Hebrew),
		//top to bottom (ie.Japanese), etc... Handle them differently
		//String orientation = thisBundle.getString(org.apache.xml.utils.res.XResourceBundle.LANG_ORIENTATION);
		// next character to set in the buffer
		int charPos;

		charPos = 0; //start at 0

		// array of number groups: ie.1000, 100, 10, 1
		IntArrayWrapper groups = (IntArrayWrapper) thisBundle.getObject(XResourceBundle.LANG_NUMBERGROUPS);

		// array of tables of hundreds, tens, digits...
		StringArrayWrapper tables = (StringArrayWrapper)(thisBundle.getObject(XResourceBundle.LANG_NUM_TABLES));

		//some languages have additive alphabetical notation,
		//some multiplicative-additive, etc... Handle them differently.
		string numbering = thisBundle.getString(XResourceBundle.LANG_NUMBERING);

		// do multiplicative part first
		if (numbering.Equals(XResourceBundle.LANG_MULT_ADD))
		{
		  string mult_order = thisBundle.getString(XResourceBundle.MULT_ORDER);
		  LongArrayWrapper multiplier = (LongArrayWrapper)(thisBundle.getObject(XResourceBundle.LANG_MULTIPLIER));
		  CharArrayWrapper zeroChar = (CharArrayWrapper) thisBundle.getObject("zero");
		  int i = 0;

		  // skip to correct multiplier
		  while (i < multiplier.Length && val < multiplier.getLong(i))
		  {
			i++;
		  }

		  do
		  {
			if (i >= multiplier.Length)
			{
			  break; //number is smaller than multipliers
			}

			// some languages (ie chinese) put a zero character (and only one) when
			// the multiplier is multiplied by zero. (ie, 1001 is 1X1000 + 0X100 + 0X10 + 1)
			// 0X100 is replaced by the zero character, we don't need one for 0X10
			if (val < multiplier.getLong(i))
			{
			  if (zeroChar.Length == 0)
			  {
				i++;
			  }
			  else
			  {
				if (buf[charPos - 1] != zeroChar.getChar(0))
				{
				  buf[charPos++] = zeroChar.getChar(0);
				}

				i++;
			  }
			}
			else if (val >= multiplier.getLong(i))
			{
			  long mult = val / multiplier.getLong(i);

			  val = val % multiplier.getLong(i); // save this.

			  int k = 0;

			  while (k < groups.Length)
			  {
				lookupIndex = 1; // initialize for each table

				if (mult / groups.getInt(k) <= 0) // look for right table
				{
				  k++;
				}
				else
				{

				  // get the table
				  CharArrayWrapper THEletters = (CharArrayWrapper) thisBundle.getObject(tables.getString(k));

				  table = new char[THEletters.Length + 1];

				  int j;

				  for (j = 0; j < THEletters.Length; j++)
				  {
					table[j + 1] = THEletters.getChar(j);
				  }

				  table[0] = THEletters.getChar(j - 1); // don't need this

				  // index in "table" of the next char to emit
				  lookupIndex = (int)mult / groups.getInt(k);

				  //this should not happen
				  if (lookupIndex == 0 && mult == 0)
				  {
					break;
				  }

				  char multiplierChar = ((CharArrayWrapper)(thisBundle.getObject(XResourceBundle.LANG_MULTIPLIER_CHAR))).getChar(i);

				  // put out the next character of output   
				  if (lookupIndex < table.Length)
				  {
					if (mult_order.Equals(XResourceBundle.MULT_PRECEDES))
					{
					  buf[charPos++] = multiplierChar;
					  buf[charPos++] = table[lookupIndex];
					}
					else
					{

					  // don't put out 1 (ie 1X10 is just 10)
					  if (lookupIndex == 1 && i == multiplier.Length - 1)
					  {
					  }
					  else
					  {
						buf[charPos++] = table[lookupIndex];
					  }

					  buf[charPos++] = multiplierChar;
					}

					break; // all done!
				  }
				  else
				  {
					return XSLTErrorResources.ERROR_STRING;
				  }
				} //end else
			  } // end while

			  i++;
			} // end else if
		  } while (i < multiplier.Length); // end do while
		}

		// Now do additive part...
		int count = 0;
		string tableName;

		// do this for each table of hundreds, tens, digits...
		while (count < groups.Length)
		{
		  if (val / groups.getInt(count) <= 0) // look for correct table
		  {
			count++;
		  }
		  else
		  {
			CharArrayWrapper theletters = (CharArrayWrapper) thisBundle.getObject(tables.getString(count));

			table = new char[theletters.Length + 1];

			int j;

			// need to start filling the table up at index 1
			for (j = 0; j < theletters.Length; j++)
			{
			  table[j + 1] = theletters.getChar(j);
			}

			table[0] = theletters.getChar(j - 1); // don't need this

			// index in "table" of the next char to emit
			lookupIndex = (int)val / groups.getInt(count);

			// shift input by one "column"
			val = val % groups.getInt(count);

			// this should not happen
			if (lookupIndex == 0 && val == 0)
			{
			  break;
			}

			if (lookupIndex < table.Length)
			{

			  // put out the next character of output       
			  buf[charPos++] = table[lookupIndex]; // left to right or top to bottom
			}
			else
			{
			  return XSLTErrorResources.ERROR_STRING;
			}

			count++;
		  }
		} // end while

		// String s = new String(buf, 0, charPos);
		return new string(buf, 0, charPos);
	  }

	  /// <summary>
	  /// Convert a long integer into roman numerals. </summary>
	  /// <param name="val"> Value to convert. </param>
	  /// <param name="prefixesAreOK"> true_ to enable prefix notation (e.g. 4 = "IV"),
	  /// false_ to disable prefix notation (e.g. 4 = "IIII"). </param>
	  /// <returns> Roman numeral string. </returns>
	  /// <seealso cref= DecimalToRoman </seealso>
	  /// <seealso cref= m_romanConvertTable </seealso>
	  protected internal virtual string long2roman(long val, bool prefixesAreOK)
	  {

		if (val <= 0)
		{
		  return ZeroString;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String roman;
		string roman;
		int place = 0;

		if (val <= 3999L)
		{
		  StringBuilder romanBuffer = new StringBuilder();
		  do
		  {
			while (val >= m_romanConvertTable[place].m_postValue)
			{
			  romanBuffer.Append(m_romanConvertTable[place].m_postLetter);
			  val -= m_romanConvertTable[place].m_postValue;
			}

			if (prefixesAreOK)
			{
			  if (val >= m_romanConvertTable[place].m_preValue)
			  {
				romanBuffer.Append(m_romanConvertTable[place].m_preLetter);
				val -= m_romanConvertTable[place].m_preValue;
			  }
			}

			place++;
		  } while (val > 0);
		  roman = romanBuffer.ToString();
		}
		else
		{
		  roman = XSLTErrorResources.ERROR_STRING;
		}

		return roman;
	  } // end long2roman

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  public override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
	  {
		  if (callAttrs)
		  {
			  if (null != m_countMatchPattern)
			  {
				  m_countMatchPattern.Expression.callVisitors(m_countMatchPattern, visitor);
			  }
			  if (null != m_fromMatchPattern)
			  {
				  m_fromMatchPattern.Expression.callVisitors(m_fromMatchPattern, visitor);
			  }
			  if (null != m_valueExpr)
			  {
				  m_valueExpr.Expression.callVisitors(m_valueExpr, visitor);
			  }

			  if (null != m_format_avt)
			  {
				  m_format_avt.callVisitors(visitor);
			  }
			  if (null != m_groupingSeparator_avt)
			  {
				  m_groupingSeparator_avt.callVisitors(visitor);
			  }
			  if (null != m_groupingSize_avt)
			  {
				  m_groupingSize_avt.callVisitors(visitor);
			  }
			  if (null != m_lang_avt)
			  {
				  m_lang_avt.callVisitors(visitor);
			  }
			  if (null != m_lettervalue_avt)
			  {
				  m_lettervalue_avt.callVisitors(visitor);
			  }
		  }

		base.callChildVisitors(visitor, callAttrs);
	  }


	  /// <summary>
	  /// This class returns tokens using non-alphanumberic
	  /// characters as delimiters.
	  /// </summary>
	  internal class NumberFormatStringTokenizer
	  {
		  private readonly ElemNumber outerInstance;


		/// <summary>
		/// Current position in the format string </summary>
		internal int currentPosition;

		/// <summary>
		/// Index of last character in the format string </summary>
		internal int maxPosition;

		/// <summary>
		/// Format string to be tokenized </summary>
		internal string str;

		/// <summary>
		/// Construct a NumberFormatStringTokenizer.
		/// </summary>
		/// <param name="str"> Format string to be tokenized </param>
		public NumberFormatStringTokenizer(ElemNumber outerInstance, string str)
		{
			this.outerInstance = outerInstance;
		  this.str = str;
		  maxPosition = str.Length;
		}

		/// <summary>
		/// Reset tokenizer so that nextToken() starts from the beginning.
		/// </summary>
		public virtual void reset()
		{
		  currentPosition = 0;
		}

		/// <summary>
		/// Returns the next token from this string tokenizer.
		/// </summary>
		/// <returns>     the next token from this string tokenizer. </returns>
		/// <exception cref="NoSuchElementException">  if there are no more tokens in this
		///               tokenizer's string. </exception>
		public virtual string nextToken()
		{

		  if (currentPosition >= maxPosition)
		  {
			throw new NoSuchElementException();
		  }

		  int start = currentPosition;

		  while ((currentPosition < maxPosition) && char.IsLetterOrDigit(str[currentPosition]))
		  {
			currentPosition++;
		  }

		  if ((start == currentPosition) && (!char.IsLetterOrDigit(str[currentPosition])))
		  {
			currentPosition++;
		  }

		  return str.Substring(start, currentPosition - start);
		}

		/// <summary>
		/// Tells if there is a digit or a letter character ahead.
		/// </summary>
		/// <returns>     true if there is a number or character ahead. </returns>
		public virtual bool LetterOrDigitAhead
		{
			get
			{
    
			  int pos = currentPosition;
    
			  while (pos < maxPosition)
			  {
				if (char.IsLetterOrDigit(str[pos]))
				{
				  return true;
				}
    
				pos++;
			  }
    
			  return false;
			}
		}

		/// <summary>
		/// Tells if there is a digit or a letter character ahead.
		/// </summary>
		/// <returns>     true if there is a number or character ahead. </returns>
		public virtual bool nextIsSep()
		{

		  if (char.IsLetterOrDigit(str[currentPosition]))
		  {
			return false;
		  }
		  else
		  {
			return true;
		  }
		}

		/// <summary>
		/// Tells if <code>nextToken</code> will throw an exception
		/// if it is called.
		/// </summary>
		/// <returns> true if <code>nextToken</code> can be called
		/// without throwing an exception. </returns>
		public virtual bool hasMoreTokens()
		{
		  return (currentPosition >= maxPosition) ? false : true;
		}

		/// <summary>
		/// Calculates the number of times that this tokenizer's
		/// <code>nextToken</code> method can be called before it generates an
		/// exception.
		/// </summary>
		/// <returns>  the number of tokens remaining in the string using the current
		///          delimiter set. </returns>
		/// <seealso cref=     java.util.StringTokenizer#nextToken() </seealso>
		public virtual int countTokens()
		{

		  int count = 0;
		  int currpos = currentPosition;

		  while (currpos < maxPosition)
		  {
			int start = currpos;

			while ((currpos < maxPosition) && char.IsLetterOrDigit(str[currpos]))
			{
			  currpos++;
			}

			if ((start == currpos) && (char.IsLetterOrDigit(str[currpos]) == false))
			{
			  currpos++;
			}

			count++;
		  }

		  return count;
		}
	  } // end NumberFormatStringTokenizer



	}

}