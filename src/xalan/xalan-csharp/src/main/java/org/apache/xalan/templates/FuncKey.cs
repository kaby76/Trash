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
 * $Id: FuncKey.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using KeyManager = org.apache.xalan.transformer.KeyManager;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using QName = org.apache.xml.utils.QName;
	using XMLString = org.apache.xml.utils.XMLString;
	using XPathContext = org.apache.xpath.XPathContext;
	using UnionPathIterator = org.apache.xpath.axes.UnionPathIterator;
	using Function2Args = org.apache.xpath.functions.Function2Args;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Execute the Key() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncKey : Function2Args
	{
		internal new const long serialVersionUID = 9089293100115347340L;

	  /// <summary>
	  /// Dummy value to be used in usedrefs hashtable </summary>
	  private static bool? ISTRUE = new bool?(true);

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

		// TransformerImpl transformer = (TransformerImpl)xctxt;
		TransformerImpl transformer = (TransformerImpl) xctxt.OwnerObject;
		XNodeSet nodes = null;
		int context = xctxt.CurrentNode;
		DTM dtm = xctxt.getDTM(context);
		int docContext = dtm.getDocumentRoot(context);

		if (DTM.NULL == docContext)
		{

		  // path.error(context, XPATHErrorResources.ER_CONTEXT_HAS_NO_OWNERDOC); //"context does not have an owner document!");
		}

		string xkeyname = Arg0.execute(xctxt).str();
		QName keyname = new QName(xkeyname, xctxt.NamespaceContext);
		XObject arg = Arg1.execute(xctxt);
		bool argIsNodeSetDTM = (XObject.CLASS_NODESET == arg.Type);
		KeyManager kmgr = transformer.KeyManager;

		// Don't bother with nodeset logic if the thing is only one node.
		if (argIsNodeSetDTM)
		{
			XNodeSet ns = (XNodeSet)arg;
			ns.ShouldCacheNodes = true;
			int len = ns.Length;
			if (len <= 1)
			{
				argIsNodeSetDTM = false;
			}
		}

		if (argIsNodeSetDTM)
		{
		  Hashtable usedrefs = null;
		  DTMIterator ni = arg.iter();
		  int pos;
		  UnionPathIterator upi = new UnionPathIterator();
		  upi.exprSetParent(this);

		  while (DTM.NULL != (pos = ni.nextNode()))
		  {
			dtm = xctxt.getDTM(pos);
			XMLString @ref = dtm.getStringValue(pos);

			if (null == @ref)
			{
			  continue;
			}

			if (null == usedrefs)
			{
			  usedrefs = new Hashtable();
			}

			if (usedrefs[@ref] != null)
			{
			  continue; // We already have 'em.
			}
			else
			{

			  // ISTRUE being used as a dummy value.
			  usedrefs[@ref] = ISTRUE;
			}

			XNodeSet nl = kmgr.getNodeSetDTMByKey(xctxt, docContext, keyname, @ref, xctxt.NamespaceContext);

			nl.setRoot(xctxt.CurrentNode, xctxt);

	//        try
	//        {
			  upi.addIterator(nl);
	//        }
	//        catch(CloneNotSupportedException cnse)
	//        {
	//          // will never happen.
	//        }
			//mnodeset.addNodesInDocOrder(nl, xctxt); needed??
		  }

		  int current = xctxt.CurrentNode;
		  upi.setRoot(current, xctxt);

		  nodes = new XNodeSet(upi);
		}
		else
		{
		  XMLString @ref = arg.xstr();
		  nodes = kmgr.getNodeSetDTMByKey(xctxt, docContext, keyname, @ref, xctxt.NamespaceContext);
		  nodes.setRoot(xctxt.CurrentNode, xctxt);
		}

		return nodes;
	  }
	}

}