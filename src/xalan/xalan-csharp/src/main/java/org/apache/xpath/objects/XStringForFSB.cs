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
 * $Id: XStringForFSB.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XMLCharacterRecognizer = org.apache.xml.utils.XMLCharacterRecognizer;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// This class will wrap a FastStringBuffer and allow for
	/// </summary>
	[Serializable]
	public class XStringForFSB : XString
	{
		internal new const long serialVersionUID = -1533039186550674548L;

	  /// <summary>
	  /// The start position in the fsb. </summary>
	  internal int m_start;

	  /// <summary>
	  /// The length of the string. </summary>
	  internal int m_length;

	  /// <summary>
	  /// If the str() function is called, the string will be cached here. </summary>
	  protected internal string m_strCache = null;

	  /// <summary>
	  /// cached hash code </summary>
	  protected internal int m_hash = 0;

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> FastStringBuffer object this will wrap, must be non-null. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  public XStringForFSB(FastStringBuffer val, int start, int length) : base(val)
	  {


		m_start = start;
		m_length = length;

		if (null == val)
		{
		  throw new System.ArgumentException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, null));
		}
	  }

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> String object this will wrap. </param>
	  private XStringForFSB(string val) : base(val)
	  {


		throw new System.ArgumentException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_FSB_CANNOT_TAKE_STRING, null)); // "XStringForFSB can not take a string for an argument!");
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public virtual FastStringBuffer fsb()
	  {
		return ((FastStringBuffer) m_obj);
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override void appendToFsb(FastStringBuffer fsb)
	  {
		// %OPT% !!! FSB has to be updated to take partial fsb's for append.
		fsb.append(str());
	  }

	  /// <summary>
	  /// Tell if this object contains a java String object.
	  /// </summary>
	  /// <returns> true if this XMLString can return a string without creating one. </returns>
	  public override bool hasString()
	  {
		return (null != m_strCache);
	  }

	//  /** NEEDSDOC Field strCount */
	//  public static int strCount = 0;
	//
	//  /** NEEDSDOC Field xtable */
	//  static java.util.Hashtable xtable = new java.util.Hashtable();

	  /// <summary>
	  /// Since this object is incomplete without the length and the offset, we 
	  /// have to convert to a string when this function is called.
	  /// </summary>
	  /// <returns> The java String representation of this object. </returns>
	  public override object @object()
	  {
		return str();
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override string str()
	  {

		if (null == m_strCache)
		{
		  m_strCache = fsb().getString(m_start, m_length);

	//      strCount++;
	//
	//      RuntimeException e = new RuntimeException("Bad!  Bad!");
	//      java.io.CharArrayWriter writer = new java.io.CharArrayWriter();
	//      java.io.PrintWriter pw = new java.io.PrintWriter(writer);
	//
	//      e.printStackTrace(pw);
	//
	//      String str = writer.toString();
	//
	//      str = str.substring(0, 600);
	//
	//      if (null == xtable.get(str))
	//      {
	//        xtable.put(str, str);
	//        System.out.println(str);
	//      }
	//      System.out.println("strCount: " + strCount);

	//      throw e;
	//      e.printStackTrace();
		  // System.exit(-1);
		}

		return m_strCache;
	  }

	  /// <summary>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value. Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// </summary>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchCharactersEvents(org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
	  public override void dispatchCharactersEvents(org.xml.sax.ContentHandler ch)
	  {
		fsb().sendSAXcharacters(ch, m_start, m_length);
	  }

	  /// <summary>
	  /// Directly call the
	  /// comment method on the passed LexicalHandler for the
	  /// string-value.
	  /// </summary>
	  /// <param name="lh"> A non-null reference to a LexicalHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchAsComment(org.xml.sax.ext.LexicalHandler lh) throws org.xml.sax.SAXException
	  public override void dispatchAsComment(org.xml.sax.ext.LexicalHandler lh)
	  {
		fsb().sendSAXComment(lh, m_start, m_length);
	  }

	  /// <summary>
	  /// Returns the length of this string.
	  /// </summary>
	  /// <returns>  the length of the sequence of characters represented by this
	  ///          object. </returns>
	  public override int length()
	  {
		return m_length;
	  }

	  /// <summary>
	  /// Returns the character at the specified index. An index ranges
	  /// from <code>0</code> to <code>length() - 1</code>. The first character
	  /// of the sequence is at index <code>0</code>, the next at index
	  /// <code>1</code>, and so on, as for array indexing.
	  /// </summary>
	  /// <param name="index">   the index of the character. </param>
	  /// <returns>     the character at the specified index of this string.
	  ///             The first character is at index <code>0</code>. </returns>
	  /// <exception cref="IndexOutOfBoundsException">  if the <code>index</code>
	  ///             argument is negative or not less than the length of this
	  ///             string. </exception>
	  public override char charAt(int index)
	  {
		return fsb().charAt(m_start + index);
	  }

	  /// <summary>
	  /// Copies characters from this string into the destination character
	  /// array.
	  /// </summary>
	  /// <param name="srcBegin">   index of the first character in the string
	  ///                        to copy. </param>
	  /// <param name="srcEnd">     index after the last character in the string
	  ///                        to copy. </param>
	  /// <param name="dst">        the destination array. </param>
	  /// <param name="dstBegin">   the start offset in the destination array. </param>
	  /// <exception cref="IndexOutOfBoundsException"> If any of the following
	  ///            is true:
	  ///            <ul><li><code>srcBegin</code> is negative.
	  ///            <li><code>srcBegin</code> is greater than <code>srcEnd</code>
	  ///            <li><code>srcEnd</code> is greater than the length of this
	  ///                string
	  ///            <li><code>dstBegin</code> is negative
	  ///            <li><code>dstBegin+(srcEnd-srcBegin)</code> is larger than
	  ///                <code>dst.length</code></ul> </exception>
	  /// <exception cref="NullPointerException"> if <code>dst</code> is <code>null</code> </exception>
	  public override void getChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
	  {

		// %OPT% Need to call this on FSB when it is implemented.
		// %UNTESTED% (I don't think anyone calls this yet?)
		int n = srcEnd - srcBegin;

		if (n > m_length)
		{
		  n = m_length;
		}

		if (n > (dst.Length - dstBegin))
		{
		  n = (dst.Length - dstBegin);
		}

		int end = srcBegin + m_start + n;
		int d = dstBegin;
		FastStringBuffer fsb = this.fsb();

		for (int i = srcBegin + m_start; i < end; i++)
		{
		  dst[d++] = fsb.charAt(i);
		}
	  }

	  /// <summary>
	  /// Compares this string to the specified object.
	  /// The result is <code>true</code> if and only if the argument is not
	  /// <code>null</code> and is a <code>String</code> object that represents
	  /// the same sequence of characters as this object.
	  /// </summary>
	  /// <param name="obj2">       the object to compare this <code>String</code>
	  ///                     against.
	  /// </param>
	  /// <returns>  <code>true</code> if the <code>String </code>are equal;
	  ///          <code>false</code> otherwise. </returns>
	  /// <seealso cref="java.lang.String.compareTo(java.lang.String)"/>
	  /// <seealso cref="java.lang.String.equalsIgnoreCase(java.lang.String)"/>
	  public override bool Equals(XMLString obj2)
	  {

		if (this == obj2)
		{
		  return true;
		}

		int n = m_length;

		if (n == obj2.length())
		{
		  FastStringBuffer fsb = this.fsb();
		  int i = m_start;
		  int j = 0;

		  while (n-- != 0)
		  {
			if (fsb.charAt(i) != obj2.charAt(j))
			{
			  return false;
			}

			i++;
			j++;
		  }

		  return true;
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> true if the two objects are equal
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public override bool Equals(XObject obj2)
	  {

		if (this == obj2)
		{
		  return true;
		}
		if (obj2.Type == XObject.CLASS_NUMBER)
		{
			return obj2.Equals(this);
		}

		string str = obj2.str();
		int n = m_length;

		if (n == str.Length)
		{
		  FastStringBuffer fsb = this.fsb();
		  int i = m_start;
		  int j = 0;

		  while (n-- != 0)
		  {
			if (fsb.charAt(i) != str[j])
			{
			  return false;
			}

			i++;
			j++;
		  }

		  return true;
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="anotherString"> Object to compare this to
	  /// </param>
	  /// <returns> true if the two objects are equal
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public override bool Equals(string anotherString)
	  {

		int n = m_length;

		if (n == anotherString.Length)
		{
		  FastStringBuffer fsb = this.fsb();
		  int i = m_start;
		  int j = 0;

		  while (n-- != 0)
		  {
			if (fsb.charAt(i) != anotherString[j])
			{
			  return false;
			}

			i++;
			j++;
		  }

		  return true;
		}

		return false;
	  }

	  /// <summary>
	  /// Compares this string to the specified object.
	  /// The result is <code>true</code> if and only if the argument is not
	  /// <code>null</code> and is a <code>String</code> object that represents
	  /// the same sequence of characters as this object.
	  /// </summary>
	  /// <param name="obj2">       the object to compare this <code>String</code>
	  ///                     against.
	  /// </param>
	  /// <returns>  <code>true</code> if the <code>String </code>are equal;
	  ///          <code>false</code> otherwise. </returns>
	  /// <seealso cref="java.lang.String.compareTo(java.lang.String)"/>
	  /// <seealso cref="java.lang.String.equalsIgnoreCase(java.lang.String)"/>
	  public override bool Equals(object obj2)
	  {

		if (null == obj2)
		{
		  return false;
		}

		if (obj2 is XNumber)
		{
			return obj2.Equals(this);
		}

		  // In order to handle the 'all' semantics of 
		  // nodeset comparisons, we always call the 
		  // nodeset function.
		else if (obj2 is XNodeSet)
		{
		  return obj2.Equals(this);
		}
		else if (obj2 is XStringForFSB)
		{
		  return Equals((XMLString) obj2);
		}
		else
		{
		  return Equals(obj2.ToString());
		}
	  }

	  /// <summary>
	  /// Compares this <code>String</code> to another <code>String</code>,
	  /// ignoring case considerations.  Two strings are considered equal
	  /// ignoring case if they are of the same length, and corresponding
	  /// characters in the two strings are equal ignoring case.
	  /// </summary>
	  /// <param name="anotherString">   the <code>String</code> to compare this
	  ///                          <code>String</code> against. </param>
	  /// <returns>  <code>true</code> if the argument is not <code>null</code>
	  ///          and the <code>String</code>s are equal,
	  ///          ignoring case; <code>false</code> otherwise. </returns>
	  /// <seealso cref=".equals(Object)"/>
	  /// <seealso cref="java.lang.Character.toLowerCase(char)"/>
	  /// <seealso cref="java.lang.Character.toUpperCase(char)"/>
	  public override bool equalsIgnoreCase(string anotherString)
	  {
		return (m_length == anotherString.Length) ? str().Equals(anotherString, StringComparison.OrdinalIgnoreCase) : false;
	  }

	  /// <summary>
	  /// Compares two strings lexicographically.
	  /// </summary>
	  /// <param name="xstr">   the <code>String</code> to be compared.
	  /// </param>
	  /// <returns>  the value <code>0</code> if the argument string is equal to
	  ///          this string; a value less than <code>0</code> if this string
	  ///          is lexicographically less than the string argument; and a
	  ///          value greater than <code>0</code> if this string is
	  ///          lexicographically greater than the string argument. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>anotherString</code>
	  ///          is <code>null</code>. </exception>
	  public override int compareTo(XMLString xstr)
	  {

		int len1 = m_length;
		int len2 = xstr.length();
		int n = Math.Min(len1, len2);
		FastStringBuffer fsb = this.fsb();
		int i = m_start;
		int j = 0;

		while (n-- != 0)
		{
		  char c1 = fsb.charAt(i);
		  char c2 = xstr.charAt(j);

		  if (c1 != c2)
		  {
			return c1 - c2;
		  }

		  i++;
		  j++;
		}

		return len1 - len2;
	  }

	  /// <summary>
	  /// Compares two strings lexicographically, ignoring case considerations.
	  /// This method returns an integer whose sign is that of
	  /// <code>this.toUpperCase().toLowerCase().compareTo(
	  /// str.toUpperCase().toLowerCase())</code>.
	  /// <para>
	  /// Note that this method does <em>not</em> take locale into account,
	  /// and will result in an unsatisfactory ordering for certain locales.
	  /// The java.text package provides <em>collators</em> to allow
	  /// locale-sensitive ordering.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="xstr">   the <code>String</code> to be compared.
	  /// </param>
	  /// <returns>  a negative integer, zero, or a positive integer as the
	  ///          the specified String is greater than, equal to, or less
	  ///          than this String, ignoring case considerations. </returns>
	  /// <seealso cref="java.text.Collator.compare(String, String)"
	  /// @since   1.2/>
	  public override int compareToIgnoreCase(XMLString xstr)
	  {

		int len1 = m_length;
		int len2 = xstr.length();
		int n = Math.Min(len1, len2);
		FastStringBuffer fsb = this.fsb();
		int i = m_start;
		int j = 0;

		while (n-- != 0)
		{
		  char c1 = char.ToLower(fsb.charAt(i));
		  char c2 = char.ToLower(xstr.charAt(j));

		  if (c1 != c2)
		  {
			return c1 - c2;
		  }

		  i++;
		  j++;
		}

		return len1 - len2;
	  }

	  /// <summary>
	  /// Returns a hashcode for this string. The hashcode for a
	  /// <code>String</code> object is computed as
	  /// <blockquote><pre>
	  /// s[0]*31^(n-1) + s[1]*31^(n-2) + ... + s[n-1]
	  /// </pre></blockquote>
	  /// using <code>int</code> arithmetic, where <code>s[i]</code> is the
	  /// <i>i</i>th character of the string, <code>n</code> is the length of
	  /// the string, and <code>^</code> indicates exponentiation.
	  /// (The hash value of the empty string is zero.)
	  /// </summary>
	  /// <returns>  a hash code value for this object. </returns>
	  public override int GetHashCode()
	  {
		// Commenting this out because in JDK1.1.8 and VJ++
		// we don't match XMLStrings. Defaulting to the super
		// causes us to create a string, but at this point
		// this only seems to get called in key processing.
		// Maybe we can live with it?

	/*
	    int h = m_hash;
	
	    if (h == 0)
	    {
	      int off = m_start;
	      int len = m_length;
	      FastStringBuffer fsb = fsb();
	
	      for (int i = 0; i < len; i++)
	      {
	        h = 31 * h + fsb.charAt(off);
	
	        off++;
	      }
	
	      m_hash = h;
	    }
	    */

		return base.GetHashCode(); // h;
	  }

	  /// <summary>
	  /// Tests if this string starts with the specified prefix beginning
	  /// a specified index.
	  /// </summary>
	  /// <param name="prefix">    the prefix. </param>
	  /// <param name="toffset">   where to begin looking in the string. </param>
	  /// <returns>  <code>true</code> if the character sequence represented by the
	  ///          argument is a prefix of the substring of this object starting
	  ///          at index <code>toffset</code>; <code>false</code> otherwise.
	  ///          The result is <code>false</code> if <code>toffset</code> is
	  ///          negative or greater than the length of this
	  ///          <code>String</code> object; otherwise the result is the same
	  ///          as the result of the expression
	  ///          <pre>
	  ///          this.subString(toffset).startsWith(prefix)
	  ///          </pre> </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>prefix</code> is
	  ///          <code>null</code>. </exception>
	  public override bool startsWith(XMLString prefix, int toffset)
	  {

		FastStringBuffer fsb = this.fsb();
		int to = m_start + toffset;
		int tlim = m_start + m_length;
		int po = 0;
		int pc = prefix.length();

		// Note: toffset might be near -1>>>1.
		if ((toffset < 0) || (toffset > m_length - pc))
		{
		  return false;
		}

		while (--pc >= 0)
		{
		  if (fsb.charAt(to) != prefix.charAt(po))
		  {
			return false;
		  }

		  to++;
		  po++;
		}

		return true;
	  }

	  /// <summary>
	  /// Tests if this string starts with the specified prefix.
	  /// </summary>
	  /// <param name="prefix">   the prefix. </param>
	  /// <returns>  <code>true</code> if the character sequence represented by the
	  ///          argument is a prefix of the character sequence represented by
	  ///          this string; <code>false</code> otherwise.
	  ///          Note also that <code>true</code> will be returned if the
	  ///          argument is an empty string or is equal to this
	  ///          <code>String</code> object as determined by the
	  ///          <seealso cref="equals(Object)"/> method. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>prefix</code> is
	  ///          <code>null</code>.
	  /// @since   JDK1. 0 </exception>
	  public override bool startsWith(XMLString prefix)
	  {
		return startsWith(prefix, 0);
	  }

	  /// <summary>
	  /// Returns the index within this string of the first occurrence of the
	  /// specified character. If a character with value <code>ch</code> occurs
	  /// in the character sequence represented by this <code>String</code>
	  /// object, then the index of the first such occurrence is returned --
	  /// that is, the smallest value <i>k</i> such that:
	  /// <blockquote><pre>
	  /// this.charAt(<i>k</i>) == ch
	  /// </pre></blockquote>
	  /// is <code>true</code>. If no such character occurs in this string,
	  /// then <code>-1</code> is returned.
	  /// </summary>
	  /// <param name="ch">   a character. </param>
	  /// <returns>  the index of the first occurrence of the character in the
	  ///          character sequence represented by this object, or
	  ///          <code>-1</code> if the character does not occur. </returns>
	  public override int indexOf(int ch)
	  {
		return indexOf(ch, 0);
	  }

	  /// <summary>
	  /// Returns the index within this string of the first occurrence of the
	  /// specified character, starting the search at the specified index.
	  /// <para>
	  /// If a character with value <code>ch</code> occurs in the character
	  /// sequence represented by this <code>String</code> object at an index
	  /// no smaller than <code>fromIndex</code>, then the index of the first
	  /// such occurrence is returned--that is, the smallest value <i>k</i>
	  /// such that:
	  /// <blockquote><pre>
	  /// (this.charAt(<i>k</i>) == ch) && (<i>k</i> >= fromIndex)
	  /// </pre></blockquote>
	  /// is true. If no such character occurs in this string at or after
	  /// position <code>fromIndex</code>, then <code>-1</code> is returned.
	  /// </para>
	  /// <para>
	  /// There is no restriction on the value of <code>fromIndex</code>. If it
	  /// is negative, it has the same effect as if it were zero: this entire
	  /// string may be searched. If it is greater than the length of this
	  /// string, it has the same effect as if it were equal to the length of
	  /// this string: <code>-1</code> is returned.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="ch">          a character. </param>
	  /// <param name="fromIndex">   the index to start the search from. </param>
	  /// <returns>  the index of the first occurrence of the character in the
	  ///          character sequence represented by this object that is greater
	  ///          than or equal to <code>fromIndex</code>, or <code>-1</code>
	  ///          if the character does not occur. </returns>
	  public override int indexOf(int ch, int fromIndex)
	  {

		int max = m_start + m_length;
		FastStringBuffer fsb = this.fsb();

		if (fromIndex < 0)
		{
		  fromIndex = 0;
		}
		else if (fromIndex >= m_length)
		{

		  // Note: fromIndex might be near -1>>>1.
		  return -1;
		}

		for (int i = m_start + fromIndex; i < max; i++)
		{
		  if (fsb.charAt(i) == (char)ch)
		  {
			return i - m_start;
		  }
		}

		return -1;
	  }

	  /// <summary>
	  /// Returns a new string that is a substring of this string. The
	  /// substring begins with the character at the specified index and
	  /// extends to the end of this string. <para>
	  /// Examples:
	  /// <blockquote><pre>
	  /// "unhappy".substring(2) returns "happy"
	  /// "Harbison".substring(3) returns "bison"
	  /// "emptiness".substring(9) returns "" (an empty string)
	  /// </pre></blockquote>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="beginIndex">   the beginning index, inclusive. </param>
	  /// <returns>     the specified substring. </returns>
	  /// <exception cref="IndexOutOfBoundsException">  if
	  ///             <code>beginIndex</code> is negative or larger than the
	  ///             length of this <code>String</code> object. </exception>
	  public override XMLString substring(int beginIndex)
	  {

		int len = m_length - beginIndex;

		if (len <= 0)
		{
		  return XString.EMPTYSTRING;
		}
		else
		{
		  int start = m_start + beginIndex;

		  return new XStringForFSB(fsb(), start, len);
		}
	  }

	  /// <summary>
	  /// Returns a new string that is a substring of this string. The
	  /// substring begins at the specified <code>beginIndex</code> and
	  /// extends to the character at index <code>endIndex - 1</code>.
	  /// Thus the length of the substring is <code>endIndex-beginIndex</code>.
	  /// </summary>
	  /// <param name="beginIndex">   the beginning index, inclusive. </param>
	  /// <param name="endIndex">     the ending index, exclusive. </param>
	  /// <returns>     the specified substring. </returns>
	  /// <exception cref="IndexOutOfBoundsException">  if the
	  ///             <code>beginIndex</code> is negative, or
	  ///             <code>endIndex</code> is larger than the length of
	  ///             this <code>String</code> object, or
	  ///             <code>beginIndex</code> is larger than
	  ///             <code>endIndex</code>. </exception>
	  public override XMLString substring(int beginIndex, int endIndex)
	  {

		int len = endIndex - beginIndex;

		if (len > m_length)
		{
		  len = m_length;
		}

		if (len <= 0)
		{
		  return XString.EMPTYSTRING;
		}
		else
		{
		  int start = m_start + beginIndex;

		  return new XStringForFSB(fsb(), start, len);
		}
	  }

	  /// <summary>
	  /// Concatenates the specified string to the end of this string.
	  /// </summary>
	  /// <param name="str">   the <code>String</code> that is concatenated to the end
	  ///                of this <code>String</code>. </param>
	  /// <returns>  a string that represents the concatenation of this object's
	  ///          characters followed by the string argument's characters. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>str</code> is
	  ///          <code>null</code>. </exception>
	  public override XMLString concat(string str)
	  {

		// %OPT% Make an FSB here?
		return new XString(this.str() + str);
	  }

	  /// <summary>
	  /// Removes white space from both ends of this string.
	  /// </summary>
	  /// <returns>  this string, with white space removed from the front and end. </returns>
	  public override XMLString trim()
	  {
		return fixWhiteSpace(true, true, false);
	  }

	  /// <summary>
	  /// Returns whether the specified <var>ch</var> conforms to the XML 1.0 definition
	  /// of whitespace.  Refer to <A href="http://www.w3.org/TR/1998/REC-xml-19980210#NT-S">
	  /// the definition of <CODE>S</CODE></A> for details. </summary>
	  /// <param name="ch">      Character to check as XML whitespace. </param>
	  /// <returns>          =true if <var>ch</var> is XML whitespace; otherwise =false. </returns>
	  private static bool isSpace(char ch)
	  {
		return XMLCharacterRecognizer.isWhiteSpace(ch); // Take the easy way out for now.
	  }

	  /// <summary>
	  /// Conditionally trim all leading and trailing whitespace in the specified String.
	  /// All strings of white space are
	  /// replaced by a single space character (#x20), except spaces after punctuation which
	  /// receive double spaces if doublePunctuationSpaces is true.
	  /// This function may be useful to a formatter, but to get first class
	  /// results, the formatter should probably do it's own white space handling
	  /// based on the semantics of the formatting object.
	  /// </summary>
	  /// <param name="trimHead">    Trim leading whitespace? </param>
	  /// <param name="trimTail">    Trim trailing whitespace? </param>
	  /// <param name="doublePunctuationSpaces">    Use double spaces for punctuation? </param>
	  /// <returns>              The trimmed string. </returns>
	  public override XMLString fixWhiteSpace(bool trimHead, bool trimTail, bool doublePunctuationSpaces)
	  {

		int end = m_length + m_start;
		char[] buf = new char[m_length];
		FastStringBuffer fsb = this.fsb();
		bool edit = false;

		/* replace S to ' '. and ' '+ -> single ' '. */
		int d = 0;
		bool pres = false;

		for (int s = m_start; s < end; s++)
		{
		  char c = fsb.charAt(s);

		  if (isSpace(c))
		  {
			if (!pres)
			{
			  if (' ' != c)
			  {
				edit = true;
			  }

			  buf[d++] = ' ';

			  if (doublePunctuationSpaces && (d != 0))
			  {
				char prevChar = buf[d - 1];

				if (!((prevChar == '.') || (prevChar == '!') || (prevChar == '?')))
				{
				  pres = true;
				}
			  }
			  else
			  {
				pres = true;
			  }
			}
			else
			{
			  edit = true;
			  pres = true;
			}
		  }
		  else
		  {
			buf[d++] = c;
			pres = false;
		  }
		}

		if (trimTail && 1 <= d && ' ' == buf[d - 1])
		{
		  edit = true;

		  d--;
		}

		int start = 0;

		if (trimHead && 0 < d && ' ' == buf[0])
		{
		  edit = true;

		  start++;
		}

		XMLStringFactory xsf = XMLStringFactoryImpl.Factory;

		return edit ? xsf.newstr(buf, start, d - start) : this;
	  }

	  /// <summary>
	  /// Convert a string to a double -- Allowed input is in fixed
	  /// notation ddd.fff.
	  /// 
	  /// %OPT% CHECK PERFORMANCE against generating a Java String and
	  /// converting it to double. The advantage of running in native
	  /// machine code -- perhaps even microcode, on some systems -- may
	  /// more than make up for the cost of allocating and discarding the
	  /// additional object. We need to benchmark this. 
	  /// 
	  /// %OPT% More importantly, we need to decide whether we _care_ about
	  /// the performance of this operation. Does XString.toDouble constitute
	  /// any measurable percentage of our typical runtime? I suspect not!
	  /// </summary>
	  /// <returns> A double value representation of the string, or return Double.NaN 
	  /// if the string can not be converted.   </returns>
	  public override double toDouble()
	  {
		if (m_length == 0)
		{
		  return Double.NaN;
		}
		int i;
		char c;
		string valueString = fsb().getString(m_start,m_length);

		// The following are permitted in the Double.valueOf, but not by the XPath spec:
		// - a plus sign
		// - The use of e or E to indicate exponents
		// - trailing f, F, d, or D
		// See function comments; not sure if this is slower than actually doing the
		// conversion ourselves (as was before).

		for (i = 0;i < m_length;i++)
		{
		  if (!XMLCharacterRecognizer.isWhiteSpace(valueString[i]))
		  {
			break;
		  }
		}
		if (i == m_length)
		{
			return Double.NaN;
		}
		if (valueString[i] == '-')
		{
		  i++;
		}
		for (;i < m_length;i++)
		{
		  c = valueString[i];
		  if (c != '.' && (c < '0' || c > '9'))
		  {
			break;
		  }
		}
		for (;i < m_length;i++)
		{
		  if (!XMLCharacterRecognizer.isWhiteSpace(valueString[i]))
		  {
			break;
		  }
		}
		if (i != m_length)
		{
		  return Double.NaN;
		}

		try
		{
		  return (Convert.ToDouble(valueString));
		}
		catch (System.FormatException)
		{
		  // This should catch double periods, empty strings.
		  return Double.NaN;
		}
	  }
	}

}