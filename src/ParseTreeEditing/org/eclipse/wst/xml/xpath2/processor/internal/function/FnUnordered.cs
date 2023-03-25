using System;
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
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Returns the items of $sourceSeq in a non-deterministic order.
	/// </summary>
	public class FnUnordered : Function
	{
		/// <summary>
		/// Constructor for FnUnordered.
		/// </summary>
		public FnUnordered() : base(new QName("unordered"), 1)
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
			return unordered(args);
		}

		/// <summary>
		/// Unordered operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:unordered operation. </returns>
		public static ResultSequence unordered(ICollection args)
		{

			Debug.Assert(args.Count == 1);

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence arg = (ResultSequence) citer.Current;

			if (arg == null || arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			// XXX lame
			ArrayList tmp = new ArrayList();
			for (IEnumerator i = arg.iterator(); i.MoveNext();)
			{
				tmp.Add(i.Current);
			}

            throw new Exception();
			//Collections.shuffle(tmp);

			//ResultBuffer rb = new ResultBuffer();
			//for (IEnumerator i = tmp.GetEnumerator(); i.MoveNext();)
			//{
			//	rb.add((AnyType) i.Current);
			//}

			//return rb.Sequence;
		}
	}

}