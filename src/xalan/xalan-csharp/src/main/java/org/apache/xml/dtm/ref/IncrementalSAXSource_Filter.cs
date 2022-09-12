using System;

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
 * $Id: IncrementalSAXSource_Filter.java 1225427 2011-12-29 04:33:32Z mrglavas $
 */

namespace org.apache.xml.dtm.@ref
{

	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using ThreadControllerWrapper = org.apache.xml.utils.ThreadControllerWrapper;

	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using InputSource = org.xml.sax.InputSource;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;
	using SAXParseException = org.xml.sax.SAXParseException;
	using XMLReader = org.xml.sax.XMLReader;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// <para>IncrementalSAXSource_Filter implements IncrementalSAXSource, using a
	/// standard SAX2 event source as its input and parcelling out those
	/// events gradually in reponse to deliverMoreNodes() requests.  Output from the
	/// filter will be passed along to a SAX handler registered as our
	/// listener, but those callbacks will pass through a counting stage
	/// which periodically yields control back to the controller coroutine.
	/// </para>
	/// 
	/// <para>%REVIEW%: This filter is not currenly intended to be reusable
	/// for parsing additional streams/documents. We may want to consider
	/// making it resettable at some point in the future. But it's a 
	/// small object, so that'd be mostly a convenience issue; the cost
	/// of allocating each time is trivial compared to the cost of processing
	/// any nontrival stream.</para>
	/// 
	/// <para>For a brief usage example, see the unit-test main() method.</para>
	/// 
	/// <para>This is a simplification of the old CoroutineSAXParser, focusing
	/// specifically on filtering. The resulting controller protocol is _far_
	/// simpler and less error-prone; the only controller operation is deliverMoreNodes(),
	/// and the only requirement is that deliverMoreNodes(false) be called if you want to
	/// discard the rest of the stream and the previous deliverMoreNodes() didn't return
	/// false.
	/// 
	/// </para>
	/// </summary>
	public class IncrementalSAXSource_Filter : IncrementalSAXSource, ContentHandler, DTDHandler, LexicalHandler, ErrorHandler, System.Threading.ThreadStart
	{
	  internal bool DEBUG = false; //Internal status report

	  //
	  // Data
	  //
	  private CoroutineManager fCoroutineManager = null;
	  private int fControllerCoroutineID = -1;
	  private int fSourceCoroutineID = -1;

	  private ContentHandler clientContentHandler = null; // %REVIEW% support multiple?
	  private LexicalHandler clientLexicalHandler = null; // %REVIEW% support multiple?
	  private DTDHandler clientDTDHandler = null; // %REVIEW% support multiple?
	  private ErrorHandler clientErrorHandler = null; // %REVIEW% support multiple?
	  private int eventcounter;
	  private int frequency = 5;

	  // Flag indicating that no more events should be delivered -- either
	  // because input stream ran to completion (endDocument), or because
	  // the user requested an early stop via deliverMoreNodes(false).
	  private bool fNoMoreEvents = false;

	  // Support for startParse()
	  private XMLReader fXMLReader = null;
	  private InputSource fXMLReaderInputSource = null;

	  //
	  // Constructors
	  //

	  public IncrementalSAXSource_Filter()
	  {
		this.init(new CoroutineManager(), -1, -1);
	  }

	  /// <summary>
	  /// Create a IncrementalSAXSource_Filter which is not yet bound to a specific
	  /// SAX event source.
	  /// 
	  /// </summary>
	  public IncrementalSAXSource_Filter(CoroutineManager co, int controllerCoroutineID)
	  {
		this.init(co, controllerCoroutineID, -1);
	  }

	  //
	  // Factories
	  //
	  public static IncrementalSAXSource createIncrementalSAXSource(CoroutineManager co, int controllerCoroutineID)
	  {
		return new IncrementalSAXSource_Filter(co, controllerCoroutineID);
	  }

	  //
	  // Public methods
	  //

	  public virtual void init(CoroutineManager co, int controllerCoroutineID, int sourceCoroutineID)
	  {
		if (co == null)
		{
		  co = new CoroutineManager();
		}
		fCoroutineManager = co;
		fControllerCoroutineID = co.co_joinCoroutineSet(controllerCoroutineID);
		fSourceCoroutineID = co.co_joinCoroutineSet(sourceCoroutineID);
		if (fControllerCoroutineID == -1 || fSourceCoroutineID == -1)
		{
		  throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_COJOINROUTINESET_FAILED, null)); //"co_joinCoroutineSet() failed");
		}

