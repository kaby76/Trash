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
 * $Id: Sort.java 1348522 2012-06-10 01:55:18Z ggregory $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Field = org.apache.bcel.classfile.Field;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ANEWARRAY = org.apache.bcel.generic.ANEWARRAY;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using NOP = org.apache.bcel.generic.NOP;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using TABLESWITCH = org.apache.bcel.generic.TABLESWITCH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using CompareGenerator = org.apache.xalan.xsltc.compiler.util.CompareGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using IntType = org.apache.xalan.xsltc.compiler.util.IntType;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NodeSortRecordFactGenerator = org.apache.xalan.xsltc.compiler.util.NodeSortRecordFactGenerator;
	using NodeSortRecordGenerator = org.apache.xalan.xsltc.compiler.util.NodeSortRecordGenerator;
	using StringType = org.apache.xalan.xsltc.compiler.util.StringType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Axis = org.apache.xml.dtm.Axis;


	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Sort : Instruction, Closure
	{

		private Expression _select;
		private AttributeValue _order;
		private AttributeValue _caseOrder;
		private AttributeValue _dataType;
		private AttributeValue _lang; // bug! see 26869, see XALANJ-2546

		private string _data = null;

		private string _className = null;
		private ArrayList _closureVars = null;
		private bool _needsSortRecordFactory = false;

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
			set
			{
			_className = value;
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
			_needsSortRecordFactory = true;
		}
		}

		// -- End Closure interface ----------------------


		/// <summary>
		/// Parse the attributes of the xsl:sort element
		/// </summary>
		public override void parseContents(Parser parser)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode parent = getParent();
		SyntaxTreeNode parent = Parent;
		if (!(parent is ApplyTemplates) && !(parent is ForEach))
		{
			reportError(this, parser, ErrorMsg.STRAY_SORT_ERR, null);
			return;
		}

		// Parse the select expression (node string value if no expression)
		_select = parser.parseExpression(this, "select", "string(.)");

		// Get the sort order; default is 'ascending'
		string val = getAttribute("order");
		if (val.Length == 0)
		{
			val = "ascending";
		}
		_order = AttributeValue.create(this, val, parser);

		// Get the sort data type; default is text
		val = getAttribute("data-type");
		if (val.Length == 0)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type type = _select.typeCheck(parser.getSymbolTable());
			Type type = _select.typeCheck(parser.SymbolTable);
			if (type is IntType)
			{
				val = "number";
			}
			else
			{
				val = "text";
			}
			}
			catch (TypeCheckError)
			{
			val = "text";
			}
		}
		_dataType = AttributeValue.create(this, val, parser);

		val = getAttribute("lang");
		_lang = AttributeValue.create(this, val, parser);
		// Get the case order; default is language dependant
		val = getAttribute("case-order");
		_caseOrder = AttributeValue.create(this, val, parser);

		}

		/// <summary>
		/// Run type checks on the attributes; expression must return a string
		/// which we will use as a sort key
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tselect = _select.typeCheck(stable);
		Type tselect = _select.typeCheck(stable);

		// If the sort data-type is not set we use the natural data-type
		// of the data we will sort
		if (!(tselect is StringType))
		{
			_select = new CastExpr(_select, Type.String);
		}

		_order.typeCheck(stable);
		_caseOrder.typeCheck(stable);
		_dataType.typeCheck(stable);
		_lang.typeCheck(stable);
		return Type.Void;
		}

		/// <summary>
		/// These two methods are needed in the static methods that compile the
		/// overloaded NodeSortRecord.compareType() and NodeSortRecord.sortOrder()
		/// </summary>
		public void translateSortType(ClassGenerator classGen, MethodGenerator methodGen)
		{
		_dataType.translate(classGen, methodGen);
		}

		public void translateSortOrder(ClassGenerator classGen, MethodGenerator methodGen)
		{
		_order.translate(classGen, methodGen);
		}

		 public void translateCaseOrder(ClassGenerator classGen, MethodGenerator methodGen)
		 {
		_caseOrder.translate(classGen, methodGen);
		 }

		public void translateLang(ClassGenerator classGen, MethodGenerator methodGen)
		{
		_lang.translate(classGen, methodGen);
		}

		/// <summary>
		/// This method compiles code for the select expression for this
		/// xsl:sort element. The method is called from the static code-generating
		/// methods in this class.
		/// </summary>
		public void translateSelect(ClassGenerator classGen, MethodGenerator methodGen)
		{
		_select.translate(classGen,methodGen);
		}

		/// <summary>
		/// This method should not produce any code
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		// empty
		}

		/// <summary>
		/// Compiles code that instantiates a SortingIterator object.
		/// This object's constructor needs referencdes to the current iterator
		/// and a node sort record producing objects as its parameters.
		/// </summary>
		public static void translateSortIterator(ClassGenerator classGen, MethodGenerator methodGen, Expression nodeSet, ArrayList sortObjects)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// SortingIterator.SortingIterator(NodeIterator,NodeSortRecordFactory);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int init = cpg.addMethodref(SORT_ITERATOR, "<init>", "(" + NODE_ITERATOR_SIG + NODE_SORT_FACTORY_SIG + ")V");
		int init = cpg.addMethodref(SORT_ITERATOR, "<init>", "(" + NODE_ITERATOR_SIG + NODE_SORT_FACTORY_SIG + ")V");

			// Backwards branches are prohibited if an uninitialized object is
			// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
			// We don't know whether this code might contain backwards branches
			// so we mustn't create the new object until after we've created
			// the suspect arguments to its constructor.  Instead we calculate
			// the values of the arguments to the constructor first, store them
			// in temporary variables, create the object and reload the
			// arguments from the temporaries to avoid the problem.

			LocalVariableGen nodesTemp = methodGen.addLocalVariable("sort_tmp1", Util.getJCRefType(NODE_ITERATOR_SIG), null, null);

			LocalVariableGen sortRecordFactoryTemp = methodGen.addLocalVariable("sort_tmp2", Util.getJCRefType(NODE_SORT_FACTORY_SIG), null, null);

		// Get the current node iterator
		if (nodeSet == null)
		{ // apply-templates default
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int children = cpg.addInterfaceMethodref(DOM_INTF, "getAxisIterator", "(I)"+ NODE_ITERATOR_SIG);
			int children = cpg.addInterfaceMethodref(DOM_INTF, "getAxisIterator", "(I)" + NODE_ITERATOR_SIG);
			il.append(methodGen.loadDOM());
			il.append(new PUSH(cpg, Axis.CHILD));
			il.append(new INVOKEINTERFACE(children, 2));
		}
		else
		{
			nodeSet.translate(classGen, methodGen);
		}

			nodesTemp.setStart(il.append(new ASTORE(nodesTemp.getIndex())));

		// Compile the code for the NodeSortRecord producing class and pass
		// that as the last argument to the SortingIterator constructor.
		compileSortRecordFactory(sortObjects, classGen, methodGen);
			sortRecordFactoryTemp.setStart(il.append(new ASTORE(sortRecordFactoryTemp.getIndex())));

		il.append(new NEW(cpg.addClass(SORT_ITERATOR)));
		il.append(DUP);
			nodesTemp.setEnd(il.append(new ALOAD(nodesTemp.getIndex())));
			sortRecordFactoryTemp.setEnd(il.append(new ALOAD(sortRecordFactoryTemp.getIndex())));
		il.append(new INVOKESPECIAL(init));
		}


		/// <summary>
		/// Compiles code that instantiates a NodeSortRecordFactory object which
		/// will produce NodeSortRecord objects of a specific type.
		/// </summary>
		public static void compileSortRecordFactory(ArrayList sortObjects, ClassGenerator classGen, MethodGenerator methodGen)
		{
		string sortRecordClass = compileSortRecord(sortObjects, classGen, methodGen);

		bool needsSortRecordFactory = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nsorts = sortObjects.size();
		int nsorts = sortObjects.Count;
		for (int i = 0; i < nsorts; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort) sortObjects.elementAt(i);
			Sort sort = (Sort) sortObjects[i];
			needsSortRecordFactory |= sort._needsSortRecordFactory;
		}

		string sortRecordFactoryClass = NODE_SORT_FACTORY;
		if (needsSortRecordFactory)
		{
			sortRecordFactoryClass = compileSortRecordFactory(sortObjects, classGen, methodGen, sortRecordClass);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

			// Backwards branches are prohibited if an uninitialized object is
			// on the stack by section 4.9.4 of the JVM Specification, 2nd Ed.
			// We don't know whether this code might contain backwards branches
			// so we mustn't create the new object until after we've created
			// the suspect arguments to its constructor.  Instead we calculate
			// the values of the arguments to the constructor first, store them
			// in temporary variables, create the object and reload the
			// arguments from the temporaries to avoid the problem.

		// Compile code that initializes the static _sortOrder
			LocalVariableGen sortOrderTemp = methodGen.addLocalVariable("sort_order_tmp", Util.getJCRefType("[" + STRING_SIG), null, null);
		il.append(new PUSH(cpg, nsorts));
		il.append(new ANEWARRAY(cpg.addClass(STRING)));
		for (int level = 0; level < nsorts; level++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort)sortObjects.elementAt(level);
			Sort sort = (Sort)sortObjects[level];
			il.append(DUP);
			il.append(new PUSH(cpg, level));
			sort.translateSortOrder(classGen, methodGen);
			il.append(AASTORE);
		}
			sortOrderTemp.setStart(il.append(new ASTORE(sortOrderTemp.getIndex())));

			LocalVariableGen sortTypeTemp = methodGen.addLocalVariable("sort_type_tmp", Util.getJCRefType("[" + STRING_SIG), null, null);
		il.append(new PUSH(cpg, nsorts));
		il.append(new ANEWARRAY(cpg.addClass(STRING)));
		for (int level = 0; level < nsorts; level++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort)sortObjects.elementAt(level);
			Sort sort = (Sort)sortObjects[level];
			il.append(DUP);
			il.append(new PUSH(cpg, level));
			sort.translateSortType(classGen, methodGen);
			il.append(AASTORE);
		}
			sortTypeTemp.setStart(il.append(new ASTORE(sortTypeTemp.getIndex())));

			LocalVariableGen sortLangTemp = methodGen.addLocalVariable("sort_lang_tmp", Util.getJCRefType("[" + STRING_SIG), null, null);
			il.append(new PUSH(cpg, nsorts));
			il.append(new ANEWARRAY(cpg.addClass(STRING)));
			for (int level = 0; level < nsorts; level++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort)sortObjects.elementAt(level);
				  Sort sort = (Sort)sortObjects[level];
				  il.append(DUP);
				  il.append(new PUSH(cpg, level));
				  sort.translateLang(classGen, methodGen);
				  il.append(AASTORE);
			}
			sortLangTemp.setStart(il.append(new ASTORE(sortLangTemp.getIndex())));

			LocalVariableGen sortCaseOrderTemp = methodGen.addLocalVariable("sort_case_order_tmp", Util.getJCRefType("[" + STRING_SIG), null, null);
			il.append(new PUSH(cpg, nsorts));
			il.append(new ANEWARRAY(cpg.addClass(STRING)));
			for (int level = 0; level < nsorts; level++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort)sortObjects.elementAt(level);
				Sort sort = (Sort)sortObjects[level];
				il.append(DUP);
				il.append(new PUSH(cpg, level));
				sort.translateCaseOrder(classGen, methodGen);
				il.append(AASTORE);
			}
			sortCaseOrderTemp.setStart(il.append(new ASTORE(sortCaseOrderTemp.getIndex())));

		il.append(new NEW(cpg.addClass(sortRecordFactoryClass)));
		il.append(DUP);
		il.append(methodGen.loadDOM());
		il.append(new PUSH(cpg, sortRecordClass));
		il.append(classGen.loadTranslet());

			sortOrderTemp.setEnd(il.append(new ALOAD(sortOrderTemp.getIndex())));
			sortTypeTemp.setEnd(il.append(new ALOAD(sortTypeTemp.getIndex())));
			sortLangTemp.setEnd(il.append(new ALOAD(sortLangTemp.getIndex())));
			sortCaseOrderTemp.setEnd(il.append(new ALOAD(sortCaseOrderTemp.getIndex())));

		il.append(new INVOKESPECIAL(cpg.addMethodref(sortRecordFactoryClass, "<init>", "(" + DOM_INTF_SIG + STRING_SIG + TRANSLET_INTF_SIG + "[" + STRING_SIG + "[" + STRING_SIG + "[" + STRING_SIG + "[" + STRING_SIG + ")V")));

		// Initialize closure variables in sortRecordFactory
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList dups = new java.util.ArrayList();
		ArrayList dups = new ArrayList();

		for (int j = 0; j < nsorts; j++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort) sortObjects.get(j);
			Sort sort = (Sort) sortObjects[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = (sort._closureVars == null) ? 0 : sort._closureVars.size();
			int length = (sort._closureVars == null) ? 0 : sort._closureVars.Count;

			for (int i = 0; i < length; i++)
			{
			VariableRefBase varRef = (VariableRefBase) sort._closureVars[i];

			// Discard duplicate variable references
			if (dups.Contains(varRef))
			{
				continue;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = varRef.getVariable();
			VariableBase var = varRef.Variable;

			// Store variable in new closure
			il.append(DUP);
			il.append(var.loadInstruction());
			il.append(new PUTFIELD(cpg.addFieldref(sortRecordFactoryClass, var.EscapedName, var.Type.toSignature())));
			dups.Add(varRef);
			}
		}
		}

		public static string compileSortRecordFactory(ArrayList sortObjects, ClassGenerator classGen, MethodGenerator methodGen, string sortRecordClass)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = ((Sort)sortObjects.firstElement()).getXSLTC();
		XSLTC xsltc = ((Sort)sortObjects[0]).XSLTC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = xsltc.getHelperClassName();
		string className = xsltc.HelperClassName;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.NodeSortRecordFactGenerator sortRecordFactory = new org.apache.xalan.xsltc.compiler.util.NodeSortRecordFactGenerator(className, NODE_SORT_FACTORY, className + ".java", ACC_PUBLIC | ACC_SUPER | ACC_FINAL, new String[] {}, classGen.getStylesheet());
		NodeSortRecordFactGenerator sortRecordFactory = new NodeSortRecordFactGenerator(className, NODE_SORT_FACTORY, className + ".java", ACC_PUBLIC | ACC_SUPER | ACC_FINAL, new string[] {}, classGen.Stylesheet);

		ConstantPoolGen cpg = sortRecordFactory.getConstantPool();

		// Add a new instance variable for each var in closure
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nsorts = sortObjects.size();
		int nsorts = sortObjects.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList dups = new java.util.ArrayList();
		ArrayList dups = new ArrayList();

		for (int j = 0; j < nsorts; j++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort) sortObjects.get(j);
			Sort sort = (Sort) sortObjects[j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = (sort._closureVars == null) ? 0 : sort._closureVars.size();
			int length = (sort._closureVars == null) ? 0 : sort._closureVars.Count;

			for (int i = 0; i < length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableRefBase varRef = (VariableRefBase) sort._closureVars.get(i);
			VariableRefBase varRef = (VariableRefBase) sort._closureVars[i];

			// Discard duplicate variable references
			if (dups.Contains(varRef))
			{
				continue;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = varRef.getVariable();
			VariableBase var = varRef.Variable;
			sortRecordFactory.addField(new Field(ACC_PUBLIC, cpg.addUtf8(var.EscapedName), cpg.addUtf8(var.Type.toSignature()), null, cpg.getConstantPool()));
			dups.Add(varRef);
			}
		}

		// Define a constructor for this class
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[7];
		org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[7];
		argTypes[0] = Util.getJCRefType(DOM_INTF_SIG);
		argTypes[1] = Util.getJCRefType(STRING_SIG);
		argTypes[2] = Util.getJCRefType(TRANSLET_INTF_SIG);
		argTypes[3] = Util.getJCRefType("[" + STRING_SIG);
		argTypes[4] = Util.getJCRefType("[" + STRING_SIG);
	  argTypes[5] = Util.getJCRefType("[" + STRING_SIG);
	  argTypes[6] = Util.getJCRefType("[" + STRING_SIG);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argNames = new String[7];
		string[] argNames = new string[7];
		argNames[0] = DOCUMENT_PNAME;
		argNames[1] = "className";
		argNames[2] = TRANSLET_PNAME;
		argNames[3] = "order";
		argNames[4] = "type";
	  argNames[5] = "lang";
	  argNames[6] = "case_order";


		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator constructor = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "<init>", className, il, cpg);
		MethodGenerator constructor = new MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "<init>", className, il, cpg);

		// Push all parameters onto the stack and called super.<init>()
		il.append(ALOAD_0);
		il.append(ALOAD_1);
		il.append(ALOAD_2);
		il.append(new ALOAD(3));
		il.append(new ALOAD(4));
		il.append(new ALOAD(5));
	  il.append(new ALOAD(6));
	  il.append(new ALOAD(7));
		il.append(new INVOKESPECIAL(cpg.addMethodref(NODE_SORT_FACTORY, "<init>", "(" + DOM_INTF_SIG + STRING_SIG + TRANSLET_INTF_SIG + "[" + STRING_SIG + "[" + STRING_SIG + "[" + STRING_SIG + "[" + STRING_SIG + ")V")));
		il.append(RETURN);

		// Override the definition of makeNodeSortRecord()
		il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator makeNodeSortRecord = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(NODE_SORT_RECORD_SIG), new org.apache.bcel.generic.Type[] { org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT }, new String[] { "node", "last" }, "makeNodeSortRecord", className, il, cpg);
		MethodGenerator makeNodeSortRecord = new MethodGenerator(ACC_PUBLIC, Util.getJCRefType(NODE_SORT_RECORD_SIG), new org.apache.bcel.generic.Type[] {org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT}, new string[] {"node", "last"}, "makeNodeSortRecord", className, il, cpg);

		il.append(ALOAD_0);
		il.append(ILOAD_1);
		il.append(ILOAD_2);
		il.append(new INVOKESPECIAL(cpg.addMethodref(NODE_SORT_FACTORY, "makeNodeSortRecord", "(II)" + NODE_SORT_RECORD_SIG)));
		il.append(DUP);
		il.append(new CHECKCAST(cpg.addClass(sortRecordClass)));

		// Initialize closure in record class
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ndups = dups.size();
		int ndups = dups.Count;
		for (int i = 0; i < ndups; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableRefBase varRef = (VariableRefBase) dups.get(i);
			VariableRefBase varRef = (VariableRefBase) dups[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = varRef.getVariable();
			VariableBase var = varRef.Variable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type varType = var.getType();
			Type varType = var.Type;

			il.append(DUP);

			// Get field from factory class
			il.append(ALOAD_0);
			il.append(new GETFIELD(cpg.addFieldref(className, var.EscapedName, varType.toSignature())));

			// Put field in record class
			il.append(new PUTFIELD(cpg.addFieldref(sortRecordClass, var.EscapedName, varType.toSignature())));
		}
		il.append(POP);
		il.append(ARETURN);

		constructor.setMaxLocals();
		constructor.setMaxStack();
		sortRecordFactory.addMethod(constructor);
		makeNodeSortRecord.setMaxLocals();
		makeNodeSortRecord.setMaxStack();
		sortRecordFactory.addMethod(makeNodeSortRecord);
		xsltc.dumpClass(sortRecordFactory.getJavaClass());

		return className;
		}

		/// <summary>
		/// Create a new auxillary class extending NodeSortRecord.
		/// </summary>
		private static string compileSortRecord(ArrayList sortObjects, ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = ((Sort)sortObjects.firstElement()).getXSLTC();
		XSLTC xsltc = ((Sort)sortObjects[0]).XSLTC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = xsltc.getHelperClassName();
		string className = xsltc.HelperClassName;

		// This generates a new class for handling this specific sort
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.NodeSortRecordGenerator sortRecord = new org.apache.xalan.xsltc.compiler.util.NodeSortRecordGenerator(className, NODE_SORT_RECORD, "sort$0.java", ACC_PUBLIC | ACC_SUPER | ACC_FINAL, new String[] {}, classGen.getStylesheet());
		NodeSortRecordGenerator sortRecord = new NodeSortRecordGenerator(className, NODE_SORT_RECORD, "sort$0.java", ACC_PUBLIC | ACC_SUPER | ACC_FINAL, new string[] {}, classGen.Stylesheet);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = sortRecord.getConstantPool();
		ConstantPoolGen cpg = sortRecord.getConstantPool();

		// Add a new instance variable for each var in closure
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nsorts = sortObjects.size();
		int nsorts = sortObjects.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList dups = new java.util.ArrayList();
		ArrayList dups = new ArrayList();

		for (int j = 0; j < nsorts; j++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort) sortObjects.get(j);
			Sort sort = (Sort) sortObjects[j];

			// Set the name of the inner class in this sort object
			sort.InnerClassName = className;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = (sort._closureVars == null) ? 0 : sort._closureVars.size();
			int length = (sort._closureVars == null) ? 0 : sort._closureVars.Count;
			for (int i = 0; i < length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableRefBase varRef = (VariableRefBase) sort._closureVars.get(i);
			VariableRefBase varRef = (VariableRefBase) sort._closureVars[i];

			// Discard duplicate variable references
			if (dups.Contains(varRef))
			{
				continue;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = varRef.getVariable();
			VariableBase var = varRef.Variable;
			sortRecord.addField(new Field(ACC_PUBLIC, cpg.addUtf8(var.EscapedName), cpg.addUtf8(var.Type.toSignature()), null, cpg.getConstantPool()));
			dups.Add(varRef);
			}
		}

		MethodGenerator init = compileInit(sortObjects, sortRecord, cpg, className);
		MethodGenerator extract = compileExtract(sortObjects, sortRecord, cpg, className);
		sortRecord.addMethod(init);
		sortRecord.addMethod(extract);

		xsltc.dumpClass(sortRecord.getJavaClass());
		return className;
		}

		/// <summary>
		/// Create a constructor for the new class. Updates the reference to the 
		/// collator in the super calls only when the stylesheet specifies a new
		/// language in xsl:sort.
		/// </summary>
		private static MethodGenerator compileInit(ArrayList sortObjects, NodeSortRecordGenerator sortRecord, ConstantPoolGen cpg, string className)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator init = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, null, null, "<init>", className, il, cpg);
		MethodGenerator init = new MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, null, null, "<init>", className, il, cpg);

		// Call the constructor in the NodeSortRecord superclass
		il.append(ALOAD_0);
		il.append(new INVOKESPECIAL(cpg.addMethodref(NODE_SORT_RECORD, "<init>", "()V")));



		il.append(RETURN);

		return init;
		}


		/// <summary>
		/// Compiles a method that overloads NodeSortRecord.extractValueFromDOM()
		/// </summary>
		private static MethodGenerator compileExtract(ArrayList sortObjects, NodeSortRecordGenerator sortRecord, ConstantPoolGen cpg, string className)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();

		// String NodeSortRecord.extractValueFromDOM(dom,node,level);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.CompareGenerator extractMethod = new org.apache.xalan.xsltc.compiler.util.CompareGenerator(ACC_PUBLIC | ACC_FINAL, org.apache.bcel.generic.Type.STRING, new org.apache.bcel.generic.Type[] { org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(DOM_INTF_SIG), org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT, org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(TRANSLET_SIG), org.apache.bcel.generic.Type.INT }, new String[] { "dom", "current", "level", "translet", "last" }, "extractValueFromDOM", className, il, cpg);
		CompareGenerator extractMethod = new CompareGenerator(ACC_PUBLIC | ACC_FINAL, org.apache.bcel.generic.Type.STRING, new org.apache.bcel.generic.Type[] {Util.getJCRefType(DOM_INTF_SIG), org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT, Util.getJCRefType(TRANSLET_SIG), org.apache.bcel.generic.Type.INT}, new string[] {"dom", "current", "level", "translet", "last"}, "extractValueFromDOM", className, il, cpg);

		// Values needed for the switch statement
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int levels = sortObjects.size();
		int levels = sortObjects.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int match[] = new int[levels];
		int[] match = new int[levels];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle target[] = new org.apache.bcel.generic.InstructionHandle[levels];
		InstructionHandle[] target = new InstructionHandle[levels];
		InstructionHandle tblswitch = null;

		// Compile switch statement only if the key has multiple levels
		if (levels > 1)
		{
			// Put the parameter to the swtich statement on the stack
			il.append(new ILOAD(extractMethod.getLocalIndex("level")));
			// Append the switch statement here later on
			tblswitch = il.append(new NOP());
		}

		// Append all the cases for the switch statment
		for (int level = 0; level < levels; level++)
		{
			match[level] = level;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Sort sort = (Sort)sortObjects.elementAt(level);
			Sort sort = (Sort)sortObjects[level];
			target[level] = il.append(NOP);
			sort.translateSelect(sortRecord, extractMethod);
			il.append(ARETURN);
		}

		// Compile def. target for switch statement if key has multiple levels
		if (levels > 1)
		{
			// Append the default target - it will _NEVER_ be reached
			InstructionHandle defaultTarget = il.append(new PUSH(cpg, EMPTYSTRING));
			il.insert(tblswitch,new TABLESWITCH(match, target, defaultTarget));
			il.append(ARETURN);
		}

		return extractMethod;
		}
	}

}