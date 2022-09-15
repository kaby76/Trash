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
 * $Id: XMLStringFactory.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{
	/// <summary>
	/// A concrete class that implements this interface creates XMLString objects.
	/// </summary>
	public abstract class XMLStringFactory
	{

	  /// <summary>
	  /// Create a new XMLString from a Java string.
	  /// 
	  /// </summary>
	  /// <param name="string"> Java String reference, which must be non-null.
	  /// </param>
	  /// <returns> An XMLString object that wraps the String reference. </returns>
	  public abstract XMLString newstr(string @string);

	  /// <summary>
	  /// Create a XMLString from a FastStringBuffer.
	  /// 
	  /// </summary>
	  /// <param name="string"> FastStringBuffer reference, which must be non-null. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array.
	  /// </param>
	  /// <returns> An XMLString object that wraps the FastStringBuffer reference. </returns>
	  public abstract XMLString newstr(FastStringBuffer @string, int start, int length);

	  /// <summary>
	  /// Create a XMLString from a FastStringBuffer.
	  /// 
	  /// </summary>
	  /// <param name="string"> FastStringBuffer reference, which must be non-null. </param>
	  /// <param name="start"> The start position in the array. </param>
	  /// <param name="length"> The number of characters to read from the array.
	  /// </param>
	  /// <returns> An XMLString object that wraps the FastStringBuffer reference. </returns>
	  public abstract XMLString newstr(char[] @string, int start, int length);

	  /// <summary>
	  /// Get a cheap representation of an empty string.
	  /// </summary>
	  /// <returns> An non-null reference to an XMLString that represents "". </returns>
	  public abstract XMLString emptystr();
	}

}