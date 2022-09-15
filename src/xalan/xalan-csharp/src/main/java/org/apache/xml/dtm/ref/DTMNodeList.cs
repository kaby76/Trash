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
 * $Id: DTMNodeList.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// <code>DTMNodeList</code> gives us an implementation of the DOM's
	/// NodeList interface wrapped around a DTM Iterator. The author
	/// considers this something of an abominations, since NodeList was not
	/// intended to be a general purpose "list of nodes" API and is
	/// generally considered by the DOM WG to have be a mistake... but I'm
	/// told that some of the XPath/XSLT folks say they must have this
	/// solution.
	/// 
	/// Please note that this is not necessarily equivlaent to a DOM
	/// NodeList operating over the same document. In particular:
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
	/// promise to implement the DOM NodeList's "live view" response to
	/// document mutation. </li>
	/// 
	/// </ul>
	/// 
	/// <para>State: In progress!!</para>
	/// 
	/// </summary>
	public class DTMNodeList : DTMNodeListBase
	{
		private DTMIterator m_iter;

		//================================================================
		// Methods unique to this class
		private DTMNodeList()
		{
		}

		/// <summary>
		/// Public constructor: Wrap a DTMNodeList around an existing
		/// and preconfigured DTMIterator
		/// 
		/// WARNING: THIS HAS THE SIDE EFFECT OF ISSUING setShouldCacheNodes(true)
		/// AGAINST THE DTMIterator.
		/// 
		/// </summary>
		public DTMNodeList(DTMIterator dtmIterator)
		{
			if (dtmIterator != null)
			{
				int pos = dtmIterator.CurrentPos;
				try
				{
					m_iter = (DTMIterator)dtmIterator.cloneWithReset();
				}
				catch (CloneNotSupportedException)
				{
					m_iter = dtmIterator;
				}
				m_iter.ShouldCacheNodes = true;
				m_iter.runTo(-1);
				m_iter.CurrentPos = pos;
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
				return m_iter;
			}
		}

		//================================================================
		// org.w3c.dom.NodeList API follows

		/// <summary>
		/// Returns the <code>index</code>th item in the collection. If 
		/// <code>index</code> is greater than or equal to the number of nodes in 
		/// the list, this returns <code>null</code>. </summary>
		/// <param name="index"> Index into the collection. </param>
		/// <returns> The node at the <code>index</code>th position in the 
		///   <code>NodeList</code>, or <code>null</code> if that is not a valid 
		///   index. </returns>
		public override Node item(int index)
		{
			if (m_iter != null)
			{
				int handle = m_iter.item(index);
				if (handle == DTM.NULL)
				{
					return null;
				}
				return m_iter.getDTM(handle).getNode(handle);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// The number of nodes in the list. The range of valid child node indices 
		/// is 0 to <code>length-1</code> inclusive. 
		/// </summary>
		public override int Length
		{
			get
			{
				return (m_iter != null) ? m_iter.Length : 0;
			}
		}
	}

}