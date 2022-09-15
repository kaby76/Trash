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
 * $Id: FuncGenerateId.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{

	using DTM = org.apache.xml.dtm.DTM;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;

	/// <summary>
	/// Execute the GenerateId() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncGenerateId : FunctionDef1Arg
	{
		internal new const long serialVersionUID = 973544842091724273L;

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		int which = getArg0AsNode(xctxt);

		if (org.apache.xml.dtm.DTM_Fields.NULL != which)
		{
		  // Note that this is a different value than in previous releases
		  // of Xalan. It's sensitive to the exact encoding of the node
		  // handle anyway, so fighting to maintain backward compatability
		  // really didn't make sense; it may change again as we continue
		  // to experiment with balancing document and node numbers within
		  // that value.
		  return new XString("N" + which.ToString("x").ToUpper());
		}
		else
		{
		  return XString.EMPTYSTRING;
		}
	  }
	}

}