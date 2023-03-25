using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2012 Andrea Bittau, University College London, and others
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
///     Lukasz Wycisk - bug 261059 - FnRoundHalfToEven is wrong in case of 2 arguments
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// The value returned is the nearest (that is, numerically closest) numeric to
	/// $arg that is a multiple of ten to the power of minus $precision. If two such
	/// values are equally near (e.g. if the fractional part in $arg is exactly
	/// .500...), returns the one whose least significant digit is even. If type of
	/// $arg is one of the four numeric types xs:float, xs:double, xs:decimal or
	/// xs:integer the type of the return is the same as the type of $arg. If the
	/// type of $arg is a type derived from one of the numeric types, the type of the
	/// return is the base numeric type.
	/// </summary>
	public class FnRoundHalfToEven : Function
	{
		/// <summary>
		/// Constructor for FnRoundHalfToEven.
		/// </summary>
		public FnRoundHalfToEven() : base(new QName("round-half-to-even"), 1, 2)
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
            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence argument = (ResultSequence) i.Current;
			if (args.Count == 2)
			{
				return fn_round_half_to_even(args);
			}

			return fn_round_half_to_even(argument);
		}

		/// <summary>
		/// Round-Half-to-Even operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:round-half-to-even operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fn_round_half_to_even(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fn_round_half_to_even(ResultSequence arg)
		{

			NumericType nt = FnAbs.get_single_numeric_arg(arg);

			// empty arg
			if (nt == null)
			{
				return ResultBuffer.EMPTY;
			}

			return nt.round_half_to_even();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fn_round_half_to_even(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fn_round_half_to_even(ICollection args)
		{

			if (args.Count > 2 || args.Count <= 1)
			{
				throw new DynamicError(TypeError.invalid_type(null));
			}

			IEnumerator argIt = args.GetEnumerator();
            argIt.MoveNext();
            ResultSequence rsArg1 = (ResultSequence) argIt.Current;
            argIt.MoveNext();
			ResultSequence rsPrecision = (ResultSequence) argIt.Current;

			NumericType nt = FnAbs.get_single_numeric_arg(rsArg1);

			// empty arg
			if (nt == null)
			{
				return ResultBuffer.EMPTY;
			}

			NumericType ntPrecision = (NumericType) rsPrecision.first();

			return nt.round_half_to_even(int.Parse(ntPrecision.StringValue));
		}
	}

}