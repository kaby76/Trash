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
 * $Id: Lte.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.operations
{

	using XBoolean = org.apache.xpath.objects.XBoolean;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// The '<=' operation expression executer.
	/// </summary>
	[Serializable]
	public class Lte : Operation
	{
		internal new const long serialVersionUID = 6945650810527140228L;

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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject operate(org.apache.xpath.objects.XObject left, org.apache.xpath.objects.XObject right) throws javax.xml.transform.TransformerException
	  public override XObject operate(XObject left, XObject right)
	  {
		return left.lessThanOrEqual(right) ? XBoolean.S_TRUE : XBoolean.S_FALSE;
	  }
	}

}