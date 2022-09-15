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
 * $Id: ElemComment.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;

	/// <summary>
	/// Implement xsl:comment.
	/// <pre>
	/// <!ELEMENT xsl:comment %char-template;>
	/// <!ATTLIST xsl:comment %space-att;>
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Creating-Comments">section-Creating-Comments in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemComment : ElemTemplateElement
	{
		internal new const long serialVersionUID = -8813199122875770142L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_COMMENT;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> This element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_COMMENT_STRING;
		  }
	  }

	  /// <summary>
	  /// Execute the xsl:comment transformation 
	  /// 
	  /// </summary>
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
		try
		{
		  // Note the content model is:
		  // <!ENTITY % instructions "
		  // %char-instructions;
		  // | xsl:processing-instruction
		  // | xsl:comment
		  // | xsl:element
		  // | xsl:attribute
		  // ">
		  string data = transformer.transformToString(this);

		  transformer.ResultTreeHandler.comment(data);
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }
		}
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// </summary>
	  /// <param name="newChild"> Child to add to this node's child list
	  /// </param>
	  /// <returns> Child that was just added to child list
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