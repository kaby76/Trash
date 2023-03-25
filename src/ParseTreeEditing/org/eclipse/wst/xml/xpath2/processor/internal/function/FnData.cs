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
///     Mukul Gandhi - bug 276134 - improvements to schema aware primitive type support
///                                 for attribute/element nodes 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// fn:data takes a sequence of items and returns a sequence of atomic values.
	/// The result of fn:data is the sequence of atomic values produced by applying
	/// the following rules to each item in $arg: - If the item is an atomic value,
	/// it is returned. - If the item is a node, fn:data() returns the typed value of
	/// the node as defined by the accessor function dm:typed-value in Section 5.6
	/// typed-value Accessor in the specification.
	/// </summary>
	public class FnData : Function
	{
		/// <summary>
		/// Constructor for FnData.
		/// </summary>
		public FnData() : base(new QName("data"), 1)
		{
		}

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <returns> Result of evaluation. </returns>
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			// 1 argument only!
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());
            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence argument = (ResultSequence) i.Current;

			return atomize(argument);
		}

		/// <summary>
		/// Atomize a ResultSequnce argument expression.
		/// </summary>
		/// <param name="arg">
		///            input expression. </param>
		/// <returns> Result of operation. </returns>
		public static ResultSequence atomize(ResultSequence arg)
		{

			ResultBuffer rs = new ResultBuffer();

			for (var i = arg.iterator(); i.MoveNext();)
			{
				AnyType at = (AnyType) i.Current;

				if (at is AnyAtomicType)
				{
					rs.add(at);
				}
				else if (at is NodeType)
				{
					NodeType nt = (NodeType) at;
					rs.concat(nt.typed_value());
				}
				else
				{
					Debug.Assert(false);
				}
			}

			return rs.Sequence;
		}

		/// <summary>
		/// Atomize argument expression of any type.
		/// </summary>
		/// <param name="arg">
		///            input expression. </param>
		/// <returns> Result of operation. </returns>
		public static AnyType atomize(Item arg)
		{
			if (arg is AnyAtomicType)
			{
				return (AnyAtomicType)arg;
			}
			else if (arg is NodeType)
			{
				NodeType nt = (NodeType) arg;

				return (AnyType) nt.typed_value().first();
			}
			else
			{
				Debug.Assert(false);
				return null;
			}
		}
	}

}