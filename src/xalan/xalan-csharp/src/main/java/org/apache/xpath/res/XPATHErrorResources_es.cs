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
 * $Id: XPATHErrorResources_es.java 468655 2006-10-28 07:12:06Z minchau $
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
	public class XPATHErrorResources_es : ListResourceBundle
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
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "No est\u00e1 permitida la funci\u00f3n current() en un patr\u00f3n de coincidencia."},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "La funci\u00f3n current() no acepta argumentos."},
				new object[] {ER_DOCUMENT_REPLACED, "La implementaci\u00f3n de la funci\u00f3n document() ha sido sustituida por org.apache.xalan.xslt.FuncDocument."},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "El contexto no tiene un documento propietario."},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() tiene demasiados argumentos."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() tiene demasiados argumentos."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() tiene demasiados argumentos."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() tiene demasiados argumentos."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() tiene demasiados argumentos."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() tiene demasiados argumentos."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string-length() tiene demasiados argumentos."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "La funci\u00f3n translate() utiliza tres argumentos."},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "La funci\u00f3n unparsed-entity-uri deber\u00eda utilizar un solo argumento."},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "Eje de espacio de nombres a\u00fan no implementado."},
				new object[] {ER_UNKNOWN_AXIS, "Eje desconocido: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "Operaci\u00f3n de coincidencia desconocida."},
				new object[] {ER_INCORRECT_ARG_LENGTH, "La longitud del argumento de prueba del nodo processing-instruction() es incorrecta."},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "No se puede convertir {0} a un n\u00famero"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "No se puede convertir {0} a NodeList."},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "No se puede convertir {0} a NodeSetDTM."},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "No se puede convertir {0} a un tipo {1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "Se esperaba un patr\u00f3n de coincidencia en getMatchScore."},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "No se ha podido obtener la variable de nombre {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "ERROR. C\u00f3digo de operaci\u00f3n desconocido: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Se\u00f1ales extra no permitidas: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Literal sin entrecomillar... Se esperaban comillas dobles."},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Literal sin entrecomillar... Se esperaban comillas simples."},
				new object[] {ER_EMPTY_EXPRESSION, "Expresi\u00f3n vac\u00eda."},
				new object[] {ER_EXPECTED_BUT_FOUND, "Se esperaba {0}, pero se ha encontrado: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "La aserci\u00f3n del programador es incorrecta. - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "El argumento boolean(...) ya no es opcional con el borrador de XPath 19990709."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Se ha encontrado ',' pero sin argumento precedente."},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Se ha encontrado ',' pero sin argumento siguiente."},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predicate]' o '.[predicate]' es una sintaxis no permitida. Utilice 'self::node()[predicate]' en su lugar."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "Nombre de eje no permitido: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "nodetype desconocido: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "El literal del patr\u00f3n ({0}) tiene que estar entrecomillado."},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "No se ha podido formatear {0} como un n\u00famero."},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "No se ha podido crear Liaison TransformerFactory XML: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Error. No se ha encontrado la expresi\u00f3n de selecci\u00f3n (-select) de xpath."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "ERROR. No se ha podido encontrar ENDOP despu\u00e9s de OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Se ha producido un error."},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference dada para la variable est\u00e1 fuera de contexto o sin definici\u00f3n.  Nombre = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "S\u00f3lo se permiten los ejes child:: y attribute:: en patrones de coincidencia.  Ejes incorrectos = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() tiene un n\u00famero incorrecto de argumentos."},
				new object[] {ER_COUNT_TAKES_1_ARG, "La funci\u00f3n count deber\u00eda utilizar un solo argumento."},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "No se ha podido encontrar la funci\u00f3n: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Codificaci\u00f3n no soportada: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Se ha producido un problema en DTM en getNextSibling... Intentando recuperar"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Error del programador: No se puede escribir enEmptyNodeList."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory no soportada por XPathContext."},
				new object[] {ER_PREFIX_MUST_RESOLVE, "El prefijo debe resolverse como un espacio de nombres: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (InputSource source) no soportada en XPathContext. No se puede abrir {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "API SAX characters(char ch[]... no manejada por DTM."},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... no manejada por DTM."},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison no puede manejar nodos de tipo {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper no puede manejar nodos de tipo {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "Error DOM2Helper.parse: SystemID - {0} l\u00ednea - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "Error DOM2Helper.parse"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u00bfSe ha detectado un sustituto UTF-16 no v\u00e1lido: {0}?"},
				new object[] {ER_OIERROR, "Error de ES"},
				new object[] {ER_CANNOT_CREATE_URL, "No se puede crear url para: {0}"},
				new object[] {ER_XPATH_READOBJECT, "En XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "Se\u00f1al de funci\u00f3n no encontrada."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "No se puede tratar con el tipo XPath: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "Este NodeSet no es mutable"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "Este NodeSetDTM no es mutable"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Variable no resoluble: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Manejador de error nulo"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Aserci\u00f3n del programador: opcode desconocido: {0} "},
				new object[] {ER_ZERO_OR_ONE, "0 \u00f3 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() no soportada por XRTreeFragSelectWrapper"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() no soportada por XRTreeFragSelectWrapper"},
				new object[] {ER_DETACH_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "detach() no soportada por XRTreeFragSelectWrapper "},
				new object[] {ER_NUM_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "num() no soportada por XRTreeFragSelectWrapper"},
				new object[] {ER_XSTR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "xstr() no soportada por XRTreeFragSelectWrapper "},
				new object[] {ER_STR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "str() no soportada por XRTreeFragSelectWrapper"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() no soportada para XStringForChars"},
				new object[] {ER_COULD_NOT_FIND_VAR, "No se ha podido encontrar la variable con el nombre {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars no puede utilizar una serie para un argumento"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "El argumento FastStringBuffer no puede ser nulo"},
				new object[] {ER_TWO_OR_THREE, "2 \u00f3 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Se ha accedido a la variable antes de enlazarla."},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB no puede utilizar una serie para un argumento."},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n Error. Estableciendo ra\u00edz de walker como nulo."},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Este NodeSetDTM no puede iterar a un nodo previo."},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Este NodeSet no puede iterar a un nodo previo."},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Este NodeSetDTM no puede realizar funciones de indexaci\u00f3n o recuento."},
				new object[] {ER_NODESET_CANNOT_INDEX, "Este NodeSet no puede realizar funciones de indexaci\u00f3n o recuento."},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "No se puede llamar a setShouldCacheNodes despu\u00e9s de llamar a nextNode."},
				new object[] {ER_ONLY_ALLOWS, "{0} s\u00f3lo admite {1} argumentos"},
				new object[] {ER_UNKNOWN_STEP, "Aserci\u00f3n del programador en getNextStepPos: stepType desconocido: {0} "},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "Se esperaba una v\u00eda de acceso de ubicaci\u00f3n relativa despu\u00e9s de la se\u00f1al '/' o '//'."},
				new object[] {ER_EXPECTED_LOC_PATH, "Se esperaba una v\u00eda de acceso de ubicaci\u00f3n, pero se ha encontrado la se\u00f1al siguiente\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_PATH_AT_END_EXPR, "Se esperaba una v\u00eda de acceso de ubicaci\u00f3n, pero en su lugar se ha encontrado el final de la expresi\u00f3n XPath."},
				new object[] {ER_EXPECTED_LOC_STEP, "Se esperaba un paso de ubicaci\u00f3n despu\u00e9s de la se\u00f1al '/' o '//'."},
				new object[] {ER_EXPECTED_NODE_TEST, "Se esperaba una prueba de nodo coincidente con NCName:* o QName."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "Se esperaba un patr\u00f3n de paso, pero se ha encontrado '/'."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "Se esperaba un patr\u00f3n de v\u00eda de acceso relativa."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "XPathResult de la expresi\u00f3n XPath ''{0}'' tiene un XPathResultType de {1} que no se puede convertir a booleano."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "XPathResult de la expresi\u00f3n XPath ''{0}'' tiene un XPathResultType de {1} que no se puede convertir a un solo nodo. El m\u00e9todo getSingleNodeValue se aplica s\u00f3lo a tipos ANY_UNORDERED_NODE_TYPE and FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "No se puede llamar al m\u00e9todo getSnapshotLength en XPathResult de la expresi\u00f3n XPath ''{0}'' porque su XPathResultType es {1}. Este m\u00e9todo se aplica s\u00f3lo a los tipos UNORDERED_NODE_SNAPSHOT_TYPE y ORDERED_NODE_SNAPSHOT_TYPE. "},
				new object[] {ER_NON_ITERATOR_TYPE, "No se puede llamar al m\u00e9todo iterateNext en XPathResult de la expresi\u00f3n XPath ''{0}'' porque su XPathResultType es {1}. Este m\u00e9todo se aplica s\u00f3lo a los tipos UNORDERED_NODE_ITERATOR_TYPE y ORDERED_NODE_ITERATOR_TYPE. "},
				new object[] {ER_DOC_MUTATED, "El documento ha mutado desde que se devolvi\u00f3 el resultado. El iterador no es v\u00e1lido."},
				new object[] {ER_INVALID_XPATH_TYPE, "Argumento de tipo XPath no v\u00e1lido: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Objeto de resultado XPath vac\u00edo"},
				new object[] {ER_INCOMPATIBLE_TYPES, "XPathResult de la expresi\u00f3n XPath ''{0}'' tiene un XPathResultType de {1} que no se puede forzar al  XPathResultType especificado de {2}"},
				new object[] {ER_NULL_RESOLVER, "Imposible resolver prefijo con un solucionador de prefijo nulo."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "XPathResult de la expresi\u00f3n XPath ''{0}'' tiene un XPathResultType de {1} que no se puede convertir a una serie."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "No se puede llamar al m\u00e9todo snapshotItem en XPathResult de la expresi\u00f3n XPath ''{0}'' porque su XPathResultType es {1}. Este m\u00e9todo se aplica s\u00f3lo a los tipos UNORDERED_NODE_SNAPSHOT_TYPE y ORDERED_NODE_SNAPSHOT_TYPE. "},
				new object[] {ER_WRONG_DOCUMENT, "El nodo de contexto no pertenece al documento que est\u00e1 enlazado a este XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "El tipo de nodo de contexto no est\u00e1 soportado."},
				new object[] {ER_XPATH_ERROR, "Error desconocido en XPath."},
				new object[] {ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, "XPathResult de la expresi\u00f3n XPath ''{0}'' tiene un XPathResultType de {1} que no se puede convertir a un n\u00famero."},
				new object[] {ER_EXTENSION_FUNCTION_CANNOT_BE_INVOKED, "La funci\u00f3n de extensi\u00f3n: ''{0}'' no se puede invocar si la caracter\u00edstica XMLConstants.FEATURE_SECURE_PROCESSING est\u00e1 establecida en true."},
				new object[] {ER_RESOLVE_VARIABLE_RETURNS_NULL, "resolveVariable para la variable {0} devuelve null"},
				new object[] {ER_UNSUPPORTED_RETURN_TYPE, "Tipo devuelto no soportado : {0}"},
				new object[] {ER_SOURCE_RETURN_TYPE_CANNOT_BE_NULL, "El tipo de origen y/o devuelto no puede ser null"},
				new object[] {ER_ARG_CANNOT_BE_NULL, "El argumento {0} no puede ser null"},
				new object[] {ER_OBJECT_MODEL_NULL, "No se puede llamar a {0}#isObjectModelSupported( String objectModel ) con objectModel == null"},
				new object[] {ER_OBJECT_MODEL_EMPTY, "No se puede llamar a {0}#isObjectModelSupported( String objectModel ) con objectModel == \"\""},
				new object[] {ER_FEATURE_NAME_NULL, "Se ha intentado establecer una caracter\u00edstica con un nombre null: {0}#setFeature( null, {1})"},
				new object[] {ER_FEATURE_UNKNOWN, "Se ha intentado establecer la caracter\u00edstica \"{0}\":{1}#setFeature({0},{2}) desconocida"},
				new object[] {ER_GETTING_NULL_FEATURE, "Se ha intentado obtener una caracter\u00edstica con un nombre null: {0}#getFeature(null)"},
				new object[] {ER_GETTING_UNKNOWN_FEATURE, "Se ha intentado obtener la caracter\u00edstica desconocida \"{0}\":{1}#getFeature({0})"},
				new object[] {ER_NULL_XPATH_FUNCTION_RESOLVER, "Se ha intentado establecer un XPathFunctionResolver:{0}#setXPathFunctionResolver(null) null"},
				new object[] {ER_NULL_XPATH_VARIABLE_RESOLVER, "Se ha intentado establecer un XPathVariableResolver:{0}#setXPathVariableResolver(null) null"},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "No se maneja a\u00fan el nombre de entorno local en la funci\u00f3n format-number."},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "Propiedad XSL no soportada: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "No hacer nada actualmente con el espacio de nombres {0} en la propiedad: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException al intentar acceder a la propiedad del sistema XSL: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "La antigua sintaxis: quo(...) ya no est\u00e1 definida en XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath necesita un objeto derivado para implementar nodeTest."},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "Se\u00f1al de funci\u00f3n no encontrada."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "No se ha podido encontrar la funci\u00f3n: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "No se puede crear URL desde: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "Opci\u00f3n -E no soportada para analizador DTM"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference dada para la variable est\u00e1 fuera de contexto o sin definici\u00f3n  Nombre = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Codificaci\u00f3n no soportada: {0}"},
				new object[] {"ui_language", "es"},
				new object[] {"help_language", "es"},
				new object[] {"language", "es"},
				new object[] {"BAD_CODE", "El par\u00e1metro para createMessage estaba fuera de los l\u00edmites"},
				new object[] {"FORMAT_FAILED", "Se ha generado una excepci\u00f3n durante la llamada messageFormat"},
				new object[] {"version", ">>>>>>> Xalan versi\u00f3n "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "s\u00ed"},
				new object[] {"line", "L\u00ednea n\u00fam."},
				new object[] {"column", "Columna n\u00fam."},
				new object[] {"xsldone", "XSLProcessor: terminado"},
				new object[] {"xpath_option", "Opciones de xpath: "},
				new object[] {"optionIN", "[-in URLXMLEntrada]"},
				new object[] {"optionSelect", "[-select expresi\u00f3n xpath]"},
				new object[] {"optionMatch", "[-match patr\u00f3n de coincidencia (para diagn\u00f3sticos de coincidencia)]"},
				new object[] {"optionAnyExpr", "O simplemente una expresi\u00f3n xpath realizar\u00e1 un vuelco de diagn\u00f3stico"},
				new object[] {"noParsermsg1", "El proceso XSL no ha sido satisfactorio."},
				new object[] {"noParsermsg2", "** No se ha podido encontrar el analizador **"},
				new object[] {"noParsermsg3", "Compruebe la classpath."},
				new object[] {"noParsermsg4", "Si no dispone del analizador XML para Java de IBM, puede descargarlo de"},
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
	  public const string ERROR_HEADER = "Error: ";

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
			return (XPATHErrorResources) ResourceBundle.getBundle(className, new Locale("es", "ES"));
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