using System.Collections;

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
 * $Id: DocumentCall.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class DocumentCall : FunctionCall
	{

		private Expression _arg1 = null;
		private Expression _arg2 = null;
		private Type _arg1Type;

		/// <summary>
		/// Default function call constructor
		/// </summary>
		public DocumentCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		}

		/// <summary>
		/// Type checks the arguments passed to the document() function. The first
		/// argument can be any type (we must cast it to a string) and contains the
		/// URI of the document
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
			// At least one argument - two at most
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ac = argumentCount();
			int ac = argumentCount();
			if ((ac < 1) || (ac > 2))
			{
				ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_ARG_ERR, this);
				throw new TypeCheckError(msg);
			}
			if (Stylesheet == null)
			{
				ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_ARG_ERR, this);
				throw new TypeCheckError(msg);
			}

			// Parse the first argument 
			_arg1 = argument(0);

			if (_arg1 == null)
			{ // should not happened
				ErrorMsg msg = new ErrorMsg(ErrorMsg.DOCUMENT_ARG_ERR, this);
				throw new TypeCheckError(msg);
			}

			_arg1Type = _arg1.typeCheck(stable);
			if ((_arg1Type != Type.NodeSet) && (_arg1Type != Type.String))
			{
				_arg1 = new CastExpr(_arg1, Type.String);
			}

			// Parse the second argument 
			if (ac == 2)
			{
				_arg2 = argument(1);

				if (_arg2 == null)
				{ // should not happened
					ErrorMsg msg = new ErrorMsg(ErrorMsg.DOCUMENT_ARG_ERR, this);
					throw new TypeCheckError(msg);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type arg2Type = _arg2.typeCheck(stable);
				Type arg2Type = _arg2.typeCheck(stable);

				if (arg2Type.identicalTo(Type.Node))
				{
					_arg2 = new CastExpr(_arg2, Type.NodeSet);
				}
				else if (arg2Type.identicalTo(Type.NodeSet))
				{
					// falls through
				}
				else
				{
					ErrorMsg msg = new ErrorMsg(ErrorMsg.DOCUMENT_ARG_ERR, this);
					throw new TypeCheckError(msg);
				}
			}

			return _type = Type.NodeSet;
		}

		/// <summary>
		/// Translates the document() function call to a call to LoadDocument()'s
		/// static method document().
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ac = argumentCount();
			int ac = argumentCount();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int domField = cpg.addFieldref(classGen.getClassName(), Constants_Fields.DOM_FIELD, Constants_Fields.DOM_INTF_SIG);
			int domField = cpg.addFieldref(classGen.ClassName, Constants_Fields.DOM_FIELD, Constants_Fields.DOM_INTF_SIG);

			string docParamList = null;
			if (ac == 1)
			{
			   // documentF(Object,String,AbstractTranslet,DOM)
			   docParamList = "(" + Constants_Fields.OBJECT_SIG + Constants_Fields.STRING_SIG + Constants_Fields.TRANSLET_SIG + Constants_Fields.DOM_INTF_SIG + ")" + Constants_Fields.NODE_ITERATOR_SIG;
			}
			else
			{ //ac == 2; ac < 1 or as >2  was tested in typeChec()
			   // documentF(Object,DTMAxisIterator,String,AbstractTranslet,DOM)
			   docParamList = "(" + Constants_Fields.OBJECT_SIG + Constants_Fields.NODE_ITERATOR_SIG + Constants_Fields.STRING_SIG + Constants_Fields.TRANSLET_SIG + Constants_Fields.DOM_INTF_SIG + ")" + Constants_Fields.NODE_ITERATOR_SIG;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int docIdx = cpg.addMethodref(Constants_Fields.LOAD_DOCUMENT_CLASS, "documentF", docParamList);
			int docIdx = cpg.addMethodref(Constants_Fields.LOAD_DOCUMENT_CLASS, "documentF", docParamList);


			// The URI can be either a node-set or something else cast to a string
			_arg1.translate(classGen, methodGen);
			if (_arg1Type == Type.NodeSet)
			{
				_arg1.startIterator(classGen, methodGen);
			}

			if (ac == 2)
			{
				//_arg2 == null was tested in typeChec()
				_arg2.translate(classGen, methodGen);
				_arg2.startIterator(classGen, methodGen);
			}

			// Process the rest of the parameters on the stack
			il.append(new PUSH(cpg, Stylesheet.SystemId));
			il.append(classGen.loadTranslet());
			il.append(DUP);
			il.append(new GETFIELD(domField));
			il.append(new INVOKESTATIC(docIdx));
		}

	}

}