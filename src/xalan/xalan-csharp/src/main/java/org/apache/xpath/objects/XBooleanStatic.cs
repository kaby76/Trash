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
 * $Id: XBooleanStatic.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{
	/// <summary>
	/// This class doesn't have any XPathContext, so override
	/// whatever to ensure it works OK.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class XBooleanStatic : XBoolean
	{
		internal new const long serialVersionUID = -8064147275772687409L;

	  /// <summary>
	  /// The value of the object.
	  ///  @serial          
	  /// </summary>
	  private readonly bool m_val;

	  /// <summary>
	  /// Construct a XBooleanStatic object.
	  /// </summary>
	  /// <param name="b"> The value of the object </param>
	  public XBooleanStatic(bool b) : base(b)
	  {


		m_val = b;
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