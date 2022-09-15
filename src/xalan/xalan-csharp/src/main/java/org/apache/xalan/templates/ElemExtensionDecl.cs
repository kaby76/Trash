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
 * $Id: ElemExtensionDecl.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using ExtensionNamespaceSupport = org.apache.xalan.extensions.ExtensionNamespaceSupport;
	using ExtensionNamespacesManager = org.apache.xalan.extensions.ExtensionNamespacesManager;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using StringVector = org.apache.xml.utils.StringVector;

	/// <summary>
	/// Implement the declaration of an extension element 
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class ElemExtensionDecl : ElemTemplateElement
	{
		internal new const long serialVersionUID = -4692738885172766789L;

	  /// <summary>
	  /// Constructor ElemExtensionDecl
	  /// 
	  /// </summary>
	  public ElemExtensionDecl()
	  {

		// System.out.println("ElemExtensionDecl ctor");
	  }

	  /// <summary>
	  /// Prefix string for this extension element.
	  ///  @serial         
	  /// </summary>
	  private string m_prefix = null;

	  /// <summary>
	  /// Set the prefix for this extension element  
	  /// 
	  /// </summary>
	  /// <param name="v"> Prefix to set for this extension element </param>
	  public override string Prefix
	  {
		  set
		  {
			m_prefix = value;
		  }
		  get
		  {
			return m_prefix;
		  }
	  }


	  /// <summary>
	  /// StringVector holding the names of functions defined in this extension.
	  ///  @serial     
	  /// </summary>
	  private StringVector m_functions = new StringVector();

	  /// <summary>
	  /// Set the names of functions defined in this extension  
	  /// 
	  /// </summary>
	  /// <param name="v"> StringVector holding the names of functions defined in this extension </param>
	  public virtual StringVector Functions
	  {
		  set
		  {
			m_functions = value;
		  }
		  get
		  {
			return m_functions;
		  }
	  }


	  /// <summary>
	  /// Get a function at a given index in this extension element 
	  /// 
	  /// </summary>
	  /// <param name="i"> Index of function to get
	  /// </param>
	  /// <returns> Name of Function at given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getFunction(int i) throws ArrayIndexOutOfBoundsException
	  public virtual string getFunction(int i)
	  {

		if (null == m_functions)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (string) m_functions.elementAt(i);
	  }

	  /// <summary>
	  /// Get count of functions defined in this extension element
	  /// 
	  /// </summary>
	  /// <returns> count of functions defined in this extension element </returns>
	  public virtual int FunctionCount
	  {
		  get
		  {
			return (null != m_functions) ? m_functions.size() : 0;
		  }
	  }

	  /// <summary>
	  /// StringVector of elements defined in this extension.
	  ///  @serial         
	  /// </summary>
	  private StringVector m_elements = null;

	  /// <summary>
	  /// Set StringVector of elements for this extension
	  /// 
	  /// </summary>
	  /// <param name="v"> StringVector of elements to set </param>
	  public virtual StringVector Elements
	  {
		  set
		  {
			m_elements = value;
		  }
		  get
		  {
			return m_elements;
		  }
	  }


	  /// <summary>
	  /// Get the element at the given index
	  /// 
	  /// </summary>
	  /// <param name="i"> Index of element to get
	  /// </param>
	  /// <returns> The element at the given index
	  /// </returns>
	  /// <exception cref="ArrayIndexOutOfBoundsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getElement(int i) throws ArrayIndexOutOfBoundsException
	  public virtual string getElement(int i)
	  {

		if (null == m_elements)
		{
		  throw new System.IndexOutOfRangeException();
		}

		return (string) m_elements.elementAt(i);
	  }

	  /// <summary>
	  /// Return the count of elements defined for this extension element 
	  /// 
	  /// </summary>
	  /// <returns> the count of elements defined for this extension element </returns>
	  public virtual int ElementCount
	  {
		  get
		  {
			return (null != m_elements) ? m_elements.size() : 0;
		  }
	  }

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_EXTENSIONDECL;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);
		string prefix = Prefix;
		string declNamespace = getNamespaceForPrefix(prefix);
		string lang = null;
		string srcURL = null;
		string scriptSrc = null;
		if (null == declNamespace)
		{
		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_NAMESPACE_DECL, new object[]{prefix}));
		}
		  //"Prefix " + prefix does not have a corresponding namespace declaration");
		for (ElemTemplateElement child = FirstChildElem; child != null; child = child.NextSiblingElem)
		{
		  if (Constants.ELEMNAME_EXTENSIONSCRIPT == child.XSLToken)
		  {
			ElemExtensionScript sdecl = (ElemExtensionScript) child;
			lang = sdecl.Lang;
			srcURL = sdecl.Src;
			ElemTemplateElement childOfSDecl = sdecl.FirstChildElem;
			if (null != childOfSDecl)
			{
			  if (Constants.ELEMNAME_TEXTLITERALRESULT == childOfSDecl.XSLToken)
			  {
				ElemTextLiteral tl = (ElemTextLiteral) childOfSDecl;
				char[] chars = tl.Chars;
				scriptSrc = new string(chars);
				if (scriptSrc.Trim().Length == 0)
				{
				  scriptSrc = null;
				}
			  }
			}
		  }
		}
		if (null == lang)
		{
		  lang = "javaclass";
		}
		if (lang.Equals("javaclass") && (!string.ReferenceEquals(scriptSrc, null)))
		{
			throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_ELEM_CONTENT_NOT_ALLOWED, new object[]{scriptSrc}));
		}
			//"Element content not allowed for lang=javaclass " + scriptSrc);

		// Register the extension namespace if it has not already been registered.
		ExtensionNamespaceSupport extNsSpt = null;
		ExtensionNamespacesManager extNsMgr = sroot.ExtensionNamespacesManager;
		if (extNsMgr.namespaceIndex(declNamespace, extNsMgr.Extensions) == -1)
		{
		  if (lang.Equals("javaclass"))
		  {
			if (null == srcURL)
			{
			   extNsSpt = extNsMgr.defineJavaNamespace(declNamespace);
			}
			else if (extNsMgr.namespaceIndex(srcURL, extNsMgr.Extensions) == -1)
			{
			  extNsSpt = extNsMgr.defineJavaNamespace(declNamespace, srcURL);
			}
		  }
		  else // not java
		  {
			string handler = "org.apache.xalan.extensions.ExtensionHandlerGeneral";
			object[] args = new object[] {declNamespace, this.m_elements, this.m_functions, lang, srcURL, scriptSrc, SystemId};
			extNsSpt = new ExtensionNamespaceSupport(declNamespace, handler, args);
		  }
		}
		if (extNsSpt != null)
		{
		  extNsMgr.registerExtension(extNsSpt);
		}
	  }


	  /// <summary>
	  /// This function will be called on top-level elements
	  /// only, just before the transform begins.
	  /// </summary>
	  /// <param name="transformer"> The XSLT TransformerFactory.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void runtimeInit(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void runtimeInit(TransformerImpl transformer)
	  {
	/*    //System.out.println("ElemExtensionDecl.runtimeInit()");
	    String lang = null;
	    String srcURL = null;
	    String scriptSrc = null;
	    String prefix = getPrefix();
	    String declNamespace = getNamespaceForPrefix(prefix);
	
	    if (null == declNamespace)
	      throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_NAMESPACE_DECL, new Object[]{prefix})); 
	      //"Prefix " + prefix does not have a corresponding namespace declaration");
	
	    for (ElemTemplateElement child = getFirstChildElem(); child != null;
	            child = child.getNextSiblingElem())
	    {
	      if (Constants.ELEMNAME_EXTENSIONSCRIPT == child.getXSLToken())
	      {
	        ElemExtensionScript sdecl = (ElemExtensionScript) child;
	
	        lang = sdecl.getLang();
	        srcURL = sdecl.getSrc();
	
	        ElemTemplateElement childOfSDecl = sdecl.getFirstChildElem();
	
	        if (null != childOfSDecl)
	        {
	          if (Constants.ELEMNAME_TEXTLITERALRESULT
	                  == childOfSDecl.getXSLToken())
	          {
	            ElemTextLiteral tl = (ElemTextLiteral) childOfSDecl;
	            char[] chars = tl.getChars();
	
	            scriptSrc = new String(chars);
	
	            if (scriptSrc.trim().length() == 0)
	              scriptSrc = null;
	          }
	        }
	      }
	    }
	
	    if (null == lang)
	      lang = "javaclass";
	
	    if (lang.equals("javaclass") && (scriptSrc != null))
	      throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_ELEM_CONTENT_NOT_ALLOWED, new Object[]{scriptSrc})); 
	      //"Element content not allowed for lang=javaclass " + scriptSrc);
	    
	    // Instantiate a handler for this extension namespace.
	    ExtensionsTable etable = transformer.getExtensionsTable();    
	    ExtensionHandler nsh = etable.get(declNamespace);
	
	    // If we have no prior ExtensionHandler for this namespace, we need to
	    // create one.
	    // If the script element is for javaclass, this is our special compiled java.
	    // Element content is not supported for this so we throw an exception if
	    // it is provided.  Otherwise, we look up the srcURL to see if we already have
	    // an ExtensionHandler.
	    if (null == nsh)
	    {
	      if (lang.equals("javaclass"))
	      {
	        if (null == srcURL)
	        {
	          nsh = etable.makeJavaNamespace(declNamespace);
	        }
	        else
	        {
	          nsh = etable.get(srcURL);
	
	          if (null == nsh)
	          {
	            nsh = etable.makeJavaNamespace(srcURL);
	          }
	        }
	      }
	      else  // not java
	      {
	        nsh = new ExtensionHandlerGeneral(declNamespace, this.m_elements,
	                                          this.m_functions, lang, srcURL,
	                                          scriptSrc, getSystemId());
	
	        // System.out.println("Adding NS Handler: declNamespace = "+
	        //                   declNamespace+", lang = "+lang+", srcURL = "+
	        //                   srcURL+", scriptSrc="+scriptSrc);
	      }
	
	      etable.addExtensionNamespace(declNamespace, nsh);
	    }*/
	  }
	}

}