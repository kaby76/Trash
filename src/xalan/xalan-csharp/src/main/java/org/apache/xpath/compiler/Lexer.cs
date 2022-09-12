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
 * $Id: Lexer.java 524810 2007-04-02 15:51:55Z zongaro $
 */
namespace org.apache.xpath.compiler
{

	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// This class is in charge of lexical processing of the XPath
	/// expression into tokens.
	/// </summary>
	internal class Lexer
	{

	  /// <summary>
	  /// The target XPath.
	  /// </summary>
	  private Compiler m_compiler;

	  /// <summary>
	  /// The prefix resolver to map prefixes to namespaces in the XPath.
	  /// </summary>
	  internal PrefixResolver m_namespaceContext;

	  /// <summary>
	  /// The XPath processor object.
	  /// </summary>
	  internal XPathParser m_processor;

	  /// <summary>
	  /// This value is added to each element name in the TARGETEXTRA
	  /// that is a 'target' (right-most top-level element name).
	  /// </summary>
	  internal const int TARGETEXTRA = 10000;

	  /// <summary>
	  /// Ignore this, it is going away.
	  /// This holds a map to the m_tokenQueue that tells where the top-level elements are.
	  /// It is used for pattern matching so the m_tokenQueue can be walked backwards.
	  /// Each element that is a 'target', (right-most top level element name) has
	  /// TARGETEXTRA added to it.
	  /// 
	  /// </summary>
	  private int[] m_patternMap = new int[100];

	  /// <summary>
	  /// Ignore this, it is going away.
	  /// The number of elements that m_patternMap maps;
	  /// </summary>
	  private int m_patternMapSize;

	  /// <summary>
	  /// Create a Lexer object.
	  /// </summary>
	  /// <param name="compiler"> The owning compiler for this lexer. </param>
	  /// <param name="resolver"> The prefix resolver for mapping qualified name prefixes 
	  ///                 to namespace URIs. </param>
	  /// <param name="xpathProcessor"> The parser that is processing strings to opcodes. </param>
	  internal Lexer(Compiler compiler, PrefixResolver resolver, XPathParser xpathProcessor)
	  {

		m_compiler = compiler;
		m_namespaceContext = resolver;
		m_processor = xpathProcessor;
	  }

	  /// <summary>
	  /// Walk through the expression and build a token queue, and a map of the top-level
	  /// elements. </summary>
	  /// <param name="pat"> XSLT Expression.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void tokenize(String pat) throws javax.xml.transform.TransformerException
	  internal virtual void tokenize(string pat)
	  {
		tokenize(pat, null);
	  }

	  /// <summary>
	  /// Walk through the expression and build a token queue, and a map of the top-level
	  /// elements. </summary>
	  /// <param name="pat"> XSLT Expression. </param>
	  /// <param name="targetStrings"> Vector to hold Strings, may be null.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void tokenize(String pat, java.util.Vector targetStrings) throws javax.xml.transform.TransformerException
	  internal virtual void tokenize(string pat, ArrayList targetStrings)
	  {

		m_compiler.m_currentPattern = pat;
		m_patternMapSize = 0;

		// This needs to grow too.  Use a conservative estimate that the OpMapVector
		// needs about five time the length of the input path expression - to a
		// maximum of MAXTOKENQUEUESIZE*5.  If the OpMapVector needs to grow, grow
		// it freely (second argument to constructor).
		int initTokQueueSize = ((pat.Length < OpMap.MAXTOKENQUEUESIZE) ? pat.Length : OpMap.MAXTOKENQUEUESIZE) * 5;
		m_compiler.m_opMap = new OpMapVector(initTokQueueSize, OpMap.BLOCKTOKENQUEUESIZE * 5, OpMap.MAPINDEX_LENGTH);

		int nChars = pat.Length;
		int startSubstring = -1;
		int posOfNSSep = -1;
		bool isStartOfPat = true;
		bool isAttrName = false;
		bool isNum = false;

		// Nesting of '[' so we can know if the given element should be
		// counted inside the m_patternMap.
		int nesting = 0;

		// char[] chars = pat.toCharArray();
		for (int i = 0; i < nChars; i++)
		{
		  char c = pat[i];

		  switch (c)
		  {
		  case '\"' :
		  {
			if (startSubstring != -1)
			{
			  isNum = false;
			  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);
			  isAttrName = false;

			  if (-1 != posOfNSSep)
			  {
				posOfNSSep = mapNSTokens(pat, startSubstring, posOfNSSep, i);
			  }
			  else
			  {
				addToTokenQueue(pat.Substring(startSubstring, i - startSubstring));
			  }
			}

			startSubstring = i;

			for (i++; (i < nChars) && ((c = pat[i]) != '\"'); i++)
			{
				;
			}

			if (c == '\"' && i < nChars)
			{
			  addToTokenQueue(pat.Substring(startSubstring, (i + 1) - startSubstring));

			  startSubstring = -1;
			}
			else
			{
			  m_processor.error(XPATHErrorResources.ER_EXPECTED_DOUBLE_QUOTE, null); //"misquoted literal... expected double quote!");
			}
		  }
		  break;
		  case '\'' :
			if (startSubstring != -1)
			{
			  isNum = false;
			  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);
			  isAttrName = false;

			  if (-1 != posOfNSSep)
			  {
				posOfNSSep = mapNSTokens(pat, startSubstring, posOfNSSep, i);
			  }
			  else
			  {
				addToTokenQueue(pat.Substring(startSubstring, i - startSubstring));
			  }
			}

			startSubstring = i;

			for (i++; (i < nChars) && ((c = pat[i]) != '\''); i++)
			{
				;
			}

			if (c == '\'' && i < nChars)
			{
			  addToTokenQueue(pat.Substring(startSubstring, (i + 1) - startSubstring));

			  startSubstring = -1;
			}
			else
			{
			  m_processor.error(XPATHErrorResources.ER_EXPECTED_SINGLE_QUOTE, null); //"misquoted literal... expected single quote!");
			}
			break;
		  case 0x0A :
		  case 0x0D :
		  case ' ' :
		  case '\t' :
			if (startSubstring != -1)
			{
			  isNum = false;
			  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);
			  isAttrName = false;

			  if (-1 != posOfNSSep)
			  {
				posOfNSSep = mapNSTokens(pat, startSubstring, posOfNSSep, i);
			  }
			  else
			  {
				addToTokenQueue(pat.Substring(startSubstring, i - startSubstring));
			  }

			  startSubstring = -1;
			}
			break;
		  case '@' :
			isAttrName = true;

