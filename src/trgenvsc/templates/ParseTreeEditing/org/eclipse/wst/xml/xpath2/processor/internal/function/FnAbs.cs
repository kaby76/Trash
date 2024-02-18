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
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;

	/// <summary>
	/// Returns the absolute value of $arg. If $arg is negative returns -$arg
	/// otherwise returns $arg. If type of $arg is one of the four numeric types
	/// xs:float, xs:double, xs:decimal or xs:integer the type of the return is the
	/// same as the type of $arg. If the type of $arg is a type derived from one of
	/// the numeric types, the type of the return is the base numeric type. For
	/// xs:float and xs:double arguments, if the argument is positive zero (+0) or
	/// negative zero (-0), then positive zero (+0) is returned. If the argument is
	/// positive or negative infinity, positive infinity is returned.
	/// </summary>
	public class FnAbs : Function
	{
		/// <summary>
		/// Constructor for FnAbs.
		/// </summary>
		public FnAbs() : base(new QName("abs"), 1)
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
			// 1 argument only!
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());
            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence argument = (ResultSequence) i.Current;

			return fn_abs(argument);
		}

		/// <summary>
		/// Absolute value operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:abs operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fn_abs(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fn_abs(ResultSequence arg)
		{
			// sanity chex
			NumericType nt = get_single_numeric_arg(arg);

			// empty arg
			if (nt == null)
			{
				return ResultBuffer.EMPTY;
			}

			if (nt is XSDouble)
			{
				XSDouble dat = (XSDouble) nt;
				if (dat.zero() || dat.negativeZero())
				{
					return new XSDouble("0");
				}
				if (dat.infinite())
				{
					return new XSDouble(double.PositiveInfinity);
				}
			}

			if (nt is XSFloat)
			{
				XSFloat dat = (XSFloat) nt;
				if (dat.zero() || dat.negativeZero())
				{
					return new XSFloat((float)0);
				}
				if (dat.infinite())
				{
					return new XSFloat(float.PositiveInfinity);
				}
			}


			return nt.abs();
		}

		/// <summary>
		/// Obtain numeric value from expression.
		/// </summary>
		/// <param name="arg">
		///            input expression. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Resulting numeric type from the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.processor.internal.types.NumericType get_single_numeric_arg(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static NumericType get_single_numeric_arg(ResultSequence arg)
		{
			int size = arg.size();
			if (size > 1)
			{
				DynamicError.throw_type_error();
			}

			if (size == 0)
			{
				return null;
			}

			arg = FnData.atomize(arg);
			AnyType at = (AnyType) arg.item(0);

			if (!(at is NumericType))
			{
				throw DynamicError.invalidType();
			}

			return (NumericType) at;
		}

	}

}