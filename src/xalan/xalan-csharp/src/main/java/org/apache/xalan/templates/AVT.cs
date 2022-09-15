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
 * $Id: AVT.java 469221 2006-10-30 18:26:44Z minchau $
 */
namespace org.apache.xalan.templates
{

	using StylesheetHandler = org.apache.xalan.processor.StylesheetHandler;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using StringBufferPool = org.apache.xml.utils.StringBufferPool;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Class to hold an Attribute Value Template.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class AVT : XSLTVisitable
	{
		internal const long serialVersionUID = 5167607155517042691L;

	  /// <summary>
	  /// We are not going to use the object pool if USE_OBJECT_POOL == false.
	  /// </summary>
	  private const bool USE_OBJECT_POOL = false;

	  /// <summary>
	  /// INIT_BUFFER_CHUNK_BITS is used to set initial size of
	  /// of the char m_array in FastStringBuffer if USE_OBJECT_POOL == false. 
	  /// size = 2^ INIT_BUFFER_CHUNK_BITS, INIT_BUFFER_CHUNK_BITS = 7 
	  /// corresponds size = 256. 
	  /// </summary>
	  private const int INIT_BUFFER_CHUNK_BITS = 8;

	  /// <summary>
	  /// If the AVT is not complex, just hold the simple string.
	  /// @serial
	  /// </summary>
	  private string m_simpleString = null;

	  /// <summary>
	  /// If the AVT is complex, hold a Vector of AVTParts.
	  /// @serial
	  /// </summary>
	  private ArrayList m_parts = null;



	  /// <summary>
	  /// The name of the attribute.
	  /// @serial
	  /// </summary>
	  private string m_rawName;

	  /// <summary>
	  /// Get the raw name of the attribute, with the prefix unprocessed.
	  /// </summary>
	  /// <returns> non-null reference to prefixed name. </returns>
	  public virtual string RawName
	  {
		  get
		  {
			return m_rawName;
		  }
		  set
		  {
			m_rawName = value;
		  }
	  }


	  /// <summary>
	  /// The name of the attribute.
	  /// @serial
	  /// </summary>
	  private string m_name;

	  /// <summary>
	  /// Get the local name of the attribute.
	  /// </summary>
	  /// <returns> non-null reference to name string. </returns>
	  public virtual string Name
	  {
		  get
		  {
			return m_name;
		  }
		  set
		  {
			m_name = value;
		  }
	  }


	  /// <summary>
	  /// The namespace URI of the owning attribute.
	  /// @serial
	  /// </summary>
	  private string m_uri;

	  /// <summary>
	  /// Get the namespace URI of the attribute.
	  /// </summary>
	  /// <returns> non-null reference to URI, "" if null namespace. </returns>
	  public virtual string URI
	  {
		  get
		  {
			return m_uri;
		  }
		  set
		  {
			m_uri = value;
		  }
	  }


	  /// <summary>
	  /// Construct an AVT by parsing the string, and either
	  /// constructing a vector of AVTParts, or simply hold
	  /// on to the string if the AVT is simple.
	  /// </summary>
	  /// <param name="handler"> non-null reference to StylesheetHandler that is constructing. </param>
	  /// <param name="uri"> non-null reference to URI, "" if null namespace. </param>
	  /// <param name="name">  non-null reference to name string. </param>
	  /// <param name="rawName"> prefixed name. </param>
	  /// <param name="stringedValue"> non-null raw string value.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public AVT(org.apache.xalan.processor.StylesheetHandler handler, String uri, String name, String rawName, String stringedValue, ElemTemplateElement owner) throws javax.xml.transform.TransformerException
	  public AVT(StylesheetHandler handler, string uri, string name, string rawName, string stringedValue, ElemTemplateElement owner)
	  {

		m_uri = uri;
		m_name = name;
		m_rawName = rawName;

		StringTokenizer tokenizer = new StringTokenizer(stringedValue, "{}\"\'", true);
		int nTokens = tokenizer.countTokens();

		if (nTokens < 2)
		{
		  m_simpleString = stringedValue; // then do the simple thing
		}
		else
		{
		  FastStringBuffer buffer = null;
		  FastStringBuffer exprBuffer = null;
		  if (USE_OBJECT_POOL)
		  {
			buffer = StringBufferPool.get();
			exprBuffer = StringBufferPool.get();
		  }
		  else
		  {
			buffer = new FastStringBuffer(6);
			exprBuffer = new FastStringBuffer(6);
		  }
		  try
		  {
			m_parts = new ArrayList(nTokens + 1);

			string t = null; // base token
			string lookahead = null; // next token
			string error = null; // if non-null, break from loop

			while (tokenizer.hasMoreTokens())
			{
			  if (!string.ReferenceEquals(lookahead, null))
			  {
				t = lookahead;
				lookahead = null;
			  }
			  else
			  {
				t = tokenizer.nextToken();
			  }

			  if (t.Length == 1)
			  {
				switch (t[0])
				{
				case ('\"') :
				case ('\'') :
				{

				  // just keep on going, since we're not in an attribute template
				  buffer.append(t);

				  break;
				}
				case ('{') :
				{

				  try
				  {
					// Attribute Value Template start
					lookahead = tokenizer.nextToken();

					if (lookahead.Equals("{"))
					{

					  // Double curlys mean escape to show curly
					  buffer.append(lookahead);

					  lookahead = null;

					  break; // from switch
					}

					/*
					else if(lookahead.equals("\"") || lookahead.equals("\'"))
					{
					// Error. Expressions can not begin with quotes.
					error = "Expressions can not begin with quotes.";
					break; // from switch
					}
					*/
					else
					{
					  if (buffer.length() > 0)
					  {
						m_parts.Add(new AVTPartSimple(buffer.ToString()));
						buffer.Length = 0;
					  }

					  exprBuffer.Length = 0;

					  while (null != lookahead)
					  {
						if (lookahead.Length == 1)
						{
						  switch (lookahead[0])
						  {
						  case '\'' :
						  case '\"' :
						  {

							  // String start
							  exprBuffer.append(lookahead);

							  string quote = lookahead;

							  // Consume stuff 'till next quote
							  lookahead = tokenizer.nextToken();

							  while (!lookahead.Equals(quote))
							  {
								exprBuffer.append(lookahead);

								lookahead = tokenizer.nextToken();
							  }

							  exprBuffer.append(lookahead);

							  lookahead = tokenizer.nextToken();

							  break;
						  }
						  case '{' :
						  {

							  // What's another curly doing here?
							  error = XSLMessages.createMessage(XSLTErrorResources.ER_NO_CURLYBRACE, null); //"Error: Can not have \"{\" within expression.";

							  lookahead = null; // breaks out of inner while loop

							  break;
						  }
						  case '}' :
						  {

							  // Proper close of attribute template.
							  // Evaluate the expression.
							  buffer.Length = 0;

							  XPath xpath = handler.createXPath(exprBuffer.ToString(), owner);

							  m_parts.Add(new AVTPartXPath(xpath));

							  lookahead = null; // breaks out of inner while loop

							  break;
						  }
						  default :
						  {

							  // part of the template stuff, just add it.
							  exprBuffer.append(lookahead);

							  lookahead = tokenizer.nextToken();
						  }
						break;
						  } // end inner switch
						} // end if lookahead length == 1
						else
						{

						  // part of the template stuff, just add it.
						  exprBuffer.append(lookahead);

						  lookahead = tokenizer.nextToken();
						}
					  } // end while(!lookahead.equals("}"))

					  if (!string.ReferenceEquals(error, null))
					  {
						break; // from inner while loop
					  }
					}

					break;
				  }
				  catch (java.util.NoSuchElementException)
				  {
					error = XSLMessages.createMessage(XSLTErrorResources.ER_ILLEGAL_ATTRIBUTE_VALUE, new object[]{name, stringedValue});
					break;
				  }
				}
					goto case ('}');
				case ('}') :
				{
				  lookahead = tokenizer.nextToken();

				  if (lookahead.Equals("}"))
				  {

					// Double curlys mean escape to show curly
					buffer.append(lookahead);

					lookahead = null; // swallow
				  }
				  else
				  {

					// Illegal, I think...
					try
					{
					  handler.warn(XSLTErrorResources.WG_FOUND_CURLYBRACE, null); //"Found \"}\" but no attribute template open!");
					}
					catch (org.xml.sax.SAXException se)
					{
					  throw new TransformerException(se);
					}

					buffer.append("}");

					// leave the lookahead to be processed by the next round.
				  }

				  break;
				}
				default :
				{

				  // Anything else just add to string.
				  buffer.append(t);
				}
			break;
				} // end switch t
			  } // end if length == 1
			  else
			  {

				// Anything else just add to string.
				buffer.append(t);
			  }

			  if (null != error)
			  {
				try
				{
				  handler.warn(XSLTErrorResources.WG_ATTR_TEMPLATE, new object[]{error}); //"Attr Template, "+error);
				}
				catch (org.xml.sax.SAXException se)
				{
				  throw new TransformerException(se);
				}

				break;
			  }
			} // end while(tokenizer.hasMoreTokens())

			if (buffer.length() > 0)
			{
			  m_parts.Add(new AVTPartSimple(buffer.ToString()));
			  buffer.Length = 0;
			}
		  }
		  finally
		  {
			if (USE_OBJECT_POOL)
			{
				 StringBufferPool.free(buffer);
				 StringBufferPool.free(exprBuffer);
			}
			 else
			 {
				buffer = null;
				exprBuffer = null;
			 };
		  }
		} // end else nTokens > 1

		if (null == m_parts && (null == m_simpleString))
		{

		  // Error?
		  m_simpleString = "";
		}
	  }

	  /// <summary>
	  /// Get the AVT as the original string.
	  /// </summary>
	  /// <returns> The AVT as the original string </returns>
	  public virtual string SimpleString
	  {
		  get
		  {
    
			if (null != m_simpleString)
			{
			  return m_simpleString;
			}
			else if (null != m_parts)
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.apache.xml.utils.FastStringBuffer buf = getBuffer();
			 FastStringBuffer buf = Buffer;
			 string @out = null;
    
			int n = m_parts.Count;
			try
			{
			  for (int i = 0; i < n; i++)
			  {
				AVTPart part = (AVTPart) m_parts[i];
				buf.append(part.SimpleString);
			  }
			  @out = buf.ToString();
			}
			finally
			{
			  if (USE_OBJECT_POOL)
			  {
				 StringBufferPool.free(buf);
			  }
			 else
			 {
				buf.Length = 0;
			 };
			}
			return @out;
			}
		  else
		  {
			  return "";
		  }
		  }
	  }

