using System.Collections;
using System.IO;

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
 * $Id: PipeDocument.java 468639 2006-10-28 06:52:33Z minchau $
 */
namespace org.apache.xalan.lib
{


	using XSLProcessorContext = org.apache.xalan.extensions.XSLProcessorContext;
	using AVT = org.apache.xalan.templates.AVT;
	using ElemExtensionCall = org.apache.xalan.templates.ElemExtensionCall;
	using ElemLiteralResult = org.apache.xalan.templates.ElemLiteralResult;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using XPathContext = org.apache.xpath.XPathContext;

	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	using SAXException = org.xml.sax.SAXException;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;
	// Imported Serializer classes
	using Serializer = org.apache.xml.serializer.Serializer;
	using SerializerFactory = org.apache.xml.serializer.SerializerFactory;

	/// <summary>
	/// PipeDocument is a Xalan extension element to set stylesheet params and pipes an XML 
	/// document through a series of 1 or more stylesheets.
	/// PipeDocument is invoked from a stylesheet as the <seealso cref="pipeDocument pipeDocument extension element"/>.
	/// 
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:pipe="http://xml.apache.org/xalan/PipeDocument"
	/// </pre>
	/// 
	/// @author Donald Leslie
	/// </summary>
	public class PipeDocument
	{
	/// <summary>
	/// Extension element for piping an XML document through a series of 1 or more transformations.
	/// 
	/// <pre>Common usage pattern: A stylesheet transforms a listing of documents to be
	/// transformed into a TOC. For each document in the listing calls the pipeDocument
	/// extension element to pipe that document through a series of 1 or more stylesheets 
	/// to the desired output document.
	/// 
	/// Syntax:
	/// &lt;xsl:stylesheet version="1.0"
	///                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	///                xmlns:pipe="http://xml.apache.org/xalan/PipeDocument"
	///                extension-element-prefixes="pipe"&gt;
	/// ...
	/// &lt;pipe:pipeDocument   source="source.xml" target="target.xml"&gt;
	///   &lt;stylesheet href="ss1.xsl"&gt;
	///     &lt;param name="param1" value="value1"/&gt;
	///   &lt;/stylesheet&gt;
	///   &lt;stylesheet href="ss2.xsl"&gt;
	///     &lt;param name="param1" value="value1"/&gt;
	///     &lt;param name="param2" value="value2"/&gt;
	///   &lt;/stylesheet&gt;
	///   &lt;stylesheet href="ss1.xsl"/&gt;     
	/// &lt;/pipe:pipeDocument&gt;
	/// 
	/// Notes:</pre>
	/// <ul>
	///   <li>The base URI for the source attribute is the XML "listing" document.<li/>
	///   <li>The target attribute is taken as is (base is the current user directory).<li/>
	///   <li>The stylsheet containg the extension element is the base URI for the
	///   stylesheet hrefs.<li/>
	/// </ul>
	/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void pipeDocument(org.apache.xalan.extensions.XSLProcessorContext context, org.apache.xalan.templates.ElemExtensionCall elem) throws TransformerException, TransformerConfigurationException, SAXException, IOException, java.io.FileNotFoundException
	  public virtual void pipeDocument(XSLProcessorContext context, ElemExtensionCall elem)
	  {

		  SAXTransformerFactory saxTFactory = (SAXTransformerFactory) TransformerFactory.newInstance();

		  // XML doc to transform.
		  string source = elem.getAttribute("source", context.ContextNode, context.Transformer);
		  TransformerImpl transImpl = context.Transformer;

		  //Base URI for input doc, so base for relative URI to XML doc to transform.
		  string baseURLOfSource = transImpl.BaseURLOfSource;
		  // Absolute URI for XML doc to transform.
		  string absSourceURL = SystemIDResolver.getAbsoluteURI(source, baseURLOfSource);

		  // Transformation target
		  string target = elem.getAttribute("target", context.ContextNode, context.Transformer);

		  XPathContext xctxt = context.Transformer.XPathContext;
		  int xt = xctxt.getDTMHandleFromNode(context.ContextNode);

		  // Get System Id for stylesheet; to be used to resolve URIs to other stylesheets.
		  string sysId = elem.SystemId;

		  NodeList ssNodes = null;
		  NodeList paramNodes = null;
		  Node ssNode = null;
		  Node paramNode = null;
		  if (elem.hasChildNodes())
		  {
			ssNodes = elem.ChildNodes;
			// Vector to contain TransformerHandler for each stylesheet.
			ArrayList vTHandler = new ArrayList(ssNodes.getLength());

			// The child nodes of an extension element node are instances of
			// ElemLiteralResult, which requires does not fully support the standard
			// Node interface. Accordingly, some special handling is required (see below)
			// to get attribute values.
			for (int i = 0; i < ssNodes.getLength(); i++)
			{
			  ssNode = ssNodes.item(i);
			  if (ssNode.getNodeType() == Node.ELEMENT_NODE && ((Element)ssNode).getTagName().Equals("stylesheet") && ssNode is ElemLiteralResult)
			  {
				AVT avt = ((ElemLiteralResult)ssNode).getLiteralResultAttribute("href");
				string href = avt.evaluate(xctxt,xt, elem);
				string absURI = SystemIDResolver.getAbsoluteURI(href, sysId);
				Templates tmpl = saxTFactory.newTemplates(new StreamSource(absURI));
				TransformerHandler tHandler = saxTFactory.newTransformerHandler(tmpl);
				Transformer trans = tHandler.getTransformer();

				// AddTransformerHandler to vector
				vTHandler.Add(tHandler);

				paramNodes = ssNode.getChildNodes();
				for (int j = 0; j < paramNodes.getLength(); j++)
				{
				  paramNode = paramNodes.item(j);
				  if (paramNode.getNodeType() == Node.ELEMENT_NODE && ((Element)paramNode).getTagName().Equals("param") && paramNode is ElemLiteralResult)
				  {
					 avt = ((ElemLiteralResult)paramNode).getLiteralResultAttribute("name");
					 string pName = avt.evaluate(xctxt,xt, elem);
					 avt = ((ElemLiteralResult)paramNode).getLiteralResultAttribute("value");
					 string pValue = avt.evaluate(xctxt,xt, elem);
					 trans.setParameter(pName, pValue);
				  }
				}
			  }
			}
			 usePipe(vTHandler, absSourceURL, target);
		  }
	  }
	  /// <summary>
	  /// Uses a Vector of TransformerHandlers to pipe XML input document through
	  /// a series of 1 or more transformations. Called by <seealso cref="pipeDocument"/>.
	  /// </summary>
	  /// <param name="vTHandler"> Vector of Transformation Handlers (1 per stylesheet). </param>
	  /// <param name="source"> absolute URI to XML input </param>
	  /// <param name="target"> absolute path to transformation output. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void usePipe(java.util.Vector vTHandler, String source, String target) throws TransformerException, TransformerConfigurationException, FileNotFoundException, IOException, SAXException, org.xml.sax.SAXNotRecognizedException
	  public virtual void usePipe(ArrayList vTHandler, string source, string target)
	  {
		XMLReader reader = XMLReaderFactory.createXMLReader();
		TransformerHandler tHFirst = (TransformerHandler)vTHandler[0];
		reader.setContentHandler(tHFirst);
		reader.setProperty("http://xml.org/sax/properties/lexical-handler", tHFirst);
		for (int i = 1; i < vTHandler.Count; i++)
		{
		  TransformerHandler tHFrom = (TransformerHandler)vTHandler[i - 1];
		  TransformerHandler tHTo = (TransformerHandler)vTHandler[i];
		  tHFrom.setResult(new SAXResult(tHTo));
		}
		TransformerHandler tHLast = (TransformerHandler)vTHandler[vTHandler.Count - 1];
		Transformer trans = tHLast.getTransformer();
		Properties outputProps = trans.getOutputProperties();
		Serializer serializer = SerializerFactory.getSerializer(outputProps);

		FileStream @out = new FileStream(target, FileMode.Create, FileAccess.Write);
		try
		{
		  serializer.OutputStream = @out;
		  tHLast.setResult(new SAXResult(serializer.asContentHandler()));
		  reader.parse(source);
		}
		finally
		{
		  // Always clean up the FileOutputStream,
		  // even if an exception was thrown in the try block
		  if (@out != null)
		  {
			@out.Close();
		  }
		}
	  }
	}

}