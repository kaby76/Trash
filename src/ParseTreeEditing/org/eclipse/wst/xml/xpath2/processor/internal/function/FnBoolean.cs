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
///     Jesper Steen Moller  - bug 262765 - use correct 'effective boolean value'
///     David Carver (STAR) - bug 262765 - fix checking of data types.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using CalendarType = org.eclipse.wst.xml.xpath2.processor.@internal.types.CalendarType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using XSUntypedAtomic = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSUntypedAtomic;

	/// <summary>
	/// Computes the effective boolean value of the sequence $arg. If $arg is the
	/// empty sequence, returns false. If $arg contains a single atomic value, then
	/// the function returns false if $arg is: - The singleton xs:boolean value
	/// false. - The singleton value "" (zero-length string) of type xs:string or
	/// xdt:untypedAtomic. - A singleton numeric value that is numerically equal to
	/// zero. - The singleton xs:float or xs:double value NaN. In all other cases,
	/// returns true.
	/// </summary>
	public class FnBoolean : Function
	{
		/// <summary>
		/// Constructor for FnBoolean.
		/// </summary>
		public FnBoolean() : base(new QName("boolean"), 1)
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

			return ResultSequenceFactory.create_new(fn_boolean(argument));
		}

		/// <summary>
		/// Boolean operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <returns> Result of fn:boolean operation. </returns>
		/// <exception cref="DynamicError">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.processor.internal.types.XSBoolean fn_boolean(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static XSBoolean fn_boolean(ResultSequence arg)
		{
			if (arg.empty())
			{
				return XSBoolean.FALSE;
			}

			Item at = arg.item(0);

			if (at is CalendarType)
			{
				throw DynamicError.throw_type_error();
			}

			if (at is NodeType)
			{
				return XSBoolean.TRUE;
			}

			if (arg.size() > 1)
			{
				throw DynamicError.throw_type_error();
			}

			// XXX ??
			if (!(at is AnyAtomicType))
			{
				return XSBoolean.TRUE;
			}

			// ok we got 1 single atomic type element

			if (at is XSBoolean)
			{
				if (!((XSBoolean) at).value())
				{
					return XSBoolean.FALSE;
				}
			}

			if ((at is XSString) || (at is XSUntypedAtomic))
			{
				if (((AnyType)at).StringValue.Equals(""))
				{
					return XSBoolean.FALSE;
				}
			}

			if (at is NumericType)
			{
				if (((NumericType) at).zero())
				{
					return XSBoolean.FALSE;
				}
			}

			if ((at is XSFloat) && (((XSFloat) at).nan()))
			{
				return XSBoolean.FALSE;
			}

			if ((at is XSDouble) && (((XSDouble) at).nan()))
			{
				return XSBoolean.FALSE;
			}


			return XSBoolean.TRUE;
		}

	}

}