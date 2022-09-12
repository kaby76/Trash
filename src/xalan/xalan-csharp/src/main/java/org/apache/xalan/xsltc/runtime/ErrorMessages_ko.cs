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
 * $Id: ErrorMessages_ko.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ko : ListResourceBundle
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "''{0}''\uc5d0 \ub7f0\ud0c0\uc784 \ub0b4\ubd80 \uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "<xsl:copy> \uc2e4\ud589\uc2dc \ub7f0\ud0c0\uc784 \uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "''{0}''\uc5d0\uc11c ''{1}''(\uc73c)\ub85c\uc758 \ubcc0\ud658\uc740 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "XSLTC\uc5d0\uc11c ''{0}'' \uc678\ubd80 \ud568\uc218\ub97c \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "\ub4f1\uc2dd\uc5d0 \uc54c \uc218 \uc5c6\ub294 \uc778\uc218 \uc720\ud615\uc774 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "''{1}''\uc5d0 \ub300\ud55c \ud638\ucd9c\uc5d0\uc11c ''{0}'' \uc778\uc218 \uc720\ud615\uc774 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "''{1}'' \ud328\ud134\uc744 \uc0ac\uc6a9\ud558\uc5ec \uc22b\uc790 ''{0}''\uc744(\ub97c) \ud3ec\ub9f7\ud558\ub824\uace0 \uc2dc\ub3c4\ud588\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "''{0}'' \ubc18\ubcf5\uae30\ub97c \ubcf5\uc81c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "''{0}'' \ucd95\uc5d0 \ub300\ud55c \ubc18\ubcf5\uae30\uac00 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "\uc720\ud615\uc774 \uc9c0\uc815\ub41c \ucd95 ''{0}''\uc5d0 \ub300\ud574 \ubc18\ubcf5\uae30\uac00 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "''{0}'' \uc18d\uc131\uc774 \uc694\uc18c\uc758 \uc678\ubd80\uc5d0 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "''{0}''=''{1}'' \uc774\ub984 \uacf5\uac04 \uc120\uc5b8\uc774 \uc694\uc18c\uc758 \uc678\ubd80\uc5d0 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "''{0}'' \uc811\ub450\ubd80\uc5d0 \ub300\ud55c \uc774\ub984 \uacf5\uac04\uc774 \uc120\uc5b8\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "\uc18c\uc2a4 DOM\uc758 \uc62c\ubc14\ub974\uc9c0 \uc54a\uc740 \uc720\ud615\uc744 \uc0ac\uc6a9\ud558\uc5ec DOMAdapter\uac00 \uc791\uc131\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "\uc0ac\uc6a9 \uc911\uc778 SAX \uad6c\ubb38 \ubd84\uc11d\uae30\uac00 DTD \uc120\uc5b8 \uc774\ubca4\ud2b8\ub97c \ucc98\ub9ac\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "\uc0ac\uc6a9 \uc911\uc778 SAX \uad6c\ubb38 \ubd84\uc11d\uae30\uac00 XML \uc774\ub984 \uacf5\uac04\uc744 \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "''{0}'' URI \ucc38\uc870\ub97c \ubd84\uc11d\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "''{0}'' XSL \uc694\uc18c\uac00 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "''{0}'' XSLTC \ud655\uc7a5\uc790\ub97c \uc778\uc2dd\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "\uc0ac\uc6a9 \uc911\uc778 XSLTC \ub7f0\ud0c0\uc784 \ubc84\uc804\ubcf4\ub2e4 \ub354 \ucd5c\uc2e0\uc758 XSLTC \ubc84\uc804\uc744 \uc0ac\uc6a9\ud558\uc5ec \uc9c0\uc815\ub41c ''{0}'' Translet\uc744 \uc791\uc131\ud588\uc2b5\ub2c8\ub2e4. \uc774 Translet\uc744 \uc2e4\ud589\ud558\ub824\uba74 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\ub97c \ub2e4\uc2dc \ucef4\ud30c\uc77c\ud558\uac70\ub098 \ub354 \ucd5c\uc2e0\uc758 XSLTC \ubc84\uc804\uc744 \uc0ac\uc6a9\ud574\uc57c \ud569\ub2c8\ub2e4. "},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "\uac12\uc774 QName\uc774\uc5b4\uc57c \ud558\ub294 \uc18d\uc131\uc5d0 ''{0}'' \uac12\uc774 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "\uac12\uc774 NCName\uc774\uc5b4\uc57c \ud558\ub294 \uc18d\uc131\uc5d0 ''{0}'' \uac12\uc774 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "\ubcf4\uc548 \ucc98\ub9ac \uae30\ub2a5\uc774 true\ub85c \uc124\uc815\ub41c \uacbd\uc6b0\uc5d0\ub294 ''{0}'' \ud655\uc7a5 \ud568\uc218\ub97c \uc0ac\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "\ubcf4\uc548 \ucc98\ub9ac \uae30\ub2a5\uc774 true\ub85c \uc124\uc815\ub41c \uacbd\uc6b0\uc5d0\ub294 ''{0}'' \ud655\uc7a5 \uc694\uc18c\ub97c \uc0ac\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."}
			  };
			}
		}

	}

}