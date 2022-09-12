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
 * $Id: SimpleResultTreeImpl.java 468651 2006-10-28 07:04:25Z minchau $
 */
namespace org.apache.xalan.xsltc.dom
{

	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;

	using DTM = org.apache.xml.dtm.DTM;
	using Axis = org.apache.xml.dtm.Axis;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;
	using DTMManagerDefault = org.apache.xml.dtm.@ref.DTMManagerDefault;
	using EmptySerializer = org.apache.xml.serializer.EmptySerializer;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XMLString = org.apache.xml.utils.XMLString;
	using XMLStringDefault = org.apache.xml.utils.XMLStringDefault;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This class represents a light-weight DOM model for simple result tree fragment(RTF).
	/// A simple RTF is an RTF that has only one Text node. The Text node can be produced by a
	/// combination of Text, xsl:value-of and xsl:number instructions. It can also be produced
	/// by a control structure (xsl:if or xsl:choose) whose body is pure Text.
	/// <para>
	/// A SimpleResultTreeImpl has only two nodes, i.e. the ROOT node and its Text child. All DOM
	/// interfaces are overridden with this in mind. For example, the getStringValue() interface
	/// returns the value of the Text node. This class receives the character data from the 
	/// characters() interface.
	/// </para>
	/// <para>
	/// This class implements DOM and SerializationHandler. It also implements the DTM interface
	/// for support in MultiDOM. The nested iterators (SimpleIterator and SingletonIterator) are
	/// used to support the nodeset() extension function.
	/// </para>
	/// </summary>
	public class SimpleResultTreeImpl : EmptySerializer, DOM, DTM
	{

		/// <summary>
		/// The SimpleIterator is designed to support the nodeset() extension function. It has
		/// a traversal direction parameter. The DOWN direction is used for child and descendant
		/// axes, while the UP direction is used for parent and ancestor axes.
		/// 
		/// This iterator only handles two nodes (RTF_ROOT and RTF_TEXT). If the type is set,
		/// it will also match the node type with the given type.
		/// </summary>
		public sealed class SimpleIterator : DTMAxisIteratorBase
		{
			private readonly SimpleResultTreeImpl outerInstance;

			internal const int DIRECTION_UP = 0;
			internal const int DIRECTION_DOWN = 1;
			internal const int org;

			// The direction of traversal (default to DOWN).
			// DOWN is for child and descendant. UP is for parent and ancestor.
			internal int _direction = DIRECTION_DOWN;

			internal int _type = org.apache.xalan.xsltc.DOM_Fields.NO_TYPE;
			internal int _currentNode;

			public SimpleIterator(SimpleResultTreeImpl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public SimpleIterator(SimpleResultTreeImpl outerInstance, int direction)
			{
				this.outerInstance = outerInstance;
				_direction = direction;
			}

			public SimpleIterator(SimpleResultTreeImpl outerInstance, int direction, int type)
			{
				this.outerInstance = outerInstance;
				 _direction = direction;
				 _type = type;
			}

			public override int next()
			{
				// Increase the node ID for down traversal. Also match the node type
				// if the type is given.
				if (_direction == DIRECTION_DOWN)
				{
					while (_currentNode < NUMBER_OF_NODES)
					{
						if (_type != org.apache.xalan.xsltc.DOM_Fields.NO_TYPE)
						{
							if ((_currentNode == RTF_ROOT && _type == org.apache.xml.dtm.DTM_Fields.ROOT_NODE) || (_currentNode == RTF_TEXT && _type == org.apache.xml.dtm.DTM_Fields.TEXT_NODE))
							{
								return returnNode(outerInstance.getNodeHandle(_currentNode++));
							}
							else
							{
								_currentNode++;
							}
						}
						else
						{
							return returnNode(outerInstance.getNodeHandle(_currentNode++));
						}
					}

					return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
				}
				// Decrease the node ID for up traversal.
				else
				{
					while (_currentNode >= 0)
					{
						if (_type != org.apache.xalan.xsltc.DOM_Fields.NO_TYPE)
						{
							if ((_currentNode == RTF_ROOT && _type == org.apache.xml.dtm.DTM_Fields.ROOT_NODE) || (_currentNode == RTF_TEXT && _type == org.apache.xml.dtm.DTM_Fields.TEXT_NODE))
							{
								return returnNode(outerInstance.getNodeHandle(_currentNode--));
							}
							else
							{
								_currentNode--;
							}
						}
						else
						{
							return returnNode(outerInstance.getNodeHandle(_currentNode--));
						}
					}

					return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
				}
			}

			public override DTMAxisIterator setStartNode(int nodeHandle)
			{
				int nodeID = outerInstance.getNodeIdent(nodeHandle);
				_startNode = nodeID;

				// Increase the node ID by 1 if self is not included.
				if (!_includeSelf && nodeID != org.apache.xml.dtm.DTM_Fields.NULL)
				{
					if (_direction == DIRECTION_DOWN)
					{
						nodeID++;
					}
					else if (_direction == DIRECTION_UP)
					{
						nodeID--;
					}
				}

				_currentNode = nodeID;
				return this;
			}

			public override void setMark()
			{
				_markedNode = _currentNode;
			}

			public override void gotoMark()
			{
				_currentNode = _markedNode;
			}

		} // END of SimpleIterator

