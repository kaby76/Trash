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
 * $Id: ElemDesc.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{
	using StringToIntTable = org.apache.xml.serializer.utils.StringToIntTable;

	/// <summary>
	/// This class has a series of flags (bit values) that describe an HTML element
	/// <para>
	/// This class is not a public API.
	/// It is public because it is used outside of this package.
	/// 
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	public sealed class ElemDesc
	{
		/// <summary>
		/// Bit flags to tell about this element type. </summary>
		private int m_flags;

		/// <summary>
		/// Table of attribute names to integers, which contain bit flags telling about
		///  the attributes.
		/// </summary>
		private StringToIntTable m_attrs = null;

		/// <summary>
		/// Bit position if this element type is empty. </summary>
		internal static readonly int EMPTY = (1 << 1);

		/// <summary>
		/// Bit position if this element type is a flow. </summary>
		private static readonly int FLOW = (1 << 2);

		/// <summary>
		/// Bit position if this element type is a block. </summary>
		internal static readonly int BLOCK = (1 << 3);

		/// <summary>
		/// Bit position if this element type is a block form. </summary>
		internal static readonly int BLOCKFORM = (1 << 4);

		/// <summary>
		/// Bit position if this element type is a block form field set. </summary>
		internal static readonly int BLOCKFORMFIELDSET = (1 << 5);

		/// <summary>
		/// Bit position if this element type is CDATA. </summary>
		private static readonly int CDATA = (1 << 6);

		/// <summary>
		/// Bit position if this element type is PCDATA. </summary>
		private static readonly int PCDATA = (1 << 7);

		/// <summary>
		/// Bit position if this element type is should be raw characters. </summary>
		internal static readonly int RAW = (1 << 8);

		/// <summary>
		/// Bit position if this element type should be inlined. </summary>
		private static readonly int INLINE = (1 << 9);

		/// <summary>
		/// Bit position if this element type is INLINEA. </summary>
		private static readonly int INLINEA = (1 << 10);

		/// <summary>
		/// Bit position if this element type is an inline label. </summary>
		internal static readonly int INLINELABEL = (1 << 11);

		/// <summary>
		/// Bit position if this element type is a font style. </summary>
		internal static readonly int FONTSTYLE = (1 << 12);

		/// <summary>
		/// Bit position if this element type is a phrase. </summary>
		internal static readonly int PHRASE = (1 << 13);

		/// <summary>
		/// Bit position if this element type is a form control. </summary>
		internal static readonly int FORMCTRL = (1 << 14);

		/// <summary>
		/// Bit position if this element type is ???. </summary>
		internal static readonly int SPECIAL = (1 << 15);

		/// <summary>
		/// Bit position if this element type is ???. </summary>
		internal static readonly int ASPECIAL = (1 << 16);

		/// <summary>
		/// Bit position if this element type is an odd header element. </summary>
		internal static readonly int HEADMISC = (1 << 17);

		/// <summary>
		/// Bit position if this element type is a head element (i.e. H1, H2, etc.) </summary>
		internal static readonly int HEAD = (1 << 18);

		/// <summary>
		/// Bit position if this element type is a list. </summary>
		internal static readonly int LIST = (1 << 19);

		/// <summary>
		/// Bit position if this element type is a preformatted type. </summary>
		internal static readonly int PREFORMATTED = (1 << 20);

		/// <summary>
		/// Bit position if this element type is whitespace sensitive. </summary>
		internal static readonly int WHITESPACESENSITIVE = (1 << 21);

		/// <summary>
		/// Bit position if this element type is a header element (i.e. HEAD). </summary>
		internal static readonly int HEADELEM = (1 << 22);

		/// <summary>
		/// Bit position if this element is the "HTML" element </summary>
		internal static readonly int HTMLELEM = (1 << 23);

		/// <summary>
		/// Bit position if this attribute type is a URL. </summary>
		public static readonly int ATTRURL = (1 << 1);

		/// <summary>
		/// Bit position if this attribute type is an empty type. </summary>
		public static readonly int ATTREMPTY = (1 << 2);

		/// <summary>
		/// Construct an ElemDesc from a set of bit flags.
		/// 
		/// </summary>
		/// <param name="flags"> Bit flags that describe the basic properties of this element type. </param>
		internal ElemDesc(int flags)
		{
			m_flags = flags;
		}

		/// <summary>
		/// Tell if this element type has the basic bit properties that are passed
		/// as an argument.
		/// </summary>
		/// <param name="flags"> Bit flags that describe the basic properties of interest.
		/// </param>
		/// <returns> true if any of the flag bits are true. </returns>
		private bool @is(int flags)
		{

			// int which = (m_flags & flags);
			return (m_flags & flags) != 0;
		}

		internal int Flags
		{
			get
			{
				return m_flags;
			}
		}

		/// <summary>
		/// Set an attribute name and it's bit properties.
		/// 
		/// </summary>
		/// <param name="name"> non-null name of attribute, in upper case. </param>
		/// <param name="flags"> flag bits. </param>
		internal void setAttr(string name, int flags)
		{

			if (null == m_attrs)
			{
				m_attrs = new StringToIntTable();
			}

			m_attrs.put(name, flags);
		}

		/// <summary>
		/// Tell if any of the bits of interest are set for a named attribute type.
		/// </summary>
		/// <param name="name"> non-null reference to attribute name, in any case. </param>
		/// <param name="flags"> flag mask.
		/// </param>
		/// <returns> true if any of the flags are set for the named attribute. </returns>
		public bool isAttrFlagSet(string name, int flags)
		{
			return (null != m_attrs) ? ((m_attrs.getIgnoreCase(name) & flags) != 0) : false;
		}
	}

}