using System;
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
 * $Id: CharInfo.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{


	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using SystemIDResolver = org.apache.xml.serializer.utils.SystemIDResolver;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using WrappedRuntimeException = org.apache.xml.serializer.utils.WrappedRuntimeException;

	/// <summary>
	/// This class provides services that tell if a character should have
	/// special treatement, such as entity reference substitution or normalization
	/// of a newline character.  It also provides character to entity reference
	/// lookup.
	/// 
	/// DEVELOPERS: See Known Issue in the constructor.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal sealed class CharInfo
	{
		/// <summary>
		/// Given a character, lookup a String to output (e.g. a decorated entity reference). </summary>
		private Hashtable m_charToString;

		/// <summary>
		/// The name of the HTML entities file.
		/// If specified, the file will be resource loaded with the default class loader.
		/// </summary>
		public static readonly string HTML_ENTITIES_RESOURCE = SerializerBase.PKG_NAME + ".HTMLEntities";

		/// <summary>
		/// The name of the XML entities file.
		/// If specified, the file will be resource loaded with the default class loader.
		/// </summary>
		public static readonly string XML_ENTITIES_RESOURCE = SerializerBase.PKG_NAME + ".XMLEntities";

		/// <summary>
		/// The horizontal tab character, which the parser should always normalize. </summary>
		internal const char S_HORIZONAL_TAB = (char)0x09;

		/// <summary>
		/// The linefeed character, which the parser should always normalize. </summary>
		internal const char S_LINEFEED = (char)0x0A;

		/// <summary>
		/// The carriage return character, which the parser should always normalize. </summary>
		internal const char S_CARRIAGERETURN = (char)0x0D;
		internal const char S_SPACE = (char)0x20;
		internal const char S_QUOTE = (char)0x22;
		internal const char S_LT = (char)0x3C;
		internal const char S_GT = (char)0x3E;
		internal const char S_NEL = (char)0x85;
		internal const char S_LINE_SEPARATOR = (char)0x2028;

		/// <summary>
		/// This flag is an optimization for HTML entities. It false if entities 
		/// other than quot (34), amp (38), lt (60) and gt (62) are defined
		/// in the range 0 to 127.
		/// @xsl.usage internal
		/// </summary>
		internal bool onlyQuotAmpLtGt;

		/// <summary>
		/// Copy the first 0,1 ... ASCII_MAX values into an array </summary>
		internal const int ASCII_MAX = 128;

		/// <summary>
		/// Array of values is faster access than a set of bits 
		/// to quickly check ASCII characters in attribute values,
		/// the value is true if the character in an attribute value
		/// should be mapped to a String. 
		/// </summary>
		private readonly bool[] shouldMapAttrChar_ASCII;

		/// <summary>
		/// Array of values is faster access than a set of bits 
		/// to quickly check ASCII characters in text nodes, 
		/// the value is true if the character in a text node
		/// should be mapped to a String. 
		/// </summary>
		private readonly bool[] shouldMapTextChar_ASCII;

		/// <summary>
		/// An array of bits to record if the character is in the set.
		/// Although information in this array is complete, the
		/// isSpecialAttrASCII array is used first because access to its values
		/// is common and faster.
		/// </summary>
		private readonly int[] array_of_bits;


		// 5 for 32 bit words,  6 for 64 bit words ...
		/*
		 * This constant is used to shift an integer to quickly
		 * calculate which element its bit is stored in.
		 * 5 for 32 bit words (int) ,  6 for 64 bit words (long)
		 */
		private const int SHIFT_PER_WORD = 5;

		/*
		 * A mask to get the low order bits which are used to
		 * calculate the value of the bit within a given word,
		 * that will represent the presence of the integer in the 
		 * set.
		 * 
		 * 0x1F for 32 bit words (int),
		 * or 0x3F for 64 bit words (long) 
		 */
		private const int LOW_ORDER_BITMASK = 0x1f;

		/*
		 * This is used for optimizing the lookup of bits representing
		 * the integers in the set. It is the index of the first element
		 * in the array array_of_bits[] that is not used.
		 */
		private int firstWordNotUsed;


		/// <summary>
		/// A base constructor just to explicitly create the fields,
		/// with the exception of m_charToString which is handled
		/// by the constructor that delegates base construction to this one.
		/// <para>
		/// m_charToString is not created here only for performance reasons,
		/// to avoid creating a Hashtable that will be replaced when
		/// making a mutable copy, <seealso cref="#mutableCopyOf(CharInfo)"/>. 
		/// 
		/// </para>
		/// </summary>
		private CharInfo()
		{
			this.array_of_bits = createEmptySetOfIntegers(65535);
			this.firstWordNotUsed = 0;
			this.shouldMapAttrChar_ASCII = new bool[ASCII_MAX];
			this.shouldMapTextChar_ASCII = new bool[ASCII_MAX];
			this.m_charKey = new CharKey();

			// Not set here, but in a constructor that uses this one
			// this.m_charToString =  new Hashtable();  

			this.onlyQuotAmpLtGt = true;


			return;
		}

		private CharInfo(string entitiesResource, string method, bool @internal) : this()
		{
			// call the default constructor to create the fields
			m_charToString = new Hashtable();

			ResourceBundle entities = null;
			bool noExtraEntities = true;

			// Make various attempts to interpret the parameter as a properties
			// file or resource file, as follows:
			//
			//   1) attempt to load .properties file using ResourceBundle
			//   2) try using the class loader to find the specified file a resource
			//      file
			//   3) try treating the resource a URI

			if (@internal)
			{
				try
				{
					// Load entity property files by using PropertyResourceBundle,
					// cause of security issure for applets
					entities = PropertyResourceBundle.getBundle(entitiesResource);
				}
				catch (Exception)
				{
				}
			}

			if (entities != null)
			{
				System.Collections.IEnumerator keys = entities.Keys;
				while (keys.MoveNext())
				{
					string name = (string) keys.Current;
					string value = entities.getString(name);
					int code = int.Parse(value);
					bool extra = defineEntity(name, (char) code);
					if (extra)
					{
						noExtraEntities = false;
					}
				}
			}
			else
			{
				System.IO.Stream @is = null;

				// Load user specified resource file by using URL loading, it
				// requires a valid URI as parameter
				try
				{
					if (@internal)
					{
						@is = typeof(CharInfo).getResourceAsStream(entitiesResource);
					}
					else
					{
						ClassLoader cl = ObjectFactory.findClassLoader();
						if (cl == null)
						{
							@is = ClassLoader.getSystemResourceAsStream(entitiesResource);
						}
						else
						{
							@is = cl.getResourceAsStream(entitiesResource);
						}

						if (@is == null)
						{
							try
							{
								URL url = new URL(entitiesResource);
								@is = url.openStream();
							}
							catch (Exception)
							{
							}
						}
					}

					if (@is == null)
					{
						throw new Exception(Utils.messages.createMessage(MsgKey.ER_RESOURCE_COULD_NOT_FIND, new object[] {entitiesResource, entitiesResource}));
					}

					// Fix Bugzilla#4000: force reading in UTF-8
					//  This creates the de facto standard that Xalan's resource 
					//  files must be encoded in UTF-8. This should work in all
					// JVMs.
					//
					// %REVIEW% KNOWN ISSUE: IT FAILS IN MICROSOFT VJ++, which
					// didn't implement the UTF-8 encoding. Theoretically, we should
					// simply let it fail in that case, since the JVM is obviously
					// broken if it doesn't support such a basic standard.  But
					// since there are still some users attempting to use VJ++ for
					// development, we have dropped in a fallback which makes a
					// second attempt using the platform's default encoding. In VJ++
					// this is apparently ASCII, which is subset of UTF-8... and
					// since the strings we'll be reading here are also primarily
					// limited to the 7-bit ASCII range (at least, in English
					// versions of Xalan), this should work well enough to keep us
					// on the air until we're ready to officially decommit from
					// VJ++.

					System.IO.StreamReader reader;
					try
					{
						reader = new System.IO.StreamReader(@is, Encoding.UTF8);
					}
					catch (UnsupportedEncodingException)
					{
						reader = new System.IO.StreamReader(@is);
					}

					string line = reader.ReadLine();

					while (!string.ReferenceEquals(line, null))
					{
						if (line.Length == 0 || line[0] == '#')
						{
							line = reader.ReadLine();

							continue;
						}

						int index = line.IndexOf(' ');

						if (index > 1)
						{
							string name = line.Substring(0, index);

							++index;

							if (index < line.Length)
							{
								string value = line.Substring(index);
								index = value.IndexOf(' ');

								if (index > 0)
								{
									value = value.Substring(0, index);
								}

								int code = int.Parse(value);

								bool extra = defineEntity(name, (char) code);
								if (extra)
								{
									noExtraEntities = false;
								}
							}
						}

						line = reader.ReadLine();
					}

					@is.Close();
				}
				catch (Exception e)
				{
					throw new Exception(Utils.messages.createMessage(MsgKey.ER_RESOURCE_COULD_NOT_LOAD, new object[] {entitiesResource, e.ToString(), entitiesResource, e.ToString()}));
				}
				finally
				{
					if (@is != null)
					{
						try
						{
							@is.Close();
						}
						catch (Exception)
						{
						}
					}
				}
			}

			onlyQuotAmpLtGt = noExtraEntities;

			/* Now that we've used get(ch) just above to initialize the
			 * two arrays we will change by adding a tab to the set of 
			 * special chars for XML (but not HTML!).
			 * We do this because a tab is always a
			 * special character in an XML attribute, 
			 * but only a special character in XML text 
			 * if it has an entity defined for it.
			 * This is the reason for this delay.
			 */
			if (Method.XML.Equals(method))
			{
				// We choose not to escape the quotation mark as &quot; in text nodes
				shouldMapTextChar_ASCII[S_QUOTE] = false;
			}

			if (Method.HTML.Equals(method))
			{
				// The XSLT 1.0 recommendation says 
				// "The html output method should not escape < characters occurring in attribute values."
				// So we don't escape '<' in an attribute for HTML
				shouldMapAttrChar_ASCII['<'] = false;

				// We choose not to escape the quotation mark as &quot; in text nodes.
				shouldMapTextChar_ASCII[S_QUOTE] = false;
			}
		}

		/// <summary>
		/// Defines a new character reference. The reference's name and value are
		/// supplied. Nothing happens if the character reference is already defined.
		/// <para>Unlike internal entities, character references are a string to single
		/// character mapping. They are used to map non-ASCII characters both on
		/// parsing and printing, primarily for HTML documents. '&amp;lt;' is an
		/// example of a character reference.</para>
		/// </summary>
		/// <param name="name"> The entity's name </param>
		/// <param name="value"> The entity's value </param>
		/// <returns> true if the mapping is not one of:
		/// <ul>
		/// <li> '<' to "&lt;"
		/// <li> '>' to "&gt;"
		/// <li> '&' to "&amp;"
		/// <li> '"' to "&quot;"
		/// </ul> </returns>
		private bool defineEntity(string name, char value)
		{
			StringBuilder sb = new StringBuilder("&");
			sb.Append(name);
			sb.Append(';');
			string entityString = sb.ToString();

			bool extra = defineChar2StringMapping(entityString, value);
			return extra;
		}

		/// <summary>
		/// A utility object, just used to map characters to output Strings,
		/// needed because a HashMap needs to map an object as a key, not a 
		/// Java primitive type, like a char, so this object gets around that
		/// and it is reusable.
		/// </summary>
		private readonly CharKey m_charKey;

		/// <summary>
		/// Map a character to a String. For example given
		/// the character '>' this method would return the fully decorated
		/// entity name "&lt;".
		/// Strings for entity references are loaded from a properties file,
		/// but additional mappings defined through calls to defineChar2String()
		/// are possible. Such entity reference mappings could be over-ridden.
		/// 
		/// This is reusing a stored key object, in an effort to avoid
		/// heap activity. Unfortunately, that introduces a threading risk.
		/// Simplest fix for now is to make it a synchronized method, or to give
		/// up the reuse; I see very little performance difference between them.
		/// Long-term solution would be to replace the hashtable with a sparse array
		/// keyed directly from the character's integer value; see DTM's
		/// string pool for a related solution.
		/// </summary>
		/// <param name="value"> The character that should be resolved to
		/// a String, e.g. resolve '>' to  "&lt;".
		/// </param>
		/// <returns> The String that the character is mapped to, or null if not found.
		/// @xsl.usage internal </returns>
		internal string getOutputStringForChar(char value)
		{
			// CharKey m_charKey = new CharKey(); //Alternative to synchronized
			m_charKey.Char = value;
			return (string) m_charToString[m_charKey];
		}

		/// <summary>
		/// Tell if the character argument that is from
		/// an attribute value has a mapping to a String.
		/// </summary>
		/// <param name="value"> the value of a character that is in an attribute value </param>
		/// <returns> true if the character should have any special treatment, 
		/// such as when writing out entity references.
		/// @xsl.usage internal </returns>
		internal bool shouldMapAttrChar(int value)
		{
			// for performance try the values in the boolean array first,
			// this is faster access than the BitSet for common ASCII values

			if (value < ASCII_MAX)
			{
				return shouldMapAttrChar_ASCII[value];
			}

			// rather than java.util.BitSet, our private
			// implementation is faster (and less general).
			return get(value);
		}

		/// <summary>
		/// Tell if the character argument that is from a 
		/// text node has a mapping to a String, for example
		/// to map '<' to "&lt;".
		/// </summary>
		/// <param name="value"> the value of a character that is in a text node </param>
		/// <returns> true if the character has a mapping to a String, 
		/// such as when writing out entity references.
		/// @xsl.usage internal </returns>
		internal bool shouldMapTextChar(int value)
		{
			// for performance try the values in the boolean array first,
			// this is faster access than the BitSet for common ASCII values

			if (value < ASCII_MAX)
			{
				return shouldMapTextChar_ASCII[value];
			}

			// rather than java.util.BitSet, our private
			// implementation is faster (and less general).
			return get(value);
		}



//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private static CharInfo getCharInfoBasedOnPrivilege(final String entitiesFileName, final String method, final boolean internal)
		private static CharInfo getCharInfoBasedOnPrivilege(string entitiesFileName, string method, bool @internal)
		{
				return (CharInfo) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass(entitiesFileName, method, @internal));
		}

		private class PrivilegedActionAnonymousInnerClass : PrivilegedAction
		{
			private string entitiesFileName;
			private string method;
			private bool @internal;

			public PrivilegedActionAnonymousInnerClass(string entitiesFileName, string method, bool @internal)
			{
				this.entitiesFileName = entitiesFileName;
				this.method = method;
				this.@internal = @internal;
			}

			public virtual object run()
			{
				return new CharInfo(entitiesFileName, method, @internal);
			}
		}

		/// <summary>
		/// Factory that reads in a resource file that describes the mapping of
		/// characters to entity references.
		/// 
		/// Resource files must be encoded in UTF-8 and have a format like:
		/// <pre>
		/// # First char # is a comment
		/// Entity numericValue
		/// quot 34
		/// amp 38
		/// </pre>
		/// (Note: Why don't we just switch to .properties files? Oct-01 -sc)
		/// </summary>
		/// <param name="entitiesResource"> Name of entities resource file that should
		/// be loaded, which describes that mapping of characters to entity references. </param>
		/// <param name="method"> the output method type, which should be one of "xml", "html", "text"...
		/// 
		/// @xsl.usage internal </param>
		internal static CharInfo getCharInfo(string entitiesFileName, string method)
		{
			CharInfo charInfo = (CharInfo) m_getCharInfoCache[entitiesFileName];
			if (charInfo != null)
			{
				return mutableCopyOf(charInfo);
			}

			// try to load it internally - cache
			try
			{
				charInfo = getCharInfoBasedOnPrivilege(entitiesFileName, method, true);
				// Put the common copy of charInfo in the cache, but return
				// a copy of it.
				m_getCharInfoCache[entitiesFileName] = charInfo;
				return mutableCopyOf(charInfo);
			}
			catch (Exception)
			{
			}

			// try to load it externally - do not cache
			try
			{
				return getCharInfoBasedOnPrivilege(entitiesFileName, method, false);
			}
			catch (Exception)
			{
			}

			string absoluteEntitiesFileName;

			if (entitiesFileName.IndexOf(':') < 0)
			{
				absoluteEntitiesFileName = SystemIDResolver.getAbsoluteURIFromRelative(entitiesFileName);
			}
			else
			{
				try
				{
					absoluteEntitiesFileName = SystemIDResolver.getAbsoluteURI(entitiesFileName, null);
				}
				catch (TransformerException te)
				{
					throw new WrappedRuntimeException(te);
				}
			}

			return getCharInfoBasedOnPrivilege(entitiesFileName, method, false);
		}

		/// <summary>
		/// Create a mutable copy of the cached one. </summary>
		/// <param name="charInfo"> The cached one.
		/// @return </param>
		private static CharInfo mutableCopyOf(CharInfo charInfo)
		{
			CharInfo copy = new CharInfo();

			int max = charInfo.array_of_bits.Length;
			Array.Copy(charInfo.array_of_bits,0,copy.array_of_bits,0,max);

			copy.firstWordNotUsed = charInfo.firstWordNotUsed;

			max = charInfo.shouldMapAttrChar_ASCII.Length;
			Array.Copy(charInfo.shouldMapAttrChar_ASCII,0,copy.shouldMapAttrChar_ASCII,0,max);

			max = charInfo.shouldMapTextChar_ASCII.Length;
			Array.Copy(charInfo.shouldMapTextChar_ASCII,0,copy.shouldMapTextChar_ASCII,0,max);

			// utility field copy.m_charKey is already created in the default constructor 

			copy.m_charToString = (Hashtable) charInfo.m_charToString.clone();

			copy.onlyQuotAmpLtGt = charInfo.onlyQuotAmpLtGt;

			return copy;
		}

		/// <summary>
		/// Table of user-specified char infos.
		/// The table maps entify file names (the name of the
		/// property file without the .properties extension)
		/// to CharInfo objects populated with entities defined in 
		/// corresponding property file.  
		/// </summary>
		private static Hashtable m_getCharInfoCache = new Hashtable();

		/// <summary>
		/// Returns the array element holding the bit value for the
		/// given integer </summary>
		/// <param name="i"> the integer that might be in the set of integers
		///  </param>
		private static int arrayIndex(int i)
		{
			return (i >> SHIFT_PER_WORD);
		}

		/// <summary>
		/// For a given integer in the set it returns the single bit
		/// value used within a given word that represents whether
		/// the integer is in the set or not.
		/// </summary>
		private static int bit(int i)
		{
			int ret = (1 << (i & LOW_ORDER_BITMASK));
			return ret;
		}

		/// <summary>
		/// Creates a new empty set of integers (characters) </summary>
		/// <param name="max"> the maximum integer to be in the set. </param>
		private int[] createEmptySetOfIntegers(int max)
		{
			firstWordNotUsed = 0; // an optimization

			int[] arr = new int[arrayIndex(max - 1) + 1];
				return arr;

		}

		/// <summary>
		/// Adds the integer (character) to the set of integers. </summary>
		/// <param name="i"> the integer to add to the set, valid values are 
		/// 0, 1, 2 ... up to the maximum that was specified at
		/// the creation of the set. </param>
		private void set(int i)
		{
			ASCIItextDirty = i;
			ASCIIattrDirty = i;

			int j = (i >> SHIFT_PER_WORD); // this word is used
			int k = j + 1;

			if (firstWordNotUsed < k) // for optimization purposes.
			{
				firstWordNotUsed = k;
			}

			array_of_bits[j] |= (1 << (i & LOW_ORDER_BITMASK));
		}


		/// <summary>
		/// Return true if the integer (character)is in the set of integers.
		/// 
		/// This implementation uses an array of integers with 32 bits per
		/// integer.  If a bit is set to 1 the corresponding integer is 
		/// in the set of integers.
		/// </summary>
		/// <param name="i"> an integer that is tested to see if it is the
		/// set of integers, or not. </param>
		private bool get(int i)
		{

			bool in_the_set = false;
			int j = (i >> SHIFT_PER_WORD); // wordIndex(i)
			// an optimization here, ... a quick test to see
			// if this integer is beyond any of the words in use
			if (j < firstWordNotUsed)
			{
				in_the_set = (array_of_bits[j] & (1 << (i & LOW_ORDER_BITMASK))) != 0; // 0L for 64 bit words
			}
			return in_the_set;
		}

		/// <summary>
		/// This method returns true if there are some non-standard mappings to
		/// entities other than quot, amp, lt, gt, and its only purpose is for
		/// performance. </summary>
		/// <param name="charToMap"> The value of the character that is mapped to a String </param>
		/// <param name="outputString"> The String to which the character is mapped, usually
		/// an entity reference such as "&lt;". </param>
		/// <returns> true if the mapping is not one of:
		/// <ul>
		/// <li> '<' to "&lt;"
		/// <li> '>' to "&gt;"
		/// <li> '&' to "&amp;"
		/// <li> '"' to "&quot;"
		/// </ul> </returns>
		private bool extraEntity(string outputString, int charToMap)
		{
			bool extra = false;
			if (charToMap < ASCII_MAX)
			{
				switch (charToMap)
				{
					case '"' : // quot
						if (!outputString.Equals("&quot;"))
						{
							extra = true;
						}
						break;
					case '&' : // amp
						if (!outputString.Equals("&amp;"))
						{
							extra = true;
						}
						break;
					case '<' : // lt
						if (!outputString.Equals("&lt;"))
						{
							extra = true;
						}
						break;
					case '>' : // gt
						if (!outputString.Equals("&gt;"))
						{
							extra = true;
						}
						break;
					default : // other entity in range 0 to 127
						extra = true;
					break;
				}
			}
			return extra;
		}

		/// <summary>
		/// If the character is in the ASCII range then
		/// mark it as needing replacement with
		/// a String on output if it occurs in a text node. </summary>
		/// <param name="ch"> </param>
		private int ASCIItextDirty
		{
			set
			{
				if (0 <= value && value < ASCII_MAX)
				{
					shouldMapTextChar_ASCII[value] = true;
				}
			}
		}

		/// <summary>
		/// If the character is in the ASCII range then
		/// mark it as needing replacement with
		/// a String on output if it occurs in a attribute value. </summary>
		/// <param name="ch"> </param>
		private int ASCIIattrDirty
		{
			set
			{
				if (0 <= value && value < ASCII_MAX)
				{
					shouldMapAttrChar_ASCII[value] = true;
				}
			}
		}


		/// <summary>
		/// Call this method to register a char to String mapping, for example
		/// to map '<' to "&lt;". </summary>
		/// <param name="outputString"> The String to map to. </param>
		/// <param name="inputChar"> The char to map from. </param>
		/// <returns> true if the mapping is not one of:
		/// <ul>
		/// <li> '<' to "&lt;"
		/// <li> '>' to "&gt;"
		/// <li> '&' to "&amp;"
		/// <li> '"' to "&quot;"
		/// </ul> </returns>
		internal bool defineChar2StringMapping(string outputString, char inputChar)
		{
			CharKey character = new CharKey(inputChar);
			m_charToString[character] = outputString;
			set(inputChar); // mark the character has having a mapping to a String

			bool extraMapping = extraEntity(outputString, inputChar);
			return extraMapping;

		}

		/// <summary>
		/// Simple class for fast lookup of char values, when used with
		/// hashtables.  You can set the char, then use it as a key.
		/// 
		/// @xsl.usage internal
		/// </summary>
		private class CharKey : object
		{

		  /// <summary>
		  /// String value </summary>
		  internal char m_char;

		  /// <summary>
		  /// Constructor CharKey
		  /// </summary>
		  /// <param name="key"> char value of this object. </param>
		  public CharKey(char key)
		  {
			m_char = key;
		  }

		  /// <summary>
		  /// Default constructor for a CharKey.
		  /// </summary>
		  /// <param name="key"> char value of this object. </param>
		  public CharKey()
		  {
		  }

		  /// <summary>
		  /// Get the hash value of the character.  
		  /// </summary>
		  /// <returns> hash value of the character. </returns>
		  public char Char
		  {
			  set
			  {
				m_char = value;
			  }
		  }



		  /// <summary>
		  /// Get the hash value of the character.  
		  /// </summary>
		  /// <returns> hash value of the character. </returns>
		  public sealed override int GetHashCode()
		  {
			return (int)m_char;
		  }

		  /// <summary>
		  /// Override of equals() for this object 
		  /// </summary>
		  /// <param name="obj"> to compare to
		  /// </param>
		  /// <returns> True if this object equals this string value  </returns>
		  public sealed override bool Equals(object obj)
		  {
			return ((CharKey)obj).m_char == m_char;
		  }
		}


	}

}