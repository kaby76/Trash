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
 * $Id: UnresolvedRef.java 476471 2006-11-18 08:36:27Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class UnresolvedRef : VariableRefBase
	{

		private QName _variableName = null;
		private VariableRefBase _ref = null;

		public UnresolvedRef(QName name) : base()
		{
		_variableName = name;
		}

		public QName Name
		{
			get
			{
			return (_variableName);
			}
		}

		private ErrorMsg reportError()
		{
		ErrorMsg err = new ErrorMsg(ErrorMsg.VARIABLE_UNDEF_ERR, _variableName, this);
		Parser.reportError(Constants_Fields.ERROR, err);
		return (err);
		}

		private VariableRefBase resolve(Parser parser, SymbolTable stable)
		{
		// At this point the AST is already built and we should be able to
		// find any declared global variable or parameter
		VariableBase @ref = parser.lookupVariable(_variableName);
		if (@ref == null)
		{
				@ref = (VariableBase)stable.lookupName(_variableName);
		}
		if (@ref == null)
		{
			reportError();
			return null;
		}

			// If in a top-level element, create dependency to the referenced var
			_variable = @ref;
			addParentDependency();

		if (@ref is Variable)
		{
			return new VariableRef((Variable) @ref);
		}
		else if (@ref is Param)
		{
			return new ParameterRef((Param)@ref);
		}
			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_ref != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _variableName.toString();
			string name = _variableName.ToString();
			ErrorMsg err = new ErrorMsg(ErrorMsg.CIRCULAR_VARIABLE_ERR, name, this);
		}
		if ((_ref = resolve(Parser, stable)) != null)
		{
			return (_type = _ref.typeCheck(stable));
		}
		throw new TypeCheckError(reportError());
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		if (_ref != null)
		{
			_ref.translate(classGen, methodGen);
		}
		else
		{
			reportError();
		}
		}

		public override string ToString()
		{
		return "unresolved-ref()";
		}

	}

}