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
 * $Id: FuncNormalizeSpace.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using DTM = org.apache.xml.dtm.DTM;
	using XMLString = org.apache.xml.utils.XMLString;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;
	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// Execute the normalize-space() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncNormalizeSpace : FunctionDef1Arg
	{
		internal new const long serialVersionUID = -3377956872032190880L;

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
		XMLString s1 = getArg0AsString(xctxt);

		return (XString)s1.fixWhiteSpace(true, true, false);
	  }

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the 
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception 
	  ///         occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void executeCharsToContentHandler(org.apache.xpath.XPathContext xctxt, org.xml.sax.ContentHandler handler) throws javax.xml.transform.TransformerException, org.xml.sax.SAXException
	  public override void executeCharsToContentHandler(XPathContext xctxt, ContentHandler handler)
	  {
		if (Arg0IsNodesetExpr())
		{
		  int node = getArg0AsNode(xctxt);
		  if (DTM.NULL != node)
		  {
			DTM dtm = xctxt.getDTM(node);
			dtm.dispatchCharactersEvents(node, handler, true);
		  }
		}
		else
		{
		  XObject obj = execute(xctxt);
		  obj.dispatchCharactersEvents(handler);
		}
	  }

	}

}