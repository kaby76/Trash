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
 * $Id: XResources_ko.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_ko.properties
	//

	/// <summary>
	/// The Korean resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_ko : XResourceBundle
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
				new object[] {"ui_language", "ko"},
				new object[] {"help_language", "ko"},
				new object[] {"language", "ko"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x3131, (char)0x3134, (char)0x3137, (char)0x3139, (char)0x3141, (char)0x3142, (char)0x3145, (char)0x3147, (char)0x3148, (char)0x314a, (char)0x314b, (char)0x314c, (char)0x314d, (char)0x314e, (char)0x314f, (char)0x3151, (char)0x3153, (char)0x3155, (char)0x3157, (char)0x315b, (char)0x315c, (char)0x3160, (char)0x3161, (char)0x3163})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "multiplicative-additive"},
				new object[] {"multiplierOrder", "follows"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1})},
				new object[] {"zero", new CharArrayWrapper(new char[0])},
				new object[] {"multiplier", new LongArrayWrapper(new long[]{100000000, 10000, 1000, 100, 10})},
				new object[] {"multiplierChar", new CharArrayWrapper(new char[]{(char)0xc5b5, (char)0xb9cc, (char)0xcc9c, (char)0xbc31, (char)0xc2ed})},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0xc77c, (char)0xc774, (char)0xc0bc, (char)0xc0ac, (char)0xc624, (char)0xc721, (char)0xce60, (char)0xd314, (char)0xad6c})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"digits"})}
			};
		  }
	  }
	}

}