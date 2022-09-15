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
 * $Id: XPATHErrorResources_sv.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.res
{
	/// <summary>
	/// Set up error messages.
	/// We build a two dimensional array of message keys and
	/// message strings. In order to add a new message here,
	/// you need to first add a Static string constant for the
	/// Key and update the contents array with Key, Value pair
	/// Also you need to  update the count of messages(MAX_CODE)or
	/// the count of warnings(MAX_WARNING) [ Information purpose only]
	/// @xsl.usage advanced
	/// </summary>
	public class XPATHErrorResources_sv : XPATHErrorResources
	{


	  /// <summary>
	  /// Field MAX_CODE </summary>
	public const int MAX_CODE = 108; // this is needed to keep track of the number of messages

	  /// <summary>
	  /// Field MAX_WARNING </summary>
	  public const int MAX_WARNING = 11; // this is needed to keep track of the number of warnings

	  /// <summary>
	  /// Field MAX_OTHERS </summary>
	  public const int MAX_OTHERS = 20;

	  /// <summary>
	  /// Field MAX_MESSAGES </summary>
	  public static readonly int MAX_MESSAGES = MAX_CODE + MAX_WARNING + 1;


	  // Error messages...
	  /// <summary>
	  /// Get the association list.
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public override object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ERROR0000", "{0}"},
				new object[] {ER_CURRENT_NOT_ALLOWED_IN_MATCH, "Funktionen current() \u00e4r inte till\u00e5ten i ett matchningsm\u00f6nster!"},
				new object[] {ER_CURRENT_TAKES_NO_ARGS, "Funktionen current() tar inte emot argument!"},
				new object[] {ER_DOCUMENT_REPLACED, "Implementeringen av funktionen document() har ersatts av org.apache.xalan.xslt.FuncDocument!"},
				new object[] {ER_CONTEXT_HAS_NO_OWNERDOC, "Kontext saknar \u00e4gardokument!"},
				new object[] {ER_LOCALNAME_HAS_TOO_MANY_ARGS, "local-name() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_NAMESPACEURI_HAS_TOO_MANY_ARGS, "namespace-uri() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_NORMALIZESPACE_HAS_TOO_MANY_ARGS, "normalize-space() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_NUMBER_HAS_TOO_MANY_ARGS, "number() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_NAME_HAS_TOO_MANY_ARGS, "name() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_STRING_HAS_TOO_MANY_ARGS, "string() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_STRINGLENGTH_HAS_TOO_MANY_ARGS, "string.length() har f\u00f6r m\u00e5nga argument."},
				new object[] {ER_TRANSLATE_TAKES_3_ARGS, "Funktionen translate() tar emot tre argument!"},
				new object[] {ER_UNPARSEDENTITYURI_TAKES_1_ARG, "Funktionen unparsed-entity-uri borde ta emot ett argument!"},
				new object[] {ER_NAMESPACEAXIS_NOT_IMPLEMENTED, "Namespace-axel inte implementerad \u00e4n!"},
				new object[] {ER_UNKNOWN_AXIS, "ok\u00e4nd axel: {0}"},
				new object[] {ER_UNKNOWN_MATCH_OPERATION, "ok\u00e4nd matchningshandling!"},
				new object[] {ER_INCORRECT_ARG_LENGTH, "Nodtests argumentl\u00e4ngd i processing-instruction() \u00e4r inte korrekt!"},
				new object[] {ER_CANT_CONVERT_TO_NUMBER, "Kan inte konvertera {0} till ett nummer"},
				new object[] {ER_CANT_CONVERT_TO_NODELIST, "Kan inte konvertera {0} till en NodeList!"},
				new object[] {ER_CANT_CONVERT_TO_MUTABLENODELIST, "Kan inte konvertera {0} till en NodeSetDTM!"},
				new object[] {ER_CANT_CONVERT_TO_TYPE, "Kan inte konvertera {0} till en type//{1}"},
				new object[] {ER_EXPECTED_MATCH_PATTERN, "Matchningsm\u00f6nster i getMatchScore f\u00f6rv\u00e4ntat!"},
				new object[] {ER_COULDNOT_GET_VAR_NAMED, "Kunde inte h\u00e4mta variabeln {0}"},
				new object[] {ER_UNKNOWN_OPCODE, "FEL! Ok\u00e4nd op-kod: {0}"},
				new object[] {ER_EXTRA_ILLEGAL_TOKENS, "Ytterligare otill\u00e5tna tecken: {0}"},
				new object[] {ER_EXPECTED_DOUBLE_QUOTE, "Litteral omges av fel sorts citationstecken... dubbla citationstecken f\u00f6rv\u00e4ntade!"},
				new object[] {ER_EXPECTED_SINGLE_QUOTE, "Litteral omges av fel sorts citationstecken... enkla citationstecken f\u00f6rv\u00e4ntade!"},
				new object[] {ER_EMPTY_EXPRESSION, "Tomt uttryck!"},
				new object[] {ER_EXPECTED_BUT_FOUND, "{0} f\u00f6rv\u00e4ntat, men hittade: {1}"},
				new object[] {ER_INCORRECT_PROGRAMMER_ASSERTION, "Programmerares f\u00f6rs\u00e4kran \u00e4r inte korrekt! - {0}"},
				new object[] {ER_BOOLEAN_ARG_NO_LONGER_OPTIONAL, "boolean(...)-argument \u00e4r inte l\u00e4ngre valfri med 19990709 XPath-utkast."},
				new object[] {ER_FOUND_COMMA_BUT_NO_PRECEDING_ARG, "Hittade ',' men inget f\u00f6reg\u00e5ende argument!"},
				new object[] {ER_FOUND_COMMA_BUT_NO_FOLLOWING_ARG, "Hittade ',' men inget efterf\u00f6ljande argument!"},
				new object[] {ER_PREDICATE_ILLEGAL_SYNTAX, "'..[predikat]' or '.[predikat]' \u00e4r otill\u00e5ten syntax.  Anv\u00e4nd 'self::node()[predikat]' ist\u00e4llet."},
				new object[] {ER_ILLEGAL_AXIS_NAME, "otill\u00e5tet axel-namn: {0}"},
				new object[] {ER_UNKNOWN_NODETYPE, "ok\u00e4nd nodtyp: {0}"},
				new object[] {ER_PATTERN_LITERAL_NEEDS_BE_QUOTED, "M\u00f6nsterlitteral {0} m\u00e5ste s\u00e4ttas inom citationstecken!"},
				new object[] {ER_COULDNOT_BE_FORMATTED_TO_NUMBER, "{0} kunde inte formateras till ett nummer"},
				new object[] {ER_COULDNOT_CREATE_XMLPROCESSORLIAISON, "Kunde inte skapa XML TransformerFactory Liaison: {0}"},
				new object[] {ER_DIDNOT_FIND_XPATH_SELECT_EXP, "Fel! Hittade inte xpath select-uttryck (-select)."},
				new object[] {ER_COULDNOT_FIND_ENDOP_AFTER_OPLOCATIONPATH, "FEL! Hittade inte ENDOP efter OP_LOCATIONPATH"},
				new object[] {ER_ERROR_OCCURED, "Fel intr\u00e4ffade!"},
				new object[] {ER_ILLEGAL_VARIABLE_REFERENCE, "VariableReference angiven f\u00f6r variabel som \u00e4r utanf\u00f6r sammanhanget eller som saknar definition!  Namn = {0}"},
				new object[] {ER_AXES_NOT_ALLOWED, "Enbart barn::- och attribut::- axlar \u00e4r till\u00e5tna i matchningsm\u00f6nster!  Regelvidriga axlar = {0}"},
				new object[] {ER_KEY_HAS_TOO_MANY_ARGS, "key() har ett felaktigt antal argument."},
				new object[] {ER_COUNT_TAKES_1_ARG, "Funktionen count borde ta emot ett argument!"},
				new object[] {ER_COULDNOT_FIND_FUNCTION, "Hittade inte funktionen: {0}"},
				new object[] {ER_UNSUPPORTED_ENCODING, "Ej underst\u00f6dd kodning: {0}"},
				new object[] {ER_PROBLEM_IN_DTM_NEXTSIBLING, "Problem intr\u00e4ffade i DTM i getNextSibling... f\u00f6rs\u00f6ker \u00e5terh\u00e4mta"},
				new object[] {ER_CANNOT_WRITE_TO_EMPTYNODELISTIMPL, "Programmerarfel: EmptyNodeList kan inte skrivas till."},
				new object[] {ER_SETDOMFACTORY_NOT_SUPPORTED, "setDOMFactory underst\u00f6ds inte av XPathContext!"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Prefix must resolve to a namespace: {0}"},
				new object[] {ER_PARSE_NOT_SUPPORTED, "parse (InputSource source) underst\u00f6ds inte av XPathContext! Kan inte \u00f6ppna {0}"},
				new object[] {ER_SAX_API_NOT_HANDLED, "SAX API-tecken(char ch[]... hanteras inte av DTM!"},
				new object[] {ER_IGNORABLE_WHITESPACE_NOT_HANDLED, "ignorableWhitespace(char ch[]... hanteras inte av DTM!"},
				new object[] {ER_DTM_CANNOT_HANDLE_NODES, "DTMLiaison kan inte hantera noder av typen {0}"},
				new object[] {ER_XERCES_CANNOT_HANDLE_NODES, "DOM2Helper kan inte hantera noder av typen {0}"},
				new object[] {ER_XERCES_PARSE_ERROR_DETAILS, "DOM2Helper.parse-fel: SystemID - {0} rad - {1}"},
				new object[] {ER_XERCES_PARSE_ERROR, "DOM2Helper.parse-fel"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Ogiltigt UTF-16-surrogat uppt\u00e4ckt: {0} ?"},
				new object[] {ER_OIERROR, "IO-fel"},
				new object[] {ER_CANNOT_CREATE_URL, "Kan inte skapa url f\u00f6r: {0}"},
				new object[] {ER_XPATH_READOBJECT, "I XPath.readObject: {0}"},
				new object[] {ER_FUNCTION_TOKEN_NOT_FOUND, "funktionstecken saknas."},
				new object[] {ER_CANNOT_DEAL_XPATH_TYPE, "Kan inte hantera XPath-typ: {0}"},
				new object[] {ER_NODESET_NOT_MUTABLE, "NodeSet \u00e4r of\u00f6r\u00e4nderlig"},
				new object[] {ER_NODESETDTM_NOT_MUTABLE, "NodeSetDTM \u00e4r of\u00f6r\u00e4nderlig"},
				new object[] {ER_VAR_NOT_RESOLVABLE, "Variabel ej l\u00f6sbar: {0}"},
				new object[] {ER_NULL_ERROR_HANDLER, "Null error handler"},
				new object[] {ER_PROG_ASSERT_UNKNOWN_OPCODE, "Programmerares f\u00f6rs\u00e4kran: ok\u00e4nd op-kod: {0}"},
				new object[] {ER_ZERO_OR_ONE, "0 eller 1"},
				new object[] {ER_RTF_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "rtf() underst\u00f6ds inte av XRTreeFragSelectWrapper!"},
				new object[] {ER_ASNODEITERATOR_NOT_SUPPORTED_XRTREEFRAGSELECTWRAPPER, "asNodeIterator() underst\u00f6ds inte av XRTreeFragSelectWrapper!"},
				new object[] {ER_FSB_NOT_SUPPORTED_XSTRINGFORCHARS, "fsb() underst\u00f6ds inte av XRStringForChars!"},
				new object[] {ER_COULD_NOT_FIND_VAR, "Hittade inte variabeln med namn {0}"},
				new object[] {ER_XSTRINGFORCHARS_CANNOT_TAKE_STRING, "XStringForChars kan inte ta en str\u00e4ng som argument"},
				new object[] {ER_FASTSTRINGBUFFER_CANNOT_BE_NULL, "FastStringBuffer-argumentet f\u00e5r inte vara null"},
				new object[] {ER_TWO_OR_THREE, "2 eller 3"},
				new object[] {ER_VARIABLE_ACCESSED_BEFORE_BIND, "Variabeln anv\u00e4ndes innan den bands!"},
				new object[] {ER_FSB_CANNOT_TAKE_STRING, "XStringForFSB kan inte ha en str\u00e4ng som argument!"},
				new object[] {ER_SETTING_WALKER_ROOT_TO_NULL, "\n !!!! Fel! Anger roten f\u00f6r en \"walker\" till null!!!"},
				new object[] {ER_NODESETDTM_CANNOT_ITERATE, "Detta NodeSetDTM kan inte iterera till en tidigare nod!"},
				new object[] {ER_NODESET_CANNOT_ITERATE, "Detta NodeSet kan inte iterera till en tidigare nod!"},
				new object[] {ER_NODESETDTM_CANNOT_INDEX, "Detta NodeSetDTM har inte funktioner f\u00f6r indexering och r\u00e4kning!"},
				new object[] {ER_NODESET_CANNOT_INDEX, "Detta NodeSet har inte funktioner f\u00f6r indexering och r\u00e4kning!"},
				new object[] {ER_CANNOT_CALL_SETSHOULDCACHENODE, "Det g\u00e5r inte att anropa setShouldCacheNodes efter att nextNode har anropats!"},
				new object[] {ER_ONLY_ALLOWS, "{0} till\u00e5ter bara {1} argument"},
				new object[] {ER_UNKNOWN_STEP, "Programmerarkontroll i getNextStepPos: ok\u00e4nt steg Typ: {0}"},
				new object[] {ER_EXPECTED_REL_LOC_PATH, "En relativ s\u00f6kv\u00e4g f\u00f6rv\u00e4ntades efter token '/' eller '//'."},
				new object[] {ER_EXPECTED_LOC_PATH, "En plats f\u00f6rv\u00e4ntades, men f\u00f6ljande token p\u00e5tr\u00e4ffades\u003a  {0}"},
				new object[] {ER_EXPECTED_LOC_STEP, "Ett platssteg f\u00f6rv\u00e4ntades efter token  '/' eller '//'."},
				new object[] {ER_EXPECTED_NODE_TEST, "Ett nodtest som matchar antingen NCName:* eller QName f\u00f6rv\u00e4ntades."},
				new object[] {ER_EXPECTED_STEP_PATTERN, "Ett stegm\u00f6nster f\u00f6rv\u00e4ntades, men '/' p\u00e5tr\u00e4ffades."},
				new object[] {ER_EXPECTED_REL_PATH_PATTERN, "Ett m\u00f6nster f\u00f6r relativ s\u00f6kv\u00e4g f\u00f6rv\u00e4ntades."},
				new object[] {ER_CANT_CONVERT_TO_BOOLEAN, "Det g\u00e5r inte att konvertera {0} till ett Booleskt v\u00e4rde."},
				new object[] {ER_CANT_CONVERT_TO_SINGLENODE, "Det g\u00e5r inte att konvertera {0} till en enda nod. G\u00e4ller typerna ANY_UNORDERED_NODE_TYPE och FIRST_ORDERED_NODE_TYPE."},
				new object[] {ER_CANT_GET_SNAPSHOT_LENGTH, "Det g\u00e5r inte att erh\u00e5lla l\u00e4ngd f\u00f6r \u00f6gonblicksbild p\u00e5 typ: {0}. G\u00e4ller typerna UNORDERED_NODE_SNAPSHOT_TYPE och ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_NON_ITERATOR_TYPE, "Det g\u00e5r inte att iterera \u00f6ver den icke itererbara typen: {0}"},
				new object[] {ER_DOC_MUTATED, "Dokumentet har \u00e4ndrats sedan resultatet genererades. Iterering ogiltig."},
				new object[] {ER_INVALID_XPATH_TYPE, "Ogiltigt XPath-typargument: {0}"},
				new object[] {ER_EMPTY_XPATH_RESULT, "Tomt XPath-resultatobjekt"},
				new object[] {ER_INCOMPATIBLE_TYPES, "Den genererade typen: {0} kan inte bearbetas i den angivna typen: {1}"},
				new object[] {ER_NULL_RESOLVER, "Det g\u00e5r inte att l\u00f6sa prefixet utan prefixl\u00f6sare."},
				new object[] {ER_CANT_CONVERT_TO_STRING, "Det g\u00e5r inte att konvertera {0} till en str\u00e4ng."},
				new object[] {ER_NON_SNAPSHOT_TYPE, "Det g\u00e5r inte att anropa snapshotItem p\u00e5 typ: {0}. Metoden g\u00e4ller typerna UNORDERED_NODE_SNAPSHOT_TYPE och ORDERED_NODE_SNAPSHOT_TYPE."},
				new object[] {ER_WRONG_DOCUMENT, "Kontextnoden tillh\u00f6r inte dokumentet som \u00e4r bundet till denna XPathEvaluator."},
				new object[] {ER_WRONG_NODETYPE, "Kontextnoden kan inte hanteras."},
				new object[] {ER_XPATH_ERROR, "Ok\u00e4nt fel i XPath."},
				new object[] {WG_LOCALE_NAME_NOT_HANDLED, "locale-namnet i format-number-funktionen \u00e4nnu inte hanterat!"},
				new object[] {WG_PROPERTY_NOT_SUPPORTED, "XSL-Egenskap underst\u00f6ds inte: {0}"},
				new object[] {WG_DONT_DO_ANYTHING_WITH_NS, "G\u00f6r f\u00f6r n\u00e4rvarande inte n\u00e5gonting med namespace {0} i egenskap: {1}"},
				new object[] {WG_SECURITY_EXCEPTION, "SecurityException vid f\u00f6rs\u00f6k att f\u00e5 tillg\u00e5ng till XSL-systemegenskap: {0}"},
				new object[] {WG_QUO_NO_LONGER_DEFINED, "Gammal syntax: quo(...) \u00e4r inte l\u00e4ngre definierad i XPath."},
				new object[] {WG_NEED_DERIVED_OBJECT_TO_IMPLEMENT_NODETEST, "XPath beh\u00f6ver ett deriverat objekt f\u00f6r att implementera nodeTest!"},
				new object[] {WG_FUNCTION_TOKEN_NOT_FOUND, "funktionstecken saknas."},
				new object[] {WG_COULDNOT_FIND_FUNCTION, "Hittade inte funktion: {0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Kan inte skapa URL fr\u00e5n: {0}"},
				new object[] {WG_EXPAND_ENTITIES_NOT_SUPPORTED, "Alternativet -E underst\u00f6ds inte f\u00f6r DTM-tolk"},
				new object[] {WG_ILLEGAL_VARIABLE_REFERENCE, "VariableReference angiven f\u00f6r variabel som \u00e4r utanf\u00f6r sammanhanget eller som saknar definition!  Namn = {0}"},
				new object[] {WG_UNSUPPORTED_ENCODING, "Ej underst\u00f6dd kodning: {0}"},
				new object[] {"ui_language", "sv"},
				new object[] {"help_language", "sv"},
				new object[] {"language", "sv"},
				new object[] {"BAD_CODE", "Parameter till createMessage ligger utanf\u00f6r till\u00e5tet intervall"},
				new object[] {"FORMAT_FAILED", "Undantag utl\u00f6st vid messageFormat-anrop"},
				new object[] {"version", ">>>>>>> Xalan Version"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "ja"},
				new object[] {"line", "Rad //"},
				new object[] {"column", "Kolumn //"},
				new object[] {"xsldone", "XSLProcessor f\u00e4rdig"},
				new object[] {"xpath_option", "xpath-alternativ"},
				new object[] {"optionIN", "    [-in inputXMLURL]"},
				new object[] {"optionSelect", "[-select xpath-uttryck]"},
				new object[] {"optionMatch", "   [-match matchningsm\u00f6nster (f\u00f6r matchningsdiagnostik)]"},
				new object[] {"optionAnyExpr", "Eller bara ett xpath-uttryck kommer att g\u00f6ra en diagnostik-dump"},
				new object[] {"noParsermsg1", "XSL-Process misslyckades."},
				new object[] {"noParsermsg2", "** Hittade inte tolk **"},
				new object[] {"noParsermsg3", "V\u00e4nligen kontrollera din classpath"},
				new object[] {"noParsermsg4", "Om du inte har IBMs XML-Tolk f\u00f6r Java, kan du ladda ner den fr\u00e5n"},
				new object[] {"noParsermsg5", "IBMs AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"}
			};
		  }
	  }

	  // ================= INFRASTRUCTURE ======================

	  /// <summary>
	  /// Field BAD_CODE </summary>
	  public new const string BAD_CODE = "D\u00c5LIG_KOD";

	  /// <summary>
	  /// Field FORMAT_FAILED </summary>
	  public const string FORMAT_FAILDE = "FORMATTERING_MISSLYCKADES";

	  /// <summary>
	  /// Field ERROR_RESOURCES </summary>
	  public new const string ERROR_RESOURCES = "org.apache.xpath.res.XPATHErrorResources";

	  /// <summary>
	  /// Field ERROR_STRING </summary>
	  public new const string ERROR_STRING = "//fel";

	  /// <summary>
	  /// Field ERROR_HEADER </summary>
	  public new const string ERROR_HEADER = "Fel: ";

	  /// <summary>
	  /// Field WARNING_HEADER </summary>
	  public new const string WARNING_HEADER = "Varning: ";

	  /// <summary>
	  /// Field XSL_HEADER </summary>
	  public new const string XSL_HEADER = "XSL ";

	  /// <summary>
	  /// Field XML_HEADER </summary>
	  public new const string XML_HEADER = "XML ";

	  /// <summary>
	  /// Field QUERY_HEADER </summary>
	  public new const string QUERY_HEADER = "M\u00d6NSTER ";

	}



}