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
 * $Id: FuncExtFunction.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Expression = org.apache.xpath.Expression;
	using ExpressionNode = org.apache.xpath.ExpressionNode;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using ExtensionsProvider = org.apache.xpath.ExtensionsProvider;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using XNull = org.apache.xpath.objects.XNull;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XPATHMessages = org.apache.xpath.res.XPATHMessages;

	/// <summary>
	/// An object of this class represents an extension call expression.  When
	/// the expression executes, it calls ExtensionsTable#extFunction, and then
	/// converts the result to the appropriate XObject.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncExtFunction : Function
	{
		internal new const long serialVersionUID = 5196115554693708718L;

	  /// <summary>
	  /// The namespace for the extension function, which should not normally
	  ///  be null or empty.
	  ///  @serial    
	  /// </summary>
	  internal string m_namespace;

	  /// <summary>
	  /// The local name of the extension.
	  ///  @serial   
	  /// </summary>
	  internal string m_extensionName;

	  /// <summary>
	  /// Unique method key, which is passed to ExtensionsTable#extFunction in
	  ///  order to allow caching of the method.
	  ///  @serial 
	  /// </summary>
	  internal object m_methodKey;

	  /// <summary>
	  /// Array of static expressions which represent the parameters to the
	  ///  function.
	  ///  @serial   
	  /// </summary>
	  internal ArrayList m_argVec = new ArrayList();

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
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {

		if (null != m_argVec)
		{
		  int nArgs = m_argVec.Count;

		  for (int i = 0; i < nArgs; i++)
		  {
			Expression arg = (Expression) m_argVec[i];

			arg.fixupVariables(vars, globalsSize);
		  }
		}
	  }

	  /// <summary>
	  /// Return the namespace of the extension function.
	  /// </summary>
	  /// <returns> The namespace of the extension function. </returns>
	  public virtual string Namespace
	  {
		  get
		  {
			return m_namespace;
		  }
	  }

	  /// <summary>
	  /// Return the name of the extension function.
	  /// </summary>
	  /// <returns> The name of the extension function. </returns>
	  public virtual string FunctionName
	  {
		  get
		  {
			return m_extensionName;
		  }
	  }

	  /// <summary>
	  /// Return the method key of the extension function.
	  /// </summary>
	  /// <returns> The method key of the extension function. </returns>
	  public virtual object MethodKey
	  {
		  get
		  {
			return m_methodKey;
		  }
	  }

	  /// <summary>
	  /// Return the nth argument passed to the extension function.
	  /// </summary>
	  /// <param name="n"> The argument number index. </param>
	  /// <returns> The Expression object at the given index. </returns>
	  public virtual Expression getArg(int n)
	  {
		if (n >= 0 && n < m_argVec.Count)
		{
		  return (Expression) m_argVec[n];
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Return the number of arguments that were passed
	  /// into this extension function.
	  /// </summary>
	  /// <returns> The number of arguments. </returns>
	  public virtual int ArgCount
	  {
		  get
		  {
			return m_argVec.Count;
		  }
	  }

	  /// <summary>
	  /// Create a new FuncExtFunction based on the qualified name of the extension,
	  /// and a unique method key.
	  /// </summary>
	  /// <param name="namespace"> The namespace for the extension function, which should
	  ///                  not normally be null or empty. </param>
	  /// <param name="extensionName"> The local name of the extension. </param>
	  /// <param name="methodKey"> Unique method key, which is passed to
	  ///                  ExtensionsTable#extFunction in order to allow caching
	  ///                  of the method. </param>
	  public FuncExtFunction(string @namespace, string extensionName, object methodKey)
	  {
		//try{throw new Exception("FuncExtFunction() " + namespace + " " + extensionName);} catch (Exception e){e.printStackTrace();}
		m_namespace = @namespace;
		m_extensionName = extensionName;
		m_methodKey = methodKey;
	  }

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		if (xctxt.SecureProcessing)
		{
		  throw new javax.xml.transform.TransformerException(XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, new object[] {ToString()}));
		}

		XObject result;
		ArrayList argVec = new ArrayList();
		int nArgs = m_argVec.Count;

		for (int i = 0; i < nArgs; i++)
		{
		  Expression arg = (Expression) m_argVec[i];

		  XObject xobj = arg.execute(xctxt);
		  /*
		   * Should cache the arguments for func:function
		   */
		  xobj.allowDetachToRelease(false);
		  argVec.Add(xobj);
		}
		//dml
		ExtensionsProvider extProvider = (ExtensionsProvider)xctxt.OwnerObject;
		object val = extProvider.extFunction(this, argVec);

		if (null != val)
		{
		  result = XObject.create(val, xctxt);
		}
		else
		{
		  result = new XNull();
		}

		return result;
	  }

	  /// <summary>
	  /// Set an argument expression for a function.  This method is called by the
	  /// XPath compiler.
	  /// </summary>
	  /// <param name="arg"> non-null expression that represents the argument. </param>
	  /// <param name="argNum"> The argument number index.
	  /// </param>
	  /// <exception cref="WrongNumberArgsException"> If the argNum parameter is beyond what
	  /// is specified for this function. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setArg(org.apache.xpath.Expression arg, int argNum) throws WrongNumberArgsException
	  public override void setArg(Expression arg, int argNum)
	  {
		m_argVec.Add(arg);
		arg.exprSetParent(this);
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


	  internal class ArgExtOwner : ExpressionOwner
	  {
		  private readonly FuncExtFunction outerInstance;


		internal Expression m_exp;

		  internal ArgExtOwner(FuncExtFunction outerInstance, Expression exp)
		  {
			  this.outerInstance = outerInstance;
			  m_exp = exp;
		  }

		/// <seealso cref="ExpressionOwner.getExpression()"/>
		public virtual Expression Expression
		{
			get
			{
			  return m_exp;
			}
			set
			{
				value.exprSetParent(outerInstance);
				m_exp = value;
			}
		}


	  }


	  /// <summary>
	  /// Call the visitors for the function arguments.
	  /// </summary>
	  public override void callArgVisitors(XPathVisitor visitor)
	  {
		  for (int i = 0; i < m_argVec.Count; i++)
		  {
			 Expression exp = (Expression)m_argVec[i];
			 exp.callVisitors(new ArgExtOwner(this, exp), visitor);
		  }

	  }

	  /// <summary>
	  /// Set the parent node.
	  /// For an extension function, we also need to set the parent
	  /// node for all argument expressions.
	  /// </summary>
	  /// <param name="n"> The parent node </param>
	  public override void exprSetParent(ExpressionNode n)
	  {

		base.exprSetParent(n);

		int nArgs = m_argVec.Count;

		for (int i = 0; i < nArgs; i++)
		{
		  Expression arg = (Expression) m_argVec[i];

		  arg.exprSetParent(n);
		}
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
	  /// Return the name of the extesion function in string format
	  /// </summary>
	  public override string ToString()
	  {
		if (!string.ReferenceEquals(m_namespace, null) && m_namespace.Length > 0)
		{
		  return "{" + m_namespace + "}" + m_extensionName;
		}
		else
		{
		  return m_extensionName;
		}
	  }
	}

}