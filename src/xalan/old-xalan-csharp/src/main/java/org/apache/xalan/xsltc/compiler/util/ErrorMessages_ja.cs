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
 * $Id: ErrorMessages_ja.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ja : ListResourceBundle
	{

	/*
	 * XSLTC compile-time error messages.
	 *
	 * General notes to translators and definitions:
	 *
	 *   1) XSLTC is the name of the product.  It is an acronym for "XSLT Compiler".
	 *      XSLT is an acronym for "XML Stylesheet Language: Transformations".
	 *
	 *   2) A stylesheet is a description of how to transform an input XML document
	 *      into a resultant XML document (or HTML document or text).  The
	 *      stylesheet itself is described in the form of an XML document.
	 *
	 *   3) A template is a component of a stylesheet that is used to match a
	 *      particular portion of an input document and specifies the form of the
	 *      corresponding portion of the output document.
	 *
	 *   4) An axis is a particular "dimension" in a tree representation of an XML
	 *      document; the nodes in the tree are divided along different axes.
	 *      Traversing the "child" axis, for instance, means that the program
	 *      would visit each child of a particular node; traversing the "descendant"
	 *      axis means that the program would visit the child nodes of a particular
	 *      node, their children, and so on until the leaf nodes of the tree are
	 *      reached.
	 *
	 *   5) An iterator is an object that traverses nodes in a tree along a
	 *      particular axis, one at a time.
	 *
	 *   6) An element is a mark-up tag in an XML document; an attribute is a
	 *      modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *      "elem" is an element name, "attr" and "attr2" are attribute names with
	 *      the values "val" and "val2", respectively.
	 *
	 *   7) A namespace declaration is a special attribute that is used to associate
	 *      a prefix with a URI (the namespace).  The meanings of element names and
	 *      attribute names that use that prefix are defined with respect to that
	 *      namespace.
	 *
	 *   8) DOM is an acronym for Document Object Model.  It is a tree
	 *      representation of an XML document.
	 *
	 *      SAX is an acronym for the Simple API for XML processing.  It is an API
	 *      used inform an XML processor (in this case XSLTC) of the structure and
	 *      content of an XML document.
	 *
	 *      Input to the stylesheet processor can come from an XML parser in the
	 *      form of a DOM tree or through the SAX API.
	 *
	 *   9) DTD is a document type declaration.  It is a way of specifying the
	 *      grammar for an XML file, the names and types of elements, attributes,
	 *      etc.
	 *
	 *  10) XPath is a specification that describes a notation for identifying
	 *      nodes in a tree-structured representation of an XML document.  An
	 *      instance of that notation is referred to as an XPath expression.
	 *
	 *  11) Translet is an invented term that refers to the class file that contains
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "\u8907\u6570\u306e\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u304c\u540c\u3058\u30d5\u30a1\u30a4\u30eb\u5185\u306b\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8 ''{0}'' \u306f\u3053\u306e\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u5185\u306b\u3059\u3067\u306b\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8 ''{0}'' \u306f\u3053\u306e\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u5185\u306b\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "\u5909\u6570 ''{0}'' \u306f\u540c\u4e00\u30b9\u30b3\u30fc\u30d7\u306b\u8907\u6570\u56de\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "\u5909\u6570\u307e\u305f\u306f\u30d1\u30e9\u30e1\u30fc\u30bf\u30fc ''{0}'' \u304c\u672a\u5b9a\u7fa9\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "\u30af\u30e9\u30b9 ''{0}'' \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "\u5916\u90e8\u30e1\u30bd\u30c3\u30c9 ''{0}'' \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093 (public \u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093)"},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "\u30e1\u30bd\u30c3\u30c9 ''{0}'' \u3078\u306e\u547c\u3073\u51fa\u3057\u306e\u5f15\u6570/\u623b\u308a\u5024\u306e\u578b\u3092\u5909\u63db\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "\u30d5\u30a1\u30a4\u30eb\u307e\u305f\u306f URI ''{0}'' \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI ''{0}'' \u304c\u7121\u52b9\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "\u30d5\u30a1\u30a4\u30eb\u307e\u305f\u306f URI ''{0}'' \u3092\u30aa\u30fc\u30d7\u30f3\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "<xsl:stylesheet> \u307e\u305f\u306f <xsl:transform> \u8981\u7d20\u304c\u5fc5\u8981\u3067\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "\u540d\u524d\u7a7a\u9593\u63a5\u982d\u90e8 ''{0}'' \u304c\u5ba3\u8a00\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "\u95a2\u6570 ''{0}'' \u3078\u306e\u547c\u3073\u51fa\u3057\u3092\u89e3\u6c7a\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "''{0}'' \u3078\u306e\u5f15\u6570\u306f\u30ea\u30c6\u30e9\u30eb\u30fb\u30b9\u30c8\u30ea\u30f3\u30b0\u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "XPath \u5f0f ''{0}'' \u3092\u69cb\u6587\u89e3\u6790\u4e2d\u306b\u30a8\u30e9\u30fc\u304c\u767a\u751f\u3057\u307e\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "\u5fc5\u8981\u5c5e\u6027 ''{0}'' \u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "XPath \u5f0f\u5185\u306b\u6b63\u3057\u304f\u306a\u3044\u6587\u5b57 ''{0}'' \u304c\u3042\u308a\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "\u51e6\u7406\u547d\u4ee4\u306e\u540d\u524d ''{0}'' \u304c\u6b63\u3057\u304f\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "\u5c5e\u6027 ''{0}'' \u304c\u8981\u7d20\u306e\u5916\u5074\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "\u5c5e\u6027 ''{0}'' \u304c\u6b63\u3057\u304f\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "import/include \u304c\u76f8\u4e92\u4f9d\u5b58\u3057\u3066\u3044\u307e\u3059\u3002\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8 ''{0}'' \u306f\u3059\u3067\u306b\u30ed\u30fc\u30c9\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "\u7d50\u679c\u30c4\u30ea\u30fc\u30fb\u30d5\u30e9\u30b0\u30e1\u30f3\u30c8\u3092\u30bd\u30fc\u30c8\u3067\u304d\u307e\u305b\u3093 (<xsl:sort> \u8981\u7d20\u306f\u7121\u8996\u3055\u308c\u307e\u3059)\u3002\u3053\u306e\u30ce\u30fc\u30c9\u306f\u7d50\u679c\u30c4\u30ea\u30fc\u306e\u4f5c\u6210\u6642\u306b\u30bd\u30fc\u30c8\u3057\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "10 \u9032\u6570\u30d5\u30a9\u30fc\u30de\u30c3\u30c8 ''{0}'' \u306f\u3059\u3067\u306b\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL \u30d0\u30fc\u30b8\u30e7\u30f3 ''{0}'' \u306f XSLTC \u306b\u3088\u3063\u3066\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "''{0}'' \u306e\u5909\u6570/\u30d1\u30e9\u30e1\u30fc\u30bf\u30fc\u306e\u53c2\u7167\u304c\u76f8\u4e92\u4f9d\u5b58\u3057\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "2 \u9032\u5f0f\u306e\u6f14\u7b97\u5b50\u304c\u4e0d\u660e\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "\u95a2\u6570\u547c\u3073\u51fa\u3057\u306e\u5f15\u6570 (1 \u3064\u4ee5\u4e0a) \u304c\u6b63\u3057\u304f\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "document() \u95a2\u6570\u3078\u306e 2 \u3064\u76ee\u306e\u5f15\u6570\u306f\u30ce\u30fc\u30c9\u30fb\u30bb\u30c3\u30c8\u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "\u5c11\u306a\u304f\u3068\u3082 1 \u3064\u306e <xsl:when> \u8981\u7d20\u304c <xsl:choose> \u5185\u306b\u5fc5\u8981\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "<xsl:choose> \u3067\u8a31\u53ef\u3055\u308c\u3066\u3044\u308b <xsl:otherwise> \u8981\u7d20\u306f 1 \u3064\u3060\u3051\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> \u3092\u4f7f\u7528\u3067\u304d\u308b\u306e\u306f <xsl:choose> \u5185\u3060\u3051\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> \u3092\u4f7f\u7528\u3067\u304d\u308b\u306e\u306f <xsl:choose> \u5185\u3060\u3051\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "<xsl:choose> \u3067\u8a31\u53ef\u3055\u308c\u3066\u3044\u308b\u306e\u306f <xsl:when> \u304a\u3088\u3073 <xsl:otherwise> \u8981\u7d20\u3060\u3051\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> \u306b 'name' \u5c5e\u6027\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "\u5b50\u8981\u7d20\u304c\u6b63\u3057\u304f\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "\u8981\u7d20\u306b ''{0}'' \u3068\u3044\u3046\u540d\u524d\u3092\u4ed8\u3051\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "\u5c5e\u6027\u306b ''{0}'' \u3068\u3044\u3046\u540d\u524d\u3092\u4ed8\u3051\u308b\u3053\u3068\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "\u30c6\u30ad\u30b9\u30c8\u30fb\u30c7\u30fc\u30bf\u304c\u6700\u4e0a\u4f4d\u306e <xsl:stylesheet> \u8981\u7d20\u306e\u5916\u5074\u306b\u3042\u308a\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP \u30d1\u30fc\u30b5\u30fc\u306f\u6b63\u3057\u304f\u69cb\u6210\u3055\u308c\u3066\u3044\u307e\u305b\u3093"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "\u30ea\u30ab\u30d0\u30ea\u30fc\u4e0d\u80fd XSLTC \u5185\u90e8\u30a8\u30e9\u30fc: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "XSL \u8981\u7d20 ''{0}'' \u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "XSLTC \u62e1\u5f35\u6a5f\u80fd ''{0}'' \u306f\u8a8d\u8b58\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "\u5165\u529b\u6587\u66f8\u306f\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3067\u306f\u3042\u308a\u307e\u305b\u3093 (XSL \u540d\u524d\u7a7a\u9593\u306f\u30eb\u30fc\u30c8\u8981\u7d20\u5185\u3067\u5ba3\u8a00\u3055\u308c\u3066\u3044\u307e\u305b\u3093)"},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u30fb\u30bf\u30fc\u30b2\u30c3\u30c8 ''{0}'' \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "''{0}'' \u304c\u5b9f\u88c5\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "\u5165\u529b\u6587\u66f8\u306b\u306f XSL \u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u304c\u5165\u3063\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "\u8981\u7d20 ''{0}'' \u3092\u69cb\u6587\u89e3\u6790\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "<key> \u306e use \u5c5e\u6027\u306f node\u3001node-set\u3001string\u3001\u307e\u305f\u306f number \u3067\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "\u51fa\u529b XML \u6587\u66f8\u306e\u30d0\u30fc\u30b8\u30e7\u30f3\u306f 1.0 \u306b\u306a\u3063\u3066\u3044\u308b\u306f\u305a\u3067\u3059"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "\u95a2\u4fc2\u5f0f\u306e\u6f14\u7b97\u5b50\u304c\u4e0d\u660e\u3067\u3059"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "\u5b58\u5728\u3057\u306a\u3044\u5c5e\u6027\u30bb\u30c3\u30c8 ''{0}'' \u3092\u4f7f\u7528\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "\u5c5e\u6027\u5024\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8 ''{0}'' \u3092\u69cb\u6587\u89e3\u6790\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "\u30af\u30e9\u30b9 ''{0}'' \u306e\u30b7\u30b0\u30cb\u30c1\u30e3\u30fc\u5185\u306e\u30c7\u30fc\u30bf\u578b\u304c\u4e0d\u660e\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "\u30c7\u30fc\u30bf\u578b ''{0}'' \u3092 ''{1}'' \u306b\u5909\u63db\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "\u3053\u306e\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306b\u306f\u6709\u52b9\u306a translet \u30af\u30e9\u30b9\u5b9a\u7fa9\u304c\u5165\u3063\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "\u3053\u306e Templates \u306b\u306f\u540d\u524d\u304c ''{0}'' \u306e\u30af\u30e9\u30b9\u306f\u542b\u307e\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "translet \u30af\u30e9\u30b9 ''{0}'' \u3092\u30ed\u30fc\u30c9\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "translet \u30af\u30e9\u30b9\u304c\u30ed\u30fc\u30c9\u3055\u308c\u307e\u3057\u305f\u304c\u3001translet \u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "''{0}'' \u306e ErrorListener \u3092\u30cc\u30eb\u306b\u8a2d\u5b9a\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC \u304c\u30b5\u30dd\u30fc\u30c8\u3057\u3066\u3044\u308b\u306e\u306f StreamSource\u3001SAXSource\u3001\u304a\u3088\u3073 DOMSource \u3060\u3051\u3067\u3059"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "''{0}'' \u306b\u6e21\u3055\u308c\u305f Source \u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u306b\u306f\u5185\u5bb9\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3092\u30b3\u30f3\u30d1\u30a4\u30eb\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory \u306f\u5c5e\u6027 ''{0}'' \u3092\u8a8d\u8b58\u3057\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() \u306f startDocument() \u306e\u524d\u306b\u547c\u3073\u51fa\u3055\u308c\u3066\u3044\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "\u5909\u63db\u30d7\u30ed\u30b0\u30e9\u30e0\u306b\u306f\u30ab\u30d7\u30bb\u30eb\u5316\u3055\u308c\u305f translet \u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "\u5909\u63db\u7d50\u679c\u306e\u51fa\u529b\u30cf\u30f3\u30c9\u30e9\u30fc\u304c\u5b9a\u7fa9\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "''{0}'' \u306b\u6e21\u3055\u308c\u305f Result \u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u304c\u7121\u52b9\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "\u7121\u52b9\u306a Transformer \u30d7\u30ed\u30d1\u30c6\u30a3\u30fc ''{0}'' \u306b\u30a2\u30af\u30bb\u30b9\u3057\u3088\u3046\u3068\u3057\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "SAX2DOM \u30a2\u30c0\u30d7\u30bf\u30fc ''{0}'' \u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() \u304c systemId \u3092\u8a2d\u5b9a\u3057\u306a\u3044\u3067\u547c\u3073\u51fa\u3055\u308c\u3066\u3044\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "\u7d50\u679c\u306f\u30cc\u30eb\u306b\u306f\u306a\u3089\u306a\u3044\u306f\u305a\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "param {0} \u306e\u5024\u306f\u6709\u52b9\u306a Java \u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u3067\u3042\u308b\u5fc5\u8981\u304c\u3042\u308a\u307e\u3059"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "-i \u30aa\u30d7\u30b7\u30e7\u30f3\u306f -o \u30aa\u30d7\u30b7\u30e7\u30f3\u3068\u4e00\u7dd2\u306b\u4f7f\u7528\u3057\u306a\u3051\u308c\u3070\u306a\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "\u5f62\u5f0f\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <output>]\n      [-d <directory>] [-j <jarfile>] [-p <package>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\n\u30aa\u30d7\u30b7\u30e7\u30f3\n   -o <output>    \u540d\u524d <output> \u3092\u751f\u6210\u3055\u308c\u308b translet \u306b\n                  \u5272\u308a\u5f53\u3066\u307e\u3059\u3002\u30c7\u30d5\u30a9\u30eb\u30c8\u3067\u306f\u3001translet \u540d\u306f <stylesheet>\n                  \u540d\u304b\u3089\u6d3e\u751f\u3057\u307e\u3059\u3002\u3053\u306e\u30aa\u30d7\u30b7\u30e7\u30f3\u306f\u8907\u6570\u306e\u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3092\n                \u30b3\u30f3\u30d1\u30a4\u30eb\u3059\u308b\u5834\u5408\u306f\u7121\u8996\u3055\u308c\u307e\u3059\u3002\n-d <directory> translet \u306e\u5b9b\u5148\u30c7\u30a3\u30ec\u30af\u30c8\u30ea\u30fc\u3092\u6307\u5b9a\u3057\u307e\u3059\n -j <jarfile>   translet \u30af\u30e9\u30b9\u3092 <jarfile> \u3068\u3057\u3066\u6307\u5b9a\u3055\u308c\u305f\n                \u540d\u524d\u306e JAR \u30d5\u30a1\u30a4\u30eb\u306b\u30d1\u30c3\u30b1\u30fc\u30b8\u3057\u307e\u3059\n -p <package>   \u751f\u6210\u5f8c\u306e\u3059\u3079\u3066\u306e translet \u30af\u30e9\u30b9\u306b\u30d1\u30c3\u30b1\u30fc\u30b8\u540d\n                \u63a5\u982d\u90e8\u3092\u6307\u5b9a\u3057\u307e\u3059\u3002\n-n             \u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306e\u30a4\u30f3\u30e9\u30a4\u30f3\u5316\u3092\u4f7f\u7528\u53ef\u80fd\u306b\u3057\u307e\u3059 (\u30c6\u30f3\u30d7\u30ec\u30fc\u30c8\u306e\u30a4\u30f3\n                \u30e9\u30a4\u30f3\u5316\u3067\u5e73\u5747\u3068\u3057\u3066\u826f\u597d\u306a\u30d1\u30d5\u30a9\u30fc\u30de\u30f3\u30b9\u3092\u5f97\u308b\u3053\u3068\u304c\u3067\u304d\u307e\u3059)\n-x             \u8ffd\u52a0\u306e\u30c7\u30d0\u30c3\u30b0\u30fb\u30e1\u30c3\u30bb\u30fc\u30b8\u51fa\u529b\u3092\u30aa\u30f3\u306b\u3057\u307e\u3059\n   -u             <stylesheet> \u5f15\u6570\u3092 URL \u3068\u3057\u3066\u89e3\u91c8\u3057\u307e\u3059\n   -i             \u30b9\u30bf\u30a4\u30eb\u30b7\u30fc\u30c8\u3092\u6a19\u6e96\u5165\u529b\u304b\u3089\u8aad\u307f\u53d6\u308b\u3088\u3046\u30b3\u30f3\u30d1\u30a4\u30e9\u30fc\u306b\u5f37\u5236\u3057\u307e\u3059\n   -v             \u30b3\u30f3\u30d1\u30a4\u30e9\u30fc\u306e\u30d0\u30fc\u30b8\u30e7\u30f3\u3092\u5370\u5237\u3057\u307e\u3059\n   -h             \u3053\u306e\u4f7f\u7528\u6cd5\u3092\u5370\u5237\u3057\u307e\u3059\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "\u5f62\u5f0f\n   java org.apache.xalan.xsltc.cmdline.Transform [-j <jarfile>]\n      [-x] [-n <iterations>] {-u <document_url> | <document>}\n      <class> [<param1>=<value1> ...]\n\n   translet <class> \u3092\u4f7f\u7528\u3057\u3066 <document> \u3067\u6307\u5b9a\u3055\u308c\u305f\n   XML \u6587\u66f8\u3092\u5909\u63db\u3057\u307e\u3059\u3002translet <class> \u306f\u30e6\u30fc\u30b6\u30fc\u306e CLASSPATH\n \u307e\u305f\u306f\u30aa\u30d7\u30b7\u30e7\u30f3\u3067\u6307\u5b9a\u3055\u308c\u308b <jarfile> \u306b\u5165\u3063\u3066\u3044\u307e\u3059\u3002\n\u30aa\u30d7\u30b7\u30e7\u30f3\n   -j <jarfile>    \u30ed\u30fc\u30c9\u3059\u308b translet \u304c\u5165\u3063\u3066\u3044\u308b JAR \u30d5\u30a1\u30a4\u30eb\u3092\u6307\u5b9a\u3057\u307e\u3059\n   -x              \u8ffd\u52a0\u306e\u30c7\u30d0\u30c3\u30b0\u30fb\u30e1\u30c3\u30bb\u30fc\u30b8\u51fa\u529b\u3092\u30aa\u30f3\u306b\u3057\u307e\u3059\n   -n <iterations> \u306f\u5909\u63db\u3092 <iterations> \u56de\u5b9f\u884c\u3057\u3066\n                   \u30d7\u30ed\u30d5\u30a1\u30a4\u30eb\u60c5\u5831\u3092\u8868\u793a\u3057\u307e\u3059\n   -u <document_url> \u306f XML \u5165\u529b\u6587\u66f8\u3092 URL \u3068\u3057\u3066\u6307\u5b9a\u3057\u307e\u3059\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> \u3092\u4f7f\u7528\u3067\u304d\u308b\u306e\u306f <xsl:for-each> \u307e\u305f\u306f <xsl:apply-templates> \u5185\u3060\u3051\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "\u51fa\u529b\u30a8\u30f3\u30b3\u30fc\u30c9 ''{0}'' \u306f\u3053\u306e JVM \u3067\u306f\u30b5\u30dd\u30fc\u30c8\u3055\u308c\u3066\u3044\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.SYNTAX_ERR, "''{0}'' \u306b\u69cb\u6587\u30a8\u30e9\u30fc\u304c\u3042\u308a\u307e\u3059\u3002"},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "\u5916\u90e8\u30b3\u30f3\u30b9\u30c8\u30e9\u30af\u30bf\u30fc ''{0}'' \u304c\u898b\u3064\u304b\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "\u975e static Java \u95a2\u6570 ''{0}'' \u3078\u306e\u5148\u982d\u306e\u5f15\u6570\u306f\u6709\u52b9\u306a\u30aa\u30d6\u30b8\u30a7\u30af\u30c8\u53c2\u7167\u3067\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "\u5f0f ''{0}'' \u306e\u578b\u3092\u691c\u67fb\u4e2d\u306b\u30a8\u30e9\u30fc\u304c\u767a\u751f\u3057\u307e\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "\u4e0d\u660e\u306a\u30ed\u30b1\u30fc\u30b7\u30e7\u30f3\u3067\u5f0f\u306e\u578b\u3092\u691c\u67fb\u4e2d\u306b\u30a8\u30e9\u30fc\u304c\u767a\u751f\u3057\u307e\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "\u30b3\u30de\u30f3\u30c9\u884c\u30aa\u30d7\u30b7\u30e7\u30f3 ''{0}'' \u304c\u7121\u52b9\u3067\u3059\u3002"},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "\u30b3\u30de\u30f3\u30c9\u884c\u30aa\u30d7\u30b7\u30e7\u30f3 ''{0}'' \u306b\u5fc5\u8981\u306a\u5f15\u6570\u304c\u3042\u308a\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "\u8b66\u544a:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "\u8b66\u544a:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "\u81f4\u547d\u7684\u30a8\u30e9\u30fc:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "\u81f4\u547d\u7684\u30a8\u30e9\u30fc:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "\u30a8\u30e9\u30fc:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "\u30a8\u30e9\u30fc:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "translet ''{0}'' \u3092\u4f7f\u7528\u3059\u308b\u5909\u63db"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "JAR \u30d5\u30a1\u30a4\u30eb ''{1}'' \u306e translet ''{0}'' \u3092\u4f7f\u7528\u3059\u308b\u5909\u63db"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "TransformerFactory \u30af\u30e9\u30b9 ''{0}'' \u306e\u30a4\u30f3\u30b9\u30bf\u30f3\u30b9\u3092\u4f5c\u6210\u3067\u304d\u307e\u305b\u3093\u3067\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "\u540d\u524d ''{0}'' \u306f translet \u30af\u30e9\u30b9\u306e\u540d\u524d\u3068\u3057\u3066\u4f7f\u7528\u3067\u304d\u307e\u305b\u3093\u3002Java \u30af\u30e9\u30b9\u306e\u540d\u524d\u306b\u8a31\u53ef\u3055\u308c\u3066\u3044\u306a\u3044\u6587\u5b57\u304c\u542b\u307e\u308c\u3066\u3044\u308b\u305f\u3081\u3067\u3059\u3002\u4ee3\u308f\u308a\u306b\u3001\u540d\u524d ''{1}'' \u304c\u4f7f\u7528\u3055\u308c\u307e\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "\u30b3\u30f3\u30d1\u30a4\u30e9\u30fc\u30fb\u30a8\u30e9\u30fc:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "\u30b3\u30f3\u30d1\u30a4\u30e9\u30fc\u8b66\u544a:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Translet \u30a8\u30e9\u30fc:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "\u5024\u304c QName \u307e\u305f\u306f\u7a7a\u767d\u3067\u533a\u5207\u3089\u308c\u305f QNames \u306e\u30ea\u30b9\u30c8\u3067\u306a\u3051\u308c\u3070\u306a\u3089\u306a\u3044\u5c5e\u6027\u306b\u3001\u5024 ''{0}'' \u304c\u3042\u308a\u307e\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "\u5024\u304c NCName \u3067\u306a\u3051\u308c\u3070\u306a\u3089\u306a\u3044\u5c5e\u6027\u306b\u3001\u5024 ''{0}'' \u304c\u3042\u308a\u307e\u3057\u305f\u3002"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "<xsl:output> \u8981\u7d20\u306e method \u5c5e\u6027\u306b\u3001\u5024 ''{0}'' \u304c\u3042\u308a\u307e\u3057\u305f\u3002\u5024\u306f ''xml''\u3001''html''\u3001''text''\u3001\u307e\u305f\u306f qname-but-not-ncname \u306e\u3044\u305a\u308c\u304b\u3067\u3042\u308b\u5fc5\u8981\u304c\u3042\u308a\u307e\u3059"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "TransformerFactory.getFeature(String name) \u306e\u6a5f\u80fd\u540d\u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "TransformerFactory.setFeature(String name, boolean value) \u306e\u6a5f\u80fd\u540d\u3092\u30cc\u30eb\u306b\u306f\u3067\u304d\u307e\u305b\u3093\u3002"},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "\u6a5f\u80fd ''{0}'' \u306f\u3053\u306e TransformerFactory \u306b\u8a2d\u5b9a\u3067\u304d\u307e\u305b\u3093\u3002"}
			  };
			}
		}

	}

}