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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;

	/// <summary>
	/// Returns a new sequence constructed from the value of $target with the value
	/// of $inserts inserted at the position specified by the value of $position.
	/// (The value of $target is not affected by the sequence construction.)
	/// </summary>
	public class FnInsertBefore : Function
	{
		/// <summary>
		/// Constructor for FnInsertBefore.
		/// </summary>
		public FnInsertBefore() : base(new QName("insert-before"), 3)
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
			return insert_before(args);
		}

		/// <summary>
		/// Insert-Before operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:insert-before operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence insert_before(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence insert_before(ICollection args)
		{

			Debug.Assert(args.Count == 3);

			ResultBuffer rs = new ResultBuffer();

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence target = (ResultSequence) citer.Current;
            citer.MoveNext();
			ResultSequence arg2 = (ResultSequence) citer.Current;
            citer.MoveNext();
			ResultSequence inserts = (ResultSequence) citer.Current;

			// sanity chex
			if (arg2.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = arg2.first();
			if (!(at is XSInteger))
			{
				DynamicError.throw_type_error();
			}

			// XXX cloning!
			if (target.empty())
			{
				return inserts;
			}
			if (inserts.empty())
			{
				return target;
			}

			int position = (int) ((XSInteger) at).int_value();

			if (position < 1)
			{
				position = 1;
			}
			int target_size = target.size();

			if (position > target_size)
			{
				position = target_size + 1;
			}

			int curpos = 1;

			for (var i = target.iterator(); i.MoveNext();)
			{
				at = (AnyType) i.Current;

				if (curpos == position)
				{
					rs.concat(inserts);
				}

				rs.add(at);

				curpos++;
			}
			if (curpos == position)
			{
				rs.concat(inserts);
			}

			return rs.Sequence;
		}
	}

}