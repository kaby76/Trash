using System;

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
///     Jesper Steen Moller  - bug 290337 - Revisit use of ICU
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	/// <summary>
	/// This exception is thrown when there is a problem with an XPath exception.
	/// </summary>
	public class XPathException : Exception
	{
		/// 
		private const long serialVersionUID = 1380394170163983863L;
		private string _reason;

		/// <summary>
		/// Constructor for XPathException
		/// </summary>
		/// <param name="reason">
		///            Is the reason why the exception has been thrown. </param>
		public XPathException(string reason)
		{
			_reason = reason;
		}

		/// <summary>
		/// The reason why the exception has been thrown.
		/// </summary>
		/// <returns> the reason why the exception has been throw. </returns>
		public virtual string reason()
		{
			return _reason;
		}

		public override string Message
		{
			get
			{
				return _reason;
			}
		}
	}

}