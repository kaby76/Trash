using System;
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
 * $Id: TransletOutputHandlerFactory.java 475979 2006-11-16 23:32:48Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime.output
{

	using SAX2DOM = org.apache.xalan.xsltc.trax.SAX2DOM;
	using ToHTMLStream = org.apache.xml.serializer.ToHTMLStream;
	using ToTextStream = org.apache.xml.serializer.ToTextStream;
	using ToUnknownStream = org.apache.xml.serializer.ToUnknownStream;
	using ToXMLSAXHandler = org.apache.xml.serializer.ToXMLSAXHandler;
	using ToXMLStream = org.apache.xml.serializer.ToXMLStream;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using Node = org.w3c.dom.Node;

	using ContentHandler = org.xml.sax.ContentHandler;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class TransletOutputHandlerFactory
	{

		public const int STREAM = 0;
		public const int SAX = 1;
		public const int DOM = 2;

		private string _encoding = "utf-8";
		private string _method = null;
		private int _outputType = STREAM;
		private Stream _ostream = System.Console.Out;
		private Writer _writer = null;
		private Node _node = null;
		private Node _nextSibling = null;
		private int _indentNumber = -1;
		private ContentHandler _handler = null;
		private LexicalHandler _lexHandler = null;

		public static TransletOutputHandlerFactory newInstance()
		{
		return new TransletOutputHandlerFactory();
		}

		public virtual int OutputType
		{
			set
			{
			_outputType = value;
			}
		}

		public virtual string Encoding
		{
			set
			{
			if (!string.ReferenceEquals(value, null))
			{
				_encoding = value;
			}
			}
		}

		public virtual string OutputMethod
		{
			set
			{
			_method = value;
			}
		}

		public virtual Stream OutputStream
		{
			set
			{
			_ostream = value;
			}
		}

		public virtual Writer Writer
		{
			set
			{
			_writer = value;
			}
		}

		public virtual ContentHandler Handler
		{
			set
			{
				_handler = value;
			}
		}

		public virtual LexicalHandler LexicalHandler
		{
			set
			{
			_lexHandler = value;
			}
		}

		public virtual Node Node
		{
			set
			{
			_node = value;
			}
			get
			{
			return (_handler is SAX2DOM) ? ((SAX2DOM)_handler).DOM : null;
			}
		}


		public virtual Node NextSibling
		{
			set
			{
				_nextSibling = value;
			}
		}

		public virtual int IndentNumber
		{
			set
			{
			_indentNumber = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler getSerializationHandler() throws IOException, javax.xml.parsers.ParserConfigurationException
		public virtual SerializationHandler SerializationHandler
		{
			get
			{
				SerializationHandler result = null;
				switch (_outputType)
				{
					case STREAM :
    
						if (string.ReferenceEquals(_method, null))
						{
							result = new ToUnknownStream();
						}
						else if (_method.Equals("xml", StringComparison.OrdinalIgnoreCase))
						{
    
							result = new ToXMLStream();
    
						}
						else if (_method.Equals("html", StringComparison.OrdinalIgnoreCase))
						{
    
							result = new ToHTMLStream();
    
						}
						else if (_method.Equals("text", StringComparison.OrdinalIgnoreCase))
						{
    
							result = new ToTextStream();
    
						}
    
						if (result != null && _indentNumber >= 0)
						{
							result.IndentAmount = _indentNumber;
						}
    
						result.Encoding = _encoding;
    
						if (_writer != null)
						{
							result.Writer = _writer;
						}
						else
						{
							result.OutputStream = _ostream;
						}
						return result;
    
					case DOM :
						_handler = (_node != null) ? new SAX2DOM(_node, _nextSibling) : new SAX2DOM();
						_lexHandler = (LexicalHandler) _handler;
						// falls through
						goto case SAX;
					case SAX :
						if (string.ReferenceEquals(_method, null))
						{
							_method = "xml"; // default case
						}
    
						if (_lexHandler == null)
						{
							result = new ToXMLSAXHandler(_handler, _encoding);
						}
						else
						{
							result = new ToXMLSAXHandler(_handler, _lexHandler, _encoding);
						}
    
						return result;
				}
				return null;
			}
		}

	}

}