using System;
using System.Text;

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: ExsltDatetime.java 1225761 2011-12-30 06:04:51Z mrglavas $
 */

namespace org.apache.xalan.lib
{



	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class contains EXSLT dates and times extension functions.
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:datetime="http://exslt.org/dates-and-times"
	/// </pre>
	/// 
	/// The documentation for each function has been copied from the relevant
	/// EXSLT Implementer page.
	/// </summary>
	/// <seealso cref= <a href="http://www.exslt.org/">EXSLT</a>
	/// @xsl.usage general </seealso>

	public class ExsltDatetime
	{
		// Datetime formats (era and zone handled separately).
		internal const string dt = "yyyy-MM-dd'T'HH:mm:ss";
		internal const string d = "yyyy-MM-dd";
		internal const string gym = "yyyy-MM";
		internal const string gy = "yyyy";
		internal const string gmd = "--MM-dd";
		internal const string gm = "--MM--";
		internal const string gd = "---dd";
		internal const string t = "HH:mm:ss";
		internal const string EMPTY_STR = "";

		/// <summary>
		/// The date:date-time function returns the current date and time as a date/time string. 
		/// The date/time string that's returned must be a string in the format defined as the 
		/// lexical representation of xs:dateTime in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/#dateTime">[3.2.7 dateTime]</a> of
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>.
		/// The date/time format is basically CCYY-MM-DDThh:mm:ss, although implementers should consult
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a> and 
		/// <a href="http://www.iso.ch/markete/8601.pdf">[ISO 8601]</a> for details.
		/// The date/time string format must include a time zone, either a Z to indicate Coordinated 
		/// Universal Time or a + or - followed by the difference between the difference from UTC 
		/// represented as hh:mm. 
		/// </summary>
		public static string dateTime()
		{
		  DateTime cal = new DateTime();
		  DateTime datetime = cal;
		  // Format for date and time.
		  SimpleDateFormat dateFormat = new SimpleDateFormat(dt);

		  StringBuilder buff = new StringBuilder(dateFormat.format(datetime));
		  // Must also include offset from UTF.
		  // Get the offset (in milliseconds).
		  int offset = cal.get(DateTime.ZONE_OFFSET) + cal.get(DateTime.DST_OFFSET);
		  // If there is no offset, we have "Coordinated
		  // Universal Time."
		  if (offset == 0)
		  {
			buff.Append('Z');
		  }
		  else
		  {
			// Convert milliseconds to hours and minutes
			int hrs = offset / (60 * 60 * 1000);
			// In a few cases, the time zone may be +/-hh:30.
			int min = offset % (60 * 60 * 1000);
			char posneg = hrs < 0? '-': '+';
			buff.Append(posneg + formatDigits(hrs) + ':' + formatDigits(min));
		  }
		  return buff.ToString();
		}

		/// <summary>
		/// Represent the hours and minutes with two-digit strings. </summary>
		/// <param name="q"> hrs or minutes. </param>
		/// <returns> two-digit String representation of hrs or minutes. </returns>
		private static string formatDigits(int q)
		{
		  string dd = Math.Abs(q).ToString();
		  return dd.Length == 1 ? '0' + dd : dd;
		}

