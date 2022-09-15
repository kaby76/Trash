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
 * $Id: Key.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IFGE = org.apache.bcel.generic.IFGE;
	using IFGT = org.apache.bcel.generic.IFGT;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using StringType = org.apache.xalan.xsltc.compiler.util.StringType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Axis = org.apache.xml.dtm.Axis;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Morten Jorgensen
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class Key : TopLevelElement
	{

		/// <summary>
		/// The name of this key as defined in xsl:key.
		/// </summary>
		private QName _name;

		/// <summary>
		/// The pattern to match starting at the root node.
		/// </summary>
		private Pattern _match;

		/// <summary>
		/// The expression that generates the values for this key.
		/// </summary>
		private Expression _use;

		/// <summary>
		/// The type of the _use expression.
		/// </summary>
		private Type _useType;

		/// <summary>
		/// Parse the <xsl:key> element and attributes </summary>
		/// <param name="parser"> A reference to the stylesheet parser </param>
		public override void parseContents(Parser parser)
		{

		// Get the required attributes and parser XPath expressions
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getAttribute("name");
			string name = getAttribute("name");
			if (!XML11Char.isXML11ValidQName(name))
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, name, this);
				parser.reportError(Constants_Fields.ERROR, err);
			}

			// Parse key name and add to symbol table
			_name = parser.getQNameIgnoreDefaultNs(name);
			SymbolTable.addKey(_name, this);

		_match = parser.parsePattern(this, "match", null);
		_use = parser.parseExpression(this, "use", null);

			// Make sure required attribute(s) have been set
			if (_name == null)
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "name");
			return;
			}
			if (_match.Dummy)
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "match");
			return;
			}
			if (_use.Dummy)
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "use");
			return;
			}
		}

		/// <summary>
		/// Returns a String-representation of this key's name </summary>
		/// <returns> The key's name (from the <xsl:key> elements 'name' attribute). </returns>
		public string Name
		{
			get
			{
			return _name.ToString();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		// Type check match pattern
		_match.typeCheck(stable);

		// Cast node values to string values (except for nodesets)
		_useType = _use.typeCheck(stable);
		if (_useType is StringType == false && _useType is NodeSetType == false)
		{
			_use = new CastExpr(_use, Type.String);
		}

		return Type.Void;
		}

		/// <summary>
		/// This method is called if the "use" attribute of the key contains a
		/// node set. In this case we must traverse all nodes in the set and
		/// create one entry in this key's index for each node in the set.
		/// </summary>
		public void traverseNodeSet(ClassGenerator classGen, MethodGenerator methodGen, int buildKeyIndex)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// DOM.getStringValueX(nodeIndex) => String
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getNodeValue = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_NODE_VALUE, "(I)"+Constants_Fields.STRING_SIG);
		int getNodeValue = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_NODE_VALUE, "(I)" + Constants_Fields.STRING_SIG);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getNodeIdent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getNodeIdent", "(I)"+Constants_Fields.NODE_SIG);
		int getNodeIdent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getNodeIdent", "(I)" + Constants_Fields.NODE_SIG);

		// AbstractTranslet.SetKeyIndexDom(name, Dom) => void
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int keyDom = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "setKeyIndexDom", "("+Constants_Fields.STRING_SIG+Constants_Fields.DOM_INTF_SIG+")V");
		int keyDom = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "setKeyIndexDom", "(" + Constants_Fields.STRING_SIG + Constants_Fields.DOM_INTF_SIG + ")V");


		// This variable holds the id of the node we found with the "match"
		// attribute of xsl:key. This is the id we store, with the value we
		// get from the nodes we find here, in the index for this key.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen parentNode = methodGen.addLocalVariable("parentNode", org.apache.xalan.xsltc.compiler.util.Util.getJCRefType("I"), null, null);
		LocalVariableGen parentNode = methodGen.addLocalVariable("parentNode", Util.getJCRefType("I"), null, null);

		// Get the 'parameter' from the stack and store it in a local var.
		parentNode.Start = il.append(new ISTORE(parentNode.Index));

		// Save current node and current iterator on the stack
		il.append(methodGen.loadCurrentNode());
		il.append(methodGen.loadIterator());

		// Overwrite current iterator with one that gives us only what we want
		_use.translate(classGen, methodGen);
		_use.startIterator(classGen, methodGen);
		il.append(methodGen.storeIterator());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle nextNode = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle nextNode = il.append(new GOTO(null));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle loop = il.append(NOP);
		InstructionHandle loop = il.append(NOP);

		// Prepare to call buildKeyIndex(String name, int node, String value);
		il.append(classGen.loadTranslet());
		il.append(new PUSH(cpg, _name.ToString()));
		parentNode.End = il.append(new ILOAD(parentNode.Index));

		// Now get the node value and push it on the parameter stack
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadCurrentNode());
		il.append(new INVOKEINTERFACE(getNodeValue, 2));

		// Finally do the call to add an entry in the index for this key.
		il.append(new INVOKEVIRTUAL(buildKeyIndex));

		il.append(classGen.loadTranslet());
		il.append(new PUSH(cpg, Name));
		il.append(methodGen.loadDOM());
		il.append(new INVOKEVIRTUAL(keyDom));

		nextNode.Target = il.append(methodGen.loadIterator());
		il.append(methodGen.nextNode());

		il.append(DUP);
		il.append(methodGen.storeCurrentNode());
		il.append(new IFGE(loop)); // Go on to next matching node....

		// Restore current node and current iterator from the stack
		il.append(methodGen.storeIterator());
		il.append(methodGen.storeCurrentNode());
		}

		/// <summary>
		/// Gather all nodes that match the expression in the attribute "match"
		/// and add one (or more) entries in this key's index.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int current = methodGen.getLocalIndex("current");
		int current = methodGen.getLocalIndex("current");

		// AbstractTranslet.buildKeyIndex(name,node_id,value) => void
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int key = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "buildKeyIndex", "("+Constants_Fields.STRING_SIG+"I"+Constants_Fields.OBJECT_SIG+")V");
		int key = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "buildKeyIndex", "(" + Constants_Fields.STRING_SIG + "I" + Constants_Fields.OBJECT_SIG + ")V");

		// AbstractTranslet.SetKeyIndexDom(name, Dom) => void
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int keyDom = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "setKeyIndexDom", "("+Constants_Fields.STRING_SIG+Constants_Fields.DOM_INTF_SIG+")V");
		int keyDom = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "setKeyIndexDom", "(" + Constants_Fields.STRING_SIG + Constants_Fields.DOM_INTF_SIG + ")V");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getNodeIdent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getNodeIdent", "(I)"+Constants_Fields.NODE_SIG);
		int getNodeIdent = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getNodeIdent", "(I)" + Constants_Fields.NODE_SIG);

		// DOM.getAxisIterator(root) => NodeIterator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int git = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getAxisIterator", "(I)"+Constants_Fields.NODE_ITERATOR_SIG);
		int git = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getAxisIterator", "(I)" + Constants_Fields.NODE_ITERATOR_SIG);

		il.append(methodGen.loadCurrentNode());
		il.append(methodGen.loadIterator());

		// Get an iterator for all nodes in the DOM
		il.append(methodGen.loadDOM());
		il.append(new PUSH(cpg,Axis.DESCENDANT));
		il.append(new INVOKEINTERFACE(git, 2));

		// Reset the iterator to start with the root node
		il.append(methodGen.loadCurrentNode());
		il.append(methodGen.setStartNode());
		il.append(methodGen.storeIterator());

		// Loop for traversing all nodes in the DOM
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle nextNode = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle nextNode = il.append(new GOTO(null));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle loop = il.append(NOP);
		InstructionHandle loop = il.append(NOP);

		// Check if the current node matches the pattern in "match"
		il.append(methodGen.loadCurrentNode());
		_match.translate(classGen, methodGen);
		_match.synthesize(classGen, methodGen); // Leaves 0 or 1 on stack
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle skipNode = il.append(new org.apache.bcel.generic.IFEQ(null));
		BranchHandle skipNode = il.append(new IFEQ(null));

		// If this is a node-set we must go through each node in the set
		if (_useType is NodeSetType)
		{
			// Pass current node as parameter (we're indexing on that node)
			il.append(methodGen.loadCurrentNode());
			traverseNodeSet(classGen, methodGen, key);
		}
		else
		{
			il.append(classGen.loadTranslet());
			il.append(DUP);
			il.append(new PUSH(cpg, _name.ToString()));
			il.append(DUP_X1);
			il.append(methodGen.loadCurrentNode());
			_use.translate(classGen, methodGen);
			il.append(new INVOKEVIRTUAL(key));

			il.append(methodGen.loadDOM());
			il.append(new INVOKEVIRTUAL(keyDom));
		}

		// Get the next node from the iterator and do loop again...
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle skip = il.append(NOP);
		InstructionHandle skip = il.append(NOP);

		il.append(methodGen.loadIterator());
		il.append(methodGen.nextNode());
		il.append(DUP);
		il.append(methodGen.storeCurrentNode());
		il.append(new IFGT(loop));

		// Restore current node and current iterator from the stack
		il.append(methodGen.storeIterator());
		il.append(methodGen.storeCurrentNode());

		nextNode.Target = skip;
		skipNode.Target = skip;
		}
	}

}