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

namespace org.apache.xalan.extensions
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	/// <summary>
	/// A sample implementation of XPathFunctionResolver, with support for
	/// EXSLT extension functions and Java extension functions.
	/// </summary>
	public class XPathFunctionResolverImpl : XPathFunctionResolver
	{
		/// <summary>
		/// Resolve an extension function from the qualified name and arity.
		/// </summary>
		public virtual XPathFunction resolveFunction(QName qname, int arity)
		{
			if (qname == null)
			{
				throw new System.NullReferenceException(XSLMessages.createMessage(XSLTErrorResources.ER_XPATH_RESOLVER_NULL_QNAME, null));
			}

			if (arity < 0)
			{
				throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_XPATH_RESOLVER_NEGATIVE_ARITY, null));
			}

			string uri = qname.getNamespaceURI();
			if (string.ReferenceEquals(uri, null) || uri.Length == 0)
			{
				return null;
			}

			string className = null;
			string methodName = null;
			if (uri.StartsWith("http://exslt.org", StringComparison.Ordinal))
			{
				className = getEXSLTClassName(uri);
				methodName = qname.getLocalPart();
			}
			else if (!uri.Equals(ExtensionNamespaceContext.JAVA_EXT_URI))
			{
				int lastSlash = className.LastIndexOf('/');
				if (-1 != lastSlash)
				{
					className = className.Substring(lastSlash + 1);
				}
			}

			string localPart = qname.getLocalPart();
			int lastDotIndex = localPart.LastIndexOf('.');
			if (lastDotIndex > 0)
			{
				if (!string.ReferenceEquals(className, null))
				{
					className = className + "." + localPart.Substring(0, lastDotIndex);
				}
				else
				{
					className = localPart.Substring(0, lastDotIndex);
				}

				methodName = localPart.Substring(lastDotIndex + 1);
			}
			else
			{
				methodName = localPart;
			}

			if (null == className || className.Trim().Length == 0 || null == methodName || methodName.Trim().Length == 0)
			{
				return null;
			}

			ExtensionHandler handler = null;
			try
			{
				ExtensionHandler.getClassForName(className);
				handler = new ExtensionHandlerJavaClass(uri, "javaclass", className);
			}
			catch (ClassNotFoundException)
			{
			   return null;
			}
			return new XPathFunctionImpl(handler, methodName);
		}

		/// <summary>
		/// Return the implementation class name of an EXSLT extension from
		/// a given namespace uri. The uri must starts with "http://exslt.org".
		/// </summary>
		private string getEXSLTClassName(string uri)
		{
			if (uri.Equals(ExtensionNamespaceContext.EXSLT_MATH_URI))
			{
				return "org.apache.xalan.lib.ExsltMath";
			}
			else if (uri.Equals(ExtensionNamespaceContext.EXSLT_SET_URI))
			{
				return "org.apache.xalan.lib.ExsltSets";
			}
			else if (uri.Equals(ExtensionNamespaceContext.EXSLT_STRING_URI))
			{
				return "org.apache.xalan.lib.ExsltStrings";
			}
			else if (uri.Equals(ExtensionNamespaceContext.EXSLT_DATETIME_URI))
			{
				return "org.apache.xalan.lib.ExsltDatetime";
			}
			else if (uri.Equals(ExtensionNamespaceContext.EXSLT_DYNAMIC_URI))
			{
				return "org.apache.xalan.lib.ExsltDynamic";
			}
			else if (uri.Equals(ExtensionNamespaceContext.EXSLT_URI))
			{
				return "org.apache.xalan.lib.ExsltCommon";
			}
			else
			{
				return null;
			}
		}
	}

}