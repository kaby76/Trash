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
 * $Id: Variable.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.operations
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using QName = org.apache.xml.utils.QName;
	using PathComponent = org.apache.xpath.axes.PathComponent;
	using WalkerFactory = org.apache.xpath.axes.WalkerFactory;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;


	/// <summary>
	/// The variable reference expression executer.
	/// </summary>
	[Serializable]
	public class Variable : Expression, PathComponent
	{
		internal new const long serialVersionUID = -4334975375609297049L;
	  /// <summary>
	  /// Tell if fixupVariables was called.
	  ///  @serial   
	  /// </summary>
	  private bool m_fixUpWasCalled = false;

	  /// <summary>
	  /// The qualified name of the variable.
	  ///  @serial   
	  /// </summary>
	  protected internal QName m_qname;

	  /// <summary>
	  /// The index of the variable, which is either an absolute index to a 
	  /// global, or, if higher than the globals area, must be adjusted by adding 
	  /// the offset to the current stack frame.
	  /// </summary>
	  protected internal int m_index;

	  /// <summary>
	  /// Set the index for the variable into the stack.  For advanced use only. You 
	  /// must know what you are doing to use this.
	  /// </summary>
	  /// <param name="index"> a global or local index. </param>
	  public virtual int Index
	  {
		  set
		  {
			  m_index = value;
		  }
		  get
		  {
			  return m_index;
		  }
	  }


	  /// <summary>
	  /// Set whether or not this is a global reference.  For advanced use only.
	  /// </summary>
	  /// <param name="isGlobal"> true if this should be a global variable reference. </param>
	  public virtual bool IsGlobal
	  {
		  set
		  {
			  m_isGlobal = value;
		  }
	  }

	  /// <summary>
	  /// Set the index for the variable into the stack.  For advanced use only.
	  /// </summary>
	  /// <returns> true if this should be a global variable reference. </returns>
	  public virtual bool Global
	  {
		  get
		  {
			  return m_isGlobal;
		  }
	  }





	  protected internal bool m_isGlobal = false;

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame 
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		m_fixUpWasCalled = true;
		int sz = vars.Count;

		for (int i = vars.Count - 1; i >= 0; i--)
		{
		  QName qn = (QName)vars[i];
		  // System.out.println("qn: "+qn);
		  if (qn.Equals(m_qname))
		  {

			if (i < globalsSize)
			{
			  m_isGlobal = true;
			  m_index = i;
			}
			else
			{
			  m_index = i - globalsSize;
			}

			return;
		  }
		}

		string msg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_COULD_NOT_FIND_VAR, new object[]{m_qname.ToString()});

		TransformerException te = new TransformerException(msg, this);

		throw new org.apache.xml.utils.WrappedRuntimeException(te);

	  }


	  /// <summary>
	  /// Set the qualified name of the variable.
	  /// </summary>
	  /// <param name="qname"> Must be a non-null reference to a qualified name. </param>
	  public virtual QName QName
	  {
		  set
		  {
			m_qname = value;
		  }
		  get
		  {
			return m_qname;
		  }
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
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		  return execute(xctxt, false);
	  }


	  /// <summary>
	  /// Dereference the variable, and return the reference value.  Note that lazy 
	  /// evaluation will occur.  If a variable within scope is not found, a warning 
	  /// will be sent to the error listener, and an empty nodeset will be returned.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The runtime execution context.
	  /// </param>
	  /// <returns> The evaluated variable, or an empty nodeset if not found.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, boolean destructiveOK) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, bool destructiveOK)
	  {
		org.apache.xml.utils.PrefixResolver xprefixResolver = xctxt.NamespaceContext;

		XObject result;
		// Is the variable fetched always the same?
		// XObject result = xctxt.getVariable(m_qname);
		if (m_fixUpWasCalled)
		{
		  if (m_isGlobal)
		  {
			result = xctxt.VarStack.getGlobalVariable(xctxt, m_index, destructiveOK);
		  }
		  else
		  {
			result = xctxt.VarStack.getLocalVariable(xctxt, m_index, destructiveOK);
		  }
		}
		else
		{
			result = xctxt.VarStack.getVariableOrParam(xctxt,m_qname);
		}

		  if (null == result)
		  {
			// This should now never happen...
			warn(xctxt, XPATHErrorResources.WG_ILLEGAL_VARIABLE_REFERENCE, new object[]{m_qname.LocalPart}); //"VariableReference given for variable out "+
	  //      (new RuntimeException()).printStackTrace();
	  //      error(xctxt, XPATHErrorResources.ER_COULDNOT_GET_VAR_NAMED,
	  //            new Object[]{ m_qname.getLocalPart() });  //"Could not get variable named "+varName);

			result = new XNodeSet(xctxt.DTMManager);
		  }

		  return result;
	//    }
	//    else
	//    {
	//      // Hack city... big time.  This is needed to evaluate xpaths from extensions, 
	//      // pending some bright light going off in my head.  Some sort of callback?
	//      synchronized(this)
	//      {
	//      	org.apache.xalan.templates.ElemVariable vvar= getElemVariable();
	//      	if(null != vvar)
	//      	{
	//          m_index = vvar.getIndex();
	//          m_isGlobal = vvar.getIsTopLevel();
	//          m_fixUpWasCalled = true;
	//          return execute(xctxt);
	//      	}
	//      }
	//      throw new javax.xml.transform.TransformerException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_VAR_NOT_RESOLVABLE, new Object[]{m_qname.toString()})); //"Variable not resolvable: "+m_qname);
	//    }
	  }

	  /// <summary>
	  /// Get the XSLT ElemVariable that this sub-expression references.  In order for 
	  /// this to work, the SourceLocator must be the owning ElemTemplateElement. </summary>
	  /// <returns> The dereference to the ElemVariable, or null if not found. </returns>
	  public virtual org.apache.xalan.templates.ElemVariable ElemVariable
	  {
		  get
		  {
    
			// Get the current ElemTemplateElement, and then walk backwards in 
			// document order, searching 
			// for an xsl:param element or xsl:variable element that matches our 
			// qname.  If we reach the top level, use the StylesheetRoot's composed
			// list of top level variables and parameters.
    
			org.apache.xalan.templates.ElemVariable vvar = null;
			org.apache.xpath.ExpressionNode owner = ExpressionOwner;
    
			if (null != owner && owner is org.apache.xalan.templates.ElemTemplateElement)
			{
    
			  org.apache.xalan.templates.ElemTemplateElement prev = (org.apache.xalan.templates.ElemTemplateElement) owner;
    
			  if (!(prev is org.apache.xalan.templates.Stylesheet))
			  {
				while (prev != null && !(prev.ParentNode is org.apache.xalan.templates.Stylesheet))
				{
				  org.apache.xalan.templates.ElemTemplateElement savedprev = prev;
    
				  while (null != (prev = prev.PreviousSiblingElem))
				  {
					if (prev is org.apache.xalan.templates.ElemVariable)
					{
					  vvar = (org.apache.xalan.templates.ElemVariable) prev;
    
					  if (vvar.Name.Equals(m_qname))
					  {
						return vvar;
					  }
					  vvar = null;
					}
				  }
				  prev = savedprev.ParentElem;
				}
			  }
			  if (prev != null)
			  {
				vvar = prev.StylesheetRoot.getVariableOrParamComposed(m_qname);
			  }
			}
			return vvar;
    
		  }
	  }

	  /// <summary>
	  /// Tell if this expression returns a stable number that will not change during 
	  /// iterations within the expression.  This is used to determine if a proximity 
	  /// position predicate can indicate that no more searching has to occur.
	  /// 
	  /// </summary>
	  /// <returns> true if the expression represents a stable number. </returns>
	  public override bool StableNumber
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Get the analysis bits for this walker, as defined in the WalkerFactory. </summary>
	  /// <returns> One of WalkerFactory#BIT_DESCENDANT, etc. </returns>
	  public virtual int AnalysisBits
	  {
		  get
		  {
			  org.apache.xalan.templates.ElemVariable vvar = ElemVariable;
			  if (null != vvar)
			  {
				  XPath xpath = vvar.Select;
				  if (null != xpath)
				  {
					  Expression expr = xpath.Expression;
					  if (null != expr && expr is PathComponent)
					  {
						  return ((PathComponent)expr).AnalysisBits;
					  }
				  }
			  }
			return WalkerFactory.BIT_FILTER;
		  }
	  }


	  /// <seealso cref= org.apache.xpath.XPathVisitable#callVisitors(ExpressionOwner, XPathVisitor) </seealso>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  visitor.visitVariableRef(owner, this);
	  }
	  /// <seealso cref= Expression#deepEquals(Expression) </seealso>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  if (!m_qname.Equals(((Variable)expr).m_qname))
		  {
			  return false;
		  }

		  // We have to make sure that the qname really references 
		  // the same variable element.
		if (ElemVariable != ((Variable)expr).ElemVariable)
		{
			return false;
		}

		  return true;
	  }

	  internal const string PSUEDOVARNAMESPACE = "http://xml.apache.org/xalan/psuedovar";

	  /// <summary>
	  /// Tell if this is a psuedo variable reference, declared by Xalan instead 
	  /// of by the user.
	  /// </summary>
	  public virtual bool PsuedoVarRef
	  {
		  get
		  {
			  string ns = m_qname.NamespaceURI;
			  if ((null != ns) && ns.Equals(PSUEDOVARNAMESPACE))
			  {
				  if (m_qname.LocalName.StartsWith("#", StringComparison.Ordinal))
				  {
					  return true;
				  }
			  }
			  return false;
		  }
	  }


	}

}