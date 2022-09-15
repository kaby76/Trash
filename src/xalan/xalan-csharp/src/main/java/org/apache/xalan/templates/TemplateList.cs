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
 * $Id: TemplateList.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using DTM = org.apache.xml.dtm.DTM;
	using QName = org.apache.xml.utils.QName;
	using Expression = org.apache.xpath.Expression;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using PsuedoNames = org.apache.xpath.compiler.PsuedoNames;
	using NodeTest = org.apache.xpath.patterns.NodeTest;
	using StepPattern = org.apache.xpath.patterns.StepPattern;
	using UnionPattern = org.apache.xpath.patterns.UnionPattern;

	/// <summary>
	/// Encapsulates a template list, and helps locate individual templates.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class TemplateList
	{
		internal const long serialVersionUID = 5803675288911728791L;

	  /// <summary>
	  /// Construct a TemplateList object. Needs to be public so it can
	  /// be invoked from the CompilingStylesheetHandler.
	  /// </summary>
	  public TemplateList() : base()
	  {
	  }

	  /// <summary>
	  /// Add a template to the table of named templates and/or the table of templates
	  /// with match patterns.  This routine should
	  /// be called in decreasing order of precedence but it checks nonetheless.
	  /// </summary>
	  /// <param name="template"> </param>
	  public virtual ElemTemplate Template
	  {
		  set
		  {
			XPath matchXPath = value.Match;
    
			if (null == value.Name && null == matchXPath)
			{
			  value.error(XSLTErrorResources.ER_NEED_NAME_OR_MATCH_ATTRIB, new object[]{"xsl:template"});
			}
    
			if (null != value.Name)
			{
			  ElemTemplate existingTemplate = (ElemTemplate) m_namedTemplates[value.Name];
			  if (null == existingTemplate)
			  {
				m_namedTemplates[value.Name] = value;
			  }
			  else
			  {
				int existingPrecedence = existingTemplate.StylesheetComposed.ImportCountComposed;
				int newPrecedence = value.StylesheetComposed.ImportCountComposed;
				if (newPrecedence > existingPrecedence)
				{
				  // This should never happen
				  m_namedTemplates[value.Name] = value;
				}
				else if (newPrecedence == existingPrecedence)
				{
				  value.error(XSLTErrorResources.ER_DUPLICATE_NAMED_TEMPLATE, new object[]{value.Name});
				}
			  }
			}
    
    
    
			if (null != matchXPath)
			{
			  Expression matchExpr = matchXPath.Expression;
    
			  if (matchExpr is StepPattern)
			  {
				insertPatternInTable((StepPattern) matchExpr, value);
			  }
			  else if (matchExpr is UnionPattern)
			  {
				UnionPattern upat = (UnionPattern) matchExpr;
				StepPattern[] pats = upat.Patterns;
				int n = pats.Length;
    
				for (int i = 0; i < n; i++)
				{
				  insertPatternInTable(pats[i], value);
				}
			  }
			  else
			  {
    
				// TODO: assert error
			  }
			}
		  }
	  }

	  /// <summary>
	  /// Flag to indicate whether in DEBUG mode </summary>
	  internal const bool DEBUG = false;

	  /// <summary>
	  /// Dump all patterns and elements that match those patterns
	  /// 
	  /// </summary>
	  internal virtual void dumpAssociationTables()
	  {

		System.Collections.IEnumerator associations = m_patternTable.Values.GetEnumerator();

		while (associations.MoveNext())
		{
		  TemplateSubPatternAssociation head = (TemplateSubPatternAssociation) associations.Current;

		  while (null != head)
		  {
			Console.Write("(" + head.TargetString + ", " + head.Pattern + ")");

			head = head.Next;
		  }

		  Console.WriteLine("\n.....");
		}

		TemplateSubPatternAssociation head = m_wildCardPatterns;

		Console.Write("wild card list: ");

		while (null != head)
		{
		  Console.Write("(" + head.TargetString + ", " + head.Pattern + ")");

		  head = head.Next;
		}

		Console.WriteLine("\n.....");
	  }

	  /// <summary>
	  /// After all templates have been added, this function
	  /// should be called.
	  /// </summary>
	  public virtual void compose(StylesheetRoot sroot)
	  {

		if (DEBUG)
		{
		  Console.WriteLine("Before wildcard insert...");
		  dumpAssociationTables();
		}

		if (null != m_wildCardPatterns)
		{
		  System.Collections.IEnumerator associations = m_patternTable.Values.GetEnumerator();

		  while (associations.MoveNext())
		  {
			TemplateSubPatternAssociation head = (TemplateSubPatternAssociation) associations.Current;
			TemplateSubPatternAssociation wild = m_wildCardPatterns;

			while (null != wild)
			{
			  try
			  {
				head = insertAssociationIntoList(head, (TemplateSubPatternAssociation) wild.clone(), true);
			  }
			  catch (CloneNotSupportedException)
			  {
			  }

			  wild = wild.Next;
			}
		  }
		}

		if (DEBUG)
		{
		  Console.WriteLine("After wildcard insert...");
		  dumpAssociationTables();
		}
	  }

	  /// <summary>
	  /// Insert the given TemplateSubPatternAssociation into the the linked
	  /// list.  Sort by import precedence, then priority, then by document order.
	  /// </summary>
	  /// <param name="head"> The first TemplateSubPatternAssociation in the linked list. </param>
	  /// <param name="item"> The item that we want to insert into the proper place. </param>
	  /// <param name="isWildCardInsert"> <code>true</code> if we are inserting a wild card 
	  ///             template onto this list. </param>
	  /// <returns> the new head of the list. </returns>
	  private TemplateSubPatternAssociation insertAssociationIntoList(TemplateSubPatternAssociation head, TemplateSubPatternAssociation item, bool isWildCardInsert)
	  {

		// Sort first by import level (higher level is at front),
		// then by priority (highest priority is at front),
		// then by document order (later in document is at front).

		double priority = getPriorityOrScore(item);
		double workPriority;
		int importLevel = item.ImportLevel;
		int docOrder = item.DocOrderPos;
		TemplateSubPatternAssociation insertPoint = head;
		TemplateSubPatternAssociation next;
		bool insertBefore; // true means insert before insertPoint; otherwise after
									  // This can only be true if insertPoint is pointing to
									  // the first or last template.

		// Spin down so that insertPoint points to:
		// (a) the template immediately _before_ the first template on the chain with
		// a precedence that is either (i) less than ours or (ii) the same as ours but
		// the template document position is less than ours
		// -or-
		// (b) the last template on the chain if no such template described in (a) exists.
		// If we are pointing to the first template or the last template (that is, case b),
		// we need to determine whether to insert before or after the template.  Otherwise,
		// we always insert after the insertPoint.

		while (true)
		{
		  next = insertPoint.Next;
		  if (null == next)
		  {
			break;
		  }
		  else
		  {
			workPriority = getPriorityOrScore(next);
			if (importLevel > next.ImportLevel)
			{
			  break;
			}
			else if (importLevel < next.ImportLevel)
			{
			  insertPoint = next;
			}
			else if (priority > workPriority) // import precedence is equal
			{
			  break;
			}
			else if (priority < workPriority)
			{
			  insertPoint = next;
			}
			else if (docOrder >= next.DocOrderPos) // priorities, import are equal
			{
			  break;
			}
			else
			{
			  insertPoint = next;
			}
		  }
		}

		if ((null == next) || (insertPoint == head)) // insert point is first or last
		{
		  workPriority = getPriorityOrScore(insertPoint);
		  if (importLevel > insertPoint.ImportLevel)
		  {
			insertBefore = true;
		  }
		  else if (importLevel < insertPoint.ImportLevel)
		  {
			insertBefore = false;
		  }
		  else if (priority > workPriority)
		  {
			insertBefore = true;
		  }
		  else if (priority < workPriority)
		  {
			insertBefore = false;
		  }
		  else if (docOrder >= insertPoint.DocOrderPos)
		  {
			insertBefore = true;
		  }
		  else
		  {
			insertBefore = false;
		  }
		}
		else
		{
		  insertBefore = false;
		}

		// System.out.println("appending: "+target+" to "+matchPat.getPattern());

		if (isWildCardInsert)
		{
		  if (insertBefore)
		  {
			item.Next = insertPoint;

			string key = insertPoint.TargetString;

			item.TargetString = key;
			putHead(key, item);
			return item;
		  }
		  else
		  {
			item.Next = next;
			insertPoint.Next = item;
			return head;
		  }
		}
		else
		{
		  if (insertBefore)
		  {
			item.Next = insertPoint;

			if (insertPoint.Wild || item.Wild)
			{
			  m_wildCardPatterns = item;
			}
			else
			{
			  putHead(item.TargetString, item);
			}
			return item;
		  }
		  else
		  {
			item.Next = next;
			insertPoint.Next = item;
			return head;
		  }
		}
	  }

	  /// <summary>
	  /// Add a template to the template list.
	  /// </summary>
	  /// <param name="pattern"> </param>
	  /// <param name="template"> </param>
	  private void insertPatternInTable(StepPattern pattern, ElemTemplate template)
	  {

		string target = pattern.TargetString;

		if (null != target)
		{
		  string pstring = template.Match.getPatternString();
		  TemplateSubPatternAssociation association = new TemplateSubPatternAssociation(template, pattern, pstring);

		  // See if there's already one there
		  bool isWildCard = association.Wild;
		  TemplateSubPatternAssociation head = isWildCard ? m_wildCardPatterns : getHead(target);

		  if (null == head)
		  {
			if (isWildCard)
			{
			  m_wildCardPatterns = association;
			}
			else
			{
			  putHead(target, association);
			}
		  }
		  else
		  {
			insertAssociationIntoList(head, association, false);
		  }
		}
	  }

	  /// <summary>
	  /// Given a match pattern and template association, return the 
	  /// score of that match.  This score or priority can always be 
	  /// statically calculated.
	  /// </summary>
	  /// <param name="matchPat"> The match pattern to template association.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NODETEST"/>, 
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NONE"/>, 
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NSWILD"/>, 
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_OTHER"/>, or 
	  ///         the value defined by the priority attribute of the template.
	  ///  </returns>
	  private double getPriorityOrScore(TemplateSubPatternAssociation matchPat)
	  {

		double priority = matchPat.Template.Priority;

		if (priority == XPath.MATCH_SCORE_NONE)
		{
		  Expression ex = matchPat.StepPattern;

		  if (ex is NodeTest)
		  {
			return ((NodeTest) ex).DefaultScore;
		  }
		}

		return priority;
	  }

	  /// <summary>
	  /// Locate a named template.
	  /// </summary>
	  /// <param name="qname">  Qualified name of the template.
	  /// </param>
	  /// <returns> Template argument with the requested name, or null if not found. </returns>
	  public virtual ElemTemplate getTemplate(QName qname)
	  {
		return (ElemTemplate) m_namedTemplates[qname];
	  }

	  /// <summary>
	  /// Get the head of the most likely list of associations to check, based on 
	  /// the name and type of the targetNode argument.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <param name="targetNode"> The target node that will be checked for a match. </param>
	  /// <param name="dtm"> The dtm owner for the target node.
	  /// </param>
	  /// <returns> The head of a linked list that contains all possible match pattern to 
	  /// template associations. </returns>
	  public virtual TemplateSubPatternAssociation getHead(XPathContext xctxt, int targetNode, DTM dtm)
	  {
		short targetNodeType = dtm.getNodeType(targetNode);
		TemplateSubPatternAssociation head;

		switch (targetNodeType)
		{
		case DTM.ELEMENT_NODE :
		case DTM.ATTRIBUTE_NODE :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getLocalName(targetNode)];
		  break;
		case DTM.TEXT_NODE :
		case DTM.CDATA_SECTION_NODE :
		  head = m_textPatterns;
		  break;
		case DTM.ENTITY_REFERENCE_NODE :
		case DTM.ENTITY_NODE :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getNodeName(targetNode)]; // %REVIEW% I think this is right
		  break;
		case DTM.PROCESSING_INSTRUCTION_NODE :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getLocalName(targetNode)];
		  break;
		case DTM.COMMENT_NODE :
		  head = m_commentPatterns;
		  break;
		case DTM.DOCUMENT_NODE :
		case DTM.DOCUMENT_FRAGMENT_NODE :
		  head = m_docPatterns;
		  break;
		case DTM.NOTATION_NODE :
		default :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getNodeName(targetNode)]; // %REVIEW% I think this is right
	  break;
		}

		return (null == head) ? m_wildCardPatterns : head;
	  }

	  /// <summary>
	  /// Given a target element, find the template that best
	  /// matches in the given XSL document, according
	  /// to the rules specified in the xsl draft.  This variation of getTemplate 
	  /// assumes the current node and current expression node have already been 
	  /// pushed. 
	  /// </summary>
	  /// <param name="xctxt"> </param>
	  /// <param name="targetNode"> </param>
	  /// <param name="mode"> A string indicating the display mode. </param>
	  /// <param name="maxImportLevel"> The maximum importCountComposed that we should consider or -1
	  ///        if we should consider all import levels.  This is used by apply-imports to
	  ///        access templates that have been overridden. </param>
	  /// <param name="quietConflictWarnings"> </param>
	  /// <returns> Rule that best matches targetElem. </returns>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemTemplate getTemplateFast(org.apache.xpath.XPathContext xctxt, int targetNode, int expTypeID, org.apache.xml.utils.QName mode, int maxImportLevel, boolean quietConflictWarnings, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
	  public virtual ElemTemplate getTemplateFast(XPathContext xctxt, int targetNode, int expTypeID, QName mode, int maxImportLevel, bool quietConflictWarnings, DTM dtm)
	  {

		TemplateSubPatternAssociation head;

		switch (dtm.getNodeType(targetNode))
		{
		case DTM.ELEMENT_NODE :
		case DTM.ATTRIBUTE_NODE :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getLocalNameFromExpandedNameID(expTypeID)];
		  break;
		case DTM.TEXT_NODE :
		case DTM.CDATA_SECTION_NODE :
		  head = m_textPatterns;
		  break;
		case DTM.ENTITY_REFERENCE_NODE :
		case DTM.ENTITY_NODE :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getNodeName(targetNode)]; // %REVIEW% I think this is right
		  break;
		case DTM.PROCESSING_INSTRUCTION_NODE :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getLocalName(targetNode)];
		  break;
		case DTM.COMMENT_NODE :
		  head = m_commentPatterns;
		  break;
		case DTM.DOCUMENT_NODE :
		case DTM.DOCUMENT_FRAGMENT_NODE :
		  head = m_docPatterns;
		  break;
		case DTM.NOTATION_NODE :
		default :
		  head = (TemplateSubPatternAssociation) m_patternTable[dtm.getNodeName(targetNode)]; // %REVIEW% I think this is right
	  break;
		}

		if (null == head)
		{
		  head = m_wildCardPatterns;
		  if (null == head)
		  {
			return null;
		  }
		}

		// XSLT functions, such as xsl:key, need to be able to get to 
		// current ElemTemplateElement via a cast to the prefix resolver.
		// Setting this fixes bug idkey03.
		xctxt.pushNamespaceContextNull();
		try
		{
		  do
		  {
			if ((maxImportLevel > -1) && (head.ImportLevel > maxImportLevel))
			{
			  continue;
			}
			ElemTemplate template = head.Template;
			xctxt.NamespaceContext = template;

			if ((head.m_stepPattern.execute(xctxt, targetNode, dtm, expTypeID) != NodeTest.SCORE_NONE) && head.matchMode(mode))
			{
			  if (quietConflictWarnings)
			  {
				checkConflicts(head, xctxt, targetNode, mode);
			  }

			  return template;
			}
		  } while (null != (head = head.Next));
		}
		finally
		{
		  xctxt.popNamespaceContext();
		}

		return null;
	  } // end findTemplate

	  /// <summary>
	  /// Given a target element, find the template that best
	  /// matches in the given XSL document, according
	  /// to the rules specified in the xsl draft.
	  /// </summary>
	  /// <param name="xctxt"> </param>
	  /// <param name="targetNode"> </param>
	  /// <param name="mode"> A string indicating the display mode. </param>
	  /// <param name="quietConflictWarnings"> </param>
	  /// <returns> Rule that best matches targetElem. </returns>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemTemplate getTemplate(org.apache.xpath.XPathContext xctxt, int targetNode, org.apache.xml.utils.QName mode, boolean quietConflictWarnings, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
	  public virtual ElemTemplate getTemplate(XPathContext xctxt, int targetNode, QName mode, bool quietConflictWarnings, DTM dtm)
	  {

		TemplateSubPatternAssociation head = getHead(xctxt, targetNode, dtm);

		if (null != head)
		{
		  // XSLT functions, such as xsl:key, need to be able to get to 
		  // current ElemTemplateElement via a cast to the prefix resolver.
		  // Setting this fixes bug idkey03.
		  xctxt.pushNamespaceContextNull();
		  xctxt.pushCurrentNodeAndExpression(targetNode, targetNode);
		  try
		  {
			do
			{
			  ElemTemplate template = head.Template;
			  xctxt.NamespaceContext = template;

			  if ((head.m_stepPattern.execute(xctxt, targetNode) != NodeTest.SCORE_NONE) && head.matchMode(mode))
			  {
				if (quietConflictWarnings)
				{
				  checkConflicts(head, xctxt, targetNode, mode);
				}

				return template;
			  }
			} while (null != (head = head.Next));
		  }
		  finally
		  {
			xctxt.popCurrentNodeAndExpression();
			xctxt.popNamespaceContext();
		  }
		}

		return null;
	  } // end findTemplate

	  /// <summary>
	  /// Given a target element, find the template that best
	  /// matches in the given XSL document, according
	  /// to the rules specified in the xsl draft.
	  /// </summary>
	  /// <param name="xctxt"> </param>
	  /// <param name="targetNode"> </param>
	  /// <param name="mode"> A string indicating the display mode. </param>
	  /// <param name="maxImportLevel"> The maximum importCountComposed that we should consider or -1
	  ///        if we should consider all import levels.  This is used by apply-imports to
	  ///        access templates that have been overridden. </param>
	  /// <param name="endImportLevel"> The count of composed imports </param>
	  /// <param name="quietConflictWarnings"> </param>
	  /// <returns> Rule that best matches targetElem. </returns>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemTemplate getTemplate(org.apache.xpath.XPathContext xctxt, int targetNode, org.apache.xml.utils.QName mode, int maxImportLevel, int endImportLevel, boolean quietConflictWarnings, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
	  public virtual ElemTemplate getTemplate(XPathContext xctxt, int targetNode, QName mode, int maxImportLevel, int endImportLevel, bool quietConflictWarnings, DTM dtm)
	  {

		TemplateSubPatternAssociation head = getHead(xctxt, targetNode, dtm);

		if (null != head)
		{
		  // XSLT functions, such as xsl:key, need to be able to get to 
		  // current ElemTemplateElement via a cast to the prefix resolver.
		  // Setting this fixes bug idkey03.
		  xctxt.pushNamespaceContextNull();
		  xctxt.pushCurrentNodeAndExpression(targetNode, targetNode);
		  try
		  {
			do
			{
			  if ((maxImportLevel > -1) && (head.ImportLevel > maxImportLevel))
			  {
				continue;
			  }
			  if (head.ImportLevel <= maxImportLevel - endImportLevel)
			  {
				return null;
			  }
			  ElemTemplate template = head.Template;
			  xctxt.NamespaceContext = template;

			  if ((head.m_stepPattern.execute(xctxt, targetNode) != NodeTest.SCORE_NONE) && head.matchMode(mode))
			  {
				if (quietConflictWarnings)
				{
				  checkConflicts(head, xctxt, targetNode, mode);
				}

				return template;
			  }
			} while (null != (head = head.Next));
		  }
		  finally
		  {
			xctxt.popCurrentNodeAndExpression();
			xctxt.popNamespaceContext();
		  }
		}

		return null;
	  } // end findTemplate

	  /// <summary>
	  /// Get a TemplateWalker for use by a compiler.  See the documentation for
	  /// the TreeWalker inner class for further details.
	  /// </summary>
	  public virtual TemplateWalker Walker
	  {
		  get
		  {
			return new TemplateWalker(this);
		  }
	  }

	  /// <summary>
	  /// Check for match conflicts, and warn the stylesheet author.
	  /// </summary>
	  /// <param name="head"> Template pattern </param>
	  /// <param name="xctxt"> Current XPath context </param>
	  /// <param name="targetNode"> Node matching the pattern </param>
	  /// <param name="mode"> reference, which may be null, to the <a href="http://www.w3.org/TR/xslt#modes">current mode</a>. </param>
	  private void checkConflicts(TemplateSubPatternAssociation head, XPathContext xctxt, int targetNode, QName mode)
	  {

		// TODO: Check for conflicts.
	  }

	  /// <summary>
	  /// Add object to vector if not already there.
	  /// </summary>
	  /// <param name="obj"> </param>
	  /// <param name="v"> </param>
	  private void addObjectIfNotFound(object obj, ArrayList v)
	  {

		int n = v.Count;
		bool addIt = true;

		for (int i = 0; i < n; i++)
		{
		  if (v[i] == obj)
		  {
			addIt = false;

			break;
		  }
		}

		if (addIt)
		{
		  v.Add(obj);
		}
	  }

	  /// <summary>
	  /// Keyed on string macro names, and holding values
	  /// that are macro elements in the XSL DOM tree.
	  /// Initialized in initMacroLookupTable, and used in
	  /// findNamedTemplate.
	  /// @serial
	  /// </summary>
	  private Hashtable m_namedTemplates = new Hashtable(89);

	  /// <summary>
	  /// This table is keyed on the target elements
	  /// of patterns, and contains linked lists of
	  /// the actual patterns that match the target element
	  /// to some degree of specifity.
	  /// @serial
	  /// </summary>
	  private Hashtable m_patternTable = new Hashtable(89);

	  /// <summary>
	  /// Wildcard patterns.
	  ///  @serial          
	  /// </summary>
	  private TemplateSubPatternAssociation m_wildCardPatterns = null;

	  /// <summary>
	  /// Text Patterns.
	  ///  @serial          
	  /// </summary>
	  private TemplateSubPatternAssociation m_textPatterns = null;

	  /// <summary>
	  /// Root document Patterns.
	  ///  @serial          
	  /// </summary>
	  private TemplateSubPatternAssociation m_docPatterns = null;

	  /// <summary>
	  /// Comment Patterns.
	  ///  @serial          
	  /// </summary>
	  private TemplateSubPatternAssociation m_commentPatterns = null;

	  /// <summary>
	  /// Get table of named Templates.
	  /// These are keyed on template names, and holding values
	  /// that are template elements.
	  /// </summary>
	  /// <returns> A Hashtable dictionary that contains <seealso cref="string"/>s 
	  /// as the keys, and <seealso cref="org.apache.xalan.templates.ElemTemplate"/>s as the 
	  /// values.  </returns>
	  private Hashtable NamedTemplates
	  {
		  get
		  {
			return m_namedTemplates;
		  }
		  set
		  {
			m_namedTemplates = value;
		  }
	  }


	  /// <summary>
	  /// Get the head of the assocation list that is keyed by target.
	  /// </summary>
	  /// <param name="key"> The name of a node. 
	  /// </param>
	  /// <returns> The head of a linked list that contains all possible match pattern to 
	  /// template associations for the given key. </returns>
	  private TemplateSubPatternAssociation getHead(string key)
	  {
		return (TemplateSubPatternAssociation) m_patternTable[key];
	  }

	  /// <summary>
	  /// Get the head of the assocation list that is keyed by target.
	  /// </summary>
	  /// <param name="key"> </param>
	  /// <param name="assoc"> </param>
	  private void putHead(string key, TemplateSubPatternAssociation assoc)
	  {

		if (key.Equals(PsuedoNames.PSEUDONAME_TEXT))
		{
		  m_textPatterns = assoc;
		}
		else if (key.Equals(PsuedoNames.PSEUDONAME_ROOT))
		{
		  m_docPatterns = assoc;
		}
		else if (key.Equals(PsuedoNames.PSEUDONAME_COMMENT))
		{
		  m_commentPatterns = assoc;
		}

		m_patternTable[key] = assoc;
	  }

	  /// <summary>
	  /// An inner class used by a compiler to iterate over all of the ElemTemplates
	  /// stored in this TemplateList.  The compiler can replace returned templates
	  /// with their compiled equivalent.
	  /// </summary>
	  public class TemplateWalker
	  {
		  private readonly TemplateList outerInstance;

		internal System.Collections.IEnumerator hashIterator;
		internal bool inPatterns;
		internal TemplateSubPatternAssociation curPattern;

		internal Hashtable m_compilerCache = new Hashtable();

		internal TemplateWalker(TemplateList outerInstance)
		{
			this.outerInstance = outerInstance;
		  hashIterator = outerInstance.m_patternTable.Values.GetEnumerator();
		  inPatterns = true;
		  curPattern = null;
		}

		public virtual ElemTemplate next()
		{

		  ElemTemplate retValue = null;
		  ElemTemplate ct;

		  while (true)
		  {
			if (inPatterns)
			{
			  if (null != curPattern)
			  {
				curPattern = curPattern.Next;
			  }

			  if (null != curPattern)
			  {
				retValue = curPattern.Template;
			  }
			  else
			  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				if (hashIterator.hasMoreElements())
				{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				  curPattern = (TemplateSubPatternAssociation) hashIterator.nextElement();
				  retValue = curPattern.Template;
				}
				else
				{
				  inPatterns = false;
				  hashIterator = outerInstance.m_namedTemplates.Values.GetEnumerator();
				}
			  }
			}

			if (!inPatterns)
			{
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			  if (hashIterator.hasMoreElements())
			  {
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				retValue = (ElemTemplate) hashIterator.nextElement();
			  }
			  else
			  {
				return null;
			  }
			}

			ct = (ElemTemplate) m_compilerCache[new int?(retValue.Uid)];
			if (null == ct)
			{
			  m_compilerCache[new int?(retValue.Uid)] = retValue;
			  return retValue;
			}
		  }
		}
	  }

	}

}