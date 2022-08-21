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
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// @since 2.0
	/// </summary>
	public interface TypeModel
	{
		TypeDefinition getType(Node node);
		TypeDefinition lookupType(string @namespace, string typeName);
		TypeDefinition lookupElementDeclaration(string @namespace, string elementName);
		TypeDefinition lookupAttributeDeclaration(string @namespace, string attributeName);

		/// <param name="at">
		///            the node type </param>
		/// <param name="et">
		///            is the qname </param>
		/// <returns> boolean </returns>
	//	public boolean derivesFrom(Node at, QName et);

		/// <param name="at">
		///            the node type </param>
		/// <param name="et">
		///            is the XSTypeDefinition of the node </param>
		/// <returns> boolean </returns>
	//	public boolean derivesFrom(Node at, TypeDefinition et);


	}

}