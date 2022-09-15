using System;
using System.Collections;
using System.IO;

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
 * $Id: AbstractTranslet.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{
	using Document = org.w3c.dom.Document;
	using DOMImplementation = org.w3c.dom.DOMImplementation;

	using DTM = org.apache.xml.dtm.DTM;

	using DOM = org.apache.xalan.xsltc.DOM;
	using DOMCache = org.apache.xalan.xsltc.DOMCache;
	using DOMEnhancedForDTM = org.apache.xalan.xsltc.DOMEnhancedForDTM;
	using Translet = org.apache.xalan.xsltc.Translet;
	using TransletException = org.apache.xalan.xsltc.TransletException;
	using DOMAdapter = org.apache.xalan.xsltc.dom.DOMAdapter;
	using KeyIndex = org.apache.xalan.xsltc.dom.KeyIndex;
	using TransletOutputHandlerFactory = org.apache.xalan.xsltc.runtime.output.TransletOutputHandlerFactory;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author G. Todd Miller
	/// @author John Howard, JohnH@schemasoft.com 
	/// </summary>
	public abstract class AbstractTranslet : Translet
	{
		public abstract void transform(DOM document, SerializationHandler[] handlers);

		// These attributes are extracted from the xsl:output element. They also
		// appear as fields (with the same type, only public) in Output.java
		public string _version = "1.0";
		public string _method = null;
		public string _encoding = "UTF-8";
		public bool _omitHeader = false;
		public string _standalone = null;
		public string _doctypePublic = null;
		public string _doctypeSystem = null;
		public bool _indent = false;
		public string _mediaType = null;
		public ArrayList _cdata = null;
		public int _indentamount = -1;

		public const int FIRST_TRANSLET_VERSION = 100;
		public const int VER_SPLIT_NAMES_ARRAY = 101;
		public const int CURRENT_TRANSLET_VERSION = VER_SPLIT_NAMES_ARRAY;

		// Initialize Translet version field to base value.  A class that extends
		// AbstractTranslet may override this value to a more recent translet
		// version; if it doesn't override the value (because it was compiled
		// before the notion of a translet version was introduced, it will get
		// this default value).
		protected internal int transletVersion = FIRST_TRANSLET_VERSION;

		// DOM/translet handshaking - the arrays are set by the compiled translet
		protected internal string[] namesArray;
		protected internal string[] urisArray;
		protected internal int[] typesArray;
		protected internal string[] namespaceArray;

		// The Templates object that is used to create this Translet instance
		protected internal Templates _templates = null;

		// Boolean flag to indicate whether this translet has id functions.
		protected internal bool _hasIdCall = false;

		// TODO - these should only be instanciated when needed
		protected internal StringValueHandler stringValueHandler = new StringValueHandler();

		// Use one empty string instead of constantly instanciating String("");
		private const string EMPTYSTRING = "";

		// This is the name of the index used for ID attributes
		private const string ID_INDEX_NAME = "##id";


		/// <summary>
		///**********************************************************************
		/// Debugging
		/// ***********************************************************************
		/// </summary>
		public virtual void printInternalState()
		{
		Console.WriteLine("-------------------------------------");
		Console.WriteLine("AbstractTranslet this = " + this);
		Console.WriteLine("pbase = " + pbase);
		Console.WriteLine("vframe = " + pframe);
		Console.WriteLine("paramsStack.size() = " + paramsStack.Count);
		Console.WriteLine("namesArray.size = " + namesArray.Length);
		Console.WriteLine("namespaceArray.size = " + namespaceArray.Length);
		Console.WriteLine("");
		Console.WriteLine("Total memory = " + Runtime.getRuntime().totalMemory());
		}

		/// <summary>
		/// Wrap the initial input DOM in a dom adapter. This adapter is wrapped in
		/// a DOM multiplexer if the document() function is used (handled by compiled
		/// code in the translet - see compiler/Stylesheet.compileTransform()).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public final org.apache.xalan.xsltc.dom.DOMAdapter makeDOMAdapter(org.apache.xalan.xsltc.DOM dom) throws org.apache.xalan.xsltc.TransletException
		public DOMAdapter makeDOMAdapter(DOM dom)
		{
			RootForKeys = dom.Document;
		return new DOMAdapter(dom, namesArray, urisArray, typesArray, namespaceArray);
		}

		/// <summary>
		///**********************************************************************
		/// Parameter handling
		/// ***********************************************************************
		/// </summary>

		// Parameter's stack: <tt>pbase</tt> and <tt>pframe</tt> are used 
		// to denote the current parameter frame.
		protected internal int pbase = 0, pframe = 0;
		protected internal ArrayList paramsStack = new ArrayList();

		/// <summary>
		/// Push a new parameter frame.
		/// </summary>
		public void pushParamFrame()
		{
		paramsStack.Insert(pframe, new int?(pbase));
		pbase = ++pframe;
		}

		/// <summary>
		/// Pop the topmost parameter frame.
		/// </summary>
		public void popParamFrame()
		{
		if (pbase > 0)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldpbase = ((System.Nullable<int>)paramsStack.get(--pbase)).intValue();
			int oldpbase = ((int?)paramsStack[--pbase]).Value;
			for (int i = pframe - 1; i >= pbase; i--)
			{
			paramsStack.RemoveAt(i);
			}
			pframe = pbase;
			pbase = oldpbase;
		}
		}

		/// <summary>
		/// Add a new global parameter if not already in the current frame.
		/// To setParameters of the form {http://foo.bar}xyz
		/// This needs to get mapped to an instance variable in the class
		/// The mapping  created so that 
		/// the global variables in the generated class become 
		/// http$colon$$flash$$flash$foo$dot$bar$colon$xyz
		/// </summary>
		public object addParameter(string name, object value)
		{
			name = BasisLibrary.mapQNameToJavaName(name);
		return addParameter(name, value, false);
		}

		/// <summary>
		/// Add a new global or local parameter if not already in the current frame.
		/// The 'isDefault' parameter is set to true if the value passed is the
		/// default value from the <xsl:parameter> element's select attribute or
		/// element body.
		/// </summary>
		public object addParameter(string name, object value, bool isDefault)
		{
		// Local parameters need to be re-evaluated for each iteration
		for (int i = pframe - 1; i >= pbase; i--)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parameter param = (Parameter) paramsStack.get(i);
			Parameter param = (Parameter) paramsStack[i];

			if (param._name.Equals(name))
			{
			// Only overwrite if current value is the default value and
			// the new value is _NOT_ the default value.
			if (param._isDefault || !isDefault)
			{
				param._value = value;
				param._isDefault = isDefault;
				return value;
			}
			return param._value;
			}
		}

		// Add new parameter to parameter stack
		paramsStack.Insert(pframe++, new Parameter(name, value, isDefault));
		return value;
		}

		/// <summary>
		/// Clears the parameter stack.
		/// </summary>
		public virtual void clearParameters()
		{
		pbase = pframe = 0;
		paramsStack.Clear();
		}

		/// <summary>
		/// Get the value of a parameter from the current frame or
		/// <tt>null</tt> if undefined.
		/// </summary>
		public object getParameter(string name)
		{

			name = BasisLibrary.mapQNameToJavaName(name);

		for (int i = pframe - 1; i >= pbase; i--)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parameter param = (Parameter)paramsStack.get(i);
			Parameter param = (Parameter)paramsStack[i];
			if (param._name.Equals(name))
			{
				return param._value;
			}
		}
		return null;
		}

		/// <summary>
		///**********************************************************************
		/// Message handling - implementation of <xsl:message>
		/// ***********************************************************************
		/// </summary>

		// Holds the translet's message handler - used for <xsl:message>.
		// The deault message handler dumps a string stdout, but anything can be
		// used, such as a dialog box for applets, etc.
		private MessageHandler _msgHandler = null;

		/// <summary>
		/// Set the translet's message handler - must implement MessageHandler
		/// </summary>
		public MessageHandler MessageHandler
		{
			set
			{
			_msgHandler = value;
			}
		}

		/// <summary>
		/// Pass a message to the message handler - used by Message class.
		/// </summary>
		public void displayMessage(string msg)
		{
		if (_msgHandler == null)
		{
				Console.Error.WriteLine(msg);
		}
		else
		{
			_msgHandler.displayMessage(msg);
		}
		}

		/// <summary>
		///**********************************************************************
		/// Decimal number format symbol handling
		/// ***********************************************************************
		/// </summary>

		// Contains decimal number formatting symbols used by FormatNumberCall
		public Hashtable _formatSymbols = null;

		/// <summary>
		/// Adds a DecimalFormat object to the _formatSymbols hashtable.
		/// The entry is created with the input DecimalFormatSymbols.
		/// </summary>
		public virtual void addDecimalFormat(string name, DecimalFormatSymbols symbols)
		{
		// Instanciate hashtable for formatting symbols if needed
		if (_formatSymbols == null)
		{
			_formatSymbols = new Hashtable();
		}

		// The name cannot be null - use empty string instead
		if (string.ReferenceEquals(name, null))
		{
			name = EMPTYSTRING;
		}

		// Construct a DecimalFormat object containing the symbols we got
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.DecimalFormat df = new java.text.DecimalFormat();
		DecimalFormat df = new DecimalFormat();
		if (symbols != null)
		{
			df.setDecimalFormatSymbols(symbols);
		}
		_formatSymbols.put(name, df);
		}

		/// <summary>
		/// Retrieves a named DecimalFormat object from _formatSymbols hashtable.
		/// </summary>
		public DecimalFormat getDecimalFormat(string name)
		{

		if (_formatSymbols != null)
		{
			// The name cannot be null - use empty string instead
			if (string.ReferenceEquals(name, null))
			{
				name = EMPTYSTRING;
			}

			DecimalFormat df = (DecimalFormat)_formatSymbols.get(name);
			if (df == null)
			{
				df = (DecimalFormat)_formatSymbols.get(EMPTYSTRING);
			}
			return df;
		}
		return (null);
		}

		/// <summary>
		/// Give the translet an opportunity to perform a prepass on the document
		/// to extract any information that it can store in an optimized form.
		/// 
		/// Currently, it only extracts information about attributes of type ID.
		/// </summary>
		public void prepassDocument(DOM document)
		{
			IndexSize = document.Size;
			buildIDIndex(document);
		}

		/// <summary>
		/// Leverages the Key Class to implement the XSLT id() function.
		/// buildIdIndex creates the index (##id) that Key Class uses.
		/// The index contains the element node index (int) and Id value (String).
		/// </summary>
		private void buildIDIndex(DOM document)
		{
			RootForKeys = document.Document;

			if (document is DOMEnhancedForDTM)
			{
				DOMEnhancedForDTM enhancedDOM = (DOMEnhancedForDTM)document;

				// If the input source is DOMSource, the KeyIndex table is not
				// built at this time. It will be built later by the lookupId()
				// and containsId() methods of the KeyIndex class.
				if (enhancedDOM.hasDOMSource())
				{
					buildKeyIndex(ID_INDEX_NAME, document);
					return;
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Hashtable elementsByID = enhancedDOM.getElementsWithIDs();
					Hashtable elementsByID = enhancedDOM.ElementsWithIDs;

					if (elementsByID == null)
					{
						return;
					}

					// Given a Hashtable of DTM nodes indexed by ID attribute values,
					// loop through the table copying information to a KeyIndex
					// for the mapping from ID attribute value to DTM node
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Enumeration idValues = elementsByID.keys();
					System.Collections.IEnumerator idValues = elementsByID.keys();
					bool hasIDValues = false;

					while (idValues.MoveNext())
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object idValue = idValues.Current;
						object idValue = idValues.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int element = document.getNodeHandle(((System.Nullable<int>)elementsByID.get(idValue)).intValue());
						int element = document.getNodeHandle(((int?)elementsByID.get(idValue)).Value);

						buildKeyIndex(ID_INDEX_NAME, element, idValue);
						hasIDValues = true;
					}

					if (hasIDValues)
					{
						setKeyIndexDom(ID_INDEX_NAME, document);
					}
				}
			}
		}

		/// <summary>
		/// After constructing the translet object, this method must be called to
		/// perform any version-specific post-initialization that's required.
		/// </summary>
		public void postInitialization()
		{
			// If the version of the translet had just one namesArray, split
			// it into multiple fields.
			if (transletVersion < VER_SPLIT_NAMES_ARRAY)
			{
				int arraySize = namesArray.Length;
				string[] newURIsArray = new string[arraySize];
				string[] newNamesArray = new string[arraySize];
				int[] newTypesArray = new int[arraySize];

				for (int i = 0; i < arraySize; i++)
				{
					string name = namesArray[i];
					int colonIndex = name.LastIndexOf(':');
					int lNameStartIdx = colonIndex + 1;

					if (colonIndex > -1)
					{
						newURIsArray[i] = name.Substring(0, colonIndex);
					}

				   // Distinguish attribute and element names.  Attribute has
				   // @ before local part of name.
				   if (name[lNameStartIdx] == '@')
				   {
					   lNameStartIdx++;
					   newTypesArray[i] = DTM.ATTRIBUTE_NODE;
				   }
				   else if (name[lNameStartIdx] == '?')
				   {
					   lNameStartIdx++;
					   newTypesArray[i] = DTM.NAMESPACE_NODE;
				   }
				   else
				   {
					   newTypesArray[i] = DTM.ELEMENT_NODE;
				   }
				   newNamesArray[i] = (lNameStartIdx == 0) ? name : name.Substring(lNameStartIdx);
				}

				namesArray = newNamesArray;
				urisArray = newURIsArray;
				typesArray = newTypesArray;
			}

			// Was translet compiled using a more recent version of the XSLTC
			// compiler than is known by the AbstractTranslet class?  If, so
			// and we've made it this far (which is doubtful), we should give up.
			if (transletVersion > CURRENT_TRANSLET_VERSION)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				BasisLibrary.runTimeError(BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, this.GetType().FullName);
			}
		}

		/// <summary>
		///**********************************************************************
		/// Index(es) for <xsl:key> / key() / id()
		/// ***********************************************************************
		/// </summary>

		// Container for all indexes for xsl:key elements
		private Hashtable _keyIndexes = null;
		private KeyIndex _emptyKeyIndex = null;
		private int _indexSize = 0;
		private int _currentRootForKeys = 0;

		/// <summary>
		/// This method is used to pass the largest DOM size to the translet.
		/// Needed to make sure that the translet can index the whole DOM.
		/// </summary>
		public virtual int IndexSize
		{
			set
			{
			if (value > _indexSize)
			{
				_indexSize = value;
			}
			}
		}

		/// <summary>
		/// Creates a KeyIndex object of the desired size - don't want to resize!!!
		/// </summary>
		public virtual KeyIndex createKeyIndex()
		{
		return (new KeyIndex(_indexSize));
		}

		/// <summary>
		/// Adds a value to a key/id index </summary>
		///   <param name="name"> is the name of the index (the key or ##id) </param>
		///   <param name="node"> is the node handle of the node to insert </param>
		///   <param name="value"> is the value that will look up the node in the given index </param>
		public virtual void buildKeyIndex(string name, int node, object value)
		{
		if (_keyIndexes == null)
		{
			_keyIndexes = new Hashtable();
		}

		KeyIndex index = (KeyIndex)_keyIndexes.get(name);
		if (index == null)
		{
			_keyIndexes.put(name, index = new KeyIndex(_indexSize));
		}
		index.add(value, node, _currentRootForKeys);
		}

		/// <summary>
		/// Create an empty KeyIndex in the DOM case </summary>
		///   <param name="name"> is the name of the index (the key or ##id) </param>
		///   <param name="dom"> is the DOM </param>
		public virtual void buildKeyIndex(string name, DOM dom)
		{
		if (_keyIndexes == null)
		{
			_keyIndexes = new Hashtable();
		}

		KeyIndex index = (KeyIndex)_keyIndexes.get(name);
		if (index == null)
		{
			_keyIndexes.put(name, index = new KeyIndex(_indexSize));
		}
		index.Dom = dom;
		}

		/// <summary>
		/// Returns the index for a given key (or id).
		/// The index implements our internal iterator interface
		/// </summary>
		public virtual KeyIndex getKeyIndex(string name)
		{
		// Return an empty key index iterator if none are defined
		if (_keyIndexes == null)
		{
			return (_emptyKeyIndex != null) ? _emptyKeyIndex : (_emptyKeyIndex = new KeyIndex(1));
		}

		// Look up the requested key index
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.dom.KeyIndex index = (org.apache.xalan.xsltc.dom.KeyIndex)_keyIndexes.get(name);
		KeyIndex index = (KeyIndex)_keyIndexes.get(name);

		// Return an empty key index iterator if the requested index not found
		if (index == null)
		{
			return (_emptyKeyIndex != null) ? _emptyKeyIndex : (_emptyKeyIndex = new KeyIndex(1));
		}

		return (index);
		}

		private int RootForKeys
		{
			set
			{
				_currentRootForKeys = value;
			}
		}

		/// <summary>
		/// This method builds key indexes - it is overridden in the compiled
		/// translet in cases where the <xsl:key> element is used
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void buildKeys(org.apache.xalan.xsltc.DOM document, org.apache.xml.dtm.DTMAxisIterator iterator, org.apache.xml.serializer.SerializationHandler handler, int root) throws org.apache.xalan.xsltc.TransletException
		public virtual void buildKeys(DOM document, DTMAxisIterator iterator, SerializationHandler handler, int root)
		{

		}

		/// <summary>
		/// This method builds key indexes - it is overridden in the compiled
		/// translet in cases where the <xsl:key> element is used
		/// </summary>
		public virtual void setKeyIndexDom(string name, DOM document)
		{
			getKeyIndex(name).Dom = document;

		}

		/// <summary>
		///**********************************************************************
		/// DOM cache handling
		/// ***********************************************************************
		/// </summary>

		// Hold the DOM cache (if any) used with this translet
		private DOMCache _domCache = null;

		/// <summary>
		/// Sets the DOM cache used for additional documents loaded using the
		/// document() function.
		/// </summary>
		public virtual DOMCache DOMCache
		{
			set
			{
			_domCache = value;
			}
			get
			{
			return (_domCache);
			}
		}


		/// <summary>
		///**********************************************************************
		/// Multiple output document extension.
		/// See compiler/TransletOutput for actual implementation.
		/// ***********************************************************************
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler openOutputHandler(String filename, boolean append) throws org.apache.xalan.xsltc.TransletException
		public virtual SerializationHandler openOutputHandler(string filename, bool append)
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.runtime.output.TransletOutputHandlerFactory factory = org.apache.xalan.xsltc.runtime.output.TransletOutputHandlerFactory.newInstance();
			TransletOutputHandlerFactory factory = TransletOutputHandlerFactory.newInstance();

				string dirStr = Path.GetDirectoryName(filename);
				if ((null != dirStr) && (dirStr.Length > 0))
				{
				   File dir = new File(dirStr);
				   dir.mkdirs();
				}

			factory.Encoding = _encoding;
			factory.OutputMethod = _method;
			factory.Writer = new StreamWriter(filename, append);
			factory.OutputType = TransletOutputHandlerFactory.STREAM;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.serializer.SerializationHandler handler = factory.getSerializationHandler();
			SerializationHandler handler = factory.SerializationHandler;

			transferOutputSettings(handler);
			handler.startDocument();
			return handler;
		}
		catch (Exception e)
		{
			throw new TransletException(e);
		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler openOutputHandler(String filename) throws org.apache.xalan.xsltc.TransletException
		public virtual SerializationHandler openOutputHandler(string filename)
		{
		   return openOutputHandler(filename, false);
		}

		public virtual void closeOutputHandler(SerializationHandler handler)
		{
		try
		{
			handler.endDocument();
			handler.close();
		}
		catch (Exception)
		{
			// what can you do?
		}
		}

		/// <summary>
		///**********************************************************************
		/// Native API transformation methods - _NOT_ JAXP/TrAX
		/// ***********************************************************************
		/// </summary>

		/// <summary>
		/// Main transform() method - this is overridden by the compiled translet
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void transform(org.apache.xalan.xsltc.DOM document, org.apache.xml.dtm.DTMAxisIterator iterator, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException;
		public abstract void transform(DOM document, DTMAxisIterator iterator, SerializationHandler handler);

		/// <summary>
		/// Calls transform() with a given output handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public final void transform(org.apache.xalan.xsltc.DOM document, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void transform(DOM document, SerializationHandler handler)
		{
			try
			{
				transform(document, document.Iterator, handler);
			}
			finally
			{
				_keyIndexes = null;
			}
		}

		/// <summary>
		/// Used by some compiled code as a shortcut for passing strings to the
		/// output handler
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public final void characters(final String string, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void characters(in string @string, SerializationHandler handler)
		{
			if (!string.ReferenceEquals(@string, null))
			{
			   //final int length = string.length();
			   try
			   {
				   handler.characters(@string);
			   }
			   catch (Exception e)
			   {
				   throw new TransletException(e);
			   }
			}
		}

		/// <summary>
		/// Add's a name of an element whose text contents should be output as CDATA
		/// </summary>
		public virtual void addCdataElement(string name)
		{
		if (_cdata == null)
		{
				_cdata = new ArrayList();
		}

			int lastColon = name.LastIndexOf(':');

			if (lastColon > 0)
			{
				string uri = name.Substring(0, lastColon);
				string localName = name.Substring(lastColon + 1);
			_cdata.Add(uri);
			_cdata.Add(localName);
			}
			else
			{
			_cdata.Add(null);
			_cdata.Add(name);
			}
		}

		/// <summary>
		/// Transfer the output settings to the output post-processor
		/// </summary>
		protected internal virtual void transferOutputSettings(SerializationHandler handler)
		{
		if (!string.ReferenceEquals(_method, null))
		{
			if (_method.Equals("xml"))
			{
				if (!string.ReferenceEquals(_standalone, null))
				{
				handler.Standalone = _standalone;
				}
			if (_omitHeader)
			{
				handler.OmitXMLDeclaration = true;
			}
			handler.CdataSectionElements = _cdata;
			if (!string.ReferenceEquals(_version, null))
			{
				handler.Version = _version;
			}
			handler.Indent = _indent;
			handler.IndentAmount = _indentamount;
			if (!string.ReferenceEquals(_doctypeSystem, null))
			{
				handler.setDoctype(_doctypeSystem, _doctypePublic);
			}
			}
			else if (_method.Equals("html"))
			{
			handler.Indent = _indent;
			handler.setDoctype(_doctypeSystem, _doctypePublic);
			if (!string.ReferenceEquals(_mediaType, null))
			{
				handler.MediaType = _mediaType;
			}
			}
		}
		else
		{
			handler.CdataSectionElements = _cdata;
			if (!string.ReferenceEquals(_version, null))
			{
			handler.Version = _version;
			}
			if (!string.ReferenceEquals(_standalone, null))
			{
			handler.Standalone = _standalone;
			}
			if (_omitHeader)
			{
			handler.OmitXMLDeclaration = true;
			}
			handler.Indent = _indent;
			handler.setDoctype(_doctypeSystem, _doctypePublic);
		}
		}

		private Hashtable _auxClasses = null;

		public virtual void addAuxiliaryClass(Type auxClass)
		{
		if (_auxClasses == null)
		{
			_auxClasses = new Hashtable();
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		_auxClasses.put(auxClass.FullName, auxClass);
		}

		public virtual Hashtable AuxiliaryClasses
		{
			set
			{
				_auxClasses = value;
			}
		}

		public virtual Type getAuxiliaryClass(string className)
		{
		if (_auxClasses == null)
		{
			return null;
		}
		return ((Type)_auxClasses.get(className));
		}

		// GTM added (see pg 110)
		public virtual string[] NamesArray
		{
			get
			{
			return namesArray;
			}
		}

		public virtual string[] UrisArray
		{
			get
			{
				return urisArray;
			}
		}

		public virtual int[] TypesArray
		{
			get
			{
				return typesArray;
			}
		}

		public virtual string[] NamespaceArray
		{
			get
			{
			return namespaceArray;
			}
		}

		public virtual bool hasIdCall()
		{
			return _hasIdCall;
		}

		public virtual Templates Templates
		{
			get
			{
				return _templates;
			}
			set
			{
				_templates = value;
			}
		}


		/// <summary>
		///**********************************************************************
		/// DOMImplementation caching for basis library
		/// ***********************************************************************
		/// </summary>
		protected internal DOMImplementation _domImplementation = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Document newDocument(String uri, String qname) throws javax.xml.parsers.ParserConfigurationException
		public virtual Document newDocument(string uri, string qname)
		{
			if (_domImplementation == null)
			{
				_domImplementation = DocumentBuilderFactory.newInstance().newDocumentBuilder().getDOMImplementation();
			}
			return _domImplementation.createDocument(uri, qname, null);
		}
	}

}