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
 * $Id: ErrorMessages_it.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_it : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "Pi\u00f9 fogli di stile definiti nello stesso file."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "La maschera ''{0}'' gi\u00e0 definita in questo foglio di lavoro."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "La maschera ''{0}'' non definita in questo foglio di lavoro."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "La variabile ''{0}'' \u00e8 definita pi\u00f9 volte nello stesso ambito."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Variabile o parametro ''{0}'' non definito. "},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Impossibile trovare la classe ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Impossibile trovare il metodo esterno ''{0}'' (deve essere pubblico)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Impossibile convertire il tipo di argomento/ritorno nella chiamata nel metodo ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "File o URI ''{0}'' non trovato."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI non valido ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Impossibile aprire il file o l''''URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Era previsto l'elemento <xsl:stylesheet> o <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Il prefisso dello spazio nome ''{0}'' non \u00e8 dichiarato. "},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Impossibile risolvere la chiamata alla funzione ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "L''''argomento di ''{0}'' deve essere una stringa letterale. "},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Errore durante l''''analisi dell''''espressione XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Attributo ''{0}'' richiesto mancante. "},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Carattere non valido ''{0}'' nell''''espressione XPath. "},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Nome ''{0}'' non valido per l''''istruzione di elaborazione. "},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "L''''attributo ''{0}'' al di fuori dell''''elemento."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Attributo non valido ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Import/include circolare. Foglio di lavoro ''{0}'' gi\u00e0 caricato. "},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Impossibile ordinare i frammenti della struttura ad albero dei risultati (elementi <xsl:sort> ignorati). \u00c8 necessario ordinare i nodi quando si crea la struttura ad albero dei risultati."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "La formattazione decimale ''{0}'' \u00e8 gi\u00e0 definita."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "La versione XSL ''{0}'' non \u00e8 supportata da XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Riferimento variabile/parametro circolare in ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Operatore sconosciuto per l'espressione binaria."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Argomento(i) non valido(i) per la chiamata alla funzione."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Il secondo argomento di una funzione document() deve essere una serie di nodi."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "\u00c8 necessario almeno un elemento <xsl:when> in <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Solo un elemento <xsl:otherwise> consentito in <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> pu\u00f2 essere utilizzato solo all'interno di <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> pu\u00f2 essere utilizzato solo all'interno di <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Solo gli elementi <xsl:when> e <xsl:otherwise> sono consentiti in <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> non contiene l'attributo 'name'."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Elemento child non valido."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Impossibile chiamare un elemento ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Impossibile chiamare un attributo ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Dati di testo al di fuori dell'elemento <xsl:stylesheet> di livello superiore."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Parser JAXP non configurato correttamente"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Errore XSLTC interno non recuperabile: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Elemento XSL non supportato ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Estensione XSLTC non riconosciuta ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Il documento di immissione non \u00e8 un foglio di lavoro (lo namespace XSL non \u00e8 dichiarato nell'elemento root)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Impossibile trovare la destinazione stylesheet ''{0}''."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Non implementato: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Il documento di immissione non contiene un foglio di lavoro XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Impossibile analizzare l''''elemento ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "L'attributo use di <key> deve essere node, node-set, string o number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "La versione del documento XML di emissione deve essere 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Operatore sconosciuto per l'espressione relazionale"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Tentativo di utilizzare una serie di attributi ''{0}'' non esistente."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Impossibile analizzare la maschera del valore di attributo ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Tipo di dati sconosciuto nella firma per la classe ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Impossibile convertire il tipo di dati da ''{0}'' a ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Questa Templates non contiene una definizione di classe translet valida."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Questa Templates non contengono una classe con il nome ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Impossibile caricare la classe translet ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Classe translet caricata, ma non \u00e8 possibile creare l'istanza translet."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Tentativo di impostare ErrorListener per ''{0}'' su null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Solo StreamSource, SAXSource e DOMSource sono supportati da XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "L''oggetto Source passato a ''{0}'' non ha contenuto."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Impossibile compilare il foglio di lavoro"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory non riconosce l''''attributo ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() deve essere richiamato prima di startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer non dispone di un oggetto translet incapsulato."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Nessun programma di gestione dell'emissione definito per il risultato della trasformazione."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "Oggetto Result passato ''{0}'' non valido."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Tentativo di accedere alla propriet\u00e0 Transformer ''{0}'' non valida."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Impossibile creare l''''adattatore SAX2DOM: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() richiamato senza che sia impostato un systemId (identificativo di sistema)."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Il risultato non pu\u00f2 essere nullo"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "Il valore del parametro {0} deve essere un oggetto Java valido"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "L'opzione -i deve essere utilizzata con l'opzione -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <output>]\n      [-d <directory>] [-j <jarfile>] [-p <package>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\nOPTIONS\n   -o <output>    assegna il nome <output> al translet generato\n.  Per impostazione predefinita, il nome translet \u00e8\n                  derivato dal nome <stylesheet>.  Questa opzione\n                  viene ignorata se vengono compilati pi\u00f9 fogli di stile.\n   -d <directory> specifica una directory di destinazione per il translet\n   -j <jarfile>   raggruppa le classi translet in un file jar del\n                  nome specificato come <jarfile>\n   -p <package>   specifica un prefisso del nome pacchetto per tutte le classi\n                  translet generate.\n   -n             abilita l'allineamento della maschera (funzionamento predefinito migliore\n                  in media).\n   -x             attiva ulteriori emissioni dei messaggi di debug\n   -u             interpreta gli argomenti <stylesheet> come URL\n   -i             impone al programma di compilazione di leggere il foglio di lavoro da stdin\n   -v             stampa la versione del programma di compilazione \n   -h             stampa queste istruzioni sull'utilizzo\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <jarfile>]\n      [-x] [-n <iterations>] {-u <document_url> | <document>}\n      <class> [<param1>=<value1> ...]\n\n   utilizza il translet <class> per convertire un documento XML \n   specificato come <document>. Il translet <classe> si trova \n   nella istruzione CLASSPATH dell'utente o nel <jarfile> eventualmente specificato.\nOPTIONS\n   -j <jarfile>    specifica un jarfile dal quale caricare il translet\n   -x    attiva ulteriori output dei messaggi di debug\n   -n <iterations> esegue la trasformazione <iterazioni> e\n                   visualizza le informazioni relative al profilo\n   -u <document_url> specifica il documento di immissione XML come URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> pu\u00f2 essere utilizzato solo all'interno di <xsl:for-each> o <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Codifica di emissione ''{0}'' non supportata in questa JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Errore di sintassi in ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Impossibile trovare un costruttore esterno ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Il primo argomento della funzione Java non statica ''{0}'' non \u00e8 un riferimento ad un oggetto valido. "},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Errore durante la verifica del tipo di espressione ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Errore durante la verifica del tipo di espressione in una posizione sconosciuta."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "L''''opzione della riga comandi ''{0}'' non \u00e8 valida."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "Manca un argomento obbligatorio per l''''opzione della riga comandi ''{0}''. "},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "ATTENZIONE:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "ATTENZIONE:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "ERRORE IRREVERSIBILE:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "ERRORE IRREVERSIBILE:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "ERRORE:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "ERRORE:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Trasformazione utilizzando il translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Trasformazione utilizzando il translet ''{0}'' dal file jar ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Impossibile creare un''istanza della classe TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "Non \u00e8 stato possibile utilizzare il nome ''{0}'' come nome della classe translet perch\u00e9 contiene dei caratteri che non sono consentiti nel nome della classe Java. \u00c8 stato invece utilizzato il nome ''{1}''."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Errori del programma di compilazione:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Messaggi di avvertenza del programma di compilazione:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Errori del translet:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Un attributo il cui valore deve essere un QName o un elenco separato da spazi vuoti di QName ha avuto il valore di ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Un attributo il cui valore deve essere un NCName aveva il valore ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "L''''attributo del metodo di un elemento <xsl:output> aveva il valore ''{0}''.  Il valore deve essere uno di ''xml'', ''html'', ''text'' o qname-but-not-ncname"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "Il nome della funzione non pu\u00f2 essere nullo in TransformerFactory.getFeature(Nome stringa)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "Il nome della funzione non pu\u00f2 essere nullo in TransformerFactory.setFeature(Nome stringa, valore booleano)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Impossibile impostare la funzione ''{0}'' su questo TransformerFactory."}
			  };
			}
		}

	}

}