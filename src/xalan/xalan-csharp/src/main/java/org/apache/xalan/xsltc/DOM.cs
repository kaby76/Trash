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
 * $Id: DOM.java 468648 2006-10-28 07:00:06Z minchau $
 */

namespace org.apache.xalan.xsltc
{
	using Hashtable = org.apache.xalan.xsltc.runtime.Hashtable;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface DOM
	{
		public static int FIRST_TYPE = 0;

		public static int NO_TYPE = -1;

		// 0 is reserved for NodeIterator.END
		public static int NULL = 0;

		// used by some node iterators to know which node to return
		public static int RETURN_CURRENT = 0;
		public static int RETURN_PARENT = 1;

		// Constants used by getResultTreeFrag to indicate the types of the RTFs.
		public static int SIMPLE_RTF = 0;
		public static int ADAPTIVE_RTF = 1;
		public static int TREE_RTF = 2;

		/// <summary>
		/// returns singleton iterator containg the document root </summary>
		DTMAxisIterator Iterator {get;}
		string StringValue {get;}

		DTMAxisIterator getChildren(in int node);
		DTMAxisIterator getTypedChildren(in int type);
		DTMAxisIterator getAxisIterator(in int axis);
		DTMAxisIterator getTypedAxisIterator(in int axis, in int type);
		DTMAxisIterator getNthDescendant(int node, int n, bool includeself);
		DTMAxisIterator getNamespaceAxisIterator(in int axis, in int ns);
		DTMAxisIterator getNodeValueIterator(DTMAxisIterator iter, int returnType, string value, bool op);
		DTMAxisIterator orderNodes(DTMAxisIterator source, int node);
		string getNodeName(in int node);
		string getNodeNameX(in int node);
		string getNamespaceName(in int node);
		int getExpandedTypeID(in int node);
		int getNamespaceType(in int node);
		int getParent(in int node);
		int getAttributeNode(in int gType, in int element);
		string getStringValueX(in int node);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		void copy(in int node, SerializationHandler handler);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void copy(org.apache.xml.dtm.DTMAxisIterator nodes, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		void copy(DTMAxisIterator nodes, SerializationHandler handler);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String shallowCopy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		string shallowCopy(in int node, SerializationHandler handler);
		bool lessThan(in int node1, in int node2);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void characters(final int textNode, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		void characters(in int textNode, SerializationHandler handler);
		Node makeNode(int index);
		Node makeNode(DTMAxisIterator iter);
		NodeList makeNodeList(int index);
		NodeList makeNodeList(DTMAxisIterator iter);
		string getLanguage(int node);
		int Size {get;}
		string getDocumentURI(int node);
		StripFilter Filter {set;}
		void setupMapping(string[] names, string[] urisArray, int[] typesArray, string[] namespaces);
		bool isElement(in int node);
		bool isAttribute(in int node);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String lookupNamespace(int node, String prefix) throws TransletException;
		string lookupNamespace(int node, string prefix);
		int getNodeIdent(in int nodehandle);
		int getNodeHandle(in int nodeId);
		DOM getResultTreeFrag(int initialSize, int rtfType);
		DOM getResultTreeFrag(int initialSize, int rtfType, bool addToDTMManager);
		SerializationHandler OutputDomBuilder {get;}
		int getNSType(int node);
		int Document {get;}
		string getUnparsedEntityURI(string name);
		Hashtable ElementsWithIDs {get;}
	}

}