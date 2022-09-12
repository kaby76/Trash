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

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_sl : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Notranja napaka izvajanja v ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Notranja napaka izvajanja pri izvajanju <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Neveljavna pretvorba iz ''{0}'' v ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "XSLTC ne podpira zunanje funkcije ''{0}''."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Neznan tip argumenta v izrazu enakovrednosti."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Neveljavna vrsta argumenta ''{0}'' pri klicu na ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Pokus nastavitve formata \u0161tevilke ''{0}'' z uporabo vzorca ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Iteratorja ''{0}'' ni mogo\u010de klonirati."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iterator za os ''{0}'' ni podprt."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iterator za tipizirano os ''{0}'' ni podprt."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Atribut ''{0}'' zunaj elementa."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Deklaracija imenskega prostora ''{0}''=''{1}'' je zunaj elementa."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "Imenski prostor za predpono ''{0}'' ni bil naveden."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter ustvarjen z uporabo napa\u010dnega tipa izvornega DOM."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "Uporabljeni raz\u010dlenjevalnik SAX ne obravnava dogodkov deklaracije DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "Uporabljeni raz\u010dlenjevalnik SAX ne podpira imenskih prostorov XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "Ni mogo\u010de razre\u0161iti sklica URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "Nepodprt XSL element ''{0}''"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Neprepoznana raz\u0161iritev XSLTC ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "Navedeni translet, ''{0}'', je bil ustvarjen z uporabo XSLTC novej\u0161e razli\u010dice, kot je trenutno uporabljana razli\u010dica izvajalnega okolja XSLTC. Slogovno datoteko morate ponovno prevesti ali pa uporabiti novej\u0161o razli\u010dico XSLTC-ja, da bi zagnali ta translet."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Atribut, katerega vrednost mora biti QName, je imel vrednost ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Atribut, katerega vrednost mora biti NCName, je imel vrednost ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "Uporaba raz\u0161iritvene funkcije ''{0}'' ni dovoljena, ko je funkcija varne obdelave nastavljena na True."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "Uporaba raz\u0161iritvene elementa ''{0}'' ni dovoljena, ko je funkcija varne obdelave nastavljena na True."}
			  };
			}
		}

	}

}