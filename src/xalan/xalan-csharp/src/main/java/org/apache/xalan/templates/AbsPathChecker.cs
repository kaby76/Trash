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
 * $Id: AbsPathChecker.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using LocPathIterator = org.apache.xpath.axes.LocPathIterator;
	using FuncCurrent = org.apache.xpath.functions.FuncCurrent;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using Function = org.apache.xpath.functions.Function;
	using Variable = org.apache.xpath.operations.Variable;

	/// <summary>
	/// This class runs over a path expression that is assumed to be absolute, and 
	/// checks for variables and the like that may make it context dependent.
	/// </summary>
	public class AbsPathChecker : XPathVisitor
	{
		private bool m_isAbs = true;

		/// <summary>
		/// Process the LocPathIterator to see if it contains variables 
		/// or functions that may make it context dependent. </summary>
		/// <param name="path"> LocPathIterator that is assumed to be absolute, but needs checking. </param>
		/// <returns> true if the path is confirmed to be absolute, false if it 
		/// may contain context dependencies. </returns>
		public virtual bool checkAbsolute(LocPathIterator path)
		{
			m_isAbs = true;
			path.callVisitors(null, this);
			return m_isAbs;
		}

		/// <summary>
		/// Visit a function. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="func"> The function reference object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public override bool visitFunction(ExpressionOwner owner, Function func)
		{
			if ((func is FuncCurrent) || (func is FuncExtFunction))
			{
				m_isAbs = false;
			}
			return true;
		}

		/// <summary>
		/// Visit a variable reference. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="var"> The variable reference object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public override bool visitVariableRef(ExpressionOwner owner, Variable var)
		{
			m_isAbs = false;
			return true;
		}
	}


}