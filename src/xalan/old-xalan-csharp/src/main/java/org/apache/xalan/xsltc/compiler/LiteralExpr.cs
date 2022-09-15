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
 * $Id: LiteralExpr.java 468650 2006-10-28 07:03:30Z minchau $
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

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class LiteralExpr : Expression
	{
		private readonly string _value;
		private readonly string _namespace;

		/// <summary>
		/// Creates a new literal expression node. </summary>
		/// <param name="value"> the literal expression content/value. </param>
		public LiteralExpr(string value)
		{
		_value = value;
		_namespace = null;
		}

		/// <summary>
		/// Creates a new literal expression node. </summary>
		/// <param name="value"> the literal expression content/value. </param>
		/// <param name="namespace"> the namespace in which the expression exists. </param>
		public LiteralExpr(string value, string @namespace)
		{
		_value = value;
		_namespace = @namespace.Equals(Constants_Fields.EMPTYSTRING) ? null : @namespace;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return _type = Type.String;
		}

		public override string ToString()
		{
		return "literal-expr(" + _value + ')';
		}

		protected internal override bool contextDependent()
		{
		return false;
		}

		protected internal string Value
		{
			get
			{
			return _value;
			}
		}

		protected internal string Namespace
		{
			get
			{
			return _namespace;
			}
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		il.append(new PUSH(cpg, _value));
		}
	}

}