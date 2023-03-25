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
///     Jesper Moller - bug 275610 - Avoid big time and memory overhead for externals
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Document = org.w3c.dom.Document;

	/// <summary>
	/// A representation of the DocumentType datatype
	/// </summary>
	public class DocType : NodeType
	{
		private const string DOCUMENT = "document";
		private Document _value;
		private string _string_value;

		/// <summary>
		/// Initialises according to the supplied parameters
		/// </summary>
		/// <param name="v">
		///            The document being represented </param>
		public DocType(Document v, TypeModel tm) : base(v, tm)
		{
			_value = v;
			_string_value = null;
		}

		/// <summary>
		/// Retrieves the actual document being represented
		/// </summary>
		/// <returns> Actual document being represented </returns>
		public virtual Document value()
		{
			return _value;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "document" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return DOCUMENT;
		}

		/// <summary>
		/// Retrieves a String representation of the document being stored
		/// </summary>
		/// <returns> String representation of the document being stored </returns>
		public override string StringValue
		{
			get
			{
				// XXX caching
				if (string.ReferenceEquals(_string_value, null))
				{
					_string_value = ElementType.textnode_strings(_value);
				}
    
				return _string_value;
			}
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the document being stored
		/// </summary>
		/// <returns> New ResultSequence consisting of the document being stored </returns>
		public override ResultSequence typed_value()
		{
			// XXX no psvi
			return new XSUntypedAtomic(StringValue);
		}

		/// <summary>
		/// Retrieves the name of the node
		/// </summary>
		/// <returns> QName representation of the name of the node </returns>
		public override QName node_name()
		{
			return null;
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool ID
		{
			get
			{
				// TODO Auto-generated method stub
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
				return BuiltinTypeLibrary.XS_UNTYPED;
			}
		}

	}

}