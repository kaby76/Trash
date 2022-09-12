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
 * $Id: ErrorMessages_tr.java 468652 2006-10-28 07:05:17Z minchau $
 */

package org.apache.xalan.xsltc.runtime;

import java.util.ListResourceBundle;

/**
 * @author Morten Jorgensen
 */
public class ErrorMessages_tr extends ListResourceBundle {

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
        "''{0}'' i\u00e7inde y\u00fcr\u00fctme zaman\u0131 i\u00e7 hatas\u0131"},

        /*
         * Note to translators:  <xsl:copy> is a keyword that should not be
         * translated.
         */
        {BasisLibrary.RUN_TIME_COPY_ERR,
        "<xsl:copy> y\u00fcr\u00fct\u00fcl\u00fcrken y\u00fcr\u00fctme zaman\u0131 hatas\u0131."},

        /*
         * Note to translators:  The substitution text refers to data types.
         * The message is displayed if a value in a particular context needs to
         * be converted to type {1}, but that's not possible for a value of type
         * {0}.
         */
        {BasisLibrary.DATA_CONVERSION_ERR,
        "''{0}'' tipinden ''{1}'' tipine d\u00f6n\u00fc\u015ft\u00fcrme ge\u00e7ersiz."},

        /*
         * Note to translators:  This message is displayed if the function named
         * by the substitution text is not a function that is supported.  XSLTC
         * is the acronym naming the product.
         */
        {BasisLibrary.EXTERNAL_FUNC_ERR,
        "''{0}'' d\u0131\u015f i\u015flevi XSLTC taraf\u0131ndan desteklenmiyor."},

        /*
         * Note to translators:  This message is displayed if two values are
         * compared for equality, but the data type of one of the values is
         * unknown.
         */
        {BasisLibrary.EQUALITY_EXPR_ERR,
        "E\u015fitlik ifadesinde bilinmeyen ba\u011f\u0131ms\u0131z de\u011fi\u015fken tipi."},

        /*
         * Note to translators:  The substitution text for {0} will be a data
         * type; the substitution text for {1} will be the name of a function.
         * This is displayed if an argument of the particular data type is not
         * permitted for a call to this function.
         */
        {BasisLibrary.INVALID_ARGUMENT_ERR,
        "''{1}'' i\u015flevi \u00e7a\u011fr\u0131s\u0131nda ba\u011f\u0131ms\u0131z de\u011fi\u015fken tipi ''{0}'' ge\u00e7ersiz."},

        /*
         * Note to translators:  There is way of specifying a format for a
         * number using a pattern; the processor was unable to format the
         * particular value using the specified pattern.
         */
        {BasisLibrary.FORMAT_NUMBER_ERR,
        "''{0}'' say\u0131s\u0131n\u0131 ''{1}'' \u00f6r\u00fcnt\u00fcs\u00fcn\u00fc kullanarak bi\u00e7imleme giri\u015fimi."},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor was unable to create a copy of an
         * iterator.  (See definition of iterator above.)
         */
        {BasisLibrary.ITERATOR_CLONE_ERR,
        "''{0}'' yineleyicisinin e\u015fkopyas\u0131 yarat\u0131lam\u0131yor."},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor attempted to create an iterator
         * for a particular axis (see definition above) that it does not
         * support.
         */
        {BasisLibrary.AXIS_SUPPORT_ERR,
        "''{0}'' ekseni i\u00e7in yineleyici desteklenmiyor."},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor attempted to create an iterator
         * for a particular axis (see definition above) that it does not
         * support.
         */
        {BasisLibrary.TYPED_AXIS_SUPPORT_ERR,
        "Tip atanm\u0131\u015f ''{0}'' ekseni i\u00e7in yineleyici desteklenmiyor."},

        /*
         * Note to translators:  This message is reported if the stylesheet
         * being processed attempted to construct an XML document with an
         * attribute in a place other than on an element.  The substitution text
         * specifies the name of the attribute.
         */
        {BasisLibrary.STRAY_ATTRIBUTE_ERR,
        "''{0}'' \u00f6zniteli\u011fi \u00f6\u011fenin d\u0131\u015f\u0131nda."},

        /*
         * Note to translators:  As with the preceding message, a namespace
         * declaration has the form of an attribute and is only permitted to
         * appear on an element.  The substitution text {0} is the namespace
         * prefix and {1} is the URI that was being used in the erroneous
         * namespace declaration.
         */
        {BasisLibrary.STRAY_NAMESPACE_ERR,
        "''{0}''=''{1}'' ad alan\u0131 bildirimi \u00f6\u011fenin d\u0131\u015f\u0131nda."},

        /*
         * Note to translators:  The stylesheet contained a reference to a
         * namespace prefix that was undefined.  The value of the substitution
         * text is the name of the prefix.
         */
        {BasisLibrary.NAMESPACE_PREFIX_ERR,
        "''{0}'' \u00f6nekine ili\u015fkin ad alan\u0131 bildirilmedi."},

        /*
         * Note to translators:  The following represents an internal error.
         * DOMAdapter is a Java class in XSLTC.
         */
        {BasisLibrary.DOM_ADAPTER_INIT_ERR,
        "DOMAdapter, yanl\u0131\u015f tipte kaynak DOM kullan\u0131larak yarat\u0131ld\u0131."},

        /*
         * Note to translators:  The following message indicates that the XML
         * parser that is providing input to XSLTC cannot be used because it
         * does not describe to XSLTC the structure of the input XML document's
         * DTD.
         */
        {BasisLibrary.PARSER_DTD_SUPPORT_ERR,
        "Kulland\u0131\u011f\u0131n\u0131z SAX ayr\u0131\u015ft\u0131r\u0131c\u0131s\u0131 DTD bildirim olaylar\u0131n\u0131 i\u015flemiyor."},

        /*
         * Note to translators:  The following message indicates that the XML
         * parser that is providing input to XSLTC cannot be used because it
         * does not distinguish between ordinary XML attributes and namespace
         * declarations.
         */
        {BasisLibrary.NAMESPACES_SUPPORT_ERR,
        "Kulland\u0131\u011f\u0131n\u0131z SAX ayr\u0131\u015ft\u0131r\u0131c\u0131s\u0131n\u0131n XML ad alanlar\u0131 deste\u011fi yok."},

        /*
         * Note to translators:  The substitution text is the URI that was in
         * error.
         */
        {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR,
        "''{0}'' URI ba\u015fvurusu \u00e7\u00f6z\u00fclemedi."},

         /*
         * Note to translators:  The stylesheet contained an element that was
         * not recognized as part of the XSL syntax.  The substitution text
         * gives the element name.
         */
        {BasisLibrary.UNSUPPORTED_XSL_ERR,
        "XSL \u00f6\u011fesi ''{0}'' desteklenmiyor"},

        /*
         * Note to translators:  The stylesheet referred to an extension to the
         * XSL syntax and indicated that it was defined by XSLTC, but XSLTC does
         * not recognize the particular extension named.  The substitution text
         * gives the extension name.
         */
        {BasisLibrary.UNSUPPORTED_EXT_ERR,
        "XSLTC uzant\u0131s\u0131 ''{0}'' tan\u0131nm\u0131yor"},


        /*
         * Note to translators:  This error message is produced if the translet
         * class was compiled using a newer version of XSLTC and deployed for
         * execution with an older version of XSLTC.  The substitution text is
         * the name of the translet class.
         */
        {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR,
        "Belirtilen derleme sonucu s\u0131n\u0131f dosyas\u0131 ''{0}'', kullan\u0131lmakta olan XSLTC s\u00fcr\u00fcm\u00fcnden daha yeni bir XSLTC s\u00fcr\u00fcm\u00fcyle yarat\u0131lm\u0131\u015f.  Bi\u00e7em yapra\u011f\u0131n\u0131 yeniden derlemeli ya da bu derleme sonucu s\u0131n\u0131f dosyas\u0131n\u0131 \u00e7al\u0131\u015ft\u0131rmak i\u00e7in daha yeni bir XSLTC s\u00fcr\u00fcm\u00fcn\u00fc kullanmal\u0131s\u0131n\u0131z."},

        /*
         * Note to translators:  An attribute whose effective value is required
         * to be a "QName" had a value that was incorrect.
         * 'QName' is an XML syntactic term that must not be translated.  The
         * substitution text contains the actual value of the attribute.
         */
        {BasisLibrary.INVALID_QNAME_ERR,
        "De\u011ferinin bir QName olmas\u0131 gereken \u00f6zniteli\u011fin de\u011feri ''{0}''"},


        /*
         * Note to translators:  An attribute whose effective value is required
         * to be a "NCName" had a value that was incorrect.
         * 'NCName' is an XML syntactic term that must not be translated.  The
         * substitution text contains the actual value of the attribute.
         */
        {BasisLibrary.INVALID_NCNAME_ERR,
        "De\u011ferinin bir NCName olmas\u0131 gereken \u00f6zniteli\u011fin de\u011feri ''{0}''"},

        {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR,
        "G\u00fcvenli i\u015fleme \u00f6zelli\u011fi true de\u011ferine ayarland\u0131\u011f\u0131nda ''{0}'' eklenti i\u015flevinin kullan\u0131lmas\u0131na izin verilmez."},

        {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR,
        "G\u00fcvenli i\u015fleme \u00f6zelli\u011fi true de\u011ferine ayarland\u0131\u011f\u0131nda ''{0}'' eklenti \u00f6\u011fesinin kullan\u0131lmas\u0131na izin verilmez."},
    };
    }

}
