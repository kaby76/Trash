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
 * $Id: DTMDOMException.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{
	/// <summary>
	/// Simple implementation of DOMException.
	/// 
	/// %REVIEW% Several classes were implementing this internally;
	/// it makes more sense to have one shared version.
	/// @xsl.usage internal
	/// </summary>
	public class DTMDOMException : org.w3c.dom.DOMException
	{
		internal const long serialVersionUID = 1895654266613192414L;
	  /// <summary>
	  /// Constructs a DOM/DTM exception.
	  /// </summary>
	  /// <param name="code"> </param>
	  /// <param name="message"> </param>
	  public DTMDOMException(short code, string message) : base(code, message)
	  {
	  }

	  /// <summary>
	  /// Constructor DTMDOMException
	  /// 
	  /// </summary>
	  /// <param name="code"> </param>
	  public DTMDOMException(short code) : base(code, "")
	  {
	  }
	}

}