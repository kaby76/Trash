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
 * $Id: ExtensionNamespacesManager.java 1225575 2011-12-29 15:50:25Z mrglavas $
 */
namespace org.apache.xalan.extensions
{

	using Constants = org.apache.xalan.templates.Constants;

	/// <summary>
	/// Used during assembly of a stylesheet to collect the information for each 
	/// extension namespace that is required during the transformation process 
	/// to generate an <seealso cref="ExtensionHandler"/>.
	/// 
	/// </summary>
	public class ExtensionNamespacesManager
	{
	  /// <summary>
	  /// Vector of ExtensionNamespaceSupport objects to be used to generate ExtensionHandlers.
	  /// </summary>
	  private ArrayList m_extensions = new ArrayList();
	  /// <summary>
	  /// Vector of ExtensionNamespaceSupport objects for predefined ExtensionNamespaces. Elements
	  /// from this vector are added to the m_extensions vector when encountered in the stylesheet.
	  /// </summary>
	  private ArrayList m_predefExtensions = new ArrayList(7);
	  /// <summary>
	  /// Vector of extension namespaces for which sufficient information is not yet available to
	  /// complete the registration process.
	  /// </summary>
	  private ArrayList m_unregisteredExtensions = new ArrayList();

	  /// <summary>
	  /// An ExtensionNamespacesManager is instantiated the first time an extension function or
	  /// element is found in the stylesheet. During initialization, a vector of ExtensionNamespaceSupport
	  /// objects is created, one for each predefined extension namespace.
	  /// </summary>
	  public ExtensionNamespacesManager()
	  {
		setPredefinedNamespaces();
	  }

	  /// <summary>
	  /// If necessary, register the extension namespace found compiling a function or 
	  /// creating an extension element. 
	  /// 
	  /// If it is a predefined namespace, create a
	  /// support object to simplify the instantiate of an appropriate ExtensionHandler
	  /// during transformation runtime. Otherwise, add the namespace, if necessary,
	  /// to a vector of undefined extension namespaces, to be defined later.
	  /// 
	  /// </summary>
	  public virtual void registerExtension(string @namespace)
	  {
		if (namespaceIndex(@namespace, m_extensions) == -1)
		{
		  int predef = namespaceIndex(@namespace, m_predefExtensions);
		  if (predef != -1)
		  {
			m_extensions.Add(m_predefExtensions[predef]);
		  }
		  else if (!(m_unregisteredExtensions.Contains(@namespace)))
		  {
			m_unregisteredExtensions.Add(@namespace);
		  }
		}
	  }

	  /// <summary>
	  /// Register the extension namespace for an ElemExtensionDecl or ElemFunction,
	  /// and prepare a support object to launch the appropriate ExtensionHandler at 
	  /// transformation runtime.
	  /// </summary>
	  public virtual void registerExtension(ExtensionNamespaceSupport extNsSpt)
	  {
		string @namespace = extNsSpt.Namespace;
		if (namespaceIndex(@namespace, m_extensions) == -1)
		{
		  m_extensions.Add(extNsSpt);
		  if (m_unregisteredExtensions.Contains(@namespace))
		  {
			m_unregisteredExtensions.Remove(@namespace);
		  }
		}

	  }

	  /// <summary>
	  /// Get the index for a namespace entry in the extension namespace Vector, -1 if
	  /// no such entry yet exists.
	  /// </summary>
	  public virtual int namespaceIndex(string @namespace, ArrayList extensions)
	  {
		for (int i = 0; i < extensions.Count; i++)
		{
		  if (((ExtensionNamespaceSupport)extensions[i]).Namespace.Equals(@namespace))
		  {
			return i;
		  }
		}
		return -1;
	  }


	  /// <summary>
	  /// Get the vector of extension namespaces. Used to provide
	  /// the extensions table access to a list of extension
	  /// namespaces encountered during composition of a stylesheet.
	  /// </summary>
	  public virtual ArrayList Extensions
	  {
		  get
		  {
			return m_extensions;
		  }
	  }

	  /// <summary>
	  /// Attempt to register any unregistered extension namespaces.
	  /// </summary>
	  public virtual void registerUnregisteredNamespaces()
	  {
		for (int i = 0; i < m_unregisteredExtensions.Count; i++)
		{
		  string ns = (string)m_unregisteredExtensions[i];
		  ExtensionNamespaceSupport extNsSpt = defineJavaNamespace(ns);
		  if (extNsSpt != null)
		  {
			m_extensions.Add(extNsSpt);
		  }
		}
	  }