		/// <summary>
		/// The date:date function returns the date specified in the date/time string given 
		/// as the argument. If no argument is given, then the current local date/time, as 
		/// returned by date:date-time is used as a default argument. 
		/// The date/time string that's returned must be a string in the format defined as the 
		/// lexical representation of xs:dateTime in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/#dateTime">[3.2.7 dateTime]</a> of
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>.
		/// If the argument is not in either of these formats, date:date returns an empty string (''). 
		/// The date/time format is basically CCYY-MM-DDThh:mm:ss, although implementers should consult 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a> and 
		/// <a href="http://www.iso.ch/markete/8601.pdf">[ISO 8601]</a> for details. 
		/// The date is returned as a string with a lexical representation as defined for xs:date in 
		/// [3.2.9 date] of [XML Schema Part 2: Datatypes]. The date format is basically CCYY-MM-DD, 
		/// although implementers should consult [XML Schema Part 2: Datatypes] and [ISO 8601] for details.
		/// If no argument is given or the argument date/time specifies a time zone, then the date string 
		/// format must include a time zone, either a Z to indicate Coordinated Universal Time or a + or - 
		/// followed by the difference between the difference from UTC represented as hh:mm. If an argument 
		/// is specified and it does not specify a time zone, then the date string format must not include 
		/// a time zone. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String date(String datetimeIn) throws java.text.ParseException
		public static string date(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string leader = edz[0];
		  string datetime = edz[1];
		  string zone = edz[2];
		  if (string.ReferenceEquals(datetime, null) || string.ReferenceEquals(zone, null))
		  {
			return EMPTY_STR;
		  }

		  string[] formatsIn = new string[] {dt, d};
		  string formatOut = d;
		  DateTime date = testFormats(datetime, formatsIn);
		  if (date == null)
		  {
			  return EMPTY_STR;
		  }

		  SimpleDateFormat dateFormat = new SimpleDateFormat(formatOut);
		  dateFormat.Lenient = false;
		  string dateOut = dateFormat.format(date);
		  if (dateOut.Length == 0)
		  {
			  return EMPTY_STR;
		  }
		  else
		  {
			return (leader + dateOut + zone);
		  }
		}


		/// <summary>
		/// See above.
		/// </summary>
		public static string date()
		{
		  string datetime = dateTime();
		  string date = datetime.Substring(0, datetime.IndexOf("T", StringComparison.Ordinal));
		  string zone = datetime.Substring(getZoneStart(datetime));
		  return (date + zone);
		}

