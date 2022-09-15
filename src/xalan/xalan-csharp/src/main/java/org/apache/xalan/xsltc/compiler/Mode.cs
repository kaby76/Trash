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
 * $Id: Mode.java 1225431 2011-12-29 04:56:50Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using Instruction = org.apache.bcel.generic.Instruction;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using DUP = org.apache.bcel.generic.DUP;
	using GOTO_W = org.apache.bcel.generic.GOTO_W;
	using IFLT = org.apache.bcel.generic.IFLT;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using SWITCH = org.apache.bcel.generic.SWITCH;
	using TargetLostException = org.apache.bcel.generic.TargetLostException;
	using InstructionFinder = org.apache.bcel.util.InstructionFinder;
	using DOM = org.apache.xalan.xsltc.DOM;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NamedMethodGenerator = org.apache.xalan.xsltc.compiler.util.NamedMethodGenerator;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// Mode gathers all the templates belonging to a given mode; 
	/// it is responsible for generating an appropriate 
	/// applyTemplates + (mode name) method in the translet.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author G. Todd Miller
	/// </summary>
	internal sealed class Mode : Constants
	{

		/// <summary>
		/// The name of this mode as defined in the stylesheet.
		/// </summary>
		private readonly QName _name;

		/// <summary>
		/// A reference to the stylesheet object that owns this mode.
		/// </summary>
		private readonly Stylesheet _stylesheet;

		/// <summary>
		/// The name of the method in which this mode is compiled.
		/// </summary>
		private readonly string _methodName;

		/// <summary>
		/// A vector of all the templates in this mode.
		/// </summary>
		private ArrayList _templates;

		/// <summary>
		/// Group for patterns with node()-type kernel and child axis.
		/// </summary>
		private ArrayList _childNodeGroup = null;

		/// <summary>
		/// Test sequence for patterns with node()-type kernel and child axis.
		/// </summary>
		private TestSeq _childNodeTestSeq = null;

		/// <summary>
		/// Group for patterns with node()-type kernel and attribute axis.
		/// </summary>
		private ArrayList _attribNodeGroup = null;

		/// <summary>
		/// Test sequence for patterns with node()-type kernel and attribute axis.
		/// </summary>
		private TestSeq _attribNodeTestSeq = null;

		/// <summary>
		/// Group for patterns with id() or key()-type kernel.
		/// </summary>
		private ArrayList _idxGroup = null;

		/// <summary>
		/// Test sequence for patterns with id() or key()-type kernel.
		/// </summary>
		private TestSeq _idxTestSeq = null;

		/// <summary>
		/// Group for patterns with any other kernel type.
		/// </summary>
		private ArrayList[] _patternGroups;

		/// <summary>
		/// Test sequence for patterns with any other kernel type.
		/// </summary>
		private TestSeq[] _testSeq;


		/// <summary>
		/// A mapping between templates and test sequences.
		/// </summary>
		private Hashtable _neededTemplates = new Hashtable();

		/// <summary>
		/// A mapping between named templates and Mode objects.
		/// </summary>
		private Hashtable _namedTemplates = new Hashtable();

		/// <summary>
		/// A mapping between templates and instruction handles.
		/// </summary>
		private Hashtable _templateIHs = new Hashtable();

		/// <summary>
		/// A mapping between templates and instruction lists.
		/// </summary>
		private Hashtable _templateILs = new Hashtable();

		/// <summary>
		/// A reference to the pattern matching the root node.
		/// </summary>
		private LocationPathPattern _rootPattern = null;

		/// <summary>
		/// Stores ranges of template precendences for the compilation 
		/// of apply-imports (a Hashtable for historical reasons).
		/// </summary>
		private Hashtable _importLevels = null;

		/// <summary>
		/// A mapping between key names and keys.
		/// </summary>
		private Hashtable _keys = null;

		/// <summary>
		/// Variable index for the current node used in code generation.
		/// </summary>
		private int _currentIndex;

		/// <summary>
		/// Creates a new Mode.
		/// </summary>
		/// <param name="name"> A textual representation of the mode's QName </param>
		/// <param name="stylesheet"> The Stylesheet in which the mode occured </param>
		/// <param name="suffix"> A suffix to append to the method name for this mode
		///               (normally a sequence number - still in a String). </param>
		public Mode(QName name, Stylesheet stylesheet, string suffix)
		{
		_name = name;
		_stylesheet = stylesheet;
		_methodName = APPLY_TEMPLATES + suffix;
		_templates = new ArrayList();
		_patternGroups = new ArrayList[32];
		}

		/// <summary>
		/// Returns the name of the method (_not_ function) that will be 
		/// compiled for this mode. Normally takes the form 'applyTemplates()' 
		/// or * 'applyTemplates2()'.
		/// </summary>
		/// <returns> Method name for this mode </returns>
		public string functionName()
		{
		return _methodName;
		}

		public string functionName(int min, int max)
		{
		if (_importLevels == null)
		{
			_importLevels = new Hashtable();
		}
		_importLevels[new int?(max)] = new int?(min);
		return _methodName + '_' + max;
		}

		/// <summary>
		/// Shortcut to get the class compiled for this mode (will be inlined).
		/// </summary>
		private string ClassName
		{
			get
			{
			return _stylesheet.ClassName;
			}
		}

		public Stylesheet Stylesheet
		{
			get
			{
			return _stylesheet;
			}
		}

		public void addTemplate(Template template)
		{
		_templates.Add(template);
		}

		private ArrayList quicksort(ArrayList templates, int p, int r)
		{
		if (p < r)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int q = partition(templates, p, r);
			int q = partition(templates, p, r);
			quicksort(templates, p, q);
			quicksort(templates, q + 1, r);
		}
		return templates;
		}

		private int partition(ArrayList templates, int p, int r)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template x = (Template)templates.elementAt(p);
		Template x = (Template)templates[p];
		int i = p - 1;
		int j = r + 1;
		while (true)
		{
			while (x.compareTo((Template)templates[--j]) > 0)
			{
					;
			}
			while (x.compareTo((Template)templates[++i]) < 0)
			{
					;
			}
			if (i < j)
			{
			templates[j] = templates[i] = templates[j];
			}
			else
			{
			return j;
			}
		}
		}

		/// <summary>
		/// Process all the test patterns in this mode
		/// </summary>
		public void processPatterns(Hashtable keys)
		{
		_keys = keys;

	/*
	System.out.println("Before Sort " + _name);
	for (int i = 0; i < _templates.size(); i++) {
	    System.out.println("name = " + ((Template)_templates.elementAt(i)).getName());
	    System.out.println("pattern = " + ((Template)_templates.elementAt(i)).getPattern());
	    System.out.println("priority = " + ((Template)_templates.elementAt(i)).getPriority());
	    System.out.println("position = " + ((Template)_templates.elementAt(i)).getPosition());
	}
	*/

		_templates = quicksort(_templates, 0, _templates.Count - 1);

	/*
	System.out.println("\n After Sort " + _name);
	for (int i = 0; i < _templates.size(); i++) {
	    System.out.println("name = " + ((Template)_templates.elementAt(i)).getName());
	    System.out.println("pattern = " + ((Template)_templates.elementAt(i)).getPattern());
	    System.out.println("priority = " + ((Template)_templates.elementAt(i)).getPriority());
	    System.out.println("position = " + ((Template)_templates.elementAt(i)).getPosition());
	}
	*/

		// Traverse all templates
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration templates = _templates.elements();
		System.Collections.IEnumerator templates = _templates.GetEnumerator();
		while (templates.MoveNext())
		{
			// Get the next template
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)templates.Current;
			Template template = (Template)templates.Current;

			/* 
			 * Add this template to a table of named templates if it has a name.
			 * If there are multiple templates with the same name, all but one
			 * (the one with highest priority) will be disabled.
			 */
			if (template.Named && !template.disabled())
			{
			_namedTemplates[template] = this;
			}

			// Add this template to a test sequence if it has a pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Pattern pattern = template.getPattern();
			Pattern pattern = template.Pattern;
			if (pattern != null)
			{
			flattenAlternative(pattern, template, keys);
			}
		}
		prepareTestSequences();
		}

		/// <summary>
		/// This method will break up alternative patterns (ie. unions of patterns,
		/// such as match="A/B | C/B") and add the basic patterns to their
		/// respective pattern groups.
		/// </summary>
		private void flattenAlternative(Pattern pattern, Template template, Hashtable keys)
		{
		// Patterns on type id() and key() are special since they do not have
		// any kernel node type (it can be anything as long as the node is in
		// the id's or key's index).
		if (pattern is IdKeyPattern)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final IdKeyPattern idkey = (IdKeyPattern)pattern;
			IdKeyPattern idkey = (IdKeyPattern)pattern;
			idkey.Template = template;
			if (_idxGroup == null)
			{
				_idxGroup = new ArrayList();
			}
			_idxGroup.Add(pattern);
		}
		// Alternative patterns are broken up and re-processed recursively
		else if (pattern is AlternativePattern)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AlternativePattern alt = (AlternativePattern)pattern;
			AlternativePattern alt = (AlternativePattern)pattern;
			flattenAlternative(alt.Left, template, keys);
			flattenAlternative(alt.Right, template, keys);
		}
		// Finally we have a pattern that can be added to a test sequence!
		else if (pattern is LocationPathPattern)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocationPathPattern lpp = (LocationPathPattern)pattern;
			LocationPathPattern lpp = (LocationPathPattern)pattern;
			lpp.Template = template;
			addPatternToGroup(lpp);
		}
		}

		/// <summary>
		/// Group patterns by NodeTests of their last Step
		/// Keep them sorted by priority within group
		/// </summary>
		private void addPatternToGroup(in LocationPathPattern lpp)
		{
		// id() and key()-type patterns do not have a kernel type
		if (lpp is IdKeyPattern)
		{
			addPattern(-1, lpp);
		}
		// Otherwise get the kernel pattern from the LPP
		else
		{
			// kernel pattern is the last (maybe only) Step
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StepPattern kernel = lpp.getKernelPattern();
			StepPattern kernel = lpp.KernelPattern;
			if (kernel != null)
			{
			addPattern(kernel.NodeType, lpp);
			}
			else if (_rootPattern == null || lpp.noSmallerThan(_rootPattern))
			{
			_rootPattern = lpp;
			}
		}
		}

		/// <summary>
		/// Adds a pattern to a pattern group
		/// </summary>
		private void addPattern(int kernelType, LocationPathPattern pattern)
		{
		// Make sure the array of pattern groups is long enough
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldLength = _patternGroups.length;
		int oldLength = _patternGroups.Length;
		if (kernelType >= oldLength)
		{
			ArrayList[] newGroups = new ArrayList[kernelType * 2];
			Array.Copy(_patternGroups, 0, newGroups, 0, oldLength);
			_patternGroups = newGroups;
		}

		// Find the vector to put this pattern into
		ArrayList patterns;

		if (kernelType == DOM.NO_TYPE)
		{
			if (pattern.Axis == Axis.ATTRIBUTE)
			{
			patterns = (_attribNodeGroup == null) ? (_attribNodeGroup = new ArrayList(2)) : _attribNodeGroup;
			}
			else
			{
			patterns = (_childNodeGroup == null) ? (_childNodeGroup = new ArrayList(2)) : _childNodeGroup;
			}
		}
		else
		{
			patterns = (_patternGroups[kernelType] == null) ? (_patternGroups[kernelType] = new ArrayList(2)) : _patternGroups[kernelType];
		}

		if (patterns.Count == 0)
		{
			patterns.Add(pattern);
		}
		else
		{
			bool inserted = false;
			for (int i = 0; i < patterns.Count; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocationPathPattern lppToCompare = (LocationPathPattern)patterns.elementAt(i);
			LocationPathPattern lppToCompare = (LocationPathPattern)patterns[i];

			if (pattern.noSmallerThan(lppToCompare))
			{
				inserted = true;
				patterns.Insert(i, pattern);
				break;
			}
			}
			if (inserted == false)
			{
			patterns.Add(pattern);
			}
		}
		}

		/// <summary>
		/// Complete test sequences of a given type by adding all patterns
		/// from a given group.
		/// </summary>
		private void completeTestSequences(int nodeType, ArrayList patterns)
		{
		if (patterns != null)
		{
			if (_patternGroups[nodeType] == null)
			{
			_patternGroups[nodeType] = patterns;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int m = patterns.size();
			int m = patterns.Count;
			for (int j = 0; j < m; j++)
			{
				addPattern(nodeType, (LocationPathPattern) patterns[j]);
			}
			}
		}
		}

		/// <summary>
		/// Build test sequences. The first step is to complete the test sequences 
		/// by including patterns of "*" and "node()" kernel to all element test 
		/// sequences, and of "@*" to all attribute test sequences.
		/// </summary>
		private void prepareTestSequences()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector starGroup = _patternGroups[org.apache.xml.dtm.DTM.ELEMENT_NODE];
		ArrayList starGroup = _patternGroups[DTM.ELEMENT_NODE];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector atStarGroup = _patternGroups[org.apache.xml.dtm.DTM.ATTRIBUTE_NODE];
		ArrayList atStarGroup = _patternGroups[DTM.ATTRIBUTE_NODE];

		// Complete test sequence for "text()" with "child::node()"
		completeTestSequences(DTM.TEXT_NODE, _childNodeGroup);

		// Complete test sequence for "*" with "child::node()"
		completeTestSequences(DTM.ELEMENT_NODE, _childNodeGroup);

		// Complete test sequence for "pi()" with "child::node()"
		completeTestSequences(DTM.PROCESSING_INSTRUCTION_NODE, _childNodeGroup);

		// Complete test sequence for "comment()" with "child::node()"
		completeTestSequences(DTM.COMMENT_NODE, _childNodeGroup);

		// Complete test sequence for "@*" with "attribute::node()"
		completeTestSequences(DTM.ATTRIBUTE_NODE, _attribNodeGroup);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector names = _stylesheet.getXSLTC().getNamesIndex();
		ArrayList names = _stylesheet.XSLTC.NamesIndex;
		if (starGroup != null || atStarGroup != null || _childNodeGroup != null || _attribNodeGroup != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _patternGroups.length;
			int n = _patternGroups.Length;

			// Complete test sequence for user-defined types
			for (int i = DTM.NTYPES; i < n; i++)
			{
			if (_patternGroups[i] == null)
			{
				continue;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = (String) names.elementAt(i - org.apache.xml.dtm.DTM.NTYPES);
			string name = (string) names[i - DTM.NTYPES];

			if (isAttributeName(name))
			{
				// If an attribute then copy "@*" to its test sequence
				completeTestSequences(i, atStarGroup);

				// And also copy "attribute::node()" to its test sequence
				completeTestSequences(i, _attribNodeGroup);
			}
			else
			{
				// If an element then copy "*" to its test sequence
				completeTestSequences(i, starGroup);

				// And also copy "child::node()" to its test sequence
				completeTestSequences(i, _childNodeGroup);
			}
			}
		}

		_testSeq = new TestSeq[DTM.NTYPES + names.Count];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _patternGroups.length;
		int n = _patternGroups.Length;
		for (int i = 0; i < n; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector patterns = _patternGroups[i];
			ArrayList patterns = _patternGroups[i];
			if (patterns != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq testSeq = new TestSeq(patterns, i, this);
			TestSeq testSeq = new TestSeq(patterns, i, this);
	// System.out.println("testSeq[" + i + "] = " + testSeq);
			testSeq.reduce();
			_testSeq[i] = testSeq;
			testSeq.findTemplates(_neededTemplates);
			}
		}

		if (_childNodeGroup != null && _childNodeGroup.Count > 0)
		{
			_childNodeTestSeq = new TestSeq(_childNodeGroup, -1, this);
			_childNodeTestSeq.reduce();
			_childNodeTestSeq.findTemplates(_neededTemplates);
		}

	/*
		if (_attribNodeGroup != null && _attribNodeGroup.size() > 0) {
		    _attribNodeTestSeq = new TestSeq(_attribNodeGroup, -1, this);
		    _attribNodeTestSeq.reduce();
		    _attribNodeTestSeq.findTemplates(_neededTemplates);
		}
	*/

		if (_idxGroup != null && _idxGroup.Count > 0)
		{
			_idxTestSeq = new TestSeq(_idxGroup, this);
			_idxTestSeq.reduce();
			_idxTestSeq.findTemplates(_neededTemplates);
		}

		if (_rootPattern != null)
		{
			// doesn't matter what is 'put', only key matters
			_neededTemplates[_rootPattern.Template] = this;
		}
		}

		private void compileNamedTemplate(Template template, ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
		string methodName = Util.escape(template.Name.ToString());

		int numParams = 0;
		if (template.SimpleNamedTemplate)
		{
			ArrayList parameters = template.Parameters;
			numParams = parameters.Count;
		}

		// Initialize the types and names arrays for the NamedMethodGenerator. 
		org.apache.bcel.generic.Type[] types = new org.apache.bcel.generic.Type[4 + numParams];
		string[] names = new string[4 + numParams];
		types[0] = Util.getJCRefType(DOM_INTF_SIG);
		types[1] = Util.getJCRefType(NODE_ITERATOR_SIG);
		types[2] = Util.getJCRefType(TRANSLET_OUTPUT_SIG);
		types[3] = org.apache.bcel.generic.Type.INT;
		names[0] = DOCUMENT_PNAME;
		names[1] = ITERATOR_PNAME;
		names[2] = TRANSLET_OUTPUT_PNAME;
		names[3] = NODE_PNAME;

		// For simple named templates, the signature of the generated method
		// is not fixed. It depends on the number of parameters declared in the
		// template.
		for (int i = 4; i < 4 + numParams; i++)
		{
			types[i] = Util.getJCRefType(OBJECT_SIG);
			names[i] = "param" + (i - 4).ToString();
		}

		NamedMethodGenerator methodGen = new NamedMethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, types, names, methodName, ClassName, il, cpg);

		il.append(template.compile(classGen, methodGen));
		il.append(RETURN);

		classGen.addMethod(methodGen);
		}

		private void compileTemplates(ClassGenerator classGen, MethodGenerator methodGen, InstructionHandle next)
		{
			System.Collections.IEnumerator templates = _namedTemplates.Keys.GetEnumerator();
			while (templates.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)templates.Current;
				Template template = (Template)templates.Current;
				compileNamedTemplate(template, classGen);
			}

		templates = _neededTemplates.Keys.GetEnumerator();
		while (templates.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)templates.Current;
			Template template = (Template)templates.Current;
			if (template.hasContents())
			{
			// !!! TODO templates both named and matched
			InstructionList til = template.compile(classGen, methodGen);
			til.append(new GOTO_W(next));
			_templateILs[template] = til;
			_templateIHs[template] = til.getStart();
			}
			else
			{
			// empty template
			_templateIHs[template] = next;
			}
		}
		}

		private void appendTemplateCode(InstructionList body)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration templates = _neededTemplates.keys();
		System.Collections.IEnumerator templates = _neededTemplates.Keys.GetEnumerator();
		while (templates.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object iList = _templateILs.get(templates.Current);
			object iList = _templateILs[templates.Current];
			if (iList != null)
			{
			body.append((InstructionList)iList);
			}
		}
		}

		private void appendTestSequences(InstructionList body)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = _testSeq.length;
		int n = _testSeq.Length;
		for (int i = 0; i < n; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq testSeq = _testSeq[i];
			TestSeq testSeq = _testSeq[i];
			if (testSeq != null)
			{
			InstructionList il = testSeq.InstructionList;
			if (il != null)
			{
				body.append(il);
			}
			// else trivial TestSeq
			}
		}
		}

		public static void compileGetChildren(ClassGenerator classGen, MethodGenerator methodGen, int node)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int git = cpg.addInterfaceMethodref(DOM_INTF, GET_CHILDREN, GET_CHILDREN_SIG);
		int git = cpg.addInterfaceMethodref(DOM_INTF, GET_CHILDREN, GET_CHILDREN_SIG);
		il.append(methodGen.loadDOM());
		il.append(new ILOAD(node));
		il.append(new INVOKEINTERFACE(git, 2));
		}

		/// <summary>
		/// Compiles the default handling for DOM elements: traverse all children
		/// </summary>
		private InstructionList compileDefaultRecursion(ClassGenerator classGen, MethodGenerator methodGen, InstructionHandle next)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String applyTemplatesSig = classGen.getApplyTemplatesSig();
		string applyTemplatesSig = classGen.ApplyTemplatesSig;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int git = cpg.addInterfaceMethodref(DOM_INTF, GET_CHILDREN, GET_CHILDREN_SIG);
		int git = cpg.addInterfaceMethodref(DOM_INTF, GET_CHILDREN, GET_CHILDREN_SIG);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int applyTemplates = cpg.addMethodref(getClassName(), functionName(), applyTemplatesSig);
		int applyTemplates = cpg.addMethodref(ClassName, functionName(), applyTemplatesSig);
		il.append(classGen.loadTranslet());
		il.append(methodGen.loadDOM());

		il.append(methodGen.loadDOM());
		il.append(new ILOAD(_currentIndex));
		il.append(new INVOKEINTERFACE(git, 2));
		il.append(methodGen.loadHandler());
		il.append(new INVOKEVIRTUAL(applyTemplates));
		il.append(new GOTO_W(next));
		return il;
		}

		/// <summary>
		/// Compiles the default action for DOM text nodes and attribute nodes:
		/// output the node's text value
		/// </summary>
		private InstructionList compileDefaultText(ClassGenerator classGen, MethodGenerator methodGen, InstructionHandle next)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int chars = cpg.addInterfaceMethodref(DOM_INTF, CHARACTERS, CHARACTERS_SIG);
		int chars = cpg.addInterfaceMethodref(DOM_INTF, CHARACTERS, CHARACTERS_SIG);
		il.append(methodGen.loadDOM());
		il.append(new ILOAD(_currentIndex));
		il.append(methodGen.loadHandler());
		il.append(new INVOKEINTERFACE(chars, 3));
		il.append(new GOTO_W(next));
		return il;
		}

		private InstructionList compileNamespaces(ClassGenerator classGen, MethodGenerator methodGen, bool[] isNamespace, bool[] isAttribute, bool attrFlag, InstructionHandle defaultTarget)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = classGen.getParser().getXSLTC();
		XSLTC xsltc = classGen.Parser.XSLTC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();

		// Append switch() statement - namespace test dispatch loop
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector namespaces = xsltc.getNamespaceIndex();
		ArrayList namespaces = xsltc.NamespaceIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector names = xsltc.getNamesIndex();
		ArrayList names = xsltc.NamesIndex;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int namespaceCount = namespaces.size() + 1;
		int namespaceCount = namespaces.Count + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int namesCount = names.size();
		int namesCount = names.Count;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] types = new int[namespaceCount];
		int[] types = new int[namespaceCount];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle[] targets = new org.apache.bcel.generic.InstructionHandle[types.length];
		InstructionHandle[] targets = new InstructionHandle[types.Length];

		if (namespaceCount > 0)
		{
			bool compiled = false;

			// Initialize targets for namespace() switch statement
			for (int i = 0; i < namespaceCount; i++)
			{
			targets[i] = defaultTarget;
			types[i] = i;
			}

			// Add test sequences for known namespace types
			for (int i = DTM.NTYPES; i < (DTM.NTYPES + namesCount); i++)
			{
			if ((isNamespace[i]) && (isAttribute[i] == attrFlag))
			{
				string name = (string)names[i - DTM.NTYPES];
				string @namespace = name.Substring(0, name.LastIndexOf(':'));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int type = xsltc.registerNamespace(namespace);
				int type = xsltc.registerNamespace(@namespace);

				if ((i < _testSeq.Length) && (_testSeq[i] != null))
				{
				targets[type] = (_testSeq[i]).compile(classGen, methodGen, defaultTarget);
				compiled = true;
				}
			}
			}

			// Return "null" if no test sequences were compiled
			if (!compiled)
			{
				return (null);
			}

			// Append first code in applyTemplates() - get type of current node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getNS = cpg.addInterfaceMethodref(DOM_INTF, "getNamespaceType", "(I)I");
			int getNS = cpg.addInterfaceMethodref(DOM_INTF, "getNamespaceType", "(I)I");
			il.append(methodGen.loadDOM());
			il.append(new ILOAD(_currentIndex));
			il.append(new INVOKEINTERFACE(getNS, 2));
			il.append(new SWITCH(types, targets, defaultTarget));
			return (il);
		}
		else
		{
			return (null);
		}
		}

	   /// <summary>
	   /// Compiles the applyTemplates() method and adds it to the translet.
	   /// This is the main dispatch method.
	   /// </summary>
		public void compileApplyTemplates(ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = classGen.getParser().getXSLTC();
		XSLTC xsltc = classGen.Parser.XSLTC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector names = xsltc.getNamesIndex();
		ArrayList names = xsltc.NamesIndex;

		// Create the applyTemplates() method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[3];
		org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[3];
		argTypes[0] = Util.getJCRefType(DOM_INTF_SIG);
		argTypes[1] = Util.getJCRefType(NODE_ITERATOR_SIG);
		argTypes[2] = Util.getJCRefType(TRANSLET_OUTPUT_SIG);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argNames = new String[3];
		string[] argNames = new string[3];
		argNames[0] = DOCUMENT_PNAME;
		argNames[1] = ITERATOR_PNAME;
		argNames[2] = TRANSLET_OUTPUT_PNAME;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList mainIL = new org.apache.bcel.generic.InstructionList();
		InstructionList mainIL = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator methodGen = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC | ACC_FINAL, org.apache.bcel.generic.Type.VOID, argTypes, argNames, functionName(), getClassName(), mainIL, classGen.getConstantPool());
		MethodGenerator methodGen = new MethodGenerator(ACC_PUBLIC | ACC_FINAL, org.apache.bcel.generic.Type.VOID, argTypes, argNames, functionName(), ClassName, mainIL, classGen.getConstantPool());
		methodGen.addException("org.apache.xalan.xsltc.TransletException");

			// Insert an extra NOP just to keep "current" from appearing as if it
			// has a value before the start of the loop.
			mainIL.append(NOP);

			// Create a local variable to hold the current node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen current;
		LocalVariableGen current;
		current = methodGen.addLocalVariable2("current", org.apache.bcel.generic.Type.INT, null);
		_currentIndex = current.getIndex();

		// Create the "body" instruction list that will eventually hold the
		// code for the entire method (other ILs will be appended).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList body = new org.apache.bcel.generic.InstructionList();
		InstructionList body = new InstructionList();
			body.append(NOP);

		// Create an instruction list that contains the default next-node
		// iteration
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList ilLoop = new org.apache.bcel.generic.InstructionList();
		InstructionList ilLoop = new InstructionList();
		ilLoop.append(methodGen.loadIterator());
		ilLoop.append(methodGen.nextNode());
		ilLoop.append(DUP);
		ilLoop.append(new ISTORE(_currentIndex));

		// The body of this code can get very large - large than can be handled
		// by a single IFNE(body.getStart()) instruction - need workaround:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle ifeq = ilLoop.append(new org.apache.bcel.generic.IFLT(null));
			BranchHandle ifeq = ilLoop.append(new IFLT(null));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.BranchHandle loop = ilLoop.append(new org.apache.bcel.generic.GOTO_W(null));
		BranchHandle loop = ilLoop.append(new GOTO_W(null));
		ifeq.setTarget(ilLoop.append(RETURN)); // applyTemplates() ends here!
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle ihLoop = ilLoop.getStart();
		InstructionHandle ihLoop = ilLoop.getStart();

			current.setStart(mainIL.append(new GOTO_W(ihLoop)));

			// Live range of "current" ends at end of loop
			current.setEnd(loop);

		// Compile default handling of elements (traverse children)
		InstructionList ilRecurse = compileDefaultRecursion(classGen, methodGen, ihLoop);
		InstructionHandle ihRecurse = ilRecurse.getStart();

		// Compile default handling of text/attribute nodes (output text)
		InstructionList ilText = compileDefaultText(classGen, methodGen, ihLoop);
		InstructionHandle ihText = ilText.getStart();

		// Distinguish attribute/element/namespace tests for further processing
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] types = new int[org.apache.xml.dtm.DTM.NTYPES + names.size()];
		int[] types = new int[DTM.NTYPES + names.Count];
		for (int i = 0; i < types.Length; i++)
		{
			types[i] = i;
		}

		// Initialize isAttribute[] and isNamespace[] arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] isAttribute = new boolean[types.length];
		bool[] isAttribute = new bool[types.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] isNamespace = new boolean[types.length];
		bool[] isNamespace = new bool[types.Length];
		for (int i = 0; i < names.Count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = (String)names.elementAt(i);
			string name = (string)names[i];
			isAttribute[i + DTM.NTYPES] = isAttributeName(name);
			isNamespace[i + DTM.NTYPES] = isNamespaceName(name);
		}

		// Compile all templates - regardless of pattern type
		compileTemplates(classGen, methodGen, ihLoop);

		// Handle template with explicit "*" pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq elemTest = _testSeq[org.apache.xml.dtm.DTM.ELEMENT_NODE];
		TestSeq elemTest = _testSeq[DTM.ELEMENT_NODE];
		InstructionHandle ihElem = ihRecurse;
		if (elemTest != null)
		{
			ihElem = elemTest.compile(classGen, methodGen, ihRecurse);
		}

		// Handle template with explicit "@*" pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq attrTest = _testSeq[org.apache.xml.dtm.DTM.ATTRIBUTE_NODE];
		TestSeq attrTest = _testSeq[DTM.ATTRIBUTE_NODE];
		InstructionHandle ihAttr = ihText;
		if (attrTest != null)
		{
			ihAttr = attrTest.compile(classGen, methodGen, ihAttr);
		}

		// Do tests for id() and key() patterns first
		InstructionList ilKey = null;
		if (_idxTestSeq != null)
		{
			loop.setTarget(_idxTestSeq.compile(classGen, methodGen, body.getStart()));
			ilKey = _idxTestSeq.InstructionList;
		}
		else
		{
			loop.setTarget(body.getStart());
		}

		// If there is a match on node() we need to replace ihElem
		// and ihText if the priority of node() is higher
		if (_childNodeTestSeq != null)
		{
			// Compare priorities of node() and "*"
			double nodePrio = _childNodeTestSeq.Priority;
			int nodePos = _childNodeTestSeq.Position;
			double elemPrio = (0 - double.MaxValue);
			int elemPos = int.MinValue;

			if (elemTest != null)
			{
			elemPrio = elemTest.Priority;
			elemPos = elemTest.Position;
			}
			if (elemPrio == Double.NaN || elemPrio < nodePrio || (elemPrio == nodePrio && elemPos < nodePos))
			{
			ihElem = _childNodeTestSeq.compile(classGen, methodGen, ihLoop);
			}

			// Compare priorities of node() and text()
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq textTest = _testSeq[org.apache.xml.dtm.DTM.TEXT_NODE];
			TestSeq textTest = _testSeq[DTM.TEXT_NODE];
			double textPrio = (0 - double.MaxValue);
			int textPos = int.MinValue;

			if (textTest != null)
			{
			textPrio = textTest.Priority;
			textPos = textTest.Position;
			}
			if (double.IsNaN(textPrio) || textPrio < nodePrio || (textPrio == nodePrio && textPos < nodePos))
			{
			ihText = _childNodeTestSeq.compile(classGen, methodGen, ihLoop);
			_testSeq[DTM.TEXT_NODE] = _childNodeTestSeq;
			}
		}

		// Handle templates with "ns:*" pattern
		InstructionHandle elemNamespaceHandle = ihElem;
		InstructionList nsElem = compileNamespaces(classGen, methodGen, isNamespace, isAttribute, false, ihElem);
		if (nsElem != null)
		{
			elemNamespaceHandle = nsElem.getStart();
		}

		// Handle templates with "ns:@*" pattern
		InstructionHandle attrNamespaceHandle = ihAttr;
		InstructionList nsAttr = compileNamespaces(classGen, methodGen, isNamespace, isAttribute, true, ihAttr);
		if (nsAttr != null)
		{
			attrNamespaceHandle = nsAttr.getStart();
		}

		// Handle templates with "ns:elem" or "ns:@attr" pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle[] targets = new org.apache.bcel.generic.InstructionHandle[types.length];
		InstructionHandle[] targets = new InstructionHandle[types.Length];
		for (int i = DTM.NTYPES; i < targets.Length; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq testSeq = _testSeq[i];
			TestSeq testSeq = _testSeq[i];
			// Jump straight to namespace tests ?
			if (isNamespace[i])
			{
			if (isAttribute[i])
			{
				targets[i] = attrNamespaceHandle;
			}
			else
			{
				targets[i] = elemNamespaceHandle;
			}
			}
			// Test first, then jump to namespace tests
			else if (testSeq != null)
			{
			if (isAttribute[i])
			{
				targets[i] = testSeq.compile(classGen, methodGen, attrNamespaceHandle);
			}
			else
			{
				targets[i] = testSeq.compile(classGen, methodGen, elemNamespaceHandle);
			}
			}
			else
			{
			targets[i] = ihLoop;
			}
		}


		// Handle pattern with match on root node - default: traverse children
		targets[DTM.ROOT_NODE] = _rootPattern != null ? getTemplateInstructionHandle(_rootPattern.Template) : ihRecurse;

			// Handle pattern with match on root node - default: traverse children
		targets[DTM.DOCUMENT_NODE] = _rootPattern != null ? getTemplateInstructionHandle(_rootPattern.Template) : ihRecurse;

		// Handle any pattern with match on text nodes - default: output text
		targets[DTM.TEXT_NODE] = _testSeq[DTM.TEXT_NODE] != null ? _testSeq[DTM.TEXT_NODE].compile(classGen, methodGen, ihText) : ihText;

		// This DOM-type is not in use - default: process next node
		targets[DTM.NAMESPACE_NODE] = ihLoop;

		// Match unknown element in DOM - default: check for namespace match
		targets[DTM.ELEMENT_NODE] = elemNamespaceHandle;

		// Match unknown attribute in DOM - default: check for namespace match
		targets[DTM.ATTRIBUTE_NODE] = attrNamespaceHandle;

		// Match on processing instruction - default: process next node
		InstructionHandle ihPI = ihLoop;
		if (_childNodeTestSeq != null)
		{
			ihPI = ihElem;
		}
		if (_testSeq[DTM.PROCESSING_INSTRUCTION_NODE] != null)
		{
			targets[DTM.PROCESSING_INSTRUCTION_NODE] = _testSeq[DTM.PROCESSING_INSTRUCTION_NODE].compile(classGen, methodGen, ihPI);
		}
		else
		{
			targets[DTM.PROCESSING_INSTRUCTION_NODE] = ihPI;
		}

		// Match on comments - default: process next node
		InstructionHandle ihComment = ihLoop;
		if (_childNodeTestSeq != null)
		{
			ihComment = ihElem;
		}
		targets[DTM.COMMENT_NODE] = _testSeq[DTM.COMMENT_NODE] != null ? _testSeq[DTM.COMMENT_NODE].compile(classGen, methodGen, ihComment) : ihComment;

			// This DOM-type is not in use - default: process next node
		targets[DTM.CDATA_SECTION_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.DOCUMENT_FRAGMENT_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.DOCUMENT_TYPE_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.ENTITY_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.ENTITY_REFERENCE_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.NOTATION_NODE] = ihLoop;


		// Now compile test sequences for various match patterns:
		for (int i = DTM.NTYPES; i < targets.Length; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq testSeq = _testSeq[i];
			TestSeq testSeq = _testSeq[i];
			// Jump straight to namespace tests ?
			if ((testSeq == null) || (isNamespace[i]))
			{
			if (isAttribute[i])
			{
				targets[i] = attrNamespaceHandle;
			}
			else
			{
				targets[i] = elemNamespaceHandle;
			}
			}
			// Match on node type
			else
			{
			if (isAttribute[i])
			{
				targets[i] = testSeq.compile(classGen, methodGen, attrNamespaceHandle);
			}
			else
			{
				targets[i] = testSeq.compile(classGen, methodGen, elemNamespaceHandle);
			}
			}
		}

		if (ilKey != null)
		{
			body.insert(ilKey);
		}

		// Append first code in applyTemplates() - get type of current node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
		int getType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
		body.append(methodGen.loadDOM());
		body.append(new ILOAD(_currentIndex));
		body.append(new INVOKEINTERFACE(getType, 2));

		// Append switch() statement - main dispatch loop in applyTemplates()
		InstructionHandle disp = body.append(new SWITCH(types, targets, ihLoop));

		// Append all the "case:" statements
		appendTestSequences(body);
		// Append the actual template code
		appendTemplateCode(body);

		// Append NS:* node tests (if any)
		if (nsElem != null)
		{
			body.append(nsElem);
		}
		// Append NS:@* node tests (if any)
		if (nsAttr != null)
		{
			body.append(nsAttr);
		}

		// Append default action for element and root nodes
		body.append(ilRecurse);
		// Append default action for text and attribute nodes
		body.append(ilText);

		// putting together constituent instruction lists
		mainIL.append(body);
		// fall through to ilLoop
		mainIL.append(ilLoop);

		peepHoleOptimization(methodGen);

			classGen.addMethod(methodGen);

		// Compile method(s) for <xsl:apply-imports/> for this mode
		if (_importLevels != null)
		{
			System.Collections.IEnumerator levels = _importLevels.Keys.GetEnumerator();
			while (levels.MoveNext())
			{
			int? max = (int?)levels.Current;
			int? min = (int?)_importLevels[max];
			compileApplyImports(classGen, min.Value, max.Value);
			}
		}
		}

		private void compileTemplateCalls(ClassGenerator classGen, MethodGenerator methodGen, InstructionHandle next, int min, int max)
		{
			System.Collections.IEnumerator templates = _neededTemplates.Keys.GetEnumerator();
		while (templates.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)templates.Current;
			Template template = (Template)templates.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int prec = template.getImportPrecedence();
			int prec = template.ImportPrecedence;
			if ((prec >= min) && (prec < max))
			{
			if (template.hasContents())
			{
				InstructionList til = template.compile(classGen, methodGen);
				til.append(new GOTO_W(next));
				_templateILs[template] = til;
				_templateIHs[template] = til.getStart();
			}
			else
			{
				// empty template
				_templateIHs[template] = next;
			}
			}
		}
		}


		public void compileApplyImports(ClassGenerator classGen, int min, int max)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = classGen.getParser().getXSLTC();
		XSLTC xsltc = classGen.Parser.XSLTC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector names = xsltc.getNamesIndex();
		ArrayList names = xsltc.NamesIndex;

		// Clear some datastructures
		_namedTemplates = new Hashtable();
		_neededTemplates = new Hashtable();
		_templateIHs = new Hashtable();
		_templateILs = new Hashtable();
		_patternGroups = new ArrayList[32];
		_rootPattern = null;

		// IMPORTANT: Save orignal & complete set of templates!!!!
		ArrayList oldTemplates = _templates;

		// Gather templates that are within the scope of this import
		_templates = new ArrayList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration templates = oldTemplates.elements();
		System.Collections.IEnumerator templates = oldTemplates.GetEnumerator();
		while (templates.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)templates.Current;
			Template template = (Template)templates.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int prec = template.getImportPrecedence();
			int prec = template.ImportPrecedence;
			if ((prec >= min) && (prec < max))
			{
				addTemplate(template);
			}
		}

		// Process all patterns from those templates
		processPatterns(_keys);

		// Create the applyTemplates() method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[4];
		org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[4];
		argTypes[0] = Util.getJCRefType(DOM_INTF_SIG);
		argTypes[1] = Util.getJCRefType(NODE_ITERATOR_SIG);
		argTypes[2] = Util.getJCRefType(TRANSLET_OUTPUT_SIG);
		argTypes[3] = org.apache.bcel.generic.Type.INT;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argNames = new String[4];
		string[] argNames = new string[4];
		argNames[0] = DOCUMENT_PNAME;
		argNames[1] = ITERATOR_PNAME;
		argNames[2] = TRANSLET_OUTPUT_PNAME;
		argNames[3] = NODE_PNAME;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList mainIL = new org.apache.bcel.generic.InstructionList();
		InstructionList mainIL = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator methodGen = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC | ACC_FINAL, org.apache.bcel.generic.Type.VOID, argTypes, argNames, functionName()+'_'+max, getClassName(), mainIL, classGen.getConstantPool());
		MethodGenerator methodGen = new MethodGenerator(ACC_PUBLIC | ACC_FINAL, org.apache.bcel.generic.Type.VOID, argTypes, argNames, functionName() + '_' + max, ClassName, mainIL, classGen.getConstantPool());
		methodGen.addException("org.apache.xalan.xsltc.TransletException");

		// Create the local variable to hold the current node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen current;
		LocalVariableGen current;
		current = methodGen.addLocalVariable2("current", org.apache.bcel.generic.Type.INT, null);
		_currentIndex = current.getIndex();

		mainIL.append(new ILOAD(methodGen.getLocalIndex(NODE_PNAME)));
		current.setStart(mainIL.append(new ISTORE(_currentIndex)));

		// Create the "body" instruction list that will eventually hold the
		// code for the entire method (other ILs will be appended).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList body = new org.apache.bcel.generic.InstructionList();
		InstructionList body = new InstructionList();
		body.append(NOP);

		// Create an instruction list that contains the default next-node
		// iteration
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList ilLoop = new org.apache.bcel.generic.InstructionList();
		InstructionList ilLoop = new InstructionList();
		ilLoop.append(RETURN);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle ihLoop = ilLoop.getStart();
		InstructionHandle ihLoop = ilLoop.getStart();

		// Compile default handling of elements (traverse children)
		InstructionList ilRecurse = compileDefaultRecursion(classGen, methodGen, ihLoop);
		InstructionHandle ihRecurse = ilRecurse.getStart();

		// Compile default handling of text/attribute nodes (output text)
		InstructionList ilText = compileDefaultText(classGen, methodGen, ihLoop);
		InstructionHandle ihText = ilText.getStart();

		// Distinguish attribute/element/namespace tests for further processing
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] types = new int[org.apache.xml.dtm.DTM.NTYPES + names.size()];
		int[] types = new int[DTM.NTYPES + names.Count];
		for (int i = 0; i < types.Length; i++)
		{
			types[i] = i;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] isAttribute = new boolean[types.length];
		bool[] isAttribute = new bool[types.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean[] isNamespace = new boolean[types.length];
		bool[] isNamespace = new bool[types.Length];
		for (int i = 0; i < names.Count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = (String)names.elementAt(i);
			string name = (string)names[i];
			isAttribute[i + DTM.NTYPES] = isAttributeName(name);
			isNamespace[i + DTM.NTYPES] = isNamespaceName(name);
		}

		// Compile all templates - regardless of pattern type
		compileTemplateCalls(classGen, methodGen, ihLoop, min, max);

		// Handle template with explicit "*" pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq elemTest = _testSeq[org.apache.xml.dtm.DTM.ELEMENT_NODE];
		TestSeq elemTest = _testSeq[DTM.ELEMENT_NODE];
		InstructionHandle ihElem = ihRecurse;
		if (elemTest != null)
		{
			ihElem = elemTest.compile(classGen, methodGen, ihLoop);
		}

		// Handle template with explicit "@*" pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq attrTest = _testSeq[org.apache.xml.dtm.DTM.ATTRIBUTE_NODE];
		TestSeq attrTest = _testSeq[DTM.ATTRIBUTE_NODE];
		InstructionHandle ihAttr = ihLoop;
		if (attrTest != null)
		{
			ihAttr = attrTest.compile(classGen, methodGen, ihAttr);
		}

		// Do tests for id() and key() patterns first
		InstructionList ilKey = null;
		if (_idxTestSeq != null)
		{
			ilKey = _idxTestSeq.InstructionList;
		}

		// If there is a match on node() we need to replace ihElem
		// and ihText if the priority of node() is higher
		if (_childNodeTestSeq != null)
		{
			// Compare priorities of node() and "*"
			double nodePrio = _childNodeTestSeq.Priority;
			int nodePos = _childNodeTestSeq.Position;
			double elemPrio = (0 - double.MaxValue);
			int elemPos = int.MinValue;

			if (elemTest != null)
			{
			elemPrio = elemTest.Priority;
			elemPos = elemTest.Position;
			}

			if (elemPrio == Double.NaN || elemPrio < nodePrio || (elemPrio == nodePrio && elemPos < nodePos))
			{
			ihElem = _childNodeTestSeq.compile(classGen, methodGen, ihLoop);
			}

			// Compare priorities of node() and text()
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq textTest = _testSeq[org.apache.xml.dtm.DTM.TEXT_NODE];
			TestSeq textTest = _testSeq[DTM.TEXT_NODE];
			double textPrio = (0 - double.MaxValue);
			int textPos = int.MinValue;

			if (textTest != null)
			{
			textPrio = textTest.Priority;
			textPos = textTest.Position;
			}

			if (double.IsNaN(textPrio) || textPrio < nodePrio || (textPrio == nodePrio && textPos < nodePos))
			{
			ihText = _childNodeTestSeq.compile(classGen, methodGen, ihLoop);
			_testSeq[DTM.TEXT_NODE] = _childNodeTestSeq;
			}
		}

		// Handle templates with "ns:*" pattern
		InstructionHandle elemNamespaceHandle = ihElem;
		InstructionList nsElem = compileNamespaces(classGen, methodGen, isNamespace, isAttribute, false, ihElem);
		if (nsElem != null)
		{
			elemNamespaceHandle = nsElem.getStart();
		}

		// Handle templates with "ns:@*" pattern
		InstructionList nsAttr = compileNamespaces(classGen, methodGen, isNamespace, isAttribute, true, ihAttr);
		InstructionHandle attrNamespaceHandle = ihAttr;
		if (nsAttr != null)
		{
			attrNamespaceHandle = nsAttr.getStart();
		}

		// Handle templates with "ns:elem" or "ns:@attr" pattern
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle[] targets = new org.apache.bcel.generic.InstructionHandle[types.length];
		InstructionHandle[] targets = new InstructionHandle[types.Length];
		for (int i = DTM.NTYPES; i < targets.Length; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq testSeq = _testSeq[i];
			TestSeq testSeq = _testSeq[i];
			// Jump straight to namespace tests ?
			if (isNamespace[i])
			{
			if (isAttribute[i])
			{
				targets[i] = attrNamespaceHandle;
			}
			else
			{
				targets[i] = elemNamespaceHandle;
			}
			}
			// Test first, then jump to namespace tests
			else if (testSeq != null)
			{
			if (isAttribute[i])
			{
				targets[i] = testSeq.compile(classGen, methodGen, attrNamespaceHandle);
			}
			else
			{
				targets[i] = testSeq.compile(classGen, methodGen, elemNamespaceHandle);
			}
			}
			else
			{
			targets[i] = ihLoop;
			}
		}

		// Handle pattern with match on root node - default: traverse children
		targets[DTM.ROOT_NODE] = _rootPattern != null ? getTemplateInstructionHandle(_rootPattern.Template) : ihRecurse;
		// Handle pattern with match on root node - default: traverse children
		targets[DTM.DOCUMENT_NODE] = _rootPattern != null ? getTemplateInstructionHandle(_rootPattern.Template) : ihRecurse; // %HZ%:  Was ihLoop in XSLTC_DTM branch

		// Handle any pattern with match on text nodes - default: loop
		targets[DTM.TEXT_NODE] = _testSeq[DTM.TEXT_NODE] != null ? _testSeq[DTM.TEXT_NODE].compile(classGen, methodGen, ihText) : ihText;

		// This DOM-type is not in use - default: process next node
		targets[DTM.NAMESPACE_NODE] = ihLoop;

		// Match unknown element in DOM - default: check for namespace match
		targets[DTM.ELEMENT_NODE] = elemNamespaceHandle;

		// Match unknown attribute in DOM - default: check for namespace match
		targets[DTM.ATTRIBUTE_NODE] = attrNamespaceHandle;

		// Match on processing instruction - default: loop
		InstructionHandle ihPI = ihLoop;
		if (_childNodeTestSeq != null)
		{
			ihPI = ihElem;
		}
		if (_testSeq[DTM.PROCESSING_INSTRUCTION_NODE] != null)
		{
			targets[DTM.PROCESSING_INSTRUCTION_NODE] = _testSeq[DTM.PROCESSING_INSTRUCTION_NODE].compile(classGen, methodGen, ihPI);
		}
		else
		{
			targets[DTM.PROCESSING_INSTRUCTION_NODE] = ihPI;
		}

		// Match on comments - default: process next node
		InstructionHandle ihComment = ihLoop;
		if (_childNodeTestSeq != null)
		{
			ihComment = ihElem;
		}
		targets[DTM.COMMENT_NODE] = _testSeq[DTM.COMMENT_NODE] != null ? _testSeq[DTM.COMMENT_NODE].compile(classGen, methodGen, ihComment) : ihComment;

				// This DOM-type is not in use - default: process next node
		targets[DTM.CDATA_SECTION_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.DOCUMENT_FRAGMENT_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.DOCUMENT_TYPE_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.ENTITY_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.ENTITY_REFERENCE_NODE] = ihLoop;

		// This DOM-type is not in use - default: process next node
		targets[DTM.NOTATION_NODE] = ihLoop;



		// Now compile test sequences for various match patterns:
		for (int i = DTM.NTYPES; i < targets.Length; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestSeq testSeq = _testSeq[i];
			TestSeq testSeq = _testSeq[i];
			// Jump straight to namespace tests ?
			if ((testSeq == null) || (isNamespace[i]))
			{
			if (isAttribute[i])
			{
				targets[i] = attrNamespaceHandle;
			}
			else
			{
				targets[i] = elemNamespaceHandle;
			}
			}
			// Match on node type
			else
			{
			if (isAttribute[i])
			{
				targets[i] = testSeq.compile(classGen, methodGen, attrNamespaceHandle);
			}
			else
			{
				targets[i] = testSeq.compile(classGen, methodGen, elemNamespaceHandle);
			}
			}
		}

		if (ilKey != null)
		{
			body.insert(ilKey);
		}

		// Append first code in applyTemplates() - get type of current node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int getType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
		int getType = cpg.addInterfaceMethodref(DOM_INTF, "getExpandedTypeID", "(I)I");
		body.append(methodGen.loadDOM());
		body.append(new ILOAD(_currentIndex));
		body.append(new INVOKEINTERFACE(getType, 2));

		// Append switch() statement - main dispatch loop in applyTemplates()
		InstructionHandle disp = body.append(new SWITCH(types,targets,ihLoop));

		// Append all the "case:" statements
		appendTestSequences(body);
		// Append the actual template code
		appendTemplateCode(body);

		// Append NS:* node tests (if any)
		if (nsElem != null)
		{
			body.append(nsElem);
		}
		// Append NS:@* node tests (if any)
		if (nsAttr != null)
		{
			body.append(nsAttr);
		}

		// Append default action for element and root nodes
		body.append(ilRecurse);
		// Append default action for text and attribute nodes
		body.append(ilText);

		// putting together constituent instruction lists
		mainIL.append(body);

			// Mark the end of the live range for the "current" variable 
			current.setEnd(body.getEnd());

			// fall through to ilLoop
		mainIL.append(ilLoop);

		peepHoleOptimization(methodGen);

			classGen.addMethod(methodGen);

			// Restore original (complete) set of templates for this transformation
		_templates = oldTemplates;
		}

		/// <summary>
		/// Peephole optimization.
		/// </summary>
		private void peepHoleOptimization(MethodGenerator methodGen)
		{
			InstructionList il = methodGen.getInstructionList();
			InstructionFinder find = new InstructionFinder(il);
		InstructionHandle ih;
		string pattern;

		// LoadInstruction, POP => (removed)
		pattern = "LoadInstruction POP";
		for (System.Collections.IEnumerator iter = find.search(pattern); iter.MoveNext();)
		{
			InstructionHandle[] match = (InstructionHandle[]) iter.Current;
			try
			{
			if (!match[0].hasTargeters() && !match[1].hasTargeters())
			{
						il.delete(match[0], match[1]);
			}
			}
			catch (TargetLostException)
			{
					// TODO: move target down into the list
			}
		}

		// ILOAD_N, ILOAD_N, SWAP, ISTORE_N => ILOAD_N
		pattern = "ILOAD ILOAD SWAP ISTORE";
		for (System.Collections.IEnumerator iter = find.search(pattern); iter.MoveNext();)
		{
				InstructionHandle[] match = (InstructionHandle[]) iter.Current;
				try
				{
					ILOAD iload1 = (ILOAD) match[0].getInstruction();
					ILOAD iload2 = (ILOAD) match[1].getInstruction();
					ISTORE istore = (ISTORE) match[3].getInstruction();

					if (!match[1].hasTargeters() && !match[2].hasTargeters() && !match[3].hasTargeters() && iload1.getIndex() == iload2.getIndex() && iload2.getIndex() == istore.getIndex())
					{
						il.delete(match[1], match[3]);
					}
				}
				catch (TargetLostException)
				{
					// TODO: move target down into the list
				}
		}

			// LoadInstruction_N, LoadInstruction_M, SWAP => LoadInstruction_M, LoadInstruction_N
		pattern = "LoadInstruction LoadInstruction SWAP";
		for (System.Collections.IEnumerator iter = find.search(pattern); iter.MoveNext();)
		{
				InstructionHandle[] match = (InstructionHandle[])iter.Current;
				try
				{
					if (!match[0].hasTargeters() && !match[1].hasTargeters() && !match[2].hasTargeters())
					{
						Instruction load_m = match[1].getInstruction();
						il.insert(match[0], load_m);
						il.delete(match[1], match[2]);
					}
				}
				catch (TargetLostException)
				{
					// TODO: move target down into the list
				}
		}

			// ALOAD_N ALOAD_N => ALOAD_N DUP
		pattern = "ALOAD ALOAD";
			for (System.Collections.IEnumerator iter = find.search(pattern); iter.MoveNext();)
			{
				InstructionHandle[] match = (InstructionHandle[])iter.Current;
				try
				{
					if (!match[1].hasTargeters())
					{
						org.apache.bcel.generic.ALOAD aload1 = (org.apache.bcel.generic.ALOAD) match[0].getInstruction();
						org.apache.bcel.generic.ALOAD aload2 = (org.apache.bcel.generic.ALOAD) match[1].getInstruction();

						if (aload1.getIndex() == aload2.getIndex())
						{
							il.insert(match[1], new DUP());
							il.delete(match[1]);
						}
					}
				}
				catch (TargetLostException)
				{
					// TODO: move target down into the list
				}
			}
		}

		public InstructionHandle getTemplateInstructionHandle(Template template)
		{
		return (InstructionHandle)_templateIHs[template];
		}

		/// <summary>
		/// Auxiliary method to determine if a qname is an attribute.
		/// </summary>
		private static bool isAttributeName(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = qname.lastIndexOf(':') + 1;
		int col = qname.LastIndexOf(':') + 1;
		return (qname[col] == '@');
		}

		/// <summary>
		/// Auxiliary method to determine if a qname is a namespace 
		/// qualified "*".
		/// </summary>
		private static bool isNamespaceName(string qname)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = qname.lastIndexOf(':');
		int col = qname.LastIndexOf(':');
		return (col > -1 && qname[qname.Length - 1] == '*');
		}
	}

}