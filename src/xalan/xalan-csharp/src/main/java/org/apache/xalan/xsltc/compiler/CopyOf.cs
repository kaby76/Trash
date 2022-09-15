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
 * $Id: CopyOf.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using ResultTreeType = org.apache.xalan.xsltc.compiler.util.ResultTreeType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class CopyOf : Instruction
	{
		private Expression _select;

		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("CopyOf");
		this.indent(indent + IndentIncrement);
		Util.println("select " + _select.ToString());
		}

		public override void parseContents(Parser parser)
		{
		_select = parser.parseExpression(this, "select", null);
			// make sure required attribute(s) have been set
			if (_select.Dummy)
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "select");
			return;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tselect = _select.typeCheck(stable);
		Type tselect = _select.typeCheck(stable);
		if (tselect is NodeType || tselect is NodeSetType || tselect is ReferenceType || tselect is ResultTreeType)
		{
			// falls through 
		}
		else
		{
			_select = new CastExpr(_select, Type.String);
		}
		return Type.Void;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tselect = _select.getType();
		Type tselect = _select.Type;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String CPY1_SIG = "("+NODE_ITERATOR_SIG+TRANSLET_OUTPUT_SIG+")V";
		string CPY1_SIG = "(" + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + ")V";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cpy1 = cpg.addInterfaceMethodref(DOM_INTF, "copy", CPY1_SIG);
		int cpy1 = cpg.addInterfaceMethodref(DOM_INTF, "copy", CPY1_SIG);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String CPY2_SIG = "("+NODE_SIG+TRANSLET_OUTPUT_SIG+")V";
		string CPY2_SIG = "(" + NODE_SIG + TRANSLET_OUTPUT_SIG + ")V";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cpy2 = cpg.addInterfaceMethodref(DOM_INTF, "copy", CPY2_SIG);
		int cpy2 = cpg.addInterfaceMethodref(DOM_INTF, "copy", CPY2_SIG);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String getDoc_SIG = "()"+NODE_SIG;
		string getDoc_SIG = "()" + NODE_SIG;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getDoc = cpg.addInterfaceMethodref(DOM_INTF, "getDocument", getDoc_SIG);
		int getDoc = cpg.addInterfaceMethodref(DOM_INTF, "getDocument", getDoc_SIG);


		if (tselect is NodeSetType)
		{
			il.append(methodGen.loadDOM());

			// push NodeIterator
			_select.translate(classGen, methodGen);
			_select.startIterator(classGen, methodGen);

			// call copy from the DOM 'library'
			il.append(methodGen.loadHandler());
			il.append(new INVOKEINTERFACE(cpy1, 3));
		}
		else if (tselect is NodeType)
		{
			il.append(methodGen.loadDOM());
			_select.translate(classGen, methodGen);
			il.append(methodGen.loadHandler());
			il.append(new INVOKEINTERFACE(cpy2, 3));
		}
		else if (tselect is ResultTreeType)
		{
			_select.translate(classGen, methodGen);
			// We want the whole tree, so we start with the root node
			il.append(DUP); //need a pointer to the DOM ;
			il.append(new INVOKEINTERFACE(getDoc,1)); //ICONST_0);
			il.append(methodGen.loadHandler());
			il.append(new INVOKEINTERFACE(cpy2, 3));
		}
		else if (tselect is ReferenceType)
		{
			_select.translate(classGen, methodGen);
			il.append(methodGen.loadHandler());
			il.append(methodGen.loadCurrentNode());
			il.append(methodGen.loadDOM());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int copy = cpg.addMethodref(BASIS_LIBRARY_CLASS, "copy", "(" + OBJECT_SIG + TRANSLET_OUTPUT_SIG + NODE_SIG + DOM_INTF_SIG + ")V");
			int copy = cpg.addMethodref(BASIS_LIBRARY_CLASS, "copy", "(" + OBJECT_SIG + TRANSLET_OUTPUT_SIG + NODE_SIG + DOM_INTF_SIG + ")V");
			il.append(new INVOKESTATIC(copy));
		}
		else
		{
			il.append(classGen.loadTranslet());
			_select.translate(classGen, methodGen);
			il.append(methodGen.loadHandler());
			il.append(new INVOKEVIRTUAL(cpg.addMethodref(TRANSLET_CLASS, CHARACTERSW, CHARACTERSW_SIG)));
		}

		}
	}

}