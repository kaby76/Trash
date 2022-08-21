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
///     Jesper Moller - bug 280555 - Add pluggable collation support
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Class for Not equal function.
	/// </summary>
	public class FsNe : Function
	{
		/// <summary>
		/// Constructor for FsNe.
		/// </summary>
		public FsNe() : base(new QName("ne"), 2)
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
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence evaluate(ICollection args, DynamicContext ec)
		{
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			return fs_ne_value(args, ec);
		}

		/// <summary>
		/// Operation on the values of the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="context"> 
		///             The dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_ne_value(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_ne_value(ICollection args, DynamicContext context)
		{
			return FnNot.fn_not(FsEq.fs_eq_value(args, context));
		}

		/// <summary>
		/// General operation on the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="dc"> 
		///             The dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_ne_general(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_ne_general(ICollection args, DynamicContext ec)
		{
			return FsEq.do_cmp_general_op(args, typeof(FsNe), "fs_ne_value", ec);
		}

	}

}