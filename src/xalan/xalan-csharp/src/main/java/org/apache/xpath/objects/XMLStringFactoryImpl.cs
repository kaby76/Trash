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
 * $Id: XMLStringFactoryImpl.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;

	/// <summary>
	/// Class XMLStringFactoryImpl creates XString versions of XMLStrings.
	/// @xsl.usage internal
	/// </summary>
	public class XMLStringFactoryImpl : XMLStringFactory
	{
	  /// <summary>
	  /// The XMLStringFactory to pass to DTM construction. </summary>
	  private static XMLStringFactory m_xstringfactory = new XMLStringFactoryImpl();

	  /// <summary>
	  /// Get the XMLStringFactory to pass to DTM construction.
	  /// 
	  /// </summary>
	  /// <returns> A never-null static reference to a String factory. </returns>
	  public static XMLStringFactory Factory
	  {
		  get
		  {
			return m_xstringfactory;
		  }
	  }

	  /// <summary>
	  /// Create a new XMLString from a Java string.
	  /// 
	  /// </summary>
	  /// <param name="string"> Java String reference, which must be non-null.
	  /// </param>
	  /// <returns> An XMLString object that wraps the String reference. </returns>
	  public override XMLString newstr(string @string)
	  {
		return new XString(@string);
	  }

	  /// <summary>
	  /// Create a XMLString from a FastStringBuffer.
	  /// 
	  /// </summary>
	  /// <param name="fsb"> FastStringBuffer reference, which must be non-null. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array.
	  /// </param>
	  /// <returns> An XMLString object that wraps the FastStringBuffer reference. </returns>
	  public override XMLString newstr(FastStringBuffer fsb, int start, int length)
	  {
		return new XStringForFSB(fsb, start, length);
	  }

	  /// <summary>
	  /// Create a XMLString from a FastStringBuffer.
	  /// 
	  /// </summary>
	  /// <param name="string"> FastStringBuffer reference, which must be non-null. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array.
	  /// </param>
	  /// <returns> An XMLString object that wraps the FastStringBuffer reference. </returns>
	  public override XMLString newstr(char[] @string, int start, int length)
	  {
		return new XStringForChars(@string, start, length);
	  }

	  /// <summary>
	  /// Get a cheap representation of an empty string.
	  /// </summary>
	  /// <returns> An non-null reference to an XMLString that represents "". </returns>
	  public override XMLString emptystr()
	  {
		return XString.EMPTYSTRING;
	  }

	}

}