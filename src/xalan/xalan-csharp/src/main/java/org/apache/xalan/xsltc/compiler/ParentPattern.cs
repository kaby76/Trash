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
 * $Id: ParentPattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
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
	/// </summary>
	internal sealed class ParentPattern : RelativePathPattern
	{
		private readonly Pattern _left;
		private readonly RelativePathPattern _right;

		public ParentPattern(Pattern left, RelativePathPattern right)
		{
		(_left = left).Parent = this;
		(_right = right).Parent = this;
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_left.Parser = value;
			_right.Parser = value;
			}
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
			return _right.KernelPattern;
			}
		}

		public override void reduceKernelPattern()
		{
		_right.reduceKernelPattern();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		_left.typeCheck(stable);
		return _right.typeCheck(stable);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen local = methodGen.addLocalVariable2("ppt", org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(Constants_Fields.NODE_SIG), null);
		LocalVariableGen local = methodGen.addLocalVariable2("ppt", Util.getJCRefType(Constants_Fields.NODE_SIG), null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction loadLocal = new org.apache.bcel.generic.ILOAD(local.getIndex());
		org.apache.bcel.generic.Instruction loadLocal = new ILOAD(local.Index);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction storeLocal = new org.apache.bcel.generic.ISTORE(local.getIndex());
		org.apache.bcel.generic.Instruction storeLocal = new ISTORE(local.Index);

		if (_right.Wildcard)
		{
			il.append(methodGen.loadDOM());
			il.append(SWAP);
		}
		else if (_right is StepPattern)
		{
			il.append(DUP);
			local.Start = il.append(storeLocal);

			_right.translate(classGen, methodGen);

			il.append(methodGen.loadDOM());
			local.End = il.append(loadLocal);
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

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getParent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_PARENT, Constants_Fields.GET_PARENT_SIG);
		int getParent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_PARENT, Constants_Fields.GET_PARENT_SIG);
		il.append(new INVOKEINTERFACE(getParent, 2));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode p = getParent();
		SyntaxTreeNode p = Parent;
		if (p == null || p is Instruction || p is TopLevelElement)
		{
			_left.translate(classGen, methodGen);
		}
		else
		{
			il.append(DUP);
			InstructionHandle storeInst = il.append(storeLocal);

				if (local.Start == null)
				{
					local.Start = storeInst;
				}
			_left.translate(classGen, methodGen);

			il.append(methodGen.loadDOM());
			local.End = il.append(loadLocal);
		}

		methodGen.removeLocalVariable(local);

		/*
		 * If _right is an ancestor pattern, backpatch _left false
		 * list to the loop that searches for more ancestors.
		 */
		if (_right is AncestorPattern)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AncestorPattern ancestor = (AncestorPattern) _right;
			AncestorPattern ancestor = (AncestorPattern) _right;
			_left.backPatchFalseList(ancestor.LoopHandle); // clears list
		}

		_trueList.append(_right._trueList.append(_left._trueList));
		_falseList.append(_right._falseList.append(_left._falseList));
		}

		public override string ToString()
		{
		return "Parent(" + _left + ", " + _right + ')';
		}
	}

}