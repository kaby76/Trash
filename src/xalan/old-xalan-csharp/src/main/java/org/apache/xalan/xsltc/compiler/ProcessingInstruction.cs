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
 * $Id: ProcessingInstruction.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class ProcessingInstruction : Instruction
	{

		private AttributeValue _name; // name treated as AVT (7.1.3)
		private bool _isLiteral = false; // specified name is not AVT

		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getAttribute("name");
		string name = getAttribute("name");

			if (name.Length > 0)
			{
				_isLiteral = Util.isLiteral(name);
				if (_isLiteral)
				{
					if (!XML11Char.isXML11ValidNCName(name))
					{
						ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_NCNAME_ERR, name, this);
						parser.reportError(Constants_Fields.ERROR, err);
					}
				}
				_name = AttributeValue.create(this, name, parser);
			}
			else
			{
				reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "name");
			}

		if (name.Equals("xml"))
		{
			reportError(this, parser, ErrorMsg.ILLEGAL_PI_ERR, "xml");
		}
		parseChildren(parser);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		_name.typeCheck(stable);
		typeCheckContents(stable);
		return Type.Void;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

			if (!_isLiteral)
			{
				// if the ncname is an AVT, then the ncname has to be checked at runtime if it is a valid ncname
				LocalVariableGen nameValue = methodGen.addLocalVariable2("nameValue", Util.getJCRefType(Constants_Fields.STRING_SIG), null);

				// store the name into a variable first so _name.translate only needs to be called once  
				_name.translate(classGen, methodGen);
				nameValue.Start = il.append(new ASTORE(nameValue.Index));
				il.append(new ALOAD(nameValue.Index));

				// call checkNCName if the name is an AVT
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int check = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "checkNCName", "(" +Constants_Fields.STRING_SIG +")V");
				int check = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "checkNCName", "(" + Constants_Fields.STRING_SIG + ")V");
									il.append(new INVOKESTATIC(check));

				// Save the current handler base on the stack
				il.append(methodGen.loadHandler());
				il.append(DUP); // first arg to "attributes" call

				// load name value again    
				nameValue.End = il.append(new ALOAD(nameValue.Index));
			}
			else
			{
				// Save the current handler base on the stack
				il.append(methodGen.loadHandler());
				il.append(DUP); // first arg to "attributes" call

				// Push attribute name
				_name.translate(classGen, methodGen); // 2nd arg

			}

		il.append(classGen.loadTranslet());
		il.append(new GETFIELD(cpg.addFieldref(Constants_Fields.TRANSLET_CLASS, "stringValueHandler", Constants_Fields.STRING_VALUE_HANDLER_SIG)));
		il.append(DUP);
		il.append(methodGen.storeHandler());

		// translate contents with substituted handler
		translateContents(classGen, methodGen);

		// get String out of the handler
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(Constants_Fields.STRING_VALUE_HANDLER, "getValueOfPI", "()" + Constants_Fields.STRING_SIG)));
		// call "processingInstruction"
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int processingInstruction = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "processingInstruction", "(" + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + ")V");
		int processingInstruction = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "processingInstruction", "(" + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + ")V");
		il.append(new INVOKEINTERFACE(processingInstruction, 3));
		// Restore old handler base from stack
		il.append(methodGen.storeHandler());
		}
	}

}