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
 * $Id: OutputSettings.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class OutputSettings
	{

		private string _cdata_section_elements = null;
		private string _doctype_public = null;
		private string _encoding = null;
		private string _indent = null;
		private string _media_type = null;
		private string _method = null;
		private string _omit_xml_declaration = null;
		private string _standalone = null;
		private string _version = null;

		public Properties Properties
		{
			get
			{
			Properties properties = new Properties();
			return (properties);
			}
		}


	}

}