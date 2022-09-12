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
 * $Id: ErrorMessages_pt_BR.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_pt_BR : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Erro interno de tempo de execu\u00e7\u00e3o em ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Erro de tempo de execu\u00e7\u00e3o ao executar <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Convers\u00e3o inv\u00e1lida de ''{0}'' em ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Fun\u00e7\u00e3o externa ''{0}'' n\u00e3o suportada por XSLTC."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Tipo de argumento desconhecido na express\u00e3o de igualdade. "},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Tipo de argumento inv\u00e1lido ''{0}'' na chamada para ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Tentando formatar o n\u00famero ''{0}'' utilizando o padr\u00e3o ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "N\u00e3o \u00e9 poss\u00edvel clonar o iterador ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "O iterador do eixo ''{0}'' n\u00e3o \u00e9 suportado. "},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "O iterador do eixo digitado ''{0}'' n\u00e3o \u00e9 suportado. "},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Atributo ''{0}'' fora do elemento. "},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Declara\u00e7\u00e3o de espa\u00e7o de nomes ''{0}''=''{1}'' fora do elemento. "},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "O espa\u00e7o de nomes do prefixo ''{0}'' n\u00e3o foi declarado. "},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter criado utilizando tipo incorreto de DOM de origem."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "O analisador SAX que est\u00e1 sendo utilizado n\u00e3o trata de eventos de declara\u00e7\u00e3o de DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "O analisador SAX que est\u00e1 sendo utilizado n\u00e3o possui suporte para Namespaces XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "N\u00e3o foi poss\u00edvel resolver a refer\u00eancia de URI ''{0}''."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "Elemento XSL n\u00e3o suportado ''{0}''"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "Extens\u00e3o XSLTC n\u00e3o reconhecida ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "O translet especificado, ''{0}'', foi criado com o uso de uma vers\u00e3o do XSLTC mais recente que a vers\u00e3o do tempo de execu\u00e7\u00e3o XSLTC atualmente em uso. \u00c9 necess\u00e1rio recompilar a folha de estilo ou utilizar uma vers\u00e3o mais recente do XSLTC para executar esse translet."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "Um atributo cujo valor deve ser um QName apresentou o valor ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "Um atributo cujo valor deve ser um NCName apresentou o valor ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "O uso da fun\u00e7\u00e3o de extens\u00e3o ''{0}'' n\u00e3o \u00e9 permitido quando o recurso de processamento seguro \u00e9 definido como true."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "O uso do elemento de extens\u00e3o ''{0}'' n\u00e3o \u00e9 permitido quando o recurso de processamento seguro \u00e9 definido como true."}
			  };
			}
		}

	}

}