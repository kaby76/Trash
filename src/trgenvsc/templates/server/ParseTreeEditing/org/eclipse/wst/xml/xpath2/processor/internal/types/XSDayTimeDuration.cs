using System;
using javax.xml.datatype;

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
///     Mukul Gandhi - bug 279377 - improvements to multiplication and division operations
///                                 on xs:dayTimeDuration.
///     David Carver - bug 282223 - implementation of xs:duration
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using Duration = javax.xml.datatype.Duration;

	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using CmpGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpGt;
	using CmpLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpLt;
	using MathDiv = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathDiv;
	using MathMinus = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathMinus;
	using MathPlus = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathPlus;
	using MathTimes = org.eclipse.wst.xml.xpath2.processor.@internal.function.MathTimes;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the DayTimeDuration datatype
	/// </summary>
	public class XSDayTimeDuration : XSDuration, CmpEq, CmpLt, CmpGt, MathPlus, MathMinus, MathTimes, MathDiv, ICloneable
	{


		private const string XS_DAY_TIME_DURATION = "xs:dayTimeDuration";

		/// <summary>
		/// Initialises to the supplied parameters. If more than 24 hours is
		/// supplied, the number of days is adjusted acordingly. The same occurs for
		/// minutes and seconds
		/// </summary>
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
		public XSDayTimeDuration(int days, int hours, int minutes, double seconds, bool negative) : base(0, 0, days, hours, minutes, seconds, negative)
		{
		}

		/// <summary>
		/// Initialises to the given number of seconds
		/// </summary>
		/// <param name="secs">
		///            Number of seconds in the duration of time </param>
		public XSDayTimeDuration(double secs) : base(0, 0, 0, 0, 0, Math.Abs(secs), secs < 0)
		{
		}

		/// <summary>
		/// Initialises to a duration of no time (0days, 0hours, 0minutes, 0seconds)
		/// </summary>
		public XSDayTimeDuration() : base(0, 0, 0, 0, 0, 0.0, false)
		{
		}

		public XSDayTimeDuration(Duration d) : this(d.Days, d.Hours, d.Minutes, 0.0, d.Sign == -1)
		{
		}

		/// <summary>
		/// Creates a copy of this representation of a time duration
		/// </summary>
		/// <returns> New XSDayTimeDuration representing the duration of time stored </returns>
		/// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public override object clone()
		{
			return new XSDayTimeDuration(days(), hours(), minutes(), seconds(), negative());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
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

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}

			XSDuration dtd = castDayTimeDuration(aat);

			if (dtd == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return dtd;
		}

		private XSDuration castDayTimeDuration(AnyAtomicType aat)
		{
			if (aat is XSDuration)
			{
				XSDuration duration = (XSDuration) aat;
				return new XSDayTimeDuration(duration.days(), duration.hours(), duration.minutes(), duration.seconds(), duration.negative());
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
		public new static XSDuration parseDTDuration(string str)
		{
			bool negative = false;
			int days = 0;
			int hours = 0;
			int minutes = 0;
			double seconds = 0;

			// string following the P
			string pstr = null;
			string tstr = null;

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
				// get the days
				int index = pstr.IndexOf('D');
				bool did_something = false;

				// no D... must have T
				if (index == -1)
				{
					if (pstr.StartsWith("T", StringComparison.Ordinal))
					{
						tstr = pstr.Substring(1, pstr.Length - 1);
					}
					else
					{
						return null;
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
						if (tstr.Length > 0)
						{
							return null;
						}
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
				if (did_something)
				{
					// make sure we parsed it all
					if (tstr.Length != 0)
					{
						return null;
					}
				}
				else
				{
					return null;
				}

			}
			catch (System.FormatException)
			{
				return null;
			}

			return new XSDayTimeDuration(days, hours, minutes, seconds, negative);
		}

		/// <summary>
		/// Retrives the datatype's name
		/// </summary>
		/// <returns> "dayTimeDuration" which is the datatype's name </returns>
		public override string type_name()
		{
			return "dayTimeDuration";
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:dayTimeDuration" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_DAY_TIME_DURATION;
		}

		/// <summary>
		/// Mathematical addition between this duration stored and the supplied
		/// duration of time (of type XSDayTimeDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to add </param>
		/// <returns> New XSDayTimeDuration representing the resulting duration after
		///         the addition </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence plus(ResultSequence arg)
		{
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDayTimeDuration));

			double res = value() + val.value();

			return ResultSequenceFactory.create_new(new XSDayTimeDuration(res));
		}

		/// <summary>
		/// Mathematical subtraction between this duration stored and the supplied
		/// duration of time (of type XSDayTimeDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to subtract </param>
		/// <returns> New XSDayTimeDuration representing the resulting duration after
		///         the subtraction </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence minus(ResultSequence arg)
		{
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDayTimeDuration));

			double res = value() - val.value();

			return ResultSequenceFactory.create_new(new XSDayTimeDuration(res));
		}

		/// <summary>
		/// Mathematical multiplication between this duration stored and the supplied
		/// duration of time (of type XSDayTimeDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to multiply by </param>
		/// <returns> New XSDayTimeDuration representing the resulting duration after
		///         the multiplication </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence times(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence times(ResultSequence arg)
		{
			ResultSequence convertedRS = arg;

			if (arg.size() == 1)
			{
				Item argValue = arg.first();
				if (argValue is XSDecimal)
				{
					convertedRS = ResultSequenceFactory.create_new(new XSDouble(argValue.StringValue));
				}
			}

			XSDouble val = (XSDouble) NumericType.get_single_type(convertedRS, typeof(XSDouble));
			if (val.nan())
			{
				throw DynamicError.nan();
			}

			double res = value() * val.double_value();

			return ResultSequenceFactory.create_new(new XSDayTimeDuration(res));
		}

		/// <summary>
		/// Mathematical division between this duration stored and the supplied
		/// duration of time (of type XSDayTimeDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to divide by </param>
		/// <returns> New XSDayTimeDuration representing the resulting duration after
		///         the division </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence div(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence div(ResultSequence arg)
		{
			if (arg.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			Item at = arg.first();

			if (at is XSDouble)
			{
				XSDouble dt = (XSDouble) at;
				double retval = 0;

				if (dt.nan())
				{
					throw DynamicError.nan();
				}

				if (!dt.zero())
				{
					decimal ret = new decimal(0);

					if (dt.infinite())
					{
						retval = value() / dt.double_value();
					}
					else
					{
						ret = new decimal(value());
						ret = decimal.Divide(ret, new decimal(dt.double_value()));
						var x = ret.ToString();
                        double.TryParse(x, out double r);
						retval = r;
					}
				}
				else
				{
					throw DynamicError.overflowUnderflow();
				}

				return ResultSequenceFactory.create_new(new XSDayTimeDuration(retval));
			}
			else if (at is XSDecimal)
			{
				XSDecimal dt = (XSDecimal) at;

				decimal ret = new decimal(0);

				if (!dt.zero())
				{
					ret = new decimal(value());
					ret = decimal.Divide(ret, dt.Value);
				}
				else
				{
					throw DynamicError.overflowUnderflow();
				}

                var x = ret.ToString();
                double.TryParse(x, out double r);
                var i = (int) r;
				return ResultSequenceFactory.create_new(new XSDayTimeDuration(i));
			}
			else if (at is XSDayTimeDuration)
			{
				XSDuration md = (XSDuration) at;

				decimal res = default;
				res = new decimal(this.value());
				decimal l = new decimal(md.value());
				res = decimal.Divide(res, l);

				return ResultSequenceFactory.create_new(new XSDecimal(res));
			}
			else
			{
				DynamicError.throw_type_error();
				return null; // unreach
			}
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_DAYTIMEDURATION;
			}
		}

	}

}