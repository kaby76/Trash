using System;
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


	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// The input $value is returned, unchanged, as the result of the function. In
	/// addition, the inputs $value, converted to an xs:string, and $label may be
	/// directed to a trace data set. The location and format of the trace data set
	/// are implementation dependent. The ordering of output from invocations of the
	/// fn:trace() function is implementation dependent.
	/// </summary>
	public class FnTrace : Function
	{
		/// <summary>
		/// Constructor for FnTrace.
		/// </summary>
		public FnTrace() : base(new QName("trace"), 2)
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
			return trace(args);
		}

		/// <summary>
		/// Trace operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:trace operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence trace(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence trace(ICollection args)
		{

			// sanity check args
			if (args.Count != 2)
			{
				DynamicError.throw_type_error();
			}

			IEnumerator argsi = args.GetEnumerator();
            argsi.MoveNext();
            ResultSequence arg1 = (ResultSequence) argsi.Current;
            argsi.MoveNext();
			ResultSequence arg2 = (ResultSequence) argsi.Current;

			if (arg2 == null || arg2.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = arg2.first();
			if (!(at is XSString))
			{
				DynamicError.throw_type_error();
			}

			XSString label = (XSString) at;

			int index = 1;

			for (var i = arg1.iterator(); i.MoveNext(); index++)
			{
				at = (AnyType) i.Current;

				Console.WriteLine(label.value() + " [" + index + "] " + ((AnyType)at).string_type() + ":" + at.StringValue);

			}

			return arg1;
		}
	}

}