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
 * $Id: SelfIteratorNoPredicate.java 469263 2006-10-30 20:45:40Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTM = org.apache.xml.dtm.DTM;
	using Compiler = org.apache.xpath.compiler.Compiler;

	/// <summary>
	/// This class implements an optimized iterator for
	/// "." patterns, that is, the self axes without any predicates. </summary>
	/// <seealso cref= org.apache.xpath.axes.LocPathIterator
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class SelfIteratorNoPredicate : LocPathIterator
	{
		internal new const long serialVersionUID = -4226887905279814201L;

	  /// <summary>
	  /// Create a SelfIteratorNoPredicate object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator. </param>
	  /// <param name="analysis"> Analysis bits.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: SelfIteratorNoPredicate(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal SelfIteratorNoPredicate(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis, false)
	  {
	  }

	  /// <summary>
	  /// Create a SelfIteratorNoPredicate object.
	  /// </summary>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SelfIteratorNoPredicate() throws javax.xml.transform.TransformerException
	  public SelfIteratorNoPredicate() : base(null)
	  {
	  }


	  /// <summary>
	  ///  Returns the next node in the set and advances the position of the
	  /// iterator in the set. After a NodeIterator is created, the first call
	  /// to nextNode() returns the first node in the set.
	  /// </summary>
	  /// <returns>  The next <code>Node</code> in the set being iterated over, or
	  ///   <code>null</code> if there are no more members in that set. </returns>
	  public override int nextNode()
	  {
		if (m_foundLast)
		{
		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}

		int next;

		m_lastFetched = next = (org.apache.xml.dtm.DTM_Fields.NULL == m_lastFetched) ? m_context : org.apache.xml.dtm.DTM_Fields.NULL;

		// m_lastFetched = next;
		if (org.apache.xml.dtm.DTM_Fields.NULL != next)
		{
		  m_pos++;

		  return next;
		}
		else
		{
		  m_foundLast = true;

		  return org.apache.xml.dtm.DTM_Fields.NULL;
		}
	  }

	  /// <summary>
	  /// Return the first node out of the nodeset, if this expression is 
	  /// a nodeset expression.  This is the default implementation for 
	  /// nodesets.  Derived classes should try and override this and return a 
	  /// value without having to do a clone operation. </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// <returns> the first node out of the nodeset, or DTM.NULL. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int asNode(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override int asNode(XPathContext xctxt)
	  {
		return xctxt.CurrentNode;
	  }

	  /// <summary>
	  /// Get the index of the last node that can be itterated to.
	  /// This probably will need to be overridded by derived classes.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> the index of the last node that can be itterated to. </returns>
	  public override int getLastPos(XPathContext xctxt)
	  {
		return 1;
	  }


	}

}