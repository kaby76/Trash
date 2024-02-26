/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Moller, and others
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
	/// Main API for the XPath2 processor.
	/// 
	/// @noimplement This interface is not intended to be implemented by clients.
	/// @since 2.0
	/// </summary>
	public interface XPath2Engine
	{

		/// <summary>
		/// Parse a full XPath2 expression and type check it against the static context
		/// (if it provides a type model to check against)
		/// </summary>
		/// <param name="expression"> String representation of XPath2 expression </param>
		/// <param name="context"> Static context for the expression. </param>
		/// <returns> A compiled expression. </returns>
		XPath2Expression parseExpression(string expression, StaticContext context);
	}

}