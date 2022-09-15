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
 * $Id: XResources_he.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Hebrew resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_he : XResourceBundle
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
				new object[] {"ui_language", "he"},
				new object[] {"help_language", "he"},
				new object[] {"language", "he"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x05D0, (char)0x05D1, (char)0x05D2, (char)0x05D3, (char)0x05D4, (char)0x05D5, (char)0x05D6, (char)0x05D7, (char)0x05D8, (char)0x05D9, (char)0x05DA, (char)0x05DB, (char)0x05DC, (char)0x05DD, (char)0x05DE, (char)0x05DF, (char)0x05E0, (char)0x05E1})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "RightToLeft"},
				new object[] {"numbering", "additive"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{10, 1})},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0x05D0, (char)0x05D1, (char)0x05D2, (char)0x05D3, (char)0x05D4, (char)0x05D5, (char)0x05D6, (char)0x05D7, (char)0x05D8})},
				new object[] {"tens", new CharArrayWrapper(new char[]{(char)0x05D9, (char)0x05DA, (char)0x05DB, (char)0x05DC, (char)0x05DD, (char)0x05DE, (char)0x05DF, (char)0x05E0, (char)0x05E1})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"tens", "digits"})}
			};
		  }
	  }
	}

}