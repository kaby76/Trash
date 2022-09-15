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
 * $Id: XPathVisitor.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{
	using LocPathIterator = org.apache.xpath.axes.LocPathIterator;
	using UnionPathIterator = org.apache.xpath.axes.UnionPathIterator;
	using Function = org.apache.xpath.functions.Function;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XString = org.apache.xpath.objects.XString;
	using Operation = org.apache.xpath.operations.Operation;
	using UnaryOperation = org.apache.xpath.operations.UnaryOperation;
	using Variable = org.apache.xpath.operations.Variable;
	using NodeTest = org.apache.xpath.patterns.NodeTest;
	using StepPattern = org.apache.xpath.patterns.StepPattern;
	using UnionPattern = org.apache.xpath.patterns.UnionPattern;

	/// <summary>
	/// A derivation from this class can be passed to a class that implements 
	/// the XPathVisitable interface, to have the appropriate method called 
	/// for each component of the XPath.  Aside from possible other uses, the 
	/// main intention is to provide a reasonable means to perform expression 
	/// rewriting.
	/// 
	/// <para>Each method has the form 
	/// <code>boolean visitComponentType(ExpressionOwner owner, ComponentType compType)</code>. 
	/// The ExpressionOwner argument is the owner of the component, and can 
	/// be used to reset the expression for rewriting.  If a method returns 
	/// false, the sub hierarchy will not be traversed.</para>
	/// 
	/// <para>This class is meant to be a base class that will be derived by concrete classes, 
	/// and doesn't much except return true for each method.</para>
	/// </summary>
	public class XPathVisitor
	{
		/// <summary>
		/// Visit a LocationPath. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="path"> The LocationPath object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitLocationPath(ExpressionOwner owner, LocPathIterator path)
		{
			return true;
		}

		/// <summary>
		/// Visit a UnionPath. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="path"> The UnionPath object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitUnionPath(ExpressionOwner owner, UnionPathIterator path)
		{
			return true;
		}

		/// <summary>
		/// Visit a step within a location path. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="step"> The Step object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitStep(ExpressionOwner owner, NodeTest step)
		{
			return true;
		}

		/// <summary>
		/// Visit a predicate within a location path.  Note that there isn't a 
		/// proper unique component for predicates, and that the expression will 
		/// be called also for whatever type Expression is.
		/// </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="pred"> The predicate object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitPredicate(ExpressionOwner owner, Expression pred)
		{
			return true;
		}

		/// <summary>
		/// Visit a binary operation. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="op"> The operation object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitBinaryOperation(ExpressionOwner owner, Operation op)
		{
			return true;
		}

		/// <summary>
		/// Visit a unary operation. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="op"> The operation object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitUnaryOperation(ExpressionOwner owner, UnaryOperation op)
		{
			return true;
		}

		/// <summary>
		/// Visit a variable reference. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="var"> The variable reference object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitVariableRef(ExpressionOwner owner, Variable var)
		{
			return true;
		}

		/// <summary>
		/// Visit a function. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="func"> The function reference object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitFunction(ExpressionOwner owner, Function func)
		{
			return true;
		}

		/// <summary>
		/// Visit a match pattern. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="pattern"> The match pattern object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitMatchPattern(ExpressionOwner owner, StepPattern pattern)
		{
			return true;
		}

		/// <summary>
		/// Visit a union pattern. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="pattern"> The union pattern object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitUnionPattern(ExpressionOwner owner, UnionPattern pattern)
		{
			return true;
		}

		/// <summary>
		/// Visit a string literal. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="str"> The string literal object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitStringLiteral(ExpressionOwner owner, XString str)
		{
			return true;
		}


		/// <summary>
		/// Visit a number literal. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="num"> The number literal object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitNumberLiteral(ExpressionOwner owner, XNumber num)
		{
			return true;
		}


	}


}