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
		// XSLT elements that are not implemented and unsupported ext.
		// Immediately terminates compilation, no translet produced
		// Fatal error in the stylesheet input (parsing or content)
		// Immediately terminates compilation, no translet produced
		// Other error in the stylesheet input (parsing or content)
		// Does not terminate compilation, no translet produced
		// Other error in the stylesheet input (content errors only)
		// Does not terminate compilation, a translet is produced
		// output interface
	}

	public static class Constants_Fields
	{
		public const int INTERNAL = 0;
		public const int UNSUPPORTED = 1;
		public const int FATAL = 2;
		public const int ERROR = 3;
		public const int WARNING = 4;
		public const string EMPTYSTRING = "";
		public const string NAMESPACE_FEATURE = "http://xml.org/sax/features/namespaces";
		public const string TRANSLET_INTF = "org.apache.xalan.xsltc.Translet";
		public const string TRANSLET_INTF_SIG = "Lorg/apache/xalan/xsltc/Translet;";
		public const string ATTRIBUTES_SIG = "Lorg/apache/xalan/xsltc/runtime/Attributes;";
		public const string NODE_ITERATOR_SIG = "Lorg/apache/xml/dtm/DTMAxisIterator;";
		public const string DOM_INTF_SIG = "Lorg/apache/xalan/xsltc/DOM;";
		public const string DOM_IMPL_CLASS = "org/apache/xalan/xsltc/DOM";
		public const string SAX_IMPL_CLASS = "org/apache/xalan/xsltc/DOM/SAXImpl";
		public const string DOM_IMPL_SIG = "Lorg/apache/xalan/xsltc/dom/SAXImpl;";
		public const string SAX_IMPL_SIG = "Lorg/apache/xalan/xsltc/dom/SAXImpl;";
		public const string DOM_ADAPTER_CLASS = "org/apache/xalan/xsltc/dom/DOMAdapter";
		public const string DOM_ADAPTER_SIG = "Lorg/apache/xalan/xsltc/dom/DOMAdapter;";
		public const string MULTI_DOM_CLASS = "org.apache.xalan.xsltc.dom.MultiDOM";
		public const string MULTI_DOM_SIG = "Lorg/apache/xalan/xsltc/dom/MultiDOM;";
		public const string STRING = "java.lang.String";
		public static readonly int ACC_PUBLIC = org.apache.bcel.Constants.ACC_PUBLIC;
		public static readonly int ACC_SUPER = org.apache.bcel.Constants.ACC_SUPER;
		public static readonly int ACC_FINAL = org.apache.bcel.Constants.ACC_FINAL;
		public static readonly int ACC_PRIVATE = org.apache.bcel.Constants.ACC_PRIVATE;
		public static readonly int ACC_PROTECTED = org.apache.bcel.Constants.ACC_PROTECTED;
		public static readonly int ACC_STATIC = org.apache.bcel.Constants.ACC_STATIC;
		public const string STRING_SIG = "Ljava/lang/String;";
		public const string STRING_BUFFER_SIG = "Ljava/lang/StringBuffer;";
		public const string OBJECT_SIG = "Ljava/lang/Object;";
		public const string DOUBLE_SIG = "Ljava/lang/Double;";
		public const string INTEGER_SIG = "Ljava/lang/Integer;";
		public const string COLLATOR_CLASS = "java/text/Collator";
		public const string COLLATOR_SIG = "Ljava/text/Collator;";
		public const string NODE = "int";
		public const string NODE_ITERATOR = "org.apache.xml.dtm.DTMAxisIterator";
		public const string NODE_ITERATOR_BASE = "org.apache.xml.dtm.ref.DTMAxisIteratorBase";
		public const string SORT_ITERATOR = "org.apache.xalan.xsltc.dom.SortingIterator";
		public const string SORT_ITERATOR_SIG = "Lorg.apache.xalan.xsltc.dom.SortingIterator;";
		public const string NODE_SORT_RECORD = "org.apache.xalan.xsltc.dom.NodeSortRecord";
		public const string NODE_SORT_FACTORY = "org/apache/xalan/xsltc/dom/NodeSortRecordFactory";
		public const string NODE_SORT_RECORD_SIG = "Lorg/apache/xalan/xsltc/dom/NodeSortRecord;";
		public const string NODE_SORT_FACTORY_SIG = "Lorg/apache/xalan/xsltc/dom/NodeSortRecordFactory;";
		public const string LOCALE_CLASS = "java.util.Locale";
		public const string LOCALE_SIG = "Ljava/util/Locale;";
		public const string STRING_VALUE_HANDLER = "org.apache.xalan.xsltc.runtime.StringValueHandler";
		public const string STRING_VALUE_HANDLER_SIG = "Lorg/apache/xalan/xsltc/runtime/StringValueHandler;";
		public static readonly string OUTPUT_HANDLER = SerializerBase.PKG_PATH + "/SerializationHandler";
		public static readonly string OUTPUT_HANDLER_SIG = "L" + SerializerBase.PKG_PATH + "/SerializationHandler;";
		public const string FILTER_INTERFACE = "org.apache.xalan.xsltc.dom.Filter";
		public const string FILTER_INTERFACE_SIG = "Lorg/apache/xalan/xsltc/dom/Filter;";
		public const string UNION_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.UnionIterator";
		public const string STEP_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.StepIterator";
		public const string CACHED_NODE_LIST_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.CachedNodeListIterator";
		public const string NTH_ITERATOR_CLASS = "org.apache.xalan.xsltc.dom.NthIterator";
		public const string ABSOLUTE_ITERATOR = "org.apache.xalan.xsltc.dom.AbsoluteIterator";
		public const string DUP_FILTERED_ITERATOR = "org.apache.xalan.xsltc.dom.DupFilterIterator";
		public const string CURRENT_NODE_LIST_ITERATOR = "org.apache.xalan.xsltc.dom.CurrentNodeListIterator";
		public const string CURRENT_NODE_LIST_FILTER = "org.apache.xalan.xsltc.dom.CurrentNodeListFilter";
		public const string CURRENT_NODE_LIST_ITERATOR_SIG = "Lorg/apache/xalan/xsltc/dom/CurrentNodeListIterator;";
		public const string CURRENT_NODE_LIST_FILTER_SIG = "Lorg/apache/xalan/xsltc/dom/CurrentNodeListFilter;";
		public const string FILTER_STEP_ITERATOR = "org.apache.xalan.xsltc.dom.FilteredStepIterator";
		public const string FILTER_ITERATOR = "org.apache.xalan.xsltc.dom.FilterIterator";
		public const string SINGLETON_ITERATOR = "org.apache.xalan.xsltc.dom.SingletonIterator";
		public const string MATCHING_ITERATOR = "org.apache.xalan.xsltc.dom.MatchingIterator";
		public const string NODE_SIG = "I";
		public const string GET_PARENT = "getParent";
		public static readonly string GET_PARENT_SIG = "(" + NODE_SIG + ")" + NODE_SIG;
		public static readonly string NEXT_SIG = "()" + NODE_SIG;
		public const string NEXT = "next";
		public const string NEXTID = "nextNodeID";
		public const string MAKE_NODE = "makeNode";
		public const string MAKE_NODE_LIST = "makeNodeList";
		public const string GET_UNPARSED_ENTITY_URI = "getUnparsedEntityURI";
		public const string STRING_TO_REAL = "stringToReal";
		public static readonly string STRING_TO_REAL_SIG = "(" + STRING_SIG + ")D";
		public const string STRING_TO_INT = "stringToInt";
		public static readonly string STRING_TO_INT_SIG = "(" + STRING_SIG + ")I";
		public const string XSLT_PACKAGE = "org.apache.xalan.xsltc";
		public static readonly string COMPILER_PACKAGE = XSLT_PACKAGE + ".compiler";
		public static readonly string RUNTIME_PACKAGE = XSLT_PACKAGE + ".runtime";
		public static readonly string TRANSLET_CLASS = RUNTIME_PACKAGE + ".AbstractTranslet";
		public const string TRANSLET_SIG = "Lorg/apache/xalan/xsltc/runtime/AbstractTranslet;";
		public const string UNION_ITERATOR_SIG = "Lorg/apache/xalan/xsltc/dom/UnionIterator;";
		public static readonly string TRANSLET_OUTPUT_SIG = "L" + SerializerBase.PKG_PATH + "/SerializationHandler;";
		public const string MAKE_NODE_SIG = "(I)Lorg/w3c/dom/Node;";
		public static readonly string MAKE_NODE_SIG2 = "(" + NODE_ITERATOR_SIG + ")Lorg/w3c/dom/Node;";
		public const string MAKE_NODE_LIST_SIG = "(I)Lorg/w3c/dom/NodeList;";
		public static readonly string MAKE_NODE_LIST_SIG2 = "(" + NODE_ITERATOR_SIG + ")Lorg/w3c/dom/NodeList;";
		public static readonly string STREAM_XML_OUTPUT = SerializerBase.PKG_NAME + ".ToXMLStream";
		public static readonly string OUTPUT_BASE = SerializerBase.PKG_NAME + ".SerializerBase";
		public const string LOAD_DOCUMENT_CLASS = "org.apache.xalan.xsltc.dom.LoadDocument";
		public const string KEY_INDEX_CLASS = "org/apache/xalan/xsltc/dom/KeyIndex";
		public const string KEY_INDEX_SIG = "Lorg/apache/xalan/xsltc/dom/KeyIndex;";
		public const string KEY_INDEX_ITERATOR_SIG = "Lorg/apache/xalan/xsltc/dom/KeyIndex$KeyIndexIterator;";
		public const string DOM_INTF = "org.apache.xalan.xsltc.DOM";
		public const string DOM_IMPL = "org.apache.xalan.xsltc.dom.SAXImpl";
		public const string SAX_IMPL = "org.apache.xalan.xsltc.dom.SAXImpl";
		public const string STRING_CLASS = "java.lang.String";
		public const string OBJECT_CLASS = "java.lang.Object";
		public const string BOOLEAN_CLASS = "java.lang.Boolean";
		public const string STRING_BUFFER_CLASS = "java.lang.StringBuffer";
		public const string STRING_WRITER = "java.io.StringWriter";
		public const string WRITER_SIG = "Ljava/io/Writer;";
		public const string TRANSLET_OUTPUT_BASE = "org.apache.xalan.xsltc.TransletOutputBase";
		public static readonly string TRANSLET_OUTPUT_INTERFACE = SerializerBase.PKG_NAME + ".SerializationHandler";
		public const string BASIS_LIBRARY_CLASS = "org.apache.xalan.xsltc.runtime.BasisLibrary";
		public const string ATTRIBUTE_LIST_IMPL_CLASS = "org.apache.xalan.xsltc.runtime.AttributeListImpl";
		public const string DOUBLE_CLASS = "java.lang.Double";
		public const string INTEGER_CLASS = "java.lang.Integer";
		public const string RUNTIME_NODE_CLASS = "org.apache.xalan.xsltc.runtime.Node";
		public const string MATH_CLASS = "java.lang.Math";
		public const string BOOLEAN_VALUE = "booleanValue";
		public const string BOOLEAN_VALUE_SIG = "()Z";
		public const string INT_VALUE = "intValue";
		public const string INT_VALUE_SIG = "()I";
		public const string DOUBLE_VALUE = "doubleValue";
		public const string DOUBLE_VALUE_SIG = "()D";
		public const string DOM_PNAME = "dom";
		public const string NODE_PNAME = "node";
		public const string TRANSLET_OUTPUT_PNAME = "handler";
		public const string ITERATOR_PNAME = "iterator";
		public const string DOCUMENT_PNAME = "document";
		public const string TRANSLET_PNAME = "translet";
		public const string INVOKE_METHOD = "invokeMethod";
		public const string GET_NODE_NAME = "getNodeNameX";
		public const string CHARACTERSW = "characters";
		public const string GET_CHILDREN = "getChildren";
		public const string GET_TYPED_CHILDREN = "getTypedChildren";
		public const string CHARACTERS = "characters";
		public const string APPLY_TEMPLATES = "applyTemplates";
		public const string GET_NODE_TYPE = "getNodeType";
		public const string GET_NODE_VALUE = "getStringValueX";
		public const string GET_ELEMENT_VALUE = "getElementValue";
		public const string GET_ATTRIBUTE_VALUE = "getAttributeValue";
		public const string HAS_ATTRIBUTE = "hasAttribute";
		public const string ADD_ITERATOR = "addIterator";
		public const string SET_START_NODE = "setStartNode";
		public const string RESET = "reset";
		public static readonly string ATTR_SET_SIG = "(" + DOM_INTF_SIG + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + ")V";
		public static readonly string GET_NODE_NAME_SIG = "(" + NODE_SIG + ")" + STRING_SIG;
		public static readonly string CHARACTERSW_SIG = "(" + STRING_SIG + TRANSLET_OUTPUT_SIG + ")V";
		public static readonly string CHARACTERS_SIG = "(" + NODE_SIG + TRANSLET_OUTPUT_SIG + ")V";
		public static readonly string GET_CHILDREN_SIG = "(" + NODE_SIG + ")" + NODE_ITERATOR_SIG;
		public static readonly string GET_TYPED_CHILDREN_SIG = "(I)" + NODE_ITERATOR_SIG;
		public const string GET_NODE_TYPE_SIG = "()S";
		public static readonly string GET_NODE_VALUE_SIG = "(I)" + STRING_SIG;
		public static readonly string GET_ELEMENT_VALUE_SIG = "(I)" + STRING_SIG;
		public static readonly string GET_ATTRIBUTE_VALUE_SIG = "(II)" + STRING_SIG;
		public const string HAS_ATTRIBUTE_SIG = "(II)Z";
		public static readonly string GET_ITERATOR_SIG = "()" + NODE_ITERATOR_SIG;
		public const string NAMES_INDEX = "namesArray";
		public static readonly string NAMES_INDEX_SIG = "[" + STRING_SIG;
		public const string URIS_INDEX = "urisArray";
		public static readonly string URIS_INDEX_SIG = "[" + STRING_SIG;
		public const string TYPES_INDEX = "typesArray";
		public const string TYPES_INDEX_SIG = "[I";
		public const string NAMESPACE_INDEX = "namespaceArray";
		public static readonly string NAMESPACE_INDEX_SIG = "[" + STRING_SIG;
		public const string NS_ANCESTORS_INDEX_SIG = "[I";
		public const string PREFIX_URIS_IDX_SIG = "[I";
		public static readonly string PREFIX_URIS_ARRAY_SIG = "[" + STRING_SIG;
		public const string HASIDCALL_INDEX = "_hasIdCall";
		public const string HASIDCALL_INDEX_SIG = "Z";
		public const string TRANSLET_VERSION_INDEX = "transletVersion";
		public const string TRANSLET_VERSION_INDEX_SIG = "I";
		public const string LOOKUP_STYLESHEET_QNAME_NS_REF = "lookupStylesheetQNameNamespace";
		public static readonly string LOOKUP_STYLESHEET_QNAME_NS_SIG = "(" + STRING_SIG + "I" + NS_ANCESTORS_INDEX_SIG + PREFIX_URIS_IDX_SIG + PREFIX_URIS_ARRAY_SIG + "Z)" + STRING_SIG;
		public const string EXPAND_STYLESHEET_QNAME_REF = "expandStylesheetQNameRef";
		public static readonly string EXPAND_STYLESHEET_QNAME_SIG = "(" + STRING_SIG + "I" + NS_ANCESTORS_INDEX_SIG + PREFIX_URIS_IDX_SIG + PREFIX_URIS_ARRAY_SIG + "Z)" + STRING_SIG;
		public const string DOM_FIELD = "_dom";
		public const string STATIC_NAMES_ARRAY_FIELD = "_sNamesArray";
		public const string STATIC_URIS_ARRAY_FIELD = "_sUrisArray";
		public const string STATIC_TYPES_ARRAY_FIELD = "_sTypesArray";
		public const string STATIC_NAMESPACE_ARRAY_FIELD = "_sNamespaceArray";
		public const string STATIC_NS_ANCESTORS_ARRAY_FIELD = "_sNamespaceAncestorsArray";
		public const string STATIC_PREFIX_URIS_IDX_ARRAY_FIELD = "_sPrefixURIsIdxArray";
		public const string STATIC_PREFIX_URIS_ARRAY_FIELD = "_sPrefixURIPairsArray";
		public const string STATIC_CHAR_DATA_FIELD = "_scharData";
		public const string STATIC_CHAR_DATA_FIELD_SIG = "[C";
		public const string FORMAT_SYMBOLS_FIELD = "format_symbols";
		public const string ITERATOR_FIELD_SIG = NODE_ITERATOR_SIG;
		public const string NODE_FIELD = "node";
		public const string NODE_FIELD_SIG = "I";
		public const string EMPTYATTR_FIELD = "EmptyAttributes";
		public const string ATTRIBUTE_LIST_FIELD = "attributeList";
		public const string CLEAR_ATTRIBUTES = "clear";
		public const string ADD_ATTRIBUTE = "addAttribute";
		public const string ATTRIBUTE_LIST_IMPL_SIG = "Lorg/apache/xalan/xsltc/runtime/AttributeListImpl;";
		public static readonly string CLEAR_ATTRIBUTES_SIG = "()" + ATTRIBUTE_LIST_IMPL_SIG;
		public static readonly string ADD_ATTRIBUTE_SIG = "(" + STRING_SIG + STRING_SIG + ")" + ATTRIBUTE_LIST_IMPL_SIG;
		public static readonly string ADD_ITERATOR_SIG = "(" + NODE_ITERATOR_SIG + ")" + UNION_ITERATOR_SIG;
		public const string ORDER_ITERATOR = "orderNodes";
		public static readonly string ORDER_ITERATOR_SIG = "(" + NODE_ITERATOR_SIG + "I)" + NODE_ITERATOR_SIG;
		public static readonly string SET_START_NODE_SIG = "(" + NODE_SIG + ")" + NODE_ITERATOR_SIG;
		public const string NODE_COUNTER = "org.apache.xalan.xsltc.dom.NodeCounter";
		public const string NODE_COUNTER_SIG = "Lorg/apache/xalan/xsltc/dom/NodeCounter;";
		public const string DEFAULT_NODE_COUNTER = "org.apache.xalan.xsltc.dom.DefaultNodeCounter";
		public const string DEFAULT_NODE_COUNTER_SIG = "Lorg/apache/xalan/xsltc/dom/DefaultNodeCounter;";
		public const string TRANSLET_FIELD = "translet";
		public const string TRANSLET_FIELD_SIG = TRANSLET_SIG;
		public static readonly string RESET_SIG = "()" + NODE_ITERATOR_SIG;
		public const string GET_PARAMETER = "getParameter";
		public const string ADD_PARAMETER = "addParameter";
		public const string PUSH_PARAM_FRAME = "pushParamFrame";
		public const string PUSH_PARAM_FRAME_SIG = "()V";
		public const string POP_PARAM_FRAME = "popParamFrame";
		public const string POP_PARAM_FRAME_SIG = "()V";
		public static readonly string GET_PARAMETER_SIG = "(" + STRING_SIG + ")" + OBJECT_SIG;
		public static readonly string ADD_PARAMETER_SIG = "(" + STRING_SIG + OBJECT_SIG + "Z)" + OBJECT_SIG;
		public const string STRIP_SPACE = "stripSpace";
		public const string STRIP_SPACE_INTF = "org/apache/xalan/xsltc/StripFilter";
		public const string STRIP_SPACE_SIG = "Lorg/apache/xalan/xsltc/StripFilter;";
		public const string STRIP_SPACE_PARAMS = "(Lorg/apache/xalan/xsltc/DOM;II)Z";
		public const string GET_NODE_VALUE_ITERATOR = "getNodeValueIterator";
		public static readonly string GET_NODE_VALUE_ITERATOR_SIG = "(" + NODE_ITERATOR_SIG + "I" + STRING_SIG + "Z)" + NODE_ITERATOR_SIG;
		public static readonly string GET_UNPARSED_ENTITY_URI_SIG = "(" + STRING_SIG + ")" + STRING_SIG;
		public const int POSITION_INDEX = 2;
		public const int LAST_INDEX = 3;
		public const string XMLNS_PREFIX = "xmlns";
		public const string XMLNS_STRING = "xmlns:";
		public const string XMLNS_URI = "http://www.w3.org/2000/xmlns/";
		public const string XSLT_URI = "http://www.w3.org/1999/XSL/Transform";
		public const string XHTML_URI = "http://www.w3.org/1999/xhtml";
		public const string TRANSLET_URI = "http://xml.apache.org/xalan/xsltc";
		public const string REDIRECT_URI = "http://xml.apache.org/xalan/redirect";
		public const string FALLBACK_CLASS = "org.apache.xalan.xsltc.compiler.Fallback";
		public const int RTF_INITIAL_SIZE = 32;
	}

}