		  // fall-through on purpose
			  goto case '-';
		  case '-' :
			if ('-' == c)
			{
			  if (!(isNum || (startSubstring == -1)))
			  {
				break;
			  }

			  isNum = false;
			}

		  // fall-through on purpose
			  goto case '(';
		  case '(' :
		  case '[' :
		  case ')' :
		  case ']' :
		  case '|' :
		  case '/' :
		  case '*' :
		  case '+' :
		  case '=' :
		  case ',' :
		  case '\\' : // Unused at the moment
		  case '^' : // Unused at the moment
		  case '!' : // Unused at the moment
		  case '$' :
		  case '<' :
		  case '>' :
			if (startSubstring != -1)
			{
			  isNum = false;
			  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);
			  isAttrName = false;

			  if (-1 != posOfNSSep)
			  {
				posOfNSSep = mapNSTokens(pat, startSubstring, posOfNSSep, i);
			  }
			  else
			  {
				addToTokenQueue(pat.Substring(startSubstring, i - startSubstring));
			  }

			  startSubstring = -1;
			}
			else if (('/' == c) && isStartOfPat)
			{
			  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);
			}
			else if ('*' == c)
			{
			  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);
			  isAttrName = false;
			}

			if (0 == nesting)
			{
			  if ('|' == c)
			  {
				if (null != targetStrings)
				{
				  recordTokenString(targetStrings);
				}

				isStartOfPat = true;
			  }
			}

			if ((')' == c) || (']' == c))
			{
			  nesting--;
			}
			else if (('(' == c) || ('[' == c))
			{
			  nesting++;
			}

			addToTokenQueue(pat.Substring(i, 1));
			break;
		  case ':' :
			if (i > 0)
			{
			  if (posOfNSSep == (i - 1))
			  {
				if (startSubstring != -1)
				{
				  if (startSubstring < (i - 1))
				  {
					addToTokenQueue(pat.Substring(startSubstring, (i - 1) - startSubstring));
				  }
				}

				isNum = false;
				isAttrName = false;
				startSubstring = -1;
				posOfNSSep = -1;

				addToTokenQueue(pat.Substring(i - 1, (i + 1) - (i - 1)));

				break;
			  }
			  else
			  {
				posOfNSSep = i;
			  }
			}

		  // fall through on purpose
			  goto default;
		  default :
			if (-1 == startSubstring)
			{
			  startSubstring = i;
			  isNum = char.IsDigit(c);
			}
			else if (isNum)
			{
			  isNum = char.IsDigit(c);
			}
		break;
		  }
		}

		if (startSubstring != -1)
		{
		  isNum = false;
		  isStartOfPat = mapPatternElemPos(nesting, isStartOfPat, isAttrName);

		  if ((-1 != posOfNSSep) || ((m_namespaceContext != null) && (m_namespaceContext.handlesNullPrefixes())))
		  {
			posOfNSSep = mapNSTokens(pat, startSubstring, posOfNSSep, nChars);
		  }
		  else
		  {
			addToTokenQueue(pat.Substring(startSubstring, nChars - startSubstring));
		  }
		}

		if (0 == m_compiler.TokenQueueSize)
		{
		  m_processor.error(XPATHErrorResources.ER_EMPTY_EXPRESSION, null); //"Empty expression!");
		}
		else if (null != targetStrings)
		{
		  recordTokenString(targetStrings);
		}

		m_processor.m_queueMark = 0;
	  }

	  /// <summary>
	  /// Record the current position on the token queue as long as
	  /// this is a top-level element.  Must be called before the
	  /// next token is added to the m_tokenQueue.
	  /// </summary>
	  /// <param name="nesting"> The nesting count for the pattern element. </param>
	  /// <param name="isStart"> true if this is the start of a pattern. </param>
	  /// <param name="isAttrName"> true if we have determined that this is an attribute name.
	  /// </param>
	  /// <returns> true if this is the start of a pattern. </returns>
	  private bool mapPatternElemPos(int nesting, bool isStart, bool isAttrName)
	  {

		if (0 == nesting)
		{
		  if (m_patternMapSize >= m_patternMap.Length)
		  {
			int[] patternMap = m_patternMap;
			int len = m_patternMap.Length;
			m_patternMap = new int[m_patternMapSize + 100];
			Array.Copy(patternMap, 0, m_patternMap, 0, len);
		  }
		  if (!isStart)
		  {
			m_patternMap[m_patternMapSize - 1] -= TARGETEXTRA;
		  }
		  m_patternMap[m_patternMapSize] = (m_compiler.TokenQueueSize - (isAttrName ? 1 : 0)) + TARGETEXTRA;

		  m_patternMapSize++;

		  isStart = false;
		}

		return isStart;
	  }

	  /// <summary>
	  /// Given a map pos, return the corresponding token queue pos.
	  /// </summary>
	  /// <param name="i"> The index in the m_patternMap.
	  /// </param>
	  /// <returns> the token queue position. </returns>
	  private int getTokenQueuePosFromMap(int i)
	  {

		int pos = m_patternMap[i];

		return (pos >= TARGETEXTRA) ? (pos - TARGETEXTRA) : pos;
	  }

	  /// <summary>
	  /// Reset token queue mark and m_token to a
	  /// given position. </summary>
	  /// <param name="mark"> The new position. </param>
	  private void resetTokenMark(int mark)
	  {

		int qsz = m_compiler.TokenQueueSize;

		m_processor.m_queueMark = (mark > 0) ? ((mark <= qsz) ? mark - 1 : mark) : 0;

		if (m_processor.m_queueMark < qsz)
		{
		  m_processor.m_token = (string) m_compiler.TokenQueue.elementAt(m_processor.m_queueMark++);
		  m_processor.m_tokenChar = m_processor.m_token[0];
		}
		else
		{
		  m_processor.m_token = null;
		  m_processor.m_tokenChar = (char)0;
		}
	  }

	  /// <summary>
	  /// Given a string, return the corresponding keyword token.
	  /// </summary>
	  /// <param name="key"> The keyword.
	  /// </param>
	  /// <returns> An opcode value. </returns>
	  internal int getKeywordToken(string key)
	  {

		int tok;

		try
		{
		  int? itok = (int?) Keywords.getKeyWord(key);

		  tok = (null != itok) ? itok.Value : 0;
		}
		catch (System.NullReferenceException)
		{
		  tok = 0;
		}
		catch (System.InvalidCastException)
		{
		  tok = 0;
		}

		return tok;
	  }

	  /// <summary>
	  /// Record the current token in the passed vector.
	  /// </summary>
	  /// <param name="targetStrings"> Vector of string. </param>
	  private void recordTokenString(ArrayList targetStrings)
	  {

		int tokPos = getTokenQueuePosFromMap(m_patternMapSize - 1);

		resetTokenMark(tokPos + 1);

		if (m_processor.lookahead('(', 1))
		{
		  int tok = getKeywordToken(m_processor.m_token);

		  switch (tok)
		  {
		  case OpCodes.NODETYPE_COMMENT :
			targetStrings.Add(PsuedoNames.PSEUDONAME_COMMENT);
			break;
		  case OpCodes.NODETYPE_TEXT :
			targetStrings.Add(PsuedoNames.PSEUDONAME_TEXT);
			break;
		  case OpCodes.NODETYPE_NODE :
			targetStrings.Add(PsuedoNames.PSEUDONAME_ANY);
			break;
		  case OpCodes.NODETYPE_ROOT :
			targetStrings.Add(PsuedoNames.PSEUDONAME_ROOT);
			break;
		  case OpCodes.NODETYPE_ANYELEMENT :
			targetStrings.Add(PsuedoNames.PSEUDONAME_ANY);
			break;
		  case OpCodes.NODETYPE_PI :
			targetStrings.Add(PsuedoNames.PSEUDONAME_ANY);
			break;
		  default :
			targetStrings.Add(PsuedoNames.PSEUDONAME_ANY);
		break;
		  }
		}
		else
		{
		  if (m_processor.tokenIs('@'))
		  {
			tokPos++;

			resetTokenMark(tokPos + 1);
		  }

		  if (m_processor.lookahead(':', 1))
		  {
			tokPos += 2;
		  }

		  targetStrings.Add(m_compiler.TokenQueue.elementAt(tokPos));
		}
	  }

	  /// <summary>
	  /// Add a token to the token queue.
	  /// 
	  /// </summary>
	  /// <param name="s"> The token. </param>
	  private void addToTokenQueue(string s)
	  {
		m_compiler.TokenQueue.addElement(s);
	  }

	  /// <summary>
	  /// When a seperator token is found, see if there's a element name or
	  /// the like to map.
	  /// </summary>
	  /// <param name="pat"> The XPath name string. </param>
	  /// <param name="startSubstring"> The start of the name string. </param>
	  /// <param name="posOfNSSep"> The position of the namespace seperator (':'). </param>
	  /// <param name="posOfScan"> The end of the name index.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// </exception>
	  /// <returns> -1 always. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int mapNSTokens(String pat, int startSubstring, int posOfNSSep, int posOfScan) throws javax.xml.transform.TransformerException
	  private int mapNSTokens(string pat, int startSubstring, int posOfNSSep, int posOfScan)
	  {

		string prefix = "";

		if ((startSubstring >= 0) && (posOfNSSep >= 0))
		{
		   prefix = pat.Substring(startSubstring, posOfNSSep - startSubstring);
		}
		string uName;

		if ((null != m_namespaceContext) && !prefix.Equals("*") && !prefix.Equals("xmlns"))
		{
		  try
		  {
			if (prefix.Length > 0)
			{
			  uName = ((PrefixResolver) m_namespaceContext).getNamespaceForPrefix(prefix);
			}
			else
			{

			  // Assume last was wildcard. This is not legal according
			  // to the draft. Set the below to true to make namespace
			  // wildcards work.
			  if (false)
			  {
				addToTokenQueue(":");

				string s = pat.Substring(posOfNSSep + 1, posOfScan - (posOfNSSep + 1));

				if (s.Length > 0)
				{
				  addToTokenQueue(s);
				}

				return -1;
			  }
			  else
			  {
				uName = ((PrefixResolver) m_namespaceContext).getNamespaceForPrefix(prefix);
			  }
			}
		  }
		  catch (System.InvalidCastException)
		  {
			uName = m_namespaceContext.getNamespaceForPrefix(prefix);
		  }
		}
		else
		{
		  uName = prefix;
		}

		if ((null != uName) && (uName.Length > 0))
		{
		  addToTokenQueue(uName);
		  addToTokenQueue(":");

		  string s = pat.Substring(posOfNSSep + 1, posOfScan - (posOfNSSep + 1));

		  if (s.Length > 0)
		  {
			addToTokenQueue(s);
		  }
		}
		else
		{
			// To older XPath code it doesn't matter if
			// error() is called or errorForDOM3().
			m_processor.errorForDOM3(XPATHErrorResources.ER_PREFIX_MUST_RESOLVE, new string[] {prefix}); //"Prefix must resolve to a namespace: {0}";

	/// <summary>
	/// old code commented out 17-Sep-2004
	/// // error("Could not locate namespace for prefix: "+prefix);
	/// //		  m_processor.error(XPATHErrorResources.ER_PREFIX_MUST_RESOLVE,
	/// //					 new String[] {prefix});  //"Prefix must resolve to a namespace: {0}";
	/// </summary>

		  /// <summary>
		  ///*  Old code commented out 10-Jan-2001
		  /// addToTokenQueue(prefix);
		  /// addToTokenQueue(":");
		  /// 
		  /// String s = pat.substring(posOfNSSep + 1, posOfScan);
		  /// 
		  /// if (s.length() > 0)
		  ///  addToTokenQueue(s);
		  /// **
		  /// </summary>
		}

		return -1;
	  }
	}

}