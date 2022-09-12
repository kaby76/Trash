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
 * $Id: XPATHErrorResources_pt_BR.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_pt_BR : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "A fun\u00e7\u00e3o current() n\u00e3o \u00e9 permitida em um padr\u00e3o de correspond\u00eancia!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "A fun\u00e7\u00e3o current() n\u00e3o aceita argumentos!"},
				new object[] {ER_DOCUMENT_REPLACED, "A implementa\u00e7\u00e3o da fun\u00e7\u00e3o document() foi substitu\u00edda por org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "context n\u00e3o possui um documento do propriet\u00e1rio!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() possui argumentos em excesso."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() possui argumentos em excesso."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() possui argumentos em excesso."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() possui argumentos em excesso."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() possui argumentos em excesso."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() possui argumentos em excesso."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() possui argumentos em excesso."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "A fun\u00e7\u00e3o translate() tem tr\u00eas argumentos!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "A fun\u00e7\u00e3o unparsed-entity-uri deve ter um argumento!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "eixo do espa\u00e7o de nomes ainda n\u00e3o implementado!"},
				new object[] {ER_UNKNOWN_AXIS, "eixo desconhecido: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "opera\u00e7\u00e3o de correspond\u00eancia desconhecida!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "O comprimento de arg do teste de n\u00f3 de processing-instruction() est\u00e1 incorreto! "},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "Imposs\u00edvel converter {0} em um n\u00famero"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "Imposs\u00edvel converter {0} em um NodeList!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "Imposs\u00edvel converter {0} em um NodeSetDTM!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "Imposs\u00edvel converter {0} em um tipo {1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "Padr\u00e3o de correspond\u00eancia esperado em getMatchScore!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "N\u00e3o foi poss\u00edvel obter a vari\u00e1vel {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "ERRO! C\u00f3digo op desconhecido: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Tokens inv\u00e1lidos extras: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "literal com aspa incorreta... era esperada aspa dupla!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "literal com aspa incorreta... era esperada aspa simples!"},
				new object[] {ER_EMPTY_EXPRESSION, "Express\u00e3o vazia!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "Esperado {0}, mas encontrado: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "A declara\u00e7\u00e3o do programador est\u00e1 incorreta! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "O argumento boolean(...) n\u00e3o \u00e9 mais opcional com o rascunho 19990709 XPath."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Encontrado ',' mas sem argumento precedente!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Encontrado ',' mas sem argumento seguinte!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' ou '.[predicate]' \u00e9 sintaxe inv\u00e1lida. Utilize ent\u00e3o 'self::node()[predicate]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "nome de eixo inv\u00e1lido: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Tipo de n\u00f3 desconhecido: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "O literal de padr\u00e3o ({0}) precisa ser colocado entre aspas!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} n\u00e3o p\u00f4de ser formatado para um n\u00famero!"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "N\u00e3o foi poss\u00edvel criar XML TransformerFactory Liaison: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Erro! N\u00e3o encontrada a express\u00e3o xpath select (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "ERRO! N\u00e3o foi poss\u00edvel encontrar ENDOP ap\u00f3s OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Ocorreu um erro!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference fornecido para a vari\u00e1vel fora de contexto ou sem defini\u00e7\u00e3o!  Nome = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "Apenas os eixos child:: e attribute:: s\u00e3o permitidos em padr\u00f5es de correspond\u00eancia! Eixos transgredidos = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() possui um n\u00famero incorreto de argumentos."},
				new object[] {ER_COUNT_TAKES_1_ARG, "A fun\u00e7\u00e3o count deve ter um argumento!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "N\u00e3o foi poss\u00edvel localizar a fun\u00e7\u00e3o: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Codifica\u00e7\u00e3o n\u00e3o suportada: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Ocorreu um problema no DTM em getNextSibling... tentando recuperar"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Erro do programador: EmptyNodeList n\u00e3o pode ser gravado."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory n\u00e3o \u00e9 suportado por XPathContext!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "O prefixo deve ser resolvido para um espa\u00e7o de nomes: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (origem InputSource) n\u00e3o suportada no XPathContext! Imposs\u00edvel abrir {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API characters(char ch[]... n\u00e3o tratado pelo DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... n\u00e3o tratado pelo DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison n\u00e3o pode tratar n\u00f3s do tipo {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper n\u00e3o pode tratar n\u00f3s do tipo {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse error: SystemID - {0} linha - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "Erro de DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Detectado substituto UTF-16 inv\u00e1lido: {0} ?"},
				new object[] {ER_OIERROR, "Erro de E/S"},
				new object[] {ER_CANNOT_CREATE_URL, "Imposs\u00edvel criar url para: {0}"},
				new object[] {ER_XPATH_READOBJECT, "Em XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "Token function n\u00e3o encontrado."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Imposs\u00edvel lidar com o tipo XPath: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Este NodeSet n\u00e3o \u00e9 mut\u00e1vel"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Este NodeSetDTM n\u00e3o \u00e9 mut\u00e1vel"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "A vari\u00e1vel n\u00e3o pode ser resolvida: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Rotina de tratamento de erros nula"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Declara\u00e7\u00e3o do programador: opcode desconhecido: {0} "},
				new object[] {ER_ZERO_OR_ONE, "0 ou 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() n\u00e3o suportado por XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() n\u00e3o suportado por XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() n\u00e3o suportado por XRTreeFragSelectWrapper "},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() n\u00e3o suportado por XRTreeFragSelectWrapper"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() n\u00e3o suportado por XRTreeFragSelectWrapper "},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() n\u00e3o suportado por XRTreeFragSelectWrapper"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() n\u00e3o suportado para XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "N\u00e3o foi poss\u00edvel encontrar a vari\u00e1vel com o nome {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars n\u00e3o pode obter uma cadeia para um argumento"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "O argumento FastStringBuffer n\u00e3o pode ser nulo"},
				new object[] {ER_TWO_OR_THREE, "2 ou 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Vari\u00e1vel acessada antes de ser ligada!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB n\u00e3o pode obter uma cadeia para um argumento!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! Erro! Definindo a raiz de um transmissor como nula!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Este NodeSetDTM n\u00e3o pode iterar em um n\u00f3 anterior!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Este NodeSet n\u00e3o pode iterar em um n\u00f3 anterior!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Este NodeSetDTM n\u00e3o pode executar fun\u00e7\u00f5es de indexa\u00e7\u00e3o ou de contagem!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Este NodeSet n\u00e3o pode executar fun\u00e7\u00f5es de indexa\u00e7\u00e3o ou de contagem!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "Imposs\u00edvel chamar setShouldCacheNodes depois de nextNode ter sido chamado!"},
				new object[] {ER_ONLY_ALLOWS, "{0} permite apenas {1} argumento(s)"},
				new object[] {ER_UNKNOWN_STEP, "Declara\u00e7\u00e3o do programador em getNextStepPos: stepType desconhecido: {0} "},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "Era esperado um caminho de localiza\u00e7\u00e3o relativo ap\u00f3s o token '/' ou '//'."},
				new object[] {ER_EXPECTED_LOC_PATH, "Era esperado um caminho de localiza\u00e7\u00e3o, mas o seguinte token foi encontrado\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "Era esperado um caminho de local, mas foi encontrado o final da express\u00e3o XPath: "},
				new object[] {ER_EXPECTED_LOC_STEP, "Era esperada uma etapa de localiza\u00e7\u00e3o ap\u00f3s o token '/' ou '//'."},
				new object[] {ER_EXPECTED_NODE_TEST, "Era esperado um n\u00f3 correspondente a NCName:* ou QName."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "Era esperado um padr\u00e3o de etapa, mas foi encontrado '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "Era esperado um padr\u00e3o de caminho relativo."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "O XPathResult da express\u00e3o XPath ''{0}'' tem um XPathResultType de {1} que n\u00e3o pode ser convertido em um booleano."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "O XPathResult da express\u00e3o XPath ''{0}'' tem um XPathResultType de {1} que n\u00e3o pode ser convertido em um \u00fanico n\u00f3. O m\u00e9todo getSingleNodeValue aplica-se apenas aos tipos ANY_UNORDERED_NODE_TYPE e FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "O m\u00e9todo getSnapshotLength n\u00e3o pode ser chamado no XPathResult da express\u00e3o XPath ''{0}'' porque seu XPathResultType \u00e9 {1}. Este m\u00e9todo aplica-se apenas aos tipos UNORDERED_NODE_SNAPSHOT_TYPE e ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "O m\u00e9todo iterateNext n\u00e3o pode ser chamado no XPathResult da express\u00e3o XPath ''{0}'' porque seu XPathResultType \u00e9 {1}. Este m\u00e9todo aplica-se apenas aos tipos UNORDERED_NODE_ITERATOR_TYPE e ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "Documento alterado desde o retorno do resultado. O iterador \u00e9 inv\u00e1lido."},
				new object[] {ER_INVALID_XPATH_TYPE, "Argumento de tipo XPath inv\u00e1lido: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Objeto de resultado XPath vazio"},
				new object[] {ER_INCOMPATIBLE_TYPES, "O XPathResult da express\u00e3o XPath ''{0}'' tem um XPathResultType de {1} que n\u00e3o pode ser for\u00e7ado no XPathResultType especificado de {2}."},
				new object[] {ER_NULL_RESOLVER, "N\u00e3o foi poss\u00edvel resolver o prefixo com um resolvedor de prefixo nulo."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "O XPathResult da express\u00e3o XPath ''{0}'' tem um XPathResultType de {1} que n\u00e3o pode ser convertido em uma cadeia."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "O m\u00e9todo snapshotItem n\u00e3o pode ser chamado no XPathResult da express\u00e3o XPath ''{0}'' porque seu XPathResultType \u00e9 {1}. Este m\u00e9todo aplica-se apenas aos tipos UNORDERED_NODE_SNAPSHOT_TYPE e ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "O n\u00f3 do contexto n\u00e3o pertence ao documento que est\u00e1 ligado a este XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "O tipo de n\u00f3 de contexto n\u00e3o \u00e9 suportado."},
				new object[] {ER_XPATH_ERROR, "Erro desconhecido em XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "O XPathResult da express\u00e3o XPath ''{0}'' tem um XPathResultType de {1} que n\u00e3o pode ser convertido em um n\u00famero."},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Fun\u00e7\u00e3o de extens\u00e3o: ''{0}'' n\u00e3o pode ser chamado quando o recurso XMLConstants.FEATURE_SECURE_PROCESSING est\u00e1 definido como true."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable para a vari\u00e1vel {0} retornando nulo"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Tipo de Retorno N\u00e3o Suportado : {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "O Tipo de Origem e/ou Retorno n\u00e3o pode ser nulo"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "O argumento {0} n\u00e3o pode ser nulo"},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( String objectModel ) n\u00e3o pode ser chamado com objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( String objectModel ) n\u00e3o pode ser chamado com objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Tentando definir um recurso com um nome nulo: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Tentando definir o recurso desconhecido \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Tentando obter um recurso com um nome nulo: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Tentando obter o recurso desconhecido \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Tentando definir um nulo XPathFunctionResolver:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Tentando definir um nulo XPathVariableResolver:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "nome de locale na fun\u00e7\u00e3o format-number ainda n\u00e3o tratado!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "Propriedade XSL n\u00e3o suportada: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "N\u00e3o fazer nada no momento com o espa\u00e7o de nomes {0} na propriedade {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException ao tentar acessar a propriedade do sistema XSL: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Sintaxe antiga: quo(...) n\u00e3o est\u00e1 mais definida no XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath precisa de um objeto derivado para implementar nodeTest!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "Token function n\u00e3o encontrado."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "N\u00e3o foi poss\u00edvel localizar a fun\u00e7\u00e3o: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Imposs\u00edvel criar URL a partir de: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "A op\u00e7\u00e3o -E n\u00e3o \u00e9 suportada pelo analisador do DTM"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference fornecido para a vari\u00e1vel fora de contexto ou sem defini\u00e7\u00e3o!  Nome = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Codifica\u00e7\u00e3o n\u00e3o suportada: {0}"},
				new object[] {"ui_language", "pt"},
				new object[] {"help_language", "pt"},
				new object[] {"language", "pt"},
				new object[] {"BAD_CODE", "O par\u00e2metro para createMessage estava fora dos limites"},
				new object[] {"FORMAT_FAILED", "Exce\u00e7\u00e3o emitida durante chamada messageFormat"},
				new object[] {"version", ">>>>>>> Vers\u00e3o Xalan"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "sim"},
				new object[] {"line", "Linha n\u00b0"},
				new object[] {"column", "Coluna n\u00b0"},
				new object[] {"xsldone", "XSLProcessor: conclu\u00eddo"},
				new object[] {"xpath_option", "op\u00e7\u00f5es xpath:"},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select xpath expression]"},
				new object[] {"optionMatch", "[-match match pattern (para corresponder diagn\u00f3sticos)]"},
				new object[] {"optionAnyExpr", "Ou apenas uma express\u00e3o xpath executar\u00e1 um dump de diagn\u00f3stico"},
				new object[] {"noParsermsg1", "O Processo XSL n\u00e3o obteve \u00eaxito."},
				new object[] {"noParsermsg2", "** N\u00e3o foi poss\u00edvel encontrar o analisador **"},
				new object[] {"noParsermsg3", "Verifique seu classpath."},
				new object[] {"noParsermsg4", "Se voc\u00ea n\u00e3o tiver o XML Parser para Java da IBM, poder\u00e1 fazer o download dele a partir de"},
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
	  public const string ERROR_HEADER = "Erro: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public const string WARNING_HEADER = "Aviso: ";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public const string QUERY_HEADER = "PADR\u00c3O ";


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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("pt", "BR"));
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