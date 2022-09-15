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
 * $Id: XSLTErrorResources_pl.java 468641 2006-10-28 06:54:42Z minchau $
 */
namespace org.apache.xalan.res
{

	/// <summary>
	/// Set up error messages.
	/// We build a two dimensional array of message keys and
	/// message strings. In order to add a new message here,
	/// you need to first add a String constant. And
	///  you need to enter key , value pair as part of contents
	/// Array. You also need to update MAX_CODE for error strings
	/// and MAX_WARNING for warnings ( Needed for only information
	/// purpose )
	/// </summary>
	public class XSLTErrorResources_pl : ListResourceBundle
	{

	/*
	 * This file contains error and warning messages related to Xalan Error
	 * Handling.
	 *
	 *  General notes to translators:
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
	 */

	  /// <summary>
	  /// Maximum error messages, this is needed to keep track of the number of messages. </summary>
	  public const int MAX_CODE = 201;

	  /// <summary>
	  /// Maximum warnings, this is needed to keep track of the number of warnings. </summary>
	  public const int MAX_WARNING = 29;

	  /// <summary>
	  /// Maximum misc strings. </summary>
	  public const int MAX_OTHERS = 55;

	  /// <summary>
	  /// Maximum total warnings and error messages. </summary>
	  public static readonly int MAX_MESSAGES = MAX_CODE + MAX_WARNING + 1;


	  /*
	   * Static variables
	   */
	  public const string ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX = "ER_INVALID_SET_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX";

	  public const string ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT = "ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT";

