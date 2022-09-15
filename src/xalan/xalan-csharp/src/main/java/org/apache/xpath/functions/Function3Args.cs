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
 * $Id: Function3Args.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathVisitor = org.apache.xpath.XPathVisitor;

	/// <summary>
	/// Base class for functions that accept three arguments.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class Function3Args : Function2Args
	{
		internal new const long serialVersionUID = 7915240747161506646L;

	  /// <summary>
	  /// The third argument passed to the function (at index 2).
	  ///  @serial  
	  /// </summary>
	  internal Expression m_arg2;

	  /// <summary>
	  /// Return the third argument passed to the function (at index 2).
	  /// </summary>
	  /// <returns> An expression that represents the third argument passed to the 
	  ///         function. </returns>
	  public virtual Expression Arg2
	  {
		  get
		  {
			return m_arg2;
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
		if (null != m_arg2)
		{
		  m_arg2.fixupVariables(vars, globalsSize);
		}
	  }

	  /// <summary>
	  /// Set an argument expression for a function.  This method is called by the 
	  /// XPath compiler.
	  /// </summary>
	  /// <param name="arg"> non-null expression that represents the argument. </param>
	  /// <param name="argNum"> The argument number index.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> If the argNum parameter is greater than 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setArg(org.apache.xpath.Expression arg, int argNum) throws WrongNumberArgsException
	  public override void setArg(Expression arg, int argNum)
	  {

		if (argNum < 2)
		{
		  base.setArg(arg, argNum);
		}
		else if (2 == argNum)
		{
		  m_arg2 = arg;
		  arg.exprSetParent(this);
		}
		else
		{
			  reportWrongNumberArgs();
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
		if (argNum != 3)
		{
		  reportWrongNumberArgs();
		}
	  }

	  /// <summary>
	  /// Constructs and throws a WrongNumberArgException with the appropriate
	  /// message for this function object.
	  /// </summary>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws WrongNumberArgsException
	  protected internal override void reportWrongNumberArgs()
	  {
		  throw new WrongNumberArgsException(XSLMessages.createXPATHMessage("three", null));
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside 
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	   public override bool canTraverseOutsideSubtree()
	   {
		return base.canTraverseOutsideSubtree() ? true : m_arg2.canTraverseOutsideSubtree();
	   }

	  internal class Arg2Owner : ExpressionOwner
	  {
		  private readonly Function3Args outerInstance;

		  public Arg2Owner(Function3Args outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <seealso cref="ExpressionOwner.getExpression()"/>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_arg2;
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_arg2 = value;
			}
		}


	  }


	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callArgVisitors(XPathVisitor visitor)
	  {
		  base.callArgVisitors(visitor);
		  if (null != m_arg2)
		  {
			  m_arg2.callVisitors(new Arg2Owner(this), visitor);
		  }
	  }

	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!base.deepEquals(expr))
		  {
			  return false;
		  }

		  if (null != m_arg2)
		  {
			  if (null == ((Function3Args)expr).m_arg2)
			  {
				  return false;
			  }

			  if (!m_arg2.deepEquals(((Function3Args)expr).m_arg2))
			  {
				  return false;
			  }
		  }
		  else if (null != ((Function3Args)expr).m_arg2)
		  {
			  return false;
		  }

		  return true;
	  }


	}

}