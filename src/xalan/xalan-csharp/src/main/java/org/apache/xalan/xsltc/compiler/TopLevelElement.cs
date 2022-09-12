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
 * $Id: TopLevelElement.java 476471 2006-11-18 08:36:27Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	internal class TopLevelElement : SyntaxTreeNode
	{

		/*
		 * List of dependencies with other variables, parameters or
		 * keys defined at the top level.
		 */
		protected internal ArrayList _dependencies = null;

		/// <summary>
		/// Type check all the children of this node.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return typeCheckContents(stable);
		}

		/// <summary>
		/// Translate this node into JVM bytecodes.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		ErrorMsg msg = new ErrorMsg(ErrorMsg.NOT_IMPLEMENTED_ERR, this.GetType(), this);
		Parser.reportError(Constants_Fields.FATAL, msg);
		}

		/// <summary>
		/// Translate this node into a fresh instruction list.
		/// The original instruction list is saved and restored.
		/// </summary>
		public virtual InstructionList compile(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList result, save = methodGen.getInstructionList();
		InstructionList result , save = methodGen.InstructionList;
		methodGen.InstructionList = result = new InstructionList();
		translate(classGen, methodGen);
		methodGen.InstructionList = save;
		return result;
		}

		public override void display(int indent)
		{
		indent(indent);
		Util.println("TopLevelElement");
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Add a dependency with other top-level elements like
		/// variables, parameters or keys.
		/// </summary>
		public virtual void addDependency(TopLevelElement other)
		{
		if (_dependencies == null)
		{
			_dependencies = new ArrayList();
		}
		if (!_dependencies.Contains(other))
		{
			_dependencies.Add(other);
		}
		}

		/// <summary>
		/// Get the list of dependencies with other top-level elements
		/// like variables, parameteres or keys.
		/// </summary>
		public virtual ArrayList Dependencies
		{
			get
			{
			return _dependencies;
			}
		}

	}

}