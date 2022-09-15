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
 * $Id: SerializerUtils.java 468642 2006-10-28 06:55:10Z minchau $
 */
namespace org.apache.xalan.serialize
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTM = org.apache.xml.dtm.DTM;
	using NamespaceMappings = org.apache.xml.serializer.NamespaceMappings;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Class that contains only static methods that are used to "serialize",
	/// these methods are used by Xalan and are not in org.apache.xml.serializer
	/// because they have dependancies on the packages org.apache.xpath or org.
	/// apache.xml.dtm or org.apache.xalan.transformer. The package org.apache.xml.
	/// serializer should not depend on Xalan or XSLTC.
	/// @xsl.usage internal
	/// </summary>
	public class SerializerUtils
	{

		/// <summary>
		/// Copy an DOM attribute to the created output element, executing
		/// attribute templates as need be, and processing the xsl:use
		/// attribute.
		/// </summary>
		/// <param name="handler"> SerializationHandler to which the attributes are added. </param>
		/// <param name="attr"> Attribute node to add to SerializationHandler.
		/// </param>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void addAttribute(org.apache.xml.serializer.SerializationHandler handler, int attr) throws javax.xml.transform.TransformerException
		public static void addAttribute(SerializationHandler handler, int attr)
		{

			TransformerImpl transformer = (TransformerImpl) handler.Transformer;
			DTM dtm = transformer.XPathContext.getDTM(attr);

			if (SerializerUtils.isDefinedNSDecl(handler, attr, dtm))
			{
				return;
			}

			string ns = dtm.getNamespaceURI(attr);

			if (string.ReferenceEquals(ns, null))
			{
				ns = "";
			}

			// %OPT% ...can I just store the node handle?
			try
			{
				handler.addAttribute(ns, dtm.getLocalName(attr), dtm.getNodeName(attr), "CDATA", dtm.getNodeValue(attr), false);
			}
			catch (SAXException)
			{
				// do something?
			}
		} // end copyAttributeToTarget method

		/// <summary>
		/// Copy DOM attributes to the result element.
		/// </summary>
		/// <param name="src"> Source node with the attributes
		/// </param>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void addAttributes(org.apache.xml.serializer.SerializationHandler handler, int src) throws javax.xml.transform.TransformerException
		public static void addAttributes(SerializationHandler handler, int src)
		{

			TransformerImpl transformer = (TransformerImpl) handler.Transformer;
			DTM dtm = transformer.XPathContext.getDTM(src);

			for (int node = dtm.getFirstAttribute(src); DTM.NULL != node; node = dtm.getNextAttribute(node))
			{
				addAttribute(handler, node);
			}
		}

		/// <summary>
		/// Given a result tree fragment, walk the tree and
		/// output it to the SerializationHandler.
		/// </summary>
		/// <param name="obj"> Result tree fragment object </param>
		/// <param name="support"> XPath context for the result tree fragment
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void outputResultTreeFragment(org.apache.xml.serializer.SerializationHandler handler, org.apache.xpath.objects.XObject obj, org.apache.xpath.XPathContext support) throws org.xml.sax.SAXException
		public static void outputResultTreeFragment(SerializationHandler handler, XObject obj, XPathContext support)
		{

			int doc = obj.rtf();
			DTM dtm = support.getDTM(doc);

			if (null != dtm)
			{
				for (int n = dtm.getFirstChild(doc); DTM.NULL != n; n = dtm.getNextSibling(n))
				{
					handler.flushPending();

					// I think. . . . This used to have a (true) arg
					// to flush prefixes, will that cause problems ???
					if (dtm.getNodeType(n) == DTM.ELEMENT_NODE && string.ReferenceEquals(dtm.getNamespaceURI(n), null))
					{
						handler.startPrefixMapping("", "");
					}
					dtm.dispatchToEvents(n, handler);
				}
			}
		}

		/// <summary>
		/// Copy <KBD>xmlns:</KBD> attributes in if not already in scope.
		/// 
		/// As a quick hack to support ClonerToResultTree, this can also be used
		/// to copy an individual namespace node.
		/// </summary>
		/// <param name="src"> Source Node </param>
		/// NEEDSDOC <param name="type"> </param>
		/// NEEDSDOC <param name="dtm">
		/// </param>
		/// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void processNSDecls(org.apache.xml.serializer.SerializationHandler handler, int src, int type, org.apache.xml.dtm.DTM dtm) throws javax.xml.transform.TransformerException
		public static void processNSDecls(SerializationHandler handler, int src, int type, DTM dtm)
		{

			try
			{
				if (type == DTM.ELEMENT_NODE)
				{
					for (int @namespace = dtm.getFirstNamespaceNode(src, true); DTM.NULL != @namespace; @namespace = dtm.getNextNamespaceNode(src, @namespace, true))
					{

						// String prefix = dtm.getPrefix(namespace);
						string prefix = dtm.getNodeNameX(@namespace);
						string desturi = handler.getNamespaceURIFromPrefix(prefix);
						//            String desturi = getURI(prefix);
						string srcURI = dtm.getNodeValue(@namespace);

						if (!srcURI.Equals(desturi, StringComparison.OrdinalIgnoreCase))
						{
							handler.startPrefixMapping(prefix, srcURI, false);
						}
					}
				}
				else if (type == DTM.NAMESPACE_NODE)
				{
					string prefix = dtm.getNodeNameX(src);
					// Brian M. - some changes here to get desturi
					string desturi = handler.getNamespaceURIFromPrefix(prefix);
					string srcURI = dtm.getNodeValue(src);

					if (!srcURI.Equals(desturi, StringComparison.OrdinalIgnoreCase))
					{
						handler.startPrefixMapping(prefix, srcURI, false);
					}
				}
			}
			catch (SAXException se)
			{
				throw new TransformerException(se);
			}
		}

		/// <summary>
		/// Returns whether a namespace is defined
		/// 
		/// </summary>
		/// <param name="attr"> Namespace attribute node </param>
		/// <param name="dtm"> The DTM that owns attr.
		/// </param>
		/// <returns> True if the namespace is already defined in
		/// list of namespaces </returns>
		public static bool isDefinedNSDecl(SerializationHandler serializer, int attr, DTM dtm)
		{

			if (DTM.NAMESPACE_NODE == dtm.getNodeType(attr))
			{

				// String prefix = dtm.getPrefix(attr);
				string prefix = dtm.getNodeNameX(attr);
				string uri = serializer.getNamespaceURIFromPrefix(prefix);
				//      String uri = getURI(prefix);

				if ((null != uri) && uri.Equals(dtm.getStringValue(attr)))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// This function checks to make sure a given prefix is really
		/// declared.  It might not be, because it may be an excluded prefix.
		/// If it's not, it still needs to be declared at this point.
		/// TODO: This needs to be done at an earlier stage in the game... -sb
		/// </summary>
		/// NEEDSDOC <param name="dtm"> </param>
		/// NEEDSDOC <param name="namespace">
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void ensureNamespaceDeclDeclared(org.apache.xml.serializer.SerializationHandler handler, org.apache.xml.dtm.DTM dtm, int namespace) throws org.xml.sax.SAXException
		public static void ensureNamespaceDeclDeclared(SerializationHandler handler, DTM dtm, int @namespace)
		{

			string uri = dtm.getNodeValue(@namespace);
			string prefix = dtm.getNodeNameX(@namespace);

			if ((!string.ReferenceEquals(uri, null) && uri.Length > 0) && (null != prefix))
			{
				string foundURI;
				NamespaceMappings ns = handler.NamespaceMappings;
				if (ns != null)
				{

					foundURI = ns.lookupNamespace(prefix);
					if ((null == foundURI) || !foundURI.Equals(uri))
					{
						handler.startPrefixMapping(prefix, uri, false);
					}
				}
			}
		}
	}

}