using System;

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
 * $Id: IntType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using BranchInstruction = org.apache.bcel.generic.BranchInstruction;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IFGE = org.apache.bcel.generic.IFGE;
	using IFGT = org.apache.bcel.generic.IFGT;
	using IFLE = org.apache.bcel.generic.IFLE;
	using IFLT = org.apache.bcel.generic.IFLT;
	using IF_ICMPGE = org.apache.bcel.generic.IF_ICMPGE;
	using IF_ICMPGT = org.apache.bcel.generic.IF_ICMPGT;
	using IF_ICMPLE = org.apache.bcel.generic.IF_ICMPLE;
	using IF_ICMPLT = org.apache.bcel.generic.IF_ICMPLT;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionConstants = org.apache.bcel.generic.InstructionConstants;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class IntType : NumberType
	{
		protected internal IntType()
		{
		}

		public override String ToString()
		{
		return "int";
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override String toSignature()
		{
		return "I";
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return org.apache.bcel.generic.Type.INT;
		}

		/// <summary>
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#distanceTo
		/// </summary>
		public override int distanceTo(Type type)
		{
		if (type == this)
		{
			return 0;
		}
		else if (type == Type.Real)
		{
			return 1;
		}
		else
		{
			return int.MaxValue;
		}
		}

		/// <summary>
		/// Translates an integer into an object of internal type <code>type</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, final Type type)
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		if (type == Type.Real)
		{
			translateTo(classGen, methodGen, (RealType) type);
		}
		else if (type == Type.String)
		{
			translateTo(classGen, methodGen, (StringType) type);
		}
		else if (type == Type.Boolean)
		{
			translateTo(classGen, methodGen, (BooleanType) type);
		}
		else if (type == Type.Reference)
		{
			translateTo(classGen, methodGen, (ReferenceType) type);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Expects an integer on the stack and pushes a real.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, RealType type)
		{
		methodGen.InstructionList.append(I2D);
		}

		/// <summary>
		/// Expects an integer on the stack and pushes its string value by calling
		/// <code>Integer.toString(int i)</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, StringType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		il.append(new INVOKESTATIC(cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.INTEGER_CLASS, "toString", "(I)" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG)));
		}

		/// <summary>
		/// Expects an integer on the stack and pushes a 0 if its value is 0 and
		/// a 1 otherwise.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle falsec = il.append(new org.apache.bcel.generic.IFEQ(null));
		BranchHandle falsec = il.append(new IFEQ(null));
		il.append(ICONST_1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle truec = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle truec = il.append(new GOTO(null));
		falsec.Target = il.append(ICONST_0);
		truec.Target = il.append(NOP);
		}

		/// <summary>
		/// Expects an integer on the stack and translates it to a non-synthesized
		/// boolean. It does not push a 0 or a 1 but instead returns branchhandle 
		/// list to be appended to the false list.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateToDesynthesized
		/// </summary>
		public override FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		return new FlowList(il.append(new IFEQ(null)));
		}

		/// <summary>
		/// Expects an integer on the stack and pushes a boxed integer.
		/// Boxed integers are represented by an instance of
		/// <code>java.lang.Integer</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ReferenceType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		il.append(new NEW(cpg.addClass(org.apache.xalan.xsltc.compiler.Constants_Fields.INTEGER_CLASS)));
		il.append(DUP_X1);
		il.append(SWAP);
		il.append(new INVOKESPECIAL(cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.INTEGER_CLASS, "<init>", "(I)V")));
		}

		/// <summary>
		/// Translates an integer into the Java type denoted by <code>clazz</code>. 
		/// Expects an integer on the stack and pushes a number of the appropriate
		/// type after coercion.
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		if (clazz == Character.TYPE)
		{
			il.append(I2C);
		}
		else if (clazz == Byte.TYPE)
		{
			il.append(I2B);
		}
		else if (clazz == Short.TYPE)
		{
			il.append(I2S);
		}
		else if (clazz == Integer.TYPE)
		{
			il.append(NOP);
		}
		else if (clazz == Long.TYPE)
		{
			il.append(I2L);
		}
		else if (clazz == Float.TYPE)
		{
			il.append(I2F);
		}
		else if (clazz == Double.TYPE)
		{
			il.append(I2D);
		}
			 // Is Double <: clazz? I.e. clazz in { Double, Number, Object }
		   else if (typeof(Double).IsSubclassOf(clazz))
		   {
			   il.append(I2D);
			   Type.Real.translateTo(classGen, methodGen, Type.Reference);
		   }
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.Name);
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Translates an object of this type to its boxed representation.
		/// </summary>
		public override void translateBox(ClassGenerator classGen, MethodGenerator methodGen)
		{
		translateTo(classGen, methodGen, Type.Reference);
		}

		/// <summary>
		/// Translates an object of this type to its unboxed representation.
		/// </summary>
		public override void translateUnBox(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		il.append(new CHECKCAST(cpg.addClass(org.apache.xalan.xsltc.compiler.Constants_Fields.INTEGER_CLASS)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.INTEGER_CLASS, org.apache.xalan.xsltc.compiler.Constants_Fields.INT_VALUE, org.apache.xalan.xsltc.compiler.Constants_Fields.INT_VALUE_SIG);
		int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.INTEGER_CLASS, org.apache.xalan.xsltc.compiler.Constants_Fields.INT_VALUE, org.apache.xalan.xsltc.compiler.Constants_Fields.INT_VALUE_SIG);
		il.append(new INVOKEVIRTUAL(index));
		}

		public override Instruction ADD()
		{
		return InstructionConstants.IADD;
		}

		public override Instruction SUB()
		{
		return InstructionConstants.ISUB;
		}

		public override Instruction MUL()
		{
		return InstructionConstants.IMUL;
		}

		public override Instruction DIV()
		{
		return InstructionConstants.IDIV;
		}

		public override Instruction REM()
		{
		return InstructionConstants.IREM;
		}

		public override Instruction NEG()
		{
		return InstructionConstants.INEG;
		}

		public override Instruction LOAD(int slot)
		{
		return new ILOAD(slot);
		}

		public override Instruction STORE(int slot)
		{
		return new ISTORE(slot);
		}

		public override BranchInstruction GT(bool tozero)
		{
		return tozero ? (BranchInstruction) new IFGT(null) : (BranchInstruction) new IF_ICMPGT(null);
		}

		public override BranchInstruction GE(bool tozero)
		{
		return tozero ? (BranchInstruction) new IFGE(null) : (BranchInstruction) new IF_ICMPGE(null);
		}

		public override BranchInstruction LT(bool tozero)
		{
		return tozero ? (BranchInstruction) new IFLT(null) : (BranchInstruction) new IF_ICMPLT(null);
		}

		public override BranchInstruction LE(bool tozero)
		{
		return tozero ? (BranchInstruction) new IFLE(null) : (BranchInstruction) new IF_ICMPLE(null);
		}
	}

}