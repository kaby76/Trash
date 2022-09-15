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
 * $Id: XUnresolvedVariable.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// An instance of this class holds unto a variable until 
	/// it is executed.  It is used at this time for global 
	/// variables which must (we think) forward reference.
	/// </summary>
	[Serializable]
	public class XUnresolvedVariable : XObject
	{
		internal new const long serialVersionUID = -256779804767950188L;
	  /// <summary>
	  /// The node context for execution. </summary>
	  [NonSerialized]
	  private int m_context;

	  /// <summary>
	  /// The transformer context for execution. </summary>
	  [NonSerialized]
	  private TransformerImpl m_transformer;

	  /// <summary>
	  /// An index to the point in the variable stack where we should
	  /// begin variable searches for evaluation of expressions.
	  /// This is -1 if m_isTopLevel is false. 
	  /// 
	  /// </summary>
	  [NonSerialized]
	  private int m_varStackPos = -1;

	  /// <summary>
	  /// An index into the variable stack where the variable context 
	  /// ends, i.e. at the point we should terminate the search. 
	  /// 
	  /// </summary>
	  [NonSerialized]
	  private int m_varStackContext;

	  /// <summary>
	  /// true if this variable or parameter is a global.
	  ///  @serial 
	  /// </summary>
	  private bool m_isGlobal;

	  /// <summary>
	  /// true if this variable or parameter is not currently being evaluated. </summary>
	  [NonSerialized]
	  private bool m_doneEval = true;

	  /// <summary>
	  /// Create an XUnresolvedVariable, that may be executed at a later time.
	  /// This is primarily used so that forward referencing works with 
	  /// global variables.  An XUnresolvedVariable is initially pushed 
	  /// into the global variable stack, and then replaced with the real 
	  /// thing when it is accessed.
	  /// </summary>
	  /// <param name="obj"> Must be a non-null reference to an ElemVariable. </param>
	  /// <param name="sourceNode"> The node context for execution. </param>
	  /// <param name="transformer"> The transformer execution context. </param>
	  /// <param name="varStackPos"> An index to the point in the variable stack where we should
	  /// begin variable searches for evaluation of expressions. </param>
	  /// <param name="varStackContext"> An index into the variable stack where the variable context 
	  /// ends, i.e. at the point we should terminate the search. </param>
	  /// <param name="isGlobal"> true if this is a global variable. </param>
	  public XUnresolvedVariable(ElemVariable obj, int sourceNode, TransformerImpl transformer, int varStackPos, int varStackContext, bool isGlobal) : base(obj)
	  {
		m_context = sourceNode;
		m_transformer = transformer;

		// For globals, this value will have to be updated once we 
		// have determined how many global variables have been pushed.
		m_varStackPos = varStackPos;

		// For globals, this should zero.
		m_varStackContext = varStackContext;

		m_isGlobal = isGlobal;
	  }

	  /// <summary>
	  /// For support of literal objects in xpaths.
	  /// </summary>
	  /// <param name="xctxt"> The XPath execution context.
	  /// </param>
	  /// <returns> This object.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		if (!m_doneEval)
		{
		  this.m_transformer.MsgMgr.error(xctxt.SAXLocator, XSLTErrorResources.ER_REFERENCING_ITSELF, new object[]{((ElemVariable)this.@object()).Name.LocalName});
		}
		VariableStack vars = xctxt.VarStack;

		// These three statements need to be combined into one operation.
		int currentFrame = vars.StackFrame;
		//// vars.setStackFrame(m_varStackPos);


		ElemVariable velem = (ElemVariable)m_obj;
		try
		{
		  m_doneEval = false;
		  if (-1 != velem.m_frameSize)
		  {
			  vars.link(velem.m_frameSize);
		  }
		  XObject @var = velem.getValue(m_transformer, m_context);
		  m_doneEval = true;
		  return @var;
		}
		finally
		{
		  // These two statements need to be combined into one operation.
		  // vars.setStackFrame(currentFrame);

		  if (-1 != velem.m_frameSize)
		  {
			  vars.unlink(currentFrame);
		  }
		}
	  }

	  /// <summary>
	  /// Set an index to the point in the variable stack where we should
	  /// begin variable searches for evaluation of expressions.
	  /// This is -1 if m_isTopLevel is false. 
	  /// </summary>
	  /// <param name="top"> A valid value that specifies where in the variable 
	  /// stack the search should begin. </param>
	  public virtual int VarStackPos
	  {
		  set
		  {
			m_varStackPos = value;
		  }
	  }

	  /// <summary>
	  /// Set an index into the variable stack where the variable context 
	  /// ends, i.e. at the point we should terminate the search.
	  /// </summary>
	  /// <param name="bottom"> The point at which the search should terminate, normally 
	  /// zero for global variables. </param>
	  public virtual int VarStackContext
	  {
		  set
		  {
			m_varStackContext = value;
		  }
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
			return "XUnresolvedVariable (" + @object().GetType().FullName + ")";
		  }
	  }


	}

}