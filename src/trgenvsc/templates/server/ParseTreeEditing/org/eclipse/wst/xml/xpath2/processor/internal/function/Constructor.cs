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
///     Mukul Gandhi - bug274784 - improvements to xs:boolean data type implementation
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using CtrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.CtrType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Constructor class for functions.
	/// </summary>
	public class Constructor : Function
	{
		private CtrType _atomic_type;

		/// <summary>
		/// Constructor for Constructor class.
		/// </summary>
		/// <param name="aat">
		///            input of any atomic type. </param>
		public Constructor(CtrType aat) : base(new QName(aat.type_name()), 1)
		{

			_atomic_type = aat;
		}

		// XXX IN GENRAL, I THIUNK WE NEED TO PULL SANITY CHECKING OUTSIDE!
		// PLUS I AM NOT ATOMIZING/ETC ETC HERE!!! BAD CODE
		// BUG XXX HACK DEATH
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
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());
            var i = args.GetEnumerator();
            i.MoveNext();
			// sanity checks
			ResultSequence arg = (ResultSequence) i.Current;

			if (arg == null || arg.size() > 1)
			{
				DynamicError.throw_type_error();
			}

			// do it
			return _atomic_type.constructor(arg);
		}

	}

}