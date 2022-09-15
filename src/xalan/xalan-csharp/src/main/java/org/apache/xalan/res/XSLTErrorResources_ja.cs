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
 * $Id: XSLTErrorResources_ja.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_ja : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "\u30a8\u30e9\u30fc: \u5f0f\u5185\u3067\u306f '{' \u3092\u4f7f\u7528\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} \u306b\u6b63\u3057\u304f\u306a\u3044\u5c5e\u6027\u304c\u3042\u308a\u307e\u3059: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "xsl:apply-imports \u5185\u306e sourceNode \u304c\u30cc\u30eb\u3067\u3059\u3002"},
				new object[] {ER_CANNOT_ADD, "{0} \u3092 {1} \u306b\u8ffd\u52a0\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "handleApplyTemplatesInstruction \u5185\u306e sourceNode \u304c\u30cc\u30eb\u3067\u3059\u3002"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} \u306b\u306f name \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_TEMPLATE_NOT_FOUND, "{0} \u3068\u3044\u3046\u540d\u524d\u306e\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "xsl:call-template \u5185\u306e\u540d\u524d AVT \u3092\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_REQUIRES_ATTRIB, "{0} \u306b\u306f\u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} \u306b\u306f ''test'' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "level \u5c5e\u6027\u3067\u5024\u304c\u9593\u9055\u3063\u3066\u3044\u307e\u3059: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "\u51e6\u7406\u547d\u4ee4\u306e\u540d\u524d\u306f 'xml' \u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "\u51e6\u7406\u547d\u4ee4\u306e\u540d\u524d\u306f\u6709\u52b9\u306a NCName \u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} \u306b\u30e2\u30fc\u30c9\u304c\u3042\u308b\u5834\u5408\u306f\u3001match \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} \u306b\u306f name \u307e\u305f\u306f match \u306e\u3044\u305a\u308c\u304b\u306e\u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "\u540d\u524d\u7a7a\u9593\u63a5\u982d\u90e8\u3092\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space \u306b\u306f\u6b63\u3057\u304f\u306a\u3044\u5024\u304c\u3042\u308a\u307e\u3059: {0}"},
				new object[] {ER_NO_OWNERDOC, "\u4e0b\u4f4d\u30ce\u30fc\u30c9\u306b\u6240\u6709\u8005\u6587\u66f8\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "ElemTemplateElement \u30a8\u30e9\u30fc: {0}"},
				new object[] {ER_NULL_CHILD, "\u30cc\u30eb\u306e\u5b50\u3092\u8ffd\u52a0\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} \u306b\u306f select \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when \u306b\u306f 'test' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param \u306b\u306f 'name' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u306b\u6240\u6709\u8005\u6587\u66f8\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "XML TransformerFactory Liaison \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: \u51e6\u7406\u306f\u6210\u529f\u3057\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: \u306f\u6210\u529f\u3057\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "\u30a8\u30f3\u30b3\u30fc\u30c9\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "TraceListener \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key \u306b\u306f 'name' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key \u306b\u306f 'match' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key \u306b\u306f 'use' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} \u306b\u306f ''elements'' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} \u306b\u5c5e\u6027 ''prefix'' \u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_BAD_STYLESHEET_URL, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8 URL \u304c\u9593\u9055\u3063\u3066\u3044\u307e\u3059: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u30fb\u30d5\u30a1\u30a4\u30eb\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f: {0}"},
				new object[] {ER_IOEXCEPTION, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u30fb\u30d5\u30a1\u30a4\u30eb\u306b\u3088\u308b\u5165\u51fa\u529b\u4f8b\u5916\u304c\u8d77\u3053\u308a\u307e\u3057\u305f: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) {0} \u306e href \u5c5e\u6027\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} \u304c\u81ea\u5206\u81ea\u8eab\u3092\u76f4\u63a5\u7684\u307e\u305f\u306f\u9593\u63a5\u7684\u306b\u7d44\u307f\u8fbc\u3082\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "StylesheetHandler.processInclude \u30a8\u30e9\u30fc\u3001{0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} \u306b\u5c5e\u6027 ''lang'' \u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) {0} \u8981\u7d20\u306e\u5834\u6240\u3092\u9593\u9055\u3048\u305f\u53ef\u80fd\u6027\u304c\u3042\u308a\u307e\u3059\u3002\u30b3\u30f3\u30c6\u30ca\u30fc\u8981\u7d20 ''component'' \u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Element\u3001DocumentFragment\u3001Document\u3001\u307e\u305f\u306f PrintWriter \u3078\u306e\u51fa\u529b\u3057\u304b\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_PROCESS_ERROR, "StylesheetRoot.\u51e6\u7406\u30a8\u30e9\u30fc"},
				new object[] {ER_UNIMPLNODE_ERROR, "UnImplNode \u30a8\u30e9\u30fc: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "\u30a8\u30e9\u30fc:  xpath select \u5f0f (-select) \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "XSLProcessor \u3092\u30b7\u30ea\u30a2\u30e9\u30a4\u30ba\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NO_INPUT_STYLESHEET, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u5165\u529b\u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3092\u51e6\u7406\u3059\u308b\u3053\u3068\u306b\u5931\u6557\u3057\u307e\u3057\u305f\u3002"},
				new object[] {ER_COULDNT_PARSE_DOC, "{0} \u6587\u66f8\u3092\u69cb\u6587\u89e3\u6790\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "\u30d5\u30e9\u30b0\u30e1\u30f3\u30c8\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "\u30d5\u30e9\u30b0\u30e1\u30f3\u30c8 ID \u306b\u3088\u308a\u6307\u3055\u308c\u3066\u3044\u308b\u30ce\u30fc\u30c9\u304c\u8981\u7d20\u3067\u3042\u308a\u307e\u305b\u3093\u3067\u3057\u305f: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each \u306b\u306f match \u307e\u305f\u306f name \u306e\u3044\u305a\u308c\u304b\u306e\u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306b\u306f match \u307e\u305f\u306f name \u306e\u3044\u305a\u308c\u304b\u306e\u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "\u6587\u66f8\u30d5\u30e9\u30b0\u30e1\u30f3\u30c8\u306e\u8907\u88fd\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_CREATE_ITEM, "\u9805\u76ee\u3092\u7d50\u679c\u30c4\u30ea\u30fc\u306b\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "\u30bd\u30fc\u30b9 XML \u5185\u306e xml:space \u306b\u306f\u6b63\u3057\u304f\u306a\u3044\u5024\u304c\u3042\u308a\u307e\u3059: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "{0} \u306e xsl:key \u5ba3\u8a00\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_CREATE_URL, "\u30a8\u30e9\u30fc:  {0} \u306e URL \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions \u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_PROCESSOR_ERROR, "XSLT TransformerFactory \u30a8\u30e9\u30fc"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} \u306f\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u306e\u5185\u90e8\u3067\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns \u306f\u3082\u3046\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002  \u4ee3\u308a\u306b xsl:output \u3092\u4f7f\u7528\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space \u306f\u3082\u3046\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002  \u4ee3\u308a\u306b xsl:strip-space \u307e\u305f\u306f xsl:preserve-space \u3092\u4f7f\u7528\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result \u306f\u3082\u3046\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002  \u4ee3\u308a\u306b xsl:output \u3092\u4f7f\u7528\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} \u306b\u306f\u6b63\u3057\u304f\u306a\u3044\u5c5e\u6027\u304c\u3042\u308a\u307e\u3059: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "\u4e0d\u660e\u306e XSL \u8981\u7d20: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort \u306f xsl:apply-templates \u307e\u305f\u306f xsl:for-each \u3068\u3057\u304b\u4f7f\u7528\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) xsl:when \u306e\u5834\u6240\u3092\u8aa4\u3063\u3066\u3044\u307e\u3057\u305f\u3002"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when \u304c xsl:choose \u306b\u3088\u308a\u89aa\u306b\u306a\u3063\u3066\u3044\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) xsl:otherwise \u306e\u5834\u6240\u3092\u8aa4\u3063\u3066\u3044\u307e\u3057\u305f\u3002"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise \u304c xsl:choose \u306b\u3088\u308a\u89aa\u306b\u306a\u3063\u3066\u3044\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} \u306f\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306e\u5185\u90e8\u3067\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0} \u62e1\u5f35\u540d\u524d\u7a7a\u9593\u63a5\u982d\u90e8 {1} \u304c\u4e0d\u660e\u3067\u3059\u3002"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) \u30a4\u30f3\u30dd\u30fc\u30c8\u306f\u3001\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u5185\u306e\u5148\u982d\u8981\u7d20\u3068\u3057\u3066\u306e\u307f\u5165\u308c\u308b\u3053\u3068\u304c\u3067\u304d\u307e\u3059\u3002"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} \u304c\u81ea\u5206\u81ea\u8eab\u3092\u76f4\u63a5\u7684\u307e\u305f\u306f\u9593\u63a5\u7684\u306b\u30a4\u30f3\u30dd\u30fc\u30c8\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space \u306b\u6b63\u3057\u304f\u306a\u3044\u5024\u304c\u3042\u308a\u307e\u3059: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet \u306f\u6210\u529f\u3057\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_SAX_EXCEPTION, "SAX \u4f8b\u5916"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "\u6a5f\u80fd\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_XSLT_ERROR, "XSLT \u30a8\u30e9\u30fc"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "\u901a\u8ca8\u8a18\u53f7\u306f\u66f8\u5f0f\u30d1\u30bf\u30fc\u30f3\u30fb\u30b9\u30c8\u30ea\u30f3\u30b0\u5185\u3067\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "\u6587\u66f8\u6a5f\u80fd\u306f\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8 DOM \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "\u975e\u63a5\u982d\u90e8\u30ea\u30be\u30eb\u30d0\u30fc\u306e\u63a5\u982d\u90e8\u3092\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "\u30ea\u30c0\u30a4\u30ec\u30af\u30c8\u62e1\u5f35: \u30d5\u30a1\u30a4\u30eb\u540d\u3092\u53d6\u5f97\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002file \u307e\u305f\u306f select \u5c5e\u6027\u306f\u6709\u52b9\u306a\u30b9\u30c8\u30ea\u30f3\u30b0\u3092\u623b\u3055\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "FormatterListener \u306f\u30ea\u30c0\u30a4\u30ec\u30af\u30c8\u62e1\u5f35\u5185\u306b\u30d3\u30eb\u30c9\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "exclude-result-prefixes \u5185\u306e\u63a5\u982d\u90e8\u304c\u7121\u52b9\u3067\u3059: {0}"},
				new object[] {ER_MISSING_NS_URI, "\u6307\u5b9a\u3055\u308c\u305f\u63a5\u982d\u90e8\u306e\u540d\u524d\u7a7a\u9593 URI \u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "\u30aa\u30d7\u30b7\u30e7\u30f3\u306e\u5f15\u6570\u304c\u3042\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_INVALID_OPTION, "\u7121\u52b9\u306a\u30aa\u30d7\u30b7\u30e7\u30f3: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "\u8aa4\u3063\u305f\u5f62\u5f0f\u306e\u66f8\u5f0f\u30b9\u30c8\u30ea\u30f3\u30b0: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet \u306b\u306f 'version' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "\u5c5e\u6027: {0} \u306b\u306f\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c\u3042\u308a\u307e\u3059\u3002"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose \u306b\u306f xsl:when \u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports \u306f xsl:for-each \u5185\u3067\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "DTMLiaison \u306f\u51fa\u529b DOM \u30ce\u30fc\u30c9\u306b\u4f7f\u7528\u3067\u304d\u307e\u305b\u3093... \u4ee3\u308a\u306b org.apache.xpath.DOM2Helper \u3092\u6e21\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "DTMLiaison \u306f\u5165\u529b DOM \u30ce\u30fc\u30c9\u306b\u4f7f\u7528\u3067\u304d\u307e\u305b\u3093... \u4ee3\u308a\u306b org.apache.xpath.DOM2Helper \u3092\u6e21\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {ER_CALL_TO_EXT_FAILED, "\u62e1\u5f35\u8981\u7d20\u3078\u306e\u547c\u3073\u51fa\u3057\u304c\u5931\u6557\u3057\u307e\u3057\u305f: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u63a5\u982d\u90e8\u306f\u540d\u524d\u7a7a\u9593\u306b\u89e3\u6c7a\u3055\u308c\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "\u7121\u52b9\u306a UTF-16 \u30b5\u30ed\u30b2\u30fc\u30c8\u304c\u691c\u51fa\u3055\u308c\u307e\u3057\u305f: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} \u304c\u81ea\u8eab\u3092\u4f7f\u7528\u3057\u3066\u3044\u308b\u305f\u3081\u3001\u7121\u9650\u30eb\u30fc\u30d7\u306e\u539f\u56e0\u3068\u306a\u308a\u307e\u3059\u3002"},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "\u975e Xerces-DOM \u5165\u529b\u3068 Xerces-DOM \u51fa\u529b\u306f\u6df7\u7528\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "ElemTemplateElement.readObject \u5185: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "\u6b21\u306e\u540d\u524d\u306e\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u304c\u8907\u6570\u898b\u3064\u304b\u308a\u307e\u3057\u305f: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "\u7121\u52b9\u306a\u95a2\u6570\u547c\u3073\u51fa\u3057: \u518d\u5e30\u7684 key() \u547c\u3073\u51fa\u3057\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_REFERENCING_ITSELF, "\u5909\u6570 {0} \u304c\u76f4\u63a5\u7684\u307e\u305f\u306f\u9593\u63a5\u7684\u306b\u81ea\u5206\u81ea\u8eab\u306b\u53c2\u7167\u3065\u3051\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "newTemplates \u306e DOMSource \u306e\u5165\u529b\u30ce\u30fc\u30c9\u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "\u30aa\u30d7\u30b7\u30e7\u30f3 {0} \u306e\u30af\u30e9\u30b9\u30fb\u30d5\u30a1\u30a4\u30eb\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "\u5fc5\u8981\u306a\u8981\u7d20\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream \u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI \u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "\u30d5\u30a1\u30a4\u30eb\u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource \u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "BSF \u30de\u30cd\u30fc\u30b8\u30e3\u30fc\u3092\u521d\u671f\u5316\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "\u62e1\u5f35\u6a5f\u80fd\u3092\u30b3\u30f3\u30d1\u30a4\u30eb\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "\u539f\u56e0: {1} \u306e\u305f\u3081\u306b\u62e1\u5f35\u6a5f\u80fd: {0} \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "\u30e1\u30bd\u30c3\u30c9 {0} \u3078\u306e\u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u30fb\u30e1\u30bd\u30c3\u30c9\u547c\u3073\u51fa\u3057\u306b\u306f\u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u30fb\u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u304c\u6700\u521d\u306e\u5f15\u6570\u3068\u3057\u3066\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_INVALID_ELEMENT_NAME, "\u7121\u52b9\u306a\u8981\u7d20\u540d\u304c\u6307\u5b9a\u3055\u308c\u307e\u3057\u305f: {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "\u8981\u7d20\u540d\u30e1\u30bd\u30c3\u30c9\u306f\u9759\u7684\u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093: {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "\u62e1\u5f35\u6a5f\u80fd {0} : {1} \u304c\u4e0d\u660e\u3067\u3059\u3002"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "{0} \u306e\u30b3\u30f3\u30b9\u30c8\u30e9\u30af\u30bf\u30fc\u306e\u6700\u9069\u4e00\u81f4\u304c\u8907\u6570\u3042\u308a\u307e\u3059\u3002"},
				new object[] {ER_MORE_MATCH_METHOD, "\u30e1\u30bd\u30c3\u30c9 {0} \u306e\u6700\u9069\u4e00\u81f4\u304c\u8907\u6570\u3042\u308a\u307e\u3059\u3002"},
				new object[] {ER_MORE_MATCH_ELEMENT, "\u8981\u7d20\u30e1\u30bd\u30c3\u30c9 {0} \u306e\u6700\u9069\u4e00\u81f4\u304c\u8907\u6570\u3042\u308a\u307e\u3059\u3002"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "{0} \u3092\u8a55\u4fa1\u3059\u308b\u305f\u3081\u306b\u6e21\u3055\u308c\u305f\u30b3\u30f3\u30c6\u30ad\u30b9\u30c8\u304c\u7121\u52b9\u3067\u3059\u3002"},
				new object[] {ER_POOL_EXISTS, "\u30d7\u30fc\u30eb\u306f\u3059\u3067\u306b\u5b58\u5728\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_NO_DRIVER_NAME, "\u30c9\u30e9\u30a4\u30d0\u30fc\u540d\u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_NO_URL, "URL \u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "\u30d7\u30fc\u30eb\u30fb\u30b5\u30a4\u30ba\u304c 1 \u3088\u308a\u5c0f\u3067\u3059\u3002"},
				new object[] {ER_INVALID_DRIVER, "\u7121\u52b9\u306a\u30c9\u30e9\u30a4\u30d0\u30fc\u540d\u304c\u6307\u5b9a\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_NO_STYLESHEETROOT, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u306e\u30eb\u30fc\u30c8\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "xml:space \u306e\u5024\u304c\u6b63\u3057\u304f\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode \u304c\u5931\u6557\u3057\u307e\u3057\u305f\u3002"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "\u30ea\u30bd\u30fc\u30b9 [ {0} ] \u3092\u30ed\u30fc\u30c9\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "\u30d0\u30c3\u30d5\u30a1\u30fc\u30fb\u30b5\u30a4\u30ba <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "\u30a8\u30af\u30b9\u30c6\u30f3\u30b7\u30e7\u30f3\u3092\u547c\u3073\u51fa\u3057\u6642\u306b\u4e0d\u660e\u30a8\u30e9\u30fc"},
				new object[] {ER_NO_NAMESPACE_DECL, "\u63a5\u982d\u90e8 {0} \u306b\u306f\u5bfe\u5fdc\u3057\u3066\u3044\u308b\u540d\u524d\u7a7a\u9593\u5ba3\u8a00\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "\u8981\u7d20\u306e\u5185\u5bb9\u306f lang=javaclass {0} \u306e\u5834\u5408\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3067\u7d42\u4e86\u304c\u6307\u56f3\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_ONE_OR_TWO, "1 \u307e\u305f\u306f 2"},
				new object[] {ER_TWO_OR_THREE, "2 \u307e\u305f\u306f 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "{0} \u3092\u30ed\u30fc\u30c9\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f (CLASSPATH \u3092\u8abf\u3079\u3066\u304f\u3060\u3055\u3044)\u3002\u73fe\u5728\u306f\u307e\u3055\u306b\u30c7\u30d5\u30a9\u30eb\u30c8\u3092\u4f7f\u7528\u4e2d\u3067\u3059\u3002"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "\u30c7\u30d5\u30a9\u30eb\u30c8\u30fb\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u3092\u521d\u671f\u5316\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_RESULT_NULL, "\u7d50\u679c\u306f\u30cc\u30eb\u306b\u306f\u306a\u3089\u306a\u3044\u306f\u305a\u3067\u3059\u3002"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "\u7d50\u679c\u3092\u8a2d\u5b9a\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "\u51fa\u529b\u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "\u578b {0} \u306e Result \u306b\u5909\u63db\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "\u578b {0} \u306e Source \u3092\u5909\u63db\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NULL_CONTENT_HANDLER, "\u30cc\u30eb\u306e\u30b3\u30f3\u30c6\u30f3\u30c4\u30fb\u30cf\u30f3\u30c9\u30e9\u30fc"},
				new object[] {ER_NULL_ERROR_HANDLER, "\u30cc\u30eb\u306e\u30a8\u30e9\u30fc\u30fb\u30cf\u30f3\u30c9\u30e9\u30fc"},
				new object[] {ER_CANNOT_CALL_PARSE, "ContentHandler \u304c\u672a\u8a2d\u5b9a\u306e\u5834\u5408\u306f parse \u306e\u547c\u3073\u51fa\u3057\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "\u30d5\u30a3\u30eb\u30bf\u30fc\u306e\u89aa\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u304c {0}\u3001\u30e1\u30c7\u30a3\u30a2= {1} \u306b\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_NO_STYLESHEET_PI, "XML \u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8 PI \u304c {0} \u306b\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_NOT_SUPPORTED, "\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "\u30d7\u30ed\u30d1\u30c6\u30a3\u30fc {0} \u306e\u5024\u306f\u30d6\u30fc\u30eb\u30fb\u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u306b\u3059\u308b\u5fc5\u8981\u304c\u3042\u308a\u307e\u3059\u3002"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "{0} \u306e\u5916\u90e8\u30b9\u30af\u30ea\u30d7\u30c8\u3078\u5230\u9054\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "\u30ea\u30bd\u30fc\u30b9 [ {0} ] \u306f\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "\u51fa\u529b\u30d7\u30ed\u30d1\u30c6\u30a3\u30fc\u306f\u8a8d\u8b58\u3055\u308c\u3066\u3044\u307e\u305b\u3093: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "ElemLiteralResult \u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u306e\u4f5c\u6210\u304c\u5931\u6557\u3057\u307e\u3057\u305f\u3002"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "{0} \u306e\u5024\u306b\u306f\u69cb\u6587\u89e3\u6790\u53ef\u80fd\u756a\u53f7\u304c\u542b\u307e\u308c\u3066\u3044\u308b\u306f\u305a\u3067\u3059\u3002"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "{0} \u306e\u5024\u306f yes \u307e\u305f\u306f no \u3068\u7b49\u3057\u304f\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_FAILED_CALLING_METHOD, "{0} \u30e1\u30bd\u30c3\u30c9\u306e\u547c\u3073\u51fa\u3057\u304c\u5931\u6557\u3057\u307e\u3057\u305f\u3002"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "ElemTemplateElement \u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u306e\u4f5c\u6210\u304c\u5931\u6557\u3057\u307e\u3057\u305f\u3002"},
				new object[] {ER_CHARS_NOT_ALLOWED, "\u6587\u5b57\u306f\u6587\u66f8\u5185\u306e\u3053\u306e\u30dd\u30a4\u30f3\u30c8\u3067\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_ATTR_NOT_ALLOWED, "\"{0}\" \u5c5e\u6027\u306f {1} \u8981\u7d20\u3067\u306f\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_BAD_VALUE, "{0} \u306e\u9593\u9055\u3063\u305f\u5024 {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "{0} \u5c5e\u6027\u5024\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002 "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "{0} \u5c5e\u6027\u5024\u306f\u8a8d\u8b58\u3055\u308c\u307e\u305b\u3093\u3002 "},
				new object[] {ER_NULL_URI_NAMESPACE, "\u540d\u524d\u7a7a\u9593\u63a5\u982d\u90e8\u3092\u30cc\u30eb\u306e URI \u3067\u751f\u6210\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_NUMBER_TOO_BIG, "\u6700\u5927 Long \u6574\u6570\u3088\u308a\u5927\u304d\u3044\u6570\u3092\u30d5\u30a9\u30fc\u30de\u30c3\u30c8\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "SAX1 \u30c9\u30e9\u30a4\u30d0\u30fc\u30fb\u30af\u30e9\u30b9 {0} \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "SAX1 \u30c9\u30e9\u30a4\u30d0\u30fc\u30fb\u30af\u30e9\u30b9 {0} \u304c\u898b\u3064\u304b\u308a\u307e\u3057\u305f\u304c\u30ed\u30fc\u30c9\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "SAX1 \u30c9\u30e9\u30a4\u30d0\u30fc\u30fb\u30af\u30e9\u30b9 {0} \u304c\u30ed\u30fc\u30c9\u3055\u308c\u307e\u3057\u305f\u304c\u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u751f\u6210\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "SAX1 \u30c9\u30e9\u30a4\u30d0\u30fc\u30fb\u30af\u30e9\u30b9 {0} \u304c org.xml.sax.Parser \u3092\u5b9f\u88c5\u3057\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "\u30b7\u30b9\u30c6\u30e0\u30fb\u30d7\u30ed\u30d1\u30c6\u30a3\u30fc org.xml.sax.parser \u306f\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "\u30d1\u30fc\u30b5\u30fc\u306e\u5f15\u6570\u3092\u30cc\u30eb\u306b\u3057\u3066\u306f\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_FEATURE, "\u6a5f\u80fd: {0}"},
				new object[] {ER_PROPERTY, "\u30d7\u30ed\u30d1\u30c6\u30a3\u30fc: {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "\u30cc\u30eb\u5b9f\u4f53\u30ea\u30be\u30eb\u30d0\u30fc"},
				new object[] {ER_NULL_DTD_HANDLER, "\u30cc\u30eb DTD \u30cf\u30f3\u30c9\u30e9\u30fc"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "\u30c9\u30e9\u30a4\u30d0\u30fc\u540d\u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_NO_URL_SPECIFIED, "URL \u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "\u30d7\u30fc\u30eb\u30fb\u30b5\u30a4\u30ba\u304c 1 \u3088\u308a\u5c0f\u3055\u304f\u306a\u3063\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_INVALID_DRIVER_NAME, "\u7121\u52b9\u306a\u30c9\u30e9\u30a4\u30d0\u30fc\u540d\u304c\u6307\u5b9a\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "\u30d7\u30ed\u30b0\u30e9\u30de\u30fc\u306e\u30a8\u30e9\u30fc:  \u5f0f\u306b ElemTemplateElement \u89aa\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "RedundentExprEliminator \u5185\u306e\u30d7\u30ed\u30b0\u30e9\u30de\u30fc\u306e\u30a2\u30b5\u30fc\u30b7\u30e7\u30f3: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} \u306f\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u306e\u3053\u306e\u4f4d\u7f6e\u3067\u306f\u8a31\u53ef\u3055\u308c\u307e\u305b\u3093\u3002"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "\u7a7a\u767d\u6587\u5b57\u4ee5\u5916\u306e\u30c6\u30ad\u30b9\u30c8\u306f\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u306e\u3053\u306e\u4f4d\u7f6e\u3067\u306f\u8a31\u53ef\u3055\u308c\u307e\u305b\u3093\u3002"},
				new object[] {INVALID_TCHAR, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c CHAR \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002CHAR \u578b\u306e\u5c5e\u6027\u306f 1 \u6587\u5b57\u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {INVALID_QNAME, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c QNAME \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {INVALID_ENUM, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c ENUM \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002\u6709\u52b9\u5024: {2}\u3002"},
				new object[] {INVALID_NMTOKEN, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c NMTOKEN \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {INVALID_NCNAME, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c NCNAME \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {INVALID_BOOLEAN, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c boolean \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {INVALID_NUMBER, "\u6b63\u3057\u304f\u306a\u3044\u5024: {1} \u304c number \u5c5e\u6027: {0} \u306b\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {ER_ARG_LITERAL, "\u30de\u30c3\u30c1\u30f3\u30b0\u30fb\u30d1\u30bf\u30fc\u30f3\u306e {0} \u3078\u306e\u5f15\u6570\u306f\u30ea\u30c6\u30e9\u30eb\u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "\u30b0\u30ed\u30fc\u30d0\u30eb\u5909\u6570\u5ba3\u8a00\u304c\u91cd\u8907\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_DUPLICATE_VAR, "\u5909\u6570\u5ba3\u8a00\u304c\u91cd\u8907\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template \u306b\u306f name \u307e\u305f\u306f match \u5c5e\u6027 (\u3042\u308b\u3044\u306f\u305d\u306e\u4e21\u65b9) \u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {ER_INVALID_PREFIX, "exclude-result-prefixes \u5185\u306e\u63a5\u982d\u90e8\u304c\u7121\u52b9\u3067\u3059: {0}"},
				new object[] {ER_NO_ATTRIB_SET, "{0} \u3068\u3044\u3046\u540d\u524d\u306e attribute-set \u304c\u5b58\u5728\u3057\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_FUNCTION_NOT_FOUND, "{0} \u3068\u3044\u3046\u540d\u524d\u306e\u95a2\u6570\u304c\u5b58\u5728\u3057\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "{0} \u8981\u7d20\u306b\u5185\u5bb9\u304a\u3088\u3073 select \u5c5e\u6027\u306e\u4e21\u65b9\u304c\u3042\u3063\u3066\u306f\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "param {0} \u306e\u5024\u306f\u6709\u52b9\u306a Java \u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u3067\u3042\u308b\u5fc5\u8981\u304c\u3042\u308a\u307e\u3059"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "xsl:namespace-alias \u8981\u7d20\u306e result-prefix \u5c5e\u6027\u306e\u5024\u304c '#default' \u306b\u306a\u3063\u3066\u3044\u307e\u3059\u304c\u3001\u3053\u306e\u8981\u7d20\u306e\u30b9\u30b3\u30fc\u30d7\u5185\u306b\u306f\u30c7\u30d5\u30a9\u30eb\u30c8\u540d\u524d\u7a7a\u9593\u306e\u5ba3\u8a00\u306f\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "xsl:namespace-alias \u8981\u7d20\u306e result-prefix \u5c5e\u6027\u306e\u5024\u304c ''{0}'' \u306b\u306a\u3063\u3066\u3044\u307e\u3059\u304c\u3001\u3053\u306e\u8981\u7d20\u306e\u30b9\u30b3\u30fc\u30d7\u5185\u306b\u306f\u63a5\u982d\u90e8 ''{0}'' \u306e\u540d\u524d\u7a7a\u9593\u5ba3\u8a00\u306f\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {ER_SET_FEATURE_NULL_NAME, "TransformerFactory.setFeature(String name, boolean value) \u306e\u6a5f\u80fd\u540d\u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_GET_FEATURE_NULL_NAME, "TransformerFactory.getFeature(String name) \u306e\u6a5f\u80fd\u540d\u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_UNSUPPORTED_FEATURE, "\u6a5f\u80fd ''{0}'' \u306f\u3053\u306e TransformerFactory \u306b\u8a2d\u5b9a\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "\u30bb\u30ad\u30e5\u30ea\u30c6\u30a3\u30fc\u4fdd\u8b77\u3055\u308c\u305f\u51e6\u7406\u6a5f\u80fd\u304c true \u306b\u8a2d\u5b9a\u3055\u308c\u3066\u3044\u308b\u3068\u304d\u306b\u3001\u62e1\u5f35\u8981\u7d20 ''{0}'' \u3092\u4f7f\u7528\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "\u30cc\u30eb\u540d\u524d\u7a7a\u9593 URI \u306e\u63a5\u982d\u90e8\u306f\u53d6\u5f97\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "\u30cc\u30eb\u63a5\u982d\u90e8\u306e\u540d\u524d\u7a7a\u9593 URI \u306f\u53d6\u5f97\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "\u95a2\u6570\u540d \u306f\u30cc\u30eb\u306b\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "\u30a2\u30ea\u30c6\u30a3\u30fc (\u5f15\u6570\u306e\u6570) \u306f\u8ca0\u306e\u5024\u306b\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {WG_FOUND_CURLYBRACE, "'}' \u304c\u898b\u3064\u304b\u308a\u307e\u3057\u305f\u304c\u3001\u30aa\u30fc\u30d7\u30f3\u3055\u308c\u305f\u5c5e\u6027\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "\u8b66\u544a: count \u5c5e\u6027\u304c xsl:number \u5185\u306e\u4e0a\u4f4d\u3068\u4e00\u81f4\u3057\u307e\u305b\u3093\u3002 \u30bf\u30fc\u30b2\u30c3\u30c8 = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "\u65e7\u69cb\u6587: 'expr' \u5c5e\u6027\u306e\u540d\u524d\u304c 'select' \u306b\u5909\u66f4\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan \u306f\u30d5\u30a9\u30fc\u30de\u30c3\u30c8\u756a\u53f7\u95a2\u6570\u5185\u3067\u307e\u3060\u30ed\u30b1\u30fc\u30eb\u540d\u3092\u51e6\u7406\u3057\u307e\u305b\u3093\u3002"},
				new object[] {WG_LOCALE_NOT_FOUND, "\u8b66\u544a: xml:lang={0} \u306e\u30ed\u30b1\u30fc\u30eb\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "URL \u3092 {0} \u304b\u3089\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "\u8981\u6c42\u3055\u308c\u305f doc: {0} \u3092\u30ed\u30fc\u30c9\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "<sort xml:lang={0} \u306e\u30b3\u30ec\u30fc\u30bf\u30fc\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "\u65e7\u69cb\u6587: \u95a2\u6570\u547d\u4ee4\u3067\u306f {0} \u306e URL \u3092\u4f7f\u7528\u3059\u308b\u5fc5\u8981\u304c\u3042\u308a\u307e\u3059\u3002"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "\u30a8\u30f3\u30b3\u30fc\u30c9\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093: {0} \u306f UTF-8 \u3092\u4f7f\u7528\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "\u30a8\u30f3\u30b3\u30fc\u30c9\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u307e\u305b\u3093: {0} \u306f Java {1} \u3092\u4f7f\u7528\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "\u9650\u5b9a\u6027\u306e\u77db\u76fe\u304c\u691c\u51fa\u3055\u308c\u307e\u3057\u305f: {0} \u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u5185\u3067\u6700\u5f8c\u306b\u691c\u51fa\u3055\u308c\u305f\u3082\u306e\u304c\u4f7f\u7528\u3055\u308c\u307e\u3059\u3002"},
				new object[] {WG_PARSING_AND_PREPARING, "========= {0} \u3092\u69cb\u6587\u89e3\u6790\u4e2d\u304a\u3088\u3073\u6e96\u5099\u4e2d =========="},
				new object[] {WG_ATTR_TEMPLATE, "\u5c5e\u6027\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8 {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "xsl:strip-space \u3068 xsl:preserve-space \u306e\u9593\u306e\u30de\u30c3\u30c1\u30f3\u30b0\u306e\u77db\u76fe"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan \u306f\u307e\u3060 {0} \u5c5e\u6027\u3092\u51e6\u7406\u3057\u307e\u305b\u3093\u3002"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "10 \u9032\u6570\u5f62\u5f0f\u306e\u5ba3\u8a00\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093: {0}"},
				new object[] {WG_OLD_XSLT_NS, "XSLT \u540d\u524d\u7a7a\u9593\u304c\u306a\u3044\u304b\u8aa4\u3063\u3066\u3044\u307e\u3059\u3002 "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "\u30c7\u30d5\u30a9\u30eb\u30c8\u306e xsl:decimal-format \u5ba3\u8a00\u306f 1 \u3064\u3057\u304b\u8a31\u53ef\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "xsl:decimal-format \u540d\u306f\u56fa\u6709\u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002\u540d\u524d \"{0}\" \u304c\u91cd\u8907\u3057\u3066\u3044\u307e\u3057\u305f\u3002"},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} \u306b\u6b63\u3057\u304f\u306a\u3044\u5c5e\u6027\u304c\u3042\u308a\u307e\u3059: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "\u540d\u524d\u7a7a\u9593\u63a5\u982d\u90e8\u3092\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f: {0} - \u30ce\u30fc\u30c9\u306f\u7121\u8996\u3055\u308c\u307e\u3059\u3002"},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet \u306b\u306f 'version' \u5c5e\u6027\u304c\u5fc5\u8981\u3067\u3059\u3002"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "\u6b63\u3057\u304f\u306a\u3044\u5c5e\u6027\u540d: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "\u5c5e\u6027 {0}: {1} \u306b\u4f7f\u7528\u3055\u308c\u305f\u5024\u306f\u6b63\u3057\u304f\u3042\u308a\u307e\u305b\u3093\u3002"},
				new object[] {WG_EMPTY_SECOND_ARG, "\u6587\u66f8\u6a5f\u80fd\u306e 2 \u756a\u76ee\u306e\u5f15\u6570\u304b\u3089\u5f97\u3089\u308c\u305f nodeset \u304c\u7a7a\u3067\u3059\u3002\u7a7a\u306e node-set \u3092\u623b\u3057\u307e\u3059\u3002"},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "xsl:processing-instruction \u540d\u306e 'name' \u5c5e\u6027\u306e\u5024\u306f 'xml' \u3067\u3042\u3063\u3066\u306f\u306a\u308a\u307e\u305b\u3093\u3002"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "xsl:processing-instruction \u306e ''name'' \u5c5e\u6027\u306e\u5024\u306f\u6709\u52b9\u306a NCName \u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "\u4e0b\u4f4d\u30ce\u30fc\u30c9\u306e\u5f8c\u307e\u305f\u306f\u8981\u7d20\u304c\u751f\u6210\u3055\u308c\u308b\u524d\u306b\u5c5e\u6027 {0} \u306f\u8ffd\u52a0\u3067\u304d\u307e\u305b\u3093\u3002\u5c5e\u6027\u306f\u7121\u8996\u3055\u308c\u307e\u3059\u3002"},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "\u5909\u66f4\u3067\u304d\u306a\u3044\u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u3092\u5909\u66f4\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "createMessage \u3078\u306e\u30d1\u30e9\u30e1\u30fc\u30bf\u30fc\u304c\u7bc4\u56f2\u5916\u3067\u3057\u305f\u3002"},
				new object[] {"FORMAT_FAILED", "messageFormat \u547c\u3073\u51fa\u3057\u4e2d\u306b\u4f8b\u5916\u304c\u30b9\u30ed\u30fc\u3055\u308c\u307e\u3057\u305f\u3002"},
				new object[] {"version", ">>>>>>> Xalan \u30d0\u30fc\u30b8\u30e7\u30f3 "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "\u306f\u3044 (y)"},
				new object[] {"line", "\u884c #"},
				new object[] {"column", "\u6841 #"},
				new object[] {"xsldone", "XSLProcessor: \u5b8c\u4e86"},
				new object[] {"xslProc_option", "Xalan-J \u30b3\u30de\u30f3\u30c9\u884c Process \u30af\u30e9\u30b9\u30fb\u30aa\u30d7\u30b7\u30e7\u30f3"},
				new object[] {"xslProc_option", "Xalan-J \u30b3\u30de\u30f3\u30c9\u884c Process \u30af\u30e9\u30b9\u30fb\u30aa\u30d7\u30b7\u30e7\u30f3\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "\u30aa\u30d7\u30b7\u30e7\u30f3 {0} \u306f XSLTC \u30e2\u30fc\u30c9\u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				new object[] {"xslProc_invalid_xalan_option", "\u30aa\u30d7\u30b7\u30e7\u30f3 {0} \u306f -XSLTC \u3068\u4e00\u7dd2\u306b\u3057\u304b\u4f7f\u7528\u3067\u304d\u307e\u305b\u3093\u3002"},
				new object[] {"xslProc_no_input", "\u30a8\u30e9\u30fc: \u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u304c\u306a\u3044\u304b\u5165\u529b xml \u304c\u6307\u5b9a\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002\u4f7f\u7528\u6cd5\u306e\u8aac\u660e\u306b\u3064\u3044\u3066\u306f\u3001\u30aa\u30d7\u30b7\u30e7\u30f3\u306a\u3057\u3067\u3053\u306e\u30b3\u30de\u30f3\u30c9\u3092\u5b9f\u884c\u3057\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {"xslProc_common_options", "-\u5171\u901a\u30aa\u30d7\u30b7\u30e7\u30f3-"},
				new object[] {"xslProc_xalan_options", "-Xalan \u7528\u30aa\u30d7\u30b7\u30e7\u30f3-"},
				new object[] {"xslProc_xsltc_options", "-XSLTC \u7528\u30aa\u30d7\u30b7\u30e7\u30f3-"},
				new object[] {"xslProc_return_to_continue", "(\u7d9a\u3051\u308b\u306b\u306f <return> \u3092\u62bc\u3057\u3066\u304f\u3060\u3055\u3044)"},
				new object[] {"optionXSLTC", "   [-XSLTC (\u5909\u63db\u306b XSLTC \u3092\u4f7f\u7528)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT outputFileName]"},
				new object[] {"optionLXCIN", "   [-LXCIN compiledStylesheetFileNameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT compiledStylesheetFileNameOutOut]"},
				new object[] {"optionPARSER", "   [-PARSER parser liaison \u306e\u5b8c\u5168\u4fee\u98fe\u30af\u30e9\u30b9\u540d]"},
				new object[] {"optionE", "   [-E (\u5b9f\u4f53\u53c2\u7167\u3092\u5c55\u958b\u3057\u306a\u3044)]"},
				new object[] {"optionV", "   [-E (\u5b9f\u4f53\u53c2\u7167\u3092\u5c55\u958b\u3057\u306a\u3044)]"},
				new object[] {"optionQC", "   [-QC (\u9759\u6b62\u30d1\u30bf\u30fc\u30f3\u77db\u76fe\u8b66\u544a)]"},
				new object[] {"optionQ", "   [-Q  (\u9759\u6b62\u30e2\u30fc\u30c9)]"},
				new object[] {"optionLF", "   [-LF (LF (\u6539\u884c) \u3092\u51fa\u529b\u6642\u306e\u307f\u306b\u4f7f\u7528  {\u30c7\u30d5\u30a9\u30eb\u30c8\u306f CR/LF})]"},
				new object[] {"optionCR", "   [-CR (CR (\u5fa9\u5e30) \u3092\u51fa\u529b\u6642\u306e\u307f\u306b\u4f7f\u7528 {\u30c7\u30d5\u30a9\u30eb\u30c8\u306f CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (\u30a8\u30b9\u30b1\u30fc\u30d7\u3059\u308b\u6587\u5b57 {\u30c7\u30d5\u30a9\u30eb\u30c8\u306f <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (\u5b57\u4e0b\u3052\u3059\u308b\u30b9\u30da\u30fc\u30b9\u3092\u5236\u5fa1 {\u30c7\u30d5\u30a9\u30eb\u30c8\u306f 0})]"},
				new object[] {"optionTT", "   [-TT (\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u3092\u547c\u3073\u51fa\u3057\u4e2d\u306b\u30c8\u30ec\u30fc\u30b9\u3002)]"},
				new object[] {"optionTG", "   [-TG (\u5404\u751f\u6210\u30a4\u30d9\u30f3\u30c8\u3092\u30c8\u30ec\u30fc\u30b9\u3002)]"},
				new object[] {"optionTS", "   [-TS (\u5404\u9078\u629e\u30a4\u30d9\u30f3\u30c8\u3092\u30c8\u30ec\u30fc\u30b9\u3002)]"},
				new object[] {"optionTTC", "   [-TTC (\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306e\u5b50\u3092\u547c\u3073\u51fa\u3057\u4e2d\u306b\u30c8\u30ec\u30fc\u30b9\u3002)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (\u30c8\u30ec\u30fc\u30b9\u62e1\u5f35\u6a5f\u80fd\u306e TraceListener \u30af\u30e9\u30b9\u3002)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (\u59a5\u5f53\u6027\u691c\u67fb\u3092\u5b9f\u884c\u3059\u308b\u304b\u3069\u3046\u304b\u3092\u8a2d\u5b9a\u3002  \u30c7\u30d5\u30a9\u30eb\u30c8\u3067\u306f\u3001\u59a5\u5f53\u6027\u691c\u67fb\u306f\u30aa\u30d5\u3067\u3059\u3002)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {optional filename} (\u30a8\u30e9\u30fc\u6642\u306b stackdump \u3092\u5b9f\u884c\u3002)]"},
				new object[] {"optionXML", "   [-XML (XML \u30d5\u30a9\u30fc\u30de\u30c3\u30bf\u30fc\u3092\u4f7f\u7528\u304a\u3088\u3073 XML \u30d8\u30c3\u30c0\u30fc\u3092\u8ffd\u52a0\u3002)]"},
				new object[] {"optionTEXT", "   [-TEXT (\u30b7\u30f3\u30d7\u30eb\u30fb\u30c6\u30ad\u30b9\u30c8\u30fb\u30d5\u30a9\u30fc\u30de\u30c3\u30bf\u30fc\u3092\u4f7f\u7528\u3002)]"},
				new object[] {"optionHTML", "   [-HTML (HTML \u30d5\u30a9\u30fc\u30de\u30c3\u30bf\u30fc\u3092\u4f7f\u7528\u3002)]"},
				new object[] {"optionPARAM", "   [-PARAM \u540d\u524d\u5f0f (stylesheet \u30d1\u30e9\u30e1\u30fc\u30bf\u30fc\u3092\u8a2d\u5b9a\u3002)]"},
				new object[] {"noParsermsg1", "XSL \u51e6\u7406\u306f\u6210\u529f\u3057\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				new object[] {"noParsermsg2", "** \u30d1\u30fc\u30b5\u30fc\u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f **"},
				new object[] {"noParsermsg3", "\u30af\u30e9\u30b9\u30d1\u30b9\u3092\u8abf\u3079\u3066\u304f\u3060\u3055\u3044\u3002"},
				new object[] {"noParsermsg4", "IBM \u306e XML Parser for Java \u304c\u306a\u3044\u5834\u5408\u306f\u3001\u6b21\u306e\u30b5\u30a4\u30c8\u304b\u3089\u30c0\u30a6\u30f3\u30ed\u30fc\u30c9\u3067\u304d\u307e\u3059:"},
				new object[] {"noParsermsg5", "IBM AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER \u7d76\u5bfe\u30af\u30e9\u30b9\u540d (URI \u3092\u89e3\u6c7a\u3059\u308b\u305f\u3081\u306b\u4f7f\u7528\u3059\u308b URIResolver)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER \u7d76\u5bfe\u30af\u30e9\u30b9\u540d (\u5b9f\u4f53\u3092\u89e3\u6c7a\u3059\u308b\u305f\u3081\u306b\u4f7f\u7528\u3059\u308b EntityResolver)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER \u7d76\u5bfe\u30af\u30e9\u30b9\u540d (\u51fa\u529b\u3092\u30b7\u30ea\u30a2\u30e9\u30a4\u30ba\u3059\u308b\u305f\u3081\u306b\u4f7f\u7528\u3059\u308b ContentHandler)]"},
				new object[] {"optionLINENUMBERS", "   [-L \u30bd\u30fc\u30b9\u30fb\u30c9\u30ad\u30e5\u30e1\u30f3\u30c8\u306e\u884c\u756a\u53f7\u3092\u4f7f\u7528]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (\u30bb\u30ad\u30e5\u30ea\u30c6\u30a3\u30fc\u4fdd\u8b77\u3055\u308c\u305f\u51e6\u7406\u6a5f\u80fd\u3092 true \u306b\u8a2d\u5b9a)]"},
				new object[] {"optionMEDIA", "   [-MEDIA mediaType (\u6587\u66f8\u3068\u95a2\u9023\u3057\u305f\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3092\u691c\u7d22\u3059\u308b\u30e1\u30c7\u30a3\u30a2\u5c5e\u6027\u3092\u4f7f\u7528\u3002)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR flavorName (\u5909\u63db\u3092\u5b9f\u884c\u3059\u308b\u305f\u3081\u306b s2s=SAX \u307e\u305f\u306f d2d=DOM \u3092\u660e\u793a\u7684\u306b\u4f7f\u7528\u3002)] "},
				new object[] {"optionDIAG", "   [-DIAG (\u5909\u63db\u306b\u304b\u304b\u3063\u305f\u5168\u30df\u30ea\u79d2\u3092\u5370\u5237\u3002)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (http://xml.apache.org/xalan/features/incremental \u3092 true \u306b\u8a2d\u5b9a\u3059\u308b\u3053\u3068\u306b\u3088\u308a\u5897\u5206 DTM \u69cb\u9020\u3092\u8981\u6c42\u3002)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (http://xml.apache.org/xalan/features/optimize \u3092 false \u306b\u8a2d\u5b9a\u3059\u308b\u3053\u3068\u306b\u3088\u308a\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u6700\u9069\u5316\u51e6\u7406\u306a\u3057\u3092\u8981\u6c42\u3002)]"},
				new object[] {"optionRL", "   [-RL recursionlimit (\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u306e\u518d\u5e30\u306e\u6df1\u3055\u306b\u3064\u3044\u3066\u306e\u6570\u5024\u9650\u754c\u3092\u6307\u5b9a\u3002)]"},
				new object[] {"optionXO", "   [-XO [transletName] (\u540d\u524d\u3092\u751f\u6210\u5f8c\u306e translet \u306b\u5272\u308a\u5f53\u3066)]"},
				new object[] {"optionXD", "   [-XD destinationDirectory (\u5b9b\u5148\u30c7\u30a3\u30ec\u30af\u30c8\u30ea\u30fc\u3092 translet \u306b\u6307\u5b9a)]"},
				new object[] {"optionXJ", "   [-XJ jarfile (translet \u30af\u30e9\u30b9\u3092\u540d\u524d <jarfile> \u306e JAR \u30d5\u30a1\u30a4\u30eb\u306b\u30d1\u30c3\u30b1\u30fc\u30b8\u3057\u307e\u3059)]"},
				new object[] {"optionXP", "   [-XP package (\u30d1\u30c3\u30b1\u30fc\u30b8\u540d\u63a5\u982d\u90e8\u3092\u3059\u3079\u3066\u306e\u751f\u6210\u5f8c\u306e translet \u30af\u30e9\u30b9\u306b\u6307\u5b9a\u3057\u307e\u3059)]"},
				new object[] {"optionXN", "   [-XN (\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u3092\u30a4\u30f3\u30e9\u30a4\u30f3\u3067\u4f7f\u7528\u53ef\u80fd\u306b\u3057\u307e\u3059)]"},
				new object[] {"optionXX", "   [-XX (\u8ffd\u52a0\u306e\u30c7\u30d0\u30c3\u30b0\u30fb\u30e1\u30c3\u30bb\u30fc\u30b8\u51fa\u529b\u3092\u30aa\u30f3\u306b\u3057\u307e\u3059)]"},
				new object[] {"optionXT", "   [-XT (\u53ef\u80fd\u306a\u5834\u5408\u306f translet \u3092\u4f7f\u7528\u3057\u3066\u5909\u63db)]"},
				new object[] {"diagTiming", " --------- {0} \u306e {1} \u306b\u3088\u308b\u5909\u63db\u306b\u306f {2} \u30df\u30ea\u79d2\u304b\u304b\u308a\u307e\u3057\u305f"},
				new object[] {"recursionTooDeep", "\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306e\u30cd\u30b9\u30c8\u304c\u6df1\u3059\u304e\u307e\u3059\u3002 \u30cd\u30b9\u30c8 = {0}\u3001\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8 {1} {2}"},
				new object[] {"nameIs", "\u540d\u524d\u306f"},
				new object[] {"matchPatternIs", "\u30de\u30c3\u30c1\u30f3\u30b0\u30fb\u30d1\u30bf\u30fc\u30f3\u306f"}
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
	  public const string ERROR_HEADER = "\u30a8\u30e9\u30fc: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "\u8b66\u544a: ";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public const string QUERY_HEADER = "\u30d1\u30bf\u30fc\u30f3 ";


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