using System;
using System.Diagnostics;
using java.util;
using javax.xml.datatype;
using TimeZone = System.TimeZone;

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
///     David Carver - bug 282223 - implementation of xs:duration data type.
///                                 correction of casting to time. 
///     David Carver - bug 280547 - fix dates for comparison 
///     Jesper Steen Moller - bug 262765 - fix type tests
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{


	using Calendar = java.util.Calendar;

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
	/// A representation of the Time datatype
	/// </summary>
	public class XSTime : CalendarType, CmpEq, CmpLt, CmpGt, MathMinus, MathPlus, ICloneable
	{

		private const string XS_TIME = "xs:time";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;

		/// <summary>
		/// Initialises to the supplied time and timezone
		/// </summary>
		/// <param name="cal">
		///            Calendar representation of the time to be stored </param>
		/// <param name="tz">
		///            The timezone (possibly null) associated with this time </param>
		public XSTime(Calendar cal, XSDuration tz)
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
		/// Initialises to the current time
		/// </summary>
		public XSTime() : this(new GregorianCalendar(TimeZone.CurrentTimeZone), null)
		{
		}

		/// <summary>
		/// Creates a new copy of the time (and timezone) stored
		/// </summary>
		/// <returns> New XSTime representing the copy of the time and timezone </returns>
		/// <exception cref="CloneNotSupportedException"> </exception>
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

			return new XSTime(c, t);
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "time" which is the datatype's name </returns>
		public override string type_name()
		{
			return "time";
		}

		/// <summary>
		/// Creates a new XSTime representing the String represented supplied time
		/// </summary>
		/// <param name="str">
		///            String represented time and timezone to be stored </param>
		/// <returns> New XSTime representing the supplied time </returns>
		public static CalendarType parse_time(string str)
		{

			string startdate = "1983-11-29T";

			XSDateTime dt = XSDateTime.parseDateTime(startdate + str);
			if (dt == null)
			{
				return null;
			}

			return new XSTime(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable time from the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to extract the time </param>
		/// <returns> New ResultSequence consisting of the supplied time </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			AnyAtomicType aat = (AnyAtomicType) arg.first();
			if (!isCastable(aat))
			{
				throw DynamicError.invalidType();
			}

			CalendarType t = castTime(aat);

			if (t == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return t;
		}

		private bool isCastable(AnyAtomicType aat)
		{
			if (aat is XSString || aat is XSUntypedAtomic)
			{
				return true;
			}

			if (aat is XSDateTime)
			{
				return true;
			}

			if (aat is XSTime)
			{
				return true;
			}
			return false;
		}

		private CalendarType castTime(AnyAtomicType aat)
		{
			if (aat is XSTime)
			{
				XSTime time = (XSTime) aat;
				return new XSTime(time.calendar(), time.tz());
			}
			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSTime(dateTime.calendar(), dateTime.tz());
			}

			return parse_time(aat.StringValue);
		}
		/// <summary>
		/// Retrieves the hour stored as an integer
		/// </summary>
		/// <returns> The hour stored </returns>
		public virtual int hour()
		{
			return _calendar.get(Calendar.HOUR_OF_DAY);
		}

		/// <summary>
		/// Retrieves the minute stored as an integer
		/// </summary>
		/// <returns> The minute stored </returns>
		public virtual int minute()
		{
			return _calendar.get(Calendar.MINUTE);
		}

		/// <summary>
		/// Retrieves the seconds stored as an integer
		/// </summary>
		/// <returns> The second stored </returns>
		public virtual double second()
		{
            double s = _calendar.get(Calendar.SECOND);

            double ms = _calendar.get(Calendar.MILLISECOND);

			ms /= 1000;

			s += ms;
			return s;
		}

		/// <summary>
		/// Check for whether the time stored has a timezone associated with it
		/// </summary>
		/// <returns> True if the time has a timezone associated. False otherwise </returns>
		public virtual bool timezoned()
		{
			return _timezoned;
		}

		/// <summary>
		/// Retrieves a String representation of the time stored
		/// </summary>
		/// <returns> String representation of the time stored </returns>
		public override string StringValue
		{
			get
			{
				string ret = "";
    
				Calendar adjustFortimezone = calendar();
                ret += XSDateTime.pad_int(adjustFortimezone.get(Calendar.HOUR_OF_DAY), 2);

                ret += ":";
                ret += XSDateTime.pad_int(adjustFortimezone.get(Calendar.MINUTE), 2);


				ret += ":";
				int isecond = (int) second();
				double sec = second();
    
				if ((sec - (isecond)) == 0.0)
				{
					ret += XSDateTime.pad_int(isecond, 2);
				}
				else
				{
					if (sec < 10.0)
					{
						ret += "0" + sec;
					}
					else
					{
						ret += sec;
					}
				}
    
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
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:time" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_TIME;
		}

		/// <summary>
		/// Retrieves a Calendar representation of time stored
		/// </summary>
		/// <returns> Calendar representation of the time stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Retrieves the timezone associated with the time stored as a duration of
		/// time
		/// </summary>
		/// <returns> The duration of time between the time stored and the actual time
		///         after the timezone is taken into account </returns>
		public virtual XSDuration tz()
		{
			return _tz;
		}

		/// <summary>
		/// Retrieves the time in milliseconds since the epoch
		/// </summary>
		/// <returns> time stored in milliseconds since the epoch </returns>
		public virtual double value()
		{
			return calendar().getTimeInMillis() / 1000.0;
		}

		/// <summary>
		/// Equality comparison between this and the supplied XSTime representation
		/// </summary>
		/// <param name="arg">
		///            The XSTime to compare with </param>
		/// <returns> True if both XSTime's represent the same time. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSTime val = (XSTime) NumericType.get_single_type(arg, typeof(XSTime));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.Equals(thatcal);
		}

		/// <summary>
		/// Comparison between this and the supplied XSTime representation
		/// </summary>
		/// <param name="arg">
		///            The XSTime to compare with </param>
		/// <returns> True if the supplied time represnts a point in time after that
		///         represented by the time stored. False otherwise </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			XSTime val = (XSTime) NumericType.get_single_type(arg, typeof(XSTime));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());
			return thiscal.IsLessThan(thatcal);
		}

		/// <summary>
		/// Comparison between this and the supplied XSTime representation
		/// </summary>
		/// <param name="arg">
		///            The XSTime to compare with </param>
		/// <returns> True if the supplied time represnts a point in time before that
		///         represented by the time stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			XSTime val = (XSTime) NumericType.get_single_type(arg, typeof(XSTime));
			Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.IsGreaterThan(thatcal);
		}

		/// <summary>
		/// Mathematical subtraction between this time stored and the supplied
		/// representation. This supplied representation must be of either type
		/// XSTime (in which case the result is the duration of time between these
		/// two times) or a XSDayTimeDuration (in which case the result is the time
		/// when this duration is subtracted from the time stored).
		/// </summary>
		/// <param name="arg">
		///            The representation to subtract (either XSTim or
		///            XDTDayTimeDuration) </param>
		/// <returns> A ResultSequence representing the result of the subtraction </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence minus(ResultSequence arg)
		{
			if (arg.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = arg.first();

			if (!(at is XSTime) && !(at is XSDayTimeDuration))
			{
				throw DynamicError.throw_type_error();
			}

			if (at is XSTime)
			{
				return minusXSTimeDuration(at);
			}

			if (at is XSDayTimeDuration)
			{
				return minusXSDayTimeDuration(at);
			}
			return null; // unreach
		}

		private ResultSequence minusXSDayTimeDuration(Item at)
		{
			XSDuration val = (XSDuration) at;

			XSTime res = null;
			try
			{
				res = (XSTime) clone();
			}
			catch 
			{
				return null;
			}

			XMLGregorianCalendar xmlCal = _datatypeFactory.newXMLGregorianCalendar((GregorianCalendar)calendar());
			Duration dtduration = _datatypeFactory.newDuration(val.StringValue);
			xmlCal.add(dtduration.negate());
			res = new XSTime(xmlCal.toGregorianCalendar(), res.tz());

			return ResultSequenceFactory.create_new(res);
		}

		private ResultSequence minusXSTimeDuration(Item at)
		{
			XSTime val = (XSTime) at;
			Duration dtduration = null;
			Calendar thisCal = normalizeCalendar(calendar(), tz());
            Calendar thatCal = normalizeCalendar(val.calendar(), val.tz());
			long duration = thisCal.getTimeInMillis() - thatCal.getTimeInMillis();
			dtduration = _datatypeFactory.newDuration(duration);
			return ResultSequenceFactory.create_new(XSDayTimeDuration.parseDTDuration(dtduration.ToString()));
		}

		/// <summary>
		/// Mathematical addition between this time stored and the supplied time
		/// duration.
		/// </summary>
		/// <param name="arg">
		///            A XDTDayTimeDuration representation of the duration of time to
		///            add </param>
		/// <returns> A XSTime representing the result of this addition. </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence plus(ResultSequence arg)
		{
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDayTimeDuration));

			try
			{
				double ms = val.time_value() * 1000.0;

				XSTime res = (XSTime) clone();

                res.calendar().add(Calendar.MILLISECOND, (int)ms);

				return ResultSequenceFactory.create_new(res);
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
				return BuiltinTypeLibrary.XS_TIME;
			}
		}

		public override object NativeValue
		{
			get
			{
				return _calendar.clone();
			}
		}

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

}