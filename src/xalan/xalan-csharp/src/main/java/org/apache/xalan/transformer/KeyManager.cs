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
 * $Id: KeyManager.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using QName = org.apache.xml.utils.QName;
	using XMLString = org.apache.xml.utils.XMLString;
	using XPathContext = org.apache.xpath.XPathContext;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;

	/// <summary>
	/// This class manages the key tables.
	/// </summary>
	public class KeyManager
	{

	  /// <summary>
	  /// Table of tables of element keys. </summary>
	  /// <seealso cref= org.apache.xalan.transformer.KeyTable </seealso>
	  [NonSerialized]
	  private ArrayList m_key_tables = null;

	  /// <summary>
	  /// Given a valid element key, return the corresponding node list.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state </param>
	  /// <param name="doc"> The document node </param>
	  /// <param name="name"> The key element name </param>
	  /// <param name="ref"> The key value we're looking for </param>
	  /// <param name="nscontext"> The prefix resolver for the execution context
	  /// </param>
	  /// <returns> A nodelist of nodes mathing the given key
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XNodeSet getNodeSetDTMByKey(org.apache.xpath.XPathContext xctxt, int doc, org.apache.xml.utils.QName name, org.apache.xml.utils.XMLString ref, org.apache.xml.utils.PrefixResolver nscontext) throws javax.xml.transform.TransformerException
	  public virtual XNodeSet getNodeSetDTMByKey(XPathContext xctxt, int doc, QName name, XMLString @ref, PrefixResolver nscontext)
	  {

		XNodeSet nl = null;
		ElemTemplateElement template = (ElemTemplateElement) nscontext; // yuck -sb

		if ((null != template) && null != template.StylesheetRoot.KeysComposed)
		{
		  bool foundDoc = false;

		  if (null == m_key_tables)
		  {
			m_key_tables = new ArrayList(4);
		  }
		  else
		  {
			int nKeyTables = m_key_tables.Count;

			for (int i = 0; i < nKeyTables; i++)
			{
			  KeyTable kt = (KeyTable) m_key_tables[i];

			  if (kt.KeyTableName.Equals(name) && doc == kt.DocKey)
			  {
				nl = kt.getNodeSetDTMByKey(name, @ref);

				if (nl != null)
				{
				  foundDoc = true;

				  break;
				}
			  }
			}
		  }

		  if ((null == nl) && !foundDoc)
		  {
			KeyTable kt = new KeyTable(doc, nscontext, name, template.StylesheetRoot.KeysComposed, xctxt);

			m_key_tables.Add(kt);

			if (doc == kt.DocKey)
			{
			  foundDoc = true;
			  nl = kt.getNodeSetDTMByKey(name, @ref);
			}
		  }
		}

		return nl;
	  }
	}

}