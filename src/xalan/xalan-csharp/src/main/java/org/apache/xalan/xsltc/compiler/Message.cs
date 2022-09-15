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
 * $Id: Message.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Message : Instruction
	{
		private bool _terminate = false;

		public override void parseContents(Parser parser)
		{
		string termstr = getAttribute("terminate");
		if (!string.ReferenceEquals(termstr, null))
		{
				_terminate = termstr.Equals("yes");
		}
		parseChildren(parser);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		typeCheckContents(stable);
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

		// Load the translet (for call to displayMessage() function)
		il.append(classGen.loadTranslet());

			switch (elementCount())
			{
				case 0:
					il.append(new PUSH(cpg, ""));
				break;
				case 1:
					SyntaxTreeNode child = (SyntaxTreeNode) elementAt(0);
					if (child is Text)
					{
						il.append(new PUSH(cpg, ((Text) child).Text));
						break;
					}
					// falls through
				default:
					// Push current output handler onto the stack
					il.append(methodGen.loadHandler());

					// Replace the current output handler by a ToXMLStream
					il.append(new NEW(cpg.addClass(STREAM_XML_OUTPUT)));
					il.append(methodGen.storeHandler());

					// Push a reference to a StringWriter
					il.append(new NEW(cpg.addClass(STRING_WRITER)));
					il.append(DUP);
					il.append(DUP);
					il.append(new INVOKESPECIAL(cpg.addMethodref(STRING_WRITER, "<init>", "()V")));

					// Load ToXMLStream
					il.append(methodGen.loadHandler());
					il.append(new INVOKESPECIAL(cpg.addMethodref(STREAM_XML_OUTPUT, "<init>", "()V")));

					// Invoke output.setWriter(STRING_WRITER)
					il.append(methodGen.loadHandler());
					il.append(SWAP);
					il.append(new INVOKEINTERFACE(cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "setWriter", "(" + WRITER_SIG + ")V"), 2));

					// Invoke output.setEncoding("UTF-8")
					il.append(methodGen.loadHandler());
					il.append(new PUSH(cpg, "UTF-8")); // other encodings?
					il.append(new INVOKEINTERFACE(cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "setEncoding", "(" + STRING_SIG + ")V"), 2));

					// Invoke output.setOmitXMLDeclaration(true)
					il.append(methodGen.loadHandler());
					il.append(ICONST_1);
					il.append(new INVOKEINTERFACE(cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "setOmitXMLDeclaration", "(Z)V"), 2));

					il.append(methodGen.loadHandler());
					il.append(new INVOKEINTERFACE(cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "startDocument", "()V"), 1));

					// Inline translation of contents
					translateContents(classGen, methodGen);

					il.append(methodGen.loadHandler());
					il.append(new INVOKEINTERFACE(cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "endDocument", "()V"), 1));

					// Call toString() on StringWriter
					il.append(new INVOKEVIRTUAL(cpg.addMethodref(STRING_WRITER, "toString", "()" + STRING_SIG)));

					// Restore old output handler
					il.append(SWAP);
					il.append(methodGen.storeHandler());
				break;
			}

		// Send the resulting string to the message handling method
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(TRANSLET_CLASS, "displayMessage", "(" + STRING_SIG + ")V")));

		// If 'terminate' attribute is set to 'yes': Instanciate a
		// RunTimeException, but it on the stack and throw an exception
		if (_terminate == true)
		{
			// Create a new instance of RunTimeException
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int einit = cpg.addMethodref("java.lang.RuntimeException", "<init>", "(Ljava/lang/String;)V");
			int einit = cpg.addMethodref("java.lang.RuntimeException", "<init>", "(Ljava/lang/String;)V");
			il.append(new NEW(cpg.addClass("java.lang.RuntimeException")));
			il.append(DUP);
			il.append(new PUSH(cpg,"Termination forced by an " + "xsl:message instruction"));
			il.append(new INVOKESPECIAL(einit));
			il.append(ATHROW);
		}
		}

	}

}