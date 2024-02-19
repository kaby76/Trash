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
///     Mukul Gandhi - bug 273760 - wrong namespace for functions and data types
///     David Carver - bug 277774 - XSDecimal returning wrong values.
///     David Carver - bug 282223 - implementation of xs:duration
///     David Carver (STAR) - bug 262765 - fixed xs:duration expected argument. 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDecimal = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDecimal;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;

	/// <summary>
	/// Returns an xs:decimal representing the seconds component in the canonical
	/// lexical representation of the value of $arg. The result may be negative. If
	/// $arg is the empty sequence, returns the empty sequence.
	/// </summary>
	public class FnSecondsFromDuration : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnSecondsFromDuration.
		/// </summary>
		public FnSecondsFromDuration() : base(new QName("seconds-from-duration"), 1)
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
			return seconds_from_duration(args);
		}

		/// <summary>
		/// Seconds-from-Duration operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:seconds-from-duration operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence seconds_from_duration(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence seconds_from_duration(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());
            var i = cargs.GetEnumerator();
            i.MoveNext();
			ResultSequence arg1 = (ResultSequence)i.Current;

			if (arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

			XSDuration dtd = (XSDuration) arg1.first();

			double res = dtd.seconds();

			if (dtd.negative())
			{
				res *= -1;
			}

			return new XSDecimal(new decimal(res));
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnSecondsFromDuration))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSDuration(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}