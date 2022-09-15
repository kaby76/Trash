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
 * $Id: XPATHErrorResources_ko.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_ko : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "\uc77c\uce58 \ud328\ud134\uc5d0\uc11c current() \ud568\uc218\uac00 \ud5c8\uc6a9\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "current() \ud568\uc218\uac00 \uc778\uc218\ub97c \uc2b9\uc778\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_DOCUMENT_REPLACED, "document() \ud568\uc218 \uad6c\ud604\uc774 org.apache.xalan.xslt.FuncDocument\ub85c \ubc14\ub00c\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "\ubb38\ub9e5\uc5d0 \uc18c\uc720\uc790 \ubb38\uc11c\uac00 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length()\uc5d0 \ub9ce\uc740 \uc778\uc218\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "translate() \ud568\uc218\uac00 \uc138 \uac1c\uc758 \uc778\uc218\ub97c \ucde8\ud569\ub2c8\ub2e4."},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "unparsed-entity-uri \ud568\uc218\ub294 \ud558\ub098\uc758 \uc778\uc218\ub97c \ucde8\ud574\uc57c \ud569\ub2c8\ub2e4."},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "\uc774\ub984 \uacf5\uac04 \ucd95\uc774 \uc544\uc9c1 \uad6c\ud604\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_UNKNOWN_AXIS, "\uc54c \uc218 \uc5c6\ub294 \ucd95: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "\uc54c \uc218 \uc5c6\ub294 \uc77c\uce58 \uc870\uc791\uc785\ub2c8\ub2e4."},
				new object[] {ER_INCORRECT_ARG_LENGTH, "processing-instruction() node \ud14c\uc2a4\ud2b8\uc758 \uc778\uc218 \uae38\uc774\uac00 \uc62c\ubc14\ub974\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "{0}\uc744(\ub97c) \uc22b\uc790\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "{0}\uc744(\ub97c) NodeList\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "{0}\uc744(\ub97c) NodeSetDTM\uc73c\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "{0}\uc744(\ub97c) \uc720\ud615 \ubc88\ud638 {1}(\uc73c)\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "\uc608\uc0c1\ub41c getMatchScore\uc758 \ud328\ud134 \uc77c\uce58\uc785\ub2c8\ub2e4."},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "\uc774\ub984\uc774 {0}\uc778 \ubcc0\uc218\ub97c \uac00\uc838\uc62c \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_UNKNOWN_OPCODE, "\uc624\ub958. \uc54c \uc218 \uc5c6\ub294 op \ucf54\ub4dc: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "\uc720\ud6a8\ud558\uc9c0 \uc54a\uc740 \ucd94\uac00 \ud1a0\ud070: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "\ub530\uc634\ud45c\uac00 \ud2c0\ub9b0 \ub9ac\ud130\ub7f4\uc774 \uc788\uc2b5\ub2c8\ub2e4. \ud070\ub530\uc634\ud45c\uac00 \uc608\uc0c1\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "\ub530\uc634\ud45c\uac00 \ud2c0\ub9b0 \ub9ac\ud130\ub7f4\uc774 \uc788\uc2b5\ub2c8\ub2e4. \uc791\uc740\ub530\uc634\ud45c\uac00 \uc608\uc0c1\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EMPTY_EXPRESSION, "\ube48 \ud45c\ud604\uc2dd\uc785\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_BUT_FOUND, "{0}\uc744(\ub97c) \uc608\uc0c1\ud588\uc73c\ub098 {1}\uc774(\uac00) \ubc1c\uacac\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "\ud504\ub85c\uadf8\ub798\uba38 \ub2e8\uc5b8\ubb38\uc774 \uc62c\ubc14\ub974\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4. - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "19990709 XPath \ucd08\uc548\uc5d0\uc11c\ub294 \ubd80\uc6b8(...) \uc778\uc218\uac00 \ub354 \uc774\uc0c1 \uc120\ud0dd\uc801\uc774\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "','\ub97c \ubc1c\uacac\ud588\uc73c\ub098 \uadf8 \uc55e\uc5d0 \uc5b4\ub5a0\ud55c \uc778\uc218\ub3c4 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "','\ub97c \ubc1c\uacac\ud588\uc73c\ub098 \ub4a4\uc5d0 \uc5b4\ub5a0\ud55c \uc778\uc218\ub3c4 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' \ub610\ub294 '.[predicate]'\ub294 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc740 \uad6c\ubb38\uc785\ub2c8\ub2e4. \ub300\uc2e0 'self::node()[predicate]'\ub97c \uc0ac\uc6a9\ud558\uc2ed\uc2dc\uc624."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "\uc720\ud6a8\ud558\uc9c0 \uc54a\uc740 \ucd95 \uc774\ub984: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "\uc54c \uc218 \uc5c6\ub294 \ub178\ub4dc \uc720\ud615: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "\ud328\ud134 \ub9ac\ud130\ub7f4({0})\uc5d0\ub294 \ub530\uc634\ud45c\uac00 \uc788\uc5b4\uc57c \ud569\ub2c8\ub2e4."},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0}\uc740(\ub294) \uc22b\uc790\ub85c \ud3ec\ub9f7\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "XML TransformerFactory Liaison\uc744 \uc791\uc131\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "\uc624\ub958. xpath \uc120\ud0dd \ud45c\ud604\uc2dd(-select)\uc744 \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "\uc624\ub958. OP_LOCATIONPATH \ub4a4\uc5d0 ENDOP\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_ERROR_OCCURED, "\uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "\ubcc0\uc218\uc5d0 \ub300\ud574 \uc8fc\uc5b4\uc9c4 VariableReference\uac00 \ubc94\uc704\ub97c \ubc97\uc5b4\ub0ac\uac70\ub098 \uc815\uc758\uac00 \uc5c6\uc2b5\ub2c8\ub2e4. \uc774\ub984 = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "\ud558\uc704:: \ubc0f \uc18d\uc131:: \ucd95\ub9cc \ud328\ud134\uc5d0 \uc77c\uce58\ud560 \uc218 \uc788\uc2b5\ub2c8\ub2e4. \uc704\ubc18 \ucd95 = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key()\uc758 \uc778\uc218 \uc218\uac00 \uc62c\ubc14\ub974\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_COUNT_TAKES_1_ARG, "count \ud568\uc218\ub294 \ud558\ub098\uc758 \uc778\uc218\ub97c \ucde8\ud574\uc57c \ud569\ub2c8\ub2e4."},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "\ud568\uc218\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "\uc9c0\uc6d0\ub418\uc9c0 \uc54a\ub294 \uc778\ucf54\ub529: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "getNextSibling\uc758 DTM\uc5d0 \ubb38\uc81c\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4. \ubcf5\uad6c \uc2dc\ub3c4 \uc911\uc785\ub2c8\ub2e4."},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "\ud504\ub85c\uadf8\ub798\uba38 \uc624\ub958: EmptyNodeList\ub97c \uc4f8 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "XPathContext\uc5d0\uc11c setDOMFactory\ub97c \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\uc811\ub450\ubd80\ub294 \uc774\ub984 \uacf5\uac04\uc73c\ub85c \ubd84\uc11d\ub418\uc5b4\uc57c \ud569\ub2c8\ub2e4: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "XPathContext\uc5d0\uc11c \uad6c\ubb38 \ubd84\uc11d(InputSource \uc18c\uc2a4)\uc774 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4. {0}\uc744(\ub97c) \uc5f4 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API \ubb38\uc790(char ch[]... \uac00 DTM\uc5d0 \uc758\ud574 \ucc98\ub9ac\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... \uac00 DTM\uc5d0 \uc758\ud574 \ucc98\ub9ac\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison\uc774 {0} \uc720\ud615\uc758 \ub178\ub4dc\ub97c \ucc98\ub9ac\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper\uac00 {0} \uc720\ud615\uc758 \ub178\ub4dc\ub97c \ucc98\ub9ac\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse \uc624\ub958: \uc2dc\uc2a4\ud15c ID - {0} \ud68c\uc120 - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "DOM2Helper.parse \uc624\ub958"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\uc798\ubabb\ub41c UTF-16 \ub300\ub9ac\uc790(surrogate)\uac00 \ubc1c\uacac\ub418\uc5c8\uc2b5\ub2c8\ub2e4: {0} ?"},
				new object[] {ER_OIERROR, "IO \uc624\ub958"},
				new object[] {ER_CANNOT_CREATE_URL, "url\uc744 \uc791\uc131\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {ER_XPATH_READOBJECT, "XPath.readObject\uc758 {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "\ud568\uc218 \ud1a0\ud070\uc774 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "XPath \uc720\ud615\uc744 \ucc98\ub9ac\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "\uc774 NodeSet\uac00 \uac00\ubcc0\uc801\uc774\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "\uc774 NodeSetDTM\uc774 \uac00\ubcc0\uc801\uc774\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_VAR_NOT_RESOLVABLE, "\ubcc0\uc218\ub97c \ubd84\uc11d\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "\ub110(null) \uc624\ub958 \ud578\ub4e4\ub7ec"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "\ud504\ub85c\uadf8\ub798\uba38\uc758 \ub2e8\uc5b8\ubb38: \uc54c \uc218 \uc5c6\ub294 op \ucf54\ub4dc: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 \ub610\ub294 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper\uc5d0\uc11c rtf()\ub97c \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper\uc5d0\uc11c asNodeIterator()\ub97c \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper\uc5d0\uc11c detach()\ub97c \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper\uc5d0\uc11c num()\uc744 \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper\uc5d0\uc11c xstr()\uc744 \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper\uc5d0\uc11c str()\uc744 \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "XStringForChars\uc5d0 \ub300\ud574 fsb()\uac00 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_COULD_NOT_FIND_VAR, "\uc774\ub984\uc774 {0}\uc778 \ubcc0\uc218\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars\ub294 \uc778\uc218\ub85c \ubb38\uc790\uc5f4\uc744 \uac00\uc838\uc62c \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "FastStringBuffer \uc778\uc218\ub294 \ub110(null)\uc774 \ub420 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_TWO_OR_THREE, "2 \ub610\ub294 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "\ubcc0\uc218\uac00 \ubc14\uc778\ub4dc\ub418\uae30 \uc804\uc5d0 \ubcc0\uc218\uc5d0 \uc561\uc138\uc2a4\ud588\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB\ub294 \uc778\uc218\ub85c \ubb38\uc790\uc5f4\uc744 \ucde8\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! \uc624\ub958. \uc6cc\ucee4\uc758 \ub8e8\ud2b8\ub85c \ub110(null)\uc774 \uc124\uc815\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "\uc774 NodeSetDTM\uc740 \uc774\uc804 \ub178\ub4dc\uc5d0 \ubc18\ubcf5 \uc801\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NODESET_CANNOT_ITERATE, "\uc774 NodeSet\ub294 \uc774\uc804 \ub178\ub4dc\uc5d0 \ubc18\ubcf5 \uc801\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "\uc774 NodeSetDTM\uc740 \uc0c9\uc778 \ub610\ub294 \uce74\uc6b4\ud305 \ud568\uc218\ub97c \uc0ac\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NODESET_CANNOT_INDEX, "\uc774 NodeSet\ub294 \uc0c9\uc778 \ub610\ub294 \uce74\uc6b4\ud305 \ud568\uc218\ub97c \uc0ac\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "nextNode\uac00 \ud638\ucd9c\ub41c \ud6c4\uc5d0 setShouldCacheNodes\ub97c \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_ONLY_ALLOWS, "{0}\uc740(\ub294) {1} \uc778\uc218\ub9cc\uc744 \ud5c8\uc6a9\ud569\ub2c8\ub2e4."},
				new object[] {ER_UNKNOWN_STEP, "getNextStepPos\uc5d0 \ud504\ub85c\uadf8\ub798\uba38\uc758 \ub2e8\uc5b8\ubb38\uc774 \uc788\uc74c: \uc54c \uc218 \uc5c6\ub294 stepType: {0} "},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "'/' \ub610\ub294 '//' \ud1a0\ud070 \ub2e4\uc74c\uc5d0 \uad00\ub828 \uc704\uce58 \uacbd\ub85c\uac00 \uc608\uc0c1\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_LOC_PATH, "\uc704\uce58 \uacbd\ub85c\uac00 \uc608\uc0c1\ub418\uc5c8\uc9c0\ub9cc \ub2e4\uc74c \ud1a0\ud070\uc774 \ubc1c\uacac\ub418\uc5c8\uc2b5\ub2c8\ub2e4.\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "\uc704\uce58 \uacbd\ub85c\uac00 \uc608\uc0c1\ub418\uc5c8\uc9c0\ub9cc XPath \ud45c\ud604\uc2dd\uc758 \ub05d\uc774 \ubc1c\uacac\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_LOC_STEP, "'/' \ub610\ub294 '//' \ud1a0\ud070 \ub2e4\uc74c\uc5d0 \uc704\uce58 \ub2e8\uacc4\uac00 \uc608\uc0c1\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_NODE_TEST, "NCName:* \ub610\ub294 QName\uacfc \uc77c\uce58\ud558\ub294 \ub178\ub4dc \ud14c\uc2a4\ud2b8\uac00 \uc608\uc0c1\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "\ub2e8\uacc4 \ud328\ud134\uc774 \uc608\uc0c1\ub418\uc5c8\uc9c0\ub9cc '/'\uac00 \ubc1c\uacac\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "\uad00\ub828 \uacbd\ub85c \ud328\ud134\uc774 \uc608\uc0c1\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0 \ubd80\uc6b8\ub85c \ubcc0\ud658\ub420 \uc218 \uc5c6\ub294 XPathResultType {1}\uc774(\uac00) \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0 \ub2e8\uc77c \ub178\ub4dc\ub85c \ubcc0\ud658\ub420 \uc218 \uc5c6\ub294 XPathResultType {1}\uc774(\uac00) \uc788\uc2b5\ub2c8\ub2e4. \uba54\uc18c\ub4dc getSingleNodeValue\ub294 ANY_UNORDERED_NODE_TYPE \ubc0f FIRST_ORDERED_NODE_TYPE \uc720\ud615\uc5d0\ub9cc \uc801\uc6a9\ub429\ub2c8\ub2e4."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "XPathResultType\uc774 {1}\uc774\uae30 \ub54c\ubb38\uc5d0 XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0\uc11c getSnapshotLength \uba54\uc18c\ub4dc\ub97c \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. \uc774 \uba54\uc18c\ub4dc\ub294 UNORDERED_NODE_SNAPSHOT_TYPE \ubc0f ORDERED_NODE_SNAPSHOT_TYPE \uc720\ud615\uc5d0\ub9cc \uc801\uc6a9\ub429\ub2c8\ub2e4."},
				new object[] {ER_NON_ITERATOR_TYPE, "XPathResultType\uc774 {1}\uc774\uae30 \ub54c\ubb38\uc5d0 XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0\uc11c iterateNext \uba54\uc18c\ub4dc\ub97c \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. \uc774 \uba54\uc18c\ub4dc\ub294 UNORDERED_NODE_ITERATOR_TYPE \ubc0f ORDERED_NODE_ITERATOR_TYPE \uc720\ud615\uc5d0\ub9cc \uc801\uc6a9\ub429\ub2c8\ub2e4."},
				new object[] {ER_DOC_MUTATED, "\uacb0\uacfc\uac00 \ub9ac\ud134\ub418\uc5c8\uc73c\ubbc0\ub85c \ubb38\uc11c\uac00 \ubcc0\uacbd\ub429\ub2c8\ub2e4. \ubc18\ubcf5\uae30\uac00 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_INVALID_XPATH_TYPE, "\uc798\ubabb\ub41c XPath \uc720\ud615 \uc778\uc218: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "\ube44\uc5b4 \uc788\ub294 XPath \uacb0\uacfc \uc624\ube0c\uc81d\ud2b8\uc785\ub2c8\ub2e4."},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0 \uc9c0\uc815\ub41c XPathResultType {2}(\uc73c)\ub85c \uac15\uc81c \uc2dc\ud589\ud560 \uc218 \uc5c6\ub294 XPathReultType {1}\uc774(\uac00) \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NULL_RESOLVER, "\ub110(null) \uc811\ub450\ubd80 \ubd84\uc11d\uae30\ub85c \uc811\ub450\ubd80\ub97c \ubd84\uc11d\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0 \ubb38\uc790\uc5f4\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\ub294 XPathResultType {1}\uc774(\uac00) \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "XPathResultType\uc774 {1}\uc774\uae30 \ub54c\ubb38\uc5d0 XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0\uc11c snapshotItem \uba54\uc18c\ub4dc\ub97c \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. \uc774 \uba54\uc18c\ub4dc\ub294 UNORDERED_NODE_SNAPSHOT_TYPE \ubc0f ORDERED_NODE_SNAPSHOT_TYPE \uc720\ud615\uc5d0\ub9cc \uc801\uc6a9\ub429\ub2c8\ub2e4."},
				new object[] {ER_WRONG_DOCUMENT, "\ucee8\ud14d\uc2a4\ud2b8 \ub178\ub4dc\ub294 \uc774 XPathEvaluator\ub85c \ubc14\uc778\ub4dc\ub418\ub294 \ubb38\uc11c\uc5d0 \ud3ec\ud568\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_WRONG_NODETYPE, "\ucee8\ud14d\uc2a4\ud2b8 \ub178\ub4dc \uc720\ud615\uc774 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_XPATH_ERROR, "XPath\uc5d0 \uc54c \uc218 \uc5c6\ub294 \uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPath \ud45c\ud604\uc2dd ''{0}''\uc758 XPathResult\uc5d0 \uc22b\uc790\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\ub294 XPathResultType {1}\uc774(\uac00) \uc788\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "\ud655\uc7a5 \ud568\uc218: XMLConstants.FEATURE_SECURE_PROCESSING \uae30\ub2a5\uc774 true\ub85c \uc124\uc815\ub41c \uacbd\uc6b0 ''{0}''\uc744(\ub97c) \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "{0} \ubcc0\uc218\uc5d0 \ub300\ud55c resolveVariable\uc774 \ub110(null)\uc744 \ub9ac\ud134\ud569\ub2c8\ub2e4."},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "\uc9c0\uc6d0\ub418\uc9c0 \uc54a\ub294 \ub9ac\ud134 \uc720\ud615: {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "\uc18c\uc2a4 \ubc0f/\ub610\ub294 \ub9ac\ud134 \uc720\ud615\uc774 \ub110(null)\uc774\uba74 \uc548\ub429\ub2c8\ub2e4."},
				new object[] {ER_ARG_CANNOT_BE_NULL, "{0} \uc778\uc218\uac00 \ub110(null)\uc774\uba74 \uc548\ub429\ub2c8\ub2e4."},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( String objectModel )\uc740 objectModel == null\uc744 \uc0ac\uc6a9\ud558\uc5ec \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( String objectModel )\uc740 objectModel == \"\"\uc744 \uc0ac\uc6a9\ud558\uc5ec \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {ER_FEATURE_NAME_NULL, "\ub110(null) \uc774\ub984\uc744 \uc0ac\uc6a9\ud558\uc5ec \uae30\ub2a5\uc744 \uc124\uc815\ud558\ub824\uace0 \ud569\ub2c8\ub2e4: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "\uc54c \uc218 \uc5c6\ub294 \uae30\ub2a5 \"{0}\":{1}#setFeature({0},{2})\ub97c \uc124\uc815\ud558\ub824\uace0 \ud569\ub2c8\ub2e4."},
				new object[] {ER_GETTING_NULL_FEATURE, "\ub110(null) \uc774\ub984\uc744 \uc0ac\uc6a9\ud558\uc5ec \uae30\ub2a5\uc744 \uc124\uc815\ud558\ub824\uace0 \ud569\ub2c8\ub2e4: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "\uc54c \uc218 \uc5c6\ub294 \uae30\ub2a5 \"{0}\"\uc744 \uac00\uc838\uc624\ub824\uace0 \ud569\ub2c8\ub2e4: {1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "\ub110(null)\uc778 XPathFunctionResolver\ub97c \uc124\uc815\ud558\ub824\uace0 \ud569\ub2c8\ub2e4: {0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "\ub110(null)\uc778 XPathVariableResolver\ub97c \uc124\uc815\ud558\ub824\uace0 \ud569\ub2c8\ub2e4: {0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "format-number \ud568\uc218\uc5d0 \uc788\ub294 \ub85c\ucf00\uc77c \uc774\ub984\uc774 \uc544\uc9c1 \ucc98\ub9ac\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "XSL \ud2b9\uc131\uc774 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "\ud2b9\uc131\uc5d0\uc11c {0} \uc774\ub984 \uacf5\uac04\uacfc \uad00\ub828\ud558\uc5ec \ud604\uc7ac \uc544\ubb34\ub7f0 \uc791\uc5c5\ub3c4 \uc218\ud589\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "XSL \uc2dc\uc2a4\ud15c \ud2b9\uc131\uc5d0 \uc561\uc138\uc2a4\ud558\ub294 \uc911 SecurityException\uc774 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "\uc774\uc804 \uad6c\ubb38: quo(...)\uac00 \ub354 \uc774\uc0c1 XPath\uc5d0 \uc815\uc758\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "nodeTest\ub97c \uad6c\ud604\ud558\ub824\uba74 XPath\uc5d0 \ud30c\uc0dd\ub41c \uc624\ube0c\uc81d\ud2b8\uac00 \uc788\uc5b4\uc57c \ud569\ub2c8\ub2e4."},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "\ud568\uc218 \ud1a0\ud070\uc774 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "\ud568\uc218\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "{0}\uc5d0\uc11c URL\uc744 \uc791\uc131\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "DTM \uad6c\ubb38 \ubd84\uc11d\uae30\uc5d0 \ub300\ud574 -E \uc635\uc158\uc774 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "\ubcc0\uc218\uc5d0 \ub300\ud574 \uc8fc\uc5b4\uc9c4 VariableReference\uac00 \ubc94\uc704\ub97c \ubc97\uc5b4\ub0ac\uac70\ub098 \uc815\uc758\uac00 \uc5c6\uc2b5\ub2c8\ub2e4. \uc774\ub984 = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "\uc9c0\uc6d0\ub418\uc9c0 \uc54a\ub294 \uc778\ucf54\ub529: {0}"},
				new object[] {"ui_language", "ko"},
				new object[] {"help_language", "ko"},
				new object[] {"language", "ko"},
				new object[] {"BAD_CODE", "createMessage\uc5d0 \ub300\ud55c \ub9e4\uac1c\ubcc0\uc218\uac00 \ubc94\uc704\ub97c \ubc97\uc5b4\ub0ac\uc2b5\ub2c8\ub2e4."},
				new object[] {"FORMAT_FAILED", "messageFormat \ud638\ucd9c \uc911 \uc608\uc678\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				new object[] {"version", ">>>>>>> Xalan \ubc84\uc804 "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\uc608"},
				new object[] {"line", "\ud589 #"},
				new object[] {"column", "\uc5f4 #"},
				new object[] {"xsldone", "XSLProcessor: \uc644\ub8cc"},
				new object[] {"xpath_option", "xpath \uc635\uc158: "},
				new object[] {"optionIN", "[-in inputXMLURL]"},
				new object[] {"optionSelect", "[-select xpath expression]"},
				new object[] {"optionMatch", "[-match match pattern(\uc77c\uce58 \uc9c4\ub2e8\uc6a9)]"},
				new object[] {"optionAnyExpr", "\ub610\ub294 xpath \ud45c\ud604\uc2dd\ub9cc\uc73c\ub85c \uc9c4\ub2e8 \ub364\ud504\uac00 \uc218\ud589\ub420 \uac83\uc785\ub2c8\ub2e4."},
				new object[] {"noParsermsg1", "XSL \ud504\ub85c\uc138\uc2a4\uac00 \uc2e4\ud328\ud588\uc2b5\ub2c8\ub2e4."},
				new object[] {"noParsermsg2", "** \uad6c\ubb38 \ubd84\uc11d\uae30\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. **"},
				new object[] {"noParsermsg3", "\ud074\ub798\uc2a4 \uacbd\ub85c\ub97c \uc810\uac80\ud558\uc2ed\uc2dc\uc624."},
				new object[] {"noParsermsg4", "Java\uc6a9 IBM XML \uad6c\ubb38 \ubd84\uc11d\uae30\uac00 \uc5c6\uc73c\uba74"},
				new object[] {"noParsermsg5", "IBM's AlphaWorks: http://www.alphaworks.ibm.com/formula/xml\uc5d0\uc11c \ub2e4\uc6b4\ub85c\ub4dc\ud560 \uc218 \uc788\uc2b5\ub2c8\ub2e4."},
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
	  public const string ERROR_HEADER = "\uc624\ub958: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "\uacbd\uace0: ";

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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("ko", "KR"));
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