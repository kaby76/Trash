using System.Diagnostics;
using System.Collections;
using System.Text;

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
///     Mukul Gandhi - bug 274471 - improvements to normalize-space function (support for arity 0)
///     David Carver (STAR) - bug 262765 - correct implementation to correctly get context node 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moller  - bug 281938 - handle context and empty sequences correctly
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Function to normalize whitespace.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:normalize-space($arg as xs:string?) as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class returns the value of $arg with whitespace normalized by stripping
	/// leading and trailing whitespace and replacing sequences of one or more than
	/// one whitespace character with a single space, #x20.
	/// </para>
	/// 
	/// <para>
	/// The whitespace characters are defined as TAB (#x9), LINE FEED (#xA), CARRIAGE
	/// RETURN (#xD) and SPACE (#x20). If the value of $arg is the empty sequence,
	/// the class returns the zero-length string.
	/// </para>
	/// </summary>
	public class FnNormalizeSpace : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnNormalizeSpace.
		/// </summary>
		public FnNormalizeSpace() : base(new QName("normalize-space"), 0, 1)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the space in the arguments being normalized. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return normalize_space(args, ec);
		}

		/// <summary>
		/// Normalize space in the arguments.
		/// </summary>
		/// <param name="args">
		///            are used to obtain space from, in order to be normalized. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of normalizing the space in the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence normalize_space(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence normalize_space(ICollection args, EvaluationContext ec)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultSequence arg1 = null;

			if (cargs.Count == 0)
			{
			  // support for arity = 0
			  arg1 = getResultSetForArityZero(ec);
			}
			else
            {
                var i = cargs.GetEnumerator();
                i.MoveNext();
                arg1 = (ResultSequence) i.Current;
            }

			string str = "";
			if (!arg1.empty())
			{
				str = ((XSString) arg1.first()).value();
			}
			return new XSString(normalize(str));
		}

		/// <summary>
		/// The normalizing process.
		/// </summary>
		/// <param name="str">
		///            is the string that space will be normalized in. </param>
		/// <returns> The result of the normalizing operation. </returns>
		// XXX fix this
		public static string normalize(string str)
		{
			StringBuilder sb = new StringBuilder();

			int state = 0; // 0 begin
			// 1 middle
			// 2 end
			// 3 skipping

			for (int i = 0; i < str.Length; i++)
			{
				char x = str[i];

				bool white = is_whitespace(x);

				switch (state)
				{
				// doing the beginning
				case 0:
					if (white)
					{
						continue;
					}
					else
					{
						sb.Append(x);
						state = 1;
					}
					break;

				// doing the middle
				case 1:
					if (white)
					{
						state = 3;
						sb.Append(' ');
					}
					else
					{
						sb.Append(x);
					}
					break;

				case 3:
					if (!white)
					{
						state = 1;
						sb.Append(x);
					}
					break;

				default:
					Debug.Assert(false);
				break;
				}
			}

			// now basically we can only have a whitespace at the end...
			string result = sb.ToString();
			int len = result.Length;

			if (len == 0)
			{
				return result;
			}
			if (result[len - 1] == ' ')
			{
				return result.Substring(0, len - 1);
			}

			return result;
		}

		/// <summary>
		/// Determine whether a character is whitespace or not.
		/// </summary>
		/// <param name="x">
		///            is the character this operation will take place on. </param>
		/// <returns> Whether or not the character is whitespace. </returns>
		public static bool is_whitespace(char x)
		{
			switch (x)
			{
			case ' ':
			case '\r':
			case '\t':
			case '\n':
				return true;
			default:
				return false;
			}
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnNormalizeSpace))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}