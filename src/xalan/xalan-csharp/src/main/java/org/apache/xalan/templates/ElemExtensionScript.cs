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
 * $Id: ElemExtensionScript.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	/// <summary>
	/// Implement Script extension element
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class ElemExtensionScript : ElemTemplateElement
	{
		internal new const long serialVersionUID = -6995978265966057744L;

	  /// <summary>
	  /// Constructor ElemExtensionScript
	  /// 
	  /// </summary>
	  public ElemExtensionScript()
	  {

		// System.out.println("ElemExtensionScript ctor");
	  }

	  /// <summary>
	  /// Language used in extension.
	  ///  @serial          
	  /// </summary>
	  private string m_lang = null;

	  /// <summary>
	  /// Set language used by extension
	  /// 
	  /// </summary>
	  /// <param name="v"> Language used by extension </param>
	  public virtual string Lang
	  {
		  set
		  {
			m_lang = value;
		  }
		  get
		  {
			return m_lang;
		  }
	  }


	  /// <summary>
	  /// Extension handler.
	  ///  @serial          
	  /// </summary>
	  private string m_src = null;

	  /// <summary>
	  /// Set Extension handler name for this extension
	  /// 
	  /// </summary>
	  /// <param name="v"> Extension handler name to set </param>
	  public virtual string Src
	  {
		  set
		  {
			m_src = value;
		  }
		  get
		  {
			return m_src;
		  }
	  }


	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element  </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_EXTENSIONSCRIPT;
		  }
	  }
	}

}