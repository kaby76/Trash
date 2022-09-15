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
 * $Id: Constants.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using InstructionConstants = org.apache.bcel.generic.InstructionConstants;
	using SerializerBase = org.apache.xml.serializer.SerializerBase;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface Constants : InstructionConstants
	{

		// Error categories used to report errors to Parser.reportError()

		// Unexpected internal errors, such as null-ptr exceptions, etc.
		// Immediately terminates compilation, no translet produced
		public static int INTERNAL = 0;
		// XSLT elements that are not implemented and unsupported ext.
		// Immediately terminates compilation, no translet produced
		public static int UNSUPPORTED = 1;
		// Fatal error in the stylesheet input (parsing or content)
		// Immediately terminates compilation, no translet produced
		public static int FATAL = 2;
		// Other error in the stylesheet input (parsing or content)
		// Does not terminate compilation, no translet produced
		public static int ERROR = 3;
		// Other error in the stylesheet input (content errors only)
		// Does not terminate compilation, a translet is produced
		public static int WARNING = 4;

		public static string EMPTYSTRING = "";

		public static string NAMESPACE_FEATURE = "http://xml.org/sax/features/namespaces";

		public static string TRANSLET_INTF = "org.apache.xalan.xsltc.Translet";
		public static string TRANSLET_INTF_SIG = "Lorg/apache/xalan/xsltc/Translet;";

		public static string ATTRIBUTES_SIG = "Lorg/apache/xalan/xsltc/runtime/Attributes;";
		public static string NODE_ITERATOR_SIG = "Lorg/apache/xml/dtm/DTMAxisIterator;";
		public static string DOM_INTF_SIG = "Lorg/apache/xalan/xsltc/DOM;";
		public static string DOM_IMPL_CLASS = "org/apache/xalan/xsltc/DOM"; // xml/dtm/ref/DTMDefaultBaseIterators"; //xalan/xsltc/dom/DOMImpl";
		public static string SAX_IMPL_CLASS = "org/apache/xalan/xsltc/DOM/SAXImpl";
		public static string DOM_IMPL_SIG = "Lorg/apache/xalan/xsltc/dom/SAXImpl;"; //xml/dtm/ref/DTMDefaultBaseIterators"; //xalan/xsltc/dom/DOMImpl;";
		public static string SAX_IMPL_SIG = "Lorg/apache/xalan/xsltc/dom/SAXImpl;";
		public static string DOM_ADAPTER_CLASS = "org/apache/xalan/xsltc/dom/DOMAdapter";
		public static string DOM_ADAPTER_SIG = "Lorg/apache/xalan/xsltc/dom/DOMAdapter;";
		public static string MULTI_DOM_CLASS = "org.apache.xalan.xsltc.dom.MultiDOM";
		public static string MULTI_DOM_SIG = "Lorg/apache/xalan/xsltc/dom/MultiDOM;";

		public static string STRING = "java.lang.String";

		public static int ACC_PUBLIC = org.apache.bcel.Constants.ACC_PUBLIC;
		public static int ACC_SUPER = org.apache.bcel.Constants.ACC_SUPER;
		public static int ACC_FINAL = org.apache.bcel.Constants.ACC_FINAL;
		public static int ACC_PRIVATE = org.apache.bcel.Constants.ACC_PRIVATE;
		public static int ACC_PROTECTED = org.apache.bcel.Constants.ACC_PROTECTED;
		public static int ACC_STATIC = org.apache.bcel.Constants.ACC_STATIC;

		public static string STRING_SIG = "Ljava/lang/String;";
		public static string STRING_BUFFER_SIG = "Ljava/lang/StringBuffer;";
		public static string OBJECT_SIG = "Ljava/lang/Object;";
		public static string DOUBLE_SIG = "Ljava/lang/Double;";
		public static string INTEGER_SIG = "Ljava/lang/Integer;";
		public static string COLLATOR_CLASS = "java/text/Collator";
		public static string COLLATOR_SIG = "Ljava/text/Collator;";

		public static string NODE = "int";
		public static string NODE_ITERATOR = "org.apache.xml.dtm.DTMAxisIterator";
		public static string NODE_ITERATOR_BASE = "org.apache.xml.dtm.ref.DTMAxisIteratorBase";
		public static string SORT_ITERATOR = "org.apache.xalan.xsltc.dom.SortingIterator";
		public static string SORT_ITERATOR_SIG = "Lorg.apache.xalan.xsltc.dom.SortingIterator;";
		public static string NODE_SORT_RECORD = "org.apache.xalan.xsltc.dom.NodeSortRecord";
		public static string NODE_SORT_FACTORY = "org/apache/xalan/xsltc/dom/NodeSortRecordFactory";
		public static string NODE_SORT_RECORD_SIG = "Lorg/apache/xalan/xsltc/dom/NodeSortRecord;";
		public static string NODE_SORT_FACTORY_SIG = "Lorg/apache/xalan/xsltc/dom/NodeSortRecordFactory;";
		public static string LOCALE_CLASS = "java.util.Locale";
		public static string LOCALE_SIG = "Ljava/util/Locale;";
		public static string STRING_VALUE_HANDLER = "org.apache.xalan.xsltc.runtime.StringValueHandler";
		public static string STRING_VALUE_HANDLER_SIG = "Lorg/apache/xalan/xsltc/runtime/StringValueHandler;";
		public static string OUTPUT_HANDLER = SerializerBase.PKG_PATH + "/SerializationHandler";
		public static string OUTPUT_HANDLER_SIG = "L" + SerializerBase.PKG_PATH + "/SerializationHandler;";
		public static string FILTER_INTERFACE = "org.apache.xalan.xsltc.dom.Filter";
		public static string FILTER_INTERFACE_SIG = "Lorg/apache/xalan/xsltc/dom/Filter;";
		public static string UNION_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.UnionIterator";
		public static string STEP_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.StepIterator";
		public static string CACHED_NODE_LIST_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.CachedNodeListIterator";
		public static string NTH_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.NthIterator";
		public static string ABSOLUTE_ITERATOR = "org.apache.xalan.xsltc.dom.AbsoluteIterator";
		public static string DUP_FILTERED_ITERATOR = "org.apache.xalan.xsltc.dom.DupFilterIterator";
		public static string CURRENT_NODE_LIST_ITERATOR = "org.apache.xalan.xsltc.dom.CurrentNodeListIterator";
		public static string CURRENT_NODE_LIST_FILTER = "org.apache.xalan.xsltc.dom.CurrentNodeListFilter";
		public static string CURRENT_NODE_LIST_ITERATOR_SIG = "Lorg/apache/xalan/xsltc/dom/CurrentNodeListIterator;";
		public static string CURRENT_NODE_LIST_FILTER_SIG = "Lorg/apache/xalan/xsltc/dom/CurrentNodeListFilter;";
		public static string FILTER_STEP_ITERATOR = "org.apache.xalan.xsltc.dom.FilteredStepIterator";
		public static string FILTER_ITERATOR = "org.apache.xalan.xsltc.dom.FilterIterator";
		public static string SINGLETON_ITERATOR = "org.apache.xalan.xsltc.dom.SingletonIterator";
		public static string MATCHING_ITERATOR = "org.apache.xalan.xsltc.dom.MatchingIterator";
		public static string NODE_SIG = "I";
		public static string GET_PARENT = "getParent";
		public static string GET_PARENT_SIG = "(" + NODE_SIG + ")" + NODE_SIG;
		public static string NEXT_SIG = "()" + NODE_SIG;
		public static string NEXT = "next";
		public static string NEXTID = "nextNodeID";
		public static string MAKE_NODE = "makeNode";
		public static string MAKE_NODE_LIST = "makeNodeList";
		public static string GET_UNPARSED_ENTITY_URI = "getUnparsedEntityURI";
		public static string STRING_TO_REAL = "stringToReal";
		public static string STRING_TO_REAL_SIG = "(" + STRING_SIG + ")D";
		public static string STRING_TO_INT = "stringToInt";
		public static string STRING_TO_INT_SIG = "(" + STRING_SIG + ")I";

		public static string XSLT_PACKAGE = "org.apache.xalan.xsltc";
		public static string COMPILER_PACKAGE = XSLT_PACKAGE + ".compiler";
		public static string RUNTIME_PACKAGE = XSLT_PACKAGE + ".runtime";
		public static string TRANSLET_CLASS = RUNTIME_PACKAGE + ".AbstractTranslet";

		public static string TRANSLET_SIG = "Lorg/apache/xalan/xsltc/runtime/AbstractTranslet;";
		public static string UNION_ITERATOR_SIG = "Lorg/apache/xalan/xsltc/dom/UnionIterator;";
		public static string TRANSLET_OUTPUT_SIG = "L" + SerializerBase.PKG_PATH + "/SerializationHandler;";
		public static string MAKE_NODE_SIG = "(I)Lorg/w3c/dom/Node;";
		public static string MAKE_NODE_SIG2 = "(" + NODE_ITERATOR_SIG + ")Lorg/w3c/dom/Node;";
		public static string MAKE_NODE_LIST_SIG = "(I)Lorg/w3c/dom/NodeList;";
		public static string MAKE_NODE_LIST_SIG2 = "(" + NODE_ITERATOR_SIG + ")Lorg/w3c/dom/NodeList;";

		public static string STREAM_XML_OUTPUT = SerializerBase.PKG_NAME + ".ToXMLStream";

		public static string OUTPUT_BASE = SerializerBase.PKG_NAME + ".SerializerBase";

		public static string LOAD_DOCUMENT_CLASS = "org.apache.xalan.xsltc.dom.LoadDocument";

		public static string KEY_INDEX_CLASS = "org/apache/xalan/xsltc/dom/KeyIndex";
		public static string KEY_INDEX_SIG = "Lorg/apache/xalan/xsltc/dom/KeyIndex;";
		public static string KEY_INDEX_ITERATOR_SIG = "Lorg/apache/xalan/xsltc/dom/KeyIndex$KeyIndexIterator;";

		public static string DOM_INTF = "org.apache.xalan.xsltc.DOM";
		public static string DOM_IMPL = "org.apache.xalan.xsltc.dom.SAXImpl";
		public static string SAX_IMPL = "org.apache.xalan.xsltc.dom.SAXImpl";
		public static string STRING_CLASS = "java.lang.String";
		public static string OBJECT_CLASS = "java.lang.Object";
		public static string BOOLEAN_CLASS = "java.lang.Boolean";
		public static string STRING_BUFFER_CLASS = "java.lang.StringBuffer";
		public static string STRING_WRITER = "java.io.StringWriter";
		public static string WRITER_SIG = "Ljava/io/Writer;";

		public static string TRANSLET_OUTPUT_BASE = "org.apache.xalan.xsltc.TransletOutputBase";
		// output interface
		public static string TRANSLET_OUTPUT_INTERFACE = SerializerBase.PKG_NAME + ".SerializationHandler";
		public static string BASIS_LIBRARY_CLASS = "org.apache.xalan.xsltc.runtime.BasisLibrary";
		public static string ATTRIBUTE_LIST_IMPL_CLASS = "org.apache.xalan.xsltc.runtime.AttributeListImpl";
		public static string DOUBLE_CLASS = "java.lang.Double";
		public static string INTEGER_CLASS = "java.lang.Integer";
		public static string RUNTIME_NODE_CLASS = "org.apache.xalan.xsltc.runtime.Node";
		public static string MATH_CLASS = "java.lang.Math";

		public static string BOOLEAN_VALUE = "booleanValue";
		public static string BOOLEAN_VALUE_SIG = "()Z";
		public static string INT_VALUE = "intValue";
		public static string INT_VALUE_SIG = "()I";
		public static string DOUBLE_VALUE = "doubleValue";
		public static string DOUBLE_VALUE_SIG = "()D";

		public static string DOM_PNAME = "dom";
		public static string NODE_PNAME = "node";
		public static string TRANSLET_OUTPUT_PNAME = "handler";
		public static string ITERATOR_PNAME = "iterator";
		public static string DOCUMENT_PNAME = "document";
		public static string TRANSLET_PNAME = "translet";

		public static string INVOKE_METHOD = "invokeMethod";
		public static string GET_NODE_NAME = "getNodeNameX";
		public static string CHARACTERSW = "characters";
		public static string GET_CHILDREN = "getChildren";
		public static string GET_TYPED_CHILDREN = "getTypedChildren";
		public static string CHARACTERS = "characters";
		public static string APPLY_TEMPLATES = "applyTemplates";
		public static string GET_NODE_TYPE = "getNodeType";
		public static string GET_NODE_VALUE = "getStringValueX";
		public static string GET_ELEMENT_VALUE = "getElementValue";
		public static string GET_ATTRIBUTE_VALUE = "getAttributeValue";
		public static string HAS_ATTRIBUTE = "hasAttribute";
		public static string ADD_ITERATOR = "addIterator";
		public static string SET_START_NODE = "setStartNode";
		public static string RESET = "reset";

		public static string ATTR_SET_SIG = "(" + DOM_INTF_SIG + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + ")V";

		public static string GET_NODE_NAME_SIG = "(" + NODE_SIG + ")" + STRING_SIG;
		public static string CHARACTERSW_SIG = "(" + STRING_SIG + TRANSLET_OUTPUT_SIG + ")V";
		public static string CHARACTERS_SIG = "(" + NODE_SIG + TRANSLET_OUTPUT_SIG + ")V";
		public static string GET_CHILDREN_SIG = "(" + NODE_SIG + ")" + NODE_ITERATOR_SIG;
		public static string GET_TYPED_CHILDREN_SIG = "(I)" + NODE_ITERATOR_SIG;
		public static string GET_NODE_TYPE_SIG = "()S";
		public static string GET_NODE_VALUE_SIG = "(I)" + STRING_SIG;
		public static string GET_ELEMENT_VALUE_SIG = "(I)" + STRING_SIG;
		public static string GET_ATTRIBUTE_VALUE_SIG = "(II)" + STRING_SIG;
		public static string HAS_ATTRIBUTE_SIG = "(II)Z";
		public static string GET_ITERATOR_SIG = "()" + NODE_ITERATOR_SIG;

		public static string NAMES_INDEX = "namesArray";
		public static string NAMES_INDEX_SIG = "[" + STRING_SIG;
		public static string URIS_INDEX = "urisArray";
		public static string URIS_INDEX_SIG = "[" + STRING_SIG;
		public static string TYPES_INDEX = "typesArray";
		public static string TYPES_INDEX_SIG = "[I";
		public static string NAMESPACE_INDEX = "namespaceArray";
		public static string NAMESPACE_INDEX_SIG = "[" + STRING_SIG;
		public static string NS_ANCESTORS_INDEX_SIG = "[I";
		public static string PREFIX_URIS_IDX_SIG = "[I";
		public static string PREFIX_URIS_ARRAY_SIG = "[" + STRING_SIG;
		public static string HASIDCALL_INDEX = "_hasIdCall";
		public static string HASIDCALL_INDEX_SIG = "Z";
		public static string TRANSLET_VERSION_INDEX = "transletVersion";
		public static string TRANSLET_VERSION_INDEX_SIG = "I";
		public static string LOOKUP_STYLESHEET_QNAME_NS_REF = "lookupStylesheetQNameNamespace";
		public static string LOOKUP_STYLESHEET_QNAME_NS_SIG = "(" + STRING_SIG + "I" + NS_ANCESTORS_INDEX_SIG + PREFIX_URIS_IDX_SIG + PREFIX_URIS_ARRAY_SIG + "Z)" + STRING_SIG;
		public static string EXPAND_STYLESHEET_QNAME_REF = "expandStylesheetQNameRef";
		public static string EXPAND_STYLESHEET_QNAME_SIG = "(" + STRING_SIG + "I" + NS_ANCESTORS_INDEX_SIG + PREFIX_URIS_IDX_SIG + PREFIX_URIS_ARRAY_SIG + "Z)" + STRING_SIG;

		public static string DOM_FIELD = "_dom";
		public static string STATIC_NAMES_ARRAY_FIELD = "_sNamesArray";
		public static string STATIC_URIS_ARRAY_FIELD = "_sUrisArray";
		public static string STATIC_TYPES_ARRAY_FIELD = "_sTypesArray";
		public static string STATIC_NAMESPACE_ARRAY_FIELD = "_sNamespaceArray";
		public static string STATIC_NS_ANCESTORS_ARRAY_FIELD = "_sNamespaceAncestorsArray";
		public static string STATIC_PREFIX_URIS_IDX_ARRAY_FIELD = "_sPrefixURIsIdxArray";
		public static string STATIC_PREFIX_URIS_ARRAY_FIELD = "_sPrefixURIPairsArray";
		public static string STATIC_CHAR_DATA_FIELD = "_scharData";
		public static string STATIC_CHAR_DATA_FIELD_SIG = "[C";
		public static string FORMAT_SYMBOLS_FIELD = "format_symbols";

		public static string ITERATOR_FIELD_SIG = NODE_ITERATOR_SIG;
		public static string NODE_FIELD = "node";
		public static string NODE_FIELD_SIG = "I";

		public static string EMPTYATTR_FIELD = "EmptyAttributes";
		public static string ATTRIBUTE_LIST_FIELD = "attributeList";
		public static string CLEAR_ATTRIBUTES = "clear";
		public static string ADD_ATTRIBUTE = "addAttribute";
		public static string ATTRIBUTE_LIST_IMPL_SIG = "Lorg/apache/xalan/xsltc/runtime/AttributeListImpl;";
		public static string CLEAR_ATTRIBUTES_SIG = "()" + ATTRIBUTE_LIST_IMPL_SIG;
		public static string ADD_ATTRIBUTE_SIG = "(" + STRING_SIG + STRING_SIG + ")" + ATTRIBUTE_LIST_IMPL_SIG;

		public static string ADD_ITERATOR_SIG = "(" + NODE_ITERATOR_SIG + ")" + UNION_ITERATOR_SIG;

		public static string ORDER_ITERATOR = "orderNodes";
		public static string ORDER_ITERATOR_SIG = "(" + NODE_ITERATOR_SIG + "I)" + NODE_ITERATOR_SIG;

		public static string SET_START_NODE_SIG = "(" + NODE_SIG + ")" + NODE_ITERATOR_SIG;

		public static string NODE_COUNTER = "org.apache.xalan.xsltc.dom.NodeCounter";
		public static string NODE_COUNTER_SIG = "Lorg/apache/xalan/xsltc/dom/NodeCounter;";
		public static string DEFAULT_NODE_COUNTER = "org.apache.xalan.xsltc.dom.DefaultNodeCounter";
		public static string DEFAULT_NODE_COUNTER_SIG = "Lorg/apache/xalan/xsltc/dom/DefaultNodeCounter;";
		public static string TRANSLET_FIELD = "translet";
		public static string TRANSLET_FIELD_SIG = TRANSLET_SIG;

		public static string RESET_SIG = "()" + NODE_ITERATOR_SIG;
		public static string GET_PARAMETER = "getParameter";
		public static string ADD_PARAMETER = "addParameter";
		public static string PUSH_PARAM_FRAME = "pushParamFrame";
		public static string PUSH_PARAM_FRAME_SIG = "()V";
		public static string POP_PARAM_FRAME = "popParamFrame";
		public static string POP_PARAM_FRAME_SIG = "()V";
		public static string GET_PARAMETER_SIG = "(" + STRING_SIG + ")" + OBJECT_SIG;
		public static string ADD_PARAMETER_SIG = "(" + STRING_SIG + OBJECT_SIG + "Z)" + OBJECT_SIG;

		public static string STRIP_SPACE = "stripSpace";
		public static string STRIP_SPACE_INTF = "org/apache/xalan/xsltc/StripFilter";
		public static string STRIP_SPACE_SIG = "Lorg/apache/xalan/xsltc/StripFilter;";
		public static string STRIP_SPACE_PARAMS = "(Lorg/apache/xalan/xsltc/DOM;II)Z";

		public static string GET_NODE_VALUE_ITERATOR = "getNodeValueIterator";
		public static string GET_NODE_VALUE_ITERATOR_SIG = "(" + NODE_ITERATOR_SIG + "I" + STRING_SIG + "Z)" + NODE_ITERATOR_SIG;

		public static string GET_UNPARSED_ENTITY_URI_SIG = "(" + STRING_SIG + ")" + STRING_SIG;

		public static int POSITION_INDEX = 2;
		public static int LAST_INDEX = 3;

		public static string XMLNS_PREFIX = "xmlns";
		public static string XMLNS_STRING = "xmlns:";
		public static string XMLNS_URI = "http://www.w3.org/2000/xmlns/";
		public static string XSLT_URI = "http://www.w3.org/1999/XSL/Transform";
		public static string XHTML_URI = "http://www.w3.org/1999/xhtml";
		public static string TRANSLET_URI = "http://xml.apache.org/xalan/xsltc";
		public static string REDIRECT_URI = "http://xml.apache.org/xalan/redirect";
		public static string FALLBACK_CLASS = "org.apache.xalan.xsltc.compiler.Fallback";

		public static int RTF_INITIAL_SIZE = 32;
	}

}