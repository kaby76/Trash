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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// <para>
	/// Sequence reverse function.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:reverse($arg as item()*) as item()*
	/// </para>
	/// 
	/// <para>
	/// This class reverses the order of items in a sequence. If $arg is the empty
	/// sequence, the empty sequence is returned.
	/// </para>
	/// </summary>
	public class FnReverse : Function
	{

		/// <summary>
		/// Constructor for FnReverse.
		/// </summary>
		public FnReverse() : base(new QName("reverse"), 1)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the reversal of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return reverse(args);
		}

		/// <summary>
		/// Reverse the arguments.
		/// </summary>
		/// <param name="args">
		///            are reversed. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of the reversal of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence reverse(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence reverse(ICollection args)
		{

			Debug.Assert(args.Count == 1);

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence arg = (ResultSequence) citer.Current;

			if (arg.size() <= 1)
			{
				return arg;
			}

			ResultBuffer rs = new ResultBuffer();

			for (int i = arg.size() - 1; i >= 0; --i)
			{
				rs.add(arg.item(i));
			}

			return rs.Sequence;
		}
	}

}