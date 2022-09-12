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
 * $Id: IncrementalSAXSource_Xerces.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xml.dtm.@ref
{


	using SAXParser = org.apache.xerces.parsers.SAXParser;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using XMLReader = org.xml.sax.XMLReader;


	/// <summary>
	/// <para>IncrementalSAXSource_Xerces takes advantage of the fact that Xerces1
	/// incremental mode is already a coroutine of sorts, and just wraps our
	/// IncrementalSAXSource API around it.</para>
	/// 
	/// <para>Usage example: See main().</para>
	/// 
	/// <para>Status: Passes simple main() unit-test. NEEDS JAVADOC.</para>
	/// 
	/// </summary>
	public class IncrementalSAXSource_Xerces : IncrementalSAXSource
	{
	  //
	  // Reflection. To allow this to compile with both Xerces1 and Xerces2, which
	  // require very different methods and objects, we need to avoid static 
	  // references to those APIs. So until Xerces2 is pervasive and we're willing 
	  // to make it a prerequisite, we will rely upon relection.
	  //
	  internal Method fParseSomeSetup = null; // Xerces1 method
	  internal Method fParseSome = null; // Xerces1 method
	  internal object fPullParserConfig = null; // Xerces2 pull control object
	  internal Method fConfigSetInput = null; // Xerces2 method
	  internal Method fConfigParse = null; // Xerces2 method
	  internal Method fSetInputSource = null; // Xerces2 pull control method
	  internal Constructor fConfigInputSourceCtor = null; // Xerces2 initialization method
	  internal Method fConfigSetByteStream = null; // Xerces2 initialization method
	  internal Method fConfigSetCharStream = null; // Xerces2 initialization method
	  internal Method fConfigSetEncoding = null; // Xerces2 initialization method
	  internal Method fReset = null; // Both Xerces1 and Xerces2, but diff. signatures

	  //
	  // Data
	  //
	  internal SAXParser fIncrementalParser;
	  private bool fParseInProgress = false;

	  //
	  // Constructors
	  //

	  /// <summary>
	  /// Create a IncrementalSAXSource_Xerces, and create a SAXParser
	  /// to go with it. Xerces2 incremental parsing is only supported if
	  /// this constructor is used, due to limitations in the Xerces2 API (as of
	  /// Beta 3). If you don't like that restriction, tell the Xerces folks that
	  /// there should be a simpler way to request incremental SAX parsing.
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IncrementalSAXSource_Xerces() throws NoSuchMethodException
	  public IncrementalSAXSource_Xerces()
	  {
			try
			{
				// Xerces-2 incremental parsing support (as of Beta 3)
				// ContentHandlers still get set on fIncrementalParser (to get
				// conversion from XNI events to SAX events), but
				// _control_ for incremental parsing must be exercised via the config.
				// 
				// At this time there's no way to read the existing config, only 
				// to assert a new one... and only when creating a brand-new parser.
				//
				// Reflection is used to allow us to continue to compile against
				// Xerces1. If/when we can abandon the older versions of the parser,
				// this will simplify significantly.

				// If we can't get the magic constructor, no need to look further.
				Type xniConfigClass = ObjectFactory.findProviderClass("org.apache.xerces.xni.parser.XMLParserConfiguration", ObjectFactory.findClassLoader(), true);
				Type[] args1 = new Type[] {xniConfigClass};
				Constructor ctor = typeof(SAXParser).GetConstructor(args1);

				// Build the parser configuration object. StandardParserConfiguration
				// happens to implement XMLPullParserConfiguration, which is the API
				// we're going to want to use.
				Type xniStdConfigClass = ObjectFactory.findProviderClass("org.apache.xerces.parsers.StandardParserConfiguration", ObjectFactory.findClassLoader(), true);
				fPullParserConfig = xniStdConfigClass.newInstance();
				object[] args2 = new object[] {fPullParserConfig};
				fIncrementalParser = (SAXParser)ctor.newInstance(args2);

				// Preload all the needed the configuration methods... I want to know they're
				// all here before we commit to trying to use them, just in case the
				// API changes again.
				Type fXniInputSourceClass = ObjectFactory.findProviderClass("org.apache.xerces.xni.parser.XMLInputSource", ObjectFactory.findClassLoader(), true);
				Type[] args3 = new Type[] {fXniInputSourceClass};
				fConfigSetInput = xniStdConfigClass.GetMethod("setInputSource",args3);

				Type[] args4 = new Type[] {typeof(string),typeof(string),typeof(string)};
				fConfigInputSourceCtor = fXniInputSourceClass.GetConstructor(args4);
				Type[] args5 = new Type[] {typeof(System.IO.Stream)};
				fConfigSetByteStream = fXniInputSourceClass.GetMethod("setByteStream",args5);
				Type[] args6 = new Type[] {typeof(java.io.Reader)};
				fConfigSetCharStream = fXniInputSourceClass.GetMethod("setCharacterStream",args6);
				Type[] args7 = new Type[] {typeof(string)};
				fConfigSetEncoding = fXniInputSourceClass.GetMethod("setEncoding",args7);

				Type[] argsb = new Type[] {Boolean.TYPE};
				fConfigParse = xniStdConfigClass.GetMethod("parse",argsb);
				Type[] noargs = new Type[0];
				fReset = fIncrementalParser.GetType().GetMethod("reset",noargs);
			}
			catch (Exception)
			{
			// Fallback if this fails (implemented in createIncrementalSAXSource) is
				// to attempt Xerces-1 incremental setup. Can't do tail-call in
				// constructor, so create new, copy Xerces-1 initialization, 
				// then throw it away... Ugh.
				IncrementalSAXSource_Xerces dummy = new IncrementalSAXSource_Xerces(new SAXParser());
				this.fParseSomeSetup = dummy.fParseSomeSetup;
				this.fParseSome = dummy.fParseSome;
				this.fIncrementalParser = dummy.fIncrementalParser;
			}
	  }

	  /// <summary>
	  /// Create a IncrementalSAXSource_Xerces wrapped around
	  /// an existing SAXParser. Currently this works only for recent
	  /// releases of Xerces-1.  Xerces-2 incremental is currently possible
	  /// only if we are allowed to create the parser instance, due to
	  /// limitations in the API exposed by Xerces-2 Beta 3; see the
	  /// no-args constructor for that code.
	  /// </summary>
	  /// <exception cref="if"> the SAXParser class doesn't support the Xerces
	  /// incremental parse operations. In that case, caller should
	  /// fall back upon the IncrementalSAXSource_Filter approach.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IncrementalSAXSource_Xerces(org.apache.xerces.parsers.SAXParser parser) throws NoSuchMethodException
	  public IncrementalSAXSource_Xerces(SAXParser parser)
	  {
			// Reflection is used to allow us to compile against
			// Xerces2. If/when we can abandon the older versions of the parser,
			// this constructor will simply have to fail until/unless the
			// Xerces2 incremental support is made available on previously
			// constructed SAXParser instances.
		fIncrementalParser = parser;
			Type me = parser.GetType();
		Type[] parms = new Type[] {typeof(InputSource)};
		fParseSomeSetup = me.GetMethod("parseSomeSetup",parms);
		parms = new Type[0];
		fParseSome = me.GetMethod("parseSome",parms);
		// Fallback if this fails (implemented in createIncrementalSAXSource) is
		// to use IncrementalSAXSource_Filter rather than Xerces-specific code.
	  }

	  //
	  // Factories
	  //
	  public static IncrementalSAXSource createIncrementalSAXSource()
	  {
			try
			{
				return new IncrementalSAXSource_Xerces();
			}
			catch (NoSuchMethodException)
			{
				// Xerces version mismatch; neither Xerces1 nor Xerces2 succeeded.
				// Fall back on filtering solution.
				IncrementalSAXSource_Filter iss = new IncrementalSAXSource_Filter();
				iss.XMLReader = new SAXParser();
				return iss;
			}
	  }

	  public static IncrementalSAXSource createIncrementalSAXSource(SAXParser parser)
	  {
			try
			{
				return new IncrementalSAXSource_Xerces(parser);
			}
			catch (NoSuchMethodException)
			{
				// Xerces version mismatch; neither Xerces1 nor Xerces2 succeeded.
				// Fall back on filtering solution.
				IncrementalSAXSource_Filter iss = new IncrementalSAXSource_Filter();
				iss.XMLReader = parser;
				return iss;
			}
	  }

	  //
	  // Public methods
	  //

	  // Register handler directly with the incremental parser
	  public virtual org.xml.sax.ContentHandler ContentHandler
	  {
		  set
		  {
			// Typecast required in Xerces2; SAXParser doesn't inheret XMLReader
			// %OPT% Cast at asignment?
			((XMLReader)fIncrementalParser).ContentHandler = value;
		  }
	  }

	  // Register handler directly with the incremental parser
	  public virtual org.xml.sax.ext.LexicalHandler LexicalHandler
	  {
		  set
		  {
			// Not supported by all SAX2 parsers but should work in Xerces:
			try
			{
			  // Typecast required in Xerces2; SAXParser doesn't inheret XMLReader
			  // %OPT% Cast at asignment?
			  ((XMLReader)fIncrementalParser).setProperty("http://xml.org/sax/properties/lexical-handler", value);
			}
			catch (org.xml.sax.SAXNotRecognizedException)
			{
			  // Nothing we can do about it
			}
			catch (org.xml.sax.SAXNotSupportedException)
			{
			  // Nothing we can do about it
			}
		  }
	  }

	  // Register handler directly with the incremental parser
	  public virtual org.xml.sax.DTDHandler DTDHandler
	  {
		  set
		  {
			// Typecast required in Xerces2; SAXParser doesn't inheret XMLReader
			// %OPT% Cast at asignment?
			((XMLReader)fIncrementalParser).DTDHandler = value;
		  }
	  }

	  //================================================================
	  /// <summary>
	  /// startParse() is a simple API which tells the IncrementalSAXSource
	  /// to begin reading a document.
	  /// </summary>
	  /// <exception cref="SAXException"> is parse thread is already in progress
	  /// or parsing can not be started.
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startParse(org.xml.sax.InputSource source) throws org.xml.sax.SAXException
	  public virtual void startParse(InputSource source)
	  {
		if (fIncrementalParser == null)
		{
		  throw new SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_STARTPARSE_NEEDS_SAXPARSER, null)); //"startParse needs a non-null SAXParser.");
		}
		if (fParseInProgress)
		{
		  throw new SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_STARTPARSE_WHILE_PARSING, null)); //"startParse may not be called while parsing.");
		}

		bool ok = false;

		try
		{
		  ok = parseSomeSetup(source);
		}
		catch (Exception ex)
		{
		  throw new SAXException(ex);
		}

		if (!ok)
		{
		  throw new SAXException(XMLMessages.createXMLMessage(XMLErrorResources.ER_COULD_NOT_INIT_PARSER, null)); //"could not initialize parser with");
		}
	  }


	  /// <summary>
	  /// deliverMoreNodes() is a simple API which tells the coroutine
	  /// parser that we need more nodes.  This is intended to be called
	  /// from one of our partner routines, and serves to encapsulate the
	  /// details of how incremental parsing has been achieved.
	  /// </summary>
	  /// <param name="parsemore"> If true, tells the incremental parser to generate
	  /// another chunk of output. If false, tells the parser that we're
	  /// satisfied and it can terminate parsing of this document. </param>
	  /// <returns> Boolean.TRUE if the CoroutineParser believes more data may be available
	  /// for further parsing. Boolean.FALSE if parsing ran to completion.
	  /// Exception if the parser objected for some reason.
	  ///  </returns>
	  public virtual object deliverMoreNodes(bool parsemore)
	  {
		if (!parsemore)
		{
		  fParseInProgress = false;
		  return false;
		}

		object arg;
		try
		{
		  bool keepgoing = parseSome();
		  arg = keepgoing ? true : false;
		}
		catch (SAXException ex)
		{
		  arg = ex;
		}
		catch (IOException ex)
		{
		  arg = ex;
		}
		catch (Exception ex)
		{
		  arg = new SAXException(ex);
		}
		return arg;
	  }

		// Private methods -- conveniences to hide the reflection details
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean parseSomeSetup(org.xml.sax.InputSource source) throws org.xml.sax.SAXException, java.io.IOException, IllegalAccessException, java.lang.reflect.InvocationTargetException, java.lang.InstantiationException
		private bool parseSomeSetup(InputSource source)
		{
			if (fConfigSetInput != null)
			{
				// Obtain input from SAX inputSource object, construct XNI version of
				// that object. Logic adapted from Xerces2.
				object[] parms1 = new object[] {source.PublicId,source.SystemId,null};
				object xmlsource = fConfigInputSourceCtor.newInstance(parms1);
				object[] parmsa = new object[] {source.ByteStream};
				fConfigSetByteStream.invoke(xmlsource,parmsa);
				parmsa[0] = source.CharacterStream;
				fConfigSetCharStream.invoke(xmlsource,parmsa);
				parmsa[0] = source.Encoding;
				fConfigSetEncoding.invoke(xmlsource,parmsa);

				// Bugzilla5272 patch suggested by Sandy Gao.
				// Has to be reflection to run with Xerces2
				// after compilation against Xerces1. or vice
				// versa, due to return type mismatches.
				object[] noparms = new object[0];
				fReset.invoke(fIncrementalParser,noparms);

				parmsa[0] = xmlsource;
				fConfigSetInput.invoke(fPullParserConfig,parmsa);

				// %REVIEW% Do first pull. Should we instead just return true?
				return parseSome();
			}
			else
			{
				object[] parm = new object[] {source};
				object ret = fParseSomeSetup.invoke(fIncrementalParser,parm);
				return ((bool?)ret).Value;
			}
		}
	//  Would null work???
		private static readonly object[] noparms = new object[0];
		private static readonly object[] parmsfalse = new object[] {false};
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean parseSome() throws org.xml.sax.SAXException, java.io.IOException, IllegalAccessException, java.lang.reflect.InvocationTargetException
		private bool parseSome()
		{
			// Take next parsing step, return false iff parsing complete:
			if (fConfigSetInput != null)
			{
				object ret = (bool?)(fConfigParse.invoke(fPullParserConfig,parmsfalse));
				return ((bool?)ret).Value;
			}
			else
			{
				object ret = fParseSome.invoke(fIncrementalParser,noparms);
				return ((bool?)ret).Value;
			}
		}


	  //================================================================
	  /// <summary>
	  /// Simple unit test. Attempt coroutine parsing of document indicated
	  /// by first argument (as a URI), report progress.
	  /// </summary>
	  public static void Main(string[] args)
	  {
		Console.WriteLine("Starting...");

		CoroutineManager co = new CoroutineManager();
		int appCoroutineID = co.co_joinCoroutineSet(-1);
		if (appCoroutineID == -1)
		{
		  Console.WriteLine("ERROR: Couldn't allocate coroutine number.\n");
		  return;
		}
		IncrementalSAXSource parser = createIncrementalSAXSource();

		// Use a serializer as our sample output
		org.apache.xml.serialize.XMLSerializer trace;
		trace = new org.apache.xml.serialize.XMLSerializer(System.out,null);
		parser.ContentHandler = trace;
		parser.LexicalHandler = trace;

		// Tell coroutine to begin parsing, run while parsing is in progress

		for (int arg = 0;arg < args.Length;++arg)
		{
		  try
		  {
			InputSource source = new InputSource(args[arg]);
			object result = null;
			bool more = true;
			parser.startParse(source);
			for (result = parser.deliverMoreNodes(more); result == true; result = parser.deliverMoreNodes(more))
			{
			  Console.WriteLine("\nSome parsing successful, trying more.\n");

			  // Special test: Terminate parsing early.
			  if (arg + 1 < args.Length && "!".Equals(args[arg + 1]))
			  {
				++arg;
				more = false;
			  }

			}

			if (result is bool? && ((bool?)result) == false)
			{
			  Console.WriteLine("\nParser ended (EOF or on request).\n");
			}
			else if (result == null)
			{
			  Console.WriteLine("\nUNEXPECTED: Parser says shut down prematurely.\n");
			}
			else if (result is Exception)
			{
			  throw new org.apache.xml.utils.WrappedRuntimeException((Exception)result);
			  //          System.out.println("\nParser threw exception:");
			  //          ((Exception)result).printStackTrace();
			}

		  }

		  catch (SAXException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
		}

	  }


	} // class IncrementalSAXSource_Xerces

}