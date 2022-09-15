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
 * $Id: SourceTree.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{


	/// <summary>
	/// This object represents a Source Tree, and any associated
	/// information.
	/// @xsl.usage internal
	/// </summary>
	public class SourceTree
	{

	  /// <summary>
	  /// Constructor SourceTree
	  /// 
	  /// </summary>
	  /// <param name="root"> The root of the source tree, which may or may not be a 
	  /// <seealso cref="org.w3c.dom.Document"/> node. </param>
	  /// <param name="url"> The URI of the source tree. </param>
	  public SourceTree(int root, string url)
	  {
		m_root = root;
		m_url = url;
	  }

	  /// <summary>
	  /// The URI of the source tree. </summary>
	  public string m_url;

	  /// <summary>
	  /// The root of the source tree, which may or may not be a 
	  /// <seealso cref="org.w3c.dom.Document"/> node.  
	  /// </summary>
	  public int m_root;
	}

}