using System.Collections;
using System.Collections.Generic;

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
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.api
{

	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;

	/// <summary>
	/// Immutable representation of a result
	/// 
	/// @since 2.0
	/// </summary>
	public interface ResultSequence : IEnumerable<Item>
	{
		/// <summary>
		/// Return the size of the result set. Only call this if you need it, since it may require that the entire result
		/// is fetched.
		/// </summary>
		/// <returns> Count of items. </returns>
		int size();

		/// <summary>
		/// Return the native representation of the item.
		/// </summary>
		/// <param name="index"> </param>
		/// <returns> Native object representing the item. </returns>
		object value(int index);

		/// <summary>
		/// Return the item.
		/// </summary>
		/// <param name="index"> </param>
		/// <returns> Native object representing the item. </returns>
		Item item(int index);

		/// <summary>
		/// Return the native representation of the first item.
		/// </summary>
		/// <returns> Native object representing the first item. </returns>
		object firstValue();

		/// <summary>
		/// XPath2 type definition description of the item at location '0'
		/// </summary>
		/// <param name="index">
		/// @return </param>
		ItemType itemType(int index);

		/// <summary>
		/// Is the sequence empty.
		/// </summary>
		/// <returns> true for empty sequences </returns>
		bool empty();

		/// <summary>
		/// Iterator of Item elements 
		/// 
		/// @return
		/// </summary>
		IEnumerator<Item> iterator();

		/// 
		/// 
		/// <summary>
		/// @return
		/// </summary>
		Item first();

		/// <summary>
		/// Describe the whole sequence's type.
		/// </summary>
		/// <returns> Item type definition. </returns>
		ItemType sequenceType();
    }

}