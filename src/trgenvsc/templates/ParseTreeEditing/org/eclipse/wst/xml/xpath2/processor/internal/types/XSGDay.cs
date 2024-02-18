
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
///     David Carver (STAR) - bug 262765 - Correct parsing of Date to get day correctly.
///     David Carver (STAR) - bug 282223 - fixed issue with casting.
///     David Carver - bug 280547 - fix dates for comparison 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>


using System;
using System.Text;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{
	using Calendar = java.util.Calendar;
	using GregorianCalendar = java.util.GregorianCalendar;
	using TimeZone = java.util.TimeZone;

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the Day datatype
	/// </summary>
	public class XSGDay : CalendarType, CmpEq
	{

		private const string XS_G_DAY = "xs:gDay";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;

		/// <summary>
		/// Initializes a representation of the supplied day
		/// </summary>
		/// <param name="cal">
		///            Calendar representation of the day to be stored </param>
		/// <param name="tz">
		///            Timezone associated with this day </param>
		public XSGDay(Calendar cal, XSDuration tz)
		{
			_calendar = cal;
			if (tz != null)
			{
				_timezoned = true;
				_tz = tz;
			}
		}

		/// <summary>
		/// Initialises a representation of the current day
		/// </summary>
		public XSGDay() : this(new GregorianCalendar(TimeZone.getTimeZone("GMT")), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "gDay" which is the datatype's name </returns>
		public override string type_name()
		{
			return "gDay";
		}

		/// <summary>
		/// Parses a String representation of a day and constructs a new XSGDay
		/// representation of it.
		/// </summary>
		/// <param name="str">
		///            The String representation of the day (and optional timezone) </param>
		/// <returns> The XSGDay representation of the supplied date </returns>
		public static XSGDay parse_gDay(string str)
		{

			string startdate = "1972-12-";
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
					if (index > 4)
					{
						zIndex = index;
					}
				}
				if (zIndex == -1)
				{
					zIndex = str.LastIndexOf('+');
				}

				string[] split = str.Split("-", true);
				startdate += split[3].Replace("Z", "");

				if (str.IndexOf('T') != -1)
				{
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
				startdate += str + starttime;
			}

			XSDateTime dt = XSDateTime.parseDateTime(startdate);
			if (dt == null)
			{
				return null;
			}

			return new XSGDay(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable gDay in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the gDay is to be extracted </param>
		/// <returns> New ResultSequence consisting of the supplied day </returns>
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

			XSGDay val = castGDay(aat);

			if (val == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return val;
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

			if (aat is XSDate || aat is XSDateTime || aat is XSGDay)
			{
				return true;
			}

			return false;
		}

		protected internal virtual bool isGDataType(AnyAtomicType aat)
		{
			string type = aat.string_type();
			if (type.Equals("xs:gMonthDay") || type.Equals("xs:gMonth") || type.Equals("xs:gYear") || type.Equals("xs:gYearMonth"))
			{
				return true;
			}
			return false;
		}


		private XSGDay castGDay(AnyAtomicType aat)
		{
			if (aat is XSGDay)
			{
				XSGDay gday = (XSGDay) aat;
				return new XSGDay(gday.calendar(), gday.tz());
			}

			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSGDay(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSGDay(dateTime.calendar(), dateTime.tz());
			}
			return parse_gDay(aat.StringValue);
		}

		/// <summary>
		/// Retrieves the actual day as an integer
		/// </summary>
		/// <returns> The actual day as an integer </returns>
		public virtual int day()
		{
			return _calendar.get(Calendar.DAY_OF_MONTH);
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
		/// Retrieves a String representation of the stored day
		/// </summary>
		/// <returns> String representation of the stored day </returns>
		public override string StringValue
		{
			get
			{
				string ret = "---";
    
				Calendar adjustFortimezone = calendar();
    
				ret += XSDateTime.pad_int(adjustFortimezone.get(Calendar.DAY_OF_MONTH), 2);

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
		/// <returns> "xs:gDay" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_G_DAY;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the day stored
		/// </summary>
		/// <returns> Calendar representation of the day stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation. This
		/// representation must be of type XSGDay
		/// </summary>
		/// <param name="arg">
		///            The XSGDay to compare with </param>
		/// <returns> True if the two representations are of the same day. False
		///         otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSGDay val = (XSGDay) NumericType.get_single_type(arg, typeof(XSGDay));
			Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.Equals(thatcal);
		}

		/// <summary>
		/// Retrieves the timezone associated with the date stored
		/// </summary>
		/// <returns> the timezone associated with the date stored
		/// @since 1.1 </returns>
		public virtual XSDuration tz()
		{
			return _tz;
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_GDAY;
			}
		}

	}

}