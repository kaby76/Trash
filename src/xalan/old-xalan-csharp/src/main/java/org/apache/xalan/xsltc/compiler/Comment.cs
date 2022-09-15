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
 * $Id: Comment.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using PUSH = org.apache.bcel.generic.PUSH;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Comment : Instruction
	{

		public override void parseContents(Parser parser)
		{
		parseChildren(parser);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		typeCheckContents(stable);
		return Type.String;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

			// Shortcut for literal strings
			Text rawText = null;
			if (elementCount() == 1)
			{
				object content = elementAt(0);
				if (content is Text)
				{
					rawText = (Text) content;
				}
			}

			// If the content is literal text, call comment(char[],int,int) or
			// comment(String), as appropriate.  Otherwise, use a
			// StringValueHandler to gather the textual content of the xsl:comment
			// and call comment(String) with the result.
			if (rawText != null)
			{
				il.append(methodGen.loadHandler());

				if (rawText.canLoadAsArrayOffsetLength())
				{
					rawText.loadAsArrayOffsetLength(classGen, methodGen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int comment = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "comment", "([CII)V");
					int comment = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "comment", "([CII)V");
					il.append(new INVOKEINTERFACE(comment, 4));
				}
				else
				{
					il.append(new PUSH(cpg, rawText.getText()));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int comment = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "comment", "(" + Constants_Fields.STRING_SIG + ")V");
					int comment = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "comment", "(" + Constants_Fields.STRING_SIG + ")V");
					il.append(new INVOKEINTERFACE(comment, 2));
				}
			}
			else
			{
				// Save the current handler base on the stack
				il.append(methodGen.loadHandler());
				il.append(DUP); // first arg to "comment" call

				// Get the translet's StringValueHandler
				il.append(classGen.loadTranslet());
				il.append(new GETFIELD(cpg.addFieldref(Constants_Fields.TRANSLET_CLASS, "stringValueHandler", Constants_Fields.STRING_VALUE_HANDLER_SIG)));
				il.append(DUP);
				il.append(methodGen.storeHandler());

				// translate contents with substituted handler
				translateContents(classGen, methodGen);

				// get String out of the handler
				il.append(new INVOKEVIRTUAL(cpg.addMethodref(Constants_Fields.STRING_VALUE_HANDLER, "getValue", "()" + Constants_Fields.STRING_SIG)));
				// call "comment"
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int comment = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "comment", "(" + Constants_Fields.STRING_SIG + ")V");
				int comment = cpg.addInterfaceMethodref(Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "comment", "(" + Constants_Fields.STRING_SIG + ")V");
				il.append(new INVOKEINTERFACE(comment, 2));
				// Restore old handler base from stack
				il.append(methodGen.storeHandler());
			}
		}
	}

}