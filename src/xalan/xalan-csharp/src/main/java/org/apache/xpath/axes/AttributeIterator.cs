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
 * $Id: AttributeIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using DTM = org.apache.xml.dtm.DTM;
	using Compiler = org.apache.xpath.compiler.Compiler;

	/// <summary>
	/// This class implements an optimized iterator for
	/// attribute axes patterns. </summary>
	/// <seealso cref="org.apache.xpath.axes.ChildTestIterator"
	/// @xsl.usage advanced/>
	[Serializable]
	public class AttributeIterator : ChildTestIterator
	{
		internal new const long serialVersionUID = -8417986700712229686L;

	  /// <summary>
	  /// Create a AttributeIterator object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: AttributeIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal AttributeIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis)
	  {
	  }

	  /// <summary>
	  /// Get the next node via getFirstAttribute && getNextAttribute.
	  /// </summary>
	  protected internal override int NextNode
	  {
		  get
		  {
			m_lastFetched = (DTM.NULL == m_lastFetched) ? m_cdtm.getFirstAttribute(m_context) : m_cdtm.getNextAttribute(m_lastFetched);
			return m_lastFetched;
		  }
	  }

	  /// <summary>
	  /// Returns the axis being iterated, if it is known.
	  /// </summary>
	  /// <returns> Axis.CHILD, etc., or -1 if the axis is not known or is of multiple 
	  /// types. </returns>
	  public override int Axis
	  {
		  get
		  {
			return org.apache.xml.dtm.Axis.ATTRIBUTE;
		  }
	  }



	}

}