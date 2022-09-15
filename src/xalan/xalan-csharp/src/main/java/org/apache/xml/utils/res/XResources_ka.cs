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
 * $Id: XResources_ka.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Georgian resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_ka : XResourceBundle
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
				new object[] {"ui_language", "ka"},
				new object[] {"help_language", "ka"},
				new object[] {"language", "ka"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x10D0, (char)0x10D1, (char)0x10D2, (char)0x10D3, (char)0x10D4, (char)0x10D5, (char)0x10D6, (char)0x10f1, (char)0x10D7, (char)0x10D8, (char)0x10D9, (char)0x10DA, (char)0x10DB, (char)0x10DC, (char)0x10f2, (char)0x10DD, (char)0x10DE, (char)0x10DF, (char)0x10E0, (char)0x10E1, (char)0x10E2, (char)0x10E3, (char)0x10E4, (char)0x10E5, (char)0x10E6, (char)0x10E7, (char)0x10E8, (char)0x10E9, (char)0x10EA, (char)0x10EB, (char)0x10EC, (char)0x10ED, (char)0x10EE, (char)0x10F4, (char)0x10EF, (char)0x10F0})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "additive"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1000, 100, 10, 1})},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0x10D0, (char)0x10D1, (char)0x10D2, (char)0x10D3, (char)0x10D4, (char)0x10D5, (char)0x10D6, (char)0x10f1, (char)0x10D7})},
				new object[] {"tens", new CharArrayWrapper(new char[]{(char)0x10D8, (char)0x10D9, (char)0x10DA, (char)0x10DB, (char)0x10DC, (char)0x10f2, (char)0x10DD, (char)0x10DE, (char)0x10DF})},
				new object[] {"hundreds", new CharArrayWrapper(new char[]{(char)0x10E0, (char)0x10E1, (char)0x10E2, (char)0x10E3, (char)0x10E4, (char)0x10E5, (char)0x10E6, (char)0x10E7, (char)0x10E8})},
				new object[] {"thousands", new CharArrayWrapper(new char[]{(char)0x10E9, (char)0x10EA, (char)0x10EB, (char)0x10EC, (char)0x10ED, (char)0x10EE, (char)0x10F4, (char)0x10EF, (char)0x10F0})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"thousands", "hundreds", "tens", "digits"})}
			};
		  }
	  }
	}

}