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
	/// @noimplement This interface is not intended to be implemented by clients.
	/// @since 2.0
	/// </summary>
	public interface XPath2PatternSet
	{

		XPath2Pattern compilePattern(StaticContext context, string pattern, string mode, int priority, object userData);
		void removePattern(XPath2Pattern pattern);

		void clear();

		Match match(DynamicContext dc, Node context);
	}

}