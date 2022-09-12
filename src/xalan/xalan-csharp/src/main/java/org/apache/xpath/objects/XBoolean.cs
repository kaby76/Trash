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
 * $Id: XBoolean.java 1225282 2011-12-28 18:55:38Z mrglavas $
 */
namespace org.apache.xpath.objects
{

	/// <summary>
	/// This class represents an XPath boolean object, and is capable of
	/// converting the boolean to other types, such as a string.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class XBoolean : XObject
	{
		internal new const long serialVersionUID = -2964933058866100881L;

	  /// <summary>
	  /// A true boolean object so we don't have to keep creating them.
	  /// @xsl.usage internal
	  /// </summary>
	  public static readonly XBoolean S_TRUE = new XBooleanStatic(true);

	  /// <summary>
	  /// A true boolean object so we don't have to keep creating them.
	  /// @xsl.usage internal
	  /// </summary>
	  public static readonly XBoolean S_FALSE = new XBooleanStatic(false);

	  /// <summary>
	  /// Value of the object.
	  ///  @serial         
	  /// </summary>
	  private readonly bool m_val;

	  /// <summary>
	  /// Construct a XBoolean object.
	  /// </summary>
	  /// <param name="b"> Value of the boolean object </param>
	  public XBoolean(bool b) : base()
	  {


		m_val = b;
	  }

	  /// <summary>
	  /// Construct a XBoolean object.
	  /// </summary>
	  /// <param name="b"> Value of the boolean object </param>
	  public XBoolean(bool? b) : base()
	  {


		m_val = b.Value;
		Object = b;
	  }


	  /// <summary>
	  /// Tell that this is a CLASS_BOOLEAN.
	  /// </summary>
	  /// <returns> type of CLASS_BOOLEAN </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_BOOLEAN;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#BOOLEAN" </returns>
	  public override string TypeString
	  {
		  get
		  {
			return "#BOOLEAN";
		  }
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> numeric value of the object value </returns>
	  public override double num()
	  {
		return m_val ? 1.0 : 0.0;
	  }

	  /// <summary>
	  /// Cast result object to a boolean.
	  /// </summary>
	  /// <returns> The object value as a boolean </returns>
	  public override bool @bool()
	  {
		return m_val;
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The object's value as a string </returns>
	  public override string str()
	  {
		return m_val ? "true" : "false";
	  }

	  /// <summary>
	  /// Return a java object that's closest to the representation
	  /// that should be handed to an extension.
	  /// </summary>
	  /// <returns> The object's value as a java object </returns>
	  public override object @object()
	  {
		if (null == m_obj)
		{
		  Object = m_val ? true : false;
		}
		return m_obj;
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare to this  
	  /// </param>
	  /// <returns> True if the two objects are equal
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public override bool Equals(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.
		if (obj2.Type == XObject.CLASS_NODESET)
		{
		  return obj2.Equals(this);
		}

		try
		{
		  return m_val == obj2.@bool();
		}
		catch (javax.xml.transform.TransformerException te)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(te);
		}
	  }

	}

}