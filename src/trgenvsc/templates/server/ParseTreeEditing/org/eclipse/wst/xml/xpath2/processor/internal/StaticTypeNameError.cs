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
///     David Carver (STAR) - bug 273763 - correct error codes  
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	/// <summary>
	/// Static type name error class.
	/// </summary>
	public class StaticTypeNameError : StaticNameError
	{
		/// 
		private const long serialVersionUID = 7328671571088574947L;
		public const string TYPE_NOT_FOUND = "XPST0051";

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reason">
		///            is the reason for the error. </param>
		public StaticTypeNameError(string reason) : base(TYPE_NOT_FOUND, reason)
		{
		}
	}

}