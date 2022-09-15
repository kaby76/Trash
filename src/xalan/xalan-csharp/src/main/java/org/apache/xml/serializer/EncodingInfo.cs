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
 * $Id: EncodingInfo.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	/// <summary>
	/// Holds information about a given encoding, which is the Java name for the
	/// encoding, the equivalent ISO name.
	/// <para>
	/// An object of this type has two useful methods
	/// <pre>
	/// isInEncoding(char ch);
	/// </pre>
	/// which can be called if the character is not the high one in
	/// a surrogate pair and:
	/// <pre>
	/// isInEncoding(char high, char low);
	/// </pre>
	/// which can be called if the two characters from a high/low surrogate pair.
	/// </para>
	/// <para>
	/// An EncodingInfo object is a node in a binary search tree. Such a node
	/// will answer if a character is in the encoding, and do so for a given
	/// range of unicode values (<code>m_first</code> to
	/// <code>m_last</code>). It will handle a certain range of values
	/// explicitly (<code>m_explFirst</code> to <code>m_explLast</code>).
	/// If the unicode point is before that explicit range, that is it
	/// is in the range <code>m_first <= value < m_explFirst</code>, then it will delegate to another EncodingInfo object for The root
	/// of such a tree, m_before.  Likewise for values in the range 
	/// <code>m_explLast < value <= m_last</code>, but delgating to <code>m_after</code>
	/// </para>
	/// <para>
	/// Actually figuring out if a code point is in the encoding is expensive. So the
	/// purpose of this tree is to cache such determinations, and not to build the
	/// entire tree of information at the start, but only build up as much of the 
	/// tree as is used during the transformation.
	/// </para>
	/// <para>
	/// This Class is not a public API, and should only be used internally within
	/// the serializer.
	/// </para>
	/// <para>
	/// This class is not a public API.
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	public sealed class EncodingInfo : object
	{

		/// <summary>
		/// Not all characters in an encoding are in on contiguous group,
		/// however there is a lowest contiguous group starting at '\u0001'
		/// and working up to m_highCharInContiguousGroup.
		/// <para>
		/// This is the char for which chars at or below this value are 
		/// definately in the encoding, although for chars
		/// above this point they might be in the encoding.
		/// This exists for performance, especially for ASCII characters
		/// because for ASCII all chars in the range '\u0001' to '\u007F' 
		/// are in the encoding.
		/// 
		/// </para>
		/// </summary>
		private readonly char m_highCharInContiguousGroup;

		/// <summary>
		/// The ISO encoding name.
		/// </summary>
		internal readonly string name;

		/// <summary>
		/// The name used by the Java convertor.
		/// </summary>
		internal readonly string javaName;

		/// <summary>
		/// A helper object that we can ask if a
		/// single char, or a surrogate UTF-16 pair
		/// of chars that form a single character,
		/// is in this encoding.
		/// </summary>
		private InEncoding m_encoding;

		/// <summary>
		/// This is not a public API. It returns true if the
		/// char in question is in the encoding. </summary>
		/// <param name="ch"> the char in question.
		/// <para>
		/// This method is not a public API.
		/// @xsl.usage internal </param>
		public bool isInEncoding(char ch)
		{
			if (m_encoding == null)
			{
				m_encoding = new EncodingImpl(this);

				// One could put alternate logic in here to
				// instantiate another object that implements the
				// InEncoding interface. For example if the JRE is 1.4 or up
				// we could have an object that uses JRE 1.4 methods
			}
			return m_encoding.isInEncoding(ch);
		}

		/// <summary>
		/// This is not a public API. It returns true if the
		/// character formed by the high/low pair is in the encoding. </summary>
		/// <param name="high"> a char that the a high char of a high/low surrogate pair. </param>
		/// <param name="low"> a char that is the low char of a high/low surrogate pair.
		/// <para>
		/// This method is not a public API.
		/// @xsl.usage internal </param>
		public bool isInEncoding(char high, char low)
		{
			if (m_encoding == null)
			{
				m_encoding = new EncodingImpl(this);

				// One could put alternate logic in here to
				// instantiate another object that implements the
				// InEncoding interface. For example if the JRE is 1.4 or up
				// we could have an object that uses JRE 1.4 methods
			}
			return m_encoding.isInEncoding(high, low);
		}

		/// <summary>
		/// Create an EncodingInfo object based on the ISO name and Java name.
		/// If both parameters are null any character will be considered to
		/// be in the encoding. This is useful for when the serializer is in
		/// temporary output state, and has no assciated encoding.
		/// </summary>
		/// <param name="name"> reference to the ISO name. </param>
		/// <param name="javaName"> reference to the Java encoding name. </param>
		/// <param name="highChar"> The char for which characters at or below this value are 
		/// definately in the
		/// encoding, although for characters above this point they might be in the encoding. </param>
		public EncodingInfo(string name, string javaName, char highChar)
		{

			this.name = name;
			this.javaName = javaName;
			this.m_highCharInContiguousGroup = highChar;
		}



		/// <summary>
		/// A simple interface to isolate the implementation.
		/// We could also use some new JRE 1.4 methods in another implementation
		/// provided we use reflection with them.
		/// <para>
		/// This interface is not a public API,
		/// and should only be used internally within the serializer. 
		/// @xsl.usage internal
		/// </para>
		/// </summary>
		private interface InEncoding
		{
			/// <summary>
			/// Returns true if the char is in the encoding
			/// </summary>
			bool isInEncoding(char ch);
			/// <summary>
			/// Returns true if the high/low surrogate pair forms
			/// a character that is in the encoding.
			/// </summary>
			bool isInEncoding(char high, char low);
		}

		/// <summary>
		/// This class implements the 
		/// </summary>
		private class EncodingImpl : InEncoding
		{
			private readonly EncodingInfo outerInstance;




			public virtual bool isInEncoding(char ch1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean ret;
				bool ret;
				int codePoint = Encodings.toCodePoint(ch1);
				if (codePoint < m_explFirst)
				{
					// The unicode value is before the range
					// that we explictly manage, so we delegate the answer.

					// If we don't have an m_before object to delegate to, make one.
					if (m_before == null)
					{
						m_before = new EncodingImpl(outerInstance, m_encoding, m_first, m_explFirst - 1, codePoint);
					}
					ret = m_before.isInEncoding(ch1);
				}
				else if (m_explLast < codePoint)
				{
					// The unicode value is after the range
					// that we explictly manage, so we delegate the answer.

					// If we don't have an m_after object to delegate to, make one.
					if (m_after == null)
					{
						m_after = new EncodingImpl(outerInstance, m_encoding, m_explLast + 1, m_last, codePoint);
					}
					ret = m_after.isInEncoding(ch1);
				}
				else
				{
					// The unicode value is in the range we explitly handle
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idx = codePoint - m_explFirst;
					int idx = codePoint - m_explFirst;

					// If we already know the answer, just return it.
					if (m_alreadyKnown[idx])
					{
						ret = m_isInEncoding[idx];
					}
					else
					{
						// We don't know the answer, so find out,
						// which may be expensive, then cache the answer 
						ret = inEncoding(ch1, m_encoding);
						m_alreadyKnown[idx] = true;
						m_isInEncoding[idx] = ret;
					}
				}
				return ret;
			}

			public virtual bool isInEncoding(char high, char low)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean ret;
				bool ret;
				int codePoint = Encodings.toCodePoint(high,low);
				if (codePoint < m_explFirst)
				{
					// The unicode value is before the range
					// that we explictly manage, so we delegate the answer.

					// If we don't have an m_before object to delegate to, make one.
					if (m_before == null)
					{
						m_before = new EncodingImpl(outerInstance, m_encoding, m_first, m_explFirst - 1, codePoint);
					}
					ret = m_before.isInEncoding(high,low);
				}
				else if (m_explLast < codePoint)
				{
					// The unicode value is after the range
					// that we explictly manage, so we delegate the answer.

					// If we don't have an m_after object to delegate to, make one.
					if (m_after == null)
					{
						m_after = new EncodingImpl(outerInstance, m_encoding, m_explLast + 1, m_last, codePoint);
					}
					ret = m_after.isInEncoding(high,low);
				}
				else
				{
					// The unicode value is in the range we explitly handle
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idx = codePoint - m_explFirst;
					int idx = codePoint - m_explFirst;

					// If we already know the answer, just return it.
					if (m_alreadyKnown[idx])
					{
						ret = m_isInEncoding[idx];
					}
					else
					{
						// We don't know the answer, so find out,
						// which may be expensive, then cache the answer 
						ret = inEncoding(high, low, m_encoding);
						m_alreadyKnown[idx] = true;
						m_isInEncoding[idx] = ret;
					}
				}
				return ret;
			}

			/// <summary>
			/// The encoding.
			/// </summary>
			internal readonly string m_encoding;
			/// <summary>
			/// m_first through m_last is the range of unicode
			/// values that this object will return an answer on.
			/// It may delegate to a similar object with a different
			/// range
			/// </summary>
			internal readonly int m_first;

			/// <summary>
			/// m_explFirst through m_explLast is the range of unicode
			/// value that this object handles explicitly and does not
			/// delegate to a similar object.
			/// </summary>
			internal readonly int m_explFirst;
			internal readonly int m_explLast;
			internal readonly int m_last;

			/// <summary>
			/// The object, of the same type as this one,
			/// that handles unicode values in a range before
			/// the range explictly handled by this object, and
			/// to which this object may delegate.
			/// </summary>
			internal InEncoding m_before;
			/// <summary>
			/// The object, of the same type as this one,
			/// that handles unicode values in a range after
			/// the range explictly handled by this object, and
			/// to which this object may delegate.
			/// </summary>
			internal InEncoding m_after;

			/// <summary>
			/// The number of unicode values explicitly handled
			/// by a single EncodingInfo object. This value is 
			/// tuneable, but is set to 128 because that covers the
			/// entire low range of ASCII type chars within a single
			/// object.
			/// </summary>
			internal const int RANGE = 128;

			/// <summary>
			/// A flag to record if we already know the answer
			/// for the given unicode value.
			/// </summary>
			internal readonly bool[] m_alreadyKnown = new bool[RANGE];
			/// <summary>
			/// A table holding the answer on whether the given unicode
			/// value is in the encoding.
			/// </summary>
			internal readonly bool[] m_isInEncoding = new bool[RANGE];

			internal EncodingImpl(EncodingInfo outerInstance) : this(outerInstance, outerInstance.javaName, 0, int.MaxValue, (char) 0)
			{
				// This object will answer whether any unicode value
				// is in the encoding, it handles values 0 through Integer.MAX_VALUE
				this.outerInstance = outerInstance;
			}

			internal EncodingImpl(EncodingInfo outerInstance, string encoding, int first, int last, int codePoint)
			{
				this.outerInstance = outerInstance;
				// Set the range of unicode values that this object manages
				// either explicitly or implicitly.
				m_first = first;
				m_last = last;

				// Set the range of unicode values that this object 
				// explicitly manages
				m_explFirst = codePoint;
				m_explLast = codePoint + (RANGE-1);

				m_encoding = encoding;

				if (!string.ReferenceEquals(outerInstance.javaName, null))
				{
					// Some optimization.
					if (0 <= m_explFirst && m_explFirst <= 127)
					{
						// This particular EncodingImpl explicitly handles
						// characters in the low range.
						if ("UTF8".Equals(outerInstance.javaName) || "UTF-16".Equals(outerInstance.javaName) || "ASCII".Equals(outerInstance.javaName) || "US-ASCII".Equals(outerInstance.javaName) || "Unicode".Equals(outerInstance.javaName) || "UNICODE".Equals(outerInstance.javaName) || outerInstance.javaName.StartsWith("ISO8859", StringComparison.Ordinal))
						{

							// Not only does this EncodingImpl object explicitly
							// handle chracters in the low range, it is
							// also one that we know something about, without
							// needing to call inEncoding(char ch, String encoding)
							// for this low range
							//
							// By initializing the table ahead of time
							// for these low values, we prevent the expensive
							// inEncoding(char ch, String encoding)
							// from being called, at least for these common
							// encodings.
							for (int unicode = 1; unicode < 127; unicode++)
							{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idx = unicode - m_explFirst;
								int idx = unicode - m_explFirst;
								if (0 <= idx && idx < RANGE)
								{
									m_alreadyKnown[idx] = true;
									m_isInEncoding[idx] = true;
								}
							}
						}
					}

					/* A little bit more than optimization.
					 * 
					 * We will say that any character is in the encoding if
					 * we don't have an encoding.
					 * This is meaningful when the serializer is being used
					 * in temporary output state, where we are not writing to
					 * the final output tree.  It is when writing to the
					 * final output tree that we need to worry about the output
					 * encoding
					 */
					if (string.ReferenceEquals(outerInstance.javaName, null))
					{
						for (int idx = 0; idx < m_alreadyKnown.Length; idx++)
						{
							m_alreadyKnown[idx] = true;
							m_isInEncoding[idx] = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// This is heart of the code that determines if a given character
		/// is in the given encoding. This method is probably expensive,
		/// and the answer should be cached.
		/// <para>
		/// This method is not a public API,
		/// and should only be used internally within the serializer.
		/// </para>
		/// </summary>
		/// <param name="ch"> the char in question, that is not a high char of
		/// a high/low surrogate pair. </param>
		/// <param name="encoding"> the Java name of the enocding.
		/// 
		/// @xsl.usage internal
		///  </param>
		private static bool inEncoding(char ch, string encoding)
		{
			bool isInEncoding;
			try
			{
				char[] cArray = new char[1];
				cArray[0] = ch;
				// Construct a String from the char 
				string s = new string(cArray);
				// Encode the String into a sequence of bytes 
				// using the given, named charset. 
				sbyte[] bArray = s.GetBytes(encoding);
				isInEncoding = inEncoding(ch, bArray);

			}
			catch (Exception)
			{
				isInEncoding = false;

				// If for some reason the encoding is null, e.g.
				// for a temporary result tree, we should just
				// say that every character is in the encoding.
				if (string.ReferenceEquals(encoding, null))
				{
					isInEncoding = true;
				}
			}
			return isInEncoding;
		}

		/// <summary>
		/// This is heart of the code that determines if a given high/low
		/// surrogate pair forms a character that is in the given encoding.
		/// This method is probably expensive, and the answer should be cached. 
		/// <para>
		/// This method is not a public API,
		/// and should only be used internally within the serializer.
		/// </para>
		/// </summary>
		/// <param name="high"> the high char of
		/// a high/low surrogate pair. </param>
		/// <param name="low"> the low char of a high/low surrogate pair. </param>
		/// <param name="encoding"> the Java name of the encoding.
		/// 
		/// @xsl.usage internal
		///  </param>
		private static bool inEncoding(char high, char low, string encoding)
		{
			bool isInEncoding;
			try
			{
				char[] cArray = new char[2];
				cArray[0] = high;
				cArray[1] = low;
				// Construct a String from the char 
				string s = new string(cArray);
				// Encode the String into a sequence of bytes 
				// using the given, named charset. 
				sbyte[] bArray = s.GetBytes(encoding);
				isInEncoding = inEncoding(high,bArray);
			}
			catch (Exception)
			{
				isInEncoding = false;
			}

			return isInEncoding;
		}

		/// <summary>
		/// This method is the core of determining if character
		/// is in the encoding. The method is not foolproof, because
		/// s.getBytes(encoding) has specified behavior only if the
		/// characters are in the specified encoding. However this
		/// method tries it's best. </summary>
		/// <param name="ch"> the char that was converted using getBytes, or
		/// the first char of a high/low pair that was converted. </param>
		/// <param name="data"> the bytes written out by the call to s.getBytes(encoding); </param>
		/// <returns> true if the character is in the encoding. </returns>
		private static bool inEncoding(char ch, sbyte[] data)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isInEncoding;
			bool isInEncoding;
			// If the string written out as data is not in the encoding,
			// the output is not specified according to the documentation
			// on the String.getBytes(encoding) method,
			// but we do our best here.        
			if (data == null || data.Length == 0)
			{
				isInEncoding = false;
			}
			else
			{
				if (data[0] == 0)
				{
					isInEncoding = false;
				}
				else if (data[0] == (sbyte)'?' && ch != '?')
				{
					isInEncoding = false;
				}
				/*
				 * else if (isJapanese) {
				 *   // isJapanese is really 
				 *   //   (    "EUC-JP".equals(javaName) 
				 *   //    ||  "EUC_JP".equals(javaName)
				 *  //     ||  "SJIS".equals(javaName)   )
				 * 
				 *   // Work around some bugs in JRE for Japanese
				 *   if(data[0] == 0x21)
				 *     isInEncoding = false;
				 *   else if (ch == 0xA5)
				 *     isInEncoding = false;
				 *   else
				 *     isInEncoding = true;
				 * }
				 */ 

				else
				{
					// We don't know for sure, but it looks like it is in the encoding
					isInEncoding = true;
				}
			}
			return isInEncoding;
		}

		/// <summary>
		/// This method exists for performance reasons.
		/// <para>
		/// Except for '\u0000', if a char is less than or equal to the value
		/// returned by this method then it in the encoding.
		/// </para>
		/// <para>
		/// The characters in an encoding are not contiguous, however
		/// there is a lowest group of chars starting at '\u0001' upto and
		/// including the char returned by this method that are all in the encoding.
		/// So the char returned by this method essentially defines the lowest
		/// contiguous group.
		/// </para>
		/// <para>
		/// chars above the value returned might be in the encoding, but 
		/// chars at or below the value returned are definately in the encoding.
		/// </para>
		/// <para>
		/// In any case however, the isInEncoding(char) method can be used
		/// regardless of the value of the char returned by this method.
		/// </para>
		/// <para>
		/// If the value returned is '\u0000' it means that every character must be tested
		/// with an isInEncoding method <seealso cref="isInEncoding(char)"/> or <seealso cref="isInEncoding(char, char)"/> 
		/// for surrogate pairs.
		/// </para>
		/// <para>
		/// This method is not a public API.
		/// @xsl.usage internal
		/// </para>
		/// </summary>
		public char HighChar
		{
			get
			{
				return m_highCharInContiguousGroup;
			}
		}

	}

}