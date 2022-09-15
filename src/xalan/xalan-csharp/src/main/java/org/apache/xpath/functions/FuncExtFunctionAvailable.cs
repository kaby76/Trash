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
 * $Id: FuncExtFunctionAvailable.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using Constants = org.apache.xalan.templates.Constants;
	using ExtensionsProvider = org.apache.xpath.ExtensionsProvider;
	using XPathContext = org.apache.xpath.XPathContext;
	using FunctionTable = org.apache.xpath.compiler.FunctionTable;
	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Execute the ExtFunctionAvailable() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncExtFunctionAvailable : FunctionOneArg
	{
		internal new const long serialVersionUID = 5118814314918592241L;

		[NonSerialized]
		private FunctionTable m_functionTable = null;

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

		if (@namespace.Equals(Constants.S_XSLNAMESPACEURL))
		{
		  try
		  {
			if (null == m_functionTable)
			{
				m_functionTable = new FunctionTable();
			}
			return m_functionTable.functionAvailable(methName) ? XBoolean.S_TRUE : XBoolean.S_FALSE;
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
		  return extProvider.functionAvailable(@namespace, methName) ? XBoolean.S_TRUE : XBoolean.S_FALSE;
		}
	  }

	  /// <summary>
	  /// The function table is an instance field. In order to access this instance 
	  /// field during evaluation, this method is called at compilation time to
	  /// insert function table information for later usage. It should only be used
	  /// during compiling of XPath expressions. </summary>
	  /// <param name="aTable"> an instance of the function table </param>
	  public virtual FunctionTable FunctionTable
	  {
		  set
		  {
				  m_functionTable = value;
		  }
	  }
	}

}