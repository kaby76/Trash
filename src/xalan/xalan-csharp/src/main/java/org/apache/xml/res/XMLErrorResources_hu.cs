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
 * $Id: XMLErrorResources_hu.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources_hu : ListResourceBundle
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
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "A f\u00fcggv\u00e9ny nem t\u00e1mogatott."},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "Nem lehet fel\u00fcl\u00edrni az okot"},
				new object[] {ER_NO_DEFAULT_IMPL, "Nem tal\u00e1lhat\u00f3 alap\u00e9rtelmezett megval\u00f3s\u00edt\u00e1s "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "A ChunkedIntArray({0}) jelenleg nem t\u00e1mogatott"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "Az eltol\u00e1s nagyobb mint a ny\u00edl\u00e1s"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "T\u00e1rs szubrutin nem \u00e9rhet\u0151 el, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager \u00e9rkezett a co_exit() k\u00e9r\u00e9sre"},
				new object[] {ER_COJOINROUTINESET_FAILED, "A co_joinCoroutineSet() nem siker\u00fclt "},
				new object[] {ER_COROUTINE_PARAM, "T\u00e1rs szubrutin param\u00e9terhiba ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nV\u00c1RATLAN: \u00c9rtelmez\u0151 doTerminate v\u00e1laszok {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "\u00e9rtelmez\u00e9s nem h\u00edvhat\u00f3 meg \u00e9rtelmez\u00e9s k\u00f6zben "},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Hiba: A tipiz\u00e1lt iter\u00e1tor a(z) {0} tengelyhez nincs megval\u00f3s\u00edtva"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Hiba: Az iter\u00e1tor a(z) {0} tengelyhez nincs megval\u00f3s\u00edtva "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Az iter\u00e1tor kl\u00f3noz\u00e1sa nem t\u00e1mogatott"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Ismeretlen tengely bej\u00e1r\u00e1si \u00fat t\u00edpus: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "A tengely bej\u00e1r\u00e1si \u00fat nem t\u00e1mogatott: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "Nincs t\u00f6bb DTM azonos\u00edt\u00f3"},
				new object[] {ER_NOT_SUPPORTED, "Nem t\u00e1mogatott: {0}"},
				new object[] {ER_NODE_NON_NULL, "A csom\u00f3pont nem lehet null a getDTMHandleFromNode f\u00fcggv\u00e9nyhez"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "A csom\u00f3pontot nem lehet azonos\u00edt\u00f3ra feloldani"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "A startParse f\u00fcggv\u00e9nyt nem h\u00edvhatja meg \u00e9rtelmez\u00e9s k\u00f6zben"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "A startParse f\u00fcggv\u00e9nyhez nemnull SAXParser sz\u00fcks\u00e9ges"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "Nem lehet inicializ\u00e1lni az \u00e9rtelmez\u0151t ezzel"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "kiv\u00e9tel egy \u00faj t\u00e1rol\u00f3p\u00e9ld\u00e1ny l\u00e9trehoz\u00e1sakor"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Az el\u00e9r\u00e9si \u00fat \u00e9rv\u00e9nytelen vez\u00e9rl\u0151 jelsorozatot tartalmaz"},
				new object[] {ER_SCHEME_REQUIRED, "S\u00e9ma sz\u00fcks\u00e9ges."},
				new object[] {ER_NO_SCHEME_IN_URI, "Nem tal\u00e1lhat\u00f3 s\u00e9ma az URI-ban: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "Nem tal\u00e1lhat\u00f3 s\u00e9ma az URI-ban"},
				new object[] {ER_PATH_INVALID_CHAR, "Az el\u00e9r\u00e9si \u00fat \u00e9rv\u00e9nytelen karaktert tartalmaz: {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Nem lehet be\u00e1ll\u00edtani a s\u00e9m\u00e1t null karaktersorozatb\u00f3l"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "A s\u00e9ma nem megfelel\u0151."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "A hoszt nem j\u00f3l form\u00e1zott c\u00edm"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "A portot nem \u00e1ll\u00edthatja be, ha a hoszt null"},
				new object[] {ER_INVALID_PORT, "\u00c9rv\u00e9nytelen portsz\u00e1m"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Csak \u00e1ltal\u00e1nos URI-hoz \u00e1ll\u00edthat be t\u00f6red\u00e9ket "},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "A t\u00f6red\u00e9ket nem \u00e1ll\u00edthatja be, ha az el\u00e9r\u00e9si \u00fat null"},
				new object[] {ER_FRAG_INVALID_CHAR, "A t\u00f6red\u00e9k \u00e9rv\u00e9nytelen karaktert tartalmaz"},
				new object[] {ER_PARSER_IN_USE, "Az \u00e9rtelmez\u0151 m\u00e1r haszn\u00e1latban van"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "Nem v\u00e1ltoztathat\u00f3 meg a(z) {0} {1} \u00e9rtelmez\u00e9s k\u00f6zben"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "Az \u00f6n-megokol\u00e1s nem megengedett"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Nem adhatja meg a felhaszn\u00e1l\u00f3i inform\u00e1ci\u00f3kat, ha nincs megadva hoszt"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Nem adhatja meg a portot, ha nincs megadva hoszt"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "Nem adhat meg lek\u00e9rdez\u00e9si karaktersorozatot az el\u00e9r\u00e9si \u00fatban \u00e9s a lek\u00e9rdez\u00e9si karaktersorozatban"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Nem adhat meg t\u00f6red\u00e9ket az el\u00e9r\u00e9si \u00fatban \u00e9s a t\u00f6red\u00e9kben is"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "Az URI nem inicializ\u00e1lhat\u00f3 \u00fcres param\u00e9terekkel"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "A met\u00f3dus m\u00e9g nem t\u00e1mogatott "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "Az IncrementalSAXSource_Filter jelenleg nem \u00ednd\u00edthat\u00f3 \u00fajra"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "Az XMLReader nem a startParse k\u00e9r\u00e9s el\u0151tt van "},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "A tengely bej\u00e1r\u00e1si \u00fat nem t\u00e1mogatott: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "A ListingErrorHandler null PrintWriter \u00e9rt\u00e9kkel j\u00f6tt l\u00e9tre."},
				new object[] {ER_SYSTEMID_UNKNOWN, "Ismeretlen SystemId"},
				new object[] {ER_LOCATION_UNKNOWN, "A hiba helye ismeretlen"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Az el\u0151tagnak egy n\u00e9vt\u00e9rre kell felold\u00f3dnia: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "A createDocument() nem t\u00e1mogatott az XPathContext-ben."},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "Az attrib\u00fatum ut\u00f3dnak nincs tulajdonos dokumentuma."},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "Az attrib\u00fatum ut\u00f3dnak nincs tulajdonos dokumentumeleme."},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Figyelmeztet\u00e9s: nem lehet sz\u00f6veget ki\u00edrni dokumentum elem el\u0151tt. Figyelmen k\u00edv\u00fcl marad..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "Nem lehet egyn\u00e9l t\u00f6bb gy\u00f6k\u00e9r a DOM-ban"},
				new object[] {ER_ARG_LOCALNAME_NULL, "A 'localName' argumentum null"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "A QNAME-beli helyi n\u00e9vnek egy \u00e9rv\u00e9nyes NCName-nek kell lennie"},
				new object[] {ER_ARG_PREFIX_INVALID, "A QNAME-beli el\u0151tagnak \u00e9rv\u00e9nyes NCName-nek kell lennie"},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "A n\u00e9v nem kezd\u0151dhet kett\u0151sponttal"},
				new object[] {"BAD_CODE", "A createMessage egyik param\u00e9tere nincs a megfelel\u0151 tartom\u00e1nyban"},
				new object[] {"FORMAT_FAILED", "Kiv\u00e9tel t\u00f6rt\u00e9nt a messageFormat h\u00edv\u00e1sa k\u00f6zben"},
				new object[] {"line", "Sor #"},
				new object[] {"column", "Oszlop #"}
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
			return (XMLErrorResources) ResourceBundle.getBundle(className, new Locale("hu", "HU"));
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