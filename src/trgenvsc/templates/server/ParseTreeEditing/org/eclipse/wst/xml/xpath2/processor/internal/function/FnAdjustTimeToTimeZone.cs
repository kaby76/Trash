using System.Collections;
using java.util;
using javax.xml.datatype;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver - bug 280547 - initial API and implementation. 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{



	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDayTimeDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDayTimeDuration;
	using XSTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSTime;

	/// <summary>
	/// Adjusts an xs:dateTime value to a specific timezone, or to no timezone at
	/// all. If <code>$timezone</code> is the empty sequence, returns an
	/// <code>xs:dateTime</code> without timezone. Otherwise, returns an
	/// <code>xs:dateTime</code> with a timezone.
	/// </summary>
	public class FnAdjustTimeToTimeZone : Function
	{

		private static ArrayList _expected_args = null;
		private static readonly XSDayTimeDuration minDuration = new XSDayTimeDuration(0, 14, 0, 0, true);
		private static readonly XSDayTimeDuration maxDuration = new XSDayTimeDuration(0, 14, 0, 0, false);

		/// <summary>
		/// Constructor for FnDateTime.
		/// </summary>
		public FnAdjustTimeToTimeZone() : base(new QName("adjust-time-to-timezone"), 1, 2)
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
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return adjustTime(args, ec.DynamicContext);
		}

		/// <summary>
		/// Evaluate the function using the arguments passed.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="sc">
		///            Result of static context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the fn:dateTime operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence adjustTime(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence adjustTime(ICollection args, DynamicContext dc)
		{

			ICollection cargs = Function.convert_arguments(args, expectedArgs());

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;
			if (arg1 == null || arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}
			ResultSequence arg2 = ResultBuffer.EMPTY;
			if (argiter.MoveNext())
			{
                arg2 = (ResultSequence) argiter.Current;
            }
			XSTime time = (XSTime) arg1.first();
			XSDayTimeDuration timezone = null;

			if (arg2.empty())
			{
				if (time.timezoned())
				{
					XSTime localized = new XSTime(time.calendar(), null);
					return localized;
				}
				else
				{
					return arg1;
				}
			}


			XMLGregorianCalendar xmlCalendar = null;

			if (time.tz() != null)
			{
				xmlCalendar = _datatypeFactory.newXMLGregorianCalendar((GregorianCalendar)time.normalizeCalendar(time.calendar(), time.tz()));
			}
			else
			{
				xmlCalendar = _datatypeFactory.newXMLGregorianCalendarTime(time.hour(), time.minute(), (int)time.second(), 0);
			}

			timezone = (XSDayTimeDuration) arg2.first();
			if (timezone.lt(minDuration, dc) || timezone.gt(maxDuration, dc))
			{
				throw DynamicError.invalidTimezone();
			}

			if (time.tz() == null)
			{
				return new XSTime(time.calendar(), timezone);
			}

			Duration duration = _datatypeFactory.newDuration(timezone.StringValue);
			xmlCalendar.add(duration);

			return new XSTime(xmlCalendar.toGregorianCalendar(), timezone);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expectedArgs()
		{
			lock (typeof(FnAdjustTimeToTimeZone))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSTime(), SeqType.OCC_QMARK));
					_expected_args.Add(new SeqType(new XSDayTimeDuration(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}