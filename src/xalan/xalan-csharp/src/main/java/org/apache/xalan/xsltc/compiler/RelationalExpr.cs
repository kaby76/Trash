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
 * $Id: RelationalExpr.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using BranchInstruction = org.apache.bcel.generic.BranchInstruction;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using IntType = org.apache.xalan.xsltc.compiler.util.IntType;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using RealType = org.apache.xalan.xsltc.compiler.util.RealType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using ResultTreeType = org.apache.xalan.xsltc.compiler.util.ResultTreeType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Operators = org.apache.xalan.xsltc.runtime.Operators;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class RelationalExpr : Expression
	{

		private int _op;
		private Expression _left, _right;

		public RelationalExpr(int op, Expression left, Expression right)
		{
		_op = op;
		(_left = left).setParent(this);
		(_right = right).setParent(this);
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

		public bool hasReferenceArgs()
		{
		return _left.Type is ReferenceType || _right.Type is ReferenceType;
		}

		public bool hasNodeArgs()
		{
		return _left.Type is NodeType || _right.Type is NodeType;
		}

		public bool hasNodeSetArgs()
		{
		return _left.Type is NodeSetType || _right.Type is NodeSetType;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		Type tleft = _left.typeCheck(stable);
		Type tright = _right.typeCheck(stable);

		//bug fix # 2838, cast to reals if both are result tree fragments
		if (tleft is ResultTreeType && tright is ResultTreeType)
		{
			_right = new CastExpr(_right, Type.Real);
			_left = new CastExpr(_left, Type.Real);
			return _type = Type.Boolean;
		}

		// If one is of reference type, then convert the other too
		if (hasReferenceArgs())
		{
			Type type = null;
			Type typeL = null;
			Type typeR = null;
			if (tleft is ReferenceType)
			{
			if (_left is VariableRefBase)
			{
				VariableRefBase @ref = (VariableRefBase)_left;
				VariableBase var = @ref.Variable;
				typeL = var.Type;
			}
			}
			if (tright is ReferenceType)
			{
			if (_right is VariableRefBase)
			{
				VariableRefBase @ref = (VariableRefBase)_right;
				VariableBase var = @ref.Variable;
				typeR = var.Type;
			}
			}
			// bug fix # 2838 
			if (typeL == null)
			{
			type = typeR;
			}
			else if (typeR == null)
			{
			type = typeL;
			}
			else
			{
			type = Type.Real;
			}
			if (type == null)
			{
				type = Type.Real;
			}

			_right = new CastExpr(_right, type);
				_left = new CastExpr(_left, type);
			return _type = Type.Boolean;
		}

		if (hasNodeSetArgs())
		{
			// Ensure that the node-set is the left argument
			if (tright is NodeSetType)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression temp = _right;
			Expression temp = _right;
			_right = _left;
			_left = temp;
			_op = (_op == Operators.GT) ? Operators.LT : (_op == Operators.LT) ? Operators.GT : (_op == Operators.GE) ? Operators.LE : Operators.GE;
			tright = _right.Type;
			}

			// Promote nodes to node sets
			if (tright is NodeType)
			{
			_right = new CastExpr(_right, Type.NodeSet);
			}
			// Promote integer to doubles to have fewer compares
			if (tright is IntType)
			{
			_right = new CastExpr(_right, Type.Real);
			}
			// Promote result-trees to strings
			if (tright is ResultTreeType)
			{
			_right = new CastExpr(_right, Type.String);
			}
			return _type = Type.Boolean;
		}

		// In the node-boolean case, convert node to boolean first
		if (hasNodeArgs())
		{
			if (tleft is BooleanType)
			{
			_right = new CastExpr(_right, Type.Boolean);
			tright = Type.Boolean;
			}
			if (tright is BooleanType)
			{
			_left = new CastExpr(_left, Type.Boolean);
			tleft = Type.Boolean;
			}
		}

		// Lookup the table of primops to find the best match
		MethodType ptype = lookupPrimop(stable, Operators.getOpNames(_op), new MethodType(Type.Void, tleft, tright));

		if (ptype != null)
		{
			Type arg1 = (Type) ptype.argsType()[0];
			if (!arg1.identicalTo(tleft))
			{
			_left = new CastExpr(_left, arg1);
			}
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
		if (hasNodeSetArgs() || hasReferenceArgs())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();

			// Call compare() from the BasisLibrary
			_left.translate(classGen, methodGen);
			_left.startIterator(classGen, methodGen);
			_right.translate(classGen, methodGen);
			_right.startIterator(classGen, methodGen);

			il.append(new PUSH(cpg, _op));
			il.append(methodGen.loadDOM());

			int index = cpg.addMethodref(BASIS_LIBRARY_CLASS, "compare", "(" + _left.Type.toSignature() + _right.Type.toSignature() + "I" + DOM_INTF_SIG + ")Z");
			il.append(new INVOKESTATIC(index));
		}
		else
		{
			translateDesynthesized(classGen, methodGen);
			synthesize(classGen, methodGen);
		}
		}

		public override void translateDesynthesized(ClassGenerator classGen, MethodGenerator methodGen)
		{
		if (hasNodeSetArgs() || hasReferenceArgs())
		{
			translate(classGen, methodGen);
			desynthesize(classGen, methodGen);
		}
		else
		{
			BranchInstruction bi = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();

			_left.translate(classGen, methodGen);
			_right.translate(classGen, methodGen);

			// TODO: optimize if one of the args is 0

			bool tozero = false;
			Type tleft = _left.Type;

			if (tleft is RealType)
			{
			il.append(tleft.CMP(_op == Operators.LT || _op == Operators.LE));
			tleft = Type.Int;
			tozero = true;
			}

			switch (_op)
			{
			case Operators.LT:
			bi = tleft.GE(tozero);
			break;

			case Operators.GT:
			bi = tleft.LE(tozero);
			break;

			case Operators.LE:
			bi = tleft.GT(tozero);
			break;

			case Operators.GE:
			bi = tleft.LT(tozero);
			break;

			default:
			ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_RELAT_OP_ERR,this);
			Parser.reportError(Constants.FATAL, msg);
		break;
			}

			_falseList.add(il.append(bi)); // must be backpatched
		}
		}

		public override string ToString()
		{
			return Operators.getOpNames(_op) + '(' + _left + ", " + _right + ')';
		}
	}

}