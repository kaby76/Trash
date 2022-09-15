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
 * $Id: XMLNSDecl.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	/// <summary>
	/// Represents an xmlns declaration
	/// </summary>
	[Serializable]
	public class XMLNSDecl // 20001009 jkess
	{
		internal const long serialVersionUID = 6710237366877605097L;

	  /// <summary>
	  /// Constructor XMLNSDecl
	  /// </summary>
	  /// <param name="prefix"> non-null reference to prefix, using "" for default namespace. </param>
	  /// <param name="uri"> non-null reference to namespace URI. </param>
	  /// <param name="isExcluded"> true if this namespace declaration should normally be excluded. </param>
	  public XMLNSDecl(string prefix, string uri, bool isExcluded)
	  {

		m_prefix = prefix;
		m_uri = uri;
		m_isExcluded = isExcluded;
	  }

	  /// <summary>
	  /// non-null reference to prefix, using "" for default namespace.
	  ///  @serial 
	  /// </summary>
	  private string m_prefix;

	  /// <summary>
	  /// Return the prefix. </summary>
	  /// <returns> The prefix that is associated with this URI, or null
	  /// if the XMLNSDecl is declaring the default namespace. </returns>
	  public virtual string Prefix
	  {
		  get
		  {
			return m_prefix;
		  }
	  }

	  /// <summary>
	  /// non-null reference to namespace URI.
	  ///  @serial  
	  /// </summary>
	  private string m_uri;

	  /// <summary>
	  /// Return the URI. </summary>
	  /// <returns> The URI that is associated with this declaration. </returns>
	  public virtual string URI
	  {
		  get
		  {
			return m_uri;
		  }
	  }

	  /// <summary>
	  /// true if this namespace declaration should normally be excluded.
	  ///  @serial  
	  /// </summary>
	  private bool m_isExcluded;

	  /// <summary>
	  /// Tell if this declaration should be excluded from the
	  /// result namespace.
	  /// </summary>
	  /// <returns> true if this namespace declaration should normally be excluded. </returns>
	  public virtual bool IsExcluded
	  {
		  get
		  {
			return m_isExcluded;
		  }
	  }
	}

}