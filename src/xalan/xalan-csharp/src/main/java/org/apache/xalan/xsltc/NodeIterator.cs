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
 * $Id: NodeIterator.java 468648 2006-10-28 07:00:06Z minchau $
 */

namespace org.apache.xalan.xsltc
{

	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface NodeIterator : ICloneable
	{

		/// <summary>
		/// Callers should not call next() after it returns END.
		/// </summary>
		int next();

		/// <summary>
		/// Resets the iterator to the last start node.
		/// </summary>
		NodeIterator reset();

		/// <summary>
		/// Returns the number of elements in this iterator.
		/// </summary>
		int Last {get;}

		/// <summary>
		/// Returns the position of the current node in the set.
		/// </summary>
		int Position {get;}

		/// <summary>
		/// Remembers the current node for the next call to gotoMark().
		/// </summary>
		void setMark();

		/// <summary>
		/// Restores the current node remembered by setMark().
		/// </summary>
		void gotoMark();

		/// <summary>
		/// Set start to END should 'close' the iterator, 
		/// i.e. subsequent call to next() should return END.
		/// </summary>
		NodeIterator setStartNode(int node);

		/// <summary>
		/// True if this iterator has a reversed axis.
		/// </summary>
		bool Reverse {get;}

		/// <summary>
		/// Returns a deep copy of this iterator.
		/// </summary>
		NodeIterator cloneIterator();

		/// <summary>
		/// Prevents or allows iterator restarts.
		/// </summary>
		bool Restartable {set;}

	}

	public static class NodeIterator_Fields
	{
		public const int END = org.apache.xml.dtm.DTM_Fields.NULL;
	}

}