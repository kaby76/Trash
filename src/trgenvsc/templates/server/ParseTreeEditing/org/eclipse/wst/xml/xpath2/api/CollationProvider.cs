using System.Collections.Generic;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Moller, and others
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

	/// <summary>
	/// Service provider interface for looking up collations from within the
	/// dynamic context.
	/// 
	/// Definition from the XPath2 specification: A collation is a specification of
	/// the manner in which strings and URIs are compared and, by extension,
	/// ordered. For a more complete , see [XQuery 1.0 and
	/// XPath 2.0 Functions and Operators (Second Edition)].
	/// @since 2.0
	/// </summary>
	public interface CollationProvider
	{
		/// <summary>
		/// The default collation which is guaranteed to always be implemented
		/// </summary>

		/// <summary>
		/// Gets the named collator. W3C does not define collation names (yet?) so
		/// we are constrained to using an implementation-defined naming scheme.
		/// </summary>
		/// <param name="name">
		///            A URI designating the collation to use </param>
		/// <returns> The collation to use, or null if no such collation exists by
		///         this provider </returns>
		IComparer<string> getCollation(string name);

		string DefaultCollation {get;}
	}

	public static class CollationProvider_Fields
	{
		public const string CODEPOINT_COLLATION = "http://www.w3.org/2005/xpath-functions/collation/codepoint";
	}

}