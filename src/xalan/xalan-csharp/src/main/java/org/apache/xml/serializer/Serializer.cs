using System.IO;

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
 * $Id: Serializer.java 471981 2006-11-07 04:28:00Z minchau $
 */
namespace org.apache.xml.serializer
{

	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// The Serializer interface is implemented by a serializer to enable users to:
	/// <ul>
	/// <li>get and set streams or writers
	/// <li>configure the serializer with key/value properties
	/// <li>get an org.xml.sax.ContentHandler or a DOMSerializer to provide input to
	/// </ul>
	/// 
	/// <para>
	/// Here is an example using the asContentHandler() method:
	/// <pre>
	/// java.util.Properties props = 
	///   OutputPropertiesFactory.getDefaultMethodProperties(Method.TEXT);
	/// Serializer ser = SerializerFactory.getSerializer(props);
	/// java.io.PrintStream ostream = System.out; 
	/// ser.setOutputStream(ostream);
	/// 
	/// // Provide the SAX input events
	/// ContentHandler handler = ser.asContentHandler();
	/// handler.startDocument();
	/// char[] chars = { 'a', 'b', 'c' };
	/// handler.characters(chars, 0, chars.length);
	/// handler.endDocument();
	/// 
	/// ser.reset(); // get ready to use the serializer for another document
	///              // of the same output method (TEXT).
	/// </pre>
	/// 
	/// </para>
	/// <para>
	/// As an alternate to supplying a series of SAX events as input through the 
	/// ContentHandler interface, the input to serialize may be given as a DOM. 
	/// </para>
	/// <para>
	/// For example:
	/// <pre>
	/// org.w3c.dom.Document     inputDoc;
	/// org.apache.xml.serializer.Serializer   ser;
	/// java.io.Writer owriter;
	/// 
	/// java.util.Properties props = 
	///   OutputPropertiesFactory.getDefaultMethodProperties(Method.XML);
	/// Serializer ser = SerializerFactory.getSerializer(props);
	/// owriter = ...;  // create a writer to serialize the document to
	/// ser.setWriter( owriter );
	/// 
	/// inputDoc = ...; // create the DOM document to be serialized
	/// DOMSerializer dser = ser.asDOMSerializer(); // a DOM will be serialized
	/// dser.serialize(inputDoc); // serialize the DOM, sending output to owriter
	/// 
	/// ser.reset(); // get ready to use the serializer for another document
	///              // of the same output method.
	/// </pre>
	/// 
	/// This interface is a public API.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref="Method"/>
	/// <seealso cref="OutputPropertiesFactory"/>
	/// <seealso cref="SerializerFactory"/>
	/// <seealso cref="DOMSerializer"/>
	/// <seealso cref="ContentHandler"
	/// 
	/// @xsl.usage general/>
	public interface Serializer
	{

		/// <summary>
		/// Specifies an output stream to which the document should be
		/// serialized. This method should not be called while the
		/// serializer is in the process of serializing a document.
		/// <para>
		/// The encoding specified in the output <seealso cref="Properties"/> is used, or
		/// if no encoding was specified, the default for the selected
		/// output method.
		/// </para>
		/// <para>
		/// Only one of setWriter() or setOutputStream() should be called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="output"> The output stream </param>
		Stream OutputStream {set;get;}


		/// <summary>
		/// Specifies a writer to which the document should be serialized.
		/// This method should not be called while the serializer is in
		/// the process of serializing a document.
		/// <para>
		/// The encoding specified for the output <seealso cref="Properties"/> must be
		/// identical to the output format used with the writer.
		/// 
		/// </para>
		/// <para>
		/// Only one of setWriter() or setOutputStream() should be called.
		/// 
		/// </para>
		/// </summary>
		/// <param name="writer"> The output writer stream </param>
		Writer Writer {set;get;}


