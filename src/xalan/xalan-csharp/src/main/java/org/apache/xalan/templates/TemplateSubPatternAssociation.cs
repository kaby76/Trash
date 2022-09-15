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
 * $Id: TemplateSubPatternAssociation.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using StepPattern = org.apache.xpath.patterns.StepPattern;

	/// <summary>
	/// A class to contain a match pattern and it's corresponding template.
	/// This class also defines a node in a match pattern linked list.
	/// </summary>
	[Serializable]
	internal class TemplateSubPatternAssociation : ICloneable
	{
		internal const long serialVersionUID = -8902606755229903350L;

	  /// <summary>
	  /// Step pattern </summary>
	  internal StepPattern m_stepPattern;

	  /// <summary>
	  /// Template pattern </summary>
	  private string m_pattern;

	  /// <summary>
	  /// The template element </summary>
	  private ElemTemplate m_template;

	  /// <summary>
	  /// Next pattern </summary>
	  private TemplateSubPatternAssociation m_next = null;

	  /// <summary>
	  /// Flag indicating whether this is wild card pattern </summary>
	  private bool m_wild;

	  /// <summary>
	  /// Target string for this match pattern </summary>
	  private string m_targetString;

	  /// <summary>
	  /// Construct a match pattern from a pattern and template. </summary>
	  /// <param name="template"> The node that contains the template for this pattern. </param>
	  /// <param name="pattern"> An executable XSLT StepPattern. </param>
	  /// <param name="pat"> For now a Nodelist that contains old-style element patterns. </param>
	  internal TemplateSubPatternAssociation(ElemTemplate template, StepPattern pattern, string pat)
	  {

		m_pattern = pat;
		m_template = template;
		m_stepPattern = pattern;
		m_targetString = m_stepPattern.TargetString;
		m_wild = m_targetString.Equals("*");
	  }

	  /// <summary>
	  /// Clone this object.
	  /// </summary>
	  /// <returns> The cloned object.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public virtual object clone()
	  {

		TemplateSubPatternAssociation tspa = (TemplateSubPatternAssociation) base.clone();

		tspa.m_next = null;

		return tspa;
	  }

	  /// <summary>
	  /// Get the target string of the pattern.  For instance, if the pattern is
	  /// "foo/baz/boo[@daba]", this string will be "boo".
	  /// </summary>
	  /// <returns> The "target" string. </returns>
	  public string TargetString
	  {
		  get
		  {
			return m_targetString;
		  }
		  set
		  {
			m_targetString = value;
		  }
	  }


	  /// <summary>
	  /// Tell if two modes match according to the rules of XSLT.
	  /// </summary>
	  /// <param name="m1"> mode to match
	  /// </param>
	  /// <returns> True if the given mode matches this template's mode </returns>
	  internal virtual bool matchMode(QName m1)
	  {
		return matchModes(m1, m_template.Mode);
	  }

	  /// <summary>
	  /// Tell if two modes match according to the rules of XSLT.
	  /// </summary>
	  /// <param name="m1"> First mode to match </param>
	  /// <param name="m2"> Second mode to match
	  /// </param>
	  /// <returns> True if the two given modes match </returns>
	  private bool matchModes(QName m1, QName m2)
	  {
		return (((null == m1) && (null == m2)) || ((null != m1) && (null != m2) && m1.Equals(m2)));
	  }

	  /// <summary>
	  /// Return the mode associated with the template.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> XPath context to use with this template </param>
	  /// <param name="targetNode"> Target node </param>
	  /// <param name="mode"> reference, which may be null, to the <a href="http://www.w3.org/TR/xslt#modes">current mode</a>. </param>
	  /// <returns> The mode associated with the template.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean matches(org.apache.xpath.XPathContext xctxt, int targetNode, org.apache.xml.utils.QName mode) throws javax.xml.transform.TransformerException
	  public virtual bool matches(XPathContext xctxt, int targetNode, QName mode)
	  {

		double score = m_stepPattern.getMatchScore(xctxt, targetNode);

		return (XPath.MATCH_SCORE_NONE != score) && matchModes(mode, m_template.Mode);
	  }

	  /// <summary>
	  /// Tell if the pattern for this association is a wildcard.
	  /// </summary>
	  /// <returns> true if this pattern is considered to be a wild match. </returns>
	  public bool Wild
	  {
		  get
		  {
			return m_wild;
		  }
	  }

	  /// <summary>
	  /// Get associated XSLT StepPattern.
	  /// </summary>
	  /// <returns> An executable StepPattern object, never null.
	  ///  </returns>
	  public StepPattern StepPattern
	  {
		  get
		  {
			return m_stepPattern;
		  }
	  }

	  /// <summary>
	  /// Get the pattern string for diagnostic purposes.
	  /// </summary>
	  /// <returns> The pattern string for diagnostic purposes.
	  ///  </returns>
	  public string Pattern
	  {
		  get
		  {
			return m_pattern;
		  }
	  }

	  /// <summary>
	  /// Return the position of the template in document
	  /// order in the stylesheet.
	  /// </summary>
	  /// <returns> The position of the template in the overall template order. </returns>
	  public virtual int DocOrderPos
	  {
		  get
		  {
			return m_template.Uid;
		  }
	  }

	  /// <summary>
	  /// Return the import level associated with the stylesheet into which  
	  /// this template is composed.
	  /// </summary>
	  /// <returns> The import level of this template. </returns>
	  public int ImportLevel
	  {
		  get
		  {
			return m_template.StylesheetComposed.ImportCountComposed;
		  }
	  }

	  /// <summary>
	  /// Get the assocated xsl:template.
	  /// </summary>
	  /// <returns> An ElemTemplate, never null.
	  ///  </returns>
	  public ElemTemplate Template
	  {
		  get
		  {
			return m_template;
		  }
	  }

	  /// <summary>
	  /// Get the next association.
	  /// </summary>
	  /// <returns> A valid TemplateSubPatternAssociation, or null. </returns>
	  public TemplateSubPatternAssociation Next
	  {
		  get
		  {
			return m_next;
		  }
		  set
		  {
			m_next = value;
		  }
	  }

	}

}