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
 * $Id: ErrorMessages_ca.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ca : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "S'ha definit m\u00e9s d'un full d'estils en el mateix fitxer."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "La plantilla ''{0}'' ja est\u00e0 definida en aquest full d''estil. "},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "La plantilla ''{0}'' no est\u00e0 definida en aquest full d''estil. "},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "La variable ''{0}'' s''ha definit m\u00e9s d''una vegada en el mateix \u00e0mbit. "},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "La variable o el par\u00e0metre ''{0}'' no s''ha definit. "},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "No es pot trobar la classe ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "No es pot trobar el m\u00e8tode extern ''{0}'' (ha de ser p\u00fablic)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "No es pot convertir el tipus d''argument/retorn en la crida al m\u00e8tode ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "No s''ha trobat el fitxer o URI ''{0}''. "},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "L''URI ''{0}'' no \u00e9s v\u00e0lid."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "No es pot obrir el fitxer o URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "S'esperava l'element <xsl:stylesheet> o <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "No s''ha declarat el prefix de l''espai de noms ''{0}''. "},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "No es pot resoldre la crida a la funci\u00f3 ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "L''argument de ''{0}'' ha de ser una cadena de literals. "},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Error en analitzar l''expressi\u00f3 XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Falta l''atribut obligatori ''{0}''. "},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "L''expressi\u00f3 XPath cont\u00e9 el car\u00e0cter no perm\u00e8s ''{0}''. "},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "La instrucci\u00f3 de proc\u00e9s cont\u00e9 el nom no perm\u00e8s ''{0}''. "},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "L''atribut ''{0}'' es troba fora de l''element. "},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Atribut no perm\u00e8s ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Import/include circular. El full d''estil ''{0}'' ja s''ha carregat. "},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Els fragments de l'arbre de resultats no es poden classificar (es passen per alt els elements <xsl:sort>). Heu de classificar els nodes quan creeu l'arbre de resultats."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "El formatatge decimal ''{0}'' ja est\u00e0 definit."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSLTC no d\u00f3na suport a la versi\u00f3 XSL ''{0}''. "},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Hi ha una refer\u00e8ncia de variable/par\u00e0metre circular a ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "L'operador de l'expressi\u00f3 bin\u00e0ria \u00e9s desconegut."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "La crida de funci\u00f3 t\u00e9 arguments no permesos."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "El segon argument de la funci\u00f3 document() ha de ser un conjunt de nodes."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Es necessita com a m\u00ednim un element <xsl:when> a <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Nom\u00e9s es permet un element <xsl:otherwise> a <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> nom\u00e9s es pot utilitzar dins de <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> nom\u00e9s es pot utilitzar dins de <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "A <xsl:choose> nom\u00e9s es permeten els elements <xsl:when> i <xsl:otherwise>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "L'atribut 'name' falta a <xsl:attribute-set>."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "L'element subordinat no \u00e9s perm\u00e8s."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "No podeu cridar un element ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "No podeu cridar un atribut ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Hi ha dades fora de l'element de nivell superior <xsl:stylesheet>."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "L'analitzador JAXP no s'ha configurat correctament"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "S''ha produ\u00eft un error intern d''XSLTC irrecuperable: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "L''element XSL ''{0}'' no t\u00e9 suport."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "No es reconeix l''extensi\u00f3 XSLTC ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "El document d'entrada no \u00e9s un full d'estils (l'espai de noms XSL no s'ha declarat en l'element arrel)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "No s''ha pogut trobar la destinaci\u00f3 del full d''estil ''{0}''."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "No s''ha implementat: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "El document d'entrada no cont\u00e9 cap full d'estils XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "No s''ha pogut analitzar l''element ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "L'atribut use de <key> ha de ser node, node-set, string o number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "La versi\u00f3 del document XML de sortida ha de ser 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "L'operador de l'expressi\u00f3 relacional \u00e9s desconegut."},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "S''ha intentat utilitzar el conjunt d''atributs ''{0}'' que no existeix."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "No es pot analitzar la plantilla del valor d''atribut ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Es desconeix el tipus de dades de la signatura de la classe ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "No es pot convertir el tipus de dades ''{0}'' a ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Templates no cont\u00e9 cap definici\u00f3 de classe translet."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Templates no cont\u00e9 cap classe amb el nom ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "No s''ha pogut carregar la classe translet ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "La classe translet s'ha carregat, per\u00f2 no s'ha pogut crear la inst\u00e0ncia translet."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "S''ha intentat establir ErrorListener de ''{0}'' en nul"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC nom\u00e9s d\u00f3na suport a StreamSource, SAXSource i DOMSource."},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "L''objecte source passat a ''{0}'' no t\u00e9 contingut. "},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "No s'ha pogut compilar el full d'estils."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory no reconeix l''atribut ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() s'ha de cridar abans de startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer no cont\u00e9 cap objecte translet."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "No s'ha definit cap manejador de sortida per al resultat de transformaci\u00f3."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "L''objecte result passat a ''{0}'' no \u00e9s v\u00e0lid. "},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "S''ha intentat accedir a una propietat Transformer no v\u00e0lida ''{0}''."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "No s''ha pogut crear l''adaptador SAX2DOMr: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "S'ha cridat XSLTCSource.build() sense que s'hagu\u00e9s establert la identificaci\u00f3 del sistema."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "El resultat no ha de ser nul"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "El valor del par\u00e0metre {0} ha de ser un objecte Java v\u00e0lid "},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "L'opci\u00f3 -i s'ha d'utilitzar amb l'opci\u00f3 -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "RESUM\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <sortida>]\n      [-d <directori>] [-j <fitxer_jar>] [-p <paquet>]\n      [-n] [-x] [-u] [-v] [-h] { <full_estil> | -i }\n\nOPCIONS\n   -o <sortida>    assigna el nom <sortida> al translet\n                  generat. Per defecte, el nom del translet s'obt\u00e9\n                  del nom de <full_estils>. Aquesta opci\u00f3\n no es t\u00e9 en compte si es compilen diversos fulls d'estils.\n   -d <directori> especifica un directori de destinaci\u00f3 per al translet\n   -j <fitxer_jar>   empaqueta les classes translet en un fitxer jar del nom\n                  especificat com a <fitxer_jar>\n   -p <paquet> especifica un prefix de nom de paquet per a totes les classes\n                  translet generades.\n   -n habilita l'inlining (com a mitjana, el funcionament per defecte\n \u00e9s millor).\n   -x             habilita la sortida de missatges de depuraci\u00f3 addicionals\n   -u             interpreta els arguments del <full_estil> com URL\n   -i             obliga el compilador a llegir el full d'estil des de l'entrada est\u00e0ndardn\n   -v          imprimeix la versi\u00f3 del compilador\n   -h             imprimeix aquesta sent\u00e8ncia d'\u00fas\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "RESUM \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <fitxer_jar>]\n      [-x] [-n <iteracions>] {-u <url_document> | <document>}\n      <classe> [<param1>=<valor1> ...]\n\n   fa servir la <classe> translet per transformar un document XML \n   especificat com <document>. La <classe> translet es troba\n   o b\u00e9 a la CLASSPATH de l'usuari o b\u00e9 al <fitxer_jar> que es pot especificar opcionalment.\nOPCIONS\n   -j <fitxer_jar>    especifica un fitxer jar des del qual es carrega la classe translet\n   -x              habilita la sortida de missatges de depuraci\u00f3 addicionals \n   -n <iteracions> executa la transformaci\u00f3 el nombre de vegades <iteracions> i\n                   mostra la informaci\u00f3 de perfil\n   -u <url_document> especifica el document d'entrada XML com una URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> nom\u00e9s es pot utilitzar amb <xsl:for-each> o <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "A aquesta JVM, no es d\u00f3na suport a la codificaci\u00f3 de sortida ''{0}''."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "S''ha produ\u00eft un error de sintaxi a ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "No es pot trobar el constructor extern ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "El primer argument de la funci\u00f3 Java no static ''{0}'' no \u00e9s una refer\u00e8ncia d''objecte v\u00e0lida. "},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "S''ha produ\u00eft un error en comprovar el tipus de l''expressi\u00f3 ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "S'ha produ\u00eft un error en comprovar el tipus d'expressi\u00f3 en una ubicaci\u00f3 desconeguda."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "L''opci\u00f3 de l\u00ednia d''ordres ''{0}'' no \u00e9s v\u00e0lida. "},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "A l''opci\u00f3 de l\u00ednia d''ordres ''{0}'' falta un argument obligatori. "},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "AV\u00cdS:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "AV\u00cdS:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "ERROR MOLT GREU:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "ERROR MOLT GREU:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "ERROR:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "ERROR:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Transformaci\u00f3 mitjan\u00e7ant la classe translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Transformaci\u00f3 mitjan\u00e7ant la classe translet ''{0}'' des del fitxer jar ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "No s''ha pogut crear una inst\u00e0ncia de la classe TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "El nom ''{0}'' no s''ha pogut fer servir com nom de la classe translet perqu\u00e8 cont\u00e9 car\u00e0cters que no estan permesos al nom de la classe Java. En lloc d''aix\u00f2, es va fer servir el nom ''{1}''. "},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Errors del compilador:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Avisos del compilador:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Errors del translet:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Un atribut que ha de tenir el valor QName o una llista separada per espais de QNames tenia el valor ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Un atribut, que ha de tenir el valor NCName, tenia el valor ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "L''atribut del m\u00e8tode d''un element <xsl:sortida> tenia el valor ''{0}''. El valor ha de ser un de ''xml'', ''html'', ''text'', o ncname per\u00f2 no qname "},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "El nom de la caracter\u00edstica no pot ser nul a TransformerFactory.getFeature(nom de cadena). "},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "El nom de la caracter\u00edstica no pot ser nul a TransformerFactory.setFeature(nom de la cadena, valor boole\u00e0). "},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "No es pot establir la caracter\u00edstica ''{0}'' en aquesta TransformerFactory."}
			  };
			}
		}
	}

}