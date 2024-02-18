/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Mukul Gandhi, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Mukul Gandhi - initial API and implementation
///     Mukul Gandhi - bug 334842 - improving support for the data types Name, NCName, ENTITY, 
///                                 ID, IDREF and NMTOKEN.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using XMLChar = org.apache.xerces.util.XMLChar;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the Name datatype
	/// </summary>
	public class XSName : XSToken
	{
		private const string XS_NAME = "xs:Name";

		/// <summary>
		/// Initialises using the supplied String
		/// </summary>
		/// <param name="x">
		///            String to be stored </param>
		public XSName(string x) : base(x)
		{
		}

		/// <summary>
		/// Initialises to null
		/// </summary>
		public XSName() : this(null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:Name" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_NAME;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "Name" which is the datatype's name </returns>
		public override string type_name()
		{
			return "Name";
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable Name within
		/// the supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to extract the Name </param>
		/// <returns> New ResultSequence consisting of the Name supplied </returns>
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
			string strValue = aat.StringValue;

			if (!XMLChar.isValidName(strValue))
			{
				// invalid input
				DynamicError.throw_type_error();
			}

			return new XSName(strValue);
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_NAME;
			}
		}
	}

}