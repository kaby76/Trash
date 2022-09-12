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
 * $Id: AttributeSetMethodGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ClassGen = org.apache.bcel.generic.ClassGen;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class AttributeSetMethodGenerator : MethodGenerator
	{
		private const int DOM_INDEX = 1;
		private const int ITERATOR_INDEX = 2;
		private const int HANDLER_INDEX = 3;

		private static readonly org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[3];
		private static readonly string[] argNames = new string[3];

		static AttributeSetMethodGenerator()
		{
		   argTypes[0] = Util.getJCRefType(org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG);
		   argNames[0] = org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_PNAME;
		   argTypes[1] = Util.getJCRefType(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		   argNames[1] = org.apache.xalan.xsltc.compiler.Constants_Fields.ITERATOR_PNAME;
		   argTypes[2] = Util.getJCRefType(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_SIG);
		   argNames[2] = org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_PNAME;
		}


		private readonly Instruction _aloadDom;
		private readonly Instruction _astoreDom;
		private readonly Instruction _astoreIterator;
		private readonly Instruction _aloadIterator;
		private readonly Instruction _astoreHandler;
		private readonly Instruction _aloadHandler;

		public AttributeSetMethodGenerator(string methodName, ClassGen classGen) : base(org.apache.bcel.Constants.ACC_PRIVATE, org.apache.bcel.generic.Type.VOID, argTypes, argNames, methodName, classGen.ClassName, new InstructionList(), classGen.ConstantPool)
		{

		_aloadDom = new ALOAD(DOM_INDEX);
		_astoreDom = new ASTORE(DOM_INDEX);
		_astoreIterator = new ASTORE(ITERATOR_INDEX);
		_aloadIterator = new ALOAD(ITERATOR_INDEX);
		_astoreHandler = new ASTORE(HANDLER_INDEX);
		_aloadHandler = new ALOAD(HANDLER_INDEX);
		}

		public override Instruction storeIterator()
		{
		return _astoreIterator;
		}

		public override Instruction loadIterator()
		{
		return _aloadIterator;
		}

		public int IteratorIndex
		{
			get
			{
			return ITERATOR_INDEX;
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