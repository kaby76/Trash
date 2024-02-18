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
	/// Error caused by static name.
	/// </summary>
	public class StaticNameError : StaticError
	{
		/// 
		private const long serialVersionUID = 4363370082563106074L;
		public const string NAME_NOT_FOUND = "XPST0008";
		public const string PREFIX_NOT_FOUND = "XPST0081";

		/// <summary>
		/// Constructor for static name error
		/// </summary>
		/// <param name="code">
		///            is the code. </param>
		/// <param name="reason">
		///            is the reason for the error. </param>
		public StaticNameError(string code, string reason) : base(code, reason)
		{
		}

		/// <summary>
		/// Constructor for static name error
		/// </summary>
		/// <param name="reason">
		///            is the reason for the error. </param>
		public StaticNameError(string reason) : this(NAME_NOT_FOUND, reason)
		{
		}

	}

}