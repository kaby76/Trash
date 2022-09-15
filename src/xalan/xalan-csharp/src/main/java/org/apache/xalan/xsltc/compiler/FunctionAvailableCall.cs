using System;
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
 * $Id: FunctionAvailableCall.java 1225364 2011-12-28 22:45:16Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author G. Todd Miller 
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class FunctionAvailableCall : FunctionCall
	{

		private Expression _arg;
		private string _nameOfFunct = null;
		private string _namespaceOfFunct = null;
		private bool _isFunctionAvailable = false;

		/// <summary>
		/// Constructs a FunctionAvailableCall FunctionCall. Takes the
		/// function name qname, for example, 'function-available', and 
		/// a list of arguments where the arguments must be instances of 
		/// LiteralExpression. 
		/// </summary>
		public FunctionAvailableCall(QName fname, ArrayList arguments) : base(fname, arguments)
		{
		_arg = (Expression)arguments[0];
		_type = null;

			if (_arg is LiteralExpr)
			{
			LiteralExpr arg = (LiteralExpr) _arg;
				_namespaceOfFunct = arg.Namespace;
				_nameOfFunct = arg.Value;

				if (!InternalNamespace)
				{
				  _isFunctionAvailable = hasMethods();
				}
			}
		}

		/// <summary>
		/// Argument of function-available call must be literal, typecheck
		/// returns the type of function-available to be boolean.  
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_type != null)
		{
		   return _type;
		}
		if (_arg is LiteralExpr)
		{
			return _type = Type.Boolean;
		}
		ErrorMsg err = new ErrorMsg(ErrorMsg.NEED_LITERAL_ERR, "function-available", this);
		throw new TypeCheckError(err);
		}

		/// <summary>
		/// Returns an object representing the compile-time evaluation 
		/// of an expression. We are only using this for function-available
		/// and element-available at this time.
		/// </summary>
		public override object evaluateAtCompileTime()
		{
		return Result ? true : false;
		}

		/// <summary>
		/// for external java functions only: reports on whether or not
		/// the specified method is found in the specifed class. 
		/// </summary>
		private bool hasMethods()
		{

		// Get the class name from the namespace uri
		string className = getClassNameFromUri(_namespaceOfFunct);

		// Get the method name from the argument to function-available
		string methodName = null;
		int colonIndex = _nameOfFunct.IndexOf(":", StringComparison.Ordinal);
		if (colonIndex > 0)
		{
		  string functionName = _nameOfFunct.Substring(colonIndex + 1);
		  int lastDotIndex = functionName.LastIndexOf('.');
		  if (lastDotIndex > 0)
		  {
			methodName = functionName.Substring(lastDotIndex + 1);
			if (!string.ReferenceEquals(className, null) && className.Length != 0)
			{
			  className = className + "." + functionName.Substring(0, lastDotIndex);
			}
			else
			{
			  className = functionName.Substring(0, lastDotIndex);
			}
		  }
		  else
		  {
			methodName = functionName;
		  }
		}
		else
		{
		  methodName = _nameOfFunct;
		}

		if (string.ReferenceEquals(className, null) || string.ReferenceEquals(methodName, null))
		{
			return false;
		}

		// Replace the '-' characters in the method name
		if (methodName.IndexOf('-') > 0)
		{
		  methodName = replaceDash(methodName);
		}

		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class clazz = ObjectFactory.findProviderClass(className, ObjectFactory.findClassLoader(), true);
				Type clazz = ObjectFactory.findProviderClass(className, ObjectFactory.findClassLoader(), true);

			if (clazz == null)
			{
				return false;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.lang.reflect.Method[] methods = clazz.getMethods();
			System.Reflection.MethodInfo[] methods = clazz.GetMethods();

			for (int i = 0; i < methods.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mods = methods[i].getModifiers();
			int mods = methods[i].getModifiers();

			if (Modifier.isPublic(mods) && Modifier.isStatic(mods) && methods[i].getName().Equals(methodName))
			{
				return true;
			}
			}
		}
		catch (ClassNotFoundException)
		{
		  return false;
		}
			return false;
		}

		/// <summary>
		/// Reports on whether the function specified in the argument to
		/// xslt function 'function-available' was found.
		/// </summary>
		public bool Result
		{
			get
			{
			if (string.ReferenceEquals(_nameOfFunct, null))
			{
				return false;
			}
    
				if (InternalNamespace)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Parser parser = getParser();
					Parser parser = Parser;
					_isFunctionAvailable = parser.functionSupported(Util.getLocalName(_nameOfFunct));
				}
			 return _isFunctionAvailable;
			}
		}

		/// <summary>
		/// Return true if the namespace uri is null or it is the XSLTC translet uri.
		/// </summary>
		private bool InternalNamespace
		{
			get
			{
				return (string.ReferenceEquals(_namespaceOfFunct, null) || _namespaceOfFunct.Equals(EMPTYSTRING) || _namespaceOfFunct.Equals(TRANSLET_URI));
			}
		}

		/// <summary>
		/// Calls to 'function-available' are resolved at compile time since 
		/// the namespaces declared in the stylsheet are not available at run
		/// time. Consequently, arguments to this function must be literals.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
		methodGen.getInstructionList().append(new PUSH(cpg, Result));
		}

	}

}