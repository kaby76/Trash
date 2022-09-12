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
 * $Id: BasicTestIterator.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpMap = org.apache.xpath.compiler.OpMap;

	/// <summary>
	/// Base for iterators that handle predicates.  Does the basic next 
	/// node logic, so all the derived iterator has to do is get the 
	/// next node.
	/// </summary>
	[Serializable]
	public abstract class BasicTestIterator : LocPathIterator
	{
		internal new const long serialVersionUID = 3505378079378096623L;
	  /// <summary>
	  /// Create a LocPathIterator object.
	  /// </summary>
	  /// <param name="nscontext"> The namespace context for this iterator,
	  /// should be OK if null. </param>
	  protected internal BasicTestIterator()
	  {
	  }


	  /// <summary>
	  /// Create a LocPathIterator object.
	  /// </summary>
	  /// <param name="nscontext"> The namespace context for this iterator,
	  /// should be OK if null. </param>
	  protected internal BasicTestIterator(PrefixResolver nscontext) : base(nscontext)
	  {

	  }

	  /// <summary>
	  /// Create a LocPathIterator object, including creation
	  /// of step walkers from the opcode list, and call back
	  /// into the Compiler to create predicate expressions.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the
	  /// opcode list from the compiler.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected BasicTestIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  protected internal BasicTestIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis, false)
	  {

		int firstStepPos = OpMap.getFirstChildPos(opPos);
		int whatToShow = compiler.getWhatToShow(firstStepPos);

		if ((0 == (whatToShow & (org.apache.xml.dtm.DTMFilter_Fields.SHOW_ATTRIBUTE | org.apache.xml.dtm.DTMFilter_Fields.SHOW_NAMESPACE | org.apache.xml.dtm.DTMFilter_Fields.SHOW_ELEMENT | org.apache.xml.dtm.DTMFilter_Fields.SHOW_PROCESSING_INSTRUCTION))) || (whatToShow == org.apache.xml.dtm.DTMFilter_Fields.SHOW_ALL))
		{
		  initNodeTest(whatToShow);
		}
		else
		{
		  initNodeTest(whatToShow, compiler.getStepNS(firstStepPos), compiler.getStepLocalName(firstStepPos));
		}
		initPredicateInfo(compiler, firstStepPos);
	  }

	  /// <summary>
	  /// Create a LocPathIterator object, including creation
	  /// of step walkers from the opcode list, and call back
	  /// into the Compiler to create predicate expressions.
	  /// </summary>
	  /// <param name="compiler"> The Compiler which is creating
	  /// this expression. </param>
	  /// <param name="opPos"> The position of this iterator in the
	  /// opcode list from the compiler. </param>
	  /// <param name="shouldLoadWalkers"> True if walkers should be
	  /// loaded, or false if this is a derived iterator and
	  /// it doesn't wish to load child walkers.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected BasicTestIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis, boolean shouldLoadWalkers) throws javax.xml.transform.TransformerException
	  protected internal BasicTestIterator(Compiler compiler, int opPos, int analysis, bool shouldLoadWalkers) : base(compiler, opPos, analysis, shouldLoadWalkers)
	  {
	  }


	  /// <summary>
	  /// Get the next node via getNextXXX.  Bottlenecked for derived class override. </summary>
	  /// <returns> The next node on the axis, or DTM.NULL. </returns>
	  protected internal abstract int NextNode {get;}

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
			  m_lastFetched = org.apache.xml.dtm.DTM_Fields.NULL;
			  return org.apache.xml.dtm.DTM_Fields.NULL;
		  }

		if (org.apache.xml.dtm.DTM_Fields.NULL == m_lastFetched)
		{
		  resetProximityPositions();
		}

		int next;

		org.apache.xpath.VariableStack vars;
		int savedStart;
		if (-1 != m_stackFrame)
		{
		  vars = m_execContext.VarStack;

		  // These three statements need to be combined into one operation.
		  savedStart = vars.StackFrame;

		  vars.StackFrame = m_stackFrame;
		}
		else
		{
		  // Yuck.  Just to shut up the compiler!
		  vars = null;
		  savedStart = 0;
		}

		try
		{
		  do
		  {
			next = NextNode;

			if (org.apache.xml.dtm.DTM_Fields.NULL != next)
			{
			  if (org.apache.xml.dtm.DTMIterator_Fields.FILTER_ACCEPT == acceptNode(next))
			  {
				break;
			  }
			  else
			  {
				continue;
			  }
			}
			else
			{
			  break;
			}
		  } while (next != org.apache.xml.dtm.DTM_Fields.NULL);

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
		finally
		{
		  if (-1 != m_stackFrame)
		  {
			// These two statements need to be combined into one operation.
			vars.StackFrame = savedStart;
		  }
		}
	  }

	  /// <summary>
	  ///  Get a cloned Iterator that is reset to the beginning
	  ///  of the query.
	  /// </summary>
	  ///  <returns> A cloned NodeIterator set of the start of the query.
	  /// </returns>
	  ///  <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public override DTMIterator cloneWithReset()
	  {

		ChildTestIterator clone = (ChildTestIterator) base.cloneWithReset();

		clone.resetProximityPositions();

		return clone;
	  }


	}


}