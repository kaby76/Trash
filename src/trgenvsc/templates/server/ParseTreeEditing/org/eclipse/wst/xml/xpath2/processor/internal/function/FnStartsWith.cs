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


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// Returns an xs:boolean indicating whether or not the value of $arg1 starts
	/// with a sequence of collation units that provides a minimal match to the
	/// collation units of $arg2 according to the collation that is used.
	/// </summary>
	public class FnStartsWith : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnStartsWith.
		/// </summary>
		public FnStartsWith() : base(new QName("starts-with"), 2)
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
			return starts_with(args);
		}

		/// <summary>
		/// Starts-with operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:starts-with operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence starts_with(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence starts_with(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;
			string str1 = "";
			string str2 = "";
			if (!arg1.empty())
			{
				str1 = ((XSString) arg1.first()).value();
			}

            argiter.MoveNext();
            ResultSequence arg2 = (ResultSequence) argiter.Current;
			if (!(arg2 == null || arg2.empty()))
			{
				str2 = ((XSString) arg2.first()).value();
			}

			int str1len = str1.Length;
			int str2len = str2.Length;

			if (str1len == 0 && str2len != 0)
			{
				return XSBoolean.FALSE;
			}
			if (str2len == 0)
			{
				return XSBoolean.TRUE;
			}

			return XSBoolean.valueOf(str1.StartsWith(str2, StringComparison.Ordinal));
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnStartsWith))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}
	}

}