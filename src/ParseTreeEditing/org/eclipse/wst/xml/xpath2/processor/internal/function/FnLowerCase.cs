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
///     Jesper Steen Moeller - bug 282096 - clean up string storage
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Conversion to lower-case function.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:lower-case($arg as xs:string?) as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class returns the value of $arg after translating every character to its
	/// lower-case correspondent. Every character that does not have an lower-case
	/// correspondent is included in the returned value in its original form.
	/// </para>
	/// 
	/// <para>
	/// If the value of $arg is the empty sequence, the zero-length string is
	/// returned.
	/// </para>
	/// </summary>
	public class FnLowerCase : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnLowerCase.
		/// </summary>
		public FnLowerCase() : base(new QName("lower-case"), 1)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the arguments being converted to lower case. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return lower_case(args);
		}

		/// <summary>
		/// Convert arguments to lower case.
		/// </summary>
		/// <param name="args">
		///            are converted to lower case. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of converting the arguments to lower case. </returns>
		public static ResultSequence lower_case(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1 == null || arg1.empty())
			{
				return new XSString("");
			}

			string str = ((XSString) arg1.first()).value();

			return new XSString(str.ToLower());
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnLowerCase))
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