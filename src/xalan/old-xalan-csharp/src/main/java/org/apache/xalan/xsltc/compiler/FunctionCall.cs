using System;
using System.Collections;
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
 * $Id: FunctionCall.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionConstants = org.apache.bcel.generic.InstructionConstants;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using InvokeInstruction = org.apache.bcel.generic.InvokeInstruction;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using IntType = org.apache.xalan.xsltc.compiler.util.IntType;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;
	using MultiHashtable = org.apache.xalan.xsltc.compiler.util.MultiHashtable;
	using ObjectType = org.apache.xalan.xsltc.compiler.util.ObjectType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author Todd Miller
	/// </summary>
	internal class FunctionCall : Expression
	{

		// Name of this function call
		private QName _fname;
		// Arguments to this function call (might not be any)
		private readonly ArrayList _arguments;
		// Empty argument list, used for certain functions
		private static readonly ArrayList EMPTY_ARG_LIST = new ArrayList(0);

		// Valid namespaces for Java function-call extension
		protected internal const string EXT_XSLTC = Constants_Fields.TRANSLET_URI;

		protected internal static readonly string JAVA_EXT_XSLTC = EXT_XSLTC + "/java";

		protected internal const string EXT_XALAN = "http://xml.apache.org/xalan";

		protected internal const string JAVA_EXT_XALAN = "http://xml.apache.org/xalan/java";

		protected internal const string JAVA_EXT_XALAN_OLD = "http://xml.apache.org/xslt/java";

		protected internal const string EXSLT_COMMON = "http://exslt.org/common";

		protected internal const string EXSLT_MATH = "http://exslt.org/math";

		protected internal const string EXSLT_SETS = "http://exslt.org/sets";

		protected internal const string EXSLT_DATETIME = "http://exslt.org/dates-and-times";

		protected internal const string EXSLT_STRINGS = "http://exslt.org/strings";

		// Namespace format constants
		protected internal const int NAMESPACE_FORMAT_JAVA = 0;
		protected internal const int NAMESPACE_FORMAT_CLASS = 1;
		protected internal const int NAMESPACE_FORMAT_PACKAGE = 2;
		protected internal const int NAMESPACE_FORMAT_CLASS_OR_PACKAGE = 3;

		// Namespace format
		private int _namespace_format = NAMESPACE_FORMAT_JAVA;

		/// <summary>
		/// Stores reference to object for non-static Java calls
		/// </summary>
		internal Expression _thisArgument = null;

		// External Java function's class/method/signature
		private string _className;
		private Type _clazz;
		private Method _chosenMethod;
		private Constructor _chosenConstructor;
		private MethodType _chosenMethodType;

		// Encapsulates all unsupported external function calls
		private bool unresolvedExternal;

		// If FunctionCall is a external java constructor 
		private bool _isExtConstructor = false;

		// If the java method is static
		private bool _isStatic = false;

		// Legal conversions between internal and Java types.
		private static readonly MultiHashtable _internal2Java = new MultiHashtable();

		// Legal conversions between Java and internal types.
		private static readonly Hashtable _java2Internal = new Hashtable();

		// The mappings between EXSLT extension namespaces and implementation classes
		private static readonly Hashtable _extensionNamespaceTable = new Hashtable();

		// Extension functions that are implemented in BasisLibrary
		private static readonly Hashtable _extensionFunctionTable = new Hashtable();
		/// <summary>
		/// inner class to used in internal2Java mappings, contains
		/// the Java type and the distance between the internal type and
		/// the Java type. 
		/// </summary>
		internal class JavaType
		{
		public Type type;
		public int distance;

		public JavaType(Type type, int distance)
		{
			this.type = type;
			this.distance = distance;
		}
		public override bool Equals(object query)
		{
			return query.Equals(type);
		}
		}

		/// <summary>
		/// Defines 2 conversion tables:
		/// 1. From internal types to Java types and
		/// 2. From Java types to internal types.
		/// These two tables are used when calling external (Java) functions.
		/// </summary>
		static FunctionCall()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class nodeClass = Class.forName("org.w3c.dom.Node");
			Type nodeClass = Type.GetType("org.w3c.dom.Node");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class nodeListClass = Class.forName("org.w3c.dom.NodeList");
			Type nodeListClass = Type.GetType("org.w3c.dom.NodeList");

			// -- Internal to Java --------------------------------------------

				// Type.Boolean -> { boolean(0), Boolean(1), Object(2) }
			_internal2Java[Type.Boolean] = new JavaType(Boolean.TYPE, 0);
			_internal2Java[Type.Boolean] = new JavaType(typeof(Boolean), 1);
			_internal2Java[Type.Boolean] = new JavaType(typeof(object), 2);

				// Type.Real -> { double(0), Double(1), float(2), long(3), int(4),
				//                short(5), byte(6), char(7), Object(8) }
			_internal2Java[Type.Real] = new JavaType(Double.TYPE, 0);
			_internal2Java[Type.Real] = new JavaType(typeof(Double), 1);
			_internal2Java[Type.Real] = new JavaType(Float.TYPE, 2);
			_internal2Java[Type.Real] = new JavaType(Long.TYPE, 3);
			_internal2Java[Type.Real] = new JavaType(Integer.TYPE, 4);
			_internal2Java[Type.Real] = new JavaType(Short.TYPE, 5);
			_internal2Java[Type.Real] = new JavaType(Byte.TYPE, 6);
			_internal2Java[Type.Real] = new JavaType(Character.TYPE, 7);
			_internal2Java[Type.Real] = new JavaType(typeof(object), 8);

				// Type.Int must be the same as Type.Real
			_internal2Java[Type.Int] = new JavaType(Double.TYPE, 0);
			_internal2Java[Type.Int] = new JavaType(typeof(Double), 1);
			_internal2Java[Type.Int] = new JavaType(Float.TYPE, 2);
			_internal2Java[Type.Int] = new JavaType(Long.TYPE, 3);
			_internal2Java[Type.Int] = new JavaType(Integer.TYPE, 4);
			_internal2Java[Type.Int] = new JavaType(Short.TYPE, 5);
			_internal2Java[Type.Int] = new JavaType(Byte.TYPE, 6);
			_internal2Java[Type.Int] = new JavaType(Character.TYPE, 7);
			_internal2Java[Type.Int] = new JavaType(typeof(object), 8);

				// Type.String -> { String(0), Object(1) }
			_internal2Java[Type.String] = new JavaType(typeof(string), 0);
			_internal2Java[Type.String] = new JavaType(typeof(object), 1);

				// Type.NodeSet -> { NodeList(0), Node(1), Object(2), String(3) }
			_internal2Java[Type.NodeSet] = new JavaType(nodeListClass, 0);
			_internal2Java[Type.NodeSet] = new JavaType(nodeClass, 1);
			_internal2Java[Type.NodeSet] = new JavaType(typeof(object), 2);
			_internal2Java[Type.NodeSet] = new JavaType(typeof(string), 3);

				// Type.Node -> { Node(0), NodeList(1), Object(2), String(3) }
			_internal2Java[Type.Node] = new JavaType(nodeListClass, 0);
			_internal2Java[Type.Node] = new JavaType(nodeClass, 1);
			_internal2Java[Type.Node] = new JavaType(typeof(object), 2);
			_internal2Java[Type.Node] = new JavaType(typeof(string), 3);

				// Type.ResultTree -> { NodeList(0), Node(1), Object(2), String(3) }
			_internal2Java[Type.ResultTree] = new JavaType(nodeListClass, 0);
			_internal2Java[Type.ResultTree] = new JavaType(nodeClass, 1);
			_internal2Java[Type.ResultTree] = new JavaType(typeof(object), 2);
			_internal2Java[Type.ResultTree] = new JavaType(typeof(string), 3);

			_internal2Java[Type.Reference] = new JavaType(typeof(object), 0);

			// Possible conversions between Java and internal types
			_java2Internal[Boolean.TYPE] = Type.Boolean;
			_java2Internal[Void.TYPE] = Type.Void;
			_java2Internal[Character.TYPE] = Type.Real;
			_java2Internal[Byte.TYPE] = Type.Real;
			_java2Internal[Short.TYPE] = Type.Real;
			_java2Internal[Integer.TYPE] = Type.Real;
			_java2Internal[Long.TYPE] = Type.Real;
			_java2Internal[Float.TYPE] = Type.Real;
			_java2Internal[Double.TYPE] = Type.Real;

			_java2Internal[typeof(string)] = Type.String;

			_java2Internal[typeof(object)] = Type.Reference;

			// Conversions from org.w3c.dom.Node/NodeList to internal NodeSet
			_java2Internal[nodeListClass] = Type.NodeSet;
			_java2Internal[nodeClass] = Type.NodeSet;

			// Initialize the extension namespace table
			_extensionNamespaceTable[EXT_XALAN] = "org.apache.xalan.lib.Extensions";
			_extensionNamespaceTable[EXSLT_COMMON] = "org.apache.xalan.lib.ExsltCommon";
			_extensionNamespaceTable[EXSLT_MATH] = "org.apache.xalan.lib.ExsltMath";
			_extensionNamespaceTable[EXSLT_SETS] = "org.apache.xalan.lib.ExsltSets";
			_extensionNamespaceTable[EXSLT_DATETIME] = "org.apache.xalan.lib.ExsltDatetime";
			_extensionNamespaceTable[EXSLT_STRINGS] = "org.apache.xalan.lib.ExsltStrings";

			// Initialize the extension function table
			_extensionFunctionTable[EXSLT_COMMON + ":nodeSet"] = "nodeset";
			_extensionFunctionTable[EXSLT_COMMON + ":objectType"] = "objectType";
			_extensionFunctionTable[EXT_XALAN + ":nodeset"] = "nodeset";
		}
		catch (ClassNotFoundException e)
		{
			Console.Error.WriteLine(e);
		}
		}

		public FunctionCall(QName fname, ArrayList arguments)
		{
		_fname = fname;
		_arguments = arguments;
		_type = null;
		}

		public FunctionCall(QName fname) : this(fname, EMPTY_ARG_LIST)
		{
		}

		public virtual string Name
		{
			get
			{
			return (_fname.ToString());
			}
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			if (_arguments != null)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int n = _arguments.size();
				int n = _arguments.Count;
				for (int i = 0; i < n; i++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Expression exp = (Expression)_arguments.elementAt(i);
				Expression exp = (Expression)_arguments[i];
				exp.Parser = value;
				exp.Parent = this;
				}
			}
			}
		}

		public virtual string getClassNameFromUri(string uri)
		{
			string className = (string)_extensionNamespaceTable[uri];

			if (!string.ReferenceEquals(className, null))
			{
				return className;
			}
			else
			{
				if (uri.StartsWith(JAVA_EXT_XSLTC, StringComparison.Ordinal))
				{
					  int length = JAVA_EXT_XSLTC.Length + 1;
					return (uri.Length > length) ? uri.Substring(length) : Constants_Fields.EMPTYSTRING;
				}
				else if (uri.StartsWith(JAVA_EXT_XALAN, StringComparison.Ordinal))
				{
					  int length = JAVA_EXT_XALAN.Length + 1;
					return (uri.Length > length) ? uri.Substring(length) : Constants_Fields.EMPTYSTRING;
				}
				else if (uri.StartsWith(JAVA_EXT_XALAN_OLD, StringComparison.Ordinal))
				{
					  int length = JAVA_EXT_XALAN_OLD.Length + 1;
					return (uri.Length > length) ? uri.Substring(length) : Constants_Fields.EMPTYSTRING;
				}
				else
				{
					  int index = uri.LastIndexOf('/');
					  return (index > 0) ? uri.Substring(index + 1) : uri;
				}
			}
		}

		/// <summary>
		/// Type check a function call. Since different type conversions apply,
		/// type checking is different for standard and external (Java) functions.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
			if (_type != null)
			{
				return _type;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String namespace = _fname.getNamespace();
		string @namespace = _fname.Namespace;
		string local = _fname.LocalPart;

		if (Extension)
		{
			_fname = new QName(null, null, local);
			return typeCheckStandard(stable);
		}
		else if (Standard)
		{
			return typeCheckStandard(stable);
		}
		// Handle extension functions (they all have a namespace)
		else
		{
			try
			{
				_className = getClassNameFromUri(@namespace);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pos = local.lastIndexOf('.');
					int pos = local.LastIndexOf('.');
			if (pos > 0)
			{
				_isStatic = true;
				if (!string.ReferenceEquals(_className, null) && _className.Length > 0)
				{
					_namespace_format = NAMESPACE_FORMAT_PACKAGE;
					 _className = _className + "." + local.Substring(0, pos);
				}
				else
				{
					 _namespace_format = NAMESPACE_FORMAT_JAVA;
					 _className = local.Substring(0, pos);
				}

				_fname = new QName(@namespace, null, local.Substring(pos + 1));
			}
			else
			{
				if (!string.ReferenceEquals(_className, null) && _className.Length > 0)
				{
					try
					{
								_clazz = ObjectFactory.findProviderClass(_className, ObjectFactory.findClassLoader(), true);
						_namespace_format = NAMESPACE_FORMAT_CLASS;
					}
					catch (ClassNotFoundException)
					{
						  _namespace_format = NAMESPACE_FORMAT_PACKAGE;
					}
				}
				else
				{
						_namespace_format = NAMESPACE_FORMAT_JAVA;
				}

				if (local.IndexOf('-') > 0)
				{
					local = replaceDash(local);
				}

				string extFunction = (string)_extensionFunctionTable[@namespace + ":" + local];
				if (!string.ReferenceEquals(extFunction, null))
				{
					  _fname = new QName(null, null, extFunction);
					  return typeCheckStandard(stable);
				}
				else
				{
					  _fname = new QName(@namespace, null, local);
				}
			}

			return typeCheckExternal(stable);
			}
			catch (TypeCheckError e)
			{
			ErrorMsg errorMsg = e.ErrorMsg;
			if (errorMsg == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _fname.getLocalPart();
				string name = _fname.LocalPart;
				errorMsg = new ErrorMsg(ErrorMsg.METHOD_NOT_FOUND_ERR, name);
			}
			Parser.reportError(Constants_Fields.ERROR, errorMsg);
			return _type = Type.Void;
			}
		}
		}

		/// <summary>
		/// Type check a call to a standard function. Insert CastExprs when needed.
		/// If as a result of the insertion of a CastExpr a type check error is 
		/// thrown, then catch it and re-throw it with a new "this".
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheckStandard(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public virtual Type typeCheckStandard(SymbolTable stable)
		{
		_fname.clearNamespace(); // HACK!!!

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _arguments.size();
		int n = _arguments.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector argsType = typeCheckArgs(stable);
		ArrayList argsType = typeCheckArgs(stable);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodType args = new org.apache.xalan.xsltc.compiler.util.MethodType(org.apache.xalan.xsltc.compiler.util.Type.Void, argsType);
		MethodType args = new MethodType(Type.Void, argsType);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodType ptype = lookupPrimop(stable, _fname.getLocalPart(), args);
		MethodType ptype = lookupPrimop(stable, _fname.LocalPart, args);

		if (ptype != null)
		{
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type argType = (org.apache.xalan.xsltc.compiler.util.Type) ptype.argsType().elementAt(i);
			Type argType = (Type) ptype.argsType()[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = (Expression)_arguments.elementAt(i);
			Expression exp = (Expression)_arguments[i];
			if (!argType.identicalTo(exp.Type))
			{
				try
				{
				_arguments[i] = new CastExpr(exp, argType);
				}
				catch (TypeCheckError)
				{
				throw new TypeCheckError(this); // invalid conversion
				}
			}
			}
			_chosenMethodType = ptype;
			return _type = ptype.resultType();
		}
		throw new TypeCheckError(this);
		}



//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheckConstructor(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public virtual Type typeCheckConstructor(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector constructors = findConstructors();
			ArrayList constructors = findConstructors();
		if (constructors == null)
		{
				// Constructor not found in this class
				throw new TypeCheckError(ErrorMsg.CONSTRUCTOR_NOT_FOUND, _className);

		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nConstructors = constructors.size();
		int nConstructors = constructors.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nArgs = _arguments.size();
		int nArgs = _arguments.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector argsType = typeCheckArgs(stable);
		ArrayList argsType = typeCheckArgs(stable);

		// Try all constructors 
		int bestConstrDistance = int.MaxValue;
		_type = null; // reset
		for (int j, i = 0; i < nConstructors; i++)
		{
			// Check if all parameters to this constructor can be converted
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Constructor constructor = (Constructor)constructors.elementAt(i);
			Constructor constructor = (Constructor)constructors[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class[] paramTypes = constructor.getParameterTypes();
			Type[] paramTypes = constructor.ParameterTypes;

			Type extType = null;
			int currConstrDistance = 0;
			for (j = 0; j < nArgs; j++)
			{
			// Convert from internal (translet) type to external (Java) type
			extType = paramTypes[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type intType = (org.apache.xalan.xsltc.compiler.util.Type)argsType.elementAt(j);
			Type intType = (Type)argsType[j];
			object match = _internal2Java.maps(intType, extType);
			if (match != null)
			{
				currConstrDistance += ((JavaType)match).distance;
			}
			else if (intType is ObjectType)
			{
				ObjectType objectType = (ObjectType)intType;
				if (objectType.JavaClass == extType)
				{
					continue;
				}
				else if (extType.IsAssignableFrom(objectType.JavaClass))
				{
					currConstrDistance += 1;
				}
				else
				{
				currConstrDistance = int.MaxValue;
				break;
				}
			}
			else
			{
				// no mapping available
				currConstrDistance = int.MaxValue;
				break;
			}
			}

			if (j == nArgs && currConstrDistance < bestConstrDistance)
			{
				_chosenConstructor = constructor;
				_isExtConstructor = true;
			bestConstrDistance = currConstrDistance;

					_type = (_clazz != null) ? Type.newObjectType(_clazz) : Type.newObjectType(_className);
			}
		}

		if (_type != null)
		{
			return _type;
		}

		throw new TypeCheckError(ErrorMsg.ARGUMENT_CONVERSION_ERR, getMethodSignature(argsType));
		}


		/// <summary>
		/// Type check a call to an external (Java) method.
		/// The method must be static an public, and a legal type conversion
		/// must exist for all its arguments and its return type.
		/// Every method of name <code>_fname</code> is inspected
		/// as a possible candidate.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheckExternal(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public virtual Type typeCheckExternal(SymbolTable stable)
		{
		int nArgs = _arguments.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _fname.getLocalPart();
		string name = _fname.LocalPart;

		 // check if function is a contructor 'new'
		if (_fname.LocalPart.Equals("new"))
		{
			return typeCheckConstructor(stable);
		}
		// check if we are calling an instance method
		else
		{
			bool hasThisArgument = false;

			if (nArgs == 0)
			{
				_isStatic = true;
			}

			if (!_isStatic)
			{
				if (_namespace_format == NAMESPACE_FORMAT_JAVA || _namespace_format == NAMESPACE_FORMAT_PACKAGE)
				{
				   hasThisArgument = true;
				}

			  Expression firstArg = (Expression)_arguments[0];
			  Type firstArgType = (Type)firstArg.typeCheck(stable);

			  if (_namespace_format == NAMESPACE_FORMAT_CLASS && firstArgType is ObjectType && _clazz != null && _clazz.IsAssignableFrom(((ObjectType)firstArgType).JavaClass))
			  {
				  hasThisArgument = true;
			  }

			  if (hasThisArgument)
			  {
				  _thisArgument = (Expression) _arguments[0];
				  _arguments.RemoveAt(0);
				  nArgs--;
				if (firstArgType is ObjectType)
				{
					_className = ((ObjectType) firstArgType).JavaClassName;
				}
				else
				{
					throw new TypeCheckError(ErrorMsg.NO_JAVA_FUNCT_THIS_REF, name);
				}
			  }
			}
			else if (_className.Length == 0)
			{
			/*
			 * Warn user if external function could not be resolved.
			 * Warning will _NOT_ be issued is the call is properly
			 * wrapped in an <xsl:if> or <xsl:when> element. For details
			 * see If.parserContents() and When.parserContents()
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parser parser = getParser();
			Parser parser = Parser;
			if (parser != null)
			{
				reportWarning(this, parser, ErrorMsg.FUNCTION_RESOLVE_ERR, _fname.ToString());
			}
			unresolvedExternal = true;
			return _type = Type.Int; // use "Int" as "unknown"
			}
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector methods = findMethods();
		ArrayList methods = findMethods();

		if (methods == null)
		{
			// Method not found in this class
			throw new TypeCheckError(ErrorMsg.METHOD_NOT_FOUND_ERR, _className + "." + name);
		}

		Type extType = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nMethods = methods.size();
		int nMethods = methods.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector argsType = typeCheckArgs(stable);
		ArrayList argsType = typeCheckArgs(stable);

		// Try all methods to identify the best fit 
		int bestMethodDistance = int.MaxValue;
		_type = null; // reset internal type
		for (int j, i = 0; i < nMethods; i++)
		{
			// Check if all paramteters to this method can be converted
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Method method = (Method)methods.elementAt(i);
			Method method = (Method)methods[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class[] paramTypes = method.getParameterTypes();
			Type[] paramTypes = method.ParameterTypes;

			int currMethodDistance = 0;
			for (j = 0; j < nArgs; j++)
			{
			// Convert from internal (translet) type to external (Java) type
			extType = paramTypes[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type intType = (org.apache.xalan.xsltc.compiler.util.Type)argsType.elementAt(j);
			Type intType = (Type)argsType[j];
			object match = _internal2Java.maps(intType, extType);
			if (match != null)
			{
				currMethodDistance += ((JavaType)match).distance;
			}
			else
			{
				// no mapping available
				//
				// Allow a Reference type to match any external (Java) type at
				// the moment. The real type checking is performed at runtime.
				if (intType is ReferenceType)
				{
				   currMethodDistance += 1;
				}
				else if (intType is ObjectType)
				{
					ObjectType @object = (ObjectType)intType;
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					if (extType.FullName.Equals(@object.JavaClassName))
					{
						currMethodDistance += 0;
					}
					  else if (extType.IsAssignableFrom(@object.JavaClass))
					  {
						currMethodDistance += 1;
					  }
					  else
					  {
						  currMethodDistance = int.MaxValue;
						  break;
					  }
				}
				else
				{
					currMethodDistance = int.MaxValue;
					break;
				}
			}
			}

			if (j == nArgs)
			{
			  // Check if the return type can be converted
			  extType = method.ReturnType;

			  _type = (Type) _java2Internal[extType];
			  if (_type == null)
			  {
				  _type = Type.newObjectType(extType);
			  }

			  // Use this method if all parameters & return type match
			  if (_type != null && currMethodDistance < bestMethodDistance)
			  {
				  _chosenMethod = method;
				  bestMethodDistance = currMethodDistance;
			  }
			}
		}

		// It is an error if the chosen method is an instance menthod but we don't
		// have a this argument.
		if (_chosenMethod != null && _thisArgument == null && !Modifier.isStatic(_chosenMethod.Modifiers))
		{
			throw new TypeCheckError(ErrorMsg.NO_JAVA_FUNCT_THIS_REF, getMethodSignature(argsType));
		}

		if (_type != null)
		{
			if (_type == Type.NodeSet)
			{
					XSLTC.MultiDocument = true;
			}
			return _type;
		}

		throw new TypeCheckError(ErrorMsg.ARGUMENT_CONVERSION_ERR, getMethodSignature(argsType));
		}

		/// <summary>
		/// Type check the actual arguments of this function call.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Vector typeCheckArgs(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public virtual ArrayList typeCheckArgs(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector result = new java.util.Vector();
		ArrayList result = new ArrayList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration e = _arguments.elements();
		System.Collections.IEnumerator e = _arguments.elements();
		while (e.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = (Expression)e.Current;
			Expression exp = (Expression)e.Current;
			result.Add(exp.typeCheck(stable));
		}
		return result;
		}

		protected internal Expression argument(int i)
		{
		return (Expression)_arguments[i];
		}

		protected internal Expression argument()
		{
		return argument(0);
		}

		protected internal int argumentCount()
		{
		return _arguments.Count;
		}

		protected internal void setArgument(int i, Expression exp)
		{
		_arguments[i] = exp;
		}

		/// <summary>
		/// Compile the function call and treat as an expression
		/// Update true/false-lists.
		/// </summary>
		public override void translateDesynthesized(ClassGenerator classGen, MethodGenerator methodGen)
		{
		Type type = Type.Boolean;
		if (_chosenMethodType != null)
		{
			type = _chosenMethodType.resultType();
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
		translate(classGen, methodGen);

		if ((type is BooleanType) || (type is IntType))
		{
			_falseList.add(il.append(new IFEQ(null)));
		}
		}


		/// <summary>
		/// Translate a function call. The compiled code will leave the function's
		/// return value on the JVM's stack.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = argumentCount();
		int n = argumentCount();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isSecureProcessing = classGen.getParser().getXSLTC().isSecureProcessing();
		bool isSecureProcessing = classGen.Parser.XSLTC.SecureProcessing;
		int index;

		// Translate calls to methods in the BasisLibrary
		if (Standard || Extension)
		{
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = argument(i);
			Expression exp = argument(i);
			exp.translate(classGen, methodGen);
			exp.startIterator(classGen, methodGen);
			}

			// append "F" to the function's name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _fname.toString().replace('-', '_') + "F";
			string name = _fname.ToString().Replace('-', '_') + "F";
			string args = Constants_Fields.EMPTYSTRING;

			// Special precautions for some method calls
			if (name.Equals("sumF"))
			{
			args = Constants_Fields.DOM_INTF_SIG;
			il.append(methodGen.loadDOM());
			}
			else if (name.Equals("normalize_spaceF"))
			{
			if (_chosenMethodType.toSignature(args).Equals("()Ljava/lang/String;"))
			{
				args = "I" + Constants_Fields.DOM_INTF_SIG;
				il.append(methodGen.loadContextNode());
				il.append(methodGen.loadDOM());
			}
			}

			// Invoke the method in the basis library
			index = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, name, _chosenMethodType.toSignature(args));
			il.append(new INVOKESTATIC(index));
		}
		// Add call to BasisLibrary.unresolved_externalF() to generate
		// run-time error message for unsupported external functions
		else if (unresolvedExternal)
		{
			index = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "unresolved_externalF", "(Ljava/lang/String;)V");
			il.append(new PUSH(cpg, _fname.ToString()));
			il.append(new INVOKESTATIC(index));
		}
		else if (_isExtConstructor)
		{
			if (isSecureProcessing)
			{
				translateUnallowedExtension(cpg, il);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String clazz = _chosenConstructor.getDeclaringClass().getName();
			string clazz = _chosenConstructor.DeclaringClass.Name;
			Type[] paramTypes = _chosenConstructor.ParameterTypes;
				LocalVariableGen[] paramTemp = new LocalVariableGen[n];

				// Backwards branches are prohibited if an uninitialized object is
				// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
				// We don't know whether this code might contain backwards branches
				// so we mustn't create the new object until after we've created
				// the suspect arguments to its constructor.  Instead we calculate
				// the values of the arguments to the constructor first, store them
				// in temporary variables, create the object and reload the
				// arguments from the temporaries to avoid the problem.

			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = argument(i);
			Expression exp = argument(i);
					Type expType = exp.Type;
			exp.translate(classGen, methodGen);
			// Convert the argument to its Java type
			exp.startIterator(classGen, methodGen);
			expType.translateTo(classGen, methodGen, paramTypes[i]);
					paramTemp[i] = methodGen.addLocalVariable("function_call_tmp" + i, expType.toJCType(), null, null);
					paramTemp[i].Start = il.append(expType.STORE(paramTemp[i].Index));
			}

			il.append(new NEW(cpg.addClass(_className)));
			il.append(InstructionConstants.DUP);

				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression arg = argument(i);
					Expression arg = argument(i);
					paramTemp[i].End = il.append(arg.Type.LOAD(paramTemp[i].Index));
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
			StringBuilder buffer = new StringBuilder();
			buffer.Append('(');
			for (int i = 0; i < paramTypes.Length; i++)
			{
			buffer.Append(getSignature(paramTypes[i]));
			}
			buffer.Append(')');
			buffer.Append("V");

			index = cpg.addMethodref(clazz, "<init>", buffer.ToString());
			il.append(new INVOKESPECIAL(index));

			// Convert the return type back to our internal type
			(Type.Object).translateFrom(classGen, methodGen, _chosenConstructor.DeclaringClass);

		}
		// Invoke function calls that are handled in separate classes
		else
		{
			if (isSecureProcessing)
			{
				translateUnallowedExtension(cpg, il);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String clazz = _chosenMethod.getDeclaringClass().getName();
			string clazz = _chosenMethod.DeclaringClass.Name;
			Type[] paramTypes = _chosenMethod.ParameterTypes;

			// Push "this" if it is an instance method
			if (_thisArgument != null)
			{
			_thisArgument.translate(classGen, methodGen);
			}

			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression exp = argument(i);
			Expression exp = argument(i);
			exp.translate(classGen, methodGen);
			// Convert the argument to its Java type
			exp.startIterator(classGen, methodGen);
			exp.Type.translateTo(classGen, methodGen, paramTypes[i]);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buffer = new StringBuffer();
			StringBuilder buffer = new StringBuilder();
			buffer.Append('(');
			for (int i = 0; i < paramTypes.Length; i++)
			{
			buffer.Append(getSignature(paramTypes[i]));
			}
			buffer.Append(')');
			buffer.Append(getSignature(_chosenMethod.ReturnType));

			if (_thisArgument != null && _clazz.IsInterface)
			{
				index = cpg.addInterfaceMethodref(clazz, _fname.LocalPart, buffer.ToString());
			il.append(new INVOKEINTERFACE(index, n + 1));
			}
				else
				{
				index = cpg.addMethodref(clazz, _fname.LocalPart, buffer.ToString());
				il.append(_thisArgument != null ? (InvokeInstruction) new INVOKEVIRTUAL(index) : (InvokeInstruction) new INVOKESTATIC(index));
				}

			// Convert the return type back to our internal type
			_type.translateFrom(classGen, methodGen, _chosenMethod.ReturnType);
		}
		}

		public override string ToString()
		{
		return "funcall(" + _fname + ", " + _arguments + ')';
		}

		public virtual bool Standard
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String namespace = _fname.getNamespace();
			string @namespace = _fname.Namespace;
			return (string.ReferenceEquals(@namespace, null)) || (@namespace.Equals(Constants_Fields.EMPTYSTRING));
			}
		}

		public virtual bool Extension
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String namespace = _fname.getNamespace();
			string @namespace = _fname.Namespace;
			return (!string.ReferenceEquals(@namespace, null)) && (@namespace.Equals(EXT_XSLTC));
			}
		}

		/// <summary>
		/// Returns a vector with all methods named <code>_fname</code>
		/// after stripping its namespace or <code>null</code>
		/// if no such methods exist.
		/// </summary>
		private ArrayList findMethods()
		{

		  ArrayList result = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String namespace = _fname.getNamespace();
		  string @namespace = _fname.Namespace;

		  if (!string.ReferenceEquals(_className, null) && _className.Length > 0)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nArgs = _arguments.size();
			int nArgs = _arguments.Count;
			try
			{
			  if (_clazz == null)
			  {
					_clazz = ObjectFactory.findProviderClass(_className, ObjectFactory.findClassLoader(), true);

			if (_clazz == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg msg = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
			  ErrorMsg msg = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
			  Parser.reportError(Constants_Fields.ERROR, msg);
			}
			  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String methodName = _fname.getLocalPart();
			  string methodName = _fname.LocalPart;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Method[] methods = _clazz.getMethods();
			  Method[] methods = _clazz.GetMethods();

			  for (int i = 0; i < methods.Length; i++)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mods = methods[i].getModifiers();
			int mods = methods[i].Modifiers;
			// Is it public and same number of args ?
			if (Modifier.isPublic(mods) && methods[i].Name.Equals(methodName) && methods[i].ParameterTypes.length == nArgs)
			{
			  if (result == null)
			  {
				result = new ArrayList();
			  }
			  result.Add(methods[i]);
			}
			  }
			}
			catch (ClassNotFoundException)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg msg = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
			  ErrorMsg msg = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
			  Parser.reportError(Constants_Fields.ERROR, msg);
			}
		  }
		  return result;
		}

		/// <summary>
		/// Returns a vector with all constructors named <code>_fname</code>
		/// after stripping its namespace or <code>null</code>
		/// if no such methods exist.
		/// </summary>
		private ArrayList findConstructors()
		{
			ArrayList result = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String namespace = _fname.getNamespace();
			string @namespace = _fname.Namespace;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nArgs = _arguments.size();
			int nArgs = _arguments.Count;
			try
			{
			  if (_clazz == null)
			  {
				_clazz = ObjectFactory.findProviderClass(_className, ObjectFactory.findClassLoader(), true);

				if (_clazz == null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg msg = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
				  ErrorMsg msg = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
				  Parser.reportError(Constants_Fields.ERROR, msg);
				}
			  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Constructor[] constructors = _clazz.getConstructors();
			  Constructor[] constructors = _clazz.GetConstructors();

			  for (int i = 0; i < constructors.Length; i++)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int mods = constructors[i].getModifiers();
				  int mods = constructors[i].Modifiers;
				  // Is it public, static and same number of args ?
				  if (Modifier.isPublic(mods) && constructors[i].ParameterTypes.length == nArgs)
				  {
					if (result == null)
					{
					  result = new ArrayList();
					}
					result.Add(constructors[i]);
				  }
			  }
			}
			catch (ClassNotFoundException)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg msg = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
			  ErrorMsg msg = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR, _className);
			  Parser.reportError(Constants_Fields.ERROR, msg);
			}

			return result;
		}


		/// <summary>
		/// Compute the JVM signature for the class.
		/// </summary>
		internal static string getSignature(Type clazz)
		{
		if (clazz.IsArray)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
			StringBuilder sb = new StringBuilder();
			Type cl = clazz;
			while (cl.IsArray)
			{
			sb.Append("[");
			cl = cl.GetElementType();
			}
			sb.Append(getSignature(cl));
			return sb.ToString();
		}
		else if (clazz.IsPrimitive)
		{
			if (clazz == Integer.TYPE)
			{
			return "I";
			}
			else if (clazz == Byte.TYPE)
			{
			return "B";
			}
			else if (clazz == Long.TYPE)
			{
			return "J";
			}
			else if (clazz == Float.TYPE)
			{
			return "F";
			}
			else if (clazz == Double.TYPE)
			{
			return "D";
			}
			else if (clazz == Short.TYPE)
			{
			return "S";
			}
			else if (clazz == Character.TYPE)
			{
			return "C";
			}
			else if (clazz == Boolean.TYPE)
			{
			return "Z";
			}
			else if (clazz == Void.TYPE)
			{
			return "V";
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = clazz.toString();
			string name = clazz.ToString();
			ErrorMsg err = new ErrorMsg(ErrorMsg.UNKNOWN_SIG_TYPE_ERR,name);
			throw new Exception(err.ToString());
			}
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return "L" + clazz.FullName.Replace('.', '/') + ';';
		}
		}

		/// <summary>
		/// Compute the JVM method descriptor for the method.
		/// </summary>
		internal static string getSignature(Method meth)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
		StringBuilder sb = new StringBuilder();
		sb.Append('(');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class[] params = meth.getParameterTypes();
		Type[] @params = meth.ParameterTypes; // avoid clone
		for (int j = 0; j < @params.Length; j++)
		{
			sb.Append(getSignature(@params[j]));
		}
		return sb.Append(')').Append(getSignature(meth.ReturnType)).ToString();
		}

		/// <summary>
		/// Compute the JVM constructor descriptor for the constructor.
		/// </summary>
		internal static string getSignature(Constructor cons)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer sb = new StringBuffer();
		StringBuilder sb = new StringBuilder();
		sb.Append('(');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class[] params = cons.getParameterTypes();
		Type[] @params = cons.ParameterTypes; // avoid clone
		for (int j = 0; j < @params.Length; j++)
		{
			sb.Append(getSignature(@params[j]));
		}
		return sb.Append(")V").ToString();
		}

		/// <summary>
		/// Return the signature of the current method
		/// </summary>
		private string getMethodSignature(ArrayList argsType)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer buf = new StringBuffer(_className);
		 StringBuilder buf = new StringBuilder(_className);
			buf.Append('.').Append(_fname.LocalPart).Append('(');

		int nArgs = argsType.Count;
		for (int i = 0; i < nArgs; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type intType = (org.apache.xalan.xsltc.compiler.util.Type)argsType.elementAt(i);
			Type intType = (Type)argsType[i];
			buf.Append(intType.ToString());
			if (i < nArgs - 1)
			{
				buf.Append(", ");
			}
		}

		buf.Append(')');
		return buf.ToString();
		}

		/// <summary>
		/// To support EXSLT extensions, convert names with dash to allowable Java names: 
		/// e.g., convert abc-xyz to abcXyz.
		/// Note: dashes only appear in middle of an EXSLT function or element name.
		/// </summary>
		protected internal static string replaceDash(string name)
		{
			char dash = '-';
			StringBuilder buff = new StringBuilder("");
			for (int i = 0; i < name.Length; i++)
			{
			if (i > 0 && name[i - 1] == dash)
			{
				buff.Append(char.ToUpper(name[i]));
			}
			else if (name[i] != dash)
			{
				buff.Append(name[i]);
			}
			}
			return buff.ToString();
		}

		/// <summary>
		/// Translate code to call the BasisLibrary.unallowed_extensionF(String)
		/// method.
		/// </summary>
		private void translateUnallowedExtension(ConstantPoolGen cpg, InstructionList il)
		{
		int index = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "unallowed_extension_functionF", "(Ljava/lang/String;)V");
		il.append(new PUSH(cpg, _fname.ToString()));
		il.append(new INVOKESTATIC(index));
		}
	}

}