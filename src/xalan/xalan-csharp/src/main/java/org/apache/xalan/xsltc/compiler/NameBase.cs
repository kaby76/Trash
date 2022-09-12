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
 * $Id: NameBase.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal class NameBase : FunctionCall
	{

		private Expression _param = null;
		private Type _paramType = Type.Node;

		/// <summary>
		/// Handles calls with no parameter (current node is implicit parameter).
		/// </summary>
		public NameBase(QName fname) : base(fname)
		{
		}

		/// <summary>
		/// Handles calls with one parameter (either node or node-set).
		/// </summary>
		public NameBase(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		_param = argument(0);
		}


		/// <summary>
		/// Check that we either have no parameters or one parameter that is
		/// either a node or a node-set.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{

		// Check the argument type (if any)
		switch (argumentCount())
		{
		case 0:
			_paramType = Type.Node;
			break;
		case 1:
			_paramType = _param.typeCheck(stable);
			break;
		default:
			throw new TypeCheckError(this);
		}

		// The argument has to be a node, a node-set or a node reference
		if ((_paramType != Type.NodeSet) && (_paramType != Type.Node) && (_paramType != Type.Reference))
		{
			throw new TypeCheckError(this);
		}

		return (_type = Type.String);
		}

		public override Type Type
		{
			get
			{
			return _type;
			}
		}

		/// <summary>
		/// Translate the code required for getting the node for which the
		/// QName, local-name or namespace URI should be extracted.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		il.append(methodGen.loadDOM());

		// Function was called with no parameters
		if (argumentCount() == 0)
		{
			il.append(methodGen.loadContextNode());
		}
		// Function was called with node parameter
		else if (_paramType == Type.Node)
		{
			_param.translate(classGen, methodGen);
		}
		else if (_paramType == Type.Reference)
		{
			_param.translate(classGen, methodGen);
			il.append(new INVOKESTATIC(cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToNodeSet", "(" + Constants_Fields.OBJECT_SIG + ")" + Constants_Fields.NODE_ITERATOR_SIG)));
			il.append(methodGen.nextNode());
		}
		// Function was called with node-set parameter
		else
		{
			_param.translate(classGen, methodGen);
			_param.startIterator(classGen, methodGen);
			il.append(methodGen.nextNode());
		}
		}
	}

}