/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2013 Andrea Bittau, University College London, and others
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
	using InternalXPathParser = org.eclipse.wst.xml.xpath2.processor.@internal.InternalXPathParser;

	/// <summary>
	/// JFlexCupParser parses the xpath expression
	/// </summary>
	public class XPathParserInAntlr : XPathParser
	{

		/// <summary>
		/// Tries to parse the xpath expression
		/// </summary>
		/// <param name="xpath">
		///            is the xpath string. </param>
		/// <exception cref="XPathParserException."> </exception>
		/// <returns> the xpath value. </returns>
		public virtual XPath parse(string xpath)
		{
            return (new InternalXPathParser()).parse(xpath, false);
		}

		/// <summary>
		/// Tries to parse the xpath expression
		/// </summary>
		/// <param name="xpath">
		///            is the xpath string. </param>
		/// <param name="isRootlessAccess">
		///            if 'true' then PsychoPath engine can't parse xpath expressions starting with / or //. </param>
		/// <exception cref="XPathParserException."> </exception>
		/// <returns> the xpath value.
		/// @since 2.0 </returns>
		public virtual XPath parse(string xpath, bool isRootlessAccess)
		{

			return (new InternalXPathParser()).parse(xpath, isRootlessAccess);
		}
	}

}