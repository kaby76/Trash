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
 * $Id: XSLTErrorResources_it.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_it : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Errore: '{' non pu\u00f2 essere contenuto in un'espressione"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} ha un attributo non valido: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode nullo in xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "Impossibile aggiungere {0} a {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode nullo in handleApplyTemplatesInstruction."},
				new object[] {ER_NO_NAME_ATTRIB, "{0} deve avere un attributo name."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Impossibile trovare la maschera: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Impossibile risolvere il nome AVT in xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} richiede l''''attributo: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} deve avere un attributo ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Valore errato nell''''attributo livello: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Il nome dell'istruzione di elaborazione non pu\u00f2 essere 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "il nome dell''''istruzione di elaborazione deve essere un NCName valido: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} deve avere un attributo match nel caso abbia un modo."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} richiede un attributo match o name."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Impossibile risolvere il prefisso dello namespace: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space ha un valore non valido: {0}"},
				new object[] {ER_NO_OWNERDOC, "Il nodo secondario non ha un documento proprietario."},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Errore ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "\u00c8 stato effettuato un tentativo di aggiungere un secondario nullo."},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} richiede un attributo select."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when deve avere un attributo 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param deve avere un attributo 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "il contesto non ha un documento proprietario."},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Impossibile creare XML TransformerFactory Liaison: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: Processo non eseguito correttamente."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: non eseguito correttamente."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Codifica non supportata: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Impossibile creare TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key richiede un attributo 'name'."},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key richiede un attributo 'match'."},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key richiede un attributo 'use'."},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} richiede un attributo ''elements''."},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} attributo ''prefix'' mancante"},
				new object[] {ER_BAD_STYLESHEET_URL, "URL del foglio di lavoro errato: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "File del foglio di lavoro non trovato: {0}"},
				new object[] {ER_IOEXCEPTION, "Eccezione IO nel file del foglio di lavoro: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Impossibile trovare l''''attributo href per {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} sta direttamente o indirettamente includendo se stesso."},
				new object[] {ER_PROCESSINCLUDE_ERROR, "Errore StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} attributo ''lang'' mancante"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) elemento {0} non ubicato correttamente. Elemento contenitore ''component'' mancante "},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "L'emissione \u00e8 consentita solo in un elemento, frammento di documento, documento o stampante."},
				new object[] {ER_PROCESS_ERROR, "Errore StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Errore UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Errore! Impossibile trovare espressione selezione xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Impossibile serializzare XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Input del foglio di lavoro non specificato."},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Impossibile elaborare il foglio di lavoro."},
				new object[] {ER_COULDNT_PARSE_DOC, "Impossibile analizzare il documento {0}."},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Impossibile trovare il frammento: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Il nodo a cui fa riferimento l''''identificativo del frammento non \u00e8 un elemento: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each deve avere un attributo match o name"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "le maschere devono avere un attributo match o name"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Non \u00e8 possibile avere un clone di un frammento di documento."},
				new object[] {ER_CANT_CREATE_ITEM, "Impossibile creare la voce nella struttura dei risultati: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space in XML di origine ha un valore non valido: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Nessuna dichiarazione xsl:key per {0}!"},
				new object[] {ER_CANT_CREATE_URL, "Errore! Impossibile creare url per: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions non supportato"},
				new object[] {ER_PROCESSOR_ERROR, "Errore XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} non consentito nel foglio di lavoro."},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns non \u00e8 pi\u00f9 supportato.  Utilizzare xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space non \u00e8 pi\u00f9 supportato.  Utilizzare xsl:strip-space oppure xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result non \u00e8 pi\u00f9 supportato.  Utilizzare xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} ha un attributo non valido: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Elemento XSL sconosciuto: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort pu\u00f2 essere utilizzato solo con xsl:apply-templates oppure xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) xsl:when posizionato in modo non corretto."},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when non reso principale da xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) xsl:otherwise posizionato in modo non corretto."},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise non reso principale da xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} non \u00e8 consentito in una maschera."},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0} prefisso namespace estensione {1} sconosciuto"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Le importazioni possono verificarsi solo come primi elementi nel foglio di lavoro."},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} sta direttamente o indirettamente importando se stesso."},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space ha un valore non valido: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet con esito negativo."},
				new object[] {ER_SAX_EXCEPTION, "Eccezione SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Funzione non supportata."},
				new object[] {ER_XSLT_ERROR, "Errore XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "il simbolo della valuta non \u00e8 consentito nella stringa modello formato."},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "La funzione documento non \u00e8 supportata nel DOM del foglio di lavoro."},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Impossibile risolvere il prefisso di un resolver non di prefisso."},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Redirect extension: Impossibile richiamare il nome file - l'attributo file o select deve restituire una stringa valida."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Impossibile creare FormatterListener in Redirect extension!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Prefisso in exclude-result-prefixes non valido: {0}"},
				new object[] {ER_MISSING_NS_URI, "URI spazio nome mancante per il prefisso specificato"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Argomento mancante per l''''opzione: {0}"},
				new object[] {ER_INVALID_OPTION, "Opzione non valida: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Stringa di formato errato: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet richiede un attributo 'version'."},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "L''attributo: {0} ha un valore non valido: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose richiede xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports non consentito in xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Impossibile utilizzare DTMLiaison per un nodo DOM di output... utilizzare invece org.apache.xpath.DOM2Helper."},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Impossibile utilizzare DTMLiaison per un nodo DON di input... utilizzare invece org.apache.xpath.DOM2Helper."},
				new object[] {ER_CALL_TO_EXT_FAILED, "Chiamata all''''elemento estensione non riuscita: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Il prefisso deve risolvere in uno namespace: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Rilevato surrogato UTF-16 non valido: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} sta utilizzando se stesso, determinando un loop infinito."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Impossibile unire input non Xerces-DOM con output Xerces-DOM."},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "In ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Sono state rilevate pi\u00f9 maschere denominate: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Chiamata funzione non valida: le chiamate key() ricorsive non sono consentite"},
				new object[] {ER_REFERENCING_ITSELF, "La variabile {0} sta direttamente o indirettamente facendo riferimento a se stessa."},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "Il nodo di input non pu\u00f2 essere nullo per DOMSource per newTemplates."},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "File di classe non trovato per l''opzione {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Elemento richiesto non trovato: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream non pu\u00f2 essere nullo"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI non pu\u00f2 essere nullo"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "File non pu\u00f2 essere nullo"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource non pu\u00f2 essere nullo"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Impossibile inizializzare BSF Manager"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Impossibile compilare l'estensione"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Impossibile creare l''''estensione: {0} a causa di: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "La chiamata metodo istanza al metodo {0} richiede un''istanza Object come primo argomento"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Specificato nome elemento non valido{0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Il metodo nome elemento deve essere statico {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Funzione estensione {0} : {1} sconosciuta"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "\u00c8 stata trovata pi\u00f9 di una corrispondenza migliore per il costruttore per {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "\u00c8 stata trovata pi\u00f9 di una corrispondenza migliore per il metodo {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "\u00c8 stata trovata pi\u00f9 di una corrispondenza migliore per il metodo elemento {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Specificato contesto non valido per valutare {0}"},
				new object[] {ER_POOL_EXISTS, "Pool gi\u00e0 esistente"},
				new object[] {ER_NO_DRIVER_NAME, "Non \u00e8 stato specificato alcun Nome driver"},
				new object[] {ER_NO_URL, "Non \u00e8 stata specificata alcuna URL"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "La dimensione del pool \u00e8 inferiore a uno."},
				new object[] {ER_INVALID_DRIVER, "Specificato nome driver non valido."},
				new object[] {ER_NO_STYLESHEETROOT, "Impossibile trovare la root del foglio di lavoro."},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Valore non valido per xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode non riuscito"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Impossibile caricare la risorsa [ {0} ]: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Dimensione buffer <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Errore sconosciuto durante la chiamata all'estensione"},
				new object[] {ER_NO_NAMESPACE_DECL, "Il prefisso {0} non ha una dichiarazione namaspace corrispondente"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Contenuto elemento non consentito per lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Il foglio di lavoro ha indirizzato l'interruzione"},
				new object[] {ER_ONE_OR_TWO, "1 o 2"},
				new object[] {ER_TWO_OR_THREE, "2 o 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Impossibile caricare {0} (controllare CLASSPATH), verranno utilizzati i valori predefiniti."},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Impossibile inizializzare le maschere predefinite"},
				new object[] {ER_RESULT_NULL, "Il risultato non pu\u00f2 essere nullo"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Impossibile impostare il risultato"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Non \u00e8 stato specificato alcun output"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Impossibile trasformare in un risultato di tipo {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Impossibile trasformare in un''origine di tipo {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Handler contenuto nullo"},
				new object[] {ER_NULL_ERROR_HANDLER, "Handler errori nullo"},
				new object[] {ER_CANNOT_CALL_PARSE, "non \u00e8 possibile richiamare l'analisi se ContentHandler non \u00e8 stato impostato"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Nessun principale per il filtro"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Nessun foglio di lavoro trovato in: {0}, supporto= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Nessun PI xml-stylesheet trovato in: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Non supportato: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "Il valore della propriet\u00e0 {0} deve essere una istanza booleana"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Impossibile richiamare lo script esterno in {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "Risorsa [ {0} ] non trovata.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Propriet\u00e0 Output non riconosciuta: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Creazione dell'istanza ElemLiteralResult non riuscita"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "Il valore di {0} deve contenere un numero analizzabile"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "Il valore di {0} deve essere uguale a yes o no"},
				new object[] {ER_FAILED_CALLING_METHOD, "Chiamata al metodo {0} non riuscita"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Creazione dell'istanza ElemTemplateElement non riuscita"},
				new object[] {ER_CHARS_NOT_ALLOWED, "I caratteri non sono consentiti in questo punto del documento"},
				new object[] {ER_ATTR_NOT_ALLOWED, "L''''attributo \"{0}\" non \u00e8 consentito nell''''elemento {1}."},
				new object[] {ER_BAD_VALUE, "{0} valore errato {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Valore attributo {0} non trovato "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Valore attributo {0} non riconosciuto "},
				new object[] {ER_NULL_URI_NAMESPACE, "\u00c8 stato effettuato un tentativo di generare un prefisso spazio nome con un URI nullo"},
				new object[] {ER_NUMBER_TOO_BIG, "Si sta effettuando un tentativo di formattare un numero superiore all'intero Long pi\u00f9 grande"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Impossibile trovare la classe driver SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "La classe driver SAX1 {0} \u00e8 stata trovata ma non \u00e8 stato possibile caricarla"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "La classe driver SAX1 {0} \u00e8 stata caricata ma non \u00e8 stato possibile istanziarla"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "La classe driver SAX1 {0} non implementa org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Propriet\u00e0 di sistema org.xml.sax.parser non specificata"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "L'argomento Parser non pu\u00f2 essere nullo"},
				new object[] {ER_FEATURE, "Funzione: {0}"},
				new object[] {ER_PROPERTY, "Propriet\u00e0: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Resolver entit\u00e0 nullo"},
				new object[] {ER_NULL_DTD_HANDLER, "Handler DTD nullo"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Non \u00e8 stato specificato alcun nome driver."},
				new object[] {ER_NO_URL_SPECIFIED, "Non \u00e8 stato specificato alcun URL."},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "La dimensione del pool \u00e8 inferiore a 1."},
				new object[] {ER_INVALID_DRIVER_NAME, "Specificato nome driver non valido."},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Errore di programmazione. Espressione senza ElemTemplateElement principale"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Asserzione del programmatore in RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0}non \u00e8 consentito in questa posizione in stylesheet"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Testo Non-whitespace non consentito in questa posizione in stylesheet"},
				new object[] {INVALID_TCHAR, "Valore non valido: {1} utilizzato per l''''attributo CHAR: {0}.  Un attributo di tipo CHAR deve essere di 1 solo carattere."},
				new object[] {INVALID_QNAME, "Valore non valido: {1} utilizzato per l''''attributo QNAME: {0}"},
				new object[] {INVALID_ENUM, "Valore non valido: {1} utilizzato per l''''attributo ENUM: {0}.  I valori validi sono: {2}."},
				new object[] {INVALID_NMTOKEN, "Valore non valido: {1} utilizzato per l''''attributo NMTOKEN: {0} "},
				new object[] {INVALID_NCNAME, "Valore non valido: {1} utilizzato per l''''attributo NCNAME: {0} "},
				new object[] {INVALID_BOOLEAN, "Valore non valido: {1} utilizzato per l''''attributo boolean: {0} "},
				new object[] {INVALID_NUMBER, "Valore non valido: {1} utilizzato per l''''attributo number: {0} "},
				new object[] {ER_ARG_LITERAL, "L''''argomento di {0} nel modello di corrispondenza deve essere letterale."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Dichiarazione di variabile globale duplicata."},
				new object[] {ER_DUPLICATE_VAR, "Dichiarazione di variabile duplicata."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template deve avere un attributo name oppure match (o entrambi)"},
				new object[] {ER_INVALID_PREFIX, "Prefisso in exclude-result-prefixes non valido: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "attribute-set denominato {0} non esiste"},
				new object[] {ER_FUNCTION_NOT_FOUND, "La funzione {0} indicata non esiste"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "L''''elemento {0} non deve avere sia un attributo content o selection."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "Il valore del parametro {0} deve essere un oggetto Java valido"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "L'attributo result-prefix si un elemento xsl:namespace-alias ha il valore '#default', ma non c'\u00e8 dichiarazione dello spazio nome predefinito nell'ambito per l'elemento"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "L''attributo result-prefix di un elemento xsl:namespace-alias ha il valore ''{0}'', ma non c''\u00e8 dichiarazione dello spazio per il prefisso ''{0}'' nell''ambito per l''elemento."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "Il nome della funzione non pu\u00f2 essere nullo in TransformerFactory.setFeature(Nome stringa, valore booleano)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "Il nome della funzione non pu\u00f2 essere nullo in TransformerFactory.getFeature(Nome stringa)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "Impossibile impostare la funzione ''{0}'' su questo TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "L''''utilizzo di un elemento di estensione ''{0}'' non \u00e8 consentito quando la funzione di elaborazione sicura \u00e8 impostata su true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Impossibile ottenere il prefisso per un uri dello spazio nome nullo."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Impossibile ottenere l'uri dello spazio nome per il prefisso null."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "Il nome della funzione non pu\u00f2 essere null."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "Arity non pu\u00f2 essere negativo."},
				new object[] {WG_FOUND_CURLYBRACE, "Rilevato '}' senza una maschera attributo aperta."},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Attenzione: l''attributo count non corrisponde ad un predecessore in xsl:number! Destinazione = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Sintassi obsoleta: Il nome dell'attributo 'expr' \u00e8 stato modificato in 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan non gestisce ancora il nome locale nella funzione formato-numero."},
				new object[] {WG_LOCALE_NOT_FOUND, "Attenzione: Impossibile trovare la locale per xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Impossibile ricavare l''''URL da: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Impossibile caricare il documento richiesto: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Impossibile trovare Collator per <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Sintassi obsoleta: l''istruzione functions deve utilizzare un url di {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "codifica non supportata: {0}, viene utilizzato UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "codifica non supportata: {0}, viene utilizzato Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Sono stati rilevati conflitti di specificit\u00e0: {0} Verr\u00e0 utilizzato l''ultimo trovato nel foglio di lavoro."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Analisi e preparazione {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Maschera attributo, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Conflitto di corrispondenza tra xsl:strip-space e xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan non pu\u00f2 ancora gestire l''''attributo {0}."},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Nessuna dichiarazione trovata per il formato decimale: {0}"},
				new object[] {WG_OLD_XSLT_NS, "XSLT Namespace mancante o non corretto. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "\u00c8 consentita una sola dichiarazione xsl:decimal-format predefinita."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "I nomi xsl:decimal-format devono essere univoci. Il nome \"{0}\" \u00e8 stato duplicato."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} ha un attributo non valido: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Impossibile risolvere il prefisso dello spazio nome: {0}. Il nodo verr\u00e0 ignorato."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet richiede un attributo 'version'."},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Nome attributo non valido: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Valore non valido utilizzato per l''''attributo {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "Il nodeset che risulta dal secondo argomento della funzione documento \u00e8 vuoto. Restituisce un nodeset vuoto."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Il valore dell'attributo 'name' del nome xsl:processing-instruction non deve essere 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Il valore dell''attributo ''name'' di xsl:processing-instruction deve essere un NCName valido: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Impossibile aggiungere l''''attributo {0} dopo i nodi secondari o prima che sia prodotto un elemento.  L''''attributo verr\u00e0 ignorato."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "\u00c8 stato effettuato un tentativo di modificare un oggetto in un contesto in cui le modifiche non sono supportate."},
				new object[] {"ui_language", "it"},
				new object[] {"help_language", "it"},
				new object[] {"language", "it"},
				new object[] {"BAD_CODE", "Il parametro per createMessage fuori limite"},
				new object[] {"FORMAT_FAILED", "Rilevata eccezione durante la chiamata messageFormat"},
				new object[] {"version", ">>>>>>> Versione Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "s\u00ec"},
				new object[] {"line", "Riga #"},
				new object[] {"column", "Colonna #"},
				new object[] {"xsldone", "XSLProcessor: eseguito"},
				new object[] {"xslProc_option", "Opzioni classe Process riga comandi Xalan-J:"},
				new object[] {"xslProc_option", "Opzioni classe Process riga comandi Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "Opzione {0} non supportata in modalit\u00e0."},
				new object[] {"xslProc_invalid_xalan_option", "L''''opzione {0} pu\u00f2 essere utilizzata solo con -XSLTC."},
				new object[] {"xslProc_no_input", "Errore: Nessun foglio di lavoro o xml di immissione specificato. Eseguire questo comando senza opzioni per istruzioni sull'utilizzo."},
				new object[] {"xslProc_common_options", "-Opzioni comuni-"},
				new object[] {"xslProc_xalan_options", "-Opzioni per Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Opzioni per XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(premere <invio> per continuare)"},
				new object[] {"optionXSLTC", "   [-XSLTC (utilizza XSLTC per la trasformazioni)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER nome classe completo del collegamento parser]"},
				new object[] {"optionE", "   [-E (non espandere i riferimenti entit\u00e0)]"},
				new object[] {"optionV", "   [-E (non espandere i riferimenti entit\u00e0)]"},
				new object[] {"optionQC", "   [-QC (Silenziamento avvertenze conflitti modelli)]"},
				new object[] {"optionQ", "   [-Q  (Modo silenzioso)]"},
				new object[] {"optionLF", "   [-LF (Utilizza il caricamento riga solo sull'output {valore predefinito: CR/LF})]"},
				new object[] {"optionCR", "   [-CR (Utilizza il ritorno a capo solo sull'output {valore predefinito: CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (specifica quali caratteri saltare {valore predefinito: <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Controlla il numero dei rientri {valore predefinito: 0})]"},
				new object[] {"optionTT", "   [-TT (Traccia le maschere quando vengono richiamate.)]"},
				new object[] {"optionTG", "   [-TG (Traccia ogni evento di generazione.)]"},
				new object[] {"optionTS", "   [-TS (Traccia ogni evento di selezione.)]"},
				new object[] {"optionTTC", "   [-TTC (Traccia il secondario della maschera quando viene elaborato.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (classe TraceListener per le estensioni di traccia.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Imposta se eseguire la convalida.  Il valore predefinito per la convalida \u00e8 disattivato.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {nome file facoltativo} (Eseguire stackdump in caso di errori.)]"},
				new object[] {"optionXML", "   [-XML (Utilizza la formattazione XML e aggiunge intestazione XML.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Utilizza la formattazione Testo semplice.)]"},
				new object[] {"optionHTML", "   [-HTML (Utilizza la formattazione HTML.)]"},
				new object[] {"optionPARAM", "   [-PARAM nome espressione (imposta un parametro del foglio di lavoro)]"},
				new object[] {"noParsermsg1", "Elaborazione XSL non riuscita."},
				new object[] {"noParsermsg2", "** Impossibile trovare il parser **"},
				new object[] {"noParsermsg3", "Controllare il classpath."},
				new object[] {"noParsermsg4", "Se non si possiede IBM XML Parser per Java, \u00e8 possibile scaricarlo da"},
				new object[] {"noParsermsg5", "IBM AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER nome classe completo (URIResolver da utilizzare per risolvere gli URI)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER nome classe completo (EntityResolver da utilizzare per risolvere le entit\u00e0)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER nome classe completo (ContentHandler da utilizzare per serializzare l'output)]"},
				new object[] {"optionLINENUMBERS", "   [-L utilizza i numeri riga per il documento di origine]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (imposta la funzione di elaborazione sicura su true.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType (utilizza l'attributo media per individuare il foglio di lavoro associato ad un documento.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName (Utilizza in modo esplicito s2s=SAX oppure d2d=DOM per eseguire la trasformazione.)] "},
				new object[] {"optionDIAG", "   [-DIAG (Visualizza il tempo impiegato in millisecondi per la trasformazione.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (richiede la costruzione DTM incrementale impostando http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (non richiede alcuna elaborazione di ottimizzazione del foglio di lavoro impostando http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", "   [-RL recursionlimit (limite numerico asserzioni nella profondit\u00e0 ricorsiva del foglio di lavoro.)]"},
				new object[] {"optionXO", "   [-XO [transletName] (assegna il nome al translet generato)]"},
				new object[] {"optionXD", "   [-XD destinationDirectory (specifica una directory di destinazione per il translet)]"},
				new object[] {"optionXJ", "   [-XJ jarfile (raggruppa la classi translet in un file jar di nome <jarfile>)]"},
				new object[] {"optionXP", "   [-XP package (specifica un prefisso di nome pacchetto per tutte le classi translet generate)]"},
				new object[] {"optionXN", "   [-XN (abilita l'allineamento della maschera)]"},
				new object[] {"optionXX", "   [-XX (attiva ulteriori emissioni di messaggi di debug)]"},
				new object[] {"optionXT", "   [-XT (utilizza il translet per la trasformazione, se possibile)]"},
				new object[] {"diagTiming", " --------- La trasformazione di {0} utilizzando {1} ha impiegato {2} ms"},
				new object[] {"recursionTooDeep", "Nidificazione della maschera troppo elevata. nesting = {0}, maschera {1} {2}"},
				new object[] {"nameIs", "il nome \u00e8"},
				new object[] {"matchPatternIs", "il modello di corrispondenza \u00e8"}
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
	  public const string ERROR_HEADER = "Errore: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Avvertenza: ";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public const string QUERY_HEADER = "MODELLO ";


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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("it", "IT"));
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