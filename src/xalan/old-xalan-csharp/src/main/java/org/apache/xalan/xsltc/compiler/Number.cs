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
 * $Id: Number.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Field = org.apache.bcel.classfile.Field;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFNONNULL = org.apache.bcel.generic.IFNONNULL;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MatchGenerator = org.apache.xalan.xsltc.compiler.util.MatchGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeCounterGenerator = org.apache.xalan.xsltc.compiler.util.NodeCounterGenerator;
	using RealType = org.apache.xalan.xsltc.compiler.util.RealType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal sealed class Number : Instruction, Closure
	{
		private const int LEVEL_SINGLE = 0;
		private const int LEVEL_MULTIPLE = 1;
		private const int LEVEL_ANY = 2;

		private static readonly string[] ClassNames = new string[] {"org.apache.xalan.xsltc.dom.SingleNodeCounter", "org.apache.xalan.xsltc.dom.MultipleNodeCounter", "org.apache.xalan.xsltc.dom.AnyNodeCounter"};

		private static readonly string[] FieldNames = new string[] {"___single_node_counter", "___multiple_node_counter", "___any_node_counter"};

		private Pattern _from = null;
		private Pattern _count = null;
		private Expression _value = null;

		private AttributeValueTemplate _lang = null;
		private AttributeValueTemplate _format = null;
		private AttributeValueTemplate _letterValue = null;
		private AttributeValueTemplate _groupingSeparator = null;
		private AttributeValueTemplate _groupingSize = null;

		private int _level = LEVEL_SINGLE;
		private bool _formatNeeded = false;

		private string _className = null;
		private ArrayList _closureVars = null;

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
			return null;
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
		}
		}

		// -- End Closure interface ----------------------

	   public override void parseContents(Parser parser)
	   {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _attributes.getLength();
		int count = _attributes.Length;

		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = _attributes.getQName(i);
			string name = _attributes.getQName(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = _attributes.getValue(i);
			string value = _attributes.getValue(i);

			if (name.Equals("value"))
			{
			_value = parser.parseExpression(this, name, null);
			}
			else if (name.Equals("count"))
			{
			_count = parser.parsePattern(this, name, null);
			}
			else if (name.Equals("from"))
			{
			_from = parser.parsePattern(this, name, null);
			}
			else if (name.Equals("level"))
			{
			if (value.Equals("single"))
			{
				_level = LEVEL_SINGLE;
			}
			else if (value.Equals("multiple"))
			{
				_level = LEVEL_MULTIPLE;
			}
			else if (value.Equals("any"))
			{
				_level = LEVEL_ANY;
			}
			}
			else if (name.Equals("format"))
			{
			_format = new AttributeValueTemplate(value, parser, this);
			_formatNeeded = true;
			}
			else if (name.Equals("lang"))
			{
			_lang = new AttributeValueTemplate(value, parser, this);
			_formatNeeded = true;
			}
			else if (name.Equals("letter-value"))
			{
			_letterValue = new AttributeValueTemplate(value, parser, this);
			_formatNeeded = true;
			}
			else if (name.Equals("grouping-separator"))
			{
			_groupingSeparator = new AttributeValueTemplate(value, parser, this);
			_formatNeeded = true;
			}
			else if (name.Equals("grouping-size"))
			{
			_groupingSize = new AttributeValueTemplate(value, parser, this);
			_formatNeeded = true;
			}
		}
	   }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_value != null)
		{
			Type tvalue = _value.typeCheck(stable);
			if (tvalue is RealType == false)
			{
			_value = new CastExpr(_value, Type.Real);
			}
		}
		if (_count != null)
		{
			_count.typeCheck(stable);
		}
		if (_from != null)
		{
			_from.typeCheck(stable);
		}
		if (_format != null)
		{
			_format.typeCheck(stable);
		}
		if (_lang != null)
		{
			_lang.typeCheck(stable);
		}
		if (_letterValue != null)
		{
			_letterValue.typeCheck(stable);
		}
		if (_groupingSeparator != null)
		{
			_groupingSeparator.typeCheck(stable);
		}
		if (_groupingSize != null)
		{
			_groupingSize.typeCheck(stable);
		}
		return Type.Void;
		}

		/// <summary>
		/// True if the has specified a value for this instance of number.
		/// </summary>
		public bool hasValue()
		{
		return _value != null;
		}

		/// <summary>
		/// Returns <tt>true</tt> if this instance of number has neither
		/// a from nor a count pattern.
		/// </summary>
		public bool Default
		{
			get
			{
			return _from == null && _count == null;
			}
		}

		private void compileDefault(ClassGenerator classGen, MethodGenerator methodGen)
		{
		int index;
		ConstantPoolGen cpg = classGen.ConstantPool;
		InstructionList il = methodGen.InstructionList;

		int[] fieldIndexes = XSLTC.NumberFieldIndexes;

		if (fieldIndexes[_level] == -1)
		{
			Field defaultNode = new Field(Constants_Fields.ACC_PRIVATE, cpg.addUtf8(FieldNames[_level]), cpg.addUtf8(Constants_Fields.NODE_COUNTER_SIG), null, cpg.ConstantPool);

			// Add a new private field to this class
			classGen.addField(defaultNode);

			// Get a reference to the newly added field
			fieldIndexes[_level] = cpg.addFieldref(classGen.ClassName, FieldNames[_level], Constants_Fields.NODE_COUNTER_SIG);
		}

		// Check if field is initialized (runtime)
		il.append(classGen.loadTranslet());
		il.append(new GETFIELD(fieldIndexes[_level]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifBlock1 = il.append(new org.apache.bcel.generic.IFNONNULL(null));
		BranchHandle ifBlock1 = il.append(new IFNONNULL(null));

		// Create an instance of DefaultNodeCounter
		index = cpg.addMethodref(ClassNames[_level], "getDefaultNodeCounter", "(" + Constants_Fields.TRANSLET_INTF_SIG + Constants_Fields.DOM_INTF_SIG + Constants_Fields.NODE_ITERATOR_SIG + ")" + Constants_Fields.NODE_COUNTER_SIG);
		il.append(classGen.loadTranslet());
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadIterator());
		il.append(new INVOKESTATIC(index));
		il.append(DUP);

		// Store the node counter in the field
		il.append(classGen.loadTranslet());
		il.append(SWAP);
		il.append(new PUTFIELD(fieldIndexes[_level]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifBlock2 = il.append(new org.apache.bcel.generic.GOTO(null));
		BranchHandle ifBlock2 = il.append(new GOTO(null));

		// Backpatch conditionals
		ifBlock1.Target = il.append(classGen.loadTranslet());
		il.append(new GETFIELD(fieldIndexes[_level]));

		ifBlock2.Target = il.append(NOP);
		}

		/// <summary>
		/// Compiles a constructor for the class <tt>_className</tt> that
		/// inherits from {Any,Single,Multiple}NodeCounter. This constructor
		/// simply calls the same constructor in the super class.
		/// </summary>
		private void compileConstructor(ClassGenerator classGen)
		{
		MethodGenerator cons;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;

		cons = new MethodGenerator(Constants_Fields.ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, new org.apache.bcel.generic.Type[] {Util.getJCRefType(Constants_Fields.TRANSLET_INTF_SIG), Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), Util.getJCRefType(Constants_Fields.NODE_ITERATOR_SIG)}, new string[] {"dom", "translet", "iterator"}, "<init>", _className, il, cpg);

		il.append(ALOAD_0); // this
		il.append(ALOAD_1); // translet
		il.append(ALOAD_2); // DOM
		il.append(new ALOAD(3)); // iterator

		int index = cpg.addMethodref(ClassNames[_level], "<init>", "(" + Constants_Fields.TRANSLET_INTF_SIG + Constants_Fields.DOM_INTF_SIG + Constants_Fields.NODE_ITERATOR_SIG + ")V");
		il.append(new INVOKESPECIAL(index));
		il.append(RETURN);

		classGen.addMethod(cons);
		}

		/// <summary>
		/// This method compiles code that is common to matchesFrom() and
		/// matchesCount() in the auxillary class.
		/// </summary>
		private void compileLocals(NodeCounterGenerator nodeCounterGen, MatchGenerator matchGen, InstructionList il)
		{
		int field;
		LocalVariableGen local;
		ConstantPoolGen cpg = nodeCounterGen.ConstantPool;

		// Get NodeCounter._iterator and store locally
		local = matchGen.addLocalVariable("iterator", Util.getJCRefType(Constants_Fields.NODE_ITERATOR_SIG), null, null);
		field = cpg.addFieldref(Constants_Fields.NODE_COUNTER, "_iterator", Constants_Fields.ITERATOR_FIELD_SIG);
		il.append(ALOAD_0); // 'this' pointer on stack
		il.append(new GETFIELD(field));
		local.Start = il.append(new ASTORE(local.Index));
		matchGen.IteratorIndex = local.Index;

		// Get NodeCounter._translet and store locally
		local = matchGen.addLocalVariable("translet", Util.getJCRefType(Constants_Fields.TRANSLET_SIG), null, null);
		field = cpg.addFieldref(Constants_Fields.NODE_COUNTER, "_translet", "Lorg/apache/xalan/xsltc/Translet;");
		il.append(ALOAD_0); // 'this' pointer on stack
		il.append(new GETFIELD(field));
		il.append(new CHECKCAST(cpg.addClass(Constants_Fields.TRANSLET_CLASS)));
		local.Start = il.append(new ASTORE(local.Index));
		nodeCounterGen.TransletIndex = local.Index;

		// Get NodeCounter._document and store locally
		local = matchGen.addLocalVariable("document", Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), null, null);
		field = cpg.addFieldref(_className, "_document", Constants_Fields.DOM_INTF_SIG);
		il.append(ALOAD_0); // 'this' pointer on stack
		il.append(new GETFIELD(field));
		// Make sure we have the correct DOM type on the stack!!!
		local.Start = il.append(new ASTORE(local.Index));
		matchGen.DomIndex = local.Index;
		}

		private void compilePatterns(ClassGenerator classGen, MethodGenerator methodGen)
		{
		int current;
		int field;
		LocalVariableGen local;
		MatchGenerator matchGen;
		NodeCounterGenerator nodeCounterGen;

		_className = XSLTC.HelperClassName;
		nodeCounterGen = new NodeCounterGenerator(_className, ClassNames[_level], ToString(), Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_SUPER, null, classGen.Stylesheet);
		InstructionList il = null;
		ConstantPoolGen cpg = nodeCounterGen.ConstantPool;

		// Add a new instance variable for each var in closure
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int closureLen = (_closureVars == null) ? 0 : _closureVars.size();
		int closureLen = (_closureVars == null) ? 0 : _closureVars.Count;

		for (int i = 0; i < closureLen; i++)
		{
			VariableBase @var = ((VariableRefBase) _closureVars[i]).Variable;

			nodeCounterGen.addField(new Field(Constants_Fields.ACC_PUBLIC, cpg.addUtf8(@var.EscapedName), cpg.addUtf8(@var.Type.toSignature()), null, cpg.ConstantPool));
		}

		// Add a single constructor to the class
		compileConstructor(nodeCounterGen);

		/*
		 * Compile method matchesFrom()
		 */
		if (_from != null)
		{
			il = new InstructionList();
			matchGen = new MatchGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] {org.apache.bcel.generic.Type.INT}, new string[] {"node"}, "matchesFrom", _className, il, cpg);

			compileLocals(nodeCounterGen,matchGen,il);

			// Translate Pattern
			il.append(matchGen.loadContextNode());
			_from.translate(nodeCounterGen, matchGen);
			_from.synthesize(nodeCounterGen, matchGen);
			il.append(IRETURN);

			nodeCounterGen.addMethod(matchGen);
		}

		/*
		 * Compile method matchesCount()
		 */
		if (_count != null)
		{
			il = new InstructionList();
			matchGen = new MatchGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] {org.apache.bcel.generic.Type.INT}, new string[] {"node"}, "matchesCount", _className, il, cpg);

			compileLocals(nodeCounterGen,matchGen,il);

			// Translate Pattern
			il.append(matchGen.loadContextNode());
			_count.translate(nodeCounterGen, matchGen);
			_count.synthesize(nodeCounterGen, matchGen);

			il.append(IRETURN);

			nodeCounterGen.addMethod(matchGen);
		}

		XSLTC.dumpClass(nodeCounterGen.JavaClass);

		// Push an instance of the newly created class
		cpg = classGen.ConstantPool;
		il = methodGen.InstructionList;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = cpg.addMethodref(_className, "<init>", "(" + Constants_Fields.TRANSLET_INTF_SIG + Constants_Fields.DOM_INTF_SIG + Constants_Fields.NODE_ITERATOR_SIG + ")V");
		int index = cpg.addMethodref(_className, "<init>", "(" + Constants_Fields.TRANSLET_INTF_SIG + Constants_Fields.DOM_INTF_SIG + Constants_Fields.NODE_ITERATOR_SIG + ")V");
		il.append(new NEW(cpg.addClass(_className)));
		il.append(DUP);
		il.append(classGen.loadTranslet());
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadIterator());
		il.append(new INVOKESPECIAL(index));

		// Initialize closure variables
		for (int i = 0; i < closureLen; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableRefBase varRef = (VariableRefBase) _closureVars.get(i);
			VariableRefBase varRef = (VariableRefBase) _closureVars[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = varRef.getVariable();
			VariableBase @var = varRef.Variable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type varType = var.getType();
			Type varType = @var.Type;

			// Store variable in new closure
			il.append(DUP);
			il.append(@var.loadInstruction());
			il.append(new PUTFIELD(cpg.addFieldref(_className, @var.EscapedName, varType.toSignature())));
		}
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		int index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;

		// Push "this" for the call to characters()
		il.append(classGen.loadTranslet());

		if (hasValue())
		{
			compileDefault(classGen, methodGen);
			_value.translate(classGen, methodGen);

			// Using java.lang.Math.floor(number + 0.5) to return a double value
				il.append(new PUSH(cpg, 0.5));
				il.append(DADD);
			index = cpg.addMethodref(Constants_Fields.MATH_CLASS, "floor", "(D)D");
			il.append(new INVOKESTATIC(index));

			// Call setValue on the node counter
			index = cpg.addMethodref(Constants_Fields.NODE_COUNTER, "setValue", "(D)" + Constants_Fields.NODE_COUNTER_SIG);
			il.append(new INVOKEVIRTUAL(index));
		}
		else if (Default)
		{
			compileDefault(classGen, methodGen);
		}
		else
		{
			compilePatterns(classGen, methodGen);
		}

		// Call setStartNode() 
		if (!hasValue())
		{
			il.append(methodGen.loadContextNode());
			index = cpg.addMethodref(Constants_Fields.NODE_COUNTER, Constants_Fields.SET_START_NODE, "(I)" + Constants_Fields.NODE_COUNTER_SIG);
			il.append(new INVOKEVIRTUAL(index));
		}

		// Call getCounter() with or without args
		if (_formatNeeded)
		{
			if (_format != null)
			{
			_format.translate(classGen, methodGen);
			}
			else
			{
			il.append(new PUSH(cpg, "1"));
			}

			if (_lang != null)
			{
			_lang.translate(classGen, methodGen);
			}
			else
			{
			il.append(new PUSH(cpg, "en")); // TODO ??
			}

			if (_letterValue != null)
			{
			_letterValue.translate(classGen, methodGen);
			}
			else
			{
			il.append(new PUSH(cpg, Constants_Fields.EMPTYSTRING));
			}

			if (_groupingSeparator != null)
			{
			_groupingSeparator.translate(classGen, methodGen);
			}
			else
			{
			il.append(new PUSH(cpg, Constants_Fields.EMPTYSTRING));
			}

			if (_groupingSize != null)
			{
			_groupingSize.translate(classGen, methodGen);
			}
			else
			{
			il.append(new PUSH(cpg, "0"));
			}

			index = cpg.addMethodref(Constants_Fields.NODE_COUNTER, "getCounter", "(" + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + Constants_Fields.STRING_SIG + ")" + Constants_Fields.STRING_SIG);
			il.append(new INVOKEVIRTUAL(index));
		}
		else
		{
			index = cpg.addMethodref(Constants_Fields.NODE_COUNTER, "setDefaultFormatting", "()" + Constants_Fields.NODE_COUNTER_SIG);
			il.append(new INVOKEVIRTUAL(index));

			index = cpg.addMethodref(Constants_Fields.NODE_COUNTER, "getCounter", "()" + Constants_Fields.STRING_SIG);
			il.append(new INVOKEVIRTUAL(index));
		}

		// Output the resulting string to the handler
		il.append(methodGen.loadHandler());
		index = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, Constants_Fields.CHARACTERSW, Constants_Fields.CHARACTERSW_SIG);
		il.append(new INVOKEVIRTUAL(index));
		}
	}

}