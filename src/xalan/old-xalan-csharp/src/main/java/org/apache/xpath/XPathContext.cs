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
 * $Id: XPathContext.java 524809 2007-04-02 15:51:51Z zongaro $
 */
namespace org.apache.xpath
{



	using ExpressionContext = org.apache.xalan.extensions.ExpressionContext;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using SAX2RTFDTM = org.apache.xml.dtm.@ref.sax2dtm.SAX2RTFDTM;
	using IntStack = org.apache.xml.utils.IntStack;
	using NodeVector = org.apache.xml.utils.NodeVector;
	using ObjectStack = org.apache.xml.utils.ObjectStack;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using SAXSourceLocator = org.apache.xml.utils.SAXSourceLocator;
	using XMLString = org.apache.xml.utils.XMLString;
	using SubContextList = org.apache.xpath.axes.SubContextList;
	using XObject = org.apache.xpath.objects.XObject;
	using DTMXRTreeFrag = org.apache.xpath.objects.DTMXRTreeFrag;
	using XString = org.apache.xpath.objects.XString;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	using XMLReader = org.xml.sax.XMLReader;

	/// <summary>
	/// Default class for the runtime execution context for XPath.
	/// 
	/// <para>This class extends DTMManager but does not directly implement it.</para>
	/// @xsl.usage advanced
	/// </summary>
	public class XPathContext : DTMManager // implements ExpressionContext
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			expressionContext = new XPathExpressionContext(this);
		}

		internal IntStack m_last_pushed_rtfdtm = new IntStack();
	  /// <summary>
	  /// Stack of cached "reusable" DTMs for Result Tree Fragments.
	  /// This is a kluge to handle the problem of starting an RTF before
	  /// the old one is complete.
	  /// 
	  /// %REVIEW% I'm using a Vector rather than Stack so we can reuse
	  /// the DTMs if the problem occurs multiple times. I'm not sure that's
	  /// really a net win versus discarding the DTM and starting a new one...
	  /// but the retained RTF DTM will have been tail-pruned so should be small.
	  /// </summary>
	  private ArrayList m_rtfdtm_stack = null;
	  /// <summary>
	  /// Index of currently active RTF DTM in m_rtfdtm_stack </summary>
	  private int m_which_rtfdtm = -1;

	 /// <summary>
	 /// Most recent "reusable" DTM for Global Result Tree Fragments. No stack is
	 /// required since we're never going to pop these.
	 /// </summary>
	  private SAX2RTFDTM m_global_rtfdtm = null;

	  /// <summary>
	  /// HashMap of cached the DTMXRTreeFrag objects, which are identified by DTM IDs.
	  /// The object are just wrappers for DTMs which are used in  XRTreeFrag.
	  /// </summary>
	  private Hashtable m_DTMXRTreeFrags = null;

	  /// <summary>
	  /// state of the secure processing feature.
	  /// </summary>
	  private bool m_isSecureProcessing = false;

	  /// <summary>
	  /// Though XPathContext context extends 
	  /// the DTMManager, it really is a proxy for this object, which 
	  /// is the real DTMManager.
	  /// </summary>
	  protected internal DTMManager m_dtmManager = DTMManager.newInstance(org.apache.xpath.objects.XMLStringFactoryImpl.Factory);

	  /// <summary>
	  /// Return the DTMManager object.  Though XPathContext context extends 
	  /// the DTMManager, it really is a proxy for the real DTMManager.  If a 
	  /// caller needs to make a lot of calls to the DTMManager, it is faster 
	  /// if it gets the real one from this function.
	  /// </summary>
	   public virtual DTMManager DTMManager
	   {
		   get
		   {
			 return m_dtmManager;
		   }
	   }

	  /// <summary>
	  /// Set the state of the secure processing feature
	  /// </summary>
	  public virtual bool SecureProcessing
	  {
		  set
		  {
			m_isSecureProcessing = value;
		  }
		  get
		  {
			return m_isSecureProcessing;
		  }
	  }


	  /// <summary>
	  /// Get an instance of a DTM, loaded with the content from the
	  /// specified source.  If the unique flag is true, a new instance will
	  /// always be returned.  Otherwise it is up to the DTMManager to return a
	  /// new instance or an instance that it already created and may be being used
	  /// by someone else.
	  /// (I think more parameters will need to be added for error handling, and entity
	  /// resolution).
	  /// </summary>
	  /// <param name="source"> the specification of the source object, which may be null, 
	  ///               in which case it is assumed that node construction will take 
	  ///               by some other means. </param>
	  /// <param name="unique"> true if the returned DTM must be unique, probably because it
	  /// is going to be mutated. </param>
	  /// <param name="wsfilter"> Enables filtering of whitespace nodes, and may be null. </param>
	  /// <param name="incremental"> true if the construction should try and be incremental. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use 
	  ///                   indexing schemes.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public override DTM getDTM(javax.xml.transform.Source source, bool unique, DTMWSFilter wsfilter, bool incremental, bool doIndexing)
	  {
		return m_dtmManager.getDTM(source, unique, wsfilter, incremental, doIndexing);
	  }

	  /// <summary>
	  /// Get an instance of a DTM that "owns" a node handle. 
	  /// </summary>
	  /// <param name="nodeHandle"> the nodeHandle.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public override DTM getDTM(int nodeHandle)
	  {
		return m_dtmManager.getDTM(nodeHandle);
	  }

	  /// <summary>
	  /// Given a W3C DOM node, try and return a DTM handle.
	  /// Note: calling this may be non-optimal.
	  /// </summary>
	  /// <param name="node"> Non-null reference to a DOM node.
	  /// </param>
	  /// <returns> a valid DTM handle. </returns>
	  public override int getDTMHandleFromNode(org.w3c.dom.Node node)
	  {
		return m_dtmManager.getDTMHandleFromNode(node);
	  }
	//
	//  
	  /// <summary>
	  /// %TBD% Doc
	  /// </summary>
	  public override int getDTMIdentity(DTM dtm)
	  {
		return m_dtmManager.getDTMIdentity(dtm);
	  }
	//  
	  /// <summary>
	  /// Creates an empty <code>DocumentFragment</code> object. </summary>
	  /// <returns> A new <code>DocumentFragment handle</code>. </returns>
	  public override DTM createDocumentFragment()
	  {
		return m_dtmManager.createDocumentFragment();
	  }
	//  
	  /// <summary>
	  /// Release a DTM either to a lru pool, or completely remove reference.
	  /// DTMs without system IDs are always hard deleted.
	  /// State: experimental.
	  /// </summary>
	  /// <param name="dtm"> The DTM to be released. </param>
	  /// <param name="shouldHardDelete"> True if the DTM should be removed no matter what. </param>
	  /// <returns> true if the DTM was removed, false if it was put back in a lru pool. </returns>
	  public override bool release(DTM dtm, bool shouldHardDelete)
	  {
		// %REVIEW% If it's a DTM which may contain multiple Result Tree
		// Fragments, we can't discard it unless we know not only that it
		// is empty, but that the XPathContext itself is going away. So do
		// _not_ accept the request. (May want to do it as part of
		// reset(), though.)
		if (m_rtfdtm_stack != null && m_rtfdtm_stack.Contains(dtm))
		{
		  return false;
		}

		return m_dtmManager.release(dtm, shouldHardDelete);
	  }

	  /// <summary>
	  /// Create a new <code>DTMIterator</code> based on an XPath
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or
	  /// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	  /// </summary>
	  /// <param name="xpathCompiler"> ??? Somehow we need to pass in a subpart of the
	  /// expression.  I hate to do this with strings, since the larger expression
	  /// has already been parsed.
	  /// </param>
	  /// <param name="pos"> The position in the expression. </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public override DTMIterator createDTMIterator(object xpathCompiler, int pos)
	  {
		return m_dtmManager.createDTMIterator(xpathCompiler, pos);
	  }
	//
	  /// <summary>
	  /// Create a new <code>DTMIterator</code> based on an XPath
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or
	  /// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	  /// </summary>
	  /// <param name="xpathString"> Must be a valid string expressing a
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or
	  /// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	  /// </param>
	  /// <param name="presolver"> An object that can resolve prefixes to namespace URLs.
	  /// </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public override DTMIterator createDTMIterator(string xpathString, PrefixResolver presolver)
	  {
		return m_dtmManager.createDTMIterator(xpathString, presolver);
	  }
	//
	  /// <summary>
	  /// Create a new <code>DTMIterator</code> based only on a whatToShow and
	  /// a DTMFilter.  The traversal semantics are defined as the descendant
	  /// access.
	  /// </summary>
	  /// <param name="whatToShow"> This flag specifies which node types may appear in
	  ///   the logical view of the tree presented by the iterator. See the
	  ///   description of <code>NodeFilter</code> for the set of possible
	  ///   <code>SHOW_</code> values.These flags can be combined using
	  ///   <code>OR</code>. </param>
	  /// <param name="filter"> The <code>NodeFilter</code> to be used with this
	  ///   <code>TreeWalker</code>, or <code>null</code> to indicate no filter. </param>
	  /// <param name="entityReferenceExpansion"> The value of this flag determines
	  ///   whether entity reference nodes are expanded.
	  /// </param>
	  /// <returns> The newly created <code>NodeIterator</code>. </returns>
	  public override DTMIterator createDTMIterator(int whatToShow, DTMFilter filter, bool entityReferenceExpansion)
	  {
		return m_dtmManager.createDTMIterator(whatToShow, filter, entityReferenceExpansion);
	  }

	  /// <summary>
	  /// Create a new <code>DTMIterator</code> that holds exactly one node.
	  /// </summary>
	  /// <param name="node"> The node handle that the DTMIterator will iterate to.
	  /// </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public override DTMIterator createDTMIterator(int node)
	  {
		// DescendantIterator iter = new DescendantIterator();
		DTMIterator iter = new org.apache.xpath.axes.OneStepIteratorForward(Axis.SELF);
		iter.setRoot(node, this);
		return iter;
		// return m_dtmManager.createDTMIterator(node);
	  }

	  /// <summary>
	  /// Create an XPathContext instance.  This is equivalent to calling
	  /// the <seealso cref="#XPathContext(boolean)"/> constructor with the value
	  /// <code>true</code>.
	  /// </summary>
	  public XPathContext() : this(true)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  /// <summary>
	  /// Create an XPathContext instance. </summary>
	  /// <param name="recursiveVarContext"> A <code>boolean</code> value indicating whether
	  ///             the XPath context needs to support pushing of scopes for
	  ///             variable resolution </param>
	  public XPathContext(bool recursiveVarContext)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		m_prefixResolvers.push(null);
		m_currentNodes.push(org.apache.xml.dtm.DTM_Fields.NULL);
		m_currentExpressionNodes.push(org.apache.xml.dtm.DTM_Fields.NULL);
		m_saxLocations.push(null);
		m_variableStacks = recursiveVarContext ? new VariableStack() : new VariableStack(1);
	  }

	  /// <summary>
	  /// Create an XPathContext instance.  This is equivalent to calling the
	  /// constructor <seealso cref="#XPathContext(java.lang.Object,boolean)"/> with the
	  /// value of the second parameter set to <code>true</code>. </summary>
	  /// <param name="owner"> Value that can be retrieved via the getOwnerObject() method. </param>
	  /// <seealso cref= #getOwnerObject </seealso>
	  public XPathContext(object owner) : this(owner, true)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  /// <summary>
	  /// Create an XPathContext instance. </summary>
	  /// <param name="owner"> Value that can be retrieved via the getOwnerObject() method. </param>
	  /// <seealso cref= #getOwnerObject </seealso>
	  /// <param name="recursiveVarContext"> A <code>boolean</code> value indicating whether
	  ///             the XPath context needs to support pushing of scopes for
	  ///             variable resolution </param>
	  public XPathContext(object owner, bool recursiveVarContext) : this(recursiveVarContext)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		m_owner = owner;
		try
		{
		  m_ownerGetErrorListener = m_owner.GetType().GetMethod("getErrorListener", new Type[] {});
		}
		catch (NoSuchMethodException)
		{
		}
	  }

	  /// <summary>
	  /// Reset for new run.
	  /// </summary>
	  public virtual void reset()
	  {
		releaseDTMXRTreeFrags();
		  // These couldn't be disposed of earlier (see comments in release()); zap them now.
		  if (m_rtfdtm_stack != null)
		  {
			   for (System.Collections.IEnumerator e = m_rtfdtm_stack.elements(); e.MoveNext();)
			   {
				   m_dtmManager.release((DTM)e.Current, true);
			   }
		  }

		m_rtfdtm_stack = null; // drop our references too
		m_which_rtfdtm = -1;

		if (m_global_rtfdtm != null)
		{
				   m_dtmManager.release(m_global_rtfdtm,true);
		}
		m_global_rtfdtm = null;


		m_dtmManager = DTMManager.newInstance(org.apache.xpath.objects.XMLStringFactoryImpl.Factory);

		m_saxLocations.removeAllElements();
		m_axesIteratorStack.removeAllElements();
		m_contextNodeLists.removeAllElements();
		m_currentExpressionNodes.removeAllElements();
		m_currentNodes.removeAllElements();
		m_iteratorRoots.RemoveAllNoClear();
		m_predicatePos.removeAllElements();
		m_predicateRoots.RemoveAllNoClear();
		m_prefixResolvers.removeAllElements();

		m_prefixResolvers.push(null);
		m_currentNodes.push(org.apache.xml.dtm.DTM_Fields.NULL);
		m_currentExpressionNodes.push(org.apache.xml.dtm.DTM_Fields.NULL);
		m_saxLocations.push(null);
	  }

	  /// <summary>
	  /// The current stylesheet locator. </summary>
	  internal ObjectStack m_saxLocations = new ObjectStack(RECURSIONLIMIT);

	  /// <summary>
	  /// Set the current locater in the stylesheet.
	  /// </summary>
	  /// <param name="location"> The location within the stylesheet. </param>
	  public virtual SourceLocator SAXLocator
	  {
		  set
		  {
			m_saxLocations.Top = value;
		  }
		  get
		  {
			return (SourceLocator) m_saxLocations.peek();
		  }
	  }

	  /// <summary>
	  /// Set the current locater in the stylesheet.
	  /// </summary>
	  /// <param name="location"> The location within the stylesheet. </param>
	  public virtual void pushSAXLocator(SourceLocator location)
	  {
		m_saxLocations.push(location);
	  }

	  /// <summary>
	  /// Push a slot on the locations stack so that setSAXLocator can be 
	  /// repeatedly called.
	  /// 
	  /// </summary>
	  public virtual void pushSAXLocatorNull()
	  {
		m_saxLocations.push(null);
	  }


	  /// <summary>
	  /// Pop the current locater.
	  /// </summary>
	  public virtual void popSAXLocator()
	  {
		m_saxLocations.pop();
	  }


	  /// <summary>
	  /// The owner context of this XPathContext.  In the case of XSLT, this will be a
	  ///  Transformer object.
	  /// </summary>
	  private object m_owner;

	  /// <summary>
	  /// The owner context of this XPathContext.  In the case of XSLT, this will be a
	  ///  Transformer object.
	  /// </summary>
	  private Method m_ownerGetErrorListener;

	  /// <summary>
	  /// Get the "owner" context of this context, which should be,
	  /// in the case of XSLT, the Transformer object.  This is needed
	  /// so that XSLT functions can get the Transformer. </summary>
	  /// <returns> The owner object passed into the constructor, or null. </returns>
	  public virtual object OwnerObject
	  {
		  get
		  {
			return m_owner;
		  }
	  }

	  // ================ VarStack ===================

	  /// <summary>
	  /// The stack of Variable stacks.  A VariableStack will be
	  /// pushed onto this stack for each template invocation.
	  /// </summary>
	  private VariableStack m_variableStacks;

	  /// <summary>
	  /// Get the variable stack, which is in charge of variables and
	  /// parameters.
	  /// </summary>
	  /// <returns> the variable stack, which should not be null. </returns>
	  public VariableStack VarStack
	  {
		  get
		  {
			return m_variableStacks;
		  }
		  set
		  {
			m_variableStacks = value;
		  }
	  }


	  // ================ SourceTreeManager ===================

	  /// <summary>
	  /// The source tree manager, which associates Source objects to source 
	  ///  tree nodes. 
	  /// </summary>
	  private SourceTreeManager m_sourceTreeManager = new SourceTreeManager();

	  /// <summary>
	  /// Get the SourceTreeManager associated with this execution context.
	  /// </summary>
	  /// <returns> the SourceTreeManager associated with this execution context. </returns>
	  public SourceTreeManager SourceTreeManager
	  {
		  get
		  {
			return m_sourceTreeManager;
		  }
		  set
		  {
			m_sourceTreeManager = value;
		  }
	  }


	  // =================================================

	  /// <summary>
	  /// The ErrorListener where errors and warnings are to be reported. </summary>
	  private ErrorListener m_errorListener;

	  /// <summary>
	  /// A default ErrorListener in case our m_errorListener was not specified and our
	  ///  owner either does not have an ErrorListener or has a null one.
	  /// </summary>
	  private ErrorListener m_defaultErrorListener;

	  /// <summary>
	  /// Get the ErrorListener where errors and warnings are to be reported.
	  /// </summary>
	  /// <returns> A non-null ErrorListener reference. </returns>
	  public ErrorListener ErrorListener
	  {
		  get
		  {
    
			if (null != m_errorListener)
			{
				return m_errorListener;
			}
    
			ErrorListener retval = null;
    
			try
			{
			  if (null != m_ownerGetErrorListener)
			  {
				retval = (ErrorListener) m_ownerGetErrorListener.invoke(m_owner, new object[] {});
			  }
			}
			catch (Exception)
			{
			}
    
			if (null == retval)
			{
			  if (null == m_defaultErrorListener)
			  {
				m_defaultErrorListener = new org.apache.xml.utils.DefaultErrorHandler();
			  }
			  retval = m_defaultErrorListener;
			}
    
			return retval;
		  }
		  set
		  {
			if (value == null)
			{
			  throw new System.ArgumentException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, null)); //"Null error handler");
			}
			m_errorListener = value;
		  }
	  }



	  // =================================================

	  /// <summary>
	  /// The TrAX URI Resolver for resolving URIs from the document(...)
	  ///  function to source tree nodes.  
	  /// </summary>
	  private URIResolver m_uriResolver;

	  /// <summary>
	  /// Get the URIResolver associated with this execution context.
	  /// </summary>
	  /// <returns> a URI resolver, which may be null. </returns>
	  public URIResolver URIResolver
	  {
		  get
		  {
			return m_uriResolver;
		  }
		  set
		  {
			m_uriResolver = value;
		  }
	  }


	  // =================================================

	  /// <summary>
	  /// The reader of the primary source tree. </summary>
	  public XMLReader m_primaryReader;

	  /// <summary>
	  /// Get primary XMLReader associated with this execution context.
	  /// </summary>
	  /// <returns> The reader of the primary source tree. </returns>
	  public XMLReader PrimaryReader
	  {
		  get
		  {
			return m_primaryReader;
		  }
		  set
		  {
			m_primaryReader = value;
		  }
	  }


	  // =================================================


	  /// <summary>
	  /// Misnamed string manager for XPath messages. </summary>
	  // private static XSLMessages m_XSLMessages = new XSLMessages();

	  /// <summary>
	  /// Tell the user of an assertion error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="b">  If false, a TransformerException will be thrown. </param>
	  /// <param name="msg"> The assertion message, which should be informative.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> if b is false. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void assertion(boolean b, String msg) throws javax.xml.transform.TransformerException
	  private void assertion(bool b, string msg)
	  {
		if (!b)
		{
		  ErrorListener errorHandler = ErrorListener;

		  if (errorHandler != null)
		  {
			errorHandler.fatalError(new TransformerException(XSLMessages.createMessage(XPATHErrorResources.ER_INCORRECT_PROGRAMMER_ASSERTION, new object[]{msg}), (SAXSourceLocator)this.SAXLocator));
		  }
		}
	  }

	  //==========================================================
	  // SECTION: Execution context state tracking
	  //==========================================================

	  /// <summary>
	  /// The current context node list.
	  /// </summary>
	  private Stack m_contextNodeLists = new Stack();

	  public virtual Stack ContextNodeListsStack
	  {
		  get
		  {
			  return m_contextNodeLists;
		  }
		  set
		  {
			  m_contextNodeLists = value;
		  }
	  }

	  /// <summary>
	  /// Get the current context node list.
	  /// </summary>
	  /// <returns>  the <a href="http://www.w3.org/TR/xslt#dt-current-node-list">current node list</a>,
	  /// also refered to here as a <term>context node list</term>. </returns>
	  public DTMIterator ContextNodeList
	  {
		  get
		  {
    
			if (m_contextNodeLists.Count > 0)
			{
			  return (DTMIterator) m_contextNodeLists.Peek();
			}
			else
			{
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// Set the current context node list.
	  /// </summary>
	  /// <param name="nl"> the <a href="http://www.w3.org/TR/xslt#dt-current-node-list">current node list</a>,
	  /// also refered to here as a <term>context node list</term>.
	  /// @xsl.usage internal </param>
	  public void pushContextNodeList(DTMIterator nl)
	  {
		m_contextNodeLists.Push(nl);
	  }

	  /// <summary>
	  /// Pop the current context node list.
	  /// @xsl.usage internal
	  /// </summary>
	  public void popContextNodeList()
	  {
		  if (m_contextNodeLists.Count == 0)
		  {
			Console.Error.WriteLine("Warning: popContextNodeList when stack is empty!");
		  }
		  else
		  {
		  m_contextNodeLists.Pop();
		  }
	  }

	  /// <summary>
	  /// The ammount to use for stacks that record information during the 
	  /// recursive execution.
	  /// </summary>
	  public const int RECURSIONLIMIT = (1024 * 4);

	  /// <summary>
	  /// The stack of <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a> objects.
	  ///  Not to be confused with the current node list.  %REVIEW% Note that there 
	  ///  are no bounds check and resize for this stack, so if it is blown, it's all 
	  ///  over.  
	  /// </summary>
	  private IntStack m_currentNodes = new IntStack(RECURSIONLIMIT);

	//  private NodeVector m_currentNodes = new NodeVector();

	  public virtual IntStack CurrentNodeStack
	  {
		  get
		  {
			  return m_currentNodes;
		  }
		  set
		  {
			  m_currentNodes = value;
		  }
	  }

	  /// <summary>
	  /// Get the current context node.
	  /// </summary>
	  /// <returns> the <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a>. </returns>
	  public int CurrentNode
	  {
		  get
		  {
			return m_currentNodes.peek();
		  }
	  }

	  /// <summary>
	  /// Set the current context node and expression node.
	  /// </summary>
	  /// <param name="cn"> the <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a>. </param>
	  /// <param name="en"> the sub-expression context node. </param>
	  public void pushCurrentNodeAndExpression(int cn, int en)
	  {
		m_currentNodes.push(cn);
		m_currentExpressionNodes.push(cn);
	  }

	  /// <summary>
	  /// Set the current context node.
	  /// </summary>
	  public void popCurrentNodeAndExpression()
	  {
		m_currentNodes.quickPop(1);
		m_currentExpressionNodes.quickPop(1);
	  }

	  /// <summary>
	  /// Push the current context node, expression node, and prefix resolver.
	  /// </summary>
	  /// <param name="cn"> the <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a>. </param>
	  /// <param name="en"> the sub-expression context node. </param>
	  /// <param name="nc"> the namespace context (prefix resolver. </param>
	  public void pushExpressionState(int cn, int en, PrefixResolver nc)
	  {
		m_currentNodes.push(cn);
		m_currentExpressionNodes.push(cn);
		m_prefixResolvers.push(nc);
	  }

	  /// <summary>
	  /// Pop the current context node, expression node, and prefix resolver.
	  /// </summary>
	  public void popExpressionState()
	  {
		m_currentNodes.quickPop(1);
		m_currentExpressionNodes.quickPop(1);
		m_prefixResolvers.pop();
	  }



	  /// <summary>
	  /// Set the current context node.
	  /// </summary>
	  /// <param name="n"> the <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a>. </param>
	  public void pushCurrentNode(int n)
	  {
		m_currentNodes.push(n);
	  }

	  /// <summary>
	  /// Pop the current context node.
	  /// </summary>
	  public void popCurrentNode()
	  {
		m_currentNodes.quickPop(1);
	  }

	  /// <summary>
	  /// Set the current predicate root.
	  /// </summary>
	  public void pushPredicateRoot(int n)
	  {
		m_predicateRoots.push(n);
	  }

	  /// <summary>
	  /// Pop the current predicate root.
	  /// </summary>
	  public void popPredicateRoot()
	  {
		m_predicateRoots.popQuick();
	  }

	  /// <summary>
	  /// Get the current predicate root.
	  /// </summary>
	  public int PredicateRoot
	  {
		  get
		  {
			return m_predicateRoots.peepOrNull();
		  }
	  }

	  /// <summary>
	  /// Set the current location path iterator root.
	  /// </summary>
	  public void pushIteratorRoot(int n)
	  {
		m_iteratorRoots.push(n);
	  }

	  /// <summary>
	  /// Pop the current location path iterator root.
	  /// </summary>
	  public void popIteratorRoot()
	  {
		m_iteratorRoots.popQuick();
	  }

	  /// <summary>
	  /// Get the current location path iterator root.
	  /// </summary>
	  public int IteratorRoot
	  {
		  get
		  {
			return m_iteratorRoots.peepOrNull();
		  }
	  }

	  /// <summary>
	  /// A stack of the current sub-expression nodes. </summary>
	  private NodeVector m_iteratorRoots = new NodeVector();

	  /// <summary>
	  /// A stack of the current sub-expression nodes. </summary>
	  private NodeVector m_predicateRoots = new NodeVector();

	  /// <summary>
	  /// A stack of the current sub-expression nodes. </summary>
	  private IntStack m_currentExpressionNodes = new IntStack(RECURSIONLIMIT);


	  public virtual IntStack CurrentExpressionNodeStack
	  {
		  get
		  {
			  return m_currentExpressionNodes;
		  }
		  set
		  {
			  m_currentExpressionNodes = value;
		  }
	  }

	  private IntStack m_predicatePos = new IntStack();

	  public int PredicatePos
	  {
		  get
		  {
			return m_predicatePos.peek();
		  }
	  }

	  public void pushPredicatePos(int n)
	  {
		m_predicatePos.push(n);
	  }

	  public void popPredicatePos()
	  {
		m_predicatePos.pop();
	  }

	  /// <summary>
	  /// Get the current node that is the expression's context (i.e. for current() support).
	  /// </summary>
	  /// <returns> The current sub-expression node. </returns>
	  public int CurrentExpressionNode
	  {
		  get
		  {
			return m_currentExpressionNodes.peek();
		  }
	  }

	  /// <summary>
	  /// Set the current node that is the expression's context (i.e. for current() support).
	  /// </summary>
	  /// <param name="n"> The sub-expression node to be current. </param>
	  public void pushCurrentExpressionNode(int n)
	  {
		m_currentExpressionNodes.push(n);
	  }

	  /// <summary>
	  /// Pop the current node that is the expression's context 
	  /// (i.e. for current() support).
	  /// </summary>
	  public void popCurrentExpressionNode()
	  {
		m_currentExpressionNodes.quickPop(1);
	  }

	  private ObjectStack m_prefixResolvers = new ObjectStack(RECURSIONLIMIT);

	  /// <summary>
	  /// Get the current namespace context for the xpath.
	  /// </summary>
	  /// <returns> the current prefix resolver for resolving prefixes to 
	  ///         namespace URLs. </returns>
	  public PrefixResolver NamespaceContext
	  {
		  get
		  {
			return (PrefixResolver) m_prefixResolvers.peek();
		  }
		  set
		  {
			m_prefixResolvers.Top = value;
		  }
	  }


	  /// <summary>
	  /// Push a current namespace context for the xpath.
	  /// </summary>
	  /// <param name="pr"> the prefix resolver to be used for resolving prefixes to 
	  ///         namespace URLs. </param>
	  public void pushNamespaceContext(PrefixResolver pr)
	  {
		m_prefixResolvers.push(pr);
	  }

	  /// <summary>
	  /// Just increment the namespace contest stack, so that setNamespaceContext
	  /// can be used on the slot.
	  /// </summary>
	  public void pushNamespaceContextNull()
	  {
		m_prefixResolvers.push(null);
	  }

	  /// <summary>
	  /// Pop the current namespace context for the xpath.
	  /// </summary>
	  public void popNamespaceContext()
	  {
		m_prefixResolvers.pop();
	  }

	  //==========================================================
	  // SECTION: Current TreeWalker contexts (for internal use)
	  //==========================================================

	  /// <summary>
	  /// Stack of AxesIterators.
	  /// </summary>
	  private Stack m_axesIteratorStack = new Stack();

	  public virtual Stack AxesIteratorStackStacks
	  {
		  get
		  {
			  return m_axesIteratorStack;
		  }
		  set
		  {
			  m_axesIteratorStack = value;
		  }
	  }

	  /// <summary>
	  /// Push a TreeWalker on the stack.
	  /// </summary>
	  /// <param name="iter"> A sub-context AxesWalker.
	  /// @xsl.usage internal </param>
	  public void pushSubContextList(SubContextList iter)
	  {
		m_axesIteratorStack.Push(iter);
	  }

	  /// <summary>
	  /// Pop the last pushed axes iterator.
	  /// @xsl.usage internal
	  /// </summary>
	  public void popSubContextList()
	  {
		m_axesIteratorStack.Pop();
	  }

	  /// <summary>
	  /// Get the current axes iterator, or return null if none.
	  /// </summary>
	  /// <returns> the sub-context node list.
	  /// @xsl.usage internal </returns>
	  public virtual SubContextList SubContextList
	  {
		  get
		  {
			return m_axesIteratorStack.Count == 0 ? null : (SubContextList) m_axesIteratorStack.Peek();
		  }
	  }

	  /// <summary>
	  /// Get the <a href="http://www.w3.org/TR/xslt#dt-current-node-list">current node list</a> 
	  /// as defined by the XSLT spec.
	  /// </summary>
	  /// <returns> the <a href="http://www.w3.org/TR/xslt#dt-current-node-list">current node list</a>.
	  /// @xsl.usage internal </returns>

	  public virtual SubContextList CurrentNodeList
	  {
		  get
		  {
			return m_axesIteratorStack.Count == 0 ? null : (SubContextList) m_axesIteratorStack.elementAt(0);
		  }
	  }
	  //==========================================================
	  // SECTION: Implementation of ExpressionContext interface
	  //==========================================================

	  /// <summary>
	  /// Get the current context node. </summary>
	  /// <returns> The current context node. </returns>
	  public int ContextNode
	  {
		  get
		  {
			return this.CurrentNode;
		  }
	  }

	  /// <summary>
	  /// Get the current context node list. </summary>
	  /// <returns> An iterator for the current context list, as
	  /// defined in XSLT. </returns>
	  public DTMIterator ContextNodes
	  {
		  get
		  {
    
			try
			{
			  DTMIterator cnl = ContextNodeList;
    
			  if (null != cnl)
			  {
				return cnl.cloneWithReset();
			  }
			  else
			  {
				return null; // for now... this might ought to be an empty iterator.
			  }
			}
			catch (CloneNotSupportedException)
			{
			  return null; // error reporting?
			}
		  }
	  }

	  internal XPathExpressionContext expressionContext;

	  /// <summary>
	  /// The the expression context for extensions for this context.
	  /// </summary>
	  /// <returns> An object that implements the ExpressionContext. </returns>
	  public virtual ExpressionContext ExpressionContext
	  {
		  get
		  {
			return expressionContext;
		  }
	  }

	  public class XPathExpressionContext : ExpressionContext
	  {
		  private readonly XPathContext outerInstance;

		  public XPathExpressionContext(XPathContext outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <summary>
		/// Return the XPathContext associated with this XPathExpressionContext.
		/// Extensions should use this judiciously and only when special processing
		/// requirements cannot be met another way.  Consider requesting an enhancement
		/// to the ExpressionContext interface to avoid having to call this method. </summary>
		/// <returns> the XPathContext associated with this XPathExpressionContext. </returns>
		 public virtual XPathContext XPathContext
		 {
			 get
			 {
			   return outerInstance;
			 }
		 }

		/// <summary>
		/// Return the DTMManager object.  Though XPathContext context extends 
		/// the DTMManager, it really is a proxy for the real DTMManager.  If a 
		/// caller needs to make a lot of calls to the DTMManager, it is faster 
		/// if it gets the real one from this function.
		/// </summary>
		 public virtual DTMManager DTMManager
		 {
			 get
			 {
			   return outerInstance.m_dtmManager;
			 }
		 }

		/// <summary>
		/// Get the current context node. </summary>
		/// <returns> The current context node. </returns>
		public virtual org.w3c.dom.Node ContextNode
		{
			get
			{
			  int context = outerInstance.CurrentNode;
    
			  return outerInstance.getDTM(context).getNode(context);
			}
		}

		/// <summary>
		/// Get the current context node list. </summary>
		/// <returns> An iterator for the current context list, as
		/// defined in XSLT. </returns>
		public virtual org.w3c.dom.traversal.NodeIterator ContextNodes
		{
			get
			{
			  return new org.apache.xml.dtm.@ref.DTMNodeIterator(outerInstance.ContextNodeList);
			}
		}

		/// <summary>
		/// Get the error listener. </summary>
		/// <returns> The registered error listener. </returns>
		public virtual ErrorListener ErrorListener
		{
			get
			{
			  return outerInstance.ErrorListener;
			}
		}

		/// <summary>
		/// Get the value of a node as a number. </summary>
		/// <param name="n"> Node to be converted to a number.  May be null. </param>
		/// <returns> value of n as a number. </returns>
		public virtual double toNumber(org.w3c.dom.Node n)
		{
		  // %REVIEW% You can't get much uglier than this...
		  int nodeHandle = outerInstance.getDTMHandleFromNode(n);
		  DTM dtm = outerInstance.getDTM(nodeHandle);
		  XString xobj = (XString)dtm.getStringValue(nodeHandle);
		  return xobj.num();
		}

		/// <summary>
		/// Get the value of a node as a string. </summary>
		/// <param name="n"> Node to be converted to a string.  May be null. </param>
		/// <returns> value of n as a string, or an empty string if n is null. </returns>
		public virtual string ToString(org.w3c.dom.Node n)
		{
		  // %REVIEW% You can't get much uglier than this...
		  int nodeHandle = outerInstance.getDTMHandleFromNode(n);
		  DTM dtm = outerInstance.getDTM(nodeHandle);
		  XMLString strVal = dtm.getStringValue(nodeHandle);
		  return strVal.ToString();
		}

		/// <summary>
		/// Get a variable based on it's qualified name. </summary>
		/// <param name="qname"> The qualified name of the variable. </param>
		/// <returns> The evaluated value of the variable. </returns>
		/// <exception cref="javax.xml.transform.TransformerException"> </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.apache.xpath.objects.XObject getVariableOrParam(org.apache.xml.utils.QName qname) throws javax.xml.transform.TransformerException
		public XObject getVariableOrParam(org.apache.xml.utils.QName qname)
		{
		  return outerInstance.m_variableStacks.getVariableOrParam(outerInstance, qname);
		}

	  }

	 /// <summary>
	 /// Get a DTM to be used as a container for a global Result Tree
	 /// Fragment. This will always be an instance of (derived from? equivalent to?) 
	 /// SAX2DTM, since each RTF is constructed by temporarily redirecting our SAX 
	 /// output to it. It may be a single DTM containing for multiple fragments, 
	 /// if the implementation supports that.
	 /// 
	 /// Note: The distinction between this method and getRTFDTM() is that the latter
	 /// allocates space from the dynamic variable stack (m_rtfdtm_stack), which may
	 /// be pruned away again as the templates which defined those variables are exited.
	 /// Global variables may be bound late (see XUnresolvedVariable), and never want to
	 /// be discarded, hence we need to allocate them separately and don't actually need
	 /// a stack to track them.
	 /// </summary>
	 /// <returns> a non-null DTM reference. </returns>
	  public virtual DTM GlobalRTFDTM
	  {
		  get
		  {
			  // We probably should _NOT_ be applying whitespace filtering at this stage!
			  //
			  // Some magic has been applied in DTMManagerDefault to recognize this set of options
			  // and generate an instance of DTM which can contain multiple documents
			  // (SAX2RTFDTM). Perhaps not the optimal way of achieving that result, but
			  // I didn't want to change the manager API at this time, or expose 
			  // too many dependencies on its internals. (Ideally, I'd like to move
			  // isTreeIncomplete all the way up to DTM, so we wouldn't need to explicitly
			  // specify the subclass here.)
    
			// If it doesn't exist, or if the one already existing is in the middle of
			// being constructed, we need to obtain a new DTM to write into. I'm not sure
			// the latter will ever arise, but I'd rather be just a bit paranoid..
			if (m_global_rtfdtm == null || m_global_rtfdtm.TreeIncomplete)
			{
				  m_global_rtfdtm = (SAX2RTFDTM)m_dtmManager.getDTM(null,true,null,false,false);
			}
			return m_global_rtfdtm;
		  }
	  }




	  /// <summary>
	  /// Get a DTM to be used as a container for a dynamic Result Tree
	  /// Fragment. This will always be an instance of (derived from? equivalent to?) 
	  /// SAX2DTM, since each RTF is constructed by temporarily redirecting our SAX 
	  /// output to it. It may be a single DTM containing for multiple fragments, 
	  /// if the implementation supports that.
	  /// </summary>
	  /// <returns> a non-null DTM reference. </returns>
	  public virtual DTM RTFDTM
	  {
		  get
		  {
			  SAX2RTFDTM rtfdtm;
    
			  // We probably should _NOT_ be applying whitespace filtering at this stage!
			  //
			  // Some magic has been applied in DTMManagerDefault to recognize this set of options
			  // and generate an instance of DTM which can contain multiple documents
			  // (SAX2RTFDTM). Perhaps not the optimal way of achieving that result, but
			  // I didn't want to change the manager API at this time, or expose 
			  // too many dependencies on its internals. (Ideally, I'd like to move
			  // isTreeIncomplete all the way up to DTM, so we wouldn't need to explicitly
			  // specify the subclass here.)
    
			if (m_rtfdtm_stack == null)
			{
				m_rtfdtm_stack = new ArrayList();
				  rtfdtm = (SAX2RTFDTM)m_dtmManager.getDTM(null,true,null,false,false);
			m_rtfdtm_stack.Add(rtfdtm);
				++m_which_rtfdtm;
			}
			else if (m_which_rtfdtm < 0)
			{
				rtfdtm = (SAX2RTFDTM)m_rtfdtm_stack[++m_which_rtfdtm];
			}
			else
			{
				rtfdtm = (SAX2RTFDTM)m_rtfdtm_stack[m_which_rtfdtm];
    
				  // It might already be under construction -- the classic example would be
				  // an xsl:variable which uses xsl:call-template as part of its value. To
				  // handle this recursion, we have to start a new RTF DTM, pushing the old
				  // one onto a stack so we can return to it. This is not as uncommon a case
				  // as we might wish, unfortunately, as some folks insist on coding XSLT
				  // as if it were a procedural language...
				  if (rtfdtm.TreeIncomplete)
				  {
					  if (++m_which_rtfdtm < m_rtfdtm_stack.Count)
					  {
						rtfdtm = (SAX2RTFDTM)m_rtfdtm_stack[m_which_rtfdtm];
					  }
					  else
					  {
						  rtfdtm = (SAX2RTFDTM)m_dtmManager.getDTM(null,true,null,false,false);
				  m_rtfdtm_stack.Add(rtfdtm);
					  }
				  }
			}
    
			return rtfdtm;
		  }
	  }

	  /// <summary>
	  /// Push the RTFDTM's context mark, to allows discarding RTFs added after this
	  /// point. (If it doesn't exist we don't push, since we might still be able to 
	  /// get away with not creating it. That requires that excessive pops be harmless.)
	  /// 
	  /// </summary>
	  public virtual void pushRTFContext()
	  {
		  m_last_pushed_rtfdtm.push(m_which_rtfdtm);
		  if (null != m_rtfdtm_stack)
		  {
			  ((SAX2RTFDTM)(RTFDTM)).pushRewindMark();
		  }
	  }

	  /// <summary>
	  /// Pop the RTFDTM's context mark. This discards any RTFs added after the last
	  /// mark was set. 
	  /// 
	  /// If there is no RTF DTM, there's nothing to pop so this
	  /// becomes a no-op. If pushes were issued before this was called, we count on
	  /// the fact that popRewindMark is defined such that overpopping just resets
	  /// to empty.
	  /// 
	  /// Complicating factor: We need to handle the case of popping back to a previous
	  /// RTF DTM, if one of the weird produce-an-RTF-to-build-an-RTF cases arose.
	  /// Basically: If pop says this DTM is now empty, then return to the previous
	  /// if one exists, in whatever state we left it in. UGLY, but hopefully the
	  /// situation which forces us to consider this will arise exceedingly rarely.
	  /// 
	  /// </summary>
	  public virtual void popRTFContext()
	  {
		  int previous = m_last_pushed_rtfdtm.pop();
		  if (null == m_rtfdtm_stack)
		  {
			  return;
		  }

		  if (m_which_rtfdtm == previous)
		  {
			  if (previous >= 0) // guard against none-active
			  {
				  bool isEmpty = ((SAX2RTFDTM)(m_rtfdtm_stack[previous])).popRewindMark();
			  }
		  }
		  else
		  {
			  while (m_which_rtfdtm != previous)
			  {
			  // Empty each DTM before popping, so it's ready for reuse
			  // _DON'T_ pop the previous, since it's still open (which is why we
			  // stacked up more of these) and did not receive a mark.
			  bool isEmpty = ((SAX2RTFDTM)(m_rtfdtm_stack[m_which_rtfdtm])).popRewindMark();
			  --m_which_rtfdtm;
			  }
		  }
	  }

	  /// <summary>
	  /// Gets DTMXRTreeFrag object if one has already been created.
	  /// Creates new DTMXRTreeFrag object and adds to m_DTMXRTreeFrags  HashMap,
	  /// otherwise. </summary>
	  /// <param name="dtmIdentity"> </param>
	  /// <returns> DTMXRTreeFrag </returns>
	  public virtual DTMXRTreeFrag getDTMXRTreeFrag(int dtmIdentity)
	  {
		if (m_DTMXRTreeFrags == null)
		{
		  m_DTMXRTreeFrags = new Hashtable();
		}

		if (m_DTMXRTreeFrags.ContainsKey(new int?(dtmIdentity)))
		{
		   return (DTMXRTreeFrag)m_DTMXRTreeFrags[new int?(dtmIdentity)];
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xpath.objects.DTMXRTreeFrag frag = new org.apache.xpath.objects.DTMXRTreeFrag(dtmIdentity,this);
		  DTMXRTreeFrag frag = new DTMXRTreeFrag(dtmIdentity,this);
		  m_DTMXRTreeFrags[new int?(dtmIdentity)] = frag;
		  return frag;
		}
	  }

	  /// <summary>
	  /// Cleans DTMXRTreeFrag objects by removing references 
	  /// to DTM and XPathContext objects.   
	  /// </summary>
	  private void releaseDTMXRTreeFrags()
	  {
		if (m_DTMXRTreeFrags == null)
		{
		  return;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator iter = (m_DTMXRTreeFrags.values()).iterator();
		IEnumerator iter = (m_DTMXRTreeFrags.Values).GetEnumerator();
		while (iter.MoveNext())
		{
		  DTMXRTreeFrag frag = (DTMXRTreeFrag)iter.Current;
		  frag.destruct();
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
		  iter.remove();
		}
		m_DTMXRTreeFrags = null;
	  }
	}

}