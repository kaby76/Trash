using System.Collections.Generic;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Jesper Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Moller - initial API and implementation
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.api
{
	using URI = java.net.URI;
	using NamespaceContext = javax.xml.@namespace.NamespaceContext;
	using QName = javax.xml.@namespace.QName;
	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;

	/// <summary>
	/// interface to static context
	/// @since 2.0
	/// </summary>
	public interface StaticContext
	{
		/// <summary>
		/// XPath 1.0 compatibility mode.
		/// </summary>
		/// <returns> true if rules for backward compatibility with XPath Version 1.0
		///         are in effect; otherwise false. </returns>
		bool XPath1Compatible {get;}

		/// <summary>
		/// [Definition: In-scope variables. This is a set of (expanded QName,
		/// type) pairs. It defines the set of variables that are available for
		/// reference within an expression. The expanded QName is the name of the
		/// variable, and the type is the static type of the variable.] An
		/// expression that binds a variable (such as a for, some, or every
		/// expression) extends the in-scope variables of its subexpressions with
		/// the new bound variable and its type.
		/// </summary>
		StaticVariableResolver InScopeVariables {get;}

		 /// <summary>
		 /// [Definition: Context item static type. This component defines the
		 /// static type of the context item within the scope of a given
		 /// expression.]
		 /// </summary>
		TypeDefinition InitialContextType {get;}

		 /// <summary>
		 /// [Definition: Function signatures. This component defines the set of
		 /// functions that are available to be called from within an expression.
		 /// Each function is uniquely identified by its expanded QName and its
		 /// arity (number of parameters).] In addition to the name and arity, each
		 /// function signature specifies the static types of the function
		 /// parameters and result. The function signatures include the signatures
		 /// of constructor functions, which are discussed in 3.10.4 Constructor
		 /// Functions.
		 /// </summary>
		IDictionary<string, FunctionLibrary> FunctionLibraries {get;}

		/// <summary>
		/// [Definition: Statically known collations. This is an
		/// implementation-defined set of (URI, collation) pairs. It defines the
		/// names of the collations that are available for use in processing
		/// expressions.]
		/// </summary>
		CollationProvider CollationProvider {get;}

		/// <summary>
		/// [Definition: Base URI. This is an absolute URI, used when necessary in
		/// the resolution of relative URIs (for example, by the fn:resolve-uri
		/// function.)] The URI value is whitespace normalized according to the
		/// rules for the xs:anyURI type in [XML Schema].
		/// </summary>
		URI BaseUri {get;}

		/// <summary>
		/// [Definition: Statically known documents. This is a mapping from strings
		/// onto types. The string represents the absolute URI of a resource that
		/// is potentially available using the fn:doc function. The type is the
		/// static type of a call to fn:doc with the given URI as its literal
		/// argument. ] If the argument to fn:doc is a string literal that is not
		/// present in statically known documents, then the static type of fn:doc
		/// is document-node()?. Note: The purpose of the statically known
		/// documents is to provide static type information, not to determine which
		/// documents are available. A URI need not be found in the statically
		/// known documents to be accessed using fn:doc.
		/// </summary>
		ItemType getDocumentType(URI documentUri);

		/// <summary>
		/// [Definition: Statically known namespaces. This is a set of (prefix,
		/// URI) pairs that define all the namespaces that are known during static
		/// processing of a given expression.] The URI value is whitespace
		/// normalized according to the rules for the xs:anyURI type in [XML
		/// Schema]. Note the difference between in-scope namespaces, which is a
		/// dynamic property of an element node, and statically known namespaces,
		/// which is a static property of an expression. </summary>
		/// <returns> The statically known namespace context  </returns>
		NamespaceContext NamespaceContext {get;}

		/// <summary>
		///  [Definition: Default element/type namespace. This is a namespace URI or
		/// "none". The namespace URI, if present, is used for any unprefixed QName
		/// appearing in a position where an element or type name is expected.] The
		/// URI value is whitespace normalized according to the rules for the
		/// xs:anyURI type in [XML Schema].
		/// 
		/// @return
		/// </summary>
		string DefaultNamespace {get;}

		/// <summary>
		/// Definition: Default function namespace. This is a namespace URI or
		/// "none". The namespace URI, if present, is used for any unprefixed QName
		/// appearing in a position where a function name is expected.] The URI
		/// value is whitespace normalized according to the rules for the xs:anyURI
		/// type in [XML Schema].
		/// </summary>
		/// <returns> The default function namespace </returns>
		string DefaultFunctionNamespace {get;}

		/// <summary>
		/// [Definition: In-scope schema definitions. This is a generic term for
		/// all the element declarations, attribute declarations, and schema type
		/// definitions that are in scope during processing of an expression.] 
		/// </summary>
		/// <returns> A type model which covers the  </returns>
		TypeModel TypeModel {get;}

		/// <summary>
		/// is the function declared/available in the source context?
		/// </summary>
		/// <param name="name">
		///            is the qname name </param>
		/// <param name="arity">
		///            integer of qname </param>
		/// <returns> boolean </returns>
		// function signatures
		Function resolveFunction(QName name, int arity);

		/// <summary>
		/// [Definition: Statically known collections. This is a mapping from
		/// strings onto types. The string represents the absolute URI of a
		/// resource that is potentially available using the fn:collection
		/// function. The type is the type of the sequence of nodes that would
		/// result from calling the fn:collection function with this URI as its
		/// argument.] If the argument to fn:collection is a string literal that is
		/// not present in statically known collections, then the static type of
		/// fn:collection is node()*. Note: The purpose of the statically known
		/// collections is to provide static type information, not to determine
		/// which collections are available. A URI need not be found in the
		/// statically known collections to be accessed using fn:collection.
		/// </summary>
		TypeDefinition getCollectionType(string collectionName);

		/// <summary>
		/// [Definition: Statically known default collection type. This is the type
		/// of the sequence of nodes that would result from calling the
		/// fn:collection function with no arguments.] Unless initialized to some
		/// other value by an implementation, the value of statically known default
		/// collection type is node()*.
		/// </summary>
		TypeDefinition DefaultCollectionType {get;}
	}

}