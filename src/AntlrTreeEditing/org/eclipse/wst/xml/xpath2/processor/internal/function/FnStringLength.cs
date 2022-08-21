using System.Collections;
using System.Numerics;

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
///     Mukul Gandhi - bug 274471 - improvements to string-length function (support for arity 0)
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type  
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     David Carver - bug 282096 - improvements for surrogate handling 
///     Jesper Steen Moeller - bug 282096 - clean up string storage
///     Jesper Steen Moller  - bug 281938 - handle context and empty sequences correctly
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Function to calculate string length.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:string-length($arg as xs:string?) as xs:integer
	/// </para>
	/// 
	/// <para>
	/// This class returns an xs:integer equal to the length in characters of the
	/// value of $arg.
	/// </para>
	/// 
	/// <para>
	/// If the value of $arg is the empty sequence, the xs:integer 0 is returned.
	/// </para>
	/// </summary>
	public class FnStringLength : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnStringLength
		/// </summary>
		public FnStringLength() : base(new QName("string-length"), 0, 1)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the string length of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return string_length(args, ec);
		}

		/// <summary>
		/// Obtain the string length of the arguments.
		/// </summary>
		/// <param name="args">
		///            are used to obtain the string length. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of obtaining the string length from the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence string_length(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence string_length(ICollection args, EvaluationContext ec)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultSequence arg1 = null;

			if (cargs.Count == 0)
			{
			  // support for arity = 0
			  return getResultSetForArityZero(ec);
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

            BigInteger.TryParse(str, out BigInteger v);
			return new XSInteger(v);
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnStringLength))
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