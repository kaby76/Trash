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
 * $Id: XMLCharacterRecognizer.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// Class used to verify whether the specified <var>ch</var> 
	/// conforms to the XML 1.0 definition of whitespace. 
	/// @xsl.usage internal
	/// </summary>
	public class XMLCharacterRecognizer
	{

	  /// <summary>
	  /// Returns whether the specified <var>ch</var> conforms to the XML 1.0 definition
	  /// of whitespace.  Refer to <A href="http://www.w3.org/TR/1998/REC-xml-19980210#NT-S">
	  /// the definition of <CODE>S</CODE></A> for details. </summary>
	  /// <param name="ch"> Character to check as XML whitespace. </param>
	  /// <returns> =true if <var>ch</var> is XML whitespace; otherwise =false. </returns>
	  public static bool isWhiteSpace(char ch)
	  {
		return (ch == 0x20) || (ch == 0x09) || (ch == 0xD) || (ch == 0xA);
	  }

	  /// <summary>
	  /// Tell if the string is whitespace.
	  /// </summary>
	  /// <param name="ch"> Character array to check as XML whitespace. </param>
	  /// <param name="start"> Start index of characters in the array </param>
	  /// <param name="length"> Number of characters in the array </param>
	  /// <returns> True if the characters in the array are 
	  /// XML whitespace; otherwise, false. </returns>
	  public static bool isWhiteSpace(char[] ch, int start, int length)
	  {

		int end = start + length;

		for (int s = start; s < end; s++)
		{
		  if (!isWhiteSpace(ch[s]))
		  {
			return false;
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// Tell if the string is whitespace.
	  /// </summary>
	  /// <param name="buf"> StringBuffer to check as XML whitespace. </param>
	  /// <returns> True if characters in buffer are XML whitespace, false otherwise </returns>
	  public static bool isWhiteSpace(StringBuilder buf)
	  {

		int n = buf.Length;

		for (int i = 0; i < n; i++)
		{
		  if (!isWhiteSpace(buf[i]))
		  {
			return false;
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// Tell if the string is whitespace.
	  /// </summary>
	  /// <param name="s"> String to check as XML whitespace. </param>
	  /// <returns> True if characters in buffer are XML whitespace, false otherwise </returns>
	  public static bool isWhiteSpace(string s)
	  {

		if (null != s)
		{
		  int n = s.Length;

		  for (int i = 0; i < n; i++)
		  {
			if (!isWhiteSpace(s[i]))
			{
			  return false;
			}
		  }
		}

		return true;
	  }

	}

}