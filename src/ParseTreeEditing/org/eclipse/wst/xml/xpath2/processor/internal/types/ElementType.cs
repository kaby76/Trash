using System;
using System.Text;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2012 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     Mukul Gandhi - bug 276134 - improvements to schema aware primitive type support
///                                  for attribute/element nodes
///     Jesper Moller - bug 275610 - Avoid big time and memory overhead for externals
///     David Carver  - bug 281186 - implementation of fn:id and fn:idref
///     David Carver (STAR) - bug 289304 - fix schema awarness of types on elements
///     Jesper Moller - bug 297958 - Fix fn:nilled for elements
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Mukul Gandhi - bug 323900 - improving computing the typed value of element &
///                                 attribute nodes, where the schema type of nodes
///                                 are simple, with varieties 'list' and 'union'.
///     Lukasz Wycisk - bug 361659 - ElemntType typed value in case of nil=�true�                              
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using PSVIElementNSImpl = org.apache.xerces.dom.PSVIElementNSImpl;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
    using NodeConstants = org.w3c.dom.NodeConstants;
	using NodeList = org.w3c.dom.NodeList;
	using Text = org.w3c.dom.Text;
	using TypeInfo = org.w3c.dom.TypeInfo;

	/// <summary>
	/// A representation of the ElementType datatype
	/// </summary>
	public class ElementType : NodeType
	{
		private const string ELEMENT = "element";

		private const string SCHEMA_INSTANCE = "http://www.w3.org/2001/XMLSchema-instance";
		private const string NIL_ATTRIBUTE = "nil";
		private const string TRUE_VALUE = "true";

		private Element _value;

		private string _string_value;

		/// <summary>
		/// Initialises to a null element
		/// </summary>
		public ElementType() : this(null, null)
		{
		}

		/// <summary>
		/// Initialises according to the supplied parameters
		/// </summary>
		/// <param name="v">
		///            The element being represented </param>
		public ElementType(Element v, TypeModel tm) : base(v, tm)
		{
			_value = v;

			_string_value = null;
		}

		/// <summary>
		/// Retrieves the actual element value being represented
		/// </summary>
		/// <returns> Actual element value being represented </returns>
		public virtual Element value()
		{
			return _value;
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "element" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return ELEMENT;
		}

		/// <summary>
		/// Retrieves a String representation of the element being stored
		/// </summary>
		/// <returns> String representation of the element being stored </returns>
		public override string StringValue
		{
			get
			{
				// XXX can we cache ?
				if (!string.ReferenceEquals(_string_value, null))
				{
					return _string_value;
				}
    
				_string_value = textnode_strings(_value);
    
				return _string_value;
			}
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the element stored
		/// </summary>
		/// <returns> New ResultSequence consisting of the element stored </returns>
		public override ResultSequence typed_value()
		{

			TypeDefinition typeDef = Type;

			if (!isNilled(_value))
			{
				if (typeDef != null)
				{
					return getXDMTypedValue(typeDef, typeDef.getSimpleTypes(_value));
				}
				else
				{
					return new XSUntypedAtomic(StringValue);
				}
			}
			return ResultBuffer.EMPTY;
		}

		private bool isNilled(Element _value2)
		{
			return TRUE_VALUE.Equals(_value2.getAttributeNS(SCHEMA_INSTANCE, NIL_ATTRIBUTE));
		}

		// recursively concatenate TextNode strings
		/// <summary>
		/// Recursively concatenate TextNode strings
		/// </summary>
		/// <param name="node">
		///            Node to recurse </param>
		/// <returns> String representation of the node supplied </returns>
		public static string textnode_strings(Node node)
		{
			string result = "";

			if (node.NodeType == NodeConstants.TEXT_NODE)
			{
				Text tn = (Text) node;
				result += tn.Data;
			}

			NodeList nl = node.ChildNodes;

			StringBuilder buf = new StringBuilder(result);
			// concatenate children
			for (int i = 0; i < nl.Length; i++)
			{
				Node n = nl.item(i);

				buf.Append(textnode_strings(n));
			}

			result = buf.ToString();
			return result;
		}

		/// <summary>
		/// Retrieves the name of the node
		/// </summary>
		/// <returns> QName representation of the name of the node </returns>
		public override QName node_name()
		{
			QName name = new QName(_value.Prefix, _value.LocalName, _value.NamespaceURI);

			return name;
		}

		public override ResultSequence nilled()
		{

			if (_value is PSVIElementNSImpl)
			{
				PSVIElementNSImpl psviElement = (PSVIElementNSImpl) _value;
				return XSBoolean.valueOf(psviElement.Nil);
			}
			else
			{
				return XSBoolean.FALSE;
			}
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool ID
		{
			get
			{
				return isElementType(SCHEMA_TYPE_ID);
			}
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public override bool IDREF
		{
			get
			{
				return isElementType(SCHEMA_TYPE_IDREF);
			}
		}

		protected internal virtual bool isElementType(string typeName)
		{
			TypeInfo typeInfo = _value.SchemaTypeInfo;
			return isType(typeInfo, typeName);
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