using System;
using System.Collections;

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
 * $Id: XMLReaderManager.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{


	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Creates XMLReader objects and caches them for re-use.
	/// This class follows the singleton pattern.
	/// </summary>
	public class XMLReaderManager
	{

		private const string NAMESPACES_FEATURE = "http://xml.org/sax/features/namespaces";
		private const string NAMESPACE_PREFIXES_FEATURE = "http://xml.org/sax/features/namespace-prefixes";
		private static readonly XMLReaderManager m_singletonManager = new XMLReaderManager();

		/// <summary>
		/// Parser factory to be used to construct XMLReader objects
		/// </summary>
		private static SAXParserFactory m_parserFactory;

		/// <summary>
		/// Cache of XMLReader objects
		/// </summary>
		private ThreadLocal m_readers;

		/// <summary>
		/// Keeps track of whether an XMLReader object is in use.
		/// </summary>
		private Hashtable m_inUse;

		/// <summary>
		/// Hidden constructor
		/// </summary>
		private XMLReaderManager()
		{
		}

		/// <summary>
		/// Retrieves the singleton reader manager
		/// </summary>
		public static XMLReaderManager Instance
		{
			get
			{
				return m_singletonManager;
			}
		}

		/// <summary>
		/// Retrieves a cached XMLReader for this thread, or creates a new
		/// XMLReader, if the existing reader is in use.  When the caller no
		/// longer needs the reader, it must release it with a call to
		/// <seealso cref="#releaseXMLReader"/>.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized org.xml.sax.XMLReader getXMLReader() throws org.xml.sax.SAXException
		public virtual XMLReader XMLReader
		{
			get
			{
				lock (this)
				{
					XMLReader reader;
					bool readerInUse;
            
					if (m_readers == null)
					{
						// When the m_readers.get() method is called for the first time
						// on a thread, a new XMLReader will automatically be created.
						m_readers = new ThreadLocal();
					}
            
					if (m_inUse == null)
					{
						m_inUse = new Hashtable();
					}
            
					// If the cached reader for this thread is in use, construct a new
					// one; otherwise, return the cached reader.
					reader = (XMLReader) m_readers.get();
					bool threadHasReader = (reader != null);
					if (!threadHasReader || m_inUse[reader] == true)
					{
						try
						{
							try
							{
								// According to JAXP 1.2 specification, if a SAXSource
								// is created using a SAX InputSource the Transformer or
								// TransformerFactory creates a reader via the
								// XMLReaderFactory if setXMLReader is not used
								reader = XMLReaderFactory.createXMLReader();
							}
							catch (Exception)
							{
							   try
							   {
									// If unable to create an instance, let's try to use
									// the XMLReader from JAXP
									if (m_parserFactory == null)
									{
										m_parserFactory = SAXParserFactory.newInstance();
										m_parserFactory.NamespaceAware = true;
									}
            
									reader = m_parserFactory.newSAXParser().XMLReader;
							   }
							   catch (ParserConfigurationException pce)
							   {
								   throw pce; // pass along pce
							   }
							}
							try
							{
								reader.setFeature(NAMESPACES_FEATURE, true);
								reader.setFeature(NAMESPACE_PREFIXES_FEATURE, false);
							}
							catch (SAXException)
							{
								// Try to carry on if we've got a parser that
								// doesn't know about namespace prefixes.
							}
						}
						catch (ParserConfigurationException ex)
						{
							throw new SAXException(ex);
						}
						catch (FactoryConfigurationError ex1)
						{
							throw new SAXException(ex1.ToString());
						}
						catch (System.MissingMethodException)
						{
						}
						catch (AbstractMethodError)
						{
						}
            
						// Cache the XMLReader if this is the first time we've created
						// a reader for this thread.
						if (!threadHasReader)
						{
							m_readers.set(reader);
							m_inUse[reader] = true;
						}
					}
					else
					{
						m_inUse[reader] = true;
					}
            
					return reader;
				}
			}
		}

		/// <summary>
		/// Mark the cached XMLReader as available.  If the reader was not
		/// actually in the cache, do nothing.
		/// </summary>
		/// <param name="reader"> The XMLReader that's being released. </param>
		public virtual void releaseXMLReader(XMLReader reader)
		{
			lock (this)
			{
				// If the reader that's being released is the cached reader
				// for this thread, remove it from the m_isUse list.
				if (m_readers.get() == reader && reader != null)
				{
					m_inUse.Remove(reader);
				}
			}
		}
	}

}