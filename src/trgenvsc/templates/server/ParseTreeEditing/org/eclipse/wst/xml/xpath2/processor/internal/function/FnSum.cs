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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type 
///     Jesper Moller - bug 281028 - fix promotion rules for fn:sum
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///    Lukasz Wycisk - bug 361060 - Aggregations with nil=�true� throw exceptions.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using ScalarTypePromoter = org.eclipse.wst.xml.xpath2.processor.@internal.utils.ScalarTypePromoter;
	using TypePromoter = org.eclipse.wst.xml.xpath2.processor.@internal.utils.TypePromoter;

	/// <summary>
	/// Returns a value obtained by adding together the values in $arg. If the
	/// single-argument form of the function is used, then the value returned for an
	/// empty sequence is the xs:integer value 0. If the two-argument form is used,
	/// then the value returned for an empty sequence is the value of the $zero
	/// argument.
	/// </summary>
	public class FnSum : Function
	{

		private static XSInteger ZERO = new XSInteger(System.Numerics.BigInteger.Zero);

		/// <summary>
		/// Constructor for FnSum.
		/// </summary>
		public FnSum() : base(new QName("sum"), 1, 2)
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
			IEnumerator argIterator = args.GetEnumerator();
            argIterator.MoveNext();
            ResultSequence argSequence = (ResultSequence) argIterator.Current;
			AnyAtomicType zero = ZERO;
			if (argIterator.MoveNext())
			{
				ResultSequence zeroSequence = (ResultSequence)argIterator.Current;
				if (zeroSequence.size() != 1)
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}
				if (!(zeroSequence.first() is AnyAtomicType))
				{
					throw new DynamicError(TypeError.invalid_type(zeroSequence.first().StringValue));
				}
				zero = (AnyAtomicType)zeroSequence.first();
			}
			return sum(argSequence, zero);
		}

		/// <summary>
		/// Sum operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:sum operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence sum(org.eclipse.wst.xml.xpath2.api.ResultSequence arg, org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType zero) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence sum(ResultSequence arg, AnyAtomicType zero)
		{


			if (arg.empty())
			{
				return ResultSequenceFactory.create_new(zero);
			}

			MathPlus total = null;

			TypePromoter tp = new ScalarTypePromoter();
			tp.considerSequence(arg);

			for (var i = arg.iterator(); i.MoveNext();)
			{
				AnyAtomicType conv = tp.promote((AnyType) i.Current);

				if (conv == null)
				{
					conv = zero;
				}

				if (conv is XSDouble && ((XSDouble)conv).nan() || conv is XSFloat && ((XSFloat)conv).nan())
				{
					return ResultSequenceFactory.create_new(tp.promote(new XSFloat(float.NaN)));
				}
				if (total == null)
				{
					total = (MathPlus)conv;
				}
				else
				{
					total = (MathPlus)total.plus(ResultSequenceFactory.create_new(conv)).first();
				}
			}

			return ResultSequenceFactory.create_new((AnyType) total);
		}
	}

}