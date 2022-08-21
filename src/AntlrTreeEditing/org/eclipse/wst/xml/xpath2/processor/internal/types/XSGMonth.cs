using System.Text;
using java.util;

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
///     David Carver (STAR) - bug 262765 - Fixed parsing of gMonth values
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
	/// A representation of the gMonth datatype
	/// </summary>
	public class XSGMonth : CalendarType, CmpEq
	{

		private const string XS_G_MONTH = "xs:gMonth";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;

		/// <summary>
		/// Initializes a representation of the supplied month
		/// </summary>
		/// <param name="cal">
		///            Calendar representation of the month to be stored </param>
		/// <param name="tz">
		///            Timezone associated with this month </param>
		public XSGMonth(Calendar cal, XSDuration tz)
		{
			_calendar = cal;
			if (tz != null)
			{
				_timezoned = true;
				_tz = tz;
			}
		}

		/// <summary>
		/// Initialises a representation of the current month
		/// </summary>
		public XSGMonth() : this(new GregorianCalendar(TimeZone.getTimeZone("GMT")), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "gMonth" which is the datatype's name </returns>
		public override string type_name()
		{
			return "gMonth";
		}

		/// <summary>
		/// Parses a String representation of a month and constructs a new XSGMonth
		/// representation of it.
		/// </summary>
		/// <param name="str">
		///            The String representation of the month (and optional timezone) </param>
		/// <returns> The XSGMonth representation of the supplied date </returns>
		public static XSGMonth parse_gMonth(string str)
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
					if (index > 3)
					{
						zIndex = index;
					}
				}

				string[] split = str.Split("-", true);
				startdate += split[2].Replace("Z", "") + "-01";

				if (str.IndexOf('T') != -1)
				{
					if (split.Length > 3)
					{
						string[] timesplit = split[3].Split(":", true);
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

			return new XSGMonth(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable gMonth in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the gMonth is to be extracted </param>
		/// <returns> New ResultSequence consisting of the supplied month </returns>
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

			XSGMonth val = castGMonth(aat);

			if (val == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return val;
		}

		protected internal virtual bool isGDataType(AnyAtomicType aat)
		{
			string type = aat.string_type();
			if (type.Equals("xs:gMonthDay") || type.Equals("xs:gDay") || type.Equals("xs:gYear") || type.Equals("xs:gYearMonth"))
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

			if (aat is XSDate || aat is XSDateTime || aat is XSGMonth)
			{
				return true;
			}

			return false;
		}

		private XSGMonth castGMonth(AnyAtomicType aat)
		{
			if (aat is XSGMonth)
			{
				XSGMonth gm = (XSGMonth) aat;
				return new XSGMonth(gm.calendar(), gm.tz());
			}

			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSGMonth(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSGMonth(dateTime.calendar(), dateTime.tz());
			}

			return parse_gMonth(aat.StringValue);
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
		/// Check for whether a timezone was specified at creation
		/// </summary>
		/// <returns> True if a timezone was specified. False otherwise </returns>
		public virtual bool timezoned()
		{
			return _timezoned;
		}

		/// <summary>
		/// Retrieves a String representation of the stored month
		/// </summary>
		/// <returns> String representation of the stored month </returns>
		public override string StringValue
		{
			get
			{
				string ret = "--";
    
				ret += XSDateTime.pad_int(month(), 2);
    
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
		/// <returns> "xs:gMonth" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_G_MONTH;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the month stored
		/// </summary>
		/// <returns> Calendar representation of the month stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation. This
		/// representation must be of type XSGMonth
		/// </summary>
		/// <param name="arg">
		///            The XSGMonth to compare with </param>
		/// <returns> True if the two representations are of the same month. False
		///         otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSGMonth val = (XSGMonth) NumericType.get_single_type(arg, typeof(XSGMonth));
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
				return BuiltinTypeLibrary.XS_GMONTH;
			}
		}
	}

}