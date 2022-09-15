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
 * $Id: ParentLocationPath.java 468650 2006-10-28 07:03:30Z minchau $
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
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class ParentLocationPath : RelativeLocationPath
	{
		private Expression _step;
		private readonly RelativeLocationPath _path;
		private Type stype;
		private bool _orderNodes = false;
		private bool _axisMismatch = false;

		public ParentLocationPath(RelativeLocationPath path, Expression step)
		{
		_path = path;
		_step = step;
		_path.Parent = this;
		_step.Parent = this;

		if (_step is Step)
		{
			_axisMismatch = checkAxisMismatch();
		}
		}

		public override int Axis
		{
			set
			{
			_path.Axis = value;
			}
			get
			{
			return _path.Axis;
			}
		}


		public RelativeLocationPath Path
		{
			get
			{
			return (_path);
			}
		}

		public Expression Step
		{
			get
			{
			return (_step);
			}
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_step.Parser = value;
			_path.Parser = value;
			}
		}

		public override string ToString()
		{
		return "ParentLocationPath(" + _path + ", " + _step + ')';
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		stype = _step.typeCheck(stable);
		_path.typeCheck(stable);

		if (_axisMismatch)
		{
			enableNodeOrdering();
		}

		return _type = Type.NodeSet;
		}

		public void enableNodeOrdering()
		{
		SyntaxTreeNode parent = Parent;
		if (parent is ParentLocationPath)
		{
			((ParentLocationPath)parent).enableNodeOrdering();
		}
		else
		{
			_orderNodes = true;
		}
		}

		/// <summary>
		/// This method is used to determine if this parent location path is a
		/// combination of two step's with axes that will create duplicate or
		/// unordered nodes.
		/// </summary>
		public bool checkAxisMismatch()
		{

		int left = _path.Axis;
		int right = ((Step)_step).Axis;

		if (((left == Axis.ANCESTOR) || (left == Axis.ANCESTORORSELF)) && ((right == Axis.CHILD) || (right == Axis.DESCENDANT) || (right == Axis.DESCENDANTORSELF) || (right == Axis.PARENT) || (right == Axis.PRECEDING) || (right == Axis.PRECEDINGSIBLING)))
		{
			return true;
		}

		if ((left == Axis.CHILD) && (right == Axis.ANCESTOR) || (right == Axis.ANCESTORORSELF) || (right == Axis.PARENT) || (right == Axis.PRECEDING))
		{
			return true;
		}

		if ((left == Axis.DESCENDANT) || (left == Axis.DESCENDANTORSELF))
		{
			return true;
		}

		if (((left == Axis.FOLLOWING) || (left == Axis.FOLLOWINGSIBLING)) && ((right == Axis.FOLLOWING) || (right == Axis.PARENT) || (right == Axis.PRECEDING) || (right == Axis.PRECEDINGSIBLING)))
		{
			return true;
		}

		if (((left == Axis.PRECEDING) || (left == Axis.PRECEDINGSIBLING)) && ((right == Axis.DESCENDANT) || (right == Axis.DESCENDANTORSELF) || (right == Axis.FOLLOWING) || (right == Axis.FOLLOWINGSIBLING) || (right == Axis.PARENT) || (right == Axis.PRECEDING) || (right == Axis.PRECEDINGSIBLING)))
		{
			return true;
		}

		if ((right == Axis.FOLLOWING) && (left == Axis.CHILD))
		{
			// Special case for '@*/following::*' expressions. The resulting
			// iterator is initialised with the parent's first child, and this
			// can cause duplicates in the output if the parent has more than
			// one attribute that matches the left step.
			if (_path is Step)
			{
			int type = ((Step)_path).NodeType;
			if (type == DTM.ATTRIBUTE_NODE)
			{
				return true;
			}
			}
		}

		return false;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

			// Backwards branches are prohibited if an uninitialized object is
			// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
			// We don't know whether this code might contain backwards branches
			// so we mustn't create the new object until after we've created
			// the suspect arguments to its constructor.  Instead we calculate
			// the values of the arguments to the constructor first, store them
			// in temporary variables, create the object and reload the
			// arguments from the temporaries to avoid the problem.

		// Compile path iterator
		_path.translate(classGen, methodGen); // iterator on stack....
			LocalVariableGen pathTemp = methodGen.addLocalVariable("parent_location_path_tmp1", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
			pathTemp.setStart(il.append(new ASTORE(pathTemp.getIndex())));

		_step.translate(classGen, methodGen);
			LocalVariableGen stepTemp = methodGen.addLocalVariable("parent_location_path_tmp2", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
			stepTemp.setStart(il.append(new ASTORE(stepTemp.getIndex())));

		// Create new StepIterator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initSI = cpg.addMethodref(STEP_ITERATOR_CLASS, "<init>", "(" +NODE_ITERATOR_SIG +NODE_ITERATOR_SIG +")V");
		int initSI = cpg.addMethodref(STEP_ITERATOR_CLASS, "<init>", "(" + NODE_ITERATOR_SIG + NODE_ITERATOR_SIG + ")V");
		il.append(new NEW(cpg.addClass(STEP_ITERATOR_CLASS)));
		il.append(DUP);

			pathTemp.setEnd(il.append(new ALOAD(pathTemp.getIndex())));
			stepTemp.setEnd(il.append(new ALOAD(stepTemp.getIndex())));

		// Initialize StepIterator with iterators from the stack
		il.append(new INVOKESPECIAL(initSI));

		// This is a special case for the //* path with or without predicates
		Expression stp = _step;
		if (stp is ParentLocationPath)
		{
			stp = ((ParentLocationPath)stp).Step;
		}

		if ((_path is Step) && (stp is Step))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int path = ((Step)_path).getAxis();
			int path = ((Step)_path).Axis;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int step = ((Step)stp).getAxis();
			int step = ((Step)stp).Axis;
			if ((path == Axis.DESCENDANTORSELF && step == Axis.CHILD) || (path == Axis.PRECEDING && step == Axis.PARENT))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int incl = cpg.addMethodref(NODE_ITERATOR_BASE, "includeSelf", "()" + NODE_ITERATOR_SIG);
			int incl = cpg.addMethodref(NODE_ITERATOR_BASE, "includeSelf", "()" + NODE_ITERATOR_SIG);
			il.append(new INVOKEVIRTUAL(incl));
			}
		}

		/*
		 * If this pattern contains a sequence of descendant iterators we
		 * run the risk of returning the same node several times. We put
		 * a new iterator on top of the existing one to assure node order
		 * and prevent returning a single node multiple times.
		 */
		if (_orderNodes)
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