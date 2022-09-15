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
 * $Id: XRTreeFragSelectWrapper.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XMLString = org.apache.xml.utils.XMLString;
	using Expression = org.apache.xpath.Expression;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// This class makes an select statement act like an result tree fragment.
	/// </summary>
	[Serializable]
	public class XRTreeFragSelectWrapper : XRTreeFrag, ICloneable
	{
		internal new const long serialVersionUID = -6526177905590461251L;
	  public XRTreeFragSelectWrapper(Expression expr) : base(expr)
	  {
	  }

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
		((Expression)m_obj).fixupVariables(vars, globalsSize);
	  }

	  /// <summary>
	  /// For support of literal objects in xpaths.
	  /// </summary>
	  /// <param name="xctxt"> The XPath execution context.
	  /// </param>
	  /// <returns> the result of executing the select expression
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		 XObject m_selected;
		 m_selected = ((Expression)m_obj).execute(xctxt);
		 m_selected.allowDetachToRelease(m_allowRelease);
		 if (m_selected.Type == CLASS_STRING)
		 {
		   return m_selected;
		 }
		 else
		 {
		   return new XString(m_selected.str());
		 }
	  }

	  /// <summary>
	  /// Detaches the <code>DTMIterator</code> from the set which it iterated
	  /// over, releasing any computational resources and placing the iterator
	  /// in the INVALID state. After <code>detach</code> has been invoked,
	  /// calls to <code>nextNode</code> or <code>previousNode</code> will
	  /// raise a runtime exception.
	  /// 
	  /// In general, detach should only be called once on the object.
	  /// </summary>
	  public override void detach()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, null)); //"detach() not supported by XRTreeFragSelectWrapper!");
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> The result tree fragment as a number or NaN </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double num() throws javax.xml.transform.TransformerException
	  public override double num()
	  {

		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, null)); //"num() not supported by XRTreeFragSelectWrapper!");
	  }


	  /// <summary>
	  /// Cast result object to an XMLString.
	  /// </summary>
	  /// <returns> The document fragment node data or the empty string.  </returns>
	  public override XMLString xstr()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, null)); //"xstr() not supported by XRTreeFragSelectWrapper!");
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The document fragment node data or the empty string.  </returns>
	  public override string str()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, null)); //"str() not supported by XRTreeFragSelectWrapper!");
	  }

	  /// <summary>
	  /// Tell what kind of class this is.
	  /// </summary>
	  /// <returns> the string type </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_STRING;
		  }
	  }

	  /// <summary>
	  /// Cast result object to a result tree fragment.
	  /// </summary>
	  /// <returns> The document fragment this wraps </returns>
	  public override int rtf()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, null)); //"rtf() not supported by XRTreeFragSelectWrapper!");
	  }

	  /// <summary>
	  /// Cast result object to a DTMIterator.
	  /// </summary>
	  /// <returns> The document fragment as a DTMIterator </returns>
	  public override DTMIterator asNodeIterator()
	  {
		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, null)); //"asNodeIterator() not supported by XRTreeFragSelectWrapper!");
	  }

	}

}