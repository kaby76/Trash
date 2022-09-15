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
 * $Id: ErrorMessages_it.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_it : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Errore run-time interno in ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Errore run-time durante l'esecuzione di <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Conversione non valida da ''{0}'' a ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Funzione esterna ''{0}'' non supportata da XSLTC."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Tipo di argomento sconosciuto nell'espressione di uguaglianza."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Tipo argomento non valido ''{0}'' nella chiamata a ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Tentativo di formattare il numero ''{0}'' utilizzando il modello ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Impossibile clonare l''''iteratore ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iteratore per asse ''{0}'' non supportato."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iteratore per l''''asse immesso ''{0}'' non sopportato."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "L''''attributo ''{0}'' al di fuori dell''''elemento."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Dichiarazione dello spazio nome ''{0}''=''{1}'' al di fuori dell''''elemento."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "Lo spazio nomi per il prefisso ''{0}'' non \u00e8 stato dichiarato. "},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter creato utilizzando il tipo di origine DOM errato."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "Il parser SAX utilizzato non gestisce gli eventi di dichiarazione DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "Il parser SAX utilizzato non dispone del supporto per gli spazi nome XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "Impossibile risolvere il riferimento URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "Elemento XSL non supportato ''{0}''"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Estensione XSLTC non riconosciuta ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "Il translet specificato, ''{0}'', \u00e8 stato creato utilizzando una versione di XSLTC pi\u00f9 recente della versione del run-time XSLTC che \u00e8 in uso. \u00c8 necessario ricompilare il foglio di lavoro oppure utilizzare una versione pi\u00f9 recente di XSLTC per eseguire questo translet."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Un attributo il cui valore deve essere un QName aveva il valore ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Un attributo il cui valore deve essere un NCName aveva il valore ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "L''''utilizzo di una funzione di estensione ''{0}'' non \u00e8 consentito quando la funzione di elaborazione sicura \u00e8 impostata su true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "L''''utilizzo di un elemento di estensione ''{0}'' non \u00e8 consentito quando la funzione di elaborazione sicura \u00e8 impostata su true."}
			  };
			}
		}

	}

}