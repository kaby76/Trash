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
 * $Id: XNull.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{
	using DTM = org.apache.xml.dtm.DTM;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// This class represents an XPath null object, and is capable of
	/// converting the null to other types, such as a string.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class XNull : XNodeSet
	{
		internal new const long serialVersionUID = -6841683711458983005L;

	  /// <summary>
	  /// Create an XObject.
	  /// </summary>
	  public XNull() : base()
	  {
	  }

	  /// <summary>
	  /// Tell what kind of class this is.
	  /// </summary>
	  /// <returns> type CLASS_NULL </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_NULL;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#CLASS_NULL" </returns>
	  public override string TypeString
	  {
		  get
		  {
			return "#CLASS_NULL";
		  }
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> 0.0 </returns>

	  public override double num()
	  {
		return 0.0;
	  }

	  /// <summary>
	  /// Cast result object to a boolean.
	  /// </summary>
	  /// <returns> false </returns>
	  public override bool @bool()
	  {
		return false;
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> empty string "" </returns>
	  public override string str()
	  {
		return "";
	  }

	  /// <summary>
	  /// Cast result object to a result tree fragment.
	  /// </summary>
	  /// <param name="support"> XPath context to use for the conversion
	  /// </param>
	  /// <returns> The object as a result tree fragment. </returns>
	  public override int rtf(XPathContext support)
	  {
		// DTM frag = support.createDocumentFragment();
		// %REVIEW%
		return DTM.NULL;
	  }

	//  /**
	//   * Cast result object to a nodelist.
	//   *
	//   * @return null
	//   */
	//  public DTMIterator iter()
	//  {
	//    return null;
	//  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if the given object is of type CLASS_NULL </returns>
	  public override bool Equals(XObject obj2)
	  {
		return obj2.Type == CLASS_NULL;
	  }
	}

}