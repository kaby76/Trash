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
 * $Id: XMLErrorResources_de.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources_de : ListResourceBundle
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
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Funktion wird nicht unterst\u00fctzt!"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "cause kann nicht \u00fcberschrieben werden."},
				new object[] {ER_NO_DEFAULT_IMPL, "Keine Standardimplementierung gefunden. "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "ChunkedIntArray({0}) wird derzeit nicht unterst\u00fctzt."},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "Offset ist gr\u00f6\u00dfer als der Bereich."},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Koroutine nicht verf\u00fcgbar, ID: {0}."},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager hat Anforderung co_exit() empfangen."},
				new object[] {ER_COJOINROUTINESET_FAILED, "co_joinCoroutineSet() ist fehlgeschlagen."},
				new object[] {ER_COROUTINE_PARAM, "Parameterfehler der Koroutine ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nUNERWARTET: Parser doTerminate antwortet {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "parse darf w\u00e4hrend der Syntaxanalyse nicht aufgerufen werden."},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Fehler: Iterator mit Typangabe f\u00fcr Achse {0} ist nicht implementiert."},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Fehler: Iterator f\u00fcr Achse {0} ist nicht implementiert. "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Iterator 'clone' wird nicht unterst\u00fctzt."},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Unbekannter Achsentraversiertyp: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Achsentraversierer wird nicht unterst\u00fctzt: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "Keine weiteren Dokumenttypmodell-IDs verf\u00fcgbar"},
				new object[] {ER_NOT_SUPPORTED, "Nicht unterst\u00fctzt: {0}"},
				new object[] {ER_NODE_NON_NULL, "Knoten muss ungleich Null sein f\u00fcr getDTMHandleFromNode."},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "Der Knoten konnte nicht in eine Kennung aufgel\u00f6st werden."},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "startParse kann w\u00e4hrend der Syntaxanalyse nicht aufgerufen werden."},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "startParse erfordert SAXParser ungleich Null."},
				new object[] {ER_COULD_NOT_INIT_PARSER, "Konnte Parser nicht initialisieren mit"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "Ausnahmebedingung beim Erstellen einer neuen Instanz f\u00fcr Pool."},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Der Pfad enth\u00e4lt eine ung\u00fcltige Escapezeichenfolge."},
				new object[] {ER_SCHEME_REQUIRED, "Schema ist erforderlich!"},
				new object[] {ER_NO_SCHEME_IN_URI, "Kein Schema gefunden in URI: {0}."},
				new object[] {ER_NO_SCHEME_INURI, "Kein Schema gefunden in URI"},
				new object[] {ER_PATH_INVALID_CHAR, "Pfad enth\u00e4lt ung\u00fcltiges Zeichen: {0}."},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Schema kann nicht von Nullzeichenfolge festgelegt werden."},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "Das Schema ist nicht angepasst."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "Der Host ist keine syntaktisch korrekte Adresse."},
				new object[] {ER_PORT_WHEN_HOST_NULL, "Der Port kann nicht festgelegt werden, wenn der Host gleich Null ist."},
				new object[] {ER_INVALID_PORT, "Ung\u00fcltige Portnummer"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Fragment kann nur f\u00fcr eine generische URI (Uniform Resource Identifier) festgelegt werden."},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "Fragment kann nicht festgelegt werden, wenn der Pfad gleich Null ist."},
				new object[] {ER_FRAG_INVALID_CHAR, "Fragment enth\u00e4lt ein ung\u00fcltiges Zeichen."},
				new object[] {ER_PARSER_IN_USE, "Der Parser wird bereits verwendet."},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "{0} {1} kann w\u00e4hrend der Syntaxanalyse nicht ge\u00e4ndert werden."},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "Selbstverursachung ist nicht zul\u00e4ssig."},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Benutzerinformationen k\u00f6nnen nicht angegeben werden, wenn der Host nicht angegeben wurde."},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Der Port kann nicht angegeben werden, wenn der Host nicht angegeben wurde."},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "Abfragezeichenfolge kann nicht im Pfad und in der Abfragezeichenfolge angegeben werden."},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Fragment kann nicht im Pfad und im Fragment angegeben werden."},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "URI (Uniform Resource Identifier) kann nicht mit leeren Parametern initialisiert werden."},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Die Methode wird noch nicht unterst\u00fctzt. "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "IncrementalSAXSource_Filter ist momentan nicht wieder anlauff\u00e4hig."},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "XMLReader nicht vor Anforderung startParse"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Achsentraversierer nicht unterst\u00fctzt: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "ListingErrorHandler erstellt ohne Druckausgabeprogramm!"},
				new object[] {ER_SYSTEMID_UNKNOWN, "System-ID unbekannt"},
				new object[] {ER_LOCATION_UNKNOWN, "Position des Fehlers unbekannt"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Das Pr\u00e4fix muss in einen Namensbereich aufgel\u00f6st werden: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "createDocument() wird nicht in XPathContext unterst\u00fctzt!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "Das Attribut child weist kein Eignerdokument auf!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "Das Attribut child weist kein Eignerdokumentelement auf!"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Warnung: Vor dem Dokumentelement kann kein Text ausgegeben werden!  Wird ignoriert..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "Mehr als ein Root f\u00fcr ein Dokumentobjektmodell ist nicht m\u00f6glich!"},
				new object[] {ER_ARG_LOCALNAME_NULL, "Das Argument 'localName' ist Null."},
				new object[] {ER_ARG_LOCALNAME_INVALID, "Der lokale Name (Localname) in QNAME muss ein g\u00fcltiger NCName sein."},
				new object[] {ER_ARG_PREFIX_INVALID, "Das Pr\u00e4fix in QNAME muss ein g\u00fcltiger NCName sein."},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "Name darf nicht mit einem Doppelpunkt beginnen."},
				new object[] {"BAD_CODE", "Der Parameter f\u00fcr createMessage lag au\u00dferhalb des g\u00fcltigen Bereichs"},
				new object[] {"FORMAT_FAILED", "W\u00e4hrend des Aufrufs von messageFormat wurde eine Ausnahmebedingung ausgel\u00f6st"},
				new object[] {"line", "Zeilennummer"},
				new object[] {"column","Spaltennummer"}
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