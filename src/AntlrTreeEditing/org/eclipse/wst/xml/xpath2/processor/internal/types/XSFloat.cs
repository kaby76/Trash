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
///     Mukul Gandhi - bug 279406 - improvements to negative zero values for xs:float
///     David Carver - bug 262765 - fixed rounding errors.
///     David Carver - bug 282223 - fixed casting errors.
///     Jesper Steen Moller - Bug 286062 - Fix idiv error cases and increase precision  
///     Jesper Steen Moller - bug 281028 - Added constructor from string
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
	/// A representation of the Float datatype
	/// </summary>
	public class XSFloat : NumericType
	{

		private const string XS_FLOAT = "xs:float";
		private float? _value;
		private XPathDecimalFormat format = new XPathDecimalFormat("0.#######E0");
		/// <summary>
		/// Initiates a representation of the supplied number
		/// </summary>
		/// <param name="x">
		///            The number to be stored </param>
		public XSFloat(float x)
		{
			_value = new float?(x);
		}

		/// <summary>
		/// Initiates a representation of 0
		/// </summary>
		public XSFloat() : this(0)
		{
		}

		/// <summary>
		/// Initialises using a String represented number
		/// </summary>
		/// <param name="init">
		///            String representation of the number to be stored </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public XSFloat(String init) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public XSFloat(string init)
		{
			try
			{
				if (init.Equals("-INF"))
				{
					_value = new float?(float.NegativeInfinity);
				}
				else if (init.Equals("INF"))
				{
					_value = new float?(float.PositiveInfinity);
				}
				else
				{
					_value = Convert.ToSingle(init);
				}
			}
			catch (System.FormatException)
			{
				throw DynamicError.cant_cast(null);
			}
		}
		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:float" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_FLOAT;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "float" which is the datatype's name </returns>
		public override string type_name()
		{
			return "float";
		}

		/// <summary>
		/// Retrieves a String representation of the stored number
		/// </summary>
		/// <returns> String representation of the stored number </returns>
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
		/// Check for whether this datatype represents NaN
		/// </summary>
		/// <returns> True is this datatype represents NaN. False otherwise </returns>
		public virtual bool nan()
		{
			return float.IsNaN(_value.Value);
		}

		/// <summary>
		/// Check for whether this datatype represents negative or positive infinity
		/// </summary>
		/// <returns> True is this datatype represents infinity. False otherwise </returns>
		public virtual bool infinite()
		{
			return float.IsInfinity(_value.Value);
		}

		/// <summary>
		/// Check for whether this datatype represents 0
		/// </summary>
		/// <returns> True if this datatype represents 0. False otherwise </returns>
		public override bool zero()
		{
		   return (_value.Value.CompareTo(0) == 0);
		}

		/*
		 * Check for whether this XSFloat represents -0
		 * 
		 * @return True if this XSFloat represents -0. False otherwise.
		 * @since 1.1
		 */
		public virtual bool negativeZero()
		{
		   return (_value.Value.CompareTo(-0.0f) == 0);
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the retrievable float in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to extract the float </param>
		/// <returns> New ResultSequence consisting of the float supplied </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			AnyType aat = (AnyType) arg.first();

			if (aat is XSDuration || aat is CalendarType || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (!(aat.string_type().Equals("xs:string") || aat is NodeType || aat.string_type().Equals("xs:untypedAtomic") || aat.string_type().Equals("xs:boolean") || aat is NumericType))
			{
				throw DynamicError.cant_cast(null);
			}


			try
			{
				float f;
				if (aat.StringValue.Equals("INF"))
				{
					f = float.PositiveInfinity;
				}
				else if (aat.StringValue.Equals("-INF"))
				{
					f = float.NegativeInfinity;
				}
				else if (aat is XSBoolean)
				{
					if (aat.StringValue.Equals("true"))
					{
						f = 1.0f;
					}
					else
					{
						f = 0.0f;
					}
				}
				else
				{
					f = Convert.ToSingle(aat.StringValue);
				}
				return new XSFloat(f);
			}
			catch (System.FormatException)
			{
				throw DynamicError.cant_cast(null);
			}

		}

		/// <summary>
		/// Retrieves the actual float value stored
		/// </summary>
		/// <returns> The actual float value stored </returns>
		public virtual float float_value()
		{
			return _value.Value;
		}

		/// <summary>
		/// Equality comparison between this number and the supplied representation. </summary>
		/// <param name="aa">
		///            The datatype to compare with
		/// </param>
		/// <returns> True if the two representations are of the same number. False
		///         otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType aa, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool eq(AnyType aa, DynamicContext dynamicContext)
		{
			Item carg = convertArg(aa);
			if (!(carg is XSFloat))
			{
				DynamicError.throw_type_error();
			}

			XSFloat f = (XSFloat) carg;
			if (nan() && f.nan())
			{
				return false;
			}

			float? thatvalue = new float?(f.float_value());
			float? thisvalue = new float?(float_value());

			return thisvalue.Equals(thatvalue);
		}

		/// <summary>
		/// Comparison between this number and the supplied representation. 
		/// </summary>
		/// <param name="arg">
		///            The datatype to compare with </param>
		/// <returns> True if the supplied representation is a smaller number than the
		///         one stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool gt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);
			XSFloat val = (XSFloat) get_single_type(carg, typeof(XSFloat));
			return float_value() > val.float_value();
		}

		/// <summary>
		/// Comparison between this number and the supplied representation. 
		/// </summary>
		/// <param name="arg">
		///            The datatype to compare with </param>
		/// <returns> True if the supplied representation is a greater number than the
		///         one stored. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool lt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);
			XSFloat val = (XSFloat) get_single_type(carg, typeof(XSFloat));
			return float_value() < val.float_value();
		}

		/// <summary>
		/// Mathematical addition operator between this XSFloat and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an addition with </param>
		/// <returns> A XSFloat consisting of the result of the mathematical addition. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence plus(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);
			Item at = get_single_arg(carg);
			if (!(at is XSFloat))
			{
				DynamicError.throw_type_error();
			}
			XSFloat val = (XSFloat) at;

			return ResultSequenceFactory.create_new(new XSFloat(float_value() + val.float_value()));
		}

		/// <summary>
		/// Mathematical subtraction operator between this XSFloat and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a subtraction with </param>
		/// <returns> A XSFloat consisting of the result of the mathematical
		///         subtraction. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence minus(ResultSequence arg)
		{
			ResultSequence carg = constructor(arg);
			Item at = get_single_arg(carg);
			if (!(at is XSFloat))
			{
				DynamicError.throw_type_error();
			}
			XSFloat val = (XSFloat) at;

			return ResultSequenceFactory.create_new(new XSFloat(float_value() - val.float_value()));
		}

		/// <summary>
		/// Mathematical multiplication operator between this XSFloat and the
		/// supplied ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a multiplication with </param>
		/// <returns> A XSFloat consisting of the result of the mathematical
		///         multiplication. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence times(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence times(ResultSequence arg)
		{
			ResultSequence carg = constructor(arg);
			XSFloat val = (XSFloat) get_single_type(carg, typeof(XSFloat));
			return ResultSequenceFactory.create_new(new XSFloat(float_value() * val.float_value()));
		}

		/// <summary>
		/// Mathematical division operator between this XSFloat and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a division with </param>
		/// <returns> A XSFloat consisting of the result of the mathematical division. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence div(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence div(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);
			XSFloat val = (XSFloat) get_single_type(carg, typeof(XSFloat));
			return ResultSequenceFactory.create_new(new XSFloat(float_value() / val.float_value()));
		}

		/// <summary>
		/// Mathematical integer division operator between this XSFloat and the
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
			XSFloat val = (XSFloat) get_single_type(carg, typeof(XSFloat));

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

			decimal result = (decimal)(
                (double)
                    (float_value() / val.float_value())
                    );
			return ResultSequenceFactory.create_new(new XSInteger(new BigInteger(result)));
		}

		/// <summary>
		/// Mathematical modulus operator between this XSFloat and the supplied
		/// ResultSequence. Due to no numeric type promotion or conversion, the
		/// ResultSequence must be of type XSFloat.
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a modulus with </param>
		/// <returns> A XSFloat consisting of the result of the mathematical modulus. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence mod(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence mod(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);
			XSFloat val = (XSFloat) get_single_type(carg, typeof(XSFloat));
			return ResultSequenceFactory.create_new(new XSFloat(float_value() % val.float_value()));
		}

		/// <summary>
		/// Negates the number stored
		/// </summary>
		/// <returns> A XSFloat representing the negation of the number stored </returns>
		public override ResultSequence unary_minus()
		{
			return ResultSequenceFactory.create_new(new XSFloat(-1 * float_value()));
		}

		/// <summary>
		/// Absolutes the number stored
		/// </summary>
		/// <returns> A XSFloat representing the absolute value of the number stored </returns>
		public override NumericType abs()
		{
			return new XSFloat(Math.Abs(float_value()));
		}

		/// <summary>
		/// Returns the smallest integer greater than the number stored
		/// </summary>
		/// <returns> A XSFloat representing the smallest integer greater than the
		///         number stored </returns>
		public override NumericType ceiling()
		{
			return new XSFloat((float) Math.Ceiling(float_value()));
		}

		/// <summary>
		/// Returns the largest integer smaller than the number stored
		/// </summary>
		/// <returns> A XSFloat representing the largest integer smaller than the
		///         number stored </returns>
		public override NumericType floor()
		{
			return new XSFloat((float) Math.Floor(float_value()));
		}

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A XSFloat representing the closest long of the number stored. </returns>
		public override NumericType round()
		{
			decimal value = new decimal(float_value());
//			decimal round = value.setScale(0, decimal.ROUND_HALF_UP);
	//		return new XSFloat(round.floatValue());
    return new XSFloat((long)value);
        }

		/// <summary>
		/// Returns the closest integer of the number stored.
		/// </summary>
		/// <returns> A XSFloat representing the closest long of the number stored. </returns>
		public override NumericType round_half_to_even()
		{
			return round_half_to_even(0);
		}

		/// <summary>
		/// Returns the closest integer of the number stored with the specified precision.
		/// </summary>
		/// <param name="precision"> An integer precision </param>
		/// <returns> A XSFloat representing the closest long of the number stored. </returns>
		public override NumericType round_half_to_even(int precision)
		{
			decimal value = new decimal(_value.Value);
			//decimal round = value.setScale(precision, decimal.ROUND_HALF_EVEN);
			//return new XSFloat(round.floatValue());
			return new XSFloat((long)value);
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

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_FLOAT;
			}
		}

		public override object NativeValue
		{
			get
			{
				return float_value();
			}
		}
	}

}