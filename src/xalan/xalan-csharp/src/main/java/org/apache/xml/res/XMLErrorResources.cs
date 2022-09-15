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
 * $Id: XMLErrorResources.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources : ListResourceBundle
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
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Function not supported!"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "Cannot overwrite cause"},
				new object[] {ER_NO_DEFAULT_IMPL, "No default implementation found "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "ChunkedIntArray({0}) not currently supported"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "Offset bigger than slot"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Coroutine not available, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager received co_exit() request"},
				new object[] {ER_COJOINROUTINESET_FAILED, "co_joinCoroutineSet() failed"},
				new object[] {ER_COROUTINE_PARAM, "Coroutine parameter error ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nUNEXPECTED: Parser doTerminate answers {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "parse may not be called while parsing"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Error: typed iterator for axis  {0} not implemented"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Error: iterator for axis {0} not implemented "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Iterator clone not supported"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Unknown axis traversal type: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Axis traverser not supported: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "No more DTM IDs are available"},
				new object[] {ER_NOT_SUPPORTED, "Not supported: {0}"},
				new object[] {ER_NODE_NON_NULL, "Node must be non-null for getDTMHandleFromNode"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "Could not resolve the node to a handle"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "startParse may not be called while parsing"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "startParse needs a non-null SAXParser"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "could not initialize parser with"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "exception creating new instance for pool"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Path contains invalid escape sequence"},
				new object[] {ER_SCHEME_REQUIRED, "Scheme is required!"},
				new object[] {ER_NO_SCHEME_IN_URI, "No scheme found in URI: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "No scheme found in URI"},
				new object[] {ER_PATH_INVALID_CHAR, "Path contains invalid character: {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Cannot set scheme from null string"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "The scheme is not conformant."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "Host is not a well formed address"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "Port cannot be set when host is null"},
				new object[] {ER_INVALID_PORT, "Invalid port number"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Fragment can only be set for a generic URI"},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "Fragment cannot be set when path is null"},
				new object[] {ER_FRAG_INVALID_CHAR, "Fragment contains invalid character"},
				new object[] {ER_PARSER_IN_USE, "Parser is already in use"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "Cannot change {0} {1} while parsing"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "Self-causation not permitted"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Userinfo may not be specified if host is not specified"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Port may not be specified if host is not specified"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "Query string cannot be specified in path and query string"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Fragment cannot be specified in both the path and fragment"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "Cannot initialize URI with empty parameters"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Method not yet supported "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "IncrementalSAXSource_Filter not currently restartable"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "XMLReader not before startParse request"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Axis traverser not supported: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "ListingErrorHandler created with null PrintWriter!"},
				new object[] {ER_SYSTEMID_UNKNOWN, "SystemId Unknown"},
				new object[] {ER_LOCATION_UNKNOWN, "Location of error unknown"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Prefix must resolve to a namespace: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "createDocument() not supported in XPathContext!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "Attribute child does not have an owner document!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "Attribute child does not have an owner document element!"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Warning: can't output text before document element!  Ignoring..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "Can't have more than one root on a DOM!"},
				new object[] {ER_ARG_LOCALNAME_NULL, "Argument 'localName' is null"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "Localname in QNAME should be a valid NCName"},
				new object[] {ER_ARG_PREFIX_INVALID, "Prefix in QNAME should be a valid NCName"},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "Name cannot start with a colon"},
				new object[] {"BAD_CODE", "Parameter to createMessage was out of bounds"},
				new object[] {"FORMAT_FAILED", "Exception thrown during messageFormat call"},
				new object[] {"line", "Line #"},
				new object[] {"column", "Column #"}
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static final XMLErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XMLErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.getDefault();
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