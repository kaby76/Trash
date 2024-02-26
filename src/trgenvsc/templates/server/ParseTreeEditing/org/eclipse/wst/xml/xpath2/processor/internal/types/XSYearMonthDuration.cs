using System;
using System.Diagnostics;

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
///     Mukul Gandhi - bug 279373 - improvements to multiply operation on xs:yearMonthDuration
///                                 data type.
///     David Carver - bug 282223 - implementation of xs:duration data type.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
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
	/// A representation of the YearMonthDuration datatype
	/// </summary>
	public class XSYearMonthDuration : XSDuration, CmpEq, CmpLt, CmpGt, MathPlus, MathMinus, MathTimes, MathDiv
	{


		private const string XS_YEAR_MONTH_DURATION = "xs:yearMonthDuration";

		/// <summary>
		/// Initialises using the supplied parameters. If the number of months
		/// supplied is more than 12, the number of years is adjusted accordingly.
		/// </summary>
		/// <param name="year">
		///            Number of years in this duration of time </param>
		/// <param name="month">
		///            Number of months in this duration of time </param>
		/// <param name="negative">
		///            True if this duration of time represents a backwards passage
		///            through time. False otherwise </param>
		public XSYearMonthDuration(int year, int month, bool negative) : base(year, month, 0, 0, 0, 0, negative)
		{
		}

		/// <summary>
		/// Initialises to the given number of months
		/// </summary>
		/// <param name="months">
		///            Number of months in the duration of time </param>
		public XSYearMonthDuration(int months) : this(0, Math.Abs(months), months < 0)
		{
		}

		/// <summary>
		/// Initialises to a duration of no time (0years and 0months)
		/// </summary>
		public XSYearMonthDuration() : this(0, 0, false)
		{
		}

		/// <summary>
		/// Creates a new XSYearMonthDuration by parsing the supplied String
		/// represented duration of time
		/// </summary>
		/// <param name="str">
		///            String represented duration of time </param>
		/// <returns> New XSYearMonthDuration representing the duration of time
		///         supplied </returns>
		public static XSDuration parseYMDuration(string str)
		{
			bool negative = false;
			int year = 0;
			int month = 0;

			int state = 0; // 0 beginning
			// 1 year
			// 2 month
			// 3 done
			// 4 expecting P
			// 5 expecting Y or M
			// 6 expecting M or end
			// 7 expecting end

			string digits = "";
			for (int i = 0; i < str.Length; i++)
			{
				char x = str[i];

				switch (state)
				{
				// beginning
				case 0:
					if (x == '-')
					{
						negative = true;
						state = 4;
					}
					else if (x == 'P')
					{
						state = 5;
					}
					else
					{
						return null;
					}
					break;

				case 4:
					if (x == 'P')
					{
						state = 5;
					}
					else
					{
						return null;
					}
					break;

				case 5:
					if ('0' <= x && x <= '9')
					{
						digits += x;
					}
					else if (x == 'Y')
					{
						if (digits.Length == 0)
						{
							return null;
						}
						year = int.Parse(digits);
						digits = "";
						state = 6;
					}
					else if (x == 'M')
					{
						if (digits.Length == 0)
						{
							return null;
						}
						month = int.Parse(digits);
						state = 7;
					}
					else
					{
						return null;
					}
					break;

				case 6:
					if ('0' <= x && x <= '9')
					{
						digits += x;
					}
					else if (x == 'M')
					{
						if (digits.Length == 0)
						{
							return null;
						}
						month = int.Parse(digits);
						state = 7;

					}
					else
					{
						return null;
					}
					break;

				case 7:
					return null;

				default:
					Debug.Assert(false);
					return null;

				}
			}

			return new XSYearMonthDuration(year, month, negative);
		}

		/// <summary>
		/// Retrives the datatype's name
		/// </summary>
		/// <returns> "yearMonthDuration" which is the datatype's name </returns>
		public override string type_name()
		{
			return "yearMonthDuration";
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable time duration
		/// from the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to extract </param>
		/// <returns> New ResultSequence consisting of the time duration extracted </returns>
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
			if (aat is NumericType || aat is CalendarType || aat is XSBoolean || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}

			XSDuration ymd = castYearMonthDuration(aat);

			if (ymd == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return ymd;
		}

		private XSDuration castYearMonthDuration(AnyAtomicType aat)
		{
			if (aat is XSDuration)
			{
				XSDuration duration = (XSDuration) aat;
				return new XSYearMonthDuration(duration.year(), duration.month(), duration.negative());
			}

			return parseYMDuration(aat.StringValue);
		}

		/// <summary>
		/// Retrieves whether this duration represents a backward passage through
		/// time
		/// </summary>
		/// <returns> True if this duration represents a backward passage through time.
		///         False otherwise </returns>
		public override bool negative()
		{
			return _negative;
		}

		/// <summary>
		/// Retrieves a String representation of the duration of time stored
		/// </summary>
		/// <returns> String representation of the duration of time stored </returns>
		public override string StringValue
		{
			get
			{
				string strval = "";
    
				if (negative())
				{
					strval += "-";
				}
    
				strval += "P";
    
				int years = year();
				if (years != 0)
				{
					strval += years + "Y";
				}
    
				int months = month();
				if (months == 0)
				{
					if (years == 0)
					{
						strval += months + "M";
					}
				}
				else
				{
					strval += months + "M";
				}
    
				return strval;
			}
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:yearMonthDuration" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_YEAR_MONTH_DURATION;
		}

		/// <summary>
		/// Retrieves the duration of time stored as the number of months within it
		/// </summary>
		/// <returns> Number of months making up this duration of time </returns>
		public virtual int monthValue()
		{
			int ret = year() * 12 + month();

			if (negative())
			{
				ret *= -1;
			}

			return ret;
		}

		/// <summary>
		/// Equality comparison between this and the supplied duration of time.
		/// </summary>
		/// <param name="arg">
		///            The duration of time to compare with </param>
		/// <returns> True if they both represent the duration of time. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			if (arg is XSDayTimeDuration)
			{
				XSDayTimeDuration dayTimeDuration = (XSDayTimeDuration)arg;
				return (monthValue() == 0 && dayTimeDuration.value() == 0.0);
			}
			else if (arg is XSYearMonthDuration)
			{
				XSYearMonthDuration yearMonthDuration = (XSYearMonthDuration)arg;
				return monthValue() == yearMonthDuration.monthValue();
			}
			XSDuration val = (XSDuration) NumericType.get_single_type(arg, typeof(XSDuration));
			return base.eq(val, dynamicContext);
		}

		/// <summary>
		/// Comparison between this and the supplied duration of time.
		/// </summary>
		/// <param name="arg">
		///            The duration of time to compare with </param>
		/// <returns> True if the supplied time represents a larger duration than that
		///         stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool lt(AnyType arg, DynamicContext context)
		{
			XSYearMonthDuration val = (XSYearMonthDuration) NumericType.get_single_type(arg, typeof(XSYearMonthDuration));

			return monthValue() < val.monthValue();
		}

		/// <summary>
		/// Comparison between this and the supplied duration of time.
		/// </summary>
		/// <param name="arg">
		///            The duration of time to compare with </param>
		/// <returns> True if the supplied time represents a smaller duration than that
		///         stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool gt(AnyType arg, DynamicContext context)
		{
			XSYearMonthDuration val = (XSYearMonthDuration) NumericType.get_single_type(arg, typeof(XSYearMonthDuration));

			return monthValue() > val.monthValue();
		}

		/// <summary>
		/// Mathematical addition between this duration stored and the supplied
		/// duration of time (of type XSYearMonthDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to add </param>
		/// <returns> New XSYearMonthDuration representing the resulting duration
		///         after the addition </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence plus(ResultSequence arg)
		{
			XSYearMonthDuration val = (XSYearMonthDuration) NumericType.get_single_type(arg, typeof(XSYearMonthDuration));

			int res = monthValue() + val.monthValue();

			return ResultSequenceFactory.create_new(new XSYearMonthDuration(res));
		}

		/// <summary>
		/// Mathematical subtraction between this duration stored and the supplied
		/// duration of time (of type XSYearMonthDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to subtract </param>
		/// <returns> New XSYearMonthDuration representing the resulting duration
		///         after the subtraction </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual ResultSequence minus(ResultSequence arg)
		{
			XSYearMonthDuration val = (XSYearMonthDuration) NumericType.get_single_type(arg, typeof(XSYearMonthDuration));

			int res = monthValue() - val.monthValue();

			return ResultSequenceFactory.create_new(new XSYearMonthDuration(res));
		}

		/// <summary>
		/// Mathematical multiplication between this duration stored and the supplied
		/// duration of time (of type XSYearMonthDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to multiply by </param>
		/// <returns> New XSYearMonthDuration representing the resulting duration
		///         after the multiplication </returns>
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

			if (val.infinite())
			{
				throw DynamicError.overflowDateTime();
			}

			int res = (int) Math.Round(monthValue() * val.double_value());

			return ResultSequenceFactory.create_new(new XSYearMonthDuration(res));
		}

		/// <summary>
		/// Mathematical division between this duration stored and the supplied
		/// duration of time (of type XSYearMonthDuration)
		/// </summary>
		/// <param name="arg">
		///            The duration of time to divide by </param>
		/// <returns> New XSYearMonthDuration representing the resulting duration
		///         after the division </returns>
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

				int ret = 0;

				if (!dt.zero())
				{
					ret = (int) Math.Round(monthValue() / dt.double_value());
				}

				return ResultSequenceFactory.create_new(new XSYearMonthDuration(ret));
			}
			else if (at is XSDecimal)
			{
				XSDecimal dt = (XSDecimal) at;

				int ret = 0;

				if (!dt.zero())
				{
					ret = (int) Math.Round(monthValue() / dt.Value);
				}

				return ResultSequenceFactory.create_new(new XSYearMonthDuration(ret));
			}
			else if (at is XSYearMonthDuration)
			{
				XSYearMonthDuration md = (XSYearMonthDuration) at;

				double res = (double) monthValue() / md.monthValue();

				return ResultSequenceFactory.create_new(new XSDecimal(new decimal(res)));
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
				return BuiltinTypeLibrary.XS_YEARMONTHDURATION;
			}
		}

	}

}