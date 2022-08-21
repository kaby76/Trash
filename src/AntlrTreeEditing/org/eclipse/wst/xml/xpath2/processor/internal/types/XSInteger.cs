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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type. Using Java
///                      BigInteger class to enhance numerical range to -INF -> +INF.
///     David Carver (STAR) - bug 262765 - fix comparision to zero.
///     David Carver (STAR) - bug 282223 - fix casting issues.
///     Jesper Steen Moller - bug 262765 - fix type tests
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
	/// A representation of the Integer datatype
	/// </summary>
	public class XSInteger : XSDecimal
	{

		private const string XS_INTEGER = "xs:integer";
		private System.Numerics.BigInteger _value;

		/// <summary>
		/// Initializes a representation of 0
		/// </summary>
		public XSInteger() : this(System.Numerics.BigInteger.Zero)
		{
		}

		/// <summary>
		/// Initializes a representation of the supplied integer
		/// </summary>
		/// <param name="x">
		///            Integer to be stored </param>
		public XSInteger(System.Numerics.BigInteger x) : base(new decimal((long)x))
		{
			_value = x;
		}

		/// <summary>
		/// Initializes a representation of the supplied integer
		/// </summary>
		/// <param name="x">
		///            Integer to be stored </param>
		public XSInteger(string x) : base(x)
        {
            BigInteger.TryParse(x, out BigInteger v);
			_value = v;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:integer" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_INTEGER;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "integer" which is the datatype's name </returns>
		public override string type_name()
		{
			return "integer";
		}

		/// <summary>
		/// Retrieves a String representation of the integer stored
		/// </summary>
		/// <returns> String representation of the integer stored </returns>
		public override string StringValue
		{
			get
			{
				return _value.ToString();
			}
		}

		public override object NativeValue
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Check whether the integer represented is 0
		/// </summary>
		/// <returns> True is the integer represented is 0. False otherwise </returns>
		public override bool zero()
		{
			return (_value.CompareTo(System.Numerics.BigInteger.Zero) == 0);
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable integer in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the integer is to be extracted </param>
		/// <returns> New ResultSequence consisting of the integer supplied </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			// the function conversion rules apply here too. Get the argument
			// and convert it's string value to an integer.
			Item aat = arg.first();

			if (aat is XSDuration || aat is CalendarType || aat is XSBase64Binary || aat is XSHexBinary || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			if (!isCastable(aat))
			{
				throw DynamicError.cant_cast(null);
			}


			try
			{
				System.Numerics.BigInteger bigInt = castInteger(aat);
				return new XSInteger(bigInt);
			}
			catch (System.FormatException)
			{
				throw DynamicError.invalidLexicalValue();
			}

		}

		private System.Numerics.BigInteger castInteger(Item aat)
		{
			if (aat is XSBoolean)
			{
				if (aat.StringValue.Equals("true"))
				{
					return System.Numerics.BigInteger.One;
				}
				else
				{
					return System.Numerics.BigInteger.Zero;
				}
			}

			if (aat is XSDecimal || aat is XSFloat || aat is XSDouble)
			{
				var s1 = aat.StringValue;
                BigInteger.TryParse(s1, out BigInteger v1);
                return v1;
			}

            var s = aat.ToString();
            BigInteger.TryParse(s, out BigInteger v);
			return v;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean isCastable(org.eclipse.wst.xml.xpath2.api.Item aat) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private bool isCastable(Item aat)
		{
			if (aat is XSString || aat is XSUntypedAtomic || aat is NodeType)
			{
				if (isLexicalValue(aat.StringValue))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			if (aat is XSBoolean || aat is NumericType)
			{
				return true;
			}
			return false;
		}

		protected internal override bool isLexicalValue(string value)
		{

			try
            {
                if (BigInteger.TryParse(value, out BigInteger v)) return true;
                else return false;
            }
			catch (System.FormatException)
			{
				return false;
			}
		}

		/// <summary>
		/// Retrieves the actual integer value stored
		/// </summary>
		/// <returns> The actual integer value stored </returns>
		public virtual System.Numerics.BigInteger int_value()
		{
			return _value;
		}

		/// <summary>
		/// Sets the integer stored to that supplied
		/// </summary>
		/// <param name="x">
		///            Integer to be stored </param>
		public virtual void set_int(System.Numerics.BigInteger x)
		{
			_value = x;
			set_double((long)x);
		}

		/// <summary>
		/// Mathematical addition operator between this XSInteger and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform an addition with </param>
		/// <returns> A XSInteger consisting of the result of the mathematical
		///         addition. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence plus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence plus(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);
			Item at = get_single_arg(carg);
			if (!(at is XSInteger))
			{
				DynamicError.throw_type_error();
			}

			XSInteger val = (XSInteger)at;

			return ResultSequenceFactory.create_new(new XSInteger(int_value() + val.int_value()));

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
		/// Mathematical subtraction operator between this XSInteger and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a subtraction with </param>
		/// <returns> A XSInteger consisting of the result of the mathematical
		///         subtraction. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence minus(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence minus(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);
			XSInteger val = (XSInteger) get_single_type(carg, typeof(XSInteger));

			return ResultSequenceFactory.create_new(new XSInteger(int_value() - val.int_value()));
		}

		/// <summary>
		/// Mathematical multiplication operator between this XSInteger and the
		/// supplied ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a multiplication with </param>
		/// <returns> A XSInteger consisting of the result of the mathematical
		///         multiplication. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence times(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence times(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSInteger val = (XSInteger) get_single_type(carg, typeof(XSInteger));

			return ResultSequenceFactory.create_new(new XSInteger(int_value() * val.int_value()));
		}

		/// <summary>
		/// Mathematical modulus operator between this XSInteger and the supplied
		/// ResultSequence. 
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence to perform a modulus with </param>
		/// <returns> A XSInteger consisting of the result of the mathematical modulus. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence mod(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence mod(ResultSequence arg)
		{
			ResultSequence carg = convertResultSequence(arg);

			XSInteger val = (XSInteger) get_single_type(carg, typeof(XSInteger));
			System.Numerics.BigInteger result = ((long)int_value()) % (val.int_value());

			return ResultSequenceFactory.create_new(new XSInteger(result));
		}

		/// <summary>
		/// Negates the integer stored
		/// </summary>
		/// <returns> New XSInteger representing the negation of the integer stored </returns>
		public override ResultSequence unary_minus()
		{
			return ResultSequenceFactory.create_new(new XSInteger(int_value() * new System.Numerics.BigInteger(-1)));
		}

		/// <summary>
		/// Absolutes the integer stored
		/// </summary>
		/// <returns> New XSInteger representing the absolute of the integer stored </returns>
		public override NumericType abs()
        {
            var i = int_value();
            var f = Math.Abs((long)i);
			return new XSInteger(f);
		}

		/*
		 * (non-Javadoc)
		 * @see org.eclipse.wst.xml.xpath2.processor.internal.types.XSDecimal#gt(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool gt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);
			XSInteger val = (XSInteger) get_single_type(carg, typeof(XSInteger));

			int compareResult = int_value().CompareTo(val.int_value());

			return compareResult > 0;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.eclipse.wst.xml.xpath2.api.Item convertArg(AnyType arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		protected internal override Item convertArg(AnyType arg)
		{
			ResultSequence rs = ResultSequenceFactory.create_new(arg);
			rs = constructor(rs);
			Item carg = rs.first();
			return carg;
		}

		/*
		 * (non-Javadoc)
		 * @see org.eclipse.wst.xml.xpath2.processor.internal.types.XSDecimal#lt(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override bool lt(AnyType arg, DynamicContext context)
		{
			Item carg = convertArg(arg);
			XSInteger val = (XSInteger) get_single_type(carg, typeof(XSInteger));

			int compareResult = int_value().CompareTo(val.int_value());

			return compareResult < 0;
		}

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

			decimal result = decimal.Divide(Value, val.Value);
			return ResultSequenceFactory.create_new(new XSDecimal(result));
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_INTEGER;
			}
		}
	}

}