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
 * $Id: Compiler.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.compiler
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Axis = org.apache.xml.dtm.Axis;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using QName = org.apache.xml.utils.QName;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using Expression = org.apache.xpath.Expression;
	using UnionPathIterator = org.apache.xpath.axes.UnionPathIterator;
	using WalkerFactory = org.apache.xpath.axes.WalkerFactory;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using FuncExtFunctionAvailable = org.apache.xpath.functions.FuncExtFunctionAvailable;
	using Function = org.apache.xpath.functions.Function;
	using WrongNumberArgsException = org.apache.xpath.functions.WrongNumberArgsException;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XString = org.apache.xpath.objects.XString;
	using And = org.apache.xpath.operations.And;
	using Div = org.apache.xpath.operations.Div;
	using Equals = org.apache.xpath.operations.Equals;
	using Gt = org.apache.xpath.operations.Gt;
	using Gte = org.apache.xpath.operations.Gte;
	using Lt = org.apache.xpath.operations.Lt;
	using Lte = org.apache.xpath.operations.Lte;
	using Minus = org.apache.xpath.operations.Minus;
	using Mod = org.apache.xpath.operations.Mod;
	using Mult = org.apache.xpath.operations.Mult;
	using Neg = org.apache.xpath.operations.Neg;
	using NotEquals = org.apache.xpath.operations.NotEquals;
	using Operation = org.apache.xpath.operations.Operation;
	using Or = org.apache.xpath.operations.Or;
	using Plus = org.apache.xpath.operations.Plus;
	using UnaryOperation = org.apache.xpath.operations.UnaryOperation;
	using Variable = org.apache.xpath.operations.Variable;
	using FunctionPattern = org.apache.xpath.patterns.FunctionPattern;
	using NodeTest = org.apache.xpath.patterns.NodeTest;
	using StepPattern = org.apache.xpath.patterns.StepPattern;
	using UnionPattern = org.apache.xpath.patterns.UnionPattern;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// An instance of this class compiles an XPath string expression into 
	/// a Expression object.  This class compiles the string into a sequence 
	/// of operation codes (op map) and then builds from that into an Expression 
	/// tree.
	/// @xsl.usage advanced
	/// </summary>
	public class Compiler : OpMap
	{

	  /// <summary>
	  /// Construct a Compiler object with a specific ErrorListener and 
	  /// SourceLocator where the expression is located.
	  /// </summary>
	  /// <param name="errorHandler"> Error listener where messages will be sent, or null 
	  ///                     if messages should be sent to System err. </param>
	  /// <param name="locator"> The location object where the expression lives, which 
	  ///                may be null, but which, if not null, must be valid over 
	  ///                the long haul, in other words, it will not be cloned. </param>
	  /// <param name="fTable">  The FunctionTable object where the xpath build-in 
	  ///                functions are stored. </param>
	  public Compiler(ErrorListener errorHandler, SourceLocator locator, FunctionTable fTable)
	  {
		m_errorHandler = errorHandler;
		m_locator = locator;
		m_functionTable = fTable;
	  }

	  /// <summary>
	  /// Construct a Compiler instance that has a null error listener and a 
	  /// null source locator.
	  /// </summary>
	  public Compiler()
	  {
		m_errorHandler = null;
		m_locator = null;
	  }

	  /// <summary>
	  /// Execute the XPath object from a given opcode position. </summary>
	  /// <param name="opPos"> The current position in the xpath.m_opMap array. </param>
	  /// <returns> The result of the XPath.
	  /// </returns>
	  /// <exception cref="TransformerException"> if there is a syntax or other error.
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.Expression compile(int opPos) throws javax.xml.transform.TransformerException
	  public virtual Expression compile(int opPos)
	  {

		int op = getOp(opPos);

		Expression expr = null;
		// System.out.println(getPatternString()+"op: "+op);
		switch (op)
		{
		case OpCodes.OP_XPATH :
		  expr = compile(opPos + 2);
		  break;
		case OpCodes.OP_OR :
		  expr = or(opPos);
		  break;
		case OpCodes.OP_AND :
		  expr = and(opPos);
		  break;
		case OpCodes.OP_NOTEQUALS :
		  expr = notequals(opPos);
		  break;
		case OpCodes.OP_EQUALS :
		  expr = Equals(opPos);
		  break;
		case OpCodes.OP_LTE :
		  expr = lte(opPos);
		  break;
		case OpCodes.OP_LT :
		  expr = lt(opPos);
		  break;
		case OpCodes.OP_GTE :
		  expr = gte(opPos);
		  break;
		case OpCodes.OP_GT :
		  expr = gt(opPos);
		  break;
		case OpCodes.OP_PLUS :
		  expr = plus(opPos);
		  break;
		case OpCodes.OP_MINUS :
		  expr = minus(opPos);
		  break;
		case OpCodes.OP_MULT :
		  expr = mult(opPos);
		  break;
		case OpCodes.OP_DIV :
		  expr = div(opPos);
		  break;
		case OpCodes.OP_MOD :
		  expr = mod(opPos);
		  break;
	//    case OpCodes.OP_QUO :
	//      expr = quo(opPos); break;
		case OpCodes.OP_NEG :
		  expr = neg(opPos);
		  break;
		case OpCodes.OP_STRING :
		  expr = @string(opPos);
		  break;
		case OpCodes.OP_BOOL :
		  expr = @bool(opPos);
		  break;
		case OpCodes.OP_NUMBER :
		  expr = number(opPos);
		  break;
		case OpCodes.OP_UNION :
		  expr = union(opPos);
		  break;
		case OpCodes.OP_LITERAL :
		  expr = literal(opPos);
		  break;
		case OpCodes.OP_VARIABLE :
		  expr = variable(opPos);
		  break;
		case OpCodes.OP_GROUP :
		  expr = group(opPos);
		  break;
		case OpCodes.OP_NUMBERLIT :
		  expr = numberlit(opPos);
		  break;
		case OpCodes.OP_ARGUMENT :
		  expr = arg(opPos);
		  break;
		case OpCodes.OP_EXTFUNCTION :
		  expr = compileExtension(opPos);
		  break;
		case OpCodes.OP_FUNCTION :
		  expr = compileFunction(opPos);
		  break;
		case OpCodes.OP_LOCATIONPATH :
		  expr = locationPath(opPos);
		  break;
		case OpCodes.OP_PREDICATE :
		  expr = null;
		  break; // should never hit this here.
		case OpCodes.OP_MATCHPATTERN :
		  expr = matchPattern(opPos + 2);
		  break;
		case OpCodes.OP_LOCATIONPATHPATTERN :
		  expr = locationPathPattern(opPos);
		  break;
		case OpCodes.OP_QUO:
		  error(XPATHErrorResources.ER_UNKNOWN_OPCODE, new object[]{"quo"}); //"ERROR! Unknown op code: "+m_opMap[opPos]);
		  break;
		default :
		  error(XPATHErrorResources.ER_UNKNOWN_OPCODE, new object[]{Convert.ToString(getOp(opPos))}); //"ERROR! Unknown op code: "+m_opMap[opPos]);
	  break;
		}
	//    if(null != expr)
	//      expr.setSourceLocator(m_locator);

		return expr;
	  }

	  /// <summary>
	  /// Bottle-neck compilation of an operation with left and right operands.
	  /// </summary>
	  /// <param name="operation"> non-null reference to parent operation. </param>
	  /// <param name="opPos"> The op map position of the parent operation.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Operation"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if there is a syntax or other error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.apache.xpath.Expression compileOperation(org.apache.xpath.operations.Operation operation, int opPos) throws javax.xml.transform.TransformerException
	  private Expression compileOperation(Operation operation, int opPos)
	  {

		int leftPos = getFirstChildPos(opPos);
		int rightPos = getNextOpPos(leftPos);

		operation.setLeftRight(compile(leftPos), compile(rightPos));

		return operation;
	  }

	  /// <summary>
	  /// Bottle-neck compilation of a unary operation.
	  /// </summary>
	  /// <param name="unary"> The parent unary operation. </param>
	  /// <param name="opPos"> The position in the op map of the parent operation.
	  /// </param>
	  /// <returns> The unary argument.
	  /// </returns>
	  /// <exception cref="TransformerException"> if syntax or other error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.apache.xpath.Expression compileUnary(org.apache.xpath.operations.UnaryOperation unary, int opPos) throws javax.xml.transform.TransformerException
	  private Expression compileUnary(UnaryOperation unary, int opPos)
	  {

		int rightPos = getFirstChildPos(opPos);

		unary.Right = compile(rightPos);

		return unary;
	  }

	  /// <summary>
	  /// Compile an 'or' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Or"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression or(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression or(int opPos)
	  {
		return compileOperation(new Or(), opPos);
	  }

	  /// <summary>
	  /// Compile an 'and' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.And"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression and(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression and(int opPos)
	  {
		return compileOperation(new And(), opPos);
	  }

	  /// <summary>
	  /// Compile a '!=' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.NotEquals"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression notequals(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression notequals(int opPos)
	  {
		return compileOperation(new NotEquals(), opPos);
	  }

	  /// <summary>
	  /// Compile a '=' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Equals"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression equals(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression Equals(int opPos)
	  {
		return compileOperation(new Equals(), opPos);
	  }

	  /// <summary>
	  /// Compile a '<=' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Lte"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression lte(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression lte(int opPos)
	  {
		return compileOperation(new Lte(), opPos);
	  }

	  /// <summary>
	  /// Compile a '<' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Lt"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression lt(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression lt(int opPos)
	  {
		return compileOperation(new Lt(), opPos);
	  }

	  /// <summary>
	  /// Compile a '>=' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Gte"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression gte(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression gte(int opPos)
	  {
		return compileOperation(new Gte(), opPos);
	  }

	  /// <summary>
	  /// Compile a '>' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Gt"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression gt(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression gt(int opPos)
	  {
		return compileOperation(new Gt(), opPos);
	  }

	  /// <summary>
	  /// Compile a '+' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Plus"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression plus(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression plus(int opPos)
	  {
		return compileOperation(new Plus(), opPos);
	  }

	  /// <summary>
	  /// Compile a '-' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Minus"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression minus(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression minus(int opPos)
	  {
		return compileOperation(new Minus(), opPos);
	  }

	  /// <summary>
	  /// Compile a '*' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Mult"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression mult(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression mult(int opPos)
	  {
		return compileOperation(new Mult(), opPos);
	  }

	  /// <summary>
	  /// Compile a 'div' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Div"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression div(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression div(int opPos)
	  {
		return compileOperation(new Div(), opPos);
	  }

	  /// <summary>
	  /// Compile a 'mod' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Mod"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression mod(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression mod(int opPos)
	  {
		return compileOperation(new Mod(), opPos);
	  }

	  /*
	   * Compile a 'quo' operation.
	   * 
	   * @param opPos The current position in the m_opMap array.
	   *
	   * @return reference to {@link org.apache.xpath.operations.Quo} instance.
	   *
	   * @throws TransformerException if a error occurs creating the Expression.
	   */
	//  protected Expression quo(int opPos) throws TransformerException
	//  {
	//    return compileOperation(new Quo(), opPos);
	//  }

	  /// <summary>
	  /// Compile a unary '-' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Neg"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression neg(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression neg(int opPos)
	  {
		return compileUnary(new Neg(), opPos);
	  }

	  /// <summary>
	  /// Compile a 'string(...)' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.String"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression string(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression @string(int opPos)
	  {
		return compileUnary(new org.apache.xpath.operations.String(), opPos);
	  }

	  /// <summary>
	  /// Compile a 'boolean(...)' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Bool"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression bool(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression @bool(int opPos)
	  {
		return compileUnary(new org.apache.xpath.operations.Bool(), opPos);
	  }

	  /// <summary>
	  /// Compile a 'number(...)' operation.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Number"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression number(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression number(int opPos)
	  {
		return compileUnary(new org.apache.xpath.operations.Number(), opPos);
	  }

	  /// <summary>
	  /// Compile a literal string value.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.objects.XString"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
	  protected internal virtual Expression literal(int opPos)
	  {

		opPos = getFirstChildPos(opPos);

		return (XString) TokenQueue.elementAt(getOp(opPos));
	  }

	  /// <summary>
	  /// Compile a literal number value.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.objects.XNumber"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
	  protected internal virtual Expression numberlit(int opPos)
	  {

		opPos = getFirstChildPos(opPos);

		return (XNumber) TokenQueue.elementAt(getOp(opPos));
	  }

	  /// <summary>
	  /// Compile a variable reference.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.operations.Variable"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression variable(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression variable(int opPos)
	  {

		Variable var = new Variable();

		opPos = getFirstChildPos(opPos);

		int nsPos = getOp(opPos);
		string @namespace = (OpCodes.EMPTY == nsPos) ? null : (string) TokenQueue.elementAt(nsPos);
		string localname = (string) TokenQueue.elementAt(getOp(opPos + 1));
		QName qname = new QName(@namespace, localname);

		var.QName = qname;

		return var;
	  }

	  /// <summary>
	  /// Compile an expression group.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to the contained expression.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression group(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression group(int opPos)
	  {

		// no-op
		return compile(opPos + 2);
	  }

	  /// <summary>
	  /// Compile a function argument.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to the argument expression.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression arg(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression arg(int opPos)
	  {

		// no-op
		return compile(opPos + 2);
	  }

	  /// <summary>
	  /// Compile a location path union. The UnionPathIterator itself may create
	  /// <seealso cref="org.apache.xpath.axes.LocPathIterator"/> children.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.axes.LocPathIterator"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression union(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression union(int opPos)
	  {
		locPathDepth++;
		try
		{
		  return UnionPathIterator.createUnionIterator(this, opPos);
		}
		finally
		{
		  locPathDepth--;
		}
	  }

	  private int locPathDepth = -1;

	  /// <summary>
	  /// Get the level of the location path or union being constructed. </summary>
	  /// <returns> 0 if it is a top-level path. </returns>
	  public virtual int LocationPathDepth
	  {
		  get
		  {
			return locPathDepth;
		  }
	  }

	  /// <summary>
	  /// Get the function table  
	  /// </summary>
	  internal virtual FunctionTable FunctionTable
	  {
		  get
		  {
			return m_functionTable;
		  }
	  }

	  /// <summary>
	  /// Compile a location path.  The LocPathIterator itself may create
	  /// <seealso cref="org.apache.xpath.axes.AxesWalker"/> children.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.axes.LocPathIterator"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.Expression locationPath(int opPos) throws javax.xml.transform.TransformerException
	  public virtual Expression locationPath(int opPos)
	  {
		locPathDepth++;
		try
		{
		  DTMIterator iter = WalkerFactory.newDTMIterator(this, opPos, (locPathDepth == 0));
		  return (Expression)iter; // cast OK, I guess.
		}
		finally
		{
		  locPathDepth--;
		}
	  }

	  /// <summary>
	  /// Compile a location step predicate expression.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> the contained predicate expression.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.Expression predicate(int opPos) throws javax.xml.transform.TransformerException
	  public virtual Expression predicate(int opPos)
	  {
		return compile(opPos + 2);
	  }

	  /// <summary>
	  /// Compile an entire match pattern expression.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.patterns.UnionPattern"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.Expression matchPattern(int opPos) throws javax.xml.transform.TransformerException
	  protected internal virtual Expression matchPattern(int opPos)
	  {
		locPathDepth++;
		try
		{
		  // First, count...
		  int nextOpPos = opPos;
		  int i;

		  for (i = 0; getOp(nextOpPos) == OpCodes.OP_LOCATIONPATHPATTERN; i++)
		  {
			nextOpPos = getNextOpPos(nextOpPos);
		  }

		  if (i == 1)
		  {
			return compile(opPos);
		  }

		  UnionPattern up = new UnionPattern();
		  StepPattern[] patterns = new StepPattern[i];

		  for (i = 0; getOp(opPos) == OpCodes.OP_LOCATIONPATHPATTERN; i++)
		  {
			nextOpPos = getNextOpPos(opPos);
			patterns[i] = (StepPattern) compile(opPos);
			opPos = nextOpPos;
		  }

		  up.Patterns = patterns;

		  return up;
		}
		finally
		{
		  locPathDepth--;
		}
	  }

	  /// <summary>
	  /// Compile a location match pattern unit expression.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.patterns.StepPattern"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.Expression locationPathPattern(int opPos) throws javax.xml.transform.TransformerException
	  public virtual Expression locationPathPattern(int opPos)
	  {

		opPos = getFirstChildPos(opPos);

		return stepPattern(opPos, 0, null);
	  }

	  /// <summary>
	  /// Get a <seealso cref="org.w3c.dom.traversal.NodeFilter"/> bit set that tells what 
	  /// to show for a given node test.
	  /// </summary>
	  /// <param name="opPos"> the op map position for the location step.
	  /// </param>
	  /// <returns> <seealso cref="org.w3c.dom.traversal.NodeFilter"/> bit set that tells what 
	  ///         to show for a given node test. </returns>
	  public virtual int getWhatToShow(int opPos)
	  {

		int axesType = getOp(opPos);
		int testType = getOp(opPos + 3);

		// System.out.println("testType: "+testType);
		switch (testType)
		{
		case OpCodes.NODETYPE_COMMENT :
		  return DTMFilter.SHOW_COMMENT;
		case OpCodes.NODETYPE_TEXT :
	//      return DTMFilter.SHOW_TEXT | DTMFilter.SHOW_COMMENT;
		  return DTMFilter.SHOW_TEXT | DTMFilter.SHOW_CDATA_SECTION;
		case OpCodes.NODETYPE_PI :
		  return DTMFilter.SHOW_PROCESSING_INSTRUCTION;
		case OpCodes.NODETYPE_NODE :
	//      return DTMFilter.SHOW_ALL;
		  switch (axesType)
		  {
		  case OpCodes.FROM_NAMESPACE:
			return DTMFilter.SHOW_NAMESPACE;
		  case OpCodes.FROM_ATTRIBUTES :
		  case OpCodes.MATCH_ATTRIBUTE :
			return DTMFilter.SHOW_ATTRIBUTE;
		  case OpCodes.FROM_SELF:
		  case OpCodes.FROM_ANCESTORS_OR_SELF:
		  case OpCodes.FROM_DESCENDANTS_OR_SELF:
			return DTMFilter.SHOW_ALL;
		  default:
			if (getOp(0) == OpCodes.OP_MATCHPATTERN)
			{
			  return ~DTMFilter.SHOW_ATTRIBUTE & ~DTMFilter.SHOW_DOCUMENT & ~DTMFilter.SHOW_DOCUMENT_FRAGMENT;
			}
			else
			{
			  return ~DTMFilter.SHOW_ATTRIBUTE;
			}
		  }
			goto case OpCodes.NODETYPE_ROOT;
		case OpCodes.NODETYPE_ROOT :
		  return DTMFilter.SHOW_DOCUMENT | DTMFilter.SHOW_DOCUMENT_FRAGMENT;
		case OpCodes.NODETYPE_FUNCTEST :
		  return NodeTest.SHOW_BYFUNCTION;
		case OpCodes.NODENAME :
		  switch (axesType)
		  {
		  case OpCodes.FROM_NAMESPACE :
			return DTMFilter.SHOW_NAMESPACE;
		  case OpCodes.FROM_ATTRIBUTES :
		  case OpCodes.MATCH_ATTRIBUTE :
			return DTMFilter.SHOW_ATTRIBUTE;

		  // break;
		  case OpCodes.MATCH_ANY_ANCESTOR :
		  case OpCodes.MATCH_IMMEDIATE_ANCESTOR :
			return DTMFilter.SHOW_ELEMENT;

		  // break;
		  default :
			return DTMFilter.SHOW_ELEMENT;
		  }
		default :
		  // System.err.println("We should never reach here.");
		  return DTMFilter.SHOW_ALL;
		}
	  }

	private const bool DEBUG = false;

	  /// <summary>
	  /// Compile a step pattern unit expression, used for both location paths 
	  /// and match patterns.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array. </param>
	  /// <param name="stepCount"> The number of steps to expect. </param>
	  /// <param name="ancestorPattern"> The owning StepPattern, which may be null.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.patterns.StepPattern"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xpath.patterns.StepPattern stepPattern(int opPos, int stepCount, org.apache.xpath.patterns.StepPattern ancestorPattern) throws javax.xml.transform.TransformerException
	  protected internal virtual StepPattern stepPattern(int opPos, int stepCount, StepPattern ancestorPattern)
	  {

		int startOpPos = opPos;
		int stepType = getOp(opPos);

		if (OpCodes.ENDOP == stepType)
		{
		  return null;
		}

		bool addMagicSelf = true;

		int endStep = getNextOpPos(opPos);

		// int nextStepType = getOpMap()[endStep];
		StepPattern pattern;

		// boolean isSimple = ((OpCodes.ENDOP == nextStepType) && (stepCount == 0));
		int argLen;

		switch (stepType)
		{
		case OpCodes.OP_FUNCTION :
		  if (DEBUG)
		  {
			Console.WriteLine("MATCH_FUNCTION: " + m_currentPattern);
		  }
		  addMagicSelf = false;
		  argLen = getOp(opPos + OpMap.MAPINDEX_LENGTH);
		  pattern = new FunctionPattern(compileFunction(opPos), Axis.PARENT, Axis.CHILD);
		  break;
		case OpCodes.FROM_ROOT :
		  if (DEBUG)
		  {
			Console.WriteLine("FROM_ROOT, " + m_currentPattern);
		  }
		  addMagicSelf = false;
		  argLen = getArgLengthOfStep(opPos);
		  opPos = getFirstChildPosOfStep(opPos);
		  pattern = new StepPattern(DTMFilter.SHOW_DOCUMENT | DTMFilter.SHOW_DOCUMENT_FRAGMENT, Axis.PARENT, Axis.CHILD);
		  break;
		case OpCodes.MATCH_ATTRIBUTE :
		 if (DEBUG)
		 {
			Console.WriteLine("MATCH_ATTRIBUTE: " + getStepLocalName(startOpPos) + ", " + m_currentPattern);
		 }
		  argLen = getArgLengthOfStep(opPos);
		  opPos = getFirstChildPosOfStep(opPos);
		  pattern = new StepPattern(DTMFilter.SHOW_ATTRIBUTE, getStepNS(startOpPos), getStepLocalName(startOpPos), Axis.PARENT, Axis.ATTRIBUTE);
		  break;
		case OpCodes.MATCH_ANY_ANCESTOR :
		  if (DEBUG)
		  {
			Console.WriteLine("MATCH_ANY_ANCESTOR: " + getStepLocalName(startOpPos) + ", " + m_currentPattern);
		  }
		  argLen = getArgLengthOfStep(opPos);
		  opPos = getFirstChildPosOfStep(opPos);
		  int what = getWhatToShow(startOpPos);
		  // bit-o-hackery, but this code is due for the morgue anyway...
		  if (0x00000500 == what)
		  {
			addMagicSelf = false;
		  }
		  pattern = new StepPattern(getWhatToShow(startOpPos), getStepNS(startOpPos), getStepLocalName(startOpPos), Axis.ANCESTOR, Axis.CHILD);
		  break;
		case OpCodes.MATCH_IMMEDIATE_ANCESTOR :
		  if (DEBUG)
		  {
			Console.WriteLine("MATCH_IMMEDIATE_ANCESTOR: " + getStepLocalName(startOpPos) + ", " + m_currentPattern);
		  }
		  argLen = getArgLengthOfStep(opPos);
		  opPos = getFirstChildPosOfStep(opPos);
		  pattern = new StepPattern(getWhatToShow(startOpPos), getStepNS(startOpPos), getStepLocalName(startOpPos), Axis.PARENT, Axis.CHILD);
		  break;
		default :
		  error(XPATHErrorResources.ER_UNKNOWN_MATCH_OPERATION, null); //"unknown match operation!");

		  return null;
		}

		pattern.Predicates = getCompiledPredicates(opPos + argLen);
		if (null == ancestorPattern)
		{
		  // This is the magic and invisible "." at the head of every 
		  // match pattern, and corresponds to the current node in the context 
		  // list, from where predicates are counted.
		  // So, in order to calculate "foo[3]", it has to count from the 
		  // current node in the context list, so, from that current node, 
		  // the full pattern is really "self::node()/child::foo[3]".  If you 
		  // translate this to a select pattern from the node being tested, 
		  // which is really how we're treating match patterns, it works out to 
		  // self::foo/parent::node[child::foo[3]]", or close enough.
		/*      if(addMagicSelf && pattern.getPredicateCount() > 0)
		  {
			StepPattern selfPattern = new StepPattern(DTMFilter.SHOW_ALL, 
													  Axis.PARENT, Axis.CHILD);
			// We need to keep the new nodetest from affecting the score...
			XNumber score = pattern.getStaticScore();
			pattern.setRelativePathPattern(selfPattern);
			pattern.setStaticScore(score);
			selfPattern.setStaticScore(score);
		}*/
		}
		else
		{
		  // System.out.println("Setting "+ancestorPattern+" as relative to "+pattern);
		  pattern.RelativePathPattern = ancestorPattern;
		}

		StepPattern relativePathPattern = stepPattern(endStep, stepCount + 1, pattern);

		return (null != relativePathPattern) ? relativePathPattern : pattern;
	  }

	  /// <summary>
	  /// Compile a zero or more predicates for a given match pattern.
	  /// </summary>
	  /// <param name="opPos"> The position of the first predicate the m_opMap array.
	  /// </param>
	  /// <returns> reference to array of <seealso cref="org.apache.xpath.Expression"/> instances.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.Expression[] getCompiledPredicates(int opPos) throws javax.xml.transform.TransformerException
	  public virtual Expression[] getCompiledPredicates(int opPos)
	  {

		int count = countPredicates(opPos);

		if (count > 0)
		{
		  Expression[] predicates = new Expression[count];

		  compilePredicates(opPos, predicates);

		  return predicates;
		}

		return null;
	  }

	  /// <summary>
	  /// Count the number of predicates in the step.
	  /// </summary>
	  /// <param name="opPos"> The position of the first predicate the m_opMap array.
	  /// </param>
	  /// <returns> The number of predicates for this step.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int countPredicates(int opPos) throws javax.xml.transform.TransformerException
	  public virtual int countPredicates(int opPos)
	  {

		int count = 0;

		while (OpCodes.OP_PREDICATE == getOp(opPos))
		{
		  count++;

		  opPos = getNextOpPos(opPos);
		}

		return count;
	  }

	  /// <summary>
	  /// Compiles predicates in the step.
	  /// </summary>
	  /// <param name="opPos"> The position of the first predicate the m_opMap array. </param>
	  /// <param name="predicates"> An empty pre-determined array of 
	  ///            <seealso cref="org.apache.xpath.Expression"/>s, that will be filled in.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void compilePredicates(int opPos, org.apache.xpath.Expression[] predicates) throws javax.xml.transform.TransformerException
	  private void compilePredicates(int opPos, Expression[] predicates)
	  {

		for (int i = 0; OpCodes.OP_PREDICATE == getOp(opPos); i++)
		{
		  predicates[i] = predicate(opPos);
		  opPos = getNextOpPos(opPos);
		}
	  }

	  /// <summary>
	  /// Compile a built-in XPath function.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.functions.Function"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: org.apache.xpath.Expression compileFunction(int opPos) throws javax.xml.transform.TransformerException
	  internal virtual Expression compileFunction(int opPos)
	  {

		int endFunc = opPos + getOp(opPos + 1) - 1;

		opPos = getFirstChildPos(opPos);

		int funcID = getOp(opPos);

		opPos++;

		if (-1 != funcID)
		{
		  Function func = m_functionTable.getFunction(funcID);

		  /// <summary>
		  /// It is a trick for function-available. Since the function table is an
		  /// instance field, insert this table at compilation time for later usage
		  /// </summary>

		  if (func is FuncExtFunctionAvailable)
		  {
			  ((FuncExtFunctionAvailable) func).FunctionTable = m_functionTable;
		  }

		  func.postCompileStep(this);

		  try
		  {
			int i = 0;

			for (int p = opPos; p < endFunc; p = getNextOpPos(p), i++)
			{

			  // System.out.println("argPos: "+ p);
			  // System.out.println("argCode: "+ m_opMap[p]);
			  func.setArg(compile(p), i);
			}

			func.checkNumberArgs(i);
		  }
		  catch (WrongNumberArgsException wnae)
		  {
			string name = m_functionTable.getFunctionName(funcID);

			m_errorHandler.fatalError(new TransformerException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ONLY_ALLOWS, new object[]{name, wnae.Message}), m_locator));
				  //"name + " only allows " + wnae.getMessage() + " arguments", m_locator));
		  }

		  return func;
		}
		else
		{
		  error(XPATHErrorResources.ER_FUNCTION_TOKEN_NOT_FOUND, null); //"function token not found.");

		  return null;
		}
	  }

	  // The current id for extension functions.
	  private static long s_nextMethodId = 0;

	  /// <summary>
	  /// Get the next available method id
	  /// </summary>
	  private long NextMethodId
	  {
		  get
		  {
			  lock (this)
			  {
				if (s_nextMethodId == long.MaxValue)
				{
				  s_nextMethodId = 0;
				}
            
				return s_nextMethodId++;
			  }
		  }
	  }

	  /// <summary>
	  /// Compile an extension function.
	  /// </summary>
	  /// <param name="opPos"> The current position in the m_opMap array.
	  /// </param>
	  /// <returns> reference to <seealso cref="org.apache.xpath.functions.FuncExtFunction"/> instance.
	  /// </returns>
	  /// <exception cref="TransformerException"> if a error occurs creating the Expression. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private org.apache.xpath.Expression compileExtension(int opPos) throws javax.xml.transform.TransformerException
	  private Expression compileExtension(int opPos)
	  {

		int endExtFunc = opPos + getOp(opPos + 1) - 1;

		opPos = getFirstChildPos(opPos);

		string ns = (string) TokenQueue.elementAt(getOp(opPos));

		opPos++;

		string funcName = (string) TokenQueue.elementAt(getOp(opPos));

		opPos++;

		// We create a method key to uniquely identify this function so that we
		// can cache the object needed to invoke it.  This way, we only pay the
		// reflection overhead on the first call.

		Function extension = new FuncExtFunction(ns, funcName, NextMethodId.ToString());

		try
		{
		  int i = 0;

		  while (opPos < endExtFunc)
		  {
			int nextOpPos = getNextOpPos(opPos);

			extension.setArg(this.compile(opPos), i);

			opPos = nextOpPos;

			i++;
		  }
		}
		catch (WrongNumberArgsException)
		{
		  ; // should never happen
		}

		return extension;
	  }

	  /// <summary>
	  /// Warn the user of an problem.
	  /// </summary>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void warn(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void warn(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHWarning(msg, args);

		if (null != m_errorHandler)
		{
		  m_errorHandler.warning(new TransformerException(fmsg, m_locator));
		}
		else
		{
		  Console.WriteLine(fmsg + "; file " + m_locator.getSystemId() + "; line " + m_locator.getLineNumber() + "; column " + m_locator.getColumnNumber());
		}
	  }

	  /// <summary>
	  /// Tell the user of an assertion error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="b">  If false, a runtime exception will be thrown. </param>
	  /// <param name="msg"> The assertion message, which should be informative.
	  /// </param>
	  /// <exception cref="RuntimeException"> if the b argument is false. </exception>
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
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public override void error(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);


		if (null != m_errorHandler)
		{
		  m_errorHandler.fatalError(new TransformerException(fmsg, m_locator));
		}
		else
		{

		  // System.out.println(te.getMessage()
		  //                    +"; file "+te.getSystemId()
		  //                    +"; line "+te.getLineNumber()
		  //                    +"; column "+te.getColumnNumber());
		  throw new TransformerException(fmsg, (SAXSourceLocator)m_locator);
		}
	  }

	  /// <summary>
	  /// The current prefixResolver for the execution context.
	  /// </summary>
	  private PrefixResolver m_currentPrefixResolver = null;

	  /// <summary>
	  /// Get the current namespace context for the xpath.
	  /// </summary>
	  /// <returns> The current prefix resolver, *may* be null, though hopefully not. </returns>
	  public virtual PrefixResolver NamespaceContext
	  {
		  get
		  {
			return m_currentPrefixResolver;
		  }
		  set
		  {
			m_currentPrefixResolver = value;
		  }
	  }


	  /// <summary>
	  /// The error listener where errors will be sent.  If this is null, errors 
	  ///  and warnings will be sent to System.err.  May be null.    
	  /// </summary>
	  internal ErrorListener m_errorHandler;

	  /// <summary>
	  /// The source locator for the expression being compiled.  May be null. </summary>
	  internal SourceLocator m_locator;

	  /// <summary>
	  /// The FunctionTable for all xpath build-in functions
	  /// </summary>
	  private FunctionTable m_functionTable;
	}

}