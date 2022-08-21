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
///     Jesper Moller- bug 275610 - Avoid big time and memory overhead for externals
///     David Carver  - bug 281186 - implementation of fn:id and fn:idref
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Text = org.w3c.dom.Text;

	/// <summary>
	/// A representation of the TextType datatype
	/// </summary>
	public class TextType : NodeType
	{
		private const string TEXT = "text";
		private Text _value;

		/// <summary>
		/// Initialises using the supplied parameters
		/// </summary>
		/// <param name="v">
		///            The value of the TextType node </param>
		public TextType(Text v, TypeModel tm) : base(v, tm)
		{
			_value = v;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "text" which is the datatype's name </returns>
		public override string string_type()
		{
			return TEXT;
		}

		/// <summary>
		/// Retrieves a String representation of the actual value stored
		/// </summary>
		/// <returns> String representation of the actual value stored </returns>
		public override string StringValue
		{
			get
			{
				return (string)_value.NodeValue;
			}
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the Text value stored
		/// </summary>
		/// <returns> New ResultSequence consisting of the Text value stored </returns>
		public override ResultSequence typed_value()
		{
			return new XSUntypedAtomic(_value.Data);
		}

		/// <summary>
		/// Unsupported method for this nodetype.
		/// </summary>
		/// <returns> null (no user defined name for this node gets defined) </returns>
		public override QName node_name()
		{
			return null;
		}

		/// <summary>
		/// Will always return false;
		/// @since 1.1
		/// </summary>
		public override bool ID
		{
			get
			{
    
				return false;
			}
		}

		/// 
		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool IDREF
		{
			get
			{
				return false;
			}
		}

		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPEDATOMIC;
			}
		}
	}

}