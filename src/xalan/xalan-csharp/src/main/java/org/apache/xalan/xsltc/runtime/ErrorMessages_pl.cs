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
 * $Id: ErrorMessages_pl.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_pl : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "B\u0142\u0105d wewn\u0119trzny czasu wykonywania w klasie ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "B\u0142\u0105d czasu wykonywania podczas wykonywania <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Niepoprawna konwersja z ''{0}'' na ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Funkcja zewn\u0119trzna ''{0}'' jest nieobs\u0142ugiwana przez XSLTC."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Nieznany typ argumentu w wyra\u017ceniu r\u00f3wno\u015bci."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Niepoprawny typ argumentu ''{0}'' w wywo\u0142aniu funkcji ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Pr\u00f3ba sformatowania liczby ''{0}'' za pomoc\u0105 wzorca ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Nie mo\u017cna utworzy\u0107 kopii iteratora ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iterator dla osi ''{0}'' jest nieobs\u0142ugiwany."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iterator dla osi ''{0}'' okre\u015blonego typu jest nieobs\u0142ugiwany."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Atrybut ''{0}'' znajduje si\u0119 poza elementem."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Deklaracja przestrzeni nazw ''{0}''=''{1}'' znajduje si\u0119 poza elementem."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "Nie zadeklarowano przestrzeni nazw dla przedrostka ''{0}''."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "Utworzono DOMAdapter za pomoc\u0105 \u017ar\u00f3d\u0142owego DOM o b\u0142\u0119dnym typie."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "U\u017cywany analizator sk\u0142adni SAX nie obs\u0142uguje zdarze\u0144 deklaracji DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "U\u017cywany analizator sk\u0142adni SAX nie obs\u0142uguje przestrzeni nazw XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "Nie mo\u017cna rozstrzygn\u0105\u0107 odwo\u0142ania do identyfikatora URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "Nieobs\u0142ugiwany element XSL ''{0}''"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Nierozpoznane rozszerzenie XSLTC ''{0}''."},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "Podany translet ''{0}'' zosta\u0142 utworzony za pomoc\u0105 wersji XSLTC, kt\u00f3ra jest nowsza od u\u017cywanego obecnie modu\u0142u wykonawczego XSLTC.  Trzeba zrekompilowa\u0107 arkusz styl\u00f3w lub uruchomi\u0107 ten translet za pomoc\u0105 nowszej wersji XSLTC."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Atrybut, kt\u00f3rego warto\u015bci\u0105 musi by\u0107 nazwa QName, mia\u0142 warto\u015b\u0107 ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Atrybut, kt\u00f3rego warto\u015bci\u0105 musi by\u0107 nazwa NCName, mia\u0142 warto\u015b\u0107 ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "U\u017cycie funkcji rozszerzenia ''{0}'' jest niedozwolone, gdy opcja przetwarzania bezpiecznego jest ustawiona na warto\u015b\u0107 true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "U\u017cycie elementu rozszerzenia ''{0}'' jest niedozwolone, gdy opcja przetwarzania bezpiecznego jest ustawiona na warto\u015b\u0107 true."}
			  };
			}
		}

	}

}