using System.Collections;
using org.w3c.dom;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standard for Technology in Automotive Retail, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
/// 	   David Carver (STAR) - bug 281168 - initial API and implementation
///     David Carver  - bug 281186 - implementation of fn:id and fn:idref
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AttrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AttrType;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSID = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSID;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Attr = org.w3c.dom.Attr;
	using Element = org.w3c.dom.Element;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// 
	public class FnIDREF : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnInsertBefore.
		/// </summary>
		public FnIDREF() : base(new QName("idref"), 1, 2)
		{
		}

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of evaluation. </returns>
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return idref(args, ec);
		}

		/// <summary>
		/// Insert-Before operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="dc"> </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:insert-before operation. </returns>
		public static ResultSequence idref(ICollection args, EvaluationContext ec)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultBuffer rs = new ResultBuffer();

			IEnumerator argIt = cargs.GetEnumerator();
            argIt.MoveNext();
            ResultSequence idrefRS = (ResultSequence) argIt.Current;
			string[] idst = idrefRS.first().StringValue.Split(" ", true);

			ArrayList ids = createIDs(idst);
			ResultSequence nodeArg = null;
			NodeType nodeType = null;
			if (argIt.MoveNext())
            {
                nodeArg = (ResultSequence) argIt.Current;
				nodeType = (NodeType)nodeArg.first();
			}
			else
			{
				if (ec.ContextItem == null)
				{
					throw DynamicError.contextUndefined();
				}
				if (!(ec.ContextItem is NodeType))
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}
				nodeType = (NodeType) ec.ContextItem;
				if (nodeType.node_value().OwnerDocument == null)
				{
					throw DynamicError.contextUndefined();
				}
			}

			Node node = nodeType.node_value();
			if (node.OwnerDocument == null)
			{
				// W3C test suite seems to want XPDY0002 here
				throw DynamicError.contextUndefined();
				//throw DynamicError.noContextDoc();
			}

			if (hasID(ids, node))
			{
				ElementType element = new ElementType((Element) node, ec.StaticContext.TypeModel);
				rs.add(element);
			}

			rs = processAttributes(node, ids, rs, ec);
			rs = processChildNodes(node, ids, rs, ec);

			return rs.Sequence;
		}

		private static ArrayList createIDs(string[] idtokens)
		{
			ArrayList xsid = new ArrayList();
			for (int i = 0; i < idtokens.Length; i++)
			{
				XSID id = new XSID(idtokens[i]);
				xsid.Add(id);
			}
			return xsid;
		}

		private static ResultBuffer processChildNodes(Node node, IList ids, ResultBuffer rs, EvaluationContext ec)
		{
			if (!node.hasChildNodes())
			{
				return rs;
			}

			NodeList nodeList = node.ChildNodes;
			for (int nodecnt = 0; nodecnt < nodeList.Length; nodecnt++)
			{
				Node childNode = nodeList.item(nodecnt);
				if (childNode.NodeType == NodeConstants.ELEMENT_NODE && !isDuplicate(childNode, rs))
				{
					ElementType element = new ElementType((Element)childNode, ec.StaticContext.TypeModel);
					if (element.IDREF)
					{
						if (hasID(ids, childNode))
						{
							rs.add(element);
						}
					}
					rs = processAttributes(childNode, ids, rs, ec);
					rs = processChildNodes(childNode, ids, rs, ec);
				}
			}

			return rs;

		}

		private static ResultBuffer processAttributes(Node node, IList idrefs, ResultBuffer rs, EvaluationContext ec)
		{
			if (!node.hasAttributes())
			{
				return rs;
			}

			NamedNodeMap attributeList = node.Attributes;
			for (int atsub = 0; atsub < attributeList.Length; atsub++)
			{
				Attr atNode = (Attr) attributeList.item(atsub);
				NodeType atType = new AttrType(atNode, ec.StaticContext.TypeModel);
				if (atType.ID)
				{
					if (hasID(idrefs, atNode))
					{
						if (!isDuplicate(node, rs))
						{
							ElementType element = new ElementType((Element)node, ec.StaticContext.TypeModel);
							rs.add(element);
						}
					}
				}
			}
			return rs;
		}

		private static bool hasID(IList ids, Node node)
		{
			for (int i = 0; i < ids.Count; i++)
			{
				XSID idref = (XSID) ids[i];
				if (idref.StringValue.Equals(node.NodeValue))
				{
					return true;
				}
			}
			return false;
		}

		private static bool isDuplicate(Node node, ResultBuffer rs)
		{
			IEnumerator it = rs.iterator();
			while (it.MoveNext())
			{
				if (it.Current.Equals(node))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnIDREF))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_STAR);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}

	}

}