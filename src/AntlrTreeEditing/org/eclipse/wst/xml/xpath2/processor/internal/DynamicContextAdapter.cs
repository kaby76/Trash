using System;
using System.Collections.Generic;
using javax.xml.datatype;
using javax.xml.@namespace;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, 2018 IBM Corporation and others.
/// This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     IBM Corporation - initial API and implementation
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using URI = java.net.URI;
	using GregorianCalendar = java.util.GregorianCalendar;

	using Duration = javax.xml.datatype.Duration;

	using CollationProvider = org.eclipse.wst.xml.xpath2.api.CollationProvider;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using FnCollection = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCollection;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
    using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;

	public class DynamicContextAdapter : org.eclipse.wst.xml.xpath2.api.DynamicContext
	{
		private readonly org.eclipse.wst.xml.xpath2.processor.DynamicContext dc;
		private StaticContextAdapter sca;

		public DynamicContextAdapter(org.eclipse.wst.xml.xpath2.processor.DynamicContext dc)
		{
			this.dc = dc;
			this.sca = new StaticContextAdapter(dc);
		}

		public virtual Node LimitNode
		{
			get
			{
				return null;
			}
		}

		public virtual ResultSequence getVariable(javax.xml.@namespace.QName name)
		{
			object @var = dc.get_variable(new QName(name));
			if (@var == null)
			{
				return ResultBuffer.EMPTY;
			}
			if (@var is ResultSequence)
			{
				return (ResultSequence)@var;
			}
			return ResultBuffer.wrap((Item)@var);
		}

		public virtual URI resolveUri(string uri)
		{
			return dc.resolve_uri(uri);
		}

		public virtual GregorianCalendar CurrentDateTime
		{
			get
			{
				return dc.current_date_time();
			}
		}

		public virtual Duration TimezoneOffset
		{
			get
			{
				XSDuration tz = dc.tz();
				try
				{
					return DatatypeFactory.newInstance().newDuration(!tz.negative(), 0, 0, 0, tz.hours(), tz.minutes(), 0);
				}
				catch (DatatypeConfigurationException)
				{
					throw;
				}
			}
		}

		public virtual Document getDocument(URI uri)
		{
			org.eclipse.wst.xml.xpath2.processor.ResultSequence rs = dc.get_doc(uri);
			if (rs == null || rs.empty())
			{
				return null;
			}
			return ((DocType)(rs.get(0))).value();
		}

		public virtual IDictionary<string, IList<Document>> Collections
		{
			get
			{
				return dc.get_collections();
			}
		}

		public virtual IList<Document> DefaultCollection
		{
			get
			{
				return Collections[FnCollection.DEFAULT_COLLECTION_URI];
			}
		}

		public virtual CollationProvider CollationProvider
		{
			get
			{
				return sca.CollationProvider;
			}
		}

	}
}