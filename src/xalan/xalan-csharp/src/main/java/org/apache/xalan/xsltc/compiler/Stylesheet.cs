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
 * $Id: Stylesheet.java 669373 2008-06-19 03:40:20Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using ANEWARRAY = org.apache.bcel.generic.ANEWARRAY;
	using BasicType = org.apache.bcel.generic.BasicType;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using FieldGen = org.apache.bcel.generic.FieldGen;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using GETSTATIC = org.apache.bcel.generic.GETSTATIC;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using NEW = org.apache.bcel.generic.NEW;
	using NEWARRAY = org.apache.bcel.generic.NEWARRAY;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using PUTSTATIC = org.apache.bcel.generic.PUTSTATIC;
	using TargetLostException = org.apache.bcel.generic.TargetLostException;
	using InstructionFinder = org.apache.bcel.util.InstructionFinder;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using DTM = org.apache.xml.dtm.DTM;

	public sealed class Stylesheet : SyntaxTreeNode
	{

		/// <summary>
		/// XSLT version defined in the stylesheet.
		/// </summary>
		private string _version;

		/// <summary>
		/// Internal name of this stylesheet used as a key into the symbol table.
		/// </summary>
		private QName _name;

		/// <summary>
		/// A URI that represents the system ID for this stylesheet.
		/// </summary>
		private string _systemId;

		/// <summary>
		/// A reference to the parent stylesheet or null if topmost.
		/// </summary>
		private Stylesheet _parentStylesheet;

		/// <summary>
		/// Contains global variables and parameters defined in the stylesheet.
		/// </summary>
		private ArrayList _globals = new ArrayList();

		/// <summary>
		/// Used to cache the result returned by <code>hasLocalParams()</code>.
		/// </summary>
		private bool? _hasLocalParams = null;

		/// <summary>
		/// The name of the class being generated.
		/// </summary>
		private string _className;

		/// <summary>
		/// Contains all templates defined in this stylesheet
		/// </summary>
		private readonly ArrayList _templates = new ArrayList();

		/// <summary>
		/// Used to cache result of <code>getAllValidTemplates()</code>. Only
		/// set in top-level stylesheets that include/import other stylesheets.
		/// </summary>
		private ArrayList _allValidTemplates = null;

		private ArrayList _elementsWithNamespacesUsedDynamically = null;

		/// <summary>
		/// Counter to generate unique mode suffixes.
		/// </summary>
		private int _nextModeSerial = 1;

		/// <summary>
		/// Mapping between mode names and Mode instances.
		/// </summary>
		private readonly Hashtable _modes = new Hashtable();

		/// <summary>
		/// A reference to the default Mode object.
		/// </summary>
		private Mode _defaultMode;

		/// <summary>
		/// Mapping between extension URIs and their prefixes.
		/// </summary>
		private readonly Hashtable _extensions = new Hashtable();

		/// <summary>
		/// Reference to the stylesheet from which this stylesheet was
		/// imported (if any).
		/// </summary>
		public Stylesheet _importedFrom = null;

		/// <summary>
		/// Reference to the stylesheet from which this stylesheet was
		/// included (if any).
		/// </summary>
		public Stylesheet _includedFrom = null;

		/// <summary>
		/// Array of all the stylesheets imported or included from this one.
		/// </summary>
		private ArrayList _includedStylesheets = null;

		/// <summary>
		/// Import precendence for this stylesheet.
		/// </summary>
		private int _importPrecedence = 1;

		/// <summary>
		/// Minimum precendence of any descendant stylesheet by inclusion or
		/// importation.
		/// </summary>
		private int _minimumDescendantPrecedence = -1;

		/// <summary>
		/// Mapping between key names and Key objects (needed by Key/IdPattern).
		/// </summary>
		private Hashtable _keys = new Hashtable();

		/// <summary>
		/// A reference to the SourceLoader set by the user (a URIResolver
		/// if the JAXP API is being used).
		/// </summary>
		private SourceLoader _loader = null;

		/// <summary>
		/// Flag indicating if format-number() is called.
		/// </summary>
		private bool _numberFormattingUsed = false;

		/// <summary>
		/// Flag indicating if this is a simplified stylesheets. A template
		/// matching on "/" must be added in this case.
		/// </summary>
		private bool _simplified = false;

		/// <summary>
		/// Flag indicating if multi-document support is needed.
		/// </summary>
		private bool _multiDocument = false;

		/// <summary>
		/// Flag indicating if nodset() is called.
		/// </summary>
		private bool _callsNodeset = false;

		/// <summary>
		/// Flag indicating if id() is called.
		/// </summary>
		private bool _hasIdCall = false;

		/// <summary>
		/// Set to true to enable template inlining optimization. </summary>
		/// <seealso cref="XSLTC._templateInlining"/>
		private bool _templateInlining = false;

		/// <summary>
		/// A reference to the last xsl:output object found in the styleshet.
		/// </summary>
		private Output _lastOutputElement = null;

		/// <summary>
		/// Output properties for this stylesheet.
		/// </summary>
		private Properties _outputProperties = null;

		/// <summary>
		/// Output method for this stylesheet (must be set to one of
		/// the constants defined below).
		/// </summary>
		private int _outputMethod = UNKNOWN_OUTPUT;

		// Output method constants
		public const int UNKNOWN_OUTPUT = 0;
		public const int XML_OUTPUT = 1;
		public const int HTML_OUTPUT = 2;
		public const int TEXT_OUTPUT = 3;

		/// <summary>
		/// Return the output method
		/// </summary>
		public int OutputMethod
		{
			get
			{
				return _outputMethod;
			}
		}

		/// <summary>
		/// Check and set the output method
		/// </summary>
		private void checkOutputMethod()
		{
		if (_lastOutputElement != null)
		{
			string method = _lastOutputElement.OutputMethod;
			if (!string.ReferenceEquals(method, null))
			{
				if (method.Equals("xml"))
				{
					_outputMethod = XML_OUTPUT;
				}
				else if (method.Equals("html"))
				{
					_outputMethod = HTML_OUTPUT;
				}
				else if (method.Equals("text"))
				{
					_outputMethod = TEXT_OUTPUT;
				}
			}
		}
		}

		public bool TemplateInlining
		{
			get
			{
			return _templateInlining;
			}
			set
			{
			_templateInlining = value;
			}
		}


		public bool Simplified
		{
			get
			{
			return (_simplified);
			}
		}

		public void setSimplified()
		{
		_simplified = true;
		}

		public bool HasIdCall
		{
			set
			{
				_hasIdCall = value;
			}
		}

		public void setOutputProperty(string key, string value)
		{
		if (_outputProperties == null)
		{
			_outputProperties = new Properties();
		}
		_outputProperties.setProperty(key, value);
		}

		public Properties OutputProperties
		{
			set
			{
			_outputProperties = value;
			}
			get
			{
			return _outputProperties;
			}
		}


		public Output LastOutputElement
		{
			get
			{
				return _lastOutputElement;
			}
		}

		public bool MultiDocument
		{
			set
			{
			_multiDocument = value;
			}
			get
			{
			return _multiDocument;
			}
		}


		public bool CallsNodeset
		{
			set
			{
			if (value)
			{
				MultiDocument = value;
			}
			_callsNodeset = value;
			}
		}

		public bool callsNodeset()
		{
		return _callsNodeset;
		}

		public void numberFormattingUsed()
		{
		_numberFormattingUsed = true;
			/*
			 * Fix for bug 23046, if the stylesheet is included, set the 
			 * numberFormattingUsed flag to the parent stylesheet too.
			 * AbstractTranslet.addDecimalFormat() will be inlined once for the
			 * outer most stylesheet. 
			 */ 
			Stylesheet parent = ParentStylesheet;
			if (null != parent)
			{
				parent.numberFormattingUsed();
			}
		}

		public int ImportPrecedence
		{
			set
			{
				// Set import value for this stylesheet
				_importPrecedence = value;

				// Set import value for all included stylesheets
				//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				//ORIGINAL LINE: final java.util.Enumeration elements = elements();
				System.Collections.IEnumerator elements = this.elements();
				while (elements.MoveNext())
				{
					SyntaxTreeNode child = (SyntaxTreeNode)elements.Current;
					if (child is Include)
					{
						Stylesheet included = ((Include)child).IncludedStylesheet;
						if (included != null && included._includedFrom == this)
						{
							included.ImportPrecedence = value;
						}
					}
				}

				// Set import value for the stylesheet that imported this one
				if (_importedFrom != null)
				{
					if (_importedFrom.ImportPrecedence < value)
					{
						//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
						//ORIGINAL LINE: final Parser parser = getParser();
						Parser parser = Parser;
						//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
						//ORIGINAL LINE: final int nextPrecedence = parser.getNextImportPrecedence();
						int nextPrecedence = parser.NextImportPrecedence;
						_importedFrom.ImportPrecedence = nextPrecedence;
					}
				}
				// Set import value for the stylesheet that included this one
				else if (_includedFrom != null)
				{
					if (_includedFrom.ImportPrecedence != value)
					{
						_includedFrom.ImportPrecedence = value;
					}
				}
			}
			get
			{
				return _importPrecedence;
			}
		}


		/// <summary>
		/// Get the minimum of the precedence of this stylesheet, any stylesheet
		/// imported by this stylesheet and any include/import descendant of this
		/// stylesheet.
		/// </summary>
		public int MinimumDescendantPrecedence
		{
			get
			{
				if (_minimumDescendantPrecedence == -1)
				{
					// Start with precedence of current stylesheet as a basis.
					int min = ImportPrecedence;
    
					// Recursively examine all imported/included stylesheets.
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int inclImpCount = (_includedStylesheets != null) ? _includedStylesheets.size() : 0;
					int inclImpCount = (_includedStylesheets != null) ? _includedStylesheets.Count : 0;
    
					for (int i = 0; i < inclImpCount; i++)
					{
						int prec = ((Stylesheet)_includedStylesheets[i]).MinimumDescendantPrecedence;
    
						if (prec < min)
						{
							min = prec;
						}
					}
    
					_minimumDescendantPrecedence = min;
				}
				return _minimumDescendantPrecedence;
			}
		}

		public bool checkForLoop(string systemId)
		{
		// Return true if this stylesheet includes/imports itself
		if (!string.ReferenceEquals(_systemId, null) && _systemId.Equals(systemId))
		{
			return true;
		}
		// Then check with any stylesheets that included/imported this one
		if (_parentStylesheet != null)
		{
			return _parentStylesheet.checkForLoop(systemId);
		}
		// Otherwise OK
		return false;
		}

		public override Parser Parser
		{
			set
			{
			base.Parser = value;
			_name = makeStylesheetName("__stylesheet_");
			}
		}

		public Stylesheet ParentStylesheet
		{
			set
			{
			_parentStylesheet = value;
			}
			get
			{
			return _parentStylesheet;
			}
		}


		public Stylesheet ImportingStylesheet
		{
			set
			{
			_importedFrom = value;
			value.addIncludedStylesheet(this);
			}
		}

		public Stylesheet IncludingStylesheet
		{
			set
			{
			_includedFrom = value;
			value.addIncludedStylesheet(this);
			}
		}

		public void addIncludedStylesheet(Stylesheet child)
		{
			if (_includedStylesheets == null)
			{
				_includedStylesheets = new ArrayList();
			}
			_includedStylesheets.Add(child);
		}

		public string SystemId
		{
			set
			{
				if (!string.ReferenceEquals(value, null))
				{
					_systemId = SystemIDResolver.getAbsoluteURI(value);
				}
			}
			get
			{
			return _systemId;
			}
		}


		public SourceLoader SourceLoader
		{
			set
			{
			_loader = value;
			}
			get
			{
			return _loader;
			}
		}


		private QName makeStylesheetName(string prefix)
		{
		return Parser.getQName(prefix + XSLTC.nextStylesheetSerial());
		}

		/// <summary>
		/// Returns true if this stylesheet has global vars or params.
		/// </summary>
		public bool hasGlobals()
		{
		return _globals.Count > 0;
		}

		/// <summary>
		/// Returns true if at least one template in the stylesheet has params
		/// defined. Uses the variable <code>_hasLocalParams</code> to cache the
		/// result.
		/// </summary>
		public bool hasLocalParams()
		{
		if (_hasLocalParams == null)
		{
			ArrayList templates = AllValidTemplates;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = templates.size();
			int n = templates.Count;
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)templates.elementAt(i);
			Template template = (Template)templates[i];
			if (template.hasParams())
			{
				_hasLocalParams = true;
				return true;
			}
			}
			_hasLocalParams = false;
			return false;
		}
		else
		{
			return _hasLocalParams.Value;
		}
		}

		/// <summary>
		/// Adds a single prefix mapping to this syntax tree node. </summary>
		/// <param name="prefix"> Namespace prefix. </param>
		/// <param name="uri"> Namespace URI. </param>
		protected internal override void addPrefixMapping(string prefix, string uri)
		{
		if (prefix.Equals(EMPTYSTRING) && uri.Equals(XHTML_URI))
		{
			return;
		}
		base.addPrefixMapping(prefix, uri);
		}

		/// <summary>
		/// Store extension URIs
		/// </summary>
		private void extensionURI(string prefixes, SymbolTable stable)
		{
		if (!string.ReferenceEquals(prefixes, null))
		{
			StringTokenizer tokens = new StringTokenizer(prefixes);
			while (tokens.hasMoreTokens())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = tokens.nextToken();
			string prefix = tokens.nextToken();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = lookupNamespace(prefix);
			string uri = lookupNamespace(prefix);
			if (!string.ReferenceEquals(uri, null))
			{
				_extensions[uri] = prefix;
			}
			}
		}
		}

		public bool isExtension(string uri)
		{
		return (_extensions[uri] != null);
		}

		public void declareExtensionPrefixes(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
		SymbolTable stable = parser.SymbolTable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String extensionPrefixes = getAttribute("extension-element-prefixes");
		string extensionPrefixes = getAttribute("extension-element-prefixes");
		extensionURI(extensionPrefixes, stable);
		}

		/// <summary>
		/// Parse the version and uri fields of the stylesheet and add an
		/// entry to the symbol table mapping the name <tt>__stylesheet_</tt>
		/// to an instance of this class.
		/// </summary>
		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
		SymbolTable stable = parser.SymbolTable;

		/*
		// Make sure the XSL version set in this stylesheet
		if ((_version == null) || (_version.equals(EMPTYSTRING))) {
		    reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR,"version");
		}
		// Verify that the version is 1.0 and nothing else
		else if (!_version.equals("1.0")) {
		    reportError(this, parser, ErrorMsg.XSL_VERSION_ERR, _version);
		}
		*/

		// Add the implicit mapping of 'xml' to the XML namespace URI
		addPrefixMapping("xml", "http://www.w3.org/XML/1998/namespace");

		// Report and error if more than one stylesheet defined
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stylesheet sheet = stable.addStylesheet(_name, this);
		Stylesheet sheet = stable.addStylesheet(_name, this);
		if (sheet != null)
		{
			// Error: more that one stylesheet defined
			ErrorMsg err = new ErrorMsg(ErrorMsg.MULTIPLE_STYLESHEET_ERR,this);
			parser.reportError(Constants.ERROR, err);
		}

		// If this is a simplified stylesheet we must create a template that
		// grabs the root node of the input doc ( <xsl:template match="/"/> ).
		// This template needs the current element (the one passed to this
		// method) as its only child, so the Template class has a special
		// method that handles this (parseSimplified()).
		if (_simplified)
		{
			stable.excludeURI(XSLT_URI);
			Template template = new Template();
			template.parseSimplified(this, parser);
		}
		// Parse the children of this node
		else
		{
			parseOwnChildren(parser);
		}
		}

		/// <summary>
		/// Parse all direct children of the <xsl:stylesheet/> element.
		/// </summary>
		public void parseOwnChildren(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable stable = parser.getSymbolTable();
			SymbolTable stable = parser.SymbolTable;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String excludePrefixes = getAttribute("exclude-result-prefixes");
			string excludePrefixes = getAttribute("exclude-result-prefixes");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String extensionPrefixes = getAttribute("extension-element-prefixes");
			string extensionPrefixes = getAttribute("extension-element-prefixes");

			// Exclude XSLT uri 
			stable.pushExcludedNamespacesContext();
			stable.excludeURI(Constants.XSLT_URI);
			stable.excludeNamespaces(excludePrefixes);
			stable.excludeNamespaces(extensionPrefixes);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector contents = getContents();
		ArrayList contents = Contents;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = contents.size();
		int count = contents.Count;

		// We have to scan the stylesheet element's top-level elements for
		// variables and/or parameters before we parse the other elements
		for (int i = 0; i < count; i++)
		{
			SyntaxTreeNode child = (SyntaxTreeNode)contents[i];
			if ((child is VariableBase) || (child is NamespaceAlias))
			{
			parser.SymbolTable.CurrentNode = child;
			child.parseContents(parser);
			}
		}

		// Now go through all the other top-level elements...
		for (int i = 0; i < count; i++)
		{
			SyntaxTreeNode child = (SyntaxTreeNode)contents[i];
			if (!(child is VariableBase) && !(child is NamespaceAlias))
			{
			parser.SymbolTable.CurrentNode = child;
			child.parseContents(parser);
			}

			// All template code should be compiled as methods if the
			// <xsl:apply-imports/> element was ever used in this stylesheet
			if (!_templateInlining && (child is Template))
			{
			Template template = (Template)child;
			string name = "template$dot$" + template.Position;
			template.Name = parser.getQName(name);
			}
		}

		stable.popExcludedNamespacesContext();
		}

		public void processModes()
		{
		if (_defaultMode == null)
		{
			_defaultMode = new Mode(null, this, Constants.EMPTYSTRING);
		}
		_defaultMode.processPatterns(_keys);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration modes = _modes.elements();
		System.Collections.IEnumerator modes = _modes.Values.GetEnumerator();
		while (modes.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Mode mode = (Mode)modes.Current;
			Mode mode = (Mode)modes.Current;
			mode.processPatterns(_keys);
		}
		}

		private void compileModes(ClassGenerator classGen)
		{
		_defaultMode.compileApplyTemplates(classGen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration modes = _modes.elements();
		System.Collections.IEnumerator modes = _modes.Values.GetEnumerator();
		while (modes.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Mode mode = (Mode)modes.Current;
			Mode mode = (Mode)modes.Current;
			mode.compileApplyTemplates(classGen);
		}
		}

		public Mode getMode(QName modeName)
		{
		if (modeName == null)
		{
			if (_defaultMode == null)
			{
			_defaultMode = new Mode(null, this, Constants.EMPTYSTRING);
			}
			return _defaultMode;
		}
		else
		{
			Mode mode = (Mode)_modes[modeName];
			if (mode == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String suffix = System.Convert.ToString(_nextModeSerial++);
			string suffix = Convert.ToString(_nextModeSerial++);
			_modes[modeName] = mode = new Mode(modeName, this, suffix);
			}
			return mode;
		}
		}

		/// <summary>
		/// Type check all the children of this node.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _globals.size();
		int count = _globals.Count;
		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final VariableBase var = (VariableBase)_globals.elementAt(i);
			VariableBase var = (VariableBase)_globals[i];
			var.typeCheck(stable);
		}
		return typeCheckContents(stable);
		}

		/// <summary>
		/// Translate the stylesheet into JVM bytecodes. 
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		translate();
		}

		private void addDOMField(ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.FieldGen fgen = new org.apache.bcel.generic.FieldGen(ACC_PUBLIC, org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(DOM_INTF_SIG), DOM_FIELD, classGen.getConstantPool());
		FieldGen fgen = new FieldGen(ACC_PUBLIC, Util.getJCRefType(DOM_INTF_SIG), DOM_FIELD, classGen.getConstantPool());
		classGen.addField(fgen.getField());
		}

		/// <summary>
		/// Add a static field
		/// </summary>
		private void addStaticField(ClassGenerator classGen, string type, string name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.FieldGen fgen = new org.apache.bcel.generic.FieldGen(ACC_PROTECTED|ACC_STATIC, org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(type), name, classGen.getConstantPool());
			FieldGen fgen = new FieldGen(ACC_PROTECTED | ACC_STATIC, Util.getJCRefType(type), name, classGen.getConstantPool());
			classGen.addField(fgen.getField());

		}

		/// <summary>
		/// Translate the stylesheet into JVM bytecodes. 
		/// </summary>
		public void translate()
		{
		_className = XSLTC.ClassName;

		// Define a new class by extending TRANSLET_CLASS
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ClassGenerator classGen = new org.apache.xalan.xsltc.compiler.util.ClassGenerator(_className, TRANSLET_CLASS, Constants.EMPTYSTRING, ACC_PUBLIC | ACC_SUPER, null, this);
		ClassGenerator classGen = new ClassGenerator(_className, TRANSLET_CLASS, Constants.EMPTYSTRING, ACC_PUBLIC | ACC_SUPER, null, this);

		addDOMField(classGen);

		// Compile transform() to initialize parameters, globals & output
		// and run the transformation
		compileTransform(classGen);

		// Translate all non-template elements and filter out all templates
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration elements = elements();
		System.Collections.IEnumerator elements = this.elements();
		while (elements.MoveNext())
		{
			object element = elements.Current;
			// xsl:template
			if (element is Template)
			{
			// Separate templates by modes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = (Template)element;
			Template template = (Template)element;
			//_templates.addElement(template);
			getMode(template.ModeName).addTemplate(template);
			}
			// xsl:attribute-set
			else if (element is AttributeSet)
			{
			((AttributeSet)element).translate(classGen, null);
			}
			else if (element is Output)
			{
			// save the element for later to pass to compileConstructor 
			Output output = (Output)element;
			if (output.enabled())
			{
				_lastOutputElement = output;
			}
			}
			else
			{
			// Global variables and parameters are handled elsewhere.
			// Other top-level non-template elements are ignored. Literal
			// elements outside of templates will never be output.
			}
		}

		checkOutputMethod();
		processModes();
		compileModes(classGen);
			compileStaticInitializer(classGen);
		compileConstructor(classGen, _lastOutputElement);

		if (!Parser.errorsFound())
		{
			XSLTC.dumpClass(classGen.getJavaClass());
		}
		}

		/// <summary>
		/// <para>Compile the namesArray, urisArray, typesArray, namespaceArray,
		/// namespaceAncestorsArray, prefixURIsIdxArray and prefixURIPairsArray into
		/// the static initializer. They are read-only from the
		/// translet. All translet instances can share a single
		/// copy of this informtion.</para>
		/// <para>The <code>namespaceAncestorsArray</code>,
		/// <code>prefixURIsIdxArray</code> and <code>prefixURIPairsArray</code>
		/// contain namespace information accessible from the stylesheet:
		/// <dl>
		/// <dt><code>namespaceAncestorsArray</code></dt>
		/// <dd>Array indexed by integer stylesheet node IDs containing node IDs of
		/// the nearest ancestor node in the stylesheet with namespace
		/// declarations or <code>-1</code> if there is no such ancestor.  There
		/// can be more than one disjoint tree of nodes - one for each stylesheet
		/// module</dd>
		/// <dt><code>prefixURIsIdxArray</code></dt>
		/// <dd>Array indexed by integer stylesheet node IDs containing the index
		/// into <code>prefixURIPairsArray</code> of the first namespace prefix
		/// declared for the node.  The values are stored in ascending order, so
		/// the next value in this array (if any) can be used to find the last such
		/// prefix-URI pair</dd>
		/// <dt>prefixURIPairsArray</dt>
		/// <dd>Array of pairs of namespace prefixes and URIs.  A zero-length
		/// string represents the default namespace if it appears as a prefix and
		/// a namespace undeclaration if it appears as a URI.</dd>
		/// </dl>
		/// </para>
		/// <para>For this stylesheet
		/// <pre><code>
		/// &lt;xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"&gt;
		///   &lt;xsl:template match="/"&gt;
		///     &lt;xsl:for-each select="*" xmlns:foo="foouri"&gt;
		///       &lt;xsl:element name="{n}" xmlns:foo="baruri"&gt;
		///     &lt;/xsl:for-each&gt;
		///     &lt;out xmlns="lumpit"/&gt;
		///     &lt;xsl:element name="{n}" xmlns="foouri"/&gt;
		///     &lt;xsl:element name="{n}" namespace="{ns}" xmlns="limpit"/gt;
		///   &lt;/xsl:template&gt;
		/// &lt;/xsl:stylesheet&gt;
		/// </code></pre>
		/// there will be four stylesheet nodes whose namespace information is
		/// needed, and
		/// <ul>
		/// <li><code>namespaceAncestorsArray</code> will have the value
		/// <code>[-1,0,1,0]</code>;</li>
		/// <li><code>prefixURIsIdxArray</code> will have the value
		/// <code>[0,4,6,8]</code>; and</li>
		/// <li><code>prefixURIPairsArray</code> will have the value
		/// <code>["xml","http://www.w3.org/XML/1998/namespace",
		///        "xsl","http://www.w3.org/1999/XSL/Transform"
		///        "foo","foouri","foo","baruri","","foouri"].</code></li>
		/// </ul>
		/// </para>
		/// </summary>
		private void compileStaticInitializer(ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator staticConst = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC|ACC_STATIC, org.apache.bcel.generic.Type.VOID, null, null, "<clinit>", _className, il, cpg);
		MethodGenerator staticConst = new MethodGenerator(ACC_PUBLIC | ACC_STATIC, org.apache.bcel.generic.Type.VOID, null, null, "<clinit>", _className, il, cpg);

		addStaticField(classGen, "[" + STRING_SIG, STATIC_NAMES_ARRAY_FIELD);
		addStaticField(classGen, "[" + STRING_SIG, STATIC_URIS_ARRAY_FIELD);
		addStaticField(classGen, "[I", STATIC_TYPES_ARRAY_FIELD);
		addStaticField(classGen, "[" + STRING_SIG, STATIC_NAMESPACE_ARRAY_FIELD);
			// Create fields of type char[] that will contain literal text from
			// the stylesheet.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int charDataFieldCount = getXSLTC().getCharacterDataCount();
			int charDataFieldCount = XSLTC.CharacterDataCount;
			for (int i = 0; i < charDataFieldCount; i++)
			{
				addStaticField(classGen, STATIC_CHAR_DATA_FIELD_SIG, STATIC_CHAR_DATA_FIELD + i);
			}

		// Put the names array into the translet - used for dom/translet mapping
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector namesIndex = getXSLTC().getNamesIndex();
		ArrayList namesIndex = XSLTC.NamesIndex;
		int size = namesIndex.Count;
		string[] namesArray = new string[size];
		string[] urisArray = new string[size];
		int[] typesArray = new int[size];

		int index;
		for (int i = 0; i < size; i++)
		{
			string encodedName = (string)namesIndex[i];
			if ((index = encodedName.LastIndexOf(':')) > -1)
			{
				urisArray[i] = encodedName.Substring(0, index);
			}

			index = index + 1;
			if (encodedName[index] == '@')
			{
				typesArray[i] = DTM.ATTRIBUTE_NODE;
				index++;
			}
			else if (encodedName[index] == '?')
			{
				typesArray[i] = DTM.NAMESPACE_NODE;
				index++;
			}
			else
			{
				typesArray[i] = DTM.ELEMENT_NODE;
			}

			if (index == 0)
			{
				namesArray[i] = encodedName;
			}
			else
			{
				namesArray[i] = encodedName.Substring(index);
			}
		}

			staticConst.markChunkStart();
		il.append(new PUSH(cpg, size));
		il.append(new ANEWARRAY(cpg.addClass(STRING)));
			int namesArrayRef = cpg.addFieldref(_className, STATIC_NAMES_ARRAY_FIELD, NAMES_INDEX_SIG);
		il.append(new PUTSTATIC(namesArrayRef));
			staticConst.markChunkEnd();

		for (int i = 0; i < size; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = namesArray[i];
			string name = namesArray[i];
				staticConst.markChunkStart();
			il.append(new GETSTATIC(namesArrayRef));
			il.append(new PUSH(cpg, i));
			il.append(new PUSH(cpg, name));
			il.append(AASTORE);
				staticConst.markChunkEnd();
		}

			staticConst.markChunkStart();
		il.append(new PUSH(cpg, size));
		il.append(new ANEWARRAY(cpg.addClass(STRING)));
			int urisArrayRef = cpg.addFieldref(_className, STATIC_URIS_ARRAY_FIELD, URIS_INDEX_SIG);
		il.append(new PUTSTATIC(urisArrayRef));
			staticConst.markChunkEnd();

		for (int i = 0; i < size; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = urisArray[i];
			string uri = urisArray[i];
				staticConst.markChunkStart();
			il.append(new GETSTATIC(urisArrayRef));
			il.append(new PUSH(cpg, i));
			il.append(new PUSH(cpg, uri));
			il.append(AASTORE);
				staticConst.markChunkEnd();
		}

			staticConst.markChunkStart();
		il.append(new PUSH(cpg, size));
		il.append(new NEWARRAY(BasicType.INT));
			int typesArrayRef = cpg.addFieldref(_className, STATIC_TYPES_ARRAY_FIELD, TYPES_INDEX_SIG);
		il.append(new PUTSTATIC(typesArrayRef));
			staticConst.markChunkEnd();

		for (int i = 0; i < size; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nodeType = typesArray[i];
			int nodeType = typesArray[i];
				staticConst.markChunkStart();
			il.append(new GETSTATIC(typesArrayRef));
			il.append(new PUSH(cpg, i));
			il.append(new PUSH(cpg, nodeType));
			il.append(IASTORE);
				staticConst.markChunkEnd();
		}

		// Put the namespace names array into the translet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector namespaces = getXSLTC().getNamespaceIndex();
		ArrayList namespaces = XSLTC.NamespaceIndex;
			staticConst.markChunkStart();
		il.append(new PUSH(cpg, namespaces.Count));
		il.append(new ANEWARRAY(cpg.addClass(STRING)));
			int namespaceArrayRef = cpg.addFieldref(_className, STATIC_NAMESPACE_ARRAY_FIELD, NAMESPACE_INDEX_SIG);
		il.append(new PUTSTATIC(namespaceArrayRef));
			staticConst.markChunkEnd();

		for (int i = 0; i < namespaces.Count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String ns = (String)namespaces.elementAt(i);
			string ns = (string)namespaces[i];
				staticConst.markChunkStart();
			il.append(new GETSTATIC(namespaceArrayRef));
			il.append(new PUSH(cpg, i));
			il.append(new PUSH(cpg, ns));
			il.append(AASTORE);
				staticConst.markChunkEnd();
		}

			// Put the tree of stylesheet namespace declarations into the translet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector namespaceAncestors = getXSLTC().getNSAncestorPointers();
			ArrayList namespaceAncestors = XSLTC.NSAncestorPointers;
			if (namespaceAncestors != null && namespaceAncestors.Count != 0)
			{
				addStaticField(classGen, NS_ANCESTORS_INDEX_SIG, STATIC_NS_ANCESTORS_ARRAY_FIELD);
				staticConst.markChunkStart();
				il.append(new PUSH(cpg, namespaceAncestors.Count));
				il.append(new NEWARRAY(BasicType.INT));
				int namespaceAncestorsArrayRef = cpg.addFieldref(_className, STATIC_NS_ANCESTORS_ARRAY_FIELD, NS_ANCESTORS_INDEX_SIG);
				il.append(new PUTSTATIC(namespaceAncestorsArrayRef));
				staticConst.markChunkEnd();
				for (int i = 0; i < namespaceAncestors.Count; i++)
				{
					int ancestor = ((int?) namespaceAncestors[i]).Value;
					staticConst.markChunkStart();
					il.append(new GETSTATIC(namespaceAncestorsArrayRef));
					il.append(new PUSH(cpg, i));
					il.append(new PUSH(cpg, ancestor));
					il.append(IASTORE);
					staticConst.markChunkEnd();
				}
			}
			// Put the array of indices into the namespace prefix/URI pairs array
			// into the translet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector prefixURIPairsIdx = getXSLTC().getPrefixURIPairsIdx();
			ArrayList prefixURIPairsIdx = XSLTC.PrefixURIPairsIdx;
			if (prefixURIPairsIdx != null && prefixURIPairsIdx.Count != 0)
			{
				addStaticField(classGen, PREFIX_URIS_IDX_SIG, STATIC_PREFIX_URIS_IDX_ARRAY_FIELD);
				staticConst.markChunkStart();
				il.append(new PUSH(cpg, prefixURIPairsIdx.Count));
				il.append(new NEWARRAY(BasicType.INT));
				int prefixURIPairsIdxArrayRef = cpg.addFieldref(_className, STATIC_PREFIX_URIS_IDX_ARRAY_FIELD, PREFIX_URIS_IDX_SIG);
				il.append(new PUTSTATIC(prefixURIPairsIdxArrayRef));
				staticConst.markChunkEnd();
				for (int i = 0; i < prefixURIPairsIdx.Count; i++)
				{
					int idx = ((int?) prefixURIPairsIdx[i]).Value;
					staticConst.markChunkStart();
					il.append(new GETSTATIC(prefixURIPairsIdxArrayRef));
					il.append(new PUSH(cpg, i));
					il.append(new PUSH(cpg, idx));
					il.append(IASTORE);
					staticConst.markChunkEnd();
				}
			}

			// Put the array of pairs of namespace prefixes and URIs into the
			// translet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector prefixURIPairs = getXSLTC().getPrefixURIPairs();
			ArrayList prefixURIPairs = XSLTC.PrefixURIPairs;
			if (prefixURIPairs != null && prefixURIPairs.Count != 0)
			{
				addStaticField(classGen, PREFIX_URIS_ARRAY_SIG, STATIC_PREFIX_URIS_ARRAY_FIELD);

				staticConst.markChunkStart();
				il.append(new PUSH(cpg, prefixURIPairs.Count));
				il.append(new ANEWARRAY(cpg.addClass(STRING)));
				int prefixURIPairsRef = cpg.addFieldref(_className, STATIC_PREFIX_URIS_ARRAY_FIELD, PREFIX_URIS_ARRAY_SIG);
				il.append(new PUTSTATIC(prefixURIPairsRef));
				staticConst.markChunkEnd();
				for (int i = 0; i < prefixURIPairs.Count; i++)
				{
					string prefixOrURI = (string) prefixURIPairs[i];
					staticConst.markChunkStart();
					il.append(new GETSTATIC(prefixURIPairsRef));
					il.append(new PUSH(cpg, i));
					il.append(new PUSH(cpg, prefixOrURI));
					il.append(AASTORE);
					staticConst.markChunkEnd();
				}
			}

			// Grab all the literal text in the stylesheet and put it in a char[]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int charDataCount = getXSLTC().getCharacterDataCount();
			int charDataCount = XSLTC.CharacterDataCount;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int toCharArray = cpg.addMethodref(STRING, "toCharArray", "()[C");
			int toCharArray = cpg.addMethodref(STRING, "toCharArray", "()[C");
			for (int i = 0; i < charDataCount; i++)
			{
				staticConst.markChunkStart();
				il.append(new PUSH(cpg, XSLTC.getCharacterData(i)));
				il.append(new INVOKEVIRTUAL(toCharArray));
				il.append(new PUTSTATIC(cpg.addFieldref(_className, STATIC_CHAR_DATA_FIELD + i, STATIC_CHAR_DATA_FIELD_SIG)));
				staticConst.markChunkEnd();
			}

		il.append(RETURN);

		classGen.addMethod(staticConst);

		}

		/// <summary>
		/// Compile the translet's constructor
		/// </summary>
		private void compileConstructor(ClassGenerator classGen, Output output)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator constructor = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, null, null, "<init>", _className, il, cpg);
		MethodGenerator constructor = new MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, null, null, "<init>", _className, il, cpg);

		// Call the constructor in the AbstractTranslet superclass
		il.append(classGen.loadTranslet());
		il.append(new INVOKESPECIAL(cpg.addMethodref(TRANSLET_CLASS, "<init>", "()V")));

			constructor.markChunkStart();
		il.append(classGen.loadTranslet());
		il.append(new GETSTATIC(cpg.addFieldref(_className, STATIC_NAMES_ARRAY_FIELD, NAMES_INDEX_SIG)));
		il.append(new PUTFIELD(cpg.addFieldref(TRANSLET_CLASS, NAMES_INDEX, NAMES_INDEX_SIG)));
			constructor.markChunkEnd();

			constructor.markChunkStart();
		il.append(classGen.loadTranslet());
		il.append(new GETSTATIC(cpg.addFieldref(_className, STATIC_URIS_ARRAY_FIELD, URIS_INDEX_SIG)));
		il.append(new PUTFIELD(cpg.addFieldref(TRANSLET_CLASS, URIS_INDEX, URIS_INDEX_SIG)));
			constructor.markChunkEnd();

			constructor.markChunkStart();
		il.append(classGen.loadTranslet());
		il.append(new GETSTATIC(cpg.addFieldref(_className, STATIC_TYPES_ARRAY_FIELD, TYPES_INDEX_SIG)));
		il.append(new PUTFIELD(cpg.addFieldref(TRANSLET_CLASS, TYPES_INDEX, TYPES_INDEX_SIG)));
			constructor.markChunkEnd();

			constructor.markChunkStart();
		il.append(classGen.loadTranslet());
		il.append(new GETSTATIC(cpg.addFieldref(_className, STATIC_NAMESPACE_ARRAY_FIELD, NAMESPACE_INDEX_SIG)));
		il.append(new PUTFIELD(cpg.addFieldref(TRANSLET_CLASS, NAMESPACE_INDEX, NAMESPACE_INDEX_SIG)));
			constructor.markChunkEnd();

			constructor.markChunkStart();
		il.append(classGen.loadTranslet());
			il.append(new PUSH(cpg, AbstractTranslet.CURRENT_TRANSLET_VERSION));
		il.append(new PUTFIELD(cpg.addFieldref(TRANSLET_CLASS, TRANSLET_VERSION_INDEX, TRANSLET_VERSION_INDEX_SIG)));
			constructor.markChunkEnd();

		if (_hasIdCall)
		{
				constructor.markChunkStart();
			il.append(classGen.loadTranslet());
			il.append(new PUSH(cpg, true));
			il.append(new PUTFIELD(cpg.addFieldref(TRANSLET_CLASS, HASIDCALL_INDEX, HASIDCALL_INDEX_SIG)));
				constructor.markChunkEnd();
		}

			// Compile in code to set the output configuration from <xsl:output>
		if (output != null)
		{
			// Set all the output settings files in the translet
				constructor.markChunkStart();
			output.translate(classGen, constructor);
				constructor.markChunkEnd();
		}

		// Compile default decimal formatting symbols.
		// This is an implicit, nameless xsl:decimal-format top-level element.
		if (_numberFormattingUsed)
		{
				constructor.markChunkStart();
			DecimalFormatting.translateDefaultDFS(classGen, constructor);
				constructor.markChunkEnd();
		}

		il.append(RETURN);

		classGen.addMethod(constructor);
		}

		/// <summary>
		/// Compile a topLevel() method into the output class. This method is 
		/// called from transform() to handle all non-template top-level elements.
		/// Returns the signature of the topLevel() method.
		/// 
		/// Global variables/params and keys are first sorted to resolve 
		/// dependencies between them. The XSLT 1.0 spec does not allow a key 
		/// to depend on a variable. However, for compatibility with Xalan
		/// interpretive, that type of dependency is allowed. Note also that
		/// the buildKeys() method is still generated as it is used by the 
		/// LoadDocument class, but it no longer called from transform().
		/// </summary>
		private string compileTopLevel(ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type[] argTypes = { org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(DOM_INTF_SIG), org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(NODE_ITERATOR_SIG), org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(TRANSLET_OUTPUT_SIG) };
		org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[] {Util.getJCRefType(DOM_INTF_SIG), Util.getJCRefType(NODE_ITERATOR_SIG), Util.getJCRefType(TRANSLET_OUTPUT_SIG)};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argNames = { DOCUMENT_PNAME, ITERATOR_PNAME, TRANSLET_OUTPUT_PNAME };
		string[] argNames = new string[] {DOCUMENT_PNAME, ITERATOR_PNAME, TRANSLET_OUTPUT_PNAME};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator toplevel = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "topLevel", _className, il, classGen.getConstantPool());
		MethodGenerator toplevel = new MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "topLevel", _className, il, classGen.getConstantPool());

		toplevel.addException("org.apache.xalan.xsltc.TransletException");

		// Define and initialize 'current' variable with the root node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen current = toplevel.addLocalVariable("current", org.apache.bcel.generic.Type.INT, null, null);
		LocalVariableGen current = toplevel.addLocalVariable("current", org.apache.bcel.generic.Type.INT, null, null);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int setFilter = cpg.addInterfaceMethodref(DOM_INTF, "setFilter", "(Lorg/apache/xalan/xsltc/StripFilter;)V");
		int setFilter = cpg.addInterfaceMethodref(DOM_INTF, "setFilter", "(Lorg/apache/xalan/xsltc/StripFilter;)V");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int gitr = cpg.addInterfaceMethodref(DOM_INTF, "getIterator", "()"+NODE_ITERATOR_SIG);
		int gitr = cpg.addInterfaceMethodref(DOM_INTF, "getIterator", "()" + NODE_ITERATOR_SIG);
		il.append(toplevel.loadDOM());
		il.append(new INVOKEINTERFACE(gitr, 1));
			il.append(toplevel.nextNode());
		current.setStart(il.append(new ISTORE(current.getIndex())));

		// Create a new list containing variables/params + keys
		ArrayList varDepElements = new ArrayList(_globals);
		System.Collections.IEnumerator elements = this.elements();
		while (elements.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object element = elements.Current;
			object element = elements.Current;
			if (element is Key)
			{
				varDepElements.Add(element);
			}
		}

		// Determine a partial order for the variables/params and keys
		varDepElements = resolveDependencies(varDepElements);

		// Translate vars/params and keys in the right order
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = varDepElements.size();
		int count = varDepElements.Count;
		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TopLevelElement tle = (TopLevelElement) varDepElements.elementAt(i);
			TopLevelElement tle = (TopLevelElement) varDepElements[i];
			tle.translate(classGen, toplevel);
			if (tle is Key)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Key key = (Key) tle;
				Key key = (Key) tle;
				_keys[key.Name] = key;
			}
		}

		// Compile code for other top-level elements
		ArrayList whitespaceRules = new ArrayList();
		elements = this.elements();
		while (elements.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object element = elements.Current;
			object element = elements.Current;
			// xsl:decimal-format
			if (element is DecimalFormatting)
			{
			((DecimalFormatting)element).translate(classGen,toplevel);
			}
			// xsl:strip/preserve-space
			else if (element is Whitespace)
			{
			whitespaceRules.AddRange(((Whitespace)element).Rules);
			}
		}

		// Translate all whitespace strip/preserve rules
		if (whitespaceRules.Count > 0)
		{
			Whitespace.translateRules(whitespaceRules,classGen);
		}

		if (classGen.containsMethod(STRIP_SPACE, STRIP_SPACE_PARAMS) != null)
		{
			il.append(toplevel.loadDOM());
			il.append(classGen.loadTranslet());
			il.append(new INVOKEINTERFACE(setFilter, 2));
		}

		il.append(RETURN);

		// Compute max locals + stack and add method to class
		classGen.addMethod(toplevel);

		return ("(" + DOM_INTF_SIG + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + ")V");
		}

		/// <summary>
		/// This method returns a vector with variables/params and keys in the 
		/// order in which they are to be compiled for initialization. The order
		/// is determined by analyzing the dependencies between them. The XSLT 1.0 
		/// spec does not allow a key to depend on a variable. However, for 
		/// compatibility with Xalan interpretive, that type of dependency is 
		/// allowed and, therefore, consider to determine the partial order.
		/// </summary>
		private ArrayList resolveDependencies(ArrayList input)
		{
		/* DEBUG CODE - INGORE 
		for (int i = 0; i < input.size(); i++) {
		    final TopLevelElement e = (TopLevelElement) input.elementAt(i);
		    System.out.println("e = " + e + " depends on:");
				Vector dep = e.getDependencies();
				for (int j = 0; j < (dep != null ? dep.size() : 0); j++) {
					System.out.println("\t" + dep.elementAt(j));
				}
		}
		System.out.println("=================================");	
			*/

		ArrayList result = new ArrayList();
		while (input.Count > 0)
		{
			bool changed = false;
			for (int i = 0; i < input.Count;)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TopLevelElement vde = (TopLevelElement) input.elementAt(i);
			TopLevelElement vde = (TopLevelElement) input[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector dep = vde.getDependencies();
			ArrayList dep = vde.Dependencies;
			if (dep == null || result.ContainsAll(dep))
			{
				result.Add(vde);
				input.RemoveAt(i);
				changed = true;
			}
			else
			{
				i++;
			}
			}

			// If nothing was changed in this pass then we have a circular ref
			if (!changed)
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.CIRCULAR_VARIABLE_ERR, input.ToString(), this);
			Parser.reportError(Constants.ERROR, err);
			return (result);
			}
		}

		/* DEBUG CODE - INGORE 
		System.out.println("=================================");
		for (int i = 0; i < result.size(); i++) {
		    final TopLevelElement e = (TopLevelElement) result.elementAt(i);
		    System.out.println("e = " + e);
		}
			*/

		return result;
		}

		/// <summary>
		/// Compile a buildKeys() method into the output class. Note that keys 
		/// for the input document are created in topLevel(), not in this method. 
		/// However, we still need this method to create keys for documents loaded
		/// via the XPath document() function. 
		/// </summary>
		private string compileBuildKeys(ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type[] argTypes = { org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(DOM_INTF_SIG), org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(NODE_ITERATOR_SIG), org.apache.xalan.xsltc.compiler.util.Util.getJCRefType(TRANSLET_OUTPUT_SIG), org.apache.bcel.generic.Type.INT };
		org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[] {Util.getJCRefType(DOM_INTF_SIG), Util.getJCRefType(NODE_ITERATOR_SIG), Util.getJCRefType(TRANSLET_OUTPUT_SIG), org.apache.bcel.generic.Type.INT};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argNames = { DOCUMENT_PNAME, ITERATOR_PNAME, TRANSLET_OUTPUT_PNAME, "current" };
		string[] argNames = new string[] {DOCUMENT_PNAME, ITERATOR_PNAME, TRANSLET_OUTPUT_PNAME, "current"};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator buildKeys = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "buildKeys", _className, il, classGen.getConstantPool());
		MethodGenerator buildKeys = new MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "buildKeys", _className, il, classGen.getConstantPool());

		buildKeys.addException("org.apache.xalan.xsltc.TransletException");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration elements = elements();
		System.Collections.IEnumerator elements = this.elements();
		while (elements.MoveNext())
		{
			// xsl:key
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object element = elements.Current;
			object element = elements.Current;
			if (element is Key)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Key key = (Key)element;
			Key key = (Key)element;
			key.translate(classGen, buildKeys);
			_keys[key.Name] = key;
			}
		}

		il.append(RETURN);

		// Add method to class
			classGen.addMethod(buildKeys);

		return ("(" + DOM_INTF_SIG + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + "I)V");
		}

		/// <summary>
		/// Compile transform() into the output class. This method is used to 
		/// initialize global variables and global parameters. The current node
		/// is set to be the document's root node.
		/// </summary>
		private void compileTransform(ClassGenerator classGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();

		/* 
		 * Define the the method transform with the following signature:
		 * void transform(DOM, NodeIterator, HandlerBase)
		 */
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
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
		InstructionList il = new InstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.MethodGenerator transf = new org.apache.xalan.xsltc.compiler.util.MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "transform", _className, il, classGen.getConstantPool());
		MethodGenerator transf = new MethodGenerator(ACC_PUBLIC, org.apache.bcel.generic.Type.VOID, argTypes, argNames, "transform", _className, il, classGen.getConstantPool());
		transf.addException("org.apache.xalan.xsltc.TransletException");

		// Define and initialize current with the root node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen current = transf.addLocalVariable("current", org.apache.bcel.generic.Type.INT, null, null);
		LocalVariableGen current = transf.addLocalVariable("current", org.apache.bcel.generic.Type.INT, null, null);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String applyTemplatesSig = classGen.getApplyTemplatesSig();
		string applyTemplatesSig = classGen.ApplyTemplatesSig;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int applyTemplates = cpg.addMethodref(getClassName(), "applyTemplates", applyTemplatesSig);
		int applyTemplates = cpg.addMethodref(ClassName, "applyTemplates", applyTemplatesSig);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int domField = cpg.addFieldref(getClassName(), DOM_FIELD, DOM_INTF_SIG);
		int domField = cpg.addFieldref(ClassName, DOM_FIELD, DOM_INTF_SIG);

		// push translet for PUTFIELD
		il.append(classGen.loadTranslet());
		// prepare appropriate DOM implementation

		if (MultiDocument)
		{
			il.append(new NEW(cpg.addClass(MULTI_DOM_CLASS)));
			il.append(DUP);
		}

		il.append(classGen.loadTranslet());
		il.append(transf.loadDOM());
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(TRANSLET_CLASS, "makeDOMAdapter", "(" + DOM_INTF_SIG + ")" + DOM_ADAPTER_SIG)));
		// DOMAdapter is on the stack

		if (MultiDocument)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int init = cpg.addMethodref(MULTI_DOM_CLASS, "<init>", "("+DOM_INTF_SIG+")V");
			int init = cpg.addMethodref(MULTI_DOM_CLASS, "<init>", "(" + DOM_INTF_SIG + ")V");
			il.append(new INVOKESPECIAL(init));
			// MultiDOM is on the stack
		}

		//store to _dom variable
		il.append(new PUTFIELD(domField));

		// continue with globals initialization
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int gitr = cpg.addInterfaceMethodref(DOM_INTF, "getIterator", "()"+NODE_ITERATOR_SIG);
		int gitr = cpg.addInterfaceMethodref(DOM_INTF, "getIterator", "()" + NODE_ITERATOR_SIG);
		il.append(transf.loadDOM());
		il.append(new INVOKEINTERFACE(gitr, 1));
			il.append(transf.nextNode());
		current.setStart(il.append(new ISTORE(current.getIndex())));

		// Transfer the output settings to the output post-processor
		il.append(classGen.loadTranslet());
		il.append(transf.loadHandler());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = cpg.addMethodref(TRANSLET_CLASS, "transferOutputSettings", "("+OUTPUT_HANDLER_SIG+")V");
		int index = cpg.addMethodref(TRANSLET_CLASS, "transferOutputSettings", "(" + OUTPUT_HANDLER_SIG + ")V");
		il.append(new INVOKEVIRTUAL(index));

			/*
			 * Compile buildKeys() method. Note that this method is not 
			 * invoked here as keys for the input document are now created
			 * in topLevel(). However, this method is still needed by the
			 * LoadDocument class.
			 */        
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String keySig = compileBuildKeys(classGen);
			string keySig = compileBuildKeys(classGen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int keyIdx = cpg.addMethodref(getClassName(), "buildKeys", keySig);
			int keyIdx = cpg.addMethodref(ClassName, "buildKeys", keySig);

			// Look for top-level elements that need handling
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration toplevel = elements();
		System.Collections.IEnumerator toplevel = elements();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		if (_globals.Count > 0 || toplevel.hasMoreElements())
		{
			// Compile method for handling top-level elements
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String topLevelSig = compileTopLevel(classGen);
			string topLevelSig = compileTopLevel(classGen);
			// Get a reference to that method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int topLevelIdx = cpg.addMethodref(getClassName(), "topLevel", topLevelSig);
			int topLevelIdx = cpg.addMethodref(ClassName, "topLevel", topLevelSig);
			// Push all parameters on the stack and call topLevel()
			il.append(classGen.loadTranslet()); // The 'this' pointer
			il.append(classGen.loadTranslet());
			il.append(new GETFIELD(domField)); // The DOM reference
			il.append(transf.loadIterator());
			il.append(transf.loadHandler()); // The output handler
			il.append(new INVOKEVIRTUAL(topLevelIdx));
		}

		// start document
		il.append(transf.loadHandler());
		il.append(transf.startDocument());

		// push first arg for applyTemplates
		il.append(classGen.loadTranslet());
		// push translet for GETFIELD to get DOM arg
		il.append(classGen.loadTranslet());
		il.append(new GETFIELD(domField));
		// push remaining 2 args
		il.append(transf.loadIterator());
		il.append(transf.loadHandler());
		il.append(new INVOKEVIRTUAL(applyTemplates));
		// endDocument
		il.append(transf.loadHandler());
		il.append(transf.endDocument());

		il.append(RETURN);

		// Compute max locals + stack and add method to class
		classGen.addMethod(transf);
		}

		/// <summary>
		/// Peephole optimization: Remove sequences of [ALOAD, POP].
		/// </summary>
		private void peepHoleOptimization(MethodGenerator methodGen)
		{
		const string pattern = "`aload'`pop'`instruction'";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.util.InstructionFinder find = new org.apache.bcel.util.InstructionFinder(il);
		InstructionFinder find = new InstructionFinder(il);
		for (System.Collections.IEnumerator iter = find.search(pattern); iter.MoveNext();)
		{
			InstructionHandle[] match = (InstructionHandle[])iter.Current;
			try
			{
			il.delete(match[0], match[1]);
			}
			catch (TargetLostException)
			{
					// TODO: move target down into the list
			}
		}
		}

		public int addParam(Param param)
		{
		_globals.Add(param);
		return _globals.Count - 1;
		}

		public int addVariable(Variable global)
		{
		_globals.Add(global);
		return _globals.Count - 1;
		}

		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("Stylesheet");
		displayContents(indent + IndentIncrement);
		}

		// do we need this wrapper ?????
		public string getNamespace(string prefix)
		{
		return lookupNamespace(prefix);
		}

		public string ClassName
		{
			get
			{
			return _className;
			}
		}

		public ArrayList Templates
		{
			get
			{
			return _templates;
			}
		}

		public ArrayList AllValidTemplates
		{
			get
			{
				// Return templates if no imported/included stylesheets
				if (_includedStylesheets == null)
				{
					return _templates;
				}
    
				// Is returned value cached?
				if (_allValidTemplates == null)
				{
				   ArrayList templates = new ArrayList();
					int size = _includedStylesheets.Count;
					for (int i = 0; i < size; i++)
					{
						Stylesheet included = (Stylesheet)_includedStylesheets[i];
						templates.AddRange(included.AllValidTemplates);
					}
					templates.AddRange(_templates);
    
					// Cache results in top-level stylesheet only
					if (_parentStylesheet != null)
					{
						return templates;
					}
					_allValidTemplates = templates;
				}
    
				return _allValidTemplates;
			}
		}

		protected internal void addTemplate(Template template)
		{
			_templates.Add(template);
		}
	}

}