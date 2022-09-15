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
 * $Id: FuncId.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using StringVector = org.apache.xml.utils.StringVector;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using XPathContext = org.apache.xpath.XPathContext;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Execute the Id() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncId : FunctionOneArg
	{
		internal new const long serialVersionUID = 8930573966143567310L;

	  /// <summary>
	  /// Fill in a list with nodes that match a space delimited list if ID 
	  /// ID references.
	  /// </summary>
	  /// <param name="xctxt"> The runtime XPath context. </param>
	  /// <param name="docContext"> The document where the nodes are being looked for. </param>
	  /// <param name="refval"> A space delimited list of ID references. </param>
	  /// <param name="usedrefs"> List of references for which nodes were found. </param>
	  /// <param name="nodeSet"> Node set where the nodes will be added to. </param>
	  /// <param name="mayBeMore"> true if there is another set of nodes to be looked for.
	  /// </param>
	  /// <returns> The usedrefs value. </returns>
	  private StringVector getNodesByID(XPathContext xctxt, int docContext, string refval, StringVector usedrefs, NodeSetDTM nodeSet, bool mayBeMore)
	  {

		if (null != refval)
		{
		  string @ref = null;
	//      DOMHelper dh = xctxt.getDOMHelper();
		  StringTokenizer tokenizer = new StringTokenizer(refval);
		  bool hasMore = tokenizer.hasMoreTokens();
		  DTM dtm = xctxt.getDTM(docContext);

		  while (hasMore)
		  {
			@ref = tokenizer.nextToken();
			hasMore = tokenizer.hasMoreTokens();

			if ((null != usedrefs) && usedrefs.contains(@ref))
			{
			  @ref = null;

			  continue;
			}

			int node = dtm.getElementById(@ref);

			if (DTM.NULL != node)
			{
			  nodeSet.addNodeInDocOrder(node, xctxt);
			}

			if ((null != @ref) && (hasMore || mayBeMore))
			{
			  if (null == usedrefs)
			  {
				usedrefs = new StringVector();
			  }

			  usedrefs.addElement(@ref);
			}
		  }
		}

		return usedrefs;
	  }

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		int context = xctxt.CurrentNode;
		DTM dtm = xctxt.getDTM(context);
		int docContext = dtm.Document;

		if (DTM.NULL == docContext)
		{
		  error(xctxt, XPATHErrorResources.ER_CONTEXT_HAS_NO_OWNERDOC, null);
		}

		XObject arg = m_arg0.execute(xctxt);
		int argType = arg.Type;
		XNodeSet nodes = new XNodeSet(xctxt.DTMManager);
		NodeSetDTM nodeSet = nodes.mutableNodeset();

		if (XObject.CLASS_NODESET == argType)
		{
		  DTMIterator ni = arg.iter();
		  StringVector usedrefs = null;
		  int pos = ni.nextNode();

		  while (DTM.NULL != pos)
		  {
			DTM ndtm = ni.getDTM(pos);
			string refval = ndtm.getStringValue(pos).ToString();

			pos = ni.nextNode();
			usedrefs = getNodesByID(xctxt, docContext, refval, usedrefs, nodeSet, DTM.NULL != pos);
		  }
		  // ni.detach();
		}
		else if (XObject.CLASS_NULL == argType)
		{
		  return nodes;
		}
		else
		{
		  string refval = arg.str();

		  getNodesByID(xctxt, docContext, refval, null, nodeSet, false);
		}

		return nodes;
	  }
	}

}