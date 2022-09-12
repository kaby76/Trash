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
 * $Id: HasPositionalPredChecker.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{

	using FuncLast = org.apache.xpath.functions.FuncLast;
	using FuncPosition = org.apache.xpath.functions.FuncPosition;
	using Function = org.apache.xpath.functions.Function;
	using XNumber = org.apache.xpath.objects.XNumber;
	using Div = org.apache.xpath.operations.Div;
	using Minus = org.apache.xpath.operations.Minus;
	using Mod = org.apache.xpath.operations.Mod;
	using Mult = org.apache.xpath.operations.Mult;
	using Plus = org.apache.xpath.operations.Plus;
	using Quo = org.apache.xpath.operations.Quo;
	using Variable = org.apache.xpath.operations.Variable;

	public class HasPositionalPredChecker : XPathVisitor
	{
		private bool m_hasPositionalPred = false;
		private int m_predDepth = 0;

		/// <summary>
		/// Process the LocPathIterator to see if it contains variables 
		/// or functions that may make it context dependent. </summary>
		/// <param name="path"> LocPathIterator that is assumed to be absolute, but needs checking. </param>
		/// <returns> true if the path is confirmed to be absolute, false if it 
		/// may contain context dependencies. </returns>
		public static bool check(LocPathIterator path)
		{
			HasPositionalPredChecker hppc = new HasPositionalPredChecker();
			path.callVisitors(null, hppc);
			return hppc.m_hasPositionalPred;
		}

		/// <summary>
		/// Visit a function. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="func"> The function reference object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public override bool visitFunction(ExpressionOwner owner, Function func)
		{
			if ((func is FuncPosition) || (func is FuncLast))
			{
				m_hasPositionalPred = true;
			}
			return true;
		}

	//	/**
	//	 * Visit a variable reference.
	//	 * @param owner The owner of the expression, to which the expression can 
	//	 *              be reset if rewriting takes place.
	//	 * @param var The variable reference object.
	//	 * @return true if the sub expressions should be traversed.
	//	 */
	//	public boolean visitVariableRef(ExpressionOwner owner, Variable var)
	//	{
	//		m_hasPositionalPred = true;
	//		return true;
	//	}

	  /// <summary>
	  /// Visit a predicate within a location path.  Note that there isn't a 
	  /// proper unique component for predicates, and that the expression will 
	  /// be called also for whatever type Expression is.
	  /// </summary>
	  /// <param name="owner"> The owner of the expression, to which the expression can 
	  ///              be reset if rewriting takes place. </param>
	  /// <param name="pred"> The predicate object. </param>
	  /// <returns> true if the sub expressions should be traversed. </returns>
	  public override bool visitPredicate(ExpressionOwner owner, Expression pred)
	  {
		m_predDepth++;

		if (m_predDepth == 1)
		{
		  if ((pred is Variable) || (pred is XNumber) || (pred is Div) || (pred is Plus) || (pred is Minus) || (pred is Mod) || (pred is Quo) || (pred is Mult) || (pred is org.apache.xpath.operations.Number) || (pred is Function))
		  {
			  m_hasPositionalPred = true;
		  }
		  else
		  {
			  pred.callVisitors(owner, this);
		  }
		}

		m_predDepth--;

		// Don't go have the caller go any further down the subtree.
		return false;
	  }


	}


}