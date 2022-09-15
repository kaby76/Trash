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
 * $Id: EqualityExpr.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using BranchInstruction = org.apache.bcel.generic.BranchInstruction;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IFNE = org.apache.bcel.generic.IFNE;
	using IF_ICMPEQ = org.apache.bcel.generic.IF_ICMPEQ;
	using IF_ICMPNE = org.apache.bcel.generic.IF_ICMPNE;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using IntType = org.apache.xalan.xsltc.compiler.util.IntType;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using NumberType = org.apache.xalan.xsltc.compiler.util.NumberType;
	using RealType = org.apache.xalan.xsltc.compiler.util.RealType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using ResultTreeType = org.apache.xalan.xsltc.compiler.util.ResultTreeType;
	using StringType = org.apache.xalan.xsltc.compiler.util.StringType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Operators = org.apache.xalan.xsltc.runtime.Operators;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal sealed class EqualityExpr : Expression
	{

		private readonly int _op;
		private Expression _left;
		private Expression _right;

		public EqualityExpr(int op, Expression left, Expression right)
		{
		_op = op;
		(_left = left).setParent(this);
		(_right = right).setParent(this);
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

		public override string ToString()
		{
			return Operators.getOpNames(_op) + '(' + _left + ", " + _right + ')';
		}

		public Expression Left
		{
			get
			{
			return _left;
			}
		}

		public Expression Right
		{
			get
			{
			return _right;
			}
		}

		public bool Op
		{
			get
			{
				return (_op != Operators.NE);
			}
		}

		/// <summary>
		/// Returns true if this expressions contains a call to position(). This is
		/// needed for context changes in node steps containing multiple predicates.
		/// </summary>
		public override bool hasPositionCall()
		{
		if (_left.hasPositionCall())
		{
			return true;
		}
		if (_right.hasPositionCall())
		{
			return true;
		}
		return false;
		}

		public override bool hasLastCall()
		{
		if (_left.hasLastCall())
		{
			return true;
		}
		if (_right.hasLastCall())
		{
			return true;
		}
		return false;
		}

		private void swapArguments()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression temp = _left;
		Expression temp = _left;
		_left = _right;
		_right = temp;
		}

		/// <summary>
		/// Typing rules: see XSLT Reference by M. Kay page 345.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tleft = _left.typeCheck(stable);
		Type tleft = _left.typeCheck(stable);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tright = _right.typeCheck(stable);
		Type tright = _right.typeCheck(stable);

		if (tleft.Simple && tright.Simple)
		{
			if (tleft != tright)
			{
			if (tleft is BooleanType)
			{
				_right = new CastExpr(_right, Type.Boolean);
			}
			else if (tright is BooleanType)
			{
				_left = new CastExpr(_left, Type.Boolean);
			}
			else if (tleft is NumberType || tright is NumberType)
			{
				_left = new CastExpr(_left, Type.Real);
				_right = new CastExpr(_right, Type.Real);
			}
			else
			{ // both compared as strings
				_left = new CastExpr(_left, Type.String);
				_right = new CastExpr(_right, Type.String);
			}
			}
		}
		else if (tleft is ReferenceType)
		{
			_right = new CastExpr(_right, Type.Reference);
		}
		else if (tright is ReferenceType)
		{
			_left = new CastExpr(_left, Type.Reference);
		}
		// the following 2 cases optimize @attr|.|.. = 'string'
		else if (tleft is NodeType && tright == Type.String)
		{
			_left = new CastExpr(_left, Type.String);
		}
		else if (tleft == Type.String && tright is NodeType)
		{
			_right = new CastExpr(_right, Type.String);
		}
		// optimize node/node
		else if (tleft is NodeType && tright is NodeType)
		{
			_left = new CastExpr(_left, Type.String);
			_right = new CastExpr(_right, Type.String);
		}
		else if (tleft is NodeType && tright is NodeSetType)
		{
			// compare(Node, NodeSet) will be invoked
		}
		else if (tleft is NodeSetType && tright is NodeType)
		{
			swapArguments(); // for compare(Node, NodeSet)
		}
		else
		{
			// At least one argument is of type node, node-set or result-tree

			// Promote an expression of type node to node-set
			if (tleft is NodeType)
			{
			_left = new CastExpr(_left, Type.NodeSet);
			}
			if (tright is NodeType)
			{
			_right = new CastExpr(_right, Type.NodeSet);
			}

			// If one arg is a node-set then make it the left one
			if (tleft.Simple || tleft is ResultTreeType && tright is NodeSetType)
			{
			swapArguments();
			}

			// Promote integers to doubles to have fewer compares
			if (_right.Type is IntType)
			{
			_right = new CastExpr(_right, Type.Real);
			}
		}
		return _type = Type.Boolean;
		}

		public override void translateDesynthesized(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tleft = _left.getType();
		Type tleft = _left.Type;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (tleft is BooleanType)
		{
			_left.translate(classGen, methodGen);
			_right.translate(classGen, methodGen);
			_falseList.add(il.append(_op == Operators.EQ ? (BranchInstruction)new IF_ICMPNE(null) : (BranchInstruction)new IF_ICMPEQ(null)));
		}
		else if (tleft is NumberType)
		{
			_left.translate(classGen, methodGen);
			_right.translate(classGen, methodGen);

			if (tleft is RealType)
			{
			il.append(DCMPG);
			_falseList.add(il.append(_op == Operators.EQ ? (BranchInstruction)new IFNE(null) : (BranchInstruction)new IFEQ(null)));
			}
			else
			{
				_falseList.add(il.append(_op == Operators.EQ ? (BranchInstruction)new IF_ICMPNE(null) : (BranchInstruction)new IF_ICMPEQ(null)));
			}
		}
		else
		{
			translate(classGen, methodGen);
			desynthesize(classGen, methodGen);
		}
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
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tleft = _left.getType();
		Type tleft = _left.Type;
		Type tright = _right.Type;

		if (tleft is BooleanType || tleft is NumberType)
		{
			translateDesynthesized(classGen, methodGen);
			synthesize(classGen, methodGen);
			return;
		}

		if (tleft is StringType)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int equals = cpg.addMethodref(STRING_CLASS, "equals", "(" + OBJECT_SIG +")Z");
			int equals = cpg.addMethodref(STRING_CLASS, "equals", "(" + OBJECT_SIG + ")Z");
			_left.translate(classGen, methodGen);
			_right.translate(classGen, methodGen);
			il.append(new INVOKEVIRTUAL(equals));

			if (_op == Operators.NE)
			{
			il.append(ICONST_1);
			il.append(IXOR); // not x <-> x xor 1
			}
			return;
		}

		BranchHandle truec, falsec;

		if (tleft is ResultTreeType)
		{
			if (tright is BooleanType)
			{
			_right.translate(classGen, methodGen);
			if (_op == Operators.NE)
			{
				il.append(ICONST_1);
				il.append(IXOR); // not x <-> x xor 1
			}
			return;
			}

			if (tright is RealType)
			{
			_left.translate(classGen, methodGen);
			tleft.translateTo(classGen, methodGen, Type.Real);
			_right.translate(classGen, methodGen);

			il.append(DCMPG);
			falsec = il.append(_op == Operators.EQ ? (BranchInstruction) new IFNE(null) : (BranchInstruction) new IFEQ(null));
			il.append(ICONST_1);
			truec = il.append(new GOTO(null));
			falsec.setTarget(il.append(ICONST_0));
			truec.setTarget(il.append(NOP));
			return;
			}

			// Next, result-tree/string and result-tree/result-tree comparisons

			_left.translate(classGen, methodGen);
			tleft.translateTo(classGen, methodGen, Type.String);
			_right.translate(classGen, methodGen);

			if (tright is ResultTreeType)
			{
			tright.translateTo(classGen, methodGen, Type.String);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int equals = cpg.addMethodref(STRING_CLASS, "equals", "(" +OBJECT_SIG+ ")Z");
			int equals = cpg.addMethodref(STRING_CLASS, "equals", "(" + OBJECT_SIG + ")Z");
			il.append(new INVOKEVIRTUAL(equals));

			if (_op == Operators.NE)
			{
			il.append(ICONST_1);
			il.append(IXOR); // not x <-> x xor 1
			}
			return;
		}

		if (tleft is NodeSetType && tright is BooleanType)
		{
			_left.translate(classGen, methodGen);
			_left.startIterator(classGen, methodGen);
			Type.NodeSet.translateTo(classGen, methodGen, Type.Boolean);
			_right.translate(classGen, methodGen);

			il.append(IXOR); // x != y <-> x xor y
			if (_op == Operators.EQ)
			{
			il.append(ICONST_1);
			il.append(IXOR); // not x <-> x xor 1
			}
			return;
		}

		if (tleft is NodeSetType && tright is StringType)
		{
			_left.translate(classGen, methodGen);
			_left.startIterator(classGen, methodGen); // needed ?
			_right.translate(classGen, methodGen);
			il.append(new PUSH(cpg, _op));
			il.append(methodGen.loadDOM());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cmp = cpg.addMethodref(BASIS_LIBRARY_CLASS, "compare", "(" + tleft.toSignature() + tright.toSignature() + "I" + DOM_INTF_SIG + ")Z");
			int cmp = cpg.addMethodref(BASIS_LIBRARY_CLASS, "compare", "(" + tleft.toSignature() + tright.toSignature() + "I" + DOM_INTF_SIG + ")Z");
			il.append(new INVOKESTATIC(cmp));
			return;
		}

		// Next, node-set/t for t in {real, string, node-set, result-tree}
		_left.translate(classGen, methodGen);
		_left.startIterator(classGen, methodGen);
		_right.translate(classGen, methodGen);
		_right.startIterator(classGen, methodGen);

		// Cast a result tree to a string to use an existing compare
		if (tright is ResultTreeType)
		{
			tright.translateTo(classGen, methodGen, Type.String);
			tright = Type.String;
		}

		// Call the appropriate compare() from the BasisLibrary
		il.append(new PUSH(cpg, _op));
		il.append(methodGen.loadDOM());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int compare = cpg.addMethodref(BASIS_LIBRARY_CLASS, "compare", "(" + tleft.toSignature() + tright.toSignature() + "I" + DOM_INTF_SIG + ")Z");
		int compare = cpg.addMethodref(BASIS_LIBRARY_CLASS, "compare", "(" + tleft.toSignature() + tright.toSignature() + "I" + DOM_INTF_SIG + ")Z");
		il.append(new INVOKESTATIC(compare));
		}
	}

}