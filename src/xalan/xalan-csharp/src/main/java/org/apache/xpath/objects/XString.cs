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
 * $Id: XString.java 570108 2007-08-27 13:30:57Z zongaro $
 */
namespace org.apache.xpath.objects
{

	using DTM = org.apache.xml.dtm.DTM;
	using XMLCharacterRecognizer = org.apache.xml.utils.XMLCharacterRecognizer;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;

	/// <summary>
	/// This class represents an XPath string object, and is capable of
	/// converting the string to other types, such as a number.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class XString : XObject, XMLString
	{
		internal new const long serialVersionUID = 2020470518395094525L;

	  /// <summary>
	  /// Empty string XString object </summary>
	  public static readonly XString EMPTYSTRING = new XString("");

	  /// <summary>
	  /// Construct a XString object.  This constructor exists for derived classes.
	  /// </summary>
	  /// <param name="val"> String object this will wrap. </param>
	  protected internal XString(object val) : base(val)
	  {
	  }

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> String object this will wrap. </param>
	  public XString(string val) : base(val)
	  {
	  }

	  /// <summary>
	  /// Tell that this is a CLASS_STRING.
	  /// </summary>
	  /// <returns> type CLASS_STRING </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_STRING;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#STRING" </returns>
	  public override string TypeString
	  {
		  get
		  {
			return "#STRING";
		  }
	  }

	  /// <summary>
	  /// Tell if this object contains a java String object.
	  /// </summary>
	  /// <returns> true if this XMLString can return a string without creating one. </returns>
	  public virtual bool hasString()
	  {
		return true;
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> 0.0 if this string is null, numeric value of this string
	  /// or NaN </returns>
	  public override double num()
	  {
		return toDouble();
	  }

	  /// <summary>
	  /// Convert a string to a double -- Allowed input is in fixed
	  /// notation ddd.fff.
	  /// </summary>
	  /// <returns> A double value representation of the string, or return Double.NaN
	  /// if the string can not be converted. </returns>
	  public virtual double toDouble()
	  {
		/* XMLCharacterRecognizer.isWhiteSpace(char c) methods treats the following 
		 * characters as white space characters.
		 * ht - horizontal tab, nl - newline , cr - carriage return and sp - space
		 * trim() methods by default also takes care of these white space characters
		 * So trim() method is used to remove leading and trailing white spaces.
		 */
		XMLString s = trim();
		double result = Double.NaN;
		for (int i = 0; i < s.length(); i++)
		{
			char c = s.charAt(i);
		if (c != '-' && c != '.' && (c < 0X30 || c > 0x39))
		{
				// The character is not a '-' or a '.' or a digit
				// then return NaN because something is wrong.
				return result;
		}
		}
		try
		{
			result = double.Parse(s.ToString());
		}
		catch (System.FormatException)
		{
		}

		return result;
	  }

	  /// <summary>
	  /// Cast result object to a boolean.
	  /// </summary>
	  /// <returns> True if the length of this string object is greater
	  /// than 0. </returns>
	  public override bool @bool()
	  {
		return str().Length > 0;
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override XMLString xstr()
	  {
		return this;
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override string str()
	  {
		return (null != m_obj) ? ((string) m_obj) : "";
	  }

	  /// <summary>
	  /// Cast result object to a result tree fragment.
	  /// </summary>
	  /// <param name="support"> Xpath context to use for the conversion
	  /// </param>
	  /// <returns> A document fragment with this string as a child node </returns>
	  public override int rtf(XPathContext support)
	  {

		DTM frag = support.createDocumentFragment();

		frag.appendTextChild(str());

		return frag.Document;
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchCharactersEvents(org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
	  public override void dispatchCharactersEvents(org.xml.sax.ContentHandler ch)
	  {

		string str = str();

		ch.characters(str.ToCharArray(), 0, str.Length);
	  }

	  /// <summary>
	  /// Directly call the
	  /// comment method on the passed LexicalHandler for the
	  /// string-value.
	  /// </summary>
	  /// <param name="lh"> A non-null reference to a LexicalHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchAsComment(org.xml.sax.ext.LexicalHandler lh) throws org.xml.sax.SAXException
	  public virtual void dispatchAsComment(org.xml.sax.ext.LexicalHandler lh)
	  {

		string str = str();

		lh.comment(str.ToCharArray(), 0, str.Length);
	  }

	  /// <summary>
	  /// Returns the length of this string.
	  /// </summary>
	  /// <returns>  the length of the sequence of characters represented by this
	  ///          object. </returns>
	  public virtual int length()
	  {
		return str().Length;
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
	  public virtual char charAt(int index)
	  {
		return str()[index];
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
	  public virtual void getChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
	  {
		str().CopyTo(srcBegin, dst, dstBegin, srcEnd - srcBegin);
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

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.
		int t = obj2.Type;
		try
		{
			if (XObject.CLASS_NODESET == t)
			{
			  return obj2.Equals(this);
			}
			// If at least one object to be compared is a boolean, then each object 
			// to be compared is converted to a boolean as if by applying the 
			// boolean function. 
			else if (XObject.CLASS_BOOLEAN == t)
			{
				return obj2.@bool() == @bool();
			}
			// Otherwise, if at least one object to be compared is a number, then each object 
			// to be compared is converted to a number as if by applying the number function. 
			else if (XObject.CLASS_NUMBER == t)
			{
				return obj2.num() == num();
			}
		}
		catch (javax.xml.transform.TransformerException te)
		{
			throw new org.apache.xml.utils.WrappedRuntimeException(te);
		}

		// Otherwise, both objects to be compared are converted to strings as 
		// if by applying the string function. 
		return xstr().Equals(obj2.xstr());
	  }

	  /// <summary>
	  /// Compares this string to the specified <code>String</code>.
	  /// The result is <code>true</code> if and only if the argument is not
	  /// <code>null</code> and is a <code>String</code> object that represents
	  /// the same sequence of characters as this object.
	  /// </summary>
	  /// <param name="obj2">   the object to compare this <code>String</code> against. </param>
	  /// <returns>  <code>true</code> if the <code>String</code>s are equal;
	  ///          <code>false</code> otherwise. </returns>
	  /// <seealso cref=     java.lang.String#compareTo(java.lang.String) </seealso>
	  /// <seealso cref=     java.lang.String#equalsIgnoreCase(java.lang.String) </seealso>
	  public virtual bool Equals(string obj2)
	  {
		return str().Equals(obj2);
	  }

	  /// <summary>
	  /// Compares this string to the specified object.
	  /// The result is <code>true</code> if and only if the argument is not
	  /// <code>null</code> and is a <code>String</code> object that represents
	  /// the same sequence of characters as this object.
	  /// </summary>
	  /// <param name="obj2">   the object to compare this <code>String</code>
	  ///                     against. </param>
	  /// <returns>  <code>true</code> if the <code>String </code>are equal;
	  ///          <code>false</code> otherwise. </returns>
	  /// <seealso cref=     java.lang.String#compareTo(java.lang.String) </seealso>
	  /// <seealso cref=     java.lang.String#equalsIgnoreCase(java.lang.String) </seealso>
	  public virtual bool Equals(XMLString obj2)
	  {
		if (obj2 != null)
		{
		  if (!obj2.hasString())
		  {
			return obj2.Equals(str());
		  }
		  else
		  {
			return str().Equals(obj2.ToString());
		  }
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
	  ///                     against. </param>
	  /// <returns>  <code>true</code> if the <code>String </code>are equal;
	  ///          <code>false</code> otherwise. </returns>
	  /// <seealso cref=     java.lang.String#compareTo(java.lang.String) </seealso>
	  /// <seealso cref=     java.lang.String#equalsIgnoreCase(java.lang.String) </seealso>
	  public override bool Equals(object obj2)
	  {

		if (null == obj2)
		{
		  return false;
		}

		  // In order to handle the 'all' semantics of 
		  // nodeset comparisons, we always call the 
		  // nodeset function.
		else if (obj2 is XNodeSet)
		{
		  return obj2.Equals(this);
		}
		else if (obj2 is XNumber)
		{
			return obj2.Equals(this);
		}
		else
		{
		  return str().Equals(obj2.ToString());
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
	  /// <seealso cref=     #equals(Object) </seealso>
	  /// <seealso cref=     java.lang.Character#toLowerCase(char) </seealso>
	  /// <seealso cref= java.lang.Character#toUpperCase(char) </seealso>
	  public virtual bool equalsIgnoreCase(string anotherString)
	  {
		return str().Equals(anotherString, StringComparison.CurrentCultureIgnoreCase);
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
	  public virtual int compareTo(XMLString xstr)
	  {

		int len1 = this.length();
		int len2 = xstr.length();
		int n = Math.Min(len1, len2);
		int i = 0;
		int j = 0;

		while (n-- != 0)
		{
		  char c1 = this.charAt(i);
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
	  /// <param name="str">   the <code>String</code> to be compared. </param>
	  /// <returns>  a negative integer, zero, or a positive integer as the
	  ///          the specified String is greater than, equal to, or less
	  ///          than this String, ignoring case considerations. </returns>
	  /// <seealso cref=     java.text.Collator#compare(String, String)
	  /// @since   1.2 </seealso>
	  public virtual int compareToIgnoreCase(XMLString str)
	  {
		// %REVIEW%  Like it says, @since 1.2. Doesn't exist in earlier
		// versions of Java, hence we can't yet shell out to it. We can implement
		// it as character-by-character compare, but doing so efficiently
		// is likely to be (ahem) interesting.
		//  
		// However, since nobody is actually _using_ this method yet:
		//    return str().compareToIgnoreCase(str.toString());

		throw new org.apache.xml.utils.WrappedRuntimeException(new java.lang.NoSuchMethodException("Java 1.2 method, not yet implemented"));
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
	  public virtual bool startsWith(string prefix, int toffset)
	  {
		return str().StartsWith(prefix, toffset);
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
	  ///          <seealso cref="#equals(Object)"/> method. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>prefix</code> is
	  ///          <code>null</code>. </exception>
	  public virtual bool startsWith(string prefix)
	  {
		return startsWith(prefix, 0);
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
	  public virtual bool startsWith(XMLString prefix, int toffset)
	  {

		int to = toffset;
		int tlim = this.length();
		int po = 0;
		int pc = prefix.length();

		// Note: toffset might be near -1>>>1.
		if ((toffset < 0) || (toffset > tlim - pc))
		{
		  return false;
		}

		while (--pc >= 0)
		{
		  if (this.charAt(to) != prefix.charAt(po))
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
	  ///          <seealso cref="#equals(Object)"/> method. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>prefix</code> is
	  ///          <code>null</code>. </exception>
	  public virtual bool startsWith(XMLString prefix)
	  {
		return startsWith(prefix, 0);
	  }

	  /// <summary>
	  /// Tests if this string ends with the specified suffix.
	  /// </summary>
	  /// <param name="suffix">   the suffix. </param>
	  /// <returns>  <code>true</code> if the character sequence represented by the
	  ///          argument is a suffix of the character sequence represented by
	  ///          this object; <code>false</code> otherwise. Note that the
	  ///          result will be <code>true</code> if the argument is the
	  ///          empty string or is equal to this <code>String</code> object
	  ///          as determined by the <seealso cref="#equals(Object)"/> method. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>suffix</code> is
	  ///          <code>null</code>. </exception>
	  public virtual bool endsWith(string suffix)
	  {
		return str().EndsWith(suffix, StringComparison.Ordinal);
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
		return str().GetHashCode();
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
	  public virtual int indexOf(int ch)
	  {
		return str().IndexOf(ch);
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
	  public virtual int indexOf(int ch, int fromIndex)
	  {
		return str().IndexOf(ch, fromIndex);
	  }

	  /// <summary>
	  /// Returns the index within this string of the last occurrence of the
	  /// specified character. That is, the index returned is the largest
	  /// value <i>k</i> such that:
	  /// <blockquote><pre>
	  /// this.charAt(<i>k</i>) == ch
	  /// </pre></blockquote>
	  /// is true.
	  /// The String is searched backwards starting at the last character.
	  /// </summary>
	  /// <param name="ch">   a character. </param>
	  /// <returns>  the index of the last occurrence of the character in the
	  ///          character sequence represented by this object, or
	  ///          <code>-1</code> if the character does not occur. </returns>
	  public virtual int lastIndexOf(int ch)
	  {
		return str().LastIndexOf(ch);
	  }

	  /// <summary>
	  /// Returns the index within this string of the last occurrence of the
	  /// specified character, searching backward starting at the specified
	  /// index. That is, the index returned is the largest value <i>k</i>
	  /// such that:
	  /// <blockquote><pre>
	  /// this.charAt(k) == ch) && (k <= fromIndex)
	  /// </pre></blockquote>
	  /// is true.
	  /// </summary>
	  /// <param name="ch">          a character. </param>
	  /// <param name="fromIndex">   the index to start the search from. There is no
	  ///          restriction on the value of <code>fromIndex</code>. If it is
	  ///          greater than or equal to the length of this string, it has
	  ///          the same effect as if it were equal to one less than the
	  ///          length of this string: this entire string may be searched.
	  ///          If it is negative, it has the same effect as if it were -1:
	  ///          -1 is returned. </param>
	  /// <returns>  the index of the last occurrence of the character in the
	  ///          character sequence represented by this object that is less
	  ///          than or equal to <code>fromIndex</code>, or <code>-1</code>
	  ///          if the character does not occur before that point. </returns>
	  public virtual int lastIndexOf(int ch, int fromIndex)
	  {
		return str().LastIndexOf(ch, fromIndex);
	  }

	  /// <summary>
	  /// Returns the index within this string of the first occurrence of the
	  /// specified substring. The integer returned is the smallest value
	  /// <i>k</i> such that:
	  /// <blockquote><pre>
	  /// this.startsWith(str, <i>k</i>)
	  /// </pre></blockquote>
	  /// is <code>true</code>.
	  /// </summary>
	  /// <param name="str">   any string. </param>
	  /// <returns>  if the string argument occurs as a substring within this
	  ///          object, then the index of the first character of the first
	  ///          such substring is returned; if it does not occur as a
	  ///          substring, <code>-1</code> is returned. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>str</code> is
	  ///          <code>null</code>. </exception>
	  public virtual int indexOf(string str)
	  {
		return str().IndexOf(str, StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Returns the index within this string of the first occurrence of the
	  /// specified substring. The integer returned is the smallest value
	  /// <i>k</i> such that:
	  /// <blockquote><pre>
	  /// this.startsWith(str, <i>k</i>)
	  /// </pre></blockquote>
	  /// is <code>true</code>.
	  /// </summary>
	  /// <param name="str">   any string. </param>
	  /// <returns>  if the string argument occurs as a substring within this
	  ///          object, then the index of the first character of the first
	  ///          such substring is returned; if it does not occur as a
	  ///          substring, <code>-1</code> is returned. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>str</code> is
	  ///          <code>null</code>. </exception>
	  public virtual int indexOf(XMLString str)
	  {
		return str().IndexOf(str.ToString(), StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Returns the index within this string of the first occurrence of the
	  /// specified substring, starting at the specified index. The integer
	  /// returned is the smallest value <i>k</i> such that:
	  /// <blockquote><pre>
	  /// this.startsWith(str, <i>k</i>) && (<i>k</i> >= fromIndex)
	  /// </pre></blockquote>
	  /// is <code>true</code>.
	  /// <para>
	  /// There is no restriction on the value of <code>fromIndex</code>. If
	  /// it is negative, it has the same effect as if it were zero: this entire
	  /// string may be searched. If it is greater than the length of this
	  /// string, it has the same effect as if it were equal to the length of
	  /// this string: <code>-1</code> is returned.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="str">         the substring to search for. </param>
	  /// <param name="fromIndex">   the index to start the search from. </param>
	  /// <returns>  If the string argument occurs as a substring within this
	  ///          object at a starting index no smaller than
	  ///          <code>fromIndex</code>, then the index of the first character
	  ///          of the first such substring is returned. If it does not occur
	  ///          as a substring starting at <code>fromIndex</code> or beyond,
	  ///          <code>-1</code> is returned. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>str</code> is
	  ///          <code>null</code> </exception>
	  public virtual int indexOf(string str, int fromIndex)
	  {
		return str().IndexOf(str, fromIndex, StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Returns the index within this string of the rightmost occurrence
	  /// of the specified substring.  The rightmost empty string "" is
	  /// considered to occur at the index value <code>this.length()</code>.
	  /// The returned index is the largest value <i>k</i> such that
	  /// <blockquote><pre>
	  /// this.startsWith(str, k)
	  /// </pre></blockquote>
	  /// is true.
	  /// </summary>
	  /// <param name="str">   the substring to search for. </param>
	  /// <returns>  if the string argument occurs one or more times as a substring
	  ///          within this object, then the index of the first character of
	  ///          the last such substring is returned. If it does not occur as
	  ///          a substring, <code>-1</code> is returned. </returns>
	  /// <exception cref="java.lang.NullPointerException">  if <code>str</code> is
	  ///          <code>null</code>. </exception>
	  public virtual int lastIndexOf(string str)
	  {
		return str().LastIndexOf(str, StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Returns the index within this string of the last occurrence of
	  /// the specified substring.
	  /// </summary>
	  /// <param name="str">         the substring to search for. </param>
	  /// <param name="fromIndex">   the index to start the search from. There is no
	  ///          restriction on the value of fromIndex. If it is greater than
	  ///          the length of this string, it has the same effect as if it
	  ///          were equal to the length of this string: this entire string
	  ///          may be searched. If it is negative, it has the same effect
	  ///          as if it were -1: -1 is returned. </param>
	  /// <returns>  If the string argument occurs one or more times as a substring
	  ///          within this object at a starting index no greater than
	  ///          <code>fromIndex</code>, then the index of the first character of
	  ///          the last such substring is returned. If it does not occur as a
	  ///          substring starting at <code>fromIndex</code> or earlier,
	  ///          <code>-1</code> is returned. </returns>
	  /// <exception cref="java.lang.NullPointerException"> if <code>str</code> is
	  ///          <code>null</code>. </exception>
	  public virtual int lastIndexOf(string str, int fromIndex)
	  {
		return str().LastIndexOf(str, fromIndex, StringComparison.Ordinal);
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
	  public virtual XMLString substring(int beginIndex)
	  {
		return new XString(str().Substring(beginIndex));
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
	  public virtual XMLString substring(int beginIndex, int endIndex)
	  {
		return new XString(str().Substring(beginIndex, endIndex - beginIndex));
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
	  public virtual XMLString concat(string str)
	  {

		// %REVIEW% Make an FSB here?
		return new XString(str() + str);
	  }

	  /// <summary>
	  /// Converts all of the characters in this <code>String</code> to lower
	  /// case using the rules of the given <code>Locale</code>.
	  /// </summary>
	  /// <param name="locale"> use the case transformation rules for this locale </param>
	  /// <returns> the String, converted to lowercase. </returns>
	  /// <seealso cref=     java.lang.Character#toLowerCase(char) </seealso>
	  /// <seealso cref=     java.lang.String#toUpperCase(Locale) </seealso>
	  public virtual XMLString toLowerCase(Locale locale)
	  {
		return new XString(str().ToLower(locale));
	  }

	  /// <summary>
	  /// Converts all of the characters in this <code>String</code> to lower
	  /// case using the rules of the default locale, which is returned
	  /// by <code>Locale.getDefault</code>.
	  /// <para>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns>  the string, converted to lowercase. </returns>
	  /// <seealso cref=     java.lang.Character#toLowerCase(char) </seealso>
	  /// <seealso cref=     java.lang.String#toLowerCase(Locale) </seealso>
	  public virtual XMLString toLowerCase()
	  {
		return new XString(str().ToLower());
	  }

	  /// <summary>
	  /// Converts all of the characters in this <code>String</code> to upper
	  /// case using the rules of the given locale. </summary>
	  /// <param name="locale"> use the case transformation rules for this locale </param>
	  /// <returns> the String, converted to uppercase. </returns>
	  /// <seealso cref=     java.lang.Character#toUpperCase(char) </seealso>
	  /// <seealso cref=     java.lang.String#toLowerCase(Locale) </seealso>
	  public virtual XMLString toUpperCase(Locale locale)
	  {
		return new XString(str().ToUpper(locale));
	  }

	  /// <summary>
	  /// Converts all of the characters in this <code>String</code> to upper
	  /// case using the rules of the default locale, which is returned
	  /// by <code>Locale.getDefault</code>.
	  /// 
	  /// <para>
	  /// If no character in this string has a different uppercase version,
	  /// based on calling the <code>toUpperCase</code> method defined by
	  /// <code>Character</code>, then the original string is returned.
	  /// </para>
	  /// <para>
	  /// Otherwise, this method creates a new <code>String</code> object
	  /// representing a character sequence identical in length to the
	  /// character sequence represented by this <code>String</code> object and
	  /// with every character equal to the result of applying the method
	  /// <code>Character.toUpperCase</code> to the corresponding character of
	  /// </para>
	  /// this <code>String</code> object. <para>
	  /// Examples:
	  /// <blockquote><pre>
	  /// "Fahrvergn&uuml;gen".toUpperCase() returns "FAHRVERGN&Uuml;GEN"
	  /// "Visit Ljubinje!".toUpperCase() returns "VISIT LJUBINJE!"
	  /// </pre></blockquote>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <returns>  the string, converted to uppercase. </returns>
	  /// <seealso cref=     java.lang.Character#toUpperCase(char) </seealso>
	  /// <seealso cref=     java.lang.String#toUpperCase(Locale) </seealso>
	  public virtual XMLString toUpperCase()
	  {
		return new XString(str().ToUpper());
	  }

	  /// <summary>
	  /// Removes white space from both ends of this string.
	  /// </summary>
	  /// <returns>  this string, with white space removed from the front and end. </returns>
	  public virtual XMLString trim()
	  {
		return new XString(str().Trim());
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
	  public virtual XMLString fixWhiteSpace(bool trimHead, bool trimTail, bool doublePunctuationSpaces)
	  {

		// %OPT% !!!!!!!
		int len = this.length();
		char[] buf = new char[len];

		this.getChars(0, len, buf, 0);

		bool edit = false;
		int s;

		for (s = 0; s < len; s++)
		{
		  if (isSpace(buf[s]))
		  {
			break;
		  }
		}

		/* replace S to ' '. and ' '+ -> single ' '. */
		int d = s;
		bool pres = false;

		for (; s < len; s++)
		{
		  char c = buf[s];

		  if (isSpace(c))
		  {
			if (!pres)
			{
			  if (' ' != c)
			  {
				edit = true;
			  }

			  buf[d++] = ' ';

			  if (doublePunctuationSpaces && (s != 0))
			  {
				char prevChar = buf[s - 1];

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

		return edit ? xsf.newstr(new string(buf, start, d - start)) : this;
	  }

	  /// <seealso cref= org.apache.xpath.XPathVisitable#callVisitors(ExpressionOwner, XPathVisitor) </seealso>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  visitor.visitStringLiteral(owner, this);
	  }

	}

}