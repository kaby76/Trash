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
 * $Id: StylesheetHandler.java 1225754 2011-12-30 05:31:15Z mrglavas $
 */
namespace org.apache.xalan.processor
{


	using ExpressionVisitor = org.apache.xalan.extensions.ExpressionVisitor;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using Constants = org.apache.xalan.templates.Constants;
	using ElemForEach = org.apache.xalan.templates.ElemForEach;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using StylesheetRoot = org.apache.xalan.templates.StylesheetRoot;
	using BoolStack = org.apache.xml.utils.BoolStack;
	using NamespaceSupport2 = org.apache.xml.utils.NamespaceSupport2;
	using NodeConsumer = org.apache.xml.utils.NodeConsumer;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using XMLCharacterRecognizer = org.apache.xml.utils.XMLCharacterRecognizer;
	using XPath = org.apache.xpath.XPath;
	using FunctionTable = org.apache.xpath.compiler.FunctionTable;
	using Node = org.w3c.dom.Node;
	using Attributes = org.xml.sax.Attributes;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;
	using NamespaceSupport = org.xml.sax.helpers.NamespaceSupport;

	/// <summary>
	/// Initializes and processes a stylesheet via SAX events.
	/// This class acts as essentially a state machine, maintaining
	/// a ContentHandler stack, and pushing appropriate content
	/// handlers as parse events occur.
	/// @xsl.usage advanced
	/// </summary>
	public class StylesheetHandler : DefaultHandler, TemplatesHandler, PrefixResolver, NodeConsumer
	{


	  /// <summary>
	  /// The function table of XPath and XSLT;
	  /// </summary>
	  private FunctionTable m_funcTable = new FunctionTable();

	  /// <summary>
	  /// The flag for the setting of the optimize feature;
	  /// </summary>
	  private bool m_optimize = true;

	  /// <summary>
	  /// The flag for the setting of the incremental feature;
	  /// </summary>
	  private bool m_incremental = false;

	  /// <summary>
	  /// The flag for the setting of the source_location feature;
	  /// </summary>
	  private bool m_source_location = false;

	  /// <summary>
	  /// Create a StylesheetHandler object, creating a root stylesheet
	  /// as the target.
	  /// </summary>
	  /// <param name="processor"> non-null reference to the transformer factory that owns this handler.
	  /// </param>
	  /// <exception cref="TransformerConfigurationException"> if a StylesheetRoot
	  /// can not be constructed for some reason. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StylesheetHandler(TransformerFactoryImpl processor) throws javax.xml.transform.TransformerConfigurationException
	  public StylesheetHandler(TransformerFactoryImpl processor)
	  {
		Type func = typeof(org.apache.xalan.templates.FuncDocument);
		m_funcTable.installFunction("document", func);

		// func = new org.apache.xalan.templates.FuncKey();
		// FunctionTable.installFunction("key", func);
		func = typeof(org.apache.xalan.templates.FuncFormatNumb);

		m_funcTable.installFunction("format-number", func);

		m_optimize = ((bool?) processor.getAttribute(TransformerFactoryImpl.FEATURE_OPTIMIZE)).Value;
		m_incremental = ((bool?) processor.getAttribute(TransformerFactoryImpl.FEATURE_INCREMENTAL)).Value;
		m_source_location = ((bool?) processor.getAttribute(TransformerFactoryImpl.FEATURE_SOURCE_LOCATION)).Value;
		// m_schema = new XSLTSchema();
		init(processor);

	  }

	  /// <summary>
	  /// Do common initialization.
	  /// </summary>
	  /// <param name="processor"> non-null reference to the transformer factory that owns this handler. </param>
	  internal virtual void init(TransformerFactoryImpl processor)
	  {
		m_stylesheetProcessor = processor;

		// Set the initial content handler.
		m_processors.Push(m_schema.ElementProcessor);
		this.pushNewNamespaceSupport();

		// m_includeStack.push(SystemIDResolver.getAbsoluteURI(this.getBaseIdentifier(), null));
		// initXPath(processor, null);
	  }

	  /// <summary>
	  /// Process an expression string into an XPath.
	  /// Must be public for access by the AVT class.
	  /// </summary>
	  /// <param name="str"> A non-null reference to a valid or invalid XPath expression string.
	  /// </param>
	  /// <returns> A non-null reference to an XPath object that represents the string argument.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if the expression can not be processed. </exception>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Expressions">Section 4 Expressions in XSLT Specification</a> </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.XPath createXPath(String str, org.apache.xalan.templates.ElemTemplateElement owningTemplate) throws javax.xml.transform.TransformerException
	  public virtual XPath createXPath(string str, ElemTemplateElement owningTemplate)
	  {
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;
		XPath xpath = new XPath(str, owningTemplate, this, XPath.SELECT, handler, m_funcTable);
		// Visit the expression, registering namespaces for any extension functions it includes.
		xpath.callVisitors(xpath, new ExpressionVisitor(StylesheetRoot));
		return xpath;
	  }

	  /// <summary>
	  /// Process an expression string into an XPath.
	  /// </summary>
	  /// <param name="str"> A non-null reference to a valid or invalid match pattern string.
	  /// </param>
	  /// <returns> A non-null reference to an XPath object that represents the string argument.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> if the pattern can not be processed. </exception>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#patterns">Section 5.2 Patterns in XSLT Specification</a> </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: org.apache.xpath.XPath createMatchPatternXPath(String str, org.apache.xalan.templates.ElemTemplateElement owningTemplate) throws javax.xml.transform.TransformerException
	  internal virtual XPath createMatchPatternXPath(string str, ElemTemplateElement owningTemplate)
	  {
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;
		XPath xpath = new XPath(str, owningTemplate, this, XPath.MATCH, handler, m_funcTable);
		// Visit the expression, registering namespaces for any extension functions it includes.
		xpath.callVisitors(xpath, new ExpressionVisitor(StylesheetRoot));
		return xpath;
	  }

	  /// <summary>
	  /// Given a namespace, get the corrisponding prefix from the current
	  /// namespace support context.
	  /// </summary>
	  /// <param name="prefix"> The prefix to look up, which may be an empty string ("") for the default Namespace.
	  /// </param>
	  /// <returns> The associated Namespace URI, or null if the prefix
	  ///         is undeclared in this context. </returns>
	  public virtual string getNamespaceForPrefix(string prefix)
	  {
		return this.NamespaceSupport.getURI(prefix);
	  }

