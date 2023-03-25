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
///     David Carver (STAR) - bug 262765 - added exception handling to toss correct error numbers. 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// The function returns the xs:string that is obtained by replacing each
	/// non-overlapping substring of $input that matches the given $pattern with an
	/// occurrence of the $replacement string.
	/// </summary>
	public class FnReplace : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for RnReplace.
		/// </summary>
		public FnReplace() : base(new QName("replace"), 3, 4)
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
			return replace(args);
		}

		/// <summary>
		/// Replace operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:replace operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence replace(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence replace(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;
			string str1 = "";
			if (!arg1.empty())
			{
				str1 = ((XSString) arg1.first()).value();
			}

            argiter.MoveNext();
            ResultSequence arg2 = (ResultSequence) argiter.Current;
            argiter.MoveNext();
            ResultSequence arg3 = (ResultSequence) argiter.Current;
			ResultSequence arg4 = null;
			if (argiter.MoveNext())
            {
                arg4 = (ResultSequence) argiter.Current;
				string flags = arg4.first().StringValue;

				if (flags.Length == 0)
				{
					arg4 = null;
				}
				else if (isFlagValid(flags) == false)
				{
					throw new DynamicError("FORX0001", "Invalid regular expression. flags");
				}
			}
			string pattern = ((XSString) arg2.first()).value();
			string replacement = ((XSString) arg3.first()).value();

			try
			{
				return new XSString(str1.Replace(pattern, replacement));
			}
			catch (Exception)
			{
				throw new DynamicError("FORX0004", "invalid regex.");
			}
		}

		private static bool isFlagValid(string flag)
		{
			char[] flags = new char[] {'s', 'm', 'i', 'x'};

			for (int i = 0; i < flags.Length; i++)
			{
				if (flag.IndexOf(flags[i]) != -1)
				{
					return true;
				}
			}

			return false;
		}



		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnReplace))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}