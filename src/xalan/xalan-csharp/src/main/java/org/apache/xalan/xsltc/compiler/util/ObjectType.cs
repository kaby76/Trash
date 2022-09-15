using System.Text;

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
 * $Id: ObjectType.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFNULL = org.apache.bcel.generic.IFNULL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using Constants = org.apache.xalan.xsltc.compiler.Constants;

	/// <summary>
	/// @author Todd Miller
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class ObjectType : Type
	{

		private string _javaClassName = "java.lang.Object";
		private System.Type _clazz = typeof(object);

		/// <summary>
		/// Used to represent a Java Class type such is required to support 
		/// non-static java functions. </summary>
		/// <param name="javaClassName"> name of the class such as 'com.foo.Processor' </param>
		protected internal ObjectType(string javaClassName)
		{
		_javaClassName = javaClassName;

		try
		{
			  _clazz = ObjectFactory.findProviderClass(javaClassName, ObjectFactory.findClassLoader(), true);
		}
		catch (ClassNotFoundException)
		{
		  _clazz = null;
		}
		}

		protected internal ObjectType(System.Type clazz)
		{
			_clazz = clazz;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			_javaClassName = clazz.FullName;
		}

		/// <summary>
		/// Must return the same value for all ObjectType instances. This is
		/// needed in CastExpr to ensure the mapping table is used correctly.
		/// </summary>
		public override int GetHashCode()
		{
			return typeof(object).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is ObjectType);
		}

		public string JavaClassName
		{
			get
			{
			return _javaClassName;
			}
		}

		public System.Type JavaClass
		{
			get
			{
				return _clazz;
			}
		}

		public override string ToString()
		{
		return _javaClassName;
		}

		public override bool identicalTo(Type other)
		{
		return this == other;
		}

		public override string toSignature()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer result = new StringBuffer("L");
		StringBuilder result = new StringBuilder("L");
		result.Append(_javaClassName.Replace('.', '/')).Append(';');
		return result.ToString();
		}

		public override org.apache.bcel.generic.Type toJCType()
		{
		return Util.getJCRefType(toSignature());
		}

		/// <summary>
		/// Translates a void into an object of internal type <code>type</code>.
		/// This translation is needed when calling external functions
		/// that return void.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		if (type == Type.String)
		{
			translateTo(classGen, methodGen, (StringType) type);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Expects an integer on the stack and pushes its string value by calling
		/// <code>Integer.toString(int i)</code>.
		/// 
		/// @see	org.apache.xalan.xsltc.compiler.util.Type#translateTo
		/// </summary>
		public void translateTo(ClassGenerator classGen, MethodGenerator methodGen, StringType type)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		il.append(DUP);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifNull = il.append(new org.apache.bcel.generic.IFNULL(null));
		BranchHandle ifNull = il.append(new IFNULL(null));
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(_javaClassName, "toString", "()" + STRING_SIG)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle gotobh = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle gotobh = il.append(new GOTO(null));
		ifNull.setTarget(il.append(POP));
		il.append(new PUSH(cpg, ""));
		gotobh.setTarget(il.append(NOP));
		}

		/// <summary>
		/// Translates an object of this type to the external (Java) type denoted
		/// by <code>clazz</code>. This method is used to translate parameters 
		/// when external functions are called.
		/// </summary>
		public override void translateTo(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
			if (clazz.IsAssignableFrom(_clazz))
			{
			methodGen.getInstructionList().append(NOP);
			}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.GetType().ToString());
			classGen.Parser.reportError(Constants.FATAL, err);
		}
		}

		/// <summary>
		/// Translates an external Java type into an Object type 
		/// </summary>
		public override void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, System.Type clazz)
		{
		methodGen.getInstructionList().append(NOP);
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