	  public const string ER_NO_CURLYBRACE = "ER_NO_CURLYBRACE";
	  public const string ER_FUNCTION_NOT_SUPPORTED = "ER_FUNCTION_NOT_SUPPORTED";
	  public const string ER_ILLEGAL_ATTRIBUTE = "ER_ILLEGAL_ATTRIBUTE";
	  public const string ER_NULL_SOURCENODE_APPLYIMPORTS = "ER_NULL_SOURCENODE_APPLYIMPORTS";
	  public const string ER_CANNOT_ADD = "ER_CANNOT_ADD";
	  public const string ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES = "ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES";
	  public const string ER_NO_NAME_ATTRIB = "ER_NO_NAME_ATTRIB";
	  public const string ER_TEMPLATE_NOT_FOUND = "ER_TEMPLATE_NOT_FOUND";
	  public const string ER_CANT_RESOLVE_NAME_AVT = "ER_CANT_RESOLVE_NAME_AVT";
	  public const string ER_REQUIRES_ATTRIB = "ER_REQUIRES_ATTRIB";
	  public const string ER_MUST_HAVE_TEST_ATTRIB = "ER_MUST_HAVE_TEST_ATTRIB";
	  public const string ER_BAD_VAL_ON_LEVEL_ATTRIB = "ER_BAD_VAL_ON_LEVEL_ATTRIB";
	  public const string ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML = "ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML";
	  public const string ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME = "ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME";
	  public const string ER_NEED_MATCH_ATTRIB = "ER_NEED_MATCH_ATTRIB";
	  public const string ER_NEED_NAME_OR_MATCH_ATTRIB = "ER_NEED_NAME_OR_MATCH_ATTRIB";
	  public const string ER_CANT_RESOLVE_NSPREFIX = "ER_CANT_RESOLVE_NSPREFIX";
	  public const string ER_ILLEGAL_VALUE = "ER_ILLEGAL_VALUE";
	  public const string ER_NO_OWNERDOC = "ER_NO_OWNERDOC";
	  public const string ER_ELEMTEMPLATEELEM_ERR = "ER_ELEMTEMPLATEELEM_ERR";
	  public const string ER_NULL_CHILD = "ER_NULL_CHILD";
	  public const string ER_NEED_SELECT_ATTRIB = "ER_NEED_SELECT_ATTRIB";
	  public const string ER_NEED_TEST_ATTRIB = "ER_NEED_TEST_ATTRIB";
	  public const string ER_NEED_NAME_ATTRIB = "ER_NEED_NAME_ATTRIB";
	  public const string ER_NO_CONTEXT_OWNERDOC = "ER_NO_CONTEXT_OWNERDOC";
	  public const string ER_COULD_NOT_CREATE_XML_PROC_LIAISON = "ER_COULD_NOT_CREATE_XML_PROC_LIAISON";
	  public const string ER_PROCESS_NOT_SUCCESSFUL = "ER_PROCESS_NOT_SUCCESSFUL";
	  public const string ER_NOT_SUCCESSFUL = "ER_NOT_SUCCESSFUL";
	  public const string ER_ENCODING_NOT_SUPPORTED = "ER_ENCODING_NOT_SUPPORTED";
	  public const string ER_COULD_NOT_CREATE_TRACELISTENER = "ER_COULD_NOT_CREATE_TRACELISTENER";
	  public const string ER_KEY_REQUIRES_NAME_ATTRIB = "ER_KEY_REQUIRES_NAME_ATTRIB";
	  public const string ER_KEY_REQUIRES_MATCH_ATTRIB = "ER_KEY_REQUIRES_MATCH_ATTRIB";
	  public const string ER_KEY_REQUIRES_USE_ATTRIB = "ER_KEY_REQUIRES_USE_ATTRIB";
	  public const string ER_REQUIRES_ELEMENTS_ATTRIB = "ER_REQUIRES_ELEMENTS_ATTRIB";
	  public const string ER_MISSING_PREFIX_ATTRIB = "ER_MISSING_PREFIX_ATTRIB";
	  public const string ER_BAD_STYLESHEET_URL = "ER_BAD_STYLESHEET_URL";
	  public const string ER_FILE_NOT_FOUND = "ER_FILE_NOT_FOUND";
	  public const string ER_IOEXCEPTION = "ER_IOEXCEPTION";
	  public const string ER_NO_HREF_ATTRIB = "ER_NO_HREF_ATTRIB";
	  public const string ER_STYLESHEET_INCLUDES_ITSELF = "ER_STYLESHEET_INCLUDES_ITSELF";
	  public const string ER_PROCESSINCLUDE_ERROR = "ER_PROCESSINCLUDE_ERROR";
	  public const string ER_MISSING_LANG_ATTRIB = "ER_MISSING_LANG_ATTRIB";
	  public const string ER_MISSING_CONTAINER_ELEMENT_COMPONENT = "ER_MISSING_CONTAINER_ELEMENT_COMPONENT";
	  public const string ER_CAN_ONLY_OUTPUT_TO_ELEMENT = "ER_CAN_ONLY_OUTPUT_TO_ELEMENT";
	  public const string ER_PROCESS_ERROR = "ER_PROCESS_ERROR";
	  public const string ER_UNIMPLNODE_ERROR = "ER_UNIMPLNODE_ERROR";
	  public const string ER_NO_SELECT_EXPRESSION = "ER_NO_SELECT_EXPRESSION";
	  public const string ER_CANNOT_SERIALIZE_XSLPROCESSOR = "ER_CANNOT_SERIALIZE_XSLPROCESSOR";
	  public const string ER_NO_INPUT_STYLESHEET = "ER_NO_INPUT_STYLESHEET";
	  public const string ER_FAILED_PROCESS_STYLESHEET = "ER_FAILED_PROCESS_STYLESHEET";
	  public const string ER_COULDNT_PARSE_DOC = "ER_COULDNT_PARSE_DOC";
	  public const string ER_COULDNT_FIND_FRAGMENT = "ER_COULDNT_FIND_FRAGMENT";
	  public const string ER_NODE_NOT_ELEMENT = "ER_NODE_NOT_ELEMENT";
	  public const string ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB = "ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB";
	  public const string ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB = "ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB";
	  public const string ER_NO_CLONE_OF_DOCUMENT_FRAG = "ER_NO_CLONE_OF_DOCUMENT_FRAG";
	  public const string ER_CANT_CREATE_ITEM = "ER_CANT_CREATE_ITEM";
	  public const string ER_XMLSPACE_ILLEGAL_VALUE = "ER_XMLSPACE_ILLEGAL_VALUE";
	  public const string ER_NO_XSLKEY_DECLARATION = "ER_NO_XSLKEY_DECLARATION";
	  public const string ER_CANT_CREATE_URL = "ER_CANT_CREATE_URL";
	  public const string ER_XSLFUNCTIONS_UNSUPPORTED = "ER_XSLFUNCTIONS_UNSUPPORTED";
	  public const string ER_PROCESSOR_ERROR = "ER_PROCESSOR_ERROR";
	  public const string ER_NOT_ALLOWED_INSIDE_STYLESHEET = "ER_NOT_ALLOWED_INSIDE_STYLESHEET";
	  public const string ER_RESULTNS_NOT_SUPPORTED = "ER_RESULTNS_NOT_SUPPORTED";
	  public const string ER_DEFAULTSPACE_NOT_SUPPORTED = "ER_DEFAULTSPACE_NOT_SUPPORTED";
	  public const string ER_INDENTRESULT_NOT_SUPPORTED = "ER_INDENTRESULT_NOT_SUPPORTED";
	  public const string ER_ILLEGAL_ATTRIB = "ER_ILLEGAL_ATTRIB";
	  public const string ER_UNKNOWN_XSL_ELEM = "ER_UNKNOWN_XSL_ELEM";
	  public const string ER_BAD_XSLSORT_USE = "ER_BAD_XSLSORT_USE";
	  public const string ER_MISPLACED_XSLWHEN = "ER_MISPLACED_XSLWHEN";
	  public const string ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE = "ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE";
	  public const string ER_MISPLACED_XSLOTHERWISE = "ER_MISPLACED_XSLOTHERWISE";
	  public const string ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE = "ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE";
	  public const string ER_NOT_ALLOWED_INSIDE_TEMPLATE = "ER_NOT_ALLOWED_INSIDE_TEMPLATE";
	  public const string ER_UNKNOWN_EXT_NS_PREFIX = "ER_UNKNOWN_EXT_NS_PREFIX";
	  public const string ER_IMPORTS_AS_FIRST_ELEM = "ER_IMPORTS_AS_FIRST_ELEM";
	  public const string ER_IMPORTING_ITSELF = "ER_IMPORTING_ITSELF";
	  public const string ER_XMLSPACE_ILLEGAL_VAL = "ER_XMLSPACE_ILLEGAL_VAL";
	  public const string ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL = "ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL";
	  public const string ER_SAX_EXCEPTION = "ER_SAX_EXCEPTION";
	  public const string ER_XSLT_ERROR = "ER_XSLT_ERROR";
	  public const string ER_CURRENCY_SIGN_ILLEGAL = "ER_CURRENCY_SIGN_ILLEGAL";
	  public const string ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM = "ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM";
	  public const string ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER = "ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER";
	  public const string ER_REDIRECT_COULDNT_GET_FILENAME = "ER_REDIRECT_COULDNT_GET_FILENAME";
	  public const string ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT = "ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT";
	  public const string ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX = "ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX";
	  public const string ER_MISSING_NS_URI = "ER_MISSING_NS_URI";
	  public const string ER_MISSING_ARG_FOR_OPTION = "ER_MISSING_ARG_FOR_OPTION";
	  public const string ER_INVALID_OPTION = "ER_INVALID_OPTION";
	  public const string ER_MALFORMED_FORMAT_STRING = "ER_MALFORMED_FORMAT_STRING";
	  public const string ER_STYLESHEET_REQUIRES_VERSION_ATTRIB = "ER_STYLESHEET_REQUIRES_VERSION_ATTRIB";
	  public const string ER_ILLEGAL_ATTRIBUTE_VALUE = "ER_ILLEGAL_ATTRIBUTE_VALUE";
	  public const string ER_CHOOSE_REQUIRES_WHEN = "ER_CHOOSE_REQUIRES_WHEN";
	  public const string ER_NO_APPLY_IMPORT_IN_FOR_EACH = "ER_NO_APPLY_IMPORT_IN_FOR_EACH";
	  public const string ER_CANT_USE_DTM_FOR_OUTPUT = "ER_CANT_USE_DTM_FOR_OUTPUT";
	  public const string ER_CANT_USE_DTM_FOR_INPUT = "ER_CANT_USE_DTM_FOR_INPUT";
	  public const string ER_CALL_TO_EXT_FAILED = "ER_CALL_TO_EXT_FAILED";
	  public const string ER_PREFIX_MUST_RESOLVE = "ER_PREFIX_MUST_RESOLVE";
	  public const string ER_INVALID_UTF16_SURROGATE = "ER_INVALID_UTF16_SURROGATE";
	  public const string ER_XSLATTRSET_USED_ITSELF = "ER_XSLATTRSET_USED_ITSELF";
	  public const string ER_CANNOT_MIX_XERCESDOM = "ER_CANNOT_MIX_XERCESDOM";
	  public const string ER_TOO_MANY_LISTENERS = "ER_TOO_MANY_LISTENERS";
	  public const string ER_IN_ELEMTEMPLATEELEM_READOBJECT = "ER_IN_ELEMTEMPLATEELEM_READOBJECT";
	  public const string ER_DUPLICATE_NAMED_TEMPLATE = "ER_DUPLICATE_NAMED_TEMPLATE";
	  public const string ER_INVALID_KEY_CALL = "ER_INVALID_KEY_CALL";
	  public const string ER_REFERENCING_ITSELF = "ER_REFERENCING_ITSELF";
	  public const string ER_ILLEGAL_DOMSOURCE_INPUT = "ER_ILLEGAL_DOMSOURCE_INPUT";
	  public const string ER_CLASS_NOT_FOUND_FOR_OPTION = "ER_CLASS_NOT_FOUND_FOR_OPTION";
	  public const string ER_REQUIRED_ELEM_NOT_FOUND = "ER_REQUIRED_ELEM_NOT_FOUND";
	  public const string ER_INPUT_CANNOT_BE_NULL = "ER_INPUT_CANNOT_BE_NULL";
	  public const string ER_URI_CANNOT_BE_NULL = "ER_URI_CANNOT_BE_NULL";
	  public const string ER_FILE_CANNOT_BE_NULL = "ER_FILE_CANNOT_BE_NULL";
	  public const string ER_SOURCE_CANNOT_BE_NULL = "ER_SOURCE_CANNOT_BE_NULL";
	  public const string ER_CANNOT_INIT_BSFMGR = "ER_CANNOT_INIT_BSFMGR";
	  public const string ER_CANNOT_CMPL_EXTENSN = "ER_CANNOT_CMPL_EXTENSN";
	  public const string ER_CANNOT_CREATE_EXTENSN = "ER_CANNOT_CREATE_EXTENSN";
	  public const string ER_INSTANCE_MTHD_CALL_REQUIRES = "ER_INSTANCE_MTHD_CALL_REQUIRES";
	  public const string ER_INVALID_ELEMENT_NAME = "ER_INVALID_ELEMENT_NAME";
	  public const string ER_ELEMENT_NAME_METHOD_STATIC = "ER_ELEMENT_NAME_METHOD_STATIC";
	  public const string ER_EXTENSION_FUNC_UNKNOWN = "ER_EXTENSION_FUNC_UNKNOWN";
	  public const string ER_MORE_MATCH_CONSTRUCTOR = "ER_MORE_MATCH_CONSTRUCTOR";
	  public const string ER_MORE_MATCH_METHOD = "ER_MORE_MATCH_METHOD";
	  public const string ER_MORE_MATCH_ELEMENT = "ER_MORE_MATCH_ELEMENT";
	  public const string ER_INVALID_CONTEXT_PASSED = "ER_INVALID_CONTEXT_PASSED";
	  public const string ER_POOL_EXISTS = "ER_POOL_EXISTS";
	  public const string ER_NO_DRIVER_NAME = "ER_NO_DRIVER_NAME";
	  public const string ER_NO_URL = "ER_NO_URL";
	  public const string ER_POOL_SIZE_LESSTHAN_ONE = "ER_POOL_SIZE_LESSTHAN_ONE";
	  public const string ER_INVALID_DRIVER = "ER_INVALID_DRIVER";
	  public const string ER_NO_STYLESHEETROOT = "ER_NO_STYLESHEETROOT";
	  public const string ER_ILLEGAL_XMLSPACE_VALUE = "ER_ILLEGAL_XMLSPACE_VALUE";
	  public const string ER_PROCESSFROMNODE_FAILED = "ER_PROCESSFROMNODE_FAILED";
	  public const string ER_RESOURCE_COULD_NOT_LOAD = "ER_RESOURCE_COULD_NOT_LOAD";
	  public const string ER_BUFFER_SIZE_LESSTHAN_ZERO = "ER_BUFFER_SIZE_LESSTHAN_ZERO";
	  public const string ER_UNKNOWN_ERROR_CALLING_EXTENSION = "ER_UNKNOWN_ERROR_CALLING_EXTENSION";
	  public const string ER_NO_NAMESPACE_DECL = "ER_NO_NAMESPACE_DECL";
	  public const string ER_ELEM_CONTENT_NOT_ALLOWED = "ER_ELEM_CONTENT_NOT_ALLOWED";
	  public const string ER_STYLESHEET_DIRECTED_TERMINATION = "ER_STYLESHEET_DIRECTED_TERMINATION";
	  public const string ER_ONE_OR_TWO = "ER_ONE_OR_TWO";
	  public const string ER_TWO_OR_THREE = "ER_TWO_OR_THREE";
	  public const string ER_COULD_NOT_LOAD_RESOURCE = "ER_COULD_NOT_LOAD_RESOURCE";
	  public const string ER_CANNOT_INIT_DEFAULT_TEMPLATES = "ER_CANNOT_INIT_DEFAULT_TEMPLATES";
	  public const string ER_RESULT_NULL = "ER_RESULT_NULL";
	  public const string ER_RESULT_COULD_NOT_BE_SET = "ER_RESULT_COULD_NOT_BE_SET";
	  public const string ER_NO_OUTPUT_SPECIFIED = "ER_NO_OUTPUT_SPECIFIED";
	  public const string ER_CANNOT_TRANSFORM_TO_RESULT_TYPE = "ER_CANNOT_TRANSFORM_TO_RESULT_TYPE";
	  public const string ER_CANNOT_TRANSFORM_SOURCE_TYPE = "ER_CANNOT_TRANSFORM_SOURCE_TYPE";
	  public const string ER_NULL_CONTENT_HANDLER = "ER_NULL_CONTENT_HANDLER";
	  public const string ER_NULL_ERROR_HANDLER = "ER_NULL_ERROR_HANDLER";
	  public const string ER_CANNOT_CALL_PARSE = "ER_CANNOT_CALL_PARSE";
	  public const string ER_NO_PARENT_FOR_FILTER = "ER_NO_PARENT_FOR_FILTER";
	  public const string ER_NO_STYLESHEET_IN_MEDIA = "ER_NO_STYLESHEET_IN_MEDIA";
	  public const string ER_NO_STYLESHEET_PI = "ER_NO_STYLESHEET_PI";
	  public const string ER_NOT_SUPPORTED = "ER_NOT_SUPPORTED";
	  public const string ER_PROPERTY_VALUE_BOOLEAN = "ER_PROPERTY_VALUE_BOOLEAN";
	  public const string ER_COULD_NOT_FIND_EXTERN_SCRIPT = "ER_COULD_NOT_FIND_EXTERN_SCRIPT";
	  public const string ER_RESOURCE_COULD_NOT_FIND = "ER_RESOURCE_COULD_NOT_FIND";
	  public const string ER_OUTPUT_PROPERTY_NOT_RECOGNIZED = "ER_OUTPUT_PROPERTY_NOT_RECOGNIZED";
	  public const string ER_FAILED_CREATING_ELEMLITRSLT = "ER_FAILED_CREATING_ELEMLITRSLT";
	  public const string ER_VALUE_SHOULD_BE_NUMBER = "ER_VALUE_SHOULD_BE_NUMBER";
	  public const string ER_VALUE_SHOULD_EQUAL = "ER_VALUE_SHOULD_EQUAL";
	  public const string ER_FAILED_CALLING_METHOD = "ER_FAILED_CALLING_METHOD";
	  public const string ER_FAILED_CREATING_ELEMTMPL = "ER_FAILED_CREATING_ELEMTMPL";
	  public const string ER_CHARS_NOT_ALLOWED = "ER_CHARS_NOT_ALLOWED";
	  public const string ER_ATTR_NOT_ALLOWED = "ER_ATTR_NOT_ALLOWED";
	  public const string ER_BAD_VALUE = "ER_BAD_VALUE";
	  public const string ER_ATTRIB_VALUE_NOT_FOUND = "ER_ATTRIB_VALUE_NOT_FOUND";
	  public const string ER_ATTRIB_VALUE_NOT_RECOGNIZED = "ER_ATTRIB_VALUE_NOT_RECOGNIZED";
	  public const string ER_NULL_URI_NAMESPACE = "ER_NULL_URI_NAMESPACE";
	  public const string ER_NUMBER_TOO_BIG = "ER_NUMBER_TOO_BIG";
	  public const string ER_CANNOT_FIND_SAX1_DRIVER = "ER_CANNOT_FIND_SAX1_DRIVER";
	  public const string ER_SAX1_DRIVER_NOT_LOADED = "ER_SAX1_DRIVER_NOT_LOADED";
	  public const string ER_SAX1_DRIVER_NOT_INSTANTIATED = "ER_SAX1_DRIVER_NOT_INSTANTIATED";
	  public const string ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER = "ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER";
	  public const string ER_PARSER_PROPERTY_NOT_SPECIFIED = "ER_PARSER_PROPERTY_NOT_SPECIFIED";
	  public const string ER_PARSER_ARG_CANNOT_BE_NULL = "ER_PARSER_ARG_CANNOT_BE_NULL";
	  public const string ER_FEATURE = "ER_FEATURE";
	  public const string ER_PROPERTY = "ER_PROPERTY";
	  public const string ER_NULL_ENTITY_RESOLVER = "ER_NULL_ENTITY_RESOLVER";
	  public const string ER_NULL_DTD_HANDLER = "ER_NULL_DTD_HANDLER";
	  public const string ER_NO_DRIVER_NAME_SPECIFIED = "ER_NO_DRIVER_NAME_SPECIFIED";
	  public const string ER_NO_URL_SPECIFIED = "ER_NO_URL_SPECIFIED";
	  public const string ER_POOLSIZE_LESS_THAN_ONE = "ER_POOLSIZE_LESS_THAN_ONE";
	  public const string ER_INVALID_DRIVER_NAME = "ER_INVALID_DRIVER_NAME";
	  public const string ER_ERRORLISTENER = "ER_ERRORLISTENER";
	  public const string ER_ASSERT_NO_TEMPLATE_PARENT = "ER_ASSERT_NO_TEMPLATE_PARENT";
	  public const string ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR = "ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR";
	  public const string ER_NOT_ALLOWED_IN_POSITION = "ER_NOT_ALLOWED_IN_POSITION";
	  public const string ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION = "ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION";
	  public const string ER_NAMESPACE_CONTEXT_NULL_NAMESPACE = "ER_NAMESPACE_CONTEXT_NULL_NAMESPACE";
	  public const string ER_NAMESPACE_CONTEXT_NULL_PREFIX = "ER_NAMESPACE_CONTEXT_NULL_PREFIX";
	  public const string ER_XPATH_RESOLVER_NULL_QNAME = "ER_XPATH_RESOLVER_NULL_QNAME";
	  public const string ER_XPATH_RESOLVER_NEGATIVE_ARITY = "ER_XPATH_RESOLVER_NEGATIVE_ARITY";
	  public const string INVALID_TCHAR = "INVALID_TCHAR";
	  public const string INVALID_QNAME = "INVALID_QNAME";
	  public const string INVALID_ENUM = "INVALID_ENUM";
	  public const string INVALID_NMTOKEN = "INVALID_NMTOKEN";
	  public const string INVALID_NCNAME = "INVALID_NCNAME";
	  public const string INVALID_BOOLEAN = "INVALID_BOOLEAN";
	  public const string INVALID_NUMBER = "INVALID_NUMBER";
	  public const string ER_ARG_LITERAL = "ER_ARG_LITERAL";
	  public const string ER_DUPLICATE_GLOBAL_VAR = "ER_DUPLICATE_GLOBAL_VAR";
	  public const string ER_DUPLICATE_VAR = "ER_DUPLICATE_VAR";
	  public const string ER_TEMPLATE_NAME_MATCH = "ER_TEMPLATE_NAME_MATCH";
	  public const string ER_INVALID_PREFIX = "ER_INVALID_PREFIX";
	  public const string ER_NO_ATTRIB_SET = "ER_NO_ATTRIB_SET";
	  public const string ER_FUNCTION_NOT_FOUND = "ER_FUNCTION_NOT_FOUND";
	  public const string ER_CANT_HAVE_CONTENT_AND_SELECT = "ER_CANT_HAVE_CONTENT_AND_SELECT";
	  public const string ER_INVALID_SET_PARAM_VALUE = "ER_INVALID_SET_PARAM_VALUE";
	  public const string ER_SET_FEATURE_NULL_NAME = "ER_SET_FEATURE_NULL_NAME";
	  public const string ER_GET_FEATURE_NULL_NAME = "ER_GET_FEATURE_NULL_NAME";
	  public const string ER_UNSUPPORTED_FEATURE = "ER_UNSUPPORTED_FEATURE";
	  public const string ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING = "ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING";

