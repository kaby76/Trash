using System;
using System.Collections;

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
///     Mukul Gandhi  - bug 276134 - improvements to schema aware primitive type support
///                                 for attribute/element nodes 
///     Jesper Moller - bug 281159 - we were missing out on qualified attributes
///     David Carver  - bug 281186 - implementation of fn:id and fn:idref
///     Jesper Moller- bug 275610 - Avoid big time and memory overhead for externals
///     David Carver (STAR) - bug 289304 - fixe schema awarness of types on attributes
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///      Mukul Gandhi - bug 323900 - improving computing the typed value of element &
///                                  attribute nodes, where the schema type of nodes
///                                  are simple, with varieties 'list' and 'union'.  
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Attr = org.w3c.dom.Attr;
	using TypeInfo = org.w3c.dom.TypeInfo;

	/// <summary>
	/// A representation of the AttributeType datatype
	/// </summary>
	public class AttrType : NodeType
	{
		private const string ATTRIBUTE = "attribute";
		internal Attr _value;

		// constructor only usefull for string_type()
		// XXX needs to be fixed in future
		/// <summary>
		/// Initialises to null
		/// </summary>
		public AttrType() : this(null, null)
		{
		}

		/// <summary>
		/// Initialises according to the supplied parameters
		/// </summary>
		/// <param name="v">
		///            The attribute being represented </param>
		public AttrType(Attr v, TypeModel tm) : base(v, tm)
		{
			_value = v;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "attribute" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return ATTRIBUTE;
		}

		/// <summary>
		/// Retrieves a String representation of the attribute being stored
		/// </summary>
		/// <returns> String representation of the attribute being stored </returns>
		public override string StringValue
		{
			get
			{
				return _value.Value;
			}
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the attribute being stored
		/// </summary>
		/// <returns> New ResultSequence consisting of the attribute being stored </returns>
		public override ResultSequence typed_value()
		{
			TypeDefinition typeDef = Type;

			if (typeDef != null)
			{
				IList types = typeDef.getSimpleTypes(_value);
				 return getXDMTypedValue(typeDef, types);
			}
			else
			{
			   return new XSUntypedAtomic(StringValue);
			}
		}

		/// <summary>
		/// Retrieves the name of the node
		/// </summary>
		/// <returns> Name of the node </returns>
		public override QName node_name()
		{
			QName name = new QName(_value.Prefix, _value.LocalName, _value.NamespaceURI);

			return name;
		}

		/// <summary>
		/// Checks if the current node is of type ID
		/// @since 1.1;
		/// </summary>
		public override bool ID
		{
			get
			{
				return isAttrType(SCHEMA_TYPE_ID);
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
				return isAttrType(SCHEMA_TYPE_IDREF);
			}
		}

		protected internal virtual bool isAttrType(string typeName)
		{
			if (_value.OwnerDocument.isSupported("Core", "3.0"))
			{
				return typeInfo(typeName);
			}
			return false;
		}

		private bool typeInfo(string typeName)
		{
			TypeInfo typeInfo = _value.SchemaTypeInfo;
			return isType(typeInfo, typeName);
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