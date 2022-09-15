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
 * $Id: XResources_hy.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Armenian resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_hy : XResourceBundle
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
				new object[] {"ui_language", "hy"},
				new object[] {"help_language", "hy"},
				new object[] {"language", "hy"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x0561, (char)0x0562, (char)0x0563, (char)0x0564, (char)0x0565, (char)0x0566, (char)0x0567, (char)0x0568, (char)0x0569, (char)0x056A, (char)0x056B, (char)0x056C, (char)0x056D, (char)0x056E, (char)0x056F, (char)0x0567, (char)0x0568, (char)0x0572, (char)0x0573, (char)0x0574, (char)0x0575, (char)0x0576, (char)0x0577, (char)0x0578, (char)0x0579, (char)0x057A, (char)0x057B, (char)0x057C, (char)0x057D, (char)0x057E, (char)0x057F, (char)0x0580, (char)0x0581, (char)0x0582, (char)0x0583, (char)0x0584})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "additive"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1000, 100, 10, 1})},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0x0561, (char)0x0562, (char)0x0563, (char)0x0564, (char)0x0565, (char)0x0566, (char)0x0567, (char)0x0568, (char)0x0569})},
				new object[] {"tens", new CharArrayWrapper(new char[]{(char)0x056A, (char)0x056B, (char)0x056C, (char)0x056D, (char)0x056E, (char)0x056F, (char)0x0567, (char)0x0568, (char)0x0572})},
				new object[] {"hundreds", new CharArrayWrapper(new char[]{(char)0x0573, (char)0x0574, (char)0x0575, (char)0x0576, (char)0x0577, (char)0x0578, (char)0x0579, (char)0x057A, (char)0x057B})},
				new object[] {"thousands", new CharArrayWrapper(new char[]{(char)0x057C, (char)0x057D, (char)0x057E, (char)0x057F, (char)0x0580, (char)0x0581, (char)0x0582, (char)0x0583, (char)0x0584})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"thousands", "hundreds", "tens", "digits"})}
			};
		  }
	  }
	}

}