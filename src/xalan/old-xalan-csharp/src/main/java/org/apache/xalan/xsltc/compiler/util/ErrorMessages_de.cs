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
 * $Id: ErrorMessages_de.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_de : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "In einer Datei sind mehrere Formatvorlagen definiert."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "Vorlage ''{0}'' ist in dieser Formatvorlage bereits definiert."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "Vorlage ''{0}'' ist in dieser Formatvorlage nicht definiert."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "Variable ''{0}'' ist in einem Bereich mehrmals definiert."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Variable oder Parameter ''{0}'' ist nicht definiert."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Klasse ''{0}'' wurde nicht gefunden."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Die externe Methode ''{0}'' wurde nicht gefunden (muss ''public'' sein)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Argument-/R\u00fcckgabetyp in Aufruf kann nicht in Methode ''{0}'' konvertiert werden."},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Datei oder URI ''{0}'' wurde nicht gefunden."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Ung\u00fcltiger URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Datei oder URI ''{0}'' kann nicht ge\u00f6ffnet werden."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "<xsl:stylesheet>- oder <xsl:transform>-Element erwartet."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Namensbereichspr\u00e4fix ''{0}'' ist nicht deklariert."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Aufruf f\u00fcr Funktion ''{0}'' kann nicht aufgel\u00f6st werden."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "Argument f\u00fcr ''{0}'' muss eine Literalzeichenfolge sein."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Fehler bei Syntaxanalyse des XPath-Ausdrucks ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Erforderliches Attribut ''{0}'' fehlt."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Unzul\u00e4ssiges Zeichen ''{0}'' in XPath-Ausdruck."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Unzul\u00e4ssiger Name ''{0}'' f\u00fcr Verarbeitungsanweisung."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Attribut ''{0}'' befindet sich nicht in einem Element."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Unzul\u00e4ssiges Attribut ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Schleife bei ''import''/''include''. Formatvorlage ''{0}'' ist bereits geladen."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Ergebnisbaumfragmente k\u00f6nnen nicht sortiert werden (<xsl:sort>-Elemente werden ignoriert). Sie m\u00fcssen die Knoten sortieren, wenn Sie den Ergebnisbaum erstellen."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Dezimalformatierung ''{0}'' ist bereits definiert."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL-Version ''{0}'' wird von XSLTC nicht unterst\u00fctzt."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "R\u00fcckwirkender Variablen-/Parameterverweis in ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Unbekannter Operator f\u00fcr Bin\u00e4rausdruck."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Unzul\u00e4ssige(s) Argument(e) f\u00fcr Funktionsaufruf."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Zweites Argument f\u00fcr document()-Funktion muss eine Knotengruppe sein."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Es ist mindestens ein <xsl:when>-Element in <xsl:choose> erforderlich."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Es ist nur ein <xsl:otherwise>-Element in <xsl:choose> zul\u00e4ssig."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> kann nur in <xsl:choose> verwendet werden."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> kann nur in <xsl:choose> verwendet werden."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "In <xsl:choose> sind nur <xsl:when>- und <xsl:otherwise>-Elemente zul\u00e4ssig."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "Das Attribut 'name' fehlt f\u00fcr <xsl:attribute-set>."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Zul\u00e4ssiges Kindelement."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Sie k\u00f6nnen ein Element nicht ''{0}'' nennen."},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Sie k\u00f6nnen ein Attribut nicht ''{0}'' nennen."},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Textdaten au\u00dferhalb von <xsl:stylesheet>-Element der h\u00f6chsten Ebene."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP-Parser ist nicht richtig konfiguriert."},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Nicht behebbarer XSLTC-interner Fehler: ''{0}'' "},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Nicht unterst\u00fctztes XSL-Element ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Nicht erkannte XSLTC-Erweiterung ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Das Eingabedokument ist keine Formatvorlage (der XSL-Namensbereich wird nicht im Stammelement deklariert)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Das Formatvorlagenziel ''{0}'' wurde nicht gefunden."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Nicht implementiert: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Das Eingabedokument enth\u00e4lt keine XSL-Formatvorlage."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Element ''{0}'' konnte nicht syntaktisch analysiert werden."},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "Das Attribut 'use' von <key> muss 'node', 'node-set', 'string' oder 'number' sein."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "Die Version des XML-Ausgabedokuments sollte 1.0 sein."},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Unbekannter Operator f\u00fcr Vergleichsausdruck."},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Es wird versucht, die nicht vorhandene Attributgruppe ''{0}'' zu verwenden."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Die Attributwertvorlage ''{0}'' kann nicht syntaktisch analysiert werden."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Unbekannter Datentyp in Signatur f\u00fcr Klasse ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Datentyp ''{0}'' kann nicht in ''{1}'' konvertiert werden."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Diese Klasse 'Templates' enth\u00e4lt keine g\u00fcltige Translet-Klassendefinition."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Diese Klasse ''Templates'' enth\u00e4lt keine Klasse mit dem Namen ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Die Transletklasse ''{0}'' konnte nicht geladen werden."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Die Translet-Klasse wurde geladen, es kann jedoch keine Translet-Instanz erstellt werden."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Es wird versucht, ErrorListener f\u00fcr ''{0}'' auf null zu setzen."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Nur StreamSource, SAXSource und DOMSource werden von XSLTC unterst\u00fctzt."},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "Das Source-Objekt, das an ''{0}'' \u00fcbergeben wurde, hat keinen Inhalt."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Die Formatvorlage konnte nicht kompiliert werden."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory erkennt Attribut ''{0}'' nicht."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() muss vor startDocument() aufgerufen werden."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer hat kein eingebundenes Translet-Objekt."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Es ist keine Ausgabesteuerroutine f\u00fcr die Umsetzungsergebnisse definiert."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "Das Result-Objekt, das an ''{0}'' \u00fcbergeben wurde, ist ung\u00fcltig."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Es wird versucht, auf das ung\u00fcltige Transformer-Merkmal ''{0}'' zuzugreifen."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Der SAX2DOM-Adapter konnte nicht erstellt werden: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() wurde aufgerufen, ohne dass die System-ID gesetzt war."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Das Ergebnis darf nicht Null sein."},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "Der Wert f\u00fcr Parameter {0} muss ein g\u00fcltiges Java-Objekt sein."},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "Die Option -i muss mit der Option -o verwendet werden."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <Ausgabe>]\n      [-d <Verzeichnis>] [-j <jarDatei>] [-p <Paket>]\n      [-n] [-x] [-u] [-v] [-h] { <Formatvorlage> | -i }\n\nOPTIONEN\n   -o <Ausgabe>    Ordnet dem generierten Translet den Namen <Ausgabe> zu.\n Der Translet-Name wird standardm\u00e4\u00dfig\n                 von dem Namen von <formatvorlage> abgeleitet. Diese Option\n                  wird ignoriert, wenn mehrere Formatvorlagen kompiliert werden. \n      -d <verzeichnis> Gibt ein Zielverzeichnis f\u00fcr Translet an.\n   -j <jardatei>   Packt Translet-Klassen in eine jar-Datei mit dem\n                  Namen, der f\u00fcr <jardatei> angegeben wurde.\n   -p <paket>   Gibt ein Paketnamenpr\u00e4fix f\u00fcr alle\n                   generierten Translet-Klassen an.\n   -n             Aktiviert Inline-Anordnung von Vorlagen (Standardverhalten \n                  durchschnittlich besser).\n   -x             Aktiviert zus\u00e4tzliche Debugnachrichtenausgabe.\n   -u             Interpretiert Argumente <Formatvorlage> als URLs.\n   -i             Erzwingt, dass der Compiler die Formatvorlage aus der Standardeingabe liest.\n   -v             Gibt die Version des Compilers aus.\n   -h             Gibt diese Syntaxanweisung aus.\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNTAX \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <JAR-Datei>]\n      [-x] [-n <Iterationen>] {-u <Dokument-URL> | <Dokument>}\n      <Klasse> [<Param1>=<Wert1> ...]\n\n   Verwendet die <Klasse> von Translet, um ein  XML-Dokument umzusetzen,\n   das als <Dokument> angegeben wurde. Die <klasse> von Translet befindet sich entweder in\n   der CLASSPATH-Angabe des Benutzers oder in der optional angegebenen <jardatei>.\nOPTIONEN\n   -j <JAR-Datei>    Gibt eine JAR-Datei an, aus der das Translet geladen wird.\n   -x              Aktiviert zus\u00e4tzliche Debugnachrichtenausgabe.\n   -n <Iterationen> F\u00fchrt die Umsetzung <Iterationen> Mal aus und \n                   zeigt Profilinformationen an.\n   -u <Dokument-URL> Gibt das XML-Eingabedokument als URL an.\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> kann nur in <xsl:for-each> oder <xsl:apply-templates> verwendet werden."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Ausgabeverschl\u00fcsselung ''{0}'' wird auf dieser JVM nicht unterst\u00fctzt."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Syntaxfehler in ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Der externe Konstruktor ''{0}'' wurde nicht gefunden."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Das erste Argument der nichtstatischen Java-Funktion ''{0}'' ist kein g\u00fcltiger Objektverweis."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Fehler beim \u00dcberpr\u00fcfen des Typs des Ausdrucks ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Fehler beim \u00dcberpr\u00fcfen des Typs eines Ausdrucks an einer unbekannten Position."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "Die Befehlszeilenoption ''{0}'' ist nicht g\u00fcltig."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "In der Befehlszeilenoption ''{0}'' fehlt ein erforderliches Argument."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "WARNUNG:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "WARNUNG:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "SCHWER WIEGENDER FEHLER:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "SCHWER WIEGENDER FEHLER:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "FEHLER:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "FEHLER:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Umsetzung mit Translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Umwandlung mit Translet ''{0}'' aus JAR-Datei ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Es konnte keine Instanz der TransformerFactory-Klasse ''{0}'' erstellt werden."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "Der Name ''{0}'' konnte nicht als Name der Transletklasse verwendet werden, da er Zeichen enth\u00e4lt, die im Namen einer Java-Klasse nicht zul\u00e4ssig sind. Stattdessen wurde der Name ''{1}'' verwendet."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Compilerfehler:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Compilerwarnungen:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Translet-Fehler:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Ein Attribut, dessen Wert ein QName oder eine durch Leerzeichen getrennte Liste von QNamen sein muss, hatte den Wert ''{0}''."},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Ein Attribut, dessen Wert ein NCName sein muss, hatte den Wert ''{0}''."},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "Das Methodenattribut eines <xsl:output>-Elements hatte den Wert ''{0}''. Als Wert muss ''xml'', ''html'', ''text'' oder ''qname-but-not-ncname'' verwendet werden."},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "Der Funktionsname darf in TransformerFactory.getFeature(Name der Zeichenfolge) nicht den Wert Null haben."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "Der Funktionsname darf in TransformerFactory.setFeature(Name der Zeichenfolge, Boolescher Wert) nicht den Wert Null haben."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Die Funktion ''{0}'' kann in dieser TransformerFactory nicht festgelegt werden."}
			  };
			}
		}

	}

}