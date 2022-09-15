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
 * $Id: TrAXFilter.java 468653 2006-10-28 07:07:05Z minchau $
 */


namespace org.apache.xalan.xsltc.trax
{


	using XMLReaderManager = org.apache.xml.utils.XMLReaderManager;

	using ContentHandler = org.xml.sax.ContentHandler;
	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLFilterImpl = org.xml.sax.helpers.XMLFilterImpl;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// skeleton extension of XMLFilterImpl for now.  
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller 
	/// </summary>
	public class TrAXFilter : XMLFilterImpl
	{
		private Templates _templates;
		private TransformerImpl _transformer;
		private TransformerHandlerImpl _transformerHandler;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TrAXFilter(javax.xml.transform.Templates templates) throws javax.xml.transform.TransformerConfigurationException
		public TrAXFilter(Templates templates)
		{
		_templates = templates;
		_transformer = (TransformerImpl) templates.newTransformer();
			_transformerHandler = new TransformerHandlerImpl(_transformer);
		}

		public virtual Transformer Transformer
		{
			get
			{
				return _transformer;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void createParent() throws org.xml.sax.SAXException
		private void createParent()
		{
		XMLReader parent = null;
			try
			{
				SAXParserFactory pfactory = SAXParserFactory.newInstance();
				pfactory.NamespaceAware = true;

				if (_transformer.SecureProcessing)
				{
					try
					{
						pfactory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
					}
					catch (SAXException)
					{
					}
				}

				SAXParser saxparser = pfactory.newSAXParser();
				parent = saxparser.XMLReader;
			}
			catch (ParserConfigurationException e)
			{
				throw new SAXException(e);
			}
			catch (FactoryConfigurationError e)
			{
				throw new SAXException(e.ToString());
			}

			if (parent == null)
			{
				parent = XMLReaderFactory.createXMLReader();
			}

			// make this XMLReader the parent of this filter
			Parent = parent;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(org.xml.sax.InputSource input) throws org.xml.sax.SAXException, java.io.IOException
		public virtual void parse(InputSource input)
		{
			XMLReader managedReader = null;

			try
			{
				if (Parent == null)
				{
					try
					{
						managedReader = XMLReaderManager.Instance.XMLReader;
						Parent = managedReader;
					}
					catch (SAXException e)
					{
						throw new SAXException(e.ToString());
					}
				}

				// call parse on the parent
				Parent.parse(input);
			}
			finally
			{
				if (managedReader != null)
				{
					XMLReaderManager.Instance.releaseXMLReader(managedReader);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void parse(String systemId) throws org.xml.sax.SAXException, java.io.IOException
		public virtual void parse(string systemId)
		{
			parse(new InputSource(systemId));
		}

		public virtual ContentHandler ContentHandler
		{
			set
			{
			_transformerHandler.Result = new SAXResult(value);
			if (Parent == null)
			{
						try
						{
							createParent();
						}
						catch (SAXException)
						{
						   return;
						}
			}
			Parent.ContentHandler = _transformerHandler;
			}
		}

		public virtual ErrorListener ErrorListener
		{
			set
			{
			}
		}
	}

}