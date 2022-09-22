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
 * $Id: Whitespace.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using ALOAD = org.apache.bcel.generic.ALOAD;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using IF_ICMPEQ = org.apache.bcel.generic.IF_ICMPEQ;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Whitespace : TopLevelElement
	{
		// Three possible actions for the translet:
		public const int USE_PREDICATE = 0;
		public int Constants_Fields;
		public const int PRESERVE_SPACE = 2;

		// The 3 different categories of strip/preserve rules (order important)
		public const int RULE_NONE = 0;
		public const int RULE_ELEMENT = 1; // priority 0
		public const int RULE_NAMESPACE = 2; // priority -1/4
		public const int RULE_ALL = 3; // priority -1/2

		private string _elementList;
		private int _action;
		private int _importPrecedence;

		/// <summary>
		/// Auxillary class for encapsulating a single strip/preserve rule
		/// </summary>
		private sealed class WhitespaceRule
		{
		internal readonly int _action;
		internal string _namespace; // Should be replaced by NS type (int)
		internal string _element; // Should be replaced by node type (int)
		internal int _type;
		internal int _priority;

		/// <summary>
		/// Strip/preserve rule constructor
		/// </summary>
		public WhitespaceRule(int action, string element, int precedence)
		{
			 // Determine the action (strip or preserve) for this rule
			_action = action;

			// Get the namespace and element name for this rule
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = element.lastIndexOf(':');
			int colon = element.LastIndexOf(':');
			if (colon >= 0)
			{
			_namespace = element.Substring(0,colon);
			_element = element.Substring(colon + 1, element.Length - (colon + 1));
			}
			else
			{
			_namespace = Constants_Fields.EMPTYSTRING;
			_element = element;
			}

			// Determine the initial priority for this rule
			_priority = precedence << 2;

			// Get the strip/preserve type; either "NS:EL", "NS:*" or "*"
			if (_element.Equals("*"))
			{
			if (string.ReferenceEquals(_namespace, Constants_Fields.EMPTYSTRING))
			{
				_type = RULE_ALL; // Strip/preserve _all_ elements
				_priority += 2; // Lowest priority
			}
			else
			{
				_type = RULE_NAMESPACE; // Strip/reserve elements within NS
				_priority += 1; // Medium priority
			}
			}
			else
			{
			_type = RULE_ELEMENT; // Strip/preserve single element
			}
		}

		/// <summary>
		/// For sorting rules depending on priority
		/// </summary>
		public int compareTo(WhitespaceRule other)
		{
			return _priority < other._priority ? -1 : _priority > other._priority ? 1 : 0;
		}

		public int Action
		{
			get
			{
				return _action;
			}
		}
		public int Strength
		{
			get
			{
				return _type;
			}
		}
		public int Priority
		{
			get
			{
				return _priority;
			}
		}
		public string Element
		{
			get
			{
				return _element;
			}
		}
		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}
		}

		/// <summary>
		/// Parse the attributes of the xsl:strip/preserve-space element.
		/// The element should have not contents (ignored if any).
		/// </summary>
		public override void parseContents(Parser parser)
		{
			// Determine if this is an xsl:strip- or preserve-space element
			_action = _qname.LocalPart.EndsWith("strip-space", StringComparison.Ordinal) ? Constants_Fields.STRIP_SPACE : PRESERVE_SPACE;

			// Determine the import precedence
			_importPrecedence = parser.CurrentImportPrecedence;

			// Get the list of elements to strip/preserve
			_elementList = getAttribute("elements");
			if (string.ReferenceEquals(_elementList, null) || _elementList.Length == 0)
			{
				reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "elements");
				return;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
			SymbolTable stable = parser.SymbolTable;
			StringTokenizer list = new StringTokenizer(_elementList);
			StringBuilder elements = new StringBuilder(Constants_Fields.EMPTYSTRING);

			while (list.hasMoreElements())
			{
				string token = list.nextToken();
				string prefix;
				string @namespace;
				int col = token.IndexOf(':');

				if (col != -1)
				{
					@namespace = lookupNamespace(token.Substring(0,col));
					if (!string.ReferenceEquals(@namespace, null))
					{
						elements.Append(@namespace + ":" + token.Substring(col + 1, token.Length - (col + 1)));
					}
					else
					{
						elements.Append(token);
					}
				}
				else
				{
					elements.Append(token);
				}

				if (list.hasMoreElements())
				{
					elements.Append(" ");
				}
			}
			_elementList = elements.ToString();
		}


		/// <summary>
		/// De-tokenize the elements listed in the 'elements' attribute and
		/// instanciate a set of strip/preserve rules.
		/// </summary>
		public ArrayList Rules
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.Vector rules = new java.util.Vector();
			ArrayList rules = new ArrayList();
			// Go through each element and instanciate strip/preserve-object
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.StringTokenizer list = new java.util.StringTokenizer(_elementList);
			StringTokenizer list = new StringTokenizer(_elementList);
			while (list.hasMoreElements())
			{
				rules.Add(new WhitespaceRule(_action, list.nextToken(), _importPrecedence));
			}
			return rules;
			}
		}


		/// <summary>
		/// Scans through the rules vector and looks for a rule of higher
		/// priority that contradicts the current rule.
		/// </summary>
		private static WhitespaceRule findContradictingRule(ArrayList rules, WhitespaceRule rule)
		{
		for (int i = 0; i < rules.Count; i++)
		{
			// Get the next rule in the prioritized list
			WhitespaceRule currentRule = (WhitespaceRule)rules[i];
			// We only consider rules with higher priority
			if (currentRule == rule)
			{
			return null;
			}

			/*
			 * See if there is a contradicting rule with higher priority.
			 * If the rules has the same action then this rule is redundant,
			 * if they have different action then this rule will never win.
			 */
			switch (currentRule.Strength)
			{
			case RULE_ALL:
			return currentRule;

			case RULE_ELEMENT:
			if (!rule.Element.Equals(currentRule.Element))
			{
				break;
			}
			// intentional fall-through
			case RULE_NAMESPACE:
			if (rule.Namespace.Equals(currentRule.Namespace))
			{
				return currentRule;
			}
			break;
			}
		}
		return null;
		}


		/// <summary>
		/// Orders a set or rules by priority, removes redundant rules and rules
		/// that are shadowed by stronger, contradicting rules.
		/// </summary>
		private static int prioritizeRules(ArrayList rules)
		{
		WhitespaceRule currentRule;
		int defaultAction = PRESERVE_SPACE;

		// Sort all rules with regard to priority
		quicksort(rules, 0, rules.Count - 1);

		// Check if there are any "xsl:strip-space" elements at all.
		// If there are no xsl:strip elements we can ignore all xsl:preserve
		// elements and signal that all whitespaces should be preserved
		bool strip = false;
		for (int i = 0; i < rules.Count; i++)
		{
			currentRule = (WhitespaceRule)rules[i];
			if (currentRule.Action == Constants_Fields.STRIP_SPACE)
			{
			strip = true;
			}
		}
		// Return with default action: PRESERVE_SPACE
		if (!strip)
		{
			rules.Clear();
			return PRESERVE_SPACE;
		}

		// Remove all rules that are contradicted by rules with higher priority
		for (int idx = 0; idx < rules.Count;)
		{
			currentRule = (WhitespaceRule)rules[idx];

			// Remove this single rule if it has no purpose
			if (findContradictingRule(rules,currentRule) != null)
			{
			rules.RemoveAt(idx);
			}
			else
			{
			// Remove all following rules if this one overrides all
			if (currentRule.Strength == RULE_ALL)
			{
				defaultAction = currentRule.Action;
				for (int i = idx; i < rules.Count; i++)
				{
				rules.RemoveAt(i);
				}
			}
			// Skip to next rule (there might not be any)...
			idx++;
			}
		}

		// The rules vector could be empty if first rule has strength RULE_ALL
		if (rules.Count == 0)
		{
			return defaultAction;
		}

		// Now work backwards and strip away all rules that have the same
		// action as the default rule (no reason the check them at the end).
		do
		{
			currentRule = (WhitespaceRule)rules[rules.Count - 1];
			if (currentRule.Action == defaultAction)
			{
			rules.RemoveAt(rules.Count - 1);
			}
			else
			{
			break;
			}
		} while (rules.Count > 0);

		// Signal that whitespace detection predicate must be used.
		return defaultAction;
		}

		public static void compileStripSpace(BranchHandle[] strip, int sCount, InstructionList il)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle target = il.append(ICONST_1);
		InstructionHandle target = il.append(ICONST_1);
		il.append(IRETURN);
		for (int i = 0; i < sCount; i++)
		{
			strip[i].Target = target;
		}
		}

		public static void compilePreserveSpace(BranchHandle[] preserve, int pCount, InstructionList il)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle target = il.append(ICONST_0);
		InstructionHandle target = il.append(ICONST_0);
		il.append(IRETURN);
		for (int i = 0; i < pCount; i++)
		{
			preserve[i].Target = target;
		}
		}

		/*
		private static void compileDebug(ClassGenerator classGen,
						 InstructionList il) {
		final ConstantPoolGen cpg = classGen.getConstantPool();
		final int prt = cpg.addMethodref("java/lang/System/out",
						 "println",
						 "(Ljava/lang/String;)V");
		il.append(DUP);
		il.append(new INVOKESTATIC(prt));
		}
		*/

		/// <summary>
		/// Compiles the predicate method
		/// </summary>
		private static void compilePredicate(ArrayList rules, int defaultAction, ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = classGen.getParser().getXSLTC();
		XSLTC xsltc = classGen.Parser.XSLTC;

		// private boolean Translet.stripSpace(int type) - cannot be static
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator stripSpace = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] { org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT }, new String[] { "dom","node","type" }, "stripSpace",classGen.getClassName(),il,cpg);
		MethodGenerator stripSpace = new MethodGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] {Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT}, new string[] {"dom","node","type"}, "stripSpace",classGen.ClassName,il,cpg);

		classGen.addInterface("org/apache/xalan/xsltc/StripFilter");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int paramDom = stripSpace.getLocalIndex("dom");
		int paramDom = stripSpace.getLocalIndex("dom");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int paramCurrent = stripSpace.getLocalIndex("node");
		int paramCurrent = stripSpace.getLocalIndex("node");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int paramType = stripSpace.getLocalIndex("type");
		int paramType = stripSpace.getLocalIndex("type");

		BranchHandle[] strip = new BranchHandle[rules.Count];
		BranchHandle[] preserve = new BranchHandle[rules.Count];
		int sCount = 0;
		int pCount = 0;

		// Traverse all strip/preserve rules
		for (int i = 0; i < rules.Count; i++)
		{
			// Get the next rule in the prioritised list
			WhitespaceRule rule = (WhitespaceRule)rules[i];

			// Returns the namespace for a node in the DOM
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int gns = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getNamespaceName", "(I)Ljava/lang/String;");
			int gns = cpg.addInterfaceMethodref(Constants_Fields.DOM_INTF, "getNamespaceName", "(I)Ljava/lang/String;");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int strcmp = cpg.addMethodref("java/lang/String", "compareTo", "(Ljava/lang/String;)I");
			int strcmp = cpg.addMethodref("java/lang/String", "compareTo", "(Ljava/lang/String;)I");

			// Handle elements="ns:*" type rule
			if (rule.Strength == RULE_NAMESPACE)
			{
			il.append(new ALOAD(paramDom));
			il.append(new ILOAD(paramCurrent));
			il.append(new INVOKEINTERFACE(gns,2));
			il.append(new PUSH(cpg, rule.Namespace));
			il.append(new INVOKEVIRTUAL(strcmp));
			il.append(ICONST_0);

			if (rule.Action == Constants_Fields.STRIP_SPACE)
			{
				strip[sCount++] = il.append(new IF_ICMPEQ(null));
			}
			else
			{
				preserve[pCount++] = il.append(new IF_ICMPEQ(null));
			}
			}
			// Handle elements="ns:el" type rule
			else if (rule.Strength == RULE_ELEMENT)
			{
			// Create the QName for the element
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parser parser = classGen.getParser();
			Parser parser = classGen.Parser;
			QName qname;
			if (!string.ReferenceEquals(rule.Namespace, Constants_Fields.EMPTYSTRING))
			{
				qname = parser.getQName(rule.Namespace, null, rule.Element);
			}
			else
			{
				qname = parser.getQName(rule.Element);
			}

			// Register the element.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int elementType = xsltc.registerElement(qname);
			int elementType = xsltc.registerElement(qname);
			il.append(new ILOAD(paramType));
			il.append(new PUSH(cpg, elementType));

			// Compare current node type with wanted element type
			if (rule.Action == Constants_Fields.STRIP_SPACE)
			{
				strip[sCount++] = il.append(new IF_ICMPEQ(null));
			}
			else
			{
				preserve[pCount++] = il.append(new IF_ICMPEQ(null));
			}
			}
		}

		if (defaultAction == Constants_Fields.STRIP_SPACE)
		{
			compileStripSpace(strip, sCount, il);
			compilePreserveSpace(preserve, pCount, il);
		}
		else
		{
			compilePreserveSpace(preserve, pCount, il);
			compileStripSpace(strip, sCount, il);
		}

		classGen.addMethod(stripSpace);
		}

		/// <summary>
		/// Compiles the predicate method
		/// </summary>
		private static void compileDefault(int defaultAction, ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XSLTC xsltc = classGen.getParser().getXSLTC();
		XSLTC xsltc = classGen.Parser.XSLTC;

		// private boolean Translet.stripSpace(int type) - cannot be static
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator stripSpace = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] { org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT }, new String[] { "dom","node","type" }, "stripSpace",classGen.getClassName(),il,cpg);
		MethodGenerator stripSpace = new MethodGenerator(Constants_Fields.ACC_PUBLIC | Constants_Fields.ACC_FINAL, org.apache.bcel.generic.Type.BOOLEAN, new org.apache.bcel.generic.Type[] {Util.getJCRefType(Constants_Fields.DOM_INTF_SIG), org.apache.bcel.generic.Type.INT, org.apache.bcel.generic.Type.INT}, new string[] {"dom","node","type"}, "stripSpace",classGen.ClassName,il,cpg);

		classGen.addInterface("org/apache/xalan/xsltc/StripFilter");

		if (defaultAction == Constants_Fields.STRIP_SPACE)
		{
			il.append(ICONST_1);
		}
		else
		{
			il.append(ICONST_0);
		}
		il.append(IRETURN);

		classGen.addMethod(stripSpace);
		}


		/// <summary>
		/// Takes a vector of WhitespaceRule objects and generates a predicate
		/// method. This method returns the translets default action for handling
		/// whitespace text-nodes:
		///    - USE_PREDICATE  (run the method generated by this method)
		///    - STRIP_SPACE    (always strip whitespace text-nodes)
		///    - PRESERVE_SPACE (always preserve whitespace text-nodes)
		/// </summary>
		public static int translateRules(ArrayList rules, ClassGenerator classGen)
		{
		// Get the core rules in prioritized order
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int defaultAction = prioritizeRules(rules);
		int defaultAction = prioritizeRules(rules);
		// The rules vector may be empty after prioritising
		if (rules.Count == 0)
		{
			compileDefault(defaultAction,classGen);
			return defaultAction;
		}
		// Now - create a predicate method and sequence through rules...
		compilePredicate(rules, defaultAction, classGen);
		// Return with the translets required action (
		return USE_PREDICATE;
		}

		/// <summary>
		/// Sorts a range of rules with regard to PRIORITY only
		/// </summary>
		private static void quicksort(ArrayList rules, int p, int r)
		{
		while (p < r)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int q = partition(rules, p, r);
			int q = partition(rules, p, r);
			quicksort(rules, p, q);
			p = q + 1;
		}
		}

		/// <summary>
		/// Used with quicksort method above
		/// </summary>
		private static int partition(ArrayList rules, int p, int r)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WhitespaceRule x = (WhitespaceRule)rules.elementAt((p+r) >>> 1);
		WhitespaceRule x = (WhitespaceRule)rules[(int)((uint)(p + r) >> 1)];
		int i = p - 1, j = r + 1;
		while (true)
		{
			while (x.compareTo((WhitespaceRule)rules[--j]) < 0)
			{
			}
			while (x.compareTo((WhitespaceRule)rules[++i]) > 0)
			{
			}
			if (i < j)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WhitespaceRule tmp = (WhitespaceRule)rules.elementAt(i);
			WhitespaceRule tmp = (WhitespaceRule)rules[i];
			rules[i] = rules[j];
			rules[j] = tmp;
			}
			else
			{
			return j;
			}
		}
		}

		/// <summary>
		/// Type-check contents/attributes - nothing to do...
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return Type.Void; // We don't return anything.
		}

		/// <summary>
		/// This method should not produce any code
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		}
	}

}