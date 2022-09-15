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
 * $Id: BinOpExpr.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class BinOpExpr : Expression
	{
		public const int PLUS = 0;
		public const int MINUS = 1;
		public const int TIMES = 2;
		public const int DIV = 3;
		public const int MOD = 4;

		private static readonly string[] Ops = new string[] {"+", "-", "*", "/", "%"};

		private int _op;
		private Expression _left, _right;

		public BinOpExpr(int op, Expression left, Expression right)
		{
		_op = op;
		(_left = left).Parent = this;
		(_right = right).Parent = this;
		}

		/// <summary>
		/// Returns true if this expressions contains a call to position(). This is
		/// needed for context changes in node steps containing multiple predicates.
		/// </summary>
		public override bool hasPositionCall()
		{
		if (_left.hasPositionCall())
		{
			return true;
		}
		if (_right.hasPositionCall())
		{
			return true;
		}
		return false;
		}

		/// <summary>
		/// Returns true if this expressions contains a call to last()
		/// </summary>
		public override bool hasLastCall()
		{
				return (_left.hasLastCall() || _right.hasLastCall());
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_left.Parser = value;
			_right.Parser = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tleft = _left.typeCheck(stable);
		Type tleft = _left.typeCheck(stable);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tright = _right.typeCheck(stable);
		Type tright = _right.typeCheck(stable);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodType ptype = lookupPrimop(stable, Ops[_op], new org.apache.xalan.xsltc.compiler.util.MethodType(org.apache.xalan.xsltc.compiler.util.Type.Void, tleft, tright));
		MethodType ptype = lookupPrimop(stable, Ops[_op], new MethodType(Type.Void, tleft, tright));
		if (ptype != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type arg1 = (org.apache.xalan.xsltc.compiler.util.Type) ptype.argsType().elementAt(0);
			Type arg1 = (Type) ptype.argsType()[0];
			if (!arg1.identicalTo(tleft))
			{
			_left = new CastExpr(_left, arg1);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type arg2 = (org.apache.xalan.xsltc.compiler.util.Type) ptype.argsType().elementAt(1);
			Type arg2 = (Type) ptype.argsType()[1];
			if (!arg2.identicalTo(tright))
			{
			_right = new CastExpr(_right, arg1);
			}
			return _type = ptype.resultType();
		}
		throw new TypeCheckError(this);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		_left.translate(classGen, methodGen);
		_right.translate(classGen, methodGen);

		switch (_op)
		{
		case PLUS:
			il.append(_type.ADD());
			break;
		case MINUS:
			il.append(_type.SUB());
			break;
		case TIMES:
			il.append(_type.MUL());
			break;
		case DIV:
			il.append(_type.DIV());
			break;
		case MOD:
			il.append(_type.REM());
			break;
		default:
			ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_BINARY_OP_ERR, this);
			Parser.reportError(Constants_Fields.ERROR, msg);
		break;
		}
		}

		public override string ToString()
		{
		return Ops[_op] + '(' + _left + ", " + _right + ')';
		}
	}

}