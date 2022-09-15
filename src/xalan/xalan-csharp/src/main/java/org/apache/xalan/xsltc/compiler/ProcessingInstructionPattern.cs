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
 * $Id: ProcessingInstructionPattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IF_ICMPEQ = org.apache.bcel.generic.IF_ICMPEQ;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class ProcessingInstructionPattern : StepPattern
	{

		private string _name = null;
		private bool _typeChecked = false;

		/// <summary>
		/// Handles calls with no parameter (current node is implicit parameter).
		/// </summary>
		public ProcessingInstructionPattern(string name) : base(Axis.CHILD, DTM.PROCESSING_INSTRUCTION_NODE, null)
		{
		_name = name;
		//if (_name.equals("*")) _typeChecked = true; no wildcard allowed!
		}

		/// 
		 public override double DefaultPriority
		 {
			 get
			 {
				return (!string.ReferenceEquals(_name, null)) ? 0.0 : -0.5;
			 }
		 }
		public override string ToString()
		{
		if (_predicates == null)
		{
			return "processing-instruction(" + _name + ")";
		}
		else
		{
			return "processing-instruction(" + _name + ")" + _predicates;
		}
		}

		public override void reduceKernelPattern()
		{
		_typeChecked = true;
		}

		public override bool Wildcard
		{
			get
			{
			return false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (hasPredicates())
		{
			// Type check all the predicates (e -> position() = e)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _predicates.size();
			int n = _predicates.Count;
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Predicate pred = (Predicate)_predicates.elementAt(i);
			Predicate pred = (Predicate)_predicates[i];
			pred.typeCheck(stable);
			}
		}
		return Type.NodeSet;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// context node is on the stack
		int gname = cpg.addInterfaceMethodref(DOM_INTF, "getNodeName", "(I)Ljava/lang/String;");
		int cmp = cpg.addMethodref(STRING_CLASS, "equals", "(Ljava/lang/Object;)Z");

		// Push current node on the stack
		il.append(methodGen.loadCurrentNode());
		il.append(SWAP);

		// Overwrite current node with matching node
		il.append(methodGen.storeCurrentNode());

		// If pattern not reduced then check kernel
		if (!_typeChecked)
		{
			il.append(methodGen.loadCurrentNode());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
			int getType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
			il.append(methodGen.loadDOM());
			il.append(methodGen.loadCurrentNode());
			il.append(new INVOKEINTERFACE(getType, 2));
			il.append(new PUSH(cpg, DTM.PROCESSING_INSTRUCTION_NODE));
			_falseList.add(il.append(new IF_ICMPEQ(null)));
		}

		// Load the requested processing instruction name
		il.append(new PUSH(cpg, _name));
		// Load the current processing instruction's name
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadCurrentNode());
		il.append(new INVOKEINTERFACE(gname, 2));
		// Compare the two strings
		il.append(new INVOKEVIRTUAL(cmp));
		_falseList.add(il.append(new IFEQ(null)));

		// Compile the expressions within the predicates
		if (hasPredicates())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _predicates.size();
			int n = _predicates.Count;
			for (int i = 0; i < n; i++)
			{
			Predicate pred = (Predicate)_predicates[i];
			Expression exp = pred.Expr;
			exp.translateDesynthesized(classGen, methodGen);
			_trueList.append(exp._trueList);
			_falseList.append(exp._falseList);
			}
		}

		// Backpatch true list and restore current iterator/node
		InstructionHandle restore;
		restore = il.append(methodGen.storeCurrentNode());
		backPatchTrueList(restore);
		BranchHandle skipFalse = il.append(new GOTO(null));

		// Backpatch false list and restore current iterator/node
		restore = il.append(methodGen.storeCurrentNode());
		backPatchFalseList(restore);
		_falseList.add(il.append(new GOTO(null)));

		// True list falls through
		skipFalse.setTarget(il.append(NOP));
		}
	}

}