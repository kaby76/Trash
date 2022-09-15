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
 * $Id: SyntaxTreeNode.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ANEWARRAY = org.apache.bcel.generic.ANEWARRAY;
	using BasicType = org.apache.bcel.generic.BasicType;
	using CHECKCAST = org.apache.bcel.generic.CHECKCAST;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using DUP_X1 = org.apache.bcel.generic.DUP_X1;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using ICONST = org.apache.bcel.generic.ICONST;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using NEW = org.apache.bcel.generic.NEW;
	using NEWARRAY = org.apache.bcel.generic.NEWARRAY;
	using PUSH = org.apache.bcel.generic.PUSH;
	using DOM = org.apache.xalan.xsltc.DOM;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using AttributeList = org.apache.xalan.xsltc.runtime.AttributeList;
	using Attributes = org.xml.sax.Attributes;



	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller
	/// @author Morten Jorensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author John Howard <JohnH@schemasoft.com>
	/// </summary>
	public abstract class SyntaxTreeNode : Constants
	{

		// Reference to the AST parser
		private Parser _parser;

		// AST navigation pointers
		protected internal SyntaxTreeNode _parent; // Parent node
		private Stylesheet _stylesheet; // Stylesheet ancestor node
		private Template _template; // Template ancestor node
		private readonly ArrayList _contents = new ArrayList(2); // Child nodes

		// Element description data
		protected internal QName _qname; // The element QName
		private int _line; // Source file line number
		protected internal AttributeList _attributes = null; // Attributes of this element
		private Hashtable _prefixMapping = null; // Namespace declarations

		public const int UNKNOWN_STYLESHEET_NODE_ID = -1;

		// Records whether this node or any descendant needs to know the
		// in-scope namespaces at transform-time
		private int _nodeIDForStylesheetNSLookup = UNKNOWN_STYLESHEET_NODE_ID;

		// Sentinel - used to denote unrecognised syntaxt tree nodes.
		internal static readonly SyntaxTreeNode Dummy = new AbsolutePathPattern(null);

		// These two are used for indenting nodes in the AST (debug output)
		protected internal const int IndentIncrement = 4;
		private static readonly char[] _spaces = "                                                       ".ToCharArray();

		/// <summary>
		/// Creates a new SyntaxTreeNode with a 'null' QName and no source file
		/// line number reference.
		/// </summary>
		public SyntaxTreeNode()
		{
		_line = 0;
		_qname = null;
		}

		/// <summary>
		/// Creates a new SyntaxTreeNode with a 'null' QName. </summary>
		/// <param name="line"> Source file line number reference </param>
		public SyntaxTreeNode(int line)
		{
		_line = line;
		_qname = null;
		}

		/// <summary>
		/// Creates a new SyntaxTreeNode with no source file line number reference. </summary>
		/// <param name="uri"> The element's namespace URI </param>
		/// <param name="prefix"> The element's namespace prefix </param>
		/// <param name="local"> The element's local name </param>
		public SyntaxTreeNode(string uri, string prefix, string local)
		{
		_line = 0;
		setQName(uri, prefix, local);
		}

		/// <summary>
		/// Set the source file line number for this element </summary>
		/// <param name="line"> The source file line number. </param>
		protected internal int LineNumber
		{
			set
			{
			_line = value;
			}
			get
			{
				if (_line > 0)
				{
					return _line;
				}
				SyntaxTreeNode parent = Parent;
				return (parent != null) ? parent.LineNumber : 0;
			}
		}


		/// <summary>
		/// Set the QName for the syntax tree node. </summary>
		/// <param name="qname"> The QName for the syntax tree node </param>
		protected internal virtual void setQName(QName qname)
		{
		_qname = qname;
		}

		/// <summary>
		/// Set the QName for the SyntaxTreeNode </summary>
		/// <param name="uri"> The element's namespace URI </param>
		/// <param name="prefix"> The element's namespace prefix </param>
		/// <param name="local"> The element's local name </param>
		protected internal virtual void setQName(string uri, string prefix, string localname)
		{
		_qname = new QName(uri, prefix, localname);
		}

		/// <summary>
		/// Set the QName for the SyntaxTreeNode </summary>
		/// <param name="qname"> The QName for the syntax tree node </param>
		protected internal virtual QName QName
		{
			get
			{
			return (_qname);
			}
		}

		/// <summary>
		/// Set the attributes for this SyntaxTreeNode. </summary>
		/// <param name="attributes"> Attributes for the element. Must be passed in as an
		///                   implementation of org.xml.sax.Attributes. </param>
		protected internal virtual AttributeList Attributes
		{
			set
			{
			_attributes = value;
			}
			get
			{
			return (_attributes);
			}
		}

		/// <summary>
		/// Returns a value for an attribute from the source element. </summary>
		/// <param name="qname"> The QName of the attribute to return. </param>
		/// <returns> The value of the attribute of name 'qname'. </returns>
		protected internal virtual string getAttribute(string qname)
		{
		if (_attributes == null)
		{
			return EMPTYSTRING;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = _attributes.getValue(qname);
		string value = _attributes.getValue(qname);
		return (string.ReferenceEquals(value, null) || value.Equals(EMPTYSTRING)) ? EMPTYSTRING : value;
		}

		protected internal virtual string getAttribute(string prefix, string localName)
		{
			return getAttribute(prefix + ':' + localName);
		}

		protected internal virtual bool hasAttribute(string qname)
		{
		return (_attributes != null && !string.ReferenceEquals(_attributes.getValue(qname), null));
		}

		protected internal virtual void addAttribute(string qname, string value)
		{
			_attributes.add(qname, value);
		}


		/// <summary>
		/// Sets the prefix mapping for the namespaces that were declared in this
		/// element. This does not include all prefix mappings in scope, so one
		/// may have to check ancestor elements to get all mappings that are in
		/// in scope. The prefixes must be passed in as a Hashtable that maps
		/// namespace prefixes (String objects) to namespace URIs (also String). </summary>
		/// <param name="mapping"> The Hashtable containing the mappings. </param>
		protected internal virtual Hashtable PrefixMapping
		{
			set
			{
			_prefixMapping = value;
			}
			get
			{
			return _prefixMapping;
			}
		}


		/// <summary>
		/// Adds a single prefix mapping to this syntax tree node. </summary>
		/// <param name="prefix"> Namespace prefix. </param>
		/// <param name="uri"> Namespace URI. </param>
		protected internal virtual void addPrefixMapping(string prefix, string uri)
		{
		if (_prefixMapping == null)
		{
			_prefixMapping = new Hashtable();
		}
		_prefixMapping[prefix] = uri;
		}

		/// <summary>
		/// Returns any namespace URI that is in scope for a given prefix. This
		/// method checks namespace mappings for this element, and if necessary
		/// for ancestor elements as well (ie. if the prefix maps to an URI in this
		/// scope then you'll definately get the URI from this method). </summary>
		/// <param name="prefix"> Namespace prefix. </param>
		/// <returns> Namespace URI. </returns>
		protected internal virtual string lookupNamespace(string prefix)
		{
		// Initialise the output (default is 'null' for undefined)
		string uri = null;

		// First look up the prefix/uri mapping in our own hashtable...
		if (_prefixMapping != null)
		{
			uri = (string)_prefixMapping[prefix];
		}
		// ... but if we can't find it there we ask our parent for the mapping
		if ((string.ReferenceEquals(uri, null)) && (_parent != null))
		{
			uri = _parent.lookupNamespace(prefix);
			if ((string.ReferenceEquals(prefix, Constants.EMPTYSTRING)) && (string.ReferenceEquals(uri, null)))
			{
			uri = Constants.EMPTYSTRING;
			}
		}
		// ... and then we return whatever URI we've got.
		return (uri);
		}

		/// <summary>
		/// Returns any namespace prefix that is mapped to a prefix in the current
		/// scope. This method checks namespace mappings for this element, and if
		/// necessary for ancestor elements as well (ie. if the URI is declared
		/// within the current scope then you'll definately get the prefix from
		/// this method). Note that this is a very slow method and consequentially
		/// it should only be used strictly when needed. </summary>
		/// <param name="uri"> Namespace URI. </param>
		/// <returns> Namespace prefix. </returns>
		protected internal virtual string lookupPrefix(string uri)
		{
		// Initialise the output (default is 'null' for undefined)
		string prefix = null;

		// First look up the prefix/uri mapping in our own hashtable...
		if ((_prefixMapping != null) && (_prefixMapping.ContainsValue(uri)))
		{
			System.Collections.IEnumerator prefixes = _prefixMapping.Keys.GetEnumerator();
			while (prefixes.MoveNext())
			{
			prefix = (string)prefixes.Current;
			string mapsTo = (string)_prefixMapping[prefix];
			if (mapsTo.Equals(uri))
			{
				return (prefix);
			}
			}
		}
		// ... but if we can't find it there we ask our parent for the mapping
		else if (_parent != null)
		{
			prefix = _parent.lookupPrefix(uri);
			if ((string.ReferenceEquals(uri, Constants.EMPTYSTRING)) && (string.ReferenceEquals(prefix, null)))
			{
			prefix = Constants.EMPTYSTRING;
			}
		}
		return (prefix);
		}

		/// <summary>
		/// Set this node's parser. The parser (the XSLT parser) gives this
		/// syntax tree node access to the symbol table and XPath parser. </summary>
		/// <param name="parser"> The XSLT parser. </param>
		protected internal virtual Parser Parser
		{
			set
			{
			_parser = value;
			}
			get
			{
			return _parser;
			}
		}


		/// <summary>
		/// Set this syntax tree node's parent node </summary>
		/// <param name="parent"> The parent node. </param>
		protected internal virtual SyntaxTreeNode Parent
		{
			set
			{
			if (_parent == null)
			{
				_parent = value;
			}
			}
			get
			{
			return _parent;
			}
		}


		/// <summary>
		/// Returns 'true' if this syntax tree node is the Sentinal node. </summary>
		/// <returns> 'true' if this syntax tree node is the Sentinal node. </returns>
		protected internal bool Dummy
		{
			get
			{
				return this == Dummy;
			}
		}

		/// <summary>
		/// Get the import precedence of this element. The import precedence equals
		/// the import precedence of the stylesheet in which this element occured. </summary>
		/// <returns> The import precedence of this syntax tree node. </returns>
		protected internal virtual int ImportPrecedence
		{
			get
			{
			Stylesheet stylesheet = Stylesheet;
			if (stylesheet == null)
			{
				return int.MinValue;
			}
			return stylesheet.ImportPrecedence;
			}
		}

		/// <summary>
		/// Get the Stylesheet node that represents the <xsl:stylesheet/> element
		/// that this node occured under. </summary>
		/// <returns> The Stylesheet ancestor node of this node. </returns>
		public virtual Stylesheet Stylesheet
		{
			get
			{
			if (_stylesheet == null)
			{
				SyntaxTreeNode parent = this;
				while (parent != null)
				{
				if (parent is Stylesheet)
				{
					return ((Stylesheet)parent);
				}
				parent = parent.Parent;
				}
				_stylesheet = (Stylesheet)parent;
			}
			return (_stylesheet);
			}
		}

		/// <summary>
		/// Get the Template node that represents the <xsl:template/> element
		/// that this node occured under. Note that this method will return 'null'
		/// for nodes that represent top-level elements. </summary>
		/// <returns> The Template ancestor node of this node or 'null'. </returns>
		protected internal virtual Template Template
		{
			get
			{
			if (_template == null)
			{
				SyntaxTreeNode parent = this;
				while ((parent != null) && (!(parent is Template)))
				{
				parent = parent.Parent;
				}
				_template = (Template)parent;
			}
			return (_template);
			}
		}

		/// <summary>
		/// Returns a reference to the XSLTC (XSLT compiler) in use. </summary>
		/// <returns> XSLTC - XSLT compiler. </returns>
		protected internal XSLTC XSLTC
		{
			get
			{
			return _parser.XSLTC;
			}
		}

		/// <summary>
		/// Returns the XSLT parser's symbol table. </summary>
		/// <returns> Symbol table. </returns>
		protected internal SymbolTable SymbolTable
		{
			get
			{
			return (_parser == null) ? null : _parser.SymbolTable;
			}
		}

		/// <summary>
		/// Parse the contents of this syntax tree nodes (child nodes, XPath
		/// expressions, patterns and functions). The default behaviour is to parser
		/// the syntax tree node's children (since there are no common expressions,
		/// patterns, etc. that can be handled in this base class. </summary>
		/// <param name="parser"> reference to the XSLT parser </param>
		public virtual void parseContents(Parser parser)
		{
		parseChildren(parser);
		}

		/// <summary>
		/// Parse all children of this syntax tree node. This method is normally
		/// called by the parseContents() method. </summary>
		/// <param name="parser"> reference to the XSLT parser </param>
		protected internal void parseChildren(Parser parser)
		{

		ArrayList locals = null; // only create when needed

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _contents.size();
		int count = _contents.Count;
		for (int i = 0; i < count; i++)
		{
			SyntaxTreeNode child = (SyntaxTreeNode)_contents[i];
			parser.SymbolTable.CurrentNode = child;
			child.parseContents(parser);
			// if variable or parameter, add it to scope
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName varOrParamName = updateScope(parser, child);
			QName varOrParamName = updateScope(parser, child);
			if (varOrParamName != null)
			{
			if (locals == null)
			{
				locals = new ArrayList(2);
			}
			locals.Add(varOrParamName);
			}
		}

		parser.SymbolTable.CurrentNode = this;

		// after the last element, remove any locals from scope
		if (locals != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nLocals = locals.size();
			int nLocals = locals.Count;
			for (int i = 0; i < nLocals; i++)
			{
			parser.removeVariable((QName)locals[i]);
			}
		}
		}

		/// <summary>
		/// Add a node to the current scope and return name of a variable or
		/// parameter if the node represents a variable or a parameter.
		/// </summary>
		protected internal virtual QName updateScope(Parser parser, SyntaxTreeNode node)
		{
		if (node is Variable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Variable var = (Variable)node;
			Variable var = (Variable)node;
			parser.addVariable(var);
			return var.Name;
		}
		else if (node is Param)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Param param = (Param)node;
			Param param = (Param)node;
			parser.addParameter(param);
			return param.Name;
		}
		else
		{
			return null;
		}
		}

		/// <summary>
		/// Type check the children of this node. The type check phase may add
		/// coercions (CastExpr) to the AST. </summary>
		/// <param name="stable"> The compiler/parser's symbol table </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError;
		public abstract Type typeCheck(SymbolTable stable);

		/// <summary>
		/// Call typeCheck() on all child syntax tree nodes. </summary>
		/// <param name="stable"> The compiler/parser's symbol table </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected org.apache.xalan.xsltc.compiler.util.Type typeCheckContents(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		protected internal virtual Type typeCheckContents(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = elementCount();
		int n = elementCount();
		for (int i = 0; i < n; i++)
		{
			SyntaxTreeNode item = (SyntaxTreeNode)_contents[i];
			item.typeCheck(stable);
		}
		return Type.Void;
		}

		/// <summary>
		/// Translate this abstract syntax tree node into JVM bytecodes. </summary>
		/// <param name="classGen"> BCEL Java class generator </param>
		/// <param name="methodGen"> BCEL Java method generator </param>
		public abstract void translate(ClassGenerator classGen, MethodGenerator methodGen);

		/// <summary>
		/// Call translate() on all child syntax tree nodes. </summary>
		/// <param name="classGen"> BCEL Java class generator </param>
		/// <param name="methodGen"> BCEL Java method generator </param>
		protected internal virtual void translateContents(ClassGenerator classGen, MethodGenerator methodGen)
		{
			// Call translate() on all child nodes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = elementCount();
			int n = elementCount();

			for (int i = 0; i < n; i++)
			{
				methodGen.markChunkStart();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode item = (SyntaxTreeNode)_contents.elementAt(i);
				SyntaxTreeNode item = (SyntaxTreeNode)_contents[i];
				item.translate(classGen, methodGen);
				methodGen.markChunkEnd();
			}

			// After translation, unmap any registers for any variables/parameters
			// that were declared in this scope. Performing this unmapping in the
			// same AST scope as the declaration deals with the problems of
			// references falling out-of-scope inside the for-each element.
			// (the cause of which being 'lazy' register allocation for references)
			for (int i = 0; i < n; i++)
			{
				if (_contents[i] is VariableBase)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = (VariableBase)_contents.elementAt(i);
					VariableBase var = (VariableBase)_contents[i];
					var.unmapRegister(methodGen);
				}
			}
		}

		/// <summary>
		/// Return true if the node represents a simple RTF.
		/// 
		/// A node is a simple RTF if all children only produce Text value.
		/// </summary>
		/// <param name="node"> A node </param>
		/// <returns> true if the node content can be considered as a simple RTF. </returns>
		private bool isSimpleRTF(SyntaxTreeNode node)
		{

			ArrayList contents = node.Contents;
			for (int i = 0; i < contents.Count; i++)
			{
				SyntaxTreeNode item = (SyntaxTreeNode)contents[i];
				if (!isTextElement(item, false))
				{
					return false;
				}
			}

			return true;
		}

		 /// <summary>
		 /// Return true if the node represents an adaptive RTF.
		 /// 
		 /// A node is an adaptive RTF if each children is a Text element
		 /// or it is <xsl:call-template> or <xsl:apply-templates>.
		 /// </summary>
		 /// <param name="node"> A node </param>
		 /// <returns> true if the node content can be considered as an adaptive RTF. </returns>
		private bool isAdaptiveRTF(SyntaxTreeNode node)
		{

			ArrayList contents = node.Contents;
			for (int i = 0; i < contents.Count; i++)
			{
				SyntaxTreeNode item = (SyntaxTreeNode)contents[i];
				if (!isTextElement(item, true))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Return true if the node only produces Text content.
		/// 
		/// A node is a Text element if it is Text, xsl:value-of, xsl:number, 
		/// or a combination of these nested in a control instruction (xsl:if or
		/// xsl:choose).
		/// 
		/// If the doExtendedCheck flag is true, xsl:call-template and xsl:apply-templates
		/// are also considered as Text elements.
		/// </summary>
		/// <param name="node"> A node </param>
		/// <param name="doExtendedCheck"> If this flag is true, <xsl:call-template> and 
		/// <xsl:apply-templates> are also considered as Text elements.
		/// </param>
		/// <returns> true if the node of Text type </returns>
		private bool isTextElement(SyntaxTreeNode node, bool doExtendedCheck)
		{
			if (node is ValueOf || node is Number || node is Text)
			{
				return true;
			}
			else if (node is If)
			{
				return doExtendedCheck ? isAdaptiveRTF(node) : isSimpleRTF(node);
			}
			else if (node is Choose)
			{
				ArrayList contents = node.Contents;
				for (int i = 0; i < contents.Count; i++)
				{
					SyntaxTreeNode item = (SyntaxTreeNode)contents[i];
					if (item is Text || ((item is When || item is Otherwise) && ((doExtendedCheck && isAdaptiveRTF(item)) || (!doExtendedCheck && isSimpleRTF(item)))))
					{
						continue;
					}
					else
					{
						return false;
					}
				}
				return true;
			}
			else if (doExtendedCheck && (node is CallTemplate || node is ApplyTemplates))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Utility method used by parameters and variables to store result trees </summary>
		/// <param name="classGen"> BCEL Java class generator </param>
		/// <param name="methodGen"> BCEL Java method generator </param>
		protected internal virtual void compileResultTree(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stylesheet stylesheet = classGen.getStylesheet();
		Stylesheet stylesheet = classGen.Stylesheet;

		bool isSimple = isSimpleRTF(this);
		bool isAdaptive = false;
		if (!isSimple)
		{
			isAdaptive = isAdaptiveRTF(this);
		}

		int rtfType = isSimple ? DOM.SIMPLE_RTF : (isAdaptive ? DOM.ADAPTIVE_RTF : DOM.TREE_RTF);

		// Save the current handler base on the stack
		il.append(methodGen.loadHandler());

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String DOM_CLASS = classGen.getDOMClass();
		string DOM_CLASS = classGen.DOMClass;

		// Create new instance of DOM class (with RTF_INITIAL_SIZE nodes)
		//int index = cpg.addMethodref(DOM_IMPL, "<init>", "(I)V");
		//il.append(new NEW(cpg.addClass(DOM_IMPL)));

		il.append(methodGen.loadDOM());
		int index = cpg.addInterfaceMethodref(DOM_INTF, "getResultTreeFrag", "(IIZ)" + DOM_INTF_SIG);
		il.append(new PUSH(cpg, RTF_INITIAL_SIZE));
		il.append(new PUSH(cpg, rtfType));
		il.append(new PUSH(cpg, stylesheet.callsNodeset()));
		il.append(new INVOKEINTERFACE(index,4));

		il.append(DUP);

		// Overwrite old handler with DOM handler
		index = cpg.addInterfaceMethodref(DOM_INTF, "getOutputDomBuilder", "()" + TRANSLET_OUTPUT_SIG);

		il.append(new INVOKEINTERFACE(index,1));
		il.append(DUP);
		il.append(methodGen.storeHandler());

		// Call startDocument on the new handler
		il.append(methodGen.startDocument());

		// Instantiate result tree fragment
		translateContents(classGen, methodGen);

		// Call endDocument on the new handler
		il.append(methodGen.loadHandler());
		il.append(methodGen.endDocument());

		// Check if we need to wrap the DOMImpl object in a DOMAdapter object.
		// DOMAdapter is not needed if the RTF is a simple RTF and the nodeset()
		// function is not used.
		if (stylesheet.callsNodeset() && !DOM_CLASS.Equals(DOM_IMPL_CLASS))
		{
			// new org.apache.xalan.xsltc.dom.DOMAdapter(DOMImpl,String[]);
			index = cpg.addMethodref(DOM_ADAPTER_CLASS, "<init>", "(" + DOM_INTF_SIG + "[" + STRING_SIG + "[" + STRING_SIG + "[I" + "[" + STRING_SIG + ")V");
			il.append(new NEW(cpg.addClass(DOM_ADAPTER_CLASS)));
			il.append(new DUP_X1());
			il.append(SWAP);

			/*
			 * Give the DOM adapter an empty type mapping if the nodeset
			 * extension function is never called.
			 */
			if (!stylesheet.callsNodeset())
			{
			il.append(new ICONST(0));
			il.append(new ANEWARRAY(cpg.addClass(STRING)));
			il.append(DUP);
			il.append(DUP);
			il.append(new ICONST(0));
			il.append(new NEWARRAY(BasicType.INT));
			il.append(SWAP);
			il.append(new INVOKESPECIAL(index));
			}
			else
			{
			// Push name arrays on the stack
			il.append(ALOAD_0);
			il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, NAMES_INDEX, NAMES_INDEX_SIG)));
			il.append(ALOAD_0);
			il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, URIS_INDEX, URIS_INDEX_SIG)));
			il.append(ALOAD_0);
			il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, TYPES_INDEX, TYPES_INDEX_SIG)));
			il.append(ALOAD_0);
			il.append(new GETFIELD(cpg.addFieldref(TRANSLET_CLASS, NAMESPACE_INDEX, NAMESPACE_INDEX_SIG)));

			// Initialized DOM adapter
			il.append(new INVOKESPECIAL(index));

			// Add DOM adapter to MultiDOM class by calling addDOMAdapter()
			il.append(DUP);
			il.append(methodGen.loadDOM());
			il.append(new CHECKCAST(cpg.addClass(classGen.DOMClass)));
			il.append(SWAP);
			index = cpg.addMethodref(MULTI_DOM_CLASS, "addDOMAdapter", "(" + DOM_ADAPTER_SIG + ")I");
			il.append(new INVOKEVIRTUAL(index));
			il.append(POP); // ignore mask returned by addDOMAdapter
			}
		}

		// Restore old handler base from stack
		il.append(SWAP);
		il.append(methodGen.storeHandler());
		}

		/// <summary>
		/// Retrieve an ID to identify the namespaces in scope at this point in the
		/// stylesheet </summary>
		/// <returns> An <code>int</code> representing the node ID or <code>-1</code>
		///         if no namespace declarations are in scope </returns>
		protected internal int NodeIDForStylesheetNSLookup
		{
			get
			{
				if (_nodeIDForStylesheetNSLookup == UNKNOWN_STYLESHEET_NODE_ID)
				{
					Hashtable prefixMapping = PrefixMapping;
					int parentNodeID = (_parent != null) ? _parent.NodeIDForStylesheetNSLookup : UNKNOWN_STYLESHEET_NODE_ID;
    
					// If this node in the stylesheet has no namespace declarations of
					// its own, use the ID of the nearest ancestor element that does 
					// have namespace declarations.
					if (prefixMapping == null)
					{
						_nodeIDForStylesheetNSLookup = parentNodeID;
					}
					else
					{
						// Inform the XSLTC object that we'll need to know about this
						// node's namespace declarations.
						_nodeIDForStylesheetNSLookup = XSLTC.registerStylesheetPrefixMappingForRuntime(prefixMapping, parentNodeID);
					}
				}
    
				return _nodeIDForStylesheetNSLookup;
			}
		}
		/// <summary>
		/// Returns true if this expression/instruction depends on the context. By 
		/// default, every expression/instruction depends on the context unless it 
		/// overrides this method. Currently used to determine if result trees are 
		/// compiled using procedures or little DOMs (result tree fragments). </summary>
		/// <returns> 'true' if this node depends on the context. </returns>
		protected internal virtual bool contextDependent()
		{
		return true;
		}

		/// <summary>
		/// Return true if any of the expressions/instructions in the contents of
		/// this node is context dependent. </summary>
		/// <returns> 'true' if the contents of this node is context dependent. </returns>
		protected internal virtual bool dependentContents()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = elementCount();
		int n = elementCount();
		for (int i = 0; i < n; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode item = (SyntaxTreeNode)_contents.elementAt(i);
			SyntaxTreeNode item = (SyntaxTreeNode)_contents[i];
			if (item.contextDependent())
			{
			return true;
			}
		}
		return false;
		}

		/// <summary>
		/// Adds a child node to this syntax tree node. </summary>
		/// <param name="element"> is the new child node. </param>
		protected internal void addElement(SyntaxTreeNode element)
		{
		_contents.Add(element);
		element.Parent = this;
		}

		/// <summary>
		/// Inserts the first child node of this syntax tree node. The existing
		/// children are shifted back one position. </summary>
		/// <param name="element"> is the new child node. </param>
		protected internal SyntaxTreeNode FirstElement
		{
			set
			{
			_contents.Insert(0, value);
			value.Parent = this;
			}
		}

		/// <summary>
		/// Removed a child node of this syntax tree node. </summary>
		/// <param name="element"> is the child node to remove. </param>
		protected internal void removeElement(SyntaxTreeNode element)
		{
		_contents.Remove(element);
		element.Parent = null;
		}

		/// <summary>
		/// Returns a Vector containing all the child nodes of this node. </summary>
		/// <returns> A Vector containing all the child nodes of this node. </returns>
		protected internal ArrayList Contents
		{
			get
			{
			return _contents;
			}
		}

		/// <summary>
		/// Tells you if this node has any child nodes. </summary>
		/// <returns> 'true' if this node has any children. </returns>
		protected internal bool hasContents()
		{
		return elementCount() > 0;
		}

		/// <summary>
		/// Returns the number of children this node has. </summary>
		/// <returns> Number of child nodes. </returns>
		protected internal int elementCount()
		{
		return _contents.Count;
		}

		/// <summary>
		/// Returns an Enumeration of all child nodes of this node. </summary>
		/// <returns> An Enumeration of all child nodes of this node. </returns>
		protected internal System.Collections.IEnumerator elements()
		{
		return _contents.GetEnumerator();
		}

		/// <summary>
		/// Returns a child node at a given position. </summary>
		/// <param name="pos"> The child node's position. </param>
		/// <returns> The child node. </returns>
		protected internal object elementAt(int pos)
		{
		return _contents[pos];
		}

		/// <summary>
		/// Returns this element's last child </summary>
		/// <returns> The child node. </returns>
		protected internal SyntaxTreeNode lastChild()
		{
		if (_contents.Count == 0)
		{
			return null;
		}
		return (SyntaxTreeNode)_contents[_contents.Count - 1];
		}

		/// <summary>
		/// Displays the contents of this syntax tree node (to stdout).
		/// This method is intended for debugging _only_, and should be overridden
		/// by all syntax tree node implementations. </summary>
		/// <param name="indent"> Indentation level for syntax tree levels. </param>
		public virtual void display(int indent)
		{
		displayContents(indent);
		}

		/// <summary>
		/// Displays the contents of this syntax tree node (to stdout).
		/// This method is intended for debugging _only_ !!! </summary>
		/// <param name="indent"> Indentation level for syntax tree levels. </param>
		protected internal virtual void displayContents(int indent)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = elementCount();
		int n = elementCount();
		for (int i = 0; i < n; i++)
		{
			SyntaxTreeNode item = (SyntaxTreeNode)_contents[i];
			item.display(indent);
		}
		}

		/// <summary>
		/// Set the indentation level for debug output. </summary>
		/// <param name="indent"> Indentation level for syntax tree levels. </param>
		protected internal void indent(int indent)
		{
		Console.Write(new string(_spaces, 0, indent));
		}

		/// <summary>
		/// Report an error to the parser. </summary>
		/// <param name="element"> The element in which the error occured (normally 'this'
		/// but it could also be an expression/pattern/etc.) </param>
		/// <param name="parser"> The XSLT parser to report the error to. </param>
		/// <param name="error"> The error code (from util/ErrorMsg). </param>
		/// <param name="message"> Any additional error message. </param>
		protected internal virtual void reportError(SyntaxTreeNode element, Parser parser, string errorCode, string message)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg error = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(errorCode, message, element);
		ErrorMsg error = new ErrorMsg(errorCode, message, element);
			parser.reportError(Constants.ERROR, error);
		}

		/// <summary>
		/// Report a recoverable error to the parser. </summary>
		/// <param name="element"> The element in which the error occured (normally 'this'
		/// but it could also be an expression/pattern/etc.) </param>
		/// <param name="parser"> The XSLT parser to report the error to. </param>
		/// <param name="error"> The error code (from util/ErrorMsg). </param>
		/// <param name="message"> Any additional error message. </param>
		protected internal virtual void reportWarning(SyntaxTreeNode element, Parser parser, string errorCode, string message)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg error = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(errorCode, message, element);
		ErrorMsg error = new ErrorMsg(errorCode, message, element);
			parser.reportError(Constants.WARNING, error);
		}

	}

}