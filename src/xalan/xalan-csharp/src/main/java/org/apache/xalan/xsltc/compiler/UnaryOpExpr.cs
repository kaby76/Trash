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
 * $Id: UnaryOpExpr.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class UnaryOpExpr : Expression
	{
		private Expression _left;

		public UnaryOpExpr(Expression left)
		{
		(_left = left).setParent(this);
		}

		/// <summary>
		/// Returns true if this expressions contains a call to position(). This is
		/// needed for context changes in node steps containing multiple predicates.
		/// </summary>
		public override bool hasPositionCall()
		{
		return (_left.hasPositionCall());
		}

		/// <summary>
		/// Returns true if this expressions contains a call to last()
		/// </summary>
		public override bool hasLastCall()
		{
				return (_left.hasLastCall());
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_left.Parser = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tleft = _left.typeCheck(stable);
		Type tleft = _left.typeCheck(stable);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodType ptype = lookupPrimop(stable, "u-", new org.apache.xalan.xsltc.compiler.util.MethodType(org.apache.xalan.xsltc.compiler.util.Type.Void, tleft));
		MethodType ptype = lookupPrimop(stable, "u-", new MethodType(Type.Void, tleft));

		if (ptype != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type arg1 = (org.apache.xalan.xsltc.compiler.util.Type) ptype.argsType().elementAt(0);
			Type arg1 = (Type) ptype.argsType()[0];
			if (!arg1.identicalTo(tleft))
			{
			_left = new CastExpr(_left, arg1);
			}
			return _type = ptype.resultType();
		}

		throw new TypeCheckError(this);
		}

		public override string ToString()
		{
		return "u-" + '(' + _left + ')';
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		InstructionList il = methodGen.getInstructionList();
		_left.translate(classGen, methodGen);
		il.append(_type.NEG());
		}
	}


}