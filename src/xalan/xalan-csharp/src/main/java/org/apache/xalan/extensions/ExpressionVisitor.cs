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
 * $Id: ExpressionVisitor.java 468637 2006-10-28 06:51:02Z minchau $
 */
namespace org.apache.xalan.extensions
{
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using FuncExtFunctionAvailable = org.apache.xpath.functions.FuncExtFunctionAvailable;
	using Function = org.apache.xpath.functions.Function;

	/// <summary>
	/// When <seealso cref="org.apache.xalan.processor.StylesheetHandler"/> creates 
	/// an <seealso cref="org.apache.xpath.XPath"/>, the ExpressionVisitor
	/// visits the XPath expression. For any extension functions it 
	/// encounters, it instructs StylesheetRoot to register the
	/// extension namespace. 
	/// 
	/// This mechanism is required to locate extension functions
	/// that may be embedded within an expression.
	/// </summary>
	public class ExpressionVisitor : XPathVisitor
	{
	  private StylesheetRoot m_sroot;

	  /// <summary>
	  /// The constructor sets the StylesheetRoot variable which
	  /// is used to register extension namespaces. </summary>
	  /// <param name="sroot"> the StylesheetRoot that is being constructed. </param>
	  public ExpressionVisitor(StylesheetRoot sroot)
	  {
		m_sroot = sroot;
	  }

	  /// <summary>
	  /// If the function is an extension function, register the namespace.
	  /// </summary>
	  /// <param name="owner"> The current XPath object that owns the expression. </param>
	  /// <param name="func"> The function currently being visited.
	  /// </param>
	  /// <returns> true to continue the visit in the subtree, if any. </returns>
	  public override bool visitFunction(ExpressionOwner owner, Function func)
	  {
		if (func is FuncExtFunction)
		{
		  string @namespace = ((FuncExtFunction)func).Namespace;
		  m_sroot.ExtensionNamespacesManager.registerExtension(@namespace);
		}
		else if (func is FuncExtFunctionAvailable)
		{
		  string arg = ((FuncExtFunctionAvailable)func).Arg0.ToString();
		  if (arg.IndexOf(":", StringComparison.Ordinal) > 0)
		  {
			  string prefix = arg.Substring(0, arg.IndexOf(":", StringComparison.Ordinal));
			  string @namespace = this.m_sroot.getNamespaceForPrefix(prefix);
			  m_sroot.ExtensionNamespacesManager.registerExtension(@namespace);
		  }
		}
		return true;
	  }

	}

}