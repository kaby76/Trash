using System;

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
 * $Id: NameSpace.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// A representation of a namespace.  One of these will
	/// be pushed on the namespace stack for each
	/// element.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class NameSpace
	{
		internal const long serialVersionUID = 1471232939184881839L;

	  /// <summary>
	  /// Next NameSpace element on the stack.
	  ///  @serial             
	  /// </summary>
	  public NameSpace m_next = null;

	  /// <summary>
	  /// Prefix of this NameSpace element.
	  ///  @serial          
	  /// </summary>
	  public string m_prefix;

	  /// <summary>
	  /// Namespace URI of this NameSpace element.
	  ///  @serial           
	  /// </summary>
	  public string m_uri; // if null, then Element namespace is empty.

	  /// <summary>
	  /// Construct a namespace for placement on the
	  /// result tree namespace stack.
	  /// </summary>
	  /// <param name="prefix"> Prefix of this element </param>
	  /// <param name="uri"> URI of  this element </param>
	  public NameSpace(string prefix, string uri)
	  {
		m_prefix = prefix;
		m_uri = uri;
	  }
	}

}