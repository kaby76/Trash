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
 * $Id: ValueOf.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class ValueOf : Instruction
	{
		private Expression _select;
		private bool _escaping = true;
		private bool _isString = false;

		public override void display(int indent)
		{
			this.indent(indent);
			Util.println("ValueOf");
			this.indent(indent + IndentIncrement);
			Util.println("select " + _select.ToString());
		}

		public override void parseContents(Parser parser)
		{
			_select = parser.parseExpression(this, "select", null);

			// make sure required attribute(s) have been set
			if (_select.Dummy)
			{
				reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "select");
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String str = getAttribute("disable-output-escaping");
			string str = getAttribute("disable-output-escaping");
			if ((!string.ReferenceEquals(str, null)) && (str.Equals("yes")))
			{
				_escaping = false;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
			Type type = _select.typeCheck(stable);

			// Prefer to handle the value as a node; fall back to String, otherwise
			if (type != null && !type.identicalTo(Type.Node))
			{
				/// <summary>
				///*
				/// *** %HZ% Would like to treat result-tree fragments in the same
				/// *** %HZ% way as node sets for value-of, but that's running into
				/// *** %HZ% some snags.  Instead, they'll be converted to String
				/// if (type.identicalTo(Type.ResultTree)) {
				///    _select = new CastExpr(new CastExpr(_select, Type.NodeSet),
				///                           Type.Node);
				/// } else
				/// **
				/// </summary>
				if (type.identicalTo(Type.NodeSet))
				{
					_select = new CastExpr(_select, Type.Node);
				}
				else
				{
					_isString = true;
					if (!type.identicalTo(Type.String))
					{
						_select = new CastExpr(_select, Type.String);
					}
					_isString = true;
				}
			}
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int setEscaping = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "setEscaping","(Z)Z");
			int setEscaping = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "setEscaping","(Z)Z");

			// Turn off character escaping if so is wanted.
			if (!_escaping)
			{
				il.append(methodGen.loadHandler());
				il.append(new PUSH(cpg,false));
				il.append(new INVOKEINTERFACE(setEscaping,2));
			}

			// Translate the contents.  If the value is a string, use the
			// translet.characters(String, TranslatOutputHandler) method.
			// Otherwise, the value is a node, and the
			// dom.characters(int node, TransletOutputHandler) method can dispatch
			// the string value of the node to the output handler more efficiently.
			if (_isString)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int characters = cpg.addMethodref(TRANSLET_CLASS, CHARACTERSW, CHARACTERSW_SIG);
				int characters = cpg.addMethodref(TRANSLET_CLASS, CHARACTERSW, CHARACTERSW_SIG);

				il.append(classGen.loadTranslet());
				_select.translate(classGen, methodGen);
				il.append(methodGen.loadHandler());
				il.append(new INVOKEVIRTUAL(characters));
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int characters = cpg.addInterfaceMethodref(DOM_INTF, CHARACTERS, CHARACTERS_SIG);
				int characters = cpg.addInterfaceMethodref(DOM_INTF, CHARACTERS, CHARACTERS_SIG);

				il.append(methodGen.loadDOM());
				_select.translate(classGen, methodGen);
				il.append(methodGen.loadHandler());
				il.append(new INVOKEINTERFACE(characters, 3));
			}

			// Restore character escaping setting to whatever it was.
			if (!_escaping)
			{
				il.append(methodGen.loadHandler());
				il.append(SWAP);
				il.append(new INVOKEINTERFACE(setEscaping,2));
				il.append(POP);
			}
		}
	}

}