		/// <summary>
		/// The date:time function returns the time specified in the date/time string given 
		/// as the argument. If no argument is given, then the current local date/time, as 
		/// returned by date:date-time is used as a default argument. 
		/// The date/time string that's returned must be a string in the format defined as the 
		/// lexical representation of xs:dateTime in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/#dateTime">[3.2.7 dateTime]</a> of
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// If the argument string is not in this format, date:time returns an empty string (''). 
		/// The date/time format is basically CCYY-MM-DDThh:mm:ss, although implementers should consult 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a> and 
		/// <a href="http://www.iso.ch/markete/8601.pdf">[ISO 8601]</a> for details.
		/// The date is returned as a string with a lexical representation as defined for xs:time in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/#time">[3.2.8 time]</a> of [XML Schema Part 2: Datatypes].
		/// The time format is basically hh:mm:ss, although implementers should consult [XML Schema Part 2: 
		/// Datatypes] and [ISO 8601] for details. 
		/// If no argument is given or the argument date/time specifies a time zone, then the time string 
		/// format must include a time zone, either a Z to indicate Coordinated Universal Time or a + or - 
		/// followed by the difference between the difference from UTC represented as hh:mm. If an argument 
		/// is specified and it does not specify a time zone, then the time string format must not include 
		/// a time zone. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String time(String timeIn) throws java.text.ParseException
		public static string time(string timeIn)
		{
		  string[] edz = getEraDatetimeZone(timeIn);
		  string time = edz[1];
		  string zone = edz[2];
		  if (string.ReferenceEquals(time, null) || string.ReferenceEquals(zone, null))
		  {
			return EMPTY_STR;
		  }

		  string[] formatsIn = new string[] {dt, d, t};
		  string formatOut = t;
		  DateTime date = testFormats(time, formatsIn);
		  if (date == null)
		  {
			  return EMPTY_STR;
		  }
		  SimpleDateFormat dateFormat = new SimpleDateFormat(formatOut);
		  string @out = dateFormat.format(date);
		  return (@out + zone);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static string time()
		{
		  string datetime = dateTime();
		  string time = datetime.Substring(datetime.IndexOf("T", StringComparison.Ordinal) + 1);

		  // The datetime() function returns the zone on the datetime string.  If we
		  // append it, we get the zone substring duplicated.
		  // Fix for JIRA 2013

		  // String zone = datetime.substring(getZoneStart(datetime));      
		  // return (time + zone);
		  return (time);
		}

		/// <summary>
		/// The date:year function returns the year of a date as a number. If no 
		/// argument is given, then the current local date/time, as returned by 
		/// date:date-time is used as a default argument.
		/// The date/time string specified as the first argument must be a right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime in one 
		/// of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>.
		/// The permitted formats are as follows: 
		///   xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///   xs:date (CCYY-MM-DD) 
		///   xs:gYearMonth (CCYY-MM) 
		///   xs:gYear (CCYY) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double year(String datetimeIn) throws java.text.ParseException
		public static double year(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  bool ad = edz[0].Length == 0; // AD (Common Era -- empty leader)
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, d, gym, gy};
		  double yr = getNumber(datetime, formats, DateTime.YEAR);
		  if (ad || yr == Double.NaN)
		  {
			return yr;
		  }
		  else
		  {
			return -yr;
		  }
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double year()
		{
		  DateTime cal = new DateTime();
		  return cal.Year;
		}

		/// <summary>
		/// The date:month-in-year function returns the month of a date as a number. If no argument 
		/// is given, then the current local date/time, as returned by date:date-time is used 
		/// as a default argument. 
		/// The date/time string specified as the first argument is a left or right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime in one of 
		/// the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>.
		/// The permitted formats are as follows: 
		///    xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///    xs:date (CCYY-MM-DD) 
		///    xs:gYearMonth (CCYY-MM)
		///    xs:gMonth (--MM--) 
		///    xs:gMonthDay (--MM-DD)
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double monthInYear(String datetimeIn) throws java.text.ParseException
		public static double monthInYear(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, d, gym, gm, gmd};
		  return getNumber(datetime, formats, DateTime.MONTH) + 1;
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double monthInYear()
		{
		  DateTime cal = new DateTime();
		  return cal.Month + 1;
		}

		/// <summary>
		/// The date:week-in-year function returns the week of the year as a number. If no argument 
		/// is given, then the current local date/time, as returned by date:date-time is used as the 
		/// default argument. For the purposes of numbering, counting follows ISO 8601: week 1 in a year 
		/// is the week containing the first Thursday of the year, with new weeks beginning on a Monday. 
		/// The date/time string specified as the argument is a right-truncated string in the format 
		/// defined as the lexical representation of xs:dateTime in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. The 
		/// permitted formats are as follows: 
		///    xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///    xs:date (CCYY-MM-DD) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double weekInYear(String datetimeIn) throws java.text.ParseException
		public static double weekInYear(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, d};
		  return getNumber(datetime, formats, DateTime.WEEK_OF_YEAR);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double weekInYear()
		{
		   DateTime cal = new DateTime();
		  return cal.get(DateTime.WEEK_OF_YEAR);
		}

		/// <summary>
		/// The date:day-in-year function returns the day of a date in a year 
		/// as a number. If no argument is given, then the current local
		/// date/time, as returned by date:date-time is used the default argument.
		/// The date/time string specified as the argument is a right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows:
		///     xs:dateTime (CCYY-MM-DDThh:mm:ss)
		///     xs:date (CCYY-MM-DD) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double dayInYear(String datetimeIn) throws java.text.ParseException
		public static double dayInYear(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, d};
		  return getNumber(datetime, formats, DateTime.DAY_OF_YEAR);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double dayInYear()
		{
		   DateTime cal = new DateTime();
		  return cal.DayOfYear;
		}


		/// <summary>
		/// The date:day-in-month function returns the day of a date as a number. 
		/// If no argument is given, then the current local date/time, as returned 
		/// by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a left or right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime 
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///      xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///      xs:date (CCYY-MM-DD) 
		///      xs:gMonthDay (--MM-DD) 
		///      xs:gDay (---DD) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double dayInMonth(String datetimeIn) throws java.text.ParseException
		public static double dayInMonth(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  string[] formats = new string[] {dt, d, gmd, gd};
		  double day = getNumber(datetime, formats, DateTime.DAY_OF_MONTH);
		  return day;
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double dayInMonth()
		{
		  DateTime cal = new DateTime();
		  return cal.Day;
		}

		/// <summary>
		/// The date:day-of-week-in-month function returns the day-of-the-week 
		/// in a month of a date as a number (e.g. 3 for the 3rd Tuesday in May). 
		/// If no argument is given, then the current local date/time, as returned 
		/// by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a right-truncated string
		/// in the format defined as the lexical representation of xs:dateTime in one 
		/// of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///      xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///      xs:date (CCYY-MM-DD) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double dayOfWeekInMonth(String datetimeIn) throws java.text.ParseException
		public static double dayOfWeekInMonth(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, d};
		  return getNumber(datetime, formats, DateTime.DAY_OF_WEEK_IN_MONTH);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double dayOfWeekInMonth()
		{
		   DateTime cal = new DateTime();
		  return cal.get(DateTime.DAY_OF_WEEK_IN_MONTH);
		}


		/// <summary>
		/// The date:day-in-week function returns the day of the week given in a 
		/// date as a number. If no argument is given, then the current local date/time, 
		/// as returned by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a right-truncated string 
		/// in the format defined as the lexical representation of xs:dateTime in one 
		/// of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///      xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///      xs:date (CCYY-MM-DD) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		///                        The numbering of days of the week starts at 1 for Sunday, 2 for Monday and so on up to 7 for Saturday.  
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double dayInWeek(String datetimeIn) throws java.text.ParseException
		public static double dayInWeek(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, d};
		  return getNumber(datetime, formats, DateTime.DAY_OF_WEEK);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double dayInWeek()
		{
		   DateTime cal = new DateTime();
		  return cal.DayOfWeek;
		}

		/// <summary>
		/// The date:hour-in-day function returns the hour of the day as a number. 
		/// If no argument is given, then the current local date/time, as returned 
		/// by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a right-truncated 
		/// string  in the format defined as the lexical representation of xs:dateTime
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///     xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///     xs:time (hh:mm:ss) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double hourInDay(String datetimeIn) throws java.text.ParseException
		public static double hourInDay(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, t};
		  return getNumber(datetime, formats, DateTime.HOUR_OF_DAY);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double hourInDay()
		{
		   DateTime cal = new DateTime();
		  return cal.Hour;
		}

		/// <summary>
		/// The date:minute-in-hour function returns the minute of the hour 
		/// as a number. If no argument is given, then the current local
		/// date/time, as returned by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///      xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///      xs:time (hh:mm:ss) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double minuteInHour(String datetimeIn) throws java.text.ParseException
		public static double minuteInHour(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt,t};
		  return getNumber(datetime, formats, DateTime.MINUTE);
		}

		/// <summary>
		/// See above.
		/// </summary>
	   public static double minuteInHour()
	   {
		   DateTime cal = new DateTime();
		  return cal.Minute;
	   }

		/// <summary>
		/// The date:second-in-minute function returns the second of the minute 
		/// as a number. If no argument is given, then the current local 
		/// date/time, as returned by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///      xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///      xs:time (hh:mm:ss) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double secondInMinute(String datetimeIn) throws java.text.ParseException
		public static double secondInMinute(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return Double.NaN;
		  }

		  string[] formats = new string[] {dt, t};
		  return getNumber(datetime, formats, DateTime.SECOND);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static double secondInMinute()
		{
		   DateTime cal = new DateTime();
		  return cal.Second;
		}

		/// <summary>
		/// The date:leap-year function returns true if the year given in a date 
		/// is a leap year. If no argument is given, then the current local
		/// date/time, as returned by date:date-time is used as a default argument. 
		/// The date/time string specified as the first argument must be a 
		/// right-truncated string in the format defined as the lexical representation
		/// of xs:dateTime in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///    xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///    xs:date (CCYY-MM-DD) 
		///    xs:gYearMonth (CCYY-MM) 
		///    xs:gYear (CCYY) 
		/// If the date/time string is not in one of these formats, then NaN is returned. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.xpath.objects.XObject leapYear(String datetimeIn) throws java.text.ParseException
		public static XObject leapYear(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return new XNumber(Double.NaN);
		  }

		  string[] formats = new string[] {dt, d, gym, gy};
		  double dbl = getNumber(datetime, formats, DateTime.YEAR);
		  if (dbl == Double.NaN)
		  {
			return new XNumber(Double.NaN);
		  }
		  int yr = (int)dbl;
		  return new XBoolean(yr % 400 == 0 || (yr % 100 != 0 && yr % 4 == 0));
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static bool leapYear()
		{
		  DateTime cal = new DateTime();
		  int yr = (int)cal.Year;
		  return (yr % 400 == 0 || (yr % 100 != 0 && yr % 4 == 0));
		}

		/// <summary>
		/// The date:month-name function returns the full name of the month of a date. 
		/// If no argument is given, then the current local date/time, as returned by 
		/// date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a left or right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime in
		///  one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///    xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///    xs:date (CCYY-MM-DD) 
		///    xs:gYearMonth (CCYY-MM) 
		///    xs:gMonth (--MM--) 
		/// If the date/time string is not in one of these formats, then an empty string ('') 
		/// is returned. 
		/// The result is an English month name: one of 'January', 'February', 'March', 
		/// 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November' 
		/// or 'December'. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String monthName(String datetimeIn) throws java.text.ParseException
		public static string monthName(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return EMPTY_STR;
		  }

		  string[] formatsIn = new string[] {dt, d, gym, gm};
		  string formatOut = "MMMM";
		  return getNameOrAbbrev(datetimeIn, formatsIn, formatOut);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static string monthName()
		{
		  string format = "MMMM";
		  return getNameOrAbbrev(format);
		}

		/// <summary>
		/// The date:month-abbreviation function returns the abbreviation of the month of 
		/// a date. If no argument is given, then the current local date/time, as returned 
		/// by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a left or right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime in 
		/// one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///    xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///    xs:date (CCYY-MM-DD) 
		///    xs:gYearMonth (CCYY-MM) 
		///    xs:gMonth (--MM--) 
		/// If the date/time string is not in one of these formats, then an empty string ('') 
		/// is returned. 
		/// The result is a three-letter English month abbreviation: one of 'Jan', 'Feb', 'Mar', 
		/// 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov' or 'Dec'. 
		/// An implementation of this extension function in the EXSLT date namespace must conform 
		/// to the behaviour described in this document. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String monthAbbreviation(String datetimeIn) throws java.text.ParseException
		public static string monthAbbreviation(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return EMPTY_STR;
		  }

		  string[] formatsIn = new string[] {dt, d, gym, gm};
		  string formatOut = "MMM";
		  return getNameOrAbbrev(datetimeIn, formatsIn, formatOut);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static string monthAbbreviation()
		{
		  string format = "MMM";
		  return getNameOrAbbrev(format);
		}

		/// <summary>
		/// The date:day-name function returns the full name of the day of the week 
		/// of a date.  If no argument is given, then the current local date/time, 
		/// as returned by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a left or right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime 
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///     xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///     xs:date (CCYY-MM-DD) 
		/// If the date/time string is not in one of these formats, then the empty string ('') 
		/// is returned. 
		/// The result is an English day name: one of 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 
		/// 'Thursday' or 'Friday'. 
		/// An implementation of this extension function in the EXSLT date namespace must conform 
		/// to the behaviour described in this document. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String dayName(String datetimeIn) throws java.text.ParseException
		public static string dayName(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return EMPTY_STR;
		  }

		  string[] formatsIn = new string[] {dt, d};
		  string formatOut = "EEEE";
		  return getNameOrAbbrev(datetimeIn, formatsIn, formatOut);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static string dayName()
		{
		  string format = "EEEE";
		  return getNameOrAbbrev(format);
		}

		/// <summary>
		/// The date:day-abbreviation function returns the abbreviation of the day 
		/// of the week of a date. If no argument is given, then the current local 
		/// date/time, as returned  by date:date-time is used the default argument. 
		/// The date/time string specified as the argument is a left or right-truncated 
		/// string in the format defined as the lexical representation of xs:dateTime 
		/// in one of the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		///     xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		///     xs:date (CCYY-MM-DD) 
		/// If the date/time string is not in one of these formats, then the empty string 
		/// ('') is returned. 
		/// The result is a three-letter English day abbreviation: one of 'Sun', 'Mon', 'Tue', 
		/// 'Wed', 'Thu' or 'Fri'. 
		/// An implementation of this extension function in the EXSLT date namespace must conform 
		/// to the behaviour described in this document. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String dayAbbreviation(String datetimeIn) throws java.text.ParseException
		public static string dayAbbreviation(string datetimeIn)
		{
		  string[] edz = getEraDatetimeZone(datetimeIn);
		  string datetime = edz[1];
		  if (string.ReferenceEquals(datetime, null))
		  {
			return EMPTY_STR;
		  }

		  string[] formatsIn = new string[] {dt, d};
		  string formatOut = "EEE";
		  return getNameOrAbbrev(datetimeIn, formatsIn, formatOut);
		}

		/// <summary>
		/// See above.
		/// </summary>
		public static string dayAbbreviation()
		{
		  string format = "EEE";
		  return getNameOrAbbrev(format);
		}

		/// <summary>
		/// Returns an array with the 3 components that a datetime input string 
		/// may contain: - (for BC era), datetime, and zone. If the zone is not
		/// valid, return null for that component.
		/// </summary>
		private static string[] getEraDatetimeZone(string @in)
		{
		  string leader = "";
		  string datetime = @in;
		  string zone = "";
		  if (@in[0] == '-' && !@in.StartsWith("--", StringComparison.Ordinal))
		  {
			leader = "-"; //  '+' is implicit , not allowed
			datetime = @in.Substring(1);
		  }
		  int z = getZoneStart(datetime);
		  if (z > 0)
		  {
			zone = datetime.Substring(z);
			datetime = datetime.Substring(0, z);
		  }
		  else if (z == -2)
		  {
			zone = null;
		  }
		  //System.out.println("'" + leader + "' " + datetime + " " + zone);
		  return new string[]{leader, datetime, zone};
		}

		/// <summary>
		/// Get the start of zone information if the input ends
		/// with 'Z' or +/-hh:mm. If a zone string is not
		/// found, return -1; if the zone string is invalid,
		/// return -2.
		/// </summary>
		private static int getZoneStart(string datetime)
		{
		  if (datetime.IndexOf('Z') == datetime.Length - 1)
		  {
			return datetime.Length - 1;
		  }
		  else if (datetime.Length >= 6 && datetime[datetime.Length - 3] == ':' && (datetime[datetime.Length - 6] == '+' || datetime[datetime.Length - 6] == '-'))
		  {
			try
			{
			  SimpleDateFormat dateFormat = new SimpleDateFormat("HH:mm");
			  dateFormat.Lenient = false;
			  DateTime d = dateFormat.parse(datetime.Substring(datetime.Length - 5));
			  return datetime.Length - 6;
			}
			catch (ParseException pe)
			{
			  Console.WriteLine("ParseException " + pe.ErrorOffset);
			  return -2; // Invalid.
			}

		  }
			return -1; // No zone information.
		}

		/// <summary>
		/// Attempt to parse an input string with the allowed formats, returning
		/// null if none of the formats work.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.util.Date testFormats(String in, String[] formats) throws java.text.ParseException
		private static DateTime testFormats(string @in, string[] formats)
		{
		  for (int i = 0; i < formats.Length; i++)
		  {
			try
			{
			  SimpleDateFormat dateFormat = new SimpleDateFormat(formats[i]);
			  dateFormat.Lenient = false;
			  return dateFormat.parse(@in);
			}
			catch (ParseException)
			{
			}
		  }
		  return null;
		}


		/// <summary>
		/// Parse the input string and return the corresponding calendar field
		/// number.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static double getNumber(String in, String[] formats, int calField) throws java.text.ParseException
		private static double getNumber(string @in, string[] formats, int calField)
		{
		  DateTime cal = new DateTime();
		  cal.Lenient = false;
		  // Try the allowed formats, from longest to shortest.
		  DateTime date = testFormats(@in, formats);
		  if (date == null)
		  {
			  return Double.NaN;
		  }
		  cal = new DateTime(date);
		  return cal.get(calField);
		}

		/// <summary>
		///  Get the full name or abbreviation of the month or day.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static String getNameOrAbbrev(String in, String[] formatsIn, String formatOut) throws java.text.ParseException
		private static string getNameOrAbbrev(string @in, string[] formatsIn, string formatOut)
		{
		  for (int i = 0; i < formatsIn.Length; i++) // from longest to shortest.
		  {
			try
			{
			  SimpleDateFormat dateFormat = new SimpleDateFormat(formatsIn[i], Locale.ENGLISH);
			  dateFormat.Lenient = false;
			  DateTime dt = dateFormat.parse(@in);
			  dateFormat.applyPattern(formatOut);
			  return dateFormat.format(dt);
			}
			catch (ParseException)
			{
			}
		  }
		  return "";
		}
		/// <summary>
		/// Get the full name or abbreviation for the current month or day 
		/// (no input string).
		/// </summary>
		private static string getNameOrAbbrev(string format)
		{
		  DateTime cal = new DateTime();
		  SimpleDateFormat dateFormat = new SimpleDateFormat(format, Locale.ENGLISH);
		  return dateFormat.format(cal);
		}

		/// <summary>
		/// The date:format-date function formats a date/time according to a pattern.
		/// <para>
		/// The first argument to date:format-date specifies the date/time to be 
		/// formatted. It must be right or left-truncated date/time strings in one of 
		/// the formats defined in 
		/// <a href="http://www.w3.org/TR/xmlschema-2/">[XML Schema Part 2: Datatypes]</a>. 
		/// The permitted formats are as follows: 
		/// <ul>
		/// <li>xs:dateTime (CCYY-MM-DDThh:mm:ss) 
		/// <li>xs:date (CCYY-MM-DD) 
		/// <li>xs:time (hh:mm:ss) 
		/// <li>xs:gYearMonth (CCYY-MM) 
		/// <li>xs:gYear (CCYY) 
		/// <li>xs:gMonthDay (--MM-DD) 
		/// <li>xs:gMonth (--MM--) 
		/// <li>xs:gDay (---DD)
		/// </ul>
		/// The second argument is a string that gives the format pattern used to 
		/// format the date. The format pattern must be in the syntax specified by 
		/// the JDK 1.1 SimpleDateFormat class. The format pattern string is 
		/// interpreted as described for the JDK 1.1 SimpleDateFormat class. 
		/// </para>
		/// <para>
		/// If the date/time format is right-truncated (i.e. in a format other than 
		/// xs:time, or xs:dateTime) then any missing components are assumed to be as 
		/// follows: if no month is specified, it is given a month of 01; if no day 
		/// is specified, it is given a day of 01; if no time is specified, it is 
		/// given a time of 00:00:00. 
		/// </para>
		/// <para>
		/// If the date/time format is left-truncated (i.e. xs:time, xs:gMonthDay, 
		/// xs:gMonth or xs:gDay) and the format pattern has a token that uses a 
		/// component that is missing from the date/time format used, then that token 
		/// is replaced with an empty string ('') within the result.
		/// 
		/// The author is Helg Bredow (helg.bredow@kalido.com)
		/// </para>
		/// </summary>
		public static string formatDate(string dateTime, string pattern)
		{
			const string yearSymbols = "Gy";
			const string monthSymbols = "M";
			const string daySymbols = "dDEFwW";
			TimeZone timeZone;
			string zone;

			// Get the timezone information if it was supplied and modify the 
			// dateTime so that SimpleDateFormat will understand it.
			if (dateTime.EndsWith("Z", StringComparison.Ordinal) || dateTime.EndsWith("z", StringComparison.Ordinal))
			{
				timeZone = TimeZone.getTimeZone("GMT");
				dateTime = dateTime.Substring(0, dateTime.Length - 1) + "GMT";
				zone = "z";
			}
			else if ((dateTime.Length >= 6) && (dateTime[dateTime.Length - 3] == ':') && ((dateTime[dateTime.Length - 6] == '+') || (dateTime[dateTime.Length - 6] == '-')))
			{
				string offset = dateTime.Substring(dateTime.Length - 6);

				if ("+00:00".Equals(offset) || "-00:00".Equals(offset))
				{
					timeZone = TimeZone.getTimeZone("GMT");
				}
				else
				{
					timeZone = TimeZone.getTimeZone("GMT" + offset);
				}
				zone = "z";
				// Need to adjust it since SimpleDateFormat requires GMT+hh:mm but
				// we have +hh:mm.
				dateTime = dateTime.Substring(0, dateTime.Length - 6) + "GMT" + offset;
			}
			else
			{
				// Assume local time.
				timeZone = TimeZone.Default;
				zone = "";
				// Leave off the timezone since SimpleDateFormat will assume local
				// time if time zone is not included.
			}
			string[] formats = new string[] {dt + zone, d, gym, gy};

			// Try the time format first. We need to do this to prevent 
			// SimpleDateFormat from interpreting a time as a year. i.e we just need
			// to check if it's a time before we check it's a year.
			try
			{
				SimpleDateFormat inFormat = new SimpleDateFormat(t + zone);
				inFormat.Lenient = false;
				DateTime d = inFormat.parse(dateTime);
				SimpleDateFormat outFormat = new SimpleDateFormat(strip(yearSymbols + monthSymbols + daySymbols, pattern));
				outFormat.TimeZone = timeZone;
				return outFormat.format(d);
			}
			catch (ParseException)
			{
			}

			// Try the right truncated formats.
			for (int i = 0; i < formats.Length; i++)
			{
				try
				{
					SimpleDateFormat inFormat = new SimpleDateFormat(formats[i]);
					inFormat.Lenient = false;
					DateTime d = inFormat.parse(dateTime);
					SimpleDateFormat outFormat = new SimpleDateFormat(pattern);
					outFormat.TimeZone = timeZone;
					return outFormat.format(d);
				}
				catch (ParseException)
				{
				}
			}

			// Now try the left truncated ones. The Java format() function doesn't
			// return the correct strings in this case. We strip any pattern 
			// symbols that shouldn't be output so that they are not defaulted to 
			// inappropriate values in the output.
			try
			{
				SimpleDateFormat inFormat = new SimpleDateFormat(gmd);
				inFormat.Lenient = false;
				DateTime d = inFormat.parse(dateTime);
				SimpleDateFormat outFormat = new SimpleDateFormat(strip(yearSymbols, pattern));
				outFormat.TimeZone = timeZone;
				return outFormat.format(d);
			}
			catch (ParseException)
			{
			}
			try
			{
				SimpleDateFormat inFormat = new SimpleDateFormat(gm);
				inFormat.Lenient = false;
				DateTime d = inFormat.parse(dateTime);
				SimpleDateFormat outFormat = new SimpleDateFormat(strip(yearSymbols, pattern));
				outFormat.TimeZone = timeZone;
				return outFormat.format(d);
			}
			catch (ParseException)
			{
			}
			try
			{
				SimpleDateFormat inFormat = new SimpleDateFormat(gd);
				inFormat.Lenient = false;
				DateTime d = inFormat.parse(dateTime);
				SimpleDateFormat outFormat = new SimpleDateFormat(strip(yearSymbols + monthSymbols, pattern));
				outFormat.TimeZone = timeZone;
				return outFormat.format(d);
			}
			catch (ParseException)
			{
			}
			return EMPTY_STR;
		}

		/// <summary>
		/// Strips occurrences of the given character from a date format pattern. </summary>
		/// <param name="symbols"> list of symbols to strip. </param>
		/// <param name="pattern">
		/// @return </param>
		private static string strip(string symbols, string pattern)
		{
			int i = 0;
			StringBuilder result = new StringBuilder(pattern.Length);

			while (i < pattern.Length)
			{
				char ch = pattern[i];
				if (ch == '\'')
				{
					// Assume it's an opening quote so simply copy the quoted 
					// text to the result. There is nothing to strip here.
					int endQuote = pattern.IndexOf('\'', i + 1);
					if (endQuote == -1)
					{
						endQuote = pattern.Length;
					}
					result.Append(pattern.Substring(i, endQuote - i));
					i = endQuote++;
				}
				else if (symbols.IndexOf(ch) > -1)
				{
					// The char needs to be stripped.
					i++;
				}
				else
				{
					result.Append(ch);
					i++;
				}
			}
			return result.ToString();
		}

	}

}