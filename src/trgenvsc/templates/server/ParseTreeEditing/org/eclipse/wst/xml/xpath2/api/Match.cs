/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Jesper Moller, and others
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
	/// A match found by the XPath2 pattern matcher
	/// 
	/// * @noimplement This interface is not intended to be implemented by clients.
	/// @since 2.0
	/// 
	/// </summary>

	public interface Match
	{
		/// <returns> The number of matching patterns on the input. </returns>
		int MatchingCount {get;}

		/// <summary>
		/// Returns the XPath2 pattern which best matched the input (considering mode and priority)
		/// </summary>
		/// <returns> Pattern which was the best match. </returns>
		XPath2Pattern BestMatch {get;}
	}

}