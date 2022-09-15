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
 * $Id: CastExpr.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using IF_ICMPNE = org.apache.bcel.generic.IF_ICMPNE;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using SIPUSH = org.apache.bcel.generic.SIPUSH;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using MultiHashtable = org.apache.xalan.xsltc.compiler.util.MultiHashtable;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using ResultTreeType = org.apache.xalan.xsltc.compiler.util.ResultTreeType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Axis = org.apache.xml.dtm.Axis;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal sealed class CastExpr : Expression
	{
		private readonly Expression _left;

		/// <summary>
		/// Legal conversions between internal types.
		/// </summary>
		private static MultiHashtable InternalTypeMap = new MultiHashtable();

		static CastExpr()
		{
		// Possible type conversions between internal types
		InternalTypeMap[Type.Boolean] = Type.Boolean;
		InternalTypeMap[Type.Boolean] = Type.Real;
		InternalTypeMap[Type.Boolean] = Type.String;
		InternalTypeMap[Type.Boolean] = Type.Reference;
			InternalTypeMap[Type.Boolean] = Type.Object;

		InternalTypeMap[Type.Real] = Type.Real;
		InternalTypeMap[Type.Real] = Type.Int;
		InternalTypeMap[Type.Real] = Type.Boolean;
		InternalTypeMap[Type.Real] = Type.String;
		InternalTypeMap[Type.Real] = Type.Reference;
			InternalTypeMap[Type.Real] = Type.Object;

		InternalTypeMap[Type.Int] = Type.Int;
		InternalTypeMap[Type.Int] = Type.Real;
		InternalTypeMap[Type.Int] = Type.Boolean;
		InternalTypeMap[Type.Int] = Type.String;
		InternalTypeMap[Type.Int] = Type.Reference;
			InternalTypeMap[Type.Int] = Type.Object;

		InternalTypeMap[Type.String] = Type.String;
		InternalTypeMap[Type.String] = Type.Boolean;
		InternalTypeMap[Type.String] = Type.Real;
		InternalTypeMap[Type.String] = Type.Reference;
			InternalTypeMap[Type.String] = Type.Object;

		InternalTypeMap[Type.NodeSet] = Type.NodeSet;
		InternalTypeMap[Type.NodeSet] = Type.Boolean;
		InternalTypeMap[Type.NodeSet] = Type.Real;
		InternalTypeMap[Type.NodeSet] = Type.String;
		InternalTypeMap[Type.NodeSet] = Type.Node;
		InternalTypeMap[Type.NodeSet] = Type.Reference;
		InternalTypeMap[Type.NodeSet] = Type.Object;

		InternalTypeMap[Type.Node] = Type.Node;
		InternalTypeMap[Type.Node] = Type.Boolean;
		InternalTypeMap[Type.Node] = Type.Real;
		InternalTypeMap[Type.Node] = Type.String;
		InternalTypeMap[Type.Node] = Type.NodeSet;
		InternalTypeMap[Type.Node] = Type.Reference;
		InternalTypeMap[Type.Node] = Type.Object;

		InternalTypeMap[Type.ResultTree] = Type.ResultTree;
		InternalTypeMap[Type.ResultTree] = Type.Boolean;
		InternalTypeMap[Type.ResultTree] = Type.Real;
		InternalTypeMap[Type.ResultTree] = Type.String;
		InternalTypeMap[Type.ResultTree] = Type.NodeSet;
		InternalTypeMap[Type.ResultTree] = Type.Reference;
		InternalTypeMap[Type.ResultTree] = Type.Object;

		InternalTypeMap[Type.Reference] = Type.Reference;
		InternalTypeMap[Type.Reference] = Type.Boolean;
		InternalTypeMap[Type.Reference] = Type.Int;
		InternalTypeMap[Type.Reference] = Type.Real;
		InternalTypeMap[Type.Reference] = Type.String;
		InternalTypeMap[Type.Reference] = Type.Node;
		InternalTypeMap[Type.Reference] = Type.NodeSet;
		InternalTypeMap[Type.Reference] = Type.ResultTree;
		InternalTypeMap[Type.Reference] = Type.Object;

		InternalTypeMap[Type.Object] = Type.String;

		InternalTypeMap[Type.Void] = Type.String;
		}

		private bool _typeTest = false;

		/// <summary>
		/// Construct a cast expression and check that the conversion is 
		/// valid by calling typeCheck().
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public CastExpr(Expression left, org.apache.xalan.xsltc.compiler.util.Type type) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public CastExpr(Expression left, Type type)
		{
		_left = left;
		_type = type; // use inherited field

		if ((_left is Step) && (_type == Type.Boolean))
		{
			Step step = (Step)_left;
			if ((step.Axis == Axis.SELF) && (step.NodeType != -1))
			{
			_typeTest = true;
			}
		}

		// check if conversion is valid
		Parser = left.Parser;
		Parent = left.Parent;
		left.Parent = this;
		typeCheck(left.Parser.getSymbolTable());
		}

		public Expression Expr
		{
			get
			{
			return _left;
			}
		}

		/// <summary>
		/// Returns true if this expressions contains a call to position(). This is
		/// needed for context changes in node steps containing multiple predicates.
		/// </summary>
		public override bool hasPositionCall()
		{
		return (_left.hasPositionCall());
		}

		public override bool hasLastCall()
		{
		return (_left.hasLastCall());
		}

		public override string ToString()
		{
		return "cast(" + _left + ", " + _type + ")";
		}

		/// <summary>
		/// Type checking a cast expression amounts to verifying that the  
		/// type conversion is legal. Cast expressions are created during 
		/// type checking, but typeCheck() is usually not called on them. 
		/// As a result, this method is called from the constructor.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		Type tleft = _left.Type;
		if (tleft == null)
		{
			tleft = _left.typeCheck(stable);
		}
		if (tleft is NodeType)
		{
			tleft = Type.Node; // multiple instances
		}
		else if (tleft is ResultTreeType)
		{
			tleft = Type.ResultTree; // multiple instances
		}
		if (InternalTypeMap.maps(tleft, _type) != null)
		{
			return _type;
		}
		// throw new TypeCheckError(this);	
		throw new TypeCheckError(new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, tleft.ToString(), _type.ToString()));
		}

		public override void translateDesynthesized(ClassGenerator classGen, MethodGenerator methodGen)
		{
		FlowList fl;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type ltype = _left.getType();
		Type ltype = _left.Type;

		// This is a special case for the self:: axis. Instead of letting
		// the Step object create and iterator that we cast back to a single
		// node, we simply ask the DOM for the node type.
		if (_typeTest)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idx = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
			int idx = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
			il.append(new SIPUSH((short)((Step)_left).getNodeType()));
			il.append(methodGen.loadDOM());
			il.append(methodGen.loadContextNode());
			il.append(new INVOKEINTERFACE(idx, 2));
			_falseList.add(il.append(new IF_ICMPNE(null)));
		}
		else
		{

			_left.translate(classGen, methodGen);
			if (_type != ltype)
			{
			_left.startIterator(classGen, methodGen);
			if (_type is BooleanType)
			{
				fl = ltype.translateToDesynthesized(classGen, methodGen, _type);
				if (fl != null)
				{
				_falseList.append(fl);
				}
			}
			else
			{
				ltype.translateTo(classGen, methodGen, _type);
			}
			}
		}
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type ltype = _left.getType();
		Type ltype = _left.Type;
		_left.translate(classGen, methodGen);
		if (_type.identicalTo(ltype) == false)
		{
			_left.startIterator(classGen, methodGen);
			ltype.translateTo(classGen, methodGen, _type);
		}
		}
	}

}