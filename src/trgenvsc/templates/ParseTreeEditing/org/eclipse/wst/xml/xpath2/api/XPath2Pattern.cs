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

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This is a compiled pattern (which is a simpler version of an expression, used to match in XSLT files, etc.)
	/// 
	/// @noimplement This interface is not intended to be implemented by clients.
	/// @since 2.0
	/// </summary>
	public interface XPath2Pattern
	{
		/// <returns> The object passed in my the caller when the patten was created. </returns>
		object UserData {get;}

		bool matches(DynamicContext dc, Node context);

	}

}