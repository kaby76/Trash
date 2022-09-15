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
 * $Id: Method.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{
	/// <summary>
	/// This class defines the constants which are the names of the four default
	/// output methods.
	/// <para>
	/// The default output methods defined are:
	/// <ul>
	/// <li>XML
	/// <li>TEXT
	/// <li>HTML
	/// </ul>
	/// These constants can be used as an argument to the
	/// OutputPropertiesFactory.getDefaultMethodProperties() method to get
	/// the properties to create a serializer.
	/// 
	/// This class is a public API.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref="OutputPropertiesFactory"/>
	/// <seealso cref="Serializer"
	/// 
	/// @xsl.usage general/>
	public sealed class Method
	{
		/// <summary>
		/// A private constructor to prevent the creation of such a class.
		/// </summary>
		private Method()
		{

		}

	  /// <summary>
	  /// The output method type for XML documents: <tt>xml</tt>.
	  /// </summary>
	  public const string XML = "xml";

	  /// <summary>
	  /// The output method type for HTML documents: <tt>html</tt>.
	  /// </summary>
	  public const string HTML = "html";

	  /// <summary>
	  /// The output method for XHTML documents: <tt>xhtml</tt>.
	  /// <para>
	  /// This method type is not currently supported.
	  /// </para>
	  /// </summary>
	  public const string XHTML = "xhtml";

	  /// <summary>
	  /// The output method type for text documents: <tt>text</tt>.
	  /// </summary>
	  public const string TEXT = "text";

	  /// <summary>
	  /// The "internal" method, just used when no method is 
	  /// specified in the style sheet, and a serializer of this type wraps either an
	  /// XML or HTML type (depending on the first tag in the output being html or
	  /// not)
	  /// </summary>
	  public const string UNKNOWN = "";
	}

}