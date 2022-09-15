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
 * $Id: XPATHErrorResources_de.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_de : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "Die Funktion current() ist in einem \u00dcbereinstimmungsmuster nicht zul\u00e4ssig!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "In der Funktion current() d\u00fcrfen keine Argumente angegeben werden!"},
				new object[] {ER_DOCUMENT_REPLACED, "Die Implementierung der Funktion document() wurde durch org.apache.xalan.xslt.FuncDocument ersetzt!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "Der Kontextknoten verf\u00fcgt nicht \u00fcber ein Eignerdokument!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() weist zu viele Argumente auf."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() weist zu viele Argumente auf."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() weist zu viele Argumente auf."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() weist zu viele Argumente auf."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() weist zu viele Argumente auf."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() weist zu viele Argumente auf."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() weist zu viele Argumente auf."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "Die Funktion translate() erfordert drei Argumente!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "Die Funktion unparsed-entity-uri sollte ein einziges Argument enthalten!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "Die Namensbereichsachse ist bisher nicht implementiert!"},
				new object[] {ER_UNKNOWN_AXIS, "Unbekannte Achse: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "Unbekannter \u00dcbereinstimmungsvorgang!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "Die L\u00e4nge des Arguments f\u00fcr den Knotentest von processing-instruction() ist falsch!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "{0} kann nicht in eine Zahl konvertiert werden!"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "{0} kann nicht in NodeList konvertiert werden!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "{0} kann nicht in NodeSetDTM konvertiert werden!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "{0} kann nicht in type#{1} konvertiert werden."},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "\u00dcbereinstimmungsmuster in getMatchScore erwartet!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "Die Variable mit dem Namen {0} konnte nicht abgerufen werden."},
				new object[] {ER_UNKNOWN_OPCODE, "FEHLER! Unbekannter Operationscode: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Zus\u00e4tzliche nicht zul\u00e4ssige Token: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Falsche Anf\u00fchrungszeichen f\u00fcr Literal... Doppelte Anf\u00fchrungszeichen wurden erwartet!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Falsche Anf\u00fchrungszeichen f\u00fcr Literal... Einfache Anf\u00fchrungszeichen wurden erwartet!"},
				new object[] {ER_EMPTY_EXPRESSION, "Leerer Ausdruck!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "Erwartet wurde {0}, gefunden wurde: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "Festlegung des Programmierers ist falsch! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "Das Argument boolean(...) ist im XPath-Entwurf 19990709 nicht mehr optional."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Gefunden wurde ',' ohne vorangestelltes Argument!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Gefunden wurde ',' ohne nachfolgendes Argument!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' oder '.[predicate]' ist eine nicht zul\u00e4ssige Syntax. Verwenden Sie stattdessen 'self::node()[predicate]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "Nicht zul\u00e4ssiger Achsenname: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Unbekannter Knotentyp: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "Musterliteral ({0}) muss in Anf\u00fchrungszeichen angegeben werden!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} konnte nicht als Zahl formatiert werden!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "XML-TransformerFactory-Liaison konnte nicht erstellt werden: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Fehler! xpath-Auswahlausdruck (-select) konnte nicht gefunden werden."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "FEHLER! ENDOP konnte nach OP_LOCATIONPATH nicht gefunden werden."},
				new object[] {ER_ERROR_OCCURED, "Es ist ein Fehler aufgetreten!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "Das f\u00fcr die Variable angegebene Argument VariableReference befindet sich au\u00dferhalb des Kontexts oder weist keine Definition auf!  Name = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "Nur die Achsen ''''child::'''' und ''''attribute::'''' sind in Suchmustern zul\u00e4ssig!  Fehlerhafte Achsen = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() weist eine falsche Anzahl Argumenten auf."},
				new object[] {ER_COUNT_TAKES_1_ARG, "Die Funktion count sollte ein einziges Argument enthalten!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "Die Funktion konnte nicht gefunden werden: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Nicht unterst\u00fctzte Verschl\u00fcsselung: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "In dem DTM in getNextSibling ist ein Fehler aufgetreten... Wiederherstellung wird durchgef\u00fchrt"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Programmierungsfehler: In EmptyNodeList kann nicht geschrieben werden."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory wird nicht von XPathContext unterst\u00fctzt!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Das Pr\u00e4fix muss in einen Namensbereich aufgel\u00f6st werden: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (InputSource Quelle) wird nicht in XPathContext unterst\u00fctzt! {0} kann nicht ge\u00f6ffnet werden."},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX-API characters(char ch[]... wird nicht von dem DTM verarbeitet!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... wird nicht von dem DTM verarbeitet!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison kann keine Knoten vom Typ {0} verarbeiten"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper kann keine Knoten vom Typ {0} verarbeiten"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "Fehler bei DOM2Helper.parse: System-ID - {0} Zeile - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "Fehler bei DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Ung\u00fcltige UTF-16-Ersetzung festgestellt: {0} ?"},
				new object[] {ER_OIERROR, "E/A-Fehler"},
				new object[] {ER_CANNOT_CREATE_URL, "URL kann nicht erstellt werden f\u00fcr: {0}"},
				new object[] {ER_XPATH_READOBJECT, "In XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "Funktionstoken wurde nicht gefunden."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Der XPath-Typ kann nicht verarbeitet werden: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Diese NodeSet kann nicht ge\u00e4ndert werden"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Dieses NodeSetDTM kann nicht ge\u00e4ndert werden"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Die Variable kann nicht aufgel\u00f6st werden: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Kein Fehlerbehandlungsprogramm vorhanden"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Programmiererfestlegung: Unbekannter Operationscode: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 oder 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() wird nicht von XRTreeFragSelectWrapper unterst\u00fctzt"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() wird nicht von XRTreeFragSelectWrapper unterst\u00fctzt"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() wird nicht von XRTreeFragSelectWrapper unterst\u00fctzt"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() wird nicht von XRTreeFragSelectWrapper unterst\u00fctzt"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() wird nicht von XRTreeFragSelectWrapper unterst\u00fctzt"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() wird nicht von XRTreeFragSelectWrapper unterst\u00fctzt"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() wird nicht f\u00fcr XStringForChars unterst\u00fctzt"},
				new object[] {ER_COULD_NOT_FIND_VAR, "Die Variable mit dem Namen {0} konnte nicht gefunden werden"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars kann keine Zeichenfolge als Argument enthalten"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "Das Argument FastStringBuffer kann nicht null sein"},
				new object[] {ER_TWO_OR_THREE, "2 oder 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Auf die Variable wurde zugegriffen, bevor diese gebunden wurde!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB kann keine Zeichenfolge als Argument enthalten!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! Fehler! Root eines Walker wird auf null gesetzt!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Dieses NodeSetDTM kann keinen vorherigen Knoten wiederholen!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Diese NodeSet kann keinen vorherigen Knoten wiederholen!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Dieses NodeSetDTM kann keine Indexierungs- oder Z\u00e4hlfunktionen ausf\u00fchren!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Diese NodeSet kann keine Indexierungs- oder Z\u00e4hlfunktionen ausf\u00fchren!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "setShouldCacheNodes kann nicht aufgerufen werden, nachdem nextNode aufgerufen wurde!"},
				new object[] {ER_ONLY_ALLOWS, "{0} erlaubt nur {1} Argument(e)"},
				new object[] {ER_UNKNOWN_STEP, "Programmiererfestlegung in getNextStepPos: stepType unbekannt: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "Nach dem Token '/' oder '//' wurde ein relativer Positionspfad erwartet."},
				new object[] {ER_EXPECTED_LOC_PATH, "Es wurde ein Positionspfad erwartet, aber folgendes Token wurde festgestellt\u003a {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "Es wurde ein Positionspfad erwartet, aber das Ende des XPath-Ausdrucks wurde stattdessen gefunden."},
				new object[] {ER_EXPECTED_LOC_STEP, "Nach dem Token '/' oder '//' wurde ein Positionsschritt erwartet."},
				new object[] {ER_EXPECTED_NODE_TEST, "Es wurde ein Knotentest erwartet, der entweder NCName:* oder dem QNamen entspricht."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "Es wurde ein Schrittmuster erwartet, aber '/' festgestellt."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "Es wurde ein Muster eines relativen Pfads erwartet."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPathResult des XPath-Ausdrucks ''{0}'' hat einen XPathResultType von {1}, der nicht in einen Booleschen Ausdruck konvertiert werden kann."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPathResult des XPath-Ausdrucks ''{0}'' hat einen XPathResultType von {1}, der nicht in einen einzelnen Knoten konvertiert werden kann. Die Methode getSingleNodeValue bezieht sich nur auf die Typen ANY_UNORDERED_NODE_TYPE und FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "Die Methode getSnapshotLength kann nicht \u00fcber XPathResult des XPath-Ausdrucks ''{0}'' aufgerufen werden, da der zugeh\u00f6rige XPathResultType {1} ist. Diese Methode gilt nur f\u00fcr die Typen UNORDERED_NODE_SNAPSHOT_TYPE und ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "Die Methode iterateNext kann nicht \u00fcber XPathResult des XPath-Ausdrucks ''{0}'' aufgerufen werden, da der zugeh\u00f6rige XPathResultType {1} ist. Diese Methode gilt nur f\u00fcr die Typen UNORDERED_NODE_ITERATOR_TYPE und ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "Seit der R\u00fcckgabe des Ergebnisses wurde das Dokument ge\u00e4ndert. Der Iterator ist ung\u00fcltig."},
				new object[] {ER_INVALID_XPATH_TYPE, "Ung\u00fcltiges XPath-Typenargument: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Leeres XPath-Ergebnisobjekt"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPathResult des XPath-Ausdrucks ''{0}'' hat einen XPathResultType von {1}, der nicht in den angegebenen XPathResultType {2} konvertiert werden kann."},
				new object[] {ER_NULL_RESOLVER, "Das Pr\u00e4fix kann nicht mit einer Aufl\u00f6sungsfunktion f\u00fcr Nullpr\u00e4fixe aufgel\u00f6st werden."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPathResult des XPath-Ausdrucks ''{0}'' hat einen XPathResultType von {1}, der nicht in eine Zeichenfolge konvertiert werden kann."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "Die Methode snapshotItem kann nicht \u00fcber XPathResult des XPath-Ausdrucks ''{0}'' aufgerufen werden, da der zugeh\u00f6rige XPathResultType {1} ist. Diese Methode gilt nur f\u00fcr die Typen UNORDERED_NODE_SNAPSHOT_TYPE und ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "Kontextknoten geh\u00f6rt nicht zu dem Dokument, das an diesen XPathEvaluator gebunden ist."},
				new object[] {ER_WRONG_NODETYPE, "Der Kontextknotentyp wird nicht unterst\u00fctzt."},
				new object[] {ER_XPATH_ERROR, "Unbekannter Fehler in XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPathResult des XPath-Ausdrucks ''{0}'' hat einen XPathResultType von {1}, der nicht in eine Zahl konvertiert werden kann."},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Erweiterungsfunktion: ''{0}'' kann nicht aufgerufen werden, wenn f\u00fcr XMLConstants.FEATURE_SECURE_PROCESSING die Einstellung ''True'' festgelegt wurde."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable f\u00fcr die Variable {0} gibt den Wert Null zur\u00fcck"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Nicht unterst\u00fctzter R\u00fcckgabetyp: {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Quellen- und/oder R\u00fcckgabetyp d\u00fcrfen nicht null sein"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "Das Argument {0} darf nicht den Wert Null haben"},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( Zeichenfolge objectModel ) kann nicht aufgerufen werden, wenn objectModel den Wert Null hat"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( Zeichenfolge objectModel ) kann nicht aufgerufen werden, wenn objectModel den Wert \"\" hat"},
				new object[] {ER_FEATURE_NAME_NULL, "Es wird versucht, eine Funktion ohne Namensangabe zu definieren: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Es wird versucht, die unbekannte Funktion \"{0}\" zu definieren: {1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Es wird versucht, eine Funktion ohne Namensangabe abzurufen: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Es wird versucht, die unbekannte Funktion \"{0}\" abzurufen: {1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Es wird versucht, XPathFunctionResolver mit dem Wert Null zu definieren: {0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Es wird versucht, XPathVariableResolver mit dem Wert Null zu definieren: {0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "Der Name der L\u00e4ndereinstellung in der Funktion format-number wurde bisher nicht verarbeitet!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "XSL-Merkmal wird nicht unterst\u00fctzt: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "F\u00fchren Sie derzeit keine Vorg\u00e4nge mit dem Namensbereich {0} in folgendem Merkmal durch: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException beim Zugriff auf XSL-Systemmerkmal: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Veraltete Syntax: quo(...) ist nicht mehr in XPath definiert."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath ben\u00f6tigt f\u00fcr die Implementierung von nodeTest ein abgeleitetes Objekt!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "Funktionstoken wurde nicht gefunden."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "Die Funktion konnte nicht gefunden werden: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "URL konnte nicht erstellt werden aus: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "Option -E wird f\u00fcr DTM-Parser nicht unterst\u00fctzt"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "Das f\u00fcr die Variable angegebene Argument VariableReference befindet sich au\u00dferhalb des Kontexts oder weist keine Definition auf!  Name = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Nicht unterst\u00fctzte Verschl\u00fcsselung: {0}"},
				new object[] {"ui_language", "de"},
				new object[] {"help_language", "de"},
				new object[] {"language", "de"},
				new object[] {"BAD_CODE", "Der Parameter f\u00fcr createMessage lag au\u00dferhalb des g\u00fcltigen Bereichs"},
				new object[] {"FORMAT_FAILED", "W\u00e4hrend des Aufrufs von messageFormat wurde eine Ausnahmebedingung ausgel\u00f6st"},
				new object[] {"version", ">>>>>>> Xalan-Version "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "ja"},
				new object[] {"line", "Zeilennummer"},
				new object[] {"column", "Spaltennummer"},
				new object[] {"xsldone", "XSLProcessor: fertig"},
				new object[] {"xpath_option", "xpath-Optionen: "},
				new object[] {"optionIN", "[-in EingabeXMLURL]"},
				new object[] {"optionSelect", "[-select Xpath-Ausdruck]"},
				new object[] {"optionMatch", "[-match \u00dcbereinstimmungsmuster (f\u00fcr \u00dcbereinstimmungsdiagnose)]"},
				new object[] {"optionAnyExpr", "\u00dcber einen einfachen xpath-Ausdruck wird ein Diagnosespeicherauszug erstellt"},
				new object[] {"noParsermsg1", "XSL-Prozess konnte nicht erfolgreich durchgef\u00fchrt werden."},
				new object[] {"noParsermsg2", "** Parser konnte nicht gefunden werden **"},
				new object[] {"noParsermsg3", "Bitte \u00fcberpr\u00fcfen Sie den Klassenpfad."},
				new object[] {"noParsermsg4", "Wenn Sie nicht \u00fcber einen IBM XML-Parser f\u00fcr Java verf\u00fcgen, k\u00f6nnen Sie ihn \u00fcber"},
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
	  public const string BAD_CODE = "FEHLERHAFTER_CODE";

	  /// <summary>
	  /// Field FORMAT_FAILED </summary>
	  public const string FORMAT_FAILED = "FORMAT_FEHLGESCHLAGEN";

	  /// <summary>
	  /// Field ERROR_RESOURCES </summary>
	  public const string ERROR_RESOURCES = "org.apache.xpath.res.XPATHErrorResources";

	  /// <summary>
	  /// Field ERROR_STRING </summary>
	  public const string ERROR_STRING = "#Fehler";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public const string ERROR_HEADER = "Fehler: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "Achtung: ";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "MUSTER ";


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