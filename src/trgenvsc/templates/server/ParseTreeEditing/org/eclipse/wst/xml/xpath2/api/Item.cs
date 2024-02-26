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

	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;

	/// <summary>
	/// An item in the XPath2 data model
	/// 
	/// @since 2.0
	/// @noimplement This interface is not intended to be implemented by clients.
	/// </summary>

	public interface Item
	{
		/// <returns> A description of the item type. </returns>
		ItemType ItemType {get;}

		/// <returns> The "Raw" Java object, e.g. org.w3.Node for a node,
		///         java.util.String for strings, etc. </returns>
		object NativeValue {get;}

		string StringValue {get;}
	}

}