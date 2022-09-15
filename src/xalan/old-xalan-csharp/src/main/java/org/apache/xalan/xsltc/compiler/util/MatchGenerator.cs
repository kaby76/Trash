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
 * $Id: MatchGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using Type = org.apache.bcel.generic.Type;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class MatchGenerator : MethodGenerator
	{
		private static int CURRENT_INDEX = 1;

		private int _iteratorIndex = INVALID_INDEX;

		private readonly Instruction _iloadCurrent;
		private readonly Instruction _istoreCurrent;
		private Instruction _aloadDom;

		public MatchGenerator(int access_flags, Type return_type, Type[] arg_types, string[] arg_names, string method_name, string class_name, InstructionList il, ConstantPoolGen cp) : base(access_flags, return_type, arg_types, arg_names, method_name, class_name, il, cp)
		{

		_iloadCurrent = new ILOAD(CURRENT_INDEX);
		_istoreCurrent = new ISTORE(CURRENT_INDEX);
		}

		public override Instruction loadCurrentNode()
		{
		return _iloadCurrent;
		}

		public override Instruction storeCurrentNode()
		{
		return _istoreCurrent;
		}

		public int HandlerIndex
		{
			get
			{
			return INVALID_INDEX; // not available
			}
		}

		/// <summary>
		/// Get index of the register where the DOM is stored.
		/// </summary>
		public override Instruction loadDOM()
		{
		return _aloadDom;
		}

		/// <summary>
		/// Set index where the reference to the DOM is stored.
		/// </summary>
		public int DomIndex
		{
			set
			{
			_aloadDom = new ALOAD(value);
			}
		}

		/// <summary>
		/// Get index of the register where the current iterator is stored.
		/// </summary>
		public int IteratorIndex
		{
			get
			{
			return _iteratorIndex;
			}
			set
			{
			_iteratorIndex = value;
			}
		}


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