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
///     David Carver - bug 282096 - improvements for surrogate handling
///     David Carver - bug 262765 - improvements to Regular Expression   
///     Jesper Steen Moeller - bug 282096 - clean up string storage
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
	/// The function returns true if $input matches the regular expression supplied
	/// as $pattern as influenced by the value of $flags, if present; otherwise, it
	/// returns false.
	/// </summary>
	public class FnMatches : AbstractRegExFunction
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnMatches.
		/// </summary>
		public FnMatches() : base(new QName("matches"), 2, 3)
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
			return matches(args);
		}

		/// <summary>
		/// Matches operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:matches operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence matches(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence matches(ICollection args)
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
			string pattern = ((XSString) arg2.first()).value();
			string flags = null;

			if (argiter.MoveNext())
			{
				ResultSequence flagRS = null;
                flagRS = (ResultSequence) argiter.Current;
				flags = flagRS.first().StringValue;
				if (validflags.IndexOf(flags, StringComparison.Ordinal) == -1 && flags.Length > 0)
				{
					throw DynamicError.regex_flags_error(null);
				}
			}

			try
			{
				bool result = false;
				result = matches(pattern, flags, str1);
				return XSBoolean.valueOf(result);
			}
			catch (Exception pex)
			{
				throw DynamicError.regex_error(pex.Message);
			}
		}


		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnMatches))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}