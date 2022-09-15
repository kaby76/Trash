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
 * $Id: DOM2DTM.java 478671 2006-11-23 21:00:31Z minchau $
 */
namespace org.apache.xml.dtm.@ref.dom2dtm
{


	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using QName = org.apache.xml.utils.QName;
	using StringBufferPool = org.apache.xml.utils.StringBufferPool;
	using TreeWalker = org.apache.xml.utils.TreeWalker;
	using XMLCharacterRecognizer = org.apache.xml.utils.XMLCharacterRecognizer;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;
	using Attr = org.w3c.dom.Attr;
	using Document = org.w3c.dom.Document;
	using DocumentType = org.w3c.dom.DocumentType;
	using Element = org.w3c.dom.Element;
	using Entity = org.w3c.dom.Entity;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// The <code>DOM2DTM</code> class serves up a DOM's contents via the
	/// DTM API.
	/// 
	/// Note that it doesn't necessarily represent a full Document
	/// tree. You can wrap a DOM2DTM around a specific node and its subtree
	/// and the right things should happen. (I don't _think_ we currently
	/// support DocumentFrgment nodes as roots, though that might be worth
	/// considering.)
	/// 
	/// Note too that we do not currently attempt to track document
	/// mutation. If you alter the DOM after wrapping DOM2DTM around it,
	/// all bets are off.
	/// 
	/// </summary>
	public class DOM2DTM : DTMDefaultBaseIterators
	{
	  internal new const bool JJK_DEBUG = false;
	  internal const bool JJK_NEWCODE = true;

	  /// <summary>
	  /// Manefest constant
	  /// </summary>
	  internal const string NAMESPACE_DECL_NS = "http://www.w3.org/XML/1998/namespace";

	  /// <summary>
	  /// The current position in the DOM tree. Last node examined for
	  /// possible copying to DTM. 
	  /// </summary>
	  [NonSerialized]
	  private Node m_pos;
	  /// <summary>
	  /// The current position in the DTM tree. Who children get appended to. </summary>
	  private int m_last_parent = 0;
	  /// <summary>
	  /// The current position in the DTM tree. Who children reference as their 
	  /// previous sib. 
	  /// </summary>
	  private int m_last_kid = org.apache.xml.dtm.DTM_Fields.NULL;

	  /// <summary>
	  /// The top of the subtree.
	  /// %REVIEW%: 'may not be the same as m_context if "//foo" pattern.'
	  /// 
	  /// </summary>
	  [NonSerialized]
	  private Node m_root;

	  /// <summary>
	  /// True iff the first element has been processed. This is used to control
	  ///    synthesis of the implied xml: namespace declaration node. 
	  /// </summary>
	  internal bool m_processedFirstElement = false;

	  /// <summary>
	  /// true if ALL the nodes in the m_root subtree have been processed;
	  /// false if our incremental build has not yet finished scanning the
	  /// DOM tree.  
	  /// </summary>
	  [NonSerialized]
	  private bool m_nodesAreProcessed;

	  /// <summary>
	  /// The node objects.  The instance part of the handle indexes
	  /// directly into this vector.  Each DTM node may actually be
	  /// composed of several DOM nodes (for example, if logically-adjacent
	  /// Text/CDATASection nodes in the DOM have been coalesced into a
	  /// single DTM Text node); this table points only to the first in
	  /// that sequence. 
	  /// </summary>
	  protected internal ArrayList m_nodes = new ArrayList();

	  /// <summary>
	  /// Construct a DOM2DTM object from a DOM node.
	  /// </summary>
	  /// <param name="mgr"> The DTMManager who owns this DTM. </param>
	  /// <param name="domSource"> the DOM source that this DTM will wrap. </param>
	  /// <param name="dtmIdentity"> The DTM identity ID for this DTM. </param>
	  /// <param name="whiteSpaceFilter"> The white space filter for this DTM, which may 
	  ///                         be null. </param>
	  /// <param name="xstringfactory"> XMLString factory for creating character content. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use 
	  ///                   indexing schemes. </param>
	  public DOM2DTM(DTMManager mgr, DOMSource domSource, int dtmIdentity, DTMWSFilter whiteSpaceFilter, XMLStringFactory xstringfactory, bool doIndexing) : base(mgr, domSource, dtmIdentity, whiteSpaceFilter, xstringfactory, doIndexing)
	  {

		// Initialize DOM navigation
		m_pos = m_root = domSource.Node;
		// Initialize DTM navigation
		m_last_parent = m_last_kid = org.apache.xml.dtm.DTM_Fields.NULL;
		m_last_kid = addNode(m_root, m_last_parent,m_last_kid, org.apache.xml.dtm.DTM_Fields.NULL);

		// Apparently the domSource root may not actually be the
		// Document node. If it's an Element node, we need to immediately
		// add its attributes. Adapted from nextNode().
		// %REVIEW% Move this logic into addNode and recurse? Cleaner!
		//
		// (If it's an EntityReference node, we're probably in 
		// seriously bad trouble. For now
		// I'm just hoping nobody is ever quite that foolish... %REVIEW%)
			//
			// %ISSUE% What about inherited namespaces in this case?
			// Do we need to special-case initialize them into the DTM model?
		if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == m_root.NodeType)
		{
		  NamedNodeMap attrs = m_root.Attributes;
		  int attrsize = (attrs == null) ? 0 : attrs.Length;
		  if (attrsize > 0)
		  {
			int attrIndex = org.apache.xml.dtm.DTM_Fields.NULL; // start with no previous sib
			for (int i = 0;i < attrsize;++i)
			{
			  // No need to force nodetype in this case;
			  // addNode() will take care of switching it from
			  // Attr to Namespace if necessary.
			  attrIndex = addNode(attrs.item(i),0,attrIndex,org.apache.xml.dtm.DTM_Fields.NULL);
			  m_firstch.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,attrIndex);
			}
			// Terminate list of attrs, and make sure they aren't
			// considered children of the element
			m_nextsib.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,attrIndex);

