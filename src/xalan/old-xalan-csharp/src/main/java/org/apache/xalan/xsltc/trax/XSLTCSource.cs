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
 * $Id: XSLTCSource.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{


	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using DOMWSFilter = org.apache.xalan.xsltc.dom.DOMWSFilter;
	using SAXImpl = org.apache.xalan.xsltc.dom.SAXImpl;
	using XSLTCDTMManager = org.apache.xalan.xsltc.dom.XSLTCDTMManager;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;

	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class XSLTCSource : Source
	{

		private string _systemId = null;
		private Source _source = null;
		private ThreadLocal _dom = new ThreadLocal();

		/// <summary>
		/// Create a new XSLTC-specific source from a system ID 
		/// </summary>
		public XSLTCSource(string systemId)
		{
			_systemId = systemId;
		}

		/// <summary>
		/// Create a new XSLTC-specific source from a JAXP Source
		/// </summary>
		public XSLTCSource(Source source)
		{
			_source = source;
		}

		/// <summary>
		/// Implements javax.xml.transform.Source.setSystemId()
		/// Set the system identifier for this Source. 
		/// This Source can get its input either directly from a file (in this case
		/// it will instanciate and use a JAXP parser) or it can receive it through
		/// ContentHandler/LexicalHandler interfaces. </summary>
		/// <param name="systemId"> The system Id for this Source </param>
		public string SystemId
		{
			set
			{
				_systemId = value;
				if (_source != null)
				{
					_source.SystemId = value;
				}
			}
			get
			{
			if (_source != null)
			{
				return _source.SystemId;
			}
			else
			{
				return (_systemId);
			}
			}
		}


		/// <summary>
		/// Internal interface which returns a DOM for a given DTMManager and translet.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected org.apache.xalan.xsltc.DOM getDOM(org.apache.xalan.xsltc.dom.XSLTCDTMManager dtmManager, org.apache.xalan.xsltc.runtime.AbstractTranslet translet) throws org.xml.sax.SAXException
		protected internal DOM getDOM(XSLTCDTMManager dtmManager, AbstractTranslet translet)
		{
			SAXImpl idom = (SAXImpl)_dom.get();

			if (idom != null)
			{
				if (dtmManager != null)
				{
					idom.migrateTo(dtmManager);
				}
			}
			else
			{
				Source source = _source;
				if (source == null)
				{
					if (!string.ReferenceEquals(_systemId, null) && _systemId.Length > 0)
					{
						source = new StreamSource(_systemId);
					}
					else
					{
						ErrorMsg err = new ErrorMsg(ErrorMsg.XSLTC_SOURCE_ERR);
						throw new SAXException(err.ToString());
					}
				}

				DOMWSFilter wsfilter = null;
				if (translet != null && translet is StripFilter)
				{
					wsfilter = new DOMWSFilter(translet);
				}

				bool hasIdCall = (translet != null) ? translet.hasIdCall() : false;

				if (dtmManager == null)
				{
					dtmManager = XSLTCDTMManager.newInstance();
				}

				idom = (SAXImpl)dtmManager.getDTM(source, true, wsfilter, false, false, hasIdCall);

				string systemId = SystemId;
				if (!string.ReferenceEquals(systemId, null))
				{
					idom.DocumentURI = systemId;
				}
				_dom.set(idom);
			}
			return idom;
		}

	}

}