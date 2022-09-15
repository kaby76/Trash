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
 * $Id: LiteralElement.java 669372 2008-06-19 03:39:52Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	using ElemDesc = org.apache.xml.serializer.ElemDesc;
	using ToHTMLStream = org.apache.xml.serializer.ToHTMLStream;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class LiteralElement : Instruction
	{

		private string _name;
		private LiteralElement _literalElemParent = null;
		private ArrayList _attributeElements = null;
		private Hashtable _accessedPrefixes = null;

		// True if all attributes of this LRE are unique, i.e. they all have
		// different names. This flag is set to false if some attribute
		// names are not known at compile time.
		private bool _allAttributesUnique = false;

		private const string XMLNS_STRING = "xmlns";

		/// <summary>
		/// Returns the QName for this literal element
		/// </summary>
		public QName Name
		{
			get
			{
			return _qname;
			}
		}

		/// <summary>
		/// Displays the contents of this literal element
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("LiteralElement name = " + _name);
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Returns the namespace URI for which a prefix is pointing to
		/// </summary>
		private string accessedNamespace(string prefix)
		{
			if (_literalElemParent != null)
			{
				string result = _literalElemParent.accessedNamespace(prefix);
				if (!string.ReferenceEquals(result, null))
				{
					return result;
				}
			}
			return _accessedPrefixes != null ? (string) _accessedPrefixes[prefix] : null;
		}

		/// <summary>
		/// Method used to keep track of what namespaces that are references by
		/// this literal element and its attributes. The output must contain a
		/// definition for each namespace, so we stuff them in a hashtable.
		/// </summary>
		public void registerNamespace(string prefix, string uri, SymbolTable stable, bool declared)
		{

		// Check if the parent has a declaration for this namespace
		if (_literalElemParent != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String parentUri = _literalElemParent.accessedNamespace(prefix);
			string parentUri = _literalElemParent.accessedNamespace(prefix);
			if (!string.ReferenceEquals(parentUri, null) && parentUri.Equals(uri))
			{
					return;
			}
		}

		// Check if we have any declared namesaces
		if (_accessedPrefixes == null)
		{
			_accessedPrefixes = new Hashtable();
		}
		else
		{
			if (!declared)
			{
			// Check if this node has a declaration for this namespace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String old = (String)_accessedPrefixes.get(prefix);
			string old = (string)_accessedPrefixes[prefix];
			if (!string.ReferenceEquals(old, null))
			{
				if (old.Equals(uri))
				{
				return;
				}
				else
				{
				prefix = stable.generateNamespacePrefix();
				}
			}
			}
		}

		if (!prefix.Equals("xml"))
		{
			_accessedPrefixes[prefix] = uri;
		}
		}

		/// <summary>
		/// Translates the prefix of a QName according to the rules set in
		/// the attributes of xsl:stylesheet. Also registers a QName to assure
		/// that the output element contains the necessary namespace declarations.
		/// </summary>
		private string translateQName(QName qname, SymbolTable stable)
		{
		// Break up the QName and get prefix:localname strings
		string localname = qname.LocalPart;
		string prefix = qname.Prefix;

		// Treat default namespace as "" and not null
		if (string.ReferenceEquals(prefix, null))
		{
			prefix = Constants.EMPTYSTRING;
		}
		else if (prefix.Equals(XMLNS_STRING))
		{
			return (XMLNS_STRING);
		}

		// Check if we must translate the prefix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String alternative = stable.lookupPrefixAlias(prefix);
		string alternative = stable.lookupPrefixAlias(prefix);
		if (!string.ReferenceEquals(alternative, null))
		{
			stable.excludeNamespaces(prefix);
			prefix = alternative;
		}

		// Get the namespace this prefix refers to
		string uri = lookupNamespace(prefix);
		if (string.ReferenceEquals(uri, null))
		{
			return (localname);
		}

		// Register the namespace as accessed
		registerNamespace(prefix, uri, stable, false);

		// Construct the new name for the element (may be unchanged)
		if (!string.ReferenceEquals(prefix, Constants.EMPTYSTRING))
		{
			return (prefix + ":" + localname);
		}
		else
		{
			return (localname);
		}
		}

		/// <summary>
		/// Add an attribute to this element
		/// </summary>
		public void addAttribute(SyntaxTreeNode attribute)
		{
		if (_attributeElements == null)
		{
			_attributeElements = new ArrayList(2);
		}
		_attributeElements.Add(attribute);
		}

		/// <summary>
		/// Set the first attribute of this element
		/// </summary>
		public SyntaxTreeNode FirstAttribute
		{
			set
			{
			if (_attributeElements == null)
			{
				_attributeElements = new ArrayList(2);
			}
			_attributeElements.Insert(0, value);
			}
		}

		/// <summary>
		/// Type-check the contents of this element. The element itself does not
		/// need any type checking as it leaves nothign on the JVM's stack.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		// Type-check all attributes
		if (_attributeElements != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _attributeElements.size();
			int count = _attributeElements.Count;
			for (int i = 0; i < count; i++)
			{
			SyntaxTreeNode node = (SyntaxTreeNode)_attributeElements[i];
			node.typeCheck(stable);
			}
		}
		typeCheckContents(stable);
		return Type.Void;
		}

		/// <summary>
		/// This method starts at a given node, traverses all namespace mappings,
		/// and assembles a list of all prefixes that (for the given node) maps
		/// to _ANY_ namespace URI. Used by literal result elements to determine
		/// </summary>
		public System.Collections.IEnumerator getNamespaceScope(SyntaxTreeNode node)
		{
		Hashtable all = new Hashtable();

		while (node != null)
		{
			Hashtable mapping = node.PrefixMapping;
			if (mapping != null)
			{
			System.Collections.IEnumerator prefixes = mapping.Keys.GetEnumerator();
			while (prefixes.MoveNext())
			{
				string prefix = (string)prefixes.Current;
				if (!all.ContainsKey(prefix))
				{
				all[prefix] = mapping[prefix];
				}
			}
			}
			node = node.Parent;
		}
		return (all.Keys.GetEnumerator());
		}

		/// <summary>
		/// Determines the final QName for the element and its attributes.
		/// Registers all namespaces that are used by the element/attributes
		/// </summary>
		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
		SymbolTable stable = parser.SymbolTable;
		stable.CurrentNode = this;

		// Check if in a literal element context
		SyntaxTreeNode parent = Parent;
			if (parent != null && parent is LiteralElement)
			{
				_literalElemParent = (LiteralElement) parent;
			}

		_name = translateQName(_qname, stable);

		// Process all attributes and register all namespaces they use
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _attributes.getLength();
		int count = _attributes.Length;
		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName qname = parser.getQName(_attributes.getQName(i));
			QName qname = parser.getQName(_attributes.getQName(i));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = qname.getNamespace();
			string uri = qname.Namespace;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String val = _attributes.getValue(i);
			string val = _attributes.getValue(i);

			// Handle xsl:use-attribute-sets. Attribute sets are placed first
			// in the vector or attributes to make sure that later local
			// attributes can override an attributes in the set.
			if (qname.Equals(parser.UseAttributeSets))
			{
					if (!Util.isValidQNames(val))
					{
						ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, val, this);
						parser.reportError(Constants.ERROR, err);
					}
			FirstAttribute = new UseAttributeSets(val, parser);
			}
			// Handle xsl:extension-element-prefixes
			else if (qname.Equals(parser.ExtensionElementPrefixes))
			{
			stable.excludeNamespaces(val);
			}
			// Handle xsl:exclude-result-prefixes
			else if (qname.Equals(parser.ExcludeResultPrefixes))
			{
			stable.excludeNamespaces(val);
			}
			else
			{
			// Ignore special attributes (e.g. xmlns:prefix and xmlns)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = qname.getPrefix();
			string prefix = qname.Prefix;
			if (!string.ReferenceEquals(prefix, null) && prefix.Equals(XMLNS_PREFIX) || string.ReferenceEquals(prefix, null) && qname.LocalPart.Equals("xmlns") || !string.ReferenceEquals(uri, null) && uri.Equals(XSLT_URI))
			{
				continue;
			}

			// Handle all other literal attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = translateQName(qname, stable);
			string name = translateQName(qname, stable);
			LiteralAttribute attr = new LiteralAttribute(name, val, parser, this);
			addAttribute(attr);
			attr.Parent = this;
			attr.parseContents(parser);
			}
		}

		// Register all namespaces that are in scope, except for those that
		// are listed in the xsl:stylesheet element's *-prefixes attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration include = getNamespaceScope(this);
		System.Collections.IEnumerator include = getNamespaceScope(this);
		while (include.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = (String)include.Current;
			string prefix = (string)include.Current;
			if (!prefix.Equals("xml"))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = lookupNamespace(prefix);
			string uri = lookupNamespace(prefix);
			if (!string.ReferenceEquals(uri, null) && !stable.isExcludedNamespace(uri))
			{
				registerNamespace(prefix, uri, stable, true);
			}
			}
		}

		parseChildren(parser);

		// Process all attributes and register all namespaces they use
		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName qname = parser.getQName(_attributes.getQName(i));
			QName qname = parser.getQName(_attributes.getQName(i));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String val = _attributes.getValue(i);
			string val = _attributes.getValue(i);

			// Handle xsl:extension-element-prefixes
			if (qname.Equals(parser.ExtensionElementPrefixes))
			{
			stable.unExcludeNamespaces(val);
			}
			// Handle xsl:exclude-result-prefixes
			else if (qname.Equals(parser.ExcludeResultPrefixes))
			{
			stable.unExcludeNamespaces(val);
			}
		}
		}

		protected internal override bool contextDependent()
		{
		return dependentContents();
		}

		/// <summary>
		/// Compiles code that emits the literal element to the output handler,
		/// first the start tag, then namespace declaration, then attributes,
		/// then the element contents, and then the element end tag. Since the
		/// value of an attribute may depend on a variable, variables must be
		/// compiled first.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

			// Check whether all attributes are unique.
			_allAttributesUnique = checkAttributesUnique();

		// Compile code to emit element start tag
		il.append(methodGen.loadHandler());

		il.append(new PUSH(cpg, _name));
		il.append(DUP2); // duplicate these 2 args for endElement
		il.append(methodGen.startElement());

		// The value of an attribute may depend on a (sibling) variable
			int j = 0;
			while (j < elementCount())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode item = (SyntaxTreeNode) elementAt(j);
				SyntaxTreeNode item = (SyntaxTreeNode) elementAt(j);
				if (item is Variable)
				{
					item.translate(classGen, methodGen);
				}
				j++;
			}

		// Compile code to emit namespace attributes
		if (_accessedPrefixes != null)
		{
			bool declaresDefaultNS = false;
			System.Collections.IEnumerator e = _accessedPrefixes.Keys.GetEnumerator();

			while (e.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = (String)e.Current;
			string prefix = (string)e.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = (String)_accessedPrefixes.get(prefix);
			string uri = (string)_accessedPrefixes[prefix];

			if (!string.ReferenceEquals(uri, Constants.EMPTYSTRING) || !string.ReferenceEquals(prefix, Constants.EMPTYSTRING))
			{
				if (string.ReferenceEquals(prefix, Constants.EMPTYSTRING))
				{
				declaresDefaultNS = true;
				}
				il.append(methodGen.loadHandler());
				il.append(new PUSH(cpg,prefix));
				il.append(new PUSH(cpg,uri));
				il.append(methodGen.@namespace());
			}
			}

			/* 
			 * If our XslElement parent redeclares the default NS, and this
			 * element doesn't, it must be redeclared one more time.
			 */
			if (!declaresDefaultNS && (_parent is XslElement) && ((XslElement) _parent).declaresDefaultNS())
			{
			il.append(methodGen.loadHandler());
			il.append(new PUSH(cpg, Constants.EMPTYSTRING));
			il.append(new PUSH(cpg, Constants.EMPTYSTRING));
			il.append(methodGen.@namespace());
			}
		}

		// Output all attributes
		if (_attributeElements != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _attributeElements.size();
			int count = _attributeElements.Count;
			for (int i = 0; i < count; i++)
			{
			SyntaxTreeNode node = (SyntaxTreeNode)_attributeElements[i];
			if (!(node is XslAttribute))
			{
				node.translate(classGen, methodGen);
			}
			}
		}

		// Compile code to emit attributes and child elements
		translateContents(classGen, methodGen);

		// Compile code to emit element end tag
		il.append(methodGen.endElement());
		}

		/// <summary>
		/// Return true if the output method is html.
		/// </summary>
		private bool HTMLOutput
		{
			get
			{
				return Stylesheet.OutputMethod == Stylesheet.HTML_OUTPUT;
			}
		}

		/// <summary>
		/// Return the ElemDesc object for an HTML element.
		/// Return null if the output method is not HTML or this is not a 
		/// valid HTML element.
		/// </summary>
		public ElemDesc ElemDesc
		{
			get
			{
				if (HTMLOutput)
				{
					return ToHTMLStream.getElemDesc(_name);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Return true if all attributes of this LRE have unique names.
		/// </summary>
		public bool allAttributesUnique()
		{
			return _allAttributesUnique;
		}

		/// <summary>
		/// Check whether all attributes are unique.
		/// </summary>
		private bool checkAttributesUnique()
		{
			 bool hasHiddenXslAttribute = canProduceAttributeNodes(this, true);
			 if (hasHiddenXslAttribute)
			 {
				 return false;
			 }

			 if (_attributeElements != null)
			 {
				 int numAttrs = _attributeElements.Count;
				 Hashtable attrsTable = null;
				 for (int i = 0; i < numAttrs; i++)
				 {
					 SyntaxTreeNode node = (SyntaxTreeNode)_attributeElements[i];

					 if (node is UseAttributeSets)
					 {
						 return false;
					 }
					 else if (node is XslAttribute)
					 {
						 if (attrsTable == null)
						 {
							 attrsTable = new Hashtable();
							 for (int k = 0; k < i; k++)
							 {
								 SyntaxTreeNode n = (SyntaxTreeNode)_attributeElements[k];
								 if (n is LiteralAttribute)
								 {
									 LiteralAttribute literalAttr = (LiteralAttribute)n;
									 attrsTable[literalAttr.Name] = literalAttr;
								 }
							 }
						 }

						 XslAttribute xslAttr = (XslAttribute)node;
						 AttributeValue attrName = xslAttr.Name;
						 if (attrName is AttributeValueTemplate)
						 {
							 return false;
						 }
						 else if (attrName is SimpleAttributeValue)
						 {
							 SimpleAttributeValue simpleAttr = (SimpleAttributeValue)attrName;
							 string name = simpleAttr.ToString();
							 if (!string.ReferenceEquals(name, null) && attrsTable[name] != null)
							 {
								 return false;
							 }
							 else if (!string.ReferenceEquals(name, null))
							 {
								 attrsTable[name] = xslAttr;
							 }
						 }
					 }
				 }
			 }
			 return true;
		}

		/// <summary>
		/// Return true if the instructions under the given SyntaxTreeNode can produce attribute nodes
		/// to an element. Only return false when we are sure that no attribute node is produced. 
		/// Return true if we are not sure. If the flag ignoreXslAttribute is true, the direct 
		/// <xsl:attribute> children of the current node are not included in the check.
		/// </summary>
		private bool canProduceAttributeNodes(SyntaxTreeNode node, bool ignoreXslAttribute)
		{
			ArrayList contents = node.Contents;
			int size = contents.Count;
			for (int i = 0; i < size; i++)
			{
				SyntaxTreeNode child = (SyntaxTreeNode)contents[i];
				if (child is Text)
				{
					Text text = (Text)child;
					if (text.Ignore)
					{
						continue;
					}
					else
					{
						return false;
					}
				}
				// Cannot add an attribute to an element after children have been added to it.
				// We can safely return false when the instruction can produce an output node.
			   else if (child is LiteralElement || child is ValueOf || child is XslElement || child is Comment || child is Number || child is ProcessingInstruction)
			   {
					return false;
			   }
				else if (child is XslAttribute)
				{
					if (ignoreXslAttribute)
					{
						continue;
					}
					else
					{
						return true;
					}
				}
				// In general, there is no way to check whether <xsl:call-template> or 
				// <xsl:apply-templates> can produce attribute nodes. <xsl:copy> and
				// <xsl:copy-of> can also copy attribute nodes to an element. Return
				// true in those cases to be safe.
				else if (child is CallTemplate || child is ApplyTemplates || child is Copy || child is CopyOf)
				{
					return true;
				}
				else if ((child is If || child is ForEach) && canProduceAttributeNodes(child, false))
				{
					 return true;
				}
				else if (child is Choose)
				{
					ArrayList chooseContents = child.Contents;
					int num = chooseContents.Count;
					for (int k = 0; k < num; k++)
					{
						SyntaxTreeNode chooseChild = (SyntaxTreeNode)chooseContents[k];
						if (chooseChild is When || chooseChild is Otherwise)
						{
							if (canProduceAttributeNodes(chooseChild, false))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

	}

}