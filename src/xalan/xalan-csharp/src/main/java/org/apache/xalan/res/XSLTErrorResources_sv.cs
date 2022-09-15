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
 * $Id: XSLTErrorResources_sv.java 468641 2006-10-28 06:54:42Z minchau $
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
	public class XSLTErrorResources_sv : XSLTErrorResources
	{

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

	  /// <summary>
	  /// Get the lookup table for error messages.   
	  /// </summary>
	  /// <returns> The int to message lookup table. </returns>
	  public override object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ERROR0000", "{0}"},
				new object[] {ER_NO_CURLYBRACE, "Fel: Kan inte ha '{' inuti uttryck"},
				new object[] {ER_ILLEGAL_ATTRIBUTE, "{0} har ett otill\u00e5tet attribut: {1}"},
				new object[] {ER_NULL_SOURCENODE_APPLYIMPORTS, "sourceNode \u00e4r null i xsl:apply-imports!"},
				new object[] {ER_CANNOT_ADD, "Kan inte l\u00e4gga {0} till {1}"},
				new object[] {ER_NULL_SOURCENODE_HANDLEAPPLYTEMPLATES, "sourceNode \u00e4r null i handleApplyTemplatesInstruction!"},
				new object[] {ER_NO_NAME_ATTRIB, "{0} m\u00e5ste ha ett namn-attribut."},
				new object[] {ER_TEMPLATE_NOT_FOUND, "Hittade inte mallen med namn: {0}"},
				new object[] {ER_CANT_RESOLVE_NAME_AVT, "Kunde inte l\u00f6sa namn-AVT i xsl:call-template."},
				new object[] {ER_REQUIRES_ATTRIB, "{0} kr\u00e4ver attribut: {1}"},
				new object[] {ER_MUST_HAVE_TEST_ATTRIB, "{0} m\u00e5ste ha ett ''test''-attribut."},
				new object[] {ER_BAD_VAL_ON_LEVEL_ATTRIB, "D\u00e5ligt v\u00e4rde p\u00e5 niv\u00e5-attribut: {0}"},
				new object[] {ER_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "Namn p\u00e5 behandlande instruktion f\u00e5r inte vara 'xml'"},
				new object[] {ER_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "Namn p\u00e5 behandlande instruktion m\u00e5ste vara ett giltigt NCNamn: {0}"},
				new object[] {ER_NEED_MATCH_ATTRIB, "{0} m\u00e5ste ha ett matchningsattribut om det har ett tillst\u00e5nd."},
				new object[] {ER_NEED_NAME_OR_MATCH_ATTRIB, "{0} kr\u00e4ver antingen ett namn eller ett matchningsattribut."},
				new object[] {ER_CANT_RESOLVE_NSPREFIX, "Kan inte l\u00f6sa namnrymdsprefix: {0}"},
				new object[] {ER_ILLEGAL_VALUE, "xml:space har ett otill\u00e5tet v\u00e4rde: {0}"},
				new object[] {ER_NO_OWNERDOC, "Barnnod saknar \u00e4gardokument!"},
				new object[] {ER_ELEMTEMPLATEELEM_ERR, "ElemTemplateElement-fel: {0}"},
				new object[] {ER_NULL_CHILD, "F\u00f6rs\u00f6ker l\u00e4gga till ett null-barn!"},
				new object[] {ER_NEED_SELECT_ATTRIB, "{0} kr\u00e4ver ett valattribut."},
				new object[] {ER_NEED_TEST_ATTRIB, "xsl:when m\u00e5ste ha ett 'test'-attribut."},
				new object[] {ER_NEED_NAME_ATTRIB, "xsl:with-param m\u00e5ste ha ett 'namn'-attribut."},
				new object[] {ER_NO_CONTEXT_OWNERDOC, "Kontext saknar \u00e4gardokument!"},
				new object[] {ER_COULD_NOT_CREATE_XML_PROC_LIAISON, "Kunde inte skapa XML TransformerFactory Liaison: {0}"},
				new object[] {ER_PROCESS_NOT_SUCCESSFUL, "Xalan: Process misslyckades."},
				new object[] {ER_NOT_SUCCESSFUL, "Xalan: misslyckades."},
				new object[] {ER_ENCODING_NOT_SUPPORTED, "Kodning inte underst\u00f6dd: {0}"},
				new object[] {ER_COULD_NOT_CREATE_TRACELISTENER, "Kunde inte skapa TraceListener: {0}"},
				new object[] {ER_KEY_REQUIRES_NAME_ATTRIB, "xsl:key m\u00e5ste ha ett 'namn'-attribut."},
				new object[] {ER_KEY_REQUIRES_MATCH_ATTRIB, "xsl:key m\u00e5ste ha ett 'matcha'-attribut."},
				new object[] {ER_KEY_REQUIRES_USE_ATTRIB, "xsl:key m\u00e5ste ha ett 'anv\u00e4nd'-attribut."},
				new object[] {ER_REQUIRES_ELEMENTS_ATTRIB, "(StylesheetHandler) {0} kr\u00e4ver ett ''element''-attribut!"},
				new object[] {ER_MISSING_PREFIX_ATTRIB, "(StylesheetHandler) {0} ''prefix''-attribut saknas"},
				new object[] {ER_BAD_STYLESHEET_URL, "Stylesheet URL \u00e4r d\u00e5lig: {0}"},
				new object[] {ER_FILE_NOT_FOUND, "Stylesheet-fil saknas: {0}"},
				new object[] {ER_IOEXCEPTION, "Fick IO-Undantag med stylesheet-fil: {0}"},
				new object[] {ER_NO_HREF_ATTRIB, "(StylesheetHandler) Hittade inte href-attribute f\u00f6r {0}"},
				new object[] {ER_STYLESHEET_INCLUDES_ITSELF, "(StylesheetHandler) {0} inkluderar, direkt eller indirekt, sig sj\u00e4lv!"},
				new object[] {ER_PROCESSINCLUDE_ERROR, "StylesheetHandler.processInclude-fel, {0}"},
				new object[] {ER_MISSING_LANG_ATTRIB, "(StylesheetHandler) {0} ''lang''-attribut' saknas"},
				new object[] {ER_MISSING_CONTAINER_ELEMENT_COMPONENT, "(StylesheetHandler) felplacerade {0} element?? Saknar beh\u00e5llarelement  ''komponent''"},
				new object[] {ER_CAN_ONLY_OUTPUT_TO_ELEMENT, "Kan endast skicka utdata till ett Element, ett DocumentFragment, ett Document, eller en PrintWriter."},
				new object[] {ER_PROCESS_ERROR, "StylesheetRoot.process-fel"},
				new object[] {ER_UNIMPLNODE_ERROR, "UnImplNode-fel: {0}"},
				new object[] {ER_NO_SELECT_EXPRESSION, "Fel! Hittade inte xpath select-uttryck (-select)."},
				new object[] {ER_CANNOT_SERIALIZE_XSLPROCESSOR, "Kan inte serialisera en XSLProcessor!"},
				new object[] {ER_NO_INPUT_STYLESHEET, "Stylesheet-indata ej angiven!"},
				new object[] {ER_FAILED_PROCESS_STYLESHEET, "Kunde inte behandla stylesheet!"},
				new object[] {ER_COULDNT_PARSE_DOC, "Kunde inte tolka {0} dokument!"},
				new object[] {ER_COULDNT_FIND_FRAGMENT, "Hittade inte fragment: {0}"},
				new object[] {ER_NODE_NOT_ELEMENT, "Nod som pekades p\u00e5 av fragment-identifierare var inte ett element: {0}"},
				new object[] {ER_FOREACH_NEED_MATCH_OR_NAME_ATTRIB, "for-each kr\u00e4ver antingen en matchning eller ett namnattribut."},
				new object[] {ER_TEMPLATES_NEED_MATCH_OR_NAME_ATTRIB, "mallar kr\u00e4ver antingen en matchning eller ett namnattribut."},
				new object[] {ER_NO_CLONE_OF_DOCUMENT_FRAG, "Ingen klon av ett dokumentfragment!"},
				new object[] {ER_CANT_CREATE_ITEM, "Kan inte skapa element i resultattr\u00e4d: {0}"},
				new object[] {ER_XMLSPACE_ILLEGAL_VALUE, "xml:space i k\u00e4ll-XML har ett otill\u00e5tet v\u00e4rde: {0}"},
				new object[] {ER_NO_XSLKEY_DECLARATION, "Det finns ingen xsl:key-deklaration f\u00f6r {0}!"},
				new object[] {ER_CANT_CREATE_URL, "Fel! Kan inte skapa url f\u00f6r: {0}"},
				new object[] {ER_XSLFUNCTIONS_UNSUPPORTED, "xsl:functions \u00e4r inte underst\u00f6dd"},
				new object[] {ER_PROCESSOR_ERROR, "XSLT TransformerFactory-Fel"},
				new object[] {ER_NOT_ALLOWED_INSIDE_STYLESHEET, "(StylesheetHandler) {0} \u00e4r inte till\u00e5ten inne i ett stylesheet!"},
				new object[] {ER_RESULTNS_NOT_SUPPORTED, "result-ns inte l\u00e4ngre underst\u00f6dd!  Anv\u00e4nd xsl:output ist\u00e4llet."},
				new object[] {ER_DEFAULTSPACE_NOT_SUPPORTED, "default-space inte l\u00e4ngre underst\u00f6dd!  Anv\u00e4nd xsl:strip-space eller xsl:preserve-space ist\u00e4llet."},
				new object[] {ER_INDENTRESULT_NOT_SUPPORTED, "indent-result inte l\u00e4ngre underst\u00f6dd!  Anv\u00e4nd xsl:output ist\u00e4llet."},
				new object[] {ER_ILLEGAL_ATTRIB, "(StylesheetHandler) {0} har ett otill\u00e5tet attribut: {1}"},
				new object[] {ER_UNKNOWN_XSL_ELEM, "Ok\u00e4nt XSL-element: {0}"},
				new object[] {ER_BAD_XSLSORT_USE, "(StylesheetHandler) xsl:sort kan endast anv\u00e4ndas med xsl:apply-templates eller xsl:for-each."},
				new object[] {ER_MISPLACED_XSLWHEN, "(StylesheetHandler) felplacerade xsl:when!"},
				new object[] {ER_XSLWHEN_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:when h\u00e4rstammar inte fr\u00e5n xsl:choose!"},
				new object[] {ER_MISPLACED_XSLOTHERWISE, "(StylesheetHandler) felplacerade xsl:otherwise!"},
				new object[] {ER_XSLOTHERWISE_NOT_PARENTED_BY_XSLCHOOSE, "(StylesheetHandler) xsl:otherwise h\u00e4rstammar inte fr\u00e5n xsl:choose!"},
				new object[] {ER_NOT_ALLOWED_INSIDE_TEMPLATE, "(StylesheetHandler) {0} \u00e4r inte till\u00e5ten inne i en mall!"},
				new object[] {ER_UNKNOWN_EXT_NS_PREFIX, "(StylesheetHandler) {0} utbyggnadsnamnrymdsprefix {1} ok\u00e4nt"},
				new object[] {ER_IMPORTS_AS_FIRST_ELEM, "(StylesheetHandler) Imports kan endast f\u00f6rekomma som de f\u00f6rsta elementen i ett stylesheet!"},
				new object[] {ER_IMPORTING_ITSELF, "(StylesheetHandler) {0} importerar, direkt eller indirekt, sig sj\u00e4lv!"},
				new object[] {ER_XMLSPACE_ILLEGAL_VAL, "(StylesheetHandler) " + "xml:space har ett otill\u00e5tet v\u00e4rde: {0}"},
				new object[] {ER_PROCESSSTYLESHEET_NOT_SUCCESSFUL, "processStylesheet misslyckades!"},
				new object[] {ER_SAX_EXCEPTION, "SAX-Undantag"},
				new object[] {ER_XSLT_ERROR, "XSLT-fel"},
				new object[] {ER_CURRENCY_SIGN_ILLEGAL, "valutatecken \u00e4r inte till\u00e5tet i formatm\u00f6nsterstr\u00e4ng"},
				new object[] {ER_DOCUMENT_FUNCTION_INVALID_IN_STYLESHEET_DOM, "Dokumentfunktion inte underst\u00f6dd i Stylesheet DOM!"},
				new object[] {ER_CANT_RESOLVE_PREFIX_OF_NON_PREFIX_RESOLVER, "Kan inte l\u00f6sa prefix i icke-Prefixl\u00f6sare!"},
				new object[] {ER_REDIRECT_COULDNT_GET_FILENAME, "Redirect extension: Hittade inte filnamn - fil eller valattribut m\u00e5ste returnera vald  str\u00e4ng."},
				new object[] {ER_CANNOT_BUILD_FORMATTERLISTENER_IN_REDIRECT, "Kan inte bygga FormatterListener i Redirect extension!"},
				new object[] {ER_INVALID_PREFIX_IN_EXCLUDERESULTPREFIX, "Prefix i exkludera-resultat-prefix \u00e4r inte giltig: {0}"},
				new object[] {ER_MISSING_NS_URI, "Namnrymds-URI saknas f\u00f6r angivna prefix"},
				new object[] {ER_MISSING_ARG_FOR_OPTION, "Argument saknas f\u00f6r alternativ: {0}"},
				new object[] {ER_INVALID_OPTION, "Ogiltigt alternativ: {0}"},
				new object[] {ER_MALFORMED_FORMAT_STRING, "Fel format p\u00e5 formatstr\u00e4ng: {0}"},
				new object[] {ER_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet m\u00e5ste ha ett 'version'-attribut!"},
				new object[] {ER_ILLEGAL_ATTRIBUTE_VALUE, "Attribut: {0} har ett otill\u00e5tet v\u00e4rde: {1}"},
				new object[] {ER_CHOOSE_REQUIRES_WHEN, "xsl:choose kr\u00e4ver ett xsl:when"},
				new object[] {ER_NO_APPLY_IMPORT_IN_FOR_EACH, "xsl:apply-imports inte till\u00e5tet i ett xsl:for-each"},
				new object[] {ER_CANT_USE_DTM_FOR_OUTPUT, "Kan inte anv\u00e4nda DTMLiaison till en DOM utdatanod... skicka en org.apache.xpath.DOM2Helper ist\u00e4llet!"},
				new object[] {ER_CANT_USE_DTM_FOR_INPUT, "Kan inte anv\u00e4nda DTMLiaison till en DOM indatanod... skicka en org.apache.xpath.DOM2Helper ist\u00e4llet!"},
				new object[] {ER_CALL_TO_EXT_FAILED, "Anrop till anslutningselement misslyckades: {0}"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "Prefix m\u00e5ste l\u00f6sa till en mamnrymd: {0}"},
				new object[] {ER_INVALID_UTF16_SURROGATE, "Ogiltigt UTF-16-surrogat uppt\u00e4ckt: {0} ?"},
				new object[] {ER_XSLATTRSET_USED_ITSELF, "xsl:attribute-set {0} anv\u00e4nde sig sj\u00e4lvt, vilket kommer att orsaka en  o\u00e4ndlig loop."},
				new object[] {ER_CANNOT_MIX_XERCESDOM, "Kan inte blanda icke-Xerces-DOM-indata med Xerces-DOM-utdata!"},
				new object[] {ER_TOO_MANY_LISTENERS, "addTraceListenersToStylesheet - TooManyListenersException"},
				new object[] {ER_IN_ELEMTEMPLATEELEM_READOBJECT, "I ElemTemplateElement.readObject: {0}"},
				new object[] {ER_DUPLICATE_NAMED_TEMPLATE, "Hittade mer \u00e4n en mall med namnet: {0}"},
				new object[] {ER_INVALID_KEY_CALL, "Ogiltigt funktionsanrop: rekursiva key()-anrop \u00e4r inte till\u00e5tna"},
				new object[] {ER_REFERENCING_ITSELF, "Variabel {0} h\u00e4nvisar, direkt eller indirekt, till sig sj\u00e4lv!"},
				new object[] {ER_ILLEGAL_DOMSOURCE_INPUT, "Indatanoden till en DOMSource f\u00f6r newTemplates f\u00e5r inte vara null!"},
				new object[] {ER_CLASS_NOT_FOUND_FOR_OPTION, "Klassfil f\u00f6r alternativ {0} saknas"},
				new object[] {ER_REQUIRED_ELEM_NOT_FOUND, "N\u00f6dv\u00e4ndigt element saknas: {0}"},
				new object[] {ER_INPUT_CANNOT_BE_NULL, "InputStream f\u00e5r inte vara null"},
				new object[] {ER_URI_CANNOT_BE_NULL, "URI f\u00e5r inte vara null"},
				new object[] {ER_FILE_CANNOT_BE_NULL, "Fil f\u00e5r inte vara null"},
				new object[] {ER_SOURCE_CANNOT_BE_NULL, "InputSource f\u00e5r inte vara null"},
				new object[] {ER_CANNOT_INIT_BSFMGR, "Kan inte initialisera BSF Manager"},
				new object[] {ER_CANNOT_CMPL_EXTENSN, "Kunde inte kompilera anslutning"},
				new object[] {ER_CANNOT_CREATE_EXTENSN, "Kunde inte skapa anslutning: {0} p\u00e5 grund av: {1}"},
				new object[] {ER_INSTANCE_MTHD_CALL_REQUIRES, "Instansmetodanrop till metod {0} kr\u00e4ver en Objektinstans som f\u00f6rsta argument"},
				new object[] {ER_INVALID_ELEMENT_NAME, "Ogiltigt elementnamn angivet {0}"},
				new object[] {ER_ELEMENT_NAME_METHOD_STATIC, "Elementnamnmetod m\u00e5ste vara static {0}"},
				new object[] {ER_EXTENSION_FUNC_UNKNOWN, "Anslutningsfunktion {0} : {1} \u00e4r ok\u00e4nd"},
				new object[] {ER_MORE_MATCH_CONSTRUCTOR, "Fler \u00e4n en b\u00e4sta matchning f\u00f6r konstruktor f\u00f6r {0}"},
				new object[] {ER_MORE_MATCH_METHOD, "Fler \u00e4n en b\u00e4sta matchning f\u00f6r metod {0}"},
				new object[] {ER_MORE_MATCH_ELEMENT, "Fler \u00e4n en b\u00e4sta matchning f\u00f6r elementmetod {0}"},
				new object[] {ER_INVALID_CONTEXT_PASSED, "Ogiltig kontext skickad f\u00f6r att utv\u00e4rdera {0}"},
				new object[] {ER_POOL_EXISTS, "Pool finns redan"},
				new object[] {ER_NO_DRIVER_NAME, "Inget driver-namn angivet"},
				new object[] {ER_NO_URL, "Ingen URL angiven"},
				new object[] {ER_POOL_SIZE_LESSTHAN_ONE, "Poolstorlek \u00e4r mindre \u00e4n ett!"},
				new object[] {ER_INVALID_DRIVER, "Ogiltigt driver-namn angivet"},
				new object[] {ER_NO_STYLESHEETROOT, "Hittade inte stylesheet-roten!"},
				new object[] {ER_ILLEGAL_XMLSPACE_VALUE, "Ogiltigt v\u00e4rde f\u00f6r xml:space"},
				new object[] {ER_PROCESSFROMNODE_FAILED, "processFromNode misslyckades"},
				new object[] {ER_RESOURCE_COULD_NOT_LOAD, "Resursen [ {0} ] kunde inte laddas: {1} \n {2} \t {3}"},
				new object[] {ER_BUFFER_SIZE_LESSTHAN_ZERO, "Bufferstorlek <=0"},
				new object[] {ER_UNKNOWN_ERROR_CALLING_EXTENSION, "Ok\u00e4nt fel vid anslutningsanrop"},
				new object[] {ER_NO_NAMESPACE_DECL, "Prefix{0} har inte en motsvarande namnrymdsdeklaration"},
				new object[] {ER_ELEM_CONTENT_NOT_ALLOWED, "Elementinneh\u00e5ll \u00e4r inte till\u00e5tet f\u00f6r lang=javaclass {0}"},
				new object[] {ER_STYLESHEET_DIRECTED_TERMINATION, "Stylesheet-ledd avslutning"},
				new object[] {ER_ONE_OR_TWO, "1 eller 2"},
				new object[] {ER_TWO_OR_THREE, "2 eller 3"},
				new object[] {ER_COULD_NOT_LOAD_RESOURCE, "Kunde inte ladda {0} (kontrollera CLASSPATH), anv\u00e4nder nu enbart standard"},
				new object[] {ER_CANNOT_INIT_DEFAULT_TEMPLATES, "Kan inte initialisera standardmallar"},
				new object[] {ER_RESULT_NULL, "Result borde inte vara null"},
				new object[] {ER_RESULT_COULD_NOT_BE_SET, "Result kunde inte s\u00e4ttas"},
				new object[] {ER_NO_OUTPUT_SPECIFIED, "Ingen utdata angiven"},
				new object[] {ER_CANNOT_TRANSFORM_TO_RESULT_TYPE, "Kan inte omvandla till en Result av typ {0}"},
				new object[] {ER_CANNOT_TRANSFORM_SOURCE_TYPE, "Kan inte omvandla en Source av typ {0}"},
				new object[] {ER_NULL_CONTENT_HANDLER, "Inneh\u00e5llshanterare med v\u00e4rde null"},
				new object[] {ER_NULL_ERROR_HANDLER, "Felhanterare med v\u00e4rde null"},
				new object[] {ER_CANNOT_CALL_PARSE, "parse kan inte anropas om ContentHandler inte har satts"},
				new object[] {ER_NO_PARENT_FOR_FILTER, "Ingen f\u00f6r\u00e4lder till filter"},
				new object[] {ER_NO_STYLESHEET_IN_MEDIA, "Stylesheet saknas i: {0}, media= {1}"},
				new object[] {ER_NO_STYLESHEET_PI, "xml-stylesheet PI saknas i: {0}"},
				new object[] {ER_NOT_SUPPORTED, "Underst\u00f6ds inte: {0}"},
				new object[] {ER_PROPERTY_VALUE_BOOLEAN, "V\u00e4rde p\u00e5 egenskap {0} borde vara en Boolesk instans"},
				new object[] {"ER_SRC_ATTRIB_NOT_SUPPORTED", "src-attributet underst\u00f6ds \u00e4nnu inte f\u00f6r {0}"},
				new object[] {ER_RESOURCE_COULD_NOT_FIND, "Resursen [ {0} ] saknas. \n {1}"},
				new object[] {ER_OUTPUT_PROPERTY_NOT_RECOGNIZED, "Utdata-egenskap k\u00e4nns inte igen: {0}"},
				new object[] {ER_FAILED_CREATING_ELEMLITRSLT, "Kunde inte skapa instans av ElemLiteralResult"},
				new object[] {ER_VALUE_SHOULD_BE_NUMBER, "V\u00e4rdet f\u00f6r {0} b\u00f6r inneh\u00e5lla en siffra som inte kan tolkas"},
				new object[] {ER_VALUE_SHOULD_EQUAL, "V\u00e4rde p\u00e5 {0} borde motsvara ja eller nej"},
				new object[] {ER_FAILED_CALLING_METHOD, " Kunde inte anropa metoden {0}"},
				new object[] {ER_FAILED_CREATING_ELEMTMPL, "Kunde inte skapa instans av ElemTemplateElement"},
				new object[] {ER_CHARS_NOT_ALLOWED, "Tecken \u00e4r inte till\u00e5tna i dokumentet vid den h\u00e4r tidpunkten"},
				new object[] {ER_ATTR_NOT_ALLOWED, "Attributet \"{0}\" \u00e4r inte till\u00e5ten i det {1} elementet!"},
				new object[] {ER_BAD_VALUE, "{0} d\u00e5ligt v\u00e4rde {1} "},
				new object[] {ER_ATTRIB_VALUE_NOT_FOUND, "Attributet {0} saknas "},
				new object[] {ER_ATTRIB_VALUE_NOT_RECOGNIZED, "Attributv\u00e4rdet {0} k\u00e4nns inte igen "},
				new object[] {ER_NULL_URI_NAMESPACE, "F\u00f6rs\u00f6ker generera ett namnomr\u00e5desprefix med en null-URI"},
				new object[] {ER_NUMBER_TOO_BIG, "F\u00f6rs\u00f6ker formatera en siffra som \u00e4r st\u00f6rre \u00e4n det st\u00f6rsta l\u00e5nga heltalet"},
				new object[] {ER_CANNOT_FIND_SAX1_DRIVER, "Det g\u00e5r inte att hitta SAX1-drivrutinen klass {0}"},
				new object[] {ER_SAX1_DRIVER_NOT_LOADED, "SAX1-drivrutinen klass {0} hittades men kan inte laddas"},
				new object[] {ER_SAX1_DRIVER_NOT_INSTANTIATED, "SAX1-drivrutinen klass {0} hittades men kan inte instansieras"},
				new object[] {ER_SAX1_DRIVER_NOT_IMPLEMENT_PARSER, "SAX1-drivrutinen klass {0} implementerar inte org.xml.sax.Parser"},
				new object[] {ER_PARSER_PROPERTY_NOT_SPECIFIED, "Systemegenskapen org.xml.sax.parser \u00e4r inte angiven"},
				new object[] {ER_PARSER_ARG_CANNOT_BE_NULL, "Tolkningsargumentet f\u00e5r inte vara null"},
				new object[] {ER_FEATURE, "Funktion:a {0}"},
				new object[] {ER_PROPERTY, "Egenskap:a {0}"},
				new object[] {ER_NULL_ENTITY_RESOLVER, "Nullenhetsl\u00f6sare"},
				new object[] {ER_NULL_DTD_HANDLER, "Null-DTD-hanterare"},
				new object[] {ER_NO_DRIVER_NAME_SPECIFIED, "Inget drivrutinsnamn \u00e4r angett!"},
				new object[] {ER_NO_URL_SPECIFIED, "Ingen URL har angetts!"},
				new object[] {ER_POOLSIZE_LESS_THAN_ONE, "Poolstorleken \u00e4r mindre \u00e4n 1!"},
				new object[] {ER_INVALID_DRIVER_NAME, "Ett ogiltigt drivrutinsnamn har angetts!"},
				new object[] {ER_ERRORLISTENER, "ErrorListener"},
				new object[] {ER_ASSERT_NO_TEMPLATE_PARENT, "Programmerarfel! expr har inget \u00f6verordnat ElemTemplateElement!"},
				new object[] {ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, "Programmerarkontroll i RundundentExprEliminator: {0}"},
				new object[] {ER_NOT_ALLOWED_IN_POSITION, "{0} \u00e4r inte till\u00e5ten i denna position i formatmallen!"},
				new object[] {ER_NONWHITESPACE_NOT_ALLOWED_IN_POSITION, "Text utan blanksteg \u00e4r inte till\u00e5ten i denna position i formatmallen!"},
				new object[] {INVALID_TCHAR, "Ogiltigt v\u00e4rde: {1} anv\u00e4nds f\u00f6r CHAR-attributet: {0}.  Ett attribut av CHAR-typ f\u00e5r bara ha 1 tecken!"},
				new object[] {INVALID_QNAME, "Ogiltigt v\u00e4rde:a {1} anv\u00e4nds f\u00f6r QNAME-attributet:a {0}"},
				new object[] {INVALID_ENUM, "Ogiltigt v\u00e4rde:a  {1} anv\u00e4nds f\u00f6r ENUM-attributet:a {0}.  Giltiga v\u00e4rden \u00e4r:a {2}."},
				new object[] {INVALID_NMTOKEN, "Ogiltigt v\u00e4rde:a {1} anv\u00e4nds f\u00f6r NMTOKEN-attributet:a {0} "},
				new object[] {INVALID_NCNAME, "Ogiltigt v\u00e4rde:a {1} anv\u00e4nds f\u00f6r NCNAME-attributet:a {0} "},
				new object[] {INVALID_BOOLEAN, "Ogiltigt v\u00e4rde:a {1} anv\u00e4nds som Booleskt attribut:a {0} "},
				new object[] {INVALID_NUMBER, "Ogiltigt v\u00e4rde:a {1} anv\u00e4nds som sifferattribut:a {0} "},
				new object[] {ER_ARG_LITERAL, "Argument f\u00f6r {0} i matchningsm\u00f6nstret m\u00e5ste vara literalt."},
				new object[] {ER_DUPLICATE_GLOBAL_VAR, "Dubbel deklaration av global variabel."},
				new object[] {ER_DUPLICATE_VAR, "Dubbel variabeldeklaration."},
				new object[] {ER_TEMPLATE_NAME_MATCH, "xsl: en mall m\u00e5ste ha ett namn och ett matchningsattribut (eller b\u00e5de och)"},
				new object[] {ER_INVALID_PREFIX, "Prefix i exclude-result-prefixes \u00e4r ogiltigt:a {0}"},
				new object[] {ER_NO_ATTRIB_SET, "attributserien {0} finns inte"},
				new object[] {WG_FOUND_CURLYBRACE, "Hittade '}' men ingen attributmall \u00e4r \u00f6ppen!"},
				new object[] {WG_COUNT_ATTRIB_MATCHES_NO_ANCESTOR, "Varning: r\u00e4knarattribut matchar inte en f\u00f6rf\u00e4der in xsl:number! Target = {0}"},
				new object[] {WG_EXPR_ATTRIB_CHANGED_TO_SELECT, "Gammal syntax: Namnet p\u00e5  'expr'-attributet har \u00e4ndrats till 'select'."},
				new object[] {WG_NO_LOCALE_IN_FORMATNUMBER, "Xalan hanterar \u00e4nnu inte locale-namnet i funktionen format-number."},
				new object[] {WG_LOCALE_NOT_FOUND, "Varning: Hittade inte locale f\u00f6r xml:lang{0}"},
				new object[] {WG_CANNOT_MAKE_URL_FROM, "Kan inte skapa URL fr\u00e5n: {0}"},
				new object[] {WG_CANNOT_LOAD_REQUESTED_DOC, "Kan inte ladda beg\u00e4rd doc: {0}"},
				new object[] {WG_CANNOT_FIND_COLLATOR, "Hittade inte Collator f\u00f6r <sort xml:lang={0}"},
				new object[] {WG_FUNCTIONS_SHOULD_USE_URL, "Gammal syntax: Funktionsinstruktionen borde anv\u00e4nda en url av {0}"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_UTF8, "kodning underst\u00f6ds inte: {0}, anv\u00e4nder UTF-8"},
				new object[] {WG_ENCODING_NOT_SUPPORTED_USING_JAVA, "kodning underst\u00f6ds inte: {0}, anv\u00e4nder Java {1}"},
				new object[] {WG_SPECIFICITY_CONFLICTS, "Hittade specificitetskonflikter: {0} Senast hittade i stylesheet kommer att anv\u00e4ndas."},
				new object[] {WG_PARSING_AND_PREPARING, "========= Tolkar och f\u00f6rbereder {0} =========="},
				new object[] {WG_ATTR_TEMPLATE, "Attributmall, {0}"},
				new object[] {WG_CONFLICT_BETWEEN_XSLSTRIPSPACE_AND_XSLPRESERVESPACE, "Matcha konflikter mellan xsl:strip-space och xsl:preserve-space"},
				new object[] {WG_ATTRIB_NOT_HANDLED, "Xalan hanterar \u00e4nnu inte attributet {0}!"},
				new object[] {WG_NO_DECIMALFORMAT_DECLARATION, "Deklaration saknas f\u00f6r decimalformat: {0}"},
				new object[] {WG_OLD_XSLT_NS, "XSLT-Namnrymd saknas eller \u00e4r inkorrekt "},
				new object[] {WG_ONE_DEFAULT_XSLDECIMALFORMAT_ALLOWED, "Endast en standarddeklaration av xsl:decimal-format \u00e4r till\u00e5ten."},
				new object[] {WG_XSLDECIMALFORMAT_NAMES_MUST_BE_UNIQUE, "xsl:decimal-formatnamn m\u00e5ste vara unika. Namnet \"{0}\" har blivit duplicerat."},
				new object[] {WG_ILLEGAL_ATTRIBUTE, "{0} har ett otill\u00e5tet attribut: {1}"},
				new object[] {WG_COULD_NOT_RESOLVE_PREFIX, "Kan inte l\u00f6sa namnrymdsprefix: {0}. Noden kommer att ignoreras."},
				new object[] {WG_STYLESHEET_REQUIRES_VERSION_ATTRIB, "xsl:stylesheet m\u00e5ste ha ett 'version'-attribut!"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_NAME, "Otill\u00e5tet attributnamn: {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_VALUE, "Ogiltigt v\u00e4rde anv\u00e4nt f\u00f6r attribut {0}: {1}"},
				new object[] {WG_EMPTY_SECOND_ARG, "Den resulterande nodm\u00e4ngden fr\u00e5n dokumentfunktions andra argument \u00e4r tomt. Det f\u00f6rsta argumentet kommer att anv\u00e4ndas."},
				new object[] {WG_PROCESSINGINSTRUCTION_NAME_CANT_BE_XML, "V\u00e4rdet p\u00e5 attributet 'name' i xsl:processing-instruction f\u00e5r inte vara 'xml'"},
				new object[] {WG_PROCESSINGINSTRUCTION_NOTVALID_NCNAME, "V\u00e4rdet p\u00e5 attributet  ''name'' i xsl:processing-instruction m\u00e5ste vara ett giltigt NCName:a {0}"},
				new object[] {WG_ILLEGAL_ATTRIBUTE_POSITION, "Det g\u00e5r inte att l\u00e4gga till attributet {0} efter undernoder eller innan ett element produceras. Attributet ignoreras."},
				new object[] {"ui_language", "sv"},
				new object[] {"help_language", "sv"},
				new object[] {"language", "sv"},
				new object[] {"BAD_CODE", "Parameter till createMessage ligger utanf\u00f6r till\u00e5tet intervall"},
				new object[] {"FORMAT_FAILED", "Undantag utl\u00f6st vid messageFormat-anrop"},
				new object[] {"version", ">>>>>>> Xalan Version"},
				new object[] {"version2", "<<<<<<<"},
				new object[] {"yes", "ja"},
				new object[] {"line", "Rad #"},
				new object[] {"column", "Kolumn #"},
				new object[] {"xsldone", "XSLProcessor: f\u00e4rdig"},
				new object[] {"xslProc_option", "Xalan-J kommando linje Process klass alternativ:"},
				new object[] {"optionIN", "    -IN inputXMLURL"},
				new object[] {"optionXSL", "   [-XSL XSLTransformationURL]"},
				new object[] {"optionOUT", "   [-OUT utdataFilnamn]"},
				new object[] {"optionLXCIN", "   [-LXCIN kompileratStylesheetFilnameIn]"},
				new object[] {"optionLXCOUT", "   [-LXCOUT kompileratStylesheetFilenameUt]"},
				new object[] {"optionPARSER", "   [-PARSER fullt kvalificerat klassnamn eller tolkf\u00f6rbindelse]"},
				new object[] {"optionE", "   [-E (Ut\u00f6ka inte enhetsreferenser)]"},
				new object[] {"optionV", "   [-E (Ut\u00f6ka inte enhetsreferenser)]"},
				new object[] {"optionQC", "   [-QC (Tysta M\u00f6nsterkonfliktvarningar)]"},
				new object[] {"optionQ", "   [-Q  (Tyst Tillst\u00e5nd)]"},
				new object[] {"optionLF", "   [-LF (Anv\u00e4nd radframmatning enbart p\u00e5 utdata {standard \u00e4r CR/LF})]"},
				new object[] {"optionCR", "   [-CR (Anv\u00e4nd vagnretur enbart p\u00e5 utdata {standard \u00e4r CR/LF})]"},
				new object[] {"optionESCAPE", "   [-ESCAPE (Vilka tecken \u00e4r skiftningstecken {standard \u00e4r <>&\"\'\\r\\n}]"},
				new object[] {"optionINDENT", "   [-INDENT (Best\u00e4m antal blanksteg f\u00f6r att tabulera {standard \u00e4r 0})]"},
				new object[] {"optionTT", "   [-TT (Sp\u00e5ra mallarna allt eftersom de blir anropade.)]"},
				new object[] {"optionTG", "   [-TG (Sp\u00e5ra varje generationsh\u00e4ndelse.)]"},
				new object[] {"optionTS", "   [-TS (Sp\u00e5ra varje valh\u00e4ndelse.)]"},
				new object[] {"optionTTC", "   [-TTC (Sp\u00e5ra mallbarnen allt eftersom de blir behandlade.)]"},
				new object[] {"optionTCLASS", "   [-TCLASS (TraceListener-klass f\u00f6r sp\u00e5rningsanslutningar.)]"},
				new object[] {"optionVALIDATE", "   [-VALIDATE (S\u00e4tt om validering ska ske.  Standard \u00e4r att validering \u00e4r avst\u00e4ngd)]"},
				new object[] {"optionEDUMP", "   [-EDUMP {valfritt filnamn) (G\u00f6r stackdump vid fel.)]"},
				new object[] {"optionXML", "   [-XML (Anv\u00e4nd XML-formaterare och l\u00e4gg till XML-huvud.)]"},
				new object[] {"optionTEXT", "   [-XML (Anv\u00e4nd enkel Text-formaterare.)]"},
				new object[] {"optionHTML", "   [-HTML (Anv\u00e4nd HTML-formaterare)]"},
				new object[] {"optionPARAM", "   [-PARAM namn uttryck (S\u00e4tt en stylesheet-parameter)]"},
				new object[] {"noParsermsg1", "XSL-Process misslyckades."},
				new object[] {"noParsermsg2", "** Hittade inte tolk **"},
				new object[] {"noParsermsg3", "V\u00e4nligen kontrollera din classpath"},
				new object[] {"noParsermsg4", "Om du inte har IBMs XML-Tolk f\u00f6r Java, kan du ladda ner den fr\u00e5n"},
				new object[] {"noParsermsg5", "IBM's AlphaWorks: http://www.alphaworks.ibm.com/formula/xml"},
				new object[] {"optionURIRESOLVER", "   [-URIRESOLVER fullst\u00e4ndigt klassnamn (URIResolver som ska anv\u00e4ndas f\u00f6r att l\u00f6sa URI-er)]"},
				new object[] {"optionENTITYRESOLVER", "   [-ENTITYRESOLVER fullst\u00e4ndigt klassnamn (EntityResolver som ska anv\u00e4ndas f\u00f6r att l\u00f6sa enheter)]"},
				new object[] {"optionCONTENTHANDLER", "   [-CONTENTRESOLVER fullst\u00e4ndigt klassnamn (ContentHandler som ska anv\u00e4ndas f\u00f6r att serialisera utdata)]"},
				new object[] {"optionLINENUMBERS", "   [-L anv\u00e4nd radnummer i k\u00e4lldokument]"},
				new object[] {"optionMEDIA", " [-MEDIA mediaType (anv\u00e4nd medieattribut f\u00f6r att hitta en formatmall som \u00e4r associerad med ett dokument.)]"},
				new object[] {"optionFLAVOR", " [-FLAVOR flavorName (Anv\u00e4nd s2s=SAX eller d2d=DOM f\u00f6r transformationen.)] "},
				new object[] {"optionDIAG", " [-DIAG (Skriv ut totala transformationer, millisekunder.)]"},
				new object[] {"optionINCREMENTAL", " [-INCREMENTAL (beg\u00e4r inkrementell DTM-konstruktion genom att ange http://xml.apache.org/xalan/features/incremental true.)]"},
				new object[] {"optionNOOPTIMIMIZE", " [-NOOPTIMIMIZE (beg\u00e4r ingen formatmallsoptimering genom att ange http://xml.apache.org/xalan/features/optimize false.)]"},
				new object[] {"optionRL", " [-RL recursionlimit (kontrollera numerisk gr\u00e4ns p\u00e5 formatmallens rekursionsdjup.)]"},
				new object[] {"optionXO", " [-XO [transletName] (tilldela namnet till genererad translet)]"},
				new object[] {"optionXD", " [-XD destinationDirectory (ange m\u00e5lkatalog f\u00f6r translet)]"},
				new object[] {"optionXJ", " [-XJ jarfile (paketerar transletklasserna i en jar-fil med namnet <jarfile>)]"},
				new object[] {"optionXP", " [-XP-paket (anger ett paketnamnsprefix f\u00f6r alla genererade transletklasser)]"}
			};
		  }
	  }

	  // ================= INFRASTRUCTURE ======================

	  /// <summary>
	  /// String for use when a bad error code was encountered. </summary>
	  public new const string BAD_CODE = "D\u00c5LIG_KOD";

	  /// <summary>
	  /// String for use when formatting of the error string failed. </summary>
	  public new const string FORMAT_FAILED = "FORMATERING_MISSLYCKADES";

	  /// <summary>
	  /// General error string. </summary>
	  public new const string ERROR_STRING = "#fel";

	  /// <summary>
	  /// String to prepend to error messages. </summary>
	  public new const string ERROR_HEADER = "Fel: ";

	  /// <summary>
	  /// String to prepend to warning messages. </summary>
	  public new const string WARNING_HEADER = "Varning: ";

	  /// <summary>
	  /// String to specify the XSLT module. </summary>
	  public new const string XSL_HEADER = "XSLT ";

	  /// <summary>
	  /// String to specify the XML parser module. </summary>
	  public new const string XML_HEADER = "XML ";

	  /// <summary>
	  /// I don't think this is used any more. </summary>
	  /// @deprecated   
	  public new const string QUERY_HEADER = "M\u00d6NSTER ";

	}



}