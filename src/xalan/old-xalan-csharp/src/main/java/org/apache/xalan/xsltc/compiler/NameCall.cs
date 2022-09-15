using System.Collections;

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
 * $Id: NameCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class NameCall : NameBase
	{

		/// <summary>
		/// Handles calls with no parameter (current node is implicit parameter).
		/// </summary>
		public NameCall(QName fname) : base(fname)
		{
		}

		/// <summary>
		/// Handles calls with one parameter (either node or node-set).
		/// </summary>
		public NameCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		}

		/// <summary>
		/// Translate code that leaves a node's QName (as a String) on the stack
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getName = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_NODE_NAME, Constants_Fields.GET_NODE_NAME_SIG);
		int getName = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, Constants_Fields.GET_NODE_NAME, Constants_Fields.GET_NODE_NAME_SIG);
		base.translate(classGen, methodGen);
		il.append(new INVOKEINTERFACE(getName, 2));
		}
	}

}