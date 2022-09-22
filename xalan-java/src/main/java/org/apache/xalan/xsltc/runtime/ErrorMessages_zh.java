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

package org.apache.xalan.xsltc.runtime;

import java.util.ListResourceBundle;

/**
 * @author Morten Jorgensen
 */
public class ErrorMessages_zh extends ListResourceBundle {

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
    /** Get the lookup table for error messages.
     *
     * @return The message lookup table.
     */
    public Object[][] getContents()
    {
      return new Object[][] {

        /*
         * Note to translators:  the substitution text in the following message
         * is a class name.  Used for internal errors in the processor.
         */
        {BasisLibrary.RUN_TIME_INTERNAL_ERR,
        "\u201c{0}\u201d\u4e2d\u51fa\u73b0\u8fd0\u884c\u65f6\u5185\u90e8\u9519\u8bef"},

        /*
         * Note to translators:  <xsl:copy> is a keyword that should not be
         * translated.
         */
        {BasisLibrary.RUN_TIME_COPY_ERR,
        "\u5728\u6267\u884c <xsl:copy> \u65f6\u53d1\u751f\u8fd0\u884c\u65f6\u9519\u8bef\u3002"},

        /*
         * Note to translators:  The substitution text refers to data types.
         * The message is displayed if a value in a particular context needs to
         * be converted to type {1}, but that's not possible for a value of type
         * {0}.
         */
        {BasisLibrary.DATA_CONVERSION_ERR,
        "\u4ece\u201c{0}\u201d\u5230\u201c{1}\u201d\u7684\u8f6c\u6362\u65e0\u6548\u3002"},

        /*
         * Note to translators:  This message is displayed if the function named
         * by the substitution text is not a function that is supported.  XSLTC
         * is the acronym naming the product.
         */
        {BasisLibrary.EXTERNAL_FUNC_ERR,
        "XSLTC \u4e0d\u652f\u6301\u5916\u90e8\u51fd\u6570\u201c{0}\u201d\u3002"},

        /*
         * Note to translators:  This message is displayed if two values are
         * compared for equality, but the data type of one of the values is
         * unknown.
         */
        {BasisLibrary.EQUALITY_EXPR_ERR,
        "\u7b49\u5f0f\u8868\u8fbe\u5f0f\u4e2d\u7684\u81ea\u53d8\u91cf\u7c7b\u578b\u672a\u77e5\u3002"},

        /*
         * Note to translators:  The substitution text for {0} will be a data
         * type; the substitution text for {1} will be the name of a function.
         * This is displayed if an argument of the particular data type is not
         * permitted for a call to this function.
         */
        {BasisLibrary.INVALID_ARGUMENT_ERR,
        "\u8c03\u7528\u201c{1}\u201d\u65f6\u4f7f\u7528\u7684\u53c2\u6570\u7c7b\u578b\u201c{0}\u201d\u65e0\u6548"},

        /*
         * Note to translators:  There is way of specifying a format for a
         * number using a pattern; the processor was unable to format the
         * particular value using the specified pattern.
         */
        {BasisLibrary.FORMAT_NUMBER_ERR,
        "\u8bd5\u56fe\u4f7f\u7528\u6a21\u5f0f\u201c{1}\u201d\u4e3a\u6570\u5b57\u201c{0}\u201d\u7f16\u6392\u683c\u5f0f\u3002"},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor was unable to create a copy of an
         * iterator.  (See definition of iterator above.)
         */
        {BasisLibrary.ITERATOR_CLONE_ERR,
        "\u65e0\u6cd5\u514b\u9686\u8fed\u4ee3\u5668\u201c{0}\u201d\u3002"},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor attempted to create an iterator
         * for a particular axis (see definition above) that it does not
         * support.
         */
        {BasisLibrary.AXIS_SUPPORT_ERR,
        "\u4e0d\u652f\u6301\u8f74\u201c{0}\u201d\u7684\u8fed\u4ee3\u5668\u3002"},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor attempted to create an iterator
         * for a particular axis (see definition above) that it does not
         * support.
         */
        {BasisLibrary.TYPED_AXIS_SUPPORT_ERR,
        "\u4e0d\u652f\u6301\u8f93\u5165\u7684\u8f74\u201c{0}\u201d\u7684\u8fed\u4ee3\u5668\u3002"},

        /*
         * Note to translators:  This message is reported if the stylesheet
         * being processed attempted to construct an XML document with an
         * attribute in a place other than on an element.  The substitution text
         * specifies the name of the attribute.
         */
        {BasisLibrary.STRAY_ATTRIBUTE_ERR,
        "\u5c5e\u6027\u201c{0}\u201d\u5728\u5143\u7d20\u5916\u3002"},

        /*
         * Note to translators:  As with the preceding message, a namespace
         * declaration has the form of an attribute and is only permitted to
         * appear on an element.  The substitution text {0} is the namespace
         * prefix and {1} is the URI that was being used in the erroneous
         * namespace declaration.
         */
        {BasisLibrary.STRAY_NAMESPACE_ERR,
        "\u540d\u79f0\u7a7a\u95f4\u58f0\u660e\u201c{0}\u201d=\u201c{1}\u201d\u5728\u5143\u7d20\u5916\u3002"},

        /*
         * Note to translators:  The stylesheet contained a reference to a
         * namespace prefix that was undefined.  The value of the substitution
         * text is the name of the prefix.
         */
        {BasisLibrary.NAMESPACE_PREFIX_ERR,
        "\u5c1a\u672a\u58f0\u660e\u524d\u7f00\u201c{0}\u201d\u7684\u540d\u79f0\u7a7a\u95f4\u3002"},

        /*
         * Note to translators:  The following represents an internal error.
         * DOMAdapter is a Java class in XSLTC.
         */
        {BasisLibrary.DOM_ADAPTER_INIT_ERR,
        "\u4f7f\u7528\u9519\u8bef\u7c7b\u578b\u7684\u6e90 DOM \u521b\u5efa\u4e86 DOMAdapter\u3002"},

        /*
         * Note to translators:  The following message indicates that the XML
         * parser that is providing input to XSLTC cannot be used because it
         * does not describe to XSLTC the structure of the input XML document's
         * DTD.
         */
        {BasisLibrary.PARSER_DTD_SUPPORT_ERR,
        "\u6b63\u5728\u4f7f\u7528\u7684 SAX \u89e3\u6790\u5668\u4e0d\u5904\u7406 DTD \u58f0\u660e\u4e8b\u4ef6\u3002"},

        /*
         * Note to translators:  The following message indicates that the XML
         * parser that is providing input to XSLTC cannot be used because it
         * does not distinguish between ordinary XML attributes and namespace
         * declarations.
         */
        {BasisLibrary.NAMESPACES_SUPPORT_ERR,
        "\u6b63\u5728\u4f7f\u7528\u7684 SAX \u89e3\u6790\u5668\u4e0d\u652f\u6301 XML \u540d\u79f0\u7a7a\u95f4\u3002"},

        /*
         * Note to translators:  The substitution text is the URI that was in
         * error.
         */
        {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR,
        "\u65e0\u6cd5\u89e3\u6790 URI \u5f15\u7528\u201c{0}\u201d\u3002"},

         /*
         * Note to translators:  The stylesheet contained an element that was
         * not recognized as part of the XSL syntax.  The substitution text
         * gives the element name.
         */
        {BasisLibrary.UNSUPPORTED_XSL_ERR,
        "\u4e0d\u53d7\u652f\u6301\u7684 XSL \u5143\u7d20\u201c{0}\u201d"},

        /*
         * Note to translators:  The stylesheet referred to an extension to the
         * XSL syntax and indicated that it was defined by XSLTC, but XSLTC does
         * not recognize the particular extension named.  The substitution text
         * gives the extension name.
         */
        {BasisLibrary.UNSUPPORTED_EXT_ERR,
        "\u672a\u88ab\u8bc6\u522b\u7684 XSLTC \u6269\u5c55\u540d\u201c{0}\u201d"},


        /*
         * Note to translators:  This error message is produced if the translet
         * class was compiled using a newer version of XSLTC and deployed for
         * execution with an older version of XSLTC.  The substitution text is
         * the name of the translet class.
         */
        {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR,
        "\u521b\u5efa\u6307\u5b9a\u7684 translet\u201c{0}\u201d\u65f6\uff0c\u4f7f\u7528\u7684 XSLTC \u7248\u672c\u6bd4\u6b63\u5728\u4f7f\u7528\u7684 XSLTC \u8fd0\u884c\u65f6\u7248\u672c\u66f4\u65b0\u3002\u60a8\u5fc5\u987b\u91cd\u65b0\u7f16\u8bd1\u6837\u5f0f\u8868\u6216\u4f7f\u7528\u66f4\u65b0\u7684 XSLTC \u7248\u672c\u6765\u8fd0\u884c\u6b64 translet\u3002"},

        /*
         * Note to translators:  An attribute whose effective value is required
         * to be a "QName" had a value that was incorrect.
         * 'QName' is an XML syntactic term that must not be translated.  The
         * substitution text contains the actual value of the attribute.
         */
        {BasisLibrary.INVALID_QNAME_ERR,
        "\u503c\u5fc5\u987b\u4e3a QName \u7684\u5c5e\u6027\u5177\u6709\u503c\u201c{0}\u201d"},


        /*
         * Note to translators:  An attribute whose effective value is required
         * to be a "NCName" had a value that was incorrect.
         * 'NCName' is an XML syntactic term that must not be translated.  The
         * substitution text contains the actual value of the attribute.
         */
        {BasisLibrary.INVALID_NCNAME_ERR,
        "\u503c\u5fc5\u987b\u4e3a NCName \u7684\u5c5e\u6027\u5177\u6709\u503c\u201c{0}\u201d"},

        {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR,
        "\u5f53\u5b89\u5168\u5904\u7406\u529f\u80fd\u8bbe\u7f6e\u4e3a true \u65f6\uff0c\u4e0d\u5141\u8bb8\u4f7f\u7528\u6269\u5c55\u51fd\u6570\u201c{0}\u201d\u3002"},

        {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR,
        "\u5f53\u5b89\u5168\u5904\u7406\u529f\u80fd\u8bbe\u7f6e\u4e3a true \u65f6\uff0c\u4e0d\u5141\u8bb8\u4f7f\u7528\u6269\u5c55\u5143\u7d20\u201c{0}\u201d\u3002"},
    };
    }

}
