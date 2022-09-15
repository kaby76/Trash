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
 * $Id: StopParseException.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// This is a special exception that is used to stop parsing when 
	/// search for an element.  For instance, when searching for xml:stylesheet 
	/// PIs, it is used to stop the parse once the document element is found. </summary>
	/// <seealso cref= StylesheetPIHandler
	/// @xsl.usage internal </seealso>
	public class StopParseException : org.xml.sax.SAXException
	{
			internal const long serialVersionUID = 210102479218258961L;
	  /// <summary>
	  /// Constructor StopParseException.
	  /// </summary>
	  internal StopParseException() : base("Stylesheet PIs found, stop the parse")
	  {
	  }
	}

}