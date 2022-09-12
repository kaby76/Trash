using System.Collections;

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
 * $Id: ExtensionHandlerJava.java 468637 2006-10-28 06:51:02Z minchau $
 */
namespace org.apache.xalan.extensions
{

	/// <summary>
	/// Abstract base class handling the java language extensions for XPath.
	/// This base class provides cache management shared by all of the
	/// various java extension handlers.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public abstract class ExtensionHandlerJava : ExtensionHandler
	{

	  /// <summary>
	  /// Extension class name </summary>
	  protected internal string m_className = "";

	  /// <summary>
	  /// Table of cached methods </summary>
	  private Hashtable m_cachedMethods = new Hashtable();

	  /// <summary>
	  /// Construct a new extension handler given all the information
	  /// needed.
	  /// </summary>
	  /// <param name="namespaceUri"> the extension namespace URI that I'm implementing </param>
	  /// <param name="funcNames">    string containing list of functions of extension NS </param>
	  /// <param name="lang">         language of code implementing the extension </param>
	  /// <param name="srcURL">       value of src attribute (if any) - treated as a URL
	  ///                     or a classname depending on the value of lang. If
	  ///                     srcURL is not null, then scriptSrc is ignored. </param>
	  /// <param name="scriptSrc">    the actual script code (if any) </param>
	  /// <param name="scriptLang">   the scripting language </param>
	  /// <param name="className">    the extension class name  </param>
	  protected internal ExtensionHandlerJava(string namespaceUri, string scriptLang, string className) : base(namespaceUri, scriptLang)
	  {


		m_className = className;
	  }

	  /// <summary>
	  /// Look up the entry in the method cache. </summary>
	  /// <param name="methodKey">   A key that uniquely identifies this invocation in
	  ///                    the stylesheet. </param>
	  /// <param name="objType">     A Class object or instance object representing the type </param>
	  /// <param name="methodArgs">  An array of the XObject arguments to be used for
	  ///                    function mangling.
	  /// </param>
	  /// <returns> The given method from the method cache </returns>
	  public virtual object getFromCache(object methodKey, object objType, object[] methodArgs)
	  {

		// Eventually, we want to insert code to mangle the methodKey with methodArgs
		return m_cachedMethods[methodKey];
	  }

	  /// <summary>
	  /// Add a new entry into the method cache. </summary>
	  /// <param name="methodKey">   A key that uniquely identifies this invocation in
	  ///                    the stylesheet. </param>
	  /// <param name="objType">     A Class object or instance object representing the type </param>
	  /// <param name="methodArgs">  An array of the XObject arguments to be used for
	  ///                    function mangling. </param>
	  /// <param name="methodObj">   A Class object or instance object representing the method
	  /// </param>
	  /// <returns> The cached method object </returns>
	  public virtual object putToCache(object methodKey, object objType, object[] methodArgs, object methodObj)
	  {

		// Eventually, we want to insert code to mangle the methodKey with methodArgs
		return m_cachedMethods[methodKey] = methodObj;
	  }
	}

}