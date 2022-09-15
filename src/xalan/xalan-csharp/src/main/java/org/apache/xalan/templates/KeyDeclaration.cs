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
 * $Id: KeyDeclaration.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	using QName = org.apache.xml.utils.QName;
	using XPath = org.apache.xpath.XPath;

	/// <summary>
	/// Holds the attribute declarations for the xsl:keys element.
	/// A stylesheet declares a set of keys for each document using
	/// the xsl:key element. When this set of keys contains a member
	/// with node x, name y and value z, we say that node x has a key
	/// with name y and value z. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.key">key in XSLT Specification</a>"
	/// @xsl.usage internal/>
	[Serializable]
	public class KeyDeclaration : ElemTemplateElement
	{
		internal new const long serialVersionUID = 7724030248631137918L;

	  /// <summary>
	  /// Constructs a new element representing the xsl:key.  The parameters
	  /// are needed to prioritize this key element as part of the recomposing
	  /// process.  For this element, they are not automatically created
	  /// because the element is never added on to the stylesheet parent.
	  /// </summary>
	  public KeyDeclaration(Stylesheet parentNode, int docOrderNumber)
	  {
		m_parentNode = parentNode;
		Uid = docOrderNumber;
	  }

	  /// <summary>
	  /// The "name" property.
	  /// @serial
	  /// </summary>
	  private QName m_name;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// The name attribute specifies the name of the key. The value
	  /// of the name attribute is a QName, which is expanded as
	  /// described in [2.4 Qualified Names].
	  /// </summary>
	  /// <param name="name"> Value to set for the "name" attribute. </param>
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
	  /// Return the node name.
	  /// </summary>
	  /// <returns> the element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_KEY_STRING;
		  }
	  }


	  /// <summary>
	  /// The "match" attribute.
	  /// @serial
	  /// </summary>
	  private XPath m_matchPattern = null;

	  /// <summary>
	  /// Set the "match" attribute.
	  /// The match attribute is a Pattern; an xsl:key element gives
	  /// information about the keys of any node that matches the
	  /// pattern specified in the match attribute. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.patterns">patterns in XSLT Specification</a>"
	  ////>
	  /// <param name="v"> Value to set for the "match" attribute. </param>
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
	  /// The match attribute is a Pattern; an xsl:key element gives
	  /// information about the keys of any node that matches the
	  /// pattern specified in the match attribute. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.patterns">patterns in XSLT Specification</a>"
	  ////>

	  /// <summary>
	  /// The "use" attribute.
	  /// @serial
	  /// </summary>
	  private XPath m_use;

	  /// <summary>
	  /// Set the "use" attribute.
	  /// The use attribute is an expression specifying the values
	  /// of the key; the expression is evaluated once for each node
	  /// that matches the pattern.
	  /// </summary>
	  /// <param name="v"> Value to set for the "use" attribute. </param>
	  public virtual XPath Use
	  {
		  set
		  {
			m_use = value;
		  }
		  get
		  {
			return m_use;
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
			return Constants.ELEMNAME_KEY;
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
		if (null != m_matchPattern)
		{
		  m_matchPattern.fixupVariables(vnames, sroot.ComposeState.GlobalsSize);
		}
		if (null != m_use)
		{
		  m_use.fixupVariables(vnames, sroot.ComposeState.GlobalsSize);
		}
	  }

	  /// <summary>
	  /// This function is called during recomposition to
	  /// control how this element is composed. </summary>
	  /// <param name="root"> The root stylesheet for this transformation. </param>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeKeys(this);
	  }

	}

}