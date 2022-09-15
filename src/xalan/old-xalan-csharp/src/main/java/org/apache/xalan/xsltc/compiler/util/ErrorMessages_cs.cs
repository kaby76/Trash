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
 * $Id: ErrorMessages_cs.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_cs : ListResourceBundle
	{

	/*
	 * XSLTC compile-time error messages.
	 *
	 * General notes to translators and definitions:
	 *
	 *   1) XSLTC is the name of the product.  It is an acronym for "XSLT Compiler".
	 *      XSLT is an acronym for "XML Stylesheet Language: Transformations".
	 *
	 *   2) A stylesheet is a description of how to transform an input XML document
	 *      into a resultant XML document (or HTML document or text).  The
	 *      stylesheet itself is described in the form of an XML document.
	 *
	 *   3) A template is a component of a stylesheet that is used to match a
	 *      particular portion of an input document and specifies the form of the
	 *      corresponding portion of the output document.
	 *
	 *   4) An axis is a particular "dimension" in a tree representation of an XML
	 *      document; the nodes in the tree are divided along different axes.
	 *      Traversing the "child" axis, for instance, means that the program
	 *      would visit each child of a particular node; traversing the "descendant"
	 *      axis means that the program would visit the child nodes of a particular
	 *      node, their children, and so on until the leaf nodes of the tree are
	 *      reached.
	 *
	 *   5) An iterator is an object that traverses nodes in a tree along a
	 *      particular axis, one at a time.
	 *
	 *   6) An element is a mark-up tag in an XML document; an attribute is a
	 *      modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *      "elem" is an element name, "attr" and "attr2" are attribute names with
	 *      the values "val" and "val2", respectively.
	 *
	 *   7) A namespace declaration is a special attribute that is used to associate
	 *      a prefix with a URI (the namespace).  The meanings of element names and
	 *      attribute names that use that prefix are defined with respect to that
	 *      namespace.
	 *
	 *   8) DOM is an acronym for Document Object Model.  It is a tree
	 *      representation of an XML document.
	 *
	 *      SAX is an acronym for the Simple API for XML processing.  It is an API
	 *      used inform an XML processor (in this case XSLTC) of the structure and
	 *      content of an XML document.
	 *
	 *      Input to the stylesheet processor can come from an XML parser in the
	 *      form of a DOM tree or through the SAX API.
	 *
	 *   9) DTD is a document type declaration.  It is a way of specifying the
	 *      grammar for an XML file, the names and types of elements, attributes,
	 *      etc.
	 *
	 *  10) XPath is a specification that describes a notation for identifying
	 *      nodes in a tree-structured representation of an XML document.  An
	 *      instance of that notation is referred to as an XPath expression.
	 *
	 *  11) Translet is an invented term that refers to the class file that contains
	 *      the compiled form of a stylesheet.
	 */

		// These message should be read from a locale-specific resource bundle
		/// <summary>
		/// Get the lookup table for error messages.
		/// </summary>
		/// <returns> The message lookup table. </returns>
		public virtual object[][] Contents
		{
			get
			{
			  return new object[][]
			  {
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "V\u00edce ne\u017e jedna p\u0159edloha stylu je definov\u00e1na ve stejn\u00e9m souboru."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "\u0160ablona ''{0}'' je ji\u017e v t\u00e9to p\u0159edloze stylu definov\u00e1na. "},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "\u0160ablona ''{0}'' nen\u00ed v t\u00e9to p\u0159edloze stylu definov\u00e1na."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "Prom\u011bnn\u00e1 ''{0}'' je n\u011bkolikan\u00e1sobn\u011b definov\u00e1na ve stejn\u00e9m oboru."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Prom\u011bnn\u00e1 nebo parametr ''{0}'' nejsou definov\u00e1ny."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Nelze naj\u00edt t\u0159\u00eddu ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Nelze naj\u00edt extern\u00ed metodu ''{0}'' (mus\u00ed b\u00fdt ve\u0159ejn\u00e1)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Nelze p\u0159ev\u00e9st argument/n\u00e1vratov\u00fd typ ve vol\u00e1n\u00ed metody ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Soubor nebo identifik\u00e1tor URI ''{0}'' nebyl nalezen."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Neplatn\u00fd identifik\u00e1tor URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Nelze otev\u0159\u00edt soubor nebo identifik\u00e1tor URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Byl o\u010dek\u00e1v\u00e1n prvek <xsl:stylesheet> nebo <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "P\u0159edpona oboru n\u00e1zv\u016f ''{0}'' nen\u00ed deklarov\u00e1na."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Nelze vy\u0159e\u0161it vol\u00e1n\u00ed funkce ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "Argumentem funkce ''{0}'' mus\u00ed b\u00fdt \u0159et\u011bzcov\u00fd liter\u00e1l."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Chyba p\u0159i anal\u00fdze v\u00fdrazu XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Po\u017eadovan\u00fd atribut ''{0}'' chyb\u00ed."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Neplatn\u00fd znak ''{0}'' ve v\u00fdrazu XPath."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Neplatn\u00fd n\u00e1zev ''{0}'' pro instrukci zpracov\u00e1n\u00ed. "},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Atribut ''{0}'' se nach\u00e1z\u00ed vn\u011b prvku."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Neplatn\u00fd atribut ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Cyklick\u00fd import/zahrnut\u00ed. P\u0159edloha stylu ''{0}'' je ji\u017e zavedena."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Fragmenty stromu v\u00fdsledk\u016f nemohou b\u00fdt \u0159azeny (prvky <xsl:sort> se ignoruj\u00ed). P\u0159i vytv\u00e1\u0159en\u00ed stromu v\u00fdsledk\u016f mus\u00edte se\u0159adit uzly."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Desetinn\u00e9 form\u00e1tov\u00e1n\u00ed ''{0}'' je ji\u017e definov\u00e1no."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "Verze XSL ''{0}'' nen\u00ed produktem XSLTC podporov\u00e1na."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Cyklick\u00fd odkaz na prom\u011bnnou/parametr ve funkci ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Nezn\u00e1m\u00fd oper\u00e1tor pro bin\u00e1rn\u00ed v\u00fdraz."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Neplatn\u00fd argument pro vol\u00e1n\u00ed funkce."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Druh\u00fd argument pro funkci document() mus\u00ed b\u00fdt node-set."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Alespo\u0148 jeden prvek <xsl:when> se vy\u017eaduje v <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Jen jeden prvek <xsl:otherwise> je povolen v <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "Prvek <xsl:otherwise> m\u016f\u017ee b\u00fdt pou\u017eit jen v <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "Prvek <xsl:when> m\u016f\u017ee b\u00fdt pou\u017eit jen v <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Pouze prvky <xsl:when> a <xsl:otherwise> jsou povoleny v <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "V prvku <xsl:attribute-set> chyb\u00ed atribut 'name'."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Neplatn\u00fd prvek potomka."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Nelze volat prvek ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Nelze volat atribut ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Textov\u00e1 data jsou vn\u011b prvku nejvy\u0161\u0161\u00ed \u00farovn\u011b <xsl:stylesheet>."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Analyz\u00e1tor JAXP je nespr\u00e1vn\u011b konfigurov\u00e1n."},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Neopraviteln\u00e1 intern\u00ed chyba XSLTC: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Nepodporovan\u00fd prvek XSL ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Nerozpoznan\u00e1 p\u0159\u00edpona XSLTC ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Vstupn\u00ed dokument nen\u00ed p\u0159edloha stylu (obor n\u00e1zv\u016f XSL nen\u00ed deklarov\u00e1n v ko\u0159enov\u00e9m elementu)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Nelze naj\u00edt c\u00edlovou p\u0159edlohu se stylem ''{0}''."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Neimplementov\u00e1no: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Vstupn\u00ed dokument neobsahuje p\u0159edlohu stylu XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Nelze analyzovat prvek ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "Atribut use prom\u011bnn\u00e9 <key> mus\u00ed b\u00fdt typu node, node-set, string nebo number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "V\u00fdstupn\u00ed verze dokumentu XML by m\u011bla b\u00fdt 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Nezn\u00e1m\u00fd oper\u00e1tor pro rela\u010dn\u00ed v\u00fdraz"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Pokus pou\u017e\u00edt neexistuj\u00edc\u00ed sadu atribut\u016f ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Nelze analyzovat \u0161ablonu hodnoty atributu ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Nezn\u00e1m\u00fd datov\u00fd typ v podpisu pro t\u0159\u00eddu ''{0}''. "},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Nelze p\u0159ev\u00e9st datov\u00fd typ ''{0}'' na ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Tato \u0161ablona neobsahuje platnou definici t\u0159\u00eddy translet."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Tato \u0161ablona neobsahuje t\u0159\u00eddu s n\u00e1zvem ''{0}''. "},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Nelze zav\u00e9st t\u0159\u00eddu translet ''{0}''. "},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "T\u0159\u00edda translet byla zavedena, av\u0161ak nelze vytvo\u0159it instanci translet."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Pokus nastavit objekt ErrorListener pro t\u0159\u00eddu ''{0}'' na hodnotu Null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Pouze prom\u011bnn\u00e9 StreamSource, SAXSource a DOMSource jsou podporov\u00e1ny produktem XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "Zdrojov\u00fd objekt p\u0159edan\u00fd metod\u011b ''{0}'' nem\u00e1 \u017e\u00e1dn\u00fd obsah."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Nelze kompilovat p\u0159edlohu se stylem"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "T\u0159\u00edda TransformerFactory nerozpoznala atribut ''{0}''. "},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "Metoda setResult() mus\u00ed b\u00fdt vol\u00e1na p\u0159ed metodou startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Objekt Transformer nem\u00e1 \u017e\u00e1dn\u00fd zapouzd\u0159en\u00fd objekt translet."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Neexistuje \u017e\u00e1dn\u00fd definovan\u00fd v\u00fdstupn\u00ed obslu\u017en\u00fd program pro v\u00fdsledek transformace."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "V\u00fdsledn\u00fd objekt p\u0159edan\u00fd metod\u011b ''{0}'' je neplatn\u00fd."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Pokus o p\u0159\u00edstup k neplatn\u00e9 vlastnosti objektu Transformer: ''{0}''. "},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Nelze vytvo\u0159it adapt\u00e9r SAX2DOM: ''{0}''. "},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "Byla vol\u00e1na metoda XSLTCSource.build(), ani\u017e by byla nastavena hodnota systemId."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "V\u00fdsledek by nem\u011bl m\u00edt hodnotu null"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "Hodnota parametru {0} mus\u00ed b\u00fdt platn\u00fdm objektem technologie Java."},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "Volba -i mus\u00ed b\u00fdt pou\u017eita s volbou -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSE\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <v\u00fdstup>]\n      [-d <adres\u00e1\u0159>] [-j <soubor_jar>] [-p <bal\u00edk>]\n      [-n] [-x] [-u] [-v] [-h] { <p\u0159edloha_stylu> | -i }\n\nVOLBY\n   -o <v\u00fdstup>    p\u0159i\u0159ad\u00ed k vygenerovan\u00e9mu transletu n\u00e1zev <v\u00fdstup>. \n                  Podle v\u00fdchoz\u00edho nastaven\u00ed je jm\u00e9no transletu\n                  odvozeno z n\u00e1zvu <p\u0159edloha_stylu>. Tato volba\n                  se ignoruje, pokud se kompiluj\u00ed n\u00e1sobn\u00e9 p\u0159edlohy styl\u016f.\n   -d <adres\u00e1\u0159>  ur\u010duje v\u00fdchoz\u00ed adres\u00e1\u0159 pro translet\n   -j <soubor_jar> zabal\u00ed t\u0159\u00eddu transletu do souboru jar\n                  pojmenovan\u00e9ho jako <soubor_jar>\n   -p <bal\u00edk>     ur\u010duje p\u0159edponu n\u00e1zvu bal\u00ed\u010dku pro v\u0161echny generovan\u00e9 \n                  t\u0159\u00eddy transletu.\n   -n             povoluje zarovn\u00e1n\u00ed \u0161ablony (v\u00fdchoz\u00ed chov\u00e1n\u00ed je v pr\u016fm\u011bru lep\u0161\u00ed)\n   -x             zapne dal\u0161\u00ed v\u00fdstup zpr\u00e1vy lad\u011bn\u00ed\n   -u             interpretuje  argumenty <p\u0159edloha_stylu> jako URL \n   -i             vynut\u00ed \u010dten\u00ed p\u0159edlohy styl\u016f kompil\u00e1torem ze vstupu stdin\n   -v             tiskne verzi kompil\u00e1toru\n   -h             tiskne tyto pokyny k pou\u017eit\u00ed\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSE\n   java org.apache.xalan.xsltc.cmdline.Transform [-j <soubor_jar>]\n      [-x] [-n <iterace>] {-u <URL_dokumentu> | <dokument>}\n         <t\u0159\u00edda> [<param1>=<hodn1>...]\n\n   Pou\u017e\u00edv\u00e1 translet <t\u0159\u00edda> k transformaci dokumentu XML \n   ur\u010den\u00e9ho parametrem <dokument>. Translet <t\u0159\u00edda> je bu\u010f\n   v u\u017eivatelsk\u00e9 cest\u011b CLASSPATH, nebo ve voliteln\u011b ur\u010den\u00e9m souboru <soubor_jar>.\nVOLBY\n   -j <soubor_jar>    ur\u010duje soubor jar, z n\u011bj\u017e m\u00e1 b\u00fdt translet na\u010dten\n   -x                 zap\u00edn\u00e1 v\u00fdstup dal\u0161\u00edch ladic\u00edch zpr\u00e1v\n   -n <iterace>       spou\u0161t\u00ed transformaci opakovan\u011b, parametr <iterace> ur\u010duje po\u010det opakov\u00e1n\u00ed,\n                      a zobraz\u00ed informace o profilu\n   -u <URL_dokumentu> ur\u010duje adresu URL vstupn\u00edho dokumentu XML\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "Prvek <xsl:sort> m\u016f\u017ee b\u00fdt pou\u017eit jen v <xsl:for-each> nebo <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "V\u00fdstupn\u00ed k\u00f3dov\u00e1n\u00ed ''{0}'' nen\u00ed v tomto prost\u0159ed\u00ed JVM podporov\u00e1no."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Chyba syntaxe ve v\u00fdrazu ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Nelze naj\u00edt extern\u00ed konstruktor ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Prvn\u00ed argument nestatick\u00e9 funkce Java ''{0}'' nen\u00ed platn\u00fdm odkazem na objekt."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Chyba p\u0159i kontrole typu v\u00fdrazu ''{0}''. "},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Chyba p\u0159i kontrole typu v\u00fdrazu na nezn\u00e1m\u00e9m m\u00edst\u011b."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "Volba p\u0159\u00edkazov\u00e9ho \u0159\u00e1dku ''{0}'' nen\u00ed platn\u00e1."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "Ve volb\u011b p\u0159\u00edkazov\u00e9ho \u0159\u00e1dku ''{0}'' chyb\u00ed po\u017eadovan\u00fd argument."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "VAROV\u00c1N\u00cd: ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "VAROV\u00c1N\u00cd: ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "Z\u00c1VA\u017dN\u00c1 CHYBA:''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "Z\u00c1VA\u017dN\u00c1 CHYBA:''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "CHYBA:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "CHYBA:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Transformace s pou\u017eit\u00edm transletu ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Transformace s pou\u017eit\u00edm transletu ''{0}'' ze souboru JAR ''{1}'' "},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Nelze vytvo\u0159it instanci t\u0159\u00eddy TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "Jm\u00e9no ''{0}'' nelze pou\u017e\u00edt jako jm\u00e9no t\u0159\u00eddy translet\u016f, proto\u017ee obsahuje znaky, kter\u00e9 nejsou ve jm\u00e9nu t\u0159\u00eddy jazyka Java povoleny. Pou\u017eito bylo jm\u00e9no ''{1}''."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Chyby kompil\u00e1toru:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Varov\u00e1n\u00ed kompil\u00e1toru:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Chyby transletu:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Atribut, jeho\u017e hodnota mus\u00ed b\u00fdt jm\u00e9no QName nebo seznam jmen QName odd\u011blen\u00fdch mezerami, m\u00e1 hodnotu ''{0}''. "},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Atribut, jeho\u017e hodnota mus\u00ed b\u00fdt jm\u00e9no NCName, m\u00e1 hodnotu ''{0}''. "},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "Atribut metody prvku <xsl:output> m\u00e1 hodnotu ''{0}''. Hodnotou mus\u00ed b\u00fdt \u0159et\u011bzec ''xml'', ''html'', ''text'' nebo jm\u00e9no QName, kter\u00e9 nen\u00ed jm\u00e9nem NCName."},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "N\u00e1zev funkce pou\u017eit\u00fd ve vol\u00e1n\u00ed TransformerFactory.getFeature(\u0159et\u011bzec n\u00e1zvu) nesm\u00ed m\u00edt hodnotu Null. "},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "N\u00e1zev funkce pou\u017eit\u00fd ve vol\u00e1n\u00ed TransformerFactory.setFeature(\u0159et\u011bzec n\u00e1zvu, booleovsk\u00e1 hodnota) nesm\u00ed m\u00edt hodnotu Null. "},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Nelze nastavit funkci ''{0}'' pro tuto t\u0159\u00eddu TransformerFactory. "}
			  };
			}
		}

	}

}