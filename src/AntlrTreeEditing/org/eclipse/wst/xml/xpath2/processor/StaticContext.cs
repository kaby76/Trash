using System.Collections.Generic;

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
///     David Carver (STAR) - bug 262765 - add ability to set the base uri 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.function.FunctionLibrary;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;
	using Node = org.w3c.dom.Node;

	using Document = org.w3c.dom.Document;

	/// <summary>
	/// interface to static context
	/// </summary>
	/// @deprecated See <tt>org.eclipse.wst.xml.xpath2.api.StaticContext</tt> instead 
	public interface StaticContext
	{

		/// <summary>
		/// is it xpath 1.0 compatible.
		/// </summary>
		/// <returns> boolean </returns>
		bool xpath1_compatible();

		/// <summary>
		/// namespaces does the prefix exist
		/// </summary>
		/// <param name="prefix">
		///            is the prefix </param>
		/// <returns> boolean </returns>
		bool prefix_exists(string prefix);

		/// <param name="prefix">
		///            is the prefix </param>
		/// <returns> string </returns>
		string resolve_prefix(string prefix);

		/// <summary>
		/// the default namespace
		/// </summary>
		/// <returns> string </returns>
		string default_namespace();

		/// <summary>
		/// the default function namespace
		/// </summary>
		/// <returns> string </returns>
		string default_function_namespace();

		// in scope schema definitions
		/// <param name="attr">
		///            is the qname variable </param>
		/// <returns> attributes's type definition
		/// @since 2.0 </returns>
		TypeDefinition attribute_type_definition(QName attr);

		/// <param name="elem">
		///            is the elem of the qname </param>
		/// <returns> element's type definition </returns>
		TypeDefinition element_type_definition(QName elem);

		/// <summary>
		/// is the attribute declared?
		/// </summary>
		/// <param name="attr">
		///            is the attribute of the qname </param>
		/// <returns> boolean </returns>
		bool attribute_declared(QName attr);

		/// <summary>
		/// is the element declared?
		/// </summary>
		/// <param name="elem">
		///            is the elem of the qname </param>
		/// <returns> boolean </returns>
		bool element_declared(QName elem);

		// in scope variables

		// context item type

		/// <summary>
		/// is the element declared?
		/// </summary>
		/// <param name="name">
		///            is the qname name </param>
		/// <param name="arity">
		///            integer of qname </param>
		/// <returns> boolean </returns>
		// function signatures
		bool function_exists(QName name, int arity);

		// collations

		/// <summary>
		/// base uri
		/// </summary>
		/// <returns> uri </returns>
		// base uri
		XSAnyURI base_uri();

		// statically known documents

		// collections

		// other stuff
		/// <summary>
		/// new scope
		/// </summary>
		void new_scope();

		/// <summary>
		/// destroy scope
		/// </summary>
		void destroy_scope();

		/// <summary>
		/// add variable
		/// </summary>
		/// <param name="name">
		///            is the qname </param>
		void add_variable(QName name);

		/// <summary>
		/// delete the variable
		/// </summary>
		/// <param name="name">
		///            is the qname </param>
		/// <returns> boolean if deleted variable </returns>
		bool del_variable(QName name);

		/// <param name="name">
		///            is the qname </param>
		/// <returns> boolean if variable exists </returns>
		bool variable_exists(QName name); // in current scope only

		/// <param name="var">
		///            is the variable of qname </param>
		bool variable_in_scope(QName @var);

		/// <param name="name">
		///            is qname </param>
		/// <returns> boolean </returns>
		bool type_defined(QName name);

		/// <param name="at">
		///            the node type </param>
		/// <param name="et">
		///            is the qname </param>
		/// <returns> boolean </returns>
		bool derives_from(NodeType at, QName et);

		/// <param name="at">
		///            the node type </param>
		/// <param name="et">
		///            is the XSTypeDefinition of the node </param>
		/// <returns> boolean
		/// @since 2.0 </returns>
		bool derives_from(NodeType at, TypeDefinition et);

		/// <summary>
		/// add namespace
		/// </summary>
		/// <param name="prefix">
		///            the prefix of the namespace </param>
		/// <param name="ns">
		///            is the XSTypeDefinition of the node </param>
		void add_namespace(string prefix, string ns);

		/// <summary>
		/// expand function
		/// </summary>
		/// <param name="name">
		///            is the qname </param>
		/// <returns> boolean if function can be expanded </returns>
		bool expand_function_qname(QName name);

		/// <summary>
		/// expand element type qname
		/// </summary>
		/// <param name="name">
		///            is the qname </param>
		/// <returns> boolean if function can be expanded </returns>
		bool expand_elem_type_qname(QName name);

		/// <summary>
		/// expand qname
		/// </summary>
		/// <param name="name">
		///            is the qname </param>
		/// <returns> boolean if function can be expanded </returns>
		bool expand_qname(QName name);

		/// <summary>
		/// add function to library
		/// </summary>
		/// <param name="fl">
		///            is the function library </param>
		void add_function_library(FunctionLibrary fl);

		/// <param name="name">
		///            is the qname </param>
		/// <returns> any atomic type </returns>
		AnyAtomicType make_atomic(QName name);

		/// <summary>
		/// Sets the base uri for the context. </summary>
		/// <param name="baseuri">
		/// @since 1.1 </param>
		void set_base_uri(string baseuri);

		/// <summary>
		/// @since 1.1
		/// 
		/// Gets the collections map, which maps a String into a List of Document, in
		/// Java5 it would be <code>Map<String, List<Document>></code>
		/// 
		/// </summary>
        IDictionary<string, IList<Document>> get_collections();

		/// <summary>
		/// @since 1.1
		/// 
		/// Sets the collections map, which maps a String into a List of Document, in
		/// Java5 it would be <code>Map<String, List<Document>></code>
		/// </summary>
		void set_collections(IDictionary<string, IList<Document>> collections);

		/// <summary>
		/// Gets the type provider in use for the specified DOM node.
		/// 
		/// @since 2.0
		/// </summary>
		TypeModel getTypeModel(Node element);

	}

}