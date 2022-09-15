using System;
using System.IO;
using System.Text;

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
 * $Id: LSSerializerImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{

	using DOM3Serializer = org.apache.xml.serializer.DOM3Serializer;
	using Encodings = org.apache.xml.serializer.Encodings;
	using OutputPropertiesFactory = org.apache.xml.serializer.OutputPropertiesFactory;
	using Serializer = org.apache.xml.serializer.Serializer;
	using SerializerFactory = org.apache.xml.serializer.SerializerFactory;
	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using SystemIDResolver = org.apache.xml.serializer.utils.SystemIDResolver;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using DOMConfiguration = org.w3c.dom.DOMConfiguration;
	using DOMError = org.w3c.dom.DOMError;
	using DOMErrorHandler = org.w3c.dom.DOMErrorHandler;
	using DOMException = org.w3c.dom.DOMException;
	using DOMStringList = org.w3c.dom.DOMStringList;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using LSException = org.w3c.dom.ls.LSException;
	using LSOutput = org.w3c.dom.ls.LSOutput;
	using LSSerializer = org.w3c.dom.ls.LSSerializer;
	using LSSerializerFilter = org.w3c.dom.ls.LSSerializerFilter;

	/// <summary>
	/// Implemenatation of DOM Level 3 org.w3c.ls.LSSerializer and 
	/// org.w3c.dom.ls.DOMConfiguration.  Serialization is achieved by delegating 
	/// serialization calls to <CODE>org.apache.xml.serializer.ToStream</CODE> or 
	/// one of its derived classes depending on the serialization method, while walking
	/// the DOM in DOM3TreeWalker. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-LS-20040407/load-save.html.LS-LSSerializer">org.w3c.dom.ls.LSSerializer</a>"/>
	/// <seealso cref="<a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407/core.html.DOMConfiguration">org.w3c.dom.DOMConfiguration</a>"
	/// 
	/// @version $Id:  
	/// 
	/// @xsl.usage internal />
	public sealed class LSSerializerImpl : DOMConfiguration, LSSerializer
	{

		// The default end-of-line character sequence used in serialization.
		private static readonly string DEFAULT_END_OF_LINE;
		static LSSerializerImpl()
		{
			string lineSeparator = (string) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass());
			// The DOM Level 3 Load and Save specification requires that implementations choose a default
			// sequence which matches one allowed by XML 1.0 (or XML 1.1). If the value of "line.separator" 
			// isn't one of the XML 1.0 end-of-line sequences then we select "\n" as the default value.
			DEFAULT_END_OF_LINE = !string.ReferenceEquals(lineSeparator, null) && (lineSeparator.Equals("\r\n") || lineSeparator.Equals("\r")) ? lineSeparator : "\n";
				try
				{
					fgThrowableInitCauseMethod = typeof(Exception).GetMethod("initCause", new Type [] {typeof(Exception)});
					fgThrowableMethodsAvailable = true;
				}
				// ClassNotFoundException, NoSuchMethodException or SecurityException
				// Whatever the case, we cannot use java.lang.Throwable.initCause(java.lang.Throwable).
				catch (Exception)
				{
					fgThrowableInitCauseMethod = null;
					fgThrowableMethodsAvailable = false;
				}
		}

		private class PrivilegedActionAnonymousInnerClass : PrivilegedAction
		{
			public object run()
			{
				try
				{
					return System.getProperty("line.separator");
				}
				catch (SecurityException)
				{
				}
				return null;
			}
		}

		/// <summary>
		/// private data members </summary>
		private Serializer fXMLSerializer = null;

		// Tracks DOMConfiguration features. 
		protected internal int fFeatures = 0;

		// Common DOM serializer
		private DOM3Serializer fDOMSerializer = null;

		// A filter set on the LSSerializer
		private LSSerializerFilter fSerializerFilter = null;

		// Stores the nodeArg parameter to speed up multiple writes of the same node.
		private Node fVisitedNode = null;

		// The end-of-line character sequence used in serialization. "\n" is whats used on the web.
		private string fEndOfLine = DEFAULT_END_OF_LINE;

		// The DOMErrorhandler.
		private DOMErrorHandler fDOMErrorHandler = null;

		// The Configuration parameter to pass to the Underlying serilaizer.
		private Properties fDOMConfigProperties = null;

		// The encoding to use during serialization.
		private string fEncoding;

		// ************************************************************************
		// DOM Level 3 DOM Configuration parameter names
		// ************************************************************************    
		// Parameter canonical-form, true [optional] - NOT SUPPORTED 
		private static readonly int CANONICAL = 0x1 << 0;

		// Parameter cdata-sections, true [required] (default)
		private static readonly int CDATA = 0x1 << 1;

		// Parameter check-character-normalization, true [optional] - NOT SUPPORTED 
		private static readonly int CHARNORMALIZE = 0x1 << 2;

		// Parameter comments, true [required] (default)
		private static readonly int COMMENTS = 0x1 << 3;

		// Parameter datatype-normalization, true [optional] - NOT SUPPORTED
		private static readonly int DTNORMALIZE = 0x1 << 4;

		// Parameter element-content-whitespace, true [required] (default) - value - false [optional] NOT SUPPORTED
		private static readonly int ELEM_CONTENT_WHITESPACE = 0x1 << 5;

		// Parameter entities, true [required] (default)
		private static readonly int ENTITIES = 0x1 << 6;

		// Parameter infoset, true [required] (default), false has no effect --> True has no effect for the serializer
		private static readonly int INFOSET = 0x1 << 7;

		// Parameter namespaces, true [required] (default)
		private static readonly int NAMESPACES = 0x1 << 8;

		// Parameter namespace-declarations, true [required] (default)
		private static readonly int NAMESPACEDECLS = 0x1 << 9;

		// Parameter normalize-characters, true [optional] - NOT SUPPORTED
		private static readonly int NORMALIZECHARS = 0x1 << 10;

		// Parameter split-cdata-sections, true [required] (default)
		private static readonly int SPLITCDATA = 0x1 << 11;

		// Parameter validate, true [optional] - NOT SUPPORTED
		private static readonly int VALIDATE = 0x1 << 12;

		// Parameter validate-if-schema, true [optional] - NOT SUPPORTED
		private static readonly int SCHEMAVALIDATE = 0x1 << 13;

		// Parameter split-cdata-sections, true [required] (default)
		private static readonly int WELLFORMED = 0x1 << 14;

		// Parameter discard-default-content, true [required] (default)
		// Not sure how this will be used in level 2 Documents
		private static readonly int DISCARDDEFAULT = 0x1 << 15;

		// Parameter format-pretty-print, true [optional] 
		private static readonly int PRETTY_PRINT = 0x1 << 16;

		// Parameter ignore-unknown-character-denormalizations, true [required] (default)
		// We currently do not support XML 1.1 character normalization
		private static readonly int IGNORE_CHAR_DENORMALIZE = 0x1 << 17;

		// Parameter discard-default-content, true [required] (default)
		private static readonly int XMLDECL = 0x1 << 18;
		// ************************************************************************

		// Recognized parameters for which atleast one value can be set
		private string[] fRecognizedParameters = new string[] {DOMConstants.DOM_CANONICAL_FORM, DOMConstants.DOM_CDATA_SECTIONS, DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, DOMConstants.DOM_COMMENTS, DOMConstants.DOM_DATATYPE_NORMALIZATION, DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, DOMConstants.DOM_ENTITIES, DOMConstants.DOM_INFOSET, DOMConstants.DOM_NAMESPACES, DOMConstants.DOM_NAMESPACE_DECLARATIONS, DOMConstants.DOM_SPLIT_CDATA, DOMConstants.DOM_VALIDATE, DOMConstants.DOM_VALIDATE_IF_SCHEMA, DOMConstants.DOM_WELLFORMED, DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, DOMConstants.DOM_FORMAT_PRETTY_PRINT, DOMConstants.DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS, DOMConstants.DOM_XMLDECL, DOMConstants.DOM_ERROR_HANDLER};


		/// <summary>
		/// Constructor:  Creates a LSSerializerImpl object.  The underlying
		/// XML 1.0 or XML 1.1 org.apache.xml.serializer.Serializer object is
		/// created and initialized the first time any of the write methods are  
		/// invoked to serialize the Node.  Subsequent write methods on the same
		/// LSSerializerImpl object will use the previously created Serializer object.
		/// </summary>
		public LSSerializerImpl()
		{
			// set default parameters
			fFeatures |= CDATA;
			fFeatures |= COMMENTS;
			fFeatures |= ELEM_CONTENT_WHITESPACE;
			fFeatures |= ENTITIES;
			fFeatures |= NAMESPACES;
			fFeatures |= NAMESPACEDECLS;
			fFeatures |= SPLITCDATA;
			fFeatures |= WELLFORMED;
			fFeatures |= DISCARDDEFAULT;
			fFeatures |= XMLDECL;

			// New OutputFormat properties
			fDOMConfigProperties = new Properties();

			// Initialize properties to be passed on the underlying serializer
			initializeSerializerProps();

			// Create the underlying serializer.
			Properties configProps = OutputPropertiesFactory.getDefaultMethodProperties("xml");

			// change xml version from 1.0 to 1.1
			//configProps.setProperty("version", "1.1");

			// Get a serializer that seriailizes according the the properties,
			// which in this case is to xml
			fXMLSerializer = SerializerFactory.getSerializer(configProps);

			// Initialize Serializer
			fXMLSerializer.OutputFormat = fDOMConfigProperties;
		}

		/// <summary>
		/// Initializes the underlying serializer's configuration depending on the
		/// default DOMConfiguration parameters. This method must be called before a
		/// node is to be serialized.
		/// 
		/// @xsl.usage internal
		/// </summary>
		public void initializeSerializerProps()
		{
			// canonical-form
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CANONICAL_FORM, DOMConstants.DOM3_DEFAULT_FALSE);

			// cdata-sections
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CDATA_SECTIONS, DOMConstants.DOM3_DEFAULT_TRUE);

			// "check-character-normalization"
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, DOMConstants.DOM3_DEFAULT_FALSE);

			// comments
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_COMMENTS, DOMConstants.DOM3_DEFAULT_TRUE);

			// datatype-normalization
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DATATYPE_NORMALIZATION, DOMConstants.DOM3_DEFAULT_FALSE);

			// element-content-whitespace
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, DOMConstants.DOM3_DEFAULT_TRUE);

			// entities
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_DEFAULT_TRUE);
			// preserve entities
			fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_DEFAULT_TRUE);

			// error-handler
			// Should we set our default ErrorHandler
			/*
			 * if (fDOMConfig.getParameter(Constants.DOM_ERROR_HANDLER) != null) {
			 * fDOMErrorHandler =
			 * (DOMErrorHandler)fDOMConfig.getParameter(Constants.DOM_ERROR_HANDLER); }
			 */

			// infoset
			if ((fFeatures & INFOSET) != 0)
			{
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACES, DOMConstants.DOM3_DEFAULT_TRUE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACE_DECLARATIONS, DOMConstants.DOM3_DEFAULT_TRUE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_COMMENTS, DOMConstants.DOM3_DEFAULT_TRUE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, DOMConstants.DOM3_DEFAULT_TRUE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_WELLFORMED, DOMConstants.DOM3_DEFAULT_TRUE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_DEFAULT_FALSE);
				// preserve entities
				fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_DEFAULT_FALSE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CDATA_SECTIONS, DOMConstants.DOM3_DEFAULT_FALSE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_VALIDATE_IF_SCHEMA, DOMConstants.DOM3_DEFAULT_FALSE);
				fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DATATYPE_NORMALIZATION, DOMConstants.DOM3_DEFAULT_FALSE);
			}

			// namespaces
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACES, DOMConstants.DOM3_DEFAULT_TRUE);

			// namespace-declarations
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACE_DECLARATIONS, DOMConstants.DOM3_DEFAULT_TRUE);

			// normalize-characters
			/*
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS
			        + DOMConstants.DOM_NORMALIZE_CHARACTERS,
			        DOMConstants.DOM3_DEFAULT_FALSE);
			*/

			// split-cdata-sections
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_SPLIT_CDATA, DOMConstants.DOM3_DEFAULT_TRUE);

			// validate
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_VALIDATE, DOMConstants.DOM3_DEFAULT_FALSE);

			// validate-if-schema
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_VALIDATE_IF_SCHEMA, DOMConstants.DOM3_DEFAULT_FALSE);

			// well-formed
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_WELLFORMED, DOMConstants.DOM3_DEFAULT_TRUE);

			// pretty-print
			fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_INDENT, DOMConstants.DOM3_DEFAULT_TRUE);
			fDOMConfigProperties.setProperty(OutputPropertiesFactory.S_KEY_INDENT_AMOUNT, Convert.ToString(3));

			// 

			// discard-default-content
			fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, DOMConstants.DOM3_DEFAULT_TRUE);

			// xml-declaration
			fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL, "no");

		}

		// ************************************************************************
		// DOMConfiguraiton implementation
		// ************************************************************************

		/// <summary>
		/// Checks if setting a parameter to a specific value is supported.    
		/// </summary>
		/// <seealso cref="org.w3c.dom.DOMConfiguration.canSetParameter(java.lang.String, java.lang.Object)"
		/// @since DOM Level 3/>
		/// <param name="name"> A String containing the DOMConfiguration parameter name. </param>
		/// <param name="value"> An Object specifying the value of the corresponding parameter.  </param>
		public bool canSetParameter(string name, object value)
		{
			if (value is Boolean)
			{
				if (name.Equals(DOMConstants.DOM_CDATA_SECTIONS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_COMMENTS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_ENTITIES, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_INFOSET, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_NAMESPACES, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_NAMESPACE_DECLARATIONS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_SPLIT_CDATA, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_WELLFORMED, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_FORMAT_PRETTY_PRINT, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_XMLDECL, StringComparison.OrdinalIgnoreCase))
				{
					// both values supported
					return true;
				}
				else if (name.Equals(DOMConstants.DOM_CANONICAL_FORM, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_DATATYPE_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE_IF_SCHEMA, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE, StringComparison.OrdinalIgnoreCase))
				{
					// true is not supported
					return !((bool?)value).Value;
				}
				else if (name.Equals(DOMConstants.DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS, StringComparison.OrdinalIgnoreCase))
				{
					// false is not supported
					return ((bool?)value).Value;
				}
			}
			else if (name.Equals(DOMConstants.DOM_ERROR_HANDLER, StringComparison.OrdinalIgnoreCase) && value == null || value is DOMErrorHandler)
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// This method returns the value of a parameter if known.
		/// </summary>
		/// <seealso cref="org.w3c.dom.DOMConfiguration.getParameter(java.lang.String)"
		////>
		/// <param name="name"> A String containing the DOMConfiguration parameter name 
		///             whose value is to be returned. </param>
		/// <returns> Object The value of the parameter if known.  </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object getParameter(String name) throws org.w3c.dom.DOMException
		public object getParameter(string name)
		{
			if (name.Equals(DOMConstants.DOM_COMMENTS, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & COMMENTS) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_CDATA_SECTIONS, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & CDATA) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_ENTITIES, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & ENTITIES) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_NAMESPACES, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & NAMESPACES) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_NAMESPACE_DECLARATIONS, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & NAMESPACEDECLS) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_SPLIT_CDATA, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & SPLITCDATA) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_WELLFORMED, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & WELLFORMED) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & DISCARDDEFAULT) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_FORMAT_PRETTY_PRINT, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & PRETTY_PRINT) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_XMLDECL, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & XMLDECL) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & ELEM_CONTENT_WHITESPACE) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_FORMAT_PRETTY_PRINT, StringComparison.OrdinalIgnoreCase))
			{
				return ((fFeatures & PRETTY_PRINT) != 0) ? true : false;
			}
			else if (name.Equals(DOMConstants.DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			else if (name.Equals(DOMConstants.DOM_CANONICAL_FORM, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_DATATYPE_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE_IF_SCHEMA, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			else if (name.Equals(DOMConstants.DOM_INFOSET, StringComparison.OrdinalIgnoreCase))
			{
				if ((fFeatures & ENTITIES) == 0 && (fFeatures & CDATA) == 0 && (fFeatures & ELEM_CONTENT_WHITESPACE) != 0 && (fFeatures & NAMESPACES) != 0 && (fFeatures & NAMESPACEDECLS) != 0 && (fFeatures & WELLFORMED) != 0 && (fFeatures & COMMENTS) != 0)
				{
					return true;
				}
				return false;
			}
			else if (name.Equals(DOMConstants.DOM_ERROR_HANDLER, StringComparison.OrdinalIgnoreCase))
			{
				return fDOMErrorHandler;
			}
			else if (name.Equals(DOMConstants.DOM_SCHEMA_LOCATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_SCHEMA_TYPE, StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			else
			{
				// Here we have to add the Xalan specific DOM Message Formatter
				string msg = Utils.messages.createMessage(MsgKey.ER_FEATURE_NOT_FOUND, new object[] {name});
				throw new DOMException(DOMException.NOT_FOUND_ERR, msg);
			}
		}

		/// <summary>
		/// This method returns a of the parameters supported by this DOMConfiguration object 
		/// and for which at least one value can be set by the application
		/// </summary>
		/// <seealso cref="org.w3c.dom.DOMConfiguration.getParameterNames()"
		////>
		/// <returns> DOMStringList A list of DOMConfiguration parameters recognized
		///                       by the serializer </returns>
		public DOMStringList ParameterNames
		{
			get
			{
				return new DOMStringListImpl(fRecognizedParameters);
			}
		}

		/// <summary>
		/// This method sets the value of the named parameter.
		/// </summary>
		/// <seealso cref="org.w3c.dom.DOMConfiguration.setParameter(java.lang.String, java.lang.Object)"
		////>
		/// <param name="name"> A String containing the DOMConfiguration parameter name. </param>
		/// <param name="value"> An Object contaiing the parameters value to set. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void setParameter(String name, Object value) throws org.w3c.dom.DOMException
		public void setParameter(string name, object value)
		{
			// If the value is a boolean
			if (value is Boolean)
			{
				bool state = ((bool?) value).Value;

				if (name.Equals(DOMConstants.DOM_COMMENTS, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | COMMENTS : fFeatures & ~COMMENTS;
					// comments
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_COMMENTS, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_COMMENTS, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_CDATA_SECTIONS, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | CDATA : fFeatures & ~CDATA;
					// cdata-sections
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CDATA_SECTIONS, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CDATA_SECTIONS, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_ENTITIES, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | ENTITIES : fFeatures & ~ENTITIES;
					// entities
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_EXPLICIT_TRUE);
						fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_EXPLICIT_FALSE);
						fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_NAMESPACES, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | NAMESPACES : fFeatures & ~NAMESPACES;
					// namespaces
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACES, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACES, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_NAMESPACE_DECLARATIONS, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | NAMESPACEDECLS : fFeatures & ~NAMESPACEDECLS;
					// namespace-declarations
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACE_DECLARATIONS, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACE_DECLARATIONS, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_SPLIT_CDATA, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | SPLITCDATA : fFeatures & ~SPLITCDATA;
					// split-cdata-sections
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_SPLIT_CDATA, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_SPLIT_CDATA, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_WELLFORMED, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | WELLFORMED : fFeatures & ~WELLFORMED;
					// well-formed
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_WELLFORMED, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_WELLFORMED, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | DISCARDDEFAULT : fFeatures & ~DISCARDDEFAULT;
					// discard-default-content
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_FORMAT_PRETTY_PRINT, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | PRETTY_PRINT : fFeatures & ~PRETTY_PRINT;
					// format-pretty-print
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_FORMAT_PRETTY_PRINT, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_FORMAT_PRETTY_PRINT, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_XMLDECL, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | XMLDECL : fFeatures & ~XMLDECL;
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL, "no");
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL, "yes");
					}
				}
				else if (name.Equals(DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, StringComparison.OrdinalIgnoreCase))
				{
					fFeatures = state ? fFeatures | ELEM_CONTENT_WHITESPACE : fFeatures & ~ELEM_CONTENT_WHITESPACE;
					// element-content-whitespace
					if (state)
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS, StringComparison.OrdinalIgnoreCase))
				{
					// false is not supported
					if (!state)
					{
						// Here we have to add the Xalan specific DOM Message Formatter
						string msg = Utils.messages.createMessage(MsgKey.ER_FEATURE_NOT_SUPPORTED, new object[] {name});
						throw new DOMException(DOMException.NOT_SUPPORTED_ERR, msg);
					}
					else
					{
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS, DOMConstants.DOM3_EXPLICIT_TRUE);
					}
				}
				else if (name.Equals(DOMConstants.DOM_CANONICAL_FORM, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE_IF_SCHEMA, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_DATATYPE_NORMALIZATION, StringComparison.OrdinalIgnoreCase))
				{
					// true is not supported
					if (state)
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_FEATURE_NOT_SUPPORTED, new object[] {name});
						throw new DOMException(DOMException.NOT_SUPPORTED_ERR, msg);
					}
					else
					{
						if (name.Equals(DOMConstants.DOM_CANONICAL_FORM, StringComparison.OrdinalIgnoreCase))
						{
							fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CANONICAL_FORM, DOMConstants.DOM3_EXPLICIT_FALSE);
						}
						else if (name.Equals(DOMConstants.DOM_VALIDATE_IF_SCHEMA, StringComparison.OrdinalIgnoreCase))
						{
							fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_VALIDATE_IF_SCHEMA, DOMConstants.DOM3_EXPLICIT_FALSE);
						}
						else if (name.Equals(DOMConstants.DOM_VALIDATE, StringComparison.OrdinalIgnoreCase))
						{
							fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_VALIDATE, DOMConstants.DOM3_EXPLICIT_FALSE);
						}
						else if (name.Equals(DOMConstants.DOM_VALIDATE_IF_SCHEMA, StringComparison.OrdinalIgnoreCase))
						{
							fDOMConfigProperties.setProperty(DOMConstants.DOM_CHECK_CHAR_NORMALIZATION + DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, DOMConstants.DOM3_EXPLICIT_FALSE);
						}
						else if (name.Equals(DOMConstants.DOM_DATATYPE_NORMALIZATION, StringComparison.OrdinalIgnoreCase))
						{
							fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DATATYPE_NORMALIZATION, DOMConstants.DOM3_EXPLICIT_FALSE);
						} /* else if (name.equalsIgnoreCase(DOMConstants.DOM_NORMALIZE_CHARACTERS)) {
	                        fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS 
	                                + DOMConstants.DOM_NORMALIZE_CHARACTERS, DOMConstants.DOM3_EXPLICIT_FALSE);
	                    } */
					}
				}
				else if (name.Equals(DOMConstants.DOM_INFOSET, StringComparison.OrdinalIgnoreCase))
				{
					// infoset
					if (state)
					{
						fFeatures &= ~ENTITIES;
						fFeatures &= ~CDATA;
						fFeatures &= ~SCHEMAVALIDATE;
						fFeatures &= ~DTNORMALIZE;
						fFeatures |= NAMESPACES;
						fFeatures |= NAMESPACEDECLS;
						fFeatures |= WELLFORMED;
						fFeatures |= ELEM_CONTENT_WHITESPACE;
						fFeatures |= COMMENTS;

						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACES, DOMConstants.DOM3_EXPLICIT_TRUE);
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_NAMESPACE_DECLARATIONS, DOMConstants.DOM3_EXPLICIT_TRUE);
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_COMMENTS, DOMConstants.DOM3_EXPLICIT_TRUE);
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, DOMConstants.DOM3_EXPLICIT_TRUE);
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_WELLFORMED, DOMConstants.DOM3_EXPLICIT_TRUE);

						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_EXPLICIT_FALSE);
						fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.DOM_ENTITIES, DOMConstants.DOM3_EXPLICIT_FALSE);

						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_CDATA_SECTIONS, DOMConstants.DOM3_EXPLICIT_FALSE);
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_VALIDATE_IF_SCHEMA, DOMConstants.DOM3_EXPLICIT_FALSE);
						fDOMConfigProperties.setProperty(DOMConstants.S_DOM3_PROPERTIES_NS + DOMConstants.DOM_DATATYPE_NORMALIZATION, DOMConstants.DOM3_EXPLICIT_FALSE);
					}
				}
				else
				{
					// If this is a non-boolean parameter a type mismatch should be thrown.
					if (name.Equals(DOMConstants.DOM_ERROR_HANDLER, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_SCHEMA_LOCATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_SCHEMA_TYPE, StringComparison.OrdinalIgnoreCase))
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_TYPE_MISMATCH_ERR, new object[] {name});
						throw new DOMException(DOMException.TYPE_MISMATCH_ERR, msg);
					}

					// Parameter is not recognized
					string msg = Utils.messages.createMessage(MsgKey.ER_FEATURE_NOT_FOUND, new object[] {name});
					throw new DOMException(DOMException.NOT_FOUND_ERR, msg);
				}
			} // If the parameter value is not a boolean
			else if (name.Equals(DOMConstants.DOM_ERROR_HANDLER, StringComparison.OrdinalIgnoreCase))
			{
				if (value == null || value is DOMErrorHandler)
				{
					fDOMErrorHandler = (DOMErrorHandler)value;
				}
				else
				{
					string msg = Utils.messages.createMessage(MsgKey.ER_TYPE_MISMATCH_ERR, new object[] {name});
					throw new DOMException(DOMException.TYPE_MISMATCH_ERR, msg);
				}
			}
			else if (name.Equals(DOMConstants.DOM_SCHEMA_LOCATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_SCHEMA_TYPE, StringComparison.OrdinalIgnoreCase))
			{
				if (value != null)
				{
					if (!(value is string))
					{
						string msg = Utils.messages.createMessage(MsgKey.ER_TYPE_MISMATCH_ERR, new object[] {name});
						throw new DOMException(DOMException.TYPE_MISMATCH_ERR, msg);
					}
					string msg = Utils.messages.createMessage(MsgKey.ER_FEATURE_NOT_SUPPORTED, new object[] {name});
					throw new DOMException(DOMException.NOT_SUPPORTED_ERR, msg);
				}
			}
			else
			{
				// If this is a boolean parameter a type mismatch should be thrown.
				if (name.Equals(DOMConstants.DOM_COMMENTS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_CDATA_SECTIONS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_ENTITIES, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_NAMESPACES, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_NAMESPACE_DECLARATIONS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_SPLIT_CDATA, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_WELLFORMED, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_DISCARD_DEFAULT_CONTENT, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_FORMAT_PRETTY_PRINT, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_XMLDECL, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_ELEMENT_CONTENT_WHITESPACE, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_IGNORE_UNKNOWN_CHARACTER_DENORMALIZATIONS, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_CANONICAL_FORM, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE_IF_SCHEMA, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_VALIDATE, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_CHECK_CHAR_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_DATATYPE_NORMALIZATION, StringComparison.OrdinalIgnoreCase) || name.Equals(DOMConstants.DOM_INFOSET, StringComparison.OrdinalIgnoreCase))
				{
					string msg = Utils.messages.createMessage(MsgKey.ER_TYPE_MISMATCH_ERR, new object[] {name});
					throw new DOMException(DOMException.TYPE_MISMATCH_ERR, msg);
				}

				// Parameter is not recognized
				string msg = Utils.messages.createMessage(MsgKey.ER_FEATURE_NOT_FOUND, new object[] {name});
				throw new DOMException(DOMException.NOT_FOUND_ERR, msg);
			}
		}
		// ************************************************************************


		// ************************************************************************
		// DOMConfiguraiton implementation
		// ************************************************************************

		/// <summary>
		/// Returns the DOMConfiguration of the LSSerializer.
		/// </summary>
		/// <seealso cref="org.w3c.dom.ls.LSSerializer.getDomConfig()"
		/// @since DOM Level 3/>
		/// <returns> A DOMConfiguration object. </returns>
		public DOMConfiguration DomConfig
		{
			get
			{
				return (DOMConfiguration)this;
			}
		}

		/// <summary>
		/// Returns the DOMConfiguration of the LSSerializer.
		/// </summary>
		/// <seealso cref="org.w3c.dom.ls.LSSerializer.getFilter()"
		/// @since DOM Level 3/>
		/// <returns> A LSSerializerFilter object. </returns>
		public LSSerializerFilter Filter
		{
			get
			{
				return fSerializerFilter;
			}
			set
			{
				fSerializerFilter = value;
			}
		}

		/// <summary>
		/// Returns the End-Of-Line sequence of characters to be used in the XML 
		/// being serialized.  If none is set a default "\n" is returned.
		/// </summary>
		/// <seealso cref="org.w3c.dom.ls.LSSerializer.getNewLine()"
		/// @since DOM Level 3/>
		/// <returns> A String containing the end-of-line character sequence  used in 
		/// serialization. </returns>
		public string NewLine
		{
			get
			{
				return fEndOfLine;
			}
			set
			{
				fEndOfLine = (!string.ReferenceEquals(value, null)) ? value : DEFAULT_END_OF_LINE;
			}
		}



		/// <summary>
		/// Serializes the specified node to the specified LSOutput and returns true if the Node 
		/// was successfully serialized. 
		/// </summary>
		/// <seealso cref="org.w3c.dom.ls.LSSerializer.write(org.w3c.dom.Node, org.w3c.dom.ls.LSOutput)"
		/// @since DOM Level 3/>
		/// <param name="nodeArg"> The Node to serialize. </param>
		/// <exception cref="org.w3c.dom.ls.LSException"> SERIALIZE_ERR: Raised if the 
		/// LSSerializer was unable to serialize the node.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean write(org.w3c.dom.Node nodeArg, org.w3c.dom.ls.LSOutput destination) throws org.w3c.dom.ls.LSException
		public bool write(Node nodeArg, LSOutput destination)
		{
			// If the destination is null
			if (destination == null)
			{
				string msg = Utils.messages.createMessage(MsgKey.ER_NO_OUTPUT_SPECIFIED, null);
				if (fDOMErrorHandler != null)
				{
					fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_NO_OUTPUT_SPECIFIED));
				}
				throw new LSException(LSException.SERIALIZE_ERR, msg);
			}

			// If nodeArg is null, return false.  Should we throw and LSException instead?
			if (nodeArg == null)
			{
				return false;
			}

			// Obtain a reference to the serializer to use
			// Serializer serializer = getXMLSerializer(xmlVersion);
			Serializer serializer = fXMLSerializer;
			serializer.reset();

			// If the node has not been seen
			if (nodeArg != fVisitedNode)
			{
				// Determine the XML Document version of the Node 
				string xmlVersion = getXMLVersion(nodeArg);

				// Determine the encoding: 1.LSOutput.encoding, 2.Document.inputEncoding, 3.Document.xmlEncoding. 
				fEncoding = destination.getEncoding();
				if (string.ReferenceEquals(fEncoding, null))
				{
					fEncoding = getInputEncoding(nodeArg);
					fEncoding = !string.ReferenceEquals(fEncoding, null) ? fEncoding : string.ReferenceEquals(getXMLEncoding(nodeArg), null)? "UTF-8": getXMLEncoding(nodeArg);
				}

				// If the encoding is not recognized throw an exception.
				// Note: The serializer defaults to UTF-8 when created
				if (!Encodings.isRecognizedEncoding(fEncoding))
				{
					string msg = Utils.messages.createMessage(MsgKey.ER_UNSUPPORTED_ENCODING, null);
					if (fDOMErrorHandler != null)
					{
						fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_UNSUPPORTED_ENCODING));
					}
					throw new LSException(LSException.SERIALIZE_ERR, msg);
				}

				serializer.OutputFormat.setProperty("version", xmlVersion);

				// Set the output encoding and xml version properties
				fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.S_XML_VERSION, xmlVersion);
				fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_ENCODING, fEncoding);

				// If the node to be serialized is not a Document, Element, or Entity
				// node
				// then the XML declaration, or text declaration, should be never be
				// serialized.
				if ((nodeArg.getNodeType() != Node.DOCUMENT_NODE || nodeArg.getNodeType() != Node.ELEMENT_NODE || nodeArg.getNodeType() != Node.ENTITY_NODE) && ((fFeatures & XMLDECL) != 0))
				{
					fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL, DOMConstants.DOM3_DEFAULT_FALSE);
				}

				fVisitedNode = nodeArg;
			}

			// Update the serializer properties
			fXMLSerializer.OutputFormat = fDOMConfigProperties;

			// 
			try
			{

				// The LSSerializer will use the LSOutput object to determine 
				// where to serialize the output to in the following order the  
				// first one that is not null and not an empty string will be    
				// used: 1.LSOutput.characterStream, 2.LSOutput.byteStream,   
				// 3. LSOutput.systemId 
				// 1.LSOutput.characterStream
				Writer writer = destination.getCharacterStream();
				if (writer == null)
				{

					// 2.LSOutput.byteStream
					Stream outputStream = destination.getByteStream();
					if (outputStream == null)
					{

						// 3. LSOutput.systemId
						string uri = destination.getSystemId();
						if (string.ReferenceEquals(uri, null))
						{
							string msg = Utils.messages.createMessage(MsgKey.ER_NO_OUTPUT_SPECIFIED, null);
							if (fDOMErrorHandler != null)
							{
								fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_NO_OUTPUT_SPECIFIED));
							}
							throw new LSException(LSException.SERIALIZE_ERR, msg);

						}
						else
						{
							// Expand the System Id and obtain an absolute URI for it.
							string absoluteURI = SystemIDResolver.getAbsoluteURI(uri);

							URL url = new URL(absoluteURI);
							Stream urlOutStream = null;
							string protocol = url.getProtocol();
							string host = url.getHost();

							// For file protocols, there is no need to use a URL to get its
							// corresponding OutputStream

							// Scheme names consist of a sequence of characters. The lower case
							// letters "a"--"z", digits, and the characters plus ("+"), period
							// ("."), and hyphen ("-") are allowed. For resiliency, programs
							// interpreting URLs should treat upper case letters as equivalent to
							// lower case in scheme names (e.g., allow "HTTP" as well as "http").
							if (protocol.Equals("file", StringComparison.OrdinalIgnoreCase) && (string.ReferenceEquals(host, null) || host.Length == 0 || host.Equals("localhost")))
							{
								// do we also need to check for host.equals(hostname)
								urlOutStream = new FileStream(getPathWithoutEscapes(url.getPath()), FileMode.Create, FileAccess.Write);

							}
							else
							{
								// This should support URL's whose schemes are mentioned in 
								// RFC1738 other than file

								URLConnection urlCon = url.openConnection();
								urlCon.setDoInput(false);
								urlCon.setDoOutput(true);
								urlCon.setUseCaches(false);
								urlCon.setAllowUserInteraction(false);

								// When writing to a HTTP URI, a HTTP PUT is performed.
								if (urlCon is HttpURLConnection)
								{
									HttpURLConnection httpCon = (HttpURLConnection) urlCon;
									httpCon.setRequestMethod("PUT");
								}
								urlOutStream = urlCon.getOutputStream();
							}
							// set the OutputStream to that obtained from the systemId
							serializer.OutputStream = urlOutStream;
						}
					}
					else
					{
						// 2.LSOutput.byteStream
						serializer.OutputStream = outputStream;
					}
				}
				else
				{
					// 1.LSOutput.characterStream
					serializer.Writer = writer;
				}

				// The associated media type by default is set to text/xml on 
				// org.apache.xml.serializer.SerializerBase.  

				// Get a reference to the serializer then lets you serilize a DOM
				// Use this hack till Xalan support JAXP1.3
				if (fDOMSerializer == null)
				{
				   fDOMSerializer = (DOM3Serializer)serializer.asDOM3Serializer();
				}

				// Set the error handler on the DOM3Serializer interface implementation
				if (fDOMErrorHandler != null)
				{
					fDOMSerializer.ErrorHandler = fDOMErrorHandler;
				}

				// Set the filter on the DOM3Serializer interface implementation
				if (fSerializerFilter != null)
				{
					fDOMSerializer.NodeFilter = fSerializerFilter;
				}

				// Set the NewLine character to be used
				fDOMSerializer.NewLine = fEndOfLine.ToCharArray();

				// Serializer your DOM, where node is an org.w3c.dom.Node
				// Assuming that Xalan's serializer can serialize any type of DOM node
				fDOMSerializer.serializeDOM3(nodeArg);

			}
			catch (UnsupportedEncodingException ue)
			{

				string msg = Utils.messages.createMessage(MsgKey.ER_UNSUPPORTED_ENCODING, null);
				if (fDOMErrorHandler != null)
				{
					fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_UNSUPPORTED_ENCODING, ue));
				}
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, ue).fillInStackTrace();
			}
			catch (LSException lse)
			{
				// Rethrow LSException.
				throw lse;
			}
			catch (Exception e)
			{
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, e).fillInStackTrace();
			}
			catch (Exception e)
			{
				if (fDOMErrorHandler != null)
				{
					fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, e.Message, null, e));
				}
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, e).fillInStackTrace();
			}
			return true;
		}

		/// <summary>
		/// Serializes the specified node and returns a String with the serialized
		/// data to the caller.  
		/// </summary>
		/// <seealso cref="org.w3c.dom.ls.LSSerializer.writeToString(org.w3c.dom.Node)"
		/// @since DOM Level 3/>
		/// <param name="nodeArg"> The Node to serialize. </param>
		/// <exception cref="org.w3c.dom.ls.LSException"> SERIALIZE_ERR: Raised if the 
		/// LSSerializer was unable to serialize the node.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String writeToString(org.w3c.dom.Node nodeArg) throws DOMException, org.w3c.dom.ls.LSException
		public string writeToString(Node nodeArg)
		{
			// return null is nodeArg is null.  Should an Exception be thrown instead?
			if (nodeArg == null)
			{
				return null;
			}

			// Should we reset the serializer configuration before each write operation?
			// Obtain a reference to the serializer to use
			Serializer serializer = fXMLSerializer;
			serializer.reset();

			if (nodeArg != fVisitedNode)
			{
				// Determine the XML Document version of the Node 
				string xmlVersion = getXMLVersion(nodeArg);

				serializer.OutputFormat.setProperty("version", xmlVersion);

				// Set the output encoding and xml version properties
				fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.S_XML_VERSION, xmlVersion);
				fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_ENCODING, "UTF-16");

				// If the node to be serialized is not a Document, Element, or Entity
				// node
				// then the XML declaration, or text declaration, should be never be
				// serialized.
				if ((nodeArg.getNodeType() != Node.DOCUMENT_NODE || nodeArg.getNodeType() != Node.ELEMENT_NODE || nodeArg.getNodeType() != Node.ENTITY_NODE) && ((fFeatures & XMLDECL) != 0))
				{
					fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL, DOMConstants.DOM3_DEFAULT_FALSE);
				}

				fVisitedNode = nodeArg;
			}
			// Update the serializer properties
			fXMLSerializer.OutputFormat = fDOMConfigProperties;

			// StringWriter to Output to
			StringWriter output = new StringWriter();

			// 
			try
			{

				// Set the Serializer's Writer to a StringWriter
				serializer.Writer = output;

				// Get a reference to the serializer then lets you serilize a DOM
				// Use this hack till Xalan support JAXP1.3
				if (fDOMSerializer == null)
				{
					fDOMSerializer = (DOM3Serializer)serializer.asDOM3Serializer();
				}

				// Set the error handler on the DOM3Serializer interface implementation
				if (fDOMErrorHandler != null)
				{
					fDOMSerializer.ErrorHandler = fDOMErrorHandler;
				}

				// Set the filter on the DOM3Serializer interface implementation
				if (fSerializerFilter != null)
				{
					fDOMSerializer.NodeFilter = fSerializerFilter;
				}

				// Set the NewLine character to be used
				fDOMSerializer.NewLine = fEndOfLine.ToCharArray();

				// Serializer your DOM, where node is an org.w3c.dom.Node
				fDOMSerializer.serializeDOM3(nodeArg);
			}
			catch (LSException lse)
			{
				// Rethrow LSException.
				throw lse;
			}
			catch (Exception e)
			{
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, e).fillInStackTrace();
			}
			catch (Exception e)
			{
				if (fDOMErrorHandler != null)
				{
					fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, e.Message, null, e));
				}
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, e).fillInStackTrace();
			}

			// return the serialized string
			return output.ToString();
		}

		/// <summary>
		/// Serializes the specified node to the specified URI and returns true if the Node 
		/// was successfully serialized. 
		/// </summary>
		/// <seealso cref="org.w3c.dom.ls.LSSerializer.writeToURI(org.w3c.dom.Node, String)"
		/// @since DOM Level 3/>
		/// <param name="nodeArg"> The Node to serialize. </param>
		/// <exception cref="org.w3c.dom.ls.LSException"> SERIALIZE_ERR: Raised if the 
		/// LSSerializer was unable to serialize the node.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean writeToURI(org.w3c.dom.Node nodeArg, String uri) throws org.w3c.dom.ls.LSException
		public bool writeToURI(Node nodeArg, string uri)
		{
			// If nodeArg is null, return false.  Should we throw and LSException instead?
			if (nodeArg == null)
			{
				return false;
			}

			// Obtain a reference to the serializer to use
			Serializer serializer = fXMLSerializer;
			serializer.reset();

			if (nodeArg != fVisitedNode)
			{
				// Determine the XML Document version of the Node 
				string xmlVersion = getXMLVersion(nodeArg);

				// Determine the encoding: 1.LSOutput.encoding,
				// 2.Document.inputEncoding, 3.Document.xmlEncoding.
				fEncoding = getInputEncoding(nodeArg);
				if (string.ReferenceEquals(fEncoding, null))
				{
					fEncoding = !string.ReferenceEquals(fEncoding, null) ? fEncoding : string.ReferenceEquals(getXMLEncoding(nodeArg), null)? "UTF-8": getXMLEncoding(nodeArg);
				}

				serializer.OutputFormat.setProperty("version", xmlVersion);

				// Set the output encoding and xml version properties
				fDOMConfigProperties.setProperty(DOMConstants.S_XERCES_PROPERTIES_NS + DOMConstants.S_XML_VERSION, xmlVersion);
				fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_ENCODING, fEncoding);

				// If the node to be serialized is not a Document, Element, or Entity
				// node
				// then the XML declaration, or text declaration, should be never be
				// serialized.
				if ((nodeArg.getNodeType() != Node.DOCUMENT_NODE || nodeArg.getNodeType() != Node.ELEMENT_NODE || nodeArg.getNodeType() != Node.ENTITY_NODE) && ((fFeatures & XMLDECL) != 0))
				{
					fDOMConfigProperties.setProperty(DOMConstants.S_XSL_OUTPUT_OMIT_XML_DECL, DOMConstants.DOM3_DEFAULT_FALSE);
				}

				fVisitedNode = nodeArg;
			}

			// Update the serializer properties
			fXMLSerializer.OutputFormat = fDOMConfigProperties;

			// 
			try
			{
				// If the specified encoding is not supported an
				// "unsupported-encoding" fatal error is raised. ??
				if (string.ReferenceEquals(uri, null))
				{
					string msg = Utils.messages.createMessage(MsgKey.ER_NO_OUTPUT_SPECIFIED, null);
					if (fDOMErrorHandler != null)
					{
						fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, msg, MsgKey.ER_NO_OUTPUT_SPECIFIED));
					}
					throw new LSException(LSException.SERIALIZE_ERR, msg);

				}
				else
				{
					// REVISIT: Can this be used to get an absolute expanded URI
					string absoluteURI = SystemIDResolver.getAbsoluteURI(uri);

					URL url = new URL(absoluteURI);
					Stream urlOutStream = null;
					string protocol = url.getProtocol();
					string host = url.getHost();

					// For file protocols, there is no need to use a URL to get its
					// corresponding OutputStream

					// Scheme names consist of a sequence of characters. The lower 
					// case letters "a"--"z", digits, and the characters plus ("+"), 
					// period ("."), and hyphen ("-") are allowed. For resiliency, 
					// programs interpreting URLs should treat upper case letters as
					// equivalent to lower case in scheme names 
					// (e.g., allow "HTTP" as well as "http").
					if (protocol.Equals("file", StringComparison.OrdinalIgnoreCase) && (string.ReferenceEquals(host, null) || host.Length == 0 || host.Equals("localhost")))
					{
						// do we also need to check for host.equals(hostname)
						urlOutStream = new FileStream(getPathWithoutEscapes(url.getPath()), FileMode.Create, FileAccess.Write);

					}
					else
					{
						// This should support URL's whose schemes are mentioned in
						// RFC1738 other than file

						URLConnection urlCon = url.openConnection();
						urlCon.setDoInput(false);
						urlCon.setDoOutput(true);
						urlCon.setUseCaches(false);
						urlCon.setAllowUserInteraction(false);

						// When writing to a HTTP URI, a HTTP PUT is performed.
						if (urlCon is HttpURLConnection)
						{
							HttpURLConnection httpCon = (HttpURLConnection) urlCon;
							httpCon.setRequestMethod("PUT");
						}
						urlOutStream = urlCon.getOutputStream();
					}
					// set the OutputStream to that obtained from the systemId
					serializer.OutputStream = urlOutStream;
				}

				// Get a reference to the serializer then lets you serilize a DOM
				// Use this hack till Xalan support JAXP1.3
				if (fDOMSerializer == null)
				{
					fDOMSerializer = (DOM3Serializer)serializer.asDOM3Serializer();
				}

				// Set the error handler on the DOM3Serializer interface implementation
				if (fDOMErrorHandler != null)
				{
					fDOMSerializer.ErrorHandler = fDOMErrorHandler;
				}

				// Set the filter on the DOM3Serializer interface implementation
				if (fSerializerFilter != null)
				{
					fDOMSerializer.NodeFilter = fSerializerFilter;
				}

				// Set the NewLine character to be used
				fDOMSerializer.NewLine = fEndOfLine.ToCharArray();

				// Serializer your DOM, where node is an org.w3c.dom.Node
				// Assuming that Xalan's serializer can serialize any type of DOM
				// node
				fDOMSerializer.serializeDOM3(nodeArg);

			}
			catch (LSException lse)
			{
				// Rethrow LSException.
				throw lse;
			}
			catch (Exception e)
			{
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, e).fillInStackTrace();
			}
			catch (Exception e)
			{
				if (fDOMErrorHandler != null)
				{
					fDOMErrorHandler.handleError(new DOMErrorImpl(DOMError.SEVERITY_FATAL_ERROR, e.Message, null, e));
				}
				throw (LSException) createLSException(LSException.SERIALIZE_ERR, e).fillInStackTrace();
			}

			return true;
		}
		// ************************************************************************


		// ************************************************************************
		// Implementaion methods
		// ************************************************************************

		/// <summary>
		/// Determines the XML Version of the Document Node to serialize.  If the Document Node
		/// is not a DOM Level 3 Node, then the default version returned is 1.0.
		/// </summary>
		/// <param name="nodeArg"> The Node to serialize </param>
		/// <returns> A String containing the version pseudo-attribute of the XMLDecl. </returns>
		/// <exception cref="Throwable"> if the DOM implementation does not implement Document.getXmlVersion()       </exception>
		//protected String getXMLVersion(Node nodeArg) throws Throwable {
		protected internal string getXMLVersion(Node nodeArg)
		{
			Document doc = null;

			// Determine the XML Version of the document
			if (nodeArg != null)
			{
				if (nodeArg.getNodeType() == Node.DOCUMENT_NODE)
				{
					// The Document node is the Node argument
					doc = (Document)nodeArg;
				}
				else
				{
					// The Document node is the Node argument's ownerDocument
					doc = nodeArg.getOwnerDocument();
				}

				// Determine the DOM Version.
				if (doc != null && doc.getImplementation().hasFeature("Core","3.0"))
				{
					return doc.getXmlVersion();
				}
			}
			// The version will be treated as "1.0" which may result in
			// an ill-formed document being serialized.
			// If nodeArg does not have an ownerDocument, treat this as XML 1.0
			return "1.0";
		}

		/// <summary>
		/// Determines the XML Encoding of the Document Node to serialize.  If the Document Node
		/// is not a DOM Level 3 Node, then the default encoding "UTF-8" is returned.
		/// </summary>
		/// <param name="nodeArg"> The Node to serialize </param>
		/// <returns> A String containing the encoding pseudo-attribute of the XMLDecl. </returns>
		/// <exception cref="Throwable"> if the DOM implementation does not implement Document.getXmlEncoding()      </exception>
		protected internal string getXMLEncoding(Node nodeArg)
		{
			Document doc = null;

			// Determine the XML Encoding of the document
			if (nodeArg != null)
			{
				if (nodeArg.getNodeType() == Node.DOCUMENT_NODE)
				{
					// The Document node is the Node argument
					doc = (Document)nodeArg;
				}
				else
				{
					// The Document node is the Node argument's ownerDocument
					doc = nodeArg.getOwnerDocument();
				}

				// Determine the XML Version. 
				if (doc != null && doc.getImplementation().hasFeature("Core","3.0"))
				{
					return doc.getXmlEncoding();
				}
			}
			// The default encoding is UTF-8 except for the writeToString method
			return "UTF-8";
		}

		/// <summary>
		/// Determines the Input Encoding of the Document Node to serialize.  If the Document Node
		/// is not a DOM Level 3 Node, then null is returned.
		/// </summary>
		/// <param name="nodeArg"> The Node to serialize </param>
		/// <returns> A String containing the input encoding.   </returns>
		protected internal string getInputEncoding(Node nodeArg)
		{
			Document doc = null;

			// Determine the Input Encoding of the document
			if (nodeArg != null)
			{
				if (nodeArg.getNodeType() == Node.DOCUMENT_NODE)
				{
					// The Document node is the Node argument
					doc = (Document)nodeArg;
				}
				else
				{
					// The Document node is the Node argument's ownerDocument
					doc = nodeArg.getOwnerDocument();
				}

				// Determine the DOM Version.
				if (doc != null && doc.getImplementation().hasFeature("Core","3.0"))
				{
					return doc.getInputEncoding();
				}
			}
			// The default encoding returned is null
			return null;
		}

		/// <summary>
		/// This method returns the LSSerializer's error handler.
		/// </summary>
		/// <returns> Returns the fDOMErrorHandler. </returns>
		public DOMErrorHandler ErrorHandler
		{
			get
			{
				return fDOMErrorHandler;
			}
		}

		/// <summary>
		/// Replaces all escape sequences in the given path with their literal characters.
		/// </summary>
		private static string getPathWithoutEscapes(string origPath)
		{
			if (!string.ReferenceEquals(origPath, null) && origPath.Length != 0 && origPath.IndexOf('%') != -1)
			{
				// Locate the escape characters
				StringTokenizer tokenizer = new StringTokenizer(origPath, "%");
				StringBuilder result = new StringBuilder(origPath.Length);
				int size = tokenizer.countTokens();
				result.Append(tokenizer.nextToken());
				for (int i = 1; i < size; ++i)
				{
					string token = tokenizer.nextToken();
					if (token.Length >= 2 && isHexDigit(token[0]) && isHexDigit(token[1]))
					{
						// Decode the 2 digit hexadecimal number following % in '%nn'
						result.Append((char)Convert.ToInt32(token.Substring(0, 2), 16));
						token = token.Substring(2);
					}
					result.Append(token);
				}
				return result.ToString();
			}
			return origPath;
		}

		/// <summary>
		/// Returns true if the given character is a valid hex character.
		/// </summary>
		private static bool isHexDigit(char c)
		{
			return (c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F');
		}

		/// <summary>
		/// Creates an LSException. On J2SE 1.4 and above the cause for the exception will be set.
		/// </summary>
		private static LSException createLSException(short code, Exception cause)
		{
			LSException lse = new LSException(code, cause != null ? cause.Message : null);
			if (cause != null && ThrowableMethods.fgThrowableMethodsAvailable)
			{
				try
				{
					ThrowableMethods.fgThrowableInitCauseMethod.invoke(lse, new object [] {cause});
				}
				// Something went wrong. There's not much we can do about it.
				catch (Exception)
				{
				}
			}
			return lse;
		}

		/// <summary>
		/// Holder of methods from java.lang.Throwable.
		/// </summary>
		internal class ThrowableMethods
		{

			// Method: java.lang.Throwable.initCause(java.lang.Throwable)
			internal static System.Reflection.MethodInfo fgThrowableInitCauseMethod = null;

			// Flag indicating whether or not Throwable methods available.
			internal static bool fgThrowableMethodsAvailable = false;

			internal ThrowableMethods()
			{
			}

			// Attempt to get methods for java.lang.Throwable on class initialization.
		}
	}

}