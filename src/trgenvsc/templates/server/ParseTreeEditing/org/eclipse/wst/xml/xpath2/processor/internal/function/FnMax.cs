using System;
using System.Diagnostics;
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
///     Jesper Moller - bug 280555 - Add pluggable collation support
///     David Carver (STAR) - bug 262765 - fixed promotion issue
///     Jesper Moller - bug 281028 - fix promotion rules for fn:max
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///    Lukasz Wycisk - bug 361060 - Aggregations with nil=�true� throw exceptions.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using ComparableTypePromoter = org.eclipse.wst.xml.xpath2.processor.@internal.utils.ComparableTypePromoter;
	using TypePromoter = org.eclipse.wst.xml.xpath2.processor.@internal.utils.TypePromoter;

	/// <summary>
	/// Selects an item from the input sequence $arg whose value is greater than or
	/// equal to the value of every other item in the input sequence. If there are
	/// two or more such items, then the specific item whose value is returned is
	/// implementation dependent.
	/// </summary>
	public class FnMax : Function
	{
		/// <summary>
		/// Constructor for FnMax.
		/// </summary>
		public FnMax() : base(new QName("max"), 1)
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
			return max(args, ec.DynamicContext);
		}

		/// <summary>
		/// Max operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="context"> 
		///            Relevant dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:max operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence max(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence max(ICollection args, DynamicContext dynamicContext)
		{

			ResultSequence arg = get_arg(args, typeof(CmpGt));
			if (arg.empty())
			{
				return ResultSequenceFactory.create_new();
			}

			CmpGt max = null;

			TypePromoter tp = new ComparableTypePromoter();
			tp.considerSequence(arg);

			for (var i = arg.iterator(); i.MoveNext();)
			{
				AnyAtomicType conv = tp.promote((AnyType) i.Current);

				if (conv != null)
				{

					if (conv is XSDouble && ((XSDouble)conv).nan() || conv is XSFloat && ((XSFloat)conv).nan())
					{
						return ResultSequenceFactory.create_new(tp.promote(new XSFloat(float.NaN)));
					}
					if (max == null || ((CmpGt)conv).gt((AnyType)max, dynamicContext))
					{
						max = (CmpGt)conv;
					}
				}
			}
			return ResultSequenceFactory.create_new((AnyType) max);
		}

		/// <summary>
		/// Obtain arguments.
		/// </summary>
		/// <param name="args">
		///            input expressions. </param>
		/// <param name="op">
		///            input class. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence get_arg(java.util.Collection args, Class op) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence get_arg(ICollection args, Type op)
		{
			Debug.Assert(args.Count == 1);
            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence arg = (ResultSequence) i.Current;

			return arg;
		}
	}

}