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
 * $Id: ErrorMessages_ca.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ca : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "S''ha produ\u00eft un error intern de temps d''execuci\u00f3 a ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Es produeix un error de temps d'execuci\u00f3 en executar <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Conversi\u00f3 no v\u00e0lida de ''{0}'' a ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "XSLTC no d\u00f3na suport a la funci\u00f3 externa ''{0}''. "},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "L'expressi\u00f3 d'igualtat cont\u00e9 un tipus d'argument desconegut."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "El tipus d''argument ''{0}'' a la crida de ''{1}'' no \u00e9s v\u00e0lid "},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "S''ha intentat formatar el n\u00famero ''{0}'' fent servir el patr\u00f3 ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "No es pot clonar l''iterador ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "No est\u00e0 suportat l''iterador de l''eix ''{0}''. "},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "No est\u00e0 suportat l''iterador de l''eix escrit ''{0}''. "},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "L''atribut ''{0}'' es troba fora de l''element. "},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "La declaraci\u00f3 de l''espai de noms ''{0}''=''{1}'' es troba fora de l''element. "},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "L''espai de noms del prefix ''{0}'' no s''ha declarat. "},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter s'ha creat mitjan\u00e7ant un tipus incorrecte de DOM d'origen."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "L'analitzador SAX que feu servir no gestiona esdeveniments de declaraci\u00f3 de DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "L'analitzador SAX que feu servir no d\u00f3na suport a espais de noms XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "No s''ha pogut resoldre la refer\u00e8ncia d''URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "L''element XSL ''{0}'' no t\u00e9 suport "},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "No es reconeix l''extensi\u00f3 XSLTC ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "La classe translet especificada, ''{0}'', es va crear fent servir una versi\u00f3 d''XSLTC m\u00e9s recent que la versi\u00f3 del temps d''execuci\u00f3 d''XSLTC que ja s''est\u00e0 utilitzant. Heu de recompilar el full d''estil o fer servir una versi\u00f3 m\u00e9s recent d''XSLTC per executar aquesta classe translet."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Un atribut, que ha de tenir el valor QName, tenia el valor ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Un atribut, que ha de tenir el valor NCName, tenia el valor ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "L''\u00fas de la funci\u00f3 d''extensi\u00f3 ''{0}'' no est\u00e0 perm\u00e8s, si la caracter\u00edstica de proc\u00e9s segur s''ha establert en true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "L''\u00fas de l''element d''extensi\u00f3 ''{0}'' no est\u00e0 perm\u00e8s, si la caracter\u00edstica de proc\u00e9s segur s''ha establert en true."}
			  };
			}
		}

	}

}