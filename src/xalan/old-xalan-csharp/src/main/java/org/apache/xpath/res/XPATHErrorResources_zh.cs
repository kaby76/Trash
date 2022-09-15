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
 * $Id: XPATHErrorResources_zh.java 1225426 2011-12-29 04:13:08Z mrglavas $
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
	public class XPATHErrorResources_zh : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "\u5339\u914d\u6a21\u5f0f\u4e2d\u4e0d\u5141\u8bb8\u6709 current() \u51fd\u6570\uff01"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "current() \u51fd\u6570\u4e0d\u63a5\u53d7\u53c2\u6570\uff01"},
				new object[] {ER_DOCUMENT_REPLACED, "document() \u51fd\u6570\u5b9e\u73b0\u5df2\u88ab org.apache.xalan.xslt.FuncDocument \u66ff\u6362\uff01"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "\u4e0a\u4e0b\u6587\u6ca1\u6709\u6240\u6709\u8005\u6587\u6863\uff01"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() \u7684\u53c2\u6570\u592a\u591a\u3002"},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "translate() \u51fd\u6570\u6709\u4e09\u4e2a\u53c2\u6570\uff01"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "unparsed-entity-uri \u51fd\u6570\u5e94\u6709\u4e00\u4e2a\u53c2\u6570\uff01"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "\u540d\u79f0\u7a7a\u95f4\u8f74\u5c1a\u672a\u5b9e\u73b0\uff01"},
				new object[] {ER_UNKNOWN_AXIS, "\u672a\u77e5\u8f74\uff1a{0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "\u672a\u77e5\u7684\u5339\u914d\u64cd\u4f5c\uff01"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "processing-instruction() \u8282\u70b9\u6d4b\u8bd5\u7684\u53c2\u6570\u957f\u5ea6\u4e0d\u6b63\u786e\uff01"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "\u65e0\u6cd5\u5c06 {0} \u8f6c\u6362\u6210\u6570\u5b57"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "\u65e0\u6cd5\u5c06 {0} \u8f6c\u6362\u6210 NodeList\uff01"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "\u65e0\u6cd5\u5c06 {0} \u8f6c\u6362\u6210 NodeSetDTM\uff01"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "\u65e0\u6cd5\u5c06 {0} \u8f6c\u6362\u6210 type#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "getMatchScore \u4e2d\u51fa\u73b0\u671f\u671b\u7684\u5339\u914d\u6a21\u5f0f\uff01"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "\u65e0\u6cd5\u83b7\u53d6\u540d\u4e3a {0} \u7684\u53d8\u91cf"},
				new object[] {ER_UNKNOWN_OPCODE, "\u9519\u8bef\uff01\u672a\u77e5\u64cd\u4f5c\u7801\uff1a{0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "\u989d\u5916\u7684\u975e\u6cd5\u6807\u8bb0\uff1a{0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "\u9519\u8bef\u5f15\u7528\u7684\u6587\u5b57... \u5e94\u8be5\u4e3a\u53cc\u5f15\u53f7\uff01"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "\u9519\u8bef\u5f15\u7528\u7684\u6587\u5b57... \u5e94\u8be5\u4e3a\u5355\u5f15\u53f7\uff01"},
				new object[] {ER_EMPTY_EXPRESSION, "\u7a7a\u8868\u8fbe\u5f0f\uff01"},
				new object[] {ER_EXPECTED_BUT_FOUND, "\u5e94\u8be5\u4e3a {0}\uff0c\u4f46\u53d1\u73b0\u7684\u662f\uff1a{1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "\u7a0b\u5e8f\u5458\u7684\u65ad\u8a00\u4e0d\u6b63\u786e\uff01\uff0d {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "19990709 XPath \u8349\u7a3f\u4e2d\uff0cboolean(...) \u53c2\u6570\u4e0d\u518d\u53ef\u9009\u3002"},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "\u5df2\u627e\u5230\u201c,\u201d\uff0c\u4f46\u524d\u9762\u6ca1\u6709\u81ea\u53d8\u91cf\uff01"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "\u5df2\u627e\u5230\u201c,\u201d\uff0c\u4f46\u540e\u9762\u6ca1\u6709\u8ddf\u81ea\u53d8\u91cf\uff01"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "\u201c..[predicate]\u201d\u6216\u201c.[predicate]\u201d\u662f\u975e\u6cd5\u7684\u8bed\u6cd5\u3002\u8bf7\u6539\u4e3a\u4f7f\u7528\u201cself::node()[predicate]\u201d\u3002"},
				new object[] {ER_ILLEGAL_AXIS_NAME, "\u975e\u6cd5\u7684\u8f74\u540d\u79f0\uff1a{0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "\u672a\u77e5\u8282\u70b9\u7c7b\u578b\uff1a{0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "\u9700\u8981\u5f15\u7528\u6a21\u5f0f\u6587\u5b57\uff08{0}\uff09\uff01"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} \u65e0\u6cd5\u683c\u5f0f\u5316\u4e3a\u6570\u5b57\uff01"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "\u65e0\u6cd5\u521b\u5efa XML TransformerFactory \u8054\u7cfb\uff1a{0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "\u9519\u8bef\uff01\u627e\u4e0d\u5230 xpath \u9009\u62e9\u8868\u8fbe\u5f0f\uff08-select\uff09\u3002"},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "\u9519\u8bef\uff01\u5728 OP_LOCATIONPATH \u4e4b\u540e\u627e\u4e0d\u5230 ENDOP"},
				new object[] {ER_ERROR_OCCURED, "\u51fa\u73b0\u9519\u8bef\uff01"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference \u8d4b\u7ed9\u4e86\u4e0a\u4e0b\u6587\u5916\u7684\u53d8\u91cf\u6216\u6ca1\u6709\u5b9a\u4e49\u7684\u53d8\u91cf\uff01\u540d\u79f0 = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "\u5728\u5339\u914d\u6a21\u5f0f\u4e2d\u53ea\u5141\u8bb8\u51fa\u73b0 child:: \u548c attribute:: \u8f74\uff01\u8fdd\u53cd\u7684\u8f74 = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() \u7684\u53c2\u6570\u4e2a\u6570\u4e0d\u6b63\u786e\u3002"},
				new object[] {ER_COUNT_TAKES_1_ARG, "count \u51fd\u6570\u5e94\u8be5\u6709\u4e00\u4e2a\u53c2\u6570\uff01"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "\u627e\u4e0d\u5230\u51fd\u6570\uff1a{0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "\u4e0d\u53d7\u652f\u6301\u7684\u7f16\u7801\uff1a{0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "getNextSibling \u8fc7\u7a0b\u4e2d\uff0cDTM \u4e2d\u51fa\u73b0\u95ee\u9898...\u6b63\u5728\u5c1d\u8bd5\u6062\u590d"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "\u7a0b\u5e8f\u5458\u9519\u8bef\uff1a\u4e0d\u53ef\u5411 EmptyNodeList \u5199\u5165\u5185\u5bb9\u3002"},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "XPathContext \u4e0d\u652f\u6301 setDOMFactory\uff01"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u524d\u7f00\u5fc5\u987b\u89e3\u6790\u4e3a\u540d\u79f0\u7a7a\u95f4\uff1a{0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "XPathContext \u4e2d\u4e0d\u652f\u6301 parse (InputSource source)\uff01\u65e0\u6cd5\u6253\u5f00 {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "DTM \u4e0d\u5904\u7406 SAX API characters(char ch[]...\uff01"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "DTM \u4e0d\u5904\u7406 ignorableWhitespace(char ch[]...\uff01"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison \u4e0d\u80fd\u5904\u7406\u7c7b\u578b {0} \u7684\u8282\u70b9"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper \u4e0d\u80fd\u5904\u7406\u7c7b\u578b {0} \u7684\u8282\u70b9"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse \u9519\u8bef\uff1aSystemID \uff0d \u7b2c {0} \u884c \uff0d {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "DOM2Helper.parse \u9519\u8bef"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u68c0\u6d4b\u5230\u65e0\u6548\u7684 UTF-16 \u8d85\u5927\u5b57\u7b26\u96c6\uff1a{0}\uff1f"},
				new object[] {ER_OIERROR, "IO \u9519\u8bef"},
				new object[] {ER_CANNOT_CREATE_URL, "\u65e0\u6cd5\u4e3a {0} \u521b\u5efa URL"},
				new object[] {ER_XPATH_READOBJECT, "\u5728 XPath.readObject \u4e2d\uff1a{0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "\u627e\u4e0d\u5230\u51fd\u6570\u6807\u8bb0\u3002"},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "\u65e0\u6cd5\u5904\u7406 XPath \u7c7b\u578b\uff1a{0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "\u6b64 NodeSet \u662f\u4e0d\u6613\u53d8\u7684"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "\u6b64 NodeSetDTM \u662f\u4e0d\u6613\u53d8\u7684"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "\u53d8\u91cf\u4e0d\u53ef\u89e3\u6790\uff1a{0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u9519\u8bef\u5904\u7406\u7a0b\u5e8f\u4e3a\u7a7a"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "\u7a0b\u5e8f\u5458\u65ad\u8a00\uff1a\u672a\u77e5\u64cd\u4f5c\u7801\uff1a{0}"},
				new object[] {ER_ZERO_OR_ONE, "0 \u6216 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper \u4e0d\u652f\u6301 rtf()"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper \u4e0d\u652f\u6301 asNodeIterator()"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper \u4e0d\u652f\u6301 detach()"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper \u4e0d\u652f\u6301 num()"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper \u4e0d\u652f\u6301 xstr()"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper \u4e0d\u652f\u6301 str()"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "XStringForChars \u4e0d\u652f\u6301 fsb()"},
				new object[] {ER_COULD_NOT_FIND_VAR, "\u627e\u4e0d\u5230\u540d\u4e3a {0} \u7684\u53d8\u91cf"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars \u65e0\u6cd5\u5c06\u5b57\u7b26\u4e32\u4f5c\u4e3a\u81ea\u53d8\u91cf"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "FastStringBuffer \u81ea\u53d8\u91cf\u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_TWO_OR_THREE, "2 \u6216 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "\u5728\u7ed1\u5b9a\u524d\u5df2\u8bbf\u95ee\u53d8\u91cf\uff01"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB \u65e0\u6cd5\u5c06\u5b57\u7b26\u4e32\u4f5c\u4e3a\u81ea\u53d8\u91cf\uff01"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n \uff01\uff01\uff01\uff01\u9519\u8bef\uff01\u6b63\u5728\u5c06\u6b65\u884c\u7a0b\u5e8f\u7684\u6839\u8bbe\u7f6e\u4e3a\u7a7a\uff01\uff01\uff01"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "\u6b64 NodeSetDTM \u65e0\u6cd5\u8fed\u4ee3\u5230\u5148\u524d\u7684\u8282\u70b9\uff01"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "\u6b64 NodeSet \u65e0\u6cd5\u8fed\u4ee3\u5230\u5148\u524d\u7684\u8282\u70b9\uff01"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "\u6b64 NodeSetDTM \u65e0\u6cd5\u6267\u884c\u7d22\u5f15\u6216\u8ba1\u6570\u529f\u80fd\uff01"},
				new object[] {ER_NODESET_CANNOT_INDEX, "\u6b64 NodeSet \u65e0\u6cd5\u6267\u884c\u7d22\u5f15\u6216\u8ba1\u6570\u529f\u80fd\uff01"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "\u8c03\u7528 nextNode \u540e\u4e0d\u80fd\u8c03\u7528 setShouldCacheNode\uff01"},
				new object[] {ER_ONLY_ALLOWS, "{0} \u4ec5\u5141\u8bb8 {1} \u4e2a\u81ea\u53d8\u91cf"},
				new object[] {ER_UNKNOWN_STEP, "\u7a0b\u5e8f\u5458\u5728 getNextStepPos \u4e2d\u7684\u65ad\u8a00\uff1a\u672a\u77e5\u7684 stepType\uff1a{0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "\u5728\u201c/\u201d\u6216\u201c//\u201d\u6807\u8bb0\u540e\u5e94\u8be5\u51fa\u73b0\u76f8\u5bf9\u4f4d\u7f6e\u8def\u5f84\u3002"},
				new object[] {ER_EXPECTED_LOC_PATH, "\u5e94\u8be5\u51fa\u73b0\u4f4d\u7f6e\u8def\u5f84\uff0c\u4f46\u9047\u5230\u4ee5\u4e0b\u6807\u8bb0\u003a{0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "\u5e94\u8be5\u51fa\u73b0\u4f4d\u7f6e\u8def\u5f84\uff0c\u4f46\u53d1\u73b0\u7684\u5374\u662f XPath \u8868\u8fbe\u5f0f\u7684\u7ed3\u5c3e\u3002"},
				new object[] {ER_EXPECTED_LOC_STEP, "\u201c/\u201d\u6216\u201c//\u201d\u6807\u8bb0\u540e\u5e94\u8be5\u51fa\u73b0\u4f4d\u7f6e\u6b65\u9aa4\u3002"},
				new object[] {ER_EXPECTED_NODE_TEST, "\u5e94\u8be5\u51fa\u73b0\u4e0e NCName:* \u6216 QName \u5339\u914d\u7684\u8282\u70b9\u6d4b\u8bd5\u3002"},
				new object[] {ER_EXPECTED_STEP_PATTERN, "\u5e94\u8be5\u51fa\u73b0\u6b65\u9aa4\u6a21\u5f0f\uff0c\u4f46\u9047\u5230\u4e86\u201c/\u201d\u3002"},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "\u5e94\u8be5\u51fa\u73b0\u76f8\u5bf9\u8def\u5f84\u6a21\u5f0f\u3002"},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u5177\u6709 XPathResultType {1}\uff0c\u8be5\u7c7b\u578b\u4e0d\u80fd\u8f6c\u6362\u4e3a\u5e03\u5c14\u578b\u3002"},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u5177\u6709 XPathResultType {1}\uff0c\u8be5\u7c7b\u578b\u4e0d\u80fd\u8f6c\u6362\u4e3a\u5355\u4e00\u8282\u70b9\u3002getSingleNodeValue \u65b9\u6cd5\u4ec5\u9002\u7528\u4e8e\u7c7b\u578b ANY_UNORDERED_NODE_TYPE \u548c FIRST_ORDERED_NODE_TYPE\u3002"},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "\u4e0d\u80fd\u5bf9 XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u8c03\u7528 getSnapshotLength \u65b9\u6cd5\uff0c\u56e0\u4e3a\u8be5\u8868\u8fbe\u5f0f\u7684 XPathResult \u7684 XPathResultType \u4e3a {1}\u3002\u6b64\u65b9\u6cd5\u4ec5\u9002\u7528\u4e8e\u7c7b\u578b UNORDERED_NODE_SNAPSHOT_TYPE \u548c ORDERED_NODE_SNAPSHOT_TYPE\u3002"},
				new object[] {ER_NON_ITERATOR_TYPE, "\u4e0d\u80fd\u5bf9 XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u8c03\u7528 iterateNext \u65b9\u6cd5\uff0c\u56e0\u4e3a\u8be5\u8868\u8fbe\u5f0f\u7684 XPathResult \u7684 XPathResultType \u4e3a {1}\u3002\u6b64\u65b9\u6cd5\u4ec5\u9002\u7528\u4e8e\u7c7b\u578b UNORDERED_NODE_ITERATOR_TYPE \u548c ORDERED_NODE_ITERATOR_TYPE\u3002"},
				new object[] {ER_DOC_MUTATED, "\u8fd4\u56de\u7ed3\u679c\u540e\u6587\u6863\u53d1\u751f\u53d8\u5316\u3002\u8fed\u4ee3\u5668\u65e0\u6548\u3002"},
				new object[] {ER_INVALID_XPATH_TYPE, "\u65e0\u6548\u7684 XPath \u7c7b\u578b\u81ea\u53d8\u91cf\uff1a{0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "\u7a7a\u7684 XPath \u7ed3\u679c\u5bf9\u8c61"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u5177\u6709 XPathResultType {1}\uff0c\u8be5\u7c7b\u578b\u4e0d\u80fd\u5f3a\u5236\u8f6c\u6362\u4e3a\u6307\u5b9a\u7684 XPathResultType {2}\u3002"},
				new object[] {ER_NULL_RESOLVER, "\u65e0\u6cd5\u4f7f\u7528\u7a7a\u7684\u524d\u7f00\u89e3\u6790\u5668\u89e3\u6790\u524d\u7f00\u3002"},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u5177\u6709 XPathResultType {1}\uff0c\u8be5\u7c7b\u578b\u4e0d\u80fd\u8f6c\u6362\u4e3a\u5b57\u7b26\u4e32\u3002"},
				new object[] {ER_NON_SNAPSHOT_TYPE, "\u4e0d\u80fd\u5bf9 XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u8c03\u7528\u65b9\u6cd5 snapshotItem\uff0c\u56e0\u4e3a\u8be5\u8868\u8fbe\u5f0f\u7684 XPathResult \u7684 XPathResultType \u4e3a {1}\u3002\u6b64\u65b9\u6cd5\u4ec5\u9002\u7528\u4e8e\u7c7b\u578b UNORDERED_NODE_SNAPSHOT_TYPE \u548c ORDERED_NODE_SNAPSHOT_TYPE\u3002"},
				new object[] {ER_WRONG_DOCUMENT, "\u4e0a\u4e0b\u6587\u8282\u70b9\u4e0d\u5c5e\u4e8e\u7ed1\u5b9a\u5230\u6b64 XPathEvaluator \u7684\u6587\u6863\u3002"},
				new object[] {ER_WRONG_NODETYPE, "\u4e0d\u652f\u6301\u4e0a\u4e0b\u6587\u8282\u70b9\u7c7b\u578b\u3002"},
				new object[] {ER_XPATH_ERROR, "XPath \u4e2d\u51fa\u73b0\u672a\u77e5\u9519\u8bef"},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684 XPathResult \u5177\u6709 XPathResultType {1}\uff0c\u8be5\u7c7b\u578b\u4e0d\u80fd\u8f6c\u6362\u4e3a\u6570\u5b57\u3002"},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "\u6269\u5c55\u51fd\u6570\uff1a\u5f53 XMLConstants.FEATURE_SECURE_PROCESSING \u529f\u80fd\u8bbe\u7f6e\u4e3a true \u65f6\uff0c\u65e0\u6cd5\u8c03\u7528\u201c{0}\u201d\u3002"},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "\u53d8\u91cf {0} \u7684 resolveVariable \u6b63\u5728\u8fd4\u56de\u7a7a\u503c"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "\u4e0d\u53d7\u652f\u6301\u7684\u8fd4\u56de\u7c7b\u578b\uff1a{0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "\u6e90\u548c\uff0f\u6216\u8fd4\u56de\u7c7b\u578b\u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "{0} \u81ea\u53d8\u91cf\u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( String objectModel ) \u4e0d\u80fd\u88ab\u8c03\u7528\uff08\u5982\u679c objectModel == null\uff09"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( String objectModel ) \u4e0d\u80fd\u88ab\u8c03\u7528\uff08\u5982\u679c objectModel == \"\"\uff09"},
				new object[] {ER_FEATURE_NAME_NULL, "\u6b63\u5728\u5c1d\u8bd5\u8bbe\u7f6e\u540d\u79f0\u4e3a\u7a7a\u7684\u7279\u5f81\uff1a{0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "\u6b63\u5728\u5c1d\u8bd5\u8bbe\u7f6e\u672a\u77e5\u7279\u5f81\u201c{0}\u201d\uff1a{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "\u6b63\u5728\u5c1d\u8bd5\u83b7\u53d6\u540d\u79f0\u4e3a\u7a7a\u7684\u7279\u5f81\uff1a{0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "\u6b63\u5728\u5c1d\u8bd5\u83b7\u53d6\u672a\u77e5\u7279\u5f81\u201c{0}\u201d\uff1a{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "\u6b63\u5728\u8bd5\u56fe\u8bbe\u7f6e\u7a7a\u7684 XPathFunctionResolver\uff1a{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "\u6b63\u5728\u8bd5\u56fe\u8bbe\u7f6e\u7a7a\u7684 XPathVariableResolver\uff1a{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "\u5728\u672a\u5904\u7406\u8fc7\u7684 format-number \u51fd\u6570\u4e2d\u51fa\u73b0\u8bed\u8a00\u73af\u5883\u540d\uff01"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "\u4e0d\u652f\u6301 XSL \u5c5e\u6027\uff1a{0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "\u5f53\u524d\u4e0d\u8981\u5728\u5c5e\u6027 {1} \u4e2d\u5bf9\u540d\u79f0\u7a7a\u95f4 {0} \u8fdb\u884c\u4efb\u4f55\u5904\u7406"},
				new object[] {WG_SECURITY_EXCEPTION, "\u5728\u8bd5\u56fe\u8bbf\u95ee XSL \u7cfb\u7edf\u5c5e\u6027 {0} \u65f6\u53d1\u751f SecurityException"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "XPath \u4e2d\u4e0d\u518d\u5b9a\u4e49\u65e7\u8bed\u6cd5\uff1aquo(...)\u3002"},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath \u9700\u8981\u4e00\u4e2a\u6d3e\u751f\u7684\u5bf9\u8c61\u4ee5\u5b9e\u73b0 nodeTest\uff01"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "\u627e\u4e0d\u5230\u51fd\u6570\u6807\u8bb0\u3002"},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "\u627e\u4e0d\u5230\u51fd\u6570\uff1a{0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "\u65e0\u6cd5\u4ece {0} \u751f\u6210 URL"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "DTM \u89e3\u6790\u5668\u4e0d\u652f\u6301 -E \u9009\u9879"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference \u8d4b\u7ed9\u4e86\u4e0a\u4e0b\u6587\u5916\u7684\u53d8\u91cf\u6216\u6ca1\u6709\u5b9a\u4e49\u7684\u53d8\u91cf\uff01\u540d\u79f0 = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "\u4e0d\u53d7\u652f\u6301\u7684\u7f16\u7801\uff1a{0}"},
				new object[] {"ui_language", "zh"},
				new object[] {"help_language", "zh"},
				new object[] {"language", "zh"},
				new object[] {"BAD_CODE", "createMessage \u7684\u53c2\u6570\u8d85\u51fa\u8303\u56f4"},
				new object[] {"FORMAT_FAILED", "\u5728 messageFormat \u8c03\u7528\u8fc7\u7a0b\u4e2d\u629b\u51fa\u4e86\u5f02\u5e38"},
				new object[] {"version", ">>>>>>> Xalan \u7248\u672c"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u662f"},
				new object[] {"line", "\u884c\u53f7"},
				new object[] {"column", "\u5217\u53f7"},
				new object[] {"xsldone", "XSLProcessor\uff1a\u5b8c\u6210"},
				new object[] {"xpath_option", "xpath \u9009\u9879\uff1a"},
				new object[] {"optionIN", "[-in inputXMLURL]"},
				new object[] {"optionSelect", "[-select xpath \u8868\u8fbe\u5f0f]"},
				new object[] {"optionMatch", "[-match \u5339\u914d\u6a21\u5f0f\uff08\u7528\u4e8e\u5339\u914d\u8bca\u65ad\uff09]"},
				new object[] {"optionAnyExpr", "\u6216\u8005\u4ec5\u4e00\u4e2a xpath \u8868\u8fbe\u5f0f\u5c31\u5c06\u5b8c\u6210\u4e00\u4e2a\u8bca\u65ad\u8f6c\u50a8"},
				new object[] {"noParsermsg1", "XSL \u5904\u7406\u4e0d\u6210\u529f\u3002"},
				new object[] {"noParsermsg2", "** \u627e\u4e0d\u5230\u89e3\u6790\u5668 **"},
				new object[] {"noParsermsg3", "\u8bf7\u68c0\u67e5\u60a8\u7684\u7c7b\u8def\u5f84\u3002"},
				new object[] {"noParsermsg4", "\u5982\u679c\u6ca1\u6709 IBM \u7684 XML Parser for Java\uff0c\u60a8\u53ef\u4ee5\u4ece\u4ee5\u4e0b\u4f4d\u7f6e\u4e0b\u8f7d\u5b83\uff1a"},
				new object[] {"noParsermsg5", "IBM \u7684 AlphaWorks\uff1ahttp://www.alphaworks.ibm.com/formula/xml"},
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
	  public const string ERROR_STRING = "#\u9519\u8bef";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "\u9519\u8bef\uff1a";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "\u8b66\u544a\uff1a";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "PATTERN";


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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("zh", "CN"));
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