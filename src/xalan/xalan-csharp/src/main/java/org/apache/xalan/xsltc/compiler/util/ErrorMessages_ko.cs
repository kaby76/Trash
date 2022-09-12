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
 * $Id: ErrorMessages_ko.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_ko : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "\ud558\ub098 \uc774\uc0c1\uc758 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\uac00 \ub3d9\uc77c\ud55c \ud30c\uc77c\uc5d0\uc11c \uc815\uc758\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "''{0}'' \ud15c\ud50c\ub9ac\ud2b8\uac00 \uc774\ubbf8 \uc774 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\uc5d0\uc11c \uc815\uc758\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "''{0}'' \ud15c\ud50c\ub9ac\ud2b8\uac00 \uc774 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\uc5d0\uc11c \uc815\uc758\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "''{0}'' \ubcc0\uc218\uac00 \ub3d9\uc77c\ud55c \ubc94\uc704 \uc548\uc5d0\uc11c \uc5ec\ub7ec \ubc88 \uc815\uc758\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "''{0}'' \ub9e4\uac1c\ubcc0\uc218 \ub610\ub294 \ubcc0\uc218\uac00 \uc815\uc758\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "''{0}'' \ud074\ub798\uc2a4\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "''{0}'' \uc678\ubd80 \uba54\uc18c\ub4dc\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. (public\uc774\uc5b4\uc57c \ud569\ub2c8\ub2e4.)"},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "''{0}'' \uba54\uc18c\ub4dc\ub85c\uc758 \ud638\ucd9c\uc5d0\uc11c \uc778\uc218/\ub9ac\ud134 \uc720\ud615\uc744 \ubcc0\ud658\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "''{0}'' URI \ub610\ub294 \ud30c\uc77c\uc744 \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "''{0}'' URI\uac00 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "''{0}'' URI \ub610\ub294 \ud30c\uc77c\uc744 \uc5f4 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "<xsl:stylesheet> \ub610\ub294 <xsl:transform> \uc694\uc18c\uac00 \uc608\uc0c1\ub429\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "''{0}'' \uc774\ub984 \uacf5\uac04 \uc811\ub450\ubd80\uac00 \uc120\uc5b8\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "''{0}'' \ud568\uc218\uc5d0 \ub300\ud55c \ud638\ucd9c\uc744 \ubd84\uc11d\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "''{0}''\uc5d0 \ub300\ud55c \uc778\uc218\ub294 \ub9ac\ud130\ub7f4 \ubb38\uc790\uc5f4\uc774\uc5b4\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "''{0}'' XPath \ud45c\ud604\uc2dd \uad6c\ubb38 \ubd84\uc11d \uc911\uc5d0 \uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "''{0}'' \ud544\uc218 \uc18d\uc131\uc774 \ub204\ub77d\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "XPath \ud45c\ud604\uc2dd\uc758 ''{0}'' \ubb38\uc790\uac00 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "\ucc98\ub9ac \uba85\ub839\uc5b4\uc5d0 \ub300\ud55c ''{0}'' \uc774\ub984\uc774 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "''{0}'' \uc18d\uc131\uc774 \uc694\uc18c\uc758 \uc678\ubd80\uc5d0 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "''{0}'' \uc18d\uc131\uc774 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "import/include\uac00 \uc21c\ud658\ub429\ub2c8\ub2e4. ''{0}'' \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\uac00 \uc774\ubbf8 \ub85c\ub4dc\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "\uacb0\uacfc \ud2b8\ub9ac \ub2e8\ud3b8\uc744 \uc815\ub82c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. (<xsl:sort> \uc694\uc18c\uac00 \ubb34\uc2dc\ub429\ub2c8\ub2e4.) \uacb0\uacfc \ud2b8\ub9ac\ub97c \uc791\uc131\ud560 \ub54c \ub178\ub4dc\ub97c \uc815\ub82c\ud574\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "''{0}'' 10\uc9c4\uc218 \ud3ec\ub9f7\ud305\uc774 \uc774\ubbf8 \uc815\uc758\ub418\uc5b4 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSLTC\uc5d0\uc11c ''{0}'' XSL \ubc84\uc804\uc744 \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "''{0}''\uc5d0\uc11c \ubcc0\uc218/\ub9e4\uac1c\ubcc0\uc218 \ucc38\uc870\uac00 \uc21c\ud658\ub429\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "2\uc9c4 \ud45c\ud604\uc2dd\uc5d0 \ub300\ud55c \uc5f0\uc0b0\uc790\ub97c \uc54c \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "\ud568\uc218 \ud638\ucd9c\uc5d0 \ub300\ud55c \uc778\uc218\uac00 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "document() \ud568\uc218\uc5d0 \ub300\ud55c \ub450 \ubc88\uc9f8 \uc778\uc218\ub294 node-set\uc5ec\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "<xsl:choose>\uc5d0 \ucd5c\uc18c \ud558\ub098\uc758 <xsl:when> \uc694\uc18c\uac00 \ud544\uc694\ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "<xsl:choose>\uc5d0 \ud558\ub098\uc758 <xsl:otherwise> \uc694\uc18c\ub9cc\uc774 \ud5c8\uc6a9\ub429\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise>\ub294 <xsl:choose>\uc5d0\uc11c\ub9cc \uc0ac\uc6a9\ub420 \uc218 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when>\uc740 <xsl:choose>\uc5d0\uc11c\ub9cc \uc0ac\uc6a9\ub420 \uc218 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "<xsl:when> \ubc0f <xsl:otherwise> \uc694\uc18c\ub9cc\uc774 <xsl:choose>\uc5d0\uc11c \ud5c8\uc6a9\ub429\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set>\uc774 'name' \uc18d\uc131\uc5d0\uc11c \ub204\ub77d\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "\ud558\uc704 \uc694\uc18c\uac00 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "''{0}'' \uc694\uc18c\ub97c \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "''{0}'' \uc18d\uc131\uc744 \ud638\ucd9c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "\ud14d\uc2a4\ud2b8 \ub370\uc774\ud130\uac00 \ucd5c\uc0c1\uc704 \ub808\ubca8 <xsl:stylesheet> \uc694\uc18c\uc758 \uc678\ubd80\uc5d0 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP \uad6c\ubb38 \ubd84\uc11d\uae30\uac00 \uc81c\ub300\ub85c \uad6c\uc131\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.INTERNAL_ERR, "\ubcf5\uad6c\ud560 \uc218 \uc5c6\ub294 XSLTC-\ub0b4\ubd80 \uc624\ub958: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "''{0}'' XSL \uc694\uc18c\uac00 \uc9c0\uc6d0\ub418\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "''{0}'' XSLTC \ud655\uc7a5\uc790\ub97c \uc778\uc2dd\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "\uc785\ub825 \ubb38\uc11c\ub294 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\uac00 \uc544\ub2d9\ub2c8\ub2e4. (XSL \uc774\ub984 \uacf5\uac04\uc774 \ub8e8\ud2b8 \uc694\uc18c\uc5d0\uc11c \uc120\uc5b8\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4.)"},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "''{0}'' \uc2a4\ud0c0\uc77c\uc2dc\ud2b8 \ub300\uc0c1\uc744 \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "\uad6c\ud604\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4: ''{0}''"},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "\uc785\ub825 \ubb38\uc11c\uc5d0 XSL \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\uac00 \ud3ec\ud568\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "''{0}'' \uc694\uc18c\ub97c \uad6c\ubb38 \ubd84\uc11d\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "<key>\uc758 use \uc18d\uc131\uc740 node, node-set, string \ub610\ub294 number\uc5ec\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "\ucd9c\ub825 XML \ubb38\uc11c \ubc84\uc804\uc740 1.0\uc774\uc5b4\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "\uad00\uacc4\uc2dd\uc5d0 \ub300\ud55c \uc5f0\uc0b0\uc790\ub97c \uc54c \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "\uc874\uc7ac\ud558\uc9c0 \uc54a\ub294 \uc18d\uc131 \uc138\ud2b8 ''{0}'' \uc0ac\uc6a9\uc744 \uc2dc\ub3c4 \uc911\uc785\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "''{0}'' \uc18d\uc131\uac12 \ud15c\ud50c\ub9ac\ud2b8\ub97c \uad6c\ubb38 \ubd84\uc11d\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "''{0}'' \ud074\ub798\uc2a4\uc5d0 \ub300\ud55c \uc11c\uba85\uc5d0 \uc54c \uc218 \uc5c6\ub294 \ub370\uc774\ud130 \uc720\ud615\uc774 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "\ub370\uc774\ud130 \uc720\ud615\uc744 ''{0}''\uc5d0\uc11c ''{1}''(\uc73c)\ub85c \ubcc0\ud658\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "\uc774 Templates\uc5d0\ub294 \uc720\ud6a8\ud55c translet \ud074\ub798\uc2a4 \uc815\uc758\uac00 \ud3ec\ud568\ub418\uc5b4 \uc788\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "\uc774 Templates\uc5d0\ub294 ''{0}'' \uc774\ub984\uc778 \ud074\ub798\uc2a4\uac00 \ud3ec\ud568\ub418\uc5b4 \uc788\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "''{0}'' translet \ud074\ub798\uc2a4\ub97c \ub85c\ub4dc\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "translet \ud074\ub798\uc2a4\uac00 \ub85c\ub4dc\ub418\uc5c8\uc9c0\ub9cc translet \uc778\uc2a4\ud134\uc2a4\ub97c \uc791\uc131\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "''{0}''\uc5d0 \ub300\ud55c ErrorListener\ub97c \ub110(null)\ub85c \uc124\uc815\ud558\ub824\uace0 \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC\uc5d0\uc11c StreamSource, SAXSource \ubc0f DOMSource\ub9cc\uc744 \uc9c0\uc6d0\ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "''{0}''(\uc73c)\ub85c \ud328\uc2a4\ub41c Source \uc624\ube0c\uc81d\ud2b8\uc5d0 \ucee8\ud150\uce20\uac00 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "\uc2a4\ud0c0\uc77c\uc2dc\ud2b8\ub97c \ucef4\ud30c\uc77c\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory ''{0}'' \uc18d\uc131\uc744 \uc778\uc2dd\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult()\ub294 startDocument()\ubcf4\ub2e4 \uba3c\uc800 \ud638\ucd9c\ub418\uc5b4\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer\uc5d0 \uc694\uc57d\ub41c translet \uc624\ube0c\uc81d\ud2b8\uac00 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "\ubcc0\ud658 \uacb0\uacfc\uc5d0 \ub300\ud55c \ucd9c\ub825 \ud578\ub4e4\ub7ec\uac00 \uc815\uc758\ub418\uc9c0 \uc54a\uc558\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "''{0}''(\uc73c)\ub85c \ud328\uc2a4\ub41c Result \uc624\ube0c\uc81d\ud2b8\uac00 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "''{0}'' \uc798\ubabb\ub41c Transformer \ud2b9\uc131\uc5d0 \uc561\uc138\uc2a4\ud558\ub824\uace0 \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "SAX2DOM ''{0}'' \uc5b4\ub311\ud130\ub97c \uc791\uc131\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build()\uac00 \uc124\uc815\ub41c \uc2dc\uc2a4\ud15c ID \uc5c6\uc774 \ud638\ucd9c\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "\uacb0\uacfc\ub294 \ub110(null)\uc774 \ub420 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "{0} \ub9e4\uac1c\ubcc0\uc218 \uac12\uc740 \uc720\ud6a8\ud55c Java \uc624\ube0c\uc81d\ud2b8\uc5ec\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "-i \uc635\uc158\uc740 -o \uc635\uc158\uacfc \ud568\uaed8 \uc0ac\uc6a9\ub418\uc5b4\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n java org.apache.xalan.xsltc.cmdline.Compile [-o <output>]\n [-d <directory>] [-j <jarfile>] [-p <package>]\n [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\n \uc635\uc158\n -o <output>    \uc0dd\uc131\ub41c Translet\uc5d0 <output> \uc774\ub984\uc744 \uc9c0\uc815\ud569\ub2c8\ub2e4. \n                \uae30\ubcf8\uc801\uc73c\ub85c Translet \uc774\ub984\uc744 <stylesheet> \uc774\ub984\uc5d0\uc11c\n \uac00\uc838\uc635\ub2c8\ub2e4. \uc774 \uc635\uc158\uc740 \uc5ec\ub7ec \uac1c\uc758 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\ub97c \n \ucef4\ud30c\uc77c\ud560 \uacbd\uc6b0 \ubb34\uc2dc\ub429\ub2c8\ub2e4.\n -d <directory> Translet\uc758 \ub300\uc0c1 \ub514\ub809\ud1a0\ub9ac\ub97c \uc9c0\uc815\ud569\ub2c8\ub2e4.\n -j <jarfile>   <jarfile>\ub85c \uc9c0\uc815\ub41c jar \ud30c\uc77c \uc774\ub984\uc73c\ub85c\n Translet \ud074\ub798\uc2a4\ub97c \ud328\ud0a4\uc9c0\ud569\ub2c8\ub2e4.\n -p <package>   \uc0dd\uc131\ub41c \ubaa8\ub4e0 Translet \ud074\ub798\uc2a4\uc5d0 \ub300\ud574 \ud328\ud0a4\uc9c0 \uc774\ub984 \uc811\ub450\ubd80\ub97c\n \uc9c0\uc815\ud569\ub2c8\ub2e4.\n -n             \ud15c\ud50c\ub9ac\ud2b8 \uc778\ub77c\uc774\ub2dd(\ud3c9\uade0\ubcf4\ub2e4 \uc6b0\uc218\ud55c)\uc744\n \uc0ac\uc6a9 \uac00\ub2a5\ud558\uac8c \ud569\ub2c8\ub2e4.\n -x             \ucd94\uac00 \ub514\ubc84\uae45 \uba54\uc2dc\uc9c0 \ucd9c\ub825\uc744 \uc2dc\uc791\ud569\ub2c8\ub2e4.\n -u             <stylesheet> \uc778\uc218\ub97c URL\ub85c \ud574\uc11d\ud569\ub2c8\ub2e4.\n -i             stdin\uc73c\ub85c\ubd80\ud130 \uc2a4\ud0c0\uc77c\uc2dc\ud2b8\ub97c \uc77d\ub3c4\ub85d \ucef4\ud30c\uc77c\ub7ec\ub97c \uac15\uc81c \uc2e4\ud589\ud569\ub2c8\ub2e4.\n -v             \ucef4\ud30c\uc77c\ub7ec \ubc84\uc804\uc744 \uc778\uc1c4\ud569\ub2c8\ub2e4.\n -h             \uc0ac\uc6a9\ubc95 \uba85\ub839\ubb38\uc744 \uc778\uc1c4\ud569\ub2c8\ub2e4.\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n java org.apache.xalan.xsltc.cmdline.Transform [-j <jarfile>]\n [-x] [-n <iterations>] {-u <document_url> | <document>}\n <class> [<param1>=<value1> ...]\n\n Translet <class>\ub97c \uc0ac\uc6a9\ud558\uc5ec <document>\ub85c \uc9c0\uc815\ub41c XML \ubb38\uc11c\ub97c \n \ubcc0\ud658\ud569\ub2c8\ub2e4. Translet <class>\ub294 \uc0ac\uc6a9\uc790\uc758 CLASSPATH \ub610\ub294\n \uc120\ud0dd\uc801\uc73c\ub85c \uc9c0\uc815\ub41c <jarfile> \ub0b4\uc5d0 \uc788\uc2b5\ub2c8\ub2e4.\n\uc635\uc158\n -j <jarfile>      Translet\uc744 \ub85c\ub4dc\ud574\uc62c jarfile\uc744 \uc9c0\uc815\ud569\ub2c8\ub2e4.\n -x                \ucd94\uac00 \ub514\ubc84\uae45 \uba54\uc2dc\uc9c0 \ucd9c\ub825\uc744 \uc2dc\uc791\ud569\ub2c8\ub2e4.\n -n <iterations>   <iterations> \ucc28\ub840 \ubcc0\ud658\uc744 \uc2e4\ud589\ud558\uba70\n \ud504\ub85c\ud30c\uc77c\ub9c1 \uc815\ubcf4\ub97c \ud45c\uc2dc\ud569\ub2c8\ub2e4.\n -u <document_url> XML \uc785\ub825 \ubb38\uc11c\ub97c URL\ub85c \uc9c0\uc815\ud569\ub2c8\ub2e4.\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort>\ub294 <xsl:for-each> \ub610\ub294 <xsl:apply-templates>\uc5d0\uc11c\ub9cc \uc0ac\uc6a9\ub420 \uc218 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "\uc774 JVM\uc5d0\uc11c ''{0}'' \ucd9c\ub825 \uc778\ucf54\ub529\uc744 \uc9c0\uc6d0\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "''{0}''\uc5d0 \uad6c\ubb38 \uc624\ub958\uac00 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "''{0}'' \uc678\ubd80 \uad6c\uc131\uc790\ub97c \ucc3e\uc744 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "non-static Java \ud568\uc218 ''{0}''\uc758 \uccab \ubc88\uc9f8 \uc778\uc218\uac00 \uc720\ud6a8\ud55c \uc624\ube0c\uc81d\ud2b8 \ucc38\uc870\uac00 \uc544\ub2d9\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "''{0}'' \ud45c\ud604\uc2dd\uc758 \uc720\ud615\uc744 \uac80\uc0ac\ud558\ub294 \uc911\uc5d0 \uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "\uc54c \uc218 \uc5c6\ub294 \uc704\uce58\uc5d0\uc11c \ud45c\ud604\uc2dd\uc758 \uc720\ud615\uc744 \uac80\uc0ac\ud558\ub294 \uc911\uc5d0 \uc624\ub958\uac00 \ubc1c\uc0dd\ud588\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "''{0}'' \uba85\ub839\ud589 \uc635\uc158\uc774 \uc720\ud6a8\ud558\uc9c0 \uc54a\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "''{0}'' \uba85\ub839\ud589 \uc635\uc158\uc5d0 \ud544\uc218 \uc778\uc218\uac00 \ub204\ub77d\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "\uacbd\uace0:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "\uacbd\uace0:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "\uc2ec\uac01\ud55c \uc624\ub958:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "\uc2ec\uac01\ud55c \uc624\ub958:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "\uc624\ub958:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "\uc624\ub958:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "''{0}'' translet\uc744 \uc0ac\uc6a9\ud558\uc5ec \ubcc0\ud658\ud558\uc2ed\uc2dc\uc624."},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "''{1}'' jar \ud30c\uc77c\uc758 ''{0}'' Translet\uc744 \uc0ac\uc6a9\ud558\uc5ec \ubcc0\ud658\ud558\uc2ed\uc2dc\uc624."},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "TransformerFactory \ud074\ub798\uc2a4 ''{0}''\uc758 \uc778\uc2a4\ud134\uc2a4\ub97c \uc791\uc131\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "''{0}'' \uc774\ub984\uc740 Java \ud074\ub798\uc2a4 \uc774\ub984\uc5d0\uc11c \uc0ac\uc6a9\ud560 \uc218 \uc5c6\ub294 \ubb38\uc790\ub97c \ud3ec\ud568\ud558\uace0 \uc788\uc73c\ubbc0\ub85c Translet \ud074\ub798\uc2a4 \uc774\ub984\uc73c\ub85c \uc0ac\uc6a9\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4. \ub300\uc2e0\uc5d0 ''{1}'' \uc774\ub984\uc774 \uc0ac\uc6a9\ub418\uc5c8\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "\ucef4\ud30c\uc77c\ub7ec \uc624\ub958:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "\ucef4\ud30c\uc77c\ub7ec \uacbd\uace0:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Translet \uc624\ub958:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "\uac12\uc774 QName \ub610\ub294 QName\uc758 \ud654\uc774\ud2b8 \uc2a4\ud398\uc774\uc2a4\ub85c \uad6c\ubd84\ub41c \ubaa9\ub85d\uc774\uc5b4\uc57c \ud558\ub294 \uc18d\uc131\uc5d0 ''{0}'' \uac12\uc774 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "\uac12\uc774 NCName\uc774\uc5b4\uc57c \ud558\ub294 \uc18d\uc131\uc5d0 ''{0}'' \uac12\uc774 \uc788\uc2b5\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "<xsl:output> \uc694\uc18c\uc758 \uba54\uc18c\ub4dc \uc18d\uc131\uc5d0 ''{0}'' \uac12\uc774 \uc788\uc2b5\ub2c8\ub2e4. \uac12\uc740 ''xml'', ''html'', ''text'' \ub610\ub294 qname-but-not-ncname \uc911 \ud558\ub098\uc5ec\uc57c \ud569\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "TransformerFactory.getFeature(\ubb38\uc790\uc5f4 \uc774\ub984)\uc5d0\uc11c \uae30\ub2a5 \uc774\ub984\uc774 \ub110(null)\uc774\uba74 \uc548\ub429\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "TransformerFactory.setFeature(\ubb38\uc790\uc5f4 \uc774\ub984, \ubd80\uc6b8 \uac12)\uc5d0\uc11c \uae30\ub2a5 \uc774\ub984\uc774 \ub110(null)\uc774\uba74 \uc548\ub429\ub2c8\ub2e4."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "\uc774 TransformerFactory\uc5d0\uc11c ''{0}'' \uae30\ub2a5\uc744 \uc124\uc815\ud560 \uc218 \uc5c6\uc2b5\ub2c8\ub2e4."}
			  };
			}
		}

	}

}