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
 * $Id: NamespaceAlias.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	/// <summary>
	/// Object to hold an xsl:namespace element.
	/// A stylesheet can use the xsl:namespace-alias element to declare
	/// that one namespace URI is an alias for another namespace URI. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.literal-result-element">literal-result-element in XSLT Specification</a>"/>
	[Serializable]
	public class NamespaceAlias : ElemTemplateElement
	{
		internal new const long serialVersionUID = 456173966637810718L;

	  /// <summary>
	  /// Constructor NamespaceAlias
	  /// </summary>
	  /// <param name="docOrderNumber"> The document order number
	  ///  </param>
	  public NamespaceAlias(int docOrderNumber) : base()
	  {
		m_docOrderNumber = docOrderNumber;
	  }

	  /// <summary>
	  /// The "stylesheet-prefix" attribute.
	  /// @serial
	  /// </summary>
	  private string m_StylesheetPrefix;

	  /// <summary>
	  /// Set the "stylesheet-prefix" attribute.
	  /// </summary>
	  /// <param name="v"> non-null prefix value. </param>
	  public virtual string StylesheetPrefix
	  {
		  set
		  {
			m_StylesheetPrefix = value;
		  }
		  get
		  {
			return m_StylesheetPrefix;
		  }
	  }


	  /// <summary>
	  /// The namespace in the stylesheet space.
	  /// @serial
	  /// </summary>
	  private string m_StylesheetNamespace;

	  /// <summary>
	  /// Set the value for the stylesheet namespace.
	  /// </summary>
	  /// <param name="v"> non-null prefix value. </param>
	  public virtual string StylesheetNamespace
	  {
		  set
		  {
			m_StylesheetNamespace = value;
		  }
		  get
		  {
			return m_StylesheetNamespace;
		  }
	  }


	  /// <summary>
	  /// The "result-prefix" attribute.
	  /// @serial
	  /// </summary>
	  private string m_ResultPrefix;

	  /// <summary>
	  /// Set the "result-prefix" attribute.
	  /// </summary>
	  /// <param name="v"> non-null prefix value. </param>
	  public virtual string ResultPrefix
	  {
		  set
		  {
			m_ResultPrefix = value;
		  }
		  get
		  {
			return m_ResultPrefix;
		  }
	  }


	  /// <summary>
	  /// The result namespace.
	  /// @serial
	  /// </summary>
	  private string m_ResultNamespace;

	  /// <summary>
	  /// Set the result namespace.
	  /// </summary>
	  /// <param name="v"> non-null namespace value </param>
	  public virtual string ResultNamespace
	  {
		  set
		  {
			m_ResultNamespace = value;
		  }
		  get
		  {
			return m_ResultNamespace;
		  }
	  }


	  /// <summary>
	  /// This function is called to recompose() all of the namespace alias properties elements.
	  /// </summary>
	  /// <param name="root"> The owning root stylesheet </param>
	  public override void recompose(StylesheetRoot root)
	  {
		root.recomposeNamespaceAliases(this);
	  }

	}

}