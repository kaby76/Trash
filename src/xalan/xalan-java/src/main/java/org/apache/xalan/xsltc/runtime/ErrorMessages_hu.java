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
 * $Id: ErrorMessages_hu.java 468652 2006-10-28 07:05:17Z minchau $
 */

package org.apache.xalan.xsltc.runtime;

import java.util.ListResourceBundle;

/**
 * @author Morten Jorgensen
 */
public class ErrorMessages_hu extends ListResourceBundle {

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
        "Fut\u00e1s k\u00f6zbeni bels\u0151 hiba a(z) ''{0}'' oszt\u00e1lyban. "},

        /*
         * Note to translators:  <xsl:copy> is a keyword that should not be
         * translated.
         */
        {BasisLibrary.RUN_TIME_COPY_ERR,
        "Fut\u00e1s k\u00f6zbeni bels\u0151 hiba az <xsl:copy> v\u00e9grehajt\u00e1sakor."},

        /*
         * Note to translators:  The substitution text refers to data types.
         * The message is displayed if a value in a particular context needs to
         * be converted to type {1}, but that's not possible for a value of type
         * {0}.
         */
        {BasisLibrary.DATA_CONVERSION_ERR,
        "\u00c9rv\u00e9nytelen \u00e1talak\u00edt\u00e1s ''{0}'' t\u00edpusr\u00f3l ''{1}'' t\u00edpusra."},

        /*
         * Note to translators:  This message is displayed if the function named
         * by the substitution text is not a function that is supported.  XSLTC
         * is the acronym naming the product.
         */
        {BasisLibrary.EXTERNAL_FUNC_ERR,
        "A(z) ''{0}'' k\u00fcls\u0151 f\u00fcggv\u00e9nyt az XSLTC nem t\u00e1mogatja."},

        /*
         * Note to translators:  This message is displayed if two values are
         * compared for equality, but the data type of one of the values is
         * unknown.
         */
        {BasisLibrary.EQUALITY_EXPR_ERR,
        "Ismeretlen argumentumt\u00edpus tal\u00e1lhat\u00f3 az egyenl\u0151s\u00e9gi kifejez\u00e9sben."},

        /*
         * Note to translators:  The substitution text for {0} will be a data
         * type; the substitution text for {1} will be the name of a function.
         * This is displayed if an argument of the particular data type is not
         * permitted for a call to this function.
         */
        {BasisLibrary.INVALID_ARGUMENT_ERR,
        "A(z) ''{0}'' \u00e9rv\u00e9nytelen argumentumt\u00edpus a(z) ''{1}'' h\u00edv\u00e1s\u00e1hoz "},

        /*
         * Note to translators:  There is way of specifying a format for a
         * number using a pattern; the processor was unable to format the
         * particular value using the specified pattern.
         */
        {BasisLibrary.FORMAT_NUMBER_ERR,
        "K\u00eds\u00e9rlet a(z) ''{0}'' form\u00e1z\u00e1s\u00e1ra a(z) ''{1}'' mint\u00e1val."},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor was unable to create a copy of an
         * iterator.  (See definition of iterator above.)
         */
        {BasisLibrary.ITERATOR_CLONE_ERR,
        "A(z) ''{0}'' iter\u00e1tor nem kl\u00f3nozhat\u00f3."},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor attempted to create an iterator
         * for a particular axis (see definition above) that it does not
         * support.
         */
        {BasisLibrary.AXIS_SUPPORT_ERR,
        "A(z) ''{0}'' tengelyre az iter\u00e1tor nem t\u00e1mogatott."},

        /*
         * Note to translators:  The following represents an internal error
         * situation in XSLTC.  The processor attempted to create an iterator
         * for a particular axis (see definition above) that it does not
         * support.
         */
        {BasisLibrary.TYPED_AXIS_SUPPORT_ERR,
        "A tipiz\u00e1lt ''{0}'' tengelyre az iter\u00e1tor nem t\u00e1mogatott."},

        /*
         * Note to translators:  This message is reported if the stylesheet
         * being processed attempted to construct an XML document with an
         * attribute in a place other than on an element.  The substitution text
         * specifies the name of the attribute.
         */
        {BasisLibrary.STRAY_ATTRIBUTE_ERR,
        "A(z) ''{0}'' attrib\u00fatum k\u00edv\u00fcl esik az elemen."},

        /*
         * Note to translators:  As with the preceding message, a namespace
         * declaration has the form of an attribute and is only permitted to
         * appear on an element.  The substitution text {0} is the namespace
         * prefix and {1} is the URI that was being used in the erroneous
         * namespace declaration.
         */
        {BasisLibrary.STRAY_NAMESPACE_ERR,
        "A(z) ''{0}''=''{1}'' n\u00e9vt\u00e9rdeklar\u00e1ci\u00f3 k\u00edv\u00fcl esik az elemen."},

        /*
         * Note to translators:  The stylesheet contained a reference to a
         * namespace prefix that was undefined.  The value of the substitution
         * text is the name of the prefix.
         */
        {BasisLibrary.NAMESPACE_PREFIX_ERR,
        "A(z) ''{0}'' el\u0151tag n\u00e9vtere nincs deklar\u00e1lva."},

        /*
         * Note to translators:  The following represents an internal error.
         * DOMAdapter is a Java class in XSLTC.
         */
        {BasisLibrary.DOM_ADAPTER_INIT_ERR,
        "Nem megfelel\u0151 t\u00edpus\u00fa forr\u00e1s DOM haszn\u00e1lat\u00e1val j\u00f6tt l\u00e9tre a DOMAdapter."},

        /*
         * Note to translators:  The following message indicates that the XML
         * parser that is providing input to XSLTC cannot be used because it
         * does not describe to XSLTC the structure of the input XML document's
         * DTD.
         */
        {BasisLibrary.PARSER_DTD_SUPPORT_ERR,
        "A haszn\u00e1lt SAX \u00e9rtelmez\u0151 nem kezeli a DTD deklar\u00e1ci\u00f3s esem\u00e9nyeket."},

        /*
         * Note to translators:  The following message indicates that the XML
         * parser that is providing input to XSLTC cannot be used because it
         * does not distinguish between ordinary XML attributes and namespace
         * declarations.
         */
        {BasisLibrary.NAMESPACES_SUPPORT_ERR,
        "A haszn\u00e1lt SAX \u00e9rtelmez\u0151 nem t\u00e1mogatja az XML n\u00e9vtereket."},

        /*
         * Note to translators:  The substitution text is the URI that was in
         * error.
         */
        {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR,
        "Nem lehet feloldani a(z) ''{0}'' URI hivatkoz\u00e1st."},

         /*
         * Note to translators:  The stylesheet contained an element that was
         * not recognized as part of the XSL syntax.  The substitution text
         * gives the element name.
         */
        {BasisLibrary.UNSUPPORTED_XSL_ERR,
        "Nem t\u00e1mogatott XSL elem: ''{0}''"},

        /*
         * Note to translators:  The stylesheet referred to an extension to the
         * XSL syntax and indicated that it was defined by XSLTC, but XSLTC does
         * not recognize the particular extension named.  The substitution text
         * gives the extension name.
         */
        {BasisLibrary.UNSUPPORTED_EXT_ERR,
        "Ismeretlen XSLTC kiterjeszt\u00e9s: ''{0}''"},


        /*
         * Note to translators:  This error message is produced if the translet
         * class was compiled using a newer version of XSLTC and deployed for
         * execution with an older version of XSLTC.  The substitution text is
         * the name of the translet class.
         */
        {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR,
        "A megadott ''{0}'' translet az XSLTC egy \u00fajabb verzi\u00f3j\u00e1val k\u00e9sz\u00fclt, mint a haszn\u00e1latban l\u00e9v\u0151 XSLTC verzi\u00f3. \u00dajra kell ford\u00edtania a st\u00edluslapot, vagy a translet futtat\u00e1s\u00e1hoz az XSLTC \u00fajabb verzi\u00f3j\u00e1t kell haszn\u00e1lnia."},

        /*
         * Note to translators:  An attribute whose effective value is required
         * to be a "QName" had a value that was incorrect.
         * 'QName' is an XML syntactic term that must not be translated.  The
         * substitution text contains the actual value of the attribute.
         */
        {BasisLibrary.INVALID_QNAME_ERR,
        "Egy olyan attrib\u00fatum, aminek az \u00e9rt\u00e9ke csak QName lehet, ''{0}'' \u00e9rt\u00e9kkel rendelkezett."},


        /*
         * Note to translators:  An attribute whose effective value is required
         * to be a "NCName" had a value that was incorrect.
         * 'NCName' is an XML syntactic term that must not be translated.  The
         * substitution text contains the actual value of the attribute.
         */
        {BasisLibrary.INVALID_NCNAME_ERR,
        "Egy olyan attrib\u00fatum, amelynek \u00e9rt\u00e9ke csak NCName lehet, ''{0}'' \u00e9rt\u00e9kkel rendelkezett."},

        {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR,
        "A(z) ''{0}'' kiterjeszt\u00e9si f\u00fcggv\u00e9ny haszn\u00e1lata nem megengedett, ha biztons\u00e1gos feldolgoz\u00e1s be van kapcsolva. "},

        {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR,
        "A(z) ''{0}'' kiterjeszt\u00e9si elem haszn\u00e1lata nem megengedett, ha biztons\u00e1gos feldolgoz\u00e1s be van kapcsolva. "},
    };
    }

}
