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
///     Jesper Steen Moller  - bug 262765 - propagate possible errors from xs:boolean
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;

	/// <summary>
	/// $arg is first reduced to an effective boolean value by applying the
	/// fn:boolean() function. Returns true if the effective boolean value is false,
	/// and false if the effective boolean value is true.
	/// </summary>
	public class FnNot : Function
	{
		/// <summary>
		/// Constructor for FnNot.
		/// </summary>
		public FnNot() : base(new QName("not"), 1)
		{
		}

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <returns> Result of evaluation. </returns>
		/// <exception cref="DynamicError">  </exception>
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			// 1 argument only!
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence argument = (ResultSequence) i.Current;

			return fn_not(argument);
		}

		/// <summary>
		/// Not operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <returns> Result of fn:note operation. </returns>
		/// <exception cref="DynamicError">  </exception>
		public static ResultSequence fn_not(ResultSequence arg)
		{
			XSBoolean ret = FnBoolean.fn_boolean(arg);

			bool answer = false;

			if (ret.value() == false)
			{
				answer = true;
			}

			return ResultSequenceFactory.create_new(new XSBoolean(answer));
		}

	}

}