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
 * $Id: NodeSortKey.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using XPath = org.apache.xpath.XPath;

	/// <summary>
	/// Data structure for use by the NodeSorter class.
	/// @xsl.usage internal
	/// </summary>
	internal class NodeSortKey
	{

	  /// <summary>
	  /// Select pattern for this sort key </summary>
	  internal XPath m_selectPat;

	  /// <summary>
	  /// Flag indicating whether to treat thee result as a number </summary>
	  internal bool m_treatAsNumbers;

	  /// <summary>
	  /// Flag indicating whether to sort in descending order </summary>
	  internal bool m_descending;

	  /// <summary>
	  /// Flag indicating by case </summary>
	  internal bool m_caseOrderUpper;

	  /// <summary>
	  /// Collator instance </summary>
	  internal Collator m_col;

	  /// <summary>
	  /// Locale we're in </summary>
	  internal Locale m_locale;

	  /// <summary>
	  /// Prefix resolver to use </summary>
	  internal org.apache.xml.utils.PrefixResolver m_namespaceContext;

	  /// <summary>
	  /// Transformer instance </summary>
	  internal TransformerImpl m_processor; // needed for error reporting.

	  /// <summary>
	  /// Constructor NodeSortKey
	  /// 
	  /// </summary>
	  /// <param name="transformer"> non null transformer instance </param>
	  /// <param name="selectPat"> Select pattern for this key </param>
	  /// <param name="treatAsNumbers"> Flag indicating whether the result will be a number </param>
	  /// <param name="descending"> Flag indicating whether to sort in descending order </param>
	  /// <param name="langValue"> Lang value to use to get locale </param>
	  /// <param name="caseOrderUpper"> Flag indicating whether case is relevant </param>
	  /// <param name="namespaceContext"> Prefix resolver
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: NodeSortKey(TransformerImpl transformer, org.apache.xpath.XPath selectPat, boolean treatAsNumbers, boolean descending, String langValue, boolean caseOrderUpper, org.apache.xml.utils.PrefixResolver namespaceContext) throws javax.xml.transform.TransformerException
	  internal NodeSortKey(TransformerImpl transformer, XPath selectPat, bool treatAsNumbers, bool descending, string langValue, bool caseOrderUpper, org.apache.xml.utils.PrefixResolver namespaceContext)
	  {

		m_processor = transformer;
		m_namespaceContext = namespaceContext;
		m_selectPat = selectPat;
		m_treatAsNumbers = treatAsNumbers;
		m_descending = descending;
		m_caseOrderUpper = caseOrderUpper;

		if (null != langValue && m_treatAsNumbers == false)
		{
		  // See http://nagoya.apache.org/bugzilla/show_bug.cgi?id=2851
		  // The constructor of Locale is defined as 
		  //   public Locale(String language, String country)
		  // with
		  //   language - lowercase two-letter ISO-639 code
		  //   country - uppercase two-letter ISO-3166 code
		  // a) language must be provided as a lower-case ISO-code 
		  //    instead of an upper-case code
		  // b) country must be provided as an ISO-code 
		  //    instead of a full localized country name (e.g. "France")
		  m_locale = new Locale(langValue.ToLower(), Locale.getDefault().getCountry());

		  // (old, before bug report 2851).
		  //  m_locale = new Locale(langValue.toUpperCase(),
		  //                        Locale.getDefault().getDisplayCountry());                    

		  if (null == m_locale)
		  {

			// m_processor.warn("Could not find locale for <sort xml:lang="+langValue);
			m_locale = Locale.getDefault();
		  }
		}
		else
		{
		  m_locale = Locale.getDefault();
		}

		m_col = Collator.getInstance(m_locale);

		if (null == m_col)
		{
		  m_processor.MsgMgr.warn(null, XSLTErrorResources.WG_CANNOT_FIND_COLLATOR, new object[]{langValue}); //"Could not find Collator for <sort xml:lang="+langValue);

		  m_col = Collator.getInstance();
		}
	  }
	}

}