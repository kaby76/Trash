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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using CtrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.CtrType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using XSUntypedAtomic = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSUntypedAtomic;

	/// <summary>
	/// Function to convert a sequence of items to a sequence of atomic values.
	/// </summary>
	public class FsConvertOperand : Function
	{

		public FsConvertOperand() : base(new QName("convert-operand"), 2)
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
			return convert_operand(args);
		}

		/// <summary>
		/// Convert-Operand operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fs: operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence convert_operand(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence convert_operand(ICollection args)
		{

			Debug.Assert(args.Count == 2);

			IEnumerator iter = args.GetEnumerator();

            iter.MoveNext();
            ResultSequence actual = (ResultSequence) iter.Current;
            iter.MoveNext();
            ResultSequence expected = (ResultSequence) iter.Current;

			if (expected == null || expected.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = expected.first();

			if (!(at is AnyAtomicType))
			{
				DynamicError.throw_type_error();
			}

			AnyAtomicType exp_aat = (AnyAtomicType) at;

			ResultBuffer result = new ResultBuffer();

			// 1
			if (actual.empty())
			{
				return result.Sequence;
			}

			// convert sequence
			for (var i = actual.iterator(); i.MoveNext();)
			{
				AnyType item = (AnyType) i.Current;

				// 2
				if (item is XSUntypedAtomic)
				{
					// a
					if (exp_aat is XSUntypedAtomic)
					{
						result.add(new XSString(item.StringValue));
					}
					// b
					else if (exp_aat is NumericType)
					{
						result.add(new XSDouble(item.StringValue));
					}
					// c
					else
					{
						Debug.Assert(exp_aat is CtrType);
						CtrType cons = (CtrType) exp_aat;
						result.concat(cons.constructor(new XSString(item.StringValue)));
					}
				}
				// 4
				else
				{
					result.add(item);
				}

			}

			return result.Sequence;
		}
	}

}