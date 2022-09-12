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
 * $Id: ErrorMessages_zh_TW.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_zh_TW : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "''{0}'' \u767c\u751f\u57f7\u884c\u6642\u671f\u5167\u90e8\u932f\u8aa4"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "\u57f7\u884c <xsl:copy> \u6642\uff0c\u767c\u751f\u57f7\u884c\u6642\u671f\u932f\u8aa4\u3002"},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "\u5f9e ''{0}'' \u6210\u70ba ''{1}'' \u7684\u8f49\u63db\u7121\u6548\u3002"},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "XSLTC \u4e0d\u652f\u63f4\u5916\u90e8\u51fd\u6578 ''{0}''\u3002"},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "\u76f8\u7b49\u8868\u793a\u5f0f\u4e2d\u5305\u542b\u4e0d\u660e\u7684\u5f15\u6578\u985e\u578b\u3002"},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "\u547c\u53eb ''{1}'' \u6240\u4f7f\u7528\u7684\u5f15\u6578\u985e\u578b ''{0}'' \u7121\u6548\u3002"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "\u5617\u8a66\u4f7f\u7528\u578b\u6a23 ''{1}'' \u683c\u5f0f\u5316\u6578\u5b57 ''{0}''\u3002"},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "\u7121\u6cd5\u8907\u88fd\u91cd\u8907\u9805\u76ee ''{0}''\u3002"},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "\u4e0d\u652f\u63f4\u8ef8 ''{0}'' \u7684\u91cd\u8907\u9805\u76ee\u3002"},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "\u4e0d\u652f\u63f4\u6240\u9375\u5165\u8ef8 ''{0}'' \u7684\u91cd\u8907\u9805\u76ee\u3002"},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "\u5c6c\u6027 ''{0}'' \u8d85\u51fa\u5143\u7d20\u5916\u3002"},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "\u540d\u7a31\u7a7a\u9593\u5ba3\u544a ''{0}''=''{1}'' \u8d85\u51fa\u5143\u7d20\u5916\u3002"},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "\u5b57\u9996 ''{0}'' \u7684\u540d\u7a31\u7a7a\u9593\u5c1a\u672a\u5ba3\u544a\u3002"},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "\u5efa\u7acb DOMAdapter \u6642\u4f7f\u7528\u7684\u539f\u59cb\u6a94 DOM \u985e\u578b\u932f\u8aa4\u3002"},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "\u60a8\u4f7f\u7528\u7684 SAX \u5256\u6790\u5668\u7121\u6cd5\u8655\u7406 DTD \u5ba3\u544a\u4e8b\u4ef6\u3002"},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "\u60a8\u4f7f\u7528\u7684 SAX \u5256\u6790\u5668\u4e0d\u652f\u63f4 XML \u540d\u7a31\u7a7a\u9593\u3002"},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "\u7121\u6cd5\u89e3\u6790 URI \u53c3\u7167 ''{0}''\u3002"},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "XSL \u5143\u7d20 ''{0}'' \u4e0d\u53d7\u652f\u63f4"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "XSLTC \u5ef6\u4f38\u9805\u76ee ''{0}'' \u7121\u6cd5\u8fa8\u8b58"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "\u6307\u5b9a\u7684 translet ''{0}'' \u662f\u4ee5\u6bd4\u4f7f\u7528\u4e2d XSLTC \u57f7\u884c\u6642\u671f\u7248\u672c\u66f4\u65b0\u7684 XSLTC \u7248\u672c\u6240\u5efa\u7acb\u3002\u60a8\u5fc5\u9808\u91cd\u65b0\u7de8\u8b6f\u6a23\u5f0f\u8868\u6216\u4f7f\u7528\u66f4\u65b0\u7684 XSLTC \u7248\u672c\u4f86\u57f7\u884c\u6b64 translet\u3002"},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "\u4e00\u500b\u503c\u5fc5\u9808\u662f QName \u7684\u5c6c\u6027\uff0c\u5177\u6709\u503c ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "\u4e00\u500b\u503c\u5fc5\u9808\u662f NCName \u7684\u5c6c\u6027\uff0c\u5177\u6709\u503c ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "\u7576\u5b89\u5168\u8655\u7406\u7279\u6027\u8a2d\u70ba true \u6642\uff0c\u4e0d\u63a5\u53d7\u4f7f\u7528\u5ef6\u4f38\u51fd\u6578 ''{0}''\u3002"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "\u7576\u5b89\u5168\u8655\u7406\u7279\u6027\u8a2d\u70ba true \u6642\uff0c\u4e0d\u63a5\u53d7\u4f7f\u7528\u5ef6\u4f38\u5143\u7d20 ''{0}''\u3002"}
			  };
			}
		}

	}

}