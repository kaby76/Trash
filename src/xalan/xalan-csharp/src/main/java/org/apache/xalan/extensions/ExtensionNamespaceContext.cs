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
	/// A sample implementation of NamespaceContext, with support for 
	/// EXSLT extension functions and Java extension functions.
	/// </summary>
	public class ExtensionNamespaceContext : NamespaceContext
	{
		public const string EXSLT_PREFIX = "exslt";
		public const string EXSLT_URI = "http://exslt.org/common";
		public const string EXSLT_MATH_PREFIX = "math";
		public const string EXSLT_MATH_URI = "http://exslt.org/math";
		public const string EXSLT_SET_PREFIX = "set";
		public const string EXSLT_SET_URI = "http://exslt.org/sets";
		public const string EXSLT_STRING_PREFIX = "str";
		public const string EXSLT_STRING_URI = "http://exslt.org/strings";
		public const string EXSLT_DATETIME_PREFIX = "datetime";
		public const string EXSLT_DATETIME_URI = "http://exslt.org/dates-and-times";
		public const string EXSLT_DYNAMIC_PREFIX = "dyn";
		public const string EXSLT_DYNAMIC_URI = "http://exslt.org/dynamic";
		public const string JAVA_EXT_PREFIX = "java";
		public const string JAVA_EXT_URI = "http://xml.apache.org/xalan/java";

		/// <summary>
		/// Return the namespace uri for a given prefix
		/// </summary>
		public virtual string getNamespaceURI(string prefix)
		{
			if (string.ReferenceEquals(prefix, null))
			{
				throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_NAMESPACE_CONTEXT_NULL_PREFIX, null));
			}

			if (prefix.Equals(XMLConstants.DEFAULT_NS_PREFIX))
			{
				return XMLConstants.NULL_NS_URI;
			}
			else if (prefix.Equals(XMLConstants.XML_NS_PREFIX))
			{
				return XMLConstants.XML_NS_URI;
			}
			else if (prefix.Equals(XMLConstants.XMLNS_ATTRIBUTE))
			{
				return XMLConstants.XMLNS_ATTRIBUTE_NS_URI;
			}
			else if (prefix.Equals(EXSLT_PREFIX))
			{
				return EXSLT_URI;
			}
			else if (prefix.Equals(EXSLT_MATH_PREFIX))
			{
				return EXSLT_MATH_URI;
			}
			else if (prefix.Equals(EXSLT_SET_PREFIX))
			{
				return EXSLT_SET_URI;
			}
			else if (prefix.Equals(EXSLT_STRING_PREFIX))
			{
				return EXSLT_STRING_URI;
			}
			else if (prefix.Equals(EXSLT_DATETIME_PREFIX))
			{
				return EXSLT_DATETIME_URI;
			}
			else if (prefix.Equals(EXSLT_DYNAMIC_PREFIX))
			{
				return EXSLT_DYNAMIC_URI;
			}
			else if (prefix.Equals(JAVA_EXT_PREFIX))
			{
				return JAVA_EXT_URI;
			}
			else
			{
				return XMLConstants.NULL_NS_URI;
			}
		}

		/// <summary>
		/// Return the prefix for a given namespace uri.
		/// </summary>
		public virtual string getPrefix(string @namespace)
		{
			if (string.ReferenceEquals(@namespace, null))
			{
				throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, null));
			}

			if (@namespace.Equals(XMLConstants.XML_NS_URI))
			{
				return XMLConstants.XML_NS_PREFIX;
			}
			else if (@namespace.Equals(XMLConstants.XMLNS_ATTRIBUTE_NS_URI))
			{
				return XMLConstants.XMLNS_ATTRIBUTE;
			}
			else if (@namespace.Equals(EXSLT_URI))
			{
				return EXSLT_PREFIX;
			}
			else if (@namespace.Equals(EXSLT_MATH_URI))
			{
				return EXSLT_MATH_PREFIX;
			}
			else if (@namespace.Equals(EXSLT_SET_URI))
			{
				return EXSLT_SET_PREFIX;
			}
			else if (@namespace.Equals(EXSLT_STRING_URI))
			{
				return EXSLT_STRING_PREFIX;
			}
			else if (@namespace.Equals(EXSLT_DATETIME_URI))
			{
				return EXSLT_DATETIME_PREFIX;
			}
			else if (@namespace.Equals(EXSLT_DYNAMIC_URI))
			{
				return EXSLT_DYNAMIC_PREFIX;
			}
			else if (@namespace.Equals(JAVA_EXT_URI))
			{
				return JAVA_EXT_PREFIX;
			}
			else
			{
				return null;
			}
		}

		public virtual System.Collections.IEnumerator getPrefixes(string @namespace)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String result = getPrefix(namespace);
			string result = getPrefix(@namespace);

			return new IteratorAnonymousInnerClass(this, result);
		}

		private class IteratorAnonymousInnerClass : System.Collections.IEnumerator
		{
			private readonly ExtensionNamespaceContext outerInstance;

			private string result;

			public IteratorAnonymousInnerClass(ExtensionNamespaceContext outerInstance, string result)
			{
				this.outerInstance = outerInstance;
				this.result = result;
				isFirstIteration = (!string.ReferenceEquals(result, null));
			}


			private bool isFirstIteration;

			public bool hasNext()
			{
				return isFirstIteration;
			}

			public object next()
			{
				if (isFirstIteration)
				{
					isFirstIteration = false;
					return result;
				}
				else
				{
					return null;
				}
			}

			public void remove()
			{
				throw new System.NotSupportedException();
			}
		}
	}

}