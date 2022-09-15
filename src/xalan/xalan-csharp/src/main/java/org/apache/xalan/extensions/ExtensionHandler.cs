using System;
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
 * $Id: ExtensionHandler.java 468637 2006-10-28 06:51:02Z minchau $
 */
namespace org.apache.xalan.extensions
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;

	/// <summary>
	/// Abstract base class for handling an extension namespace for XPath.
	/// Provides functions to test a function's existence and call a function.
	/// Also provides functions for calling an element and testing for
	/// an element's existence.
	/// 
	/// @author Sanjiva Weerawarana (sanjiva@watson.ibm.com)
	/// @xsl.usage internal
	/// </summary>
	public abstract class ExtensionHandler
	{

	  /// <summary>
	  /// uri of the extension namespace </summary>
	  protected internal string m_namespaceUri;

	  /// <summary>
	  /// scripting language of implementation </summary>
	  protected internal string m_scriptLang;

	  /// <summary>
	  /// This method loads a class using the context class loader if we're
	  /// running under Java2 or higher.
	  /// </summary>
	  /// <param name="className"> Name of the class to load </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static Class getClassForName(String className) throws ClassNotFoundException
	  internal static Type getClassForName(string className)
	  {
		// Hack for backwards compatibility with XalanJ1 stylesheets
		if (className.Equals("org.apache.xalan.xslt.extensions.Redirect"))
		{
		  className = "org.apache.xalan.lib.Redirect";
		}

		return ObjectFactory.findProviderClass(className, ObjectFactory.findClassLoader(), true);
	  }

	  /// <summary>
	  /// Construct a new extension namespace handler given all the information
	  /// needed.
	  /// </summary>
	  /// <param name="namespaceUri"> the extension namespace URI that I'm implementing </param>
	  /// <param name="scriptLang">   language of code implementing the extension </param>
	  protected internal ExtensionHandler(string namespaceUri, string scriptLang)
	  {
		m_namespaceUri = namespaceUri;
		m_scriptLang = scriptLang;
	  }

	  /// <summary>
	  /// Tests whether a certain function name is known within this namespace. </summary>
	  /// <param name="function"> name of the function being tested </param>
	  /// <returns> true if its known, false if not. </returns>
	  public abstract bool isFunctionAvailable(string function);

	  /// <summary>
	  /// Tests whether a certain element name is known within this namespace. </summary>
	  /// <param name="element"> Name of element to check </param>
	  /// <returns> true if its known, false if not. </returns>
	  public abstract bool isElementAvailable(string element);

	  /// <summary>
	  /// Process a call to a function.
	  /// </summary>
	  /// <param name="funcName"> Function name. </param>
	  /// <param name="args">     The arguments of the function call. </param>
	  /// <param name="methodKey"> A key that uniquely identifies this class and method call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed.
	  /// </param>
	  /// <returns> the return value of the function evaluation.
	  /// </returns>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract Object callFunction(String funcName, java.util.Vector args, Object methodKey, ExpressionContext exprContext) throws javax.xml.transform.TransformerException;
	  public abstract object callFunction(string funcName, ArrayList args, object methodKey, ExpressionContext exprContext);

	  /// <summary>
	  /// Process a call to a function.
	  /// </summary>
	  /// <param name="extFunction"> The XPath extension function. </param>
	  /// <param name="args">     The arguments of the function call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed.
	  /// </param>
	  /// <returns> the return value of the function evaluation.
	  /// </returns>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract Object callFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector args, ExpressionContext exprContext) throws javax.xml.transform.TransformerException;
	  public abstract object callFunction(FuncExtFunction extFunction, ArrayList args, ExpressionContext exprContext);

	  /// <summary>
	  /// Process a call to this extension namespace via an element. As a side
	  /// effect, the results are sent to the TransformerImpl's result tree.
	  /// </summary>
	  /// <param name="localPart">      Element name's local part. </param>
	  /// <param name="element">        The extension element being processed. </param>
	  /// <param name="transformer">    Handle to TransformerImpl. </param>
	  /// <param name="stylesheetTree"> The compiled stylesheet tree. </param>
	  /// <param name="methodKey">      A key that uniquely identifies this class and method call.
	  /// </param>
	  /// <exception cref="XSLProcessorException"> thrown if something goes wrong
	  ///            while running the extension handler. </exception>
	  /// <exception cref="MalformedURLException"> if loading trouble </exception>
	  /// <exception cref="FileNotFoundException"> if loading trouble </exception>
	  /// <exception cref="IOException">           if loading trouble </exception>
	  /// <exception cref="TransformerException">  if parsing trouble </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void processElement(String localPart, org.apache.xalan.templates.ElemTemplateElement element, org.apache.xalan.transformer.TransformerImpl transformer, org.apache.xalan.templates.Stylesheet stylesheetTree, Object methodKey) throws TransformerException, java.io.IOException;
	  public abstract void processElement(string localPart, ElemTemplateElement element, TransformerImpl transformer, Stylesheet stylesheetTree, object methodKey);
	}

}