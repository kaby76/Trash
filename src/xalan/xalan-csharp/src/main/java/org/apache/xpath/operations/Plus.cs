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
 * $Id: Plus.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.operations
{
	using XPathContext = org.apache.xpath.XPathContext;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// The '+' operation expression executer.
	/// </summary>
	[Serializable]
	public class Plus : Operation
	{
		internal new const long serialVersionUID = -4492072861616504256L;

	  /// <summary>
	  /// Apply the operation to two operands, and return the result.
	  /// 
	  /// </summary>
	  /// <param name="left"> non-null reference to the evaluated left operand. </param>
	  /// <param name="right"> non-null reference to the evaluated right operand.
	  /// </param>
	  /// <returns> non-null reference to the XObject that represents the result of the operation.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject operate(org.apache.xpath.objects.XObject left, org.apache.xpath.objects.XObject right) throws javax.xml.transform.TransformerException
	  public override XObject operate(XObject left, XObject right)
	  {
		return new XNumber(left.num() + right.num());
	  }

	  /// <summary>
	  /// Evaluate this operation directly to a double.
	  /// </summary>
	  /// <param name="xctxt"> The runtime execution context.
	  /// </param>
	  /// <returns> The result of the operation as a double.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double num(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override double num(XPathContext xctxt)
	  {

		return (m_right.num(xctxt) + m_left.num(xctxt));
	  }

	}

}