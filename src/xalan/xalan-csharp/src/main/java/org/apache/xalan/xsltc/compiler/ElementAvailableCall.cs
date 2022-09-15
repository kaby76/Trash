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
 * $Id: ElementAvailableCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class ElementAvailableCall : FunctionCall
	{

		public ElementAvailableCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		}

		/// <summary>
		/// Force the argument to this function to be a literal string.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (argument() is LiteralExpr)
		{
			return _type = Type.Boolean;
		}
		ErrorMsg err = new ErrorMsg(ErrorMsg.NEED_LITERAL_ERR, "element-available", this);
		throw new TypeCheckError(err);
		}

		/// <summary>
		/// Returns an object representing the compile-time evaluation 
		/// of an expression. We are only using this for function-available
		/// and element-available at this time.
		/// </summary>
		public override object evaluateAtCompileTime()
		{
		return Result ? true : false;
		}

		/// <summary>
		/// Returns the result that this function will return
		/// </summary>
		public bool Result
		{
			get
			{
			try
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final LiteralExpr arg = (LiteralExpr) argument();
				LiteralExpr arg = (LiteralExpr) argument();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String qname = arg.getValue();
				string qname = arg.Value;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int index = qname.indexOf(':');
				int index = qname.IndexOf(':');
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String localName = (index > 0) ? qname.substring(index + 1) : qname;
				string localName = (index > 0) ? qname.Substring(index + 1) : qname;
				return Parser.elementSupported(arg.Namespace, localName);
			}
			catch (System.InvalidCastException)
			{
				return false;
			}
			}
		}

		/// <summary>
		/// Calls to 'element-available' are resolved at compile time since 
		/// the namespaces declared in the stylsheet are not available at run
		/// time. Consequently, arguments to this function must be literals.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean result = getResult();
		bool result = Result;
		methodGen.getInstructionList().append(new PUSH(cpg, result));
		}
	}

}