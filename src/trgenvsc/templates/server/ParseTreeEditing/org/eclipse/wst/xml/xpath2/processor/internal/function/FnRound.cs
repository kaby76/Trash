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
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Returns the number with no fractional part that is closest to the argument.
	/// If there are two such numbers, then the one that is closest to positive
	/// infinity is returned. More formally, fn:round(x) produces the same result as
	/// fn:floor(x+0.5). If type of $arg is one of the four numeric types xs:float,
	/// xs:double, xs:decimal or xs:integer the type of the return is the same as the
	/// type of $arg. If the type of $arg is a type derived from one of the numeric
	/// types, the type of the return is the base numeric type.
	/// </summary>
	public class FnRound : Function
	{
		/// <summary>
		/// Constructor for FnRound.
		/// </summary>
		public FnRound() : base(new QName("round"), 1)
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
			// 1 argument only!
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());
            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence argument = (ResultSequence) args.GetEnumerator().Current;

			return fn_round(argument);
		}

		/// <summary>
		/// Round operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:round operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fn_round(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fn_round(ResultSequence arg)
		{

			// sanity chex
			NumericType nt = FnAbs.get_single_numeric_arg(arg);

			// empty arg
			if (nt == null)
			{
				return ResultBuffer.EMPTY;
			}

			return nt.round();
		}
	}

}