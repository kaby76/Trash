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
 * $Id: XResources_zh_TW.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Chinese(Taiwan) resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_zh_TW : XResourceBundle
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
				new object[] {"ui_language", "zh"},
				new object[] {"help_language", "zh"},
				new object[] {"language", "zh"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0xff21, (char)0xff22, (char)0xff23, (char)0xff24, (char)0xff25, (char)0xff26, (char)0xff27, (char)0xff28, (char)0xff29, (char)0xff2a, (char)0xff2b, (char)0xff2c, (char)0xff2d, (char)0xff2e, (char)0xff2f, (char)0xff30, (char)0xff31, (char)0xff32, (char)0xff33, (char)0xff34, (char)0xff35, (char)0xff36, (char)0xff37, (char)0xff38, (char)0xff39, (char)0xff3a})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "multiplicative-additive"},
				new object[] {"multiplierOrder", "follows"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1})},
				new object[] {"zero", new CharArrayWrapper(new char[]{(char)0x96f6})},
				new object[] {"multiplier", new LongArrayWrapper(new long[]{100000000, 10000, 1000, 100, 10})},
				new object[] {"multiplierChar", new CharArrayWrapper(new char[]{(char)0x5104, (char)0x842c, (char)0x4edf, (char)0x4f70, (char)0x62fe})},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0x58f9, (char)0x8cb3, (char)0x53c3, (char)0x8086, (char)0x4f0d, (char)0x9678, (char)0x67d2, (char)0x634c, (char)0x7396})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"digits"})}
			};
		  }
	  }
	}

}