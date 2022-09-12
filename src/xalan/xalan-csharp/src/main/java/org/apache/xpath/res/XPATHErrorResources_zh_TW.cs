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
 * $Id: XPATHErrorResources_zh_TW.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_zh_TW : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "\u5728\u6bd4\u5c0d\u578b\u6a23\u4e2d\u4e0d\u5141\u8a31\u4f7f\u7528 current() \u51fd\u6578\uff01"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "current() \u51fd\u6578\u4e0d\u63a5\u53d7\u5f15\u6578\uff01"},
				new object[] {ER_DOCUMENT_REPLACED, "document() \u51fd\u6578\u5be6\u4f5c\u5df2\u88ab org.apache.xalan.xslt.FuncDocument \u53d6\u4ee3\uff01"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "\u74b0\u5883\u5b9a\u7fa9\u6c92\u6709\u64c1\u6709\u8005\u6587\u4ef6\uff01"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() \u6709\u592a\u591a\u5f15\u6578\u3002"},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "translate() \u51fd\u6578\u9700\u8981 3 \u500b\u5f15\u6578\uff01"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "unparsed-entity-uri \u51fd\u6578\u53ea\u9700\u8981 1 \u500b\u5f15\u6578\uff01"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "namespace axis \u5c1a\u672a\u5be6\u4f5c\uff01"},
				new object[] {ER_UNKNOWN_AXIS, "\u4e0d\u660e\u8ef8\uff1a{0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "\u4e0d\u660e\u7684\u6bd4\u5c0d\u4f5c\u696d\uff01"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "processing-instruction() \u7bc0\u9ede\u6e2c\u8a66\u7684\u5f15\u6578\u9577\u5ea6\u4e0d\u6b63\u78ba\uff01"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "{0} \u7121\u6cd5\u8f49\u63db\u70ba\u6578\u5b57"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "{0} \u7121\u6cd5\u8f49\u63db\u70ba NodeList\uff01"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "{0} \u7121\u6cd5\u8f49\u63db\u70ba NodeSetDTM\uff01"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "\u7121\u6cd5\u5c07 {0} \u8f49\u63db\u70ba type#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "\u539f\u9810\u671f\u5728 getMatchScore \u4e2d\u6703\u51fa\u73fe\u6bd4\u5c0d\u578b\u6a23\uff01"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "\u7121\u6cd5\u53d6\u5f97\u8b8a\u6578\u540d\u7a31 {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "\u932f\u8aa4\uff01\u4e0d\u660e\u4f5c\u696d\u78bc\uff1a{0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "\u984d\u5916\u7684\u4e0d\u5408\u6cd5\u8a18\u865f\uff1a{0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "\u62ec\u932f\u5f15\u865f\u7684\u6587\u5b57... \u539f\u9810\u671f\u70ba\u96d9\u5f15\u865f\uff01"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "\u62ec\u932f\u5f15\u865f\u7684\u6587\u5b57... \u539f\u9810\u671f\u70ba\u55ae\u5f15\u865f\uff01"},
				new object[] {ER_EMPTY_EXPRESSION, "\u7a7a\u7684\u8868\u793a\u5f0f\uff01"},
				new object[] {ER_EXPECTED_BUT_FOUND, "\u539f\u9810\u671f\u70ba {0}\uff0c\u537b\u767c\u73fe\uff1a{1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "\u7a0b\u5f0f\u8a2d\u8a08\u5e2b\u5047\u8a2d(Programmer assertion)\u4e0d\u6b63\u78ba\uff01- {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "boolean(...) \u5f15\u6578\u5728 19990709 XPath \u521d\u7a3f\u4e2d\u4e0d\u518d\u662f\u53ef\u9078\u7528\u7684\u3002"},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "\u627e\u5230 ','\uff0c\u4f46\u4e4b\u524d\u6c92\u6709\u5f15\u6578\uff01"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "\u627e\u5230 ','\uff0c\u4f46\u4e4b\u5f8c\u6c92\u6709\u5f15\u6578\uff01"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' \u6216 '.[predicate]' \u662f\u4e0d\u5408\u6cd5\u8a9e\u6cd5\u3002\u8acb\u6539\u7528 'self::node()[predicate]'\u3002"},
				new object[] {ER_ILLEGAL_AXIS_NAME, "\u4e0d\u5408\u6cd5\u8ef8\u540d\u7a31\uff1a{0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "\u4e0d\u660e\u7bc0\u9ede\u985e\u578b\uff1a{0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "\u578b\u6a23\u6587\u5b57 ({0}) \u9700\u8981\u7528\u5f15\u865f\u62ec\u4f4f\uff01"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} \u7121\u6cd5\u683c\u5f0f\u5316\u70ba\u6578\u5b57\uff01"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "\u7121\u6cd5\u5efa\u7acb XML TransformerFactory Liaison\uff1a{0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "\u932f\u8aa4\uff01\u6c92\u6709\u627e\u5230 xpath select \u8868\u793a\u5f0f (-select)\u3002"},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "\u932f\u8aa4\uff01\u5728 OP_LOCATIONPATH \u4e4b\u5f8c\u627e\u4e0d\u5230 ENDOP"},
				new object[] {ER_ERROR_OCCURED, "\u767c\u751f\u932f\u8aa4\uff01"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "\u63d0\u4f9b\u7d66\u8b8a\u6578\u7684 VariableReference \u8d85\u51fa\u74b0\u5883\u5b9a\u7fa9\u6216\u6c92\u6709\u5b9a\u7fa9\uff01\u540d\u7a31 = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "\u6bd4\u5c0d\u578b\u6a23\u4e2d\u53ea\u63a5\u53d7 child:: \u4ee5\u53ca attribute:: \u5169\u7a2e\u8ef8\uff01\u4e0d\u7576\u7684\u8ef8 = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() \u542b\u6709\u4e0d\u6b63\u78ba\u5f15\u6578\u6578\u76ee\u3002"},
				new object[] {ER_COUNT_TAKES_1_ARG, "count \u51fd\u6578\u53ea\u9700\u8981\u4e00\u500b\u5f15\u6578\uff01"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "\u627e\u4e0d\u5230\u51fd\u6578\uff1a{0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "\u4e0d\u652f\u63f4\u7de8\u78bc\uff1a{0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "getNextSibling \u6642\u5728 DTM \u767c\u751f\u554f\u984c... \u5617\u8a66\u56de\u5fa9"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "\u7a0b\u5f0f\u8a2d\u8a08\u5e2b\u932f\u8aa4\uff1a\u7121\u6cd5\u5beb\u5165 EmptyNodeList\u3002"},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory \u4e0d\u53d7 XPathContext \u652f\u63f4\uff01"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u5b57\u9996\u5fc5\u9808\u89e3\u6790\u70ba\u540d\u7a31\u7a7a\u9593\uff1a{0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "\u5728 XPathContext \u4e2d\u4e0d\u652f\u63f4\u5256\u6790\uff08InputSource \u539f\u59cb\u6a94\uff09\uff01\u7121\u6cd5\u958b\u555f {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API character(char ch[]... \u4e0d\u80fd\u88ab DTM \u8655\u7406\uff01"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... \u4e0d\u80fd\u88ab DTM \u8655\u7406\uff01"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison \u4e0d\u80fd\u8655\u7406 {0} \u985e\u578b\u7684\u7bc0\u9ede"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper \u4e0d\u80fd\u8655\u7406 {0} \u985e\u578b\u7684\u7bc0\u9ede"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse \u932f\u8aa4\uff1aSystemID - {0} \u884c - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "DOM2Helper.parse \u932f\u8aa4"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u5075\u6e2c\u5230\u7121\u6548\u7684 UTF-16 \u4ee3\u7406\uff1a{0}?"},
				new object[] {ER_OIERROR, "IO \u932f\u8aa4"},
				new object[] {ER_CANNOT_CREATE_URL, "\u7121\u6cd5\u91dd\u5c0d\uff1a{0} \u5efa\u7acb URL"},
				new object[] {ER_XPATH_READOBJECT, "\u4f4d\u65bc XPath.readObject\uff1a{0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "\u627e\u4e0d\u5230\u51fd\u6578\u8a18\u865f\u3002"},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "\u7121\u6cd5\u8655\u7406 XPath \u985e\u578b\uff1a{0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "\u6b64 NodeSet \u4e0d\u662f\u6613\u8b8a\u7684"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "\u6b64 NodeSetDTM \u4e0d\u662f\u6613\u8b8a\u7684"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "\u8b8a\u6578\u7121\u6cd5\u89e3\u6790\uff1a{0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u7a7a\u503c\u932f\u8aa4\u8655\u7406\u7a0b\u5f0f"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "\u7a0b\u5f0f\u8a2d\u8a08\u5e2b\u7684\u78ba\u8a8d\uff1a\u4e0d\u660e\u4f5c\u696d\u78bc\uff1a{0}"},
				new object[] {ER_ZERO_OR_ONE, "0 \u6216 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() \u4e0d\u53d7 XRTreeFragSelectWrapper \u652f\u63f4"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() \u4e0d\u53d7 XRTreeFragSelectWrapper \u652f\u63f4"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() \u4e0d\u53d7 XRTreeFragSelectWrapper \u652f\u63f4"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() \u4e0d\u53d7 XRTreeFragSelectWrapper \u652f\u63f4"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() \u4e0d\u53d7 XRTreeFragSelectWrapper \u652f\u63f4"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() \u4e0d\u53d7 XRTreeFragSelectWrapper \u652f\u63f4"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() \u4e0d\u53d7 XStringForChars \u652f\u63f4"},
				new object[] {ER_COULD_NOT_FIND_VAR, "\u627e\u4e0d\u5230\u540d\u7a31\u70ba {0} \u7684\u8b8a\u6578"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars \u4e0d\u63a5\u53d7\u5b57\u4e32\u4f5c\u70ba\u5f15\u6578"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "FastStringBuffer \u5f15\u6578\u4e0d\u53ef\u70ba\u7a7a\u503c"},
				new object[] {ER_TWO_OR_THREE, "2 \u6216 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "\u8b8a\u6578\u5728\u9023\u7d50\u4e4b\u524d\u5373\u88ab\u5b58\u53d6\uff01"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB \u4e0d\u53ef\u4f7f\u7528\u5b57\u4e32\u4f5c\u70ba\u5f15\u6578\uff01"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n!!!! \u932f\u8aa4\uff01\u8a2d\u5b9a Walker \u7684\u6839\u76ee\u9304\u70ba\u7a7a\u503c!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "\u6b64 NodeSetDTM \u4e0d\u53ef\u758a\u4ee3\u70ba\u524d\u4e00\u500b\u7bc0\u9ede\uff01"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "\u6b64 NodeSet \u4e0d\u53ef\u758a\u4ee3\u70ba\u524d\u4e00\u500b\u7bc0\u9ede\uff01"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "\u6b64 NodeSetDTM \u4e0d\u53ef\u57f7\u884c\u6aa2\u7d22\u6216\u8a08\u6578\u529f\u80fd\uff01"},
				new object[] {ER_NODESET_CANNOT_INDEX, "\u6b64 NodeSet \u4e0d\u53ef\u57f7\u884c\u6aa2\u7d22\u6216\u8a08\u6578\u529f\u80fd\uff01"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "\u5728\u547c\u53eb nextNode \u4e4b\u5f8c\u4e0d\u80fd\u547c\u53eb setShouldCacheNodes\u3002"},
				new object[] {ER_ONLY_ALLOWS, "{0} \u53ea\u5141\u8a31 {1} \u5f15\u6578"},
				new object[] {ER_UNKNOWN_STEP, "\u7a0b\u5f0f\u8a2d\u8a08\u5e2b\u5728 getNextStepPos \u4e2d\u7684\u78ba\u8a8d\uff1a\u4e0d\u660e\u7684 stepType\uff1a{0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "\u9810\u671f\u5728 '/' \u6216 '//' \u8a18\u865f\u4e4b\u5f8c\u70ba\u76f8\u5c0d\u7684\u4f4d\u7f6e\u8def\u5f91\u3002"},
				new object[] {ER_EXPECTED_LOC_PATH, "\u5fc5\u9808\u662f\u4f4d\u7f6e\u8def\u5f91\uff0c\u537b\u9047\u5230\u4e0b\u5217\u8a18\u865f\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "\u539f\u9810\u671f\u70ba\u4f4d\u7f6e\u8def\u5f91\uff0c\u4f46\u627e\u5230\u7684\u537b\u662f XPath \u8868\u793a\u5f0f\u7684\u7d50\u5c3e\u3002"},
				new object[] {ER_EXPECTED_LOC_STEP, "\u9810\u671f\u5728 '/' \u6216 '//' \u8a18\u865f\u4e4b\u5f8c\u70ba location step\u3002"},
				new object[] {ER_EXPECTED_NODE_TEST, "\u539f\u9810\u671f\u70ba\u7b26\u5408 NCName:* \u6216 QName \u7684 node test\u3002"},
				new object[] {ER_EXPECTED_STEP_PATTERN, "\u539f\u9810\u671f\u70ba step pattern\uff0c\u4f46\u537b\u9047\u5230 '/'\u3002"},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "\u539f\u9810\u671f\u70ba\u76f8\u5c0d\u7684\u8def\u5f91\u578b\u6a23\u3002"},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u6709\u7121\u6cd5\u8f49\u63db\u70ba boolean \u7684 {1} \u7684 XPathResultType\u3002"},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u6709\u7121\u6cd5\u8f49\u63db\u70ba\u55ae\u4e00\u7bc0\u9ede\u7684 {1} \u7684 XPathResultType\u3002\u65b9\u6cd5 getSingleNodeValue \u50c5\u9069\u7528\u65bc ANY_UNORDERED_NODE_TYPE \u53ca FIRST_ORDERED_NODE_TYPE \u5169\u7a2e\u985e\u578b\u3002"},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "\u7121\u6cd5\u5728 XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u4e0a\u547c\u53eb\u65b9\u6cd5 getSnapshotLength\uff0c\u56e0\u70ba\u5b83\u7684 XPathResultType \u662f {1}\u3002\u6b64\u65b9\u6cd5\u50c5\u9069\u7528\u65bc UNORDERED_NODE_SNAPSHOT_TYPE \u53ca ORDERED_NODE_SNAPSHOT_TYPE \u5169\u7a2e\u985e\u578b\u3002"},
				new object[] {ER_NON_ITERATOR_TYPE, "\u7121\u6cd5\u5728 XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u4e0a\u547c\u53eb\u65b9\u6cd5 iterateNext\uff0c\u56e0\u70ba\u5b83\u7684 XPathResultType \u662f {1}\u3002\u6b64\u65b9\u6cd5\u50c5\u9069\u7528\u65bc UNORDERED_NODE_ITERATOR_TYPE \u53ca ORDERED_NODE_ITERATOR_TYPE \u5169\u7a2e\u985e\u578b\u3002"},
				new object[] {ER_DOC_MUTATED, "\u81ea\u50b3\u56de\u7d50\u679c\u4e4b\u5f8c\uff0c\u6587\u4ef6\u5df2\u7522\u751f\u8b8a\u5316\u3002\u91cd\u8907\u9805\u76ee\u7121\u6548\u3002"},
				new object[] {ER_INVALID_XPATH_TYPE, "XPath \u985e\u578b\u5f15\u6578 {0} \u7121\u6548"},
				new object[] {ER_EMPTY_XPATH_RESULT, "XPath \u7d50\u679c\u7269\u4ef6\u7a7a\u767d"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u6709\u7121\u6cd5\u5f37\u5236\u7f6e\u5165 {2} \u7684\u6307\u5b9a XPathResultType \u4e2d\u7684 {1} \u7684 XPathResultType\u3002"},
				new object[] {ER_NULL_RESOLVER, "\u7121\u6cd5\u89e3\u6790\u542b\u7a7a\u503c\u5b57\u9996\u89e3\u6790\u5668\u7684\u5b57\u9996\u3002"},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u6709\u7121\u6cd5\u8f49\u63db\u70ba\u5b57\u4e32\u7684 {1} \u7684 XPathResultType\u3002"},
				new object[] {ER_NON_SNAPSHOT_TYPE, "\u7121\u6cd5\u5728 XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u4e0a\u547c\u53eb\u65b9\u6cd5 snapshotItem\uff0c\u56e0\u70ba\u5b83\u7684 XPathResultType \u662f {1}\u3002\u6b64\u65b9\u6cd5\u50c5\u9069\u7528\u65bc UNORDERED_NODE_SNAPSHOT_TYPE \u53ca ORDERED_NODE_SNAPSHOT_TYPE \u5169\u7a2e\u985e\u578b\u3002"},
				new object[] {ER_WRONG_DOCUMENT, "\u74b0\u5883\u5b9a\u7fa9\u7bc0\u9ede\u4e0d\u5c6c\u65bc\u548c\u6b64 XPathEvaluator \u9023\u7d50\u7684\u6587\u4ef6\u3002"},
				new object[] {ER_WRONG_NODETYPE, "\u74b0\u5883\u5b9a\u7fa9\u7bc0\u9ede\u985e\u578b\u672a\u53d7\u652f\u63f4\u3002"},
				new object[] {ER_XPATH_ERROR, "XPath \u767c\u751f\u4e0d\u660e\u932f\u8aa4\u3002"},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPath \u8868\u793a\u5f0f ''{0}'' \u7684 XPathResult \u6709\u7121\u6cd5\u8f49\u63db\u70ba\u6578\u5b57\u7684 {1} \u7684 XPathResultType\u3002"},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "\u7576 XMLConstants.FEATURE_SECURE_PROCESSING \u7279\u6027\u8a2d\u70ba true \u6642\uff0c\u7121\u6cd5\u547c\u53eb\u5ef6\u4f38\u51fd\u6578\uff1a''{0}''\u3002"},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "\u8b8a\u6578 {0} \u7684 resolveVariable \u50b3\u56de\u7a7a\u503c"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "\u4e0d\u53d7\u652f\u63f4\u7684\u50b3\u56de\u985e\u578b\uff1a{0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "\u539f\u59cb\u6a94\u53ca/\u6216\u50b3\u56de\u985e\u578b\u4e0d\u53ef\u70ba\u7a7a\u503c"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "{0} \u5f15\u6578\u4e0d\u53ef\u70ba\u7a7a\u503c"},
				new object[] {ER_OBJECT_MODEL_NULL, "\u7576 objectModel == null \u6642\u7121\u6cd5\u547c\u53eb {0}#isObjectModelSupported(String objectModel )"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "\u7576 objectModel == \"\" \u6642\u7121\u6cd5\u547c\u53eb {0}#isObjectModelSupported(String objectModel )"},
				new object[] {ER_FEATURE_NAME_NULL, "\u5617\u8a66\u8a2d\u5b9a\u4f7f\u7528\u7a7a\u503c\u540d\u7a31\u7684\u7279\u6027\uff1a{0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "\u5617\u8a66\u8a2d\u5b9a\u4e0d\u660e\u7279\u6027 \"{0}\"\uff1a{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "\u5617\u8a66\u53d6\u5f97\u4f7f\u7528\u7a7a\u503c\u540d\u7a31\u7684\u7279\u6027\uff1a{0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "\u5617\u8a66\u53d6\u5f97\u4e0d\u660e\u7279\u6027 \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "\u5617\u8a66\u8a2d\u5b9a\u7a7a\u503c XPathFunctionResolver\uff1a{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "\u5617\u8a66\u8a2d\u5b9a\u7a7a\u503c XPathVariableResolver\uff1a{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "format-number \u51fd\u6578\u4e2d\u7684\u8a9e\u8a00\u74b0\u5883\u540d\u7a31\u5c1a\u672a\u8655\u7406\uff01"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "XSL \u5167\u5bb9\u672a\u53d7\u652f\u63f4\uff1a{0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "\u76ee\u524d\u4e0d\u8981\u5c0d\u5167\u5bb9\uff1a{1} \u4e2d\u7684\u540d\u7a31\u7a7a\u9593 {0} \u505a\u4efb\u4f55\u52d5\u4f5c"},
				new object[] {WG_SECURITY_EXCEPTION, "\u5617\u8a66\u5b58\u53d6 XSL \u7cfb\u7d71\u5167\u5bb9\uff1a{0} \u6642\u767c\u751f SecurityException"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "XPath \u4e2d\u5df2\u4e0d\u518d\u5b9a\u7fa9\u820a\u8a9e\u6cd5\uff1aquo(...)\u3002"},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath \u9700\u8981\u884d\u751f\u7269\u4ef6\u4f86\u5be6\u4f5c nodeTest\uff01"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "\u627e\u4e0d\u5230\u51fd\u6578\u8a18\u865f\u3002"},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "\u627e\u4e0d\u5230\u51fd\u6578\uff1a{0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "\u7121\u6cd5\u5f9e\uff1a{0} \u7522\u751f URL"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "-E \u9078\u9805\u4e0d\u53d7 DTM \u5256\u6790\u5668\u652f\u63f4"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "\u63d0\u4f9b\u7d66\u8b8a\u6578\u7684 VariableReference \u8d85\u51fa\u74b0\u5883\u5b9a\u7fa9\u6216\u6c92\u6709\u5b9a\u7fa9\uff01\u540d\u7a31 = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "\u4e0d\u652f\u63f4\u7de8\u78bc\uff1a{0}"},
				new object[] {"ui_language", "zh"},
				new object[] {"help_language", "zh"},
				new object[] {"language", "zh"},
				new object[] {"BAD_CODE", "createMessage \u7684\u53c3\u6578\u8d85\u51fa\u754c\u9650"},
				new object[] {"FORMAT_FAILED", "\u5728 messageFormat \u547c\u53eb\u671f\u9593\u64f2\u51fa\u7570\u5e38"},
				new object[] {"version", ">>>>>>> Xalan \u7248\u672c"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "yes"},
				new object[] {"line", "\u884c\u865f"},
				new object[] {"column", "\u6b04\u865f"},
				new object[] {"xsldone", "XSLProcessor\uff1a\u5b8c\u6210"},
				new object[] {"xpath_option", "xpath \u9078\u9805\uff1a"},
				new object[] {"optionIN", "[-in inputXMLURL]"},
				new object[] {"optionSelect", "[-select xpath \u8868\u793a\u5f0f]"},
				new object[] {"optionMatch", "[-match \u7b26\u5408\u578b\u6a23\uff08\u7528\u65bc\u6bd4\u5c0d\u8a3a\u65b7\uff09]"},
				new object[] {"optionAnyExpr", "\u6216\u53ea\u6709\u4e00\u500b xpath \u8868\u793a\u5f0f\u6703\u57f7\u884c\u8a3a\u65b7\u50be\u51fa"},
				new object[] {"noParsermsg1", "XSL \u7a0b\u5e8f\u6c92\u6709\u9806\u5229\u5b8c\u6210\u3002"},
				new object[] {"noParsermsg2", "** \u627e\u4e0d\u5230\u5256\u6790\u5668 **"},
				new object[] {"noParsermsg3", "\u8acb\u6aa2\u67e5\u985e\u5225\u8def\u5f91\u3002"},
				new object[] {"noParsermsg4", "\u5982\u679c\u60a8\u6c92\u6709 IBM \u7684 XML Parser for Java\uff0c\u53ef\u81ea\u4ee5\u4e0b\u7db2\u5740\u4e0b\u8f09"},
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
	  public const string ERROR_STRING = "#error";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "\u932f\u8aa4\uff1a";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "\u8b66\u544a\uff1a";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "PATTERN ";


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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("zh", "TW"));
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