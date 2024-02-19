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
///     Jesper Steen Mooller - bug 280555 - Add pluggable collation support
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;

	/// <summary>
	/// Class for the Greater than or equal to function.
	/// </summary>
	public class FsGe : Function
	{
		/// <summary>
		/// Constructor for FsGe.
		/// </summary>
		public FsGe() : base(new QName("ge"), 2)
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
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			return fs_ge_value(args, ec.DynamicContext);
		}

		/// <summary>
		/// Greater than or equal to operation on the values of the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="dc"> </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_ge_value(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_ge_value(ICollection args, DynamicContext dc)
		{
			ResultSequence greater = FsGt.fs_gt_value(args, dc);

			if (((XSBoolean) greater.first()).value())
			{
				return greater;
			}

			ResultSequence equal = FsEq.fs_eq_value(args, dc);

			if (((XSBoolean) equal.first()).value())
			{
				return equal;
			}

			return ResultSequenceFactory.create_new(new XSBoolean(false));
		}

		/// <summary>
		/// General greater than or equal to operation.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="dc"> 
		///             The dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_ge_general(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_ge_general(ICollection args, DynamicContext dc)
		{
			return FsEq.do_cmp_general_op(args, typeof(FsGe), "fs_ge_value", dc);
		}
	}

}