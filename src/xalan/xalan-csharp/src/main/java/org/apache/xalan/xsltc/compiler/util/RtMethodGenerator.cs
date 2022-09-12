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
 * $Id: RtMethodGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using Type = org.apache.bcel.generic.Type;

	/// <summary>
	/// This class is used for result trees implemented as methods. These
	/// methods take a reference to the DOM and to the handler only.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class RtMethodGenerator : MethodGenerator
	{
		private const int HANDLER_INDEX = 2;
		private readonly Instruction _astoreHandler;
		private readonly Instruction _aloadHandler;

		public RtMethodGenerator(int access_flags, Type return_type, Type[] arg_types, string[] arg_names, string method_name, string class_name, InstructionList il, ConstantPoolGen cp) : base(access_flags, return_type, arg_types, arg_names, method_name, class_name, il, cp)
		{

		_astoreHandler = new ASTORE(HANDLER_INDEX);
		_aloadHandler = new ALOAD(HANDLER_INDEX);
		}

		public int IteratorIndex
		{
			get
			{
			return INVALID_INDEX; // not available
			}
		}

		public override Instruction storeHandler()
		{
		return _astoreHandler;
		}

		public override Instruction loadHandler()
		{
		return _aloadHandler;
		}

		public override int getLocalIndex(string name)
		{
		return INVALID_INDEX; // not available
		}
	}

}