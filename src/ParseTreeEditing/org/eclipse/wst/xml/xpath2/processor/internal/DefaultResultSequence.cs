using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using org.eclipse.wst.xml.xpath2.api;
using System;

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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{


	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using SimpleAtomicItemTypeImpl = org.eclipse.wst.xml.xpath2.processor.@internal.types.SimpleAtomicItemTypeImpl;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// Default implementation of a result sequence. </summary>
	/// @deprecated use ResultBuffer instead 
	public class DefaultResultSequence : ResultSequence, IEnumerable<Item>
	{

		private IList<Item>_seq;

		/// <summary>
		/// Constructor.
		/// 
		/// an empty array is created
		/// </summary>
		public DefaultResultSequence()
		{
			_seq = new List<Item>();
		}

		/// <param name="item">
		///            is added </param>
		public DefaultResultSequence(AnyType item) : this()
		{
			add(item);
		}

		/// <param name="item">
		///            is added to array _seq </param>
		public override void add(AnyType item)
		{
			Debug.Assert(item != null);
			_seq.Add(item);
		}

		/// <param name="rs">
		///            ResultSequence </param>
		public override void concat(ResultSequence rs)
		{
			for (var i = rs.iterator(); i.MoveNext();)
			{
				_seq.Add(i.Current);
			}
		}

		/// <returns> the next iteration of array _seq </returns>
		public override IEnumerator<Item> iterator()
        {
            return _seq.GetEnumerator();
        }

		/// <returns> integer of the size of array _seq </returns>
		public override int size()
		{
			return _seq.Count;
		}

		/// <param name="i">
		///            is the position of the array item that is wanted. </param>
		/// <returns> item i from array _seq </returns>
		public override AnyType get(int i)
		{
			return (AnyType) _seq[i];
		}

		/// <returns> first item from array _seq </returns>
		public override AnyType first()
		{
			if (_seq.Count == 0)
			{
				return null;
			}

			return get(0);
		}

		/// <returns> first item from array _seq </returns>
		public override object firstValue()
		{
			return get(0).NativeValue;
		}

		/// <summary>
		/// Whether or not array _seq is empty
		/// </summary>
		/// <returns> a boolean </returns>
		public override bool empty()
		{
			return _seq.Count == 0;
		}

		public override ItemType sequenceType()
		{
			return new SimpleAtomicItemTypeImpl(BuiltinTypeLibrary.XS_ANYTYPE, org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_NONE_OR_MANY);
		}

		/// <summary>
		/// Clears the sequence.
		/// </summary>
		public override void clear()
		{
			_seq.Clear();
		}

		/// <summary>
		/// Create a new sequence.
		/// </summary>
		/// <returns> The new sequence. </returns>
		public override ResultSequence create_new()
		{
			return new DefaultResultSequence();
		}


		internal new IEnumerator GetEnumerator()
		{
			return _seq.GetEnumerator();
		}

		IEnumerator<Item> IEnumerable<Item>.GetEnumerator()
		{
			return _seq.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _seq.GetEnumerator();
		}

	}

}