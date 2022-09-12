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
 * $Id: XSLTErrorResources_hu.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_hu : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Hiba: Nem lehet '{' a kifejez\u00e9seken bel\u00fcl"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "A(z) {0}-nak \u00e9rv\u00e9nytelen attrib\u00fatuma van: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "A sourceNode \u00e9rt\u00e9ke null az xsl:apply-imports met\u00f3dusban."},
				new object[] {ER_CANNOT_ADD, "Nem lehet a(z) {0}-t felvenni a(z) {1}-ba"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "A sourceNode null a handleApplyTemplatesInstruction-ban!"},
				new object[] {ER_NO_NAME_ATTRIB, "A(z) {0}-nak kell legyen name attrib\u00fatuma."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Nem tal\u00e1lhat\u00f3 {0} nev\u0171 sablon"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Nem lehet feloldani a n\u00e9v AVT-t az xsl:call-template-ben."},
				new object[] {ER_REQUIRES_ATTRIB, "{0}-nek attrib\u00fatum sz\u00fcks\u00e9ges: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "A(z) {0} -nak kell legyen ''test'' attrib\u00fatuma. "},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Rossz \u00e9rt\u00e9k a level attrib\u00fatumban: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "A feldolgoz\u00e1si utas\u00edt\u00e1s neve nem lehet 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "A feldolgoz\u00e1si utas\u00edt\u00e1s neve \u00e9rv\u00e9nyes NCName kell legyen: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "A(z) {0}-nek kell legyen illeszked\u00e9si attrib\u00fatuma, ha van m\u00f3dja."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "A(z) {0}-nak kell vagy n\u00e9v vagy illeszked\u00e9si attrib\u00fatum."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Nem lehet feloldani a n\u00e9vt\u00e9r el\u0151tagot: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "Az xml:space \u00e9rt\u00e9ke \u00e9rv\u00e9nytelen: {0}"},
				new object[] {ER_NO_OWNERDOC, "A lesz\u00e1rmazott csom\u00f3pontnak nincs tulajdonos dokumentuma!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "ElemTemplateElement hiba: {0}"},
				new object[] {ER_NULL_CHILD, "K\u00eds\u00e9rlet null lesz\u00e1rmazott felv\u00e9tel\u00e9re!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "A(z) {0}-nak kell kiv\u00e1laszt\u00e1si attrib\u00fatum."},
				new object[] {ER_NEED_TEST_ATTRIB, "Az xsl:when-nek kell legyen 'test' attrib\u00fatuma."},
				new object[] {ER_NEED_NAME_ATTRIB, "Az xsl:param-nak kell legyen 'name' attrib\u00fatuma."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "A k\u00f6rnyezetnek nincs tulajdonos dokumentuma!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Nem lehet XML TransformerFactory Liaison-t l\u00e9trehozni: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "A Xalan folyamat sikertelen volt."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: sikertelen volt."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "A k\u00f3dol\u00e1s nem t\u00e1mogatott: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Nem lehet TraceListener-t l\u00e9trehozni: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "Az xsl:key-nek kell legyen 'name' attrib\u00fatuma!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "Az xsl:key-nek kell legyen 'match' attrib\u00fatuma!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "Az xsl:key-nek kell legyen 'use' attrib\u00fatuma!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) A(z) {0}-nak kell legyen ''elements'' attrib\u00fatuma! "},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) A(z) {0}-nak hi\u00e1nyzik a ''prefix'' attrib\u00fatuma"},
				new object[] {ER_BAD_STYLESHEET_URL, "A st\u00edluslap URL rossz: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "A st\u00edluslap f\u00e1jl nem tal\u00e1lhat\u00f3: {0}"},
				new object[] {ER_IOEXCEPTION, "IO kiv\u00e9tel t\u00f6rt\u00e9nt a st\u00edluslap f\u00e1jln\u00e1l: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) A(z) {0} href attrib\u00fatuma nem tal\u00e1lhat\u00f3"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) A(z) {0} k\u00f6zvetlen\u00fcl vagy k\u00f6zvetetten tartalmazza saj\u00e1t mag\u00e1t!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "StylesheetHandler.processInclude hiba, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) A(z) {0}-nak hi\u00e1nyzik a ''lang'' attrib\u00fatuma "},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) Rosszul elhelyezett {0} elem?? Hi\u00e1nyzik a ''component'' t\u00e1rol\u00f3elem"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Csak egy Element-be, DocumentFragment-be, Document-be vagy PrintWriter-be lehet kimenetet k\u00fcldeni."},
				new object[] {ER_PROCESS_ERROR, "StylesheetRoot.process hiba"},
				new object[] {ER_UNIMPLNODE_ERROR, "UnImplNode hiba: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Hiba! Az xpath kiv\u00e1laszt\u00e1si kifejez\u00e9s nem tal\u00e1lhat\u00f3 (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Nem lehet sorbarakni az XSLProcessor-t!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Nem adott meg st\u00edluslap bemenetet!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Nem siker\u00fclt feldolgozni a st\u00edluslapot!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Nem lehet elemezni a(z) {0} dokumentumot!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Nem tal\u00e1lhat\u00f3 a darab: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "A darab azonos\u00edt\u00f3 \u00e1ltal mutatott csom\u00f3pont nem elem: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "A for-each-nek legal\u00e1bb egy match vagy egy name attrib\u00fatuma kell legyen"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "A sablonoknak vagy match vagy name attrib\u00fatumuk kell legyen"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Nincs kl\u00f3nja egy dokumentumdarabnak!"},
				new object[] {ER_CANT_CREATE_ITEM, "Nem lehet elemet l\u00e9trehozni az eredm\u00e9nyf\u00e1ban: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "Az xml:space-nek a forr\u00e1s XML-ben tiltott \u00e9rt\u00e9ke van: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Nincs xsl:key deklar\u00e1ci\u00f3 a(z) {0}-hoz!"},
				new object[] {ER_CANT_CREATE_URL, "Hiba! Nem lehet URL-t l\u00e9trehozni ehhez: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "Az xsl:functions nem t\u00e1mogatott"},
				new object[] {ER_PROCESSOR_ERROR, "XSLT TransformerFactory hiba"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) A(z) {0} nem megengedett a st\u00edluslapon bel\u00fcl!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "A result-ns t\u00f6bb\u00e9 m\u00e1r nem t\u00e1mogatott!  Haszn\u00e1lja ink\u00e1bb az xsl:output-ot."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "A default-space t\u00f6bb\u00e9 m\u00e1r nem t\u00e1mogatott!  Haszn\u00e1lja ink\u00e1bb az xsl:strip-space-t vagy az  xsl:preserve-space-t."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "Az indent-result t\u00f6bb\u00e9 m\u00e1r nem t\u00e1mogatott!  Haszn\u00e1lja ink\u00e1bb az xsl:output-ot."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) A(z) {0}-nak tiltott attrib\u00fatuma van: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Ismeretlen XSL elem: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) Az xsl:sort csak az xsl:apply-templates-szel vagy xsl:for-each-el egy\u00fctt haszn\u00e1lhat\u00f3."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) Rosszul elhelyezett xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) Az xsl:when sz\u00fcl\u0151je nem xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) Rosszul elhelyezett xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) Az xsl:otherwise sz\u00fcl\u0151je nem xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) A(z) {0} nem megengedett sablonok belsej\u00e9ben!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) A(z) {0} kiterjeszt\u00e9s n\u00e9vt\u00e9r el\u0151tag {1} ismeretlen"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Az import\u00e1l\u00e1sok csak a st\u00edluslap els\u0151 elemei lehetnek!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) A(z) {0} k\u00f6zvetlen\u00fcl vagy k\u00f6zvetve tartalmazza saj\u00e1t mag\u00e1t!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space \u00e9rt\u00e9ke nem megengedett: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "A processStylesheet sikertelen volt!"},
				new object[] {ER_SAX_EXCEPTION, "SAX kiv\u00e9tel"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "A f\u00fcggv\u00e9ny nem t\u00e1mogatott!"},
				new object[] {ER_XSLT_ERROR, "XSLT hiba"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "A p\u00e9nzjel nem megengedett a form\u00e1tum minta karakterl\u00e1ncban"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "A document funkci\u00f3 nem t\u00e1mogatott a Stylesheet DOM-ban!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Nem lehet feloldani az el\u0151tagot egy nem-el\u0151tag felold\u00f3nak!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "\u00c1tir\u00e1ny\u00edt\u00e1s kiterjeszt\u00e9s: Nem lehet megkapni a f\u00e1jlnevet - a file vagy select attrib\u00fatumnak egy \u00e9rv\u00e9nyes karakterl\u00e1ncot kell visszaadnia."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Nem lehet FormatterListener-t \u00e9p\u00edteni az \u00e1tir\u00e1ny\u00edt\u00e1s kiterjeszt\u00e9sben!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Az el\u0151tag az exclude-result-prefixes-ben nem \u00e9rv\u00e9nyes: {0}"},
				new object[] {ER_MISSING_NS_URI, "Hi\u00e1nyzik a megadott el\u0151tag n\u00e9vt\u00e9r URI-ja"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Hi\u00e1nyzik az opci\u00f3 argumentuma: {0}"},
				new object[] {ER_INVALID_OPTION, "\u00c9rv\u00e9nytelen opci\u00f3: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Rossz form\u00e1tum\u00fa karakterl\u00e1nc: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "Az xsl:stylesheet-nek kell legyen 'version' attrib\u00fatuma!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "A(z) {0} attib\u00fatum \u00e9rt\u00e9ke \u00e9rv\u00e9nytelen: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "Az xsl:choose-hoz egy xsl:when sz\u00fcks\u00e9ges"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "Az xsl:apply-imports nem megengedett xsl:for-each-en bel\u00fcl"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Nem haszn\u00e1lhat DTMLiaison-t kimeneti DOM csom\u00f3pontk\u00e9nt... adjon \u00e1t ink\u00e1bb egy org.apache.xpath.DOM2Helper-t!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Nem haszn\u00e1lhat DTMLiaison-t bemeneti DOM csom\u00f3pontk\u00e9nt... adjon \u00e1t ink\u00e1bb egy org.apache.xpath.DOM2Helper-t!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "A kiterjeszt\u00e9s-elem megh\u00edv\u00e1sa sikertelen volt: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Az el\u0151tagnak egy n\u00e9vt\u00e9rre kell felold\u00f3dnia: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u00c9rv\u00e9nytelen UTF-16 helyettes\u00edt\u00e9s: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "A(z) {0} xsl:attribute-set-et saj\u00e1t mag\u00e1val haszn\u00e1lta, ami v\u00e9gtelen ciklust eredm\u00e9nyez."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Nem keverheti a nem Xerces-DOM bemenetet a Xerces-DOM kimenettel!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "Az ElemTemplateElement.readObject met\u00f3dusban: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Egyn\u00e9l t\u00f6bb ''{0}'' nev\u0171 sablont tal\u00e1ltam"},
				new object[] {ER_INVALID_KEY_CALL, "\u00c9rv\u00e9nytelen f\u00fcggv\u00e9nyh\u00edv\u00e1s: rekurz\u00edv key() h\u00edv\u00e1sok nem megengedettek"},
				new object[] {ER_REFERENCING_ITSELF, "A(z) {0} v\u00e1ltoz\u00f3 k\u00f6zvetlen\u00fcl vagy k\u00f6zvetve \u00f6nmag\u00e1ra hivatkozik!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "A bemeneti csom\u00f3pont nem lehet null egy DOMSource-ban a newTemplates-hez!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Az oszt\u00e1ly f\u00e1jl nem tal\u00e1lhat\u00f3 a(z) {0} opci\u00f3hoz"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "A sz\u00fcks\u00e9ges elem nem tal\u00e1lhat\u00f3: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "Az InputStream nem lehet null"},
				new object[] {ER_URI_CANNOT_BE_NULL, "Az URI nem lehet null"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "A f\u00e1jl nem lehet null"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "Az InputSource nem lehet null"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Nem lehet inicializ\u00e1lni a BSF kezel\u0151t"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Nem lehet leford\u00edtani a kiterjeszt\u00e9st"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Nem lehet l\u00e9trehozni a kiterjeszt\u00e9st ({0}) {1} miatt"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Az {0} met\u00f3dus p\u00e9ld\u00e1ny met\u00f3dush\u00edv\u00e1s\u00e1hoz sz\u00fcks\u00e9g van egy objektump\u00e9ld\u00e1nyra els\u0151 argumentumk\u00e9nt"},
				new object[] {ER_INVALID_ELEMENT_NAME, "\u00c9rv\u00e9nytelen elemnevet adott meg {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Az elemn\u00e9v met\u00f3dus statikus {0} kell legyen"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "{0} kiterjeszt\u00e9s funkci\u00f3 : A(z) {1} ismeretlen"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "T\u00f6bb legjobb illeszked\u00e9s a(z) {0} konstruktor\u00e1ra"},
				new object[] {ER_MORE_MATCH_METHOD, "T\u00f6bb legjobb illeszked\u00e9s a(z) {0} met\u00f3dusra"},
				new object[] {ER_MORE_MATCH_ELEMENT, "T\u00f6bb legjobb illeszked\u00e9s a(z) {0} elem met\u00f3dusra"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "\u00c9rv\u00e9nytelen k\u00f6rnyzetet adott \u00e1t a(z) {0} ki\u00e9rt\u00e9kel\u00e9s\u00e9hez"},
				new object[] {ER_POOL_EXISTS, "A t\u00e1rol\u00f3 m\u00e1r l\u00e9tezik"},
				new object[] {ER_NO_DRIVER_NAME, "Nem adott meg meghajt\u00f3nevet"},
				new object[] {ER_NO_URL, "Nem adott meg URL-t"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "A t\u00e1rol\u00f3 m\u00e9rete egyn\u00e9l kisebb!"},
				new object[] {ER_INVALID_DRIVER, "\u00c9rv\u00e9nytelen meghajt\u00f3nevet adott meg!"},
				new object[] {ER_NO_STYLESHEETROOT, "Nem tal\u00e1lhat\u00f3 a st\u00edluslap gy\u00f6kere!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Tiltott \u00e9rt\u00e9k az xml:space-hez"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "A processFromNode nem siker\u00fclt"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Az er\u0151forr\u00e1st [ {0} ] nem lehet bet\u00f6lteni: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Pufferm\u00e9ret <= 0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Ismeretlen hiba a kiterjeszt\u00e9s h\u00edv\u00e1s\u00e1n\u00e1l"},
				new object[] {ER_NO_NAMESPACE_DECL, "A(z) {0} el\u0151taghoz nem tartozik n\u00e9vt\u00e9r deklar\u00e1ci\u00f3"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Elem tartalom nem megengedett a(z) {0} lang=javaclass-hoz"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "St\u00edluslap \u00e1ltal ir\u00e1ny\u00edtott le\u00e1ll\u00e1s"},
				new object[] {ER_ONE_OR_TWO, "1 vagy 2"},
				new object[] {ER_TWO_OR_THREE, "2 vagy 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Nem lehet bet\u00f6lteni a(z) {0}-t (ellen\u0151rizze a CLASSPATH-t), most csak az alap\u00e9rtelmez\u00e9seket haszn\u00e1ljuk"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Nem lehet inicializ\u00e1lni az alap\u00e9rtelmezett sablonokat"},
				new object[] {ER_RESULT_NULL, "Az eredm\u00e9ny nem lehet null"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Nem lehet be\u00e1ll\u00edtani az eredm\u00e9nyt"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Nem adott meg kimenetet"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Nem alak\u00edthat\u00f3 \u00e1t {0} t\u00edpus\u00fa eredm\u00e9nny\u00e9"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "A(z) {0} t\u00edpus\u00fa forr\u00e1s nem alak\u00edthat\u00f3 \u00e1t "},
				new object[] {ER_NULL_CONTENT_HANDLER, "Null tartalomkezel\u0151"},
				new object[] {ER_NULL_ERROR_HANDLER, "Null hibakezel\u0151"},
				new object[] {ER_CANNOT_CALL_PARSE, "A parse nem h\u00edvhat\u00f3 meg, ha a ContentHandler-t nem \u00e1ll\u00edtotta be"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "A sz\u0171r\u0151nek nincs sz\u00fcl\u0151je"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Nincs st\u00edluslap ebben: {0}, adathordoz\u00f3: {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Nem tal\u00e1lhat\u00f3 xml-stylesheet PI itt: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Nem t\u00e1mogatott: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "A(z) {0} tulajdons\u00e1g \u00e9rt\u00e9ke Boolean p\u00e9ld\u00e1ny kell legyen"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Nem lehet eljutni a k\u00fcls\u0151 parancsf\u00e1jlhoz a(z) {0}-n"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "A(z) [ {0} ] er\u0151forr\u00e1s nem tal\u00e1lhat\u00f3.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "A kimeneti tulajdons\u00e1g nem felismerhet\u0151: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Nem siker\u00fclt ElemLiteralResult p\u00e9ld\u00e1nyt l\u00e9trehozni"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "A(z) {0} tulajdons\u00e1g \u00e9rt\u00e9ke \u00e9rtelmezhet\u0151 sz\u00e1m kell legyen"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "A(z) {0} \u00e9rt\u00e9ke igen vagy nem kell legyen"},
				new object[] {ER_FAILED_CALLING_METHOD, "Nem siker\u00fclt megh\u00edvni a(z) {0} met\u00f3dust"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Nem siker\u00fclt ElemTemplateElement p\u00e9ld\u00e1nyt l\u00e9trehozni"},
				new object[] {ER_CHARS_NOT_ALLOWED, "Karakterek nem megengedettek a dokumentumnak ezen a pontj\u00e1n"},
				new object[] {ER_ATTR_NOT_ALLOWED, "A(z) \"{0}\" attrib\u00fatum nem megengedett a(z) {1} elemhez!"},
				new object[] {ER_BAD_VALUE, "{0} rossz \u00e9rt\u00e9k {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "{0} attrib\u00fatum \u00e9rt\u00e9k nem tal\u00e1lhat\u00f3 "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "{0} attrib\u00fatum \u00e9rt\u00e9k ismeretlen "},
				new object[] {ER_NULL_URI_NAMESPACE, "K\u00eds\u00e9rlet egy n\u00e9vt\u00e9r el\u0151tag l\u00e9trehoz\u00e1s\u00e1ra null URI-val"},
				new object[] {ER_NUMBER_TOO_BIG, "K\u00eds\u00e9rlet egy sz\u00e1m megform\u00e1z\u00e1s\u00e1ra, ami nagyobb, mint a legnagyobb Long eg\u00e9sz"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Nem tal\u00e1lhat\u00f3 a(z) {0} SAX1 meghajt\u00f3oszt\u00e1ly"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "A(z) {0} SAX1 meghajt\u00f3oszt\u00e1ly megvan, de nem t\u00f6lthet\u0151 be"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "A(z) {0} SAX1 meghajt\u00f3oszt\u00e1ly bet\u00f6ltve, de nem lehet p\u00e9ld\u00e1nyt l\u00e9trehozni bel\u0151le"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "A(z) {0} SAX1 meghajt\u00f3oszt\u00e1ly nem implement\u00e1lja az org.xml.sax.Parser-t"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Nem adta meg az org.xml.sax.parser rendszertulajdons\u00e1got"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Az \u00e9rtelmez\u0151 argumentuma nem lehet null"},
				new object[] {ER_FEATURE, "K\u00e9pess\u00e9g: {0}"},
				new object[] {ER_PROPERTY, "Tulajdons\u00e1g: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Null entit\u00e1s felold\u00f3"},
				new object[] {ER_NULL_DTD_HANDLER, "Null DTD kezel\u0151"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Nem adott meg meghajt\u00f3nevet!"},
				new object[] {ER_NO_URL_SPECIFIED, "Nem adott meg URL-t!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "A t\u00e1rol\u00f3 m\u00e9rete 1-n\u00e9l kisebb!"},
				new object[] {ER_INVALID_DRIVER_NAME, "\u00c9rv\u00e9nytelen meghajt\u00f3nevet adott meg!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Programoz\u00f3i hiba! A kifejez\u00e9snek nincs ElemTemplateElement sz\u00fcl\u0151je!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Programoz\u00f3i \u00e9rtes\u00edt\u00e9s a RedundentExprEliminator h\u00edv\u00e1sban: {0} "},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} nem enged\u00e9lyezett a st\u00edluslap ezen hely\u00e9n!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Nem-szepar\u00e1tor sz\u00f6veg nem megengedett a st\u00edluslap ezen hely\u00e9n!"},
				new object[] {INVALID_TCHAR, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} attrib\u00fatumhoz: {1}.  A CHAR t\u00edpus\u00fa attrib\u00fatum csak 1 karakter lehet!"},
				new object[] {INVALID_QNAME, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} CHAR attrib\u00fatumhoz: {1}."},
				new object[] {INVALID_ENUM, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} ENUM attrib\u00fatumhoz: {1}.  Az \u00e9rv\u00e9nyes \u00e9rt\u00e9kek: {2}."},
				new object[] {INVALID_NMTOKEN, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} NMTOKEN attrib\u00fatumhoz: {1}. "},
				new object[] {INVALID_NCNAME, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} NCNAME attrib\u00fatumhoz: {1}. "},
				new object[] {INVALID_BOOLEAN, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} logikai attrib\u00fatumhoz: {1}. "},
				new object[] {INVALID_NUMBER, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} sz\u00e1m attrib\u00fatumhoz: {1}. "},
				new object[] {ER_ARG_LITERAL, "A(z) {0} argumentuma az illeszked\u00e9si mint\u00e1ban egy liter\u00e1l kell legyen."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "K\u00e9tszer szerepel a glob\u00e1lis v\u00e1ltoz\u00f3-deklar\u00e1ci\u00f3."},
				new object[] {ER_DUPLICATE_VAR, "K\u00e9tszer szerepel a v\u00e1ltoz\u00f3-deklar\u00e1ci\u00f3."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "Az xsl:template-nek kell legyen neve vagy illeszked\u00e9si attrib\u00fatuma (vagy mindkett\u0151)"},
				new object[] {ER_INVALID_PREFIX, "Az el\u0151tag az exclude-result-prefixes-ben nem \u00e9rv\u00e9nyes: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "A(z) {0} nev\u0171 attribute-set nem l\u00e9tezik"},
				new object[] {ER_FUNCTION_NOT_FOUND, "A(z) {0} nev\u0171 funkci\u00f3 nem l\u00e9tezik"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "A(z) {0} elemnek nem lehet egyszerre content \u00e9s select attrib\u00fatuma."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "A(z) {0} param\u00e9ter \u00e9rt\u00e9ke egy \u00e9rv\u00e9nyes J\u00e1va objektum kell legyen"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "Az xsl:namespace-alias elem result-prefix r\u00e9sz\u00e9nek \u00e9rt\u00e9ke '#default', de nincs meghat\u00e1rozva alap\u00e9rtelmezett n\u00e9vt\u00e9r az elem hat\u00f3k\u00f6r\u00e9ben. "},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "Egy xsl:namespace-alias elem result-prefix attrib\u00fatum\u00e1nak \u00e9rt\u00e9ke ''{0}'', de nincs n\u00e9vt\u00e9r deklar\u00e1ci\u00f3 a(z) ''{0}'' el\u0151taghoz az elem hat\u00f3k\u00f6r\u00e9ben. "},
				new object[] {ER_SET_FEATURE_NULL_NAME, "A szolg\u00e1ltat\u00e1s neve nem lehet null a TransformerFactory.setFeature(String name, boolean value) met\u00f3dusban."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "A szolg\u00e1ltat\u00e1s neve nem lehet null a TransformerFactory.getFeature(String name) met\u00f3dusban."},
				new object[] {ER_UNSUPPORTED_FEATURE, "A(z) ''{0}'' szolg\u00e1ltat\u00e1s nem \u00e1ll\u00edthat\u00f3 be ehhez a TransformerFactory oszt\u00e1lyhoz."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "A(z) ''{0}'' kiterjeszt\u00e9si elem haszn\u00e1lata nem megengedett, ha biztons\u00e1gos feldolgoz\u00e1s be van kapcsolva. "},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Nem lehet beolvasni az el\u0151tagot null n\u00e9vt\u00e9r URI eset\u00e9n. "},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Nem olvashat\u00f3 be a n\u00e9vt\u00e9r null el\u0151tag miatt. "},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "A f\u00fcggv\u00e9ny neve nem lehet null."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "Az arit\u00e1s nem lehet negat\u00edv."},
				new object[] {WG_FOUND_CURLYBRACE, "'}'-t tal\u00e1ltunk, de nincs attrib\u00fatumsablon megnyitva!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Figyelmeztet\u00e9s: A count attrib\u00fatum nem felel meg a egy felmen\u0151nek az xsl:number-ben! C\u00e9l = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "R\u00e9gi szintaktika: Az 'expr' attrib\u00fatum neve 'select'-re v\u00e1ltozott."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Az Xalan m\u00e9g nem kezeli a locale nevet a format-number f\u00fcggv\u00e9nyben."},
				new object[] {WG_LOCALE_NOT_FOUND, "Figyelmeztet\u00e9s: Nem tal\u00e1lhat\u00f3 az xml:lang={0} \u00e9rt\u00e9khez tartoz\u00f3 locale"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Nem k\u00e9sz\u00edthet\u0151 URL ebb\u0151l: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "A k\u00e9r dokumentum nem t\u00f6lthet\u0151 be: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Nem tal\u00e1lhat\u00f3 Collator a <sort xml:lang={0}-hez"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "R\u00e9gi szintaktika: a functions utas\u00edt\u00e1s {0} URL-t kell haszn\u00e1ljon"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "a k\u00f3dol\u00e1s nem t\u00e1mogatott: {0}, UTF-8-at haszn\u00e1lunk"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "a k\u00f3dol\u00e1s nem t\u00e1mogatott: {0}, Java {1}-t haszn\u00e1lunk"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Specifikuss\u00e1gi konfliktust tal\u00e1ltunk: {0} A st\u00edluslapon legutolj\u00e1ra megtal\u00e1ltat haszn\u00e1ljuk."},
				new object[] {WG_PARSING_AND_PREPARING, "========= {0} elemz\u00e9se \u00e9s el\u0151k\u00e9sz\u00edt\u00e9se =========="},
				new object[] {WG_ATTR_TEMPLATE, "Attr sablon, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Illeszt\u00e9si konfliktus az xsl:strip-space \u00e9s az xsl:preserve-space k\u00f6z\u00f6tt"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "A Xalan m\u00e9g nem kezeli a(z) {0} attrib\u00fatumot!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Nem tal\u00e1ltuk meg a deklar\u00e1ci\u00f3t a decim\u00e1lis form\u00e1tumhoz: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Hi\u00e1nyz\u00f3 vagy helytelen XSLT n\u00e9vt\u00e9r. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Csak az alap\u00e9rtelmezett xsl:decimal-format deklar\u00e1ci\u00f3 megengedett."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "Az xsl:decimal-format neveknek egyedieknek kell lenni\u00fck. A(z) \"{0}\" n\u00e9v meg lett ism\u00e9telve."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "A(z) {0}-nak \u00e9rv\u00e9nytelen attrib\u00fatuma van: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Nem lehet feloldani a n\u00e9vt\u00e9r el\u0151tagot: {0}. A csom\u00f3pont figyelmen k\u00edv\u00fcl marad."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "Az xsl:stylesheet-nek kell legyen 'version' attrib\u00fatuma!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Nem megengedett attrib\u00fatumn\u00e9v: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Tiltott \u00e9rt\u00e9ket haszn\u00e1lt a(z) {0} attrib\u00fatumhoz: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "A document f\u00fcggv\u00e9ny m\u00e1sodik argumentum\u00e1b\u00f3l el\u0151\u00e1ll\u00f3 csom\u00f3ponthalmaz \u00fcres. \u00dcres node-k\u00e9szletetet adok vissza."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "A(z) xsl:processing-instruction  n\u00e9v 'name' attrib\u00fatuma nem lehet 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "A(z) xsl:processing-instruction  n\u00e9v ''name'' attrib\u00fatuma \u00e9rv\u00e9nyes NCName kell legyen: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Nem lehet {0} attrib\u00fatumat felvenni a gyermek node-ok ut\u00e1n vagy miel\u0151tt egy elem l\u00e9trej\u00f6nne.  Az attrib\u00fatum figyelmen k\u00edv\u00fcl marad."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "K\u00eds\u00e9rlet t\u00f6rt\u00e9nt egy objektum m\u00f3dos\u00edt\u00e1s\u00e1ra, ahol a m\u00f3dos\u00edt\u00e1sok nem megengedettek. "},
				new object[] {"ui_language", "hu"},
				new object[] {"help_language", "hu"},
				new object[] {"language", "hu"},
				new object[] {"BAD_CODE", "A createMessage param\u00e9tere nincs a megfelel\u0151 tartom\u00e1nyban"},
				new object[] {"FORMAT_FAILED", "Kiv\u00e9tel t\u00f6rt\u00e9nt a messageFormat h\u00edv\u00e1s alatt"},
				new object[] {"version", ">>>>>>> Xalan verzi\u00f3 "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "igen"},
				new object[] {"line", "Sor #"},
				new object[] {"column","Oszlop #"},
				new object[] {"xsldone", "XSLProcessor: k\u00e9sz"},
				new object[] {"xslProc_option", "Xalan-J parancssori Process oszt\u00e1ly opci\u00f3k:"},
				new object[] {"xslProc_option", "Xalan-J parancssori Process oszt\u00e1ly opci\u00f3k\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "A(z) {0} opci\u00f3 nem t\u00e1mogatott XSLTC m\u00f3dban."},
				new object[] {"xslProc_invalid_xalan_option", "A(z) {0} opci\u00f3 csak -XSLTC-vel egy\u00fctt haszn\u00e1lhat\u00f3."},
				new object[] {"xslProc_no_input", "Hiba: Nem adott meg st\u00edluslapot vagy bemeneti xml-t. Futtassa ezt a parancsot kapcsol\u00f3k n\u00e9lk\u00fcl a haszn\u00e1lati utas\u00edt\u00e1sok megjelen\u00edt\u00e9s\u00e9re."},
				new object[] {"xslProc_common_options", "-\u00c1ltal\u00e1nos opci\u00f3k-"},
				new object[] {"xslProc_xalan_options", "-Xalan opci\u00f3k-"},
				new object[] {"xslProc_xsltc_options", "-XSLTC opci\u00f3k-"},
				new object[] {"xslProc_return_to_continue", "(nyomja la a <return> gombot a folytat\u00e1shoz)"},
				new object[] {"optionXSLTC", "   [-XSLTC (XSLTC-t haszn\u00e1l a transzform\u00e1l\u00e1shoz)]"},
				new object[] {"optionIN", "   [-IN bemenetiXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTranszform\u00e1ci\u00f3sURL]"},
				new object[] {"optionOUT", "   [-OUT kimenetiF\u00e1jln\u00e9v]"},
				new object[] {"optionLXCIN", "   [-LXCIN leford\u00edtottst\u00edluslapF\u00e1jln\u00e9vBe]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT leford\u00edtottSt\u00edluslapF\u00e1jln\u00e9vKi]"},
				new object[] {"optionPARSER", "   [-PARSER az \u00e9rtelmez\u0151kapcsolat teljesen meghat\u00e1rozott oszt\u00e1lyneve]"},
				new object[] {"optionE", "   [-E (Nem bontja ki az entit\u00e1s hivatkoz\u00e1sokat)]"},
				new object[] {"optionV", "   [-E (Nem bontja ki az entit\u00e1s hivatkoz\u00e1sokat)]"},
				new object[] {"optionQC", "   [-QC (Csendes mintakonfliktus figyelmeztet\u00e9sek)]"},
				new object[] {"optionQ", "   [-Q  (Csendes m\u00f3d)]"},
				new object[] {"optionLF", "   [-LF (A soremel\u00e9seket csak kimenet eset\u00e9n haszn\u00e1lja {alap\u00e9rtelmez\u00e9s: CR/LF})]"},
				new object[] {"optionCR", "   [-CR (A kocsivissza karaktert csak kimenet eset\u00e9n haszn\u00e1lja {alap\u00e9rtelmez\u00e9s: CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Mely karaktereket kell escape-elni {alap\u00e9rtelmez\u00e9s: <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Meghat\u00e1rozza, hogy h\u00e1ny sz\u00f3k\u00f6zzel kell beljebb kezdeni {alap\u00e9rtelmez\u00e9s: 0})]"},
				new object[] {"optionTT", "   [-TT (Nyomk\u00f6veti a sablonokat, ahogy azokat megh\u00edvj\u00e1k.)]"},
				new object[] {"optionTG", "   [-TG (Nyomk\u00f6veti az \u00f6sszes gener\u00e1l\u00e1si esem\u00e9nyt.)]"},
				new object[] {"optionTS", "   [-TS (Nyomk\u00f6veti az \u00f6sszes kiv\u00e1laszt\u00e1si esem\u00e9nyt.)]"},
				new object[] {"optionTTC", "   [-TTC (Nyomk\u00f6veti a sablon-lesz\u00e1rmazottakat, ahogy azokat feldolgozz\u00e1k.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (TraceListener oszt\u00e1ly a nyomk\u00f6vet\u00e9si kiterjeszt\u00e9sekhez.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Be\u00e1ll\u00edtja, hogy legyen-e \u00e9rv\u00e9nyess\u00e9gvizsg\u00e1lat.  Alap\u00e9rtelmez\u00e9sben nincs \u00e9rv\u00e9nyess\u00e9gvizsg\u00e1lat.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {opcion\u00e1lis f\u00e1jln\u00e9v} (Hib\u00e1n\u00e1l stackdump-ot hajt v\u00e9gre.)]"},
				new object[] {"optionXML", "   [-XML (XML form\u00e1z\u00f3 haszn\u00e1lata \u00e9s XML fejl\u00e9c hozz\u00e1ad\u00e1sa.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Egyszer\u0171 sz\u00f6vegform\u00e1z\u00f3 haszn\u00e1lata.)]"},
				new object[] {"optionHTML", "   [-HTML (HTML form\u00e1z\u00f3 haszn\u00e1lata.)]"},
				new object[] {"optionPARAM", "   [-PARAM n\u00e9v kifejez\u00e9s (Be\u00e1ll\u00edt egy st\u00edluslap param\u00e9tert)]"},
				new object[] {"noParsermsg1", "Az XSL folyamat sikertelen volt."},
				new object[] {"noParsermsg2", "** Az \u00e9rtelmez\u0151 nem tal\u00e1lhat\u00f3 **"},
				new object[] {"noParsermsg3", "K\u00e9rem, ellen\u0151rizze az oszt\u00e1ly el\u00e9r\u00e9si utat."},
				new object[] {"noParsermsg4", "Ha \u00f6nnek nincs meg az IBM Java XML \u00e9rtelmez\u0151je, akkor let\u00f6ltheti az"},
				new object[] {"noParsermsg5", "az IBM AlphaWorks weblapr\u00f3l: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER teljes oszt\u00e1lyn\u00e9v (az URIResolver fogja feloldani az URI-kat)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER teljes oszt\u00e1lyn\u00e9v (az EntityResolver fogja feloldani az entit\u00e1sokat)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER teljes oszt\u00e1lyn\u00e9v (a ContentHandler fogja soros\u00edtani a kimenetet)]"},
				new object[] {"optionLINENUMBERS", "   [-L sorsz\u00e1mokat haszn\u00e1l a forr\u00e1sdokumentumhoz]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (biztons\u00e1gos feldolgoz\u00e1s szolg\u00e1ltat\u00e1s igazra \u00e1ll\u00edt\u00e1sa.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA adathordoz\u00f3T\u00edpus (a media attrib\u00fatum seg\u00edts\u00e9g\u00e9vel megkeresi a dokumentumhoz tartoz\u00f3 st\u00edluslapot.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR \u00edzl\u00e9sN\u00e9v (Explicit haszn\u00e1lja az s2s=SAX-ot vagy d2d=DOM-ot a transzform\u00e1ci\u00f3hoz.)] "},
				new object[] {"optionDIAG", "   [-DIAG (Ki\u00edrja, hogy \u00f6sszesen h\u00e1ny ezredm\u00e1sodpercig tartott a transzform\u00e1ci\u00f3.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (n\u00f6vekm\u00e9nyes DTM l\u00e9trehoz\u00e1st ig\u00e9nyel a http://xml.apache.org/xalan/features/incremental igazra \u00e1ll\u00edt\u00e1s\u00e1val.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (nem ig\u00e9nyel st\u00edluslap optimiz\u00e1l\u00e1st a http://xml.apache.org/xalan/features/optimize hamisra \u00e1ll\u00edt\u00e1s\u00e1t.)]"},
				new object[] {"optionRL", "   [-RL rekurzi\u00f3korl\u00e1t (numerikusan korl\u00e1tozza a st\u00edluslap rekurzi\u00f3 m\u00e9lys\u00e9g\u00e9t.)]"},
				new object[] {"optionXO", "   [-XO [transletNeve] (a nevet rendeli a gener\u00e1lt translethez)]"},
				new object[] {"optionXD", "   [-XD c\u00e9lAlk\u00f6nyvt\u00e1r (a translet c\u00e9l-alk\u00f6nyvt\u00e1ra)]"},
				new object[] {"optionXJ", "   [-XJ jarf\u00e1jl (a translet oszt\u00e1lyokat a megadott <jarf\u00e1jl>-ba csomagolja)]"},
				new object[] {"optionXP", "   [-XP csomag (megadja a gener\u00e1lt translet oszt\u00e1lyok n\u00e9v-prefix\u00e9t)]"},
				new object[] {"optionXN", "   [-XN (enged\u00e9lyezi a template inlining optimaliz\u00e1l\u00e1st)]"},
				new object[] {"optionXX", "   [-XX (bekapcsolja a tov\u00e1bbi hibakeres\u00e9si kimenetet)]"},
				new object[] {"optionXT", "   [-XT (translet-et haszn\u00e1lt az \u00e1talak\u00edt\u00e1shoz, ha lehet)]"},
				new object[] {"diagTiming"," --------- A(z) {0} tarnszform\u00e1ci\u00f3a a(z) {1}-el {2} ms-ig tartott"},
				new object[] {"recursionTooDeep","A sablonon egym\u00e1sba \u00e1gyaz\u00e1sa t\u00fal m\u00e9ly. Be\u00e1gyaz\u00e1s = {0}, sablon: {1} {2}"},
				new object[] {"nameIs", "A n\u00e9v:"},
				new object[] {"matchPatternIs", "Az illeszked\u00e9si minta:"}
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
	  public const string ERROR_HEADER = "Hiba: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Figyelmeztet\u00e9s: ";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public const string QUERY_HEADER = "MINTA ";


	  /// <summary>
	  ///   Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  ///   of ResourceBundle.getBundle().
	  /// </summary>
	  ///   <param name="className"> the name of the class that implements the resource bundle. </param>
	  ///   <returns> the ResourceBundle </returns>
	  ///   <exception cref="MissingResourceException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final XSLTErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XSLTErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.Default;
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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("hu", "HU"));
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