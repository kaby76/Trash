using System;
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
///     David Carver (STAR) - bug 282223 - fixed casting issues. 
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
	/// A representation of the YearMonth datatype
	/// </summary>
	public class XSGYearMonth : CalendarType, CmpEq
	{

		private const string XS_G_YEAR_MONTH = "xs:gYearMonth";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;


		/// <summary>
		/// Initialises a representation of the supplied year and month
		/// </summary>
		/// <param name="cal">
		///            Calendar representation of the year and month to be stored </param>
		/// <param name="tz">
		///            Timezone associated with this year and month </param>
		public XSGYearMonth(Calendar cal, XSDuration tz)
		{
			_calendar = cal;
			if (tz != null)
			{
				_timezoned = true;
				_tz = tz;
			}
		}

		/// <summary>
		/// Initialises a representation of the current year and month
		/// </summary>
		public XSGYearMonth() : this(new GregorianCalendar(TimeZone.CurrentTimeZone), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "gYearMonth" which is the datatype's name </returns>
		public override string type_name()
		{
			return "gYearMonth";
		}

		/// <summary>
		/// Parses a String representation of a year and month and constructs a new
		/// XSGYearMonth representation of it.
		/// </summary>
		/// <param name="str">
		///            The String representation of the year and month (and optional
		///            timezone) </param>
		/// <returns> The XSGYearMonth representation of the supplied date </returns>
		public static XSGYearMonth parse_gYearMonth(string str)
		{

			string yearMonth = "";
			string dayTime = "-01T00:00:00.0";

			int index = str.IndexOf('+', 1);
			if (index == -1)
			{
				index = str.IndexOf('-', 1);
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
				yearMonth = str.Substring(0, index);
				yearMonth += dayTime;
				yearMonth += str.Substring(index, str.Length - index);
			}
			else
			{
				yearMonth = str + dayTime;
			}

			XSDateTime dt = XSDateTime.parseDateTime(yearMonth);
			if (dt == null)
			{
				return null;
			}

			return new XSGYearMonth(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable gYearMonth in
		/// the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the gYearMonth is to be
		///            extracted </param>
		/// <returns> New ResultSequence consisting of the supplied year and month </returns>
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

			XSGYearMonth val = castGYearMonth(aat);

			if (val == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return val;
		}

		protected internal virtual bool isGDataType(AnyAtomicType aat)
		{
			string type = aat.string_type();
			if (type.Equals("xs:gMonthDay") || type.Equals("xs:gDay") || type.Equals("xs:gMonth") || type.Equals("xs:gYear"))
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

			if (aat is XSGYearMonth)
			{
				return true;
			}

			if (aat is XSDate)
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

			return false;

		}

		private XSGYearMonth castGYearMonth(AnyAtomicType aat)
		{
			if (aat is XSGYearMonth)
			{
				XSGYearMonth gym = (XSGYearMonth) aat;
				return new XSGYearMonth(gym.calendar(), gym.tz());
			}

			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSGYearMonth(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSGYearMonth(dateTime.calendar(), dateTime.tz());
			}

			return parse_gYearMonth(aat.StringValue);
		}

		/// <summary>
		/// Retrieves the actual year as an integer
		/// </summary>
		/// <returns> The actual year as an integer </returns>
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
		/// Retrieves a String representation of the stored year and month
		/// </summary>
		/// <returns> String representation of the stored year and month </returns>
		public override string StringValue
		{
			get
			{
				string ret = "";
    
				ret += XSDateTime.pad_int(year(), 4);
    
				ret += "-";
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
		/// <returns> "xs:gYearMonth" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_G_YEAR_MONTH;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the year and month stored
		/// </summary>
		/// <returns> Calendar representation of the year and month stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation. This
		/// representation must be of type XSGYearMonth
		/// </summary>
		/// <param name="arg">
		///            The XSGYearMonth to compare with </param>
		/// <returns> True if the two representations are of the same year and month.
		///         False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSGYearMonth val = (XSGYearMonth) NumericType.get_single_type(arg, typeof(XSGYearMonth));
			Calendar thiscal = normalizeCalendar(calendar(), tz());
            Calendar thatcal = normalizeCalendar(val.calendar(), val.tz());

			return thiscal.Equals(thatcal);
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
				return BuiltinTypeLibrary.XS_GYEARMONTH;
			}
		}
	}

}