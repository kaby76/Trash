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
 * $Id: UnionPathExpr.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class UnionPathExpr : Expression
	{

		private readonly Expression _pathExpr;
		private readonly Expression _rest;
		private bool _reverse = false;

		// linearization for top level UnionPathExprs
		private Expression[] _components;

		public UnionPathExpr(Expression pathExpr, Expression rest)
		{
		_pathExpr = pathExpr;
		_rest = rest;
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			// find all expressions in this Union
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.Vector components = new java.util.Vector();
			ArrayList components = new ArrayList();
			flatten(components);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int size = components.size();
			int size = components.Count;
			_components = (Expression[])components.ToArray(typeof(Expression));
			for (int i = 0; i < size; i++)
			{
				_components[i].Parser = value;
				_components[i].Parent = this;
				if (_components[i] is Step)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Step step = (Step)_components[i];
				Step step = (Step)_components[i];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int axis = step.getAxis();
				int axis = step.Axis;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int type = step.getNodeType();
				int type = step.NodeType;
				// Put attribute iterators first
				if ((axis == Axis.ATTRIBUTE) || (type == DTM.ATTRIBUTE_NODE))
				{
					_components[i] = _components[0];
					_components[0] = step;
				}
				// Check if the union contains a reverse iterator
				if (Axis.isReverse(axis))
				{
					_reverse = true;
				}
				}
			}
			// No need to reverse anything if another expression lies on top of this
			if (Parent is Expression)
			{
				_reverse = false;
			}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = _components.length;
		int length = _components.Length;
		for (int i = 0; i < length; i++)
		{
			if (_components[i].typeCheck(stable) != Type.NodeSet)
			{
			_components[i] = new CastExpr(_components[i], Type.NodeSet);
			}
		}
		return _type = Type.NodeSet;
		}

		public override string ToString()
		{
		return "union(" + _pathExpr + ", " + _rest + ')';
		}

		private void flatten(ArrayList components)
		{
		components.Add(_pathExpr);
		if (_rest != null)
		{
			if (_rest is UnionPathExpr)
			{
			((UnionPathExpr)_rest).flatten(components);
			}
			else
			{
			components.Add(_rest);
			}
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
//ORIGINAL LINE: final int init = cpg.addMethodref(UNION_ITERATOR_CLASS, "<init>", "("+DOM_INTF_SIG+")V");
		int init = cpg.addMethodref(UNION_ITERATOR_CLASS, "<init>", "(" + DOM_INTF_SIG + ")V");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iter = cpg.addMethodref(UNION_ITERATOR_CLASS, ADD_ITERATOR, ADD_ITERATOR_SIG);
		int iter = cpg.addMethodref(UNION_ITERATOR_CLASS, ADD_ITERATOR, ADD_ITERATOR_SIG);

		// Create the UnionIterator and leave it on the stack
		il.append(new NEW(cpg.addClass(UNION_ITERATOR_CLASS)));
		il.append(DUP);
		il.append(methodGen.loadDOM());
		il.append(new INVOKESPECIAL(init));

		// Add the various iterators to the UnionIterator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = _components.length;
		int length = _components.Length;
		for (int i = 0; i < length; i++)
		{
			_components[i].translate(classGen, methodGen);
			il.append(new INVOKEVIRTUAL(iter));
		}

		// Order the iterator only if strictly needed
		if (_reverse)
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