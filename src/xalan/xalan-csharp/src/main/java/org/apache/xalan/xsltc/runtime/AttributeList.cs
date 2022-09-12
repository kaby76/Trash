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
 * $Id: AttributeList.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class AttributeList : org.xml.sax.Attributes
	{

		private const string EMPTYSTRING = "";
		private const string CDATASTRING = "CDATA";

		private Hashtable _attributes;
		private ArrayList _names;
		private ArrayList _qnames;
		private ArrayList _values;
		private ArrayList _uris;
		private int _length;

		/// <summary>
		/// AttributeList constructor
		/// </summary>
		public AttributeList()
		{
		/*
		_attributes = new Hashtable();
		_names  = new Vector();
		_values = new Vector();
		_qnames = new Vector();
		_uris   = new Vector();
		*/
		_length = 0;
		}

		/// <summary>
		/// Attributes clone constructor
		/// </summary>
		public AttributeList(org.xml.sax.Attributes attributes) : this()
		{
		if (attributes != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = attributes.getLength();
			int count = attributes.Length;
			for (int i = 0; i < count; i++)
			{
			add(attributes.getQName(i),attributes.getValue(i));
			}
		}
		}

		/// <summary>
		/// Allocate memory for the AttributeList
		/// %OPT% Use on-demand allocation for the internal vectors. The memory
		/// is only allocated when there is an attribute. This reduces the cost 
		/// of creating many small RTFs.
		/// </summary>
		private void alloc()
		{
		_attributes = new Hashtable();
		_names = new ArrayList();
		_values = new ArrayList();
		_qnames = new ArrayList();
		_uris = new ArrayList();
		}

		/// <summary>
		/// SAX2: Return the number of attributes in the list. 
		/// </summary>
		public virtual int Length
		{
			get
			{
			return (_length);
			}
		}

		/// <summary>
		/// SAX2: Look up an attribute's Namespace URI by index.
		/// </summary>
		public virtual string getURI(int index)
		{
		if (index < _length)
		{
			return ((string)_uris[index]);
		}
		else
		{
			return (null);
		}
		}

		/// <summary>
		/// SAX2: Look up an attribute's local name by index.
		/// </summary>
		public virtual string getLocalName(int index)
		{
		if (index < _length)
		{
			return ((string)_names[index]);
		}
		else
		{
			return (null);
		}
		}

		/// <summary>
		/// Return the name of an attribute in this list (by position).
		/// </summary>
		public virtual string getQName(int pos)
		{
		if (pos < _length)
		{
			return ((string)_qnames[pos]);
		}
		else
		{
			return (null);
		}
		}

		/// <summary>
		/// SAX2: Look up an attribute's type by index.
		/// </summary>
		public virtual string getType(int index)
		{
		return (CDATASTRING);
		}

		/// <summary>
		/// SAX2: Look up the index of an attribute by Namespace name.
		/// </summary>
		public virtual int getIndex(string namespaceURI, string localPart)
		{
		return (-1);
		}

		/// <summary>
		/// SAX2: Look up the index of an attribute by XML 1.0 qualified name.
		/// </summary>
		public virtual int getIndex(string qname)
		{
		return (-1);
		}

		/// <summary>
		/// SAX2: Look up an attribute's type by Namespace name.
		/// </summary>
		public virtual string getType(string uri, string localName)
		{
		return (CDATASTRING);
		}

		/// <summary>
		/// SAX2: Look up an attribute's type by qname.
		/// </summary>
		public virtual string getType(string qname)
		{
		return (CDATASTRING);
		}

		/// <summary>
		/// SAX2: Look up an attribute's value by index.
		/// </summary>
		public virtual string getValue(int pos)
		{
		if (pos < _length)
		{
			return ((string)_values[pos]);
		}
		else
		{
			return (null);
		}
		}

		/// <summary>
		/// SAX2: Look up an attribute's value by qname.
		/// </summary>
		public virtual string getValue(string qname)
		{
		if (_attributes != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Nullable<int> obj = (Nullable<int>)_attributes.get(qname);
			int? obj = (int?)_attributes.get(qname);
			if (obj == null)
			{
				return null;
			}
			return (getValue(obj.Value));
		}
		else
		{
			return null;
		}
		}

		/// <summary>
		/// SAX2: Look up an attribute's value by Namespace name - SLOW!
		/// </summary>
		public virtual string getValue(string uri, string localName)
		{
		return (getValue(uri + ':' + localName));
		}

		/// <summary>
		/// Adds an attribute to the list
		/// </summary>
		public virtual void add(string qname, string value)
		{
		// Initialize the internal vectors at the first usage.
		if (_attributes == null)
		{
			alloc();
		}

		// Stuff the QName into the names vector & hashtable
		int? obj = (int?)_attributes.get(qname);
		if (obj == null)
		{
			_attributes.put(qname, obj = new int?(_length++));
			_qnames.Add(qname);
			_values.Add(value);
			int col = qname.LastIndexOf(':');
			if (col > -1)
			{
			_uris.Add(qname.Substring(0,col));
			_names.Add(qname.Substring(col + 1));
			}
			else
			{
			_uris.Add(EMPTYSTRING);
			_names.Add(qname);
			}
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = obj.intValue();
			int index = obj.Value;
			_values[index] = value;
		}
		}

		/// <summary>
		/// Clears the attribute list
		/// </summary>
		public virtual void clear()
		{
		_length = 0;
		if (_attributes != null)
		{
			_attributes.clear();
			_names.Clear();
			_values.Clear();
			_qnames.Clear();
			_uris.Clear();
		}
		}

	}

}