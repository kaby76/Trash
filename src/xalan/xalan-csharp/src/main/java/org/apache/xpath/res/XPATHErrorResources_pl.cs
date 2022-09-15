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
 * $Id: XPATHErrorResources_pl.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_pl : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "Funkcja current() jest niedozwolona we wzorcu!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "Funkcja current() nie akceptuje argument\u00f3w!"},
				new object[] {ER_DOCUMENT_REPLACED, "Implementacja funkcji document() zosta\u0142a zast\u0105piona przez org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "Kontekst nie ma dokumentu w\u0142a\u015bciciela!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "Funkcja local-name() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "Funkcja namespace-uri() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "Funkcja normalize-space() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "Funkcja number() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "Funkcja name() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "Funkcja string() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "Funkcja string-length() ma zbyt wiele argument\u00f3w."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "Funkcja translate() akceptuje trzy argumenty!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "Funkcja unparsed-entity-uri() akceptuje tylko jeden argument!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "O\u015b przestrzeni nazw nie zosta\u0142a jeszcze zaimplementowana!"},
				new object[] {ER_UNKNOWN_AXIS, "nieznana o\u015b: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "Nieznana operacja uzgadniania!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "D\u0142ugo\u015b\u0107 argumentu testu w\u0119z\u0142a processing-instruction() jest niepoprawna!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "Nie mo\u017cna przekszta\u0142ci\u0107 {0} w liczb\u0119"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "Nie mo\u017cna przekszta\u0142ci\u0107 {0} w NodeList!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "Nie mo\u017cna przekszta\u0142ci\u0107 {0} w NodeSetDTM!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "Nie mo\u017cna przekszta\u0142ci\u0107 {0} w type#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "Oczekiwano wzorca uzgadniania w getMatchScore!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "Nie mo\u017cna pobra\u0107 zmiennej o nazwie {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "B\u0141\u0104D! Nieznany kod operacji: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Nadmiarowe niedozwolone leksemy: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Litera\u0142 bez cudzys\u0142owu... oczekiwano podw\u00f3jnego cudzys\u0142owu!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Litera\u0142 bez cudzys\u0142owu... oczekiwano pojedynczego cudzys\u0142owu!"},
				new object[] {ER_EMPTY_EXPRESSION, "Puste wyra\u017cenie!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "Oczekiwano {0}, ale znaleziono: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "Asercja programisty jest niepoprawna! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "argument boolean(...) nie jest ju\u017c opcjonalny wg projektu 19990709 XPath draft."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Znaleziono znak ',', ale nie ma poprzedzaj\u0105cego argumentu!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Znaleziono znak ',', ale nie ma nast\u0119puj\u0105cego argumentu!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predykat]' lub '.[predykat]' to niedozwolona sk\u0142adnia. U\u017cyj zamiast tego 'self::node()[predykat]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "Niedozwolona nazwa osi: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Nieznany typ w\u0119z\u0142a: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "Litera\u0142 wzorca ({0}) musi by\u0107 w cudzys\u0142owie!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "Nie mo\u017cna sformatowa\u0107 {0} do postaci liczbowej!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "Nie mo\u017cna utworzy\u0107 po\u0142\u0105czenia XML TransformerFactory: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "B\u0142\u0105d! Nie znaleziono wyra\u017cenia wyboru xpath (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "B\u0141\u0104D! Nie mo\u017cna znale\u017a\u0107 ENDOP po OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Wyst\u0105pi\u0142 b\u0142\u0105d!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference nadana zmiennej nie nale\u017cy do kontekstu lub nie ma definicji!  Nazwa = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "We wzorcach zgodno\u015bci dozwolone s\u0105 tylko osie child:: oraz attribute::!  Niew\u0142a\u015bciwe osie = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "Funkcja key() ma niepoprawn\u0105 liczb\u0119 argument\u00f3w."},
				new object[] {ER_COUNT_TAKES_1_ARG, "Funkcja count() akceptuje tylko jeden argument!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "Nie mo\u017cna znale\u017a\u0107 funkcji: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Nieobs\u0142ugiwane kodowanie: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Wyst\u0105pi\u0142 problem w DTM w getNextSibling... pr\u00f3ba wyj\u015bcia z b\u0142\u0119du"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "B\u0142\u0105d programisty: Nie mo\u017cna zapisywa\u0107 do EmptyNodeList."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory nie jest obs\u0142ugiwane przez XPathContext!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Przedrostek musi da\u0107 si\u0119 przet\u0142umaczy\u0107 na przestrze\u0144 nazw: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (InputSource \u017ar\u00f3d\u0142o) nie jest obs\u0142ugiwane w XPathContext! Nie mo\u017cna otworzy\u0107 {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API characters(char ch[]... nie jest obs\u0142ugiwane przez DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... nie jest obs\u0142ugiwane przez DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison nie mo\u017ce obs\u0142u\u017cy\u0107 w\u0119z\u0142\u00f3w typu {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper nie mo\u017ce obs\u0142u\u017cy\u0107 w\u0119z\u0142\u00f3w typu {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "B\u0142\u0105d DOM2Helper.parse : ID systemu - {0} wiersz - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "B\u0142\u0105d DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Wykryto niepoprawny odpowiednik UTF-16: {0} ?"},
				new object[] {ER_OIERROR, "B\u0142\u0105d we/wy"},
				new object[] {ER_CANNOT_CREATE_URL, "Nie mo\u017cna utworzy\u0107 adresu url dla {0}"},
				new object[] {ER_XPATH_READOBJECT, "W XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "Nie znaleziono leksemu funkcji."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Nie mo\u017cna upora\u0107 si\u0119 z typem XPath {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Ten NodeSet nie jest zmienny"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Ten NodeSetDTM nie jest zmienny"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Nie mo\u017cna rozstrzygn\u0105\u0107 zmiennej {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Pusta procedura obs\u0142ugi b\u0142\u0119du"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Asercja programisty: nieznany kod opcode: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 lub 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "Funkcja rtf() nie jest obs\u0142ugiwana przez XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "Funkcja asNodeIterator() nie jest obs\u0142ugiwana przez XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "Funkcja detach() nie jest obs\u0142ugiwana przez XRTreeFragSelectWrapper"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "Funkcja num() nie jest obs\u0142ugiwana przez XRTreeFragSelectWrapper"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "Funkcja xstr() nie jest obs\u0142ugiwana przez XRTreeFragSelectWrapper"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "Funkcja str() nie jest obs\u0142ugiwana przez XRTreeFragSelectWrapper"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "Funkcja fsb() nie jest obs\u0142ugiwana dla XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "Nie mo\u017cna znale\u017a\u0107 zmiennej o nazwie {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars nie mo\u017ce pobra\u0107 ci\u0105gu znak\u00f3w jako argumentu"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "Argument FastStringBuffer nie mo\u017ce by\u0107 pusty"},
				new object[] {ER_TWO_OR_THREE, "2 lub 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Nast\u0105pi\u0142o odwo\u0142anie do zmiennej, zanim zosta\u0142a ona zwi\u0105zana!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB nie mo\u017ce pobra\u0107 ci\u0105gu znak\u00f3w jako argumentu!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! B\u0142\u0105d! Ustawienie root w\u0119drownika na null!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Ten NodeSetDTM nie mo\u017ce iterowa\u0107 do poprzedniego w\u0119z\u0142a!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Ten NodeSet nie mo\u017ce iterowa\u0107 do poprzedniego w\u0119z\u0142a!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Ten NodeSetDTM nie mo\u017ce wykona\u0107 funkcji indeksowania lub zliczania!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Ten NodeSet nie mo\u017ce wykona\u0107 funkcji indeksowania lub zliczania!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "Nie mo\u017cna wywo\u0142a\u0107 setShouldCacheNodes po wywo\u0142aniu nextNode!"},
				new object[] {ER_ONLY_ALLOWS, "{0} zezwala tylko na {1} argument\u00f3w"},
				new object[] {ER_UNKNOWN_STEP, "Asercja programisty w getNextStepPos: nieznany stepType: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "Po leksemie '/' oraz '//' oczekiwana by\u0142a \u015bcie\u017cka wzgl\u0119dna po\u0142o\u017cenia."},
				new object[] {ER_EXPECTED_LOC_PATH, "Oczekiwano \u015bcie\u017cki po\u0142o\u017cenia, ale napotkano nast\u0119puj\u0105cy leksem\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "Oczekiwano \u015bcie\u017cki po\u0142o\u017cenia, ale zamiast niej znaleziono koniec wyra\u017cenia XPath."},
				new object[] {ER_EXPECTED_LOC_STEP, "Po leksemie '/' oraz '//' oczekiwany by\u0142 krok po\u0142o\u017cenia."},
				new object[] {ER_EXPECTED_NODE_TEST, "Oczekiwano testu w\u0119z\u0142a zgodnego albo z NCName:*, albo z QName."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "Oczekiwano wzorca kroku, ale napotkano '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "Oczekiwano wzorca \u015bcie\u017cki wzgl\u0119dnej."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "Rezultat (XPathResult) wyra\u017cenia XPath ''{0}'' ma typ (XPathResultType) {1}, kt\u00f3rego nie mo\u017cna przekszta\u0142ci\u0107 w typ boolowski."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "Rezultat (XPathResult) wyra\u017cenia XPath ''{0}'' ma typ (XPathResultType) {1}, kt\u00f3rego nie mo\u017cna przekszta\u0142ci\u0107 w pojedynczy w\u0119ze\u0142. Metod\u0119 getSingleNodeValue mo\u017cna stosowa\u0107 tylko do typ\u00f3w ANY_UNORDERED_NODE_TYPE oraz FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "Metody getSnapshotLength nie mo\u017cna wywo\u0142a\u0107 na rezultacie (XPathResult) wyra\u017cenia XPath ''{0}'', poniewa\u017c jego typem (XPathResultType) jest {1}. Metod\u0119 t\u0119 mo\u017cna stosowa\u0107 tylko do typ\u00f3w UNORDERED_NODE_SNAPSHOT_TYPE oraz ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "Metody iterateNext nie mo\u017cna wywo\u0142a\u0107 na rezultacie (XPathResult) wyra\u017cenia XPath ''{0}'', poniewa\u017c jego typem (XPathResultType) jest {1}. Metod\u0119 t\u0119 mo\u017cna stosowa\u0107 tylko do typ\u00f3w UNORDERED_NODE_ITERATOR_TYPE oraz ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "Dokument uleg\u0142 zmianie od czasu zwr\u00f3cenia rezultatu. Iterator jest niepoprawny."},
				new object[] {ER_INVALID_XPATH_TYPE, "Niepoprawny argument typu XPath: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Pusty obiekt rezultatu XPath"},
				new object[] {ER_INCOMPATIBLE_TYPES, "Rezultat (XPathResult) wyra\u017cenia XPath ''{0}'' ma typ (XPathResultType) {1}, na kt\u00f3rym nie mo\u017cna wymusi\u0107 dzia\u0142ania jak na okre\u015blonym typie (XPathResultType) {2}."},
				new object[] {ER_NULL_RESOLVER, "Nie mo\u017cna przet\u0142umaczy\u0107 przedrostka za pomoc\u0105 procedury t\u0142umacz\u0105cej o pustym przedrostku."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "Rezultat (XPathResult) wyra\u017cenia XPath ''{0}'' ma typ (XPathResultType) {1}, kt\u00f3rego nie mo\u017cna przekszta\u0142ci\u0107 w typ \u0142a\u0144cuchowy."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "Metody snapshotItem nie mo\u017cna wywo\u0142a\u0107 na rezultacie (XPathResult) wyra\u017cenia XPath ''{0}'', poniewa\u017c jego typem (XPathResultType) jest {1}. Metod\u0119 t\u0119 mo\u017cna stosowa\u0107 tylko do typ\u00f3w UNORDERED_NODE_SNAPSHOT_TYPE oraz ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "W\u0119ze\u0142 kontekstu nie nale\u017cy do dokumentu, kt\u00f3ry jest zwi\u0105zany z tym interfejsem XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "Nieobs\u0142ugiwany typ w\u0119z\u0142a kontekstu."},
				new object[] {ER_XPATH_ERROR, "Nieznany b\u0142\u0105d w XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "Rezultat (XPathResult) wyra\u017cenia XPath ''{0}'' ma typ (XPathResultType) {1}, kt\u00f3rego nie mo\u017cna przekszta\u0142ci\u0107 w typ liczbowy."},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Nie mo\u017cna wywo\u0142a\u0107 funkcji rozszerzenia ''{0}'', kiedy opcja XMLConstants.FEATURE_SECURE_PROCESSING ma warto\u015b\u0107 true."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable zwraca warto\u015b\u0107 null dla zmiennej {0}"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Nieobs\u0142ugiwany typ zwracany : {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Typ \u017ar\u00f3d\u0142owy i/lub zwracany nie mo\u017ce mie\u0107 warto\u015bci null"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "Argument {0} nie mo\u017ce mie\u0107 warto\u015bci null"},
				new object[] {ER_OBJECT_MODEL_NULL, "Nie mo\u017cna wywo\u0142a\u0107 {0}#isObjectModelSupported( String objectModel ) ze zmienn\u0105 objectModel o warto\u015bci null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "Nie mo\u017cna wywo\u0142a\u0107 {0}#isObjectModelSupported( String objectModel ) ze zmienn\u0105 objectModel o warto\u015bci \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Pr\u00f3ba ustawienia opcji o nazwie r\u00f3wnej null: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Pr\u00f3ba ustawienia nieznanej opcji \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Pr\u00f3ba pobrania opcji o nazwie r\u00f3wnej null: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Pr\u00f3ba pobrania nieznanej opcji \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Pr\u00f3ba ustawienia XPathFunctionResolver o warto\u015bci null:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Pr\u00f3ba ustawienia XPathVariableResolver o warto\u015bci null:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "Nazwa ustawie\u0144 narodowych w funkcji format-number nie jest jeszcze obs\u0142ugiwana!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "Nieobs\u0142ugiwana w\u0142a\u015bciwo\u015b\u0107 XSL {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "Nie r\u00f3b teraz niczego z przestrzeni\u0105 nazw {0} we w\u0142a\u015bciwo\u015bci {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "Wyj\u0105tek SecurityException podczas pr\u00f3by dost\u0119pu do w\u0142a\u015bciwo\u015bci systemowej XSL {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Stara sk\u0142adnia: quo(...) nie jest ju\u017c zdefiniowana w XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath potrzebuje obiektu pochodnego, aby zaimplementowa\u0107 nodeTest!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "Nie znaleziono leksemu funkcji."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "Nie mo\u017cna znale\u017a\u0107 funkcji: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Nie mo\u017cna utworzy\u0107 adresu URL z {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "Opcja -E nie jest obs\u0142ugiwana przez analizator DTM"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference nadana zmiennej nie nale\u017cy do kontekstu lub nie ma definicji!  Nazwa = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Nieobs\u0142ugiwane kodowanie: {0}"},
				new object[] {"ui_language", "pl"},
				new object[] {"help_language", "pl"},
				new object[] {"language", "pl"},
				new object[] {"BAD_CODE", "Parametr createMessage by\u0142 spoza zakresu"},
				new object[] {"FORMAT_FAILED", "Podczas wywo\u0142ania messageFormat zg\u0142oszony zosta\u0142 wyj\u0105tek"},
				new object[] {"version", ">>>>>>> Wersja Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "tak"},
				new object[] {"line", "Nr wiersza: "},
				new object[] {"column", "Nr kolumny: "},
				new object[] {"xsldone", "XSLProcessor: gotowe"},
				new object[] {"xpath_option", "opcje xpath: "},
				new object[] {"optionIN", "[-in wej\u015bciowyXMLURL]"},
				new object[] {"optionSelect", "[-select wyra\u017cenie xpath]"},
				new object[] {"optionMatch", "[-match wzorzec (do diagnostyki odnajdywania zgodno\u015bci ze wzorcem)]"},
				new object[] {"optionAnyExpr", "Lub po prostu wyra\u017cenie xpath dokona zrzutu diagnostycznego"},
				new object[] {"noParsermsg1", "Proces XSL nie wykona\u0142 si\u0119 pomy\u015blnie."},
				new object[] {"noParsermsg2", "** Nie mo\u017cna znale\u017a\u0107 analizatora **"},
				new object[] {"noParsermsg3", "Sprawd\u017a classpath."},
				new object[] {"noParsermsg4", "Je\u015bli nie masz analizatora XML dla j\u0119zyka Java firmy IBM, mo\u017cesz go pobra\u0107 "},
				new object[] {"noParsermsg5", "z serwisu AlphaWorks firmy IBM: http://www.alphaworks.ibm.com/formula/xml"},
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
	  public const string ERROR_STRING = "nr b\u0142\u0119du";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "B\u0142\u0105d: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "Ostrze\u017cenie: ";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "WZORZEC ";


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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("pl", "PL"));
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