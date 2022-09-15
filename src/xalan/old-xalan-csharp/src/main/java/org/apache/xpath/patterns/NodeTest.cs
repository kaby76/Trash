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
 * $Id: NodeTest.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.patterns
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This is the basic node test class for both match patterns and location path
	/// steps.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class NodeTest : Expression
	{
		internal new const long serialVersionUID = -5736721866747906182L;

	  /// <summary>
	  /// The namespace or local name for node tests with a wildcard. </summary>
	  ///  <seealso cref= <a href="http://www.w3.org/TR/xpath#NT-NameTest">the XPath NameTest production.</a>  </seealso>
	  public const string WILD = "*";

	  /// <summary>
	  /// The URL to pass to the Node#supports method, to see if the
	  /// DOM has already been stripped of whitespace nodes. 
	  /// </summary>
	  public const string SUPPORTS_PRE_STRIPPING = "http://xml.apache.org/xpath/features/whitespace-pre-stripping";

	  /// <summary>
	  /// This attribute determines which node types are accepted.
	  /// @serial
	  /// </summary>
	  protected internal int m_whatToShow;

	  /// <summary>
	  /// Special bitmap for match patterns starting with a function.
	  /// Make sure this does not conflict with <seealso cref="org.w3c.dom.traversal.NodeFilter"/>.
	  /// </summary>
	  public const int SHOW_BYFUNCTION = 0x00010000;

	  /// <summary>
	  /// This attribute determines which node types are accepted.
	  /// These constants are defined in the <seealso cref="org.w3c.dom.traversal.NodeFilter"/>
	  /// interface.
	  /// </summary>
	  /// <returns> bitset mainly defined in <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </returns>
	  public virtual int WhatToShow
	  {
		  get
		  {
			return m_whatToShow;
		  }
		  set
		  {
			m_whatToShow = value;
		  }
	  }


	  /// <summary>
	  /// The namespace to be tested for, which may be null.
	  ///  @serial 
	  /// </summary>
	  internal string m_namespace;

	  /// <summary>
	  /// Return the namespace to be tested.
	  /// </summary>
	  /// <returns> The namespace to be tested for, or <seealso cref="#WILD"/>, or null. </returns>
	  public virtual string Namespace
	  {
		  get
		  {
			return m_namespace;
		  }
		  set
		  {
			m_namespace = value;
		  }
	  }


	  /// <summary>
	  /// The local name to be tested for.
	  ///  @serial 
	  /// </summary>
	  protected internal string m_name;

	  /// <summary>
	  /// Return the local name to be tested.
	  /// </summary>
	  /// <returns> the local name to be tested, or <seealso cref="#WILD"/>, or an empty string. </returns>
	  public virtual string LocalName
	  {
		  get
		  {
			return (null == m_name) ? "" : m_name;
		  }
		  set
		  {
			m_name = value;
		  }
	  }


	  /// <summary>
	  /// Statically calculated score for this test.  One of
	  ///  <seealso cref="#SCORE_NODETEST"/>,
	  ///  <seealso cref="#SCORE_NONE"/>,
	  ///  <seealso cref="#SCORE_NSWILD"/>,
	  ///  <seealso cref="#SCORE_QNAME"/>, or
	  ///  <seealso cref="#SCORE_OTHER"/>.
	  ///  @serial
	  /// </summary>
	  internal XNumber m_score;

	  /// <summary>
	  /// The match score if the pattern consists of just a NodeTest. </summary>
	  ///  <seealso cref= <a href="http://www.w3.org/TR/xslt#conflict">XSLT Specification - 5.5 Conflict Resolution for Template Rules</a>  </seealso>
	  public static readonly XNumber SCORE_NODETEST = new XNumber(XPath.MATCH_SCORE_NODETEST);

	  /// <summary>
	  /// The match score if the pattern pattern has the form NCName:*. </summary>
	  ///  <seealso cref= <a href="http://www.w3.org/TR/xslt#conflict">XSLT Specification - 5.5 Conflict Resolution for Template Rules</a>  </seealso>
	  public static readonly XNumber SCORE_NSWILD = new XNumber(XPath.MATCH_SCORE_NSWILD);

	  /// <summary>
	  /// The match score if the pattern has the form
	  /// of a QName optionally preceded by an @ character. </summary>
	  ///  <seealso cref= <a href="http://www.w3.org/TR/xslt#conflict">XSLT Specification - 5.5 Conflict Resolution for Template Rules</a>  </seealso>
	  public static readonly XNumber SCORE_QNAME = new XNumber(XPath.MATCH_SCORE_QNAME);

	  /// <summary>
	  /// The match score if the pattern consists of something
	  /// other than just a NodeTest or just a qname. </summary>
	  ///  <seealso cref= <a href="http://www.w3.org/TR/xslt#conflict">XSLT Specification - 5.5 Conflict Resolution for Template Rules</a>  </seealso>
	  public static readonly XNumber SCORE_OTHER = new XNumber(XPath.MATCH_SCORE_OTHER);

	  /// <summary>
	  /// The match score if no match is made. </summary>
	  ///  <seealso cref= <a href="http://www.w3.org/TR/xslt#conflict">XSLT Specification - 5.5 Conflict Resolution for Template Rules</a>  </seealso>
	  public static readonly XNumber SCORE_NONE = new XNumber(XPath.MATCH_SCORE_NONE);

	  /// <summary>
	  /// Construct an NodeTest that tests for namespaces and node names.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </param>
	  /// <param name="namespace"> The namespace to be tested. </param>
	  /// <param name="name"> The local name to be tested. </param>
	  public NodeTest(int whatToShow, string @namespace, string name)
	  {
		initNodeTest(whatToShow, @namespace, name);
	  }

	  /// <summary>
	  /// Construct an NodeTest that doesn't test for node names.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </param>
	  public NodeTest(int whatToShow)
	  {
		initNodeTest(whatToShow);
	  }

	  /// <seealso cref= Expression#deepEquals(Expression) </seealso>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  NodeTest nt = (NodeTest)expr;

		  if (null != nt.m_name)
		  {
			  if (null == m_name)
			  {
				  return false;
			  }
			  else if (!nt.m_name.Equals(m_name))
			  {
				  return false;
			  }
		  }
		  else if (null != m_name)
		  {
			  return false;
		  }

		  if (null != nt.m_namespace)
		  {
			  if (null == m_namespace)
			  {
				  return false;
			  }
			  else if (!nt.m_namespace.Equals(m_namespace))
			  {
				  return false;
			  }
		  }
		  else if (null != m_namespace)
		  {
			  return false;
		  }

		  if (m_whatToShow != nt.m_whatToShow)
		  {
			  return false;
		  }

		  if (m_isTotallyWild != nt.m_isTotallyWild)
		  {
			  return false;
		  }

		return true;
	  }

	  /// <summary>
	  /// Null argument constructor.
	  /// </summary>
	  public NodeTest()
	  {
	  }

	  /// <summary>
	  /// Initialize this node test by setting the whatToShow property, and
	  /// calculating the score that this test will return if a test succeeds.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </param>
	  public virtual void initNodeTest(int whatToShow)
	  {

		m_whatToShow = whatToShow;

		calcScore();
	  }

	  /// <summary>
	  /// Initialize this node test by setting the whatToShow property and the
	  /// namespace and local name, and
	  /// calculating the score that this test will return if a test succeeds.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by <seealso cref="org.w3c.dom.traversal.NodeFilter"/>. </param>
	  /// <param name="namespace"> The namespace to be tested. </param>
	  /// <param name="name"> The local name to be tested. </param>
	  public virtual void initNodeTest(int whatToShow, string @namespace, string name)
	  {

		m_whatToShow = whatToShow;
		m_namespace = @namespace;
		m_name = name;

		calcScore();
	  }

	  /// <summary>
	  /// True if this test has a null namespace and a local name of <seealso cref="#WILD"/>.
	  ///  @serial 
	  /// </summary>
	  private bool m_isTotallyWild;

	  /// <summary>
	  /// Get the static score for this node test. </summary>
	  /// <returns> Should be one of the SCORE_XXX constants. </returns>
	  public virtual XNumber StaticScore
	  {
		  get
		  {
			return m_score;
		  }
		  set
		  {
			m_score = value;
		  }
	  }


	  /// <summary>
	  /// Static calc of match score.
	  /// </summary>
	  protected internal virtual void calcScore()
	  {

		if ((string.ReferenceEquals(m_namespace, null)) && (string.ReferenceEquals(m_name, null)))
		{
		  m_score = SCORE_NODETEST;
		}
		else if (((string.ReferenceEquals(m_namespace, WILD)) || (string.ReferenceEquals(m_namespace, null))) && (string.ReferenceEquals(m_name, WILD)))
		{
		  m_score = SCORE_NODETEST;
		}
		else if ((!string.ReferenceEquals(m_namespace, WILD)) && (string.ReferenceEquals(m_name, WILD)))
		{
		  m_score = SCORE_NSWILD;
		}
		else
		{
		  m_score = SCORE_QNAME;
		}

		m_isTotallyWild = (string.ReferenceEquals(m_namespace, null) && string.ReferenceEquals(m_name, WILD));
	  }

	  /// <summary>
	  /// Get the score that this test will return if a test succeeds.
	  /// 
	  /// </summary>
	  /// <returns> the score that this test will return if a test succeeds. </returns>
	  public virtual double DefaultScore
	  {
		  get
		  {
			return m_score.num();
		  }
	  }

	  /// <summary>
	  /// Tell what node type to test, if not DTMFilter.SHOW_ALL.
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by 
	  ///        <seealso cref="org.apache.xml.dtm.DTMFilter"/>. </param>
	  /// <returns> the node type for the whatToShow.  Since whatToShow can specify 
	  ///         multiple types, it will return the first bit tested that is on, 
	  ///         so the caller of this function should take care that this is 
	  ///         the function they really want to call.  If none of the known bits
	  ///         are set, this function will return zero. </returns>
	  public static int getNodeTypeTest(int whatToShow)
	  {
		// %REVIEW% Is there a better way?
		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ELEMENT))
		{
		  return org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ATTRIBUTE))
		{
		  return org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_TEXT))
		{
		  return org.apache.xml.dtm.DTM_Fields.TEXT_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT))
		{
		  return org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT_FRAGMENT))
		{
		  return org.apache.xml.dtm.DTM_Fields.DOCUMENT_FRAGMENT_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_NAMESPACE))
		{
		  return org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_COMMENT))
		{
		  return org.apache.xml.dtm.DTM_Fields.COMMENT_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_PROCESSING_INSTRUCTION))
		{
		  return org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT_TYPE))
		{
		  return org.apache.xml.dtm.DTM_Fields.DOCUMENT_TYPE_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ENTITY))
		{
		  return org.apache.xml.dtm.DTM_Fields.ENTITY_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ENTITY_REFERENCE))
		{
		  return org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_NOTATION))
		{
		  return org.apache.xml.dtm.DTM_Fields.NOTATION_NODE;
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_CDATA_SECTION))
		{
		  return org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE;
		}


		return 0;
	  }


	  /// <summary>
	  /// Do a diagnostics dump of a whatToShow bit set.
	  /// 
	  /// </summary>
	  /// <param name="whatToShow"> Bit set defined mainly by 
	  ///        <seealso cref="org.apache.xml.dtm.DTMFilter"/>. </param>
	  public static void debugWhatToShow(int whatToShow)
	  {

		ArrayList v = new ArrayList();

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ATTRIBUTE))
		{
		  v.Add("SHOW_ATTRIBUTE");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_NAMESPACE))
		{
		  v.Add("SHOW_NAMESPACE");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_CDATA_SECTION))
		{
		  v.Add("SHOW_CDATA_SECTION");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_COMMENT))
		{
		  v.Add("SHOW_COMMENT");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT))
		{
		  v.Add("SHOW_DOCUMENT");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT_FRAGMENT))
		{
		  v.Add("SHOW_DOCUMENT_FRAGMENT");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT_TYPE))
		{
		  v.Add("SHOW_DOCUMENT_TYPE");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ELEMENT))
		{
		  v.Add("SHOW_ELEMENT");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ENTITY))
		{
		  v.Add("SHOW_ENTITY");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_ENTITY_REFERENCE))
		{
		  v.Add("SHOW_ENTITY_REFERENCE");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_NOTATION))
		{
		  v.Add("SHOW_NOTATION");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_PROCESSING_INSTRUCTION))
		{
		  v.Add("SHOW_PROCESSING_INSTRUCTION");
		}

		if (0 != (whatToShow & org.apache.xml.dtm.DTMFilter_Fields.SHOW_TEXT))
		{
		  v.Add("SHOW_TEXT");
		}

		int n = v.Count;

		for (int i = 0; i < n; i++)
		{
		  if (i > 0)
		  {
			Console.Write(" | ");
		  }

		  Console.Write(v[i]);
		}

		if (0 == n)
		{
		  Console.Write("empty whatToShow: " + whatToShow);
		}

		Console.WriteLine();
	  }

	  /// <summary>
	  /// Two names are equal if they and either both are null or
	  /// the name t is wild and the name p is non-null, or the two
	  /// strings are equal.
	  /// </summary>
	  /// <param name="p"> part string from the node. </param>
	  /// <param name="t"> target string, which may be <seealso cref="#WILD"/>.
	  /// </param>
	  /// <returns> true if the strings match according to the rules of this method. </returns>
	  private static bool subPartMatch(string p, string t)
	  {

		// boolean b = (p == t) || ((null != p) && ((t == WILD) || p.equals(t)));
		// System.out.println("subPartMatch - p: "+p+", t: "+t+", result: "+b);
		return (string.ReferenceEquals(p, t)) || ((null != p) && ((string.ReferenceEquals(t, WILD)) || p.Equals(t)));
	  }

	  /// <summary>
	  /// This is temporary to patch over Xerces issue with representing DOM
	  /// namespaces as "".
	  /// </summary>
	  /// <param name="p"> part string from the node, which may represent the null namespace
	  ///        as null or as "". </param>
	  /// <param name="t"> target string, which may be <seealso cref="#WILD"/>.
	  /// </param>
	  /// <returns> true if the strings match according to the rules of this method. </returns>
	  private static bool subPartMatchNS(string p, string t)
	  {

		return (string.ReferenceEquals(p, t)) || ((null != p) && ((p.Length > 0) ? ((string.ReferenceEquals(t, WILD)) || p.Equals(t)) : null == t));
	  }

	  /// <summary>
	  /// Tell what the test score is for the given node.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context. </param>
	  /// <param name="context"> The node being tested.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, int context) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, int context)
	  {

		DTM dtm = xctxt.getDTM(context);
		short nodeType = dtm.getNodeType(context);

		if (m_whatToShow == org.apache.xml.dtm.DTMFilter_Fields.SHOW_ALL)
		{
		  return m_score;
		}

		int nodeBit = (m_whatToShow & (0x00000001 << (nodeType - 1)));

		switch (nodeBit)
		{
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT_FRAGMENT :
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT :
		  return SCORE_OTHER;
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_COMMENT :
		  return m_score;
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_CDATA_SECTION :
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_TEXT :

		  // was: 
		  // return (!xctxt.getDOMHelper().shouldStripSourceNode(context))
		  //       ? m_score : SCORE_NONE;
		  return m_score;
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_PROCESSING_INSTRUCTION :
		  return subPartMatch(dtm.getNodeName(context), m_name) ? m_score : SCORE_NONE;

		// From the draft: "Two expanded names are equal if they 
		// have the same local part, and either both have no URI or 
		// both have the same URI."
		// "A node test * is true for any node of the principal node type. 
		// For example, child::* will select all element children of the 
		// context node, and attribute::* will select all attributes of 
		// the context node."
		// "A node test can have the form NCName:*. In this case, the prefix 
		// is expanded in the same way as with a QName using the context 
		// namespace declarations. The node test will be true for any node 
		// of the principal type whose expanded name has the URI to which 
		// the prefix expands, regardless of the local part of the name."
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_NAMESPACE :
		{
		  string ns = dtm.getLocalName(context);

		  return (subPartMatch(ns, m_name)) ? m_score : SCORE_NONE;
		}
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_ATTRIBUTE :
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_ELEMENT :
		{
		  return (m_isTotallyWild || (subPartMatchNS(dtm.getNamespaceURI(context), m_namespace) && subPartMatch(dtm.getLocalName(context), m_name))) ? m_score : SCORE_NONE;
		}
		default :
		  return SCORE_NONE;
		} // end switch(testType)
	  }

	  /// <summary>
	  /// Tell what the test score is for the given node.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context. </param>
	  /// <param name="context"> The node being tested.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, int context, org.apache.xml.dtm.DTM dtm, int expType) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, int context, DTM dtm, int expType)
	  {

		if (m_whatToShow == org.apache.xml.dtm.DTMFilter_Fields.SHOW_ALL)
		{
		  return m_score;
		}

		int nodeBit = (m_whatToShow & (0x00000001 << ((dtm.getNodeType(context)) - 1)));

		switch (nodeBit)
		{
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT_FRAGMENT :
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_DOCUMENT :
		  return SCORE_OTHER;
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_COMMENT :
		  return m_score;
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_CDATA_SECTION :
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_TEXT :

		  // was: 
		  // return (!xctxt.getDOMHelper().shouldStripSourceNode(context))
		  //       ? m_score : SCORE_NONE;
		  return m_score;
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_PROCESSING_INSTRUCTION :
		  return subPartMatch(dtm.getNodeName(context), m_name) ? m_score : SCORE_NONE;

		// From the draft: "Two expanded names are equal if they 
		// have the same local part, and either both have no URI or 
		// both have the same URI."
		// "A node test * is true for any node of the principal node type. 
		// For example, child::* will select all element children of the 
		// context node, and attribute::* will select all attributes of 
		// the context node."
		// "A node test can have the form NCName:*. In this case, the prefix 
		// is expanded in the same way as with a QName using the context 
		// namespace declarations. The node test will be true for any node 
		// of the principal type whose expanded name has the URI to which 
		// the prefix expands, regardless of the local part of the name."
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_NAMESPACE :
		{
		  string ns = dtm.getLocalName(context);

		  return (subPartMatch(ns, m_name)) ? m_score : SCORE_NONE;
		}
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_ATTRIBUTE :
		case org.apache.xml.dtm.DTMFilter_Fields.SHOW_ELEMENT :
		{
		  return (m_isTotallyWild || (subPartMatchNS(dtm.getNamespaceURI(context), m_namespace) && subPartMatch(dtm.getLocalName(context), m_name))) ? m_score : SCORE_NONE;
		}
		default :
		  return SCORE_NONE;
		} // end switch(testType)
	  }

	  /// <summary>
	  /// Test the current node to see if it matches the given node test.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		return execute(xctxt, xctxt.CurrentNode);
	  }

	  /// <summary>
	  /// Node tests by themselves do not need to fix up variables.
	  /// </summary>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		// no-op
	  }

	  /// <seealso cref= org.apache.xpath.XPathVisitable#callVisitors(ExpressionOwner, XPathVisitor) </seealso>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  assertion(false, "callVisitors should not be called for this object!!!");
	  }

	}

}