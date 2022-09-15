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
 * $Id: XSLTCDTMManager.java 468651 2006-10-28 07:04:25Z minchau $
 */
namespace org.apache.xalan.xsltc.dom
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMDefaultBase = org.apache.xml.dtm.@ref.DTMDefaultBase;
	using DTMException = org.apache.xml.dtm.DTMException;
	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using DTMManagerDefault = org.apache.xml.dtm.@ref.DTMManagerDefault;
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using DOM2SAX = org.apache.xalan.xsltc.trax.DOM2SAX;

	using InputSource = org.xml.sax.InputSource;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;
	using XMLReader = org.xml.sax.XMLReader;

	/// <summary>
	/// The default implementation for the DTMManager.
	/// </summary>
	public class XSLTCDTMManager : DTMManagerDefault
	{

		/// <summary>
		/// The default class name to use as the manager. </summary>
		private const string DEFAULT_CLASS_NAME = "org.apache.xalan.xsltc.dom.XSLTCDTMManager";

		private const string DEFAULT_PROP_NAME = "org.apache.xalan.xsltc.dom.XSLTCDTMManager";

		/// <summary>
		/// Set this to true if you want a dump of the DTM after creation </summary>
		private const bool DUMPTREE = false;

		/// <summary>
		/// Set this to true if you want basic diagnostics </summary>
		private const bool DEBUG = false;

		/// <summary>
		/// Constructor DTMManagerDefault
		/// 
		/// </summary>
		public XSLTCDTMManager() : base()
		{
		}

		/// <summary>
		/// Obtain a new instance of a <code>DTMManager</code>.
		/// This static method creates a new factory instance.
		/// The current implementation just returns a new XSLTCDTMManager instance.
		/// </summary>
		public static XSLTCDTMManager newInstance()
		{
			return new XSLTCDTMManager();
		}

		/// <summary>
		/// Look up the class that provides the XSLTC DTM Manager service.
		/// The following lookup procedure is used to find the service provider.
		/// <ol>
		/// <li>The value of the
		/// <code>org.apache.xalan.xsltc.dom.XSLTCDTMManager</code> property, is
		/// checked.</li>
		/// <li>The <code>xalan.propeties</code> file is checked for a property
		/// of the same name.</li>
		/// <li>The
		/// <code>META-INF/services/org.apache.xalan.xsltc.dom.XSLTCDTMManager</code>
		/// file is checked.
		/// </ol>
		/// The default is <code>org.apache.xalan.xsltc.dom.XSLTCDTMManager</code>.
		/// </summary>
		public static Type DTMManagerClass
		{
			get
			{
				Type mgrClass = ObjectFactory.lookUpFactoryClass(DEFAULT_PROP_NAME, null, DEFAULT_CLASS_NAME);
				// If no class found, default to this one.  (This should never happen -
				// the ObjectFactory has already been told that the current class is
				// the default).
				return (mgrClass != null) ? mgrClass : typeof(XSLTCDTMManager);
			}
		}

		/// <summary>
		/// Get an instance of a DTM, loaded with the content from the
		/// specified source.  If the unique flag is true, a new instance will
		/// always be returned.  Otherwise it is up to the DTMManager to return a
		/// new instance or an instance that it already created and may be being used
		/// by someone else.
		/// (I think more parameters will need to be added for error handling, and
		/// entity resolution).
		/// </summary>
		/// <param name="source"> the specification of the source object. </param>
		/// <param name="unique"> true if the returned DTM must be unique, probably because it
		/// is going to be mutated. </param>
		/// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
		///                         be null. </param>
		/// <param name="incremental"> true if the DTM should be built incrementally, if
		///                    possible. </param>
		/// <param name="doIndexing"> true if the caller considers it worth it to use
		///                   indexing schemes.
		/// </param>
		/// <returns> a non-null DTM reference. </returns>
		public override DTM getDTM(Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing)
		{
			return getDTM(source, unique, whiteSpaceFilter, incremental, doIndexing, false, 0, true, false);
		}

		/// <summary>
		/// Get an instance of a DTM, loaded with the content from the
		/// specified source.  If the unique flag is true, a new instance will
		/// always be returned.  Otherwise it is up to the DTMManager to return a
		/// new instance or an instance that it already created and may be being used
		/// by someone else.
		/// (I think more parameters will need to be added for error handling, and
		/// entity resolution).
		/// </summary>
		/// <param name="source"> the specification of the source object. </param>
		/// <param name="unique"> true if the returned DTM must be unique, probably because it
		/// is going to be mutated. </param>
		/// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
		///                         be null. </param>
		/// <param name="incremental"> true if the DTM should be built incrementally, if
		///                    possible. </param>
		/// <param name="doIndexing"> true if the caller considers it worth it to use
		///                   indexing schemes. </param>
		/// <param name="buildIdIndex"> true if the id index table should be built.
		/// </param>
		/// <returns> a non-null DTM reference. </returns>
		public virtual DTM getDTM(Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing, bool buildIdIndex)
		{
			return getDTM(source, unique, whiteSpaceFilter, incremental, doIndexing, false, 0, buildIdIndex, false);
		}

		/// <summary>
		/// Get an instance of a DTM, loaded with the content from the
		/// specified source.  If the unique flag is true, a new instance will
		/// always be returned.  Otherwise it is up to the DTMManager to return a
		/// new instance or an instance that it already created and may be being used
		/// by someone else.
		/// (I think more parameters will need to be added for error handling, and
		/// entity resolution).
		/// </summary>
		/// <param name="source"> the specification of the source object. </param>
		/// <param name="unique"> true if the returned DTM must be unique, probably because it
		/// is going to be mutated. </param>
		/// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
		///                         be null. </param>
		/// <param name="incremental"> true if the DTM should be built incrementally, if
		///                    possible. </param>
		/// <param name="doIndexing"> true if the caller considers it worth it to use
		///                   indexing schemes. </param>
		/// <param name="buildIdIndex"> true if the id index table should be built. </param>
		/// <param name="newNameTable"> true if we want to use a separate ExpandedNameTable
		///                     for this DTM.
		/// </param>
		/// <returns> a non-null DTM reference. </returns>
	  public virtual DTM getDTM(Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing, bool buildIdIndex, bool newNameTable)
	  {
		return getDTM(source, unique, whiteSpaceFilter, incremental, doIndexing, false, 0, buildIdIndex, newNameTable);
	  }

	  /// <summary>
	  /// Get an instance of a DTM, loaded with the content from the
	  /// specified source.  If the unique flag is true, a new instance will
	  /// always be returned.  Otherwise it is up to the DTMManager to return a
	  /// new instance or an instance that it already created and may be being used
	  /// by someone else.
	  /// (I think more parameters will need to be added for error handling, and
	  /// entity resolution).
	  /// </summary>
	  /// <param name="source"> the specification of the source object. </param>
	  /// <param name="unique"> true if the returned DTM must be unique, probably because it
	  /// is going to be mutated. </param>
	  /// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
	  ///                         be null. </param>
	  /// <param name="incremental"> true if the DTM should be built incrementally, if
	  ///                    possible. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  /// <param name="hasUserReader"> true if <code>source</code> is a
	  ///                      <code>SAXSource</code> object that has an
	  ///                      <code>XMLReader</code>, that was specified by the
	  ///                      user. </param>
	  /// <param name="size">  Specifies initial size of tables that represent the DTM </param>
	  /// <param name="buildIdIndex"> true if the id index table should be built.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
		public virtual DTM getDTM(Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing, bool hasUserReader, int size, bool buildIdIndex)
		{
		  return getDTM(source, unique, whiteSpaceFilter, incremental, doIndexing, hasUserReader, size, buildIdIndex, false);
		}

	  /// <summary>
	  /// Get an instance of a DTM, loaded with the content from the
	  /// specified source.  If the unique flag is true, a new instance will
	  /// always be returned.  Otherwise it is up to the DTMManager to return a
	  /// new instance or an instance that it already created and may be being used
	  /// by someone else.
	  /// (I think more parameters will need to be added for error handling, and
	  /// entity resolution).
	  /// </summary>
	  /// <param name="source"> the specification of the source object. </param>
	  /// <param name="unique"> true if the returned DTM must be unique, probably because it
	  /// is going to be mutated. </param>
	  /// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
	  ///                         be null. </param>
	  /// <param name="incremental"> true if the DTM should be built incrementally, if
	  ///                    possible. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use
	  ///                   indexing schemes. </param>
	  /// <param name="hasUserReader"> true if <code>source</code> is a
	  ///                      <code>SAXSource</code> object that has an
	  ///                      <code>XMLReader</code>, that was specified by the
	  ///                      user. </param>
	  /// <param name="size">  Specifies initial size of tables that represent the DTM </param>
	  /// <param name="buildIdIndex"> true if the id index table should be built. </param>
	  /// <param name="newNameTable"> true if we want to use a separate ExpandedNameTable
	  ///                     for this DTM.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public virtual DTM getDTM(Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing, bool hasUserReader, int size, bool buildIdIndex, bool newNameTable)
	  {
			if (DEBUG && null != source)
			{
				Console.WriteLine("Starting " + (unique ? "UNIQUE" : "shared") + " source: " + source.getSystemId());
			}

			int dtmPos = FirstFreeDTMID;
			int documentID = dtmPos << IDENT_DTM_NODE_BITS;

			if ((null != source) && source is DOMSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.dom.DOMSource domsrc = (javax.xml.transform.dom.DOMSource) source;
				DOMSource domsrc = (DOMSource) source;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node node = domsrc.getNode();
				org.w3c.dom.Node node = domsrc.getNode();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.trax.DOM2SAX dom2sax = new org.apache.xalan.xsltc.trax.DOM2SAX(node);
				DOM2SAX dom2sax = new DOM2SAX(node);

				SAXImpl dtm;

				if (size <= 0)
				{
					dtm = new SAXImpl(this, source, documentID, whiteSpaceFilter, null, doIndexing, DTMDefaultBase.DEFAULT_BLOCKSIZE, buildIdIndex, newNameTable);
				}
				else
				{
					dtm = new SAXImpl(this, source, documentID, whiteSpaceFilter, null, doIndexing, size, buildIdIndex, newNameTable);
				}

				dtm.DocumentURI = source.getSystemId();

				addDTM(dtm, dtmPos, 0);

				dom2sax.ContentHandler = dtm;

				try
				{
					dom2sax.parse();
				}
				catch (Exception re)
				{
					throw re;
				}
				catch (Exception e)
				{
					throw new org.apache.xml.utils.WrappedRuntimeException(e);
				}

				return dtm;
			}
			else
			{
				bool isSAXSource = (null != source) ? (source is SAXSource) : true;
				bool isStreamSource = (null != source) ? (source is StreamSource) : false;

				if (isSAXSource || isStreamSource)
				{
					XMLReader reader;
					InputSource xmlSource;

					if (null == source)
					{
						xmlSource = null;
						reader = null;
						hasUserReader = false; // Make sure the user didn't lie
					}
					else
					{
						reader = getXMLReader(source);
						xmlSource = SAXSource.sourceToInputSource(source);

						string urlOfSource = xmlSource.getSystemId();

						if (null != urlOfSource)
						{
							try
							{
								urlOfSource = SystemIDResolver.getAbsoluteURI(urlOfSource);
							}
							catch (Exception)
							{
								// %REVIEW% Is there a better way to send a warning?
								Console.Error.WriteLine("Can not absolutize URL: " + urlOfSource);
							}

							xmlSource.setSystemId(urlOfSource);
						}
					}

					// Create the basic SAX2DTM.
					SAXImpl dtm;
					if (size <= 0)
					{
						dtm = new SAXImpl(this, source, documentID, whiteSpaceFilter, null, doIndexing, DTMDefaultBase.DEFAULT_BLOCKSIZE, buildIdIndex, newNameTable);
					}
					else
					{
						dtm = new SAXImpl(this, source, documentID, whiteSpaceFilter, null, doIndexing, size, buildIdIndex, newNameTable);
					}

					// Go ahead and add the DTM to the lookup table.  This needs to be
					// done before any parsing occurs. Note offset 0, since we've just
					// created a new DTM.
					addDTM(dtm, dtmPos, 0);

					if (null == reader)
					{
						// Then the user will construct it themselves.
						return dtm;
					}

					reader.setContentHandler(dtm.Builder);

					if (!hasUserReader || null == reader.getDTDHandler())
					{
						reader.setDTDHandler(dtm);
					}

					if (!hasUserReader || null == reader.getErrorHandler())
					{
						reader.setErrorHandler(dtm);
					}

					try
					{
						reader.setProperty("http://xml.org/sax/properties/lexical-handler", dtm);
					}
					catch (SAXNotRecognizedException)
					{
					}
					catch (SAXNotSupportedException)
					{
					}

					try
					{
						reader.parse(xmlSource);
					}
					catch (Exception re)
					{
						throw re;
					}
					catch (Exception e)
					{
						throw new org.apache.xml.utils.WrappedRuntimeException(e);
					}
					finally
					{
						if (!hasUserReader)
						{
							releaseXMLReader(reader);
						}
					}

					if (DUMPTREE)
					{
						Console.WriteLine("Dumping SAX2DOM");
						dtm.dumpDTM(System.err);
					}

					return dtm;
				}
				else
				{
					// It should have been handled by a derived class or the caller
					// made a mistake.
					throw new DTMException(XMLMessages.createXMLMessage(XMLErrorResources.ER_NOT_SUPPORTED, new object[]{source}));
				}
			}
	  }
	}

}