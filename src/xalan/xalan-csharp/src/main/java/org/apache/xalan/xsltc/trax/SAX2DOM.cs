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
 * $Id: SAX2DOM.java 468653 2006-10-28 07:07:05Z minchau $
 */


namespace org.apache.xalan.xsltc.trax
{



	using Constants = org.apache.xalan.xsltc.runtime.Constants;

	using Comment = org.w3c.dom.Comment;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using Text = org.w3c.dom.Text;
	using ProcessingInstruction = org.w3c.dom.ProcessingInstruction;
	using Attributes = org.xml.sax.Attributes;
	using ContentHandler = org.xml.sax.ContentHandler;
	using Locator = org.xml.sax.Locator;
	using SAXException = org.xml.sax.SAXException;
	using LexicalHandler = org.xml.sax.ext.LexicalHandler;

	/// <summary>
	/// @author G. Todd Miller 
	/// </summary>
	public class SAX2DOM : ContentHandler, LexicalHandler, Constants
	{

		private Node _root = null;
		private Document _document = null;
		private Node _nextSibling = null;
		private System.Collections.Stack _nodeStk = new System.Collections.Stack();
		private ArrayList _namespaceDecls = null;
		private Node _lastSibling = null;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public SAX2DOM() throws javax.xml.parsers.ParserConfigurationException
		public SAX2DOM()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.DocumentBuilderFactory factory = javax.xml.parsers.DocumentBuilderFactory.newInstance();
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		_document = factory.newDocumentBuilder().newDocument();
		_root = _document;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public SAX2DOM(org.w3c.dom.Node root, org.w3c.dom.Node nextSibling) throws javax.xml.parsers.ParserConfigurationException
		public SAX2DOM(Node root, Node nextSibling)
		{
		_root = root;
		if (root is Document)
		{
		  _document = (Document)root;
		}
		else if (root != null)
		{
		  _document = root.getOwnerDocument();
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.DocumentBuilderFactory factory = javax.xml.parsers.DocumentBuilderFactory.newInstance();
		  DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		  _document = factory.newDocumentBuilder().newDocument();
		  _root = _document;
		}

		_nextSibling = nextSibling;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public SAX2DOM(org.w3c.dom.Node root) throws javax.xml.parsers.ParserConfigurationException
		public SAX2DOM(Node root) : this(root, null)
		{
		}

		public virtual Node DOM
		{
			get
			{
			return _root;
			}
		}

		public virtual void characters(char[] ch, int start, int length)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node last = (org.w3c.dom.Node)_nodeStk.peek();
		Node last = (Node)_nodeStk.Peek();

		// No text nodes can be children of root (DOM006 exception)
			if (last != _document)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String text = new String(ch, start, length);
				string text = new string(ch, start, length);
				if (_lastSibling != null && _lastSibling.getNodeType() == Node.TEXT_NODE)
				{
					  ((Text)_lastSibling).appendData(text);
				}
				else if (last == _root && _nextSibling != null)
				{
					_lastSibling = last.insertBefore(_document.createTextNode(text), _nextSibling);
				}
				else
				{
					_lastSibling = last.appendChild(_document.createTextNode(text));
				}

			}
		}

		public virtual void startDocument()
		{
		_nodeStk.Push(_root);
		}

		public virtual void endDocument()
		{
			_nodeStk.Pop();
		}

		public virtual void startElement(string @namespace, string localName, string qName, Attributes attrs)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Element tmp = (org.w3c.dom.Element)_document.createElementNS(namespace, qName);
		Element tmp = (Element)_document.createElementNS(@namespace, qName);

		// Add namespace declarations first
		if (_namespaceDecls != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nDecls = _namespaceDecls.size();
			int nDecls = _namespaceDecls.Count;
			for (int i = 0; i < nDecls; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = (String) _namespaceDecls.elementAt(i++);
			string prefix = (string) _namespaceDecls[i++];

			if (string.ReferenceEquals(prefix, null) || prefix.Equals(EMPTYSTRING))
			{
				tmp.setAttributeNS(XMLNS_URI, XMLNS_PREFIX, (string) _namespaceDecls[i]);
			}
			else
			{
				tmp.setAttributeNS(XMLNS_URI, XMLNS_STRING + prefix, (string) _namespaceDecls[i]);
			}
			}
			_namespaceDecls.Clear();
		}

		// Add attributes to element
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nattrs = attrs.getLength();
		int nattrs = attrs.getLength();
		for (int i = 0; i < nattrs; i++)
		{
			if (attrs.getLocalName(i) == null)
			{
			tmp.setAttribute(attrs.getQName(i), attrs.getValue(i));
			}
			else
			{
			tmp.setAttributeNS(attrs.getURI(i), attrs.getQName(i), attrs.getValue(i));
			}
		}

		// Append this new node onto current stack node
		Node last = (Node)_nodeStk.Peek();

		// If the SAX2DOM is created with a non-null next sibling node,
		// insert the result nodes before the next sibling under the root.
		if (last == _root && _nextSibling != null)
		{
			last.insertBefore(tmp, _nextSibling);
		}
		else
		{
			last.appendChild(tmp);
		}

		// Push this node onto stack
		_nodeStk.Push(tmp);
		_lastSibling = null;
		}

		public virtual void endElement(string @namespace, string localName, string qName)
		{
		_nodeStk.Pop();
		_lastSibling = null;
		}

		public virtual void startPrefixMapping(string prefix, string uri)
		{
		if (_namespaceDecls == null)
		{
			_namespaceDecls = new ArrayList(2);
		}
		_namespaceDecls.Add(prefix);
		_namespaceDecls.Add(uri);
		}

		public virtual void endPrefixMapping(string prefix)
		{
		// do nothing
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual void ignorableWhitespace(char[] ch, int start, int length)
		{
		}

		/// <summary>
		/// adds processing instruction node to DOM.
		/// </summary>
		public virtual void processingInstruction(string target, string data)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node last = (org.w3c.dom.Node)_nodeStk.peek();
		Node last = (Node)_nodeStk.Peek();
		ProcessingInstruction pi = _document.createProcessingInstruction(target, data);
		if (pi != null)
		{
			  if (last == _root && _nextSibling != null)
			  {
				  last.insertBefore(pi, _nextSibling);
			  }
			  else
			  {
				  last.appendChild(pi);
			  }

			  _lastSibling = pi;
		}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual Locator DocumentLocator
		{
			set
			{
			}
		}

		/// <summary>
		/// This class is only used internally so this method should never 
		/// be called.
		/// </summary>
		public virtual void skippedEntity(string name)
		{
		}


		/// <summary>
		/// Lexical Handler method to create comment node in DOM tree.
		/// </summary>
		public virtual void comment(char[] ch, int start, int length)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node last = (org.w3c.dom.Node)_nodeStk.peek();
		Node last = (Node)_nodeStk.Peek();
		Comment comment = _document.createComment(new string(ch,start,length));
		if (comment != null)
		{
			  if (last == _root && _nextSibling != null)
			  {
				  last.insertBefore(comment, _nextSibling);
			  }
			  else
			  {
				  last.appendChild(comment);
			  }

			  _lastSibling = comment;
		}
		}

		// Lexical Handler methods- not implemented
		public virtual void startCDATA()
		{
		}
		public virtual void endCDATA()
		{
		}
		public virtual void startEntity(string name)
		{
		}
		public virtual void endDTD()
		{
		}
		public virtual void endEntity(string name)
		{
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public virtual void startDTD(string name, string publicId, string systemId)
		{
		}

	}

}