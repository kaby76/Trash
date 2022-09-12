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
 * $Id: XPATHErrorResources_ja.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.res
{


	/// <summary>
	/// Set up error messages.
	/// We build a two dimensional array of message keys and
	/// message strings. In order to add a new message here,
	/// you need to first add a Static string constant for the
	/// Key and update the contents array with Key, Value pair
	/// Also you need to  update the count of messages(MAX_CODE)or
	/// the count of warnings(MAX_WARNING) [ Information purpose only]
	/// @xsl.usage advanced
	/// </summary>
	public class XPATHErrorResources_ja : ListResourceBundle
	{

	/*
	 * General notes to translators:
	 *
	 * This file contains error and warning messages related to XPath Error
	 * Handling.
	 *
	 *  1) Xalan (or more properly, Xalan-interpretive) and XSLTC are names of
	 *     components.
	 *     XSLT is an acronym for "XML Stylesheet Language: Transformations".
	 *     XSLTC is an acronym for XSLT Compiler.
	 *
	 *  2) A stylesheet is a description of how to transform an input XML document
	 *     into a resultant XML document (or HTML document or text).  The
	 *     stylesheet itself is described in the form of an XML document.
	 *
	 *  3) A template is a component of a stylesheet that is used to match a
	 *     particular portion of an input document and specifies the form of the
	 *     corresponding portion of the output document.
	 *
	 *  4) An element is a mark-up tag in an XML document; an attribute is a
	 *     modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *     "elem" is an element name, "attr" and "attr2" are attribute names with
	 *     the values "val" and "val2", respectively.
	 *
	 *  5) A namespace declaration is a special attribute that is used to associate
	 *     a prefix with a URI (the namespace).  The meanings of element names and
	 *     attribute names that use that prefix are defined with respect to that
	 *     namespace.
	 *
	 *  6) "Translet" is an invented term that describes the class file that
	 *     results from compiling an XML stylesheet into a Java class.
	 *
	 *  7) XPath is a specification that describes a notation for identifying
	 *     nodes in a tree-structured representation of an XML document.  An
	 *     instance of that notation is referred to as an XPath expression.
	 *
	 *  8) The context node is the node in the document with respect to which an
	 *     XPath expression is being evaluated.
	 *
	 *  9) An iterator is an object that traverses nodes in the tree, one at a time.
	 *
	 *  10) NCName is an XML term used to describe a name that does not contain a
	 *     colon (a "no-colon name").
	 *
	 *  11) QName is an XML term meaning "qualified name".
	 */

	  /*
	   * static variables
	   */
	  public const string ERROR0000 = "ERROR0000";
	  public const string ER_CURRENT_NOT_ALLOWED_IN_MATCH = "ER_CURRENT_NOT_ALLOWED_IN_MATCH";
	  public const string ER_CURRENT_TAKES_NO_ARGS = "ER_CURRENT_TAKES_NO_ARGS";
	  public const string ER_DOCUMENT_REPLACED = "ER_DOCUMENT_REPLACED";
	  public const string ER_CONTEXT_HAS_NO_OWNERDOC = "ER_CONTEXT_HAS_NO_OWNERDOC";
	  public const string ER_LOCALNAME_HAS_TOO_MANY_ARGS = "ER_LOCALNAME_HAS_TOO_MANY_ARGS";
	  public const string ER_NAMESPACEURI_HAS_TOO_MANY_ARGS = "ER_NAMESPACEURI_HAS_TOO_MANY_ARGS";
	  public const string ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS = "ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS";
	  public const string ER_NUMBER_HAS_TOO_MANY_ARGS = "ER_NUMBER_HAS_TOO_MANY_ARGS";
	  public const string ER_NAME_HAS_TOO_MANY_ARGS = "ER_NAME_HAS_TOO_MANY_ARGS";
	  public const string ER_STRING_HAS_TOO_MANY_ARGS = "ER_STRING_HAS_TOO_MANY_ARGS";
	  public const string ER_STRINGLENGTH_HAS_TOO_MANY_ARGS = "ER_STRINGLENGTH_HAS_TOO_MANY_ARGS";
	  public const string ER_TRANSLATE_TAKES_3_ARGS = "ER_TRANSLATE_TAKES_3_ARGS";
	  public const string ER_UNPARSEDENTITYURI_TAKES_1_ARG = "ER_UNPARSEDENTITYURI_TAKES_1_ARG";
	  public const string ER_NAMESPACEAXIS_NOT_IMPLEMENTED = "ER_NAMESPACEAXIS_NOT_IMPLEMENTED";
	  public const string ER_UNKNOWN_AXIS = "ER_UNKNOWN_AXIS";
	  public const string ER_UNKNOWN_MATCH_OPERATION = "ER_UNKNOWN_MATCH_OPERATION";
	  public const string ER_INCORRECT_ARG_LENGTH = "ER_INCORRECT_ARG_LENGTH";
	  public const string ER_CANT_CONVERT_TO_NUMBER = "ER_CANT_CONVERT_TO_NUMBER";
	  public const string ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER = "ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER";
	  public const string ER_CANT_CONVERT_TO_NODELIST = "ER_CANT_CONVERT_TO_NODELIST";
	  public const string ER_CANT_CONVERT_TO_MUTABLENODELIST = "ER_CANT_CONVERT_TO_MUTABLENODELIST";
	  public const string ER_CANT_CONVERT_TO_TYPE = "ER_CANT_CONVERT_TO_TYPE";
	  public const string ER_EXPECTED_MATCH_PATTERN = "ER_EXPECTED_MATCH_PATTERN";
	  public const string ER_COULDNOT_GET_VAR_NAMED = "ER_COULDNOT_GET_VAR_NAMED";
	  public const string ER_UNKNOWN_OPCODE = "ER_UNKNOWN_OPCODE";
	  public const string ER_EXTRA_ILLEGAL_TOKENS = "ER_EXTRA_ILLEGAL_TOKENS";
	  public const string ER_EXPECTED_DOUBLE_QUOTE = "ER_EXPECTED_DOUBLE_QUOTE";
	  public const string ER_EXPECTED_SINGLE_QUOTE = "ER_EXPECTED_SINGLE_QUOTE";
	  public const string ER_EMPTY_EXPRESSION = "ER_EMPTY_EXPRESSION";
	  public const string ER_EXPECTED_BUT_FOUND = "ER_EXPECTED_BUT_FOUND";
	  public const string ER_INCORRECT_PROGRAMMER_ASSERTION = "ER_INCORRECT_PROGRAMMER_ASSERTION";
	  public const string ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL = "ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL";
	  public const string ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG = "ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG";
	  public const string ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG = "ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG";
	  public const string ER_PREDICATE_ILLEGAL_SYNTAX = "ER_PREDICATE_ILLEGAL_SYNTAX";
	  public const string ER_ILLEGAL_AXIS_NAME = "ER_ILLEGAL_AXIS_NAME";
	  public const string ER_UNKNOWN_NODETYPE = "ER_UNKNOWN_NODETYPE";
	  public const string ER_PATTERN_LITERAL_NEEDS_BE_QUOTED = "ER_PATTERN_LITERAL_NEEDS_BE_QUOTED";
	  public const string ER_COULDNOT_BE_FORMATTED_TO_NUMBER = "ER_COULDNOT_BE_FORMATTED_TO_NUMBER";
	  public const string ER_COULDNOT_CREATE_XMLPROCESSORLIAISON = "ER_COULDNOT_CREATE_XMLPROCESSORLIAISON";
	  public const string ER_DIDNOT_FIND_XPATH_SELECT_EXP = "ER_DIDNOT_FIND_XPATH_SELECT_EXP";
	  public const string ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH = "ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH";
	  public const string ER_ERROR_OCCURED = "ER_ERROR_OCCURED";
	  public const string ER_ILLEGAL_VARIABLE_REFERENCE = "ER_ILLEGAL_VARIABLE_REFERENCE";
	  public const string ER_AXES_NOT_ALLOWED = "ER_AXES_NOT_ALLOWED";
	  public const string ER_KEY_HAS_TOO_MANY_ARGS = "ER_KEY_HAS_TOO_MANY_ARGS";
	  public const string ER_COUNT_TAKES_1_ARG = "ER_COUNT_TAKES_1_ARG";
	  public const string ER_COULDNOT_FIND_FUNCTION = "ER_COULDNOT_FIND_FUNCTION";
	  public const string ER_UNSUPPORTED_ENCODING = "ER_UNSUPPORTED_ENCODING";
	  public const string ER_PROBLEM_IN_DTM_NEXTSIBLING = "ER_PROBLEM_IN_DTM_NEXTSIBLING";
	  public const string ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL = "ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL";
	  public const string ER_SETDOMFACTORY_NOT_SUPPORTED = "ER_SETDOMFACTORY_NOT_SUPPORTED";
	  public const string ER_PREFIX_MUST_RESOLVE = "ER_PREFIX_MUST_RESOLVE";
	  public const string ER_PARSE_NOT_SUPPORTED = "ER_PARSE_NOT_SUPPORTED";
	  public const string ER_SAX_API_NOT_HANDLED = "ER_SAX_API_NOT_HANDLED";
	public const string ER_IGNORABLE_WHITESPACE_NOT_HANDLED = "ER_IGNORABLE_WHITESPACE_NOT_HANDLED";
	  public const string ER_DTM_CANNOT_HANDLE_NODES = "ER_DTM_CANNOT_HANDLE_NODES";
	  public const string ER_XERCES_CANNOT_HANDLE_NODES = "ER_XERCES_CANNOT_HANDLE_NODES";
	  public const string ER_XERCES_PARSE_ERROR_DETAILS = "ER_XERCES_PARSE_ERROR_DETAILS";
	  public const string ER_XERCES_PARSE_ERROR = "ER_XERCES_PARSE_ERROR";
	  public const string ER_INVALID_UTF16_SURROGATE = "ER_INVALID_UTF16_SURROGATE";
	  public const string ER_OIERROR = "ER_OIERROR";
	  public const string ER_CANNOT_CREATE_URL = "ER_CANNOT_CREATE_URL";
	  public const string ER_XPATH_READOBJECT = "ER_XPATH_READOBJECT";
	 public const string ER_FUNCTION_TOKEN_NOT_FOUND = "ER_FUNCTION_TOKEN_NOT_FOUND";
	  public const string ER_CANNOT_DEAL_XPATH_TYPE = "ER_CANNOT_DEAL_XPATH_TYPE";
	  public const string ER_NODESET_NOT_MUTABLE = "ER_NODESET_NOT_MUTABLE";
	  public const string ER_NODESETDTM_NOT_MUTABLE = "ER_NODESETDTM_NOT_MUTABLE";
	   /// <summary>
	   ///  Variable not resolvable: </summary>
	  public const string ER_VAR_NOT_RESOLVABLE = "ER_VAR_NOT_RESOLVABLE";
	   /// <summary>
	   /// Null error handler </summary>
	 public const string ER_NULL_ERROR_HANDLER = "ER_NULL_ERROR_HANDLER";
	   /// <summary>
	   ///  Programmer's assertion: unknown opcode </summary>
	  public const string ER_PROG_ASSERT_UNKNOWN_OPCODE = "ER_PROG_ASSERT_UNKNOWN_OPCODE";
	   /// <summary>
	   ///  0 or 1 </summary>
	  public const string ER_ZERO_OR_ONE = "ER_ZERO_OR_ONE";
	   /// <summary>
	   ///  rtf() not supported by XRTreeFragSelectWrapper </summary>
	  public const string ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER = "ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER";
	   /// <summary>
	   ///  asNodeIterator() not supported by XRTreeFragSelectWrapper </summary>
	  public const string ER_ASNODEITERATOR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER = "ER_ASNODEITERATOR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER";
	   /// <summary>
	   ///  fsb() not supported for XStringForChars </summary>
	  public const string ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS = "ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS";
	   /// <summary>
	   ///  Could not find variable with the name of </summary>
	 public const string ER_COULD_NOT_FIND_VAR = "ER_COULD_NOT_FIND_VAR";
	   /// <summary>
	   ///  XStringForChars can not take a string for an argument </summary>
	 public const string ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING = "ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING";
	   /// <summary>
	   ///  The FastStringBuffer argument can not be null </summary>
	 public const string ER_FASTSTRINGBUFFER_CANNOT_BE_NULL = "ER_FASTSTRINGBUFFER_CANNOT_BE_NULL";
	   /// <summary>
	   ///  2 or 3 </summary>
	  public const string ER_TWO_OR_THREE = "ER_TWO_OR_THREE";
	   /// <summary>
	   /// Variable accessed before it is bound! </summary>
	  public const string ER_VARIABLE_ACCESSED_BEFORE_BIND = "ER_VARIABLE_ACCESSED_BEFORE_BIND";
	   /// <summary>
	   /// XStringForFSB can not take a string for an argument! </summary>
	 public const string ER_FSB_CANNOT_TAKE_STRING = "ER_FSB_CANNOT_TAKE_STRING";
	   /// <summary>
	   /// Error! Setting the root of a walker to null! </summary>
	  public const string ER_SETTING_WALKER_ROOT_TO_NULL = "ER_SETTING_WALKER_ROOT_TO_NULL";
	   /// <summary>
	   /// This NodeSetDTM can not iterate to a previous node! </summary>
	  public const string ER_NODESETDTM_CANNOT_ITERATE = "ER_NODESETDTM_CANNOT_ITERATE";
	  /// <summary>
	  /// This NodeSet can not iterate to a previous node! </summary>
	 public const string ER_NODESET_CANNOT_ITERATE = "ER_NODESET_CANNOT_ITERATE";
	  /// <summary>
	  /// This NodeSetDTM can not do indexing or counting functions! </summary>
	  public const string ER_NODESETDTM_CANNOT_INDEX = "ER_NODESETDTM_CANNOT_INDEX";
	  /// <summary>
	  /// This NodeSet can not do indexing or counting functions! </summary>
	  public const string ER_NODESET_CANNOT_INDEX = "ER_NODESET_CANNOT_INDEX";
	  /// <summary>
	  /// Can not call setShouldCacheNodes after nextNode has been called! </summary>
	  public const string ER_CANNOT_CALL_SETSHOULDCACHENODE = "ER_CANNOT_CALL_SETSHOULDCACHENODE";
	  /// <summary>
	  /// {0} only allows {1} arguments </summary>
	 public const string ER_ONLY_ALLOWS = "ER_ONLY_ALLOWS";
	  /// <summary>
	  /// Programmer's assertion in getNextStepPos: unknown stepType: {0} </summary>
	  public const string ER_UNKNOWN_STEP = "ER_UNKNOWN_STEP";
	  /// <summary>
	  /// Problem with RelativeLocationPath </summary>
	  public const string ER_EXPECTED_REL_LOC_PATH = "ER_EXPECTED_REL_LOC_PATH";
	  /// <summary>
	  /// Problem with LocationPath </summary>
	  public const string ER_EXPECTED_LOC_PATH = "ER_EXPECTED_LOC_PATH";
	  public const string ER_EXPECTED_LOC_PATH_AT_END_EXPR = "ER_EXPECTED_LOC_PATH_AT_END_EXPR";
	  /// <summary>
	  /// Problem with Step </summary>
	  public const string ER_EXPECTED_LOC_STEP = "ER_EXPECTED_LOC_STEP";
	  /// <summary>
	  /// Problem with NodeTest </summary>
	  public const string ER_EXPECTED_NODE_TEST = "ER_EXPECTED_NODE_TEST";
	  /// <summary>
	  /// Expected step pattern </summary>
	  public const string ER_EXPECTED_STEP_PATTERN = "ER_EXPECTED_STEP_PATTERN";
	  /// <summary>
	  /// Expected relative path pattern </summary>
	  public const string ER_EXPECTED_REL_PATH_PATTERN = "ER_EXPECTED_REL_PATH_PATTERN";
	  /// <summary>
	  /// ER_CANT_CONVERT_XPATHRESULTTYPE_TO_BOOLEAN </summary>
	  public const string ER_CANT_CONVERT_TO_BOOLEAN = "ER_CANT_CONVERT_TO_BOOLEAN";
	  /// <summary>
	  /// Field ER_CANT_CONVERT_TO_SINGLENODE </summary>
	  public const string ER_CANT_CONVERT_TO_SINGLENODE = "ER_CANT_CONVERT_TO_SINGLENODE";
	  /// <summary>
	  /// Field ER_CANT_GET_SNAPSHOT_LENGTH </summary>
	  public const string ER_CANT_GET_SNAPSHOT_LENGTH = "ER_CANT_GET_SNAPSHOT_LENGTH";
	  /// <summary>
	  /// Field ER_NON_ITERATOR_TYPE </summary>
	  public const string ER_NON_ITERATOR_TYPE = "ER_NON_ITERATOR_TYPE";
	  /// <summary>
	  /// Field ER_DOC_MUTATED </summary>
	  public const string ER_DOC_MUTATED = "ER_DOC_MUTATED";
	  public const string ER_INVALID_XPATH_TYPE = "ER_INVALID_XPATH_TYPE";
	  public const string ER_EMPTY_XPATH_RESULT = "ER_EMPTY_XPATH_RESULT";
	  public const string ER_INCOMPATIBLE_TYPES = "ER_INCOMPATIBLE_TYPES";
	  public const string ER_NULL_RESOLVER = "ER_NULL_RESOLVER";
	  public const string ER_CANT_CONVERT_TO_STRING = "ER_CANT_CONVERT_TO_STRING";
	  public const string ER_NON_SNAPSHOT_TYPE = "ER_NON_SNAPSHOT_TYPE";
	  public const string ER_WRONG_DOCUMENT = "ER_WRONG_DOCUMENT";
	  /* Note to translators:  The XPath expression cannot be evaluated with respect
	   * to this type of node.
	   */
	  /// <summary>
	  /// Field ER_WRONG_NODETYPE </summary>
	  public const string ER_WRONG_NODETYPE = "ER_WRONG_NODETYPE";
	  public const string ER_XPATH_ERROR = "ER_XPATH_ERROR";

	  //BEGIN: Keys needed for exception messages of  JAXP 1.3 XPath API implementation
	  public const string ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED = "ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED";
	  public const string ER_RESOLVE_VARIABLE_RETURNS_NULL = "ER_RESOLVE_VARIABLE_RETURNS_NULL";
	  public const string ER_UNSUPPORTED_RETURN_TYPE = "ER_UNSUPPORTED_RETURN_TYPE";
	  public const string ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL = "ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL";
	  public const string ER_ARG_CANNOT_BE_NULL = "ER_ARG_CANNOT_BE_NULL";

	  public const string ER_OBJECT_MODEL_NULL = "ER_OBJECT_MODEL_NULL";
	  public const string ER_OBJECT_MODEL_EMPTY = "ER_OBJECT_MODEL_EMPTY";
	  public const string ER_FEATURE_NAME_NULL = "ER_FEATURE_NAME_NULL";
	  public const string ER_FEATURE_UNKNOWN = "ER_FEATURE_UNKNOWN";
	  public const string ER_GETTING_NULL_FEATURE = "ER_GETTING_NULL_FEATURE";
	  public const string ER_GETTING_UNKNOWN_FEATURE = "ER_GETTING_UNKNOWN_FEATURE";
	  public const string ER_NULL_XPATH_FUNCTION_RESOLVER = "ER_NULL_XPATH_FUNCTION_RESOLVER";
	  public const string ER_NULL_XPATH_VARIABLE_RESOLVER = "ER_NULL_XPATH_VARIABLE_RESOLVER";
	  //END: Keys needed for exception messages of  JAXP 1.3 XPath API implementation

	  public const string WG_LOCALE_NAME_NOT_HANDLED = "WG_LOCALE_NAME_NOT_HANDLED";
	  public const string WG_PROPERTY_NOT_SUPPORTED = "WG_PROPERTY_NOT_SUPPORTED";
	  public const string WG_DONT_DO_ANYTHING_WITH_NS = "WG_DONT_DO_ANYTHING_WITH_NS";
	  public const string WG_SECURITY_EXCEPTION = "WG_SECURITY_EXCEPTION";
	  public const string WG_QUO_NO_LONGER_DEFINED = "WG_QUO_NO_LONGER_DEFINED";
	  public const string WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST = "WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST";
	  public const string WG_FUNCTION_TOKEN_NOT_FOUND = "WG_FUNCTION_TOKEN_NOT_FOUND";
	  public const string WG_COULDNOT_FIND_FUNCTION = "WG_COULDNOT_FIND_FUNCTION";
	  public const string WG_CANNOT_MAKE_URL_FROM = "WG_CANNOT_MAKE_URL_FROM";
	  public const string WG_EXPAND_ENTITIES_NOT_SUPPORTED = "WG_EXPAND_ENTITIES_NOT_SUPPORTED";
	  public const string WG_ILLEGAL_VARIABLE_REFERENCE = "WG_ILLEGAL_VARIABLE_REFERENCE";
	  public const string WG_UNSUPPORTED_ENCODING = "WG_UNSUPPORTED_ENCODING";

	  /// <summary>
	  ///  detach() not supported by XRTreeFragSelectWrapper </summary>
	  public const string ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER = "ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER";
	  /// <summary>
	  ///  num() not supported by XRTreeFragSelectWrapper </summary>
	  public const string ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER = "ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER";
	  /// <summary>
	  ///  xstr() not supported by XRTreeFragSelectWrapper </summary>
	  public const string ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER = "ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER";
	  /// <summary>
	  ///  str() not supported by XRTreeFragSelectWrapper </summary>
	  public const string ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER = "ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER";

	  // Error messages...


	  /// <summary>
	  /// Get the association list.
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public virtual object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ERROR0000", "{0}"},
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "current() \u95a2\u6570\u306f\u30d1\u30bf\u30fc\u30f3\u306e\u30de\u30c3\u30c1\u30f3\u30b0\u3067\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "current() \u95a2\u6570\u306f\u5f15\u6570\u3092\u53d7\u3051\u5165\u308c\u307e\u305b\u3093\u3002"},
				new object[] {ER_DOCUMENT_REPLACED, "document() \u95a2\u6570\u306e\u5b9f\u88c5\u304c org.apache.xalan.xslt.FuncDocument \u306b\u3088\u308a\u7f6e\u304d\u63db\u3048\u3089\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u306b\u6240\u6709\u8005\u6587\u66f8\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() \u306e\u5f15\u6570\u304c\u591a\u3059\u304e\u307e\u3059\u3002"},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "translate() \u95a2\u6570\u306f 3 \u500b\u306e\u5f15\u6570\u3092\u4f7f\u7528\u3057\u307e\u3059\u3002"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "unparsed-entity-uri \u95a2\u6570\u306f\u5f15\u6570\u3092 1 \u500b\u4f7f\u7528\u3057\u307e\u3059\u3002"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "namespace axis \u304c\u307e\u3060\u5b9f\u88c5\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_UNKNOWN_AXIS, "\u4e0d\u660e\u306a\u8ef8: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "\u4e0d\u660e\u306e\u30de\u30c3\u30c1\u30f3\u30b0\u64cd\u4f5c\u3002"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "processing-instruction() \u306e\u30ce\u30fc\u30c9\u30fb\u30c6\u30b9\u30c8\u306e\u5f15\u6570\u306e\u9577\u3055\u304c\u8aa4\u3063\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "{0} \u3092\u6570\u306b\u5909\u63db\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "{0} \u3092 NodeList \u306b\u5909\u63db\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "{0} \u3092 NodeSetDTM \u306b\u5909\u63db\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "{0} \u3092 type#{1} \u306b\u5909\u63db\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "getMatchScore \u3067\u5fc5\u8981\u306a\u4e00\u81f4\u30d1\u30bf\u30fc\u30f3\u3067\u3059\u3002"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "{0} \u3068\u3044\u3046\u540d\u524d\u306e\u5909\u6570\u3092\u53d6\u5f97\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f"},
				new object[] {ER_UNKNOWN_OPCODE, "\u30a8\u30e9\u30fc:  \u4e0d\u660e\u306a\u547d\u4ee4\u30b3\u30fc\u30c9: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "\u4f59\u5206\u306e\u6b63\u3057\u304f\u306a\u3044\u30c8\u30fc\u30af\u30f3: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "\u5f15\u7528\u7b26\u304c\u8aa4\u3063\u3066\u3044\u308b\u30ea\u30c6\u30e9\u30eb... \u4e8c\u91cd\u5f15\u7528\u7b26\u304c\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "\u5f15\u7528\u7b26\u304c\u8aa4\u3063\u3066\u3044\u308b\u30ea\u30c6\u30e9\u30eb... \u5358\u4e00\u5f15\u7528\u7b26\u304c\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				new object[] {ER_EMPTY_EXPRESSION, "\u7a7a\u306e\u5f0f\u3067\u3059\u3002"},
				new object[] {ER_EXPECTED_BUT_FOUND, "{0} \u304c\u5fc5\u8981\u3067\u3057\u305f\u304c\u3001{1} \u304c\u898b\u3064\u304b\u308a\u307e\u3057\u305f"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "\u30d7\u30ed\u30b0\u30e9\u30de\u30fc\u306e\u30a2\u30b5\u30fc\u30b7\u30e7\u30f3\u304c\u8aa4\u3063\u3066\u3044\u307e\u3059\u3002 - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "\u30d6\u30fc\u30eb(...) \u5f15\u6570\u306f 19990709 XPath \u30c9\u30e9\u30d5\u30c8\u3067\u306f\u3082\u3046\u30aa\u30d7\u30b7\u30e7\u30f3\u3067\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "',' \u304c\u898b\u3064\u304b\u308a\u307e\u3057\u305f\u304c\u3001\u5148\u7acb\u3064\u5f15\u6570\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "',' \u304c\u898b\u3064\u304b\u308a\u307e\u3057\u305f\u304c\u3001\u5f8c\u7d9a\u306e\u5f15\u6570\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' \u307e\u305f\u306f '.[predicate]' \u306f\u6b63\u3057\u304f\u306a\u3044\u69cb\u6587\u3067\u3059\u3002\u4ee3\u308a\u306b  'self::node()[predicate]' \u3092\u4f7f\u7528\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {ER_ILLEGAL_AXIS_NAME, "\u6b63\u3057\u304f\u306a\u3044\u8ef8\u306e\u540d\u524d: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "\u4e0d\u660e\u306a\u30ce\u30fc\u30c9\u578b: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "\u30d1\u30bf\u30fc\u30f3\u30fb\u30ea\u30c6\u30e9\u30eb ({0}) \u306b\u306f\u5f15\u7528\u7b26\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} \u3092\u6570\u306b\u30d5\u30a9\u30fc\u30de\u30c3\u30c8\u8a2d\u5b9a\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "XML TransformerFactory Liaison \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "\u30a8\u30e9\u30fc:  xpath select \u5f0f (-select) \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "\u30a8\u30e9\u30fc:  OP_LOCATIONPATH \u306e\u5f8c\u306b ENDOP \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f"},
				new object[] {ER_ERROR_OCCURED, "\u30a8\u30e9\u30fc\u304c\u8d77\u3053\u308a\u307e\u3057\u305f\u3002"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "\u5909\u6570\u306b\u6307\u5b9a\u3055\u308c\u305f VariableReference \u304c\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u5916\u304b\u3001\u5b9a\u7fa9\u304c\u3042\u308a\u307e\u305b\u3093 \u540d\u524d = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "\u30de\u30c3\u30c1\u30f3\u30b0\u30fb\u30d1\u30bf\u30fc\u30f3\u3067\u8a31\u53ef\u3055\u308c\u3066\u3044\u308b\u306e\u306f child:: \u8ef8\u304a\u3088\u3073 attribute:: \u8ef8\u306e\u307f\u3067\u3059\u3002 \u554f\u984c\u306e\u8ef8 = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() \u306e\u5f15\u6570\u306e\u6570\u304c\u8aa4\u3063\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_COUNT_TAKES_1_ARG, "count \u95a2\u6570\u306f\u5f15\u6570\u3092 1 \u500b\u4f7f\u7528\u3057\u307e\u3059\u3002"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "\u95a2\u6570: {0} \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f"},
				new object[] {ER_UNSUPPORTED_ENCODING, "\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u306a\u3044\u30a8\u30f3\u30b3\u30fc\u30c9: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "\u554f\u984c\u304c getNextSibling \u5185\u306e DTM \u3067\u8d77\u3053\u308a\u307e\u3057\u305f... \u30ea\u30ab\u30d0\u30ea\u30fc\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "\u30d7\u30ed\u30b0\u30e9\u30de\u30fc\u30fb\u30a8\u30e9\u30fc: EmptyNodeList \u3092\u66f8\u304d\u8fbc\u307f\u5148\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory \u306f XPathContext \u306b\u3088\u308a\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093\u3002"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u63a5\u982d\u90e8\u306f\u540d\u524d\u7a7a\u9593\u306b\u89e3\u6c7a\u3055\u308c\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (InputSource \u30bd\u30fc\u30b9) \u306f XPathContext \u5185\u3067\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093\u3002 {0} \u3092\u30aa\u30fc\u30d7\u30f3\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API characters(char ch[]... \u306f DTM \u306b\u3088\u308a\u51e6\u7406\u3055\u308c\u307e\u305b\u3093\u3002"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... \u306f DTM \u306b\u3088\u308a\u51e6\u7406\u3055\u308c\u307e\u305b\u3093\u3002"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison \u306f\u578b {0} \u306e\u30ce\u30fc\u30c9\u3092\u51e6\u7406\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper \u306f\u578b {0} \u306e\u30ce\u30fc\u30c9\u3092\u51e6\u7406\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse \u30a8\u30e9\u30fc: SystemID - {0} \u884c - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "DOM2Helper.parse \u30a8\u30e9\u30fc"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u7121\u52b9\u306a UTF-16 \u30b5\u30ed\u30b2\u30fc\u30c8\u304c\u691c\u51fa\u3055\u308c\u307e\u3057\u305f: {0} ?"},
				new object[] {ER_OIERROR, "\u5165\u51fa\u529b\u30a8\u30e9\u30fc"},
				new object[] {ER_CANNOT_CREATE_URL, "{0} \u306e URL \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_XPATH_READOBJECT, "XPath.readObject \u5185: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "\u95a2\u6570\u30c8\u30fc\u30af\u30f3\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "XPath \u578b: {0} \u3092\u51e6\u7406\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_NODESET_NOT_MUTABLE, "\u3053\u306e NodeSet \u306f\u53ef\u5909\u3067\u3042\u308a\u307e\u305b\u3093"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "\u3053\u306e NodeSetDTM \u306f\u53ef\u5909\u3067\u3042\u308a\u307e\u305b\u3093"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "\u5909\u6570\u306f\u89e3\u6c7a\u53ef\u80fd\u3067\u3042\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u30cc\u30eb\u306e\u30a8\u30e9\u30fc\u30fb\u30cf\u30f3\u30c9\u30e9\u30fc"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "\u30d7\u30ed\u30b0\u30e9\u30de\u30fc\u306e\u30a2\u30b5\u30fc\u30b7\u30e7\u30f3: \u4e0d\u660e\u306a\u547d\u4ee4\u30b3\u30fc\u30c9: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 \u307e\u305f\u306f 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() \u306f XRTreeFragSelectWrapper \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() \u306f XRTreeFragSelectWrapper \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() \u306f XRTreeFragSelectWrapper \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() \u306f XRTreeFragSelectWrapper \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() \u306f XRTreeFragSelectWrapper \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() \u306f XRTreeFragSelectWrapper \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() \u306f XStringForChars \u306e\u5834\u5408\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {ER_COULD_NOT_FIND_VAR, "\u540d\u524d\u304c {0} \u306e\u5909\u6570\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars \u306f\u5f15\u6570\u306b\u30b9\u30c8\u30ea\u30f3\u30b0\u3092\u4f7f\u7528\u3057\u307e\u305b\u3093"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "FastStringBuffer \u5f15\u6570\u306f\u30cc\u30eb\u306b\u3067\u304d\u307e\u305b\u3093"},
				new object[] {ER_TWO_OR_THREE, "2 \u307e\u305f\u306f 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "\u5909\u6570\u304c\u30d0\u30a4\u30f3\u30c9\u3055\u308c\u308b\u524d\u306b\u30a2\u30af\u30bb\u30b9\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB \u306f\u5f15\u6570\u306b\u30b9\u30c8\u30ea\u30f3\u30b0\u3092\u4f7f\u7528\u3057\u307e\u305b\u3093\u3002"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n \u30a8\u30e9\u30fc: \u30a6\u30a9\u30fc\u30ab\u30fc\u306e\u30eb\u30fc\u30c8\u3092\u30cc\u30eb\u306b\u8a2d\u5b9a\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "\u3053\u306e NodeSetDTM \u306f\u76f4\u524d\u306e\u30ce\u30fc\u30c9\u3092\u7e70\u308a\u8fd4\u3059\u3053\u3068\u304c\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "\u3053\u306e NodeSet \u306f\u76f4\u524d\u306e\u30ce\u30fc\u30c9\u3092\u7e70\u308a\u8fd4\u3059\u3053\u3068\u304c\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "\u3053\u306e NodeSetDTM \u306f\u7d22\u5f15\u4ed8\u3051\u3084\u30ab\u30a6\u30f3\u30c8\u306e\u6a5f\u80fd\u3092\u5b9f\u884c\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NODESET_CANNOT_INDEX, "\u3053\u306e NodeSet \u306f\u7d22\u5f15\u4ed8\u3051\u3084\u30ab\u30a6\u30f3\u30c8\u306e\u6a5f\u80fd\u3092\u5b9f\u884c\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "nextNode \u3092\u547c\u3073\u51fa\u3057\u305f\u5f8c\u306b setShouldCacheNodes \u3092\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_ONLY_ALLOWS, "{0} \u306b\u8a31\u53ef\u3055\u308c\u308b\u5f15\u6570\u306f {1} \u500b\u306e\u307f\u3067\u3059"},
				new object[] {ER_UNKNOWN_STEP, "getNextStepPos \u5185\u306e\u30d7\u30ed\u30b0\u30e9\u30de\u30fc\u306e\u30a2\u30b5\u30fc\u30b7\u30e7\u30f3: \u4e0d\u660e\u306a stepType: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "\u76f8\u5bfe\u30ed\u30b1\u30fc\u30b7\u30e7\u30f3\u30fb\u30d1\u30b9\u306f '/' \u307e\u305f\u306f '//' \u30c8\u30fc\u30af\u30f3\u306e\u6b21\u306b\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				new object[] {ER_EXPECTED_LOC_PATH, "\u30ed\u30b1\u30fc\u30b7\u30e7\u30f3\u30fb\u30d1\u30b9\u304c\u5fc5\u8981\u3067\u3057\u305f\u304c\u3001\u6b21\u306e\u30c8\u30fc\u30af\u30f3\u304c\u691c\u51fa\u3055\u308c\u307e\u3057\u305f\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "\u30ed\u30b1\u30fc\u30b7\u30e7\u30f3\u30fb\u30d1\u30b9\u304c\u5fc5\u8981\u3067\u3057\u305f\u304c\u3001\u4ee3\u308f\u308a\u306b XPath \u5f0f\u306e\u7d42\u308f\u308a\u304c\u691c\u51fa\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_EXPECTED_LOC_STEP, "\u30ed\u30b1\u30fc\u30b7\u30e7\u30f3\u30fb\u30b9\u30c6\u30c3\u30d7\u306f '/' \u307e\u305f\u306f '//' \u30c8\u30fc\u30af\u30f3\u306e\u6b21\u306b\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				new object[] {ER_EXPECTED_NODE_TEST, "NCName:* \u307e\u305f\u306f QName \u306e\u3044\u305a\u308c\u304b\u3068\u4e00\u81f4\u3059\u308b\u30ce\u30fc\u30c9\u30fb\u30c6\u30b9\u30c8\u304c\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				new object[] {ER_EXPECTED_STEP_PATTERN, "\u30b9\u30c6\u30c3\u30d7\u30fb\u30d1\u30bf\u30fc\u30f3\u304c\u5fc5\u8981\u3067\u3057\u305f\u304c\u3001'/' \u304c\u691c\u51fa\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "\u76f8\u5bfe\u30d1\u30b9\u30fb\u30d1\u30bf\u30fc\u30f3\u304c\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPath \u5f0f ''{0}'' \u306e XPathResult \u306e XPathResultType \u306f {1} \u3067\u3001\u3053\u308c\u3092\u30d6\u30fc\u30eb\u306b\u5909\u63db\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPath \u5f0f ''{0}'' \u306e XPathResult \u306e XPathResultType \u306f {1} \u3067\u3001\u3053\u308c\u3092\u5358\u4e00\u30ce\u30fc\u30c9\u306b\u5909\u63db\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002\u30e1\u30bd\u30c3\u30c9 getSingleNodeValue \u304c\u9069\u7528\u3055\u308c\u308b\u306e\u306f\u3001\u578b ANY_UNORDERED_NODE_TYPE \u304a\u3088\u3073 FIRST_ORDERED_NODE_TYPE \u306e\u307f\u3067\u3059\u3002"},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "XPathResultType \u304c {1} \u3067\u3042\u308b\u305f\u3081\u3001\u30e1\u30bd\u30c3\u30c9 getSnapshotLength \u3092 XPath \u5f0f ''{0}'' \u306e XPathResult \u3092\u5bfe\u8c61\u306b\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002\u3053\u306e\u30e1\u30bd\u30c3\u30c9\u304c\u9069\u7528\u3055\u308c\u308b\u306e\u306f\u3001\u578b UNORDERED_NODE_SNAPSHOT_TYPE \u304a\u3088\u3073 ORDERED_NODE_SNAPSHOT_TYPE \u306e\u307f\u3067\u3059\u3002"},
				new object[] {ER_NON_ITERATOR_TYPE, "XPathResultType \u304c {1} \u3067\u3042\u308b\u305f\u3081\u3001\u30e1\u30bd\u30c3\u30c9 iterateNext \u3092 XPath \u5f0f ''{0}'' \u306e XPathResult \u3092\u5bfe\u8c61\u306b\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002\u3053\u306e\u30e1\u30bd\u30c3\u30c9\u304c\u9069\u7528\u3055\u308c\u308b\u306e\u306f\u3001\u578b UNORDERED_NODE_ITERATOR_TYPE \u304a\u3088\u3073 ORDERED_NODE_ITERATOR_TYPE \u306e\u307f\u3067\u3059\u3002"},
				new object[] {ER_DOC_MUTATED, "\u7d50\u679c\u304c\u623b\u3055\u308c\u305f\u4ee5\u5f8c\u306b\u6587\u66f8\u304c\u5909\u66f4\u3055\u308c\u307e\u3057\u305f\u3002\u30a4\u30c6\u30ec\u30fc\u30bf\u30fc\u304c\u7121\u52b9\u3067\u3059\u3002"},
				new object[] {ER_INVALID_XPATH_TYPE, "\u7121\u52b9\u306a XPath \u578b\u5f15\u6570: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "\u7a7a\u306e XPath \u7d50\u679c\u30aa\u30d6\u30b8\u30a7\u30af\u30c8"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPath \u5f0f ''{0}'' \u306e XPathResult \u306e XPathResultType \u306f {1} \u3067\u3001\u3053\u308c\u3092\u6307\u5b9a\u3055\u308c\u305f XPathResultType {2} \u306b\u5f37\u5236\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NULL_RESOLVER, "\u63a5\u982d\u90e8\u3092\u30cc\u30eb\u63a5\u982d\u90e8\u30ea\u30be\u30eb\u30d0\u30fc\u306b\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPath \u5f0f ''{0}'' \u306e XPathResult \u306e XPathResultType \u306f {1} \u3067\u3001\u3053\u308c\u3092\u30b9\u30c8\u30ea\u30f3\u30b0\u306b\u5909\u63db\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NON_SNAPSHOT_TYPE, "XPathResultType \u304c {1} \u3067\u3042\u308b\u305f\u3081\u3001\u30e1\u30bd\u30c3\u30c9 snapshotItem \u3092 XPath \u5f0f ''{0}'' \u306e XPathResult \u3092\u5bfe\u8c61\u306b\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002\u3053\u306e\u30e1\u30bd\u30c3\u30c9\u304c\u9069\u7528\u3055\u308c\u308b\u306e\u306f\u3001\u578b UNORDERED_NODE_SNAPSHOT_TYPE \u304a\u3088\u3073 ORDERED_NODE_SNAPSHOT_TYPE \u306e\u307f\u3067\u3059\u3002"},
				new object[] {ER_WRONG_DOCUMENT, "\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u30fb\u30ce\u30fc\u30c9\u306f\u3053\u306e XPathEvaluator \u306b\u30d0\u30a4\u30f3\u30c9\u3055\u308c\u3066\u3044\u308b\u6587\u66f8\u306b\u5c5e\u3057\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_WRONG_NODETYPE, "\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u30fb\u30ce\u30fc\u30c9\u578b\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_XPATH_ERROR, "XPath \u306b\u4e0d\u660e\u306a\u30a8\u30e9\u30fc\u304c\u3042\u308a\u307e\u3059\u3002"},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPath \u5f0f ''{0}'' \u306e XPathResult \u306e XPathResultType \u306f {1} \u3067\u3001\u3053\u308c\u3092\u6570\u5024\u306b\u5909\u63db\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "XMLConstants.FEATURE_SECURE_PROCESSING \u6a5f\u80fd\u306e\u8a2d\u5b9a\u304c true \u306e\u3068\u304d\u306b\u3001\u62e1\u5f35\u95a2\u6570 ''{0}'' \u3092\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "\u5909\u6570 {0} \u306e resolveVariable \u304c NULL \u3092\u623b\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u306a\u3044\u623b\u308a\u5024\u306e\u578b: {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Source \u307e\u305f\u306f Return Type\u3001\u3042\u308b\u3044\u306f\u305d\u306e\u4e21\u65b9\u3092\u30cc\u30eb\u306b\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "{0} \u5f15\u6570\u306f\u30cc\u30eb\u306b\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_OBJECT_MODEL_NULL, "objectModel == null \u3067 {0}#isObjectModelSupported( String objectModel ) \u3092\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "objectModel == \"\" \u3067 {0}#isObjectModelSupported( String objectModel ) \u3092\u547c\u3073\u51fa\u3059\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_FEATURE_NAME_NULL, "\u6a5f\u80fd\u3092\u30cc\u30eb\u540d\u3067\u8a2d\u5b9a\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "\u4e0d\u660e\u306a\u6a5f\u80fd \"{0}\" \u3092\u8a2d\u5b9a\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059: {1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "\u6a5f\u80fd\u3092\u30cc\u30eb\u540d\u3067\u691c\u7d22\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "\u4e0d\u660e\u306a\u6a5f\u80fd \"{0}\" \u3092\u691c\u7d22\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059: {1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "XPathFunctionResolver \u3092\u30cc\u30eb\u3067\u8a2d\u5b9a\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059: {0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "XPathVariableResolver \u3092\u30cc\u30eb\u3067\u8a2d\u5b9a\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059: {0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "\u30d5\u30a9\u30fc\u30de\u30c3\u30c8\u756a\u53f7\u95a2\u6570\u5185\u306e\u30ed\u30b1\u30fc\u30eb\u540d\u306f\u307e\u3060\u51e6\u7406\u3055\u308c\u307e\u305b\u3093\u3002"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "XSL \u30d7\u30ed\u30d1\u30c6\u30a3\u30fc: {0} \u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "\u73fe\u5728\u3001\u30d7\u30ed\u30d1\u30c6\u30a3\u30fc {1} \u306e\u540d\u524d\u7a7a\u9593 {0} \u3067\u4f55\u3082\u5b9f\u884c\u3055\u308c\u3066\u3044\u307e\u305b\u3093"},
				new object[] {WG_SECURITY_EXCEPTION, "XSL \u30b7\u30b9\u30c6\u30e0\u30fb\u30d7\u30ed\u30d1\u30c6\u30a3\u30fc: {0} \u306b\u30a2\u30af\u30bb\u30b9\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u308b\u3068\u304d\u306b SecurityException"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "\u65e7\u69cb\u6587: quo(...) \u306f XPath \u5185\u306b\u3082\u3046\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "nodeTest \u3092\u5b9f\u88c5\u3059\u308b\u306b\u306f XPath \u306b\u6d3e\u751f\u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "\u95a2\u6570\u30c8\u30fc\u30af\u30f3\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "\u95a2\u6570: {0} \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "URL \u3092 {0} \u304b\u3089\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "-E \u30aa\u30d7\u30b7\u30e7\u30f3\u306f DTM \u30d1\u30fc\u30b5\u30fc\u306e\u5834\u5408\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "\u5909\u6570\u306b\u6307\u5b9a\u3055\u308c\u305f VariableReference \u304c\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u5916\u304b\u3001\u5b9a\u7fa9\u304c\u3042\u308a\u307e\u305b\u3093\u3002\u540d\u524d = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u306a\u3044\u30a8\u30f3\u30b3\u30fc\u30c9: {0}"},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "createMessage \u3078\u306e\u30d1\u30e9\u30e1\u30fc\u30bf\u30fc\u304c\u7bc4\u56f2\u5916\u3067\u3057\u305f\u3002"},
				new object[] {"FORMAT_FAILED", "messageFormat \u547c\u3073\u51fa\u3057\u4e2d\u306b\u4f8b\u5916\u304c\u30b9\u30ed\u30fc\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {"version", ">>>>>>> Xalan \u30d0\u30fc\u30b8\u30e7\u30f3 "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u306f\u3044 (y)"},
				new object[] {"line", "\u884c #"},
				new object[] {"column", "\u6841 #"},
				new object[] {"xsldone", "XSLProcessor: \u5b8c\u4e86"},
				new object[] {"xpath_option", "xpath \u30aa\u30d7\u30b7\u30e7\u30f3: "},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select xpath \u5f0f]"},
				new object[] {"optionMatch", "   [-match \u30de\u30c3\u30c1\u30f3\u30b0\u30fb\u30d1\u30bf\u30fc\u30f3 (\u30de\u30c3\u30c1\u30f3\u30b0\u8a3a\u65ad\u7528)]"},
				new object[] {"optionAnyExpr", "\u3042\u308b\u3044\u306f\u8a3a\u65ad\u30c0\u30f3\u30d7\u3092\u5b9f\u884c\u3059\u308b\u306e\u306f xpath \u5f0f\u3060\u3051\u3067\u3059"},
				new object[] {"noParsermsg1", "XSL \u51e6\u7406\u306f\u6210\u529f\u3057\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {"noParsermsg2", "** \u30d1\u30fc\u30b5\u30fc\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f **"},
				new object[] {"noParsermsg3", "\u30af\u30e9\u30b9\u30d1\u30b9\u3092\u8abf\u3079\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {"noParsermsg4", "IBM \u306e XML Parser for Java \u304c\u306a\u3044\u5834\u5408\u306f\u3001\u6b21\u306e\u30b5\u30a4\u30c8\u304b\u3089\u30c0\u30a6\u30f3\u30ed\u30fc\u30c9\u3067\u304d\u307e\u3059:"},
				new object[] {"noParsermsg5", "IBM AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"gtone", ">1"},
				new object[] {"zero", "0"},
				new object[] {"one", "1"},
				new object[] {"two", "2"},
				new object[] {"three", "3"}
			};
		  }
	  }


	  // ================= INFRASTRUCTURE ======================

	  /// <summary>
	  /// Field BAD_CODE </summary>
	  public const string BAD_CODE = "BAD_CODE";

	  /// <summary>
	  /// Field FORMAT_FAILED </summary>
	  public const string FORMAT_FAILED = "FORMAT_FAILED";

	  /// <summary>
	  /// Field ERROR_RESOURCES </summary>
	  public const string ERROR_RESOURCES = "org.apache.xpath.res.XPATHErrorResources";

	  /// <summary>
	  /// Field ERROR_STRING </summary>
	  public const string ERROR_STRING = "#\u30a8\u30e9\u30fc";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "\u30a8\u30e9\u30fc: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "\u8b66\u544a: ";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "\u30d1\u30bf\u30fc\u30f3 ";


	  /// <summary>
	  /// Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  /// of ResourceBundle.getBundle().
	  /// </summary>
	  /// <param name="className"> Name of local-specific subclass. </param>
	  /// <returns> the ResourceBundle </returns>
	  /// <exception cref="MissingResourceException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final XPATHErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XPATHErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.Default;
		string suffix = getResourceSuffix(locale);

		try
		{

		  // first try with the given locale
		  return (XPATHErrorResources) ResourceBundle.getBundle(className + suffix, locale);
		}
		catch (MissingResourceException)
		{
		  try // try to fall back to en_US if we can't load
		  {

			// Since we can't find the localized property file,
			// fall back to en_US.
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("en", "US"));
		  }
		  catch (MissingResourceException)
		  {

			// Now we are really in trouble.
			// very bad, definitely very bad...not going to get very far
			throw new MissingResourceException("Could not load any resource bundles.", className, "");
		  }
		}
	  }

	  /// <summary>
	  /// Return the resource file suffic for the indicated locale
	  /// For most locales, this will be based the language code.  However
	  /// for Chinese, we do distinguish between Taiwan and PRC
	  /// </summary>
	  /// <param name="locale"> the locale </param>
	  /// <returns> an String suffix which canbe appended to a resource name </returns>
	  private static string getResourceSuffix(Locale locale)
	  {

		string suffix = "_" + locale.Language;
		string country = locale.Country;

		if (country.Equals("TW"))
		{
		  suffix += "_" + country;
		}

		return suffix;
	  }

	}

}