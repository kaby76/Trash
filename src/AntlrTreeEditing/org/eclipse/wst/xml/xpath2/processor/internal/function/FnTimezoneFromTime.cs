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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSTime;

	/// <summary>
	/// Returns the timezone component of $arg if any. If $arg has a timezone
	/// component, then the result is an xdt:dayTimeDuration that indicates deviation
	/// from UTC; its value may range from +14:00 to -14:00 hours, both inclusive.
	/// Otherwise, the result is the empty sequence. If $arg is the empty sequence,
	/// returns the empty sequence.
	/// </summary>
	public class FnTimezoneFromTime : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnTimezoneFromTime.
		/// </summary>
		public FnTimezoneFromTime() : base(new QName("timezone-from-time"), 1)
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
			return timezone_from_time(args);
		}

		/// <summary>
		/// Timezone-from-Time operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:timezone-from-time operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence timezone_from_time(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence timezone_from_time(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1 == null || arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

			XSTime dt = (XSTime) arg1.first();

			if (dt.timezoned())
			{
				return dt.tz();
			}
			return ResultBuffer.EMPTY;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnTimezoneFromTime))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSTime(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}