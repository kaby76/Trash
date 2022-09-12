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
 * $Id: KeyTable.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{


	using KeyDeclaration = org.apache.xalan.templates.KeyDeclaration;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using QName = org.apache.xml.utils.QName;
	using WrappedRuntimeException = org.apache.xml.utils.WrappedRuntimeException;
	using XMLString = org.apache.xml.utils.XMLString;
	using XPathContext = org.apache.xpath.XPathContext;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Table of element keys, keyed by document node.  An instance of this
	/// class is keyed by a Document node that should be matched with the
	/// root of the current context.
	/// @xsl.usage advanced
	/// </summary>
	public class KeyTable
	{
	  /// <summary>
	  /// The document key.  This table should only be used with contexts
	  /// whose Document roots match this key.
	  /// </summary>
	  private int m_docKey;

	  /// <summary>
	  /// Vector of KeyDeclaration instances holding the key declarations.
	  /// </summary>
	  private ArrayList m_keyDeclarations;

	  /// <summary>
	  /// Hold a cache of key() function result for each ref.
	  /// Key is XMLString, the ref value
	  /// Value is XNodeSet, the key() function result for the given ref value.
	  /// </summary>
	  private Hashtable m_refsTable = null;

	  /// <summary>
	  /// Get the document root matching this key.  
	  /// </summary>
	  /// <returns> the document root matching this key </returns>
	  public virtual int DocKey
	  {
		  get
		  {
			return m_docKey;
		  }
	  }

	  /// <summary>
	  /// The main iterator that will walk through the source  
	  /// tree for this key.
	  /// </summary>
	  private XNodeSet m_keyNodes;

	  internal virtual KeyIterator KeyIterator
	  {
		  get
		  {
			  return (KeyIterator)(m_keyNodes.ContainedIter);
		  }
	  }

	  /// <summary>
	  /// Build a keys table. </summary>
	  /// <param name="doc"> The owner document key. </param>
	  /// <param name="nscontext"> The stylesheet's namespace context. </param>
	  /// <param name="name"> The key name </param>
	  /// <param name="keyDeclarations"> The stylesheet's xsl:key declarations.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KeyTable(int doc, org.apache.xml.utils.PrefixResolver nscontext, org.apache.xml.utils.QName name, java.util.Vector keyDeclarations, org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public KeyTable(int doc, PrefixResolver nscontext, QName name, ArrayList keyDeclarations, XPathContext xctxt)
	  {
		m_docKey = doc;
		m_keyDeclarations = keyDeclarations;
		KeyIterator ki = new KeyIterator(name, keyDeclarations);

		m_keyNodes = new XNodeSet(ki);
		m_keyNodes.allowDetachToRelease(false);
		m_keyNodes.setRoot(doc, xctxt);
	  }

	  /// <summary>
	  /// Given a valid element key, return the corresponding node list.
	  /// </summary>
	  /// <param name="name"> The name of the key, which must match the 'name' attribute on xsl:key. </param>
	  /// <param name="ref"> The value that must match the value found by the 'match' attribute on xsl:key. </param>
	  /// <returns> a set of nodes referenced by the key named <CODE>name</CODE> and the reference <CODE>ref</CODE>. If no node is referenced by this key, an empty node set is returned. </returns>
	  public virtual XNodeSet getNodeSetDTMByKey(QName name, XMLString @ref)

	  {
		XNodeSet refNodes = (XNodeSet) RefsTable[@ref];
		// clone wiht reset the node set
	   try
	   {
		  if (refNodes != null)
		  {
			 refNodes = (XNodeSet) refNodes.cloneWithReset();
		  }
	   }
		catch (CloneNotSupportedException)
		{
		  refNodes = null;
		}

		if (refNodes == null)
		{
		 //  create an empty XNodeSet
		  KeyIterator ki = (KeyIterator)(m_keyNodes).ContainedIter;
		  XPathContext xctxt = ki.XPathContext;
		  refNodes = new XNodeSetAnonymousInnerClass(this, xctxt.DTMManager);
		  refNodes.reset();
		}

		return refNodes;
	  }

	  private class XNodeSetAnonymousInnerClass : XNodeSet
	  {
		  private readonly KeyTable outerInstance;

		  public XNodeSetAnonymousInnerClass(KeyTable outerInstance, org.apache.xml.dtm.DTMManager getDTMManager) : base(getDTMManager)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override void setRoot(int nodeHandle, object environment)
		  {
			// Root cannot be set on non-iterated node sets. Ignore it.
		  }
	  }

	  /// <summary>
	  /// Get Key Name for this KeyTable  
	  /// </summary>
	  /// <returns> Key name </returns>
	  public virtual QName KeyTableName
	  {
		  get
		  {
			return KeyIterator.Name;
		  }
	  }

	  /// <returns> key declarations for the key associated to this KeyTable </returns>
	  private ArrayList KeyDeclarations
	  {
		  get
		  {
			int nDeclarations = m_keyDeclarations.Count;
			ArrayList keyDecls = new ArrayList(nDeclarations);
    
			// Walk through each of the declarations made with xsl:key
			for (int i = 0; i < nDeclarations; i++)
			{
			  KeyDeclaration kd = (KeyDeclaration) m_keyDeclarations[i];
    
			  // Add the declaration if the name on this key declaration
			  // matches the name on the iterator for this walker.
			  if (kd.Name.Equals(KeyTableName))
			  {
				keyDecls.Add(kd);
			  }
			}
    
			return keyDecls;
		  }
	  }

	  /// <returns> lazy initialized refs table associating evaluation of key function
	  ///         with a XNodeSet </returns>
	  private Hashtable RefsTable
	  {
		  get
		  {
			if (m_refsTable == null)
			{
			  // initial capacity set to a prime number to improve hash performance
			  m_refsTable = new Hashtable(89);
    
			  KeyIterator ki = (KeyIterator)(m_keyNodes).ContainedIter;
			  XPathContext xctxt = ki.XPathContext;
    
			  ArrayList keyDecls = KeyDeclarations;
			  int nKeyDecls = keyDecls.Count;
    
			  int currentNode;
			  m_keyNodes.reset();
			  while (org.apache.xml.dtm.DTM_Fields.NULL != (currentNode = m_keyNodes.nextNode()))
			  {
				try
				{
				  for (int keyDeclIdx = 0; keyDeclIdx < nKeyDecls; keyDeclIdx++)
				  {
					KeyDeclaration keyDeclaration = (KeyDeclaration) keyDecls[keyDeclIdx];
					XObject xuse = keyDeclaration.Use.execute(xctxt, currentNode, ki.PrefixResolver);
    
					if (xuse.Type != XObject.CLASS_NODESET)
					{
					  XMLString exprResult = xuse.xstr();
					  addValueInRefsTable(xctxt, exprResult, currentNode);
					}
					else
					{
					  DTMIterator i = ((XNodeSet)xuse).iterRaw();
					  int currentNodeInUseClause;
    
					  while (org.apache.xml.dtm.DTM_Fields.NULL != (currentNodeInUseClause = i.nextNode()))
					  {
						DTM dtm = xctxt.getDTM(currentNodeInUseClause);
						XMLString exprResult = dtm.getStringValue(currentNodeInUseClause);
						addValueInRefsTable(xctxt, exprResult, currentNode);
					  }
					}
				  }
				}
				catch (TransformerException te)
				{
				  throw new WrappedRuntimeException(te);
				}
			  }
			}
			return m_refsTable;
		  }
	  }

	  /// <summary>
	  /// Add an association between a ref and a node in the m_refsTable.
	  /// Requires that m_refsTable != null </summary>
	  /// <param name="xctxt"> XPath context </param>
	  /// <param name="ref"> the value of the use clause of the current key for the given node </param>
	  /// <param name="node"> the node to reference </param>
	  private void addValueInRefsTable(XPathContext xctxt, XMLString @ref, int node)
	  {

		XNodeSet nodes = (XNodeSet) m_refsTable[@ref];
		if (nodes == null)
		{
		  nodes = new XNodeSet(node, xctxt.DTMManager);
		  nodes.nextNode();
		  m_refsTable[@ref] = nodes;
		}
		else
		{
		  // Nodes are passed to this method in document order.  Since we need to
		  // suppress duplicates, we only need to check against the last entry
		  // in each nodeset.  We use nodes.nextNode after each entry so we can
		  // easily compare node against the current node.
		  if (nodes.CurrentNode != node)
		  {
			  nodes.mutableNodeset().addNode(node);
			  nodes.nextNode();
		  }
		}
	  }
	}

}