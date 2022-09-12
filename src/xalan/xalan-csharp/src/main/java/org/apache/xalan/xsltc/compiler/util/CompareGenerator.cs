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
 * $Id: CompareGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using ACONST_NULL = org.apache.bcel.generic.ACONST_NULL;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using Type = org.apache.bcel.generic.Type;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class CompareGenerator : MethodGenerator
	{

		private static int DOM_INDEX = 1;
		private static int CURRENT_INDEX = 2;
		private static int LEVEL_INDEX = 3;
		private static int TRANSLET_INDEX = 4;
		private static int org;
		private int ITERATOR_INDEX = 6;

		private readonly Instruction _iloadCurrent;
		private readonly Instruction _istoreCurrent;
		private readonly Instruction _aloadDom;
		private readonly Instruction _iloadLast;
		private readonly Instruction _aloadIterator;
		private readonly Instruction _astoreIterator;

		public CompareGenerator(int access_flags, Type return_type, Type[] arg_types, string[] arg_names, string method_name, string class_name, InstructionList il, ConstantPoolGen cp) : base(access_flags, return_type, arg_types, arg_names, method_name, class_name, il, cp)
		{

		_iloadCurrent = new ILOAD(CURRENT_INDEX);
		_istoreCurrent = new ISTORE(CURRENT_INDEX);
		_aloadDom = new ALOAD(DOM_INDEX);
		_iloadLast = new ILOAD(org.apache.xalan.xsltc.compiler.Constants_Fields.LAST_INDEX);

		LocalVariableGen iterator = addLocalVariable("iterator", Util.getJCRefType(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG), null, null);
		ITERATOR_INDEX = iterator.Index;
		_aloadIterator = new ALOAD(ITERATOR_INDEX);
		_astoreIterator = new ASTORE(ITERATOR_INDEX);
		il.append(new ACONST_NULL());
		il.append(storeIterator());
		}

		public Instruction loadLastNode()
		{
		return _iloadLast;
		}

		public override Instruction loadCurrentNode()
		{
		return _iloadCurrent;
		}

		public override Instruction storeCurrentNode()
		{
		return _istoreCurrent;
		}

		public override Instruction loadDOM()
		{
		return _aloadDom;
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
			return INVALID_INDEX;
			}
		}

		public override Instruction storeIterator()
		{
		return _astoreIterator;
		}

		public override Instruction loadIterator()
		{
		return _aloadIterator;
		}

		//??? may not be used anymore
		public override int getLocalIndex(string name)
		{
		if (name.Equals("current"))
		{
			return CURRENT_INDEX;
		}
		return base.getLocalIndex(name);
		}
	}

}