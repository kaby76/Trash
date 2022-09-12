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
 * $Id: XPathException.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This class implements an exception object that all
	/// XPath classes will throw in case of an error.  This class
	/// extends TransformerException, and may hold other exceptions. In the
	/// case of nested exceptions, printStackTrace will dump
	/// all the traces of the nested exceptions, not just the trace
	/// of this object.
	/// @xsl.usage general
	/// </summary>
	public class XPathException : TransformerException
	{
		internal const long serialVersionUID = 4263549717619045963L;

	  /// <summary>
	  /// The home of the expression that caused the error.
	  ///  @serial  
	  /// </summary>
	  internal object m_styleNode = null;

	  /// <summary>
	  /// Get the stylesheet node from where this error originated. </summary>
	  /// <returns> The stylesheet node from where this error originated, or null. </returns>
	  public virtual object StylesheetNode
	  {
		  get
		  {
			return m_styleNode;
		  }
		  set
		  {
			m_styleNode = value;
		  }
	  }



	  /// <summary>
	  /// A nested exception.
	  ///  @serial   
	  /// </summary>
	  protected internal Exception m_exception;

	  /// <summary>
	  /// Create an XPathException object that holds
	  /// an error message. </summary>
	  /// <param name="message"> The error message. </param>
	  public XPathException(string message, ExpressionNode ex) : base(message)
	  {
		this.Locator = ex;
		StylesheetNode = getStylesheetNode(ex);
	  }

	  /// <summary>
	  /// Create an XPathException object that holds
	  /// an error message. </summary>
	  /// <param name="message"> The error message. </param>
	  public XPathException(string message) : base(message)
	  {
	  }


	  /// <summary>
	  /// Get the XSLT ElemVariable that this sub-expression references.  In order for 
	  /// this to work, the SourceLocator must be the owning ElemTemplateElement. </summary>
	  /// <returns> The dereference to the ElemVariable, or null if not found. </returns>
	  public virtual Node getStylesheetNode(ExpressionNode ex)
	  {

		ExpressionNode owner = getExpressionOwner(ex);

		if (null != owner && owner is Node)
		{
			return ((Node)owner);
		}
		return null;

	  }

	  /// <summary>
	  /// Get the first non-Expression parent of this node. </summary>
	  /// <returns> null or first ancestor that is not an Expression. </returns>
	  protected internal virtual ExpressionNode getExpressionOwner(ExpressionNode ex)
	  {
		  ExpressionNode parent = ex.exprGetParent();
		  while ((null != parent) && (parent is Expression))
		  {
			  parent = parent.exprGetParent();
		  }
		  return parent;
	  }



	  /// <summary>
	  /// Create an XPathException object that holds
	  /// an error message and the stylesheet node that
	  /// the error originated from. </summary>
	  /// <param name="message"> The error message. </param>
	  /// <param name="styleNode"> The stylesheet node that the error originated from. </param>
	  public XPathException(string message, object styleNode) : base(message)
	  {


		m_styleNode = styleNode;
	  }

	  /// <summary>
	  /// Create an XPathException object that holds
	  /// an error message, the stylesheet node that
	  /// the error originated from, and another exception
	  /// that caused this exception. </summary>
	  /// <param name="message"> The error message. </param>
	  /// <param name="styleNode"> The stylesheet node that the error originated from. </param>
	  /// <param name="e"> The exception that caused this exception. </param>
	  public XPathException(string message, Node styleNode, Exception e) : base(message)
	  {


		m_styleNode = styleNode;
		this.m_exception = e;
	  }

	  /// <summary>
	  /// Create an XPathException object that holds
	  /// an error message, and another exception
	  /// that caused this exception. </summary>
	  /// <param name="message"> The error message. </param>
	  /// <param name="e"> The exception that caused this exception. </param>
	  public XPathException(string message, Exception e) : base(message)
	  {


		this.m_exception = e;
	  }

	  /// <summary>
	  /// Print the the trace of methods from where the error
	  /// originated.  This will trace all nested exception
	  /// objects, as well as this object. </summary>
	  /// <param name="s"> The stream where the dump will be sent to. </param>
	  public virtual void printStackTrace(java.io.PrintStream s)
	  {

		if (s == null)
		{
		  s = System.err;
		}

		try
		{
		  base.printStackTrace(s);
		}
		catch (Exception)
		{
		}

		Exception exception = m_exception;

		for (int i = 0; (i < 10) && (null != exception); i++)
		{
		  s.println("---------");
		  exception.printStackTrace(s);

		  if (exception is TransformerException)
		  {
			TransformerException se = (TransformerException) exception;
			Exception prev = exception;

			exception = se.Exception;

			if (prev == exception)
			{
			  break;
			}
		  }
		  else
		  {
			exception = null;
		  }
		}
	  }

	  /// <summary>
	  /// Find the most contained message.
	  /// </summary>
	  /// <returns> The error message of the originating exception. </returns>
	  public virtual string Message
	  {
		  get
		  {
    
			string lastMessage = base.Message;
			Exception exception = m_exception;
    
			while (null != exception)
			{
			  string nextMessage = exception.Message;
    
			  if (null != nextMessage)
			  {
				lastMessage = nextMessage;
			  }
    
			  if (exception is TransformerException)
			  {
				TransformerException se = (TransformerException) exception;
				Exception prev = exception;
    
				exception = se.Exception;
    
				if (prev == exception)
				{
				  break;
				}
			  }
			  else
			  {
				exception = null;
			  }
			}
    
			return (null != lastMessage) ? lastMessage : "";
		  }
	  }

	  /// <summary>
	  /// Print the the trace of methods from where the error
	  /// originated.  This will trace all nested exception
	  /// objects, as well as this object. </summary>
	  /// <param name="s"> The writer where the dump will be sent to. </param>
	  public virtual void printStackTrace(java.io.PrintWriter s)
	  {

		if (s == null)
		{
		  s = new java.io.PrintWriter(System.err);
		}

		try
		{
		  base.printStackTrace(s);
		}
		catch (Exception)
		{
		}


		bool isJdk14OrHigher = false;
		try
		{
			typeof(Exception).GetMethod("getCause",null);
			isJdk14OrHigher = true;
		}
		catch (NoSuchMethodException)
		{
			// do nothing
		}

		// The printStackTrace method of the Throwable class in jdk 1.4 
		// and higher will include the cause when printing the backtrace.
		// The following code is only required when using jdk 1.3 or lower               
		if (!isJdk14OrHigher)
		{

		  Exception exception = m_exception;

		  for (int i = 0; (i < 10) && (null != exception); i++)
		  {
			s.println("---------");

			try
			{
			  exception.printStackTrace(s);
			}
			catch (Exception)
			{
			  s.println("Could not print stack trace...");
			}

			if (exception is TransformerException)
			{
			  TransformerException se = (TransformerException) exception;
			  Exception prev = exception;

			  exception = se.Exception;

			  if (prev == exception)
			  {
				exception = null;

				break;
			  }
			}
			else
			{
			  exception = null;
			}
		  }
		}
	  }

	  /// <summary>
	  ///  Return the embedded exception, if any.
	  ///  Overrides javax.xml.transform.TransformerException.getException().
	  /// </summary>
	  ///  <returns> The embedded exception, or null if there is none. </returns>
	  public virtual Exception Exception
	  {
		  get
		  {
			return m_exception;
		  }
	  }
	}

}