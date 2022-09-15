using System;

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
 * $Id: Type.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using BranchInstruction = org.apache.bcel.generic.BranchInstruction;
	using Instruction = org.apache.bcel.generic.Instruction;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	public abstract class Type : Constants
	{
		public static readonly Type Int = new IntType();
		public static readonly Type Real = new RealType();
		public static readonly Type Boolean = new BooleanType();
		public static readonly Type NodeSet = new NodeSetType();
		public static readonly Type String = new StringType();
		public static readonly Type ResultTree = new ResultTreeType();
		public static readonly Type Reference = new ReferenceType();
		public static readonly Type Void = new VoidType();
		public static readonly Type Object = new ObjectType(typeof(object));

		public static readonly Type Node = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.ANODE);
		public static readonly Type Root = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.ROOT);
		public static readonly Type Element = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.ELEMENT);
		public static readonly Type Attribute = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.ATTRIBUTE);
		public static readonly Type Text = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.TEXT);
		public static readonly Type Comment = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.COMMENT);
		public static readonly Type Processing_Instruction = new NodeType(org.apache.xalan.xsltc.compiler.NodeTest_Fields.PI);

		/// <summary>
		/// Factory method to instantiate object types. Returns a pre-defined
		/// instance for "java.lang.Object" and "java.lang.String".
		/// </summary>
		public static Type newObjectType(string javaClassName)
		{
			if (string.ReferenceEquals(javaClassName, "java.lang.Object"))
			{
				return Type.Object;
			}
			else if (string.ReferenceEquals(javaClassName, "java.lang.String"))
			{
				return Type.String;
			}
			else
			{
				return new ObjectType(javaClassName);
			}
		}

	   /// <summary>
	   /// Factory method to instantiate object types. Returns a pre-defined
	   /// instance for java.lang.Object.class and java.lang.String.class.
	   /// </summary>
		public static Type newObjectType(Type clazz)
		{
			if (clazz == typeof(object))
			{
				return Type.Object;
			}
			else if (clazz == typeof(string))
			{
				return Type.String;
			}
			else
			{
				return new ObjectType(clazz);
			}
		}

		/// <summary>
		/// Returns a string representation of this type.	
		/// </summary>
		public override abstract String ToString();

		/// <summary>
		/// Returns true if this and other are identical types.
		/// </summary>
		public abstract bool identicalTo(Type other);

		/// <summary>
		/// Returns true if this type is a numeric type. Redefined in NumberType.
		/// </summary>
		public virtual bool Number
		{
			get
			{
			return false;
			}
		}

		/// <summary>
		/// Returns true if this type has no object representaion. Redefined in
		/// ResultTreeType.
		/// </summary>
		public virtual bool implementedAsMethod()
		{
		return false;
		}

		/// <summary>
		/// Returns true if this type is a simple type. Redefined in NumberType,
		/// BooleanType and StringType.
		/// </summary>
		public virtual bool Simple
		{
			get
			{
			return false;
			}
		}

		public abstract org.apache.bcel.generic.Type toJCType();

		/// <summary>
		/// Returns the distance between two types. This measure is used to select
		/// overloaded functions/operators. This method is typically redefined by
		/// the subclasses.
		/// </summary>
		public virtual int distanceTo(Type type)
		{
		return type == this ? 0 : int.MaxValue;
		}

		/// <summary>
		/// Returns the signature of an internal type's external representation.
		/// </summary>
		public abstract String toSignature();

		/// <summary>
		/// Translates an object of this type to an object of type
		/// <code>type</code>. 
		/// Expects an object of the former type and pushes an object of the latter.
		/// </summary>
		public virtual void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
		classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}

		/// <summary>
		/// Translates object of this type to an object of type <code>type</code>. 
		/// Expects an object of the former type and pushes an object of the latter
		/// if not boolean. If type <code>type</code> is boolean then a branchhandle
		/// list (to be appended to the false list) is returned.
		/// </summary>
		public virtual FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, Type type)
		{
		FlowList fl = null;
		if (type == Type.Boolean)
		{
			fl = translateToDesynthesized(classGen, methodGen, (BooleanType)type);
		}
		else
		{
			translateTo(classGen, methodGen, type);
		}
		return fl;
		}

		/// <summary>
		/// Translates an object of this type to an non-synthesized boolean. It
		/// does not push a 0 or a 1 but instead returns branchhandle list to be
		/// appended to the false list.
		/// </summary>
		public virtual FlowList translateToDesynthesized(ClassGenerator classGen, MethodGenerator methodGen, BooleanType type)
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), type.ToString());
		classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		return null;
		}

		/// <summary>
		/// Translates an object of this type to the external (Java) type denoted
		/// by <code>clazz</code>. This method is used to translate parameters 
		/// when external functions are called.
		/// </summary>
		public virtual void translateTo(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), clazz.GetType().ToString());
		classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}

		/// <summary>
		/// Translates an external (Java) type denoted by <code>clazz</code> to 
		/// an object of this type. This method is used to translate return values 
		/// when external functions are called.
		/// </summary>
		public virtual void translateFrom(ClassGenerator classGen, MethodGenerator methodGen, Type clazz)
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, clazz.GetType().ToString(), ToString());
		classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}

		/// <summary>
		/// Translates an object of this type to its boxed representation.
		/// </summary>
		public virtual void translateBox(ClassGenerator classGen, MethodGenerator methodGen)
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, ToString(), "[" + ToString() + "]");
		classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}

		/// <summary>
		/// Translates an object of this type to its unboxed representation.
		/// </summary>
		public virtual void translateUnBox(ClassGenerator classGen, MethodGenerator methodGen)
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.DATA_CONVERSION_ERR, "[" + ToString() + "]", ToString());
		classGen.Parser.reportError(org.apache.xalan.xsltc.compiler.Constants_Fields.FATAL, err);
		}

		/// <summary>
		/// Returns the class name of an internal type's external representation.
		/// </summary>
		public virtual String ClassName
		{
			get
			{
			return (org.apache.xalan.xsltc.compiler.Constants_Fields.EMPTYSTRING);
			}
		}

		public virtual Instruction ADD()
		{
		return null; // should never be called
		}

		public virtual Instruction SUB()
		{
		return null; // should never be called
		}

		public virtual Instruction MUL()
		{
		return null; // should never be called
		}

		public virtual Instruction DIV()
		{
		return null; // should never be called
		}

		public virtual Instruction REM()
		{
		return null; // should never be called
		}

		public virtual Instruction NEG()
		{
		return null; // should never be called
		}

		public virtual Instruction LOAD(int slot)
		{
		return null; // should never be called
		}

		public virtual Instruction STORE(int slot)
		{
		return null; // should never be called
		}

		public virtual Instruction POP()
		{
		return POP;
		}

		public virtual BranchInstruction GT(bool tozero)
		{
		return null; // should never be called
		}

		public virtual BranchInstruction GE(bool tozero)
		{
		return null; // should never be called
		}

		public virtual BranchInstruction LT(bool tozero)
		{
		return null; // should never be called
		}

		public virtual BranchInstruction LE(bool tozero)
		{
		return null; // should never be called
		}

		public virtual Instruction CMP(bool less)
		{
		return null; // should never be called
		}

		public virtual Instruction DUP()
		{
		return DUP; // default
		}
	}

}