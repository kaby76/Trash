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
 * $Id: XPATHErrorResources_sk.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_sk : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "Funkcia current () nie je povolen\u00e1 v porovn\u00e1vacom vzore!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "Funkcia current () nepr\u00edma argumenty!"},
				new object[] {ER_DOCUMENT_REPLACED, "Implement\u00e1cia funkcie document() bola nahraden\u00e1 org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "kontext nem\u00e1 dokument vlastn\u00edka!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() m\u00e1 prive\u013ea argumentov."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() m\u00e1 prive\u013ea argumentov."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() m\u00e1 prive\u013ea argumentov."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() m\u00e1 prive\u013ea argumentov."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() m\u00e1 prive\u013ea argumentov."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() m\u00e1 prive\u013ea argumentov"},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() m\u00e1 prive\u013ea argumentov"},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "Funkcia translate() pr\u00edma tri argumenty!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "Funkcia unparsed-entity-uri by mala prija\u0165 jeden argument!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "osi n\u00e1zvov\u00fdch priestorov e\u0161te nie s\u00fa implementovan\u00e9!"},
				new object[] {ER_UNKNOWN_AXIS, "nezn\u00e1ma os: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "nezn\u00e1ma porovn\u00e1vacia oper\u00e1cia!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "Testovanie uzla arg length of processing-instruction() je nespr\u00e1vne!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "Nie je mo\u017en\u00e9 konvertova\u0165 {0} na \u010d\u00edslo"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "Nie je mo\u017en\u00e9 konvertova\u0165 {0} na NodeList!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "Nie je mo\u017en\u00e9 konvertova\u0165 {0} na NodeSetDTM!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "Nie je mo\u017en\u00e1 konverzia {0} na typ#{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "O\u010dak\u00e1van\u00fd porovn\u00e1vac\u00ed vzor v getMatchScore!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "Nie je mo\u017en\u00e9 dosiahnu\u0165 premenn\u00fa s n\u00e1zvom {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "CHYBA! Nezn\u00e1my k\u00f3d op: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Nadbyto\u010dn\u00e9 neplatn\u00e9 symboly: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Nespr\u00e1vny liter\u00e1l... o\u010dak\u00e1van\u00e1 dvojit\u00e1 cit\u00e1cia!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Nespr\u00e1vny liter\u00e1l... o\u010dak\u00e1van\u00e1 jedin\u00e1 cit\u00e1cia!"},
				new object[] {ER_EMPTY_EXPRESSION, "Pr\u00e1zdny v\u00fdraz!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "O\u010dak\u00e1vala sa {0}, ale bola n\u00e1jden\u00e1: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "Program\u00e1torsk\u00e9 vyjadrenie je nespr\u00e1vne! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "argument boolean(...) u\u017e nie je volite\u013en\u00fd s konceptom 19990709 XPath."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "N\u00e1jdene ',' ale \u017eiaden predch\u00e1dzaj\u00faci argument!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "N\u00e1jden\u00e9 ',' ale \u017eiaden nasleduj\u00faci argument!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' alebo '.[predicate]' je nespr\u00e1vna syntax.  Pou\u017eite namiesto toho 'self::node()[predicate]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "Neplatn\u00fd n\u00e1zov osi: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Nezn\u00e1my typ uzla: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "Vzorov\u00fd liter\u00e1l ({0}) potrebuje by\u0165 citovan\u00fd!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} nem\u00f4\u017ee by\u0165 form\u00e1tovan\u00e9 na \u010d\u00edslo!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "Nebolo mo\u017en\u00e9 vytvori\u0165 vz\u0165ah XML TransformerFactory: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Chyba! Nena\u0161lo sa vyjadrenie v\u00fdberu xpath (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "CHYBA! Nebolo mo\u017en\u00e9 n\u00e1js\u0165 ENDOP po OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Vyskytla sa chyba!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference bol dan\u00fd pre premenn\u00fa mimo kontext, alebo bez defin\u00edcie!  N\u00e1zov = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "Len potomok:: atrib\u00fat:: osi s\u00fa povolen\u00e9 v zhodn\u00fdch vzoroch!  Chybn\u00e9 osi = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() m\u00e1 nespr\u00e1vny po\u010det argumentov."},
				new object[] {ER_COUNT_TAKES_1_ARG, "Funkcia count by mala prija\u0165 jeden argument!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "Nebolo mo\u017en\u00e9 n\u00e1js\u0165 funkciu: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Nepodporovan\u00e9 k\u00f3dovanie: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Vyskytol sa probl\u00e9m v DTM v getNextSibling... pokus o obnovu"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Chyba program\u00e1tora: EmptyNodeList nebolo mo\u017en\u00e9 zap\u00edsa\u0165."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory nie je podporovan\u00e9 XPathContext!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Predpona sa mus\u00ed rozl\u00ed\u0161i\u0165 do n\u00e1zvov\u00e9ho priestoru: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "anal\u00fdza (InputSource source) nie je podporovan\u00e1 XPathContext! Nie je mo\u017en\u00e9 otvori\u0165 {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API znaky(char ch[]... nie s\u00fa spracovan\u00e9 DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... nie s\u00fa spracovan\u00e9 DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison nem\u00f4\u017ee spracova\u0165 uzly typu {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper nem\u00f4\u017ee spracova\u0165 uzly typu {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "Chyba DOM2Helper.parse: SystemID - {0} riadok - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "chyba DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Bolo zisten\u00e9 neplatn\u00e9 nahradenie UTF-16: {0} ?"},
				new object[] {ER_OIERROR, "chyba IO"},
				new object[] {ER_CANNOT_CREATE_URL, "Nie je mo\u017en\u00e9 vytvori\u0165 url pre: {0}"},
				new object[] {ER_XPATH_READOBJECT, "V XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "nebol n\u00e1jden\u00fd symbol funkcie."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Nie je mo\u017en\u00e9 pracova\u0165 s typom XPath: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Tento NodeSet je nest\u00e1ly"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Tento NodeSetDTM nie je nest\u00e1ly"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Premenn\u00fa nie je mo\u017en\u00e9 rozl\u00ed\u0161i\u0165: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Nulov\u00fd chybov\u00fd manipula\u010dn\u00fd program"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Tvrdenie program\u00e1tora: nezn\u00e1my opcode: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0, alebo 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() nie je podporovan\u00fd XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() nie je podporovan\u00fd XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper nepodporuje detach()"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper nepodporuje num()"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper nepodporuje xstr()"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "XRTreeFragSelectWrapper nepodporuje str()"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() nie je podporovan\u00fd pre XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "Nebolo mo\u017en\u00e9 n\u00e1js\u0165 premenn\u00fa s n\u00e1zvom {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars nem\u00f4\u017ee ako argument prija\u0165 re\u0165azec"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "Argument FastStringBuffer nem\u00f4\u017ee by\u0165 nulov\u00fd"},
				new object[] {ER_TWO_OR_THREE, "2, alebo 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Premenn\u00e1 bola z\u00edskan\u00e1 sk\u00f4r, ne\u017e bola viazan\u00e1!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB nem\u00f4\u017ee pova\u017eova\u0165 re\u0165azec za argument!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! Chyba! Nastavenie root of a walker na null!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Tento NodeSetDTM sa nem\u00f4\u017ee iterova\u0165 na predch\u00e1dzaj\u00faci uzol!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Tento NodeSet sa nem\u00f4\u017ee iterova\u0165 na predch\u00e1dzaj\u00faci uzol!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Tento NodeSetDTM nem\u00f4\u017ee vykon\u00e1va\u0165 funkcie indexovania alebo po\u010d\u00edtania!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Tento NodeSet nem\u00f4\u017ee vykon\u00e1va\u0165 funkcie indexovania alebo po\u010d\u00edtania!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "Nie je mo\u017en\u00e9 vola\u0165 setShouldCacheNodes po volan\u00ed nextNode!"},
				new object[] {ER_ONLY_ALLOWS, "{0} povo\u013eulje iba {1} argumentov"},
				new object[] {ER_UNKNOWN_STEP, "Tvrdenie program\u00e1tora v getNextStepPos: nezn\u00e1my stepType: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "Po symbole '/' alebo '//' sa o\u010dak\u00e1vala cesta relat\u00edvneho umiestnenia."},
				new object[] {ER_EXPECTED_LOC_PATH, "O\u010dak\u00e1vala sa cesta umiestnenia, ale na\u0161iel sa tento symbol \u003a {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "Bola o\u010dak\u00e1van\u00e1 cesta umiestnenia, ale namiesto nej bol n\u00e1jden\u00fd koniec v\u00fdrazu XPath."},
				new object[] {ER_EXPECTED_LOC_STEP, "Po symbole '/' alebo '//' sa o\u010dak\u00e1val krok umiestnenia."},
				new object[] {ER_EXPECTED_NODE_TEST, "O\u010dak\u00e1val sa test uzlov, ktor\u00fd sa zhoduje bu\u010f s NCName:* alebo s QName."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "O\u010dak\u00e1val sa vzor kroku, ale bol zaznamenan\u00fd '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "O\u010dak\u00e1val sa vzor relat\u00edvnej cesty."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPathResult z XPath v\u00fdrazu ''{0}'' m\u00e1 XPathResultType {1}, ktor\u00fd sa ned\u00e1 skonvertova\u0165 do boolovsk\u00e9ho v\u00fdrazu."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPathResult z XPath v\u00fdrazu ''{0}'' m\u00e1 XPathResultType {1}, ktor\u00fd sa ned\u00e1 skonvertova\u0165 do jedn\u00e9ho uzla. Met\u00f3da getSingleNodeValue sa pou\u017e\u00edva iba pre typy ANY_UNORDERED_NODE_TYPE a FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "Met\u00f3da getSnapshotLength sa nem\u00f4\u017ee vola\u0165 na XPathResult z XPath v\u00fdrazu ''{0}'', preto\u017ee jeho XPathResultType je {1}. T\u00e1to met\u00f3da sa pou\u017eije iba pre typy UNORDERED_NODE_SNAPSHOT_TYPE a ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "Met\u00f3da iterateNext sa nem\u00f4\u017ee vola\u0165 na XPathResult z XPath v\u00fdrazu ''{0}'', preto\u017ee jej XPathResultType je {1}. T\u00e1to met\u00f3da sa pou\u017eije iba pre typy UNORDERED_NODE_ITERATOR_TYPE a ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "Dokument sa od vr\u00e1tenia v\u00fdsledku zmenil. Iter\u00e1tor je neplatn\u00fd."},
				new object[] {ER_INVALID_XPATH_TYPE, "Neplatn\u00fd argument typu XPath: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Pr\u00e1zdny objekt v\u00fdsledku XPath"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPathResult z XPath v\u00fdrazu ''{0}'' m\u00e1 XPathResultType {1}, ktor\u00fd sa ned\u00e1 stla\u010di\u0165 do \u0161pecifikovan\u00e9ho XPathResultType {2}."},
				new object[] {ER_NULL_RESOLVER, "Nie je mo\u017en\u00e9 rozl\u00ed\u0161i\u0165 predponu s rozli\u0161ova\u010dom nulovej predpony."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPathResult z XPath v\u00fdrazu ''{0}'' m\u00e1 XPathResultType {1}, ktor\u00fd sa ned\u00e1 skonvertova\u0165 na re\u0165azec."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "Met\u00f3da snapshotItem sa nem\u00f4\u017ee vola\u0165 na XPathResult z XPath v\u00fdrazu ''{0}'', preto\u017ee jej XPathResultType je {1}. T\u00e1to met\u00f3da sa pou\u017eije iba pre typy UNORDERED_NODE_SNAPSHOT_TYPE a ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "Uzol kontextu nepatr\u00ed k dokumentu, ktor\u00fd je viazan\u00fd na tento XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "Typ uzla kontextu nie je podporovan\u00fd."},
				new object[] {ER_XPATH_ERROR, "Nezn\u00e1ma chyba v XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPathResult z XPath v\u00fdrazu ''{0}'' m\u00e1 XPathResultType {1}, ktor\u00fd sa ned\u00e1 skonvertova\u0165 na \u010d\u00edslo"},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Funkcia roz\u0161\u00edrenia: ''{0}'' sa ned\u00e1 vyvola\u0165, ke\u010f je funkcia XMLConstants.FEATURE_SECURE_PROCESSING nastaven\u00e1 na hodnotu true."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable pre premenn\u00fa {0} vracia hodnotu null"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Nepodporovan\u00fd typ n\u00e1vratu : {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "Zdroj a/alebo typ n\u00e1vratu nem\u00f4\u017ee ma\u0165 hodnotu null"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "Argument {0} nem\u00f4\u017ee ma\u0165 hodnotu null"},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( Re\u0165azec objectModel ) nem\u00f4\u017ee by\u0165 volan\u00fd s objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( Re\u0165azec objectModel ) nem\u00f4\u017ee by\u0165 volan\u00fd s objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Prebieha pokus o nastavenie funkcie s n\u00e1zvom null: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Prebieha pokus o nastavenie nezn\u00e1mej funkcie \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Prebieha pokus o z\u00edskanie funkcie s n\u00e1zvom null: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Prebieha pokus o z\u00edskanie nezn\u00e1mej funkcie \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Prebieha pokus o nastavenie hodnoty null pre XPathFunctionResolver:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Prebieha pokus o nastavenie hodnoty null pre XPathVariableResolver:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "n\u00e1zov umiestnenia vo funkcii format-number e\u0161te nebol spracovan\u00fd!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "Vlastn\u00edctvo XSL nie je podporovan\u00e9: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "Nerobte moment\u00e1lne ni\u010d s n\u00e1zvov\u00fdm priestorom {0} vo vlastn\u00edctve: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException po\u010das pokusu o pr\u00edstup do syst\u00e9mov\u00e9ho vlastn\u00edctva XSL: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Star\u00e1 syntax: quo(...) u\u017e nie je v XPath definovan\u00e9."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath potrebuje odvoden\u00fd objekt na implement\u00e1ciu nodeTest!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "nebol n\u00e1jden\u00fd symbol funkcie."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "Nebolo mo\u017en\u00e9 n\u00e1js\u0165 funkciu: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Nie je mo\u017en\u00e9 vytvori\u0165 URL z: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "-E vo\u013eba nie je podporovan\u00e1 syntaktick\u00fdm analyz\u00e1torom DTM"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference bol dan\u00fd pre premenn\u00fa mimo kontext, alebo bez defin\u00edcie!  N\u00e1zov = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Nepodporovan\u00e9 k\u00f3dovanie: {0}"},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "Parameter na createMessage bol mimo ohrani\u010denia"},
				new object[] {"FORMAT_FAILED", "V\u00fdnimka po\u010das volania messageFormat"},
				new object[] {"version", ">>>>>>> Verzia Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u00e1no"},
				new object[] {"line", "Riadok #"},
				new object[] {"column", "St\u013apec #"},
				new object[] {"xsldone", "XSLProcessor: vykonan\u00e9"},
				new object[] {"xpath_option", "vo\u013eby xpath: "},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select vyjadrenie xpath]"},
				new object[] {"optionMatch", "   [-match porovn\u00e1vac\u00ed vzor (pre diagnostiku zhody)]"},
				new object[] {"optionAnyExpr", "Alebo len vyjadrenie xpath vykon\u00e1 v\u00fdpis pam\u00e4te diagnostiky"},
				new object[] {"noParsermsg1", "Proces XSL nebol \u00faspe\u0161n\u00fd."},
				new object[] {"noParsermsg2", "** Nebolo mo\u017en\u00e9 n\u00e1js\u0165 syntaktick\u00fd analyz\u00e1tor **"},
				new object[] {"noParsermsg3", "Skontroluje, pros\u00edm, svoju classpath."},
				new object[] {"noParsermsg4", "Ak nem\u00e1te Syntaktick\u00fd analyz\u00e1tor XML pre jazyk Java od firmy IBM, m\u00f4\u017eete si ho stiahnu\u0165 z"},
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
	  public const string ERROR_HEADER = "Chyba: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "Upozornenie: ";

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