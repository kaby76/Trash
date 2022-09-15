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
 * $Id: XSLTErrorResources_cs.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_cs : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Chyba: Ve v\u00fdrazu nelze pou\u017e\u00edt znak '{'"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} m\u00e1 neplatn\u00fd atribut: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "Funkce sourceNode m\u00e1 v prvku xsl:apply-imports hodnotu null!"},
				new object[] {ER_CANNOT_ADD, "Nelze p\u0159idat {0} do {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "Funkce sourceNode m\u00e1 v instrukci handleApplyTemplatesInstruction hodnotu null!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} mus\u00ed m\u00edt jmenn\u00fd atribut"},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Nelze nal\u00e9zt \u0161ablonu s n\u00e1zvem: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Nelze nal\u00e9zt n\u00e1zev AVT v \u0161ablon\u011b xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} mus\u00ed m\u00edt atribut: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} mus\u00ed m\u00edt atribut ''test''. "},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Nespr\u00e1vn\u00e1 hodnota atributu \u00farovn\u011b: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "n\u00e1zev instrukce processing-instruction nem\u016f\u017ee b\u00fdt 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "n\u00e1zev instrukce processing-instruction mus\u00ed b\u00fdt platn\u00fd n\u00e1zev NCName: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "M\u00e1-li {0} re\u017eim, mus\u00ed m\u00edt vyhovuj\u00edc\u00ed atribut."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} vy\u017eaduje bu\u010f n\u00e1zev, nebo vyhovuj\u00edc\u00ed atribut."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Nelze p\u0159elo\u017eit p\u0159edponu oboru n\u00e1zv\u016f: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "Nepovolen\u00e1 hodnota prvku xml:space: {0}"},
				new object[] {ER_NO_OWNERDOC, "Uzel potomka nem\u00e1 dokument vlastn\u00edka!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Chyba funkce ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "Pokus o p\u0159id\u00e1n\u00ed potomka s hodnotou null!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} vy\u017eaduje atribut select."},
				new object[] {ER_NEED_TEST_ATTRIB, "Prvek xsl:when mus\u00ed obsahovat atribut 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "Prvek xsl:with-param mus\u00ed obsahovat atribut 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "Parametr context nem\u00e1 dokument vlastn\u00edka!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Nelze vytvo\u0159it prvek XML TransformerFactory Liaison: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: Proces nebyl \u00fasp\u011b\u0161n\u00fd."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: nebylo \u00fasp\u011b\u0161n\u00e9."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "K\u00f3dov\u00e1n\u00ed nen\u00ed podporov\u00e1no: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Nelze vytvo\u0159it TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "Prvek xsl:key vy\u017eaduje atribut 'name'!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "Prvek xsl:key vy\u017eaduje atribut 'match'!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "Prvek xsl:key vy\u017eaduje atribut 'use'!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} vy\u017eaduje atribut ''elements''!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) chyb\u00ed atribut ''prefix'' objektu {0}"},
				new object[] {ER_BAD_STYLESHEET_URL, "Nespr\u00e1vn\u00e1 adresa URL p\u0159edlohy se styly: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "Soubor p\u0159edlohy se styly nebyl nalezen: {0}"},
				new object[] {ER_IOEXCEPTION, "Soubor p\u0159edlohy se styly m\u00e1 v\u00fdjimku vstupu/v\u00fdstupu: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Pro {0} nelze naj\u00edt atribut href"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0}: p\u0159\u00edmo nebo nep\u0159\u00edmo zahrnuje sebe sama!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "Chyba: StylesheetHandler.processInclude {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) chyb\u00ed atribut ''lang'' objektu {0}"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) Nespr\u00e1vn\u011b um\u00edst\u011bn\u00fd prvek {0}?? Chyb\u00ed prvek po\u0159ada\u010de ''component''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "U atribut\u016f Element, DocumentFragment, Document a PrintWriter lze volat pouze v\u00fdstup."},
				new object[] {ER_PROCESS_ERROR, "Chyba: StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Chyba: UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Chyba! Nebyl nalezen v\u00fdraz v\u00fdb\u011bru xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Nelze serializovat XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Nebyl zad\u00e1n vstup p\u0159edlohy se styly!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Nepoda\u0159ilo se zpracovat p\u0159edlohu se styly!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Nelze analyzovat dokument {0}!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Nelze nal\u00e9zt fragment: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Uzel, na kter\u00fd odkazuje identifik\u00e1tor fragmentu, nen\u00ed prvek: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "atribut for-each mus\u00ed m\u00edt bu\u010f shodu, nebo n\u00e1zev atributu"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "atribut templates mus\u00ed m\u00edt bu\u010f shodu, nebo n\u00e1zev atributu"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "\u017d\u00e1dn\u00fd klon fragmentu dokumentu!"},
				new object[] {ER_CANT_CREATE_ITEM, "Ve stromu v\u00fdsledk\u016f nelze vytvo\u0159it polo\u017eku: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "Parametr xml:space ve zdrojov\u00e9m XML m\u00e1 neplatnou hodnotu: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "{0} nem\u00e1 deklarov\u00e1n \u017e\u00e1dn\u00fd parametr xsl:key!"},
				new object[] {ER_CANT_CREATE_URL, "Chyba! Nelze vytvo\u0159it url pro: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "Nepodporovan\u00e1 funkce xsl:functions"},
				new object[] {ER_PROCESSOR_ERROR, "Chyba: XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} - nen\u00ed v r\u00e1mci p\u0159edlohy se styly povoleno!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "Parametr result-ns ji\u017e nen\u00ed podporov\u00e1n!  M\u00edsto toho pou\u017eijte parametr xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "Parametr default-space ji\u017e nen\u00ed podporov\u00e1n!  M\u00edsto toho pou\u017eijte parametr xsl:strip-space nebo xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "Parametr indent-result ji\u017e nen\u00ed podporov\u00e1n!  M\u00edsto toho pou\u017eijte parametr xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} m\u00e1 neplatn\u00fd atribut: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Nezn\u00e1m\u00fd prvek XSL: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) Parametr xsl:sort lze pou\u017e\u00edt pouze s parametrem xsl:apply-templates nebo xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) Nespr\u00e1vn\u011b um\u00edst\u011bn\u00fd prvek xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) Prvkek xsl:when nem\u00e1 v parametru xsl:choose \u017e\u00e1dn\u00e9ho rodi\u010de!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) Nespr\u00e1vn\u011b um\u00edst\u011bn\u00fd prvek xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) Prvek xsl:otherwise nem\u00e1 v parametru xsl:choose \u017e\u00e1dn\u00e9ho rodi\u010de!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} - nen\u00ed v r\u00e1mci \u0161ablony povoleno!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0}: nezn\u00e1m\u00e1 p\u0159edpona {1} p\u0159\u00edpony oboru n\u00e1zv\u016f"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Importy mus\u00ed b\u00fdt v r\u00e1mci \u0161ablony se styly na prvn\u00edch m\u00edstech!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0}: p\u0159\u00edmo nebo nep\u0159\u00edmo importuje samo sebe!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) Parametr xml:space m\u00e1 neplatnou hodnotu: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "Ne\u00fasp\u011b\u0161n\u00fd proces processStylesheet!"},
				new object[] {ER_SAX_EXCEPTION, "V\u00fdjimka SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Nepodporovan\u00e1 funkce!"},
				new object[] {ER_XSLT_ERROR, "Chyba XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "znak m\u011bny nen\u00ed v \u0159et\u011bzci vzorku form\u00e1tu povolen"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Funkce Document nen\u00ed v p\u0159edloze se styly DOM podporov\u00e1na!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Nelze p\u0159elo\u017eit p\u0159edponu p\u0159eklada\u010de non-Prefix!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "P\u0159esm\u011brov\u00e1n\u00ed p\u0159\u00edpony: Nelze z\u00edskat n\u00e1zev souboru - atribut file nebo select mus\u00ed vr\u00e1tit platn\u00fd \u0159et\u011bzec."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "V p\u0159\u00edpon\u011b Redirect nelze vytvo\u0159it FormatterListener!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Neplatn\u00e1 p\u0159edpona ve funkci exclude-result-prefixes: {0}"},
				new object[] {ER_MISSING_NS_URI, "U zadan\u00e9 p\u0159edpony chyb\u00ed obor n\u00e1zv\u016f URI"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Chyb\u011bj\u00edc\u00ed argument volby: {0}"},
				new object[] {ER_INVALID_OPTION, "Neplatn\u00e1 volba: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Vadn\u00fd form\u00e1tovac\u00ed \u0159et\u011bzec: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "Prvek xsl:stylesheet vy\u017eaduje atribut 'version'!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Parametr Attribute: {0} m\u00e1 neplatnou hodnotu: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "Prvek xsl:choose vy\u017eaduje parametr xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "Parametr xsl:for-each nen\u00ed v xsl:apply-imports povolen"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Nelze pou\u017e\u00edt DTMLiaison u v\u00fdstupu uzlu DOM node... M\u00edsto toho pou\u017eijte org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Nelze pou\u017e\u00edt DTMLiaison u vstupu uzlu DOM node... M\u00edsto toho pou\u017eijte org.apache.xpath.DOM2Helper!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Ne\u00fasp\u011b\u0161n\u00e9 vol\u00e1n\u00ed prvku p\u0159\u00edpony: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "P\u0159edponu mus\u00ed b\u00fdt mo\u017eno p\u0159elo\u017eit do oboru n\u00e1zv\u016f: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Byla zji\u0161t\u011bna neplatn\u00e1 n\u00e1hrada UTF-16: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "Prvek xsl:attribute-set {0} pou\u017e\u00edv\u00e1 s\u00e1m sebe, co\u017e zp\u016fsob\u00ed nekone\u010dnou smy\u010dku."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Vstup Xerces-DOM nelze sm\u011b\u0161ovat s v\u00fdstupem Xerces-DOM!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "V ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Nalezena v\u00edce ne\u017e jedna \u0161ablona s n\u00e1zvem: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Neplatn\u00e9 vol\u00e1n\u00ed funkce: rekurzivn\u00ed vol\u00e1n\u00ed funkce key() nen\u00ed povoleno"},
				new object[] {ER_REFERENCING_ITSELF, "Prom\u011bnn\u00e1 {0} odkazuje p\u0159\u00edmo \u010di nep\u0159\u00edmo sama na sebe!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "Vstupn\u00ed uzel DOMSource pro newTemplates nesm\u00ed m\u00edt hodnotu null!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Nebyl nalezen soubor t\u0159\u00eddy pro volbu {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Nebyl nalezen po\u017eadovan\u00fd prvek: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "Parametr InputStream nesm\u00ed m\u00edt hodnotu null"},
				new object[] {ER_URI_CANNOT_BE_NULL, "Parametr URI nesm\u00ed m\u00edt hodnotu null"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "Parametr File nesm\u00ed m\u00edt hodnotu null"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "Parametr InputSource nesm\u00ed m\u00edt hodnotu null"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Nelze inicializovat BSF Manager"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "P\u0159\u00edponu nelze kompilovat"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Nelze vytvo\u0159it p\u0159\u00edponu: {0}, proto\u017ee: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Vol\u00e1n\u00ed metody {0} metodou Instance vy\u017eaduje jako prvn\u00ed argument instanci Object"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Byl zad\u00e1n neplatn\u00fd n\u00e1zev prvku {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "N\u00e1zev metody prvku mus\u00ed b\u00fdt static {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Funkce v\u00fdjimky {0} : {1} je nezn\u00e1m\u00e1"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Konstruktor {0} m\u00e1 v\u00edce nejlep\u0161\u00edch shod."},
				new object[] {ER_MORE_MATCH_METHOD, "Metoda {0} m\u00e1 v\u00edce nejlep\u0161\u00edch shod."},
				new object[] {ER_MORE_MATCH_ELEMENT, "Metoda prvku {0} m\u00e1 v\u00edce nejlep\u0161\u00edch shod."},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Do vyhodnocen\u00ed byl p\u0159ed\u00e1n neplatn\u00fd kontext {0}."},
				new object[] {ER_POOL_EXISTS, "Spole\u010dn\u00e1 oblast ji\u017e existuje."},
				new object[] {ER_NO_DRIVER_NAME, "Nebylo zad\u00e1no \u017e\u00e1dn\u00e9 jm\u00e9no ovlada\u010de."},
				new object[] {ER_NO_URL, "Nebyla specifikov\u00e1na \u017e\u00e1dn\u00e1 adresa URL."},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "Velikost spole\u010dn\u00e9 oblasti je men\u0161\u00ed ne\u017e jedna!"},
				new object[] {ER_INVALID_DRIVER, "Byl zad\u00e1n neplatn\u00fd n\u00e1zev ovlada\u010de!"},
				new object[] {ER_NO_STYLESHEETROOT, "Nebyl nalezen ko\u0159en p\u0159edlohy se styly!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Neplatn\u00e1 hodnota parametru xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "Selh\u00e1n\u00ed procesu processFromNode"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Nelze zav\u00e9st zdroj [ {0} ]: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Velikost vyrovn\u00e1vac\u00ed pam\u011bti <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "P\u0159i vol\u00e1n\u00ed p\u0159\u00edpony do\u0161lo k nezn\u00e1m\u00e9 chyb\u011b"},
				new object[] {ER_NO_NAMESPACE_DECL, "P\u0159edpona {0} nem\u00e1 deklarov\u00e1n odpov\u00eddaj\u00edc\u00ed obor n\u00e1zv\u016f"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Obsah prvku nen\u00ed povolen pro lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Ukon\u010den\u00ed sm\u011brovan\u00e9 na p\u0159edlohu se styly."},
				new object[] {ER_ONE_OR_TWO, "1 nebo 2"},
				new object[] {ER_TWO_OR_THREE, "2 nebo 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Nelze zav\u00e9st {0} (zkontrolujte prom\u011bnnou CLASSPATH) - proto se pou\u017e\u00edvaj\u00ed pouze v\u00fdchoz\u00ed hodnoty"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Nelze aktualizovat v\u00fdchoz\u00ed \u0161ablony."},
				new object[] {ER_RESULT_NULL, "V\u00fdsledek by nem\u011bl m\u00edt hodnotu null"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Nelze nastavit v\u00fdsledek"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Nebyl ur\u010den \u017e\u00e1dn\u00fd v\u00fdstup"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Nelze prov\u00e9st p\u0159evod na v\u00fdsledek typu {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Nelze prov\u00e9st p\u0159evod zdroje typu {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Obslu\u017en\u00fd program obsahu hodnoty null"},
				new object[] {ER_NULL_ERROR_HANDLER, "Obslu\u017en\u00fd program pro zpracov\u00e1n\u00ed chyb hodnoty null"},
				new object[] {ER_CANNOT_CALL_PARSE, "Nen\u00ed-li nastaven obslu\u017en\u00fd program ContentHandler, nelze volat analyz\u00e1tor."},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Filtr nem\u00e1 rodi\u010de."},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Nebyla nalezena p\u0159edloha se styly v: {0}, m\u00e9dium= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Nebyla nalezena p\u0159edloha se styly xml-stylesheet PI v: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Nepodporov\u00e1no: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "Hodnota vlastnosti {0} by m\u011bla b\u00fdt booleovsk\u00e1 instance"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Z {0} nelze z\u00edskat extern\u00ed skript."},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "Nelze naj\u00edt zdroj [ {0} ].\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Nezn\u00e1m\u00e1 vlastnost v\u00fdstupu: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Nepoda\u0159ilo se vytvo\u0159it instanci ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "Hodnota pro {0} by m\u011bla obsahovat analyzovateln\u00e9 \u010d\u00edslo"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "Hodnota {0} mus\u00ed b\u00fdt yes nebo no."},
				new object[] {ER_FAILED_CALLING_METHOD, "Vol\u00e1n\u00ed metody {0} selhalo."},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Nepoda\u0159ilo se vytvo\u0159it instanci ElemTemplateElement."},
				new object[] {ER_CHARS_NOT_ALLOWED, "V t\u00e9to \u010d\u00e1sti dokumentu nejsou znaky povoleny."},
				new object[] {ER_ATTR_NOT_ALLOWED, "Atribut \"{0}\" nen\u00ed u prvku {1} povolen!"},
				new object[] {ER_BAD_VALUE, "{0}: nespr\u00e1vn\u00e1 hodnota {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Atribut hodnoty ({0}) nebyl nalezen "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Atribut hodnoty ({0}) nebyl rozpozn\u00e1n "},
				new object[] {ER_NULL_URI_NAMESPACE, "Pokus o generov\u00e1n\u00ed oboru n\u00e1zv\u016f s URI s hodnotou null."},
				new object[] {ER_NUMBER_TOO_BIG, "Pokus o form\u00e1tov\u00e1n\u00ed v\u011bt\u0161\u00edho \u010d\u00edsla, ne\u017e je nejv\u011bt\u0161\u00ed dlouh\u00e9 cel\u00e9 \u010d\u00edslo."},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Nelze naj\u00edt t\u0159\u00eddu ovlada\u010de SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "T\u0159\u00edda ovlada\u010de SAX1 {0} byla nalezena, ale nebylo mo\u017eno ji zav\u00e9st."},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "T\u0159\u00edda ovlada\u010de SAX1 {0} byla nalezena, ale nebylo mo\u017eno s n\u00ed zalo\u017eit instanci."},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "T\u0159\u00edda ovlada\u010de SAX1 {0} neimplementuje org.xml.sax.Parser."},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Nebyla ur\u010dena vlastnost syst\u00e9mu org.xml.sax.parser."},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Argument analyz\u00e1toru nesm\u00ed m\u00edt hodnotu null."},
				new object[] {ER_FEATURE, "Funkce: {0}"},
				new object[] {ER_PROPERTY, "Vlastnost: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "\u0158e\u0161itel s hodnotou entity null"},
				new object[] {ER_NULL_DTD_HANDLER, "Obslu\u017en\u00fd program DTD s hodnotou null"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Nebyl zad\u00e1n n\u00e1zev ovlada\u010de!"},
				new object[] {ER_NO_URL_SPECIFIED, "Nebyla specifikov\u00e1na adresa URL!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "Velikost spole\u010dn\u00e9 oblasti je men\u0161\u00ed ne\u017e 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Zad\u00e1n neplatn\u00fd n\u00e1zev ovlada\u010de!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Chyba program\u00e1tora! Ve v\u00fdrazu chyb\u00ed nad\u0159azen\u00fd \u010dlen ElemTemplateElement."},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Tvrzen\u00ed program\u00e1tora v RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} - nen\u00ed povoleno v tomto stylu na dan\u00e9m m\u00edst\u011b!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Nepr\u00e1zdn\u00fd text nen\u00ed povolen v tomto stylu na dan\u00e9m m\u00edst\u011b!"},
				new object[] {INVALID_TCHAR, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro atribut CHAR: {0}. Atribut typu CHAR sm\u00ed m\u00edt pouze jeden znak."},
				new object[] {INVALID_QNAME, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro atribut QNAME: {0}"},
				new object[] {INVALID_ENUM, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro atribut ENUM {0}. Platn\u00e9 hodnoty jsou: {2}."},
				new object[] {INVALID_NMTOKEN, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro atribut NMTOKEN: {0} "},
				new object[] {INVALID_NCNAME, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro atribut NCNAME: {0} "},
				new object[] {INVALID_BOOLEAN, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro booleovsk\u00fd atribut: {0} "},
				new object[] {INVALID_NUMBER, "Neplatn\u00e1 hodnota: {1} pou\u017eito pro atribut \u010d\u00edsla: {0} "},
				new object[] {ER_ARG_LITERAL, "Argument pro {0} ve vyhovuj\u00edc\u00edm vzorku mus\u00ed b\u00fdt typu literal."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Duplicitn\u00ed deklarace glob\u00e1ln\u00ed prom\u011bnn\u00e9."},
				new object[] {ER_DUPLICATE_VAR, "Duplicitn\u00ed deklarace prom\u011bnn\u00e9."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "Prvek xsl:template mus\u00ed m\u00edt n\u00e1zev nebo odpov\u00eddaj\u00edc\u00ed atribut (nebo oboj\u00ed)"},
				new object[] {ER_INVALID_PREFIX, "Neplatn\u00e1 p\u0159edpona ve funkci exclude-result-prefixes: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "sada atribut\u016f pojmenovan\u00e1 {0} neexistuje"},
				new object[] {ER_FUNCTION_NOT_FOUND, "Funkce se jm\u00e9nem {0} neexistuje."},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "Prvek {0} nesm\u00ed obsahovat sou\u010dasn\u011b obsah i atribut volby."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "Hodnota parametru {0} mus\u00ed b\u00fdt platn\u00fdm objektem technologie Java."},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "Atribut result-prefix prvku xsl:namespace-alias m\u00e1 hodnotu '#default', neexistuje v\u0161ak \u017e\u00e1dn\u00e1 deklarace v\u00fdchoz\u00edho oboru n\u00e1zv\u016f v rozsahu dan\u00e9ho prvku"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "Atribut result-prefix prvku xsl:namespace-alias m\u00e1 hodnotu ''{0}'', neexistuje v\u0161ak \u017e\u00e1dn\u00e1 deklarace oboru n\u00e1zv\u016f pro p\u0159edponu ''{0}'' v rozsahu dan\u00e9ho prvku. "},
				new object[] {ER_SET_FEATURE_NULL_NAME, "N\u00e1zev funkce pou\u017eit\u00fd ve vol\u00e1n\u00ed TransformerFactory.setFeature(\u0159et\u011bzec n\u00e1zvu, booleovsk\u00e1 hodnota) nesm\u00ed m\u00edt hodnotu Null. "},
				new object[] {ER_GET_FEATURE_NULL_NAME, "N\u00e1zev funkce pou\u017eit\u00fd ve vol\u00e1n\u00ed TransformerFactory.getFeature(\u0159et\u011bzec n\u00e1zvu) nesm\u00ed m\u00edt hodnotu Null. "},
				new object[] {ER_UNSUPPORTED_FEATURE, "Nelze nastavit funkci ''{0}'' pro tuto t\u0159\u00eddu TransformerFactory. "},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "Je-li funkce zabezpe\u010den\u00e9ho zpracov\u00e1n\u00ed nastavena na hodnotu true, nen\u00ed povoleno pou\u017eit\u00ed roz\u0161i\u0159uj\u00edc\u00edho prvku ''{0}''. "},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Nelze na\u010d\u00edst p\u0159edponu pro identifik\u00e1tor URI, jeho\u017e obor n\u00e1zv\u016f m\u00e1 hodnotu Null. "},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Nelze na\u010d\u00edst identifik\u00e1tor URI oboru n\u00e1zv\u016f pro p\u0159edponu s hodnotou Null. "},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "N\u00e1zev funkce nesm\u00ed m\u00edt hodnotu Null. "},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "Arita nesm\u00ed m\u00edt z\u00e1pornou hodnotu. "},
				new object[] {WG_FOUND_CURLYBRACE, "Byl nalezen znak '}', ale nen\u00ed otev\u0159ena \u017e\u00e1dn\u00e1 \u0161ablona atributu!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Varov\u00e1n\u00ed: \u010d\u00edta\u010d atributu se neshoduje s p\u0159edch\u016fdcem v xsl:number! C\u00edl = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Star\u00e1 syntaxe: N\u00e1zev atributu 'expr' byl zm\u011bn\u011bn na 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan je\u0161t\u011b neobsluhuje n\u00e1zev n\u00e1rodn\u00edho prost\u0159ed\u00ed ve funkci format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Varov\u00e1n\u00ed: Nebylo nalezeno n\u00e1rodn\u00ed prost\u0159ed\u00ed pro parametr xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Nelze vytvo\u0159it adresu URL z: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Po\u017eadovan\u00fd dokument nelze na\u010d\u00edst: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Nelze naj\u00edt funkci Collator pro <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Star\u00e1 syntaxe: instrukce funkc\u00ed by m\u011bla pou\u017e\u00edvat adresu url {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "nepodporovan\u00e9 k\u00f3dov\u00e1n\u00ed: {0}, pou\u017eito k\u00f3dov\u00e1n\u00ed UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "nepodporovan\u00e9 k\u00f3dov\u00e1n\u00ed: {0}, pou\u017eita Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Byl nalezen konflikt specifi\u010dnosti: {0} Bude pou\u017eit naposledy nalezen\u00fd v\u00fdskyt z p\u0159edlohy se styly."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Anal\u00fdza a p\u0159\u00edprava {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "\u0160ablona atribut\u016f, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Konflikt souladu funkc\u00ed xsl:strip-space a xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan prozat\u00edm neobsluhuje atribut {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "U desetinn\u00e9ho form\u00e1tu nebyla nalezena \u017e\u00e1dn\u00e1 deklarace: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Chyb\u011bj\u00edc\u00ed nebo nespr\u00e1vn\u00fd obor n\u00e1zv\u016f XSLT. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Povolena je pouze v\u00fdchoz\u00ed deklarace prvku xsl:decimal-format."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "N\u00e1zvy prvk\u016f xsl:decimal-format mus\u00ed b\u00fdt jedine\u010dn\u00e9. Byla vytvo\u0159ena kopie n\u00e1zvu \"{0}\"."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} m\u00e1 neplatn\u00fd atribut: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Nelze p\u0159elo\u017eit p\u0159edponu oboru n\u00e1zv\u016f: {0}. Uzel bude ignorov\u00e1n."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "Prvek xsl:stylesheet vy\u017eaduje atribut 'version'!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Neplatn\u00fd n\u00e1zev atributu: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "V atributu {0} byla pou\u017eita neplatn\u00e1 hodnota: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "V\u00fdsledn\u00e9 nastaven\u00ed uzlu z druh\u00e9ho argumentu dokumentu je pr\u00e1zdn\u00e9. Vr\u00e1t\u00ed se pr\u00e1zdn\u00e1 sada uzl\u016f."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Hodnota atributu 'name' n\u00e1zvu prvku xsl:processing-instruction nesm\u00ed b\u00fdt 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Hodnota atributu ''name'' instrukce xsl:processing-instruction mus\u00ed b\u00fdt platn\u00fdm n\u00e1zvem NCName: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Nelze p\u0159idat atribut {0} po uzlech potomk\u016f ani p\u0159ed t\u00edm, ne\u017e je vytvo\u0159en prvek. Atribut bude ignorov\u00e1n."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Pokus o \u00fapravu objektu, u kter\u00e9ho nejsou \u00fapravy povoleny."},
				new object[] {"ui_language", "cs"},
				new object[] {"help_language", "cs"},
				new object[] {"language", "cs"},
				new object[] {"BAD_CODE", "Parametr funkce createMessage je mimo limit"},
				new object[] {"FORMAT_FAILED", "P\u0159i vol\u00e1n\u00ed funkce messageFormat do\u0161lo k v\u00fdjimce"},
				new object[] {"version", ">>>>>>> Verze Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "ano"},
				new object[] {"line", "\u0158\u00e1dek #"},
				new object[] {"column","Sloupec #"},
				new object[] {"xsldone", "XSLProcessor: hotovo"},
				new object[] {"xslProc_option", "P\u0159\u00edkazov\u00fd \u0159\u00e1dek Xalan-J: Zpracov\u00e1n\u00ed voleb t\u0159\u00eddy:"},
				new object[] {"xslProc_option", "P\u0159\u00edkazov\u00fd \u0159\u00e1dek Xalan-J: Zpracov\u00e1n\u00ed voleb t\u0159\u00eddy\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "Volba {0} nen\u00ed v re\u017eimu XSLTC podporovan\u00e1."},
				new object[] {"xslProc_invalid_xalan_option", "Volba {0} m\u016f\u017ee b\u00fdt pou\u017eita pouze s -XSLTC."},
				new object[] {"xslProc_no_input", "Chyba: \u017d\u00e1dn\u00e1 p\u0159edloha stylu ani vstup xml nejsou ur\u010deny. K zobrazen\u00ed pokyn\u016f spus\u0165te tento p\u0159\u00edkaz bez jak\u00e9koliv volby."},
				new object[] {"xslProc_common_options", "-Obecn\u00e9 volby-"},
				new object[] {"xslProc_xalan_options", "-Volby pro Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Volby pro XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(pokra\u010dujte stisknut\u00edm kl\u00e1vesy <Enter>)"},
				new object[] {"optionXSLTC", "   [-XSLTC (pou\u017eije XSLTC pro transformaci)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER pln\u011b kvalifikovan\u00fd n\u00e1zev t\u0159\u00eddy spojen\u00ed analyz\u00e1toru]"},
				new object[] {"optionE", "   [-E (neroz\u0161i\u0159ovat odkazy entity)]"},
				new object[] {"optionV", "   [-E (neroz\u0161i\u0159ovat odkazy entity)]"},
				new object[] {"optionQC", "   [-QC (varov\u00e1n\u00ed p\u0159ed konflikty vzorkov\u00e1n\u00ed v tich\u00e9m re\u017eimu)]"},
				new object[] {"optionQ", "   [-Q  (tich\u00fd re\u017eim)]"},
				new object[] {"optionLF", "   [-LF (ve v\u00fdstupu pou\u017e\u00edt pouze \u0159\u00e1dkov\u00e1n\u00ed - LF {v\u00fdchoz\u00ed nastaven\u00ed je CR/LF})]"},
				new object[] {"optionCR", "   [-CR (ve v\u00fdstupu pou\u017e\u00edt pouze znak nov\u00fd \u0159\u00e1dek - CR {v\u00fdchoz\u00ed nastaven\u00ed je CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (nastaven\u00ed znak\u016f escape sekvence {v\u00fdchoz\u00ed nastaven\u00ed je <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (ovliv\u0148uje po\u010det znak\u016f odsazen\u00ed {v\u00fdchoz\u00ed nastaven\u00ed je 0})]"},
				new object[] {"optionTT", "   [-TT (trasuje \u0161ablony p\u0159i vol\u00e1n\u00ed)]"},
				new object[] {"optionTG", "   [-TG (trasuje v\u0161echny ud\u00e1losti generov\u00e1n\u00ed)]"},
				new object[] {"optionTS", "   [-TS (trasuje v\u0161echny ud\u00e1losti v\u00fdb\u011bru)]"},
				new object[] {"optionTTC", "   [-TTC (trasuje potomky \u0161ablony v pr\u016fb\u011bhu jejich zpracov\u00e1n\u00ed)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (t\u0159\u00edda TraceListener p\u0159\u00edpon trasov\u00e1n\u00ed)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (zap\u00edn\u00e1/vyp\u00edn\u00e1 validaci;  v\u00fdchoz\u00ed nastaven\u00ed je vypnuto)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {voliteln\u00fd n\u00e1zev souboru} (p\u0159i chyb\u011b vyp\u00ed\u0161e obsah z\u00e1sobn\u00edku)]"},
				new object[] {"optionXML", "   [-XML (pou\u017eije program pro form\u00e1tov\u00e1n\u00ed XML a p\u0159id\u00e1 z\u00e1hlav\u00ed XML)]"},
				new object[] {"optionTEXT", "   [-TEXT (pou\u017eije jednoduch\u00fd program pro form\u00e1tov\u00e1n\u00ed textu)]"},
				new object[] {"optionHTML", "   [-HTML (pou\u017eije program pro form\u00e1tov\u00e1n\u00ed HTML)]"},
				new object[] {"optionPARAM", "   [-PARAM n\u00e1zev v\u00fdrazu (nastav\u00ed parametr p\u0159edlohy se styly)]"},
				new object[] {"noParsermsg1", "Proces XSL nebyl \u00fasp\u011b\u0161n\u00fd."},
				new object[] {"noParsermsg2", "** Nelze naj\u00edt analyz\u00e1tor **"},
				new object[] {"noParsermsg3", "Zkontrolujte cestu classpath."},
				new object[] {"noParsermsg4", "Nem\u00e1te-li analyz\u00e1tor XML jazyka Java spole\u010dnosti IBM, m\u016f\u017eete si jej st\u00e1hnout z adresy:"},
				new object[] {"noParsermsg5", "AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER cel\u00e9 jm\u00e9no t\u0159\u00eddy (pro p\u0159eklad URI pou\u017eije funkci URIResolver)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER cel\u00e9 jm\u00e9no t\u0159\u00eddy (pro p\u0159eklad entit pou\u017eije funkci EntityResolver)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER cel\u00e9 jm\u00e9no t\u0159\u00eddy (pro serializaci v\u00fdstupu pou\u017eije funkci ContentHandler)]"},
				new object[] {"optionLINENUMBERS", "   [-L ve zdrojov\u00e9m dokumentu pou\u017eije \u010d\u00edsla \u0159\u00e1dk\u016f]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (nastav\u00ed funkci zabezpe\u010den\u00e9ho zpracov\u00e1n\u00ed na hodnotu True.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType (k vyhled\u00e1n\u00ed p\u0159edlohy se styly p\u0159i\u0159azen\u00e9 dokumentu pou\u017eije atribut m\u00e9dia)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName (p\u0159i transformaci se explicitn\u011b pou\u017eije s2s=SAX nebo d2d=DOM)] "},
				new object[] {"optionDIAG", "   [-DIAG (vytiskne \u010das transformace v milisekund\u00e1ch)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (vy\u017eaduje inkrement\u00e1ln\u00ed konstrukci DTM nastaven\u00edm hodnoty http://xml.apache.org/xalan/features/incremental na true)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (vy\u017eaduje optimalizaci p\u0159edlohy se styly nastaven\u00edm hodnoty http://xml.apache.org/xalan/features/optimize na false)]"},
				new object[] {"optionRL", "   [-RL recursionlimit (potvrd\u00ed \u010d\u00edseln\u00fd limit hloubky p\u0159edlohy se styly)]"},
				new object[] {"optionXO", "   [-XO [transletName] (p\u0159i\u0159ad\u00ed n\u00e1zev k generovan\u00e9mu transletu)]"},
				new object[] {"optionXD", "   [-XD destinationDirectory (ur\u010duje c\u00edlov\u00fd adres\u00e1\u0159 pro translet)]"},
				new object[] {"optionXJ", "   [-XJ jarfile (zabal\u00ed t\u0159\u00eddy transletu do souboru jar s n\u00e1zvem <jarfile>)]"},
				new object[] {"optionXP", "   [-XP package (ur\u010d\u00ed p\u0159edponu n\u00e1zvu sady pro v\u0161echny generovan\u00e9 t\u0159\u00eddy transletu)]"},
				new object[] {"optionXN", "   [-XN (povol\u00ed zarovn\u00e1n\u00ed \u0161ablon)]"},
				new object[] {"optionXX", "   [-XX (zapne dal\u0161\u00ed v\u00fdstup zpr\u00e1v lad\u011bn\u00ed)]"},
				new object[] {"optionXT", "   [-XT (Pou\u017eije translet k transformaci, je-li to mo\u017en\u00e9)]"},
				new object[] {"diagTiming"," --------- Transformace {0} pomoc\u00ed {1} trvala {2} ms."},
				new object[] {"recursionTooDeep","Vno\u0159en\u00ed \u0161ablon je p\u0159\u00edli\u0161 hlubok\u00e9. Vno\u0159en\u00ed = {0}, \u0161ablona {1} {2}"},
				new object[] {"nameIs", "n\u00e1zev je"},
				new object[] {"matchPatternIs", "vzorek shody je"}
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
	  public const string ERROR_STRING = "#chyba";

	  /// <summary>
	  /// String to prepend to error messages. </summary>
	  public const string ERROR_HEADER = "Chyba: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Varov\u00e1n\u00ed: ";

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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("cs", "CZ"));
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