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
 * $Id: DecimalToRoman.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{
	/// <summary>
	/// Structure to help in converting integers to roman numerals
	/// @xsl.usage internal
	/// </summary>
	public class DecimalToRoman
	{

	  /// <summary>
	  /// Constructor DecimalToRoman
	  /// 
	  /// </summary>
	  /// <param name="postValue"> Minimum value for a given range of 
	  /// roman numbers </param>
	  /// <param name="postLetter"> Correspoding letter (roman) to postValue </param>
	  /// <param name="preValue"> Value of last prefixed number within 
	  /// that same range (i.e. IV if postval is 5 (V)) </param>
	  /// <param name="preLetter"> Correspoding letter(roman) to preValue </param>
	  public DecimalToRoman(long postValue, string postLetter, long preValue, string preLetter)
	  {

		this.m_postValue = postValue;
		this.m_postLetter = postLetter;
		this.m_preValue = preValue;
		this.m_preLetter = preLetter;
	  }

	  /// <summary>
	  /// Minimum value for a given range of roman numbers </summary>
	  public long m_postValue;

	  /// <summary>
	  /// Correspoding letter (roman) to m_postValue </summary>
	  public string m_postLetter;

	  /// <summary>
	  /// Value of last prefixed number within that same range </summary>
	  public long m_preValue;

	  /// <summary>
	  /// Correspoding letter (roman) to m_preValue </summary>
	  public string m_preLetter;
	}

}