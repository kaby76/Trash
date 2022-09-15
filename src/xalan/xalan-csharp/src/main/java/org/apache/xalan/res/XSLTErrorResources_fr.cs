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
 * $Id: XSLTErrorResources_fr.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_fr : ListResourceBundle
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
				new object[] {ER_NO_CURLYBRACE, "Erreur : '{' interdit dans une expression"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} comporte un attribut incorrect : {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode est vide dans xsl:apply-imports !"},
				new object[] {ER_CANNOT_ADD, "Impossible d''ajouter {0} \u00e0 {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode est vide dans handleApplyTemplatesInstruction !"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} doit poss\u00e9der un attribut de nom."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Impossible de trouver le mod\u00e8le : {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Impossible de convertir l'AVT du nom dans xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} requiert l''attribut : {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} doit avoir un attribut ''test''."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "Valeur erron\u00e9e dans l''attribut de niveau : {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Le nom de l'instruction de traitement ne peut \u00eatre 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Le nom de l''instruction de traitement doit \u00eatre un NCName valide : {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} doit poss\u00e9der un attribut de correspondance s''il poss\u00e8de un mode."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} requiert un nom ou un attribut de correspondance."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Impossible de r\u00e9soudre le pr\u00e9fixe de l''espace de noms : {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space comporte une valeur non valide : {0}"},
				new object[] {ER_NO_OWNERDOC, "Le noeud enfant ne poss\u00e8de pas de document propri\u00e9taire !"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "Erreur de ElemTemplateElement : {0}"},
				new object[] {ER_NULL_CHILD, "Tentative d'ajout d'un enfant vide !"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} requiert un attribut de s\u00e9lection."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when doit poss\u00e9der un attribut 'test'."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param doit poss\u00e9der un attribut 'name'."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "Le contexte ne poss\u00e8de pas de document propri\u00e9taire !"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Impossible de cr\u00e9er XML TransformerFactory Liaison : {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Echec du processus Xalan."},
				new object[] {ER_NOT_SUCCESSFUL, "Echec de Xalan."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Encodage non pris en charge : {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Impossible de cr\u00e9er TraceListener : {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key requiert un attribut 'name' !"},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key requiert un attribut 'match' !"},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key requiert un attribut 'use' !"},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} requiert un attribut ''elements''"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "L''attribut ''prefix'' de (StylesheetHandler) {0} est manquant"},
				new object[] {ER_BAD_STYLESHEET_URL, "URL de la feuille de style erron\u00e9 : {0}"},
				new object[] {ER_FILE_NOT_FOUND, "Fichier de la feuille de style introuvable : {0}"},
				new object[] {ER_IOEXCEPTION, "Exception d''E-S avec le fichier de la feuille de style : {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Impossible de trouver d''attribut href pour {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} est directement ou indirectement inclus dans lui-m\u00eame !"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "Erreur de StylesheetHandler.processInclude, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "L''attribut ''lang'' de (StylesheetHandler) {0} est manquant"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) position de l''\u00e9l\u00e9ment {0} inad\u00e9quate ? El\u00e9ment ''component'' de conteneur manquant"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Seule sortie possible vers Element, DocumentFragment, Document ou PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "Erreur de StylesheetRoot.process"},
				new object[] {ER_UNIMPLNODE_ERROR, "Erreur de UnImplNode : {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Erreur ! Impossible de trouver l'expression de s\u00e9lection xpath (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Impossible de s\u00e9rialiser un XSLProcessor !"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Entr\u00e9e de feuille de style non sp\u00e9cifi\u00e9e !"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Impossible de traiter la feuille de style !"},
				new object[] {ER_COULDNT_PARSE_DOC, "Impossible d''analyser le document {0} !"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Impossible de trouver le fragment : {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Le noeud d\u00e9sign\u00e9 par l''identificateur de fragment n''est pas un \u00e9l\u00e9ment : {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each doit poss\u00e9der un attribut de correspondance ou de nom"},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "Les mod\u00e8les doivent poss\u00e9der un attribut de correspondance ou de nom"},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Pas de clone dans un fragment de document !"},
				new object[] {ER_CANT_CREATE_ITEM, "Impossible de cr\u00e9er l''\u00e9l\u00e9ment dans l''arborescence de r\u00e9sultats : {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space du source XML poss\u00e8de une valeur incorrecte : {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Aucune d\u00e9claration xsl:key pour {0} !"},
				new object[] {ER_CANT_CREATE_URL, "Erreur ! Impossible de cr\u00e9er une URL pour : {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions n'est pas pris en charge"},
				new object[] {ER_PROCESSOR_ERROR, "Erreur TransformerFactory de XSLT"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} n''est pas pris en charge dans une feuille de style !"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns n'est plus pris en charge !  Pr\u00e9f\u00e9rez xsl:output."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space n'est plus pris en charge !  Pr\u00e9f\u00e9rez xsl:strip-space ou xsl:preserve-space."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result n'est plus pris en charge !  Pr\u00e9f\u00e9rez xsl:output."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} comporte un attribut incorrect : {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "El\u00e9ment XSL inconnu : {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort ne peut \u00eatre utilis\u00e9 qu'avec xsl:apply-templates ou xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) xsl:when ne figure pas \u00e0 la bonne position !"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when sans rapport avec xsl:choose !"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) xsl:otherwise ne figure pas \u00e0 la bonne position !"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise sans rapport avec xsl:choose !"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} n''est pas admis dans un mod\u00e8le !"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0} pr\u00e9fixe de l''espace de noms de l''extension {1} inconnu"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Les importations peuvent \u00eatre effectu\u00e9es uniquement en tant que premiers \u00e9l\u00e9ments de la feuille de style !"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} s''importe lui-m\u00eame directement ou indirectement !"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) xml:space poss\u00e8de une valeur incorrecte : {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "Echec de processStylesheet !"},
				new object[] {ER_SAX_EXCEPTION, "Exception SAX"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Fonction non prise en charge !"},
				new object[] {ER_XSLT_ERROR, "Erreur XSLT"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "Tout symbole mon\u00e9taire est interdit dans une cha\u00eene de motif de correspondance"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Fonction de document non prise en charge dans le DOM de la feuille de style !"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Impossible de r\u00e9soudre le pr\u00e9fixe du solveur !"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Extension de redirection : Impossible d'extraire le nom du fichier - l'attribut de fichier ou de s\u00e9lection doit retourner une cha\u00eene valide."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Impossible de cr\u00e9er FormatterListener dans une extension Redirect !"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Pr\u00e9fixe de exclude-result-prefixes non valide : {0}"},
				new object[] {ER_MISSING_NS_URI, "URI de l'espace de noms manquant pour le pr\u00e9fixe indiqu\u00e9"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Argument manquant pour l''option : {0}"},
				new object[] {ER_INVALID_OPTION, "Option incorrecte : {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Cha\u00eene de format mal form\u00e9e : {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet requiert un attribut 'version' !"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "L''attribut : {0} poss\u00e8de une valeur non valide : {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose requiert xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports interdit dans un xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Impossible d'utiliser DTMLiaison pour un noeud de DOM en sortie... Transmettez org.apache.xpath.DOM2Helper \u00e0 la place !"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Impossible d'utiliser DTMLiaison pour un noeud de DOM en entr\u00e9e... Transmettez org.apache.xpath.DOM2Helper \u00e0 la place !"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Echec de l''appel de l''\u00e9l\u00e9ment d''extension : {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Le pr\u00e9fixe doit se convertir en espace de noms : {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Substitut UTF-16 non valide d\u00e9tect\u00e9 : {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} s''utilise lui-m\u00eame, ce qui provoque une boucle infinie."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Impossible de m\u00e9langer une entr\u00e9e autre que Xerces-DOM avec une sortie Xerces-DOM !"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "Dans ElemTemplateElement.readObject : {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Plusieurs mod\u00e8les s''appellent : {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Appel de fonction non valide : appels de key() r\u00e9cursifs interdits"},
				new object[] {ER_REFERENCING_ITSELF, "La variable {0} fait r\u00e9f\u00e9rence \u00e0 elle-m\u00eame directement ou indirectement !"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "Le noeud d'entr\u00e9e ne peut \u00eatre vide pour un DOMSource de newTemplates !"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Fichier de classe introuvable pour l''option {0}"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "El\u00e9ment requis introuvable : {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream ne doit pas \u00eatre vide"},
				new object[] {ER_URI_CANNOT_BE_NULL, "L'URI ne doit pas \u00eatre vide"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "Le fichier ne doit pas \u00eatre vide"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource ne doit pas \u00eatre vide"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Impossible d'initialiser le gestionnaire de BSF"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Impossible de compiler l'extension"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Impossible de cr\u00e9er l''extension : {0} en raison de : {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "L''appel de la m\u00e9thode d''instance de la m\u00e9thode {0} requiert une instance d''Object comme premier argument"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Nom d''\u00e9l\u00e9ment non valide sp\u00e9cifi\u00e9 {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "La m\u00e9thode de nom d''\u00e9l\u00e9ment doit \u00eatre statique {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "La fonction d''extension {0} : {1} est inconnue"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Plusieurs occurrences proches pour le constructeur de {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Plusieurs occurrences proches pour la m\u00e9thode {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Plusieurs occurrences proches pour la m\u00e9thode d''\u00e9l\u00e9ment {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Contexte non valide transmis pour \u00e9valuer {0}"},
				new object[] {ER_POOL_EXISTS, "Pool d\u00e9j\u00e0 existant"},
				new object[] {ER_NO_DRIVER_NAME, "Aucun nom de p\u00e9riph\u00e9rique indiqu\u00e9"},
				new object[] {ER_NO_URL, "Aucune URL sp\u00e9cifi\u00e9e"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "La taille du pool est inf\u00e9rieure \u00e0 1 !"},
				new object[] {ER_INVALID_DRIVER, "Nom de pilote non valide sp\u00e9cifi\u00e9 !"},
				new object[] {ER_NO_STYLESHEETROOT, "Impossible de trouver la racine de la feuille de style !"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Valeur incorrecte pour xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "Echec de processFromNode"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "La ressource [ {0} ] n''a pas pu charger : {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Taille du tampon <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Erreur inconnue lors de l'appel de l'extension"},
				new object[] {ER_NO_NAMESPACE_DECL, "Le pr\u00e9fixe {0} ne poss\u00e8de pas de d\u00e9claration d''espace de noms correspondante"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Contenu d''\u00e9l\u00e9ment interdit pour lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "La feuille de style a provoqu\u00e9 l'arr\u00eat"},
				new object[] {ER_ONE_OR_TWO, "1 ou 2"},
				new object[] {ER_TWO_OR_THREE, "2 ou 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Impossible de charger {0} (v\u00e9rifier CLASSPATH), les valeurs par d\u00e9faut sont donc employ\u00e9es"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Impossible d'initialiser les mod\u00e8les par d\u00e9faut"},
				new object[] {ER_RESULT_NULL, "Le r\u00e9sultat doit \u00eatre vide"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Le r\u00e9sultat ne peut \u00eatre d\u00e9fini"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Aucune sortie sp\u00e9cifi\u00e9e"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Transformation impossible vers un r\u00e9sultat de type {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Impossible de transformer une source de type {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Gestionnaire de contenu vide"},
				new object[] {ER_NULL_ERROR_HANDLER, "Gestionnaire d'erreurs vide"},
				new object[] {ER_CANNOT_CALL_PARSE, "L'analyse ne peut \u00eatre appel\u00e9e si le ContentHandler n'a pas \u00e9t\u00e9 d\u00e9fini"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Pas de parent pour le filtre"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Aucune feuille de style dans : {0}, support = {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "Pas de PI xml-stylesheet dans : {0}"},
				new object[] {ER_NOT_SUPPORTED, "Non pris en charge : {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "La valeur de la propri\u00e9t\u00e9 {0} doit \u00eatre une instance bool\u00e9enne"},
				new object[] {ER_COULD_NOT_FIND_EXTERN_SCRIPT, "Impossible d''extraire le script externe \u00e0 {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "La ressource [ {0} ] est introuvable.\n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Propri\u00e9t\u00e9 de sortie non identifi\u00e9e : {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Impossible de cr\u00e9er une instance de ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "La valeur de {0} doit contenir un nombre analysable"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "La valeur de {0} doit \u00eatre oui ou non"},
				new object[] {ER_FAILED_CALLING_METHOD, "Echec de l''appel de la m\u00e9thode {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Echec de la cr\u00e9ation de l'instance de ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "La pr\u00e9sence de caract\u00e8res n'est pas admise \u00e0 cet endroit du document"},
				new object[] {ER_ATTR_NOT_ALLOWED, "L''attribut \"{0}\" n''est pas admis sur l''\u00e9l\u00e9ment {1} !"},
				new object[] {ER_BAD_VALUE, "{0} valeur erron\u00e9e {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Impossible de trouver la valeur de l''attribut {0} "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Valeur de l''attribut {0} non identifi\u00e9e "},
				new object[] {ER_NULL_URI_NAMESPACE, "Tentative de cr\u00e9ation d'un pr\u00e9fixe d'espace de noms avec un URI vide"},
				new object[] {ER_NUMBER_TOO_BIG, "Tentative de formatage d'un nombre sup\u00e9rieur \u00e0 l'entier Long le plus \u00e9lev\u00e9"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Impossible de trouver la classe {0} du pilote SAX1"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "Classe {0} du pilote SAX1 trouv\u00e9e mais non charg\u00e9e"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "Classe {0} du pilote SAX1 trouv\u00e9e mais non instanci\u00e9e"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "La classe {0} du pilote SAX1 n''impl\u00e9mente pas org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Propri\u00e9t\u00e9 syst\u00e8me org.xml.sax.parser non sp\u00e9cifi\u00e9e"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "L'argument de l'analyseur ne doit pas \u00eatre vide"},
				new object[] {ER_FEATURE, "Fonction : {0}"},
				new object[] {ER_PROPERTY, "Propri\u00e9t\u00e9 : {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Solveur d'entit\u00e9 vide"},
				new object[] {ER_NULL_DTD_HANDLER, "Gestionnaire de DT vide"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Aucun nom de pilote sp\u00e9cifi\u00e9 !"},
				new object[] {ER_NO_URL_SPECIFIED, "Aucune URL sp\u00e9cifi\u00e9e !"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "La taille du pool est inf\u00e9rieure \u00e0 1 !"},
				new object[] {ER_INVALID_DRIVER_NAME, "Nom de pilote non valide sp\u00e9cifi\u00e9 !"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Erreur de programme ! L'expression n'a pas de parent ElemTemplateElement !"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Assertion du programmeur dans RedundentExprEliminator : {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} ne peut pas figurer \u00e0 cette position dans la feuille de style !"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Seul de l'espace est accept\u00e9 \u00e0 cette position dans la feuille de style !"},
				new object[] {INVALID_TCHAR, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut CHAR : {0}. Un attribut de type CHAR ne peut comporter qu''un seul caract\u00e8re !"},
				new object[] {INVALID_QNAME, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut QNAME : {0}"},
				new object[] {INVALID_ENUM, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut ENUM : {0}. Les valeurs autoris\u00e9es sont : {2}."},
				new object[] {INVALID_NMTOKEN, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut NMTOKEN : {0}. "},
				new object[] {INVALID_NCNAME, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut NCNAME : {0}. "},
				new object[] {INVALID_BOOLEAN, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut bool\u00e9en : {0}. "},
				new object[] {INVALID_NUMBER, "Valeur incorrecte : {1} utilis\u00e9e pour l''attribut number : {0}. "},
				new object[] {ER_ARG_LITERAL, "L''argument de {0} dans le motif de correspondance doit \u00eatre un litt\u00e9ral."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "D\u00e9claration de variable globale en double."},
				new object[] {ER_DUPLICATE_VAR, "D\u00e9claration de variable en double."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl:template doit comporter un attribut name et/ou match"},
				new object[] {ER_INVALID_PREFIX, "Pr\u00e9fixe de exclude-result-prefixes non valide : {0}"},
				new object[] {ER_NO_ATTRIB_SET, "attribute-set {0} n''existe pas"},
				new object[] {ER_FUNCTION_NOT_FOUND, "La fonction {0} n''existe pas"},
				new object[] {ER_CANT_HAVE_CONTENT_AND_SELECT, "L''\u00e9l\u00e9ment {0} ne peut poss\u00e9der un attribut content et un attribut select."},
				new object[] {ER_INVALID_SET_PARAM_VALUE, "La valeur du param\u00e8tre {0} doit \u00eatre un objet Java valide"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX_FOR_DEFAULT, "L'attribut result-prefix d'un \u00e9l\u00e9ment xsl:namespace-alias a la valeur '#default', mais il n'existe aucune d\u00e9claration de l'espace de noms par d\u00e9faut dans la port\u00e9e de l'\u00e9l\u00e9ment"},
				new object[] {ER_INVALID_NAMESPACE_URI_VALUE_FOR_RESULT_PREFIX, "L''attribut result-prefix d''un \u00e9l\u00e9ment xsl:namespace-alias a la valeur ''{0}'', mais il n''existe aucune d\u00e9claration d''espace de noms pour le pr\u00e9fixe ''{0}'' dans la port\u00e9e de l''\u00e9l\u00e9ment."},
				new object[] {ER_SET_FEATURE_NULL_NAME, "Le nom de la fonction ne peut pas avoir une valeur null dans TransformerFactory.setFeature (nom cha\u00eene, valeur bool\u00e9nne)."},
				new object[] {ER_GET_FEATURE_NULL_NAME, "Le nom de la fonction ne peut pas avoir une valeur null dans TransformerFactory.getFeature (nom cha\u00eene)."},
				new object[] {ER_UNSUPPORTED_FEATURE, "Impossible de d\u00e9finir la fonction ''{0}'' sur cet \u00e9l\u00e9ment TransformerFactory."},
				new object[] {ER_EXTENSION_ELEMENT_NOT_ALLOWED_IN_SECURE_PROCESSING, "L''utilisation de l''\u00e9l\u00e9ment d''extension ''{0}'' n''est pas admise lorsque la fonction de traitement s\u00e9curis\u00e9e a la valeur true."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_NAMESPACE, "Impossible d'obtenir le pr\u00e9fixe pour un uri d'espace de noms null."},
				new object[] {ER_NAMESPACE_CONTEXT_NULL_PREFIX, "Impossible d'obtenir l'uri d'espace de noms pour le pr\u00e9fixe null."},
				new object[] {ER_XPATH_RESOLVER_NULL_QNAME, "Le nom de la fonction ne peut pas avoir une valeur null."},
				new object[] {ER_XPATH_RESOLVER_NEGATIVE_ARITY, "La parit\u00e9 ne peut pas \u00eatre n\u00e9gative."},
				new object[] {WG_FOUND_CURLYBRACE, "Une accolade ('}') a \u00e9t\u00e9 trouv\u00e9e alors qu'aucun mod\u00e8le d'attribut n'est ouvert !"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Avertissement : L''attribut de count n''a pas d''ascendant dans xsl:number ! Cible = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Syntaxe obsol\u00e8te : Le nom de l'attribut 'expr' a \u00e9t\u00e9 remplac\u00e9 par 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan ne g\u00e8re pas encore le nom d'environnement local de la fonction format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Avertissement : Impossible de trouver un environnement local pour xml:lang={0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Impossible de cr\u00e9er l''URL \u00e0 partir de : {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Impossible de charger le document demand\u00e9 : {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Impossible de trouver une fonction de regroupement pour <sort xml:lang= {0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Syntaxe obsol\u00e8te : L''instruction de fonction doit utiliser une URL {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "encodage non pris en charge : {0}, en utilisant UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "encodage non pris en charge : {0}, en utilisant Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Conflits de sp\u00e9cificit\u00e9s trouv\u00e9s : {0} La derni\u00e8re de la feuille de style sera employ\u00e9e."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Analyse et pr\u00e9paration de {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Mod\u00e8le d''attribut, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Conflit de correspondances entre xsl:strip-space et xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan ne g\u00e8re pas encore l''attribut {0} !"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Pas de d\u00e9claration pour le format d\u00e9cimal : {0}"},
				new object[] {WG_OLD_XSLT_NS, "Espace de noms XSLT manquant ou incorrect. "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Une seule d\u00e9claration xsl:decimal-format par d\u00e9faut est admise."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "Les noms xsl:decimal-format doivent \u00eatre uniques. Le nom \"{0}\" a \u00e9t\u00e9 dupliqu\u00e9."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} comporte un attribut incorrect : {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Impossible de convertir le pr\u00e9fixe de l''espace de noms : {0}. Le noeud n''est pas trait\u00e9."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet requiert un attribut 'version' !"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Nom d''attribut incorrect : {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Valeur incorrecte pour l''attribut {0} : {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "L'ensemble de noeuds r\u00e9sultant du second argument de la fonction du document est vide. Un ensemble de noeuds vide est retourn\u00e9."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "La valeur de l'attribut 'name' de xsl:processing-instruction doit \u00eatre diff\u00e9rente de 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "La valeur de l''attribut ''name'' de xsl:processing-instruction doit \u00eatre un nom NCName valide : {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Ajout impossible de l''attribut {0} apr\u00e8s des noeuds enfants ou avant la production d''un \u00e9l\u00e9ment. L''attribut est ignor\u00e9."},
				new object[] {NO_MODIFICATION_ALLOWED_ERR, "Tentative de modification d'un objet qui n'accepte pas les modifications."},
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"BAD_CODE", "Le param\u00e8tre de createMessage se trouve hors limites"},
				new object[] {"FORMAT_FAILED", "Exception soulev\u00e9e lors de l'appel de messageFormat"},
				new object[] {"version", ">>>>>>> Version de Xalan "},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "oui"},
				new object[] {"line", "Ligne #"},
				new object[] {"column", "Colonne #"},
				new object[] {"xsldone", "XSLProcessor : termin\u00e9"},
				new object[] {"xslProc_option", "Options de classe Process de ligne de commande Xalan-J :"},
				new object[] {"xslProc_option", "Options de classe Process de ligne de commande Xalan-J\u003a"},
				new object[] {"xslProc_invalid_xsltc_option", "L''option {0} n''est pas prise en charge en mode XSLTC."},
				new object[] {"xslProc_invalid_xalan_option", "L''option {0} s''utilise uniquement avec -XSLTC."},
				new object[] {"xslProc_no_input", "Erreur : Aucun xml de feuille de style ou d'entr\u00e9e n'est sp\u00e9cifi\u00e9. Ex\u00e9cutez cette commande sans option pour les instructions d'utilisation."},
				new object[] {"xslProc_common_options", "-Options courantes-"},
				new object[] {"xslProc_xalan_options", "-Options pour Xalan-"},
				new object[] {"xslProc_xsltc_options", "-Options pour XSLTC-"},
				new object[] {"xslProc_return_to_continue", "(appuyez sur <Retour> pour continuer)"},
				new object[] {"optionXSLTC", "   [-XSLTC (utilisez XSLTC pour la transformation)]"},
				new object[] {"optionIN", "   [-IN inputXMLURL]"},
				new object[] {"optionXSL", "   [-XSL URLXSLTransformation]"},
				new object[] {"optionOUT", "   [-OUT nomFichierSortie]"},
				new object[] {"optionLXCIN", "   [-LXCIN NomFichierFeuilleDeStylesCompil\u00e9Entr\u00e9e]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT NomFichierFeuilleDeStylesCompil\u00e9Sortie]"},
				new object[] {"optionPARSER", "   [-PARSER nom de classe pleinement qualifi\u00e9 pour la liaison de l'analyseur]"},
				new object[] {"optionE", "   [-E (Ne pas d\u00e9velopper les r\u00e9f. d'entit\u00e9)]"},
				new object[] {"optionV", "   [-E (Ne pas d\u00e9velopper les r\u00e9f. d'entit\u00e9)]"},
				new object[] {"optionQC", "   [-QC (Avertissements brefs de conflits de motifs)]"},
				new object[] {"optionQ", "   [-Q  (Mode bref)]"},
				new object[] {"optionLF", "   [-LF (Utilise des sauts de ligne uniquement dans la sortie {CR/LF par d\u00e9faut})]"},
				new object[] {"optionCR", "   [-LF (Utilise des retours chariot uniquement dans la sortie {CR/LF par d\u00e9faut})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Caract\u00e8res d'\u00e9chappement {<>&\"\'\\r\\n par d\u00e9faut}]"},
				new object[] {"optionINDENT", "   [-INDENT (Nombre d'espaces pour le retrait {par d\u00e9faut 0})]"},
				new object[] {"optionTT", "   [-TT (Contr\u00f4le les appels de mod\u00e8les - fonction de trace.)]"},
				new object[] {"optionTG", "   [-TG (Contr\u00f4le chaque \u00e9v\u00e9nement de g\u00e9n\u00e9ration - fonction de trace.)]"},
				new object[] {"optionTS", "   [-TS (Contr\u00f4le chaque \u00e9v\u00e9nement de s\u00e9lection - fonction de trace.)]"},
				new object[] {"optionTTC", "   [-TTC (Contr\u00f4le les enfants du mod\u00e8le lors de leur traitement - fonction de trace.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (Classe TraceListener pour les extensions de trace.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (Indique si la validation se produit. La validation est d\u00e9sactiv\u00e9e par d\u00e9faut.)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {nom de fichier optionnel} (Cr\u00e9e un vidage de pile en cas d'erreur.)]"},
				new object[] {"optionXML", "   [-XML (Utilise un formateur XML et ajoute un en-t\u00eate XML.)]"},
				new object[] {"optionTEXT", "   [-TEXT (Utilise un formateur de texte simple.)]"},
				new object[] {"optionHTML", "   [-HTML (Utilise un formateur HTML.)]"},
				new object[] {"optionPARAM", "   [-PARAM nom expression (D\u00e9finit un param\u00e8tre de feuille de style)]"},
				new object[] {"noParsermsg1", "Echec du processus XSL."},
				new object[] {"noParsermsg2", "** Analyseur introuvable **"},
				new object[] {"noParsermsg3", "V\u00e9rifiez le chemin d'acc\u00e8s des classes."},
				new object[] {"noParsermsg4", "XML Parser for Java disponible en t\u00e9l\u00e9chargement sur le site"},
				new object[] {"noParsermsg5", "AlphaWorks de IBM : http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER nom de classe complet (Les URI sont r\u00e9solus par URIResolver)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER nom de classe complet (Les URI sont r\u00e9solus par EntityResolver)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTHANDLER nom de classe complet (La s\u00e9rialisation de la sortie est effectu\u00e9e par ContentHandler)]"},
				new object[] {"optionLINENUMBERS", "   [-L utilisation des num\u00e9ros de ligne pour le document source]"},
				new object[] {"optionSECUREPROCESSING", "   [-SECURE (attribuez la valeur true \u00e0 la fonction de traitement s\u00e9curis\u00e9.)]"},
				new object[] {"optionMEDIA", "   [-MEDIA type_de_support (Utilise un attribut de support pour trouver la feuille de style associ\u00e9e \u00e0 un document.)]"},
				new object[] {"optionFLAVOR", "   [-FLAVOR sax_ou_dom (effectue la transformation \u00e0 l'aide de SAX (s2s) ou de DOM (d2d).)] "},
				new object[] {"optionDIAG", "   [-DIAG (affiche la dur\u00e9e globale de la transformation - en millisecondes.)]"},
				new object[] {"optionINCREMENTAL", "   [-INCREMENTAL (construction incr\u00e9mentielle du DTM en d\u00e9finissant http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", "   [-NOOPTIMIMIZE (pas de traitement d'optimisation des feuilles de style en d\u00e9finissant http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", "   [-RL r\u00e9cursivit\u00e9_maxi (limite de la profondeur de la r\u00e9cursivit\u00e9 pour les feuilles de style.)]"},
				new object[] {"optionXO", "   [-XO [nom_translet] (assignation du nom au translet g\u00e9n\u00e9r\u00e9)]"},
				new object[] {"optionXD", "   [-XD r\u00e9pertoire_cible (sp\u00e9cification d'un r\u00e9pertoire de destination pour translet)]"},
				new object[] {"optionXJ", "   [-XJ fichier_jar (r\u00e9union des classes translet dans un fichier jar appel\u00e9 <fichier_jar>)]"},
				new object[] {"optionXP", "   [-XP module (sp\u00e9cification d'un pr\u00e9fixe de nom de module pour toutes les classes translet g\u00e9n\u00e9r\u00e9es)]"},
				new object[] {"optionXN", "   [-XN (activation de la mise en ligne de mod\u00e8le)]"},
				new object[] {"optionXX", "   [-XX (activation du d\u00e9bogage suppl\u00e9mentaire de sortie de message)]"},
				new object[] {"optionXT", "   [-XT (utilisation de translet pour la transformation si possible)]"},
				new object[] {"diagTiming", " --------- La transformation de {0} via {1} a pris {2} ms"},
				new object[] {"recursionTooDeep", "Trop grande imbrication de mod\u00e8le. imbrication = {0}, mod\u00e8le {1} {2}"},
				new object[] {"nameIs", "le nom est"},
				new object[] {"matchPatternIs", "le motif de correspondance est"}
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
	  public const string ERROR_HEADER = "Erreur : ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public const string WARNING_HEADER = "Avertissement : ";

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