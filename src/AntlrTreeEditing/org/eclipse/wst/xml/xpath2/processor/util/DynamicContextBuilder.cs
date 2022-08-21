using System;
using System.Collections.Generic;
using System.IO;
using javax.xml.datatype;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, Jesper Steen Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
///     Jesper Steen Moller - bug 343804 - Updated API information
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.util
{
    using MalformedURLException = java.net.MalformedURLException;
	using URI = java.net.URI;
	using GregorianCalendar = java.util.GregorianCalendar;
	using TimeZone = java.util.TimeZone;

	using Duration = javax.xml.datatype.Duration;
	using QName = javax.xml.@namespace.QName;

	using CollationProvider = org.eclipse.wst.xml.xpath2.api.CollationProvider;
	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using FnCollection = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCollection;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;


	/// <summary>
	/// An implementation of a Dynamic Context.
	/// 
	/// Initializes and provides functionality of a dynamic context according to the
	/// XPath 2.0 specification.
	/// 
	/// @since 2.0
	/// </summary>
	public class DynamicContextBuilder : DynamicContext
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
        {
            throw new Exception();
//			_tz = _datatypeFactory.newDuration(_systemTimezone.RawOffset);
		}


		private static DatatypeFactory _datatypeFactory;
		static DynamicContextBuilder()
		{
			try
			{
				_datatypeFactory = DatatypeFactory.newInstance();
			}
			catch (DatatypeConfigurationException e)
			{
				throw new Exception("Cannot initialize XML datatypes", e);
			}
		}
		private TimeZone _systemTimezone = TimeZone.Default;

		private Duration _tz;
		private GregorianCalendar _currentDateTime;

		private IDictionary<QName, ResultSequence> _variables = new Dictionary<QName, ResultSequence>();
		private readonly StaticContext _staticContext;

		private IDictionary<string, IList<Document>> _collections;

		private IDictionary<URI, Document> _loaded_documents = new Dictionary<URI, Document>();

		public DynamicContextBuilder(StaticContext sc)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			_staticContext = sc;
		}

		/// <summary>
		/// Reads the day from a TimeDuration type
		/// </summary>
		/// <returns> an xs:integer _tz </returns>
		public virtual Duration TimezoneOffset
		{
			get
			{
				return _tz;
			}
		}

		/// <summary>
		/// Gets the Current stable date time from the dynamic context.
		/// </summary>
		public virtual GregorianCalendar CurrentDateTime
		{
			get
			{
				if (_currentDateTime == null)
				{
					_currentDateTime = new GregorianCalendar(TimeZone.getTimeZone("GMT"));
				}
				return _currentDateTime;
			}
		}

		public virtual Node LimitNode
		{
			get
			{
				return null;
			}
		}

		public virtual ResultSequence getVariable(QName name)
		{
			return _variables[name];
		}

		public virtual Document getDocument(URI resolved)
		{
			Document doc = null;
			if (_loaded_documents.ContainsKey(resolved))
			{
				 //tried before
				doc = _loaded_documents[resolved];
			}
			else
			{
				doc = retrieve_doc(resolved);
				_loaded_documents[resolved] = doc;
			}
			return doc;
		}

		// XXX make it nice, and move it out as a utility function
		private Document retrieve_doc(URI uri)
		{
			try
			{
				throw new Exception();
				//DOMLoader loader = new XercesLoader();
				//loader.set_validating(false);

				//Document doc = loader.load((new URL(uri.ToString())).openStream());
				//doc.DocumentURI = uri.ToString();
				//return doc;
			}
			catch (FileNotFoundException)
			{
				return null;
			}
			catch (MalformedURLException)
			{
				return null;
			}
			catch (IOException)
			{
				return null;
			}
		}

		public virtual URI resolveUri(string uri)
		{
			try
			{
				URI realURI = URI.create(uri);
				if (realURI.Absolute)
				{
					return realURI;
				}
				return _staticContext.BaseUri.resolve(uri);
			}
			catch (System.ArgumentException)
			{
				return null;
			}
		}

		public virtual IDictionary<string, IList<Document>> Collections
		{
			get
			{
				return _collections;
			}
		}

		public virtual IList<Document> DefaultCollection
		{
			get
			{
				return Collections[FnCollection.DEFAULT_COLLECTION_URI];
			}
		}

		public virtual DynamicContextBuilder withVariable(QName qName, ResultSequence values)
		{
			this._variables[qName] = values;
			return this;
		}

		public virtual DynamicContextBuilder withTimezoneOffset(Duration d)
		{
			this._tz = d;
			return this;
		}

		public virtual void withCollections(IDictionary<string, IList<Document>> map)
		{
			this._collections = map;
		}

		public virtual CollationProvider CollationProvider
		{
			get
			{
				return _staticContext.CollationProvider;
			}
		}
	}

}