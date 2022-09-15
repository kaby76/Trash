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
 * $Id: NodeSetType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFLT = org.apache.bcel.generic.IFLT;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class NodeSetType : Type
	{
		protected internal NodeSetType()
		{
		}

		public override String ToString()
		{
		return "node-set";
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override String toSignature()
		{
		return org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG;
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return new org.apache.bcel.generic.ObjectType(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR);
		}

		/// <summary>
		/// Translates a node-set into an object of internal type
		/// <code>type</code>. The translation to int is undefined
		/// since node-sets are always converted to
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
		else if (type == Type.Boolean)
		{
			translateTo(classGen, methodGen, (BooleanType) type);
		}
		else if (type == Type.Real)
		{
			translateTo(classGen, methodGen, (RealType) type);
		}
		else if (type == Type.Node)
		{
			translateTo(classGen, methodGen, (NodeType) type);
		}
		else if (type == Type.Reference)
		{
			translateTo(classGen, methodGen, (ReferenceType) type);
		}
		else if (type == Type.Object)
		{
			translateTo(classGen, methodGen, (ObjectType) type);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Translates an external Java Class into an internal type.
		/// Expects the Java object on the stack, pushes the internal type
		/// </summary>
		public override void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{

		  InstructionList il = methodGen.InstructionList;
		ConstantPoolGen cpg = classGen.ConstantPool;
		if (clazz.Name.Equals("org.w3c.dom.NodeList"))
		{
		   // w3c NodeList is on the stack from the external Java function call.
		   // call BasisFunction to consume NodeList and leave Iterator on
		   //    the stack. 
		   il.append(classGen.loadTranslet()); // push translet onto stack
		   il.append(methodGen.loadDOM()); // push DOM onto stack
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int convert = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "nodeList2Iterator", "(" + "Lorg/w3c/dom/NodeList;" + org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_INTF_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		   int convert = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "nodeList2Iterator", "(" + "Lorg/w3c/dom/NodeList;" + org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_INTF_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		   il.append(new INVOKESTATIC(convert));
		}
		else if (clazz.Name.Equals("org.w3c.dom.Node"))
		{
		   // w3c Node is on the stack from the external Java function call.
		   // call BasisLibrary.node2Iterator() to consume Node and leave 
		   // Iterator on the stack. 
		   il.append(classGen.loadTranslet()); // push translet onto stack
		   il.append(methodGen.loadDOM()); // push DOM onto stack
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int convert = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "node2Iterator", "(" + "Lorg/w3c/dom/Node;" + org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_INTF_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		   int convert = cpg.addMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.BASIS_LIBRARY_CLASS, "node2Iterator", "(" + "Lorg/w3c/dom/Node;" + org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_INTF_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + ")" + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		   il.append(new INVOKESTATIC(convert));
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.Name);
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}


		/// <summary>
		/// Translates a node-set into a synthesized boolean.
		/// The boolean value of a node-set is "true" if non-empty
		/// and "false" otherwise. Notice that the 
		/// function getFirstNode() is called in translateToDesynthesized().
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		FlowList falsel = translateToDesynthesized(classGen, methodGen, type);
		il.append(ICONST_1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle truec = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle truec = il.append(new GOTO(null));
		falsel.backPatch(il.append(ICONST_0));
		truec.Target = il.append(NOP);
		}

		/// <summary>
		/// Translates a node-set into a string. The string value of a node-set is
		/// value of its first element.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, StringType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		getFirstNode(classGen, methodGen);
		il.append(DUP);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle falsec = il.append(new org.apache.bcel.generic.IFLT(null));
		BranchHandle falsec = il.append(new IFLT(null));
		Type.Node.translateTo(classGen, methodGen, type);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle truec = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle truec = il.append(new GOTO(null));
		falsec.Target = il.append(POP);
		il.append(new PUSH(classGen.ConstantPool, ""));
		truec.Target = il.append(NOP);
		}

		/// <summary>
		/// Expects a node-set on the stack and pushes a real.
		/// First the node-set is converted to string, and from string to real.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, RealType type)
		{
		translateTo(classGen, methodGen, Type.String);
		Type.String.translateTo(classGen, methodGen, Type.Real);
		}

		/// <summary>
		/// Expects a node-set on the stack and pushes a node.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, NodeType type)
		{
		getFirstNode(classGen, methodGen);
		}

		/// <summary>
		/// Subsume node-set into ObjectType.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ObjectType type)
		{
			methodGen.InstructionList.append(NOP);
		}

		/// <summary>
		/// Translates a node-set into a non-synthesized boolean. It does not 
		/// push a 0 or a 1 but instead returns branchhandle list to be appended 
		/// to the false list.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateToDesynthesized
		/// </summary>
		public override FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		getFirstNode(classGen, methodGen);
		return new FlowList(il.append(new IFLT(null)));
		}

		/// <summary>
		/// Expects a node-set on the stack and pushes a boxed node-set.
		/// Node sets are already boxed so the translation is just a NOP.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ReferenceType type)
		{
		methodGen.InstructionList.append(NOP);
		}

		/// <summary>
		/// Translates a node-set into the Java type denoted by <code>clazz</code>. 
		/// Expects a node-set on the stack and pushes an object of the appropriate
		/// type after coercion.
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = clazz.getName();
		string className = clazz.Name;

		il.append(methodGen.loadDOM());
		il.append(SWAP);

		if (className.Equals("org.w3c.dom.Node"))
		{
			int index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF, org.apache.xalan.xsltc.compiler.Constants_Fields.MAKE_NODE, org.apache.xalan.xsltc.compiler.Constants_Fields.MAKE_NODE_SIG2);
			il.append(new INVOKEINTERFACE(index, 2));
		}
			else if (className.Equals("org.w3c.dom.NodeList") || className.Equals("java.lang.Object"))
			{
			int index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF, org.apache.xalan.xsltc.compiler.Constants_Fields.MAKE_NODE_LIST, org.apache.xalan.xsltc.compiler.Constants_Fields.MAKE_NODE_LIST_SIG2);
			il.append(new INVOKEINTERFACE(index, 2));
			}
			else if (className.Equals("java.lang.String"))
			{
				int next = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR, "next", "()I");
				int index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF, org.apache.xalan.xsltc.compiler.Constants_Fields.GET_NODE_VALUE, "(I)" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG);

				// Get next node from the iterator
				il.append(new INVOKEINTERFACE(next, 1));
				// Get the node's string value (from the DOM)
				il.append(new INVOKEINTERFACE(index, 2));

			}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), className);
			classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}
		}

		/// <summary>
		/// Some type conversions require gettting the first node from the node-set.
		/// This function is defined to avoid code repetition.
		/// </summary>
		private void getFirstNode(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		il.append(new INVOKEINTERFACE(cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR, org.apache.xalan.xsltc.compiler.Constants_Fields.NEXT, org.apache.xalan.xsltc.compiler.Constants_Fields.NEXT_SIG), 1));
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
		methodGen.InstructionList.append(NOP);
		}

		/// <summary>
		/// Returns the class name of an internal type's external representation.
		/// </summary>
		public override String ClassName
		{
			get
			{
			return (org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR);
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