			// IMPORTANT: This does NOT change m_last_parent or m_last_kid!
		  } // if attrs exist
		} //if(ELEMENT_NODE)

		// Initialize DTM-completed status 
		m_nodesAreProcessed = false;
	  }

	  /// <summary>
	  /// Construct the node map from the node.
	  /// </summary>
	  /// <param name="node"> The node that is to be added to the DTM. </param>
	  /// <param name="parentIndex"> The current parent index. </param>
	  /// <param name="previousSibling"> The previous sibling index. </param>
	  /// <param name="forceNodeType"> If not DTM.NULL, overrides the DOM node type.
	  /// Used to force nodes to Text rather than CDATASection when their
	  /// coalesced value includes ordinary Text nodes (current DTM behavior).
	  /// </param>
	  /// <returns> The index identity of the node that was added. </returns>
	  protected internal virtual int addNode(Node node, int parentIndex, int previousSibling, int forceNodeType)
	  {
		int nodeIndex = m_nodes.Count;

		// Have we overflowed a DTM Identity's addressing range?
		if (m_dtmIdent.size() == ((int)((uint)nodeIndex >> DTMManager.IDENT_DTM_NODE_BITS)))
		{
		  try
		  {
			if (m_mgr == null)
			{
			  throw new System.InvalidCastException();
			}

									// Handle as Extended Addressing
			DTMManagerDefault mgrD = (DTMManagerDefault)m_mgr;
			int id = mgrD.FirstFreeDTMID;
			mgrD.addDTM(this,id,nodeIndex);
			m_dtmIdent.addElement(id << DTMManager.IDENT_DTM_NODE_BITS);
		  }
		  catch (System.InvalidCastException)
		  {
			// %REVIEW% Wrong error message, but I've been told we're trying
			// not to add messages right not for I18N reasons.
			// %REVIEW% Should this be a Fatal Error?
			error(XMLMessages.createXMLMessage(XMLErrorResources.ER_NO_DTMIDS_AVAIL, null)); //"No more DTM IDs are available";
		  }
		}

		m_size++;
		// ensureSize(nodeIndex);

		int type;
		if (org.apache.xml.dtm.DTM_Fields.NULL == forceNodeType)
		{
			type = node.NodeType;
		}
		else
		{
			type = forceNodeType;
		}

		// %REVIEW% The Namespace Spec currently says that Namespaces are
		// processed in a non-namespace-aware manner, by matching the
		// QName, even though there is in fact a namespace assigned to
		// these nodes in the DOM. If and when that changes, we will have
		// to consider whether we check the namespace-for-namespaces
		// rather than the node name.
		//
		// %TBD% Note that the DOM does not necessarily explicitly declare
		// all the namespaces it uses. DOM Level 3 will introduce a
		// namespace-normalization operation which reconciles that, and we
		// can request that users invoke it or otherwise ensure that the
		// tree is namespace-well-formed before passing the DOM to Xalan.
		// But if they don't, what should we do about it? We probably
		// don't want to alter the source DOM (and may not be able to do
		// so if it's read-only). The best available answer might be to
		// synthesize additional DTM Namespace Nodes that don't correspond
		// to DOM Attr Nodes.
		if (Node.ATTRIBUTE_NODE == type)
		{
		  string name = node.NodeName;

		  if (name.StartsWith("xmlns:", StringComparison.Ordinal) || name.Equals("xmlns"))
		  {
			type = org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE;
		  }
		}

		m_nodes.Add(node);

		m_firstch.setElementAt(NOTPROCESSED,nodeIndex);
		m_nextsib.setElementAt(NOTPROCESSED,nodeIndex);
		m_prevsib.setElementAt(previousSibling,nodeIndex);
		m_parent.setElementAt(parentIndex,nodeIndex);

		if (org.apache.xml.dtm.DTM_Fields.NULL != parentIndex && type != org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE && type != org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE)
		{
		  // If the DTM parent had no children, this becomes its first child.
		  if (NOTPROCESSED == m_firstch.elementAt(parentIndex))
		  {
			m_firstch.setElementAt(nodeIndex,parentIndex);
		  }
		}

		string nsURI = node.NamespaceURI;

		// Deal with the difference between Namespace spec and XSLT
		// definitions of local name. (The former says PIs don't have
		// localnames; the latter says they do.)
		string localName = (type == Node.PROCESSING_INSTRUCTION_NODE) ? node.NodeName : node.LocalName;

		// Hack to make DOM1 sort of work...
		if (((type == Node.ELEMENT_NODE) || (type == Node.ATTRIBUTE_NODE)) && null == localName)
		{
		  localName = node.NodeName; // -sb
		}

		ExpandedNameTable exnt = m_expandedNameTable;

		// %TBD% Nodes created with the old non-namespace-aware DOM
		// calls createElement() and createAttribute() will never have a
		// localname. That will cause their expandedNameID to be just the
		// nodeType... which will keep them from being matched
		// successfully by name. Since the DOM makes no promise that
		// those will participate in namespace processing, this is
		// officially accepted as Not Our Fault. But it might be nice to
		// issue a diagnostic message!
		if (node.LocalName == null && (type == Node.ELEMENT_NODE || type == Node.ATTRIBUTE_NODE))
		{
			// warning("DOM 'level 1' node "+node.getNodeName()+" won't be mapped properly in DOM2DTM.");
		}

		int expandedNameID = (null != localName) ? exnt.getExpandedTypeID(nsURI, localName, type) : exnt.getExpandedTypeID(type);

		m_exptype.setElementAt(expandedNameID,nodeIndex);

		indexNode(expandedNameID, nodeIndex);

		if (org.apache.xml.dtm.DTM_Fields.NULL != previousSibling)
		{
		  m_nextsib.setElementAt(nodeIndex,previousSibling);
		}

		// This should be done after m_exptype has been set, and probably should
		// always be the last thing we do
		if (type == org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE)
		{
			declareNamespaceInContext(parentIndex,nodeIndex);
		}

		return nodeIndex;
	  }

	  /// <summary>
	  /// Get the number of nodes that have been added.
	  /// </summary>
	  public override int NumberOfNodes
	  {
		  get
		  {
			return m_nodes.Count;
		  }
	  }

	 /// <summary>
	 /// This method iterates to the next node that will be added to the table.
	 /// Each call to this method adds a new node to the table, unless the end
	 /// is reached, in which case it returns null.
	 /// </summary>
	 /// <returns> The true if a next node is found or false if 
	 ///         there are no more nodes. </returns>
	  protected internal override bool nextNode()
	  {
		// Non-recursive one-fetch-at-a-time depth-first traversal with 
		// attribute/namespace nodes and white-space stripping.
		// Navigating the DOM is simple, navigating the DTM is simple;
		// keeping track of both at once is a trifle baroque but at least
		// we've avoided most of the special cases.
		if (m_nodesAreProcessed)
		{
		  return false;
		}

		// %REVIEW% Is this local copy Really Useful from a performance
		// point of view?  Or is this a false microoptimization?
		Node pos = m_pos;
		Node next = null;
		int nexttype = org.apache.xml.dtm.DTM_Fields.NULL;

		// Navigate DOM tree
		do
		{
			// Look down to first child.
			if (pos.hasChildNodes())
			{
				next = pos.FirstChild;

				// %REVIEW% There's probably a more elegant way to skip
				// the doctype. (Just let it go and Suppress it?
				if (next != null && org.apache.xml.dtm.DTM_Fields.DOCUMENT_TYPE_NODE == next.NodeType)
				{
				  next = next.NextSibling;
				}

				// Push DTM context -- except for children of Entity References, 
				// which have no DTM equivalent and cause no DTM navigation.
				if (org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE != pos.NodeType)
				{
					m_last_parent = m_last_kid;
					m_last_kid = org.apache.xml.dtm.DTM_Fields.NULL;
					// Whitespace-handler context stacking
					if (null != m_wsfilter)
					{
					  short wsv = m_wsfilter.getShouldStripSpace(makeNodeHandle(m_last_parent),this);
					  bool shouldStrip = (org.apache.xml.dtm.DTMWSFilter_Fields.INHERIT == wsv) ? ShouldStripWhitespace : (org.apache.xml.dtm.DTMWSFilter_Fields.STRIP == wsv);
					  pushShouldStripWhitespace(shouldStrip);
					} // if(m_wsfilter)
				}
			}

			// If that fails, look up and right (but not past root!)
			else
			{
				if (m_last_kid != org.apache.xml.dtm.DTM_Fields.NULL)
				{
					// Last node posted at this level had no more children
					// If it has _no_ children, we need to record that.
					if (m_firstch.elementAt(m_last_kid) == NOTPROCESSED)
					{
					  m_firstch.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,m_last_kid);
					}
				}

				while (m_last_parent != org.apache.xml.dtm.DTM_Fields.NULL)
				{
					// %REVIEW% There's probably a more elegant way to
					// skip the doctype. (Just let it go and Suppress it?
					next = pos.NextSibling;
					if (next != null && org.apache.xml.dtm.DTM_Fields.DOCUMENT_TYPE_NODE == next.NodeType)
					{
					  next = next.NextSibling;
					}

					if (next != null)
					{
					  break; // Found it!
					}

					// No next-sibling found. Pop the DOM.
					pos = pos.ParentNode;
					if (pos == null)
					{
						// %TBD% Should never arise, but I want to be sure of that...
						if (JJK_DEBUG)
						{
							Console.WriteLine("***** DOM2DTM Pop Control Flow problem");
							for (;;)
							{
								; // Freeze right here!
							}
						}
					}

					// The only parents in the DTM are Elements.  However,
					// the DOM could contain EntityReferences.  If we
					// encounter one, pop it _without_ popping DTM.
					if (pos != null && org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE == pos.NodeType)
					{
						// Nothing needs doing
						if (JJK_DEBUG)
						{
						  Console.WriteLine("***** DOM2DTM popping EntRef");
						}
					}
					else
					{
						popShouldStripWhitespace();
						// Fix and pop DTM
						if (m_last_kid == org.apache.xml.dtm.DTM_Fields.NULL)
						{
						  m_firstch.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,m_last_parent); // Popping from an element
						}
						else
						{
						  m_nextsib.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,m_last_kid); // Popping from anything else
						}
						m_last_parent = m_parent.elementAt(m_last_kid = m_last_parent);
					}
				}
				if (m_last_parent == org.apache.xml.dtm.DTM_Fields.NULL)
				{
				  next = null;
				}
			}

			if (next != null)
			{
			  nexttype = next.NodeType;
			}

			// If it's an entity ref, advance past it.
			//
			// %REVIEW% Should we let this out the door and just suppress it?
			// More work, but simpler code, more likely to be correct, and
			// it doesn't happen very often. We'd get rid of the loop too.
			if (org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE == nexttype)
			{
			  pos = next;
			}
		} while (org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE == nexttype);

		// Did we run out of the tree?
		if (next == null)
		{
			m_nextsib.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,0);
			m_nodesAreProcessed = true;
			m_pos = null;

			if (JJK_DEBUG)
			{
				Console.WriteLine("***** DOM2DTM Crosscheck:");
				for (int i = 0;i < m_nodes.Count;++i)
				{
				  Console.WriteLine(i + ":\t" + m_firstch.elementAt(i) + "\t" + m_nextsib.elementAt(i));
				}
			}

			return false;
		}

		// Text needs some special handling:
		//
		// DTM may skip whitespace. This is handled by the suppressNode flag, which
		// when true will keep the DTM node from being created.
		//
		// DTM only directly records the first DOM node of any logically-contiguous
		// sequence. The lastTextNode value will be set to the last node in the 
		// contiguous sequence, and -- AFTER the DTM addNode -- can be used to 
		// advance next over this whole block. Should be simpler than special-casing
		// the above loop for "Was the logically-preceeding sibling a text node".
		// 
		// Finally, a DTM node should be considered a CDATASection only if all the
		// contiguous text it covers is CDATASections. The first Text should
		// force DTM to Text.

		bool suppressNode = false;
		Node lastTextNode = null;

		nexttype = next.NodeType;

		// nexttype=pos.getNodeType();
		if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE == nexttype || org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE == nexttype)
		{
			// If filtering, initially assume we're going to suppress the node
			suppressNode = ((null != m_wsfilter) && ShouldStripWhitespace);

			// Scan logically contiguous text (siblings, plus "flattening"
			// of entity reference boundaries).
			Node n = next;
			while (n != null)
			{
				lastTextNode = n;
				// Any Text node means DTM considers it all Text
				if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE == n.NodeType)
				{
				  nexttype = org.apache.xml.dtm.DTM_Fields.TEXT_NODE;
				}
				// Any non-whitespace in this sequence blocks whitespace
				// suppression
				suppressNode &= XMLCharacterRecognizer.isWhiteSpace(n.NodeValue);

				n = logicalNextDOMTextNode(n);
			}
		}

		// Special handling for PIs: Some DOMs represent the XML
		// Declaration as a PI. This is officially incorrect, per the DOM
		// spec, but is considered a "wrong but tolerable" temporary
		// workaround pending proper handling of these fields in DOM Level
		// 3. We want to recognize and reject that case.
		else if (org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE == nexttype)
		{
			suppressNode = (pos.NodeName.ToLower().Equals("xml"));
		}


		if (!suppressNode)
		{
			// Inserting next. NOTE that we force the node type; for
			// coalesced Text, this records CDATASections adjacent to
			// ordinary Text as Text.
			int nextindex = addNode(next,m_last_parent,m_last_kid, nexttype);

			m_last_kid = nextindex;

			if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == nexttype)
			{
				int attrIndex = org.apache.xml.dtm.DTM_Fields.NULL; // start with no previous sib
				// Process attributes _now_, rather than waiting.
				// Simpler control flow, makes NS cache available immediately.
				NamedNodeMap attrs = next.Attributes;
				int attrsize = (attrs == null) ? 0 : attrs.Length;
				if (attrsize > 0)
				{
					for (int i = 0;i < attrsize;++i)
					{
						// No need to force nodetype in this case;
						// addNode() will take care of switching it from
						// Attr to Namespace if necessary.
						attrIndex = addNode(attrs.item(i), nextindex,attrIndex,org.apache.xml.dtm.DTM_Fields.NULL);
						m_firstch.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,attrIndex);

						// If the xml: prefix is explicitly declared
						// we don't need to synthesize one.
				//
				// NOTE that XML Namespaces were not originally
				// defined as being namespace-aware (grrr), and
				// while the W3C is planning to fix this it's
				// safer for now to test the QName and trust the
				// parsers to prevent anyone from redefining the
				// reserved xmlns: prefix
						if (!m_processedFirstElement && "xmlns:xml".Equals(attrs.item(i).NodeName))
						{
						  m_processedFirstElement = true;
						}
					}
					// Terminate list of attrs, and make sure they aren't
					// considered children of the element
				} // if attrs exist
				if (!m_processedFirstElement)
				{
				  // The DOM might not have an explicit declaration for the
				  // implicit "xml:" prefix, but the XPath data model
				  // requires that this appear as a Namespace Node so we
				  // have to synthesize one. You can think of this as
				  // being a default attribute defined by the XML
				  // Namespaces spec rather than by the DTD.
				  attrIndex = addNode(new DOM2DTMdefaultNamespaceDeclarationNode((Element)next,"xml",NAMESPACE_DECL_NS, makeNodeHandle(((attrIndex == org.apache.xml.dtm.DTM_Fields.NULL)?nextindex:attrIndex) + 1)), nextindex,attrIndex,org.apache.xml.dtm.DTM_Fields.NULL);
				  m_firstch.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,attrIndex);
				  m_processedFirstElement = true;
				}
				if (attrIndex != org.apache.xml.dtm.DTM_Fields.NULL)
				{
				  m_nextsib.setElementAt(org.apache.xml.dtm.DTM_Fields.NULL,attrIndex);
				}
			} //if(ELEMENT_NODE)
		} // (if !suppressNode)

		// Text postprocessing: Act on values stored above
		if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE == nexttype || org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE == nexttype)
		{
			// %TBD% If nexttype was forced to TEXT, patch the DTM node

			next = lastTextNode; // Advance the DOM cursor over contiguous text
		}

		// Remember where we left off.
		m_pos = next;
		return true;
	  }


	  /// <summary>
	  /// Return an DOM node for the given node.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A node representation of the DTM node. </returns>
	  public override Node getNode(int nodeHandle)
	  {

		int identity = makeNodeIdentity(nodeHandle);

		return (Node) m_nodes[identity];
	  }

	  /// <summary>
	  /// Get a Node from an identity index.
	  /// </summary>
	  /// NEEDSDOC <param name="nodeIdentity">
	  /// 
	  /// NEEDSDOC ($objectName$) @return </param>
	  protected internal virtual Node lookupNode(int nodeIdentity)
	  {
		return (Node) m_nodes[nodeIdentity];
	  }

	  /// <summary>
	  /// Get the next node identity value in the list, and call the iterator
	  /// if it hasn't been added yet.
	  /// </summary>
	  /// <param name="identity"> The node identity (index). </param>
	  /// <returns> identity+1, or DTM.NULL. </returns>
	  protected internal override int getNextNodeIdentity(int identity)
	  {

		identity += 1;

		if (identity >= m_nodes.Count)
		{
		  if (!nextNode())
		  {
			identity = org.apache.xml.dtm.DTM_Fields.NULL;
		  }
		}

		return identity;
	  }

	  /// <summary>
	  /// Get the handle from a Node.
	  /// <para>%OPT% This will be pretty slow.</para>
	  /// 
	  /// <para>%OPT% An XPath-like search (walk up DOM to root, tracking path;
	  /// walk down DTM reconstructing path) might be considerably faster
	  /// on later nodes in large documents. That might also imply improving
	  /// this call to handle nodes which would be in this DTM but
	  /// have not yet been built, which might or might not be a Good Thing.</para>
	  /// 
	  /// %REVIEW% This relies on being able to test node-identity via
	  /// object-identity. DTM2DOM proxying is a great example of a case where
	  /// that doesn't work. DOM Level 3 will provide the isSameNode() method
	  /// to fix that, but until then this is going to be flaky.
	  /// </summary>
	  /// <param name="node"> A node, which may be null.
	  /// </param>
	  /// <returns> The node handle or <code>DTM.NULL</code>. </returns>
	  private int getHandleFromNode(Node node)
	  {
		if (null != node)
		{
		  int len = m_nodes.Count;
		  bool isMore;
		  int i = 0;
		  do
		  {
			for (; i < len; i++)
			{
			  if (m_nodes[i] == node)
			  {
				return makeNodeHandle(i);
			  }
			}

			isMore = nextNode();

			len = m_nodes.Count;

		  } while (isMore || i < len);
		}

		return org.apache.xml.dtm.DTM_Fields.NULL;
	  }

	  /// <summary>
	  /// Get the handle from a Node. This is a more robust version of
	  /// getHandleFromNode, intended to be usable by the public.
	  /// 
	  /// <para>%OPT% This will be pretty slow.</para>
	  /// 
	  /// %REVIEW% This relies on being able to test node-identity via
	  /// object-identity. DTM2DOM proxying is a great example of a case where
	  /// that doesn't work. DOM Level 3 will provide the isSameNode() method
	  /// to fix that, but until then this is going to be flaky.
	  /// </summary>
	  /// <param name="node"> A node, which may be null.
	  /// </param>
	  /// <returns> The node handle or <code>DTM.NULL</code>.   </returns>
	  public virtual int getHandleOfNode(Node node)
	  {
		if (null != node)
		{
		  // Is Node actually within the same document? If not, don't search!
		  // This would be easier if m_root was always the Document node, but
		  // we decided to allow wrapping a DTM around a subtree.
		  if ((m_root == node) || (m_root.NodeType == org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE && m_root == node.OwnerDocument) || (m_root.NodeType != org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE && m_root.OwnerDocument == node.OwnerDocument))
		  {
			  // If node _is_ in m_root's tree, find its handle
			  //
			  // %OPT% This check may be improved significantly when DOM
			  // Level 3 nodeKey and relative-order tests become
			  // available!
			  for (Node cursor = node; cursor != null; cursor = (cursor.NodeType != org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE) ? cursor.ParentNode : ((Attr)cursor).OwnerElement)
			  {
				  if (cursor == m_root)
				  {
					// We know this node; find its handle.
					return getHandleFromNode(node);
				  }
			  } // for ancestors of node
		  } // if node and m_root in same Document
		} // if node!=null

		return org.apache.xml.dtm.DTM_Fields.NULL;
	  }

	  /// <summary>
	  /// Retrieves an attribute node by by qualified name and namespace URI.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node upon which to look up this attribute.. </param>
	  /// <param name="namespaceURI"> The namespace URI of the attribute to
	  ///   retrieve, or null. </param>
	  /// <param name="name"> The local name of the attribute to
	  ///   retrieve. </param>
	  /// <returns> The attribute node handle with the specified name (
	  ///   <code>nodeName</code>) or <code>DTM.NULL</code> if there is no such
	  ///   attribute. </returns>
	  public override int getAttributeNode(int nodeHandle, string namespaceURI, string name)
	  {

		// %OPT% This is probably slower than it needs to be.
		if (null == namespaceURI)
		{
		  namespaceURI = "";
		}

		int type = getNodeType(nodeHandle);

		if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == type)
		{

		  // Assume that attributes immediately follow the element.
		  int identity = makeNodeIdentity(nodeHandle);

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (identity = getNextNodeIdentity(identity)))
		  {
			// Assume this can not be null.
			type = _type(identity);

					// %REVIEW%
					// Should namespace nodes be retrievable DOM-style as attrs?
					// If not we need a separate function... which may be desirable
					// architecturally, but which is ugly from a code point of view.
					// (If we REALLY insist on it, this code should become a subroutine
					// of both -- retrieve the node, then test if the type matches
					// what you're looking for.)
			if (type == org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE || type == org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE)
			{
			  Node node = lookupNode(identity);
			  string nodeuri = node.NamespaceURI;

			  if (null == nodeuri)
			  {
				nodeuri = "";
			  }

			  string nodelocalname = node.LocalName;

			  if (nodeuri.Equals(namespaceURI) && name.Equals(nodelocalname))
			  {
				return makeNodeHandle(identity);
			  }
			}

			else // if (DTM.NAMESPACE_NODE != type)
			{
			  break;
			}
		  }
		}

		return org.apache.xml.dtm.DTM_Fields.NULL;
	  }

	  /// <summary>
	  /// Get the string-value of a node as a String object
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A string object that represents the string-value of the given node. </returns>
	  public override XMLString getStringValue(int nodeHandle)
	  {

		int type = getNodeType(nodeHandle);
		Node node = getNode(nodeHandle);
		// %TBD% If an element only has one text node, we should just use it 
		// directly.
		if (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == type || org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE == type || org.apache.xml.dtm.DTM_Fields.DOCUMENT_FRAGMENT_NODE == type)
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

		  return m_xstrf.newstr(s);
		}
		else if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE == type || org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE == type)
		{
		  // If this is a DTM text node, it may be made of multiple DOM text
		  // nodes -- including navigating into Entity References. DOM2DTM
		  // records the first node in the sequence and requires that we
		  // pick up the others when we retrieve the DTM node's value.
		  //
		  // %REVIEW% DOM Level 3 is expected to add a "whole text"
		  // retrieval method which performs this function for us.
		  FastStringBuffer buf = StringBufferPool.get();
		  while (node != null)
		  {
			buf.append(node.NodeValue);
			node = logicalNextDOMTextNode(node);
		  }
		  string s = (buf.length() > 0) ? buf.ToString() : "";
		  StringBufferPool.free(buf);
		  return m_xstrf.newstr(s);
		}
		else
		{
		  return m_xstrf.newstr(node.NodeValue);
		}
	  }

	  /// <summary>
	  /// Determine if the string-value of a node is whitespace
	  /// </summary>
	  /// <param name="nodeHandle"> The node Handle.
	  /// </param>
	  /// <returns> Return true if the given node is whitespace. </returns>
	  public virtual bool isWhitespace(int nodeHandle)
	  {
		  int type = getNodeType(nodeHandle);
		Node node = getNode(nodeHandle);
		  if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE == type || org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE == type)
		  {
		  // If this is a DTM text node, it may be made of multiple DOM text
		  // nodes -- including navigating into Entity References. DOM2DTM
		  // records the first node in the sequence and requires that we
		  // pick up the others when we retrieve the DTM node's value.
		  //
		  // %REVIEW% DOM Level 3 is expected to add a "whole text"
		  // retrieval method which performs this function for us.
		  FastStringBuffer buf = StringBufferPool.get();
		  while (node != null)
		  {
			buf.append(node.NodeValue);
			node = logicalNextDOMTextNode(node);
		  }
		 bool b = buf.isWhitespace(0, buf.length());
		  StringBufferPool.free(buf);
		 return b;
		  }
		return false;
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
	  /// </para>
	  /// <para>
	  /// %REVIEW% Actually, since this method operates on the DOM side of the
	  /// fence rather than the DTM side, it SHOULDN'T do
	  /// any special handling. The DOM does what the DOM does; if you want
	  /// DTM-level abstractions, use DTM-level methods.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="node"> Node whose subtree is to be walked, gathering the
	  /// contents of all Text or CDATASection nodes. </param>
	  /// <param name="buf"> FastStringBuffer into which the contents of the text
	  /// nodes are to be concatenated. </param>
	  protected internal static void getNodeData(Node node, FastStringBuffer buf)
	  {

		switch (node.NodeType)
		{
		case Node.DOCUMENT_FRAGMENT_NODE :
		case Node.DOCUMENT_NODE :
		case Node.ELEMENT_NODE :
		{
		  for (Node child = node.FirstChild; null != child; child = child.NextSibling)
		  {
			getNodeData(child, buf);
		  }
		}
		break;
		case Node.TEXT_NODE :
		case Node.CDATA_SECTION_NODE :
		case Node.ATTRIBUTE_NODE : // Never a child but might be our starting node
		  buf.append(node.NodeValue);
		  break;
		case Node.PROCESSING_INSTRUCTION_NODE :
		  // warning(XPATHErrorResources.WG_PARSING_AND_PREPARING);        
		  break;
		default :
		  // ignore
		  break;
		}
	  }

	  /// <summary>
	  /// Given a node handle, return its DOM-style node name. This will
	  /// include names such as #text or #document.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string.
	  /// %REVIEW% Document when empty string is possible...
	  /// %REVIEW-COMMENT% It should never be empty, should it? </returns>
	  public override string getNodeName(int nodeHandle)
	  {

		Node node = getNode(nodeHandle);

		// Assume non-null.
		return node.NodeName;
	  }

	  /// <summary>
	  /// Given a node handle, return the XPath node name.  This should be
	  /// the name as described by the XPath data model, NOT the DOM-style
	  /// name.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string. </returns>
	  public override string getNodeNameX(int nodeHandle)
	  {

		string name;
		short type = getNodeType(nodeHandle);

		switch (type)
		{
		case org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE :
		{
		  Node node = getNode(nodeHandle);

		  // assume not null.
		  name = node.NodeName;
		  if (name.StartsWith("xmlns:", StringComparison.Ordinal))
		  {
			name = QName.getLocalPart(name);
		  }
		  else if (name.Equals("xmlns"))
		  {
			name = "";
		  }
		}
		break;
		case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
		case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
		case org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE :
		case org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE :
		{
		  Node node = getNode(nodeHandle);

		  // assume not null.
		  name = node.NodeName;
		}
		break;
		default :
		  name = "";
	  break;
		}

		return name;
	  }

	  /// <summary>
	  /// Given a node handle, return its XPath-style localname.
	  /// (As defined in Namespaces, this is the portion of the name after any
	  /// colon character).
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Local name of this node. </returns>
	  public override string getLocalName(int nodeHandle)
	  {
		if (JJK_NEWCODE)
		{
		  int id = makeNodeIdentity(nodeHandle);
		  if (org.apache.xml.dtm.DTM_Fields.NULL == id)
		  {
			  return null;
		  }
		  Node newnode = (Node)m_nodes[id];
		  string newname = newnode.LocalName;
		  if (null == newname)
		  {
		// XSLT treats PIs, and possibly other things, as having QNames.
		string qname = newnode.NodeName;
		if ('#' == qname[0])
		{
		  //  Match old default for this function
		  // This conversion may or may not be necessary
		  newname = "";
		}
		else
		{
		  int index = qname.IndexOf(':');
		  newname = (index < 0) ? qname : qname.Substring(index + 1);
		}
		  }
		  return newname;
		}
		else
		{
		  string name;
		  short type = getNodeType(nodeHandle);
		  switch (type)
		  {
		  case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
		  case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
		  case org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE :
		  case org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE :
		  case org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE :
		  {
		  Node node = getNode(nodeHandle);

		  // assume not null.
		  name = node.LocalName;

		  if (null == name)
		  {
			string qname = node.NodeName;
			int index = qname.IndexOf(':');

			name = (index < 0) ? qname : qname.Substring(index + 1);
		  }
		  }
		break;
		  default :
		name = "";
	break;
		  }
		  return name;
		}
	  }

	  /// <summary>
	  /// Given a namespace handle, return the prefix that the namespace decl is
	  /// mapping.
	  /// Given a node handle, return the prefix used to map to the namespace.
	  /// 
	  /// <para> %REVIEW% Are you sure you want "" for no prefix?  </para>
	  /// <para> %REVIEW-COMMENT% I think so... not totally sure. -sb  </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String prefix of this node's name, or "" if no explicit
	  /// namespace prefix was given. </returns>
	  public override string getPrefix(int nodeHandle)
	  {

		string prefix;
		short type = getNodeType(nodeHandle);

		switch (type)
		{
		case org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE :
		{
		  Node node = getNode(nodeHandle);

		  // assume not null.
		  string qname = node.NodeName;
		  int index = qname.IndexOf(':');

		  prefix = (index < 0) ? "" : qname.Substring(index + 1);
		}
		break;
		case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
		case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
		{
		  Node node = getNode(nodeHandle);

		  // assume not null.
		  string qname = node.NodeName;
		  int index = qname.IndexOf(':');

		  prefix = (index < 0) ? "" : qname.Substring(0, index);
		}
		break;
		default :
		  prefix = "";
	  break;
		}

		return prefix;
	  }

	  /// <summary>
	  /// Given a node handle, return its DOM-style namespace URI
	  /// (As defined in Namespaces, this is the declared URI which this node's
	  /// prefix -- or default in lieu thereof -- was mapped to.)
	  /// 
	  /// <para>%REVIEW% Null or ""? -sb</para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String URI value of this node's namespace, or null if no
	  /// namespace was resolved. </returns>
	  public override string getNamespaceURI(int nodeHandle)
	  {
		if (JJK_NEWCODE)
		{
		  int id = makeNodeIdentity(nodeHandle);
		  if (id == org.apache.xml.dtm.DTM_Fields.NULL)
		  {
			  return null;
		  }
		  Node node = (Node)m_nodes[id];
		  return node.NamespaceURI;
		}
		else
		{
		  string nsuri;
		  short type = getNodeType(nodeHandle);

		  switch (type)
		  {
		  case org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE :
		  case org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE :
		  case org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE :
		  case org.apache.xml.dtm.DTM_Fields.NAMESPACE_NODE :
		  case org.apache.xml.dtm.DTM_Fields.PROCESSING_INSTRUCTION_NODE :
		  {
		  Node node = getNode(nodeHandle);

		  // assume not null.
		  nsuri = node.NamespaceURI;

		  // %TBD% Handle DOM1?
		  }
		break;
		  default :
		nsuri = null;
	break;
		  }

		  return nsuri;
		}

	  }

	  /// <summary>
	  /// Utility function: Given a DOM Text node, determine whether it is
	  /// logically followed by another Text or CDATASection node. This may
	  /// involve traversing into Entity References.
	  /// 
	  /// %REVIEW% DOM Level 3 is expected to add functionality which may 
	  /// allow us to retire this.
	  /// </summary>
	  private Node logicalNextDOMTextNode(Node n)
	  {
			Node p = n.NextSibling;
			if (p == null)
			{
					// Walk out of any EntityReferenceNodes that ended with text
					for (n = n.ParentNode; n != null && org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE == n.NodeType; n = n.ParentNode)
					{
							p = n.NextSibling;
							if (p != null)
							{
									break;
							}
					}
			}
			n = p;
			while (n != null && org.apache.xml.dtm.DTM_Fields.ENTITY_REFERENCE_NODE == n.NodeType)
			{
					// Walk into any EntityReferenceNodes that start with text
					if (n.hasChildNodes())
					{
							n = n.FirstChild;
					}
					else
					{
							n = n.NextSibling;
					}
			}
			if (n != null)
			{
					// Found a logical next sibling. Is it text?
					int ntype = n.NodeType;
					if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE != ntype && org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE != ntype)
					{
							n = null;
					}
			}
			return n;
	  }

	  /// <summary>
	  /// Given a node handle, return its node value. This is mostly
	  /// as defined by the DOM, but may ignore some conveniences.
	  /// <para>
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> String Value of this node, or null if not
	  /// meaningful for this node type. </returns>
	  public override string getNodeValue(int nodeHandle)
	  {
		// The _type(nodeHandle) call was taking the lion's share of our
		// time, and was wrong anyway since it wasn't coverting handle to
		// identity. Inlined it.
		int type = _exptype(makeNodeIdentity(nodeHandle));
		type = (org.apache.xml.dtm.DTM_Fields.NULL != type) ? getNodeType(nodeHandle) : org.apache.xml.dtm.DTM_Fields.NULL;

		if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE != type && org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE != type)
		{
		  return getNode(nodeHandle).NodeValue;
		}

		// If this is a DTM text node, it may be made of multiple DOM text
		// nodes -- including navigating into Entity References. DOM2DTM
		// records the first node in the sequence and requires that we
		// pick up the others when we retrieve the DTM node's value.
		//
		// %REVIEW% DOM Level 3 is expected to add a "whole text"
		// retrieval method which performs this function for us.
		Node node = getNode(nodeHandle);
		Node n = logicalNextDOMTextNode(node);
		if (n == null)
		{
		  return node.NodeValue;
		}

		FastStringBuffer buf = StringBufferPool.get();
			buf.append(node.NodeValue);
		while (n != null)
		{
		  buf.append(n.NodeValue);
		  n = logicalNextDOMTextNode(n);
		}
		string s = (buf.length() > 0) ? buf.ToString() : "";
		StringBufferPool.free(buf);
		return s;
	  }

	  /// <summary>
	  ///   A document type declaration information item has the following properties:
	  /// 
	  ///     1. [system identifier] The system identifier of the external subset, if
	  ///        it exists. Otherwise this property has no value.
	  /// </summary>
	  /// <returns> the system identifier String object, or null if there is none. </returns>
	  public override string DocumentTypeDeclarationSystemIdentifier
	  {
		  get
		  {
    
			Document doc;
    
			if (m_root.NodeType == Node.DOCUMENT_NODE)
			{
			  doc = (Document) m_root;
			}
			else
			{
			  doc = m_root.OwnerDocument;
			}
    
			if (null != doc)
			{
			  DocumentType dtd = doc.Doctype;
    
			  if (null != dtd)
			  {
				return dtd.SystemId;
			  }
			}
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Return the public identifier of the external subset,
	  /// normalized as described in 4.2.2 External Entities [XML]. If there is
	  /// no external subset or if it has no public identifier, this property
	  /// has no value.
	  /// </summary>
	  /// <returns> the public identifier String object, or null if there is none. </returns>
	  public override string DocumentTypeDeclarationPublicIdentifier
	  {
		  get
		  {
    
			Document doc;
    
			if (m_root.NodeType == Node.DOCUMENT_NODE)
			{
			  doc = (Document) m_root;
			}
			else
			{
			  doc = m_root.OwnerDocument;
			}
    
			if (null != doc)
			{
			  DocumentType dtd = doc.Doctype;
    
			  if (null != dtd)
			  {
				return dtd.PublicId;
			  }
			}
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Returns the <code>Element</code> whose <code>ID</code> is given by
	  /// <code>elementId</code>. If no such element exists, returns
	  /// <code>DTM.NULL</code>. Behavior is not defined if more than one element
	  /// has this <code>ID</code>. Attributes (including those
	  /// with the name "ID") are not of type ID unless so defined by DTD/Schema
	  /// information available to the DTM implementation.
	  /// Implementations that do not know whether attributes are of type ID or
	  /// not are expected to return <code>DTM.NULL</code>.
	  /// 
	  /// <para>%REVIEW% Presumably IDs are still scoped to a single document,
	  /// and this operation searches only within a single document, right?
	  /// Wouldn't want collisions between DTMs in the same process.</para>
	  /// </summary>
	  /// <param name="elementId"> The unique <code>id</code> value for an element. </param>
	  /// <returns> The handle of the matching element. </returns>
	  public override int getElementById(string elementId)
	  {

		Document doc = (m_root.NodeType == Node.DOCUMENT_NODE) ? (Document) m_root : m_root.OwnerDocument;

		if (null != doc)
		{
		  Node elem = doc.getElementById(elementId);
		  if (null != elem)
		  {
			int elemHandle = getHandleFromNode(elem);

			if (org.apache.xml.dtm.DTM_Fields.NULL == elemHandle)
			{
			  int identity = m_nodes.Count - 1;
			  while (org.apache.xml.dtm.DTM_Fields.NULL != (identity = getNextNodeIdentity(identity)))
			  {
				Node node = getNode(identity);
				if (node == elem)
				{
				  elemHandle = getHandleFromNode(elem);
				  break;
				}
			  }
			}

			return elemHandle;
		  }

		}
		return org.apache.xml.dtm.DTM_Fields.NULL;
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
	  /// pushed up to a higher level of our application. (Note that DOM Level
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
	  /// entity.
	  /// </param>
	  /// <returns> String containing the URI of the Unparsed Entity, or an
	  /// empty string if no such entity exists. </returns>
	  public override string getUnparsedEntityURI(string name)
	  {

		string url = "";
		Document doc = (m_root.NodeType == Node.DOCUMENT_NODE) ? (Document) m_root : m_root.OwnerDocument;

		if (null != doc)
		{
		  DocumentType doctype = doc.Doctype;

		  if (null != doctype)
		  {
			NamedNodeMap entities = doctype.Entities;
			if (null == entities)
			{
			  return url;
			}
			Entity entity = (Entity) entities.getNamedItem(name);
			if (null == entity)
			{
			  return url;
			}

			string notationName = entity.NotationName;

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
			  url = entity.SystemId;

			  if (null == url)
			  {
				url = entity.PublicId;
			  }
			  else
			  {
				// This should be resolved to an absolute URL, but that's hard 
				// to do from here.
			  }
			}
		  }
		}

		return url;
	  }

	  /// <summary>
	  ///     5. [specified] A flag indicating whether this attribute was actually
	  ///        specified in the start-tag of its element, or was defaulted from the
	  ///        DTD.
	  /// </summary>
	  /// <param name="attributeHandle"> the attribute handle </param>
	  /// <returns> <code>true</code> if the attribute was specified;
	  ///         <code>false</code> if it was defaulted. </returns>
	  public override bool isAttributeSpecified(int attributeHandle)
	  {
		int type = getNodeType(attributeHandle);

		if (org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE == type)
		{
		  Attr attr = (Attr)getNode(attributeHandle);
		  return attr.Specified;
		}
		return false;
	  }

	  /// <summary>
	  /// Bind an IncrementalSAXSource to this DTM. NOT RELEVANT for DOM2DTM, since
	  /// we're wrapped around an existing DOM.
	  /// </summary>
	  /// <param name="source"> The IncrementalSAXSource that we want to recieve events from
	  /// on demand. </param>
	  public virtual IncrementalSAXSource IncrementalSAXSource
	  {
		  set
		  {
		  }
	  }

	  /// <summary>
	  /// getContentHandler returns "our SAX builder" -- the thing that
	  /// someone else should send SAX events to in order to extend this
	  /// DTM model.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX events,
	  /// "this" if the DTM object has a built-in SAX ContentHandler,
	  /// the IncrmentalSAXSource if we're bound to one and should receive
	  /// the SAX stream via it for incremental build purposes...
	  ///  </returns>
	  public override ContentHandler ContentHandler
	  {
		  get
		  {
			  return null;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's lexical handler.
	  /// 
	  /// %REVIEW% Should this return null if constrution already done/begun?
	  /// </summary>
	  /// <returns> null if this model doesn't respond to lexical SAX events,
	  /// "this" if the DTM object has a built-in SAX ContentHandler,
	  /// the IncrementalSAXSource if we're bound to one and should receive
	  /// the SAX stream via it for incremental build purposes... </returns>
	  public override org.xml.sax.ext.LexicalHandler LexicalHandler
	  {
		  get
		  {
    
			return null;
		  }
	  }


	  /// <summary>
	  /// Return this DTM's EntityResolver.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX entity ref events. </returns>
	  public override org.xml.sax.EntityResolver EntityResolver
	  {
		  get
		  {
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's DTDHandler.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX dtd events. </returns>
	  public override org.xml.sax.DTDHandler DTDHandler
	  {
		  get
		  {
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's ErrorHandler.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX error events. </returns>
	  public override org.xml.sax.ErrorHandler ErrorHandler
	  {
		  get
		  {
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Return this DTM's DeclHandler.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX Decl events. </returns>
	  public override org.xml.sax.ext.DeclHandler DeclHandler
	  {
		  get
		  {
    
			return null;
		  }
	  }

	  /// <returns> true iff we're building this model incrementally (eg
	  /// we're partnered with a IncrementalSAXSource) and thus require that the
	  /// transformation and the parse run simultaneously. Guidance to the
	  /// DTMManager.
	  ///  </returns>
	  public override bool needsTwoThreads()
	  {
		return false;
	  }

	  // ========== Direct SAX Dispatch, for optimization purposes ========

	  /// <summary>
	  /// Returns whether the specified <var>ch</var> conforms to the XML 1.0 definition
	  /// of whitespace.  Refer to <A href="http://www.w3.org/TR/1998/REC-xml-19980210#NT-S">
	  /// the definition of <CODE>S</CODE></A> for details. </summary>
	  /// <param name="ch">      Character to check as XML whitespace. </param>
	  /// <returns>          =true if <var>ch</var> is XML whitespace; otherwise =false. </returns>
	  private static bool isSpace(char ch)
	  {
		return XMLCharacterRecognizer.isWhiteSpace(ch); // Take the easy way out for now.
	  }

	  /// <summary>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value of the given node (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value). Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, boolean normalize) throws org.xml.sax.SAXException
	  public override void dispatchCharactersEvents(int nodeHandle, ContentHandler ch, bool normalize)
	  {
		if (normalize)
		{
		  XMLString str = getStringValue(nodeHandle);
		  str = str.fixWhiteSpace(true, true, false);
		  str.dispatchCharactersEvents(ch);
		}
		else
		{
		  int type = getNodeType(nodeHandle);
		  Node node = getNode(nodeHandle);
		  dispatchNodeData(node, ch, 0);
			  // Text coalition -- a DTM text node may represent multiple
			  // DOM nodes.
			  if (org.apache.xml.dtm.DTM_Fields.TEXT_NODE == type || org.apache.xml.dtm.DTM_Fields.CDATA_SECTION_NODE == type)
			  {
					  while (null != (node = logicalNextDOMTextNode(node)))
					  {
						  dispatchNodeData(node, ch, 0);
					  }
			  }
		}
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
	  /// </para>
	  /// <para>
	  /// %REVIEW% Note that as a DOM-level operation, it can be argued that this
	  /// routine _shouldn't_ perform any processing beyond what the DOM already
	  /// does, and that whitespace stripping and so on belong at the DTM level.
	  /// If you want a stripped DOM view, wrap DTM2DOM around DOM2DTM.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="node"> Node whose subtree is to be walked, gathering the
	  /// contents of all Text or CDATASection nodes. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void dispatchNodeData(org.w3c.dom.Node node, org.xml.sax.ContentHandler ch, int depth) throws org.xml.sax.SAXException
	  protected internal static void dispatchNodeData(Node node, ContentHandler ch, int depth)
	  {

		switch (node.NodeType)
		{
		case Node.DOCUMENT_FRAGMENT_NODE :
		case Node.DOCUMENT_NODE :
		case Node.ELEMENT_NODE :
		{
		  for (Node child = node.FirstChild; null != child; child = child.NextSibling)
		  {
			dispatchNodeData(child, ch, depth + 1);
		  }
		}
		break;
		case Node.PROCESSING_INSTRUCTION_NODE : // %REVIEW%
		case Node.COMMENT_NODE :
		  if (0 != depth)
		  {
			break;
		  }
			// NOTE: Because this operation works in the DOM space, it does _not_ attempt
			// to perform Text Coalition. That should only be done in DTM space. 
		case Node.TEXT_NODE :
		case Node.CDATA_SECTION_NODE :
		case Node.ATTRIBUTE_NODE :
		  string str = node.NodeValue;
		  if (ch is CharacterNodeHandler)
		  {
			((CharacterNodeHandler)ch).characters(node);
		  }
		  else
		  {
			ch.characters(str.ToCharArray(), 0, str.Length);
		  }
		  break;
	//    /* case Node.PROCESSING_INSTRUCTION_NODE :
	//      // warning(XPATHErrorResources.WG_PARSING_AND_PREPARING);        
	//      break; */
		default :
		  // ignore
		  break;
		}
	  }

	  internal TreeWalker m_walker = new TreeWalker(null);

	  /// <summary>
	  /// Directly create SAX parser events from a subtree.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
	  public override void dispatchToEvents(int nodeHandle, ContentHandler ch)
	  {
		TreeWalker treeWalker = m_walker;
		ContentHandler prevCH = treeWalker.ContentHandler;

		if (null != prevCH)
		{
		  treeWalker = new TreeWalker(null);
		}
		treeWalker.ContentHandler = ch;

		try
		{
		  Node node = getNode(nodeHandle);
		  treeWalker.traverseFragment(node);
		}
		finally
		{
		  treeWalker.ContentHandler = null;
		}
	  }

	  public interface CharacterNodeHandler
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(org.w3c.dom.Node node) throws org.xml.sax.SAXException;
		void characters(Node node);
	  }

	  /// <summary>
	  /// For the moment all the run time properties are ignored by this
	  /// class.
	  /// </summary>
	  /// <param name="property"> a <code>String</code> value </param>
	  /// <param name="value"> an <code>Object</code> value </param>
	  public override void setProperty(string property, object value)
	  {
	  }

	  /// <summary>
	  /// No source information is available for DOM2DTM, so return
	  /// <code>null</code> here.
	  /// </summary>
	  /// <param name="node"> an <code>int</code> value </param>
	  /// <returns> null </returns>
	  public override SourceLocator getSourceLocatorFor(int node)
	  {
		return null;
	  }

	}



}