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
/*
 * $Id: FuncLast.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XPathContext = org.apache.xpath.XPathContext;
	using SubContextList = org.apache.xpath.axes.SubContextList;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;


	/// <summary>
	/// Execute the Last() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncLast : Function
	{
		internal new const long serialVersionUID = 9205812403085432943L;

	  private bool m_isTopLevel;

	  /// <summary>
	  /// Figure out if we're executing a toplevel expression.
	  /// If so, we can't be inside of a predicate. 
	  /// </summary>
	  public override void postCompileStep(Compiler compiler)
	  {
		m_isTopLevel = compiler.LocationPathDepth == -1;
	  }

	  /// <summary>
	  /// Get the position in the current context node list.
	  /// </summary>
	  /// <param name="xctxt"> non-null reference to XPath runtime context.
	  /// </param>
	  /// <returns> The number of nodes in the list.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int getCountOfContextNodeList(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual int getCountOfContextNodeList(XPathContext xctxt)
	  {

		// assert(null != m_contextNodeList, "m_contextNodeList must be non-null");
		// If we're in a predicate, then this will return non-null.
		SubContextList iter = m_isTopLevel ? null : xctxt.SubContextList;

		// System.out.println("iter: "+iter);
		if (null != iter)
		{
		  return iter.getLastPos(xctxt);
		}

		DTMIterator cnl = xctxt.ContextNodeList;
		int count;
		if (null != cnl)
		{
		  count = cnl.Length;
		  // System.out.println("count: "+count); 
		}
		else
		{
		  count = 0;
		}
		return count;
	  }

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
		XNumber xnum = new XNumber((double) getCountOfContextNodeList(xctxt));
		// System.out.println("last: "+xnum.num());
		return xnum;
	  }

	  /// <summary>
	  /// No arguments to process, so this does nothing.
	  /// </summary>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		// no-op
	  }

	}

}