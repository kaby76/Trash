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
 * $Id: LoadDocument.java 1225369 2011-12-28 22:54:01Z mrglavas $
 */

namespace org.apache.xalan.xsltc.dom
{

	using DOM = org.apache.xalan.xsltc.DOM;
	using DOMCache = org.apache.xalan.xsltc.DOMCache;
	using DOMEnhancedForDTM = org.apache.xalan.xsltc.DOMEnhancedForDTM;
	using TransletException = org.apache.xalan.xsltc.TransletException;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using TemplatesImpl = org.apache.xalan.xsltc.trax.TemplatesImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using EmptyIterator = org.apache.xml.dtm.@ref.EmptyIterator;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class LoadDocument
	{

		private const string NAMESPACE_FEATURE = "http://xml.org/sax/features/namespaces";

		/// <summary>
		/// Interprets the arguments passed from the document() function (see
		/// org/apache/xalan/xsltc/compiler/DocumentCall.java) and returns an
		/// iterator containing the requested nodes. Builds a union-iterator if
		/// several documents are requested.
		/// 2 arguments arg1 and arg2.  document(Obj, node-set) call 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xml.dtm.DTMAxisIterator documentF(Object arg1, org.apache.xml.dtm.DTMAxisIterator arg2, String xslURI, org.apache.xalan.xsltc.runtime.AbstractTranslet translet, org.apache.xalan.xsltc.DOM dom) throws org.apache.xalan.xsltc.TransletException
		public static DTMAxisIterator documentF(object arg1, DTMAxisIterator arg2, string xslURI, AbstractTranslet translet, DOM dom)
		{
			string baseURI = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int arg2FirstNode = arg2.next();
			int arg2FirstNode = arg2.next();
			if (arg2FirstNode == DTMAxisIterator.END)
			{
				//  the second argument node-set is empty
				return EmptyIterator.Instance;
			}
			else
			{
				//System.err.println("arg2FirstNode name: "
				//                   + dom.getNodeName(arg2FirstNode )+"["
				//                   +Integer.toHexString(arg2FirstNode )+"]");
				baseURI = dom.getDocumentURI(arg2FirstNode);
				if (!SystemIDResolver.isAbsoluteURI(baseURI))
				{
				   baseURI = SystemIDResolver.getAbsoluteURIFromRelative(baseURI);
				}
			}

			try
			{
				if (arg1 is string)
				{
					if (((string)arg1).Length == 0)
					{
						return document(xslURI, "", translet, dom);
					}
					else
					{
						return document((string)arg1, baseURI, translet, dom);
					}
				}
				else if (arg1 is DTMAxisIterator)
				{
					return document((DTMAxisIterator)arg1, baseURI, translet, dom);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String err = "document("+arg1.toString()+")";
					string err = "document(" + arg1.ToString() + ")";
					throw new System.ArgumentException(err);
				}
			}
			catch (Exception e)
			{
				throw new TransletException(e);
			}
		}
		/// <summary>
		/// Interprets the arguments passed from the document() function (see
		/// org/apache/xalan/xsltc/compiler/DocumentCall.java) and returns an
		/// iterator containing the requested nodes. Builds a union-iterator if
		/// several documents are requested.
		/// 1 arguments arg.  document(Obj) call
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xml.dtm.DTMAxisIterator documentF(Object arg, String xslURI, org.apache.xalan.xsltc.runtime.AbstractTranslet translet, org.apache.xalan.xsltc.DOM dom) throws org.apache.xalan.xsltc.TransletException
		public static DTMAxisIterator documentF(object arg, string xslURI, AbstractTranslet translet, DOM dom)
		{
			try
			{
				if (arg is string)
				{
					if (string.ReferenceEquals(xslURI, null))
					{
						xslURI = "";
					}

					string baseURI = xslURI;
					if (!SystemIDResolver.isAbsoluteURI(xslURI))
					{
					   baseURI = SystemIDResolver.getAbsoluteURIFromRelative(xslURI);
					}

					string href = (string)arg;
					if (href.Length == 0)
					{
						href = "";
						// %OPT% Optimization to cache the stylesheet DOM.
						// The stylesheet DOM is built once and cached
						// in the Templates object.
						TemplatesImpl templates = (TemplatesImpl)translet.Templates;
						DOM sdom = null;
						if (templates != null)
						{
							sdom = templates.StylesheetDOM;
						}

						// If the cached dom exists, we need to migrate it
						// to the new DTMManager and create a DTMAxisIterator
						// for the document.
						if (sdom != null)
						{
							return document(sdom, translet, dom);
						}
						else
						{
							return document(href, baseURI, translet, dom, true);
						}
					}
					else
					{
						return document(href, baseURI, translet, dom);
					}
				}
				else if (arg is DTMAxisIterator)
				{
					return document((DTMAxisIterator)arg, null, translet, dom);
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String err = "document("+arg.toString()+")";
					string err = "document(" + arg.ToString() + ")";
					throw new System.ArgumentException(err);
				}
			}
			catch (Exception e)
			{
				throw new TransletException(e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static org.apache.xml.dtm.DTMAxisIterator document(String uri, String super, org.apache.xalan.xsltc.runtime.AbstractTranslet translet, org.apache.xalan.xsltc.DOM dom) throws Exception
		private static DTMAxisIterator document(string uri, string @base, AbstractTranslet translet, DOM dom)
		{
			return document(uri, @base, translet, dom, false);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static org.apache.xml.dtm.DTMAxisIterator document(String uri, String super, org.apache.xalan.xsltc.runtime.AbstractTranslet translet, org.apache.xalan.xsltc.DOM dom, boolean cacheDOM) throws Exception
		private static DTMAxisIterator document(string uri, string @base, AbstractTranslet translet, DOM dom, bool cacheDOM)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String originalUri = uri;
			string originalUri = uri;
			MultiDOM multiplexer = (MultiDOM)dom;

			// Prepend URI base to URI (from context)
			if (!string.ReferenceEquals(@base, null) && @base.Length != 0)
			{
				uri = SystemIDResolver.getAbsoluteURI(uri, @base);
			}

			// Return an empty iterator if the URI is clearly invalid
			// (to prevent some unncessary MalformedURL exceptions).
			if (string.ReferenceEquals(uri, null) || uri.Length == 0)
			{
				return (EmptyIterator.Instance);
			}

			// Check if this DOM has already been added to the multiplexer
			int mask = multiplexer.getDocumentMask(uri);
			if (mask != -1)
			{
				DOM newDom = ((DOMAdapter)multiplexer.getDOMAdapter(uri)).DOMImpl;
				if (newDom is DOMEnhancedForDTM)
				{
					return new SingletonIterator(((DOMEnhancedForDTM)newDom).Document, true);
				}
			}

			// Check if we can get the DOM from a DOMCache
			DOMCache cache = translet.DOMCache;
			DOM newdom;

			mask = multiplexer.nextMask(); // peek

			if (cache != null)
			{
				newdom = cache.retrieveDocument(@base, originalUri, translet);
				if (newdom == null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Exception e = new java.io.FileNotFoundException(originalUri);
					Exception e = new FileNotFoundException(originalUri);
					throw new TransletException(e);
				}
			}
			else
			{
				// Parse the input document and construct DOM object
				// Trust the DTMManager to pick the right parser and
				// set up the DOM correctly.
				XSLTCDTMManager dtmManager = (XSLTCDTMManager)multiplexer.DTMManager;
				DOMEnhancedForDTM enhancedDOM = (DOMEnhancedForDTM) dtmManager.getDTM(new StreamSource(uri), false, null, true, false, translet.hasIdCall(), cacheDOM);
				newdom = enhancedDOM;

				// Cache the stylesheet DOM in the Templates object
				if (cacheDOM)
				{
					TemplatesImpl templates = (TemplatesImpl)translet.Templates;
					if (templates != null)
					{
						templates.StylesheetDOM = enhancedDOM;
					}
				}

				translet.prepassDocument(enhancedDOM);
				enhancedDOM.DocumentURI = uri;
			}

			// Wrap the DOM object in a DOM adapter and add to multiplexer
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DOMAdapter domAdapter = translet.makeDOMAdapter(newdom);
			DOMAdapter domAdapter = translet.makeDOMAdapter(newdom);
			multiplexer.addDOMAdapter(domAdapter);

			// Create index for any key elements
			translet.buildKeys(domAdapter, null, null, newdom.Document);

			// Return a singleton iterator containing the root node
			return new SingletonIterator(newdom.Document, true);
			}
			catch (Exception e)
			{
				throw e;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static org.apache.xml.dtm.DTMAxisIterator document(org.apache.xml.dtm.DTMAxisIterator arg1, String baseURI, org.apache.xalan.xsltc.runtime.AbstractTranslet translet, org.apache.xalan.xsltc.DOM dom) throws Exception
		private static DTMAxisIterator document(DTMAxisIterator arg1, string baseURI, AbstractTranslet translet, DOM dom)
		{
			UnionIterator union = new UnionIterator(dom);
			int node = DTM.NULL;

			while ((node = arg1.next()) != DTM.NULL)
			{
				string uri = dom.getStringValueX(node);
				//document(node-set) if true;  document(node-set,node-set) if false
				if (string.ReferenceEquals(baseURI, null))
				{
				   baseURI = dom.getDocumentURI(node);
				   if (!SystemIDResolver.isAbsoluteURI(baseURI))
				   {
						baseURI = SystemIDResolver.getAbsoluteURIFromRelative(baseURI);
				   }
				}
				union.addIterator(document(uri, baseURI, translet, dom));
			}
			return (union);
		}

		/// <summary>
		/// Create a DTMAxisIterator for the newdom. This is currently only
		/// used to create an iterator for the cached stylesheet DOM.
		/// </summary>
		/// <param name="newdom"> the cached stylesheet DOM </param>
		/// <param name="translet"> the translet </param>
		/// <param name="the"> main dom (should be a MultiDOM) </param>
		/// <returns> a DTMAxisIterator from the document root </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static org.apache.xml.dtm.DTMAxisIterator document(org.apache.xalan.xsltc.DOM newdom, org.apache.xalan.xsltc.runtime.AbstractTranslet translet, org.apache.xalan.xsltc.DOM dom) throws Exception
		private static DTMAxisIterator document(DOM newdom, AbstractTranslet translet, DOM dom)
		{
			DTMManager dtmManager = ((MultiDOM)dom).DTMManager;
			// Need to migrate the cached DTM to the new DTMManager
			if (dtmManager != null && newdom is DTM)
			{
				((DTM)newdom).migrateTo(dtmManager);
			}

			translet.prepassDocument(newdom);

			// Wrap the DOM object in a DOM adapter and add to multiplexer
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DOMAdapter domAdapter = translet.makeDOMAdapter(newdom);
			DOMAdapter domAdapter = translet.makeDOMAdapter(newdom);
			((MultiDOM)dom).addDOMAdapter(domAdapter);

			// Create index for any key elements
			translet.buildKeys(domAdapter, null, null, newdom.Document);

			// Return a singleton iterator containing the root node
			return new SingletonIterator(newdom.Document, true);
		}

	}

}