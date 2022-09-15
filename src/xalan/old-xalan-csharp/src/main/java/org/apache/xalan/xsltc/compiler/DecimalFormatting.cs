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
 * $Id: DecimalFormatting.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETSTATIC = org.apache.bcel.generic.GETSTATIC;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class DecimalFormatting : TopLevelElement
	{

		private const string DFS_CLASS = "java.text.DecimalFormatSymbols";
		private const string DFS_SIG = "Ljava/text/DecimalFormatSymbols;";

		private QName _name = null;

		/// <summary>
		/// No type check needed for the <xsl:decimal-formatting/> element
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return Type.Void;
		}

		/// <summary>
		/// Parse the name of the <xsl:decimal-formatting/> element
		/// </summary>
		public override void parseContents(Parser parser)
		{
		// Get the name of these decimal formatting symbols
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getAttribute("name");
			string name = getAttribute("name");
			if (name.Length > 0)
			{
				if (!XML11Char.isXML11ValidQName(name))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, name, this);
					parser.reportError(Constants_Fields.ERROR, err);
				}
			}
			_name = parser.getQNameIgnoreDefaultNs(name);
			if (_name == null)
			{
				_name = parser.getQNameIgnoreDefaultNs(Constants_Fields.EMPTYSTRING);
			}

		// Check if a set of symbols has already been registered under this name
		SymbolTable stable = parser.SymbolTable;
		if (stable.getDecimalFormatting(_name) != null)
		{
			reportWarning(this, parser, ErrorMsg.SYMBOLS_REDEF_ERR, _name.ToString());
		}
		else
		{
			stable.addDecimalFormatting(_name, this);
		}
		}

		/// <summary>
		/// This method is called when the constructor is compiled in
		/// Stylesheet.compileConstructor() and not as the syntax tree is traversed.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

		ConstantPoolGen cpg = classGen.ConstantPool;
		InstructionList il = methodGen.InstructionList;

		// DecimalFormatSymbols.<init>(Locale);
			// xsl:decimal-format - except for the NaN and infinity attributes.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int init = cpg.addMethodref(DFS_CLASS, "<init>", "("+Constants_Fields.LOCALE_SIG+")V");
		int init = cpg.addMethodref(DFS_CLASS, "<init>", "(" + Constants_Fields.LOCALE_SIG + ")V");

		// Push the format name on the stack for call to addDecimalFormat()
		il.append(classGen.loadTranslet());
		il.append(new PUSH(cpg, _name.ToString()));

		// Manufacture a DecimalFormatSymbols on the stack
		// for call to addDecimalFormat()
			// Use the US Locale as the default, as most of its settings
			// are equivalent to the default settings required of
		il.append(new NEW(cpg.addClass(DFS_CLASS)));
		il.append(DUP);
			il.append(new GETSTATIC(cpg.addFieldref(Constants_Fields.LOCALE_CLASS, "US", Constants_Fields.LOCALE_SIG)));
		il.append(new INVOKESPECIAL(init));

		string tmp = getAttribute("NaN");
		if ((string.ReferenceEquals(tmp, null)) || (tmp.Equals(Constants_Fields.EMPTYSTRING)))
		{
			int nan = cpg.addMethodref(DFS_CLASS, "setNaN", "(Ljava/lang/String;)V");
			il.append(DUP);
			il.append(new PUSH(cpg, "NaN"));
			il.append(new INVOKEVIRTUAL(nan));
		}

		tmp = getAttribute("infinity");
		if ((string.ReferenceEquals(tmp, null)) || (tmp.Equals(Constants_Fields.EMPTYSTRING)))
		{
			int inf = cpg.addMethodref(DFS_CLASS, "setInfinity", "(Ljava/lang/String;)V");
			il.append(DUP);
			il.append(new PUSH(cpg, "Infinity"));
			il.append(new INVOKEVIRTUAL(inf));
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nAttributes = _attributes.getLength();
		int nAttributes = _attributes.Length;
		for (int i = 0; i < nAttributes; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _attributes.getQName(i);
			string name = _attributes.getQName(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = _attributes.getValue(i);
			string value = _attributes.getValue(i);

			bool valid = true;
			int method = 0;

			if (name.Equals("decimal-separator"))
			{
			// DecimalFormatSymbols.setDecimalSeparator();
			method = cpg.addMethodref(DFS_CLASS, "setDecimalSeparator", "(C)V");
			}
			else if (name.Equals("grouping-separator"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setGroupingSeparator", "(C)V");
			}
			else if (name.Equals("minus-sign"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setMinusSign", "(C)V");
			}
			else if (name.Equals("percent"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setPercent", "(C)V");
			}
			else if (name.Equals("per-mille"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setPerMill", "(C)V");
			}
			else if (name.Equals("zero-digit"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setZeroDigit", "(C)V");
			}
			else if (name.Equals("digit"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setDigit", "(C)V");
			}
			else if (name.Equals("pattern-separator"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setPatternSeparator", "(C)V");
			}
			else if (name.Equals("NaN"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setNaN", "(Ljava/lang/String;)V");
				il.append(DUP);
			il.append(new PUSH(cpg, value));
			il.append(new INVOKEVIRTUAL(method));
			valid = false;
			}
			else if (name.Equals("infinity"))
			{
			method = cpg.addMethodref(DFS_CLASS, "setInfinity", "(Ljava/lang/String;)V");
				il.append(DUP);
			il.append(new PUSH(cpg, value));
			il.append(new INVOKEVIRTUAL(method));
			valid = false;
			}
			else
			{
			valid = false;
			}

			if (valid)
			{
			il.append(DUP);
			il.append(new PUSH(cpg, value[0]));
			il.append(new INVOKEVIRTUAL(method));
			}

		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int put = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "addDecimalFormat", "("+Constants_Fields.STRING_SIG+DFS_SIG+")V");
		int put = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "addDecimalFormat", "(" + Constants_Fields.STRING_SIG + DFS_SIG + ")V");
		il.append(new INVOKEVIRTUAL(put));
		}

		/// <summary>
		/// Creates the default, nameless, DecimalFormat object in
		/// AbstractTranslet's format_symbols hashtable.
		/// This should be called for every stylesheet, and the entry
		/// may be overridden by later nameless xsl:decimal-format instructions.
		/// </summary>
		public static void translateDefaultDFS(ClassGenerator classGen, MethodGenerator methodGen)
		{

		ConstantPoolGen cpg = classGen.ConstantPool;
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int init = cpg.addMethodref(DFS_CLASS, "<init>", "("+Constants_Fields.LOCALE_SIG+")V");
		int init = cpg.addMethodref(DFS_CLASS, "<init>", "(" + Constants_Fields.LOCALE_SIG + ")V");

		// Push the format name, which is empty, on the stack
		// for call to addDecimalFormat()
		il.append(classGen.loadTranslet());
		il.append(new PUSH(cpg, Constants_Fields.EMPTYSTRING));

		// Manufacture a DecimalFormatSymbols on the stack for
		// call to addDecimalFormat().  Use the US Locale as the
			// default, as most of its settings are equivalent to
			// the default settings required of xsl:decimal-format -
			// except for the NaN and infinity attributes.
		il.append(new NEW(cpg.addClass(DFS_CLASS)));
		il.append(DUP);
			il.append(new GETSTATIC(cpg.addFieldref(Constants_Fields.LOCALE_CLASS, "US", Constants_Fields.LOCALE_SIG)));
		il.append(new INVOKESPECIAL(init));

		int nan = cpg.addMethodref(DFS_CLASS, "setNaN", "(Ljava/lang/String;)V");
		il.append(DUP);
		il.append(new PUSH(cpg, "NaN"));
		il.append(new INVOKEVIRTUAL(nan));

		int inf = cpg.addMethodref(DFS_CLASS, "setInfinity", "(Ljava/lang/String;)V");
		il.append(DUP);
		il.append(new PUSH(cpg, "Infinity"));
		il.append(new INVOKEVIRTUAL(inf));

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int put = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "addDecimalFormat", "("+Constants_Fields.STRING_SIG+DFS_SIG+")V");
		int put = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "addDecimalFormat", "(" + Constants_Fields.STRING_SIG + DFS_SIG + ")V");
		il.append(new INVOKEVIRTUAL(put));
		}
	}

}