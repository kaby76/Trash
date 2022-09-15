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
 * $Id: ContainsCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using IFLT = org.apache.bcel.generic.IFLT;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class ContainsCall : FunctionCall
	{

		private Expression _base = null;
		private Expression _token = null;

		/// <summary>
		/// Create a contains() call - two arguments, both strings
		/// </summary>
		public ContainsCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		}

		/// <summary>
		/// This XPath function returns true/false values
		/// </summary>
		public bool Boolean
		{
			get
			{
			return true;
			}
		}

		/// <summary>
		/// Type check the two parameters for this function
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{

		// Check that the function was passed exactly two arguments
		if (argumentCount() != 2)
		{
			throw new TypeCheckError(ErrorMsg.ILLEGAL_ARG_ERR, Name, this);
		}

		// The first argument must be a String, or cast to a String
		_base = argument(0);
		Type baseType = _base.typeCheck(stable);
		if (baseType != Type.String)
		{
			_base = new CastExpr(_base, Type.String);
		}

		// The second argument must also be a String, or cast to a String
		_token = argument(1);
		Type tokenType = _token.typeCheck(stable);
		if (tokenType != Type.String)
		{
			_token = new CastExpr(_token, Type.String);
		}

		return _type = Type.Boolean;
		}

		/// <summary>
		/// Compile the expression - leave boolean expression on stack
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		translateDesynthesized(classGen, methodGen);
		synthesize(classGen, methodGen);
		}

		/// <summary>
		/// Compile expression and update true/false-lists
		/// </summary>
		public override void translateDesynthesized(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		_base.translate(classGen, methodGen);
		_token.translate(classGen, methodGen);
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(STRING_CLASS, "indexOf", "(" + STRING_SIG + ")I")));
		_falseList.add(il.append(new IFLT(null)));
		}
	}

}