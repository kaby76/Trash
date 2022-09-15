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
 * $Id: Expression.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XMLString = org.apache.xml.utils.XMLString;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// This abstract class serves as the base for all expression objects.  An
	/// Expression can be executed to return a <seealso cref="org.apache.xpath.objects.XObject"/>,
	/// normally has a location within a document or DOM, can send error and warning
	/// events, and normally do not hold state and are meant to be immutable once
	/// construction has completed.  An exception to the immutibility rule is iterators
	/// and walkers, which must be cloned in order to be used -- the original must
	/// still be immutable.
	/// </summary>
	[Serializable]
	public abstract class Expression : ExpressionNode, XPathVisitable
	{
		public abstract void callVisitors(ExpressionOwner owner, XPathVisitor visitor);
		internal const long serialVersionUID = 565665869777906902L;
	  /// <summary>
	  /// The location where this expression was built from.  Need for diagnostic
	  ///  messages. May be null.
	  ///  @serial
	  /// </summary>
	  private ExpressionNode m_parent;

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public virtual bool canTraverseOutsideSubtree()
	  {
		return false;
	  }

	//  /**
	//   * Set the location where this expression was built from.
	//   *
	//   *
	//   * @param locator the location where this expression was built from, may be
	//   *                null.
	//   */
	//  public void setSourceLocator(SourceLocator locator)
	//  {
	//    m_slocator = locator;
	//  }

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="currentNode"> The currentNode.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(XPathContext xctxt, int currentNode) throws javax.xml.transform.TransformerException
	  public virtual XObject execute(XPathContext xctxt, int currentNode)
	  {

		// For now, the current node is already pushed.
		return execute(xctxt);
	  }

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="currentNode"> The currentNode. </param>
	  /// <param name="dtm"> The DTM of the current node. </param>
	  /// <param name="expType"> The expanded type ID of the current node.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(XPathContext xctxt, int currentNode, org.apache.xml.dtm.DTM dtm, int expType) throws javax.xml.transform.TransformerException
	  public virtual XObject execute(XPathContext xctxt, int currentNode, DTM dtm, int expType)
	  {

		// For now, the current node is already pushed.
		return execute(xctxt);
	  }

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.apache.xpath.objects.XObject execute(XPathContext xctxt) throws javax.xml.transform.TransformerException;
	  public abstract XObject execute(XPathContext xctxt);

	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression, but tell that a "safe" object doesn't have 
	  /// to be returned.  The default implementation just calls execute(xctxt).
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="destructiveOK"> true if a "safe" object doesn't need to be returned.
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(XPathContext xctxt, boolean destructiveOK) throws javax.xml.transform.TransformerException
	  public virtual XObject execute(XPathContext xctxt, bool destructiveOK)
	  {
		  return execute(xctxt);
	  }


	  /// <summary>
	  /// Evaluate expression to a number.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> The expression evaluated as a double.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double num(XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual double num(XPathContext xctxt)
	  {
		return execute(xctxt).num();
	  }

	  /// <summary>
	  /// Evaluate expression to a boolean.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> false
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean bool(XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual bool @bool(XPathContext xctxt)
	  {
		return execute(xctxt).@bool();
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> The string this wraps or the empty string if null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.utils.XMLString xstr(XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual XMLString xstr(XPathContext xctxt)
	  {
		return execute(xctxt).xstr();
	  }

	  /// <summary>
	  /// Tell if the expression is a nodeset expression.  In other words, tell
	  /// if you can execute <seealso cref="#asNode(XPathContext) asNode"/> without an exception. </summary>
	  /// <returns> true if the expression can be represented as a nodeset. </returns>
	  public virtual bool NodesetExpr
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Return the first node out of the nodeset, if this expression is
	  /// a nodeset expression. </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> the first node out of the nodeset, or DTM.NULL.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int asNode(XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual int asNode(XPathContext xctxt)
	  {
		  DTMIterator iter = execute(xctxt).iter();
		return iter.nextNode();
	  }

	  /// <summary>
	  /// Given an select expression and a context, evaluate the XPath
	  /// and return the resulting iterator.
	  /// </summary>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="contextNode"> The node that "." expresses.
	  /// 
	  /// </param>
	  /// <returns> A valid DTMIterator. </returns>
	  /// <exception cref="TransformerException"> thrown if the active ProblemListener decides
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage experimental </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator asIterator(XPathContext xctxt, int contextNode) throws javax.xml.transform.TransformerException
	  public virtual DTMIterator asIterator(XPathContext xctxt, int contextNode)
	  {

		try
		{
		  xctxt.pushCurrentNodeAndExpression(contextNode, contextNode);

		  return execute(xctxt).iter();
		}
		finally
		{
		  xctxt.popCurrentNodeAndExpression();
		}
	  }

	  /// <summary>
	  /// Given an select expression and a context, evaluate the XPath
	  /// and return the resulting iterator, but do not clone.
	  /// </summary>
	  /// <param name="xctxt"> The execution context. </param>
	  /// <param name="contextNode"> The node that "." expresses.
	  /// 
	  /// </param>
	  /// <returns> A valid DTMIterator. </returns>
	  /// <exception cref="TransformerException"> thrown if the active ProblemListener decides
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage experimental </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator asIteratorRaw(XPathContext xctxt, int contextNode) throws javax.xml.transform.TransformerException
	  public virtual DTMIterator asIteratorRaw(XPathContext xctxt, int contextNode)
	  {

		try
		{
		  xctxt.pushCurrentNodeAndExpression(contextNode, contextNode);

		  XNodeSet nodeset = (XNodeSet)execute(xctxt);
		  return nodeset.iterRaw();
		}
		finally
		{
		  xctxt.popCurrentNodeAndExpression();
		}
	  }


	  /// <summary>
	  /// Execute an expression in the XPath runtime context, and return the
	  /// result of the expression.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// NEEDSDOC <param name="handler">
	  /// </param>
	  /// <returns> The result of the expression in the form of a <code>XObject</code>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if a runtime exception
	  ///         occurs. </exception>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeCharsToContentHandler(XPathContext xctxt, org.xml.sax.ContentHandler handler) throws javax.xml.transform.TransformerException, org.xml.sax.SAXException
	  public virtual void executeCharsToContentHandler(XPathContext xctxt, ContentHandler handler)
	  {

		XObject obj = execute(xctxt);

		obj.dispatchCharactersEvents(handler);
		obj.detach();
	  }

	  /// <summary>
	  /// Tell if this expression returns a stable number that will not change during 
	  /// iterations within the expression.  This is used to determine if a proximity 
	  /// position predicate can indicate that no more searching has to occur.
	  /// 
	  /// </summary>
	  /// <returns> true if the expression represents a stable number. </returns>
	  public virtual bool StableNumber
	  {
		  get
		  {
			return false;
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
	  /// NEEDSDOC <param name="globalsSize"> </param>
	  public abstract void fixupVariables(ArrayList vars, int globalsSize);

	  /// <summary>
	  /// Compare this object with another object and see 
	  /// if they are equal, include the sub heararchy.
	  /// </summary>
	  /// <param name="expr"> Another expression object. </param>
	  /// <returns> true if this objects class and the expr
	  /// object's class are the same, and the data contained 
	  /// within both objects are considered equal. </returns>
	  public abstract bool deepEquals(Expression expr);

	  /// <summary>
	  /// This is a utility method to tell if the passed in 
	  /// class is the same class as this.  It is to be used by
	  /// the deepEquals method.  I'm bottlenecking it here 
	  /// because I'm not totally confident that comparing the 
	  /// class objects is the best way to do this. </summary>
	  /// <returns> true of the passed in class is the exact same 
	  /// class as this class. </returns>
	  protected internal bool isSameClass(Expression expr)
	  {
		  if (null == expr)
		  {
			return false;
		  }

		  return (this.GetType() == expr.GetType());
	  }

	  /// <summary>
	  /// Warn the user of an problem.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="msg"> An error msgkey that corresponds to one of the conststants found
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to
	  ///                              throw an exception.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warn(XPathContext xctxt, String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void warn(XPathContext xctxt, string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHWarning(msg, args);

		if (null != xctxt)
		{
		  ErrorListener eh = xctxt.ErrorListener;

		  // TO DO: Need to get stylesheet Locator from here.
		  eh.warning(new TransformerException(fmsg, xctxt.SAXLocator));
		}
	  }

	  /// <summary>
	  /// Tell the user of an assertion error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="b">  If false, a runtime exception will be thrown. </param>
	  /// <param name="msg"> The assertion message, which should be informative.
	  /// </param>
	  /// <exception cref="RuntimeException"> if the b argument is false.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
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
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to
	  ///                              throw an exception.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(XPathContext xctxt, String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void error(XPathContext xctxt, string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);

		if (null != xctxt)
		{
		  ErrorListener eh = xctxt.ErrorListener;
		  TransformerException te = new TransformerException(fmsg, this);

		  eh.fatalError(te);
		}
	  }

	  /// <summary>
	  /// Get the first non-Expression parent of this node. </summary>
	  /// <returns> null or first ancestor that is not an Expression. </returns>
	  public virtual ExpressionNode ExpressionOwner
	  {
		  get
		  {
			  ExpressionNode parent = exprGetParent();
			  while ((null != parent) && (parent is Expression))
			  {
				  parent = parent.exprGetParent();
			  }
			  return parent;
		  }
	  }

	  //=============== ExpressionNode methods ================

	  /// <summary>
	  /// This pair of methods are used to inform the node of its
	  ///  parent. 
	  /// </summary>
	  public virtual void exprSetParent(ExpressionNode n)
	  {
		  assertion(n != this, "Can not parent an expression to itself!");
		  m_parent = n;
	  }

	  public virtual ExpressionNode exprGetParent()
	  {
		  return m_parent;
	  }

	  /// <summary>
	  /// This method tells the node to add its argument to the node's
	  ///  list of children.  
	  /// </summary>
	  public virtual void exprAddChild(ExpressionNode n, int i)
	  {
		  assertion(false, "exprAddChild method not implemented!");
	  }

	  /// <summary>
	  /// This method returns a child node.  The children are numbered
	  ///   from zero, left to right. 
	  /// </summary>
	  public virtual ExpressionNode exprGetChild(int i)
	  {
		  return null;
	  }

	  /// <summary>
	  /// Return the number of children the node has. </summary>
	  public virtual int exprGetNumChildren()
	  {
		  return 0;
	  }

	  //=============== SourceLocator methods ================

	  /// <summary>
	  /// Return the public identifier for the current document event.
	  /// 
	  /// <para>The return value is the public identifier of the document
	  /// entity or of the external parsed entity in which the markup that
	  /// triggered the event appears.</para>
	  /// </summary>
	  /// <returns> A string containing the public identifier, or
	  ///         null if none is available. </returns>
	  /// <seealso cref= #getSystemId </seealso>
	  public virtual string PublicId
	  {
		  get
		  {
			  if (null == m_parent)
			  {
				return null;
			  }
			  return m_parent.PublicId;
		  }
	  }

	  /// <summary>
	  /// Return the system identifier for the current document event.
	  /// 
	  /// <para>The return value is the system identifier of the document
	  /// entity or of the external parsed entity in which the markup that
	  /// triggered the event appears.</para>
	  /// 
	  /// <para>If the system identifier is a URL, the parser must resolve it
	  /// fully before passing it to the application.</para>
	  /// </summary>
	  /// <returns> A string containing the system identifier, or null
	  ///         if none is available. </returns>
	  /// <seealso cref= #getPublicId </seealso>
	  public virtual string SystemId
	  {
		  get
		  {
			  if (null == m_parent)
			  {
				return null;
			  }
			  return m_parent.SystemId;
		  }
	  }

	  /// <summary>
	  /// Return the line number where the current document event ends.
	  /// 
	  /// <para><strong>Warning:</strong> The return value from the method
	  /// is intended only as an approximation for the sake of error
	  /// reporting; it is not intended to provide sufficient information
	  /// to edit the character content of the original XML document.</para>
	  /// 
	  /// <para>The return value is an approximation of the line number
	  /// in the document entity or external parsed entity where the
	  /// markup that triggered the event appears.</para>
	  /// </summary>
	  /// <returns> The line number, or -1 if none is available. </returns>
	  /// <seealso cref= #getColumnNumber </seealso>
	  public virtual int LineNumber
	  {
		  get
		  {
			  if (null == m_parent)
			  {
				return 0;
			  }
			  return m_parent.LineNumber;
		  }
	  }

	  /// <summary>
	  /// Return the character position where the current document event ends.
	  /// 
	  /// <para><strong>Warning:</strong> The return value from the method
	  /// is intended only as an approximation for the sake of error
	  /// reporting; it is not intended to provide sufficient information
	  /// to edit the character content of the original XML document.</para>
	  /// 
	  /// <para>The return value is an approximation of the column number
	  /// in the document entity or external parsed entity where the
	  /// markup that triggered the event appears.</para>
	  /// </summary>
	  /// <returns> The column number, or -1 if none is available. </returns>
	  /// <seealso cref= #getLineNumber </seealso>
	  public virtual int ColumnNumber
	  {
		  get
		  {
			  if (null == m_parent)
			  {
				return 0;
			  }
			  return m_parent.ColumnNumber;
		  }
	  }
	}

}