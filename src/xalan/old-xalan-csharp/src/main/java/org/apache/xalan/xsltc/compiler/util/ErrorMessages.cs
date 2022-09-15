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
 * $Id: ErrorMessages.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "More than one stylesheet defined in the same file."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "Template ''{0}'' already defined in this stylesheet."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "Template ''{0}'' not defined in this stylesheet."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "Variable ''{0}'' is multiply defined in the same scope."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "Variable or parameter ''{0}'' is undefined."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "Cannot find class ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "Cannot find external method ''{0}'' (must be public)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "Cannot convert argument/return type in call to method ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "File or URI ''{0}'' not found."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Invalid URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Cannot open file or URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "<xsl:stylesheet> or <xsl:transform> element expected."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Namespace prefix ''{0}'' is undeclared."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Unable to resolve call to function ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "Argument to ''{0}'' must be a literal string."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Error parsing XPath expression ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Required attribute ''{0}'' is missing."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Illegal character ''{0}'' in XPath expression."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Illegal name ''{0}'' for processing instruction."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Attribute ''{0}'' outside of element."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Illegal attribute ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Circular import/include. Stylesheet ''{0}'' already loaded."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Result-tree fragments cannot be sorted (<xsl:sort> elements are " + "ignored). You must sort the nodes when creating the result tree."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Decimal formatting ''{0}'' is already defined."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL version ''{0}'' is not supported by XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Circular variable/parameter reference in ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Unknown operator for binary expression."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Illegal argument(s) for function call."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "Second argument to document() function must be a node-set."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "At least one <xsl:when> element required in <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Only one <xsl:otherwise> element allowed in <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> can only be used within <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> can only be used within <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Only <xsl:when> and <xsl:otherwise> elements allowed in <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> is missing the 'name' attribute."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Illegal child element."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "You cannot call an element ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "You cannot call an attribute ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Text data outside of top-level <xsl:stylesheet> element."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP parser not configured correctly"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Unrecoverable XSLTC-internal error: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Unsupported XSL element ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Unrecognised XSLTC extension ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "The input document is not a stylesheet (the XSL namespace is not " + "declared in the root element)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Could not find stylesheet target ''{0}''."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Not implemented: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "The input document does not contain an XSL stylesheet."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "Could not parse element ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "The use attribute of <key> must be node, node-set, string or number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "Output XML document version should be 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Unknown operator for relational expression"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Attempting to use non-existing attribute set ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "Cannot parse attribute value template ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Unknown data-type in signature for class ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "Cannot convert data-type ''{0}'' to ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "This Templates does not contain a valid translet class definition."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "This Templates does not contain a class with the name ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Could not load the translet class ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Translet class loaded, but unable to create translet instance."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Attempting to set ErrorListener for ''{0}'' to null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Only StreamSource, SAXSource and DOMSource are supported by XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "Source object passed to ''{0}'' has no contents."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Could not compile stylesheet"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory does not recognise attribute ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() must be called prior to startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "The Transformer has no encapsulated translet object."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "No defined output handler for transformation result."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "Result object passed to ''{0}'' is invalid."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Attempting to access invalid Transformer property ''{0}''."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "Could not create SAX2DOM adapter: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() called without systemId being set."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Result should not be null"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "The value of param {0} must be a valid Java Object"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "The -i option must be used with the -o option."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n" + "   java org.apache.xalan.xsltc.cmdline.Compile [-o <output>]\n" + "      [-d <directory>] [-j <jarfile>] [-p <package>]\n" + "      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\n" + "OPTIONS\n" + "   -o <output>    assigns the name <output> to the generated\n" + "                  translet.  By default the translet name is\n" + "                  derived from the <stylesheet> name.  This option\n" + "                  is ignored if compiling multiple stylesheets.\n" + "   -d <directory> specifies a destination directory for translet\n" + "   -j <jarfile>   packages translet classes into a jar file of the\n" + "                  name specified as <jarfile>\n" + "   -p <package>   specifies a package name prefix for all generated\n" + "                  translet classes.\n" + "   -n             enables template inlining (default behavior better\n" + "                  on average).\n" + "   -x             turns on additional debugging message output\n" + "   -u             interprets <stylesheet> arguments as URLs\n" + "   -i             forces compiler to read stylesheet from stdin\n" + "   -v             prints the version of the compiler\n" + "   -h             prints this usage statement\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n" + "   java org.apache.xalan.xsltc.cmdline.Transform [-j <jarfile>]\n" + "      [-x] [-n <iterations>] {-u <document_url> | <document>}\n" + "      <class> [<param1>=<value1> ...]\n\n" + "   uses the translet <class> to transform an XML document \n" + "   specified as <document>. The translet <class> is either in\n" + "   the user's CLASSPATH or in the optionally specified <jarfile>.\n" + "OPTIONS\n" + "   -j <jarfile>    specifies a jarfile from which to load translet\n" + "   -x              turns on additional debugging message output\n" + "   -n <iterations> runs the transformation <iterations> times and\n" + "                   displays profiling information\n" + "   -u <document_url> specifies XML input document as a URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> can only be used within <xsl:for-each> or <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "Output encoding ''{0}'' is not supported on this JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Syntax error in ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "Cannot find external constructor ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "The first argument to the non-static Java function ''{0}'' is not a " + "valid object reference."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Error checking type of the expression ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Error checking type of an expression at an unknown location."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "The command-line option ''{0}'' is not valid."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "The command-line option ''{0}'' is missing a required argument."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "WARNING:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "WARNING:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "FATAL ERROR:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "FATAL ERROR:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "ERROR:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "ERROR:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Transform using translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Transform using translet ''{0}'' from jar file ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "Could not create an instance of the TransformerFactory class ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "The name ''{0}'' could not be used as the name of the translet " + "class because it contains characters that are not permitted in the " + "name of Java class.  The name ''{1}'' was used instead."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Compiler errors:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Compiler warnings:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Translet errors:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "An attribute whose value must be a QName or whitespace-separated list of QNames had the value ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "An attribute whose value must be an NCName had the value ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "The method attribute of an <xsl:output> element had the value ''{0}''.  The value must be one of ''xml'', ''html'', ''text'', or qname-but-not-ncname"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "The feature name cannot be null in TransformerFactory.getFeature(String name)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "The feature name cannot be null in TransformerFactory.setFeature(String name, boolean value)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Cannot set the feature ''{0}'' on this TransformerFactory."},
				  new object[] {ErrorMsg.OUTLINE_ERR_TRY_CATCH, "Internal XSLTC error:  the generated byte code contains a " + "try-catch-finally block and cannot be outlined."},
				  new object[] {ErrorMsg.OUTLINE_ERR_UNBALANCED_MARKERS, "Internal XSLTC error:  OutlineableChunkStart and " + "OutlineableChunkEnd markers must be balanced and properly nested."},
				  new object[] {ErrorMsg.OUTLINE_ERR_DELETED_TARGET, "Internal XSLTC error:  an instruction that was part of a block of " + "byte code that was outlined is still referred to in the original " + "method."},
				  new object[] {ErrorMsg.OUTLINE_ERR_METHOD_TOO_BIG, "Internal XSLTC error:  a method in the translet exceeds the Java " + "Virtual Machine limitation on the length of a method of 64 " + "kilobytes.  This is usually caused by templates in a stylesheet " + "that are very large.  Try restructuring your stylesheet to use " + "smaller templates."}
			  };
			}
		}
	}

}