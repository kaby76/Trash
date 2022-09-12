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
 * $Id: XslElement.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETSTATIC = org.apache.bcel.generic.GETSTATIC;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using PUSH = org.apache.bcel.generic.PUSH;
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
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class XslElement : Instruction
	{

		private string _prefix;
		private bool _ignore = false;
		private bool _isLiteralName = true;
		private AttributeValueTemplate _name;
		private AttributeValueTemplate _namespace;

		/// <summary>
		/// Displays the contents of the element
		/// </summary>
		public override void display(int indent)
		{
		indent(indent);
		Util.println("Element " + _name);
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// This method is now deprecated. The new implemation of this class
		/// never declares the default NS.
		/// </summary>
		public bool declaresDefaultNS()
		{
		return false;
		}

		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
		SymbolTable stable = parser.SymbolTable;

		// Handle the 'name' attribute
		string name = getAttribute("name");
		if (string.ReferenceEquals(name, Constants_Fields.EMPTYSTRING))
		{
			ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_ELEM_NAME_ERR, name, this);
			parser.reportError(Constants_Fields.WARNING, msg);
			parseChildren(parser);
			_ignore = true; // Ignore the element if the QName is invalid
			return;
		}

		// Get namespace attribute
		string @namespace = getAttribute("namespace");

		// Optimize compilation when name is known at compile time
			_isLiteralName = Util.isLiteral(name);
		if (_isLiteralName)
		{
				if (!XML11Char.isXML11ValidQName(name))
				{
			ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_ELEM_NAME_ERR, name, this);
			parser.reportError(Constants_Fields.WARNING, msg);
			parseChildren(parser);
			_ignore = true; // Ignore the element if the QName is invalid
			return;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName qname = parser.getQNameSafe(name);
			QName qname = parser.getQNameSafe(name);
			string prefix = qname.Prefix;
			string local = qname.LocalPart;

			if (string.ReferenceEquals(prefix, null))
			{
			prefix = Constants_Fields.EMPTYSTRING;
			}

			if (!hasAttribute("namespace"))
			{
			@namespace = lookupNamespace(prefix);
			if (string.ReferenceEquals(@namespace, null))
			{
				ErrorMsg err = new ErrorMsg(ErrorMsg.NAMESPACE_UNDEF_ERR, prefix, this);
				parser.reportError(Constants_Fields.WARNING, err);
				parseChildren(parser);
				_ignore = true; // Ignore the element if prefix is undeclared
				return;
			}
			_prefix = prefix;
			_namespace = new AttributeValueTemplate(@namespace, parser, this);
			}
			else
			{
			if (string.ReferenceEquals(prefix, Constants_Fields.EMPTYSTRING))
			{
					if (Util.isLiteral(@namespace))
					{
				prefix = lookupPrefix(@namespace);
				if (string.ReferenceEquals(prefix, null))
				{
					prefix = stable.generateNamespacePrefix();
				}
					}

				// Prepend prefix to local name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer newName = new StringBuffer(prefix);
				StringBuilder newName = new StringBuilder(prefix);
				if (!string.ReferenceEquals(prefix, Constants_Fields.EMPTYSTRING))
				{
				newName.Append(':');
				}
				name = newName.Append(local).ToString();
			}
			_prefix = prefix;
			_namespace = new AttributeValueTemplate(@namespace, parser, this);
			}
		}
		else
		{
				// name attribute contains variable parts.  If there is no namespace
				// attribute, the generated code needs to be prepared to look up
				// any prefix in the stylesheet at run-time.
				_namespace = (string.ReferenceEquals(@namespace, Constants_Fields.EMPTYSTRING)) ? null : new AttributeValueTemplate(@namespace, parser, this);
		}

		_name = new AttributeValueTemplate(name, parser, this);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String useSets = getAttribute("use-attribute-sets");
		string useSets = getAttribute("use-attribute-sets");
		if (useSets.Length > 0)
		{
				if (!Util.isValidQNames(useSets))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, useSets, this);
					parser.reportError(Constants_Fields.ERROR, err);
				}
			FirstElement = new UseAttributeSets(useSets, parser);
		}

		parseChildren(parser);
		}

		/// <summary>
		/// Run type check on element name & contents
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
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
		}
		typeCheckContents(stable);
		return Type.Void;
		}

		/// <summary>
		/// This method is called when the name of the element is known at compile time.
		/// In this case, there is no need to inspect the element name at runtime to
		/// determine if a prefix exists, needs to be generated, etc.
		/// </summary>
		public void translateLiteral(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		if (!_ignore)
		{
			il.append(methodGen.loadHandler());
			_name.translate(classGen, methodGen);
			il.append(DUP2);
			il.append(methodGen.startElement());

			if (_namespace != null)
			{
			il.append(methodGen.loadHandler());
			il.append(new PUSH(cpg, _prefix));
			_namespace.translate(classGen,methodGen);
			il.append(methodGen.@namespace());
			}
		}

		translateContents(classGen, methodGen);

		if (!_ignore)
		{
			il.append(methodGen.endElement());
		}
		}

		/// <summary>
		/// At runtime the compilation of xsl:element results in code that: (i)
		/// evaluates the avt for the name, (ii) checks for a prefix in the name
		/// (iii) generates a new prefix and create a new qname when necessary
		/// (iv) calls startElement() on the handler (v) looks up a uri in the XML
		/// when the prefix is not known at compile time (vi) calls namespace() 
		/// on the handler (vii) evaluates the contents (viii) calls endElement().
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		LocalVariableGen local = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// Optimize translation if element name is a literal
		if (_isLiteralName)
		{
			translateLiteral(classGen, methodGen);
			return;
		}

		if (!_ignore)
		{

				// if the qname is an AVT, then the qname has to be checked at runtime if it is a valid qname
				LocalVariableGen nameValue = methodGen.addLocalVariable2("nameValue", Util.getJCRefType(Constants_Fields.STRING_SIG), null);

				// store the name into a variable first so _name.translate only needs to be called once  
				_name.translate(classGen, methodGen);
				nameValue.Start = il.append(new ASTORE(nameValue.Index));
				il.append(new ALOAD(nameValue.Index));

				// call checkQName if the name is an AVT
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int check = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "checkQName", "(" +Constants_Fields.STRING_SIG +")V");
				int check = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "checkQName", "(" + Constants_Fields.STRING_SIG + ")V");
				il.append(new INVOKESTATIC(check));

				// Push handler for call to endElement()
				il.append(methodGen.loadHandler());

				// load name value again
				nameValue.End = il.append(new ALOAD(nameValue.Index));

			if (_namespace != null)
			{
			_namespace.translate(classGen, methodGen);
			}
			else
			{
					// If name is an AVT and namespace is not specified, need to
					// look up any prefix in the stylesheet by calling
					//   BasisLibrary.lookupStylesheetQNameNamespace(
					//                name, stylesheetNode, ancestorsArray,
					//                prefixURIsIndexArray, prefixURIPairsArray,
					//                !ignoreDefaultNamespace)
					string transletClassName = XSLTC.ClassName;
					il.append(DUP);
					il.append(new PUSH(cpg, NodeIDForStylesheetNSLookup));
					il.append(new GETSTATIC(cpg.addFieldref(transletClassName, Constants_Fields.STATIC_NS_ANCESTORS_ARRAY_FIELD, Constants_Fields.NS_ANCESTORS_INDEX_SIG)));
					il.append(new GETSTATIC(cpg.addFieldref(transletClassName, Constants_Fields.STATIC_PREFIX_URIS_IDX_ARRAY_FIELD, Constants_Fields.PREFIX_URIS_IDX_SIG)));
					il.append(new GETSTATIC(cpg.addFieldref(transletClassName, Constants_Fields.STATIC_PREFIX_URIS_ARRAY_FIELD, Constants_Fields.PREFIX_URIS_ARRAY_SIG)));
					// Default namespace is significant
					il.append(ICONST_0);
					il.append(new INVOKESTATIC(cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, Constants_Fields.LOOKUP_STYLESHEET_QNAME_NS_REF, Constants_Fields.LOOKUP_STYLESHEET_QNAME_NS_SIG)));
			}

				// Push additional arguments
			il.append(methodGen.loadHandler());
			il.append(methodGen.loadDOM());
			il.append(methodGen.loadCurrentNode());

				// Invoke BasisLibrary.startXslElemCheckQName()
				il.append(new INVOKESTATIC(cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "startXslElement", "(" + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + Constants_Fields.TRANSLET_OUTPUT_SIG + Constants_Fields.DOM_INTF_SIG + "I)" + Constants_Fields.STRING_SIG)));


		}

		translateContents(classGen, methodGen);

		if (!_ignore)
		{
			il.append(methodGen.endElement());
		}
		}

		/// <summary>
		/// Override this method to make sure that xsl:attributes are not
		/// copied to output if this xsl:element is to be ignored
		/// </summary>
		public override void translateContents(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = elementCount();
		int n = elementCount();
		for (int i = 0; i < n; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode item = (SyntaxTreeNode)getContents().elementAt(i);
			SyntaxTreeNode item = (SyntaxTreeNode)Contents[i];
			if (_ignore && item is XslAttribute)
			{
				continue;
			}
			item.translate(classGen, methodGen);
		}
		}

	}

}