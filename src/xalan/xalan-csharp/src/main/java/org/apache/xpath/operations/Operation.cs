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
 * $Id: Operation.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.operations
{

	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// The baseclass for a binary operation.
	/// </summary>
	[Serializable]
	public class Operation : Expression, ExpressionOwner
	{
		internal new const long serialVersionUID = -3037139537171050430L;

	  /// <summary>
	  /// The left operand expression.
	  ///  @serial 
	  /// </summary>
	  protected internal Expression m_left;

	  /// <summary>
	  /// The right operand expression.
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
		m_left.fixupVariables(vars, globalsSize);
		m_right.fixupVariables(vars, globalsSize);
	  }


	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public override bool canTraverseOutsideSubtree()
	  {

		if (null != m_left && m_left.canTraverseOutsideSubtree())
		{
		  return true;
		}

		if (null != m_right && m_right.canTraverseOutsideSubtree())
		{
		  return true;
		}

		return false;
	  }

	  /// <summary>
	  /// Set the left and right operand expressions for this operation.
	  /// 
	  /// </summary>
	  /// <param name="l"> The left expression operand. </param>
	  /// <param name="r"> The right expression operand. </param>
	  public virtual void setLeftRight(Expression l, Expression r)
	  {
		m_left = l;
		m_right = r;
		l.exprSetParent(this);
		r.exprSetParent(this);
	  }

	  /// <summary>
	  /// Execute a binary operation by calling execute on each of the operands,
	  /// and then calling the operate method on the derived class.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The runtime execution context.
	  /// </param>
	  /// <returns> The XObject result of the operation.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		XObject left = m_left.execute(xctxt, true);
		XObject right = m_right.execute(xctxt, true);

		XObject result = operate(left, right);
		left.detach();
		right.detach();
		return result;
	  }

	  /// <summary>
	  /// Apply the operation to two operands, and return the result.
	  /// 
	  /// </summary>
	  /// <param name="left"> non-null reference to the evaluated left operand. </param>
	  /// <param name="right"> non-null reference to the evaluated right operand.
	  /// </param>
	  /// <returns> non-null reference to the XObject that represents the result of the operation.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject operate(org.apache.xpath.objects.XObject left, org.apache.xpath.objects.XObject right) throws javax.xml.transform.TransformerException
	  public virtual XObject operate(XObject left, XObject right)
	  {
		return null; // no-op
	  }

	  /// <returns> the left operand of binary operation, as an Expression. </returns>
	  public virtual Expression LeftOperand
	  {
		  get
		  {
			return m_left;
		  }
	  }

	  /// <returns> the right operand of binary operation, as an Expression. </returns>
	  public virtual Expression RightOperand
	  {
		  get
		  {
			return m_right;
		  }
	  }

	  internal class LeftExprOwner : ExpressionOwner
	  {
		  private readonly Operation outerInstance;

		  public LeftExprOwner(Operation outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <seealso cref= ExpressionOwner#getExpression() </seealso>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_left;
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_left = value;
			}
		}

	  }

	  /// <seealso cref= org.apache.xpath.XPathVisitable#callVisitors(ExpressionOwner, XPathVisitor) </seealso>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  if (visitor.visitBinaryOperation(owner, this))
		  {
			  m_left.callVisitors(new LeftExprOwner(this), visitor);
			  m_right.callVisitors(this, visitor);
		  }
	  }

	  /// <seealso cref= ExpressionOwner#getExpression() </seealso>
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


	  /// <seealso cref= Expression#deepEquals(Expression) </seealso>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  if (!m_left.deepEquals(((Operation)expr).m_left))
		  {
			  return false;
		  }

		  if (!m_right.deepEquals(((Operation)expr).m_right))
		  {
			  return false;
		  }

		  return true;
	  }
	}

}