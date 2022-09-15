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
 * $Id: ExpressionContext.java 468637 2006-10-28 06:51:02Z minchau $
 */
namespace org.apache.xalan.extensions
{

	using XObject = org.apache.xpath.objects.XObject;
	using Node = org.w3c.dom.Node;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// An object that implements this interface can supply
	/// information about the current XPath expression context.
	/// </summary>
	public interface ExpressionContext
	{

	  /// <summary>
	  /// Get the current context node. </summary>
	  /// <returns> The current context node. </returns>
	  Node ContextNode {get;}

	  /// <summary>
	  /// Get the current context node list. </summary>
	  /// <returns> An iterator for the current context list, as
	  /// defined in XSLT. </returns>
	  NodeIterator ContextNodes {get;}

	  /// <summary>
	  /// Get the error listener. </summary>
	  /// <returns> The registered error listener. </returns>
	  ErrorListener ErrorListener {get;}

	  /// <summary>
	  /// Get the value of a node as a number. </summary>
	  /// <param name="n"> Node to be converted to a number.  May be null. </param>
	  /// <returns> value of n as a number. </returns>
	  double toNumber(Node n);

	  /// <summary>
	  /// Get the value of a node as a string. </summary>
	  /// <param name="n"> Node to be converted to a string.  May be null. </param>
	  /// <returns> value of n as a string, or an empty string if n is null. </returns>
	  string toString(Node n);

	  /// <summary>
	  /// Get a variable based on it's qualified name.
	  /// </summary>
	  /// <param name="qname"> The qualified name of the variable.
	  /// </param>
	  /// <returns> The evaluated value of the variable.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getVariableOrParam(org.apache.xml.utils.QName qname) throws javax.xml.transform.TransformerException;
	  XObject getVariableOrParam(org.apache.xml.utils.QName qname);

	  /// <summary>
	  /// Get the XPathContext that owns this ExpressionContext.
	  /// 
	  /// Note: exslt:function requires the XPathContext to access
	  /// the variable stack and TransformerImpl.
	  /// </summary>
	  /// <returns> The current XPathContext. </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.XPathContext getXPathContext() throws javax.xml.transform.TransformerException;
	  org.apache.xpath.XPathContext XPathContext {get;}

	}

}