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
 * $Id: Function.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This is a superclass of all XPath functions.  This allows two
	/// ways for the class to be called. One method is that the
	/// super class processes the arguments and hands the results to
	/// the derived class, the other method is that the derived
	/// class may process it's own arguments, which is faster since
	/// the arguments don't have to be added to an array, but causes
	/// a larger code footprint.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public abstract class Function : Expression
	{
		internal new const long serialVersionUID = 6927661240854599768L;

	  /// <summary>
	  /// Set an argument expression for a function.  This method is called by the 
	  /// XPath compiler.
	  /// </summary>
	  /// <param name="arg"> non-null expression that represents the argument. </param>
	  /// <param name="argNum"> The argument number index.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> If the argNum parameter is beyond what 
	  /// is specified for this function. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setArg(org.apache.xpath.Expression arg, int argNum) throws WrongNumberArgsException
	  public virtual void setArg(Expression arg, int argNum)
	  {
				// throw new WrongNumberArgsException(XSLMessages.createXPATHMessage("zero", null));
		  reportWrongNumberArgs();
	  }

	  /// <summary>
	  /// Check that the number of arguments passed to this function is correct.
	  /// This method is meant to be overloaded by derived classes, to check for 
	  /// the number of arguments for a specific function type.  This method is 
	  /// called by the compiler for static number of arguments checking.
	  /// </summary>
	  /// <param name="argNum"> The number of arguments that is being passed to the function.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkNumberArgs(int argNum) throws WrongNumberArgsException
	  public virtual void checkNumberArgs(int argNum)
	  {
		if (argNum != 0)
		{
		  reportWrongNumberArgs();
		}
	  }

	  /// <summary>
	  /// Constructs and throws a WrongNumberArgException with the appropriate
	  /// message for this function object.  This method is meant to be overloaded
	  /// by derived classes so that the message will be as specific as possible.
	  /// </summary>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws WrongNumberArgsException
	  protected internal virtual void reportWrongNumberArgs()
	  {
		  throw new WrongNumberArgsException(XSLMessages.createXPATHMessage("zero", null));
	  }

	  /// <summary>
	  /// Execute an XPath function object.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The execution current context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		// Programmer's assert.  (And, no, I don't want the method to be abstract).
		Console.WriteLine("Error! Function.execute should not be called!");

		return null;
	  }

	  /// <summary>
	  /// Call the visitors for the function arguments.
	  /// </summary>
	  public virtual void callArgVisitors(XPathVisitor visitor)
	  {
	  }


	  /// <seealso cref= org.apache.xpath.XPathVisitable#callVisitors(ExpressionOwner, XPathVisitor) </seealso>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  if (visitor.visitFunction(owner, this))
		  {
			  callArgVisitors(visitor);
		  }
	  }

	  /// <seealso cref= Expression#deepEquals(Expression) </seealso>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  return true;
	  }

	  /// <summary>
	  /// This function is currently only being used by Position()
	  /// and Last(). See respective functions for more detail.
	  /// </summary>
	  public virtual void postCompileStep(Compiler compiler)
	  {
		// no default action
	  }
	}

}