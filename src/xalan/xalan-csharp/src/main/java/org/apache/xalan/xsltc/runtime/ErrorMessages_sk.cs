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
 * $Id: ErrorMessages_sk.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_sk : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "V ''{0}'' sa vyskytla intern\u00e1 runtime chyba"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Chyba \u010dasu spustenia pri sp\u00fa\u0161\u0165an\u00ed <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Konverzia z ''{0}'' na ''{1}'' je neplatn\u00e1."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "XLTC nepodporuje extern\u00fa funkciu ''{0}''."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Nezn\u00e1my typ argumentu je v\u00fdrazom rovnosti."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Vo volan\u00ed do ''{1}'' je neplatn\u00fd typ argumentu ''{0}'' "},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Prebieha pokus o form\u00e1tovanie \u010d\u00edsla ''{0}'' pomocou vzoru ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Iter\u00e1tor ''{0}'' sa ned\u00e1 klonova\u0165."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iter\u00e1tor pre os ''{0}'' nie je podporovan\u00fd."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iter\u00e1tor pre zadan\u00fa os ''{0}'' nie je podporovan\u00fd."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Atrib\u00fat ''{0}'' je mimo prvku."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Deklar\u00e1cia n\u00e1zvov\u00e9ho priestoru ''{0}''=''{1}'' je mimo prvku."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "N\u00e1zvov\u00fd priestor pre predponu ''{0}'' nebol deklarovan\u00fd."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter bol vytvoren\u00fd pomocou nespr\u00e1vneho typu zdrojov\u00e9ho DOM."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "Analyz\u00e1tor SAX, ktor\u00fd pou\u017e\u00edvate, nesprac\u00fava udalosti deklar\u00e1cie DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "Analyz\u00e1tor SAX, ktor\u00fd pou\u017e\u00edvate, nem\u00e1 podporu pre n\u00e1zvov\u00e9 priestory XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "Nebolo mo\u017en\u00e9 rozl\u00ed\u0161i\u0165 odkaz na URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "XSL prvok ''{0}'' nie je podporovan\u00fd"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "XSLTC pr\u00edpona ''{0}'' nebola rozpoznan\u00e1"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "\u0160pecifikovan\u00fd translet ''{0}'' bol vytvoren\u00fd pomocou verzie XSLTC, ktor\u00e1 je nov\u0161ia ako verzia XSLTC runtime, ktor\u00fd sa pou\u017e\u00edva.  Mus\u00edte prekompilova\u0165 defin\u00edcie \u0161t\u00fdlov (objekt stylesheet) alebo pou\u017ei\u0165 na spustenie tohto transletu nov\u0161iu verziu n\u00e1stroja XSLTC."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Atrib\u00fat, ktor\u00fd mus\u00ed ma\u0165 hodnotu QName, mal hodnotu ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Atrib\u00fat, ktor\u00fd mus\u00ed ma\u0165 hodnotu NCName, mal hodnotu ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "Pou\u017e\u00edvanie funkcie roz\u0161\u00edrenia ''{0}'' nie je povolen\u00e9, ke\u010f je funkcia bezpe\u010dn\u00e9ho spracovania nastaven\u00e1 na hodnotu true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "Pou\u017e\u00edvanie prvku roz\u0161\u00edrenia ''{0}'' nie je povolen\u00e9, ke\u010f je funkcia bezpe\u010dn\u00e9ho spracovania nastaven\u00e1 na hodnotu true."}
			  };
			}
		}

	}

}