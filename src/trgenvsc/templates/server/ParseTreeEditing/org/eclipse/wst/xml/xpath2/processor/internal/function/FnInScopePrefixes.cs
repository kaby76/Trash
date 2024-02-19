using System.Collections;
using java.xml;
using org.w3c.dom;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver  - initial API and implementation
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Element = org.w3c.dom.Element;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Returns the in-scope-prefixes for the element and any of it's ancestors.
	/// </summary>
	public class FnInScopePrefixes : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnPrefixFromQName
		/// </summary>
		public FnInScopePrefixes() : base(new QName("in-scope-prefixes"), 1)
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return inScopePrefixes(args, ec.DynamicContext);
		}

		/// <summary>
		/// Prefix-from-QName operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:prefix-from-QName operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence inScopePrefixes(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence inScopePrefixes(ICollection args, DynamicContext dc)
		{

	//		Collection cargs = Function.convert_arguments(args, expected_args());
			ICollection cargs = args;
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1 == null || arg1.empty())
			{
			  return ResultBuffer.EMPTY;
			}

			ResultBuffer rs = new ResultBuffer();

			Item anytype = arg1.item(0);
			if (!(anytype is ElementType))
			{
				throw new DynamicError(TypeError.invalid_type(null));
			}

			ElementType element = (ElementType) anytype;
			IList prefixList = lookupPrefixes(element);
			createPrefixResultSet(rs, prefixList);
			return rs.Sequence;
		}

		private static void createPrefixResultSet(ResultBuffer rs, IList prefixList)
		{
			for (int i = 0; i < prefixList.Count; i++)
			{
				string prefix = (string) prefixList[i];
				rs.add(new XSString(prefix));
			}
		}

		private static IList lookupPrefixes(ElementType element)
		{
			Element domElm = (Element) element.node_value();

			IList prefixList = new ArrayList();
			Node node = domElm;

			while (node != null && node.NodeType != NodeConstants.DOCUMENT_NODE)
			{
				NamedNodeMap attrs = node.Attributes;
				for (int i = 0; i < attrs.Length; i++)
				{
					Node attr = attrs.item(i);
					string prefix = null;
					if (attr.NamespaceURI != null && attr.NamespaceURI.Equals(XMLConstants.XMLNS_ATTRIBUTE_NS_URI))
					{
						// Default Namespace
						if (attr.NodeName.Equals(XMLConstants.XMLNS_ATTRIBUTE))
						{
							prefix = "";
						}
						else
						{
							// Should we check the namespace in the Dynamic Context and return that???
							prefix = attr.LocalName;
						}
						if (!string.ReferenceEquals(prefix, null))
						{
							if (!prefixList.Contains(prefix))
							{
								prefixList.Add(prefix);
							}
						}
					}
				}

				node = node.ParentNode;
			}
			return prefixList;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnInScopePrefixes))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new ElementType(), SeqType.OCC_PLUS);
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}
	}

}