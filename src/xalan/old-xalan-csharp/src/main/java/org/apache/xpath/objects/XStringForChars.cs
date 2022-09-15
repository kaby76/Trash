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
 * $Id: XStringForChars.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;


	/// <summary>
	/// This class will wrap a FastStringBuffer and allow for
	/// </summary>
	[Serializable]
	public class XStringForChars : XString
	{
		internal new const long serialVersionUID = -2235248887220850467L;
	  /// <summary>
	  /// The start position in the fsb. </summary>
	  internal int m_start;

	  /// <summary>
	  /// The length of the string. </summary>
	  internal int m_length;

	  protected internal string m_strCache = null;

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> FastStringBuffer object this will wrap, must be non-null. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array. </param>
	  public XStringForChars(char[] val, int start, int length) : base(val)
	  {
		m_start = start;
		m_length = length;
		if (null == val)
		{
		  throw new System.ArgumentException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, null)); //"The FastStringBuffer argument can not be null!!");
		}
	  }


	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> String object this will wrap. </param>
	  private XStringForChars(string val) : base(val)
	  {
		throw new System.ArgumentException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, null)); //"XStringForChars can not take a string for an argument!");
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public virtual FastStringBuffer fsb()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, null)); //"fsb() not supported for XStringForChars!");
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override void appendToFsb(FastStringBuffer fsb)
	  {
		fsb.append((char[])m_obj, m_start, m_length);
	  }


	  /// <summary>
	  /// Tell if this object contains a java String object.
	  /// </summary>
	  /// <returns> true if this XMLString can return a string without creating one. </returns>
	  public override bool hasString()
	  {
		return (null != m_strCache);
	  }


	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override string str()
	  {
		if (null == m_strCache)
		{
		  m_strCache = new string((char[])m_obj, m_start, m_length);
		}

		return m_strCache;
	  }


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
		ch.characters((char[])m_obj, m_start, m_length);
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
	  public override void dispatchAsComment(org.xml.sax.ext.LexicalHandler lh)
	  {
		lh.comment((char[])m_obj, m_start, m_length);
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
		return ((char[])m_obj)[index + m_start];
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
		Array.Copy((char[])m_obj, m_start + srcBegin, dst, dstBegin, srcEnd);
	  }

	}

}