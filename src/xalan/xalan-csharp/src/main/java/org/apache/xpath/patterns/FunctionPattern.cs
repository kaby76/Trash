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
 * $Id: FunctionPattern.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.patterns
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Match pattern step that contains a function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FunctionPattern : StepPattern
	{
		internal new const long serialVersionUID = -5426793413091209944L;

	  /// <summary>
	  /// Construct a FunctionPattern from a
	  /// <seealso cref="org.apache.xpath.functions.Function expression"/>.
	  /// </summary>
	  /// NEEDSDOC <param name="expr"> </param>
	  public FunctionPattern(Expression expr, int axis, int predaxis) : base(0, null, null, axis, predaxis)
	  {


		m_functionExpr = expr;
	  }

	  /// <summary>
	  /// Static calc of match score.
	  /// </summary>
	  public sealed override void calcScore()
	  {

		m_score = SCORE_OTHER;

		if (null == m_targetString)
		{
		  calcTargetString();
		}
	  }

	  /// <summary>
	  /// Should be a <seealso cref="org.apache.xpath.functions.Function expression"/>.
	  ///  @serial   
	  /// </summary>
	  internal Expression m_functionExpr;

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame 
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		base.fixupVariables(vars, globalsSize);
		m_functionExpr.fixupVariables(vars, globalsSize);
	  }


	  /// <summary>
	  /// Test a node to see if it matches the given node test.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, int context) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, int context)
	  {

		DTMIterator nl = m_functionExpr.asIterator(xctxt, context);
		XNumber score = SCORE_NONE;

		if (null != nl)
		{
		  int n;

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (n = nl.nextNode()))
		  {
			score = (n == context) ? SCORE_OTHER : SCORE_NONE;

			if (score == SCORE_OTHER)
			{
			  context = n;

			  break;
			}
		  }

		  // nl.detach();
		}
		nl.detach();

		return score;
	  }

	  /// <summary>
	  /// Test a node to see if it matches the given node test.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, int context, org.apache.xml.dtm.DTM dtm, int expType) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, int context, DTM dtm, int expType)
	  {

		DTMIterator nl = m_functionExpr.asIterator(xctxt, context);
		XNumber score = SCORE_NONE;

		if (null != nl)
		{
		  int n;

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (n = nl.nextNode()))
		  {
			score = (n == context) ? SCORE_OTHER : SCORE_NONE;

			if (score == SCORE_OTHER)
			{
			  context = n;

			  break;
			}
		  }

		  nl.detach();
		}

		return score;
	  }

	  /// <summary>
	  /// Test a node to see if it matches the given node test.
	  /// </summary>
	  /// <param name="xctxt"> XPath runtime context.
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		int context = xctxt.CurrentNode;
		DTMIterator nl = m_functionExpr.asIterator(xctxt, context);
		XNumber score = SCORE_NONE;

		if (null != nl)
		{
		  int n;

		  while (org.apache.xml.dtm.DTM_Fields.NULL != (n = nl.nextNode()))
		  {
			score = (n == context) ? SCORE_OTHER : SCORE_NONE;

			if (score == SCORE_OTHER)
			{
			  context = n;

			  break;
			}
		  }

		  nl.detach();
		}

		return score;
	  }

	  internal class FunctionOwner : ExpressionOwner
	  {
		  private readonly FunctionPattern outerInstance;

		  public FunctionOwner(FunctionPattern outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		/// <seealso cref= ExpressionOwner#getExpression() </seealso>
		public virtual Expression Expression
		{
			get
			{
			  return outerInstance.m_functionExpr;
			}
			set
			{
				value.exprSetParent(outerInstance);
				outerInstance.m_functionExpr = value;
			}
		}


	  }

	  /// <summary>
	  /// Call the visitor for the function.
	  /// </summary>
	  protected internal override void callSubtreeVisitors(XPathVisitor visitor)
	  {
		m_functionExpr.callVisitors(new FunctionOwner(this), visitor);
		base.callSubtreeVisitors(visitor);
	  }

	}

}