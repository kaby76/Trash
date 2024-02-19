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
///    Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///    Mukul Gandhi - bug 273760 - wrong namespace for functions and data types 
///    David Carver - bug 262765 - fix issue with casting items to XSDouble cast
///                                needed to cast to Numeric so that evaluations
///                                and formatting occur correctly.
///                              - fix fn:avg casting issues and divide by zero issues.
///    Jesper Moller - bug 281028 - fix promotion rules for fn:avg
///    Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///    Lukasz Wycisk - bug 361060 - Aggregations with nil=�true� throw exceptions.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using ScalarTypePromoter = org.eclipse.wst.xml.xpath2.processor.@internal.utils.ScalarTypePromoter;
	using TypePromoter = org.eclipse.wst.xml.xpath2.processor.@internal.utils.TypePromoter;

	/// <summary>
	/// Returns the average of the values in the input sequence $arg, that is, the
	/// sum of the values divided by the number of values.
	/// </summary>
	public class FnAvg : Function
	{
		/// <summary>
		/// Constructor for FnAvg.
		/// </summary>
		public FnAvg() : base(new QName("avg"), 1)
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
			return avg(args);
		}

		/// <summary>
		/// Average value operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:avg operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence avg(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence avg(ICollection args)
        {
            var j = args.GetEnumerator();
            j.MoveNext();
            ResultSequence arg = (ResultSequence)j.Current;

			if (arg == null || arg.empty())
			{
				return ResultSequenceFactory.create_new();
			}

			int elems = 0;

			MathPlus total = null;

			TypePromoter tp = new ScalarTypePromoter();
			tp.considerSequence(arg);

			for (var i = arg.iterator(); i.MoveNext();)
			{
				++elems;
				AnyAtomicType conv = tp.promote((AnyType) i.Current);
				if (conv != null)
				{

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
			}

			if (!(total is MathDiv))
			{
				DynamicError.throw_type_error();
			}

			return ((MathDiv)total).div(ResultSequenceFactory.create_new(new XSInteger(new System.Numerics.BigInteger(elems))));
		}

		public override TypeDefinition ResultType
		{
			get
			{
				// TODO Auto-generated method stub
				return base.ResultType;
			}
		}
	}

}