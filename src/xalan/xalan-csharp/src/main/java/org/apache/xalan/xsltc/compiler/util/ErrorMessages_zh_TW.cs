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
 * $Id: ErrorMessages_zh_TW.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_zh_TW : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "\u540c\u4e00\u500b\u6a94\u6848\u4e2d\u5b9a\u7fa9\u4e00\u500b\u4ee5\u4e0a\u7684\u6a23\u5f0f\u8868\u3002"},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "\u6b64\u6a23\u5f0f\u8868\u4e2d\u5df2\u7d93\u5b9a\u7fa9\u6709\u7bc4\u672c ''{0}''\u3002"},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "\u6b64\u6a23\u5f0f\u8868\u4e2d\u5c1a\u672a\u5b9a\u7fa9\u7bc4\u672c ''{0}''\u3002"},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "\u76f8\u540c\u7684\u7bc4\u570d\u4e2d\u5b9a\u7fa9\u4e86\u591a\u500b\u8b8a\u6578 ''{0}''\u3002"},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "\u8b8a\u6578\u6216\u53c3\u6578 ''{0}'' \u5c1a\u672a\u5b9a\u7fa9\u3002"},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "\u627e\u4e0d\u5230\u985e\u5225 ''{0}''\u3002"},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "\u627e\u4e0d\u5230\u5916\u90e8\u65b9\u6cd5 ''{0}''\uff08\u5fc5\u9808\u662f\u516c\u7528\u7684\uff09\u3002"},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "\u7121\u6cd5\u5c07\u547c\u53eb\u4e2d\u7684\u5f15\u6578/\u50b3\u56de\u985e\u578b\u8f49\u63db\u70ba\u65b9\u6cd5 ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "\u627e\u4e0d\u5230\u6a94\u6848\u6216 URI ''{0}''\u3002"},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI ''{0}'' \u7121\u6548\u3002"},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "\u7121\u6cd5\u958b\u555f\u6a94\u6848\u6216 URI ''{0}''\u3002"},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "\u539f\u9810\u671f\u70ba <xsl:stylesheet> \u6216 <xsl:transform> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "\u540d\u7a31\u7a7a\u9593\u5b57\u9996 ''{0}'' \u5c1a\u672a\u5ba3\u544a\u3002"},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "\u7121\u6cd5\u89e3\u6790\u5c0d\u51fd\u6578 ''{0}'' \u7684\u547c\u53eb\u3002"},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "''{0}'' \u7684\u5f15\u6578\u5fc5\u9808\u662f\u6587\u5b57\u5b57\u4e32\u3002"},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "\u5256\u6790 XPath \u8868\u793a\u5f0f ''{0}'' \u6642\u767c\u751f\u932f\u8aa4\u3002"},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "\u907a\u6f0f\u5fc5\u8981\u7684\u5c6c\u6027 ''{0}''\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "XPath \u8868\u793a\u5f0f\u4e2d\u5305\u542b\u4e0d\u5408\u6cd5\u5b57\u5143 ''{0}''\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "\u8655\u7406\u6307\u793a\u7684\u540d\u7a31 ''{0}'' \u4e0d\u5408\u6cd5\u3002"},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "\u5c6c\u6027 ''{0}'' \u8d85\u51fa\u5143\u7d20\u5916\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "\u5c6c\u6027 ''{0}'' \u4e0d\u5408\u6cd5\u3002"},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "\u5faa\u74b0\u532f\u5165/\u4f75\u5165\u3002\u6a23\u5f0f\u8868 ''{0}'' \u5df2\u7d93\u8f09\u5165\u3002"},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "\u7d50\u679c\u6a39\u7247\u6bb5\u7121\u6cd5\u6392\u5e8f\uff08<xsl:sort> \u5143\u7d20\u88ab\u5ffd\u7565\uff09\u3002\u60a8\u5fc5\u9808\u65bc\u5efa\u7acb\u7d50\u679c\u6a39\u6642\uff0c\u5c07\u7bc0\u9ede\u6392\u5e8f\u3002"},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "\u5df2\u7d93\u5b9a\u7fa9\u6709\u5341\u9032\u4f4d\u683c\u5f0f ''{0}''\u3002"},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL \u7248\u672c ''{0}'' \u4e0d\u53d7 XSLTC \u652f\u63f4\u3002"},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "''{0}'' \u4e2d\u5305\u542b\u5faa\u74b0\u8b8a\u6578/\u53c3\u6578\u53c3\u7167\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "\u4e8c\u9032\u4f4d\u8868\u793a\u5f0f\u7684\u904b\u7b97\u5b50\u4e0d\u660e\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "\u51fd\u6578\u547c\u53eb\u7684\u5f15\u6578\u4e0d\u5408\u6cd5\u3002"},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "document() \u51fd\u6578\u7684\u7b2c\u4e8c\u500b\u5f15\u6578\u5fc5\u9808\u662f\u7bc0\u9ede\u96c6\u3002"},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "<xsl:choose> \u4e2d\u81f3\u5c11\u8981\u6709\u4e00\u500b <xsl:when> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "<xsl:choose> \u4e2d\u53ea\u63a5\u53d7\u4e00\u500b <xsl:otherwise> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> \u53ea\u80fd\u7528\u5728 <xsl:choose> \u4e2d\u3002"},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> \u53ea\u80fd\u7528\u5728 <xsl:choose> \u4e2d\u3002"},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "<xsl:choose> \u4e2d\u53ea\u63a5\u53d7 <xsl:when> \u548c <xsl:otherwise> \u5169\u500b\u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> \u907a\u6f0f 'name' \u5c6c\u6027\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "\u5b50\u9805\u5143\u7d20\u4e0d\u5408\u6cd5\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "\u4e0d\u53ef\u4ee5\u547c\u53eb\u5143\u7d20 ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "\u4e0d\u53ef\u4ee5\u547c\u53eb\u5143\u7d20 ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "\u6587\u5b57\u8cc7\u6599\u8d85\u51fa\u9802\u5c64 <xsl:stylesheet> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP \u5256\u6790\u5668\u672a\u6b63\u78ba\u914d\u7f6e"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "\u767c\u751f\u7121\u6cd5\u5fa9\u539f\u7684 XSLTC \u5167\u90e8\u932f\u8aa4\uff1a''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "XSL \u5143\u7d20 ''{0}'' \u4e0d\u53d7\u652f\u63f4\u3002"},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "XSLTC \u5ef6\u4f38\u9805\u76ee ''{0}'' \u7121\u6cd5\u8fa8\u8b58\u3002"},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "\u8f38\u5165\u6587\u4ef6\u4e0d\u662f\u6a23\u5f0f\u8868\uff08XSL \u540d\u7a31\u7a7a\u9593\u672a\u5728\u6839\u5143\u7d20\u4e2d\u5ba3\u544a\uff09\u3002"},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "\u627e\u4e0d\u5230\u6a23\u5f0f\u8868\u76ee\u6a19 ''{0}''\u3002"},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "\u5c1a\u672a\u5be6\u4f5c\uff1a''{0}''\u3002"},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "\u8f38\u5165\u6587\u4ef6\u672a\u5305\u542b XSL \u6a23\u5f0f\u8868\u3002"},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "\u7121\u6cd5\u5256\u6790\u5143\u7d20 ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "<key> \u7684 use \u5c6c\u6027\u5fc5\u9808\u662f node\u3001node-set\u3001string \u6216 number\u3002"},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "\u8f38\u51fa XML \u6587\u4ef6\u7248\u672c\u61c9\u8a72\u662f 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "\u95dc\u806f\u5f0f\u8868\u793a\u5f0f\u7684\u904b\u7b97\u5b50\u4e0d\u660e"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "\u5617\u8a66\u4f7f\u7528\u4e0d\u5b58\u5728\u7684\u5c6c\u6027\u96c6 ''{0}''\u3002"},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "\u7121\u6cd5\u5256\u6790\u5c6c\u6027\u503c\u7bc4\u672c ''{0}''\u3002"},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "\u985e\u5225 ''{0}'' \u7684\u7c3d\u7ae0\u8cc7\u6599\u985e\u578b\u4e0d\u660e\u3002"},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "\u7121\u6cd5\u5c07\u8cc7\u6599\u985e\u578b ''{0}'' \u8f49\u63db\u70ba ''{1}''\u3002"},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "\u6b64 Templates \u672a\u5305\u542b\u6709\u6548\u7684 translet \u985e\u5225\u5b9a\u7fa9\u3002"},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "\u6b64\u7bc4\u672c\u672a\u5305\u542b\u540d\u7a31\u70ba ''{0}'' \u7684\u985e\u5225\u3002"},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "\u7121\u6cd5\u8f09\u5165 Translet \u985e\u5225 ''{0}''\u3002"},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "\u5df2\u8f09\u5165 Translet \u985e\u5225\uff0c\u4f46\u662f\u7121\u6cd5\u5efa\u7acb translet \u5be6\u4f8b\u3002"},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "\u5617\u8a66\u5c07 ''{0}'' \u7684 ErrorListener \u8a2d\u70ba null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC \u53ea\u652f\u63f4 StreamSource\u3001SAXSource \u8207 DOMSource"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "\u50b3\u905e\u5230 ''{0}'' \u7684 Source \u7269\u4ef6\u6c92\u6709\u5167\u5bb9\u3002"},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "\u7121\u6cd5\u7de8\u8b6f\u6a23\u5f0f\u8868"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory \u7121\u6cd5\u8fa8\u8b58\u5c6c\u6027 ''{0}''\u3002"},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() \u5fc5\u9808\u5728 startDocument() \u4e4b\u524d\u547c\u53eb\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer \u6c92\u6709\u7c21\u5316\u7684 translet \u7269\u4ef6\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "\u6c92\u6709\u5df2\u5b9a\u7fa9\u7684\u8f38\u51fa\u8655\u7406\u7a0b\u5f0f\u4f9b\u8f49\u63db\u7d50\u679c\u4f7f\u7528\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "\u50b3\u905e\u5230 ''{0}'' \u7684 Result \u7269\u4ef6\u7121\u6548\u3002"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "\u5617\u8a66\u5b58\u53d6\u7121\u6548\u7684 Transformer \u5167\u5bb9 ''{0}''\u3002"},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "\u7121\u6cd5\u5efa\u7acb SAX2DOM \u914d\u63a5\u5361\uff1a''{0}''\u3002"},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "\u547c\u53eb XSLTCSource.build() \u6642\uff0c\u672a\u8a2d\u5b9a systemId \u3002"},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "\u7d50\u679c\u4e0d\u61c9\u70ba\u7a7a\u503c"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "\u53c3\u6578 {0} \u7684\u503c\u5fc5\u9808\u662f\u6709\u6548\u7684 Java \u7269\u4ef6"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "-i \u9078\u9805\u5fc5\u9808\u548c -o \u9078\u9805\u4e00\u8d77\u4f7f\u7528\u3002"},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "\u7528\u6cd5\u6982\u8981\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <\u8f38\u51fa>]\n      [-d <directory>] [-j <Jar \u6a94>] [-p <\u5957\u4ef6>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\nOPTIONS\n   -o <\u8f38\u51fa>    \u6307\u5b9a\u540d\u7a31 <\u8f38\u51fa> \u7d66\u7522\u751f\u7684\n                  translet\u3002\u4f9d\u9810\u8a2d\uff0ctranslet \u540d\u7a31\u662f\n                  \u884d\u751f\u81ea <stylesheet> \u540d\u7a31\u3002\u82e5\u7de8\u8b6f\n                  \u591a\u4efd\u6a23\u5f0f\u8868\u6642\uff0c\u6b64\u9078\u9805\u6703\u88ab\u5ffd\u7565\u3002\n   -d <directory> \u6307\u5b9a translet \u7684\u76ee\u6a19\u76ee\u9304\n   -j <jarfile>   \u5c07 translet \u985e\u5225\u5c01\u88dd\u5728 jar \u6a94\u6848\u4e2d\uff0c\u8a72\u6a94\u6848\n                  \u540d\u7a31\u7531 <jarfile> \u6307\u5b9a\n   -p <package>   \u6307\u5b9a\u6240\u6709\u7522\u751f\u7684\n                  translet \u985e\u5225\u4e4b\u5957\u4ef6\u540d\u7a31\u5b57\u9996\u3002\n   -n             \u555f\u7528\u7bc4\u672c\u5217\u5165\uff08\u5e73\u5747\u800c\u8a00\uff0c\u9810\u8a2d\u884c\u70ba\u8f03\u4f73\uff09\u3002\n                  \n   -x             \u958b\u555f\u984d\u5916\u7684\u9664\u932f\u8a0a\u606f\u8f38\u51fa\n   -u             \u5c07\u5f15\u6578 <stylesheet> \u89e3\u8b6f\u70ba URL\n   -i             \u5f37\u5236\u7de8\u8b6f\u5668\u5f9e stdin \u8b80\u53d6\u6a23\u5f0f\u8868\n   -v          \u5217\u5370\u7de8\u8b6f\u5668\u7684\u7248\u672c\n   -h             \u5217\u5370\u6b64\u7528\u6cd5\u9673\u8ff0\u5f0f\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "\u7528\u6cd5\u6982\u8981\n   java org.apache.xalan.xsltc.cmdline.Transform [-j <Jar \u6a94>]\n   [-x] [-n <\u758a\u4ee3>] {-u <document_url> | <\u6587\u4ef6>}\n      <\u985e\u5225> [<param1>=<value1> ...]\n\n   \u4f7f\u7528 translet <\u985e\u5225> \u8f49\u63db\u6307\u5b9a\u70ba <\u6587\u4ef6> \u7684 \n   XML \u6587\u4ef6\u3002translet <class> \u4f4d\u65bc\n   \u4f7f\u7528\u8005\u7684 CLASSPATH \u4e2d\uff0c\u6216\u9078\u64c7\u6027\u6307\u5b9a\u7684 <jarfile> \u4e2d\u3002\nOPTIONS\n   -j <Jar \u6a94>    \u6307\u5b9a\u7528\u4f86\u8f09\u5165 translet \u7684 Jar \u6a94\n   -x              \u958b\u555f\u984d\u5916\u7684\u9664\u932f\u8a0a\u606f\u8f38\u51fa\n-n <\u758a\u4ee3> \u57f7\u884c\u8f49\u63db <\u758a\u4ee3> \u6b21\u4ee5\u53ca\n                   \u986f\u793a\u8a2d\u5b9a\u6a94\u8cc7\u8a0a\n   -u <document_url> \u6307\u5b9a XML \u8f38\u5165\u6587\u4ef6\u70ba URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> \u53ea\u80fd\u7528\u5728 <xsl:for-each> \u6216 <xsl:apply-templates> \u5167\u3002"},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "\u6b64 JVM \u4e0d\u652f\u63f4\u8f38\u51fa\u7de8\u78bc ''{0}''\u3002"},
				  new object[] {ErrorMsg.SYNTAX_ERR, "''{0}'' \u4e2d\u6709\u8a9e\u6cd5\u932f\u8aa4\u3002"},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "\u627e\u4e0d\u5230\u5916\u90e8\u5efa\u69cb\u5b50 ''{0}''\u3002"},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "\u975e\u975c\u614b Java \u51fd\u6578 ''{0}'' \u7684\u7b2c\u4e00\u500b\u5f15\u6578\u4e0d\u662f\u6709\u6548\u7684\u7269\u4ef6\u53c3\u7167\u3002"},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "\u6aa2\u67e5\u8868\u793a\u5f0f ''{0}'' \u7684\u985e\u578b\u6642\u767c\u751f\u932f\u8aa4\u3002"},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "\u5728\u4e0d\u660e\u4f4d\u7f6e\u6aa2\u67e5\u8868\u793a\u5f0f\u7684\u985e\u578b\u6642\uff0c\u767c\u751f\u932f\u8aa4\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "\u6307\u4ee4\u884c\u9078\u9805 ''{0}'' \u7121\u6548\u3002"},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "\u6307\u4ee4\u884c\u9078\u9805 ''{0}'' \u907a\u6f0f\u5fc5\u8981\u7684\u5f15\u6578\u3002"},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "\u8b66\u544a\uff1a  ''{0}''\n       \uff1a{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "\u8b66\u544a\uff1a  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "\u56b4\u91cd\u932f\u8aa4\uff1a  ''{0}''\n           \uff1a{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "\u56b4\u91cd\u932f\u8aa4\uff1a  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "\u932f\u8aa4\uff1a  ''{0}''\n     \uff1a{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "\u932f\u8aa4\uff1a''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "\u4f7f\u7528 translet ''{0}'' \u9032\u884c\u8f49\u63db"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "\u5f9e Jar \u6a94 ''{1}'' \u4f7f\u7528 translet ''{0}'' \u9032\u884c\u8f49\u63db"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "\u7121\u6cd5\u5efa\u7acb TransformerFactory \u985e\u5225 ''{0}'' \u7684\u5be6\u4f8b\u3002"},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "\u7121\u6cd5\u4f7f\u7528\u540d\u7a31 ''{0}'' \u4f5c\u70ba translet \u985e\u5225\u7684\u540d\u7a31\uff0c\u56e0\u70ba\u5b83\u542b\u6709\u5728 Java \u985e\u5225\u7684\u540d\u7a31\u4e2d\u4e0d\u5141\u8a31\u4f7f\u7528\u7684\u5b57\u5143\u3002\u5df2\u6539\u70ba\u4f7f\u7528\u540d\u7a31 ''{1}''\u3002"},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "\u7de8\u8b6f\u5668\u932f\u8aa4\uff1a"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "\u7de8\u8b6f\u5668\u8b66\u544a\uff1a"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "\u7de8\u8b6f\u5668\u932f\u8aa4\uff1a"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "\u4e00\u500b\u503c\u5fc5\u9808\u662f QName \u6216\u662f\u7a7a\u683c\u5206\u9694 QNames \u6e05\u55ae\u7684\u5c6c\u6027\uff0c\u5177\u6709\u503c ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "\u4e00\u500b\u503c\u5fc5\u9808\u662f NCName \u7684\u5c6c\u6027\uff0c\u5177\u6709\u503c ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "<xsl:output> \u5143\u7d20\u7684\u65b9\u6cd5\u5c6c\u6027\u5177\u6709\u503c ''{0}''\u3002\u5176\u503c\u5fc5\u9808\u662f ''xml''\u3001''html''\u3001''text'' \u6216 qname-but-not-ncname \u5176\u4e2d\u4e4b\u3127"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "\u7279\u6027\u540d\u7a31\u5728 TransformerFactory.getFeature(String name) \u4e2d\u4e0d\u53ef\u662f\u7a7a\u503c\u3002"},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "\u7279\u6027\u540d\u7a31\u5728 TransformerFactory.setFeature(String name, boolean value) \u4e2d\u4e0d\u53ef\u662f\u7a7a\u503c\u3002"},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "\u7121\u6cd5\u5728\u9019\u500b TransformerFactory \u8a2d\u5b9a\u7279\u6027 ''{0}''\u3002"}
			  };
			}
		}

	}

}