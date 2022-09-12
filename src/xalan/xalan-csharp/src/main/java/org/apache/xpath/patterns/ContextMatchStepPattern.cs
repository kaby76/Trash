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
 * $Id: ContextMatchStepPattern.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.patterns
{

	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using WalkerFactory = org.apache.xpath.axes.WalkerFactory;
	using XObject = org.apache.xpath.objects.XObject;
	/// <summary>
	/// Special context node pattern matcher.
	/// </summary>
	[Serializable]
	public class ContextMatchStepPattern : StepPattern
	{
		internal new const long serialVersionUID = -1888092779313211942L;

	  /// <summary>
	  /// Construct a ContextMatchStepPattern.
	  /// 
	  /// </summary>
	  public ContextMatchStepPattern(int axis, int paxis) : base(org.apache.xml.dtm.DTMFilter_Fields.SHOW_ALL, axis, paxis)
	  {
	  }

	  /// <summary>
	  /// Execute this pattern step, including predicates.
	  /// 
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

		if (xctxt.IteratorRoot == xctxt.CurrentNode)
		{
		  return StaticScore;
		}
		else
		{
		  return SCORE_NONE;
		}
	  }

	  /// <summary>
	  /// Execute the match pattern step relative to another step.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime context. </param>
	  /// NEEDSDOC <param name="prevStep">
	  /// </param>
	  /// <returns> <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NODETEST"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NONE"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_NSWILD"/>,
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_QNAME"/>, or
	  ///         <seealso cref="org.apache.xpath.patterns.NodeTest#SCORE_OTHER"/>.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject executeRelativePathPattern(org.apache.xpath.XPathContext xctxt, StepPattern prevStep) throws javax.xml.transform.TransformerException
	  public virtual XObject executeRelativePathPattern(XPathContext xctxt, StepPattern prevStep)
	  {

		XObject score = NodeTest.SCORE_NONE;
		int context = xctxt.CurrentNode;
		DTM dtm = xctxt.getDTM(context);

		if (null != dtm)
		{
		  int predContext = xctxt.CurrentNode;
		  DTMAxisTraverser traverser;

		  int axis = m_axis;

		  bool needToTraverseAttrs = WalkerFactory.isDownwardAxisOfMany(axis);
		  bool iterRootIsAttr = (dtm.getNodeType(xctxt.IteratorRoot) == org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE);

		  if ((Axis.PRECEDING == axis) && iterRootIsAttr)
		  {
			axis = Axis.PRECEDINGANDANCESTOR;
		  }

		  traverser = dtm.getAxisTraverser(axis);

		  for (int relative = traverser.first(context); org.apache.xml.dtm.DTM_Fields.NULL != relative; relative = traverser.next(context, relative))
		  {
			try
			{
			  xctxt.pushCurrentNode(relative);

			  score = execute(xctxt);

			  if (score != NodeTest.SCORE_NONE)
			  {
			  //score = executePredicates( xctxt, prevStep, SCORE_OTHER, 
			  //       predContext, relative);
			  if (executePredicates(xctxt, dtm, context))
			  {
			  return score;
			  }

			  score = NodeTest.SCORE_NONE;
			  }

			  if (needToTraverseAttrs && iterRootIsAttr && (org.apache.xml.dtm.DTM_Fields.ELEMENT_NODE == dtm.getNodeType(relative)))
			  {
				int xaxis = Axis.ATTRIBUTE;
				for (int i = 0; i < 2; i++)
				{
				  DTMAxisTraverser atraverser = dtm.getAxisTraverser(xaxis);

				  for (int arelative = atraverser.first(relative); org.apache.xml.dtm.DTM_Fields.NULL != arelative; arelative = atraverser.next(relative, arelative))
				  {
					try
					{
					  xctxt.pushCurrentNode(arelative);

					  score = execute(xctxt);

					  if (score != NodeTest.SCORE_NONE)
					  {
				  //score = executePredicates( xctxt, prevStep, SCORE_OTHER, 
				  //       predContext, arelative);

						if (score != NodeTest.SCORE_NONE)
						{
						  return score;
						}
					  }
					}
					finally
					{
					  xctxt.popCurrentNode();
					}
				  }
				  xaxis = Axis.NAMESPACE;
				}
			  }

			}
			finally
			{
			  xctxt.popCurrentNode();
			}
		  }

		}

		return score;
	  }

	}

}