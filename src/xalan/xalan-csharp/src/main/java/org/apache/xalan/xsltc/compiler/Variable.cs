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
 * $Id: Variable.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Field = org.apache.bcel.classfile.Field;
	using ACONST_NULL = org.apache.bcel.generic.ACONST_NULL;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using DCONST = org.apache.bcel.generic.DCONST;
	using ICONST = org.apache.bcel.generic.ICONST;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using IntType = org.apache.xalan.xsltc.compiler.util.IntType;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using RealType = org.apache.xalan.xsltc.compiler.util.RealType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	internal sealed class Variable : VariableBase
	{

		public int Index
		{
			get
			{
			return (_local != null) ? _local.Index : -1;
			}
		}

		/// <summary>
		/// Parse the contents of the variable
		/// </summary>
		public override void parseContents(Parser parser)
		{
		// Parse 'name' and 'select' attributes plus parameter contents
		base.parseContents(parser);

		// Add a ref to this var to its enclosing construct
		SyntaxTreeNode parent = Parent;
		if (parent is Stylesheet)
		{
			// Mark this as a global variable
			_isLocal = false;
			// Check if a global variable with this name already exists...
			Variable @var = parser.SymbolTable.lookupVariable(_name);
			// ...and if it does we need to check import precedence
			if (@var != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int us = this.getImportPrecedence();
			int us = this.ImportPrecedence;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int them = var.getImportPrecedence();
			int them = @var.ImportPrecedence;
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
				@var.disable();
			}
			// Add this variable if we have higher precedence
			}
			((Stylesheet)parent).addVariable(this);
			parser.SymbolTable.addVariable(this);
		}
		else
		{
			_isLocal = true;
		}
		}

		/// <summary>
		/// Runs a type check on either the variable element body or the
		/// expression in the 'select' attribute
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{

		// Type check the 'select' expression if present
		if (_select != null)
		{
			_type = _select.typeCheck(stable);
		}
		// Type check the element contents otherwise
		else if (hasContents())
		{
			typeCheckContents(stable);
			_type = Type.ResultTree;
		}
		else
		{
			_type = Type.Reference;
		}
		// The return type is void as the variable element does not leave
		// anything on the JVM's stack. The '_type' global will be returned
		// by the references to this variable, and not by the variable itself.
		return Type.Void;
		}

		/// <summary>
		/// This method is part of a little trick that is needed to use local
		/// variables inside nested for-each loops. See the initializeVariables()
		/// method in the ForEach class for an explanation
		/// </summary>
		public void initialize(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// This is only done for local variables that are actually used
		if (Local && _refs.Count > 0)
		{
			// Create a variable slot if none is allocated
			if (_local == null)
			{
			_local = methodGen.addLocalVariable2(EscapedName, _type.toJCType(), null);
			}
			// Push the default value on the JVM's stack
			if ((_type is IntType) || (_type is NodeType) || (_type is BooleanType))
			{
			il.append(new ICONST(0)); // 0 for node-id, integer and boolean
			}
			else if (_type is RealType)
			{
			il.append(new DCONST(0)); // 0.0 for floating point numbers
			}
			else
			{
			il.append(new ACONST_NULL()); // and 'null' for anything else
			}

				// Mark the store as the start of the live range of the variable
				_local.Start = il.append(_type.STORE(_local.Index));
		}
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

			// Don't generate code for unreferenced variables
			if (_refs.Count == 0)
			{
				_ignore = true;
			}

		// Make sure that a variable instance is only compiled once
		if (_ignore)
		{
			return;
		}
		_ignore = true;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getEscapedName();
		string name = EscapedName;

		if (Local)
		{
			// Compile variable value computation
			translateValue(classGen, methodGen);

			// Add a new local variable and store value
				bool createLocal = _local == null;
			if (createLocal)
			{
					mapRegister(methodGen);
			}
			InstructionHandle storeInst = il.append(_type.STORE(_local.Index));

				// If the local is just being created, mark the store as the start
				// of its live range.  Note that it might have been created by
				// initializeVariables already, which would have set the start of
				// the live range already.
				if (createLocal)
				{
					_local.Start = storeInst;
				}
		}
		else
		{
			string signature = _type.toSignature();

			// Global variables are store in class fields
			if (classGen.containsField(name) == null)
			{
			classGen.addField(new Field(Constants_Fields.ACC_PUBLIC, cpg.addUtf8(name), cpg.addUtf8(signature), null, cpg.ConstantPool));

			// Push a reference to "this" for putfield
			il.append(classGen.loadTranslet());
			// Compile variable value computation
			translateValue(classGen, methodGen);
			// Store the variable in the allocated field
			il.append(new PUTFIELD(cpg.addFieldref(classGen.ClassName, name, signature)));
			}
		}
		}
	}

}