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
 * $Id: Serializer.java 468642 2006-10-28 06:55:10Z minchau $
 */
namespace org.apache.xalan.serialize
{


	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// The Serializer interface is implemented by Serializers to publish methods
	/// to get and set streams and writers, to set the output properties, and
	/// get the Serializer as a ContentHandler or DOMSerializer. </summary>
	/// @deprecated Use org.apache.xml.serializer.Serializer 
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
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="output"> The output stream
	  /// </param>
	  /// @deprecated Use org.apache.xml.serializer.Serializer 
	  System.IO.Stream OutputStream {set;get;}


	  /// <summary>
	  /// Specifies a writer to which the document should be serialized.
	  /// This method should not be called while the serializer is in
	  /// the process of serializing a document.
	  /// <para>
	  /// The encoding specified for the output <seealso cref="Properties"/> must be
	  /// identical to the output format used with the writer.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="writer"> The output writer stream
	  /// </param>
	  /// @deprecated Use org.apache.xml.serializer.Serializer 
	  Writer Writer {set;get;}


	  /// <summary>
	  /// Specifies an output format for this serializer. It the
	  /// serializer has already been associated with an output format,
	  /// it will switch to the new format. This method should not be
	  /// called while the serializer is in the process of serializing
	  /// a document.
	  /// </summary>
	  /// <param name="format"> The output format to use
	  /// </param>
	  /// @deprecated Use org.apache.xml.serializer.Serializer 
	  Properties OutputFormat {set;get;}


	  /// <summary>
	  /// Return a <seealso cref="ContentHandler"/> interface into this serializer.
	  /// If the serializer does not support the <seealso cref="ContentHandler"/>
	  /// interface, it should return null.
	  /// </summary>
	  /// <returns> A <seealso cref="ContentHandler"/> interface into this serializer,
	  ///  or null if the serializer is not SAX 2 capable </returns>
	  /// <exception cref="IOException"> An I/O exception occured </exception>
	  /// @deprecated Use org.apache.xml.serializer.Serializer 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.ContentHandler asContentHandler() throws java.io.IOException;
	  ContentHandler asContentHandler();

	  /// <summary>
	  /// Return a <seealso cref="DOMSerializer"/> interface into this serializer.
	  /// If the serializer does not support the <seealso cref="DOMSerializer"/>
	  /// interface, it should return null.
	  /// </summary>
	  /// <returns> A <seealso cref="DOMSerializer"/> interface into this serializer,
	  ///  or null if the serializer is not DOM capable </returns>
	  /// <exception cref="IOException"> An I/O exception occured </exception>
	  /// @deprecated Use org.apache.xml.serializer.Serializer 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DOMSerializer asDOMSerializer() throws java.io.IOException;
	  DOMSerializer asDOMSerializer();

	  /// <summary>
	  /// Resets the serializer. If this method returns true, the
	  /// serializer may be used for subsequent serialization of new
	  /// documents. It is possible to change the output format and
	  /// output stream prior to serializing, or to use the existing
	  /// output format and output stream.
	  /// </summary>
	  /// <returns> True if serializer has been reset and can be reused
	  /// </returns>
	  /// @deprecated Use org.apache.xml.serializer.Serializer 
	  bool reset();
	}

}