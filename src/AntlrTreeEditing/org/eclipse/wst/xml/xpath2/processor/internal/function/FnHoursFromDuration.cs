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
///     David Carver - bug 282223 - implementation of xs:duration
///     David Carver (STAR) - bug 262765 - fixed expected xs:duration argument. 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;

	/// <summary>
	/// Returns an xs:integer representing the hours component in the canonical
	/// lexical representation of the value of $arg. The result may be negative. If
	/// $arg is the empty sequence, returns the empty sequence.
	/// </summary>
	public class FnHoursFromDuration : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnHoursFromDuration.
		/// </summary>
		public FnHoursFromDuration() : base(new QName("hours-from-duration"), 1)
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
			return hours_from_duration(args);
		}

		/// <summary>
		/// Hours-from-Duration operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:hours-from-duration operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence hours_from_duration(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence hours_from_duration(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1 == null || arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

			XSDuration dtd = (XSDuration) arg1.first();

			int res = dtd.hours();

			if (dtd.negative())
			{
				res *= -1;
			}

			return new XSInteger(new System.Numerics.BigInteger(res));
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnHoursFromDuration))
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