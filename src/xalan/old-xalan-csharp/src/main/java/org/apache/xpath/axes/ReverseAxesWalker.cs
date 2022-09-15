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
 * $Id: ReverseAxesWalker.java 513117 2007-03-01 03:28:52Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;

	/// <summary>
	/// Walker for a reverse axes. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xpath#predicates">XPath 2.4 Predicates</a> </seealso>
	[Serializable]
	public class ReverseAxesWalker : AxesWalker
	{
		internal new const long serialVersionUID = 2847007647832768941L;

	  /// <summary>
	  /// Construct an AxesWalker using a LocPathIterator.
	  /// </summary>
	  /// <param name="locPathIterator"> The location path iterator that 'owns' this walker. </param>
	  internal ReverseAxesWalker(LocPathIterator locPathIterator, int axis) : base(locPathIterator, axis)
	  {
	  }

	  /// <summary>
	  /// Set the root node of the TreeWalker.
	  /// (Not part of the DOM2 TreeWalker interface).
	  /// </summary>
	  /// <param name="root"> The context node of this step. </param>
	  public override int Root
	  {
		  set
		  {
			base.Root = value;
			m_iterator = getDTM(value).getAxisIterator(m_axis);
			m_iterator.StartNode = value;
		  }
	  }

	  /// <summary>
	  /// Detaches the walker from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state.
	  /// </summary>
	  public override void detach()
	  {
		m_iterator = null;
		base.detach();
	  }

	  /// <summary>
	  /// Get the next node in document order on the axes.
	  /// </summary>
	  /// <returns> the next node in document order on the axes, or null. </returns>
	  protected internal override int NextNode
	  {
		  get
		  {
			if (m_foundLast)
			{
			  return org.apache.xml.dtm.DTM_Fields.NULL;
			}
    
			int next = m_iterator.next();
    
			if (m_isFresh)
			{
			  m_isFresh = false;
			}
    
			if (org.apache.xml.dtm.DTM_Fields.NULL == next)
			{
			  this.m_foundLast = true;
			}
    
			return next;
		  }
	  }


	  /// <summary>
	  /// Tells if this is a reverse axes.  Overrides AxesWalker#isReverseAxes.
	  /// </summary>
	  /// <returns> true for this class. </returns>
	  public override bool ReverseAxes
	  {
		  get
		  {
			return true;
		  }
	  }

	//  /**
	//   *  Set the root node of the TreeWalker.
	//   *
	//   * @param root The context node of this step.
	//   */
	//  public void setRoot(int root)
	//  {
	//    super.setRoot(root);
	//  }

	  /// <summary>
	  /// Get the current sub-context position.  In order to do the
	  /// reverse axes count, for the moment this re-searches the axes
	  /// up to the predicate.  An optimization on this is to cache
	  /// the nodes searched, but, for the moment, this case is probably
	  /// rare enough that the added complexity isn't worth it.
	  /// </summary>
	  /// <param name="predicateIndex"> The predicate index of the proximity position.
	  /// </param>
	  /// <returns> The pridicate index, or -1. </returns>
	  protected internal override int getProximityPosition(int predicateIndex)
	  {
		// A negative predicate index seems to occur with
		// (preceding-sibling::*|following-sibling::*)/ancestor::*[position()]/*[position()]
		// -sb
		if (predicateIndex < 0)
		{
		  return -1;
		}

		int count = m_proximityPositions[predicateIndex];

		if (count <= 0)
		{
		  AxesWalker savedWalker = wi().LastUsedWalker;

		  try
		  {
			ReverseAxesWalker clone = (ReverseAxesWalker) this.clone();

			clone.Root = this.Root;

			clone.PredicateCount = predicateIndex;

			clone.PrevWalker = null;
			clone.NextWalker = null;
			wi().LastUsedWalker = clone;

			// Count 'em all
			count++;
			int next;

			while (org.apache.xml.dtm.DTM_Fields.NULL != (next = clone.nextNode()))
			{
			  count++;
			}

			m_proximityPositions[predicateIndex] = count;
		  }
		  catch (CloneNotSupportedException)
		  {

			// can't happen
		  }
		  finally
		  {
			wi().LastUsedWalker = savedWalker;
		  }
		}

		return count;
	  }

	  /// <summary>
	  /// Count backwards one proximity position.
	  /// </summary>
	  /// <param name="i"> The predicate index. </param>
	  protected internal override void countProximityPosition(int i)
	  {
		if (i < m_proximityPositions.Length)
		{
		  m_proximityPositions[i]--;
		}
	  }

	  /// <summary>
	  /// Get the number of nodes in this node list.  The function is probably ill
	  /// named?
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context.
	  /// </param>
	  /// <returns> the number of nodes in this node list. </returns>
	  public override int getLastPos(XPathContext xctxt)
	  {

		int count = 0;
		AxesWalker savedWalker = wi().LastUsedWalker;

		try
		{
		  ReverseAxesWalker clone = (ReverseAxesWalker) this.clone();

		  clone.Root = this.Root;

		  clone.PredicateCount = m_predicateIndex;

		  clone.PrevWalker = null;
		  clone.NextWalker = null;
		  wi().LastUsedWalker = clone;

		  // Count 'em all
		  // count = 1;
		  int next;

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (next = clone.nextNode()))
		  {
			count++;
		  }
		}
		catch (CloneNotSupportedException)
		{

		  // can't happen
		}
		finally
		{
		  wi().LastUsedWalker = savedWalker;
		}

		return count;
	  }

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
	  /// Warning: This can only be called after setRoot has been called!
	  /// </summary>
	  /// <returns> false. </returns>
	  public override bool DocOrdered
	  {
		  get
		  {
			return false; // I think.
		  }
	  }

	  /// <summary>
	  /// The DTM inner traversal class, that corresponds to the super axis. </summary>
	  protected internal DTMAxisIterator m_iterator;
	}

}