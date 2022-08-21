using System;
using System.Text;
using java.util;
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
///     David Carver (STAR) - bug 262765 - Fix parsing of gMonthDay to valid date
///     David Carver (STAR) - bug 282223 - fix timezone adjustment creation.
///                                        fixed casting issue.
///     David Carver - bug 280547 - fix dates for comparison 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the MonthDay datatype
	/// </summary>
	public class XSGMonthDay : CalendarType, CmpEq
	{

		private const string XS_G_MONTH_DAY = "xs:gMonthDay";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;


		/// <summary>
		/// Initialises a representation of the supplied month and day
		/// </summary>
		/// <param name="cal">
		///            Calendar representation of the month and day to be stored </param>
		/// <param name="tz">
		///            Timezone associated with this month and day </param>
		public XSGMonthDay(Calendar cal, XSDuration tz)
		{
			_calendar = cal;
			if (tz != null)
			{
				_timezoned = true;
				_tz = tz;
			}
		}

		/// <summary>
		/// Initialises a representation of the current month and day
		/// </summary>
		public XSGMonthDay() : this(new GregorianCalendar(TimeZone.CurrentTimeZone), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "gMonthDay" which is the datatype's name </returns>
		public override string type_name()
		{
			return "gMonthDay";
		}

		/// <summary>
		/// Parses a String representation of a month and day and constructs a new
		/// XSGMonthDay representation of it.
		/// </summary>
		/// <param name="str">
		///            The String representation of the month and day (and optional
		///            timezone) </param>
		/// <returns> The XSGMonthDay representation of the supplied date </returns>
		public static XSGMonthDay parse_gMonthDay(string str)
		{

			string startdate = "1972-";
			string starttime = "T00:00:00";

			int index = str.LastIndexOf('+', str.Length);

			if (index == -1)
			{
				index = str.LastIndexOf('-');
			}
			if (index == -1)
			{
				index = str.LastIndexOf('Z', str.Length);
			}
			if (index != -1)
			{
				int zIndex = str.LastIndexOf('Z', str.Length);
				if (zIndex == -1)
				{
					if (index > 5)
					{
						zIndex = index;
					}
				}
				if (zIndex == -1)
				{
					zIndex = str.LastIndexOf('+');
				}


				string[] split = str.Split("-", true);
				startdate += split[2].Replace("Z", "") + "-" + split[3].Replace("Z", "").Substring(0, 2);

				if (split.Length > 4)
				{
					string[] timesplit = split[4].Split(":", true);
					if (timesplit.Length < 3)
					{
						starttime = "T";
						StringBuilder buf = new StringBuilder(starttime);
						for (int cnt = 0; cnt < timesplit.Length; cnt++)
						{
							buf.Append(timesplit[cnt] + ":");
						}
						buf.Append("00");
						starttime = buf.ToString();
					}
					else
					{
						starttime += timesplit[0] + ":" + timesplit[1] + ":" + timesplit[2];
					}
				}

				startdate = startdate.Trim();
				startdate += starttime;


				if (zIndex != -1)
				{
					startdate += str.Substring(zIndex);
				}
			}
			else
			{
				startdate += starttime;
			}

			XSDateTime dt = XSDateTime.parseDateTime(startdate);
			if (dt == null)
			{
				return null;
			}

			return new XSGMonthDay(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable gMonthDay in
		/// the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the gMonthDay is to be extracted </param>
		/// <returns> New ResultSequence consisting of the supplied month and day </returns>
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
			if (aat is NumericType || aat is XSDuration || aat is XSTime || isGDataType(aat) || aat is XSBoolean || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}

			XSGMonthDay val = castGMonthDay(aat);

			if (val == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return val;
		}

		protected internal virtual bool isGDataType(AnyAtomicType aat)
		{
			string type = aat.string_type();
			if (type.Equals("xs:gDay") || type.Equals("xs:gMonth") || type.Equals("xs:gYear") || type.Equals("xs:gYearMonth"))
			{
				return true;
			}
			return false;
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

			if (aat is XSDate || aat is XSDateTime || aat is XSGMonthDay)
			{
				return true;
			}

			return false;
		}

		private XSGMonthDay castGMonthDay(AnyAtomicType aat)
		{
			if (aat is XSGMonthDay)
			{
				XSGMonthDay gmd = (XSGMonthDay) aat;
				return new XSGMonthDay(gmd.calendar(), gmd.tz());
			}

			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSGMonthDay(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSGMonthDay(dateTime.calendar(), dateTime.tz());
			}

			return parse_gMonthDay(aat.StringValue);
		}

		/// <summary>
		/// Retrieves the actual month as an integer
		/// </summary>
		/// <returns> The actual month as an integer </returns>
		public virtual int month()
		{
			return _calendar.Month + 1;
		}

		/// <summary>
		/// Retrieves the actual day as an integer
		/// </summary>
		/// <returns> The actual day as an integer </returns>
		public virtual int day()
		{
			return _calendar.Day;
		}

		/// <summary>
		/// Check for whether a timezone was specified at creation
		/// </summary>
		/// <returns> True if a timezone was specified. False otherwise </returns>
		public virtual bool timezoned()
		{
			return _timezoned;
		}

		/// <summary>
		/// Retrieves a String representation of the stored month and day
		/// </summary>
		/// <returns> String representation of the stored month and day </returns>
		public override string StringValue
		{
			get
			{
				string ret = "--";
    
				Calendar adjustFortimezone = calendar();
    
				ret += XSDateTime.pad_int(month(), 2);
    
				ret += "-";
				ret += XSDateTime.pad_int(adjustFortimezone.Day, 2);
    
				if (timezoned())
				{
    
					int hrs = tz().hours();
					int min = tz().minutes();
					double secs = tz().seconds();
					if (hrs == 0 && min == 0 && secs == 0)
					{
					  ret += "Z";
					}
					else
					{
					  string tZoneStr = "";
					  if (tz().negative())
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
		/// <returns> "xs:gMonthDay" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_G_MONTH_DAY;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the month and day stored
		/// </summary>
		/// <returns> Calendar representation of the month and day stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation. This
		/// representation must be of type XSGMonthDay
		/// </summary>
		/// <param name="arg">
		///            The XSGMonthDay to compare with </param>
		/// <returns> True if the two representations are of the same month and day.
		///         False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSGMonthDay val = (XSGMonthDay) NumericType.get_single_type(arg, typeof(XSGMonthDay));

			return calendar().Equals(val.calendar());
		}

		/// <summary>
		/// Retrieves the timezone associated with the date stored
		/// </summary>
		/// <returns> the timezone associated with the date stored </returns>
		public virtual XSDuration tz()
		{
			return _tz;
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_GMONTHDAY;
			}
		}
	}

}