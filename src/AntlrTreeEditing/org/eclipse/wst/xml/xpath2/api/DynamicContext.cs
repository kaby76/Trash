using System.Collections.Generic;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, Jesper Steen Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.api
{
	using URI = java.net.URI;
	using GregorianCalendar = java.util.GregorianCalendar;
	using Duration = javax.xml.datatype.Duration;
	using QName = javax.xml.@namespace.QName;
    using Document = org.w3c.dom.Document;
    using Node = org.w3c.dom.Node;

	/// <summary>
	/// Interface for dynamic context. This covers the Dynamic Context as defined by the XPath2 specification, EXCEPT for 
	/// context item and size (handled in EvaluationContext) 
	/// @since 2.0
	/// </summary>
	public interface DynamicContext
	{

		/// <summary>
		/// Returns a "top" node which the XPath2 engine is not allowed to
		/// leave
		/// </summary>
		/// <returns> DOM node which limits axis navigation. </returns>
		Node LimitNode {get;}

		/// <summary>
		/// Get variable.
		/// </summary>
		/// <param name="name">
		///            is the name of the variable. </param>
		/// <returns> variable. </returns>
		ResultSequence getVariable(QName name);

		/// <summary>
		/// Resolve an URI
		/// </summary>
		/// <param name="uri">
		///            is the possibly relative URI to resolve </param>
		/// <returns> the absolutized, resolved URI. </returns>
		URI resolveUri(string uri);

		/// <summary>
		/// [Definition: Current dateTime. This
		/// information represents an implementation-dependent point in time during
		/// the processing of an expression, and includes an explicit timezone. It
		/// can be retrieved by the fn:current-dateTime function. If invoked
		/// multiple times during the execution of an expression, this function
		/// always returns the same result.]
		///    
		/// Returns the current date time using the GregorianCalendar.
		/// </summary>
		/// <returns> The current date and time, which will always be same for the dynamic context. </returns>
		GregorianCalendar CurrentDateTime {get;}
		/// <summary>
		/// [Definition: Implicit timezone. This
		/// is the timezone to be used when a date, time, or dateTime value that
		/// does not have a timezone is used in a comparison or arithmetic
		/// operation. The implicit timezone is an implementation-defined value of
		/// type xs:dayTimeDuration. See [XML Schema] for the range of legal values
		/// of a timezone.]
		/// </summary>
		/// <returns> current date time and implicit timezone. </returns>
		Duration TimezoneOffset {get;}

		/// <summary>
		/// [Definition: Available documents. This is a mapping of
		/// strings onto document nodes. The string represents the absolute URI of
		/// a resource. The document node is the root of a tree that represents
		/// that resource using the data model. The document node is returned by
		/// the fn:doc function when applied to that URI.] The set of available
		/// documents is not limited to the set of statically known documents, and
		/// it may be empty. If there are one or more URIs in available documents
		/// that map to a document node D, then the document-uri property of D must
		/// either be absent, or must be one of these URIs. Note: This means that
		/// given a document node $N, the result of fn:doc(fn:document-uri($N)) is
		/// $N will always be True, unless fn:document-uri($N) is an empty
		/// sequence.
		/// </summary>
		/// <param name="uri">
		///            is the URI of the document. </param>
		/// <returns> document. </returns>
		Document getDocument(URI uri);

		/// <summary>
		/// [Definition: Available collections. This is a mapping of
		/// strings onto sequences of nodes. The string represents the absolute URI
		/// of a resource. The sequence of nodes represents the result of the
		/// fn:collection function when that URI is supplied as the argument. ] The
		/// set of available collections is not limited to the set of statically
		/// known collections, and it may be empty. For every document node D that
		/// is in the target of a mapping in available collections, or that is the
		/// root of a tree containing such a node, the document-uri property of D
		/// must either be absent, or must be a URI U such that available documents
		/// contains a mapping from U to D." Note: This means that for any document
		/// node $N retrieved using the fn:collection function, either directly or
		/// by navigating to the root of a node that was returned, the result of
		/// fn:doc(fn:document-uri($N)) is $N will always be True, unless
		/// fn:document-uri($N) is an empty sequence. This implies a requirement
		/// for the fn:doc and fn:collection functions to be consistent in their
		/// effect. If the implementation uses catalogs or user-supplied URI
		/// resolvers to dereference URIs supplied to the fn:doc function, the
		/// implementation of the fn:collection function must take these mechanisms
		/// into account. For example, an implementation might achieve this by
		/// mapping the collection URI to a set of document URIs, which are then
		/// resolved using the same catalog or URI resolver that is used by the
		/// fn:doc function.
		/// </summary>
		IDictionary<string, IList<Document>> Collections {get;}
		/// <summary>
		/// [Definition: Default collection. This is the sequence
		/// of nodes that would result from calling the fn:collection function with
		/// no arguments.] The value of default collection may be initialized by
		/// the implementation.
		/// </summary>
		IList<Document> DefaultCollection {get;}

		/// <summary>
		/// Actual collation providers available for use dynamically. This could
		/// differ from the collations available statically, but would give
		/// unexpected results.
		/// </summary>
		CollationProvider CollationProvider {get;}
	}

}