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
 * $Id: XMLErrorResources_sv.java 468653 2006-10-28 07:07:05Z minchau $
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
	public class XMLErrorResources_sv : XMLErrorResources
	{

	  /// <summary>
	  /// Maximum error messages, this is needed to keep track of the number of messages. </summary>
	  public const int MAX_CODE = 61;

	  /// <summary>
	  /// Maximum warnings, this is needed to keep track of the number of warnings. </summary>
	  public const int MAX_WARNING = 0;

	  /// <summary>
	  /// Maximum misc strings. </summary>
	  public const int MAX_OTHERS = 4;

	  /// <summary>
	  /// Maximum total warnings and error messages. </summary>
	  public static readonly int MAX_MESSAGES = MAX_CODE + MAX_WARNING + 1;


	  // Error messages...

	  /// <summary>
	  /// Get the lookup table for error messages
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public override object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ER0000", "{0}"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "Funktion inte underst\u00f6dd:"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "Kan inte skriva \u00f6ver orsak"},
				new object[] {ER_NO_DEFAULT_IMPL, "Standardimplementering saknas i:"},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "ChunkedIntArray({0}) underst\u00f6ds f\u00f6r n\u00e4rvarande inte"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "Offset st\u00f6rre \u00e4n fack"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Sidorutin inte tillg\u00e4nglig, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager mottog co_exit()-f\u00f6rfr\u00e5gan"},
				new object[] {ER_COJOINROUTINESET_FAILED, "co_joinCoroutineSet() misslyckades"},
				new object[] {ER_COROUTINE_PARAM, "Sidorutin fick parameterfel ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nOV\u00c4NTAT: Parser doTerminate-svar {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "parse f\u00e5r inte anropas medan tolkning sker"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Fel: typad upprepare f\u00f6r axel {0} inte implementerad"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Fel: upprepare f\u00f6r axel {0} inte implementerad"},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Uppreparklon underst\u00f6ds inte"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Ok\u00e4nd axeltraverstyp: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Axeltravers underst\u00f6ds inte: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "Inga fler DTM-IDs \u00e4r tillg\u00e4ngliga"},
				new object[] {ER_NOT_SUPPORTED, "Underst\u00f6ds inte: {0}"},
				new object[] {ER_NODE_NON_NULL, "Nod m\u00e5ste vara icke-null f\u00f6r getDTMHandleFromNode"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "Kunde inte l\u00f6sa nod till ett handtag"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "startParse f\u00e5r inte anropas medan tolkning sker"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "startParse beh\u00f6ver en SAXParser som \u00e4r icke-null"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "kunde inte initialisera tolk med"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "undantag skapar ny instans f\u00f6r pool"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "V\u00e4g inneh\u00e5ller ogiltig flyktsekvens"},
				new object[] {ER_SCHEME_REQUIRED, "Schema kr\u00e4vs!"},
				new object[] {ER_NO_SCHEME_IN_URI, "Schema saknas i URI: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "Schema saknas i URI"},
				new object[] {ER_PATH_INVALID_CHAR, "V\u00e4g inneh\u00e5ller ogiltigt tecken: {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Kan inte s\u00e4tta schema fr\u00e5n null-str\u00e4ng"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "Schemat \u00e4r inte likformigt."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "V\u00e4rd \u00e4r inte en v\u00e4lformulerad adress"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "Port kan inte s\u00e4ttas n\u00e4r v\u00e4rd \u00e4r null"},
				new object[] {ER_INVALID_PORT, "Ogiltigt portnummer"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Fragment kan bara s\u00e4ttas f\u00f6r en allm\u00e4n URI"},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "Fragment kan inte s\u00e4ttas n\u00e4r v\u00e4g \u00e4r null"},
				new object[] {ER_FRAG_INVALID_CHAR, "Fragment inneh\u00e5ller ogiltigt tecken"},
				new object[] {ER_PARSER_IN_USE, "Tolk anv\u00e4nds redan"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "Kan inte \u00e4ndra {0} {1} medan tolkning sker"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "Sj\u00e4lvorsakande inte till\u00e5ten"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Userinfo f\u00e5r inte anges om v\u00e4rden inte \u00e4r angiven"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Port f\u00e5r inte anges om v\u00e4rden inte \u00e4r angiven"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "F\u00f6rfr\u00e5gan-str\u00e4ng kan inte anges i v\u00e4g och f\u00f6rfr\u00e5gan-str\u00e4ng"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Fragment kan inte anges i b\u00e5de v\u00e4gen och fragmentet"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "Kan inte initialisera URI med tomma parametrar"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Metod \u00e4nnu inte underst\u00f6dd "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "IncrementalSAXSource_Filter kan f\u00f6r n\u00e4rvarande inte startas om"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "XMLReader inte innan startParse-beg\u00e4ran"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Det g\u00e5r inte att v\u00e4nda axeln: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "ListingErrorHandler skapad med null PrintWriter!"},
				new object[] {ER_SYSTEMID_UNKNOWN, "SystemId ok\u00e4nt"},
				new object[] {ER_LOCATION_UNKNOWN, "Platsen f\u00f6r felet \u00e4r ok\u00e4nd"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Prefix must resolve to a namespace: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "createDocument() underst\u00f6ds inte av XPathContext!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "Attributbarn saknar \u00e4gardokument!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "Attributbarn saknar \u00e4gardokumentelement!"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Varning: kan inte skriva ut text innan dokumentelement!  Ignorerar..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "Kan inte ha mer \u00e4n en rot p\u00e5 en DOM!"},
				new object[] {ER_ARG_LOCALNAME_NULL, "Argument 'localName' \u00e4r null"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "Localname i QNAME b\u00f6r vara ett giltigt NCName"},
				new object[] {ER_ARG_PREFIX_INVALID, "Prefixet i QNAME b\u00f6r vara ett giltigt NCName"},
				new object[] {"BAD_CODE", "Parameter till createMessage ligger utanf\u00f6r till\u00e5tet intervall"},
				new object[] {"FORMAT_FAILED", "Undantag utl\u00f6st vid messageFormat-anrop"},
				new object[] {"line", "Rad #"},
				new object[] {"column", "Kolumn #"}
			};
		  }
	  }

	}

}