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
 * $Id: ElemAttributeSet.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;

	/// <summary>
	/// Implement xsl:attribute-set.
	/// <pre>
	/// &amp;!ELEMENT xsl:attribute-set (xsl:attribute)*>
	/// &amp;!ATTLIST xsl:attribute-set
	///   name %qname; #REQUIRED
	///   use-attribute-sets %qnames; #IMPLIED
	/// &amp;
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#attribute-sets">attribute-sets in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemAttributeSet : ElemUse
	{
		internal new const long serialVersionUID = -426740318278164496L;

	  /// <summary>
	  /// The name attribute specifies the name of the attribute set.
	  /// @serial
	  /// </summary>
	  public QName m_qname = null;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// The name attribute specifies the name of the attribute set.
	  /// </summary>
	  /// <param name="name"> Name attribute to set </param>
	  public virtual QName Name
	  {
		  set
		  {
			m_qname = value;
		  }
		  get
		  {
			return m_qname;
		  }
	  }


	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> Token ID of the element  </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_DEFINEATTRIBUTESET;
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
			return Constants.ELEMNAME_ATTRIBUTESET_STRING;
		  }
	  }

	  /// <summary>
	  /// Apply a set of attributes to the element.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		if (transformer.isRecursiveAttrSet(this))
		{
		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_XSLATTRSET_USED_ITSELF, new object[]{m_qname.LocalPart})); //"xsl:attribute-set '"+m_qname.m_localpart+
		}

		transformer.pushElemAttributeSet(this);
		base.execute(transformer);

		ElemAttribute attr = (ElemAttribute) FirstChildElem;

		while (null != attr)
		{
		  attr.execute(transformer);

		  attr = (ElemAttribute) attr.NextSiblingElem;
		}

		transformer.popElemAttributeSet();

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}

	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// <!ELEMENT xsl:attribute-set (xsl:attribute)*>
	  /// <!ATTLIST xsl:attribute-set
	  ///   name %qname; #REQUIRED
	  ///   use-attribute-sets %qnames; #IMPLIED
	  /// >
	  /// </summary>
	  /// <param name="newChild"> Child to be added to this node's list of children
	  /// </param>
	  /// <returns> The child that was just added to the list of children
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public virtual ElemTemplateElement appendChildElem(ElemTemplateElement newChild)
	  {

		int type = ((ElemTemplateElement) newChild).XSLToken;

		switch (type)
		{
		case Constants.ELEMNAME_ATTRIBUTE :
		  break;
		default :
		  error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
	  break;
		}

		return base.appendChild(newChild);
	  }

	  /// <summary>
	  /// This function is called during recomposition to
	  /// control how this element is composed. </summary>
	  /// <param name="root"> The root stylesheet for this transformation. </param>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeAttributeSets(this);
	  }

	}

}