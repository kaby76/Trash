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
 * $Id: ReferenceType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using PUSH = org.apache.bcel.generic.PUSH;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;


	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	public sealed class ReferenceType : Type
	{
		protected internal ReferenceType()
		{
		}

		public override String ToString()
		{
		return "reference";
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override String toSignature()
		{
		return "Ljava/lang/Object;";
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return org.apache.bcel.generic.Type.OBJECT;
		}

		/// <summary>
		/// Translates a reference to an object of internal type <code>type</code>.
		/// The translation to int is undefined since references
		/// are always converted to reals in arithmetic expressions.
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
		else if (type == Type.Boolean)
		{
			translateTo(classGen, methodGen, (BooleanType) type);
		}
		else if (type == Type.NodeSet)
		{
			translateTo(classGen, methodGen, (NodeSetType) type);
		}
		else if (type == Type.Node)
		{
			translateTo(classGen, methodGen, (NodeType) type);
		}
		else if (type == Type.ResultTree)
		{
			translateTo(classGen, methodGen, (ResultTreeType) type);
		}
		else if (type == Type.Object)
		{
			translateTo(classGen, methodGen, (ObjectType) type);
		}
		else if (type == Type.Reference)
		{
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.INTERNAL_ERR, type.ToString());
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Translates reference into object of internal type <code>type</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, StringType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int current = methodGen.getLocalIndex("current");
		int current = methodGen.getLocalIndex("current");
		ConstantPoolGen cpg = classGen.ConstantPool;
		InstructionList il = methodGen.InstructionList;

		// If no current, conversion is a top-level
		if (current < 0)
		{
			il.append(new PUSH(cpg, org.apache.xml.dtm.DTM_Fields.ROOT_NODE)); // push root node
		}
		else
		{
			il.append(new ILOAD(current));
		}
		il.append(methodGen.loadDOM());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stringF = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "stringF", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG);
		int stringF = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "stringF", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG);
		il.append(new INVOKESTATIC(stringF));
		}

		/// <summary>
		/// Translates a reference into an object of internal type <code>type</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, RealType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		il.append(methodGen.loadDOM());
		int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "numberF", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")D");
		il.append(new INVOKESTATIC(index));
		}

		/// <summary>
		/// Translates a reference to an object of internal type <code>type</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "booleanF", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + ")Z");
		il.append(new INVOKESTATIC(index));
		}

		/// <summary>
		/// Casts a reference into a NodeIterator.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, NodeSetType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToNodeSet", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		il.append(new INVOKESTATIC(index));

		// Reset this iterator
		index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR, org.apache.xalan.xsltc.compiler.Constants_Fields.RESET, org.apache.xalan.xsltc.compiler.Constants_Fields.RESET_SIG);
		il.append(new INVOKEINTERFACE(index, 1));
		}

		/// <summary>
		/// Casts a reference into a Node.
		/// </summary>
		/// <seealso cref= org.apache.xalan.xsltc.compiler.util.Type#translateTo </seealso>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, NodeType type)
		{
		translateTo(classGen, methodGen, Type.NodeSet);
		Type.NodeSet.translateTo(classGen, methodGen, type);
		}

		/// <summary>
		/// Casts a reference into a ResultTree.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ResultTreeType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToResultTree", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG);
		il.append(new INVOKESTATIC(index));
		}

		/// <summary>
		/// Subsume reference into ObjectType.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ObjectType type)
		{
		methodGen.InstructionList.append(NOP);
		}

		/// <summary>
		/// Translates a reference into the Java type denoted by <code>clazz</code>.
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

			int referenceToLong = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToLong", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + ")J");
			int referenceToDouble = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToDouble", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + ")D");
			int referenceToBoolean = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToBoolean", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + ")Z");

		if (clazz.Name.Equals("java.lang.Object"))
		{
			il.append(NOP);
		}
		else if (clazz == Double.TYPE)
		{
			il.append(new INVOKESTATIC(referenceToDouble));
		}
		else if (clazz.Name.Equals("java.lang.Double"))
		{
			il.append(new INVOKESTATIC(referenceToDouble));
				Type.Real.translateTo(classGen, methodGen, Type.Reference);
		}
		else if (clazz == Float.TYPE)
		{
			il.append(new INVOKESTATIC(referenceToDouble));
				il.append(D2F);
		}
		else if (clazz.Name.Equals("java.lang.String"))
		{
			int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToString", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + "Ljava/lang/String;");
			il.append(methodGen.loadDOM());
			il.append(new INVOKESTATIC(index));
		}
		else if (clazz.Name.Equals("org.w3c.dom.Node"))
		{
			int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToNode", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + "Lorg/w3c/dom/Node;");
			il.append(methodGen.loadDOM());
			il.append(new INVOKESTATIC(index));
		}
		else if (clazz.Name.Equals("org.w3c.dom.NodeList"))
		{
			int index = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "referenceToNodeList", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + "Lorg/w3c/dom/NodeList;");
			il.append(methodGen.loadDOM());
			il.append(new INVOKESTATIC(index));
		}
		else if (clazz.Name.Equals("org.apache.xalan.xsltc.DOM"))
		{
			translateTo(classGen, methodGen, Type.ResultTree);
		}
		else if (clazz == Long.TYPE)
		{
			il.append(new INVOKESTATIC(referenceToLong));
		}
		else if (clazz == Integer.TYPE)
		{
			il.append(new INVOKESTATIC(referenceToLong));
				il.append(L2I);
		}
			else if (clazz == Short.TYPE)
			{
			il.append(new INVOKESTATIC(referenceToLong));
				il.append(L2I);
				il.append(I2S);
			}
			else if (clazz == Byte.TYPE)
			{
			il.append(new INVOKESTATIC(referenceToLong));
				il.append(L2I);
				il.append(I2B);
			}
			else if (clazz == Character.TYPE)
			{
			il.append(new INVOKESTATIC(referenceToLong));
				il.append(L2I);
				il.append(I2C);
			}
		else if (clazz == Boolean.TYPE)
		{
			il.append(new INVOKESTATIC(referenceToBoolean));
		}
		else if (clazz.Name.Equals("java.lang.Boolean"))
		{
			il.append(new INVOKESTATIC(referenceToBoolean));
				Type.Boolean.translateTo(classGen, methodGen, Type.Reference);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.Name);
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Translates an external Java type into a reference. Only conversion
		/// allowed is from java.lang.Object.
		/// </summary>
		public override void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{
		if (clazz.Name.Equals("java.lang.Object"))
		{
			methodGen.InstructionList.append(NOP);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.Name);
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Expects a reference on the stack and translates it to a non-synthesized
		/// boolean. It does not push a 0 or a 1 but instead returns branchhandle
		/// list to be appended to the false list.
		/// </summary>
		/// <seealso cref= org.apache.xalan.xsltc.compiler.util.Type#translateToDesynthesized </seealso>
		public override FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
		InstructionList il = methodGen.InstructionList;
		translateTo(classGen, methodGen, type);
		return new FlowList(il.append(new IFEQ(null)));
		}

		/// <summary>
		/// Translates an object of this type to its boxed representation.
		/// </summary>
		public override void translateBox(ClassGenerator classGen, MethodGenerator methodGen)
		{
		}

		/// <summary>
		/// Translates an object of this type to its unboxed representation.
		/// </summary>
		public override void translateUnBox(ClassGenerator classGen, MethodGenerator methodGen)
		{
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