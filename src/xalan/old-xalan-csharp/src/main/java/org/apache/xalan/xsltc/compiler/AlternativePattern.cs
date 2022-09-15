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
 * $Id: AlternativePattern.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using GOTO = org.apache.bcel.generic.GOTO;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class AlternativePattern : Pattern
	{
		private readonly Pattern _left;
		private readonly Pattern _right;

		/// <summary>
		/// Construct an alternative pattern. The method <code>setParent</code>
		/// should not be called in this case.
		/// </summary>
		public AlternativePattern(Pattern left, Pattern right)
		{
		_left = left;
		_right = right;
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_left.Parser = value;
			_right.Parser = value;
			}
		}

		public Pattern Left
		{
			get
			{
			return _left;
			}
		}

		public Pattern Right
		{
			get
			{
			return _right;
			}
		}

		/// <summary>
		/// The type of an '|' is not really defined, hence null is returned.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		_left.typeCheck(stable);
		_right.typeCheck(stable);
		return null;
		}

		public override double Priority
		{
			get
			{
			double left = _left.Priority;
			double right = _right.Priority;
    
			if (left < right)
			{
				return (left);
			}
			else
			{
				return (right);
			}
			}
		}

		public override string ToString()
		{
		return "alternative(" + _left + ", " + _right + ')';
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		_left.translate(classGen, methodGen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle gotot = il.append(new org.apache.bcel.generic.GOTO(null));
		InstructionHandle gotot = il.append(new GOTO(null));
		il.append(methodGen.loadContextNode());
		_right.translate(classGen, methodGen);

		_left._trueList.backPatch(gotot);
		_left._falseList.backPatch(gotot.Next);

		_trueList.append(_right._trueList.add(gotot));
		_falseList.append(_right._falseList);
		}
	}

}