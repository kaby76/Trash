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

namespace org.eclipse.wst.xml.xpath2.api.typesystem
{
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// @since 2.0
	/// </summary>
	public interface NodeItemType : ItemType
	{
		/// <summary>
		/// For attribute and element types, return whether the name
		/// part of the type test is a wildcard.
		/// </summary>
		/// <returns> Wildcard test? </returns>
		bool Wildcard {get;}

		/// <returns> name of the item type, if applicable, otherwise null </returns>
		QName Name {get;}

		/// <summary>
		/// Node type as per list in org.w3c.dom.Node
		/// </summary>
		/// <returns> The DOM node type </returns>
		short NodeType {get;}
	}
}