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
 * $Id: ErrorMessages_fr.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_fr : ListResourceBundle
	{

	/*
	 * XSLTC run-time error messages.
	 *
	 * General notes to translators and definitions:
	 *
	 *   1) XSLTC is the name of the product.  It is an acronym for XML Stylesheet:
	 *      Transformations Compiler
	 *
	 *   2) A stylesheet is a description of how to transform an input XML document
	 *      into a resultant output XML document (or HTML document or text)
	 *
	 *   3) An axis is a particular "dimension" in a tree representation of an XML
	 *      document; the nodes in the tree are divided along different axes.
	 *      Traversing the "child" axis, for instance, means that the program
	 *      would visit each child of a particular node; traversing the "descendant"
	 *      axis means that the program would visit the child nodes of a particular
	 *      node, their children, and so on until the leaf nodes of the tree are
	 *      reached.
	 *
	 *   4) An iterator is an object that traverses nodes in a tree along a
	 *      particular axis, one at a time.
	 *
	 *   5) An element is a mark-up tag in an XML document; an attribute is a
	 *      modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *      "elem" is an element name, "attr" and "attr2" are attribute names with
	 *      the values "val" and "val2", respectively.
	 *
	 *   6) A namespace declaration is a special attribute that is used to associate
	 *      a prefix with a URI (the namespace).  The meanings of element names and
	 *      attribute names that use that prefix are defined with respect to that
	 *      namespace.
	 *
	 *   7) DOM is an acronym for Document Object Model.  It is a tree
	 *      representation of an XML document.
	 *
	 *      SAX is an acronym for the Simple API for XML processing.  It is an API
	 *      used inform an XML processor (in this case XSLTC) of the structure and
	 *      content of an XML document.
	 *
	 *      Input to the stylesheet processor can come from an XML parser in the
	 *      form of a DOM tree or through the SAX API.
	 *
	 *   8) DTD is a document type declaration.  It is a way of specifying the
	 *      grammar for an XML file, the names and types of elements, attributes,
	 *      etc.
	 *
	 *   9) Translet is an invented term that refers to the class file that contains
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Erreur interne d''ex\u00e9cution dans ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Erreur d'ex\u00e9cution de <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Conversion non valide de ''{0}'' en ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Fonction externe ''{0}'' non prise en charge par XSLTC."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Type d'argument inconnu dans l'expression d'\u00e9galit\u00e9."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Type d''argument non valide ''{0}'' lors de l''appel de ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Tentative de formatage du nombre ''{0}'' \u00e0 l''aide du motif ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Clonage impossible de l''it\u00e9rateur ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "It\u00e9rateur de l''axe ''{0}'' non pris en charge."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "It\u00e9rateur de l''axe indiqu\u00e9 ''{0}'' non pris en charge."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "L''attribut ''{0}'' est \u00e0 l''ext\u00e9rieur de l''\u00e9l\u00e9ment."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "La d\u00e9claration d''espace de noms ''{0}''=''{1}'' est \u00e0 l''ext\u00e9rieur de l''\u00e9l\u00e9ment."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "L''espace de noms du pr\u00e9fixe ''{0}'' n''a pas \u00e9t\u00e9 d\u00e9clar\u00e9."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter a \u00e9t\u00e9 cr\u00e9\u00e9 avec un type incorrect de source de DOM."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "L'analyseur SAX que vous utilisez ne traite pas les \u00e9v\u00e9nements de d\u00e9claration DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "L'analyseur SAX que vous utilisez ne prend pas en charge les espaces de nom XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "R\u00e9solution impossible de la r\u00e9f\u00e9rence de l''URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "El\u00e9ment XSL non pris en charge ''{0}''"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Extension XSLTC non reconnue ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "La classe translet indiqu\u00e9e, ''{0}'', a \u00e9t\u00e9 cr\u00e9\u00e9e \u00e0 l''aide d''une version de XSLTC plus r\u00e9cente que la version de l''ex\u00e9cutable XSLTC utilis\u00e9e. Vous devez compiler \u00e0 nouveau la feuille de style ou utiliser une version plus r\u00e9cente pour ex\u00e9cuter la classe translet."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Un attribut dont la valeur doit \u00eatre un QName poss\u00e8de la valeur ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Un attribut dont la valeur doit \u00eatre un NCName poss\u00e8de la valeur ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "L''utilisation de la fonction d''extension ''{0}'' n''est pas admise lorsque la fonction de traitement s\u00e9curis\u00e9e a la valeur true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "L''utilisation de l''\u00e9l\u00e9ment d''extension ''{0}'' n''est pas admise lorsque la fonction de traitement s\u00e9curis\u00e9e a la valeur true."}
			  };
			}
		}

	}

}