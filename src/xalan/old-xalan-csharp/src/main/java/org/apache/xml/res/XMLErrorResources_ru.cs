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
 * $Id: XMLErrorResources_ru.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.res
{



	/// <summary>
	/// Set up error messages.
	/// We build a two dimensional array of message keys and
	/// message strings. In order to add a new message here,
	/// you need to first add a String constant. And you need
	/// to enter key, value pair as part of the contents
	/// array. You also need to update MAX_CODE for error strings
	/// and MAX_WARNING for warnings ( Needed for only information
	/// purpose )
	/// </summary>
	public class XMLErrorResources_ru : ListResourceBundle
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
	   * Message keys
	   */
	  public const string ER_FUNCTION_NOT_SUPPORTED = "ER_FUNCTION_NOT_SUPPORTED";
	  public const string ER_CANNOT_OVERWRITE_CAUSE = "ER_CANNOT_OVERWRITE_CAUSE";
	  public const string ER_NO_DEFAULT_IMPL = "ER_NO_DEFAULT_IMPL";
	  public const string ER_CHUNKEDINTARRAY_NOT_SUPPORTED = "ER_CHUNKEDINTARRAY_NOT_SUPPORTED";
	  public const string ER_OFFSET_BIGGER_THAN_SLOT = "ER_OFFSET_BIGGER_THAN_SLOT";
	  public const string ER_COROUTINE_NOT_AVAIL = "ER_COROUTINE_NOT_AVAIL";
	  public const string ER_COROUTINE_CO_EXIT = "ER_COROUTINE_CO_EXIT";
	  public const string ER_COJOINROUTINESET_FAILED = "ER_COJOINROUTINESET_FAILED";
	  public const string ER_COROUTINE_PARAM = "ER_COROUTINE_PARAM";
	  public const string ER_PARSER_DOTERMINATE_ANSWERS = "ER_PARSER_DOTERMINATE_ANSWERS";
	  public const string ER_NO_PARSE_CALL_WHILE_PARSING = "ER_NO_PARSE_CALL_WHILE_PARSING";
	  public const string ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED = "ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED";
	  public const string ER_ITERATOR_AXIS_NOT_IMPLEMENTED = "ER_ITERATOR_AXIS_NOT_IMPLEMENTED";
	  public const string ER_ITERATOR_CLONE_NOT_SUPPORTED = "ER_ITERATOR_CLONE_NOT_SUPPORTED";
	  public const string ER_UNKNOWN_AXIS_TYPE = "ER_UNKNOWN_AXIS_TYPE";
	  public const string ER_AXIS_NOT_SUPPORTED = "ER_AXIS_NOT_SUPPORTED";
	  public const string ER_NO_DTMIDS_AVAIL = "ER_NO_DTMIDS_AVAIL";
	  public const string ER_NOT_SUPPORTED = "ER_NOT_SUPPORTED";
	  public const string ER_NODE_NON_NULL = "ER_NODE_NON_NULL";
	  public const string ER_COULD_NOT_RESOLVE_NODE = "ER_COULD_NOT_RESOLVE_NODE";
	  public const string ER_STARTPARSE_WHILE_PARSING = "ER_STARTPARSE_WHILE_PARSING";
	  public const string ER_STARTPARSE_NEEDS_SAXPARSER = "ER_STARTPARSE_NEEDS_SAXPARSER";
	  public const string ER_COULD_NOT_INIT_PARSER = "ER_COULD_NOT_INIT_PARSER";
	  public const string ER_EXCEPTION_CREATING_POOL = "ER_EXCEPTION_CREATING_POOL";
	  public const string ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE = "ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE";
	  public const string ER_SCHEME_REQUIRED = "ER_SCHEME_REQUIRED";
	  public const string ER_NO_SCHEME_IN_URI = "ER_NO_SCHEME_IN_URI";
	  public const string ER_NO_SCHEME_INURI = "ER_NO_SCHEME_INURI";
	  public const string ER_PATH_INVALID_CHAR = "ER_PATH_INVALID_CHAR";
	  public const string ER_SCHEME_FROM_NULL_STRING = "ER_SCHEME_FROM_NULL_STRING";
	  public const string ER_SCHEME_NOT_CONFORMANT = "ER_SCHEME_NOT_CONFORMANT";
	  public const string ER_HOST_ADDRESS_NOT_WELLFORMED = "ER_HOST_ADDRESS_NOT_WELLFORMED";
	  public const string ER_PORT_WHEN_HOST_NULL = "ER_PORT_WHEN_HOST_NULL";
	  public const string ER_INVALID_PORT = "ER_INVALID_PORT";
	  public const string ER_FRAG_FOR_GENERIC_URI = "ER_FRAG_FOR_GENERIC_URI";
	  public const string ER_FRAG_WHEN_PATH_NULL = "ER_FRAG_WHEN_PATH_NULL";
	  public const string ER_FRAG_INVALID_CHAR = "ER_FRAG_INVALID_CHAR";
	  public const string ER_PARSER_IN_USE = "ER_PARSER_IN_USE";
	  public const string ER_CANNOT_CHANGE_WHILE_PARSING = "ER_CANNOT_CHANGE_WHILE_PARSING";
	  public const string ER_SELF_CAUSATION_NOT_PERMITTED = "ER_SELF_CAUSATION_NOT_PERMITTED";
	  public const string ER_NO_USERINFO_IF_NO_HOST = "ER_NO_USERINFO_IF_NO_HOST";
	  public const string ER_NO_PORT_IF_NO_HOST = "ER_NO_PORT_IF_NO_HOST";
	  public const string ER_NO_QUERY_STRING_IN_PATH = "ER_NO_QUERY_STRING_IN_PATH";
	  public const string ER_NO_FRAGMENT_STRING_IN_PATH = "ER_NO_FRAGMENT_STRING_IN_PATH";
	  public const string ER_CANNOT_INIT_URI_EMPTY_PARMS = "ER_CANNOT_INIT_URI_EMPTY_PARMS";
	  public const string ER_METHOD_NOT_SUPPORTED = "ER_METHOD_NOT_SUPPORTED";
	  public const string ER_INCRSAXSRCFILTER_NOT_RESTARTABLE = "ER_INCRSAXSRCFILTER_NOT_RESTARTABLE";
	  public const string ER_XMLRDR_NOT_BEFORE_STARTPARSE = "ER_XMLRDR_NOT_BEFORE_STARTPARSE";
	  public const string ER_AXIS_TRAVERSER_NOT_SUPPORTED = "ER_AXIS_TRAVERSER_NOT_SUPPORTED";
	  public const string ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER = "ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER";
	  public const string ER_SYSTEMID_UNKNOWN = "ER_SYSTEMID_UNKNOWN";
	  public const string ER_LOCATION_UNKNOWN = "ER_LOCATION_UNKNOWN";
	  public const string ER_PREFIX_MUST_RESOLVE = "ER_PREFIX_MUST_RESOLVE";
	  public const string ER_CREATEDOCUMENT_NOT_SUPPORTED = "ER_CREATEDOCUMENT_NOT_SUPPORTED";
	  public const string ER_CHILD_HAS_NO_OWNER_DOCUMENT = "ER_CHILD_HAS_NO_OWNER_DOCUMENT";
	  public const string ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT = "ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT";
	  public const string ER_CANT_OUTPUT_TEXT_BEFORE_DOC = "ER_CANT_OUTPUT_TEXT_BEFORE_DOC";
	  public const string ER_CANT_HAVE_MORE_THAN_ONE_ROOT = "ER_CANT_HAVE_MORE_THAN_ONE_ROOT";
	  public const string ER_ARG_LOCALNAME_NULL = "ER_ARG_LOCALNAME_NULL";
	  public const string ER_ARG_LOCALNAME_INVALID = "ER_ARG_LOCALNAME_INVALID";
	  public const string ER_ARG_PREFIX_INVALID = "ER_ARG_PREFIX_INVALID";
	  public const string ER_NAME_CANT_START_WITH_COLON = "ER_NAME_CANT_START_WITH_COLON";

	  /*
	   * Now fill in the message text.
	   * Then fill in the message text for that message code in the
	   * array. Use the new error code as the index into the array.
	   */

	  // Error messages...

	  /// <summary>
	  /// Get the lookup table for error messages
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public virtual object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ER0000", "{0}"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "\u0424\u0443\u043d\u043a\u0446\u0438\u044f \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f!"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043f\u0435\u0440\u0435\u0437\u0430\u043f\u0438\u0441\u0430\u0442\u044c \u043f\u0440\u0438\u0447\u0438\u043d\u0443"},
				new object[] {ER_NO_DEFAULT_IMPL, "\u0420\u0435\u0430\u043b\u0438\u0437\u0430\u0446\u0438\u044f \u043f\u043e \u0443\u043c\u043e\u043b\u0447\u0430\u043d\u0438\u044e \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430 "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "ChunkedIntArray({0}) \u0432 \u043d\u0430\u0441\u0442\u043e\u044f\u0449\u0435\u0435 \u0432\u0440\u0435\u043c\u044f \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "\u0421\u043c\u0435\u0449\u0435\u043d\u0438\u0435 \u0431\u043e\u043b\u044c\u0448\u0435 \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d\u0430"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Coroutine \u043d\u0435\u0434\u043e\u0441\u0442\u0443\u043f\u043d\u0430, \u0418\u0414={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager \u043f\u043e\u043b\u0443\u0447\u0438\u043b \u0437\u0430\u043f\u0440\u043e\u0441 co_exit()"},
				new object[] {ER_COJOINROUTINESET_FAILED, "\u041e\u0448\u0438\u0431\u043a\u0430 co_joinCoroutineSet()"},
				new object[] {ER_COROUTINE_PARAM, "\u041e\u0448\u0438\u0431\u043a\u0430 \u043f\u0430\u0440\u0430\u043c\u0435\u0442\u0440\u0430 Coroutine ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\n\u041d\u0435\u043f\u0440\u0435\u0434\u0432\u0438\u0434\u0435\u043d\u043d\u0430\u044f \u043e\u0448\u0438\u0431\u043a\u0430: \u041e\u0442\u0432\u0435\u0442 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440\u0430 doTerminate: {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "\u041d\u0435\u043b\u044c\u0437\u044f \u0432\u044b\u0437\u044b\u0432\u0430\u0442\u044c \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 \u0432\u043e \u0432\u0440\u0435\u043c\u044f \u0430\u043d\u0430\u043b\u0438\u0437\u0430"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "\u041e\u0448\u0438\u0431\u043a\u0430: \u0442\u0438\u043f\u0438\u0437\u0438\u0440\u043e\u0432\u0430\u043d\u043d\u044b\u0439 \u0438\u0442\u0435\u0440\u0430\u0442\u043e\u0440 \u0434\u043b\u044f \u043e\u0441\u0438 {0} \u043d\u0435 \u0440\u0435\u0430\u043b\u0438\u0437\u043e\u0432\u0430\u043d"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "\u041e\u0448\u0438\u0431\u043a\u0430: \u043d\u0435 \u0440\u0435\u0430\u043b\u0438\u0437\u043e\u0432\u0430\u043d \u0441\u0447\u0435\u0442\u0447\u0438\u043a \u0434\u043b\u044f \u043e\u0441\u0438 {0} "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "\u041a\u043e\u043f\u0438\u044f \u0438\u0442\u0435\u0440\u0430\u0442\u043e\u0440\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u0442\u0438\u043f Traverser \u0434\u043b\u044f \u043e\u0441\u0438: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Traverser \u0434\u043b\u044f \u043e\u0441\u0438 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "\u041d\u0435\u0442 \u0434\u043e\u0441\u0442\u0443\u043f\u043d\u044b\u0445 \u0418\u0414 DTM"},
				new object[] {ER_NOT_SUPPORTED, "\u041d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}"},
				new object[] {ER_NODE_NON_NULL, "\u0414\u043b\u044f getDTMHandleFromNode \u0443\u0437\u0435\u043b \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u043d\u0435\u043f\u0443\u0441\u0442\u044b\u043c"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u0442\u044c \u0443\u0437\u0435\u043b \u0432 \u0434\u0435\u0441\u043a\u0440\u0438\u043f\u0442\u043e\u0440"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "\u041d\u0435\u043b\u044c\u0437\u044f \u0432\u044b\u0437\u044b\u0432\u0430\u0442\u044c startParse \u0432\u043e \u0432\u0440\u0435\u043c\u044f \u0430\u043d\u0430\u043b\u0438\u0437\u0430"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "\u0414\u043b\u044f startParse \u043d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c \u043d\u0435\u043f\u0443\u0441\u0442\u043e\u0439 SAXParser"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "\u041d\u0435 \u0443\u0434\u0430\u043b\u043e\u0441\u044c \u0438\u043d\u0438\u0446\u0438\u0430\u043b\u0438\u0437\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 \u0441"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "\u0418\u0441\u043a\u043b\u044e\u0447\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0438\u0442\u0443\u0430\u0446\u0438\u044f \u043f\u0440\u0438 \u0441\u043e\u0437\u0434\u0430\u043d\u0438\u0438 \u043d\u043e\u0432\u043e\u0433\u043e \u044d\u043a\u0437\u0435\u043c\u043f\u043b\u044f\u0440\u0430 \u043f\u0443\u043b\u0430"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "\u0412 \u0438\u043c\u0435\u043d\u0438 \u043f\u0443\u0442\u0438 \u0432\u0441\u0442\u0440\u0435\u0447\u0430\u0435\u0442\u0441\u044f \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u0430\u044f Esc-\u043f\u043e\u0441\u043b\u0435\u0434\u043e\u0432\u0430\u0442\u0435\u043b\u044c\u043d\u043e\u0441\u0442\u044c"},
				new object[] {ER_SCHEME_REQUIRED, "\u041d\u0435\u043e\u0431\u0445\u043e\u0434\u0438\u043c\u0430 \u0441\u0445\u0435\u043c\u0430!"},
				new object[] {ER_NO_SCHEME_IN_URI, "\u0412 URI \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430 \u0441\u0445\u0435\u043c\u0430: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "\u0412 URI \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430 \u0441\u0445\u0435\u043c\u0430"},
				new object[] {ER_PATH_INVALID_CHAR, "\u0412 \u0438\u043c\u0435\u043d\u0438 \u043f\u0443\u0442\u0438 \u043e\u0431\u043d\u0430\u0440\u0443\u0436\u0435\u043d \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0441\u0438\u043c\u0432\u043e\u043b: {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u0441\u0445\u0435\u043c\u0443 \u0434\u043b\u044f \u043f\u0443\u0441\u0442\u043e\u0439 \u0441\u0442\u0440\u043e\u043a\u0438"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "\u0421\u0445\u0435\u043c\u0430 \u043d\u0435 \u043a\u043e\u043d\u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0432\u043d\u0430."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "\u041d\u0435\u043f\u0440\u0430\u0432\u0438\u043b\u044c\u043d\u043e \u0441\u0444\u043e\u0440\u043c\u0438\u0440\u043e\u0432\u0430\u043d \u0430\u0434\u0440\u0435\u0441 \u0445\u043e\u0441\u0442\u0430"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u043f\u043e\u0440\u0442 \u0434\u043b\u044f \u043f\u0443\u0441\u0442\u043e\u0433\u043e \u0430\u0434\u0440\u0435\u0441\u0430 \u0445\u043e\u0441\u0442\u0430"},
				new object[] {ER_INVALID_PORT, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u043d\u043e\u043c\u0435\u0440 \u043f\u043e\u0440\u0442\u0430"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "\u0424\u0440\u0430\u0433\u043c\u0435\u043d\u0442 \u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u0442\u043e\u043b\u044c\u043a\u043e \u0434\u043b\u044f \u0448\u0430\u0431\u043b\u043e\u043d\u0430 URI"},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u0444\u0440\u0430\u0433\u043c\u0435\u043d\u0442 \u0434\u043b\u044f \u043f\u0443\u0441\u0442\u043e\u0433\u043e \u043f\u0443\u0442\u0438"},
				new object[] {ER_FRAG_INVALID_CHAR, "\u0424\u0440\u0430\u0433\u043c\u0435\u043d\u0442 \u0441\u043e\u0434\u0435\u0440\u0436\u0438\u0442 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0441\u0438\u043c\u0432\u043e\u043b"},
				new object[] {ER_PARSER_IN_USE, "\u0410\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 \u0443\u0436\u0435 \u0438\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0435\u0442\u0441\u044f"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0438\u0437\u043c\u0435\u043d\u0438\u0442\u044c {0} {1} \u0432\u043e \u0432\u0440\u0435\u043c\u044f \u0430\u043d\u0430\u043b\u0438\u0437\u0430"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "\u0421\u0430\u043c\u043e\u043f\u0440\u0438\u0441\u0432\u043e\u0435\u043d\u0438\u0435 \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "\u041d\u0435\u043b\u044c\u0437\u044f \u0443\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u0438\u043d\u0444\u043e\u0440\u043c\u0430\u0446\u0438\u044e \u043e \u043f\u043e\u043b\u044c\u0437\u043e\u0432\u0430\u0442\u0435\u043b\u0435, \u0435\u0441\u043b\u0438 \u043d\u0435 \u0437\u0430\u0434\u0430\u043d \u0445\u043e\u0441\u0442"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "\u041d\u0435\u043b\u044c\u0437\u044f \u0443\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u043f\u043e\u0440\u0442, \u0435\u0441\u043b\u0438 \u043d\u0435 \u0437\u0430\u0434\u0430\u043d \u0445\u043e\u0441\u0442"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "\u041d\u0435\u043b\u044c\u0437\u044f \u0443\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u0441\u0442\u0440\u043e\u043a\u0443 \u0437\u0430\u043f\u0440\u043e\u0441\u0430 \u0432 \u0441\u0442\u0440\u043e\u043a\u0435 \u043f\u0443\u0442\u0438 \u0438 \u0437\u0430\u043f\u0440\u043e\u0441\u0430"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0437\u0430\u0434\u0430\u0442\u044c \u0444\u0440\u0430\u0433\u043c\u0435\u043d\u0442 \u043e\u0434\u043d\u043e\u0432\u0440\u0435\u043c\u0435\u043d\u043d\u043e \u0434\u043b\u044f \u043f\u0443\u0442\u0438 \u0438 \u0444\u0440\u0430\u0433\u043c\u0435\u043d\u0442\u0430"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0438\u043d\u0438\u0446\u0438\u0430\u043b\u0438\u0437\u0438\u0440\u043e\u0432\u0430\u0442\u044c URI \u0441 \u043f\u0443\u0441\u0442\u044b\u043c\u0438 \u043f\u0430\u0440\u0430\u043c\u0435\u0442\u0440\u0430\u043c\u0438"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "\u041c\u0435\u0442\u043e\u0434 \u043f\u043e\u043a\u0430 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "\u041f\u0435\u0440\u0435\u0437\u0430\u043f\u0443\u0441\u043a IncrementalSAXSource_Filter \u0432 \u043d\u0430\u0441\u0442\u043e\u044f\u0449\u0435\u0435 \u0432\u0440\u0435\u043c\u044f \u043d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u0435\u043d"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "\u041d\u0435\u043b\u044c\u0437\u044f \u043f\u0440\u0438\u043c\u0435\u043d\u044f\u0442\u044c XMLReader \u0434\u043e startParse"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Traverser \u0434\u043b\u044f \u043e\u0441\u0438 \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "ListingErrorHandler \u0441\u043e\u0437\u0434\u0430\u043d \u0441 \u043f\u0443\u0441\u0442\u044b\u043c PrintWriter!"},
				new object[] {ER_SYSTEMID_UNKNOWN, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u0418\u0414 \u0441\u0438\u0441\u0442\u0435\u043c\u044b"},
				new object[] {ER_LOCATION_UNKNOWN, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u043e\u0435 \u0440\u0430\u0441\u043f\u043e\u043b\u043e\u0436\u0435\u043d\u0438\u0435 \u0438\u043b\u0438 \u043e\u0448\u0438\u0431\u043a\u0430"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u041f\u0440\u0435\u0444\u0438\u043a\u0441 \u0434\u043e\u043b\u0436\u0435\u043d \u043e\u0431\u0435\u0441\u043f\u0435\u0447\u0438\u0432\u0430\u0442\u044c \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435 \u0432 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u043e \u0438\u043c\u0435\u043d: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "createDocument() \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f XPathContext!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "\u0423 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 child \u043d\u0435\u0442 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430-\u0432\u043b\u0430\u0434\u0435\u043b\u044c\u0446\u0430!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "\u0423 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0430 child \u043d\u0435\u0442 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430-\u0432\u043b\u0430\u0434\u0435\u043b\u044c\u0446\u0430!"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "\u041f\u0440\u0435\u0434\u0443\u043f\u0440\u0435\u0436\u0434\u0435\u043d\u0438\u0435: \u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0432\u044b\u0432\u0435\u0441\u0442\u0438 \u0442\u0435\u043a\u0441\u0442 \u043f\u0435\u0440\u0435\u0434 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u043e\u043c \u0434\u043e\u043a\u0443\u043c\u0435\u043d\u0442\u0430!  \u041f\u0440\u043e\u0438\u0433\u043d\u043e\u0440\u0438\u0440\u043e\u0432\u0430\u043d..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "\u0412 DOM \u043c\u043e\u0436\u0435\u0442 \u0431\u044b\u0442\u044c \u0442\u043e\u043b\u044c\u043a\u043e \u043e\u0434\u0438\u043d \u043a\u043e\u0440\u043d\u0435\u0432\u043e\u0439 \u044d\u043b\u0435\u043c\u0435\u043d\u0442!"},
				new object[] {ER_ARG_LOCALNAME_NULL, "\u041f\u0443\u0441\u0442\u043e\u0439 \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442 'localName'"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "\u041b\u043e\u043a\u0430\u043b\u044c\u043d\u043e\u0435 \u0438\u043c\u044f \u0432 QNAME \u0434\u043e\u043b\u0436\u043d\u043e \u0431\u044b\u0442\u044c \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c NCName"},
				new object[] {ER_ARG_PREFIX_INVALID, "\u041f\u0440\u0435\u0444\u0438\u043a\u0441 \u0432 QNAME \u0434\u043e\u043b\u0436\u0435\u043d \u0431\u044b\u0442\u044c \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u043c \u0438\u043c\u0435\u043d\u0435\u043c NCName"},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "\u0418\u043c\u044f \u043d\u0435 \u043c\u043e\u0436\u0435\u0442 \u043d\u0430\u0447\u0438\u043d\u0430\u0442\u044c\u0441\u044f \u0441 \u0434\u0432\u043e\u0435\u0442\u043e\u0447\u0438\u044f"},
				new object[] {"BAD_CODE", "\u041f\u0430\u0440\u0430\u043c\u0435\u0442\u0440 createMessage \u043b\u0435\u0436\u0438\u0442 \u0432\u043d\u0435 \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0433\u043e \u0434\u0438\u0430\u043f\u0430\u0437\u043e\u043d\u0430"},
				new object[] {"FORMAT_FAILED", "\u0418\u0441\u043a\u043b\u044e\u0447\u0438\u0442\u0435\u043b\u044c\u043d\u0430\u044f \u0441\u0438\u0442\u0443\u0430\u0446\u0438\u044f \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 messageFormat"},
				new object[] {"line", "\u041d\u043e\u043c\u0435\u0440 \u0441\u0442\u0440\u043e\u043a\u0438 "},
				new object[] {"column","\u041d\u043e\u043c\u0435\u0440 \u0441\u0442\u043e\u043b\u0431\u0446\u0430 "}
			};
		  }
	  }

	  /// <summary>
	  ///   Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  ///   of ResourceBundle.getBundle().
	  /// </summary>
	  ///   <param name="className"> the name of the class that implements the resource bundle. </param>
	  ///   <returns> the ResourceBundle </returns>
	  ///   <exception cref="MissingResourceException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static final XMLErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XMLErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.Default;
		string suffix = getResourceSuffix(locale);

		try
		{

		  // first try with the given locale
		  return (XMLErrorResources) ResourceBundle.getBundle(className + suffix, locale);
		}
		catch (MissingResourceException)
		{
		  try // try to fall back to en_US if we can't load
		  {

			// Since we can't find the localized property file,
			// fall back to en_US.
			return (XMLErrorResources) ResourceBundle.getBundle(className, new Locale("en", "US"));
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