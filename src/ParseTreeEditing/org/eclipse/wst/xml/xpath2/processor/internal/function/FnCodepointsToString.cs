using System;
using System.Collections;
using System.Linq;

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
///     Mukul Gandhi - improvements to the function implementation
///     David Carver - bug 282096 - improvements for surrogate handling 
///     Jesper Steen Moeller - bug 282096 - clean up string storage
///     Jesper Steen Moeller - bug 280553 - further checks of legal Unicode codepoints.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	//using UTF16 = com.ibm.icu.text.UTF16;

	/// <summary>
	/// Creates an xs:string from a sequence of code points. Returns the zero-length
	/// string if $arg is the empty sequence. If any of the code points in $arg is
	/// not a legal XML character, an error is raised [err:FOCH0001].
	/// </summary>
	public class FnCodepointsToString : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// The maximum value of a Unicode code point.
		/// </summary>
		public const int MIN_LEGAL_CODEPOINT = 0x1;


		/// <summary>
		/// The maximum value of a Unicode code point.
		/// </summary>
		public const int MAX_LEGAL_CODEPOINT = 0x10ffff;

		/// <summary>
		/// Constructor for FnCodepointsToString.
		/// </summary>
		public FnCodepointsToString() : base(new QName("codepoints-to-string"), 1)
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
			return codepoints_to_string(args);
		}

		/// <summary>
		/// Codepoints to string operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:codepoints-to-string operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence codepoints_to_string(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence codepoints_to_string(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());
            var j = cargs.GetEnumerator();
            j.MoveNext();
            ResultSequence arg1 = (ResultSequence) j.Current;
			if (arg1 == null || arg1.empty())
			{
				return new XSString("");
			}

			int[] codePointArray = new int[arg1.size()];
			int codePointIndex = 0;
			for (var i = arg1.iterator(); i.MoveNext();)
			{
				XSInteger code = (XSInteger) i.Current;

				int codepoint = (int)code.int_value();
				if (codepoint < MIN_LEGAL_CODEPOINT || codepoint > MAX_LEGAL_CODEPOINT)
				{
					throw DynamicError.unsupported_codepoint("U+" + Convert.ToString(codepoint, 16).ToUpper());
				}

				codePointArray[codePointIndex] = codepoint;
				codePointIndex++;
			}

			try
            {
                var c = codePointArray.Select(x => (char) x).ToArray();
                string str = new string(c);
				return new XSString(str);
			}
			catch (System.ArgumentException iae)
			{
				// This should be duoble checked above, but rather safe than sorry
				throw DynamicError.unsupported_codepoint(iae.Message);
			}
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnCodepointsToString))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSInteger(), SeqType.OCC_STAR));
				}
        
				return _expected_args;
			}
		}
	}

}