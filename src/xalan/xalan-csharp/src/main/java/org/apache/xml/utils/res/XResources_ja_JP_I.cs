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
				new object[] {"alphabet", new CharArrayWrapper(new char[]{(char)0x30a4, (char)0x30ed, (char)0x30cf, (char)0x30cb, (char)0x30db, (char)0x30d8, (char)0x30c8, (char)0x30c1, (char)0x30ea, (char)0x30cc, (char)0x30eb, (char)0x30f2, (char)0x30ef, (char)0x30ab, (char)0x30e8, (char)0x30bf, (char)0x30ec, (char)0x30bd, (char)0x30c4, (char)0x30cd, (char)0x30ca, (char)0x30e9, (char)0x30e0, (char)0x30a6, (char)0x30f0, (char)0x30ce, (char)0x30aa, (char)0x30af, (char)0x30e4, (char)0x30de, (char)0x30b1, (char)0x30d5, (char)0x30b3, (char)0x30a8, (char)0x30c6, (char)0x30a2, (char)0x30b5, (char)0x30ad, (char)0x30e6, (char)0x30e1, (char)0x30df, (char)0x30b7, (char)0x30f1, (char)0x30d2, (char)0x30e2, (char)0x30bb, (char)0x30b9})},
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