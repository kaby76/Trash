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
 * $Id: SourceTreeManager.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{



	using DTM = org.apache.xml.dtm.DTM;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;

	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// This class bottlenecks all management of source trees.  The methods
	/// in this class should allow easy garbage collection of source
	/// trees (not yet!), and should centralize parsing for those source trees.
	/// </summary>
	public class SourceTreeManager
	{

	  /// <summary>
	  /// Vector of SourceTree objects that this manager manages. </summary>
	  private ArrayList m_sourceTree = new ArrayList();

	  /// <summary>
	  /// Reset the list of SourceTree objects that this manager manages.
	  /// 
	  /// </summary>
	  public virtual void reset()
	  {
		m_sourceTree = new ArrayList();
	  }

	  /// <summary>
	  /// The TrAX URI resolver used to obtain source trees. </summary>
	  internal URIResolver m_uriResolver;

	  /// <summary>
	  /// Set an object that will be used to resolve URIs used in
	  /// document(), etc. </summary>
	  /// <param name="resolver"> An object that implements the URIResolver interface,
	  /// or null. </param>
	  public virtual URIResolver URIResolver
	  {
		  set
		  {
			m_uriResolver = value;
		  }
		  get
		  {
			return m_uriResolver;
		  }
	  }


	  /// <summary>
	  /// Given a document, find the URL associated with that document. </summary>
	  /// <param name="owner"> Document that was previously processed by this liaison.
	  /// </param>
	  /// <returns> The base URI of the owner argument. </returns>
	  public virtual string findURIFromDoc(int owner)
	  {
		int n = m_sourceTree.Count;

		for (int i = 0; i < n; i++)
		{
		  SourceTree sTree = (SourceTree) m_sourceTree[i];

		  if (owner == sTree.m_root)
		  {
			return sTree.m_url;
		  }
		}

		return null;
	  }

	  /// <summary>
	  /// This will be called by the processor when it encounters
	  /// an xsl:include, xsl:import, or document() function.
	  /// </summary>
	  /// <param name="base"> The base URI that should be used. </param>
	  /// <param name="urlString"> Value from an xsl:import or xsl:include's href attribute,
	  /// or a URI specified in the document() function.
	  /// </param>
	  /// <returns> a Source that can be used to process the resource.
	  /// </returns>
	  /// <exception cref="IOException"> </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public javax.xml.transform.Source resolveURI(String super, String urlString, javax.xml.transform.SourceLocator locator) throws javax.xml.transform.TransformerException, java.io.IOException
	  public virtual Source resolveURI(string @base, string urlString, SourceLocator locator)
	  {

		Source source = null;

		if (null != m_uriResolver)
		{
		  source = m_uriResolver.resolve(urlString, @base);
		}

		if (null == source)
		{
		  string uri = SystemIDResolver.getAbsoluteURI(urlString, @base);

		  source = new StreamSource(uri);
		}

		return source;
	  }

	  /// <summary>
	  /// JJK: Support  <?xalan:doc_cache_off?> kluge in ElemForEach.
	  /// TODO: This function is highly dangerous. Cache management must be improved.
	  /// </summary>
	  /// <param name="n"> The node to remove. </param>
	  public virtual void removeDocumentFromCache(int n)
	  {
		if (org.apache.xml.dtm.DTM_Fields.NULL == n)
		{
		  return;
		}
		for (int i = m_sourceTree.Count - 1;i >= 0;--i)
		{
		  SourceTree st = (SourceTree)m_sourceTree[i];
		  if (st != null && st.m_root == n)
		  {
		m_sourceTree.RemoveAt(i);
		return;
		  }
		}
	  }



	  /// <summary>
	  /// Put the source tree root node in the document cache.
	  /// TODO: This function needs to be a LOT more sophisticated.
	  /// </summary>
	  /// <param name="n"> The node to cache. </param>
	  /// <param name="source"> The Source object to cache. </param>
	  public virtual void putDocumentInCache(int n, Source source)
	  {

		int cachedNode = getNode(source);

		if (org.apache.xml.dtm.DTM_Fields.NULL != cachedNode)
		{
		  if (!(cachedNode == n))
		  {
			throw new Exception("Programmer's Error!  " + "putDocumentInCache found reparse of doc: " + source.SystemId);
		  }
		  return;
		}
		if (null != source.SystemId)
		{
		  m_sourceTree.Add(new SourceTree(n, source.SystemId));
		}
	  }

	  /// <summary>
	  /// Given a Source object, find the node associated with it.
	  /// </summary>
	  /// <param name="source"> The Source object to act as the key.
	  /// </param>
	  /// <returns> The node that is associated with the Source, or null if not found. </returns>
	  public virtual int getNode(Source source)
	  {

	//    if (source instanceof DOMSource)
	//      return ((DOMSource) source).getNode();

		// TODO: Not sure if the BaseID is really the same thing as the ID.
		string url = source.SystemId;

		if (null == url)
		{
		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		int n = m_sourceTree.Count;

		// System.out.println("getNode: "+n);
		for (int i = 0; i < n; i++)
		{
		  SourceTree sTree = (SourceTree) m_sourceTree[i];

		  // System.out.println("getNode -         url: "+url);
		  // System.out.println("getNode - sTree.m_url: "+sTree.m_url);
		  if (url.Equals(sTree.m_url))
		  {
			return sTree.m_root;
		  }
		}

		// System.out.println("getNode - returning: "+node);
		return org.apache.xml.dtm.DTM_Fields.NULL;
	  }

	  /// <summary>
	  /// Get the source tree from the a base URL and a URL string.
	  /// </summary>
	  /// <param name="base"> The base URI to use if the urlString is relative. </param>
	  /// <param name="urlString"> An absolute or relative URL string. </param>
	  /// <param name="locator"> The location of the caller, for diagnostic purposes.
	  /// </param>
	  /// <returns> should be a non-null reference to the node identified by the 
	  /// base and urlString.
	  /// </returns>
	  /// <exception cref="TransformerException"> If the URL can not resolve to a node. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSourceTree(String super, String urlString, javax.xml.transform.SourceLocator locator, XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual int getSourceTree(string @base, string urlString, SourceLocator locator, XPathContext xctxt)
	  {

		// System.out.println("getSourceTree");
		try
		{
		  Source source = this.resolveURI(@base, urlString, locator);

		  // System.out.println("getSourceTree - base: "+base+", urlString: "+urlString+", source: "+source.getSystemId());
		  return getSourceTree(source, locator, xctxt);
		}
		catch (IOException ioe)
		{
		  throw new TransformerException(ioe.Message, locator, ioe);
		}

		/* catch (TransformerException te)
		 {
		   throw new TransformerException(te.getMessage(), locator, te);
		 }*/
	  }

	  /// <summary>
	  /// Get the source tree from the input source.
	  /// </summary>
	  /// <param name="source"> The Source object that should identify the desired node. </param>
	  /// <param name="locator"> The location of the caller, for diagnostic purposes.
	  /// </param>
	  /// <returns> non-null reference to a node.
	  /// </returns>
	  /// <exception cref="TransformerException"> if the Source argument can't be resolved to 
	  ///         a node. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getSourceTree(javax.xml.transform.Source source, javax.xml.transform.SourceLocator locator, XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual int getSourceTree(Source source, SourceLocator locator, XPathContext xctxt)
	  {

		int n = getNode(source);

		if (org.apache.xml.dtm.DTM_Fields.NULL != n)
		{
		  return n;
		}

		n = parseToNode(source, locator, xctxt);

		if (org.apache.xml.dtm.DTM_Fields.NULL != n)
		{
		  putDocumentInCache(n, source);
		}

		return n;
	  }

	  /// <summary>
	  /// Try to create a DOM source tree from the input source.
	  /// </summary>
	  /// <param name="source"> The Source object that identifies the source node. </param>
	  /// <param name="locator"> The location of the caller, for diagnostic purposes.
	  /// </param>
	  /// <returns> non-null reference to node identified by the source argument.
	  /// </returns>
	  /// <exception cref="TransformerException"> if the source argument can not be resolved 
	  ///         to a source node. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int parseToNode(javax.xml.transform.Source source, javax.xml.transform.SourceLocator locator, XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public virtual int parseToNode(Source source, SourceLocator locator, XPathContext xctxt)
	  {

		try
		{
		  object xowner = xctxt.OwnerObject;
		  DTM dtm;
		  if (null != xowner && xowner is org.apache.xml.dtm.DTMWSFilter)
		  {
			dtm = xctxt.getDTM(source, false, (org.apache.xml.dtm.DTMWSFilter)xowner, false, true);
		  }
		  else
		  {
			dtm = xctxt.getDTM(source, false, null, false, true);
		  }
		  return dtm.Document;
		}
		catch (Exception e)
		{
		  //e.printStackTrace();
		  throw new TransformerException(e.Message, locator, e);
		}

	  }

	  /// <summary>
	  /// This method returns the SAX2 parser to use with the InputSource
	  /// obtained from this URI.
	  /// It may return null if any SAX2-conformant XML parser can be used,
	  /// or if getInputSource() will also return null. The parser must
	  /// be free for use (i.e.
	  /// not currently in use for another parse().
	  /// </summary>
	  /// <param name="inputSource"> The value returned from the URIResolver. </param>
	  /// <returns> a SAX2 XMLReader to use to resolve the inputSource argument. </returns>
	  /// <param name="locator"> The location of the original caller, for diagnostic purposes.
	  /// </param>
	  /// <exception cref="TransformerException"> if the reader can not be created. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.xml.sax.XMLReader getXMLReader(javax.xml.transform.Source inputSource, javax.xml.transform.SourceLocator locator) throws javax.xml.transform.TransformerException
	  public static XMLReader getXMLReader(Source inputSource, SourceLocator locator)
	  {

		try
		{
		  XMLReader reader = (inputSource is SAXSource) ? ((SAXSource) inputSource).XMLReader : null;

		  if (null == reader)
		  {
			try
			{
			  javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();
			  factory.NamespaceAware = true;
			  javax.xml.parsers.SAXParser jaxpParser = factory.newSAXParser();
			  reader = jaxpParser.XMLReader;

			}
			catch (javax.xml.parsers.ParserConfigurationException ex)
			{
			  throw new org.xml.sax.SAXException(ex);
			}
			catch (javax.xml.parsers.FactoryConfigurationError ex1)
			{
				throw new org.xml.sax.SAXException(ex1.ToString());
			}
			catch (System.MissingMethodException)
			{
			}
			catch (AbstractMethodError)
			{
			}
			if (null == reader)
			{
			  reader = XMLReaderFactory.createXMLReader();
			}
		  }

		  try
		  {
			reader.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
		  }
		  catch (org.xml.sax.SAXException)
		  {

			// What can we do?
			// TODO: User diagnostics.
		  }

		  return reader;
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se.Message, locator, se);
		}
	  }
	}

}