using System.Collections;

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
 * $Id: FlowList.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class FlowList
	{
		private ArrayList _elements;

		public FlowList()
		{
		_elements = null;
		}

		public FlowList(InstructionHandle bh)
		{
		_elements = new ArrayList();
		_elements.Add(bh);
		}

		public FlowList(FlowList list)
		{
		_elements = list._elements;
		}

		public FlowList add(InstructionHandle bh)
		{
		if (_elements == null)
		{
			_elements = new ArrayList();
		}
		_elements.Add(bh);
		return this;
		}

		public FlowList append(FlowList right)
		{
		if (_elements == null)
		{
			_elements = right._elements;
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector temp = right._elements;
			ArrayList temp = right._elements;
			if (temp != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = temp.size();
			int n = temp.Count;
			for (int i = 0; i < n; i++)
			{
				_elements.Add(temp[i]);
			}
			}
		}
		return this;
		}

		/// <summary>
		/// Back patch a flow list. All instruction handles must be branch handles.
		/// </summary>
		public void backPatch(InstructionHandle target)
		{
		if (_elements != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _elements.size();
			int n = _elements.Count;
			for (int i = 0; i < n; i++)
			{
			BranchHandle bh = (BranchHandle)_elements[i];
			bh.setTarget(target);
			}
			_elements.Clear(); // avoid backpatching more than once
		}
		}

		/// <summary>
		/// Redirect the handles from oldList to newList. "This" flow list
		/// is assumed to be relative to oldList.
		/// </summary>
		public FlowList copyAndRedirect(InstructionList oldList, InstructionList newList)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FlowList result = new FlowList();
		FlowList result = new FlowList();
		if (_elements == null)
		{
			return result;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _elements.size();
		int n = _elements.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator oldIter = oldList.iterator();
		System.Collections.IEnumerator oldIter = oldList.GetEnumerator();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator newIter = newList.iterator();
		System.Collections.IEnumerator newIter = newList.GetEnumerator();

		while (oldIter.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle oldIh = (org.apache.bcel.generic.InstructionHandle) oldIter.Current;
			InstructionHandle oldIh = (InstructionHandle) oldIter.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle newIh = (org.apache.bcel.generic.InstructionHandle) newIter.next();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			InstructionHandle newIh = (InstructionHandle) newIter.next();

			for (int i = 0; i < n; i++)
			{
			if (_elements[i] == oldIh)
			{
				result.add(newIh);
			}
			}
		}
		return result;
		}
	}

}