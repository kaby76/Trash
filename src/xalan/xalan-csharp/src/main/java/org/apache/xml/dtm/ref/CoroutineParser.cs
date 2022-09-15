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
 * $Id: CoroutineParser.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xml.dtm.@ref
{
	using ContentHandler = org.xml.sax.ContentHandler;
	using InputSource = org.xml.sax.InputSource;
	using XMLReader = org.xml.sax.XMLReader;

	/// <summary>
	/// <para>CoroutineParser is an API for parser threads that operate as
	/// coroutines. See CoroutineSAXParser and CoroutineSAXParser_Xerces
	/// for examples.</para>
	/// 
	/// <para>&lt;grumble&gt; I'd like the interface to require a specific form
	/// for either the base constructor or a static factory method. Java
	/// doesn't allow us to specify either, so I'll just document them
	/// here:
	/// 
	/// <ul>
	/// <li>public CoroutineParser(CoroutineManager co, int appCoroutine);</li>
	/// <li>public CoroutineParser createCoroutineParser(CoroutineManager co, int appCoroutine);</li>
	/// </ul>
	/// 
	/// &lt;/grumble&gt;</para>
	/// </summary>
	/// @deprecated Since the ability to start a parse via the
	/// coroutine protocol was not being used and was complicating design.
	/// See <seealso cref="IncrementalSAXSource"/>.
	///  
	public interface CoroutineParser
	{

		/// <returns> the coroutine ID number for this CoroutineParser object.
		/// Note that this isn't useful unless you know which CoroutineManager
		/// you're talking to. Also note that the do...() methods encapsulate
		/// the common transactions with the CoroutineParser, so you shouldn't
		/// need this in most cases.
		///  </returns>
		int ParserCoroutineID {get;}

		/// <returns> the CoroutineManager for this CoroutineParser object.
		/// If you're using the do...() methods, applications should only
		/// need to talk to the CoroutineManager once, to obtain the
		/// application's Coroutine ID.
		///  </returns>
		CoroutineManager CoroutineManager {get;}

	  /// <summary>
	  /// Register a SAX-style content handler for us to output to </summary>
	  ContentHandler ContentHandler {set;}

	  /// <summary>
	  ///  Register a SAX-style lexical handler for us to output to
	  ///  Not all parsers support this...
	  /// 
	  /// %REVIEW% Not called setLexicalHandler because Xalan uses that name
	  /// internally, which causes subclassing nuisances. 
	  /// </summary>
	  org.xml.sax.ext.LexicalHandler LexHandler {set;}

	  /* The run() method is required in CoroutineParsers that run as
	   * threads (of course)... but it isn't part of our API, and
	   * shouldn't be declared here.
	   * */

	  //================================================================
	  /// <summary>
	  /// doParse() is a simple API which tells the coroutine parser
	  /// to begin reading from a file.  This is intended to be called from one
	  /// of our partner coroutines, and serves both to encapsulate the
	  /// communication protocol and to avoid having to explicitly use the
	  /// CoroutineParser's coroutine ID number.
	  /// 
	  /// %REVIEW% Can/should this unify with doMore? (if URI hasn't changed,
	  /// parse more from same file, else end and restart parsing...?
	  /// </summary>
	  /// <param name="source"> The InputSource to parse from. </param>
	  /// <param name="appCoroutine"> The coroutine ID number of the coroutine invoking
	  /// this method, so it can be resumed after the parser has responded to the
	  /// request. </param>
	  /// <returns> Boolean.TRUE if the CoroutineParser believes more data may be available
	  /// for further parsing. Boolean.FALSE if parsing ran to completion.
	  /// Exception if the parser objected for some reason.
	  ///  </returns>
	  object doParse(InputSource source, int appCoroutine);

	  /// <summary>
	  /// doMore() is a simple API which tells the coroutine parser
	  /// that we need more nodes.  This is intended to be called from one
	  /// of our partner coroutines, and serves both to encapsulate the
	  /// communication protocol and to avoid having to explicitly use the
	  /// CoroutineParser's coroutine ID number.
	  /// </summary>
	  /// <param name="parsemore"> If true, tells the incremental parser to generate
	  /// another chunk of output. If false, tells the parser that we're
	  /// satisfied and it can terminate parsing of this document. </param>
	  /// <param name="appCoroutine"> The coroutine ID number of the coroutine invoking
	  /// this method, so it can be resumed after the parser has responded to the
	  /// request. </param>
	  /// <returns> Boolean.TRUE if the CoroutineParser believes more data may be available
	  /// for further parsing. Boolean.FALSE if parsing ran to completion.
	  /// Exception if the parser objected for some reason.
	  ///  </returns>
	  object doMore(bool parsemore, int appCoroutine);

	  /// <summary>
	  /// doTerminate() is a simple API which tells the coroutine
	  /// parser to terminate itself.  This is intended to be called from
	  /// one of our partner coroutines, and serves both to encapsulate the
	  /// communication protocol and to avoid having to explicitly use the
	  /// CoroutineParser's coroutine ID number.
	  /// 
	  /// Returns only after the CoroutineParser has acknowledged the request.
	  /// </summary>
	  /// <param name="appCoroutine"> The coroutine ID number of the coroutine invoking
	  /// this method, so it can be resumed after the parser has responded to the
	  /// request.
	  ///  </param>
	  void doTerminate(int appCoroutine);

	  /// <summary>
	  /// Initialize the coroutine parser. Same parameters could be passed
	  /// in a non-default constructor, or by using using context ClassLoader
	  /// and newInstance and then calling init()
	  /// </summary>
	  void init(CoroutineManager co, int appCoroutineID, XMLReader parser);

	} // class CoroutineParser

}