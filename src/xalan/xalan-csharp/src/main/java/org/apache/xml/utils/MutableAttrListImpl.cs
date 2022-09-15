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
 * $Id: MutableAttrListImpl.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	using Attributes = org.xml.sax.Attributes;
	using AttributesImpl = org.xml.sax.helpers.AttributesImpl;

	/// <summary>
	/// Mutable version of AttributesImpl.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class MutableAttrListImpl : AttributesImpl
	{
		internal const long serialVersionUID = 6289452013442934470L;

	/// <summary>
	/// Construct a new, empty AttributesImpl object.
	/// </summary>

	public MutableAttrListImpl() : base()
	{
	}

	  /// <summary>
	  /// Copy an existing Attributes object.
	  /// 
	  /// <para>This constructor is especially useful inside a start
	  /// element event.</para>
	  /// </summary>
	  /// <param name="atts"> The existing Attributes object. </param>
	  public MutableAttrListImpl(Attributes atts) : base(atts)
	  {
	  }

	  /// <summary>
	  /// Add an attribute to the end of the list.
	  /// 
	  /// <para>For the sake of speed, this method does no checking
	  /// to see if the attribute is already in the list: that is
	  /// the responsibility of the application.</para>
	  /// </summary>
	  /// <param name="uri"> The Namespace URI, or the empty string if
	  ///        none is available or Namespace processing is not
	  ///        being performed. </param>
	  /// <param name="localName"> The local name, or the empty string if
	  ///        Namespace processing is not being performed. </param>
	  /// <param name="qName"> The qualified (prefixed) name, or the empty string
	  ///        if qualified names are not available. </param>
	  /// <param name="type"> The attribute type as a string. </param>
	  /// <param name="value"> The attribute value. </param>
	  public virtual void addAttribute(string uri, string localName, string qName, string type, string value)
	  {

		if (null == uri)
		{
		  uri = "";
		}

		// getIndex(qName) seems to be more reliable than getIndex(uri, localName), 
		// in the case of the xmlns attribute anyway.
		int index = this.getIndex(qName);
		// int index = this.getIndex(uri, localName);

		// System.out.println("MutableAttrListImpl#addAttribute: "+uri+":"+localName+", "+index+", "+qName+", "+this);

		if (index >= 0)
		{
		  this.setAttribute(index, uri, localName, qName, type, value);
		}
		else
		{
		  base.addAttribute(uri, localName, qName, type, value);
		}
	  }

	  /// <summary>
	  /// Add the contents of the attribute list to this list.
	  /// </summary>
	  /// <param name="atts"> List of attributes to add to this list </param>
	  public virtual void addAttributes(Attributes atts)
	  {

		int nAtts = atts.getLength();

		for (int i = 0; i < nAtts; i++)
		{
		  string uri = atts.getURI(i);

		  if (null == uri)
		  {
			uri = "";
		  }

		  string localName = atts.getLocalName(i);
		  string qname = atts.getQName(i);
		  int index = this.getIndex(uri, localName);
		  // System.out.println("MutableAttrListImpl#addAttributes: "+uri+":"+localName+", "+index+", "+atts.getQName(i)+", "+this);
		  if (index >= 0)
		  {
			this.setAttribute(index, uri, localName, qname, atts.getType(i), atts.getValue(i));
		  }
		  else
		  {
			addAttribute(uri, localName, qname, atts.getType(i), atts.getValue(i));
		  }
		}
	  }

	  /// <summary>
	  /// Return true if list contains the given (raw) attribute name.
	  /// </summary>
	  /// <param name="name"> Raw name of attribute to look for 
	  /// </param>
	  /// <returns> true if an attribute is found with this name </returns>
	  public virtual bool contains(string name)
	  {
		return getValue(name) != null;
	  }
	}

	// end of MutableAttrListImpl.java

}