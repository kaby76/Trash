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
 * $Id: RealType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using DLOAD = org.apache.bcel.generic.DLOAD;
	using DSTORE = org.apache.bcel.generic.DSTORE;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IFNE = org.apache.bcel.generic.IFNE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionConstants = org.apache.bcel.generic.InstructionConstants;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using Constants = org.apache.xalan.xsltc.compiler.Constants;
	using FlowList = org.apache.xalan.xsltc.compiler.FlowList;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class RealType : NumberType
	{
		protected internal RealType()
		{
		}

		public override string ToString()
		{
		return "real";
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override string toSignature()
		{
		return "D";
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return org.apache.bcel.generic.Type.DOUBLE;
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
		else if (type == Type.Int)
		{
			return 1;
		}
		else
		{
			return int.MaxValue;
		}
		}

		/// <summary>
		/// Translates a real into an object of internal type <code>type</code>. The
		/// translation to int is undefined since reals are never converted to ints.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		if (type == Type.String)
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
		else if (type == Type.Int)
		{
			translateTo(classGen, methodGen, (IntType) type);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Expects a real on the stack and pushes its string value by calling
		/// <code>Double.toString(double d)</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, StringType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		il.append(new INVOKESTATIC(cpg.addMethodref(BASIS_LIBRARY_CLASS, "realToString", "(D)" + STRING_SIG)));
		}

		/// <summary>
		/// Expects a real on the stack and pushes a 0 if that number is 0.0 and
		/// a 1 otherwise.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		FlowList falsel = translateToDesynthesized(classGen, methodGen, type);
		il.append(ICONST_1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle truec = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle truec = il.append(new GOTO(null));
		falsel.backPatch(il.append(ICONST_0));
		truec.setTarget(il.append(NOP));
		}

		/// <summary>
		/// Expects a real on the stack and pushes a truncated integer value
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, IntType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		il.append(new INVOKESTATIC(cpg.addMethodref(BASIS_LIBRARY_CLASS, "realToInt","(D)I")));
		}

		/// <summary>
		/// Translates a real into a non-synthesized boolean. It does not push a 
		/// 0 or a 1 but instead returns branchhandle list to be appended to the 
		/// false list. A NaN must be converted to "false".
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateToDesynthesized
		/// </summary>
		public override FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
		LocalVariableGen local;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.FlowList flowlist = new org.apache.xalan.xsltc.compiler.FlowList();
		FlowList flowlist = new FlowList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// Store real into a local variable
		il.append(DUP2);
		local = methodGen.addLocalVariable("real_to_boolean_tmp", org.apache.bcel.generic.Type.DOUBLE, null, null);
		local.setStart(il.append(new DSTORE(local.getIndex())));

		// Compare it to 0.0
		il.append(DCONST_0);
		il.append(DCMPG);
		flowlist.add(il.append(new IFEQ(null)));

		//!!! call isNaN
		// Compare it to itself to see if NaN
		il.append(new DLOAD(local.getIndex()));
		local.setEnd(il.append(new DLOAD(local.getIndex())));
		il.append(DCMPG);
		flowlist.add(il.append(new IFNE(null))); // NaN != NaN
		return flowlist;
		}

		/// <summary>
		/// Expects a double on the stack and pushes a boxed double. Boxed 
		/// double are represented by an instance of <code>java.lang.Double</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ReferenceType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		il.append(new NEW(cpg.addClass(DOUBLE_CLASS)));
		il.append(DUP_X2);
		il.append(DUP_X2);
		il.append(POP);
		il.append(new INVOKESPECIAL(cpg.addMethodref(DOUBLE_CLASS, "<init>", "(D)V")));
		}

		/// <summary>
		/// Translates a real into the Java type denoted by <code>clazz</code>. 
		/// Expects a real on the stack and pushes a number of the appropriate
		/// type after coercion.
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, in System.Type clazz)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		if (clazz == Character.TYPE)
		{
			il.append(D2I);
			il.append(I2C);
		}
		else if (clazz == Byte.TYPE)
		{
			il.append(D2I);
			il.append(I2B);
		}
		else if (clazz == Short.TYPE)
		{
			il.append(D2I);
			il.append(I2S);
		}
		else if (clazz == Integer.TYPE)
		{
			il.append(D2I);
		}
		else if (clazz == Long.TYPE)
		{
			il.append(D2L);
		}
		else if (clazz == Float.TYPE)
		{
			il.append(D2F);
		}
		else if (clazz == Double.TYPE)
		{
			il.append(NOP);
		}
			// Is Double <: clazz? I.e. clazz in { Double, Number, Object }
			else if (clazz.IsAssignableFrom(typeof(Double)))
			{
				translateTo(classGen, methodGen, Type.Reference);
			}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.FullName);
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Translates an external (primitive) Java type into a real. Expects a java 
		/// object on the stack and pushes a real (i.e., a double).
		/// </summary>
		public override void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
		InstructionList il = methodGen.getInstructionList();

		if (clazz == Character.TYPE || clazz == Byte.TYPE || clazz == Short.TYPE || clazz == Integer.TYPE)
		{
			il.append(I2D);
		}
		else if (clazz == Long.TYPE)
		{
			il.append(L2D);
		}
		else if (clazz == Float.TYPE)
		{
			il.append(F2D);
		}
		else if (clazz == Double.TYPE)
		{
			il.append(NOP);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.FullName);
			classGen.Parser.reportError(Constants.FATAL, err);
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
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		il.append(new CHECKCAST(cpg.addClass(DOUBLE_CLASS)));
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(DOUBLE_CLASS, DOUBLE_VALUE, DOUBLE_VALUE_SIG)));
		}

		public override Instruction ADD()
		{
		return InstructionConstants.DADD;
		}

		public override Instruction SUB()
		{
		return InstructionConstants.DSUB;
		}

		public override Instruction MUL()
		{
		return InstructionConstants.DMUL;
		}

		public override Instruction DIV()
		{
		return InstructionConstants.DDIV;
		}

		public override Instruction REM()
		{
		return InstructionConstants.DREM;
		}

		public override Instruction NEG()
		{
		return InstructionConstants.DNEG;
		}

		public override Instruction LOAD(int slot)
		{
		return new DLOAD(slot);
		}

		public override Instruction STORE(int slot)
		{
		return new DSTORE(slot);
		}

		public override Instruction POP()
		{
		return POP2;
		}

		public override Instruction CMP(bool less)
		{
		return less ? InstructionConstants.DCMPG : InstructionConstants.DCMPL;
		}

		public override Instruction DUP()
		{
		return DUP2;
		}
	}


}