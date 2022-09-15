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
 * $Id: SerializerFactory.java 468642 2006-10-28 06:55:10Z minchau $
 */
namespace org.apache.xalan.serialize
{


	using Node = org.w3c.dom.Node;
	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// Factory for creating serializers. </summary>
	/// @deprecated The new class to use is 
	/// org.apache.xml.serializer.SerializerFactory 
	public abstract class SerializerFactory
	{

		private SerializerFactory()
		{
		}
		/// <summary>
		/// Returns a serializer for the specified output method. Returns
		/// null if no implementation exists that supports the specified
		/// output method. For a list of the default output methods see
		/// <seealso cref="org.apache.xml.serializer.Method"/>.
		/// </summary>
		/// <param name="format"> The output format </param>
		/// <returns> A suitable serializer, or null </returns>
		/// <exception cref="IllegalArgumentException"> (apparently -sc) if method is
		/// null or an appropriate serializer can't be found </exception>
		/// <exception cref="WrappedRuntimeException"> (apparently -sc) if an
		/// exception is thrown while trying to find serializer </exception>
		/// @deprecated Use org.apache.xml.serializer.SerializerFactory 
		public static Serializer getSerializer(Properties format)
		{
			org.apache.xml.serializer.Serializer ser;
			ser = org.apache.xml.serializer.SerializerFactory.getSerializer(format);
			SerializerFactory.SerializerWrapper si = new SerializerWrapper(ser);
			return si;

		}

		/// <summary>
		/// This class just exists to wrap a new Serializer in the new package by
		/// an old one.
		/// @deprecated
		/// </summary>

		private class SerializerWrapper : Serializer
		{
			internal readonly org.apache.xml.serializer.Serializer m_serializer;
			internal DOMSerializer m_old_DOMSerializer;

			internal SerializerWrapper(org.apache.xml.serializer.Serializer ser)
			{
				m_serializer = ser;

			}

			public virtual System.IO.Stream OutputStream
			{
				set
				{
					m_serializer.OutputStream = value;
				}
				get
				{
					return m_serializer.OutputStream;
				}
			}


			public virtual Writer Writer
			{
				set
				{
					m_serializer.Writer = value;
				}
				get
				{
					return m_serializer.Writer;
				}
			}


			public virtual Properties OutputFormat
			{
				set
				{
					m_serializer.OutputFormat = value;
				}
				get
				{
					return m_serializer.OutputFormat;
				}
			}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.xml.sax.ContentHandler asContentHandler() throws java.io.IOException
			public virtual ContentHandler asContentHandler()
			{
				return m_serializer.asContentHandler();
			}

			/// <returns> an old style DOMSerializer that wraps a new one. </returns>
			/// <seealso cref= org.apache.xalan.serialize.Serializer#asDOMSerializer() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DOMSerializer asDOMSerializer() throws java.io.IOException
			public virtual DOMSerializer asDOMSerializer()
			{
				if (m_old_DOMSerializer == null)
				{
					m_old_DOMSerializer = new DOMSerializerWrapper(m_serializer.asDOMSerializer());
				}
				return m_old_DOMSerializer;
			}
			/// <seealso cref= org.apache.xalan.serialize.Serializer#reset() </seealso>
			public virtual bool reset()
			{
				return m_serializer.reset();
			}

		}

		/// <summary>
		/// This class just wraps a new DOMSerializer with an old style one for
		/// migration purposes. 
		/// 
		/// </summary>
		private class DOMSerializerWrapper : DOMSerializer
		{
			internal readonly org.apache.xml.serializer.DOMSerializer m_dom;
			internal DOMSerializerWrapper(org.apache.xml.serializer.DOMSerializer domser)
			{
				m_dom = domser;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialize(org.w3c.dom.Node node) throws java.io.IOException
			public virtual void serialize(Node node)
			{
				m_dom.serialize(node);
			}
		}

	}

}