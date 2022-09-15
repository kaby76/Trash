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
 * $Id: ElemExtensionCall.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using ExtensionHandler = org.apache.xalan.extensions.ExtensionHandler;
	using ExtensionsTable = org.apache.xalan.extensions.ExtensionsTable;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPathContext = org.apache.xpath.XPathContext;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Implement an extension element. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemExtensionCall : ElemLiteralResult
	{
		internal new const long serialVersionUID = 3171339708500216920L;

	  /// <summary>
	  /// The Namespace URI for this extension call element.
	  ///  @serial          
	  /// </summary>
	  internal string m_extns;

	  /// <summary>
	  /// Language used by extension.
	  ///  @serial          
	  /// </summary>
	  internal string m_lang;

	  /// <summary>
	  /// URL pointing to extension.
	  ///  @serial          
	  /// </summary>
	  internal string m_srcURL;

	  /// <summary>
	  /// Source for script.
	  ///  @serial          
	  /// </summary>
	  internal string m_scriptSrc;

	  /// <summary>
	  /// Declaration for Extension element. 
	  ///  @serial          
	  /// </summary>
	  internal ElemExtensionDecl m_decl = null;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_EXTENSIONCALL;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The element's name </returns>

	  // public String getNodeName()
	  // {
	  // TODO: Need prefix.
	  // return localPart;
	  // }

	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);
		m_extns = this.Namespace;
		m_decl = getElemExtensionDecl(sroot, m_extns);
		// Register the extension namespace if the extension does not have
		// an ElemExtensionDecl ("component").
		if (m_decl == null)
		{
		  sroot.ExtensionNamespacesManager.registerExtension(m_extns);
		}
	  }

	  /// <summary>
	  /// Return the ElemExtensionDecl for this extension element 
	  /// 
	  /// </summary>
	  /// <param name="stylesheet"> Stylesheet root associated with this extension element </param>
	  /// <param name="namespace"> Namespace associated with this extension element
	  /// </param>
	  /// <returns> the ElemExtensionDecl for this extension element.  </returns>
	  private ElemExtensionDecl getElemExtensionDecl(StylesheetRoot stylesheet, string @namespace)
	  {

		ElemExtensionDecl decl = null;
		int n = stylesheet.GlobalImportCount;

		for (int i = 0; i < n; i++)
		{
		  Stylesheet imported = stylesheet.getGlobalImport(i);

		  for (ElemTemplateElement child = imported.FirstChildElem; child != null; child = child.NextSiblingElem)
		  {
			if (Constants.ELEMNAME_EXTENSIONDECL == child.XSLToken)
			{
			  decl = (ElemExtensionDecl) child;

			  string prefix = decl.Prefix;
			  string declNamespace = child.getNamespaceForPrefix(prefix);

			  if (@namespace.Equals(declNamespace))
			  {
				return decl;
			  }
			}
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// Execute the fallbacks when an extension is not available.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void executeFallbacks(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  private void executeFallbacks(TransformerImpl transformer)
	  {
		for (ElemTemplateElement child = m_firstChild; child != null; child = child.m_nextSibling)
		{
		  if (child.XSLToken == Constants.ELEMNAME_FALLBACK)
		  {
			try
			{
			  transformer.pushElemTemplateElement(child);
			  ((ElemFallback) child).executeFallback(transformer);
			}
			finally
			{
			  transformer.popElemTemplateElement();
			}
		  }
		}

	  }

	  /// <summary>
	  /// Return true if this extension element has a <xsl:fallback> child element.
	  /// </summary>
	  /// <returns> true if this extension element has a <xsl:fallback> child element. </returns>
	  private bool hasFallbackChildren()
	  {
		for (ElemTemplateElement child = m_firstChild; child != null; child = child.m_nextSibling)
		{
		  if (child.XSLToken == Constants.ELEMNAME_FALLBACK)
		  {
			return true;
		  }
		}

		return false;
	  }


	  /// <summary>
	  /// Execute an extension.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {
		if (transformer.Stylesheet.SecureProcessing)
		{
		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, new object[] {RawName}));
		}

		if (transformer.Debug)
		{
			transformer.TraceManager.fireTraceEvent(this);
		}
		try
		{
		  transformer.ResultTreeHandler.flushPending();

		  ExtensionsTable etable = transformer.getExtensionsTable();
		  ExtensionHandler nsh = etable.get(m_extns);

		  if (null == nsh)
		  {
			if (hasFallbackChildren())
			{
			  executeFallbacks(transformer);
			}
			else
			{
		  TransformerException te = new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CALL_TO_EXT_FAILED, new object[]{NodeName}));
		  transformer.ErrorListener.fatalError(te);
			}

			return;
		  }

		  try
		  {
			nsh.processElement(this.LocalName, this, transformer, Stylesheet, this);
		  }
		  catch (Exception e)
		  {

		if (hasFallbackChildren())
		{
		  executeFallbacks(transformer);
		}
		else
		{
			  if (e is TransformerException)
			  {
				TransformerException te = (TransformerException)e;
				if (null == te.Locator)
				{
				  te.Locator = this;
				}

				transformer.ErrorListener.fatalError(te);
			  }
			  else if (e is Exception)
			  {
				transformer.ErrorListener.fatalError(new TransformerException(e));
			  }
			  else
			  {
				transformer.ErrorListener.warning(new TransformerException(e));
			  }
		}
		  }
		}
		catch (TransformerException e)
		{
		  transformer.ErrorListener.fatalError(e);
		}
		catch (SAXException se)
		{
		  throw new TransformerException(se);
		}
		if (transformer.Debug)
		{
			transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	  /// <summary>
	  /// Return the value of the attribute interpreted as an Attribute
	  /// Value Template (in other words, you can use curly expressions
	  /// such as href="http://{website}".
	  /// </summary>
	  /// <param name="rawName"> Raw name of the attribute to get </param>
	  /// <param name="sourceNode"> non-null reference to the <a href="http://www.w3.org/TR/xslt#dt-current-node">current source node</a>. </param>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <returns> the value of the attribute
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getAttribute(String rawName, org.w3c.dom.Node sourceNode, org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public virtual string getAttribute(string rawName, org.w3c.dom.Node sourceNode, TransformerImpl transformer)
	  {

		AVT avt = getLiteralResultAttribute(rawName);

		if ((null != avt) && avt.RawName.Equals(rawName))
		{
		  XPathContext xctxt = transformer.XPathContext;

		  return avt.evaluate(xctxt, xctxt.getDTMHandleFromNode(sourceNode), this);
		}

		return null;
	  }

	  /// <summary>
	  /// Accept a visitor and call the appropriate method 
	  /// for this class.
	  /// </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  /// <returns> true if the children of the object should be visited. </returns>
	  protected internal override bool accept(XSLTVisitor visitor)
	  {
		  return visitor.visitExtensionElement(this);
	  }


	}

}