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
 * $Id: NamedMethodGenerator.java 468649 2006-10-28 07:00:55Z minchau $
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
	/// This class is used for named templates. Named template methods have access
	/// to the DOM, the current iterator, the handler and the current node.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class NamedMethodGenerator : MethodGenerator
	{
		protected internal const int CURRENT_INDEX = 4;

		// The index of the first parameter (after dom/iterator/handler/current)
		private const int PARAM_START_INDEX = 5;

		public NamedMethodGenerator(int access_flags, Type return_type, Type[] arg_types, string[] arg_names, string method_name, string class_name, InstructionList il, ConstantPoolGen cp) : base(access_flags, return_type, arg_types, arg_names, method_name, class_name, il, cp)
		{
		}

		public override int getLocalIndex(string name)
		{
		if (name.Equals("current"))
		{
			return CURRENT_INDEX;
		}
		return base.getLocalIndex(name);
		}

		public Instruction loadParameter(int index)
		{
			return new ALOAD(index + PARAM_START_INDEX);
		}

		public Instruction storeParameter(int index)
		{
			return new ASTORE(index + PARAM_START_INDEX);
		}
	}

}