	  /// <summary>
	  /// Given a namespace, get the corrisponding prefix.  This is here only
	  /// to support the <seealso cref="org.apache.xml.utils.PrefixResolver"/> interface,
	  /// and will throw an error if invoked on this object.
	  /// </summary>
	  /// <param name="prefix"> The prefix to look up, which may be an empty string ("") for the default Namespace. </param>
	  /// <param name="context"> The node context from which to look up the URI.
	  /// </param>
	  /// <returns> The associated Namespace URI, or null if the prefix
	  ///         is undeclared in this context. </returns>
	  public virtual string getNamespaceForPrefix(string prefix, Node context)
	  {

		// Don't need to support this here.  Return the current URI for the prefix,
		// ignoring the context.
		assertion(true, "can't process a context node in StylesheetHandler!");

		return null;
	  }

	  /// <summary>
	  /// Utility function to see if the stack contains the given URL.
	  /// </summary>
	  /// <param name="stack"> non-null reference to a Stack. </param>
	  /// <param name="url"> URL string on which an equality test will be performed.
	  /// </param>
	  /// <returns> true if the stack contains the url argument. </returns>
	  private bool stackContains(Stack stack, string url)
	  {

		int n = stack.Count;
		bool contains = false;

		for (int i = 0; i < n; i++)
		{
		  string url2 = (string) stack.elementAt(i);

		  if (url2.Equals(url))
		  {
			contains = true;

			break;
		  }
		}

		return contains;
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of the TRAX TemplatesBuilder interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// When this object is used as a ContentHandler or ContentHandler, it will
	  /// create a Templates object, which the caller can get once
	  /// the SAX events have been completed. </summary>
	  /// <returns> The stylesheet object that was created during
	  /// the SAX event process, or null if no stylesheet has
	  /// been created.
	  /// 
	  /// Author <a href="mailto:scott_boag@lotus.com">Scott Boag</a>
	  /// 
	  ///  </returns>
	  public virtual Templates Templates
	  {
		  get
		  {
			return StylesheetRoot;
		  }
	  }

	  /// <summary>
	  /// Set the base ID (URL or system ID) for the stylesheet
	  /// created by this builder.  This must be set in order to
	  /// resolve relative URLs in the stylesheet.
	  /// </summary>
	  /// <param name="baseID"> Base URL for this stylesheet. </param>
	  public virtual string SystemId
	  {
		  set
		  {
			pushBaseIndentifier(value);
		  }
		  get
		  {
			return this.BaseIdentifier;
		  }
	  }


	  ////////////////////////////////////////////////////////////////////
	  // Implementation of the EntityResolver interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Resolve an external entity.
	  /// </summary>
	  /// <param name="publicId"> The public identifer, or null if none is
	  ///                 available. </param>
	  /// <param name="systemId"> The system identifier provided in the XML
	  ///                 document. </param>
	  /// <returns> The new input source, or null to require the
	  ///         default behaviour.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the entity can not be resolved. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.InputSource resolveEntity(String publicId, String systemId) throws org.xml.sax.SAXException
	  public virtual InputSource resolveEntity(string publicId, string systemId)
	  {
		return CurrentProcessor.resolveEntity(this, publicId, systemId);
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of DTDHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive notification of a notation declaration.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass if they wish to keep track of the notations
	  /// declared in a document.</para>
	  /// </summary>
	  /// <param name="name"> The notation name. </param>
	  /// <param name="publicId"> The notation public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The notation system identifier. </param>
	  /// <seealso cref= org.xml.sax.DTDHandler#notationDecl </seealso>
	  public virtual void notationDecl(string name, string publicId, string systemId)
	  {
		CurrentProcessor.notationDecl(this, name, publicId, systemId);
	  }

	  /// <summary>
	  /// Receive notification of an unparsed entity declaration.
	  /// </summary>
	  /// <param name="name"> The entity name. </param>
	  /// <param name="publicId"> The entity public identifier, or null if not
	  ///                 available. </param>
	  /// <param name="systemId"> The entity system identifier. </param>
	  /// <param name="notationName"> The name of the associated notation. </param>
	  /// <seealso cref= org.xml.sax.DTDHandler#unparsedEntityDecl </seealso>
	  public virtual void unparsedEntityDecl(string name, string publicId, string systemId, string notationName)
	  {
		CurrentProcessor.unparsedEntityDecl(this, name, publicId, systemId, notationName);
	  }

	  /// <summary>
	  /// Given a namespace URI, and a local name or a node type, get the processor
	  /// for the element, or return null if not allowed.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix).
	  /// </param>
	  /// <returns> A non-null reference to a element processor.
	  /// </returns>
	  /// <exception cref="org.xml.sax.SAXException"> if the element is not allowed in the
	  /// found position in the stylesheet. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: XSLTElementProcessor getProcessorFor(String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  internal virtual XSLTElementProcessor getProcessorFor(string uri, string localName, string rawName)
	  {

		XSLTElementProcessor currentProcessor = CurrentProcessor;
		XSLTElementDef def = currentProcessor.ElemDef;
		XSLTElementProcessor elemProcessor = def.getProcessorFor(uri, localName);

		if (null == elemProcessor && !(currentProcessor is ProcessorStylesheetDoc) && ((null == Stylesheet || Convert.ToDouble(Stylesheet.Version) > Constants.XSLTVERSUPPORTED) || (!uri.Equals(Constants.S_XSLNAMESPACEURL) && currentProcessor is ProcessorStylesheetElement) || ElemVersion > Constants.XSLTVERSUPPORTED))
		{
		  elemProcessor = def.getProcessorForUnknown(uri, localName);
		}

		if (null == elemProcessor)
		{
		  error(XSLMessages.createMessage(XSLTErrorResources.ER_NOT_ALLOWED_IN_POSITION, new object[]{rawName}),null); //rawName + " is not allowed in this position in the stylesheet!",
		}


		return elemProcessor;
	  }

	  ////////////////////////////////////////////////////////////////////
	  // Implementation of ContentHandler interface.
	  ////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Receive a Locator object for document events.
	  /// This is called by the parser to push a locator for the
	  /// stylesheet being parsed. The stack needs to be popped
	  /// after the stylesheet has been parsed. We pop in
	  /// popStylesheet.
	  /// </summary>
	  /// <param name="locator"> A locator for all SAX document events. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#setDocumentLocator </seealso>
	  /// <seealso cref= org.xml.sax.Locator </seealso>
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
    
			// System.out.println("pushing locator for: "+value.getSystemId());
			m_stylesheetLocatorStack.Push(new SAXSourceLocator(value));
		  }
	  }

