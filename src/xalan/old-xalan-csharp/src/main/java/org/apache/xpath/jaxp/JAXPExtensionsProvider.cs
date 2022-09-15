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

// $Id: JAXPExtensionsProvider.java 468655 2006-10-28 07:12:06Z minchau $

namespace org.apache.xpath.jaxp
{


	using XObject = org.apache.xpath.objects.XObject;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XSLMessages = org.apache.xalan.res.XSLMessages;

	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;

	/// 
	/// <summary>
	/// @author Ramesh Mandava ( ramesh.mandava@sun.com )
	/// </summary>
	public class JAXPExtensionsProvider : ExtensionsProvider
	{

		private readonly XPathFunctionResolver resolver;
		private bool extensionInvocationDisabled = false;

		public JAXPExtensionsProvider(XPathFunctionResolver resolver)
		{
			this.resolver = resolver;
			this.extensionInvocationDisabled = false;
		}

		public JAXPExtensionsProvider(XPathFunctionResolver resolver, bool featureSecureProcessing)
		{
			this.resolver = resolver;
			this.extensionInvocationDisabled = featureSecureProcessing;
		}

		/// <summary>
		/// Is the extension function available?
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean functionAvailable(String ns, String funcName) throws javax.xml.transform.TransformerException
		public virtual bool functionAvailable(string ns, string funcName)
		{
		  try
		  {
			if (string.ReferenceEquals(funcName, null))
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"Function Name"});
				throw new System.NullReferenceException(fmsg);
			}
			//Find the XPathFunction corresponding to namespace and funcName
			QName myQName = new QName(ns, funcName);
			XPathFunction xpathFunction = resolver.resolveFunction(myQName, 0);
			if (xpathFunction == null)
			{
				return false;
			}
			return true;
		  }
		  catch (Exception)
		  {
			return false;
		  }


		}


		/// <summary>
		/// Is the extension element available?
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean elementAvailable(String ns, String elemName) throws javax.xml.transform.TransformerException
		public virtual bool elementAvailable(string ns, string elemName)
		{
			return false;
		}

		/// <summary>
		/// Execute the extension function.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object extFunction(String ns, String funcName, java.util.Vector argVec, Object methodKey) throws javax.xml.transform.TransformerException
		public virtual object extFunction(string ns, string funcName, ArrayList argVec, object methodKey)
		{
			try
			{

				if (string.ReferenceEquals(funcName, null))
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"Function Name"});
					throw new System.NullReferenceException(fmsg);
				}
				//Find the XPathFunction corresponding to namespace and funcName
				QName myQName = new QName(ns, funcName);

				// JAXP 1.3 spec says When XMLConstants.FEATURE_SECURE_PROCESSING 
				// feature is set then invocation of extension functions need to
				// throw XPathFunctionException
				if (extensionInvocationDisabled)
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, new object[] {myQName.ToString()});
					throw new XPathFunctionException(fmsg);
				}

				// Assuming user is passing all the needed parameters ( including
				// default values )
				int arity = argVec.Count;

				XPathFunction xpathFunction = resolver.resolveFunction(myQName, arity);

				// not using methodKey
				ArrayList argList = new ArrayList(arity);
				for (int i = 0; i < arity; i++)
				{
					object argument = argVec[i];
					// XNodeSet object() returns NodeVector and not NodeList
					// Explicitly getting NodeList by using nodelist()
					if (argument is XNodeSet)
					{
						argList.Insert(i, ((XNodeSet)argument).nodelist());
					}
					else if (argument is XObject)
					{
						object passedArgument = ((XObject)argument).@object();
						argList.Insert(i, passedArgument);
					}
					else
					{
						argList.Insert(i, argument);
					}
				}

				return (xpathFunction.evaluate(argList));
			}
			catch (XPathFunctionException xfe)
			{
				// If we get XPathFunctionException then we want to terminate
				// further execution by throwing WrappedRuntimeException 
				throw new org.apache.xml.utils.WrappedRuntimeException(xfe);
			}
			catch (Exception e)
			{
				throw new TransformerException(e);
			}

		}

		/// <summary>
		/// Execute the extension function.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object extFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector argVec) throws javax.xml.transform.TransformerException
		public virtual object extFunction(FuncExtFunction extFunction, ArrayList argVec)
		{
			try
			{
				string @namespace = extFunction.Namespace;
				string functionName = extFunction.FunctionName;
				int arity = extFunction.ArgCount;
				QName myQName = new QName(@namespace, functionName);

				// JAXP 1.3 spec says  When XMLConstants.FEATURE_SECURE_PROCESSING
				// feature is set then invocation of extension functions need to
				// throw XPathFunctionException
				if (extensionInvocationDisabled)
				{
					string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, new object[] {myQName.ToString()});
					throw new XPathFunctionException(fmsg);
				}

				XPathFunction xpathFunction = resolver.resolveFunction(myQName, arity);

				ArrayList argList = new ArrayList(arity);
				for (int i = 0; i < arity; i++)
				{
					object argument = argVec[i];
					// XNodeSet object() returns NodeVector and not NodeList
					// Explicitly getting NodeList by using nodelist()
					if (argument is XNodeSet)
					{
						argList.Insert(i, ((XNodeSet)argument).nodelist());
					}
					else if (argument is XObject)
					{
						object passedArgument = ((XObject)argument).@object();
						argList.Insert(i, passedArgument);
					}
					else
					{
						argList.Insert(i, argument);
					}
				}

				return (xpathFunction.evaluate(argList));

			}
			catch (XPathFunctionException xfe)
			{
				// If we get XPathFunctionException then we want to terminate 
				// further execution by throwing WrappedRuntimeException
				throw new org.apache.xml.utils.WrappedRuntimeException(xfe);
			}
			catch (Exception e)
			{
				throw new TransformerException(e);
			}
		}

	}

}