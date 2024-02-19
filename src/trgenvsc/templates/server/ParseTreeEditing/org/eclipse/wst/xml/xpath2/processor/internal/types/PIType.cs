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
	using ProcessingInstruction = org.w3c.dom.ProcessingInstruction;

	/// <summary>
	/// A representation of the ProcessingInstruction datatype
	/// </summary>
	public class PIType : NodeType
	{
		private const string PROCESSING_INSTRUCTION = "processing instruction";
		private ProcessingInstruction _value;

		/// <summary>
		/// Initialises according to the supplied parameters
		/// </summary>
		/// <param name="v">
		///            The processing instruction this node represents </param>
		/// <param name="doc_order">
		///            The document order </param>
		public PIType(ProcessingInstruction v, TypeModel tm) : base(v, tm)
		{
			_value = v;
		}

		/// <summary>
		/// Retrieves the actual processing instruction this node represents
		/// </summary>
		/// <returns> Actual processing instruction this node represents </returns>
		public virtual ProcessingInstruction value()
		{
			return _value;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "processing-instruction" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return PROCESSING_INSTRUCTION;
		}

		/// <summary>
		/// Retrieves a String representation of the actual processing instruction
		/// stored
		/// </summary>
		/// <returns> String representation of the actual processing instruction stored </returns>
		public override string StringValue
		{
			get
			{
				return _value.Data;
			}
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the processing instruction
		/// stored
		/// </summary>
		/// <returns> New ResultSequence consisting of the processing instruction
		///         stored </returns>
		public override ResultSequence typed_value()
		{
			return new XSString(StringValue);
		}

		/// <summary>
		/// Constructs the node's name
		/// </summary>
		/// <returns> A QName representation of the node's name </returns>
		public override QName node_name()
		{
			QName name = new QName(null, _value.Target);

			name.set_namespace(null);

			return name;
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool ID
		{
			get
			{
				return false;
			}
		}

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