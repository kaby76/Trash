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
 * $Id: FuncExtElementAvailable.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using Constants = org.apache.xalan.templates.Constants;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;
	using ExtensionsProvider = org.apache.xpath.ExtensionsProvider;
	using XPathContext = org.apache.xpath.XPathContext;
	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Execute the ExtElementAvailable() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncExtElementAvailable : FunctionOneArg
	{
		internal new const long serialVersionUID = -472533699257968546L;

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		string prefix;
		string @namespace;
		string methName;

		string fullName = m_arg0.execute(xctxt).str();
		int indexOfNSSep = fullName.IndexOf(':');

		if (indexOfNSSep < 0)
		{
		  prefix = "";
		  @namespace = Constants.S_XSLNAMESPACEURL;
		  methName = fullName;
		}
		else
		{
		  prefix = fullName.Substring(0, indexOfNSSep);
		  @namespace = xctxt.NamespaceContext.getNamespaceForPrefix(prefix);
		  if (null == @namespace)
		  {
			return XBoolean.S_FALSE;
		  }
		  methName = fullName.Substring(indexOfNSSep + 1);
		}

		if (@namespace.Equals(Constants.S_XSLNAMESPACEURL) || @namespace.Equals(Constants.S_BUILTIN_EXTENSIONS_URL))
		{
		  try
		  {
			TransformerImpl transformer = (TransformerImpl) xctxt.OwnerObject;
			return transformer.Stylesheet.getAvailableElements().containsKey(new QName(@namespace, methName)) ? XBoolean.S_TRUE : XBoolean.S_FALSE;
		  }
		  catch (Exception)
		  {
			return XBoolean.S_FALSE;
		  }
		}
		else
		{
		  //dml
		  ExtensionsProvider extProvider = (ExtensionsProvider)xctxt.OwnerObject;
		  return extProvider.elementAvailable(@namespace, methName) ? XBoolean.S_TRUE : XBoolean.S_FALSE;
		}
	  }
	}

}