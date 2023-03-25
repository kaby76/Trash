using System;
using System.Collections;
using System.Numerics;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2013 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type
///     David Carver (STAR) - bug 277774 - XSDecimal returning wrong values.
///     David Carver (STAR) - bug 262765 - various numeric formatting fixes and calculations
///     David Carver (STAR) - bug 282223 - Can't Cast Exponential values to Decimal values.
///     David Carver (STAR) - bug 262765 - fixed edge case where rounding was occuring when it shouldn't. 
///     Jesper Steen Moller - bug 262765 - fix type tests
///     David Carver (STAR) - bug 262765 - fixed abs value tests.
///     Jesper Steen Moller - bug 281028 - fixed division of zero (no, not by)
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
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the Decimal datatype
	/// </summary>
	public class XSDecimal : NumericType
	{

		private const string XS_DECIMAL = "xs:decimal";
		private decimal _value;
		private XPathDecimalFormat format = new XPathDecimalFormat("0.####################");

		/// <summary>
		/// Initiates a representation of 0.0
		/// </summary>
		public XSDecimal() : this(decimal.Zero)
		{
		}

		/// <summary>
		/// Initiates a representation of the supplied number
		/// </summary>
		/// <param name="x">
		///            Number to be stored </param>
		public XSDecimal(decimal x)
		{
			_value = x;
		}

		public XSDecimal(string x)
		{
			//_value = new BigDecimal(x, MathContext.DECIMAL128);
            decimal.TryParse(x, out _value);
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:decimal" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_DECIMAL;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "decimal" which is the datatype's name </returns>
		public override string type_name()
		{
			return "decimal";
		}

		/// <summary>
		/// Retrieves a String representation of the Decimal value stored
		/// </summary>
		/// <returns> String representation of the Decimal value stored </returns>
		public override string StringValue
		{
			get
			{
    
				if (zero())
				{
					return "0";
				}
    
				// strip trailing zeros
				//_value = new decimal((_value.ToString()).replaceFirst("0*", ""));
                throw new Exception();
				//return format.xpathFormat(_value);
			}
		}

		/// <summary>
		/// Check if this XSDecimal represents 0
		/// </summary>
		/// <returns> True if this XSDecimal represents 0. False otherwise </returns>
		public override bool zero()
		{
			return (_value.CompareTo(new decimal(0.0)) == 0);
		}

		/// <summary>
		/// Creates a new result sequence consisting of the retrievable decimal
		/// number in the supplied result sequence
		/// </summary>
		/// <param name="arg">
		///            The result sequence from which to extract the decimal number. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> A new result sequence consisting of the decimal number supplied. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			Item aat = arg.first();

			if (aat is XSDuration || aat is CalendarType || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (aat.StringValue.IndexOf("-INF", StringComparison.Ordinal) != -1)
			{
				throw DynamicError.cant_cast(null);
			}

			if (!isLexicalValue(aat.StringValue))
			{
				throw DynamicError.invalidLexicalValue();
			}

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}

			try
			{
				// XPath doesn't allow for converting Exponents to Decimal values.

				return castDecimal(aat);
			}
			catch (System.FormatException)
			{
				throw DynamicError.cant_cast(null);
			}

		}

		protected internal virtual bool isLexicalValue(string value)
		{
			if (value.Equals("inf", StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}

			if (value.Equals("-inf", StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}
			return true;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean isCastable(org.eclipse.wst.xml.xpath2.api.Item aat) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private bool isCastable(Item aat)
		{
			if (aat is XSBoolean || aat is NumericType)
			{
				return true;
			}

			if ((aat.StringValue.IndexOf("E", StringComparison.Ordinal) != -1) || (aat.StringValue.IndexOf("e", StringComparison.Ordinal) != -1))
			{
				return false;
			}

			if (aat is XSString || aat is XSUntypedAtomic || aat is NodeType)
			{
				return true;
			}
			return false;
		}

		private XSDecimal castDecimal(Item aat)
		{
			if (aat is XSBoolean)
			{
				if (aat.StringValue.Equals("true"))
                {
                    decimal.TryParse("1", out decimal v);
					return new XSDecimal(v);
				}
				else
				{
                    decimal.TryParse("0", out decimal v);
                    return new XSDecimal(v);
				}
			}
			return new XSDecimal(aat.StringValue);
		}

		/// <summary>
		/// Retrieves the actual value of the number stored
		/// </summary>
		/// <returns> The actual value of the number stored </returns>
		/// @deprecated Use getValue() instead. 
		public virtual double double_value()
		{
			return (double) _value;
		}

		public virtual decimal Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Sets the number stored to that supplied
		/// </summary>
		/// <param name="x">
		///            Number to be stored </param>
		public virtual void set_double(double x)
		{
			_value = new decimal(x);
		}

		// comparisons
		/// <summary>
		/// Equality comparison between this number and the supplied representation. </summary>
		/// <param name="at">
		///            Representation to be compared with (must currently be of type
		///            XSDecimal)
		/// 
		/// </param>
		/// <returns> True if the 2 representation represent the same number. False
		///         otherwise </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType at, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool eq(AnyType at, DynamicContext dynamicContext)
		{
			XSDecimal dt = null;
			if (!(at is XSDecimal))
			{
				ResultSequence rs = ResultSequenceFactory.create_new(at);

				ResultSequence crs = constructor(rs);
				if (crs.empty())
				{
					throw DynamicError.throw_type_error();
				}

				Item cat = crs.first();

				dt = (XSDecimal) cat;
			}
			else
			{
				dt = (XSDecimal) at;
			}
			return (_value.CompareTo(dt.Value) == 0);
		}

		/// <summary>
		/// Comparison between this number and the supplied representation. 
		/// </summary>
		/// <param name="arg">
		///            Representation to be compared with (must currently be of type
		///            XSDecimal) </param>
		/// <returns> True if the supplied type represents a number smaller than this
		///         one stored. False otherwise </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool gt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);

			XSDecimal val = (XSDecimal) get_single_type(carg, typeof(XSDecimal));
			return (_value.CompareTo(val.Value) == 1);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.eclipse.wst.xml.xpath2.api.Item convertArg(AnyType arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		protected internal virtual Item convertArg(AnyType arg)
		{
			ResultSequence rs = ResultSequenceFactory.create_new(arg);
			rs = constructor(rs);
			Item carg = rs.first();
			return carg;
		}

		/// <summary>
		/// Comparison between this number and the supplied representation. 
		/// </summary>
		/// <param name="arg">
		///            Representation to be compared with (must currently be of type
		///            XSDecimal) </param>
		/// <returns> True if the supplied type represents a number greater than this
		///         one stored. False otherwise </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool lt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);
			XSDecimal val = (XSDecimal) get_single_type(carg, typeof(XSDecimal));
			return (_value.CompareTo(val.Value) == -1);
		}

		// math
		/// <summary>
		/// Mathematical addition operator between this XSDecimal and the supplied
		/// ResultSequence. Due to no numeric type promotion or conversion, the
		/// ResultSequence must be of type XSDecimal.
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an addition with </param>
		/// <returns> A XSDecimal consisting of the result of the mathematical
		///         addition. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence plus(ResultSequence arg)
		{
			// get arg
			ResultSequence carg = convertResultSequence(arg);
			Item at = get_single_arg(carg);
			if (!(at is XSDecimal))
			{
				DynamicError.throw_type_error();
			}
			XSDecimal dt = (XSDecimal) at;

			// own it
			return ResultSequenceFactory.create_new(new XSDecimal(_value + dt.Value));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.eclipse.wst.xml.xpath2.api.ResultSequence convertResultSequence(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private ResultSequence convertResultSequence(ResultSequence arg)
		{
			ResultSequence carg = arg;
			var it = carg.iterator();
			while (it.MoveNext())
			{
				AnyType type = (AnyType) it.Current;
				if (type.string_type().Equals("xs:untypedAtomic") || type.string_type().Equals("xs:string"))
				{
					throw DynamicError.invalidType();
				}
			}
			carg = constructor(carg);
			return carg;
		}

		/// <summary>
		/// Mathematical subtraction operator between this XSDecimal and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a subtraction with </param>
		/// <returns> A XSDecimal consisting of the result of the mathematical
		///         subtraction. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence minus(ResultSequence arg)
		{

			ResultSequence carg = convertResultSequence(arg);

			Item at = get_single_arg(carg);
			if (!(at is XSDecimal))
			{
				DynamicError.throw_type_error();
			}
			XSDecimal dt = (XSDecimal) at;

			return ResultSequenceFactory.create_new(new XSDecimal(_value - dt.Value));
		}

		/// <summary>
		/// Mathematical multiplication operator between this XSDecimal and the
		/// supplied ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a multiplication with </param>
		/// <returns> A XSDecimal consisting of the result of the mathematical
		///         multiplication. </returns>
		public override ResultSequence times(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDecimal val = (XSDecimal) get_single_type(carg, typeof(XSDecimal));
			decimal result = _value * val.Value;
			return ResultSequenceFactory.create_new(new XSDecimal(result));
		}

		/// <summary>
		/// Mathematical division operator between this XSDecimal and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a division with </param>
		/// <returns> A XSDecimal consisting of the result of the mathematical
		///         division. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence div(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence div(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDecimal val = (XSDecimal) get_single_type(carg, typeof(XSDecimal));
			if (val.zero())
			{
				throw DynamicError.div_zero(null);
			}
			//decimal result = Value.divide(val.Value, 18, decimal.ROUND_HALF_EVEN);
			//return ResultSequenceFactory.create_new(new XSDecimal(result));
			throw new Exception();
		}

		/// <summary>
		/// Mathematical integer division operator between this XSDecimal and the
		/// supplied ResultSequence. Due to no numeric type promotion or conversion,
		/// the ResultSequence must be of type XSDecimal.
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an integer division with </param>
		/// <returns> A XSInteger consisting of the result of the mathematical integer
		///         division. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence idiv(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence idiv(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDecimal val = (XSDecimal) get_single_type(carg, typeof(XSDecimal));

			if (val.zero())
			{
				throw DynamicError.div_zero(null);
			}
			System.Numerics.BigInteger _ivalue = new BigInteger(_value);
			System.Numerics.BigInteger ival = new BigInteger(val.Value);
			System.Numerics.BigInteger result = _ivalue / ival;
			return ResultSequenceFactory.create_new(new XSInteger(result));
		}

		/// <summary>
		/// Mathematical modulus operator between this XSDecimal and the supplied
		/// ResultSequence. Due to no numeric type promotion or conversion, the
		/// ResultSequence must be of type XSDecimal.
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a modulus with </param>
		/// <returns> A XSDecimal consisting of the result of the mathematical modulus. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence mod(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence mod(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDecimal val = (XSDecimal) get_single_type(carg, typeof(XSDecimal));

			// BigDecimal result = _value.remainder(val.getValue());
			decimal result = remainder(_value, val.Value);

			return ResultSequenceFactory.create_new(new XSDecimal(result));
		}

		public static decimal remainder(decimal value, decimal divisor)
		{
			// return value.remainder(divisor);

			// appx as of now. JDK 1.4 doesn't support BigDecimal.remainder(..)
			//decimal dividend = value.divide(divisor, decimal.ROUND_DOWN);
			//decimal ceilDividend = new decimal(dividend.toBigInteger());
            throw new Exception();

			//return value - ceilDividend * divisor;
		}

		/// <summary>
		/// Negation of the number stored
		/// </summary>
		/// <returns> A XSDecimal representing the negation of this XSDecimal </returns>
		public override ResultSequence unary_minus()
		{
			decimal result = -_value;
			return ResultSequenceFactory.create_new(new XSDecimal(result));
		}

		// functions
		/// <summary>
		/// Absolutes the number stored
		/// </summary>
		/// <returns> A XSDecimal representing the absolute value of the number stored </returns>
		public override NumericType abs()
		{
            throw new Exception();
			//return new XSDecimal(_value.abs());
		}

		/// <summary>
		/// Returns the smallest integer greater than the number stored
		/// </summary>
		/// <returns> A XSDecimal representing the smallest integer greater than the
		///         number stored </returns>
		public override NumericType ceiling()
		{
            throw new Exception();
			//decimal ceiling = _value.setScale(0, decimal.ROUND_CEILING);
			//return new XSDecimal(ceiling);
		}

		/// <summary>
		/// Returns the largest integer smaller than the number stored
		/// </summary>
		/// <returns> A XSDecimal representing the largest integer smaller than the
		///         number stored </returns>
		public override NumericType floor()
		{
			throw new Exception();
			//decimal floor = _value.setScale(0, decimal.ROUND_FLOOR);
			//return new XSDecimal(floor);
		}

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A XSDecimal representing the closest long of the number stored. </returns>
		public override NumericType round()
		{
			throw new Exception();
			//decimal round = _value.setScale(0, decimal.ROUND_UP);
			//return new XSDecimal(round);
		}

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A XSDecimal representing the closest long of the number stored. </returns>
		public override NumericType round_half_to_even()
		{
			return round_half_to_even(0);
		}

		/// <summary>
		/// Returns the closest integer of the number stored with the specified precision.
		/// </summary>
		/// <param name="precision"> An integer precision </param>
		/// <returns> A XSDecimal representing the closest long of the number stored. </returns>
		public override NumericType round_half_to_even(int precision)
		{
			throw new Exception();
			//decimal round = _value.setScale(precision, decimal.ROUND_HALF_EVEN);
			//return new XSDecimal(round);
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_DECIMAL;
			}
		}

		public override object NativeValue
		{
			get
			{
				return _value;
			}
		}

	}

}