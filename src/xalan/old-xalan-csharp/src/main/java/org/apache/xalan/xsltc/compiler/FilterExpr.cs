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
 * $Id: FilterExpr.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal class FilterExpr : Expression
	{

		/// <summary>
		/// Primary expression of this filter. I.e., 'e' in '(e)[p1]...[pn]'.
		/// </summary>
		private Expression _primary;

		/// <summary>
		/// Array of predicates in '(e)[p1]...[pn]'.
		/// </summary>
		private readonly ArrayList _predicates;

		public FilterExpr(Expression primary, ArrayList predicates)
		{
		_primary = primary;
		_predicates = predicates;
		primary.Parent = this;
		}

		protected internal virtual Expression Expr
		{
			get
			{
			if (_primary is CastExpr)
			{
				return ((CastExpr)_primary).Expr;
			}
			else
			{
				return _primary;
			}
			}
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_primary.Parser = value;
			if (_predicates != null)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = _predicates.size();
				int n = _predicates.Count;
				for (int i = 0; i < n; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Expression exp = (Expression)_predicates.elementAt(i);
				Expression exp = (Expression)_predicates[i];
				exp.Parser = value;
				exp.Parent = this;
				}
			}
			}
		}

		public override string ToString()
		{
		return "filter-expr(" + _primary + ", " + _predicates + ")";
		}

		/// <summary>
		/// Type check a FilterParentPath. If the filter is not a node-set add a 
		/// cast to node-set only if it is of reference type. This type coercion 
		/// is needed for expressions like $x where $x is a parameter reference.
		/// All optimizations are turned off before type checking underlying
		/// predicates.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		Type ptype = _primary.typeCheck(stable);
			bool canOptimize = _primary is KeyCall;

		if (ptype is NodeSetType == false)
		{
			if (ptype is ReferenceType)
			{
			_primary = new CastExpr(_primary, Type.NodeSet);
			}
			else
			{
			throw new TypeCheckError(this);
			}
		}

			// Type check predicates and turn all optimizations off if appropriate
		int n = _predicates.Count;
		for (int i = 0; i < n; i++)
		{
			Predicate pred = (Predicate) _predicates[i];

				if (!canOptimize)
				{
					pred.dontOptimize();
				}
			pred.typeCheck(stable);
		}
		return _type = Type.NodeSet;
		}

		/// <summary>
		/// Translate a filter expression by pushing the appropriate iterator
		/// onto the stack.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		if (_predicates.Count > 0)
		{
			translatePredicates(classGen, methodGen);
		}
		else
		{
			_primary.translate(classGen, methodGen);
		}
		}

		/// <summary>
		/// Translate a sequence of predicates. Each predicate is translated 
		/// by constructing an instance of <code>CurrentNodeListIterator</code> 
		/// which is initialized from another iterator (recursive call), a 
		/// filter and a closure (call to translate on the predicate) and "this". 
		/// </summary>
		public virtual void translatePredicates(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

			// If not predicates left, translate primary expression
		if (_predicates.Count == 0)
		{
			translate(classGen, methodGen);
		}
		else
		{
				// Remove the next predicate to be translated
				Predicate predicate = (Predicate)_predicates[_predicates.Count - 1];
				_predicates.Remove(predicate);

				// Translate the rest of the predicates from right to left
				translatePredicates(classGen, methodGen);

				if (predicate.NthPositionFilter)
				{
					int nthIteratorIdx = cpg.addMethodref(Constants_Fields.NTH_ITERATOR_CLASS, "<init>", "(" + Constants_Fields.NODE_ITERATOR_SIG + "I)V");

					// Backwards branches are prohibited if an uninitialized object
					// is on the stack by section 4.9.4 of the JVM Specification,
					// 2nd Ed.  We don't know whether this code might contain
					// backwards branches, so we mustn't create the new object unti

					// after we've created the suspect arguments to its constructor

					// Instead we calculate the values of the arguments to the
					// constructor first, store them in temporary variables, create
					// the object and reload the arguments from the temporaries to
					// avoid the problem.
					LocalVariableGen iteratorTemp = methodGen.addLocalVariable("filter_expr_tmp1", Util.getJCRefType(Constants_Fields.NODE_ITERATOR_SIG), null, null);
					iteratorTemp.Start = il.append(new ASTORE(iteratorTemp.Index));

					predicate.translate(classGen, methodGen);
					LocalVariableGen predicateValueTemp = methodGen.addLocalVariable("filter_expr_tmp2", Util.getJCRefType("I"), null, null);
					predicateValueTemp.Start = il.append(new ISTORE(predicateValueTemp.Index));

					il.append(new NEW(cpg.addClass(Constants_Fields.NTH_ITERATOR_CLASS)));
					il.append(DUP);
					iteratorTemp.End = il.append(new ALOAD(iteratorTemp.Index));
					predicateValueTemp.End = il.append(new ILOAD(predicateValueTemp.Index));
					il.append(new INVOKESPECIAL(nthIteratorIdx));
				}
				else
				{
					// Translate predicates from right to left
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initCNLI = cpg.addMethodref(Constants_Fields.CURRENT_NODE_LIST_ITERATOR, "<init>", "("+Constants_Fields.NODE_ITERATOR_SIG+"Z"+ Constants_Fields.CURRENT_NODE_LIST_FILTER_SIG + Constants_Fields.NODE_SIG+Constants_Fields.TRANSLET_SIG+")V");
					int initCNLI = cpg.addMethodref(Constants_Fields.CURRENT_NODE_LIST_ITERATOR, "<init>", "(" + Constants_Fields.NODE_ITERATOR_SIG + "Z" + Constants_Fields.CURRENT_NODE_LIST_FILTER_SIG + Constants_Fields.NODE_SIG + Constants_Fields.TRANSLET_SIG + ")V");

					// Backwards branches are prohibited if an uninitialized object
					// is on the stack by section 4.9.4 of the JVM Specification,
					// 2nd Ed.  We don't know whether this code might contain
					// backwards branches, so we mustn't create the new object
					// until after we've created the suspect arguments to its
					// constructor.  Instead we calculate the values of the
					// arguments to the constructor first, store them in temporary
					// variables, create the object and reload the arguments from
					// the temporaries to avoid the problem.


					LocalVariableGen nodeIteratorTemp = methodGen.addLocalVariable("filter_expr_tmp1", Util.getJCRefType(Constants_Fields.NODE_ITERATOR_SIG), null, null);
					nodeIteratorTemp.Start = il.append(new ASTORE(nodeIteratorTemp.Index));

					predicate.translate(classGen, methodGen);
					LocalVariableGen filterTemp = methodGen.addLocalVariable("filter_expr_tmp2", Util.getJCRefType(Constants_Fields.CURRENT_NODE_LIST_FILTER_SIG), null, null);
					filterTemp.Start = il.append(new ASTORE(filterTemp.Index));

					// Create a CurrentNodeListIterator
					il.append(new NEW(cpg.addClass(Constants_Fields.CURRENT_NODE_LIST_ITERATOR)));
					il.append(DUP);

					// Initialize CurrentNodeListIterator
					nodeIteratorTemp.End = il.append(new ALOAD(nodeIteratorTemp.Index));
					il.append(ICONST_1);
					filterTemp.End = il.append(new ALOAD(filterTemp.Index));
					il.append(methodGen.loadCurrentNode());
					il.append(classGen.loadTranslet());
					il.append(new INVOKESPECIAL(initCNLI));
				}
		}
		}
	}

}