		/// <summary>
		/// The SingletonIterator is used for the self axis.
		/// </summary>
		public sealed class SingletonIterator : DTMAxisIteratorBase
		{
			private readonly SimpleResultTreeImpl outerInstance;

			internal const int org;
			internal int _type = org.apache.xalan.xsltc.DOM_Fields.NO_TYPE;
			internal int _currentNode;

			public SingletonIterator(SimpleResultTreeImpl outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public SingletonIterator(SimpleResultTreeImpl outerInstance, int type)
			{
				this.outerInstance = outerInstance;
				_type = type;
			}

			public override void setMark()
			{
				_markedNode = _currentNode;
			}

			public override void gotoMark()
			{
				_currentNode = _markedNode;
			}

			public override DTMAxisIterator setStartNode(int nodeHandle)
			{
				_currentNode = _startNode = outerInstance.getNodeIdent(nodeHandle);
				return this;
			}

			public override int next()
			{
				if (_currentNode == org.apache.xml.dtm.DTMAxisIterator_Fields.END)
				{
					return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
				}

				_currentNode = org.apache.xml.dtm.DTMAxisIterator_Fields.END;

				if (_type != org.apache.xalan.xsltc.DOM_Fields.NO_TYPE)
				{
					if ((_currentNode == RTF_ROOT && _type == org.apache.xml.dtm.DTM_Fields.ROOT_NODE) || (_currentNode == RTF_TEXT && _type == org.apache.xml.dtm.DTM_Fields.TEXT_NODE))
					{
						return outerInstance.getNodeHandle(_currentNode);
					}
				}
				else
				{
					return outerInstance.getNodeHandle(_currentNode);
				}

				return org.apache.xml.dtm.DTMAxisIterator_Fields.END;
			}

		} // END of SingletonIterator

		// empty iterator to be returned when there are no children
		private static readonly DTMAxisIterator EMPTY_ITERATOR = new DTMAxisIteratorBaseAnonymousInnerClass();

		private class DTMAxisIteratorBaseAnonymousInnerClass : DTMAxisIteratorBase
		{
			public DTMAxisIteratorBaseAnonymousInnerClass()
			{
			}

			public override DTMAxisIterator reset()
			{
				return this;
			}
			public override DTMAxisIterator setStartNode(int node)
			{
				return this;
			}
			public override int next()
			{
				return org.apache.xml.dtm.DTM_Fields.NULL;
			}
			public override void setMark()
			{
			}
			public override void gotoMark()
			{
			}
			public override int Last
			{
				get
				{
					return 0;
				}
			}
			public override int Position
			{
				get
				{
					return 0;
				}
			}
			public override DTMAxisIterator cloneIterator()
			{
				return this;
			}
			public override bool Restartable
			{
				set
				{
				}
			}
		}


