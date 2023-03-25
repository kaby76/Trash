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
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;

	/// <summary>
	/// Common base class for Attribute and Element tests.
	/// </summary>
	public abstract class AttrElemTest : KindTest
	{
		private QName _name;
		private QName _type; // may be null
		private bool _wild; // use wild qname ?

		/// <summary>
		/// Constructor for Attribute and Element tests. This takes in 3 inputs,
		/// Name, wildcard test(true/false) and type.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="wild">
		///            Wildcard test? True/False. </param>
		/// <param name="type">
		///            QName type. </param>
		public AttrElemTest(QName name, bool wild, QName type)
		{
			_name = name;
			_wild = wild;
			_type = type;
		}

		/// <summary>
		/// Constructor for Attribute and Element tests. This takes in 2 inputs, Name
		/// and wildcard test(true/false).
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="wild">
		///            Wildcard test? True/False. </param>
		public AttrElemTest(QName name, bool wild) : this(name, wild, null)
		{
		}

		/// <summary>
		/// Default Constructor for Attribute and Element tests. This takes in no
		/// inputs.
		/// </summary>
		public AttrElemTest() : this(null, false)
		{
		}

		/// <summary>
		/// Support for wildcard test.
		/// </summary>
		/// <returns> Result of wildcard test. </returns>
		public virtual bool wild()
		{
			return _wild;
		}

		/// <summary>
		/// Support for name test.
		/// </summary>
		/// <returns> Result of name test. </returns>
		public override QName name()
		{
			return _name;
		}

		/// <summary>
		/// Support for type test.
		/// </summary>
		/// <returns> Result of type test. </returns>
		public virtual QName type()
		{
			return _type;
		}

		protected internal virtual short DerviationTypes
		{
			get
			{
				return org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_LIST | org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_EXTENSION | org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_RESTRICTION | org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_SUBSTITUTION;
			}
		}
	}

}