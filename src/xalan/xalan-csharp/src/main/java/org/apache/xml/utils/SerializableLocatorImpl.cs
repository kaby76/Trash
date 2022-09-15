using System;

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
 * $Id: SerializableLocatorImpl.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// The standard SAX implementation of LocatorImpl is not serializable,
	/// limiting its utility as "a persistent snapshot of a locator".
	/// This is a quick hack to make it so. Note that it makes more sense
	/// in many cases to set up fields to hold this data rather than pointing
	/// at another object... but that decision should be made on architectural
	/// grounds rather than serializability.
	/// <para>
	/// It isn't clear whether subclassing LocatorImpl and adding serialization
	/// methods makes more sense than copying it and just adding Serializable
	/// to its interface. Since it's so simple, I've taken the latter approach
	/// for now.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref="org.xml.sax.helpers.LocatorImpl"/>
	/// <seealso cref="org.xml.sax.Locator Locator"
	/// @since XalanJ2
	/// @author Joe Kesselman
	/// @version 1.0/>
	[Serializable]
	public class SerializableLocatorImpl : org.xml.sax.Locator

	{
		internal const long serialVersionUID = -2660312888446371460L;
		/// <summary>
		/// Zero-argument constructor.
		/// 
		/// <para>SAX says "This will not normally be useful, since the main purpose
		/// of this class is to make a snapshot of an existing Locator." In fact,
		/// it _is_ sometimes useful when you want to construct a new Locator
		/// pointing to a specific location... which, after all, is why the
		/// setter methods are provided.
		/// </para>
		/// </summary>
		public SerializableLocatorImpl()
		{
		}


		/// <summary>
		/// Copy constructor.
		/// 
		/// <para>Create a persistent copy of the current state of a locator.
		/// When the original locator changes, this copy will still keep
		/// the original values (and it can be used outside the scope of
		/// DocumentHandler methods).</para>
		/// </summary>
		/// <param name="locator"> The locator to copy. </param>
		public SerializableLocatorImpl(org.xml.sax.Locator locator)
		{
			PublicId = locator.getPublicId();
			SystemId = locator.getSystemId();
			LineNumber = locator.getLineNumber();
			ColumnNumber = locator.getColumnNumber();
		}


		////////////////////////////////////////////////////////////////////
		// Implementation of org.xml.sax.Locator
		////////////////////////////////////////////////////////////////////


		/// <summary>
		/// Return the saved public identifier.
		/// </summary>
		/// <returns> The public identifier as a string, or null if none
		///         is available. </returns>
		/// <seealso cref="org.xml.sax.Locator.getPublicId"/>
		/// <seealso cref=".setPublicId"/>
		public virtual string PublicId
		{
			get
			{
				return publicId;
			}
			set
			{
				this.publicId = value;
			}
		}


		/// <summary>
		/// Return the saved system identifier.
		/// </summary>
		/// <returns> The system identifier as a string, or null if none
		///         is available. </returns>
		/// <seealso cref="org.xml.sax.Locator.getSystemId"/>
		/// <seealso cref=".setSystemId"/>
		public virtual string SystemId
		{
			get
			{
				return systemId;
			}
			set
			{
				this.systemId = value;
			}
		}


		/// <summary>
		/// Return the saved line number (1-based).
		/// </summary>
		/// <returns> The line number as an integer, or -1 if none is available. </returns>
		/// <seealso cref="org.xml.sax.Locator.getLineNumber"/>
		/// <seealso cref=".setLineNumber"/>
		public virtual int LineNumber
		{
			get
			{
				return lineNumber;
			}
			set
			{
				this.lineNumber = value;
			}
		}


		/// <summary>
		/// Return the saved column number (1-based).
		/// </summary>
		/// <returns> The column number as an integer, or -1 if none is available. </returns>
		/// <seealso cref="org.xml.sax.Locator.getColumnNumber"/>
		/// <seealso cref=".setColumnNumber"/>
		public virtual int ColumnNumber
		{
			get
			{
				return columnNumber;
			}
			set
			{
				this.columnNumber = value;
			}
		}


		////////////////////////////////////////////////////////////////////
		// Setters for the properties (not in org.xml.sax.Locator)
		////////////////////////////////////////////////////////////////////










		////////////////////////////////////////////////////////////////////
		// Internal state.
		////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The public ID.
		/// @serial
		/// </summary>
		private string publicId;

		/// <summary>
		/// The system ID.
		/// @serial
		/// </summary>
		private string systemId;

		/// <summary>
		/// The line number.
		/// @serial
		/// </summary>
		private int lineNumber;

		/// <summary>
		/// The column number.
		/// @serial
		/// </summary>
		private int columnNumber;

	}

	// end of LocatorImpl.java

}