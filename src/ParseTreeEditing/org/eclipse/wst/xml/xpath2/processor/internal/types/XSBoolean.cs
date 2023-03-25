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
///     Mukul Gandhi - bug274784 - improvements to xs:boolean data type implementation
///     David Carver - bug 282223 - corrected casting to boolean.
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
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of a true or a false value.
	/// </summary>
	public class XSBoolean : CtrType, CmpEq, CmpGt, CmpLt
	{
		private const string XS_BOOLEAN = "xs:boolean";
		public static readonly XSBoolean TRUE = new XSBoolean(true);
		public static readonly XSBoolean FALSE = new XSBoolean(false);
		private bool _value;

		/// <summary>
		/// Initiates the new representation to the boolean supplied
		/// </summary>
		/// <param name="x">
		///       Initializes this datatype to represent this boolean </param>
		public XSBoolean(bool x)
		{
			_value = x;
		}

		/// <summary>
		/// Initiates to a default representation of false.
		/// </summary>
		public XSBoolean() : this(false)
		{
		}

		/// <summary>
		/// Retrieve the full type pathname of this datatype
		/// </summary>
		/// <returns> "xs:boolean", the full datatype pathname </returns>
		public override string string_type()
		{
			return XS_BOOLEAN;
		}

		public override object NativeValue
		{
			get
			{
				return Convert.ToBoolean(_value);
			}
		}

		/// <summary>
		/// Retrieve the datatype name
		/// </summary>
		/// <returns> "boolean", which is the datatype name. </returns>
		public override string type_name()
		{
			return "boolean";
		}

		/// <summary>
		/// Retrieve the String representation of the boolean value stored
		/// </summary>
		/// <returns> the String representation of the boolean value stored </returns>
		public override string StringValue
		{
			get
			{
				return "" + _value;
			}
		}

		/// <summary>
		/// Retrieves the actual boolean value stored
		/// </summary>
		/// <returns> the actual boolean value stored </returns>
		public virtual bool value()
		{
			return _value;
		}

		/// <summary>
		/// Creates a new result sequence consisting of the retrievable boolean value
		/// in the supplied result sequence
		/// </summary>
		/// <param name="arg">
		///            The result sequence from which to extract the boolean value. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> A new result sequence consisting of the boolean value supplied. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
			  return ResultBuffer.EMPTY;
			}

			Item anyType = arg.first();

			if (anyType is XSDuration || anyType is CalendarType || anyType is XSBase64Binary || anyType is XSHexBinary || anyType is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			string str_value = anyType.StringValue;


			if (!(isCastable(anyType, str_value)))
			{
			   throw DynamicError.cant_cast(null);
			}

			return XSBoolean.valueOf(!isFalse(str_value));
		}

		private bool isFalse(string str_value)
		{
			return str_value.Equals("0") || str_value.Equals("false") || str_value.Equals("+0") || str_value.Equals("-0") || str_value.Equals("0.0E0") || str_value.Equals("NaN");
		}

		private bool isCastable(Item anyType, string str_value)
		{
			return str_value.Equals("0") || str_value.Equals("1") || str_value.Equals("true") || str_value.Equals("false") || anyType is NumericType;
		}

		// comparisons
		/// <summary>
		/// Comparison for equality between the supplied and this boolean
		/// representation. Returns true if both represent same boolean value, false
		/// otherwise
		/// </summary>
		/// <param name="arg">
		///            The XSBoolean representation of the boolean value to compare
		///            with. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> New XSBoolean representation of true/false result of the equality
		///         comparison </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			XSBoolean barg = (XSBoolean) NumericType.get_single_type((Item)arg, typeof(XSBoolean));

			return value() == barg.value();
		}

		/// <summary>
		/// Comparison between the supplied and this boolean representation. Returns
		/// true if this XSBoolean represents true and that XSBoolean supplied
		/// represents false. Returns false otherwise
		/// </summary>
		/// <param name="arg">
		///            The XSBoolean representation of the boolean value to compare
		///            with. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> New XSBoolean representation of true/false result of the
		///         comparison </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			XSBoolean barg = (XSBoolean) NumericType.get_single_type((Item)arg, typeof(XSBoolean));

			bool result = false;

			if (value() && !barg.value())
			{
				result = true;
			}
			return result;
		}

		/// <summary>
		/// Comparison between the supplied and this boolean representation. Returns
		/// true if this XSBoolean represents false and that XSBoolean supplied
		/// represents true. Returns false otherwise
		/// </summary>
		/// <param name="arg">
		///            The XSBoolean representation of the boolean value to compare
		///            with. </param>
		/// <exception cref="DynamicError"> </exception>
		/// <returns> New XSBoolean representation of true/false result of the
		///         comparison </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			XSBoolean barg = (XSBoolean) NumericType.get_single_type((Item)arg, typeof(XSBoolean));

			bool result = false;

			if (!value() && barg.value())
			{
				result = true;
			}
			return result;
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_BOOLEAN;
			}
		}

		public static ResultSequence valueOf(bool answer)
		{
			return answer ? TRUE : FALSE;
		}

	}

}