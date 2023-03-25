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
	using XSDate = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDate;
	using XSDayTimeDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDayTimeDuration;

	/// <summary>
	/// Adjusts an xs:date value to a specific timezone, or to no timezone at
	/// all. If <code>$timezone</code> is the empty sequence, returns an
	/// <code>xs:date</code> without timezone. Otherwise, returns an
	/// <code>xs:date</code> with a timezone.
	/// </summary>
	public class FnAdjustDateToTimeZone : Function
	{
		private static ArrayList _expected_args = null;
		private static readonly XSDayTimeDuration minDuration = new XSDayTimeDuration(0, 14, 0, 0, true);
		private static readonly XSDayTimeDuration maxDuration = new XSDayTimeDuration(0, 14, 0, 0, false);

		/// <summary>
		/// Constructor for FnDateTime.
		/// </summary>
		public FnAdjustDateToTimeZone() : base(new QName("adjust-date-to-timezone"), 1, 2)
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
			return adjustDate(args, ec.DynamicContext);
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
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence adjustDate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence adjustDate(ICollection args, DynamicContext dc)
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
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                arg2 = (ResultSequence) argiter.Current;
            }

			XSDate date = (XSDate) arg1.item(0);
			XSDayTimeDuration timezone = null;

			if (arg2.empty())
			{
				if (date.timezoned())
				{
					XSDate localized = new XSDate(date.calendar(), null);
					return localized;
				}
				return arg1;
			}

			timezone = (XSDayTimeDuration) arg2.item(0);
			if (timezone.lt(minDuration, dc) || timezone.gt(maxDuration, dc))
			{
				throw DynamicError.invalidTimezone();
			}

			if (date.tz() == null)
			{
				return new XSDate(date.calendar(), timezone);
			}

			XMLGregorianCalendar xmlCalendar = _datatypeFactory.newXMLGregorianCalendar((GregorianCalendar)date.normalizeCalendar(date.calendar(), date.tz()));

			Duration duration = _datatypeFactory.newDuration(timezone.StringValue);
			xmlCalendar.add(duration);

			return new XSDate(xmlCalendar.toGregorianCalendar(), timezone);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expectedArgs()
		{
			lock (typeof(FnAdjustDateToTimeZone))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSDate(), SeqType.OCC_QMARK));
					_expected_args.Add(new SeqType(new XSDayTimeDuration(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}