	  /// <summary>
	  /// The level of the stylesheet we are at.
	  /// </summary>
	  private int m_stylesheetLevel = -1;

	  /// <summary>
	  /// Receive notification of the beginning of the document.
	  /// </summary>
	  /// <seealso cref= org.xml.sax.ContentHandler#startDocument
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
	  public virtual void startDocument()
	  {
		m_stylesheetLevel++;
		pushSpaceHandling(false);
	  }

	  /// <summary>
	  /// m_parsingComplete becomes true when the top-level stylesheet and all
	  /// its included/imported stylesheets have been been fully parsed, as an
	  /// indication that composition/optimization/compilation can begin. </summary>
	  /// <seealso cref= isStylesheetParsingComplete   </seealso>
	  private bool m_parsingComplete = false;

	  /// <summary>
	  /// Test whether the _last_ endDocument() has been processed.
	  /// This is needed as guidance for stylesheet optimization
	  /// and compilation engines, which generally don't want to start
	  /// until all included and imported stylesheets have been fully
	  /// parsed.
	  /// </summary>
	  /// <returns> true iff the complete stylesheet tree has been built. </returns>
	  public virtual bool StylesheetParsingComplete
	  {
		  get
		  {
			return m_parsingComplete;
		  }
	  }

	  /// <summary>
	  /// Receive notification of the end of the document.
	  /// </summary>
	  /// <seealso cref= org.xml.sax.ContentHandler#endDocument
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
	  public virtual void endDocument()
	  {

		try
		{
		  if (null != StylesheetRoot)
		  {
			if (0 == m_stylesheetLevel)
			{
			  StylesheetRoot.recompose();
			}
		  }
		  else
		  {
			throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_STYLESHEETROOT, null)); //"Did not find the stylesheet root!");
		  }

		  XSLTElementProcessor elemProcessor = CurrentProcessor;

		  if (null != elemProcessor)
		  {
			elemProcessor.startNonText(this);
		  }

		  m_stylesheetLevel--;

		  popSpaceHandling();

		  // WARNING: This test works only as long as stylesheets are parsed
		  // more or less recursively. If we switch to an iterative "work-list"
		  // model, this will become true prematurely. In that case,
		  // isStylesheetParsingComplete() will have to be adjusted to be aware
		  // of the worklist.
		  m_parsingComplete = (m_stylesheetLevel < 0);
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  private ArrayList m_prefixMappings = new ArrayList();

	  /// <summary>
	  /// Receive notification of the start of a Namespace mapping.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each element (such as allocating a new tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <param name="uri"> The Namespace URI mapped to the prefix. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#startPrefixMapping
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(String prefix, String uri) throws org.xml.sax.SAXException
	  public virtual void startPrefixMapping(string prefix, string uri)
	  {

		// m_nsSupport.pushContext();
		// this.getNamespaceSupport().declarePrefix(prefix, uri);
		//m_prefixMappings.add(prefix); // JDK 1.2+ only -sc
		//m_prefixMappings.add(uri); // JDK 1.2+ only -sc
		m_prefixMappings.Add(prefix); // JDK 1.1.x compat -sc
		m_prefixMappings.Add(uri); // JDK 1.1.x compat -sc
	  }

	  /// <summary>
	  /// Receive notification of the end of a Namespace mapping.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions at the start of
	  /// each element (such as allocating a new tree node or writing
	  /// output to a file).</para>
	  /// </summary>
	  /// <param name="prefix"> The Namespace prefix being declared. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#endPrefixMapping
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(String prefix) throws org.xml.sax.SAXException
	  public virtual void endPrefixMapping(string prefix)
	  {

		// m_nsSupport.popContext();
	  }

	  /// <summary>
	  /// Flush the characters buffer.
	  /// </summary>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void flushCharacters() throws org.xml.sax.SAXException
	  private void flushCharacters()
	  {

		XSLTElementProcessor elemProcessor = CurrentProcessor;

		if (null != elemProcessor)
		{
		  elemProcessor.startNonText(this);
		}
	  }

	  /// <summary>
	  /// Receive notification of the start of an element.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <param name="attributes"> The specified or defaulted attributes.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public virtual void startElement(string uri, string localName, string rawName, Attributes attributes)
	  {
		NamespaceSupport nssupport = this.NamespaceSupport;
		nssupport.pushContext();

		int n = m_prefixMappings.Count;

		for (int i = 0; i < n; i++)
		{
		  string prefix = (string)m_prefixMappings[i++];
		  string nsURI = (string)m_prefixMappings[i];
		  nssupport.declarePrefix(prefix, nsURI);
		}
		//m_prefixMappings.clear(); // JDK 1.2+ only -sc
		m_prefixMappings.Clear(); // JDK 1.1.x compat -sc

		m_elementID++;

		// This check is currently done for all elements.  We should possibly consider
		// limiting this check to xsl:stylesheet elements only since that is all it really
		// applies to.  Also, it could be bypassed if m_shouldProcess is already true.
		// In other words, the next two statements could instead look something like this:
		// if (!m_shouldProcess)
		// {
		//   if (localName.equals(Constants.ELEMNAME_STYLESHEET_STRING) &&
		//       url.equals(Constants.S_XSLNAMESPACEURL))
		//   {
		//     checkForFragmentID(attributes);
		//     if (!m_shouldProcess)
		//       return;
		//   }
		//   else
		//     return;
		// } 
		// I didn't include this code statement at this time because in practice 
		// it is a small performance hit and I was waiting to see if its absence
		// caused a problem. - GLP

		checkForFragmentID(attributes);

		if (!m_shouldProcess)
		{
		  return;
		}

		flushCharacters();

		pushSpaceHandling(attributes);

		XSLTElementProcessor elemProcessor = getProcessorFor(uri, localName, rawName);

		if (null != elemProcessor) // defensive, for better multiple error reporting. -sb
		{
		  this.pushProcessor(elemProcessor);
		  elemProcessor.startElement(this, uri, localName, rawName, attributes);
		}
		else
		{
		  m_shouldProcess = false;
		  popSpaceHandling();
		}

	  }

