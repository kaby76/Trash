using System;

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
 * $Id: MultiDOM.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{
	using DOM = org.apache.xalan.xsltc.DOM;
	using StripFilter = org.apache.xalan.xsltc.StripFilter;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using TransletException = org.apache.xalan.xsltc.TransletException;
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;
	using DTM = org.apache.xml.dtm.DTM;
	using Axis = org.apache.xml.dtm.Axis;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMAxisIteratorBase = org.apache.xml.dtm.@ref.DTMAxisIteratorBase;
	using DTMDefaultBase = org.apache.xml.dtm.@ref.DTMDefaultBase;
	using SuballocatedIntVector = org.apache.xml.utils.SuballocatedIntVector;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	public sealed class MultiDOM : DOM
	{

		private static readonly int NO_TYPE = DOM.FIRST_TYPE - 2;
		private const int INITIAL_SIZE = 4;

		private DOM[] _adapters;
		private DOMAdapter _main;
		private DTMManager _dtmManager;
		private int _free;
		private int _size;

		private Hashtable _documents = new Hashtable();

		private sealed class AxisIterator : DTMAxisIteratorBase
		{
			private readonly MultiDOM outerInstance;

			// constitutive data
			internal readonly int _axis;
			internal readonly int _type;
			// implementation mechanism
			internal DTMAxisIterator _source;
			internal int _dtmId = -1;

			public AxisIterator(MultiDOM outerInstance, in int axis, in int type)
			{
				this.outerInstance = outerInstance;
				_axis = axis;
				_type = type;
			}

			public override int next()
			{
				if (_source == null)
				{
					return (END);
				}
				return _source.next();
			}


			public override bool Restartable
			{
				set
				{
					if (_source != null)
					{
						_source.Restartable = value;
					}
				}
			}

			public override DTMAxisIterator setStartNode(in int node)
			{
				if (node == DTM.NULL)
				{
					return this;
				}

				int dom = (int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS);

				// Get a new source first time and when mask changes
				if (_source == null || _dtmId != dom)
				{
					if (_type == NO_TYPE)
					{
						_source = outerInstance._adapters[dom].getAxisIterator(_axis);
					}
					else if (_axis == Axis.CHILD)
					{
						_source = outerInstance._adapters[dom].getTypedChildren(_type);
					}
					else
					{
						_source = outerInstance._adapters[dom].getTypedAxisIterator(_axis, _type);
					}
				}

				_dtmId = dom;
				_source.StartNode = node;
				return this;
			}

			public override DTMAxisIterator reset()
			{
				if (_source != null)
				{
					_source.reset();
				}
				return this;
			}

			public override int Last
			{
				get
				{
					if (_source != null)
					{
						return _source.Last;
					}
					else
					{
						return END;
					}
				}
			}

			public override int Position
			{
				get
				{
					if (_source != null)
					{
						return _source.Position;
					}
					else
					{
						return END;
					}
				}
			}

			public override bool Reverse
			{
				get
				{
					return Axis.isReverse(_axis);
				}
			}

			public override void setMark()
			{
				if (_source != null)
				{
					_source.setMark();
				}
			}

			public override void gotoMark()
			{
				if (_source != null)
				{
					_source.gotoMark();
				}
			}

			public override DTMAxisIterator cloneIterator()
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AxisIterator clone = new AxisIterator(_axis, _type);
				AxisIterator clone = new AxisIterator(outerInstance, _axis, _type);
				if (_source != null)
				{
					clone._source = _source.cloneIterator();
				}
				clone._dtmId = _dtmId;
				return clone;
			}
		} // end of AxisIterator


		/// <summary>
		///************************************************************
		/// This is a specialised iterator for predicates comparing node or
		/// attribute values to variable or parameter values.
		/// </summary>
		private sealed class NodeValueIterator : DTMAxisIteratorBase
		{
			private readonly MultiDOM outerInstance;


			internal DTMAxisIterator _source;
			internal string _value;
			internal bool _op;
			internal readonly bool _isReverse;
			internal int _returnType = RETURN_PARENT;

			public NodeValueIterator(MultiDOM outerInstance, DTMAxisIterator source, int returnType, string value, bool op)
			{
				this.outerInstance = outerInstance;
				_source = source;
				_returnType = returnType;
				_value = value;
				_op = op;
				_isReverse = source.Reverse;
			}

			public override bool Reverse
			{
				get
				{
					return _isReverse;
				}
			}

			public override DTMAxisIterator cloneIterator()
			{
				try
				{
					NodeValueIterator clone = (NodeValueIterator)base.clone();
					clone._source = _source.cloneIterator();
					clone.Restartable = false;
					return clone.reset();
				}
				catch (CloneNotSupportedException e)
				{
					BasisLibrary.runTimeError(BasisLibrary.ITERATOR_CLONE_ERR, e.ToString());
					return null;
				}
			}


			public override bool Restartable
			{
				set
				{
					_isRestartable = value;
					_source.Restartable = value;
				}
			}

			public override DTMAxisIterator reset()
			{
				_source.reset();
				return resetPosition();
			}

			public override int next()
			{

				int node;
				while ((node = _source.next()) != END)
				{
					string val = outerInstance.getStringValueX(node);
					if (_value.Equals(val) == _op)
					{
						if (_returnType == RETURN_CURRENT)
						{
							return returnNode(node);
						}
						else
						{
							return returnNode(outerInstance.getParent(node));
						}
					}
				}
				return END;
			}

			public override DTMAxisIterator setStartNode(int node)
			{
				if (_isRestartable)
				{
					_source.StartNode = _startNode = node;
					return resetPosition();
				}
				return this;
			}

			public override void setMark()
			{
				_source.setMark();
			}

			public override void gotoMark()
			{
				_source.gotoMark();
			}
		}

		public MultiDOM(DOM main)
		{
			_size = INITIAL_SIZE;
			_free = 1;
			_adapters = new DOM[INITIAL_SIZE];
			DOMAdapter adapter = (DOMAdapter)main;
			_adapters[0] = adapter;
			_main = adapter;
			DOM dom = adapter.DOMImpl;
			if (dom is DTMDefaultBase)
			{
				_dtmManager = ((DTMDefaultBase)dom).Manager;
			}

			// %HZ% %REVISIT% Is this the right thing to do here?  In the old
			// %HZ% %REVISIT% version, the main document did not get added through
			// %HZ% %REVISIT% a call to addDOMAdapter, which meant it couldn't be
			// %HZ% %REVISIT% found by a call to getDocumentMask.  The problem is
			// %HZ% %REVISIT% TransformerHandler is typically constructed with a
			// %HZ% %REVISIT% system ID equal to the stylesheet's URI; with SAX
			// %HZ% %REVISIT% input, it ends up giving that URI to the document.
			// %HZ% %REVISIT% Then, any references to document('') are resolved
			// %HZ% %REVISIT% using the stylesheet's URI.
			// %HZ% %REVISIT% MultiDOM.getDocumentMask is called to verify that
			// %HZ% %REVISIT% a document associated with that URI has not been
			// %HZ% %REVISIT% encountered, and that method ends up returning the
			// %HZ% %REVISIT% mask of the main document, when what we really what
			// %HZ% %REVISIT% is to read the stylesheet itself!
			addDOMAdapter(adapter, false);
		}

		public int nextMask()
		{
			return _free;
		}

		public void setupMapping(string[] names, string[] uris, int[] types, string[] namespaces)
		{
			// This method only has a function in DOM adapters
		}

		public int addDOMAdapter(DOMAdapter adapter)
		{
			return addDOMAdapter(adapter, true);
		}

		private int addDOMAdapter(DOMAdapter adapter, bool indexByURI)
		{
			// Add the DOM adapter to the array of DOMs
			DOM dom = adapter.DOMImpl;

			int domNo = 1;
			int dtmSize = 1;
			SuballocatedIntVector dtmIds = null;
			if (dom is DTMDefaultBase)
			{
				DTMDefaultBase dtmdb = (DTMDefaultBase)dom;
				dtmIds = dtmdb.DTMIDs;
				dtmSize = dtmIds.size();
				domNo = (int)((uint)dtmIds.elementAt(dtmSize-1) >> DTMManager.IDENT_DTM_NODE_BITS);
			}
			else if (dom is SimpleResultTreeImpl)
			{
				SimpleResultTreeImpl simpleRTF = (SimpleResultTreeImpl)dom;
				domNo = (int)((uint)simpleRTF.Document >> DTMManager.IDENT_DTM_NODE_BITS);
			}

			if (domNo >= _size)
			{
				int oldSize = _size;
				do
				{
					_size *= 2;
				} while (_size <= domNo);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DOMAdapter[] newArray = new DOMAdapter[_size];
				DOMAdapter[] newArray = new DOMAdapter[_size];
				Array.Copy(_adapters, 0, newArray, 0, oldSize);
				_adapters = newArray;
			}

			_free = domNo + 1;

			if (dtmSize == 1)
			{
				_adapters[domNo] = adapter;
			}
			else if (dtmIds != null)
			{
				int domPos = 0;
				for (int i = dtmSize - 1; i >= 0; i--)
				{
					domPos = (int)((uint)dtmIds.elementAt(i) >> DTMManager.IDENT_DTM_NODE_BITS);
					_adapters[domPos] = adapter;
				}
				domNo = domPos;
			}

			// Store reference to document (URI) in hashtable
			if (indexByURI)
			{
				string uri = adapter.getDocumentURI(0);
				_documents.put(uri, new int?(domNo));
			}

			// If the dom is an AdaptiveResultTreeImpl, we need to create a
			// DOMAdapter around its nested dom object (if it is non-null) and
			// add the DOMAdapter to the list.
			if (dom is AdaptiveResultTreeImpl)
			{
				AdaptiveResultTreeImpl adaptiveRTF = (AdaptiveResultTreeImpl)dom;
				DOM nestedDom = adaptiveRTF.NestedDOM;
				if (nestedDom != null)
				{
					DOMAdapter newAdapter = new DOMAdapter(nestedDom, adapter.NamesArray, adapter.UrisArray, adapter.TypesArray, adapter.NamespaceArray);
					addDOMAdapter(newAdapter);
				}
			}

			return domNo;
		}

		public int getDocumentMask(string uri)
		{
			int? domIdx = (int?)_documents.get(uri);
			if (domIdx == null)
			{
				return (-1);
			}
			else
			{
				return domIdx.Value;
			}
		}

		public DOM getDOMAdapter(string uri)
		{
			int? domIdx = (int?)_documents.get(uri);
			if (domIdx == null)
			{
				return (null);
			}
			else
			{
				return (_adapters[domIdx.Value]);
			}
		}

		public int Document
		{
			get
			{
				return _main.Document;
			}
		}

		public DTMManager DTMManager
		{
			get
			{
				return _dtmManager;
			}
		}

		/// <summary>
		/// Returns singleton iterator containing the document root 
		/// </summary>
		public DTMAxisIterator Iterator
		{
			get
			{
				// main source document @ 0
				return _main.Iterator;
			}
		}

		public string StringValue
		{
			get
			{
				return _main.StringValue;
			}
		}

		public DTMAxisIterator getChildren(in int node)
		{
			return _adapters[getDTMId(node)].getChildren(node);
		}

		public DTMAxisIterator getTypedChildren(in int type)
		{
			return new AxisIterator(this, Axis.CHILD, type);
		}

		public DTMAxisIterator getAxisIterator(in int axis)
		{
			return new AxisIterator(this, axis, NO_TYPE);
		}

		public DTMAxisIterator getTypedAxisIterator(in int axis, in int type)
		{
			return new AxisIterator(this, axis, type);
		}

		public DTMAxisIterator getNthDescendant(int node, int n, bool includeself)
		{
			return _adapters[getDTMId(node)].getNthDescendant(node, n, includeself);
		}

		public DTMAxisIterator getNodeValueIterator(DTMAxisIterator iterator, int type, string value, bool op)
		{
			return (new NodeValueIterator(this, iterator, type, value, op));
		}

		public DTMAxisIterator getNamespaceAxisIterator(in int axis, in int ns)
		{
			DTMAxisIterator iterator = _main.getNamespaceAxisIterator(axis, ns);
			return (iterator);
		}

		public DTMAxisIterator orderNodes(DTMAxisIterator source, int node)
		{
			return _adapters[getDTMId(node)].orderNodes(source, node);
		}

		public int getExpandedTypeID(in int node)
		{
			if (node != DTM.NULL)
			{
				return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getExpandedTypeID(node);
			}
			else
			{
				return DTM.NULL;
			}
		}

		public int getNamespaceType(in int node)
		{
			return _adapters[getDTMId(node)].getNamespaceType(node);
		}

		public int getNSType(int node)
		{
			return _adapters[getDTMId(node)].getNSType(node);
		}

		public int getParent(in int node)
		{
			if (node == DTM.NULL)
			{
				return DTM.NULL;
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getParent(node);
		}

		public int getAttributeNode(in int type, in int el)
		{
			if (el == DTM.NULL)
			{
				return DTM.NULL;
			}
			return _adapters[(int)((uint)el >> DTMManager.IDENT_DTM_NODE_BITS)].getAttributeNode(type, el);
		}

		public string getNodeName(in int node)
		{
			if (node == DTM.NULL)
			{
				return "";
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getNodeName(node);
		}

		public string getNodeNameX(in int node)
		{
			if (node == DTM.NULL)
			{
				return "";
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getNodeNameX(node);
		}

		public string getNamespaceName(in int node)
		{
			if (node == DTM.NULL)
			{
				return "";
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getNamespaceName(node);
		}

		public string getStringValueX(in int node)
		{
			if (node == DTM.NULL)
			{
				return "";
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getStringValueX(node);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void copy(in int node, SerializationHandler handler)
		{
			if (node != DTM.NULL)
			{
				_adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].copy(node, handler);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(org.apache.xml.dtm.DTMAxisIterator nodes, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void copy(DTMAxisIterator nodes, SerializationHandler handler)
		{
			int node;
			while ((node = nodes.next()) != DTM.NULL)
			{
				_adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].copy(node, handler);
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String shallowCopy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public string shallowCopy(in int node, SerializationHandler handler)
		{
			if (node == DTM.NULL)
			{
				return "";
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].shallowCopy(node, handler);
		}

		public bool lessThan(in int node1, in int node2)
		{
			if (node1 == DTM.NULL)
			{
				return true;
			}
			if (node2 == DTM.NULL)
			{
				return false;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dom1 = getDTMId(node1);
			int dom1 = getDTMId(node1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dom2 = getDTMId(node2);
			int dom2 = getDTMId(node2);
			return dom1 == dom2 ? _adapters[dom1].lessThan(node1, node2) : dom1 < dom2;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(final int textNode, org.apache.xml.serializer.SerializationHandler handler) throws org.apache.xalan.xsltc.TransletException
		public void characters(in int textNode, SerializationHandler handler)
		{
			if (textNode != DTM.NULL)
			{
				_adapters[(int)((uint)textNode >> DTMManager.IDENT_DTM_NODE_BITS)].characters(textNode, handler);
			}
		}

		public StripFilter Filter
		{
			set
			{
				for (int dom = 0; dom < _free; dom++)
				{
					if (_adapters[dom] != null)
					{
						_adapters[dom].Filter = value;
					}
				}
			}
		}

		public Node makeNode(int index)
		{
			if (index == DTM.NULL)
			{
				return null;
			}
			return _adapters[getDTMId(index)].makeNode(index);
		}

		public Node makeNode(DTMAxisIterator iter)
		{
			// TODO: gather nodes from all DOMs ?
			return _main.makeNode(iter);
		}

		public NodeList makeNodeList(int index)
		{
			if (index == DTM.NULL)
			{
				return null;
			}
			return _adapters[getDTMId(index)].makeNodeList(index);
		}

		public NodeList makeNodeList(DTMAxisIterator iter)
		{
			// TODO: gather nodes from all DOMs ?
			return _main.makeNodeList(iter);
		}

		public string getLanguage(int node)
		{
			return _adapters[getDTMId(node)].getLanguage(node);
		}

		public int Size
		{
			get
			{
				int size = 0;
				for (int i = 0; i < _size; i++)
				{
					size += _adapters[i].Size;
				}
				return (size);
			}
		}

		public string getDocumentURI(int node)
		{
			if (node == DTM.NULL)
			{
				node = DOM.NULL;
			}
			return _adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].getDocumentURI(0);
		}

		public bool isElement(in int node)
		{
			if (node == DTM.NULL)
			{
				return false;
			}
			return (_adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].isElement(node));
		}

		public bool isAttribute(in int node)
		{
			if (node == DTM.NULL)
			{
				return false;
			}
			return (_adapters[(int)((uint)node >> DTMManager.IDENT_DTM_NODE_BITS)].isAttribute(node));
		}

		public int getDTMId(int nodeHandle)
		{
			if (nodeHandle == DTM.NULL)
			{
				return 0;
			}

			int id = (int)((uint)nodeHandle >> DTMManager.IDENT_DTM_NODE_BITS);
			while (id >= 2 && _adapters[id] == _adapters[id - 1])
			{
				id--;
			}
			return id;
		}

		public int getNodeIdent(int nodeHandle)
		{
			return _adapters[(int)((uint)nodeHandle >> DTMManager.IDENT_DTM_NODE_BITS)].getNodeIdent(nodeHandle);
		}

		public int getNodeHandle(int nodeId)
		{
			return _main.getNodeHandle(nodeId);
		}

		public DOM getResultTreeFrag(int initSize, int rtfType)
		{
			return _main.getResultTreeFrag(initSize, rtfType);
		}

		public DOM getResultTreeFrag(int initSize, int rtfType, bool addToManager)
		{
			return _main.getResultTreeFrag(initSize, rtfType, addToManager);
		}

		public DOM Main
		{
			get
			{
				return _main;
			}
		}

		/// <summary>
		/// Returns a DOMBuilder class wrapped in a SAX adapter.
		/// </summary>
		public SerializationHandler OutputDomBuilder
		{
			get
			{
				return _main.OutputDomBuilder;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String lookupNamespace(int node, String prefix) throws org.apache.xalan.xsltc.TransletException
		public string lookupNamespace(int node, string prefix)
		{
			return _main.lookupNamespace(node, prefix);
		}

		// %HZ% Does this method make any sense here???
		public string getUnparsedEntityURI(string entity)
		{
			return _main.getUnparsedEntityURI(entity);
		}

		// %HZ% Does this method make any sense here???
		public Hashtable ElementsWithIDs
		{
			get
			{
				return _main.ElementsWithIDs;
			}
		}
	}

}