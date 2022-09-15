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
 * $Id: DOMConstants.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{
	/// <summary>
	/// DOM Constants used by the DOM Level 3 LSSerializer implementation.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal sealed class DOMConstants
	{
		//
		// Constants: DOM Level 3 feature ids
		//
		public const string DOM3_REC_URL = "http://www.w3.org/TR/DOM-Level-3-LS";

		public const string XERCES_URL = "http://xml.apache.org/xerces-2j";

		// The namespace used to qualified DOM Level 3 DOMConfiguration parameters
		public static readonly string S_DOM3_PROPERTIES_NS = "{" + DOMConstants.DOM3_REC_URL + "}";

		public static readonly string S_XERCES_PROPERTIES_NS = "{" + DOMConstants.XERCES_URL + "}";

		// xmlns namespaces 
		private const string XMLNS_URI = "http://www.w3.org/2000/xmlns/";

		// namespace prefix
		private const string XMLNS_PREFIX = "xmlns";

		// ************************************************************************
		// DOM Level 3 DOM Configuration parameter names
		// ************************************************************************
		// DOM Level 3 parameters defined in Core
		public const string DOM_CANONICAL_FORM = "canonical-form"; // Unsupported, we only appear to support this

		public const string DOM_CDATA_SECTIONS = "cdata-sections";

		public const string DOM_CHECK_CHAR_NORMALIZATION = "check-character-normalization"; // Unsupported

		public const string DOM_COMMENTS = "comments";

		public const string DOM_DATATYPE_NORMALIZATION = "datatype-normalization"; // Unsupported

		public const string DOM_ELEMENT_CONTENT_WHITESPACE = "element-content-whitespace";

		public const string DOM_ENTITIES = "entities";

		public const string DOM_INFOSET = "infoset";

		public const string DOM_NAMESPACES = "namespaces";

		public const string DOM_NAMESPACE_DECLARATIONS = "namespace-declarations";

		public const string DOM_NORMALIZE_CHARACTERS = "normalize-characters"; // Unsupported

		public const string DOM_SPLIT_CDATA = "split-cdata-sections";

		public const string DOM_VALIDATE_IF_SCHEMA = "validate-if-schema"; // Unsopported

		public const string DOM_VALIDATE = "validate"; // Unsopported

		public const string DOM_WELLFORMED = "well-formed";

		// DOM Level 3 Save
		public const string DOM_DISCARD_DEFAULT_CONTENT = "discard-default-content";

		public const string DOM_FORMAT_PRETTY_PRINT = "format-pretty-print";

		public const string DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS = "ignore-unknown-character-denormalizations"; // Unsupported

		public const string DOM_XMLDECL = "xml-declaration";

		// DOM Properties
		public const string DOM_ERROR_HANDLER = "error-handler";

		public const string DOM_SCHEMA_TYPE = "schema-type"; // Unsupported

		public const string DOM_SCHEMA_LOCATION = "schema-location"; // Unsupported

		// ************************************************************************

		// XSL Output properties
		// The xsl:output 'indent' property used in LSSerializer 
		public const string S_XSL_OUTPUT_INDENT = "indent";

		// The xsl:output 'indent' property used in LSSerializer 
		public const string S_XSL_OUTPUT_ENCODING = "encoding";

		// The xsl:output 'omit-xml-declaration' property used in LSSerializer 
		public const string S_XSL_OUTPUT_OMIT_XML_DECL = "omit-xml-declaration";

		// The xerces serializer specific 'omit-xml-declaration' property used in LSSerializer 
		public const string S_XML_VERSION = "xml-version";

		//     
		public const string S_XSL_VALUE_ENTITIES = "org/apache/xml/serializer/XMLEntities";

		// Parameter values
		public const string DOM3_EXPLICIT_TRUE = "explicit:yes";

		public const string DOM3_DEFAULT_TRUE = "default:yes";

		public const string DOM3_EXPLICIT_FALSE = "explicit:no";

		public const string DOM3_DEFAULT_FALSE = "default:no";

		// DOM Exceptions
		public const string DOM_EXCEPTION_FEATURE_NOT_FOUND = "FEATURE_NOT_FOUND";

		public const string DOM_EXCEPTION_FEATURE_NOT_SUPPORTED = "FEATURE_NOT_SUPPORTED";

		public const string DOM_LSEXCEPTION_SERIALIZER_ERR = "SERIALIZER_ERROR";

	}

}