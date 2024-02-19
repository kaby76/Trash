using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using org.eclipse.wst.xml.xpath2.api;

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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{


	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using org.eclipse.wst.xml.xpath2.processor.@internal.types;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A range expression can be used to construct a sequence of consecutive
	/// integers.
	/// </summary>
	public class RangeResultSequence : ResultSequence
	{

		private int _start;
		private int _end;
		private int _size;
		private ResultSequence _tail;

		/// <summary>
		/// set the start and end of the range result sequence
		/// </summary>
		/// <param name="start">
		///            is the integer position of the start of range. </param>
		/// <param name="end">
		///            is the integer position of the end of range. </param>
		public RangeResultSequence(int start, int end)
		{
			_size = (end - start) + 1;

			Debug.Assert(_size >= 0);

			_start = start;
			_end = end;

			_tail = ResultSequenceFactory.create_new();
		}

		/// <summary>
		/// item is an integer to add to the range.
		/// </summary>
		/// <param name="item">
		///            is an integer. </param>
		public override void add(AnyType item)
		{
			_tail.add(item);
		}

		/// <summary>
		/// remove the tail from the range given.
		/// </summary>
		/// <param name="rs">
		///            is the range </param>
		public override void concat(ResultSequence rs)
		{
			_tail.concat(rs);
		}

		/// <summary>
		/// interate through range.
		/// </summary>
		/// <returns> tail </returns>
		public override IEnumerator<Item> iterator()
		{
			// XXX life is getting hard...
			if (_size != 0)
			{
				ResultSequence newtail = ResultSequenceFactory.create_new();

				for (; _start <= _end; _start++)
				{
					newtail.add(new XSInteger(new System.Numerics.BigInteger(_start)));
				}

				newtail.concat(_tail);
				_tail.release();
				_tail = newtail;

				_size = 0;
				_start = 0;
				_end = 0;

			}

			return _tail.iterator();
		}

		/// <returns> item from range </returns>
		public override AnyType get(int i)
		{
			if (i < _size)
			{
				return new XSInteger(new System.Numerics.BigInteger(_start + i));
			}
			else
			{
				return _tail.get(i - _size);
			}
		}

		/// <returns> size </returns>
		public override int size()
		{
			return _size + _tail.size();
		}

		/// <summary>
		/// clear range
		/// </summary>
		public override void clear()
		{
			_size = 0;
			_tail.clear();
		}

		/// <summary>
		/// create new result sequence
		/// </summary>
		/// <returns> null </returns>
		public override ResultSequence create_new()
		{
			Debug.Assert(false);
			return null;
		}

		/// <returns> first item in range </returns>
		public override AnyType first()
		{
			return get(0);
		}

		/// <returns> first item in range </returns>
		public override object firstValue()
		{
			return get(0).NativeValue;
		}

		/// <summary>
		/// asks if the range is empty?
		/// </summary>
		/// <returns> boolean </returns>
		public override bool empty()
		{
			return size() == 0;
		}

		public override ItemType sequenceType()
		{
			return new SimpleAtomicItemTypeImpl(BuiltinTypeLibrary.XS_INTEGER, org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_NONE_OR_MANY);
		}

		/// <summary>
		/// release
		/// </summary>
		public override void release()
		{
		}
	}

}