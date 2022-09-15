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
 * $Id: XResources_ja_JP_I.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{

	//
	//  LangResources_en.properties
	//

	/// <summary>
	/// The Japanese (Katakana) resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_ja_JP_I : XResourceBundle
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
				new object[] {"alphabet", new CharArrayWrapper(new char[]{0x30a4, 0x30ed, 0x30cf, 0x30cb, 0x30db, 0x30d8, 0x30c8, 0x30c1, 0x30ea, 0x30cc, 0x30eb, 0x30f2, 0x30ef, 0x30ab, 0x30e8, 0x30bf, 0x30ec, 0x30bd, 0x30c4, 0x30cd, 0x30ca, 0x30e9, 0x30e0, 0x30a6, 0x30f0, 0x30ce, 0x30aa, 0x30af, 0x30e4, 0x30de, 0x30b1, 0x30d5, 0x30b3, 0x30a8, 0x30c6, 0x30a2, 0x30b5, 0x30ad, 0x30e6, 0x30e1, 0x30df, 0x30b7, 0x30f1, 0x30d2, 0x30e2, 0x30bb, 0x30b9})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "multiplicative-additive"},
				new object[] {"multiplierOrder", "follows"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{1})},
				new object[] {"multiplier", new LongArrayWrapper(new long[]{long.MaxValue, long.MaxValue, 100000000, 10000, 1000, 100, 10})},
				new object[] {"multiplierChar", new CharArrayWrapper(new char[]{0x4EAC, 0x5146, 0x5104, 0x4E07, 0x5343, 0x767e, 0x5341})},
				new object[] {"zero", new CharArrayWrapper(new char[0])},
				new object[] {"digits", new CharArrayWrapper(new char[]{0x4E00, 0x4E8C, 0x4E09, 0x56DB, 0x4E94, 0x516D, 0x4E03, 0x516B, 0x4E5D})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"digits"})}
			};
		  }
	  }
	}

}