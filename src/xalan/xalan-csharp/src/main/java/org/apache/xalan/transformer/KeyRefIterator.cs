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
 * $Id: KeyRefIterator.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using KeyDeclaration = org.apache.xalan.templates.KeyDeclaration;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using QName = org.apache.xml.utils.QName;
	using XMLString = org.apache.xml.utils.XMLString;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class filters nodes from a key iterator, according to 
	/// whether or not the use value matches the ref value.  
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class KeyRefIterator : org.apache.xpath.axes.ChildTestIterator
	{
		internal new const long serialVersionUID = 3837456451659435102L;
	  /// <summary>
	  /// Constructor KeyRefIterator
	  /// 
	  /// </summary>
	  /// <param name="ref"> Key value to match </param>
	  /// <param name="ki"> The main key iterator used to walk the source tree  </param>
	  public KeyRefIterator(QName name, XMLString @ref, ArrayList keyDecls, DTMIterator ki) : base(null)
	  {
		m_name = name;
		m_ref = @ref;
		m_keyDeclarations = keyDecls;
		m_keysNodes = ki;
		WhatToShow = org.apache.xml.dtm.DTMFilter.SHOW_ALL;
	  }

	  internal DTMIterator m_keysNodes;

	  /// <summary>
	  /// Get the next node via getNextXXX.  Bottlenecked for derived class override. </summary>
	  /// <returns> The next node on the axis, or DTM.NULL. </returns>
	  protected internal override int NextNode
	  {
		  get
		  {
			  int next;
			while (DTM.NULL != (next = m_keysNodes.nextNode()))
			{
				if (DTMIterator.FILTER_ACCEPT == filterNode(next))
				{
					break;
				}
			}
			m_lastFetched = next;
    
			return next;
		  }
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
	  public virtual short filterNode(int testNode)
	  {
		bool foundKey = false;
		ArrayList keys = m_keyDeclarations;

		QName name = m_name;
		KeyIterator ki = (KeyIterator)(((XNodeSet)m_keysNodes).ContainedIter);
		org.apache.xpath.XPathContext xctxt = ki.XPathContext;

		if (null == xctxt)
		{
			assertion(false, "xctxt can not be null here!");
		}

		try
		{
		  XMLString lookupKey = m_ref;

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

			// Query from the node, according the the select pattern in the
			// use attribute in xsl:key.
			XObject xuse = kd.Use.execute(xctxt, testNode, ki.PrefixResolver);

			if (xuse.Type != XObject.CLASS_NODESET)
			{
			  XMLString exprResult = xuse.xstr();

			  if (lookupKey.Equals(exprResult))
			  {
				return DTMIterator.FILTER_ACCEPT;
			  }
			}
			else
			{
			  DTMIterator nl = ((XNodeSet)xuse).iterRaw();
			  int useNode;

			  while (DTM.NULL != (useNode = nl.nextNode()))
			  {
				DTM dtm = getDTM(useNode);
				XMLString exprResult = dtm.getStringValue(useNode);
				if ((null != exprResult) && lookupKey.Equals(exprResult))
				{
				  return DTMIterator.FILTER_ACCEPT;
				}
			  }
			}

		  } // end for(int i = 0; i < nDeclarations; i++)
		}
		catch (javax.xml.transform.TransformerException te)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(te);
		}

		if (!foundKey)
		{
		  throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_NO_XSLKEY_DECLARATION, new object[] {name.LocalName}));
		}
		return DTMIterator.FILTER_REJECT;
	  }

	  protected internal XMLString m_ref;
	  protected internal new QName m_name;

	  /// <summary>
	  /// Vector of Key declarations in the stylesheet.
	  ///  @serial          
	  /// </summary>
	  protected internal ArrayList m_keyDeclarations;

	}

}