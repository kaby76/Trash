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
 * $Id: OutputPropertyUtils.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	/// <summary>
	/// This class contains some static methods that act as helpers when parsing a
	/// Java Property object.
	/// 
	/// This class is not a public API. 
	/// It is only public because it is used outside of this package.
	/// </summary>
	/// <seealso cref= java.util.Properties
	/// @xsl.usage internal </seealso>
	public sealed class OutputPropertyUtils
	{
		/// <summary>
		/// Searches for the boolean property with the specified key in the property list.
		/// If the key is not found in this property list, the default property list,
		/// and its defaults, recursively, are then checked. The method returns
		/// <code>false</code> if the property is not found, or if the value is other
		/// than "yes".
		/// </summary>
		/// <param name="key">   the property key. </param>
		/// <param name="props">   the list of properties that will be searched. </param>
		/// <returns>  the value in this property list as a boolean value, or false
		/// if null or not "yes". </returns>
		public static bool getBooleanProperty(string key, Properties props)
		{

			string s = props.getProperty(key);

			if (null == s || !s.Equals("yes"))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Searches for the int property with the specified key in the property list.
		/// If the key is not found in this property list, the default property list,
		/// and its defaults, recursively, are then checked. The method returns
		/// <code>false</code> if the property is not found, or if the value is other
		/// than "yes".
		/// </summary>
		/// <param name="key">   the property key. </param>
		/// <param name="props">   the list of properties that will be searched. </param>
		/// <returns>  the value in this property list as a int value, or 0
		/// if null or not a number. </returns>
		public static int getIntProperty(string key, Properties props)
		{

			string s = props.getProperty(key);

			if (null == s)
			{
				return 0;
			}
			else
			{
				return int.Parse(s);
			}
		}

	}

}