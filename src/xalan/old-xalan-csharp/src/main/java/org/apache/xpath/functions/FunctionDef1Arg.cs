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
 * $Id: FunctionDef1Arg.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using XMLString = org.apache.xml.utils.XMLString;
	using XString = org.apache.xpath.objects.XString;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Base class for functions that accept one argument that can be defaulted if
	/// not specified.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FunctionDef1Arg : FunctionOneArg
	{
		internal new const long serialVersionUID = 2325189412814149264L;

	  /// <summary>
	  /// Execute the first argument expression that is expected to return a
	  /// nodeset.  If the argument is null, then return the current context node.
	  /// </summary>
	  /// <param name="xctxt"> Runtime XPath context.
	  /// </param>
	  /// <returns> The first node of the executed nodeset, or the current context
	  ///         node if the first argument is null.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if an error occurs while
	  ///                                   executing the argument expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected int getArg0AsNode(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  protected internal virtual int getArg0AsNode(XPathContext xctxt)
	  {

		return (null == m_arg0) ? xctxt.CurrentNode : m_arg0.asNode(xctxt);
	  }

	  /// <summary>
	  /// Tell if the expression is a nodeset expression. </summary>
	  /// <returns> true if the expression can be represented as a nodeset. </returns>
	  public virtual bool Arg0IsNodesetExpr()
	  {
		return (null == m_arg0) ? true : m_arg0.NodesetExpr;
	  }

	  /// <summary>
	  /// Execute the first argument expression that is expected to return a
	  /// string.  If the argument is null, then get the string value from the
	  /// current context node.
	  /// </summary>
	  /// <param name="xctxt"> Runtime XPath context.
	  /// </param>
	  /// <returns> The string value of the first argument, or the string value of the
	  ///         current context node if the first argument is null.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if an error occurs while
	  ///                                   executing the argument expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.xml.utils.XMLString getArg0AsString(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  protected internal virtual XMLString getArg0AsString(XPathContext xctxt)
	  {
		if (null == m_arg0)
		{
		  int currentNode = xctxt.CurrentNode;
		  if (org.apache.xml.dtm.DTM_Fields.NULL == currentNode)
		  {
			return XString.EMPTYSTRING;
		  }
		  else
		  {
			DTM dtm = xctxt.getDTM(currentNode);
			return dtm.getStringValue(currentNode);
		  }

		}
		else
		{
		  return m_arg0.execute(xctxt).xstr();
		}
	  }

	  /// <summary>
	  /// Execute the first argument expression that is expected to return a
	  /// number.  If the argument is null, then get the number value from the
	  /// current context node.
	  /// </summary>
	  /// <param name="xctxt"> Runtime XPath context.
	  /// </param>
	  /// <returns> The number value of the first argument, or the number value of the
	  ///         current context node if the first argument is null.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if an error occurs while
	  ///                                   executing the argument expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double getArg0AsNumber(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  protected internal virtual double getArg0AsNumber(XPathContext xctxt)
	  {

		if (null == m_arg0)
		{
		  int currentNode = xctxt.CurrentNode;
		  if (org.apache.xml.dtm.DTM_Fields.NULL == currentNode)
		  {
			return 0;
		  }
		  else
		  {
			DTM dtm = xctxt.getDTM(currentNode);
			XMLString str = dtm.getStringValue(currentNode);
			return str.toDouble();
		  }

		}
		else
		{
		  return m_arg0.execute(xctxt).num();
		}
	  }

	  /// <summary>
	  /// Check that the number of arguments passed to this function is correct.
	  /// </summary>
	  /// <param name="argNum"> The number of arguments that is being passed to the function.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> if the number of arguments is not 0 or 1. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkNumberArgs(int argNum) throws WrongNumberArgsException
	  public override void checkNumberArgs(int argNum)
	  {
		if (argNum > 1)
		{
		  reportWrongNumberArgs();
		}
	  }

	  /// <summary>
	  /// Constructs and throws a WrongNumberArgException with the appropriate
	  /// message for this function object.
	  /// </summary>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws WrongNumberArgsException
	  protected internal override void reportWrongNumberArgs()
	  {
		  throw new WrongNumberArgsException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ZERO_OR_ONE, null)); //"0 or 1");
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public override bool canTraverseOutsideSubtree()
	  {
		return (null == m_arg0) ? false : base.canTraverseOutsideSubtree();
	  }
	}

}