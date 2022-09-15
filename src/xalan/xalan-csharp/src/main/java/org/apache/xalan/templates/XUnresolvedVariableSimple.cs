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
 * $Id: XUnresolvedVariableSimple.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	using Expression = org.apache.xpath.Expression;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;


	/// <summary>
	/// This is the same as XUnresolvedVariable, but it assumes that the 
	/// context is already set up.  For use with psuedo variables.
	/// Also, it holds an Expression object, instead of an ElemVariable.
	/// It must only hold static context, since a single copy will be 
	/// held in the template.
	/// </summary>
	[Serializable]
	public class XUnresolvedVariableSimple : XObject
	{
		internal new const long serialVersionUID = -1224413807443958985L;
	  public XUnresolvedVariableSimple(ElemVariable obj) : base(obj)
	  {
	  }


	  /// <summary>
	  /// For support of literal objects in xpaths.
	  /// </summary>
	  /// <param name="xctxt"> The XPath execution context.
	  /// </param>
	  /// <returns> This object.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		  Expression expr = ((ElemVariable)m_obj).Select.getExpression();
		XObject xobj = expr.execute(xctxt);
		xobj.allowDetachToRelease(false);
		return xobj;
	  }

	  /// <summary>
	  /// Tell what kind of class this is.
	  /// </summary>
	  /// <returns> CLASS_UNRESOLVEDVARIABLE </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_UNRESOLVEDVARIABLE;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> An informational string. </returns>
	  public override string TypeString
	  {
		  get
		  {
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return "XUnresolvedVariableSimple (" + @object().GetType().FullName + ")";
		  }
	  }


	}


}