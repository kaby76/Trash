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
 * $Id: TransletOutput.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using StringType = org.apache.xalan.xsltc.compiler.util.StringType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class TransletOutput : Instruction
	{

		private Expression _filename;
		private bool _append;

		/// <summary>
		/// Displays the contents of this <xsltc:output> element.
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("TransletOutput: " + _filename);
		}

		/// <summary>
		/// Parse the contents of this <xsltc:output> element. The only attribute
		/// we recognise is the 'file' attribute that contains teh output filename.
		/// </summary>
		public override void parseContents(Parser parser)
		{
		// Get the output filename from the 'file' attribute
		string filename = getAttribute("file");

			// If the 'append' attribute is set to "yes" or "true",
			// the output is appended to the file.
			string append = getAttribute("append");

		// Verify that the filename is in fact set
		if ((string.ReferenceEquals(filename, null)) || (filename.Equals(EMPTYSTRING)))
		{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "file");
		}

		// Save filename as an attribute value template
		_filename = AttributeValue.create(this, filename, parser);

			if (!string.ReferenceEquals(append, null) && (append.ToLower().Equals("yes") || append.ToLower().Equals("true")))
			{
			  _append = true;
			}
			else
			{
			  _append = false;
			}

		parseChildren(parser);
		}

		/// <summary>
		/// Type checks the 'file' attribute (must be able to convert it to a str).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type type = _filename.typeCheck(stable);
		Type type = _filename.typeCheck(stable);
		if (type is StringType == false)
		{
			_filename = new CastExpr(_filename, Type.String);
		}
		typeCheckContents(stable);
		return Type.Void;
		}

		/// <summary>
		/// Compile code that opens the give file for output, dumps the contents of
		/// the element to the file, then closes the file.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isSecureProcessing = classGen.getParser().getXSLTC().isSecureProcessing();
		bool isSecureProcessing = classGen.Parser.XSLTC.isSecureProcessing();

		if (isSecureProcessing)
		{
			int index = cpg.addMethodref(BASIS_LIBRARY_CLASS, "unallowed_extension_elementF", "(Ljava/lang/String;)V");
			il.append(new PUSH(cpg, "redirect"));
			il.append(new INVOKESTATIC(index));
			return;
		}

		// Save the current output handler on the stack
		il.append(methodGen.loadHandler());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int open = cpg.addMethodref(TRANSLET_CLASS, "openOutputHandler", "(" + STRING_SIG + "Z)" + TRANSLET_OUTPUT_SIG);
		int open = cpg.addMethodref(TRANSLET_CLASS, "openOutputHandler", "(" + STRING_SIG + "Z)" + TRANSLET_OUTPUT_SIG);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int close = cpg.addMethodref(TRANSLET_CLASS, "closeOutputHandler", "("+TRANSLET_OUTPUT_SIG+")V");
		int close = cpg.addMethodref(TRANSLET_CLASS, "closeOutputHandler", "(" + TRANSLET_OUTPUT_SIG + ")V");

		// Create the new output handler (leave it on stack)
		il.append(classGen.loadTranslet());
		_filename.translate(classGen, methodGen);
			il.append(new PUSH(cpg, _append));
		il.append(new INVOKEVIRTUAL(open));

		// Overwrite current handler
		il.append(methodGen.storeHandler());

		// Translate contents with substituted handler
		translateContents(classGen, methodGen);

		// Close the output handler (close file)
		il.append(classGen.loadTranslet());
		il.append(methodGen.loadHandler());
		il.append(new INVOKEVIRTUAL(close));

		// Restore old output handler from stack
		il.append(methodGen.storeHandler());
		}
	}


}