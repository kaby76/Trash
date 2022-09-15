using System;
using System.Collections;
using System.IO;

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
 * $Id: Encodings.java 1225414 2011-12-29 02:38:30Z mrglavas $
 */
namespace org.apache.xml.serializer
{


	/// <summary>
	/// Provides information about encodings. Depends on the Java runtime
	/// to provides writers for the different encodings.
	/// <para>
	/// This class is not a public API. It is only public because it
	/// is used outside of this package.
	/// 
	/// @xsl.usage internal
	/// </para>
	/// </summary>

	public sealed class Encodings : object
	{
		/// <summary>
		/// Standard filename for properties file with encodings data.
		/// </summary>
		private static readonly string ENCODINGS_FILE = SerializerBase.PKG_PATH + "/Encodings.properties";

		/// <summary>
		/// Returns a writer for the specified encoding based on
		/// an output stream.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="output"> The output stream </param>
		/// <param name="encoding"> The encoding MIME name, not a Java name for the encoding. </param>
		/// <returns> A suitable writer </returns>
		/// <exception cref="UnsupportedEncodingException"> There is no convertor
		///  to support this encoding
		/// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static java.io.Writer getWriter(java.io.OutputStream output, String encoding) throws java.io.UnsupportedEncodingException
		internal static Writer getWriter(Stream output, string encoding)
		{

			for (int i = 0; i < _encodings.Length; ++i)
			{
				if (_encodings[i].name.Equals(encoding, StringComparison.OrdinalIgnoreCase))
				{
					try
					{
						string javaName = _encodings[i].javaName;
						StreamWriter osw = new StreamWriter(output, javaName);
						return osw;
					}
					catch (System.ArgumentException) // java 1.1.8
					{
						// keep trying
					}
					catch (UnsupportedEncodingException)
					{

						// keep trying
					}
				}
			}

			try
			{
				return new StreamWriter(output, encoding);
			}
			catch (System.ArgumentException) // java 1.1.8
			{
				throw new UnsupportedEncodingException(encoding);
			}
		}

		/// <summary>
		/// Returns the EncodingInfo object for the specified
		/// encoding, never null, although the encoding name 
		/// inside the returned EncodingInfo object will be if
		/// we can't find a "real" EncodingInfo for the encoding.
		/// <para>
		/// This is not a public API.
		/// 
		/// </para>
		/// </summary>
		/// <param name="encoding"> The encoding </param>
		/// <returns> The object that is used to determine if 
		/// characters are in the given encoding.
		/// @xsl.usage internal </returns>
		internal static EncodingInfo getEncodingInfo(string encoding)
		{
			EncodingInfo ei;

			string normalizedEncoding = toUpperCaseFast(encoding);
			ei = (EncodingInfo) _encodingTableKeyJava[normalizedEncoding];
			if (ei == null)
			{
				ei = (EncodingInfo) _encodingTableKeyMime[normalizedEncoding];
			}
			if (ei == null)
			{
				// We shouldn't have to do this, but just in case.
				ei = new EncodingInfo(null,null, '\u0000');
			}

			return ei;
		}

