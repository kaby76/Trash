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
 * $Id: Expression.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO_W = org.apache.bcel.generic.GOTO_W;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal abstract class Expression : SyntaxTreeNode
	{
		/// <summary>
		/// The type of this expression. It is set after calling 
		/// <code>typeCheck()</code>.
		/// </summary>
		protected internal Type _type;

		/// <summary>
		/// Instruction handles that comprise the true list.
		/// </summary>
		protected internal FlowList _trueList = new FlowList();

		/// <summary>
		/// Instruction handles that comprise the false list.
		/// </summary>
		protected internal FlowList _falseList = new FlowList();

		public virtual Type Type
		{
			get
			{
			return _type;
			}
		}

		public override abstract string ToString();

		public virtual bool hasPositionCall()
		{
		return false; // default should be 'false' for StepPattern
		}

		public virtual bool hasLastCall()
		{
		return false;
		}

		/// <summary>
		/// Returns an object representing the compile-time evaluation 
		/// of an expression. We are only using this for function-available
		/// and element-available at this time.
		/// </summary>
		public virtual object evaluateAtCompileTime()
		{
		return null;
		}

		/// <summary>
		/// Type check all the children of this node.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return typeCheckContents(stable);
		}

		/// <summary>
		/// Translate this node into JVM bytecodes.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		ErrorMsg msg = new ErrorMsg(ErrorMsg.NOT_IMPLEMENTED_ERR, this.GetType(), this);
		Parser.reportError(Constants_Fields.FATAL, msg);
		}

		/// <summary>
		/// Translate this node into a fresh instruction list.
		/// The original instruction list is saved and restored.
		/// </summary>
		public InstructionList compile(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList result, save = methodGen.getInstructionList();
		InstructionList result , save = methodGen.InstructionList;
		methodGen.InstructionList = result = new InstructionList();
		translate(classGen, methodGen);
		methodGen.InstructionList = save;
		return result;
		}

		/// <summary>
		/// Redefined by expressions of type boolean that use flow lists.
		/// </summary>
		public virtual void translateDesynthesized(ClassGenerator classGen, MethodGenerator methodGen)
		{
		translate(classGen, methodGen);
		if (_type is BooleanType)
		{
			desynthesize(classGen, methodGen);
		}
		}

		/// <summary>
		/// If this expression is of type node-set and it is not a variable
		/// reference, then call setStartNode() passing the context node.
		/// </summary>
		public virtual void startIterator(ClassGenerator classGen, MethodGenerator methodGen)
		{
		// Ignore if type is not node-set
		if (_type is NodeSetType == false)
		{
			return;
		}

		// setStartNode() should not be called if expr is a variable ref
		Expression expr = this;
		if (expr is CastExpr)
		{
			expr = ((CastExpr) expr).Expr;
		}
		if (expr is VariableRefBase == false)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.InstructionList;
			il.append(methodGen.loadContextNode());
			il.append(methodGen.setStartNode());
		}
		}

		/// <summary>
		/// Synthesize a boolean expression, i.e., either push a 0 or 1 onto the 
		/// operand stack for the next statement to succeed. Returns the handle
		/// of the instruction to be backpatched.
		/// </summary>
		public virtual void synthesize(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		_trueList.backPatch(il.append(ICONST_1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle truec = il.append(new org.apache.bcel.generic.GOTO_W(null));
		BranchHandle truec = il.append(new GOTO_W(null));
		_falseList.backPatch(il.append(ICONST_0));
		truec.Target = il.append(NOP);
		}

		public virtual void desynthesize(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		_falseList.add(il.append(new IFEQ(null)));
		}

		public virtual FlowList FalseList
		{
			get
			{
			return _falseList;
			}
		}

		public virtual FlowList TrueList
		{
			get
			{
			return _trueList;
			}
		}

		public virtual void backPatchFalseList(InstructionHandle ih)
		{
		_falseList.backPatch(ih);
		}

		public virtual void backPatchTrueList(InstructionHandle ih)
		{
		_trueList.backPatch(ih);
		}

		/// <summary>
		/// Search for a primop in the symbol table that matches the method type 
		/// <code>ctype</code>. Two methods match if they have the same arity.
		/// If a primop is overloaded then the "closest match" is returned. The
		/// first entry in the vector of primops that has the right arity is 
		/// considered to be the default one.
		/// </summary>
		public virtual MethodType lookupPrimop(SymbolTable stable, string op, MethodType ctype)
		{
		MethodType result = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector primop = stable.lookupPrimop(op);
		ArrayList primop = stable.lookupPrimop(op);
		if (primop != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = primop.size();
			int n = primop.Count;
			int minDistance = int.MaxValue;
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodType ptype = (org.apache.xalan.xsltc.compiler.util.MethodType) primop.elementAt(i);
			MethodType ptype = (MethodType) primop[i];
			// Skip if different arity
			if (ptype.argsCount() != ctype.argsCount())
			{
				continue;
			}

			// The first method with the right arity is the default
			if (result == null)
			{
				result = ptype; // default method
			}

			// Check if better than last one found
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int distance = ctype.distanceTo(ptype);
			int distance = ctype.distanceTo(ptype);
			if (distance < minDistance)
			{
				minDistance = distance;
				result = ptype;
			}
			}
		}
		return result;
		}
	}

}