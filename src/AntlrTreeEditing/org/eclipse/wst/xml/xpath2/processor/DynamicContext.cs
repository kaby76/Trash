
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
///     Mukul Gandhi - bug 273760 - wrong namespace for functions and data types
///     David Carver - bug 282223 - implementation of xs:duration data types. 
///     Jesper Moller- bug 281159 - fix document loading and resolving URIs 
///     Jesper Moller- bug 286452 - always return the stable date/time from dynamic context
///     Jesper Moller- bug 275610 - Avoid big time and memory overhead for externals
///     Jesper Moller- bug 280555 - Add pluggable collation support
///     Mukul Gandhi - bug 325262 - providing ability to store an XPath2 sequence into
///                                 an user-defined variable.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

using System.Collections;
using System.Collections.Generic;
using java.net;

namespace org.eclipse.wst.xml.xpath2.processor
{
	using GregorianCalendar = java.util.GregorianCalendar;
	using URI = java.net.URI;
	using Focus = org.eclipse.wst.xml.xpath2.processor.@internal.Focus;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Interface for dynamic context.
	/// </summary>
	/// @deprecated Use  
	public interface DynamicContext : StaticContext
	{

		/// <summary>
		/// The default collation which is guaranteed to always be implemented
		/// @since 1.1
		/// </summary>

		/// <summary>
		/// Get context item.
		/// </summary>
		/// <returns> the context item. </returns>
		AnyType context_item();

		/// <summary>
		/// Get context node position.
		/// </summary>
		/// <returns> position of context node. </returns>
		int context_position();

		/// <summary>
		/// Get position of last item.
		/// </summary>
		/// <returns> last item position. </returns>
		int last();

		/// <summary>
		/// Get variable.
		/// </summary>
		/// <param name="name">
		///            is the name of the variable. </param>
		/// <returns> variable.
		/// @since 2.0 </returns>
		object get_variable(QName name);

		/// <summary>
		/// Set variable.
		/// </summary>
		/// <param name="var">
		///            is name of the variable. </param>
		/// <param name="val">
		///            is the value to be set for the variable. </param>
		void set_variable(QName @var, AnyType val);


		/// <summary>
		/// Sets a XPath2 sequence into a variable.
		/// 
		/// @since 2.0
		/// </summary>
		void set_variable(QName @var, ResultSequence val);

		/// <summary>
		/// Evaluate the function of the arguments.
		/// </summary>
		/// <param name="name">
		///            is the name. </param>
		/// <param name="args">
		///            are the arguments. </param>
		/// <exception cref="DynamicError">
		///             dynamic error. </exception>
		/// <returns> result of the function evaluation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSequence evaluate_function(org.eclipse.wst.xml.xpath2.processor.internal.types.QName name, java.util.Collection args) throws DynamicError;
		ResultSequence evaluate_function(QName name, ICollection args);

		/// <summary>
		/// Reads the day from a TimeDuration type
		/// </summary>
		/// <returns> current date time and implicit timezone.
		/// @since 1.1 </returns>
		XSDuration tz();

		/// <summary>
		/// Get document.
		/// </summary>
		/// <param name="uri">
		///            is the URI of the document. </param>
		/// <returns> document.
		/// @since 1.1 </returns>
		// available doc
		ResultSequence get_doc(URI uri);

		/// <summary>
		/// Resolve an URI
		/// </summary>
		/// <param name="uri">
		///            is the possibly relative URI to resolve </param>
		/// <returns> the absolutized, resolved URI.
		/// @since 1.1 </returns>
		URI resolve_uri(string uri);

		// available collections

		// default collection

		/// <summary>
		/// Returns the current date time using the GregorianCalendar.
		/// </summary>
		/// <returns> The current date and time, which will always be same for the dynamic context.
		/// @since 1.1 </returns>
		GregorianCalendar current_date_time();

		/// <summary>
		/// Set focus.
		/// </summary>
		/// <param name="focus">
		///            is focus to be set. </param>
		// Other functions
		void set_focus(Focus focus);

		/// <summary>
		/// Return focus.
		/// </summary>
		/// <returns> Focus </returns>
		Focus focus();

		/// <summary>
		/// Return a useful collator for the specified URI
		/// </summary>
		/// <param name="uri"> </param>
		/// <returns> A Jaa collator, or null, if no such Collator exists 
		/// @since 1.1 </returns>
		IComparer<string> get_collation(string uri);

		/// <summary>
		/// Returns the current default collator
		/// </summary>
		/// <returns> The default name to use as the collator
		/// @since 1.1 </returns>
		string default_collation_name();

		// deprecated
		/// <summary>
		/// @deprecated
		/// </summary>
		int node_position(Node node);

	}

	public static class DynamicContext_Fields
	{
		public const string CODEPOINT_COLLATION = "http://www.w3.org/2005/xpath-functions/collation/codepoint";
	}

}