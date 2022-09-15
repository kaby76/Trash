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
				new object[] {"alphabet", new CharArrayWrapper(new char[]{0x10D0, 0x10D1, 0x10D2, 0x10D3, 0x10D4, 0x10D5, 0x10D6, 0x10f1, 0x10D7, 0x10D8, 0x10D9, 0x10DA, 0x10DB, 0x10DC, 0x10f2, 0x10DD, 0x10DE, 0x10DF, 0x10E0, 0x10E1, 0x10E2, 0x10E3, 0x10E4, 0x10E5, 0x10E6, 0x10E7, 0x10E8, 0x10E9, 0x10EA, 0x10EB, 0x10EC, 0x10ED, 0x10EE, 0x10F4, 0x10EF, 0x10F0})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "additive"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1000, 100, 10, 1})},
				new object[] {"digits", new CharArrayWrapper(new char[]{0x10D0, 0x10D1, 0x10D2, 0x10D3, 0x10D4, 0x10D5, 0x10D6, 0x10f1, 0x10D7})},
				new object[] {"tens", new CharArrayWrapper(new char[]{0x10D8, 0x10D9, 0x10DA, 0x10DB, 0x10DC, 0x10f2, 0x10DD, 0x10DE, 0x10DF})},
				new object[] {"hundreds", new CharArrayWrapper(new char[]{0x10E0, 0x10E1, 0x10E2, 0x10E3, 0x10E4, 0x10E5, 0x10E6, 0x10E7, 0x10E8})},
				new object[] {"thousands", new CharArrayWrapper(new char[]{0x10E9, 0x10EA, 0x10EB, 0x10EC, 0x10ED, 0x10EE, 0x10F4, 0x10EF, 0x10F0})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"thousands", "hundreds", "tens", "digits"})}
			};
		  }
	  }
	}

}