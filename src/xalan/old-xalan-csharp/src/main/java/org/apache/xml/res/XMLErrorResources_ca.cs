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
 * $Id: XMLErrorResources_ca.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources_ca : ListResourceBundle
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
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Aquesta funci\u00f3 no t\u00e9 suport."},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "No es pot sobreescriure una causa"},
				new object[] {ER_NO_DEFAULT_IMPL, "No s'ha trobat cap implementaci\u00f3 per defecte "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "Actualment no es d\u00f3na suport a ChunkedIntArray({0})"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "El despla\u00e7ament \u00e9s m\u00e9s gran que la ranura"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Coroutine no est\u00e0 disponible, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager ha rebut una sol\u00b7licitud co_exit()"},
				new object[] {ER_COJOINROUTINESET_FAILED, "S'ha produ\u00eft un error a co_joinCoroutineSet()"},
				new object[] {ER_COROUTINE_PARAM, "Error de par\u00e0metre coroutine ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nUNEXPECTED: doTerminate de l''analitzador respon {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "L'an\u00e0lisi no es pot cridar mentre s'est\u00e0 duent a terme"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Error: l''iterador de tipus de l''eix {0} no s''ha implementat"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Error: l''iterador de l''eix {0} no s''ha implementat "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "El clonatge de l'iterador no t\u00e9 suport"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Tipus de commutaci\u00f3 de l''eix desconeguda: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "La commutaci\u00f3 de l''eix no t\u00e9 suport: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "No hi ha m\u00e9s ID de DTM disponibles"},
				new object[] {ER_NOT_SUPPORTED, "No t\u00e9 suport: {0}"},
				new object[] {ER_NODE_NON_NULL, "El node no ha de ser nul per a getDTMHandleFromNode"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "No s'ha pogut resoldre el node en un manejador"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "startParse no es pot cridar mentre s'est\u00e0 duent a terme l'an\u00e0lisi"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "startParse necessita un SAXParser que no sigui nul"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "No s'ha pogut inicialitzar l'analitzador amb"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "S'ha produ\u00eft una excepci\u00f3 en crear una nova inst\u00e0ncia de l'agrupaci\u00f3"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "La via d'acc\u00e9s cont\u00e9 una seq\u00fc\u00e8ncia d'escapament no v\u00e0lida"},
				new object[] {ER_SCHEME_REQUIRED, "Es necessita l'esquema"},
				new object[] {ER_NO_SCHEME_IN_URI, "No s''ha trobat cap esquema a l''URI: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "No s'ha trobat cap esquema a l'URI"},
				new object[] {ER_PATH_INVALID_CHAR, "La via d''acc\u00e9s cont\u00e9 un car\u00e0cter no v\u00e0lid {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "No es pot establir un esquema des d'una cadena nul\u00b7la"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "L'esquema no t\u00e9 conformitat."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "El format de l'adre\u00e7a del sistema principal no \u00e9s el correcte"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "El port no es pot establir quan el sistema principal \u00e9s nul"},
				new object[] {ER_INVALID_PORT, "N\u00famero de port no v\u00e0lid"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "El fragment nom\u00e9s es pot establir per a un URI gen\u00e8ric"},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "El fragment no es pot establir si la via d'acc\u00e9s \u00e9s nul\u00b7la"},
				new object[] {ER_FRAG_INVALID_CHAR, "El fragment cont\u00e9 un car\u00e0cter no v\u00e0lid"},
				new object[] {ER_PARSER_IN_USE, "L'analitzador ja s'est\u00e0 utilitzant"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "No es pot modificar {0} {1} mentre es duu a terme l''an\u00e0lisi"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "La causalitat pr\u00f2pia no est\u00e0 permesa."},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "No es pot especificar informaci\u00f3 de l'usuari si no s'especifica el sistema principal"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "No es pot especificar el port si no s'especifica el sistema principal"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "No es pot especificar una cadena de consulta en la via d'acc\u00e9s i la cadena de consulta"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "No es pot especificar un fragment tant en la via d'acc\u00e9s com en el fragment"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "No es pot inicialitzar l'URI amb par\u00e0metres buits"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Aquest m\u00e8tode encara no t\u00e9 suport "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "Ara mateix no es pot reiniciar IncrementalSAXSource_Filter"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "XMLReader no es pot produir abans de la sol\u00b7licitud d'startParse"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "La commutaci\u00f3 de l''eix no t\u00e9 suport: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "S'ha creat ListingErrorHandler amb PrintWriter nul"},
				new object[] {ER_SYSTEMID_UNKNOWN, "ID del sistema (SystemId) desconegut"},
				new object[] {ER_LOCATION_UNKNOWN, "Ubicaci\u00f3 de l'error desconeguda"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "El prefix s''ha de resoldre en un espai de noms: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "createDocument() no t\u00e9 suport a XPathContext"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "El subordinat de l'atribut no t\u00e9 un document de propietari."},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "El subordinat de l'atribut no t\u00e9 un element de document de propietari."},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Av\u00eds: no es pot produir text abans de l'element de document. Es passa per alt."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "No hi pot haver m\u00e9s d'una arrel en un DOM."},
				new object[] {ER_ARG_LOCALNAME_NULL, "L'argument 'localName' \u00e9s nul."},
				new object[] {ER_ARG_LOCALNAME_INVALID, "El nom local de QNAME ha de ser un NCName v\u00e0lid."},
				new object[] {ER_ARG_PREFIX_INVALID, "El prefix de QNAME ha de ser un NCName v\u00e0lid."},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "El nom no pot comen\u00e7ar amb dos punts. "},
				new object[] {"BAD_CODE", "El par\u00e0metre de createMessage estava fora dels l\u00edmits."},
				new object[] {"FORMAT_FAILED", "S'ha generat una excepci\u00f3 durant la crida messageFormat."},
				new object[] {"line", "L\u00ednia n\u00fam."},
				new object[] {"column","Columna n\u00fam."}
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
			return (XMLErrorResources) ResourceBundle.getBundle(className, new Locale("ca", "ES"));
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