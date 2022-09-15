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
 * $Id: LiteralAttribute.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	using ElemDesc = org.apache.xml.serializer.ElemDesc;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class LiteralAttribute : Instruction
	{

		private readonly string _name; // Attribute name (incl. prefix)
		private readonly AttributeValue _value; // Attribute value

		/// <summary>
		/// Creates a new literal attribute (but does not insert it into the AST). </summary>
		/// <param name="name"> the attribute name (incl. prefix) as a String. </param>
		/// <param name="value"> the attribute value. </param>
		/// <param name="parser"> the XSLT parser (wraps XPath parser). </param>
		public LiteralAttribute(string name, string value, Parser parser, SyntaxTreeNode parent)
		{
		_name = name;
			Parent = parent;
		_value = AttributeValue.create(this, value, parser);
		}

		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("LiteralAttribute name=" + _name + " value=" + _value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		_value.typeCheck(stable);
		typeCheckContents(stable);
		return Type.Void;
		}

		protected internal override bool contextDependent()
		{
		return _value.contextDependent();
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// push handler
		il.append(methodGen.loadHandler());
		// push attribute name - namespace prefix set by parent node
		il.append(new PUSH(cpg, _name));
		// push attribute value
		_value.translate(classGen, methodGen);

		// Generate code that calls SerializationHandler.addUniqueAttribute()
		// if all attributes are unique.
		SyntaxTreeNode parent = Parent;
		if (parent is LiteralElement && ((LiteralElement)parent).allAttributesUnique())
		{

			int flags = 0;
			bool isHTMLAttrEmpty = false;
			ElemDesc elemDesc = ((LiteralElement)parent).ElemDesc;

			// Set the HTML flags
			if (elemDesc != null)
			{
				if (elemDesc.isAttrFlagSet(_name, ElemDesc.ATTREMPTY))
				{
					flags = flags | SerializationHandler.HTML_ATTREMPTY;
					isHTMLAttrEmpty = true;
				}
				else if (elemDesc.isAttrFlagSet(_name, ElemDesc.ATTRURL))
				{
					flags = flags | SerializationHandler.HTML_ATTRURL;
				}
			}

			if (_value is SimpleAttributeValue)
			{
				string attrValue = ((SimpleAttributeValue)_value).ToString();

				if (!hasBadChars(attrValue) && !isHTMLAttrEmpty)
				{
					flags = flags | SerializationHandler.NO_BAD_CHARS;
				}
			}

			il.append(new PUSH(cpg, flags));
			il.append(methodGen.uniqueAttribute());
		}
		else
		{
			// call attribute
			il.append(methodGen.attribute());
		}
		}

		/// <summary>
		/// Return true if at least one character in the String is considered to
		/// be a "bad" character. A bad character is one whose code is:
		/// less than 32 (a space),
		/// or greater than 126,
		/// or it is one of '<', '>', '&' or '\"'. 
		/// This helps the serializer to decide whether the String needs to be escaped.
		/// </summary>
		private bool hasBadChars(string value)
		{
			char[] chars = value.ToCharArray();
			int size = chars.Length;
			for (int i = 0; i < size; i++)
			{
				char ch = chars[i];
				if (ch < (char)32 || (char)126 < ch || ch == '<' || ch == '>' || ch == '&' || ch == '\"')
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Return the name of the attribute
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Return the value of the attribute
		/// </summary>
		public AttributeValue Value
		{
			get
			{
				return _value;
			}
		}

	}

}