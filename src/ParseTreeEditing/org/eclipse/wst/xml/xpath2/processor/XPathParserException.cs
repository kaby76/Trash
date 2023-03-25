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
///     David Carver (STAR) - bug 273763 - correct error codes 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{


	/// <summary>
	/// This exception is thrown if there is a problem with the XPath parser.
	/// </summary>
	public class XPathParserException : StaticError
	{

		/// 
		private const long serialVersionUID = -4974805230489762419L;
		/// <summary>
		/// The type of exception.
		/// </summary>
		public const string INVALID_XPATH_EXPRESSION = "XPST0003";

		/// <summary>
		/// Constructor for XPathParserException.
		/// </summary>
		/// <param name="reason">
		///            is the reason why the exception has been thrown. </param>
		public XPathParserException(string reason) : base(INVALID_XPATH_EXPRESSION, reason)
		{
		}

		/// <summary>
		/// Constructor for XPathParserException.
		/// </summary>
		/// <param name="code">
		///            the XPath2 standard code for the problem. </param>
		/// <param name="reason">
		///            is the reason why the exception has been thrown. </param>
		public XPathParserException(string code, string reason) : base(code, reason)
		{
		}
	}

}