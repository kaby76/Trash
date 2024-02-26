/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver (STAR) bug 228223 - initial API and implementation
///     Mukul Gandhi - bug 334842 - improving support for the data types Name, NCName, ENTITY, 
///                                 ID, IDREF and NMTOKEN.
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/*
	 * Implements the xs:IDREF data type.
	 * 
	 * @since 1.1
	 */
	public class XSIDREF : XSNCName
	{

		private const string XS_IDREF = "xs:IDREF";

		public XSIDREF(string x) : base(x)
		{
		}

		public XSIDREF() : base()
		{
		}

		public override string string_type()
		{
			return XS_IDREF;
		}

		public override string type_name()
		{
			return "IDREF";
		}

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

			if (!isConstraintSatisfied(strValue))
			{
				// invalid input
				DynamicError.throw_type_error();
			}

			return new XSIDREF(strValue);
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_IDREF;
			}
		}

	}

}