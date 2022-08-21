using System.Diagnostics;
using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     David Carver - STAR - bug 262765 - clean up fn:root according to spec. 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moeller - bug 281159 - tak extra care to find the root 
///     Jesper Steen Moller - bug 275610 - Avoid big time and memory overhead for externals
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using Attr = org.w3c.dom.Attr;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Returns the root of the tree to which $arg belongs. This will usually, but
	/// not necessarily, be a document node.
	/// </summary>
	public class FnRoot : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnRoot.
		/// </summary>
		public FnRoot() : base(new QName("root"), 0, 1)
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

			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			//ResultSequence argument = (ResultSequence) args.iterator().next();

			return fn_root(args, ec);
		}

		/// <summary>
		/// Root operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <param name="dc">
		///            Result of dynamic context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:root operation. </returns>
		public static ResultSequence fn_root(ICollection args, EvaluationContext ec)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			if (cargs.Count > 1)
			{
				throw new DynamicError(TypeError.invalid_type(null));
			}

			ResultSequence arg = null;
			if (cargs.Count == 0)
			{
				if (ec.ContextItem == null)
				{
					throw DynamicError.contextUndefined();
				}
				arg = ResultBuffer.wrap(ec.ContextItem);
			}
			else
            {
                var i = cargs.GetEnumerator();
                i.MoveNext();
                arg = (ResultSequence) i.Current;
            }

			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			Item aa = arg.item(0);

			if (!(aa is NodeType))
			{
				throw new DynamicError(TypeError.invalid_type(null));
			}

			NodeType nt = (NodeType) aa;

			// ok we got a sane argument... own it.
			Node root = nt.node_value();

			while (root != null && !(root is Document))
			{
				Node newroot = root.ParentNode;
				if (newroot == null && root is Attr)
				{
					newroot = ((Attr)root).OwnerElement;
				}

				// found it
				if (newroot == null)
				{
					break;
				}

				root = newroot;
			}

			return NodeType.dom_to_xpath(root, ec.StaticContext.TypeModel);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnRoot))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(SeqType.OCC_QMARK);
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}


	}

}