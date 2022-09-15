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
 * $Id: FuncNamespace.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using DTM = org.apache.xml.dtm.DTM;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;

	/// <summary>
	/// Execute the Namespace() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncNamespace : FunctionDef1Arg
	{
		internal new const long serialVersionUID = -4695674566722321237L;

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

		int context = getArg0AsNode(xctxt);

		string s;
		if (context != DTM.NULL)
		{
		  DTM dtm = xctxt.getDTM(context);
		  int t = dtm.getNodeType(context);
		  if (t == DTM.ELEMENT_NODE)
		  {
			s = dtm.getNamespaceURI(context);
		  }
		  else if (t == DTM.ATTRIBUTE_NODE)
		  {

			// This function always returns an empty string for namespace nodes.
			// We check for those here.  Fix inspired by Davanum Srinivas.

			s = dtm.getNodeName(context);
			if (s.StartsWith("xmlns:", StringComparison.Ordinal) || s.Equals("xmlns"))
			{
			  return XString.EMPTYSTRING;
			}

			s = dtm.getNamespaceURI(context);
		  }
		  else
		  {
			return XString.EMPTYSTRING;
		  }
		}
		else
		{
		  return XString.EMPTYSTRING;
		}

		return ((null == s) ? XString.EMPTYSTRING : new XString(s));
	  }
	}

}