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
 * $Id: ErrorMessages_es.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_es : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Error interno de ejecuci\u00f3n en ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Error de ejecuci\u00f3n al ejecutar <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Conversi\u00f3n no v\u00e1lida de ''{0}'' a ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Funci\u00f3n externa ''{0}'' no soportada por XSLTC."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Tipo de argumento desconocido en expresi\u00f3n de igualdad."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Tipo de argumento ''{0}'' no v\u00e1lido en la llamada a ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Se intenta dar formato al n\u00famero ''{0}'' con el patr\u00f3n ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "No se puede replicar el iterador ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iterador para el eje ''{0}'' no soportado."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iterador para el eje escrito ''{0}'' no soportado."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Atributo ''{0}'' fuera del elemento."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Declaraci\u00f3n del espacio de nombres ''{0}''=''{1}'' fuera del elemento."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "No se ha declarado el espacio de nombres para el prefijo ''{0}''."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter creado mediante un tipo incorrecto de DOM origen."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "El analizador SAX utilizado no maneja sucesos de declaraci\u00f3n DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "El analizador SAX utilizado no tiene soporte de espacios de nombres XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "No se ha podido resolver la referencia de URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "Elemento XSL ''{0}'' no soportado."},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Extensi\u00f3n XSLTC ''{0}'' no reconocida."},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "El translet especificado, ''{0}'', se ha creado utilizando una versi\u00f3n de XSLTC m\u00e1s reciente que la versi\u00f3n de ejecuci\u00f3n de XSLTC que est\u00e1 en uso. Debe recompilar la hoja de estilos o utilizar una versi\u00f3n m\u00e1s reciente de XSLTC para ejecutar este translet."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Un atributo cuyo valor debe ser un QName tiene el valor ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Un atributo cuyo valor debe ser un NCName tiene el valor ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "No se permite el uso de la funci\u00f3n de extensi\u00f3n ''{0}'' cuando la caracter\u00edstica de proceso seguro est\u00e1 establecida en true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "No se permite el uso del elemento de extensi\u00f3n ''{0}'' cuando la caracter\u00edstica de proceso seguro est\u00e1 establecida en true."}
			  };
			}
		}

	}

}