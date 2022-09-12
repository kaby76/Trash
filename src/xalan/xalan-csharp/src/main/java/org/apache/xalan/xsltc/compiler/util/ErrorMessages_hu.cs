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
 * $Id: ErrorMessages_hu.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_hu : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "Egyn\u00e9l t\u00f6bb st\u00edluslap van meghat\u00e1rozva ugyanabban a f\u00e1jlban."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "A(z) ''{0}'' sablon m\u00e1r meg van hat\u00e1rozva ebben a st\u00edluslapban."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "A(z) ''{0}'' sablon nincs meghat\u00e1rozva ebben a st\u00edluslapban."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "A(z) ''{0}'' v\u00e1ltoz\u00f3 t\u00f6bbsz\u00f6r van meghat\u00e1rozva ugyanabban a hat\u00f3k\u00f6rben."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "A(z) ''{0}'' v\u00e1ltoz\u00f3 vagy param\u00e9ter nincs meghat\u00e1rozva."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "A(z) ''{0}'' oszt\u00e1ly nem tal\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Nem tal\u00e1lhat\u00f3 a(z) ''{0}'' k\u00fcls\u0151 met\u00f3dus (nyilv\u00e1nosnak kell lennie)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Nem lehet \u00e1talak\u00edtani az argumentum/visszat\u00e9r\u00e9si t\u00edpust a(z) ''{0}'' met\u00f3dus h\u00edv\u00e1s\u00e1ban."},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "A(z) ''{0}'' f\u00e1jl vagy URI nem tal\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "\u00c9rv\u00e9nytelen URI: ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "A(z) ''{0}'' f\u00e1jlt vagy URI nem nyithat\u00f3 meg. "},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Hi\u00e1nyzik az <xsl:stylesheet> vagy <xsl:transform> elem."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "A(z) ''{0}'' n\u00e9vt\u00e9r el\u0151tag nincs deklar\u00e1lva."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Nem lehet feloldani a(z) ''{0}'' f\u00fcggv\u00e9ny h\u00edv\u00e1s\u00e1t."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "A(z) ''{0}'' argumentum\u00e1nak egy liter\u00e1l karaktersorozatnak kell lennie."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Hiba t\u00f6rt\u00e9nt a(z) ''{0}'' XPath kifejez\u00e9s \u00e9rtelmez\u00e9sekor."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Hi\u00e1nyzik a(z) ''{0}'' k\u00f6telez\u0151 attrib\u00fatum."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Nem megengedett karakter (''{0}'') szerepel az XPath kifejez\u00e9sben."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Nem megengedett n\u00e9v (''{0}'') szerepel a feldolgoz\u00e1si utas\u00edt\u00e1sban."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "A(z) ''{0}'' attrib\u00fatum k\u00edv\u00fcl esik az elemen."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Illeg\u00e1lis attrib\u00fatum: ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "K\u00f6rk\u00f6r\u00f6s import\u00e1l\u00e1s/tartalmaz\u00e1s. A(z) ''{0}'' st\u00edluslap m\u00e1r be van t\u00f6ltve."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Az eredm\u00e9nyfa-r\u00e9szleteket nem lehet rendezni (az <xsl:sort> elemek figyelmen k\u00edv\u00fcl maradnak). Rendeznie kell a node-okat, amikor eredm\u00e9nyf\u00e1t hoz l\u00e9tre."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "M\u00e1r defini\u00e1lva van a(z) ''{0}'' decim\u00e1lis form\u00e1z\u00e1s."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "Az XSLTC nem t\u00e1mogatja a(z) ''{0}'' XSL verzi\u00f3t."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "K\u00f6rk\u00f6r\u00f6s v\u00e1ltoz\u00f3/param\u00e9ter hivatkoz\u00e1s a(z) ''{0}'' helyen."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Ismeretlen oper\u00e1tort haszn\u00e1lt a bin\u00e1ris kifejez\u00e9sben."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Nem megengedett argumentumo(ka)t haszn\u00e1lt a f\u00fcggv\u00e9nyh\u00edv\u00e1sban."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "A document() f\u00fcggv\u00e9ny m\u00e1sodik argumentuma egy node-k\u00e9szlet kell legyen."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Legal\u00e1bb egy <xsl:when> elem sz\u00fcks\u00e9ges az <xsl:choose>-ban."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Csak egy <xsl:otherwise> elem megengedett <xsl:choose>-ban."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "Az <xsl:otherwise> csak <xsl:choose>-on bel\u00fcl haszn\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "Az <xsl:when> csak <xsl:choose>-on bel\u00fcl haszn\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Csak <xsl:when> \u00e9s <xsl:otherwise> elemek megengedettek az <xsl:choose>-ban."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "Hi\u00e1nyzik az <xsl:attribute-set>-b\u0151l a 'name' attrib\u00fatum."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Nem megengedett gyermek elem."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Az elem neve nem lehet ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Az attrib\u00fatum neve nem lehet ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Sz\u00f6vegadat szerepel a fels\u0151 szint\u0171 <xsl:stylesheet> elemen k\u00edv\u00fcl."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Nincs megfelel\u0151en konfigur\u00e1lva a JAXP \u00e9rtelmez\u0151."},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Helyrehozhatatlan bels\u0151 XSLTC hiba t\u00f6rt\u00e9nt: ''{0}''  "},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Nem t\u00e1mogatott XSL elem: ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Ismeretlen XSLTC kiterjeszt\u00e9s: ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "A bemen\u0151 dokumentum nem st\u00edluslap (az XSL n\u00e9vt\u00e9r nincs deklar\u00e1lva a root elemben)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "A(z) ''{0}'' st\u00edluslap c\u00e9l nem tal\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Nincs megval\u00f3s\u00edtva: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "A bemen\u0151 dokumentum nem tartalmaz XSL st\u00edluslapot."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "A(z) ''{0}'' elem nem \u00e9rtelmezhet\u0151. "},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "A(z) <key> attrib\u00fatuma node, node-k\u00e9szlet, sz\u00f6veg vagy sz\u00e1m lehet."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "A kimen\u0151 XML dokumentum-verzi\u00f3 1.0 kell legyen."},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Ismeretlen oper\u00e1tort haszn\u00e1lt a rel\u00e1ci\u00f3s kifejez\u00e9sben."},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Neml\u00e9tez\u0151 attrib\u00fatumk\u00e9szletet (''{0}'') pr\u00f3b\u00e1lt haszn\u00e1lni."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Nem lehet \u00e9rtelmezni a(z) ''{0}'' attrib\u00fatum\u00e9rt\u00e9k-sablont."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Ismeretlen adatt\u00edpus szerepel a(z) ''{0}'' oszt\u00e1ly al\u00e1\u00edr\u00e1s\u00e1ban."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Nem lehet a(z) ''{0}'' adatt\u00edpust ''{1}'' t\u00edpusra konvert\u00e1lni."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Ez a Templates oszt\u00e1ly nem tartalmaz \u00e9rv\u00e9nyes translet oszt\u00e1lymeghat\u00e1roz\u00e1st."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Ez a Templates oszt\u00e1ly nem tartalmaz ''{0}'' nev\u0171 oszt\u00e1lyt."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Nem lehet bet\u00f6lteni a(z) ''{0}'' translet oszt\u00e1lyt."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "A translet oszt\u00e1ly bet\u00f6lt\u0151d\u00f6tt, de nem siker\u00fclt l\u00e9trehozni a translet p\u00e9ld\u00e1nyt."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Megpr\u00f3b\u00e1lta null\u00e9rt\u00e9kre \u00e1ll\u00edtani a(z) ''{0}'' objektum ErrorListener fel\u00fclet\u00e9t."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Az XSLTC csak a StreamSource, SAXSource \u00e9s DOMSource interf\u00e9szeket t\u00e1mogatja."},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "A(z) ''{0}''  met\u00f3dusnak \u00e1tadott source objektumnak nincs tartalma."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Nem siker\u00fclt leford\u00edtani a st\u00edluslapot."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "A TransformerFactory oszt\u00e1ly nem simeri fel a(z) ''{0}'' attrib\u00fatumot."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "A setResult() met\u00f3dust a startDocument() h\u00edv\u00e1sa el\u0151tt kell megh\u00edvni."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "A transformer interf\u00e9sz nem tartalmaz be\u00e1gyazott translet objektumot."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Nincs defini\u00e1lva kimenetkezel\u0151 az \u00e1talak\u00edt\u00e1s eredm\u00e9ny\u00e9hez."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "A(z) ''{0}'' met\u00f3dusnak \u00e1tadott result objektum \u00e9rv\u00e9nytelen."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "\u00c9rv\u00e9nytelen Transformer tulajdons\u00e1got (''{0}'') pr\u00f3b\u00e1lt meg el\u00e9rni."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Nem lehet l\u00e9trehozni a SAX2DOM adaptert: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() h\u00edv\u00e1sa systemId be\u00e1ll\u00edt\u00e1sa n\u00e9lk\u00fcl t\u00f6rt\u00e9nt."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Az eredm\u00e9ny nem lehet null"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "A(z) {0} param\u00e9ter \u00e9rt\u00e9ke egy \u00e9rv\u00e9nyes J\u00e1va objektum kell legyen"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "A -i kapcsol\u00f3t a -o kapcsol\u00f3val egy\u00fctt kell haszn\u00e1lni."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "Haszn\u00e1lat:\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <kimenet>]\n      [-d <k\u00f6nyvt\u00e1r>] [-j <jar_f\u00e1jl>] [-p <csomag>]\n      [-n] [-x] [-u] [-v] [-h] { <st\u00edluslap> | -i }\n\nBE\u00c1LL\u00cdT\u00c1SOK\n   -o <kimenet>   hozz\u00e1rendeli a <kimenet> nevet az el\u0151\u00e1ll\u00edtott\n                  translethez. Alap\u00e9rtelmez\u00e9s szerint\n                  a translet neve a <st\u00edluslap>\n                  nev\u00e9b\u0151l sz\u00e1rmazik. Ez a be\u00e1ll\u00edt\u00e1s figyelmen\n                  k\u00edv\u00fcl marad, ha t\u00f6bb st\u00edluslapot ford\u00edt.\n   -d <k\u00f6nyvt\u00e1r>  meghat\u00e1rozza a translet c\u00e9lk\u00f6nyvt\u00e1r\u00e1t\n   -j <jar_f\u00e1jl>  a translet oszt\u00e1lyokat egy jar f\u00e1jlba csomagolja,\n                  aminek a nev\u00e9t a <jar_f\u00e1jl> attrib\u00fatum adja meg\n   -p <csomag>    meghat\u00e1rozza az \u00f6sszes el\u0151\u00e1ll\u00edtott translet oszt\u00e1ly\n                  csomagn\u00e9v el\u0151tagj\u00e1t.\n   -n             enged\u00e9lyezi a sablonbeemel\u00e9st\n                  (az alap\u00e9rtelmezett viselked\u00e9s \u00e1ltal\u00e1ban jobb).\n   -x             bekapcsolja a tov\u00e1bbi hibakeres\u00e9si \u00fczeneteket\n   -u             \u00e9rtelmezi a <st\u00edluslap> argumentumokat \u00e9s URL c\u00edmeket\n   -i             k\u00e9nyszer\u00edti a ford\u00edt\u00f3t, hogy a st\u00edluslapot az stdin\n                  bemenetr\u0151l olvassa\n   -v             megjelen\u00edti a ford\u00edt\u00f3 verzi\u00f3sz\u00e1m\u00e1t\n   -h             megjelen\u00edti ezt a haszn\u00e1lati utas\u00edt\u00e1st\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "HASZN\u00c1LAT:\n   java org.apache.xalan.xsltc.cmdline.Transform [-j <jar_f\u00e1jl>]\n      [-x] [-n <ism\u00e9tl\u00e9s>] {-u <dokumentum_url_c\u00edme> | <dokumentum>}\n      <oszt\u00e1ly> [<param1>=<\u00e9rt\u00e9k1> ...]\n\n   a translet <oszt\u00e1ly> seg\u00edts\u00e9g\u00e9vel \u00e1talak\u00edtja a\n   <dokumentum> param\u00e9terben megadott dokumentumot. A translet\n   <oszt\u00e1ly> vagy a felhaszn\u00e1l\u00f3 CLASSPATH v\u00e1ltoz\u00f3ja\n   alapj\u00e1n, vagy a megadott <jar_f\u00e1jl>-ban tal\u00e1lhat\u00f3 meg.\nBE\u00c1LL\u00cdT\u00c1SOK\n   -j <jar_f\u00e1jl>   megadja a jar f\u00e1jlt a translet bet\u00f6lt\u00e9s\u00e9hez\n   -x              bekapcsolja a tov\u00e1bbi hibakeres\u00e9si \u00fczeneteket\n   -n <ism\u00e9tl\u00e9s>   az \u00e1talak\u00edt\u00e1st az <ism\u00e9tl\u00e9s> param\u00e9terben megadott\n                   alkalommal futtatja le, \u00e9s megjelen\u00edti a profiloz\u00e1si\n                   inform\u00e1ci\u00f3kat\n   -u <dokumentum_url_c\u00edme> megadja a bemeneti XML dokumentum URL c\u00edm\u00e9t\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "Az <xsl:sort> csak <xsl:for-each>-en vagy <xsl:apply-templates>-en bel\u00fcl haszn\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "A(z) ''{0}'' kimeneti k\u00f3dol\u00e1st nem t\u00e1mogatja ez a JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Szintaktikai hiba a(z) ''{0}'' kifejez\u00e9sben."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "A(z) ''{0}'' k\u00fcls\u0151 konstruktor nem tal\u00e1lhat\u00f3."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "A(z) ''{0}'' nem statikus Java f\u00fcggv\u00e9ny els\u0151 argumentuma nem \u00e9rv\u00e9nyes objektumhivatkoz\u00e1s."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Hiba t\u00f6rt\u00e9nt a(z) ''{0}'' kifejez\u00e9s t\u00edpus\u00e1nak ellen\u0151rz\u00e9sekor."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Hiba t\u00f6rt\u00e9nt egy ismeretlen helyen l\u00e9v\u0151 kifejez\u00e9s t\u00edpus\u00e1nak ellen\u0151rz\u00e9sekor."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "A(z) ''{0}'' parancssori param\u00e9ter \u00e9rv\u00e9nytelen."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "A(z) ''{0}'' parancssori param\u00e9terhez hi\u00e1nyzik egy k\u00f6telez\u0151 argumentum."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "FIGYELMEZTET\u00c9S:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "FIGYELMEZTET\u00c9S:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "S\u00daLYOS HIBA:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "S\u00daLYOS HIBA:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "HIBA:   ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "HIBA:   ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "\u00c1talak\u00edt\u00e1s a(z) ''{0}'' translet seg\u00edts\u00e9g\u00e9vel. "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "\u00c1talak\u00edt\u00e1s a(z) ''{0}'' translet haszn\u00e1lat\u00e1val a(z) ''{1}'' jar f\u00e1jlb\u00f3l. "},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Nem lehet l\u00e9trehozni a(z) ''{0}'' TransformerFactory oszt\u00e1ly p\u00e9ld\u00e1ny\u00e1t."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "A(z) ''{0}'' n\u00e9v nem haszn\u00e1lhat\u00f3 a translet oszt\u00e1ly nevek\u00e9nt, mivel olyan karaktereket tartalmaz, amelyek nem megengedettek Java oszt\u00e1lyok nev\u00e9ben. A rendszer a(z) ''{1}'' nevet haszn\u00e1lta helyette. "},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Ford\u00edt\u00e1s hib\u00e1k:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Ford\u00edt\u00e1si figyelmeztet\u00e9sek:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Translet hib\u00e1k:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Egy olyan attrib\u00fatum, amelynek az \u00e9rt\u00e9ke csak QName vagy QName \u00e9rt\u00e9kek sz\u00f3k\u00f6zzel elv\u00e1lasztott list\u00e1ja lehet, ''{0}'' \u00e9rt\u00e9kkel rendelkezett."},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Egy olyan attrib\u00fatum, amelynek \u00e9rt\u00e9ke csak NCName lehet, ''{0}'' \u00e9rt\u00e9kkel rendelkezett."},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "Egy <xsl:output> elem met\u00f3dus attrib\u00fatum\u00e1nak \u00e9rt\u00e9ke ''{0}'' volt. Az \u00e9rt\u00e9k csak ''xml'', ''html'', ''text'' vagy qname-but-not-ncname lehet."},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "A szolg\u00e1ltat\u00e1s neve nem lehet null a TransformerFactory.getFeature(String name) met\u00f3dusban."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "A szolg\u00e1ltat\u00e1s neve nem lehet null a TransformerFactory.setFeature(String name, boolean value) met\u00f3dusban."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "A(z) ''{0}'' szolg\u00e1ltat\u00e1s nem \u00e1ll\u00edthat\u00f3 be ehhez a TransformerFactory oszt\u00e1lyhoz."}
			  };
			}
		}

	}

}