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
 * $Id: ExtensionNamespaceSupport.java 468637 2006-10-28 06:51:02Z minchau $
 */
namespace org.apache.xalan.extensions
{


	/// <summary>
	/// During styleseet composition, an ExtensionNamespaceSupport object is created for each extension 
	/// namespace the stylesheet uses. At the beginning of a transformation, TransformerImpl generates
	/// an ExtensionHandler for each of these objects and adds an entry to the ExtensionsTable hashtable.
	/// </summary>
	public class ExtensionNamespaceSupport
	{
	  // Namespace, ExtensionHandler class name, constructor signature 
	  // and arguments.
	  internal string m_namespace = null;
	  internal string m_handlerClass = null;
	  internal Type[] m_sig = null;
	  internal object[] m_args = null;

	  public ExtensionNamespaceSupport(string @namespace, string handlerClass, object[] constructorArgs)
	  {
		m_namespace = @namespace;
		m_handlerClass = handlerClass;
		m_args = constructorArgs;
		// Create the constructor signature.
		m_sig = new Type[m_args.Length];
		for (int i = 0; i < m_args.Length; i++)
		{
		  if (m_args[i] != null)
		  {
			m_sig[i] = m_args[i].GetType(); //System.out.println("arg class " + i + " " +m_sig[i]);
		  }
		  else // If an arguments is null, pick the constructor later.
		  {
			m_sig = null;
			break;
		  }
		}
	  }

	  public virtual string Namespace
	  {
		  get
		  {
			return m_namespace;
		  }
	  }

	  /// <summary>
	  /// Launch the ExtensionHandler that this ExtensionNamespaceSupport object defines.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ExtensionHandler launch() throws javax.xml.transform.TransformerException
	  public virtual ExtensionHandler launch()
	  {
		ExtensionHandler handler = null;
		try
		{
		  Type cl = ExtensionHandler.getClassForName(m_handlerClass);
		  Constructor con = null;
		  //System.out.println("class " + cl + " " + m_args + " " + m_args.length + " " + m_sig);
		  if (m_sig != null)
		  {
			con = cl.GetConstructor(m_sig);
		  }
		  else // Pick the constructor based on number of args.
		  {
			Constructor[] cons = cl.GetConstructors();
			for (int i = 0; i < cons.Length; i++)
			{
			  if (cons[i].ParameterTypes.length == m_args.Length)
			  {
				con = cons[i];
				break;
			  }
			}
		  }
		  // System.out.println("constructor " + con);
		  if (con != null)
		  {
			handler = (ExtensionHandler)con.newInstance(m_args);
		  }
		  else
		  {
			throw new TransformerException("ExtensionHandler constructor not found");
		  }
		}
		catch (Exception e)
		{
		  throw new TransformerException(e);
		}
		return handler;
	  }

	}

}