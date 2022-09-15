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
 * $Id: OneStepIterator.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Expression = org.apache.xpath.Expression;
	using XPathContext = org.apache.xpath.XPathContext;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpMap = org.apache.xpath.compiler.OpMap;

	/// <summary>
	/// This class implements a general iterator for
	/// those LocationSteps with only one step, and perhaps a predicate. </summary>
	/// <seealso cref="org.apache.xpath.axes.LocPathIterator"
	/// @xsl.usage advanced/>
	[Serializable]
	public class OneStepIterator : ChildTestIterator
	{
		internal new const long serialVersionUID = 4623710779664998283L;
	  /// <summary>
	  /// The traversal axis from where the nodes will be filtered. </summary>
	  protected internal int m_axis = -1;

	  /// <summary>
	  /// The DTM inner traversal class, that corresponds to the super axis. </summary>
	  protected internal DTMAxisIterator m_iterator;

	  /// <summary>
	  /// Create a OneStepIterator object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: OneStepIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal OneStepIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis)
	  {
		int firstStepPos = OpMap.getFirstChildPos(opPos);

		m_axis = WalkerFactory.getAxisFromStep(compiler, firstStepPos);

	  }


	  /// <summary>
	  /// Create a OneStepIterator object.
	  /// </summary>
	  /// <param name="iterator"> The DTM iterator which this iterator will use. </param>
	  /// <param name="axis"> One of Axis.Child, etc., or -1 if the axis is unknown.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public OneStepIterator(org.apache.xml.dtm.DTMAxisIterator iterator, int axis) throws javax.xml.transform.TransformerException
	  public OneStepIterator(DTMAxisIterator iterator, int axis) : base(null)
	  {

		m_iterator = iterator;
		m_axis = axis;
		int whatToShow = DTMFilter.SHOW_ALL;
		initNodeTest(whatToShow);
	  }

	  /// <summary>
	  /// Initialize the context values for this expression
	  /// after it is cloned.
	  /// </summary>
	  /// <param name="context"> The XPath runtime context for this
	  /// transformation. </param>
	  public override void setRoot(int context, object environment)
	  {
		base.setRoot(context, environment);
		if (m_axis > -1)
		{
		  m_iterator = m_cdtm.getAxisIterator(m_axis);
		}
		m_iterator.StartNode = m_context;
	  }

	  /// <summary>
	  ///  Detaches the iterator from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state. After<code>detach</code> has been invoked, calls to
	  /// <code>nextNode</code> or<code>previousNode</code> will raise the
	  /// exception INVALID_STATE_ERR.
	  /// </summary>
	  public override void detach()
	  {
		if (m_allowDetach)
		{
		  if (m_axis > -1)
		  {
			m_iterator = null;
		  }

		  // Always call the superclass detach last!
		  base.detach();
		}
	  }

	  /// <summary>
	  /// Get the next node via getFirstAttribute && getNextAttribute.
	  /// </summary>
	  protected internal override int NextNode
	  {
		  get
		  {
			return m_lastFetched = m_iterator.next();
		  }
	  }

	  /// <summary>
	  /// Get a cloned iterator.
	  /// </summary>
	  /// <returns> A new iterator that can be used without mutating this one.
	  /// </returns>
	  /// <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
	  public override object clone()
	  {
		// Do not access the location path itterator during this operation!

		OneStepIterator clone = (OneStepIterator) base.clone();

		if (m_iterator != null)
		{
		  clone.m_iterator = m_iterator.cloneIterator();
		}
		return clone;
	  }

	  /// <summary>
	  ///  Get a cloned Iterator that is reset to the beginning
	  ///  of the query.
	  /// </summary>
	  ///  <returns> A cloned NodeIterator set of the start of the query.
	  /// </returns>
	  ///  <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public override DTMIterator cloneWithReset()
	  {

		OneStepIterator clone = (OneStepIterator) base.cloneWithReset();
		clone.m_iterator = m_iterator;

		return clone;
	  }



	  /// <summary>
	  /// Tells if this is a reverse axes.  Overrides AxesWalker#isReverseAxes.
	  /// </summary>
	  /// <returns> true for this class. </returns>
	  public override bool ReverseAxes
	  {
		  get
		  {
			return m_iterator.Reverse;
		  }
	  }

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
		if (!ReverseAxes)
		{
		  return base.getProximityPosition(predicateIndex);
		}

		// A negative predicate index seems to occur with
		// (preceding-sibling::*|following-sibling::*)/ancestor::*[position()]/*[position()]
		// -sb
		if (predicateIndex < 0)
		{
		  return -1;
		}

		if (m_proximityPositions[predicateIndex] <= 0)
		{
		  XPathContext xctxt = XPathContext;
		  try
		  {
			OneStepIterator clone = (OneStepIterator) this.clone();

			int root = Root;
			xctxt.pushCurrentNode(root);
			clone.setRoot(root, xctxt);

			// clone.setPredicateCount(predicateIndex);
			clone.m_predCount = predicateIndex;

			// Count 'em all
			int count = 1;
			int next;

			while (DTM.NULL != (next = clone.nextNode()))
			{
			  count++;
			}

			m_proximityPositions[predicateIndex] += count;
		  }
		  catch (CloneNotSupportedException)
		  {

			// can't happen
		  }
		  finally
		  {
			xctxt.popCurrentNode();
		  }
		}

		return m_proximityPositions[predicateIndex];
	  }

	  /// <summary>
	  ///  The number of nodes in the list. The range of valid child node indices
	  /// is 0 to <code>length-1</code> inclusive.
	  /// </summary>
	  /// <returns> The number of nodes in the list, always greater or equal to zero. </returns>
	  public override int Length
	  {
		  get
		  {
			if (!ReverseAxes)
			{
			  return base.Length;
			}
    
			// Tell if this is being called from within a predicate.
			bool isPredicateTest = (this == m_execContext.SubContextList);
    
			// And get how many total predicates are part of this step.
			int predCount = PredicateCount;
    
			// If we have already calculated the length, and the current predicate 
			// is the first predicate, then return the length.  We don't cache 
			// the anything but the length of the list to the first predicate.
			if (-1 != m_length && isPredicateTest && m_predicateIndex < 1)
			{
			   return m_length;
			}
    
			int count = 0;
    
			XPathContext xctxt = XPathContext;
			try
			{
			  OneStepIterator clone = (OneStepIterator) this.cloneWithReset();
    
			  int root = Root;
			  xctxt.pushCurrentNode(root);
			  clone.setRoot(root, xctxt);
    
			  clone.m_predCount = m_predicateIndex;
    
			  int next;
    
			  while (DTM.NULL != (next = clone.nextNode()))
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
			  xctxt.popCurrentNode();
			}
			if (isPredicateTest && m_predicateIndex < 1)
			{
			  m_length = count;
			}
    
			return count;
		  }
	  }

	  /// <summary>
	  /// Count backwards one proximity position.
	  /// </summary>
	  /// <param name="i"> The predicate index. </param>
	  protected internal override void countProximityPosition(int i)
	  {
		if (!ReverseAxes)
		{
		  base.countProximityPosition(i);
		}
		else if (i < m_proximityPositions.Length)
		{
		  m_proximityPositions[i]--;
		}
	  }

	  /// <summary>
	  /// Reset the iterator.
	  /// </summary>
	  public override void reset()
	  {

		base.reset();
		if (null != m_iterator)
		{
		  m_iterator.reset();
		}
	  }

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public override int Axis
	  {
		  get
		  {
			return m_axis;
		  }
	  }

	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!base.deepEquals(expr))
		  {
			  return false;
		  }

		  if (m_axis != ((OneStepIterator)expr).m_axis)
		  {
			  return false;
		  }

		  return true;
	  }


	}

}