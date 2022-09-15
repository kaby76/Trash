using System;
using System.Collections;
using System.Threading;

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
 * $Id: TransformerImpl.java 1581058 2014-03-24 20:55:14Z ggregory $
 */
namespace org.apache.xalan.transformer
{



	using ExtensionsTable = org.apache.xalan.extensions.ExtensionsTable;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using Method = org.apache.xml.serializer.Method;
	using Serializer = org.apache.xml.serializer.Serializer;
	using SerializerFactory = org.apache.xml.serializer.SerializerFactory;
	using AVT = org.apache.xalan.templates.AVT;
	using Constants = org.apache.xalan.templates.Constants;
	using ElemAttributeSet = org.apache.xalan.templates.ElemAttributeSet;
	using ElemForEach = org.apache.xalan.templates.ElemForEach;
	using ElemSort = org.apache.xalan.templates.ElemSort;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemTextLiteral = org.apache.xalan.templates.ElemTextLiteral;
	using ElemVariable = org.apache.xalan.templates.ElemVariable;
	using OutputProperties = org.apache.xalan.templates.OutputProperties;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using StylesheetComposed = org.apache.xalan.templates.StylesheetComposed;
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;
	using XUnresolvedVariable = org.apache.xalan.templates.XUnresolvedVariable;
	using GenerateEvent = org.apache.xalan.trace.GenerateEvent;
	using TraceManager = org.apache.xalan.trace.TraceManager;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using ToSAXHandler = org.apache.xml.serializer.ToSAXHandler;
	using ToTextStream = org.apache.xml.serializer.ToTextStream;
	using ToXMLSAXHandler = org.apache.xml.serializer.ToXMLSAXHandler;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using BoolStack = org.apache.xml.utils.BoolStack;
	using DOMBuilder = org.apache.xml.utils.DOMBuilder;
	using NodeVector = org.apache.xml.utils.NodeVector;
	using ObjectPool = org.apache.xml.utils.ObjectPool;
	using ObjectStack = org.apache.xml.utils.ObjectStack;
	using QName = org.apache.xml.utils.QName;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using ThreadControllerWrapper = org.apache.xml.utils.ThreadControllerWrapper;
	using Arg = org.apache.xpath.Arg;
	using ExtensionsProvider = org.apache.xpath.ExtensionsProvider;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathContext = org.apache.xpath.XPathContext;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using XObject = org.apache.xpath.objects.XObject;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using SAXException = org.xml.sax.SAXException;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;
	using DeclHandler = org.xml.sax.ext.DeclHandler;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// This class implements the
	/// <seealso cref="javax.xml.transform.Transformer"/> interface, and is the core
	/// representation of the transformation execution.</p>
	/// @xsl.usage advanced
	/// </summary>
	public class TransformerImpl : Transformer, System.Threading.ThreadStart, DTMWSFilter, ExtensionsProvider, org.apache.xml.serializer.SerializerTrace
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			m_traceManager = new TraceManager(this);
		}


	  // Synch object to gaurd against setting values from the TrAX interface 
	  // or reentry while the transform is going on.

	  /// <summary>
	  /// NEEDSDOC Field m_reentryGuard </summary>
	  private bool? m_reentryGuard = new bool?(true);

	  /// <summary>
	  /// This is null unless we own the stream.
	  /// </summary>
	  private System.IO.FileStream m_outputStream = null;

	  /// <summary>
	  /// True if the parser events should be on the main thread,
	  /// false if not.  Experemental.  Can not be set right now.
	  /// </summary>
	  private bool m_parserEventsOnMain = true;

	  /// <summary>
	  /// The thread that the transformer is running on. </summary>
	  private Thread m_transformThread;

	  /// <summary>
	  /// The base URL of the source tree. </summary>
	  private string m_urlOfSource = null;

	  /// <summary>
	  /// The Result object at the start of the transform, if any. </summary>
	  private Result m_outputTarget = null;

	  /// <summary>
	  /// The output format object set by the user.  May be null.
	  /// </summary>
	  private OutputProperties m_outputFormat;


	  /// <summary>
	  /// The content handler for the source input tree.
	  /// </summary>
	  internal ContentHandler m_inputContentHandler;

	  /// <summary>
	  /// The content handler for the result tree.
	  /// </summary>
	  private ContentHandler m_outputContentHandler = null;

	  //  /*
	  //   * Use member variable to store param variables as they're
	  //   * being created, use member variable so we don't
	  //   * have to create a new vector every time.
	  //   */
	  //  private Vector m_newVars = new Vector();

	  /// <summary>
	  /// The JAXP Document Builder, mainly to create Result Tree Fragments. </summary>
	  internal DocumentBuilder m_docBuilder = null;

	  /// <summary>
	  /// A pool of ResultTreeHandlers, for serialization of a subtree to text.
	  ///  Please note that each of these also holds onto a Text Serializer.  
	  /// </summary>
	  private ObjectPool m_textResultHandlerObjectPool = new ObjectPool(typeof(ToTextStream));

	  /// <summary>
	  /// Related to m_textResultHandlerObjectPool, this is a pool of
	  /// StringWriters, which are passed to the Text Serializers.
	  /// (I'm not sure if this is really needed any more.  -sb)      
	  /// </summary>
	  private ObjectPool m_stringWriterObjectPool = new ObjectPool(typeof(StringWriter));

	  /// <summary>
	  /// A static text format object, which can be used over and
	  /// over to create the text serializers.    
	  /// </summary>
	  private OutputProperties m_textformat = new OutputProperties(Method.TEXT);

	  // Commenteded out in response to problem reported by 
	  // Nicola Brown <Nicola.Brown@jacobsrimell.com>
	  //  /**
	  //   * Flag to let us know if an exception should be reported inside the 
	  //   * postExceptionFromThread method.  This is needed if the transform is 
	  //   * being generated from SAX events, and thus there is no central place 
	  //   * to report the exception from.  (An exception is usually picked up in 
	  //   * the main thread from the transform thread in {@link #transform(Source source)} 
	  //   * from {@link #getExceptionThrown()}. )
	  //   */
	  //  private boolean m_reportInPostExceptionFromThread = false;

	  /// <summary>
	  /// A node vector used as a stack to track the current
	  /// ElemTemplateElement.  Needed for the
	  /// org.apache.xalan.transformer.TransformState interface,
	  /// so a tool can discover the calling template. Note the use of an array 
	  /// for this limits the recursion depth to 4K.
	  /// </summary>
	  internal ObjectStack m_currentTemplateElements = new ObjectStack(XPathContext.RECURSIONLIMIT);

	  /// <summary>
	  /// The top of the currentTemplateElements stack. </summary>
	  //int m_currentTemplateElementsTop = 0;

	  /// <summary>
	  /// A node vector used as a stack to track the current
	  /// ElemTemplate that was matched.
	  /// Needed for the
	  /// org.apache.xalan.transformer.TransformState interface,
	  /// so a tool can discover the matched template
	  /// </summary>
	  internal Stack m_currentMatchTemplates = new Stack();

	  /// <summary>
	  /// A node vector used as a stack to track the current
	  /// node that was matched.
	  /// Needed for the
	  /// org.apache.xalan.transformer.TransformState interface,
	  /// so a tool can discover the matched
	  /// node. 
	  /// </summary>
	  internal NodeVector m_currentMatchedNodes = new NodeVector();

	  /// <summary>
	  /// The root of a linked set of stylesheets.
	  /// </summary>
	  private StylesheetRoot m_stylesheetRoot = null;

	  /// <summary>
	  /// If this is set to true, do not warn about pattern
	  /// match conflicts.
	  /// </summary>
	  private bool m_quietConflictWarnings = true;

	  /// <summary>
	  /// The liason to the XML parser, so the XSL processor
	  /// can handle included files, and the like, and do the
	  /// initial parse of the XSL document.
	  /// </summary>
	  private XPathContext m_xcontext;

	  /// <summary>
	  /// Object to guard agains infinite recursion when
	  /// doing queries.
	  /// </summary>
	  private StackGuard m_stackGuard;

	  /// <summary>
	  /// Output handler to bottleneck SAX events.
	  /// </summary>
	  private SerializationHandler m_serializationHandler;

	  /// <summary>
	  /// The key manager, which manages xsl:keys. </summary>
	  private KeyManager m_keyManager = new KeyManager();

	  /// <summary>
	  /// Stack for the purposes of flagging infinite recursion with
	  /// attribute sets.
	  /// </summary>
	  internal Stack m_attrSetStack = null;

	  /// <summary>
	  /// The table of counters for xsl:number support. </summary>
	  /// <seealso cref= ElemNumber </seealso>
	  internal CountersTable m_countersTable = null;

	  /// <summary>
	  /// Is > 0 when we're processing a for-each.
	  /// </summary>
	  internal BoolStack m_currentTemplateRuleIsNull = new BoolStack();

	  /// <summary>
	  /// Keeps track of the result delivered by any EXSLT <code>func:result</code>
	  /// instruction that has been executed for the currently active EXSLT
	  /// <code>func:function</code>
	  /// </summary>
	  internal ObjectStack m_currentFuncResult = new ObjectStack();

	  /// <summary>
	  /// The message manager, which manages error messages, warning
	  /// messages, and other types of message events.   
	  /// </summary>
	  private MsgMgr m_msgMgr;

	  /// <summary>
	  /// The flag for the setting of the optimize feature;
	  /// This flag should have the same value as the FEATURE_OPTIMIZE feature
	  /// which is set by the TransformerFactory.setAttribut() method before a
	  /// Transformer is created
	  /// </summary>
	  private bool m_optimizer = true;

	  /// <summary>
	  /// The flag for the setting of the incremental feature;
	  /// This flag should have the same value as the FEATURE_INCREMENTAL feature
	  /// which is set by the TransformerFactory.setAttribut() method before a
	  /// Transformer is created
	  /// </summary>
	  private bool m_incremental = false;

	  /// <summary>
	  /// The flag for the setting of the source_location feature;
	  /// This flag should have the same value as the FEATURE_SOURCE_LOCATION feature
	  /// which is set by the TransformerFactory.setAttribut() method before a
	  /// Transformer is created
	  /// </summary>
	  private bool m_source_location = false;

	  /// <summary>
	  /// This is a compile-time flag to turn off calling
	  /// of trace listeners. Set this to false for optimization purposes.
	  /// </summary>
	  private bool m_debug = false;

	  /// <summary>
	  /// The SAX error handler, where errors and warnings are sent.
	  /// </summary>
	  private ErrorListener m_errorHandler = new org.apache.xml.utils.DefaultErrorHandler(false);

	  /// <summary>
	  /// The trace manager.
	  /// </summary>
	  private TraceManager m_traceManager;

	  /// <summary>
	  /// If the transform thread throws an exception, the exception needs to
	  /// be stashed away so that the main thread can pass it on to the
	  /// client. 
	  /// </summary>
	  private Exception m_exceptionThrown = null;

	  /// <summary>
	  /// The InputSource for the source tree, which is needed if the
	  /// parse thread is not the main thread, in order for the parse
	  /// thread's run method to get to the input source.
	  /// (Delete this if reversing threads is outlawed. -sb)    
	  /// </summary>
	  private Source m_xmlSource;

	  /// <summary>
	  /// This is needed for support of setSourceTreeDocForThread(Node doc),
	  /// which must be called in order for the transform thread's run
	  /// method to obtain the root of the source tree to be transformed.     
	  /// </summary>
	  private int m_doc;

	  /// <summary>
	  /// If the the transform is on the secondary thread, we
	  /// need to know when it is done, so we can return.
	  /// </summary>
	  private bool m_isTransformDone = false;

	  /// <summary>
	  /// Flag to to tell if the tranformer needs to be reset. </summary>
	  private bool m_hasBeenReset = false;

	  /// <summary>
	  /// NEEDSDOC Field m_shouldReset </summary>
	  private bool m_shouldReset = true;

	  /// <summary>
	  /// NEEDSDOC Method setShouldReset 
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="shouldReset"> </param>
	  public virtual bool ShouldReset
	  {
		  set
		  {
			m_shouldReset = value;
		  }
	  }

	  /// <summary>
	  /// A stack of current template modes.
	  /// </summary>
	  private Stack m_modes = new Stack();

	  //==========================================================
	  // SECTION: Constructor
	  //==========================================================

	  /// <summary>
	  /// Construct a TransformerImpl.
	  /// </summary>
	  /// <param name="stylesheet"> The root of the stylesheet tree. </param>
	  public TransformerImpl(StylesheetRoot stylesheet)
	  {
	   // throws javax.xml.transform.TransformerException    
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		m_optimizer = stylesheet.Optimizer;
		m_incremental = stylesheet.Incremental;
		m_source_location = stylesheet.Source_location;
		Stylesheet = stylesheet;
		XPathContext xPath = new XPathContext(this);
		xPath.Incremental = m_incremental;
		xPath.DTMManager.Incremental = m_incremental;
		xPath.Source_location = m_source_location;
		xPath.DTMManager.Source_location = m_source_location;

		if (stylesheet.SecureProcessing)
		{
		  xPath.SecureProcessing = true;
		}

		XPathContext = xPath;
		XPathContext.NamespaceContext = stylesheet;
		m_stackGuard = new StackGuard(this);
	  }

	  // ================ ExtensionsTable ===================

	  /// <summary>
	  /// The table of ExtensionHandlers.
	  /// </summary>
	  private ExtensionsTable m_extensionsTable = null;

	  /// <summary>
	  /// Get the extensions table object. 
	  /// </summary>
	  /// <returns> The extensions table. </returns>
	  public virtual ExtensionsTable getExtensionsTable()
	  {
		return m_extensionsTable;
	  }

	  /// <summary>
	  /// If the stylesheet contains extensions, set the extensions table object.
	  /// 
	  /// </summary>
	  /// <param name="sroot"> The stylesheet. </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setExtensionsTable(org.apache.xalan.templates.StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  internal virtual void setExtensionsTable(StylesheetRoot sroot)
	  {
		try
		{
		  if (sroot.Extensions != null)
		  {
			//only load extensions if secureProcessing is disabled
			if (!sroot.SecureProcessing)
			{
				m_extensionsTable = new ExtensionsTable(sroot);
			}
		  }
		}
		catch (TransformerException te)
		{
			Console.WriteLine(te.ToString());
			Console.Write(te.StackTrace);
		}
	  }

	  //== Implementation of the XPath ExtensionsProvider interface.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean functionAvailable(String ns, String funcName) throws javax.xml.transform.TransformerException
	  public virtual bool functionAvailable(string ns, string funcName)
	  {
		return getExtensionsTable().functionAvailable(ns, funcName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean elementAvailable(String ns, String elemName) throws javax.xml.transform.TransformerException
	  public virtual bool elementAvailable(string ns, string elemName)
	  {
		return getExtensionsTable().elementAvailable(ns, elemName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object extFunction(String ns, String funcName, java.util.Vector argVec, Object methodKey) throws javax.xml.transform.TransformerException
	  public virtual object extFunction(string ns, string funcName, ArrayList argVec, object methodKey)
	  { //System.out.println("TransImpl.extFunction() " + ns + " " + funcName +" " + getExtensionsTable());
		return getExtensionsTable().extFunction(ns, funcName, argVec, methodKey, XPathContext.ExpressionContext);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object extFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector argVec) throws javax.xml.transform.TransformerException
	  public virtual object extFunction(FuncExtFunction extFunction, ArrayList argVec)
	  {
		return getExtensionsTable().extFunction(extFunction, argVec, XPathContext.ExpressionContext);
	  }

	  //=========================

	  /// <summary>
	  /// Reset the state.  This needs to be called after a process() call
	  /// is invoked, if the processor is to be used again.
	  /// </summary>
	  public virtual void reset()
	  {

		if (!m_hasBeenReset && m_shouldReset)
		{
		  m_hasBeenReset = true;

		  if (this.m_outputStream != null)
		  {
			try
			{
			  m_outputStream.Close();
			}
			catch (IOException)
			{
			}
		  }

		  m_outputStream = null;

		  // I need to look more carefully at which of these really
		  // needs to be reset.
		  m_countersTable = null;

		  m_xcontext.reset();

		  m_xcontext.VarStack.reset();
		  resetUserParameters();


		  m_currentTemplateElements.removeAllElements();
		  m_currentMatchTemplates.removeAllElements();
		  m_currentMatchedNodes.removeAllElements();

		  m_serializationHandler = null;
		  m_outputTarget = null;
		  m_keyManager = new KeyManager();
		  m_attrSetStack = null;
		  m_countersTable = null;
		  m_currentTemplateRuleIsNull = new BoolStack();
		  m_xmlSource = null;
		  m_doc = org.apache.xml.dtm.DTM_Fields.NULL;
		  m_isTransformDone = false;
		  m_transformThread = null;

		  // m_inputContentHandler = null;
		  // For now, reset the document cache each time.
		  m_xcontext.SourceTreeManager.reset();
		}

		//    m_reportInPostExceptionFromThread = false;
	  }

	  /// <summary>
	  /// <code>getProperty</code> returns the current setting of the
	  /// property described by the <code>property</code> argument.
	  /// 
	  /// %REVIEW% Obsolete now that source_location is handled in the TransformerFactory?
	  /// </summary>
	  /// <param name="property"> a <code>String</code> value </param>
	  /// <returns> a <code>boolean</code> value </returns>
	  public virtual bool getProperty(string property)
	  {
		return false;
	  }

	  /// <summary>
	  /// Set a runtime property for this <code>TransformerImpl</code>.
	  /// 
	  /// %REVIEW% Obsolete now that source_location is handled in the TransformerFactory?
	  /// </summary>
	  /// <param name="property"> a <code>String</code> value </param>
	  /// <param name="value"> an <code>Object</code> value </param>
	  public virtual void setProperty(string property, object value)
	  {
	  }

	  // ========= Transformer Interface Implementation ==========

	  /// <summary>
	  /// Get true if the parser events should be on the main thread,
	  /// false if not.  Experimental.  Can not be set right now.
	  /// </summary>
	  /// <returns> true if the parser events should be on the main thread,
	  /// false if not.
	  /// @xsl.usage experimental </returns>
	  public virtual bool ParserEventsOnMain
	  {
		  get
		  {
			return m_parserEventsOnMain;
		  }
	  }

	  /// <summary>
	  /// Get the thread that the transform process is on.
	  /// </summary>
	  /// <returns> The thread that the transform process is on, or null.
	  /// @xsl.usage internal </returns>
	  public virtual Thread TransformThread
	  {
		  get
		  {
			return m_transformThread;
		  }
		  set
		  {
			m_transformThread = value;
		  }
	  }


	  /// <summary>
	  /// NEEDSDOC Field m_hasTransformThreadErrorCatcher </summary>
	  private bool m_hasTransformThreadErrorCatcher = false;

	  /// <summary>
	  /// Return true if the transform was initiated from the transform method,
	  /// otherwise it was probably done from a pure parse events.
	  /// 
	  /// NEEDSDOC ($objectName$) @return
	  /// </summary>
	  public virtual bool hasTransformThreadErrorCatcher()
	  {
		return m_hasTransformThreadErrorCatcher;
	  }

			/// <summary>
			/// Process the source tree to SAX parse events. </summary>
			/// <param name="source">  The input for the source tree.
			/// </param>
			/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(javax.xml.transform.Source source) throws javax.xml.transform.TransformerException
	  public virtual void transform(Source source)
	  {
					transform(source, true);
	  }

	  /// <summary>
	  /// Process the source tree to SAX parse events. </summary>
	  /// <param name="source">  The input for the source tree. </param>
	  /// <param name="shouldRelease">  Flag indicating whether to release DTMManager.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(javax.xml.transform.Source source, boolean shouldRelease) throws javax.xml.transform.TransformerException
	  public virtual void transform(Source source, bool shouldRelease)
	  {

		try
		{

		  // Patch for bugzilla #13863.  If we don't reset the namespaceContext
		  // then we will get a NullPointerException if transformer is reused 
		  // (for stylesheets that use xsl:key).  Not sure if this should go 
		  // here or in reset(). -is  
		  if (XPathContext.NamespaceContext == null)
		  {
			 XPathContext.NamespaceContext = Stylesheet;
		  }
		  string @base = source.SystemId;

		  // If no systemID of the source, use the base of the stylesheet.
		  if (null == @base)
		  {
			@base = m_stylesheetRoot.BaseIdentifier;
		  }

		  // As a last resort, use the current user dir.
		  if (null == @base)
		  {
			string currentDir = "";
			try
			{
			  currentDir = System.getProperty("user.dir");
			}
			catch (SecurityException)
			{
			} // user.dir not accessible from applet

			if (currentDir.StartsWith(java.io.File.separator, StringComparison.Ordinal))
			{
			  @base = "file://" + currentDir;
			}
			else
			{
			  @base = "file:///" + currentDir;
			}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			@base = @base + System.IO.Path.DirectorySeparatorChar + source.GetType().FullName;
		  }
		  BaseURLOfSource = @base;
		  DTMManager mgr = m_xcontext.DTMManager;
		  /*
		   * According to JAXP1.2, new SAXSource()/StreamSource()
		   * should create an empty input tree, with a default root node. 
		   * new DOMSource()creates an empty document using DocumentBuilder.
		   * newDocument(); Use DocumentBuilder.newDocument() for all 3 situations,
		   * since there is no clear spec. how to create an empty tree when
		   * both SAXSource() and StreamSource() are used.
		   */
		  if ((source is StreamSource && source.SystemId == null && ((StreamSource)source).InputStream == null && ((StreamSource)source).Reader == null) || (source is SAXSource && ((SAXSource)source).InputSource == null && ((SAXSource)source).XMLReader == null) || (source is DOMSource && ((DOMSource)source).Node == null))
		  {
			try
			{
			  DocumentBuilderFactory builderF = DocumentBuilderFactory.newInstance();
			  DocumentBuilder builder = builderF.newDocumentBuilder();
			  string systemID = source.SystemId;
			  source = new DOMSource(builder.newDocument());

			  // Copy system ID from original, empty Source to new Source
			  if (!string.ReferenceEquals(systemID, null))
			  {
				source.SystemId = systemID;
			  }
			}
			catch (ParserConfigurationException e)
			{
			  fatalError(e);
			}
		  }
		  DTM dtm = mgr.getDTM(source, false, this, true, true);
		  dtm.DocumentBaseURI = @base;

		  bool hardDelete = true; // %REVIEW% I have to think about this. -sb

		  try
		  {
			  // NOTE: This will work because this is _NOT_ a shared DTM, and thus has
			  // only a single Document node. If it could ever be an RTF or other
			  // shared DTM, look at dtm.getDocumentRoot(nodeHandle).
			this.transformNode(dtm.Document);
		  }
		  finally
		  {
			if (shouldRelease)
			{
			  mgr.release(dtm, hardDelete);
			}
		  }

		  // Kick off the parse.  When the ContentHandler gets 
		  // the startDocument event, it will call transformNode( node ).
		  // reader.parse( xmlSource );
		  // This has to be done to catch exceptions thrown from 
		  // the transform thread spawned by the STree handler.
		  Exception e = ExceptionThrown;

		  if (null != e)
		  {
			if (e is TransformerException)
			{
			  throw (TransformerException) e;
			}
			else if (e is org.apache.xml.utils.WrappedRuntimeException)
			{
			  fatalError(((org.apache.xml.utils.WrappedRuntimeException) e).Exception);
			}
			else
			{
			  throw new TransformerException(e);
			}
		  }
		  else if (null != m_serializationHandler)
		  {
			m_serializationHandler.endDocument();
		  }
		}
		catch (org.apache.xml.utils.WrappedRuntimeException wre)
		{
		  Exception throwable = wre.Exception;

		  while (throwable is org.apache.xml.utils.WrappedRuntimeException)
		  {
			throwable = ((org.apache.xml.utils.WrappedRuntimeException) throwable).Exception;
		  }

		  fatalError(throwable);
		}

		// Patch attributed to David Eisenberg <david@catcode.com>
		catch (org.xml.sax.SAXParseException spe)
		{
		  fatalError(spe);
		}
		catch (SAXException se)
		{
		  m_errorHandler.fatalError(new TransformerException(se));
		}
		finally
		{
		  m_hasTransformThreadErrorCatcher = false;

		  // This looks to be redundent to the one done in TransformNode.
		  reset();
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fatalError(Throwable throwable) throws javax.xml.transform.TransformerException
	  private void fatalError(Exception throwable)
	  {
		if (throwable is org.xml.sax.SAXParseException)
		{
		  m_errorHandler.fatalError(new TransformerException(throwable.Message,new SAXSourceLocator((org.xml.sax.SAXParseException)throwable)));
		}
		else
		{
		  m_errorHandler.fatalError(new TransformerException(throwable));
		}

	  }

	  /// <summary>
	  /// Get the base URL of the source.
	  /// </summary>
	  /// <returns> The base URL of the source tree, or null. </returns>
	  public virtual string BaseURLOfSource
	  {
		  get
		  {
			return m_urlOfSource;
		  }
		  set
		  {
			m_urlOfSource = value;
		  }
	  }


	  /// <summary>
	  /// Get the original output target.
	  /// </summary>
	  /// <returns> The Result object used to kick of the transform or null. </returns>
	  public virtual Result OutputTarget
	  {
		  get
		  {
			return m_outputTarget;
		  }
		  set
		  {
			m_outputTarget = value;
		  }
	  }


	  /// <summary>
	  /// Get an output property that is in effect for the
	  /// transformation.  The property specified may be a property
	  /// that was set with setOutputProperty, or it may be a
	  /// property specified in the stylesheet.
	  /// </summary>
	  /// NEEDSDOC <param name="qnameString">
	  /// </param>
	  /// <returns> The string value of the output property, or null
	  /// if no property was found.
	  /// </returns>
	  /// <exception cref="IllegalArgumentException"> If the property is not supported.
	  /// </exception>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOutputProperty(String qnameString) throws IllegalArgumentException
	  public virtual string getOutputProperty(string qnameString)
	  {

		string value = null;
		OutputProperties props = OutputFormat;

		value = props.getProperty(qnameString);

		if (null == value)
		{
		  if (!OutputProperties.isLegalPropertyKey(qnameString))
		  {
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, new object[]{qnameString})); //"output property not recognized: "
		  }
											   //+ qnameString);
		}

		return value;
	  }

	  /// <summary>
	  /// Get the value of a property, without using the default properties.  This
	  /// can be used to test if a property has been explicitly set by the stylesheet
	  /// or user.
	  /// </summary>
	  /// NEEDSDOC <param name="qnameString">
	  /// </param>
	  /// <returns> The value of the property, or null if not found.
	  /// </returns>
	  /// <exception cref="IllegalArgumentException"> If the property is not supported,
	  /// and is not namespaced. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getOutputPropertyNoDefault(String qnameString) throws IllegalArgumentException
	  public virtual string getOutputPropertyNoDefault(string qnameString)
	  {

		string value = null;
		OutputProperties props = OutputFormat;

		value = (string) props.Properties.get(qnameString);

		if (null == value)
		{
		  if (!OutputProperties.isLegalPropertyKey(qnameString))
		  {
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, new object[]{qnameString})); //"output property not recognized: "
		  }
											  // + qnameString);
		}

		return value;
	  }

	  /// <summary>
	  /// This method is used to set or override the value
	  /// of the effective xsl:output attribute values
	  /// specified in the stylesheet.
	  /// <para>
	  /// The recognized standard output properties are:
	  /// <ul>
	  /// <li>cdata-section-elements
	  /// <li>doctype-system
	  /// <li>doctype-public
	  /// <li>indent
	  /// <li>media-type
	  /// <li>method
	  /// <li>omit-xml-declaration
	  /// <li>standalone
	  /// <li>version
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// For example:
	  /// <pre>
	  ///   tran.setOutputProperty("standalone", "yes");
	  /// </pre>
	  /// </para>
	  /// <para>
	  /// In the case of the cdata-section-elements property,
	  /// the value should be a whitespace separated list of
	  /// element names.  The element name is the local name
	  /// of the element, if it is in no namespace, or, the URI
	  /// in braces followed immediately by the local name
	  /// if the element is in that namespace. For example: 
	  /// <pre>
	  /// tran.setOutputProperty(
	  ///   "cdata-section-elements", 
	  ///   "elem1 {http://example.uri}elem2 elem3");
	  /// </pre>
	  /// </para>
	  /// <para>
	  /// The recognized Xalan extension elements are: 
	  /// <ul>
	  /// <li>content-handler
	  /// <li>entities
	  /// <li>indent-amount
	  /// <li>line-separator
	  /// <li>omit-meta-tag
	  /// <li>use-url-escaping
	  /// </ul>
	  /// </para>
	  /// <para>
	  /// These must be in the extension namespace of
	  /// "http://xml.apache.org/xalan".  This is accomplished
	  /// by putting the namespace URI in braces before the 
	  /// property name, for example:
	  /// <pre>
	  ///   tran.setOutputProperty(
	  ///     "{http://xml.apache.org/xalan}line-separator" ,
	  ///     "\n");
	  /// </pre> 
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="name"> The property name. </param>
	  /// <param name="value"> The requested value for the property. </param>
	  /// <exception cref="IllegalArgumentException"> if the property name is not legal. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOutputProperty(String name, String value) throws IllegalArgumentException
	  public virtual void setOutputProperty(string name, string value)
	  {

		lock (m_reentryGuard)
		{

		  // Get the output format that was set by the user, otherwise get the 
		  // output format from the stylesheet.
		  if (null == m_outputFormat)
		  {
			m_outputFormat = (OutputProperties) Stylesheet.OutputComposed.clone();
		  }

		  if (!OutputProperties.isLegalPropertyKey(name))
		  {
			throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, new object[]{name})); //"output property not recognized: "
		  }
											   //+ name);

		  m_outputFormat.setProperty(name, value);
		}
	  }

	  /// <summary>
	  /// Set the output properties for the transformation.  These
	  /// properties will override properties set in the templates
	  /// with xsl:output.
	  /// 
	  /// <para>If argument to this function is null, any properties
	  /// previously set will be removed.</para>
	  /// </summary>
	  /// <param name="oformat"> A set of output properties that will be
	  /// used to override any of the same properties in effect
	  /// for the transformation.
	  /// </param>
	  /// <seealso cref= javax.xml.transform.OutputKeys </seealso>
	  /// <seealso cref= java.util.Properties
	  /// </seealso>
	  /// <exception cref="IllegalArgumentException"> if any of the argument keys are not
	  /// recognized and are not namespace qualified.    </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setOutputProperties(java.util.Properties oformat) throws IllegalArgumentException
	  public virtual Properties OutputProperties
	  {
		  set
		  {
    
			lock (m_reentryGuard)
			{
			  if (null != value)
			  {
    
				// See if an *explicit* method was set.
				string method = (string) value.get(OutputKeys.METHOD);
    
				if (null != method)
				{
				  m_outputFormat = new OutputProperties(method);
				}
				else if (m_outputFormat == null)
				{
				  m_outputFormat = new OutputProperties();
				}
    
				m_outputFormat.copyFrom(value);
				// copyFrom does not set properties that have been already set, so 
				// this must be called after, which is a bit in the reverse from 
				// what one might think.
				m_outputFormat.copyFrom(m_stylesheetRoot.OutputProperties);
			  }
			  else
			  {
				// if value is null JAXP says that any props previously set are removed
				// and we are to revert back to those in the templates object (i.e. Stylesheet).
				m_outputFormat = null;
			  }
			}
		  }
		  get
		  {
			return (Properties) OutputFormat.Properties.clone();
		  }
	  }


		/// <summary>
		/// Create a result ContentHandler from a Result object, based
		/// on the current OutputProperties.
		/// </summary>
		/// <param name="outputTarget"> Where the transform result should go,
		/// should not be null.
		/// </param>
		/// <returns> A valid ContentHandler that will create the
		/// result tree when it is fed SAX events.
		/// </returns>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler createSerializationHandler(javax.xml.transform.Result outputTarget) throws javax.xml.transform.TransformerException
		public virtual SerializationHandler createSerializationHandler(Result outputTarget)
		{
		   SerializationHandler xoh = createSerializationHandler(outputTarget, OutputFormat);
		   return xoh;
		}

		/// <summary>
		/// Create a ContentHandler from a Result object and an OutputProperties.
		/// </summary>
		/// <param name="outputTarget"> Where the transform result should go,
		/// should not be null. </param>
		/// <param name="format"> The OutputProperties object that will contain
		/// instructions on how to serialize the output.
		/// </param>
		/// <returns> A valid ContentHandler that will create the
		/// result tree when it is fed SAX events.
		/// </returns>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler createSerializationHandler(javax.xml.transform.Result outputTarget, org.apache.xalan.templates.OutputProperties format) throws javax.xml.transform.TransformerException
		public virtual SerializationHandler createSerializationHandler(Result outputTarget, OutputProperties format)
		{

		  SerializationHandler xoh;

		  // If the Result object contains a Node, then create
		  // a ContentHandler that will add nodes to the input node.
		  org.w3c.dom.Node outputNode = null;

		  if (outputTarget is DOMResult)
		  {
			outputNode = ((DOMResult) outputTarget).Node;
			org.w3c.dom.Node nextSibling = ((DOMResult)outputTarget).NextSibling;

			org.w3c.dom.Document doc;
			short type;

			if (null != outputNode)
			{
			  type = outputNode.NodeType;
			  doc = (org.w3c.dom.Node.DOCUMENT_NODE == type) ? (org.w3c.dom.Document) outputNode : outputNode.OwnerDocument;
			}
			else
			{
			  bool isSecureProcessing = m_stylesheetRoot.SecureProcessing;
			  doc = org.apache.xml.utils.DOMHelper.createDocument(isSecureProcessing);
			  outputNode = doc;
			  type = outputNode.NodeType;

			  ((DOMResult) outputTarget).Node = outputNode;
			}

			DOMBuilder handler = (org.w3c.dom.Node.DOCUMENT_FRAGMENT_NODE == type) ? new DOMBuilder(doc, (org.w3c.dom.DocumentFragment) outputNode) : new DOMBuilder(doc, outputNode);

			if (nextSibling != null)
			{
			  handler.NextSibling = nextSibling;
			}

			  string encoding = format.getProperty(OutputKeys.ENCODING);
			  xoh = new ToXMLSAXHandler(handler, (LexicalHandler)handler, encoding);
		  }
		  else if (outputTarget is SAXResult)
		  {
			ContentHandler handler = ((SAXResult) outputTarget).Handler;

			if (null == handler)
			{
			   throw new System.ArgumentException("handler can not be null for a SAXResult");
			}

			LexicalHandler lexHandler;
			if (handler is LexicalHandler)
			{
				lexHandler = (LexicalHandler) handler;
			}
			else
			{
				lexHandler = null;
			}

			string encoding = format.getProperty(OutputKeys.ENCODING);
			string method = format.getProperty(OutputKeys.METHOD);

			ToXMLSAXHandler toXMLSAXHandler = new ToXMLSAXHandler(handler, lexHandler, encoding);
			toXMLSAXHandler.ShouldOutputNSAttr = false;
			xoh = toXMLSAXHandler;


			string publicID = format.getProperty(OutputKeys.DOCTYPE_PUBLIC);
			string systemID = format.getProperty(OutputKeys.DOCTYPE_SYSTEM);
			if (!string.ReferenceEquals(systemID, null))
			{
				xoh.DoctypeSystem = systemID;
			}
			if (!string.ReferenceEquals(publicID, null))
			{
				xoh.DoctypePublic = publicID;
			}

			if (handler is TransformerClient)
			{
				XalanTransformState state = new XalanTransformState();
				((TransformerClient)handler).TransformState = state;
				((ToSAXHandler)xoh).TransformState = state;
			}


		  }

		  // Otherwise, create a ContentHandler that will serialize the
		  // result tree to either a stream or a writer.
		  else if (outputTarget is StreamResult)
		  {
			StreamResult sresult = (StreamResult) outputTarget;

			try
			{
			  SerializationHandler serializer = (SerializationHandler) SerializerFactory.getSerializer(format.Properties);

			  if (null != sresult.Writer)
			  {
				serializer.Writer = sresult.Writer;
			  }
			  else if (null != sresult.OutputStream)
			  {
				serializer.OutputStream = sresult.OutputStream;
			  }
			  else if (null != sresult.SystemId)
			  {
				string fileURL = sresult.SystemId;

				if (fileURL.StartsWith("file:///", StringComparison.Ordinal))
				{
				  if (fileURL.Substring(8).IndexOf(":", StringComparison.Ordinal) > 0)
				  {
					fileURL = fileURL.Substring(8);
				  }
				  else
				  {
					fileURL = fileURL.Substring(7);
				  }
				}
				else if (fileURL.StartsWith("file:/", StringComparison.Ordinal))
				{
					if (fileURL.Substring(6).IndexOf(":", StringComparison.Ordinal) > 0)
					{
						fileURL = fileURL.Substring(6);
					}
					  else
					  {
						fileURL = fileURL.Substring(5);
					  }
				}

				m_outputStream = new System.IO.FileStream(fileURL, System.IO.FileMode.Create, System.IO.FileAccess.Write);

				serializer.OutputStream = m_outputStream;

				xoh = serializer;
			  }
			  else
			  {
				throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_OUTPUT_SPECIFIED, null)); //"No output specified!");
			  }

			  // handler = serializer.asContentHandler();

			//  this.setSerializer(serializer);

			  xoh = serializer;
			}
	//        catch (UnsupportedEncodingException uee)
	//        {
	//          throw new TransformerException(uee);
	//        }
			catch (IOException ioe)
			{
			  throw new TransformerException(ioe);
			}
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, new object[]{outputTarget.GetType().FullName})); //"Can't transform to a Result of type "
										   //+ outputTarget.getClass().getName()
										   //+ "!");
		  }

		  // before we forget, lets make the created handler hold a reference
		  // to the current TransformImpl object
		  xoh.Transformer = this;

		  SourceLocator srcLocator = Stylesheet;
		  xoh.SourceLocator = srcLocator;


		  return xoh;


		}

			/// <summary>
			/// Process the source tree to the output result. </summary>
			/// <param name="xmlSource">  The input for the source tree. </param>
			/// <param name="outputTarget"> The output source target.
			/// </param>
			/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(javax.xml.transform.Source xmlSource, javax.xml.transform.Result outputTarget) throws javax.xml.transform.TransformerException
	  public virtual void transform(Source xmlSource, Result outputTarget)
	  {
					transform(xmlSource, outputTarget, true);
	  }

	  /// <summary>
	  /// Process the source tree to the output result. </summary>
	  /// <param name="xmlSource">  The input for the source tree. </param>
	  /// <param name="outputTarget"> The output source target. </param>
	  /// <param name="shouldRelease">  Flag indicating whether to release DTMManager. 
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(javax.xml.transform.Source xmlSource, javax.xml.transform.Result outputTarget, boolean shouldRelease) throws javax.xml.transform.TransformerException
	  public virtual void transform(Source xmlSource, Result outputTarget, bool shouldRelease)
	  {

		lock (m_reentryGuard)
		{
		  SerializationHandler xoh = createSerializationHandler(outputTarget);
		  this.SerializationHandler = xoh;

		  m_outputTarget = outputTarget;

		  transform(xmlSource, shouldRelease);
		}
	  }

	  /// <summary>
	  /// Process the source node to the output result, if the
	  /// processor supports the "http://xml.org/trax/features/dom/input"
	  /// feature.
	  /// %REVIEW% Do we need a Node version of this? </summary>
	  /// <param name="node">  The input source node, which can be any valid DTM node. </param>
	  /// <param name="outputTarget"> The output source target.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transformNode(int node, javax.xml.transform.Result outputTarget) throws javax.xml.transform.TransformerException
	  public virtual void transformNode(int node, Result outputTarget)
	  {


		SerializationHandler xoh = createSerializationHandler(outputTarget);
		this.SerializationHandler = xoh;

		m_outputTarget = outputTarget;

		transformNode(node);
	  }

	  /// <summary>
	  /// Process the source node to the output result, if the
	  /// processor supports the "http://xml.org/trax/features/dom/input"
	  /// feature.
	  /// %REVIEW% Do we need a Node version of this? </summary>
	  /// <param name="node">  The input source node, which can be any valid DTM node.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transformNode(int node) throws javax.xml.transform.TransformerException
	  public virtual void transformNode(int node)
	  {
		//dml
		setExtensionsTable(Stylesheet);
		// Make sure we're not writing to the same output content handler.
		lock (m_serializationHandler)
		{
		  m_hasBeenReset = false;

		  XPathContext xctxt = XPathContext;
		  DTM dtm = xctxt.getDTM(node);

		  try
		  {
			pushGlobalVars(node);

			// ==========
			// Give the top-level templates a chance to pass information into 
			// the context (this is mainly for setting up tables for extensions).
			StylesheetRoot stylesheet = this.Stylesheet;
			int n = stylesheet.GlobalImportCount;

			for (int i = 0; i < n; i++)
			{
			  StylesheetComposed imported = stylesheet.getGlobalImport(i);
			  int includedCount = imported.IncludeCountComposed;

			  for (int j = -1; j < includedCount; j++)
			  {
				Stylesheet included = imported.getIncludeComposed(j);

				included.runtimeInit(this);

				for (ElemTemplateElement child = included.FirstChildElem; child != null; child = child.NextSiblingElem)
				{
				  child.runtimeInit(this);
				}
			  }
			}
			// ===========        
			// System.out.println("Calling applyTemplateToNode - "+Thread.currentThread().getName());
			DTMIterator dtmIter = new org.apache.xpath.axes.SelfIteratorNoPredicate();
			dtmIter.setRoot(node, xctxt);
			xctxt.pushContextNodeList(dtmIter);
			try
			{
			  this.applyTemplateToNode(null, null, node);
			}
			finally
			{
			  xctxt.popContextNodeList();
			}
			// m_stylesheetRoot.getStartRule().execute(this);

			// System.out.println("Done with applyTemplateToNode - "+Thread.currentThread().getName());
			if (null != m_serializationHandler)
			{
			  m_serializationHandler.endDocument();
			}
		  }
		  catch (Exception se)
		  {

			// System.out.println(Thread.currentThread().getName()+" threw an exception! "
			//                   +se.getMessage());
			// If an exception was thrown, we need to make sure that any waiting 
			// handlers can terminate, which I guess is best done by sending 
			// an endDocument.

			// SAXSourceLocator
			while (se is org.apache.xml.utils.WrappedRuntimeException)
			{
			  Exception e = ((org.apache.xml.utils.WrappedRuntimeException)se).Exception;
			  if (null != e)
			  {
				se = e;
			  }
			}

			if (null != m_serializationHandler)
			{
			  try
			  {
				if (se is org.xml.sax.SAXParseException)
				{
				  m_serializationHandler.fatalError((org.xml.sax.SAXParseException)se);
				}
				else if (se is TransformerException)
				{
				  TransformerException te = ((TransformerException)se);
				  SAXSourceLocator sl = new SAXSourceLocator(te.Locator);
				  m_serializationHandler.fatalError(new org.xml.sax.SAXParseException(te.Message, sl, te));
				}
				else
				{
				  m_serializationHandler.fatalError(new org.xml.sax.SAXParseException(se.Message, new SAXSourceLocator(), se));
				}
			  }
			  catch (Exception)
			  {
			  }
			}

			if (se is TransformerException)
			{
			  m_errorHandler.fatalError((TransformerException)se);
			}
			else if (se is org.xml.sax.SAXParseException)
			{
			  m_errorHandler.fatalError(new TransformerException(se.Message, new SAXSourceLocator((org.xml.sax.SAXParseException)se), se));
			}
			else
			{
			  m_errorHandler.fatalError(new TransformerException(se));
			}

		  }
		  finally
		  {
			this.reset();
		  }
		}
	  }

	  /// <summary>
	  /// Get a SAX2 ContentHandler for the input.
	  /// </summary>
	  /// <returns> A valid ContentHandler, which should never be null, as
	  /// long as getFeature("http://xml.org/trax/features/sax/input")
	  /// returns true. </returns>
	  public virtual ContentHandler InputContentHandler
	  {
		  get
		  {
			return getInputContentHandler(false);
		  }
	  }

	  /// <summary>
	  /// Get a SAX2 ContentHandler for the input.
	  /// </summary>
	  /// <param name="doDocFrag"> true if a DocumentFragment should be created as
	  /// the root, rather than a Document.
	  /// </param>
	  /// <returns> A valid ContentHandler, which should never be null, as
	  /// long as getFeature("http://xml.org/trax/features/sax/input")
	  /// returns true. </returns>
	  public virtual ContentHandler getInputContentHandler(bool doDocFrag)
	  {

		if (null == m_inputContentHandler)
		{

		  //      if(null == m_urlOfSource && null != m_stylesheetRoot)
		  //        m_urlOfSource = m_stylesheetRoot.getBaseIdentifier();
		  m_inputContentHandler = new TransformerHandlerImpl(this, doDocFrag, m_urlOfSource);
		}

		return m_inputContentHandler;
	  }

	  /// <summary>
	  /// Get a SAX2 DeclHandler for the input. </summary>
	  /// <returns> A valid DeclHandler, which should never be null, as
	  /// long as getFeature("http://xml.org/trax/features/sax/input")
	  /// returns true. </returns>
	  public virtual DeclHandler InputDeclHandler
	  {
		  get
		  {
    
			if (m_inputContentHandler is DeclHandler)
			{
			  return (DeclHandler) m_inputContentHandler;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// Get a SAX2 LexicalHandler for the input. </summary>
	  /// <returns> A valid LexicalHandler, which should never be null, as
	  /// long as getFeature("http://xml.org/trax/features/sax/input")
	  /// returns true. </returns>
	  public virtual LexicalHandler InputLexicalHandler
	  {
		  get
		  {
    
			if (m_inputContentHandler is LexicalHandler)
			{
			  return (LexicalHandler) m_inputContentHandler;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// Set the output properties for the transformation.  These
	  /// properties will override properties set in the templates
	  /// with xsl:output.
	  /// </summary>
	  /// <param name="oformat"> A valid OutputProperties object (which will
	  /// not be mutated), or null. </param>
	  public virtual OutputProperties OutputFormat
	  {
		  set
		  {
			m_outputFormat = value;
		  }
		  get
		  {
    
			// Get the output format that was set by the user, otherwise get the 
			// output format from the stylesheet.
			OutputProperties format = (null == m_outputFormat) ? Stylesheet.OutputComposed : m_outputFormat;
    
			return format;
		  }
	  }


	  /// <summary>
	  /// Set a parameter for the templates.
	  /// </summary>
	  /// <param name="name"> The name of the parameter. </param>
	  /// <param name="namespace"> The namespace of the parameter. </param>
	  /// <param name="value"> The value object.  This can be any valid Java object
	  /// -- it's up to the processor to provide the proper
	  /// coersion to the object, or simply pass it on for use
	  /// in extensions. </param>
	  public virtual void setParameter(string name, string @namespace, object value)
	  {

		VariableStack varstack = XPathContext.VarStack;
		QName qname = new QName(@namespace, name);
		XObject xobject = XObject.create(value, XPathContext);

		StylesheetRoot sroot = m_stylesheetRoot;
		ArrayList vars = sroot.VariablesAndParamsComposed;
		int i = vars.Count;
		while (--i >= 0)
		{
		  ElemVariable variable = (ElemVariable)vars[i];
		  if (variable.XSLToken == Constants.ELEMNAME_PARAMVARIABLE && variable.Name.Equals(qname))
		  {
			  varstack.setGlobalVariable(i, xobject);
		  }
		}
	  }

	  /// <summary>
	  /// NEEDSDOC Field m_userParams </summary>
	  internal ArrayList m_userParams;

	  /// <summary>
	  /// Set a parameter for the transformation.
	  /// </summary>
	  /// <param name="name"> The name of the parameter,
	  ///             which may have a namespace URI. </param>
	  /// <param name="value"> The value object.  This can be any valid Java object
	  /// -- it's up to the processor to provide the proper
	  /// coersion to the object, or simply pass it on for use
	  /// in extensions. </param>
	  public virtual void setParameter(string name, object value)
	  {

		if (value == null)
		{
		  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_SET_PARAM_VALUE, new object[]{name}));
		}

		StringTokenizer tokenizer = new StringTokenizer(name, "{}", false);

		try
		{

		  // The first string might be the namespace, or it might be 
		  // the local name, if the namespace is null.
		  string s1 = tokenizer.nextToken();
		  string s2 = tokenizer.hasMoreTokens() ? tokenizer.nextToken() : null;

		  if (null == m_userParams)
		  {
			m_userParams = new ArrayList();
		  }

		  if (null == s2)
		  {
			replaceOrPushUserParam(new QName(s1), XObject.create(value, XPathContext));
			setParameter(s1, null, value);
		  }
		  else
		  {
			replaceOrPushUserParam(new QName(s1, s2), XObject.create(value, XPathContext));
			setParameter(s2, s1, value);
		  }
		}
		catch (java.util.NoSuchElementException)
		{

		  // Should throw some sort of an error.
		}
	  }

	  /// <summary>
	  /// NEEDSDOC Method replaceOrPushUserParam 
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="qname"> </param>
	  /// NEEDSDOC <param name="xval"> </param>
	  private void replaceOrPushUserParam(QName qname, XObject xval)
	  {

		int n = m_userParams.Count;

		for (int i = n - 1; i >= 0; i--)
		{
		  Arg arg = (Arg) m_userParams[i];

		  if (arg.QName.Equals(qname))
		  {
			m_userParams[i] = new Arg(qname, xval, true);

			return;
		  }
		}

		m_userParams.Add(new Arg(qname, xval, true));
	  }

	  /// <summary>
	  /// Get a parameter that was explicitly set with setParameter
	  /// or setParameters.
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="name"> </param>
	  /// <returns> A parameter that has been set with setParameter
	  /// or setParameters,
	  /// *not* all the xsl:params on the stylesheet (which require
	  /// a transformation Source to be evaluated). </returns>
	  public virtual object getParameter(string name)
	  {

		try
		{

		  // VariableStack varstack = getXPathContext().getVarStack();
		  // The first string might be the namespace, or it might be 
		  // the local name, if the namespace is null.
		  QName qname = QName.getQNameFromString(name);

		  if (null == m_userParams)
		  {
			return null;
		  }

		  int n = m_userParams.Count;

		  for (int i = n - 1; i >= 0; i--)
		  {
			Arg arg = (Arg) m_userParams[i];

			if (arg.QName.Equals(qname))
			{
			  return arg.Val.@object();
			}
		  }

		  return null;
		}
		catch (java.util.NoSuchElementException)
		{

		  // Should throw some sort of an error.
		  return null;
		}
	  }

	  /// <summary>
	  /// Reset parameters that the user specified for the transformation.
	  /// Called during transformer.reset() after we have cleared the 
	  /// variable stack. We need to make sure that user params are
	  /// reset so that the transformer object can be reused. 
	  /// </summary>
	  private void resetUserParameters()
	  {

		try
		{

		  if (null == m_userParams)
		  {
			return;
		  }

		  int n = m_userParams.Count;
		  for (int i = n - 1; i >= 0; i--)
		  {
			Arg arg = (Arg) m_userParams[i];
			QName name = arg.QName;
			// The first string might be the namespace, or it might be 
			// the local name, if the namespace is null.
			string s1 = name.Namespace;
			string s2 = name.LocalPart;

			setParameter(s2, s1, arg.Val.@object());

		  }

		}
		catch (java.util.NoSuchElementException)
		{
		  // Should throw some sort of an error.

		}
	  }

	  /// <summary>
	  /// Set a bag of parameters for the transformation. Note that
	  /// these will not be additive, they will replace the existing
	  /// set of parameters.
	  /// </summary>
	  /// NEEDSDOC <param name="params"> </param>
	  public virtual Properties Parameters
	  {
		  set
		  {
    
			clearParameters();
    
			System.Collections.IEnumerator names = value.propertyNames();
    
			while (names.MoveNext())
			{
			  string name = value.getProperty((string) names.Current);
			  StringTokenizer tokenizer = new StringTokenizer(name, "{}", false);
    
			  try
			  {
    
				// The first string might be the namespace, or it might be 
				// the local name, if the namespace is null.
				string s1 = tokenizer.nextToken();
				string s2 = tokenizer.hasMoreTokens() ? tokenizer.nextToken() : null;
    
				if (null == s2)
				{
				  setParameter(s1, null, value.getProperty(name));
				}
				else
				{
				  setParameter(s2, s1, value.getProperty(name));
				}
			  }
			  catch (java.util.NoSuchElementException)
			  {
    
				// Should throw some sort of an error.
			  }
			}
		  }
	  }

	  /// <summary>
	  /// Reset the parameters to a null list.
	  /// </summary>
	  public virtual void clearParameters()
	  {

		lock (m_reentryGuard)
		{
		  VariableStack varstack = new VariableStack();

		  m_xcontext.VarStack = varstack;

		  m_userParams = null;
		}
	  }


	  /// <summary>
	  /// Internal -- push the global variables from the Stylesheet onto
	  /// the context's runtime variable stack.
	  /// <para>If we encounter a variable
	  /// that is already defined in the variable stack, we ignore it.  This
	  /// is because the second variable definition will be at a lower import
	  /// precedence.  Presumably, global"variables at the same import precedence
	  /// with the same name will have been caught during the recompose process.
	  /// </para>
	  /// <para>However, if we encounter a parameter that is already defined in the
	  /// variable stack, we need to see if this is a parameter whose value was
	  /// supplied by a setParameter call.  If so, we need to "receive" the one
	  /// already in the stack, ignoring this one.  If it is just an earlier
	  /// xsl:param or xsl:variable definition, we ignore it using the same
	  /// reasoning as explained above for the variable.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="contextNode"> The root of the source tree, can't be null.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void pushGlobalVars(int contextNode) throws javax.xml.transform.TransformerException
	  protected internal virtual void pushGlobalVars(int contextNode)
	  {

		XPathContext xctxt = m_xcontext;
		VariableStack vs = xctxt.VarStack;
		StylesheetRoot sr = Stylesheet;
		ArrayList vars = sr.VariablesAndParamsComposed;

		int i = vars.Count;
		vs.link(i);

		while (--i >= 0)
		{
		  ElemVariable v = (ElemVariable) vars[i];

		  // XObject xobj = v.getValue(this, contextNode);
		  XObject xobj = new XUnresolvedVariable(v, contextNode, this, vs.StackFrame, 0, true);

		  if (null == vs.elementAt(i))
		  {
			vs.setGlobalVariable(i, xobj);
		  }
		}

	  }

	  /// <summary>
	  /// Set an object that will be used to resolve URIs used in
	  /// document(), etc. </summary>
	  /// <param name="resolver"> An object that implements the URIResolver interface,
	  /// or null. </param>
	  public virtual URIResolver URIResolver
	  {
		  set
		  {
    
			lock (m_reentryGuard)
			{
			  m_xcontext.SourceTreeManager.URIResolver = value;
			}
		  }
		  get
		  {
			return m_xcontext.SourceTreeManager.URIResolver;
		  }
	  }


	  // ======== End Transformer Implementation ========  

	  /// <summary>
	  /// Set the content event handler.
	  /// </summary>
	  /// NEEDSDOC <param name="handler"> </param>
	  /// <exception cref="java.lang.NullPointerException"> If the handler
	  ///            is null. </exception>
	  /// <seealso cref= org.xml.sax.XMLReader#setContentHandler </seealso>
	  public virtual ContentHandler ContentHandler
	  {
		  set
		  {
    
			if (value == null)
			{
			  throw new System.NullReferenceException(XSLMessages.createMessage(XSLTErrorResources.ER_NULL_CONTENT_HANDLER, null)); //"Null content handler");
			}
			else
			{
			  m_outputContentHandler = value;
    
			  if (null == m_serializationHandler)
			  {
				ToXMLSAXHandler h = new ToXMLSAXHandler();
				h.ContentHandler = value;
				h.Transformer = this;
    
				m_serializationHandler = h;
			  }
			  else
			  {
				m_serializationHandler.ContentHandler = value;
			  }
			}
		  }
		  get
		  {
			return m_outputContentHandler;
		  }
	  }


	  /// <summary>
	  /// Given a stylesheet element, create a result tree fragment from it's
	  /// contents. The fragment will be built within the shared RTF DTM system
	  /// used as a variable stack. </summary>
	  /// <param name="templateParent"> The template element that holds the fragment. </param>
	  /// <returns> the NodeHandle for the root node of the resulting RTF.
	  /// </returns>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int transformToRTF(org.apache.xalan.templates.ElemTemplateElement templateParent) throws javax.xml.transform.TransformerException
	  public virtual int transformToRTF(ElemTemplateElement templateParent)
	  {
		// Retrieve a DTM to contain the RTF. At this writing, this may be a
		// multi-document DTM (SAX2RTFDTM).
		DTM dtmFrag = m_xcontext.RTFDTM;
		return transformToRTF(templateParent,dtmFrag);
	  }

	  /// <summary>
	  /// Given a stylesheet element, create a result tree fragment from it's
	  /// contents. The fragment will also use the shared DTM system, but will
	  /// obtain its space from the global variable pool rather than the dynamic
	  /// variable stack. This allows late binding of XUnresolvedVariables without
	  /// the risk that their content will be discarded when the variable stack
	  /// is popped.
	  /// </summary>
	  /// <param name="templateParent"> The template element that holds the fragment. </param>
	  /// <returns> the NodeHandle for the root node of the resulting RTF.
	  /// </returns>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int transformToGlobalRTF(org.apache.xalan.templates.ElemTemplateElement templateParent) throws javax.xml.transform.TransformerException
	  public virtual int transformToGlobalRTF(ElemTemplateElement templateParent)
	  {
		// Retrieve a DTM to contain the RTF. At this writing, this may be a
		// multi-document DTM (SAX2RTFDTM).
		DTM dtmFrag = m_xcontext.GlobalRTFDTM;
		return transformToRTF(templateParent,dtmFrag);
	  }

	  /// <summary>
	  /// Given a stylesheet element, create a result tree fragment from it's
	  /// contents. </summary>
	  /// <param name="templateParent"> The template element that holds the fragment. </param>
	  /// <param name="dtmFrag"> The DTM to write the RTF into </param>
	  /// <returns> the NodeHandle for the root node of the resulting RTF.
	  /// </returns>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int transformToRTF(org.apache.xalan.templates.ElemTemplateElement templateParent,org.apache.xml.dtm.DTM dtmFrag) throws javax.xml.transform.TransformerException
	  private int transformToRTF(ElemTemplateElement templateParent, DTM dtmFrag)
	  {

		XPathContext xctxt = m_xcontext;

		ContentHandler rtfHandler = dtmFrag.ContentHandler;

		// Obtain the ResultTreeFrag's root node.
		// NOTE: In SAX2RTFDTM, this value isn't available until after
		// the startDocument has been issued, so assignment has been moved
		// down a bit in the code.
		int resultFragment; // not yet reliably = dtmFrag.getDocument();

		// Save the current result tree handler.
		SerializationHandler savedRTreeHandler = this.m_serializationHandler;


		// And make a new handler for the RTF.
		ToSAXHandler h = new ToXMLSAXHandler();
		h.ContentHandler = rtfHandler;
		h.Transformer = this;

		// Replace the old handler (which was already saved)
		m_serializationHandler = h;

		// use local variable for the current handler
		SerializationHandler rth = m_serializationHandler;

		try
		{
		  rth.startDocument();

		  // startDocument is "bottlenecked" in RTH. We need it acted upon immediately,
		  // to set the DTM's state as in-progress, so that if the xsl:variable's body causes
		  // further RTF activity we can keep that from bashing this DTM.
		  rth.flushPending();

		  try
		  {

			// Do the transformation of the child elements.
			executeChildTemplates(templateParent, true);

			// Make sure everything is flushed!
			rth.flushPending();

			// Get the document ID. May not exist until the RTH has not only
			// received, but flushed, the startDocument, and may be invalid
			// again after the document has been closed (still debating that)
			// ... so waiting until just before the end seems simplest/safest. 
		resultFragment = dtmFrag.Document;
		  }
		  finally
		  {
			rth.endDocument();
		  }
		}
		catch (SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{

		  // Restore the previous result tree handler.
		  this.m_serializationHandler = savedRTreeHandler;
		}

		return resultFragment;
	  }

	  /// <summary>
	  /// Get the StringWriter pool, so that StringWriter
	  /// objects may be reused.
	  /// </summary>
	  /// <returns> The string writer pool, not null.
	  /// @xsl.usage internal </returns>
	  public virtual ObjectPool StringWriterPool
	  {
		  get
		  {
			return m_stringWriterObjectPool;
		  }
	  }

	  /// <summary>
	  /// Take the contents of a template element, process it, and
	  /// convert it to a string.
	  /// </summary>
	  /// <param name="elem"> The parent element whose children will be output
	  /// as a string.
	  /// </param>
	  /// <returns> The stringized result of executing the elements children.
	  /// </returns>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String transformToString(org.apache.xalan.templates.ElemTemplateElement elem) throws javax.xml.transform.TransformerException
	  public virtual string transformToString(ElemTemplateElement elem)
	  {
		ElemTemplateElement firstChild = elem.FirstChildElem;
		if (null == firstChild)
		{
		  return "";
		}
		if (elem.hasTextLitOnly() && m_optimizer)
		{
		  return ((ElemTextLiteral)firstChild).NodeValue;
		}

		// Save the current result tree handler.
		SerializationHandler savedRTreeHandler = this.m_serializationHandler;

		// Create a Serializer object that will handle the SAX events 
		// and build the ResultTreeFrag nodes.
		StringWriter sw = (StringWriter) m_stringWriterObjectPool.Instance;

		m_serializationHandler = (ToTextStream) m_textResultHandlerObjectPool.Instance;

		  if (null == m_serializationHandler)
		  {
			// if we didn't get one from the pool, go make a new one


			Serializer serializer = SerializerFactory.getSerializer(m_textformat.Properties);
			m_serializationHandler = (SerializationHandler) serializer;
		  }

			m_serializationHandler.Transformer = this;
			m_serializationHandler.Writer = sw;


		string result;

		try
		{
			/* Don't call startDocument, the SerializationHandler  will
			 * generate its own internal startDocument call anyways
			 */
		  // this.m_serializationHandler.startDocument();

		  // Do the transformation of the child elements.
		  executeChildTemplates(elem, true);
			this.m_serializationHandler.endDocument();

		  result = sw.ToString();
		}
		catch (SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{
		  sw.Buffer.Length = 0;

		  try
		  {
			sw.close();
		  }
		  catch (Exception)
		  {
		  }

		  m_stringWriterObjectPool.freeInstance(sw);
		  m_serializationHandler.reset();
		  m_textResultHandlerObjectPool.freeInstance(m_serializationHandler);

		  // Restore the previous result tree handler.
		  m_serializationHandler = savedRTreeHandler;
		}

		return result;
	  }

	  /// <summary>
	  /// Given an element and mode, find the corresponding
	  /// template and process the contents.
	  /// </summary>
	  /// <param name="xslInstruction"> The calling element. </param>
	  /// <param name="template"> The template to use if xsl:for-each, current template for apply-imports, or null. </param>
	  /// <param name="child"> The source context node. </param>
	  /// <exception cref="TransformerException"> </exception>
	  /// <returns> true if applied a template, false if not.
	  /// @xsl.usage advanced </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean applyTemplateToNode(org.apache.xalan.templates.ElemTemplateElement xslInstruction, org.apache.xalan.templates.ElemTemplate template, int child) throws javax.xml.transform.TransformerException
	  public virtual bool applyTemplateToNode(ElemTemplateElement xslInstruction, ElemTemplate template, int child) // xsl:apply-templates or xsl:for-each
	  {

		DTM dtm = m_xcontext.getDTM(child);
		short nodeType = dtm.getNodeType(child);
		bool isDefaultTextRule = false;
		bool isApplyImports = false;

		isApplyImports = ((xslInstruction == null) ? false : xslInstruction.XSLToken == Constants.ELEMNAME_APPLY_IMPORTS);

		if (null == template || isApplyImports)
		{
		  int maxImportLevel , endImportLevel = 0;

		  if (isApplyImports)
		  {
			maxImportLevel = template.StylesheetComposed.ImportCountComposed - 1;
			endImportLevel = template.StylesheetComposed.EndImportCountComposed;
		  }
		  else
		  {
			maxImportLevel = -1;
		  }

		  // If we're trying an xsl:apply-imports at the top level (ie there are no
		  // imported stylesheets), we need to indicate that there is no matching template.
		  // The above logic will calculate a maxImportLevel of -1 which indicates
		  // that we should find any template.  This is because a value of -1 for
		  // maxImportLevel has a special meaning.  But we don't want that.
		  // We want to match -no- templates. See bugzilla bug 1170.
		  if (isApplyImports && (maxImportLevel == -1))
		  {
			template = null;
		  }
		  else
		  {

			// Find the XSL template that is the best match for the 
			// element.        
			XPathContext xctxt = m_xcontext;

			try
			{
			  xctxt.pushNamespaceContext(xslInstruction);

			  QName mode = this.Mode;

			  if (isApplyImports)
			  {
				template = m_stylesheetRoot.getTemplateComposed(xctxt, child, mode, maxImportLevel, endImportLevel, m_quietConflictWarnings, dtm);
			  }
			  else
			  {
				template = m_stylesheetRoot.getTemplateComposed(xctxt, child, mode, m_quietConflictWarnings, dtm);
			  }

			}
			finally
			{
			  xctxt.popNamespaceContext();
			}
		  }

		  // If that didn't locate a node, fall back to a default template rule.
		  // See http://www.w3.org/TR/xslt#built-in-rule.
		  if (null == template)
		  {
			switch (nodeType)
			{
			case org.apache.xml.dtm.DTM_Fields.DOCUMENT_FRAGMENT_NODE :
			case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
			  template = m_stylesheetRoot.DefaultRule;
			  break;
			case org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE :
			case org.apache.xml.dtm.DTM_Fields.TEXT_NODE :
			case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
			  template = m_stylesheetRoot.DefaultTextRule;
			  isDefaultTextRule = true;
			  break;
			case org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE :
			  template = m_stylesheetRoot.DefaultRootRule;
			  break;
			default :

			  // No default rules for processing instructions and the like.
			  return false;
			}
		  }
		}

		// If we are processing the default text rule, then just clone 
		// the value directly to the result tree.
		try
		{
		  pushElemTemplateElement(template);
		  m_xcontext.pushCurrentNode(child);
		  pushPairCurrentMatched(template, child);

		  // Fix copy copy29 test.
		  if (!isApplyImports)
		  {
			  DTMIterator cnl = new org.apache.xpath.NodeSetDTM(child, m_xcontext.DTMManager);
			  m_xcontext.pushContextNodeList(cnl);
		  }

		  if (isDefaultTextRule)
		  {
			switch (nodeType)
			{
			case org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE :
			case org.apache.xml.dtm.DTM_Fields.TEXT_NODE :
			  ClonerToResultTree.cloneToResultTree(child, nodeType, dtm, ResultTreeHandler, false);
			  break;
			case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
			  dtm.dispatchCharactersEvents(child, ResultTreeHandler, false);
			  break;
			}
		  }
		  else
		  {

			// Fire a trace event for the template.

			if (m_debug)
			{
			  TraceManager.fireTraceEvent(template);
			}
			// And execute the child templates.
			// 9/11/00: If template has been compiled, hand off to it
			// since much (most? all?) of the processing has been inlined.
			// (It would be nice if there was a single entry point that
			// worked for both... but the interpretive system works by
			// having the Tranformer execute the children, while the
			// compiled obviously has to run its own code. It's
			// also unclear that "execute" is really the right name for
			// that entry point.)
			m_xcontext.SAXLocator = template;
			// m_xcontext.getVarStack().link();
			m_xcontext.VarStack.link(template.m_frameSize);
			executeChildTemplates(template, true);

			if (m_debug)
			{
			  TraceManager.fireTraceEndEvent(template);
			}
		  }
		}
		catch (SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{
		  if (!isDefaultTextRule)
		  {
			m_xcontext.VarStack.unlink();
		  }
		  m_xcontext.popCurrentNode();
		  if (!isApplyImports)
		  {
			  m_xcontext.popContextNodeList();
		  }
		  popCurrentMatched();

		  popElemTemplateElement();
		}

		return true;
	  }


	  /// <summary>
	  /// Execute each of the children of a template element.  This method
	  /// is only for extension use.
	  /// </summary>
	  /// <param name="elem"> The ElemTemplateElement that contains the children
	  /// that should execute. </param>
	  /// NEEDSDOC <param name="context"> </param>
	  /// <param name="mode"> The current mode. </param>
	  /// <param name="handler"> The ContentHandler to where the result events
	  /// should be fed.
	  /// </param>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeChildTemplates(org.apache.xalan.templates.ElemTemplateElement elem, org.w3c.dom.Node context, org.apache.xml.utils.QName mode, org.xml.sax.ContentHandler handler) throws javax.xml.transform.TransformerException
	  public virtual void executeChildTemplates(ElemTemplateElement elem, org.w3c.dom.Node context, QName mode, ContentHandler handler)
	  {

		XPathContext xctxt = m_xcontext;

		try
		{
		  if (null != mode)
		  {
			pushMode(mode);
		  }
		  xctxt.pushCurrentNode(xctxt.getDTMHandleFromNode(context));
		  executeChildTemplates(elem, handler);
		}
		finally
		{
		  xctxt.popCurrentNode();

		  // I'm not sure where or why this was here.  It is clearly in 
		  // error though, without a corresponding pushMode().
		  if (null != mode)
		  {
			popMode();
		  }
		}
	  }

	  /// <summary>
	  /// Execute each of the children of a template element.
	  /// </summary>
	  /// <param name="elem"> The ElemTemplateElement that contains the children
	  /// that should execute. </param>
	  /// <param name="shouldAddAttrs"> true if xsl:attributes should be executed.
	  /// </param>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeChildTemplates(org.apache.xalan.templates.ElemTemplateElement elem, boolean shouldAddAttrs) throws javax.xml.transform.TransformerException
	  public virtual void executeChildTemplates(ElemTemplateElement elem, bool shouldAddAttrs)
	  {

		// Does this element have any children?
		ElemTemplateElement t = elem.FirstChildElem;

		if (null == t)
		{
		  return;
		}

		if (elem.hasTextLitOnly() && m_optimizer)
		{
		  char[] chars = ((ElemTextLiteral)t).Chars;
		  try
		  {
			// Have to push stuff on for tooling...
			this.pushElemTemplateElement(t);
			m_serializationHandler.characters(chars, 0, chars.Length);
		  }
		  catch (SAXException se)
		  {
			throw new TransformerException(se);
		  }
		  finally
		  {
			this.popElemTemplateElement();
		  }
		  return;
		}

	//    // Check for infinite loops if we have to.
	//    boolean check = (m_stackGuard.m_recursionLimit > -1);
	//
	//    if (check)
	//      getStackGuard().push(elem, xctxt.getCurrentNode());

		XPathContext xctxt = m_xcontext;
		xctxt.pushSAXLocatorNull();
		int currentTemplateElementsTop = m_currentTemplateElements.size();
		m_currentTemplateElements.push(null);

		try
		{
		  // Loop through the children of the template, calling execute on 
		  // each of them.
		  for (; t != null; t = t.NextSiblingElem)
		  {
			if (!shouldAddAttrs && t.XSLToken == Constants.ELEMNAME_ATTRIBUTE)
			{
			  continue;
			}

			xctxt.SAXLocator = t;
			m_currentTemplateElements.setElementAt(t,currentTemplateElementsTop);
			t.execute(this);
		  }
		}
		catch (Exception re)
		{
			TransformerException te = new TransformerException(re);
			te.Locator = t;
			throw te;
		}
		finally
		{
		  m_currentTemplateElements.pop();
		  xctxt.popSAXLocator();
		}

		// Check for infinite loops if we have to
	//    if (check)
	//      getStackGuard().pop();
	  }
		/// <summary>
		/// Execute each of the children of a template element.
		/// </summary>
		/// <param name="elem"> The ElemTemplateElement that contains the children
		/// that should execute. </param>
		/// <param name="handler"> The ContentHandler to where the result events
		/// should be fed.
		/// </param>
		/// <exception cref="TransformerException">
		/// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeChildTemplates(org.apache.xalan.templates.ElemTemplateElement elem, org.xml.sax.ContentHandler handler) throws javax.xml.transform.TransformerException
		 public virtual void executeChildTemplates(ElemTemplateElement elem, ContentHandler handler)
		 {

		   SerializationHandler xoh = this.SerializationHandler;

		   // These may well not be the same!  In this case when calling
		   // the Redirect extension, it has already set the ContentHandler
		   // in the Transformer.
		   SerializationHandler savedHandler = xoh;

		   try
		   {
			 xoh.flushPending();

			 // %REVIEW% Make sure current node is being pushed.
			 LexicalHandler lex = null;
			 if (handler is LexicalHandler)
			 {
				lex = (LexicalHandler) handler;
			 }
			 m_serializationHandler = new ToXMLSAXHandler(handler, lex, savedHandler.Encoding);
			 m_serializationHandler.Transformer = this;
			 executeChildTemplates(elem, true);
		   }
		   catch (TransformerException e)
		   {
			 throw e;
		   }
		   catch (SAXException se)
		   {
				throw new TransformerException(se);
		   }
		   finally
		   {
			 m_serializationHandler = savedHandler;
		   }
		 }

	  /// <summary>
	  /// Get the keys for the xsl:sort elements.
	  /// Note: Should this go into ElemForEach?
	  /// </summary>
	  /// <param name="foreach"> Valid ElemForEach element, not null. </param>
	  /// <param name="sourceNodeContext"> The current node context in the source tree,
	  /// needed to evaluate the Attribute Value Templates.
	  /// </param>
	  /// <returns> A Vector of NodeSortKeys, or null.
	  /// </returns>
	  /// <exception cref="TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.Vector processSortKeys(org.apache.xalan.templates.ElemForEach foreach, int sourceNodeContext) throws javax.xml.transform.TransformerException
	  public virtual ArrayList processSortKeys(ElemForEach @foreach, int sourceNodeContext)
	  {

		ArrayList keys = null;
		XPathContext xctxt = m_xcontext;
		int nElems = @foreach.SortElemCount;

		if (nElems > 0)
		{
		  keys = new ArrayList();
		}

		// March backwards, collecting the sort keys.
		for (int i = 0; i < nElems; i++)
		{
		  ElemSort sort = @foreach.getSortElem(i);

		  if (m_debug)
		  {
			TraceManager.fireTraceEvent(sort);
		  }

		  string langString = (null != sort.Lang) ? sort.Lang.evaluate(xctxt, sourceNodeContext, @foreach) : null;
		  string dataTypeString = sort.DataType.evaluate(xctxt, sourceNodeContext, @foreach);

		  if (dataTypeString.IndexOf(":", StringComparison.Ordinal) >= 0)
		  {
			Console.WriteLine("TODO: Need to write the hooks for QNAME sort data type");
		  }
		  else if (!(dataTypeString.Equals(Constants.ATTRVAL_DATATYPE_TEXT, StringComparison.CurrentCultureIgnoreCase)) && !(dataTypeString.Equals(Constants.ATTRVAL_DATATYPE_NUMBER, StringComparison.CurrentCultureIgnoreCase)))
		  {
			@foreach.error(XSLTErrorResources.ER_ILLEGAL_ATTRIBUTE_VALUE, new object[]{Constants.ATTRNAME_DATATYPE, dataTypeString});
		  }

		  bool treatAsNumbers = ((null != dataTypeString) && dataTypeString.Equals(Constants.ATTRVAL_DATATYPE_NUMBER)) ? true : false;
		  string orderString = sort.Order.evaluate(xctxt, sourceNodeContext, @foreach);

		  if (!(orderString.Equals(Constants.ATTRVAL_ORDER_ASCENDING, StringComparison.CurrentCultureIgnoreCase)) && !(orderString.Equals(Constants.ATTRVAL_ORDER_DESCENDING, StringComparison.CurrentCultureIgnoreCase)))
		  {
			@foreach.error(XSLTErrorResources.ER_ILLEGAL_ATTRIBUTE_VALUE, new object[]{Constants.ATTRNAME_ORDER, orderString});
		  }

		  bool descending = ((null != orderString) && orderString.Equals(Constants.ATTRVAL_ORDER_DESCENDING)) ? true : false;
		  AVT caseOrder = sort.CaseOrder;
		  bool caseOrderUpper;

		  if (null != caseOrder)
		  {
			string caseOrderString = caseOrder.evaluate(xctxt, sourceNodeContext, @foreach);

			if (!(caseOrderString.Equals(Constants.ATTRVAL_CASEORDER_UPPER, StringComparison.CurrentCultureIgnoreCase)) && !(caseOrderString.Equals(Constants.ATTRVAL_CASEORDER_LOWER, StringComparison.CurrentCultureIgnoreCase)))
			{
			  @foreach.error(XSLTErrorResources.ER_ILLEGAL_ATTRIBUTE_VALUE, new object[]{Constants.ATTRNAME_CASEORDER, caseOrderString});
			}

			caseOrderUpper = ((null != caseOrderString) && caseOrderString.Equals(Constants.ATTRVAL_CASEORDER_UPPER)) ? true : false;
		  }
		  else
		  {
			caseOrderUpper = false;
		  }

		  keys.Add(new NodeSortKey(this, sort.Select, treatAsNumbers, descending, langString, caseOrderUpper, @foreach));
		  if (m_debug)
		  {
			TraceManager.fireTraceEndEvent(sort);
		  }
		}

		return keys;
	  }

	  //==========================================================
	  // SECTION: TransformState implementation
	  //==========================================================

	  /// <summary>
	  /// Get the stack of ElemTemplateElements.
	  /// </summary>
	  /// <returns> A copy of stack that contains the xsl element instructions, 
	  /// the earliest called in index zero, and the latest called in index size()-1. </returns>
	  public virtual ArrayList ElementCallstack
	  {
		  get
		  {
			  ArrayList elems = new ArrayList();
			  int nStackSize = m_currentTemplateElements.size();
			  for (int i = 0; i < nStackSize; i++)
			  {
				  ElemTemplateElement elem = (ElemTemplateElement) m_currentTemplateElements.elementAt(i);
				  if (null != elem)
				  {
					  elems.Add(elem);
				  }
			  }
			  return elems;
		  }
	  }

	  /// <summary>
	  /// Get the count of how many elements are 
	  /// active. </summary>
	  /// <returns> The number of active elements on 
	  /// the currentTemplateElements stack. </returns>
	  public virtual int CurrentTemplateElementsCount
	  {
		  get
		  {
			  return m_currentTemplateElements.size();
		  }
	  }


	  /// <summary>
	  /// Get the count of how many elements are 
	  /// active. </summary>
	  /// <returns> The number of active elements on 
	  /// the currentTemplateElements stack. </returns>
	  public virtual ObjectStack CurrentTemplateElements
	  {
		  get
		  {
			  return m_currentTemplateElements;
		  }
	  }

	  /// <summary>
	  /// Push the current template element.
	  /// </summary>
	  /// <param name="elem"> The current ElemTemplateElement (may be null, and then
	  /// set via setCurrentElement). </param>
	  public virtual void pushElemTemplateElement(ElemTemplateElement elem)
	  {
		m_currentTemplateElements.push(elem);
	  }

	  /// <summary>
	  /// Pop the current template element.
	  /// </summary>
	  public virtual void popElemTemplateElement()
	  {
		m_currentTemplateElements.pop();
	  }

	  /// <summary>
	  /// Set the top of the current template elements
	  /// stack.
	  /// </summary>
	  /// <param name="e"> The current ElemTemplateElement about to
	  /// be executed. </param>
	  public virtual ElemTemplateElement CurrentElement
	  {
		  set
		  {
			m_currentTemplateElements.Top = value;
		  }
		  get
		  {
			return (m_currentTemplateElements.size() > 0) ? (ElemTemplateElement) m_currentTemplateElements.peek() : null;
		  }
	  }


	  /// <summary>
	  /// This method retrieves the current context node
	  /// in the source tree.
	  /// </summary>
	  /// <returns> The current context node (should never be null?). </returns>
	  public virtual int CurrentNode
	  {
		  get
		  {
			return m_xcontext.CurrentNode;
		  }
	  }

	  /// <summary>
	  /// Get the call stack of xsl:template elements.
	  /// </summary>
	  /// <returns> A copy of stack that contains the xsl:template 
	  /// (ElemTemplate) instructions, the earliest called in index 
	  /// zero, and the latest called in index size()-1. </returns>
	  public virtual ArrayList TemplateCallstack
	  {
		  get
		  {
			  ArrayList elems = new ArrayList();
			  int nStackSize = m_currentTemplateElements.size();
			  for (int i = 0; i < nStackSize; i++)
			  {
				  ElemTemplateElement elem = (ElemTemplateElement) m_currentTemplateElements.elementAt(i);
				  if (null != elem && (elem.XSLToken != Constants.ELEMNAME_TEMPLATE))
				  {
					  elems.Add(elem);
				  }
			  }
			  return elems;
		  }
	  }


	  /// <summary>
	  /// This method retrieves the xsl:template
	  /// that is in effect, which may be a matched template
	  /// or a named template.
	  /// 
	  /// <para>Please note that the ElemTemplate returned may
	  /// be a default template, and thus may not have a template
	  /// defined in the stylesheet.</para>
	  /// </summary>
	  /// <returns> The current xsl:template, should not be null. </returns>
	  public virtual ElemTemplate CurrentTemplate
	  {
		  get
		  {
    
			ElemTemplateElement elem = CurrentElement;
    
			while ((null != elem) && (elem.XSLToken != Constants.ELEMNAME_TEMPLATE))
			{
			  elem = elem.ParentElem;
			}
    
			return (ElemTemplate) elem;
		  }
	  }

	  /// <summary>
	  /// Push both the current xsl:template or xsl:for-each onto the
	  /// stack, along with the child node that was matched.
	  /// (Note: should this only be used for xsl:templates?? -sb)
	  /// </summary>
	  /// <param name="template"> xsl:template or xsl:for-each. </param>
	  /// <param name="child"> The child that was matched. </param>
	  public virtual void pushPairCurrentMatched(ElemTemplateElement template, int child)
	  {
		m_currentMatchTemplates.Push(template);
		m_currentMatchedNodes.push(child);
	  }

	  /// <summary>
	  /// Pop the elements that were pushed via pushPairCurrentMatched.
	  /// </summary>
	  public virtual void popCurrentMatched()
	  {
		m_currentMatchTemplates.Pop();
		m_currentMatchedNodes.pop();
	  }

	  /// <summary>
	  /// This method retrieves the xsl:template
	  /// that was matched.  Note that this may not be
	  /// the same thing as the current template (which
	  /// may be from getCurrentElement()), since a named
	  /// template may be in effect.
	  /// </summary>
	  /// <returns> The pushed template that was pushed via pushPairCurrentMatched. </returns>
	  public virtual ElemTemplate MatchedTemplate
	  {
		  get
		  {
			return (ElemTemplate) m_currentMatchTemplates.Peek();
		  }
	  }

	  /// <summary>
	  /// Retrieves the node in the source tree that matched
	  /// the template obtained via getMatchedTemplate().
	  /// </summary>
	  /// <returns> The matched node that corresponds to the
	  /// match attribute of the current xsl:template. </returns>
	  public virtual int MatchedNode
	  {
		  get
		  {
			return m_currentMatchedNodes.peepTail();
		  }
	  }

	  /// <summary>
	  /// Get the current context node list.
	  /// </summary>
	  /// <returns> A reset clone of the context node list. </returns>
	  public virtual DTMIterator ContextNodeList
	  {
		  get
		  {
    
			try
			{
			  DTMIterator cnl = m_xcontext.ContextNodeList;
    
			  return (cnl == null) ? null : (DTMIterator) cnl.cloneWithReset();
			}
			catch (CloneNotSupportedException)
			{
    
			  // should never happen.
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// Get the TrAX Transformer object in effect.
	  /// </summary>
	  /// <returns> This object. </returns>
	  public virtual Transformer Transformer
	  {
		  get
		  {
			return this;
		  }
	  }

	  //==========================================================
	  // SECTION: Accessor Functions
	  //==========================================================

	  /// <summary>
	  /// Set the stylesheet for this processor.  If this is set, then the
	  /// process calls that take only the input .xml will use
	  /// this instead of looking for a stylesheet PI.  Also,
	  /// setting the stylesheet is needed if you are going
	  /// to use the processor as a SAX ContentHandler.
	  /// </summary>
	  /// <param name="stylesheetRoot"> A non-null StylesheetRoot object,
	  /// or null if you wish to clear the stylesheet reference. </param>
	  public virtual StylesheetRoot Stylesheet
	  {
		  set
		  {
			m_stylesheetRoot = value;
		  }
		  get
		  {
			return m_stylesheetRoot;
		  }
	  }


	  /// <summary>
	  /// Get quietConflictWarnings property. If the quietConflictWarnings
	  /// property is set to true, warnings about pattern conflicts won't be
	  /// printed to the diagnostics stream.
	  /// </summary>
	  /// <returns> True if this transformer should not report
	  /// template match conflicts. </returns>
	  public virtual bool QuietConflictWarnings
	  {
		  get
		  {
			return m_quietConflictWarnings;
		  }
		  set
		  {
			m_quietConflictWarnings = value;
		  }
	  }


	  /// <summary>
	  /// Set the execution context for XPath.
	  /// </summary>
	  /// <param name="xcontext"> A non-null reference to the XPathContext
	  /// associated with this transformer.
	  /// @xsl.usage internal </param>
	  public virtual XPathContext XPathContext
	  {
		  set
		  {
			m_xcontext = value;
		  }
		  get
		  {
			return m_xcontext;
		  }
	  }


	  /// <summary>
	  /// Get the object used to guard the stack from
	  /// recursion.
	  /// </summary>
	  /// <returns> The StackGuard object, which should never be null.
	  /// @xsl.usage internal </returns>
	  public virtual StackGuard StackGuard
	  {
		  get
		  {
			return m_stackGuard;
		  }
	  }

	  /// <summary>
	  /// Get the recursion limit.
	  /// Used for infinite loop check. If the value is -1, do not
	  /// check for infinite loops. Anyone who wants to enable that
	  /// check should change the value of this variable to be the
	  /// level of recursion that they want to check. Be careful setting
	  /// this variable, if the number is too low, it may report an
	  /// infinite loop situation, when there is none.
	  /// Post version 1.0.0, we'll make this a runtime feature.
	  /// </summary>
	  /// <returns> The limit on recursion, or -1 if no check is to be made. </returns>
	  public virtual int RecursionLimit
	  {
		  get
		  {
			return m_stackGuard.RecursionLimit;
		  }
		  set
		  {
			m_stackGuard.RecursionLimit = value;
		  }
	  }


	  /// <summary>
	  /// Get the SerializationHandler object.
	  /// </summary>
	  /// <returns> The current SerializationHandler, which may not
	  /// be the main result tree manager. </returns>
	  public virtual SerializationHandler ResultTreeHandler
	  {
		  get
		  {
			return m_serializationHandler;
		  }
	  }

	  /// <summary>
	  /// Get the SerializationHandler object.
	  /// </summary>
	  /// <returns> The current SerializationHandler, which may not
	  /// be the main result tree manager. </returns>
	  public virtual SerializationHandler SerializationHandler
	  {
		  get
		  {
			return m_serializationHandler;
		  }
		  set
		  {
			  m_serializationHandler = value;
		  }
	  }

	  /// <summary>
	  /// Get the KeyManager object.
	  /// </summary>
	  /// <returns> A reference to the KeyManager object, which should
	  /// never be null. </returns>
	  public virtual KeyManager KeyManager
	  {
		  get
		  {
			return m_keyManager;
		  }
	  }

	  /// <summary>
	  /// Check to see if this is a recursive attribute definition.
	  /// </summary>
	  /// <param name="attrSet"> A non-null ElemAttributeSet reference.
	  /// </param>
	  /// <returns> true if the attribute set is recursive. </returns>
	  public virtual bool isRecursiveAttrSet(ElemAttributeSet attrSet)
	  {

		if (null == m_attrSetStack)
		{
		  m_attrSetStack = new Stack();
		}

		if (m_attrSetStack.Count > 0)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Stack equivalent to the Java 'search' method:
		  int loc = m_attrSetStack.search(attrSet);

		  if (loc > -1)
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Push an executing attribute set, so we can check for
	  /// recursive attribute definitions.
	  /// </summary>
	  /// <param name="attrSet"> A non-null ElemAttributeSet reference. </param>
	  public virtual void pushElemAttributeSet(ElemAttributeSet attrSet)
	  {
		m_attrSetStack.Push(attrSet);
	  }

	  /// <summary>
	  /// Pop the current executing attribute set.
	  /// </summary>
	  public virtual void popElemAttributeSet()
	  {
		m_attrSetStack.Pop();
	  }

	  /// <summary>
	  /// Get the table of counters, for optimized xsl:number support.
	  /// </summary>
	  /// <returns> The CountersTable, never null. </returns>
	  public virtual CountersTable CountersTable
	  {
		  get
		  {
    
			if (null == m_countersTable)
			{
			  m_countersTable = new CountersTable();
			}
    
			return m_countersTable;
		  }
	  }

	  /// <summary>
	  /// Tell if the current template rule is null, i.e. if we are
	  /// directly within an apply-templates.  Used for xsl:apply-imports.
	  /// </summary>
	  /// <returns> True if the current template rule is null. </returns>
	  public virtual bool currentTemplateRuleIsNull()
	  {
		return ((!m_currentTemplateRuleIsNull.Empty) && (m_currentTemplateRuleIsNull.peek() == true));
	  }

	  /// <summary>
	  /// Push true if the current template rule is null, false
	  /// otherwise.
	  /// </summary>
	  /// <param name="b"> True if the we are executing an xsl:for-each
	  /// (or xsl:call-template?). </param>
	  public virtual void pushCurrentTemplateRuleIsNull(bool b)
	  {
		m_currentTemplateRuleIsNull.push(b);
	  }

	  /// <summary>
	  /// Push true if the current template rule is null, false
	  /// otherwise.
	  /// </summary>
	  public virtual void popCurrentTemplateRuleIsNull()
	  {
		m_currentTemplateRuleIsNull.pop();
	  }

	  /// <summary>
	  /// Push a funcion result for the currently active EXSLT
	  /// <code>func:function</code>.
	  /// </summary>
	  /// <param name="val"> the result of executing an EXSLT
	  /// <code>func:result</code> instruction for the current
	  /// <code>func:function</code>. </param>
	  public virtual void pushCurrentFuncResult(object val)
	  {
		m_currentFuncResult.push(val);
	  }

	  /// <summary>
	  /// Pops the result of the currently active EXSLT <code>func:function</code>.
	  /// </summary>
	  /// <returns> the value of the <code>func:function</code> </returns>
	  public virtual object popCurrentFuncResult()
	  {
		return m_currentFuncResult.pop();
	  }

	  /// <summary>
	  /// Determines whether an EXSLT <code>func:result</code> instruction has been
	  /// executed for the currently active EXSLT <code>func:function</code>.
	  /// </summary>
	  /// <returns> <code>true</code> if and only if a <code>func:result</code>
	  /// instruction has been executed </returns>
	  public virtual bool currentFuncResultSeen()
	  {
		return !m_currentFuncResult.empty() && m_currentFuncResult.peek() != null;
	  }

	  /// <summary>
	  /// Return the message manager.
	  /// </summary>
	  /// <returns> The message manager, never null. </returns>
	  public virtual MsgMgr MsgMgr
	  {
		  get
		  {
    
			if (null == m_msgMgr)
			{
			  m_msgMgr = new MsgMgr(this);
			}
    
			return m_msgMgr;
		  }
	  }

	  /// <summary>
	  /// Set the error event listener.
	  /// </summary>
	  /// <param name="listener"> The new error listener. </param>
	  /// <exception cref="IllegalArgumentException"> if </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setErrorListener(javax.xml.transform.ErrorListener listener) throws IllegalArgumentException
	  public virtual ErrorListener ErrorListener
	  {
		  set
		  {
    
			lock (m_reentryGuard)
			{
			  if (value == null)
			  {
				throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_NULL_ERROR_HANDLER, null)); //"Null error handler");
			  }
    
			  m_errorHandler = value;
			}
		  }
		  get
		  {
			return m_errorHandler;
		  }
	  }


	  /// <summary>
	  /// Get an instance of the trace manager for this transformation.
	  /// This object can be used to set trace listeners on various
	  /// events during the transformation.
	  /// </summary>
	  /// <returns> A reference to the TraceManager, never null. </returns>
	  public virtual TraceManager TraceManager
	  {
		  get
		  {
			return m_traceManager;
		  }
	  }

	  /// <summary>
	  /// Look up the value of a feature.
	  /// 
	  /// <para>The feature name is any fully-qualified URI.  It is
	  /// possible for an TransformerFactory to recognize a feature name but
	  /// to be unable to return its value; this is especially true
	  /// in the case of an adapter for a SAX1 Parser, which has
	  /// no way of knowing whether the underlying parser is
	  /// validating, for example.</para>
	  /// 
	  /// <h3>Open issues:</h3>
	  /// <dl>
	  ///    <dt><h4>Should getFeature be changed to hasFeature?</h4></dt>
	  ///    <dd>Keith Visco writes: Should getFeature be changed to hasFeature?
	  ///        It returns a boolean which indicated whether the "state"
	  ///        of feature is "true or false". I assume this means whether
	  ///        or not a feature is supported? I know SAX is using "getFeature",
	  ///        but to me "hasFeature" is cleaner.</dd>
	  /// </dl>
	  /// </summary>
	  /// <param name="name"> The feature name, which is a fully-qualified
	  ///        URI. </param>
	  /// <returns> The current state of the feature (true or false). </returns>
	  /// <exception cref="org.xml.sax.SAXNotRecognizedException"> When the
	  ///            TransformerFactory does not recognize the feature name. </exception>
	  /// <exception cref="org.xml.sax.SAXNotSupportedException"> When the
	  ///            TransformerFactory recognizes the feature name but
	  ///            cannot determine its value at this time.
	  /// </exception>
	  /// <exception cref="SAXNotRecognizedException"> </exception>
	  /// <exception cref="SAXNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getFeature(String name) throws org.xml.sax.SAXNotRecognizedException, org.xml.sax.SAXNotSupportedException
	  public virtual bool getFeature(string name)
	  {

		if ("http://xml.org/trax/features/sax/input".Equals(name))
		{
		  return true;
		}
		else if ("http://xml.org/trax/features/dom/input".Equals(name))
		{
		  return true;
		}

		throw new SAXNotRecognizedException(name);
	  }

	  // %TODO% Doc

	  /// <summary>
	  /// NEEDSDOC Method getMode 
	  /// 
	  /// 
	  /// NEEDSDOC (getMode) @return
	  /// </summary>
	  public virtual QName Mode
	  {
		  get
		  {
			return m_modes.Count == 0 ? null : (QName) m_modes.Peek();
		  }
	  }

	  // %TODO% Doc

	  /// <summary>
	  /// NEEDSDOC Method pushMode 
	  /// 
	  /// </summary>
	  /// NEEDSDOC <param name="mode"> </param>
	  public virtual void pushMode(QName mode)
	  {
		m_modes.Push(mode);
	  }

	  // %TODO% Doc

	  /// <summary>
	  /// NEEDSDOC Method popMode 
	  /// 
	  /// </summary>
	  public virtual void popMode()
	  {
		m_modes.Pop();
	  }

	  /// <summary>
	  /// Called by SourceTreeHandler to start the transformation
	  ///  in a separate thread
	  /// </summary>
	  /// NEEDSDOC <param name="priority"> </param>
	  public virtual void runTransformThread(int priority)
	  {

		// used in SourceTreeHandler
		Thread t = ThreadControllerWrapper.runThread(this, priority);
		this.TransformThread = t;
	  }

	  /// <summary>
	  /// Called by this.transform() if isParserEventsOnMain()==false.
	  ///  Similar with runTransformThread(), but no priority is set
	  ///  and setTransformThread is not set.
	  /// </summary>
	  public virtual void runTransformThread()
	  {
		ThreadControllerWrapper.runThread(this, -1);
	  }

	  /// <summary>
	  /// Called by CoRoutineSAXParser. Launches the CoroutineSAXParser
	  /// in a thread, and prepares it to invoke the parser from that thread
	  /// upon request. 
	  /// 
	  /// </summary>
	  public static void runTransformThread(ThreadStart runnable)
	  {
		ThreadControllerWrapper.runThread(runnable, -1);
	  }

	  /// <summary>
	  /// Used by SourceTreeHandler to wait until the transform
	  ///   completes
	  /// </summary>
	  /// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void waitTransformThread() throws org.xml.sax.SAXException
	  public virtual void waitTransformThread()
	  {

		// This is called to make sure the task is done.
		// It is possible that the thread has been reused -
		// but for a different transformation. ( what if we 
		// recycle the transformer ? Not a problem since this is
		// still in use. )
		Thread transformThread = this.TransformThread;

		if (null != transformThread)
		{
		  try
		  {
			ThreadControllerWrapper.waitThread(transformThread, this);

			if (!this.hasTransformThreadErrorCatcher())
			{
			  Exception e = this.ExceptionThrown;

			  if (null != e)
			  {
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new SAXException(e);
			  }
			}

			this.TransformThread = null;
		  }
		  catch (InterruptedException)
		  {
		  }
		}
	  }

	  /// <summary>
	  /// Get the exception thrown by the secondary thread (normally
	  /// the transform thread).
	  /// </summary>
	  /// <returns> The thrown exception, or null if no exception was
	  /// thrown. </returns>
	  public virtual Exception ExceptionThrown
	  {
		  get
		  {
			return m_exceptionThrown;
		  }
		  set
		  {
			m_exceptionThrown = value;
		  }
	  }


	  /// <summary>
	  /// This is just a way to set the document for run().
	  /// </summary>
	  /// <param name="doc"> A non-null reference to the root of the
	  /// tree to be transformed. </param>
	  public virtual int SourceTreeDocForThread
	  {
		  set
		  {
			m_doc = value;
		  }
	  }

	  /// <summary>
	  /// Set the input source for the source tree, which is needed if the
	  /// parse thread is not the main thread, in order for the parse
	  /// thread's run method to get to the input source.
	  /// </summary>
	  /// <param name="source"> The input source for the source tree. </param>
	  public virtual Source XMLSource
	  {
		  set
		  {
			m_xmlSource = value;
		  }
	  }

	  /// <summary>
	  /// Tell if the transform method is completed.
	  /// </summary>
	  /// <returns> True if transformNode has completed, or
	  /// an exception was thrown. </returns>
	  public virtual bool TransformDone
	  {
		  get
		  {
    
			lock (this)
			{
			  return m_isTransformDone;
			}
		  }
	  }

	  /// <summary>
	  /// Set if the transform method is completed.
	  /// </summary>
	  /// <param name="done"> True if transformNode has completed, or
	  /// an exception was thrown. </param>
	  public virtual bool IsTransformDone
	  {
		  set
		  {
    
			lock (this)
			{
			  m_isTransformDone = value;
			}
		  }
	  }

	  /// <summary>
	  /// From a secondary thread, post the exception, so that
	  /// it can be picked up from the main thread.
	  /// </summary>
	  /// <param name="e"> The exception that was thrown. </param>
	  internal virtual void postExceptionFromThread(Exception e)
	  {

		// Commented out in response to problem reported by Nicola Brown <Nicola.Brown@jacobsrimell.com>
		//    if(m_reportInPostExceptionFromThread)
		//    {
		//      // Consider re-throwing the exception if this flag is set.
		//      e.printStackTrace();
		//    }
		// %REVIEW Need DTM equivelent?    
		//    if (m_inputContentHandler instanceof SourceTreeHandler)
		//    {
		//      SourceTreeHandler sth = (SourceTreeHandler) m_inputContentHandler;
		//
		//      sth.setExceptionThrown(e);
		//    }
	 //   ContentHandler ch = getContentHandler();

		//    if(ch instanceof SourceTreeHandler)
		//    {
		//      SourceTreeHandler sth = (SourceTreeHandler) ch;
		//      ((TransformerImpl)(sth.getTransformer())).postExceptionFromThread(e);
		//    }
		m_isTransformDone = true;
		m_exceptionThrown = e;
		; // should have already been reported via the error handler?

		lock (this)
		{

		  // See message from me on 3/27/2001 to Patrick Moore.
		  //      String msg = e.getMessage();
		  // System.out.println(e.getMessage());
		  // Is this really needed?  -sb
		  Monitor.PulseAll(this);

		  //      if (null == msg)
		  //      {
		  //
		  //        // m_throwNewError = false;
		  //        e.printStackTrace();
		  //      }
		  // throw new org.apache.xml.utils.WrappedRuntimeException(e);
		}
	  }

	  /// <summary>
	  /// Run the transform thread.
	  /// </summary>
	  public virtual void run()
	  {

		m_hasBeenReset = false;

		try
		{

		  // int n = ((SourceTreeHandler)getInputContentHandler()).getDTMRoot();
		  // transformNode(n);
		  try
		  {
			m_isTransformDone = false;

			// Should no longer be needed...
	//          if(m_inputContentHandler instanceof TransformerHandlerImpl)
	//          {
	//            TransformerHandlerImpl thi = (TransformerHandlerImpl)m_inputContentHandler;
	//            thi.waitForInitialEvents();
	//          }

			transformNode(m_doc);

		  }
		  catch (Exception e)
		  {
			// e.printStackTrace();

			// Strange that the other catch won't catch this...
			if (null != m_transformThread)
			{
			  postExceptionFromThread(e); // Assume we're on the main thread
			}
			else
			{
			  throw new Exception(e.Message);
			}
		  }
		  finally
		  {
			m_isTransformDone = true;

			if (m_inputContentHandler is TransformerHandlerImpl)
			{
			  ((TransformerHandlerImpl) m_inputContentHandler).clearCoRoutine();
			}

			//        synchronized (this)
			//        {
			//          notifyAll();
			//        }
		  }
		}
		catch (Exception e)
		{

		  // e.printStackTrace();
		  if (null != m_transformThread)
		  {
			postExceptionFromThread(e);
		  }
		  else
		  {
			throw new Exception(e.Message); // Assume we're on the main thread.
		  }
		}
	  }

	  // Fragment re-execution interfaces for a tool.

	  /// <summary>
	  /// This will get a snapshot of the current executing context 
	  /// 
	  /// </summary>
	  /// <returns> TransformSnapshot object, snapshot of executing context </returns>
	  /// @deprecated This is an internal tooling API that nobody seems to be using 
	  public virtual TransformSnapshot Snapshot
	  {
		  get
		  {
			return new TransformSnapshotImpl(this);
		  }
	  }

	  /// <summary>
	  /// This will execute the following XSLT instructions
	  /// from the snapshot point, after the stylesheet execution
	  /// context has been reset from the snapshot point. 
	  /// </summary>
	  /// <param name="ts"> The snapshot of where to start execution
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
	  /// @deprecated This is an internal tooling API that nobody seems to be using 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void executeFromSnapshot(TransformSnapshot ts) throws javax.xml.transform.TransformerException
	  public virtual void executeFromSnapshot(TransformSnapshot ts)
	  {

		ElemTemplateElement template = MatchedTemplate;
		int child = MatchedNode;

		pushElemTemplateElement(template); //needed??
		m_xcontext.pushCurrentNode(child); //needed??
		this.executeChildTemplates(template, true); // getResultTreeHandler());
	  }

	  /// <summary>
	  /// This will reset the stylesheet execution context
	  /// from the snapshot point.
	  /// </summary>
	  /// <param name="ts"> The snapshot of where to start execution </param>
	  /// @deprecated This is an internal tooling API that nobody seems to be using 
	  public virtual void resetToStylesheet(TransformSnapshot ts)
	  {
		((TransformSnapshotImpl) ts).apply(this);
	  }

	  /// <summary>
	  /// NEEDSDOC Method stopTransformation 
	  /// 
	  /// </summary>
	  public virtual void stopTransformation()
	  {
	  }

	  /// <summary>
	  /// Test whether whitespace-only text nodes are visible in the logical
	  /// view of <code>DTM</code>. Normally, this function
	  /// will be called by the implementation of <code>DTM</code>;
	  /// it is not normally called directly from
	  /// user code.
	  /// </summary>
	  /// <param name="elementHandle"> int Handle of the element. </param>
	  /// <returns> one of NOTSTRIP, STRIP, or INHERIT. </returns>
	  public virtual short getShouldStripSpace(int elementHandle, DTM dtm)
	  {

		try
		{
		  org.apache.xalan.templates.WhiteSpaceInfo info = m_stylesheetRoot.getWhiteSpaceInfo(m_xcontext, elementHandle, dtm);

		  if (null == info)
		  {
			return org.apache.xml.dtm.DTMWSFilter_Fields.INHERIT;
		  }
		  else
		  {

			// System.out.println("getShouldStripSpace: "+info.getShouldStripSpace());
			return info.ShouldStripSpace ? org.apache.xml.dtm.DTMWSFilter_Fields.STRIP : org.apache.xml.dtm.DTMWSFilter_Fields.NOTSTRIP;
		  }
		}
		catch (TransformerException)
		{
		  return org.apache.xml.dtm.DTMWSFilter_Fields.INHERIT;
		}
	  }
	  /// <summary>
	  /// Initializer method.
	  /// </summary>
	  /// <param name="transformer"> non-null transformer instance </param>
	  /// <param name="realHandler"> Content Handler instance </param>
	   public virtual void init(ToXMLSAXHandler h, Transformer transformer, ContentHandler realHandler)
	   {
		  h.Transformer = transformer;
		  h.ContentHandler = realHandler;
	   }




		/// <summary>
		/// Fire off characters, cdate events. </summary>
		/// <seealso cref= org.apache.xml.serializer.SerializerTrace#fireGenerateEvent(int, char[], int, int) </seealso>
		public virtual void fireGenerateEvent(int eventType, char[] ch, int start, int length)
		{

			GenerateEvent ge = new GenerateEvent(this, eventType, ch, start, length);
			m_traceManager.fireGenerateEvent(ge);
		}

		/// <summary>
		/// Fire off startElement, endElement events. </summary>
		/// <seealso cref= org.apache.xml.serializer.SerializerTrace#fireGenerateEvent(int, String, Attributes) </seealso>
		public virtual void fireGenerateEvent(int eventType, string name, Attributes atts)
		{

			GenerateEvent ge = new GenerateEvent(this, eventType, name, atts);
			m_traceManager.fireGenerateEvent(ge);
		}

		/// <summary>
		/// Fire off processingInstruction events. </summary>
		/// <seealso cref= org.apache.xml.serializer.SerializerTrace#fireGenerateEvent(int, String, String) </seealso>
		public virtual void fireGenerateEvent(int eventType, string name, string data)
		{
			GenerateEvent ge = new GenerateEvent(this, eventType, name,data);
			m_traceManager.fireGenerateEvent(ge);
		}

		/// <summary>
		/// Fire off comment and entity ref events. </summary>
		/// <seealso cref= org.apache.xml.serializer.SerializerTrace#fireGenerateEvent(int, String) </seealso>
		public virtual void fireGenerateEvent(int eventType, string data)
		{
			GenerateEvent ge = new GenerateEvent(this, eventType, data);
			m_traceManager.fireGenerateEvent(ge);
		}

		/// <summary>
		/// Fire off startDocument, endDocument events. </summary>
		/// <seealso cref= org.apache.xml.serializer.SerializerTrace#fireGenerateEvent(int) </seealso>
		public virtual void fireGenerateEvent(int eventType)
		{
			GenerateEvent ge = new GenerateEvent(this, eventType);
			m_traceManager.fireGenerateEvent(ge);
		}

		/// <seealso cref= org.apache.xml.serializer.SerializerTrace#hasTraceListeners() </seealso>
		public virtual bool hasTraceListeners()
		{
			return m_traceManager.hasTraceListeners();
		}

		public virtual bool Debug
		{
			get
			{
				return m_debug;
			}
			set
			{
				m_debug = value;
			}
		}


		/// <returns> Incremental flag </returns>
		public virtual bool Incremental
		{
			get
			{
				return m_incremental;
			}
		}

		/// <returns> Optimization flag </returns>
		public virtual bool Optimize
		{
			get
			{
				return m_optimizer;
			}
		}

		/// <returns> Source location flag </returns>
		public virtual bool Source_location
		{
			get
			{
				return m_source_location;
			}
		}

	} // end TransformerImpl class


}