	  public const string WG_FOUND_CURLYBRACE = "WG_FOUND_CURLYBRACE";
	  public const string WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR = "WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR";
	  public const string WG_EXPR_ATTRIB_CHANGED_TO_SELECT = "WG_EXPR_ATTRIB_CHANGED_TO_SELECT";
	  public const string WG_NO_LOCALE_IN_FORMATNUMBER = "WG_NO_LOCALE_IN_FORMATNUMBER";
	  public const string WG_LOCALE_NOT_FOUND = "WG_LOCALE_NOT_FOUND";
	  public const string WG_CANNOT_MAKE_URL_FROM = "WG_CANNOT_MAKE_URL_FROM";
	  public const string WG_CANNOT_LOAD_REQUESTED_DOC = "WG_CANNOT_LOAD_REQUESTED_DOC";
	  public const string WG_CANNOT_FIND_COLLATOR = "WG_CANNOT_FIND_COLLATOR";
	  public const string WG_FUNCTIONS_SHOULD_USE_URL = "WG_FUNCTIONS_SHOULD_USE_URL";
	  public const string WG_ENCODING_NOT_SUPPORTED_USING_UTF8 = "WG_ENCODING_NOT_SUPPORTED_USING_UTF8";
	  public const string WG_ENCODING_NOT_SUPPORTED_USING_JAVA = "WG_ENCODING_NOT_SUPPORTED_USING_JAVA";
	  public const string WG_SPECIFICITY_CONFLICTS = "WG_SPECIFICITY_CONFLICTS";
	  public const string WG_PARSING_AND_PREPARING = "WG_PARSING_AND_PREPARING";
	  public const string WG_ATTR_TEMPLATE = "WG_ATTR_TEMPLATE";
	  public const string WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE = "WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESP";
	  public const string WG_ATTRIB_NOT_HANDLED = "WG_ATTRIB_NOT_HANDLED";
	  public const string WG_NO_DECIMALFORMAT_DECLARATION = "WG_NO_DECIMALFORMAT_DECLARATION";
	  public const string WG_OLD_XSLT_NS = "WG_OLD_XSLT_NS";
	  public const string WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED = "WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED";
	  public const string WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE = "WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE";
	  public const string WG_ILLEGAL_ATTRIBUTE = "WG_ILLEGAL_ATTRIBUTE";
	  public const string WG_COULD_NOT_RESOLVE_PREFIX = "WG_COULD_NOT_RESOLVE_PREFIX";
	  public const string WG_STYLESHEET_REQUIRES_VERSION_ATTRIB = "WG_STYLESHEET_REQUIRES_VERSION_ATTRIB";
	  public const string WG_ILLEGAL_ATTRIBUTE_NAME = "WG_ILLEGAL_ATTRIBUTE_NAME";
	  public const string WG_ILLEGAL_ATTRIBUTE_VALUE = "WG_ILLEGAL_ATTRIBUTE_VALUE";
	  public const string WG_EMPTY_SECOND_ARG = "WG_EMPTY_SECOND_ARG";
	  public const string WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML = "WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML";
	  public const string WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME = "WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME";
	  public const string WG_ILLEGAL_ATTRIBUTE_POSITION = "WG_ILLEGAL_ATTRIBUTE_POSITION";
	  public const string NO_MODIFICATION_ALLOWED_ERR = "NO_MODIFICATION_ALLOWED_ERR";

