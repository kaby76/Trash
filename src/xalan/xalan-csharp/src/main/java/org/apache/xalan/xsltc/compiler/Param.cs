using System;

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
 * $Id: Param.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using Field = org.apache.bcel.classfile.Field;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using IFNONNULL = org.apache.bcel.generic.IFNONNULL;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using ObjectType = org.apache.xalan.xsltc.compiler.util.ObjectType;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author John Howard <JohnH@schemasoft.com>
	/// </summary>
	internal sealed class Param : VariableBase
	{

		/// <summary>
		/// True if this Param is declared in a simple named template.
		/// This is used to optimize codegen for parameter passing
		/// in named templates.
		/// </summary>
		private bool _isInSimpleNamedTemplate = false;

		/// <summary>
		/// Display variable as single string
		/// </summary>
		public override string ToString()
		{
		return "param(" + _name + ")";
		}

		/// <summary>
		/// Set the instruction for loading the value of this variable onto the 
		/// JVM stack and returns the old instruction.
		/// </summary>
		public Instruction setLoadInstruction(Instruction instruction)
		{
			Instruction tmp = _loadInstruction;
			_loadInstruction = instruction;
			return tmp;
		}

		/// <summary>
		/// Set the instruction for storing a value from the stack into this
		/// variable and returns the old instruction.
		/// </summary>
		public Instruction setStoreInstruction(Instruction instruction)
		{
			Instruction tmp = _storeInstruction;
			_storeInstruction = instruction;
			return tmp;
		}

		/// <summary>
		/// Display variable in a full AST dump
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Console.WriteLine("param " + _name);
		if (_select != null)
		{
			this.indent(indent + IndentIncrement);
			Console.WriteLine("select " + _select.ToString());
		}
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Parse the contents of the <xsl:param> element. This method must read
		/// the 'name' (required) and 'select' (optional) attributes.
		/// </summary>
		public override void parseContents(Parser parser)
		{

		// Parse 'name' and 'select' attributes plus parameter contents
		base.parseContents(parser);

		// Add a ref to this param to its enclosing construct
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode parent = getParent();
		SyntaxTreeNode parent = Parent;
		if (parent is Stylesheet)
		{
			// Mark this as a global parameter
			_isLocal = false;
			// Check if a global variable with this name already exists...
			Param param = parser.SymbolTable.lookupParam(_name);
			// ...and if it does we need to check import precedence
			if (param != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int us = this.getImportPrecedence();
			int us = this.ImportPrecedence;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int them = param.getImportPrecedence();
			int them = param.ImportPrecedence;
			// It is an error if the two have the same import precedence
			if (us == them)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _name.toString();
				string name = _name.ToString();
				reportError(this, parser, ErrorMsg.VARIABLE_REDEF_ERR,name);
			}
			// Ignore this if previous definition has higher precedence
			else if (them > us)
			{
				_ignore = true;
				return;
			}
			else
			{
				param.disable();
			}
			}
			// Add this variable if we have higher precedence
			((Stylesheet)parent).addParam(this);
			parser.SymbolTable.addParam(this);
		}
		else if (parent is Template)
		{
				Template template = (Template) parent;
			_isLocal = true;
				template.addParameter(this);
				if (template.SimpleNamedTemplate)
				{
					_isInSimpleNamedTemplate = true;
				}
		}
		}

		/// <summary>
		/// Type-checks the parameter. The parameter type is determined by the
		/// 'select' expression (if present) or is a result tree if the parameter
		/// element has a body and no 'select' expression.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_select != null)
		{
			_type = _select.typeCheck(stable);
			if (_type is ReferenceType == false && !(_type is ObjectType))
			{
			_select = new CastExpr(_select, Type.Reference);
			}
		}
		else if (hasContents())
		{
			typeCheckContents(stable);
		}
		_type = Type.Reference;

		// This element has no type (the parameter does, but the parameter
		// element itself does not).
		return Type.Void;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (_ignore)
		{
			return;
		}
		_ignore = true;

			/*
			 * To fix bug 24518 related to setting parameters of the form
			 * {namespaceuri}localName which will get mapped to an instance 
			 * variable in the class.
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = org.apache.xalan.xsltc.runtime.BasisLibrary.mapQNameToJavaName(_name.toString());
		string name = BasisLibrary.mapQNameToJavaName(_name.ToString());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String signature = _type.toSignature();
		string signature = _type.toSignature();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = _type.getClassName();
		string className = _type.ClassName;

		if (Local)
		{
				/*
				  * If simple named template then generate a conditional init of the 
				  * param using its default value: 
				  *       if (param == null) param = <default-value>
				  */
				if (_isInSimpleNamedTemplate)
				{
			il.append(loadInstruction());
					BranchHandle ifBlock = il.append(new IFNONNULL(null));
					translateValue(classGen, methodGen);
					il.append(storeInstruction());
					ifBlock.setTarget(il.append(NOP));
					return;
				}

			il.append(classGen.loadTranslet());
			il.append(new PUSH(cpg, name));
			translateValue(classGen, methodGen);
			il.append(new PUSH(cpg, true));

			// Call addParameter() from this class
			il.append(new INVOKEVIRTUAL(cpg.addMethodref(TRANSLET_CLASS, ADD_PARAMETER, ADD_PARAMETER_SIG)));
			if (!string.ReferenceEquals(className, EMPTYSTRING))
			{
			il.append(new CHECKCAST(cpg.addClass(className)));
			}

			_type.translateUnBox(classGen, methodGen);

			if (_refs.Count == 0)
			{ // nobody uses the value
			il.append(_type.POP());
			_local = null;
			}
			else
			{ // normal case
			_local = methodGen.addLocalVariable2(name, _type.toJCType(), null);
			// Cache the result of addParameter() in a local variable
			_local.setStart(il.append(_type.STORE(_local.getIndex())));
			}
		}
		else
		{
			if (classGen.containsField(name) == null)
			{
			classGen.addField(new Field(ACC_PUBLIC, cpg.addUtf8(name), cpg.addUtf8(signature), null, cpg.getConstantPool()));
			il.append(classGen.loadTranslet());
			il.append(DUP);
			il.append(new PUSH(cpg, name));
			translateValue(classGen, methodGen);
			il.append(new PUSH(cpg, true));

			// Call addParameter() from this class
			il.append(new INVOKEVIRTUAL(cpg.addMethodref(TRANSLET_CLASS, ADD_PARAMETER, ADD_PARAMETER_SIG)));

			_type.translateUnBox(classGen, methodGen);

			// Cache the result of addParameter() in a field
			if (!string.ReferenceEquals(className, EMPTYSTRING))
			{
				il.append(new CHECKCAST(cpg.addClass(className)));
			}
			il.append(new PUTFIELD(cpg.addFieldref(classGen.ClassName, name, signature)));
			}
		}
		}
	}

}