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
///     Mukul Gandhi - bug 277599 - Initial API and implementation, of xs:nonPositiveInteger
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

	public class XSNonPositiveInteger : XSInteger
	{

		private const string XS_NON_POSITIVE_INTEGER = "xs:nonPositiveInteger";

		/// <summary>
		/// Initializes a representation of 0
		/// </summary>
		public XSNonPositiveInteger() : this(new System.Numerics.BigInteger(0))
		{
		}

		/// <summary>
		/// Initializes a representation of the supplied nonPositiveInteger value
		/// </summary>
		/// <param name="x">
		///            nonPositiveInteger to be stored </param>
		public XSNonPositiveInteger(System.Numerics.BigInteger x) : base(x)
		{
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:nonPositiveInteger" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_NON_POSITIVE_INTEGER;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "nonPositiveInteger" which is the datatype's name </returns>
		public override string type_name()
		{
			return "nonPositiveInteger";
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable nonPositiveInteger
		/// in the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which the nonPositiveInteger is to be extracted </param>
		/// <returns> New ResultSequence consisting of the 'nonPositiveInteger' supplied </returns>
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
			// and convert it's string value to a nonPositiveInteger.
			Item aat = arg.first();

			try
			{
				System.Numerics.BigInteger.TryParse(aat.StringValue, out System.Numerics.BigInteger bigInt);

				// doing the range checking
				// min value is, -INF
				// max value is 0
				System.Numerics.BigInteger max = new System.Numerics.BigInteger(0L);

				if (bigInt.CompareTo(max) > 0)
				{
				   // invalid input
				   throw DynamicError.cant_cast(null);
				}

				return new XSNonPositiveInteger(bigInt);
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
				return BuiltinTypeLibrary.XS_NONPOSITIVEINTEGER;
			}
		}

	}

}