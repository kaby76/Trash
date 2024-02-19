/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2013 Mukul Gandhi, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Mukul Gandhi - bug 277642 - Initial API and implementation, of xs:unsignedInt
///                                 data type.
///     David Carver (STAR) - bug 262765 - fixed abs value tests.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	public class XSUnsignedInt : XSUnsignedLong
	{

		private const string XS_UNSIGNED_INT = "xs:unsignedInt";

		/// <summary>
		/// Initializes a representation of 0
		/// </summary>
		public XSUnsignedInt() : this(new System.Numerics.BigInteger(0))
		{
		}

		/// <summary>
		/// Initializes a representation of the supplied unsignedInt value
		/// </summary>
		/// <param name="x">
		///            unsignedInt to be stored </param>
		public XSUnsignedInt(System.Numerics.BigInteger x) : base(x)
		{
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:unsignedInt" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_UNSIGNED_INT;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "unsignedInt" which is the datatype's name </returns>
		public override string type_name()
		{
			return "unsignedInt";
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable unsignedInt
		/// in the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the unsignedInt is to be extracted </param>
		/// <returns> New ResultSequence consisting of the 'unsignedInt' supplied </returns>
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
			// and convert it's string value to a unsignedInt.
			Item aat = arg.first();

			try
			{
                System.Numerics.BigInteger.TryParse(aat.StringValue, out System.Numerics.BigInteger bigInt);

				// doing the range checking
				// min value is 0
				// max value is 4294967295
				System.Numerics.BigInteger min = new System.Numerics.BigInteger(0);
				System.Numerics.BigInteger max = new System.Numerics.BigInteger(4294967295L);

				if (bigInt.CompareTo(min) < 0 || bigInt.CompareTo(max) > 0)
				{
				   // invalid input
				   throw DynamicError.cant_cast(null);
				}

				return new XSUnsignedInt(bigInt);
			}
			catch (System.FormatException)
			{
				throw DynamicError.cant_cast(null);
			}

		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNSIGNEDINT;
			}
		}

		public override object NativeValue
		{
			get
			{
				return (int) Value;
			}
		}
	}

}