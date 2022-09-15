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
 * $Id: Parser.java 669374 2008-06-19 03:40:41Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using Symbol = java_cup.runtime.Symbol;

	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using AttributeList = org.apache.xalan.xsltc.runtime.AttributeList;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;
	using XMLReader = org.xml.sax.XMLReader;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	public class Parser : Constants, ContentHandler
	{

		private const string XSL = "xsl"; // standard prefix
		private const string TRANSLET = "translet"; // extension prefix

		private Locator _locator = null;

		private XSLTC _xsltc; // Reference to the compiler object.
		private XPathParser _xpathParser; // Reference to the XPath parser.
		private ArrayList _errors; // Contains all compilation errors
		private ArrayList _warnings; // Contains all compilation errors

		private Hashtable _instructionClasses; // Maps instructions to classes
		private Hashtable _instructionAttrs;
 // reqd and opt attrs
		private Hashtable _qNames;
		private Hashtable _namespaces;
		private QName _useAttributeSets;
		private QName _excludeResultPrefixes;
		private QName _extensionElementPrefixes;
		private Hashtable _variableScope;
		private Stylesheet _currentStylesheet;
		private SymbolTable _symbolTable; // Maps QNames to syntax-tree nodes
		private Output _output;
		private Template _template; // Reference to the template being parsed.

		private bool _rootNamespaceDef; // Used for validity check

		private SyntaxTreeNode _root;

		private string _target;

		private int _currentImportPrecedence;

		public Parser(XSLTC xsltc)
		{
		_xsltc = xsltc;
		}

		public virtual void init()
		{
		_qNames = new Hashtable(512);
		_namespaces = new Hashtable();
		_instructionClasses = new Hashtable();
		_instructionAttrs = new Hashtable();
		_variableScope = new Hashtable();
		_template = null;
		_errors = new ArrayList();
		_warnings = new ArrayList();
		_symbolTable = new SymbolTable();
		_xpathParser = new XPathParser(this);
		_currentStylesheet = null;
			_output = null;
			_root = null;
			_rootNamespaceDef = false;
		_currentImportPrecedence = 1;

		initStdClasses();
		initInstructionAttrs();
		initExtClasses();
		initSymbolTable();

		_useAttributeSets = getQName(Constants_Fields.XSLT_URI, XSL, "use-attribute-sets");
		_excludeResultPrefixes = getQName(Constants_Fields.XSLT_URI, XSL, "exclude-result-prefixes");
		_extensionElementPrefixes = getQName(Constants_Fields.XSLT_URI, XSL, "extension-element-prefixes");
		}

		public virtual Output Output
		{
			set
			{
			if (_output != null)
			{
				if (_output.ImportPrecedence <= value.ImportPrecedence)
				{
				string cdata = _output.Cdata;
						value.mergeOutput(_output);
				_output.disable();
				_output = value;
				}
				else
				{
				value.disable();
				}
			}
			else
			{
				_output = value;
			}
			}
			get
			{
			return _output;
			}
		}


		public virtual Properties OutputProperties
		{
			get
			{
			return TopLevelStylesheet.OutputProperties;
			}
		}

		public virtual void addVariable(Variable @var)
		{
		addVariableOrParam(@var);
		}

		public virtual void addParameter(Param param)
		{
		addVariableOrParam(param);
		}

		private void addVariableOrParam(VariableBase @var)
		{
		object existing = _variableScope[@var.Name];
		if (existing != null)
		{
			if (existing is Stack)
			{
			Stack stack = (Stack)existing;
			stack.Push(@var);
			}
			else if (existing is VariableBase)
			{
			Stack stack = new Stack();
			stack.Push(existing);
			stack.Push(@var);
			_variableScope[@var.Name] = stack;
			}
		}
		else
		{
			_variableScope[@var.Name] = @var;
		}
		}

		public virtual void removeVariable(QName name)
		{
		object existing = _variableScope[name];
		if (existing is Stack)
		{
			Stack stack = (Stack)existing;
			if (stack.Count > 0)
			{
				stack.Pop();
			}
			if (stack.Count > 0)
			{
				return;
			}
		}
		_variableScope.Remove(name);
		}

		public virtual VariableBase lookupVariable(QName name)
		{
		object existing = _variableScope[name];
		if (existing is VariableBase)
		{
			return ((VariableBase)existing);
		}
		else if (existing is Stack)
		{
			Stack stack = (Stack)existing;
			return ((VariableBase)stack.Peek());
		}
		return (null);
		}

		public virtual XSLTC XSLTC
		{
			set
			{
			_xsltc = value;
			}
			get
			{
			return _xsltc;
			}
		}


		public virtual int CurrentImportPrecedence
		{
			get
			{
			return _currentImportPrecedence;
			}
		}

		public virtual int NextImportPrecedence
		{
			get
			{
			return ++_currentImportPrecedence;
			}
		}

		public virtual Stylesheet CurrentStylesheet
		{
			set
			{
			_currentStylesheet = value;
			}
			get
			{
			return _currentStylesheet;
			}
		}


		public virtual Stylesheet TopLevelStylesheet
		{
			get
			{
			return _xsltc.Stylesheet;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public QName getQNameSafe(final String stringRep)
		public virtual QName getQNameSafe(string stringRep)
		{
		// parse and retrieve namespace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = stringRep.lastIndexOf(':');
		int colon = stringRep.LastIndexOf(':');
		if (colon != -1)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = stringRep.substring(0, colon);
			string prefix = stringRep.Substring(0, colon);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localname = stringRep.substring(colon + 1);
			string localname = stringRep.Substring(colon + 1);
			string @namespace = null;

			// Get the namespace uri from the symbol table
			if (prefix.Equals(Constants_Fields.XMLNS_PREFIX) == false)
			{
			@namespace = _symbolTable.lookupNamespace(prefix);
			if (string.ReferenceEquals(@namespace, null))
			{
				@namespace = Constants_Fields.EMPTYSTRING;
			}
			}
			return getQName(@namespace, prefix, localname);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = stringRep.equals(Constants_Fields.XMLNS_PREFIX) ? null : _symbolTable.lookupNamespace(Constants_Fields.EMPTYSTRING);
			string uri = stringRep.Equals(Constants_Fields.XMLNS_PREFIX) ? null : _symbolTable.lookupNamespace(Constants_Fields.EMPTYSTRING);
			return getQName(uri, null, stringRep);
		}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public QName getQName(final String stringRep)
		public virtual QName getQName(string stringRep)
		{
		return getQName(stringRep, true, false);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public QName getQNameIgnoreDefaultNs(final String stringRep)
		public virtual QName getQNameIgnoreDefaultNs(string stringRep)
		{
		return getQName(stringRep, true, true);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public QName getQName(final String stringRep, boolean reportError)
		public virtual QName getQName(string stringRep, bool reportError)
		{
		return getQName(stringRep, reportError, false);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private QName getQName(final String stringRep, boolean reportError, boolean ignoreDefaultNs)
		private QName getQName(string stringRep, bool reportError, bool ignoreDefaultNs)
		{
		// parse and retrieve namespace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colon = stringRep.lastIndexOf(':');
		int colon = stringRep.LastIndexOf(':');
		if (colon != -1)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = stringRep.substring(0, colon);
			string prefix = stringRep.Substring(0, colon);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String localname = stringRep.substring(colon + 1);
			string localname = stringRep.Substring(colon + 1);
			string @namespace = null;

			// Get the namespace uri from the symbol table
			if (prefix.Equals(Constants_Fields.XMLNS_PREFIX) == false)
			{
			@namespace = _symbolTable.lookupNamespace(prefix);
			if (string.ReferenceEquals(@namespace, null) && reportError)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int line = getLineNumber();
				int line = LineNumber;
				ErrorMsg err = new ErrorMsg(ErrorMsg.NAMESPACE_UNDEF_ERR, line, prefix);
				reportError(Constants_Fields.ERROR, err);
			}
			}
			return getQName(@namespace, prefix, localname);
		}
		else
		{
			if (stringRep.Equals(Constants_Fields.XMLNS_PREFIX))
			{
			ignoreDefaultNs = true;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String defURI = ignoreDefaultNs ? null : _symbolTable.lookupNamespace(Constants_Fields.EMPTYSTRING);
			string defURI = ignoreDefaultNs ? null : _symbolTable.lookupNamespace(Constants_Fields.EMPTYSTRING);
			return getQName(defURI, null, stringRep);
		}
		}

		public virtual QName getQName(string @namespace, string prefix, string localname)
		{
		if (string.ReferenceEquals(@namespace, null) || @namespace.Equals(Constants_Fields.EMPTYSTRING))
		{
			QName name = (QName)_qNames[localname];
			if (name == null)
			{
			name = new QName(null, prefix, localname);
			_qNames[localname] = name;
			}
			return name;
		}
		else
		{
			Dictionary space = (Dictionary)_namespaces[@namespace];
			string lexicalQName = (string.ReferenceEquals(prefix, null) || prefix.Length == 0) ? localname : (prefix + ':' + localname);

			if (space == null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName name = new QName(namespace, prefix, localname);
			QName name = new QName(@namespace, prefix, localname);
			_namespaces[@namespace] = space = new Hashtable();
			space.put(lexicalQName, name);
			return name;
			}
			else
			{
				QName name = (QName)space.get(lexicalQName);

				if (name == null)
				{
				name = new QName(@namespace, prefix, localname);
				space.put(lexicalQName, name);
				}
			return name;
			}
		}
		}

		public virtual QName getQName(string scope, string name)
		{
		return getQName(scope + name);
		}

		public virtual QName getQName(QName scope, QName name)
		{
		return getQName(scope.ToString() + name.ToString());
		}

		public virtual QName UseAttributeSets
		{
			get
			{
			return _useAttributeSets;
			}
		}

		public virtual QName ExtensionElementPrefixes
		{
			get
			{
			return _extensionElementPrefixes;
			}
		}

		public virtual QName ExcludeResultPrefixes
		{
			get
			{
			return _excludeResultPrefixes;
			}
		}

		/// <summary>
		/// Create an instance of the <code>Stylesheet</code> class,
		/// and then parse, typecheck and compile the instance.
		/// Must be called after <code>parse()</code>.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Stylesheet makeStylesheet(SyntaxTreeNode element) throws CompilerException
		public virtual Stylesheet makeStylesheet(SyntaxTreeNode element)
		{
		try
		{
			Stylesheet stylesheet;

			if (element is Stylesheet)
			{
			stylesheet = (Stylesheet)element;
			}
			else
			{
			stylesheet = new Stylesheet();
			stylesheet.setSimplified();
			stylesheet.addElement(element);
			stylesheet.Attributes = (AttributeList) element.getAttributes();

			// Map the default NS if not already defined
			if (string.ReferenceEquals(element.lookupNamespace(Constants_Fields.EMPTYSTRING), null))
			{
				element.addPrefixMapping(Constants_Fields.EMPTYSTRING, Constants_Fields.EMPTYSTRING);
			}
			}
			stylesheet.Parser = this;
			return stylesheet;
		}
		catch (System.InvalidCastException)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.NOT_STYLESHEET_ERR, element);
			throw new CompilerException(err.ToString());
		}
		}

		/// <summary>
		/// Instanciates a SAX2 parser and generate the AST from the input.
		/// </summary>
		public virtual void createAST(Stylesheet stylesheet)
		{
		try
		{
			if (stylesheet != null)
			{
			stylesheet.parseContents(this);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int precedence = stylesheet.getImportPrecedence();
			int precedence = stylesheet.ImportPrecedence;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration elements = stylesheet.elements();
			System.Collections.IEnumerator elements = stylesheet.elements();
			while (elements.MoveNext())
			{
				object child = elements.Current;
				if (child is Text)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int l = getLineNumber();
				int l = LineNumber;
				ErrorMsg err = new ErrorMsg(ErrorMsg.ILLEGAL_TEXT_NODE_ERR,l,null);
				reportError(Constants_Fields.ERROR, err);
				}
			}
			if (!errorsFound())
			{
				stylesheet.typeCheck(_symbolTable);
			}
			}
		}
		catch (TypeCheckError e)
		{
			reportError(Constants_Fields.ERROR, new ErrorMsg(e));
		}
		}

		/// <summary>
		/// Parses a stylesheet and builds the internal abstract syntax tree </summary>
		/// <param name="reader"> A SAX2 SAXReader (parser) </param>
		/// <param name="input"> A SAX2 InputSource can be passed to a SAX reader </param>
		/// <returns> The root of the abstract syntax tree </returns>
		public virtual SyntaxTreeNode parse(XMLReader reader, InputSource input)
		{
		try
		{
			// Parse the input document and build the abstract syntax tree
			reader.ContentHandler = this;
			reader.parse(input);
			// Find the start of the stylesheet within the tree
			return (SyntaxTreeNode)getStylesheet(_root);
		}
		catch (IOException e)
		{
			if (_xsltc.debug())
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			reportError(Constants_Fields.ERROR,new ErrorMsg(e));
		}
		catch (SAXException e)
		{
			Exception ex = e.Exception;
			if (_xsltc.debug())
			{
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
			if (ex != null)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
			}
			reportError(Constants_Fields.ERROR, new ErrorMsg(e));
		}
		catch (CompilerException e)
		{
			if (_xsltc.debug())
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			reportError(Constants_Fields.ERROR, new ErrorMsg(e));
		}
		catch (Exception e)
		{
			if (_xsltc.debug())
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			reportError(Constants_Fields.ERROR, new ErrorMsg(e));
		}
		return null;
		}

		/// <summary>
		/// Parses a stylesheet and builds the internal abstract syntax tree </summary>
		/// <param name="input"> A SAX2 InputSource can be passed to a SAX reader </param>
		/// <returns> The root of the abstract syntax tree </returns>
		public virtual SyntaxTreeNode parse(InputSource input)
		{
		try
		{
			// Create a SAX parser and get the XMLReader object it uses
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();
			SAXParserFactory factory = SAXParserFactory.newInstance();

			if (_xsltc.SecureProcessing)
			{
				try
				{
					factory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
				}
				catch (SAXException)
				{
				}
			}

			try
			{
			factory.setFeature(Constants_Fields.NAMESPACE_FEATURE,true);
			}
			catch (Exception)
			{
			factory.NamespaceAware = true;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.SAXParser parser = factory.newSAXParser();
			SAXParser parser = factory.newSAXParser();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.XMLReader reader = parser.getXMLReader();
			XMLReader reader = parser.XMLReader;
			return (parse(reader, input));
		}
		catch (ParserConfigurationException)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.SAX_PARSER_CONFIG_ERR);
			reportError(Constants_Fields.ERROR, err);
		}
		catch (SAXParseException e)
		{
			reportError(Constants_Fields.ERROR, new ErrorMsg(e.Message,e.LineNumber));
		}
		catch (SAXException e)
		{
			reportError(Constants_Fields.ERROR, new ErrorMsg(e.Message));
		}
		return null;
		}

		public virtual SyntaxTreeNode DocumentRoot
		{
			get
			{
			return _root;
			}
		}

		private string _PImedia = null;
		private string _PItitle = null;
		private string _PIcharset = null;

		/// <summary>
		/// Set the parameters to use to locate the correct <?xml-stylesheet ...?>
		/// processing instruction in the case where the input document is an
		/// XML document with one or more references to a stylesheet. </summary>
		/// <param name="media"> The media attribute to be matched. May be null, in which
		/// case the prefered templates will be used (i.e. alternate = no). </param>
		/// <param name="title"> The value of the title attribute to match. May be null. </param>
		/// <param name="charset"> The value of the charset attribute to match. May be null. </param>
		protected internal virtual void setPIParameters(string media, string title, string charset)
		{
		_PImedia = media;
		_PItitle = title;
		_PIcharset = charset;
		}

		/// <summary>
		/// Extracts the DOM for the stylesheet. In the case of an embedded
		/// stylesheet, it extracts the DOM subtree corresponding to the 
		/// embedded stylesheet that has an 'id' attribute whose value is the
		/// same as the value declared in the <?xml-stylesheet...?> processing 
		/// instruction (P.I.). In the xml-stylesheet P.I. the value is labeled
		/// as the 'href' data of the P.I. The extracted DOM representing the
		/// stylesheet is returned as an Element object.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private SyntaxTreeNode getStylesheet(SyntaxTreeNode root) throws CompilerException
		private SyntaxTreeNode getStylesheet(SyntaxTreeNode root)
		{

		// Assume that this is a pure XSL stylesheet if there is not
		// <?xml-stylesheet ....?> processing instruction
		if (string.ReferenceEquals(_target, null))
		{
			if (!_rootNamespaceDef)
			{
			ErrorMsg msg = new ErrorMsg(ErrorMsg.MISSING_XSLT_URI_ERR);
			throw new CompilerException(msg.ToString());
			}
			return (root);
		}

		// Find the xsl:stylesheet or xsl:transform with this reference
		if (_target[0] == '#')
		{
			SyntaxTreeNode element = findStylesheet(root, _target.Substring(1));
			if (element == null)
			{
			ErrorMsg msg = new ErrorMsg(ErrorMsg.MISSING_XSLT_TARGET_ERR, _target, root);
			throw new CompilerException(msg.ToString());
			}
			return (element);
		}
		else
		{
			return (loadExternalStylesheet(_target));
		}
		}

		/// <summary>
		/// Find a Stylesheet element with a specific ID attribute value.
		/// This method is used to find a Stylesheet node that is referred
		/// in a <?xml-stylesheet ... ?> processing instruction.
		/// </summary>
		private SyntaxTreeNode findStylesheet(SyntaxTreeNode root, string href)
		{

		if (root == null)
		{
			return null;
		}

		if (root is Stylesheet)
		{
			string id = root.getAttribute("id");
			if (id.Equals(href))
			{
				return root;
			}
		}
		ArrayList children = root.Contents;
		if (children != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = children.size();
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
			SyntaxTreeNode child = (SyntaxTreeNode)children[i];
			SyntaxTreeNode node = findStylesheet(child, href);
			if (node != null)
			{
				return node;
			}
			}
		}
		return null;
		}

		/// <summary>
		/// For embedded stylesheets: Load an external file with stylesheet
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private SyntaxTreeNode loadExternalStylesheet(String location) throws CompilerException
		private SyntaxTreeNode loadExternalStylesheet(string location)
		{

		InputSource source;

		// Check if the location is URL or a local file
		if (System.IO.Directory.Exists(location) || System.IO.File.Exists(location))
		{
			source = new InputSource("file:" + location);
		}
		else
		{
			source = new InputSource(location);
		}

		SyntaxTreeNode external = (SyntaxTreeNode)parse(source);
		return (external);
		}

		private void initAttrTable(string elementName, string[] attrs)
		{
		_instructionAttrs[getQName(Constants_Fields.XSLT_URI, XSL, elementName)] = attrs;
		}

		private void initInstructionAttrs()
		{
		initAttrTable("template", new string[] {"match", "name", "priority", "mode"});
		initAttrTable("stylesheet", new string[] {"id", "version", "extension-element-prefixes", "exclude-result-prefixes"});
		initAttrTable("transform", new string[] {"id", "version", "extension-element-prefixes", "exclude-result-prefixes"});
		initAttrTable("text", new string[] {"disable-output-escaping"});
		initAttrTable("if", new string[] {"test"});
		initAttrTable("choose", new string[] {});
		initAttrTable("when", new string[] {"test"});
		initAttrTable("otherwise", new string[] {});
		initAttrTable("for-each", new string[] {"select"});
		initAttrTable("message", new string[] {"terminate"});
		initAttrTable("number", new string[] {"level", "count", "from", "value", "format", "lang", "letter-value", "grouping-separator", "grouping-size"});
			initAttrTable("comment", new string[] {});
		initAttrTable("copy", new string[] {"use-attribute-sets"});
		initAttrTable("copy-of", new string[] {"select"});
		initAttrTable("param", new string[] {"name", "select"});
		initAttrTable("with-param", new string[] {"name", "select"});
		initAttrTable("variable", new string[] {"name", "select"});
		initAttrTable("output", new string[] {"method", "version", "encoding", "omit-xml-declaration", "standalone", "doctype-public", "doctype-system", "cdata-section-elements", "indent", "media-type"});
		initAttrTable("sort", new string[] {"select", "order", "case-order", "lang", "data-type"});
		initAttrTable("key", new string[] {"name", "match", "use"});
		initAttrTable("fallback", new string[] {});
		initAttrTable("attribute", new string[] {"name", "namespace"});
		initAttrTable("attribute-set", new string[] {"name", "use-attribute-sets"});
		initAttrTable("value-of", new string[] {"select", "disable-output-escaping"});
		initAttrTable("element", new string[] {"name", "namespace", "use-attribute-sets"});
		initAttrTable("call-template", new string[] {"name"});
		initAttrTable("apply-templates", new string[] {"select", "mode"});
		initAttrTable("apply-imports", new string[] {});
		initAttrTable("decimal-format", new string[] {"name", "decimal-separator", "grouping-separator", "infinity", "minus-sign", "NaN", "percent", "per-mille", "zero-digit", "digit", "pattern-separator"});
		initAttrTable("import", new string[] {"href"});
		initAttrTable("include", new string[] {"href"});
		initAttrTable("strip-space", new string[] {"elements"});
		initAttrTable("preserve-space", new string[] {"elements"});
		initAttrTable("processing-instruction", new string[] {"name"});
		initAttrTable("namespace-alias", new string[] {"stylesheet-prefix", "result-prefix"});
		}



		/// <summary>
		/// Initialize the _instructionClasses Hashtable, which maps XSL element
		/// names to Java classes in this package.
		/// </summary>
		private void initStdClasses()
		{
		initStdClass("template", "Template");
		initStdClass("stylesheet", "Stylesheet");
		initStdClass("transform", "Stylesheet");
		initStdClass("text", "Text");
		initStdClass("if", "If");
		initStdClass("choose", "Choose");
		initStdClass("when", "When");
		initStdClass("otherwise", "Otherwise");
		initStdClass("for-each", "ForEach");
		initStdClass("message", "Message");
		initStdClass("number", "Number");
		initStdClass("comment", "Comment");
		initStdClass("copy", "Copy");
		initStdClass("copy-of", "CopyOf");
		initStdClass("param", "Param");
		initStdClass("with-param", "WithParam");
		initStdClass("variable", "Variable");
		initStdClass("output", "Output");
		initStdClass("sort", "Sort");
		initStdClass("key", "Key");
		initStdClass("fallback", "Fallback");
		initStdClass("attribute", "XslAttribute");
		initStdClass("attribute-set", "AttributeSet");
		initStdClass("value-of", "ValueOf");
		initStdClass("element", "XslElement");
		initStdClass("call-template", "CallTemplate");
		initStdClass("apply-templates", "ApplyTemplates");
		initStdClass("apply-imports", "ApplyImports");
		initStdClass("decimal-format", "DecimalFormatting");
		initStdClass("import", "Import");
		initStdClass("include", "Include");
		initStdClass("strip-space", "Whitespace");
		initStdClass("preserve-space", "Whitespace");
		initStdClass("processing-instruction", "ProcessingInstruction");
		initStdClass("namespace-alias", "NamespaceAlias");
		}

		private void initStdClass(string elementName, string className)
		{
		_instructionClasses[getQName(Constants_Fields.XSLT_URI, XSL, elementName)] = Constants_Fields.COMPILER_PACKAGE + '.' + className;
		}

		public virtual bool elementSupported(string @namespace, string localName)
		{
		return (_instructionClasses[getQName(@namespace, XSL, localName)] != null);
		}

		public virtual bool functionSupported(string fname)
		{
		return (_symbolTable.lookupPrimop(fname) != null);
		}

		private void initExtClasses()
		{
		initExtClass("output", "TransletOutput");
			initExtClass(Constants_Fields.REDIRECT_URI, "write", "TransletOutput");
		}

		private void initExtClass(string elementName, string className)
		{
		_instructionClasses[getQName(Constants_Fields.TRANSLET_URI, TRANSLET, elementName)] = Constants_Fields.COMPILER_PACKAGE + '.' + className;
		}

		private void initExtClass(string @namespace, string elementName, string className)
		{
			_instructionClasses[getQName(@namespace, TRANSLET, elementName)] = Constants_Fields.COMPILER_PACKAGE + '.' + className;
		}

		/// <summary>
		/// Add primops and base functions to the symbol table.
		/// </summary>
		private void initSymbolTable()
		{
		MethodType I_V = new MethodType(Type.Int, Type.Void);
		MethodType I_R = new MethodType(Type.Int, Type.Real);
		MethodType I_S = new MethodType(Type.Int, Type.String);
		MethodType I_D = new MethodType(Type.Int, Type.NodeSet);
		MethodType R_I = new MethodType(Type.Real, Type.Int);
		MethodType R_V = new MethodType(Type.Real, Type.Void);
		MethodType R_R = new MethodType(Type.Real, Type.Real);
		MethodType R_D = new MethodType(Type.Real, Type.NodeSet);
		MethodType R_O = new MethodType(Type.Real, Type.Reference);
		MethodType I_I = new MethodType(Type.Int, Type.Int);
		 MethodType D_O = new MethodType(Type.NodeSet, Type.Reference);
		MethodType D_V = new MethodType(Type.NodeSet, Type.Void);
		MethodType D_S = new MethodType(Type.NodeSet, Type.String);
		MethodType D_D = new MethodType(Type.NodeSet, Type.NodeSet);
		MethodType A_V = new MethodType(Type.Node, Type.Void);
		MethodType S_V = new MethodType(Type.String, Type.Void);
		MethodType S_S = new MethodType(Type.String, Type.String);
		MethodType S_A = new MethodType(Type.String, Type.Node);
		MethodType S_D = new MethodType(Type.String, Type.NodeSet);
		MethodType S_O = new MethodType(Type.String, Type.Reference);
		MethodType B_O = new MethodType(Type.Boolean, Type.Reference);
		MethodType B_V = new MethodType(Type.Boolean, Type.Void);
		MethodType B_B = new MethodType(Type.Boolean, Type.Boolean);
		MethodType B_S = new MethodType(Type.Boolean, Type.String);
		MethodType D_X = new MethodType(Type.NodeSet, Type.Object);
		MethodType R_RR = new MethodType(Type.Real, Type.Real, Type.Real);
		MethodType I_II = new MethodType(Type.Int, Type.Int, Type.Int);
		MethodType B_RR = new MethodType(Type.Boolean, Type.Real, Type.Real);
		MethodType B_II = new MethodType(Type.Boolean, Type.Int, Type.Int);
		MethodType S_SS = new MethodType(Type.String, Type.String, Type.String);
		MethodType S_DS = new MethodType(Type.String, Type.Real, Type.String);
		MethodType S_SR = new MethodType(Type.String, Type.String, Type.Real);
		MethodType O_SO = new MethodType(Type.Reference, Type.String, Type.Reference);

		MethodType D_SS = new MethodType(Type.NodeSet, Type.String, Type.String);
		MethodType D_SD = new MethodType(Type.NodeSet, Type.String, Type.NodeSet);
		MethodType B_BB = new MethodType(Type.Boolean, Type.Boolean, Type.Boolean);
		MethodType B_SS = new MethodType(Type.Boolean, Type.String, Type.String);
		MethodType S_SD = new MethodType(Type.String, Type.String, Type.NodeSet);
		MethodType S_DSS = new MethodType(Type.String, Type.Real, Type.String, Type.String);
		MethodType S_SRR = new MethodType(Type.String, Type.String, Type.Real, Type.Real);
		MethodType S_SSS = new MethodType(Type.String, Type.String, Type.String, Type.String);

		/*
		 * Standard functions: implemented but not in this table concat().
		 * When adding a new function make sure to uncomment
		 * the corresponding line in <tt>FunctionAvailableCall</tt>.
		 */

		// The following functions are inlined

		_symbolTable.addPrimop("current", A_V);
		_symbolTable.addPrimop("last", I_V);
		_symbolTable.addPrimop("position", I_V);
		_symbolTable.addPrimop("true", B_V);
		_symbolTable.addPrimop("false", B_V);
		_symbolTable.addPrimop("not", B_B);
		_symbolTable.addPrimop("name", S_V);
		_symbolTable.addPrimop("name", S_A);
		_symbolTable.addPrimop("generate-id", S_V);
		_symbolTable.addPrimop("generate-id", S_A);
		_symbolTable.addPrimop("ceiling", R_R);
		_symbolTable.addPrimop("floor", R_R);
		_symbolTable.addPrimop("round", R_R);
		_symbolTable.addPrimop("contains", B_SS);
		_symbolTable.addPrimop("number", R_O);
		_symbolTable.addPrimop("number", R_V);
		_symbolTable.addPrimop("boolean", B_O);
		_symbolTable.addPrimop("string", S_O);
		_symbolTable.addPrimop("string", S_V);
		_symbolTable.addPrimop("translate", S_SSS);
		_symbolTable.addPrimop("string-length", I_V);
		_symbolTable.addPrimop("string-length", I_S);
		_symbolTable.addPrimop("starts-with", B_SS);
		_symbolTable.addPrimop("format-number", S_DS);
		_symbolTable.addPrimop("format-number", S_DSS);
		_symbolTable.addPrimop("unparsed-entity-uri", S_S);
		_symbolTable.addPrimop("key", D_SS);
		_symbolTable.addPrimop("key", D_SD);
		_symbolTable.addPrimop("id", D_S);
		_symbolTable.addPrimop("id", D_D);
		_symbolTable.addPrimop("namespace-uri", S_V);
		_symbolTable.addPrimop("function-available", B_S);
		_symbolTable.addPrimop("element-available", B_S);
		_symbolTable.addPrimop("document", D_S);
		_symbolTable.addPrimop("document", D_V);

		// The following functions are implemented in the basis library
		_symbolTable.addPrimop("count", I_D);
		_symbolTable.addPrimop("sum", R_D);
		_symbolTable.addPrimop("local-name", S_V);
		_symbolTable.addPrimop("local-name", S_D);
		_symbolTable.addPrimop("namespace-uri", S_V);
		_symbolTable.addPrimop("namespace-uri", S_D);
		_symbolTable.addPrimop("substring", S_SR);
		_symbolTable.addPrimop("substring", S_SRR);
		_symbolTable.addPrimop("substring-after", S_SS);
		_symbolTable.addPrimop("substring-before", S_SS);
		_symbolTable.addPrimop("normalize-space", S_V);
		_symbolTable.addPrimop("normalize-space", S_S);
		_symbolTable.addPrimop("system-property", S_S);

		// Extensions
			_symbolTable.addPrimop("nodeset", D_O);
			_symbolTable.addPrimop("objectType", S_O);
			_symbolTable.addPrimop("cast", O_SO);

		// Operators +, -, *, /, % defined on real types.
		_symbolTable.addPrimop("+", R_RR);
		_symbolTable.addPrimop("-", R_RR);
		_symbolTable.addPrimop("*", R_RR);
		_symbolTable.addPrimop("/", R_RR);
		_symbolTable.addPrimop("%", R_RR);

		// Operators +, -, * defined on integer types.
		// Operators / and % are not  defined on integers (may cause exception)
		_symbolTable.addPrimop("+", I_II);
		_symbolTable.addPrimop("-", I_II);
		_symbolTable.addPrimop("*", I_II);

		 // Operators <, <= >, >= defined on real types.
		_symbolTable.addPrimop("<", B_RR);
		_symbolTable.addPrimop("<=", B_RR);
		_symbolTable.addPrimop(">", B_RR);
		_symbolTable.addPrimop(">=", B_RR);

		// Operators <, <= >, >= defined on int types.
		_symbolTable.addPrimop("<", B_II);
		_symbolTable.addPrimop("<=", B_II);
		_symbolTable.addPrimop(">", B_II);
		_symbolTable.addPrimop(">=", B_II);

		// Operators <, <= >, >= defined on boolean types.
		_symbolTable.addPrimop("<", B_BB);
		_symbolTable.addPrimop("<=", B_BB);
		_symbolTable.addPrimop(">", B_BB);
		_symbolTable.addPrimop(">=", B_BB);

		// Operators 'and' and 'or'.
		_symbolTable.addPrimop("or", B_BB);
		_symbolTable.addPrimop("and", B_BB);

		// Unary minus.
		_symbolTable.addPrimop("u-", R_R);
		_symbolTable.addPrimop("u-", I_I);
		}

		public virtual SymbolTable SymbolTable
		{
			get
			{
			return _symbolTable;
			}
		}

		public virtual Template Template
		{
			get
			{
			return _template;
			}
			set
			{
			_template = value;
			}
		}


		private int _templateIndex = 0;

		public virtual int TemplateIndex
		{
			get
			{
			return (_templateIndex++);
			}
		}

		/// <summary>
		/// Creates a new node in the abstract syntax tree. This node can be
		///  o) a supported XSLT 1.0 element
		///  o) an unsupported XSLT element (post 1.0)
		///  o) a supported XSLT extension
		///  o) an unsupported XSLT extension
		///  o) a literal result element (not an XSLT element and not an extension)
		/// Unsupported elements do not directly generate an error. We have to wait
		/// until we have received all child elements of an unsupported element to
		/// see if any <xsl:fallback> elements exist.
		/// </summary>

		private bool versionIsOne = true;

		public virtual SyntaxTreeNode makeInstance(string uri, string prefix, string local, Attributes attributes)
		{
		SyntaxTreeNode node = null;
		QName qname = getQName(uri, prefix, local);
		string className = (string)_instructionClasses[qname];

		if (!string.ReferenceEquals(className, null))
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class clazz = ObjectFactory.findProviderClass(className, ObjectFactory.findClassLoader(), true);
			Type clazz = ObjectFactory.findProviderClass(className, ObjectFactory.findClassLoader(), true);
			node = (SyntaxTreeNode)clazz.newInstance();
			node.QName = qname;
			node.Parser = this;
			if (_locator != null)
			{
				node.LineNumber = LineNumber;
			}
			if (node is Stylesheet)
			{
				_xsltc.Stylesheet = (Stylesheet)node;
			}
			checkForSuperfluousAttributes(node, attributes);
			}
			catch (ClassNotFoundException)
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR, node);
			reportError(Constants_Fields.ERROR, err);
			}
			catch (Exception e)
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.INTERNAL_ERR, e.Message, node);
			reportError(Constants_Fields.FATAL, err);
			}
		}
		else
		{
			if (!string.ReferenceEquals(uri, null))
			{
			// Check if the element belongs in our namespace
			if (uri.Equals(Constants_Fields.XSLT_URI))
			{
				node = new UnsupportedElement(uri, prefix, local, false);
				UnsupportedElement element = (UnsupportedElement)node;
				ErrorMsg msg = new ErrorMsg(ErrorMsg.UNSUPPORTED_XSL_ERR, LineNumber,local);
				element.ErrorMessage = msg;
				if (versionIsOne)
				{
					reportError(Constants_Fields.UNSUPPORTED,msg);
				}
			}
			// Check if this is an XSLTC extension element
			else if (uri.Equals(Constants_Fields.TRANSLET_URI))
			{
				node = new UnsupportedElement(uri, prefix, local, true);
				UnsupportedElement element = (UnsupportedElement)node;
				ErrorMsg msg = new ErrorMsg(ErrorMsg.UNSUPPORTED_EXT_ERR, LineNumber,local);
				element.ErrorMessage = msg;
			}
			// Check if this is an extension of some other XSLT processor
			else
			{
				Stylesheet sheet = _xsltc.Stylesheet;
				if ((sheet != null) && (sheet.isExtension(uri)))
				{
				if (sheet != (SyntaxTreeNode)_parentStack.Peek())
				{
					node = new UnsupportedElement(uri, prefix, local, true);
					UnsupportedElement elem = (UnsupportedElement)node;
					ErrorMsg msg = new ErrorMsg(ErrorMsg.UNSUPPORTED_EXT_ERR, LineNumber, prefix + ":" + local);
					elem.ErrorMessage = msg;
				}
				}
			}
			}
			if (node == null)
			{
					node = new LiteralElement();
					node.LineNumber = LineNumber;
			}
				node.Parser = this;
		}
		if ((node != null) && (node is LiteralElement))
		{
			((LiteralElement)node).QName = qname;
		}
		return (node);
		}

		/// <summary>
		/// checks the list of attributes against a list of allowed attributes
		/// for a particular element node.
		/// </summary>
		private void checkForSuperfluousAttributes(SyntaxTreeNode node, Attributes attrs)
		{
		QName qname = node.QName;
		bool isStylesheet = (node is Stylesheet);
			string[] legal = (string[]) _instructionAttrs[qname];
		if (versionIsOne && legal != null)
		{
			int j;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = attrs.getLength();
			int n = attrs.Length;

			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String attrQName = attrs.getQName(i);
				string attrQName = attrs.getQName(i);

				if (isStylesheet && attrQName.Equals("version"))
				{
					versionIsOne = attrs.getValue(i).Equals("1.0");
				}

			// Ignore if special or if it has a prefix
				if (attrQName.StartsWith("xml", StringComparison.Ordinal) || attrQName.IndexOf(':') > 0)
				{
					continue;
				}

				for (j = 0; j < legal.Length; j++)
				{
					if (attrQName.Equals(legal[j], StringComparison.CurrentCultureIgnoreCase))
					{
					break;
					}
				}
				if (j == legal.Length)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.ErrorMsg err = new org.apache.xalan.xsltc.compiler.util.ErrorMsg(org.apache.xalan.xsltc.compiler.util.ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, attrQName, node);
					ErrorMsg err = new ErrorMsg(ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, attrQName, node);
				// Workaround for the TCK failure ErrorListener.errorTests.error001..
					err.WarningError = true;
				reportError(Constants_Fields.WARNING, err);
				}
			}
		}
		}


		/// <summary>
		/// Parse an XPath expression: </summary>
		///  <param name="parent"> - XSL element where the expression occured </param>
		///  <param name="exp">    - textual representation of the expression </param>
		public virtual Expression parseExpression(SyntaxTreeNode parent, string exp)
		{
		return (Expression)parseTopLevel(parent, "<EXPRESSION>" + exp, null);
		}

		/// <summary>
		/// Parse an XPath expression: </summary>
		///  <param name="parent"> - XSL element where the expression occured </param>
		///  <param name="attr">   - name of this element's attribute to get expression from </param>
		///  <param name="def">    - default expression (if the attribute was not found) </param>
		public virtual Expression parseExpression(SyntaxTreeNode parent, string attr, string def)
		{
		// Get the textual representation of the expression (if any)
			string exp = parent.getAttribute(attr);
		// Use the default expression if none was found
			if ((exp.Length == 0) && (!string.ReferenceEquals(def, null)))
			{
				exp = def;
			}
		// Invoke the XPath parser
			return (Expression)parseTopLevel(parent, "<EXPRESSION>" + exp, exp);
		}

		/// <summary>
		/// Parse an XPath pattern: </summary>
		///  <param name="parent">  - XSL element where the pattern occured </param>
		///  <param name="pattern"> - textual representation of the pattern </param>
		public virtual Pattern parsePattern(SyntaxTreeNode parent, string pattern)
		{
		return (Pattern)parseTopLevel(parent, "<PATTERN>" + pattern, pattern);
		}

		/// <summary>
		/// Parse an XPath pattern: </summary>
		///  <param name="parent"> - XSL element where the pattern occured </param>
		///  <param name="attr">   - name of this element's attribute to get pattern from </param>
		///  <param name="def">    - default pattern (if the attribute was not found) </param>
		public virtual Pattern parsePattern(SyntaxTreeNode parent, string attr, string def)
		{
		// Get the textual representation of the pattern (if any)
			string pattern = parent.getAttribute(attr);
		// Use the default pattern if none was found
		if ((pattern.Length == 0) && (!string.ReferenceEquals(def, null)))
		{
			pattern = def;
		}
		// Invoke the XPath parser
			return (Pattern)parseTopLevel(parent, "<PATTERN>" + pattern, pattern);
		}

		/// <summary>
		/// Parse an XPath expression or pattern using the generated XPathParser
		/// The method will return a Dummy node if the XPath parser fails.
		/// </summary>
		private SyntaxTreeNode parseTopLevel(SyntaxTreeNode parent, string text, string expression)
		{
		int line = LineNumber;

		try
		{
			_xpathParser.Scanner = new XPathLexer(new StringReader(text));
			Symbol result = _xpathParser.parse(expression, line);
			if (result != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode node = (SyntaxTreeNode)result.value;
			SyntaxTreeNode node = (SyntaxTreeNode)result.value;
			if (node != null)
			{
				node.Parser = this;
				node.Parent = parent;
				node.LineNumber = line;
	// System.out.println("e = " + text + " " + node);
				return node;
			}
			}
			reportError(Constants_Fields.ERROR, new ErrorMsg(ErrorMsg.XPATH_PARSER_ERR, expression, parent));
		}
		catch (Exception e)
		{
			if (_xsltc.debug())
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			reportError(Constants_Fields.ERROR, new ErrorMsg(ErrorMsg.XPATH_PARSER_ERR, expression, parent));
		}

		// Return a dummy pattern (which is an expression)
		SyntaxTreeNode.Dummy_Renamed.Parser = this;
			return SyntaxTreeNode.Dummy_Renamed;
		}

		/// <summary>
		///********************** ERROR HANDLING SECTION *********************** </summary>

		/// <summary>
		/// Returns true if there were any errors during compilation
		/// </summary>
		public virtual bool errorsFound()
		{
		return _errors.Count > 0;
		}

		/// <summary>
		/// Prints all compile-time errors
		/// </summary>
		public virtual void printErrors()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = _errors.size();
		int size = _errors.Count;
		if (size > 0)
		{
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.COMPILER_ERROR_KEY));
			for (int i = 0; i < size; i++)
			{
			Console.Error.WriteLine("  " + _errors[i]);
			}
		}
		}

		/// <summary>
		/// Prints all compile-time warnings
		/// </summary>
		public virtual void printWarnings()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = _warnings.size();
		int size = _warnings.Count;
		if (size > 0)
		{
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.COMPILER_WARNING_KEY));
			for (int i = 0; i < size; i++)
			{
			Console.Error.WriteLine("  " + _warnings[i]);
			}
		}
		}

		/// <summary>
		/// Common error/warning message handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void reportError(final int category, final org.apache.xalan.xsltc.compiler.util.ErrorMsg error)
		public virtual void reportError(int category, ErrorMsg error)
		{
		switch (category)
		{
		case Constants_Fields.INTERNAL:
			// Unexpected internal errors, such as null-ptr exceptions, etc.
			// Immediately terminates compilation, no translet produced
			_errors.Add(error);
			break;
		case Constants_Fields.UNSUPPORTED:
			// XSLT elements that are not implemented and unsupported ext.
			// Immediately terminates compilation, no translet produced
			_errors.Add(error);
			break;
		case Constants_Fields.FATAL:
			// Fatal error in the stylesheet input (parsing or content)
			// Immediately terminates compilation, no translet produced
			_errors.Add(error);
			break;
		case Constants_Fields.ERROR:
			// Other error in the stylesheet input (parsing or content)
			// Does not terminate compilation, no translet produced
			_errors.Add(error);
			break;
		case Constants_Fields.WARNING:
			// Other error in the stylesheet input (content errors only)
			// Does not terminate compilation, a translet is produced
			_warnings.Add(error);
			break;
		}
		}

		public virtual ArrayList Errors
		{
			get
			{
			return _errors;
			}
		}

		public virtual ArrayList Warnings
		{
			get
			{
			return _warnings;
			}
		}

		/// <summary>
		///********************** SAX2 ContentHandler INTERFACE **************** </summary>

		private Stack _parentStack = null;
		private Hashtable _prefixMapping = null;

		/// <summary>
		/// SAX2: Receive notification of the beginning of a document.
		/// </summary>
		public virtual void startDocument()
		{
		_root = null;
		_target = null;
		_prefixMapping = null;
		_parentStack = new Stack();
		}

		/// <summary>
		/// SAX2: Receive notification of the end of a document.
		/// </summary>
		public virtual void endDocument()
		{
		}


		/// <summary>
		/// SAX2: Begin the scope of a prefix-URI Namespace mapping.
		///       This has to be passed on to the symbol table!
		/// </summary>
		public virtual void startPrefixMapping(string prefix, string uri)
		{
		if (_prefixMapping == null)
		{
			_prefixMapping = new Hashtable();
		}
		_prefixMapping[prefix] = uri;
		}

		/// <summary>
		/// SAX2: End the scope of a prefix-URI Namespace mapping.
		///       This has to be passed on to the symbol table!
		/// </summary>
		public virtual void endPrefixMapping(string prefix)
		{
		}

		/// <summary>
		/// SAX2: Receive notification of the beginning of an element.
		///       The parser may re-use the attribute list that we're passed so
		///       we clone the attributes in our own Attributes implementation
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localname, String qname, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
		public virtual void startElement(string uri, string localname, string qname, Attributes attributes)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int col = qname.lastIndexOf(':');
		int col = qname.LastIndexOf(':');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = (col == -1) ? null : qname.substring(0, col);
		string prefix = (col == -1) ? null : qname.Substring(0, col);

		SyntaxTreeNode element = makeInstance(uri, prefix, localname, attributes);
		if (element == null)
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.ELEMENT_PARSE_ERR, prefix + ':' + localname);
			throw new SAXException(err.ToString());
		}

		// If this is the root element of the XML document we need to make sure
		// that it contains a definition of the XSL namespace URI
		if (_root == null)
		{
			if ((_prefixMapping == null) || (_prefixMapping.ContainsValue(Constants_Fields.XSLT_URI) == false))
			{
			_rootNamespaceDef = false;
			}
			else
			{
			_rootNamespaceDef = true;
			}
			_root = element;
		}
		else
		{
			SyntaxTreeNode parent = (SyntaxTreeNode)_parentStack.Peek();
			parent.addElement(element);
			element.Parent = parent;
		}
		element.setAttributes(new AttributeList(attributes));
		element.PrefixMapping = _prefixMapping;

		if (element is Stylesheet)
		{
			// Extension elements and excluded elements have to be
			// handled at this point in order to correctly generate
			// Fallback elements from <xsl:fallback>s.
			SymbolTable.CurrentNode = element;
			((Stylesheet)element).declareExtensionPrefixes(this);
		}

		_prefixMapping = null;
		_parentStack.Push(element);
		}

		/// <summary>
		/// SAX2: Receive notification of the end of an element.
		/// </summary>
		public virtual void endElement(string uri, string localname, string qname)
		{
		_parentStack.Pop();
		}

		/// <summary>
		/// SAX2: Receive notification of character data.
		/// </summary>
		public virtual void characters(char[] ch, int start, int length)
		{
		string @string = new string(ch, start, length);
		SyntaxTreeNode parent = (SyntaxTreeNode)_parentStack.Peek();

		if (@string.Length == 0)
		{
			return;
		}

		// If this text occurs within an <xsl:text> element we append it
		// as-is to the existing text element
		if (parent is Text)
		{
			((Text)parent).setText(@string);
			return;
		}

		// Ignore text nodes that occur directly under <xsl:stylesheet>
		if (parent is Stylesheet)
		{
			return;
		}

		SyntaxTreeNode bro = parent.lastChild();
		if ((bro != null) && (bro is Text))
		{
			Text text = (Text)bro;
			if (!text.TextElement)
			{
			if ((length > 1) || (((int)ch[0]) < 0x100))
			{
				text.setText(@string);
				return;
			}
			}
		}

		// Add it as a regular text node otherwise
		parent.addElement(new Text(@string));
		}

		private string getTokenValue(string token)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int start = token.indexOf('"');
		int start = token.IndexOf('"');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stop = token.lastIndexOf('"');
		int stop = token.LastIndexOf('"');
		return token.Substring(start + 1, stop - (start + 1));
		}

		/// <summary>
		/// SAX2: Receive notification of a processing instruction.
		///       These require special handling for stylesheet PIs.
		/// </summary>
		public virtual void processingInstruction(string name, string value)
		{
		// We only handle the <?xml-stylesheet ...?> PI
		if ((string.ReferenceEquals(_target, null)) && (name.Equals("xml-stylesheet")))
		{

			string href = null; // URI of stylesheet found
			string media = null; // Media of stylesheet found
			string title = null; // Title of stylesheet found
			string charset = null; // Charset of stylesheet found

			// Get the attributes from the processing instruction
			StringTokenizer tokens = new StringTokenizer(value);
			while (tokens.hasMoreElements())
			{
			string token = (string)tokens.nextElement();
			if (token.StartsWith("href", StringComparison.Ordinal))
			{
				href = getTokenValue(token);
			}
			else if (token.StartsWith("media", StringComparison.Ordinal))
			{
				media = getTokenValue(token);
			}
			else if (token.StartsWith("title", StringComparison.Ordinal))
			{
				title = getTokenValue(token);
			}
			else if (token.StartsWith("charset", StringComparison.Ordinal))
			{
				charset = getTokenValue(token);
			}
			}

			// Set the target to this PI's href if the parameters are
			// null or match the corresponding attributes of this PI.
			if (((string.ReferenceEquals(_PImedia, null)) || (_PImedia.Equals(media))) && ((string.ReferenceEquals(_PItitle, null)) || (_PImedia.Equals(title))) && ((string.ReferenceEquals(_PIcharset, null)) || (_PImedia.Equals(charset))))
			{
			_target = href;
			}
		}
		}

		/// <summary>
		/// IGNORED - all ignorable whitespace is ignored
		/// </summary>
		public virtual void ignorableWhitespace(char[] ch, int start, int length)
		{
		}

		/// <summary>
		/// IGNORED - we do not have to do anything with skipped entities
		/// </summary>
		public virtual void skippedEntity(string name)
		{
		}

		/// <summary>
		/// Store the document locator to later retrieve line numbers of all
		/// elements from the stylesheet
		/// </summary>
		public virtual Locator DocumentLocator
		{
			set
			{
			_locator = value;
			}
		}

		/// <summary>
		/// Get the line number, or zero
		/// if there is no _locator.
		/// </summary>
		private int LineNumber
		{
			get
			{
				int line = 0;
				if (_locator != null)
				{
					line = _locator.LineNumber;
				}
				return line;
			}
		}

	}

}