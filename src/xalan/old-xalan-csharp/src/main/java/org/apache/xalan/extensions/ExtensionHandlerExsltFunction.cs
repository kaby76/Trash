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
 * $Id: ExtensionHandlerExsltFunction.java 469672 2006-10-31 21:56:19Z minchau $
 */
namespace org.apache.xalan.extensions
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using Constants = org.apache.xalan.templates.Constants;
	using ElemExsltFuncResult = org.apache.xalan.templates.ElemExsltFuncResult;
	using ElemExsltFunction = org.apache.xalan.templates.ElemExsltFunction;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;
	using ExpressionNode = org.apache.xpath.ExpressionNode;
	using XPathContext = org.apache.xpath.XPathContext;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;

	/// <summary>
	/// Execute EXSLT functions, determine the availability of EXSLT functions, and the
	/// availability of an EXSLT result element.
	/// </summary>
	public class ExtensionHandlerExsltFunction : ExtensionHandler
	{
	  private string m_namespace;
	  private StylesheetRoot m_stylesheet;
	  private static readonly QName RESULTQNAME = new QName(Constants.S_EXSLT_FUNCTIONS_URL, Constants.EXSLT_ELEMNAME_FUNCRESULT_STRING);
	  /// <summary>
	  /// Constructor called from ElemExsltFunction runtimeInit().
	  /// </summary>
	  public ExtensionHandlerExsltFunction(string ns, StylesheetRoot stylesheet) : base(ns, "xml") // required by ExtensionHandler interface.
	  {
		m_namespace = ns;
		m_stylesheet = stylesheet;
	  }

	  /// <summary>
	  /// Required by ExtensionHandler (an abstract method). No-op.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processElement(String localPart, org.apache.xalan.templates.ElemTemplateElement element, org.apache.xalan.transformer.TransformerImpl transformer, org.apache.xalan.templates.Stylesheet stylesheetTree, Object methodKey) throws javax.xml.transform.TransformerException, java.io.IOException
	  public override void processElement(string localPart, ElemTemplateElement element, TransformerImpl transformer, Stylesheet stylesheetTree, object methodKey)
	  {
	  }

	  /// <summary>
	  /// Get the ElemExsltFunction element associated with the 
	  /// function.
	  /// </summary>
	  /// <param name="funcName"> Local name of the function. </param>
	  /// <returns> the ElemExsltFunction element associated with
	  /// the function, null if none exists. </returns>
	  public virtual ElemExsltFunction getFunction(string funcName)
	  {
		QName qname = new QName(m_namespace, funcName);
		ElemTemplate templ = m_stylesheet.getTemplateComposed(qname);
		if (templ != null && templ is ElemExsltFunction)
		{
		  return (ElemExsltFunction) templ;
		}
		else
		{
		  return null;
		}
	  }


	  /// <summary>
	  /// Does the EXSLT function exist?
	  /// </summary>
	  /// <param name="funcName"> Local name of the function. </param>
	  /// <returns> true if the function exists. </returns>
	  public override bool isFunctionAvailable(string funcName)
	  {
		return getFunction(funcName) != null;
	  }

	   /// <summary>
	   /// If an element-available() call applies to an EXSLT result element within 
	   /// an EXSLT function element, return true.
	   /// 
	   /// Note: The EXSLT function element is a template-level element, and 
	   /// element-available() returns false for it.
	   /// </summary>
	   /// <param name="elemName"> name of the element. </param>
	   /// <returns> true if the function is available. </returns>
	  public override bool isElementAvailable(string elemName)
	  {
		if (!((new QName(m_namespace, elemName)).Equals(RESULTQNAME)))
		{
		  return false;
		}
		else
		{
		  ElemTemplateElement elem = m_stylesheet.FirstChildElem;
		  while (elem != null && elem != m_stylesheet)
		  {
			if (elem is ElemExsltFuncResult && ancestorIsFunction(elem))
			{
			  return true;
			}
			ElemTemplateElement nextElem = elem.FirstChildElem;
			if (nextElem == null)
			{
			  nextElem = elem.NextSiblingElem;
			}
			if (nextElem == null)
			{
			  nextElem = elem.ParentElem;
			}
			elem = nextElem;
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Determine whether the func:result element is within a func:function element. 
	  /// If not, it is illegal.
	  /// </summary>
	  private bool ancestorIsFunction(ElemTemplateElement child)
	  {
		while (child.ParentElem != null && !(child.ParentElem is StylesheetRoot))
		{
		  if (child.ParentElem is ElemExsltFunction)
		  {
			return true;
		  }
		  child = child.ParentElem;
		}
		return false;
	  }

	  /// <summary>
	  /// Execute the EXSLT function and return the result value.
	  /// </summary>
	  /// <param name="funcName"> Name of the EXSLT function. </param>
	  /// <param name="args">     The arguments of the function call. </param>
	  /// <param name="methodKey"> Not used. </param>
	  /// <param name="exprContext"> Used to get the XPathContext. </param>
	  /// <returns> the return value of the function evaluation. </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object callFunction(String funcName, java.util.Vector args, Object methodKey, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(string funcName, ArrayList args, object methodKey, ExpressionContext exprContext)
	  {
		throw new TransformerException("This method should not be called.");
	  }

	  /// <summary>
	  /// Execute the EXSLT function and return the result value.
	  /// </summary>
	  /// <param name="extFunction"> The XPath extension function </param>
	  /// <param name="args"> The arguments of the function call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed. </param>
	  /// <returns> the return value of the function evaluation. </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object callFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector args, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(FuncExtFunction extFunction, ArrayList args, ExpressionContext exprContext)
	  {
		// Find the template which invokes this EXSLT function.
		ExpressionNode parent = extFunction.exprGetParent();
		while (parent != null && !(parent is ElemTemplate))
		{
		  parent = parent.exprGetParent();
		}

		ElemTemplate callerTemplate = (parent != null) ? (ElemTemplate)parent: null;

		XObject[] methodArgs;
		methodArgs = new XObject[args.Count];
		try
		{
		  for (int i = 0; i < methodArgs.Length; i++)
		  {
			methodArgs[i] = XObject.create(args[i]);
		  }

		  ElemExsltFunction elemFunc = getFunction(extFunction.FunctionName);

		  if (null != elemFunc)
		  {
			XPathContext context = exprContext.XPathContext;
			TransformerImpl transformer = (TransformerImpl)context.OwnerObject;
			transformer.pushCurrentFuncResult(null);

			elemFunc.execute(transformer, methodArgs);

			XObject val = (XObject)transformer.popCurrentFuncResult();
			return (val == null) ? new XString("") : val; // value if no result element.
		  }
		  else
		  {
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_FUNCTION_NOT_FOUND, new object[] {extFunction.FunctionName}));
		  }
		}
		catch (TransformerException e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new TransformerException(e);
		}
	  }

	}

}