	  /// <summary>
	  /// Receive notification of the end of an element.
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="rawName"> The qualified name (with prefix). </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#endElement
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  public virtual void endElement(string uri, string localName, string rawName)
	  {

		m_elementID--;

		if (!m_shouldProcess)
		{
		  return;
		}

		if ((m_elementID + 1) == m_fragmentID)
		{
		  m_shouldProcess = false;
		}

		flushCharacters();

		popSpaceHandling();

		XSLTElementProcessor p = CurrentProcessor;

		p.endElement(this, uri, localName, rawName);
		this.popProcessor();
		this.NamespaceSupport.popContext();
	  }

	  /// <summary>
	  /// Receive notification of character data inside an element.
	  /// </summary>
	  /// <param name="ch"> The characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#characters
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {

		if (!m_shouldProcess)
		{
		  return;
		}

		XSLTElementProcessor elemProcessor = CurrentProcessor;
		XSLTElementDef def = elemProcessor.ElemDef;

		if (def.Type != XSLTElementDef.T_PCDATA)
		{
		  elemProcessor = def.getProcessorFor(null, "text()");
		}

		if (null == elemProcessor)
		{

		  // If it's whitespace, just ignore it, otherwise flag an error.
		  if (!XMLCharacterRecognizer.isWhiteSpace(ch, start, length))
		  {
			error(XSLMessages.createMessage(XSLTErrorResources.ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, null),null); //"Non-whitespace text is not allowed in this position in the stylesheet!",
		  }

		}
		else
		{
		  elemProcessor.characters(this, ch, start, length);
		}
	  }

	  /// <summary>
	  /// Receive notification of ignorable whitespace in element content.
	  /// </summary>
	  /// <param name="ch"> The whitespace characters. </param>
	  /// <param name="start"> The start position in the character array. </param>
	  /// <param name="length"> The number of characters to use from the
	  ///               character array. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#ignorableWhitespace
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char ch[], int start, int length) throws org.xml.sax.SAXException
	  public virtual void ignorableWhitespace(char[] ch, int start, int length)
	  {

		if (!m_shouldProcess)
		{
		  return;
		}

		CurrentProcessor.ignorableWhitespace(this, ch, start, length);
	  }

	  /// <summary>
	  /// Receive notification of a processing instruction.
	  /// 
	  /// <para>The Parser will invoke this method once for each processing
	  /// instruction found: note that processing instructions may occur
	  /// before or after the main document element.</para>
	  /// 
	  /// <para>A SAX parser should never report an XML declaration (XML 1.0,
	  /// section 2.8) or a text declaration (XML 1.0, section 4.3.1)
	  /// using this method.</para>
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions for each
	  /// processing instruction, such as setting status variables or
	  /// invoking other methods.</para>
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///             none is supplied. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#processingInstruction
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {
		if (!m_shouldProcess)
		{
		  return;
		}

		// Recreating Scott's kluge:
		// A xsl:for-each or xsl:apply-templates may have a special 
		// PI that tells us not to cache the document.  This PI 
		// should really be namespaced.
		//    String localName = getLocalName(target);
		//    String ns = m_stylesheet.getNamespaceFromStack(target);
		//
		// %REVIEW%: We need a better PI architecture

		string prefix = "", ns = "", localName = target;
		int colon = target.IndexOf(':');
		if (colon >= 0)
		{
		  ns = getNamespaceForPrefix(prefix = target.Substring(0,colon));
		  localName = target.Substring(colon + 1);
		}

		try
		{
		  // A xsl:for-each or xsl:apply-templates may have a special 
		  // PI that tells us not to cache the document.  This PI 
		  // should really be namespaced... but since the XML Namespaces
		  // spec never defined namespaces as applying to PI's, and since
		  // the testcase we're trying to support is inconsistant in whether
		  // it binds the prefix, I'm going to make this sloppy for
		  // testing purposes.
		  if ("xalan-doc-cache-off".Equals(target) || "xalan:doc-cache-off".Equals(target) || ("doc-cache-off".Equals(localName) && ns.Equals("org.apache.xalan.xslt.extensions.Redirect")))
		  {
		if (!(m_elems.Peek() is ElemForEach))
		{
			  throw new TransformerException("xalan:doc-cache-off not allowed here!", Locator);
		}
			ElemForEach elem = (ElemForEach)m_elems.Peek();

			elem.m_doc_cache_off = true;

		//System.out.println("JJK***** Recognized <? {"+ns+"}"+prefix+":"+localName+" "+data+"?>");
		  }
		}
		catch (Exception)
		{
		  // JJK: Officially, unknown PIs can just be ignored.
		  // Do we want to issue a warning?
		}


		flushCharacters();
		CurrentProcessor.processingInstruction(this, target, data);
	  }

	  /// <summary>
	  /// Receive notification of a skipped entity.
	  /// 
	  /// <para>By default, do nothing.  Application writers may override this
	  /// method in a subclass to take specific actions for each
	  /// processing instruction, such as setting status variables or
	  /// invoking other methods.</para>
	  /// </summary>
	  /// <param name="name"> The name of the skipped entity. </param>
	  /// <seealso cref= org.xml.sax.ContentHandler#processingInstruction
	  /// </seealso>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(String name) throws org.xml.sax.SAXException
	  public virtual void skippedEntity(string name)
	  {

		if (!m_shouldProcess)
		{
		  return;
		}

		CurrentProcessor.skippedEntity(this, name);
	  }

