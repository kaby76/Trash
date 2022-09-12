using System.Collections;

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
 * $Id: XSLOutputAttributes.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	/// <summary>
	/// This interface has methods associated with the XSLT xsl:output attribues
	/// specified in the stylesheet that effect the format of the document output.
	/// 
	/// In an XSLT stylesheet these attributes appear for example as:
	/// <pre>
	/// <xsl:output method="xml" omit-xml-declaration="no" indent="yes"/> 
	/// </pre>
	/// The xsl:output attributes covered in this interface are:
	/// <pre>
	/// version
	/// encoding
	/// omit-xml-declarations
	/// standalone
	/// doctype-public
	/// doctype-system
	/// cdata-section-elements
	/// indent
	/// media-type
	/// </pre>
	/// 
	/// The one attribute not covered in this interface is <code>method</code> as
	/// this value is implicitly chosen by the serializer that is created, for
	/// example ToXMLStream vs. ToHTMLStream or another one.
	/// 
	/// This interface is only used internally within Xalan.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal interface XSLOutputAttributes
	{
		/// <summary>
		/// Returns the previously set value of the value to be used as the public
		/// identifier in the document type declaration (DTD).
		/// </summary>
		/// <returns> the public identifier to be used in the DOCTYPE declaration in the
		/// output document. </returns>
		string DoctypePublic {get;set;}
		/// <summary>
		/// Returns the previously set value of the value to be used
		/// as the system identifier in the document type declaration (DTD). </summary>
		/// <returns> the system identifier to be used in the DOCTYPE declaration in
		/// the output document.
		///  </returns>
		string DoctypeSystem {get;set;}
		/// <returns> the character encoding to be used in the output document. </returns>
		string Encoding {get;set;}
		/// <returns> true if the output document should be indented to visually
		/// indicate its structure. </returns>
		bool Indent {get;set;}

		/// <returns> the number of spaces to indent for each indentation level. </returns>
		int IndentAmount {get;}
		/// <returns> the mediatype the media-type or MIME type associated with the
		/// output document. </returns>
		string MediaType {get;set;}
		/// <returns> true if the XML declaration is to be omitted from the output
		/// document. </returns>
		bool OmitXMLDeclaration {get;set;}
		/// <returns> a value of "yes" if the <code>standalone</code> delaration is to
		/// be included in the output document. </returns>
		string Standalone {get;set;}
		/// <returns> the version of the output format. </returns>
		string Version {get;set;}






		/// <summary>
		/// Sets the value coming from the xsl:output cdata-section-elements
		/// stylesheet property.
		/// 
		/// This sets the elements whose text elements are to be output as CDATA
		/// sections. </summary>
		/// <param name="URI_and_localNames"> pairs of namespace URI and local names that
		/// identify elements whose text elements are to be output as CDATA sections.
		/// The namespace of the local element must be the given URI to match. The
		/// qName is not given because the prefix does not matter, only the namespace
		/// URI to which that prefix would map matters, so the prefix itself is not
		/// relevant in specifying which elements have their text to be output as
		/// CDATA sections. </param>
		ArrayList CdataSectionElements {set;}

		/// <summary>
		/// Set the value coming from the xsl:output doctype-public and doctype-system stylesheet properties </summary>
		/// <param name="system"> the system identifier to be used in the DOCTYPE declaration
		/// in the output document. </param>
		/// <param name="pub"> the public identifier to be used in the DOCTYPE declaration in
		/// the output document. </param>
		void setDoctype(string system, string pub);


		/// <summary>
		/// Get the value for a property that affects seraialization,
		/// if a property was set return that value, otherwise return
		/// the default value, otherwise return null. </summary>
		/// <param name="name"> The name of the property, which is just the local name
		/// if it is in no namespace, but is the URI in curly braces followed by
		/// the local name if it is in a namespace, for example:
		/// <ul>
		/// <li> "encoding"
		/// <li> "method"
		/// <li> "{http://xml.apache.org/xalan}indent-amount"
		/// <li> "{http://xml.apache.org/xalan}line-separator"
		/// </ul> </param>
		/// <returns> The value of the parameter </returns>
		string getOutputProperty(string name);
		/// <summary>
		/// Get the default value for a property that affects seraialization,
		/// or null if there is none. It is possible that a non-default value
		/// was set for the property, however the value returned by this method
		/// is unaffected by any non-default settings. </summary>
		/// <param name="name"> The name of the property. </param>
		/// <returns> The default value of the parameter, or null if there is no default value. </returns>
		string getOutputPropertyDefault(string name);
		/// <summary>
		/// Set the non-default value for a property that affects seraialization. </summary>
		/// <param name="name"> The name of the property, which is just the local name
		/// if it is in no namespace, but is the URI in curly braces followed by
		/// the local name if it is in a namespace, for example:
		/// <ul>
		/// <li> "encoding"
		/// <li> "method"
		/// <li> "{http://xml.apache.org/xalan}indent-amount"
		/// <li> "{http://xml.apache.org/xalan}line-separator"
		/// </ul>
		/// @val The non-default value of the parameter </param>
		void setOutputProperty(string name, string val);

		/// <summary>
		/// Set the default value for a property that affects seraialization. </summary>
		/// <param name="name"> The name of the property, which is just the local name
		/// if it is in no namespace, but is the URI in curly braces followed by
		/// the local name if it is in a namespace, for example:
		/// <ul>
		/// <li> "encoding"
		/// <li> "method"
		/// <li> "{http://xml.apache.org/xalan}indent-amount"
		/// <li> "{http://xml.apache.org/xalan}line-separator"
		/// </ul>
		/// @val The default value of the parameter </param>
		void setOutputPropertyDefault(string name, string val);
	}

}