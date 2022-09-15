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
 * $Id: ElemApplyImport.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;

	/// <summary>
	/// Implement xsl:apply-imports.
	/// <pre>
	/// <!ELEMENT xsl:apply-imports EMPTY>
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#apply-imports">apply-imports in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemApplyImport : ElemTemplateElement
	{
		internal new const long serialVersionUID = 3764728663373024038L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> Token ID for xsl:apply-imports element types </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_APPLY_IMPORTS;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> Element name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_APPLY_IMPORTS_STRING;
		  }
	  }

	  /// <summary>
	  /// Execute the xsl:apply-imports transformation.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		if (transformer.currentTemplateRuleIsNull())
		{
		  transformer.MsgMgr.error(this, XSLTErrorResources.ER_NO_APPLY_IMPORT_IN_FOR_EACH); //"xsl:apply-imports not allowed in a xsl:for-each");
		}

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		int sourceNode = transformer.XPathContext.CurrentNode;
		if (org.apache.xml.dtm.DTM_Fields.NULL != sourceNode)
		{
		  // supply the current templated (matched, not named)        
		  ElemTemplate matchTemplate = transformer.MatchedTemplate;
		  transformer.applyTemplateToNode(this, matchTemplate, sourceNode);
		}
		else // if(null == sourceNode)
		{
		  transformer.MsgMgr.error(this, XSLTErrorResources.ER_NULL_SOURCENODE_APPLYIMPORTS); //"sourceNode is null in xsl:apply-imports!");
		}
		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// <!ELEMENT xsl:apply-imports EMPTY>
	  /// </summary>
	  /// <param name="newChild"> New element to append to this element's children list
	  /// </param>
	  /// <returns> null, xsl:apply-Imports cannot have children  </returns>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
		return null;
	  }
	}

}