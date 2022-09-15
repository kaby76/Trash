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

namespace org.apache.xalan.extensions
{


	/// <summary>
	/// A sample implementation of XPathFunction, with support for
	/// EXSLT extension functions and Java extension functions.
	/// </summary>
	public class XPathFunctionImpl : XPathFunction
	{
		private ExtensionHandler m_handler;
		private string m_funcName;

		/// <summary>
		/// Construct an instance of XPathFunctionImpl from the
		/// ExtensionHandler and function name.
		/// </summary>
		public XPathFunctionImpl(ExtensionHandler handler, string funcName)
		{
			m_handler = handler;
			m_funcName = funcName;
		}

		/// <seealso cref="javax.xml.xpath.XPathFunction.evaluate(java.util.List)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object evaluate(java.util.List args) throws javax.xml.xpath.XPathFunctionException
		public virtual object evaluate(System.Collections.IList args)
		{
			ArrayList argsVec = listToVector(args);

			try
			{
				// The method key and ExpressionContext are set to null.
				return m_handler.callFunction(m_funcName, argsVec, null, null);
			}
			catch (TransformerException e)
			{
				throw new XPathFunctionException(e);
			}
		}

		/// <summary>
		/// Convert a java.util.List to a java.util.Vector. 
		/// No conversion is done if the List is already a Vector.
		/// </summary>
		private static ArrayList listToVector(System.Collections.IList args)
		{
			if (args == null)
			{
				return null;
			}
			else if (args is ArrayList)
			{
				return (ArrayList)args;
			}
			else
			{
				ArrayList result = new ArrayList();
				result.AddRange(args);
				return result;
			}
		}
	}

}