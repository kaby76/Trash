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
 * $Id: Function2Args.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathVisitor = org.apache.xpath.XPathVisitor;

	/// <summary>
	/// Base class for functions that accept two arguments.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class Function2Args : FunctionOneArg
	{
		internal new const long serialVersionUID = 5574294996842710641L;

	  /// <summary>
	  /// The second argument passed to the function (at index 1).
	  ///  @serial  
	  /// </summary>
	  internal Expression m_arg1;

	  /// <summary>
	  /// Return the second argument passed to the function (at index 1).
	  /// </summary>
	  /// <returns> An expression that represents the second argument passed to the 
	  ///         function. </returns>
	  public virtual Expression Arg1
	  {
		  get
		  {
			return m_arg1;
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
		if (null != m_arg1)
		{
		  m_arg1.fixupVariables(vars, globalsSize);
		}
	  }


	  /// <summary>
	  /// Set an argument expression for a function.  This method is called by the 
	  /// XPath compiler.
	  /// </summary>
	  /// <param name="arg"> non-null expression that represents the argument. </param>
	  /// <param name="argNum"> The argument number index.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> If the argNum parameter is greater than 1. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setArg(org.apache.xpath.Expression arg, int argNum) throws WrongNumberArgsException
	  public override void setArg(Expression arg, int argNum)
	  {

		// System.out.println("argNum: "+argNum);
		if (argNum == 0)
		{
		  base.setArg(arg, argNum);
		}
		else if (1 == argNum)
		{
		  m_arg1 = arg;
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
		if (argNum != 2)
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
		  throw new WrongNumberArgsException(XSLMessages.createXPATHMessage("two", null));
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside 
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	   public override bool canTraverseOutsideSubtree()
	   {
		return base.canTraverseOutsideSubtree() ? true : m_arg1.canTraverseOutsideSubtree();
	   }

	  internal class Arg1Owner : ExpressionOwner
	  {
		  private readonly Function2Args outerInstance;

		  public Arg1Owner(Function2Args outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <seealso cref="ExpressionOwner.getExpression()"/>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_arg1;
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_arg1 = value;
			}
		}


	  }


	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callArgVisitors(XPathVisitor visitor)
	  {
		  base.callArgVisitors(visitor);
		  if (null != m_arg1)
		  {
			  m_arg1.callVisitors(new Arg1Owner(this), visitor);
		  }
	  }

	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!base.deepEquals(expr))
		  {
			  return false;
		  }

		  if (null != m_arg1)
		  {
			  if (null == ((Function2Args)expr).m_arg1)
			  {
				  return false;
			  }

			  if (!m_arg1.deepEquals(((Function2Args)expr).m_arg1))
			  {
				  return false;
			  }
		  }
		  else if (null != ((Function2Args)expr).m_arg1)
		  {
			  return false;
		  }

		  return true;
	  }

	}

}