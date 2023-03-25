using System;

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
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using CmpGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpGt;
	using CmpLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpLt;
	using MathDiv = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathDiv;
	using MathIDiv = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathIDiv;
	using MathMinus = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathMinus;
	using MathMod = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathMod;
	using MathPlus = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathPlus;
	using MathTimes = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathTimes;

	/// <summary>
	/// A representation of the NumericType datatype
	/// </summary>
	public abstract class NumericType : CtrType, CmpEq, CmpGt, CmpLt, MathPlus, MathMinus, MathTimes, MathDiv, MathIDiv, MathMod
	{
		public abstract ResultSequence mod(ResultSequence arg);
		public abstract ResultSequence idiv(ResultSequence arg);
		public abstract ResultSequence div(ResultSequence arg);
		public abstract ResultSequence times(ResultSequence arg);
		public abstract ResultSequence minus(ResultSequence arg);
		public abstract ResultSequence plus(ResultSequence arg);
		public abstract bool lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context);
		public abstract bool gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context);
		public abstract bool eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context);

		// XXX needed for fn:boolean
		/// <summary>
		/// Check whether node represnts 0
		/// </summary>
		/// <returns> True if node represnts 0. False otherwise </returns>
		public abstract bool zero();

		/// <summary>
		/// Creates a new ResultSequence representing the negation of the number
		/// stored
		/// </summary>
		/// <returns> New ResultSequence representing the negation of the number stored </returns>
		public abstract ResultSequence unary_minus();

		// numeric functions
		/// <summary>
		/// Absolutes the number stored
		/// </summary>
		/// <returns> New NumericType representing the absolute of the number stored </returns>
		public abstract NumericType abs();

		/// <summary>
		/// Returns the smallest integer greater than the number stored
		/// </summary>
		/// <returns> A NumericType representing the smallest integer greater than the
		///         number stored </returns>
		public abstract NumericType ceiling();

		/// <summary>
		/// Returns the largest integer smaller than the number stored
		/// </summary>
		/// <returns> A NumericType representing the largest integer smaller than the
		///         number stored </returns>
		public abstract NumericType floor();

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A NumericType representing the closest long of the number stored. </returns>
		public abstract NumericType round();

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A NumericType representing the closest long of the number stored. </returns>
		public abstract NumericType round_half_to_even();

		public abstract NumericType round_half_to_even(int precision);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.eclipse.wst.xml.xpath2.api.Item get_single_arg(org.eclipse.wst.xml.xpath2.api.ResultSequence rs) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		protected internal virtual Item get_single_arg(ResultSequence rs)
		{
			if (rs.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			return rs.first();
		}

		/// <summary>
		///*
		/// Check whether the supplied node is of the supplied type
		/// </summary>
		/// <param name="at">
		///            The node being tested </param>
		/// <param name="type">
		///            The type expected </param>
		/// <returns> The node being tested </returns>
		/// <exception cref="DynamicError">
		///             If node being tested is not of expected type </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.Item get_single_type(org.eclipse.wst.xml.xpath2.api.Item at, Class type) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static Item get_single_type(Item at, Type type)
		{

			if (!type.IsInstanceOfType(at))
			{
				DynamicError.throw_type_error();
			}

			return at;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.Item get_single_type(AnyType at, Class type) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static Item get_single_type(AnyType at, Type type)
		{
			return get_single_type((Item)at, type);
		}

		/// <summary>
		///*
		/// Check whether first node in supplied ResultSequence is of the supplied
		/// type
		/// </summary>
		/// <param name="rs">
		///            The node being tested </param>
		/// <param name="type">
		///            The type expected </param>
		/// <returns> The node being tested </returns>
		/// <exception cref="DynamicError">
		///             If node being tested is not of expected type </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static AnyType get_single_type(org.eclipse.wst.xml.xpath2.api.ResultSequence rs, Class type) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static AnyType get_single_type(ResultSequence rs, Type type)
		{
			if (rs.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = rs.first();

			if (!type.IsInstanceOfType(at))
			{
				DynamicError.throw_type_error();
			}

			return (AnyType) at;
		}
	}

}