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
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSTime;

	/// <summary>
	/// Returns an xs:integer value between 0 to 59, both inclusive, representing the
	/// value of the minutes component in the localized value of $arg. If $arg is the
	/// empty sequence, returns the empty sequence.
	/// </summary>
	public class FnMinutesFromTime : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnMinutesFromTime.
		/// </summary>
		public FnMinutesFromTime() : base(new QName("minutes-from-time"), 1)
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
			return minutes_from_time(args);
		}

		/// <summary>
		/// Minutes-from-Time operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:minutes-from-time operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence minutes_from_time(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence minutes_from_time(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

			XSTime dt = (XSTime) arg1.first();

			int res = dt.minute();

			return new XSInteger(new System.Numerics.BigInteger(res));
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnMinutesFromTime))
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