		/// <summary>
		/// Specifies an output format for this serializer. It the
		/// serializer has already been associated with an output format,
		/// it will switch to the new format. This method should not be
		/// called while the serializer is in the process of serializing
		/// a document.
		/// <para>
		/// The standard property keys supported are: "method", "version", "encoding",
		/// "omit-xml-declaration", "standalone", doctype-public",
		/// "doctype-system", "cdata-section-elements", "indent", "media-type". 
		/// These property keys and their values are described in the XSLT recommendation,
		/// see <seealso cref="<a href="http://www.w3.org/TR/1999/REC-xslt-19991116"> XSLT 1.0 recommendation</a>"/>
		/// </para>
		/// <para>
		/// The non-standard property keys supported are defined in <seealso cref="OutputPropertiesFactory"/>.
		/// 
		/// </para>
		/// <para>
		/// This method can be called multiple times before a document is serialized. Each 
		/// time it is called more, or over-riding property values, can be specified. One 
		/// property value that can not be changed is that of the "method" property key.
		/// </para>
		/// <para>
		/// The value of the "cdata-section-elements" property key is a whitespace
		/// separated list of elements. If the element is in a namespace then 
		/// value is passed in this format: {uri}localName 
		/// </para>
		/// <para>
		/// If the "cdata-section-elements" key is specified on multiple calls
		/// to this method the set of elements specified in the value
		/// is not replaced from one call to the
		/// next, but it is cumulative across the calls.
		/// 
		/// </para>
		/// </summary>
		/// <param name="format"> The output format to use, as a set of key/value pairs. </param>
		Properties OutputFormat {set;get;}


		/// <summary>
		/// Return a <seealso cref="ContentHandler"/> interface to provide SAX input to.
		/// Through the returned object the document to be serailized,
		/// as a series of SAX events, can be provided to the serialzier.
		/// If the serializer does not support the <seealso cref="ContentHandler"/>
		/// interface, it will return null.
		/// <para>
		/// In principle only one of asDOMSerializer() or asContentHander() 
		/// should be called.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A <seealso cref="ContentHandler"/> interface into this serializer,
		///  or null if the serializer is not SAX 2 capable </returns>
		/// <exception cref="IOException"> An I/O exception occured </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.xml.sax.ContentHandler asContentHandler() throws java.io.IOException;
		ContentHandler asContentHandler();

		/// <summary>
		/// Return a <seealso cref="DOMSerializer"/> interface into this serializer.
		/// Through the returned object the document to be serialized,
		/// a DOM, can be provided to the serializer.
		/// If the serializer does not support the <seealso cref="DOMSerializer"/>
		/// interface, it should return null.
		/// <para>
		/// In principle only one of asDOMSerializer() or asContentHander() 
		/// should be called.
		/// 
		/// </para>
		/// </summary>
		/// <returns> A <seealso cref="DOMSerializer"/> interface into this serializer,
		///  or null if the serializer is not DOM capable </returns>
		/// <exception cref="IOException"> An I/O exception occured </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public DOMSerializer asDOMSerializer() throws java.io.IOException;
		DOMSerializer asDOMSerializer();

		/// <summary>
		/// This method resets the serializer. 
		/// If this method returns true, the
		/// serializer may be used for subsequent serialization of new
		/// documents. It is possible to change the output format and
		/// output stream prior to serializing, or to reuse the existing
		/// output format and output stream or writer.
		/// </summary>
		/// <returns> True if serializer has been reset and can be reused </returns>
		bool reset();

		/// <summary>
		/// Return an Object into this serializer to be cast to a DOM3Serializer.
		/// Through the returned object the document to be serialized,
		/// a DOM (Level 3), can be provided to the serializer.
		/// If the serializer does not support casting to a <seealso cref="DOM3Serializer"/>
		/// interface, it should return null.
		/// <para>
		/// In principle only one of asDOM3Serializer() or asContentHander() 
		/// should be called.
		/// 
		/// </para>
		/// </summary>
		/// <returns> An Object to be cast to a DOM3Serializer interface into this serializer,
		///  or null if the serializer is not DOM capable </returns>
		/// <exception cref="IOException"> An I/O exception occured </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object asDOM3Serializer() throws java.io.IOException;
		object asDOM3Serializer();
	}


}