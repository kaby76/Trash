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
				new object[] {"alphabet", new CharArrayWrapper(new char[]{0x0561, 0x0562, 0x0563, 0x0564, 0x0565, 0x0566, 0x0567, 0x0568, 0x0569, 0x056A, 0x056B, 0x056C, 0x056D, 0x056E, 0x056F, 0x0567, 0x0568, 0x0572, 0x0573, 0x0574, 0x0575, 0x0576, 0x0577, 0x0578, 0x0579, 0x057A, 0x057B, 0x057C, 0x057D, 0x057E, 0x057F, 0x0580, 0x0581, 0x0582, 0x0583, 0x0584})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "additive"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1000, 100, 10, 1})},
				new object[] {"digits", new CharArrayWrapper(new char[]{0x0561, 0x0562, 0x0563, 0x0564, 0x0565, 0x0566, 0x0567, 0x0568, 0x0569})},
				new object[] {"tens", new CharArrayWrapper(new char[]{0x056A, 0x056B, 0x056C, 0x056D, 0x056E, 0x056F, 0x0567, 0x0568, 0x0572})},
				new object[] {"hundreds", new CharArrayWrapper(new char[]{0x0573, 0x0574, 0x0575, 0x0576, 0x0577, 0x0578, 0x0579, 0x057A, 0x057B})},
				new object[] {"thousands", new CharArrayWrapper(new char[]{0x057C, 0x057D, 0x057E, 0x057F, 0x0580, 0x0581, 0x0582, 0x0583, 0x0584})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"thousands", "hundreds", "tens", "digits"})}
			};
		  }
	  }
	}

}