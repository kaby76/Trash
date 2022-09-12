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
 * $Id: DOM3Serializer.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer
{

	using DOMErrorHandler = org.w3c.dom.DOMErrorHandler;
	using Node = org.w3c.dom.Node;
	using LSSerializerFilter = org.w3c.dom.ls.LSSerializerFilter;

	/// <summary>
	/// This interface is not intended to be used
	/// by an end user, but rather by an XML parser that is implementing the DOM 
	/// Level 3 Load and Save APIs.
	/// <para>
	/// 
	/// See the DOM Level 3 Load and Save interface at <a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-LS-20040407/load-save.html#LS-LSSerializer">LSSeializer</a>.
	/// 
	/// For a list of configuration parameters for DOM Level 3 see <a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407/core.html#DOMConfiguration">DOMConfiguration</a>.
	/// For additional configuration parameters available with the DOM Level 3 Load and Save API LSSerializer see
	/// <a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-LS-20040407/load-save.html#LS-LSSerializer-config">LSerializer config</a>.
	/// </para>
	/// <para>
	/// The following example uses a DOM3Serializer indirectly, through an an XML
	/// parser that uses this class as part of its implementation of the DOM Level 3
	/// Load and Save APIs, and is the prefered way to serialize with DOM Level 3 APIs.
	/// </para>
	/// <para>
	/// Example:
	/// <pre>
	///    public class TestDOM3 {
	/// 
	///    public static void main(String args[]) throws Exception {
	///        // Get document to serialize
	///        TestDOM3 test = new TestDOM3();
	/// 
	///        // Serialize using standard DOM Level 3 Load/Save APIs        
	///        System.out.println(test.testDOM3LS());
	///    }
	/// 
	///    public org.w3c.dom.Document getDocument() throws Exception {
	///        // Create a simple DOM Document.
	///        javax.xml.parsers.DocumentBuilderFactory factory = 
	///            javax.xml.parsers.DocumentBuilderFactory.newInstance();
	///        javax.xml.parsers.DocumentBuilder builder = factory.newDocumentBuilder();
	///        byte[] bytes = "<parent><child/></parent>".getBytes();
	///        java.io.InputStream is = new java.io.ByteArrayInputStream(bytes);
	///        org.w3c.dom.Document doc = builder.parse(is);
	///        return doc;
	///    }
	/// 
	///    //
	///    // This method uses standard DOM Level 3 Load Save APIs:
	///    //   org.w3c.dom.bootstrap.DOMImplementationRegistry
	///    //   org.w3c.dom.ls.DOMImplementationLS
	///    //   org.w3c.dom.ls.DOMImplementationLS
	///    //   org.w3c.dom.ls.LSSerializer
	///    //   org.w3c.dom.DOMConfiguration
	///    //   
	///    // The only thing non-standard in this method is the value set for the
	///    // name of the class implementing the DOM Level 3 Load Save APIs,
	///    // which in this case is:
	///    //   org.apache.xerces.dom.DOMImplementationSourceImpl
	///    //
	/// 
	///    public String testDOM3LS() throws Exception {
	/// 
	///        // Get a simple DOM Document that will be serialized.
	///        org.w3c.dom.Document docToSerialize = getDocument();
	/// 
	///        // Get a factory (DOMImplementationLS) for creating a Load and Save object.
	///        org.w3c.dom.ls.DOMImplementationLS impl = 
	///            (org.w3c.dom.ls.DOMImplementationLS) 
	///            org.w3c.dom.bootstrap.DOMImplementationRegistry.newInstance().getDOMImplementation("LS");
	/// 
	///        // Use the factory to create an object (LSSerializer) used to 
	///        // write out or save the document.
	///        org.w3c.dom.ls.LSSerializer writer = impl.createLSSerializer();
	///        org.w3c.dom.DOMConfiguration config = writer.getDomConfig();
	///        config.setParameter("format-pretty-print", Boolean.TRUE);
	/// 
	///        // Use the LSSerializer to write out or serialize the document to a String.
	///        String serializedXML = writer.writeToString(docToSerialize);
	///        return serializedXML;
	///    }
	/// 
	///    }  // end of class TestDOM3
	/// </pre>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407/core.html#DOMConfiguration">DOMConfiguration</a> </seealso>
	/// <seealso cref= <a href="http://www.w3.org/TR/2004/REC-DOM-Level-3-LS-20040407/load-save.html#LS-LSSerializer-config">LSSerializer</a> </seealso>
	/// <seealso cref= org.apache.xml.serializer.Serializer </seealso>
	/// <seealso cref= org.apache.xml.serializer.DOMSerializer
	/// 
	/// @xsl.usage advanced
	///  </seealso>
	public interface DOM3Serializer
	{
		/// <summary>
		/// Serializes the Level 3 DOM node. Throws an exception only if an I/O
		/// exception occured while serializing.
		/// 
		/// This interface is a public API.
		/// </summary>
		/// <param name="node"> the Level 3 DOM node to serialize </param>
		/// <exception cref="IOException"> if an I/O exception occured while serializing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serializeDOM3(org.w3c.dom.Node node) throws java.io.IOException;
		void serializeDOM3(Node node);

		/// <summary>
		/// Sets a DOMErrorHandler on the DOM Level 3 Serializer.
		/// 
		/// This interface is a public API.
		/// </summary>
		/// <param name="handler"> the Level 3 DOMErrorHandler </param>
		DOMErrorHandler ErrorHandler {set;get;}


		/// <summary>
		/// Sets a LSSerializerFilter on the DOM Level 3 Serializer to filter nodes
		/// during serialization.
		/// 
		/// This interface is a public API.
		/// </summary>
		/// <param name="filter"> the Level 3 LSSerializerFilter </param>
		LSSerializerFilter NodeFilter {set;get;}


		/// <summary>
		/// Sets the end-of-line sequence of characters to be used during serialization </summary>
		/// <param name="newLine"> The end-of-line sequence of characters to be used during serialization </param>
		char[] NewLine {set;}
	}

}