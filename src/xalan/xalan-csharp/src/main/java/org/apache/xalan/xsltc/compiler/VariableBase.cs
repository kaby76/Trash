using System;
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
 * $Id: VariableBase.java 528589 2007-04-13 18:50:56Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author John Howard <JohnH@schemasoft.com>
	/// </summary>
	internal class VariableBase : TopLevelElement
	{

		protected internal QName _name; // The name of the variable.
		protected internal string _escapedName; // The escaped qname of the variable.
		protected internal Type _type; // The type of this variable.
		protected internal bool _isLocal; // True if the variable is local.
		protected internal LocalVariableGen _local; // Reference to JVM variable
		protected internal Instruction _loadInstruction; // Instruction to load JVM variable
		protected internal Instruction _storeInstruction; // Instruction to load JVM variable
		protected internal Expression _select; // Reference to variable expression
		protected internal string select; // Textual repr. of variable expr.

		// References to this variable (when local)
		protected internal ArrayList _refs = new ArrayList(2);

		// Dependencies to other variables/parameters (for globals only)
		protected internal new ArrayList _dependencies = null;

		// Used to make sure parameter field is not added twice
		protected internal bool _ignore = false;

		/// <summary>
		/// Disable this variable/parameter
		/// </summary>
		public virtual void disable()
		{
		_ignore = true;
		}

		/// <summary>
		/// Add a reference to this variable. Called by VariableRef when an
		/// expression contains a reference to this variable.
		/// </summary>
		public virtual void addReference(VariableRefBase vref)
		{
		_refs.Add(vref);
		}

		/// <summary>
		/// Map this variable to a register
		/// </summary>
		public virtual void mapRegister(MethodGenerator methodGen)
		{
			if (_local == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getEscapedName();
			string name = EscapedName; // TODO: namespace ?
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type varType = _type.toJCType();
			org.apache.bcel.generic.Type varType = _type.toJCType();
				_local = methodGen.addLocalVariable2(name, varType, null);
			}
		}

		/// <summary>
		/// Remove the mapping of this variable to a register.
		/// Called when we leave the AST scope of the variable's declaration
		/// </summary>
		public virtual void unmapRegister(MethodGenerator methodGen)
		{
		if (_local != null)
		{
			_local.setEnd(methodGen.getInstructionList().getEnd());
			methodGen.removeLocalVariable(_local);
			_refs = null;
			_local = null;
		}
		}

		/// <summary>
		/// Returns an instruction for loading the value of this variable onto 
		/// the JVM stack.
		/// </summary>
		public virtual Instruction loadInstruction()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction instr = _loadInstruction;
		Instruction instr = _loadInstruction;
		if (_loadInstruction == null)
		{
			_loadInstruction = _type.LOAD(_local.getIndex());
		}
		return _loadInstruction;
		}

		/// <summary>
		/// Returns an instruction for storing a value from the JVM stack
		/// into this variable.
		/// </summary>
		public virtual Instruction storeInstruction()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction instr = _storeInstruction;
		Instruction instr = _storeInstruction;
		if (_storeInstruction == null)
		{
			_storeInstruction = _type.STORE(_local.getIndex());
		}
		return _storeInstruction;
		}

		/// <summary>
		/// Returns the expression from this variable's select attribute (if any)
		/// </summary>
		public virtual Expression Expression
		{
			get
			{
			return (_select);
			}
		}

		/// <summary>
		/// Display variable as single string
		/// </summary>
		public override string ToString()
		{
		return ("variable(" + _name + ")");
		}

		/// <summary>
		/// Display variable in a full AST dump
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Console.WriteLine("Variable " + _name);
		if (_select != null)
		{
			this.indent(indent + IndentIncrement);
			Console.WriteLine("select " + _select.ToString());
		}
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Returns the type of the variable
		/// </summary>
		public virtual Type Type
		{
			get
			{
			return _type;
			}
		}

		/// <summary>
		/// Returns the name of the variable or parameter as it will occur in the
		/// compiled translet.
		/// </summary>
		public virtual QName Name
		{
			get
			{
			return _name;
			}
			set
			{
			_name = value;
			_escapedName = Util.escape(value.StringRep);
			}
		}

		/// <summary>
		/// Returns the escaped qname of the variable or parameter 
		/// </summary>
		public virtual string EscapedName
		{
			get
			{
			return _escapedName;
			}
		}


		/// <summary>
		/// Returns the true if the variable is local
		/// </summary>
		public virtual bool Local
		{
			get
			{
			return _isLocal;
			}
		}

		/// <summary>
		/// Parse the contents of the <xsl:decimal-format> element.
		/// </summary>
		public override void parseContents(Parser parser)
		{
		// Get the 'name attribute
		string name = getAttribute("name");

			if (name.Length > 0)
			{
				if (!XML11Char.isXML11ValidQName(name))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, name, this);
					parser.reportError(Constants.ERROR, err);
				}
			Name = parser.getQNameIgnoreDefaultNs(name);
			}
			else
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "name");
			}

		// Check whether variable/param of the same name is already in scope
		VariableBase other = parser.lookupVariable(_name);
		if ((other != null) && (other.Parent == Parent))
		{
			reportError(this, parser, ErrorMsg.VARIABLE_REDEF_ERR, name);
		}

		select = getAttribute("select");
		if (select.Length > 0)
		{
			_select = Parser.parseExpression(this, "select", null);
			if (_select.Dummy)
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "select");
			return;
			}
		}

		// Children must be parsed first -> static scoping
		parseChildren(parser);
		}

		/// <summary>
		/// Compile the value of the variable, which is either in an expression in
		/// a 'select' attribute, or in the variable elements body
		/// </summary>
		public virtual void translateValue(ClassGenerator classGen, MethodGenerator methodGen)
		{
		// Compile expression is 'select' attribute if present
		if (_select != null)
		{
			_select.translate(classGen, methodGen);
			// Create a CachedNodeListIterator for select expressions
			// in a variable or parameter.
			if (_select.Type is NodeSetType)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
				ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
				InstructionList il = methodGen.getInstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initCNI = cpg.addMethodref(CACHED_NODE_LIST_ITERATOR_CLASS, "<init>", "(" +NODE_ITERATOR_SIG +")V");
				int initCNI = cpg.addMethodref(CACHED_NODE_LIST_ITERATOR_CLASS, "<init>", "(" + NODE_ITERATOR_SIG + ")V");
				il.append(new NEW(cpg.addClass(CACHED_NODE_LIST_ITERATOR_CLASS)));
				il.append(DUP_X1);
				il.append(SWAP);

				il.append(new INVOKESPECIAL(initCNI));
			}
			_select.startIterator(classGen, methodGen);
		}
		// If not, compile result tree from parameter body if present.
		else if (hasContents())
		{
			compileResultTree(classGen, methodGen);
		}
		// If neither are present then store empty string in variable
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();
			il.append(new PUSH(cpg, Constants.EMPTYSTRING));
		}
		}

	}

}