	  /// <summary>
	  /// Warn the user of an problem.
	  /// </summary>
	  /// <param name="msg"> An key into the <seealso cref="org.apache.xalan.res.XSLTErrorResources"/>
	  /// table, that is one of the WG_ prefixed definitions. </param>
	  /// <param name="args"> An array of arguments for the given warning.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the current
	  /// <seealso cref="javax.xml.transform.ErrorListener#warning"/>
	  /// method chooses to flag this condition as an error.
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warn(String msg, Object args[]) throws org.xml.sax.SAXException
	  public virtual void warn(string msg, object[] args)
	  {

		string formattedMsg = XSLMessages.createWarning(msg, args);
		SAXSourceLocator locator = Locator;
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;

		try
		{
		  if (null != handler)
		  {
			handler.warning(new TransformerException(formattedMsg, locator));
		  }
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Assert that a condition is true.  If it is not true, throw an error.
	  /// </summary>
	  /// <param name="condition"> false if an error should not be thrown, otherwise true. </param>
	  /// <param name="msg"> Error message to be passed to the RuntimeException as an
	  /// argument. </param>
	  /// <exception cref="RuntimeException"> if the condition is not true.
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void assertion(boolean condition, String msg) throws RuntimeException
	  private void assertion(bool condition, string msg)
	  {
		if (!condition)
		{
		  throw new Exception(msg);
		}
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> An error message. </param>
	  /// <param name="e"> An error which the SAXException should wrap.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the current
	  /// <seealso cref="javax.xml.transform.ErrorListener#error"/>
	  /// method chooses to flag this condition as an error.
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void error(String msg, Exception e) throws org.xml.sax.SAXException
	  protected internal virtual void error(string msg, Exception e)
	  {

		SAXSourceLocator locator = Locator;
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;
		TransformerException pe;

		if (!(e is TransformerException))
		{
		  pe = (null == e) ? new TransformerException(msg, locator) : new TransformerException(msg, locator, e);
		}
		else
		{
		  pe = (TransformerException) e;
		}

		if (null != handler)
		{
		  try
		  {
			handler.error(pe);
		  }
		  catch (TransformerException te)
		  {
			throw new org.xml.sax.SAXException(te);
		  }
		}
		else
		{
		  throw new org.xml.sax.SAXException(pe);
		}
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> A key into the <seealso cref="org.apache.xalan.res.XSLTErrorResources"/>
	  /// table, that is one of the WG_ prefixed definitions. </param>
	  /// <param name="args"> An array of arguments for the given warning. </param>
	  /// <param name="e"> An error which the SAXException should wrap.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the current
	  /// <seealso cref="javax.xml.transform.ErrorListener#error"/>
	  /// method chooses to flag this condition as an error.
	  /// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void error(String msg, Object args[], Exception e) throws org.xml.sax.SAXException
	  protected internal virtual void error(string msg, object[] args, Exception e)
	  {

		string formattedMsg = XSLMessages.createMessage(msg, args);

		error(formattedMsg, e);
	  }

	  /// <summary>
	  /// Receive notification of a XSLT processing warning.
	  /// </summary>
	  /// <param name="e"> The warning information encoded as an exception.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the current
	  /// <seealso cref="javax.xml.transform.ErrorListener#warning"/>
	  /// method chooses to flag this condition as an error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
	  public virtual void warning(org.xml.sax.SAXParseException e)
	  {

		string formattedMsg = e.Message;
		SAXSourceLocator locator = Locator;
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;

		try
		{
		  handler.warning(new TransformerException(formattedMsg, locator));
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Receive notification of a recoverable XSLT processing error.
	  /// </summary>
	  /// <param name="e"> The error information encoded as an exception.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the current
	  /// <seealso cref="javax.xml.transform.ErrorListener#error"/>
	  /// method chooses to flag this condition as an error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
	  public virtual void error(org.xml.sax.SAXParseException e)
	  {

		string formattedMsg = e.Message;
		SAXSourceLocator locator = Locator;
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;

		try
		{
		  handler.error(new TransformerException(formattedMsg, locator));
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// Report a fatal XSLT processing error.
	  /// </summary>
	  /// <param name="e"> The error information encoded as an exception.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> that wraps a
	  /// <seealso cref="javax.xml.transform.TransformerException"/> if the current
	  /// <seealso cref="javax.xml.transform.ErrorListener#fatalError"/>
	  /// method chooses to flag this condition as an error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException e) throws org.xml.sax.SAXException
	  public virtual void fatalError(org.xml.sax.SAXParseException e)
	  {

		string formattedMsg = e.Message;
		SAXSourceLocator locator = Locator;
		ErrorListener handler = m_stylesheetProcessor.ErrorListener;

		try
		{
		  handler.fatalError(new TransformerException(formattedMsg, locator));
		}
		catch (TransformerException te)
		{
		  throw new org.xml.sax.SAXException(te);
		}
	  }

	  /// <summary>
	  /// If we have a URL to a XML fragment, this is set
	  /// to false until the ID is found.
	  /// (warning: I worry that this should be in a stack).
	  /// </summary>
	  private bool m_shouldProcess = true;

	  /// <summary>
	  /// If we have a URL to a XML fragment, the value is stored
	  /// in this string, and the m_shouldProcess flag is set to
	  /// false until we match an ID with this string.
	  /// (warning: I worry that this should be in a stack).
	  /// </summary>
	  private string m_fragmentIDString;

	  /// <summary>
	  /// Keep track of the elementID, so we can tell when
	  /// is has completed.  This isn't a real ID, but rather
	  /// a nesting level.  However, it's good enough for
	  /// our purposes.
	  /// (warning: I worry that this should be in a stack).
	  /// </summary>
	  private int m_elementID = 0;

	  /// <summary>
	  /// The ID of the fragment that has been found
	  /// (warning: I worry that this should be in a stack).
	  /// </summary>
	  private int m_fragmentID = 0;

	  /// <summary>
	  /// Check to see if an ID attribute matched the #id, called
	  /// from startElement.
	  /// </summary>
	  /// <param name="attributes"> The specified or defaulted attributes. </param>
	  private void checkForFragmentID(Attributes attributes)
	  {

		if (!m_shouldProcess)
		{
		  if ((null != attributes) && (null != m_fragmentIDString))
		  {
			int n = attributes.Length;

			for (int i = 0; i < n; i++)
			{
			  string name = attributes.getQName(i);

			  if (name.Equals(Constants.ATTRNAME_ID))
			  {
				string val = attributes.getValue(i);

				if (val.Equals(m_fragmentIDString, StringComparison.CurrentCultureIgnoreCase))
				{
				  m_shouldProcess = true;
				  m_fragmentID = m_elementID;
				}
			  }
			}
		  }
		}
	  }

	  /// <summary>
	  ///  The XSLT TransformerFactory for needed services.
	  /// </summary>
	  private TransformerFactoryImpl m_stylesheetProcessor;

	  /// <summary>
	  /// Get the XSLT TransformerFactoryImpl for needed services.
	  /// TODO: This method should be renamed.
	  /// </summary>
	  /// <returns> The TransformerFactoryImpl that owns this handler. </returns>
	  public virtual TransformerFactoryImpl StylesheetProcessor
	  {
		  get
		  {
			return m_stylesheetProcessor;
		  }
	  }

	  /// <summary>
	  /// If getStylesheetType returns this value, the current stylesheet
	  ///  is a root stylesheet.
	  /// @xsl.usage internal
	  /// </summary>
	  public const int STYPE_ROOT = 1;

	  /// <summary>
	  /// If getStylesheetType returns this value, the current stylesheet
	  ///  is an included stylesheet.
	  /// @xsl.usage internal
	  /// </summary>
	  public const int STYPE_INCLUDE = 2;

	  /// <summary>
	  /// If getStylesheetType returns this value, the current stylesheet
	  ///  is an imported stylesheet.
	  /// @xsl.usage internal
	  /// </summary>
	  public const int STYPE_IMPORT = 3;

	  /// <summary>
	  /// The current stylesheet type. </summary>
	  private int m_stylesheetType = STYPE_ROOT;

	  /// <summary>
	  /// Get the type of stylesheet that should be built
	  /// or is being processed.
	  /// </summary>
	  /// <returns> one of STYPE_ROOT, STYPE_INCLUDE, or STYPE_IMPORT. </returns>
	  internal virtual int StylesheetType
	  {
		  get
		  {
			return m_stylesheetType;
		  }
		  set
		  {
			m_stylesheetType = value;
		  }
	  }


	  /// <summary>
	  /// The stack of stylesheets being processed.
	  /// </summary>
	  private Stack m_stylesheets = new Stack();

	  /// <summary>
	  /// Return the stylesheet that this handler is constructing.
	  /// </summary>
	  /// <returns> The current stylesheet that is on top of the stylesheets stack,
	  ///  or null if no stylesheet is on the stylesheets stack. </returns>
	  internal virtual Stylesheet Stylesheet
	  {
		  get
		  {
			return (m_stylesheets.Count == 0) ? null : (Stylesheet) m_stylesheets.Peek();
		  }
	  }

	  /// <summary>
	  /// Return the last stylesheet that was popped off the stylesheets stack.
	  /// </summary>
	  /// <returns> The last popped stylesheet, or null. </returns>
	  internal virtual Stylesheet LastPoppedStylesheet
	  {
		  get
		  {
			return m_lastPoppedStylesheet;
		  }
	  }

	  /// <summary>
	  /// Return the stylesheet root that this handler is constructing.
	  /// </summary>
	  /// <returns> The root stylesheet of the stylesheets tree. </returns>
	  public virtual StylesheetRoot StylesheetRoot
	  {
		  get
		  {
			if (m_stylesheetRoot != null)
			{
				m_stylesheetRoot.Optimizer = m_optimize;
				m_stylesheetRoot.Incremental = m_incremental;
				m_stylesheetRoot.Source_location = m_source_location;
			}
			return m_stylesheetRoot;
		  }
	  }

	  /// <summary>
	  /// The root stylesheet of the stylesheets tree. </summary>
	  internal StylesheetRoot m_stylesheetRoot;

			/// <summary>
			/// The last stylesheet that was popped off the stylesheets stack. </summary>
	  internal Stylesheet m_lastPoppedStylesheet;

	  /// <summary>
	  /// Push the current stylesheet being constructed. If no other stylesheets
	  /// have been pushed onto the stack, assume the argument is a stylesheet
	  /// root, and also set the stylesheet root member.
	  /// </summary>
	  /// <param name="s"> non-null reference to a stylesheet. </param>
	  public virtual void pushStylesheet(Stylesheet s)
	  {

		if (m_stylesheets.Count == 0)
		{
		  m_stylesheetRoot = (StylesheetRoot) s;
		}

		m_stylesheets.Push(s);
	  }

	  /// <summary>
	  /// Pop the last stylesheet pushed, and return the stylesheet that this
	  /// handler is constructing, and set the last popped stylesheet member.
	  /// Also pop the stylesheet locator stack.
	  /// </summary>
	  /// <returns> The stylesheet popped off the stack, or the last popped stylesheet. </returns>
	  internal virtual Stylesheet popStylesheet()
	  {

		// The stylesheetLocatorStack needs to be popped because
		// a locator was pushed in for this stylesheet by the SAXparser by calling
		// setDocumentLocator().
		if (m_stylesheetLocatorStack.Count > 0)
		{
		  m_stylesheetLocatorStack.Pop();
		}

		if (m_stylesheets.Count > 0)
		{
		  m_lastPoppedStylesheet = (Stylesheet) m_stylesheets.Pop();
		}

		// Shouldn't this be null if stylesheets is empty?  -sb
		return m_lastPoppedStylesheet;
	  }

	  /// <summary>
	  /// The stack of current processors.
	  /// </summary>
	  private Stack m_processors = new Stack();

	  /// <summary>
	  /// Get the current XSLTElementProcessor at the top of the stack.
	  /// </summary>
	  /// <returns> Valid XSLTElementProcessor, which should never be null. </returns>
	  internal virtual XSLTElementProcessor CurrentProcessor
	  {
		  get
		  {
			return (XSLTElementProcessor) m_processors.Peek();
		  }
	  }

	  /// <summary>
	  /// Push the current XSLTElementProcessor onto the top of the stack.
	  /// </summary>
	  /// <param name="processor"> non-null reference to the current element processor. </param>
	  internal virtual void pushProcessor(XSLTElementProcessor processor)
	  {
		m_processors.Push(processor);
	  }

	  /// <summary>
	  /// Pop the current XSLTElementProcessor from the top of the stack. </summary>
	  /// <returns> the XSLTElementProcessor which was popped. </returns>
	  internal virtual XSLTElementProcessor popProcessor()
	  {
		return (XSLTElementProcessor) m_processors.Pop();
	  }

	  /// <summary>
	  /// The root of the XSLT Schema, which tells us how to
	  /// transition content handlers, create elements, etc.
	  /// For the moment at least, this can't be static, since
	  /// the processors store state.
	  /// </summary>
	  private XSLTSchema m_schema = new XSLTSchema();

	  /// <summary>
	  /// Get the root of the XSLT Schema, which tells us how to
	  /// transition content handlers, create elements, etc.
	  /// </summary>
	  /// <returns> The root XSLT Schema, which should never be null.
	  /// @xsl.usage internal </returns>
	  public virtual XSLTSchema Schema
	  {
		  get
		  {
			return m_schema;
		  }
	  }

	  /// <summary>
	  /// The stack of elements, pushed and popped as events occur.
	  /// </summary>
	  private Stack m_elems = new Stack();

	  /// <summary>
	  /// Get the current ElemTemplateElement at the top of the stack. </summary>
	  /// <returns> Valid ElemTemplateElement, which may be null. </returns>
	  internal virtual ElemTemplateElement ElemTemplateElement
	  {
		  get
		  {
    
			try
			{
			  return (ElemTemplateElement) m_elems.Peek();
			}
			catch (java.util.EmptyStackException)
			{
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// An increasing number that is used to indicate the order in which this element
	  ///  was encountered during the parse of the XSLT tree.
	  /// </summary>
	  private int m_docOrderCount = 0;

	  /// <summary>
	  /// Returns the next m_docOrderCount number and increments the number for future use.
	  /// </summary>
	  internal virtual int nextUid()
	  {
		return m_docOrderCount++;
	  }

	  /// <summary>
	  /// Push the current XSLTElementProcessor to the top of the stack.  As a
	  /// side-effect, set the document order index (simply because this is a
	  /// convenient place to set it).
	  /// </summary>
	  /// <param name="elem"> Should be a non-null reference to the intended current
	  /// template element. </param>
	  internal virtual void pushElemTemplateElement(ElemTemplateElement elem)
	  {

		if (elem.Uid == -1)
		{
		  elem.Uid = nextUid();
		}

		m_elems.Push(elem);
	  }

	  /// <summary>
	  /// Get the current XSLTElementProcessor from the top of the stack. </summary>
	  /// <returns> the ElemTemplateElement which was popped. </returns>
	  internal virtual ElemTemplateElement popElemTemplateElement()
	  {
		return (ElemTemplateElement) m_elems.Pop();
	  }

	  /// <summary>
	  /// This will act as a stack to keep track of the
	  /// current include base.
	  /// </summary>
	  internal Stack m_baseIdentifiers = new Stack();

	  /// <summary>
	  /// Push a base identifier onto the base URI stack.
	  /// </summary>
	  /// <param name="baseID"> The current base identifier for this position in the
	  /// stylesheet, which may be a fragment identifier, or which may be null. </param>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#base-uri">
	  /// Section 3.2 Base URI of XSLT specification.</a> </seealso>
	  internal virtual void pushBaseIndentifier(string baseID)
	  {

		if (null != baseID)
		{
		  int posOfHash = baseID.IndexOf('#');

		  if (posOfHash > -1)
		  {
			m_fragmentIDString = baseID.Substring(posOfHash + 1);
			m_shouldProcess = false;
		  }
		  else
		  {
			m_shouldProcess = true;
		  }
		}
		else
		{
		  m_shouldProcess = true;
		}

		m_baseIdentifiers.Push(baseID);
	  }

	  /// <summary>
	  /// Pop a base URI from the stack. </summary>
	  /// <returns> baseIdentifier. </returns>
	  internal virtual string popBaseIndentifier()
	  {
		return (string) m_baseIdentifiers.Pop();
	  }

	  /// <summary>
	  /// Return the base identifier.
	  /// </summary>
	  /// <returns> The base identifier of the current stylesheet. </returns>
	  public virtual string BaseIdentifier
	  {
		  get
		  {
    
			// Try to get the baseIdentifier from the baseIdentifier's stack,
			// which may not be the same thing as the value found in the
			// SourceLocators stack.
			string @base = (string)(m_baseIdentifiers.Count == 0 ? null : m_baseIdentifiers.Peek());
    
			// Otherwise try the stylesheet.
			if (null == @base)
			{
			  SourceLocator locator = Locator;
    
			  @base = (null == locator) ? "" : locator.SystemId;
			}
    
			return @base;
		  }
	  }

	  /// <summary>
	  /// The top of this stack should contain the currently processed
	  /// stylesheet SAX locator object.
	  /// </summary>
	  private Stack m_stylesheetLocatorStack = new Stack();

	  /// <summary>
	  /// Get the current stylesheet Locator object.
	  /// </summary>
	  /// <returns> non-null reference to the current locator object. </returns>
	  public virtual SAXSourceLocator Locator
	  {
		  get
		  {
    
			if (m_stylesheetLocatorStack.Count == 0)
			{
			  SAXSourceLocator locator = new SAXSourceLocator();
    
			  locator.SystemId = this.StylesheetProcessor.DOMsystemID;
    
			  return locator;
    
			  // m_stylesheetLocatorStack.push(locator);
			}
    
			return ((SAXSourceLocator) m_stylesheetLocatorStack.Peek());
		  }
	  }

	  /// <summary>
	  /// A stack of URL hrefs for imported stylesheets.  This is
	  /// used to diagnose circular imports.
	  /// </summary>
	  private Stack m_importStack = new Stack();

	  /// <summary>
	  /// A stack of Source objects obtained from a URIResolver,
	  /// for each element in this stack there is a 1-1 correspondence
	  /// with an element in the m_importStack.
	  /// </summary>
	  private Stack m_importSourceStack = new Stack();

	  /// <summary>
	  /// Push an import href onto the stylesheet stack.
	  /// </summary>
	  /// <param name="hrefUrl"> non-null reference to the URL for the current imported
	  /// stylesheet. </param>
	  internal virtual void pushImportURL(string hrefUrl)
	  {
		m_importStack.Push(hrefUrl);
	  }

	  /// <summary>
	  /// Push the Source of an import href onto the stylesheet stack,
	  /// obtained from a URIResolver, null if there is no URIResolver,
	  /// or if that resolver returned null.
	  /// </summary>
	  internal virtual void pushImportSource(Source sourceFromURIResolver)
	  {
		m_importSourceStack.Push(sourceFromURIResolver);
	  }

	  /// <summary>
	  /// See if the imported stylesheet stack already contains
	  /// the given URL.  Used to test for recursive imports.
	  /// </summary>
	  /// <param name="hrefUrl"> non-null reference to a URL string.
	  /// </param>
	  /// <returns> true if the URL is on the import stack. </returns>
	  internal virtual bool importStackContains(string hrefUrl)
	  {
		return stackContains(m_importStack, hrefUrl);
	  }

	  /// <summary>
	  /// Pop an import href from the stylesheet stack.
	  /// </summary>
	  /// <returns> non-null reference to the import URL that was popped. </returns>
	  internal virtual string popImportURL()
	  {
		return (string) m_importStack.Pop();
	  }

	  internal virtual string peekImportURL()
	  {
		return (string) m_importStack.Peek();
	  }

	  internal virtual Source peekSourceFromURIResolver()
	  {
		return (Source) m_importSourceStack.Peek();
	  }

	  /// <summary>
	  /// Pop a Source from a user provided URIResolver, corresponding
	  /// to the URL popped from the m_importStack.
	  /// </summary>
	  internal virtual Source popImportSource()
	  {
		return (Source) m_importSourceStack.Pop();
	  }

	  /// <summary>
	  /// If this is set to true, we've already warned about using the
	  /// older XSLT namespace URL.
	  /// </summary>
	  private bool warnedAboutOldXSLTNamespace = false;

	  /// <summary>
	  /// Stack of NamespaceSupport objects. </summary>
	  internal Stack m_nsSupportStack = new Stack();

	  /// <summary>
	  /// Push a new NamespaceSupport instance.
	  /// </summary>
	  internal virtual void pushNewNamespaceSupport()
	  {
		m_nsSupportStack.Push(new NamespaceSupport2());
	  }

	  /// <summary>
	  /// Pop the current NamespaceSupport object.
	  /// 
	  /// </summary>
	  internal virtual void popNamespaceSupport()
	  {
		m_nsSupportStack.Pop();
	  }

	  /// <summary>
	  /// Get the current NamespaceSupport object.
	  /// </summary>
	  /// <returns> a non-null reference to the current NamespaceSupport object,
	  /// which is the top of the namespace support stack. </returns>
	  internal virtual NamespaceSupport NamespaceSupport
	  {
		  get
		  {
			return (NamespaceSupport) m_nsSupportStack.Peek();
		  }
	  }

	  /// <summary>
	  /// The originating node if the current stylesheet is being created
	  ///  from a DOM. </summary>
	  ///  <seealso cref= org.apache.xml.utils.NodeConsumer </seealso>
	  private Node m_originatingNode;

	  /// <summary>
	  /// Set the node that is originating the SAX event.
	  /// </summary>
	  /// <param name="n"> Reference to node that originated the current event. </param>
	  /// <seealso cref= org.apache.xml.utils.NodeConsumer </seealso>
	  public virtual Node OriginatingNode
	  {
		  set
		  {
			m_originatingNode = value;
		  }
		  get
		  {
			return m_originatingNode;
		  }
	  }


	  /// <summary>
	  /// Stack of booleans that are pushed and popped in start/endElement depending 
	  /// on the value of xml:space=default/preserve.
	  /// </summary>
	  private BoolStack m_spacePreserveStack = new BoolStack();

	  /// <summary>
	  /// Return boolean value from the spacePreserve stack depending on the value 
	  /// of xml:space=default/preserve.
	  /// </summary>
	  /// <returns> true if space should be preserved, false otherwise. </returns>
	  internal virtual bool SpacePreserve
	  {
		  get
		  {
			return m_spacePreserveStack.peek();
		  }
	  }

	  /// <summary>
	  /// Pop boolean value from the spacePreserve stack.
	  /// </summary>
	  internal virtual void popSpaceHandling()
	  {
		m_spacePreserveStack.pop();
	  }

	  /// <summary>
	  /// Push boolean value on to the spacePreserve stack.
	  /// </summary>
	  /// <param name="b"> true if space should be preserved, false otherwise. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void pushSpaceHandling(boolean b) throws org.xml.sax.SAXParseException
	  internal virtual void pushSpaceHandling(bool b)
	  {
		m_spacePreserveStack.push(b);
	  }

	  /// <summary>
	  /// Push boolean value on to the spacePreserve stack depending on the value 
	  /// of xml:space=default/preserve.
	  /// </summary>
	  /// <param name="attrs"> list of attributes that were passed to startElement. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void pushSpaceHandling(org.xml.sax.Attributes attrs) throws org.xml.sax.SAXParseException
	  internal virtual void pushSpaceHandling(Attributes attrs)
	  {
		string value = attrs.getValue("xml:space");
		if (null == value)
		{
		  m_spacePreserveStack.push(m_spacePreserveStack.peekOrFalse());
		}
		else if (value.Equals("preserve"))
		{
		  m_spacePreserveStack.push(true);
		}
		else if (value.Equals("default"))
		{
		  m_spacePreserveStack.push(false);
		}
		else
		{
		  SAXSourceLocator locator = Locator;
		  ErrorListener handler = m_stylesheetProcessor.ErrorListener;

		  try
		  {
			handler.error(new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_ILLEGAL_XMLSPACE_VALUE, null), locator)); //"Illegal value for xml:space", locator));
		  }
		  catch (TransformerException te)
		  {
			throw new org.xml.sax.SAXParseException(te.Message, locator, te);
		  }
		  m_spacePreserveStack.push(m_spacePreserveStack.peek());
		}
	  }

	  private double ElemVersion
	  {
		  get
		  {
			ElemTemplateElement elem = ElemTemplateElement;
			double version = -1;
			while ((version == -1 || version == Constants.XSLTVERSUPPORTED) && elem != null)
			{
			  try
			  {
			  version = Convert.ToDouble(elem.XmlVersion);
			  }
			  catch (Exception)
			  {
				version = -1;
			  }
			  elem = elem.ParentElem;
			}
			return (version == -1)? Constants.XSLTVERSUPPORTED : version;
		  }
	  }
		/// <seealso cref= PrefixResolver#handlesNullPrefixes() </seealso>
		public virtual bool handlesNullPrefixes()
		{
			return false;
		}

		/// <returns> Optimization flag </returns>
		public virtual bool Optimize
		{
			get
			{
				return m_optimize;
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

		/// <returns> Source Location flag </returns>
		public virtual bool Source_location
		{
			get
			{
				return m_source_location;
			}
		}

	}




}