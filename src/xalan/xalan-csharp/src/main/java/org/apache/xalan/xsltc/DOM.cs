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

		// 0 is reserved for NodeIterator.END

		// used by some node iterators to know which node to return

		// Constants used by getResultTreeFrag to indicate the types of the RTFs.

		/// <summary>
		/// returns singleton iterator containg the document root </summary>
		DTMAxisIterator Iterator {get;}
		string StringValue {get;}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getChildren(final int node);
		DTMAxisIterator getChildren(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getTypedChildren(final int type);
		DTMAxisIterator getTypedChildren(int type);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getAxisIterator(final int axis);
		DTMAxisIterator getAxisIterator(int axis);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getTypedAxisIterator(final int axis, final int type);
		DTMAxisIterator getTypedAxisIterator(int axis, int type);
		DTMAxisIterator getNthDescendant(int node, int n, bool includeself);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMAxisIterator getNamespaceAxisIterator(final int axis, final int ns);
		DTMAxisIterator getNamespaceAxisIterator(int axis, int ns);
		DTMAxisIterator getNodeValueIterator(DTMAxisIterator iter, int returnType, string value, bool op);
		DTMAxisIterator orderNodes(DTMAxisIterator source, int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getNodeName(final int node);
		string getNodeName(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getNodeNameX(final int node);
		string getNodeNameX(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getNamespaceName(final int node);
		string getNamespaceName(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getExpandedTypeID(final int node);
		int getExpandedTypeID(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getNamespaceType(final int node);
		int getNamespaceType(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getParent(final int node);
		int getParent(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getAttributeNode(final int gType, final int element);
		int getAttributeNode(int gType, int element);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public String getStringValueX(final int node);
		string getStringValueX(int node);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		void copy(int node, SerializationHandler handler);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void copy(org.apache.xml.dtm.DTMAxisIterator nodes, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		void copy(DTMAxisIterator nodes, SerializationHandler handler);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String shallowCopy(final int node, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		string shallowCopy(int node, SerializationHandler handler);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean lessThan(final int node1, final int node2);
		bool lessThan(int node1, int node2);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(final int textNode, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		void characters(int textNode, SerializationHandler handler);
		Node makeNode(int index);
		Node makeNode(DTMAxisIterator iter);
		NodeList makeNodeList(int index);
		NodeList makeNodeList(DTMAxisIterator iter);
		string getLanguage(int node);
		int Size {get;}
		string getDocumentURI(int node);
		StripFilter Filter {set;}
		void setupMapping(string[] names, string[] urisArray, int[] typesArray, string[] namespaces);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean isElement(final int node);
		bool isElement(int node);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean isAttribute(final int node);
		bool isAttribute(int node);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String lookupNamespace(int node, String prefix) throws TransletException;
		string lookupNamespace(int node, string prefix);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getNodeIdent(final int nodehandle);
		int getNodeIdent(int nodehandle);
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public int getNodeHandle(final int nodeId);
		int getNodeHandle(int nodeId);
		DOM getResultTreeFrag(int initialSize, int rtfType);
		DOM getResultTreeFrag(int initialSize, int rtfType, bool addToDTMManager);
		SerializationHandler OutputDomBuilder {get;}
		int getNSType(int node);
		int Document {get;}
		string getUnparsedEntityURI(string name);
		Hashtable ElementsWithIDs {get;}
	}

	public static class DOM_Fields
	{
		public const int FIRST_TYPE = 0;
		public const int NO_TYPE = -1;
		public const int NULL = 0;
		public const int RETURN_CURRENT = 0;
		public const int RETURN_PARENT = 1;
		public const int SIMPLE_RTF = 0;
		public const int ADAPTIVE_RTF = 1;
		public const int TREE_RTF = 2;
	}

}