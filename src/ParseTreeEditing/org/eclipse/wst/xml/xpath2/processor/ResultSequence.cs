using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
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

namespace org.eclipse.wst.xml.xpath2.processor
{


	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Interface to the methods of range of result sequence </summary>
	/// @deprecated Use org.eclipse.wst.xml.xpath2.api.ResultSequence instead 
	public abstract class ResultSequence : api.ResultSequence
	{
		public abstract ItemType sequenceType();
		public abstract object firstValue();

		/// <summary>
		/// add item
		/// </summary>
		/// <param name="item">
		///            is an item of any type. </param>
		public abstract void add(AnyType item);

		/// <summary>
		/// concatinate from rs
		/// </summary>
		/// <param name="rs">
		///            is a Result Sequence. </param>
		public abstract void concat(ResultSequence rs);

		/// <summary>
		/// List Iterator.
		/// </summary>
		public abstract IEnumerator<Item> iterator();

		/// <summary>
		/// get item in index i
		/// </summary>
		/// <param name="i">
		///            is the position. </param>
		public abstract AnyType get(int i);

		/// <summary>
		/// get the size
		/// </summary>
		/// <returns> the size. </returns>
		public abstract int size();

		/// <summary>
		/// clear
		/// </summary>
		public abstract void clear();

		/// <summary>
		/// create a new result sequence
		/// </summary>
		/// <returns> a new result sequence. </returns>
		public abstract ResultSequence create_new();

		/// <summary>
		/// retrieve the first item
		/// </summary>
		/// <returns> the first item. </returns>
		public virtual AnyType first()
		{
			return get(0);
		}

		/// <summary>
		/// check is the sequence is empty
		/// </summary>
		/// <returns> boolean. </returns>
		public virtual bool empty()
		{
			if (size() == 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// retrieve items in sequence
		/// </summary>
		/// <returns> result string </returns>
		public virtual string @string()
		{
			string result = "";
			int num = 1;

			StringBuilder buf = new StringBuilder();
			for (IEnumerator i = iterator(); i.MoveNext();)
			{
				AnyType elem = (AnyType) i.Current;

				buf.Append(num + ") ");

				buf.Append(elem.string_type() + ": ");

				string value = elem.StringValue;

				if (elem is NodeType)
				{
					QName tmp = ((NodeType) elem).node_name();

					if (tmp != null)
					{
						value = tmp.expanded_name();
					}
				}
				buf.Append(value + "\n");

				num++;
			}
			result = buf.ToString();
			if (num == 1)
			{
				result = "Empty results\n";
			}
			return result;
		}

        internal IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// release the result sequence
        /// </summary>
        public virtual void release()
		{
			ResultSequenceFactory.release(this);
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual Item item(int index)
		{
			return get(index);
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual ItemType itemType(int index)
		{
			return get(index).ItemType;
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual object value(int index)
		{
			return get(index).NativeValue;
		}


        Item api.ResultSequence.first()
        {
            return get(0);
        }

        IEnumerator<Item> IEnumerable<Item>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}