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
 * $Id: Step.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using ICONST = org.apache.bcel.generic.ICONST;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using DOM = org.apache.xalan.xsltc.DOM;
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
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Step : RelativeLocationPath
	{

		/// <summary>
		/// This step's axis as defined in class Axis.
		/// </summary>
		private int _axis;

		/// <summary>
		/// A vector of predicates (filters) defined on this step - may be null
		/// </summary>
		private ArrayList _predicates;

		/// <summary>
		/// Some simple predicates can be handled by this class (and not by the
		/// Predicate class) and will be removed from the above vector as they are
		/// handled. We use this boolean to remember if we did have any predicates.
		/// </summary>
		private bool _hadPredicates = false;

		/// <summary>
		/// Type of the node test.
		/// </summary>
		private int _nodeType;

		public Step(int axis, int nodeType, ArrayList predicates)
		{
		_axis = axis;
		_nodeType = nodeType;
		_predicates = predicates;
		}

		/// <summary>
		/// Set the parser for this element and all child predicates
		/// </summary>
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

		/// <summary>
		/// Define the axis (defined in Axis class) for this step
		/// </summary>
		public override int Axis
		{
			get
			{
			return _axis;
			}
			set
			{
			_axis = value;
			}
		}


		/// <summary>
		/// Returns the node-type for this step
		/// </summary>
		public int NodeType
		{
			get
			{
			return _nodeType;
			}
		}

		/// <summary>
		/// Returns the vector containing all predicates for this step.
		/// </summary>
		public ArrayList Predicates
		{
			get
			{
			return _predicates;
			}
		}

		/// <summary>
		/// Returns the vector containing all predicates for this step.
		/// </summary>
		public void addPredicates(ArrayList predicates)
		{
		if (_predicates == null)
		{
			_predicates = predicates;
		}
		else
		{
			_predicates.AddRange(predicates);
		}
		}

		/// <summary>
		/// Returns 'true' if this step has a parent pattern.
		/// This method will return 'false' if this step occurs on its own under
		/// an element like <xsl:for-each> or <xsl:apply-templates>.
		/// </summary>
		private bool hasParentPattern()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode parent = getParent();
		SyntaxTreeNode parent = Parent;
		return (parent is ParentPattern || parent is ParentLocationPath || parent is UnionPathExpr || parent is FilterParentPath);
		}

		/// <summary>
		/// Returns 'true' if this step has any predicates
		/// </summary>
		private bool hasPredicates()
		{
		return _predicates != null && _predicates.Count > 0;
		}

		/// <summary>
		/// Returns 'true' if this step is used within a predicate
		/// </summary>
		private bool Predicate
		{
			get
			{
			SyntaxTreeNode parent = this;
			while (parent != null)
			{
				parent = parent.Parent;
				if (parent is Predicate)
				{
					return true;
				}
			}
			return false;
			}
		}

		/// <summary>
		/// True if this step is the abbreviated step '.'
		/// </summary>
		public bool AbbreviatedDot
		{
			get
			{
			return _nodeType == NodeTest.ANODE && _axis == Axis.SELF;
			}
		}


		/// <summary>
		/// True if this step is the abbreviated step '..'
		/// </summary>
		public bool AbbreviatedDDot
		{
			get
			{
			return _nodeType == NodeTest.ANODE && _axis == Axis.PARENT;
			}
		}

		/// <summary>
		/// Type check this step. The abbreviated steps '.' and '@attr' are
		/// assigned type node if they have no predicates. All other steps 
		/// have type node-set.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{

		// Save this value for later - important for testing for special
		// combinations of steps and patterns than can be optimised
		_hadPredicates = hasPredicates();

		// Special case for '.'
		 //   in the case where '.' has a context such as book/. 
		//   or .[false()] we can not optimize the nodeset to a single node. 
		if (AbbreviatedDot)
		{
			_type = (hasParentPattern() || hasPredicates()) ? Type.NodeSet : Type.Node;
		}
		else
		{
			_type = Type.NodeSet;
		}

		// Type check all predicates (expressions applied to the step)
		if (_predicates != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _predicates.size();
			int n = _predicates.Count;
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression pred = (Expression)_predicates.elementAt(i);
			Expression pred = (Expression)_predicates[i];
			pred.typeCheck(stable);
			}
		}

		// Return either Type.Node or Type.NodeSet
		return _type;
		}

		/// <summary>
		/// Translate a step by pushing the appropriate iterator onto the stack.
		/// The abbreviated steps '.' and '@attr' do not create new iterators
		/// if they are not part of a LocationPath and have no filters.
		/// In these cases a node index instead of an iterator is pushed
		/// onto the stack.
		/// </summary>
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
			translatePredicates(classGen, methodGen);
		}
		else
		{
				int star = 0;
				string name = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = getParser().getXSLTC();
				XSLTC xsltc = Parser.getXSLTC();

				if (_nodeType >= DTM.NTYPES)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector ni = xsltc.getNamesIndex();
			ArrayList ni = xsltc.NamesIndex;

					name = (string)ni[_nodeType - DTM.NTYPES];
					star = name.LastIndexOf('*');
				}

			// If it is an attribute, but not '@*', '@pre:*' or '@node()',
				// and has no parent
			if (_axis == Axis.ATTRIBUTE && _nodeType != NodeTest.ATTRIBUTE && _nodeType != NodeTest.ANODE && !hasParentPattern() && star == 0)
			{
			int iter = cpg.addInterfaceMethodref(DOM_INTF, "getTypedAxisIterator", "(II)" + NODE_ITERATOR_SIG);
			il.append(methodGen.loadDOM());
			il.append(new PUSH(cpg, Axis.ATTRIBUTE));
			il.append(new PUSH(cpg, _nodeType));
			il.append(new INVOKEINTERFACE(iter, 3));
			return;
			}

			SyntaxTreeNode parent = Parent;
			// Special case for '.'
			if (AbbreviatedDot)
			{
			if (_type == Type.Node)
			{
				// Put context node on stack if using Type.Node
				il.append(methodGen.loadContextNode());
			}
			else
			{
				if (parent is ParentLocationPath)
				{
				// Wrap the context node in a singleton iterator if not.
				int init = cpg.addMethodref(SINGLETON_ITERATOR, "<init>", "(" + NODE_SIG + ")V");
				il.append(new NEW(cpg.addClass(SINGLETON_ITERATOR)));
				il.append(DUP);
				il.append(methodGen.loadContextNode());
				il.append(new INVOKESPECIAL(init));
				}
				else
				{
				// DOM.getAxisIterator(int axis);
				int git = cpg.addInterfaceMethodref(DOM_INTF, "getAxisIterator", "(I)" + NODE_ITERATOR_SIG);
				il.append(methodGen.loadDOM());
				il.append(new PUSH(cpg, _axis));
				il.append(new INVOKEINTERFACE(git, 2));
				}
			}
			return;
			}

			// Special case for /foo/*/bar
			if ((parent is ParentLocationPath) && (parent.Parent is ParentLocationPath))
			{
			if ((_nodeType == NodeTest.ELEMENT) && (!_hadPredicates))
			{
				_nodeType = NodeTest.ANODE;
			}
			}

			// "ELEMENT" or "*" or "@*" or ".." or "@attr" with a parent.
			switch (_nodeType)
			{
			case NodeTest.ATTRIBUTE:
			_axis = Axis.ATTRIBUTE;
				goto case NodeTest.ANODE;
			case NodeTest.ANODE:
			// DOM.getAxisIterator(int axis);
			int git = cpg.addInterfaceMethodref(DOM_INTF, "getAxisIterator", "(I)" + NODE_ITERATOR_SIG);
			il.append(methodGen.loadDOM());
			il.append(new PUSH(cpg, _axis));
			il.append(new INVOKEINTERFACE(git, 2));
			break;
			default:
			if (star > 1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String namespace;
				string @namespace;
				if (_axis == Axis.ATTRIBUTE)
				{
				@namespace = name.Substring(0, star - 2);
				}
				else
				{
				@namespace = name.Substring(0, star - 1);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nsType = xsltc.registerNamespace(namespace);
				int nsType = xsltc.registerNamespace(@namespace);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ns = cpg.addInterfaceMethodref(DOM_INTF, "getNamespaceAxisIterator", "(II)"+NODE_ITERATOR_SIG);
				int ns = cpg.addInterfaceMethodref(DOM_INTF, "getNamespaceAxisIterator", "(II)" + NODE_ITERATOR_SIG);
				il.append(methodGen.loadDOM());
				il.append(new PUSH(cpg, _axis));
				il.append(new PUSH(cpg, nsType));
				il.append(new INVOKEINTERFACE(ns, 3));
				break;
			}
			case NodeTest.ELEMENT:
			// DOM.getTypedAxisIterator(int axis, int type);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ty = cpg.addInterfaceMethodref(DOM_INTF, "getTypedAxisIterator", "(II)"+NODE_ITERATOR_SIG);
			int ty = cpg.addInterfaceMethodref(DOM_INTF, "getTypedAxisIterator", "(II)" + NODE_ITERATOR_SIG);
			// Get the typed iterator we're after
			il.append(methodGen.loadDOM());
			il.append(new PUSH(cpg, _axis));
			il.append(new PUSH(cpg, _nodeType));
			il.append(new INVOKEINTERFACE(ty, 3));

			break;
			}
		}
		}


		/// <summary>
		/// Translate a sequence of predicates. Each predicate is translated
		/// by constructing an instance of <code>CurrentNodeListIterator</code>
		/// which is initialized from another iterator (recursive call),
		/// a filter and a closure (call to translate on the predicate) and "this". 
		/// </summary>
		public void translatePredicates(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		int idx = 0;

		if (_predicates.Count == 0)
		{
			translate(classGen, methodGen);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Predicate predicate = (Predicate)_predicates.lastElement();
			Predicate predicate = (Predicate)_predicates[_predicates.Count - 1];
			_predicates.Remove(predicate);

			// Special case for predicates that can use the NodeValueIterator
			// instead of an auxiliary class. Certain path/predicates pairs
			// are translated into a base path, on top of which we place a
			// node value iterator that tests for the desired value:
			//   foo[@attr = 'str']  ->  foo/@attr + test(value='str')
			//   foo[bar = 'str']    ->  foo/bar + test(value='str')
			//   foo/bar[. = 'str']  ->  foo/bar + test(value='str')
			if (predicate.NodeValueTest)
			{
			Step step = predicate.Step;

			il.append(methodGen.loadDOM());
			// If the predicate's Step is simply '.' we translate this Step
			// and place the node test on top of the resulting iterator
			if (step.AbbreviatedDot)
			{
				translate(classGen, methodGen);
				il.append(new ICONST(DOM.RETURN_CURRENT));
			}
			// Otherwise we create a parent location path with this Step and
			// the predicates Step, and place the node test on top of that
			else
			{
				ParentLocationPath path = new ParentLocationPath(this,step);
				try
				{
				path.typeCheck(Parser.getSymbolTable());
				}
				catch (TypeCheckError)
				{
				}
				path.translate(classGen, methodGen);
				il.append(new ICONST(DOM.RETURN_PARENT));
			}
			predicate.translate(classGen, methodGen);
			idx = cpg.addInterfaceMethodref(DOM_INTF, GET_NODE_VALUE_ITERATOR, GET_NODE_VALUE_ITERATOR_SIG);
			il.append(new INVOKEINTERFACE(idx, 5));
			}
			// Handle '//*[n]' expression
			else if (predicate.NthDescendant)
			{
			il.append(methodGen.loadDOM());
			// il.append(new ICONST(NodeTest.ELEMENT));
			il.append(new ICONST(predicate.PosType));
			predicate.translate(classGen, methodGen);
			il.append(new ICONST(0));
			idx = cpg.addInterfaceMethodref(DOM_INTF, "getNthDescendant", "(IIZ)" + NODE_ITERATOR_SIG);
			il.append(new INVOKEINTERFACE(idx, 4));
			}
			// Handle 'elem[n]' expression
			else if (predicate.NthPositionFilter)
			{
			idx = cpg.addMethodref(NTH_ITERATOR_CLASS, "<init>", "(" + NODE_ITERATOR_SIG + "I)V");

					// Backwards branches are prohibited if an uninitialized object
					// is on the stack by section 4.9.4 of the JVM Specification,
					// 2nd Ed.  We don't know whether this code might contain
					// backwards branches, so we mustn't create the new object until
					// after we've created the suspect arguments to its constructor.
					// Instead we calculate the values of the arguments to the
					// constructor first, store them in temporary variables, create
					// the object and reload the arguments from the temporaries to
					// avoid the problem.
			translatePredicates(classGen, methodGen); // recursive call
					LocalVariableGen iteratorTemp = methodGen.addLocalVariable("step_tmp1", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
					iteratorTemp.setStart(il.append(new ASTORE(iteratorTemp.getIndex())));

			predicate.translate(classGen, methodGen);
					LocalVariableGen predicateValueTemp = methodGen.addLocalVariable("step_tmp2", Util.getJCRefType("I"), null, null);
					predicateValueTemp.setStart(il.append(new ISTORE(predicateValueTemp.getIndex())));

			il.append(new NEW(cpg.addClass(NTH_ITERATOR_CLASS)));
			il.append(DUP);
					iteratorTemp.setEnd(il.append(new ALOAD(iteratorTemp.getIndex())));
					predicateValueTemp.setEnd(il.append(new ILOAD(predicateValueTemp.getIndex())));
			il.append(new INVOKESPECIAL(idx));
			}
			else
			{
			idx = cpg.addMethodref(CURRENT_NODE_LIST_ITERATOR, "<init>", "(" + NODE_ITERATOR_SIG + CURRENT_NODE_LIST_FILTER_SIG + NODE_SIG + TRANSLET_SIG + ")V");

					// Backwards branches are prohibited if an uninitialized object
					// is on the stack by section 4.9.4 of the JVM Specification,
					// 2nd Ed.  We don't know whether this code might contain
					// backwards branches, so we mustn't create the new object until
					// after we've created the suspect arguments to its constructor.
					// Instead we calculate the values of the arguments to the
					// constructor first, store them in temporary variables, create
					// the object and reload the arguments from the temporaries to
					// avoid the problem.
			translatePredicates(classGen, methodGen); // recursive call
					LocalVariableGen iteratorTemp = methodGen.addLocalVariable("step_tmp1", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);
					iteratorTemp.setStart(il.append(new ASTORE(iteratorTemp.getIndex())));

			predicate.translateFilter(classGen, methodGen);
					LocalVariableGen filterTemp = methodGen.addLocalVariable("step_tmp2", Util.getJCRefType(CURRENT_NODE_LIST_FILTER_SIG), null, null);
					filterTemp.setStart(il.append(new ASTORE(filterTemp.getIndex())));

			// create new CurrentNodeListIterator
			il.append(new NEW(cpg.addClass(CURRENT_NODE_LIST_ITERATOR)));
			il.append(DUP);

					iteratorTemp.setEnd(il.append(new ALOAD(iteratorTemp.getIndex())));
					filterTemp.setEnd(il.append(new ALOAD(filterTemp.getIndex())));

			il.append(methodGen.loadCurrentNode());
			il.append(classGen.loadTranslet());
			if (classGen.External)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getClassName();
				string className = classGen.ClassName;
				il.append(new CHECKCAST(cpg.addClass(className)));
			}
			il.append(new INVOKESPECIAL(idx));
			}
		}
		}

		/// <summary>
		/// Returns a string representation of this step.
		/// </summary>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = new StringBuffer("step(\"");
		StringBuilder buffer = new StringBuilder("step(\"");
		buffer.Append(Axis.getNames(_axis)).Append("\", ").Append(_nodeType);
		if (_predicates != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _predicates.size();
			int n = _predicates.Count;
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Predicate pred = (Predicate)_predicates.elementAt(i);
			Predicate pred = (Predicate)_predicates[i];
			buffer.Append(", ").Append(pred.ToString());
			}
		}
		return buffer.Append(')').ToString();
		}
	}

}