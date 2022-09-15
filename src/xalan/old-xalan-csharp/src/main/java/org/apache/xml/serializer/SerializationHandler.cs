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
 * $Id: SerializationHandler.java 471981 2006-11-07 04:28:00Z minchau $
 */
namespace org.apache.xml.serializer
{


	using Node = org.w3c.dom.Node;
	using ContentHandler = org.xml.sax.ContentHandler;
	using ErrorHandler = org.xml.sax.ErrorHandler;
	using SAXException = org.xml.sax.SAXException;
	using DeclHandler = org.xml.sax.ext.DeclHandler;

	/// <summary>
	/// This interface is the one that a serializer implements. It is a group of
	/// other interfaces, such as ExtendedContentHandler, ExtendedLexicalHandler etc.
	/// In addition there are other methods, such as reset().
	/// 
	/// This class is public only because it is used in another package,
	/// it is not a public API.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public interface SerializationHandler : ExtendedContentHandler, ExtendedLexicalHandler, XSLOutputAttributes, DeclHandler, org.xml.sax.DTDHandler, ErrorHandler, DOMSerializer, Serializer
	{
		/// <summary>
		/// Set the SAX Content handler that the serializer sends its output to. This
		/// method only applies to a ToSAXHandler, not to a ToStream serializer.
		/// </summary>
		/// <seealso cref= Serializer#asContentHandler() </seealso>
		/// <seealso cref= ToSAXHandler </seealso>
		ContentHandler ContentHandler {set;}

		void close();

		/// <summary>
		/// Notify that the serializer should take this DOM node as input to be
		/// serialized.
		/// </summary>
		/// <param name="node"> the DOM node to be serialized. </param>
		/// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException;
		void serialize(Node node);
		/// <summary>
		/// Turns special character escaping on/off. 
		/// 
		/// Note that characters will
		/// never, even if this option is set to 'true', be escaped within
		/// CDATA sections in output XML documents.
		/// </summary>
		/// <param name="escape"> true if escaping is to be set on. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean setEscaping(boolean escape) throws org.xml.sax.SAXException;
		bool setEscaping(bool escape);

		/// <summary>
		/// Set the number of spaces to indent for each indentation level. </summary>
		/// <param name="spaces"> the number of spaces to indent for each indentation level. </param>
		int IndentAmount {set;}

		/// <summary>
		/// Set the transformer associated with the serializer. </summary>
		/// <param name="transformer"> the transformer associated with the serializer. </param>
		Transformer Transformer {set;get;}


		/// <summary>
		/// Used only by TransformerSnapshotImpl to restore the serialization 
		/// to a previous state. 
		/// </summary>
		/// <param name="mappings"> NamespaceMappings </param>
		NamespaceMappings NamespaceMappings {set;}

		/// <summary>
		/// A SerializationHandler accepts SAX-like events, so
		/// it can accumulate attributes or namespace nodes after
		/// a startElement().
		/// <para>
		/// If the SerializationHandler has a Writer or OutputStream, 
		/// a call to this method will flush such accumulated 
		/// events as a closed start tag for an element.
		/// </para>
		/// <para>
		/// If the SerializationHandler wraps a ContentHandler,
		/// a call to this method will flush such accumulated
		/// events as a SAX (not SAX-like) calls to
		/// startPrefixMapping() and startElement().
		/// </para>
		/// <para>
		/// If one calls endDocument() then one need not call
		/// this method since a call to endDocument() will
		/// do what this method does. However, in some
		/// circumstances, such as with document fragments,
		/// endDocument() is not called and it may be
		/// necessary to call this method to flush
		/// any pending events.
		/// </para>
		/// <para> 
		/// For performance reasons this method should not be called
		/// very often. 
		/// </para>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flushPending() throws org.xml.sax.SAXException;
		void flushPending();

		/// <summary>
		/// Default behavior is to expand DTD entities,
		/// that is the initall default value is true. </summary>
		/// <param name="expand"> true if DTD entities are to be expanded,
		/// false if they are to be left as DTD entity references.  </param>
		bool DTDEntityExpansion {set;}

	}

}