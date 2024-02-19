using System;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Mukul Gandhi, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Mukul Gandhi - bug 281054 - initial API and implementation
///     David Carver (STAR) - bug 228223 - fixed casting issue.  Needed to encode the value. 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

//	using Base64 = org.apache.xerces.impl.dv.util.Base64;
//	using HexBin = org.apache.xerces.impl.dv.util.HexBin;
	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the xs:hexBinary datatype
	/// </summary>
	public class XSHexBinary : CtrType, CmpEq
	{

		private const string XS_HEX_BINARY = "xs:hexBinary";
		private string _value;

		/// <summary>
		/// Initialises using the supplied String
		/// </summary>
		/// <param name="x">
		///            The String to initialise to </param>
		public XSHexBinary(string x)
		{
			_value = x;
		}

		/// <summary>
		/// Initialises to null
		/// </summary>
		public XSHexBinary() : this(null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:hexBinary" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_HEX_BINARY;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "hexBinary" which is the datatype's name </returns>
		public override string type_name()
		{
			return "hexBinary";
		}

		/// <summary>
		/// Retrieves a String representation of the base64Binary stored. This method is
		/// functionally identical to value()
		/// </summary>
		/// <returns> The hexBinary stored </returns>
		public override string StringValue
		{
			get
			{
				return _value.ToUpper();
			}
		}

		/// <summary>
		/// Retrieves a String representation of the hexBinary stored. This method is
		/// functionally identical to string_value()
		/// </summary>
		/// <returns> The hexBinary stored </returns>
		public virtual string value()
		{
			return _value;
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the hexBinary value
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to construct hexBinary value </param>
		/// <returns> New ResultSequence representing hexBinary value </returns>
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
			if (aat is NumericType || aat is XSDuration || aat is CalendarType || aat is XSBoolean || aat is XSAnyURI)
			{
				throw DynamicError.invalidType();
			}

			string str_value = aat.StringValue;

			if (!(aat is XSHexBinary || aat is XSString || aat is XSUntypedAtomic || aat is XSBase64Binary))
			{
					throw DynamicError.cant_cast(null);
			}

			if (aat is XSUntypedAtomic || aat is XSString)
			{
				string[] nonHexValues = null;
				try
				{
					nonHexValues = str_value.Split("[0-9a-fA-F]", true);
				}
				catch (Exception)
				{
					throw DynamicError.throw_type_error();
				}

				string[] binValues = null;
				try
				{
					binValues = str_value.Split("[0-1]", true);
				}
				catch (Exception)
				{
					throw DynamicError.throw_type_error();
				}

				if (nonHexValues.Length > 0 || binValues.Length == 0)
				{
					throw DynamicError.invalidForCastConstructor();
				}
			}


			sbyte[] decodedValue = null;

			if (aat is XSBase64Binary)
			{
				throw new Exception();
				//decodedValue = Base64.decode(str_value);
				//decodedValue = HexBin.encode(decodedValue).Bytes;
			}
			else
			{
				decodedValue = str_value.GetBytes();
			}

			if (decodedValue != null)
			{
			  return new XSHexBinary(StringHelperClass.NewString(decodedValue));
			}
			else
			{
			  // invalid hexBinary string
			  throw DynamicError.throw_type_error();
			}
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation which
		/// must be of type hexBinary
		/// </summary>
		/// <param name="arg">
		///            The representation to compare with </param>
		/// <returns> True if the two representation are same. False otherwise.
		/// </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
        {
            throw new Exception();
		  //string valToCompare = arg.StringValue;

		 // sbyte[] value1 = HexBin.decode(_value);
		 // sbyte[] value2 = HexBin.decode(valToCompare);
		 // if (value2 == null)
		 // {
			//return false;
		 // }

		 // int len = value1.Length;
		 // if (len != value2.Length)
		 // {
			//return false;
		 // }

		 // for (int i = 0; i < len; i++)
		 // {
			//if (value1[i] != value2[i])
			//{
			//  return false;
			//}
		 // }

		 // return true;
		}
		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_HEXBINARY;
			}
		}

		public override object NativeValue
		{
			get
            {
                throw new Exception();
                //	return HexBin.decode(_value);
            }
		}
	}

}