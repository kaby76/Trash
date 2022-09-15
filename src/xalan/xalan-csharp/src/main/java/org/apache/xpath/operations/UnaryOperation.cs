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
 * $Id: UnaryOperation.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.operations
{
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// The unary operation base class.
	/// </summary>
	[Serializable]
	public abstract class UnaryOperation : Expression, ExpressionOwner
	{
		internal new const long serialVersionUID = 6536083808424286166L;

	  /// <summary>
	  /// The operand for the operation.
	  ///  @serial 
	  /// </summary>
	  protected internal Expression m_right;

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
		m_right.fixupVariables(vars, globalsSize);
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public override bool canTraverseOutsideSubtree()
	  {

		if (null != m_right && m_right.canTraverseOutsideSubtree())
		{
		  return true;
		}

		return false;
	  }

	  /// <summary>
	  /// Set the expression operand for the operation.
	  /// 
	  /// </summary>
	  /// <param name="r"> The expression operand to which the unary operation will be 
	  ///          applied. </param>
	  public virtual Expression Right
	  {
		  set
		  {
			m_right = value;
			value.exprSetParent(this);
		  }
	  }

	  /// <summary>
	  /// Execute the operand and apply the unary operation to the result.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The runtime execution context.
	  /// </param>
	  /// <returns> An XObject that represents the result of applying the unary 
	  ///         operation to the evaluated operand.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		return operate(m_right.execute(xctxt));
	  }

	  /// <summary>
	  /// Apply the operation to two operands, and return the result.
	  /// 
	  /// </summary>
	  /// <param name="right"> non-null reference to the evaluated right operand.
	  /// </param>
	  /// <returns> non-null reference to the XObject that represents the result of the operation.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract org.apache.xpath.objects.XObject operate(org.apache.xpath.objects.XObject right) throws javax.xml.transform.TransformerException;
	  public abstract XObject operate(XObject right);

	  /// <returns> the operand of unary operation, as an Expression. </returns>
	  public virtual Expression Operand
	  {
		  get
		  {
			return m_right;
		  }
	  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  if (visitor.visitUnaryOperation(owner, this))
		  {
			  m_right.callVisitors(this, visitor);
		  }
	  }


	  /// <seealso cref="ExpressionOwner.getExpression()"/>
	  public virtual Expression Expression
	  {
		  get
		  {
			return m_right;
		  }
		  set
		  {
			  value.exprSetParent(this);
			  m_right = value;
		  }
	  }


	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  if (!m_right.deepEquals(((UnaryOperation)expr).m_right))
		  {
			  return false;
		  }

		  return true;
	  }


	}

}