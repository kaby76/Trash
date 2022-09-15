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
 * $Id: ExtendedContentHandler.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This interface describes extensions to the SAX ContentHandler interface.
	/// It is intended to be used by a serializer. The methods on this interface will
	/// implement SAX- like behavior. This allows the gradual collection of
	/// information rather than having it all up front. For example the call
	/// <pre>
	/// startElement(namespaceURI,localName,qName,atts)
	/// </pre>
	/// could be replaced with the calls
	/// <pre>
	/// startElement(namespaceURI,localName,qName)
	/// addAttributes(atts)
	/// </pre>
	/// If there are no attributes the second call can be dropped. If attributes are
	/// to be added one at a time with calls to
	/// <pre>
	/// addAttribute(namespaceURI, localName, qName, type, value)
	/// </pre>
	/// @xsl.usage internal
	/// </summary>
	public interface ExtendedContentHandler : org.xml.sax.ContentHandler
	{
		/// <summary>
		/// Add at attribute to the current element </summary>
		/// <param name="uri"> the namespace URI of the attribute name </param>
		/// <param name="localName"> the local name of the attribute (without prefix) </param>
		/// <param name="rawName"> the qualified name of the attribute </param>
		/// <param name="type"> the attribute type typically character data (CDATA) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <param name="XSLAttribute"> true if the added attribute is coming from an xsl:attribute element </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value, boolean XSLAttribute) throws org.xml.sax.SAXException;
		void addAttribute(string uri, string localName, string rawName, string type, string value, bool XSLAttribute);
		/// <summary>
		/// Add attributes to the current element </summary>
		/// <param name="atts"> the attributes to add. </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttributes(org.xml.sax.Attributes atts) throws org.xml.sax.SAXException;
		void addAttributes(org.xml.sax.Attributes atts);
		/// <summary>
		/// Add an attribute to the current element. The namespace URI of the
		/// attribute will be calculated from the prefix of qName. The local name
		/// will be derived from qName and the type will be assumed to be "CDATA". </summary>
		/// <param name="qName"> </param>
		/// <param name="value"> </param>
		void addAttribute(string qName, string value);

		/// <summary>
		/// This method is used to notify of a character event, but passing the data
		/// as a character String rather than the standard character array. </summary>
		/// <param name="chars"> the character data </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(String chars) throws org.xml.sax.SAXException;
		void characters(string chars);

		/// <summary>
		/// This method is used to notify of a character event, but passing the data
		/// as a DOM Node rather than the standard character array. </summary>
		/// <param name="node"> a DOM Node containing text. </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void characters(org.w3c.dom.Node node) throws org.xml.sax.SAXException;
		void characters(org.w3c.dom.Node node);
		/// <summary>
		/// This method is used to notify that an element has ended. Unlike the
		/// standard SAX method
		/// <pre>
		/// endElement(namespaceURI,localName,qName)
		/// </pre>
		/// only the last parameter is passed. If needed the serializer can derive
		/// the localName from the qualified name and derive the namespaceURI from
		/// its implementation. </summary>
		/// <param name="elemName"> the fully qualified element name. </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(String elemName) throws org.xml.sax.SAXException;
		void endElement(string elemName);

		/// <summary>
		/// This method is used to notify that an element is starting.
		/// This method is just like the standard SAX method
		/// <pre>
		/// startElement(uri,localName,qname,atts)
		/// </pre>
		/// but without the attributes. </summary>
		/// <param name="uri"> the namespace URI of the element </param>
		/// <param name="localName"> the local name (without prefix) of the element </param>
		/// <param name="qName"> the qualified name of the element
		/// </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String uri, String localName, String qName) throws org.xml.sax.SAXException;
		void startElement(string uri, string localName, string qName);

		/// <summary>
		/// This method is used to notify of the start of an element </summary>
		/// <param name="qName"> the fully qualified name of the element </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String qName) throws org.xml.sax.SAXException;
		void startElement(string qName);
		/// <summary>
		/// This method is used to notify that a prefix mapping is to start, but
		/// after an element is started. The SAX method call
		/// <pre>
		/// startPrefixMapping(prefix,uri)
		/// </pre>
		/// is used just before an element starts and applies to the element to come,
		/// not to the current element.  This method applies to the current element.
		/// For example one could make the calls in this order:
		/// <pre>
		/// startElement("prfx8:elem9")
		/// namespaceAfterStartElement("http://namespace8","prfx8")
		/// </pre>
		/// </summary>
		/// <param name="uri"> the namespace URI being declared </param>
		/// <param name="prefix"> the prefix that maps to the given namespace </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void namespaceAfterStartElement(String uri, String prefix) throws org.xml.sax.SAXException;
		void namespaceAfterStartElement(string uri, string prefix);

		/// <summary>
		/// This method is used to notify that a prefix maping is to start, which can
		/// be for the current element, or for the one to come. </summary>
		/// <param name="prefix"> the prefix that maps to the given URI </param>
		/// <param name="uri"> the namespace URI of the given prefix </param>
		/// <param name="shouldFlush"> if true this call is like the SAX
		/// startPrefixMapping(prefix,uri) call and the mapping applies to the
		/// element to come.  If false the mapping applies to the current element. </param>
		/// <returns> boolean false if the prefix mapping was already in effect (in
		/// other words we are just re-declaring), true if this is a new, never
		/// before seen mapping for the element. </returns>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean startPrefixMapping(String prefix, String uri, boolean shouldFlush) throws org.xml.sax.SAXException;
		bool startPrefixMapping(string prefix, string uri, bool shouldFlush);
		/// <summary>
		/// Notify of an entity reference. </summary>
		/// <param name="entityName"> the name of the entity </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void entityReference(String entityName) throws org.xml.sax.SAXException;
		void entityReference(string entityName);

		/// <summary>
		/// This method returns an object that has the current namespace mappings in
		/// effect.
		/// </summary>
		/// <returns> NamespaceMappings an object that has the current namespace
		/// mappings in effect. </returns>
		NamespaceMappings NamespaceMappings {get;}
		/// <summary>
		/// This method returns the prefix that currently maps to the given namespace
		/// URI. </summary>
		/// <param name="uri"> the namespace URI </param>
		/// <returns> String the prefix that currently maps to the given URI. </returns>
		string getPrefix(string uri);
		/// <summary>
		/// This method gets the prefix associated with a current element or
		/// attribute name. </summary>
		/// <param name="name"> the qualified name of an element, or attribute </param>
		/// <param name="isElement"> true if it is an element name, false if it is an
		/// atttribute name </param>
		/// <returns> String the namespace URI associated with the element or
		/// attribute. </returns>
		string getNamespaceURI(string name, bool isElement);
		/// <summary>
		/// This method returns the namespace URI currently associated with the
		/// prefix. </summary>
		/// <param name="prefix"> a prefix of an element or attribute. </param>
		/// <returns> String the namespace URI currently associated with the prefix. </returns>
		string getNamespaceURIFromPrefix(string prefix);

		/// <summary>
		/// This method is used to set the source locator, which might be used to
		/// generated an error message. </summary>
		/// <param name="locator"> the source locator </param>
		SourceLocator SourceLocator {set;}

		// Bit constants for addUniqueAttribute().

		// The attribute value contains no bad characters. A "bad" character is one which
		// is greater than 126 or it is one of '<', '>', '&' or '"'.

		// An HTML empty attribute (e.g. <OPTION selected>).

		// An HTML URL attribute

		/// <summary>
		/// Add a unique attribute to the current element.
		/// The attribute is guaranteed to be unique here. The serializer can write
		/// it out immediately without saving it in a table first. The integer
		/// flag contains information about the attribute, which helps the serializer
		/// to decide whether a particular processing is needed.
		/// </summary>
		/// <param name="qName"> the fully qualified attribute name. </param>
		/// <param name="value"> the attribute value </param>
		/// <param name="flags"> a bitwise flag </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addUniqueAttribute(String qName, String value, int flags) throws org.xml.sax.SAXException;
		void addUniqueAttribute(string qName, string value, int flags);

		/// <summary>
		/// Add an attribute from an xsl:attribute element. </summary>
		/// <param name="qName"> the qualified attribute name (prefix:localName) </param>
		/// <param name="value"> the attributes value </param>
		/// <param name="uri"> the uri that the prefix of the qName is mapped to. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void addXSLAttribute(String qName, final String value, final String uri);
		void addXSLAttribute(string qName, string value, string uri);

		/// <summary>
		/// Add at attribute to the current element, not from an xsl:attribute
		/// element. </summary>
		/// <param name="uri"> the namespace URI of the attribute name </param>
		/// <param name="localName"> the local name of the attribute (without prefix) </param>
		/// <param name="rawName"> the qualified name of the attribute </param>
		/// <param name="type"> the attribute type typically character data (CDATA) </param>
		/// <param name="value"> the value of the attribute </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addAttribute(String uri, String localName, String rawName, String type, String value) throws org.xml.sax.SAXException;
		void addAttribute(string uri, string localName, string rawName, string type, string value);
	}

	public static class ExtendedContentHandler_Fields
	{
		public const int NO_BAD_CHARS = 0x1;
		public const int HTML_ATTREMPTY = 0x2;
		public const int HTML_ATTRURL = 0x4;
	}

}