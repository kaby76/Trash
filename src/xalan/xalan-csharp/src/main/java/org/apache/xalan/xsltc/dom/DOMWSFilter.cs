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
 * $Id: DOMWSFilter.java 468651 2006-10-28 07:04:25Z minchau $
 */
namespace org.apache.xalan.xsltc.dom
{
	using DOM = org.apache.xalan.xsltc.DOM;
	using DOMEnhancedForDTM = org.apache.xalan.xsltc.DOMEnhancedForDTM;
	using StripFilter = org.apache.xalan.xsltc.StripFilter;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;

	/// <summary>
	/// A wrapper class that adapts the
	/// <seealso cref="org.apache.xml.dtm.DTMWSFilter DTMWSFilter"/> interface to the XSLTC
	/// DOM <seealso cref="org.apache.xalan.xsltc.StripFilter StripFilter"/> interface.
	/// </summary>
	public class DOMWSFilter : DTMWSFilter
	{

		private AbstractTranslet m_translet;
		private StripFilter m_filter;

		// The Hashtable for DTM to mapping array
		private Hashtable m_mappings;

		// Cache the DTM and mapping that are used last time
		private DTM m_currentDTM;
		private short[] m_currentMapping;

		/// <summary>
		/// Construct an adapter connecting the <code>DTMWSFilter</code> interface
		/// to the <code>StripFilter</code> interface.
		/// </summary>
		/// <param name="translet"> A translet that also implements the StripFilter
		/// interface.
		/// </param>
		/// <seealso cref="org.apache.xml.dtm.DTMWSFilter"/>
		/// <seealso cref="org.apache.xalan.xsltc.StripFilter"/>
		public DOMWSFilter(AbstractTranslet translet)
		{
			m_translet = translet;
			m_mappings = new Hashtable();

			if (translet is StripFilter)
			{
				m_filter = (StripFilter) translet;
			}
		}

		/// <summary>
		/// Test whether whitespace-only text nodes are visible in the logical
		/// view of <code>DTM</code>. Normally, this function
		/// will be called by the implementation of <code>DTM</code>;
		/// it is not normally called directly from
		/// user code.
		/// </summary>
		/// <param name="node"> int handle of the node. </param>
		/// <param name="dtm"> the DTM that owns this node </param>
		/// <returns> one of <code>NOTSTRIP</code>, <code>STRIP</code> or
		/// <code>INHERIT</code>. </returns>
		public virtual short getShouldStripSpace(int node, DTM dtm)
		{
			if (m_filter != null && dtm is DOM)
			{
				DOM dom = (DOM)dtm;
				int type = 0;

				if (dtm is DOMEnhancedForDTM)
				{
					DOMEnhancedForDTM mappableDOM = (DOMEnhancedForDTM)dtm;

					short[] mapping;
					if (dtm == m_currentDTM)
					{
						mapping = m_currentMapping;
					}
					else
					{
						mapping = (short[])m_mappings.get(dtm);
						if (mapping == null)
						{
							mapping = mappableDOM.getMapping(m_translet.NamesArray, m_translet.UrisArray, m_translet.TypesArray);
							m_mappings.put(dtm, mapping);
							m_currentDTM = dtm;
							m_currentMapping = mapping;
						}
					}

					int expType = mappableDOM.getExpandedTypeID(node);

					// %OPT% The mapping array does not have information about all the
					// exptypes. However it does contain enough information about all names
					// in the translet's namesArray. If the expType does not fall into the
					// range of the mapping array, it means that the expType is not for one
					// of the recognized names. In this case we can just set the type to -1.
					if (expType >= 0 && expType < mapping.Length)
					{
					  type = mapping[expType];
					}
					else
					{
					  type = -1;
					}

				}
				else
				{
					return INHERIT;
				}

				if (m_filter.stripSpace(dom, node, type))
				{
					return STRIP;
				}
				else
				{
					return NOTSTRIP;
				}
			}
			else
			{
				return NOTSTRIP;
			}
		}
	}

}