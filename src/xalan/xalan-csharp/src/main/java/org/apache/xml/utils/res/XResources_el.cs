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
 * $Id: XResources_el.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Greek resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_el : XResourceBundle
	{

	  /// <summary>
	  /// Get the association list.
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public override object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ui_language", "el"},
				new object[] {"help_language", "el"},
				new object[] {"language", "el"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x03b1, (char)0x03b2, (char)0x03b3, (char)0x03b4, (char)0x03b5, (char)0x03b6, (char)0x03b7, (char)0x03b8, (char)0x03b9, (char)0x03ba, (char)0x03bb, (char)0x03bc, (char)0x03bd, (char)0x03be, (char)0x03bf, (char)0x03c0, (char)0x03c1, (char)0x03c2, (char)0x03c3, (char)0x03c4, (char)0x03c5, (char)0x03c6, (char)0x03c7, (char)0x03c8, (char)0x03c9})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "multiplicative-additive"},
				new object[] {"multiplierOrder", "precedes"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{100, 10, 1})},
				new object[] {"multiplier", new LongArrayWrapper(new long[]{1000})},
				new object[] {"multiplierChar", new CharArrayWrapper(new char[]{(char)0x03d9})},
				new object[] {"zero", new CharArrayWrapper(new char[0])},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0x03b1, (char)0x03b2, (char)0x03b3, (char)0x03b4, (char)0x03b5, (char)0x03db, (char)0x03b6, (char)0x03b7, (char)0x03b8})},
				new object[] {"tens", new CharArrayWrapper(new char[]{(char)0x03b9, (char)0x03ba, (char)0x03bb, (char)0x03bc, (char)0x03bd, (char)0x03be, (char)0x03bf, (char)0x03c0, (char)0x03df})},
				new object[] {"hundreds", new CharArrayWrapper(new char[]{(char)0x03c1, (char)0x03c2, (char)0x03c4, (char)0x03c5, (char)0x03c6, (char)0x03c7, (char)0x03c8, (char)0x03c9, (char)0x03e1})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"hundreds", "tens", "digits"})}
			};
		  }
	  }
	}

}