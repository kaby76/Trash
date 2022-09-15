using System.Collections;
using System.Text;

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
 * $Id: AttributeValueTemplate.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class AttributeValueTemplate : AttributeValue
	{

		internal const int OUT_EXPR = 0;
		internal const int IN_EXPR = 1;
		internal const int IN_EXPR_SQUOTES = 2;
		internal const int IN_EXPR_DQUOTES = 3;
		internal const string DELIMITER = "\uFFFE"; // A Unicode nonchar

		public AttributeValueTemplate(string value, Parser parser, SyntaxTreeNode parent)
		{
		Parent = parent;
		Parser = parser;

			try
			{
				parseAVTemplate(value, parser);
			}
			catch (NoSuchElementException)
			{
				reportError(parent, parser, ErrorMsg.ATTR_VAL_TEMPLATE_ERR, value);
			}
		}

		/// <summary>
		/// Two-pass parsing of ATVs. In the first pass, double curly braces are 
		/// replaced by one, and expressions are delimited using DELIMITER. The 
		/// second pass splits up the resulting buffer into literal and non-literal
		/// expressions. Errors are reported during the first pass.
		/// </summary>
		private void parseAVTemplate(string text, Parser parser)
		{
			StringTokenizer tokenizer = new StringTokenizer(text, "{}\"\'", true);

			/*
			  * First pass: replace double curly braces and delimit expressions
			  * Simple automaton to parse ATVs, delimit expressions and report
			  * errors.
			  */
			string t = null;
			string lookahead = null;
			StringBuilder buffer = new StringBuilder();
			int state = OUT_EXPR;

			while (tokenizer.hasMoreTokens())
			{
				// Use lookahead if available
				if (!string.ReferenceEquals(lookahead, null))
				{
					t = lookahead;
					lookahead = null;
				}
				else
				{
					t = tokenizer.nextToken();
				}

				if (t.Length == 1)
				{
					switch (t[0])
					{
						case '{':
							switch (state)
							{
								case OUT_EXPR:
									lookahead = tokenizer.nextToken();
									if (lookahead.Equals("{"))
									{
										buffer.Append(lookahead); // replace {{ by {
										lookahead = null;
									}
									else
									{
										buffer.Append(DELIMITER);
										state = IN_EXPR;
									}
									break;
								case IN_EXPR:
								case IN_EXPR_SQUOTES:
								case IN_EXPR_DQUOTES:
									reportError(Parent, parser, ErrorMsg.ATTR_VAL_TEMPLATE_ERR, text);
									break;
							}
							break;
						case '}':
							switch (state)
							{
								case OUT_EXPR:
									lookahead = tokenizer.nextToken();
									if (lookahead.Equals("}"))
									{
										buffer.Append(lookahead); // replace }} by }
										lookahead = null;
									}
									else
									{
										reportError(Parent, parser, ErrorMsg.ATTR_VAL_TEMPLATE_ERR, text);
									}
									break;
								case IN_EXPR:
									buffer.Append(DELIMITER);
									state = OUT_EXPR;
									break;
								case IN_EXPR_SQUOTES:
								case IN_EXPR_DQUOTES:
									buffer.Append(t);
									break;
							}
							break;
						case '\'':
							switch (state)
							{
								case IN_EXPR:
									state = IN_EXPR_SQUOTES;
									break;
								case IN_EXPR_SQUOTES:
									state = IN_EXPR;
									break;
								case OUT_EXPR:
								case IN_EXPR_DQUOTES:
									break;
							}
							buffer.Append(t);
							break;
						case '\"':
							switch (state)
							{
								case IN_EXPR:
									state = IN_EXPR_DQUOTES;
									break;
								case IN_EXPR_DQUOTES:
									state = IN_EXPR;
									break;
								case OUT_EXPR:
								case IN_EXPR_SQUOTES:
									break;
							}
							buffer.Append(t);
							break;
						default:
							buffer.Append(t);
							break;
					}
				}
				else
				{
					buffer.Append(t);
				}
			}

			// Must be in OUT_EXPR at the end of parsing
			if (state != OUT_EXPR)
			{
				reportError(Parent, parser, ErrorMsg.ATTR_VAL_TEMPLATE_ERR, text);
			}

			/*
			  * Second pass: split up buffer into literal and non-literal expressions.
			  */
			tokenizer = new StringTokenizer(buffer.ToString(), DELIMITER, true);

			while (tokenizer.hasMoreTokens())
			{
				t = tokenizer.nextToken();

				if (t.Equals(DELIMITER))
				{
			addElement(parser.parseExpression(this, tokenizer.nextToken()));
					tokenizer.nextToken(); // consume other delimiter
				}
				else
				{
			addElement(new LiteralExpr(t));
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector contents = getContents();
		ArrayList contents = Contents;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = contents.size();
		int n = contents.Count;
		for (int i = 0; i < n; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = (Expression)contents.elementAt(i);
			Expression exp = (Expression)contents[i];
			if (!exp.typeCheck(stable).identicalTo(Type.String))
			{
			contents[i] = new CastExpr(exp, Type.String);
			}
		}
		return _type = Type.String;
		}

		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = new StringBuffer("AVT:[");
		StringBuilder buffer = new StringBuilder("AVT:[");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = elementCount();
		int count = elementCount();
		for (int i = 0; i < count; i++)
		{
			buffer.Append(elementAt(i).ToString());
			if (i < count - 1)
			{
			buffer.Append(' ');
			}
		}
		return buffer.Append(']').ToString();
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		if (elementCount() == 1)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = (Expression)elementAt(0);
			Expression exp = (Expression)elementAt(0);
			exp.translate(classGen, methodGen);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initBuffer = cpg.addMethodref(STRING_BUFFER_CLASS, "<init>", "()V");
			int initBuffer = cpg.addMethodref(STRING_BUFFER_CLASS, "<init>", "()V");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Instruction append = new org.apache.bcel.generic.INVOKEVIRTUAL(cpg.addMethodref(STRING_BUFFER_CLASS, "append", "(" + STRING_SIG + ")" + STRING_BUFFER_SIG));
			Instruction append = new INVOKEVIRTUAL(cpg.addMethodref(STRING_BUFFER_CLASS, "append", "(" + STRING_SIG + ")" + STRING_BUFFER_SIG));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int toString = cpg.addMethodref(STRING_BUFFER_CLASS, "toString", "()"+STRING_SIG);
			int toString = cpg.addMethodref(STRING_BUFFER_CLASS, "toString", "()" + STRING_SIG);
			il.append(new NEW(cpg.addClass(STRING_BUFFER_CLASS)));
			il.append(DUP);
			il.append(new INVOKESPECIAL(initBuffer));
			// StringBuffer is on the stack
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration elements = elements();
			System.Collections.IEnumerator elements = this.elements();
			while (elements.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = (Expression)elements.Current;
			Expression exp = (Expression)elements.Current;
			exp.translate(classGen, methodGen);
			il.append(append);
			}
			il.append(new INVOKEVIRTUAL(toString));
		}
		}
	}

}