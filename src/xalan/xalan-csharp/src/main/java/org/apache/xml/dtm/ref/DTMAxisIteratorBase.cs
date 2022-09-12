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
 * $Id: DTMAxisIteratorBase.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{

	/// <summary>
	/// This class serves as a default base for implementations of mutable
	/// DTMAxisIterators.
	/// </summary>
	public abstract class DTMAxisIteratorBase : DTMAxisIterator
	{
		public abstract DTMAxisIterator setStartNode(int node);
		public abstract void gotoMark();
		public abstract void setMark();
		public abstract int next();

	  /// <summary>
	  /// The position of the last node within the iteration, as defined by XPath.
	  /// Note that this is _not_ the node's handle within the DTM. Also, don't
	  /// confuse it with the current (most recently returned) position.
	  /// </summary>
	  protected internal int _last = -1;

	  /// <summary>
	  /// The position of the current node within the iteration, as defined by XPath.
	  /// Note that this is _not_ the node's handle within the DTM!
	  /// </summary>
	  protected internal int _position = 0;

	  /// <summary>
	  /// The position of the marked node within the iteration;
	  /// a saved itaration state that we may want to come back to.
	  /// Note that only one mark is maintained; there is no stack.
	  /// </summary>
	  protected internal int _markedNode;

	  /// <summary>
	  /// The handle to the start, or root, of the iteration.
	  /// Set this to END to construct an empty iterator.
	  /// </summary>
	  protected internal int _startNode = org.apache.xml.dtm.DTMAxisIterator_Fields.END;

	  /// <summary>
	  /// True if the start node should be considered part of the iteration.
	  /// False will cause it to be skipped.
	  /// </summary>
	  protected internal bool _includeSelf = false;

	  /// <summary>
	  /// True if this iteration can be restarted. False otherwise (eg, if
	  /// we are iterating over a stream that can not be re-scanned, or if
	  /// the iterator was produced by cloning another iterator.)
	  /// </summary>
	  protected internal bool _isRestartable = true;

	  /// <summary>
	  /// Get start to END should 'close' the iterator,
	  /// i.e. subsequent call to next() should return END.
	  /// </summary>
	  /// <returns> The root node of the iteration. </returns>
	  public virtual int StartNode
	  {
		  get
		  {
			return _startNode;
		  }
	  }

	  /// <returns> A DTMAxisIterator which has been reset to the start node,
	  /// which may or may not be the same as this iterator.
	  ///  </returns>
	  public virtual DTMAxisIterator reset()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _isRestartable;
		bool temp = _isRestartable;

		_isRestartable = true;

		StartNode = _startNode;

		_isRestartable = temp;

		return this;
	  }

	  /// <summary>
	  /// Set the flag to include the start node in the iteration. 
	  /// 
	  /// </summary>
	  /// <returns> This default method returns just returns this DTMAxisIterator,
	  /// after setting the flag.
	  /// (Returning "this" permits C++-style chaining of
	  /// method calls into a single expression.) </returns>
	  public virtual DTMAxisIterator includeSelf()
	  {

		_includeSelf = true;

		return this;
	  }

	  /// <summary>
	  /// Returns the position of the last node within the iteration, as
	  /// defined by XPath.  In a forward iterator, I believe this equals the number of nodes which this
	  /// iterator will yield. In a reverse iterator, I believe it should return
	  /// 1 (since the "last" is the first produced.)
	  /// 
	  /// This may be an expensive operation when called the first time, since
	  /// it may have to iterate through a large part of the document to produce
	  /// its answer.
	  /// </summary>
	  /// <returns> The number of nodes in this iterator (forward) or 1 (reverse). </returns>
	  public virtual int Last
	  {
		  get
		  {
    
			if (_last == -1) // Not previously established
			{
			  // Note that we're doing both setMark() -- which saves _currentChild
			  // -- and explicitly saving our position counter (number of nodes
			  // yielded so far).
			  //
			  // %REVIEW% Should position also be saved by setMark()?
			  // (It wasn't in the XSLTC version, but I don't understand why not.)
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int temp = _position;
			  int temp = _position; // Save state
			  setMark();
    
			  reset(); // Count the nodes found by this iterator
			  do
			  {
				_last++;
			  } while (next() != org.apache.xml.dtm.DTMAxisIterator_Fields.END);
    
			  gotoMark(); // Restore saved state
			  _position = temp;
			}
    
			return _last;
		  }
	  }

	  /// <returns> The position of the current node within the set, as defined by
	  /// XPath. Note that this is one-based, not zero-based. </returns>
	  public virtual int Position
	  {
		  get
		  {
			return _position == 0 ? 1 : _position;
		  }
	  }

	  /// <returns> true if this iterator has a reversed axis, else false </returns>
	  public virtual bool Reverse
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Returns a deep copy of this iterator. Cloned iterators may not be
	  /// restartable. The iterator being cloned may or may not become
	  /// non-restartable as a side effect of this operation.
	  /// </summary>
	  /// <returns> a deep copy of this iterator. </returns>
	  public virtual DTMAxisIterator cloneIterator()
	  {

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DTMAxisIteratorBase clone = (DTMAxisIteratorBase) super.clone();
		  DTMAxisIteratorBase clone = (DTMAxisIteratorBase) base.clone();

		  clone._isRestartable = false;

		  // return clone.reset();
		  return clone;
		}
		catch (CloneNotSupportedException e)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(e);
		}
	  }

	  /// <summary>
	  /// Do any final cleanup that is required before returning the node that was
	  /// passed in, and then return it. The intended use is
	  /// <br />
	  /// <code>return returnNode(node);</code>
	  /// 
	  /// %REVIEW% If we're calling it purely for side effects, should we really
	  /// be bothering with a return value? Something like
	  /// <br />
	  /// <code> accept(node); return node; </code>
	  /// <br />
	  /// would probably optimize just about as well and avoid questions
	  /// about whether what's returned could ever be different from what's
	  /// passed in.
	  /// </summary>
	  /// <param name="node"> Node handle which iteration is about to yield.
	  /// </param>
	  /// <returns> The node handle passed in.   </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected final int returnNode(final int node)
	  protected internal int returnNode(int node)
	  {
		_position++;

		return node;
	  }

	  /// <summary>
	  /// Reset the position to zero. NOTE that this does not change the iteration
	  /// state, only the position number associated with that state.
	  /// 
	  /// %REVIEW% Document when this would be used?
	  /// </summary>
	  /// <returns> This instance. </returns>
	  protected internal DTMAxisIterator resetPosition()
	  {

		_position = 0;

		return this;
	  }

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
	  /// </summary>
	  /// <returns> true as a default. </returns>
	  public virtual bool DocOrdered
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public virtual int Axis
	  {
		  get
		  {
			return -1;
		  }
	  }

	  public virtual bool Restartable
	  {
		  set
		  {
			_isRestartable = value;
		  }
	  }

	  /// <summary>
	  /// Return the node at the given position.
	  /// </summary>
	  /// <param name="position"> The position </param>
	  /// <returns> The node at the given position. </returns>
	  public virtual int getNodeByPosition(int position)
	  {
		if (position > 0)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pos = isReverse() ? getLast() - position + 1 : position;
		  int pos = Reverse ? Last - position + 1 : position;
		  int node;
		  while ((node = next()) != org.apache.xml.dtm.DTMAxisIterator_Fields.END)
		  {
			if (pos == Position)
			{
			  return node;
			}
		  }
		}
		return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
	  }

	}

}