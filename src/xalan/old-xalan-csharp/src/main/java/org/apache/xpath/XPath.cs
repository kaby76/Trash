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
 * $Id: XPath.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using FunctionTable = org.apache.xpath.compiler.FunctionTable;
	using XPathParser = org.apache.xpath.compiler.XPathParser;
	using Function = org.apache.xpath.functions.Function;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// The XPath class wraps an expression object and provides general services 
	/// for execution of that expression.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class XPath : ExpressionOwner
	{
		internal const long serialVersionUID = 3976493477939110553L;

	  /// <summary>
	  /// The top of the expression tree. 
	  ///  @serial 
	  /// </summary>
	  private Expression m_mainExp;

	  /// <summary>
	  /// The function table for xpath build-in functions
	  /// </summary>
	  [NonSerialized]
	  private FunctionTable m_funcTable = null;

	  /// <summary>
	  /// initial the function table
	  /// </summary>
	  private void initFunctionTable()
	  {
				m_funcTable = new FunctionTable();
	  }

	  /// <summary>
	  /// Get the raw Expression object that this class wraps.
	  /// 
	  /// </summary>
	  /// <returns> the raw Expression object, which should not normally be null. </returns>
	  public virtual Expression Expression
	  {
		  get
		  {
			return m_mainExp;
		  }
		  set
		  {
			  if (null != m_mainExp)
			  {
				value.exprSetParent(m_mainExp.exprGetParent()); // a bit bogus
			  }
			m_mainExp = value;
		  }
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
	  public virtual void fixupVariables(ArrayList vars, int globalsSize)
	  {
		m_mainExp.fixupVariables(vars, globalsSize);
	  }


	  /// <summary>
	  /// Get the SourceLocator on the expression object.
	  /// 
	  /// </summary>
	  /// <returns> the SourceLocator on the expression object, which may be null. </returns>
	  public virtual SourceLocator Locator
	  {
		  get
		  {
			return m_mainExp;
		  }
	  }

	//  /**
	//   * Set the SourceLocator on the expression object.
	//   *
	//   *
	//   * @param l the SourceLocator on the expression object, which may be null.
	//   */
	//  public void setLocator(SourceLocator l)
	//  {
	//    // Note potential hazards -- l may not be serializable, or may be changed
	//      // after being assigned here.
	//    m_mainExp.setSourceLocator(l);
	//  }

	  /// <summary>
	  /// The pattern string, mainly kept around for diagnostic purposes.
	  ///  @serial  
	  /// </summary>
	  internal string m_patternString;

	  /// <summary>
	  /// Return the XPath string associated with this object.
	  /// 
	  /// </summary>
	  /// <returns> the XPath string associated with this object. </returns>
	  public virtual string PatternString
	  {
		  get
		  {
			return m_patternString;
		  }
	  }

	  /// <summary>
	  /// Represents a select type expression. </summary>
	  public const int SELECT = 0;

	  /// <summary>
	  /// Represents a match type expression. </summary>
	  public const int MATCH = 1;

	  /// <summary>
	  /// Construct an XPath object.  
	  /// 
	  /// (Needs review -sc) This method initializes an XPathParser/
	  /// Compiler and compiles the expression. </summary>
	  /// <param name="exprString"> The XPath expression. </param>
	  /// <param name="locator"> The location of the expression, may be null. </param>
	  /// <param name="prefixResolver"> A prefix resolver to use to resolve prefixes to 
	  ///                       namespace URIs. </param>
	  /// <param name="type"> one of <seealso cref="#SELECT"/> or <seealso cref="#MATCH"/>. </param>
	  /// <param name="errorListener"> The error listener, or null if default should be used.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> if syntax or other error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public XPath(String exprString, javax.xml.transform.SourceLocator locator, org.apache.xml.utils.PrefixResolver prefixResolver, int type, javax.xml.transform.ErrorListener errorListener) throws javax.xml.transform.TransformerException
	  public XPath(string exprString, SourceLocator locator, PrefixResolver prefixResolver, int type, ErrorListener errorListener)
	  {
		initFunctionTable();
		if (null == errorListener)
		{
		  errorListener = new org.apache.xml.utils.DefaultErrorHandler();
		}

		m_patternString = exprString;

		XPathParser parser = new XPathParser(errorListener, locator);
		Compiler compiler = new Compiler(errorListener, locator, m_funcTable);

		if (SELECT == type)
		{
		  parser.initXPath(compiler, exprString, prefixResolver);
		}
		else if (MATCH == type)
		{
		  parser.initMatchPattern(compiler, exprString, prefixResolver);
		}
		else
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_CANNOT_DEAL_XPATH_TYPE, new object[]{Convert.ToString(type)})); //"Can not deal with XPath type: " + type);
		}

		// System.out.println("----------------");
		Expression expr = compiler.compile(0);

		// System.out.println("expr: "+expr);
		this.Expression = expr;

		if ((null != locator) && locator is ExpressionNode)
		{
			expr.exprSetParent((ExpressionNode)locator);
		}

	  }

	  /// <summary>
	  /// Construct an XPath object.  
	  /// 
	  /// (Needs review -sc) This method initializes an XPathParser/
	  /// Compiler and compiles the expression. </summary>
	  /// <param name="exprString"> The XPath expression. </param>
	  /// <param name="locator"> The location of the expression, may be null. </param>
	  /// <param name="prefixResolver"> A prefix resolver to use to resolve prefixes to 
	  ///                       namespace URIs. </param>
	  /// <param name="type"> one of <seealso cref="#SELECT"/> or <seealso cref="#MATCH"/>. </param>
	  /// <param name="errorListener"> The error listener, or null if default should be used.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> if syntax or other error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public XPath(String exprString, javax.xml.transform.SourceLocator locator, org.apache.xml.utils.PrefixResolver prefixResolver, int type, javax.xml.transform.ErrorListener errorListener, org.apache.xpath.compiler.FunctionTable aTable) throws javax.xml.transform.TransformerException
	  public XPath(string exprString, SourceLocator locator, PrefixResolver prefixResolver, int type, ErrorListener errorListener, FunctionTable aTable)
	  {
		m_funcTable = aTable;
		if (null == errorListener)
		{
		  errorListener = new org.apache.xml.utils.DefaultErrorHandler();
		}

		m_patternString = exprString;

		XPathParser parser = new XPathParser(errorListener, locator);
		Compiler compiler = new Compiler(errorListener, locator, m_funcTable);

		if (SELECT == type)
		{
		  parser.initXPath(compiler, exprString, prefixResolver);
		}
		else if (MATCH == type)
		{
		  parser.initMatchPattern(compiler, exprString, prefixResolver);
		}
		else
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_CANNOT_DEAL_XPATH_TYPE, new object[]{Convert.ToString(type)}));
		}
				//"Can not deal with XPath type: " + type);

		// System.out.println("----------------");
		Expression expr = compiler.compile(0);

		// System.out.println("expr: "+expr);
		this.Expression = expr;

		if ((null != locator) && locator is ExpressionNode)
		{
			expr.exprSetParent((ExpressionNode)locator);
		}

	  }

	  /// <summary>
	  /// Construct an XPath object.  
	  /// 
	  /// (Needs review -sc) This method initializes an XPathParser/
	  /// Compiler and compiles the expression. </summary>
	  /// <param name="exprString"> The XPath expression. </param>
	  /// <param name="locator"> The location of the expression, may be null. </param>
	  /// <param name="prefixResolver"> A prefix resolver to use to resolve prefixes to 
	  ///                       namespace URIs. </param>
	  /// <param name="type"> one of <seealso cref="#SELECT"/> or <seealso cref="#MATCH"/>.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> if syntax or other error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public XPath(String exprString, javax.xml.transform.SourceLocator locator, org.apache.xml.utils.PrefixResolver prefixResolver, int type) throws javax.xml.transform.TransformerException
	  public XPath(string exprString, SourceLocator locator, PrefixResolver prefixResolver, int type) : this(exprString, locator, prefixResolver, type, null)
	  {
	  }

	  /// <summary>
	  /// Construct an XPath object.
	  /// </summary>
	  /// <param name="expr"> The Expression object.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> if syntax or other error. </exception>
	  public XPath(Expression expr)
	  {
		this.Expression = expr;
		initFunctionTable();
	  }

	  /// <summary>
	  /// Given an expression and a context, evaluate the XPath
	  /// and return the result.
	  /// </summary>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="contextNode"> The node that "." expresses. </param>
	  /// <param name="namespaceContext"> The context in which namespaces in the
	  /// XPath are supposed to be expanded.
	  /// </param>
	  /// <returns> The result of the XPath or null if callbacks are used. </returns>
	  /// <exception cref="TransformerException"> thrown if
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage experimental </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(XPathContext xctxt, org.w3c.dom.Node contextNode, org.apache.xml.utils.PrefixResolver namespaceContext) throws javax.xml.transform.TransformerException
	  public virtual XObject execute(XPathContext xctxt, org.w3c.dom.Node contextNode, PrefixResolver namespaceContext)
	  {
		return execute(xctxt, xctxt.getDTMHandleFromNode(contextNode), namespaceContext);
	  }


	  /// <summary>
	  /// Given an expression and a context, evaluate the XPath
	  /// and return the result.
	  /// </summary>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="contextNode"> The node that "." expresses. </param>
	  /// <param name="namespaceContext"> The context in which namespaces in the
	  /// XPath are supposed to be expanded.
	  /// </param>
	  /// <exception cref="TransformerException"> thrown if the active ProblemListener decides
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage experimental </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(XPathContext xctxt, int contextNode, org.apache.xml.utils.PrefixResolver namespaceContext) throws javax.xml.transform.TransformerException
	  public virtual XObject execute(XPathContext xctxt, int contextNode, PrefixResolver namespaceContext)
	  {

		xctxt.pushNamespaceContext(namespaceContext);

		xctxt.pushCurrentNodeAndExpression(contextNode, contextNode);

		XObject xobj = null;

		try
		{
		  xobj = m_mainExp.execute(xctxt);
		}
		catch (TransformerException te)
		{
		  te.Locator = this.Locator;
		  ErrorListener el = xctxt.ErrorListener;
		  if (null != el) // defensive, should never happen.
		  {
			el.error(te);
		  }
		  else
		  {
			throw te;
		  }
		}
		catch (Exception e)
		{
		  while (e is org.apache.xml.utils.WrappedRuntimeException)
		  {
			e = ((org.apache.xml.utils.WrappedRuntimeException) e).Exception;
		  }
		  // e.printStackTrace();

		  string msg = e.Message;

		  if (string.ReferenceEquals(msg, null) || msg.Length == 0)
		  {
			   msg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_XPATH_ERROR, null);

		  }
		  TransformerException te = new TransformerException(msg, Locator, e);
		  ErrorListener el = xctxt.ErrorListener;
		  // te.printStackTrace();
		  if (null != el) // defensive, should never happen.
		  {
			el.fatalError(te);
		  }
		  else
		  {
			throw te;
		  }
		}
		finally
		{
		  xctxt.popNamespaceContext();

		  xctxt.popCurrentNodeAndExpression();
		}

		return xobj;
	  }

	  /// <summary>
	  /// Given an expression and a context, evaluate the XPath
	  /// and return the result.
	  /// </summary>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="contextNode"> The node that "." expresses. </param>
	  /// <param name="namespaceContext"> The context in which namespaces in the
	  /// XPath are supposed to be expanded.
	  /// </param>
	  /// <exception cref="TransformerException"> thrown if the active ProblemListener decides
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage experimental </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean bool(XPathContext xctxt, int contextNode, org.apache.xml.utils.PrefixResolver namespaceContext) throws javax.xml.transform.TransformerException
	  public virtual bool @bool(XPathContext xctxt, int contextNode, PrefixResolver namespaceContext)
	  {

		xctxt.pushNamespaceContext(namespaceContext);

		xctxt.pushCurrentNodeAndExpression(contextNode, contextNode);

		try
		{
		  return m_mainExp.@bool(xctxt);
		}
		catch (TransformerException te)
		{
		  te.Locator = this.Locator;
		  ErrorListener el = xctxt.ErrorListener;
		  if (null != el) // defensive, should never happen.
		  {
			el.error(te);
		  }
		  else
		  {
			throw te;
		  }
		}
		catch (Exception e)
		{
		  while (e is org.apache.xml.utils.WrappedRuntimeException)
		  {
			e = ((org.apache.xml.utils.WrappedRuntimeException) e).Exception;
		  }
		  // e.printStackTrace();

		  string msg = e.Message;

		  if (string.ReferenceEquals(msg, null) || msg.Length == 0)
		  {
			   msg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_XPATH_ERROR, null);

		  }

		  TransformerException te = new TransformerException(msg, Locator, e);
		  ErrorListener el = xctxt.ErrorListener;
		  // te.printStackTrace();
		  if (null != el) // defensive, should never happen.
		  {
			el.fatalError(te);
		  }
		  else
		  {
			throw te;
		  }
		}
		finally
		{
		  xctxt.popNamespaceContext();

		  xctxt.popCurrentNodeAndExpression();
		}

		return false;
	  }

	  /// <summary>
	  /// Set to true to get diagnostic messages about the result of 
	  ///  match pattern testing.  
	  /// </summary>
	  private const bool DEBUG_MATCHES = false;

	  /// <summary>
	  /// Get the match score of the given node.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context. </param>
	  /// <param name="context"> The current source tree context node.
	  /// </param>
	  /// <returns> score, one of <seealso cref="#MATCH_SCORE_NODETEST"/>,
	  /// <seealso cref="#MATCH_SCORE_NONE"/>, <seealso cref="#MATCH_SCORE_OTHER"/>, 
	  /// or <seealso cref="#MATCH_SCORE_QNAME"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getMatchScore(XPathContext xctxt, int context) throws javax.xml.transform.TransformerException
	  public virtual double getMatchScore(XPathContext xctxt, int context)
	  {

		xctxt.pushCurrentNode(context);
		xctxt.pushCurrentExpressionNode(context);

		try
		{
		  XObject score = m_mainExp.execute(xctxt);

		  if (DEBUG_MATCHES)
		  {
			DTM dtm = xctxt.getDTM(context);
			Console.WriteLine("score: " + score.num() + " for " + dtm.getNodeName(context) + " for xpath " + this.PatternString);
		  }

		  return score.num();
		}
		finally
		{
		  xctxt.popCurrentNode();
		  xctxt.popCurrentExpressionNode();
		}

		// return XPath.MATCH_SCORE_NONE;
	  }


	  /// <summary>
	  /// Warn the user of an problem.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="sourceNode"> Not used. </param>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warn(XPathContext xctxt, int sourceNode, String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void warn(XPathContext xctxt, int sourceNode, string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHWarning(msg, args);
		ErrorListener ehandler = xctxt.ErrorListener;

		if (null != ehandler)
		{

		  // TO DO: Need to get stylesheet Locator from here.
		  ehandler.warning(new TransformerException(fmsg, (SAXSourceLocator)xctxt.SAXLocator));
		}
	  }

	  /// <summary>
	  /// Tell the user of an assertion error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="b">  If false, a runtime exception will be thrown. </param>
	  /// <param name="msg"> The assertion message, which should be informative.
	  /// </param>
	  /// <exception cref="RuntimeException"> if the b argument is false. </exception>
	  public virtual void assertion(bool b, string msg)
	  {

		if (!b)
		{
		  string fMsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_INCORRECT_PROGRAMMER_ASSERTION, new object[]{msg});

		  throw new Exception(fMsg);
		}
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="sourceNode"> Not used. </param>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(XPathContext xctxt, int sourceNode, String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void error(XPathContext xctxt, int sourceNode, string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);
		ErrorListener ehandler = xctxt.ErrorListener;

		if (null != ehandler)
		{
		  ehandler.fatalError(new TransformerException(fmsg, (SAXSourceLocator)xctxt.SAXLocator));
		}
		else
		{
		  SourceLocator slocator = xctxt.SAXLocator;
		  Console.WriteLine(fmsg + "; file " + slocator.SystemId + "; line " + slocator.LineNumber + "; column " + slocator.ColumnNumber);
		}
	  }

	  /// <summary>
	  /// This will traverse the heararchy, calling the visitor for 
	  /// each member.  If the called visitor method returns 
	  /// false, the subtree should not be called.
	  /// </summary>
	  /// <param name="owner"> The owner of the visitor, where that path may be 
	  ///              rewritten if needed. </param>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  public virtual void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  m_mainExp.callVisitors(this, visitor);
	  }

	  /// <summary>
	  /// The match score if no match is made.
	  /// @xsl.usage advanced
	  /// </summary>
	  public static readonly double MATCH_SCORE_NONE = double.NegativeInfinity;

	  /// <summary>
	  /// The match score if the pattern has the form
	  /// of a QName optionally preceded by an @ character.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const double MATCH_SCORE_QNAME = 0.0;

	  /// <summary>
	  /// The match score if the pattern pattern has the form NCName:*.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const double MATCH_SCORE_NSWILD = -0.25;

	  /// <summary>
	  /// The match score if the pattern consists of just a NodeTest.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const double MATCH_SCORE_NODETEST = -0.5;

	  /// <summary>
	  /// The match score if the pattern consists of something
	  /// other than just a NodeTest or just a qname.
	  /// @xsl.usage advanced
	  /// </summary>
	  public const double MATCH_SCORE_OTHER = 0.5;
	}

}