		fNoMoreEvents = false;
		eventcounter = frequency;
	  }

	  /// <summary>
	  /// Bind our input streams to an XMLReader.
	  /// 
	  /// Just a convenience routine; obviously you can explicitly register
	  /// this as a listener with the same effect.
	  /// 
	  /// </summary>
	  public virtual XMLReader XMLReader
	  {
		  set
		  {
			fXMLReader = value;
			value.ContentHandler = this;
			value.DTDHandler = this;
			value.ErrorHandler = this; // to report fatal errors in filtering mode
    
			// Not supported by all SAX2 filters:
			try
			{
			  value.setProperty("http://xml.org/sax/properties/lexical-handler", this);
			}
			catch (SAXNotRecognizedException)
			{
			  // Nothing we can do about it
			}
			catch (SAXNotSupportedException)
			{
			  // Nothing we can do about it
			}
    
			// Should we also bind as other varieties of handler?
			// (DTDHandler and so on)
		  }
	  }

	  // Register a content handler for us to output to
	  public virtual ContentHandler ContentHandler
	  {
		  set
		  {
			clientContentHandler = value;
		  }
	  }
	  // Register a DTD handler for us to output to
	  public virtual DTDHandler DTDHandler
	  {
		  set
		  {
			clientDTDHandler = value;
		  }
	  }
	  // Register a lexical handler for us to output to
	  // Not all filters support this...
	  // ??? Should we register directly on the filter?
	  // NOTE NAME -- subclassing issue in the Xerces version
	  public virtual LexicalHandler LexicalHandler
	  {
		  set
		  {
			clientLexicalHandler = value;
		  }
	  }
	  // Register an error handler for us to output to
	  // NOTE NAME -- subclassing issue in the Xerces version
	  public virtual ErrorHandler ErrHandler
	  {
		  set
		  {
			clientErrorHandler = value;
		  }
	  }

	  // Set the number of events between resumes of our coroutine
	  // Immediately resets number of events before _next_ resume as well.
	  public virtual int ReturnFrequency
	  {
		  set
		  {
			if (value < 1)
			{
				value = 1;
			}
			frequency = eventcounter = value;
		  }
	  }

	  //
	  // ContentHandler methods
	  // These  pass the data to our client ContentHandler...
	  // but they also count the number of events passing through,
	  // and resume our coroutine each time that counter hits zero and
	  // is reset.
	  //
	  // Note that for everything except endDocument and fatalError, we do the count-and-yield
	  // BEFORE passing the call along. I'm hoping that this will encourage JIT
	  // compilers to realize that these are tail-calls, reducing the expense of
	  // the additional layer of data flow.
	  //
	  // %REVIEW% Glenn suggests that pausing after endElement, endDocument,
	  // and characters may be sufficient. I actually may not want to
	  // stop after characters, since in our application these wind up being
	  // concatenated before they're processed... but that risks huge blocks of
	  // text causing greater than usual readahead. (Unlikely? Consider the
	  // possibility of a large base-64 block in a SOAP stream.)
	  //
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual void characters(char[] ch, int start, int length)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.characters(ch,start,length);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
	  public virtual void endDocument()
	  {
		// EXCEPTION: In this case we need to run the event BEFORE we yield.
		if (clientContentHandler != null)
		{
		  clientContentHandler.endDocument();
		}

		eventcounter = 0;
		co_yield(false);
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(java.lang.String namespaceURI, java.lang.String localName, java.lang.String qName) throws org.xml.sax.SAXException
	  public virtual void endElement(string namespaceURI, string localName, string qName)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.endElement(namespaceURI,localName,qName);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endPrefixMapping(java.lang.String prefix) throws org.xml.sax.SAXException
	  public virtual void endPrefixMapping(string prefix)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.endPrefixMapping(prefix);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void ignorableWhitespace(char[] ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual void ignorableWhitespace(char[] ch, int start, int length)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.ignorableWhitespace(ch,start,length);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(java.lang.String target, java.lang.String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.processingInstruction(target,data);
		}
	  }
	  public virtual Locator DocumentLocator
	  {
		  set
		  {
			if (--eventcounter <= 0)
			{
				// This can cause a hang.  -sb
				// co_yield(true);
				eventcounter = frequency;
			}
			if (clientContentHandler != null)
			{
			  clientContentHandler.DocumentLocator = value;
			}
		  }
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void skippedEntity(java.lang.String name) throws org.xml.sax.SAXException
	  public virtual void skippedEntity(string name)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.skippedEntity(name);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
	  public virtual void startDocument()
	  {
		co_entry_pause();

		// Otherwise, begin normal event delivery
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.startDocument();
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(java.lang.String namespaceURI, java.lang.String localName, java.lang.String qName, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
	  public virtual void startElement(string namespaceURI, string localName, string qName, Attributes atts)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.startElement(namespaceURI, localName, qName, atts);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startPrefixMapping(java.lang.String prefix, java.lang.String uri) throws org.xml.sax.SAXException
	  public virtual void startPrefixMapping(string prefix, string uri)
	  {
		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
		if (clientContentHandler != null)
		{
		  clientContentHandler.startPrefixMapping(prefix,uri);
		}
	  }

	  //
	  // LexicalHandler support. Not all SAX2 filters support these events
	  // but we may want to pass them through when they exist...
	  //
	  // %REVIEW% These do NOT currently affect the eventcounter; I'm asserting
	  // that they're rare enough that it makes little or no sense to
	  // pause after them. As such, it may make more sense for folks who
	  // actually want to use them to register directly with the filter.
	  // But I want 'em here for now, to remind us to recheck this assertion!
	  //
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char[] ch, int start, int length) throws org.xml.sax.SAXException
	  public virtual void comment(char[] ch, int start, int length)
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.comment(ch,start,length);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCDATA() throws org.xml.sax.SAXException
	  public virtual void endCDATA()
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.endCDATA();
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
	  public virtual void endDTD()
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.endDTD();
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endEntity(java.lang.String name) throws org.xml.sax.SAXException
	  public virtual void endEntity(string name)
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.endEntity(name);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startCDATA() throws org.xml.sax.SAXException
	  public virtual void startCDATA()
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.startCDATA();
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDTD(java.lang.String name, java.lang.String publicId, java.lang.String systemId) throws org.xml.sax.SAXException
	  public virtual void startDTD(string name, string publicId, string systemId)
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.startDTD(name, publicId, systemId);
		}
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startEntity(java.lang.String name) throws org.xml.sax.SAXException
	  public virtual void startEntity(string name)
	  {
		if (null != clientLexicalHandler)
		{
		  clientLexicalHandler.startEntity(name);
		}
	  }

	  //
	  // DTDHandler support.

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notationDecl(String a, String b, String c) throws org.xml.sax.SAXException
	  public virtual void notationDecl(string a, string b, string c)
	  {
		  if (null != clientDTDHandler)
		  {
			  clientDTDHandler.notationDecl(a,b,c);
		  }
	  }
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void unparsedEntityDecl(String a, String b, String c, String d) throws org.xml.sax.SAXException
	  public virtual void unparsedEntityDecl(string a, string b, string c, string d)
	  {
		  if (null != clientDTDHandler)
		  {
			  clientDTDHandler.unparsedEntityDecl(a,b,c,d);
		  }
	  }

	  //
	  // ErrorHandler support.
	  //
	  // PROBLEM: Xerces is apparently _not_ calling the ErrorHandler for
	  // exceptions thrown by the ContentHandler, which prevents us from
	  // handling this properly when running in filtering mode with Xerces
	  // as our event source.  It's unclear whether this is a Xerces bug
	  // or a SAX design flaw.
	  // 
	  // %REVIEW% Current solution: In filtering mode, it is REQUIRED that
	  // event source make sure this method is invoked if the event stream
	  // abends before endDocument is delivered. If that means explicitly calling
	  // us in the exception handling code because it won't be delivered as part
	  // of the normal SAX ErrorHandler stream, that's fine; Not Our Problem.
	  //
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
	  public virtual void error(SAXParseException exception)
	  {
		if (null != clientErrorHandler)
		{
		  clientErrorHandler.error(exception);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
	  public virtual void fatalError(SAXParseException exception)
	  {
		// EXCEPTION: In this case we need to run the event BEFORE we yield --
		// just as with endDocument, this terminates the event stream.
		if (null != clientErrorHandler)
		{
		  clientErrorHandler.error(exception);
		}

		eventcounter = 0;
		co_yield(false);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
	  public virtual void warning(SAXParseException exception)
	  {
		if (null != clientErrorHandler)
		{
		  clientErrorHandler.error(exception);
		}
	  }


	  //
	  // coroutine support
	  //

	  public virtual int SourceCoroutineID
	  {
		  get
		  {
			return fSourceCoroutineID;
		  }
	  }
	  public virtual int ControllerCoroutineID
	  {
		  get
		  {
			return fControllerCoroutineID;
		  }
	  }

	  /// <returns> the CoroutineManager this CoroutineFilter object is bound to.
	  /// If you're using the do...() methods, applications should only
	  /// need to talk to the CoroutineManager once, to obtain the
	  /// application's Coroutine ID.
	  ///  </returns>
	  public virtual CoroutineManager CoroutineManager
	  {
		  get
		  {
			return fCoroutineManager;
		  }
	  }

	  /// <summary>
	  /// <para>In the SAX delegation code, I've inlined the count-down in
	  /// the hope of encouraging compilers to deliver better
	  /// performance. However, if we subclass (eg to directly connect the
	  /// output to a DTM builder), that would require calling super in
	  /// order to run that logic... which seems inelegant.  Hence this
	  /// routine for the convenience of subclasses: every [frequency]
	  /// invocations, issue a co_yield.</para>
	  /// </summary>
	  /// <param name="moreExepected"> Should always be true unless this is being called
	  /// at the end of endDocument() handling.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void count_and_yield(boolean moreExpected) throws org.xml.sax.SAXException
	  protected internal virtual void count_and_yield(bool moreExpected)
	  {
		if (!moreExpected)
		{
			eventcounter = 0;
		}

		if (--eventcounter <= 0)
		{
			co_yield(true);
			eventcounter = frequency;
		}
	  }

	  /// <summary>
	  /// co_entry_pause is called in startDocument() before anything else
	  /// happens. It causes the filter to wait for a "go ahead" request
	  /// from the controller before delivering any events. Note that
	  /// the very first thing the controller tells us may be "I don't
	  /// need events after all"!
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void co_entry_pause() throws org.xml.sax.SAXException
	  private void co_entry_pause()
	  {
		if (fCoroutineManager == null)
		{
		  // Nobody called init()? Do it now...
		  init(null,-1,-1);
		}

		try
		{
		  object arg = fCoroutineManager.co_entry_pause(fSourceCoroutineID);
		  if (arg == false)
		  {
			co_yield(false);
		  }
		}
		catch (NoSuchMethodException e)
		{
		  // Coroutine system says we haven't registered. That's an
		  // application coding error, and is unrecoverable.
		  if (DEBUG)
		  {
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
		  }
		  throw new SAXException(e);
		}
	  }

	  /// <summary>
	  /// Co_Yield handles coroutine interactions while a parse is in progress.
	  /// 
	  /// When moreRemains==true, we are pausing after delivering events, to
	  /// ask if more are needed. We will resume the controller thread with 
	  ///   co_resume(Boolean.TRUE, ...)
	  /// When control is passed back it may indicate
	  ///      Boolean.TRUE    indication to continue delivering events
	  ///      Boolean.FALSE   indication to discontinue events and shut down.
	  /// 
	  /// When moreRemains==false, we shut down immediately without asking the
	  /// controller's permission. Normally this means end of document has been
	  /// reached.
	  /// 
	  /// Shutting down a IncrementalSAXSource_Filter requires terminating the incoming
	  /// SAX event stream. If we are in control of that stream (if it came
	  /// from an XMLReader passed to our startReader() method), we can do so
	  /// very quickly by throwing a reserved exception to it. If the stream is
	  /// coming from another source, we can't do that because its caller may
	  /// not be prepared for this "normal abnormal exit", and instead we put
	  /// ourselves in a "spin" mode where events are discarded.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void co_yield(boolean moreRemains) throws org.xml.sax.SAXException
	  private void co_yield(bool moreRemains)
	  {
		// Horrendous kluge to run filter to completion. See below.
		if (fNoMoreEvents)
		{
		  return;
		}

		try // Coroutine manager might throw no-such.
		{
		  object arg = false;
		  if (moreRemains)
		  {
			// Yield control, resume parsing when done
			arg = fCoroutineManager.co_resume(true, fSourceCoroutineID, fControllerCoroutineID);

		  }

		  // If we're at end of document or were told to stop early
		  if (arg == false)
		  {
			fNoMoreEvents = true;

			if (fXMLReader != null) // Running under startParseThread()
			{
			  throw new StopException(); // We'll co_exit from there.
			}

			// Yield control. We do NOT expect anyone to ever ask us again.
			fCoroutineManager.co_exit_to(false, fSourceCoroutineID, fControllerCoroutineID);
		  }
		}
		catch (NoSuchMethodException e)
		{
		  // Shouldn't happen unless we've miscoded our coroutine logic
		  // "Shut down the garbage smashers on the detention level!"
		  fNoMoreEvents = true;
		  fCoroutineManager.co_exit(fSourceCoroutineID);
		  throw new SAXException(e);
		}
	  }

	  //
	  // Convenience: Run an XMLReader in a thread
	  //

	  /// <summary>
	  /// Launch a thread that will run an XMLReader's parse() operation within
	  ///  a thread, feeding events to this IncrementalSAXSource_Filter. Mostly a convenience
	  ///  routine, but has the advantage that -- since we invoked parse() --
	  ///  we can halt parsing quickly via a StopException rather than waiting
	  ///  for the SAX stream to end by itself.
	  /// </summary>
	  /// <exception cref="SAXException"> is parse thread is already in progress
	  /// or parsing can not be started.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startParse(org.xml.sax.InputSource source) throws org.xml.sax.SAXException
	  public virtual void startParse(InputSource source)
	  {
		if (fNoMoreEvents)
		{
		  throw new SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, null)); //"IncrmentalSAXSource_Filter not currently restartable.");
		}
		if (fXMLReader == null)
		{
		  throw new SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_XMLRDR_NOT_BEFORE_STARTPARSE, null)); //"XMLReader not before startParse request");
		}

		fXMLReaderInputSource = source;

		// Xalan thread pooling...
		// org.apache.xalan.transformer.TransformerImpl.runTransformThread(this);
		ThreadControllerWrapper.runThread(this, -1);
	  }

	  /* Thread logic to support startParseThread()
	   */
	  public virtual void run()
	  {
		// Guard against direct invocation of start().
		if (fXMLReader == null)
		{
			return;
		}

		if (DEBUG)
		{
			Console.WriteLine("IncrementalSAXSource_Filter parse thread launched");
		}

		// Initially assume we'll run successfully.
		object arg = false;

		// For the duration of this operation, all coroutine handshaking
		// will occur in the co_yield method. That's the nice thing about
		// coroutines; they give us a way to hand off control from the
		// middle of a synchronous method.
		try
		{
		  fXMLReader.parse(fXMLReaderInputSource);
		}
		catch (IOException ex)
		{
		  arg = ex;
		}
		catch (StopException)
		{
		  // Expected and harmless
		  if (DEBUG)
		  {
			  Console.WriteLine("Active IncrementalSAXSource_Filter normal stop exception");
		  }
		}
		catch (SAXException ex)
		{
		  Exception inner = ex.Exception;
		  if (inner is StopException)
		  {
			// Expected and harmless
			if (DEBUG)
			{
				Console.WriteLine("Active IncrementalSAXSource_Filter normal stop exception");
			}
		  }
		  else
		  {
			// Unexpected malfunction
			if (DEBUG)
			{
			  Console.WriteLine("Active IncrementalSAXSource_Filter UNEXPECTED SAX exception: " + inner);
			  Console.WriteLine(inner.ToString());
			  Console.Write(inner.StackTrace);
			}
			arg = ex;
		  }
		} // end parse

		// Mark as no longer running in thread.
		fXMLReader = null;

		try
		{
		  // Mark as done and yield control to the controller coroutine
		  fNoMoreEvents = true;
		  fCoroutineManager.co_exit_to(arg, fSourceCoroutineID, fControllerCoroutineID);
		}
		catch (java.lang.NoSuchMethodException e)
		{
		  // Shouldn't happen unless we've miscoded our coroutine logic
		  // "CPO, shut down the garbage smashers on the detention level!"
		  e.printStackTrace(System.err);
		  fCoroutineManager.co_exit(fSourceCoroutineID);
		}
	  }

	  /// <summary>
	  /// Used to quickly terminate parse when running under a
	  ///    startParse() thread. Only its type is important. 
	  /// </summary>
	  internal class StopException : Exception
	  {
			  internal const long serialVersionUID = -1129245796185754956L;
	  }

	  /// <summary>
	  /// deliverMoreNodes() is a simple API which tells the coroutine
	  /// parser that we need more nodes.  This is intended to be called
	  /// from one of our partner routines, and serves to encapsulate the
	  /// details of how incremental parsing has been achieved.
	  /// </summary>
	  /// <param name="parsemore"> If true, tells the incremental filter to generate
	  /// another chunk of output. If false, tells the filter that we're
	  /// satisfied and it can terminate parsing of this document.
	  /// </param>
	  /// <returns> Boolean.TRUE if there may be more events available by invoking
	  /// deliverMoreNodes() again. Boolean.FALSE if parsing has run to completion (or been
	  /// terminated by deliverMoreNodes(false). Or an exception object if something
	  /// malfunctioned. %REVIEW% We _could_ actually throw the exception, but
	  /// that would require runinng deliverMoreNodes() in a try/catch... and for many
	  /// applications, exception will be simply be treated as "not TRUE" in
	  /// any case.
	  ///  </returns>
	  public virtual object deliverMoreNodes(bool parsemore)
	  {
		// If parsing is already done, we can immediately say so
		if (fNoMoreEvents)
		{
		  return false;
		}

		try
		{
		  object result = fCoroutineManager.co_resume(parsemore?true:false, fControllerCoroutineID, fSourceCoroutineID);
		  if (result == false)
		  {
			fCoroutineManager.co_exit(fControllerCoroutineID);
		  }

		  return result;
		}

		// SHOULD NEVER OCCUR, since the coroutine number and coroutine manager
		// are those previously established for this IncrementalSAXSource_Filter...
		// So I'm just going to return it as a parsing exception, for now.
		catch (NoSuchMethodException e)
		{
			return e;
		}
	  }


	  //================================================================
	  /// <summary>
	  /// Simple unit test. Attempt coroutine parsing of document indicated
	  /// by first argument (as a URI), report progress.
	  /// </summary>
		/*
	  public static void main(String args[])
	  {
		System.out.println("Starting...");
	
		org.xml.sax.XMLReader theSAXParser=
		  new org.apache.xerces.parsers.SAXParser();
	
	
		for(int arg=0;arg<args.length;++arg)
		{
		  // The filter is not currently designed to be restartable
		  // after a parse has ended. Generate a new one each time.
		  IncrementalSAXSource_Filter filter=
		    new IncrementalSAXSource_Filter();
		  // Use a serializer as our sample output
		  org.apache.xml.serialize.XMLSerializer trace;
		  trace=new org.apache.xml.serialize.XMLSerializer(System.out,null);
		  filter.setContentHandler(trace);
		  filter.setLexicalHandler(trace);
	
		  try
		  {
		    InputSource source = new InputSource(args[arg]);
		    Object result=null;
		    boolean more=true;
	
		    // init not issued; we _should_ automagically Do The Right Thing
	
		    // Bind parser, kick off parsing in a thread
		    filter.setXMLReader(theSAXParser);
		    filter.startParse(source);
		  
		    for(result = filter.deliverMoreNodes(more);
		        (result instanceof Boolean && ((Boolean)result)==Boolean.TRUE);
		        result = filter.deliverMoreNodes(more))
		    {
		      System.out.println("\nSome parsing successful, trying more.\n");
		      
		      // Special test: Terminate parsing early.
		      if(arg+1<args.length && "!".equals(args[arg+1]))
		      {
		        ++arg;
		        more=false;
		      }
		      
		    }
		  
		    if (result instanceof Boolean && ((Boolean)result)==Boolean.FALSE)
		    {
		      System.out.println("\nFilter ended (EOF or on request).\n");
		    }
		    else if (result == null) {
		      System.out.println("\nUNEXPECTED: Filter says shut down prematurely.\n");
		    }
		    else if (result instanceof Exception) {
		      System.out.println("\nFilter threw exception:");
		      ((Exception)result).printStackTrace();
		    }
		  
		  }
		  catch(SAXException e)
		  {
		    e.printStackTrace();
		  }
		} // end for
	  }
		*/  
	} // class IncrementalSAXSource_Filter

}