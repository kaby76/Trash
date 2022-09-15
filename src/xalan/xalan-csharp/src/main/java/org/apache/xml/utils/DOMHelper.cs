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
 * $Id: DOMHelper.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{


	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using Attr = org.w3c.dom.Attr;
	using DOMImplementation = org.w3c.dom.DOMImplementation;
	using Document = org.w3c.dom.Document;
	using DocumentType = org.w3c.dom.DocumentType;
	using Element = org.w3c.dom.Element;
	using Entity = org.w3c.dom.Entity;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using Text = org.w3c.dom.Text;

	/// @deprecated Since the introduction of the DTM, this class will be removed.
	/// This class provides a front-end to DOM implementations, providing
	/// a number of utility functions that either aren't yet standardized
	/// by the DOM spec or that are defined in optional DOM modules and
	/// hence may not be present in all DOMs. 
	public class DOMHelper
	{

	  /// <summary>
	  /// DOM Level 1 did not have a standard mechanism for creating a new
	  /// Document object. This function provides a DOM-implementation-independent
	  /// abstraction for that for that concept. It's typically used when 
	  /// outputting a new DOM as the result of an operation.
	  /// <para>
	  /// TODO: This isn't directly compatable with DOM Level 2. 
	  /// The Level 2 createDocument call also creates the root 
	  /// element, and thus requires that you know what that element will be
	  /// before creating the Document. We should think about whether we want
	  /// to change this code, and the callers, so we can use the DOM's own 
	  /// method. (It's also possible that DOM Level 3 may relax this
	  /// sequence, but you may give up some intelligence in the DOM by
	  /// doing so; the intent was that knowing the document type and root
	  /// element might let the DOM automatically switch to a specialized
	  /// subclass for particular kinds of documents.)
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="isSecureProcessing"> state of the secure processing feature. </param>
	  /// <returns> The newly created DOM Document object, with no children, or
	  /// null if we can't find a DOM implementation that permits creating
	  /// new empty Documents. </returns>
	  public static Document createDocument(bool isSecureProcessing)
	  {

		try
		{

		  // Use an implementation of the JAVA API for XML Parsing 1.0 to
		  // create a DOM Document node to contain the result.
		  DocumentBuilderFactory dfactory = DocumentBuilderFactory.newInstance();

		  dfactory.setNamespaceAware(true);
		  dfactory.setValidating(true);

		  if (isSecureProcessing)
		  {
			try
			{
			  dfactory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
			}
			catch (ParserConfigurationException)
			{
			}
		  }

		  DocumentBuilder docBuilder = dfactory.newDocumentBuilder();
		  Document outNode = docBuilder.newDocument();

		  return outNode;
		}
		catch (ParserConfigurationException)
		{
		  throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_CREATEDOCUMENT_NOT_SUPPORTED, null)); //"createDocument() not supported in XPathContext!");

		  // return null;
		}
	  }

	  /// <summary>
	  /// DOM Level 1 did not have a standard mechanism for creating a new
	  /// Document object. This function provides a DOM-implementation-independent
	  /// abstraction for that for that concept. It's typically used when 
	  /// outputting a new DOM as the result of an operation.
	  /// </summary>
	  /// <returns> The newly created DOM Document object, with no children, or
	  /// null if we can't find a DOM implementation that permits creating
	  /// new empty Documents. </returns>
	  public static Document createDocument()
	  {
		return createDocument(false);
	  }

	  /// <summary>
	  /// Tells, through the combination of the default-space attribute
	  /// on xsl:stylesheet, xsl:strip-space, xsl:preserve-space, and the
	  /// xml:space attribute, whether or not extra whitespace should be stripped
	  /// from the node.  Literal elements from template elements should
	  /// <em>not</em> be tested with this function. </summary>
	  /// <param name="textNode"> A text node from the source tree. </param>
	  /// <returns> true if the text node should be stripped of extra whitespace.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean shouldStripSourceNode(org.w3c.dom.Node textNode) throws javax.xml.transform.TransformerException
	  public virtual bool shouldStripSourceNode(Node textNode)
	  {

		// return (null == m_envSupport) ? false : m_envSupport.shouldStripSourceNode(textNode);
		return false;
	  }

	  /// <summary>
	  /// Supports the XPath function GenerateID by returning a unique
	  /// identifier string for any given DOM Node.
	  /// <para>
	  /// Warning: The base implementation uses the Node object's hashCode(),
	  /// which is NOT guaranteed to be unique. If that method hasn't been
	  /// overridden in this DOM ipmlementation, most Java implementions will
	  /// derive it from the object's address and should be OK... but if
	  /// your DOM uses a different definition of hashCode (eg hashing the
	  /// contents of the subtree), or if your DOM may have multiple objects
	  /// that represent a single Node in the data structure (eg via proxying),
	  /// you may need to find another way to assign a unique identifier.
	  /// </para>
	  /// <para>
	  /// Also, be aware that if nodes are destroyed and recreated, there is
	  /// an open issue regarding whether an ID may be reused. Currently
	  /// we're assuming that the input document is stable for the duration
	  /// of the XPath/XSLT operation, so this shouldn't arise in this context.
	  /// </para>
	  /// <para>
	  /// (DOM Level 3 is investigating providing a unique node "key", but
	  /// that won't help Level 1 and Level 2 implementations.)
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="node"> whose identifier you want to obtain
	  /// </param>
	  /// <returns> a string which should be different for every Node object. </returns>
	  public virtual string getUniqueID(Node node)
	  {
		return "N" + Convert.ToString(node.GetHashCode(), 16).ToUpper();
	  }

	  /// <summary>
	  /// Figure out whether node2 should be considered as being later
	  /// in the document than node1, in Document Order as defined
	  /// by the XPath model. This may not agree with the ordering defined
	  /// by other XML applications.
	  /// <para>
	  /// There are some cases where ordering isn't defined, and neither are
	  /// the results of this function -- though we'll generally return true.
	  /// 
	  /// TODO: Make sure this does the right thing with attribute nodes!!!
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="node1"> DOM Node to perform position comparison on. </param>
	  /// <param name="node2"> DOM Node to perform position comparison on .
	  /// </param>
	  /// <returns> false if node2 comes before node1, otherwise return true.
	  /// You can think of this as 
	  /// <code>(node1.documentOrderPosition &lt;= node2.documentOrderPosition)</code>. </returns>
	  public static bool isNodeAfter(Node node1, Node node2)
	  {
		if (node1 == node2 || isNodeTheSame(node1, node2))
		{
		  return true;
		}

			// Default return value, if there is no defined ordering
		bool isNodeAfter = true;

		Node parent1 = getParentOfNode(node1);
		Node parent2 = getParentOfNode(node2);

		// Optimize for most common case
		if (parent1 == parent2 || isNodeTheSame(parent1, parent2)) // then we know they are siblings
		{
		  if (null != parent1)
		  {
			isNodeAfter = isNodeAfterSibling(parent1, node1, node2);
		  }
		  else
		  {
					  // If both parents are null, ordering is not defined.
					  // We're returning a value in lieu of throwing an exception.
					  // Not a case we expect to arise in XPath, but beware if you
					  // try to reuse this method.

					  // We can just fall through in this case, which allows us
					  // to hit the debugging code at the end of the function.
			  //return isNodeAfter;
		  }
		}
		else
		{

		  // General strategy: Figure out the lengths of the two 
		  // ancestor chains, reconcile the lengths, and look for
			  // the lowest common ancestor. If that ancestor is one of
			  // the nodes being compared, it comes before the other.
		  // Otherwise perform a sibling compare. 
					//
					// NOTE: If no common ancestor is found, ordering is undefined
					// and we return the default value of isNodeAfter.

		  // Count parents in each ancestor chain
		  int nParents1 = 2, nParents2 = 2; // include node & parent obtained above

		  while (parent1 != null)
		  {
			nParents1++;

			parent1 = getParentOfNode(parent1);
		  }

		  while (parent2 != null)
		  {
			nParents2++;

			parent2 = getParentOfNode(parent2);
		  }

			  // Initially assume scan for common ancestor starts with
			  // the input nodes.
		  Node startNode1 = node1, startNode2 = node2;

		  // If one ancestor chain is longer, adjust its start point
			  // so we're comparing at the same depths
		  if (nParents1 < nParents2)
		  {
			// Adjust startNode2 to depth of startNode1
			int adjust = nParents2 - nParents1;

			for (int i = 0; i < adjust; i++)
			{
			  startNode2 = getParentOfNode(startNode2);
			}
		  }
		  else if (nParents1 > nParents2)
		  {
			// adjust startNode1 to depth of startNode2
			int adjust = nParents1 - nParents2;

			for (int i = 0; i < adjust; i++)
			{
			  startNode1 = getParentOfNode(startNode1);
			}
		  }

		  Node prevChild1 = null, prevChild2 = null; // so we can "back up"

		  // Loop up the ancestor chain looking for common parent
		  while (null != startNode1)
		  {
			if (startNode1 == startNode2 || isNodeTheSame(startNode1, startNode2)) // common parent?
			{
			  if (null == prevChild1) // first time in loop?
			  {

				// Edge condition: one is the ancestor of the other.
				isNodeAfter = (nParents1 < nParents2) ? true : false;

				break; // from while loop
			  }
			  else
			  {
							// Compare ancestors below lowest-common as siblings
				isNodeAfter = isNodeAfterSibling(startNode1, prevChild1, prevChild2);

				break; // from while loop
			  }
			} // end if(startNode1 == startNode2)

					// Move up one level and try again
			prevChild1 = startNode1;
			startNode1 = getParentOfNode(startNode1);
			prevChild2 = startNode2;
			startNode2 = getParentOfNode(startNode2);
		  } // end while(parents exist to examine)
		} // end big else (not immediate siblings)

			// WARNING: The following diagnostic won't report the early
			// "same node" case. Fix if/when needed.

		/* -- please do not remove... very useful for diagnostics --
		System.out.println("node1 = "+node1.getNodeName()+"("+node1.getNodeType()+")"+
		", node2 = "+node2.getNodeName()
		+"("+node2.getNodeType()+")"+
		", isNodeAfter = "+isNodeAfter); */
		return isNodeAfter;
	  } // end isNodeAfter(Node node1, Node node2)

	  /// <summary>
	  /// Use DTMNodeProxy to determine whether two nodes are the same.
	  /// </summary>
	  /// <param name="node1"> The first DOM node to compare. </param>
	  /// <param name="node2"> The second DOM node to compare. </param>
	  /// <returns> true if the two nodes are the same. </returns>
	  public static bool isNodeTheSame(Node node1, Node node2)
	  {
		if (node1 is DTMNodeProxy && node2 is DTMNodeProxy)
		{
		  return ((DTMNodeProxy)node1).Equals((DTMNodeProxy)node2);
		}
		else
		{
		  return (node1 == node2);
		}
	  }

	  /// <summary>
	  /// Figure out if child2 is after child1 in document order.
	  /// <para>
	  /// Warning: Some aspects of "document order" are not well defined.
	  /// For example, the order of attributes is considered
	  /// meaningless in XML, and the order reported by our model will
	  /// be consistant for a given invocation but may not 
	  /// match that of either the source file or the serialized output.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="parent"> Must be the parent of both child1 and child2. </param>
	  /// <param name="child1"> Must be the child of parent and not equal to child2. </param>
	  /// <param name="child2"> Must be the child of parent and not equal to child1. </param>
	  /// <returns> true if child 2 is after child1 in document order. </returns>
	  private static bool isNodeAfterSibling(Node parent, Node child1, Node child2)
	  {

		bool isNodeAfterSibling = false;
		short child1type = child1.getNodeType();
		short child2type = child2.getNodeType();

		if ((Node.ATTRIBUTE_NODE != child1type) && (Node.ATTRIBUTE_NODE == child2type))
		{

		  // always sort attributes before non-attributes.
		  isNodeAfterSibling = false;
		}
		else if ((Node.ATTRIBUTE_NODE == child1type) && (Node.ATTRIBUTE_NODE != child2type))
		{

		  // always sort attributes before non-attributes.
		  isNodeAfterSibling = true;
		}
		else if (Node.ATTRIBUTE_NODE == child1type)
		{
		  NamedNodeMap children = parent.getAttributes();
		  int nNodes = children.getLength();
		  bool found1 = false, found2 = false;

			  // Count from the start until we find one or the other.
		  for (int i = 0; i < nNodes; i++)
		  {
			Node child = children.item(i);

			if (child1 == child || isNodeTheSame(child1, child))
			{
			  if (found2)
			  {
				isNodeAfterSibling = false;

				break;
			  }

			  found1 = true;
			}
			else if (child2 == child || isNodeTheSame(child2, child))
			{
			  if (found1)
			  {
				isNodeAfterSibling = true;

				break;
			  }

			  found2 = true;
			}
		  }
		}
		else
		{
					// TODO: Check performance of alternate solution:
					// There are two choices here: Count from the start of
					// the document until we find one or the other, or count
					// from one until we find or fail to find the other.
					// Either can wind up scanning all the siblings in the worst
					// case, which on a wide document can be a lot of work but
					// is more typically is a short list. 
					// Scanning from the start involves two tests per iteration,
					// but it isn't clear that scanning from the middle doesn't
					// yield more iterations on average. 
					// We should run some testcases.
		  Node child = parent.getFirstChild();
		  bool found1 = false, found2 = false;

		  while (null != child)
		  {

			// Node child = children.item(i);
			if (child1 == child || isNodeTheSame(child1, child))
			{
			  if (found2)
			  {
				isNodeAfterSibling = false;

				break;
			  }

			  found1 = true;
			}
			else if (child2 == child || isNodeTheSame(child2, child))
			{
			  if (found1)
			  {
				isNodeAfterSibling = true;

				break;
			  }

			  found2 = true;
			}

			child = child.getNextSibling();
		  }
		}

		return isNodeAfterSibling;
	  } // end isNodeAfterSibling(Node parent, Node child1, Node child2)

	  //==========================================================
	  // SECTION: Namespace resolution
	  //==========================================================

	  /// <summary>
	  /// Get the depth level of this node in the tree (equals 1 for
	  /// a parentless node).
	  /// </summary>
	  /// <param name="n"> Node to be examined. </param>
	  /// <returns> the number of ancestors, plus one
	  /// @xsl.usage internal </returns>
	  public virtual short getLevel(Node n)
	  {

		short level = 1;

		while (null != (n = getParentOfNode(n)))
		{
		  level++;
		}

		return level;
	  }

	  /// <summary>
	  /// Given an XML Namespace prefix and a context in which the prefix
	  /// is to be evaluated, return the Namespace Name this prefix was 
	  /// bound to. Note that DOM Level 3 is expected to provide a version of
	  /// this which deals with the DOM's "early binding" behavior.
	  /// 
	  /// Default handling:
	  /// </summary>
	  /// <param name="prefix"> String containing namespace prefix to be resolved, 
	  /// without the ':' which separates it from the localname when used 
	  /// in a Node Name. The empty sting signifies the default namespace
	  /// at this point in the document. </param>
	  /// <param name="namespaceContext"> Element which provides context for resolution.
	  /// (We could extend this to work for other nodes by first seeking their
	  /// nearest Element ancestor.)
	  /// </param>
	  /// <returns> a String containing the Namespace URI which this prefix
	  /// represents in the specified context. </returns>
	  public virtual string getNamespaceForPrefix(string prefix, Element namespaceContext)
	  {

		int type;
		Node parent = namespaceContext;
		string @namespace = null;

		if (prefix.Equals("xml"))
		{
		  @namespace = QName.S_XMLNAMESPACEURI; // Hardcoded, per Namespace spec
		}
			else if (prefix.Equals("xmlns"))
			{
			  // Hardcoded in the DOM spec, expected to be adopted by
			  // Namespace spec. NOTE: Namespace declarations _must_ use
			  // the xmlns: prefix; other prefixes declared as belonging
			  // to this namespace will not be recognized and should
			  // probably be rejected by parsers as erroneous declarations.
		  @namespace = "http://www.w3.org/2000/xmlns/";
			}
		else
		{
			  // Attribute name for this prefix's declaration
			  string declname = (string.ReferenceEquals(prefix, "")) ? "xmlns" : "xmlns:" + prefix;

			  // Scan until we run out of Elements or have resolved the namespace
		  while ((null != parent) && (null == @namespace) && (((type = parent.getNodeType()) == Node.ELEMENT_NODE) || (type == Node.ENTITY_REFERENCE_NODE)))
		  {
			if (type == Node.ELEMENT_NODE)
			{

							// Look for the appropriate Namespace Declaration attribute,
							// either "xmlns:prefix" or (if prefix is "") "xmlns".
							// TODO: This does not handle "implicit declarations"
							// which may be created when the DOM is edited. DOM Level
							// 3 will define how those should be interpreted. But
							// this issue won't arise in freshly-parsed DOMs.

					// NOTE: declname is set earlier, outside the loop.
							Attr attr = ((Element)parent).getAttributeNode(declname);
							if (attr != null)
							{
					@namespace = attr.getNodeValue();
					break;
							}
			}

			parent = getParentOfNode(parent);
		  }
		}

		return @namespace;
	  }

	  /// <summary>
	  /// An experiment for the moment.
	  /// </summary>
	  internal Hashtable m_NSInfos = new Hashtable();

	  /// <summary>
	  /// Object to put into the m_NSInfos table that tells that a node has not been 
	  ///  processed, but has xmlns namespace decls.  
	  /// </summary>
	  protected internal static readonly NSInfo m_NSInfoUnProcWithXMLNS = new NSInfo(false, true);

	  /// <summary>
	  /// Object to put into the m_NSInfos table that tells that a node has not been 
	  ///  processed, but has no xmlns namespace decls.  
	  /// </summary>
	  protected internal static readonly NSInfo m_NSInfoUnProcWithoutXMLNS = new NSInfo(false, false);

	  /// <summary>
	  /// Object to put into the m_NSInfos table that tells that a node has not been 
	  ///  processed, and has no xmlns namespace decls, and has no ancestor decls.  
	  /// </summary>
	  protected internal static readonly NSInfo m_NSInfoUnProcNoAncestorXMLNS = new NSInfo(false, false, NSInfo.ANCESTORNOXMLNS);

	  /// <summary>
	  /// Object to put into the m_NSInfos table that tells that a node has been 
	  ///  processed, and has xmlns namespace decls.  
	  /// </summary>
	  protected internal static readonly NSInfo m_NSInfoNullWithXMLNS = new NSInfo(true, true);

	  /// <summary>
	  /// Object to put into the m_NSInfos table that tells that a node has been 
	  ///  processed, and has no xmlns namespace decls.  
	  /// </summary>
	  protected internal static readonly NSInfo m_NSInfoNullWithoutXMLNS = new NSInfo(true, false);

	  /// <summary>
	  /// Object to put into the m_NSInfos table that tells that a node has been 
	  ///  processed, and has no xmlns namespace decls. and has no ancestor decls.  
	  /// </summary>
	  protected internal static readonly NSInfo m_NSInfoNullNoAncestorXMLNS = new NSInfo(true, false, NSInfo.ANCESTORNOXMLNS);

	  /// <summary>
	  /// Vector of node (odd indexes) and NSInfos (even indexes) that tell if 
	  ///  the given node is a candidate for ancestor namespace processing.  
	  /// </summary>
	  protected internal ArrayList m_candidateNoAncestorXMLNS = new ArrayList();

	  /// <summary>
	  /// Returns the namespace of the given node. Differs from simply getting
	  /// the node's prefix and using getNamespaceForPrefix in that it attempts
	  /// to cache some of the data in NSINFO objects, to avoid repeated lookup.
	  /// TODO: Should we consider moving that logic into getNamespaceForPrefix?
	  /// </summary>
	  /// <param name="n"> Node to be examined.
	  /// </param>
	  /// <returns> String containing the Namespace Name (uri) for this node.
	  /// Note that this is undefined for any nodes other than Elements and
	  /// Attributes. </returns>
	  public virtual string getNamespaceOfNode(Node n)
	  {

		string namespaceOfPrefix;
		bool hasProcessedNS;
		NSInfo nsInfo;
		short ntype = n.getNodeType();

		if (Node.ATTRIBUTE_NODE != ntype)
		{
		  object nsObj = m_NSInfos[n]; // return value

		  nsInfo = (nsObj == null) ? null : (NSInfo) nsObj;
		  hasProcessedNS = (nsInfo == null) ? false : nsInfo.m_hasProcessedNS;
		}
		else
		{
		  hasProcessedNS = false;
		  nsInfo = null;
		}

		if (hasProcessedNS)
		{
		  namespaceOfPrefix = nsInfo.m_namespace;
		}
		else
		{
		  namespaceOfPrefix = null;

		  string nodeName = n.getNodeName();
		  int indexOfNSSep = nodeName.IndexOf(':');
		  string prefix;

		  if (Node.ATTRIBUTE_NODE == ntype)
		  {
			if (indexOfNSSep > 0)
			{
			  prefix = nodeName.Substring(0, indexOfNSSep);
			}
			else
			{

			  // Attributes don't use the default namespace, so if 
			  // there isn't a prefix, we're done.
			  return namespaceOfPrefix;
			}
		  }
		  else
		  {
			prefix = (indexOfNSSep >= 0) ? nodeName.Substring(0, indexOfNSSep) : "";
		  }

		  bool ancestorsHaveXMLNS = false;
		  bool nHasXMLNS = false;

		  if (prefix.Equals("xml"))
		  {
			namespaceOfPrefix = QName.S_XMLNAMESPACEURI;
		  }
		  else
		  {
			int parentType;
			Node parent = n;

			while ((null != parent) && (null == namespaceOfPrefix))
			{
			  if ((null != nsInfo) && (nsInfo.m_ancestorHasXMLNSAttrs == NSInfo.ANCESTORNOXMLNS))
			  {
				break;
			  }

			  parentType = parent.getNodeType();

			  if ((null == nsInfo) || nsInfo.m_hasXMLNSAttrs)
			  {
				bool elementHasXMLNS = false;

				if (parentType == Node.ELEMENT_NODE)
				{
				  NamedNodeMap nnm = parent.getAttributes();

				  for (int i = 0; i < nnm.getLength(); i++)
				  {
					Node attr = nnm.item(i);
					string aname = attr.getNodeName();

					if (aname[0] == 'x')
					{
					  bool isPrefix = aname.StartsWith("xmlns:", StringComparison.Ordinal);

					  if (aname.Equals("xmlns") || isPrefix)
					  {
						if (n == parent)
						{
						  nHasXMLNS = true;
						}

						elementHasXMLNS = true;
						ancestorsHaveXMLNS = true;

						string p = isPrefix ? aname.Substring(6) : "";

						if (p.Equals(prefix))
						{
						  namespaceOfPrefix = attr.getNodeValue();

						  break;
						}
					  }
					}
				  }
				}

				if ((Node.ATTRIBUTE_NODE != parentType) && (null == nsInfo) && (n != parent))
				{
				  nsInfo = elementHasXMLNS ? m_NSInfoUnProcWithXMLNS : m_NSInfoUnProcWithoutXMLNS;

				  m_NSInfos[parent] = nsInfo;
				}
			  }

			  if (Node.ATTRIBUTE_NODE == parentType)
			  {
				parent = getParentOfNode(parent);
			  }
			  else
			  {
				m_candidateNoAncestorXMLNS.Add(parent);
				m_candidateNoAncestorXMLNS.Add(nsInfo);

				parent = parent.getParentNode();
			  }

			  if (null != parent)
			  {
				object nsObj = m_NSInfos[parent]; // return value

				nsInfo = (nsObj == null) ? null : (NSInfo) nsObj;
			  }
			}

			int nCandidates = m_candidateNoAncestorXMLNS.Count;

			if (nCandidates > 0)
			{
			  if ((false == ancestorsHaveXMLNS) && (null == parent))
			  {
				for (int i = 0; i < nCandidates; i += 2)
				{
				  object candidateInfo = m_candidateNoAncestorXMLNS[i + 1];

				  if (candidateInfo == m_NSInfoUnProcWithoutXMLNS)
				  {
					m_NSInfos[m_candidateNoAncestorXMLNS[i]] = m_NSInfoUnProcNoAncestorXMLNS;
				  }
				  else if (candidateInfo == m_NSInfoNullWithoutXMLNS)
				  {
					m_NSInfos[m_candidateNoAncestorXMLNS[i]] = m_NSInfoNullNoAncestorXMLNS;
				  }
				}
			  }

			  m_candidateNoAncestorXMLNS.Clear();
			}
		  }

		  if (Node.ATTRIBUTE_NODE != ntype)
		  {
			if (null == namespaceOfPrefix)
			{
			  if (ancestorsHaveXMLNS)
			  {
				if (nHasXMLNS)
				{
				  m_NSInfos[n] = m_NSInfoNullWithXMLNS;
				}
				else
				{
				  m_NSInfos[n] = m_NSInfoNullWithoutXMLNS;
				}
			  }
			  else
			  {
				m_NSInfos[n] = m_NSInfoNullNoAncestorXMLNS;
			  }
			}
			else
			{
			  m_NSInfos[n] = new NSInfo(namespaceOfPrefix, nHasXMLNS);
			}
		  }
		}

		return namespaceOfPrefix;
	  }

	  /// <summary>
	  /// Returns the local name of the given node. If the node's name begins
	  /// with a namespace prefix, this is the part after the colon; otherwise
	  /// it's the full node name.
	  /// </summary>
	  /// <param name="n"> the node to be examined.
	  /// </param>
	  /// <returns> String containing the Local Name </returns>
	  public virtual string getLocalNameOfNode(Node n)
	  {

		string qname = n.getNodeName();
		int index = qname.IndexOf(':');

		return (index < 0) ? qname : qname.Substring(index + 1);
	  }

	  /// <summary>
	  /// Returns the element name with the namespace prefix (if any) replaced
	  /// by the Namespace URI it was bound to. This is not a standard 
	  /// representation of a node name, but it allows convenient 
	  /// single-string comparison of the "universal" names of two nodes.
	  /// </summary>
	  /// <param name="elem"> Element to be examined.
	  /// </param>
	  /// <returns> String in the form "namespaceURI:localname" if the node
	  /// belongs to a namespace, or simply "localname" if it doesn't. </returns>
	  /// <seealso cref=".getExpandedAttributeName"/>
	  public virtual string getExpandedElementName(Element elem)
	  {

		string @namespace = getNamespaceOfNode(elem);

		return (null != @namespace) ? @namespace + ":" + getLocalNameOfNode(elem) : getLocalNameOfNode(elem);
	  }

	  /// <summary>
	  /// Returns the attribute name with the namespace prefix (if any) replaced
	  /// by the Namespace URI it was bound to. This is not a standard 
	  /// representation of a node name, but it allows convenient 
	  /// single-string comparison of the "universal" names of two nodes.
	  /// </summary>
	  /// <param name="attr"> Attr to be examined
	  /// </param>
	  /// <returns> String in the form "namespaceURI:localname" if the node
	  /// belongs to a namespace, or simply "localname" if it doesn't. </returns>
	  /// <seealso cref=".getExpandedElementName"/>
	  public virtual string getExpandedAttributeName(Attr attr)
	  {

		string @namespace = getNamespaceOfNode(attr);

		return (null != @namespace) ? @namespace + ":" + getLocalNameOfNode(attr) : getLocalNameOfNode(attr);
	  }

	  //==========================================================
	  // SECTION: DOM Helper Functions
	  //==========================================================

	  /// <summary>
	  /// Tell if the node is ignorable whitespace. Note that this can
	  /// be determined only in the context of a DTD or other Schema,
	  /// and that DOM Level 2 has nostandardized DOM API which can
	  /// return that information.
	  /// @deprecated
	  /// </summary>
	  /// <param name="node"> Node to be examined
	  /// </param>
	  /// <returns> CURRENTLY HARDCODED TO FALSE, but should return true if
	  /// and only if the node is of type Text, contains only whitespace,
	  /// and does not appear as part of the #PCDATA content of an element.
	  /// (Note that determining this last may require allowing for 
	  /// Entity References.) </returns>
	  public virtual bool isIgnorableWhitespace(Text node)
	  {

		bool isIgnorable = false; // return value

		// TODO: I can probably do something to figure out if this 
		// space is ignorable from just the information in
		// the DOM tree.
			// -- You need to be able to distinguish whitespace
			// that is #PCDATA from whitespace that isn't.  That requires
			// DTD support, which won't be standardized until DOM Level 3.
		return isIgnorable;
	  }

	  /// <summary>
	  /// Get the first unparented node in the ancestor chain.
	  /// @deprecated
	  /// </summary>
	  /// <param name="node"> Starting node, to specify which chain to chase
	  /// </param>
	  /// <returns> the topmost ancestor. </returns>
	  public virtual Node getRoot(Node node)
	  {

		Node root = null;

		while (node != null)
		{
		  root = node;
		  node = getParentOfNode(node);
		}

		return root;
	  }

	  /// <summary>
	  /// Get the root node of the document tree, regardless of
	  /// whether or not the node passed in is a document node.
	  /// <para>
	  /// TODO: This doesn't handle DocumentFragments or "orphaned" subtrees
	  /// -- it's currently returning ownerDocument even when the tree is
	  /// not actually part of the main Document tree. We should either
	  /// rewrite the description to say that it finds the Document node,
	  /// or change the code to walk up the ancestor chain.
	  /// 
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="n"> Node to be examined
	  /// </param>
	  /// <returns> the Document node. Note that this is not the correct answer
	  /// if n was (or was a child of) a DocumentFragment or an orphaned node,
	  /// as can arise if the DOM has been edited rather than being generated
	  /// by a parser. </returns>
	  public virtual Node getRootNode(Node n)
	  {
		int nt = n.getNodeType();
		return ((Node.DOCUMENT_NODE == nt) || (Node.DOCUMENT_FRAGMENT_NODE == nt)) ? n : n.getOwnerDocument();
	  }

	  /// <summary>
	  /// Test whether the given node is a namespace decl node. In DOM Level 2
	  /// this can be done in a namespace-aware manner, but in Level 1 DOMs
	  /// it has to be done by testing the node name.
	  /// </summary>
	  /// <param name="n"> Node to be examined.
	  /// </param>
	  /// <returns> boolean -- true iff the node is an Attr whose name is 
	  /// "xmlns" or has the "xmlns:" prefix. </returns>
	  public virtual bool isNamespaceNode(Node n)
	  {

		if (Node.ATTRIBUTE_NODE == n.getNodeType())
		{
		  string attrName = n.getNodeName();

		  return (attrName.StartsWith("xmlns:", StringComparison.Ordinal) || attrName.Equals("xmlns"));
		}

		return false;
	  }

	  /// <summary>
	  /// Obtain the XPath-model parent of a DOM node -- ownerElement for Attrs,
	  /// parent for other nodes. 
	  /// <para>
	  /// Background: The DOM believes that you must be your Parent's
	  /// Child, and thus Attrs don't have parents. XPath said that Attrs
	  /// do have their owning Element as their parent. This function
	  /// bridges the difference, either by using the DOM Level 2 ownerElement
	  /// function or by using a "silly and expensive function" in Level 1
	  /// DOMs.
	  /// </para>
	  /// <para>
	  /// (There's some discussion of future DOMs generalizing ownerElement 
	  /// into ownerNode and making it work on all types of nodes. This
	  /// still wouldn't help the users of Level 1 or Level 2 DOMs)
	  /// </para>
	  /// <para>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="node"> Node whose XPath parent we want to obtain
	  /// </param>
	  /// <returns> the parent of the node, or the ownerElement if it's an
	  /// Attr node, or null if the node is an orphan.
	  /// </returns>
	  /// <exception cref="RuntimeException"> if the Document has no root element.
	  /// This can't arise if the Document was created
	  /// via the DOM Level 2 factory methods, but is possible if other
	  /// mechanisms were used to obtain it </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.w3c.dom.Node getParentOfNode(org.w3c.dom.Node node) throws RuntimeException
	  public static Node getParentOfNode(Node node)
	  {
		Node parent;
		short nodeType = node.getNodeType();

		if (Node.ATTRIBUTE_NODE == nodeType)
		{
		  Document doc = node.getOwnerDocument();
			  /*
		  TBD:
		  if(null == doc)
		  {
			throw new RuntimeException(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_CHILD_HAS_NO_OWNER_DOCUMENT, null));//"Attribute child does not have an owner document!");
		  }
		  */

			  // Given how expensive the tree walk may be, we should first ask 
			  // whether this DOM can answer the question for us. The additional
			  // test does slow down Level 1 DOMs slightly. DOMHelper2, which
			  // is currently specialized for Xerces, assumes it can use the
			  // Level 2 solution. We might want to have an intermediate stage,
			  // which would assume DOM Level 2 but not assume Xerces.
			  //
			  // (Shouldn't have to check whether impl is null in a compliant DOM,
			  // but let's be paranoid for a moment...)
			  DOMImplementation impl = doc.getImplementation();
			  if (impl != null && impl.hasFeature("Core","2.0"))
			  {
					  parent = ((Attr)node).getOwnerElement();
					  return parent;
			  }

			  // DOM Level 1 solution, as fallback. Hugely expensive. 

		  Element rootElem = doc.getDocumentElement();

		  if (null == rootElem)
		  {
			throw new Exception(XMLMessages.createXMLMessage(XMLErrorResources.ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, null)); //"Attribute child does not have an owner document element!");
		  }

		  parent = locateAttrParent(rootElem, node);

		}
		else
		{
		  parent = node.getParentNode();

		  // if((Node.DOCUMENT_NODE != nodeType) && (null == parent))
		  // {
		  //   throw new RuntimeException("Child does not have parent!");
		  // }
		}

		return parent;
	  }

	  /// <summary>
	  /// Given an ID, return the element. This can work only if the document
	  /// is interpreted in the context of a DTD or Schema, since otherwise
	  /// we don't know which attributes are or aren't IDs.
	  /// <para>
	  /// Note that DOM Level 1 had no ability to retrieve this information.
	  /// DOM Level 2 introduced it but does not promise that it will be
	  /// supported in all DOMs; those which can't support it will always
	  /// return null.
	  /// </para>
	  /// <para>
	  /// TODO: getElementByID is currently unimplemented. Support DOM Level 2?
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="id"> The unique identifier to be searched for. </param>
	  /// <param name="doc"> The document to search within. </param>
	  /// <returns> CURRENTLY HARDCODED TO NULL, but it should be:
	  /// The node which has this unique identifier, or null if there
	  /// is no such node or this DOM can't reliably recognize it. </returns>
	  public virtual Element getElementByID(string id, Document doc)
	  {
		return null;
	  }

	  /// <summary>
	  /// The getUnparsedEntityURI function returns the URI of the unparsed
	  /// entity with the specified name in the same document as the context
	  /// node (see [3.3 Unparsed Entities]). It returns the empty string if
	  /// there is no such entity.
	  /// <para>
	  /// XML processors may choose to use the System Identifier (if one
	  /// is provided) to resolve the entity, rather than the URI in the
	  /// Public Identifier. The details are dependent on the processor, and
	  /// we would have to support some form of plug-in resolver to handle
	  /// this properly. Currently, we simply return the System Identifier if
	  /// present, and hope that it a usable URI or that our caller can
	  /// map it to one.
	  /// TODO: Resolve Public Identifiers... or consider changing function name.
	  /// </para>
	  /// <para>
	  /// If we find a relative URI 
	  /// reference, XML expects it to be resolved in terms of the base URI 
	  /// of the document. The DOM doesn't do that for us, and it isn't 
	  /// entirely clear whether that should be done here; currently that's
	  /// pushed up to a higher levelof our application. (Note that DOM Level 
	  /// 1 didn't store the document's base URI.)
	  /// TODO: Consider resolving Relative URIs.
	  /// </para>
	  /// <para>
	  /// (The DOM's statement that "An XML processor may choose to
	  /// completely expand entities before the structure model is passed
	  /// to the DOM" refers only to parsed entities, not unparsed, and hence
	  /// doesn't affect this function.)
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="name"> A string containing the Entity Name of the unparsed
	  /// entity. </param>
	  /// <param name="doc"> Document node for the document to be searched.
	  /// </param>
	  /// <returns> String containing the URI of the Unparsed Entity, or an
	  /// empty string if no such entity exists. </returns>
	  public virtual string getUnparsedEntityURI(string name, Document doc)
	  {

		string url = "";
		DocumentType doctype = doc.getDoctype();

		if (null != doctype)
		{
		  NamedNodeMap entities = doctype.getEntities();
		  if (null == entities)
		  {
			return url;
		  }
		  Entity entity = (Entity) entities.getNamedItem(name);
		  if (null == entity)
		  {
			return url;
		  }

		  string notationName = entity.getNotationName();

		  if (null != notationName) // then it's unparsed
		  {
			// The draft says: "The XSLT processor may use the public 
			// identifier to generate a URI for the entity instead of the URI 
			// specified in the system identifier. If the XSLT processor does 
			// not use the public identifier to generate the URI, it must use 
			// the system identifier; if the system identifier is a relative 
			// URI, it must be resolved into an absolute URI using the URI of 
			// the resource containing the entity declaration as the base 
			// URI [RFC2396]."
			// So I'm falling a bit short here.
			url = entity.getSystemId();

			if (null == url)
			{
			  url = entity.getPublicId();
			}
			else
			{
			  // This should be resolved to an absolute URL, but that's hard 
			  // to do from here.
			}
		  }
		}

		return url;
	  }

	  /// <summary>
	  /// Support for getParentOfNode; walks a DOM tree until it finds
	  /// the Element which owns the Attr. This is hugely expensive, and
	  /// if at all possible you should use the DOM Level 2 Attr.ownerElement()
	  /// method instead.
	  ///  <para>
	  /// The DOM Level 1 developers expected that folks would keep track
	  /// of the last Element they'd seen and could recover the info from
	  /// that source. Obviously that doesn't work very well if the only
	  /// information you've been presented with is the Attr. The DOM Level 2
	  /// getOwnerElement() method fixes that, but only for Level 2 and
	  /// later DOMs.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="elem"> Element whose subtree is to be searched for this Attr </param>
	  /// <param name="attr"> Attr whose owner is to be located.
	  /// </param>
	  /// <returns> the first Element whose attribute list includes the provided
	  /// attr. In modern DOMs, this will also be the only such Element. (Early
	  /// DOMs had some hope that Attrs might be sharable, but this idea has
	  /// been abandoned.) </returns>
	  private static Node locateAttrParent(Element elem, Node attr)
	  {

		Node parent = null;

			// This should only be called for Level 1 DOMs, so we don't have to
			// worry about namespace issues. In later levels, it's possible
			// for a DOM to have two Attrs with the same NodeName but
			// different namespaces, and we'd need to get getAttributeNodeNS...
			// but later levels also have Attr.getOwnerElement.
			Attr check = elem.getAttributeNode(attr.getNodeName());
			if (check == attr)
			{
					parent = elem;
			}

		if (null == parent)
		{
		  for (Node node = elem.getFirstChild(); null != node; node = node.getNextSibling())
		  {
			if (Node.ELEMENT_NODE == node.getNodeType())
			{
			  parent = locateAttrParent((Element) node, attr);

			  if (null != parent)
			  {
				break;
			  }
			}
		  }
		}

		return parent;
	  }

	  /// <summary>
	  /// The factory object used for creating nodes
	  /// in the result tree.
	  /// </summary>
	  protected internal Document m_DOMFactory = null;

	  /// <summary>
	  /// Store the factory object required to create DOM nodes
	  /// in the result tree. In fact, that's just the result tree's
	  /// Document node...
	  /// </summary>
	  /// <param name="domFactory"> The DOM Document Node within whose context
	  /// the result tree will be built. </param>
	  public virtual Document DOMFactory
	  {
		  set
		  {
			this.m_DOMFactory = value;
		  }
		  get
		  {
    
			if (null == this.m_DOMFactory)
			{
			  this.m_DOMFactory = createDocument();
			}
    
			return this.m_DOMFactory;
		  }
	  }


	  /// <summary>
	  /// Get the textual contents of the node. See
	  /// getNodeData(Node,FastStringBuffer) for discussion of how
	  /// whitespace nodes are handled.
	  /// </summary>
	  /// <param name="node"> DOM Node to be examined </param>
	  /// <returns> String containing a concatenation of all the 
	  /// textual content within that node. </returns>
	  /// <seealso cref=".getNodeData(Node,FastStringBuffer)"
	  /// />
	  public static string getNodeData(Node node)
	  {

		FastStringBuffer buf = StringBufferPool.get();
		string s;

		try
		{
		  getNodeData(node, buf);

		  s = (buf.length() > 0) ? buf.ToString() : "";
		}
		finally
		{
		  StringBufferPool.free(buf);
		}

		return s;
	  }

	  /// <summary>
	  /// Retrieve the text content of a DOM subtree, appending it into a
	  /// user-supplied FastStringBuffer object. Note that attributes are
	  /// not considered part of the content of an element.
	  /// <para>
	  /// There are open questions regarding whitespace stripping. 
	  /// Currently we make no special effort in that regard, since the standard
	  /// DOM doesn't yet provide DTD-based information to distinguish
	  /// whitespace-in-element-context from genuine #PCDATA. Note that we
	  /// should probably also consider xml:space if/when we address this.
	  /// DOM Level 3 may solve the problem for us.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="node"> Node whose subtree is to be walked, gathering the
	  /// contents of all Text or CDATASection nodes. </param>
	  /// <param name="buf"> FastStringBuffer into which the contents of the text
	  /// nodes are to be concatenated. </param>
	  public static void getNodeData(Node node, FastStringBuffer buf)
	  {

		switch (node.getNodeType())
		{
		case Node.DOCUMENT_FRAGMENT_NODE :
		case Node.DOCUMENT_NODE :
		case Node.ELEMENT_NODE :
		{
		  for (Node child = node.getFirstChild(); null != child; child = child.getNextSibling())
		  {
			getNodeData(child, buf);
		  }
		}
		break;
		case Node.TEXT_NODE :
		case Node.CDATA_SECTION_NODE :
		  buf.append(node.getNodeValue());
		  break;
		case Node.ATTRIBUTE_NODE :
		  buf.append(node.getNodeValue());
		  break;
		case Node.PROCESSING_INSTRUCTION_NODE :
		  // warning(XPATHErrorResources.WG_PARSING_AND_PREPARING);        
		  break;
		default :
		  // ignore
		  break;
		}
	  }
	}

}