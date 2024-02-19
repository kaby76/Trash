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
///     Bug 338494    - prohibiting xpath expressions starting with / or // to be parsed.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;

	/// <summary>
	/// This is an interface class for the XPath parser.
	/// </summary>
	public interface XPathParser
	{

		/// <summary>
		/// Constructor for the XPath parser interface.
		/// </summary>
		/// <param name="xpath">
		///            is the input XPath to be parsed. </param>
		/// <exception cref="XPathParserException">
		///             XPath parser exception. </exception>
		/// <returns> The parsed XPath. </returns>
		XPath parse(string xpath);

		/// <summary>
		/// Constructor for the XPath parser interface.
		/// </summary>
		/// <param name="xpath">
		///            is the input XPath to be parsed. </param>
		/// <param name="isRootlessAccess">
		///            if 'true' then PsychoPath engine can't parse xpath expressions starting with / or //. </param>
		/// <exception cref="XPathParserException">
		///             XPath parser exception. </exception>
		/// <returns> The parsed XPath.
		/// @since 2.0 </returns>
		XPath parse(string xpath, bool isRootlessAccess);
	}

}