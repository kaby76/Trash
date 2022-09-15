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
 * $Id: XResources_ja_JP_HI.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{
	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Japanese (Hiragana) resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_ja_JP_HI : XResourceBundle
	{

	  /// <summary>
	  /// Get the association table for this resource.
	  /// 
	  /// </summary>
	  /// <returns> the association table for this resource. </returns>
	  public override object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ui_language", "ja"},
				new object[] {"help_language", "ja"},
				new object[] {"language", "ja"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x3044, (char)0x308d, (char)0x306f, (char)0x306b, (char)0x307b, (char)0x3078, (char)0x3068, (char)0x3061, (char)0x308a, (char)0x306c, (char)0x308b, (char)0x3092, (char)0x308f, (char)0x304b, (char)0x3088, (char)0x305f, (char)0x308c, (char)0x305d, (char)0x3064, (char)0x306d, (char)0x306a, (char)0x3089, (char)0x3080, (char)0x3046, (char)0x3090, (char)0x306e, (char)0x304a, (char)0x304f, (char)0x3084, (char)0x307e, (char)0x3051, (char)0x3075, (char)0x3053, (char)0x3048, (char)0x3066, (char)0x3042, (char)0x3055, (char)0x304d, (char)0x3086, (char)0x3081, (char)0x307f, (char)0x3057, (char)0x3091, (char)0x3072, (char)0x3082, (char)0x305b, (char)0x3059})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "multiplicative-additive"},
				new object[] {"multiplierOrder", "follows"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1})},
				new object[] {"multiplier", new LongArrayWrapper(new long[]{long.MaxValue, long.MaxValue, 100000000, 10000, 1000, 100, 10})},
				new object[] {"multiplierChar", new CharArrayWrapper(new char[]{(char)0x4EAC, (char)0x5146, (char)0x5104, (char)0x4E07, (char)0x5343, (char)0x767e, (char)0x5341})},
				new object[] {"zero", new CharArrayWrapper(new char[0])},
				new object[] {"digits", new CharArrayWrapper(new char[]{(char)0x4E00, (char)0x4E8C, (char)0x4E09, (char)0x56DB, (char)0x4E94, (char)0x516D, (char)0x4E03, (char)0x516B, (char)0x4E5D})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"digits"})}
			};
		  }
	  }
	}

}