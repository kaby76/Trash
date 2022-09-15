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
 * $Id: XSLTErrorResources_sk.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_sk : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Chyba: Nie je mo\u017en\u00e9 ma\u0165 vo v\u00fdraze '{'"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} m\u00e1 neplatn\u00fd atrib\u00fat: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode je v xsl:apply-imports nulov\u00fd!"},
				new object[] {ER_CANNOT_ADD, "Nem\u00f4\u017ee prida\u0165 {0} do {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode je nulov\u00fd v handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} mus\u00ed ma\u0165 atrib\u00fat n\u00e1zvu."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Nebolo mo\u017en\u00e9 n\u00e1js\u0165 vzor s n\u00e1zvom: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Nebolo mo\u017en\u00e9 rozl\u00ed\u0161i\u0165 n\u00e1zov AVT v xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} vy\u017eaduje atrib\u00fat: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} mus\u00ed ma\u0165 atrib\u00fat ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Nespr\u00e1vna hodnota na atrib\u00fate \u00farovne: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "n\u00e1zov processing-instruction nem\u00f4\u017ee by\u0165 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "n\u00e1zov in\u0161trukcie spracovania mus\u00ed by\u0165 platn\u00fdm NCName: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} mus\u00ed ma\u0165 porovn\u00e1vac\u00ed atrib\u00fat, ak m\u00e1 re\u017eim."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} vy\u017eaduje bu\u010f n\u00e1zov, alebo porovn\u00e1vac\u00ed atrib\u00fat."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Nie je mo\u017en\u00e9 rozl\u00ed\u0161i\u0165 predponu n\u00e1zvov\u00e9ho priestoru: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space m\u00e1 neplatn\u00fa hodnotu: {0}"},
				new object[] {ER_NO_OWNERDOC, "Potomok uzla nem\u00e1 dokument vlastn\u00edka!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Chyba ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "Pokus o pridanie nulov\u00e9ho potomka!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} vy\u017eaduje atrib\u00fat v\u00fdberu."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when mus\u00ed ma\u0165 atrib\u00fat 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param mus\u00ed ma\u0165 atrib\u00fat 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "kontext nem\u00e1 dokument vlastn\u00edka!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Nebolo mo\u017en\u00e9 vytvori\u0165 vz\u0165ah XML TransformerFactory: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: Proces bol ne\u00faspe\u0161n\u00fd."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: bol ne\u00faspe\u0161n\u00fd."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "K\u00f3dovanie nie je podporovan\u00e9: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Nebolo mo\u017en\u00e9 vytvori\u0165 TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key vy\u017eaduje atrib\u00fat 'name'!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key vy\u017eaduje atrib\u00fat 'match'!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key vy\u017eaduje atrib\u00fat 'use'!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} vy\u017eaduje atrib\u00fat ''elements''!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} ch\u00fdba atrib\u00fat ''prefix''"},
				new object[] {ER_BAD_STYLESHEET_URL, "URL \u0161t\u00fdlu dokumentu je nespr\u00e1vna: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "S\u00fabor \u0161t\u00fdlu dokumentu nebol n\u00e1jden\u00fd: {0}"},
				new object[] {ER_IOEXCEPTION, "V s\u00fabore \u0161t\u00fdlu dokumentu bola vstupno-v\u00fdstupn\u00e1 v\u00fdnimka: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Nebolo mo\u017en\u00e9 n\u00e1js\u0165 atrib\u00fat href pre {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} priamo, alebo nepriamo, obsahuje s\u00e1m seba!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "chyba StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} ch\u00fdba atrib\u00fat ''lang''"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) chybne umiestnen\u00fd {0} element?? Ch\u00fdba kontajnerov\u00fd prvok ''component''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "M\u00f4\u017ee prev\u00e1dza\u0165 v\u00fdstup len do Element, DocumentFragment, Document, alebo PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "chyba StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Chyba UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Chyba! Nena\u0161lo sa vyjadrenie v\u00fdberu xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Nie je mo\u017en\u00e9 serializova\u0165 XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Nebol zadan\u00fd vstup \u0161t\u00fdl dokumentu!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Zlyhalo spracovanie \u0161t\u00fdlu dokumentu!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Nebolo mo\u017en\u00e9 analyzova\u0165 dokument {0}!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Nebolo mo\u017en\u00e9 n\u00e1js\u0165 fragment: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Uzol, na ktor\u00fd ukazuje identifik\u00e1tor fragmentu nebol elementom: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each mus\u00ed ma\u0165 bu\u010f porovn\u00e1vac\u00ed atrib\u00fat, alebo atrib\u00fat n\u00e1zvu"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "vzory musia ma\u0165 bu\u010f porovn\u00e1vacie atrib\u00faty, alebo atrib\u00faty n\u00e1zvov"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "\u017diadna k\u00f3pia fragmentu dokumentu!"},
				new object[] {ER_CANT_CREATE_ITEM, "Nie je mo\u017en\u00e9 vytvori\u0165 polo\u017eku vo v\u00fdsledkovom strome: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space v zdrojovom XML m\u00e1 neplatn\u00fa hodnotu: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Neexistuje \u017eiadna deklar\u00e1cia xsl:key pre {0}!"},
				new object[] {ER_CANT_CREATE_URL, "Chyba! Nie je mo\u017en\u00e9 vytvori\u0165 url pre: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions nie je podporovan\u00e9"},
				new object[] {ER_PROCESSOR_ERROR, "Chyba XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} nie je povolen\u00fd vn\u00fatri \u0161t\u00fdlu dokumentu!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns u\u017e viac nie je podporovan\u00fd!  Pou\u017eite namiesto toho xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space u\u017e viac nie je podporovan\u00fd!  Pou\u017eite namiesto toho xsl:strip-space alebo xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result u\u017e viac nie je podporovan\u00fd!  Pou\u017eite namiesto toho xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} m\u00e1 neplatn\u00fd atrib\u00fat: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Nezn\u00e1my element XSL: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort mo\u017eno pou\u017ei\u0165 len s xsl:apply-templates alebo xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) xsl:when na nespr\u00e1vnom mieste!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when nem\u00e1 ako rodi\u010da xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) xsl:otherwise na nespr\u00e1vnom mieste!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise nem\u00e1 ako rodi\u010da xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} nie je povolen\u00fd vn\u00fatri vzoru!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0} prefix roz\u0161\u00edren\u00e9ho n\u00e1zvov\u00e9ho priestoru {1} nie je zn\u00e1my"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Importy sa m\u00f4\u017eu vyskytn\u00fa\u0165 len ako prv\u00e9 \u010dasti \u0161t\u00fdlu dokumentu!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} priamo, alebo nepriami, importuje s\u00e1m seba!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space m\u00e1 neplatn\u00fa hodnotu: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet nebol \u00faspe\u0161n\u00fd!"},
				new object[] {ER_SAX_EXCEPTION, "V\u00fdnimka SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Funkcia nie je podporovan\u00e1!"},
				new object[] {ER_XSLT_ERROR, "Chyba XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "znak meny nie je povolen\u00fd vo re\u0165azci form\u00e1tu vzoru"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Funckia dokumentu nie je podporovan\u00e1 v \u0161t\u00fdle dokumentu DOM!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Nie je mo\u017en\u00e9 ur\u010di\u0165 prefix bezprefixov\u00e9ho rozklada\u010da!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Roz\u0161\u00edrenie presmerovania: Nedal sa z\u00edska\u0165 n\u00e1zov s\u00faboru - s\u00fabor alebo atrib\u00fat v\u00fdberu mus\u00ed vr\u00e1ti\u0165 platn\u00fd re\u0165azec."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Nie je mo\u017en\u00e9 vytvori\u0165 FormatterListener v roz\u0161\u00edren\u00ed Redirect!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Predpona v exclude-result-prefixes je neplatn\u00e1: {0}"},
				new object[] {ER_MISSING_NS_URI, "Ch\u00fdbaj\u00faci n\u00e1zvov\u00fd priestor URI pre zadan\u00fd prefix"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Ch\u00fdbaj\u00faci argument pre vo\u013ebu: {0}"},
				new object[] {ER_INVALID_OPTION, "Neplatn\u00e1 vo\u013eba. {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Znetvoren\u00fd re\u0165azec form\u00e1tu: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet si vy\u017eaduje atrib\u00fat 'version'!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Atrib\u00fat: {0} m\u00e1 neplatn\u00fa hodnotu: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose vy\u017eaduje xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports nie je povolen\u00fd v xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Nem\u00f4\u017ee pou\u017ei\u0165 DTMLiaison pre v\u00fdstupn\u00fd uzol DOM... namiesto neho odo\u0161lite org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Nem\u00f4\u017ee pou\u017ei\u0165 DTMLiaison pre vstupn\u00fd uzol DOM... namiesto neho odo\u0161lite org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Volanie elementu roz\u0161\u00edrenia zlyhalo: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Predpona sa mus\u00ed rozl\u00ed\u0161i\u0165 do n\u00e1zvov\u00e9ho priestoru: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Bolo zisten\u00e9 neplatn\u00e9 nahradenie UTF-16: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} pou\u017eil s\u00e1m seba, \u010do sp\u00f4sob\u00ed nekone\u010dn\u00fa slu\u010dku."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Nie je mo\u017en\u00e9 mie\u0161a\u0165 vstup in\u00fd, ne\u017e Xerces-DOM s v\u00fdstupom Xerces-DOM!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "V ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Na\u0161iel sa viac ne\u017e jeden vzor s n\u00e1zvom: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Neplatn\u00e9 volanie funkcie: rekurz\u00edvne volanie k\u013e\u00fa\u010da() nie je povolen\u00e9"},
				new object[] {ER_REFERENCING_ITSELF, "Premenn\u00e1 {0} sa priamo, alebo nepriamo, odkazuje sama na seba!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "Vstupn\u00fd uzol nem\u00f4\u017ee by\u0165 pre DOMSource pre newTemplates nulov\u00fd!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "S\u00fabor triedy nebol pre vo\u013ebu {0} n\u00e1jden\u00fd"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Po\u017eadovan\u00fd element sa nena\u0161iel: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream nem\u00f4\u017ee by\u0165 nulov\u00fd"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI nem\u00f4\u017ee by\u0165 nulov\u00fd"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "S\u00fabor nem\u00f4\u017ee by\u0165 nulov\u00fd"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource nem\u00f4\u017ee by\u0165 nulov\u00fd"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Nebolo mo\u017en\u00e9 inicializova\u0165 Spr\u00e1vcu BSF"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Nebolo mo\u017en\u00e9 skompilova\u0165 pr\u00edponu"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Nebolo mo\u017en\u00e9 vytvori\u0165 roz\u0161\u00edrenie: {0} z d\u00f4vodu: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Volanie met\u00f3dy met\u00f3dou in\u0161tancie {0} vy\u017eaduje ako prv\u00fd argument In\u0161tanciu objektu"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Bol zadan\u00fd neplatn\u00fd n\u00e1zov s\u00fa\u010dasti {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Met\u00f3da n\u00e1zvu s\u00fa\u010dasti mus\u00ed by\u0165 statick\u00e1 {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Roz\u0161\u00edrenie funkcie {0} : {1} je nezn\u00e1me"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Bola n\u00e1jden\u00e1 viac ne\u017e jedna najlep\u0161ia zhoda s kon\u0161truktorom pre {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Bola n\u00e1jden\u00e1 viac ne\u017e jedna najlep\u0161ia zhoda pre met\u00f3du {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Bola n\u00e1jden\u00e1 viac ne\u017e jedna najlep\u0161ia zhoda pre met\u00f3du s\u00fa\u010dasti {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Bolo odoslan\u00fd neplatn\u00fd kontext na zhodnotenie {0}"},
				new object[] {ER_POOL_EXISTS, "Oblas\u0165 u\u017e existuje"},
				new object[] {ER_NO_DRIVER_NAME, "Nebol zadan\u00fd \u017eiaden n\u00e1zov ovl\u00e1da\u010da"},
				new object[] {ER_NO_URL, "Nebola zadan\u00e1 \u017eiadna URL"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "Ve\u013ekos\u0165 oblasti je men\u0161ia ne\u017e jeden!"},
				new object[] {ER_INVALID_DRIVER, "Bol zadan\u00fd neplatn\u00fd n\u00e1zov ovl\u00e1da\u010da!"},
				new object[] {ER_NO_STYLESHEETROOT, "Nebol n\u00e1jden\u00fd kore\u0148 \u0161t\u00fdlu dokumentu!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Neplatn\u00e1 hodnota pre xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "zlyhal processFromNode"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Prostriedok [ {0} ] sa nedal na\u010d\u00edta\u0165: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Ve\u013ekos\u0165 vyrovn\u00e1vacej pam\u00e4te <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Nezn\u00e1ma chyba po\u010das volania pr\u00edpony"},
				new object[] {ER_NO_NAMESPACE_DECL, "Prefix {0} nem\u00e1 zodpovedaj\u00facu deklar\u00e1ciu n\u00e1zvov\u00e9ho priestoru"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Obsah elementu nie je povolen\u00fd pre lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Ukon\u010denie riaden\u00e9 \u0161t\u00fdlom dokumentu"},
				new object[] {ER_ONE_OR_TWO, "1, alebo 2"},
				new object[] {ER_TWO_OR_THREE, "2, alebo 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Nebolo mo\u017en\u00e9 zavies\u0165 {0} (check CLASSPATH), teraz s\u00fa po\u017eit\u00e9 len predvolen\u00e9 \u0161tandardy"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Nie je mo\u017en\u00e9 inicializova\u0165 predvolen\u00e9 vzory"},
				new object[] {ER_RESULT_NULL, "V\u00fdsledok by nemal by\u0165 nulov\u00fd"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "V\u00fdsledkom nem\u00f4\u017ee by\u0165 mno\u017eina"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Nie je zadan\u00fd \u017eiaden v\u00fdstup"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Ned\u00e1 sa transformova\u0165 na v\u00fdsledok typu {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Ned\u00e1 sa transformova\u0165 zdroj typu {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Nulov\u00fd manipula\u010dn\u00fd program obsahu"},
				new object[] {ER_NULL_ERROR_HANDLER, "Nulov\u00fd chybov\u00fd manipula\u010dn\u00fd program"},
				new object[] {ER_CANNOT_CALL_PARSE, "nem\u00f4\u017ee by\u0165 volan\u00e9 analyzovanie, ak nebol nastaven\u00fd ContentHandler"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "\u017diaden rodi\u010d pre filter"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Nena\u0161iel sa \u017eiadny stylesheet v: {0}, m\u00e9dium= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Nena\u0161iel sa \u017eiadny xml-stylesheet PI v: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Nie je podporovan\u00e9: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "Hodnota vlastnosti {0} by mala by\u0165 boolovsk\u00e1 in\u0161tancia"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Nie je mo\u017en\u00e9 dosiahnu\u0165 extern\u00fd skript na {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "Prostriedok [ {0} ] nemohol by\u0165 n\u00e1jden\u00fd.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "V\u00fdstupn\u00e9 vlastn\u00edctvo nebolo rozoznan\u00e9: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Zlyhalo vytv\u00e1ranie in\u0161tancie ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "Hodnota pre {0} by mala obsahova\u0165 analyzovate\u013en\u00e9 \u010d\u00edslo"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "Hodnota {0} by sa mala rovna\u0165 \u00e1no, alebo nie"},
				new object[] {ER_FAILED_CALLING_METHOD, "Zlyhalo volanie met\u00f3dy {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Zlyhalo vytv\u00e1ranie in\u0161tancie ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "V tomto bode dokumentu nie s\u00fa znaky povolen\u00e9"},
				new object[] {ER_ATTR_NOT_ALLOWED, "Atrib\u00fat \"{0}\" nie je povolen\u00fd na s\u00fa\u010dasti {1}!"},
				new object[] {ER_BAD_VALUE, "{0} zl\u00e1 hodnota {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Hodnota atrib\u00fatu {0} nebola n\u00e1jden\u00e1 "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Hodnota atrib\u00fatu {0} nebola rozpoznan\u00e1 "},
				new object[] {ER_NULL_URI_NAMESPACE, "Pokus o vytvorenie prefixu n\u00e1zvov\u00e9ho priestoru s nulov\u00fdm URI"},
				new object[] {ER_NUMBER_TOO_BIG, "Pokus o form\u00e1tovanie \u010d\u00edsla v\u00e4\u010d\u0161ieho, ne\u017e je najdlh\u0161\u00ed dlh\u00fd celo\u010d\u00edseln\u00fd typ"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Nie je mo\u017en\u00e9 n\u00e1js\u0165 triedu ovl\u00e1da\u010da SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "Trieda ovl\u00e1da\u010da SAX1 {0} bola n\u00e1jden\u00e1, ale nem\u00f4\u017ee by\u0165 zaveden\u00e1"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "Trieda ovl\u00e1da\u010da SAX1 {0} bola zaveden\u00e1, ale nem\u00f4\u017ee by\u0165 dolo\u017een\u00e1 pr\u00edkladom"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "Trieda ovl\u00e1da\u010da SAX1 {0} neimplementuje org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Syst\u00e9mov\u00e1 vlastnos\u0165 org.xml.sax.parser nie je zadan\u00e1"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Argument syntaktick\u00e9ho analyz\u00e1tora nesmie by\u0165 nulov\u00fd"},
				new object[] {ER_FEATURE, "Vlastnos\u0165: {0}"},
				new object[] {ER_PROPERTY, "Vlastn\u00edctvo: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Rozklada\u010d nulov\u00fdch ent\u00edt"},
				new object[] {ER_NULL_DTD_HANDLER, "Nulov\u00fd manipula\u010dn\u00fd program DTD"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Nie je zadan\u00fd \u017eiaden n\u00e1zov ovl\u00e1da\u010da!"},
				new object[] {ER_NO_URL_SPECIFIED, "Nie je zadan\u00e1 \u017eiadna URL!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "Ve\u013ekos\u0165 oblasti je men\u0161ia ne\u017e 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Je zadan\u00fd neplatn\u00fd n\u00e1zov ovl\u00e1da\u010da!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Chyba program\u00e1tora! V\u00fdraz nem\u00e1 rodi\u010da ElemTemplateElement!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Tvrdenie program\u00e1tora v RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0}nie je na tejto poz\u00edcii predlohy so \u0161t\u00fdlmi povolen\u00e9!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Text bez medzier nie je povolen\u00fd na tejto poz\u00edcii predlohy so \u0161t\u00fdlmi!"},
				new object[] {INVALID_TCHAR, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00fd pre atrib\u00fat CHAR: {0}.  Atrib\u00fat typu CHAR mus\u00ed by\u0165 len 1 znak!"},
				new object[] {INVALID_QNAME, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00e1 pre atrib\u00fat QNAME: {0}"},
				new object[] {INVALID_ENUM, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00e1 pre atrib\u00fat ENUM: {0}.  Platn\u00e9 hodnoty s\u00fa: {2}."},
				new object[] {INVALID_NMTOKEN, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00e1 pre atrib\u00fat NMTOKEN:{0} "},
				new object[] {INVALID_NCNAME, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00e1 pre atrib\u00fat NCNAME: {0} "},
				new object[] {INVALID_BOOLEAN, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00e1 pre boolovsk\u00fd atrib\u00fat: {0} "},
				new object[] {INVALID_NUMBER, "Neplatn\u00e1 hodnota: {1} pou\u017e\u00edvan\u00e1 pre atrib\u00fat \u010d\u00edsla: {0} "},
				new object[] {ER_ARG_LITERAL, "Argument pre {0} v zhodnom vzore mus\u00ed by\u0165 liter\u00e1lom."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Duplicitn\u00e1 deklar\u00e1cia glob\u00e1lnej premennej."},
				new object[] {ER_DUPLICATE_VAR, "Duplicitn\u00e1 deklar\u00e1cia premennej."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template mus\u00ed ma\u0165 n\u00e1zov alebo atrib\u00fat zhody (alebo oboje)"},
				new object[] {ER_INVALID_PREFIX, "Predpona v exclude-result-prefixes je neplatn\u00e1: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "pomenovan\u00e1 sada atrib\u00fatov {0} neexistuje"},
				new object[] {ER_FUNCTION_NOT_FOUND, "Funkcia s n\u00e1zvom {0} neexistuje."},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "Prvok {0} nesmie ma\u0165 aj atrib\u00fat content aj atrib\u00fat select."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "Hodnota parametra {0} mus\u00ed by\u0165 platn\u00fdm objektom jazyka Java."},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "Atrib\u00fat result-prefix prvku xsl:namespace-alias m\u00e1 hodnotu '#default', ale v rozsahu pre prvok neexistuje \u017eiadna deklar\u00e1cia \u0161tandardn\u00e9ho n\u00e1zvov\u00e9ho priestoru"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "Atrib\u00fat result-prefix prvku xsl:namespace-alias m\u00e1 hodnotu ''{0}'', ale v rozsahu pre prvok neexistuje \u017eiadna deklar\u00e1cia n\u00e1zvov\u00e9ho priestoru pre predponu ''{0}''."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "V TransformerFactory.setFeature(N\u00e1zov re\u0165azca, boolovsk\u00e1 hodnota)nem\u00f4\u017ee ma\u0165 funkcia n\u00e1zov null."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "N\u00e1zov vlastnosti nem\u00f4\u017ee by\u0165 null v TransformerFactory.getFeature(N\u00e1zov re\u0165azca)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "V tomto TransformerFactory sa ned\u00e1 nastavi\u0165 vlastnos\u0165 ''{0}''."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "Pou\u017e\u00edvanie prvku roz\u0161\u00edrenia ''{0}'' nie je povolen\u00e9, ke\u010f je funkcia bezpe\u010dn\u00e9ho spracovania nastaven\u00e1 na hodnotu true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Ned\u00e1 sa z\u00edska\u0165 predpona pre null n\u00e1zvov\u00fd priestor uri."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Ned\u00e1 sa z\u00edska\u0165 n\u00e1zvov\u00fd priestor uri pre predponu null."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "N\u00e1zov funkcie nem\u00f4\u017ee by\u0165 null."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "Arita nem\u00f4\u017ee by\u0165 z\u00e1porn\u00e1."},
				new object[] {WG_FOUND_CURLYBRACE, "Bol n\u00e1jden\u00fd znak '}', ale nie otvoren\u00fd \u017eiaden vzor atrib\u00fatu!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Upozornenie: atrib\u00fat po\u010dtu sa nezhoduje s predchodcom v xsl:number! Cie\u013e = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Star\u00e1 syntax: N\u00e1zov atrib\u00fatu 'expr' bol zmenen\u00fd na 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan zatia\u013e nespracov\u00e1va n\u00e1zov umiestnenia vo funkcii format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Upozornenie: Nebolo mo\u017en\u00e9 n\u00e1js\u0165 lok\u00e1l pre xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Nie je mo\u017en\u00e9 vytvori\u0165 URL z: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Nie je mo\u017en\u00e9 zavies\u0165 po\u017eadovan\u00fd doc: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Nebolo mo\u017en\u00e9 n\u00e1js\u0165 porovn\u00e1va\u010d pre <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Star\u00e1 syntax: in\u0161trukcia funkci\u00ed by mala pou\u017e\u00edva\u0165 url {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "K\u00f3dovanie nie je podporovan\u00e9: {0}, pou\u017e\u00edva UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "K\u00f3dovanie nie je podporovan\u00e9: {0}, pou\u017e\u00edva Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Boli zisten\u00e9 konflikty \u0161pecifickosti: {0} naposledy n\u00e1jden\u00e1 v \u0161t\u00fdle dokumentu bude pou\u017eit\u00e1."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Anal\u00fdza a pr\u00edprava {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Attr vzor, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Konflikt zhodnosti medzi xsl:strip-space a xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan zatia\u013e nesprac\u00fava atrib\u00fat {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Pre desiatkov\u00fd form\u00e1t sa nena\u0161la \u017eiadna deklar\u00e1cia: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Ch\u00fdbaj\u00faci, alebo nespr\u00e1vny n\u00e1zvov\u00fd priestor XSLT. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Povolen\u00e1 je len jedna \u0161tandardn\u00e1 deklar\u00e1cia xsl:decimal-format."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "N\u00e1zvy xsl:decimal-format musia by\u0165 jedine\u010dn\u00e9. N\u00e1zov \"{0}\" bol zopakovan\u00fd."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} m\u00e1 neplatn\u00fd atrib\u00fat: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Nebolo mo\u017en\u00e9 rozl\u00ed\u0161i\u0165 predponu n\u00e1zvov\u00e9ho priestoru: {0}. Uzol bude ignorovan\u00fd."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet si vy\u017eaduje atrib\u00fat 'version'!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Neplatn\u00fd n\u00e1zov atrib\u00fatu: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Neplatn\u00e1 hodnota pou\u017e\u00edvan\u00e1 pre atrib\u00fat {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "V\u00fdsledn\u00fd nodeset z druh\u00e9ho argumentu funkcie dokumentu je pr\u00e1zdny. Vr\u00e1\u0165te pr\u00e1zdnu mno\u017einu uzlov."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Hodnota atrib\u00fatu 'name' n\u00e1zvu xsl:processing-instruction nesmie by\u0165 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Hodnota atrib\u00fatu ''name'' xsl:processing-instruction mus\u00ed by\u0165 platn\u00fdm NCName: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Nie je mo\u017en\u00e9 prida\u0165 atrib\u00fat {0} po uzloch potomka alebo pred vytvoren\u00edm elementu.  Atrib\u00fat bude ignorovan\u00fd."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Prebieha pokus o \u00fapravu objektu, pre ktor\u00fd nie s\u00fa povolen\u00e9 \u00fapravy."},
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
				new object[] {"xslProc_option", "Vo\u013eby triedy procesu pr\u00edkazov\u00e9ho riadka Xalan-J:"},
				new object[] {"xslProc_option", "Vo\u013eby triedy Process pr\u00edkazov\u00e9ho riadka Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "Vo\u013eba {0} nie je podporovan\u00e1 v re\u017eime XSLTC."},
				new object[] {"xslProc_invalid_xalan_option", "Vo\u013ebu {0} mo\u017eno pou\u017ei\u0165 len spolu s -XSLTC."},
				new object[] {"xslProc_no_input", "Chyba: nie je uveden\u00fd \u017eiadny \u0161t\u00fdl dokumentu, ani vstupn\u00fd xml. Spustite tento pr\u00edkaz bez akejko\u013evek vo\u013eby pre in\u0161trukcie pou\u017eitia."},
				new object[] {"xslProc_common_options", "-Be\u017en\u00e9 vo\u013eby-"},
				new object[] {"xslProc_xalan_options", "-Vo\u013eby pre Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Vo\u013eby pre XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(stla\u010dte <return> a pokra\u010dujte)"},
				new object[] {"optionXSLTC", "   [-XSLTC (pou\u017eite XSLTC na transform\u00e1ciu)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER plne kvalifikovan\u00fd n\u00e1zov triedy sprostredkovate\u013ea syntaktick\u00e9ho analyz\u00e1tora]"},
				new object[] {"optionE", "   [-E (Nerozvinie odkazy na entity)]"},
				new object[] {"optionV", "   [-E (Nerozvinie odkazy na entity)]"},
				new object[] {"optionQC", "   [-QC (Varovania pri konfliktoch Quiet Pattern)]"},
				new object[] {"optionQ", "   [-Q  (Tich\u00fd re\u017eim)]"},
				new object[] {"optionLF", "   [-LF (Znaky pre posun riadka pou\u017ei\u0165 len vo v\u00fdstupe {default is CR/LF})]"},
				new object[] {"optionCR", "   [-CR (Znaky n\u00e1vratu voz\u00edka pou\u017ei\u0165 len vo v\u00fdstupe {default is CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Ktor\u00e9 znaky maj\u00fa ma\u0165 zmenen\u00fd v\u00fdznam {default is <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Riadi po\u010det medzier odsadenia {default is 0})]"},
				new object[] {"optionTT", "   [-TT (Sledovanie, ako s\u00fa volan\u00e9 vzory.)]"},
				new object[] {"optionTG", "   [-TG (Sledovanie udalost\u00ed ka\u017edej gener\u00e1cie.)]"},
				new object[] {"optionTS", "   [-TS (Sledovanie udalost\u00ed ka\u017ed\u00e9ho v\u00fdberu.)]"},
				new object[] {"optionTTC", "   [-TTC (Sledovanie ako s\u00fa vytv\u00e1ran\u00ed potomkovia vzorov.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (Trieda TraceListener pre pr\u00edpony sledovania.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Ur\u010duje, \u010di m\u00e1 doch\u00e1dza\u0165 k overovaniu.  Overovanie je \u0161tandardne vypnut\u00e9.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {optional filename} (Vytvori\u0165 v\u00fdpis z\u00e1sobn\u00edka pri chybe.)]"},
				new object[] {"optionXML", "   [-XML (Pou\u017eije form\u00e1tor XML a prid\u00e1 hlavi\u010dku XML.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Jednoduch\u00fd textov\u00fd form\u00e1tor.)]"},
				new object[] {"optionHTML", "   [-HTML (Pou\u017eije form\u00e1tor HTML.)]"},
				new object[] {"optionPARAM", "   [-PARAM vyjadrenie n\u00e1zvu (nastav\u00ed parameter \u0161t\u00fdlu dokumentu)]"},
				new object[] {"noParsermsg1", "Proces XSL nebol \u00faspe\u0161n\u00fd."},
				new object[] {"noParsermsg2", "** Nebolo mo\u017en\u00e9 n\u00e1js\u0165 syntaktick\u00fd analyz\u00e1tor **"},
				new object[] {"noParsermsg3", "Skontroluje, pros\u00edm, svoju classpath."},
				new object[] {"noParsermsg4", "Ak nem\u00e1te Syntaktick\u00fd analyz\u00e1tor XML pre jazyk Java od firmy IBM, m\u00f4\u017eete si ho stiahnu\u0165 z"},
				new object[] {"noParsermsg5", "IBM's AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER pln\u00fd n\u00e1zov triedy (URIResolver bude pou\u017eit\u00fd na ur\u010dovanie URI)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER pln\u00fd n\u00e1zov triedy (EntityResolver bude pou\u017eit\u00fd na ur\u010denie ent\u00edt)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER pln\u00fd n\u00e1zov triedy (ContentHandler bude pou\u017eit\u00fd na serializ\u00e1ciu v\u00fdstupu)]"},
				new object[] {"optionLINENUMBERS", "   [-L pou\u017eije \u010d\u00edsla riadkov pre zdrojov\u00fd dokument]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (nastav\u00ed funkciu bezpe\u010dn\u00e9ho spracovania na hodnotu true.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType (pou\u017ei\u0165 atrib\u00fat m\u00e9dia na n\u00e1jdenie \u0161t\u00fdlu h\u00e1rka, priraden\u00e9ho k dokumentu.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName (Explicitne pou\u017ei\u0165 s2s=SAX alebo d2d=DOM na vykonanie transform\u00e1cie.)] "},
				new object[] {"optionDIAG", "   [-DIAG (Vytla\u010di\u0165 celkov\u00fd \u010das transform\u00e1cie v milisekund\u00e1ch.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (\u017eiados\u0165 o inkrement\u00e1lnu kon\u0161trukciu DTM nastaven\u00edm http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (po\u017eiadavka na nesprac\u00favanie optimaliz\u00e1cie defin\u00edcie \u0161t\u00fdlov nastaven\u00edm http://xml.apache.org/xalan/features/optimize na hodnotu false.)]"},
				new object[] {"optionRL", "   [-RL recursionlimit (nastavi\u0165 \u010d\u00edseln\u00fd limit pre h\u013abku rekurzie \u0161t\u00fdlov h\u00e1rkov.)]"},
				new object[] {"optionXO", "   [-XO [transletName] (prira\u010fuje n\u00e1zov ku generovan\u00e9mu transletu)]"},
				new object[] {"optionXD", "   [-XD destinationDirectory (uv\u00e1dza cie\u013eov\u00fd adres\u00e1r pre translet)]"},
				new object[] {"optionXJ", "   [-XJ jarfile (pakuje triedy transletu do s\u00faboru jar s n\u00e1zvom <jarfile>)]"},
				new object[] {"optionXP", "   [-XP package (uv\u00e1dza predponu n\u00e1zvu bal\u00edka pre v\u0161etky generovan\u00e9 triedy transletu)]"},
				new object[] {"optionXN", "   [-XN (povo\u013euje zoradenie vzorov do riadka)]"},
				new object[] {"optionXX", "   [-XX (zap\u00edna \u010fal\u0161\u00ed v\u00fdstup spr\u00e1v ladenia)]"},
				new object[] {"optionXT", "   [-XT (ak je to mo\u017en\u00e9, pou\u017eite translet na transform\u00e1ciu)]"},
				new object[] {"diagTiming", " --------- Transform\u00e1cia z {0} cez {1} trvala {2} ms"},
				new object[] {"recursionTooDeep", "Vnorenie vzoru je pr\u00edli\u0161 hlbok\u00e9. vnorenie = {0}, vzor {1} {2}"},
				new object[] {"nameIs", "n\u00e1zov je"},
				new object[] {"matchPatternIs", "vzor zhody je"}
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
	  public const string ERROR_HEADER = "Chyba: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Upozornenie: ";

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