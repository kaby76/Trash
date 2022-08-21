using System;
using System.Collections;
using System.Numerics;

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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type
///     David Carver - bug 277770 - format of XSDouble for zero values incorrect.
///     Mukul Gandhi - bug 279406 - improvements to negative zero values for xs:double
///     David Carver (STAR) - bug 262765 - various numeric formatting fixes and calculations
///     David Carver (STAR) - bug 262765 - fixed rounding errors.      
///     Jesper Steen Moller - Bug 286062 - Fix idiv error cases and increase precision  
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
	/// A representation of the Double datatype
	/// </summary>
	public class XSDouble : NumericType
	{

		private const string XS_DOUBLE = "xs:double";
		private double? _value;
		private XPathDecimalFormat format = new XPathDecimalFormat("0.################E0");

		/// <summary>
		/// Initialises a representation of the supplied number
		/// </summary>
		/// <param name="x">
		///            Number to be stored </param>
		public XSDouble(double x)
		{
			_value = new double?(x);
		}

		/// <summary>
		/// Initializes a representation of 0
		/// </summary>
		public XSDouble() : this(0)
		{
		}

		/// <summary>
		/// Initialises using a String represented number
		/// </summary>
		/// <param name="init">
		///            String representation of the number to be stored </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public XSDouble(String init) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public XSDouble(string init)
		{
			try
			{
				if (init.Equals("-INF"))
				{
					_value = new double?(double.NegativeInfinity);
				}
				else if (init.Equals("INF"))
				{
					_value = new double?(double.PositiveInfinity);
				}
				else
				{
					_value = Convert.ToDouble(init);
				}
			}
			catch (System.FormatException)
			{
				throw DynamicError.cant_cast(null);
			}
		}

		/// <summary>
		/// Creates a new representation of the String represented number
		/// </summary>
		/// <param name="i">
		///            String representation of the number to be stored </param>
		/// <returns> New XSDouble representing the number supplied </returns>
		public static XSDouble parse_double(string i)
		{
			try
			{
				double? d = null;
				if (i.Equals("INF"))
				{
					d = new double?(double.PositiveInfinity);
				}
				else if (i.Equals("-INF"))
				{
					d = new double?(double.NegativeInfinity);
				}
				else
				{
					d = Convert.ToDouble(i);
				}
				return new XSDouble(d.Value);
			}
			catch (System.FormatException)
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a new result sequence consisting of the retrievable double number
		/// in the supplied result sequence
		/// </summary>
		/// <param name="arg">
		///            The result sequence from which to extract the double number. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> A new result sequence consisting of the double number supplied. </returns>
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

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}

			XSDouble d = castDouble(aat);

			if (d == null)
			{
				throw DynamicError.cant_cast(null);
			}

			return d;
		}

		private bool isCastable(Item aat)
		{
			if (aat is XSString || aat is XSUntypedAtomic || aat is NodeType)
			{
				return true;
			}
			if (aat is XSBoolean || aat is NumericType)
			{
				return true;
			}
			return false;
		}

		private XSDouble castDouble(Item aat)
		{
			if (aat is XSBoolean)
			{
				if (aat.StringValue.Equals("true"))
				{
					return new XSDouble(1.0E0);
				}
				else
				{
					return new XSDouble(0.0E0);
				}
			}
			return parse_double(aat.StringValue);

		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:double" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_DOUBLE;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "double" which is the datatype's name </returns>
		public override string type_name()
		{
			return "double";
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
    
				if (negativeZero())
				{
					return "-0";
				}
    
				if (nan())
				{
					return "NaN";
				}
    
				return format.xpathFormat(_value);
			}
		}

		/// <summary>
		/// Check for whether this XSDouble represents NaN
		/// </summary>
		/// <returns> True if this XSDouble represents NaN. False otherwise. </returns>
		public virtual bool nan()
		{
			return double.IsNaN(_value.Value);
		}

		/// <summary>
		/// Check for whether this XSDouble represents an infinite number (negative or positive)
		/// </summary>
		/// <returns> True if this XSDouble represents infinity. False otherwise. </returns>
		public virtual bool infinite()
		{
			return double.IsInfinity(_value.Value);
		}

		/// <summary>
		/// Check for whether this XSDouble represents 0
		/// </summary>
		/// <returns> True if this XSDouble represents 0. False otherwise. </returns>
		public override bool zero()
		{
			return (_value.Value.CompareTo(0.0E0) == 0);
		}

		/*
		 * Check for whether this XSDouble represents -0
		 * 
		 * @return True if this XSDouble represents -0. False otherwise.
		 * 
		 * @since 1.1
		 */
		public virtual bool negativeZero()
		{
			return (_value.Value.CompareTo(-0.0E0) == 0);
		}

		/// <summary>
		/// Retrieves the actual value of the number stored
		/// </summary>
		/// <returns> The actual value of the number stored </returns>
		public virtual double double_value()
		{
			return _value.Value;
		}

		/// <summary>
		/// Equality comparison between this number and the supplied representation. </summary>
		/// <param name="aa">
		///            Representation to be compared with (must currently be of type
		///            XSDouble)
		/// </param>
		/// <returns> True if the 2 representations represent the same number. False
		///         otherwise
		/// @since 1.1 </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType aa, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool eq(AnyType aa, DynamicContext dynamicContext)
		{
			ResultSequence rs = ResultSequenceFactory.create_new(aa);
			ResultSequence crs = constructor(rs);

			if (crs.empty())
			{
				throw DynamicError.throw_type_error();
			}
			Item cat = crs.first();

			XSDouble d = (XSDouble) cat;
			if (d.nan() && nan())
			{
				return false;
			}

			double? thatvalue = new double?(d.double_value());
			double? thisvalue = new double?(double_value());

			return thisvalue.Equals(thatvalue);
		}

		/// <summary>
		/// Comparison between this number and the supplied representation. 
		/// </summary>
		/// <param name="arg">
		///            Representation to be compared with (must currently be of type
		///            XSDouble) </param>
		/// <returns> True if the supplied type represents a number smaller than this
		///         one stored. False otherwise </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool gt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));
			return double_value() > val.double_value();
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
		/// Comparison between this number and the supplied representation. Currently
		/// no numeric type promotion exists so the supplied representation must be
		/// of type XSDouble.
		/// </summary>
		/// <param name="arg">
		///            Representation to be compared with (must currently be of type
		///            XSDouble) </param>
		/// <returns> True if the supplied type represents a number greater than this
		///         one stored. False otherwise </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool lt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));
			return double_value() < val.double_value();
		}

		// math
		/// <summary>
		/// Mathematical addition operator between this XSDouble and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an addition with </param>
		/// <returns> A XSDouble consisting of the result of the mathematical addition. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence plus(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);
			Item at = get_single_arg(carg);

			if (!(at is XSDouble))
			{
				DynamicError.throw_type_error();
			}
			XSDouble val = (XSDouble) at;

			return ResultSequenceFactory.create_new(new XSDouble(double_value() + val.double_value()));
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
					throw DynamicError.throw_type_error();
				}
			}

			carg = constructor(carg);
			return carg;
		}

		/// <summary>
		/// Mathematical subtraction operator between this XSDouble and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an subtraction with </param>
		/// <returns> A XSDouble consisting of the result of the mathematical
		///         subtraction. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence minus(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));

			return ResultSequenceFactory.create_new(new XSDouble(double_value() - val.double_value()));
		}

		/// <summary>
		/// Mathematical multiplication operator between this XSDouble and the
		/// supplied ResultSequence. Due to no numeric type promotion or conversion,
		/// the ResultSequence must be of type XSDouble.
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an multiplication with </param>
		/// <returns> A XSDouble consisting of the result of the mathematical
		///         multiplication. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence times(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence times(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));
			return ResultSequenceFactory.create_new(new XSDouble(double_value() * val.double_value()));
		}

		/// <summary>
		/// Mathematical division operator between this XSDouble and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an division with </param>
		/// <returns> A XSDouble consisting of the result of the mathematical division. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence div(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence div(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));
			return ResultSequenceFactory.create_new(new XSDouble(double_value() / val.double_value()));
		}

		/// <summary>
		/// Mathematical integer division operator between this XSDouble and the
		/// supplied ResultSequence. 
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

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));

			if (this.nan() || val.nan())
			{
				throw DynamicError.numeric_overflow("Dividend or divisor is NaN");
			}

			if (this.infinite())
			{
				throw DynamicError.numeric_overflow("Dividend is infinite");
			}

			if (val.zero())
			{
				throw DynamicError.div_zero(null);
			}

			decimal result = new decimal((double_value() / val.double_value()));
			return ResultSequenceFactory.create_new(new XSInteger(new BigInteger(result)));
		}

		/// <summary>
		/// Mathematical modulus operator between this XSDouble and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a modulus with </param>
		/// <returns> A XSDouble consisting of the result of the mathematical modulus. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence mod(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence mod(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSDouble val = (XSDouble) get_single_type(carg, typeof(XSDouble));
			return ResultSequenceFactory.create_new(new XSDouble(double_value() % val.double_value()));
		}

		/// <summary>
		/// Negation of the number stored
		/// </summary>
		/// <returns> A XSDouble representing the negation of this XSDecimal </returns>
		public override ResultSequence unary_minus()
		{
			return ResultSequenceFactory.create_new(new XSDouble(-1 * double_value()));
		}

		// functions
		/// <summary>
		/// Absolutes the number stored
		/// </summary>
		/// <returns> A XSDouble representing the absolute value of the number stored </returns>
		public override NumericType abs()
		{
			return new XSDouble(Math.Abs(double_value()));
		}

		/// <summary>
		/// Returns the smallest integer greater than the number stored
		/// </summary>
		/// <returns> A XSDouble representing the smallest integer greater than the
		///         number stored </returns>
		public override NumericType ceiling()
		{
			return new XSDouble(Math.Ceiling(double_value()));
		}

		/// <summary>
		/// Returns the largest integer smaller than the number stored
		/// </summary>
		/// <returns> A XSDouble representing the largest integer smaller than the
		///         number stored </returns>
		public override NumericType floor()
		{
			return new XSDouble(Math.Floor(double_value()));
		}

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A XSDouble representing the closest long of the number stored. </returns>
		public override NumericType round()
		{
			decimal value = new decimal(_value.Value);
			//decimal round = value.setScale(0, decimal.ROUND_HALF_UP);
			//return new XSDouble(round.doubleValue());
			return new XSDouble((long)value);
		}

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A XSDouble representing the closest long of the number stored. </returns>
		public override NumericType round_half_to_even()
		{

			return round_half_to_even(0);
		}

		/// <summary>
		/// Returns the closest integer of the number stored with the specified
		/// precision.
		/// </summary>
		/// <param name="precision">
		///            An integer precision </param>
		/// <returns> A XSDouble representing the closest long of the number stored. </returns>
		public override NumericType round_half_to_even(int precision)
		{
			decimal value = new decimal(_value.Value);
			//decimal round = value.setScale(precision, decimal.ROUND_HALF_EVEN);
			//return new XSDouble(round.doubleValue());
			return new XSDouble((long)value);
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_DOUBLE;
			}
		}

		public override object NativeValue
		{
			get
			{
				return double_value();
			}
		}
	}

}