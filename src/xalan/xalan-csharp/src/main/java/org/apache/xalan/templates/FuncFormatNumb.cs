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
 * $Id: FuncFormatNumb.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using QName = org.apache.xml.utils.QName;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using Expression = org.apache.xpath.Expression;
	using XPathContext = org.apache.xpath.XPathContext;
	using Function3Args = org.apache.xpath.functions.Function3Args;
	using WrongNumberArgsException = org.apache.xpath.functions.WrongNumberArgsException;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;

	/// <summary>
	/// Execute the FormatNumber() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncFormatNumb : Function3Args
	{
		internal new const long serialVersionUID = -8869935264870858636L;

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

		// A bit of an ugly hack to get our context.
		ElemTemplateElement templElem = (ElemTemplateElement) xctxt.NamespaceContext;
		StylesheetRoot ss = templElem.StylesheetRoot;
		java.text.DecimalFormat formatter = null;
		java.text.DecimalFormatSymbols dfs = null;
		double num = Arg0.execute(xctxt).num();
		string patternStr = Arg1.execute(xctxt).str();

		// TODO: what should be the behavior here??
		if (patternStr.IndexOf(0x00A4) > 0)
		{
		  ss.error(XSLTErrorResources.ER_CURRENCY_SIGN_ILLEGAL); // currency sign not allowed
		}

		// this third argument is not a locale name. It is the name of a
		// decimal-format declared in the stylesheet!(xsl:decimal-format
		try
		{
		  Expression arg2Expr = Arg2;

		  if (null != arg2Expr)
		  {
			string dfName = arg2Expr.execute(xctxt).str();
			QName qname = new QName(dfName, xctxt.NamespaceContext);

			dfs = ss.getDecimalFormatComposed(qname);

			if (null == dfs)
			{
			  warn(xctxt, XSLTErrorResources.WG_NO_DECIMALFORMAT_DECLARATION, new object[]{dfName}); //"not found!!!

			  //formatter = new java.text.DecimalFormat(patternStr);
			}
			else
			{

			  //formatter = new java.text.DecimalFormat(patternStr, dfs);
			  formatter = new java.text.DecimalFormat();

			  formatter.setDecimalFormatSymbols(dfs);
			  formatter.applyLocalizedPattern(patternStr);
			}
		  }

		  //else
		  if (null == formatter)
		  {

			// look for a possible default decimal-format
			dfs = ss.getDecimalFormatComposed(new QName(""));

			if (dfs != null)
			{
			  formatter = new java.text.DecimalFormat();

			  formatter.setDecimalFormatSymbols(dfs);
			  formatter.applyLocalizedPattern(patternStr);
			}
			else
			{
			  dfs = new java.text.DecimalFormatSymbols(java.util.Locale.US);

			  dfs.setInfinity(Constants.ATTRVAL_INFINITY);
			  dfs.setNaN(Constants.ATTRVAL_NAN);

			  formatter = new java.text.DecimalFormat();

			  formatter.setDecimalFormatSymbols(dfs);

			  if (null != patternStr)
			  {
				formatter.applyLocalizedPattern(patternStr);
			  }
			}
		  }

		  return new XString(formatter.format(num));
		}
		catch (Exception)
		{
		  templElem.error(XSLTErrorResources.ER_MALFORMED_FORMAT_STRING, new object[]{patternStr});

		  return XString.EMPTYSTRING;

		  //throw new XSLProcessorException(iae);
		}
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state. </param>
	  /// <param name="msg"> Warning message key </param>
	  /// <param name="args"> Arguments to be used in warning message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warn(org.apache.xpath.XPathContext xctxt, String msg, Object args[]) throws javax.xml.transform.TransformerException
	  public override void warn(XPathContext xctxt, string msg, object[] args)
	  {

		string formattedMsg = XSLMessages.createWarning(msg, args);
		ErrorListener errHandler = xctxt.ErrorListener;

		errHandler.warning(new TransformerException(formattedMsg, (SAXSourceLocator)xctxt.SAXLocator));
	  }

	  /// <summary>
	  /// Overide the superclass method to allow one or two arguments. 
	  /// 
	  /// </summary>
	  /// <param name="argNum"> Number of arguments passed in
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void checkNumberArgs(int argNum) throws org.apache.xpath.functions.WrongNumberArgsException
	  public override void checkNumberArgs(int argNum)
	  {
		if ((argNum > 3) || (argNum < 2))
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
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws org.apache.xpath.functions.WrongNumberArgsException
	  protected internal override void reportWrongNumberArgs()
	  {
		  throw new WrongNumberArgsException(XSLMessages.createMessage(XSLTErrorResources.ER_TWO_OR_THREE, null)); //"2 or 3");
	  }
	}

}