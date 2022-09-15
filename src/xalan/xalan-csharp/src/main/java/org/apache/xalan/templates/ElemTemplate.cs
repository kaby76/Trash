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
 * $Id: ElemTemplate.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Implement xsl:template.
	/// <pre>
	/// <!ELEMENT xsl:template
	///  (#PCDATA
	///   %instructions;
	///   %result-elements;
	///   | xsl:param)
	/// >
	/// 
	/// <!ATTLIST xsl:template
	///   match %pattern; #IMPLIED
	///   name %qname; #IMPLIED
	///   priority %priority; #IMPLIED
	///   mode %qname; #IMPLIED
	///   %space-att;
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Defining-Template-Rules">section-Defining-Template-Rules in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemTemplate : ElemTemplateElement
	{
		internal new const long serialVersionUID = -5283056789965384058L;
	  /// <summary>
	  /// The public identifier for the current document event.
	  ///  @serial          
	  /// </summary>
	  private string m_publicId;

	  /// <summary>
	  /// The system identifier for the current document event.
	  ///  @serial          
	  /// </summary>
	  private string m_systemId;

	  /// <summary>
	  /// Return the public identifier for the current document event.
	  /// <para>This will be the public identifier
	  /// </para>
	  /// </summary>
	  /// <returns> A string containing the public identifier, or
	  ///         null if none is available. </returns>
	  /// <seealso cref=".getSystemId"/>
	  public override string PublicId
	  {
		  get
		  {
			return m_publicId;
		  }
	  }

	  /// <summary>
	  /// Return the system identifier for the current document event.
	  /// 
	  /// <para>If the system identifier is a URL, the parser must resolve it
	  /// fully before passing it to the application.</para>
	  /// </summary>
	  /// <returns> A string containing the system identifier, or null
	  ///         if none is available. </returns>
	  /// <seealso cref=".getPublicId"/>
	  public override string SystemId
	  {
		  get
		  {
			return m_systemId;
		  }
	  }

	  /// <summary>
	  /// Set the location information for this element.
	  /// </summary>
	  /// <param name="locator"> SourceLocator holding location information  </param>
	  public override SourceLocator LocaterInfo
	  {
		  set
		  {
    
			m_publicId = value.getPublicId();
			m_systemId = value.getSystemId();
    
			base.LocaterInfo = value;
		  }
	  }

	  /// <summary>
	  /// The owning stylesheet.
	  /// (Should this only be put on the template element, to
	  /// conserve space?)
	  /// @serial
	  /// </summary>
	  private Stylesheet m_stylesheet;

	  /// <summary>
	  /// Get the stylesheet composed (resolves includes and
	  /// imports and has methods on it that return "composed" properties.
	  /// </summary>
	  /// <returns> The stylesheet composed. </returns>
	  public override StylesheetComposed StylesheetComposed
	  {
		  get
		  {
			return m_stylesheet.StylesheetComposed;
		  }
	  }

	  /// <summary>
	  /// Get the owning stylesheet.
	  /// </summary>
	  /// <returns> The owning stylesheet. </returns>
	  public override Stylesheet Stylesheet
	  {
		  get
		  {
			return m_stylesheet;
		  }
		  set
		  {
			m_stylesheet = value;
		  }
	  }


	  /// <summary>
	  /// Get the root stylesheet.
	  /// </summary>
	  /// <returns> The root stylesheet for this element </returns>
	  public override StylesheetRoot StylesheetRoot
	  {
		  get
		  {
			return m_stylesheet.StylesheetRoot;
		  }
	  }

	  /// <summary>
	  /// The match attribute is a Pattern that identifies the source
	  /// node or nodes to which the rule applies.
	  /// @serial
	  /// </summary>
	  private XPath m_matchPattern = null;

	  /// <summary>
	  /// Set the "match" attribute.
	  /// The match attribute is a Pattern that identifies the source
	  /// node or nodes to which the rule applies. The match attribute
	  /// is required unless the xsl:template element has a name
	  /// attribute (see [6 Named Templates]). It is an error for the
	  /// value of the match attribute to contain a VariableReference. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.patterns">patterns in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Value to set for the "match" attribute </param>
	  public virtual XPath Match
	  {
		  set
		  {
			m_matchPattern = value;
		  }
		  get
		  {
			return m_matchPattern;
		  }
	  }

	  /// <summary>
	  /// Get the "match" attribute.
	  /// The match attribute is a Pattern that identifies the source
	  /// node or nodes to which the rule applies. The match attribute
	  /// is required unless the xsl:template element has a name
	  /// attribute (see [6 Named Templates]). It is an error for the
	  /// value of the match attribute to contain a VariableReference. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.patterns">patterns in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// An xsl:template element with a name attribute specifies a named template.
	  /// @serial
	  /// </summary>
	  private QName m_name = null;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// An xsl:template element with a name attribute specifies a named template.
	  /// If an xsl:template element has a name attribute, it may, but need not,
	  /// also have a match attribute. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.named-templates">named-templates in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Value to set the "name" attribute </param>
	  public virtual QName Name
	  {
		  set
		  {
			m_name = value;
		  }
		  get
		  {
			return m_name;
		  }
	  }

	  /// <summary>
	  /// Get the "name" attribute.
	  /// An xsl:template element with a name attribute specifies a named template.
	  /// If an xsl:template element has a name attribute, it may, but need not,
	  /// also have a match attribute. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.named-templates">named-templates in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// Modes allow an element to be processed multiple times,
	  /// each time producing a different result.
	  /// @serial
	  /// </summary>
	  private QName m_mode;

	  /// <summary>
	  /// Set the "mode" attribute.
	  /// Modes allow an element to be processed multiple times,
	  /// each time producing a different result.  If xsl:template
	  /// does not have a match attribute, it must not have a mode attribute. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.modes">modes in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Value to set the "mode" attribute </param>
	  public virtual QName Mode
	  {
		  set
		  {
			m_mode = value;
		  }
		  get
		  {
			return m_mode;
		  }
	  }

	  /// <summary>
	  /// Get the "mode" attribute.
	  /// Modes allow an element to be processed multiple times,
	  /// each time producing a different result.  If xsl:template
	  /// does not have a match attribute, it must not have a mode attribute. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.modes">modes in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// The priority of a template rule is specified by the priority
	  /// attribute on the template rule.
	  /// @serial
	  /// </summary>
	  private double m_priority = XPath.MATCH_SCORE_NONE;

	  /// <summary>
	  /// Set the "priority" attribute.
	  /// The priority of a template rule is specified by the priority
	  /// attribute on the template rule. The value of this must be a
	  /// real number (positive or negative), matching the production
	  /// Number with an optional leading minus sign (-). </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.conflict">conflict in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> The value to set for the "priority" attribute </param>
	  public virtual double Priority
	  {
		  set
		  {
			m_priority = value;
		  }
		  get
		  {
			return m_priority;
		  }
	  }

	  /// <summary>
	  /// Get the "priority" attribute.
	  /// The priority of a template rule is specified by the priority
	  /// attribute on the template rule. The value of this must be a
	  /// real number (positive or negative), matching the production
	  /// Number with an optional leading minus sign (-). </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.conflict">conflict in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for the element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_TEMPLATE;
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
			return Constants.ELEMNAME_TEMPLATE_STRING;
		  }
	  }

	  /// <summary>
	  /// The stack frame size for this template, which is equal to the maximum number 
	  /// of params and variables that can be declared in the template at one time.
	  /// </summary>
	  public int m_frameSize;

	  /// <summary>
	  /// The size of the portion of the stack frame that can hold parameter 
	  /// arguments.
	  /// </summary>
	  internal int m_inArgsSize;

	  /// <summary>
	  /// List of namespace/local-name pairs, DTM style, that are unique 
	  /// qname identifiers for the arguments.  The position of a given qname 
	  /// in the list is the argument ID, and thus the position in the stack
	  /// frame.
	  /// </summary>
	  private int[] m_argsQNameIDs;

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
		StylesheetRoot.ComposeState cstate = sroot.ComposeState;
		ArrayList vnames = cstate.VariableNames;
		if (null != m_matchPattern)
		{
		  m_matchPattern.fixupVariables(vnames, sroot.ComposeState.GlobalsSize);
		}

		cstate.resetStackFrameSize();
		m_inArgsSize = 0;
	  }

	  /// <summary>
	  /// This after the template's children have been composed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void endCompose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void endCompose(StylesheetRoot sroot)
	  {
		StylesheetRoot.ComposeState cstate = sroot.ComposeState;
		base.endCompose(sroot);
		m_frameSize = cstate.FrameSize;

		cstate.resetStackFrameSize();
	  }

	  /// <summary>
	  /// Copy the template contents into the result tree.
	  /// The content of the xsl:template element is the template
	  /// that is instantiated when the template rule is applied.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {
		XPathContext xctxt = transformer.XPathContext;

		transformer.StackGuard.checkForInfinateLoop();

		xctxt.pushRTFContext();

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		  // %REVIEW% commenting out of the code below.
	//    if (null != sourceNode)
	//    {
		  transformer.executeChildTemplates(this, true);
	//    }
	//    else  // if(null == sourceNode)
	//    {
	//      transformer.getMsgMgr().error(this,
	//        this, sourceNode,
	//        XSLTErrorResources.ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES);
	//
	//      //"sourceNode is null in handleApplyTemplatesInstruction!");
	//    }

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}

		xctxt.popRTFContext();
	  }

	  /// <summary>
	  /// This function is called during recomposition to
	  /// control how this element is composed. </summary>
	  /// <param name="root"> The root stylesheet for this transformation. </param>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeTemplates(this);
	  }

	}

}