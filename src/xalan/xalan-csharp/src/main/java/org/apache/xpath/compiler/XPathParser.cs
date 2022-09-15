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
 * $Id: XPathParser.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.compiler
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using XPathProcessorException = org.apache.xpath.XPathProcessorException;
	using XPathStylesheetDOM3Exception = org.apache.xpath.domapi.XPathStylesheetDOM3Exception;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XString = org.apache.xpath.objects.XString;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Tokenizes and parses XPath expressions. This should really be named
	/// XPathParserImpl, and may be renamed in the future.
	/// @xsl.usage general
	/// </summary>
	public class XPathParser
	{
		// %REVIEW% Is there a better way of doing this?
		// Upside is minimum object churn. Downside is that we don't have a useful
		// backtrace in the exception itself -- but we don't expect to need one.
		public const string CONTINUE_AFTER_FATAL_ERROR = "CONTINUE_AFTER_FATAL_ERROR";

	  /// <summary>
	  /// The XPath to be processed.
	  /// </summary>
	  private OpMap m_ops;

	  /// <summary>
	  /// The next token in the pattern.
	  /// </summary>
	  [NonSerialized]
	  internal string m_token;

	  /// <summary>
	  /// The first char in m_token, the theory being that this
	  /// is an optimization because we won't have to do charAt(0) as
	  /// often.
	  /// </summary>
	  [NonSerialized]
	  internal char m_tokenChar = (char)0;

	  /// <summary>
	  /// The position in the token queue is tracked by m_queueMark.
	  /// </summary>
	  internal int m_queueMark = 0;

	  /// <summary>
	  /// Results from checking FilterExpr syntax
	  /// </summary>
	  protected internal const int FILTER_MATCH_FAILED = 0;
	  protected internal const int FILTER_MATCH_PRIMARY = 1;
	  protected internal const int FILTER_MATCH_PREDICATES = 2;

	  /// <summary>
	  /// The parser constructor.
	  /// </summary>
	  public XPathParser(ErrorListener errorListener, javax.xml.transform.SourceLocator sourceLocator)
	  {
		m_errorListener = errorListener;
		m_sourceLocator = sourceLocator;
	  }

	  /// <summary>
	  /// The prefix resolver to map prefixes to namespaces in the OpMap.
	  /// </summary>
	  internal PrefixResolver m_namespaceContext;

	  /// <summary>
	  /// Given an string, init an XPath object for selections,
	  /// in order that a parse doesn't
	  /// have to be done each time the expression is evaluated.
	  /// </summary>
	  /// <param name="compiler"> The compiler object. </param>
	  /// <param name="expression"> A string conforming to the XPath grammar. </param>
	  /// <param name="namespaceContext"> An object that is able to resolve prefixes in
	  /// the XPath to namespaces.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void initXPath(Compiler compiler, String expression, org.apache.xml.utils.PrefixResolver namespaceContext) throws javax.xml.transform.TransformerException
	  public virtual void initXPath(Compiler compiler, string expression, PrefixResolver namespaceContext)
	  {

		m_ops = compiler;
		m_namespaceContext = namespaceContext;
		m_functionTable = compiler.FunctionTable;

		Lexer lexer = new Lexer(compiler, namespaceContext, this);

		lexer.tokenize(expression);

		m_ops.setOp(0,OpCodes.OP_XPATH);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH,2);


		// Patch for Christine's gripe. She wants her errorHandler to return from
		// a fatal error and continue trying to parse, rather than throwing an exception.
		// Without the patch, that put us into an endless loop.
		//
		// %REVIEW% Is there a better way of doing this?
		// %REVIEW% Are there any other cases which need the safety net?
		// 	(and if so do we care right now, or should we rewrite the XPath
		//	grammar engine and can fix it at that time?)
		try
		{

		  nextToken();
		  Expr();

		  if (null != m_token)
		  {
			string extraTokens = "";

			while (null != m_token)
			{
			  extraTokens += "'" + m_token + "'";

			  nextToken();

			  if (null != m_token)
			  {
				extraTokens += ", ";
			  }
			}

			error(XPATHErrorResources.ER_EXTRA_ILLEGAL_TOKENS, new object[]{extraTokens}); //"Extra illegal tokens: "+extraTokens);
		  }

		}
		catch (XPathProcessorException e)
		{
		  if (CONTINUE_AFTER_FATAL_ERROR.Equals(e.Message))
		  {
			// What I _want_ to do is null out this XPath.
			// I doubt this has the desired effect, but I'm not sure what else to do.
			// %REVIEW%!!!
			initXPath(compiler, "/..", namespaceContext);
		  }
		  else
		  {
			throw e;
		  }
		}

		compiler.shrink();
	  }

	  /// <summary>
	  /// Given an string, init an XPath object for pattern matches,
	  /// in order that a parse doesn't
	  /// have to be done each time the expression is evaluated. </summary>
	  /// <param name="compiler"> The XPath object to be initialized. </param>
	  /// <param name="expression"> A String representing the XPath. </param>
	  /// <param name="namespaceContext"> An object that is able to resolve prefixes in
	  /// the XPath to namespaces.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void initMatchPattern(Compiler compiler, String expression, org.apache.xml.utils.PrefixResolver namespaceContext) throws javax.xml.transform.TransformerException
	  public virtual void initMatchPattern(Compiler compiler, string expression, PrefixResolver namespaceContext)
	  {

		m_ops = compiler;
		m_namespaceContext = namespaceContext;
		m_functionTable = compiler.FunctionTable;

		Lexer lexer = new Lexer(compiler, namespaceContext, this);

		lexer.tokenize(expression);

		m_ops.setOp(0, OpCodes.OP_MATCHPATTERN);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, 2);

		nextToken();
		Pattern();

		if (null != m_token)
		{
		  string extraTokens = "";

		  while (null != m_token)
		  {
			extraTokens += "'" + m_token + "'";

			nextToken();

			if (null != m_token)
			{
			  extraTokens += ", ";
			}
		  }

		  error(XPATHErrorResources.ER_EXTRA_ILLEGAL_TOKENS, new object[]{extraTokens}); //"Extra illegal tokens: "+extraTokens);
		}

		// Terminate for safety.
		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ENDOP);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		m_ops.shrink();
	  }

	  /// <summary>
	  /// The error listener where syntax errors are to be sent.
	  /// </summary>
	  private ErrorListener m_errorListener;

	  /// <summary>
	  /// The source location of the XPath. </summary>
	  internal javax.xml.transform.SourceLocator m_sourceLocator;

	  /// <summary>
	  /// The table contains build-in functions and customized functions </summary>
	  private FunctionTable m_functionTable;

	  /// <summary>
	  /// Allow an application to register an error event handler, where syntax 
	  /// errors will be sent.  If the error listener is not set, syntax errors 
	  /// will be sent to System.err.
	  /// </summary>
	  /// <param name="handler"> Reference to error listener where syntax errors will be 
	  ///                sent. </param>
	  public virtual ErrorListener ErrorHandler
	  {
		  set
		  {
			m_errorListener = value;
		  }
	  }

	  /// <summary>
	  /// Return the current error listener.
	  /// </summary>
	  /// <returns> The error listener, which should not normally be null, but may be. </returns>
	  public virtual ErrorListener ErrorListener
	  {
		  get
		  {
			return m_errorListener;
		  }
	  }

	  /// <summary>
	  /// Check whether m_token matches the target string. 
	  /// </summary>
	  /// <param name="s"> A string reference or null.
	  /// </param>
	  /// <returns> If m_token is null, returns false (or true if s is also null), or 
	  /// return true if the current token matches the string, else false. </returns>
	  internal bool tokenIs(string s)
	  {
		return (!string.ReferenceEquals(m_token, null)) ? (m_token.Equals(s)) : (string.ReferenceEquals(s, null));
	  }

	  /// <summary>
	  /// Check whether m_tokenChar==c. 
	  /// </summary>
	  /// <param name="c"> A character to be tested.
	  /// </param>
	  /// <returns> If m_token is null, returns false, or return true if c matches 
	  ///         the current token. </returns>
	  internal bool tokenIs(char c)
	  {
		return (!string.ReferenceEquals(m_token, null)) ? (m_tokenChar == c) : false;
	  }

	  /// <summary>
	  /// Look ahead of the current token in order to
	  /// make a branching decision.
	  /// </summary>
	  /// <param name="c"> the character to be tested for. </param>
	  /// <param name="n"> number of tokens to look ahead.  Must be
	  /// greater than 1.
	  /// </param>
	  /// <returns> true if the next token matches the character argument. </returns>
	  internal bool lookahead(char c, int n)
	  {

		int pos = (m_queueMark + n);
		bool b;

		if ((pos <= m_ops.TokenQueueSize) && (pos > 0) && (m_ops.TokenQueueSize != 0))
		{
		  string tok = ((string) m_ops.m_tokenQueue.elementAt(pos - 1));

		  b = (tok.Length == 1) ? (tok[0] == c) : false;
		}
		else
		{
		  b = false;
		}

		return b;
	  }

	  /// <summary>
	  /// Look behind the first character of the current token in order to
	  /// make a branching decision.
	  /// </summary>
	  /// <param name="c"> the character to compare it to. </param>
	  /// <param name="n"> number of tokens to look behind.  Must be
	  /// greater than 1.  Note that the look behind terminates
	  /// at either the beginning of the string or on a '|'
	  /// character.  Because of this, this method should only
	  /// be used for pattern matching.
	  /// </param>
	  /// <returns> true if the token behind the current token matches the character 
	  ///         argument. </returns>
	  private bool lookbehind(char c, int n)
	  {

		bool isToken;
		int lookBehindPos = m_queueMark - (n + 1);

		if (lookBehindPos >= 0)
		{
		  string lookbehind = (string) m_ops.m_tokenQueue.elementAt(lookBehindPos);

		  if (lookbehind.Length == 1)
		  {
			char c0 = (string.ReferenceEquals(lookbehind, null)) ? '|' : lookbehind[0];

			isToken = (c0 == '|') ? false : (c0 == c);
		  }
		  else
		  {
			isToken = false;
		  }
		}
		else
		{
		  isToken = false;
		}

		return isToken;
	  }

	  /// <summary>
	  /// look behind the current token in order to
	  /// see if there is a useable token.
	  /// </summary>
	  /// <param name="n"> number of tokens to look behind.  Must be
	  /// greater than 1.  Note that the look behind terminates
	  /// at either the beginning of the string or on a '|'
	  /// character.  Because of this, this method should only
	  /// be used for pattern matching.
	  /// </param>
	  /// <returns> true if look behind has a token, false otherwise. </returns>
	  private bool lookbehindHasToken(int n)
	  {

		bool hasToken;

		if ((m_queueMark - n) > 0)
		{
		  string lookbehind = (string) m_ops.m_tokenQueue.elementAt(m_queueMark - (n - 1));
		  char c0 = (string.ReferenceEquals(lookbehind, null)) ? '|' : lookbehind[0];

		  hasToken = (c0 == '|') ? false : true;
		}
		else
		{
		  hasToken = false;
		}

		return hasToken;
	  }

	  /// <summary>
	  /// Look ahead of the current token in order to
	  /// make a branching decision.
	  /// </summary>
	  /// <param name="s"> the string to compare it to. </param>
	  /// <param name="n"> number of tokens to lookahead.  Must be
	  /// greater than 1.
	  /// </param>
	  /// <returns> true if the token behind the current token matches the string 
	  ///         argument. </returns>
	  private bool lookahead(string s, int n)
	  {

		bool isToken;

		if ((m_queueMark + n) <= m_ops.TokenQueueSize)
		{
		  string lookahead = (string) m_ops.m_tokenQueue.elementAt(m_queueMark + (n - 1));

		  isToken = (!string.ReferenceEquals(lookahead, null)) ? lookahead.Equals(s) : (string.ReferenceEquals(s, null));
		}
		else
		{
		  isToken = (null == s);
		}

		return isToken;
	  }

	  /// <summary>
	  /// Retrieve the next token from the command and
	  /// store it in m_token string.
	  /// </summary>
	  private void nextToken()
	  {

		if (m_queueMark < m_ops.TokenQueueSize)
		{
		  m_token = (string) m_ops.m_tokenQueue.elementAt(m_queueMark++);
		  m_tokenChar = m_token[0];
		}
		else
		{
		  m_token = null;
		  m_tokenChar = (char)0;
		}
	  }

	  /// <summary>
	  /// Retrieve a token relative to the current token.
	  /// </summary>
	  /// <param name="i"> Position relative to current token.
	  /// </param>
	  /// <returns> The string at the given index, or null if the index is out 
	  ///         of range. </returns>
	  private string getTokenRelative(int i)
	  {

		string tok;
		int relative = m_queueMark + i;

		if ((relative > 0) && (relative < m_ops.TokenQueueSize))
		{
		  tok = (string) m_ops.m_tokenQueue.elementAt(relative);
		}
		else
		{
		  tok = null;
		}

		return tok;
	  }

	  /// <summary>
	  /// Retrieve the previous token from the command and
	  /// store it in m_token string.
	  /// </summary>
	  private void prevToken()
	  {

		if (m_queueMark > 0)
		{
		  m_queueMark--;

		  m_token = (string) m_ops.m_tokenQueue.elementAt(m_queueMark);
		  m_tokenChar = m_token[0];
		}
		else
		{
		  m_token = null;
		  m_tokenChar = (char)0;
		}
	  }

	  /// <summary>
	  /// Consume an expected token, throwing an exception if it
	  /// isn't there.
	  /// </summary>
	  /// <param name="expected"> The string to be expected.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private final void consumeExpected(String expected) throws javax.xml.transform.TransformerException
	  private void consumeExpected(string expected)
	  {

		if (tokenIs(expected))
		{
		  nextToken();
		}
		else
		{
		  error(XPATHErrorResources.ER_EXPECTED_BUT_FOUND, new object[]{expected, m_token}); //"Expected "+expected+", but found: "+m_token);

		  // Patch for Christina's gripe. She wants her errorHandler to return from
		  // this error and continue trying to parse, rather than throwing an exception.
		  // Without the patch, that put us into an endless loop.
			throw new XPathProcessorException(CONTINUE_AFTER_FATAL_ERROR);
		}
	  }

	  /// <summary>
	  /// Consume an expected token, throwing an exception if it
	  /// isn't there.
	  /// </summary>
	  /// <param name="expected"> the character to be expected.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private final void consumeExpected(char expected) throws javax.xml.transform.TransformerException
	  private void consumeExpected(char expected)
	  {

		if (tokenIs(expected))
		{
		  nextToken();
		}
		else
		{
		  error(XPATHErrorResources.ER_EXPECTED_BUT_FOUND, new object[]{expected.ToString(), m_token}); //"Expected "+expected+", but found: "+m_token);

		  // Patch for Christina's gripe. She wants her errorHandler to return from
		  // this error and continue trying to parse, rather than throwing an exception.
		  // Without the patch, that put us into an endless loop.
			throw new XPathProcessorException(CONTINUE_AFTER_FATAL_ERROR);
		}
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// </summary>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void warn(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  internal virtual void warn(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHWarning(msg, args);
		ErrorListener ehandler = this.ErrorListener;

		if (null != ehandler)
		{
		  // TO DO: Need to get stylesheet Locator from here.
		  ehandler.warning(new TransformerException(fmsg, m_sourceLocator));
		}
		else
		{
		  // Should never happen.
		  Console.Error.WriteLine(fmsg);
		}
	  }

	  /// <summary>
	  /// Notify the user of an assertion error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="b">  If false, a runtime exception will be thrown. </param>
	  /// <param name="msg"> The assertion message, which should be informative.
	  /// </param>
	  /// <exception cref="RuntimeException"> if the b argument is false. </exception>
	  private void assertion(bool b, string msg)
	  {

		if (!b)
		{
		  string fMsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_INCORRECT_PROGRAMMER_ASSERTION, new object[]{msg});

		  throw new Exception(fMsg);
		}
	  }

	  /// <summary>
	  /// Notify the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void error(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  internal virtual void error(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);
		ErrorListener ehandler = this.ErrorListener;

		TransformerException te = new TransformerException(fmsg, m_sourceLocator);
		if (null != ehandler)
		{
		  // TO DO: Need to get stylesheet Locator from here.
		  ehandler.fatalError(te);
		}
		else
		{
		  // System.err.println(fmsg);
		  throw te;
		}
	  }

	  /// <summary>
	  /// This method is added to support DOM 3 XPath API.
	  /// <para>
	  /// This method is exactly like error(String, Object[]); except that
	  /// the underlying TransformerException is 
	  /// XpathStylesheetDOM3Exception (which extends TransformerException).
	  /// </para>
	  /// <para>
	  /// So older XPath code in Xalan is not affected by this. To older XPath code
	  /// the behavior of whether error() or errorForDOM3() is called because it is
	  /// always catching TransformerException objects and is oblivious to
	  /// the new subclass of XPathStylesheetDOM3Exception. Older XPath code 
	  /// runs as before.
	  /// </para>
	  /// <para>
	  /// However, newer DOM3 XPath code upon catching a TransformerException can
	  /// can check if the exception is an instance of XPathStylesheetDOM3Exception
	  /// and take appropriate action.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void errorForDOM3(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  internal virtual void errorForDOM3(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);
		ErrorListener ehandler = this.ErrorListener;

		TransformerException te = new XPathStylesheetDOM3Exception(fmsg, m_sourceLocator);
		if (null != ehandler)
		{
		  // TO DO: Need to get stylesheet Locator from here.
		  ehandler.fatalError(te);
		}
		else
		{
		  // System.err.println(fmsg);
		  throw te;
		}
	  }
	  /// <summary>
	  /// Dump the remaining token queue.
	  /// Thanks to Craig for this.
	  /// </summary>
	  /// <returns> A dump of the remaining token queue, which may be appended to 
	  ///         an error message. </returns>
	  protected internal virtual string dumpRemainingTokenQueue()
	  {

		int q = m_queueMark;
		string returnMsg;

		if (q < m_ops.TokenQueueSize)
		{
		  string msg = "\n Remaining tokens: (";

		  while (q < m_ops.TokenQueueSize)
		  {
			string t = (string) m_ops.m_tokenQueue.elementAt(q++);

			msg += (" '" + t + "'");
		  }

		  returnMsg = msg + ")";
		}
		else
		{
		  returnMsg = "";
		}

		return returnMsg;
	  }

	  /// <summary>
	  /// Given a string, return the corresponding function token.
	  /// </summary>
	  /// <param name="key"> A local name of a function.
	  /// </param>
	  /// <returns>   The function ID, which may correspond to one of the FUNC_XXX 
	  ///    values found in <seealso cref="org.apache.xpath.compiler.FunctionTable"/>, but may 
	  ///    be a value installed by an external module. </returns>
	  internal int getFunctionToken(string key)
	  {

		int tok;

		object id;

		try
		{
		  // These are nodetests, xpathparser treats them as functions when parsing
		  // a FilterExpr. 
		  id = Keywords.lookupNodeTest(key);
		  if (null == id)
		  {
			  id = m_functionTable.getFunctionID(key);
		  }
		  tok = ((int?) id).Value;
		}
		catch (System.NullReferenceException)
		{
		  tok = -1;
		}
		catch (System.InvalidCastException)
		{
		  tok = -1;
		}

		return tok;
	  }

	  /// <summary>
	  /// Insert room for operation.  This will NOT set
	  /// the length value of the operation, but will update
	  /// the length value for the total expression.
	  /// </summary>
	  /// <param name="pos"> The position where the op is to be inserted. </param>
	  /// <param name="length"> The length of the operation space in the op map. </param>
	  /// <param name="op"> The op code to the inserted. </param>
	  internal virtual void insertOp(int pos, int length, int op)
	  {

		int totalLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		for (int i = totalLen - 1; i >= pos; i--)
		{
		  m_ops.setOp(i + length, m_ops.getOp(i));
		}

		m_ops.setOp(pos,op);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH,totalLen + length);
	  }

	  /// <summary>
	  /// Insert room for operation.  This WILL set
	  /// the length value of the operation, and will update
	  /// the length value for the total expression.
	  /// </summary>
	  /// <param name="length"> The length of the operation. </param>
	  /// <param name="op"> The op code to the inserted. </param>
	  internal virtual void appendOp(int length, int op)
	  {

		int totalLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		m_ops.setOp(totalLen, op);
		m_ops.setOp(totalLen + OpMap.MAPINDEX_LENGTH, length);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, totalLen + length);
	  }

	  // ============= EXPRESSIONS FUNCTIONS =================

	  /// 
	  /// 
	  /// <summary>
	  /// Expr  ::=  OrExpr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Expr() throws javax.xml.transform.TransformerException
	  protected internal virtual void Expr()
	  {
		OrExpr();
	  }

	  /// 
	  /// 
	  /// <summary>
	  /// OrExpr  ::=  AndExpr
	  /// | OrExpr 'or' AndExpr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void OrExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void OrExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		AndExpr();

		if ((null != m_token) && tokenIs("or"))
		{
		  nextToken();
		  insertOp(opPos, 2, OpCodes.OP_OR);
		  OrExpr();

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
		}
	  }

	  /// 
	  /// 
	  /// <summary>
	  /// AndExpr  ::=  EqualityExpr
	  /// | AndExpr 'and' EqualityExpr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void AndExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void AndExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		EqualityExpr(-1);

		if ((null != m_token) && tokenIs("and"))
		{
		  nextToken();
		  insertOp(opPos, 2, OpCodes.OP_AND);
		  AndExpr();

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
		}
	  }

	  /// 
	  /// <summary>
	  /// @returns an Object which is either a String, a Number, a Boolean, or a vector
	  /// of nodes.
	  /// 
	  /// EqualityExpr  ::=  RelationalExpr
	  /// | EqualityExpr '=' RelationalExpr
	  /// 
	  /// </summary>
	  /// <param name="addPos"> Position where expression is to be added, or -1 for append.
	  /// </param>
	  /// <returns> the position at the end of the equality expression.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int EqualityExpr(int addPos) throws javax.xml.transform.TransformerException
	  protected internal virtual int EqualityExpr(int addPos)
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		if (-1 == addPos)
		{
		  addPos = opPos;
		}

		RelationalExpr(-1);

		if (null != m_token)
		{
		  if (tokenIs('!') && lookahead('=', 1))
		  {
			nextToken();
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_NOTEQUALS);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = EqualityExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		  else if (tokenIs('='))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_EQUALS);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = EqualityExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		}

		return addPos;
	  }

	  /// <summary>
	  /// .
	  /// @returns an Object which is either a String, a Number, a Boolean, or a vector
	  /// of nodes.
	  /// 
	  /// RelationalExpr  ::=  AdditiveExpr
	  /// | RelationalExpr '<' AdditiveExpr
	  /// | RelationalExpr '>' AdditiveExpr
	  /// | RelationalExpr '<=' AdditiveExpr
	  /// | RelationalExpr '>=' AdditiveExpr
	  /// 
	  /// </summary>
	  /// <param name="addPos"> Position where expression is to be added, or -1 for append.
	  /// </param>
	  /// <returns> the position at the end of the relational expression.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int RelationalExpr(int addPos) throws javax.xml.transform.TransformerException
	  protected internal virtual int RelationalExpr(int addPos)
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		if (-1 == addPos)
		{
		  addPos = opPos;
		}

		AdditiveExpr(-1);

		if (null != m_token)
		{
		  if (tokenIs('<'))
		  {
			nextToken();

			if (tokenIs('='))
			{
			  nextToken();
			  insertOp(addPos, 2, OpCodes.OP_LTE);
			}
			else
			{
			  insertOp(addPos, 2, OpCodes.OP_LT);
			}

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = RelationalExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		  else if (tokenIs('>'))
		  {
			nextToken();

			if (tokenIs('='))
			{
			  nextToken();
			  insertOp(addPos, 2, OpCodes.OP_GTE);
			}
			else
			{
			  insertOp(addPos, 2, OpCodes.OP_GT);
			}

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = RelationalExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		}

		return addPos;
	  }

	  /// <summary>
	  /// This has to handle construction of the operations so that they are evaluated
	  /// in pre-fix order.  So, for 9+7-6, instead of |+|9|-|7|6|, this needs to be
	  /// evaluated as |-|+|9|7|6|.
	  /// 
	  /// AdditiveExpr  ::=  MultiplicativeExpr
	  /// | AdditiveExpr '+' MultiplicativeExpr
	  /// | AdditiveExpr '-' MultiplicativeExpr
	  /// 
	  /// </summary>
	  /// <param name="addPos"> Position where expression is to be added, or -1 for append.
	  /// </param>
	  /// <returns> the position at the end of the equality expression.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int AdditiveExpr(int addPos) throws javax.xml.transform.TransformerException
	  protected internal virtual int AdditiveExpr(int addPos)
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		if (-1 == addPos)
		{
		  addPos = opPos;
		}

		MultiplicativeExpr(-1);

		if (null != m_token)
		{
		  if (tokenIs('+'))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_PLUS);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = AdditiveExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		  else if (tokenIs('-'))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_MINUS);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = AdditiveExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		}

		return addPos;
	  }

	  /// <summary>
	  /// This has to handle construction of the operations so that they are evaluated
	  /// in pre-fix order.  So, for 9+7-6, instead of |+|9|-|7|6|, this needs to be
	  /// evaluated as |-|+|9|7|6|.
	  /// 
	  /// MultiplicativeExpr  ::=  UnaryExpr
	  /// | MultiplicativeExpr MultiplyOperator UnaryExpr
	  /// | MultiplicativeExpr 'div' UnaryExpr
	  /// | MultiplicativeExpr 'mod' UnaryExpr
	  /// | MultiplicativeExpr 'quo' UnaryExpr
	  /// </summary>
	  /// <param name="addPos"> Position where expression is to be added, or -1 for append.
	  /// </param>
	  /// <returns> the position at the end of the equality expression.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int MultiplicativeExpr(int addPos) throws javax.xml.transform.TransformerException
	  protected internal virtual int MultiplicativeExpr(int addPos)
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		if (-1 == addPos)
		{
		  addPos = opPos;
		}

		UnaryExpr();

		if (null != m_token)
		{
		  if (tokenIs('*'))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_MULT);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = MultiplicativeExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		  else if (tokenIs("div"))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_DIV);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = MultiplicativeExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		  else if (tokenIs("mod"))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_MOD);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = MultiplicativeExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		  else if (tokenIs("quo"))
		  {
			nextToken();
			insertOp(addPos, 2, OpCodes.OP_QUO);

			int opPlusLeftHandLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - addPos;

			addPos = MultiplicativeExpr(addPos);
			m_ops.setOp(addPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(addPos + opPlusLeftHandLen + 1) + opPlusLeftHandLen);
			addPos += 2;
		  }
		}

		return addPos;
	  }

	  /// 
	  /// <summary>
	  /// UnaryExpr  ::=  UnionExpr
	  /// | '-' UnaryExpr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void UnaryExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void UnaryExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
		bool isNeg = false;

		if (m_tokenChar == '-')
		{
		  nextToken();
		  appendOp(2, OpCodes.OP_NEG);

		  isNeg = true;
		}

		UnionExpr();

		if (isNeg)
		{
		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
		}
	  }

	  /// 
	  /// <summary>
	  /// StringExpr  ::=  Expr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void StringExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void StringExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		appendOp(2, OpCodes.OP_STRING);
		Expr();

		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// 
	  /// 
	  /// <summary>
	  /// StringExpr  ::=  Expr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void BooleanExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void BooleanExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		appendOp(2, OpCodes.OP_BOOL);
		Expr();

		int opLen = m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos;

		if (opLen == 2)
		{
		  error(XPATHErrorResources.ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, null); //"boolean(...) argument is no longer optional with 19990709 XPath draft.");
		}

		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, opLen);
	  }

	  /// 
	  /// 
	  /// <summary>
	  /// NumberExpr  ::=  Expr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void NumberExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void NumberExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		appendOp(2, OpCodes.OP_NUMBER);
		Expr();

		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// <summary>
	  /// The context of the right hand side expressions is the context of the
	  /// left hand side expression. The results of the right hand side expressions
	  /// are node sets. The result of the left hand side UnionExpr is the union
	  /// of the results of the right hand side expressions.
	  /// 
	  /// 
	  /// UnionExpr    ::=    PathExpr
	  /// | UnionExpr '|' PathExpr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void UnionExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void UnionExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
		bool continueOrLoop = true;
		bool foundUnion = false;

		do
		{
		  PathExpr();

		  if (tokenIs('|'))
		  {
			if (false == foundUnion)
			{
			  foundUnion = true;

			  insertOp(opPos, 2, OpCodes.OP_UNION);
			}

			nextToken();
		  }
		  else
		  {
			break;
		  }

		  // this.m_testForDocOrder = true;
		} while (continueOrLoop);

		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// <summary>
	  /// PathExpr  ::=  LocationPath
	  /// | FilterExpr
	  /// | FilterExpr '/' RelativeLocationPath
	  /// | FilterExpr '//' RelativeLocationPath
	  /// </summary>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void PathExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void PathExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		int filterExprMatch = FilterExpr();

		if (filterExprMatch != FILTER_MATCH_FAILED)
		{
		  // If FilterExpr had Predicates, a OP_LOCATIONPATH opcode would already
		  // have been inserted.
		  bool locationPathStarted = (filterExprMatch == FILTER_MATCH_PREDICATES);

		  if (tokenIs('/'))
		  {
			nextToken();

			if (!locationPathStarted)
			{
			  // int locationPathOpPos = opPos;
			  insertOp(opPos, 2, OpCodes.OP_LOCATIONPATH);

			  locationPathStarted = true;
			}

			if (!RelativeLocationPath())
			{
			  // "Relative location path expected following '/' or '//'"
			  error(XPATHErrorResources.ER_EXPECTED_REL_LOC_PATH, null);
			}

		  }

		  // Terminate for safety.
		  if (locationPathStarted)
		  {
			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ENDOP);
			m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
			m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
		  }
		}
		else
		{
		  LocationPath();
		}
	  }

	  /// 
	  /// 
	  /// <summary>
	  /// FilterExpr  ::=  PrimaryExpr
	  /// | FilterExpr Predicate
	  /// </summary>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <returns>  FILTER_MATCH_PREDICATES, if this method successfully matched a
	  ///          FilterExpr with one or more Predicates;
	  ///          FILTER_MATCH_PRIMARY, if this method successfully matched a
	  ///          FilterExpr that was just a PrimaryExpr; or
	  ///          FILTER_MATCH_FAILED, if this method did not match a FilterExpr
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int FilterExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual int FilterExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		int filterMatch;

		if (PrimaryExpr())
		{
		  if (tokenIs('['))
		  {

			// int locationPathOpPos = opPos;
			insertOp(opPos, 2, OpCodes.OP_LOCATIONPATH);

			while (tokenIs('['))
			{
			  Predicate();
			}

			filterMatch = FILTER_MATCH_PREDICATES;
		  }
		  else
		  {
			filterMatch = FILTER_MATCH_PRIMARY;
		  }
		}
		else
		{
		  filterMatch = FILTER_MATCH_FAILED;
		}

		return filterMatch;

		/*
		 * if(tokenIs('['))
		 * {
		 *   Predicate();
		 *   m_ops.m_opMap[opPos + OpMap.MAPINDEX_LENGTH] = m_ops.m_opMap[OpMap.MAPINDEX_LENGTH] - opPos;
		 * }
		 */
	  }

	  /// 
	  /// <summary>
	  /// PrimaryExpr  ::=  VariableReference
	  /// | '(' Expr ')'
	  /// | Literal
	  /// | Number
	  /// | FunctionCall
	  /// </summary>
	  /// <returns> true if this method successfully matched a PrimaryExpr
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected boolean PrimaryExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual bool PrimaryExpr()
	  {

		bool matchFound;
		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		if ((m_tokenChar == '\'') || (m_tokenChar == '"'))
		{
		  appendOp(2, OpCodes.OP_LITERAL);
		  Literal();

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		  matchFound = true;
		}
		else if (m_tokenChar == '$')
		{
		  nextToken(); // consume '$'
		  appendOp(2, OpCodes.OP_VARIABLE);
		  QName();

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		  matchFound = true;
		}
		else if (m_tokenChar == '(')
		{
		  nextToken();
		  appendOp(2, OpCodes.OP_GROUP);
		  Expr();
		  consumeExpected(')');

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		  matchFound = true;
		}
		else if ((null != m_token) && ((('.' == m_tokenChar) && (m_token.Length > 1) && char.IsDigit(m_token[1])) || char.IsDigit(m_tokenChar)))
		{
		  appendOp(2, OpCodes.OP_NUMBERLIT);
		  Number();

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		  matchFound = true;
		}
		else if (lookahead('(', 1) || (lookahead(':', 1) && lookahead('(', 3)))
		{
		  matchFound = FunctionCall();
		}
		else
		{
		  matchFound = false;
		}

		return matchFound;
	  }

	  /// 
	  /// <summary>
	  /// Argument    ::=    Expr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Argument() throws javax.xml.transform.TransformerException
	  protected internal virtual void Argument()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		appendOp(2, OpCodes.OP_ARGUMENT);
		Expr();

		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// 
	  /// <summary>
	  /// FunctionCall    ::=    FunctionName '(' ( Argument ( ',' Argument)*)? ')'
	  /// </summary>
	  /// <returns> true if, and only if, a FunctionCall was matched
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected boolean FunctionCall() throws javax.xml.transform.TransformerException
	  protected internal virtual bool FunctionCall()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		if (lookahead(':', 1))
		{
		  appendOp(4, OpCodes.OP_EXTFUNCTION);

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH + 1, m_queueMark - 1);

		  nextToken();
		  consumeExpected(':');

		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH + 2, m_queueMark - 1);

		  nextToken();
		}
		else
		{
		  int funcTok = getFunctionToken(m_token);

		  if (-1 == funcTok)
		  {
			error(XPATHErrorResources.ER_COULDNOT_FIND_FUNCTION, new object[]{m_token}); //"Could not find function: "+m_token+"()");
		  }

		  switch (funcTok)
		  {
		  case OpCodes.NODETYPE_PI :
		  case OpCodes.NODETYPE_COMMENT :
		  case OpCodes.NODETYPE_TEXT :
		  case OpCodes.NODETYPE_NODE :
			// Node type tests look like function calls, but they're not
			return false;
		  default :
			appendOp(3, OpCodes.OP_FUNCTION);

			m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH + 1, funcTok);
		break;
		  }

		  nextToken();
		}

		consumeExpected('(');

		while (!tokenIs(')') && !string.ReferenceEquals(m_token, null))
		{
		  if (tokenIs(','))
		  {
			error(XPATHErrorResources.ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, null); //"Found ',' but no preceding argument!");
		  }

		  Argument();

		  if (!tokenIs(')'))
		  {
			consumeExpected(',');

			if (tokenIs(')'))
			{
			  error(XPATHErrorResources.ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, null); //"Found ',' but no following argument!");
			}
		  }
		}

		consumeExpected(')');

		// Terminate for safety.
		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ENDOP);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH,m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		return true;
	  }

	  // ============= GRAMMAR FUNCTIONS =================

	  /// 
	  /// <summary>
	  /// LocationPath ::= RelativeLocationPath
	  /// | AbsoluteLocationPath
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void LocationPath() throws javax.xml.transform.TransformerException
	  protected internal virtual void LocationPath()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		// int locationPathOpPos = opPos;
		appendOp(2, OpCodes.OP_LOCATIONPATH);

		bool seenSlash = tokenIs('/');

		if (seenSlash)
		{
		  appendOp(4, OpCodes.FROM_ROOT);

		  // Tell how long the step is without the predicate
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 2, 4);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 1, OpCodes.NODETYPE_ROOT);

		  nextToken();
		}
		else if (string.ReferenceEquals(m_token, null))
		{
		  error(XPATHErrorResources.ER_EXPECTED_LOC_PATH_AT_END_EXPR, null);
		}

		if (!string.ReferenceEquals(m_token, null))
		{
		  if (!RelativeLocationPath() && !seenSlash)
		  {
			// Neither a '/' nor a RelativeLocationPath - i.e., matched nothing
			// "Location path expected, but found "+m_token+" was encountered."
			error(XPATHErrorResources.ER_EXPECTED_LOC_PATH, new object [] {m_token});
		  }
		}

		// Terminate for safety.
		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ENDOP);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH,m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// 
	  /// <summary>
	  /// RelativeLocationPath ::= Step
	  /// | RelativeLocationPath '/' Step
	  /// | AbbreviatedRelativeLocationPath
	  /// 
	  /// @returns true if, and only if, a RelativeLocationPath was matched
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected boolean RelativeLocationPath() throws javax.xml.transform.TransformerException
	  protected internal virtual bool RelativeLocationPath()
	  {
		if (!Step())
		{
		  return false;
		}

		while (tokenIs('/'))
		{
		  nextToken();

		  if (!Step())
		  {
			// RelativeLocationPath can't end with a trailing '/'
			// "Location step expected following '/' or '//'"
			error(XPATHErrorResources.ER_EXPECTED_LOC_STEP, null);
		  }
		}

		return true;
	  }

	  /// 
	  /// <summary>
	  /// Step    ::=    Basis Predicate
	  /// | AbbreviatedStep
	  /// 
	  /// @returns false if step was empty (or only a '/'); true, otherwise
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected boolean Step() throws javax.xml.transform.TransformerException
	  protected internal virtual bool Step()
	  {
		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		bool doubleSlash = tokenIs('/');

		// At most a single '/' before each Step is consumed by caller; if the
		// first thing is a '/', that means we had '//' and the Step must not
		// be empty.
		if (doubleSlash)
		{
		  nextToken();

		  appendOp(2, OpCodes.FROM_DESCENDANTS_OR_SELF);

		  // Have to fix up for patterns such as '//@foo' or '//attribute::foo',
		  // which translate to 'descendant-or-self::node()/attribute::foo'.
		  // notice I leave the '/' on the queue, so the next will be processed
		  // by a regular step pattern.

		  // Make room for telling how long the step is without the predicate
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH,m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.NODETYPE_NODE);
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH,m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  // Tell how long the step is without the predicate
		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH + 1, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		  // Tell how long the step is with the predicate
		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		  opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
		}

		if (tokenIs("."))
		{
		  nextToken();

		  if (tokenIs('['))
		  {
			error(XPATHErrorResources.ER_PREDICATE_ILLEGAL_SYNTAX, null); //"'..[predicate]' or '.[predicate]' is illegal syntax.  Use 'self::node()[predicate]' instead.");
		  }

		  appendOp(4, OpCodes.FROM_SELF);

		  // Tell how long the step is without the predicate
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 2,4);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 1, OpCodes.NODETYPE_NODE);
		}
		else if (tokenIs(".."))
		{
		  nextToken();
		  appendOp(4, OpCodes.FROM_PARENT);

		  // Tell how long the step is without the predicate
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 2,4);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 1, OpCodes.NODETYPE_NODE);
		}

		// There is probably a better way to test for this 
		// transition... but it gets real hairy if you try 
		// to do it in basis().
		else if (tokenIs('*') || tokenIs('@') || tokenIs('_') || (!string.ReferenceEquals(m_token, null) && char.IsLetter(m_token[0])))
		{
		  Basis();

		  while (tokenIs('['))
		  {
			Predicate();
		  }

		  // Tell how long the entire step is.
		  m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
		}
		else
		{
		  // No Step matched - that's an error if previous thing was a '//'
		  if (doubleSlash)
		  {
			// "Location step expected following '/' or '//'"
			error(XPATHErrorResources.ER_EXPECTED_LOC_STEP, null);
		  }

		  return false;
		}

		return true;
	  }

	  /// 
	  /// <summary>
	  /// Basis    ::=    AxisName '::' NodeTest
	  /// | AbbreviatedBasis
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Basis() throws javax.xml.transform.TransformerException
	  protected internal virtual void Basis()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
		int axesType;

		// The next blocks guarantee that a FROM_XXX will be added.
		if (lookahead("::", 1))
		{
		  axesType = AxisName();

		  nextToken();
		  nextToken();
		}
		else if (tokenIs('@'))
		{
		  axesType = OpCodes.FROM_ATTRIBUTES;

		  appendOp(2, axesType);
		  nextToken();
		}
		else
		{
		  axesType = OpCodes.FROM_CHILDREN;

		  appendOp(2, axesType);
		}

		// Make room for telling how long the step is without the predicate
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		NodeTest(axesType);

		// Tell how long the step is without the predicate
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH + 1, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// 
	  /// <summary>
	  /// Basis    ::=    AxisName '::' NodeTest
	  /// | AbbreviatedBasis
	  /// </summary>
	  /// <returns> FROM_XXX axes type, found in <seealso cref="org.apache.xpath.compiler.Keywords"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int AxisName() throws javax.xml.transform.TransformerException
	  protected internal virtual int AxisName()
	  {

		object val = Keywords.getAxisName(m_token);

		if (null == val)
		{
		  error(XPATHErrorResources.ER_ILLEGAL_AXIS_NAME, new object[]{m_token}); //"illegal axis name: "+m_token);
		}

		int axesType = ((int?) val).Value;

		appendOp(2, axesType);

		return axesType;
	  }

	  /// 
	  /// <summary>
	  /// NodeTest    ::=    WildcardName
	  /// | NodeType '(' ')'
	  /// | 'processing-instruction' '(' Literal ')'
	  /// </summary>
	  /// <param name="axesType"> FROM_XXX axes type, found in <seealso cref="org.apache.xpath.compiler.Keywords"/>.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void NodeTest(int axesType) throws javax.xml.transform.TransformerException
	  protected internal virtual void NodeTest(int axesType)
	  {

		if (lookahead('(', 1))
		{
		  object nodeTestOp = Keywords.getNodeType(m_token);

		  if (null == nodeTestOp)
		  {
			error(XPATHErrorResources.ER_UNKNOWN_NODETYPE, new object[]{m_token}); //"Unknown nodetype: "+m_token);
		  }
		  else
		  {
			nextToken();

			int nt = ((int?) nodeTestOp).Value;

			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), nt);
			m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

			consumeExpected('(');

			if (OpCodes.NODETYPE_PI == nt)
			{
			  if (!tokenIs(')'))
			  {
				Literal();
			  }
			}

			consumeExpected(')');
		  }
		}
		else
		{

		  // Assume name of attribute or element.
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.NODENAME);
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  if (lookahead(':', 1))
		  {
			if (tokenIs('*'))
			{
			  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ELEMWILDCARD);
			}
			else
			{
			  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), m_queueMark - 1);

			  // Minimalist check for an NCName - just check first character
			  // to distinguish from other possible tokens
			  if (!char.IsLetter(m_tokenChar) && !tokenIs('_'))
			  {
				// "Node test that matches either NCName:* or QName was expected."
				error(XPATHErrorResources.ER_EXPECTED_NODE_TEST, null);
			  }
			}

			nextToken();
			consumeExpected(':');
		  }
		  else
		  {
			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.EMPTY);
		  }

		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  if (tokenIs('*'))
		  {
			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ELEMWILDCARD);
		  }
		  else
		  {
			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), m_queueMark - 1);

			// Minimalist check for an NCName - just check first character
			// to distinguish from other possible tokens
			if (!char.IsLetter(m_tokenChar) && !tokenIs('_'))
			{
			  // "Node test that matches either NCName:* or QName was expected."
			  error(XPATHErrorResources.ER_EXPECTED_NODE_TEST, null);
			}
		  }

		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  nextToken();
		}
	  }

	  /// 
	  /// <summary>
	  /// Predicate ::= '[' PredicateExpr ']'
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Predicate() throws javax.xml.transform.TransformerException
	  protected internal virtual void Predicate()
	  {

		if (tokenIs('['))
		{
		  nextToken();
		  PredicateExpr();
		  consumeExpected(']');
		}
	  }

	  /// 
	  /// <summary>
	  /// PredicateExpr ::= Expr
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void PredicateExpr() throws javax.xml.transform.TransformerException
	  protected internal virtual void PredicateExpr()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		appendOp(2, OpCodes.OP_PREDICATE);
		Expr();

		// Terminate for safety.
		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ENDOP);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// <summary>
	  /// QName ::=  (Prefix ':')? LocalPart
	  /// Prefix ::=  NCName
	  /// LocalPart ::=  NCName
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void QName() throws javax.xml.transform.TransformerException
	  protected internal virtual void QName()
	  {
		// Namespace
		if (lookahead(':', 1))
		{
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), m_queueMark - 1);
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  nextToken();
		  consumeExpected(':');
		}
		else
		{
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.EMPTY);
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
		}

		// Local name
		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), m_queueMark - 1);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		nextToken();
	  }

	  /// <summary>
	  /// NCName ::=  (Letter | '_') (NCNameChar)
	  /// NCNameChar ::=  Letter | Digit | '.' | '-' | '_' | CombiningChar | Extender
	  /// </summary>
	  protected internal virtual void NCName()
	  {

		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), m_queueMark - 1);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		nextToken();
	  }

	  /// <summary>
	  /// The value of the Literal is the sequence of characters inside
	  /// the " or ' characters>.
	  /// 
	  /// Literal  ::=  '"' [^"]* '"'
	  /// | "'" [^']* "'"
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Literal() throws javax.xml.transform.TransformerException
	  protected internal virtual void Literal()
	  {

		int last = m_token.Length - 1;
		char c0 = m_tokenChar;
		char cX = m_token[last];

		if (((c0 == '\"') && (cX == '\"')) || ((c0 == '\'') && (cX == '\'')))
		{

		  // Mutate the token to remove the quotes and have the XString object
		  // already made.
		  int tokenQueuePos = m_queueMark - 1;

		  m_ops.m_tokenQueue.setElementAt(null,tokenQueuePos);

		  object obj = new XString(m_token.Substring(1, last - 1));

		  m_ops.m_tokenQueue.setElementAt(obj,tokenQueuePos);

		  // lit = m_token.substring(1, last);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), tokenQueuePos);
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  nextToken();
		}
		else
		{
		  error(XPATHErrorResources.ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, new object[]{m_token}); //"Pattern literal ("+m_token+") needs to be quoted!");
		}
	  }

	  /// 
	  /// <summary>
	  /// Number ::= [0-9]+('.'[0-9]+)? | '.'[0-9]+
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Number() throws javax.xml.transform.TransformerException
	  protected internal virtual void Number()
	  {

		if (null != m_token)
		{

		  // Mutate the token to remove the quotes and have the XNumber object
		  // already made.
		  double num;

		  try
		  {
			  // XPath 1.0 does not support number in exp notation
			  if ((m_token.IndexOf('e') > -1) || (m_token.IndexOf('E') > -1))
			  {
				  throw new System.FormatException();
			  }
			num = Convert.ToDouble(m_token);
		  }
		  catch (System.FormatException)
		  {
			num = 0.0; // to shut up compiler.

			error(XPATHErrorResources.ER_COULDNOT_BE_FORMATTED_TO_NUMBER, new object[]{m_token}); //m_token+" could not be formatted to a number!");
		  }

		  m_ops.m_tokenQueue.setElementAt(new XNumber(num),m_queueMark - 1);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), m_queueMark - 1);
		  m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		  nextToken();
		}
	  }

	  // ============= PATTERN FUNCTIONS =================

	  /// 
	  /// <summary>
	  /// Pattern  ::=  LocationPathPattern
	  /// | Pattern '|' LocationPathPattern
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void Pattern() throws javax.xml.transform.TransformerException
	  protected internal virtual void Pattern()
	  {

		while (true)
		{
		  LocationPathPattern();

		  if (tokenIs('|'))
		  {
			nextToken();
		  }
		  else
		  {
			break;
		  }
		}
	  }

	  /// 
	  /// 
	  /// <summary>
	  /// LocationPathPattern  ::=  '/' RelativePathPattern?
	  /// | IdKeyPattern (('/' | '//') RelativePathPattern)?
	  /// | '//'? RelativePathPattern
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void LocationPathPattern() throws javax.xml.transform.TransformerException
	  protected internal virtual void LocationPathPattern()
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);

		const int RELATIVE_PATH_NOT_PERMITTED = 0;
		const int RELATIVE_PATH_PERMITTED = 1;
		const int RELATIVE_PATH_REQUIRED = 2;

		int relativePathStatus = RELATIVE_PATH_NOT_PERMITTED;

		appendOp(2, OpCodes.OP_LOCATIONPATHPATTERN);

		if (lookahead('(', 1) && (tokenIs(Keywords.FUNC_ID_STRING) || tokenIs(Keywords.FUNC_KEY_STRING)))
		{
		  IdKeyPattern();

		  if (tokenIs('/'))
		  {
			nextToken();

			if (tokenIs('/'))
			{
			  appendOp(4, OpCodes.MATCH_ANY_ANCESTOR);

			  nextToken();
			}
			else
			{
			  appendOp(4, OpCodes.MATCH_IMMEDIATE_ANCESTOR);
			}

			// Tell how long the step is without the predicate
			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 2, 4);
			m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 1, OpCodes.NODETYPE_FUNCTEST);

			relativePathStatus = RELATIVE_PATH_REQUIRED;
		  }
		}
		else if (tokenIs('/'))
		{
		  if (lookahead('/', 1))
		  {
			appendOp(4, OpCodes.MATCH_ANY_ANCESTOR);

			// Added this to fix bug reported by Myriam for match="//x/a"
			// patterns.  If you don't do this, the 'x' step will think it's part
			// of a '//' pattern, and so will cause 'a' to be matched when it has
			// any ancestor that is 'x'.
			nextToken();

			relativePathStatus = RELATIVE_PATH_REQUIRED;
		  }
		  else
		  {
			appendOp(4, OpCodes.FROM_ROOT);

			relativePathStatus = RELATIVE_PATH_PERMITTED;
		  }


		  // Tell how long the step is without the predicate
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 2, 4);
		  m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH) - 1, OpCodes.NODETYPE_ROOT);

		  nextToken();
		}
		else
		{
		  relativePathStatus = RELATIVE_PATH_REQUIRED;
		}

		if (relativePathStatus != RELATIVE_PATH_NOT_PERMITTED)
		{
		  if (!tokenIs('|') && (null != m_token))
		  {
			RelativePathPattern();
		  }
		  else if (relativePathStatus == RELATIVE_PATH_REQUIRED)
		  {
			// "A relative path pattern was expected."
			error(XPATHErrorResources.ER_EXPECTED_REL_PATH_PATTERN, null);
		  }
		}

		// Terminate for safety.
		m_ops.setOp(m_ops.getOp(OpMap.MAPINDEX_LENGTH), OpCodes.ENDOP);
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);
	  }

	  /// 
	  /// <summary>
	  /// IdKeyPattern  ::=  'id' '(' Literal ')'
	  /// | 'key' '(' Literal ',' Literal ')'
	  /// (Also handle doc())
	  /// 
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void IdKeyPattern() throws javax.xml.transform.TransformerException
	  protected internal virtual void IdKeyPattern()
	  {
		FunctionCall();
	  }

	  /// 
	  /// <summary>
	  /// RelativePathPattern  ::=  StepPattern
	  /// | RelativePathPattern '/' StepPattern
	  /// | RelativePathPattern '//' StepPattern
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void RelativePathPattern() throws javax.xml.transform.TransformerException
	  protected internal virtual void RelativePathPattern()
	  {

		// Caller will have consumed any '/' or '//' preceding the
		// RelativePathPattern, so let StepPattern know it can't begin with a '/'
		bool trailingSlashConsumed = StepPattern(false);

		while (tokenIs('/'))
		{
		  nextToken();

		  // StepPattern() may consume first slash of pair in "a//b" while
		  // processing StepPattern "a".  On next iteration, let StepPattern know
		  // that happened, so it doesn't match ill-formed patterns like "a///b".
		  trailingSlashConsumed = StepPattern(!trailingSlashConsumed);
		}
	  }

	  /// 
	  /// <summary>
	  /// StepPattern  ::=  AbbreviatedNodeTestStep
	  /// </summary>
	  /// <param name="isLeadingSlashPermitted"> a boolean indicating whether a slash can
	  ///        appear at the start of this step
	  /// </param>
	  /// <returns> boolean indicating whether a slash following the step was consumed
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected boolean StepPattern(boolean isLeadingSlashPermitted) throws javax.xml.transform.TransformerException
	  protected internal virtual bool StepPattern(bool isLeadingSlashPermitted)
	  {
		return AbbreviatedNodeTestStep(isLeadingSlashPermitted);
	  }

	  /// 
	  /// <summary>
	  /// AbbreviatedNodeTestStep    ::=    '@'? NodeTest Predicate
	  /// </summary>
	  /// <param name="isLeadingSlashPermitted"> a boolean indicating whether a slash can
	  ///        appear at the start of this step
	  /// </param>
	  /// <returns> boolean indicating whether a slash following the step was consumed
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected boolean AbbreviatedNodeTestStep(boolean isLeadingSlashPermitted) throws javax.xml.transform.TransformerException
	  protected internal virtual bool AbbreviatedNodeTestStep(bool isLeadingSlashPermitted)
	  {

		int opPos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
		int axesType;

		// The next blocks guarantee that a MATCH_XXX will be added.
		int matchTypePos = -1;

		if (tokenIs('@'))
		{
		  axesType = OpCodes.MATCH_ATTRIBUTE;

		  appendOp(2, axesType);
		  nextToken();
		}
		else if (this.lookahead("::", 1))
		{
		  if (tokenIs("attribute"))
		  {
			axesType = OpCodes.MATCH_ATTRIBUTE;

			appendOp(2, axesType);
		  }
		  else if (tokenIs("child"))
		  {
			matchTypePos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
			axesType = OpCodes.MATCH_IMMEDIATE_ANCESTOR;

			appendOp(2, axesType);
		  }
		  else
		  {
			axesType = -1;

			this.error(XPATHErrorResources.ER_AXES_NOT_ALLOWED, new object[]{this.m_token});
		  }

		  nextToken();
		  nextToken();
		}
		else if (tokenIs('/'))
		{
		  if (!isLeadingSlashPermitted)
		  {
			// "A step was expected in the pattern, but '/' was encountered."
			error(XPATHErrorResources.ER_EXPECTED_STEP_PATTERN, null);
		  }
		  axesType = OpCodes.MATCH_ANY_ANCESTOR;

		  appendOp(2, axesType);
		  nextToken();
		}
		else
		{
		  matchTypePos = m_ops.getOp(OpMap.MAPINDEX_LENGTH);
		  axesType = OpCodes.MATCH_IMMEDIATE_ANCESTOR;

		  appendOp(2, axesType);
		}

		// Make room for telling how long the step is without the predicate
		m_ops.setOp(OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) + 1);

		NodeTest(axesType);

		// Tell how long the step is without the predicate
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH + 1, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		while (tokenIs('['))
		{
		  Predicate();
		}

		bool trailingSlashConsumed;

		// For "a//b", where "a" is current step, we need to mark operation of
		// current step as "MATCH_ANY_ANCESTOR".  Then we'll consume the first
		// slash and subsequent step will be treated as a MATCH_IMMEDIATE_ANCESTOR
		// (unless it too is followed by '//'.)
		//
		// %REVIEW%  Following is what happens today, but I'm not sure that's
		// %REVIEW%  correct behaviour.  Perhaps no valid case could be constructed
		// %REVIEW%  where it would matter?
		//
		// If current step is on the attribute axis (e.g., "@x//b"), we won't
		// change the current step, and let following step be marked as
		// MATCH_ANY_ANCESTOR on next call instead.
		if ((matchTypePos > -1) && tokenIs('/') && lookahead('/', 1))
		{
		  m_ops.setOp(matchTypePos, OpCodes.MATCH_ANY_ANCESTOR);

		  nextToken();

		  trailingSlashConsumed = true;
		}
		else
		{
		  trailingSlashConsumed = false;
		}

		// Tell how long the entire step is.
		m_ops.setOp(opPos + OpMap.MAPINDEX_LENGTH, m_ops.getOp(OpMap.MAPINDEX_LENGTH) - opPos);

		return trailingSlashConsumed;
	  }
	}

}