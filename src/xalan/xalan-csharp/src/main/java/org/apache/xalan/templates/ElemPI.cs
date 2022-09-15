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
 * $Id: ElemPI.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XML11Char = org.apache.xml.utils.XML11Char;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Implement xsl:processing-instruction.
	/// <pre>
	/// <!ELEMENT xsl:processing-instruction %char-template;>
	/// <!ATTLIST xsl:processing-instruction
	///   name %avt; #REQUIRED
	///   %space-att;
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Creating-Processing-Instructions">section-Creating-Processing-Instructions in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemPI : ElemTemplateElement
	{
		internal new const long serialVersionUID = 5621976448020889825L;

	  /// <summary>
	  /// The xsl:processing-instruction element has a required name
	  /// attribute that specifies the name of the processing instruction node.
	  /// The value of the name attribute is interpreted as an
	  /// attribute value template.
	  /// @serial
	  /// </summary>
	  private AVT m_name_atv = null;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// DJD
	  /// </summary>
	  /// <param name="v"> Value for the name attribute </param>
	  public virtual AVT Name
	  {
		  set
		  {
			m_name_atv = value;
		  }
		  get
		  {
			return m_name_atv;
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
		ArrayList vnames = sroot.ComposeState.VariableNames;
		if (null != m_name_atv)
		{
		  m_name_atv.fixupVariables(vnames, sroot.ComposeState.GlobalsSize);
		}
	  }



	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for the element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_PI;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_PI_STRING;
		  }
	  }

	  /// <summary>
	  /// Create a processing instruction in the result tree.
	  /// The content of the xsl:processing-instruction element is a
	  /// template for the string-value of the processing instruction node. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Creating-Processing-Instructions">section-Creating-Processing-Instructions in XSLT Specification</a>"
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

		XPathContext xctxt = transformer.XPathContext;
		int sourceNode = xctxt.CurrentNode;

		string piName = m_name_atv == null ? null : m_name_atv.evaluate(xctxt, sourceNode, this);

		// Ignore processing instruction if name is null
		if (string.ReferenceEquals(piName, null))
		{
			return;
		}

		if (piName.Equals("xml", StringComparison.OrdinalIgnoreCase))
		{
			 transformer.MsgMgr.warn(this, XSLTErrorResources.WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, new object[]{Constants.ATTRNAME_NAME, piName});
			return;
		}

		// Only check if an avt was used (ie. this wasn't checked at compose time.)
		// Ignore processing instruction, if invalid
		else if ((!m_name_atv.Simple) && (!XML11Char.isXML11ValidNCName(piName)))
		{
			 transformer.MsgMgr.warn(this, XSLTErrorResources.WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, new object[]{Constants.ATTRNAME_NAME, piName});
			return;
		}

		// Note the content model is:
		// <!ENTITY % instructions "
		// %char-instructions;
		// | xsl:processing-instruction
		// | xsl:comment
		// | xsl:element
		// | xsl:attribute
		// ">
		string data = transformer.transformToString(this);

		try
		{
		  transformer.ResultTreeHandler.processingInstruction(piName, data);
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// </summary>
	  /// <param name="newChild"> Child to add to child list
	  /// </param>
	  /// <returns> The child just added to the child list
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		int type = ((ElemTemplateElement) newChild).XSLToken;

		switch (type)
		{

		// char-instructions 
		case Constants.ELEMNAME_TEXTLITERALRESULT :
		case Constants.ELEMNAME_APPLY_TEMPLATES :
		case Constants.ELEMNAME_APPLY_IMPORTS :
		case Constants.ELEMNAME_CALLTEMPLATE :
		case Constants.ELEMNAME_FOREACH :
		case Constants.ELEMNAME_VALUEOF :
		case Constants.ELEMNAME_COPY_OF :
		case Constants.ELEMNAME_NUMBER :
		case Constants.ELEMNAME_CHOOSE :
		case Constants.ELEMNAME_IF :
		case Constants.ELEMNAME_TEXT :
		case Constants.ELEMNAME_COPY :
		case Constants.ELEMNAME_VARIABLE :
		case Constants.ELEMNAME_MESSAGE :

		  // instructions 
		  // case Constants.ELEMNAME_PI:
		  // case Constants.ELEMNAME_COMMENT:
		  // case Constants.ELEMNAME_ELEMENT:
		  // case Constants.ELEMNAME_ATTRIBUTE:
		  break;
		default :
		  error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
	  break;
		}

		return base.appendChild(newChild);
	  }
	}

}