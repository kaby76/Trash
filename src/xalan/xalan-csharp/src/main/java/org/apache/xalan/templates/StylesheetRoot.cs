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
 * $Id: StylesheetRoot.java 476466 2006-11-18 08:22:31Z minchau $
 */
namespace org.apache.xalan.templates
{


	using ExtensionNamespacesManager = org.apache.xalan.extensions.ExtensionNamespacesManager;
	using XSLTSchema = org.apache.xalan.processor.XSLTSchema;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using ExpandedNameTable = org.apache.xml.dtm.@ref.ExpandedNameTable;
	using IntStack = org.apache.xml.utils.IntStack;
	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// This class represents the root object of the stylesheet tree.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class StylesheetRoot : StylesheetComposed, Templates
	{
		internal new const long serialVersionUID = 3875353123529147855L;

		/// <summary>
		/// The flag for the setting of the optimize feature;
		/// </summary>
		private bool m_optimizer = true;

		/// <summary>
		/// The flag for the setting of the incremental feature;
		/// </summary>
		private bool m_incremental = false;

		/// <summary>
		/// The flag for the setting of the source_location feature;
		/// </summary>
		private bool m_source_location = false;

		/// <summary>
		/// State of the secure processing feature.
		/// </summary>
		private bool m_isSecureProcessing = false;

	  /// <summary>
	  /// Uses an XSL stylesheet document. </summary>
	  /// <exception cref="TransformerConfigurationException"> if the baseIdentifier can not be resolved to a URL. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public StylesheetRoot(javax.xml.transform.ErrorListener errorListener) throws javax.xml.transform.TransformerConfigurationException
	  public StylesheetRoot(ErrorListener errorListener) : base(null)
	  {


		StylesheetRoot = this;

		try
		{
		  m_selectDefault = new XPath("node()", this, this, XPath.SELECT, errorListener);

		  initDefaultRule(errorListener);
		}
		catch (TransformerException se)
		{
		  throw new TransformerConfigurationException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_INIT_DEFAULT_TEMPLATES, null), se); //"Can't init default templates!", se);
		}
	  }

	  /// <summary>
	  /// The schema used when creating this StylesheetRoot
	  /// @serial
	  /// </summary>
	  private Hashtable m_availElems;

	  /// <summary>
	  /// Creates a StylesheetRoot and retains a pointer to the schema used to create this
	  /// StylesheetRoot.  The schema may be needed later for an element-available() function call.
	  /// </summary>
	  /// <param name="schema"> The schema used to create this stylesheet </param>
	  /// <exception cref="TransformerConfigurationException"> if the baseIdentifier can not be resolved to a URL. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public StylesheetRoot(org.apache.xalan.processor.XSLTSchema schema, javax.xml.transform.ErrorListener listener) throws javax.xml.transform.TransformerConfigurationException
	  public StylesheetRoot(XSLTSchema schema, ErrorListener listener) : this(listener)
	  {

		m_availElems = schema.ElemsAvailable;
	  }

	  /// <summary>
	  /// Tell if this is the root of the stylesheet tree.
	  /// </summary>
	  /// <returns> True since this is the root of the stylesheet tree. </returns>
	  public override bool Root
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Set the state of the secure processing feature.
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
	  /// Get the hashtable of available elements.
	  /// </summary>
	  /// <returns> table of available elements, keyed by qualified names, and with 
	  /// values of the same qualified names. </returns>
	  public virtual Hashtable AvailableElements
	  {
		  get
		  {
			return m_availElems;
		  }
	  }

	  [NonSerialized]
	  private ExtensionNamespacesManager m_extNsMgr = null;

	  /// <summary>
	  /// Only instantiate an ExtensionNamespacesManager if one is called for
	  /// (i.e., if the stylesheet contains  extension functions and/or elements).
	  /// </summary>
	  public virtual ExtensionNamespacesManager ExtensionNamespacesManager
	  {
		  get
		  {
			 if (m_extNsMgr == null)
			 {
			   m_extNsMgr = new ExtensionNamespacesManager();
			 }
			 return m_extNsMgr;
		  }
	  }

	  /// <summary>
	  /// Get the vector of extension namespaces. Used to provide
	  /// the extensions table access to a list of extension
	  /// namespaces encountered during composition of a stylesheet.
	  /// </summary>
	  public virtual ArrayList Extensions
	  {
		  get
		  {
			return m_extNsMgr != null ? m_extNsMgr.Extensions : null;
		  }
	  }

	/*
	  public void runtimeInit(TransformerImpl transformer) throws TransformerException
	  {
	    System.out.println("StylesheetRoot.runtimeInit()");
	      
	  //    try{throw new Exception("StylesheetRoot.runtimeInit()");} catch(Exception e){e.printStackTrace();}
	
	    }
	*/  

	  //============== Templates Interface ================

	  /// <summary>
	  /// Create a new transformation context for this Templates object.
	  /// </summary>
	  /// <returns> A Transformer instance, never null. </returns>
	  public virtual Transformer newTransformer()
	  {
		return new TransformerImpl(this);
	  }


	  public virtual Properties DefaultOutputProps
	  {
		  get
		  {
			return m_outputProperties.Properties;
		  }
	  }

	  /// <summary>
	  /// Get the static properties for xsl:output.  The object returned will
	  /// be a clone of the internal values, and thus it can be mutated
	  /// without mutating the Templates object, and then handed in to
	  /// the process method.
	  /// 
	  /// <para>For XSLT, Attribute Value Templates attribute values will
	  /// be returned unexpanded (since there is no context at this point).</para>
	  /// </summary>
	  /// <returns> A Properties object, not null. </returns>
	  public virtual Properties OutputProperties
	  {
		  get
		  {
			return (Properties)DefaultOutputProps.clone();
		  }
	  }

	  //============== End Templates Interface ================

	  /// <summary>
	  /// Recompose the values of all "composed" properties, meaning
	  /// properties that need to be combined or calculated from
	  /// the combination of imported and included stylesheets.  This
	  /// method determines the proper import precedence of all imported
	  /// stylesheets.  It then iterates through all of the elements and 
	  /// properties in the proper order and triggers the individual recompose
	  /// methods.
	  /// </summary>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void recompose() throws javax.xml.transform.TransformerException
	  public virtual void recompose()
	  {
		// Now we make a Vector that is going to hold all of the recomposable elements

		  ArrayList recomposableElements = new ArrayList();

		// First, we build the global import tree.

		if (null == m_globalImportList)
		{

		  ArrayList importList = new ArrayList();

		  addImports(this, true, importList);

		  // Now we create an array and reverse the order of the importList vector.
		  // We built the importList vector backwards so that we could use addElement
		  // to append to the end of the vector instead of constantly pushing new
		  // stylesheets onto the front of the vector and having to shift the rest
		  // of the vector each time.

		  m_globalImportList = new StylesheetComposed[importList.Count];

		  for (int i = 0, j = importList.Count - 1; i < importList.Count; i++)
		  {
			m_globalImportList[j] = (StylesheetComposed) importList[i];
			// Build the global include list for this stylesheet.
			// This needs to be done ahead of the recomposeImports
			// because we need the info from the composed includes. 
			m_globalImportList[j].recomposeIncludes(m_globalImportList[j]);
			// Calculate the number of this import.    
			m_globalImportList[j--].recomposeImports();
		  }
		}
		// Next, we walk the import tree and add all of the recomposable elements to the vector.
		int n = GlobalImportCount;

		for (int i = 0; i < n; i++)
		{
		  StylesheetComposed imported = getGlobalImport(i);
		  imported.recompose(recomposableElements);
		}

		// We sort the elements into ascending order.

		QuickSort2(recomposableElements, 0, recomposableElements.Count - 1);

		// We set up the global variables that will hold the recomposed information.


		m_outputProperties = new OutputProperties(org.apache.xml.serializer.Method.UNKNOWN);
	//  m_outputProperties = new OutputProperties(Method.XML);

		m_attrSets = new Hashtable();
		m_decimalFormatSymbols = new Hashtable();
		m_keyDecls = new ArrayList();
		m_namespaceAliasComposed = new Hashtable();
		m_templateList = new TemplateList();
		m_variables = new ArrayList();

		// Now we sequence through the sorted elements, 
		// calling the recompose() function on each one.  This will call back into the
		// appropriate routine here to actually do the recomposition.
		// Note that we're going backwards, encountering the highest precedence items first.
		for (int i = recomposableElements.Count - 1; i >= 0; i--)
		{
		  ((ElemTemplateElement) recomposableElements[i]).recompose(this);
		}

	/*
	 * Backing out REE again, as it seems to cause some new failures
	 * which need to be investigated. -is
	 */      
		// This has to be done before the initialization of the compose state, because 
		// eleminateRedundentGlobals will add variables to the m_variables vector, which 
		// it then copied in the ComposeState constructor.

	//    if(true && org.apache.xalan.processor.TransformerFactoryImpl.m_optimize)
	//    {
	//          RedundentExprEliminator ree = new RedundentExprEliminator();
	//          callVisitors(ree);
	//          ree.eleminateRedundentGlobals(this);
	//    }

		initComposeState();

		// Need final composition of TemplateList.  This adds the wild cards onto the chains.
		m_templateList.compose(this);

		// Need to clear check for properties at the same import level.
		m_outputProperties.compose(this);
		m_outputProperties.endCompose(this);

		// Now call the compose() method on every element to give it a chance to adjust
		// based on composed values.

		n = GlobalImportCount;

		for (int i = 0; i < n; i++)
		{
		  StylesheetComposed imported = this.getGlobalImport(i);
		  int includedCount = imported.IncludeCountComposed;
		  for (int j = -1; j < includedCount; j++)
		  {
			Stylesheet included = imported.getIncludeComposed(j);
			composeTemplates(included);
		  }
		}
		// Attempt to register any remaining unregistered extension namespaces.
		if (m_extNsMgr != null)
		{
		  m_extNsMgr.registerUnregisteredNamespaces();
		}

		clearComposeState();
	  }

	  /// <summary>
	  /// Call the compose function for each ElemTemplateElement.
	  /// </summary>
	  /// <param name="templ"> non-null reference to template element that will have 
	  /// the composed method called on it, and will have it's children's composed 
	  /// methods called. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void composeTemplates(ElemTemplateElement templ) throws javax.xml.transform.TransformerException
	  internal virtual void composeTemplates(ElemTemplateElement templ)
	  {

		templ.compose(this);

		for (ElemTemplateElement child = templ.FirstChildElem; child != null; child = child.NextSiblingElem)
		{
		  composeTemplates(child);
		}

		templ.endCompose(this);
	  }

	  /// <summary>
	  /// The combined list of imports.  The stylesheet with the highest
	  /// import precedence will be at element 0.  The one with the lowest
	  /// import precedence will be at element length - 1.
	  /// @serial
	  /// </summary>
	  private StylesheetComposed[] m_globalImportList;

	  /// <summary>
	  /// Add the imports in the given sheet to the working importList vector.
	  /// The will be added from highest import precedence to
	  /// least import precedence.  This is a post-order traversal of the
	  /// import tree as described in <a href="http://www.w3.org/TR/xslt.html#import">the
	  /// XSLT Recommendation</a>.
	  /// <para>For example, suppose</para>
	  /// <para>stylesheet A imports stylesheets B and C in that order;</para>
	  /// <para>stylesheet B imports stylesheet D;</para>
	  /// <para>stylesheet C imports stylesheet E.</para>
	  /// <para>Then the order of import precedence (highest first) is
	  /// A, C, E, B, D.</para>
	  /// </summary>
	  /// <param name="stylesheet"> Stylesheet to examine for imports. </param>
	  /// <param name="addToList">  <code>true</code> if this template should be added to the import list </param>
	  /// <param name="importList"> The working import list.  Templates are added here in the reverse
	  ///        order of priority.  When we're all done, we'll reverse this to the correct
	  ///        priority in an array. </param>
	  protected internal virtual void addImports(Stylesheet stylesheet, bool addToList, ArrayList importList)
	  {

		// Get the direct imports of this sheet.

		int n = stylesheet.ImportCount;

		if (n > 0)
		{
		  for (int i = 0; i < n; i++)
		  {
			Stylesheet imported = stylesheet.getImport(i);

			addImports(imported, true, importList);
		  }
		}

		n = stylesheet.IncludeCount;

		if (n > 0)
		{
		  for (int i = 0; i < n; i++)
		  {
			Stylesheet included = stylesheet.getInclude(i);

			addImports(included, false, importList);
		  }
		}

		if (addToList)
		{
		  importList.Add(stylesheet);
		}

	  }

	  /// <summary>
	  /// Get a stylesheet from the global import list. 
	  /// TODO: JKESS PROPOSES SPECIAL-CASE FOR NO IMPORT LIST, TO MATCH COUNT.
	  /// </summary>
	  /// <param name="i"> Index of stylesheet to get from global import list 
	  /// </param>
	  /// <returns> The stylesheet at the given index  </returns>
	  public virtual StylesheetComposed getGlobalImport(int i)
	  {
		return m_globalImportList[i];
	  }

	  /// <summary>
	  /// Get the total number of imports in the global import list.
	  /// </summary>
	  /// <returns> The total number of imported stylesheets, including
	  /// the root stylesheet, thus the number will always be 1 or
	  /// greater.
	  /// TODO: JKESS PROPOSES SPECIAL-CASE FOR NO IMPORT LIST, TO MATCH DESCRIPTION. </returns>
	  public virtual int GlobalImportCount
	  {
		  get
		  {
				  return (m_globalImportList != null) ? m_globalImportList.Length : 1;
		  }
	  }

	  /// <summary>
	  /// Given a stylesheet, return the number of the stylesheet
	  /// in the global import list. </summary>
	  /// <param name="sheet"> The stylesheet which will be located in the
	  /// global import list. </param>
	  /// <returns> The index into the global import list of the given stylesheet,
	  /// or -1 if it is not found (which should never happen). </returns>
	  public virtual int getImportNumber(StylesheetComposed sheet)
	  {

		if (this == sheet)
		{
		  return 0;
		}

		int n = GlobalImportCount;

		for (int i = 0; i < n; i++)
		{
		  if (sheet == getGlobalImport(i))
		  {
			return i;
		  }
		}

		return -1;
	  }

	  /// <summary>
	  /// This will be set up with the default values, and then the values
	  /// will be set as stylesheets are encountered.
	  /// @serial
	  /// </summary>
	  private OutputProperties m_outputProperties;

	  /// <summary>
	  /// Recompose the output format object from the included elements.
	  /// </summary>
	  /// <param name="oprops"> non-null reference to xsl:output properties representation. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void recomposeOutput(OutputProperties oprops) throws javax.xml.transform.TransformerException
	  internal virtual void recomposeOutput(OutputProperties oprops)
	  {

		m_outputProperties.copyFrom(oprops);
	  }

	  /// <summary>
	  /// Get the combined "xsl:output" property with the properties
	  /// combined from the included stylesheets.  If a xsl:output
	  /// is not declared in this stylesheet or an included stylesheet,
	  /// look in the imports.
	  /// Please note that this returns a reference to the OutputProperties
	  /// object, not a cloned object, like getOutputProperties does. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.output">output in XSLT Specification</a>"
	  ////>
	  /// <returns> non-null reference to composed output properties object. </returns>
	  public virtual OutputProperties OutputComposed
	  {
		  get
		  {
    
			// System.out.println("getOutputComposed.getIndent: "+m_outputProperties.getIndent());
			// System.out.println("getOutputComposed.getIndenting: "+m_outputProperties.getIndenting());
			return m_outputProperties;
		  }
	  }

	  /// <summary>
	  /// Flag indicating whether an output method has been set by the user.
	  ///  @serial           
	  /// </summary>
	  private bool m_outputMethodSet = false;

	  /// <summary>
	  /// Find out if an output method has been set by the user.
	  /// </summary>
	  /// <returns> Value indicating whether an output method has been set by the user
	  /// @xsl.usage internal </returns>
	  public virtual bool OutputMethodSet
	  {
		  get
		  {
			return m_outputMethodSet;
		  }
	  }

	  /// <summary>
	  /// Composed set of all included and imported attribute set properties.
	  /// Each entry is a vector of ElemAttributeSet objects.
	  /// @serial
	  /// </summary>
	  private Hashtable m_attrSets;

	  /// <summary>
	  /// Recompose the attribute-set declarations.
	  /// </summary>
	  /// <param name="attrSet"> An attribute-set to add to the hashtable of attribute sets. </param>
	  internal virtual void recomposeAttributeSets(ElemAttributeSet attrSet)
	  {
		ArrayList attrSetList = (ArrayList) m_attrSets[attrSet.Name];

		if (null == attrSetList)
		{
		  attrSetList = new ArrayList();

		  m_attrSets[attrSet.Name] = attrSetList;
		}

		attrSetList.Add(attrSet);
	  }

	  /// <summary>
	  /// Get a list "xsl:attribute-set" properties that match the qname. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.attribute-sets">attribute-sets in XSLT Specification</a>"
	  ////>
	  /// <param name="name"> Qualified name of attribute set properties to get
	  /// </param>
	  /// <returns> A vector of attribute sets matching the given name
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.util.ArrayList getAttributeSetComposed(org.apache.xml.utils.QName name) throws ArrayIndexOutOfBoundsException
	  public virtual ArrayList getAttributeSetComposed(QName name)
	  {
		return (ArrayList) m_attrSets[name];
	  }

	  /// <summary>
	  /// Table of DecimalFormatSymbols, keyed by QName.
	  /// @serial
	  /// </summary>
	  private Hashtable m_decimalFormatSymbols;

	  /// <summary>
	  /// Recompose the decimal-format declarations.
	  /// </summary>
	  /// <param name="dfp"> A DecimalFormatProperties to add to the hashtable of decimal formats. </param>
	  internal virtual void recomposeDecimalFormats(DecimalFormatProperties dfp)
	  {
		DecimalFormatSymbols oldDfs = (DecimalFormatSymbols) m_decimalFormatSymbols[dfp.Name];
		if (null == oldDfs)
		{
		  m_decimalFormatSymbols[dfp.Name] = dfp.DecimalFormatSymbols;
		}
		else if (!dfp.DecimalFormatSymbols.Equals(oldDfs))
		{
		  string themsg;
		  if (dfp.Name.Equals(new QName("")))
		  {
			// "Only one default xsl:decimal-format declaration is allowed."
			themsg = XSLMessages.createWarning(XSLTErrorResources.WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, new object[0]);
		  }
		  else
		  {
			// "xsl:decimal-format names must be unique. Name {0} has been duplicated."
			themsg = XSLMessages.createWarning(XSLTErrorResources.WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, new object[] {dfp.Name});
		  }

		  error(themsg); // Should we throw TransformerException instead?
		}

	  }

	  /// <summary>
	  /// Given a valid element decimal-format name, return the
	  /// decimalFormatSymbols with that name.
	  /// <para>It is an error to declare either the default decimal-format or
	  /// a decimal-format with a given name more than once (even with
	  /// different import precedence), unless it is declared every
	  /// time with the same value for all attributes (taking into
	  /// account any default values).</para>
	  /// <para>Which means, as far as I can tell, the decimal-format
	  /// properties are not additive.</para>
	  /// </summary>
	  /// <param name="name"> Qualified name of the decimal format to find </param>
	  /// <returns> DecimalFormatSymbols object matching the given name or
	  /// null if name is not found. </returns>
	  public virtual DecimalFormatSymbols getDecimalFormatComposed(QName name)
	  {
		return (DecimalFormatSymbols) m_decimalFormatSymbols[name];
	  }

	  /// <summary>
	  /// A list of all key declarations visible from this stylesheet and all
	  /// lesser stylesheets.
	  /// @serial
	  /// </summary>
	  private ArrayList m_keyDecls;

	  /// <summary>
	  /// Recompose the key declarations.
	  /// </summary>
	  /// <param name="keyDecl"> A KeyDeclaration to be added to the vector of key declarations. </param>
	  internal virtual void recomposeKeys(KeyDeclaration keyDecl)
	  {
		m_keyDecls.Add(keyDecl);
	  }

	  /// <summary>
	  /// Get the composed "xsl:key" properties. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.key">key in XSLT Specification</a>"
	  ////>
	  /// <returns> A vector of the composed "xsl:key" properties. </returns>
	  public virtual ArrayList KeysComposed
	  {
		  get
		  {
			return m_keyDecls;
		  }
	  }

	  /// <summary>
	  /// Composed set of all namespace aliases.
	  /// @serial
	  /// </summary>
	  private Hashtable m_namespaceAliasComposed;

	  /// <summary>
	  /// Recompose the namespace-alias declarations.
	  /// </summary>
	  /// <param name="nsAlias"> A NamespaceAlias object to add to the hashtable of namespace aliases. </param>
	  internal virtual void recomposeNamespaceAliases(NamespaceAlias nsAlias)
	  {
		m_namespaceAliasComposed[nsAlias.StylesheetNamespace] = nsAlias;
	  }

	  /// <summary>
	  /// Get the "xsl:namespace-alias" property.
	  /// Return the NamespaceAlias for a given namespace uri. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"
	  ////>
	  /// <param name="uri"> non-null reference to namespace that is to be aliased.
	  /// </param>
	  /// <returns> NamespaceAlias that matches uri, or null if no match. </returns>
	  public virtual NamespaceAlias getNamespaceAliasComposed(string uri)
	  {
		return (NamespaceAlias)((null == m_namespaceAliasComposed) ? null : m_namespaceAliasComposed[uri]);
	  }

	  /// <summary>
	  /// The "xsl:template" properties.
	  /// @serial
	  /// </summary>
	  private TemplateList m_templateList;

	  /// <summary>
	  /// Recompose the template declarations.
	  /// </summary>
	  /// <param name="template"> An ElemTemplate object to add to the template list. </param>
	  internal virtual void recomposeTemplates(ElemTemplate template)
	  {
		m_templateList.Template = template;
	  }

	  /// <summary>
	  /// Accessor method to retrieve the <code>TemplateList</code> associated with
	  /// this StylesheetRoot.
	  /// </summary>
	  /// <returns> The composed <code>TemplateList</code>. </returns>
	  public TemplateList TemplateListComposed
	  {
		  get
		  {
			return m_templateList;
		  }
		  set
		  {
			m_templateList = value;
		  }
	  }


	  /// <summary>
	  /// Get an "xsl:template" property by node match. This looks in the imports as
	  /// well as this stylesheet. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	  ////>
	  /// <param name="xctxt"> non-null reference to XPath runtime execution context. </param>
	  /// <param name="targetNode"> non-null reference of node that the template must match. </param>
	  /// <param name="mode"> qualified name of the node, or null. </param>
	  /// <param name="quietConflictWarnings"> true if conflict warnings should not be reported.
	  /// </param>
	  /// <returns> reference to ElemTemplate that is the best match for targetNode, or 
	  ///         null if no match could be made.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemTemplate getTemplateComposed(org.apache.xpath.XPathContext xctxt, int targetNode, org.apache.xml.utils.QName mode, boolean quietConflictWarnings, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
	  public virtual ElemTemplate getTemplateComposed(XPathContext xctxt, int targetNode, QName mode, bool quietConflictWarnings, DTM dtm)
	  {
		return m_templateList.getTemplate(xctxt, targetNode, mode, quietConflictWarnings, dtm);
	  }

	  /// <summary>
	  /// Get an "xsl:template" property by node match. This looks in the imports as
	  /// well as this stylesheet. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	  ////>
	  /// <param name="xctxt"> non-null reference to XPath runtime execution context. </param>
	  /// <param name="targetNode"> non-null reference of node that the template must match. </param>
	  /// <param name="mode"> qualified name of the node, or null. </param>
	  /// <param name="maxImportLevel"> The maximum importCountComposed that we should consider or -1
	  ///        if we should consider all import levels.  This is used by apply-imports to
	  ///        access templates that have been overridden. </param>
	  /// <param name="endImportLevel"> The count of composed imports </param>
	  /// <param name="quietConflictWarnings"> true if conflict warnings should not be reported.
	  /// </param>
	  /// <returns> reference to ElemTemplate that is the best match for targetNode, or 
	  ///         null if no match could be made.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemTemplate getTemplateComposed(org.apache.xpath.XPathContext xctxt, int targetNode, org.apache.xml.utils.QName mode, int maxImportLevel, int endImportLevel, boolean quietConflictWarnings, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
	  public virtual ElemTemplate getTemplateComposed(XPathContext xctxt, int targetNode, QName mode, int maxImportLevel, int endImportLevel, bool quietConflictWarnings, DTM dtm)
	  {
		return m_templateList.getTemplate(xctxt, targetNode, mode, maxImportLevel, endImportLevel, quietConflictWarnings, dtm);
	  }

	  /// <summary>
	  /// Get an "xsl:template" property. This looks in the imports as
	  /// well as this stylesheet. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	  ////>
	  /// <param name="qname"> non-null reference to qualified name of template.
	  /// </param>
	  /// <returns> reference to named template, or null if not found. </returns>
	  public virtual ElemTemplate getTemplateComposed(QName qname)
	  {
		return m_templateList.getTemplate(qname);
	  }

	  /// <summary>
	  /// Composed set of all variables and params.
	  /// @serial
	  /// </summary>
	  private ArrayList m_variables;

	  /// <summary>
	  /// Recompose the top level variable and parameter declarations.
	  /// </summary>
	  /// <param name="elemVar"> A top level variable or parameter to be added to the Vector. </param>
	  internal virtual void recomposeVariables(ElemVariable elemVar)
	  {
		// Don't overide higher priority variable        
		if (getVariableOrParamComposed(elemVar.Name) == null)
		{
		  elemVar.IsTopLevel = true; // Mark as a top-level variable or param
		  elemVar.Index = m_variables.Count;
		  m_variables.Add(elemVar);
		}
	  }

	  /// <summary>
	  /// Get an "xsl:variable" property. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <param name="qname"> Qualified name of variable or param
	  /// </param>
	  /// <returns> The ElemVariable with the given qualified name </returns>
	  public virtual ElemVariable getVariableOrParamComposed(QName qname)
	  {
		if (null != m_variables)
		{
		  int n = m_variables.Count;

		  for (int i = 0; i < n; i++)
		  {
			ElemVariable var = (ElemVariable)m_variables[i];
			if (var.Name.Equals(qname))
			{
			  return var;
			}
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Get all global "xsl:variable" properties in scope for this stylesheet. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.top-level-variables">top-level-variables in XSLT Specification</a>"
	  ////>
	  /// <returns> Vector of all variables and params in scope </returns>
	  public virtual ArrayList VariablesAndParamsComposed
	  {
		  get
		  {
			return m_variables;
		  }
	  }

	  /// <summary>
	  /// A list of properties that specify how to do space
	  /// stripping. This uses the same exact mechanism as Templates.
	  /// @serial
	  /// </summary>
	  private TemplateList m_whiteSpaceInfoList;

	  /// <summary>
	  /// Recompose the strip-space and preserve-space declarations.
	  /// </summary>
	  /// <param name="wsi"> A WhiteSpaceInfo element to add to the list of WhiteSpaceInfo elements. </param>
	  internal virtual void recomposeWhiteSpaceInfo(WhiteSpaceInfo wsi)
	  {
		if (null == m_whiteSpaceInfoList)
		{
		  m_whiteSpaceInfoList = new TemplateList();
		}

		m_whiteSpaceInfoList.Template = wsi;
	  }

	  /// <summary>
	  /// Check to see if the caller should bother with check for
	  /// whitespace nodes.
	  /// </summary>
	  /// <returns> Whether the caller should bother with check for
	  /// whitespace nodes. </returns>
	  public virtual bool shouldCheckWhitespace()
	  {
		return null != m_whiteSpaceInfoList;
	  }

	  /// <summary>
	  /// Get information about whether or not an element should strip whitespace. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="support"> The XPath runtime state. </param>
	  /// <param name="targetElement"> Element to check
	  /// </param>
	  /// <returns> WhiteSpaceInfo for the given element
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public WhiteSpaceInfo getWhiteSpaceInfo(org.apache.xpath.XPathContext support, int targetElement, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
	  public virtual WhiteSpaceInfo getWhiteSpaceInfo(XPathContext support, int targetElement, DTM dtm)
	  {

		if (null != m_whiteSpaceInfoList)
		{
		  return (WhiteSpaceInfo) m_whiteSpaceInfoList.getTemplate(support, targetElement, null, false, dtm);
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Get information about whether or not an element should strip whitespace. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="support"> The XPath runtime state. </param>
	  /// <param name="targetElement"> Element to check
	  /// </param>
	  /// <returns> true if the whitespace should be stripped.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean shouldStripWhiteSpace(org.apache.xpath.XPathContext support, int targetElement) throws javax.xml.transform.TransformerException
	  public virtual bool shouldStripWhiteSpace(XPathContext support, int targetElement)
	  {
		if (null != m_whiteSpaceInfoList)
		{
		  while (DTM.NULL != targetElement)
		  {
			DTM dtm = support.getDTM(targetElement);
			WhiteSpaceInfo info = (WhiteSpaceInfo) m_whiteSpaceInfoList.getTemplate(support, targetElement, null, false, dtm);
			if (null != info)
			{
			  return info.ShouldStripSpace;
			}

			int parent = dtm.getParent(targetElement);
			if (DTM.NULL != parent && DTM.ELEMENT_NODE == dtm.getNodeType(parent))
			{
			  targetElement = parent;
			}
			else
			{
			  targetElement = DTM.NULL;
			}
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Get information about whether or not whitespace can be stripped. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <returns> true if the whitespace can be stripped. </returns>
	  public override bool canStripWhiteSpace()
	  {
		return (null != m_whiteSpaceInfoList);
	  }



	  /// <summary>
	  /// The default template to use for text nodes if we don't find
	  /// anything else.  This is initialized in initDefaultRule().
	  /// @serial
	  /// @xsl.usage advanced
	  /// </summary>
	  private ElemTemplate m_defaultTextRule;

	  /// <summary>
	  /// Get the default template for text.
	  /// </summary>
	  /// <returns> the default template for text.
	  /// @xsl.usage advanced </returns>
	  public ElemTemplate DefaultTextRule
	  {
		  get
		  {
			return m_defaultTextRule;
		  }
	  }

	  /// <summary>
	  /// The default template to use if we don't find anything
	  /// else.  This is initialized in initDefaultRule().
	  /// @serial
	  /// @xsl.usage advanced
	  /// </summary>
	  private ElemTemplate m_defaultRule;

	  /// <summary>
	  /// Get the default template for elements.
	  /// </summary>
	  /// <returns> the default template for elements.
	  /// @xsl.usage advanced </returns>
	  public ElemTemplate DefaultRule
	  {
		  get
		  {
			return m_defaultRule;
		  }
	  }

	  /// <summary>
	  /// The default template to use for the root if we don't find
	  /// anything else.  This is initialized in initDefaultRule().
	  /// We kind of need this because the defaultRule isn't good
	  /// enough because it doesn't supply a document context.
	  /// For now, I default the root document element to "HTML".
	  /// Don't know if this is really a good idea or not.
	  /// I suspect it is not.
	  /// @serial
	  /// @xsl.usage advanced
	  /// </summary>
	  private ElemTemplate m_defaultRootRule;

	  /// <summary>
	  /// Get the default template for a root node.
	  /// </summary>
	  /// <returns> The default template for a root node.
	  /// @xsl.usage advanced </returns>
	  public ElemTemplate DefaultRootRule
	  {
		  get
		  {
			return m_defaultRootRule;
		  }
	  }

	  /// <summary>
	  /// The start rule to kick off the transformation.
	  /// @serial
	  /// @xsl.usage advanced
	  /// </summary>
	  private ElemTemplate m_startRule;

	  /// <summary>
	  /// Get the default template for a root node.
	  /// </summary>
	  /// <returns> The default template for a root node.
	  /// @xsl.usage advanced </returns>
	  public ElemTemplate StartRule
	  {
		  get
		  {
			return m_startRule;
		  }
	  }


	  /// <summary>
	  /// Used for default selection.
	  /// @serial
	  /// </summary>
	  internal XPath m_selectDefault;

	  /// <summary>
	  /// Create the default rule if needed.
	  /// </summary>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void initDefaultRule(javax.xml.transform.ErrorListener errorListener) throws javax.xml.transform.TransformerException
	  private void initDefaultRule(ErrorListener errorListener)
	  {

		// Then manufacture a default
		m_defaultRule = new ElemTemplate();

		m_defaultRule.Stylesheet = this;

		XPath defMatch = new XPath("*", this, this, XPath.MATCH, errorListener);

		m_defaultRule.Match = defMatch;

		ElemApplyTemplates childrenElement = new ElemApplyTemplates();

		childrenElement.IsDefaultTemplate = true;
		childrenElement.setSelect(m_selectDefault);
		m_defaultRule.appendChild(childrenElement);

		m_startRule = m_defaultRule;

		// -----------------------------
		m_defaultTextRule = new ElemTemplate();

		m_defaultTextRule.Stylesheet = this;

		defMatch = new XPath("text() | @*", this, this, XPath.MATCH, errorListener);

		m_defaultTextRule.Match = defMatch;

		ElemValueOf elemValueOf = new ElemValueOf();

		m_defaultTextRule.appendChild(elemValueOf);

		XPath selectPattern = new XPath(".", this, this, XPath.SELECT, errorListener);

		elemValueOf.Select = selectPattern;

		//--------------------------------
		m_defaultRootRule = new ElemTemplate();

		m_defaultRootRule.Stylesheet = this;

		defMatch = new XPath("/", this, this, XPath.MATCH, errorListener);

		m_defaultRootRule.Match = defMatch;

		childrenElement = new ElemApplyTemplates();

		childrenElement.IsDefaultTemplate = true;
		m_defaultRootRule.appendChild(childrenElement);
		childrenElement.setSelect(m_selectDefault);
	  }

	  /// <summary>
	  /// This is a generic version of C.A.R Hoare's Quick Sort
	  /// algorithm.  This will handle arrays that are already
	  /// sorted, and arrays with duplicate keys.  It was lifted from
	  /// the NodeSorter class but should probably be eliminated and replaced
	  /// with a call to Collections.sort when we migrate to Java2.<BR>
	  /// 
	  /// If you think of a one dimensional array as going from
	  /// the lowest index on the left to the highest index on the right
	  /// then the parameters to this function are lowest index or
	  /// left and highest index or right.  The first time you call
	  /// this function it will be with the parameters 0, a.length - 1.
	  /// </summary>
	  /// <param name="v">       a vector of ElemTemplateElement elements </param>
	  /// <param name="lo0">     left boundary of partition </param>
	  /// <param name="hi0">     right boundary of partition
	  ///  </param>

	  private void QuickSort2(ArrayList v, int lo0, int hi0)
	  {
		  int lo = lo0;
		  int hi = hi0;

		  if (hi0 > lo0)
		  {
			// Arbitrarily establishing partition element as the midpoint of
			// the array.
			ElemTemplateElement midNode = (ElemTemplateElement) v[(lo0 + hi0) / 2];

			// loop through the array until indices cross
			while (lo <= hi)
			{
			  // find the first element that is greater than or equal to
			  // the partition element starting from the left Index.
			  while ((lo < hi0) && (((ElemTemplateElement) v[lo]).compareTo(midNode) < 0))
			  {
				++lo;
			  } // end while

			  // find an element that is smaller than or equal to
			  // the partition element starting from the right Index.
			  while ((hi > lo0) && (((ElemTemplateElement) v[hi]).compareTo(midNode) > 0))
			  {
				--hi;
			  }

			  // if the indexes have not crossed, swap
			  if (lo <= hi)
			  {
				ElemTemplateElement node = (ElemTemplateElement) v[lo];
				v[lo] = v[hi];
				v[hi] = node;

				++lo;
				--hi;
			  }
			}

			// If the right index has not reached the left side of array
			// must now sort the left partition.
			if (lo0 < hi)
			{
			  QuickSort2(v, lo0, hi);
			}

			// If the left index has not reached the right side of array
			// must now sort the right partition.
			if (lo < hi0)
			{
			  QuickSort2(v, lo, hi0);
			}
		  }
	  } // end QuickSort2  */

		[NonSerialized]
		private ComposeState m_composeState;

		/// <summary>
		/// Initialize a new ComposeState.
		/// </summary>
		internal virtual void initComposeState()
		{
		  m_composeState = new ComposeState(this);
		}

		/// <summary>
		/// Return class to track state global state during the compose() operation. </summary>
		/// <returns> ComposeState reference, or null if endCompose has been called. </returns>
		internal virtual ComposeState ComposeState
		{
			get
			{
			  return m_composeState;
			}
		}

		/// <summary>
		/// Clear the compose state.
		/// </summary>
		private void clearComposeState()
		{
		  m_composeState = null;
		}

		private string m_extensionHandlerClass = "org.apache.xalan.extensions.ExtensionHandlerExsltFunction";

		/// <summary>
		/// This internal method allows the setting of the java class
		/// to handle the extension function (if other than the default one).
		/// 
		/// @xsl.usage internal
		/// </summary>
		public virtual string setExtensionHandlerClass(string handlerClassName)
		{
			string oldvalue = m_extensionHandlerClass;
			m_extensionHandlerClass = handlerClassName;
			return oldvalue;
		}
		/// 
		/// <summary>
		/// @xsl.usage internal
		/// </summary>
		public virtual string ExtensionHandlerClass
		{
			get
			{
				return m_extensionHandlerClass;
			}
		}

		/// <summary>
		/// Class to track state global state during the compose() operation.
		/// </summary>
		internal class ComposeState
		{
			private readonly StylesheetRoot outerInstance;

		  internal ComposeState(StylesheetRoot outerInstance)
		  {
			  this.outerInstance = outerInstance;
			int size = outerInstance.m_variables.Count;
			for (int i = 0; i < size; i++)
			{
			  ElemVariable ev = (ElemVariable)outerInstance.m_variables[i];
			  m_variableNames.Add(ev.Name);
			}

		  }

		  internal ExpandedNameTable m_ent = new ExpandedNameTable();

		  /// <summary>
		  /// Given a qualified name, return an integer ID that can be 
		  /// quickly compared.
		  /// </summary>
		  /// <param name="qname"> a qualified name object, must not be null.
		  /// </param>
		  /// <returns> the expanded-name id of the qualified name. </returns>
		  public virtual int getQNameID(QName qname)
		  {

			return m_ent.getExpandedTypeID(qname.Namespace, qname.LocalName, DTM.ELEMENT_NODE);
		  }

		  /// <summary>
		  /// A Vector of the current params and QNames within the current template.
		  /// Set by ElemTemplate and used by ProcessorVariable.
		  /// </summary>
		  internal ArrayList m_variableNames = new ArrayList();

		  /// <summary>
		  /// Add the name of a qualified name within the template.  The position in 
		  /// the vector is its ID. </summary>
		  /// <param name="qname"> A qualified name of a param or variable, should be non-null. </param>
		  /// <returns> the index where the variable was added. </returns>
		  internal virtual int addVariableName(in QName qname)
		  {
			int pos = m_variableNames.Count;
			m_variableNames.Add(qname);
			int frameSize = m_variableNames.Count - GlobalsSize;
			if (frameSize > m_maxStackFrameSize)
			{
			  m_maxStackFrameSize++;
			}
			return pos;
		  }

		  internal virtual void resetStackFrameSize()
		  {
			m_maxStackFrameSize = 0;
		  }

		  internal virtual int FrameSize
		  {
			  get
			  {
				return m_maxStackFrameSize;
			  }
		  }

		  /// <summary>
		  /// Get the current size of the stack frame.  Use this to record the position 
		  /// in a template element at startElement, so that it can be popped 
		  /// at endElement.
		  /// </summary>
		  internal virtual int CurrentStackFrameSize
		  {
			  get
			  {
				return m_variableNames.Count;
			  }
			  set
			  {
				m_variableNames.Capacity = value;
			  }
		  }


		  internal virtual int GlobalsSize
		  {
			  get
			  {
				return outerInstance.m_variables.Count;
			  }
		  }

		  internal IntStack m_marks = new IntStack();

		  internal virtual void pushStackMark()
		  {
			m_marks.push(CurrentStackFrameSize);
		  }

		  internal virtual void popStackMark()
		  {
			int mark = m_marks.pop();
			CurrentStackFrameSize = mark;
		  }

		  /// <summary>
		  /// Get the Vector of the current params and QNames to be collected 
		  /// within the current template. </summary>
		  /// <returns> A reference to the vector of variable names.  The reference 
		  /// returned is owned by this class, and so should not really be mutated, or 
		  /// stored anywhere. </returns>
		  internal virtual ArrayList VariableNames
		  {
			  get
			  {
				return m_variableNames;
			  }
		  }

		  internal int m_maxStackFrameSize;

		}

		/// <returns> Optimization flag </returns>
		public virtual bool Optimizer
		{
			get
			{
				return m_optimizer;
			}
			set
			{
				m_optimizer = value;
			}
		}


		/// <returns> Incremental flag </returns>
		public virtual bool Incremental
		{
			get
			{
				return m_incremental;
			}
			set
			{
				m_incremental = value;
			}
		}

		/// <returns> source location flag </returns>
		public virtual bool Source_location
		{
			get
			{
				return m_source_location;
			}
			set
			{
				m_source_location = value;
			}
		}



	}

}