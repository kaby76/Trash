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
 * $Id: ExtensionsTable.java 469672 2006-10-31 21:56:19Z minchau $
 */
namespace org.apache.xalan.extensions
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;
	using XPathProcessorException = org.apache.xpath.XPathProcessorException;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;

	/// <summary>
	/// Class holding a table registered extension namespace handlers
	/// @xsl.usage internal
	/// </summary>
	public class ExtensionsTable
	{
	  /// <summary>
	  /// Table of extensions that may be called from the expression language
	  /// via the call(name, ...) function.  Objects are keyed on the call
	  /// name.
	  /// @xsl.usage internal
	  /// </summary>
	  public Hashtable m_extensionFunctionNamespaces = new Hashtable();

	  /// <summary>
	  /// The StylesheetRoot associated with this extensions table.
	  /// </summary>
	  private StylesheetRoot m_sroot;

	  /// <summary>
	  /// The constructor (called from TransformerImpl) registers the
	  /// StylesheetRoot for the transformation and instantiates an
	  /// ExtensionHandler for each extension namespace.
	  /// @xsl.usage advanced
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ExtensionsTable(org.apache.xalan.templates.StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public ExtensionsTable(StylesheetRoot sroot)
	  {
		m_sroot = sroot;
		ArrayList extensions = m_sroot.Extensions;
		for (int i = 0; i < extensions.Count; i++)
		{
		  ExtensionNamespaceSupport extNamespaceSpt = (ExtensionNamespaceSupport)extensions[i];
		  ExtensionHandler extHandler = extNamespaceSpt.launch();
			if (extHandler != null)
			{
			  addExtensionNamespace(extNamespaceSpt.Namespace, extHandler);
			}
		}
	  }

	  /// <summary>
	  /// Get an ExtensionHandler object that represents the
	  /// given namespace. </summary>
	  /// <param name="extns"> A valid extension namespace.
	  /// </param>
	  /// <returns> ExtensionHandler object that represents the
	  /// given namespace. </returns>
	  public virtual ExtensionHandler get(string extns)
	  {
		return (ExtensionHandler) m_extensionFunctionNamespaces[extns];
	  }

	  /// <summary>
	  /// Register an extension namespace handler. This handler provides
	  /// functions for testing whether a function is known within the
	  /// namespace and also for invoking the functions.
	  /// </summary>
	  /// <param name="uri"> the URI for the extension. </param>
	  /// <param name="extNS"> the extension handler.
	  /// @xsl.usage advanced </param>
	  public virtual void addExtensionNamespace(string uri, ExtensionHandler extNS)
	  {
		m_extensionFunctionNamespaces[uri] = extNS;
	  }

	  /// <summary>
	  /// Execute the function-available() function. </summary>
	  /// <param name="ns">       the URI of namespace in which the function is needed </param>
	  /// <param name="funcName"> the function name being tested
	  /// </param>
	  /// <returns> whether the given function is available or not.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean functionAvailable(String ns, String funcName) throws javax.xml.transform.TransformerException
	  public virtual bool functionAvailable(string ns, string funcName)
	  {
		bool isAvailable = false;

		if (null != ns)
		{
		  ExtensionHandler extNS = (ExtensionHandler) m_extensionFunctionNamespaces[ns];
		  if (extNS != null)
		  {
			isAvailable = extNS.isFunctionAvailable(funcName);
		  }
		}
		return isAvailable;
	  }

	  /// <summary>
	  /// Execute the element-available() function. </summary>
	  /// <param name="ns">       the URI of namespace in which the function is needed </param>
	  /// <param name="elemName"> name of element being tested
	  /// </param>
	  /// <returns> whether the given element is available or not.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean elementAvailable(String ns, String elemName) throws javax.xml.transform.TransformerException
	  public virtual bool elementAvailable(string ns, string elemName)
	  {
		bool isAvailable = false;
		if (null != ns)
		{
		  ExtensionHandler extNS = (ExtensionHandler) m_extensionFunctionNamespaces[ns];
		  if (extNS != null) // defensive
		  {
			isAvailable = extNS.isElementAvailable(elemName);
		  }
		}
		return isAvailable;
	  }

	  /// <summary>
	  /// Handle an extension function. </summary>
	  /// <param name="ns">        the URI of namespace in which the function is needed </param>
	  /// <param name="funcName">  the function name being called </param>
	  /// <param name="argVec">    arguments to the function in a vector </param>
	  /// <param name="methodKey"> a unique key identifying this function instance in the
	  ///                  stylesheet </param>
	  /// <param name="exprContext"> a context which may be passed to an extension function
	  ///                  and provides callback functions to access various
	  ///                  areas in the environment
	  /// </param>
	  /// <returns> result of executing the function
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object extFunction(String ns, String funcName, java.util.Vector argVec, Object methodKey, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public virtual object extFunction(string ns, string funcName, ArrayList argVec, object methodKey, ExpressionContext exprContext)
	  {
		object result = null;
		if (null != ns)
		{
		  ExtensionHandler extNS = (ExtensionHandler) m_extensionFunctionNamespaces[ns];
		  if (null != extNS)
		  {
			try
			{
			  result = extNS.callFunction(funcName, argVec, methodKey, exprContext);
			}
			catch (javax.xml.transform.TransformerException e)
			{
			  throw e;
			}
			catch (Exception e)
			{
			  throw new javax.xml.transform.TransformerException(e);
			}
		  }
		  else
		  {
			throw new XPathProcessorException(XSLMessages.createMessage(XSLTErrorResources.ER_EXTENSION_FUNC_UNKNOWN, new object[]{ns, funcName}));
			//"Extension function '" + ns + ":" + funcName + "' is unknown");
		  }
		}
		return result;
	  }

	  /// <summary>
	  /// Handle an extension function. </summary>
	  /// <param name="extFunction">  the extension function </param>
	  /// <param name="argVec">    arguments to the function in a vector </param>
	  /// <param name="exprContext"> a context which may be passed to an extension function
	  ///                  and provides callback functions to access various
	  ///                  areas in the environment
	  /// </param>
	  /// <returns> result of executing the function
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object extFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector argVec, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public virtual object extFunction(FuncExtFunction extFunction, ArrayList argVec, ExpressionContext exprContext)
	  {
		object result = null;
		string ns = extFunction.Namespace;
		if (null != ns)
		{
		  ExtensionHandler extNS = (ExtensionHandler) m_extensionFunctionNamespaces[ns];
		  if (null != extNS)
		  {
			try
			{
			  result = extNS.callFunction(extFunction, argVec, exprContext);
			}
			catch (javax.xml.transform.TransformerException e)
			{
			  throw e;
			}
			catch (Exception e)
			{
			  throw new javax.xml.transform.TransformerException(e);
			}
		  }
		  else
		  {
			throw new XPathProcessorException(XSLMessages.createMessage(XSLTErrorResources.ER_EXTENSION_FUNC_UNKNOWN, new object[]{ns, extFunction.FunctionName}));
		  }
		}
		return result;
	  }
	}

}