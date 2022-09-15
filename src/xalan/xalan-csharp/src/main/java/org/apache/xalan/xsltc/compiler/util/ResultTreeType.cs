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
 * $Id: ResultTreeType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using Constants = org.apache.xalan.xsltc.compiler.Constants;
	using FlowList = org.apache.xalan.xsltc.compiler.FlowList;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class ResultTreeType : Type
	{
		private readonly string _methodName;

		protected internal ResultTreeType()
		{
		_methodName = null;
		}

		public ResultTreeType(string methodName)
		{
		_methodName = methodName;
		}

		public override string ToString()
		{
		return "result-tree";
		}

		public override bool identicalTo(Type other)
		{
		return (other is ResultTreeType);
		}

		public override string toSignature()
		{
		return DOM_INTF_SIG;
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return Util.getJCRefType(toSignature());
		}

		public string MethodName
		{
			get
			{
			return _methodName;
			}
		}

		public override bool implementedAsMethod()
		{
		return !string.ReferenceEquals(_methodName, null);
		}

		/// <summary>
		/// Translates a result tree to object of internal type <code>type</code>. 
		/// The translation to int is undefined since result trees
		/// are always converted to reals in arithmetic expressions.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of the type to translate the result tree to </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		if (type == Type.String)
		{
			translateTo(classGen, methodGen, (StringType)type);
		}
		else if (type == Type.Boolean)
		{
			translateTo(classGen, methodGen, (BooleanType)type);
		}
		else if (type == Type.Real)
		{
			translateTo(classGen, methodGen, (RealType)type);
		}
		else if (type == Type.NodeSet)
		{
			translateTo(classGen, methodGen, (NodeSetType)type);
		}
		else if (type == Type.Reference)
		{
			translateTo(classGen, methodGen, (ReferenceType)type);
		}
		else if (type == Type.Object)
		{
			translateTo(classGen, methodGen, (ObjectType) type);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Expects an result tree on the stack and pushes a boolean.
		/// Translates a result tree to a boolean by first converting it to string.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of BooleanType (any) </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
		// A result tree is always 'true' when converted to a boolean value,
		// since the tree always has at least one node (the root).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		il.append(POP); // don't need the DOM reference
		il.append(ICONST_1); // push 'true' on the stack
		}

		/// <summary>
		/// Expects an result tree on the stack and pushes a string.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of StringType (any) </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, StringType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (string.ReferenceEquals(_methodName, null))
		{
			int index = cpg.addInterfaceMethodref(DOM_INTF, "getStringValue", "()" + STRING_SIG);
			il.append(new INVOKEINTERFACE(index, 1));
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getClassName();
			string className = classGen.ClassName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int current = methodGen.getLocalIndex("current");
			int current = methodGen.getLocalIndex("current");

			// Push required parameters 
			il.append(classGen.loadTranslet());
			if (classGen.External)
			{
			il.append(new CHECKCAST(cpg.addClass(className)));
			}
			il.append(DUP);
			il.append(new GETFIELD(cpg.addFieldref(className, "_dom", DOM_INTF_SIG)));

			// Create a new instance of a StringValueHandler
			int index = cpg.addMethodref(STRING_VALUE_HANDLER, "<init>", "()V");
			il.append(new NEW(cpg.addClass(STRING_VALUE_HANDLER)));
			il.append(DUP);
			il.append(DUP);
			il.append(new INVOKESPECIAL(index));

			// Store new Handler into a local variable
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen handler = methodGen.addLocalVariable("rt_to_string_handler", Util.getJCRefType(STRING_VALUE_HANDLER_SIG), null, null);
			LocalVariableGen handler = methodGen.addLocalVariable("rt_to_string_handler", Util.getJCRefType(STRING_VALUE_HANDLER_SIG), null, null);
			handler.setStart(il.append(new ASTORE(handler.getIndex())));

			// Call the method that implements this result tree
			index = cpg.addMethodref(className, _methodName, "(" + DOM_INTF_SIG + TRANSLET_OUTPUT_SIG + ")V");
			il.append(new INVOKEVIRTUAL(index));

			// Restore new handler and call getValue()
			handler.setEnd(il.append(new ALOAD(handler.getIndex())));
			index = cpg.addMethodref(STRING_VALUE_HANDLER, "getValue", "()" + STRING_SIG);
			il.append(new INVOKEVIRTUAL(index));
		}
		}

		/// <summary>
		/// Expects an result tree on the stack and pushes a real.
		/// Translates a result tree into a real by first converting it to string.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of RealType (any) </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, RealType type)
		{
		translateTo(classGen, methodGen, Type.String);
		Type.String.translateTo(classGen, methodGen, Type.Real);
		}

		/// <summary>
		/// Expects a result tree on the stack and pushes a boxed result tree.
		/// Result trees are already boxed so the translation is just a NOP.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of ReferenceType (any) </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ReferenceType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (string.ReferenceEquals(_methodName, null))
		{
			il.append(NOP);
		}
		else
		{
			LocalVariableGen domBuilder, newDom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getClassName();
			string className = classGen.ClassName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int current = methodGen.getLocalIndex("current");
			int current = methodGen.getLocalIndex("current");

			// Push required parameters 
			il.append(classGen.loadTranslet());
			if (classGen.External)
			{
			il.append(new CHECKCAST(cpg.addClass(className)));
			}
			il.append(methodGen.loadDOM());

			// Create new instance of DOM class (with RTF_INITIAL_SIZE nodes)
			il.append(methodGen.loadDOM());
			int index = cpg.addInterfaceMethodref(DOM_INTF, "getResultTreeFrag", "(IZ)" + DOM_INTF_SIG);
			il.append(new PUSH(cpg, RTF_INITIAL_SIZE));
			il.append(new PUSH(cpg, false));
			il.append(new INVOKEINTERFACE(index,3));
			il.append(DUP);

			// Store new DOM into a local variable
			newDom = methodGen.addLocalVariable("rt_to_reference_dom", Util.getJCRefType(DOM_INTF_SIG), null, null);
			il.append(new CHECKCAST(cpg.addClass(DOM_INTF_SIG)));
			newDom.setStart(il.append(new ASTORE(newDom.getIndex())));

			// Overwrite old handler with DOM handler
			index = cpg.addInterfaceMethodref(DOM_INTF, "getOutputDomBuilder", "()" + TRANSLET_OUTPUT_SIG);

			il.append(new INVOKEINTERFACE(index,1));
			//index = cpg.addMethodref(DOM_IMPL,
			//		     "getOutputDomBuilder", 
			//		     "()" + TRANSLET_OUTPUT_SIG);
			//il.append(new INVOKEVIRTUAL(index));
			il.append(DUP);
			il.append(DUP);

			// Store DOM handler in a local in order to call endDocument()
			domBuilder = methodGen.addLocalVariable("rt_to_reference_handler", Util.getJCRefType(TRANSLET_OUTPUT_SIG), null, null);
			domBuilder.setStart(il.append(new ASTORE(domBuilder.getIndex())));

			// Call startDocument on the new handler
			index = cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "startDocument", "()V");
			il.append(new INVOKEINTERFACE(index, 1));

			// Call the method that implements this result tree
			index = cpg.addMethodref(className, _methodName, "(" + DOM_INTF_SIG + TRANSLET_OUTPUT_SIG + ")V");
			il.append(new INVOKEVIRTUAL(index));

			// Call endDocument on the DOM handler
			domBuilder.setEnd(il.append(new ALOAD(domBuilder.getIndex())));
			index = cpg.addInterfaceMethodref(TRANSLET_OUTPUT_INTERFACE, "endDocument", "()V");
			il.append(new INVOKEINTERFACE(index, 1));

			// Push the new DOM on the stack
			newDom.setEnd(il.append(new ALOAD(newDom.getIndex())));
		}
		}

		/// <summary>
		/// Expects a result tree on the stack and pushes a node-set (iterator).
		/// Note that the produced iterator is an iterator for the DOM that
		/// contains the result tree, and not the DOM that is currently in use.
		/// This conversion here will therefore not directly work with elements
		/// such as <xsl:apply-templates> and <xsl:for-each> without the DOM
		/// parameter/variable being updates as well.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of NodeSetType (any) </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, NodeSetType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// Put an extra copy of the result tree (DOM) on the stack
		il.append(DUP);

		// DOM adapters containing a result tree are not initialised with
		// translet-type to DOM-type mapping. This must be done now for
		// XPath expressions and patterns to work for the iterator we create.
		il.append(classGen.loadTranslet()); // get names array
		il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, NAMES_INDEX, NAMES_INDEX_SIG)));
		il.append(classGen.loadTranslet()); // get uris array
		il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, URIS_INDEX, URIS_INDEX_SIG)));
		il.append(classGen.loadTranslet()); // get types array
		il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, TYPES_INDEX, TYPES_INDEX_SIG)));
		il.append(classGen.loadTranslet()); // get namespaces array
		il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, NAMESPACE_INDEX, NAMESPACE_INDEX_SIG)));
		// Pass the type mappings to the DOM adapter
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mapping = cpg.addInterfaceMethodref(DOM_INTF, "setupMapping", "(["+STRING_SIG+ "["+STRING_SIG+ "[I" + "["+STRING_SIG+")V");
		int mapping = cpg.addInterfaceMethodref(DOM_INTF, "setupMapping", "([" + STRING_SIG + "[" + STRING_SIG + "[I" + "[" + STRING_SIG + ")V");
		il.append(new INVOKEINTERFACE(mapping, 5));
		il.append(DUP);

		// Create an iterator for the root node of the DOM adapter
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iter = cpg.addInterfaceMethodref(DOM_INTF, "getIterator", "()"+NODE_ITERATOR_SIG);
		int iter = cpg.addInterfaceMethodref(DOM_INTF, "getIterator", "()" + NODE_ITERATOR_SIG);
		il.append(new INVOKEINTERFACE(iter, 1));
		}

		/// <summary>
		/// Subsume result tree into ObjectType.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, ObjectType type)
		{
		methodGen.getInstructionList().append(NOP);
		}

		/// <summary>
		/// Translates a result tree into a non-synthesized boolean.
		/// It does not push a 0 or a 1 but instead returns branchhandle list
		/// to be appended to the false list.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="type"> An instance of BooleanType (any) </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateToDesynthesized"/>
		public override FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
		translateTo(classGen, methodGen, Type.Boolean);
		return new FlowList(il.append(new IFEQ(null)));
		}

		/// <summary>
		/// Translates a result tree to a Java type denoted by <code>clazz</code>. 
		/// Expects a result tree on the stack and pushes an object
		/// of the appropriate type after coercion. Result trees are translated
		/// to W3C Node or W3C NodeList and the translation is done
		/// via node-set type.
		/// </summary>
		/// <param name="classGen"> A BCEL class generator </param>
		/// <param name="methodGen"> A BCEL method generator </param>
		/// <param name="clazz"> An reference to the Class to translate to </param>
		/// <seealso cref="org.apache.xalan.xsltc.compiler.util.Type.translateTo"/>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = clazz.getName();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		string className = clazz.FullName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (className.Equals("org.w3c.dom.Node"))
		{
			translateTo(classGen, methodGen, Type.NodeSet);
			int index = cpg.addInterfaceMethodref(DOM_INTF, MAKE_NODE, MAKE_NODE_SIG2);
			il.append(new INVOKEINTERFACE(index, 2));
		}
		else if (className.Equals("org.w3c.dom.NodeList"))
		{
			translateTo(classGen, methodGen, Type.NodeSet);
			int index = cpg.addInterfaceMethodref(DOM_INTF, MAKE_NODE_LIST, MAKE_NODE_LIST_SIG2);
			il.append(new INVOKEINTERFACE(index, 2));
		}
		else if (className.Equals("java.lang.Object"))
		{
			il.append(NOP);
		}
			else if (className.Equals("java.lang.String"))
			{
				translateTo(classGen, methodGen, Type.String);
			}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), className);
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
			return (DOM_INTF);
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