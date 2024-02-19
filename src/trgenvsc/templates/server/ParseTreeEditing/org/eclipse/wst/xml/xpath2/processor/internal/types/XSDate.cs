using System;
using System.Diagnostics;
using java.time;

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
///     Mukul Gandhi - bug 274792 - improvements to xs:date constructor function.
///     David Carver - bug 282223 - implementation of xs:duration.
///                                 fixed casting issue. 
///     David Carver - bug 280547 - fix dates for comparison 
///     Jesper Steen Moller  - bug 262765 - fix type tests
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

    using Calendar = java.util.Calendar;
    using GregorianCalendar = java.util.GregorianCalendar;
    using TimeZone = java.util.TimeZone;
	using Duration = javax.xml.datatype.Duration;
    using XMLGregorianCalendar = javax.xml.datatype.XMLGregorianCalendar;

    using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using CmpGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpGt;
	using CmpLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpLt;
	using MathMinus = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathMinus;
	using MathPlus = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathPlus;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// Representation of a date of the form year-month-day and optional timezone
	/// </summary>
	public class XSDate : CalendarType, CmpEq, CmpLt, CmpGt, MathMinus, MathPlus, ICloneable
	{
		private const string XS_DATE = "xs:date";
        private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;

		/// <summary>
		/// Initializes a new representation of a supplied date
		/// </summary>
		/// <param name="cal">
		///            The Calendar representation of the date to be stored </param>
		/// <param name="tz">
		///            The time zone of the date to be stored. </param>
		public XSDate(Calendar cal, XSDuration tz)
		{
			_calendar = cal;

			_tz = tz;
			if (tz == null)
			{
				_timezoned = false;
			}
			else
			{
				_timezoned = true;
			}
		}

		/// <summary>
		/// Initializes a new representation of the current date
		/// </summary>
		public XSDate() : this(new GregorianCalendar(TimeZone.getTimeZone("GMT")), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype name
		/// </summary>
		/// <returns> "date" which is the dataype name </returns>
		public override string type_name()
		{
			return "date";
		}

		/// <summary>
		/// Creates a copy of this date representation
		/// </summary>
		/// <returns> A copy of this date representation </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
            Calendar c = (Calendar) calendar().clone();
			XSDuration t = tz();

			if (t != null)
			{
				t = (XSDuration) t.clone();
			}

			return new XSDate(c, t);
		}

		/// <summary>
		/// Parses a String representation of a date (of the form year-month-day or
		/// year-month-day+timezone) and constructs a new XSDate representation of
		/// it.
		/// </summary>
		/// <param name="str">
		///            The String representation of the date (and optional timezone) </param>
		/// <returns> The XSDate representation of the supplied date </returns>
		public static XSDate parse_date(string str)
		{

			string date = "";
			string time = "T00:00:00.0";

			int index = str.IndexOf('+', 1);
			if (index == -1)
			{
				index = str.IndexOf('-', 1);
				if (index == -1)
				{
					return null;
				}
				index = str.IndexOf('-', index + 1);
				if (index == -1)
				{
					return null;
				}
				index = str.IndexOf('-', index + 1);
			}
			if (index == -1)
			{
				index = str.IndexOf('Z', 1);
			}
			if (index != -1)
			{
				date = str.Substring(0, index);
				// here we go
				date += time;
				date += str.Substring(index, str.Length - index);
			}
			else
			{
				date = str + time;
			}

			// sorry again =D
			XSDateTime dt = XSDateTime.parseDateTime(date);
			if (dt == null)
			{
				return null;
			}

			return new XSDate(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new result sequence consisting of the retrievable date value in
		/// the supplied result sequence
		/// </summary>
		/// <param name="arg">
		///            The result sequence from which to extract the date value. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> A new result sequence consisting of the date value supplied. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			Item aat = arg.first();

			if (!isCastable(aat))
			{
				throw DynamicError.invalidType();
			}

			XSDate dt = castDate(aat);

			if (dt == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return dt;
		}

		private bool isCastable(Item aat)
		{

			// We might be able to cast these.
			if (aat is XSString || aat is XSUntypedAtomic || aat is NodeType)
			{
				return true;
			}

			if (aat is XSTime)
			{
				return false;
			}

			if (aat is XSDateTime)
			{
				return true;

			}

			if (aat is XSDate)
			{
				return true;
			}

			return false;
		}

		private XSDate castDate(Item aat)
		{
			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSDate(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSDate(dateTime.calendar(), dateTime.tz());
			}

			return parse_date(aat.StringValue);
		}

		/// <summary>
		/// Retrieve the year from the date stored
		/// </summary>
		/// <returns> the year value of the date stored </returns>
		public virtual int year()
		{
			int y = _calendar.get(Calendar.YEAR);
			if (_calendar.get(Calendar.ERA) == GregorianCalendar.BC)
			{
				y *= -1;
			}

			return y;
		}

		/// <summary>
		/// Retrieve the month from the date stored
		/// </summary>
		/// <returns> the month value of the date stored </returns>
		public virtual int month()
		{
			return _calendar.get(Calendar.MONTH) + 1;
		}

		/// <summary>
		/// Retrieve the day from the date stored
		/// </summary>
		/// <returns> the day value of the date stored </returns>
		public virtual int day()
		{
			return _calendar.get(Calendar.DAY_OF_MONTH);
		}

		/// <summary>
		/// Retrieves whether this date has an optional timezone associated with it
		/// </summary>
		/// <returns> True if there is a timezone associated with this date. False
		///         otherwise. </returns>
		public virtual bool timezoned()
		{
			return _timezoned;
		}

		/// <summary>
		/// Retrieves a String representation of the date stored
		/// </summary>
		/// <returns> String representation of the date stored </returns>
		public override string StringValue
		{
			get
			{
				string ret = "";
    
				Calendar adjustFortimezone = calendar();
    
				if (adjustFortimezone.get(Calendar.ERA) == GregorianCalendar.BC)
				{
					ret += "-";
				}
    
				ret += XSDateTime.pad_int(adjustFortimezone.get(Calendar.YEAR), 4);

                ret += "-";
                ret += XSDateTime.pad_int(month(), 2);

				ret += "-";
                ret += XSDateTime.pad_int(adjustFortimezone.get(Calendar.DAY_OF_MONTH),
                    2);

				if (timezoned())
				{
					int hrs = _tz.hours();
					int min = _tz.minutes();
					double secs = _tz.seconds();
					if (hrs == 0 && min == 0 && secs == 0)
					{
						ret += "Z";
					}
					else
					{
						string tZoneStr = "";
						if (_tz.negative())
						{
							tZoneStr += "-";
						}
						else
						{
							tZoneStr += "+";
						}
						tZoneStr += XSDateTime.pad_int(hrs, 2);
						tZoneStr += ":";
						tZoneStr += XSDateTime.pad_int(min, 2);
    
						ret += tZoneStr;
					}
				}
    
				return ret;
			}
		}

		/// <summary>
		/// Retrive the datatype full pathname
		/// </summary>
		/// <returns> "xs:date" which is the datatype full pathname </returns>
		public override string string_type()
		{
			return XS_DATE;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the date stored
		/// </summary>
		/// <returns> Calendar representation of the date stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Retrieves the timezone associated with the date stored
		/// </summary>
		/// <returns> the timezone associated with the date stored </returns>
		public virtual XSDuration tz()
		{
			return _tz;
		}

		// comparisons
		/// <summary>
		/// Equality comparison on this and the supplied dates (taking timezones into
		/// account)
		/// </summary>
		/// <param name="arg">
		///            XSDate representation of the date to compare to </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> True if the two dates are represent the same exact point in time.
		///         False otherwise. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSDate val = (XSDate) NumericType.get_single_type((Item)arg, typeof(XSDate));
			Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.Equals(thatcal);
		}

		/// <summary>
		/// Comparison on this and the supplied dates (taking timezones into account)
		/// </summary>
		/// <param name="arg">
		///            XSDate representation of the date to compare to </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> True if in time, this date lies before the date supplied. False
		///         otherwise. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			XSDate val = (XSDate) NumericType.get_single_type((Item)arg, typeof(XSDate));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.CompareTo(thatcal) < 0;
		}

		/// <summary>
		/// Comparison on this and the supplied dates (taking timezones into account)
		/// </summary>
		/// <param name="arg">
		///            XSDate representation of the date to compare to </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> True if in time, this date lies after the date supplied. False
		///         otherwise. </returns>
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			XSDate val = (XSDate) NumericType.get_single_type((Item)arg, typeof(XSDate));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.CompareTo(thatcal) > 0;
		}

		// XXX this is incorrect [epoch]
		/// <summary>
		/// Currently unsupported method. Retrieves the date in milliseconds since
		/// the begining of epoch
		/// </summary>
		/// <returns> Number of milliseconds since the begining of the epoch </returns>
		public virtual double value()
		{
			return calendar().getTimeInMillis() / 1000.0;
		}

		// math
		/// <summary>
		/// Mathematical minus operator between this XSDate and a supplied result
		/// sequence (XSDate, XSYearMonthDuration and XSDayTimeDuration are only
		/// valid ones).
		/// </summary>
		/// <param name="arg">
		///            The supplied ResultSequence that is on the right of the minus
		///            operator. If this is an XSDate, the result will be a
		///            XSDayTimeDuration of the duration of time between these two
		///            dates. If arg is an XSYearMonthDuration or an
		///            XSDayTimeDuration the result will be a XSDate of the result of
		///            the current date minus the duration of time supplied. </param>
		/// <returns> New ResultSequence consisting of the result of the mathematical
		///         minus operation. </returns>
		public virtual ResultSequence minus(ResultSequence arg)
		{
			if (arg.size() != 1)
			{
				throw DynamicError.throw_type_error();
			}

			Item at = arg.first();

			if (!(at is XSDate) && !(at is XSYearMonthDuration) && !(at is XSDayTimeDuration))
			{
				throw DynamicError.throw_type_error();
			}

			if (at is XSDate)
			{
				return minusXSDate(arg);
			}

			if (at is XSYearMonthDuration)
			{
				return minusXSYearMonthDuration((XSYearMonthDuration)at);
			}

			if (at is XSDayTimeDuration)
			{
				return minusXSDayTimeDuration((XSDayTimeDuration)at);
			}

			return null;
		}

		private ResultSequence minusXSDayTimeDuration(AnyType at)
		{
			XSDuration val = (XSDuration) at;

			try
			{
				XSDate res = (XSDate) clone();
				XMLGregorianCalendar xmlCal = _datatypeFactory
                    .newXMLGregorianCalendar(
                        (GregorianCalendar) calendar());
				Duration dtduration = _datatypeFactory.newDuration(val.StringValue);
				xmlCal.add(dtduration.negate());
				res = new XSDate(xmlCal.toGregorianCalendar(), res.tz());
				return ResultSequenceFactory.create_new(res);
			}
			catch 
			{
			}
			return null;
		}

		private ResultSequence minusXSYearMonthDuration(AnyType at)
		{
			XSYearMonthDuration val = (XSYearMonthDuration) at;
			try
			{
				XSDate res = (XSDate) clone();

                res.calendar().add(Calendar.MONTH, val.monthValue() * -1);
				return ResultSequenceFactory.create_new(res);
			}
			catch 
			{

			}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.eclipse.wst.xml.xpath2.api.ResultSequence minusXSDate(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private ResultSequence minusXSDate(ResultSequence arg)
		{
			XSDate val = (XSDate) NumericType.get_single_type(arg, typeof(XSDate));
			Duration dtduration = null;
			Calendar thisCal = normalizeCalendar(calendar(), tz());
            Calendar thatCal = normalizeCalendar(val.calendar(), val.tz());
            long duration = thisCal.getTimeInMillis()
                            - thatCal.getTimeInMillis();
			dtduration = _datatypeFactory.newDuration(duration);
			return ResultSequenceFactory.create_new(XSDayTimeDuration.parseDTDuration(dtduration.ToString()));
		}

		/// <summary>
		/// Mathematical addition operator between this XSDate and a supplied result
		/// sequence (XDTYearMonthDuration and XDTDayTimeDuration are only valid
		/// ones).
		/// </summary>
		/// <param name="arg">
		///            The supplied ResultSequence that is on the right of the minus
		///            operator. If arg is an XDTYearMonthDuration or an
		///            XDTDayTimeDuration the result will be a XSDate of the result
		///            of the current date minus the duration of time supplied. </param>
		/// <returns> New ResultSequence consisting of the result of the mathematical
		///         minus operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence plus(ResultSequence arg)
		{
			if (arg.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = arg.first();

			try
			{
				if (at is XSYearMonthDuration)
				{
					XSYearMonthDuration val = (XSYearMonthDuration) at;

					XSDate res = (XSDate) clone();

                    res.calendar().add(Calendar.MONTH, val.monthValue());
					return ResultSequenceFactory.create_new(res);
				}
				else if (at is XSDayTimeDuration)
				{
					XSDayTimeDuration val = (XSDayTimeDuration) at;

					XSDate res = (XSDate) clone();

					// We only need to add the Number of days dropping the rest.
					int days = val.days();
					if (val.negative())
					{
						days *= -1;
					}
                    res.calendar().add(Calendar.DAY_OF_MONTH, days);

                    res.calendar().add(Calendar.MILLISECOND,
                        (int)(val.time_value() * 1000.0));
                    return ResultSequenceFactory.create_new(res);
				}
				else
				{
					DynamicError.throw_type_error();
					return null; // unreach
				}
			}
			catch 
			{
				Debug.Assert(false);
				return null;
			}

		}
		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_DATE;
			}
		}

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

}