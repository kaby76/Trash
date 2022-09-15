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
 * $Id: ContextNodeList.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using Node = org.w3c.dom.Node;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// Classes who implement this interface can be a
	/// <a href="http://www.w3.org/TR/xslt#dt-current-node-list">current node list</a>,
	/// also refered to here as a <term>context node list</term>.
	/// @xsl.usage advanced
	/// </summary>
	public interface ContextNodeList
	{

	  /// <summary>
	  /// Get the <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a>.
	  /// 
	  /// </summary>
	  /// <returns> The current node, or null. </returns>
	  Node CurrentNode {get;}

	  /// <summary>
	  /// Get the current position, which is one less than
	  /// the next nextNode() call will retrieve.  i.e. if
	  /// you call getCurrentPos() and the return is 0, the next
	  /// fetch will take place at index 1.
	  /// </summary>
	  /// <returns> The position of the
	  /// <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a>
	  /// in the  <a href="http://www.w3.org/TR/xslt#dt-current-node-list">current node list</a>. </returns>
	  int CurrentPos {get;set;}

	  /// <summary>
	  /// Reset the iterator.
	  /// </summary>
	  void reset();

	  /// <summary>
	  /// If setShouldCacheNodes(true) is called, then nodes will
	  /// be cached.  They are not cached by default.
	  /// </summary>
	  /// <param name="b"> true if the nodes should be cached. </param>
	  bool ShouldCacheNodes {set;}

	  /// <summary>
	  /// If an index is requested, NodeSetDTM will call this method
	  /// to run the iterator to the index.  By default this sets
	  /// m_next to the index.  If the index argument is -1, this
	  /// signals that the iterator should be run to the end.
	  /// </summary>
	  /// <param name="index"> The index to run to, or -1 if the iterator should be run
	  ///              to the end. </param>
	  void runTo(int index);


	  /// <summary>
	  /// Get the length of the list.
	  /// </summary>
	  /// <returns> The number of nodes in this node list. </returns>
	  int size();

	  /// <summary>
	  /// Tells if this NodeSetDTM is "fresh", in other words, if
	  /// the first nextNode() that is called will return the
	  /// first node in the set.
	  /// </summary>
	  /// <returns> true if the iteration of this list has not yet begun. </returns>
	  bool Fresh {get;}

	  /// <summary>
	  /// Get a cloned Iterator that is reset to the start of the iteration.
	  /// </summary>
	  /// <returns> A clone of this iteration that has been reset.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.traversal.NodeIterator cloneWithReset() throws CloneNotSupportedException;
	  NodeIterator cloneWithReset();

	  /// <summary>
	  /// Get a clone of this iterator.  Be aware that this operation may be
	  /// somewhat expensive.
	  /// 
	  /// </summary>
	  /// <returns> A clone of this object.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException;
	  object clone();

	  /// <summary>
	  /// Get the index of the last node in this list.
	  /// 
	  /// </summary>
	  /// <returns> the index of the last node in this list. </returns>
	  int Last {get;set;}

	}

}