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
 * $Id: AVTPartXPath.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathFactory = org.apache.xpath.XPathFactory;
	using XPathParser = org.apache.xpath.compiler.XPathParser;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Simple string part of a complex AVT.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class AVTPartXPath : AVTPart
	{
		internal new const long serialVersionUID = -4460373807550527675L;

	  /// <summary>
	  /// The XPath object contained in this part.
	  /// @serial
	  /// </summary>
	  private XPath m_xpath;

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame 
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		m_xpath.fixupVariables(vars, globalsSize);
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside 
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	   public override bool canTraverseOutsideSubtree()
	   {
		return m_xpath.Expression.canTraverseOutsideSubtree();
	   }

	  /// <summary>
	  /// Construct a simple AVT part.
	  /// </summary>
	  /// <param name="xpath"> Xpath section of AVT  </param>
	  public AVTPartXPath(XPath xpath)
	  {
		m_xpath = xpath;
	  }

	  /// <summary>
	  /// Construct a simple AVT part.
	  /// </summary>
	  /// <param name="val"> A pure string section of an AVT. </param>
	  /// <param name="nsNode"> An object which can be used to determine the
	  /// Namespace Name (URI) for any Namespace prefix used in the XPath. 
	  /// Usually this is based on the context where the XPath was specified,
	  /// such as a node within a Stylesheet. </param>
	  /// <param name="xpathProcessor"> XPath parser </param>
	  /// <param name="factory"> XPath factory </param>
	  /// <param name="liaison"> An XPathContext object, providing infomation specific
	  /// to this invocation and this thread. Maintains SAX output state, 
	  /// variables, error handler and so on, so the transformation/XPath 
	  /// object itself can be simultaneously invoked from multiple threads.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// TODO: Fix or remove this unused c'tor. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public AVTPartXPath(String val, org.apache.xml.utils.PrefixResolver nsNode, org.apache.xpath.compiler.XPathParser xpathProcessor, org.apache.xpath.XPathFactory factory, org.apache.xpath.XPathContext liaison) throws javax.xml.transform.TransformerException
	  public AVTPartXPath(string val, org.apache.xml.utils.PrefixResolver nsNode, XPathParser xpathProcessor, XPathFactory factory, XPathContext liaison)
	  {
		m_xpath = new XPath(val, null, nsNode, XPath.SELECT, liaison.ErrorListener);
	  }

	  /// <summary>
	  /// Get the AVT part as the original string.
	  /// </summary>
	  /// <returns> the AVT part as the original string. </returns>
	  public override string SimpleString
	  {
		  get
		  {
			return "{" + m_xpath.PatternString + "}";
		  }
	  }

	  /// <summary>
	  /// Write the value into the buffer.
	  /// </summary>
	  /// <param name="xctxt"> An XPathContext object, providing infomation specific
	  /// to this invocation and this thread. Maintains SAX state, variables, 
	  /// error handler and  so on, so the transformation/XPath object itself
	  /// can be simultaneously invoked from multiple threads. </param>
	  /// <param name="buf"> Buffer to write into. </param>
	  /// <param name="context"> The current source tree context. </param>
	  /// <param name="nsNode"> The current namespace context (stylesheet tree context).
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void evaluate(org.apache.xpath.XPathContext xctxt, org.apache.xml.utils.FastStringBuffer buf, int context, org.apache.xml.utils.PrefixResolver nsNode) throws javax.xml.transform.TransformerException
	  public override void evaluate(XPathContext xctxt, FastStringBuffer buf, int context, org.apache.xml.utils.PrefixResolver nsNode)
	  {

		XObject xobj = m_xpath.execute(xctxt, context, nsNode);

		if (null != xobj)
		{
		  xobj.appendToFsb(buf);
		}
	  }

	  /// <seealso cref="XSLTVisitable.callVisitors(XSLTVisitor)"/>
	  public override void callVisitors(XSLTVisitor visitor)
	  {
		  m_xpath.Expression.callVisitors(m_xpath, visitor);
	  }
	}

}