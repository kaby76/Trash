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
 * $Id: AbsolutePathPattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO_W = org.apache.bcel.generic.GOTO_W;
	using IF_ICMPEQ = org.apache.bcel.generic.IF_ICMPEQ;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class AbsolutePathPattern : LocationPathPattern
	{
		private readonly RelativePathPattern _left; // may be null

		public AbsolutePathPattern(RelativePathPattern left)
		{
		_left = left;
		if (left != null)
		{
			left.Parent = this;
		}
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			if (_left != null)
			{
				_left.Parser = value;
			}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return _left == null ? Type.Root : _left.typeCheck(stable);
		}

		public override bool Wildcard
		{
			get
			{
			return false;
			}
		}

		public override StepPattern KernelPattern
		{
			get
			{
			return _left != null ? _left.KernelPattern : null;
			}
		}

		public override void reduceKernelPattern()
		{
		_left.reduceKernelPattern();
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		if (_left != null)
		{
			if (_left is StepPattern)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen local = methodGen.addLocalVariable2("apptmp", org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(Constants_Fields.NODE_SIG), null);
			LocalVariableGen local = methodGen.addLocalVariable2("apptmp", Util.getJCRefType(Constants_Fields.NODE_SIG), null);
				// absolute path pattern temporary
			il.append(DUP);
			local.Start = il.append(new ISTORE(local.Index));
			_left.translate(classGen, methodGen);
			il.append(methodGen.loadDOM());
			local.End = il.append(new ILOAD(local.Index));
			methodGen.removeLocalVariable(local);
			}
			else
			{
			_left.translate(classGen, methodGen);
			}
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getParent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_PARENT, Constants_Fields.GET_PARENT_SIG);
		int getParent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_PARENT, Constants_Fields.GET_PARENT_SIG);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getType = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getExpandedTypeID", "(I)I");
		int getType = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getExpandedTypeID", "(I)I");

		InstructionHandle begin = il.append(methodGen.loadDOM());
		il.append(SWAP);
		il.append(new INVOKEINTERFACE(getParent, 2));
		if (_left is AncestorPattern)
		{
			il.append(methodGen.loadDOM());
			il.append(SWAP);
		}
		il.append(new INVOKEINTERFACE(getType, 2));
		il.append(new PUSH(cpg, org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle skip = il.append(new org.apache.bcel.generic.IF_ICMPEQ(null));
		BranchHandle skip = il.append(new IF_ICMPEQ(null));
		_falseList.add(il.append(new GOTO_W(null)));
		skip.Target = il.append(NOP);

		if (_left != null)
		{
			_left.backPatchTrueList(begin);

			/*
			 * If _left is an ancestor pattern, backpatch this pattern's false
			 * list to the loop that searches for more ancestors.
			 */
			if (_left is AncestorPattern)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AncestorPattern ancestor = (AncestorPattern) _left;
			AncestorPattern ancestor = (AncestorPattern) _left;
			_falseList.backPatch(ancestor.LoopHandle); // clears list
			}
			_falseList.append(_left._falseList);
		}
		}

		public override string ToString()
		{
		return "absolutePathPattern(" + (_left != null ? _left.ToString() : ")");
		}
	}

}