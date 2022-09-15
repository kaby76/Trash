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
 * $Id: When.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class When : Instruction
	{

		private Expression _test;
		private bool _ignore = false;

		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("When");
		this.indent(indent + IndentIncrement);
		Console.Write("test ");
		Util.println(_test.ToString());
		displayContents(indent + IndentIncrement);
		}

		public Expression Test
		{
			get
			{
			return _test;
			}
		}

		public bool ignore()
		{
		return (_ignore);
		}

		public override void parseContents(Parser parser)
		{
		_test = parser.parseExpression(this, "test", null);

		// Ignore xsl:if when test is false (function-available() and
		// element-available())
		object result = _test.evaluateAtCompileTime();
		if (result != null && result is Boolean)
		{
			_ignore = !((bool?) result).Value;
		}

		parseChildren(parser);

		// Make sure required attribute(s) have been set
		if (_test.Dummy)
		{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "test");
		}
		}

		/// <summary>
		/// Type-check this when element. The test should always be type checked,
		/// while we do not bother with the contents if we know the test fails.
		/// This is important in cases where the "test" expression tests for
		/// the support of a non-available element, and the <xsl:when> body contains
		/// this non-available element.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		// Type-check the test expression
		if (_test.typeCheck(stable) is BooleanType == false)
		{
			_test = new CastExpr(_test, Type.Boolean);
		}
		// Type-check the contents (if necessary)
		if (!_ignore)
		{
			typeCheckContents(stable);
		}

		return Type.Void;
		}

		/// <summary>
		/// This method should never be called. An Otherwise object will explicitly
		/// translate the "test" expression and and contents of this element.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg msg = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.STRAY_WHEN_ERR, this);
		ErrorMsg msg = new ErrorMsg(ErrorMsg.STRAY_WHEN_ERR, this);
		Parser.reportError(Constants.ERROR, msg);
		}
	}

}