	  /*
	   * Now fill in the message text.
	   * Then fill in the message text for that message code in the
	   * array. Use the new error code as the index into the array.
	   */

	  // Error messages...

	  /// <summary>
	  /// Get the lookup table for error messages.
	  /// </summary>
	  /// <returns> The message lookup table. </returns>
	  public virtual object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ER0000", "{0}"},
				new object[] {ER_NO_CURLYBRACE, "B\u0142\u0105d: Wewn\u0105trz wyra\u017cenia nie mo\u017ce by\u0107 znaku '{'"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} ma niedozwolony atrybut {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode jest puste w xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "Nie mo\u017cna doda\u0107 {0} do {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode jest puste w handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} musi mie\u0107 atrybut name."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Nie mo\u017cna znale\u017a\u0107 szablonu o nazwie {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Nie mo\u017cna przet\u0142umaczy\u0107 AVT nazwy na xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} wymaga atrybutu: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} musi mie\u0107 atrybut ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "B\u0142\u0119dna warto\u015b\u0107 w atrybucie level: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Nazw\u0105 instrukcji przetwarzania nie mo\u017ce by\u0107 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Nazwa instrukcji przetwarzania musi by\u0107 poprawn\u0105 nazw\u0105 NCName {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} musi mie\u0107 atrybut match, je\u015bli ma mode."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} wymaga albo atrybutu name, albo match."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Nie mo\u017cna rozstrzygn\u0105\u0107 przedrostka przestrzeni nazw {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space ma niepoprawn\u0105 warto\u015b\u0107 {0}"},
				new object[] {ER_NO_OWNERDOC, "Bezpo\u015bredni w\u0119ze\u0142 potomny nie ma dokumentu w\u0142a\u015bciciela!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "B\u0142\u0105d ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "Pr\u00f3ba dodania pustego bezpo\u015bredniego elementu potomnego!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} wymaga atrybutu select."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when musi mie\u0107 atrybut 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param musi mie\u0107 atrybut 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "Kontekst nie ma dokumentu w\u0142a\u015bciciela!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Nie mo\u017cna utworzy\u0107 po\u0142\u0105czenia XML TransformerFactory: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Proces Xalan nie wykona\u0142 si\u0119 pomy\u015blnie."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan nie wykona\u0142 si\u0119 pomy\u015blnie."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Nieobs\u0142ugiwane kodowanie {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Nie mo\u017cna utworzy\u0107 TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key wymaga atrybutu 'name'."},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key wymaga atrybutu 'match'."},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key wymaga atrybutu 'use'."},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} wymaga atrybutu ''elements''!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} brakuje atrybutu ''prefix''"},
				new object[] {ER_BAD_STYLESHEET_URL, "Adres URL arkusza styl\u00f3w jest b\u0142\u0119dny {0}"},
				new object[] {ER_FILE_NOT_FOUND, "Nie znaleziono pliku arkusza styl\u00f3w {0}"},
				new object[] {ER_IOEXCEPTION, "Wyst\u0105pi\u0142 wyj\u0105tek we/wy w pliku arkusza styl\u00f3w {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Nie mo\u017cna znale\u017a\u0107 atrybutu href dla {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} zawiera siebie bezpo\u015brednio lub po\u015brednio!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "B\u0142\u0105d StylesheetHandler.processInclude {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} brakuje atrybutu ''lang''"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) \u017ale umieszczony element {0}?? Brakuje elementu kontenera ''component''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Mo\u017cna wyprowadza\u0107 dane tylko do Element, DocumentFragment, Document lub PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "B\u0142\u0105d StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "B\u0142\u0105d UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "B\u0142\u0105d! Nie znaleziono wyra\u017cenia wyboru xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Nie mo\u017cna szeregowa\u0107 XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Nie podano danych wej\u015bciowych do arkusza styl\u00f3w!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Nie powiod\u0142o si\u0119 przetworzenie arkusza styl\u00f3w!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Nie mo\u017cna zanalizowa\u0107 dokumentu {0}!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Nie mo\u017cna znale\u017a\u0107 fragmentu {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "W\u0119ze\u0142 wskazywany przez identyfikator fragmentu nie by\u0142 elementem {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each musi mie\u0107 albo atrybut match, albo name"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "templates musi mie\u0107 albo atrybut match, albo name"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Brak klonu fragmentu dokumentu!"},
				new object[] {ER_CANT_CREATE_ITEM, "Nie mo\u017cna utworzy\u0107 elementu w wynikowym drzewie {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space w \u017ar\u00f3d\u0142owym pliku XML ma niepoprawn\u0105 warto\u015b\u0107 {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Nie ma deklaracji xsl:key dla {0}!"},
				new object[] {ER_CANT_CREATE_URL, "B\u0142\u0105d! Nie mo\u017cna utworzy\u0107 adresu url dla {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions jest nieobs\u0142ugiwane"},
				new object[] {ER_PROCESSOR_ERROR, "B\u0142\u0105d XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} jest niedozwolone wewn\u0105trz arkusza styl\u00f3w!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns nie jest ju\u017c obs\u0142ugiwane!  U\u017cyj zamiast tego xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space nie jest ju\u017c obs\u0142ugiwane!  U\u017cyj zamiast tego xsl:strip-space lub xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result nie jest ju\u017c obs\u0142ugiwane!  U\u017cyj zamiast tego xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} ma niedozwolony atrybut {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Nieznany element XSL {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort mo\u017ce by\u0107 u\u017cywane tylko z xsl:apply-templates lub xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) b\u0142\u0119dnie umieszczone xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when bez nadrz\u0119dnego xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) b\u0142\u0119dnie umieszczone xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise bez nadrz\u0119dnego xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} jest niedozwolone wewn\u0105trz szablonu!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) Nieznany przedrostek {1} rozszerzenia {0} przestrzeni nazw"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Importy mog\u0105 wyst\u0105pi\u0107 tylko jako pierwsze elementy w arkuszu styl\u00f3w!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} importuje siebie bezpo\u015brednio lub po\u015brednio!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space ma niedozwolon\u0105 warto\u015b\u0107: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet by\u0142o niepomy\u015blne!"},
				new object[] {ER_SAX_EXCEPTION, "Wyj\u0105tek SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Nieobs\u0142ugiwana funkcja!"},
				new object[] {ER_XSLT_ERROR, "B\u0142\u0105d XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "Znak waluty jest niedozwolony w ci\u0105gu znak\u00f3w wzorca formatu"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Funkcja Document nie jest obs\u0142ugiwana w arkuszu styl\u00f3w DOM!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Nie mo\u017cna rozstrzygn\u0105\u0107 przedrostka przelicznika bez przedrostka!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Rozszerzenie Redirect: Nie mo\u017cna pobra\u0107 nazwy pliku - atrybut file lub select musi zwr\u00f3ci\u0107 poprawny ci\u0105g znak\u00f3w."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Nie mo\u017cna zbudowa\u0107 FormatterListener w rozszerzeniu Redirect!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Przedrostek w exclude-result-prefixes jest niepoprawny: {0}"},
				new object[] {ER_MISSING_NS_URI, "Nieobecny identyfikator URI przestrzeni nazw w podanym przedrostku"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Nieobecny argument opcji {0}"},
				new object[] {ER_INVALID_OPTION, "Niepoprawna opcja {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Zniekszta\u0142cony ci\u0105g znak\u00f3w formatu {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet wymaga atrybutu 'version'!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Atrybut {0} ma niepoprawn\u0105 warto\u015b\u0107 {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose wymaga xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports jest niedozwolone w xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Nie mo\u017cna u\u017cy\u0107 DTMLiaison w wyj\u015bciowym w\u0119\u017ale DOM... przeka\u017c zamiast tego org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Nie mo\u017cna u\u017cy\u0107 DTMLiaison w wej\u015bciowym w\u0119\u017ale DOM... przeka\u017c zamiast tego org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Wywo\u0142anie elementu rozszerzenia nie powiod\u0142o si\u0119: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Przedrostek musi da\u0107 si\u0119 przet\u0142umaczy\u0107 na przestrze\u0144 nazw: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Wykryto niepoprawny odpowiednik UTF-16: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} u\u017cy\u0142o siebie, co wywo\u0142a niesko\u0144czon\u0105 p\u0119tl\u0119."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Nie mo\u017cna miesza\u0107 wej\u015bcia innego ni\u017c Xerces-DOM z wyj\u015bciem Xerces-DOM!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "W ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Znaleziono wi\u0119cej ni\u017c jeden szablon o nazwie {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Niepoprawne wywo\u0142anie funkcji: Rekurencyjne wywo\u0142ania key() s\u0105 niedozwolone"},
				new object[] {ER_REFERENCING_ITSELF, "Zmienna {0} odwo\u0142uje si\u0119 do siebie bezpo\u015brednio lub po\u015brednio!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "W\u0119ze\u0142 wej\u015bciowy nie mo\u017ce by\u0107 pusty dla DOMSource dla newTemplates!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Nie znaleziono pliku klasy dla opcji {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Nie znaleziono wymaganego elementu {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream nie mo\u017ce by\u0107 pusty"},
				new object[] {ER_URI_CANNOT_BE_NULL, "Identyfikator URI nie mo\u017ce by\u0107 pusty"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "File nie mo\u017ce by\u0107 pusty"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource nie mo\u017ce by\u0107 pusty"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Nie mo\u017cna zainicjowa\u0107 mened\u017cera BSF"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Nie mo\u017cna skompilowa\u0107 rozszerzenia"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Nie mo\u017cna utworzy\u0107 rozszerzenia {0} z powodu  {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Wywo\u0142anie metody Instance do metody {0} wymaga instancji Object jako pierwszego argumentu"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Podano niepoprawn\u0105 nazw\u0119 elementu {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Metoda nazwy elementu musi by\u0107 statyczna {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Funkcja rozszerzenia {0} : {1} jest nieznana"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Wi\u0119cej ni\u017c jedno najlepsze dopasowanie dla konstruktora {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Wi\u0119cej ni\u017c jedno najlepsze dopasowanie dla metody {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Wi\u0119cej ni\u017c jedno najlepsze dopasowanie dla metody elementu {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Przekazano niepoprawny kontekst do wyliczenia {0}"},
				new object[] {ER_POOL_EXISTS, "Pula ju\u017c istnieje"},
				new object[] {ER_NO_DRIVER_NAME, "Nie podano nazwy sterownika"},
				new object[] {ER_NO_URL, "Nie podano adresu URL"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "Wielko\u015b\u0107 puli jest mniejsza od jedno\u015bci!"},
				new object[] {ER_INVALID_DRIVER, "Podano niepoprawn\u0105 nazw\u0119 sterownika!"},
				new object[] {ER_NO_STYLESHEETROOT, "Nie znaleziono elementu g\u0142\u00f3wnego arkusza styl\u00f3w!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Niedozwolona warto\u015b\u0107 xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode nie powiod\u0142o si\u0119"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Zas\u00f3b [ {0} ] nie m\u00f3g\u0142 za\u0142adowa\u0107: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Wielko\u015b\u0107 buforu <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Nieznany b\u0142\u0105d podczas wywo\u0142ywania rozszerzenia"},
				new object[] {ER_NO_NAMESPACE_DECL, "Przedrostek {0} nie ma odpowiadaj\u0105cej mu deklaracji przestrzeni nazw"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Zawarto\u015b\u0107 elementu niedozwolona dla lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Arkusz styl\u00f3w zarz\u0105dzi\u0142 zako\u0144czenie"},
				new object[] {ER_ONE_OR_TWO, "1 lub 2"},
				new object[] {ER_TWO_OR_THREE, "2 lub 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Nie mo\u017cna za\u0142adowa\u0107 {0} (sprawd\u017a CLASSPATH), u\u017cywane s\u0105 teraz warto\u015bci domy\u015blne"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Nie mo\u017cna zainicjowa\u0107 domy\u015blnych szablon\u00f3w"},
				new object[] {ER_RESULT_NULL, "Rezultat nie powinien by\u0107 pusty"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Nie mo\u017cna ustawi\u0107 rezultatu"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Nie podano wyj\u015bcia"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Nie mo\u017cna przekszta\u0142ci\u0107 do rezultatu o typie {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Nie mo\u017cna przekszta\u0142ci\u0107 \u017ar\u00f3d\u0142a o typie {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Pusta procedura obs\u0142ugi zawarto\u015bci"},
				new object[] {ER_NULL_ERROR_HANDLER, "Pusta procedura obs\u0142ugi b\u0142\u0119du"},
				new object[] {ER_CANNOT_CALL_PARSE, "Nie mo\u017cna wywo\u0142a\u0107 parse, je\u015bli nie ustawiono ContentHandler"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Brak elementu nadrz\u0119dnego dla filtru"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Nie znaleziono arkusza styl\u00f3w w {0}, no\u015bnik= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Nie znaleziono instrukcji przetwarzania xml-stylesheet w {0}"},
				new object[] {ER_NOT_SUPPORTED, "Nieobs\u0142ugiwane: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "Warto\u015b\u0107 w\u0142a\u015bciwo\u015bci {0} powinna by\u0107 instancj\u0105 typu Boolean"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Nie mo\u017cna si\u0119 dosta\u0107 do zewn\u0119trznego skryptu w {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "Nie mo\u017cna znale\u017a\u0107 zasobu [ {0} ].\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Nierozpoznana w\u0142a\u015bciwo\u015b\u0107 wyj\u015bciowa {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Nie powiod\u0142o si\u0119 utworzenie instancji ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "Warto\u015b\u0107 {0} powinna zawiera\u0107 liczb\u0119 mo\u017cliw\u0105 do zanalizowania"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "Warto\u015bci\u0105 {0} powinno by\u0107 yes lub no"},
				new object[] {ER_FAILED_CALLING_METHOD, "Niepowodzenie wywo\u0142ania metody {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Nie powiod\u0142o si\u0119 utworzenie instancji ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "W tym miejscu dokumentu znaki s\u0105 niedozwolone"},
				new object[] {ER_ATTR_NOT_ALLOWED, "Atrybut \"{0}\" nie jest dozwolony w elemencie {1}!"},
				new object[] {ER_BAD_VALUE, "B\u0142\u0119dna warto\u015b\u0107 {0} {1}"},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Nie znaleziono warto\u015bci atrybutu {0}"},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Nie rozpoznano warto\u015bci atrybutu {0}"},
				new object[] {ER_NULL_URI_NAMESPACE, "Pr\u00f3ba wygenerowania przedrostka przestrzeni nazw z pustym identyfikatorem URI"},
				new object[] {ER_NUMBER_TOO_BIG, "Pr\u00f3ba sformatowania liczby wi\u0119kszej ni\u017c najwi\u0119ksza liczba typu long integer"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Nie mo\u017cna znale\u017a\u0107 klasy sterownika SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "Znaleziono klas\u0119 sterownika SAX1 {0}, ale nie mo\u017cna jej za\u0142adowa\u0107"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "Klasa sterownika SAX1 {0} zosta\u0142a za\u0142adowana, ale nie mo\u017cna utworzy\u0107 jej instancji"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "Klasa sterownika SAX1 {0} nie implementuje org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "W\u0142a\u015bciwo\u015b\u0107 systemowa org.xml.sax.parser nie zosta\u0142a podana"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Argument analizatora nie mo\u017ce by\u0107 pusty"},
				new object[] {ER_FEATURE, "Opcja: {0}"},
				new object[] {ER_PROPERTY, "W\u0142a\u015bciwo\u015b\u0107: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Pusty przelicznik encji"},
				new object[] {ER_NULL_DTD_HANDLER, "Pusta procedura obs\u0142ugi DTD"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Nie podano nazwy sterownika!"},
				new object[] {ER_NO_URL_SPECIFIED, "Nie podano adresu URL!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "Wielko\u015b\u0107 puli jest mniejsza od 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Podano niepoprawn\u0105 nazw\u0119 sterownika!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "B\u0142\u0105d programisty! Wyra\u017cenie nie ma elementu nadrz\u0119dnego ElemTemplateElement!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Asercja programisty w RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} jest niedozwolone na tej pozycji w arkuszu styl\u00f3w!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Tekst z\u0142o\u017cony ze znak\u00f3w innych ni\u017c odst\u0119py jest niedozwolony na tej pozycji w arkuszu styl\u00f3w!"},
				new object[] {INVALID_TCHAR, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie CHAR {0}.  Atrybut typu CHAR musi by\u0107 pojedynczym znakiem!"},
				new object[] {INVALID_QNAME, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie QNAME {0}"},
				new object[] {INVALID_ENUM, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie ENUM {0}.  Poprawne warto\u015bci to: {2}."},
				new object[] {INVALID_NMTOKEN, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie NMTOKEN {0}"},
				new object[] {INVALID_NCNAME, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie NCNAME {0}"},
				new object[] {INVALID_BOOLEAN, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie logicznym {0}"},
				new object[] {INVALID_NUMBER, "Niedozwolona warto\u015b\u0107 {1} u\u017cyta w atrybucie liczbowym {0}"},
				new object[] {ER_ARG_LITERAL, "Argument opcji {0} we wzorcu uzgadniania musi by\u0107 litera\u0142em."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Zduplikowana deklaracja zmiennej globalnej."},
				new object[] {ER_DUPLICATE_VAR, "Zduplikowana deklaracja zmiennej."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template musi mie\u0107 atrybut name lub match (lub obydwa)"},
				new object[] {ER_INVALID_PREFIX, "Przedrostek w exclude-result-prefixes jest niepoprawny: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "Zbi\u00f3r atrybut\u00f3w o nazwie {0} nie istnieje"},
				new object[] {ER_FUNCTION_NOT_FOUND, "Funkcja o nazwie {0} nie istnieje"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "Element {0} nie mo\u017ce mie\u0107 jednocze\u015bnie zawarto\u015bci i atrybutu select."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "Warto\u015bci\u0105 parametru {0} musi by\u0107 poprawny obiekt j\u0119zyka Java."},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "Atrybut result-prefix elementu xsl:namespace-alias ma warto\u015b\u0107 '#default', ale nie ma deklaracji domy\u015blnej przestrzeni nazw w zasi\u0119gu tego elementu."},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "Atrybut result-prefix elementu xsl:namespace-alias ma warto\u015b\u0107 ''{0}'', ale nie ma deklaracji przestrzeni nazw dla przedrostka ''{0}'' w zasi\u0119gu tego elementu."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "Nazwa opcji nie mo\u017ce mie\u0107 warto\u015bci null w TransformerFactory.setFeature(String nazwa, boolean warto\u015b\u0107)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "Nazwa opcji nie mo\u017ce mie\u0107 warto\u015bci null w TransformerFactory.getFeature(String nazwa)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "Nie mo\u017cna ustawi\u0107 opcji ''{0}'' w tej klasie TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "U\u017cycie elementu rozszerzenia ''{0}'' jest niedozwolone, gdy opcja przetwarzania bezpiecznego jest ustawiona na warto\u015b\u0107 true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Nie mo\u017cna pobra\u0107 przedrostka dla pustego identyfikatora uri przestrzeni nazw."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Nie mo\u017cna pobra\u0107 identyfikatora uri przestrzeni nazw dla pustego przedrostka."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "Nazwa funkcji nie mo\u017ce by\u0107 pusta (null)."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "Liczba parametr\u00f3w nie mo\u017ce by\u0107 ujemna."},
				new object[] {WG_FOUND_CURLYBRACE, "Znaleziono znak '}', ale nie jest otwarty \u017caden szablon atrybut\u00f3w!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Ostrze\u017cenie: Atrybut count nie jest zgodny ze swym przodkiem w xsl:number! Cel = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Stara sk\u0142adnia: Nazwa atrybutu 'expr' zosta\u0142a zmieniona na 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan nie obs\u0142uguje jeszcze nazwy ustawie\u0144 narodowych w funkcji format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Ostrze\u017cenie: Nie mo\u017cna znale\u017a\u0107 ustawie\u0144 narodowych dla xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Nie mo\u017cna utworzy\u0107 adresu URL z {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Nie mo\u017cna za\u0142adowa\u0107 \u017c\u0105danego dokumentu {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Nie mo\u017cna znale\u017a\u0107 procesu sortuj\u0105cego (Collator) dla <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Stara sk\u0142adnia: Instrukcja functions powinna u\u017cywa\u0107 adresu url {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "Kodowanie nieobs\u0142ugiwane: {0}, u\u017cywane jest UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "Kodowanie nieobs\u0142ugiwane: {0}, u\u017cywane jest Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Znaleziono konflikty specyfiki {0}, u\u017cywany b\u0119dzie ostatni znaleziony w arkuszu styl\u00f3w."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Analizowanie i przygotowywanie {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Szablon atrybutu {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Konflikt zgodno\u015bci pomi\u0119dzy xsl:strip-space oraz xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan nie obs\u0142uguje jeszcze atrybutu {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Nie znaleziono deklaracji formatu dziesi\u0119tnego {0}"},
				new object[] {WG_OLD_XSLT_NS, "Nieobecna lub niepoprawna przestrze\u0144 nazw XSLT."},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Dozwolona jest tylko jedna domy\u015blna deklaracja xsl:decimal-format."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "Nazwy xsl:decimal-format musz\u0105 by\u0107 unikalne. Nazwa \"{0}\" zosta\u0142a zduplikowana."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} ma niedozwolony atrybut {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Nie mo\u017cna przet\u0142umaczy\u0107 przedrostka przestrzeni nazw {0}. W\u0119ze\u0142 zostanie zignorowany."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet wymaga atrybutu 'version'!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Niedozwolona nazwa atrybutu {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Niedozwolona warto\u015b\u0107 atrybutu {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "Wynikaj\u0105cy z drugiego argumentu funkcji document zestaw w\u0119z\u0142\u00f3w jest pusty. Zwracany jest pusty zestaw w\u0119z\u0142\u00f3w."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Warto\u015bci\u0105 atrybutu 'name' nazwy xsl:processing-instruction nie mo\u017ce by\u0107 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Warto\u015bci\u0105 atrybutu ''name'' xsl:processing-instruction musi by\u0107 poprawna nazwa NCName: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Nie mo\u017cna doda\u0107 atrybutu {0} po w\u0119z\u0142ach potomnych ani przed wyprodukowaniem elementu.  Atrybut zostanie zignorowany."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Usi\u0142owano zmodyfikowa\u0107 obiekt, tam gdzie modyfikacje s\u0105 niedozwolone."},
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
				new object[] {"xslProc_option", "Opcje wiersza komend klasy Process Xalan-J:"},
				new object[] {"xslProc_option", "Opcje wiersza komend klasy Process Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "Opcja {0} jest nieobs\u0142ugiwana w trybie XSLTC."},
				new object[] {"xslProc_invalid_xalan_option", "Opcji {0} mo\u017cna u\u017cywa\u0107 tylko razem z -XSLTC."},
				new object[] {"xslProc_no_input", "B\u0142\u0105d: Nie podano arkusza styl\u00f3w lub wej\u015bciowego pliku xml. Wykonaj t\u0119 komend\u0119 bez \u017cadnych opcji, aby zapozna\u0107 si\u0119 z informacjami o sk\u0142adni."},
				new object[] {"xslProc_common_options", "-Wsp\u00f3lne opcje-"},
				new object[] {"xslProc_xalan_options", "-Opcje dla Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Opcje dla XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(naci\u015bnij klawisz <enter>, aby kontynuowa\u0107)"},
				new object[] {"optionXSLTC", "[-XSLTC (u\u017cycie XSLTC do transformacji)]"},
				new object[] {"optionIN", "[-IN wej\u015bciowyXMLURL]"},
				new object[] {"optionXSL", "[-XSL URLTransformacjiXSL]"},
				new object[] {"optionOUT", "[-OUT NazwaPlikuWyj\u015bciowego]"},
				new object[] {"optionLXCIN", "[-LXCIN NazwaWej\u015bciowegoPlikuSkompilowanegoArkuszaStyl\u00f3w]"},
				new object[] {"optionLXCOUT", "[-LXCOUT NazwaWyj\u015bciowegoPlikuSkompilowanegoArkuszaStyl\u00f3w]"},
				new object[] {"optionPARSER", "[-PARSER pe\u0142na nazwa klasy po\u0142\u0105czenia analizatora]"},
				new object[] {"optionE", "[-E (bez rozwijania odwo\u0142a\u0144 do encji)]"},
				new object[] {"optionV", "[-E (bez rozwijania odwo\u0142a\u0144 do encji)]"},
				new object[] {"optionQC", "[-QC (ciche ostrze\u017cenia o konfliktach wzorc\u00f3w)]"},
				new object[] {"optionQ", "[-Q  (tryb cichy)]"},
				new object[] {"optionLF", "[-LF (u\u017cycie tylko znak\u00f3w wysuwu wiersza na wyj\u015bciu {domy\u015blnie CR/LF})]"},
				new object[] {"optionCR", "[-LF (u\u017cycie tylko znak\u00f3w powrotu karetki na wyj\u015bciu {domy\u015blnie CR/LF})]"},
				new object[] {"optionESCAPE", "[-ESCAPE (znaki o zmienionym znaczeniu {domy\u015blne <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "[-INDENT (liczba znak\u00f3w wci\u0119cia {domy\u015blnie 0})]"},
				new object[] {"optionTT", "[-TT (\u015bledzenie szablon\u00f3w podczas ich wywo\u0142ywania)]"},
				new object[] {"optionTG", "[-TG (\u015bledzenie ka\u017cdego zdarzenia generowania)]"},
				new object[] {"optionTS", "[-TS (\u015bledzenie ka\u017cdego zdarzenia wyboru)]"},
				new object[] {"optionTTC", "[-TTC (\u015bledzenie szablon\u00f3w potomnych podczas ich przetwarzania)]"},
				new object[] {"optionTCLASS", "[-TCLASS (klasa TraceListener dla rozszerze\u0144 \u015bledzenia)]"},
				new object[] {"optionVALIDATE", "[-VALIDATE (w\u0142\u0105czenie sprawdzania poprawno\u015bci - domy\u015blnie jest wy\u0142\u0105czona)]"},
				new object[] {"optionEDUMP", "[-EDUMP {opcjonalna nazwa pliku} (wykonywanie zrzutu stosu w przypadku wyst\u0105pienia b\u0142\u0119du)]"},
				new object[] {"optionXML", "[-XML (u\u017cycie formatera XML i dodanie nag\u0142\u00f3wka XML)]"},
				new object[] {"optionTEXT", "[-TEXT (u\u017cycie prostego formatera tekstu)]"},
				new object[] {"optionHTML", "[-HTML (u\u017cycie formatera HTML)]"},
				new object[] {"optionPARAM", "[-PARAM nazwa wyra\u017cenie (ustawienie parametru arkusza styl\u00f3w)]"},
				new object[] {"noParsermsg1", "Proces XSL nie wykona\u0142 si\u0119 pomy\u015blnie."},
				new object[] {"noParsermsg2", "** Nie mo\u017cna znale\u017a\u0107 analizatora **"},
				new object[] {"noParsermsg3", "Sprawd\u017a classpath."},
				new object[] {"noParsermsg4", "Je\u015bli nie masz analizatora XML dla j\u0119zyka Java firmy IBM, mo\u017cesz go pobra\u0107"},
				new object[] {"noParsermsg5", "z serwisu AlphaWorks firmy IBM: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER pe\u0142na nazwa klasy (URIResolver u\u017cywany do t\u0142umaczenia URI)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER pe\u0142na nazwa klasy (EntityResolver u\u017cywany do t\u0142umaczenia encji)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER pe\u0142na nazwa klasy (ContentHandler u\u017cywany do szeregowania wyj\u015bcia)]"},
				new object[] {"optionLINENUMBERS", "    [-L u\u017cycie numer\u00f3w wierszy w dokumentach \u017ar\u00f3d\u0142owych]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (ustawienie opcji przetwarzania bezpiecznego na warto\u015b\u0107 true.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA typ_no\u015bnika (u\u017cywaj atrybutu media w celu znalezienia arkusza styl\u00f3w zwi\u0105zanego z dokumentem)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR nazwa_posmaku (u\u017cywaj jawnie s2s=SAX lub d2d=DOM w celu wykonania transformacji)]"},
				new object[] {"optionDIAG", "   [-DIAG (wy\u015bwietlenie ca\u0142kowitego czasu trwania transformacji)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (\u017c\u0105danie przyrostowego budowania DTM poprzez ustawienie http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (\u017c\u0105danie braku optymalizowania arkuszy styl\u00f3w poprzez ustawienie http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", "   [-RL limit_rekurencji (okre\u015blenie liczbowego limitu g\u0142\u0119boko\u015bci rekurencji w arkuszach styl\u00f3w)]"},
				new object[] {"optionXO", "[-XO [NazwaTransletu] (przypisanie nazwy wygenerowanemu transletowi)]"},
				new object[] {"optionXD", "[-XD KatalogDocelowy (okre\u015blenie katalogu docelowego dla transletu)]"},
				new object[] {"optionXJ", "[-XJ plik_jar (pakowanie klas transletu do pliku jar o nazwie <plik_jar>)]"},
				new object[] {"optionXP", "[-XP pakiet (okre\u015blenie przedrostka nazwy pakietu dla wszystkich wygenerowanych klas transletu)]"},
				new object[] {"optionXN", "[-XN (w\u0142\u0105czenie wstawiania szablon\u00f3w)]"},
				new object[] {"optionXX", "[-XX (w\u0142\u0105czenie dodatkowych diagnostycznych komunikat\u00f3w wyj\u015bciowych)]"},
				new object[] {"optionXT", "[-XT (u\u017cycie transletu do transformacji, je\u015bli to mo\u017cliwe)]"},
				new object[] {"diagTiming", "--------- Transformacja {0} przez {1} zaj\u0119\u0142a {2} ms"},
				new object[] {"recursionTooDeep", "Zbyt g\u0142\u0119bokie zagnie\u017cd\u017cenie szablon\u00f3w. zagnie\u017cd\u017cenie= {0}, szablon {1} {2}"},
				new object[] {"nameIs", "nazw\u0105 jest"},
				new object[] {"matchPatternIs", "wzorcem uzgadniania jest"}
			};
		  }
	  }
	  // ================= INFRASTRUCTURE ======================

	  /// <summary>
	  /// String for use when a bad error code was encountered. </summary>
	  public const string BAD_CODE = "BAD_CODE";

	  /// <summary>
	  /// String for use when formatting of the error string failed. </summary>
	  public const string FORMAT_FAILED = "FORMAT_FAILED";

	  /// <summary>
	  /// General error string. </summary>
	  public const string ERROR_STRING = "nr b\u0142\u0119du";

	  /// <summary>
	  /// String to prepend to error messages. </summary>
	  public const string ERROR_HEADER = "B\u0142\u0105d: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Ostrze\u017cenie: ";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public const string QUERY_HEADER = "WZORZEC ";


	  /// <summary>
	  ///   Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  ///   of ResourceBundle.getBundle().
	  /// </summary>
	  ///   <param name="className"> the name of the class that implements the resource bundle. </param>
	  ///   <returns> the ResourceBundle </returns>
	  ///   <exception cref="MissingResourceException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static final XSLTErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XSLTErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.getDefault();
		string suffix = getResourceSuffix(locale);

		try
		{

		  // first try with the given locale
		  return (XSLTErrorResources) ResourceBundle.getBundle(className + suffix, locale);
		}
		catch (MissingResourceException)
		{
		  try // try to fall back to en_US if we can't load
		  {

			// Since we can't find the localized property file,
			// fall back to en_US.
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("pl", "PL"));
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