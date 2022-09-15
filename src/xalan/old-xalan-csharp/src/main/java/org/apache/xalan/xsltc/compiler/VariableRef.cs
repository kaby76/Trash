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
 * $Id: VariableRef.java 528589 2007-04-13 18:50:56Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSetType = org.apache.xalan.xsltc.compiler.util.NodeSetType;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal sealed class VariableRef : VariableRefBase
	{

		public VariableRef(Variable variable) : base(variable)
		{
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// Fall-through for variables that are implemented as methods
		if (_type.implementedAsMethod())
		{
			return;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _variable.getEscapedName();
		string name = _variable.EscapedName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String signature = _type.toSignature();
		string signature = _type.toSignature();

		if (_variable.Local)
		{
			if (classGen.External)
			{
			Closure variableClosure = _closure;
			while (variableClosure != null)
			{
				if (variableClosure.inInnerClass())
				{
					break;
				}
				variableClosure = variableClosure.ParentClosure;
			}

			if (variableClosure != null)
			{
				il.append(ALOAD_0);
				il.append(new GETFIELD(cpg.addFieldref(variableClosure.InnerClassName, name, signature)));
			}
			else
			{
				il.append(_variable.loadInstruction());
			}
			}
			else
			{
			il.append(_variable.loadInstruction());
			}
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getClassName();
			string className = classGen.ClassName;
			il.append(classGen.loadTranslet());
			if (classGen.External)
			{
			il.append(new CHECKCAST(cpg.addClass(className)));
			}
			il.append(new GETFIELD(cpg.addFieldref(className,name,signature)));
		}

		if (_variable.Type is NodeSetType)
		{
			// The method cloneIterator() also does resetting
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int clone = cpg.addInterfaceMethodref(Constants_Fields.NODE_ITERATOR, "cloneIterator", "()" + Constants_Fields.NODE_ITERATOR_SIG);
			int clone = cpg.addInterfaceMethodref(Constants_Fields.NODE_ITERATOR, "cloneIterator", "()" + Constants_Fields.NODE_ITERATOR_SIG);
			il.append(new INVOKEINTERFACE(clone, 1));
		}
		}
	}

}