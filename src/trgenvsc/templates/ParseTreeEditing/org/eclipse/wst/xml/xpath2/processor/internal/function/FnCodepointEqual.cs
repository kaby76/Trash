using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Steen Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using CollationProvider = org.eclipse.wst.xml.xpath2.api.CollationProvider;
	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// 
	/// <summary>
	/// <para>
	/// String comparison function.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:codepoint-equal($comparand1 as xs:string?,
	///                           $comparand2 as xs:string?) as xs:boolean?
	/// </para>
	/// 
	/// <para>
	/// Returns true or false depending on whether the value of $comparand1 is equal
	/// to the value of $comparand2, according to the Unicode code point collation
	/// (http://www.w3.org/2005/xpath-functions/collation/codepoint).
	/// </para>
	/// 
	/// <para>
	/// If either argument is the empty sequence, the result is the empty sequence.
	/// </para>
	/// 
	/// </summary>
	public class FnCodepointEqual : Function
	{

		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor of FnCodepointEqual.
		/// </summary>
		public FnCodepointEqual() : base(new QName("codepoint-equal"), 2)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            is evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the comparison of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return codepoint_equals(args, ec.DynamicContext);
		}

		/// <summary>
		/// Compare the arguments as codepoints
		/// </summary>
		/// <param name="args">
		///            are compared. </param>
		/// <param name="dynamicContext">
		///            The current dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of the comparison of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence codepoint_equals(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence codepoint_equals(ICollection args, DynamicContext dynamicContext)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultBuffer rs = new ResultBuffer();

			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;
			XSString xstr1 = arg1 == null || arg1.empty() ? null : (XSString) arg1.first();

            argiter.MoveNext();
            ResultSequence arg2 = (ResultSequence) argiter.Current;
			XSString xstr2 = arg2 == null || arg2.empty() ? null : (XSString) arg2.first();

			// This delegates to FnCompare
			System.Numerics.BigInteger result = FnCompare.compare_string(org.eclipse.wst.xml.xpath2.api.CollationProvider_Fields.CODEPOINT_COLLATION, xstr1, xstr2, dynamicContext);
			if (result != null)
			{
				rs.add(new XSBoolean(System.Numerics.BigInteger.Zero.Equals(result)));
			}

			return rs.Sequence;
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnCodepointEqual))
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