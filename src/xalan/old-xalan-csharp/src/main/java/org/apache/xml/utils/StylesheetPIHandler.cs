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
 * $Id: StylesheetPIHandler.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{



	using Attributes = org.xml.sax.Attributes;
	using InputSource = org.xml.sax.InputSource;
	using DefaultHandler = org.xml.sax.helpers.DefaultHandler;

	/// <summary>
	/// Search for the xml-stylesheet processing instructions in an XML document. </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xml-stylesheet/">Associating Style Sheets with XML documents, Version 1.0</a> </seealso>
	public class StylesheetPIHandler : DefaultHandler
	{
	  /// <summary>
	  /// The baseID of the document being processed. </summary>
	  internal string m_baseID;

	  /// <summary>
	  /// The desired media criteria. </summary>
	  internal string m_media;

	  /// <summary>
	  /// The desired title criteria. </summary>
	  internal string m_title;

	  /// <summary>
	  /// The desired character set criteria. </summary>
	  internal string m_charset;

	  /// <summary>
	  /// A list of SAXSource objects that match the criteria. </summary>
	  internal ArrayList m_stylesheets = new ArrayList();

	  // Add code to use a URIResolver. Patch from Dmitri Ilyin. 

	  /// <summary>
	  /// The object that implements the URIResolver interface,
	  /// or null.
	  /// </summary>
	  internal URIResolver m_uriResolver;

	  /// <summary>
	  /// Get the object that will be used to resolve URIs in href 
	  /// in xml-stylesheet processing instruction.
	  /// </summary>
	  /// <param name="resolver"> An object that implements the URIResolver interface,
	  /// or null. </param>
	  public virtual URIResolver URIResolver
	  {
		  set
		  {
			m_uriResolver = value;
		  }
		  get
		  {
			return m_uriResolver;
		  }
	  }


	  /// <summary>
	  /// Construct a StylesheetPIHandler instance that will search 
	  /// for xml-stylesheet PIs based on the given criteria.
	  /// </summary>
	  /// <param name="baseID"> The base ID of the XML document, needed to resolve 
	  ///               relative IDs. </param>
	  /// <param name="media"> The desired media criteria. </param>
	  /// <param name="title"> The desired title criteria. </param>
	  /// <param name="charset"> The desired character set criteria. </param>
	  public StylesheetPIHandler(string baseID, string media, string title, string charset)
	  {

		m_baseID = baseID;
		m_media = media;
		m_title = title;
		m_charset = charset;
	  }

	  /// <summary>
	  /// Return the last stylesheet found that match the constraints.
	  /// </summary>
	  /// <returns> Source object that references the last stylesheet reference 
	  ///         that matches the constraints. </returns>
	  public virtual Source AssociatedStylesheet
	  {
		  get
		  {
    
			int sz = m_stylesheets.Count;
    
			if (sz > 0)
			{
			  Source source = (Source) m_stylesheets[sz - 1];
			  return source;
			}
			else
			{
			  return null;
			}
		  }
	  }

	  /// <summary>
	  /// Handle the xml-stylesheet processing instruction.
	  /// </summary>
	  /// <param name="target"> The processing instruction target. </param>
	  /// <param name="data"> The processing instruction data, or null if
	  ///             none is supplied. </param>
	  /// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= org.xml.sax.ContentHandler#processingInstruction </seealso>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xml-stylesheet/">Associating Style Sheets with XML documents, Version 1.0</a> </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
	  public virtual void processingInstruction(string target, string data)
	  {

		if (target.Equals("xml-stylesheet"))
		{
		  string href = null; // CDATA #REQUIRED
		  string type = null; // CDATA #REQUIRED
		  string title = null; // CDATA #IMPLIED
		  string media = null; // CDATA #IMPLIED
		  string charset = null; // CDATA #IMPLIED
		  bool alternate = false; // (yes|no) "no"
		  StringTokenizer tokenizer = new StringTokenizer(data, " \t=\n", true);
		  bool lookedAhead = false;
		  Source source = null;

		  string token = "";
		  while (tokenizer.hasMoreTokens())
		  {
			if (!lookedAhead)
			{
			  token = tokenizer.nextToken();
			}
			else
			{
			  lookedAhead = false;
			}
			if (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			{
			  continue;
			}

			string name = token;
			if (name.Equals("type"))
			{
			  token = tokenizer.nextToken();
			  while (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			  {
				token = tokenizer.nextToken();
			  }
			  type = token.Substring(1, (token.Length - 1) - 1);

			}
			else if (name.Equals("href"))
			{
			  token = tokenizer.nextToken();
			  while (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			  {
				token = tokenizer.nextToken();
			  }
			  href = token;
			  if (tokenizer.hasMoreTokens())
			  {
				token = tokenizer.nextToken();
				// If the href value has parameters to be passed to a 
				// servlet(something like "foobar?id=12..."), 
				// we want to make sure we get them added to
				// the href value. Without this check, we would move on 
				// to try to process another attribute and that would be
				// wrong.
				// We need to set lookedAhead here to flag that we
				// already have the next token. 
				while (token.Equals("=") && tokenizer.hasMoreTokens())
				{
				  href = href + token + tokenizer.nextToken();
				  if (tokenizer.hasMoreTokens())
				  {
					token = tokenizer.nextToken();
					lookedAhead = true;
				  }
				  else
				  {
					break;
				  }
				}
			  }
			  href = href.Substring(1, (href.Length - 1) - 1);
			  try
			  {
				// Add code to use a URIResolver. Patch from Dmitri Ilyin. 
				if (m_uriResolver != null)
				{
				  source = m_uriResolver.resolve(href, m_baseID);
				}
			   else
			   {
				  href = SystemIDResolver.getAbsoluteURI(href, m_baseID);
				  source = new SAXSource(new InputSource(href));
			   }
			  }
			  catch (TransformerException te)
			  {
				throw new org.xml.sax.SAXException(te);
			  }
			}
			else if (name.Equals("title"))
			{
			  token = tokenizer.nextToken();
			  while (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			  {
				token = tokenizer.nextToken();
			  }
			  title = token.Substring(1, (token.Length - 1) - 1);
			}
			else if (name.Equals("media"))
			{
			  token = tokenizer.nextToken();
			  while (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			  {
				token = tokenizer.nextToken();
			  }
			  media = token.Substring(1, (token.Length - 1) - 1);
			}
			else if (name.Equals("charset"))
			{
			  token = tokenizer.nextToken();
			  while (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			  {
				token = tokenizer.nextToken();
			  }
			  charset = token.Substring(1, (token.Length - 1) - 1);
			}
			else if (name.Equals("alternate"))
			{
			  token = tokenizer.nextToken();
			  while (tokenizer.hasMoreTokens() && (token.Equals(" ") || token.Equals("\t") || token.Equals("=")))
			  {
				token = tokenizer.nextToken();
			  }
			  alternate = token.Substring(1, (token.Length - 1) - 1).Equals("yes");
			}

		  }

		  if ((null != type) && (type.Equals("text/xsl") || type.Equals("text/xml") || type.Equals("application/xml+xslt")) && (null != href))
		  {
			if (null != m_media)
			{
			  if (null != media)
			  {
				if (!media.Equals(m_media))
				{
				  return;
				}
			  }
			  else
			  {
				return;
			  }
			}

			if (null != m_charset)
			{
			  if (null != charset)
			  {
				if (!charset.Equals(m_charset))
				{
				  return;
				}
			  }
			  else
			  {
				return;
			  }
			}

			if (null != m_title)
			{
			  if (null != title)
			  {
				if (!title.Equals(m_title))
				{
				  return;
				}
			  }
			  else
			  {
				return;
			  }
			}

			m_stylesheets.Add(source);
		  }
		}
	  }


	  /// <summary>
	  /// The spec notes that "The xml-stylesheet processing instruction is allowed only in the prolog of an XML document.",
	  /// so, at least for right now, I'm going to go ahead an throw a TransformerException
	  /// in order to stop the parse.
	  /// </summary>
	  /// <param name="namespaceURI"> The Namespace URI, or an empty string. </param>
	  /// <param name="localName"> The local name (without prefix), or empty string if not namespace processing. </param>
	  /// <param name="qName"> The qualified name (with prefix). </param>
	  /// <param name="atts">  The specified or defaulted attributes.
	  /// </param>
	  /// <exception cref="StopParseException"> since there can be no valid xml-stylesheet processing 
	  ///                            instructions past the first element. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String qName, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
	  public virtual void startElement(string namespaceURI, string localName, string qName, Attributes atts)
	  {
		throw new StopParseException();
	  }

	  /// <summary>
	  /// Added additional getter and setter methods for the Base Id
	  /// to fix bugzilla bug 24187
	  /// 
	  /// </summary>
	   public virtual string BaseId
	   {
		   set
		   {
			   m_baseID = value;
    
		   }
		   get
		   {
			   return m_baseID;
		   }
	   }

	}

}