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
 * $Id: FunctionMultiArgs.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Base class for functions that accept an undetermined number of multiple
	/// arguments.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FunctionMultiArgs : Function3Args
	{
		internal new const long serialVersionUID = 7117257746138417181L;

	  /// <summary>
	  /// Argument expressions that are at index 3 or greater.
	  ///  @serial 
	  /// </summary>
	  internal Expression[] m_args;

	  /// <summary>
	  /// Return an expression array containing arguments at index 3 or greater.
	  /// </summary>
	  /// <returns> An array that contains the arguments at index 3 or greater. </returns>
	  public virtual Expression[] Args
	  {
		  get
		  {
			return m_args;
		  }
	  }

	  /// <summary>
	  /// Set an argument expression for a function.  This method is called by the
	  /// XPath compiler.
	  /// </summary>
	  /// <param name="arg"> non-null expression that represents the argument. </param>
	  /// <param name="argNum"> The argument number index.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> If a derived class determines that the
	  /// number of arguments is incorrect. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setArg(org.apache.xpath.Expression arg, int argNum) throws WrongNumberArgsException
	  public override void setArg(Expression arg, int argNum)
	  {

		if (argNum < 3)
		{
		  base.setArg(arg, argNum);
		}
		else
		{
		  if (null == m_args)
		  {
			m_args = new Expression[1];
			m_args[0] = arg;
		  }
		  else
		  {

			// Slow but space conservative.
			Expression[] args = new Expression[m_args.Length + 1];

			Array.Copy(m_args, 0, args, 0, m_args.Length);

			args[m_args.Length] = arg;
			m_args = args;
		  }
		  arg.exprSetParent(this);
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
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		base.fixupVariables(vars, globalsSize);
		if (null != m_args)
		{
		  for (int i = 0; i < m_args.Length; i++)
		  {
			m_args[i].fixupVariables(vars, globalsSize);
		  }
		}
	  }

	  /// <summary>
	  /// Check that the number of arguments passed to this function is correct.
	  /// 
	  /// </summary>
	  /// <param name="argNum"> The number of arguments that is being passed to the function.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void checkNumberArgs(int argNum) throws WrongNumberArgsException
	  public override void checkNumberArgs(int argNum)
	  {
	  }

	  /// <summary>
	  /// Constructs and throws a WrongNumberArgException with the appropriate
	  /// message for this function object.  This class supports an arbitrary
	  /// number of arguments, so this method must never be called.
	  /// </summary>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws WrongNumberArgsException
	  protected internal override void reportWrongNumberArgs()
	  {
		string fMsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_INCORRECT_PROGRAMMER_ASSERTION, new object[]{"Programmer's assertion:  the method FunctionMultiArgs.reportWrongNumberArgs() should never be called."});

		throw new Exception(fMsg);
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public override bool canTraverseOutsideSubtree()
	  {

		if (base.canTraverseOutsideSubtree())
		{
		  return true;
		}
		else
		{
		  int n = m_args.Length;

		  for (int i = 0; i < n; i++)
		  {
			if (m_args[i].canTraverseOutsideSubtree())
			{
			  return true;
			}
		  }

		  return false;
		}
	  }

	  internal class ArgMultiOwner : ExpressionOwner
	  {
		  private readonly FunctionMultiArgs outerInstance;

		  internal int m_argIndex;

		  internal ArgMultiOwner(FunctionMultiArgs outerInstance, int index)
		  {
			  this.outerInstance = outerInstance;
			  m_argIndex = index;
		  }

		/// <seealso cref="ExpressionOwner.getExpression()"/>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_args[m_argIndex];
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_args[m_argIndex] = value;
			}
		}


	  }


		/// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
		public override void callArgVisitors(XPathVisitor visitor)
		{
		  base.callArgVisitors(visitor);
		  if (null != m_args)
		  {
			int n = m_args.Length;
			for (int i = 0; i < n; i++)
			{
			  m_args[i].callVisitors(new ArgMultiOwner(this, i), visitor);
			}
		  }
		}

		/// <seealso cref="Expression.deepEquals(Expression)"/>
		public override bool deepEquals(Expression expr)
		{
		  if (!base.deepEquals(expr))
		  {
				return false;
		  }

		  FunctionMultiArgs fma = (FunctionMultiArgs) expr;
		  if (null != m_args)
		  {
			int n = m_args.Length;
			if ((null == fma) || (fma.m_args.Length != n))
			{
				  return false;
			}

			for (int i = 0; i < n; i++)
			{
			  if (!m_args[i].deepEquals(fma.m_args[i]))
			  {
					return false;
			  }
			}

		  }
		  else if (null != fma.m_args)
		  {
			  return false;
		  }

		  return true;
		}
	}

}