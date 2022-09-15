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
 * $Id: DocumentCache.java 1225369 2011-12-28 22:54:01Z mrglavas $
 */

namespace org.apache.xalan.xsltc.dom
{



	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using BasisLibrary = org.apache.xalan.xsltc.runtime.BasisLibrary;
	using Constants = org.apache.xalan.xsltc.runtime.Constants;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;

	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using XMLReader = org.xml.sax.XMLReader;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class DocumentCache : DOMCache
	{

		private int _size;
		private Hashtable _references;
		private string[] _URIs;
		private int _count;
		private int _current;
		private SAXParser _parser;
		private XMLReader _reader;
		private XSLTCDTMManager _dtmManager;

		private const int REFRESH_INTERVAL = 1000;

		/*
		 * Inner class containing a DOMImpl object and DTD handler
		 */
		public sealed class CachedDocument
		{
			private readonly DocumentCache outerInstance;


		// Statistics data
		internal long _firstReferenced;
		internal long _lastReferenced;
		internal long _accessCount;
		internal long _lastModified;
		internal long _lastChecked;
		internal long _buildTime;

		// DOM and DTD handler references
		internal DOMEnhancedForDTM _dom = null;

		/// <summary>
		/// Constructor - load document and initialise statistics
		/// </summary>
		public CachedDocument(DocumentCache outerInstance, string uri)
		{
			this.outerInstance = outerInstance;
			// Initialise statistics variables
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long stamp = System.currentTimeMillis();
			long stamp = DateTimeHelperClass.CurrentUnixTimeMillis();
			_firstReferenced = stamp;
			_lastReferenced = stamp;
			_accessCount = 0;
			loadDocument(uri);

			_buildTime = DateTimeHelperClass.CurrentUnixTimeMillis() - stamp;
		}

		/// <summary>
		/// Loads the document and updates build-time (latency) statistics
		/// </summary>
		public void loadDocument(string uri)
		{

			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long stamp = System.currentTimeMillis();
			long stamp = DateTimeHelperClass.CurrentUnixTimeMillis();
					_dom = (DOMEnhancedForDTM)outerInstance._dtmManager.getDTM(new SAXSource(outerInstance._reader, new InputSource(uri)), false, null, true, false);
			_dom.DocumentURI = uri;

			// The build time can be used for statistics for a better
			// priority algorithm (currently round robin).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long thisTime = System.currentTimeMillis() - stamp;
			long thisTime = DateTimeHelperClass.CurrentUnixTimeMillis() - stamp;
			if (_buildTime > 0)
			{
				_buildTime = (long)((ulong)(_buildTime + thisTime) >> 1);
			}
			else
			{
				_buildTime = thisTime;
			}
			}
			catch (Exception)
			{
			_dom = null;
			}
		}

		public DOM Document
		{
			get
			{
				return (_dom);
			}
		}

		public long FirstReferenced
		{
			get
			{
				return (_firstReferenced);
			}
		}

		public long LastReferenced
		{
			get
			{
				return (_lastReferenced);
			}
		}

		public long AccessCount
		{
			get
			{
				return (_accessCount);
			}
		}

		public void incAccessCount()
		{
			_accessCount++;
		}

		public long LastModified
		{
			get
			{
				return (_lastModified);
			}
			set
			{
				_lastModified = value;
			}
		}


		public long Latency
		{
			get
			{
				return (_buildTime);
			}
		}

		public long LastChecked
		{
			get
			{
				return (_lastChecked);
			}
			set
			{
				_lastChecked = value;
			}
		}


		public long EstimatedSize
		{
			get
			{
				if (_dom != null)
				{
				return (_dom.Size << 5); // ???
				}
				else
				{
				return (0);
				}
			}
		}

		}

		/// <summary>
		/// DocumentCache constructor
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DocumentCache(int size) throws org.xml.sax.SAXException
		public DocumentCache(int size) : this(size, null)
		{
			try
			{
				_dtmManager = (XSLTCDTMManager)XSLTCDTMManager.DTMManagerClass.newInstance();
			}
			catch (Exception e)
			{
				throw new SAXException(e);
			}
		}

		/// <summary>
		/// DocumentCache constructor
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DocumentCache(int size, XSLTCDTMManager dtmManager) throws org.xml.sax.SAXException
		public DocumentCache(int size, XSLTCDTMManager dtmManager)
		{
		_dtmManager = dtmManager;
		_count = 0;
		_current = 0;
		_size = size;
		_references = new Hashtable(_size+2);
		_URIs = new string[_size];

		try
		{
			// Create a SAX parser and get the XMLReader object it uses
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();
			SAXParserFactory factory = SAXParserFactory.newInstance();
			try
			{
			factory.setFeature(org.apache.xalan.xsltc.runtime.Constants_Fields.NAMESPACE_FEATURE,true);
			}
			catch (Exception)
			{
			factory.NamespaceAware = true;
			}
			_parser = factory.newSAXParser();
			_reader = _parser.XMLReader;
		}
		catch (ParserConfigurationException)
		{
			BasisLibrary.runTimeError(BasisLibrary.NAMESPACES_SUPPORT_ERR);
		}
		}

