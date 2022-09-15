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
 * $Id: Util.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.trax
{




	using XSLTC = org.apache.xalan.xsltc.compiler.XSLTC;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;

	using Document = org.w3c.dom.Document;

	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using SAXNotRecognizedException = org.xml.sax.SAXNotRecognizedException;
	using SAXNotSupportedException = org.xml.sax.SAXNotSupportedException;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class Util
	{

		public static string baseName(string name)
		{
		return org.apache.xalan.xsltc.compiler.util.Util.baseName(name);
		}

		public static string noExtName(string name)
		{
		return org.apache.xalan.xsltc.compiler.util.Util.noExtName(name);
		}

		public static string toJavaName(string name)
		{
		return org.apache.xalan.xsltc.compiler.util.Util.toJavaName(name);
		}




		/// <summary>
		/// Creates a SAX2 InputSource object from a TrAX Source object
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.xml.sax.InputSource getInputSource(org.apache.xalan.xsltc.compiler.XSLTC xsltc, javax.xml.transform.Source source) throws javax.xml.transform.TransformerConfigurationException
		public static InputSource getInputSource(XSLTC xsltc, Source source)
		{
		InputSource input = null;

		string systemId = source.SystemId;

		try
		{
			// Try to get InputSource from SAXSource input
			if (source is SAXSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.sax.SAXSource sax = (javax.xml.transform.sax.SAXSource)source;
			SAXSource sax = (SAXSource)source;
			input = sax.InputSource;
			// Pass the SAX parser to the compiler
					try
					{
						XMLReader reader = sax.XMLReader;

						 /*
						  * Fix for bug 24695
						  * According to JAXP 1.2 specification if a SAXSource
						  * is created using a SAX InputSource the Transformer or
						  * TransformerFactory creates a reader via the
						  * XMLReaderFactory if setXMLReader is not used
						  */

						if (reader == null)
						{
						   try
						   {
							   reader = XMLReaderFactory.createXMLReader();
						   }
						   catch (Exception)
						   {
							   try
							   {

								   //Incase there is an exception thrown 
								   // resort to JAXP 
								   SAXParserFactory parserFactory = SAXParserFactory.newInstance();
								   parserFactory.NamespaceAware = true;

								   if (xsltc.SecureProcessing)
								   {
									  try
									  {
										  parserFactory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
									  }
									  catch (SAXException)
									  {
									  }
								   }

								   reader = parserFactory.newSAXParser().XMLReader;


							   }
							   catch (ParserConfigurationException pce)
							   {
								   throw new TransformerConfigurationException("ParserConfigurationException",pce);
							   }
						   }
						}
						reader.setFeature("http://xml.org/sax/features/namespaces",true);
						reader.setFeature("http://xml.org/sax/features/namespace-prefixes",false);

						xsltc.XMLReader = reader;
					}
					catch (SAXNotRecognizedException snre)
					{
					  throw new TransformerConfigurationException("SAXNotRecognizedException ",snre);
					}
					catch (SAXNotSupportedException snse)
					{
					  throw new TransformerConfigurationException("SAXNotSupportedException ",snse);
					}
					catch (SAXException se)
					{
					  throw new TransformerConfigurationException("SAXException ",se);
					}

			}
			// handle  DOMSource  
			else if (source is DOMSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.dom.DOMSource domsrc = (javax.xml.transform.dom.DOMSource)source;
			DOMSource domsrc = (DOMSource)source;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Document dom = (org.w3c.dom.Document)domsrc.getNode();
			Document dom = (Document)domsrc.Node;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DOM2SAX dom2sax = new DOM2SAX(dom);
			DOM2SAX dom2sax = new DOM2SAX(dom);
			xsltc.XMLReader = dom2sax;

				// Try to get SAX InputSource from DOM Source.
			input = SAXSource.sourceToInputSource(source);
			if (input == null)
			{
				input = new InputSource(domsrc.SystemId);
			}
			}
			// Try to get InputStream or Reader from StreamSource
			else if (source is StreamSource)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.transform.stream.StreamSource stream = (javax.xml.transform.stream.StreamSource)source;
			StreamSource stream = (StreamSource)source;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream istream = stream.getInputStream();
			System.IO.Stream istream = stream.InputStream;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Reader reader = stream.getReader();
			Reader reader = stream.Reader;
					xsltc.XMLReader = null; // Clear old XML reader

			// Create InputSource from Reader or InputStream in Source
			if (istream != null)
			{
				input = new InputSource(istream);
			}
			else if (reader != null)
			{
				input = new InputSource(reader);
			}
			else
			{
				input = new InputSource(systemId);
			}
			}
			else
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR);
			throw new TransformerConfigurationException(err.ToString());
			}
			input.SystemId = systemId;
		}
		catch (System.NullReferenceException)
		{
			 ErrorMsg err = new ErrorMsg(ErrorMsg.JAXP_NO_SOURCE_ERR, "TransformerFactory.newTemplates()");
			throw new TransformerConfigurationException(err.ToString());
		}
		catch (SecurityException)
		{
			 ErrorMsg err = new ErrorMsg(ErrorMsg.FILE_ACCESS_ERR, systemId);
			throw new TransformerConfigurationException(err.ToString());
		}
		return input;
		}

	}


}