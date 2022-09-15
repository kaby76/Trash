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
 * $Id: XSLTErrorResources_pt_BR.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_pt_BR : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Erro: Imposs\u00edvel ter '{' na express\u00e3o"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} possui um atributo inv\u00e1lido: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode \u00e9 nulo em xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "Imposs\u00edvel incluir {0} em {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode \u00e9 nulo em handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} deve ter um atributo name."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "N\u00e3o foi poss\u00edvel localizar o template: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "N\u00e3o foi poss\u00edvel resolver nome AVT em xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} requer o atributo: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} deve ter um atributo ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Valor inv\u00e1lido no atributo de n\u00edvel: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "O nome de processing-instruction n\u00e3o pode ser 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "O nome de processing-instruction deve ser um NCName v\u00e1lido: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} deve ter um atributo de correspond\u00eancia se tiver um modo."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} requer um nome ou um atributo de correspond\u00eancia."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Imposs\u00edvel resolver prefixo do espa\u00e7o de nomes: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space possui um valor inv\u00e1lido: {0}"},
				new object[] {ER_NO_OWNERDOC, "O n\u00f3 filho n\u00e3o possui um documento do propriet\u00e1rio!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Erro de ElemTemplateElement: {0}"},
				new object[] {ER_NULL_CHILD, "Tentando incluir um filho nulo!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} requer um atributo select."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when deve ter um atributo 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param deve ter um atributo 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "context n\u00e3o possui um documento do propriet\u00e1rio!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "N\u00e3o foi poss\u00edvel criar XML TransformerFactory Liaison: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: O processo n\u00e3o foi bem-sucedido."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: n\u00e3o foi bem-sucedido."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Codifica\u00e7\u00e3o n\u00e3o suportada: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "N\u00e3o foi poss\u00edvel criar TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key requer um atributo 'name'!"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key requer um atributo 'match'!"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key requer um atributo 'use'!"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} requer um atributo ''elements''!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) O atributo ''prefix'' de {0} est\u00e1 ausente"},
				new object[] {ER_BAD_STYLESHEET_URL, "A URL da p\u00e1gina de estilo \u00e9 inv\u00e1lida: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "O arquivo da p\u00e1gina de estilo n\u00e3o foi encontrado: {0}"},
				new object[] {ER_IOEXCEPTION, "Ocorreu uma Exce\u00e7\u00e3o de E/S (entrada/sa\u00edda) no arquivo de p\u00e1gina de estilo: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) N\u00e3o foi poss\u00edvel encontrar o atributo href para {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} est\u00e1 incluindo a si mesmo, direta ou indiretamente!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "Erro de StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) O atributo ''lang'' de {0} est\u00e1 ausente"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) Elemento {0} aplicado incorretamente?? O elemento de cont\u00eainer ''component'' est\u00e1 ausente "},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "A sa\u00edda pode ser apenas para um Element, DocumentFragment, Document ou PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "Erro de StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Erro de UnImplNode: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Erro! N\u00e3o encontrada a express\u00e3o xpath select (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "N\u00e3o \u00e9 poss\u00edvel serializar um XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "A entrada de folha de estilo n\u00e3o foi especificada!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Falha ao processar folha de estilo!"},
				new object[] {ER_COULDNT_PARSE_DOC, "N\u00e3o foi poss\u00edvel analisar o documento {0}!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "N\u00e3o foi poss\u00edvel localizar o fragmento: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "O n\u00f3 apontado por um identificador de fragmento n\u00e3o era um elemento: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each deve ter um atributo match ou name"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "templates deve ter um atributo match ou name"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Nenhum clone de fragmento de documento!"},
				new object[] {ER_CANT_CREATE_ITEM, "Imposs\u00edvel criar item na \u00e1rvore de resultados: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space no XML de origem possui um valor inv\u00e1lido: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "N\u00e3o existe nenhuma declara\u00e7\u00e3o xsl:key para {0}!"},
				new object[] {ER_CANT_CREATE_URL, "Erro! Imposs\u00edvel criar url para: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions n\u00e3o \u00e9 suportado"},
				new object[] {ER_PROCESSOR_ERROR, "Erro de XSLT TransformerFactory"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} n\u00e3o permitido dentro de uma folha de estilo!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns n\u00e3o \u00e9 mais suportado!  Utilize ent\u00e3o xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space n\u00e3o \u00e9 mais suportado!  Utilize ent\u00e3o xsl:strip-space ou xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result n\u00e3o \u00e9 mais suportado!  Utilize ent\u00e3o xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} possui um atributo inv\u00e1lido: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Elemento XSL desconhecido: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort somente pode ser utilizado com xsl:apply-templates ou xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) xsl:when aplicado incorretamente!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when n\u00e3o est\u00e1 ligado a xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) xsl:otherwise aplicado incorretamente!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise n\u00e3o est\u00e1 ligado a xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} n\u00e3o \u00e9 permitido dentro de um template!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) o espa\u00e7o de nomes de extens\u00e3o {0} possui prefixo {1} desconhecido"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Importa\u00e7\u00f5es s\u00f3 podem ocorrer como os primeiros elementos na folha de estilo!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} est\u00e1 importando a si mesmo, direta ou indiretamente!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space tem um valor inv\u00e1lido: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet n\u00e3o obteve \u00eaxito!"},
				new object[] {ER_SAX_EXCEPTION, "Exce\u00e7\u00e3o de SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Fun\u00e7\u00e3o n\u00e3o suportada!"},
				new object[] {ER_XSLT_ERROR, "Erro de XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "O sinal monet\u00e1rio n\u00e3o \u00e9 permitido na cadeia de padr\u00f5es de formato"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Fun\u00e7\u00e3o Document n\u00e3o suportada no DOM da Folha de Estilo!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Imposs\u00edvel resolver prefixo de solucionador sem Prefixo!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Redirecionar extens\u00e3o: N\u00e3o foi poss\u00edvel obter o nome do arquivo - o atributo file ou select deve retornar uma cadeia v\u00e1lida."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Imposs\u00edvel construir FormatterListener em Redirecionar extens\u00e3o!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "O prefixo em exclude-result-prefixes n\u00e3o \u00e9 v\u00e1lido: {0}"},
				new object[] {ER_MISSING_NS_URI, "URI do espa\u00e7o de nomes ausente para o prefixo especificado"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Argumento ausente para a op\u00e7\u00e3o: {0}"},
				new object[] {ER_INVALID_OPTION, "Op\u00e7\u00e3o inv\u00e1lida: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Cadeia com problemas de formata\u00e7\u00e3o: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet requer um atributo 'version'!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Atributo: {0} possui um valor inv\u00e1lido: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose requer um xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports n\u00e3o permitido em um xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Imposs\u00edvel utilizar um DTMLiaison para um n\u00f3 DOM de sa\u00edda... transmita um org.apache.xpath.DOM2Helper no lugar!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Imposs\u00edvel utilizar um DTMLiaison para um n\u00f3 DOM de entrada... transmita um org.apache.xpath.DOM2Helper no lugar!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Falha na chamada do elemento da extens\u00e3o: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "O prefixo deve ser resolvido para um espa\u00e7o de nomes: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Detectado substituto UTF-16 inv\u00e1lido: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} utilizou a si mesmo, o que causar\u00e1 um loop infinito."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Imposs\u00edvel misturar entrada n\u00e3o Xerces-DOM com sa\u00edda Xerces-DOM!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "Em ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Encontrado mais de um template chamado: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Chamada de fun\u00e7\u00e3o inv\u00e1lida: chamadas key() recursivas n\u00e3o s\u00e3o permitidas"},
				new object[] {ER_REFERENCING_ITSELF, "A vari\u00e1vel {0} est\u00e1 fazendo refer\u00eancia a si mesmo, direta ou indiretamente!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "O n\u00f3 de entrada n\u00e3o pode ser nulo para um DOMSource de newTemplates!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Arquivo de classe n\u00e3o encontrado para a op\u00e7\u00e3o {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "Elemento requerido n\u00e3o encontrado: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream n\u00e3o pode ser nulo"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI n\u00e3o pode ser nulo"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "File n\u00e3o pode ser nulo"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource n\u00e3o pode ser nulo"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "N\u00e3o foi poss\u00edvel inicializar o BSF Manager"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "N\u00e3o foi poss\u00edvel compilar a extens\u00e3o"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "N\u00e3o foi poss\u00edvel criar extens\u00e3o: {0} devido a: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "A chamada do m\u00e9todo da inst\u00e2ncia para o m\u00e9todo {0} requer uma inst\u00e2ncia Object como primeiro argumento"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Especificado nome de elemento inv\u00e1lido {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "O m\u00e9todo do nome de elemento deve ser est\u00e1tico {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "A fun\u00e7\u00e3o de extens\u00e3o {0} : {1} \u00e9 desconhecida"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Mais de uma correspond\u00eancia principal para o construtor de {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Mais de uma correspond\u00eancia principal para o m\u00e9todo {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Mais de uma correspond\u00eancia principal para o m\u00e9todo do elemento {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Contexto inv\u00e1lido transmitido para avaliar {0}"},
				new object[] {ER_POOL_EXISTS, "O conjunto j\u00e1 existe"},
				new object[] {ER_NO_DRIVER_NAME, "Nenhum Nome de driver foi especificado"},
				new object[] {ER_NO_URL, "Nenhuma URL especificada"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "O tamanho do conjunto \u00e9 menor que um!"},
				new object[] {ER_INVALID_DRIVER, "Especificado nome de driver inv\u00e1lido!"},
				new object[] {ER_NO_STYLESHEETROOT, "N\u00e3o encontrada a raiz da folha de estilo!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Valor inv\u00e1lido para xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode falhou"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "O recurso [ {0} ] n\u00e3o p\u00f4de carregar: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Tamanho do buffer <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Erro desconhecido ao chamar a extens\u00e3o"},
				new object[] {ER_NO_NAMESPACE_DECL, "O prefixo {0} n\u00e3o possui uma declara\u00e7\u00e3o do espa\u00e7o de nomes correspondente"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Conte\u00fado de elemento n\u00e3o permitido para lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Finaliza\u00e7\u00e3o direcionada por folha de estilo"},
				new object[] {ER_ONE_OR_TWO, "1 ou 2"},
				new object[] {ER_TWO_OR_THREE, "2 ou 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "N\u00e3o foi poss\u00edvel carregar {0} (verificar CLASSPATH); utilizando apenas os padr\u00f5es"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Imposs\u00edvel inicializar templates padr\u00e3o"},
				new object[] {ER_RESULT_NULL, "O resultado n\u00e3o deve ser nulo"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "O resultado n\u00e3o p\u00f4de ser definido"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Nenhuma sa\u00edda especificada"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "N\u00e3o \u00e9 poss\u00edvel transformar em um Resultado do tipo {0} "},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "N\u00e3o \u00e9 poss\u00edvel transformar em uma Origem do tipo {0} "},
				new object[] {ER_NULL_CONTENT_HANDLER, "Rotina de tratamento de conte\u00fado nula"},
				new object[] {ER_NULL_ERROR_HANDLER, "Rotina de tratamento de erros nula"},
				new object[] {ER_CANNOT_CALL_PARSE, "parse n\u00e3o pode ser chamado se ContentHandler n\u00e3o tiver sido definido"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Nenhum pai para o filtro"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Nenhuma p\u00e1gina de estilo foi encontrada em: {0}, m\u00eddia= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Nenhum PI xml-stylesheet encontrado em: {0}"},
				new object[] {ER_NOT_SUPPORTED, "N\u00e3o suportado: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "O valor para a propriedade {0} deve ser uma inst\u00e2ncia Booleana"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "N\u00e3o foi poss\u00edvel obter script externo em {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "O recurso [ {0} ] n\u00e3o p\u00f4de ser encontrado.\n{1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Propriedade de sa\u00edda n\u00e3o reconhecida: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Falha ao criar a inst\u00e2ncia ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "O valor para {0} deve conter um n\u00famero analis\u00e1vel"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "O valor de {0} deve ser igual a yes ou no"},
				new object[] {ER_FAILED_CALLING_METHOD, "Falha ao chamar o m\u00e9todo {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Falha ao criar a inst\u00e2ncia ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "N\u00e3o s\u00e3o permitidos caracteres neste ponto do documento"},
				new object[] {ER_ATTR_NOT_ALLOWED, "O atributo \"{0}\" n\u00e3o \u00e9 permitido no elemento {1}!"},
				new object[] {ER_BAD_VALUE, "{0} possui valor inv\u00e1lido {1}"},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Valor do atributo {0} n\u00e3o encontrado"},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Valor do atributo {0} n\u00e3o reconhecido"},
				new object[] {ER_NULL_URI_NAMESPACE, "Tentando gerar um prefixo do espa\u00e7o de nomes com URI nulo"},
				new object[] {ER_NUMBER_TOO_BIG, "Tentando formatar um n\u00famero superior ao maior inteiro Longo"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Imposs\u00edvel encontrar a classe de driver SAX1 {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "Classe de driver SAX1 {0} encontrada, mas n\u00e3o pode ser carregada"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "Classe de driver SAX1 {0} carregada, mas n\u00e3o pode ser instanciada"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "A classe de driver SAX1 {0} n\u00e3o implementa org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Propriedade de sistema org.xml.sax.parser n\u00e3o especificada"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "O argumento Parser n\u00e3o deve ser nulo"},
				new object[] {ER_FEATURE, "Recurso: {0}"},
				new object[] {ER_PROPERTY, "Propriedade: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Solucionador de entidade nulo"},
				new object[] {ER_NULL_DTD_HANDLER, "Rotina de tratamento DTD nula"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Nenhum Nome de Driver Especificado!"},
				new object[] {ER_NO_URL_SPECIFIED, "Nenhuma URL Especificada!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "O tamanho do conjunto \u00e9 menor que 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Especificado Nome de Driver Inv\u00e1lido!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Erro do programador! A express\u00e3o n\u00e3o possui o pai ElemTemplateElement!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Declara\u00e7\u00e3o do programador em RedundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} n\u00e3o \u00e9 permitido nesta posi\u00e7\u00e3o na p\u00e1gina de estilo!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "O texto sem espa\u00e7o em branco n\u00e3o \u00e9 permitido nesta posi\u00e7\u00e3o na p\u00e1gina de estilo!"},
				new object[] {INVALID_TCHAR, "Valor inv\u00e1lido: {1} utilizado para o caractere CHAR: {0}. Um atributo de tipo CHAR deve ter apenas 1 caractere!"},
				new object[] {INVALID_QNAME, "Valor inv\u00e1lido: {1} utilizado para o atributo QNAME: {0}"},
				new object[] {INVALID_ENUM, "Valor inv\u00e1lido: {1} utilizado para o atributo ENUM: {0}. Os valores v\u00e1lidos s\u00e3o: {2}."},
				new object[] {INVALID_NMTOKEN, "Valor inv\u00e1lido: {1} utilizado para o atributo NMTOKEN: {0}"},
				new object[] {INVALID_NCNAME, "Valor inv\u00e1lido: {1} utilizado para o atributo NCNAME: {0}"},
				new object[] {INVALID_BOOLEAN, "Valor inv\u00e1lido: {1} utilizado para o atributo boolean: {0}"},
				new object[] {INVALID_NUMBER, "Valor inv\u00e1lido: {1} utilizado para o atributo number: {0}"},
				new object[] {ER_ARG_LITERAL, "Argumento para {0} no padr\u00e3o de correspond\u00eancia deve ser um literal."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Declara\u00e7\u00e3o de vari\u00e1vel global duplicada."},
				new object[] {ER_DUPLICATE_VAR, "Declara\u00e7\u00e3o de vari\u00e1vel duplicada."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template deve ter um atributo name ou match (ou ambos)"},
				new object[] {ER_INVALID_PREFIX, "O prefixo em exclude-result-prefixes n\u00e3o \u00e9 v\u00e1lido: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "O attribute-set {0} n\u00e3o existe"},
				new object[] {ER_FUNCTION_NOT_FOUND, "A fun\u00e7\u00e3o denominada {0} n\u00e3o existe"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "O elemento {0} n\u00e3o deve ter um conte\u00fado e um atributo de sele\u00e7\u00e3o ao mesmo tempo."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "O valor do par\u00e2metro {0} deve ser um Objeto Java v\u00e1lido"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "O atributo result-prefix de um elemento xsl:namespace-alias tem o valor '#default', mas n\u00e3o h\u00e1 nenhuma declara\u00e7\u00e3o do espa\u00e7o de nomes padr\u00e3o no escopo para o elemento"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "O atributo result-prefix de um elemento xsl:namespace-alias tem o valor ''{0}'', mas n\u00e3o h\u00e1 nenhuma declara\u00e7\u00e3o do espa\u00e7o de nomes para o prefixo ''{0}'' no escopo para o elemento."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "O nome do recurso n\u00e3o pode ser nulo em TransformerFactory.setFeature(String name, boolean value)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "O nome do recurso n\u00e3o pode ser nulo em TransformerFactory.getFeature(String name)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "N\u00e3o \u00e9 poss\u00edvel definir o recurso ''{0}'' neste TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "O uso do elemento de extens\u00e3o ''{0}'' n\u00e3o \u00e9 permitido quando o recurso de processamento seguro \u00e9 definido como true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "N\u00e3o \u00e9 poss\u00edvel obter o prefixo para um uri de espa\u00e7o de nomes nulo."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "N\u00e3o \u00e9 poss\u00edvel obter o uri do espa\u00e7o de nomes para um prefixo nulo."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "O nome da fun\u00e7\u00e3o n\u00e3o pode ser nulo."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "O arity n\u00e3o pode ser negativo."},
				new object[] {WG_FOUND_CURLYBRACE, "Encontrado '}', mas nenhum template de atributo aberto!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Aviso: o atributo count n\u00e3o corresponde a um predecessor em xsl:number! Destino = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Sintaxe antiga: O nome do atributo 'expr' foi alterado para 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan ainda n\u00e3o trata do nome de locale na fun\u00e7\u00e3o format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Aviso: N\u00e3o foi poss\u00edvel localizar o locale para xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Imposs\u00edvel criar URL a partir de: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Imposs\u00edvel carregar doc solicitado: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Imposs\u00edvel localizar Intercalador para <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Sintaxe antiga: a instru\u00e7\u00e3o functions deve utilizar uma url de {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "codifica\u00e7\u00e3o n\u00e3o suportada: {0}, utilizando UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "codifica\u00e7\u00e3o n\u00e3o suportada: {0}, utilizando Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Encontrados conflitos de especifica\u00e7\u00e3o: O \u00faltimo {0} encontrado na p\u00e1gina de estilo ser\u00e1 utilizado."},
				new object[] {WG_PARSING_AND_PREPARING, "========= An\u00e1lise e prepara\u00e7\u00e3o {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Template de Atr, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Conflito de correspond\u00eancia entre xsl:strip-space e xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan ainda n\u00e3o trata do atributo {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Nenhuma declara\u00e7\u00e3o encontrada para formato decimal: {0}"},
				new object[] {WG_OLD_XSLT_NS, "Espa\u00e7o de nomes XSLT ausente ou incorreto."},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Apenas uma declara\u00e7\u00e3o padr\u00e3o xsl:decimal-format \u00e9 permitida."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "Os nomes de xsl:decimal-format devem ser exclusivos. O nome \"{0}\" foi duplicado."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} possui um atributo inv\u00e1lido: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "N\u00e3o foi poss\u00edvel resolver prefixo do espa\u00e7o de nomes: {0}. O n\u00f3 ser\u00e1 ignorado."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet requer um atributo 'version'!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Nome de atributo inv\u00e1lido: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Valor inv\u00e1lido utilizado para o atributo {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "O nodeset resultante do segundo argumento da fun\u00e7\u00e3o document est\u00e1 vazio. Retornar um node-set vazio."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "O valor do atributo 'name' do nome xsl:processing-instruction n\u00e3o deve ser 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "O valor do atributo ''name'' de xsl:processing-instruction deve ser um NCName v\u00e1lido: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Imposs\u00edvel incluir atributo {0} depois de n\u00f3s filhos ou antes da gera\u00e7\u00e3o de um elemento. O atributo ser\u00e1 ignorado."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Foi feita uma tentativa de modificar um objeto no qual n\u00e3o s\u00e3o permitidas modifica\u00e7\u00f5es. "},
				new object[] {"ui_language", "pt"},
				new object[] {"help_language", "pt"},
				new object[] {"language", "pt"},
				new object[] {"BAD_CODE", "O par\u00e2metro para createMessage estava fora dos limites"},
				new object[] {"FORMAT_FAILED", "Exce\u00e7\u00e3o emitida durante chamada messageFormat"},
				new object[] {"version", ">>>>>>> Vers\u00e3o Xalan"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "sim"},
				new object[] {"line", "Linha n\u00b0"},
				new object[] {"column","Coluna n\u00b0"},
				new object[] {"xsldone", "XSLProcessor: conclu\u00eddo"},
				new object[] {"xslProc_option", "Op\u00e7\u00f5es da classe Process da linha de comando de Xalan-J:"},
				new object[] {"xslProc_option", "Op\u00e7\u00f5es da classe Process da linha de comandos de Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "A op\u00e7\u00e3o {0} n\u00e3o \u00e9 suportada no modo XSLTC."},
				new object[] {"xslProc_invalid_xalan_option", "A op\u00e7\u00e3o {0} somente pode ser utilizada com -XSLTC."},
				new object[] {"xslProc_no_input", "Erro: Nenhuma p\u00e1gina de estilo ou xml de entrada foi especificado. Execute este comando sem nenhuma op\u00e7\u00e3o para instru\u00e7\u00f5es de uso."},
				new object[] {"xslProc_common_options", "-Op\u00e7\u00f5es Comuns-"},
				new object[] {"xslProc_xalan_options", "-Op\u00e7\u00f5es para Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Op\u00e7\u00f5es para XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(pressione <return> para continuar)"},
				new object[] {"optionXSLTC", "   [-XSLTC (utilizar XSLTC para transforma\u00e7\u00e3o)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER nome completo da classe do analisador liaison]"},
				new object[] {"optionE", "   [-E (N\u00e3o expandir refs de entidade)]"},
				new object[] {"optionV", "   [-E (N\u00e3o expandir refs de entidade)]"},
				new object[] {"optionQC", "   [-QC (Avisos de Conflitos de Padr\u00e3o Silencioso)]"},
				new object[] {"optionQ", "   [-Q  (Modo Silencioso)]"},
				new object[] {"optionLF", "   [-LF (Utilizar avan\u00e7os de linha apenas na sa\u00edda {padr\u00e3o \u00e9 CR/LF})]"},
				new object[] {"optionCR", "   [-CR (Utilizar retornos de carro apenas na sa\u00edda {padr\u00e3o \u00e9 CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Quais caracteres de escape {padr\u00e3o \u00e9 <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Controlar como os espa\u00e7os s\u00e3o recuados {padr\u00e3o \u00e9 0})]"},
				new object[] {"optionTT", "   [-TT (Rastrear os templates enquanto est\u00e3o sendo chamados.)]"},
				new object[] {"optionTG", "   [-TG (Rastrear cada evento de gera\u00e7\u00e3o.)]"},
				new object[] {"optionTS", "   [-TS (Rastrear cada evento de sele\u00e7\u00e3o.)]"},
				new object[] {"optionTTC", "   [-TTC (Rastrear os filhos do modelo enquanto est\u00e3o sendo processados.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (Classe TraceListener para extens\u00f5es de rastreio.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Definir se ocorrer valida\u00e7\u00e3o. A valida\u00e7\u00e3o fica desativada por padr\u00e3o.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {nome de arquivo opcional} (Executar stackdump sob erro.)]"},
				new object[] {"optionXML", "   [-XML (Utilizar formatador XML e incluir cabe\u00e7alho XML.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Utilizar formatador de Texto simples.)]"},
				new object[] {"optionHTML", "   [-HTML (Utilizar formatador HTML.)]"},
				new object[] {"optionPARAM", "   [-PARAM express\u00e3o de nome (Definir um par\u00e2metro stylesheet)]"},
				new object[] {"noParsermsg1", "O Processo XSL n\u00e3o obteve \u00eaxito."},
				new object[] {"noParsermsg2", "** N\u00e3o foi poss\u00edvel encontrar o analisador **"},
				new object[] {"noParsermsg3", "Verifique seu classpath."},
				new object[] {"noParsermsg4", "Se voc\u00ea n\u00e3o tiver o XML Parser para Java da IBM, poder\u00e1 fazer o download dele a partir de"},
				new object[] {"noParsermsg5", "IBM's AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER nome completo da classe (URIResolver a ser utilizado para resolver URIs)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER nome completo da classe (EntityResolver a ser utilizado para resolver entidades)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER nome completo da classe (ContentHandler a ser utilizado para serializar sa\u00edda)]"},
				new object[] {"optionLINENUMBERS", "   [-L utilizar n\u00fameros de linha para documento de origem]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (define o recurso de processamento seguro como true.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType (utilizar atributo de m\u00eddia para encontrar folha de estilo associada a um documento.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName (Utilizar explicitamente s2s=SAX ou d2d=DOM para executar transforma\u00e7\u00e3o.)]"},
				new object[] {"optionDIAG", "   [-DIAG (Imprimir total de milissegundos que a transforma\u00e7\u00e3o gastou.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (pedir constru\u00e7\u00e3o incremental de DTM definindo http://xml.apache.org/xalan/features/incremental verdadeiro.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (n\u00e3o solicitar o processamento de otimiza\u00e7\u00e3o de folha de estilo definindo http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", "   [-RL recursionlimit (declarar limite num\u00e9rico em profundidade de recorr\u00eancia de folha de estilo.)]"},
				new object[] {"optionXO", "   [-XO [transletName] (atribuir nome ao translet gerado)]"},
				new object[] {"optionXD", "   [-XD destinationDirectory (especificar um diret\u00f3rio de destino para translet)]"},
				new object[] {"optionXJ", "   [-XJ jarfile (empacota classes translet em um arquivo jar denominado <arquivo_jar>)]"},
				new object[] {"optionXP", "   [-XP package (especifica um prefixo de nome de pacote para todas as classes translet geradas)]"},
				new object[] {"optionXN", "   [-XN (ativa a seq\u00fc\u00eancia de templates)]"},
				new object[] {"optionXX", "   [-XX (ativa a sa\u00edda de mensagem de depura\u00e7\u00e3o adicional)]"},
				new object[] {"optionXT", "   [-XT (utilizar translet para transforma\u00e7\u00e3o, se poss\u00edvel)]"},
				new object[] {"diagTiming"," --------- Transforma\u00e7\u00e3o de {0} via {1} levou {2} ms"},
				new object[] {"recursionTooDeep","Aninhamento de templates muito extenso. aninhamento = {0}, template {1} {2}"},
				new object[] {"nameIs", "o nome \u00e9"},
				new object[] {"matchPatternIs", "o padr\u00e3o de correspond\u00eancia \u00e9"}
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
	  public const string ERROR_HEADER = "Erro: ";

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
	  public const string QUERY_HEADER = "PADR\u00c3O ";


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
			return (XSLTErrorResources) ResourceBundle.getBundle(className, new Locale("pt", "BR"));
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