		// The root node id of the simple RTF
		public const int RTF_ROOT = 0;

		// The Text node id of the simple RTF (simple RTF has only one Text node).
		public const int RTF_TEXT = 1;

		// The number of nodes.
		public const int NUMBER_OF_NODES = 2;

		// Document URI index, which increases by 1 at each getDocumentURI() call.
		private static int _documentURIIndex = 0;

		// Constant for empty String
		private const string EMPTY_STR = "";

		// The String value of the Text node.
		// This is set at the endDocument() call.
		private string _text;

		// The array of Text items, which is built by the characters() call.
		// The characters() interface can be called multiple times. Each character item
		// can have different escape settings.
		protected internal string[] _textArray;

		// The DTMManager
		protected internal XSLTCDTMManager _dtmManager;

		// Number of character items
		protected internal int _size = 0;

		// The document ID
		private int _documentID;

		// A BitArray, each bit holding the escape setting for a character item.
		private BitArray _dontEscape = null;

		// The current escape setting
		private bool _escaping = true;

		// Create a SimpleResultTreeImpl from a DTMManager and a document ID.
		public SimpleResultTreeImpl(XSLTCDTMManager dtmManager, int documentID)
		{
			_dtmManager = dtmManager;
			_documentID = documentID;
			_textArray = new string[4];
		}

		public virtual DTMManagerDefault DTMManager
		{
			get
			{
				return _dtmManager;
			}
		}

		// Return the document ID
		public virtual int Document
		{
			get
			{
				return _documentID;
			}
		}

		// Return the String value of the RTF
		public virtual string StringValue
		{
			get
			{
				return _text;
			}
		}

