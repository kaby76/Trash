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
 * $Id: ElemUse.java 476466 2006-11-18 08:22:31Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using QName = org.apache.xml.utils.QName;

	/// <summary>
	/// Implement xsl:use.
	/// This acts as a superclass for ElemCopy, ElemAttributeSet,
	/// ElemElement, and ElemLiteralResult, on order to implement
	/// shared behavior the use-attribute-sets attribute. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.attribute-sets">attribute-sets in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemUse : ElemTemplateElement
	{
		internal new const long serialVersionUID = 5830057200289299736L;

	  /// <summary>
	  /// The value of the "use-attribute-sets" attribute.
	  /// @serial
	  /// </summary>
	  private QName[] m_attributeSetsNames = null;

	  /// <summary>
	  /// Set the "use-attribute-sets" attribute.
	  /// Attribute sets are used by specifying a use-attribute-sets
	  /// attribute on xsl:element, xsl:copy (see [7.5 Copying]) or
	  /// xsl:attribute-set elements. The value of the use-attribute-sets
	  /// attribute is a whitespace-separated list of names of attribute
	  /// sets. Each name is specified as a QName, which is expanded as
	  /// described in [2.4 Qualified Names].
	  /// </summary>
	  /// <param name="v"> The value to set for the "use-attribute-sets" attribute.  </param>
	  public virtual void setUseAttributeSets(ArrayList v)
	  {

		int n = v.Count;

		m_attributeSetsNames = new QName[n];

		for (int i = 0; i < n; i++)
		{
		  m_attributeSetsNames[i] = (QName) v[i];
		}
	  }

	  /// <summary>
	  /// Set the "use-attribute-sets" attribute.
	  /// Attribute sets are used by specifying a use-attribute-sets
	  /// attribute on xsl:element, xsl:copy (see [7.5 Copying]) or
	  /// xsl:attribute-set elements. The value of the use-attribute-sets
	  /// attribute is a whitespace-separated list of names of attribute
	  /// sets. Each name is specified as a QName, which is expanded as
	  /// described in [2.4 Qualified Names].
	  /// </summary>
	  /// <param name="v"> The value to set for the "use-attribute-sets" attribute.  </param>
	  public virtual void setUseAttributeSets(QName[] v)
	  {
		m_attributeSetsNames = v;
	  }

	  /// <summary>
	  /// Get the "use-attribute-sets" attribute.
	  /// Attribute sets are used by specifying a use-attribute-sets
	  /// attribute on xsl:element, xsl:copy (see [7.5 Copying]) or
	  /// xsl:attribute-set elements, or a xsl:use-attribute-sets attribute on
	  /// Literal Result Elements.
	  /// The value of the use-attribute-sets
	  /// attribute is a whitespace-separated list of names of attribute
	  /// sets. Each name is specified as a QName, which is expanded as
	  /// described in [2.4 Qualified Names].
	  /// </summary>
	  /// <returns> The value of the "use-attribute-sets" attribute.  </returns>
	  public virtual QName[] UseAttributeSets
	  {
		  get
		  {
			return m_attributeSetsNames;
		  }
	  }

	  /// <summary>
	  /// Add the attributes from the named attribute sets to the attribute list.
	  /// TODO: Error handling for: "It is an error if there are two attribute sets
	  /// with the same expanded-name and with equal import precedence and that both
	  /// contain the same attribute unless there is a definition of the attribute
	  /// set with higher import precedence that also contains the attribute."
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="stylesheet"> The owning root stylesheet
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void applyAttrSets(org.apache.xalan.transformer.TransformerImpl transformer, StylesheetRoot stylesheet) throws javax.xml.transform.TransformerException
	  public virtual void applyAttrSets(TransformerImpl transformer, StylesheetRoot stylesheet)
	  {
		applyAttrSets(transformer, stylesheet, m_attributeSetsNames);
	  }

	  /// <summary>
	  /// Add the attributes from the named attribute sets to the attribute list.
	  /// TODO: Error handling for: "It is an error if there are two attribute sets
	  /// with the same expanded-name and with equal import precedence and that both
	  /// contain the same attribute unless there is a definition of the attribute
	  /// set with higher import precedence that also contains the attribute."
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="stylesheet"> The owning root stylesheet </param>
	  /// <param name="attributeSetsNames"> List of attribute sets names to apply
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void applyAttrSets(org.apache.xalan.transformer.TransformerImpl transformer, StylesheetRoot stylesheet, org.apache.xml.utils.QName attributeSetsNames[]) throws javax.xml.transform.TransformerException
	  private void applyAttrSets(TransformerImpl transformer, StylesheetRoot stylesheet, QName[] attributeSetsNames)
	  {

		if (null != attributeSetsNames)
		{
		  int nNames = attributeSetsNames.Length;

		  for (int i = 0; i < nNames; i++)
		  {
			QName qname = attributeSetsNames[i];
			System.Collections.IList attrSets = stylesheet.getAttributeSetComposed(qname);

			if (null != attrSets)
			{
			  int nSets = attrSets.Count;

			  // Highest priority attribute set will be at the top,
			  // so process it last.
			  for (int k = nSets - 1; k >= 0 ; k--)
			  {
				ElemAttributeSet attrSet = (ElemAttributeSet) attrSets[k];

				attrSet.execute(transformer);
			  }
			}
			else
			{
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_ATTRIB_SET, new object[] {qname}),this);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Copy attributes specified by use-attribute-sets to the result tree.
	  /// Specifying a use-attribute-sets attribute is equivalent to adding
	  /// xsl:attribute elements for each of the attributes in each of the
	  /// named attribute sets to the beginning of the content of the element
	  /// with the use-attribute-sets attribute, in the same order in which
	  /// the names of the attribute sets are specified in the use-attribute-sets
	  /// attribute. It is an error if use of use-attribute-sets attributes
	  /// on xsl:attribute-set elements causes an attribute set to directly
	  /// or indirectly use itself.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		if (null != m_attributeSetsNames)
		{
		  applyAttrSets(transformer, StylesheetRoot, m_attributeSetsNames);
		}

	  }
	}

}