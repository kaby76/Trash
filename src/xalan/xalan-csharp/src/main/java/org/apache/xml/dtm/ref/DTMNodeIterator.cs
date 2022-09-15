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
 * $Id: DTMNodeIterator.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMDOMException = org.apache.xml.dtm.DTMDOMException;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;

	using DOMException = org.w3c.dom.DOMException;
	using Node = org.w3c.dom.Node;
	using NodeFilter = org.w3c.dom.traversal.NodeFilter;

	/// <summary>
	/// <code>DTMNodeIterator</code> gives us an implementation of the 
	/// DTMNodeIterator which returns DOM nodes.
	/// 
	/// Please note that this is not necessarily equivlaent to a DOM
	/// NodeIterator operating over the same document. In particular:
	/// <ul>
	/// 
	/// <li>If there are several Text nodes in logical succession (ie,
	/// across CDATASection and EntityReference boundaries), we will return
	/// only the first; the caller is responsible for stepping through
	/// them.
	/// (%REVIEW% Provide a convenience routine here to assist, pending
	/// proposed DOM Level 3 getAdjacentText() operation?) </li>
	/// 
	/// <li>Since the whole XPath/XSLT architecture assumes that the source
	/// document is not altered while we're working with it, we do not
	/// promise to implement the DOM NodeIterator's "maintain current
	/// position" response to document mutation. </li>
	/// 
	/// <li>Since our design for XPath NodeIterators builds a stateful
	/// filter directly into the traversal object, getNodeFilter() is not
	/// supported.</li>
	/// 
	/// </ul>
	/// 
	/// <para>State: In progress!!</para>
	/// 
	/// </summary>
	public class DTMNodeIterator : org.w3c.dom.traversal.NodeIterator
	{
	  private DTMIterator dtm_iter;
	  private bool valid = true;

	  //================================================================
	  // Methods unique to this class

	  /// <summary>
	  /// Public constructor: Wrap a DTMNodeIterator around an existing
	  /// and preconfigured DTMIterator
	  /// 
	  /// </summary>
	  public DTMNodeIterator(DTMIterator dtmIterator)
	  {
		  try
		  {
			dtm_iter = (DTMIterator)dtmIterator.clone();
		  }
		  catch (CloneNotSupportedException cnse)
		  {
			throw new org.apache.xml.utils.WrappedRuntimeException(cnse);
		  }
	  }

	  /// <summary>
	  /// Access the wrapped DTMIterator. I'm not sure whether anyone will
	  /// need this or not, but let's write it and think about it.
	  /// 
	  /// </summary>
	  public virtual DTMIterator DTMIterator
	  {
		  get
		  {
			  return dtm_iter;
		  }
	  }


	  //================================================================
	  // org.w3c.dom.traversal.NodeFilter API follows

	  /// <summary>
	  /// Detaches the NodeIterator from the set which it iterated over,
	  /// releasing any computational resources and placing the iterator in
	  /// the INVALID state.
	  /// 
	  /// </summary>
	  public virtual void detach()
	  {
		  // Theoretically, we could release dtm_iter at this point. But
		  // some of the operations may still want to consult it even though
		  // navigation is now invalid.
		  valid = false;
	  }

	  /// <summary>
	  /// The value of this flag determines whether the children
	  /// of entity reference nodes are visible to the iterator.
	  /// </summary>
	  /// <returns> false, always (the DTM model flattens entity references)
	  ///  </returns>
	  public virtual bool ExpandEntityReferences
	  {
		  get
		  {
			  return false;
		  }
	  }

	  /// <summary>
	  /// Return a handle to the filter used to screen nodes.
	  /// 
	  /// This is ill-defined in Xalan's usage of Nodeiterator, where we have
	  /// built stateful XPath-based filtering directly into the traversal
	  /// object. We could return something which supports the NodeFilter interface
	  /// and allows querying whether a given node would be permitted if it appeared
	  /// as our next node, but in the current implementation that would be very
	  /// complex -- and just isn't all that useful.
	  /// </summary>
	  /// <exception cref="DOMException"> -- NOT_SUPPORTED_ERROR because I can't think
	  /// of anything more useful to do in this case
	  ///  </exception>
	  public virtual NodeFilter Filter
	  {
		  get
		  {
			  throw new DTMDOMException(DOMException.NOT_SUPPORTED_ERR);
		  }
	  }


	  /// <returns> The root node of the NodeIterator, as specified
	  /// when it was created.
	  ///  </returns>
	  public virtual Node Root
	  {
		  get
		  {
			  int handle = dtm_iter.Root;
			  return dtm_iter.getDTM(handle).getNode(handle);
		  }
	  }


	  /// <summary>
	  /// Return a mask describing which node types are presented via the
	  /// iterator.
	  /// 
	  /// </summary>
	  public virtual int WhatToShow
	  {
		  get
		  {
			  return dtm_iter.WhatToShow;
		  }
	  }

	  /// <returns> the next node in the set and advance the position of the
	  /// iterator in the set.
	  /// </returns>
	  /// <exception cref="DOMException"> - INVALID_STATE_ERR Raised if this method is
	  /// called after the detach method was invoked.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node nextNode() throws org.w3c.dom.DOMException
	  public virtual Node nextNode()
	  {
		  if (!valid)
		  {
			throw new DTMDOMException(DOMException.INVALID_STATE_ERR);
		  }

		  int handle = dtm_iter.nextNode();
		  if (handle == DTM.NULL)
		  {
			return null;
		  }
		  return dtm_iter.getDTM(handle).getNode(handle);
	  }


	  /// <returns> the next previous in the set and advance the position of the
	  /// iterator in the set.
	  /// </returns>
	  /// <exception cref="DOMException"> - INVALID_STATE_ERR Raised if this method is
	  /// called after the detach method was invoked.
	  ///  </exception>
	  public virtual Node previousNode()
	  {
		  if (!valid)
		  {
			throw new DTMDOMException(DOMException.INVALID_STATE_ERR);
		  }

		  int handle = dtm_iter.previousNode();
		  if (handle == DTM.NULL)
		  {
			return null;
		  }
		  return dtm_iter.getDTM(handle).getNode(handle);
	  }
	}

}