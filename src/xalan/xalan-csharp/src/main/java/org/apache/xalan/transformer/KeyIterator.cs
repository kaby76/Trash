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
 * $Id: KeyIterator.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using KeyDeclaration = org.apache.xalan.templates.KeyDeclaration;
	using Axis = org.apache.xml.dtm.Axis;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;
	using OneStepIteratorForward = org.apache.xpath.axes.OneStepIteratorForward;

	/// <summary>
	/// This class implements an optimized iterator for 
	/// "key()" patterns, matching each node to the 
	/// match attribute in one or more xsl:key declarations.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class KeyIterator : OneStepIteratorForward
	{
		internal new const long serialVersionUID = -1349109910100249661L;

	  /// <summary>
	  /// Key name.
	  ///  @serial           
	  /// </summary>
	  private new QName m_name;

	  /// <summary>
	  /// Get the key name from a key declaration this iterator will process
	  /// 
	  /// </summary>
	  /// <returns> Key name </returns>
	  public virtual QName Name
	  {
		  get
		  {
			return m_name;
		  }
	  }

	  /// <summary>
	  /// Vector of Key declarations in the stylesheet.
	  ///  @serial          
	  /// </summary>
	  private ArrayList m_keyDeclarations;

	  /// <summary>
	  /// Get the key declarations from the stylesheet 
	  /// 
	  /// </summary>
	  /// <returns> Vector containing the key declarations from the stylesheet </returns>
	  public virtual ArrayList KeyDeclarations
	  {
		  get
		  {
			return m_keyDeclarations;
		  }
	  }

	  /// <summary>
	  /// Create a KeyIterator object.
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  internal KeyIterator(QName name, ArrayList keyDeclarations) : base(Axis.ALL)
	  {
		m_keyDeclarations = keyDeclarations;
		// m_prefixResolver = nscontext;
		m_name = name;
	  }

	  /// <summary>
	  ///  Test whether a specified node is visible in the logical view of a
	  /// TreeWalker or NodeIterator. This function will be called by the
	  /// implementation of TreeWalker and NodeIterator; it is not intended to
	  /// be called directly from user code.
	  /// </summary>
	  /// <param name="testNode">  The node to check to see if it passes the filter or not.
	  /// </param>
	  /// <returns>  a constant to determine whether the node is accepted,
	  ///   rejected, or skipped, as defined  above . </returns>
	  public override short acceptNode(int testNode)
	  {
		bool foundKey = false;
		KeyIterator ki = (KeyIterator) m_lpi;
		org.apache.xpath.XPathContext xctxt = ki.XPathContext;
		ArrayList keys = ki.KeyDeclarations;

		QName name = ki.Name;
		try
		{
		  // System.out.println("lookupKey: "+lookupKey);
		  int nDeclarations = keys.Count;

		  // Walk through each of the declarations made with xsl:key
		  for (int i = 0; i < nDeclarations; i++)
		  {
			KeyDeclaration kd = (KeyDeclaration) keys[i];

			// Only continue if the name on this key declaration
			// matches the name on the iterator for this walker. 
			if (!kd.Name.Equals(name))
			{
			  continue;
			}

			foundKey = true;
			// xctxt.setNamespaceContext(ki.getPrefixResolver());

			// See if our node matches the given key declaration according to 
			// the match attribute on xsl:key.
			XPath matchExpr = kd.Match;
			double score = matchExpr.getMatchScore(xctxt, testNode);

			if (score == kd.Match.MATCH_SCORE_NONE)
			{
			  continue;
			}

			return DTMIterator.FILTER_ACCEPT;

		  } // end for(int i = 0; i < nDeclarations; i++)
		}
		catch (TransformerException)
		{

		  // TODO: What to do?
		}

		if (!foundKey)
		{
		  throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_NO_XSLKEY_DECLARATION, new object[] {name.LocalName}));
		}

		return DTMIterator.FILTER_REJECT;
	  }

	}

}