	  /// <summary>
	  /// Evaluate the AVT and return a String.
	  /// </summary>
	  /// <param name="xctxt"> Te XPathContext to use to evaluate this. </param>
	  /// <param name="context"> The current source tree context. </param>
	  /// <param name="nsNode"> The current namespace context (stylesheet tree context).
	  /// </param>
	  /// <returns> The AVT evaluated as a string
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String evaluate(org.apache.xpath.XPathContext xctxt, int context, org.apache.xml.utils.PrefixResolver nsNode) throws javax.xml.transform.TransformerException
	  public virtual string evaluate(XPathContext xctxt, int context, org.apache.xml.utils.PrefixResolver nsNode)
	  {
		if (null != m_simpleString)
		{
			return m_simpleString;
		}
		else if (null != m_parts)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.utils.FastStringBuffer buf =getBuffer();
		  FastStringBuffer buf = Buffer;
		  string @out = null;
		  int n = m_parts.Count;
		  try
		  {
			for (int i = 0; i < n; i++)
			{
			  AVTPart part = (AVTPart) m_parts[i];
			  part.evaluate(xctxt, buf, context, nsNode);
			}
		   @out = buf.ToString();
		  }
		  finally
		  {
			  if (USE_OBJECT_POOL)
			  {
				 StringBufferPool.free(buf);
			  }
			 else
			 {
			   buf.Length = 0;
			 }
		  }
		 return @out;
		}
		else
		{
		  return "";
		}
	  }

	  /// <summary>
	  /// Test whether the AVT is insensitive to the context in which
	  ///  it is being evaluated. This is intended to facilitate
	  ///  compilation of templates, by allowing simple AVTs to be
	  ///  converted back into strings.
	  /// 
	  ///  Currently the only case we recognize is simple strings.
	  /// ADDED 9/5/2000 to support compilation experiment
	  /// </summary>
	  /// <returns> True if the m_simpleString member of this AVT is not null </returns>
	  public virtual bool ContextInsensitive
	  {
		  get
		  {
			return null != m_simpleString;
		  }
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	  public virtual bool canTraverseOutsideSubtree()
	  {

		if (null != m_parts)
		{
		  int n = m_parts.Count;

		  for (int i = 0; i < n; i++)
		  {
			AVTPart part = (AVTPart) m_parts[i];

			if (part.canTraverseOutsideSubtree())
			{
			  return true;
			}
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame 
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public virtual void fixupVariables(ArrayList vars, int globalsSize)
	  {
		if (null != m_parts)
		{
		  int n = m_parts.Count;

		  for (int i = 0; i < n; i++)
		  {
			AVTPart part = (AVTPart) m_parts[i];

			part.fixupVariables(vars, globalsSize);
		  }
		}
	  }

	  /// <seealso cref="XSLTVisitable.callVisitors(XSLTVisitor)"/>
	  public virtual void callVisitors(XSLTVisitor visitor)
	  {
		  if (visitor.visitAVT(this) && (null != m_parts))
		  {
		  int n = m_parts.Count;

		  for (int i = 0; i < n; i++)
		  {
			AVTPart part = (AVTPart) m_parts[i];

			part.callVisitors(visitor);
		  }
		  }
	  }


	  /// <summary>
	  /// Returns true if this AVT is simple
	  /// </summary>
	  public virtual bool Simple
	  {
		  get
		  {
			  return !string.ReferenceEquals(m_simpleString, null);
		  }
	  }

	  private FastStringBuffer Buffer
	  {
		  get
		  {
			if (USE_OBJECT_POOL)
			{
			  return StringBufferPool.get();
			}
			else
			{
			  return new FastStringBuffer(INIT_BUFFER_CHUNK_BITS);
			}
		  }
	  }
	}

}