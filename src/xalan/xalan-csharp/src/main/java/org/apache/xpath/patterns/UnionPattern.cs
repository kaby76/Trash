using System;
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
 * $Id: UnionPattern.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.patterns
{
	using Expression = org.apache.xpath.Expression;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class represents a union pattern, which can have multiple individual 
	/// StepPattern patterns.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class UnionPattern : Expression
	{
		internal new const long serialVersionUID = -6670449967116905820L;

	  /// <summary>
	  /// Array of the contained step patterns to be tested.
	  ///  @serial  
	  /// </summary>
	  private StepPattern[] m_patterns;

	  /// <summary>
	  /// No arguments to process, so this does nothing.
	  /// </summary>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		for (int i = 0; i < m_patterns.Length; i++)
		{
		  m_patterns[i].fixupVariables(vars, globalsSize);
		}
	  }


	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside 
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	   public override bool canTraverseOutsideSubtree()
	   {
		 if (null != m_patterns)
		 {
		  int n = m_patterns.Length;
		  for (int i = 0; i < n; i++)
		  {
			if (m_patterns[i].canTraverseOutsideSubtree())
			{
			  return true;
			}
		  }
		 }
		 return false;
	   }

	  /// <summary>
	  /// Set the contained step patterns to be tested. 
	  /// 
	  /// </summary>
	  /// <param name="patterns"> the contained step patterns to be tested.  </param>
	  public virtual StepPattern[] Patterns
	  {
		  set
		  {
			m_patterns = value;
			if (null != value)
			{
				for (int i = 0; i < value.Length; i++)
				{
					value[i].exprSetParent(this);
				}
			}
    
		  }
		  get
		  {
			return m_patterns;
		  }
	  }


	  /// <summary>
	  /// Test a node to see if it matches any of the patterns in the union.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest.SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		XObject bestScore = null;
		int n = m_patterns.Length;

		for (int i = 0; i < n; i++)
		{
		  XObject score = m_patterns[i].execute(xctxt);

		  if (score != NodeTest.SCORE_NONE)
		  {
			if (null == bestScore)
			{
			  bestScore = score;
			}
			else if (score.num() > bestScore.num())
			{
			  bestScore = score;
			}
		  }
		}

		if (null == bestScore)
		{
		  bestScore = NodeTest.SCORE_NONE;
		}

		return bestScore;
	  }

	  internal class UnionPathPartOwner : ExpressionOwner
	  {
		  private readonly UnionPattern outerInstance;

		  internal int m_index;

		  internal UnionPathPartOwner(UnionPattern outerInstance, int index)
		  {
			  this.outerInstance = outerInstance;
			  m_index = index;
		  }

		/// <seealso cref="ExpressionOwner.getExpression()"/>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_patterns[m_index];
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_patterns[m_index] = (StepPattern)value;
			}
		}


	  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  visitor.visitUnionPattern(owner, this);
		  if (null != m_patterns)
		  {
			  int n = m_patterns.Length;
			  for (int i = 0; i < n; i++)
			  {
				  m_patterns[i].callVisitors(new UnionPathPartOwner(this, i), visitor);
			  }
		  }
	  }

	  /// <seealso cref="Expression.deepEquals(Expression)"/>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!isSameClass(expr))
		  {
			  return false;
		  }

		  UnionPattern up = (UnionPattern)expr;

		  if (null != m_patterns)
		  {
			  int n = m_patterns.Length;
			  if ((null == up.m_patterns) || (up.m_patterns.Length != n))
			  {
				  return false;
			  }

			  for (int i = 0; i < n; i++)
			  {
				  if (!m_patterns[i].deepEquals(up.m_patterns[i]))
				  {
					  return false;
				  }
			  }
		  }
		  else if (up.m_patterns != null)
		  {
			  return false;
		  }

		  return true;

	  }


	}

}