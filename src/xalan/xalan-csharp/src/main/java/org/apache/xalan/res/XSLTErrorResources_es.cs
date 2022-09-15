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
 * $Id: XSLTErrorResources_es.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_es : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Error: No puede haber '{' dentro de la expresi\u00f3n"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} tiene un atributo no permitido: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode es nulo en xsl:apply-imports."},
				new object[] {ER_CANNOT_ADD, "No se puede a\u00f1adir {0} a {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode es nulo en handleApplyTemplatesInstruction."},
				new object[] {ER_NO_NAME_ATTRIB, "{0} debe tener un atributo de nombre."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "No se ha podido encontrar la plantilla: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "No se ha podido resolver AVT de nombre en xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} necesita un atributo: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} debe tener un atributo ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Valor incorrecto en atributo de nivel: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Nombre de processing-instruction no puede ser 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Nombre de processing-instruction debe ser un NCName v\u00e1lido: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} debe tener un atributo de coincidencia si tiene una modalidad."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} necesita un atributo de nombre o de coincidencia."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "No se puede resolver el prefijo del espacio de nombres: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space tiene un valor no permitido: {0}"},
				new object[] {ER_NO_OWNERDOC, "El nodo hijo no tiene un documento propietario."},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Error de ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "Intentando a\u00f1adir un hijo nulo"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} necesita un atributo de selecci\u00f3n."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when debe tener un atributo 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param debe tener un atributo 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "El contexto no tiene un documento propietario."},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "No se ha podido crear Liaison TransformerFactory XML: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "El proceso Xalan no ha sido satisfactorio."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan no ha sido satisfactorio."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Codificaci\u00f3n no soportada: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "No se ha podido crear TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key necesita un atributo 'name'."},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key necesita un atributo 'match'."},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key necesita un atributo 'use'."},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} necesita un atributo ''elements'' "},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) Falta el atributo ''prefix'' de {0} "},
				new object[] {ER_BAD_STYLESHEET_URL, "El URL de la hoja de estilos es incorrecto: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "No se ha encontrado el archivo de hoja de estilos: {0}"},
				new object[] {ER_IOEXCEPTION, "Se ha producido una excepci\u00f3n de ES con el archivo de hoja de estilos: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) No se ha podido encontrar el atributo href para {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) Inclusi\u00f3n propia de {0} directa o indirectamente."},
				new object[] {ER_PROCESSINCLUDE_ERROR, "Error de StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) Falta el atributo ''lang'' de {0}"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) Elemento {0} incorrecto. Falta el elemento de contenedor ''component''."},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "S\u00f3lo se puede dar salida hacia Element, DocumentFragment, Document o PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "Error de StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Error de UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Error. No se ha encontrado la expresi\u00f3n de selecci\u00f3n (-select) de xpath."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "No se puede serializar un XSLProcessor."},
				new object[] {ER_NO_INPUT_STYLESHEET, "No se ha especificado la entrada de hoja de estilos."},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "No se ha podido procesar la hoja de estilos."},
				new object[] {ER_COULDNT_PARSE_DOC, "No se ha podido analizar el documento {0}."},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "No se ha podido encontrar el fragmento: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "El nodo se\u00f1alado por un identificador de fragmento no es un elemento: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each debe tener un atributo de coincidencia o de nombre."},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "templates debe tener un atributo de coincidencia o de nombre."},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "No es r\u00e9plica de un fragmento de documento."},
				new object[] {ER_CANT_CREATE_ITEM, "No se puede crear el elemento en el \u00e1rbol de resultados: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space en el XML fuente tiene un valor no permitido: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "No hay declaraci\u00f3n xsl:key para {0}."},
				new object[] {ER_CANT_CREATE_URL, "Error. No se puede crear url para: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions no est\u00e1 soportado"},
				new object[] {ER_PROCESSOR_ERROR, "Error de XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} no permitido dentro de una hoja de estilos."},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "Ya no se soporta result-ns.  Utilice xsl:output en su lugar."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "Ya no se soporta default-space.  Utilice xsl:strip-space o xsl:preserve-space en su lugar."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "Ya no se soporta indent-result.  Utilice xsl:output en su lugar."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} tiene un atributo no permitido: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Elemento XSL desconocido: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort s\u00f3lo puede utilizarse con xsl:apply-templates o xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) xsl:when equivocado."},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when no emparentado por xsl:choose."},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) xsl:otherwise equivocado."},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise no emparentado por xsl:choose."},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} no permitido dentro de una plantilla."},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) Prefijo {1} de espacio de nombres de extensi\u00f3n {0} desconocido"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Las importaciones s\u00f3lo pueden aparecer como primeros elementos de la hoja de estilos."},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) Importaci\u00f3n propia de {0} directa o indirectamente."},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space tiene un valor no permitido: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet no satisfactorio."},
				new object[] {ER_SAX_EXCEPTION, "Excepci\u00f3n SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Funci\u00f3n no soportada."},
				new object[] {ER_XSLT_ERROR, "Error de XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "El signo monetario no est\u00e1 permitido en la serie del patr\u00f3n de formato"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "La funci\u00f3n de documento no est\u00e1 soportada en DOM de hoja de estilos."},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "No se puede resolver el prefijo de un resolucionador sin prefijo."},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Extensi\u00f3n Redirect: No se ha podido obtener el nombre de archivo - el atributo de archivo o de selecci\u00f3n debe devolver una serie v\u00e1lida."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "No se puede crear FormatterListener en extensi\u00f3n Redirect."},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "El prefijo en exclude-result-prefixes no es v\u00e1lido: {0}"},
				new object[] {ER_MISSING_NS_URI, "Falta el URI del espacio de nombres para el prefijo especificado"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Falta un argumento para la opci\u00f3n: {0}"},
				new object[] {ER_INVALID_OPTION, "Opci\u00f3n no v\u00e1lida: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Serie de formato mal formada: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet necesita un atributo 'version'."},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Atributo: {0} tiene un valor no permitido: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose necesita un xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports no permitido en xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "No se puede utilizar DTMLiaison para un nodo DOM de salida... Pase org.apache.xpath.DOM2Helper en su lugar."},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "No se puede utilizar DTMLiaison para un nodo DOM de entrada... Pase org.apache.xpath.DOM2Helper en su lugar."},
				new object[] {ER_CALL_TO_EXT_FAILED, "Anomal\u00eda al llamar al elemento de extensi\u00f3n: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "El prefijo debe resolverse como un espacio de nombres: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u00bfSe ha detectado un sustituto UTF-16 no v\u00e1lido: {0}?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} se ha utilizado a s\u00ed mismo lo que puede provocar un bucle infinito."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "No se puede mezclar la entrada Xerces-DOM con la salida Xerces-DOM."},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "En ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Se ha encontrado m\u00e1s de una plantilla con el nombre: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Llamada de funci\u00f3n no v\u00e1lida: no est\u00e1n permitidas las llamadas key() recursivas"},
				new object[] {ER_REFERENCING_ITSELF, "La variable {0} se est\u00e1 referenciando a s\u00ed misma directa o indirectamente."},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "El nodo de entrada no puede ser nulo para DOMSource de newTemplates."},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "No se ha encontrado el archivo de clase para la opci\u00f3n {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "No se ha encontrado un elemento necesario: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream no puede ser nulo"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI no puede ser nulo"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "Archivo no puede ser nulo"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource no puede ser nulo"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "No se ha podido inicializar el Gestor BSF"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "No se ha podido compilar la extensi\u00f3n"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "No se ha podido crear la extensi\u00f3n: {0} como consecuencia de: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "La llamada del m\u00e9todo de instancia al m\u00e9todo {0} necesita una instancia Object como primer argumento"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Se ha especificado un nombre de elemento no v\u00e1lido {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "El m\u00e9todo del nombre de elemento debe ser est\u00e1tico {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Funci\u00f3n de extensi\u00f3n {0} : {1} desconocida"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Hay m\u00e1s de una coincidencia m\u00e1xima para el constructor de {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Hay m\u00e1s de una coincidencia m\u00e1xima para el m\u00e9todo {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Hay m\u00e1s de una coincidencia m\u00e1xima para el m\u00e9todo de elemento {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Se ha pasado un contexto no v\u00e1lido para evaluar {0}"},
				new object[] {ER_POOL_EXISTS, "La agrupaci\u00f3n ya existe"},
				new object[] {ER_NO_DRIVER_NAME, "No se ha especificado un nombre de controlador"},
				new object[] {ER_NO_URL, "No se ha especificado un URL"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "El tama\u00f1o de la agrupaci\u00f3n es menor que uno."},
				new object[] {ER_INVALID_DRIVER, "Se ha especificado un nombre de controlador no v\u00e1lido."},
				new object[] {ER_NO_STYLESHEETROOT, "No se ha encontrado la ra\u00edz de la hoja de estilos."},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Valor no permitido para xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "Anomal\u00eda de processFromNode"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "No se ha podido cargar el recurso [ {0} ]: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Tama\u00f1o de almacenamiento intermedio <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Error desconocido al llamar a la extensi\u00f3n"},
				new object[] {ER_NO_NAMESPACE_DECL, "El prefijo {0} no tiene una declaraci\u00f3n de espacio de nombres correspondiente"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "No se permite el contenido del elemento para lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Terminaci\u00f3n de hoja de estilos dirigida"},
				new object[] {ER_ONE_OR_TWO, "1 \u00f3 2"},
				new object[] {ER_TWO_OR_THREE, "2 \u00f3 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "No se ha podido cargar {0} (compruebe la CLASSPATH), ahora s\u00f3lo se est\u00e1n utilizando los valores predeterminados"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "No se han podido inicializar las plantillas predeterminadas"},
				new object[] {ER_RESULT_NULL, "El resultado no deber\u00eda ser nulo"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "No se ha podido establecer el resultado"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "No se ha especificado salida"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "No se puede transformar un resultado de tipo {0} "},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "No se puede transformar un fuente de tipo {0} "},
				new object[] {ER_NULL_CONTENT_HANDLER, "Manejador de contenido nulo"},
				new object[] {ER_NULL_ERROR_HANDLER, "Manejador de error nulo"},
				new object[] {ER_CANNOT_CALL_PARSE, "No se puede llamar a parse si no se ha establecido ContentHandler"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "No hay padre para el filtro"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "No se han encontrado hojas de estilos en: {0}, soporte= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "No se ha encontrado xml-stylesheet PI en: {0}"},
				new object[] {ER_NOT_SUPPORTED, "No soportado: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "El valor de la propiedad {0} deber\u00eda ser una instancia Boolean"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "No se ha podido encontrar el script externo en {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "No se ha podido encontrar el recurso [ {0} ].\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "No se reconoce la propiedad de salida: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Anomal\u00eda al crear la instancia ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "El valor para {0} deber\u00eda contener un n\u00famero analizable"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "El valor de {0} deber\u00eda ser s\u00ed o no"},
				new object[] {ER_FAILED_CALLING_METHOD, "Anomal\u00eda al llamar al m\u00e9todo {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Anomal\u00eda al crear la instancia ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "No se permiten caracteres en este punto del documento"},
				new object[] {ER_ATTR_NOT_ALLOWED, "El atributo \"{0}\" no est\u00e1 permitido en el elemento {1}."},
				new object[] {ER_BAD_VALUE, "{0} valor incorrecto {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "No se ha encontrado el valor del atributo {0} "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "No se ha reconocido el valor del atributo {0} "},
				new object[] {ER_NULL_URI_NAMESPACE, "Se ha intentado generar un prefijo de espacio de nombres con un URI nulo"},
				new object[] {ER_NUMBER_TOO_BIG, "Se ha intentado formatear un n\u00famero mayor que el entero largo m\u00e1s grande"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "No se ha podido encontrar la clase de controlador SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "Se ha encontrado la clase de controlador SAX1 {0} pero no se ha podido cargar"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "Se ha cargado la clase de controlador SAX1 {0} pero no se ha podido crear una instancia"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "La clase de controlador SAX1 {0} no implementa org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "No se ha especificado la propiedad del sistema org.xml.sax.parser"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "El argumento del analizador no debe ser nulo"},
				new object[] {ER_FEATURE, "Caracter\u00edstica: {0}"},
				new object[] {ER_PROPERTY, "Propiedad: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Resolucionador de entidad nulo"},
				new object[] {ER_NULL_DTD_HANDLER, "Manejador DTD nulo"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "No se ha especificado un nombre de controlador."},
				new object[] {ER_NO_URL_SPECIFIED, "No se ha especificado un URL."},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "El tama\u00f1o de la agrupaci\u00f3n es menor que 1."},
				new object[] {ER_INVALID_DRIVER_NAME, "Se ha especificado un nombre de controlador no v\u00e1lido."},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Error del programador. La expresi\u00f3n no tiene un padre ElemTemplateElement."},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Aserci\u00f3n del programador en RedundentExprEliminator: {0} "},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} no est\u00e1 permitido en esta posici\u00f3n de la hoja de estilos."},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "No est\u00e1 permitido texto sin espacios en blanco en esta posici\u00f3n de la hoja de estilos."},
				new object[] {INVALID_TCHAR, "Valor no permitido: se ha utilizado {1} para el atributo CHAR: {0}.  Un atributo de tipo CHAR debe ser de un solo car\u00e1cter."},
				new object[] {INVALID_QNAME, "Valor no permitido: se ha utilizado {1} para el atributo QNAME: {0}"},
				new object[] {INVALID_ENUM, "Valor no permitido: se ha utilizado {1} para el atributo ENUM: {0}.  Los valores v\u00e1lidos son: {2}."},
				new object[] {INVALID_NMTOKEN, "Valor no permitido: se ha utilizado {1} para el atributo NMTOKEN: {0} "},
				new object[] {INVALID_NCNAME, "Valor no permitido: se ha utilizado {1} para el atributo NCNAME: {0} "},
				new object[] {INVALID_BOOLEAN, "Valor no permitido: se ha utilizado {1} para el atributo boolean: {0} "},
				new object[] {INVALID_NUMBER, "Valor no permitido: se ha utilizado {1} para el atributo number: {0} "},
				new object[] {ER_ARG_LITERAL, "El argumento para {0} en el patr\u00f3n de coincidencia debe ser un literal."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Declaraci\u00f3n de variable global duplicada."},
				new object[] {ER_DUPLICATE_VAR, "Declaraci\u00f3n de variable duplicada."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template debe tener un atributo name o match (o ambos)."},
				new object[] {ER_INVALID_PREFIX, "El prefijo en exclude-result-prefixes no es v\u00e1lido: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "attribute-set de nombre {0} no existe"},
				new object[] {ER_FUNCTION_NOT_FOUND, "La funci\u00f3n de nombre {0} no existe"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "El elemento {0} no debe tener contenido y un atributo select."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "El valor del par\u00e1metro {0} debe ser un objeto Java v\u00e1lido"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "El atributo result-prefix de un elemento xsl:namespace-alias tiene el valor '#default', pero no hay declaraci\u00f3n de espacio de nombres predeterminado en el \u00e1mbito del elemento."},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "El atributo result-prefix de un elemento xsl:namespace-alias tiene el valor ''{0}'', pero no hay declaraci\u00f3n de espacio de nombres para el prefijo ''{0}'' en el \u00e1mbito del elemento."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "El nombre de caracter\u00edstica no puede ser nulo en TransformerFactory.setFeature(nombre de tipo String, valor booleano)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "El nombre de caracter\u00edstica no puede ser nulo en TransformerFactory.getFeature(nombre de tipo String)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "No se puede establecer la caracter\u00edstica ''{0}'' en esta TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "No se permite el uso del elemento de extensi\u00f3n ''{0}'' cuando la caracter\u00edstica de proceso seguro est\u00e1 establecida en true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "No se puede obtener el prefijo de un uri de espacio de nombres nulo."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "No se puede obtener el uri de espacio de nombres para un prefijo nulo."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "El nombre de funci\u00f3n no puede ser nulo."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "La aridad no puede ser negativa."},
				new object[] {WG_FOUND_CURLYBRACE, "Se ha encontrado '}' pero no se ha abierto una plantilla de atributos."},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Aviso: El atributo count no coincide con un antecesor en xsl:number. Destino = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Sintaxis antigua: El nombre del atributo 'expr' se ha cambiado por 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan no maneja a\u00fan el nombre de entorno local en la funci\u00f3n format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Aviso: No se ha podido encontrar el entorno local para xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "No se puede crear URL desde: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "No se puede cargar el doc solicitado: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "No se ha podido encontrar clasificador para <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Sintaxis antigua: La instrucci\u00f3n functions deber\u00eda utilizar un url de {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "Codificaci\u00f3n no soportada: {0}, se utiliza UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "Codificaci\u00f3n no soportada: {0}, se utiliza Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Se han encontrado conflictos de especificaci\u00f3n: {0} Se utilizar\u00e1 lo \u00faltimo encontrado en la hoja de estilos."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Analizando y preparando {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Plantilla de atributos, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Conflicto de coincidencia entre xsl:strip-space y xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan no maneja a\u00fan el atributo {0}."},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "No se ha encontrado declaraci\u00f3n para el formato decimal: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Falta el espacio de nombres XSLT o es incorrecto. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "S\u00f3lo se permite una declaraci\u00f3n xsl:decimal-format predeterminada."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "Los nombres de xsl:decimal-format deben ser \u00fanicos. El nombre \"{0}\" se ha duplicado."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} tiene un atributo no permitido: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "No se ha podido resolver el prefijo del espacio de nombres: {0}. Se ignorar\u00e1 el nodo."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet necesita un atributo 'version'."},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Nombre de atributo no permitido: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Se ha utilizado un valor no permitido para el atributo {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "El NodeSet resultante del segundo argumento de la funci\u00f3n del documento est\u00e1 vac\u00edo. Devuelve un conjunto de nodos vac\u00edo."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "El valor del atributo 'name' de nombre xsl:processing-instruction no debe ser 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "El valor del atributo ''name'' de xsl:processing-instruction debe ser un NCName v\u00e1lido: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "No se puede a\u00f1adir el atributo {0} despu\u00e9s de nodos hijo o antes de que se produzca un elemento.  Se ignorar\u00e1 el atributo."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Se ha intentado modificar un objeto que no admite modificaciones. "},
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
				new object[] {"xslProc_option", "Opciones de la clase Process de la l\u00ednea de mandatos Xalan-J:"},
				new object[] {"xslProc_option", "Opciones de la clase Process de la l\u00ednea de mandatos Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "La opci\u00f3n {0} no est\u00e1 soportada en modalidad XSLTC."},
				new object[] {"xslProc_invalid_xalan_option", "La opci\u00f3n {0} s\u00f3lo puede utilizarse con -XSLTC."},
				new object[] {"xslProc_no_input", "Error: No se ha especificado ninguna hoja de estilos ni xml de entrada. Ejecute este mandato sin opciones para ver las instrucciones de uso."},
				new object[] {"xslProc_common_options", "-Opciones comunes-"},
				new object[] {"xslProc_xalan_options", "-Opciones para Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Opciones para XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(pulse <Intro> para continuar)"},
				new object[] {"optionXSLTC", "[-XSLTC (Utilizar XSLTC para transformaci\u00f3n)]"},
				new object[] {"optionIN", "[-IN URL_XML_entrada]"},
				new object[] {"optionXSL", "[-XSL URL_transformaci\u00f3n_XSL]"},
				new object[] {"optionOUT", "[-OUT nombre_archivo_salida]"},
				new object[] {"optionLXCIN", "[-LXCIN entrada_nombre_archivo_hoja_estilos_compilada]"},
				new object[] {"optionLXCOUT", "[-LXCOUT salida_nombre_archivo_hoja_estilos_compilada]"},
				new object[] {"optionPARSER", "[-PARSER nombre de clase completamente cualificado del enlace del analizador]"},
				new object[] {"optionE", "[-E (No expandir referencias de entidades)]"},
				new object[] {"optionV", "[-E (No expandir referencias de entidades)]"},
				new object[] {"optionQC", "[-QC (Avisos silenciosos de conflictos de patrones)]"},
				new object[] {"optionQ", "[-Q  (Modalidad silenciosa)]"},
				new object[] {"optionLF", "[-LF (Utilizar s\u00f3lo avances de l\u00ednea en la salida {el valor predeterminado es CR/LF})]"},
				new object[] {"optionCR", "[-CR (Utilizar s\u00f3lo retornos de carro en la salida {el valor predeterminado es CR/LF})]"},
				new object[] {"optionESCAPE", "[-ESCAPE (Caracteres con escape {el valor predeterminado es <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "[-INDENT (Controlar el n\u00famero de espacios de sangrado {el valor predeterminado es 0})]"},
				new object[] {"optionTT", "[-TT (Rastrear las plantillas a medida que se llaman.)]"},
				new object[] {"optionTG", "[-TG (Rastrear cada suceso de generaci\u00f3n.)]"},
				new object[] {"optionTS", "[-TS (Rastrear cada suceso de selecci\u00f3n.)]"},
				new object[] {"optionTTC", "[-TTC (Rastrear los hijos de plantillas a medida que se procesan.)]"},
				new object[] {"optionTCLASS", "[-TCLASS (Clase TraceListener para extensiones de rastreo.)]"},
				new object[] {"optionVALIDATE", "[-VALIDATE (Establecer si se realiza la validaci\u00f3n.  De forma predeterminada la validaci\u00f3n est\u00e1 desactivada.)]"},
				new object[] {"optionEDUMP", "[-EDUMP {nombre de archivo opcional} (Realizar vuelco de pila si se produce un error.)]"},
				new object[] {"optionXML", "[-XML (Utilizar el formateador XML y a\u00f1adir la cabecera XML.)]"},
				new object[] {"optionTEXT", "[-TEXT (Utilizar el formateador de texto sencillo.)]"},
				new object[] {"optionHTML", "[-HTML (Utilizar el formateador HTML.)]"},
				new object[] {"optionPARAM", "[-PARAM expresi\u00f3n de nombre (Establecer un par\u00e1metro de hoja de estilos)]"},
				new object[] {"noParsermsg1", "El proceso XSL no ha sido satisfactorio."},
				new object[] {"noParsermsg2", "** No se ha podido encontrar el analizador **"},
				new object[] {"noParsermsg3", "Compruebe la classpath."},
				new object[] {"noParsermsg4", "Si no dispone del analizador XML para Java de IBM, puede descargarlo de"},
				new object[] {"noParsermsg5", "IBM AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "[-URIRESOLVER nombre de clase completo (URIResolver a utilizar para resolver URI)]"},
				new object[] {"optionENTITYRESOLVER", "[-ENTITYRESOLVER nombre de clase completo (EntityResolver a utilizar para resolver entidades)]"},
				new object[] {"optionCONTENTHANDLER", "[-CONTENTHANDLER nombre de clase completo (ContentHandler a utilizar para serializar la salida)]"},
				new object[] {"optionLINENUMBERS", "[-L utilizar n\u00fameros de l\u00ednea para el documento fuente]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (establecer la caracter\u00edstica de proceso seguro en true.)]"},
				new object[] {"optionMEDIA", "[-MEDIA tipo_soporte (Utilizar el atributo de soporte para encontrar la hoja de estilos asociada con un documento.)]"},
				new object[] {"optionFLAVOR", "[-FLAVOR nombre_estilo (Utilizar expl\u00edcitamente s2s=SAX o d2d=DOM para realizar la transformaci\u00f3n.)] "},
				new object[] {"optionDIAG", "[-DIAG (Imprimir el total de milisegundos que lleva la transformaci\u00f3n.)]"},
				new object[] {"optionINCREMENTAL", "[-INCREMENTAL (Solicitar construcci\u00f3n DTM incremental estableciendo http://xml.apache.org/xalan/features/incremental como verdadero.)]"},
				new object[] {"optionNOOPTIMIMIZE", "[-NOOPTIMIMIZE (Solicitar proceso de optimizaci\u00f3n de hoja de estilos estableciendo http://xml.apache.org/xalan/features/optimize como falso.)]"},
				new object[] {"optionRL", "[-RL l\u00edmite_recursi\u00f3n (L\u00edmite num\u00e9rico de aserci\u00f3n sobre profundidad de recursi\u00f3n de hoja de estilos.)]"},
				new object[] {"optionXO", "[-XO [nombreTranslet] (Asignar el nombre al translet generado)]"},
				new object[] {"optionXD", "[-XD directorioDestino (Especificar un directorio de destino para translet)]"},
				new object[] {"optionXJ", "[-XJ archivoJar (Empaqueta las clases translet en un archivo jar de nombre <archivoJar>)]"},
				new object[] {"optionXP", "[-XP paquete (Especifica un prefijo para el nombre del paquete de todas las clases translet generadas)]"},
				new object[] {"optionXN", "[-XN (habilita la inclusi\u00f3n en l\u00ednea de plantillas)]"},
				new object[] {"optionXX", "[-XX (activa la salida de mensajes de depuraci\u00f3n adicionales)]"},
				new object[] {"optionXT", "[-XT (utilizar translet para transformar si es posible)]"},
				new object[] {"diagTiming", "--------- La transformaci\u00f3n de {0} mediante {1} ha durado {2} ms"},
				new object[] {"recursionTooDeep", "Anidado de plantilla demasiado profundo. anidado = {0}, plantilla {1} {2}"},
				new object[] {"nameIs", "el nombre es"},
				new object[] {"matchPatternIs", "el patr\u00f3n de coincidencia es"}
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
	  public const string ERROR_STRING = "#error";

	  /// <summary>
	  /// String to prepend to error messages. </summary>
	  public const string ERROR_HEADER = "Error: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Aviso: ";

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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("es", "ES"));
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