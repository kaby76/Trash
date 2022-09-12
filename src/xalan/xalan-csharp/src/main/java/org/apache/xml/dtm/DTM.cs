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
 * $Id: DTM.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{

	using XMLString = org.apache.xml.utils.XMLString;

	/// <summary>
	/// <code>DTM</code> is an XML document model expressed as a table
	/// rather than an object tree. It attempts to provide an interface to
	/// a parse tree that has very little object creation. (DTM
	/// implementations may also support incremental construction of the
	/// model, but that's hidden from the DTM API.)
	/// 
	/// <para>Nodes in the DTM are identified by integer "handles".  A handle must
	/// be unique within a process, and carries both node identification and
	/// document identification.  It must be possible to compare two handles
	/// (and thus their nodes) for identity with "==".</para>
	/// 
	/// <para>Namespace URLs, local-names, and expanded-names can all be
	/// represented by and tested as integer ID values.  An expanded name
	/// represents (and may or may not directly contain) a combination of
	/// the URL ID, and the local-name ID.  Note that the namespace URL id
	/// can be 0, which should have the meaning that the namespace is null.
	/// For consistancy, zero should not be used for a local-name index. </para>
	/// 
	/// <para>Text content of a node is represented by an index and length,
	/// permitting efficient storage such as a shared FastStringBuffer.</para>
	/// 
	/// <para>The model of the tree, as well as the general navigation model,
	/// is that of XPath 1.0, for the moment.  The model will eventually be
	/// adapted to match the XPath 2.0 data model, XML Schema, and
	/// InfoSet.</para>
	/// 
	/// <para>DTM does _not_ directly support the W3C's Document Object
	/// Model. However, it attempts to come close enough that an
	/// implementation of DTM can be created that wraps a DOM and vice
	/// versa.</para>
	/// 
	/// <para><strong>Please Note:</strong> The DTM API is still
	/// <strong>Subject To Change.</strong> This wouldn't affect most
	/// users, but might require updating some extensions.</para>
	/// 
	/// <para> The largest change being contemplated is a reconsideration of
	/// the Node Handle representation.  We are still not entirely sure
	/// that an integer packed with two numeric subfields is really the
	/// best solution. It has been suggested that we move up to a Long, to
	/// permit more nodes per document without having to reduce the number
	/// of slots in the DTMManager. There's even been a proposal that we
	/// replace these integers with "cursor" objects containing the
	/// internal node id and a pointer to the actual DTM object; this might
	/// reduce the need to continuously consult the DTMManager to retrieve
	/// the latter, and might provide a useful "hook" back into normal Java
	/// heap management.  But changing this datatype would have huge impact
	/// on Xalan's internals -- especially given Java's lack of C-style
	/// typedefs -- so we won't cut over unless we're convinced the new
	/// solution really would be an improvement!</para>
	/// 
	/// </summary>
	public interface DTM
	{

	  /// <summary>
	  /// Null node handles are represented by this value.
	  /// </summary>

	  // These nodeType mnemonics and values are deliberately the same as those
	  // used by the DOM, for convenient mapping
	  //
	  // %REVIEW% Should we actually define these as initialized to,
	  // eg. org.w3c.dom.Document.ELEMENT_NODE?

	  /// <summary>
	  /// The node is a <code>Root</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is an <code>Element</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is an <code>Attr</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>Text</code> node.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>CDATASection</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is an <code>EntityReference</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is an <code>Entity</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>ProcessingInstruction</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>Comment</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>Document</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>DocumentType</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>DocumentFragment</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>Notation</code>.
	  /// </summary>

	  /// <summary>
	  /// The node is a <code>namespace node</code>. Note that this is not
	  /// currently a node type defined by the DOM API.
	  /// </summary>

	  /// <summary>
	  /// The number of valid nodetypes.
	  /// </summary>

	  // ========= DTM Implementation Control Functions. ==============
	  // %TBD% RETIRED -- do via setFeature if needed. Remove from impls.
	  // public void setParseBlockSize(int blockSizeSuggestion);

	  /// <summary>
	  /// Set an implementation dependent feature.
	  /// <para>
	  /// %REVIEW% Do we really expect to set features on DTMs?
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="featureId"> A feature URL. </param>
	  /// <param name="state"> true if this feature should be on, false otherwise. </param>
	  void setFeature(string featureId, bool state);

	  /// <summary>
	  /// Set a run time property for this DTM instance.
	  /// </summary>
	  /// <param name="property"> a <code>String</code> value </param>
	  /// <param name="value"> an <code>Object</code> value </param>
	  void setProperty(string property, object value);

	  // ========= Document Navigation Functions =========

	  /// <summary>
	  /// This returns a stateless "traverser", that can navigate over an
	  /// XPath axis, though not in document order.
	  /// </summary>
	  /// <param name="axis"> One of Axes.ANCESTORORSELF, etc.
	  /// </param>
	  /// <returns> A DTMAxisIterator, or null if the givin axis isn't supported. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DTMAxisTraverser getAxisTraverser(final int axis);
	  DTMAxisTraverser getAxisTraverser(int axis);

	  /// <summary>
	  /// This is a shortcut to the iterators that implement
	  /// XPath axes.
	  /// Returns a bare-bones iterator that must be initialized
	  /// with a start node (using iterator.setStartNode()).
	  /// </summary>
	  /// <param name="axis"> One of Axes.ANCESTORORSELF, etc.
	  /// </param>
	  /// <returns> A DTMAxisIterator, or null if the givin axis isn't supported. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DTMAxisIterator getAxisIterator(final int axis);
	  DTMAxisIterator getAxisIterator(int axis);

	  /// <summary>
	  /// Get an iterator that can navigate over an XPath Axis, predicated by
	  /// the extended type ID.
	  /// </summary>
	  /// <param name="axis"> </param>
	  /// <param name="type"> An extended type ID.
	  /// </param>
	  /// <returns> A DTMAxisIterator, or null if the givin axis isn't supported. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public DTMAxisIterator getTypedAxisIterator(final int axis, final int type);
	  DTMAxisIterator getTypedAxisIterator(int axis, int type);

	  /// <summary>
	  /// Given a node handle, test if it has child nodes.
	  /// <para> %REVIEW% This is obviously useful at the DOM layer, where it
	  /// would permit testing this without having to create a proxy
	  /// node. It's less useful in the DTM API, where
	  /// (dtm.getFirstChild(nodeHandle)!=DTM.NULL) is just as fast and
	  /// almost as self-evident. But it's a convenience, and eases porting
	  /// of DOM code to DTM.  </para>
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int true if the given node has child nodes. </returns>
	  bool hasChildNodes(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, get the handle of the node's first child.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int DTM node-number of first child,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getFirstChild(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, get the handle of the node's last child.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int Node-number of last child,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getLastChild(int nodeHandle);

	  /// <summary>
	  /// Retrieves an attribute node by local name and namespace URI
	  /// 
	  /// %TBD% Note that we currently have no way to support
	  /// the DOM's old getAttribute() call, which accesses only the qname.
	  /// </summary>
	  /// <param name="elementHandle"> Handle of the node upon which to look up this attribute. </param>
	  /// <param name="namespaceURI"> The namespace URI of the attribute to
	  ///   retrieve, or null. </param>
	  /// <param name="name"> The local name of the attribute to
	  ///   retrieve. </param>
	  /// <returns> The attribute node handle with the specified name (
	  ///   <code>nodeName</code>) or <code>DTM.NULL</code> if there is no such
	  ///   attribute. </returns>
	  int getAttributeNode(int elementHandle, string namespaceURI, string name);

	  /// <summary>
	  /// Given a node handle, get the index of the node's first attribute.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> Handle of first attribute, or DTM.NULL to indicate none exists. </returns>
	  int getFirstAttribute(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, get the index of the node's first namespace node.
	  /// </summary>
	  /// <param name="nodeHandle"> handle to node, which should probably be an element
	  ///                   node, but need not be.
	  /// </param>
	  /// <param name="inScope"> true if all namespaces in scope should be
	  ///                   returned, false if only the node's own
	  ///                   namespace declarations should be returned. </param>
	  /// <returns> handle of first namespace,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getFirstNamespaceNode(int nodeHandle, bool inScope);

	  /// <summary>
	  /// Given a node handle, advance to its next sibling. </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int Node-number of next sibling,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getNextSibling(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, find its preceeding sibling.
	  /// WARNING: DTM implementations may be asymmetric; in some,
	  /// this operation has been resolved by search, and is relatively expensive.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node-number of the previous sib,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getPreviousSibling(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, advance to the next attribute. If an
	  /// element, we advance to its first attribute; if an attr, we advance to
	  /// the next attr of the same element.
	  /// </summary>
	  /// <param name="nodeHandle"> int Handle of the node. </param>
	  /// <returns> int DTM node-number of the resolved attr,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getNextAttribute(int nodeHandle);

	  /// <summary>
	  /// Given a namespace handle, advance to the next namespace in the same scope
	  /// (local or local-plus-inherited, as selected by getFirstNamespaceNode)
	  /// </summary>
	  /// <param name="baseHandle"> handle to original node from where the first child
	  /// was relative to (needed to return nodes in document order). </param>
	  /// <param name="namespaceHandle"> handle to node which must be of type
	  /// NAMESPACE_NODE. </param>
	  /// NEEDSDOC <param name="inScope"> </param>
	  /// <returns> handle of next namespace,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getNextNamespaceNode(int baseHandle, int namespaceHandle, bool inScope);

	  /// <summary>
	  /// Given a node handle, find its parent node.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node handle of parent,
	  /// or DTM.NULL to indicate none exists. </returns>
	  int getParent(int nodeHandle);

	  /// <summary>
	  /// Given a DTM which contains only a single document, 
	  /// find the Node Handle of the  Document node. Note 
	  /// that if the DTM is configured so it can contain multiple
	  /// documents, this call will return the Document currently
	  /// under construction -- but may return null if it's between
	  /// documents. Generally, you should use getOwnerDocument(nodeHandle)
	  /// or getDocumentRoot(nodeHandle) instead.
	  /// </summary>
	  /// <returns> int Node handle of document, or DTM.NULL if a shared DTM
	  /// can not tell us which Document is currently active. </returns>
	  int Document {get;}

	  /// <summary>
	  /// Given a node handle, find the owning document node. This version mimics
	  /// the behavior of the DOM call by the same name.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node handle of owning document, or DTM.NULL if the node was
	  /// a Document. </returns>
	  /// <seealso cref= #getDocumentRoot(int nodeHandle) </seealso>
	  int getOwnerDocument(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, find the owning document node.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> int Node handle of owning document, or the node itself if it was
	  /// a Document. (Note difference from DOM, where getOwnerDocument returns
	  /// null for the Document node.) </returns>
	  /// <seealso cref= #getOwnerDocument(int nodeHandle) </seealso>
	  int getDocumentRoot(int nodeHandle);

	  /// <summary>
	  /// Get the string-value of a node as a String object
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A string object that represents the string-value of the given node. </returns>
	  XMLString getStringValue(int nodeHandle);

	  /// <summary>
	  /// Get number of character array chunks in
	  /// the string-value of a node.
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// Note that a single text node may have multiple text chunks.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> number of character array chunks in
	  ///         the string-value of a node. </returns>
	  int getStringValueChunkCount(int nodeHandle);

	  /// <summary>
	  /// Get a character array chunk in the string-value of a node.
	  /// (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value).
	  /// Note that a single text node may have multiple text chunks.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="chunkIndex"> Which chunk to get. </param>
	  /// <param name="startAndLen">  A two-integer array which, upon return, WILL
	  /// BE FILLED with values representing the chunk's start position
	  /// within the returned character buffer and the length of the chunk. </param>
	  /// <returns> The character array buffer within which the chunk occurs,
	  /// setting startAndLen's contents as a side-effect. </returns>
	  char[] getStringValueChunk(int nodeHandle, int chunkIndex, int[] startAndLen);

	  /// <summary>
	  /// Given a node handle, return an ID that represents the node's expanded name.
	  /// </summary>
	  /// <param name="nodeHandle"> The handle to the node in question.
	  /// </param>
	  /// <returns> the expanded-name id of the node. </returns>
	  int getExpandedTypeID(int nodeHandle);

	  /// <summary>
	  /// Given an expanded name, return an ID.  If the expanded-name does not
	  /// exist in the internal tables, the entry will be created, and the ID will
	  /// be returned.  Any additional nodes that are created that have this
	  /// expanded name will use this ID.
	  /// </summary>
	  /// NEEDSDOC <param name="namespace"> </param>
	  /// NEEDSDOC <param name="localName"> </param>
	  /// NEEDSDOC <param name="type">
	  /// </param>
	  /// <returns> the expanded-name id of the node. </returns>
	  int getExpandedTypeID(string @namespace, string localName, int type);

	  /// <summary>
	  /// Given an expanded-name ID, return the local name part.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> String Local name of this node. </returns>
	  string getLocalNameFromExpandedNameID(int ExpandedNameID);

	  /// <summary>
	  /// Given an expanded-name ID, return the namespace URI part.
	  /// </summary>
	  /// <param name="ExpandedNameID"> an ID that represents an expanded-name. </param>
	  /// <returns> String URI value of this node's namespace, or null if no
	  /// namespace was resolved. </returns>
	  string getNamespaceFromExpandedNameID(int ExpandedNameID);

	  /// <summary>
	  /// Given a node handle, return its DOM-style node name. This will
	  /// include names such as #text or #document.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node, which may be an empty string.
	  /// %REVIEW% Document when empty string is possible... </returns>
	  string getNodeName(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return the XPath node name.  This should be
	  /// the name as described by the XPath data model, NOT the DOM-style
	  /// name.
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Name of this node. </returns>
	  string getNodeNameX(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its DOM-style localname.
	  /// (As defined in Namespaces, this is the portion of the name after the
	  /// prefix, if present, or the whole node name if no prefix exists)
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String Local name of this node. </returns>
	  string getLocalName(int nodeHandle);

	  /// <summary>
	  /// Given a namespace handle, return the prefix that the namespace decl is
	  /// mapping.
	  /// Given a node handle, return the prefix used to map to the namespace.
	  /// (As defined in Namespaces, this is the portion of the name before any
	  /// colon character).
	  /// 
	  /// <para> %REVIEW% Are you sure you want "" for no prefix?  </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String prefix of this node's name, or "" if no explicit
	  /// namespace prefix was given. </returns>
	  string getPrefix(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its DOM-style namespace URI
	  /// (As defined in Namespaces, this is the declared URI which this node's
	  /// prefix -- or default in lieu thereof -- was mapped to.) </summary>
	  /// <param name="nodeHandle"> the id of the node. </param>
	  /// <returns> String URI value of this node's namespace, or null if no
	  /// namespace was resolved. </returns>
	  string getNamespaceURI(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its node value. This is mostly
	  /// as defined by the DOM, but may ignore some conveniences.
	  /// <para>
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> String Value of this node, or null if not
	  /// meaningful for this node type. </returns>
	  string getNodeValue(int nodeHandle);

	  /// <summary>
	  /// Given a node handle, return its DOM-style node type.
	  /// 
	  /// <para>%REVIEW% Generally, returning short is false economy. Return int?</para>
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> int Node type, as per the DOM's Node._NODE constants. </returns>
	  short getNodeType(int nodeHandle);

	  /// <summary>
	  /// Get the depth level of this node in the tree (equals 1 for
	  /// a parentless node).
	  /// </summary>
	  /// <param name="nodeHandle"> The node id. </param>
	  /// <returns> the number of ancestors, plus one
	  /// @xsl.usage internal </returns>
	  short getLevel(int nodeHandle);

	  // ============== Document query functions ==============

	  /// <summary>
	  /// Tests whether DTM DOM implementation implements a specific feature and
	  /// that feature is supported by this node. </summary>
	  /// <param name="feature"> The name of the feature to test. </param>
	  /// <param name="version"> This is the version number of the feature to test.
	  ///   If the version is not
	  ///   specified, supporting any version of the feature will cause the
	  ///   method to return <code>true</code>. </param>
	  /// <returns> Returns <code>true</code> if the specified feature is
	  ///   supported on this node, <code>false</code> otherwise. </returns>
	  bool isSupported(string feature, string version);

	  /// <summary>
	  /// Return the base URI of the document entity. If it is not known
	  /// (because the document was parsed from a socket connection or from
	  /// standard input, for example), the value of this property is unknown.
	  /// </summary>
	  /// <returns> the document base URI String object or null if unknown. </returns>
	  string DocumentBaseURI {get;set;}


	  /// <summary>
	  /// Return the system identifier of the document entity. If
	  /// it is not known, the value of this property is null.
	  /// </summary>
	  /// <param name="nodeHandle"> The node id, which can be any valid node handle. </param>
	  /// <returns> the system identifier String object or null if unknown. </returns>
	  string getDocumentSystemIdentifier(int nodeHandle);

	  /// <summary>
	  /// Return the name of the character encoding scheme
	  ///        in which the document entity is expressed.
	  /// </summary>
	  /// <param name="nodeHandle"> The node id, which can be any valid node handle. </param>
	  /// <returns> the document encoding String object. </returns>
	  string getDocumentEncoding(int nodeHandle);

	  /// <summary>
	  /// Return an indication of the standalone status of the document,
	  ///        either "yes" or "no". This property is derived from the optional
	  ///        standalone document declaration in the XML declaration at the
	  ///        beginning of the document entity, and has no value if there is no
	  ///        standalone document declaration.
	  /// </summary>
	  /// <param name="nodeHandle"> The node id, which can be any valid node handle. </param>
	  /// <returns> the document standalone String object, either "yes", "no", or null. </returns>
	  string getDocumentStandalone(int nodeHandle);

	  /// <summary>
	  /// Return a string representing the XML version of the document. This
	  /// property is derived from the XML declaration optionally present at the
	  /// beginning of the document entity, and has no value if there is no XML
	  /// declaration.
	  /// </summary>
	  /// <param name="documentHandle"> the document handle </param>
	  /// <returns> the document version String object </returns>
	  string getDocumentVersion(int documentHandle);

	  /// <summary>
	  /// Return an indication of
	  /// whether the processor has read the complete DTD. Its value is a
	  /// boolean. If it is false, then certain properties (indicated in their
	  /// descriptions below) may be unknown. If it is true, those properties
	  /// are never unknown.
	  /// </summary>
	  /// <returns> <code>true</code> if all declarations were processed;
	  ///         <code>false</code> otherwise. </returns>
	  bool DocumentAllDeclarationsProcessed {get;}

	  /// <summary>
	  ///   A document type declaration information item has the following properties:
	  /// 
	  ///     1. [system identifier] The system identifier of the external subset, if
	  ///        it exists. Otherwise this property has no value.
	  /// </summary>
	  /// <returns> the system identifier String object, or null if there is none. </returns>
	  string DocumentTypeDeclarationSystemIdentifier {get;}

	  /// <summary>
	  /// Return the public identifier of the external subset,
	  /// normalized as described in 4.2.2 External Entities [XML]. If there is
	  /// no external subset or if it has no public identifier, this property
	  /// has no value.
	  /// </summary>
	  /// <returns> the public identifier String object, or null if there is none. </returns>
	  string DocumentTypeDeclarationPublicIdentifier {get;}

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
	  int getElementById(string elementId);

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
	  /// %REVIEW% Resolve Public Identifiers... or consider changing function name.
	  /// </para>
	  /// <para>
	  /// If we find a relative URI
	  /// reference, XML expects it to be resolved in terms of the base URI
	  /// of the document. The DOM doesn't do that for us, and it isn't
	  /// entirely clear whether that should be done here; currently that's
	  /// pushed up to a higher level of our application. (Note that DOM Level
	  /// 1 didn't store the document's base URI.)
	  /// %REVIEW% Consider resolving Relative URIs.
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
	  string getUnparsedEntityURI(string name);

	  // ============== Boolean methods ================

	  /// <summary>
	  /// Return true if the xsl:strip-space or xsl:preserve-space was processed
	  /// during construction of the document contained in this DTM.
	  /// 
	  /// NEEDSDOC ($objectName$) @return
	  /// </summary>
	  bool supportsPreStripping();

	  /// <summary>
	  /// Figure out whether nodeHandle2 should be considered as being later
	  /// in the document than nodeHandle1, in Document Order as defined
	  /// by the XPath model. This may not agree with the ordering defined
	  /// by other XML applications.
	  /// <para>
	  /// There are some cases where ordering isn't defined, and neither are
	  /// the results of this function -- though we'll generally return true.
	  /// </para>
	  /// <para>
	  /// %REVIEW% Make sure this does the right thing with attribute nodes!!!
	  /// </para>
	  /// <para>
	  /// %REVIEW% Consider renaming for clarity. Perhaps isDocumentOrder(a,b)?
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="firstNodeHandle"> DOM Node to perform position comparison on. </param>
	  /// <param name="secondNodeHandle"> DOM Node to perform position comparison on.
	  /// </param>
	  /// <returns> false if secondNode comes before firstNode, otherwise return true.
	  /// You can think of this as
	  /// <code>(firstNode.documentOrderPosition &lt;= secondNode.documentOrderPosition)</code>. </returns>
	  bool isNodeAfter(int firstNodeHandle, int secondNodeHandle);

	  /// <summary>
	  /// 2. [element content whitespace] A boolean indicating whether a
	  /// text node represents white space appearing within element content
	  /// (see [XML], 2.10 "White Space Handling").  Note that validating
	  /// XML processors are required by XML 1.0 to provide this
	  /// information... but that DOM Level 2 did not support it, since it
	  /// depends on knowledge of the DTD which DOM2 could not guarantee
	  /// would be available.
	  /// <para>
	  /// If there is no declaration for the containing element, an XML
	  /// processor must assume that the whitespace could be meaningful and
	  /// return false. If no declaration has been read, but the [all
	  /// declarations processed] property of the document information item
	  /// is false (so there may be an unread declaration), then the value
	  /// of this property is indeterminate for white space characters and
	  /// should probably be reported as false. It is always false for text
	  /// nodes that contain anything other than (or in addition to) white
	  /// space.
	  /// </para>
	  /// <para>
	  /// Note too that it always returns false for non-Text nodes.
	  /// </para>
	  /// <para>
	  /// %REVIEW% Joe wants to rename this isWhitespaceInElementContent() for clarity
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeHandle"> the node ID. </param>
	  /// <returns> <code>true</code> if the node definitely represents whitespace in
	  /// element content; <code>false</code> otherwise. </returns>
	  bool isCharacterElementContentWhitespace(int nodeHandle);

	  /// <summary>
	  ///    10. [all declarations processed] This property is not strictly speaking
	  ///        part of the infoset of the document. Rather it is an indication of
	  ///        whether the processor has read the complete DTD. Its value is a
	  ///        boolean. If it is false, then certain properties (indicated in their
	  ///        descriptions below) may be unknown. If it is true, those properties
	  ///        are never unknown.
	  /// </summary>
	  /// <param name="documentHandle"> A node handle that must identify a document. </param>
	  /// <returns> <code>true</code> if all declarations were processed;
	  ///         <code>false</code> otherwise. </returns>
	  bool isDocumentAllDeclarationsProcessed(int documentHandle);

	  /// <summary>
	  ///     5. [specified] A flag indicating whether this attribute was actually
	  ///        specified in the start-tag of its element, or was defaulted from the
	  ///        DTD (or schema).
	  /// </summary>
	  /// <param name="attributeHandle"> The attribute handle </param>
	  /// <returns> <code>true</code> if the attribute was specified;
	  ///         <code>false</code> if it was defaulted or the handle doesn't
	  ///            refer to an attribute node. </returns>
	  bool isAttributeSpecified(int attributeHandle);

	  // ========== Direct SAX Dispatch, for optimization purposes ========

	  /// <summary>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value of the given node (see http://www.w3.org/TR/xpath#data-model
	  /// for the definition of a node's string-value). Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler. </param>
	  /// <param name="normalize"> true if the content should be normalized according to
	  /// the rules for the XPath
	  /// <a href="http://www.w3.org/TR/xpath#function-normalize-space">normalize-space</a>
	  /// function.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, boolean normalize) throws org.xml.sax.SAXException;
	  void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, bool normalize);

	  /// <summary>
	  /// Directly create SAX parser events representing the XML content of
	  /// a DTM subtree. This is a "serialize" operation.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID. </param>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException;
	  void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch);

	  /// <summary>
	  /// Return an DOM node for the given node.
	  /// </summary>
	  /// <param name="nodeHandle"> The node ID.
	  /// </param>
	  /// <returns> A node representation of the DTM node. </returns>
	  org.w3c.dom.Node getNode(int nodeHandle);

	  // ==== Construction methods (may not be supported by some implementations!) =====
	  // %REVIEW% What response occurs if not supported?

	  /// <returns> true iff we're building this model incrementally (eg
	  /// we're partnered with a CoroutineParser) and thus require that the
	  /// transformation and the parse run simultaneously. Guidance to the
	  /// DTMManager. </returns>
	  bool needsTwoThreads();

	  // %REVIEW% Do these appends make any sense, should we support a
	  // wider set of methods (like the "append" methods in the
	  // current DTMDocumentImpl draft), or should we just support SAX
	  // listener interfaces?  Should it be a separate interface to
	  // make that distinction explicit?

	  /// <summary>
	  /// Return this DTM's content handler, if it has one.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX events. </returns>
	  org.xml.sax.ContentHandler ContentHandler {get;}

	  /// <summary>
	  /// Return this DTM's lexical handler, if it has one.
	  /// 
	  /// %REVIEW% Should this return null if constrution already done/begun?
	  /// </summary>
	  /// <returns> null if this model doesn't respond to lexical SAX events. </returns>
	  org.xml.sax.ext.LexicalHandler LexicalHandler {get;}

	  /// <summary>
	  /// Return this DTM's EntityResolver, if it has one.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX entity ref events. </returns>
	  org.xml.sax.EntityResolver EntityResolver {get;}

	  /// <summary>
	  /// Return this DTM's DTDHandler, if it has one.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX dtd events. </returns>
	  org.xml.sax.DTDHandler DTDHandler {get;}

	  /// <summary>
	  /// Return this DTM's ErrorHandler, if it has one.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX error events. </returns>
	  org.xml.sax.ErrorHandler ErrorHandler {get;}

	  /// <summary>
	  /// Return this DTM's DeclHandler, if it has one.
	  /// </summary>
	  /// <returns> null if this model doesn't respond to SAX Decl events. </returns>
	  org.xml.sax.ext.DeclHandler DeclHandler {get;}

	  /// <summary>
	  /// Append a child to "the end of the document". Please note that
	  /// the node is always cloned in a base DTM, since our basic behavior
	  /// is immutable so nodes can't be removed from their previous
	  /// location.
	  /// 
	  /// <para> %REVIEW%  DTM maintains an insertion cursor which
	  /// performs a depth-first tree walk as nodes come in, and this operation
	  /// is really equivalent to:
	  ///    insertionCursor.appendChild(document.importNode(newChild)))
	  /// where the insert point is the last element that was appended (or
	  /// the last one popped back to by an end-element operation).</para>
	  /// </summary>
	  /// <param name="newChild"> Must be a valid new node handle. </param>
	  /// <param name="clone"> true if the child should be cloned into the document. </param>
	  /// <param name="cloneDepth"> if the clone argument is true, specifies that the
	  ///                   clone should include all it's children. </param>
	  void appendChild(int newChild, bool clone, bool cloneDepth);

	  /// <summary>
	  /// Append a text node child that will be constructed from a string,
	  /// to the end of the document. Behavior is otherwise like appendChild().
	  /// </summary>
	  /// <param name="str"> Non-null reference to a string. </param>
	  void appendTextChild(string str);

	  /// <summary>
	  /// Get the location of a node in the source document.
	  /// </summary>
	  /// <param name="node"> an <code>int</code> value </param>
	  /// <returns> a <code>SourceLocator</code> value or null if no location
	  /// is available </returns>
	  SourceLocator getSourceLocatorFor(int node);

	  /// <summary>
	  /// As the DTM is registered with the DTMManager, this method
	  /// will be called. This will give the DTM implementation a
	  /// chance to initialize any subsystems that are required to
	  /// build the DTM
	  /// </summary>
	  void documentRegistration();

	  /// <summary>
	  /// As documents are released from the DTMManager, the DTM implementation
	  /// will be notified of the event. This will allow the DTM implementation
	  /// to shutdown any subsystem activity that may of been assoiated with
	  /// the active DTM Implementation.
	  /// </summary>

	   void documentRelease();

	   /// <summary>
	   /// Migrate a DTM built with an old DTMManager to a new DTMManager.
	   /// After the migration, the new DTMManager will treat the DTM as
	   /// one that is built by itself.
	   /// This is used to support DTM sharing between multiple transformations. </summary>
	   /// <param name="manager"> the DTMManager </param>
	   void migrateTo(DTMManager manager);
	}

	public static class DTM_Fields
	{
	  public const int NULL = -1;
	  public const short ROOT_NODE = 0;
	  public const short ELEMENT_NODE = 1;
	  public const short ATTRIBUTE_NODE = 2;
	  public const short TEXT_NODE = 3;
	  public const short CDATA_SECTION_NODE = 4;
	  public const short ENTITY_REFERENCE_NODE = 5;
	  public const short ENTITY_NODE = 6;
	  public const short PROCESSING_INSTRUCTION_NODE = 7;
	  public const short COMMENT_NODE = 8;
	  public const short DOCUMENT_NODE = 9;
	  public const short DOCUMENT_TYPE_NODE = 10;
	  public const short DOCUMENT_FRAGMENT_NODE = 11;
	  public const short NOTATION_NODE = 12;
	  public const short NAMESPACE_NODE = 13;
	  public const short NTYPES = 14;
	}

}