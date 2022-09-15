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
 * $Id: ErrorMessages_cs.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_cs : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Vnit\u0159n\u00ed b\u011bhov\u00e1 chyba ve t\u0159\u00edd\u011b ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Vnit\u0159n\u00ed b\u011bhov\u00e1 chyba p\u0159i prov\u00e1d\u011bn\u00ed funkce <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Neplatn\u00e1 konverze z typu ''{0}'' na typ ''{1}''. "},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Extern\u00ed funkce ''{0}'' nen\u00ed produktem XSLTC podporov\u00e1na. "},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Nezn\u00e1m\u00fd typ argumentu ve v\u00fdrazu rovnosti."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Neplatn\u00fd argument typu ''{0}'' ve vol\u00e1n\u00ed funkce ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Pokus o zform\u00e1tov\u00e1n\u00ed \u010d\u00edsla ''{0}'' s pou\u017eit\u00edm vzorku ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Nelze klonovat iter\u00e1tor ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iter\u00e1tor pro osu ''{0}'' nen\u00ed podporov\u00e1n."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iter\u00e1tor pro typizovanou osu ''{0}'' nen\u00ed podporov\u00e1n."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Atribut ''{0}'' se nach\u00e1z\u00ed vn\u011b prvku."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Deklarace oboru n\u00e1zv\u016f ''{0}''=''{1}'' se nach\u00e1z\u00ed vn\u011b prvku."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "Obor n\u00e1zv\u016f pro p\u0159edponu ''{0}'' nebyl deklarov\u00e1n."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter byl vytvo\u0159en s pou\u017eit\u00edm chybn\u00e9ho typu zdroje DOM."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "Pou\u017eit\u00fd analyz\u00e1tor SAX nem\u016f\u017ee manipulovat s deklara\u010dn\u00edmi ud\u00e1lostmi DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "Pou\u017eit\u00fd analyz\u00e1tor SAX nem\u016f\u017ee podporovat obory n\u00e1zv\u016f pro XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "Nelze p\u0159elo\u017eit odkaz na URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "Nepodporovan\u00fd prvek XSL ''{0}''"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Nerozpoznan\u00e1 p\u0159\u00edpona XSLTC ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "Ur\u010den\u00fd translet ''{0}'' byl vytvo\u0159en pomoc\u00ed verze prost\u0159ed\u00ed XSLTC, kter\u00e1 je nov\u011bj\u0161\u00ed ne\u017e verze pou\u017e\u00edvan\u00e9ho b\u011bhov\u00e9ho prost\u0159ed\u00ed XSLTC. P\u0159edlohu se styly je t\u0159eba znovu zkompilovat nebo tento translet spustit v nov\u011bj\u0161\u00ed verzi prost\u0159ed\u00ed XSLTC."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Atribut, jeho\u017e hodnotou mus\u00ed b\u00fdt jm\u00e9no QName, m\u00e1 hodnotu ''{0}''. "},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Atribut, jeho\u017e hodnotou mus\u00ed b\u00fdt jm\u00e9no NCName, m\u00e1 hodnotu ''{0}''. "},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "Je-li funkce zabezpe\u010den\u00e9ho zpracov\u00e1n\u00ed nastavena na hodnotu true, nen\u00ed povoleno pou\u017eit\u00ed roz\u0161i\u0159uj\u00edc\u00ed funkce ''{0}''. "},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "Je-li funkce zabezpe\u010den\u00e9ho zpracov\u00e1n\u00ed nastavena na hodnotu true, nen\u00ed povoleno pou\u017eit\u00ed roz\u0161i\u0159uj\u00edc\u00edho prvku ''{0}''. "}
			  };
			}
		}

	}

}