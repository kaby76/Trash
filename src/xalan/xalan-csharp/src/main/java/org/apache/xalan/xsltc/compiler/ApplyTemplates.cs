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
 * $Id: ApplyTemplates.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
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
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class ApplyTemplates : Instruction
	{
		private Expression _select;
		private Type _type = null;
		private QName _modeName;
		private string _functionName;

		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("ApplyTemplates");
		this.indent(indent + IndentIncrement);
		Util.println("select " + _select.ToString());
		if (_modeName != null)
		{
			this.indent(indent + IndentIncrement);
			Util.println("mode " + _modeName);
		}
		}

		public bool hasWithParams()
		{
		return hasContents();
		}

		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String select = getAttribute("select");
		string select = getAttribute("select");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String mode = getAttribute("mode");
		string mode = getAttribute("mode");

		if (select.Length > 0)
		{
			_select = parser.parseExpression(this, "select", null);

		}

		if (mode.Length > 0)
		{
				if (!XML11Char.isXML11ValidQName(mode))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, mode, this);
					parser.reportError(Constants.ERROR, err);
				}
			_modeName = parser.getQNameIgnoreDefaultNs(mode);
		}

		// instantiate Mode if needed, cache (apply temp) function name
		_functionName = parser.TopLevelStylesheet.getMode(_modeName).functionName();
		parseChildren(parser); // with-params
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_select != null)
		{
			_type = _select.typeCheck(stable);
			if (_type is NodeType || _type is ReferenceType)
			{
			_select = new CastExpr(_select, Type.NodeSet);
			_type = Type.NodeSet;
			}
			if (_type is NodeSetType || _type is ResultTreeType)
			{
			typeCheckContents(stable); // with-params
			return Type.Void;
			}
			throw new TypeCheckError(this);
		}
		else
		{
			typeCheckContents(stable); // with-params
			return Type.Void;
		}
		}

		/// <summary>
		/// Translate call-template. A parameter frame is pushed only if
		/// some template in the stylesheet uses parameters. 
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		bool setStartNodeCalled = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stylesheet stylesheet = classGen.getStylesheet();
		Stylesheet stylesheet = classGen.Stylesheet;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int current = methodGen.getLocalIndex("current");
		int current = methodGen.getLocalIndex("current");

		// check if sorting nodes is required
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector sortObjects = new java.util.Vector();
		ArrayList sortObjects = new ArrayList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration children = elements();
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

		// Push a new parameter frame
		if (stylesheet.hasLocalParams() || hasContents())
		{
			il.append(classGen.loadTranslet());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pushFrame = cpg.addMethodref(TRANSLET_CLASS, PUSH_PARAM_FRAME, PUSH_PARAM_FRAME_SIG);
			int pushFrame = cpg.addMethodref(TRANSLET_CLASS, PUSH_PARAM_FRAME, PUSH_PARAM_FRAME_SIG);
			il.append(new INVOKEVIRTUAL(pushFrame));
			// translate with-params
			translateContents(classGen, methodGen);
		}


		il.append(classGen.loadTranslet());

		// The 'select' expression is a result-tree
		if ((_type != null) && (_type is ResultTreeType))
		{
			// <xsl:sort> cannot be applied to a result tree - issue warning
			if (sortObjects.Count > 0)
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.RESULT_TREE_SORT_ERR,this);
			Parser.reportError(WARNING, err);
			}
			// Put the result tree (a DOM adapter) on the stack
			_select.translate(classGen, methodGen);
			// Get back the DOM and iterator (not just iterator!!!)
			_type.translateTo(classGen, methodGen, Type.NodeSet);
		}
		else
		{
			il.append(methodGen.loadDOM());

			// compute node iterator for applyTemplates
			if (sortObjects.Count > 0)
			{
			Sort.translateSortIterator(classGen, methodGen, _select, sortObjects);
			int setStartNode = cpg.addInterfaceMethodref(NODE_ITERATOR, SET_START_NODE, "(I)" + NODE_ITERATOR_SIG);
			il.append(methodGen.loadCurrentNode());
			il.append(new INVOKEINTERFACE(setStartNode,2));
			setStartNodeCalled = true;
			}
			else
			{
			if (_select == null)
			{
				Mode.compileGetChildren(classGen, methodGen, current);
			}
			else
			{
				_select.translate(classGen, methodGen);
			}
			}
		}

		if (_select != null && !setStartNodeCalled)
		{
			_select.startIterator(classGen, methodGen);
		}

		//!!! need to instantiate all needed modes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getStylesheet().getClassName();
		string className = classGen.Stylesheet.ClassName;
		il.append(methodGen.loadHandler());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String applyTemplatesSig = classGen.getApplyTemplatesSig();
		string applyTemplatesSig = classGen.ApplyTemplatesSig;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int applyTemplates = cpg.addMethodref(className, _functionName, applyTemplatesSig);
		int applyTemplates = cpg.addMethodref(className, _functionName, applyTemplatesSig);
		il.append(new INVOKEVIRTUAL(applyTemplates));

		// Pop parameter frame
		if (stylesheet.hasLocalParams() || hasContents())
		{
			il.append(classGen.loadTranslet());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int popFrame = cpg.addMethodref(TRANSLET_CLASS, POP_PARAM_FRAME, POP_PARAM_FRAME_SIG);
			int popFrame = cpg.addMethodref(TRANSLET_CLASS, POP_PARAM_FRAME, POP_PARAM_FRAME_SIG);
			il.append(new INVOKEVIRTUAL(popFrame));
		}
		}
	}

}