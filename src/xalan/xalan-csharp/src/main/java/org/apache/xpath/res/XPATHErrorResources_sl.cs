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
 * $Id: XPATHErrorResources_sl.java 1225426 2011-12-29 04:13:08Z mrglavas $
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
	public class XPATHErrorResources_sl : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "Funkcija current() v primerjalnem vzorcu ni dovoljena!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "Funkcija current() ne sprejema argumentov!"},
				new object[] {ER_DOCUMENT_REPLACED, "Implementacija funkcije document() je bila nadome\u0161\u010dena z org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "Kontekst ne vsebuje lastni\u0161kega dokumenta!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() ima preve\u010d argumentov."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() ima preve\u010d argumentov."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() ima preve\u010d argumentov."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() ima preve\u010d argumentov."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() ima preve\u010d argumentov."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() ima preve\u010d argumentov."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() ima preve\u010d argumentov."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "Funkcija translate() sprejme tri argumente!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "Funkcija unparsed-entity-uri bi morala vsebovati en argument!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "Os imenskega prostora \u0161e ni implementirana!"},
				new object[] {ER_UNKNOWN_AXIS, "neznana os: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "neznana operacija ujemanja!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "Dol\u017eina argumenta pri preskusu vozli\u0161\u010da s processing-instruction() ni pravilna!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "{0} ni mogo\u010de pretvoriti v \u0161tevilko"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "{0} ni mogo\u010de pretvoriti v seznam vozli\u0161\u010d (NodeList)"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "{0} ni mogo\u010de pretvoriti v NodeSetDTM"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "{0} ni mogo\u010de pretvoriti v type#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "Pri\u010dakovan primerjalni vzorec v getMatchScore!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "Nisem na\u0161el predloge z imenom {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "NAPAKA! Neznana op. koda: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Dodatni neveljavni \u017eetoni: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Napa\u010dno postavljena dobesedna navedba... pri\u010dakovani dvojni narekovaji!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Napa\u010dno postavljena dobesedna navedba... pri\u010dakovani enojni narekovaji!"},
				new object[] {ER_EMPTY_EXPRESSION, "Prazen izraz!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "Pri\u010dakovano {0}, najdeno: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "Programerjeva predpostavka ni pravilna! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "Argument logi\u010dne vrednosti(...) ni ve\u010d izbiren z osnutkom 19990709 XPath."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Najdeno ',' vendar ni predhodnih argumentov!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Najdeno ',' vendar ni slede\u010dih argumentov!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' ali '.[predicate]' je neveljavna sintaksa.  Namesto tega uporabite 'self::node()[predicate]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "Neveljavno ime osi: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Neveljavni tip vozli\u0161\u010da: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "Navedek vzorca ({0}) mora biti v navednicah!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} ni mogo\u010de oblikovati v \u0161tevilko!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "Ne morem ustvariti zveze XML TransformerFactory: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Napaka! Ne najdem izbirnega izraza xpath (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "NAPAKA! Ne najdem ENDOP po OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Pri\u0161lo je do napake!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "Dani VariableReference je izven konteksta ali brez definicije!  Ime = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "V primerjalnih vzorcih so dovoljene samo osi podrejenega:: in atributa::!  Sporne osi = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key()ima nepravilno \u0161tevilo argumentov."},
				new object[] {ER_COUNT_TAKES_1_ARG, "\u0160tevna funkcija zahteva en argument!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "Ne najdem funkcije: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Nepodprto kodiranje: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Pri\u0161lo je do te\u017eave v DTM pri getNextSibling... poskus obnovitve"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Programerska napaka: pisanje v EmptyNodeList ni mogo\u010de."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory v XPathContext ni podprt!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Predpona se mora razre\u0161iti v imenski prostor: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "Raz\u010dlenitev (vir InputSource) v XPathContext ni podprta! Ne morem odpreti {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "Znaki SAX API(znaka ch[]... ne obravnava DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(znaka ch[]... ne obravnava DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison ne more upravljati z vozli\u0161\u010di tipa {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper ne more upravljati z vozli\u0161\u010di tipa {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "Napaka DOM2Helper.parse: ID sistema - {0} vrstica - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "Napaka DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Zaznan neveljaven nadomestek UTF-16: {0} ?"},
				new object[] {ER_OIERROR, "Napaka V/I"},
				new object[] {ER_CANNOT_CREATE_URL, "Ne morem ustvariti naslova URL za: {0}"},
				new object[] {ER_XPATH_READOBJECT, "V XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "ne najdem \u017eetona funkcije."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Ne morem ravnati z tipom XPath: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Ta NodeSet ni spremenljiv"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Ta NodeSetDTM ni spremenljiv"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Spremenljivka ni razre\u0161ljiva: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Program za obravnavo napak NULL"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Programerjeva izjava: neznana opkoda: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 ali 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper ne podpira rtf()"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper ne podpira asNodeIterator()"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper ne podpira detach()"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper ne podpira num()"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper ne podpira xstr()"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper ne podpira str()"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() ni podprt za XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "Spremenljivke z imenom {0} ni mogo\u010de najti"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars ne more uporabiti niza za argument"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "Argument FastStringBuffer ne more biti NULL"},
				new object[] {ER_TWO_OR_THREE, "2 ali 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Spremenljivka uporabljena \u0161e pred njeno vezavo!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB ne more uporabiti niza za argument!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! Napaka! Koren sprehajalca nastavljam na NULL!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Tega NodeSetDTM ni mogo\u010de ponavljati do prej\u0161njega vozli\u0161\u010da!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Tega NodeSet ni mogo\u010de ponavljati do prej\u0161njega vozli\u0161\u010da!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Ta NodeSetDTM ne more opravljati funkcij priprave kazala ali \u0161tetja!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Ta NodeSet ne more opravljati funkcij priprave kazala ali \u0161tetja!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "Za klicem nextNode klic setShouldCacheNodes ni mogo\u010d!"},
				new object[] {ER_ONLY_ALLOWS, "{0} dovoljuje samo argumente {1}"},
				new object[] {ER_UNKNOWN_STEP, "Programerjeva izjava v getNextStepPos: neznan stepType: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "Za \u017eetonom '/' ali '//' je pri\u010dakovana relativna pot do mesta."},
				new object[] {ER_EXPECTED_LOC_PATH, "Pri\u010dakovana pot do lokacije, na\u0161jden pa je naslednji \u017eeton\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "Namesto pri\u010dakovane poti do lokacije je najden konec izraza XPath."},
				new object[] {ER_EXPECTED_LOC_STEP, "Za \u017eetonom '/' ali '//' je pri\u010dakovan korak mesta."},
				new object[] {ER_EXPECTED_NODE_TEST, "Pri\u010dakovan preskus vozli\u0161\u010da, ki ustreza NCImenu:* ali QImenu."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "Pri\u010dakovan stopnjevalni vzorec, najden pa je '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "Pri\u010dakovan vzorec z relativno potjo."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPathResult izraza XPath ''{0}'' ima XPathResultType {1}, ki ga ni mogoe\u010de pretvoriti v boolovo vrednost."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPathResult izraza XPath ''{0}'' ima XPathResultType {1}, ki ga ni mogo\u010de pretvoriti v eno vozli\u0161\u010de. Metoda getSingleNodeValue velja samo za tipa ANY_UNORDERED_NODE_TYPE in FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "Metoda getSnapshotLength ne more biti priklicana za XPathResult izraza XPath ''{0}'', ker je tip XPathResultType {1}. Ta metoda se nana\u0161a samo na tipa UNORDERED_NODE_SNAPSHOT_TYPE in ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "Metoda iterateNext ne more biti priklicana za XPathResult izraza XPath ''{0}'', ker je tip XPathResultType {1}. Ta metoda se nana\u0161a samo na tipa UNORDERED_NODE_ITERATOR_TYPE in ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "Dokument se je spremenil po vrnitvi rezultatov. Iterator je neveljaven."},
				new object[] {ER_INVALID_XPATH_TYPE, "Neveljaven argument tipa XPath: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Prazen objekt rezultatov XPath"},
				new object[] {ER_INCOMPATIBLE_TYPES, "Rezultat XPathResult izraza XPath ''{0}'' ima XPathResultType {1}, ki ga ni mogo\u010de prisiliti v dolo\u010den tip XPathResultType {2}."},
				new object[] {ER_NULL_RESOLVER, "Predpone ni bilo mogo\u010de razre\u0161iti z razre\u0161evalnikom predpon NULL."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "Rezultat XPathResult izraza XPath''{0}'' ima XPathResultType {1}, ki ga ni mogo\u010de pretvoriti v niz."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "Metoda snapshotItem ne more biti priklicana za XPathResult izraza XPath ''{0}'', ker je tip XPathResultType {1}. Ta metoda se nana\u0161a samo na tipa UNORDERED_NODE_SNAPSHOT_TYPE in ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "Kontekstno vozli\u0161\u010de ne pripada dokumentu, povezanem s tem XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "Tip kontekstnega vozli\u0161\u010da ni podprt."},
				new object[] {ER_XPATH_ERROR, "Neznana napaka v XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPathResult izraza XPath ''{0}'' ima XPathResultType {1}, ki ga ni mogo\u010de pretvoriti v \u0161tevilko."},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Raz\u0161iritvene funkcije: ''{0}'' ni mogo\u010de priklicati, kadar je zna\u010dilnost XMLConstants.FEATURE_SECURE_PROCESSING nastavljena na True."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "Funkcija resolveVariable za spremenljivko {0} vra\u010da rezultat ni\u010d"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Nepodprt tip vrnitve : {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Vir in/ali Tip vrnitve ne moreta biti ni\u010d"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "Argument {0} ne more biti ni\u010d"},
				new object[] {ER_OBJECT_MODEL_NULL, "Funkcije {0}#isObjectModelSupported( String objectModel ) ni mogo\u010de priklicati, kadar je objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "Funkcije {0}#isObjectModelSupported( String objectModel ) ni mogo\u010de priklicati, kadar je objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Poskus nastavitve funkcije brez imena (null name): {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Poskus nastavitve neznane funkcije \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Poskus pridobitve funkcije brez imena (null name): {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Poskus pridobitve neznane funkcije \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Poskus nastavitve XPathFunctionResolver na ni\u010d:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Poskus nastavitve funkcije XPathVariableResolver na ni\u010d:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "Podro\u010dno ime v funkciji za oblikovanje \u0161tevilk \u0161e ni podprto!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "Lastnost XSL ni podprta: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "V tem trenutku ne po\u010dnite ni\u010desar z imenskim prostorom {0} v lastnosti: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "Pri\u0161lo je do SecurityException (varnostna izjema) pri poskusu dostopa do sistemske lastnosti XSL: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Stara sintaksa: quo(...) v XPath ni ve\u010d definiran."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath potrebuje izpeljani objekt za implementacijo nodeTest!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "ne najdem \u017eetona funkcije."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "Ne najdem funkcije: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Ne morem narediti naslova URL iz: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "Mo\u017enost -E za raz\u010dlenjevalnik DTM ni podprta."},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "Dani VariableReference je izven konteksta ali brez definicije!  Ime = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Nepodprto kodiranje: {0}"},
				new object[] {"ui_language", "sl"},
				new object[] {"help_language", "sl"},
				new object[] {"language", "sl"},
				new object[] {"BAD_CODE", "Parameter za createMessage presega meje"},
				new object[] {"FORMAT_FAILED", "Med klicem je messageFormat naletel na izjemo"},
				new object[] {"version", ">>>>>>> Razli\u010dica Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "da"},
				new object[] {"line", "Vrstica #"},
				new object[] {"column", "Stolpec #"},
				new object[] {"xsldone", "XSLProcessor: dokon\u010dano"},
				new object[] {"xpath_option", "Mo\u017enosti xpath: "},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select izraz xpath]"},
				new object[] {"optionMatch", "   [-match primerjalni vzorec (za diagnostiko ujemanja)]"},
				new object[] {"optionAnyExpr", "Ali pa bo samo izraz xpath izvedel diagnosti\u010dni izvoz podatkov"},
				new object[] {"noParsermsg1", "Postopek XSL ni uspel."},
				new object[] {"noParsermsg2", "** Nisem na\u0161el raz\u010dlenjevalnika **"},
				new object[] {"noParsermsg3", "Preverite pot razreda."},
				new object[] {"noParsermsg4", "\u010ce nimate IBM raz\u010dlenjevalnika za Javo, ga lahko prenesete iz"},
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
	  public const string ERROR_STRING = "#error";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "Napaka: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "Opozorilo: ";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "VZOREC ";


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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("sl", "SL"));
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