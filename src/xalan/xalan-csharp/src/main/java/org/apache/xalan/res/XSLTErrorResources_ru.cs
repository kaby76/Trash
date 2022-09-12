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
 * $Id: XSLTErrorResources_ru.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_ru : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "\u041e\u0448\u0438\u0431\u043a\u0430: \u0421\u043a\u043e\u0431\u043a\u0430 '{' \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u0430 \u0432 \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0438"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "\u0414\u043b\u044f {0} \u0443\u043a\u0430\u0437\u0430\u043d \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0430\u0442\u0440\u0438\u0431\u0443\u0442: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "\u041f\u0443\u0441\u0442\u043e\u0439 sourceNode \u0432 xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0434\u043e\u0431\u0430\u0432\u0438\u0442\u044c {0} \u0432 {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "\u041f\u0443\u0441\u0442\u043e\u0439 sourceNode \u0432 handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "\u0423 {0} \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 name"},
				new object[] {ER_TEMPLATE_NOT_FOUND, "\u0423\u043a\u0430\u0437\u0430\u043d\u043d\u044b\u0439 \u0448\u0430\u0431\u043b\u043e\u043d \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0438\u043c\u044f AVT \u0432 xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "\u0414\u043b\u044f {0} \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0443\u043a\u0430\u0437\u0430\u043d \u0430\u0442\u0440\u0438\u0431\u0443\u0442: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} \u0434\u043e\u043b\u0436\u0435\u043d \u0441\u043e\u0434\u0435\u0440\u0436\u0430\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 ''test''. "},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "\u041d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 \u0443\u0440\u043e\u0432\u043d\u044f: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "\u0418\u043c\u044f processing-instruction \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u0440\u0430\u0432\u043d\u043e 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "\u0418\u043c\u044f processing-instruction \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c NCName: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "\u0415\u0441\u043b\u0438 \u0434\u043b\u044f {0} \u043e\u043f\u0440\u0435\u0434\u0435\u043b\u0435\u043d \u0440\u0435\u0436\u0438\u043c, \u0442\u043e \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 match."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "\u0423 {0} \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 name \u0438\u043b\u0438 match."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "\u0412 xml:space \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435: {0}"},
				new object[] {ER_NO_OWNERDOC, "\u0423 \u0434\u043e\u0447\u0435\u0440\u043d\u0435\u0433\u043e \u0443\u0437\u043b\u0430 \u043d\u0435\u0442 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430-\u0432\u043b\u0430\u0434\u0435\u043b\u044c\u0446\u0430!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "\u041e\u0448\u0438\u0431\u043a\u0430 ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0434\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u043f\u0443\u0441\u0442\u043e\u0433\u043e \u043f\u043e\u0442\u043e\u043c\u043a\u0430!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "\u0423 {0} \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 select."},
				new object[] {ER_NEED_TEST_ATTRIB, "\u0414\u043b\u044f xsl:when \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0437\u0430\u0434\u0430\u043d \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "\u0414\u043b\u044f xsl:with-param \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0437\u0430\u0434\u0430\u043d \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "\u0412 \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442\u0435 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442-\u0432\u043b\u0430\u0434\u0435\u043b\u0435\u0446!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0441\u043e\u0437\u0434\u0430\u0442\u044c XML TransformerFactory Liaison: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: \u0412 \u043f\u0440\u043e\u0446\u0435\u0441\u0441\u0435 \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u044b \u043e\u0448\u0438\u0431\u043a\u0438."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: \u041e\u0448\u0438\u0431\u043a\u0430."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "\u041a\u043e\u0434\u0438\u0440\u043e\u0432\u043a\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0441\u043e\u0437\u0434\u0430\u0442\u044c TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "\u0414\u043b\u044f xsl:key \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'name'!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "\u0414\u043b\u044f xsl:key \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'match'!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "\u0414\u043b\u044f xsl:key \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'use'!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "\u0414\u043b\u044f (StylesheetHandler) {0} \u0442\u0440\u0435\u0431\u0443\u0435\u0442\u0441\u044f \u0430\u0442\u0440\u0438\u0431\u0443\u0442 ''elements''!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "\u0414\u043b\u044f (StylesheetHandler) {0} \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0430\u0442\u0440\u0438\u0431\u0443\u0442 ''prefix''"},
				new object[] {ER_BAD_STYLESHEET_URL, "\u041d\u0435\u0432\u0435\u0440\u043d\u044b\u0439 URL \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d \u0444\u0430\u0439\u043b \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439: {0}"},
				new object[] {ER_IOEXCEPTION, "\u0418\u0441\u043a\u043b\u044e\u0447\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0438\u0442\u0443\u0430\u0446\u0438\u044f \u0432\u0432\u043e\u0434\u0430-\u0432\u044b\u0432\u043e\u0434\u0430 \u043f\u0440\u0438 \u043e\u0431\u0440\u0430\u0449\u0435\u043d\u0438\u0438 \u043a \u0444\u0430\u0439\u043b\u0443 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) \u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d \u0430\u0442\u0440\u0438\u0431\u0443\u0442 href \u0434\u043b\u044f {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} \u043d\u0435\u043f\u043e\u0441\u0440\u0435\u0434\u0441\u0442\u0432\u0435\u043d\u043d\u043e \u0438\u043b\u0438 \u043a\u043e\u0441\u0432\u0435\u043d\u043d\u043e \u0441\u0441\u044b\u043b\u0430\u0435\u0442\u0441\u044f \u043d\u0430 \u0441\u0435\u0431\u044f!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "\u0414\u043b\u044f (StylesheetHandler) {0} \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0430\u0442\u0440\u0438\u0431\u0443\u0442 ''lang''"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) \u041d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 {0} ?? \u041e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u044d\u043b\u0435\u043c\u0435\u043d\u0442-\u043a\u043e\u043d\u0442\u0435\u0439\u043d\u0435\u0440 ''component''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "\u0412\u044b\u0432\u043e\u0434 \u0432\u043e\u0437\u043c\u043e\u0436\u0435\u043d \u0442\u043e\u043b\u044c\u043a\u043e \u0432 \u0441\u043b\u0435\u0434\u0443\u044e\u0449\u0438\u0435 \u043e\u0431\u044a\u0435\u043a\u0442\u044b: Element, DocumentFragment, Document \u0438\u043b\u0438 PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "\u041e\u0448\u0438\u0431\u043a\u0430! \u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u043e \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0435 \u0432\u044b\u0431\u043e\u0440\u0430 xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u0435\u0440\u0438\u0430\u043b\u0438\u0437\u043e\u0432\u0430\u0442\u044c XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "\u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d\u0430 \u0438\u0441\u0445\u043e\u0434\u043d\u0430\u044f \u0442\u0430\u0431\u043b\u0438\u0446\u0430 \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "\u041e\u0448\u0438\u0431\u043a\u0430 \u043f\u0440\u0438 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u043a\u0435 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_COULDNT_PARSE_DOC, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0440\u043e\u0430\u043d\u0430\u043b\u0438\u0437\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442 {0} !"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d \u0444\u0440\u0430\u0433\u043c\u0435\u043d\u0442: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "\u0423\u0437\u0435\u043b, \u043d\u0430 \u043a\u043e\u0442\u043e\u0440\u044b\u0439 \u0441\u0441\u044b\u043b\u0430\u0435\u0442\u0441\u044f \u0438\u0434\u0435\u043d\u0442\u0438\u0444\u0438\u043a\u0430\u0442\u043e\u0440 \u0444\u0440\u0430\u0433\u043c\u0435\u043d\u0442\u0430, \u043d\u0435 \u044f\u0432\u043b\u044f\u0435\u0442\u0441\u044f \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u043c: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "\u0423 for-each \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 match \u0438\u043b\u0438 name"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "\u0423 templates \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 match \u0438\u043b\u0438 name"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "\u041e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u043a\u043e\u043f\u0438\u044f \u0444\u0440\u0430\u0433\u043c\u0435\u043d\u0442\u0430 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430!"},
				new object[] {ER_CANT_CREATE_ITEM, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442 \u0434\u0435\u0440\u0435\u0432\u0430 \u0440\u0435\u0437\u0443\u043b\u044c\u0442\u0430\u0442\u043e\u0432: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "\u0417\u0430\u0434\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 xml:space \u0432 \u0438\u0441\u0445\u043e\u0434\u043d\u043e\u043c XML: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "\u041e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 xsl:key \u0434\u043b\u044f {0}!"},
				new object[] {ER_CANT_CREATE_URL, "\u041e\u0448\u0438\u0431\u043a\u0430! \u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c URL \u0434\u043b\u044f {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f"},
				new object[] {ER_PROCESSOR_ERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c \u0432 \u0442\u0430\u0431\u043b\u0438\u0446\u0435 \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns \u0431\u043e\u043b\u044c\u0448\u0435 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f!  \u0418\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space \u0431\u043e\u043b\u044c\u0448\u0435 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f!  \u0418\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 xsl:strip-space \u0438\u043b\u0438 xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result \u0431\u043e\u043b\u044c\u0448\u0435 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f!  \u0418\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) \u0414\u043b\u044f {0} \u0443\u043a\u0430\u0437\u0430\u043d \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0430\u0442\u0440\u0438\u0431\u0443\u0442: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u044d\u043b\u0435\u043c\u0435\u043d\u0442 XSL: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort \u043c\u043e\u0436\u0435\u0442 \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c\u0441\u044f \u0442\u043e\u043b\u044c\u043a\u043e \u0441 xsl:apply-templates \u0438\u043b\u0438 xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) \u041d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u0435 xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when \u043d\u0435 \u0438\u043c\u0435\u0435\u0442 \u043f\u0440\u0435\u0434\u0448\u0435\u0441\u0442\u0432\u0443\u044e\u0449\u0435\u0433\u043e xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) \u041d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u0435 xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise \u043d\u0435 \u0438\u043c\u0435\u0435\u0442 \u043f\u0440\u0435\u0434\u0448\u0435\u0441\u0442\u0432\u0443\u044e\u0449\u0435\u0433\u043e xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c \u0432 \u0448\u0430\u0431\u043b\u043e\u043d\u0435!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) \u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u043f\u0440\u0435\u0444\u0438\u043a\u0441 {1} \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f {0}"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Imports \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c \u0442\u043e\u043b\u044c\u043a\u043e \u0432 \u043a\u0430\u0447\u0435\u0441\u0442\u0432\u0435 \u043f\u0435\u0440\u0432\u043e\u0433\u043e \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} \u043d\u0435\u043f\u043e\u0441\u0440\u0435\u0434\u0441\u0442\u0432\u0435\u043d\u043d\u043e \u0438\u043b\u0438 \u043a\u043e\u0441\u0432\u0435\u043d\u043d\u043e \u0438\u043c\u043f\u043e\u0440\u0442\u0438\u0440\u0443\u0435\u0442 \u0441\u0435\u0431\u044f!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) \u0414\u043b\u044f xml:space \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "\u041e\u0448\u0438\u0431\u043a\u0430 processStylesheet!"},
				new object[] {ER_SAX_EXCEPTION, "\u0418\u0441\u043a\u043b\u044e\u0447\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0438\u0442\u0443\u0430\u0446\u0438\u044f SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f!"},
				new object[] {ER_XSLT_ERROR, "\u041e\u0448\u0438\u0431\u043a\u0430 XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "\u0421\u0438\u043c\u0432\u043e\u043b \u0434\u0435\u043d\u0435\u0436\u043d\u043e\u0439 \u0435\u0434\u0438\u043d\u0438\u0446\u044b \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c \u0432 \u0441\u0442\u0440\u043e\u043a\u0435 \u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u044f \u0448\u0430\u0431\u043b\u043e\u043d\u0430"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 DOM \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u044f non-Prefix!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "\u0420\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0435 \u043f\u0435\u0440\u0435\u043d\u0430\u043f\u0440\u0430\u0432\u043b\u0435\u043d\u0438\u044f: \u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u043e\u043b\u0443\u0447\u0438\u0442\u044c \u0438\u043c\u044f \u0444\u0430\u0439\u043b\u0430 - \u0430\u0442\u0440\u0438\u0431\u0443\u0442 file \u0438\u043b\u0438 select \u0434\u043e\u043b\u0436\u0435\u043d \u0432\u043e\u0437\u0432\u0440\u0430\u0449\u0430\u0442\u044c \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u0443\u044e \u0441\u0442\u0440\u043e\u043a\u0443."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c FormatterListener \u0432 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0438 \u043f\u0435\u0440\u0435\u043d\u0430\u043f\u0440\u0430\u0432\u043b\u0435\u043d\u0438\u044f!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u0432 exclude-result-prefixes: {0}"},
				new object[] {ER_MISSING_NS_URI, "\u0414\u043b\u044f \u0443\u043a\u0430\u0437\u0430\u043d\u043d\u043e\u0433\u043e \u043f\u0440\u0435\u0444\u0438\u043a\u0441\u0430 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 URI \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "\u041e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442 \u043e\u043f\u0446\u0438\u0438: {0}"},
				new object[] {ER_INVALID_OPTION, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u0430\u044f \u043e\u043f\u0446\u0438\u044f: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "\u041d\u0435\u043f\u0440\u0430\u0432\u0438\u043b\u044c\u043d\u0430\u044f \u0441\u0442\u0440\u043e\u043a\u0430 \u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u044f: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "\u0414\u043b\u044f xsl:stylesheet \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'version'!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "\u0410\u0442\u0440\u0438\u0431\u0443\u0442: \u0412 {0} \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "\u0414\u043b\u044f xsl:choose \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports \u043d\u0435 \u0434\u043e\u043f\u0443\u0441\u043a\u0430\u0435\u0442\u0441\u044f \u0432 xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "\u041d\u0435\u043b\u044c\u0437\u044f \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c DTMLiaison \u0434\u043b\u044f \u0443\u0437\u043b\u0430 \u0432\u044b\u0432\u043e\u0434\u0430 DOM ... \u0418\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "\u041d\u0435\u043b\u044c\u0437\u044f \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c DTMLiaison \u0434\u043b\u044f \u0443\u0437\u043b\u0430 \u0432\u0432\u043e\u0434\u0430 DOM ... \u0418\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "\u041e\u0448\u0438\u0431\u043a\u0430 \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u041f\u0440\u0435\u0444\u0438\u043a\u0441 \u0434\u043e\u043b\u0436\u0435\u043d \u043e\u0431\u0435\u0441\u043f\u0435\u0447\u0438\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435 \u0432 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u043e \u0438\u043c\u0435\u043d: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u041e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 UTF-16: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0435\u0442 \u0441\u0435\u0431\u044f, \u0447\u0442\u043e \u043f\u0440\u0438\u0432\u0435\u0434\u0435\u0442 \u043a \u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044e \u0431\u0435\u0441\u043a\u043e\u043d\u0435\u0447\u043d\u043e\u0433\u043e \u0446\u0438\u043a\u043b\u0430."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "\u041d\u0435\u043b\u044c\u0437\u044f \u043e\u0434\u043d\u043e\u0432\u0440\u0435\u043c\u0435\u043d\u043d\u043e \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0432\u0432\u043e\u0434 \u043d\u0435-Xerces-DOM \u0438 \u0432\u044b\u0432\u043e\u0434 Xerces-DOM!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "\u0412 ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "\u041e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u043e \u043d\u0435\u0441\u043a\u043e\u043b\u044c\u043a\u043e \u0448\u0430\u0431\u043b\u043e\u043d\u043e\u0432 \u0441 \u0437\u0430\u0434\u0430\u043d\u043d\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0432\u044b\u0437\u043e\u0432 \u0444\u0443\u043d\u043a\u0446\u0438\u0438: \u0440\u0435\u043a\u0443\u0440\u0441\u0438\u0432\u043d\u044b\u0435 \u0432\u044b\u0437\u043e\u0432\u044b key() \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b"},
				new object[] {ER_REFERENCING_ITSELF, "\u041f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u0430\u044f {0} \u043d\u0435\u043f\u043e\u0441\u0440\u0435\u0434\u0441\u0442\u0432\u0435\u043d\u043d\u043e \u0438\u043b\u0438 \u043a\u043e\u0441\u0432\u0435\u043d\u043d\u043e \u0441\u0441\u044b\u043b\u0430\u0435\u0442\u0441\u044f \u043d\u0430 \u0441\u0435\u0431\u044f!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "\u0414\u043b\u044f DOMSource \u0432 newTemplates \u0443\u0437\u0435\u043b \u0432\u0432\u043e\u0434\u0430 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d \u0444\u0430\u0439\u043b \u043a\u043b\u0430\u0441\u0441\u0430 \u0434\u043b\u044f \u043e\u043f\u0446\u0438\u0438 {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d \u043e\u0431\u044f\u0437\u0430\u0442\u0435\u043b\u044c\u043d\u044b\u0439 \u044d\u043b\u0435\u043c\u0435\u043d\u0442: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "\u0424\u0430\u0439\u043b \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0438\u043d\u0438\u0446\u0438\u0430\u043b\u0438\u0437\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0430\u0434\u043c\u0438\u043d\u0438\u0441\u0442\u0440\u0430\u0442\u043e\u0440 BSF"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043e\u0442\u043a\u043e\u043c\u043f\u0438\u043b\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0435"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0435: {0}, \u043f\u0440\u0438\u0447\u0438\u043d\u0430: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "\u041f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 \u0432 \u044d\u043a\u0437\u0435\u043c\u043f\u043b\u044f\u0440\u0435 \u043c\u0435\u0442\u043e\u0434\u0430 {0} \u0432 \u043f\u0435\u0440\u0432\u043e\u043c \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u0435 \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043f\u0435\u0440\u0435\u0434\u0430\u043d \u044d\u043a\u0437\u0435\u043c\u043f\u043b\u044f\u0440 \u043e\u0431\u044a\u0435\u043a\u0442\u0430"},
				new object[] {ER_INVALID_ELEMENT_NAME, "\u0423\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0438\u043c\u044f \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "\u041c\u0435\u0442\u043e\u0434 name \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0441\u0442\u0430\u0442\u0438\u0447\u0435\u0441\u043a\u0438\u043c {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0430\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u044f \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f {0} : {1}"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "\u041d\u0435\u0441\u043a\u043e\u043b\u044c\u043a\u043e \u043d\u0430\u0438\u043b\u0443\u0447\u0448\u0438\u0445 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u0439 \u0434\u043b\u044f \u043a\u043e\u043d\u0441\u0442\u0440\u0443\u043a\u0442\u043e\u0440\u0430 {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "\u041d\u0435\u0441\u043a\u043e\u043b\u044c\u043a\u043e \u043b\u0443\u0447\u0448\u0438\u0445 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u0439 \u0434\u043b\u044f \u043c\u0435\u0442\u043e\u0434\u0430 {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "\u041d\u0435\u0441\u043a\u043e\u043b\u044c\u043a\u043e \u043b\u0443\u0447\u0448\u0438\u0445 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u0439 \u0434\u043b\u044f \u043c\u0435\u0442\u043e\u0434\u0430 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "\u0414\u043b\u044f \u0432\u044b\u0447\u0438\u0441\u043b\u0435\u043d\u0438\u044f \u043f\u0435\u0440\u0435\u0434\u0430\u043d \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u043a\u043e\u043d\u0442\u0435\u043a\u0441\u0442 {0}"},
				new object[] {ER_POOL_EXISTS, "\u041f\u0443\u043b \u0443\u0436\u0435 \u0441\u0443\u0449\u0435\u0441\u0442\u0432\u0443\u0435\u0442"},
				new object[] {ER_NO_DRIVER_NAME, "\u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u0438\u043c\u044f \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430"},
				new object[] {ER_NO_URL, "\u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d URL"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "\u0420\u0430\u0437\u043c\u0435\u0440 \u043f\u0443\u043b\u0430 \u043c\u0435\u043d\u044c\u0448\u0435 \u0435\u0434\u0438\u043d\u0438\u0446\u044b!"},
				new object[] {ER_INVALID_DRIVER, "\u0423\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0438\u043c\u044f \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430!"},
				new object[] {ER_NO_STYLESHEETROOT, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d \u043a\u043e\u0440\u043d\u0435\u0432\u043e\u0439 \u044d\u043b\u0435\u043c\u0435\u043d\u0442 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "\u041e\u0448\u0438\u0431\u043a\u0430 processFromNode"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0437\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c \u0440\u0435\u0441\u0443\u0440\u0441 [ {0} ]: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "\u0420\u0430\u0437\u043c\u0435\u0440 \u0431\u0443\u0444\u0435\u0440\u0430 <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u0430\u044f \u043e\u0448\u0438\u0431\u043a\u0430 \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f"},
				new object[] {ER_NO_NAMESPACE_DECL, "\u0423 \u043f\u0440\u0435\u0444\u0438\u043a\u0441\u0430 {0} \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0443\u044e\u0449\u0435\u0433\u043e \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "\u0421\u043e\u0434\u0435\u0440\u0436\u0438\u043c\u043e\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e \u0434\u043b\u044f lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "\u041f\u0440\u0435\u0440\u0432\u0430\u043d\u043e \u0432 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u0438 \u0441 \u0442\u0430\u0431\u043b\u0438\u0446\u0435\u0439 \u0441\u0442\u0438\u043b\u0435\u0439"},
				new object[] {ER_ONE_OR_TWO, "1 \u0438\u043b\u0438 2"},
				new object[] {ER_TWO_OR_THREE, "2 \u0438\u043b\u0438 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0437\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c {0} (\u043f\u0440\u043e\u0432\u0435\u0440\u044c\u0442\u0435 \u043d\u0430\u0441\u0442\u0440\u043e\u0439\u043a\u0443 CLASSPATH), \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u044e\u0442\u0441\u044f \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f \u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0438\u043d\u0438\u0446\u0438\u0430\u043b\u0438\u0437\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0448\u0430\u0431\u043b\u043e\u043d\u044b \u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e"},
				new object[] {ER_RESULT_NULL, "\u0420\u0435\u0437\u0443\u043b\u044c\u0442\u0430\u0442 \u043d\u0435 \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u0440\u0435\u0437\u0443\u043b\u044c\u0442\u0430\u0442"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "\u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d \u0432\u044b\u0432\u043e\u0434"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0420\u0435\u0437\u0443\u043b\u044c\u0442\u0430\u0442 \u0442\u0438\u043f\u0430 {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0432 \u0418\u0441\u0442\u043e\u0447\u043d\u0438\u043a \u0442\u0438\u043f\u0430 {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "\u041f\u0443\u0441\u0442\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0447\u0438\u043a \u0441\u043e\u0434\u0435\u0440\u0436\u0430\u043d\u0438\u044f"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u041f\u0443\u0441\u0442\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0447\u0438\u043a \u043e\u0448\u0438\u0431\u043a\u0438"},
				new object[] {ER_CANNOT_CALL_PARSE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0437\u0432\u0430\u0442\u044c \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440, \u0435\u0441\u043b\u0438 \u043d\u0435 \u0437\u0430\u0434\u0430\u043d ContentHandler"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "\u041d\u0435 \u0437\u0430\u0434\u0430\u043d \u0440\u043e\u0434\u0438\u0442\u0435\u043b\u044c\u0441\u043a\u0438\u0439 \u043e\u0431\u044a\u0435\u043a\u0442 \u0444\u0438\u043b\u044c\u0442\u0440\u0430"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "\u0412 {0} \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430 \u0442\u0430\u0431\u043b\u0438\u0446\u0430 \u0441\u0442\u0438\u043b\u0435\u0439, \u043d\u043e\u0441\u0438\u0442\u0435\u043b\u044c={1}"},
				new object[] {ER_NO_STYLESHEET_PI, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d PI xml-stylesheet \u0432 {0}"},
				new object[] {ER_NOT_SUPPORTED, "\u041d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0441\u0432\u043e\u0439\u0441\u0442\u0432\u0430 {0} \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u044d\u043a\u0437\u0435\u043c\u043f\u043b\u044f\u0440\u043e\u043c Boolean"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u043e\u043b\u0443\u0447\u0438\u0442\u044c \u0432\u043d\u0435\u0448\u043d\u0438\u0439 \u0441\u0446\u0435\u043d\u0430\u0440\u0438\u0439 \u0432 {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "\u0420\u0435\u0441\u0443\u0440\u0441 [ {0} ] \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "\u0421\u0432\u043e\u0439\u0441\u0442\u0432\u043e \u0432\u044b\u0432\u043e\u0434\u0430 \u043d\u0435 \u0440\u0430\u0441\u043f\u043e\u0437\u043d\u0430\u043d\u043e: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u044d\u043b\u0435\u043c\u0435\u043d\u0442 ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {0} \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0430\u043d\u0430\u043b\u0438\u0437\u0438\u0440\u0443\u0435\u043c\u044b\u043c \u0447\u0438\u0441\u043b\u043e\u043c"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "\u0412 {0} \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 yes \u0438\u043b\u0438 no"},
				new object[] {ER_FAILED_CALLING_METHOD, "\u041e\u0448\u0438\u0431\u043a\u0430 \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 \u043c\u0435\u0442\u043e\u0434\u0430 {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "\u041e\u0448\u0438\u0431\u043a\u0430 \u043f\u0440\u0438 \u0441\u043e\u0437\u0434\u0430\u043d\u0438\u0438 \u044d\u043a\u0437\u0435\u043c\u043f\u043b\u044f\u0440\u0430 ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "\u0421\u0438\u043c\u0432\u043e\u043b\u044b \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b \u0432 \u0434\u0430\u043d\u043d\u043e\u0439 \u043f\u043e\u0437\u0438\u0446\u0438\u0438 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430"},
				new object[] {ER_ATTR_NOT_ALLOWED, "\u0410\u0442\u0440\u0438\u0431\u0443\u0442 \"{0}\" \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c \u0432 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0435 {1}!"},
				new object[] {ER_BAD_VALUE, "{0} \u043d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 {0} "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 {0} \u043d\u0435 \u0440\u0430\u0441\u043f\u043e\u0437\u043d\u0430\u043d\u043e "},
				new object[] {ER_NULL_URI_NAMESPACE, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d \u0441 \u043f\u0443\u0441\u0442\u044b\u043c URI"},
				new object[] {ER_NUMBER_TOO_BIG, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u043e\u0442\u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0447\u0438\u0441\u043b\u043e \u0431\u043e\u043b\u044c\u0448\u0435 \u043c\u0430\u043a\u0441\u0438\u043c\u0430\u043b\u044c\u043d\u043e \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0433\u043e LongInteger"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043d\u0430\u0439\u0442\u0438 \u043a\u043b\u0430\u0441\u0441 \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430 SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "\u041a\u043b\u0430\u0441\u0441 \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430 SAX1 {0} \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d, \u043d\u043e \u0435\u0433\u043e \u0437\u0430\u0433\u0440\u0443\u0437\u043a\u0430 \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u0430"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "\u041a\u043b\u0430\u0441\u0441 \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430 SAX1 {0} \u0437\u0430\u0433\u0440\u0443\u0436\u0435\u043d, \u043d\u043e \u0435\u0433\u043e \u0438\u043d\u0438\u0446\u0438\u0430\u043b\u0438\u0437\u0430\u0446\u0438\u044f \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u0430"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "\u0412 \u043a\u043b\u0430\u0441\u0441\u0435 \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430 SAX1 {0} \u043d\u0435 \u0440\u0435\u0430\u043b\u0438\u0437\u043e\u0432\u0430\u043d org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "\u041d\u0435 \u0437\u0430\u0434\u0430\u043d\u043e \u0441\u0438\u0441\u0442\u0435\u043c\u043d\u043e\u0435 \u0441\u0432\u043e\u0439\u0441\u0442\u0432\u043e org.xml.sax.parser"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "\u0410\u0440\u0433\u0443\u043c\u0435\u043d\u0442 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440\u0430 \u043d\u0435 \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_FEATURE, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f: {0}"},
				new object[] {ER_PROPERTY, "\u0421\u0432\u043e\u0439\u0441\u0442\u0432\u043e: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "\u041f\u0443\u0441\u0442\u043e\u0439 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u044c \u043c\u0430\u043a\u0440\u043e\u0441\u0430"},
				new object[] {ER_NULL_DTD_HANDLER, "\u041f\u0443\u0441\u0442\u043e\u0439 \u0434\u0435\u0441\u043a\u0440\u0438\u043f\u0442\u043e\u0440 DTD"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "\u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u0438\u043c\u044f \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430!"},
				new object[] {ER_NO_URL_SPECIFIED, "\u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d URL!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "\u0420\u0430\u0437\u043c\u0435\u0440 \u043f\u0443\u043b\u0430 \u043c\u0435\u043d\u044c\u0448\u0435 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "\u0423\u043a\u0430\u0437\u0430\u043d\u043e \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0438\u043c\u044f \u0434\u0440\u0430\u0439\u0432\u0435\u0440\u0430!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "\u041f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u043d\u0430\u044f \u043e\u0448\u0438\u0431\u043a\u0430! \u0423 \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u044f \u043d\u0435\u0442 \u0440\u043e\u0434\u0438\u0442\u0435\u043b\u044c\u0441\u043a\u043e\u0433\u043e \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 ElemTemplateElement!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "\u0417\u0430\u043f\u0438\u0441\u044c \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u0438\u0441\u0442\u0430 \u0432 RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e \u0432 \u0434\u0430\u043d\u043d\u043e\u0439 \u043f\u043e\u0437\u0438\u0446\u0438\u0438 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "\u0422\u0435\u043a\u0441\u0442 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c \u0432 \u0434\u0430\u043d\u043d\u043e\u0439 \u043f\u043e\u0437\u0438\u0446\u0438\u0438 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439!"},
				new object[] {INVALID_TCHAR, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 CHAR: {0}.  \u0410\u0442\u0440\u0438\u0431\u0443\u0442 \u0442\u0438\u043f\u0430 CHAR \u0434\u043e\u043b\u0436\u0435\u043d \u0441\u043e\u0434\u0435\u0440\u0436\u0430\u0442\u044c \u0442\u043e\u043b\u044c\u043a\u043e 1 \u0441\u0438\u043c\u0432\u043e\u043b!"},
				new object[] {INVALID_QNAME, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 QNAME: {0}"},
				new object[] {INVALID_ENUM, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 ENUM: {0}.  \u0414\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f: {2}."},
				new object[] {INVALID_NMTOKEN, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 NMTOKEN: {0}. "},
				new object[] {INVALID_NCNAME, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 NCNAME: {0}. "},
				new object[] {INVALID_BOOLEAN, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 boolean: {0}. "},
				new object[] {INVALID_NUMBER, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 {1} \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 number: {0}. "},
				new object[] {ER_ARG_LITERAL, "\u0410\u0440\u0433\u0443\u043c\u0435\u043d\u0442 {0} \u0432 \u0448\u0430\u0431\u043b\u043e\u043d\u0435 \u0441\u0440\u0430\u0432\u043d\u0435\u043d\u0438\u044f \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043b\u0438\u0442\u0435\u0440\u0430\u043b\u043e\u043c."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "\u041f\u043e\u0432\u0442\u043e\u0440\u043d\u043e\u0435 \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u0433\u043b\u043e\u0431\u0430\u043b\u044c\u043d\u043e\u0439 \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u043e\u0439."},
				new object[] {ER_DUPLICATE_VAR, "\u041f\u043e\u0432\u0442\u043e\u0440\u043d\u043e\u0435 \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u043f\u0435\u0440\u0435\u043c\u0435\u043d\u043d\u043e\u0439."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "\u0414\u043b\u044f xsl:template \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0437\u0430\u0434\u0430\u043d \u0430\u0442\u0440\u0438\u0431\u0443\u0442 name \u0438\u043b\u0438 match, \u043b\u0438\u0431\u043e \u043e\u0431\u0430 \u044d\u0442\u0438\u0445 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430"},
				new object[] {ER_INVALID_PREFIX, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u0432 exclude-result-prefixes: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "attribute-set \u0441 \u0438\u043c\u0435\u043d\u0435\u043c {0} \u043d\u0435 \u0441\u0443\u0449\u0435\u0441\u0442\u0432\u0443\u0435\u0442"},
				new object[] {ER_FUNCTION_NOT_FOUND, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u0441 \u0438\u043c\u0435\u043d\u0435\u043c {0} \u043d\u0435 \u0441\u0443\u0449\u0435\u0441\u0442\u0432\u0443\u0435\u0442"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "\u0414\u043b\u044f \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 {0} \u043d\u0435\u043b\u044c\u0437\u044f \u043e\u0434\u043d\u043e\u0432\u0440\u0435\u043c\u0435\u043d\u043d\u043e \u0443\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u044b \u0441\u043e\u0434\u0435\u0440\u0436\u0430\u043d\u0438\u044f \u0438 \u0432\u044b\u0431\u043e\u0440\u0430. "},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u043f\u0430\u0440\u0430\u043c\u0435\u0442\u0440\u0430 {0} \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u043c \u043e\u0431\u044a\u0435\u043a\u0442\u043e\u043c Java"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "\u0410\u0442\u0440\u0438\u0431\u0443\u0442 result-prefix \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 xsl:namespace-alias \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u0442 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 '#default', \u043d\u043e \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d \u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e \u0432 \u043e\u0431\u043b\u0430\u0441\u0442\u0438 \u0434\u043b\u044f \u0434\u0430\u043d\u043d\u043e\u0433\u043e \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u043e"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "\u0410\u0442\u0440\u0438\u0431\u0443\u0442 result-prefix \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 xsl:namespace-alias \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u0442 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 ''{0}'', \u043d\u043e \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d \u0434\u043b\u044f \u043f\u0440\u0435\u0444\u0438\u043a\u0441\u0430 ''{0}'' \u0432 \u043e\u0431\u043b\u0430\u0441\u0442\u0438 \u0434\u0430\u043d\u043d\u043e\u0433\u043e \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u043e. "},
				new object[] {ER_SET_FEATURE_NULL_NAME, "\u0418\u043c\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c \u0432 TransformerFactory.setFeature(\u0418\u043c\u044f \u0441\u0442\u0440\u043e\u043a\u0438, \u0431\u0443\u043b\u0435\u0432\u0441\u043a\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435). "},
				new object[] {ER_GET_FEATURE_NULL_NAME, "\u0418\u043c\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c \u0432 TransformerFactory.getFeature(\u0418\u043c\u044f \u0441\u0442\u0440\u043e\u043a\u0438). "},
				new object[] {ER_UNSUPPORTED_FEATURE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u0444\u0443\u043d\u043a\u0446\u0438\u044e ''{0}'' \u0432 \u044d\u0442\u043e\u043c \u043a\u043b\u0430\u0441\u0441\u0435 TransformerFactory. "},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "\u041f\u0440\u0438\u043c\u0435\u043d\u0435\u043d\u0438\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f ''{0}'' \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e, \u0435\u0441\u043b\u0438 \u0434\u043b\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0437\u0430\u0449\u0438\u0449\u0435\u043d\u043d\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u043a\u0438 \u0437\u0430\u0434\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 true. "},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043d\u0430\u0439\u0442\u0438 \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u0434\u043b\u044f uri \u043f\u0443\u0441\u0442\u043e\u0433\u043e \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d. "},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043d\u0430\u0439\u0442\u0438 uri \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d \u0434\u043b\u044f \u043f\u0443\u0441\u0442\u043e\u0433\u043e \u043f\u0440\u0435\u0444\u0438\u043a\u0441\u0430. "},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "\u0418\u043c\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043f\u0443\u0441\u0442\u044b\u043c. "},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "\u0427\u0438\u0441\u043b\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u043e\u0432 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u043e\u0442\u0440\u0438\u0446\u0430\u0442\u0435\u043b\u044c\u043d\u044b\u043c. "},
				new object[] {WG_FOUND_CURLYBRACE, "\u041d\u0430\u0439\u0434\u0435\u043d\u0430 \u0437\u0430\u043a\u0440\u044b\u0432\u0430\u044e\u0449\u0430\u044f \u0441\u043a\u043e\u0431\u043a\u0430 '}', \u043d\u043e \u043e\u0442\u043a\u0440\u044b\u0442\u044b\u0435 \u0448\u0430\u0431\u043b\u043e\u043d\u044b \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u043e\u0432 \u043e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u044e\u0442!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "\u041f\u0440\u0435\u0434\u0443\u043f\u0440\u0435\u0436\u0434\u0435\u043d\u0438\u0435: \u0410\u0442\u0440\u0438\u0431\u0443\u0442 count \u043d\u0435 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0443\u0435\u0442 \u0440\u043e\u0434\u0438\u0442\u0435\u043b\u044c\u0441\u043a\u043e\u043c\u0443 \u0432 xsl:number! \u0426\u0435\u043b\u0435\u0432\u043e\u0439 = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "\u0421\u0442\u0430\u0440\u044b\u0439 \u0441\u0438\u043d\u0442\u0430\u043a\u0441\u0438\u0441: \u0418\u043c\u044f \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 'expr' \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u043e \u043d\u0430 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan \u0435\u0449\u0435 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u043e\u0431\u0440\u0430\u0431\u0430\u0442\u044b\u0432\u0430\u0442\u044c \u0438\u043c\u044f \u043b\u043e\u043a\u0430\u043b\u0438 \u0432 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "\u041f\u0440\u0435\u0434\u0443\u043f\u0440\u0435\u0436\u0434\u0435\u043d\u0438\u0435: \u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430 \u043b\u043e\u043a\u0430\u043b\u044c \u0434\u043b\u044f xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0441\u043e\u0437\u0434\u0430\u0442\u044c URL \u0438\u0437: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0437\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c \u0437\u0430\u043f\u0440\u043e\u0448\u0435\u043d\u043d\u044b\u0439 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043d\u0430\u0439\u0442\u0438 Collator \u0434\u043b\u044f <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "\u0421\u0442\u0430\u0440\u044b\u0439 \u0441\u0438\u043d\u0442\u0430\u043a\u0441\u0438\u0441: \u0432 \u0438\u043d\u0441\u0442\u0440\u0443\u043a\u0446\u0438\u0438 functions \u043d\u0435 \u0441\u043b\u0435\u0434\u0443\u0435\u0442 \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c url {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "\u041a\u043e\u0434\u0438\u0440\u043e\u0432\u043a\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}, \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0435\u0442\u0441\u044f UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "\u041a\u043e\u0434\u0438\u0440\u043e\u0432\u043a\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}, \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0435\u0442\u0441\u044f Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "\u041e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d \u043a\u043e\u043d\u0444\u043b\u0438\u043a\u0442 \u0441\u043f\u0435\u0446\u0438\u0444\u0438\u043a\u0430\u0446\u0438\u0439: \u0411\u0443\u0434\u0435\u0442 \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c\u0441\u044f \u043f\u043e\u0441\u043b\u0435\u0434\u043d\u0438\u0439 \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u043d\u044b\u0439 \u0432 \u0442\u0430\u0431\u043b\u0438\u0446\u0435 \u0441\u0442\u0438\u043b\u0435\u0439 {0}."},
				new object[] {WG_PARSING_AND_PREPARING, "========= \u0410\u043d\u0430\u043b\u0438\u0437 \u0438 \u043f\u043e\u0434\u0433\u043e\u0442\u043e\u0432\u043a\u0430 {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "\u0428\u0430\u0431\u043b\u043e\u043d \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "\u041a\u043e\u043d\u0444\u043b\u0438\u043a\u0442 \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u044f xsl:strip-space \u0438 xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan \u0435\u0449\u0435 \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u043e\u0431\u0440\u0430\u0431\u0430\u0442\u044b\u0432\u0430\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "\u041d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u043e \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u0434\u0435\u0441\u044f\u0442\u0438\u0447\u043d\u043e\u0433\u043e \u0444\u043e\u0440\u043c\u0430\u0442\u0430: {0}"},
				new object[] {WG_OLD_XSLT_NS, "\u041e\u0442\u0441\u0443\u0442\u0441\u0442\u0432\u0443\u044e\u0449\u0435\u0435 \u0438\u043b\u0438 \u043d\u0435\u0432\u0435\u0440\u043d\u043e\u0435 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u043e \u0438\u043c\u0435\u043d XSLT. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "\u0414\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e \u0442\u043e\u043b\u044c\u043a\u043e \u043e\u0434\u043d\u043e \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 xsl:decimal-format \u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "\u0418\u043c\u0435\u043d\u0430 xsl:decimal-format \u0434\u043e\u043b\u0436\u043d\u044b \u0431\u044b\u0442\u044c \u0443\u043d\u0438\u043a\u0430\u043b\u044c\u043d\u044b\u043c\u0438. \u0418\u043c\u044f \"{0}\" \u043f\u043e\u0432\u0442\u043e\u0440\u044f\u0435\u0442\u0441\u044f."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "\u0414\u043b\u044f {0} \u0443\u043a\u0430\u0437\u0430\u043d \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0430\u0442\u0440\u0438\u0431\u0443\u0442: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u0444\u0438\u043a\u0441 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d: {0}. \u0423\u0437\u0435\u043b \u0431\u0443\u0434\u0435\u0442 \u043f\u0440\u043e\u0438\u0433\u043d\u043e\u0440\u0438\u0440\u043e\u0432\u0430\u043d."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "\u0414\u043b\u044f xsl:stylesheet \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 'version'!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0438\u043c\u044f \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "\u0420\u0435\u0437\u0443\u043b\u044c\u0442\u0438\u0440\u0443\u044e\u0449\u0438\u0439 \u043d\u0430\u0431\u043e\u0440 \u0443\u0437\u043b\u043e\u0432 \u0438\u0437 \u0432\u0442\u043e\u0440\u043e\u0433\u043e \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u0430 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 document \u043f\u0443\u0441\u0442. \u0412\u043e\u0437\u0432\u0440\u0430\u0442 \u043f\u0443\u0441\u0442\u043e\u0433\u043e node-set."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 'name' \u0432 \u0438\u043c\u0435\u043d\u0438 xsl:processing-instruction \u043d\u0435 \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0440\u0430\u0432\u043d\u043e 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "\u0417\u043d\u0430\u0447\u0435\u043d\u0438\u0435 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 ''name'' \u0432 xsl:processing-instruction \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c NCName: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0434\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 {0} \u043f\u043e\u0441\u043b\u0435 \u0434\u043e\u0447\u0435\u0440\u043d\u0438\u0445 \u0443\u0437\u043b\u043e\u0432 \u0438\u043b\u0438 \u043f\u0435\u0440\u0435\u0434 \u0441\u043e\u0437\u0434\u0430\u043d\u0438\u0435\u043c \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430.  \u0410\u0442\u0440\u0438\u0431\u0443\u0442 \u0431\u0443\u0434\u0435\u0442 \u043f\u0440\u043e\u0438\u0433\u043d\u043e\u0440\u0438\u0440\u043e\u0432\u0430\u043d. "},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u044f \u043e\u0431\u044a\u0435\u043a\u0442\u0430, \u0438\u0437\u043c\u0435\u043d\u0435\u043d\u0438\u0435 \u043a\u043e\u0442\u043e\u0440\u043e\u0433\u043e \u0437\u0430\u043f\u0440\u0435\u0449\u0435\u043d\u043e. "},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "\u041f\u0430\u0440\u0430\u043c\u0435\u0442\u0440 createMessage \u043b\u0435\u0436\u0438\u0442 \u0432\u043d\u0435 \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0433\u043e \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d\u0430"},
				new object[] {"FORMAT_FAILED", "\u0418\u0441\u043a\u043b\u044e\u0447\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0438\u0442\u0443\u0430\u0446\u0438\u044f \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 messageFormat"},
				new object[] {"version", ">>>>>>> \u0412\u0435\u0440\u0441\u0438\u044f Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u0434\u0430"},
				new object[] {"line", "\u041d\u043e\u043c\u0435\u0440 \u0441\u0442\u0440\u043e\u043a\u0438 "},
				new object[] {"column","\u041d\u043e\u043c\u0435\u0440 \u0441\u0442\u043e\u043b\u0431\u0446\u0430 "},
				new object[] {"xsldone", "XSLProcessor: \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d\u043e"},
				new object[] {"xslProc_option", "\u041e\u043f\u0446\u0438\u0438 \u043f\u0440\u043e\u0446\u0435\u0441\u0441\u0430 \u043a\u043e\u043c\u0430\u043d\u0434\u043d\u043e\u0439 \u0441\u0442\u0440\u043e\u043a\u0438 Xalan-J:"},
				new object[] {"xslProc_option", "\u041e\u043f\u0446\u0438\u0438 \u043f\u0440\u043e\u0446\u0435\u0441\u0441\u0430 \u043a\u043e\u043c\u0430\u043d\u0434\u043d\u043e\u0439 \u0441\u0442\u0440\u043e\u043a\u0438 Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "\u041e\u043f\u0446\u0438\u044f {0} \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f \u0432 \u0440\u0435\u0436\u0438\u043c\u0435 XSLTC."},
				new object[] {"xslProc_invalid_xalan_option", "\u041e\u043f\u0446\u0438\u044f {0} \u043c\u043e\u0436\u0435\u0442 \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c\u0441\u044f \u0442\u043e\u043b\u044c\u043a\u043e \u0441 -XSLTC."},
				new object[] {"xslProc_no_input", "\u041e\u0448\u0438\u0431\u043a\u0430: \u041d\u0435 \u0443\u043a\u0430\u0437\u0430\u043d\u0430 \u0442\u0430\u0431\u043b\u0438\u0446\u0430 \u0441\u0442\u0438\u043b\u0435\u0439 \u0438\u043b\u0438 \u0438\u0441\u0445\u043e\u0434\u043d\u044b\u0439 \u0444\u0430\u0439\u043b xml. \u0414\u043b\u044f \u043f\u0440\u043e\u0441\u043c\u043e\u0442\u0440\u0430 \u0441\u043f\u0440\u0430\u0432\u043a\u0438 \u043f\u043e \u0438\u0441\u043f\u043e\u043b\u044c\u0437\u043e\u0432\u0430\u043d\u0438\u044e \u043a\u043e\u043c\u0430\u043d\u0434\u044b \u0432\u0432\u0435\u0434\u0438\u0442\u0435 \u043a\u043e\u043c\u0430\u043d\u0434\u0443 \u0431\u0435\u0437 \u043f\u0430\u0440\u0430\u043c\u0435\u0442\u0440\u043e\u0432."},
				new object[] {"xslProc_common_options", "-\u041e\u0431\u0449\u0438\u0435 \u043e\u043f\u0446\u0438\u0438-"},
				new object[] {"xslProc_xalan_options", "-\u041e\u043f\u0446\u0438\u0438 Xalan-"},
				new object[] {"xslProc_xsltc_options", "-\u041e\u043f\u0446\u0438\u0438 XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(\u0414\u043b\u044f \u043f\u0440\u043e\u0434\u043e\u043b\u0436\u0435\u043d\u0438\u044f \u043d\u0430\u0436\u043c\u0438\u0442\u0435 <Enter>)"},
				new object[] {"optionXSLTC", "   [-XSLTC (\u0434\u043b\u044f \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f \u0438\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0439\u0442\u0435 XSLTC)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER \u043f\u043e\u043b\u043d\u043e\u0435 \u0438\u043c\u044f \u043a\u043b\u0430\u0441\u0441\u0430 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440\u0430]"},
				new object[] {"optionE", "   [-E (\u041d\u0435 \u0440\u0430\u0437\u0432\u043e\u0440\u0430\u0447\u0438\u0432\u0430\u0442\u044c \u0441\u0441\u044b\u043b\u043a\u0438 \u043d\u0430 \u043c\u0430\u043a\u0440\u043e\u0441\u044b entity)]"},
				new object[] {"optionV", "   [-E (\u041d\u0435 \u0440\u0430\u0437\u0432\u043e\u0440\u0430\u0447\u0438\u0432\u0430\u0442\u044c \u0441\u0441\u044b\u043b\u043a\u0438 \u043d\u0430 \u043c\u0430\u043a\u0440\u043e\u0441\u044b entity)]"},
				new object[] {"optionQC", "   [-QC (\u041d\u0435 \u043f\u043e\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u0434\u0443\u043f\u0440\u0435\u0436\u0434\u0435\u043d\u0438\u044f \u043e \u043a\u043e\u043d\u0444\u043b\u0438\u043a\u0442\u0430\u0445 \u0448\u0430\u0431\u043b\u043e\u043d\u043e\u0432)]"},
				new object[] {"optionQ", "   [-Q  (\u0422\u0438\u0445\u0438\u0439 \u0440\u0435\u0436\u0438\u043c)]"},
				new object[] {"optionLF", "   [-LF (\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0432 \u0432\u044b\u0432\u043e\u0434\u0435 \u0442\u043e\u043b\u044c\u043a\u043e LF {\u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e - CR/LF})]"},
				new object[] {"optionCR", "   [-CR (\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0432 \u0432\u044b\u0432\u043e\u0434\u0435 \u0442\u043e\u043b\u044c\u043a\u043e CR {\u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e - CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (\u0421\u0438\u043c\u0432\u043e\u043b\u044b, \u0442\u0440\u0435\u0431\u0443\u044e\u0449\u0438\u0435 Esc-\u043f\u043e\u0441\u043b\u0435\u0434\u043e\u0432\u0430\u0442\u0435\u043b\u044c\u043d\u043e\u0441\u0442\u0435\u0439 {\u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e - <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (\u0427\u0438\u0441\u043b\u043e \u043f\u0440\u043e\u0431\u0435\u043b\u043e\u0432 \u0432 \u043e\u0442\u0441\u0442\u0443\u043f\u0435 {\u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e - 0})]"},
				new object[] {"optionTT", "   [-TT (\u0422\u0440\u0430\u0441\u0441\u0438\u0440\u043e\u0432\u043a\u0430 \u0432\u044b\u0437\u044b\u0432\u0430\u0435\u043c\u044b\u0445 \u0448\u0430\u0431\u043b\u043e\u043d\u043e\u0432.)]"},
				new object[] {"optionTG", "   [-TG (\u0422\u0440\u0430\u0441\u0441\u0438\u0440\u043e\u0432\u043a\u0430 \u0441\u043e\u0431\u044b\u0442\u0438\u0439 \u0441\u043e\u0437\u0434\u0430\u043d\u0438\u044f.)]"},
				new object[] {"optionTS", "   [-TS (\u0422\u0440\u0430\u0441\u0441\u0438\u0440\u043e\u0432\u043a\u0430 \u0441\u043e\u0431\u044b\u0442\u0438\u0439 \u0432\u044b\u0431\u043e\u0440\u0430.)]"},
				new object[] {"optionTTC", "   [-TTC (\u0422\u0440\u0430\u0441\u0441\u0438\u0440\u043e\u0432\u043a\u0430 \u043e\u0431\u0440\u0430\u0431\u0430\u0442\u044b\u0432\u0430\u0435\u043c\u044b\u0445 \u0434\u043e\u0447\u0435\u0440\u043d\u0438\u0445 \u043e\u0431\u044a\u0435\u043a\u0442\u043e\u0432 \u0448\u0430\u0431\u043b\u043e\u043d\u043e\u0432.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (\u041a\u043b\u0430\u0441\u0441 TraceListener \u0434\u043b\u044f \u0442\u0440\u0430\u0441\u0441\u0438\u0440\u043e\u0432\u043a\u0430 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0439.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (\u0412\u043a\u043b\u044e\u0447\u0430\u0435\u0442 \u043f\u0440\u043e\u0432\u0435\u0440\u043a\u0443.  \u041f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e \u043f\u0440\u043e\u0432\u0435\u0440\u043a\u0430 \u0432\u044b\u043a\u043b\u044e\u0447\u0435\u043d\u0430.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {\u043d\u0435\u043e\u0431\u044f\u0437\u0430\u0442\u0435\u043b\u044c\u043d\u043e\u0435 \u0438\u043c\u044f \u0444\u0430\u0439\u043b\u0430} (\u0414\u0430\u043c\u043f \u0441\u0442\u0435\u043a\u0430 \u043f\u0440\u0438 \u043e\u0448\u0438\u0431\u043a\u0435.)]"},
				new object[] {"optionXML", "   [-XML (\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 XML \u0438 \u0434\u043e\u0431\u0430\u0432\u043b\u044f\u0442\u044c \u0437\u0430\u0433\u043e\u043b\u043e\u0432\u043e\u043a XML.)]"},
				new object[] {"optionTEXT", "   [-TEXT (\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0442\u0435\u043a\u0441\u0442\u043e\u0432\u043e\u0435 \u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435.)]"},
				new object[] {"optionHTML", "   [-HTML (\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 HTML.)]"},
				new object[] {"optionPARAM", "   [-PARAM \u0438\u043c\u044f \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0435 (\u0417\u0430\u0434\u0430\u0442\u044c \u043f\u0430\u0440\u0430\u043c\u0435\u0442\u0440 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439)]"},
				new object[] {"noParsermsg1", "\u0412 \u043f\u0440\u043e\u0446\u0435\u0441\u0441\u0435 XSL \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d\u044b \u043e\u0448\u0438\u0431\u043a\u0438."},
				new object[] {"noParsermsg2", "** \u0410\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d **"},
				new object[] {"noParsermsg3", "\u041f\u0440\u043e\u0432\u0435\u0440\u044c\u0442\u0435 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 classpath."},
				new object[] {"noParsermsg4", "\u0415\u0441\u043b\u0438 \u0443 \u0432\u0430\u0441 \u043d\u0435\u0442 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440\u0430 XML Parser for Java \u0444\u0438\u0440\u043c\u044b IBM, \u0432\u044b \u043c\u043e\u0436\u0435\u0442\u0435 \u0437\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c \u0435\u0433\u043e \u0441 \u0441\u0430\u0439\u0442\u0430"},
				new object[] {"noParsermsg5", "IBM AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER \u0438\u043c\u044f \u043a\u043b\u0430\u0441\u0441\u0430 (URIResolver \u0434\u043b\u044f \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f URL)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER \u0438\u043c\u044f \u043a\u043b\u0430\u0441\u0441\u0430 (EntityResolver \u0434\u043b\u044f \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f \u043c\u0430\u043a\u0440\u043e\u0441\u043e\u0432)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER \u0438\u043c\u044f \u043a\u043b\u0430\u0441\u0441\u0430 (ContentHandler \u0434\u043b\u044f \u0441\u0435\u0440\u0438\u0430\u043b\u0438\u0437\u0430\u0446\u0438\u0438 \u0432\u044b\u0432\u043e\u0434\u0430)]"},
				new object[] {"optionLINENUMBERS", "   [-L \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u043d\u043e\u043c\u0435\u0440\u0430 \u0441\u0442\u0440\u043e\u043a \u0438\u0441\u0445\u043e\u0434\u043d\u043e\u0433\u043e \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (\u0437\u0430\u0434\u0430\u0439\u0442\u0435 \u0434\u043b\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0437\u0430\u0449\u0438\u0449\u0435\u043d\u043d\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u043a\u0438 \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 true.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA \u0442\u0438\u043f-\u043d\u043e\u0441\u0438\u0442. (\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c \u0430\u0442\u0440\u0438\u0431\u0443\u0442 \u043d\u043e\u0441\u0438\u0442\u0435\u043b\u044f \u0434\u043b\u044f \u043f\u043e\u0438\u0441\u043a\u0430 \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR \u0438\u043c\u044f-\u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f (\u042f\u0432\u043d\u043e \u0443\u043a\u0430\u0437\u0430\u0442\u044c s2s=SAX \u0438\u043b\u0438 d2d=DOM.)] "},
				new object[] {"optionDIAG", "   [-DIAG (\u041f\u0435\u0447\u0430\u0442\u044c \u043e\u0442\u0447\u0435\u0442\u0430 \u043e \u0432\u0440\u0435\u043c\u0435\u043d\u0438 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f \u0432 \u043c\u0438\u043b\u043b\u0438\u0441\u0435\u043a\u0443\u043d\u0434\u0430\u0445.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (\u0417\u0430\u043f\u0440\u043e\u0441\u0438\u0442\u044c \u0434\u043e\u043f\u043e\u043b\u043d\u044f\u044e\u0449\u0443\u044e \u043c\u043e\u0434\u0435\u043b\u044c DTM \u0441 \u043f\u043e\u043c\u043e\u0449\u044c\u044e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (\u041e\u0442\u043a\u043b\u044e\u0447\u0438\u0442\u044c \u043e\u043f\u0442\u0438\u043c\u0438\u0437\u0430\u0446\u0438\u044e \u0442\u0430\u0431\u043b\u0438\u0446\u044b \u0441\u0442\u0438\u043b\u0435\u0439 \u0441 \u043f\u043e\u043c\u043e\u0449\u044c\u044e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u044f http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", "   [-RL \u043e\u0433\u0440\u0430\u043d\u0438\u0447\u0435\u043d\u0438\u0435 (\u0427\u0438\u0441\u043b\u043e\u0432\u043e\u0435 \u043e\u0433\u0440\u0430\u043d\u0438\u0447\u0435\u043d\u0438\u0435 \u0433\u043b\u0443\u0431\u0438\u043d\u044b \u0440\u0435\u043a\u0443\u0440\u0441\u0438\u0438 \u0442\u0430\u0431\u043b\u0438\u0446 \u0441\u0442\u0438\u043b\u0435\u0439.)]"},
				new object[] {"optionXO", "   [-XO [\u0438\u043c\u044f-\u043f\u0440\u043e\u0446\u0435\u0434\u0443\u0440\u044b] (\u041f\u0440\u0438\u0441\u0432\u043e\u0438\u0442\u044c \u0438\u043c\u044f \u0441\u043e\u0437\u0434\u0430\u043d\u043d\u043e\u0439 \u043f\u0440\u043e\u0446\u0435\u0434\u0443\u0440\u0435 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f)]"},
				new object[] {"optionXD", "   [-XD \u0446\u0435\u043b\u0435\u0432\u043e\u0439-\u043a\u0430\u0442\u0430\u043b\u043e\u0433 (\u0417\u0430\u0434\u0430\u0435\u0442 \u0446\u0435\u043b\u0435\u0432\u043e\u0439 \u043a\u0430\u0442\u0430\u043b\u043e\u0433 \u0434\u043b\u044f \u043f\u0440\u043e\u0446\u0435\u0434\u0443\u0440\u044b \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f)]"},
				new object[] {"optionXJ", "   [-XJ \u0444\u0430\u0439\u043b-jar (\u0423\u043f\u0430\u043a\u043e\u0432\u044b\u0432\u0430\u0435\u0442 \u043a\u043b\u0430\u0441\u0441\u044b \u043f\u0440\u043e\u0446\u0435\u0434\u0443\u0440\u044b \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f \u0432 <\u0444\u0430\u0439\u043b-jar>)]"},
				new object[] {"optionXP", "   [-XP \u043f\u0430\u043a\u0435\u0442 (\u041f\u0440\u0435\u0444\u0438\u043a\u0441 \u0438\u043c\u0435\u043d\u0438 \u043f\u0430\u043a\u0435\u0442\u0430 \u0434\u043b\u044f \u0432\u0441\u0435\u0445 \u0441\u043e\u0437\u0434\u0430\u043d\u043d\u044b\u0445 \u043a\u043b\u0430\u0441\u0441\u043e\u0432 translet)]"},
				new object[] {"optionXN", "   [-XN (\u0440\u0430\u0437\u0440\u0435\u0448\u0430\u0435\u0442 \u043a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u043d\u0438\u0435 \u0448\u0430\u0431\u043b\u043e\u043d\u0430)]"},
				new object[] {"optionXX", "   [-XX (\u0432\u043a\u043b\u044e\u0447\u0430\u0435\u0442 \u0432\u044b\u0432\u043e\u0434 \u0434\u043e\u043f\u043e\u043b\u043d\u0438\u0442\u0435\u043b\u044c\u043d\u044b\u0445 \u043e\u0442\u043b\u0430\u0434\u043e\u0447\u043d\u044b\u0445 \u0441\u043e\u043e\u0431\u0449\u0435\u043d\u0438\u0439)]"},
				new object[] {"optionXT", "   [-XT (\u043f\u043e \u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e\u0441\u0442\u0438 \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0435\u0442 \u043f\u0440\u043e\u0446\u0435\u0434\u0443\u0440\u0443 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f)]"},
				new object[] {"diagTiming"," --------- \u041f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435 {0} \u0441 \u043f\u043e\u043c\u043e\u0449\u044c\u044e {1} \u0437\u0430\u043d\u044f\u043b\u043e {2} \u043c\u0441"},
				new object[] {"recursionTooDeep","\u0421\u043b\u0438\u0448\u043a\u043e\u043c \u0431\u043e\u043b\u044c\u0448\u0430\u044f \u0432\u043b\u043e\u0436\u0435\u043d\u043d\u043e\u0441\u0442\u044c \u0448\u0430\u0431\u043b\u043e\u043d\u043e\u0432. \u0412\u043b\u043e\u0436\u0435\u043d\u043d\u043e\u0441\u0442\u044c = {0}, \u0448\u0430\u0431\u043b\u043e\u043d {1} {2}"},
				new object[] {"nameIs", "\u0438\u043c\u044f"},
				new object[] {"matchPatternIs", "\u0448\u0430\u0431\u043b\u043e\u043d \u0441\u043e\u043e\u0442\u0432\u0435\u0442\u0441\u0442\u0432\u0438\u044f"}
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
	  public const string ERROR_STRING = "\u041e\u0448\u0438\u0431\u043a\u0430";

	  /// <summary>
	  /// String to prepend to error messages. </summary>
	  public const string ERROR_HEADER = "\u041e\u0448\u0438\u0431\u043a\u0430: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "\u041f\u0440\u0435\u0434\u0443\u043f\u0440\u0435\u0436\u0434\u0435\u043d\u0438\u0435: ";

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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("en", "US"));
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