		public virtual DTMAxisIterator Iterator
		{
			get
			{
				return new SingletonIterator(this, Document);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getChildren(final int node)
		public virtual DTMAxisIterator getChildren(int node)
		{
			return (new SimpleIterator(this)).setStartNode(node);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getTypedChildren(final int type)
		public virtual DTMAxisIterator getTypedChildren(int type)
		{
			return new SimpleIterator(this, SimpleIterator.DIRECTION_DOWN, type);
		}

		// Return the axis iterator for a given axis.
		// The SimpleIterator is used for the child, descendant, parent and ancestor axes.
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getAxisIterator(final int axis)
		public virtual DTMAxisIterator getAxisIterator(int axis)
		{
			switch (axis)
			{
				case Axis.CHILD:
				case Axis.DESCENDANT:
					return new SimpleIterator(this, SimpleIterator.DIRECTION_DOWN);
				case Axis.PARENT:
				case Axis.ANCESTOR:
					return new SimpleIterator(this, SimpleIterator.DIRECTION_UP);
				case Axis.ANCESTORORSELF:
					return (new SimpleIterator(this, SimpleIterator.DIRECTION_UP)).includeSelf();
				case Axis.DESCENDANTORSELF:
					return (new SimpleIterator(this, SimpleIterator.DIRECTION_DOWN)).includeSelf();
				case Axis.SELF:
					return new SingletonIterator(this);
				default:
					return EMPTY_ITERATOR;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getTypedAxisIterator(final int axis, final int type)
		public virtual DTMAxisIterator getTypedAxisIterator(int axis, int type)
		{
			switch (axis)
			{
				case Axis.CHILD:
				case Axis.DESCENDANT:
					return new SimpleIterator(this, SimpleIterator.DIRECTION_DOWN, type);
				case Axis.PARENT:
				case Axis.ANCESTOR:
					return new SimpleIterator(this, SimpleIterator.DIRECTION_UP, type);
				case Axis.ANCESTORORSELF:
					return (new SimpleIterator(this, SimpleIterator.DIRECTION_UP, type)).includeSelf();
				case Axis.DESCENDANTORSELF:
					return (new SimpleIterator(this, SimpleIterator.DIRECTION_DOWN, type)).includeSelf();
				case Axis.SELF:
					return new SingletonIterator(this, type);
				default:
					return EMPTY_ITERATOR;
			}
		}

		// %REVISIT% Can this one ever get used?
		public virtual DTMAxisIterator getNthDescendant(int node, int n, bool includeself)
		{
			return null;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getNamespaceAxisIterator(final int axis, final int ns)
		public virtual DTMAxisIterator getNamespaceAxisIterator(int axis, int ns)
		{
			return null;
		}

		// %REVISIT% Can this one ever get used?
		public virtual DTMAxisIterator getNodeValueIterator(DTMAxisIterator iter, int returnType, string value, bool op)
		{
			return null;
		}

		public virtual DTMAxisIterator orderNodes(DTMAxisIterator source, int node)
		{
			return source;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getNodeName(final int node)
		public virtual string getNodeName(int node)
		{
			if (getNodeIdent(node) == RTF_TEXT)
			{
				return "#text";
			}
			else
			{
				return EMPTY_STR;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getNodeNameX(final int node)
		public virtual string getNodeNameX(int node)
		{
			return EMPTY_STR;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getNamespaceName(final int node)
		public virtual string getNamespaceName(int node)
		{
			return EMPTY_STR;
		}

		// Return the expanded type id of a given node
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getExpandedTypeID(final int nodeHandle)
		public virtual int getExpandedTypeID(int nodeHandle)
		{
			int nodeID = getNodeIdent(nodeHandle);
			if (nodeID == RTF_TEXT)
			{
				return org.apache.xml.dtm.DTM_Fields.TEXT_NODE;
			}
			else if (nodeID == RTF_ROOT)
			{
				return org.apache.xml.dtm.DTM_Fields.ROOT_NODE;
			}
			else
			{
				return org.apache.xml.dtm.DTM_Fields.NULL;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getNamespaceType(final int node)
		public virtual int getNamespaceType(int node)
		{
			return 0;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParent(final int nodeHandle)
		public virtual int getParent(int nodeHandle)
		{
			int nodeID = getNodeIdent(nodeHandle);
			return (nodeID == RTF_TEXT) ? getNodeHandle(RTF_ROOT) : org.apache.xml.dtm.DTM_Fields.NULL;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getAttributeNode(final int gType, final int element)
		public virtual int getAttributeNode(int gType, int element)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getStringValueX(final int nodeHandle)
		public virtual string getStringValueX(int nodeHandle)
		{
			int nodeID = getNodeIdent(nodeHandle);
			if (nodeID == RTF_ROOT || nodeID == RTF_TEXT)
			{
				return _text;
			}
			else
			{
				return EMPTY_STR;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public virtual void copy(int node, SerializationHandler handler)
		{
			characters(node, handler);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copy(org.apache.xml.dtm.DTMAxisIterator nodes, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public virtual void copy(DTMAxisIterator nodes, SerializationHandler handler)
		{
			int node;
			while ((node = nodes.next()) != org.apache.xml.dtm.DTM_Fields.NULL)
			{
				copy(node, handler);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String shallowCopy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public virtual string shallowCopy(int node, SerializationHandler handler)
		{
			characters(node, handler);
			return null;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean lessThan(final int node1, final int node2)
		public virtual bool lessThan(int node1, int node2)
		{
			if (node1 == org.apache.xml.dtm.DTM_Fields.NULL)
			{
				return false;
			}
			else if (node2 == org.apache.xml.dtm.DTM_Fields.NULL)
			{
				return true;
			}
			else
			{
				return (node1 < node2);
			}
		}

		/// <summary>
		/// Dispatch the character content of a node to an output handler.
		/// 
		/// The escape setting should be taken care of when outputting to
		/// a handler.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public virtual void characters(int node, SerializationHandler handler)
		{
			int nodeID = getNodeIdent(node);
			if (nodeID == RTF_ROOT || nodeID == RTF_TEXT)
			{
				bool escapeBit = false;
				bool oldEscapeSetting = false;

				try
				{
					for (int i = 0; i < _size; i++)
					{

						if (_dontEscape != null)
						{
							escapeBit = _dontEscape.getBit(i);
							if (escapeBit)
							{
								oldEscapeSetting = handler.setEscaping(false);
							}
						}

						handler.characters(_textArray[i]);

						if (escapeBit)
						{
							handler.Escaping = oldEscapeSetting;
						}
					}
				}
				catch (SAXException e)
				{
					throw new TransletException(e);
				}
			}
		}

		// %REVISIT% Can the makeNode() and makeNodeList() interfaces ever get used?
		public virtual Node makeNode(int index)
		{
			return null;
		}

		public virtual Node makeNode(DTMAxisIterator iter)
		{
			return null;
		}

		public virtual NodeList makeNodeList(int index)
		{
			return null;
		}

		public virtual NodeList makeNodeList(DTMAxisIterator iter)
		{
			return null;
		}

		public virtual string getLanguage(int node)
		{
			return null;
		}

		public virtual int Size
		{
			get
			{
				return 2;
			}
		}

		public virtual string getDocumentURI(int node)
		{
			return "simple_rtf" + _documentURIIndex++;
		}

		public virtual StripFilter Filter
		{
			set
			{
			}
		}

		public virtual void setupMapping(string[] names, string[] uris, int[] types, string[] namespaces)
		{
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean isElement(final int node)
		public virtual bool isElement(int node)
		{
			return false;
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean isAttribute(final int node)
		public virtual bool isAttribute(int node)
		{
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String lookupNamespace(int node, String prefix) throws org.apache.xalan.xsltc.TransletException
		public virtual string lookupNamespace(int node, string prefix)
		{
			return null;
		}

		/// <summary>
		/// Return the node identity from a node handle.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getNodeIdent(final int nodehandle)
		public virtual int getNodeIdent(int nodehandle)
		{
			return (nodehandle != org.apache.xml.dtm.DTM_Fields.NULL) ? (nodehandle - _documentID) : org.apache.xml.dtm.DTM_Fields.NULL;
		}

		/// <summary>
		/// Return the node handle from a node identity.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getNodeHandle(final int nodeId)
		public virtual int getNodeHandle(int nodeId)
		{
			return (nodeId != org.apache.xml.dtm.DTM_Fields.NULL) ? (nodeId + _documentID) : org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual DOM getResultTreeFrag(int initialSize, int rtfType)
		{
			return null;
		}

		public virtual DOM getResultTreeFrag(int initialSize, int rtfType, bool addToManager)
		{
			return null;
		}

		public virtual SerializationHandler OutputDomBuilder
		{
			get
			{
				return this;
			}
		}

		public virtual int getNSType(int node)
		{
			return 0;
		}

		public virtual string getUnparsedEntityURI(string name)
		{
			return null;
		}

		public virtual Hashtable ElementsWithIDs
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Implementation of the SerializationHandler interfaces * </summary>

		/// <summary>
		/// We only need to override the endDocument, characters, and 
		/// setEscaping interfaces. A simple RTF does not have element
		/// nodes. We do not need to touch startElement and endElement.
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDocument() throws org.xml.sax.SAXException
		public override void startDocument()
		{

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDocument() throws org.xml.sax.SAXException
		public override void endDocument()
		{
			// Set the String value when the document is built.
			if (_size == 1)
			{
				_text = _textArray[0];
			}
			else
			{
				StringBuilder buffer = new StringBuilder();
				for (int i = 0; i < _size; i++)
				{
					buffer.Append(_textArray[i]);
				}
				_text = buffer.ToString();
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(String str) throws org.xml.sax.SAXException
		public override void characters(string str)
		{
			// Resize the text array if necessary
			if (_size >= _textArray.Length)
			{
				string[] newTextArray = new string[_textArray.Length * 2];
				Array.Copy(_textArray, 0, newTextArray, 0, _textArray.Length);
				_textArray = newTextArray;
			}

			// If the escape setting is false, set the corresponding bit in
			// the _dontEscape BitArray.
			if (!_escaping)
			{
				// The _dontEscape array is only created when needed.
				if (_dontEscape == null)
				{
					_dontEscape = new BitArray(8);
				}

				// Resize the _dontEscape array if necessary
				if (_size >= _dontEscape.size())
				{
					_dontEscape.resize(_dontEscape.size() * 2);
				}

				_dontEscape.Bit = _size;
			}

			_textArray[_size++] = str;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(char[] ch, int offset, int length) throws org.xml.sax.SAXException
		public override void characters(char[] ch, int offset, int length)
		{
			if (_size >= _textArray.Length)
			{
				string[] newTextArray = new string[_textArray.Length * 2];
				Array.Copy(_textArray, 0, newTextArray, 0, _textArray.Length);
				_textArray = newTextArray;
			}

			if (!_escaping)
			{
				if (_dontEscape == null)
				{
					_dontEscape = new BitArray(8);
				}

				if (_size >= _dontEscape.size())
				{
					_dontEscape.resize(_dontEscape.size() * 2);
				}

				_dontEscape.Bit = _size;
			}

			_textArray[_size++] = new string(ch, offset, length);

		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean setEscaping(boolean escape) throws org.xml.sax.SAXException
		public override bool setEscaping(bool escape)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean temp = _escaping;
			bool temp = _escaping;
			_escaping = escape;
			return temp;
		}

		/// <summary>
		/// Implementation of the DTM interfaces * </summary>

		/// <summary>
		/// The DTM interfaces are not used in this class. Implementing the DTM
		/// interface is a requirement from MultiDOM. If we have a better way
		/// of handling multiple documents, we can get rid of the DTM dependency.
		/// 
		/// The following interfaces are just placeholders. The implementation
		/// does not have an impact because they will not be used.
		/// </summary>

		public virtual void setFeature(string featureId, bool state)
		{
		}

		public virtual void setProperty(string property, object value)
		{
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisTraverser getAxisTraverser(final int axis)
		public virtual DTMAxisTraverser getAxisTraverser(int axis)
		{
			return null;
		}

		public virtual bool hasChildNodes(int nodeHandle)
		{
			return (getNodeIdent(nodeHandle) == RTF_ROOT);
		}

		public virtual int getFirstChild(int nodeHandle)
		{
			int nodeID = getNodeIdent(nodeHandle);
			if (nodeID == RTF_ROOT)
			{
				return getNodeHandle(RTF_TEXT);
			}
			else
			{
				return org.apache.xml.dtm.DTM_Fields.NULL;
			}
		}

		public virtual int getLastChild(int nodeHandle)
		{
			return getFirstChild(nodeHandle);
		}

		public virtual int getAttributeNode(int elementHandle, string namespaceURI, string name)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getFirstAttribute(int nodeHandle)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getFirstNamespaceNode(int nodeHandle, bool inScope)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getNextSibling(int nodeHandle)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getPreviousSibling(int nodeHandle)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getNextAttribute(int nodeHandle)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getNextNamespaceNode(int baseHandle, int namespaceHandle, bool inScope)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual int getOwnerDocument(int nodeHandle)
		{
			return Document;
		}

		public virtual int getDocumentRoot(int nodeHandle)
		{
			return Document;
		}

		public virtual XMLString getStringValue(int nodeHandle)
		{
			return new XMLStringDefault(getStringValueX(nodeHandle));
		}

		public virtual int getStringValueChunkCount(int nodeHandle)
		{
			return 0;
		}

		public virtual char[] getStringValueChunk(int nodeHandle, int chunkIndex, int[] startAndLen)
		{
			return null;
		}

		public virtual int getExpandedTypeID(string @namespace, string localName, int type)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual string getLocalNameFromExpandedNameID(int ExpandedNameID)
		{
			return EMPTY_STR;
		}

		public virtual string getNamespaceFromExpandedNameID(int ExpandedNameID)
		{
			return EMPTY_STR;
		}

		public virtual string getLocalName(int nodeHandle)
		{
			return EMPTY_STR;
		}

		public virtual string getPrefix(int nodeHandle)
		{
			return null;
		}

		public virtual string getNamespaceURI(int nodeHandle)
		{
			return EMPTY_STR;
		}

		public virtual string getNodeValue(int nodeHandle)
		{
			return (getNodeIdent(nodeHandle) == RTF_TEXT) ? _text : null;
		}

		public virtual short getNodeType(int nodeHandle)
		{
			int nodeID = getNodeIdent(nodeHandle);
			if (nodeID == RTF_TEXT)
			{
				return org.apache.xml.dtm.DTM_Fields.TEXT_NODE;
			}
			else if (nodeID == RTF_ROOT)
			{
				return org.apache.xml.dtm.DTM_Fields.ROOT_NODE;
			}
			else
			{
				return org.apache.xml.dtm.DTM_Fields.NULL;
			}

		}

		public virtual short getLevel(int nodeHandle)
		{
			int nodeID = getNodeIdent(nodeHandle);
			if (nodeID == RTF_TEXT)
			{
				return 2;
			}
			else if (nodeID == RTF_ROOT)
			{
				return 1;
			}
			else
			{
				return org.apache.xml.dtm.DTM_Fields.NULL;
			}
		}

		public virtual bool isSupported(string feature, string version)
		{
			return false;
		}

		public virtual string DocumentBaseURI
		{
			get
			{
				return EMPTY_STR;
			}
			set
			{
			}
		}


		public virtual string getDocumentSystemIdentifier(int nodeHandle)
		{
			return null;
		}

		public virtual string getDocumentEncoding(int nodeHandle)
		{
			return null;
		}

		public virtual string getDocumentStandalone(int nodeHandle)
		{
			return null;
		}

		public virtual string getDocumentVersion(int documentHandle)
		{
			return null;
		}

		public virtual bool DocumentAllDeclarationsProcessed
		{
			get
			{
				return false;
			}
		}

		public virtual string DocumentTypeDeclarationSystemIdentifier
		{
			get
			{
				return null;
			}
		}

		public virtual string DocumentTypeDeclarationPublicIdentifier
		{
			get
			{
				return null;
			}
		}

		public virtual int getElementById(string elementId)
		{
			return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		public virtual bool supportsPreStripping()
		{
			return false;
		}

		public virtual bool isNodeAfter(int firstNodeHandle, int secondNodeHandle)
		{
			return lessThan(firstNodeHandle, secondNodeHandle);
		}

		public virtual bool isCharacterElementContentWhitespace(int nodeHandle)
		{
			return false;
		}

		public virtual bool isDocumentAllDeclarationsProcessed(int documentHandle)
		{
			return false;
		}

		public virtual bool isAttributeSpecified(int attributeHandle)
		{
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, boolean normalize) throws org.xml.sax.SAXException
		public virtual void dispatchCharactersEvents(int nodeHandle, org.xml.sax.ContentHandler ch, bool normalize)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
		public virtual void dispatchToEvents(int nodeHandle, org.xml.sax.ContentHandler ch)
		{
		}

		public virtual Node getNode(int nodeHandle)
		{
			return makeNode(nodeHandle);
		}

		public virtual bool needsTwoThreads()
		{
			return false;
		}

		public virtual org.xml.sax.ContentHandler ContentHandler
		{
			get
			{
				return null;
			}
		}

		public virtual org.xml.sax.ext.LexicalHandler LexicalHandler
		{
			get
			{
				return null;
			}
		}

		public virtual org.xml.sax.EntityResolver EntityResolver
		{
			get
			{
				return null;
			}
		}

		public virtual org.xml.sax.DTDHandler DTDHandler
		{
			get
			{
				return null;
			}
		}

		public virtual org.xml.sax.ErrorHandler ErrorHandler
		{
			get
			{
				return null;
			}
		}

		public virtual org.xml.sax.ext.DeclHandler DeclHandler
		{
			get
			{
				return null;
			}
		}

		public virtual void appendChild(int newChild, bool clone, bool cloneDepth)
		{
		}

		public virtual void appendTextChild(string str)
		{
		}

		public virtual SourceLocator getSourceLocatorFor(int node)
		{
			return null;
		}

		public virtual void documentRegistration()
		{
		}

		public virtual void documentRelease()
		{
		}

		public virtual void migrateTo(DTMManager manager)
		{
		}
	}

}