		/// <summary>
		/// Determines if the encoding specified was recognized by the
		/// serializer or not.
		/// </summary>
		/// <param name="encoding"> The encoding </param>
		/// <returns> boolean - true if the encoding was recognized else false </returns>
		public static bool isRecognizedEncoding(string encoding)
		{
			EncodingInfo ei;

			string normalizedEncoding = encoding.ToUpper();
			ei = (EncodingInfo) _encodingTableKeyJava[normalizedEncoding];
			if (ei == null)
			{
				ei = (EncodingInfo) _encodingTableKeyMime[normalizedEncoding];
			}
			if (ei != null)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// A fast and cheap way to uppercase a String that is
		/// only made of printable ASCII characters.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="s"> a String of ASCII characters </param>
		/// <returns> an uppercased version of the input String,
		/// possibly the same String.
		/// @xsl.usage internal </returns>
		private static string toUpperCaseFast(in string s)
		{

			bool different = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mx = s.length();
			int mx = s.Length;
			char[] chars = new char[mx];
			for (int i = 0; i < mx; i++)
			{
				char ch = s[i];
				// is the character a lower case ASCII one?
				if ('a' <= ch && ch <= 'z')
				{
					// a cheap and fast way to uppercase that is good enough
					ch = (char)(ch + ('A' - 'a'));
					different = true; // the uppercased String is different
				}
				chars[i] = ch;
			}

			// A little optimization, don't call String.valueOf() if
			// the uppercased string is the same as the input string.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String upper;
			string upper;
			if (different)
			{
				upper = new string(chars);
			}
			else
			{
				upper = s;
			}

			return upper;
		}

		/// <summary>
		/// The default encoding, ISO style, ISO style. </summary>
		internal const string DEFAULT_MIME_ENCODING = "UTF-8";

		/// <summary>
		/// Get the proper mime encoding.  From the XSLT recommendation: "The encoding
		/// attribute specifies the preferred encoding to use for outputting the result
		/// tree. XSLT processors are required to respect values of UTF-8 and UTF-16.
		/// For other values, if the XSLT processor does not support the specified
		/// encoding it may signal an error; if it does not signal an error it should
		/// use UTF-8 or UTF-16 instead. The XSLT processor must not use an encoding
		/// whose name does not match the EncName production of the XML Recommendation
		/// [XML]. If no encoding attribute is specified, then the XSLT processor should
		/// use either UTF-8 or UTF-16."
		/// <para>
		/// This is not a public API.
		/// 
		/// </para>
		/// </summary>
		/// <param name="encoding"> Reference to java-style encoding string, which may be null,
		/// in which case a default will be found.
		/// </param>
		/// <returns> The ISO-style encoding string, or null if failure.
		/// @xsl.usage internal </returns>
		internal static string getMimeEncoding(string encoding)
		{

			if (null == encoding)
			{
				try
				{

					// Get the default system character encoding.  This may be
					// incorrect if they passed in a writer, but right now there
					// seems to be no way to get the encoding from a writer.
					encoding = System.getProperty("file.encoding", "UTF8");

					if (null != encoding)
					{

						/*
						* See if the mime type is equal to UTF8.  If you don't
						* do that, then  convertJava2MimeEncoding will convert
						* 8859_1 to "ISO-8859-1", which is not what we want,
						* I think, and I don't think I want to alter the tables
						* to convert everything to UTF-8.
						*/
						string jencoding = (encoding.Equals("Cp1252", StringComparison.OrdinalIgnoreCase) || encoding.Equals("ISO8859_1", StringComparison.OrdinalIgnoreCase) || encoding.Equals("8859_1", StringComparison.OrdinalIgnoreCase) || encoding.Equals("UTF8", StringComparison.OrdinalIgnoreCase)) ? DEFAULT_MIME_ENCODING : convertJava2MimeEncoding(encoding);

						encoding = (null != jencoding) ? jencoding : DEFAULT_MIME_ENCODING;
					}
					else
					{
						encoding = DEFAULT_MIME_ENCODING;
					}
				}
				catch (SecurityException)
				{
					encoding = DEFAULT_MIME_ENCODING;
				}
			}
			else
			{
				encoding = convertJava2MimeEncoding(encoding);
			}

			return encoding;
		}

		/// <summary>
		/// Try the best we can to convert a Java encoding to a XML-style encoding.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="encoding"> non-null reference to encoding string, java style.
		/// </param>
		/// <returns> ISO-style encoding string.
		/// @xsl.usage internal </returns>
		private static string convertJava2MimeEncoding(string encoding)
		{
			EncodingInfo enc = (EncodingInfo) _encodingTableKeyJava[toUpperCaseFast(encoding)];
			if (null != enc)
			{
				return enc.name;
			}
			return encoding;
		}

		/// <summary>
		/// Try the best we can to convert a Java encoding to a XML-style encoding.
		/// <para>
		/// This is not a public API.
		/// 
		/// </para>
		/// </summary>
		/// <param name="encoding"> non-null reference to encoding string, java style.
		/// </param>
		/// <returns> ISO-style encoding string.
		/// <para>
		/// This method is not a public API.
		/// @xsl.usage internal </returns>
		public static string convertMime2JavaEncoding(string encoding)
		{

			for (int i = 0; i < _encodings.Length; ++i)
			{
				if (_encodings[i].name.Equals(encoding, StringComparison.OrdinalIgnoreCase))
				{
					return _encodings[i].javaName;
				}
			}

			return encoding;
		}

		/// <summary>
		/// Load a list of all the supported encodings.
		/// 
		/// System property "encodings" formatted using URL syntax may define an
		/// external encodings list. Thanks to Sergey Ushakov for the code
		/// contribution!
		/// @xsl.usage internal
		/// </summary>
		private static EncodingInfo[] loadEncodingInfo()
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream is;
				Stream @is;
				@is = SecuritySupport.getResourceAsStream(ObjectFactory.findClassLoader(), ENCODINGS_FILE);

				Properties props = new Properties();
				if (@is != null)
				{
					props.load(@is);
					@is.Close();
				}
				else
				{
					// Seems to be no real need to force failure here, let the
					// system do its best... The issue is not really very critical,
					// and the output will be in any case _correct_ though maybe not
					// always human-friendly... :)
					// But maybe report/log the resource problem?
					// Any standard ways to report/log errors (in static context)?
				}

				int totalEntries = props.size();

				System.Collections.IList encodingInfo_list = new ArrayList();
				System.Collections.IEnumerator keys = props.keys();
				for (int i = 0; i < totalEntries; ++i)
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					string javaName = (string) keys.nextElement();
					string val = props.getProperty(javaName);
					int len = lengthOfMimeNames(val);

					string mimeName;
					char highChar;
					if (len == 0)
					{
						// There is no property value, only the javaName, so try and recover
						mimeName = javaName;
						highChar = '\u0000'; // don't know the high code point, will need to test every character
					}
					else
					{
						try
						{
							// Get the substring after the Mime names
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String highVal = val.substring(len).trim();
							string highVal = val.Substring(len).Trim();
							highChar = (char) Integer.decode(highVal).intValue();
						}
						catch (System.FormatException)
						{
							highChar = (char)0;
						}
						string mimeNames = val.Substring(0, len);
						StringTokenizer st = new StringTokenizer(mimeNames, ",");
						for (bool first = true; st.hasMoreTokens(); first = false)
						{
							mimeName = st.nextToken();
							EncodingInfo ei = new EncodingInfo(mimeName, javaName, highChar);
							encodingInfo_list.Add(ei);
							_encodingTableKeyMime[mimeName.ToUpper()] = ei;
							if (first)
							{
								_encodingTableKeyJava[javaName.ToUpper()] = ei;
							}
						}
					}
				}
				// Convert the Vector of EncodingInfo objects into an array of them,
				// as that is the kind of thing this method returns.
				EncodingInfo[] ret_ei = new EncodingInfo[encodingInfo_list.Count];
				encodingInfo_list.toArray(ret_ei);
				return ret_ei;
			}
			catch (java.net.MalformedURLException mue)
			{
				throw new org.apache.xml.serializer.utils.WrappedRuntimeException(mue);
			}
			catch (java.io.IOException ioe)
			{
				throw new org.apache.xml.serializer.utils.WrappedRuntimeException(ioe);
			}
		}

		/// <summary>
		/// Get the length of the Mime names within the property value </summary>
		/// <param name="val"> The value of the property, which should contain a comma
		/// separated list of Mime names, followed optionally by a space and the
		/// high char value
		/// @return </param>
		private static int lengthOfMimeNames(string val)
		{
			// look for the space preceding the optional high char
			int len = val.IndexOf(' ');
			// If len is zero it means the optional part is not there, so
			// the value must be all Mime names, so set the length appropriately
			if (len < 0)
			{
				len = val.Length;
			}

			return len;
		}

		/// <summary>
		/// Return true if the character is the high member of a surrogate pair.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to test
		/// @xsl.usage internal </param>
		internal static bool isHighUTF16Surrogate(char ch)
		{
			return ('\uD800' <= ch && ch <= '\uDBFF');
		}
		/// <summary>
		/// Return true if the character is the low member of a surrogate pair.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="ch"> the character to test
		/// @xsl.usage internal </param>
		internal static bool isLowUTF16Surrogate(char ch)
		{
			return ('\uDC00' <= ch && ch <= '\uDFFF');
		}
		/// <summary>
		/// Return the unicode code point represented by the high/low surrogate pair.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="highSurrogate"> the high char of the high/low pair </param>
		/// <param name="lowSurrogate"> the low char of the high/low pair
		/// @xsl.usage internal </param>
		internal static int toCodePoint(char highSurrogate, char lowSurrogate)
		{
			int codePoint = ((highSurrogate - 0xd800) << 10) + (lowSurrogate - 0xdc00) + 0x10000;
			return codePoint;
		}
		/// <summary>
		/// Return the unicode code point represented by the char.
		/// A bit of a dummy method, since all it does is return the char,
		/// but as an int value.
		/// <para>
		/// This is not a public API.
		/// </para>
		/// </summary>
		/// <param name="ch"> the char.
		/// @xsl.usage internal </param>
		internal static int toCodePoint(char ch)
		{
			int codePoint = ch;
			return codePoint;
		}

		/// <summary>
		/// Characters with values at or below the high code point are
		/// in the encoding. Code point values above this one may or may
		/// not be in the encoding, but lower ones certainly are.
		/// <para>
		/// This is for performance.
		/// 
		/// </para>
		/// </summary>
		/// <param name="encoding"> The encoding </param>
		/// <returns> The code point for which characters at or below this code point
		/// are in the encoding. Characters with higher code point may or may not be
		/// in the encoding. A value of zero is returned if the high code point is unknown.
		/// <para>
		/// This method is not a public API.
		/// @xsl.usage internal </returns>
		public static char getHighChar(string encoding)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char highCodePoint;
			char highCodePoint;
			EncodingInfo ei;

			string normalizedEncoding = toUpperCaseFast(encoding);
			ei = (EncodingInfo) _encodingTableKeyJava[normalizedEncoding];
			if (ei == null)
			{
				ei = (EncodingInfo) _encodingTableKeyMime[normalizedEncoding];
			}
			if (ei != null)
			{
				highCodePoint = ei.HighChar;
			}
			else
			{
				highCodePoint = (char)0;
			}
			return highCodePoint;
		}

		private static readonly Hashtable _encodingTableKeyJava = new Hashtable();
		private static readonly Hashtable _encodingTableKeyMime = new Hashtable();
		private static readonly EncodingInfo[] _encodings = loadEncodingInfo();
	}

}