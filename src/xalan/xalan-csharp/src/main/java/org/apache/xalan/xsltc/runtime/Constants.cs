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
 * $Id: Constants.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// This class defines constants used by both the compiler and the 
	/// runtime system.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface Constants
	{

		public static int ANY = -1;
		public static int ATTRIBUTE = -2;
		public static int ROOT = DTM.ROOT_NODE;
		public static int TEXT = DTM.TEXT_NODE;
		public static int ELEMENT = DTM.ELEMENT_NODE;
		public static int COMMENT = DTM.COMMENT_NODE;
		public static int PROCESSING_INSTRUCTION = DTM.PROCESSING_INSTRUCTION_NODE;

		public static string XSLT_URI = "http://www.w3.org/1999/XSL/Transform";
		public static string NAMESPACE_FEATURE = "http://xml.org/sax/features/namespaces";

		public static string EMPTYSTRING = "";
		public static string XML_PREFIX = "xml";
		public static string XMLNS_PREFIX = "xmlns";
		public static string XMLNS_STRING = "xmlns:";
		public static string XMLNS_URI = "http://www.w3.org/2000/xmlns/";
	}

}