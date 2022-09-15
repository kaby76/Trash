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
 * $Id: LangCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using FilterGenerator = org.apache.xalan.xsltc.compiler.util.FilterGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using StringType = org.apache.xalan.xsltc.compiler.util.StringType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class LangCall : FunctionCall
	{
		private Expression _lang;
		private Type _langType;

		/// <summary>
		/// Get the parameters passed to function:
		///   lang(string)
		/// </summary>
		public LangCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		_lang = argument(0);
		}

		/// 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		_langType = _lang.typeCheck(stable);
		if (!(_langType is StringType))
		{
			_lang = new CastExpr(_lang, Type.String);
		}
		return Type.Boolean;
		}

		/// 
		public override Type Type
		{
			get
			{
			return (Type.Boolean);
			}
		}

		/// <summary>
		/// This method is called when the constructor is compiled in
		/// Stylesheet.compileConstructor() and not as the syntax tree is traversed.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int tst = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "testLanguage", "("+Constants_Fields.STRING_SIG+Constants_Fields.DOM_INTF_SIG+"I)Z");
		int tst = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "testLanguage", "(" + Constants_Fields.STRING_SIG + Constants_Fields.DOM_INTF_SIG + "I)Z");
		_lang.translate(classGen,methodGen);
		il.append(methodGen.loadDOM());
		if (classGen is FilterGenerator)
		{
			il.append(new ILOAD(1));
		}
		else
		{
			il.append(methodGen.loadContextNode());
		}
		il.append(new INVOKESTATIC(tst));
		}
	}

}