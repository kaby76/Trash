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
 * $Id: ErrorMessages_no.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class ErrorMessages_no : ErrorMessages
	{

		// Disse feilmeldingene maa korrespondere med konstantene som er definert
		// i kildekoden til {ErrorMsg.
		/// <summary>
		/// Get the lookup table for error messages.   
		/// </summary>
		/// <returns> The message lookup table. </returns>
		public override object[][] Contents
		{
			get
			{
			  return new object[][]
			  {
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "En fil kan bare innehold ett stilark."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "<xsl:template> ''{0}'' er allerede definert i dette stilarket."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "<xsl:template> ''{0}'' er ikke definert i dette stilarket."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "Variabel ''{0}'' er allerede definert."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Variabel eller parameter ''{0}'' er ikke definert."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Finner ikke klassen ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Finner ikke ekstern funksjon ''{0}'' (m\u00e5 v\x0000e6re deklarert b\u00e5de 'static' og 'public')."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Kan ikke konvertere argument/retur type i kall til funksjon ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Finner ikke fil eller URI ''{0}''."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Ugyldig URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Kan ikke \u00e5pne fil eller URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Forvented <xsl:stylesheet> eller <xsl:transform> element."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Prefiks ''{0}'' er ikke deklarert."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Kunne ikke resolvere kall til funksjon ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "Argument til ''{0}'' m\u00e5 v\x0000e6re ordrett tekst."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Kunne ikke tolke XPath uttrykk ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "N\u00f8dvendig attributt ''{0}'' er ikke deklarert."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Ugyldig bokstav/tegn ''{0}'' i XPath uttrykk."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Ugyldig navn ''{0}'' for prosesserings-instruksjon."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Attributt ''{0}'' utenfor element."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Ugyldig attributt ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Sirkul \x0000e6 import/include; stilark ''{0}'' er alt lest."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Result-tre fragmenter kan ikke sorteres (<xsl:sort> elementer vil " + "bli ignorert). Du m\u00e5 sortere nodene mens du bygger treet."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Formatterings-symboler ''{0}'' er alt definert."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL versjon ''{0}'' er ikke st\u00f8ttet av XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Sirkul\x0000e6r variabel/parameter referanse i ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Ugyldig operator for bin\x0000e6rt uttrykk."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Ugyldig parameter i funksjons-kall."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Andre argument til document() m\u00e5 v\x0000e6re et node-sett."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Du m\u00e5 deklarere minst ett <xsl:when> element innenfor <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Kun ett <xsl:otherwise> element kan deklareres innenfor <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> kan kun benyttes innenfor <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> kan kun benyttes innenfor <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Kun <xsl:when> og <xsl:otherwise> kan benyttes innenfor <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> element manger 'name' attributt."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Ugyldig element."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "''{0}'' er ikke et gyldig navn for et element."},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "''{0}'' er ikke et gyldig navn for et attributt."},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Du kan ikke plassere tekst utenfor et <xsl:stylesheet> element."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP parser er ikke korrekt konfigurert."},
				  new object[] {ErrorMsg.INTERNAL_ERR, "XSLTC-intern feil: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "St\u00f8tter ikke XSL element ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "XSLTC st\u00f8tter ikke utvidet funksjon ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Dette dokumentet er ikke et XSL stilark " + "(xmlns:xsl='http://www.w3.org/1999/XSL/Transform' er ikke deklarert)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Kan ikke finne stilark ved navn ''{0}'' i dette dokumentet."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Ikke implementert/gjenkjent: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Dokumentet inneholder ikke et XSL stilark"},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Kan ikke tolke element ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "'use'-attributtet i <xsl:key> m\u00e5 v\x0000e6re node, node-sett, tekst eller nummer."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "Det genererte XML dokumentet m\u00e5 gis versjon 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Ugyldig operator for relasjons-uttrykk."},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Finner ikke <xsl:attribute-set> element med navn ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Kan ikke tolke attributt ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Ukjent data-type i signatur for klassen ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Kan ikke oversette mellom data-type ''{0}'' og ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Dette Templates objected inneholder ingen translet klasse definisjon."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Dette Templates objected inneholder ingen klasse ved navn ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Kan ikke laste translet-klasse ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Translet klassen er lastet man kan instansieres."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "ErrorListener for ''{0}'' fors\u00f8kt satt til 'null'."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Kun StreamSource, SAXSource og DOMSOurce er st\u00f8ttet av XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "Source objekt sendt til ''{0}'' har intet innhold."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Kan ikke kompilere stilark."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory gjenkjenner ikke attributtet ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() m\u00e5 kalles f\u00f8r startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer objektet inneholder ikken noen translet instans."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Ingen 'handler' er satt for \u00e5 ta imot generert dokument."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "Result objektet sendt til ''{0}'' er ikke gyldig."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Fors\u00f8ker \u00e5 lese ugyldig attributt ''{0}'' fra Transformer."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Kan ikke instansiere SAX2DOM adapter: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() kalt uten at 'systemId' er definert."},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "Du kan ikke bruke -i uten \u00e5 ogs\u00e5 angi klasse-navn med -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "Bruk:\n" + "   xsltc [-o <klasse>] [-d <katalog>] [-j <arkiv>]\n" + "         [-p <pakke>] [-x] [-s] [-u] <stilark>|-i\n\n" + "   Der:  <klasse> er navnet du vil gi den kompilerte java klassen.\n" + "         <stilark> er ett eller flere XSL stilark, eller dersom -u\n" + "         er benyttet, en eller flere URL'er til stilark.\n" + "         <katalog> katalog der klasse filer vil plasseres.\n" + "         <arkiv> er en JAR-fil der klassene vil plasseres\n" + "         <pakke> er an Java 'package' klassene vil legges i.\n\n" + "   Annet:\n" + "         -i tvinger kompilatoren til \u00e5 lese fra stdin.\n" + "         -o ignoreres dersom flere enn ett silark kompileres.\n" + "         -x sl\u00e5r p\u00e5 debug meldinger.\n" + "         -s blokkerer alle kall til System.exit()."},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "Bruk: \n" + "   xslt  [-j <arkiv>] {-u <url> | <dokument>} <klasse>\n" + "         [<param>=<verdi> ...]\n\n" + "   Der:  <dokument> er XML dokumentet som skal behandles.\n" + "         <url> er en URL til XML dokumentet som skal behandles.\n" + "         <klasse> er Java klassen som skal benyttes.\n" + "         <arkiv> er en JAR-fil som klassen leses fra.\n" + "   Annet:\n" + "         -x sl\u00e5r p\u00e5 debug meldinger.\n" + "         -s blokkerer alle kall til System.exit()."},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> kan bare brukes under <xsl:for-each> eller <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Karaktersett ''{0}'' er ikke st\u00f8ttet av denne JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Syntax error in ''{0}''."}
			  };
			}
		}
	}

}