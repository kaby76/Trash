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
 * $Id: TestGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using Type = org.apache.bcel.generic.Type;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class TestGenerator : MethodGenerator
	{
		private static int CONTEXT_NODE_INDEX = 1;
		private static int CURRENT_NODE_INDEX = 4;
		private static int ITERATOR_INDEX = 6;

		private Instruction _aloadDom;
		private readonly Instruction _iloadCurrent;
		private readonly Instruction _iloadContext;
		private readonly Instruction _istoreCurrent;
		private readonly Instruction _istoreContext;
		private readonly Instruction _astoreIterator;
		private readonly Instruction _aloadIterator;

		public TestGenerator(int access_flags, Type return_type, Type[] arg_types, string[] arg_names, string method_name, string class_name, InstructionList il, ConstantPoolGen cp) : base(access_flags, return_type, arg_types, arg_names, method_name, class_name, il, cp)
		{

		_iloadCurrent = new ILOAD(CURRENT_NODE_INDEX);
		_istoreCurrent = new ISTORE(CURRENT_NODE_INDEX);
		_iloadContext = new ILOAD(CONTEXT_NODE_INDEX);
		_istoreContext = new ILOAD(CONTEXT_NODE_INDEX);
		_astoreIterator = new ASTORE(ITERATOR_INDEX);
		_aloadIterator = new ALOAD(ITERATOR_INDEX);
		}

		public int HandlerIndex
		{
			get
			{
			return INVALID_INDEX; // not available
			}
		}

		public int IteratorIndex
		{
			get
			{
			return ITERATOR_INDEX; // not available
			}
		}

		public int DomIndex
		{
			set
			{
			_aloadDom = new ALOAD(value);
			}
		}

		public override Instruction loadDOM()
		{
		return _aloadDom;
		}

		public override Instruction loadCurrentNode()
		{
		return _iloadCurrent;
		}

		/// <summary>
		/// by default context node is the same as current node. MK437 </summary>
		public override Instruction loadContextNode()
		{
		return _iloadContext;
		}

		public override Instruction storeContextNode()
		{
		return _istoreContext;
		}

		public override Instruction storeCurrentNode()
		{
		return _istoreCurrent;
		}

		public override Instruction storeIterator()
		{
		return _astoreIterator;
		}

		public override Instruction loadIterator()
		{
		return _aloadIterator;
		}

		public override int getLocalIndex(string name)
		{
		if (name.Equals("current"))
		{
			return CURRENT_NODE_INDEX;
		}
		else
		{
			return base.getLocalIndex(name);
		}
		}
	}

}