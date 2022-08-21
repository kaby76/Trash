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
	/// Static namespace name error class.
	/// </summary>
	public class StaticNsNameError : StaticNameError
	{

		/// 
		private const long serialVersionUID = -6873980377966290062L;

		public StaticNsNameError(string reason) : base(PREFIX_NOT_FOUND, reason)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pref">
		///            is the unknown prefix. </param>
		/// <returns> the error. </returns>
		public static StaticNsNameError unknown_prefix(string pref)
		{
			string error = "Unknown prefix";

			if (!string.ReferenceEquals(pref, null))
			{
				error += ": " + pref;
			}
			error += ".";

			return new StaticNsNameError(error);
		}
	}

}