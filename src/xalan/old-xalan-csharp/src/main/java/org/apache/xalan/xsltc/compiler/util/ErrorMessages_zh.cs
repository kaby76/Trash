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
 * $Id: ErrorMessages_zh.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_zh : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "\u540c\u4e00\u6587\u4ef6\u4e2d\u5b9a\u4e49\u4e86\u591a\u4e2a\u6837\u5f0f\u8868\u3002"},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "\u6b64\u6837\u5f0f\u8868\u4e2d\u5df2\u7ecf\u5b9a\u4e49\u4e86\u6a21\u677f\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "\u6b64\u6837\u5f0f\u8868\u4e2d\u672a\u5b9a\u4e49\u6a21\u677f\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "\u540c\u4e00\u4f5c\u7528\u57df\u4e2d\u591a\u6b21\u5b9a\u4e49\u4e86\u53d8\u91cf\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "\u672a\u5b9a\u4e49\u53d8\u91cf\u6216\u53c2\u6570\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "\u627e\u4e0d\u5230\u7c7b\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "\u627e\u4e0d\u5230\u5916\u90e8\u65b9\u6cd5\u201c{0}\u201d\uff08\u5fc5\u987b\u662f public\uff09\u3002"},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "\u65e0\u6cd5\u5c06\u8c03\u7528\u4e2d\u7684\u53c2\u6570\uff0f\u8fd4\u56de\u7c7b\u578b\u8f6c\u6362\u4e3a\u65b9\u6cd5\u201c{0}\u201d"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "\u627e\u4e0d\u5230\u6587\u4ef6\u6216 URI\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI\u201c{0}\u201d\u65e0\u6548\u3002"},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "\u65e0\u6cd5\u6253\u5f00\u6587\u4ef6\u6216 URI\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "\u9884\u671f\u5b58\u5728 <xsl:stylesheet> \u6216 <xsl:transform> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "\u672a\u58f0\u660e\u540d\u79f0\u7a7a\u95f4\u524d\u7f00\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "\u65e0\u6cd5\u89e3\u6790\u5bf9\u51fd\u6570\u201c{0}\u201d\u7684\u8c03\u7528\u3002"},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "\u201c{0}\u201d\u7684\u81ea\u53d8\u91cf\u5fc5\u987b\u662f\u6587\u5b57\u4e32\u3002"},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "\u89e3\u6790 XPath \u8868\u8fbe\u5f0f\u201c{0}\u201d\u65f6\u51fa\u9519\u3002"},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "\u7f3a\u5c11\u5fc5\u9700\u7684\u5c5e\u6027\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "XPath \u8868\u8fbe\u5f0f\u4e2d\u7684\u5b57\u7b26\u201c{0}\u201d\u975e\u6cd5\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "\u5904\u7406\u6307\u4ee4\u7684\u540d\u79f0\u201c{0}\u201d\u975e\u6cd5\u3002"},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "\u5c5e\u6027\u201c{0}\u201d\u5728\u5143\u7d20\u5916\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "\u5c5e\u6027\u201c{0}\u201d\u975e\u6cd5\u3002"},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "\u5faa\u73af import\uff0finclude\u3002\u5df2\u88c5\u5165\u6837\u5f0f\u8868\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "\u65e0\u6cd5\u5bf9\u7ed3\u679c\u6811\u7247\u6bb5\u6392\u5e8f\uff08<xsl:sort> \u5143\u7d20\u88ab\u5ffd\u7565\uff09\u3002\u5fc5\u987b\u5728\u521b\u5efa\u7ed3\u679c\u6811\u65f6\u5bf9\u8282\u70b9\u8fdb\u884c\u6392\u5e8f\u3002"},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "\u5df2\u7ecf\u5b9a\u4e49\u4e86\u5341\u8fdb\u5236\u683c\u5f0f\u7f16\u6392\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSLTC \u4e0d\u652f\u6301 XSL V{0}\u3002"},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "\u201c{0}\u201d\u4e2d\u7684\u5faa\u73af\u53d8\u91cf\uff0f\u53c2\u6570\u5f15\u7528\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "\u4e8c\u8fdb\u5236\u8868\u8fbe\u5f0f\u7684\u8fd0\u7b97\u7b26\u672a\u77e5\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "\u51fd\u6570\u8c03\u7528\u7684\u53c2\u6570\u975e\u6cd5\u3002"},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "\u51fd\u6570 document() \u7684\u7b2c\u4e8c\u4e2a\u53c2\u6570\u5fc5\u987b\u662f\u8282\u70b9\u96c6\u3002"},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "<xsl:choose> \u4e2d\u81f3\u5c11\u8981\u6709\u4e00\u4e2a <xsl:when> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "<xsl:choose> \u4e2d\u53ea\u5141\u8bb8\u6709\u4e00\u4e2a <xsl:otherwise> \u5143\u7d20\u3002"},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> \u53ea\u80fd\u5728 <xsl:choose> \u4e2d\u4f7f\u7528\u3002"},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> \u53ea\u80fd\u5728 <xsl:choose> \u4e2d\u4f7f\u7528\u3002"},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "<xsl:choose> \u4e2d\u53ea\u5141\u8bb8\u4f7f\u7528 <xsl:when> \u548c <xsl:otherwise>\u3002"},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> \u7f3a\u5c11\u201cname\u201d\u5c5e\u6027\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "\u5b50\u5143\u7d20\u975e\u6cd5\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "\u4e0d\u80fd\u8c03\u7528\u5143\u7d20\u201c{0}\u201d"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "\u4e0d\u80fd\u8c03\u7528\u5c5e\u6027\u201c{0}\u201d"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "\u6587\u672c\u6570\u636e\u5728\u9876\u7ea7 <xsl:stylesheet> \u5143\u7d20\u5916\u3002"},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP \u89e3\u6790\u5668\u6ca1\u6709\u6b63\u786e\u914d\u7f6e"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "\u4e0d\u53ef\u6062\u590d\u7684 XSLTC \u5185\u90e8\u9519\u8bef\uff1a\u201c{0}\u201d"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "\u4e0d\u53d7\u652f\u6301\u7684 XSL \u5143\u7d20\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "\u672a\u88ab\u8bc6\u522b\u7684 XSLTC \u6269\u5c55\u540d\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "\u8f93\u5165\u6587\u6863\u4e0d\u662f\u6837\u5f0f\u8868\uff08XSL \u540d\u79f0\u7a7a\u95f4\u6ca1\u6709\u5728\u6839\u5143\u7d20\u4e2d\u58f0\u660e\uff09\u3002"},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "\u627e\u4e0d\u5230\u6837\u5f0f\u8868\u76ee\u6807\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "\u6ca1\u6709\u5b9e\u73b0\uff1a\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "\u8f93\u5165\u6587\u6863\u4e0d\u5305\u542b XSL \u6837\u5f0f\u8868\u3002"},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "\u65e0\u6cd5\u89e3\u6790\u5143\u7d20\u201c{0}\u201d"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "<key> \u7684 use \u5c5e\u6027\u5fc5\u987b\u662f node\u3001node-set\u3001string \u6216 number\u3002"},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "\u8f93\u51fa XML \u6587\u6863\u7684\u7248\u672c\u5e94\u5f53\u662f 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "\u5173\u7cfb\u8868\u8fbe\u5f0f\u7684\u8fd0\u7b97\u7b26\u672a\u77e5"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "\u8bd5\u56fe\u4f7f\u7528\u4e0d\u5b58\u5728\u7684\u5c5e\u6027\u96c6\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "\u65e0\u6cd5\u89e3\u6790\u5c5e\u6027\u503c\u6a21\u677f\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "\u7c7b\u201c{0}\u201d\u7684\u7279\u5f81\u7b26\u4e2d\u7684\u6570\u636e\u7c7b\u578b\u672a\u77e5\u3002"},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "\u65e0\u6cd5\u5c06\u6570\u636e\u7c7b\u578b\u201c{0}\u201d\u8f6c\u6362\u6210\u201c{1}\u201d\u3002"},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "\u6b64 Templates \u4e0d\u5305\u542b\u6709\u6548\u7684 translet \u7c7b\u5b9a\u4e49\u3002"},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "\u6b64 Templates \u4e0d\u5305\u542b\u540d\u4e3a\u201c{0}\u201d\u7684\u7c7b\u3002"},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "\u65e0\u6cd5\u88c5\u5165 translet \u7c7b\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Translet \u7c7b\u5df2\u88c5\u5165\uff0c\u4f46\u65e0\u6cd5\u521b\u5efa translet \u5b9e\u4f8b\u3002"},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "\u8bd5\u56fe\u5c06\u201c{0}\u201d\u7684 ErrorListener \u8bbe\u7f6e\u4e3a\u7a7a"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC \u53ea\u652f\u6301 StreamSource\u3001SAXSource \u548c DOMSource"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "\u4f20\u9012\u7ed9\u201c{0}\u201d\u7684\u6e90\u5bf9\u8c61\u6ca1\u6709\u5185\u5bb9\u3002"},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "\u65e0\u6cd5\u7f16\u8bd1\u6837\u5f0f\u8868"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory \u65e0\u6cd5\u8bc6\u522b\u5c5e\u6027\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() \u5fc5\u987b\u5728 startDocument() \u4e4b\u524d\u8c03\u7528\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer \u6ca1\u6709\u5c01\u88c5\u7684 translet \u5bf9\u8c61\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "\u6ca1\u6709\u4e3a\u8f6c\u6362\u7ed3\u679c\u5b9a\u4e49\u8f93\u51fa\u5904\u7406\u5668\u3002"},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "\u4f20\u9012\u7ed9\u201c{0}\u201d\u7684 Result \u5bf9\u8c61\u65e0\u6548\u3002"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "\u8bd5\u56fe\u8bbf\u95ee\u65e0\u6548\u7684 Transformer \u5c5e\u6027\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "\u65e0\u6cd5\u521b\u5efa SAX2DOM \u9002\u914d\u5668\uff1a\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "\u6ca1\u6709\u8bbe\u7f6e systemId \u5c31\u8c03\u7528\u4e86 XSLTCSource.build()\u3002"},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "\u7ed3\u679c\u4e0d\u5e94\u4e3a\u7a7a"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "\u53c2\u6570 {0} \u7684\u503c\u5fc5\u987b\u4e3a\u6709\u6548\u7684 Java \u5bf9\u8c61"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "-i \u9009\u9879\u5fc5\u987b\u4e0e -o \u9009\u9879\u4e00\u8d77\u4f7f\u7528\u3002"},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <output>]\n      [-d <directory>] [-j <jarfile>] [-p <package>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\nOPTIONS\n   -o <output>    \u5c06\u540d\u79f0 <output> \u6307\u5b9a\u7ed9\u751f\u6210\u7684\n                  translet\u3002\u7f3a\u7701\u60c5\u51b5\u4e0b\uff0ctranslet \u540d\u79f0\n                  \u6d3e\u751f\u81ea <stylesheet> \u540d\u79f0\u3002\n                  \u5982\u679c\u7f16\u8bd1\u591a\u4e2a\u6837\u5f0f\u8868\uff0c\u5219\u5ffd\u7565\u6b64\u9009\u9879\u3002\n-d <directory> \u6307\u5b9a translet \u7684\u76ee\u6807\u76ee\u5f55\n   -j <jarfile>   \u5c06 translet \u7c7b\u6253\u5305\u6210\u547d\u540d\u4e3a <jarfile>\n                  \u7684 jar \u6587\u4ef6\n   -p <package>   \u4e3a\u6240\u6709\u751f\u6210\u7684 translet \u7c7b\n                  \u6307\u5b9a\u8f6f\u4ef6\u5305\u540d\u79f0\u7684\u524d\u7f00\u3002\n   -n             \u542f\u7528\u6a21\u677f\u5728\u7ebf\uff08\u5e73\u5747\u7f3a\u7701\n                  \u884c\u4e3a\u66f4\u4f73\uff09\u3002\n   -x             \u6253\u5f00\u989d\u5916\u8c03\u8bd5\u6d88\u606f\u8f93\u51fa\n   -u             \u5c06 <stylesheet> \u81ea\u53d8\u91cf\u89e3\u91ca\u4e3a URL\n   -i             \u5f3a\u5236\u7f16\u8bd1\u5668\u4ece stdin \u8bfb\u5165\u6837\u5f0f\u8868\n   -v             \u6253\u5370\u7f16\u8bd1\u5668\u7684\u7248\u672c\n   -h             \u6253\u5370\u6b64\u7528\u6cd5\u8bed\u53e5\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <jarfile>]\n      [-x] [-n <iterations>] {-u <document_url> | <document>}\n      <class> [<param1>=<value1> ...]\n\n   \u4f7f\u7528 translet <class> \u6765\u8f6c\u6362\u6307\u5b9a\u4e3a <document>\n   \u7684 XML \u6587\u6863\u3002translet <class> \u8981\u4e48\u5728\n   \u7528\u6237\u7684 CLASSPATH \u4e2d\uff0c\u8981\u4e48\u5728\u4efb\u610f\u6307\u5b9a\u7684 <jarfile> \u4e2d\u3002\nOPTIONS\n   -j <jarfile>    \u6307\u5b9a\u88c5\u5165 translet \u7684 jarfile\n   -x              \u6253\u5f00\u989d\u5916\u8c03\u8bd5\u6d88\u606f\u8f93\u51fa\n   -n <iterations> \u8fd0\u884c\u8f6c\u6362\u8fc7\u7a0b <iterations> \u6570\u6b21\u5e76\n                   \u663e\u793a\u6982\u8981\u5206\u6790\u4fe1\u606f\n   -u <document_url> \u5c06 XML \u8f93\u5165\u6307\u5b9a\u4e3a URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> \u53ea\u80fd\u5728 <xsl:for-each> \u6216 <xsl:apply-templates> \u4e2d\u4f7f\u7528\u3002"},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "\u6b64 JVM \u4e2d\u4e0d\u652f\u6301\u8f93\u51fa\u7f16\u7801\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.SYNTAX_ERR, "\u201c{0}\u201d\u4e2d\u51fa\u73b0\u8bed\u6cd5\u9519\u8bef\u3002"},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "\u627e\u4e0d\u5230\u5916\u90e8\u6784\u9020\u51fd\u6570\u201c{0}\u201d\u3002"},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "\u975e static Java \u51fd\u6570\u201c{0}\u201d\u7684\u7b2c\u4e00\u4e2a\u53c2\u6570\u4e0d\u662f\u6709\u6548\u7684\u5bf9\u8c61\u5f15\u7528\u3002"},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "\u68c0\u67e5\u8868\u8fbe\u5f0f\u201c{0}\u201d\u7684\u7c7b\u578b\u65f6\u51fa\u9519\u3002"},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "\u68c0\u67e5\u672a\u77e5\u4f4d\u7f6e\u7684\u8868\u8fbe\u5f0f\u7c7b\u578b\u65f6\u51fa\u9519\u3002"},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "\u547d\u4ee4\u884c\u9009\u9879\u201c{0}\u201d\u65e0\u6548\u3002"},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "\u547d\u4ee4\u884c\u9009\u9879\u201c{0}\u201d\u7f3a\u5c11\u5fc5\u9700\u7684\u81ea\u53d8\u91cf\u3002"},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "\u8b66\u544a\uff1a\u201c{0}\u201d\n    \uff1a{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "\u8b66\u544a\uff1a\u201c{0}\u201d"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "\u81f4\u547d\u9519\u8bef\uff1a\u201c{0}\u201d\n        \uff1a{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "\u81f4\u547d\u9519\u8bef\uff1a\u201c{0}\u201d"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "\u9519\u8bef\uff1a{0}\u201c\n    \uff1a{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "\u9519\u8bef\uff1a\u201c{0}\u201d"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "\u4f7f\u7528 translet\u201c{0}\u201d\u8f6c\u6362"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "\u4f7f\u7528 translet\u201c{0}\u201d\u4ece jar \u6587\u4ef6\u201c{1}\u201d\u8f6c\u6362"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "\u65e0\u6cd5\u521b\u5efa TransformerFactory \u7c7b\u201c{0}\u201d\u7684\u5b9e\u4f8b\u3002"},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "\u7531\u4e8e\u540d\u79f0\u201c{0}\u201d\u5305\u542b Java \u7c7b\u540d\u79f0\u4e2d\u4e0d\u5141\u8bb8\u4f7f\u7528\u7684\u5b57\u7b26\uff0c\u56e0\u6b64\u4e0d\u80fd\u5c06\u5b83\u7528\u4f5c translet \u7c7b\u7684\u540d\u79f0\u3002\u4f7f\u7528\u540d\u79f0\u201c{1}\u201d\u6765\u4ee3\u66ff\u3002"},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "\u7f16\u8bd1\u5668\u9519\u8bef\uff1a"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "\u7f16\u8bd1\u5668\u8b66\u544a\uff1a"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Translet \u9519\u8bef\uff1a"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "\u503c\u5fc5\u987b\u4e3a QName \u6216\u4e3a\u7528\u7a7a\u683c\u5206\u5f00\u7684 QName \u5217\u8868\u7684\u5c5e\u6027\u5177\u6709\u503c\u201c{0}\u201d"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "\u503c\u5fc5\u987b\u4e3a NCName \u7684\u5c5e\u6027\u5177\u6709\u503c\u201c{0}\u201d"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "<xsl:output> \u5143\u7d20\u7684 method \u5c5e\u6027\u5177\u6709\u503c\u201c{0}\u201d\u3002\u6b64\u503c\u5fc5\u987b\u4e3a\u201cxml\u201d\u3001\u201chtml\u201d\u3001\u201ctext\u201d \u6216 qname-but-not-ncname \u4e2d\u7684\u4e00\u4e2a"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "\u5728 TransformerFactory.getFeature(String name) \u4e2d\u7279\u5f81\u540d\u4e0d\u80fd\u4e3a\u7a7a\u3002"},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "\u5728 TransformerFactory.setFeature(String name, boolean value) \u4e2d\u7279\u5f81\u540d\u4e0d\u80fd\u4e3a\u7a7a\u3002"},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "\u65e0\u6cd5\u5bf9\u6b64 TransformerFactory \u8bbe\u7f6e\u7279\u5f81\u201c{0}\u201d\u3002"}
			  };
			}
		}

	}

}