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
 * $Id: DOMLocatorImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{
	using DOMLocator = org.w3c.dom.DOMLocator;
	using Node = org.w3c.dom.Node;


	/// <summary>
	/// <code>DOMLocatorImpl</code> is an implementaion that describes a location (e.g. 
	/// where an error occured).
	/// <para>See also the <a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407'>Document Object Model (DOM) Level 3 Core Specification</a>.
	/// This class is a copy of the Xerces-2J class org.apache.xerces.dom.DOMLocatorImpl.java v 1.10 
	/// 
	/// @author Gopal Sharma, SUN Microsystems Inc.
	/// @version $Id: 
	/// 
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	internal sealed class DOMLocatorImpl : DOMLocator
	{

		//
		// Data
		//

		/// <summary>
		/// The column number where the error occured, 
		/// or -1 if there is no column number available.
		/// </summary>
		private readonly int fColumnNumber;

		/// <summary>
		/// The line number where the error occured, 
		/// or -1 if there is no line number available.
		/// </summary>
		private readonly int fLineNumber;

		/// <summary>
		/// related data node </summary>
		private readonly Node fRelatedNode;

		/// <summary>
		/// The URI where the error occured, 
		/// or null if there is no URI available.
		/// </summary>
		private readonly string fUri;

		/// <summary>
		/// The byte offset into the input source this locator is pointing to or -1 
		/// if there is no byte offset available
		/// </summary>
		private readonly int fByteOffset;

		/// <summary>
		/// The UTF-16, as defined in [Unicode] and Amendment 1 of [ISO/IEC 10646], 
		/// offset into the input source this locator is pointing to or -1 if there 
		/// is no UTF-16 offset available.
		/// </summary>
		private readonly int fUtf16Offset;

		//
		// Constructors
		//

		internal DOMLocatorImpl()
		{
			fColumnNumber = -1;
			fLineNumber = -1;
			fRelatedNode = null;
			fUri = null;
			fByteOffset = -1;
			fUtf16Offset = -1;
		}

		internal DOMLocatorImpl(int lineNumber, int columnNumber, string uri)
		{
			fLineNumber = lineNumber;
			fColumnNumber = columnNumber;
			fUri = uri;

			fRelatedNode = null;
			fByteOffset = -1;
			fUtf16Offset = -1;
		} // DOMLocatorImpl (int lineNumber, int columnNumber, String uri )

		internal DOMLocatorImpl(int lineNumber, int columnNumber, int utf16Offset, string uri)
		{
			fLineNumber = lineNumber;
			fColumnNumber = columnNumber;
			fUri = uri;
			fUtf16Offset = utf16Offset;


			fRelatedNode = null;
			fByteOffset = -1;
		} // DOMLocatorImpl (int lineNumber, int columnNumber, int utf16Offset, String uri )

		internal DOMLocatorImpl(int lineNumber, int columnNumber, int byteoffset, Node relatedData, string uri)
		{
			fLineNumber = lineNumber;
			fColumnNumber = columnNumber;
			fByteOffset = byteoffset;
			fRelatedNode = relatedData;
			fUri = uri;

			fUtf16Offset = -1;
		} // DOMLocatorImpl (int lineNumber, int columnNumber, int offset, Node errorNode, String uri )

		internal DOMLocatorImpl(int lineNumber, int columnNumber, int byteoffset, Node relatedData, string uri, int utf16Offset)
		{
			fLineNumber = lineNumber;
			fColumnNumber = columnNumber;
			fByteOffset = byteoffset;
			fRelatedNode = relatedData;
			fUri = uri;
			fUtf16Offset = utf16Offset;
		} // DOMLocatorImpl (int lineNumber, int columnNumber, int offset, Node errorNode, String uri )


		/// <summary>
		/// The line number where the error occured, or -1 if there is no line 
		/// number available.
		/// </summary>
		public int LineNumber
		{
			get
			{
				return fLineNumber;
			}
		}

		/// <summary>
		/// The column number where the error occured, or -1 if there is no column 
		/// number available.
		/// </summary>
		public int ColumnNumber
		{
			get
			{
				return fColumnNumber;
			}
		}


		/// <summary>
		/// The URI where the error occured, or null if there is no URI available.
		/// </summary>
		public string Uri
		{
			get
			{
				return fUri;
			}
		}


		public Node RelatedNode
		{
			get
			{
				return fRelatedNode;
			}
		}


		/// <summary>
		/// The byte offset into the input source this locator is pointing to or -1 
		/// if there is no byte offset available
		/// </summary>
		public int ByteOffset
		{
			get
			{
				return fByteOffset;
			}
		}

		/// <summary>
		/// The UTF-16, as defined in [Unicode] and Amendment 1 of [ISO/IEC 10646], 
		/// offset into the input source this locator is pointing to or -1 if there 
		/// is no UTF-16 offset available.
		/// </summary>
		public int Utf16Offset
		{
			get
			{
				return fUtf16Offset;
			}
		}

	} // class DOMLocatorImpl

}