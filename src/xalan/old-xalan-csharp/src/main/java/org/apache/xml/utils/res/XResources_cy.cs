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
 * $Id: XResources_cy.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{


	/// <summary>
	/// The Cyrillic resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResources_cy : XResourceBundle
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
				new object[] {"ui_language", "cy"},
				new object[] {"help_language", "cy"},
				new object[] {"language", "cy"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{0x0430, 0x0432, 0x0433, 0x0434, 0x0435, 0x0437, 0x0438, 0x0439, 0x04A9, 0x0457, 0x043A, 0x043B, 0x043C, 0x043D, 0x046F, 0x043E, 0x043F, 0x0447, 0x0440, 0x0441, 0x0442, 0x0443, 0x0444, 0x0445, 0x0470, 0x0460, 0x0446})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "multiplicative-additive"},
				new object[] {"multiplierOrder", "precedes"},
				new object[] {"numberGroups", new IntArrayWrapper(new int[]{100, 10, 1})},
				new object[] {"multiplier", new LongArrayWrapper(new long[]{1000})},
				new object[] {"multiplierChar", new CharArrayWrapper(new char[]{0x03D9})},
				new object[] {"zero", new CharArrayWrapper(new char[0])},
				new object[] {"digits", new CharArrayWrapper(new char[]{0x0430, 0x0432, 0x0433, 0x0434, 0x0435, 0x0437, 0x0438, 0x0439, 0x04A9})},
				new object[] {"tens", new CharArrayWrapper(new char[]{0x0457, 0x043A, 0x043B, 0x043C, 0x043D, 0x046F, 0x043E, 0x043F, 0x0447})},
				new object[] {"hundreds", new CharArrayWrapper(new char[]{0x0440, 0x0441, 0x0442, 0x0443, 0x0444, 0x0445, 0x0470, 0x0460, 0x0446})},
				new object[] {"tables", new StringArrayWrapper(new string[]{"hundreds", "tens", "digits"})}
			};
		  }
	  }
	}

}