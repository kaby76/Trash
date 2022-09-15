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
 * $Id: ErrorMessages_pl.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_pl : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "W jednym pliku zdefiniowano wi\u0119cej ni\u017c jeden arkusz styl\u00f3w."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "Szablon ''{0}'' zosta\u0142 ju\u017c zdefiniowany w tym arkuszu styl\u00f3w."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "Szablon ''{0}'' nie zosta\u0142 zdefiniowany w tym arkuszu styl\u00f3w."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "Zmienna ''{0}'' zosta\u0142a zdefiniowana wielokrotnie w tym samym zasi\u0119gu."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Nie zdefiniowano zmiennej lub parametru ''{0}''."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Nie mo\u017cna znale\u017a\u0107 klasy ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Nie mo\u017cna znale\u017a\u0107 metody zewn\u0119trznej ''{0}'' (musi by\u0107 zadeklarowana jako public)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Nie mo\u017cna przekszta\u0142ci\u0107 typu argumentu lub typu wyniku w wywo\u0142aniu metody ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Nie mo\u017cna znale\u017a\u0107 pliku lub identyfikatora URI ''{0}''."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Niepoprawny identyfikator URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Nie mo\u017cna otworzy\u0107 pliku lub identyfikatora URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Oczekiwano elementu <xsl:stylesheet> lub <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Nie zadeklarowano przedrostka przestrzeni nazw ''{0}''."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Nie mo\u017cna rozstrzygn\u0105\u0107 wywo\u0142ania funkcji ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "Argument funkcji ''{0}'' musi by\u0107 litera\u0142em \u0142a\u0144cuchowym."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "B\u0142\u0105d podczas analizowania wyra\u017cenia XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Brakuje atrybutu wymaganego ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Niedozwolony znak ''{0}'' w wyra\u017ceniu XPath."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Niedozwolona nazwa ''{0}'' instrukcji przetwarzania."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Atrybut ''{0}'' znajduje si\u0119 poza elementem."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Niedozwolony atrybut ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Cykliczny import/include. Arkusz styl\u00f3w ''{0}'' zosta\u0142 ju\u017c za\u0142adowany."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Nie mo\u017cna posortowa\u0107 fragment\u00f3w drzewa rezultat\u00f3w (elementy <xsl:sort> s\u0105 ignorowane). Trzeba sortowa\u0107 w\u0119z\u0142y podczas tworzenia drzewa rezultat\u00f3w."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Formatowanie dziesi\u0119tne ''{0}'' zosta\u0142o ju\u017c zdefiniowane."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "Wersja XSL ''{0}'' nie jest obs\u0142ugiwana przez XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Cykliczne odwo\u0142anie do zmiennej lub parametru w ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Nieznany operator wyra\u017cenia dwuargumentowego."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Niedozwolone argumenty w wywo\u0142aniu funkcji."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Drugim argumentem funkcji document() musi by\u0107 zbi\u00f3r w\u0119z\u0142\u00f3w."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "W <xsl:choose> wymagany jest przynajmniej jeden element <xsl:when>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "W <xsl:choose> dozwolony jest tylko jeden element <xsl:otherwise>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "Elementu <xsl:otherwise> mo\u017cna u\u017cy\u0107 tylko wewn\u0105trz <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "Elementu <xsl:when> mo\u017cna u\u017cy\u0107 tylko wewn\u0105trz <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Tylko elementy <xsl:when> i <xsl:otherwise> s\u0105 dozwolone w <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> nie ma atrybutu 'name'."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Niedozwolony element potomny."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Nie mo\u017cna wywo\u0142a\u0107 elementu ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Nie mo\u017cna wywo\u0142a\u0107 atrybutu ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Dane tekstowe poza elementem <xsl:stylesheet> najwy\u017cszego poziomu."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Analizator sk\u0142adni JAXP nie zosta\u0142 poprawnie skonfigurowany."},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Nienaprawialny b\u0142\u0105d wewn\u0119trzny XSLTC: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Nieobs\u0142ugiwany element XSL ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Nierozpoznane rozszerzenie XSLTC ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Dokument wej\u015bciowy nie jest arkuszem styl\u00f3w (przestrze\u0144 nazw XSL nie zosta\u0142a zadeklarowana w elemencie g\u0142\u00f3wnym)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Nie mo\u017cna znale\u017a\u0107 elementu docelowego ''{0}'' arkusza styl\u00f3w."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Nie zaimplementowano: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Dokument wej\u015bciowy nie zawiera arkusza styl\u00f3w XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Nie mo\u017cna zanalizowa\u0107 elementu ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "Warto\u015bci\u0105 atrybutu use elementu <key> musi by\u0107: node, node-set, string lub number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "Wyj\u015bciowy dokument XML powinien mie\u0107 wersj\u0119 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Nieznany operator wyra\u017cenia relacyjnego"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Pr\u00f3ba u\u017cycia nieistniej\u0105cego zbioru atrybut\u00f3w ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Nie mo\u017cna zanalizowa\u0107 szablonu warto\u015bci atrybutu ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Nieznany typ danych w sygnaturze klasy ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Nie mo\u017cna przekszta\u0142ci\u0107 typu danych ''{0}'' w ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Klasa Templates nie zawiera poprawnej definicji klasy transletu."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Ta klasa Templates nie zawiera klasy o nazwie ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Nie mo\u017cna za\u0142adowa\u0107 klasy transletu ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Za\u0142adowano klas\u0119 transletu, ale nie mo\u017cna utworzy\u0107 jego instancji."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Pr\u00f3ba ustawienia obiektu ErrorListener klasy ''{0}'' na warto\u015b\u0107 null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Tylko StreamSource, SAXSource i DOMSource s\u0105 obs\u0142ugiwane przez XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "Obiekt klasy Source przekazany do ''{0}'' nie ma kontekstu."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Nie mo\u017cna skompilowa\u0107 arkusza styl\u00f3w."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "Klasa TransformerFactory nie rozpoznaje atrybutu ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "Przed wywo\u0142aniem metody startDocument() nale\u017cy wywo\u0142a\u0107 metod\u0119 setResult()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Obiekt Transformer nie zawiera referencji do obiektu transletu."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Nie zdefiniowano procedury obs\u0142ugi wyj\u015bcia rezultat\u00f3w transformacji."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "Obiekt Result przekazany do ''{0}'' jest niepoprawny."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Pr\u00f3ba dost\u0119pu do niepoprawnej w\u0142a\u015bciwo\u015bci interfejsu Transformer ''{0}''."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Nie mo\u017cna utworzy\u0107 adaptera SAX2DOM: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "Metoda XSLTCSource.build() zosta\u0142a wywo\u0142ana bez ustawienia warto\u015bci systemId."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Rezultat nie powinien by\u0107 pusty"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "Warto\u015bci\u0105 parametru {0} musi by\u0107 poprawny obiekt j\u0119zyka Java."},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "Z opcj\u0105 -o trzeba u\u017cy\u0107 tak\u017ce opcji -i."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SK\u0141ADNIA\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <wyj\u015bcie>]\n      [-d <katalog>] [-j <plik_jar>] [-p <pakiet>]\n      [-n] [-x] [-u] [-v] [-h] { <arkusz_styl\u00f3w> | -i }\n\nOPCJE\n   -o <wyj\u015bcie>    przypisanie nazwy <wyj\u015bcie> do wygenerowanego\n                   transletu.  Domy\u015blnie nazwa transletu pochodzi \n                   od nazwy <arkusza_styl\u00f3w>. Opcja ta jest ignorowana \n                   w przypadku kompilowania wielu arkuszy styl\u00f3w.\n   -d <katalog>    Okre\u015blenie katalogu docelowego transletu.\n   -j <plik_jar>   Pakowanie klas transletu do pliku jar o nazwie\n                   okre\u015blonej jako <plik_jar>.\n   -p <pakiet>     Okre\u015blenie przedrostka nazwy pakietu dla wszystkich\n                   wygenerowanych klas translet\u00f3w.\n   -n              W\u0142\u0105czenie wstawiania szablon\u00f3w (zachowanie domy\u015blne\n                   zwykle lepsze).\n   -x              W\u0142\u0105czenie wypisywania dodatkowych komunikat\u00f3w debugowania.\n    -u              Interpretowanie argument\u00f3w <arkusz_styl\u00f3w> jako\n                   adres\u00f3w URL.\n   -i              Wymuszenie odczytywania przez kompilator arkusza styl\u00f3w\n                   ze standardowego wej\u015bcia (stdin).\n   -v              Wypisanie wersji kompilatora.\n   -h              Wypisanie informacji o sk\u0142adni.\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SK\u0141ADNIA \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <plik_jar>]\n      [-x] [-n <iteracje>] {-u <url_dokumentu> | <dokument>}\n <klasa> [<param1>=<warto\u015b\u01071> ...]\n\n   U\u017cycie transletu <klasa> do transformacji dokumentu XML \n   okre\u015blonego jako <dokument>. Translet <klasa> znajduje si\u0119 w\n   \u015bcie\u017cce CLASSPATH u\u017cytkownika lub w opcjonalnie podanym pliku <plik_jar>.\nOPCJE\n   -j <plik_jar>   Okre\u015blenie pliku jar, z kt\u00f3rego nale\u017cy za\u0142adowa\u0107 translet.\n   -x              W\u0142\u0105czenie wypisywania dodatkowych komunikat\u00f3w debugowania.\n   -n <iteracje>   Okre\u015blenie krotno\u015bci wykonywania transformacji oraz \n                   w\u0142\u0105czenie wy\u015bwietlania informacji z profilowania.\n   -u <url_dokumentu>\n                   Okre\u015blenie wej\u015bciowego dokumentu XML w postaci adresu URL.\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "Elementu <xsl:sort> mo\u017cna u\u017cy\u0107 tylko wewn\u0105trz <xsl:for-each> lub <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Kodowanie wyj\u015bciowe ''{0}'' nie jest obs\u0142ugiwane przez t\u0119 maszyn\u0119 wirtualn\u0105 j\u0119zyka Java."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "B\u0142\u0105d sk\u0142adniowy w ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Nie mo\u017cna znale\u017a\u0107 konstruktora zewn\u0119trznego ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Pierwszy argument funkcji ''{0}'' j\u0119zyka Java (innej ni\u017c static) nie jest poprawnym odniesieniem do obiektu."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "B\u0142\u0105d podczas sprawdzania typu wyra\u017cenia ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "B\u0142\u0105d podczas sprawdzania typu wyra\u017cenia w nieznanym po\u0142o\u017ceniu."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "Niepoprawna opcja ''{0}'' wiersza komend."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "Brakuje argumentu wymaganego opcji ''{0}'' wiersza komend."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "OSTRZE\u017bENIE:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "OSTRZE\u017bENIE:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "B\u0141\u0104D KRYTYCZNY:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "B\u0141\u0104D KRYTYCZNY:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "B\u0141\u0104D:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "B\u0141\u0104D:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Dokonaj transformacji za pomoc\u0105 transletu ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Dokonaj transformacji za pomoc\u0105 transletu ''{0}'' z pliku jar ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Nie mo\u017cna utworzy\u0107 instancji klasy ''{0}'' interfejsu TransformerFactory."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "Nazwy ''{0}'' nie mo\u017cna u\u017cy\u0107 jako nazwy klasy transletu, poniewa\u017c zawiera ona znaki, kt\u00f3re s\u0105 niedozwolone w nazwach klas j\u0119zyka Java.  Zamiast niej u\u017cyto nazwy ''{1}''."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "B\u0142\u0119dy kompilatora:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Ostrze\u017cenia kompilatora:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "B\u0142\u0119dy transletu:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Atrybut, kt\u00f3rego warto\u015bci\u0105 musi by\u0107 nazwa QName lub lista rozdzielonych odst\u0119pami nazw QName, mia\u0142 warto\u015b\u0107 ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Atrybut, kt\u00f3rego warto\u015bci\u0105 musi by\u0107 nazwa NCName, mia\u0142 warto\u015b\u0107 ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "Atrybut method elementu <xsl:output> mia\u0142 warto\u015b\u0107 ''{0}''.  Warto\u015bci\u0105 mo\u017ce by\u0107: ''xml'', ''html'', ''text'' lub nazwa qname nie b\u0119d\u0105ca nazw\u0105 ncname."},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "Nazwa opcji nie mo\u017ce mie\u0107 warto\u015bci null w TransformerFactory.getFeature(String nazwa)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "Nazwa opcji nie mo\u017ce mie\u0107 warto\u015bci null w TransformerFactory.setFeature(String nazwa, boolean warto\u015b\u0107)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Nie mo\u017cna ustawi\u0107 opcji ''{0}'' w tej klasie TransformerFactory."}
			  };
			}
		}

	}

}