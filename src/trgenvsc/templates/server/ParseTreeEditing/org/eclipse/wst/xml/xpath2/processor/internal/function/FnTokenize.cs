using System;
using System.Collections;
using System.Text.RegularExpressions;

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
///     Jesper Steen Moeller - bug 282096 - clean up string storage
///     Jesper S Moller      - Bug 281938 - no matches should return full input 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// This function breaks the $input string into a sequence of strings, treating
	/// any substring that matches $pattern as a separator. The separators themselves
	/// are not returned.
	/// </summary>
	public class FnTokenize : AbstractRegExFunction
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnTokenize.
		/// </summary>
		public FnTokenize() : base(new QName("tokenize"), 2, 3)
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
			return tokenize(args);
		}

		/// <summary>
		/// Tokenize operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:tokenize operation. </returns>
		public static ResultSequence tokenize(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultBuffer rs = new ResultBuffer();

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
				ArrayList ret = tokenize(pattern, flags, str1);

				for (IEnumerator retIter = ret.GetEnumerator(); retIter.MoveNext();)
				{
				   rs.add(new XSString((string)retIter.Current));
				}

			}
			catch (Exception)
			{
				throw DynamicError.regex_error(null);
			}

			return rs.Sequence;
		}

		private static ArrayList tokenize(string pattern, string flags, string src)
		{
			MatchCollection matches = regex(pattern, flags, src);
			ArrayList tokens = new ArrayList();
			int startpos = 0;
			int endpos = src.Length;
			foreach (Match match in matches)
            {
                string delim = match.Groups[0].Value;
				if (delim.Length == 0)
				{
					throw DynamicError.regex_match_zero_length(null);
				}
				string token = src.Substring(startpos, match.Index- startpos);
                startpos = match.Index + match.Length;
				tokens.Add(token);
			}
			if (startpos < endpos)
			{
				string token = src.Substring(startpos, endpos - startpos);
				tokens.Add(token);
			}
			return tokens;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnTokenize))
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