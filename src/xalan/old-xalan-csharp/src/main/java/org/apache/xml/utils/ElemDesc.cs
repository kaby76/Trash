using System.Collections;

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
 * $Id: ElemDesc.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// This class is in support of SerializerToHTML, and acts as a sort
	/// of element representative for HTML elements.
	/// @xsl.usage internal
	/// </summary>
	internal class ElemDesc
	{

	  /// <summary>
	  /// Table of attributes for the element </summary>
	  internal Hashtable m_attrs = null;

	  /// <summary>
	  /// Element's flags, describing the role this element plays during
	  /// formatting of the document. This is used as a bitvector; more than one flag
	  /// may be set at a time, bitwise-ORed together. Mnemonic and bits
	  /// have been assigned to the flag values. NOTE: Some bits are
	  /// currently assigned multiple mnemonics; it is the caller's
	  /// responsibility to disambiguate these if necessary. 
	  /// </summary>
	  internal int m_flags;

	  /// <summary>
	  /// Defines mnemonic and bit-value for the EMPTY flag </summary>
	  internal static readonly int EMPTY = (1 << 1);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the FLOW flag </summary>
	  internal static readonly int FLOW = (1 << 2);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the BLOCK flag </summary>
	  internal static readonly int BLOCK = (1 << 3);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the BLOCKFORM  flag </summary>
	  internal static readonly int BLOCKFORM = (1 << 4);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the BLOCKFORMFIELDSET flag </summary>
	  internal static readonly int BLOCKFORMFIELDSET = (1 << 5);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the CDATA flag </summary>
	  internal static readonly int CDATA = (1 << 6);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the PCDATA flag </summary>
	  internal static readonly int PCDATA = (1 << 7);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the RAW flag </summary>
	  internal static readonly int RAW = (1 << 8);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the INLINE flag </summary>
	  internal static readonly int INLINE = (1 << 9);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the INLINEA flag </summary>
	  internal static readonly int INLINEA = (1 << 10);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the INLINELABEL flag </summary>
	  internal static readonly int INLINELABEL = (1 << 11);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the FONTSTYLE flag </summary>
	  internal static readonly int FONTSTYLE = (1 << 12);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the PHRASE flag </summary>
	  internal static readonly int PHRASE = (1 << 13);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the FORMCTRL flag </summary>
	  internal static readonly int FORMCTRL = (1 << 14);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the SPECIAL flag </summary>
	  internal static readonly int SPECIAL = (1 << 15);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the ASPECIAL flag </summary>
	  internal static readonly int ASPECIAL = (1 << 16);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the HEADMISC flag </summary>
	  internal static readonly int HEADMISC = (1 << 17);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the HEAD flag </summary>
	  internal static readonly int HEAD = (1 << 18);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the LIST flag </summary>
	  internal static readonly int LIST = (1 << 19);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the PREFORMATTED flag </summary>
	  internal static readonly int PREFORMATTED = (1 << 20);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the WHITESPACESENSITIVE flag </summary>
	  internal static readonly int WHITESPACESENSITIVE = (1 << 21);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the ATTRURL flag </summary>
	  internal static readonly int ATTRURL = (1 << 1);

	  /// <summary>
	  /// Defines mnemonic and bit-value for the ATTREMPTY flag </summary>
	  internal static readonly int ATTREMPTY = (1 << 2);

	  /// <summary>
	  /// Construct an ElementDescription with an initial set of flags.
	  /// </summary>
	  /// <param name="flags"> Element flags </param>
	  /// <seealso cref= m_flags </seealso>
	  internal ElemDesc(int flags)
	  {
		m_flags = flags;
	  }

	  /// <summary>
	  /// "is (this element described by these flags)".
	  /// 
	  /// This might more properly be called areFlagsSet(). It accepts an
	  /// integer (being used as a bitvector) and checks whether all the 
	  /// corresponding bits are set in our internal flags. Note that this
	  /// test is performed as a bitwise AND, not an equality test, so a
	  /// 0 bit in the input means "don't test", not "must be set false".
	  /// </summary>
	  /// <param name="flags"> Vector of flags to compare against this element's flags
	  /// </param>
	  /// <returns> true if the flags set in the parameter are also set in the
	  /// element's stored flags.
	  /// </returns>
	  /// <seealso cref= m_flags </seealso>
	  /// <seealso cref= isAttrFlagSet </seealso>
	  internal virtual bool @is(int flags)
	  {
		// int which = (m_flags & flags);
		return (m_flags & flags) != 0;
	  }

	  /// <summary>
	  /// Set a new attribute for this element 
	  /// 
	  /// </summary>
	  /// <param name="name"> Attribute name </param>
	  /// <param name="flags"> Attibute flags </param>
	  internal virtual void setAttr(string name, int flags)
	  {

		if (null == m_attrs)
		{
		  m_attrs = new Hashtable();
		}

		m_attrs[name] = new int?(flags);
	  }

	  /// <summary>
	  /// Find out if a flag is set in a given attribute of this element 
	  /// 
	  /// </summary>
	  /// <param name="name"> Attribute name </param>
	  /// <param name="flags"> Flag to check
	  /// </param>
	  /// <returns> True if the flag is set in the attribute. Returns false
	  /// if the attribute is not found </returns>
	  /// <seealso cref= m_flags </seealso>
	  internal virtual bool isAttrFlagSet(string name, int flags)
	  {

		if (null != m_attrs)
		{
		  int? _flags = (int?) m_attrs[name];

		  if (null != _flags)
		  {
			return (_flags.Value & flags) != 0;
		  }
		}

		return false;
	  }
	}

}