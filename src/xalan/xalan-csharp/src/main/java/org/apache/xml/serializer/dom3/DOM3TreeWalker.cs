using System;
using System.Collections;
using System.Text;

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
 * $Id: DOM3TreeWalker.java 1225446 2011-12-29 06:12:59Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{


	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using XML11Char = org.apache.xml.serializer.utils.XML11Char;
	using XMLChar = org.apache.xml.serializer.utils.XMLChar;
	using Attr = org.w3c.dom.Attr;
	using CDATASection = org.w3c.dom.CDATASection;
	using Comment = org.w3c.dom.Comment;
	using DOMError = org.w3c.dom.DOMError;
	using DOMErrorHandler = org.w3c.dom.DOMErrorHandler;
	using Document = org.w3c.dom.Document;
	using DocumentType = org.w3c.dom.DocumentType;
	using Element = org.w3c.dom.Element;
	using Entity = org.w3c.dom.Entity;
	using EntityReference = org.w3c.dom.EntityReference;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using ProcessingInstruction = org.w3c.dom.ProcessingInstruction;
	using Text = org.w3c.dom.Text;
	using LSSerializerFilter = org.w3c.dom.ls.LSSerializerFilter;
	using NodeFilter = org.w3c.dom.traversal.NodeFilter;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;
	using LocatorImpl = org.xml.sax.helpers.LocatorImpl;

	/// <summary>
	/// Built on org.apache.xml.serializer.TreeWalker and adds functionality to
	/// traverse and serialize a DOM Node (Level 2 or Level 3) as specified in 
	/// the DOM Level 3 LS Recommedation by evaluating and applying DOMConfiguration 
	/// parameters and filters if any during serialization.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal sealed class DOM3TreeWalker
	{

		/// <summary>
		/// The SerializationHandler, it extends ContentHandler and when
		/// this class is instantiated via the constructor provided, a
		/// SerializationHandler object is passed to it.
		/// </summary>
		private SerializationHandler fSerializer = null;

		/// <summary>
		/// We do not need DOM2Helper since DOM Level 3 LS applies to DOM Level 2 or newer </summary>

		/// <summary>
		/// Locator object for this TreeWalker </summary>
		private LocatorImpl fLocator = new LocatorImpl();

		/// <summary>
		/// ErrorHandler </summary>
		private DOMErrorHandler fErrorHandler = null;

		/// <summary>
		/// LSSerializerFilter </summary>
		private LSSerializerFilter fFilter = null;

		/// <summary>
		/// If the serializer is an instance of a LexicalHandler </summary>
		private LexicalHandler fLexicalHandler = null;

		private int fWhatToShowFilter;

		/// <summary>
		/// New Line character to use in serialization </summary>
		private string fNewLine = null;

		/// <summary>
		/// DOMConfiguration Properties </summary>
		private Properties fDOMConfigProperties = null;

		/// <summary>
		/// Keeps track if we are in an entity reference when entities=true </summary>
		private bool fInEntityRef = false;

		/// <summary>
		/// Stores the version of the XML document to be serialize </summary>
		private string fXMLVersion = null;

		/// <summary>
		/// XML Version, default 1.0 </summary>
		private bool fIsXMLVersion11 = false;

		/// <summary>
		/// Is the Node a Level 3 DOM node </summary>
		private bool fIsLevel3DOM = false;

		/// <summary>
		/// DOM Configuration Parameters </summary>
		private int fFeatures = 0;

		/// <summary>
		/// Flag indicating whether following text to be processed is raw text </summary>
		internal bool fNextIsRaw = false;

		// 
		private const string XMLNS_URI = "http://www.w3.org/2000/xmlns/";

		//
		private const string XMLNS_PREFIX = "xmlns";

		// 
		private const string XML_URI = "http://www.w3.org/XML/1998/namespace";

		// 
		private const string XML_PREFIX = "xml";

		/// <summary>
		/// stores namespaces in scope </summary>
		protected internal NamespaceSupport fNSBinder;

		/// <summary>
		/// stores all namespace bindings on the current element </summary>
		protected internal NamespaceSupport fLocalNSBinder;

		/// <summary>
		/// stores the current element depth </summary>
		private int fElementDepth = 0;

		// ***********************************************************************
		// DOMConfiguration paramter settings 
		// ***********************************************************************
		// Parameter canonical-form, true [optional] - NOT SUPPORTED 
		private static readonly int CANONICAL = 0x1 << 0;

		// Parameter cdata-sections, true [required] (default)
		private static readonly int CDATA = 0x1 << 1;

		// Parameter check-character-normalization, true [optional] - NOT SUPPORTED 
		private static readonly int CHARNORMALIZE = 0x1 << 2;

		// Parameter comments, true [required] (default)
		private static readonly int COMMENTS = 0x1 << 3;

		// Parameter datatype-normalization, true [optional] - NOT SUPPORTED
		private static readonly int DTNORMALIZE = 0x1 << 4;

		// Parameter element-content-whitespace, true [required] (default) - value - false [optional] NOT SUPPORTED
		private static readonly int ELEM_CONTENT_WHITESPACE = 0x1 << 5;

		// Parameter entities, true [required] (default)
		private static readonly int ENTITIES = 0x1 << 6;

		// Parameter infoset, true [required] (default), false has no effect --> True has no effect for the serializer
		private static readonly int INFOSET = 0x1 << 7;

		// Parameter namespaces, true [required] (default)
		private static readonly int NAMESPACES = 0x1 << 8;

		// Parameter namespace-declarations, true [required] (default)
		private static readonly int NAMESPACEDECLS = 0x1 << 9;

		// Parameter normalize-characters, true [optional] - NOT SUPPORTED
		private static readonly int NORMALIZECHARS = 0x1 << 10;

		// Parameter split-cdata-sections, true [required] (default)
		private static readonly int SPLITCDATA = 0x1 << 11;

		// Parameter validate, true [optional] - NOT SUPPORTED
		private static readonly int VALIDATE = 0x1 << 12;

		// Parameter validate-if-schema, true [optional] - NOT SUPPORTED
		private static readonly int SCHEMAVALIDATE = 0x1 << 13;

		// Parameter split-cdata-sections, true [required] (default)
		private static readonly int WELLFORMED = 0x1 << 14;

		// Parameter discard-default-content, true [required] (default)
		// Not sure how this will be used in level 2 Documents
		private static readonly int DISCARDDEFAULT = 0x1 << 15;

		// Parameter format-pretty-print, true [optional] 
		private static readonly int PRETTY_PRINT = 0x1 << 16;

		// Parameter ignore-unknown-character-denormalizations, true [required] (default)
		// We currently do not support XML 1.1 character normalization
		private static readonly int IGNORE_CHAR_DENORMALIZE = 0x1 << 17;

		// Parameter discard-default-content, true [required] (default)
		private static readonly int XMLDECL = 0x1 << 18;

		/// <summary>
		/// Constructor. </summary>
		/// <param name="contentHandler"> serialHandler The implemention of the SerializationHandler interface </param>
		internal DOM3TreeWalker(SerializationHandler serialHandler, DOMErrorHandler errHandler, LSSerializerFilter filter, string newLine)
		{
			fSerializer = serialHandler;
			//fErrorHandler = errHandler == null ? new DOMErrorHandlerImpl() : errHandler; // Should we be using the default?
			fErrorHandler = errHandler;
			fFilter = filter;
			fLexicalHandler = null;
			fNewLine = newLine;

			fNSBinder = new NamespaceSupport();
			fLocalNSBinder = new NamespaceSupport();

			fDOMConfigProperties = fSerializer.OutputFormat;
			fSerializer.DocumentLocator = fLocator;
			initProperties(fDOMConfigProperties);

			try
			{
				// Bug see Bugzilla  26741
				fLocator.SystemId = System.getProperty("user.dir") + File.separator + "dummy.xsl";
			}
			catch (SecurityException)
			{ // user.dir not accessible from applet

			}
		}

		/// <summary>
		/// Perform a pre-order traversal non-recursive style.  
		/// 
		/// Note that TreeWalker assumes that the subtree is intended to represent 
		/// a complete (though not necessarily well-formed) document and, during a 
		/// traversal, startDocument and endDocument will always be issued to the 
		/// SAX listener.
		/// </summary>
		/// <param name="pos"> Node in the tree where to start traversal
		/// </param>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void traverse(org.w3c.dom.Node pos) throws org.xml.sax.SAXException
		public void traverse(Node pos)
		{
			this.fSerializer.startDocument();

			// Determine if the Node is a DOM Level 3 Core Node.
			if (pos.NodeType != Node.DOCUMENT_NODE)
			{
				Document ownerDoc = pos.OwnerDocument;
				if (ownerDoc != null && ownerDoc.Implementation.hasFeature("Core", "3.0"))
				{
					fIsLevel3DOM = true;
				}
			}
			else
			{
				if (((Document) pos).Implementation.hasFeature("Core", "3.0"))
				{
					fIsLevel3DOM = true;
				}
			}

			if (fSerializer is LexicalHandler)
			{
				fLexicalHandler = ((LexicalHandler) this.fSerializer);
			}

			if (fFilter != null)
			{
				fWhatToShowFilter = fFilter.WhatToShow;
			}

			Node top = pos;

			while (null != pos)
			{
				startNode(pos);

				Node nextNode = null;

				nextNode = pos.FirstChild;

				while (null == nextNode)
				{
					endNode(pos);

					if (top.Equals(pos))
					{
						break;
					}

					nextNode = pos.NextSibling;

					if (null == nextNode)
					{
						pos = pos.ParentNode;

						if ((null == pos) || (top.Equals(pos)))
						{
							if (null != pos)
							{
								endNode(pos);
							}

							nextNode = null;

							break;
						}
					}
				}

				pos = nextNode;
			}
			this.fSerializer.endDocument();
		}

		/// <summary>
		/// Perform a pre-order traversal non-recursive style.
		/// 
		/// Note that TreeWalker assumes that the subtree is intended to represent 
		/// a complete (though not necessarily well-formed) document and, during a 
		/// traversal, startDocument and endDocument will always be issued to the 
		/// SAX listener.
		/// </summary>
		/// <param name="pos"> Node in the tree where to start traversal </param>
		/// <param name="top"> Node in the tree where to end traversal
		/// </param>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void traverse(org.w3c.dom.Node pos, org.w3c.dom.Node top) throws org.xml.sax.SAXException
		public void traverse(Node pos, Node top)
		{

			this.fSerializer.startDocument();

			// Determine if the Node is a DOM Level 3 Core Node.
			if (pos.NodeType != Node.DOCUMENT_NODE)
			{
				Document ownerDoc = pos.OwnerDocument;
				if (ownerDoc != null && ownerDoc.Implementation.hasFeature("Core", "3.0"))
				{
					fIsLevel3DOM = true;
				}
			}
			else
			{
				if (((Document) pos).Implementation.hasFeature("Core", "3.0"))
				{
					fIsLevel3DOM = true;
				}
			}

			if (fSerializer is LexicalHandler)
			{
				fLexicalHandler = ((LexicalHandler) this.fSerializer);
			}

			if (fFilter != null)
			{
				fWhatToShowFilter = fFilter.WhatToShow;
			}

			while (null != pos)
			{
				startNode(pos);

				Node nextNode = null;

				nextNode = pos.FirstChild;

				while (null == nextNode)
				{
					endNode(pos);

					if ((null != top) && top.Equals(pos))
					{
						break;
					}

					nextNode = pos.NextSibling;

					if (null == nextNode)
					{
						pos = pos.ParentNode;

						if ((null == pos) || ((null != top) && top.Equals(pos)))
						{
							nextNode = null;

							break;
						}
					}
				}

				pos = nextNode;
			}
			this.fSerializer.endDocument();
		}

		/// <summary>
		/// Optimized dispatch of characters.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final void dispatachChars(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		private void dispatachChars(Node node)
		{
			if (fSerializer != null)
			{
				this.fSerializer.characters(node);
			}
			else
			{
				string data = ((Text) node).Data;
				this.fSerializer.characters(data.ToCharArray(), 0, data.Length);
			}
		}

		/// <summary>
		/// Start processing given node
		/// </summary>
		/// <param name="node"> Node to process
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void startNode(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		protected internal void startNode(Node node)
		{
			if (node is Locator)
			{
				Locator loc = (Locator) node;
				fLocator.ColumnNumber = loc.ColumnNumber;
				fLocator.LineNumber = loc.LineNumber;
				fLocator.PublicId = loc.PublicId;
				fLocator.SystemId = loc.SystemId;
			}
			else
			{
				fLocator.ColumnNumber = 0;
				fLocator.LineNumber = 0;
			}

			switch (node.NodeType)
			{
				case Node.DOCUMENT_TYPE_NODE :
					serializeDocType((DocumentType) node, true);
					break;
				case Node.COMMENT_NODE :
					serializeComment((Comment) node);
					break;
				case Node.DOCUMENT_FRAGMENT_NODE :
					// Children are traversed
					break;
				case Node.DOCUMENT_NODE :
					break;
				case Node.ELEMENT_NODE :
					serializeElement((Element) node, true);
					break;
				case Node.PROCESSING_INSTRUCTION_NODE :
					serializePI((ProcessingInstruction) node);
					break;
				case Node.CDATA_SECTION_NODE :
					serializeCDATASection((CDATASection) node);
					break;
				case Node.TEXT_NODE :
					serializeText((Text) node);
					break;
				case Node.ENTITY_REFERENCE_NODE :
					serializeEntityReference((EntityReference) node, true);
					break;
				default :
			break;
			}
		}

		/// <summary>
		/// End processing of given node 
		/// 
		/// </summary>
		/// <param name="node"> Node we just finished processing
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void endNode(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		protected internal void endNode(Node node)
		{

			switch (node.NodeType)
			{
				case Node.DOCUMENT_NODE :
					break;
				case Node.DOCUMENT_TYPE_NODE :
					serializeDocType((DocumentType) node, false);
					break;
				case Node.ELEMENT_NODE :
					serializeElement((Element) node, false);
					break;
				case Node.CDATA_SECTION_NODE :
					break;
				case Node.ENTITY_REFERENCE_NODE :
					serializeEntityReference((EntityReference) node, false);
					break;
				default :
			break;
			}
		}

		// ***********************************************************************
		// Node serialization methods
		// ***********************************************************************
		/// <summary>
		/// Applies a filter on the node to serialize
		/// </summary>
		/// <param name="node"> The Node to serialize </param>
		/// <returns> True if the node is to be serialized else false if the node 
		///         is to be rejected or skipped.  </returns>
		protected internal bool applyFilter(Node node, int nodeType)
		{
			if (fFilter != null && (fWhatToShowFilter & nodeType) != 0)
			{

				short code = fFilter.acceptNode(node);
				switch (code)
				{
					case NodeFilter.FILTER_REJECT :
					case NodeFilter.FILTER_SKIP :
						return false; // skip the node
					default : // fall through..
				break;
				}
			}
			return true;
		}

		/// <summary>
		/// Serializes a Document Type Node.
		/// </summary>
		/// <param name="node"> The Docuemnt Type Node to serialize </param>
		/// <param name="bStart"> Invoked at the start or end of node.  Default true.  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeDocType(org.w3c.dom.DocumentType node, boolean bStart) throws org.xml.sax.SAXException
		protected internal void serializeDocType(DocumentType node, bool bStart)
		{
			// The DocType and internalSubset can not be modified in DOM and is
			// considered to be well-formed as the outcome of successful parsing.
			string docTypeName = node.NodeName;
			string publicId = node.PublicId;
			string systemId = node.SystemId;
			string internalSubset = node.InternalSubset;

			//DocumentType nodes are never passed to the filter

			if (!string.ReferenceEquals(internalSubset, null) && !"".Equals(internalSubset))
			{

				if (bStart)
				{
					try
					{
						// The Serializer does not provide a way to write out the
						// DOCTYPE internal subset via an event call, so we write it
						// out here.
						Writer writer = fSerializer.Writer;
						StringBuilder dtd = new StringBuilder();

						dtd.Append("<!DOCTYPE ");
						dtd.Append(docTypeName);
						if (null != publicId)
						{
							dtd.Append(" PUBLIC \"");
							dtd.Append(publicId);
							dtd.Append('\"');
						}

						if (null != systemId)
						{
							if (null == publicId)
							{
								dtd.Append(" SYSTEM \"");
							}
							else
							{
								dtd.Append(" \"");
							}
							dtd.Append(systemId);
							dtd.Append('\"');
						}

						dtd.Append(" [ ");

						dtd.Append(fNewLine);
						dtd.Append(internalSubset);
						dtd.Append("]>");
						dtd.Append(fNewLine);

						writer.write(dtd.ToString());
						writer.flush();

					}
					catch (IOException e)
					{
						throw new SAXException(Utils.messages.createMessage(MsgKey.ER_WRITING_INTERNAL_SUBSET, null), e);
					}
				} // else if !bStart do nothing

			}
			else
			{

				if (bStart)
				{
					if (fLexicalHandler != null)
					{
						fLexicalHandler.startDTD(docTypeName, publicId, systemId);
					}
				}
				else
				{
					if (fLexicalHandler != null)
					{
						fLexicalHandler.endDTD();
					}
				}
			}
		}

		/// <summary>
		/// Serializes a Comment Node.
		/// </summary>
		/// <param name="node"> The Comment Node to serialize </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeComment(org.w3c.dom.Comment node) throws org.xml.sax.SAXException
		protected internal void serializeComment(Comment node)
		{
			// comments=true
			if ((fFeatures & COMMENTS) != 0)
			{
				string data = node.Data;

				// well-formed=true
				if ((fFeatures & WELLFORMED) != 0)
				{
					isCommentWellFormed(data);
				}

				if (fLexicalHandler != null)
				{
					// apply the LSSerializer filter after the operations requested by the 
					// DOMConfiguration parameters have been applied 
					if (!applyFilter(node, NodeFilter.SHOW_COMMENT))
					{
						return;
					}

					fLexicalHandler.comment(data.ToCharArray(), 0, data.Length);
				}
			}
		}

		/// <summary>
		/// Serializes an Element Node.
		/// </summary>
		/// <param name="node"> The Element Node to serialize </param>
		/// <param name="bStart"> Invoked at the start or end of node.    </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeElement(org.w3c.dom.Element node, boolean bStart) throws org.xml.sax.SAXException
		protected internal void serializeElement(Element node, bool bStart)
		{
			if (bStart)
			{
				fElementDepth++;

				// We use the Xalan specific startElement and starPrefixMapping calls 
				// (and addAttribute and namespaceAfterStartElement) as opposed to
				// SAX specific, for performance reasons as they reduce the overhead
				// of creating an AttList object upfront.

				// well-formed=true
				if ((fFeatures & WELLFORMED) != 0)
				{
					isElementWellFormed(node);
				}

				// REVISIT: We apply the LSSerializer filter for elements before
				// namesapce fixup
				if (!applyFilter(node, NodeFilter.SHOW_ELEMENT))
				{
					return;
				}

				// namespaces=true, record and fixup namspaced element
				if ((fFeatures & NAMESPACES) != 0)
				{
					fNSBinder.pushContext();
					fLocalNSBinder.reset();

					recordLocalNSDecl(node);
					fixupElementNS(node);
				}

				// Namespace normalization
				fSerializer.startElement(node.NamespaceURI, node.LocalName, node.NodeName);

				serializeAttList(node);

			}
			else
			{
				fElementDepth--;

				// apply the LSSerializer filter
				if (!applyFilter(node, NodeFilter.SHOW_ELEMENT))
				{
					return;
				}

				this.fSerializer.endElement(node.NamespaceURI, node.LocalName, node.NodeName);
				// since endPrefixMapping was not used by SerializationHandler it was removed
				// for performance reasons.

				if ((fFeatures & NAMESPACES) != 0)
				{
						fNSBinder.popContext();
				}

			}
		}

		/// <summary>
		/// Serializes the Attr Nodes of an Element.
		/// </summary>
		/// <param name="node"> The OwnerElement whose Attr Nodes are to be serialized. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeAttList(org.w3c.dom.Element node) throws org.xml.sax.SAXException
		protected internal void serializeAttList(Element node)
		{
			NamedNodeMap atts = node.Attributes;
			int nAttrs = atts.Length;

			for (int i = 0; i < nAttrs; i++)
			{
				Node attr = atts.item(i);

				string localName = attr.LocalName;
				string attrName = attr.NodeName;
				string attrPrefix = attr.Prefix == null ? "" : attr.Prefix;
				string attrValue = attr.NodeValue;

				// Determine the Attr's type.
				string type = null;
				if (fIsLevel3DOM)
				{
					type = ((Attr) attr).SchemaTypeInfo.TypeName;
				}
				type = string.ReferenceEquals(type, null) ? "CDATA" : type;

				string attrNS = attr.NamespaceURI;
				if (!string.ReferenceEquals(attrNS, null) && attrNS.Length == 0)
				{
					attrNS = null;
					// we must remove prefix for this attribute
					attrName = attr.LocalName;
				}

				bool isSpecified = ((Attr) attr).Specified;
				bool addAttr = true;
				bool applyFilter = false;
				bool xmlnsAttr = attrName.Equals("xmlns") || attrName.StartsWith("xmlns:", StringComparison.Ordinal);

				// well-formed=true
				if ((fFeatures & WELLFORMED) != 0)
				{
					isAttributeWellFormed(attr);
				}

				//-----------------------------------------------------------------
				// start Attribute namespace fixup
				//-----------------------------------------------------------------
				// namespaces=true, normalize all non-namespace attributes
				// Step 3. Attribute
				if ((fFeatures & NAMESPACES) != 0 && !xmlnsAttr)
				{

					// If the Attr has a namespace URI
					if (!string.ReferenceEquals(attrNS, null))
					{
						attrPrefix = string.ReferenceEquals(attrPrefix, null) ? "" : attrPrefix;

						string declAttrPrefix = fNSBinder.getPrefix(attrNS);
						string declAttrNS = fNSBinder.getURI(attrPrefix);

						// attribute has no prefix (default namespace decl does not apply to
						// attributes)
						// OR
						// attribute prefix is not declared
						// OR
						// conflict: attribute has a prefix that conflicts with a binding
						if ("".Equals(attrPrefix) || "".Equals(declAttrPrefix) || !attrPrefix.Equals(declAttrPrefix))
						{

							// namespaceURI matches an in scope declaration of one or
							// more prefixes
							if (!string.ReferenceEquals(declAttrPrefix, null) && !"".Equals(declAttrPrefix))
							{
								// pick the prefix that was found and change attribute's
								// prefix and nodeName.
								attrPrefix = declAttrPrefix;

								if (declAttrPrefix.Length > 0)
								{
									attrName = declAttrPrefix + ":" + localName;
								}
								else
								{
									attrName = localName;
								}
							}
							else
							{
								// The current prefix is not null and it has no in scope
								// declaration
								if (!string.ReferenceEquals(attrPrefix, null) && !"".Equals(attrPrefix) && string.ReferenceEquals(declAttrNS, null))
								{
									// declare this prefix
									if ((fFeatures & NAMESPACEDECLS) != 0)
									{
										fSerializer.addAttribute(XMLNS_URI, attrPrefix, XMLNS_PREFIX + ":" + attrPrefix, "CDATA", attrNS);
										fNSBinder.declarePrefix(attrPrefix, attrNS);
										fLocalNSBinder.declarePrefix(attrPrefix, attrNS);
									}
								}
								else
								{
									// find a prefix following the pattern "NS" +index
									// (starting at 1)
									// make sure this prefix is not declared in the current
									// scope.
									int counter = 1;
									attrPrefix = "NS" + counter++;

									while (!string.ReferenceEquals(fLocalNSBinder.getURI(attrPrefix), null))
									{
										attrPrefix = "NS" + counter++;
									}
									// change attribute's prefix and Name
									attrName = attrPrefix + ":" + localName;

									// create a local namespace declaration attribute
									// Add the xmlns declaration attribute
									if ((fFeatures & NAMESPACEDECLS) != 0)
									{

										fSerializer.addAttribute(XMLNS_URI, attrPrefix, XMLNS_PREFIX + ":" + attrPrefix, "CDATA", attrNS);
										fNSBinder.declarePrefix(attrPrefix, attrNS);
										fLocalNSBinder.declarePrefix(attrPrefix, attrNS);
									}
								}
							}
						}

					}
					else
					{ // if the Attr has no namespace URI
						// Attr has no localName
						if (string.ReferenceEquals(localName, null))
						{
							// DOM Level 1 node!
							string msg = Utils.messages.createMessage(MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, new object[] {attrName});

							if (fErrorHandler != null)
							{
								fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_ERROR, msg, MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, null, null, null));
							}

						}
						else
						{ // uri=null and no colon
							// attr has no namespace URI and no prefix
							// no action is required, since attrs don't use default
						}
					}

				}


				// discard-default-content=true
				// Default attr's are not passed to the filter and this contraint
				// is applied only when discard-default-content=true 
				// What about default xmlns attributes???? check for xmlnsAttr
				if ((((fFeatures & DISCARDDEFAULT) != 0) && isSpecified) || ((fFeatures & DISCARDDEFAULT) == 0))
				{
					applyFilter = true;
				}
				else
				{
					addAttr = false;
				}

				if (applyFilter)
				{
					// apply the filter for Attributes that are not default attributes
					// or namespace decl attributes
					if (fFilter != null && (fFilter.WhatToShow & NodeFilter.SHOW_ATTRIBUTE) != 0)
					{

						if (!xmlnsAttr)
						{
							short code = fFilter.acceptNode(attr);
							switch (code)
							{
								case NodeFilter.FILTER_REJECT :
								case NodeFilter.FILTER_SKIP :
									addAttr = false;
									break;
								default : //fall through..
							break;
							}
						}
					}
				}

				// if the node is a namespace node
				if (addAttr && xmlnsAttr)
				{
					// If namespace-declarations=true, add the node , else don't add it
					if ((fFeatures & NAMESPACEDECLS) != 0)
					{
						   // The namespace may have been fixed up, in that case don't add it.
						if (!string.ReferenceEquals(localName, null) && !"".Equals(localName))
						{
							fSerializer.addAttribute(attrNS, localName, attrName, type, attrValue);
						}
					}
				}
				else if (addAttr && !xmlnsAttr)
				{ // if the node is not a namespace node
					// If namespace-declarations=true, add the node with the Attr nodes namespaceURI
					// else add the node setting it's namespace to null or else the serializer will later
					// attempt to add a xmlns attr for the prefixed attribute
					if (((fFeatures & NAMESPACEDECLS) != 0) && (!string.ReferenceEquals(attrNS, null)))
					{
						fSerializer.addAttribute(attrNS, localName, attrName, type, attrValue);
					}
					else
					{
						fSerializer.addAttribute("", localName, attrName, type, attrValue);
					}
				}

				// 
				if (xmlnsAttr && ((fFeatures & NAMESPACEDECLS) != 0))
				{
					int index;
					// Use "" instead of null, as Xerces likes "" for the 
					// name of the default namespace.  Fix attributed 
					// to "Steven Murray" <smurray@ebt.com>.
					string prefix = (index = attrName.IndexOf(":", StringComparison.Ordinal)) < 0 ? "" : attrName.Substring(index + 1);

					if (!"".Equals(prefix))
					{
						fSerializer.namespaceAfterStartElement(prefix, attrValue);
					}
				}
			}

		}

		/// <summary>
		/// Serializes an ProcessingInstruction Node.
		/// </summary>
		/// <param name="node"> The ProcessingInstruction Node to serialize </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializePI(org.w3c.dom.ProcessingInstruction node) throws org.xml.sax.SAXException
		protected internal void serializePI(ProcessingInstruction node)
		{
			ProcessingInstruction pi = node;
			string name = pi.NodeName;

			// well-formed=true
			if ((fFeatures & WELLFORMED) != 0)
			{
				isPIWellFormed(node);
			}

			// apply the LSSerializer filter
			if (!applyFilter(node, NodeFilter.SHOW_PROCESSING_INSTRUCTION))
			{
				return;
			}

			// String data = pi.getData();
			if (name.Equals("xslt-next-is-raw"))
			{
				fNextIsRaw = true;
			}
			else
			{
				this.fSerializer.processingInstruction(name, pi.Data);
			}
		}

		/// <summary>
		/// Serializes an CDATASection Node.
		/// </summary>
		/// <param name="node"> The CDATASection Node to serialize </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeCDATASection(org.w3c.dom.CDATASection node) throws org.xml.sax.SAXException
		protected internal void serializeCDATASection(CDATASection node)
		{
			// well-formed=true
			if ((fFeatures & WELLFORMED) != 0)
			{
				isCDATASectionWellFormed(node);
			}

			// cdata-sections = true
			if ((fFeatures & CDATA) != 0)
			{

				// split-cdata-sections = true
				// Assumption: This parameter has an effect only when
				// cdata-sections=true
				// ToStream, by default splits cdata-sections. Hence the check
				// below.
				string nodeValue = node.NodeValue;
				int endIndex = nodeValue.IndexOf("]]>", StringComparison.Ordinal);
				if ((fFeatures & SPLITCDATA) != 0)
				{
					if (endIndex >= 0)
					{
						// The first node split will contain the ]] markers
						string relatedData = nodeValue.Substring(0, endIndex + 2);

						string msg = Utils.messages.createMessage(MsgKey.ER_CDATA_SECTIONS_SPLIT, null);

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_WARNING, msg, MsgKey.ER_CDATA_SECTIONS_SPLIT, null, relatedData, null));
						}
					}
				}
				else
				{
					if (endIndex >= 0)
					{
						// The first node split will contain the ]] markers 
						string relatedData = nodeValue.Substring(0, endIndex + 2);

						string msg = Utils.messages.createMessage(MsgKey.ER_CDATA_SECTIONS_SPLIT, null);

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_ERROR, msg, MsgKey.ER_CDATA_SECTIONS_SPLIT));
						}
						// Report an error and return.  What error???
						return;
					}
				}

				// apply the LSSerializer filter
				if (!applyFilter(node, NodeFilter.SHOW_CDATA_SECTION))
				{
					return;
				}

				// splits the cdata-section
				if (fLexicalHandler != null)
				{
					fLexicalHandler.startCDATA();
				}
				dispatachChars(node);
				if (fLexicalHandler != null)
				{
					fLexicalHandler.endCDATA();
				}
			}
			else
			{
				dispatachChars(node);
			}
		}

		/// <summary>
		/// Serializes an Text Node.
		/// </summary>
		/// <param name="node"> The Text Node to serialize </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeText(org.w3c.dom.Text node) throws org.xml.sax.SAXException
		protected internal void serializeText(Text node)
		{
			if (fNextIsRaw)
			{
				fNextIsRaw = false;
				fSerializer.processingInstruction(javax.xml.transform.Result.PI_DISABLE_OUTPUT_ESCAPING, "");
				dispatachChars(node);
				fSerializer.processingInstruction(javax.xml.transform.Result.PI_ENABLE_OUTPUT_ESCAPING, "");
			}
			else
			{
				// keep track of dispatch or not to avoid duplicaiton of filter code
				bool bDispatch = false;

				// well-formed=true
				if ((fFeatures & WELLFORMED) != 0)
				{
					isTextWellFormed(node);
				}

				// if the node is whitespace
				// Determine the Attr's type.
				bool isElementContentWhitespace = false;
				if (fIsLevel3DOM)
				{
					isElementContentWhitespace = node.ElementContentWhitespace;
				}

				if (isElementContentWhitespace)
				{
					// element-content-whitespace=true
					if ((fFeatures & ELEM_CONTENT_WHITESPACE) != 0)
					{
						bDispatch = true;
					}
				}
				else
				{
					bDispatch = true;
				}

				// apply the LSSerializer filter
				if (!applyFilter(node, NodeFilter.SHOW_TEXT))
				{
					return;
				}

				if (bDispatch)
				{
					dispatachChars(node);
				}
			}
		}

		/// <summary>
		/// Serializes an EntityReference Node.
		/// </summary>
		/// <param name="node"> The EntityReference Node to serialize </param>
		/// <param name="bStart"> Inicates if called from start or endNode  </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void serializeEntityReference(org.w3c.dom.EntityReference node, boolean bStart) throws org.xml.sax.SAXException
		protected internal void serializeEntityReference(EntityReference node, bool bStart)
		{
			if (bStart)
			{
				EntityReference eref = node;
				// entities=true
				if ((fFeatures & ENTITIES) != 0)
				{

					// perform well-formedness and other checking only if 
					// entities = true

					// well-formed=true
					if ((fFeatures & WELLFORMED) != 0)
					{
						isEntityReferneceWellFormed(node);
					}

					// check "unbound-prefix-in-entity-reference" [fatal] 
					// Raised if the configuration parameter "namespaces" is set to true
					if ((fFeatures & NAMESPACES) != 0)
					{
						checkUnboundPrefixInEntRef(node);
					}

					// The filter should not apply in this case, since the
					// EntityReference is not being expanded.
					// should we pass entity reference nodes to the filter???
				}

				if (fLexicalHandler != null)
				{

					// startEntity outputs only Text but not Element, Attr, Comment 
					// and PI child nodes.  It does so by setting the m_inEntityRef 
					// in ToStream and using this to decide if a node is to be 
					// serialized or not.
					fLexicalHandler.startEntity(eref.NodeName);
				}

			}
			else
			{
				EntityReference eref = node;
				// entities=true or false, 
				if (fLexicalHandler != null)
				{
					fLexicalHandler.endEntity(eref.NodeName);
				}
			}
		}


		// ***********************************************************************
		// Methods to check well-formedness
		// ***********************************************************************
		/// <summary>
		/// Taken from org.apache.xerces.dom.CoreDocumentImpl
		/// 
		/// Check the string against XML's definition of acceptable names for
		/// elements and attributes and so on using the XMLCharacterProperties
		/// utility class
		/// </summary>
		protected internal bool isXMLName(string s, bool xml11Version)
		{

			if (string.ReferenceEquals(s, null))
			{
				return false;
			}
			if (!xml11Version)
			{
				return XMLChar.isValidName(s);
			}
			else
			{
				return XML11Char.isXML11ValidName(s);
			}
		}

		/// <summary>
		/// Taken from org.apache.xerces.dom.CoreDocumentImpl
		/// 
		/// Checks if the given qualified name is legal with respect
		/// to the version of XML to which this document must conform.
		/// </summary>
		/// <param name="prefix"> prefix of qualified name </param>
		/// <param name="local"> local part of qualified name </param>
		protected internal bool isValidQName(string prefix, string local, bool xml11Version)
		{

			// check that both prefix and local part match NCName
			if (string.ReferenceEquals(local, null))
			{
				return false;
			}
			bool validNCName = false;

			if (!xml11Version)
			{
				validNCName = (string.ReferenceEquals(prefix, null) || XMLChar.isValidNCName(prefix)) && XMLChar.isValidNCName(local);
			}
			else
			{
				validNCName = (string.ReferenceEquals(prefix, null) || XML11Char.isXML11ValidNCName(prefix)) && XML11Char.isXML11ValidNCName(local);
			}

			return validNCName;
		}

		/// <summary>
		/// Checks if a XML character is well-formed
		/// </summary>
		/// <param name="characters"> A String of characters to be checked for Well-Formedness </param>
		/// <param name="refInvalidChar"> A reference to the character to be returned that was determined invalid.  </param>
		protected internal bool isWFXMLChar(string chardata, char? refInvalidChar)
		{
			if (string.ReferenceEquals(chardata, null) || (chardata.Length == 0))
			{
				return true;
			}

			char[] dataarray = chardata.ToCharArray();
			int datalength = dataarray.Length;

			// version of the document is XML 1.1
			if (fIsXMLVersion11)
			{
				//we need to check all characters as per production rules of XML11
				int i = 0;
				while (i < datalength)
				{
					if (XML11Char.isXML11Invalid(dataarray[i++]))
					{
						// check if this is a supplemental character
						char ch = dataarray[i - 1];
						if (XMLChar.isHighSurrogate(ch) && i < datalength)
						{
							char ch2 = dataarray[i++];
							if (XMLChar.isLowSurrogate(ch2) && XMLChar.isSupplemental(XMLChar.supplemental(ch, ch2)))
							{
								continue;
							}
						}
						// Reference to invalid character which is returned
						refInvalidChar = new char?(ch);
						return false;
					}
				}
			} // version of the document is XML 1.0
			else
			{
				// we need to check all characters as per production rules of XML 1.0
				int i = 0;
				while (i < datalength)
				{
					if (XMLChar.isInvalid(dataarray[i++]))
					{
						// check if this is a supplemental character
						char ch = dataarray[i - 1];
						if (XMLChar.isHighSurrogate(ch) && i < datalength)
						{
							char ch2 = dataarray[i++];
							if (XMLChar.isLowSurrogate(ch2) && XMLChar.isSupplemental(XMLChar.supplemental(ch, ch2)))
							{
								continue;
							}
						}
						// Reference to invalid character which is returned                    
						refInvalidChar = new char?(ch);
						return false;
					}
				}
			} // end-else fDocument.isXMLVersion()

			return true;
		} // isXMLCharWF

		/// <summary>
		/// Checks if a XML character is well-formed.  If there is a problem with
		/// the character a non-null Character is returned else null is returned.
		/// </summary>
		/// <param name="characters"> A String of characters to be checked for Well-Formedness </param>
		/// <returns> Character A reference to the character to be returned that was determined invalid.  </returns>
		protected internal char? isWFXMLChar(string chardata)
		{
			char? refInvalidChar;
			if (string.ReferenceEquals(chardata, null) || (chardata.Length == 0))
			{
				return null;
			}

			char[] dataarray = chardata.ToCharArray();
			int datalength = dataarray.Length;

			// version of the document is XML 1.1
			if (fIsXMLVersion11)
			{
				//we need to check all characters as per production rules of XML11
				int i = 0;
				while (i < datalength)
				{
					if (XML11Char.isXML11Invalid(dataarray[i++]))
					{
						// check if this is a supplemental character
						char ch = dataarray[i - 1];
						if (XMLChar.isHighSurrogate(ch) && i < datalength)
						{
							char ch2 = dataarray[i++];
							if (XMLChar.isLowSurrogate(ch2) && XMLChar.isSupplemental(XMLChar.supplemental(ch, ch2)))
							{
								continue;
							}
						}
						// Reference to invalid character which is returned
						refInvalidChar = new char?(ch);
						return refInvalidChar;
					}
				}
			} // version of the document is XML 1.0
			else
			{
				// we need to check all characters as per production rules of XML 1.0
				int i = 0;
				while (i < datalength)
				{
					if (XMLChar.isInvalid(dataarray[i++]))
					{
						// check if this is a supplemental character
						char ch = dataarray[i - 1];
						if (XMLChar.isHighSurrogate(ch) && i < datalength)
						{
							char ch2 = dataarray[i++];
							if (XMLChar.isLowSurrogate(ch2) && XMLChar.isSupplemental(XMLChar.supplemental(ch, ch2)))
							{
								continue;
							}
						}
						// Reference to invalid character which is returned                    
						refInvalidChar = new char?(ch);
						return refInvalidChar;
					}
				}
			} // end-else fDocument.isXMLVersion()

			return null;
		} // isXMLCharWF

		/// <summary>
		/// Checks if a comment node is well-formed
		/// </summary>
		/// <param name="data"> The contents of the comment node </param>
		/// <returns> a boolean indiacating if the comment is well-formed or not. </returns>
		protected internal void isCommentWellFormed(string data)
		{
			if (string.ReferenceEquals(data, null) || (data.Length == 0))
			{
				return;
			}

			char[] dataarray = data.ToCharArray();
			int datalength = dataarray.Length;

			// version of the document is XML 1.1
			if (fIsXMLVersion11)
			{
				// we need to check all chracters as per production rules of XML11
				int i = 0;
				while (i < datalength)
				{
					char c = dataarray[i++];
					if (XML11Char.isXML11Invalid(c))
					{
						// check if this is a supplemental character
						if (XMLChar.isHighSurrogate(c) && i < datalength)
						{
							char c2 = dataarray[i++];
							if (XMLChar.isLowSurrogate(c2) && XMLChar.isSupplemental(XMLChar.supplemental(c, c2)))
							{
								continue;
							}
						}
						string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_COMMENT, new object[] {new char?(c)});

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
						}
					}
					else if (c == '-' && i < datalength && dataarray[i] == '-')
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_WF_DASH_IN_COMMENT, null);

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
						}
					}
				}
			} // version of the document is XML 1.0
			else
			{
				// we need to check all chracters as per production rules of XML 1.0
				int i = 0;
				while (i < datalength)
				{
					char c = dataarray[i++];
					if (XMLChar.isInvalid(c))
					{
						// check if this is a supplemental character
						if (XMLChar.isHighSurrogate(c) && i < datalength)
						{
							char c2 = dataarray[i++];
							if (XMLChar.isLowSurrogate(c2) && XMLChar.isSupplemental(XMLChar.supplemental(c, c2)))
							{
								continue;
							}
						}
						string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_COMMENT, new object[] {new char?(c)});

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
						}
					}
					else if (c == '-' && i < datalength && dataarray[i] == '-')
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_WF_DASH_IN_COMMENT, null);

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
						}
					}
				}
			}
			return;
		}

		/// <summary>
		/// Checks if an element node is well-formed, by checking its Name for well-formedness.
		/// </summary>
		/// <param name="data"> The contents of the comment node </param>
		/// <returns> a boolean indiacating if the comment is well-formed or not. </returns>
		protected internal void isElementWellFormed(Node node)
		{
			bool isNameWF = false;
			if ((fFeatures & NAMESPACES) != 0)
			{
				isNameWF = isValidQName(node.Prefix, node.LocalName, fIsXMLVersion11);
			}
			else
			{
				isNameWF = isXMLName(node.NodeName, fIsXMLVersion11);
			}

			if (!isNameWF)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, new object[] {"Element", node.NodeName});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, null, null, null));
				}
			}
		}

		/// <summary>
		/// Checks if an attr node is well-formed, by checking it's Name and value
		/// for well-formedness.
		/// </summary>
		/// <param name="data"> The contents of the comment node </param>
		/// <returns> a boolean indiacating if the comment is well-formed or not. </returns>
		protected internal void isAttributeWellFormed(Node node)
		{
			bool isNameWF = false;
			if ((fFeatures & NAMESPACES) != 0)
			{
				isNameWF = isValidQName(node.Prefix, node.LocalName, fIsXMLVersion11);
			}
			else
			{
				isNameWF = isXMLName(node.NodeName, fIsXMLVersion11);
			}

			if (!isNameWF)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, new object[] {"Attr", node.NodeName});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, null, null, null));
				}
			}

			// Check the Attr's node value
			// WFC: No < in Attribute Values
			string value = node.NodeValue;
			if (value.IndexOf('<') >= 0)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_LT_IN_ATTVAL, new object[] {((Attr) node).OwnerElement.NodeName, node.NodeName});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_LT_IN_ATTVAL, null, null, null));
				}
			}

			// we need to loop through the children of attr nodes and check their values for
			// well-formedness  
			NodeList children = node.ChildNodes;
			for (int i = 0; i < children.Length; i++)
			{
				Node child = children.item(i);
				// An attribute node with no text or entity ref child for example
				// doc.createAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns:ns");
				// followes by
				// element.setAttributeNodeNS(attribute);
				// can potentially lead to this situation.  If the attribute
				// was a prefix Namespace attribute declaration then then DOM Core 
				// should have some exception defined for this.
				if (child == null)
				{
					// we should probably report an error
					continue;
				}
				switch (child.NodeType)
				{
					case Node.TEXT_NODE :
						isTextWellFormed((Text) child);
						break;
					case Node.ENTITY_REFERENCE_NODE :
						isEntityReferneceWellFormed((EntityReference) child);
						break;
					default :
				break;
				}
			}

			// TODO:
			// WFC: Check if the attribute prefix is bound to 
			// http://www.w3.org/2000/xmlns/  

			// WFC: Unique Att Spec
			// Perhaps pass a seen boolean value to this method.  serializeAttList will determine
			// if the attr was seen before.
		}

		/// <summary>
		/// Checks if a PI node is well-formed, by checking it's Name and data
		/// for well-formedness.
		/// </summary>
		/// <param name="data"> The contents of the comment node </param>
		protected internal void isPIWellFormed(ProcessingInstruction node)
		{
			// Is the PI Target a valid XML name
			if (!isXMLName(node.NodeName, fIsXMLVersion11))
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, new object[] {"ProcessingInstruction", node.Target});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, null, null, null));
				}
			}

			// Does the PI Data carry valid XML characters

			// REVISIT: Should we check if the PI DATA contains a ?> ???
			char? invalidChar = isWFXMLChar(node.Data);
			if (invalidChar != null)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_PI, new object[] {((int)char.GetNumericValue(invalidChar.Value)).ToString("x")});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
				}
			}
		}

		/// <summary>
		/// Checks if an CDATASection node is well-formed, by checking it's data
		/// for well-formedness.  Note that the presence of a CDATA termination mark
		/// in the contents of a CDATASection is handled by the parameter 
		/// spli-cdata-sections
		/// </summary>
		/// <param name="data"> The contents of the comment node </param>
		protected internal void isCDATASectionWellFormed(CDATASection node)
		{
			// Does the data valid XML character data        
			char? invalidChar = isWFXMLChar(node.Data);
			//if (!isWFXMLChar(node.getData(), invalidChar)) {
			if (invalidChar != null)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_CDATA, new object[] {((int)char.GetNumericValue(invalidChar.Value)).ToString("x")});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
				}
			}
		}

		/// <summary>
		/// Checks if an Text node is well-formed, by checking if it contains invalid
		/// XML characters.
		/// </summary>
		/// <param name="data"> The contents of the comment node </param>
		protected internal void isTextWellFormed(Text node)
		{
			// Does the data valid XML character data        
			char? invalidChar = isWFXMLChar(node.Data);
			if (invalidChar != null)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_TEXT, new object[] {((int)char.GetNumericValue(invalidChar.Value)).ToString("x")});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER, null, null, null));
				}
			}
		}

		/// <summary>
		/// Checks if an EntityRefernece node is well-formed, by checking it's node name.  Then depending
		/// on whether it is referenced in Element content or in an Attr Node, checks if the EntityReference
		/// references an unparsed entity or a external entity and if so throws raises the 
		/// appropriate well-formedness error.  
		/// </summary>
		/// <param name="data"> The contents of the comment node
		/// @parent The parent of the EntityReference Node </param>
		protected internal void isEntityReferneceWellFormed(EntityReference node)
		{
			// Is the EntityReference name a valid XML name
			if (!isXMLName(node.NodeName, fIsXMLVersion11))
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, new object[] {"EntityReference", node.NodeName});

				if (fErrorHandler != null)
				{
					fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, null, null, null));
				}
			}

			// determine the parent node
			Node parent = node.ParentNode;

			// Traverse the declared entities and check if the nodeName and namespaceURI
			// of the EntityReference matches an Entity.  If so, check the if the notationName
			// is not null, if so, report an error.
			DocumentType docType = node.OwnerDocument.Doctype;
			if (docType != null)
			{
				NamedNodeMap entities = docType.Entities;
				for (int i = 0; i < entities.Length; i++)
				{
					Entity ent = (Entity) entities.item(i);

					string nodeName = node.NodeName == null ? "" : node.NodeName;
					string nodeNamespaceURI = node.NamespaceURI == null ? "" : node.NamespaceURI;
					string entName = ent.NodeName == null ? "" : ent.NodeName;
					string entNamespaceURI = ent.NamespaceURI == null ? "" : ent.NamespaceURI;
					// If referenced in Element content
					// WFC: Parsed Entity
					if (parent.NodeType == Node.ELEMENT_NODE)
					{
						if (entNamespaceURI.Equals(nodeNamespaceURI) && entName.Equals(nodeName))
						{

							if (ent.NotationName != null)
							{
								string msg = Utils.messages.createMessage(MsgKey.ER_WF_REF_TO_UNPARSED_ENT, new object[] {node.NodeName});

								if (fErrorHandler != null)
								{
									fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_REF_TO_UNPARSED_ENT, null, null, null));
								}
							}
						}
					} // end if WFC: Parsed Entity

					// If referenced in an Attr value
					// WFC: No External Entity References
					if (parent.NodeType == Node.ATTRIBUTE_NODE)
					{
						if (entNamespaceURI.Equals(nodeNamespaceURI) && entName.Equals(nodeName))
						{

							if (ent.PublicId != null || ent.SystemId != null || ent.NotationName != null)
							{
								string msg = Utils.messages.createMessage(MsgKey.ER_WF_REF_TO_EXTERNAL_ENT, new object[] {node.NodeName});

								if (fErrorHandler != null)
								{
									fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_WF_REF_TO_EXTERNAL_ENT, null, null, null));
								}
							}
						}
					} //end if WFC: No External Entity References
				}
			}
		} // isEntityReferneceWellFormed

		/// <summary>
		/// If the configuration parameter "namespaces" is set to true, this methods
		/// checks if an entity whose replacement text contains unbound namespace 
		/// prefixes is referenced in a location where there are no bindings for 
		/// the namespace prefixes and if so raises a LSException with the error-type
		/// "unbound-prefix-in-entity-reference"   
		/// </summary>
		/// <param name="Node">, The EntityReference nodes whose children are to be checked </param>
		protected internal void checkUnboundPrefixInEntRef(Node node)
		{
			Node child, next;
			for (child = node.FirstChild; child != null; child = next)
			{
				next = child.NextSibling;

				if (child.NodeType == Node.ELEMENT_NODE)
				{

					//If a NamespaceURI is not declared for the current
					//node's prefix, raise a fatal error.
					string prefix = child.Prefix;
					if (!string.ReferenceEquals(prefix, null) && string.ReferenceEquals(fNSBinder.getURI(prefix), null))
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_ELEM_UNBOUND_PREFIX_IN_ENTREF, new object[] {node.NodeName, child.NodeName, prefix});

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_ELEM_UNBOUND_PREFIX_IN_ENTREF, null, null, null));
						}
					}

					NamedNodeMap attrs = child.Attributes;

					for (int i = 0; i < attrs.Length; i++)
					{
						string attrPrefix = attrs.item(i).Prefix;
						if (!string.ReferenceEquals(attrPrefix, null) && string.ReferenceEquals(fNSBinder.getURI(attrPrefix), null))
						{
							string msg = Utils.messages.createMessage(MsgKey.ER_ATTR_UNBOUND_PREFIX_IN_ENTREF, new object[] {node.NodeName, child.NodeName, attrs.item(i)});

							if (fErrorHandler != null)
							{
								fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_ATTR_UNBOUND_PREFIX_IN_ENTREF, null, null, null));
							}
						}
					}
				}

				if (child.hasChildNodes())
				{
					checkUnboundPrefixInEntRef(child);
				}
			}
		}

		// ***********************************************************************
		// Namespace normalization
		// ***********************************************************************
		/// <summary>
		/// Records local namespace declarations, to be used for normalization later
		/// </summary>
		/// <param name="Node">, The element node, whose namespace declarations are to be recorded </param>
		protected internal void recordLocalNSDecl(Node node)
		{
			NamedNodeMap atts = ((Element) node).Attributes;
			int length = atts.Length;

			for (int i = 0; i < length; i++)
			{
				Node attr = atts.item(i);

				string localName = attr.LocalName;
				string attrPrefix = attr.Prefix;
				string attrValue = attr.NodeValue;
				string attrNS = attr.NamespaceURI;

				localName = string.ReferenceEquals(localName, null) || XMLNS_PREFIX.Equals(localName) ? "" : localName;
				attrPrefix = string.ReferenceEquals(attrPrefix, null) ? "" : attrPrefix;
				attrValue = string.ReferenceEquals(attrValue, null) ? "" : attrValue;
				attrNS = string.ReferenceEquals(attrNS, null) ? "" : attrNS;

				// check if attribute is a namespace decl
				if (XMLNS_URI.Equals(attrNS))
				{

					// No prefix may be bound to http://www.w3.org/2000/xmlns/.
					if (XMLNS_URI.Equals(attrValue))
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_NS_PREFIX_CANNOT_BE_BOUND, new object[] {attrPrefix, XMLNS_URI});

						if (fErrorHandler != null)
						{
							fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_ERROR, msg, MsgKey.ER_NS_PREFIX_CANNOT_BE_BOUND, null, null, null));
						}
					}
					else
					{
						// store the namespace-declaration
						if (XMLNS_PREFIX.Equals(attrPrefix))
						{
							// record valid decl
							if (attrValue.Length != 0)
							{
								fNSBinder.declarePrefix(localName, attrValue);
							}
							else
							{
								// Error; xmlns:prefix=""
							}
						}
						else
						{ // xmlns
							// empty prefix is always bound ("" or some string)
							fNSBinder.declarePrefix("", attrValue);
						}
					}

				}
			}
		}

		/// <summary>
		/// Fixes an element's namespace
		/// </summary>
		/// <param name="Node">, The element node, whose namespace is to be fixed </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void fixupElementNS(org.w3c.dom.Node node) throws org.xml.sax.SAXException
		protected internal void fixupElementNS(Node node)
		{
			string namespaceURI = ((Element) node).NamespaceURI;
			string prefix = ((Element) node).Prefix;
			string localName = ((Element) node).LocalName;

			if (!string.ReferenceEquals(namespaceURI, null))
			{
				//if ( Element's prefix/namespace pair (or default namespace,
				// if no prefix) are within the scope of a binding )
				prefix = string.ReferenceEquals(prefix, null) ? "" : prefix;
				string inScopeNamespaceURI = fNSBinder.getURI(prefix);

				if ((!string.ReferenceEquals(inScopeNamespaceURI, null) && inScopeNamespaceURI.Equals(namespaceURI)))
				{
					// do nothing, declaration in scope is inherited

				}
				else
				{
					// Create a local namespace declaration attr for this namespace,
					// with Element's current prefix (or a default namespace, if
					// no prefix). If there's a conflicting local declaration
					// already present, change its value to use this namespace.

					// Add the xmlns declaration attribute
					//fNSBinder.pushNamespace(prefix, namespaceURI, fElementDepth);
					if ((fFeatures & NAMESPACEDECLS) != 0)
					{
						if ("".Equals(prefix) || "".Equals(namespaceURI))
						{
							((Element)node).setAttributeNS(XMLNS_URI, XMLNS_PREFIX, namespaceURI);
						}
						else
						{
							((Element)node).setAttributeNS(XMLNS_URI, XMLNS_PREFIX + ":" + prefix, namespaceURI);
						}
					}
					fLocalNSBinder.declarePrefix(prefix, namespaceURI);
					fNSBinder.declarePrefix(prefix, namespaceURI);

				}
			}
			else
			{
				// Element has no namespace
				// DOM Level 1
				if (string.ReferenceEquals(localName, null) || "".Equals(localName))
				{
					//  DOM Level 1 node!
					string msg = Utils.messages.createMessage(MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, new object[] {node.NodeName});

					if (fErrorHandler != null)
					{
						fErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_ERROR, msg, MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, null, null, null));
					}
				}
				else
				{
					namespaceURI = fNSBinder.getURI("");
					if (!string.ReferenceEquals(namespaceURI, null) && namespaceURI.Length > 0)
					{
						((Element)node).setAttributeNS(XMLNS_URI, XMLNS_PREFIX, "");
						fLocalNSBinder.declarePrefix("", "");
						fNSBinder.declarePrefix("", "");
					}
				}
			}
		}
		/// <summary>
		/// This table is a quick lookup of a property key (String) to the integer that
		/// is the bit to flip in the fFeatures field, so the integers should have
		/// values 1,2,4,8,16...
		/// 
		/// </summary>
		private static readonly Hashtable s_propKeys = new Hashtable();
		static DOM3TreeWalker()
		{

			// Initialize the mappings of property keys to bit values (Integer objects)
			// or mappings to a String object "", which indicates we are interested
			// in the property, but it does not have a simple bit value to flip

			// cdata-sections
			int i = CDATA;
			int? val = new int?(i);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CDATA_SECTIONS] = val;

			// comments
			int i1 = COMMENTS;
			val = new int?(i1);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_COMMENTS] = val;

			// element-content-whitespace
			int i2 = ELEM_CONTENT_WHITESPACE;
			val = new int?(i2);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE] = val;
			int i3 = ENTITIES;

			// entities
			val = new int?(i3);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ENTITIES] = val;

			// namespaces
			int i4 = NAMESPACES;
			val = new int?(i4);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACES] = val;

			// namespace-declarations
			int i5 = NAMESPACEDECLS;
			val = new int?(i5);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACE_DECLARATIONS] = val;

			// split-cdata-sections
			int i6 = SPLITCDATA;
			val = new int?(i6);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_SPLIT_CDATA] = val;

			// discard-default-content	
			int i7 = WELLFORMED;
			val = new int?(i7);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_WELLFORMED] = val;

			// discard-default-content	
			int i8 = DISCARDDEFAULT;
			val = new int?(i8);
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DISCARD_DEFAULT_CONTENT] = val;

			// We are interested in these properties, but they don't have a simple
			// bit value to deal with.
			s_propKeys[DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_FORMAT_PRETTY_PRINT] = "";
			s_propKeys[DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL] = "";
			s_propKeys[DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.S_XML_VERSION] = "";
			s_propKeys[DOMConstants.S_XSL_OUTPUT_ENCODING] = "";
			s_propKeys[DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES] = "";
		}

		/// <summary>
		/// Initializes fFeatures based on the DOMConfiguration Parameters set.
		/// </summary>
		/// <param name="properties"> DOMConfiguraiton properties that were set and which are
		/// to be used while serializing the DOM.  </param>
		protected internal void initProperties(Properties properties)
		{

			for (System.Collections.IEnumerator keys = properties.keys(); keys.MoveNext();)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String key = (String) keys.Current;
				string key = (string) keys.Current;

				// caonical-form
				// Other features will be enabled or disabled when this is set to true or false.

				// error-handler; set via the constructor

				// infoset
				// Other features will be enabled or disabled when this is set to true

				// A quick lookup for the given set of properties (cdata-sections ...)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object iobj = s_propKeys.get(key);
				object iobj = s_propKeys[key];
				if (iobj != null)
				{
					if (iobj is int?)
					{
						// Dealing with a property that has a simple bit value that
						// we need to set

						// cdata-sections			
						// comments
						// element-content-whitespace
						// entities
						// namespaces
						// namespace-declarations
						// split-cdata-sections
						// well-formed
						// discard-default-content
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int BITFLAG = ((Nullable<int>) iobj).intValue();
						int BITFLAG = ((int?) iobj).Value;
						if ((properties.getProperty(key).EndsWith("yes")))
						{
							fFeatures = fFeatures | BITFLAG;
						}
						else
						{
							fFeatures = fFeatures & ~BITFLAG;
						}
					}
					else
					{
						// We are interested in the property, but it is not
						// a simple bit that we need to set.

						if ((DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_FORMAT_PRETTY_PRINT).Equals(key))
						{
							// format-pretty-print; set internally on the serializers via xsl:output properties in LSSerializer
							if ((properties.getProperty(key).EndsWith("yes")))
							{
								fSerializer.Indent = true;
								fSerializer.IndentAmount = 3;
							}
							else
							{
								fSerializer.Indent = false;
							}
						}
						else if ((DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL).Equals(key))
						{
							// omit-xml-declaration; set internally on the serializers via xsl:output properties in LSSerializer
							if ((properties.getProperty(key).EndsWith("yes")))
							{
								fSerializer.OmitXMLDeclaration = true;
							}
							else
							{
								fSerializer.OmitXMLDeclaration = false;
							}
						}
						else if ((DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.S_XML_VERSION).Equals(key))
						{
							// Retreive the value of the XML Version attribute via the xml-version
							string version = properties.getProperty(key);
							if ("1.1".Equals(version))
							{
								fIsXMLVersion11 = true;
								fSerializer.Version = version;
							}
							else
							{
								fSerializer.Version = "1.0";
							}
						}
						else if ((DOMConstants.S_XSL_OUTPUT_ENCODING).Equals(key))
						{
							// Retreive the value of the XML Encoding attribute
							string encoding = properties.getProperty(key);
							if (!string.ReferenceEquals(encoding, null))
							{
								fSerializer.Encoding = encoding;
							}
						}
						else if ((DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES).Equals(key))
						{
							// Preserve entity references in the document
							if ((properties.getProperty(key).EndsWith("yes")))
							{
								fSerializer.DTDEntityExpansion = false;
							}
							else
							{
								fSerializer.DTDEntityExpansion = true;
							}
						}
						else
						{
							// We shouldn't get here, ever, now what?
						}
					}
				}
			}
			// Set the newLine character to use
			if (!string.ReferenceEquals(fNewLine, null))
			{
				fSerializer.setOutputProperty(OutputPropertiesFactory.S_KEY_LINE_SEPARATOR, fNewLine);
			}
		}

	} //TreeWalker

}