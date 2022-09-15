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
 * $Id: ForEach.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFGT = org.apache.bcel.generic.IFGT;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using ResultTreeType = org.apache.xalan.xsltc.compiler.util.ResultTreeType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class ForEach : Instruction
	{

		private Expression _select;
		private Type _type;

		public override void display(int indent)
		{
		indent(indent);
		Util.println("ForEach");
		indent(indent + IndentIncrement);
		Util.println("select " + _select.ToString());
		displayContents(indent + IndentIncrement);
		}

		public override void parseContents(Parser parser)
		{
		_select = parser.parseExpression(this, "select", null);

		parseChildren(parser);

			// make sure required attribute(s) have been set
			if (_select.Dummy)
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "select");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		_type = _select.typeCheck(stable);

		if (_type is ReferenceType || _type is NodeType)
		{
			_select = new CastExpr(_select, Type.NodeSet);
			typeCheckContents(stable);
			return Type.Void;
		}
		if (_type is NodeSetType || _type is ResultTreeType)
		{
			typeCheckContents(stable);
			return Type.Void;
		}
		throw new TypeCheckError(this);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// Save current node and current iterator on the stack
		il.append(methodGen.loadCurrentNode());
		il.append(methodGen.loadIterator());

		// Collect sort objects associated with this instruction
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector sortObjects = new java.util.Vector();
		ArrayList sortObjects = new ArrayList();
		System.Collections.IEnumerator children = elements();
		while (children.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object child = children.Current;
			object child = children.Current;
			if (child is Sort)
			{
			sortObjects.Add(child);
			}
		}

		if ((_type != null) && (_type is ResultTreeType))
		{
			// Store existing DOM on stack - must be restored when loop is done
			il.append(methodGen.loadDOM());

			// <xsl:sort> cannot be applied to a result tree - issue warning
			if (sortObjects.Count > 0)
			{
			ErrorMsg msg = new ErrorMsg(ErrorMsg.RESULT_TREE_SORT_ERR,this);
			Parser.reportError(Constants_Fields.WARNING, msg);
			}

			// Put the result tree on the stack (DOM)
			_select.translate(classGen, methodGen);
			// Get an iterator for the whole DOM - excluding the root node
			_type.translateTo(classGen, methodGen, Type.NodeSet);
			// Store the result tree as the default DOM
			il.append(SWAP);
			il.append(methodGen.storeDOM());
		}
		else
		{
			// Compile node iterator
			if (sortObjects.Count > 0)
			{
			Sort.translateSortIterator(classGen, methodGen, _select, sortObjects);
			}
			else
			{
			_select.translate(classGen, methodGen);
			}

			if (_type is ReferenceType == false)
			{
					il.append(methodGen.loadContextNode());
					il.append(methodGen.setStartNode());
			}
		}


		// Overwrite current iterator
		il.append(methodGen.storeIterator());

		// Give local variables (if any) default values before starting loop
		initializeVariables(classGen, methodGen);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle nextNode = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle nextNode = il.append(new GOTO(null));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle loop = il.append(NOP);
		InstructionHandle loop = il.append(NOP);

		translateContents(classGen, methodGen);

		nextNode.Target = il.append(methodGen.loadIterator());
		il.append(methodGen.nextNode());
		il.append(DUP);
		il.append(methodGen.storeCurrentNode());
		il.append(new IFGT(loop));

		// Restore current DOM (if result tree was used instead for this loop)
		if ((_type != null) && (_type is ResultTreeType))
		{
			il.append(methodGen.storeDOM());
		}

		// Restore current node and current iterator from the stack
		il.append(methodGen.storeIterator());
		il.append(methodGen.storeCurrentNode());
		}

		/// <summary>
		/// The code that is generated by nested for-each loops can appear to some
		/// JVMs as if it is accessing un-initialized variables. We must add some
		/// code that pushes the default variable value on the stack and pops it
		/// into the variable slot. This is done by the Variable.initialize()
		/// method. The code that we compile for this loop looks like this:
		/// 
		///           initialize iterator
		///           initialize variables <-- HERE!!!
		///           goto   Iterate
		///  Loop:    :
		///           : (code for <xsl:for-each> contents)
		///           :
		///  Iterate: node = iterator.next();
		///           if (node != END) goto Loop
		/// </summary>
		public void initializeVariables(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = elementCount();
		int n = elementCount();
		for (int i = 0; i < n; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object child = getContents().elementAt(i);
			object child = Contents[i];
			if (child is Variable)
			{
			Variable @var = (Variable)child;
			@var.initialize(classGen, methodGen);
			}
		}
		}

	}

}