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
 * $Id: AbsoluteLocationPath.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeType = org.apache.xalan.xsltc.compiler.util.NodeType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class AbsoluteLocationPath : Expression
	{
		private Expression _path; // may be null

		public AbsoluteLocationPath()
		{
		_path = null;
		}

		public AbsoluteLocationPath(Expression path)
		{
		_path = path;
		if (path != null)
		{
			_path.Parent = this;
		}
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			if (_path != null)
			{
				_path.Parser = value;
			}
			}
		}

		public Expression Path
		{
			get
			{
			return (_path);
			}
		}

		public override string ToString()
		{
		return "AbsoluteLocationPath(" + (_path != null ? _path.ToString() : "null") + ')';
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_path != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type ptype = _path.typeCheck(stable);
			Type ptype = _path.typeCheck(stable);
			if (ptype is NodeType)
			{ // promote to node-set
			_path = new CastExpr(_path, Type.NodeSet);
			}
		}
		return _type = Type.NodeSet;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		if (_path != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int initAI = cpg.addMethodref(Constants_Fields.ABSOLUTE_ITERATOR, "<init>", "(" + Constants_Fields.NODE_ITERATOR_SIG + ")V");
			int initAI = cpg.addMethodref(Constants_Fields.ABSOLUTE_ITERATOR, "<init>", "(" + Constants_Fields.NODE_ITERATOR_SIG + ")V");

			// Compile relative path iterator(s)
				//
				// Backwards branches are prohibited if an uninitialized object is
				// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
				// We don't know whether this code might contain backwards branches,
				// so we mustn't create the new object until after we've created
				// this argument to its constructor.  Instead we calculate the
				// value of the argument to the constructor first, store it in
				// a temporary variable, create the object and reload the argument
				// from the temporary to avoid the problem.
			_path.translate(classGen, methodGen);
				LocalVariableGen relPathIterator = methodGen.addLocalVariable("abs_location_path_tmp", Util.getJCRefType(Constants_Fields.NODE_ITERATOR_SIG), null, null);
				relPathIterator.Start = il.append(new ASTORE(relPathIterator.Index));

			// Create new AbsoluteIterator
			il.append(new NEW(cpg.addClass(Constants_Fields.ABSOLUTE_ITERATOR)));
			il.append(DUP);
				relPathIterator.End = il.append(new ALOAD(relPathIterator.Index));

			// Initialize AbsoluteIterator with iterator from the stack
			il.append(new INVOKESPECIAL(initAI));
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int gitr = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getIterator", "()"+Constants_Fields.NODE_ITERATOR_SIG);
			int gitr = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getIterator", "()" + Constants_Fields.NODE_ITERATOR_SIG);
			il.append(methodGen.loadDOM());
			il.append(new INVOKEINTERFACE(gitr, 1));
		}
		}
	}

}