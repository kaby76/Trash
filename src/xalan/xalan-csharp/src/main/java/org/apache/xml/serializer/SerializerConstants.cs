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
 * $Id: SerializerConstants.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{
	/// <summary>
	/// Constants used in serialization, such as the string "xmlns"
	/// @xsl.usage internal
	/// </summary>
	internal interface SerializerConstants
	{

		/// <summary>
		/// To insert ]]> in a CDATA section by ending the last CDATA section with
		/// ]] and starting the next CDATA section with >
		/// </summary>
		public static string CDATA_CONTINUE = "]]]]><![CDATA[>";
		/// <summary>
		/// The constant "]]>"
		/// </summary>
		public static string CDATA_DELIMITER_CLOSE = "]]>";
		public static string CDATA_DELIMITER_OPEN = "<![CDATA[";

		public static string EMPTYSTRING = "";

		public static string ENTITY_AMP = "&amp;";
		public static string ENTITY_CRLF = "&#xA;";
		public static string ENTITY_GT = "&gt;";
		public static string ENTITY_LT = "&lt;";
		public static string ENTITY_QUOT = "&quot;";

		public static string XML_PREFIX = "xml";
		public static string XMLNS_PREFIX = "xmlns";
		public static string XMLNS_URI = "http://www.w3.org/2000/xmlns/";

		public static string DEFAULT_SAX_SERIALIZER = SerializerBase.PKG_NAME + ".ToXMLSAXHandler";

		/// <summary>
		/// Define the XML version.
		/// </summary>
		public static string XMLVERSION11 = "1.1";
		public static string XMLVERSION10 = "1.0";
	}

}