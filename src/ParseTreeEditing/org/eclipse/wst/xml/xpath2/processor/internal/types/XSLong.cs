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
///     Mukul Gandhi - bug 274952 - Initial API and implementation, of xs:long data 
///                                 type.
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

	public class XSLong : XSInteger
	{

		private const string XS_LONG = "xs:long";

		/// <summary>
		/// Initializes a representation of 0
		/// </summary>
		public XSLong() : this(System.Numerics.BigInteger.Zero)
		{
		}

		/// <summary>
		/// Initializes a representation of the supplied long value
		/// </summary>
		/// <param name="x">
		///            Long to be stored </param>
		public XSLong(System.Numerics.BigInteger x) : base(x)
		{
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:long" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_LONG;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "long" which is the datatype's name </returns>
		public override string type_name()
		{
			return "long";
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable long in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the long is to be extracted </param>
		/// <returns> New ResultSequence consisting of the 'long' supplied </returns>
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
			// and convert it's string value to a long.
			Item aat = arg.first();

			try
			{
				System.Numerics.BigInteger.TryParse(aat.StringValue, out System.Numerics.BigInteger bigInt);

				// doing the range checking
				System.Numerics.BigInteger min = new System.Numerics.BigInteger(-9223372036854775808L);
				System.Numerics.BigInteger max = new System.Numerics.BigInteger(9223372036854775807L);

				if (bigInt.CompareTo(min) < 0 || bigInt.CompareTo(max) > 0)
				{
				   // invalid input
				   DynamicError.throw_type_error();
				}

				return new XSLong(bigInt);
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
				return BuiltinTypeLibrary.XS_LONG;
			}
		}

	}

}