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
 * $Id: ErrorMessages_sl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_sl : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "V isti datoteki je dolo\u010denih ve\u010d slogovnih datotek."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "Predloga ''{0}'' je \u017ee dolo\u010dena v tej slogovni datoteki."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "Predloga ''{0}'' ni dolo\u010dena v tej slogovni datoteki."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "Spremenljivka ''{0}'' je ve\u010dkrat dolo\u010dena znotraj istega obsega."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Spremenljivka ali parameter ''{0}'' je nedolo\u010den."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Ni mogo\u010de najti razreda ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Zunanje metode ''{0}'' ni mogo\u010de najti (mora biti javna)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Ni mogo\u010de pretvoriti argumenta / tipa vrnitve v klicu metode ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Datoteke ali URI-ja ''{0}'' ni mogo\u010de najti."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Neveljaven URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Datoteke ali URI-ja ''{0}'' ni mogo\u010de odpreti."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Pri\u010dakovan element <xsl:stylesheet> ali <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Predpona imenskega prostora ''{0}'' ni dolo\u010dena."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Klica funkcije ''{0}'' ni mogo\u010de razre\u0161iti."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "Argument za ''{0}'' mora biti dobesedni niz."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Napaka pri raz\u010dlenjevanju izraza XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Manjka zahtevani atribut ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Neveljavni znak ''{0}'' v izrazu XPath."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Neveljavno ime ''{0}'' za navodila za obdelavo."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Atribut ''{0}'' zunaj elementa."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Neveljaven atribut ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Kro\u017eni uvoz/vklju\u010ditev. Slogovna datoteka ''{0}'' je \u017ee nalo\u017eena."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Ne morem razvrstiti fragmentov drevesa rezultatov (elementi <xsl:sort> so prezrti). Pri pripravi drevesa rezultatov morate razvrstiti vozli\u0161\u010da."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Decimalno oblikovanje ''{0}'' je \u017ee dolo\u010deno."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL razli\u010dice ''{0}'' XSLTC ne podpira."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Sklic na kro\u017eno spremenljivko/parameter v ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Neznan operator za binarni izraz."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Neveljavni argument(i) za klic funkcije."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Drugi argument funkcije document() mora biti skupina vozli\u0161\u010d."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Zahtevan vsaj en element <xsl:when> v <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Dovoljen samo en element <xsl:otherwise> v <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> lahko uporabljamo samo znotraj <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> lahko uporabljamo samo znotraj <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "V <xsl:choose> sta dovoljena samo elementa <xsl:when> in <xsl:otherwise>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "V <xsl:attribute-set> manjka atribut 'name'."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Neveljavni podrejeni element."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Elementa ''{0}'' ni mogo\u010de poklicati"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Atributa ''{0}'' ni mogo\u010de poklicati"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Tekstovni podatki zunaj elementa na najvi\u0161ji ravni <xsl:stylesheet>."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Raz\u010dlenjevalnik JAXP ni pravilno konfiguriran"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Nepopravljiva XSLTC-notranja napaka: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Nepodprt XSL element ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Neprepoznana pripona XSLTC ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Vhodni dokument ni slogovna datoteka (v korenskem elementu ni naveden imenski prostor XSL)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Cilja slogovne datoteke ''{0}'' ni mogo\u010de najti."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Ni izvedeno: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Vhodni dokument ne vsebuje slogovne datoteke XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Elementa ''{0}'' ni mogo\u010de raz\u010dleniti"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "Atribut uporabe za <key> mora biti vozli\u0161\u010de, skupina vozli\u0161\u010d, niz ali \u0161tevilka."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "Razli\u010dica izhodnega dokumenta XML mora biti 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Neznan operator za relacijski izraz"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Poskus uporabe neobstoje\u010de skupine atributov ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Predloge vrednosti atributa ''{0}'' ni mogo\u010de raz\u010dleniti."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "V podpisu za razred ''{0}'' je neznan podatkovni tip."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Ni mogo\u010de pretvoriti podatkovnega tipa ''{0}'' v ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Te Templates ne vsebujejo veljavne definicije razreda translet."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Te predloge ne vsebujejo razreda z imenom ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Ni mogo\u010de nalo\u017eiti razreda transleta ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Razred transleta je nalo\u017een, vendar priprava primerka transleta ni mogo\u010da."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Poskus nastavitve ErrorListener za ''{0}'' na null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC podpira samo StreamSource, SAXSource in DOMSource"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "Predmet Source, ki je bil podan z ''{0}'', nima vsebine."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Ni mogo\u010de prevesti slogovne datoteke"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory ne prepozna atributa ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "Klic za setResult() mora biti izveden pred klicem startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer ne vsebuje enkapsuliranih translet objektov."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Za rezultat transformacije ni izhodne obravnave."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "Rezultat, ki je bil posredovan ''{0}'', je neveljaven."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Poskus dostopa do neveljavne lastnosti (property) Transformer ''{0}''."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Ni mogo\u010de ustvariti adapterja SAX2DOM: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "Klic XSLTCSource.build() brez predhodne nastavitve systemId."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Rezultat naj ne bi bil NULL"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "Vrednost parametra {0} mora biti veljaven javanski objekt"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "Mo\u017enost -i mora biti uporabljena skupaj z mo\u017enostjo -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <izhodna datoteka>]\n      [-d <directory>] [-j  <datoteka jar>] [-p <paket>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\nOPTIONS\n   -o <izhodna datoteka>    dodeli ime <izhodna datoteka> generiranemu\n                  transletu.  Ime transleta se po privzetih nastavitvah\n                 izpelje iz imena <stylesheet>.  Pri prevajanju\n                  ve\u010d slogovnih datotek je ta mo\u017enost prezrta.\n   -d <directory> dolo\u010di ciljno mapo za translet\n   -j <datoteka jar>   zdru\u017ei razrede translet v datoteko jar\n       pod imenom, dolo\u010denim z <datoteka jar>\n   -p <paket>   dolo\u010di predpono imena paketa vsem generiranim\n               razredom translet.\n   -n             omogo\u010da vrivanje predlog (v povpre\u010dju bolj\u0161e privzeto\n                      obna\u0161anje).\n   -x             vklopi dodatna izhodna sporo\u010dila za iskanje napak\n   -u             prevede argumente <stylesheet> kot URL-je\n   -i             prisili prevajalnik k branju slogovne datoteke iz stdin\n   -v             natisne razli\u010dico prevajalnika\n   -h             natisne ta stavek za uporabo\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <datoteka jar>]\n      [-x] [-n <ponovitve>] {-u <document_url> | <dokument>}\n      <razred> [<param1>=<value1> ...]\n\n   uporablja translet <razred> za pretvorbo dokumenta XML \n   navedenega kot <dokument>. Translet <razred> je ali v\n   uporabnikovem CLASSPATH ali v izbirno navedeni datoteki <datoteka jar>.\nOPTIONS\n   -j <datoteka jar>    dolo\u010di datoteko jar, iz katere bo nalo\u017een translet\n   -x               vklopi dodatna sporo\u010dila za iskanje napak\n   n  <ponovitve> <ponovitve>-krat po\u017eene preoblikovanje in\n                   prika\u017ee informacije profiliranja\n   -u <document_url> dolo\u010di vhodni dokument XML za URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> je mogo\u010de uporabljati samo znotraj <xsl:for-each> ali <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Ta JVM ne podpira izhodnega kodiranja ''{0}''."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Napaka v sintaksi v ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Ni mogo\u010de najti zunanjega konstruktorja ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Prvi argument nestati\u010dne (non-static) javanske funkcije ''{0}'' ni veljaven sklic na objekt."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Napaka pri preverjanju tipa izraza ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Napaka pri preverjanju tipa izraza na neznani lokaciji."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "Mo\u017enost ukazne vrstice ''{0}'' ni veljavna."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "Mo\u017enosti ukazne vrstice ''{0}'' manjka zahtevani argument."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "OPOZORILO:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "OPOZORILO:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "USODNA NAPAKA:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "USODNA NAPAKA:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "USODNA NAPAKA:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "NAPAKA:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Pretvorba z uporabo transleta ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Pretvorba z uporabo transleta ''{0}'' iz datoteke jar ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Ni mogo\u010de ustvariti primerka razreda TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "Imena ''{0}'' ni bilo mogo\u010de uporabiti kot ime razreda translet, saj vsebuje znake, ki v imenu javanskega razreda niso dovoljeni.  Uporabljeno je bilo ime ''{1}''."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Napake prevajalnika:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Opozorila prevajalnika:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Napake transleta:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Atribut, katerega vrednost mora biti vrednost QName ali s presledki lo\u010den seznam vrednosti Qname, je imel vrednost ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Atribut, katerega vrednost mora biti NCName, je imel vrednost ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "Atribut metode elementa <xsl:output> je imel vrednost ''{0}''.  Vrednost mora biti ena izmed ''xml'', ''html'', ''text'', ali qname-but-not-ncname (qname, vendar pa ne ncname)"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "Ime funkcije ne sme biti null v TransformerFactory.getFeature(Ime niza)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "Ime funkcije ne sme biti null v TransformerFactory.getFeature(Ime niza, boolova vrednost)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Ni mogo\u010de nastaviti funkcije ''{0}'' v tem TransformerFactory."}
			  };
			}
		}

	}

}