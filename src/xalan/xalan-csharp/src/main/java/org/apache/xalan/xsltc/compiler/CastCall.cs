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
 * $Id: CastCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using ObjectType = org.apache.xalan.xsltc.compiler.util.ObjectType;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class CastCall : FunctionCall
	{

		/// <summary>
		/// Name of the class that is the target of the cast. Must be a 
		/// fully-qualified Java class Name.
		/// </summary>
		private string _className;

		/// <summary>
		/// A reference to the expression being casted.
		/// </summary>
		private Expression _right;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CastCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
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
			throw new TypeCheckError(new ErrorMsg(ErrorMsg.ILLEGAL_ARG_ERR, Name, this));
		}

			// The first argument must be a literal String
		Expression exp = argument(0);
			if (exp is LiteralExpr)
			{
				_className = ((LiteralExpr) exp).Value;
				_type = Type.newObjectType(_className);
			}
			else
			{
			throw new TypeCheckError(new ErrorMsg(ErrorMsg.NEED_LITERAL_ERR, Name, this));
			}

			 // Second argument must be of type reference or object
			_right = argument(1);
			Type tright = _right.typeCheck(stable);
			if (tright != Type.Reference && tright is ObjectType == false)
			{
			throw new TypeCheckError(new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, tright, _type, this));
			}

			return _type;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

			_right.translate(classGen, methodGen);
			il.append(new CHECKCAST(cpg.addClass(_className)));
		}
	}

}