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
 * $Id: XSLTErrorResources.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources : ListResourceBundle
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
	  /// <returns> The int to message lookup table. </returns>
	  public virtual object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ER0000", "{0}"},
				new object[] {ER_NO_CURLYBRACE, "Error: Can not have '{' within expression"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} has an illegal attribute: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode is null in xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "Can not add {0} to {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode is null in handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} must have a name attribute."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Could not find template named: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Could not resolve name AVT in xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} requires attribute: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} must have a ''test'' attribute."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Bad value on level attribute: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "processing-instruction name can not be 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "processing-instruction name must be a valid NCName: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} must have a match attribute if it has a mode."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} requires either a name or a match attribute."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Can not resolve namespace prefix: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space has an illegal value: {0}"},
				new object[] {ER_NO_OWNERDOC, "Child node does not have an owner document!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "ElemTemplateElement error: {0}"},
				new object[] {ER_NULL_CHILD, "Trying to add a null child!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} requires a select attribute."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when must have a 'test' attribute."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param must have a 'name' attribute."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "context does not have an owner document!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Could not create XML TransformerFactory Liaison: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: Process was not successful."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: was not successful."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Encoding not supported: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Could not create TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key requires a 'name' attribute!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key requires a 'match' attribute!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key requires a 'use' attribute!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} requires an ''elements'' attribute!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} attribute ''prefix'' is missing"},
				new object[] {ER_BAD_STYLESHEET_URL, "Stylesheet URL is bad: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "Stylesheet file was not found: {0}"},
				new object[] {ER_IOEXCEPTION, "Had IO Exception with stylesheet file: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Could not find href attribute for {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} is directly or indirectly including itself!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "StylesheetHandler.processInclude error, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} attribute ''lang'' is missing"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) misplaced {0} element?? Missing container element ''component''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Can only output to an Element, DocumentFragment, Document, or PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "StylesheetRoot.process error"},
				new object[] {ER_UNIMPLNODE_ERROR, "UnImplNode error: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Error! Did not find xpath select expression (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Can not serialize an XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Stylesheet input was not specified!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Failed to process stylesheet!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Could not parse {0} document!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Could not find fragment: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Node pointed to by fragment identifier was not an element: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each must have either a match or name attribute"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "templates must have either a match or name attribute"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "No clone of a document fragment!"},
				new object[] {ER_CANT_CREATE_ITEM, "Can not create item in result tree: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space in the source XML has an illegal value: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "There is no xsl:key declaration for {0}!"},
				new object[] {ER_CANT_CREATE_URL, "Error! Cannot create url for: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions is unsupported"},
				new object[] {ER_PROCESSOR_ERROR, "XSLT TransformerFactory Error"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} not allowed inside a stylesheet!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns no longer supported!  Use xsl:output instead."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space no longer supported!  Use xsl:strip-space or xsl:preserve-space instead."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result no longer supported!  Use xsl:output instead."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} has an illegal attribute: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Unknown XSL element: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort can only be used with xsl:apply-templates or xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) misplaced xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when not parented by xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) misplaced xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise not parented by xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} is not allowed inside a template!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0} extension namespace prefix {1} unknown"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Imports can only occur as the first elements in the stylesheet!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} is directly or indirectly importing itself!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) " + "xml:space has an illegal value: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet not succesfull!"},
				new object[] {ER_SAX_EXCEPTION, "SAX Exception"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Function not supported!"},
				new object[] {ER_XSLT_ERROR, "XSLT Error"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "currency sign is not allowed in format pattern string"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Document function not supported in Stylesheet DOM!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Can't resolve prefix of non-Prefix resolver!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Redirect extension: Could not get filename - file or select attribute must return vald string."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Can not build FormatterListener in Redirect extension!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Prefix in exclude-result-prefixes is not valid: {0}"},
				new object[] {ER_MISSING_NS_URI, "Missing namespace URI for specified prefix"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Missing argument for option: {0}"},
				new object[] {ER_INVALID_OPTION, "Invalid option: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Malformed format string: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet requires a 'version' attribute!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Attribute: {0} has an illegal value: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose requires an xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports not allowed in a xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Cannot use a DTMLiaison for an output DOM node... pass a org.apache.xpath.DOM2Helper instead!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Cannot use a DTMLiaison for a input DOM node... pass a org.apache.xpath.DOM2Helper instead!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Call to extension element failed: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Prefix must resolve to a namespace: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Invalid UTF-16 surrogate detected: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} used itself, which will cause an infinite loop."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Can not mix non Xerces-DOM input with Xerces-DOM output!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "In ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Found more than one template named: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Invalid function call: recursive key() calls are not allowed"},
				new object[] {ER_REFERENCING_ITSELF, "Variable {0} is directly or indirectly referencing itself!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "The input node can not be null for a DOMSource for newTemplates!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Class file not found for option {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Required Element not found: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream cannot be null"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI cannot be null"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "File cannot be null"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource cannot be null"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Could not initialize BSF Manager"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Could not compile extension"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Could not create extension: {0} because of: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Instance method call to method {0} requires an Object instance as first argument"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Invalid element name specified {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Element name method must be static {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Extension function {0} : {1} is unknown"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "More than one best match for constructor for {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "More than one best match for method {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "More than one best match for element method {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Invalid context passed to evaluate {0}"},
				new object[] {ER_POOL_EXISTS, "Pool already exists"},
				new object[] {ER_NO_DRIVER_NAME, "No driver Name specified"},
				new object[] {ER_NO_URL, "No URL specified"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "Pool size is less than one!"},
				new object[] {ER_INVALID_DRIVER, "Invalid driver name specified!"},
				new object[] {ER_NO_STYLESHEETROOT, "Did not find the stylesheet root!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Illegal value for xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode failed"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "The resource [ {0} ] could not load: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Buffer size <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Unknown error when calling extension"},
				new object[] {ER_NO_NAMESPACE_DECL, "Prefix {0} does not have a corresponding namespace declaration"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Element content not allowed for lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Stylesheet directed termination"},
				new object[] {ER_ONE_OR_TWO, "1 or 2"},
				new object[] {ER_TWO_OR_THREE, "2 or 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Could not load {0} (check CLASSPATH), now using just the defaults"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Cannot initialize default templates"},
				new object[] {ER_RESULT_NULL, "Result should not be null"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Result could not be set"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "No output specified"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Can''t transform to a Result of type {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Can''t transform a Source of type {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Null content handler"},
				new object[] {ER_NULL_ERROR_HANDLER, "Null error handler"},
				new object[] {ER_CANNOT_CALL_PARSE, "parse can not be called if the ContentHandler has not been set"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "No parent for filter"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "No stylesheet found in: {0}, media= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "No xml-stylesheet PI found in: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Not supported: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "Value for property {0} should be a Boolean instance"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Could not get to external script at {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "The resource [ {0} ] could not be found.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Output property not recognized: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Failed creating ElemLiteralResult instance"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "Value for {0} should contain a parsable number"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "Value for {0} should equal yes or no"},
				new object[] {ER_FAILED_CALLING_METHOD, "Failed calling {0} method"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Failed creating ElemTemplateElement instance"},
				new object[] {ER_CHARS_NOT_ALLOWED, "Characters are not allowed at this point in the document"},
				new object[] {ER_ATTR_NOT_ALLOWED, "\"{0}\" attribute is not allowed on the {1} element!"},
				new object[] {ER_BAD_VALUE, "{0} bad value {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "{0} attribute value not found "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "{0} attribute value not recognized "},
				new object[] {ER_NULL_URI_NAMESPACE, "Attempting to generate a namespace prefix with a null URI"},
				new object[] {ER_NUMBER_TOO_BIG, "Attempting to format a number bigger than the largest Long integer"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Cannot find SAX1 driver class {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "SAX1 driver class {0} found but cannot be loaded"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "SAX1 driver class {0} loaded but cannot be instantiated"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "SAX1 driver class {0} does not implement org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "System property org.xml.sax.parser not specified"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Parser argument must not be null"},
				new object[] {ER_FEATURE, "Feature: {0}"},
				new object[] {ER_PROPERTY, "Property: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Null entity resolver"},
				new object[] {ER_NULL_DTD_HANDLER, "Null DTD handler"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "No Driver Name Specified!"},
				new object[] {ER_NO_URL_SPECIFIED, "No URL Specified!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "Pool size is less than 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Invalid Driver Name Specified!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Programmer's error! The expression has no ElemTemplateElement parent!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Programmer''s assertion in RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} is not allowed in this position in the stylesheet!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Non-whitespace text is not allowed in this position in the stylesheet!"},
				new object[] {INVALID_TCHAR, "Illegal value: {1} used for CHAR attribute: {0}.  An attribute of type CHAR must be only 1 character!"},
				new object[] {INVALID_QNAME, "Illegal value: {1} used for QNAME attribute: {0}"},
				new object[] {INVALID_ENUM, "Illegal value: {1} used for ENUM attribute: {0}.  Valid values are: {2}."},
				new object[] {INVALID_NMTOKEN, "Illegal value: {1} used for NMTOKEN attribute: {0} "},
				new object[] {INVALID_NCNAME, "Illegal value: {1} used for NCNAME attribute: {0} "},
				new object[] {INVALID_BOOLEAN, "Illegal value: {1} used for boolean attribute: {0} "},
				new object[] {INVALID_NUMBER, "Illegal value: {1} used for number attribute: {0} "},
				new object[] {ER_ARG_LITERAL, "Argument to {0} in match pattern must be a literal."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Duplicate global variable declaration."},
				new object[] {ER_DUPLICATE_VAR, "Duplicate variable declaration."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template must have a name or match attribute (or both)"},
				new object[] {ER_INVALID_PREFIX, "Prefix in exclude-result-prefixes is not valid: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "attribute-set named {0} does not exist"},
				new object[] {ER_FUNCTION_NOT_FOUND, "The function named {0} does not exist"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "The {0} element must not have both content and a select attribute."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "The value of param {0} must be a valid Java Object"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "The result-prefix attribute of an xsl:namespace-alias element has the value '#default', but there is no declaration of the default namespace in scope for the element"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "The result-prefix attribute of an xsl:namespace-alias element has the value ''{0}'', but there is no namespace declaration for the prefix ''{0}'' in scope for the element."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "The feature name cannot be null in TransformerFactory.setFeature(String name, boolean value)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "The feature name cannot be null in TransformerFactory.getFeature(String name)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "Cannot set the feature ''{0}'' on this TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "Use of the extension element ''{0}'' is not allowed when the secure processing feature is set to true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Cannot get the prefix for a null namespace uri."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Cannot get the namespace uri for null prefix."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "The function name cannot be null."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "The arity cannot be negative."},
				new object[] {WG_FOUND_CURLYBRACE, "Found '}' but no attribute template open!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Warning: count attribute does not match an ancestor in xsl:number! Target = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Old syntax: The name of the 'expr' attribute has been changed to 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan doesn't yet handle the locale name in the format-number function."},
				new object[] {WG_LOCALE_NOT_FOUND, "Warning: Could not find locale for xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Can not make URL from: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Can not load requested doc: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Could not find Collator for <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Old syntax: the functions instruction should use a url of {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "encoding not supported: {0}, using UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "encoding not supported: {0}, using Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Specificity conflicts found: {0} Last found in stylesheet will be used."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Parsing and preparing {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Attr Template, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Match conflict between xsl:strip-space and xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan does not yet handle the {0} attribute!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "No declaration found for decimal format: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Missing or incorrect XSLT Namespace. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Only one default xsl:decimal-format declaration is allowed."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "xsl:decimal-format names must be unique. Name \"{0}\" has been duplicated."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} has an illegal attribute: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Could not resolve namespace prefix: {0}. The node will be ignored."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet requires a 'version' attribute!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Illegal attribute name: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Illegal value used for attribute {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "Resulting nodeset from second argument of document function is empty. Return an empty node-set."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "The value of the 'name' attribute of xsl:processing-instruction name must not be 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "The value of the ''name'' attribute of xsl:processing-instruction must be a valid NCName: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Cannot add attribute {0} after child nodes or before an element is produced.  Attribute will be ignored."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "An attempt is made to modify an object where modifications are not allowed."},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "Parameter to createMessage was out of bounds"},
				new object[] {"FORMAT_FAILED", "Exception thrown during messageFormat call"},
				new object[] {"version", ">>>>>>> Xalan Version "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "yes"},
				new object[] {"line", "Line #"},
				new object[] {"column", "Column #"},
				new object[] {"xsldone", "XSLProcessor: done"},
				new object[] {"xslProc_option", "Xalan-J command line Process class options:"},
				new object[] {"xslProc_option", "Xalan-J command line Process class options\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "The option {0} is not supported in XSLTC mode."},
				new object[] {"xslProc_invalid_xalan_option", "The option {0} can only be used with -XSLTC."},
				new object[] {"xslProc_no_input", "Error: No stylesheet or input xml is specified. Run this command without any option for usage instructions."},
				new object[] {"xslProc_common_options", "-Common Options-"},
				new object[] {"xslProc_xalan_options", "-Options for Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Options for XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(press <return> to continue)"},
				new object[] {"optionXSLTC", "   [-XSLTC (use XSLTC for transformation)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER fully qualified class name of parser liaison]"},
				new object[] {"optionE", "   [-E (Do not expand entity refs)]"},
				new object[] {"optionV", "   [-E (Do not expand entity refs)]"},
				new object[] {"optionQC", "   [-QC (Quiet Pattern Conflicts Warnings)]"},
				new object[] {"optionQ", "   [-Q  (Quiet Mode)]"},
				new object[] {"optionLF", "   [-LF (Use linefeeds only on output {default is CR/LF})]"},
				new object[] {"optionCR", "   [-CR (Use carriage returns only on output {default is CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Which characters to escape {default is <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Control how many spaces to indent {default is 0})]"},
				new object[] {"optionTT", "   [-TT (Trace the templates as they are being called.)]"},
				new object[] {"optionTG", "   [-TG (Trace each generation event.)]"},
				new object[] {"optionTS", "   [-TS (Trace each selection event.)]"},
				new object[] {"optionTTC", "   [-TTC (Trace the template children as they are being processed.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (TraceListener class for trace extensions.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Set whether validation occurs.  Validation is off by default.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {optional filename} (Do stackdump on error.)]"},
				new object[] {"optionXML", "   [-XML (Use XML formatter and add XML header.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Use simple Text formatter.)]"},
				new object[] {"optionHTML", "   [-HTML (Use HTML formatter.)]"},
				new object[] {"optionPARAM", "   [-PARAM name expression (Set a stylesheet parameter)]"},
				new object[] {"noParsermsg1", "XSL Process was not successful."},
				new object[] {"noParsermsg2", "** Could not find parser **"},
				new object[] {"noParsermsg3", "Please check your classpath."},
				new object[] {"noParsermsg4", "If you don't have IBM's XML Parser for Java, you can download it from"},
				new object[] {"noParsermsg5", "IBM's AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER full class name (URIResolver to be used to resolve URIs)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER full class name (EntityResolver to be used to resolve entities)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER full class name (ContentHandler to be used to serialize output)]"},
				new object[] {"optionLINENUMBERS", "   [-L use line numbers for source document]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (set the secure processing feature to true.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType (use media attribute to find stylesheet associated with a document.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName (Explicitly use s2s=SAX or d2d=DOM to do transform.)] "},
				new object[] {"optionDIAG", "   [-DIAG (Print overall milliseconds transform took.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (request incremental DTM construction by setting http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (request no stylesheet optimization processing by setting http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", "   [-RL recursionlimit (assert numeric limit on stylesheet recursion depth.)]"},
				new object[] {"optionXO", "   [-XO [transletName] (assign the name to the generated translet)]"},
				new object[] {"optionXD", "   [-XD destinationDirectory (specify a destination directory for translet)]"},
				new object[] {"optionXJ", "   [-XJ jarfile (packages translet classes into a jar file of name <jarfile>)]"},
				new object[] {"optionXP", "   [-XP package (specifies a package name prefix for all generated translet classes)]"},
				new object[] {"optionXN", "   [-XN (enables template inlining)]"},
				new object[] {"optionXX", "   [-XX (turns on additional debugging message output)]"},
				new object[] {"optionXT", "   [-XT (use translet to transform if possible)]"},
				new object[] {"diagTiming", " --------- Transform of {0} via {1} took {2} ms"},
				new object[] {"recursionTooDeep", "Template nesting too deep. nesting = {0}, template {1} {2}"},
				new object[] {"nameIs", "name is"},
				new object[] {"matchPatternIs", "match pattern is"}
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
	  public const string WARNING_HEADER = "Warning: ";

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