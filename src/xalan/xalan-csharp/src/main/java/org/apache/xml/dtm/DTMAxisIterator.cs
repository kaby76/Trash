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
 * $Id: DTMAxisIterator.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{

	/// <summary>
	/// This class iterates over a single XPath Axis, and returns node handles.
	/// </summary>
	public interface DTMAxisIterator : ICloneable
	{

	  /// <summary>
	  /// Specifies the end of the iteration, and is the same as DTM.NULL. </summary>

	  /// <summary>
	  /// Get the next node in the iteration.
	  /// </summary>
	  /// <returns> The next node handle in the iteration, or END. </returns>
	  int next();


	  /// <summary>
	  /// Resets the iterator to the last start node.
	  /// </summary>
	  /// <returns> A DTMAxisIterator, which may or may not be the same as this 
	  ///         iterator. </returns>
	  DTMAxisIterator reset();

	  /// <returns> the number of nodes in this iterator.  This may be an expensive 
	  /// operation when called the first time. </returns>
	  int Last {get;}

	  /// <returns> The position of the current node in the set, as defined by XPath. </returns>
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
	  /// <param name="node"> Sets the root of the iteration.
	  /// </param>
	  /// <returns> A DTMAxisIterator set to the start of the iteration. </returns>
	  DTMAxisIterator setStartNode(int node);

	  /// <summary>
	  /// Get start to END should 'close' the iterator,
	  /// i.e. subsequent call to next() should return END.
	  /// </summary>
	  /// <returns> The root node of the iteration. </returns>
	  int StartNode {get;}

	  /// <returns> true if this iterator has a reversed axis, else false. </returns>
	  bool Reverse {get;}

	  /// <returns> a deep copy of this iterator. The clone should not be reset 
	  /// from its current position. </returns>
	  DTMAxisIterator cloneIterator();

	  /// <summary>
	  /// Set if restartable.
	  /// </summary>
	  bool Restartable {set;}

	  /// <summary>
	  /// Return the node at the given position.
	  /// </summary>
	  /// <param name="position"> The position </param>
	  /// <returns> The node at the given position. </returns>
	  int getNodeByPosition(int position);
	}

	public static class DTMAxisIterator_Fields
	{
	  public const int END = DTM_Fields.NULL;
	}

}