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
 * $Id: IdKeyPattern.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFNE = org.apache.bcel.generic.IFNE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal abstract class IdKeyPattern : LocationPathPattern
	{

		protected internal RelativePathPattern _left = null;
		private string _index = null;
		private string _value = null;

		public IdKeyPattern(string index, string value)
		{
		_index = index;
		_value = value;
		}

		public virtual string IndexName
		{
			get
			{
			return (_index);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return Type.NodeSet;
		}

		public override bool Wildcard
		{
			get
			{
			return false;
			}
		}

		public virtual RelativePathPattern Left
		{
			set
			{
			_left = value;
			}
		}

		public override StepPattern KernelPattern
		{
			get
			{
			return (null);
			}
		}

		public override void reduceKernelPattern()
		{
		}

		public override string ToString()
		{
		return "id/keyPattern(" + _index + ", " + _value + ')';
		}

		/// <summary>
		/// This method is called when the constructor is compiled in
		/// Stylesheet.compileConstructor() and not as the syntax tree is traversed.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// Returns the KeyIndex object of a given name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getKeyIndex = cpg.addMethodref(TRANSLET_CLASS, "getKeyIndex", "(Ljava/lang/String;)"+ KEY_INDEX_SIG);
		int getKeyIndex = cpg.addMethodref(TRANSLET_CLASS, "getKeyIndex", "(Ljava/lang/String;)" + KEY_INDEX_SIG);

		// Initialises a KeyIndex to return nodes with specific values
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lookupId = cpg.addMethodref(KEY_INDEX_CLASS, "containsID", "(ILjava/lang/Object;)I");
		int lookupId = cpg.addMethodref(KEY_INDEX_CLASS, "containsID", "(ILjava/lang/Object;)I");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lookupKey = cpg.addMethodref(KEY_INDEX_CLASS, "containsKey", "(ILjava/lang/Object;)I");
		int lookupKey = cpg.addMethodref(KEY_INDEX_CLASS, "containsKey", "(ILjava/lang/Object;)I");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getNodeIdent = cpg.addInterfaceMethodref(DOM_INTF, "getNodeIdent", "(I)"+NODE_SIG);
		int getNodeIdent = cpg.addInterfaceMethodref(DOM_INTF, "getNodeIdent", "(I)" + NODE_SIG);

		// Call getKeyIndex in AbstractTranslet with the name of the key
		// to get the index for this key (which is also a node iterator).
		il.append(classGen.loadTranslet());
		il.append(new PUSH(cpg,_index));
		il.append(new INVOKEVIRTUAL(getKeyIndex));

		// Now use the value in the second argument to determine what nodes
		// the iterator should return.
		il.append(SWAP);
		il.append(new PUSH(cpg,_value));
		if (this is IdPattern)
		{
			il.append(new INVOKEVIRTUAL(lookupId));
		}
		else
		{
			il.append(new INVOKEVIRTUAL(lookupKey));
		}

		_trueList.add(il.append(new IFNE(null)));
		_falseList.add(il.append(new GOTO(null)));
		}

	}


}