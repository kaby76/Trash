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
 * $Id: SerializerMessages.java 471981 2006-11-07 04:28:00Z minchau $
 */
namespace org.apache.xml.serializer.utils
{

	/// <summary>
	/// An instance of this class is a ListResourceBundle that
	/// has the required getContents() method that returns
	/// an array of message-key/message associations.
	/// <para>
	/// The message keys are defined in <seealso cref="MsgKey"/>. The
	/// messages that those keys map to are defined here.
	/// </para>
	/// <para>
	/// The messages in the English version are intended to be
	/// translated. 
	/// 
	/// This class is not a public API, it is only public because it is 
	/// used in the serializer.
	/// 
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	public class SerializerMessages : ListResourceBundle
	{

		/*
		 * This file contains error and warning messages related to
		 * Serializer Error Handling.
		 *
		 *  General notes to translators:
		
		 *  1) A stylesheet is a description of how to transform an input XML document
		 *     into a resultant XML document (or HTML document or text).  The
		 *     stylesheet itself is described in the form of an XML document.
		
		 *
		 *  2) An element is a mark-up tag in an XML document; an attribute is a
		 *     modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
		 *     "elem" is an element name, "attr" and "attr2" are attribute names with
		 *     the values "val" and "val2", respectively.
		 *
		 *  3) A namespace declaration is a special attribute that is used to associate
		 *     a prefix with a URI (the namespace).  The meanings of element names and
		 *     attribute names that use that prefix are defined with respect to that
		 *     namespace.
		 *
		 *
		 */

		/// <summary>
		/// The lookup table for error messages. </summary>
		public virtual object[][] Contents
		{
			get
			{
				object[][] contents = new object[][]
				{
					new object[] {MsgKey.BAD_MSGKEY, "The message key ''{0}'' is not in the message class ''{1}''"},
					new object[] {MsgKey.BAD_MSGFORMAT, "The format of message ''{0}'' in message class ''{1}'' failed."},
					new object[] {MsgKey.ER_SERIALIZER_NOT_CONTENTHANDLER, "The serializer class ''{0}'' does not implement org.xml.sax.ContentHandler."},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_FIND, "The resource [ {0} ] could not be found.\n {1}"},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_LOAD, "The resource [ {0} ] could not load: {1} \n {2} \t {3}"},
					new object[] {MsgKey.ER_BUFFER_SIZE_LESSTHAN_ZERO, "Buffer size <=0"},
					new object[] {MsgKey.ER_INVALID_UTF16_SURROGATE, "Invalid UTF-16 surrogate detected: {0} ?"},
					new object[] {MsgKey.ER_OIERROR, "IO error"},
					new object[] {MsgKey.ER_ILLEGAL_ATTRIBUTE_POSITION, "Cannot add attribute {0} after child nodes or before an element is produced.  Attribute will be ignored."},
					new object[] {MsgKey.ER_NAMESPACE_PREFIX, "Namespace for prefix ''{0}'' has not been declared."},
					new object[] {MsgKey.ER_STRAY_ATTRIBUTE, "Attribute ''{0}'' outside of element."},
					new object[] {MsgKey.ER_STRAY_NAMESPACE, "Namespace declaration ''{0}''=''{1}'' outside of element."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_RESOURCE, "Could not load ''{0}'' (check CLASSPATH), now using just the defaults"},
					new object[] {MsgKey.ER_ILLEGAL_CHARACTER, "Attempt to output character of integral value {0} that is not represented in specified output encoding of {1}."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_METHOD_PROPERTY, "Could not load the propery file ''{0}'' for output method ''{1}'' (check CLASSPATH)"},
					new object[] {MsgKey.ER_INVALID_PORT, "Invalid port number"},
					new object[] {MsgKey.ER_PORT_WHEN_HOST_NULL, "Port cannot be set when host is null"},
					new object[] {MsgKey.ER_HOST_ADDRESS_NOT_WELLFORMED, "Host is not a well formed address"},
					new object[] {MsgKey.ER_SCHEME_NOT_CONFORMANT, "The scheme is not conformant."},
					new object[] {MsgKey.ER_SCHEME_FROM_NULL_STRING, "Cannot set scheme from null string"},
					new object[] {MsgKey.ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Path contains invalid escape sequence"},
					new object[] {MsgKey.ER_PATH_INVALID_CHAR, "Path contains invalid character: {0}"},
					new object[] {MsgKey.ER_FRAG_INVALID_CHAR, "Fragment contains invalid character"},
					new object[] {MsgKey.ER_FRAG_WHEN_PATH_NULL, "Fragment cannot be set when path is null"},
					new object[] {MsgKey.ER_FRAG_FOR_GENERIC_URI, "Fragment can only be set for a generic URI"},
					new object[] {MsgKey.ER_NO_SCHEME_IN_URI, "No scheme found in URI"},
					new object[] {MsgKey.ER_CANNOT_INIT_URI_EMPTY_PARMS, "Cannot initialize URI with empty parameters"},
					new object[] {MsgKey.ER_NO_FRAGMENT_STRING_IN_PATH, "Fragment cannot be specified in both the path and fragment"},
					new object[] {MsgKey.ER_NO_QUERY_STRING_IN_PATH, "Query string cannot be specified in path and query string"},
					new object[] {MsgKey.ER_NO_PORT_IF_NO_HOST, "Port may not be specified if host is not specified"},
					new object[] {MsgKey.ER_NO_USERINFO_IF_NO_HOST, "Userinfo may not be specified if host is not specified"},
					new object[] {MsgKey.ER_XML_VERSION_NOT_SUPPORTED, "Warning:  The version of the output document is requested to be ''{0}''.  This version of XML is not supported.  The version of the output document will be ''1.0''."},
					new object[] {MsgKey.ER_SCHEME_REQUIRED, "Scheme is required!"},
					new object[] {MsgKey.ER_FACTORY_PROPERTY_MISSING, "The Properties object passed to the SerializerFactory does not have a ''{0}'' property."},
					new object[] {MsgKey.ER_ENCODING_NOT_SUPPORTED, "Warning:  The encoding ''{0}'' is not supported by the Java runtime."},
					new object[] {MsgKey.ER_FEATURE_NOT_FOUND, "The parameter ''{0}'' is not recognized."},
					new object[] {MsgKey.ER_FEATURE_NOT_SUPPORTED, "The parameter ''{0}'' is recognized but the requested value cannot be set."},
					new object[] {MsgKey.ER_STRING_TOO_LONG, "The resulting string is too long to fit in a DOMString: ''{0}''."},
					new object[] {MsgKey.ER_TYPE_MISMATCH_ERR, "The value type for this parameter name is incompatible with the expected value type."},
					new object[] {MsgKey.ER_NO_OUTPUT_SPECIFIED, "The output destination for data to be written to was null."},
					new object[] {MsgKey.ER_UNSUPPORTED_ENCODING, "An unsupported encoding is encountered."},
					new object[] {MsgKey.ER_UNABLE_TO_SERIALIZE_NODE, "The node could not be serialized."},
					new object[] {MsgKey.ER_CDATA_SECTIONS_SPLIT, "The CDATA Section contains one or more termination markers ']]>'."},
					new object[] {MsgKey.ER_WARNING_WF_NOT_CHECKED, "An instance of the Well-Formedness checker could not be created.  The well-formed parameter was set to true but well-formedness checking can not be performed."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER, "The node ''{0}'' contains invalid XML characters."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_COMMENT, "An invalid XML character (Unicode: 0x{0}) was found in the comment."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_PI, "An invalid XML character (Unicode: 0x{0}) was found in the processing instructiondata."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_CDATA, "An invalid XML character (Unicode: 0x{0}) was found in the contents of the CDATASection."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_TEXT, "An invalid XML character (Unicode: 0x{0}) was found in the node''s character data content."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, "An invalid XML character(s) was found in the {0} node named ''{1}''."},
					new object[] {MsgKey.ER_WF_DASH_IN_COMMENT, "The string \"--\" is not permitted within comments."},
					new object[] {MsgKey.ER_WF_LT_IN_ATTVAL, "The value of attribute \"{1}\" associated with an element type \"{0}\" must not contain the ''<'' character."},
					new object[] {MsgKey.ER_WF_REF_TO_UNPARSED_ENT, "The unparsed entity reference \"&{0};\" is not permitted."},
					new object[] {MsgKey.ER_WF_REF_TO_EXTERNAL_ENT, "The external entity reference \"&{0};\" is not permitted in an attribute value."},
					new object[] {MsgKey.ER_NS_PREFIX_CANNOT_BE_BOUND, "The prefix \"{0}\" can not be bound to namespace \"{1}\"."},
					new object[] {MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, "The local name of element \"{0}\" is null."},
					new object[] {MsgKey.ER_NULL_LOCAL_ATTR_NAME, "The local name of attr \"{0}\" is null."},
					new object[] {MsgKey.ER_ELEM_UNBOUND_PREFIX_IN_ENTREF, "The replacement text of the entity node \"{0}\" contains an element node \"{1}\" with an unbound prefix \"{2}\"."},
					new object[] {MsgKey.ER_ATTR_UNBOUND_PREFIX_IN_ENTREF, "The replacement text of the entity node \"{0}\" contains an attribute node \"{1}\" with an unbound prefix \"{2}\"."},
					new object[] {MsgKey.ER_WRITING_INTERNAL_SUBSET, "An error occured while writing the internal subset."}
				};
    
				return contents;
			}
		}
	}

}