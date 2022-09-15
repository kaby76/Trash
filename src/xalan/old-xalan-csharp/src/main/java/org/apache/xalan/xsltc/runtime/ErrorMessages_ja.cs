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
 * $Id: ErrorMessages_ja.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ja : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "''{0}'' \u3067\u30e9\u30f3\u30bf\u30a4\u30e0\u5185\u90e8\u30a8\u30e9\u30fc\u304c\u3042\u308a\u307e\u3057\u305f\u3002"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "<xsl:copy> \u5b9f\u884c\u6642\u306e\u30e9\u30f3\u30bf\u30a4\u30e0\u30fb\u30a8\u30e9\u30fc"},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "''{0}'' \u304b\u3089 ''{1}'' \u3078\u306e\u5909\u63db\u306f\u7121\u52b9\u3067\u3059\u3002"},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "\u5916\u90e8\u95a2\u6570 ''{0}'' \u306f XSLTC \u306b\u3088\u3063\u3066\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "\u7b49\u5f0f\u5185\u306e\u5f15\u6570\u304c\u4e0d\u660e\u3067\u3059\u3002"},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "''{1}'' \u3078\u306e\u547c\u3073\u51fa\u3057\u306e\u5f15\u6570\u306e\u578b ''{0}'' \u304c\u7121\u52b9\u3067\u3059\u3002"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "\u30d1\u30bf\u30fc\u30f3 ''{1}'' \u3092\u4f7f\u7528\u3057\u3066\u6570\u5024 ''{0}'' \u306e\u30d5\u30a9\u30fc\u30de\u30c3\u30c8\u8a2d\u5b9a\u3092\u8a66\u307f\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "\u30a4\u30c6\u30ec\u30fc\u30bf\u30fc ''{0}'' \u3092\u8907\u88fd\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "\u8ef8 ''{0}'' \u306e\u30a4\u30c6\u30ec\u30fc\u30bf\u30fc\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "\u578b\u4ed8\u304d\u306e\u8ef8 ''{0}'' \u306e\u30a4\u30c6\u30ec\u30fc\u30bf\u30fc\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "\u5c5e\u6027 ''{0}'' \u304c\u8981\u7d20\u306e\u5916\u5074\u3067\u3059\u3002"},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "\u540d\u524d\u7a7a\u9593\u5ba3\u8a00 ''{0}''=''{1}'' \u304c\u8981\u7d20\u306e\u5916\u5074\u3067\u3059\u3002"},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "\u63a5\u982d\u90e8 ''{0}'' \u306e\u540d\u524d\u7a7a\u9593\u304c\u5ba3\u8a00\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter \u304c\u9593\u9055\u3063\u305f\u578b\u306e\u30bd\u30fc\u30b9 DOM \u3092\u4f7f\u7528\u3057\u3066\u4f5c\u6210\u3055\u308c\u307e\u3057\u305f\u3002"},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "\u4f7f\u7528\u4e2d\u306e SAX \u30d1\u30fc\u30b5\u30fc\u306f DTD \u5ba3\u8a00\u30a4\u30d9\u30f3\u30c8\u3092\u51e6\u7406\u3057\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "\u4f7f\u7528\u4e2d\u306e SAX \u30d1\u30fc\u30b5\u30fc\u306b\u306f XML \u540d\u524d\u7a7a\u9593\u306e\u30b5\u30dd\u30fc\u30c8\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "URI \u53c2\u7167 ''{0}'' \u3092\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "XSL \u8981\u7d20 ''{0}'' \u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "XSLTC \u62e1\u5f35 ''{0}'' \u306f\u8a8d\u8b58\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "\u6307\u5b9a\u3055\u308c\u305f translet ''{0}'' \u306f\u3001\u4f7f\u7528\u4e2d\u306e XSLTC \u30e9\u30f3\u30bf\u30a4\u30e0\u3088\u308a\u65b0\u3057\u3044\u30d0\u30fc\u30b8\u30e7\u30f3\u306e XSLTC \u3092\u4f7f\u7528\u3057\u3066\u4f5c\u6210\u3055\u308c\u307e\u3057\u305f\u3002\u3053\u306e translet \u3092\u5b9f\u884c\u3059\u308b\u306b\u306f\u3001\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3092\u518d\u30b3\u30f3\u30d1\u30a4\u30eb\u3059\u308b\u304b\u3001\u307e\u305f\u306f\u65b0\u3057\u3044\u30d0\u30fc\u30b8\u30e7\u30f3\u306e XSLTC \u3092\u4f7f\u7528\u3057\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "\u5024\u304c QName \u3067\u306a\u3051\u308c\u3070\u306a\u3089\u306a\u3044\u5c5e\u6027\u306b\u3001\u5024 ''{0}'' \u304c\u3042\u308a\u307e\u3057\u305f\u3002"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "\u5024\u304c NCName \u3067\u306a\u3051\u308c\u3070\u306a\u3089\u306a\u3044\u5c5e\u6027\u306b\u3001\u5024 ''{0}'' \u304c\u3042\u308a\u307e\u3057\u305f\u3002"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "\u30bb\u30ad\u30e5\u30ea\u30c6\u30a3\u30fc\u4fdd\u8b77\u3055\u308c\u305f\u51e6\u7406\u6a5f\u80fd\u304c true \u306b\u8a2d\u5b9a\u3055\u308c\u3066\u3044\u308b\u3068\u304d\u306b\u3001\u62e1\u5f35\u95a2\u6570 ''{0}'' \u3092\u4f7f\u7528\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "\u30bb\u30ad\u30e5\u30ea\u30c6\u30a3\u30fc\u4fdd\u8b77\u3055\u308c\u305f\u51e6\u7406\u6a5f\u80fd\u304c true \u306b\u8a2d\u5b9a\u3055\u308c\u3066\u3044\u308b\u3068\u304d\u306b\u3001\u62e1\u5f35\u8981\u7d20 ''{0}'' \u3092\u4f7f\u7528\u3059\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"}
			  };
			}
		}

	}

}