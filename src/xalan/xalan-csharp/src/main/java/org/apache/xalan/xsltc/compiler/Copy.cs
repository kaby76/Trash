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
 * $Id: Copy.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IFNULL = org.apache.bcel.generic.IFNULL;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class Copy : Instruction
	{
		private UseAttributeSets _useSets;

		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String useSets = getAttribute("use-attribute-sets");
		string useSets = getAttribute("use-attribute-sets");
		if (useSets.Length > 0)
		{
				if (!Util.isValidQNames(useSets))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, useSets, this);
					parser.reportError(Constants.ERROR, err);
				}
			_useSets = new UseAttributeSets(useSets, parser);
		}
		parseChildren(parser);
		}

		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("Copy");
		this.indent(indent + IndentIncrement);
		displayContents(indent + IndentIncrement);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_useSets != null)
		{
			_useSets.typeCheck(stable);
		}
		typeCheckContents(stable);
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
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen name = methodGen.addLocalVariable2("name", org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(STRING_SIG), null);
		LocalVariableGen name = methodGen.addLocalVariable2("name", Util.getJCRefType(STRING_SIG), null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen length = methodGen.addLocalVariable2("length", org.apache.xalan.xsltc.compiler.util.Util.getJCRefType("I"), null);
		LocalVariableGen length = methodGen.addLocalVariable2("length", Util.getJCRefType("I"), null);

		// Get the name of the node to copy and save for later
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadCurrentNode());
		il.append(methodGen.loadHandler());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cpy = cpg.addInterfaceMethodref(DOM_INTF, "shallowCopy", "(" + NODE_SIG + TRANSLET_OUTPUT_SIG + ")" + STRING_SIG);
		int cpy = cpg.addInterfaceMethodref(DOM_INTF, "shallowCopy", "(" + NODE_SIG + TRANSLET_OUTPUT_SIG + ")" + STRING_SIG);
		il.append(new INVOKEINTERFACE(cpy, 3));
		il.append(DUP);
		name.setStart(il.append(new ASTORE(name.getIndex())));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifBlock1 = il.append(new org.apache.bcel.generic.IFNULL(null));
		BranchHandle ifBlock1 = il.append(new IFNULL(null));

		// Get the length of the node name and save for later
		il.append(new ALOAD(name.getIndex()));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lengthMethod = cpg.addMethodref(STRING_CLASS,"length","()I");
		int lengthMethod = cpg.addMethodref(STRING_CLASS,"length","()I");
		il.append(new INVOKEVIRTUAL(lengthMethod));
		length.setStart(il.append(new ISTORE(length.getIndex())));

		// Copy in attribute sets if specified
		if (_useSets != null)
		{
			// If the parent of this element will result in an element being
			// output then we know that it is safe to copy out the attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode parent = getParent();
			SyntaxTreeNode parent = Parent;
			if ((parent is LiteralElement) || (parent is LiteralElement))
			{
			_useSets.translate(classGen, methodGen);
			}
			// If not we have to check to see if the copy will result in an
			// element being output.
			else
			{
			// check if element; if not skip to translate body
			il.append(new ILOAD(length.getIndex()));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifBlock2 = il.append(new org.apache.bcel.generic.IFEQ(null));
			BranchHandle ifBlock2 = il.append(new IFEQ(null));
			// length != 0 -> element -> do attribute sets
			_useSets.translate(classGen, methodGen);
			// not an element; root
			ifBlock2.setTarget(il.append(NOP));
			}
		}

		// Instantiate body of xsl:copy
		translateContents(classGen, methodGen);

		// Call the output handler's endElement() if we copied an element
		// (The DOM.shallowCopy() method calls startElement().)
		length.setEnd(il.append(new ILOAD(length.getIndex())));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifBlock3 = il.append(new org.apache.bcel.generic.IFEQ(null));
		BranchHandle ifBlock3 = il.append(new IFEQ(null));
		il.append(methodGen.loadHandler());
		name.setEnd(il.append(new ALOAD(name.getIndex())));
		il.append(methodGen.endElement());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle end = il.append(NOP);
		InstructionHandle end = il.append(NOP);
		ifBlock1.setTarget(end);
		ifBlock3.setTarget(end);
		methodGen.removeLocalVariable(name);
		methodGen.removeLocalVariable(length);
		}
	}

}