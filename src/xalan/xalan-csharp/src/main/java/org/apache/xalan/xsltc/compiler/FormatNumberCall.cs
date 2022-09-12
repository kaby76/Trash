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
 * $Id: FormatNumberCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using RealType = org.apache.xalan.xsltc.compiler.util.RealType;
	using StringType = org.apache.xalan.xsltc.compiler.util.StringType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class FormatNumberCall : FunctionCall
	{
		private Expression _value;
		private Expression _format;
		private Expression _name;
		private QName _resolvedQName = null;

		public FormatNumberCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		_value = argument(0);
		_format = argument(1);
		_name = argumentCount() == 3 ? argument(2) : null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{

		// Inform stylesheet to instantiate a DecimalFormat object
		Stylesheet.numberFormattingUsed();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tvalue = _value.typeCheck(stable);
		Type tvalue = _value.typeCheck(stable);
		if (tvalue is RealType == false)
		{
			_value = new CastExpr(_value, Type.Real);
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tformat = _format.typeCheck(stable);
		Type tformat = _format.typeCheck(stable);
		if (tformat is StringType == false)
		{
			_format = new CastExpr(_format, Type.String);
		}
		if (argumentCount() == 3)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tname = _name.typeCheck(stable);
			Type tname = _name.typeCheck(stable);

			if (_name is LiteralExpr)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LiteralExpr literal = (LiteralExpr) _name;
			LiteralExpr literal = (LiteralExpr) _name;
			_resolvedQName = Parser.getQNameIgnoreDefaultNs(literal.Value);
			}
			else if (tname is StringType == false)
			{
			_name = new CastExpr(_name, Type.String);
			}
		}
		return _type = Type.String;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		_value.translate(classGen, methodGen);
		_format.translate(classGen, methodGen);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fn3arg = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "formatNumber", "(DLjava/lang/String;"+ "Ljava/text/DecimalFormat;)"+ "Ljava/lang/String;");
		int fn3arg = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "formatNumber", "(DLjava/lang/String;" + "Ljava/text/DecimalFormat;)" + "Ljava/lang/String;");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int get = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "getDecimalFormat", "(Ljava/lang/String;)"+ "Ljava/text/DecimalFormat;");
		int get = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, "getDecimalFormat", "(Ljava/lang/String;)" + "Ljava/text/DecimalFormat;");

		il.append(classGen.loadTranslet());
		if (_name == null)
		{
			il.append(new PUSH(cpg, Constants_Fields.EMPTYSTRING));
		}
		else if (_resolvedQName != null)
		{
			il.append(new PUSH(cpg, _resolvedQName.ToString()));
		}
		else
		{
			_name.translate(classGen, methodGen);
		}
		il.append(new INVOKEVIRTUAL(get));
		il.append(new INVOKESTATIC(fn3arg));
		}
	}

}