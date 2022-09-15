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
 * $Id: DOM3SerializerImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{

	using WrappedRuntimeException = org.apache.xml.serializer.utils.WrappedRuntimeException;
	using DOMErrorHandler = org.w3c.dom.DOMErrorHandler;
	using Node = org.w3c.dom.Node;
	using LSSerializerFilter = org.w3c.dom.ls.LSSerializerFilter;

	/// <summary>
	/// This class implements the DOM3Serializer interface.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public sealed class DOM3SerializerImpl : DOM3Serializer
	{

		/// <summary>
		/// Private class members
		/// </summary>
		// The DOMErrorHandler
		private DOMErrorHandler fErrorHandler;

		// A LSSerializerFilter
		private LSSerializerFilter fSerializerFilter;

		// A LSSerializerFilter
		private string fNewLine;

		// A SerializationHandler ex. an instance of ToXMLStream
		private SerializationHandler fSerializationHandler;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="handler"> An instance of the SerializationHandler interface.  </param>
		public DOM3SerializerImpl(SerializationHandler handler)
		{
			fSerializationHandler = handler;
		}

		// Public memebers

		/// <summary>
		/// Returns a DOMErrorHandler set on the DOM Level 3 Serializer.
		/// 
		/// This interface is a public API.
		/// </summary>
		/// <returns> A Level 3 DOMErrorHandler </returns>
		public DOMErrorHandler ErrorHandler
		{
			get
			{
				return fErrorHandler;
			}
			set
			{
				fErrorHandler = value;
			}
		}

		/// <summary>
		/// Returns a LSSerializerFilter set on the DOM Level 3 Serializer to filter nodes
		/// during serialization.
		/// 
		/// This interface is a public API.
		/// </summary>
		/// <returns> The Level 3 LSSerializerFilter </returns>
		public LSSerializerFilter NodeFilter
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
		/// Gets the end-of-line sequence of characters to be used during serialization.
		/// </summary>
		public char[] NewLine
		{
			get
			{
				return (!string.ReferenceEquals(fNewLine, null)) ? fNewLine.ToCharArray() : null;
			}
			set
			{
				fNewLine = (value != null) ? new string(value) : null;
			}
		}

		/// <summary>
		/// Serializes the Level 3 DOM node by creating an instance of DOM3TreeWalker
		/// which traverses the DOM tree and invokes handler events to serialize
		/// the DOM NOde. Throws an exception only if an I/O exception occured
		/// while serializing.
		/// This interface is a public API.
		/// </summary>
		/// <param name="node"> the Level 3 DOM node to serialize </param>
		/// <exception cref="IOException"> if an I/O exception occured while serializing </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serializeDOM3(org.w3c.dom.Node node) throws java.io.IOException
		public void serializeDOM3(Node node)
		{
			try
			{
				DOM3TreeWalker walker = new DOM3TreeWalker(fSerializationHandler, fErrorHandler, fSerializerFilter, fNewLine);

				walker.traverse(node);
			}
			catch (org.xml.sax.SAXException se)
			{
				throw new WrappedRuntimeException(se);
			}
		}



		/// <summary>
		/// Sets a SerializationHandler on the DOM Serializer.
		/// 
		/// This interface is a public API.
		/// </summary>
		/// <param name="handler"> An instance of SerializationHandler </param>
		public SerializationHandler SerializationHandler
		{
			set
			{
				fSerializationHandler = value;
			}
		}

	}

}