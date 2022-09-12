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
 * $Id: NodeTest.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface NodeTest
	{

		// generalized type
	}

	public static class NodeTest_Fields
	{
		public const int TEXT = org.apache.xml.dtm.DTM_Fields.TEXT_NODE;
		public const int COMMENT = org.apache.xml.dtm.DTM_Fields.COMMENT_NODE;
		public const int PI = org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE;
		public const int ROOT = org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE;
		public const int ELEMENT = org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE;
		public const int ATTRIBUTE = org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE;
		public const int GTYPE = org.apache.xml.dtm.DTM_Fields.NTYPES;
		public static readonly int ANODE = org.apache.xalan.xsltc.DOM_Fields.FIRST_TYPE - 1;
	}

}