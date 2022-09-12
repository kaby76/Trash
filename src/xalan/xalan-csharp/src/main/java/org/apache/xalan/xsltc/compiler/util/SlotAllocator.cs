using System;

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: SlotAllocator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using Type = org.apache.bcel.generic.Type;

	/// <summary>
	/// @author Jacek Ambroziak
	/// </summary>
	internal sealed class SlotAllocator
	{
		private bool InstanceFieldsInitialized = false;

		public SlotAllocator()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			_slotsTaken = new int[_size];
		}


		private int _firstAvailableSlot;
		private int _size = 8;
		private int _free = 0;
		private int[] _slotsTaken;

		public void initialize(LocalVariableGen[] vars)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = vars.length;
		int length = vars.Length;
		int slot = 0, size , index ;

		for (int i = 0; i < length; i++)
		{
			size = vars[i].Type.Size;
			index = vars[i].Index;
			slot = Math.Max(slot, index + size);
		}
		_firstAvailableSlot = slot;
		}

		public int allocateSlot(Type type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = type.getSize();
		int size = type.Size;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int limit = _free;
		int limit = _free;
		int slot = _firstAvailableSlot, where = 0;

		if (_free + size > _size)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] array = new int[_size *= 2];
			int[] array = new int[_size *= 2];
			for (int j = 0; j < limit; j++)
			{
			array[j] = _slotsTaken[j];
			}
			_slotsTaken = array;
		}

		while (where < limit)
		{
			if (slot + size <= _slotsTaken[where])
			{
			// insert
			for (int j = limit - 1; j >= where; j--)
			{
				_slotsTaken[j + size] = _slotsTaken[j];
			}
			break;
			}
			else
			{
			slot = _slotsTaken[where++] + 1;
			}
		}

		for (int j = 0; j < size; j++)
		{
			_slotsTaken[where + j] = slot + j;
		}

		_free += size;
		return slot;
		}

		public void releaseSlot(LocalVariableGen lvg)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = lvg.getType().getSize();
		int size = lvg.Type.Size;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int slot = lvg.getIndex();
		int slot = lvg.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int limit = _free;
		int limit = _free;

		for (int i = 0; i < limit; i++)
		{
			if (_slotsTaken[i] == slot)
			{
			int j = i + size;
			while (j < limit)
			{
				_slotsTaken[i++] = _slotsTaken[j++];
			}
			_free -= size;
			return;
			}
		}
		string state = "Variable slot allocation error" + "(size=" + size + ", slot=" + slot + ", limit=" + limit + ")";
		ErrorMsg err = new ErrorMsg(ErrorMsg.INTERNAL_ERR, state);
		throw new Exception(err.ToString());
		}
	}

}