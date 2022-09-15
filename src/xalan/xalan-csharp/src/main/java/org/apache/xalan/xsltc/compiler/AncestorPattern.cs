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
 * $Id: AncestorPattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFLT = org.apache.bcel.generic.IFLT;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal sealed class AncestorPattern : RelativePathPattern
	{

		private readonly Pattern _left; // may be null
		private readonly RelativePathPattern _right;
		private InstructionHandle _loop;

		public AncestorPattern(RelativePathPattern right) : this(null, right)
		{
		}

		public AncestorPattern(Pattern left, RelativePathPattern right)
		{
		_left = left;
		(_right = right).setParent(this);
		if (left != null)
		{
			left.Parent = this;
		}
		}

		public InstructionHandle LoopHandle
		{
			get
			{
			return _loop;
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
			_right.Parser = value;
			}
		}

		public override bool Wildcard
		{
			get
			{
			//!!! can be wildcard
			return false;
			}
		}

		public override StepPattern KernelPattern
		{
			get
			{
			return _right.KernelPattern;
			}
		}

		public override void reduceKernelPattern()
		{
		_right.reduceKernelPattern();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_left != null)
		{
			_left.typeCheck(stable);
		}
		return _right.typeCheck(stable);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		InstructionHandle parent;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		/* 
		 * The scope of this local var must be the entire method since
		 * a another pattern may decide to jump back into the loop
		 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen local = methodGen.addLocalVariable2("app", org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(NODE_SIG), il.getEnd());
		LocalVariableGen local = methodGen.addLocalVariable2("app", Util.getJCRefType(NODE_SIG), il.getEnd());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction loadLocal = new org.apache.bcel.generic.ILOAD(local.getIndex());
		org.apache.bcel.generic.Instruction loadLocal = new ILOAD(local.getIndex());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction storeLocal = new org.apache.bcel.generic.ISTORE(local.getIndex());
		org.apache.bcel.generic.Instruction storeLocal = new ISTORE(local.getIndex());

		if (_right is StepPattern)
		{
			il.append(DUP);
			il.append(storeLocal);
			_right.translate(classGen, methodGen);
			il.append(methodGen.loadDOM());
			il.append(loadLocal);
		}
		else
		{
			_right.translate(classGen, methodGen);

			if (_right is AncestorPattern)
			{
			il.append(methodGen.loadDOM());
			il.append(SWAP);
			}
		}

		if (_left != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getParent = cpg.addInterfaceMethodref(DOM_INTF, GET_PARENT, GET_PARENT_SIG);
			int getParent = cpg.addInterfaceMethodref(DOM_INTF, GET_PARENT, GET_PARENT_SIG);
			parent = il.append(new INVOKEINTERFACE(getParent, 2));

			il.append(DUP);
			il.append(storeLocal);
			_falseList.add(il.append(new IFLT(null)));
			il.append(loadLocal);

			_left.translate(classGen, methodGen);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode p = getParent();
			SyntaxTreeNode p = this.Parent;
			if (p == null || p is Instruction || p is TopLevelElement)
			{
			// do nothing
			}
			else
			{
			il.append(loadLocal);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle exit = il.append(new org.apache.bcel.generic.GOTO(null));
			BranchHandle exit = il.append(new GOTO(null));
			_loop = il.append(methodGen.loadDOM());
			il.append(loadLocal);
			local.setEnd(_loop);
			il.append(new GOTO(parent));
			exit.setTarget(il.append(NOP));
			_left.backPatchFalseList(_loop);

			_trueList.append(_left._trueList);
		}
		else
		{
			il.append(POP2);
		}

		/* 
		 * If _right is an ancestor pattern, backpatch this pattern's false
		 * list to the loop that searches for more ancestors.
		 */
		if (_right is AncestorPattern)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AncestorPattern ancestor = (AncestorPattern) _right;
			AncestorPattern ancestor = (AncestorPattern) _right;
			_falseList.backPatch(ancestor.LoopHandle); // clears list
		}

		_trueList.append(_right._trueList);
		_falseList.append(_right._falseList);
		}

		public override string ToString()
		{
		return "AncestorPattern(" + _left + ", " + _right + ')';
		}
	}

}