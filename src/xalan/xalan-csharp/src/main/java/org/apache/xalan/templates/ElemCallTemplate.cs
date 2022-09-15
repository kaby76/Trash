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
 * $Id: ElemCallTemplate.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Implement xsl:call-template.
	/// <pre>
	/// &amp;!ELEMENT xsl:call-template (xsl:with-param)*>
	/// &amp;!ATTLIST xsl:call-template
	///   name %qname; #REQUIRED
	/// &amp;
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.named-templates">named-templates in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemCallTemplate : ElemForEach
	{
		internal new const long serialVersionUID = 5009634612916030591L;

	  /// <summary>
	  /// An xsl:call-template element invokes a template by name;
	  /// it has a required name attribute that identifies the template to be invoked.
	  /// @serial
	  /// </summary>
	  public QName m_templateName = null;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// An xsl:call-template element invokes a template by name;
	  /// it has a required name attribute that identifies the template to be invoked.
	  /// </summary>
	  /// <param name="name"> Name attribute to set </param>
	  public virtual QName Name
	  {
		  set
		  {
			m_templateName = value;
		  }
		  get
		  {
			return m_templateName;
		  }
	  }


	  /// <summary>
	  /// The template which is named by QName.
	  /// @serial
	  /// </summary>
	  private ElemTemplate m_template = null;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element  </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_CALLTEMPLATE;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The name of this element </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_CALLTEMPLATE_STRING;
		  }
	  }

	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);

		// Call compose on each param no matter if this is apply-templates 
		// or call templates.
		int length = ParamElemCount;
		for (int i = 0; i < length; i++)
		{
		  ElemWithParam ewp = getParamElem(i);
		  ewp.compose(sroot);
		}

		if ((null != m_templateName) && (null == m_template))
		{
			m_template = this.StylesheetRoot.getTemplateComposed(m_templateName);

			if (null == m_template)
			{
				string themsg = XSLMessages.createMessage(XSLTErrorResources.ER_ELEMTEMPLATEELEM_ERR, new object[] {m_templateName});

				throw new TransformerException(themsg, this);
				//"Could not find template named: '"+templateName+"'");
			}

		  length = ParamElemCount;
		  for (int i = 0; i < length; i++)
		  {
			ElemWithParam ewp = getParamElem(i);
			ewp.m_index = -1;
			// Find the position of the param in the template being called, 
			// and set the index of the param slot.
			int etePos = 0;
			for (ElemTemplateElement ete = m_template.FirstChildElem; null != ete; ete = ete.NextSiblingElem)
			{
			  if (ete.XSLToken == Constants.ELEMNAME_PARAMVARIABLE)
			  {
				ElemParam ep = (ElemParam)ete;
				if (ep.Name.Equals(ewp.Name))
				{
				  ewp.m_index = etePos;
				}
			  }
			  else
			  {
				break;
			  }
			  etePos++;
			}

		  }
		}
	  }

	  /// <summary>
	  /// This after the template's children have been composed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCompose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void endCompose(StylesheetRoot sroot)
	  {
		int length = ParamElemCount;
		for (int i = 0; i < length; i++)
		{
		  ElemWithParam ewp = getParamElem(i);
		  ewp.endCompose(sroot);
		}

		base.endCompose(sroot);
	  }

	  /// <summary>
	  /// Invoke a named template. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.named-templates">named-templates in XSLT Specification</a>"
	  ////>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		if (null != m_template)
		{
		  XPathContext xctxt = transformer.XPathContext;
		  VariableStack vars = xctxt.VarStack;

		  int thisframe = vars.StackFrame;
		  int nextFrame = vars.link(m_template.m_frameSize);

		  // We have to clear the section of the stack frame that has params 
		  // so that the default param evaluation will work correctly.
		  if (m_template.m_inArgsSize > 0)
		  {
			vars.clearLocalSlots(0, m_template.m_inArgsSize);

			if (null != m_paramElems)
			{
			  int currentNode = xctxt.CurrentNode;
			  vars.StackFrame = thisframe;
			  int size = m_paramElems.Length;

			  for (int i = 0; i < size; i++)
			  {
				ElemWithParam ewp = m_paramElems[i];
				if (ewp.m_index >= 0)
				{
				  if (transformer.Debug)
				  {
					transformer.TraceManager.fireTraceEvent(ewp);
				  }
				  XObject obj = ewp.getValue(transformer, currentNode);
				  if (transformer.Debug)
				  {
					transformer.TraceManager.fireTraceEndEvent(ewp);
				  }

				  // Note here that the index for ElemWithParam must have been 
				  // statically made relative to the xsl:template being called, 
				  // NOT this xsl:template.
				  vars.setLocalVariable(ewp.m_index, obj, nextFrame);
				}
			  }
			  vars.StackFrame = nextFrame;
			}
		  }

		  SourceLocator savedLocator = xctxt.SAXLocator;

		  try
		  {
			xctxt.SAXLocator = m_template;

			// template.executeChildTemplates(transformer, sourceNode, mode, true);
			transformer.pushElemTemplateElement(m_template);
			m_template.execute(transformer);
		  }
		  finally
		  {
			transformer.popElemTemplateElement();
			xctxt.SAXLocator = savedLocator;
			// When we entered this function, the current 
			// frame buffer (cfb) index in the variable stack may 
			// have been manually set.  If we just call 
			// unlink(), however, it will restore the cfb to the 
			// previous link index from the link stack, rather than 
			// the manually set cfb.  So, 
			// the only safe solution is to restore it back 
			// to the same position it was on entry, since we're 
			// really not working in a stack context here. (Bug4218)
			vars.unlink(thisframe);
		  }
		}
		else
		{
		  transformer.MsgMgr.error(this, XSLTErrorResources.ER_TEMPLATE_NOT_FOUND, new object[]{m_templateName}); //"Could not find template named: '"+templateName+"'");
		}

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}

	  }

	  /// <summary>
	  /// Vector of xsl:param elements associated with this element. 
	  ///  @serial 
	  /// </summary>
	  protected internal ElemWithParam[] m_paramElems = null;

	  /// <summary>
	  /// Get the count xsl:param elements associated with this element. </summary>
	  /// <returns> The number of xsl:param elements. </returns>
	  public virtual int ParamElemCount
	  {
		  get
		  {
			return (m_paramElems == null) ? 0 : m_paramElems.Length;
		  }
	  }

	  /// <summary>
	  /// Get a xsl:param element associated with this element.
	  /// </summary>
	  /// <param name="i"> Index of element to find
	  /// </param>
	  /// <returns> xsl:param element at given index </returns>
	  public virtual ElemWithParam getParamElem(int i)
	  {
		return m_paramElems[i];
	  }

	  /// <summary>
	  /// Set a xsl:param element associated with this element.
	  /// </summary>
	  /// <param name="ParamElem"> xsl:param element to set.  </param>
	  public virtual ElemWithParam ParamElem
	  {
		  set
		  {
			if (null == m_paramElems)
			{
			  m_paramElems = new ElemWithParam[1];
			  m_paramElems[0] = value;
			}
			else
			{
			  // Expensive 1 at a time growth, but this is done at build time, so 
			  // I think it's OK.
			  int length = m_paramElems.Length;
			  ElemWithParam[] ewp = new ElemWithParam[length + 1];
			  Array.Copy(m_paramElems, 0, ewp, 0, length);
			  m_paramElems = ewp;
			  ewp[length] = value;
			}
		  }
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// <!ELEMENT xsl:apply-templates (xsl:sort|xsl:with-param)*>
	  /// <!ATTLIST xsl:apply-templates
	  ///   select %expr; "node()"
	  ///   mode %qname; #IMPLIED
	  /// >
	  /// </summary>
	  /// <param name="newChild"> Child to add to this node's children list
	  /// </param>
	  /// <returns> The child that was just added the children list 
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		int type = ((ElemTemplateElement) newChild).XSLToken;

		if (Constants.ELEMNAME_WITHPARAM == type)
		{
		  ParamElem = (ElemWithParam) newChild;
		}

		// You still have to append, because this element can
		// contain a for-each, and other elements.
		return base.appendChild(newChild);
	  }

		/// <summary>
		/// Call the children visitors. </summary>
		/// <param name="visitor"> The visitor whose appropriate method will be called. </param>
		public override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
		{
	//      if (null != m_paramElems)
	//      {
	//        int size = m_paramElems.length;
	//
	//        for (int i = 0; i < size; i++)
	//        {
	//          ElemWithParam ewp = m_paramElems[i];
	//          ewp.callVisitors(visitor);
	//        }
	//      }

		  base.callChildVisitors(visitor, callAttrs);
		}
	}

}