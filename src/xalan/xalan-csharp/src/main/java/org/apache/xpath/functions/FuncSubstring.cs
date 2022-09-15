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
 * $Id: FuncSubstring.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XMLString = org.apache.xml.utils.XMLString;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Execute the Substring() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncSubstring : Function3Args
	{
		internal new const long serialVersionUID = -5996676095024715502L;

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

		XMLString s1 = m_arg0.execute(xctxt).xstr();
		double start = m_arg1.execute(xctxt).num();
		int lenOfS1 = s1.length();
		XMLString substr;

		if (lenOfS1 <= 0)
		{
		  return XString.EMPTYSTRING;
		}
		else
		{
		  int startIndex;

		  if (double.IsNaN(start))
		  {

			// Double.MIN_VALUE doesn't work with math below 
			// so just use a big number and hope I never get caught.
			start = -1000000;
			startIndex = 0;
		  }
		  else
		  {
			start = (long)Math.Round(start, MidpointRounding.AwayFromZero);
			startIndex = (start > 0) ? (int) start - 1 : 0;
		  }

		  if (null != m_arg2)
		  {
			double len = m_arg2.num(xctxt);
			int end = (int)((long)Math.Round(len, MidpointRounding.AwayFromZero) + start) - 1;

			// Normalize end index.
			if (end < 0)
			{
			  end = 0;
			}
			else if (end > lenOfS1)
			{
			  end = lenOfS1;
			}

			if (startIndex > lenOfS1)
			{
			  startIndex = lenOfS1;
			}

			substr = s1.substring(startIndex, end - startIndex);
		  }
		  else
		  {
			if (startIndex > lenOfS1)
			{
			  startIndex = lenOfS1;
			}
			substr = s1.substring(startIndex);
		  }
		}

		return (XString)substr; // cast semi-safe
	  }

	  /// <summary>
	  /// Check that the number of arguments passed to this function is correct. 
	  /// 
	  /// </summary>
	  /// <param name="argNum"> The number of arguments that is being passed to the function.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void checkNumberArgs(int argNum) throws WrongNumberArgsException
	  public override void checkNumberArgs(int argNum)
	  {
		if (argNum < 2)
		{
		  reportWrongNumberArgs();
		}
	  }

	  /// <summary>
	  /// Constructs and throws a WrongNumberArgException with the appropriate
	  /// message for this function object.
	  /// </summary>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws WrongNumberArgsException
	  protected internal override void reportWrongNumberArgs()
	  {
		  throw new WrongNumberArgsException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_TWO_OR_THREE, null)); //"2 or 3");
	  }
	}

}