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
 * $Id: XMLErrorResources_fr.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources_fr : ListResourceBundle
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
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Fonction non prise en charge !"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "Impossible de remplacer la cause"},
				new object[] {ER_NO_DEFAULT_IMPL, "Impossible de trouver une impl\u00e9mentation par d\u00e9faut "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "ChunkedIntArray({0}) n''est pas pris en charge"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "D\u00e9calage plus important que l'emplacement"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Coroutine non disponible, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager a re\u00e7u une demande de co_exit()"},
				new object[] {ER_COJOINROUTINESET_FAILED, "Echec de co_joinCoroutineSet()"},
				new object[] {ER_COROUTINE_PARAM, "Erreur de param\u00e8tre de Coroutine ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nRESULTAT INATTENDU : L''analyseur doTerminate r\u00e9pond {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "parse ne peut \u00eatre appel\u00e9 lors de l'analyse"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Erreur : it\u00e9rateur typ\u00e9 de l''axe {0} non impl\u00e9ment\u00e9"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Erreur : it\u00e9rateur de l''axe {0} non impl\u00e9ment\u00e9 "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Clone de l'it\u00e9rateur non pris en charge"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Type transversal d''axe inconnu : {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Traverseur d''axe non pris en charge : {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "Aucun autre ID de DTM disponible"},
				new object[] {ER_NOT_SUPPORTED, "Non pris en charge : {0}"},
				new object[] {ER_NODE_NON_NULL, "Le noeud ne doit pas \u00eatre vide pour getDTMHandleFromNode"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "Impossible de convertir le noeud en pointeur"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "startParse ne peut \u00eatre appel\u00e9 pendant l'analyse"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "startParse requiert un SAXParser non vide"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "impossible d'initialiser l'analyseur"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "exception durant la cr\u00e9ation d'une instance du pool"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Le chemin d'acc\u00e8s contient une s\u00e9quence d'\u00e9chappement non valide"},
				new object[] {ER_SCHEME_REQUIRED, "Processus requis !"},
				new object[] {ER_NO_SCHEME_IN_URI, "Processus introuvable dans l''URI : {0}"},
				new object[] {ER_NO_SCHEME_INURI, "Processus introuvable dans l'URI"},
				new object[] {ER_PATH_INVALID_CHAR, "Le chemin contient un caract\u00e8re non valide : {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Impossible de d\u00e9finir le processus \u00e0 partir de la cha\u00eene vide"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "Le processus n'est pas conforme."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "L'h\u00f4te n'est pas une adresse bien form\u00e9e"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "Le port ne peut \u00eatre d\u00e9fini quand l'h\u00f4te est vide"},
				new object[] {ER_INVALID_PORT, "Num\u00e9ro de port non valide"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Le fragment ne peut \u00eatre d\u00e9fini que pour un URI g\u00e9n\u00e9rique"},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "Le fragment ne peut \u00eatre d\u00e9fini quand le chemin d'acc\u00e8s est vide"},
				new object[] {ER_FRAG_INVALID_CHAR, "Le fragment contient un caract\u00e8re non valide"},
				new object[] {ER_PARSER_IN_USE, "L'analyseur est d\u00e9j\u00e0 utilis\u00e9"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "Impossible de modifier {0} {1} durant l''analyse"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "Auto-causalit\u00e9 interdite"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Userinfo ne peut \u00eatre sp\u00e9cifi\u00e9 si l'h\u00f4te ne l'est pas"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Le port peut ne pas \u00eatre sp\u00e9cifi\u00e9 si l'h\u00f4te n'est pas sp\u00e9cifi\u00e9"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "La cha\u00eene de requ\u00eate ne doit pas figurer dans un chemin et une cha\u00eene de requ\u00eate"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Le fragment ne doit pas \u00eatre indiqu\u00e9 \u00e0 la fois dans le chemin et dans le fragment"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "Impossible d'initialiser l'URI avec des param\u00e8tres vides"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Cette m\u00e9thode n'est pas encore prise en charge "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "IncrementalSAXSource_Filter ne peut red\u00e9marrer"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "XMLReader ne figure pas avant la demande startParse"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Traverseur d''axe non pris en charge : {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "ListingErrorHandler cr\u00e9\u00e9 avec PrintWriter vide !"},
				new object[] {ER_SYSTEMID_UNKNOWN, "ID syst\u00e8me inconnu"},
				new object[] {ER_LOCATION_UNKNOWN, "Emplacement inconnu de l'erreur"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Le pr\u00e9fixe doit se convertir en espace de noms : {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "createDocument() non pris en charge dans XPathContext !"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "L'enfant de l'attribut ne poss\u00e8de pas de document propri\u00e9taire !"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "Le contexte ne poss\u00e8de pas d'\u00e9l\u00e9ment de document propri\u00e9taire !"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Avertissement : impossible d'afficher du texte avant l'\u00e9l\u00e9ment de document !  Traitement ignor\u00e9..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "Un DOM ne peut poss\u00e9der plusieurs racines !"},
				new object[] {ER_ARG_LOCALNAME_NULL, "L'argument 'localName' est vide"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "Dans QNAME, le nom local doit \u00eatre un nom NCName valide"},
				new object[] {ER_ARG_PREFIX_INVALID, "Dans QNAME, le pr\u00e9fixe doit \u00eatre un nom NCName valide"},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "Un nom ne peut commencer par le signe deux-points"},
				new object[] {"BAD_CODE", "Le param\u00e8tre de createMessage se trouve hors limites"},
				new object[] {"FORMAT_FAILED", "Exception soulev\u00e9e lors de l'appel de messageFormat"},
				new object[] {"line", "Ligne #"},
				new object[] {"column","Colonne #"}
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