		/// <summary>
		/// Returns the time-stamp for a document's last update
		/// </summary>
		private long getLastModified(string uri)
		{
		try
		{
			URL url = new URL(uri);
			URLConnection connection = url.openConnection();
			long timestamp = connection.LastModified;
			// Check for a "file:" URI (courtesy of Brian Ewins)
			if (timestamp == 0)
			{ // get 0 for local URI
				if ("file".Equals(url.Protocol))
				{
					File localfile = new File(URLDecoder.decode(url.File));
					timestamp = localfile.lastModified();
				}
			}
			return (timestamp);
		}
		// Brutal handling of all exceptions
		catch (Exception)
		{
			return (DateTimeHelperClass.CurrentUnixTimeMillis());
		}
		}

		/// 
		private CachedDocument lookupDocument(string uri)
		{
		return ((CachedDocument)_references[uri]);
		}

		/// 
		private void insertDocument(string uri, CachedDocument doc)
		{
			lock (this)
			{
			if (_count < _size)
			{
				// Insert out URI in circular buffer
				_URIs[_count++] = uri;
				_current = 0;
			}
			else
			{
				// Remove oldest URI from reference Hashtable
				_references.Remove(_URIs[_current]);
				// Insert our URI in circular buffer
				_URIs[_current] = uri;
				if (++_current >= _size)
				{
					_current = 0;
				}
			}
			_references[uri] = doc;
			}
		}

		/// 
		private void replaceDocument(string uri, CachedDocument doc)
		{
			lock (this)
			{
			CachedDocument old = (CachedDocument)_references[uri];
			if (doc == null)
			{
				insertDocument(uri, doc);
			}
			else
			{
				_references[uri] = doc;
			}
			}
		}

		/// <summary>
		/// Returns a document either by finding it in the cache or
		/// downloading it and putting it in the cache.
		/// </summary>
		public DOM retrieveDocument(string baseURI, string href, Translet trs)
		{
		CachedDocument doc;

		string uri = href;
		if (!string.ReferenceEquals(baseURI, null) && baseURI.Length != 0)
		{
			try
			{
				uri = SystemIDResolver.getAbsoluteURI(uri, baseURI);
			}
			catch (TransformerException)
			{
				// ignore    
			}
		}

		// Try to get the document from the cache first
		if ((doc = lookupDocument(uri)) == null)
		{
			doc = new CachedDocument(this, uri);
			if (doc == null)
			{
				return null; // better error handling needed!!!
			}
			doc.LastModified = getLastModified(uri);
			insertDocument(uri, doc);
		}
		// If the document is in the cache we must check if it is still valid
		else
		{
			long now = DateTimeHelperClass.CurrentUnixTimeMillis();
			long chk = doc.LastChecked;
			doc.LastChecked = now;
			// Has the modification time for this file been checked lately?
			if (now > (chk + REFRESH_INTERVAL))
			{
			doc.LastChecked = now;
			long last = getLastModified(uri);
			// Reload document if it has been modified since last download
			if (last > doc.LastModified)
			{
				doc = new CachedDocument(this, uri);
				if (doc == null)
				{
					return null;
				}
				doc.LastModified = getLastModified(uri);
				replaceDocument(uri, doc);
			}
			}

		}

		// Get the references to the actual DOM and DTD handler
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.DOM dom = doc.getDocument();
		DOM dom = doc.Document;

		// The dom reference may be null if the URL pointed to a
		// non-existing document
		if (dom == null)
		{
			return null;
		}

		doc.incAccessCount(); // For statistics

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.runtime.AbstractTranslet translet = (org.apache.xalan.xsltc.runtime.AbstractTranslet)trs;
		AbstractTranslet translet = (AbstractTranslet)trs;

		// Give the translet an early opportunity to extract any
			// information from the DOM object that it would like.
		translet.prepassDocument(dom);

		return (doc.Document);
		}

		/// <summary>
		/// Outputs the cache statistics
		/// </summary>
		public void getStatistics(PrintWriter @out)
		{
		@out.println("<h2>DOM cache statistics</h2><center><table border=\"2\">" + "<tr><td><b>Document URI</b></td>" + "<td><center><b>Build time</b></center></td>" + "<td><center><b>Access count</b></center></td>" + "<td><center><b>Last accessed</b></center></td>" + "<td><center><b>Last modified</b></center></td></tr>");

		for (int i = 0; i < _count; i++)
		{
			CachedDocument doc = (CachedDocument)_references[_URIs[i]];
			@out.print("<tr><td><a href=\"" + _URIs[i] + "\">" + "<font size=-1>" + _URIs[i] + "</font></a></td>");
			@out.print("<td><center>" + doc.Latency + "ms</center></td>");
			@out.print("<td><center>" + doc.AccessCount + "</center></td>");
			@out.print("<td><center>" + (new DateTime(doc.LastReferenced)) + "</center></td>");
			@out.print("<td><center>" + (new DateTime(doc.LastModified)) + "</center></td>");
			@out.println("</tr>");
		}

		@out.println("</table></center>");
		}
	}

}