using System;
using System.Collections;
using System.IO;
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
 * $Id: XSLTC.java 1225366 2011-12-28 22:49:12Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using JavaClass = org.apache.bcel.classfile.JavaClass;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using DTM = org.apache.xml.dtm.DTM;

	using InputSource = org.xml.sax.InputSource;
	using XMLReader = org.xml.sax.XMLReader;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller
	/// @author Morten Jorgensen
	/// @author John Howard (johnh@schemasoft.com)
	/// </summary>
	public sealed class XSLTC
	{

		// A reference to the main stylesheet parser object.
		private Parser _parser;

		// A reference to an external XMLReader (SAX parser) passed to us
		private XMLReader _reader = null;

		// A reference to an external SourceLoader (for use with include/import)
		private SourceLoader _loader = null;

		// A reference to the stylesheet being compiled.
		private Stylesheet _stylesheet;

		// Counters used by various classes to generate unique names.
		// private int _variableSerial     = 1;
		private int _modeSerial = 1;
		private int _stylesheetSerial = 1;
		private int _stepPatternSerial = 1;
		private int _helperClassSerial = 0;
		private int _attributeSetSerial = 0;

		private int[] _numberFieldIndexes;

		// Name index tables
		private int _nextGType; // Next available element type
		private ArrayList _namesIndex; // Index of all registered QNames
		private Hashtable _elements; // Hashtable of all registered elements
		private Hashtable _attributes; // Hashtable of all registered attributes

		// Namespace index tables
		private int _nextNSType; // Next available namespace type
		private ArrayList _namespaceIndex; // Index of all registered namespaces
		private Hashtable _namespaces; // Hashtable of all registered namespaces
		private Hashtable _namespacePrefixes; // Hashtable of all registered namespace prefixes


		// All literal text in the stylesheet
		private ArrayList m_characterData;

		// These define the various methods for outputting the translet
		public const int FILE_OUTPUT = 0;
		public const int JAR_OUTPUT = 1;
		public const int BYTEARRAY_OUTPUT = 2;
		public const int CLASSLOADER_OUTPUT = 3;
		public const int BYTEARRAY_AND_FILE_OUTPUT = 4;
		public const int BYTEARRAY_AND_JAR_OUTPUT = 5;


		// Compiler options (passed from command line or XSLTC client)
		private bool _debug = false; // -x
		private string _jarFileName = null; // -j <jar-file-name>
		private string _className = null; // -o <class-name>
		private string _packageName = null; // -p <package-name>
		private File _destDir = null; // -d <directory-name>
		private int _outputType = FILE_OUTPUT; // by default

		private ArrayList _classes;
		private ArrayList _bcelClasses;
		private bool _callsNodeset = false;
		private bool _multiDocument = false;
		private bool _hasIdCall = false;

		private ArrayList _stylesheetNSAncestorPointers;
		private ArrayList _prefixURIPairs;
		private ArrayList _prefixURIPairsIdx;

		/// <summary>
		/// Set to true if template inlining is requested. Template
		/// inlining used to be the default, but we have found that
		/// Hotspots does a better job with shorter methods, so the
		/// default is *not* to inline now.
		/// </summary>
		private bool _templateInlining = false;

		/// <summary>
		/// State of the secure processing feature.
		/// </summary>
		private bool _isSecureProcessing = false;

		/// <summary>
		/// XSLTC compiler constructor
		/// </summary>
		public XSLTC()
		{
		_parser = new Parser(this);
		}

		/// <summary>
		/// Set the state of the secure processing feature.
		/// </summary>
		public bool SecureProcessing
		{
			set
			{
				_isSecureProcessing = value;
			}
			get
			{
				return _isSecureProcessing;
			}
		}


		/// <summary>
		/// Only for user by the internal TrAX implementation.
		/// </summary>
		public Parser Parser
		{
			get
			{
				return _parser;
			}
		}

		/// <summary>
		/// Only for user by the internal TrAX implementation.
		/// </summary>
		public int OutputType
		{
			set
			{
			_outputType = value;
			}
		}

		/// <summary>
		/// Only for user by the internal TrAX implementation.
		/// </summary>
		public Properties OutputProperties
		{
			get
			{
			return _parser.OutputProperties;
			}
		}

		/// <summary>
		/// Initializes the compiler to compile a new stylesheet
		/// </summary>
		public void init()
		{
		reset();
		_reader = null;
		_classes = new ArrayList();
		_bcelClasses = new ArrayList();
		}

		/// <summary>
		/// Initializes the compiler to produce a new translet
		/// </summary>
		private void reset()
		{
		_nextGType = DTM.NTYPES;
		_elements = new Hashtable();
		_attributes = new Hashtable();
		_namespaces = new Hashtable();
		_namespaces[""] = new int?(_nextNSType);
		_namesIndex = new ArrayList(128);
		_namespaceIndex = new ArrayList(32);
		_namespacePrefixes = new Hashtable();
			_stylesheet = null;
		_parser.init();
		//_variableSerial     = 1;
		_modeSerial = 1;
		_stylesheetSerial = 1;
		_stepPatternSerial = 1;
		_helperClassSerial = 0;
		_attributeSetSerial = 0;
		_multiDocument = false;
		_hasIdCall = false;
			_stylesheetNSAncestorPointers = null;
			_prefixURIPairs = null;
			_prefixURIPairsIdx = null;
		_numberFieldIndexes = new int[] {-1, -1, -1};
		}

		/// <summary>
		/// Defines an external SourceLoader to provide the compiler with documents
		/// referenced in xsl:include/import </summary>
		/// <param name="loader"> The SourceLoader to use for include/import </param>
		public SourceLoader SourceLoader
		{
			set
			{
			_loader = value;
			}
		}

		/// <summary>
		/// Set a flag indicating if templates are to be inlined or not. The
		/// default is to do inlining, but this causes problems when the
		/// stylesheets have a large number of templates (e.g. branch targets
		/// exceeding 64K or a length of a method exceeding 64K).
		/// </summary>
		public bool TemplateInlining
		{
			set
			{
			_templateInlining = value;
			}
			get
			{
				return _templateInlining;
			}
		}


		/// <summary>
		/// Set the parameters to use to locate the correct <?xml-stylesheet ...?>
		/// processing instruction in the case where the input document to the
		/// compiler (and parser) is an XML document. </summary>
		/// <param name="media"> The media attribute to be matched. May be null, in which
		/// case the prefered templates will be used (i.e. alternate = no). </param>
		/// <param name="title"> The value of the title attribute to match. May be null. </param>
		/// <param name="charset"> The value of the charset attribute to match. May be null. </param>
		public void setPIParameters(string media, string title, string charset)
		{
		_parser.setPIParameters(media, title, charset);
		}

		/// <summary>
		/// Compiles an XSL stylesheet pointed to by a URL </summary>
		/// <param name="url"> An URL containing the input XSL stylesheet </param>
		public bool compile(URL url)
		{
		try
		{
			// Open input stream from URL and wrap inside InputSource
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream stream = url.openStream();
			Stream stream = url.openStream();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.InputSource input = new org.xml.sax.InputSource(stream);
			InputSource input = new InputSource(stream);
			input.setSystemId(url.ToString());
			return compile(input, _className);
		}
		catch (IOException e)
		{
			_parser.reportError(Constants.FATAL, new ErrorMsg(e));
			return false;
		}
		}

		/// <summary>
		/// Compiles an XSL stylesheet pointed to by a URL </summary>
		/// <param name="url"> An URL containing the input XSL stylesheet </param>
		/// <param name="name"> The name to assign to the translet class </param>
		public bool compile(URL url, string name)
		{
		try
		{
			// Open input stream from URL and wrap inside InputSource
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream stream = url.openStream();
			Stream stream = url.openStream();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.InputSource input = new org.xml.sax.InputSource(stream);
			InputSource input = new InputSource(stream);
			input.setSystemId(url.ToString());
			return compile(input, name);
		}
		catch (IOException e)
		{
			_parser.reportError(Constants.FATAL, new ErrorMsg(e));
			return false;
		}
		}

		/// <summary>
		/// Compiles an XSL stylesheet passed in through an InputStream </summary>
		/// <param name="stream"> An InputStream that will pass in the stylesheet contents </param>
		/// <param name="name"> The name of the translet class to generate </param>
		/// <returns> 'true' if the compilation was successful </returns>
		public bool compile(Stream stream, string name)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.InputSource input = new org.xml.sax.InputSource(stream);
		InputSource input = new InputSource(stream);
		input.setSystemId(name); // We have nothing else!!!
		return compile(input, name);
		}

		/// <summary>
		/// Compiles an XSL stylesheet passed in through an InputStream </summary>
		/// <param name="input"> An InputSource that will pass in the stylesheet contents </param>
		/// <param name="name"> The name of the translet class to generate - can be null </param>
		/// <returns> 'true' if the compilation was successful </returns>
		public bool compile(InputSource input, string name)
		{
		try
		{
			// Reset globals in case we're called by compile(Vector v);
			reset();

			// The systemId may not be set, so we'll have to check the URL
			string systemId = null;
			if (input != null)
			{
				systemId = input.getSystemId();
			}

			// Set the translet class name if not already set
			if (string.ReferenceEquals(_className, null))
			{
			if (!string.ReferenceEquals(name, null))
			{
				ClassName = name;
			}
			else if (!string.ReferenceEquals(systemId, null) && systemId.Length != 0)
			{
				ClassName = Util.baseName(systemId);
			}

					// Ensure we have a non-empty class name at this point
					if (string.ReferenceEquals(_className, null) || _className.Length == 0)
					{
				ClassName = "GregorSamsa"; // default translet name
					}
			}

			// Get the root node of the abstract syntax tree
			SyntaxTreeNode element = null;
			if (_reader == null)
			{
			element = _parser.parse(input);
			}
			else
			{
			element = _parser.parse(_reader, input);
			}

			// Compile the translet - this is where the work is done!
			if ((!_parser.errorsFound()) && (element != null))
			{
			// Create a Stylesheet element from the root node
			_stylesheet = _parser.makeStylesheet(element);
			_stylesheet.SourceLoader = _loader;
			_stylesheet.SystemId = systemId;
			_stylesheet.ParentStylesheet = null;
			_stylesheet.TemplateInlining = _templateInlining;
			_parser.CurrentStylesheet = _stylesheet;

			// Create AST under the Stylesheet element (parse & type-check)
			_parser.createAST(_stylesheet);
			}
			// Generate the bytecodes and output the translet class(es)
			if ((!_parser.errorsFound()) && (_stylesheet != null))
			{
			_stylesheet.CallsNodeset = _callsNodeset;
			_stylesheet.MultiDocument = _multiDocument;
			_stylesheet.HasIdCall = _hasIdCall;

			// Class synchronization is needed for BCEL
			lock (this.GetType())
			{
				_stylesheet.translate();
			}
			}
		}
		catch (Exception e)
		{
			/*if (_debug)*/
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
			_parser.reportError(Constants.FATAL, new ErrorMsg(e));
		}
		catch (Exception e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			_parser.reportError(Constants.FATAL, new ErrorMsg(e));
		}
		finally
		{
			_reader = null; // reset this here to be sure it is not re-used
		}
		return !_parser.errorsFound();
		}

		/// <summary>
		/// Compiles a set of stylesheets pointed to by a Vector of URLs </summary>
		/// <param name="stylesheets"> A Vector containing URLs pointing to the stylesheets </param>
		/// <returns> 'true' if the compilation was successful </returns>
		public bool compile(ArrayList stylesheets)
		{
		// Get the number of stylesheets (ie. URLs) in the vector
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = stylesheets.size();
		int count = stylesheets.Count;

		// Return straight away if the vector is empty
		if (count == 0)
		{
			return true;
		}

		// Special handling needed if the URL count is one, becuase the
		// _className global must not be reset if it was set explicitly
		if (count == 1)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object url = stylesheets.firstElement();
			object url = stylesheets[0];
			if (url is URL)
			{
			return compile((URL)url);
			}
			else
			{
			return false;
			}
		}
		else
		{
			// Traverse all elements in the vector and compile
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration urls = stylesheets.elements();
			System.Collections.IEnumerator urls = stylesheets.GetEnumerator();
			while (urls.MoveNext())
			{
			_className = null; // reset, so that new name will be computed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object url = urls.Current;
			object url = urls.Current;
			if (url is URL)
			{
				if (!compile((URL)url))
				{
					return false;
				}
			}
			}
		}
		return true;
		}

		/// <summary>
		/// Returns an array of bytecode arrays generated by a compilation. </summary>
		/// <returns> JVM bytecodes that represent translet class definition </returns>
		public sbyte[][] Bytecodes
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int count = _classes.size();
			int count = _classes.Count;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final byte[][] result = new byte[count][1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: sbyte[][] result = new sbyte[count][1];
			sbyte[][] result = RectangularArrays.RectangularSbyteArray(count, 1);
			for (int i = 0; i < count; i++)
			{
				result[i] = (sbyte[])_classes[i];
			}
			return result;
			}
		}

		/// <summary>
		/// Compiles a stylesheet pointed to by a URL. The result is put in a
		/// set of byte arrays. One byte array for each generated class. </summary>
		/// <param name="name"> The name of the translet class to generate </param>
		/// <param name="input"> An InputSource that will pass in the stylesheet contents </param>
		/// <param name="outputType"> The output type </param>
		/// <returns> JVM bytecodes that represent translet class definition </returns>
		public sbyte[][] compile(string name, InputSource input, int outputType)
		{
		_outputType = outputType;
		if (compile(input, name))
		{
			return Bytecodes;
		}
		else
		{
			return null;
		}
		}

		/// <summary>
		/// Compiles a stylesheet pointed to by a URL. The result is put in a
		/// set of byte arrays. One byte array for each generated class. </summary>
		/// <param name="name"> The name of the translet class to generate </param>
		/// <param name="input"> An InputSource that will pass in the stylesheet contents </param>
		/// <returns> JVM bytecodes that represent translet class definition </returns>
		public sbyte[][] compile(string name, InputSource input)
		{
			return compile(name, input, BYTEARRAY_OUTPUT);
		}

		/// <summary>
		/// Set the XMLReader to use for parsing the next input stylesheet </summary>
		/// <param name="reader"> XMLReader (SAX2 parser) to use </param>
		public XMLReader XMLReader
		{
			set
			{
			_reader = value;
			}
			get
			{
			return _reader;
			}
		}


		/// <summary>
		/// Get a Vector containing all compile error messages </summary>
		/// <returns> A Vector containing all compile error messages </returns>
		public ArrayList Errors
		{
			get
			{
			return _parser.Errors;
			}
		}

		/// <summary>
		/// Get a Vector containing all compile warning messages </summary>
		/// <returns> A Vector containing all compile error messages </returns>
		public ArrayList Warnings
		{
			get
			{
			return _parser.Warnings;
			}
		}

		/// <summary>
		/// Print all compile error messages to standard output
		/// </summary>
		public void printErrors()
		{
		_parser.printErrors();
		}

		/// <summary>
		/// Print all compile warning messages to standard output
		/// </summary>
		public void printWarnings()
		{
		_parser.printWarnings();
		}

		/// <summary>
		/// This method is called by the XPathParser when it encounters a call
		/// to the document() function. Affects the DOM used by the translet.
		/// </summary>
		protected internal bool MultiDocument
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


		/// <summary>
		/// This method is called by the XPathParser when it encounters a call
		/// to the nodeset() extension function. Implies multi document.
		/// </summary>
		protected internal bool CallsNodeset
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

		protected internal bool HasIdCall
		{
			set
			{
				_hasIdCall = value;
			}
		}

		public bool hasIdCall()
		{
			return _hasIdCall;
		}

		/// <summary>
		/// Set the class name for the generated translet. This class name is
		/// overridden if multiple stylesheets are compiled in one go using the
		/// compile(Vector urls) method. </summary>
		/// <param name="className"> The name to assign to the translet class </param>
		public string ClassName
		{
			set
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String super = org.apache.xalan.xsltc.compiler.util.Util.baseName(value);
			string @base = Util.baseName(value);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final String noext = org.apache.xalan.xsltc.compiler.util.Util.noExtName(super);
			string noext = Util.noExtName(@base);
			string name = Util.toJavaName(noext);
    
			if (string.ReferenceEquals(_packageName, null))
			{
				_className = name;
			}
			else
			{
				_className = _packageName + '.' + name;
			}
			}
			get
			{
			return _className;
			}
		}


		/// <summary>
		/// Convert for Java class name of local system file name.
		/// (Replace '.' with '/' on UNIX and replace '.' by '\' on Windows/DOS.)
		/// </summary>
		private string classFileName(in string className)
		{
		return className.Replace('.', Path.DirectorySeparatorChar) + ".class";
		}

		/// <summary>
		/// Generate an output File object to send the translet to
		/// </summary>
		private File getOutputFile(string className)
		{
		if (_destDir != null)
		{
			return new File(_destDir, classFileName(className));
		}
		else
		{
			return new File(classFileName(className));
		}
		}

		/// <summary>
		/// Set the destination directory for the translet.
		/// The current working directory will be used by default.
		/// </summary>
		public bool setDestDirectory(string dstDirName)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.File dir = new java.io.File(dstDirName);
		File dir = new File(dstDirName);
		if (dir.exists() || dir.mkdirs())
		{
			_destDir = dir;
			return true;
		}
		else
		{
			_destDir = null;
			return false;
		}
		}

		/// <summary>
		/// Set an optional package name for the translet and auxiliary classes
		/// </summary>
		public string PackageName
		{
			set
			{
			_packageName = value;
			if (!string.ReferenceEquals(_className, null))
			{
				ClassName = _className;
			}
			}
		}

		/// <summary>
		/// Set the name of an optional JAR-file to dump the translet and
		/// auxiliary classes to
		/// </summary>
		public string JarFileName
		{
			set
			{
			const string JAR_EXT = ".jar";
			if (value.EndsWith(JAR_EXT, StringComparison.Ordinal))
			{
				_jarFileName = value;
			}
			else
			{
				_jarFileName = value + JAR_EXT;
			}
			_outputType = JAR_OUTPUT;
			}
			get
			{
			return _jarFileName;
			}
		}


		/// <summary>
		/// Set the top-level stylesheet
		/// </summary>
		public Stylesheet Stylesheet
		{
			set
			{
			if (_stylesheet == null)
			{
				_stylesheet = value;
			}
			}
			get
			{
			return _stylesheet;
			}
		}


		/// <summary>
		/// Registers an attribute and gives it a type so that it can be mapped to
		/// DOM attribute types at run-time.
		/// </summary>
		public int registerAttribute(QName name)
		{
		int? code = (int?)_attributes[name.ToString()];
		if (code == null)
		{
			code = new int?(_nextGType++);
			_attributes[name.ToString()] = code;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = name.getNamespace();
			string uri = name.Namespace;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String local = "@"+name.getLocalPart();
			string local = "@" + name.LocalPart;
			if ((!string.ReferenceEquals(uri, null)) && (uri.Length != 0))
			{
			_namesIndex.Add(uri + ":" + local);
			}
			else
			{
			_namesIndex.Add(local);
			}
			if (name.LocalPart.Equals("*"))
			{
			registerNamespace(name.Namespace);
			}
		}
		return code.Value;
		}

		/// <summary>
		/// Registers an element and gives it a type so that it can be mapped to
		/// DOM element types at run-time.
		/// </summary>
		public int registerElement(QName name)
		{
		// Register element (full QName)
		int? code = (int?)_elements[name.ToString()];
		if (code == null)
		{
			_elements[name.ToString()] = code = new int?(_nextGType++);
			_namesIndex.Add(name.ToString());
		}
		if (name.LocalPart.Equals("*"))
		{
			registerNamespace(name.Namespace);
		}
		return code.Value;
		}

		 /// <summary>
		 /// Registers a namespace prefix and gives it a type so that it can be mapped to
		 /// DOM namespace types at run-time.
		 /// </summary>

		public int registerNamespacePrefix(QName name)
		{

		int? code = (int?)_namespacePrefixes[name.ToString()];
		if (code == null)
		{
			code = new int?(_nextGType++);
			_namespacePrefixes[name.ToString()] = code;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri = name.getNamespace();
			string uri = name.Namespace;
			if ((!string.ReferenceEquals(uri, null)) && (uri.Length != 0))
			{
				// namespace::ext2:ped2 will be made empty in TypedNamespaceIterator
				_namesIndex.Add("?");
			}
			else
			{
			   _namesIndex.Add("?" + name.LocalPart);
			}
		}
		return code.Value;
		}

		/// <summary>
		/// Registers a namespace and gives it a type so that it can be mapped to
		/// DOM namespace types at run-time.
		/// </summary>
		public int registerNamespacePrefix(string name)
		{
			int? code = (int?)_namespacePrefixes[name];
			if (code == null)
			{
				code = new int?(_nextGType++);
				_namespacePrefixes[name] = code;
				_namesIndex.Add("?" + name);
			}
			return code.Value;
		}

		/// <summary>
		/// Registers a namespace and gives it a type so that it can be mapped to
		/// DOM namespace types at run-time.
		/// </summary>
		public int registerNamespace(string namespaceURI)
		{
		int? code = (int?)_namespaces[namespaceURI];
		if (code == null)
		{
			code = new int?(_nextNSType++);
			_namespaces[namespaceURI] = code;
			_namespaceIndex.Add(namespaceURI);
		}
		return code.Value;
		}

		/// <summary>
		/// Registers namespace declarations that the stylesheet might need to
		/// look up dynamically - for instance, if an <code>xsl:element</code> has a
		/// a <code>name</code> attribute with variable parts and has no
		/// <code>namespace</code> attribute.
		/// </summary>
		/// <param name="prefixMap"> a <code>Hashtable</code> mapping namespace prefixes to
		///                  URIs.  Must not be <code>null</code>.  The default
		///                  namespace and namespace undeclarations are represented
		///                  by a zero-length string. </param>
		/// <param name="ancestorID"> The <code>int</code> node ID of the nearest ancestor in
		///                 the stylesheet that declares namespaces, or a value less
		///                 than zero if there is no such ancestor </param>
		/// <returns> A new node ID for the stylesheet element  </returns>
		public int registerStylesheetPrefixMappingForRuntime(Hashtable prefixMap, int ancestorID)
		{
			if (_stylesheetNSAncestorPointers == null)
			{
				_stylesheetNSAncestorPointers = new ArrayList();
			}

			if (_prefixURIPairs == null)
			{
				_prefixURIPairs = new ArrayList();
			}

			if (_prefixURIPairsIdx == null)
			{
				_prefixURIPairsIdx = new ArrayList();
			}

			int currentNodeID = _stylesheetNSAncestorPointers.Count;
			_stylesheetNSAncestorPointers.Add(new int?(ancestorID));

			System.Collections.IEnumerator prefixMapIterator = prefixMap.SetOfKeyValuePairs().GetEnumerator();
			int prefixNSPairStartIdx = _prefixURIPairs.Count;
			_prefixURIPairsIdx.Add(new int?(prefixNSPairStartIdx));

			while (prefixMapIterator.MoveNext())
			{
				DictionaryEntry entry = (DictionaryEntry) prefixMapIterator.Current;
				string prefix = (string) entry.Key;
				string uri = (string) entry.Value;
				_prefixURIPairs.Add(prefix);
				_prefixURIPairs.Add(uri);
			}

			return currentNodeID;
		}

		public ArrayList NSAncestorPointers
		{
			get
			{
				return _stylesheetNSAncestorPointers;
			}
		}

		public ArrayList PrefixURIPairs
		{
			get
			{
				return _prefixURIPairs;
			}
		}

		public ArrayList PrefixURIPairsIdx
		{
			get
			{
				return _prefixURIPairsIdx;
			}
		}

		public int nextModeSerial()
		{
		return _modeSerial++;
		}

		public int nextStylesheetSerial()
		{
		return _stylesheetSerial++;
		}

		public int nextStepPatternSerial()
		{
		return _stepPatternSerial++;
		}

		public int[] NumberFieldIndexes
		{
			get
			{
			return _numberFieldIndexes;
			}
		}

		public int nextHelperClassSerial()
		{
		return _helperClassSerial++;
		}

		public int nextAttributeSetSerial()
		{
		return _attributeSetSerial++;
		}

		public ArrayList NamesIndex
		{
			get
			{
			return _namesIndex;
			}
		}

		public ArrayList NamespaceIndex
		{
			get
			{
			return _namespaceIndex;
			}
		}

		/// <summary>
		/// Returns a unique name for every helper class needed to
		/// execute a translet.
		/// </summary>
		public string HelperClassName
		{
			get
			{
			return ClassName + '$' + _helperClassSerial++;
			}
		}

		public void dumpClass(JavaClass clazz)
		{

		if (_outputType == FILE_OUTPUT || _outputType == BYTEARRAY_AND_FILE_OUTPUT)
		{
			File outFile = getOutputFile(clazz.getClassName());
			string parentDir = outFile.getParent();
			if (!string.ReferenceEquals(parentDir, null))
			{
				  File parentFile = new File(parentDir);
				  if (!parentFile.exists())
				  {
					parentFile.mkdirs();
				  }
			}
		}

		try
		{
			switch (_outputType)
			{
			case FILE_OUTPUT:
			clazz.dump(new BufferedOutputStream(new FileStream(getOutputFile(clazz.getClassName()), FileMode.Create, FileAccess.Write)));
			break;
			case JAR_OUTPUT:
			_bcelClasses.Add(clazz);
			break;
			case BYTEARRAY_OUTPUT:
			case BYTEARRAY_AND_FILE_OUTPUT:
			case BYTEARRAY_AND_JAR_OUTPUT:
			case CLASSLOADER_OUTPUT:
			MemoryStream @out = new MemoryStream(2048);
			clazz.dump(@out);
			_classes.Add(@out.toByteArray());

			if (_outputType == BYTEARRAY_AND_FILE_OUTPUT)
			{
			  clazz.dump(new BufferedOutputStream(new FileStream(getOutputFile(clazz.getClassName()), FileMode.Create, FileAccess.Write)));
			}
			else if (_outputType == BYTEARRAY_AND_JAR_OUTPUT)
			{
			  _bcelClasses.Add(clazz);
			}

			break;
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		}
		}

		/// <summary>
		/// File separators are converted to forward slashes for ZIP files.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private String entryName(java.io.File f) throws java.io.IOException
		private string entryName(File f)
		{
		return f.getName().replace(Path.DirectorySeparatorChar, '/');
		}

		/// <summary>
		/// Generate output JAR-file and packages
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void outputToJar() throws java.io.IOException
		public void outputToJar()
		{
		// create the manifest
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.jar.Manifest manifest = new java.util.jar.Manifest();
		Manifest manifest = new Manifest();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.jar.Attributes atrs = manifest.getMainAttributes();
		java.util.jar.Attributes atrs = manifest.getMainAttributes();
		atrs.put(java.util.jar.Attributes.Name.MANIFEST_VERSION,"1.2");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map map = manifest.getEntries();
		System.Collections.IDictionary map = manifest.getEntries();
		// create manifest
		System.Collections.IEnumerator classes = _bcelClasses.GetEnumerator();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String now = (new java.util.Date()).toString();
		string now = (DateTime.Now).ToString();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.jar.Attributes.Name dateAttr = new java.util.jar.Attributes.Name("Date");
		java.util.jar.Attributes.Name dateAttr = new java.util.jar.Attributes.Name("Date");
		while (classes.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.classfile.JavaClass clazz = (org.apache.bcel.classfile.JavaClass)classes.Current;
			JavaClass clazz = (JavaClass)classes.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = clazz.getClassName().replace('.','/');
			string className = clazz.getClassName().replace('.','/');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.jar.Attributes attr = new java.util.jar.Attributes();
			java.util.jar.Attributes attr = new java.util.jar.Attributes();
			attr.put(dateAttr, now);
			map[className + ".class"] = attr;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.File jarFile = new java.io.File(_destDir, _jarFileName);
		File jarFile = new File(_destDir, _jarFileName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.jar.JarOutputStream jos = new java.util.jar.JarOutputStream(new java.io.FileOutputStream(jarFile), manifest);
		JarOutputStream jos = new JarOutputStream(new FileStream(jarFile, FileMode.Create, FileAccess.Write), manifest);
		classes = _bcelClasses.GetEnumerator();
		while (classes.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.classfile.JavaClass clazz = (org.apache.bcel.classfile.JavaClass)classes.Current;
			JavaClass clazz = (JavaClass)classes.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = clazz.getClassName().replace('.','/');
			string className = clazz.getClassName().replace('.','/');
			jos.putNextEntry(new JarEntry(className + ".class"));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.ByteArrayOutputStream out = new java.io.ByteArrayOutputStream(2048);
			MemoryStream @out = new MemoryStream(2048);
			clazz.dump(@out); // dump() closes it's output stream
			@out.writeTo(jos);
		}
		jos.close();
		}

		/// <summary>
		/// Turn debugging messages on/off
		/// </summary>
		public bool Debug
		{
			set
			{
			_debug = value;
			}
		}

		/// <summary>
		/// Get current debugging message setting
		/// </summary>
		public bool debug()
		{
		return _debug;
		}


		/// <summary>
		/// Retrieve a string representation of the character data to be stored
		/// in the translet as a <code>char[]</code>.  There may be more than
		/// one such array required. </summary>
		/// <param name="index"> The index of the <code>char[]</code>.  Zero-based. </param>
		/// <returns> String The character data to be stored in the corresponding
		///               <code>char[]</code>. </returns>
		public string getCharacterData(int index)
		{
			return ((StringBuilder) m_characterData[index]).ToString();
		}

		/// <summary>
		/// Get the number of char[] arrays, thus far, that will be created to
		/// store literal text in the stylesheet.
		/// </summary>
		public int CharacterDataCount
		{
			get
			{
				return (m_characterData != null) ? m_characterData.Count : 0;
			}
		}

		/// <summary>
		/// Add literal text to char arrays that will be used to store character
		/// data in the stylesheet. </summary>
		/// <param name="newData"> String data to be added to char arrays.
		///                Pre-condition:  <code>newData.length() &le; 21845</code> </param>
		/// <returns> int offset at which character data will be stored </returns>
		public int addCharacterData(string newData)
		{
			StringBuilder currData;
			if (m_characterData == null)
			{
				m_characterData = new ArrayList();
				currData = new StringBuilder();
				m_characterData.Add(currData);
			}
			else
			{
				currData = (StringBuilder) m_characterData[m_characterData.Count - 1];
			}

			// Character data could take up to three-times as much space when
			// written to the class file as UTF-8.  The maximum size for a
			// constant is 65535/3.  If we exceed that,
			// (We really should use some "bin packing".)
			if (newData.Length + currData.Length > 21845)
			{
				currData = new StringBuilder();
				m_characterData.Add(currData);
			}

			int newDataOffset = currData.Length;
			currData.Append(newData);

			return newDataOffset;
		}
	}

}