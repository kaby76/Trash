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


	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;

	/// <summary>
	/// Returns a new sequence constructed from the value of $target with the item at
	/// the position specified by the value of $position removed. If $position is
	/// less than 1 or greater than the number of items in $target, $target is
	/// returned. Otherwise, the value returned by the function consists of all items
	/// of $target whose index is less than $position, followed by all items of
	/// $target whose index is greater than $position. If $target is the empty
	/// sequence, the empty sequence is returned.
	/// </summary>
	public class FnRemove : Function
	{
		/// <summary>
		/// Constructor for FnRemove.
		/// </summary>
		public FnRemove() : base(new QName("remove"), 2)
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
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return remove(args);
		}

		/// <summary>
		/// Remove operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:remove operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence remove(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence remove(ICollection args)
		{

			Debug.Assert(args.Count == 2);

			ResultBuffer rs = new ResultBuffer();

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence target = (ResultSequence) citer.Current;
            citer.MoveNext();
			ResultSequence arg2 = (ResultSequence) citer.Current;

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

			int position = (int) ((XSInteger) at).int_value();

			if (position < 1)
			{
				return target;
			}

			if (position > target.size())
			{
				return target;
			}

			if (target.empty())
			{
				return rs.Sequence;
			}

			int curpos = 1;

			for (var i = target.iterator(); i.MoveNext();)
			{
				at = (AnyType) i.Current;

				if (curpos != position)
				{
					rs.add(at);
				}

				curpos++;
			}

			return rs.Sequence;
		}
	}

}