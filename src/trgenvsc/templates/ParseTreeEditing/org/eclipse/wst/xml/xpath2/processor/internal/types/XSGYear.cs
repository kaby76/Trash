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
///     David Carver (STAR) - bug 282223 - fix casting issues. 
///     David Carver - bug 280547 - fix dates for comparison 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

using java.util;

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
	/// A representation of the gMonth datatype
	/// </summary>
	public class XSGYear : CalendarType, CmpEq
	{

		private const string XS_G_YEAR = "xs:gYear";
		private Calendar _calendar;
		private bool _timezoned;
		private XSDuration _tz;


		/// <summary>
		/// Initialises a representation of the supplied month
		/// </summary>
		/// <param name="cal">
		///            Calendar representation of the month to be stored </param>
		/// <param name="tz">
		///            Timezone associated with this month </param>
		public XSGYear(Calendar cal, XSDuration tz)
		{
			_calendar = cal;
			if (tz != null)
			{
				_timezoned = true;
				_tz = tz;
			}

		}

		/// <summary>
		/// Initialises a representation of the current year
		/// </summary>
		public XSGYear() : this(new GregorianCalendar(TimeZone.getTimeZone("GMT")), null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "gYear" which is the datatype's name </returns>
		public override string type_name()
		{
			return "gYear";
		}

		/// <summary>
		/// Parses a String representation of a year and constructs a new XSGYear
		/// representation of it.
		/// </summary>
		/// <param name="str">
		///            The String representation of the year (and optional timezone) </param>
		/// <returns> The XSGYear representation of the supplied date </returns>
		public static XSGYear parse_gYear(string str)
		{


			string year = "";
			string monthDaytime = "-01-01T00:00:00.0";


			int index = str.IndexOf('+', 1);
			if (index == -1)
			{
				index = str.IndexOf('-', 1);
			}
			if (index == -1)
			{
				index = str.IndexOf('Z', 1);
			}
			if (index != -1)
			{
				year = str.Substring(0, index);
				year += monthDaytime;
				year += str.Substring(index, str.Length - index);
			}
			else
			{
				year = str + monthDaytime;
			}

			XSDateTime dt = XSDateTime.parseDateTime(year);
			if (dt == null)
			{
				return null;
			}

			return new XSGYear(dt.calendar(), dt.tz());
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable gYear in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the gYear is to be extracted </param>
		/// <returns> New ResultSequence consisting of the supplied year </returns>
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

			XSGYear val = castGYear(aat);

			if (val == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return val;
		}

		protected internal virtual bool isGDataType(AnyAtomicType aat)
		{
			string type = aat.string_type();
			if (type.Equals("xs:gMonthDay") || type.Equals("xs:gDay") || type.Equals("xs:gMonth") || type.Equals("xs:gYearMonth"))
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

			if (aat is XSDate || aat is XSDateTime || aat is XSGYear)
			{
				return true;
			}

			return false;
		}

		private XSGYear castGYear(AnyAtomicType aat)
		{
			if (aat is XSGYear)
			{
				XSGYear gy = (XSGYear) aat;
				return new XSGYear(gy.calendar(), gy.tz());
			}

			if (aat is XSDate)
			{
				XSDate date = (XSDate) aat;
				return new XSGYear(date.calendar(), date.tz());
			}

			if (aat is XSDateTime)
			{
				XSDateTime dateTime = (XSDateTime) aat;
				return new XSGYear(dateTime.calendar(), dateTime.tz());
			}

			return parse_gYear(aat.StringValue);
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
		/// Check for whether a timezone was specified at creation
		/// </summary>
		/// <returns> True if a timezone was specified. False otherwise </returns>
		public virtual bool timezoned()
		{
			return _timezoned;
		}

		/// <summary>
		/// Retrieves a String representation of the stored year
		/// </summary>
		/// <returns> String representation of the stored year </returns>
		public override string StringValue
		{
			get
			{
				string ret = "";
    
				ret += XSDateTime.pad_int(year(), 4);
    
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
		/// <returns> "xs:gYear" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_G_YEAR;
		}

		/// <summary>
		/// Retrieves the Calendar representation of the year stored
		/// </summary>
		/// <returns> Calendar representation of the year stored </returns>
		public override Calendar calendar()
		{
			return _calendar;
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation. This
		/// representation must be of type XSGYear
		/// </summary>
		/// <param name="arg">
		///            The XSGYear to compare with </param>
		/// <returns> True if the two representations are of the same year. False
		///         otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSGYear val = (XSGYear) NumericType.get_single_type(arg, typeof(XSGYear));
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
				return BuiltinTypeLibrary.XS_GYEAR;
			}
		}
	}

}