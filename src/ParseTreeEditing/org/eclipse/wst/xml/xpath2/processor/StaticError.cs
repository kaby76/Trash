/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2009 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{


	/// <summary>
	/// Base class for all static errors as defined by the XPath 2.0 specification
	/// 
	/// </summary>
	public class StaticError : XPathException
	{
		/// 
		private const long serialVersionUID = 7870866130837870971L;
		// errorcode specified in http://www.w3.org/2004/10/xqt-errors i fink
		private string _code;

		/// <summary>
		/// Constructor for a generic static error
		/// </summary>
		/// <param name="code">
		///            The error code as specified in XPath 2.0 </param>
		/// <param name="err">
		///            Humar readable error message </param>
		public StaticError(string code, string err) : base(err)
		{
			_code = code;
		}

		/// <returns> error code which represents the error </returns>
		public virtual string code()
		{
			return _code;
		}
	}

}