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
 * $Id: BooleanType.java 468649 2006-10-28 07:00:55Z minchau $
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
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using Constants = org.apache.xalan.xsltc.compiler.Constants;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class BooleanType : Type
	{
		protected internal BooleanType()
		{
		}

		public override string ToString()
		{
		return "boolean";
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override string toSignature()
		{
		return "Z";
		}

		public override bool Simple
		{
			get
			{
			return true;
			}
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return org.apache.bcel.generic.Type.BOOLEAN;
		}

		/// <summary>
		/// Translates a real into an object of internal type <code>type</code>. The
		/// translation to int is undefined since booleans are always converted to
		/// reals in arithmetic expressions.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		if (type == Type.String)
		{
			translateTo(classGen, methodGen, (StringType) type);
		}
		else if (type == Type.Real)
		{
			translateTo(classGen, methodGen, (RealType) type);
		}
		else if (type == Type.Reference)
		{
			translateTo(classGen, methodGen, (ReferenceType) type);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Expects a boolean on the stack and pushes a string. If the value on the
		/// stack is zero, then the string 'false' is pushed. Otherwise, the string
		/// 'true' is pushed.
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle falsec = il.append(new org.apache.bcel.generic.IFEQ(null));
		BranchHandle falsec = il.append(new IFEQ(null));
		il.append(new PUSH(cpg, "true"));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle truec = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle truec = il.append(new GOTO(null));
		falsec.setTarget(il.append(new PUSH(cpg, "false")));
		truec.setTarget(il.append(NOP));
		}

		/// <summary>
		/// Expects a boolean on the stack and pushes a real. The value "true" is
		/// converted to 1.0 and the value "false" to 0.0.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, RealType type)
		{
		methodGen.getInstructionList().append(I2D);
		}

		/// <summary>
		/// Expects a boolean on the stack and pushes a boxed boolean.
		/// Boxed booleans are represented by an instance of
		/// <code>java.lang.Boolean</code>.
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
		il.append(new NEW(cpg.addClass(BOOLEAN_CLASS)));
		il.append(DUP_X1);
		il.append(SWAP);
		il.append(new INVOKESPECIAL(cpg.addMethodref(BOOLEAN_CLASS, "<init>", "(Z)V")));
		}

		/// <summary>
		/// Translates an internal boolean into an external (Java) boolean.
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
		if (clazz == Boolean.TYPE)
		{
			methodGen.getInstructionList().append(NOP);
		}
			// Is Boolean <: clazz? I.e. clazz in { Boolean, Object }
			else if (clazz.IsAssignableFrom(typeof(Boolean)))
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
		/// Translates an external (Java) boolean into internal boolean.
		/// </summary>
		public override void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
		translateTo(classGen, methodGen, clazz);
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
		il.append(new CHECKCAST(cpg.addClass(BOOLEAN_CLASS)));
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(BOOLEAN_CLASS, BOOLEAN_VALUE, BOOLEAN_VALUE_SIG)));
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