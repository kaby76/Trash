using System.Collections;

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
 * $Id: XslAttribute.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	using ElemDesc = org.apache.xml.serializer.ElemDesc;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author Gunnlaugur Briem <gthb@dimon.is>
	/// </summary>
	internal sealed class XslAttribute : Instruction
	{

		private string _prefix;
		private AttributeValue _name; // name treated as AVT (7.1.3)
		private AttributeValueTemplate _namespace = null;
		private bool _ignore = false;
		private bool _isLiteral = false; // specified name is not AVT

		/// <summary>
		/// Returns the name of the attribute
		/// </summary>
		public AttributeValue Name
		{
			get
			{
			return _name;
			}
		}

		/// <summary>
		/// Displays the contents of the attribute
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("Attribute " + _name);
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Parses the attribute's contents. Special care taken for namespaces.
		/// </summary>
		public override void parseContents(Parser parser)
		{
		bool generated = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
		SymbolTable stable = parser.SymbolTable;

		string name = getAttribute("name");
		string @namespace = getAttribute("namespace");
		QName qname = parser.getQName(name, false);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = qname.getPrefix();
		string prefix = qname.Prefix;

			if (((!string.ReferenceEquals(prefix, null)) && (prefix.Equals(XMLNS_PREFIX))) || (name.Equals(XMLNS_PREFIX)))
			{
			reportError(this, parser, ErrorMsg.ILLEGAL_ATTR_NAME_ERR, name);
			return;
			}

			_isLiteral = Util.isLiteral(name);
			if (_isLiteral)
			{
				if (!XML11Char.isXML11ValidQName(name))
				{
					reportError(this, parser, ErrorMsg.ILLEGAL_ATTR_NAME_ERR, name);
					return;
				}
			}

		// Ignore attribute if preceeded by some other type of element
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode parent = getParent();
		SyntaxTreeNode parent = Parent;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector siblings = parent.getContents();
		ArrayList siblings = parent.Contents;
		for (int i = 0; i < parent.elementCount(); i++)
		{
			SyntaxTreeNode item = (SyntaxTreeNode)siblings[i];
			if (item == this)
			{
				break;
			}

			// These three objects result in one or more attribute output
			if (item is XslAttribute)
			{
				continue;
			}
			if (item is UseAttributeSets)
			{
				continue;
			}
			if (item is LiteralAttribute)
			{
				continue;
			}
			if (item is Text)
			{
				continue;
			}

			// These objects _can_ result in one or more attribute
			// The output handler will generate an error if not (at runtime)
			if (item is If)
			{
				continue;
			}
			if (item is Choose)
			{
				continue;
			}
			 if (item is CopyOf)
			 {
				 continue;
			 }
			 if (item is VariableBase)
			 {
				 continue;
			 }

			// Report warning but do not ignore attribute
			reportWarning(this, parser, ErrorMsg.STRAY_ATTRIBUTE_ERR, name);
		}

		// Get namespace from namespace attribute?
		if (!string.ReferenceEquals(@namespace, null) && !string.ReferenceEquals(@namespace, Constants.EMPTYSTRING))
		{
			_prefix = lookupPrefix(@namespace);
			_namespace = new AttributeValueTemplate(@namespace, parser, this);
		}
		// Get namespace from prefix in name attribute?
		else if (!string.ReferenceEquals(prefix, null) && !string.ReferenceEquals(prefix, Constants.EMPTYSTRING))
		{
			_prefix = prefix;
			@namespace = lookupNamespace(prefix);
			if (!string.ReferenceEquals(@namespace, null))
			{
			_namespace = new AttributeValueTemplate(@namespace, parser, this);
			}
		}

		// Common handling for namespaces:
		if (_namespace != null)
		{
			// Generate prefix if we have none
			if (string.ReferenceEquals(_prefix, null) || string.ReferenceEquals(_prefix, Constants.EMPTYSTRING))
			{
			if (!string.ReferenceEquals(prefix, null))
			{
				_prefix = prefix;
			}
			else
			{
				_prefix = stable.generateNamespacePrefix();
				generated = true;
			}
			}
			else if (!string.ReferenceEquals(prefix, null) && !prefix.Equals(_prefix))
			{
			_prefix = prefix;
			}

			name = _prefix + ":" + qname.LocalPart;

			/*
			 * TODO: The namespace URI must be passed to the parent 
			 * element but we don't yet know what the actual URI is 
			 * (as we only know it as an attribute value template). 
			 */
			if ((parent is LiteralElement) && (!generated))
			{
			((LiteralElement)parent).registerNamespace(_prefix, @namespace, stable, false);
			}
		}

		if (parent is LiteralElement)
		{
			((LiteralElement)parent).addAttribute(this);
		}

		_name = AttributeValue.create(this, name, parser);
		parseChildren(parser);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (!_ignore)
		{
			_name.typeCheck(stable);
			if (_namespace != null)
			{
			_namespace.typeCheck(stable);
			}
			typeCheckContents(stable);
		}
		return Type.Void;
		}

		/// 
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

		// Compile code that emits any needed namespace declaration
		if (_namespace != null)
		{
			// public void attribute(final String name, final String value)
			il.append(methodGen.loadHandler());
			il.append(new PUSH(cpg,_prefix));
			_namespace.translate(classGen,methodGen);
			il.append(methodGen.@namespace());
		}

			if (!_isLiteral)
			{
				// if the qname is an AVT, then the qname has to be checked at runtime if it is a valid qname
				LocalVariableGen nameValue = methodGen.addLocalVariable2("nameValue", Util.getJCRefType(STRING_SIG), null);

				// store the name into a variable first so _name.translate only needs to be called once  
				_name.translate(classGen, methodGen);
				nameValue.setStart(il.append(new ASTORE(nameValue.getIndex())));
				il.append(new ALOAD(nameValue.getIndex()));

				// call checkQName if the name is an AVT
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int check = cpg.addMethodref(BASIS_LIBRARY_CLASS, "checkAttribQName", "(" +STRING_SIG +")V");
				int check = cpg.addMethodref(BASIS_LIBRARY_CLASS, "checkAttribQName", "(" + STRING_SIG + ")V");
				il.append(new INVOKESTATIC(check));

				// Save the current handler base on the stack
				il.append(methodGen.loadHandler());
				il.append(DUP); // first arg to "attributes" call

				// load name value again    
				nameValue.setEnd(il.append(new ALOAD(nameValue.getIndex())));
			}
			else
			{
				// Save the current handler base on the stack
				il.append(methodGen.loadHandler());
				il.append(DUP); // first arg to "attributes" call

				// Push attribute name
				_name.translate(classGen, methodGen); // 2nd arg

			}

		// Push attribute value - shortcut for literal strings
		if ((elementCount() == 1) && (elementAt(0) is Text))
		{
			il.append(new PUSH(cpg, ((Text)elementAt(0)).Text));
		}
		else
		{
			il.append(classGen.loadTranslet());
			il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, "stringValueHandler", STRING_VALUE_HANDLER_SIG)));
			il.append(DUP);
			il.append(methodGen.storeHandler());
			// translate contents with substituted handler
			translateContents(classGen, methodGen);
			// get String out of the handler
			il.append(new INVOKEVIRTUAL(cpg.addMethodref(STRING_VALUE_HANDLER, "getValue", "()" + STRING_SIG)));
		}

		SyntaxTreeNode parent = Parent;
		if (parent is LiteralElement && ((LiteralElement)parent).allAttributesUnique())
		{
				int flags = 0;
			ElemDesc elemDesc = ((LiteralElement)parent).ElemDesc;

			// Set the HTML flags
			if (elemDesc != null && _name is SimpleAttributeValue)
			{
				string attrName = ((SimpleAttributeValue)_name).ToString();
				if (elemDesc.isAttrFlagSet(attrName, ElemDesc.ATTREMPTY))
				{
					flags = flags | SerializationHandler.HTML_ATTREMPTY;
				}
				else if (elemDesc.isAttrFlagSet(attrName, ElemDesc.ATTRURL))
				{
					flags = flags | SerializationHandler.HTML_ATTRURL;
				}
			}
			il.append(new PUSH(cpg, flags));
			il.append(methodGen.uniqueAttribute());
		}
		else
		{
			// call "attribute"
			il.append(methodGen.attribute());
		}

		// Restore old handler base from stack
		il.append(methodGen.storeHandler());



		}

	}

}