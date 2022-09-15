using System;
using System.IO;

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
 * $Id: DTMDocument.java 468638 2006-10-28 06:52:06Z minchau $
 */

namespace org.apache.xalan.lib.sql
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMDefaultBaseIterators = org.apache.xml.dtm.@ref.DTMDefaultBaseIterators;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using StringBufferPool = org.apache.xml.utils.StringBufferPool;
	using SuballocatedIntVector = org.apache.xml.utils.SuballocatedIntVector;
	using XMLString = org.apache.xml.utils.XMLString;

	using Node = org.w3c.dom.Node;

	using ContentHandler = org.xml.sax.ContentHandler;
	using DTDHandler = org.xml.sax.DTDHandler;
	using EntityResolver = org.xml.sax.EntityResolver;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using DeclHandler = org.xml.sax.ext.DeclHandler;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// The SQL Document is the main controlling class the executesa SQL Query
	/// </summary>
	public class DTMDocument : DTMDefaultBaseIterators
	{

	  public interface CharacterNodeHandler
	  {
		/// <param name="node">
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(org.w3c.dom.Node node)throws org.xml.sax.SAXException;
		void characters(Node node);
	  }

	  private bool DEBUG = false;

	  protected internal const string S_NAMESPACE = "http://xml.apache.org/xalan/SQLExtension";

	  protected internal const string S_ATTRIB_NOT_SUPPORTED = "Not Supported";
	  protected internal const string S_ISTRUE = "true";
	  protected internal const string S_ISFALSE = "false";

	  protected internal const string S_DOCUMENT = "#root";
	  protected internal const string S_TEXT_NODE = "#text";
	  protected internal const string S_ELEMENT_NODE = "#element";

	  protected internal int m_Document_TypeID = 0;
	  protected internal int m_TextNode_TypeID = 0;


	  /// <summary>
	  /// Store the SQL Data in this growable array
	  /// </summary>
	  protected internal ObjectArray m_ObjectArray = new ObjectArray();

	  /// <summary>
	  /// For each element node, there can be zero or more attributes. If Attributes
	  /// are assigned, the first attribute for that element will be use here.
	  /// Subsequent elements will use the m_nextsib, m_prevsib array. The sibling
	  /// arrays are not meeant to hold indexes to attribute information but as
	  /// long as there is not direct connection back into the main DTM tree
	  /// we should be OK.
	  /// </summary>
	  protected internal SuballocatedIntVector m_attribute;

	  /// <summary>
	  /// The Document Index will most likely be 0, but we will reference it
	  /// by variable in case that paradigm falls through.
	  /// </summary>
	  protected internal int m_DocumentIdx;


	  /// <param name="mgr"> </param>
	  /// <param name="ident"> </param>
	  public DTMDocument(DTMManager mgr, int ident) : base(mgr, null, ident, null, mgr.XMLStringFactory, true)
	  {

		m_attribute = new SuballocatedIntVector(DEFAULT_BLOCKSIZE);
	  }

	  /// <summary>
	  /// A common routine that allocates an Object from the Object Array.
	  /// One of the common bugs in this code was to allocate an Object and
	  /// not incerment m_size, using this method will assure that function. </summary>
	  /// <param name="o">
	  ///  </param>
	  private int allocateNodeObject(object o)
	  {
		// Need to keep this counter going even if we don't use it.
		m_size++;
		return m_ObjectArray.append(o);
	  }

	  /// <param name="o"> </param>
	  /// <param name="level"> </param>
	  /// <param name="extendedType"> </param>
	  /// <param name="parent"> </param>
	  /// <param name="prevsib">
	  ///  </param>
	  protected internal virtual int addElementWithData(object o, int level, int extendedType, int parent, int prevsib)
	  {
		int elementIdx = addElement(level,extendedType,parent,prevsib);

		int data = allocateNodeObject(o);
		m_firstch.setElementAt(data,elementIdx);

		m_exptype.setElementAt(m_TextNode_TypeID, data);
		// m_level.setElementAt((byte)(level), data);
		m_parent.setElementAt(elementIdx, data);

		m_prevsib.setElementAt(DTM.NULL, data);
		m_nextsib.setElementAt(DTM.NULL, data);
		m_attribute.setElementAt(DTM.NULL, data);
		m_firstch.setElementAt(DTM.NULL, data);

		return elementIdx;
	  }

	  /// <param name="level"> </param>
	  /// <param name="extendedType"> </param>
	  /// <param name="parent"> </param>
	  /// <param name="prevsib">
	  ///  </param>
	  protected internal virtual int addElement(int level, int extendedType, int parent, int prevsib)
	  {
		int node = DTM.NULL;

		try
		{
		  // Add the Node and adjust its Extended Type
		  node = allocateNodeObject(S_ELEMENT_NODE);

		  m_exptype.setElementAt(extendedType, node);
		  m_nextsib.setElementAt(DTM.NULL, node);
		  m_prevsib.setElementAt(prevsib, node);

		  m_parent.setElementAt(parent, node);
		  m_firstch.setElementAt(DTM.NULL, node);
		  // m_level.setElementAt((byte)level, node);
		  m_attribute.setElementAt(DTM.NULL, node);

		  if (prevsib != DTM.NULL)
		  {
			// If the previous sibling is already assigned, then we are
			// inserting a value into the chain.
			if (m_nextsib.elementAt(prevsib) != DTM.NULL)
			{
			  m_nextsib.setElementAt(m_nextsib.elementAt(prevsib), node);
			}

			// Tell the proevious sibling that they have a new bother/sister.
			m_nextsib.setElementAt(node, prevsib);
		  }

		   // So if we have a valid parent and the new node ended up being first
		  // in the list, i.e. no prevsib, then set the new node up as the
		  // first child of the parent. Since we chained the node in the list,
		  // there should be no reason to worry about the current first child
		  // of the parent node.
		  if ((parent != DTM.NULL) && (m_prevsib.elementAt(node) == DTM.NULL))
		  {
			m_firstch.setElementAt(node, parent);
		  }
		}
		catch (Exception e)
		{
		  error("Error in addElement: " + e.Message);
		}

		return node;
	  }

	  /// <summary>
	  /// Link an attribute to a node, if the node already has one or more
	  /// attributes assigned, then just link this one to the attribute list.
	  /// The first attribute is attached to the Parent Node (pnode) through the
	  /// m_attribute array, subsequent attributes are linked through the
	  /// m_prevsib, m_nextsib arrays. </summary>
	  /// <param name="o"> </param>
	  /// <param name="extendedType"> </param>
	  /// <param name="pnode">
	  ///  </param>
	  protected internal virtual int addAttributeToNode(object o, int extendedType, int pnode)
	  {
		int attrib = DTM.NULL;
		//int prevsib = DTM.NULL;
		int lastattrib = DTM.NULL;
		// int value = DTM.NULL;

		try
		{
		  // Add the Node and adjust its Extended Type
		  attrib = allocateNodeObject(o);

		  m_attribute.setElementAt(DTM.NULL, attrib);
		  m_exptype.setElementAt(extendedType, attrib);
		  // m_level.setElementAt((byte)0, attrib);

		  // Clear the sibling references
		  m_nextsib.setElementAt(DTM.NULL, attrib);
		  m_prevsib.setElementAt(DTM.NULL,attrib);
		  // Set the parent, although the was we are using attributes
		  // in the SQL extension this reference will more than likly
		  // be wrong
		  m_parent.setElementAt(pnode, attrib);
		  m_firstch.setElementAt(DTM.NULL, attrib);

		  if (m_attribute.elementAt(pnode) != DTM.NULL)
		  {
			// OK, we already have an attribute assigned to this
			// Node, Insert us into the head of the list.
			lastattrib = m_attribute.elementAt(pnode);
			m_nextsib.setElementAt(lastattrib, attrib);
			m_prevsib.setElementAt(attrib, lastattrib);
		  }
		  // Okay set the new attribute up as the first attribute
		  // for the node.
		  m_attribute.setElementAt(attrib, pnode);
		}
		catch (Exception e)
		{
		  error("Error in addAttributeToNode: " + e.Message);
		}

		return attrib;
	  }

	  /// <summary>
	  /// Allow two nodes to share the same set of attributes. There may be some
	  /// problems because the parent of any attribute will be the original node
	  /// they were assigned to. Need to see how the attribute walker works, then
	  /// we should be able to fake it out. </summary>
	  /// <param name="toNode"> </param>
	  /// <param name="fromNode">
	  ///  </param>
	  protected internal virtual void cloneAttributeFromNode(int toNode, int fromNode)
	  {
	   try
	   {
		  if (m_attribute.elementAt(toNode) != DTM.NULL)
		  {
			error("Cloneing Attributes, where from Node already had addtibures assigned");
		  }

		  m_attribute.setElementAt(m_attribute.elementAt(fromNode), toNode);
	   }
		catch (Exception)
		{
		  error("Cloning attributes");
		}
	  }


	  /// <param name="parm1">
	  ///  </param>
	  public override int getFirstAttribute(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getFirstAttribute(" + parm1 + ")");
		}
		int nodeIdx = makeNodeIdentity(parm1);
		if (nodeIdx != DTM.NULL)
		{
		  int attribIdx = m_attribute.elementAt(nodeIdx);
		  return makeNodeHandle(attribIdx);
		}
		else
		{
			return DTM.NULL;
		}
	  }

	 /// <param name="parm1">
	 ///   </param>
	  public override string getNodeValue(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNodeValue(" + parm1 + ")");
		}
		try
		{
		  object o = m_ObjectArray.getAt(makeNodeIdentity(parm1));
		  if (o != null && o != S_ELEMENT_NODE)
		  {
			return o.ToString();
		  }
		  else
		  {
			return "";
		  }
		}
		catch (Exception)
		{
		  error("Getting String Value");
		  return null;
		}
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
		int nodeIdx = makeNodeIdentity(nodeHandle);
		if (DEBUG)
		{
			Console.WriteLine("getStringValue(" + nodeIdx + ")");
		}

		  object o = m_ObjectArray.getAt(nodeIdx);
		if (o == S_ELEMENT_NODE)
		{
			FastStringBuffer buf = StringBufferPool.get();
			string s;

			try
			{
			  getNodeData(nodeIdx, buf);

			  s = (buf.length() > 0) ? buf.ToString() : "";
			}
			finally
			{
			  StringBufferPool.free(buf);
			}

			return m_xstrf.newstr(s);
		}
		  else if (o != null)
		  {
			return m_xstrf.newstr(o.ToString());
		  }
		else
		{
		  return (m_xstrf.emptystr());
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
	  /// %REVIEW% Actually, since this method operates on the DOM side of the
	  /// fence rather than the DTM side, it SHOULDN'T do
	  /// any special handling. The DOM does what the DOM does; if you want
	  /// DTM-level abstractions, use DTM-level methods.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nodeIdx"> Index of node whose subtree is to be walked, gathering the
	  /// contents of all Text or CDATASection nodes. </param>
	  /// <param name="buf"> FastStringBuffer into which the contents of the text
	  /// nodes are to be concatenated. </param>
	  protected internal virtual void getNodeData(int nodeIdx, FastStringBuffer buf)
	  {
		for (int child = _firstch(nodeIdx) ; child != DTM.NULL ; child = _nextsib(child))
		{
		  object o = m_ObjectArray.getAt(child);
		  if (o == S_ELEMENT_NODE)
		  {
			getNodeData(child, buf);
		  }
		  else if (o != null)
		  {
			buf.append(o.ToString());
		  }
		}
	  }




	  /// <param name="parm1">
	  ///  </param>
	  public override int getNextAttribute(int parm1)
	  {
		int nodeIdx = makeNodeIdentity(parm1);
		if (DEBUG)
		{
			Console.WriteLine("getNextAttribute(" + nodeIdx + ")");
		}
		if (nodeIdx != DTM.NULL)
		{
			return makeNodeHandle(m_nextsib.elementAt(nodeIdx));
		}
		else
		{
			return DTM.NULL;
		}
	  }


	  /// 
	  protected internal override int NumberOfNodes
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getNumberOfNodes()");
			}
			return m_size;
		  }
	  }

	  /// 
	  protected internal override bool nextNode()
	  {
		if (DEBUG)
		{
			Console.WriteLine("nextNode()");
		}
		return false;
	  }


	  /// <summary>
	  /// The Expanded Name table holds all of our Node names. The Base class
	  /// will add the common element types, need to call this function from
	  /// the derived class.
	  /// 
	  /// </summary>
	  protected internal virtual void createExpandedNameTable()
	  {
		m_Document_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_DOCUMENT, DTM.DOCUMENT_NODE);

		m_TextNode_TypeID = m_expandedNameTable.getExpandedTypeID(S_NAMESPACE, S_TEXT_NODE, DTM.TEXT_NODE);
	  }


	  /// 
	  public virtual void dumpDTM()
	  {
		try
		{
	//      File f = new File("DTMDump"+((Object)this).hashCode()+".txt");
		  File f = new File("DTMDump.txt");
		  Console.Error.WriteLine("Dumping... " + f.getAbsolutePath());
		  PrintStream ps = new PrintStream(new FileStream(f, FileMode.Create, FileAccess.Write));

		  while (nextNode())
		  {
		  }

		  int nRecords = m_size;

		  ps.println("Total nodes: " + nRecords);

		  for (int i = 0; i < nRecords; i++)
		  {
			ps.println("=========== " + i + " ===========");
			ps.println("NodeName: " + getNodeName(makeNodeHandle(i)));
			ps.println("NodeNameX: " + getNodeNameX(makeNodeHandle(i)));
			ps.println("LocalName: " + getLocalName(makeNodeHandle(i)));
			ps.println("NamespaceURI: " + getNamespaceURI(makeNodeHandle(i)));
			ps.println("Prefix: " + getPrefix(makeNodeHandle(i)));

			int exTypeID = getExpandedTypeID(makeNodeHandle(i));

			ps.println("Expanded Type ID: " + Convert.ToString(exTypeID, 16));

			int type = getNodeType(makeNodeHandle(i));
			string typestring;

			switch (type)
			{
			case DTM.ATTRIBUTE_NODE :
			  typestring = "ATTRIBUTE_NODE";
			  break;
			case DTM.CDATA_SECTION_NODE :
			  typestring = "CDATA_SECTION_NODE";
			  break;
			case DTM.COMMENT_NODE :
			  typestring = "COMMENT_NODE";
			  break;
			case DTM.DOCUMENT_FRAGMENT_NODE :
			  typestring = "DOCUMENT_FRAGMENT_NODE";
			  break;
			case DTM.DOCUMENT_NODE :
			  typestring = "DOCUMENT_NODE";
			  break;
			case DTM.DOCUMENT_TYPE_NODE :
			  typestring = "DOCUMENT_NODE";
			  break;
			case DTM.ELEMENT_NODE :
			  typestring = "ELEMENT_NODE";
			  break;
			case DTM.ENTITY_NODE :
			  typestring = "ENTITY_NODE";
			  break;
			case DTM.ENTITY_REFERENCE_NODE :
			  typestring = "ENTITY_REFERENCE_NODE";
			  break;
			case DTM.NAMESPACE_NODE :
			  typestring = "NAMESPACE_NODE";
			  break;
			case DTM.NOTATION_NODE :
			  typestring = "NOTATION_NODE";
			  break;
			case DTM.NULL :
			  typestring = "NULL";
			  break;
			case DTM.PROCESSING_INSTRUCTION_NODE :
			  typestring = "PROCESSING_INSTRUCTION_NODE";
			  break;
			case DTM.TEXT_NODE :
			  typestring = "TEXT_NODE";
			  break;
			default :
			  typestring = "Unknown!";
			  break;
			}

			ps.println("Type: " + typestring);

			int firstChild = _firstch(i);

			if (DTM.NULL == firstChild)
			{
			  ps.println("First child: DTM.NULL");
			}
			else if (NOTPROCESSED == firstChild)
			{
			  ps.println("First child: NOTPROCESSED");
			}
			else
			{
			  ps.println("First child: " + firstChild);
			}

			int prevSibling = _prevsib(i);

			if (DTM.NULL == prevSibling)
			{
			  ps.println("Prev sibling: DTM.NULL");
			}
			else if (NOTPROCESSED == prevSibling)
			{
			  ps.println("Prev sibling: NOTPROCESSED");
			}
			else
			{
			  ps.println("Prev sibling: " + prevSibling);
			}

			int nextSibling = _nextsib(i);

			if (DTM.NULL == nextSibling)
			{
			  ps.println("Next sibling: DTM.NULL");
			}
			else if (NOTPROCESSED == nextSibling)
			{
			  ps.println("Next sibling: NOTPROCESSED");
			}
			else
			{
			  ps.println("Next sibling: " + nextSibling);
			}

			int parent = _parent(i);

			if (DTM.NULL == parent)
			{
			  ps.println("Parent: DTM.NULL");
			}
			else if (NOTPROCESSED == parent)
			{
			  ps.println("Parent: NOTPROCESSED");
			}
			else
			{
			  ps.println("Parent: " + parent);
			}

			int level = _level(i);

			ps.println("Level: " + level);
			ps.println("Node Value: " + getNodeValue(i));
			ps.println("String Value: " + getStringValue(i));

			ps.println("First Attribute Node: " + m_attribute.elementAt(i));
		  }

		}
		catch (IOException ioe)
		{
		  ioe.printStackTrace(System.err);
		  throw new Exception(ioe.Message);
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
	  /// </para>
	  /// </summary>
	  /// <param name="node"> Node whose subtree is to be walked, gathering the
	  /// contents of all Text or CDATASection nodes. </param>
	  /// <param name="ch"> </param>
	  /// <param name="depth">
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected static void dispatchNodeData(org.w3c.dom.Node node, org.xml.sax.ContentHandler ch, int depth)throws org.xml.sax.SAXException
	  protected internal static void dispatchNodeData(Node node, ContentHandler ch, int depth)
	  {

		switch (node.getNodeType())
		{
		case Node.DOCUMENT_FRAGMENT_NODE :
		case Node.DOCUMENT_NODE :
		case Node.ELEMENT_NODE :
		{
		  for (Node child = node.getFirstChild(); null != child; child = child.getNextSibling())
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
		  string str = node.getNodeValue();
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

	  /// <summary>
	  ///****************************************************************** </summary>
	  /// <summary>
	  ///****************************************************************** </summary>
	  /// <summary>
	  ///***************** End of Functions we Wrote ********************** </summary>
	  /// <summary>
	  ///****************************************************************** </summary>
	  /// <summary>
	  ///****************************************************************** </summary>


	  /// <summary>
	  /// For the moment all the run time properties are ignored by this
	  /// class. </summary>
	  /// <param name="property"> a <code>String</code> value </param>
	  /// <param name="value"> an <code>Object</code> value
	  ///  </param>
	  public override void setProperty(string property, object value)
	  {
	  }

	  /// <summary>
	  /// No source information is available for DOM2DTM, so return
	  /// <code>null</code> here. </summary>
	  /// <param name="node"> an <code>int</code> value </param>
	  /// <returns> null </returns>
	  public override SourceLocator getSourceLocatorFor(int node)
	  {
		return null;
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int getNextNodeIdentity(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNextNodeIdenty(" + parm1 + ")");
		}
		return DTM.NULL;
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2"> </param>
	  /// <param name="parm3">
	  ///  </param>
	  public override int getAttributeNode(int parm1, string parm2, string parm3)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("getAttributeNode(" + parm1 + "," + parm2 + "," + parm3 + ")");
		}
		return DTM.NULL;
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getLocalName(int parm1)
	  {
	//    int exID = this.getExpandedTypeID( makeNodeIdentity(parm1) );
		  int exID = getExpandedTypeID(parm1);

		if (DEBUG)
		{
		  DEBUG = false;
		  Console.Write("getLocalName(" + parm1 + ") -> ");
		  Console.WriteLine("..." + getLocalNameFromExpandedNameID(exID));
		  DEBUG = true;
		}

		return getLocalNameFromExpandedNameID(exID);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getNodeName(int parm1)
	  {
	//    int exID = getExpandedTypeID( makeNodeIdentity(parm1) );
		int exID = getExpandedTypeID(parm1);
		if (DEBUG)
		{
		  DEBUG = false;
		  Console.Write("getLocalName(" + parm1 + ") -> ");
		  Console.WriteLine("..." + getLocalNameFromExpandedNameID(exID));
		  DEBUG = true;
		}
		return getLocalNameFromExpandedNameID(exID);
	  }

	   /// <param name="parm1">
	   ///  </param>
	  public override bool isAttributeSpecified(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("isAttributeSpecified(" + parm1 + ")");
		}
		return false;
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getUnparsedEntityURI(string parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getUnparsedEntityURI(" + parm1 + ")");
		}
		return "";
	  }

	  /// 
	  public override DTDHandler DTDHandler
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getDTDHandler()");
			}
			return null;
		  }
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getPrefix(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getPrefix(" + parm1 + ")");
		}
		return "";
	  }

	  /// 
	  public override EntityResolver EntityResolver
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getEntityResolver()");
			}
			return null;
		  }
	  }

	  /// 
	  public override string DocumentTypeDeclarationPublicIdentifier
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("get_DTD_PubId()");
			}
			return "";
		  }
	  }

	  /// 
	  public override LexicalHandler LexicalHandler
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getLexicalHandler()");
			}
			return null;
		  }
	  }
	  /// 
	  public override bool needsTwoThreads()
	  {
		if (DEBUG)
		{
			Console.WriteLine("needsTwoThreads()");
		}
		return false;
	  }

	  /// 
	  public override ContentHandler ContentHandler
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getContentHandler()");
			}
			return null;
		  }
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchToEvents(int parm1, org.xml.sax.ContentHandler parm2)throws org.xml.sax.SAXException
	  public override void dispatchToEvents(int parm1, ContentHandler parm2)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("dispathcToEvents(" + parm1 + "," + parm2 + ")");
		}
		return;
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getNamespaceURI(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNamespaceURI(" + parm1 + ")");
		}
		return "";
	  }

	  /// <param name="nodeHandle"> </param>
	  /// <param name="ch"> </param>
	  /// <param name="normalize">
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, boolean normalize)throws org.xml.sax.SAXException
	  public override void dispatchCharactersEvents(int nodeHandle, ContentHandler ch, bool normalize)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("dispatchCharacterEvents(" + nodeHandle + "," + ch + "," + normalize + ")");
		}

		if (normalize)
		{
		  XMLString str = getStringValue(nodeHandle);
		  str = str.fixWhiteSpace(true, true, false);
		  str.dispatchCharactersEvents(ch);
		}
		else
		{
		  Node node = getNode(nodeHandle);
		  dispatchNodeData(node, ch, 0);
		}
	  }

	  /// <summary>
	  /// Event overriding for Debug
	  /// 
	  /// </summary>
	  public override bool supportsPreStripping()
	  {
		if (DEBUG)
		{
			Console.WriteLine("supportsPreStripping()");
		}
		return base.supportsPreStripping();
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int _exptype(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_exptype(" + parm1 + ")");
		}
		return base._exptype(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override SuballocatedIntVector findNamespaceContext(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("SuballocatedIntVector(" + parm1 + ")");
		}
		return base.findNamespaceContext(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int _prevsib(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_prevsib(" + parm1 + ")");
		}
		return base._prevsib(parm1);
	  }


	  /// <param name="parm1">
	  ///  </param>
	  protected internal override short _type(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_type(" + parm1 + ")");
		}
		return base._type(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override Node getNode(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNode(" + parm1 + ")");
		}
		return base.getNode(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getPreviousSibling(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getPrevSib(" + parm1 + ")");
		}
		return base.getPreviousSibling(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getDocumentStandalone(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getDOcStandAlone(" + parm1 + ")");
		}
		return base.getDocumentStandalone(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getNodeNameX(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNodeNameX(" + parm1 + ")");
		}
		//return super.getNodeNameX( parm1);
		return getNodeName(parm1);

	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  public override void setFeature(string parm1, bool parm2)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("setFeature(" + parm1 + "," + parm2 + ")");
		}
		base.setFeature(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int _parent(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_parent(" + parm1 + ")");
		}
		return base._parent(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  protected internal override void indexNode(int parm1, int parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("indexNode(" + parm1 + "," + parm2 + ")");
		}
		base.indexNode(parm1, parm2);
	  }

	  /// 
	  protected internal override bool ShouldStripWhitespace
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getShouldStripWS()");
			}
			return base.ShouldStripWhitespace;
		  }
		  set
		  {
			if (DEBUG)
			{
				Console.WriteLine("set_ShouldStripWS(" + value + ")");
			}
			base.ShouldStripWhitespace = value;
		  }
	  }

	  /// 
	  protected internal override void popShouldStripWhitespace()
	  {
		if (DEBUG)
		{
			Console.WriteLine("popShouldStripWS()");
		}
		base.popShouldStripWhitespace();
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  public override bool isNodeAfter(int parm1, int parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("isNodeAfter(" + parm1 + "," + parm2 + ")");
		}
		return base.isNodeAfter(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getNamespaceType(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNamespaceType(" + parm1 + ")");
		}
		return base.getNamespaceType(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int _level(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_level(" + parm1 + ")");
		}
		return base._level(parm1);
	  }


	  /// <param name="parm1">
	  ///  </param>
	  protected internal override void pushShouldStripWhitespace(bool parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("push_ShouldStripWS(" + parm1 + ")");
		}
		base.pushShouldStripWhitespace(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getDocumentVersion(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getDocVer(" + parm1 + ")");
		}
		return base.getDocumentVersion(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  public override bool isSupported(string parm1, string parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("isSupported(" + parm1 + "," + parm2 + ")");
		}
		return base.isSupported(parm1, parm2);
	  }




	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  protected internal override void ensureSizeOfIndex(int parm1, int parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("ensureSizeOfIndex(" + parm1 + "," + parm2 + ")");
		}
		base.ensureSizeOfIndex(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal virtual void ensureSize(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("ensureSize(" + parm1 + ")");
		}

		// IntVectors in DTMDefaultBase are now self-sizing, and ensureSize()
		// is being dropped.
		//super.ensureSize( parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getDocumentEncoding(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getDocumentEncoding(" + parm1 + ")");
		}
		return base.getDocumentEncoding(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2"> </param>
	  /// <param name="parm3">
	  ///  </param>
	  public override void appendChild(int parm1, bool parm2, bool parm3)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("appendChild(" + parm1 + "," + parm2 + "," + parm3 + ")");
		}
		base.appendChild(parm1, parm2, parm3);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override short getLevel(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getLevel(" + parm1 + ")");
		}
		return base.getLevel(parm1);
	  }

	  /// 
	  public override string DocumentBaseURI
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getDocBaseURI()");
			}
			return base.DocumentBaseURI;
		  }
		  set
		  {
			if (DEBUG)
			{
				Console.WriteLine("setDocBaseURI()");
			}
			base.DocumentBaseURI = value;
		  }
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2"> </param>
	  /// <param name="parm3">
	  ///  </param>
	  public override int getNextNamespaceNode(int parm1, int parm2, bool parm3)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("getNextNamesapceNode(" + parm1 + "," + parm2 + "," + parm3 + ")");
		}
		return base.getNextNamespaceNode(parm1, parm2, parm3);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override void appendTextChild(string parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("appendTextChild(" + parm1 + ")");
		}
		base.appendTextChild(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2"> </param>
	  /// <param name="parm3"> </param>
	  /// <param name="parm4">
	  ///  </param>
	  protected internal override int findGTE(int[] parm1, int parm2, int parm3, int parm4)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("findGTE(" + parm1 + "," + parm2 + "," + parm3 + ")");
		}
		return base.findGTE(parm1, parm2, parm3, parm4);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  public override int getFirstNamespaceNode(int parm1, bool parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getFirstNamespaceNode()");
		}
		return base.getFirstNamespaceNode(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getStringValueChunkCount(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getStringChunkCount(" + parm1 + ")");
		}
		return base.getStringValueChunkCount(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getLastChild(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getLastChild(" + parm1 + ")");
		}
		return base.getLastChild(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override bool hasChildNodes(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("hasChildNodes(" + parm1 + ")");
		}
		return base.hasChildNodes(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override short getNodeType(int parm1)
	  {
		if (DEBUG)
		{
		  DEBUG = false;
		  Console.Write("getNodeType(" + parm1 + ") ");
		  int exID = getExpandedTypeID(parm1);
		  string name = getLocalNameFromExpandedNameID(exID);
		  Console.WriteLine(".. Node name [" + name + "]" + "[" + getNodeType(parm1) + "]");

		  DEBUG = true;
		}

		return base.getNodeType(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override bool isCharacterElementContentWhitespace(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("isCharacterElementContentWhitespace(" + parm1 + ")");
		}
		return base.isCharacterElementContentWhitespace(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getFirstChild(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getFirstChild(" + parm1 + ")");
		}
		return base.getFirstChild(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getDocumentSystemIdentifier(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getDocSysID(" + parm1 + ")");
		}
		return base.getDocumentSystemIdentifier(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  protected internal override void declareNamespaceInContext(int parm1, int parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("declareNamespaceContext(" + parm1 + "," + parm2 + ")");
		}
		base.declareNamespaceInContext(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getNamespaceFromExpandedNameID(int parm1)
	  {
		if (DEBUG)
		{
		  DEBUG = false;
		  Console.Write("getNamespaceFromExpandedNameID(" + parm1 + ")");
		  Console.WriteLine("..." + base.getNamespaceFromExpandedNameID(parm1));
		  DEBUG = true;
		}
		return base.getNamespaceFromExpandedNameID(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override string getLocalNameFromExpandedNameID(int parm1)
	  {
		if (DEBUG)
		{
		  DEBUG = false;
		  Console.Write("getLocalNameFromExpandedNameID(" + parm1 + ")");
		  Console.WriteLine("..." + base.getLocalNameFromExpandedNameID(parm1));
		  DEBUG = true;
		}
		return base.getLocalNameFromExpandedNameID(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getExpandedTypeID(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getExpandedTypeID(" + parm1 + ")");
		}
		return base.getExpandedTypeID(parm1);
	  }

	  /// 
	  public override int Document
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getDocument()");
			}
			return base.Document;
		  }
	  }


	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  protected internal override int findInSortedSuballocatedIntVector(SuballocatedIntVector parm1, int parm2)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("findInSortedSubAlloctedVector(" + parm1 + "," + parm2 + ")");
		}
		return base.findInSortedSuballocatedIntVector(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override bool isDocumentAllDeclarationsProcessed(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("isDocumentAllDeclProc(" + parm1 + ")");
		}
		return base.isDocumentAllDeclarationsProcessed(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override void error(string parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("error(" + parm1 + ")");
		}
		base.error(parm1);
	  }


	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int _firstch(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_firstch(" + parm1 + ")");
		}
		return base._firstch(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getOwnerDocument(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getOwnerDoc(" + parm1 + ")");
		}
		return base.getOwnerDocument(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  protected internal override int _nextsib(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("_nextSib(" + parm1 + ")");
		}
		return base._nextsib(parm1);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getNextSibling(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getNextSibling(" + parm1 + ")");
		}
		return base.getNextSibling(parm1);
	  }


	  /// 
	  public override bool DocumentAllDeclarationsProcessed
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getDocAllDeclProc()");
			}
			return base.DocumentAllDeclarationsProcessed;
		  }
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override int getParent(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getParent(" + parm1 + ")");
		}
		return base.getParent(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2"> </param>
	  /// <param name="parm3">
	  ///  </param>
	  public override int getExpandedTypeID(string parm1, string parm2, int parm3)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getExpandedTypeID()");
		}
		return base.getExpandedTypeID(parm1, parm2, parm3);
	  }


	  /// <param name="parm1"> </param>
	  /// <param name="parm2"> </param>
	  /// <param name="parm3">
	  ///  </param>
	  public override char[] getStringValueChunk(int parm1, int parm2, int[] parm3)
	  {
		if (DEBUG)
		{
		  Console.WriteLine("getStringChunkValue(" + parm1 + "," + parm2 + ")");
		}
		return base.getStringValueChunk(parm1, parm2, parm3);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override DTMAxisTraverser getAxisTraverser(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getAxixTraverser(" + parm1 + ")");
		}
		return base.getAxisTraverser(parm1);
	  }

	  /// <param name="parm1"> </param>
	  /// <param name="parm2">
	  ///  </param>
	  public override DTMAxisIterator getTypedAxisIterator(int parm1, int parm2)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getTypedAxisIterator(" + parm1 + "," + parm2 + ")");
		}
		return base.getTypedAxisIterator(parm1, parm2);
	  }

	  /// <param name="parm1">
	  ///  </param>
	  public override DTMAxisIterator getAxisIterator(int parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getAxisIterator(" + parm1 + ")");
		}
		return base.getAxisIterator(parm1);
	  }
	  /// <param name="parm1">
	  ///  </param>
	  public override int getElementById(string parm1)
	  {
		if (DEBUG)
		{
			Console.WriteLine("getElementByID(" + parm1 + ")");
		}
		return DTM.NULL;
	  }

	  /// 
	  public override DeclHandler DeclHandler
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getDeclHandler()");
			}
			return null;
		  }
	  }

	  /// 
	  public override ErrorHandler ErrorHandler
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("getErrorHandler()");
			}
			return null;
		  }
	  }

	  /// 
	  public override string DocumentTypeDeclarationSystemIdentifier
	  {
		  get
		  {
			if (DEBUG)
			{
				Console.WriteLine("get_DTD-SID()");
			}
			return null;
		  }
	  }


	}

}