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
 * $Id: FilterParentPath.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class FilterParentPath : Expression
	{

		private Expression _filterExpr;
		private Expression _path;
		private bool _hasDescendantAxis = false;

		public FilterParentPath(Expression filterExpr, Expression path)
		{
		(_path = path).setParent(this);
		(_filterExpr = filterExpr).setParent(this);
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_filterExpr.Parser = value;
			_path.Parser = value;
			}
		}

		public override string ToString()
		{
		return "FilterParentPath(" + _filterExpr + ", " + _path + ')';
		}

		public void setDescendantAxis()
		{
		_hasDescendantAxis = true;
		}

		/// <summary>
		/// Type check a FilterParentPath. If the filter is not a node-set add a 
		/// cast to node-set only if it is of reference type. This type coercion is
		/// needed for expressions like $x/LINE where $x is a parameter reference.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type ftype = _filterExpr.typeCheck(stable);
		Type ftype = _filterExpr.typeCheck(stable);
		if (ftype is NodeSetType == false)
		{
			if (ftype is ReferenceType)
			{
			_filterExpr = new CastExpr(_filterExpr, Type.NodeSet);
			}
			/*
			else if (ftype instanceof ResultTreeType)  {
			_filterExpr = new CastExpr(_filterExpr, Type.NodeSet);
			}
			*/
			else if (ftype is NodeType)
			{
			_filterExpr = new CastExpr(_filterExpr, Type.NodeSet);
			}
			else
			{
			throw new TypeCheckError(this);
			}
		}

		// Wrap single node path in a node set
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type ptype = _path.typeCheck(stable);
		Type ptype = _path.typeCheck(stable);
		if (!(ptype is NodeSetType))
		{
			_path = new CastExpr(_path, Type.NodeSet);
		}

		return _type = Type.NodeSet;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		// Create new StepIterator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initSI = cpg.addMethodref(STEP_ITERATOR_CLASS, "<init>", "(" +NODE_ITERATOR_SIG +NODE_ITERATOR_SIG +")V");
		int initSI = cpg.addMethodref(STEP_ITERATOR_CLASS, "<init>", "(" + NODE_ITERATOR_SIG + NODE_ITERATOR_SIG + ")V");

			// Backwards branches are prohibited if an uninitialized object is
			// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
			// We don't know whether this code might contain backwards branches,
			// so we mustn't create the new object until after we've created
			// the suspect arguments to its constructor.  Instead we calculate
			// the values of the arguments to the constructor first, store them
			// in temporary variables, create the object and reload the
			// arguments from the temporaries to avoid the problem.

		// Recursively compile 2 iterators
		_filterExpr.translate(classGen, methodGen);
			LocalVariableGen filterTemp = methodGen.addLocalVariable("filter_parent_path_tmp1", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
			filterTemp.setStart(il.append(new ASTORE(filterTemp.getIndex())));

		_path.translate(classGen, methodGen);
			LocalVariableGen pathTemp = methodGen.addLocalVariable("filter_parent_path_tmp2", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
			pathTemp.setStart(il.append(new ASTORE(pathTemp.getIndex())));

		il.append(new NEW(cpg.addClass(STEP_ITERATOR_CLASS)));
		il.append(DUP);
			filterTemp.setEnd(il.append(new ALOAD(filterTemp.getIndex())));
			pathTemp.setEnd(il.append(new ALOAD(pathTemp.getIndex())));

		// Initialize StepIterator with iterators from the stack
		il.append(new INVOKESPECIAL(initSI));

		// This is a special case for the //* path with or without predicates
			if (_hasDescendantAxis)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int incl = cpg.addMethodref(NODE_ITERATOR_BASE, "includeSelf", "()" + NODE_ITERATOR_SIG);
			int incl = cpg.addMethodref(NODE_ITERATOR_BASE, "includeSelf", "()" + NODE_ITERATOR_SIG);
			il.append(new INVOKEVIRTUAL(incl));
			}

			SyntaxTreeNode parent = Parent;

			bool parentAlreadyOrdered = (parent is RelativeLocationPath) || (parent is FilterParentPath) || (parent is KeyCall) || (parent is CurrentCall) || (parent is DocumentCall);

		if (!parentAlreadyOrdered)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = cpg.addInterfaceMethodref(DOM_INTF, ORDER_ITERATOR, ORDER_ITERATOR_SIG);
			int order = cpg.addInterfaceMethodref(DOM_INTF, ORDER_ITERATOR, ORDER_ITERATOR_SIG);
			il.append(methodGen.loadDOM());
			il.append(SWAP);
			il.append(methodGen.loadContextNode());
			il.append(new INVOKEINTERFACE(order, 3));
		}
		}
	}

}