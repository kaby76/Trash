using System;
using System.Collections;
using System.Text;

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
 * $Id: StepPattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Field = org.apache.bcel.classfile.Field;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using GOTO = org.apache.bcel.generic.GOTO;
	using GOTO_W = org.apache.bcel.generic.GOTO_W;
	using IFLT = org.apache.bcel.generic.IFLT;
	using IFNE = org.apache.bcel.generic.IFNE;
	using IFNONNULL = org.apache.bcel.generic.IFNONNULL;
	using IF_ICMPEQ = org.apache.bcel.generic.IF_ICMPEQ;
	using IF_ICMPLT = org.apache.bcel.generic.IF_ICMPLT;
	using IF_ICMPNE = org.apache.bcel.generic.IF_ICMPNE;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal class StepPattern : RelativePathPattern
	{

		private const int NO_CONTEXT = 0;
		private const int SIMPLE_CONTEXT = 1;
		private const int GENERAL_CONTEXT = 2;

		protected internal readonly int _axis;
		protected internal readonly int _nodeType;
		protected internal ArrayList _predicates;

		private Step _step = null;
		private bool _isEpsilon = false;
		private int _contextCase;

		private double _priority = double.MaxValue;

		public StepPattern(int axis, int nodeType, ArrayList predicates)
		{
		_axis = axis;
		_nodeType = nodeType;
		_predicates = predicates;
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			if (_predicates != null)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = _predicates.size();
				int n = _predicates.Count;
				for (int i = 0; i < n; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Predicate exp = (Predicate)_predicates.elementAt(i);
				Predicate exp = (Predicate)_predicates[i];
				exp.Parser = value;
				exp.Parent = this;
				}
			}
			}
		}

		public virtual int NodeType
		{
			get
			{
			return _nodeType;
			}
		}

		public virtual double Priority
		{
			set
			{
			_priority = value;
			}
		}

		public override StepPattern KernelPattern
		{
			get
			{
			return this;
			}
		}

		public override bool Wildcard
		{
			get
			{
			return _isEpsilon && hasPredicates() == false;
			}
		}

		public virtual StepPattern setPredicates(ArrayList predicates)
		{
		_predicates = predicates;
		return (this);
		}

		protected internal virtual bool hasPredicates()
		{
		return _predicates != null && _predicates.Count > 0;
		}

		public override double DefaultPriority
		{
			get
			{
			if (_priority != double.MaxValue)
			{
				return _priority;
			}
    
			if (hasPredicates())
			{
				return 0.5;
			}
			else
			{
				switch (_nodeType)
				{
				case -1:
				return -0.5; // node()
				case 0:
				return 0.0;
				default:
				return (_nodeType >= NodeTest.GTYPE) ? 0.0 : -0.5;
				}
			}
			}
		}

		public override int Axis
		{
			get
			{
			return _axis;
			}
		}

		public override void reduceKernelPattern()
		{
		_isEpsilon = true;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = new StringBuffer("stepPattern(\"");
		StringBuilder buffer = new StringBuilder("stepPattern(\"");
		buffer.Append(Axis.getNames(_axis)).Append("\", ").Append(_isEpsilon ? ("epsilon{" + Convert.ToString(_nodeType) + "}") : Convert.ToString(_nodeType));
		if (_predicates != null)
		{
			buffer.Append(", ").Append(_predicates.ToString());
		}
		return buffer.Append(')').ToString();
		}

		private int analyzeCases()
		{
		bool noContext = true;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _predicates.size();
		int n = _predicates.Count;

		for (int i = 0; i < n && noContext; i++)
		{
			Predicate pred = (Predicate) _predicates[i];
				if (pred.NthPositionFilter || pred.hasPositionCall() || pred.hasLastCall())
				{
			noContext = false;
				}
		}

		if (noContext)
		{
			return NO_CONTEXT;
		}
		else if (n == 1)
		{
			return SIMPLE_CONTEXT;
		}
		return GENERAL_CONTEXT;
		}

		private string NextFieldName
		{
			get
			{
			return "__step_pattern_iter_" + XSLTC.nextStepPatternSerial();
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

				// Analyze context cases
				_contextCase = analyzeCases();

				Step step = null;

				// Create an instance of Step to do the translation
				if (_contextCase == SIMPLE_CONTEXT)
				{
					Predicate pred = (Predicate)_predicates[0];
					if (pred.NthPositionFilter)
					{
						_contextCase = GENERAL_CONTEXT;
						step = new Step(_axis, _nodeType, _predicates);
					}
					else
					{
						step = new Step(_axis, _nodeType, null);
					}
				}
				else if (_contextCase == GENERAL_CONTEXT)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = _predicates.size();
					int len = _predicates.Count;
					for (int i = 0; i < len; i++)
					{
						((Predicate)_predicates[i]).dontOptimize();
					}

					step = new Step(_axis, _nodeType, _predicates);
				}

				if (step != null)
				{
					step.Parser = Parser;
					step.typeCheck(stable);
					_step = step;
				}
			}
			return _axis == Axis.CHILD ? Type.Element : Type.Attribute;
		}

		private void translateKernel(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (_nodeType == DTM.ELEMENT_NODE)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int check = cpg.addInterfaceMethodref(DOM_INTF, "isElement", "(I)Z");
			int check = cpg.addInterfaceMethodref(DOM_INTF, "isElement", "(I)Z");
			il.append(methodGen.loadDOM());
			il.append(SWAP);
			il.append(new INVOKEINTERFACE(check, 2));

			// Need to allow for long jumps here
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle icmp = il.append(new org.apache.bcel.generic.IFNE(null));
			BranchHandle icmp = il.append(new IFNE(null));
			_falseList.add(il.append(new GOTO_W(null)));
			icmp.setTarget(il.append(NOP));
		}
		else if (_nodeType == DTM.ATTRIBUTE_NODE)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int check = cpg.addInterfaceMethodref(DOM_INTF, "isAttribute", "(I)Z");
			int check = cpg.addInterfaceMethodref(DOM_INTF, "isAttribute", "(I)Z");
			il.append(methodGen.loadDOM());
			il.append(SWAP);
			il.append(new INVOKEINTERFACE(check, 2));

			// Need to allow for long jumps here
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle icmp = il.append(new org.apache.bcel.generic.IFNE(null));
			BranchHandle icmp = il.append(new IFNE(null));
			_falseList.add(il.append(new GOTO_W(null)));
			icmp.setTarget(il.append(NOP));
		}
		else
		{
			// context node is on the stack
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getEType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
			int getEType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
			il.append(methodGen.loadDOM());
			il.append(SWAP);
			il.append(new INVOKEINTERFACE(getEType, 2));
			il.append(new PUSH(cpg, _nodeType));

			// Need to allow for long jumps here
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle icmp = il.append(new org.apache.bcel.generic.IF_ICMPEQ(null));
			BranchHandle icmp = il.append(new IF_ICMPEQ(null));
			_falseList.add(il.append(new GOTO_W(null)));
			icmp.setTarget(il.append(NOP));
		}
		}

		private void translateNoContext(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// Push current node on the stack
		il.append(methodGen.loadCurrentNode());
		il.append(SWAP);

		// Overwrite current node with matching node
		il.append(methodGen.storeCurrentNode());

		// If pattern not reduced then check kernel
		if (!_isEpsilon)
		{
			il.append(methodGen.loadCurrentNode());
			translateKernel(classGen, methodGen);
		}

		// Compile the expressions within the predicates
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

		private void translateSimpleContext(ClassGenerator classGen, MethodGenerator methodGen)
		{
		int index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// Store matching node into a local variable
		LocalVariableGen match;
		match = methodGen.addLocalVariable("step_pattern_tmp1", Util.getJCRefType(NODE_SIG), null, null);
		match.setStart(il.append(new ISTORE(match.getIndex())));

		// If pattern not reduced then check kernel
		if (!_isEpsilon)
		{
			il.append(new ILOAD(match.getIndex()));
			 translateKernel(classGen, methodGen);
		}

		// Push current iterator and current node on the stack
		il.append(methodGen.loadCurrentNode());
		il.append(methodGen.loadIterator());

		// Create a new matching iterator using the matching node
		index = cpg.addMethodref(MATCHING_ITERATOR, "<init>", "(I" + NODE_ITERATOR_SIG + ")V");

			// Backwards branches are prohibited if an uninitialized object is
			// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
			// We don't know whether this code might contain backwards branches,
			// so we mustn't create the new object until after we've created
			// the suspect arguments to its constructor.  Instead we calculate
			// the values of the arguments to the constructor first, store them
			// in temporary variables, create the object and reload the
			// arguments from the temporaries to avoid the problem.

		_step.translate(classGen, methodGen);
			LocalVariableGen stepIteratorTemp = methodGen.addLocalVariable("step_pattern_tmp2", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
			stepIteratorTemp.setStart(il.append(new ASTORE(stepIteratorTemp.getIndex())));

		il.append(new NEW(cpg.addClass(MATCHING_ITERATOR)));
		il.append(DUP);
		il.append(new ILOAD(match.getIndex()));
			stepIteratorTemp.setEnd(il.append(new ALOAD(stepIteratorTemp.getIndex())));
		il.append(new INVOKESPECIAL(index));

		// Get the parent of the matching node
		il.append(methodGen.loadDOM());
		il.append(new ILOAD(match.getIndex()));
		index = cpg.addInterfaceMethodref(DOM_INTF, GET_PARENT, GET_PARENT_SIG);
		il.append(new INVOKEINTERFACE(index, 2));

		// Start the iterator with the parent 
		il.append(methodGen.setStartNode());

		// Overwrite current iterator and current node
		il.append(methodGen.storeIterator());
		match.setEnd(il.append(new ILOAD(match.getIndex())));
		il.append(methodGen.storeCurrentNode());

		// Translate the expression of the predicate 
		Predicate pred = (Predicate) _predicates[0];
		Expression exp = pred.Expr;
		exp.translateDesynthesized(classGen, methodGen);

		// Backpatch true list and restore current iterator/node
		InstructionHandle restore = il.append(methodGen.storeIterator());
		il.append(methodGen.storeCurrentNode());
		exp.backPatchTrueList(restore);
		BranchHandle skipFalse = il.append(new GOTO(null));

		// Backpatch false list and restore current iterator/node
		restore = il.append(methodGen.storeIterator());
		il.append(methodGen.storeCurrentNode());
		exp.backPatchFalseList(restore);
		_falseList.add(il.append(new GOTO(null)));

		// True list falls through
		skipFalse.setTarget(il.append(NOP));
		}

		private void translateGeneralContext(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		int iteratorIndex = 0;
		BranchHandle ifBlock = null;
		LocalVariableGen iter, node, node2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String iteratorName = getNextFieldName();
		string iteratorName = NextFieldName;

		// Store node on the stack into a local variable
		node = methodGen.addLocalVariable("step_pattern_tmp1", Util.getJCRefType(NODE_SIG), null, null);
		node.setStart(il.append(new ISTORE(node.getIndex())));

		// Create a new local to store the iterator
		iter = methodGen.addLocalVariable("step_pattern_tmp2", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);

		// Add a new private field if this is the main class
		if (!classGen.External)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.classfile.Field iterator = new org.apache.bcel.classfile.Field(ACC_PRIVATE, cpg.addUtf8(iteratorName), cpg.addUtf8(NODE_ITERATOR_SIG), null, cpg.getConstantPool());
			Field iterator = new Field(ACC_PRIVATE, cpg.addUtf8(iteratorName), cpg.addUtf8(NODE_ITERATOR_SIG), null, cpg.getConstantPool());
			classGen.addField(iterator);
			iteratorIndex = cpg.addFieldref(classGen.ClassName, iteratorName, NODE_ITERATOR_SIG);

			il.append(classGen.loadTranslet());
			il.append(new GETFIELD(iteratorIndex));
			il.append(DUP);
			iter.setStart(il.append(new ASTORE(iter.getIndex())));
			ifBlock = il.append(new IFNONNULL(null));
			il.append(classGen.loadTranslet());
		}

		// Compile the step created at type checking time
		_step.translate(classGen, methodGen);
		InstructionHandle iterStore = il.append(new ASTORE(iter.getIndex()));

		// If in the main class update the field too
		if (!classGen.External)
		{
			il.append(new ALOAD(iter.getIndex()));
			il.append(new PUTFIELD(iteratorIndex));
			ifBlock.setTarget(il.append(NOP));
		}
		else
		{
				// If class is not external, start of range for iter variable was
				// set above
				iter.setStart(iterStore);
		}

		// Get the parent of the node on the stack
		il.append(methodGen.loadDOM());
		il.append(new ILOAD(node.getIndex()));
		int index = cpg.addInterfaceMethodref(DOM_INTF, GET_PARENT, GET_PARENT_SIG);
		il.append(new INVOKEINTERFACE(index, 2));

		// Initialize the iterator with the parent
		il.append(new ALOAD(iter.getIndex()));
		il.append(SWAP);
		il.append(methodGen.setStartNode());

		/* 
		 * Inline loop:
		 *
		 * int node2;
		 * while ((node2 = iter.next()) != NodeIterator.END 
		 *		  && node2 < node);
		 * return node2 == node; 
		 */
		BranchHandle skipNext;
		InstructionHandle begin, next;
		node2 = methodGen.addLocalVariable("step_pattern_tmp3", Util.getJCRefType(NODE_SIG), null, null);

		skipNext = il.append(new GOTO(null));
		next = il.append(new ALOAD(iter.getIndex()));
			node2.setStart(next);
		begin = il.append(methodGen.nextNode());
		il.append(DUP);
		il.append(new ISTORE(node2.getIndex()));
		_falseList.add(il.append(new IFLT(null))); // NodeIterator.END

		il.append(new ILOAD(node2.getIndex()));
		il.append(new ILOAD(node.getIndex()));
		iter.setEnd(il.append(new IF_ICMPLT(next)));

		node2.setEnd(il.append(new ILOAD(node2.getIndex())));
		node.setEnd(il.append(new ILOAD(node.getIndex())));
		_falseList.add(il.append(new IF_ICMPNE(null)));

		skipNext.setTarget(begin);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (hasPredicates())
		{
			switch (_contextCase)
			{
			case NO_CONTEXT:
			translateNoContext(classGen, methodGen);
			break;

			case SIMPLE_CONTEXT:
			translateSimpleContext(classGen, methodGen);
			break;

			default:
			translateGeneralContext(classGen, methodGen);
			break;
			}
		}
		else if (Wildcard)
		{
			il.append(POP); // true list falls through
		}
		else
		{
			translateKernel(classGen, methodGen);
		}
		}
	}

}