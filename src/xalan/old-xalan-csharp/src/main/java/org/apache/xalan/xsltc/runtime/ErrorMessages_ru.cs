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
 * $Id: ErrorMessages_ru.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ru : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "\u0412\u043d\u0443\u0442\u0440\u0435\u043d\u043d\u044f\u044f \u043e\u0448\u0438\u0431\u043a\u0430 \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d\u0438\u044f \u0432 ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "\u041e\u0448\u0438\u0431\u043a\u0430 \u0432\u0440\u0435\u043c\u0435\u043d\u0438 \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d\u0438\u044f \u043f\u0440\u0438 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u043a\u0435 <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e\u0435 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u0435 \u0438\u0437 ''{0}'' \u0432 ''{1}''. "},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "\u0412\u043d\u0435\u0448\u043d\u044f\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u044f ''{0}'' \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f XSLTC. "},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u044b\u0439 \u0442\u0438\u043f \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u0430 \u0432 \u0432\u044b\u0440\u0430\u0436\u0435\u043d\u0438\u0438 \u0440\u0430\u0432\u0435\u043d\u0441\u0442\u0432\u0430."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "\u041d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u044b\u0439 \u0442\u0438\u043f \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u0430 ''{0}'' \u043f\u0440\u0438 \u0432\u044b\u0437\u043e\u0432\u0435 ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u043e\u0442\u0444\u043e\u0440\u043c\u0430\u0442\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0447\u0438\u0441\u043b\u043e ''{0}'' \u0441 \u043f\u043e\u043c\u043e\u0449\u044c\u044e \u0448\u0430\u0431\u043b\u043e\u043d\u0430 ''{1}''. "},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u0441\u043e\u0437\u0434\u0430\u0442\u044c \u0434\u0443\u0431\u043b\u0438\u043a\u0430\u0442 \u0441\u0447\u0435\u0442\u0447\u0438\u043a\u0430 ''{0}''. "},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "\u0421\u0447\u0435\u0442\u0447\u0438\u043a \u0434\u043b\u044f \u043e\u0441\u0438 ''{0}'' \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f. "},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "\u0421\u0447\u0435\u0442\u0447\u0438\u043a \u0434\u043b\u044f \u0443\u043a\u0430\u0437\u0430\u043d\u043d\u043e\u0439 \u043e\u0441\u0438 ''{0}'' \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442\u0441\u044f. "},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "\u0410\u0442\u0440\u0438\u0431\u0443\u0442 ''{0}'' \u0432\u043d\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430. "},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "\u041e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u0435 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d ''{0}''=''{1}'' \u0432\u043d\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430. "},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "\u041f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u043e \u0438\u043c\u0435\u043d \u0434\u043b\u044f \u043f\u0440\u0435\u0444\u0438\u043a\u0441\u0430 ''{0}'' \u043d\u0435 \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u043e. "},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter \u0441\u043e\u0437\u0434\u0430\u043d \u0441 \u043d\u0435\u043f\u0440\u0430\u0432\u0438\u043b\u044c\u043d\u044b\u043c \u0442\u0438\u043f\u043e\u043c \u0438\u0441\u0445\u043e\u0434\u043d\u043e\u0433\u043e DOM."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0435\u043c\u044b\u0439 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 SAX \u043d\u0435 \u043e\u0431\u0440\u0430\u0431\u0430\u0442\u044b\u0432\u0430\u0435\u0442 \u0441\u043e\u0431\u044b\u0442\u0438\u044f \u043e\u0431\u044a\u044f\u0432\u043b\u0435\u043d\u0438\u044f DTD."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "\u041f\u0440\u0438\u043c\u0435\u043d\u044f\u0435\u043c\u044b\u0439 \u0430\u043d\u0430\u043b\u0438\u0437\u0430\u0442\u043e\u0440 SAX \u043d\u0435 \u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u0442 \u043f\u0440\u043e\u0441\u0442\u0440\u0430\u043d\u0441\u0442\u0432\u0430 \u0438\u043c\u0435\u043d XML."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "\u041d\u0435\u0432\u043e\u0437\u043c\u043e\u0436\u043d\u043e \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u0430\u0442\u044c \u0441\u0441\u044b\u043b\u043a\u0443 \u043d\u0430 URI ''{0}''. "},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "\u041d\u0435\u043f\u043e\u0434\u0434\u0435\u0440\u0436\u0438\u0432\u0430\u0435\u043c\u044b\u0439 \u044d\u043b\u0435\u043c\u0435\u043d\u0442 XSL ''{0}'' "},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "\u041d\u0435\u0438\u0437\u0432\u0435\u0441\u0442\u043d\u043e\u0435 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u0435 XSLTC ''{0}''"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "\u0423\u043a\u0430\u0437\u0430\u043d\u043d\u0430\u044f \u043f\u0440\u043e\u0446\u0435\u0434\u0443\u0440\u0430 \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f ''{0}'' \u0431\u044b\u043b\u0430 \u0441\u043e\u0437\u0434\u0430\u043d\u0430 \u0441 \u043f\u043e\u043c\u043e\u0449\u044c\u044e \u0431\u043e\u043b\u0435\u0435 \u043d\u043e\u0432\u043e\u0439 \u0432\u0435\u0440\u0441\u0438\u0438 XSLTC, \u0447\u0435\u043c \u0438\u0441\u043f\u043e\u043b\u044c\u0437\u0443\u0435\u043c\u0430\u044f \u0434\u043b\u044f \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d\u0438\u044f \u0432\u0435\u0440\u0441\u0438\u044f XSLTC. \u0414\u043b\u044f \u0432\u044b\u043f\u043e\u043b\u043d\u0435\u043d\u0438\u044f \u044d\u0442\u043e\u0433\u043e \u043f\u0440\u0435\u043e\u0431\u0440\u0430\u0437\u043e\u0432\u0430\u043d\u0438\u044f \u0441\u043b\u0435\u0434\u0443\u0435\u0442 \u043f\u0435\u0440\u0435\u043a\u043e\u043c\u043f\u0438\u043b\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0442\u0430\u0431\u043b\u0438\u0446\u0443 \u0441\u0442\u0438\u043b\u0435\u0439 \u0438\u043b\u0438 \u0443\u0441\u0442\u0430\u043d\u043e\u0432\u0438\u0442\u044c \u0431\u043e\u043b\u0435\u0435 \u043f\u043e\u0437\u0434\u043d\u044e\u044e \u0432\u0435\u0440\u0441\u0438\u044e XSLTC. "},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "\u0412 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0435, \u0434\u043b\u044f \u043a\u043e\u0442\u043e\u0440\u043e\u0433\u043e \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 QName, \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "\u0412 \u0430\u0442\u0440\u0438\u0431\u0443\u0442\u0435, \u0434\u043b\u044f \u043a\u043e\u0442\u043e\u0440\u043e\u0433\u043e \u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 NCName, \u0443\u043a\u0430\u0437\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "\u041f\u0440\u0438\u043c\u0435\u043d\u0435\u043d\u0438\u0435 \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f ''{0}'' \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e, \u0435\u0441\u043b\u0438 \u0434\u043b\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0437\u0430\u0449\u0438\u0449\u0435\u043d\u043d\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u043a\u0438 \u0437\u0430\u0434\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 true. "},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "\u041f\u0440\u0438\u043c\u0435\u043d\u0435\u043d\u0438\u0435 \u044d\u043b\u0435\u043c\u0435\u043d\u0442\u0430 \u0440\u0430\u0441\u0448\u0438\u0440\u0435\u043d\u0438\u044f ''{0}'' \u043d\u0435\u0434\u043e\u043f\u0443\u0441\u0442\u0438\u043c\u043e, \u0435\u0441\u043b\u0438 \u0434\u043b\u044f \u0444\u0443\u043d\u043a\u0446\u0438\u0438 \u0437\u0430\u0449\u0438\u0449\u0435\u043d\u043d\u043e\u0439 \u043e\u0431\u0440\u0430\u0431\u043e\u0442\u043a\u0438 \u0437\u0430\u0434\u0430\u043d\u043e \u0437\u043d\u0430\u0447\u0435\u043d\u0438\u0435 true. "}
			  };
			}
		}

	}

}