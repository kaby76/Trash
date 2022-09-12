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
 * $Id: XPATHErrorResources.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "The current() function is not allowed in a match pattern!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "The current() function does not accept arguments!"},
				new object[] {ER_DOCUMENT_REPLACED, "document() function implementation has been replaced by org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "context does not have an owner document!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() has too many arguments."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() has too many arguments."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() has too many arguments."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() has too many arguments."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() has too many arguments."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() has too many arguments."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() has too many arguments."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "The translate() function takes three arguments!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "The unparsed-entity-uri function should take one argument!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "namespace axis not implemented yet!"},
				new object[] {ER_UNKNOWN_AXIS, "unknown axis: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "unknown match operation!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "Arg length of processing-instruction() node test is incorrect!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "Can not convert {0} to a number"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "Can not convert {0} to a NodeList!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "Can not convert {0} to a NodeSetDTM!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "Can not convert {0} to a type#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "Expected match pattern in getMatchScore!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "Could not get variable named {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "ERROR! Unknown op code: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Extra illegal tokens: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "misquoted literal... expected double quote!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "misquoted literal... expected single quote!"},
				new object[] {ER_EMPTY_EXPRESSION, "Empty expression!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "Expected {0}, but found: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "Programmer assertion is incorrect! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "boolean(...) argument is no longer optional with 19990709 XPath draft."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Found ',' but no preceding argument!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Found ',' but no following argument!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' or '.[predicate]' is illegal syntax.  Use 'self::node()[predicate]' instead."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "illegal axis name: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Unknown nodetype: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "Pattern literal ({0}) needs to be quoted!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} could not be formatted to a number!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "Could not create XML TransformerFactory Liaison: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Error! Did not find xpath select expression (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "ERROR! Could not find ENDOP after OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Error occured!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference given for variable out of context or without definition!  Name = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "Only child:: and attribute:: axes are allowed in match patterns!  Offending axes = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() has an incorrect number of arguments."},
				new object[] {ER_COUNT_TAKES_1_ARG, "The count function should take one argument!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "Could not find function: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Unsupported encoding: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Problem occured in DTM in getNextSibling... trying to recover"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Programmer error: EmptyNodeList can not be written to."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory is not supported by XPathContext!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Prefix must resolve to a namespace: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (InputSource source) not supported in XPathContext! Can not open {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API characters(char ch[]... not handled by the DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... not handled by the DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison can not handle nodes of type {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper can not handle nodes of type {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse error: SystemID - {0} line - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "DOM2Helper.parse error"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Invalid UTF-16 surrogate detected: {0} ?"},
				new object[] {ER_OIERROR, "IO error"},
				new object[] {ER_CANNOT_CREATE_URL, "Cannot create url for: {0}"},
				new object[] {ER_XPATH_READOBJECT, "In XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "function token not found."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Can not deal with XPath type: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "This NodeSet is not mutable"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "This NodeSetDTM is not mutable"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Variable not resolvable: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Null error handler"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Programmer''s assertion: unknown opcode: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 or 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() not supported by XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() not supported by XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() not supported by XRTreeFragSelectWrapper"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() not supported by XRTreeFragSelectWrapper"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() not supported by XRTreeFragSelectWrapper"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() not supported by XRTreeFragSelectWrapper"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() not supported for XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "Could not find variable with the name of {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars can not take a string for an argument"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "The FastStringBuffer argument can not be null"},
				new object[] {ER_TWO_OR_THREE, "2 or 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Variable accessed before it is bound!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB can not take a string for an argument!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! Error! Setting the root of a walker to null!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "This NodeSetDTM can not iterate to a previous node!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "This NodeSet can not iterate to a previous node!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "This NodeSetDTM can not do indexing or counting functions!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "This NodeSet can not do indexing or counting functions!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "Can not call setShouldCacheNodes after nextNode has been called!"},
				new object[] {ER_ONLY_ALLOWS, "{0} only allows {1} arguments"},
				new object[] {ER_UNKNOWN_STEP, "Programmer''s assertion in getNextStepPos: unknown stepType: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "A relative location path was expected following the '/' or '//' token."},
				new object[] {ER_EXPECTED_LOC_PATH, "A location path was expected, but the following token was encountered\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "A location path was expected, but the end of the XPath expression was found instead."},
				new object[] {ER_EXPECTED_LOC_STEP, "A location step was expected following the '/' or '//' token."},
				new object[] {ER_EXPECTED_NODE_TEST, "A node test that matches either NCName:* or QName was expected."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "A step pattern was expected, but '/' was encountered."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "A relative path pattern was expected."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "The XPathResult of XPath expression ''{0}'' has an XPathResultType of {1} which cannot be converted to a boolean."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "The XPathResult of XPath expression ''{0}'' has an XPathResultType of {1} which cannot be converted to a single node. The method getSingleNodeValue applies only to types ANY_UNORDERED_NODE_TYPE and FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "The method getSnapshotLength cannot be called on the XPathResult of XPath expression ''{0}'' because its XPathResultType is {1}. This method applies only to types UNORDERED_NODE_SNAPSHOT_TYPE and ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "The method iterateNext cannot be called on the XPathResult of XPath expression ''{0}'' because its XPathResultType is {1}. This method applies only to types UNORDERED_NODE_ITERATOR_TYPE and ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "Document mutated since result was returned. Iterator is invalid."},
				new object[] {ER_INVALID_XPATH_TYPE, "Invalid XPath type argument: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Empty XPath result object"},
				new object[] {ER_INCOMPATIBLE_TYPES, "The XPathResult of XPath expression ''{0}'' has an XPathResultType of {1} which cannot be coerced into the specified XPathResultType of {2}."},
				new object[] {ER_NULL_RESOLVER, "Unable to resolve prefix with null prefix resolver."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "The XPathResult of XPath expression ''{0}'' has an XPathResultType of {1} which cannot be converted to a string."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "The method snapshotItem cannot be called on the XPathResult of XPath expression ''{0}'' because its XPathResultType is {1}. This method applies only to types UNORDERED_NODE_SNAPSHOT_TYPE and ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "Context node does not belong to the document that is bound to this XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "The context node type is not supported."},
				new object[] {ER_XPATH_ERROR, "Unknown error in XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "The XPathResult of XPath expression ''{0}'' has an XPathResultType of {1} which cannot be converted to a number"},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Extension function: ''{0}'' can not be invoked when the XMLConstants.FEATURE_SECURE_PROCESSING feature is set to true."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable for variable {0} returning null"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "UnSupported Return Type : {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Source and/or Return Type can not be null"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Source and/or Return Type can not be null"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "{0} argument can not be null"},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( String objectModel ) cannot be called with objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( String objectModel ) cannot be called with objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Trying to set a feature with a null name: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Trying to set the unknown feature \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Trying to get a feature with a null name: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Trying to get the unknown feature \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Attempting to set a null XPathFunctionResolver:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Attempting to set a null XPathVariableResolver:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "locale name in the format-number function not yet handled!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "XSL Property not supported: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "Do not currently do anything with namespace {0} in property: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException when trying to access XSL system property: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Old syntax: quo(...) is no longer defined in XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath needs a derived object to implement nodeTest!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "function token not found."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "Could not find function: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Can not make URL from: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "-E option not supported for DTM parser"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference given for variable out of context or without definition!  Name = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Unsupported encoding: {0}"},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "Parameter to createMessage was out of bounds"},
				new object[] {"FORMAT_FAILED", "Exception thrown during messageFormat call"},
				new object[] {"version", ">>>>>>> Xalan Version "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "yes"},
				new object[] {"line", "Line #"},
				new object[] {"column", "Column #"},
				new object[] {"xsldone", "XSLProcessor: done"},
				new object[] {"xpath_option", "xpath options: "},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select xpath expression]"},
				new object[] {"optionMatch", "   [-match match pattern (for match diagnostics)]"},
				new object[] {"optionAnyExpr", "Or just an xpath expression will do a diagnostic dump"},
				new object[] {"noParsermsg1", "XSL Process was not successful."},
				new object[] {"noParsermsg2", "** Could not find parser **"},
				new object[] {"noParsermsg3", "Please check your classpath."},
				new object[] {"noParsermsg4", "If you don't have IBM's XML Parser for Java, you can download it from"},
				new object[] {"noParsermsg5", "IBM's AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
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
	  public const string ERROR_HEADER = "Error: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "Warning: ";

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