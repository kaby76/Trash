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
 * $Id: XPATHErrorResources_ca.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_ca : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "La funci\u00f3 current() no \u00e9s permesa en un patr\u00f3 de coincid\u00e8ncia."},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "La funci\u00f3 current() no accepta arguments."},
				new object[] {ER_DOCUMENT_REPLACED, "La implementaci\u00f3 de la funci\u00f3 document() s'ha substitu\u00eft per org.apache.xalan.xslt.FuncDocument."},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "El context no t\u00e9 un document de propietari."},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() t\u00e9 massa arguments."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() t\u00e9 massa arguments."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() t\u00e9 massa arguments."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() t\u00e9 massa arguments."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() t\u00e9 massa arguments."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() t\u00e9 massa arguments."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() t\u00e9 massa arguments."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "La funci\u00f3 translate() t\u00e9 tres arguments."},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "La funci\u00f3 unparsed-entity-uri ha de tenir un argument."},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "L'eix de l'espai de noms encara no s'ha implementat."},
				new object[] {ER_UNKNOWN_AXIS, "Eix desconegut: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "Operaci\u00f3 de coincid\u00e8ncia desconeguda."},
				new object[] {ER_INCORRECT_ARG_LENGTH, "La longitud de l'argument de la prova de node processing-instruction() no \u00e9s correcta."},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "No es pot convertir {0} en un n\u00famero."},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "No es pot convertir {0} en una NodeList."},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "No es pot convertir {0} en un NodeSetDTM."},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "No es pot convertir {0} en un tipus #{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "El patr\u00f3 de coincid\u00e8ncia de getMatchScore \u00e9s l'esperat."},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "No s''ha pogut obtenir la variable {0}."},
				new object[] {ER_UNKNOWN_OPCODE, "ERROR. Codi op desconegut: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Senyals addicionals no permesos: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Les cometes del literal s\u00f3n incorrectes. Hi ha d'haver cometes dobles."},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Les cometes del literal s\u00f3n incorrectes. Hi ha d'haver una cometa simple."},
				new object[] {ER_EMPTY_EXPRESSION, "Expressi\u00f3 buida."},
				new object[] {ER_EXPECTED_BUT_FOUND, "S''esperava {0}, per\u00f2 s''ha detectat {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "L''afirmaci\u00f3 del programador \u00e9s incorrecta. - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "L'argument boolean(...) ja no \u00e9s opcional amb l'esborrany d'XPath 19990709."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "S'ha trobat ',' per\u00f2 al davant no hi havia cap argument."},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "S'ha trobat ',' per\u00f2 al darrere no hi havia cap argument."},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' o '.[predicate]' no \u00e9s una sintaxi permesa. En comptes d'aix\u00f2, utilitzeu 'self::node()[predicate]'."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "Nom d''eix no perm\u00e8s: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "Tipus de node desconegut: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "El literal de patr\u00f3 ({0}) ha d''anar entre cometes."},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} no s''ha pogut formatar com a n\u00famero."},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "No s''ha pogut crear la relaci\u00f3 XML TransformerFactory: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Error. No s'ha trobat l'expressi\u00f3 select d'xpath (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "ERROR. No s'ha trobat ENDOP despr\u00e9s d'OP_LOCATIONPATH."},
				new object[] {ER_ERROR_OCCURED, "S'ha produ\u00eft un error."},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "S''ha donat VariableReference per a una variable fora de context o sense definici\u00f3. Nom = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "Nom\u00e9s es permeten els eixos subordinat:: i atribut:: en els patrons de coincid\u00e8ncia. Eixos incorrectes = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() t\u00e9 un nombre incorrecte d'arguments."},
				new object[] {ER_COUNT_TAKES_1_ARG, "La funci\u00f3 count ha de tenir un argument."},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "No s''ha pogut trobar la funci\u00f3: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Codificaci\u00f3 sense suport: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "S'ha produ\u00eft un error en el DTM de getNextSibling. S'intentar\u00e0 solucionar."},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Error del programador: no es pot escriure a EmptyNodeList."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "XPathContext no d\u00f3na suport a setDOMFactory."},
				new object[] {ER_PREFIX_MUST_RESOLVE, "El prefix s''ha de resoldre en un espai de noms: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "L''an\u00e0lisi (origen InputSource) no t\u00e9 suport a XPathContext. No es pot obrir {0}."},
				new object[] {ER_SAX_API_NOT_HANDLED, "Els car\u00e0cters de l'API SAX (char ch[]... no es poden gestionar pel DTM."},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... no es poden gestionar pel DTM."},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison no pot gestionar nodes del tipus {0}."},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper no pot gestionar nodes del tipus {0}."},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "Error de DOM2Helper.parse: ID del sistema - {0} l\u00ednia - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "Error de DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "S''ha detectat un suplent UTF-16 no v\u00e0lid: {0} ?"},
				new object[] {ER_OIERROR, "Error d'E/S"},
				new object[] {ER_CANNOT_CREATE_URL, "No es pot crear la URL de: {0}"},
				new object[] {ER_XPATH_READOBJECT, "En XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "No s'ha trobat el senyal de funci\u00f3."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "No s''ha pogut tractar amb el tipus d''XPath: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Aquest NodeSet no \u00e9s mutable."},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Aquest NodeSetDTM no \u00e9s mutable."},
				new object[] {ER_VAR_NOT_RESOLVABLE, "No es pot resoldre la variable: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Manejador d'error nul"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Afirmaci\u00f3 del programador: opcode desconegut: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 o 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() no t\u00e9 suport d'XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() no t\u00e9 suport d'XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() no t\u00e9 suport d'XRTreeFragSelectWrapper"},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() no t\u00e9 suport d'XRTreeFragSelectWrapper"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() no t\u00e9 suport d'XRTreeFragSelectWrapper"},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() no t\u00e9 suport d'XRTreeFragSelectWrapper"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() no t\u00e9 suport d'XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "No s''ha trobat la variable amb el nom de {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars no pot agafar una cadena com a argument."},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "L'argument FastStringBuffer no pot ser nul."},
				new object[] {ER_TWO_OR_THREE, "2 o 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "S'ha accedit a la variable abans que estigu\u00e9s vinculada."},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB no pot agafar una cadena com a argument."},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n Error. S'est\u00e0 establint l'arrel d'un itinerant en nul."},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Aquest NodeSetDTM no es pot iterar en un node previ"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Aquest NodeSet no es pot iterar en un node previ"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Aquest NodeSetDTM no pot indexar ni efectuar funcions de recompte"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Aquest NodeSet no pot indexar ni efectuar funcions de recompte"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "No es pot cridar setShouldCacheNodes despr\u00e9s que s'hagi cridat nextNode"},
				new object[] {ER_ONLY_ALLOWS, "{0} nom\u00e9s permet {1} arguments"},
				new object[] {ER_UNKNOWN_STEP, "Afirmaci\u00f3 del programador a getNextStepPos: stepType desconegut: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "S'esperava una via d'acc\u00e9s relativa darrere del senyal '/' o '//'."},
				new object[] {ER_EXPECTED_LOC_PATH, "S''esperava una via d''acc\u00e9s d''ubicaci\u00f3, per\u00f2 s''ha trobat el senyal seg\u00fcent\u003a {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "S'esperava una via d'acc\u00e9s, per\u00f2 enlloc d'aix\u00f2 s'ha trobat el final de l'expressi\u00f3 XPath. "},
				new object[] {ER_EXPECTED_LOC_STEP, "S'esperava un pas d'ubicaci\u00f3 despr\u00e9s del senyal '/' o '//'."},
				new object[] {ER_EXPECTED_NODE_TEST, "S'esperava una prova de node que coincid\u00eds amb NCName:* o QName."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "S'esperava un patr\u00f3 de pas per\u00f2 s'ha trobat '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "S'esperava un patr\u00f3 de via d'acc\u00e9s relativa."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "L''expressi\u00f3 XPathResult d''XPath ''{0}'' t\u00e9 un XPathResultType de {1} que no es pot convertir a un cap valor boole\u00e0. "},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "L''expressi\u00f3 XPathResult d''XPath ''{0}'' t\u00e9 un XPathResultType de {1} que no es pot convertir a un node \u00fanic. El m\u00e8tode getSingleNodeValue s''aplica nom\u00e9s al tipus ANY_UNORDERED_NODE_TYPE i FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "El m\u00e8tode getSnapshotLength no es pot cridar a l''expressi\u00f3 XPathResult d''XPath ''{0}'' perqu\u00e8 el seu XPathResultType \u00e9s {1}. Aquest m\u00e8tode nom\u00e9s s''aplica als tipus UNORDERED_NODE_SNAPSHOT_TYPE i ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "El m\u00e8tode iterateNext no es pot cridar a l''expressi\u00f3 XPathResult d''XPath ''{0}'' perqu\u00e8 el seu XPathResultType \u00e9s {1}. Aquest m\u00e8tode nom\u00e9s s''aplica als tipus UNORDERED_NODE_ITERATOR_TYPE i ORDERED_NODE_ITERATOR_TYPE."},
				new object[] {ER_DOC_MUTATED, "El document s'ha modificat des que es van produir els resultats. L'iterador no \u00e9s v\u00e0lid."},
				new object[] {ER_INVALID_XPATH_TYPE, "L''argument de tipus XPath no \u00e9s v\u00e0lid: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "L'objecte de resultats XPath est\u00e0 buit."},
				new object[] {ER_INCOMPATIBLE_TYPES, "L''expressi\u00f3 XPathResult d''XPath ''{0}'' t\u00e9 un XPathResultType de {1} que no es pot encaixar al XPathResultType especificat de {2}."},
				new object[] {ER_NULL_RESOLVER, "No es pot resoldre el prefix amb un solucionador de prefix nul."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "L''expressi\u00f3 XPathResult d''XPath ''{0}'' t\u00e9 un XPathResultType de {1} que no es pot convertir a cap cadena. "},
				new object[] {ER_NON_SNAPSHOT_TYPE, "El m\u00e8tode snapshotItem no es pot cridar a l''expressi\u00f3 XPathResult d''XPath ''{0}'' perqu\u00e8 el seu XPathResultType \u00e9s {1}. Aquest m\u00e8tode nom\u00e9s s''aplica als tipus UNORDERED_NODE_SNAPSHOT_TYPE i ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "El node de context no pertany al document vinculat a aquest XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "El tipus de node de context no t\u00e9 suport."},
				new object[] {ER_XPATH_ERROR, "S'ha produ\u00eft un error desconegut a XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "L''expressi\u00f3 XPathResult d''XPath ''{0}'' t\u00e9 un XPathResultType de {1} que no es pot convertir a cap n\u00famero "},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "Funci\u00f3 d''extensi\u00f3: no es pot invocar ''{0}'' si la caracter\u00edstica XMLConstants.FEATURE_SECURE_PROCESSING s''ha establert en true."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable de la variable {0} torna el valor nul"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Tipus de retorn no suportat: {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "El tipus de retorn o d'origen no pot ser nul"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "L''argument {0} no pot ser nul "},
				new object[] {ER_OBJECT_MODEL_NULL, "{0}#isObjectModelSupported( String objectModel) no es pot cridar amb objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "{0}#isObjectModelSupported( String objectModel ) no es pot cridar amb objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Intent d''establir una caracter\u00edstica amb un nom nul: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Intent d''establir una caracter\u00edstica desconeguda \"{0}\":{1}#setFeature({0},{2})"},
				new object[] {ER_GETTING_NULL_FEATURE, "Intent d''obtenir una caracter\u00edstica amb un nom nul: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Intent d''obtenir la caracter\u00edstica desconeguda \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "S''ha intentat establir un XPathFunctionResolver nul:{0}#setXPathFunctionResolver(null)"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "S''ha intentat establir un XPathVariableResolver null:{0}#setXPathVariableResolver(null)"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "No s'ha gestionat encara el nom d'entorn nacional en la funci\u00f3 format-number."},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "La propietat XSL no t\u00e9 suport: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "No feu res ara mateix amb l''espai de noms {0} de la propietat: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "S''ha produ\u00eft SecurityException en intentar accedir a la propietat de sistema XSL: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Sintaxi antiga: quo(...) ja no est\u00e0 definit a XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath necessita un objecte dedu\u00eft per implementar nodeTest."},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "No s'ha trobat el senyal de funci\u00f3."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "No s''ha pogut trobar la funci\u00f3: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "No es pot crear la URL de: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "L'opci\u00f3 -E no t\u00e9 suport a l'analitzador de DTM"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "S''ha donat VariableReference per a una variable fora de context o sense definici\u00f3. Nom = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Codificaci\u00f3 sense suport: {0}"},
				new object[] {"ui_language", "ca"},
				new object[] {"help_language", "ca"},
				new object[] {"language", "ca"},
				new object[] {"BAD_CODE", "El par\u00e0metre de createMessage estava fora dels l\u00edmits."},
				new object[] {"FORMAT_FAILED", "S'ha generat una excepci\u00f3 durant la crida messageFormat."},
				new object[] {"version", ">>>>>>> Versi\u00f3 Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "s\u00ed"},
				new object[] {"line", "L\u00ednia n\u00fam."},
				new object[] {"column", "Columna n\u00fam."},
				new object[] {"xsldone", "XSLProcessor: fet"},
				new object[] {"xpath_option", "Opcions d'xpath: "},
				new object[] {"optionIN", "   [-in inputXMLURL]"},
				new object[] {"optionSelect", "   [-select expressi\u00f3 xpath]"},
				new object[] {"optionMatch", "   [-match patr\u00f3 coincid\u00e8ncia (per a diagn\u00f2stics de coincid\u00e8ncia)]"},
				new object[] {"optionAnyExpr", "O nom\u00e9s una expressi\u00f3 xpath far\u00e0 un buidatge de diagn\u00f2stic."},
				new object[] {"noParsermsg1", "El proc\u00e9s XSL no ha estat correcte."},
				new object[] {"noParsermsg2", "** No s'ha trobat l'analitzador **"},
				new object[] {"noParsermsg3", "Comproveu la vostra classpath."},
				new object[] {"noParsermsg4", "Si no teniu XML Parser for Java d'IBM, el podeu baixar de l'indret web"},
				new object[] {"noParsermsg5", "AlphaWorks d'IBM: http://www.alphaworks.ibm.com/formula/xml"},
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
	  public const string WARNING_HEADER = "Av\u00eds: ";

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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("ca", "ES"));
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