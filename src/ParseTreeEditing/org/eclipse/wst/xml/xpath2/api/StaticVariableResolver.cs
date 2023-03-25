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
	using QName = javax.xml.@namespace.QName;
	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;

	/// <summary>
	/// @since 2.0
	/// </summary>
	public interface StaticVariableResolver
	{

		/// <summary>
		/// Is the variable present in the current context.
		/// </summary>
		/// <param name="name"> Variable name </param>
		/// <returns> Availability of the variable </returns>
		bool isVariablePresent(QName name);

		/// <param name="name">
		/// @return </param>
		ItemType getVariableType(QName name);
	}

}