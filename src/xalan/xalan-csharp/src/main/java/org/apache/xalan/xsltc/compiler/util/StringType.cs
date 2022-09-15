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
 * $Id: StringType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using IFNONNULL = org.apache.bcel.generic.IFNONNULL;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using Constants = org.apache.xalan.xsltc.compiler.Constants;
	using FlowList = org.apache.xalan.xsltc.compiler.FlowList;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class StringType : Type
	{
		protected internal StringType()
		{
		}

		public override string ToString()
		{
		return "string";
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override string toSignature()
		{
		return "Ljava/lang/String;";
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
		return org.apache.bcel.generic.Type.STRING;
		}

		/// <summary>
		/// Translates a string into an object of internal type <code>type</code>.
		/// The translation to int is undefined since strings are always converted
		/// to reals in arithmetic expressions.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		if (type == Type.Boolean)
		{
			translateTo(classGen, methodGen, (BooleanType) type);
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
		/// Translates a string into a synthesized boolean.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public virtual void translateTo(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
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
		/// Translates a string into a real by calling stringToReal() from the
		/// basis library.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public virtual void translateTo(ClassGenerator classGen, MethodGenerator methodGen, RealType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		il.append(new INVOKESTATIC(cpg.addMethodref(BASIS_LIBRARY_CLASS, STRING_TO_REAL, STRING_TO_REAL_SIG)));
		}

		/// <summary>
		/// Translates a string into a non-synthesized boolean. It does not push a 
		/// 0 or a 1 but instead returns branchhandle list to be appended to the 
		/// false list.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateToDesynthesized
		/// </summary>
		public override FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		il.append(new INVOKEVIRTUAL(cpg.addMethodref(STRING_CLASS, "length", "()I")));
		return new FlowList(il.append(new IFEQ(null)));
		}

		/// <summary>
		/// Expects a string on the stack and pushes a boxed string.
		/// Strings are already boxed so the translation is just a NOP.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public virtual void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ReferenceType type)
		{
		methodGen.getInstructionList().append(NOP);
		}

		/// <summary>
		/// Translates a internal string into an external (Java) string. 
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateFrom
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
			// Is String <: clazz? I.e. clazz in { String, Object }
			if (clazz.IsAssignableFrom(typeof(string)))
			{
			methodGen.getInstructionList().append(NOP);
			}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.FullName);
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Translates an external (primitive) Java type into a string. 
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateFrom
		/// </summary>
		public override void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		if (clazz.FullName.Equals("java.lang.String"))
		{
			// same internal representation, convert null to ""
			il.append(DUP);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifNonNull = il.append(new org.apache.bcel.generic.IFNONNULL(null));
			BranchHandle ifNonNull = il.append(new IFNONNULL(null));
			il.append(POP);
			il.append(new PUSH(cpg, ""));
			ifNonNull.setTarget(il.append(NOP));
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
		methodGen.getInstructionList().append(NOP);
		}

		/// <summary>
		/// Returns the class name of an internal type's external representation.
		/// </summary>
		public override string ClassName
		{
			get
			{
			return (STRING_CLASS);
			}
		}


		public override Instruction LOAD(int slot)
		{
		return new ALOAD(slot);
		}

		public override Instruction STORE(int slot)
		{
		return new ASTORE(slot);
		}
	}

}