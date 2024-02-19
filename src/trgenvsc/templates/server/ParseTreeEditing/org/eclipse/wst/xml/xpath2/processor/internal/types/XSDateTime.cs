
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
///     Mukul Gandhi - improved string_value() implementation (motivated by bug, 281822)
///     David Carver - bug 282223 - implementation of xs:duration.
///                  - bug 262765 - additional tweak to convert 24:00:00 to 00:00:00
///     David Carver - bug 280547 - fix dates for comparison 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

using System;
using System.Diagnostics;
using System.Text;

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
	/// A representation of a date and time (and optional timezone)
	/// </summary>
	public class XSDateTime : CalendarType, CmpEq, CmpLt, CmpGt, MathMinus, MathPlus, ICloneable
	{
		private const string XS_DATE_TIME = "xs:dateTime";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;

		/// <summary>
		/// Initiates a new representation of a supplied date and time
		/// </summary>
		/// <param name="cal">
		///            The Calendar representation of the date and time to be stored </param>
		/// <param name="tz">
		///            The timezone of the date to be stored. </param>
		public XSDateTime(Calendar cal, XSDuration tz)
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
		/// Creates a copy of this date and time representation
		/// </summary>
		/// <returns> A copy of this date and time representation </returns>
		public virtual object clone()
		{
            Calendar c = (Calendar) calendar().clone();
			XSDuration t = tz();

			if (t != null)
			{
				t = (XSDuration) t.clone();
			}

			return new XSDateTime(c, t);
		}

		/// <summary>
		/// Inititates a new representation of the current date and time
		/// </summary>
		public XSDateTime() : this(new GregorianCalendar(), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype name
		/// </summary>
		/// <returns> "dateTime" which is the dataype name </returns>
		public override string type_name()
		{
			return "dateTime";
		}

		/// <summary>
		/// Check to see if a character is numeric
		/// </summary>
		/// <param name="x">
		///            Character to be tested </param>
		/// <returns> True if the character is numeric. False otherwise. </returns>
		public static bool is_digit(char x)
		{
			if ('0' <= x && x <= '9')
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parses a String representation of a date and time and retrieves the year,
		/// month and day from it
		/// </summary>
		/// <param name="str">
		///            The String representation of the date (and optional timezone) </param>
		/// <returns> Integer array of size 3. Element 1 is the year, element 2 is the
		///         month and element 3 is the day </returns>
		public static int[] parse_date(string str)
		{
			int state = 0; // 0 expect year or minus
			// 1 getting year
			// 2 getting month
			// 3 getting day

			int[] ret = new int[3];

			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = 0;
			}

			string token = "";
			for (int i = 0; i < str.Length; i++)
			{
				char x = str[i];

				switch (state)
				{
				case 0:
					if (is_digit(x))
					{
						token += x;
					}
					else if (x == '-')
					{
						token += x;
					}
					else
					{
						return null;
					}
					state = 1;
					break;

				case 1:
					// we got the year in theory...
					if (x == '-')
					{

						// check out the unsigned year
						string uy = token;
						if (uy.StartsWith("-", StringComparison.Ordinal))
						{
							uy = uy.Substring(1, uy.Length - 1);
						}
						int uyl = uy.Length;

						if (uyl < 4)
						{
							return null;
						}

						if (uyl == 4)
						{
							if (uy.CompareTo("0000") == 0)
							{
								return null;
							}
						}
						else if (uy[0] == '0')
						{
							return null;
						}

						// semms good to me
						ret[0] = int.Parse(token);
						token = "";
						state = 2;
					}
					else if (is_digit(x))
					{
						token += x;
					}
					else
					{
						return null;
					}
					break;

				case 2:
					// we got the month
					if (x == '-')
					{
						if (token.Length != 2)
						{
							return null;
						}

						ret[1] = int.Parse(token);
						token = "";
						state = 3;
					}
					else if (is_digit(x))
					{
						token += x;
					}
					else
					{
						return null;
					}
					break;

				case 3:
					// getting day
					if (is_digit(x))
					{
						token += x;
					}
					else
					{
						return null;
					}
					break;

				default:
					Debug.Assert(false);
					return ret;
				}
			}
			if (state != 3)
			{
				return null;
			}

			// got the day
			if (token.Length != 2)
			{
				return null;
			}

			ret[2] = int.Parse(token);

			return ret;
		}

		// return
		// hour
		// minute
		// seconds
		/// <summary>
		/// Parses a String representation of a date and time and retrieves the hour,
		/// minute and seconds from it
		/// </summary>
		/// <param name="str">
		///            The String representation of the date (and optional timezone) </param>
		/// <returns> Integer array of size 3. Element 1 is the hour, element 2 is the
		///         minute and element 3 is the seconds </returns>
		public static double[] parse_time(string str)
		{
			int state = 0; // 0 getting minute
			// 1 getting hour
			// 2 getting seconds [the whole part]
			// 3 getting fraction of seconds

			double[] ret = new double[3];

			string token = "";

			for (int i = 0; i < str.Length; i++)
			{
				char x = str[i];

				switch (state)
				{
				case 0:
				case 1:
					// got minute / hour
					if (x == ':')
					{
						if (token.Length != 2)
						{
							return null;
						}
						ret[state] = int.Parse(token);
						state++;
						token = "";
					}
					else if (is_digit(x))
					{
						token += x;
					}
					else
					{
						return null;
					}
					break;

				case 2:
					if (is_digit(x))
					{
						token += x;
						if (token.Length > 2)
						{
							return null;
						}
					}
					else if (x == '.')
					{
						token += x;
						state = 3;
					}
					else
					{
						return null;
					}
					break;

				case 3:
					if (is_digit(x))
					{
						token += x;
					}
					else
					{
						return null;
					}
					break;

				default:
					Debug.Assert(false);
					return null;
				}
			}
			if (!(state == 3 || state == 2))
			{
				return null;
			}

			// get seconds
			// this is whole + dot + nothing else
			if (token.Length == 3)
			{
				return null;
			}

			ret[2] = double.Parse(token);

			if (ret[0] == 24.0)
			{
				ret[0] = 0.0;
			}

			// XXX sanity check args...
			return ret;
		}

		// returns
		// positive/negative
		// hour
		// minute
		/// <summary>
		/// Parses a String representation of a date and time and retrieves the
		/// timezone from it
		/// </summary>
		/// <param name="str">
		///            The String representation of the date (and optional timezone) </param>
		/// <returns> Integer array of size 3. Element 1 represents whether the
		///         timezone is ahead or behind GMT, element 2 is the hour
		///         displacement and element 3 is the minute displacement. </returns>
		public static int[] parse_timezone(string str)
		{
			int[] ret = new int[3];

			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = 0;
			}
			ret[0] = 1;

			if (str.Equals("Z"))
			{
				return ret;
			}

			// get initial plus/minus
			if (str.StartsWith("+", StringComparison.Ordinal))
			{
				ret[0] = 1;
			}
			else if (str.StartsWith("-", StringComparison.Ordinal))
			{
				ret[0] = -1;
			}
			else
			{
				return null;
			}

			str = str.Substring(1, str.Length - 1);

			if (str.Length != (2 + 1 + 2))
			{
				return null;
			}

			try
			{
				ret[1] = int.Parse(str.Substring(0, 2));
				ret[2] = int.Parse(str.Substring(3, 2));

				// According to schema spec, timezone is limited to
				// this... [well.. almost...]
				if (ret[1] > 14)
				{
					return null;
				}
				if (ret[2] > 59)
				{
					return null;
				}

				return ret;
			}
			catch (System.FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Attempts to set a particular field in the Calendar
		/// </summary>
		/// <param name="cal">
		///            The Calendar object to set the field in </param>
		/// <param name="item">
		///            The field to set </param>
		/// <param name="val">
		///            The value to set the field to </param>
		/// <returns> True if successfully set. False otherwise (due to out of bounds
		///         for that field) </returns>
		private static bool set_item(Calendar cal, int item, int val)
		{
			int min = cal.getActualMinimum(item);

			if (val < min)
			{
				return false;
			}

			int max = cal.getActualMaximum(item);

			if (val > max)
			{
				return false;
			}

			cal.set(item, val);
			return true;
		}

		/// <summary>
		/// Parses a String representation of a date and time and constructs a new
		/// XSDateTime object using that information
		/// </summary>
		/// <param name="str">
		///            The String representation of the date (and optional timezone) </param>
		/// <returns> The XSDateTime representation of the date and time (and optional
		///         timezone) </returns>
		public static XSDateTime parseDateTime(string str)
		{
			// oh no... not again

			// ok its three things:
			// date T time timezone

			int index = str.IndexOf('T');
			if (index == -1)
			{
				return null;
			}

			// split date and rest...
			string date = str.Substring(0, index);
			string time = str.Substring(index + 1, str.Length - (index + 1));
			string timezone = null;

			// check for timezone
			index = time.IndexOf('+');
			if (index == -1)
			{
				index = time.IndexOf('-');
			}
			if (index == -1)
			{
				index = time.IndexOf('Z');
			}
			if (index != -1)
			{
				timezone = time.Substring(index, time.Length - index);
				time = time.Substring(0, index);
			}

			// get date
			int[] d = parse_date(date);
			if (d == null)
			{
				return null;
			}

			// SANITY CHEX
			var UTC = TimeZone.getTimeZone("UTC");
			GregorianCalendar cal = new GregorianCalendar(UTC);

			// year
			int year = d[0];
			if (year < 0)
			{
				year *= -1;
				cal.set(Calendar.ERA, GregorianCalendar.BC);
			}
			else
			{
				cal.set(Calendar.ERA, GregorianCalendar.AD);
			}

			// this is a nice bug....
			// if say the current day is 29...
			// then if we set the month to feb for example, and 29 doesn't
			// exist in that year, then the date screws up.
			cal.set(Calendar.DAY_OF_MONTH, 2);
			cal.set(Calendar.MONTH, 2);

			if (!set_item(cal, Calendar.YEAR, year))
			{
				return null;
			}

			if (!set_item(cal, Calendar.MONTH, d[1] - 1))
			{
				return null;
			}

			if (!set_item(cal, Calendar.DAY_OF_MONTH, d[2]))
			{
				return null;
			}

			// get time
			double[] t = parse_time(time);
			if (t == null)
			{
				return null;
			}

			if (!set_item(cal, Calendar.HOUR_OF_DAY, (int) t[0]))
			{
				return null;
			}

			if (!set_item(cal, Calendar.MINUTE, (int) t[1]))
			{
				return null;
			}

			if (!set_item(cal, Calendar.SECOND, (int) t[2]))
			{
				return null;
			}

			double ms = t[2] - ((int) t[2]);
			ms *= 1000;
			if (!set_item(cal, Calendar.MILLISECOND, (int) ms))
			{
				return null;
			}

			// get timezone
			int[] tz = null;
			XSDuration tzd = null;
			if (!string.ReferenceEquals(timezone, null))
			{
				tz = parse_timezone(timezone);

				if (tz == null)
				{
					return null;
				}

				tzd = new XSDayTimeDuration(0, tz[1], tz[2], 0.0, tz[0] < 0);

			}

			return new XSDateTime(cal, tzd);
		}

		/// <summary>
		/// Creates a new result sequence consisting of the retrievable date and time
		/// value in the supplied result sequence
		/// </summary>
		/// <param name="arg">
		///            The result sequence from which to extract the date and time
		///            value. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> A new result sequence consisting of the date and time value
		///         supplied. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			AnyAtomicType aat = (AnyAtomicType) arg.first();
			if (aat is NumericType || aat is XSDuration || aat is XSTime || isGDataType(aat) || aat is XSBoolean || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}

			CalendarType dt = castDateTime(aat);

			if (dt == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return dt;
		}

		private bool isCastable(AnyAtomicType aat)
		{
			if (aat is XSString || aat is XSUntypedAtomic)
			{
				return true;
			}

			if (aat is XSTime)
			{
				return false;
			}

			if (aat is XSDate || aat is XSDateTime)
			{
				return true;
			}

			return false;
		}

		private CalendarType castDateTime(AnyAtomicType aat)
		{
			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSDateTime(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSDateTime(dateTime.calendar(), dateTime.tz());
			}

			return parseDateTime(aat.StringValue);
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
		/// Retrieve the hour from the date stored
		/// </summary>
		/// <returns> the hour value of the date stored </returns>
		public virtual int hour()
		{
			return _calendar.get(Calendar.HOUR_OF_DAY);
		}

		/// <summary>
		/// Retrieve the minute from the date stored
		/// </summary>
		/// <returns> the minute value of the date stored </returns>
		public virtual int minute()
		{
			return _calendar.get(Calendar.MINUTE);
		}

		/// <summary>
		/// Retrieve the seconds from the date stored
		/// </summary>
		/// <returns> the seconds value of the date stored </returns>
		public virtual double second()
		{
			double s = _calendar.get(Calendar.SECOND);

			double ms = _calendar.get(Calendar.MILLISECOND);

			ms /= 1000;

			s += ms;
			return s;
		}

		public virtual bool timezoned()
		{
			return _timezoned;
		}

		/// <summary>
		/// Pads the supplied number to the supplied number of digits by adding 0's
		/// in front of it
		/// </summary>
		/// <param name="num">
		///            Number that si to be padded (if neccessay) </param>
		/// <param name="len">
		///            Desired length after padding </param>
		/// <returns> String representation of the padded integer </returns>
		public static string pad_int(int num, int len)
		{
			string ret = "";
			string snum = "" + num;

			int pad = len - snum.Length;

			// sort out the negative
			if (num < 0)
			{
				ret += "-";
				snum = snum.Substring(1, snum.Length - 1);
				pad++;
			}

			StringBuilder buf = new StringBuilder(ret);
			for (int i = 0; i < pad; i++)
			{
				buf.Append("0");
			}
			buf.Append(snum);
			ret = buf.ToString();
			return ret;
		}

		/// <summary>
		/// Retrieves a String representation of the date and time stored
		/// </summary>
		/// <returns> String representation of the date and time stored </returns>
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
    
				ret += pad_int(adjustFortimezone.get(Calendar.YEAR), 4);

				ret += "-";
				ret += pad_int(adjustFortimezone.get(Calendar.DAY_OF_MONTH), 2);

				ret += "-";
				ret += pad_int(adjustFortimezone.get(Calendar.HOUR_OF_DAY), 2);

				// time
				ret += "T";
    
				ret += pad_int(adjustFortimezone.get(Calendar.HOUR_OF_DAY), 2);

				ret += ":";
				ret += pad_int(adjustFortimezone.get(Calendar.MINUTE), 2);

				ret += ":";
				int isecond = (int) second();
				double sec = second();
    
				if ((sec - (isecond)) == 0.0)
				{
					ret += pad_int(isecond, 2);
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
						tZoneStr += pad_int(hrs, 2);
						tZoneStr += ":";
						tZoneStr += pad_int(min, 2);
    
						ret += tZoneStr;
					}
				}
    
				return ret;
			}
		}

		/// <summary>
		/// Retrive the datatype full pathname
		/// </summary>
		/// <returns> "xs:dateTime" which is the datatype full pathname </returns>
		public override string string_type()
		{
			return XS_DATE_TIME;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the date stored
		/// </summary>
		/// <returns> Calendar representation of the date stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		// comparisons
		/// <summary>
		/// Equality comparison on this and the supplied dates and times (taking
		/// timezones into account)
		/// </summary>
		/// <param name="arg">
		///            XSDateTime representation of the date to compare to </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> True if the two dates and times are represent the same exact
		///         point in time. False otherwise. </returns>
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSDateTime val = (XSDateTime) NumericType.get_single_type(arg, typeof(XSDateTime));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.Equals(thatcal);
		}

		/// <summary>
		/// Comparison on this and the supplied dates and times (taking timezones
		/// into account)
		/// </summary>
		/// <param name="arg">
		///            XSDateTime representation of the date to compare to </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> True if in time, this date and time lies before the date and time
		///         supplied. False otherwise. </returns>
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			XSDateTime val = (XSDateTime) NumericType.get_single_type(arg, typeof(XSDateTime));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.CompareTo(thatcal) < 0;
		}

		/// <summary>
		/// Comparison on this and the supplied dates and times (taking timezones
		/// into account)
		/// </summary>
		/// <param name="arg">
		///            XSDateTime representation of the date to compare to </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> True if in time, this date and time lies after the date and time
		///         supplied. False otherwise. </returns>
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			XSDateTime val = (XSDateTime) NumericType.get_single_type(arg, typeof(XSDateTime));
            Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.CompareTo(thatcal) > 0;
		}

		/// <summary>
		/// Retrieves the timezone associated with the date stored
		/// </summary>
		/// <returns> the timezone associated with the date stored </returns>
		public virtual XSDuration tz()
		{
			return _tz;
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
		/// Mathematical minus operator between this XSDateTime and a supplied result
		/// sequence (XSDateTime, XDTYearMonthDuration and XDTDayTimeDuration are
		/// only valid ones).
		/// </summary>
		/// <param name="arg">
		///            The supplied ResultSequence that is on the right of the minus
		///            operator. If this is an XSDateTime, the result will be a
		///            XDTDayTimeDuration of the duration of time between these two
		///            dates. If arg is an XDTYearMonthDuration or an
		///            XDTDayTimeDuration the result will be a XSDateTime of the
		///            result of the current date minus the duration of time
		///            supplied. </param>
		/// <returns> New ResultSequence consisting of the result of the mathematical
		///         minus operation. </returns>
		public virtual ResultSequence minus(ResultSequence arg)
		{
			if (arg.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = arg.first();

			if (!(at is XSDateTime) && !(at is XSYearMonthDuration) && !(at is XSDayTimeDuration))
			{
				DynamicError.throw_type_error();
			}

			if (at is XSDateTime)
			{
				return minusXSDateTime(arg);
			}

			if (at is XSYearMonthDuration)
			{
				return minusXSYearMonthDuration(at);
			}

			if (at is XSDayTimeDuration)
			{
				return minusXSDayTimeDuration(at);
			}
			return null; // unreach

		}

		private ResultSequence minusXSDateTime(ResultSequence arg)
		{
			XSDateTime val = (XSDateTime) NumericType.get_single_type(arg, typeof(XSDateTime));

			Calendar thisCal = normalizeCalendar(calendar(), tz());
            Calendar thatCal = normalizeCalendar(val.calendar(), val.tz());
			long duration = thisCal.getTimeInMillis()
                            - thatCal.getTimeInMillis();
			Duration dtduration = _datatypeFactory.newDuration(duration);
			return ResultSequenceFactory.create_new(XSDayTimeDuration.parseDTDuration(dtduration.ToString()));
		}

		private ResultSequence minusXSDayTimeDuration(Item at)
		{
			XSDuration val = (XSDuration) at;
            try
            {
                XSDateTime res = (XSDateTime) clone();
                XMLGregorianCalendar xmlCal = _datatypeFactory.newXMLGregorianCalendar((GregorianCalendar) calendar());
                Duration dtduration = _datatypeFactory.newDuration(val.StringValue);
                xmlCal.add(dtduration.negate());
                res = new XSDateTime(xmlCal.toGregorianCalendar(), res.tz());

                return ResultSequenceFactory.create_new(res);
            }
            catch
            {
                throw;
            }
			//catch (CloneNotSupportedException)
			//{

			//}
			//return null;
		}

		private ResultSequence minusXSYearMonthDuration(Item at)
		{
			XSYearMonthDuration val = (XSYearMonthDuration) at;

			try
			{
				XSDateTime res = (XSDateTime) clone();

                res.calendar().add(Calendar.MONTH, val.monthValue() * -1);
				return ResultSequenceFactory.create_new(res);
			}
			catch 
			{

			}
			return null;
		}

		/// <summary>
		/// Mathematical addition operator between this XSDateTime and a supplied
		/// result sequence (XDTYearMonthDuration and XDTDayTimeDuration are only
		/// valid ones).
		/// </summary>
		/// <param name="arg">
		///            The supplied ResultSequence that is on the right of the minus
		///            operator. If arg is an XDTYearMonthDuration or an
		///            XDTDayTimeDuration the result will be a XSDateTime of the
		///            result of the current date minus the duration of time
		///            supplied. </param>
		/// <returns> New ResultSequence consisting of the result of the mathematical
		///         minus operation. </returns>
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

					XSDateTime res = (XSDateTime) clone();

                    res.calendar().add(Calendar.MONTH, val.monthValue());
					return ResultSequenceFactory.create_new(res);
				}
				else if (at is XSDayTimeDuration)
				{
					XSDuration val = (XSDuration) at;

					XSDateTime res = (XSDateTime) clone();

					XMLGregorianCalendar xmlCal = _datatypeFactory.newXMLGregorianCalendar((GregorianCalendar) calendar());
					Duration dtduration = _datatypeFactory.newDuration(val.StringValue);
					xmlCal.add(dtduration);
					res = new XSDateTime(xmlCal.toGregorianCalendar(), res.tz());
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
				return BuiltinTypeLibrary.XS_DATETIME;
			}
		}

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

}