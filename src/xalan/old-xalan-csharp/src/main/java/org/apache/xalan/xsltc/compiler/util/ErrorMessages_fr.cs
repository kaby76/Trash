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
 * $Id: ErrorMessages_fr.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_fr : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "Plusieurs feuilles de style ont \u00e9t\u00e9 d\u00e9finies dans le m\u00eame fichier."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "Le mod\u00e8le ''{0}'' est d\u00e9j\u00e0 d\u00e9fini dans cette feuille de style."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "Le mod\u00e8le ''{0}'' n''est pas d\u00e9fini dans cette feuille de style."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "La variable ''{0}'' est d\u00e9finie plusieurs fois dans la m\u00eame port\u00e9e."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "La variable ou le param\u00e8tre ''{0}'' n''est pas d\u00e9fini."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "La classe ''{0}'' est introuvable."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "La m\u00e9thode externe ''{0}'' est introuvable (doit \u00eatre public)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Impossible de convertir le type d''argument/de retour lors de l''appel de la m\u00e9thode ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Le fichier ou l'' URI ''{0}'' est introuvable."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI non valide ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Impossible d''ouvrir le fichier ou l''URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "L'\u00e9l\u00e9ment <xsl:stylesheet> ou <xsl:transform> est attendu."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Le pr\u00e9fixe de l''espace de noms ''{0}'' n''est pas d\u00e9clar\u00e9."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Impossible de r\u00e9soudre l''appel \u00e0 la fonction ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "L''argument de ''{0}'' doit \u00eatre une cha\u00eene litt\u00e9rale."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Erreur lors de l''analyse de l''expression XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "L''attribut obligatoire ''{0}'' est absent."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Caract\u00e8re ''{0}'' non conforme dans l''expression XPath."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Nom ''{0}'' non conforme dans l''instruction de traitement."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "L''attribut ''{0}'' est \u00e0 l''ext\u00e9rieur de l''\u00e9l\u00e9ment."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Attribut ''{0}'' non conforme."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "import/include circulaire. La feuille de style ''{0}'' est d\u00e9j\u00e0 charg\u00e9e."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Tri impossible des fragments de l'arborescence de r\u00e9sultats (les \u00e9l\u00e9ments <xsl:sort> sont ignor\u00e9s). Vous devez trier les noeuds lors de la cr\u00e9ation de l'arborescence de r\u00e9sultats."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Le formatage d\u00e9cimal ''{0}'' est d\u00e9j\u00e0 d\u00e9fini."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "La version XSL ''{0}'' n''est pas prise en charge par XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "R\u00e9f\u00e9rence variable/param\u00e8tre circulaire dans ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Op\u00e9rateur inconnu dans une expression binaire."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Argument(s) incorrect(s) pour l'appel de fonction."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Le deuxi\u00e8me argument de la fonction document() doit \u00eatre un ensemble de noeuds."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Au moins un \u00e9l\u00e9ment <xsl:when> est requis dans <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Un seul \u00e9l\u00e9ment <xsl:otherwise> est autoris\u00e9 dans <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> peut \u00eatre utilis\u00e9 uniquement dans <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> peut \u00eatre utilis\u00e9 uniquement dans <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Seuls les \u00e9l\u00e9ments <xsl:when> et <xsl:otherwise> sont autoris\u00e9s dans <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "Attribut 'name' absent de <xsl:attribute-set>."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "El\u00e9ment enfant incorrect."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Vous ne pouvez pas appeler un \u00e9l\u00e9ment ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Vous ne pouvez pas appeler un attribut ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Donn\u00e9es textuelles \u00e0 l'ext\u00e9rieur de l'\u00e9l\u00e9ment de niveau sup\u00e9rieur <xsl:stylesheet>."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "L'analyseur JAXP n'est pas configur\u00e9 correctement"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Erreur interne \u00e0 XSLTC irr\u00e9m\u00e9diable : ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "El\u00e9ment XSL non pris en charge ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Extension XSLTC ''{0}'' non reconnue."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Le document de base n'est pas une feuille de style (l'espace de noms XSL n'est pas d\u00e9clar\u00e9 dans l'\u00e9l\u00e9ment root)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "La feuille de style cible ''{0}'' est introuvable."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Non impl\u00e9ment\u00e9 : ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Le document de base ne contient pas de feuille de style XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Impossible d''analyser l''\u00e9l\u00e9ment ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "L'attribut use de <key> doit avoir la valeur node, node-set, string ou number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "La version du document XML de sortie doit \u00eatre 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Op\u00e9rateur inconnu dans une expression relationnelle."},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Tentative d''utilisation d''un jeu d''attributs ''{0}'' inexistant."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Impossible d''analyser le mod\u00e8le de valeur d''attribut ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Type de donn\u00e9es inconnu dans la signature pour la classe ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Impossible de convertir le type de donn\u00e9es ''{0}'' en ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Cette classe Templates ne contient pas de d\u00e9finition de classe translet valide."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Cette classe Templates ne contient pas de classe portant le nom ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Impossible de charger la classe translet ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "La classe translet est charg\u00e9e, mais il est impossible de cr\u00e9er une instance translet."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Tentative de d\u00e9finition de l''\u00e9l\u00e9ment ErrorListener pour ''{0}'' \u00e0 une valeur null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Seuls StreamSource, SAXSource et DOMSource sont pris en charge par XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "L''objet source transmis \u00e0 ''{0}'' est vide."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Impossible de compiler la feuille de style"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory ne reconna\u00eet pas l''attribut ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() doit \u00eatre appel\u00e9 avant startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer ne comporte pas d'objet translet encapsul\u00e9."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Aucun gestionnaire de sortie n'a \u00e9t\u00e9 d\u00e9fini pour le r\u00e9sultat de la transformation."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "L''objet Result object transmis \u00e0 ''{0}'' n''est pas valide."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Tentative d''acc\u00e8s \u00e0 une propri\u00e9t\u00e9 Transformer non valide ''{0}''."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Impossible de cr\u00e9er l''adaptateur SAX2DOM : ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "Appel de la part de XSLTCSource.build() sans d\u00e9finition d'identification du syst\u00e8me."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Le r\u00e9sultat doit \u00eatre vide"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "La valeur du param\u00e8tre {0} doit \u00eatre un objet Java valide"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "L'option -i doit \u00eatre utilis\u00e9e avec l'option -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <sortie>]\n      [-d <r\u00e9pertoire>] [-j <fichierjar>] [-p <package>]\n      [-n] [-x] [-u] [-v] [-h] { <feuille_de_style> | -i }\n\nOPTIONS\n   -o <sortie>    attribue le nom <sortie> au translet\n                  g\u00e9n\u00e9r\u00e9. Par d\u00e9faut, le nom du translet est\n                  d\u00e9riv\u00e9 du nom <feuille_de_style>.  Cette option\n                  est ignor\u00e9e si plusieurs feuilles de style sont compil\u00e9es.\n   -d <r\u00e9pertoire> sp\u00e9cifie un r\u00e9pertoire de destination pour translet\n   -j <fichier_jar>   rassemble les classes translet dans le fichier \n       <fichier_jar>\n   -p <module>   sp\u00e9cifie un pr\u00e9fixe de nom de module pour toutes les classes\n              translet g\u00e9n\u00e9r\u00e9es.\n   -n             active la mise en ligne de mod\u00e8le (comportement par d\u00e9faut - pr\u00e9f\u00e9rable \n                  on en moyenne).\n   -x             active le d\u00e9bogage suppl\u00e9mentaire de sortie de message\n   -u             interpr\u00e8te les arguments <feuille_de_style> comme des adresses URL\n   -i             force le compilateur \u00e0 lire la feuille de style dans stdin\n   -v             imprime la version du compilateur\n   -h             imprime cette instruction de syntaxe\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <fichierjar>]\n      [-x] [-n <it\u00e9rations>] {-u <url_document> | <document>}\n    <classe> [<param1>=<valeur1> ...]\n\n   utilise la <classe> translet pour transformer un document XML \n   d\u00e9fini comme <document>. La <classe> translet se trouve dans la\n   fonction CLASSPATH de l'utilisateur ou dans le <fichier_jar> indiqu\u00e9 en option.\nOPTIONS\n   -j <fichierjar>    sp\u00e9cifie un fichier jar \u00e0 partir duquel charger le translet\n   -x              active le d\u00e9bogage suppl\u00e9mentaire de sortie de message\n   -n <it\u00e9rations> ex\u00e9cute la transformation <it\u00e9rations> fois et\n                   affiche des informations de profil\n   -u <url_document> d\u00e9finit le document d'entr\u00e9e XML comme une URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> peut \u00eatre utilis\u00e9 uniquement dans <xsl:for-each> ou <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Le codage de sortie ''{0}'' n''est pas pris en charge dans cette JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Erreur de syntaxe dans ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Impossible de trouver le constructeur externe ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Le premier argument de la fonction Java non static ''{0}'' n''est pas une r\u00e9f\u00e9rence d''objet valide."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Erreur lors de la v\u00e9rification du type de l''expression ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Erreur de contr\u00f4le du type d'une expression se trouvant dans un emplacement inconnu."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "L''option de ligne de commande ''{0}'' n''est pas valide."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "Un argument obligatoire est absent de l''option de ligne de commande ''{0}''."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "AVERTISSEMENT :  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "AVERTISSEMENT :  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "ERREUR BLOQUANTE :  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "ERREUR BLOQUANTE :  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "ERREUR :  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "ERREUR :  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Transformation \u00e0 l''aide du translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Transformation \u00e0 l''aide du translet ''{0}'' \u00e0 partir du fichier jar ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Impossible de cr\u00e9er une instance de la classe TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "Le nom ''{0}'' ne peut pas \u00eatre utilis\u00e9 pour la classe translet du fait qu''elle contient des caract\u00e8res qui ne sont pas admis dans le nom d''une classe Java. Le nom ''{1}'' a \u00e9t\u00e9 utilis\u00e9 \u00e0 la place."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Erreurs de compilation :"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Avertissements de compilation :"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Erreurs de translet :"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Un attribut dont la valeur doit \u00eatre un QName ou une liste de QName d\u00e9limit\u00e9e par des espaces poss\u00e8de la valeur ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Un attribut dont la valeur doit \u00eatre un NCName poss\u00e8de la valeur ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "L''attribut method d''un \u00e9l\u00e9ment <xsl:output> poss\u00e8de la valeur ''{0}''. La valeur doit \u00eatre ''xml'', ''html'', ''text'' ou qname-but-not-ncname"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "Le nom de la fonction ne peut pas avoir une valeur null dans TransformerFactory.getFeature (nom cha\u00eene)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "Le nom de la fonction ne peut pas avoir la valeur null dans TransformerFactory.setFeature (nom cha\u00eene, valeur bool\u00e9nne)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Impossible de d\u00e9finir la fonction ''{0}'' sur cet \u00e9l\u00e9ment TransformerFactory."}
			  };
			}
		}

	}

}