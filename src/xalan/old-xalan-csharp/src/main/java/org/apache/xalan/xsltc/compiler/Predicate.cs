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
 * $Id: Predicate.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Field = org.apache.bcel.classfile.Field;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using BooleanType = org.apache.xalan.xsltc.compiler.util.BooleanType;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using FilterGenerator = org.apache.xalan.xsltc.compiler.util.FilterGenerator;
	using IntType = org.apache.xalan.xsltc.compiler.util.IntType;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NumberType = org.apache.xalan.xsltc.compiler.util.NumberType;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using ResultTreeType = org.apache.xalan.xsltc.compiler.util.ResultTreeType;
	using TestGenerator = org.apache.xalan.xsltc.compiler.util.TestGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Operators = org.apache.xalan.xsltc.runtime.Operators;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Predicate : Expression, Closure
	{

		/// <summary>
		/// The predicate's expression.
		/// </summary>
		private Expression _exp = null;

		/// <summary>
		/// This flag indicates if optimizations are turned on. The 
		/// method <code>dontOptimize()</code> can be called to turn
		/// optimizations off.
		/// </summary>
		private bool _canOptimize = true;

		/// <summary>
		/// Flag indicatig if the nth position optimization is on. It
		/// is set in <code>typeCheck()</code>.
		/// </summary>
		private bool _nthPositionFilter = false;

		/// <summary>
		/// Flag indicatig if the nth position descendant is on. It
		/// is set in <code>typeCheck()</code>.
		/// </summary>
		private bool _nthDescendant = false;

		/// <summary>
		/// Cached node type of the expression that owns this predicate.
		/// </summary>
		internal int _ptype = -1;

		/// <summary>
		/// Name of the inner class.
		/// </summary>
		private string _className = null;

		/// <summary>
		/// List of variables in closure.
		/// </summary>
		private ArrayList _closureVars = null;

		/// <summary>
		/// Reference to parent closure.
		/// </summary>
		private Closure _parentClosure = null;

		/// <summary>
		/// Cached value of method <code>getCompareValue()</code>.
		/// </summary>
		private Expression _value = null;

		/// <summary>
		/// Cached value of method <code>getCompareValue()</code>.
		/// </summary>
		private Step _step = null;

		/// <summary>
		/// Initializes a predicate. 
		/// </summary>
		public Predicate(Expression exp)
		{
			_exp = exp;
			_exp.Parent = this;

		}

		/// <summary>
		/// Set the parser for this expression.
		/// </summary>
		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_exp.Parser = value;
			}
		}

		/// <summary>
		/// Returns a boolean value indicating if the nth position optimization
		/// is on. Must be call after type checking!
		/// </summary>
		public bool NthPositionFilter
		{
			get
			{
			return _nthPositionFilter;
			}
		}

		/// <summary>
		/// Returns a boolean value indicating if the nth descendant optimization
		/// is on. Must be call after type checking!
		/// </summary>
		public bool NthDescendant
		{
			get
			{
			return _nthDescendant;
			}
		}

		/// <summary>
		/// Turns off all optimizations for this predicate.
		/// </summary>
		public void dontOptimize()
		{
		_canOptimize = false;
		}

		/// <summary>
		/// Returns true if the expression in this predicate contains a call 
		/// to position().
		/// </summary>
		public override bool hasPositionCall()
		{
		return _exp.hasPositionCall();
		}

		/// <summary>
		/// Returns true if the expression in this predicate contains a call 
		/// to last().
		/// </summary>
		public override bool hasLastCall()
		{
		return _exp.hasLastCall();
		}

		// -- Begin Closure interface --------------------

		/// <summary>
		/// Returns true if this closure is compiled in an inner class (i.e.
		/// if this is a real closure).
		/// </summary>
		public bool inInnerClass()
		{
		return (!string.ReferenceEquals(_className, null));
		}

		/// <summary>
		/// Returns a reference to its parent closure or null if outermost.
		/// </summary>
		public Closure ParentClosure
		{
			get
			{
			if (_parentClosure == null)
			{
				SyntaxTreeNode node = Parent;
				do
				{
				if (node is Closure)
				{
					_parentClosure = (Closure) node;
					break;
				}
				if (node is TopLevelElement)
				{
					break; // way up in the tree
				}
				node = node.Parent;
				} while (node != null);
			}
			return _parentClosure;
			}
		}

		/// <summary>
		/// Returns the name of the auxiliary class or null if this predicate 
		/// is compiled inside the Translet.
		/// </summary>
		public string InnerClassName
		{
			get
			{
			return _className;
			}
		}

		/// <summary>
		/// Add new variable to the closure.
		/// </summary>
		public void addVariable(VariableRefBase variableRef)
		{
		if (_closureVars == null)
		{
			_closureVars = new ArrayList();
		}

		// Only one reference per variable
		if (!_closureVars.Contains(variableRef))
		{
			_closureVars.Add(variableRef);

			// Add variable to parent closure as well
			Closure parentClosure = ParentClosure;
			if (parentClosure != null)
			{
			parentClosure.addVariable(variableRef);
			}
		}
		}

		// -- End Closure interface ----------------------

		/// <summary>
		/// Returns the node type of the expression owning this predicate. The
		/// return value is cached in <code>_ptype</code>.
		/// </summary>
		public int PosType
		{
			get
			{
			if (_ptype == -1)
			{
				SyntaxTreeNode parent = Parent;
				if (parent is StepPattern)
				{
				_ptype = ((StepPattern)parent).NodeType;
				}
				else if (parent is AbsoluteLocationPath)
				{
				AbsoluteLocationPath path = (AbsoluteLocationPath)parent;
				Expression exp = path.Path;
				if (exp is Step)
				{
					_ptype = ((Step)exp).NodeType;
				}
				}
				else if (parent is VariableRefBase)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final VariableRefBase ref = (VariableRefBase)parent;
				VariableRefBase @ref = (VariableRefBase)parent;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final VariableBase var = ref.getVariable();
				VariableBase @var = @ref.Variable;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Expression exp = var.getExpression();
				Expression exp = @var.Expression;
				if (exp is Step)
				{
					_ptype = ((Step)exp).NodeType;
				}
				}
				else if (parent is Step)
				{
				_ptype = ((Step)parent).NodeType;
				}
			}
			return _ptype;
			}
		}

		public bool parentIsPattern()
		{
		return (Parent is Pattern);
		}

		public Expression Expr
		{
			get
			{
			return _exp;
			}
		}

		public override string ToString()
		{
			return "pred(" + _exp + ')';
		}

		/// <summary>
		/// Type check a predicate expression. If the type of the expression is 
		/// number convert it to boolean by adding a comparison with position().
		/// Note that if the expression is a parameter, we cannot distinguish
		/// at compile time if its type is number or not. Hence, expressions of 
		/// reference type are always converted to booleans.
		/// 
		/// This method may be called twice, before and after calling
		/// <code>dontOptimize()</code>. If so, the second time it should honor
		/// the new value of <code>_canOptimize</code>.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		Type texp = _exp.typeCheck(stable);

		// We need explicit type information for reference types - no good!
		if (texp is ReferenceType)
		{
			_exp = new CastExpr(_exp, texp = Type.Real);
		}

		// A result tree fragment should not be cast directly to a number type,
		// but rather to a boolean value, and then to a numer (0 or 1).
		// Ref. section 11.2 of the XSLT 1.0 spec
		if (texp is ResultTreeType)
		{
			_exp = new CastExpr(_exp, Type.Boolean);
			_exp = new CastExpr(_exp, Type.Real);
			texp = _exp.typeCheck(stable);
		}

		// Numerical types will be converted to a position filter
		if (texp is NumberType)
		{
			// Cast any numerical types to an integer
			if (texp is IntType == false)
			{
			_exp = new CastExpr(_exp, Type.Int);
			}

				if (_canOptimize)
				{
					// Nth position optimization. Expression must not depend on context
					_nthPositionFilter = !_exp.hasLastCall() && !_exp.hasPositionCall();

					// _nthDescendant optimization - only if _nthPositionFilter is on
					if (_nthPositionFilter)
					{
						SyntaxTreeNode parent = Parent;
						_nthDescendant = (parent is Step) && (parent.Parent is AbsoluteLocationPath);
						return _type = Type.NodeSet;
					}
				}

			   // Reset optimization flags
				_nthPositionFilter = _nthDescendant = false;

			   // Otherwise, expand [e] to [position() = e]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName position = getParser().getQNameIgnoreDefaultNs("position");
			   QName position = Parser.getQNameIgnoreDefaultNs("position");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PositionCall positionCall = new PositionCall(position);
			   PositionCall positionCall = new PositionCall(position);
			   positionCall.Parser = Parser;
			   positionCall.Parent = this;

			   _exp = new EqualityExpr(Operators.EQ, positionCall, _exp);
			   if (_exp.typeCheck(stable) != Type.Boolean)
			   {
				   _exp = new CastExpr(_exp, Type.Boolean);
			   }
			   return _type = Type.Boolean;
		}
		else
		{
				// All other types will be handled as boolean values
			if (texp is BooleanType == false)
			{
			_exp = new CastExpr(_exp, Type.Boolean);
			}
				return _type = Type.Boolean;
		}
		}

		/// <summary>
		/// Create a new "Filter" class implementing
		/// <code>CurrentNodeListFilter</code>. Allocate registers for local 
		/// variables and local parameters passed in the closure to test().
		/// Notice that local variables need to be "unboxed".
		/// </summary>
		private void compileFilter(ClassGenerator classGen, MethodGenerator methodGen)
		{
		TestGenerator testGen;
		LocalVariableGen local;
		FilterGenerator filterGen;

		_className = XSLTC.HelperClassName;
		filterGen = new FilterGenerator(_className, "java.lang.Object", ToString(), Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_SUPER, new string[] {Constants_Fields.CURRENT_NODE_LIST_FILTER}, classGen.Stylesheet);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = filterGen.getConstantPool();
		ConstantPoolGen cpg = filterGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = (_closureVars == null) ? 0 : _closureVars.size();
		int length = (_closureVars == null) ? 0 : _closureVars.Count;

		// Add a new instance variable for each var in closure
		for (int i = 0; i < length; i++)
		{
			VariableBase @var = ((VariableRefBase) _closureVars[i]).Variable;

			filterGen.addField(new Field(Constants_Fields.ACC_PUBLIC, cpg.addUtf8(@var.EscapedName), cpg.addUtf8(@var.Type.toSignature()), null, cpg.ConstantPool));
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
		testGen = new TestGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] {org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT, Util.getJCRefType(Constants_Fields.TRANSLET_SIG), Util.getJCRefType(Constants_Fields.NODE_ITERATOR_SIG)}, new string[] {"node", "position", "last", "current", "translet", "iterator"}, "test", _className, il, cpg);

		// Store the dom in a local variable
		local = testGen.addLocalVariable("document", Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), null, null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getClassName();
		string className = classGen.ClassName;
		il.append(filterGen.loadTranslet());
		il.append(new CHECKCAST(cpg.addClass(className)));
		il.append(new GETFIELD(cpg.addFieldref(className, Constants_Fields.DOM_FIELD, Constants_Fields.DOM_INTF_SIG)));
		local.Start = il.append(new ASTORE(local.Index));

		// Store the dom index in the test generator
		testGen.DomIndex = local.Index;

		_exp.translate(filterGen, testGen);
		il.append(IRETURN);

		filterGen.addEmptyConstructor(Constants_Fields.ACC_PUBLIC);
		filterGen.addMethod(testGen);

		XSLTC.dumpClass(filterGen.JavaClass);
		}

		/// <summary>
		/// Returns true if the predicate is a test for the existance of an
		/// element or attribute. All we have to do is to get the first node
		/// from the step, check if it is there, and then return true/false.
		/// </summary>
		public bool BooleanTest
		{
			get
			{
			return (_exp is BooleanExpr);
			}
		}

		/// <summary>
		/// Method to see if we can optimise the predicate by using a specialised
		/// iterator for expressions like '/foo/bar[@attr = $var]', which are
		/// very common in many stylesheets
		/// </summary>
		public bool NodeValueTest
		{
			get
			{
			if (!_canOptimize)
			{
				return false;
			}
			return (Step != null && CompareValue != null);
			}
		}

	   /// <summary>
	   /// Returns the step in an expression of the form 'step = value'. 
	   /// Null is returned if the expression is not of the right form.
	   /// Optimization if off if null is returned.
	   /// </summary>
		public Step Step
		{
			get
			{
				// Returned cached value if called more than once
			if (_step != null)
			{
					return _step;
			}
    
				// Nothing to do if _exp is null
			if (_exp == null)
			{
					return null;
			}
    
				// Ignore if not equality expression
			if (_exp is EqualityExpr)
			{
				EqualityExpr exp = (EqualityExpr)_exp;
				Expression left = exp.Left;
				Expression right = exp.Right;
    
					// Unwrap and set _step if appropriate
				if (left is CastExpr)
				{
						left = ((CastExpr) left).Expr;
				}
				if (left is Step)
				{
						_step = (Step) left;
				}
    
					// Unwrap and set _step if appropriate
				if (right is CastExpr)
				{
						right = ((CastExpr)right).Expr;
				}
				if (right is Step)
				{
						_step = (Step)right;
				}
			}
			return _step;
			}
		}

		/// <summary>
		/// Returns the value in an expression of the form 'step = value'. 
		/// A value may be either a literal string or a variable whose 
		/// type is string. Optimization if off if null is returned.
		/// </summary>
		public Expression CompareValue
		{
			get
			{
				// Returned cached value if called more than once
			if (_value != null)
			{
					return _value;
			}
    
				// Nothing to to do if _exp is null
			if (_exp == null)
			{
					return null;
			}
    
				// Ignore if not an equality expression
			if (_exp is EqualityExpr)
			{
				EqualityExpr exp = (EqualityExpr) _exp;
				Expression left = exp.Left;
				Expression right = exp.Right;
    
					// Return if left is literal string
					if (left is LiteralExpr)
					{
						_value = left;
						return _value;
					}
					// Return if left is a variable reference of type string
					if (left is VariableRefBase && left.Type == Type.String)
					{
						_value = left;
						return _value;
					}
    
					// Return if right is literal string
					if (right is LiteralExpr)
					{
						_value = right;
						return _value;
					}
					// Return if left is a variable reference whose type is string
					if (right is VariableRefBase && right.Type == Type.String)
					{
						_value = right;
						return _value;
					}
			}
			return null;
			}
		}

		/// <summary>
		/// Translate a predicate expression. This translation pushes
		/// two references on the stack: a reference to a newly created
		/// filter object and a reference to the predicate's closure.
		/// </summary>
		public void translateFilter(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// Compile auxiliary class for filter
		compileFilter(classGen, methodGen);

		// Create new instance of filter
		il.append(new NEW(cpg.addClass(_className)));
		il.append(DUP);
		il.append(new INVOKESPECIAL(cpg.addMethodref(_className, "<init>", "()V")));

		// Initialize closure variables
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = (_closureVars == null) ? 0 : _closureVars.size();
		int length = (_closureVars == null) ? 0 : _closureVars.Count;

		for (int i = 0; i < length; i++)
		{
			VariableRefBase varRef = (VariableRefBase) _closureVars[i];
			VariableBase @var = varRef.Variable;
			Type varType = @var.Type;

			il.append(DUP);

			// Find nearest closure implemented as an inner class
			Closure variableClosure = _parentClosure;
			while (variableClosure != null)
			{
			if (variableClosure.inInnerClass())
			{
				break;
			}
			variableClosure = variableClosure.ParentClosure;
			}

			// Use getfield if in an inner class
			if (variableClosure != null)
			{
			il.append(ALOAD_0);
			il.append(new GETFIELD(cpg.addFieldref(variableClosure.InnerClassName, @var.EscapedName, varType.toSignature())));
			}
			else
			{
			// Use a load of instruction if in translet class
			il.append(@var.loadInstruction());
			}

			// Store variable in new closure
			il.append(new PUTFIELD(cpg.addFieldref(_className, @var.EscapedName, varType.toSignature())));
		}
		}

		/// <summary>
		/// Translate a predicate expression. If non of the optimizations apply
		/// then this translation pushes two references on the stack: a reference 
		/// to a newly created filter object and a reference to the predicate's 
		/// closure. See class <code>Step</code> for further details.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		if (_nthPositionFilter || _nthDescendant)
		{
			_exp.translate(classGen, methodGen);
		}
		else if (NodeValueTest && (Parent is Step))
		{
			_value.translate(classGen, methodGen);
			il.append(new CHECKCAST(cpg.addClass(Constants_Fields.STRING_CLASS)));
			il.append(new PUSH(cpg, ((EqualityExpr)_exp).Op));
		}
		else
		{
			translateFilter(classGen, methodGen);
		}
		}
	}

}