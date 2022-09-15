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
 * $Id: Text.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETSTATIC = org.apache.bcel.generic.GETSTATIC;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Text : Instruction
	{

		private string _text;
		private bool _escaping = true;
		private bool _ignore = false;
		private bool _textElement = false;

		/// <summary>
		/// Create a blank Text syntax tree node.
		/// </summary>
		public Text()
		{
		_textElement = true;
		}

		/// <summary>
		/// Create text syntax tree node. </summary>
		/// <param name="text"> is the text to put in the node. </param>
		public Text(string text)
		{
		_text = text;
		}

		/// <summary>
		/// Returns the text wrapped inside this node </summary>
		/// <returns> The text wrapped inside this node </returns>
		protected internal string Text
		{
			get
			{
			return _text;
			}
			set
			{
			if (string.ReferenceEquals(_text, null))
			{
				_text = value;
			}
			else
			{
				_text = _text + value;
			}
			}
		}


		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("Text");
		this.indent(indent + IndentIncrement);
		Util.println(_text);
		}

		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String str = getAttribute("disable-output-escaping");
			string str = getAttribute("disable-output-escaping");
		if ((!string.ReferenceEquals(str, null)) && (str.Equals("yes")))
		{
			_escaping = false;
		}

		parseChildren(parser);

		if (string.ReferenceEquals(_text, null))
		{
			if (_textElement)
			{
			_text = EMPTYSTRING;
			}
			else
			{
			_ignore = true;
			}
		}
		else if (_textElement)
		{
			if (_text.Length == 0)
			{
				_ignore = true;
			}
		}
		else if (Parent is LiteralElement)
		{
			LiteralElement element = (LiteralElement)Parent;
			string space = element.getAttribute("xml:space");
			if ((string.ReferenceEquals(space, null)) || (!space.Equals("preserve")))
			{
				int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int textLength = _text.length();
				int textLength = _text.Length;
				for (i = 0; i < textLength; i++)
				{
					char c = _text[i];
					if (!isWhitespace(c))
					{
						break;
					}
				}
				if (i == textLength)
				{
					_ignore = true;
				}
			}
		}
		else
		{
			int i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int textLength = _text.length();
			int textLength = _text.Length;
			for (i = 0; i < textLength; i++)
			{
				char c = _text[i];
				if (!isWhitespace(c))
				{
					break;
				}
			}
			if (i == textLength)
			{
				_ignore = true;
			}
		}
		}

		public void ignore()
		{
		_ignore = true;
		}

		public bool Ignore
		{
			get
			{
				return _ignore;
			}
		}

		public bool TextElement
		{
			get
			{
			return _textElement;
			}
		}

		protected internal override bool contextDependent()
		{
		return false;
		}

		private static bool isWhitespace(char c)
		{
			return (c == (char)0x20 || c == (char)0x09 || c == (char)0x0A || c == (char)0x0D);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (!_ignore)
		{
			// Turn off character escaping if so is wanted.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int esc = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "setEscaping", "(Z)Z");
			int esc = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "setEscaping", "(Z)Z");
			if (!_escaping)
			{
			il.append(methodGen.loadHandler());
			il.append(new PUSH(cpg, false));
			il.append(new INVOKEINTERFACE(esc, 2));
			}

				il.append(methodGen.loadHandler());

				// Call characters(String) or characters(char[],int,int), as
				// appropriate.
				if (!canLoadAsArrayOffsetLength())
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int characters = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "characters", "("+STRING_SIG+")V");
					int characters = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "characters", "(" + STRING_SIG + ")V");
					il.append(new PUSH(cpg, _text));
					il.append(new INVOKEINTERFACE(characters, 2));
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int characters = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "characters", "([CII)V");
					int characters = cpg.addInterfaceMethodref(OUTPUT_HANDLER, "characters", "([CII)V");
					loadAsArrayOffsetLength(classGen, methodGen);
				il.append(new INVOKEINTERFACE(characters, 4));
				}

			// Restore character escaping setting to whatever it was.
			// Note: setEscaping(bool) returns the original (old) value
			if (!_escaping)
			{
			il.append(methodGen.loadHandler());
			il.append(SWAP);
			il.append(new INVOKEINTERFACE(esc, 2));
			il.append(POP);
			}
		}
		translateContents(classGen, methodGen);
		}

		/// <summary>
		/// Check whether this Text node can be stored in a char[] in the translet.
		/// Calling this is precondition to calling loadAsArrayOffsetLength. </summary>
		/// <seealso cref=".loadAsArrayOffsetLength(ClassGenerator,MethodGenerator)"/>
		/// <returns> true if this Text node can be </returns>
		public bool canLoadAsArrayOffsetLength()
		{
			// Magic number!  21845*3 == 65535.  BCEL uses a DataOutputStream to
			// serialize class files.  The Java run-time places a limit on the size
			// of String data written using a DataOutputStream - it cannot require
			// more than 64KB when represented as UTF-8.  The number of bytes
			// required to represent a Java string as UTF-8 cannot be greater
			// than three times the number of char's in the string, hence the
			// check for 21845.

			return (_text.Length <= 21845);
		}

		/// <summary>
		/// Generates code that loads the array that will contain the character
		/// data represented by this Text node, followed by the offset of the
		/// data from the start of the array, and then the length of the data.
		/// 
		/// The pre-condition to calling this method is that
		/// canLoadAsArrayOffsetLength() returns true. </summary>
		/// <seealso cref=".canLoadArrayOffsetLength()"/>
		public void loadAsArrayOffsetLength(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = classGen.getParser().getXSLTC();
			XSLTC xsltc = classGen.Parser.XSLTC;

			// The XSLTC object keeps track of character data
			// that is to be stored in char arrays.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int offset = xsltc.addCharacterData(_text);
			int offset = xsltc.addCharacterData(_text);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = _text.length();
			int length = _text.Length;
			string charDataFieldName = STATIC_CHAR_DATA_FIELD + (xsltc.CharacterDataCount - 1);

			il.append(new GETSTATIC(cpg.addFieldref(xsltc.ClassName, charDataFieldName, STATIC_CHAR_DATA_FIELD_SIG)));
			il.append(new PUSH(cpg, offset));
			il.append(new PUSH(cpg, _text.Length));
		}
	}

}