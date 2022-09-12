using System.Collections;
using System.Text;

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
 * $Id: AttributesImplSerializer.java 468654 2006-10-28 07:09:23Z minchau $
 */

namespace org.apache.xml.serializer
{

	using Attributes = org.xml.sax.Attributes;
	using AttributesImpl = org.xml.sax.helpers.AttributesImpl;

	/// <summary>
	/// This class extends org.xml.sax.helpers.AttributesImpl which implements org.
	/// xml.sax.Attributes. But for optimization this class adds a Hashtable for
	/// faster lookup of an index by qName, which is commonly done in the stream
	/// serializer.
	/// </summary>
	/// <seealso cref= org.xml.sax.Attributes
	/// 
	/// @xsl.usage internal </seealso>
	public sealed class AttributesImplSerializer : AttributesImpl
	{
		/// <summary>
		/// Hash table of qName/index values to quickly lookup the index
		/// of an attributes qName.  qNames are in uppercase in the hash table
		/// to make the search case insensitive.
		/// 
		/// The keys to the hashtable to find the index are either
		/// "prefix:localName"  or "{uri}localName".
		/// </summary>
		private readonly Hashtable m_indexFromQName = new Hashtable();

		private readonly StringBuilder m_buff = new StringBuilder();

		/// <summary>
		/// This is the number of attributes before switching to the hash table,
		/// and can be tuned, but 12 seems good for now - Brian M.
		/// </summary>
		private const int MAX = 12;

		/// <summary>
		/// One less than the number of attributes before switching to
		/// the Hashtable.
		/// </summary>
		private static readonly int MAXMinus1 = MAX - 1;

		/// <summary>
		/// This method gets the index of an attribute given its qName. </summary>
		/// <param name="qname"> the qualified name of the attribute, e.g. "prefix1:locName1" </param>
		/// <returns> the integer index of the attribute. </returns>
		/// <seealso cref= org.xml.sax.Attributes#getIndex(String) </seealso>
		public int getIndex(string qname)
		{
			int index;

			if (base.Length < MAX)
			{
				// if we haven't got too many attributes let the
				// super class look it up
				index = base.getIndex(qname);
				return index;
			}
			// we have too many attributes and the super class is slow
			// so find it quickly using our Hashtable.
			int? i = (int?)m_indexFromQName[qname];
			if (i == null)
			{
				index = -1;
			}
			else
			{
				index = i.Value;
			}
			return index;
		}
		/// <summary>
		/// This method adds the attribute, but also records its qName/index pair in
		/// the hashtable for fast lookup by getIndex(qName). </summary>
		/// <param name="uri"> the URI of the attribute </param>
		/// <param name="local"> the local name of the attribute </param>
		/// <param name="qname"> the qualified name of the attribute </param>
		/// <param name="type"> the type of the attribute </param>
		/// <param name="val"> the value of the attribute
		/// </param>
		/// <seealso cref= org.xml.sax.helpers.AttributesImpl#addAttribute(String, String, String, String, String) </seealso>
		/// <seealso cref= #getIndex(String) </seealso>
		public void addAttribute(string uri, string local, string qname, string type, string val)
		{
			int index = base.Length;
			base.addAttribute(uri, local, qname, type, val);
			// (index + 1) is now the number of attributes
			// so either compare (index+1) to MAX, or compare index to (MAX-1)

			if (index < MAXMinus1)
			{
				return;
			}
			else if (index == MAXMinus1)
			{
				switchOverToHash(MAX);
			}
			else
			{
				/* add the key with the format of "prefix:localName" */
				/* we have just added the attibute, its index is the old length */
				int? i = new int?(index);
				m_indexFromQName[qname] = i;

				/* now add with key of the format "{uri}localName" */
				m_buff.Length = 0;
				m_buff.Append('{').Append(uri).Append('}').Append(local);
				string key = m_buff.ToString();
				m_indexFromQName[key] = i;
			}
			return;
		}

		/// <summary>
		/// We are switching over to having a hash table for quick look
		/// up of attributes, but up until now we haven't kept any
		/// information in the Hashtable, so we now update the Hashtable.
		/// Future additional attributes will update the Hashtable as
		/// they are added. </summary>
		/// <param name="numAtts"> </param>
		private void switchOverToHash(int numAtts)
		{
			for (int index = 0; index < numAtts; index++)
			{
				string qName = base.getQName(index);
				int? i = new int?(index);
				m_indexFromQName[qName] = i;

				// Add quick look-up to find with uri/local name pair
				string uri = base.getURI(index);
				string local = base.getLocalName(index);
				m_buff.Length = 0;
				m_buff.Append('{').Append(uri).Append('}').Append(local);
				string key = m_buff.ToString();
				m_indexFromQName[key] = i;
			}
		}

		/// <summary>
		/// This method clears the accumulated attributes.
		/// </summary>
		/// <seealso cref= org.xml.sax.helpers.AttributesImpl#clear() </seealso>
		public void clear()
		{

			int len = base.Length;
			base.clear();
			if (MAX <= len)
			{
				// if we have had enough attributes and are
				// using the Hashtable, then clear the Hashtable too.
				m_indexFromQName.Clear();
			}

		}

		/// <summary>
		/// This method sets the attributes, previous attributes are cleared,
		/// it also keeps the hashtable up to date for quick lookup via
		/// getIndex(qName). </summary>
		/// <param name="atts"> the attributes to copy into these attributes. </param>
		/// <seealso cref= org.xml.sax.helpers.AttributesImpl#setAttributes(Attributes) </seealso>
		/// <seealso cref= #getIndex(String) </seealso>
		public Attributes Attributes
		{
			set
			{
    
				base.Attributes = value;
    
				// we've let the super class add the attributes, but
				// we need to keep the hash table up to date ourselves for the
				// potentially new qName/index pairs for quick lookup. 
				int numAtts = value.Length;
				if (MAX <= numAtts)
				{
					switchOverToHash(numAtts);
				}
    
			}
		}

		/// <summary>
		/// This method gets the index of an attribute given its uri and locanName. </summary>
		/// <param name="uri"> the URI of the attribute name. </param>
		/// <param name="localName"> the local namer (after the ':' ) of the attribute name. </param>
		/// <returns> the integer index of the attribute. </returns>
		/// <seealso cref= org.xml.sax.Attributes#getIndex(String) </seealso>
		public int getIndex(string uri, string localName)
		{
			int index;

			if (base.Length < MAX)
			{
				// if we haven't got too many attributes let the
				// super class look it up
				index = base.getIndex(uri,localName);
				return index;
			}
			// we have too many attributes and the super class is slow
			// so find it quickly using our Hashtable.
			// Form the key of format "{uri}localName"
			m_buff.Length = 0;
			m_buff.Append('{').Append(uri).Append('}').Append(localName);
			string key = m_buff.ToString();
			int? i = (int?)m_indexFromQName[key];
			if (i == null)
			{
				index = -1;
			}
			else
			{
				index = i.Value;
			}
			return index;
		}
	}

}