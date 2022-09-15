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
 * $Id: XSLTErrorResources_zh.java 1225426 2011-12-29 04:13:08Z mrglavas $
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
	public class XSLTErrorResources_zh : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "\u9519\u8bef\uff1a\u8868\u8fbe\u5f0f\u4e2d\u4e0d\u80fd\u6709\u201c{\u201d"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} \u6709\u4e00\u4e2a\u975e\u6cd5\u5c5e\u6027\uff1a{1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode \u5728 xsl:apply-imports \u4e2d\u4e3a\u7a7a\uff01"},
				new object[] {ER_CANNOT_ADD, "\u65e0\u6cd5\u5c06\u201c{0}\u201d\u6dfb\u52a0\u5230\u201c{1}\u201d"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode \u5728 handleApplyTemplatesInstruction \u4e2d\u4e3a\u7a7a\uff01"},
				new object[] {ER_NO_NAME_ATTRIB, "\u201c{0}\u201d\u5fc5\u987b\u6709 name \u5c5e\u6027\u3002"},
				new object[] {ER_TEMPLATE_NOT_FOUND, "\u627e\u4e0d\u5230\u4ee5\u4e0b\u540d\u79f0\u7684\u6a21\u677f\uff1a{0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "\u65e0\u6cd5\u89e3\u6790 xsl:call-template \u4e2d\u7684\u540d\u79f0 AVT\u3002"},
				new object[] {ER_REQUIRES_ATTRIB, "\u201c{0}\u201d\u9700\u8981\u5c5e\u6027\uff1a{1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "\u201c{0}\u201d\u5fc5\u987b\u6709\u201ctest\u201d\u5c5e\u6027\u3002"},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "\u7ea7\u522b\u5c5e\u6027\u4e0a\u51fa\u73b0\u9519\u8bef\u503c\uff1a{0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "processing-instruction \u540d\u79f0\u4e0d\u80fd\u662f\u201cxml\u201d"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "processing-instruction \u540d\u79f0\u5fc5\u987b\u662f\u6709\u6548\u7684 NCName\uff1a{0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "\u201c{0}\u201d\u5982\u679c\u6709\u67d0\u79cd\u65b9\u5f0f\uff0c\u5c31\u5fc5\u987b\u6709 match \u5c5e\u6027\u3002"},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "\u201c{0}\u201d\u9700\u8981 name \u5c5e\u6027\u6216 match \u5c5e\u6027\u3002"},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "\u65e0\u6cd5\u89e3\u6790\u540d\u79f0\u7a7a\u95f4\u524d\u7f00\uff1a{0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space \u6709\u975e\u6cd5\u7684\u503c\uff1a{0}"},
				new object[] {ER_NO_OWNERDOC, "\u5b50\u8282\u70b9\u6ca1\u6709\u6240\u6709\u8005\u6587\u6863\uff01"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "ElemTemplateElement \u9519\u8bef\uff1a{0}"},
				new object[] {ER_NULL_CHILD, "\u6b63\u5728\u5c1d\u8bd5\u6dfb\u52a0\u7a7a\u7684\u5b50\u4ee3\uff01"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} \u9700\u8981 select \u5c5e\u6027\u3002"},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when \u5fc5\u987b\u6709\u201ctest\u201d\u5c5e\u6027\u3002"},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param \u5fc5\u987b\u6709\u201cname\u201d\u5c5e\u6027\u3002"},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "\u4e0a\u4e0b\u6587\u6ca1\u6709\u6240\u6709\u8005\u6587\u6863\uff01"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "\u65e0\u6cd5\u521b\u5efa XML TransformerFactory \u8054\u7cfb\uff1a{0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: Process \u4e0d\u6210\u529f\u3002"},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: \u4e0d\u6210\u529f\u3002"},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "\u4e0d\u652f\u6301\u7f16\u7801\uff1a{0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "\u65e0\u6cd5\u521b\u5efa TraceListener\uff1a{0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key \u9700\u8981\u201cname\u201d\u5c5e\u6027\uff01"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key \u9700\u8981\u201cmatch\u201d\u5c5e\u6027\uff01"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key \u9700\u8981\u201cuse\u201d\u5c5e\u6027\uff01"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} \u9700\u8981\u201celements\u201d\u5c5e\u6027\uff01"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} \u5c5e\u6027\u201cprefix\u201d\u4e22\u5931"},
				new object[] {ER_BAD_STYLESHEET_URL, "\u6837\u5f0f\u8868 URL \u9519\u8bef\uff1a{0}"},
				new object[] {ER_FILE_NOT_FOUND, "\u627e\u4e0d\u5230\u6837\u5f0f\u8868\u6587\u4ef6\uff1a{0}"},
				new object[] {ER_IOEXCEPTION, "\u6837\u5f0f\u8868\u6587\u4ef6\u53d1\u751f IO \u5f02\u5e38\uff1a{0}"},
				new object[] {ER_NO_HREF_ATTRIB, "\uff08StylesheetHandler\uff09\u65e0\u6cd5\u4e3a {0} \u627e\u5230 href \u5c5e\u6027"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "\uff08StylesheetHandler\uff09{0} \u6b63\u5728\u76f4\u63a5\u6216\u95f4\u63a5\u5730\u5305\u542b\u5b83\u81ea\u8eab\uff01"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "StylesheetHandler.processInclude \u9519\u8bef\uff0c{0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} \u5c5e\u6027\u201clang\u201d\u4e22\u5931"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "\uff08StylesheetHandler\uff09\u662f\u5426\u9519\u653e\u4e86 {0} \u5143\u7d20\uff1f\uff1f\u7f3a\u5c11\u5bb9\u5668\u5143\u7d20\u201ccomponent\u201d"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "\u53ea\u80fd\u8f93\u51fa\u5230 Element\u3001DocumentFragment\u3001Document \u6216 PrintWriter\u3002"},
				new object[] {ER_PROCESS_ERROR, "StylesheetRoot.process \u9519\u8bef"},
				new object[] {ER_UNIMPLNODE_ERROR, "UnImplNode \u9519\u8bef\uff1a{0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "\u9519\u8bef\uff01\u627e\u4e0d\u5230 xpath \u9009\u62e9\u8868\u8fbe\u5f0f\uff08-select\uff09\u3002"},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "\u65e0\u6cd5\u5e8f\u5217\u5316 XSLProcessor\uff01"},
				new object[] {ER_NO_INPUT_STYLESHEET, "\u6ca1\u6709\u6307\u5b9a\u6837\u5f0f\u8868\u8f93\u5165\uff01"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "\u65e0\u6cd5\u5904\u7406\u6837\u5f0f\u8868\uff01"},
				new object[] {ER_COULDNT_PARSE_DOC, "\u65e0\u6cd5\u89e3\u6790 {0} \u6587\u6863\uff01"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "\u627e\u4e0d\u5230\u7247\u6bb5\uff1a{0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "\u7247\u6bb5\u6807\u8bc6\u6307\u5411\u7684\u8282\u70b9\u4e0d\u662f\u5143\u7d20\uff1a{0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each \u5fc5\u987b\u6709 match \u6216 name \u5c5e\u6027"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "templates \u5fc5\u987b\u6709 match \u6216 name \u5c5e\u6027"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "\u65e0\u6587\u6863\u7247\u6bb5\u7684\u514b\u9686\uff01"},
				new object[] {ER_CANT_CREATE_ITEM, "\u65e0\u6cd5\u5728\u7ed3\u679c\u6811\u4e2d\u521b\u5efa\u9879\uff1a{0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "\u6e90 XML \u4e2d\u7684 xml:space \u6709\u975e\u6cd5\u503c\uff1a{0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "{0} \u6ca1\u6709 xsl:key \u58f0\u660e\uff01"},
				new object[] {ER_CANT_CREATE_URL, "\u9519\u8bef\uff01\u65e0\u6cd5\u4e3a {0} \u521b\u5efa URL"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "\u4e0d\u652f\u6301 xsl:functions"},
				new object[] {ER_PROCESSOR_ERROR, "XSLT TransformerFactory \u9519\u8bef"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "\uff08StylesheetHandler\uff09\u6837\u5f0f\u8868\u5185\u4e0d\u5141\u8bb8 {0}\uff01"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "\u4e0d\u518d\u652f\u6301 result-ns\uff01\u8bf7\u6539\u4e3a\u4f7f\u7528 xsl:output\u3002"},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "\u4e0d\u518d\u652f\u6301 default-space\uff01\u8bf7\u6539\u4e3a\u4f7f\u7528 xsl:strip-space \u6216 xsl:preserve-space\u3002"},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "\u4e0d\u518d\u652f\u6301 indent-result\uff01\u8bf7\u6539\u4e3a\u4f7f\u7528 xsl:output\u3002"},
				new object[] {ER_ILLEGAL_ATTRIB, "\uff08StylesheetHandler\uff09{0} \u6709\u975e\u6cd5\u5c5e\u6027\uff1a{1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "\u672a\u77e5 XSL \u5143\u7d20\uff1a{0}"},
				new object[] {ER_BAD_XSLSORT_USE, "\uff08StylesheetHandler\uff09xsl:sort \u53ea\u80fd\u4e0e xsl:apply-templates \u6216 xsl:for-each \u4e00\u8d77\u4f7f\u7528\u3002"},
				new object[] {ER_MISPLACED_XSLWHEN, "\uff08StylesheetHandler\uff09\u9519\u653e\u4e86 xsl:when\uff01"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "\uff08StylesheetHandler\uff09xsl:choose \u4e0d\u662f xsl:when \u7684\u7236\u4ee3\uff01"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "\uff08StylesheetHandler\uff09\u9519\u653e\u4e86 xsl:otherwise\uff01"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "\uff08StylesheetHandler\uff09xsl:choose \u4e0d\u662f xsl:otherwise \u7684\u7236\u4ee3\uff01"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "\uff08StylesheetHandler\uff09\u6a21\u677f\u5185\u4e0d\u5141\u8bb8\u51fa\u73b0 {0}\uff01"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "\uff08StylesheetHandler\uff09{0} \u6269\u5c55\u540d\u79f0\u7a7a\u95f4\u524d\u7f00 {1} \u672a\u77e5"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "\uff08StylesheetHandler\uff09\u5bfc\u5165\u53ea\u80fd\u4f5c\u4e3a\u6837\u5f0f\u8868\u4e2d\u6700\u524d\u9762\u7684\u5143\u7d20\u53d1\u751f\uff01"},
				new object[] {ER_IMPORTING_ITSELF, "\uff08StylesheetHandler\uff09{0} \u6b63\u5728\u76f4\u63a5\u6216\u95f4\u63a5\u5730\u5bfc\u5165\u5b83\u81ea\u8eab\uff01"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "\uff08StylesheetHandler\uff09xml:space \u6709\u975e\u6cd5\u503c\uff1a{0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet \u4e0d\u6210\u529f\uff01"},
				new object[] {ER_SAX_EXCEPTION, "SAX \u5f02\u5e38"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "\u51fd\u6570\u4e0d\u53d7\u652f\u6301\uff01"},
				new object[] {ER_XSLT_ERROR, "XSLT \u9519\u8bef"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "\u683c\u5f0f\u6a21\u5f0f\u5b57\u7b26\u4e32\u4e2d\u4e0d\u5141\u8bb8\u5b58\u5728\u8d27\u5e01\u7b26\u53f7"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "\u6837\u5f0f\u8868 DOM \u4e2d\u4e0d\u652f\u6301\u6587\u6863\u51fd\u6570\uff01"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "\u65e0\u6cd5\u89e3\u6790\u975e\u524d\u7f00\u89e3\u6790\u5668\u7684\u524d\u7f00\uff01"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "\u91cd\u5b9a\u5411\u6269\u5c55\uff1a\u65e0\u6cd5\u83b7\u53d6\u6587\u4ef6\u540d \uff0d file \u6216 select \u5c5e\u6027\u5fc5\u987b\u8fd4\u56de\u6709\u6548\u5b57\u7b26\u4e32\u3002"},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "\u65e0\u6cd5\u5728\u91cd\u5b9a\u5411\u6269\u5c55\u4e2d\u6784\u5efa FormatterListener\uff01"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "exclude-result-prefixes \u4e2d\u7684\u524d\u7f00\u65e0\u6548\uff1a{0}"},
				new object[] {ER_MISSING_NS_URI, "\u6307\u5b9a\u7684\u524d\u7f00\u7f3a\u5c11\u540d\u79f0\u7a7a\u95f4 URI"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "\u9009\u9879 {0} \u7f3a\u5c11\u81ea\u53d8\u91cf"},
				new object[] {ER_INVALID_OPTION, "\u9009\u9879 {0} \u65e0\u6548"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "\u683c\u5f0f\u5b57\u7b26\u4e32 {0} \u7684\u683c\u5f0f\u9519\u8bef"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet \u9700\u8981\u201cversion\u201d\u5c5e\u6027\uff01"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "\u5c5e\u6027\uff1a{0} \u6709\u975e\u6cd5\u7684\u503c\uff1a{1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose \u9700\u8981 xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:for-each \u4e2d\u4e0d\u5141\u8bb8\u6709 xsl:apply-imports"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "\u65e0\u6cd5\u5c06 DTMLiaison \u7528\u4e8e\u8f93\u51fa DOM \u8282\u70b9... \u6539\u4e3a\u4f20\u9012 org.apache.xpath.DOM2Helper\uff01"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "\u65e0\u6cd5\u5c06 DTMLiaison \u7528\u4e8e\u8f93\u5165 DOM \u8282\u70b9... \u6539\u4e3a\u4f20\u9012 org.apache.xpath.DOM2Helper\uff01"},
				new object[] {ER_CALL_TO_EXT_FAILED, "\u8c03\u7528\u6269\u5c55\u5143\u7d20\u5931\u8d25\uff1a{0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u524d\u7f00\u5fc5\u987b\u89e3\u6790\u4e3a\u540d\u79f0\u7a7a\u95f4\uff1a{0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u68c0\u6d4b\u5230\u65e0\u6548\u7684 UTF-16 \u8d85\u5927\u5b57\u7b26\u96c6\uff1a{0}\uff1f"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} \u4f7f\u7528\u4e86\u81ea\u8eab\uff0c\u8fd9\u5c06\u5bfc\u81f4\u65e0\u9650\u5faa\u73af\u3002"},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "\u65e0\u6cd5\u5c06\u975e Xerces-DOM \u8f93\u5165\u4e0e Xerces-DOM \u8f93\u51fa\u6df7\u5408\uff01"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet \u2015 TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "\u5728 ElemTemplateElement.readObject \u4e2d\uff1a{0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "\u627e\u5230\u591a\u4e2a\u540d\u4e3a {0} \u7684\u6a21\u677f"},
				new object[] {ER_INVALID_KEY_CALL, "\u65e0\u6548\u7684\u51fd\u6570\u8c03\u7528\uff1a\u4e0d\u5141\u8bb8\u9012\u5f52 key() \u8c03\u7528"},
				new object[] {ER_REFERENCING_ITSELF, "\u53d8\u91cf {0} \u6b63\u5728\u76f4\u63a5\u6216\u95f4\u63a5\u5730\u5f15\u7528\u5b83\u81ea\u8eab\uff01"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "\u8f93\u5165\u8282\u70b9\u5bf9\u4e8e newTemplates \u7684 DOMSource \u4e0d\u80fd\u4e3a\u7a7a\uff01"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "\u627e\u4e0d\u5230\u9009\u9879 {0} \u7684\u7c7b\u6587\u4ef6"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "\u627e\u4e0d\u5230\u5fc5\u9700\u7684\u5143\u7d20\uff1a{0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream \u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI \u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "File \u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource \u4e0d\u80fd\u4e3a\u7a7a"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "\u65e0\u6cd5\u521d\u59cb\u5316 BSF \u7ba1\u7406\u5668"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "\u65e0\u6cd5\u7f16\u8bd1\u6269\u5c55"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "\u7531\u4e8e {1}\uff0c\u65e0\u6cd5\u521b\u5efa\u6269\u5c55 {0}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "\u5bf9\u65b9\u6cd5 {0} \u7684\u5b9e\u4f8b\u65b9\u6cd5\u8c03\u7528\u8981\u6c42\u4ee5\u5bf9\u8c61\u5b9e\u4f8b\u4f5c\u4e3a\u7b2c\u4e00\u53c2\u6570"},
				new object[] {ER_INVALID_ELEMENT_NAME, "\u6307\u5b9a\u4e86\u65e0\u6548\u7684\u5143\u7d20\u540d\u79f0 {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "\u5143\u7d20\u540d\u79f0\u65b9\u6cd5\u5fc5\u987b\u662f static {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "\u6269\u5c55\u51fd\u6570 {0} : {1} \u672a\u77e5"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "\u5bf9\u4e8e {0}\uff0c\u6784\u9020\u51fd\u6570\u6709\u591a\u4e2a\u6700\u4f73\u5339\u914d"},
				new object[] {ER_MORE_MATCH_METHOD, "\u65b9\u6cd5 {0} \u6709\u591a\u4e2a\u6700\u4f73\u5339\u914d"},
				new object[] {ER_MORE_MATCH_ELEMENT, "element \u65b9\u6cd5 {0} \u6709\u591a\u4e2a\u6700\u4f73\u5339\u914d"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "\u8bc4\u4f30 {0} \u65f6\u4f20\u9012\u4e86\u65e0\u6548\u7684\u4e0a\u4e0b\u6587"},
				new object[] {ER_POOL_EXISTS, "\u6c60\u5df2\u7ecf\u5b58\u5728"},
				new object[] {ER_NO_DRIVER_NAME, "\u672a\u6307\u5b9a\u9a71\u52a8\u7a0b\u5e8f\u540d\u79f0"},
				new object[] {ER_NO_URL, "\u672a\u6307\u5b9a URL"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "\u6c60\u5927\u5c0f\u5c0f\u4e8e 1\uff01"},
				new object[] {ER_INVALID_DRIVER, "\u6307\u5b9a\u4e86\u65e0\u6548\u7684\u9a71\u52a8\u7a0b\u5e8f\u540d\u79f0\uff01"},
				new object[] {ER_NO_STYLESHEETROOT, "\u627e\u4e0d\u5230\u6837\u5f0f\u8868\u6839\u76ee\u5f55\uff01"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "xml:space \u7684\u503c\u975e\u6cd5"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode \u5931\u8d25"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "\u8d44\u6e90 [ {0} ] \u65e0\u6cd5\u88c5\u5165\uff1a{1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "\u7f13\u51b2\u533a\u5927\u5c0f <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "\u8c03\u7528\u6269\u5c55\u65f6\u53d1\u751f\u672a\u77e5\u9519\u8bef"},
				new object[] {ER_NO_NAMESPACE_DECL, "\u524d\u7f00 {0} \u6ca1\u6709\u76f8\u5e94\u7684\u540d\u79f0\u7a7a\u95f4\u58f0\u660e"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "lang=javaclass {0} \u4e0d\u5141\u8bb8\u51fa\u73b0\u5143\u7d20\u5185\u5bb9"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "\u6837\u5f0f\u8868\u5b9a\u5411\u7684\u7ec8\u6b62"},
				new object[] {ER_ONE_OR_TWO, "1 \u6216 2"},
				new object[] {ER_TWO_OR_THREE, "2 \u6216 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "\u65e0\u6cd5\u88c5\u5165 {0}\uff08\u68c0\u67e5 CLASSPATH\uff09\uff0c\u73b0\u5728\u53ea\u4f7f\u7528\u7f3a\u7701\u503c"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "\u65e0\u6cd5\u521d\u59cb\u5316\u7f3a\u7701\u6a21\u677f"},
				new object[] {ER_RESULT_NULL, "\u7ed3\u679c\u4e0d\u5e94\u4e3a\u7a7a"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "\u65e0\u6cd5\u8bbe\u7f6e\u7ed3\u679c"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "\u672a\u6307\u5b9a\u8f93\u51fa"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "\u65e0\u6cd5\u8f6c\u6362\u6210\u7c7b\u578b\u4e3a {0} \u7684\u7ed3\u679c"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "\u65e0\u6cd5\u8f6c\u6362\u7c7b\u578b\u4e3a {0} \u7684\u6e90"},
				new object[] {ER_NULL_CONTENT_HANDLER, "\u5185\u5bb9\u5904\u7406\u7a0b\u5e8f\u4e3a\u7a7a"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u9519\u8bef\u5904\u7406\u7a0b\u5e8f\u4e3a\u7a7a"},
				new object[] {ER_CANNOT_CALL_PARSE, "\u5982\u679c\u6ca1\u6709\u8bbe\u7f6e ContentHandler\uff0c\u5219\u65e0\u6cd5\u8c03\u7528\u89e3\u6790"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "\u8fc7\u6ee4\u5668\u65e0\u7236\u4ee3"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "\u5728 {0} \u4e2d\u627e\u4e0d\u5230\u6837\u5f0f\u8868\uff0c\u4ecb\u8d28 = {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "\u5728 {0} \u4e2d\u627e\u4e0d\u5230 xml-stylesheet PI"},
				new object[] {ER_NOT_SUPPORTED, "\u4e0d\u652f\u6301\uff1a{0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "\u7279\u6027 {0} \u7684\u503c\u5e94\u5f53\u662f\u5e03\u5c14\u5b9e\u4f8b"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "\u627e\u4e0d\u5230 {0} \u4e0a\u7684\u5916\u90e8\u811a\u672c"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "\u627e\u4e0d\u5230\u8d44\u6e90 [ {0} ]\u3002\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "\u672a\u8bc6\u522b\u51fa\u8f93\u51fa\u5c5e\u6027\uff1a{0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "\u521b\u5efa ElemLiteralResult \u5b9e\u4f8b\u5931\u8d25"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "{0} \u7684\u503c\u5e94\u5f53\u5305\u542b\u53ef\u8fdb\u884c\u89e3\u6790\u7684\u6570\u5b57"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "{0} \u7684\u503c\u5e94\u5f53\u7b49\u4e8e yes \u6216 no"},
				new object[] {ER_FAILED_CALLING_METHOD, "\u8c03\u7528 {0} \u65b9\u6cd5\u5931\u8d25"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "\u521b\u5efa ElemTemplateElement \u5b9e\u4f8b\u5931\u8d25"},
				new object[] {ER_CHARS_NOT_ALLOWED, "\u6b64\u65f6\u6587\u6863\u4e2d\u4e0d\u5141\u8bb8\u6709\u5b57\u7b26"},
				new object[] {ER_ATTR_NOT_ALLOWED, "{1} \u5143\u7d20\u4e0a\u4e0d\u5141\u8bb8\u4f7f\u7528\u201c{0}\u201d\u5c5e\u6027\uff01"},
				new object[] {ER_BAD_VALUE, "{0} \u9519\u8bef\u503c {1}"},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "\u627e\u4e0d\u5230 {0} \u5c5e\u6027\u503c"},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "\u672a\u8bc6\u522b\u51fa {0} \u5c5e\u6027\u503c"},
				new object[] {ER_NULL_URI_NAMESPACE, "\u6b63\u5728\u8bd5\u56fe\u4ee5\u7a7a\u7684 URI \u751f\u6210\u540d\u79f0\u7a7a\u95f4\u524d\u7f00"},
				new object[] {ER_NUMBER_TOO_BIG, "\u6b63\u5728\u8bd5\u56fe\u683c\u5f0f\u5316\u5927\u4e8e\u6700\u5927\u957f\u6574\u6570\u7684\u6570\u5b57"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "\u627e\u4e0d\u5230 SAX1 \u9a71\u52a8\u7a0b\u5e8f\u7c7b {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "\u627e\u5230\u4e86 SAX1 \u9a71\u52a8\u7a0b\u5e8f\u7c7b {0}\uff0c\u4f46\u65e0\u6cd5\u88c5\u5165\u5b83"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "\u88c5\u5165\u4e86 SAX1 \u9a71\u52a8\u7a0b\u5e8f\u7c7b {0}\uff0c\u4f46\u65e0\u6cd5\u5c06\u5b83\u5b9e\u4f8b\u5316"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "SAX1 \u9a71\u52a8\u7a0b\u5e8f\u7c7b {0} \u4e0d\u5b9e\u73b0 org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "\u6ca1\u6709\u6307\u5b9a\u7cfb\u7edf\u5c5e\u6027 org.xml.sax.parser"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "\u89e3\u6790\u5668\u53c2\u6570\u4e0d\u5f97\u4e3a\u7a7a"},
				new object[] {ER_FEATURE, "\u7279\u5f81\uff1a{0}"},
				new object[] {ER_PROPERTY, "\u5c5e\u6027\uff1a{0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "\u5b9e\u4f53\u89e3\u6790\u5668\u4e3a\u7a7a"},
				new object[] {ER_NULL_DTD_HANDLER, "DTD \u5904\u7406\u7a0b\u5e8f\u4e3a\u7a7a"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "\u672a\u6307\u5b9a\u9a71\u52a8\u7a0b\u5e8f\u540d\u79f0\uff01"},
				new object[] {ER_NO_URL_SPECIFIED, "\u672a\u6307\u5b9a URL\uff01"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "\u6c60\u5927\u5c0f\u5c0f\u4e8e 1\uff01"},
				new object[] {ER_INVALID_DRIVER_NAME, "\u6307\u5b9a\u4e86\u65e0\u6548\u7684\u9a71\u52a8\u7a0b\u5e8f\u540d\u79f0\uff01"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "\u7a0b\u5e8f\u5458\u7684\u9519\u8bef\uff01\u8868\u8fbe\u5f0f\u6ca1\u6709 ElemTemplateElement \u7236\u4ee3\uff01"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "\u7a0b\u5e8f\u5458\u5728 RedundentExprEliminator \u4e2d\u7684\u65ad\u8a00\uff1a{0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "\u6837\u5f0f\u8868\u7684\u6b64\u4f4d\u7f6e\u4e2d\u4e0d\u5141\u8bb8\u6709 {0}\uff01"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "\u6837\u5f0f\u8868\u7684\u6b64\u4f4d\u7f6e\u4e2d\u4e0d\u5141\u8bb8\u6709\u975e\u7a7a\u683c\u7684\u6587\u672c\uff01"},
				new object[] {INVALID_TCHAR, "\u7528\u4e8e CHAR \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5\u3002\u7c7b\u578b CHAR \u7684\u5c5e\u6027\u5fc5\u987b\u53ea\u6709 1 \u4e2a\u5b57\u7b26\uff01"},
				new object[] {INVALID_QNAME, "\u7528\u4e8e QNAME \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5"},
				new object[] {INVALID_ENUM, "\u7528\u4e8e ENUM \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5\u3002\u6709\u6548\u7684\u503c\u662f\uff1a{2}\u3002"},
				new object[] {INVALID_NMTOKEN, "\u7528\u4e8e NMTOKEN \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5"},
				new object[] {INVALID_NCNAME, "\u7528\u4e8e NCNAME \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5"},
				new object[] {INVALID_BOOLEAN, "\u7528\u4e8e boolean \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5"},
				new object[] {INVALID_NUMBER, "\u7528\u4e8e number \u5c5e\u6027 {0} \u7684\u503c {1} \u975e\u6cd5"},
				new object[] {ER_ARG_LITERAL, "\u5339\u914d\u6a21\u5f0f\u4e2d {0} \u7684\u81ea\u53d8\u91cf\u5fc5\u987b\u662f\u6587\u5b57\u3002"},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "\u5168\u5c40\u53d8\u91cf\u7684\u58f0\u660e\u91cd\u590d\u3002"},
				new object[] {ER_DUPLICATE_VAR, "\u53d8\u91cf\u58f0\u660e\u91cd\u590d\u3002"},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template \u5fc5\u987b\u6709\u4e00\u4e2a name \u6216 match \u5c5e\u6027\uff08\u6216\u4e24\u8005\u517c\u6709\uff09"},
				new object[] {ER_INVALID_PREFIX, "exclude-result-prefixes \u4e2d\u7684\u524d\u7f00\u65e0\u6548\uff1a{0}"},
				new object[] {ER_NO_ATTRIB_SET, "\u540d\u4e3a {0} \u7684\u5c5e\u6027\u96c6\u4e0d\u5b58\u5728"},
				new object[] {ER_FUNCTION_NOT_FOUND, "\u540d\u4e3a {0} \u7684\u51fd\u6570\u4e0d\u5b58\u5728"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "{0} \u5143\u7d20\u4e0d\u5f97\u540c\u65f6\u5177\u6709\u5185\u5bb9\u548c select \u5c5e\u6027\u3002"},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "\u53c2\u6570 {0} \u7684\u503c\u5fc5\u987b\u4e3a\u6709\u6548\u7684 Java \u5bf9\u8c61"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "xsl:namespace-alias \u5143\u7d20\u7684 result-prefix \u5c5e\u6027\u542b\u6709\u201c#default\u201d\u503c\uff0c\u4f46\u5728\u8be5\u5143\u7d20\u7684\u4f5c\u7528\u57df\u4e2d\u6ca1\u6709\u7f3a\u7701\u540d\u79f0\u7a7a\u95f4\u7684\u58f0\u660e\u3002"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "xsl:namespace-alias \u5143\u7d20\u7684 result-prefix \u5c5e\u6027\u542b\u6709\u201c{0}\u201d\u503c\uff0c\u4f46\u662f\u5728\u8be5\u5143\u7d20\u7684\u4f5c\u7528\u57df\u4e2d\u6ca1\u6709\u524d\u7f00\u201c{0}\u201d\u7684\u540d\u79f0\u7a7a\u95f4\u58f0\u660e\u3002"},
				new object[] {ER_SET_FEATURE_NULL_NAME, "\u5728 TransformerFactory.setFeature(String name, boolean value) \u4e2d\u7279\u5f81\u540d\u4e0d\u80fd\u4e3a\u7a7a\u3002"},
				new object[] {ER_GET_FEATURE_NULL_NAME, "\u5728 TransformerFactory.getFeature(String name) \u4e2d\u7279\u5f81\u540d\u4e0d\u80fd\u4e3a\u7a7a\u3002"},
				new object[] {ER_UNSUPPORTED_FEATURE, "\u65e0\u6cd5\u5bf9\u6b64 TransformerFactory \u8bbe\u7f6e\u7279\u5f81\u201c{0}\u201d\u3002"},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "\u5f53\u5b89\u5168\u5904\u7406\u529f\u80fd\u8bbe\u7f6e\u4e3a true \u65f6\uff0c\u4e0d\u5141\u8bb8\u4f7f\u7528\u6269\u5c55\u5143\u7d20\u201c{0}\u201d\u3002"},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "\u65e0\u6cd5\u4e3a\u7a7a\u540d\u79f0\u7a7a\u95f4 uri \u83b7\u53d6\u524d\u7f00\u3002"},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "\u65e0\u6cd5\u4e3a\u7a7a\u524d\u7f00\u83b7\u53d6\u540d\u79f0\u7a7a\u95f4 uri\u3002"},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "\u51fd\u6570\u540d\u4e0d\u80fd\u4e3a\u7a7a\u3002"},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "\u6570\u91cf\u4e0d\u80fd\u4e3a\u8d1f\u3002"},
				new object[] {WG_FOUND_CURLYBRACE, "\u627e\u5230\u201c}\u201d\uff0c\u4f46\u6ca1\u6709\u6253\u5f00\u5c5e\u6027\u6a21\u677f\uff01"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "\u8b66\u544a\uff1acount \u5c5e\u6027\u4e0e xsl:number \u4e2d\u7684\u4e0a\u7ea7\u4e0d\u5339\u914d\uff01\u76ee\u6807 = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "\u65e7\u8bed\u6cd5\uff1a\u201cexpr\u201d\u5c5e\u6027\u7684\u540d\u79f0\u5df2\u7ecf\u66f4\u6539\u4e3a\u201cselect\u201d\u3002"},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan \u5728 format-number \u51fd\u6570\u4e2d\u5c1a\u672a\u5904\u7406\u8bed\u8a00\u73af\u5883\u540d\u3002"},
				new object[] {WG_LOCALE_NOT_FOUND, "\u8b66\u544a\uff1a\u627e\u4e0d\u5230 xml:lang={0} \u7684\u8bed\u8a00\u73af\u5883"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "\u65e0\u6cd5\u4ece {0} \u751f\u6210 URL"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "\u65e0\u6cd5\u88c5\u5165\u8bf7\u6c42\u7684\u6587\u6863\uff1a{0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "\u627e\u4e0d\u5230 <sort xml:lang={0} \u7684\u6574\u7406\u5668"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "\u65e7\u8bed\u6cd5\uff1a\u51fd\u6570\u6307\u4ee4\u5e94\u5f53\u4f7f\u7528 {0} \u7684 URL"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "\u4e0d\u652f\u6301\u7f16\u7801\uff1a{0}\uff0c\u6b63\u5728\u4f7f\u7528 UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "\u4e0d\u652f\u6301\u7f16\u7801\uff1a{0}\uff0c\u6b63\u5728\u4f7f\u7528 Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "\u53d1\u73b0\u7279\u6027\u51b2\u7a81\uff1a\u5c06\u4f7f\u7528\u6837\u5f0f\u8868\u4e2d\u6700\u540e\u627e\u5230\u7684 {0}\u3002"},
				new object[] {WG_PARSING_AND_PREPARING, "========= \u89e3\u6790\u548c\u51c6\u5907 {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "\u5c5e\u6027\u6a21\u677f\uff0c{0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "xsl:strip-space \u548c xsl:preserve-space \u4e4b\u95f4\u7684\u5339\u914d\u51b2\u7a81"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan \u5c1a\u672a\u5904\u7406 {0} \u5c5e\u6027\uff01"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "\u627e\u4e0d\u5230\u5341\u8fdb\u5236\u683c\u5f0f\u7684\u58f0\u660e\uff1a{0}"},
				new object[] {WG_OLD_XSLT_NS, "XSLT \u540d\u79f0\u7a7a\u95f4\u4e22\u5931\u6216\u4e0d\u6b63\u786e\u3002"},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "\u53ea\u5141\u8bb8\u4e00\u4e2a\u7f3a\u7701\u7684 xsl:decimal-format \u58f0\u660e\u3002"},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "xsl:decimal-format \u540d\u79f0\u5fc5\u987b\u662f\u552f\u4e00\u7684\u3002\u540d\u79f0\u201c{0}\u201d\u6709\u91cd\u590d\u3002"},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} \u6709\u4e00\u4e2a\u975e\u6cd5\u5c5e\u6027\uff1a{1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "\u65e0\u6cd5\u89e3\u6790\u540d\u79f0\u7a7a\u95f4\u524d\u7f00\uff1a{0}\u3002\u5c06\u5ffd\u7565\u8be5\u8282\u70b9\u3002"},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet \u9700\u8981\u201cversion\u201d\u5c5e\u6027\uff01"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "\u975e\u6cd5\u5c5e\u6027\u540d\u79f0\uff1a{0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "\u7528\u4e8e\u5c5e\u6027 {0} \u7684\u503c\u975e\u6cd5\uff1a{1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "\u4ece\u6587\u6863\u51fd\u6570\u7684\u7b2c\u4e8c\u53c2\u6570\u4ea7\u751f\u7684\u8282\u70b9\u96c6\u662f\u7a7a\u7684\u3002\u8fd4\u56de\u4e00\u4e2a\u7a7a\u8282\u70b9\u96c6\u3002"},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "xsl:processing-instruction \u540d\u79f0\u7684\u201cname\u201d\u5c5e\u6027\u7684\u503c\u4e0d\u5f97\u4e3a\u201cxml\u201d"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "xsl:processing-instruction \u7684\u201cname\u201d\u5c5e\u6027\u7684\u503c\u5fc5\u987b\u662f\u6709\u6548\u7684 NCName\uff1a{0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "\u5728\u751f\u6210\u5b50\u8282\u70b9\u4e4b\u540e\u6216\u5728\u751f\u6210\u5143\u7d20\u4e4b\u524d\u65e0\u6cd5\u6dfb\u52a0\u5c5e\u6027 {0}\u3002\u5c06\u5ffd\u7565\u5c5e\u6027\u3002"},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "\u8bd5\u56fe\u4fee\u6539\u4e0d\u5141\u8bb8\u4fee\u6539\u7684\u5bf9\u8c61\u3002"},
				new object[] {"ui_language", "zh"},
				new object[] {"help_language", "zh"},
				new object[] {"language", "zh"},
				new object[] {"BAD_CODE", "createMessage \u7684\u53c2\u6570\u8d85\u51fa\u8303\u56f4"},
				new object[] {"FORMAT_FAILED", "\u5728 messageFormat \u8c03\u7528\u8fc7\u7a0b\u4e2d\u629b\u51fa\u4e86\u5f02\u5e38"},
				new object[] {"version", ">>>>>>> Xalan \u7248\u672c"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u662f"},
				new object[] {"line", "\u884c\u53f7"},
				new object[] {"column", "\u5217\u53f7"},
				new object[] {"xsldone", "XSLProcessor\uff1a\u5b8c\u6210"},
				new object[] {"xslProc_option", "Xalan-J \u547d\u4ee4\u884c Process \u7c7b\u9009\u9879\uff1a"},
				new object[] {"xslProc_option", "Xalan-J \u547d\u4ee4\u884c Process \u7c7b\u9009\u9879\uff1a"},
				new object[] {"xslProc_invalid_xsltc_option", "\u5728 XSLTC \u65b9\u5f0f\u4e2d\uff0c\u4e0d\u652f\u6301\u9009\u9879 {0}\u3002"},
				new object[] {"xslProc_invalid_xalan_option", "\u9009\u9879 {0} \u53ea\u80fd\u4e0e -XSLTC \u4e00\u8d77\u4f7f\u7528\u3002"},
				new object[] {"xslProc_no_input", "\u9519\u8bef\uff1a\u6ca1\u6709\u6307\u5b9a\u6837\u5f0f\u8868\u6216\u8f93\u5165 xml\u3002\u4e0d\u5e26\u4efb\u4f55\u9009\u9879\u8fd0\u884c\u6b64\u547d\u4ee4\uff0c\u4ee5\u4e86\u89e3\u4f7f\u7528\u8bf4\u660e\u3002"},
				new object[] {"xslProc_common_options", "\uff0d \u5e38\u7528\u9009\u9879 \uff0d"},
				new object[] {"xslProc_xalan_options", "\u2015 Xalan \u9009\u9879 \u2015"},
				new object[] {"xslProc_xsltc_options", "\u2015 XSLTC \u9009\u9879 \u2015"},
				new object[] {"xslProc_return_to_continue", "\uff08\u8bf7\u6309 <return> \u952e\u7ee7\u7eed\uff09"},
				new object[] {"optionXSLTC", "   [-XSLTC \uff08\u4f7f\u7528 XSLTC \u8f6c\u6362\uff09]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "[-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "[-OUT outputFileName]"},
				new object[] {"optionLXCIN", "[-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "[-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER fully qualified class name of parser liaison]"},
				new object[] {"optionE", "[-E \uff08\u4e0d\u8981\u5c55\u5f00\u5b9e\u4f53\u5f15\u7528\uff09]"},
				new object[] {"optionV", "[-E \uff08\u4e0d\u8981\u5c55\u5f00\u5b9e\u4f53\u5f15\u7528\uff09]"},
				new object[] {"optionQC", "[-QC \uff08\u9759\u9ed8\u6a21\u5f0f\u51b2\u7a81\u8b66\u544a\uff09]"},
				new object[] {"optionQ", "[-Q \uff08\u9759\u9ed8\u65b9\u5f0f\uff09]"},
				new object[] {"optionLF", "[-LF \uff08\u4ec5\u5728\u8f93\u51fa\u65f6\u4f7f\u7528\u6362\u884c {\u7f3a\u7701\u503c\u662f CR/LF}\uff09]"},
				new object[] {"optionCR", "[-CR \uff08\u4ec5\u5728\u8f93\u51fa\u65f6\u4f7f\u7528\u56de\u8f66\u7b26 {\u7f3a\u7701\u503c\u662f CR/LF}\uff09]"},
				new object[] {"optionESCAPE", "[-ESCAPE \uff08\u8bbe\u7f6e\u8f6c\u4e49\u5b57\u7b26 {\u7f3a\u7701\u503c\u662f <>&\"\'\\r\\n}\uff09]"},
				new object[] {"optionINDENT", "[-INDENT \uff08\u63a7\u5236\u7f29\u8fdb\u591a\u5c11\u7a7a\u683c {\u7f3a\u7701\u503c\u662f 0}\uff09]"},
				new object[] {"optionTT", "[-TT \uff08\u5728\u6a21\u677f\u88ab\u8c03\u7528\u65f6\u8ddf\u8e2a\u6a21\u677f\u3002\uff09]"},
				new object[] {"optionTG", "[-TG \uff08\u8ddf\u8e2a\u6bcf\u4e00\u4e2a\u751f\u6210\u4e8b\u4ef6\u3002\uff09]"},
				new object[] {"optionTS", "[-TS \uff08\u8ddf\u8e2a\u6bcf\u4e00\u4e2a\u9009\u62e9\u4e8b\u4ef6\u3002\uff09]"},
				new object[] {"optionTTC", "[-TTC \uff08\u5728\u5b50\u6a21\u677f\u88ab\u5904\u7406\u65f6\u5bf9\u5176\u8fdb\u884c\u8ddf\u8e2a\u3002\uff09]"},
				new object[] {"optionTCLASS", "[-TCLASS \uff08\u8ddf\u8e2a\u6269\u5c55\u7684 TraceListener \u7c7b\u3002\uff09]"},
				new object[] {"optionVALIDATE", "[-VALIDATE \uff08\u8bbe\u7f6e\u662f\u5426\u8fdb\u884c\u9a8c\u8bc1\u3002\u7f3a\u7701\u65f6\u9a8c\u8bc1\u662f\u5173\u95ed\u7684\u3002\uff09]"},
				new object[] {"optionEDUMP", "[-EDUMP {\u53ef\u9009\u6587\u4ef6\u540d} \uff08\u53d1\u751f\u9519\u8bef\u65f6\u5806\u6808\u8f6c\u50a8\u3002\uff09]"},
				new object[] {"optionXML", "[-XML \uff08\u4f7f\u7528 XML \u683c\u5f0f\u5316\u7a0b\u5e8f\u5e76\u6dfb\u52a0 XML \u5934\u3002\uff09]"},
				new object[] {"optionTEXT", "[-TEXT \uff08\u4f7f\u7528\u7b80\u5355\u6587\u672c\u683c\u5f0f\u5316\u7a0b\u5e8f\u3002\uff09]"},
				new object[] {"optionHTML", "[-HTML \uff08\u4f7f\u7528 HTML \u683c\u5f0f\u5316\u7a0b\u5e8f\uff09]"},
				new object[] {"optionPARAM", "[-PARAM name expression \uff08\u8bbe\u7f6e\u6837\u8bc6\u8868\u53c2\u6570\uff09]"},
				new object[] {"noParsermsg1", "XSL \u5904\u7406\u4e0d\u6210\u529f\u3002"},
				new object[] {"noParsermsg2", "** \u627e\u4e0d\u5230\u89e3\u6790\u5668 **"},
				new object[] {"noParsermsg3", "\u8bf7\u68c0\u67e5\u60a8\u7684\u7c7b\u8def\u5f84\u3002"},
				new object[] {"noParsermsg4", "\u5982\u679c\u6ca1\u6709 IBM \u7684 XML Parser for Java\uff0c\u60a8\u53ef\u4ee5\u4ece\u4ee5\u4e0b\u4f4d\u7f6e\u4e0b\u8f7d\u5b83\uff1a"},
				new object[] {"noParsermsg5", "IBM \u7684 AlphaWorks\uff1ahttp://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "[-URIRESOLVER full class name \uff08\u4f7f\u7528 URIResolver \u89e3\u6790 URI\uff09]"},
				new object[] {"optionENTITYRESOLVER", "[-ENTITYRESOLVER full class name \uff08\u4f7f\u7528 EntityResolver \u89e3\u6790\u5b9e\u4f53\uff09]"},
				new object[] {"optionCONTENTHANDLER", "[-CONTENTHANDLER full class name \uff08\u4f7f\u7528 ContentHandler \u4e32\u884c\u5316\u8f93\u51fa\uff09]"},
				new object[] {"optionLINENUMBERS", "[-L use line numbers for source document]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE \uff08\u5c06\u5b89\u5168\u5904\u7406\u529f\u80fd\u8bbe\u7f6e\u4e3a true\u3002\uff09]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType \uff08\u4f7f\u7528 media \u5c5e\u6027\u67e5\u627e\u4e0e\u6587\u6863\u5173\u8054\u7684\u6837\u5f0f\u8868\u3002\uff09]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName \uff08\u663e\u5f0f\u4f7f\u7528 s2s=SAX \u6216 d2d=DOM \u8fdb\u884c\u8f6c\u6362\u3002\uff09]"},
				new object[] {"optionDIAG", "[-DIAG \uff08\u6253\u5370\u5168\u90e8\u6beb\u79d2\u8f6c\u6362\u6807\u8bb0\u3002\uff09]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL \uff08\u901a\u8fc7\u5c06 http://xml.apache.org/xalan/features/incremental \u8bbe\u7f6e\u4e3a true \u8bf7\u6c42\u589e\u91cf DTM \u6784\u9020\u3002\uff09]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE \uff08\u901a\u8fc7\u5c06 http://xml.apache.org/xalan/features/optimize \u8bbe\u7f6e\u4e3a false \u8bf7\u6c42\u65e0\u6837\u5f0f\u8868\u7684\u4f18\u5316\u5904\u7406\u3002\uff09]"},
				new object[] {"optionRL", "   [-RL recursionlimit \uff08\u65ad\u8a00\u6837\u5f0f\u8868\u9012\u5f52\u6df1\u5ea6\u7684\u6570\u5b57\u6781\u9650\u3002\uff09]"},
				new object[] {"optionXO", "[-XO [transletName] \uff08\u65ad\u8a00\u751f\u6210\u7684 translet \u7684\u540d\u79f0\uff09]"},
				new object[] {"optionXD", "[-XD destinationDirectory \uff08\u6307\u5b9a translet \u7684\u76ee\u6807\u76ee\u5f55\uff09]"},
				new object[] {"optionXJ", "[-XJ jarfile \uff08\u5c06 translet \u7c7b\u6253\u5305\u6210\u540d\u79f0\u4e3a <jarfile> \u7684 jar \u6587\u4ef6\uff09]"},
				new object[] {"optionXP", "[-XP package \uff08\u6307\u51fa\u6240\u6709\u751f\u6210\u7684 translet \u7c7b\u7684\u8f6f\u4ef6\u5305\u540d\u79f0\u524d\u7f00\uff09]"},
				new object[] {"optionXN", "[-XN \uff08\u542f\u7528\u6a21\u677f\u4ee3\u7801\u5d4c\u5165\uff09]"},
				new object[] {"optionXX", "[-XX \uff08\u6253\u5f00\u9644\u52a0\u8c03\u8bd5\u6d88\u606f\u8f93\u51fa\uff09]"},
				new object[] {"optionXT", "[-XT \uff08\u53ef\u80fd\u7684\u8bdd\u4f7f\u7528 translet \u8fdb\u884c\u8f6c\u6362\uff09]"},
				new object[] {"diagTiming", "--------- {0} \u901a\u8fc7 {1} \u7684\u8f6c\u6362\u8017\u65f6 {2} \u6beb\u79d2"},
				new object[] {"recursionTooDeep", "\u6a21\u677f\u5d4c\u5957\u592a\u6df1\u3002\u5d4c\u5957 = {0}\uff0c\u6a21\u677f {1} {2}"},
				new object[] {"nameIs", "\u540d\u79f0\u4e3a"},
				new object[] {"matchPatternIs", "\u5339\u914d\u6a21\u5f0f\u4e3a"}
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
	  public const string ERROR_STRING = "#\u9519\u8bef";

	  /// <summary>
	  /// String to prepend to error messages. </summary>
	  public const string ERROR_HEADER = "\u9519\u8bef\uff1a";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "\u8b66\u544a\uff1a";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public const string QUERY_HEADER = "PATTERN ";


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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("zh", "CN"));
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