using System;
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
 * $Id: NamespaceSupport.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xml.serializer.dom3
{

	/// <summary>
	/// Namespace support for XML document handlers. This class doesn't 
	/// perform any error checking and assumes that all strings passed
	/// as arguments to methods are unique symbols. The SymbolTable class
	/// can be used for this purpose.
	/// 
	/// Derived from org.apache.xerces.util.NamespaceSupport
	/// 
	/// @author Andy Clark, IBM
	/// 
	/// @version $Id: NamespaceSupport.java 1225426 2011-12-29 04:13:08Z mrglavas $
	/// </summary>
	public class NamespaceSupport
	{

		internal static readonly string PREFIX_XML = "xml".intern();

		internal static readonly string PREFIX_XMLNS = "xmlns".intern();

		/// <summary>
		/// The XML Namespace ("http://www.w3.org/XML/1998/namespace"). This is
		/// the Namespace URI that is automatically mapped to the "xml" prefix.
		/// </summary>
		public static readonly string XML_URI = "http://www.w3.org/XML/1998/namespace".intern();

		/// <summary>
		/// XML Information Set REC
		/// all namespace attributes (including those named xmlns, 
		/// whose [prefix] property has no value) have a namespace URI of http://www.w3.org/2000/xmlns/
		/// </summary>
		public static readonly string XMLNS_URI = "http://www.w3.org/2000/xmlns/".intern();

		//
		// Data
		//

		/// <summary>
		/// Namespace binding information. This array is composed of a
		/// series of tuples containing the namespace binding information:
		/// &lt;prefix, uri&gt;. The default size can be set to anything
		/// as long as it is a power of 2 greater than 1.
		/// </summary>
		/// <seealso cref=".fNamespaceSize"/>
		/// <seealso cref=".fContext"/>
		protected internal string[] fNamespace = new string[16 * 2];

		/// <summary>
		/// The top of the namespace information array. </summary>
		protected internal int fNamespaceSize;

		// NOTE: The constructor depends on the initial context size 
		//       being at least 1. -Ac

		/// <summary>
		/// Context indexes. This array contains indexes into the namespace
		/// information array. The index at the current context is the start
		/// index of declared namespace bindings and runs to the size of the
		/// namespace information array.
		/// </summary>
		/// <seealso cref=".fNamespaceSize"/>
		protected internal int[] fContext = new int[8];

		/// <summary>
		/// The current context. </summary>
		protected internal int fCurrentContext;

		protected internal string[] fPrefixes = new string[16];

		//
		// Constructors
		//

		/// <summary>
		/// Default constructor. </summary>
		public NamespaceSupport()
		{
		} // <init>()

		//
		// Public methods
		//

		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.reset()"/>
		public virtual void reset()
		{

			// reset namespace and context info
			fNamespaceSize = 0;
			fCurrentContext = 0;
			fContext[fCurrentContext] = fNamespaceSize;

			// bind "xml" prefix to the XML uri
			fNamespace[fNamespaceSize++] = PREFIX_XML;
			fNamespace[fNamespaceSize++] = XML_URI;
			// bind "xmlns" prefix to the XMLNS uri
			fNamespace[fNamespaceSize++] = PREFIX_XMLNS;
			fNamespace[fNamespaceSize++] = XMLNS_URI;
			++fCurrentContext;

		} // reset(SymbolTable)


		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.pushContext()"/>
		public virtual void pushContext()
		{

			// extend the array, if necessary
			if (fCurrentContext + 1 == fContext.Length)
			{
				int[] contextarray = new int[fContext.Length * 2];
				Array.Copy(fContext, 0, contextarray, 0, fContext.Length);
				fContext = contextarray;
			}

			// push context
			fContext[++fCurrentContext] = fNamespaceSize;

		} // pushContext()


		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.popContext()"/>
		public virtual void popContext()
		{
			fNamespaceSize = fContext[fCurrentContext--];
		} // popContext()

		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.declarePrefix(String, String)"/>
		public virtual bool declarePrefix(string prefix, string uri)
		{
			// ignore "xml" and "xmlns" prefixes
			if (string.ReferenceEquals(prefix, PREFIX_XML) || string.ReferenceEquals(prefix, PREFIX_XMLNS))
			{
				return false;
			}

			// see if prefix already exists in current context
			for (int i = fNamespaceSize; i > fContext[fCurrentContext]; i -= 2)
			{
				//if (fNamespace[i - 2] == prefix) {
				if (fNamespace[i - 2].Equals(prefix))
				{
					// REVISIT: [Q] Should the new binding override the
					//          previously declared binding or should it
					//          it be ignored? -Ac
					// NOTE:    The SAX2 "NamespaceSupport" helper allows
					//          re-bindings with the new binding overwriting
					//          the previous binding. -Ac
					fNamespace[i - 1] = uri;
					return true;
				}
			}

			// resize array, if needed
			if (fNamespaceSize == fNamespace.Length)
			{
				string[] namespacearray = new string[fNamespaceSize * 2];
				Array.Copy(fNamespace, 0, namespacearray, 0, fNamespaceSize);
				fNamespace = namespacearray;
			}

			// bind prefix to uri in current context
			fNamespace[fNamespaceSize++] = prefix;
			fNamespace[fNamespaceSize++] = uri;

			return true;

		} // declarePrefix(String,String):boolean

		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.getURI(String)"/>
		public virtual string getURI(string prefix)
		{

			// find prefix in current context
			for (int i = fNamespaceSize; i > 0; i -= 2)
			{
				//if (fNamespace[i - 2] == prefix) {
				if (fNamespace[i - 2].Equals(prefix))
				{
					return fNamespace[i - 1];
				}
			}

			// prefix not found
			return null;

		} // getURI(String):String


		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.getPrefix(String)"/>
		public virtual string getPrefix(string uri)
		{

			// find uri in current context
			for (int i = fNamespaceSize; i > 0; i -= 2)
			{
				//if (fNamespace[i - 1] == uri) {
				if (fNamespace[i - 1].Equals(uri))
				{
					//if (getURI(fNamespace[i - 2]) == uri)
					if (getURI(fNamespace[i - 2]).Equals(uri))
					{
						return fNamespace[i - 2];
					}
				}
			}

			// uri not found
			return null;

		} // getPrefix(String):String


		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.getDeclaredPrefixCount()"/>
		public virtual int DeclaredPrefixCount
		{
			get
			{
				return (fNamespaceSize - fContext[fCurrentContext]) / 2;
			}
		} // getDeclaredPrefixCount():int

		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.getDeclaredPrefixAt(int)"/>
		public virtual string getDeclaredPrefixAt(int index)
		{
			return fNamespace[fContext[fCurrentContext] + index * 2];
		} // getDeclaredPrefixAt(int):String

		/// <seealso cref="org.apache.xerces.xni.NamespaceContext.getAllPrefixes()"/>
		public virtual System.Collections.IEnumerator AllPrefixes
		{
			get
			{
				int count = 0;
				if (fPrefixes.Length < (fNamespace.Length / 2))
				{
					// resize prefix array          
					string[] prefixes = new string[fNamespaceSize];
					fPrefixes = prefixes;
				}
				string prefix = null;
				bool unique = true;
				for (int i = 2; i < (fNamespaceSize-2); i += 2)
				{
					prefix = fNamespace[i + 2];
					for (int k = 0;k < count;k++)
					{
						if (string.ReferenceEquals(fPrefixes[k], prefix))
						{
							unique = false;
							break;
						}
					}
					if (unique)
					{
						fPrefixes[count++] = prefix;
					}
					unique = true;
				}
				return new Prefixes(this, fPrefixes, count);
			}
		}

		protected internal sealed class Prefixes : System.Collections.IEnumerator
		{
			private readonly NamespaceSupport outerInstance;

			internal string[] prefixes;
			internal int counter = 0;
			internal int size = 0;

			/// <summary>
			/// Constructor for Prefixes.
			/// </summary>
			public Prefixes(NamespaceSupport outerInstance, string[] prefixes, int size)
			{
				this.outerInstance = outerInstance;
				this.prefixes = prefixes;
				this.size = size;
			}

		   /// <seealso cref="java.util.Enumeration.hasMoreElements()"/>
			public bool hasMoreElements()
			{
				return (counter < size);
			}

			/// <seealso cref="java.util.Enumeration.nextElement()"/>
			public object nextElement()
			{
				if (counter < size)
				{
					return outerInstance.fPrefixes[counter++];
				}
				throw new NoSuchElementException("Illegal access to Namespace prefixes enumeration.");
			}

			public override string ToString()
			{
				StringBuilder buf = new StringBuilder();
				for (int i = 0;i < size;i++)
				{
					buf.Append(prefixes[i]);
					buf.Append(" ");
				}

				return buf.ToString();
			}

		}

	} // class NamespaceSupport

}