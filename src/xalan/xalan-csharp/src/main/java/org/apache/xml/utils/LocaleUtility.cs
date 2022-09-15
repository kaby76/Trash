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
 * $Id: LocaleUtility.java 468655 2006-10-28 07:12:06Z minchau $
 */

namespace org.apache.xml.utils
{

	/// <summary>
	/// @author Igor Hersht, igorh@ca.ibm.com
	/// </summary>
	public class LocaleUtility
	{
		/// <summary>
		/// IETF RFC 1766 tag separator
		/// </summary>
		public const char IETF_SEPARATOR = '-';
		public const string EMPTY_STRING = "";


	 public static Locale langToLocale(string lang)
	 {
		   if ((string.ReferenceEquals(lang, null)) || lang.Equals(EMPTY_STRING))
		   { // not specified => getDefault
				return Locale.getDefault();
		   }
			string language = EMPTY_STRING;
			string country = EMPTY_STRING;
			string variant = EMPTY_STRING;

			int i1 = lang.IndexOf(IETF_SEPARATOR);
			if (i1 < 0)
			{
				language = lang;
			}
			else
			{
				language = lang.Substring(0, i1);
				++i1;
				int i2 = lang.IndexOf(IETF_SEPARATOR, i1);
				if (i2 < 0)
				{
					country = lang.Substring(i1);
				}
				else
				{
					country = lang.Substring(i1, i2 - i1);
					variant = lang.Substring(i2 + 1);
				}
			}

			if (language.Length == 2)
			{
			   language = language.ToLower();
			}
			else
			{
			  language = EMPTY_STRING;
			}

			if (country.Length == 2)
			{
			   country = country.ToUpper();
			}
			else
			{
			  country = EMPTY_STRING;
			}

			if ((variant.Length > 0) && ((language.Length == 2) || (country.Length == 2)))
			{
			   variant = variant.ToUpper();
			}
			else
			{
				variant = EMPTY_STRING;
			}

			return new Locale(language, country, variant);
	 }



	}



}