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
 * $Id: MsgMgr.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This class will manage error messages, warning messages, and other types of
	/// message events.
	/// </summary>
	public class MsgMgr
	{

	  /// <summary>
	  /// Create a message manager object.
	  /// </summary>
	  /// <param name="transformer"> non transformer instance </param>
	  public MsgMgr(TransformerImpl transformer)
	  {
		m_transformer = transformer;
	  }

	  /// <summary>
	  /// Transformer instance </summary>
	  private TransformerImpl m_transformer;

	  /// <summary>
	  /// Warn the user of a problem.
	  /// This is public for access by extensions.
	  /// </summary>
	  /// <param name="msg"> The message text to issue </param>
	  /// <param name="terminate"> Flag indicating whether to terminate this process </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void message(javax.xml.transform.SourceLocator srcLctr, String msg, boolean terminate) throws javax.xml.transform.TransformerException
	  public virtual void message(SourceLocator srcLctr, string msg, bool terminate)
	  {

		ErrorListener errHandler = m_transformer.ErrorListener;

		if (null != errHandler)
		{
		  errHandler.warning(new TransformerException(msg, srcLctr));
		}
		else
		{
		  if (terminate)
		  {
			throw new TransformerException(msg, srcLctr);
		  }
		  else
		  {
			Console.WriteLine(msg);
		  }
		}
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// </summary>
	  /// <param name="msg"> Message text to issue </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warn(javax.xml.transform.SourceLocator srcLctr, String msg) throws javax.xml.transform.TransformerException
	  public virtual void warn(SourceLocator srcLctr, string msg)
	  {
		warn(srcLctr, null, null, msg, null);
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// </summary>
	  /// <param name="msg"> Message text to issue </param>
	  /// <param name="args"> Arguments to pass to the message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warn(javax.xml.transform.SourceLocator srcLctr, String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void warn(SourceLocator srcLctr, string msg, object[] args)
	  {
		warn(srcLctr, null, null, msg, args);
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// 
	  /// </summary>
	  /// <param name="styleNode"> Stylesheet node </param>
	  /// <param name="sourceNode"> Source tree node </param>
	  /// <param name="msg"> Message text to issue </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warn(javax.xml.transform.SourceLocator srcLctr, org.w3c.dom.Node styleNode, org.w3c.dom.Node sourceNode, String msg) throws javax.xml.transform.TransformerException
	  public virtual void warn(SourceLocator srcLctr, Node styleNode, Node sourceNode, string msg)
	  {
		warn(srcLctr, styleNode, sourceNode, msg, null);
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// </summary>
	  /// <param name="styleNode"> Stylesheet node </param>
	  /// <param name="sourceNode"> Source tree node </param>
	  /// <param name="msg"> Message text to issue </param>
	  /// <param name="args"> Arguments to pass to the message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warn(javax.xml.transform.SourceLocator srcLctr, org.w3c.dom.Node styleNode, org.w3c.dom.Node sourceNode, String msg, Object args[]) throws javax.xml.transform.TransformerException
	  public virtual void warn(SourceLocator srcLctr, Node styleNode, Node sourceNode, string msg, object[] args)
	  {

		string formattedMsg = XSLMessages.createWarning(msg, args);
		ErrorListener errHandler = m_transformer.ErrorListener;

		if (null != errHandler)
		{
		  errHandler.warning(new TransformerException(formattedMsg, srcLctr));
		}
		else
		{
		  Console.WriteLine(formattedMsg);
		}
	  }

	  /* This method is not properly i18nized. We need to use the following method
	   * Tell the user of an error, and probably throw an
	   * exception.
	   *
	   * @param msg Message text to issue
	   * @throws XSLProcessorException thrown if the active ProblemListener and XPathContext decide
	   * the error condition is severe enough to halt processing.
	   *
	   * @throws TransformerException
	   *
	  public void error(SourceLocator srcLctr, String msg) throws TransformerException
	  {
	
	    // Locator locator = m_stylesheetLocatorStack.isEmpty()
	    //                  ? null :
	    //                    ((Locator)m_stylesheetLocatorStack.peek());
	    // Locator locator = null;
	    ErrorListener errHandler = m_transformer.getErrorListener();
	
	    if (null != errHandler)
	      errHandler.fatalError(new TransformerException(msg, srcLctr));
	    else
	      throw new TransformerException(msg, srcLctr);
	  }
	
	 * @xsl.usage internal
	 */

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> Message text to issue </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.SourceLocator srcLctr, String msg) throws javax.xml.transform.TransformerException
	  public virtual void error(SourceLocator srcLctr, string msg)
	  {
		error(srcLctr, null, null, msg, null);
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> Message text to issue </param>
	  /// <param name="args"> Arguments to be passed to the message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.SourceLocator srcLctr, String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void error(SourceLocator srcLctr, string msg, object[] args)
	  {
		error(srcLctr, null, null, msg, args);
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> Message text to issue </param>
	  /// <param name="e"> Exception to throw </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.SourceLocator srcLctr, String msg, Exception e) throws javax.xml.transform.TransformerException
	  public virtual void error(SourceLocator srcLctr, string msg, Exception e)
	  {
		error(srcLctr, msg, null, e);
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> Message text to issue </param>
	  /// <param name="args"> Arguments to use in message </param>
	  /// <param name="e"> Exception to throw </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.SourceLocator srcLctr, String msg, Object args[], Exception e) throws javax.xml.transform.TransformerException
	  public virtual void error(SourceLocator srcLctr, string msg, object[] args, Exception e)
	  {

		//msg  = (null == msg) ? XSLTErrorResources.ER_PROCESSOR_ERROR : msg;
		string formattedMsg = XSLMessages.createMessage(msg, args);

		// Locator locator = m_stylesheetLocatorStack.isEmpty()
		//                   ? null :
		//                    ((Locator)m_stylesheetLocatorStack.peek());
		// Locator locator = null;
		ErrorListener errHandler = m_transformer.ErrorListener;

		if (null != errHandler)
		{
		  errHandler.fatalError(new TransformerException(formattedMsg, srcLctr));
		}
		else
		{
		  throw new TransformerException(formattedMsg, srcLctr);
		}
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="styleNode"> Stylesheet node </param>
	  /// <param name="sourceNode"> Source tree node </param>
	  /// <param name="msg"> Message text to issue </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.SourceLocator srcLctr, org.w3c.dom.Node styleNode, org.w3c.dom.Node sourceNode, String msg) throws javax.xml.transform.TransformerException
	  public virtual void error(SourceLocator srcLctr, Node styleNode, Node sourceNode, string msg)
	  {
		error(srcLctr, styleNode, sourceNode, msg, null);
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="styleNode"> Stylesheet node </param>
	  /// <param name="sourceNode"> Source tree node </param>
	  /// <param name="msg"> Message text to issue </param>
	  /// <param name="args"> Arguments to use in message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(javax.xml.transform.SourceLocator srcLctr, org.w3c.dom.Node styleNode, org.w3c.dom.Node sourceNode, String msg, Object args[]) throws javax.xml.transform.TransformerException
	  public virtual void error(SourceLocator srcLctr, Node styleNode, Node sourceNode, string msg, object[] args)
	  {

		string formattedMsg = XSLMessages.createMessage(msg, args);

		// Locator locator = m_stylesheetLocatorStack.isEmpty()
		//                   ? null :
		//                    ((Locator)m_stylesheetLocatorStack.peek());
		// Locator locator = null;
		ErrorListener errHandler = m_transformer.ErrorListener;

		if (null != errHandler)
		{
		  errHandler.fatalError(new TransformerException(formattedMsg, srcLctr));
		}
		else
		{
		  throw new TransformerException(formattedMsg, srcLctr);
		}
	  }
	}

}