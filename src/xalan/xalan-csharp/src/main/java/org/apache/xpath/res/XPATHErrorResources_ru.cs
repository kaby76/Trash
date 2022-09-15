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
 * $Id: XPATHErrorResources_ru.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_ru : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f current() \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u0430 \u0432 \u0448\u0430\u0431\u043b\u043e\u043d\u0435 \u0434\u043b\u044f \u0441\u0440\u0430\u0432\u043d\u0435\u043d\u0438\u044f!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 current() \u043d\u0435\u0442 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432!"},
				new object[] {ER_DOCUMENT_REPLACED, "\u0420\u0435\u0430\u043b\u0438\u0437\u0430\u0446\u0438\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 document() \u0437\u0430\u043c\u0435\u043d\u0435\u043d\u0430 \u043d\u0430 org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "\u0412 \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442\u0435 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442-\u0432\u043b\u0430\u0434\u0435\u043b\u0435\u0446!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 local-name() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 namespace-uri() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 normalize-space() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 number() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 name() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 string() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 string-length() \u0441\u043b\u0438\u0448\u043a\u043e\u043c \u043c\u043d\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 translate() \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0442\u0440\u0438 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u0430!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 unparsed-entity-uri \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043e\u0434\u0438\u043d \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "\u041e\u0441\u044c \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d \u0435\u0449\u0435 \u043d\u0435 \u0440\u0435\u0430\u043b\u0438\u0437\u043e\u0432\u0430\u043d\u0430!"},
				new object[] {ER_UNKNOWN_AXIS, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0430\u044f \u043e\u0441\u044c: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0430\u044f \u043e\u043f\u0435\u0440\u0430\u0446\u0438\u044f \u0441\u0440\u0430\u0432\u043d\u0435\u043d\u0438\u044f!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u0430\u044f \u0434\u043b\u0438\u043d\u0430 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432 \u043f\u0440\u0438 \u0441\u0440\u0430\u0432\u043d\u0435\u043d\u0438\u0438 \u0443\u0437\u043b\u0430 processing-instruction()!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c {0} \u0432 \u0447\u0438\u0441\u043b\u043e"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c {0} \u0432 NodeList!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c {0} \u0432 NodeSetDTM!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c {0} \u0432 \u0442\u0438\u043f#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "\u0412 getMatchScore \u043e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u0448\u0430\u0431\u043b\u043e\u043d \u0434\u043b\u044f \u0441\u0440\u0430\u0432\u043d\u0435\u043d\u0438\u044f!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u043e\u043b\u0443\u0447\u0438\u0442\u044c \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u0443\u044e {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "\u041e\u0448\u0438\u0431\u043a\u0430! \u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u043a\u043e\u0434 \u043e\u043f\u0435\u0440\u0430\u0446\u0438\u0438: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "\u0414\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0435 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0435 \u043c\u0430\u0440\u043a\u0435\u0440\u044b: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "\u041b\u0438\u0442\u0435\u0440\u0430\u043b \u043d\u0435 \u0437\u0430\u043a\u043b\u044e\u0447\u0435\u043d \u0432 \u043a\u0430\u0432\u044b\u0447\u043a\u0438... \u041e\u0436\u0438\u0434\u0430\u043b\u0438\u0441\u044c \u0434\u0432\u043e\u0439\u043d\u044b\u0435 \u043a\u0430\u0432\u044b\u0447\u043a\u0438!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "\u041b\u0438\u0442\u0435\u0440\u0430\u043b \u043d\u0435 \u0437\u0430\u043a\u043b\u044e\u0447\u0435\u043d \u0432 \u043a\u0430\u0432\u044b\u0447\u043a\u0438... \u041e\u0436\u0438\u0434\u0430\u043b\u0438\u0441\u044c \u043e\u0434\u0438\u043d\u043e\u0447\u043d\u044b\u0435 \u043a\u0430\u0432\u044b\u0447\u043a\u0438!"},
				new object[] {ER_EMPTY_EXPRESSION, "\u041f\u0443\u0441\u0442\u043e\u0435 \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0435!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "\u041e\u0436\u0438\u0434\u0430\u043b\u043e\u0441\u044c {0}, \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u043e: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u043d\u043e\u0435 \u043f\u0440\u0435\u0434\u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u0435! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "\u0412 19990709 XPath \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442 boolean(...) \u0431\u043e\u043b\u044c\u0448\u0435 \u043d\u0435 \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f \u043d\u0435\u043e\u0431\u044f\u0437\u0430\u0442\u0435\u043b\u044c\u043d\u044b\u043c."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "\u041e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u0430 \u0437\u0430\u043f\u044f\u0442\u0430\u044f ',' \u043d\u043e \u043f\u0435\u0440\u0435\u0434 \u043d\u0435\u0439 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "\u041e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u0430 \u0437\u0430\u043f\u044f\u0442\u0430\u044f ',' \u043d\u043e \u043f\u043e\u0441\u043b\u0435 \u043d\u0435\u0435 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "\u0421\u0438\u043d\u0442\u0430\u043a\u0441\u0438\u0441 '..[\u043f\u0440\u0435\u0434\u0438\u043a\u0430\u0442]' \u0438\u043b\u0438 '.[\u043f\u0440\u0435\u0434\u0438\u043a\u0430\u0442]' \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c.  \u0418\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 'self::node()[\u043f\u0440\u0435\u0434\u0438\u043a\u0430\u0442]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0438\u043c\u044f \u043e\u0441\u0438: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u0442\u0438\u043f \u0443\u0437\u043b\u0430: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "\u0412 \u0448\u0430\u0431\u043b\u043e\u043d\u0435 \u043b\u0438\u0442\u0435\u0440\u0430\u043b ({0}) \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0437\u0430\u043a\u043b\u044e\u0447\u0435\u043d \u0432 \u043a\u0430\u0432\u044b\u0447\u043a\u0438!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043e\u0442\u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u043a\u0430\u043a \u0447\u0438\u0441\u043b\u043e!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0441\u043e\u0437\u0434\u0430\u0442\u044c XML TransformerFactory Liaison: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "\u041e\u0448\u0438\u0431\u043a\u0430! \u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u043e \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0435 \u0432\u044b\u0431\u043e\u0440\u0430 xpath (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "\u041e\u0448\u0438\u0431\u043a\u0430! \u041f\u043e\u0441\u043b\u0435 OP_LOCATIONPATH \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 ENDOP"},
				new object[] {ER_ERROR_OCCURED, "\u041e\u0448\u0438\u0431\u043a\u0430!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference \u0434\u043b\u044f \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u043e\u0439 \u0437\u0430\u0434\u0430\u043d \u0432\u043d\u0435 \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442\u0430 \u0438\u043b\u0438 \u0431\u0435\u0437 \u043e\u043f\u0440\u0435\u0434\u0435\u043b\u0435\u043d\u0438\u044f!  \u0418\u043c\u044f = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "\u0412 \u0448\u0430\u0431\u043b\u043e\u043d\u0430\u0445 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u044f \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b \u0442\u043e\u043b\u044c\u043a\u043e \u043e\u0441\u0438 child:: \u0438 attribute::!  \u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0435 \u043e\u0441\u0438 = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "\u0412 key() \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u0447\u0438\u0441\u043b\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432."},
				new object[] {ER_COUNT_TAKES_1_ARG, "\u0423 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 count \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043e\u0434\u0438\u043d \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "\u041d\u0435\u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u043c\u0430\u044f \u043a\u043e\u0434\u0438\u0440\u043e\u0432\u043a\u0430: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "\u041e\u0448\u0438\u0431\u043a\u0430 \u0432 DTM \u0432 getNextSibling... \u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0432\u043e\u0441\u0441\u0442\u0430\u043d\u043e\u0432\u043b\u0435\u043d\u0438\u044f"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "\u041f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u043d\u0430\u044f \u043e\u0448\u0438\u0431\u043a\u0430: \u0437\u0430\u043f\u0438\u0441\u044c \u0432 EmptyNodeList \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u0430."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f XPathContext!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u041f\u0440\u0435\u0444\u0438\u043a\u0441 \u0434\u043e\u043b\u0436\u0435\u043d \u043e\u0431\u0435\u0441\u043f\u0435\u0447\u0438\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435 \u0432 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u043e \u0438\u043c\u0435\u043d: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "\u0410\u043d\u0430\u043b\u0438\u0437 \u0441 (InputSource \u0438\u0441\u0442\u043e\u0447\u043d\u0438\u043a) \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 XPathContext! \u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043e\u0442\u043a\u0440\u044b\u0442\u044c {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API characters(char ch[]... \u043d\u0435 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0430\u043d DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... \u043d\u0435 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0430\u043d DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u043e\u0431\u0440\u0430\u0431\u0430\u0442\u044b\u0432\u0430\u0442\u044c \u0443\u0437\u043b\u044b \u0442\u0438\u043f\u0430 {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u043e\u0431\u0440\u0430\u0431\u0430\u0442\u044b\u0432\u0430\u0442\u044c \u0443\u0437\u043b\u044b \u0442\u0438\u043f\u0430 {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "\u041e\u0448\u0438\u0431\u043a\u0430 DOM2Helper.parse: SystemID - {0} \u0441\u0442\u0440\u043e\u043a\u0430 - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u041e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 UTF-16: {0} ?"},
				new object[] {ER_OIERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 \u0432\u0432\u043e\u0434\u0430-\u0432\u044b\u0432\u043e\u0434\u0430"},
				new object[] {ER_CANNOT_CREATE_URL, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c URL \u0434\u043b\u044f {0}"},
				new object[] {ER_XPATH_READOBJECT, "\u0412 XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "\u041c\u0430\u0440\u043a\u0435\u0440 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "\u0420\u0430\u0431\u043e\u0442\u0430 \u0441 \u0442\u0438\u043f\u043e\u043c XPath \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u0430: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "\u0414\u0430\u043d\u043d\u044b\u0439 \u043d\u0430\u0431\u043e\u0440 NodeSet \u043d\u0435 \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f \u0434\u0432\u0443\u0441\u0442\u043e\u0440\u043e\u043d\u043d\u0438\u043c"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "\u0414\u0430\u043d\u043d\u044b\u0439 \u043d\u0430\u0431\u043e\u0440 NodeSetDTM \u043d\u0435 \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f \u0434\u0432\u0443\u0441\u0442\u043e\u0440\u043e\u043d\u043d\u0438\u043c"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u0443\u044e: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u041f\u0443\u0441\u0442\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0447\u0438\u043a \u043e\u0448\u0438\u0431\u043a\u0438"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "\u0417\u0430\u043f\u0438\u0441\u044c \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u0438\u0441\u0442\u0430: \u043d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u043a\u043e\u0434 \u043e\u043f\u0446\u0438\u0438: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 \u0438\u043b\u0438 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 XRTreeFragSelectWrapper"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 XRTreeFragSelectWrapper"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 XRTreeFragSelectWrapper"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 XRTreeFragSelectWrapper"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "\u041f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u0430\u044f {0} \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "\u0410\u0440\u0433\u0443\u043c\u0435\u043d\u0442 XStringForChars \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u0441\u0442\u0440\u043e\u043a\u043e\u0439"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "\u0410\u0440\u0433\u0443\u043c\u0435\u043d\u0442 FastStringBuffer \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_TWO_OR_THREE, "2 \u0438\u043b\u0438 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "\u041e\u0431\u0440\u0430\u0449\u0435\u043d\u0438\u0435 \u043a \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u043e\u0439 \u0434\u043e \u0435\u0435 \u0441\u0432\u044f\u0437\u044b\u0432\u0430\u043d\u0438\u044f!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "\u0410\u0440\u0433\u0443\u043c\u0435\u043d\u0442 XStringForFSB \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u0441\u0442\u0440\u043e\u043a\u043e\u0439!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! \u041e\u0448\u0438\u0431\u043a\u0430! \u041a\u043e\u0440\u043d\u0435\u0432\u043e\u043c\u0443 \u043a\u0430\u0442\u0430\u043b\u043e\u0433\u0443 walker \u043f\u0440\u0438\u0441\u0432\u043e\u0435\u043d\u043e \u043f\u0443\u0441\u0442\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "\u0414\u0430\u043d\u043d\u044b\u0439 NodeSetDTM \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0440\u0430\u0431\u043e\u0442\u0430\u0442\u044c \u0441 \u043f\u0440\u0435\u0434\u044b\u0434\u0443\u0449\u0438\u043c \u0443\u0437\u043b\u043e\u043c!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "\u0414\u0430\u043d\u043d\u044b\u0439 NodeSet \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0440\u0430\u0431\u043e\u0442\u0430\u0442\u044c \u0441 \u043f\u0440\u0435\u0434\u044b\u0434\u0443\u0449\u0438\u043c \u0443\u0437\u043b\u043e\u043c!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "\u0414\u0430\u043d\u043d\u044b\u0439 NodeSetDTM \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0432\u044b\u043f\u043e\u043b\u043d\u044f\u0442\u044c \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0438\u043d\u0434\u0435\u043a\u0441\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u044f \u0438 \u043f\u043e\u0434\u0441\u0447\u0435\u0442\u0430!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "\u0414\u0430\u043d\u043d\u044b\u0439 NodeSet \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0432\u044b\u043f\u043e\u043b\u043d\u044f\u0442\u044c \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0438\u043d\u0434\u0435\u043a\u0441\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u044f \u0438 \u043f\u043e\u0434\u0441\u0447\u0435\u0442\u0430!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "\u041d\u0435\u043b\u044c\u0437\u044f \u0432\u044b\u0437\u044b\u0432\u0430\u0442\u044c setShouldCacheNodes \u043f\u043e\u0441\u043b\u0435 \u0432\u044b\u0437\u043e\u0432\u0430 nextNode!"},
				new object[] {ER_ONLY_ALLOWS, "\u041c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e\u0435 \u0447\u0438\u0441\u043b\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432 {0} \u0440\u0430\u0432\u043d\u043e {1}"},
				new object[] {ER_UNKNOWN_STEP, "\u0417\u0430\u043f\u0438\u0441\u044c \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u0438\u0441\u0442\u0430 \u0432 getNextStepPos: \u043d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 stepType: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u043e\u0442\u043d\u043e\u0441\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0439 \u043f\u0443\u0442\u044c, \u043f\u043e\u0441\u043b\u0435 \u043a\u043e\u0442\u043e\u0440\u043e\u0433\u043e \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u043b \u0441\u043b\u0435\u0434\u043e\u0432\u0430\u0442\u044c \u043c\u0430\u0440\u043a\u0435\u0440 '/' \u0438\u043b\u0438 '//'."},
				new object[] {ER_EXPECTED_LOC_PATH, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u043f\u0443\u0442\u044c, \u043e\u0434\u043d\u0430\u043a\u043e \u0431\u044b\u043b \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0438\u0439 \u043c\u0430\u0440\u043a\u0435\u0440\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u043f\u0443\u0442\u044c \u043a \u0440\u0430\u0441\u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u044e, \u043d\u043e \u0432\u043c\u0435\u0441\u0442\u043e \u044d\u0442\u043e\u0433\u043e \u0431\u044b\u043b \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d \u043a\u043e\u043d\u0435\u0446 \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath. "},
				new object[] {ER_EXPECTED_LOC_STEP, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u0448\u0430\u0433 \u0440\u0430\u0441\u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u044f, \u043f\u043e\u0441\u043b\u0435 \u043a\u043e\u0442\u043e\u0440\u043e\u0433\u043e \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u043b \u0441\u043b\u0435\u0434\u043e\u0432\u0430\u0442\u044c \u043c\u0430\u0440\u043a\u0435\u0440 '/' \u0438\u043b\u0438 '//'."},
				new object[] {ER_EXPECTED_NODE_TEST, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u0442\u0435\u0441\u0442 \u0443\u0437\u043b\u0430, \u0441\u043e\u0432\u043f\u0430\u0434\u0430\u044e\u0449\u0435\u0433\u043e \u0441 NCName:* \u0438\u043b\u0438 QName. "},
				new object[] {ER_EXPECTED_STEP_PATTERN, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u0448\u0430\u0431\u043b\u043e\u043d \u0448\u0430\u0433\u0430, \u043e\u0434\u043d\u0430\u043a\u043e \u0431\u044b\u043b \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "\u041e\u0436\u0438\u0434\u0430\u043b\u0441\u044f \u0448\u0430\u0431\u043b\u043e\u043d \u043e\u0442\u043d\u043e\u0441\u0438\u0442\u0435\u043b\u044c\u043d\u043e\u0433\u043e \u043f\u0443\u0442\u0438."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "\u0412 XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'' \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 XPathResultType \u0440\u0430\u0432\u043d\u043e {1}, \u0447\u0442\u043e \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0431\u0443\u043b\u0435\u0432\u0441\u043a\u043e\u0435  \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435. "},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "\u0412 XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'' \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 XPathResultType \u0440\u0430\u0432\u043d\u043e {1}, \u0447\u0442\u043e \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0443\u0437\u0435\u043b. \u041c\u0435\u0442\u043e\u0434 getSingleNodeValue \u043f\u0440\u0438\u043c\u0435\u043d\u0438\u043c \u0442\u043e\u043b\u044c\u043a\u043e \u043a \u0442\u0438\u043f\u0430\u043c ANY_UNORDERED_NODE_TYPE \u0438 FIRST_ORDERED_NODE_TYPE. "},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "\u041c\u0435\u0442\u043e\u0434 getSnapshotLength \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c \u0434\u043b\u044f XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'', \u0442\u0430\u043a \u043a\u0430\u043a \u0435\u0433\u043e XPathResultType \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f {1}. \u042d\u0442\u043e\u0442 \u043c\u0435\u0442\u043e\u0434 \u043f\u0440\u0438\u043c\u0435\u043d\u0438\u043c \u0442\u043e\u043b\u044c\u043a\u043e \u043a \u0442\u0438\u043f\u0430\u043c UNORDERED_NODE_SNAPSHOT_TYPE \u0438 ORDERED_NODE_SNAPSHOT_TYPE. "},
				new object[] {ER_NON_ITERATOR_TYPE, "\u041c\u0435\u0442\u043e\u0434 iterateNext \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c \u0434\u043b\u044f XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'', \u0442\u0430\u043a \u043a\u0430\u043a \u0435\u0433\u043e XPathResultType \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f {1}. \u042d\u0442\u043e\u0442 \u043c\u0435\u0442\u043e\u0434 \u043f\u0440\u0438\u043c\u0435\u043d\u0438\u043c \u0442\u043e\u043b\u044c\u043a\u043e \u043a \u0442\u0438\u043f\u0430\u043c UNORDERED_NODE_ITERATOR_TYPE \u0438 ORDERED_NODE_ITERATOR_TYPE. "},
				new object[] {ER_DOC_MUTATED, "\u0421 \u043c\u043e\u043c\u0435\u043d\u0442\u0430 \u043f\u043e\u043b\u0443\u0447\u0435\u043d\u0438\u044f \u0440\u0435\u0437\u0443\u043b\u044c\u0442\u0430\u0442\u0430 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442 \u0431\u044b\u043b \u0438\u0437\u043c\u0435\u043d\u0435\u043d. \u0418\u0442\u0435\u0440\u0430\u0442\u043e\u0440 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c."},
				new object[] {ER_INVALID_XPATH_TYPE, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0442\u0438\u043f \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u0430 XPath: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "\u041f\u0443\u0441\u0442\u043e\u0439 \u043e\u0431\u044a\u0435\u043a\u0442 \u0440\u0435\u0437\u0443\u043b\u044c\u0442\u0430\u0442\u0430 XPath"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'' \u0438\u043c\u0435\u0435\u0442 XPathResultType {1}, \u043a\u043e\u0442\u043e\u0440\u043e\u0435 \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0443\u043a\u0430\u0437\u0430\u043d\u043d\u044b\u0439 XPathResultType {2}. "},
				new object[] {ER_NULL_RESOLVER, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u0441 \u043f\u043e\u043c\u043e\u0449\u044c\u044e \u043f\u0443\u0441\u0442\u043e\u0433\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u044f."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "\u0412 XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'' \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 XPathResultType \u0440\u0430\u0432\u043d\u043e {1}, \u0447\u0442\u043e \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0441\u0442\u0440\u043e\u043a\u043e\u0432\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435. "},
				new object[] {ER_NON_SNAPSHOT_TYPE, "\u041c\u0435\u0442\u043e\u0434 snapshotItem \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c \u0434\u043b\u044f XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'', \u0442\u0430\u043a \u043a\u0430\u043a \u0435\u0433\u043e XPathResultType \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f {1}. \u042d\u0442\u043e\u0442 \u043c\u0435\u0442\u043e\u0434 \u043f\u0440\u0438\u043c\u0435\u043d\u0438\u043c \u0442\u043e\u043b\u044c\u043a\u043e \u043a \u0442\u0438\u043f\u0430\u043c UNORDERED_NODE_SNAPSHOT_TYPE \u0438 ORDERED_NODE_SNAPSHOT_TYPE. "},
				new object[] {ER_WRONG_DOCUMENT, "\u0423\u0437\u0435\u043b \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442\u0430 \u043d\u0435 \u043e\u0442\u043d\u043e\u0441\u0438\u0442\u0441\u044f \u043a \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0443, \u0441\u0432\u044f\u0437\u0430\u043d\u043d\u043e\u043c\u0443 \u0441 \u0434\u0430\u043d\u043d\u044b\u043c XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "\u0422\u0438\u043f \u0443\u0437\u043b\u0430 \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f."},
				new object[] {ER_XPATH_ERROR, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0430\u044f \u043e\u0448\u0438\u0431\u043a\u0430 \u0432 XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "\u0412 XPathResult \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f XPath ''{0}'' \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 XPathResultType \u0440\u0430\u0432\u043d\u043e {1}, \u0447\u0442\u043e \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0447\u0438\u0441\u043b\u043e\u0432\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435. "},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f: \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c ''{0}'', \u043a\u043e\u0433\u0434\u0430 \u0434\u043b\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 XMLConstants.FEATURE_SECURE_PROCESSING \u0437\u0430\u0434\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 true. "},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable \u0434\u043b\u044f \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u043e\u0439 {0} \u0432\u0435\u0440\u043d\u0443\u043b\u0430 \u043f\u0443\u0441\u0442\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435. "},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "\u041d\u0435\u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u043c\u044b\u0439 \u0442\u0438\u043f \u0432\u043e\u0437\u0432\u0440\u0430\u0442\u0430: {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "\u0418\u0441\u0442\u043e\u0447\u043d\u0438\u043a \u0438/\u0438\u043b\u0438 \u0442\u0438\u043f \u0432\u043e\u0437\u0432\u0440\u0430\u0442\u0430 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "\u0410\u0440\u0433\u0443\u043c\u0435\u043d\u0442 {0} \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_OBJECT_MODEL_NULL, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c {0}#isObjectModelSupported( \u0441\u0442\u0440\u043e\u043a\u0430 objectModel ) \u043f\u0440\u0438 objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c {0}#isObjectModelSupported( \u0441\u0442\u0440\u043e\u043a\u0430 objectModel ) \u043f\u0440\u0438 objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0434\u0430\u0442\u044c \u0444\u0443\u043d\u043a\u0446\u0438\u044e \u0441 \u043f\u0443\u0441\u0442\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0434\u0430\u0442\u044c \u043d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0443\u044e \u0444\u0443\u043d\u043a\u0446\u0438\u044e \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0434\u0430\u0442\u044c \u0444\u0443\u043d\u043a\u0446\u0438\u044e \u0441 \u043f\u0443\u0441\u0442\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u043f\u043e\u043b\u0443\u0447\u0438\u0442\u044c \u043d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0443\u044e \u0444\u0443\u043d\u043a\u0446\u0438\u044e \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0434\u0430\u0442\u044c \u043f\u0443\u0441\u0442\u043e\u0439 XPathFunctionResolver:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0434\u0430\u0442\u044c \u043f\u0443\u0441\u0442\u043e\u0439 XPathVariableResolver:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "\u041b\u043e\u043a\u0430\u043b\u044c\u043d\u043e\u0435 \u0438\u043c\u044f \u0432 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 format-number \u0435\u0449\u0435 \u043d\u0435 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0430\u043d\u043e!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "\u0421\u0432\u043e\u0439\u0441\u0442\u0432\u043e XSL \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "\u041d\u0435 \u0432\u044b\u043f\u043e\u043b\u043d\u044f\u0439\u0442\u0435 \u043d\u0438\u043a\u0430\u043a\u0438\u0445 \u043e\u043f\u0435\u0440\u0430\u0446\u0438\u0439 \u0441 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u043e\u043c \u0438\u043c\u0435\u043d {0} \u0432 \u0441\u0432\u043e\u0439\u0441\u0442\u0432\u0435: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException \u043f\u0440\u0438 \u043f\u043e\u043f\u044b\u0442\u043a\u0435 \u043e\u0431\u0440\u0430\u0449\u0435\u043d\u0438\u044f \u043a \u0441\u0438\u0441\u0442\u0435\u043c\u043d\u043e\u043c\u0443 \u0441\u0432\u043e\u0439\u0441\u0442\u0432\u0443 XSL: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "\u0421\u0442\u0430\u0440\u044b\u0439 \u0441\u0438\u043d\u0442\u0430\u043a\u0441\u0438\u0441: quo(...) \u0431\u043e\u043b\u044c\u0448\u0435 \u043d\u0435 \u043e\u043f\u0440\u0435\u0434\u0435\u043b\u0435\u043d \u0432 XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "\u0414\u043b\u044f \u0440\u0435\u0430\u043b\u0438\u0437\u0430\u0446\u0438\u0438 nodeTest \u0432 XPath \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u043f\u0440\u043e\u0438\u0437\u0432\u043e\u0434\u043d\u044b\u0439 \u043e\u0431\u044a\u0435\u043a\u0442!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "\u041c\u0430\u0440\u043a\u0435\u0440 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0441\u043e\u0437\u0434\u0430\u0442\u044c URL \u0438\u0437: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "\u041e\u043f\u0446\u0438\u044f -E \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440\u043e\u043c DTM"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference \u0434\u043b\u044f \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u043e\u0439 \u0437\u0430\u0434\u0430\u043d \u0432\u043d\u0435 \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442\u0430 \u0438\u043b\u0438 \u0431\u0435\u0437 \u043e\u043f\u0440\u0435\u0434\u0435\u043b\u0435\u043d\u0438\u044f!  \u0418\u043c\u044f = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "\u041d\u0435\u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u043c\u0430\u044f \u043a\u043e\u0434\u0438\u0440\u043e\u0432\u043a\u0430: {0}"},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "\u041f\u0430\u0440\u0430\u043c\u0435\u0442\u0440 createMessage \u043b\u0435\u0436\u0438\u0442 \u0432\u043d\u0435 \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0433\u043e \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d\u0430"},
				new object[] {"FORMAT_FAILED", "\u0418\u0441\u043a\u043b\u044e\u0447\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0438\u0442\u0443\u0430\u0446\u0438\u044f \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 messageFormat"},
				new object[] {"version", ">>>>>>> \u0412\u0435\u0440\u0441\u0438\u044f Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u0434\u0430"},
				new object[] {"line", "\u041d\u043e\u043c\u0435\u0440 \u0441\u0442\u0440\u043e\u043a\u0438 "},
				new object[] {"column", "\u041d\u043e\u043c\u0435\u0440 \u0441\u0442\u043e\u043b\u0431\u0446\u0430 "},
				new object[] {"xsldone", "XSLProcessor: \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d\u043e"},
				new object[] {"xpath_option", "\u041e\u043f\u0446\u0438\u0438 xpath: "},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0435 xpath]"},
				new object[] {"optionMatch", "   [-match \u0448\u0430\u0431\u043b\u043e\u043d \u0441\u0440\u0430\u0432\u043d\u0435\u043d\u0438\u044f (\u0434\u043b\u044f \u0434\u0438\u0430\u0433\u043d\u043e\u0441\u0442\u0438\u043a\u0438)]"},
				new object[] {"optionAnyExpr", "\u0418\u043b\u0438 \u043f\u0440\u043e\u0441\u0442\u043e \u0443\u043a\u0430\u0436\u0438\u0442\u0435 \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0435 xpath \u0434\u043b\u044f \u0441\u043e\u0437\u0434\u0430\u043d\u0438\u044f \u0434\u0438\u0430\u0433\u043d\u043e\u0441\u0442\u0438\u0447\u0435\u0441\u043a\u043e\u0433\u043e \u0434\u0430\u043c\u043f\u0430"},
				new object[] {"noParsermsg1", "\u0412 \u043f\u0440\u043e\u0446\u0435\u0441\u0441\u0435 XSL \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u044b \u043e\u0448\u0438\u0431\u043a\u0438."},
				new object[] {"noParsermsg2", "** \u0410\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d **"},
				new object[] {"noParsermsg3", "\u041f\u0440\u043e\u0432\u0435\u0440\u044c\u0442\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 classpath."},
				new object[] {"noParsermsg4", "\u0415\u0441\u043b\u0438 \u0443 \u0432\u0430\u0441 \u043d\u0435\u0442 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440\u0430 XML Parser for Java \u0444\u0438\u0440\u043c\u044b IBM, \u0432\u044b \u043c\u043e\u0436\u0435\u0442\u0435 \u0437\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c \u0435\u0433\u043e \u0441 \u0441\u0430\u0439\u0442\u0430"},
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
	  public const string ERROR_STRING = "\u041e\u0448\u0438\u0431\u043a\u0430";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "\u041e\u0448\u0438\u0431\u043a\u0430: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "\u041f\u0440\u0435\u0434\u0443\u043f\u0440\u0435\u0436\u0434\u0435\u043d\u0438\u0435: ";

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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static final XPATHErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XPATHErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.getDefault();
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

		string suffix = "_" + locale.getLanguage();
		string country = locale.getCountry();

		if (country.Equals("TW"))
		{
		  suffix += "_" + country;
		}

		return suffix;
	  }

	}

}