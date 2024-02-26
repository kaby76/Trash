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
///     David Carver - bug 262765 - corrected implementation of XSUntypedAtomic 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the UntypedAtomic datatype which is used to represent
	/// untyped atomic nodes.
	/// </summary>
	public class XSUntypedAtomic : CtrType
	{
		private const string XS_UNTYPED_ATOMIC = "xs:untypedAtomic";
		private string _value;


		public XSUntypedAtomic() : this(null)
		{
		}

		/// <summary>
		/// Initialises using the supplied String
		/// </summary>
		/// <param name="x">
		///            The String representation of the value of the untyped atomic
		///            node </param>
		public XSUntypedAtomic(string x)
		{
			_value = x;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:untypedAtomic" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_UNTYPED_ATOMIC;
		}

		/// <summary>
		/// Retrieves a String representation of the value of this untyped atomic
		/// node
		/// </summary>
		/// <returns> String representation of the value of this untyped atomic node </returns>
		public override string StringValue
		{
			get
			{
				return _value;
			}
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
			return new XSUntypedAtomic(aat.StringValue);
		}

		public override string type_name()
		{
			return "untypedAtomic";
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPEDATOMIC;
			}
		}

		public override object NativeValue
		{
			get
			{
				return _value;
			}
		}

	}

}