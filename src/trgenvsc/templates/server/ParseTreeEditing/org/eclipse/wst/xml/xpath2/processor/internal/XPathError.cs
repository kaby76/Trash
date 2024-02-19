using System;

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

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	/// <summary>
	/// This error is thrown when there is a problem with an XPath exception.
	/// </summary>
	public class XPathError : Exception
	{
		/// 
		private const long serialVersionUID = 6624631792087303209L;
		private string _reason;

		/// <summary>
		/// Constructor for XPathError
		/// </summary>
		/// <param name="reason">
		///            Is the reason why the error has been thrown. </param>
		public XPathError(string reason)
		{
			_reason = reason;
		}

		/// <summary>
		/// The reason why the error has been thrown.
		/// </summary>
		/// <returns> the reason why the error has been throw. </returns>
		public virtual string reason()
		{
			return _reason;
		}
	}

}