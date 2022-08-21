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

	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Class for Minus function.
	/// </summary>
	public class FsMinus : Function
	{
		/// <summary>
		/// Constructor for FsMinus.
		/// </summary>
		public FsMinus() : base(new QName("minus"), 2)
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
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			return fs_minus(args);
		}

		/// <summary>
		/// General operation on the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_minus(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_minus(ICollection args)
		{
			return FsPlus.do_math_op(args, typeof(MathMinus), "minus");
		}

		/// <summary>
		/// Unary operation on the values of the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_minus_unary(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_minus_unary(ICollection args)
		{
			// make sure we got only one arg
			if (args.Count != 1)
			{
				DynamicError.throw_type_error();
			}

            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence arg = (ResultSequence) i.Current;

			// make sure we got only one numeric atom
			if (arg.size() != 1)
			{
				DynamicError.throw_type_error();
			}
			Item at = arg.first();
			if (!(at is NumericType))
			{
				DynamicError.throw_type_error();
			}

			NumericType nt = (NumericType) at;

			return nt.unary_minus();
		}
	}

}