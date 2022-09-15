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
 * $Id: XMLErrorResources_cs.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources_cs : ListResourceBundle
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
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Nepodporovan\u00e1 funkce!"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "P\u0159\u00ed\u010dinu nelze p\u0159epsat"},
				new object[] {ER_NO_DEFAULT_IMPL, "Nebyla nalezena v\u00fdchoz\u00ed implementace. "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "Funkce ChunkedIntArray({0}) nen\u00ed aktu\u00e1ln\u011b podporov\u00e1na."},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "Offset je v\u011bt\u0161\u00ed ne\u017e slot."},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Spole\u010dn\u00e1 rutina nen\u00ed k dispozici, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "Funkce CoroutineManager obdr\u017eela po\u017eadavek co_exit()"},
				new object[] {ER_COJOINROUTINESET_FAILED, "Selhala funkce co_joinCoroutineSet()"},
				new object[] {ER_COROUTINE_PARAM, "Chyba parametru spole\u010dn\u00e9 rutiny ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nNeo\u010dek\u00e1van\u00e9: odpov\u011bdi funkce analyz\u00e1toru doTerminate {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "b\u011bhem anal\u00fdzy nelze volat analyz\u00e1tor"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Chyba: zadan\u00fd iter\u00e1tor osy {0} nen\u00ed implementov\u00e1n"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Chyba: zadan\u00fd iter\u00e1tor osy {0} nen\u00ed implementov\u00e1n "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Nepodporovan\u00fd klon iter\u00e1toru."},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Nezn\u00e1m\u00fd typ osy pr\u016fchodu: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Nepodporovan\u00e1 osa pr\u016fchodu: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "\u017d\u00e1dn\u00e1 dal\u0161\u00ed ID DTM nejsou k dispozici"},
				new object[] {ER_NOT_SUPPORTED, "Nepodporov\u00e1no: {0}"},
				new object[] {ER_NODE_NON_NULL, "Uzel pou\u017eit\u00fd ve funkci getDTMHandleFromNode mus\u00ed m\u00edt hodnotu not-null"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "Uzel nelze p\u0159elo\u017eit do manipul\u00e1toru"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "B\u011bhem anal\u00fdzy nelze volat funkci startParse."},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "Funkce startParse vy\u017eaduje SAXParser s hodnotou not-null."},
				new object[] {ER_COULD_NOT_INIT_PARSER, "nelze inicializovat analyz\u00e1tor s:"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "v\u00fdjimka p\u0159i vytv\u00e1\u0159en\u00ed nov\u00e9 instance spole\u010dn\u00e9 oblasti"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Cesta obsahuje neplatnou escape sekvenci"},
				new object[] {ER_SCHEME_REQUIRED, "Je vy\u017eadov\u00e1no sch\u00e9ma!"},
				new object[] {ER_NO_SCHEME_IN_URI, "V URI nebylo nalezeno \u017e\u00e1dn\u00e9 sch\u00e9ma: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "V URI nebylo nalezeno \u017e\u00e1dn\u00e9 sch\u00e9ma"},
				new object[] {ER_PATH_INVALID_CHAR, "Cesta obsahuje neplatn\u00fd znak: {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Nelze nastavit sch\u00e9ma \u0159et\u011bzce s hodnotou null."},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "Sch\u00e9ma nevyhovuje."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "Adresa hostitele m\u00e1 nespr\u00e1vn\u00fd form\u00e1t."},
				new object[] {ER_PORT_WHEN_HOST_NULL, "M\u00e1-li hostitel hodnotu null, nelze nastavit port."},
				new object[] {ER_INVALID_PORT, "Neplatn\u00e9 \u010d\u00edslo portu."},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Fragment lze nastavit jen u generick\u00e9ho URI."},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "M\u00e1-li cesta hodnotu null, nelze nastavit fragment."},
				new object[] {ER_FRAG_INVALID_CHAR, "Fragment obsahuje neplatn\u00fd znak."},
				new object[] {ER_PARSER_IN_USE, "Analyz\u00e1tor se ji\u017e pou\u017e\u00edv\u00e1."},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "B\u011bhem anal\u00fdzy nelze m\u011bnit {0} {1}."},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "Zp\u016fsoben\u00ed sama sebe (self-causation) nen\u00ed povoleno"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Nen\u00ed-li ur\u010den hostitel, nelze zadat \u00fadaje o u\u017eivateli."},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Nen\u00ed-li ur\u010den hostitel, nelze zadat port."},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "V \u0159et\u011bzci cesty a dotazu nelze zadat \u0159et\u011bzec dotazu."},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Fragment nelze ur\u010dit z\u00e1rove\u0148 v cest\u011b i ve fragmentu."},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "URI nelze inicializovat s pr\u00e1zdn\u00fdmi parametry."},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Prozat\u00edm nepodporovan\u00e1 metoda. "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "Filtr IncrementalSAXSource_Filter nelze aktu\u00e1ln\u011b znovu spustit."},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "P\u0159ed po\u017eadavkem startParse nen\u00ed XMLReader."},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Nepodporovan\u00e1 osa pr\u016fchodu: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "Prvek ListingErrorHandler byl vytvo\u0159en s funkc\u00ed PrintWriter s hodnotou null!"},
				new object[] {ER_SYSTEMID_UNKNOWN, "Nezn\u00e1m\u00fd identifik\u00e1tor SystemId"},
				new object[] {ER_LOCATION_UNKNOWN, "Chyba se vyskytla na nezn\u00e1m\u00e9m m\u00edst\u011b"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "P\u0159edponu mus\u00ed b\u00fdt mo\u017eno p\u0159elo\u017eit do oboru n\u00e1zv\u016f: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "Funkce XPathContext nepodporuje funkci createDocument()!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "Potomek atributu nem\u00e1 dokument vlastn\u00edka!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "Potomek atributu nem\u00e1 prvek dokumentu vlastn\u00edka!"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Varov\u00e1n\u00ed: v\u00fdstup textu nem\u016f\u017ee p\u0159edch\u00e1zet prvku dokumentu!  Ignorov\u00e1no..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "DOM nem\u016f\u017ee m\u00edt n\u011bkolik ko\u0159en\u016f!"},
				new object[] {ER_ARG_LOCALNAME_NULL, "Argument 'localName' m\u00e1 hodnotu null"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "Hodnota Localname ve funkci QNAME by m\u011bla b\u00fdt platn\u00fdm prvkem NCName"},
				new object[] {ER_ARG_PREFIX_INVALID, "P\u0159edpona ve funkci QNAME by m\u011bla b\u00fdt platn\u00fdm prvkem NCName"},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "N\u00e1zev nesm\u00ed za\u010d\u00ednat dvojte\u010dkou"},
				new object[] {"BAD_CODE", "Parametr funkce createMessage je mimo limit"},
				new object[] {"FORMAT_FAILED", "P\u0159i vol\u00e1n\u00ed funkce messageFormat do\u0161lo k v\u00fdjimce"},
				new object[] {"line", "\u0158\u00e1dek #"},
				new object[] {"column", "Sloupec #"}
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
			return (XMLErrorResources) ResourceBundle.getBundle(className, new Locale("cs", "CZ"));
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