		/// <summary>
		/// For any extension namespace that is not either predefined or defined 
		/// by a "component" declaration or exslt function declaration, attempt 
		/// to create an ExtensionNamespaceSuport object for the appropriate 
		/// Java class or Java package Extension Handler.
		/// 
		/// Called by StylesheetRoot.recompose(), after all ElemTemplate compose()
		/// operations have taken place, in order to set up handlers for
		/// the remaining extension namespaces.
		/// </summary>
		/// <param name="ns"> The extension namespace URI. </param>
		/// <returns>   An ExtensionNamespaceSupport object for this namespace
		/// (which defines the ExtensionHandler to be used), or null if such 
		/// an object cannot be created. 
		/// </returns>
		/// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public virtual ExtensionNamespaceSupport defineJavaNamespace(string ns)
	  {
		return defineJavaNamespace(ns, ns);
	  }
	  public virtual ExtensionNamespaceSupport defineJavaNamespace(string ns, string classOrPackage)
	  {
		if (null == ns || ns.Trim().Length == 0) // defensive. I don't think it's needed.  -sb
		{
		  return null;
		}

		// Prepare the name of the actual class or package, stripping
		// out any leading "class:".  Next, see if there is a /.  If so,
		// only look at the text to the right of the rightmost /.
		string className = classOrPackage;
		if (className.StartsWith("class:", StringComparison.Ordinal))
		{
		  className = className.Substring(6);
		}

		int lastSlash = className.LastIndexOf('/');
		if (-1 != lastSlash)
		{
		  className = className.Substring(lastSlash + 1);
		}

		// The className can be null here, and can cause an error in getClassForName
		// in JDK 1.8.
		if (null == className || className.Trim().Length == 0)
		{
		  return null;
		}

		try
		{
		  ExtensionHandler.getClassForName(className);
		  return new ExtensionNamespaceSupport(ns, "org.apache.xalan.extensions.ExtensionHandlerJavaClass", new object[]{ns, "javaclass", className});
		}
		catch (ClassNotFoundException)
		{
		  return new ExtensionNamespaceSupport(ns, "org.apache.xalan.extensions.ExtensionHandlerJavaPackage", new object[]{ns, "javapackage", className + "."});
		}
	  }

	/*
	  public ExtensionNamespaceSupport getSupport(int index, Vector extensions)
	  {
	    return (ExtensionNamespaceSupport)extensions.elementAt(index);
	  }
	*/


	  /// <summary>
	  /// Set up a Vector for predefined extension namespaces.
	  /// </summary>
	  private void setPredefinedNamespaces()
	  {
		string uri = Constants.S_EXTENSIONS_JAVA_URL;
		string handlerClassName = "org.apache.xalan.extensions.ExtensionHandlerJavaPackage";
		string lang = "javapackage";
		string lib = "";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXTENSIONS_OLD_JAVA_URL;
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXTENSIONS_LOTUSXSL_JAVA_URL;
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_BUILTIN_EXTENSIONS_URL;
		handlerClassName = "org.apache.xalan.extensions.ExtensionHandlerJavaClass";
		lang = "javaclass"; // for remaining predefined extension namespaces.
		lib = "org.apache.xalan.lib.Extensions";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_BUILTIN_OLD_EXTENSIONS_URL;
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		// Xalan extension namespaces (redirect, pipe and SQL).
		uri = Constants.S_EXTENSIONS_REDIRECT_URL;
		lib = "org.apache.xalan.lib.Redirect";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXTENSIONS_PIPE_URL;
		lib = "org.apache.xalan.lib.PipeDocument";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXTENSIONS_SQL_URL;
		lib = "org.apache.xalan.lib.sql.XConnection";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));


		//EXSLT namespaces (not including EXSLT function namespaces which are
		// registered by the associated ElemFunction.
		uri = Constants.S_EXSLT_COMMON_URL;
		lib = "org.apache.xalan.lib.ExsltCommon";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXSLT_MATH_URL;
		lib = "org.apache.xalan.lib.ExsltMath";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXSLT_SETS_URL;
		lib = "org.apache.xalan.lib.ExsltSets";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXSLT_DATETIME_URL;
		lib = "org.apache.xalan.lib.ExsltDatetime";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXSLT_DYNAMIC_URL;
		lib = "org.apache.xalan.lib.ExsltDynamic";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));

		uri = Constants.S_EXSLT_STRINGS_URL;
		lib = "org.apache.xalan.lib.ExsltStrings";
		m_predefExtensions.Add(new ExtensionNamespaceSupport(uri, handlerClassName, new object[]{uri, lang, lib}));
	  }

	}

}