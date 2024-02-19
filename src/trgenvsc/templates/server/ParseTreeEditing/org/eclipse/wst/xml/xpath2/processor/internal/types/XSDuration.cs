using System;

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
///     David Carver (STAR) - bug 282223 - Implemented XSDuration type for castable checking.
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
	using CmpGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpGt;
	using CmpLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpLt;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the xs:duration data type. Other duration implementations
	/// should inherit from this implementation.
	/// 
	/// @since 1.1 This used to be an abstract class but was incorrectly implemented
	///        as such.
	/// </summary>
	public class XSDuration : CtrType, CmpEq, CmpLt, CmpGt, ICloneable
	{

		private const string XS_DURATION = "xs:duration";
		protected internal int _year;
		protected internal int _month;
		protected internal int _days;
		protected internal int _hours;
		protected internal int _minutes;
		protected internal double _seconds;
		protected internal bool _negative;

		/// <summary>
		/// Initializes to the supplied parameters. If more than 24 hours is
		/// supplied, the number of days is adjusted accordingly. The same occurs for
		/// minutes and seconds
		/// </summary>
		/// <param name="years">
		///            Number of years in this duration of time. </param>
		/// <param name="months">
		///            Number of months in this duration of time. </param>
		/// <param name="days">
		///            Number of days in this duration of time </param>
		/// <param name="hours">
		///            Number of hours in this duration of time </param>
		/// <param name="minutes">
		///            Number of minutes in this duration of time </param>
		/// <param name="seconds">
		///            Number of seconds in this duration of time </param>
		/// <param name="negative">
		///            True if this duration of time represents a backwards passage
		///            through time. False otherwise </param>
		public XSDuration(int years, int months, int days, int hours, int minutes, double seconds, bool negative)
		{
			_year = years;
			_month = months;
			_days = days;
			_hours = hours;
			_minutes = minutes;
			_seconds = seconds;
			_negative = negative;

			if (_month >= 12)
			{
				_year += _month / 12;
				_month = _month % 12;
			}

			if (_seconds >= 60)
			{
				int isec = (int) _seconds;
				double rem = _seconds - (isec);

				_minutes += isec / 60;
				_seconds = isec % 60;
				_seconds += rem;
			}
			if (_minutes >= 60)
			{
				_hours += _minutes / 60;
				_minutes = _minutes % 60;
			}
			if (_hours >= 24)
			{
				_days += _hours / 24;
				_hours = _hours % 24;
			}

		}

		/// <summary>
		/// Initialises to the given number of seconds
		/// </summary>
		/// <param name="secs">
		///            Number of seconds in the duration of time </param>
		public XSDuration(double secs) : this(0, 0, 0, 0, 0, Math.Abs(secs), secs < 0)
		{
		}

		/// <summary>
		/// Initialises to a duration of no time (0days, 0hours, 0minutes, 0seconds)
		/// </summary>
		public XSDuration() : this(0, 0, 0, 0, 0, 0.0, false)
		{
		}

		public override string type_name()
		{
			return "duration";
		}

		public override string string_type()
		{
			return XS_DURATION;
		}

		/// <summary>
		/// Retrieves a String representation of the duration stored
		/// </summary>
		/// <returns> String representation of the duration stored </returns>
		public override string StringValue
		{
			get
			{
				string ret = "";
				bool did_something = false;
				string tret = "";
    
				if (negative() && !(days() == 0 && hours() == 0 && seconds() == 0))
				{
					ret += "-";
				}
    
				ret += "P";
    
				int years = year();
				if (years != 0)
				{
					ret += years + "Y";
				}
    
				int months = month();
				if (months != 0)
				{
					ret += months + "M";
				}
    
				if (days() != 0)
				{
					ret += days() + "D";
					did_something = true;
				}
    
				// do the "time" bit
				int xhours = hours();
				int xminutes = minutes();
				double xseconds = seconds();
    
				if (xhours != 0)
				{
					tret += xhours + "H";
					did_something = true;
				}
				if (xminutes != 0)
				{
					tret += xminutes + "M";
					did_something = true;
				}
				if (xseconds != 0)
				{
					string doubStr = ((new double?(xseconds)).ToString());
					if (doubStr.EndsWith(".0", StringComparison.Ordinal))
					{
						// string value of x.0 seconds is xS. e.g, 7.0S is converted to
						// 7S.
						tret += doubStr.Substring(0, doubStr.IndexOf(".0", StringComparison.Ordinal)) + "S";
					}
					else
					{
						tret += xseconds + "S";
					}
					did_something = true;
				}
				else if (!did_something)
				{
						tret += "0S";
				}
    
				if ((year() == 0 && month() == 0) || (xhours > 0 || xminutes > 0 || xseconds > 0))
				{
					if (tret.Length > 0)
					{
						ret += "T" + tret;
					}
				}
    
				return ret;
			}
		}

		/// <summary>
		/// Retrieves the number of days within the duration of time stored
		/// </summary>
		/// <returns> Number of days within the duration of time stored </returns>
		public virtual int days()
		{
			return _days;
		}

		/// <summary>
		/// Retrieves the number of minutes (max 60) within the duration of time
		/// stored
		/// </summary>
		/// <returns> Number of minutes within the duration of time stored </returns>
		public virtual int minutes()
		{
			return _minutes;
		}

		/// <summary>
		/// Retrieves the number of hours (max 24) within the duration of time stored
		/// </summary>
		/// <returns> Number of hours within the duration of time stored </returns>
		public virtual int hours()
		{
			return _hours;
		}

		/// <summary>
		/// Retrieves the number of seconds (max 60) within the duration of time
		/// stored
		/// </summary>
		/// <returns> Number of seconds within the duration of time stored </returns>
		public virtual double seconds()
		{
			return _seconds;
		}

		/// <summary>
		/// Equality comparison between this and the supplied duration of time.
		/// </summary>
		/// <param name="arg">
		///            The duration of time to compare with </param>
		/// <returns> True if they both represent the duration of time. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDuration));

			return value() == val.value();
		}

		/// <summary>
		/// Comparison between this and the supplied duration of time.
		/// </summary>
		/// <param name="arg">
		///            The duration of time to compare with </param>
		/// <returns> True if the supplied time represents a larger duration than that
		///         stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDayTimeDuration));

			return value() < val.value();
		}

		/// <summary>
		/// Comparison between this and the supplied duration of time.
		/// </summary>
		/// <param name="arg">
		///            The duration of time to compare with </param>
		/// <returns> True if the supplied time represents a smaller duration than that
		///         stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDayTimeDuration));

			return value() > val.value();
		}

		/// <summary>
		/// Retrieves whether this duration represents a backward passage through
		/// time
		/// </summary>
		/// <returns> True if this duration represents a backward passage through time.
		///         False otherwise </returns>
		public virtual bool negative()
		{
			return _negative;
		}

		/// <summary>
		/// Retrieves the duration of time stored as the number of seconds within it
		/// </summary>
		/// <returns> Number of seconds making up this duration of time </returns>
		public virtual double value()
		{
			double ret = days() * 24 * 60 * 60;

			ret += hours() * 60 * 60;
			ret += minutes() * 60;
			ret += seconds();

			if (negative())
			{
				ret *= -1;
			}



			return ret;
		}

		public virtual double time_value()
		{
			double ret = 0;
			ret += hours() * 60 * 60;
			ret += minutes() * 60;
			ret += seconds();

			if (negative())
			{
				ret *= -1;
			}
			return ret;
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable time duration
		/// from the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to extract </param>
		/// <returns> New ResultSequence consisting of the time duration extracted </returns>
		/// <exception cref="DynamicError"> </exception>
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			AnyAtomicType aat = (AnyAtomicType) arg.first();

			if (aat is NumericType || aat is CalendarType || aat is XSBoolean || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (!(isCastable(aat)))
			{
				throw DynamicError.cant_cast(null);
			}

			XSDuration duration = castDuration(aat);

			if (duration == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return duration;
		}

		private XSDuration castDuration(AnyAtomicType aat)
		{
			if (aat is XSDuration)
			{
				XSDuration duration = (XSDuration) aat;
				return new XSDuration(duration.year(), duration.month(), duration.days(), duration.hours(), duration.minutes(), duration.seconds(), duration.negative());
			}

			return parseDTDuration(aat.StringValue);
		}
		/// <summary>
		/// Creates a new XSDayTimeDuration by parsing the supplied String
		/// represented duration of time
		/// </summary>
		/// <param name="str">
		///            String represented duration of time </param>
		/// <returns> New XSDayTimeDuration representing the duration of time supplied </returns>
		public static XSDuration parseDTDuration(string str)
		{
			bool negative = false;
			int years = 0;
			int months = 0;
			int days = 0;
			int hours = 0;
			int minutes = 0;
			double seconds = 0;

			// string following the P
			string pstr = "";
			string tstr = "";

			// get the negative and pstr
			if (str.StartsWith("-P", StringComparison.Ordinal))
			{
				negative = true;
				pstr = str.Substring(2, str.Length - 2);
			}
			else if (str.StartsWith("P", StringComparison.Ordinal))
			{
				negative = false;
				pstr = str.Substring(1, str.Length - 1);
			}
			else
			{
				return null;
			}

			try
			{
				int index = pstr.IndexOf('Y');
				bool did_something = false;

				if (index != -1)
				{
					string digit = pstr.Substring(0, index);
					years = int.Parse(digit);
					pstr = pstr.Substring(index + 1, pstr.Length - (index + 1));
					did_something = true;
				}

				index = pstr.IndexOf('M');
				if (index != -1)
				{
					string digit = pstr.Substring(0, index);
					months = int.Parse(digit);
					pstr = pstr.Substring(index + 1, pstr.Length - (index + 1));
					did_something = true;
				}

				// get the days
				index = pstr.IndexOf('D');

				if (index == -1)
				{
					if (pstr.StartsWith("T", StringComparison.Ordinal))
					{
						tstr = pstr.Substring(1, pstr.Length - 1);
					}
				}
				else
				{
					string digit = pstr.Substring(0, index);
					days = int.Parse(digit);
					tstr = pstr.Substring(index + 1, pstr.Length - (index + 1));

					if (tstr.StartsWith("T", StringComparison.Ordinal))
					{
						tstr = tstr.Substring(1, tstr.Length - 1);
					}
					else
					{
						tstr = "";
						did_something = true;
					}
				}

				// do the T str

				// hour
				index = tstr.IndexOf('H');
				if (index != -1)
				{
					string digit = tstr.Substring(0, index);
					hours = int.Parse(digit);
					tstr = tstr.Substring(index + 1, tstr.Length - (index + 1));
					did_something = true;
				}
				// minute
				index = tstr.IndexOf('M');
				if (index != -1)
				{
					string digit = tstr.Substring(0, index);
					minutes = int.Parse(digit);
					tstr = tstr.Substring(index + 1, tstr.Length - (index + 1));
					did_something = true;
				}
				// seconds
				index = tstr.IndexOf('S');
				if (index != -1)
				{
					string digit = tstr.Substring(0, index);
					seconds = double.Parse(digit);
					tstr = tstr.Substring(index + 1, tstr.Length - (index + 1));
					did_something = true;
				}
				if (!did_something)
				{
					return null;
				}

			}
			catch (System.FormatException)
			{
				return null;
			}

			return new XSDuration(years, months, days, hours, minutes, seconds, negative);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
			return new XSDuration(year(), month(), days(), hours(), minutes(), seconds(), negative());
		}

		/// <summary>
		/// Retrieves the number of years within the duration of time stored
		/// </summary>
		/// <returns> Number of years within the duration of time stored </returns>
		public virtual int year()
		{
			return _year;
		}

		/// <summary>
		/// Retrieves the number of months within the duration of time stored
		/// </summary>
		/// <returns> Number of months within the duration of time stored </returns>
		public virtual int month()
		{
			return _month;
		}

		protected internal virtual bool isCastable(AnyAtomicType aat)
		{
			string value = aat.StringValue; // get this once so we don't recreate everytime.
			string type = aat.string_type();
			if (type.Equals("xs:string") || type.Equals("xs:untypedAtomic"))
			{
				if (isDurationValue(value))
				{
					return true; // We might be able to cast this.
				}
			}

			// We can cast from ourself or derivations of ourselves.
			if (aat is XSDuration)
			{
				return true;
			}

			return false;
		}

		private bool isDurationValue(string value)
		{
			return value.StartsWith("P", StringComparison.Ordinal) || value.StartsWith("-P", StringComparison.Ordinal);
		}


		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_DURATION;
			}
		}

		public override object NativeValue
		{
			get
			{
				return _datatypeFactory.newDuration(!negative(), year(), month(), days(), hours(), minutes(), (int)seconds());
			}
		}

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

}