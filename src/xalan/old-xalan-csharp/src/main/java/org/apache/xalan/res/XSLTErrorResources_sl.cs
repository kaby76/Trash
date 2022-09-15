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
 * $Id: XSLTErrorResources_sl.java 1225426 2011-12-29 04:13:08Z mrglavas $
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
	public class XSLTErrorResources_sl : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Napaka: Izraz ne sme vsebovati '{'"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} vsebuje neveljaven atribut: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode je NULL v xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "Ne morem dodati {0} k {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode je NULL v handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} mora vsebovati atribut imena."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Nisem na\u0161em predloge z imenom: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Imena AVT v xsl:call-template ni bilo mogo\u010de razre\u0161iti."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} zahteva atribut: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} mora imeti atribut ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Slaba vrednost pri atributu stopnje: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Ime navodila za obdelavo ne more biti 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Ime navodila za obdelavo mora biti veljavno NCIme: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} mora vsebovati primerjalni atribut, \u010de vsebuje vozli\u0161\u010de."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} zahteva atribut imena ali primerjalni atribut."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Predpone imenskega prostora ni mogo\u010de razre\u0161iti: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space vsebuje neveljavno vrednost: {0}"},
				new object[] {ER_NO_OWNERDOC, "Podrejeno vozli\u0161\u010de ne vsebuje lastni\u0161kega dokumenta!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Napaka ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "Poskus dodajanja podrejenega elementa z vrednostjo NULL!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} zahteva atribut izbire."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when mora vsebovati atribut 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param mora vsebovati atribut 'ime'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "Kontekst ne vsebuje lastni\u0161kega dokumenta!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Ne morem ustvariti zveze XML TransformerFactory: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: postopek ni uspel."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: ni uspel."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Kodiranje ni podprto: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Ne morem ustvariti javanskega razreda TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key zahteva atribut 'ime'!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key zahteva atribut 'ujemanje'!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key zahteva atribut 'uporaba'!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} zahteva atribut ''elementi''!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} manjka atribut ''predpona''"},
				new object[] {ER_BAD_STYLESHEET_URL, "URL slogovne datoteke je neveljaven: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "Slogovne datoteke ni bilo mogo\u010de najti: {0}"},
				new object[] {ER_IOEXCEPTION, "Pri slogovni datoteki je pri\u0161lo do izjeme IO: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Atributa href za {0} ni bilo mogo\u010de najti"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} neposredno ali posredno vklju\u010duje samega sebe!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "Napaka StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} manjka atribut ''lang'' "},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) napa\u010dna postavitev elementa {0}?? Manjka vsebni element ''komponenta''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Prenos mogo\u010d samo v Element, DocumentFragment, Document, ali PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "Napaka StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Napaka UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Napaka! Ne najdem izbirnega izraza xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Ne morem serializirati XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Vnos slogovne datoteke ni dolo\u010den!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Obdelava slogovne datoteke ni uspela!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Dokumenta {0} ni mogo\u010de raz\u010dleniti!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Ne najdem fragmenta: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Vozli\u0161\u010de, na katerega ka\u017ee identifikator fragmenta, ni element: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "vsak mora vsebovati primerjalni atribut ali atribut imena"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "predloge morajo imeti primerjalni atribut ali atribut imena"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Ni klona fragmenta dokumenta!"},
				new object[] {ER_CANT_CREATE_ITEM, "Ne morem ustvariti elementa v drevesu rezultatov: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space v izvirnem XML ima neveljavno vrednost: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Ni deklaracije xsl:key za {0}!"},
				new object[] {ER_CANT_CREATE_URL, "Napaka! Ne morem ustvariti URL za: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions niso podprte"},
				new object[] {ER_PROCESSOR_ERROR, "Napaka XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} ni dovoljen znotraj slogovne datoteke!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns ni ve\u010d podprt!  Namesto njega uporabite xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space ni ve\u010d podprt!  Namesto njega uporabite xsl:strip-space ali xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result ni ve\u010d podprt!  Namesto njega uporabite xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} ima neveljaven atribut: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Neznani element XSL: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort lahko uporabljamo samo z xsl:apply-templates ali z xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) napa\u010dna postavitev xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:choose ni nadrejen xsl:when!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) napa\u010dna postavitev xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:choose ni nadrejen xsl:otherwise!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} ni dovoljen znotraj predloge!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) Neznana {0} kon\u010dnica predpone imenskega prostora {1}"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Uvozi se lahko pojavijo samo kot prvi elementi v slogovni datoteki!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} neposredno ali posredno uva\u017ea samega sebe!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space vsebuje neveljavno vrednost: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet ni uspelo!"},
				new object[] {ER_SAX_EXCEPTION, "Izjema SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Funkcija ni podprta!"},
				new object[] {ER_XSLT_ERROR, "Napaka XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "V oblikovnem nizu vzorca znak za denarno enoto ni dovoljen"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Funkcija dokumenta v slogovni datoteki DOM ni podprta!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Ne morem razbrati predpone nepredponskega razre\u0161evalnika!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Preusmeri kon\u010dnico: ne morem pridobiti imena datoteke - atribut datoteke ali izbire mora vrniti veljaven niz."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "V Preusmeritvi kon\u010dnice ne morem zgraditi FormatterListener!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Predpona v izklju\u010di-predpone-rezultatov (exclude-result-prefixes) ni veljavna: {0}"},
				new object[] {ER_MISSING_NS_URI, "Za navedeno predpono manjka imenski prostor URI"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Manjka argument za mo\u017enost: {0}"},
				new object[] {ER_INVALID_OPTION, "Neveljavna mo\u017enost: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Po\u0161kodovan niz sloga: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet zahteva atribut 'razli\u010dica'!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Atribut: {0} ima neveljavno vrednost: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose zahteva xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports v xsl:for-each ni dovoljen"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Za izhodno vozli\u0161\u010de DOM ne morem uporabiti DTMLiaison... namesto njega posredujte org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Za vhodno vozli\u0161\u010de DOM ne morem uporabiti DTMLiaison... namesto njega posredujte org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Klic elementa kon\u010dnice ni uspel: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Predpona se mora razre\u0161iti v imenski prostor: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Zaznan neveljaven nadomestek UTF-16: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} je uporabil samega sebe, kar bo povzro\u010dilo neskon\u010do ponavljanje."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Prepletanje ne-Xerces-DOM vhoda s Xerces-DOM vhodom ni mogo\u010de!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "V ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Na\u0161el ve\u010d predlog z istim imenom: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Neveljaven klic funkcije: povratni klici key() niso dovoljeni"},
				new object[] {ER_REFERENCING_ITSELF, "Spremenljivka {0} se neposredno ali posredno sklicuje sama nase!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "Vhodno vozli\u0161\u010de za DOMSource za newTemplates ne more biti NULL!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Datoteke razreda za mo\u017enost {0} ni bilo mogo\u010de najti"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Zahtevanega elementa ni bilo mogo\u010de najti: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream ne more biti NULL"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI ne more biti NULL"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "Datoteka ne more biti NULL"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource ne more biti NULL"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Inicializacija BSF Manager-ja ni mogo\u010da"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Kon\u010dnice ni mogo\u010de prevesti"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Ne morem ustvariti kon\u010dnice: {0} zaradi: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Klic primerkov metode za metodo {0} zahteva primerek objekta kot prvi argument"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Navedeno neveljavno ime elementa {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Metoda imena elementa mora biti stati\u010dna (static) {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Funkcija kon\u010dnice {0} : {1} je neznana"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Ve\u010d kot eno najbolj\u0161e ujemanje za graditelja za {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Ve\u010d kot eno najbolj\u0161e ujemanje za metodo {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Ve\u010d kot eno najbolj\u0161e ujemanje za metodo elementa {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Posredovan neveljaven kontekst za ovrednotenje {0}"},
				new object[] {ER_POOL_EXISTS, "Zaloga \u017ee obstaja"},
				new object[] {ER_NO_DRIVER_NAME, "Ime gonilnika ni dolo\u010deno"},
				new object[] {ER_NO_URL, "URL ni dolo\u010den"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "Zaloga je manj\u0161a od ena!"},
				new object[] {ER_INVALID_DRIVER, "Navedeno neveljavno ime gonilnika!"},
				new object[] {ER_NO_STYLESHEETROOT, "Korena slogovne datoteke ni mogo\u010de najti!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Neveljavna vrednost za xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode spodletelo"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Sredstva [ {0} ] ni bilo mogo\u010de nalo\u017eiti: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Velikost medpomnilnika <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Neznana napaka pri klicu kon\u010dnice"},
				new object[] {ER_NO_NAMESPACE_DECL, "Predpona {0} nima ustrezne deklaracije imenskega prostora"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Vsebina elementa za lang=javaclass {0} ni dovoljena"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Prekinitev usmerja slogovna datoteka"},
				new object[] {ER_ONE_OR_TWO, "1 ali 2"},
				new object[] {ER_TWO_OR_THREE, "2 ali 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Nisem mogel nalo\u017eiti {0} (preverite CLASSPATH), trenutno se uporabljajo privzete vrednosti"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Ne morem inicializirati privzetih predlog"},
				new object[] {ER_RESULT_NULL, "Rezultat naj ne bi bil NULL"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Rezultata ni bilo mogo\u010de nastaviti"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Izhod ni naveden"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Ne morem pretvoriti v rezultat tipa {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Ne morem pretvoriti vira tipa {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Program za obravnavo vsebine NULL"},
				new object[] {ER_NULL_ERROR_HANDLER, "Program za obravnavo napak NULL"},
				new object[] {ER_CANNOT_CALL_PARSE, "klic raz\u010dlenitve ni mo\u017een \u010de ContentHandler ni bil nastavljen"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Ni nadrejenega za filter"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Ni mogo\u010de najti slogovne datoteke v: {0}, medij= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Ne najdem xml-stylesheet PI v: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Ni podprto: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "Vrednost lastnosti {0} bi morala biti ponovitev logi\u010dne vrednosti"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Ne morem dostopati do zunanje skripte na {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "Vira [ {0} ] ni mogo\u010de najti.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Izhodna lastnost ni prepoznana: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Priprava primerka ElemLiteralResult ni uspela"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "Vrednost za {0} bi morala biti \u0161tevilka, ki jo je mogo\u010de raz\u010dleniti"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "Vrednost za {0} bi morala biti enaka da ali ne"},
				new object[] {ER_FAILED_CALLING_METHOD, "Klic metode {0} ni uspel"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Priprava primerka ElemTemplateElement ni uspela"},
				new object[] {ER_CHARS_NOT_ALLOWED, "V tem trenutku znaki v dokumentu niso na voljo"},
				new object[] {ER_ATTR_NOT_ALLOWED, "Atribut \"{0}\" v elementu {1} ni dovoljen!"},
				new object[] {ER_BAD_VALUE, "{0} slaba vrednost {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Vrednosti atributa {0} ni bilo mogo\u010de najti "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Vrednosti atributa {0} ni bilo mogo\u010de prepoznati "},
				new object[] {ER_NULL_URI_NAMESPACE, "Posku\u0161am generirati predpono imenskega prostora z URI z vrednostjo NULL"},
				new object[] {ER_NUMBER_TOO_BIG, "Poskus oblikovanja \u0161tevilke, ve\u010dje od najve\u010djega dolgega celega \u0161tevila"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Ne najdem razreda gonilnika SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "Na\u0161el razred gonilnika SAX1 {0}, vendar ga ne morem nalo\u017eiti"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "Nalo\u017eil razred gonilnika SAX1 {0}, vendar ga ne morem udejaniti"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "Razred gonilnika SAX1 {0} ne vklju\u010duje org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Sistemska lastnost org.xml.sax.parser ni dolo\u010dena"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Argument raz\u010dlenjevalnika sme biti NULL"},
				new object[] {ER_FEATURE, "Zna\u010dilnost: {0}"},
				new object[] {ER_PROPERTY, "Lastnost: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Razre\u0161evalnik entitet NULL"},
				new object[] {ER_NULL_DTD_HANDLER, "Program za obravnavanje DTD z vrednostjo NULL"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Ime gonilnika ni dolo\u010deno!"},
				new object[] {ER_NO_URL_SPECIFIED, "URL ni dolo\u010den!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "Zaloga je manj\u0161a od 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Dolo\u010deno neveljavno ime gonilnika!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Programerjeva napaka! Izraz nima nadrejenega ElemTemplateElement!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Programerjeva izjava v RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "Na tem polo\u017eaju v slogovni datoteki {0} ni dovoljen!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Besedilo, ki niso presledki in drugi podobni znaki, na tem polo\u017eaju v slogovni datoteki ni dovoljeno.!"},
				new object[] {INVALID_TCHAR, "Neveljavna vrednost: {1} uporabljena za atribut CHAR: {0}.  Atribut tipa CHAR mora biti samo 1 znak!"},
				new object[] {INVALID_QNAME, "Neveljavna vrednost: {1} uporabljena za atribut QNAME: {0}"},
				new object[] {INVALID_ENUM, "Neveljavna vrednost: {1} uporabljena za atribut ENUM: {0}.  Veljavne vrednosti so: {2}."},
				new object[] {INVALID_NMTOKEN, "Neveljavna vrednost: {1} uporabljena za atribut NMTOKEN: {0} "},
				new object[] {INVALID_NCNAME, "Neveljavna vrednost: {1} uporabljena za atribut NCNAME: {0} "},
				new object[] {INVALID_BOOLEAN, "Neveljavna vrednost: {1} uporabljena za atribut boolean: {0} "},
				new object[] {INVALID_NUMBER, "Neveljavna vrednost: {1} uporabljena za atribut number: {0} "},
				new object[] {ER_ARG_LITERAL, "Argument za {0} v primerjalnem vzorcu mora biti dobesedni niz."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Dvojnik deklaracije globalne spremenljivke."},
				new object[] {ER_DUPLICATE_VAR, "Dvojnik deklaracije spremenljivke."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template mora vsebovati atribut name ali match (ali oba)"},
				new object[] {ER_INVALID_PREFIX, "Predpona v izklju\u010di-predpone-rezultatov (exclude-result-prefixes) ni veljavna: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "Nabor atributov, imenovana {0}, ne obstaja"},
				new object[] {ER_FUNCTION_NOT_FOUND, "Funkcija, imenovana {0}, ne obstaja"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "Element {0} ne sme imeti vsebine in atributa izbire hkrati."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "Vrednost parametra {0} mora biti veljaven javanski objekt"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "Atribut result-prefix elementa xsl:namespace-alias element ima vrednost '#default' (privzeto), ampak ni deklaracije privzetega imenskega prostora v razponu za ta element."},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "Atribut result-prefix elementa xsl:namespace-alias ima vrednost ''{0}'', ampak ni deklaracije privzetega imenskega prostora za predpono ''{0}'' v razponu za ta element."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "Ime funkcije ne sme biti null v TransformerFactory.getFeature(Ime niza, vrednost boolean)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "Ime funkcije ne sme biti null v TransformerFactory.getFeature(Ime niza)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "Ni mogo\u010de nastaviti funkcije ''{0}'' v tem TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "Uporaba raz\u0161iritvene elementa ''{0}'' ni na voljo, ko je funkcija varnega procesiranja nastavljena na true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Ni mogo\u010de dobiti predpone za URI imenskega prostora null."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Ni mogo\u010de dobiti URI-ja imenskega prostora za predpono null."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "Ime funkcije ne more biti ni\u010d."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "\u0160tevilo argumentov ne more biti negativno"},
				new object[] {WG_FOUND_CURLYBRACE, "Najden '}' vendar ni odprtih predlog atributov!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Opozorilo: \u0161tevni atribut ni skladen s prednikom v xsl:number! Cilj = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Stara sintaksa: Ime atributa 'izraz' je bilo spremenjeno v 'izbira'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan \u0161e ne podpira podro\u010dnih imen v funkciji za oblikovanje \u0161tevilk."},
				new object[] {WG_LOCALE_NOT_FOUND, "Opozorilo: ne najdem podro\u010dnih nastavitev za xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Iz {0} ni mogo\u010de narediti naslova URL."},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Ne morem nalo\u017eiti zahtevanega dokumenta: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Ne najdem kolacionista (collator) za <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Stara sintaksa: navodilo za funkcije bi moralo uporabljati URL {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "Kodiranje ni podprto: {0}, uporabljen bo UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "kodiranje ni podprto: {0}, uporabljena bo Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Spori pri specifi\u010dnosti: uporabljen bo zadnji najdeni {0} v slogovni datoteki."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Poteka raz\u010dlenjevanje in priprava {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Predloga atributa, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Spor ujemanja med xsl:strip-space in xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan \u0161e ne podpira atributa {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Deklaracije za decimalno obliko ni bilo mogo\u010de najti: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Manjkajo\u010d ali nepravilen imenski prostor XSLT. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Dovoljena je samo ena privzeta deklaracija xsl:decimal-format."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "Imena xsl:decimal-format morajo biti enoli\u010dna. Ime \"{0}\" je bilo podvojeno."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} vsebuje neveljaven atribut: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Ne morem razre\u0161iti predpone imenskega prostora: {0}. Vozli\u0161\u010de bo prezrto."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet zahteva atribut 'razli\u010dica'!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Neveljavno ime atributa: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Uporabljena neveljavna vrednost za atribut {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "Posledi\u010dna skupina vozli\u0161\u010d iz drugega argumenta funkcije dokumenta je prazna. Posredujte prazno skupino vozli\u0161\u010d."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Vrednost atributa 'ime' iz imena xsl:processing-instruction ne sme biti 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Vrednost atributa ''ime'' iz xsl:processing-instruction mora biti veljavno NCIme: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Atributa {0} ne morem dodati za podrejenimi vozli\u0161\u010di ali pred izdelavo elementa.  Atribut bo prezrt."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Izveden je poskus spremembe objekta tam, kjer spremembe niso dovoljene."},
				new object[] {"ui_language", "sl"},
				new object[] {"help_language", "sl"},
				new object[] {"language", "sl"},
				new object[] {"BAD_CODE", "Parameter za createMessage presega meje"},
				new object[] {"FORMAT_FAILED", "Med klicem messageFormat naletel na izjemo"},
				new object[] {"version", ">>>>>>> Razli\u010dica Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "da"},
				new object[] {"line", "Vrstica #"},
				new object[] {"column","Stolpec #"},
				new object[] {"xsldone", "XSLProcessor: dokon\u010dano"},
				new object[] {"xslProc_option", "Ukazna vrstica Xalan-J Mo\u017enosti razreda postopka:"},
				new object[] {"xslProc_option", "Ukazna vrstica Xalan-J Mo\u017enosti razredov postopkov\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "Mo\u017enost {0} v na\u010dinu XSLTC ni podprta."},
				new object[] {"xslProc_invalid_xalan_option", "Mo\u017enost {0} se lahko uporablja samo z -XSLTC."},
				new object[] {"xslProc_no_input", "Napaka: ni dolo\u010dene slogovne datoteke ali vhodnega xml. Po\u017eenite ta ukaz, za katerega ni na voljo napotkov za uporabo."},
				new object[] {"xslProc_common_options", "-Splo\u0161ne mo\u017enosti-"},
				new object[] {"xslProc_xalan_options", "-Mo\u017enosti za Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Mo\u017enosti za XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(za nadaljevanje pritisnite <return>)"},
				new object[] {"optionXSLTC", "   [-XSLTC (za preoblikovanje uporabite XSLTC)]"},
				new object[] {"optionIN", "   [-IN vhodniXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLPreoblikovanjeURL]"},
				new object[] {"optionOUT", "   [-OUT ImeIzhodneDatoteke]"},
				new object[] {"optionLXCIN", "   [-LXCIN ImeVhodneDatotekePrevedeneSlogovneDatoteke]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT ImeIzhodneDatotekePrevedeneSlogovneDatoteke]"},
				new object[] {"optionPARSER", "   [-PARSER popolnoma ustrezno ime razreda zveze raz\u010dlenjevalnika]"},
				new object[] {"optionE", "   [-E (Ne raz\u0161irjajte sklicev entitet)]"},
				new object[] {"optionV", "   [-E (Ne raz\u0161irjajte sklicev entitet)]"},
				new object[] {"optionQC", "   [-QC (Tiha opozorila o sporih vzorcev)]"},
				new object[] {"optionQ", "   [-Q  (Tihi na\u010din)]"},
				new object[] {"optionLF", "   [-LF (Uporabite pomike samo na izhodu {privzeto je CR/LF})]"},
				new object[] {"optionCR", "   [-CR (Uporabite prehode v novo vrstico samo na izhodu {privzeto je CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Znaki za izogib {privzeto je <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Koliko presledkov zamika {privzeto je 0})]"},
				new object[] {"optionTT", "   [-TT (Sledite predlogam glede na njihov poziv.)]"},
				new object[] {"optionTG", "   [-TG (Sledite vsakemu dogodku rodu.)]"},
				new object[] {"optionTS", "   [-TS (Sledite vsakemu dogodku izbire.)]"},
				new object[] {"optionTTC", "   [-TTC (Sledite podrejenim predloge kot se obdelujejo.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (Razred TraceListener za kon\u010dnice sledi.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Nastavi v primeru preverjanja veljavnosti.  Privzeta vrednost za preverjanje veljavnosti je izklopljeno.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {izbirno ime datoteke} (V primeru napake naredi izvoz skladov.)]"},
				new object[] {"optionXML", "   [-XML (Uporabite oblikovalnik XML in dodajte glavo XML.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Uporabite preprost oblikovalnik besedila.)]"},
				new object[] {"optionHTML", "   [-HTML (Uporabite oblikovalnik za HTML.)]"},
				new object[] {"optionPARAM", "   [-PARAM izraz imena (nastavite parameter slogovne datoteke)]"},
				new object[] {"noParsermsg1", "Postopek XSL ni uspel."},
				new object[] {"noParsermsg2", "** Nisem na\u0161el raz\u010dlenjevalnika **"},
				new object[] {"noParsermsg3", "Preverite pot razreda."},
				new object[] {"noParsermsg4", "\u010ce nimate IBM raz\u010dlenjevalnika za Javo, ga lahko prenesete iz"},
				new object[] {"noParsermsg5", "IBM AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER polno ime razreda (URIResolver za razre\u0161evanje URL-jev)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER polno ime razreda (EntityResolver za razre\u0161evanje entitet)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER polno ime razreda (ContentHandler za serializacijo izhoda)]"},
				new object[] {"optionLINENUMBERS", "   [-L za izvorni dokument uporabite \u0161tevilke vrstic]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (nastavite funkcijo varne obdelave na True.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA TipMedija (z atributom medija poi\u0161\u010dite slogovno datoteko, ki se nana\u0161a na dokument.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR ImePosebnosti (Za preoblikovanje izrecno uporabljajte s2s=SAX ali d2d=DOM.)] "},
				new object[] {"optionDIAG", "   [-DIAG (Natisnite skupni \u010das trajanja pretvorbe v milisekundah.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (zahtevajte gradnjo prirastnega DTM tako, da nastavite http://xml.apache.org/xalan/features/incremental na resni\u010dno.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (prepre\u010dite obdelavo optimiziranja slogovne datoteke, tako da nastavite http://xml.apache.org/xalan/features/optimize na false.)]"},
				new object[] {"optionRL", "   [-RL mejaRekurzije (zahtevajte numeri\u010dno mejo globine rekurzije slogovne datoteke.)]"},
				new object[] {"optionXO", "   [-XO [imeTransleta] (dodelite ime ustvarjenemu transletu)]"},
				new object[] {"optionXD", "   [-XD ciljnaMapa (navedite ciljno mapo za translet)]"},
				new object[] {"optionXJ", "   [-XJ datotekaJar (zdru\u017ei razrede transleta v datoteko jar z imenom <jarfile>)]"},
				new object[] {"optionXP", "   [-XP paket (navede predpono imena paketa vsem ustvarjenim razredom transletov)]"},
				new object[] {"optionXN", "   [-XN (omogo\u010da vstavljanje predlog)]"},
				new object[] {"optionXX", "   [-XX (vklopi izhod za dodatna sporo\u010dila za iskanje napak)]"},
				new object[] {"optionXT", "   [-XT (\u010de je mogo\u010de, uporabite translet za pretvorbo)]"},
				new object[] {"diagTiming"," --------- Pretvorba {0} prek {1} je trajala {2} ms"},
				new object[] {"recursionTooDeep","Predloga pregloboko vgnezdena. Gnezdenje = {0}, predloga {1} {2}"},
				new object[] {"nameIs", "ime je"},
				new object[] {"matchPatternIs", "primerjalni vzorec je"}
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
	  public const string ERROR_HEADER = "Napaka: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Opozorilo: ";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public const string QUERY_HEADER = "VZOREC ";


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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("sl", "US"));
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