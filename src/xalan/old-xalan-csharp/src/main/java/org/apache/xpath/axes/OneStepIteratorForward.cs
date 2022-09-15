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
 * $Id: OneStepIteratorForward.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using OpMap = org.apache.xpath.compiler.OpMap;

	/// <summary>
	/// This class implements a general iterator for
	/// those LocationSteps with only one step, and perhaps a predicate, 
	/// that only go forward (i.e. it can not be used with ancestors, 
	/// preceding, etc.) </summary>
	/// <seealso cref= org.apache.xpath.axes#ChildTestIterator
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class OneStepIteratorForward : ChildTestIterator
	{
		internal new const long serialVersionUID = -1576936606178190566L;
	  /// <summary>
	  /// The traversal axis from where the nodes will be filtered. </summary>
	  protected internal int m_axis = -1;

	  /// <summary>
	  /// Create a OneStepIterator object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: OneStepIteratorForward(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal OneStepIteratorForward(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis)
	  {
		int firstStepPos = OpMap.getFirstChildPos(opPos);

		m_axis = WalkerFactory.getAxisFromStep(compiler, firstStepPos);

	  }

	  /// <summary>
	  /// Create a OneStepIterator object that will just traverse the self axes.
	  /// </summary>
	  /// <param name="axis"> One of the org.apache.xml.dtm.Axis integers.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public OneStepIteratorForward(int axis) : base(null)
	  {

		m_axis = axis;
		int whatToShow = org.apache.xml.dtm.DTMFilter_Fields.SHOW_ALL;
		initNodeTest(whatToShow);
	  }




	  /// <summary>
	  /// Initialize the context values for this expression
	  /// after it is cloned.
	  /// </summary>
	  /// <param name="context"> The XPath runtime context for this
	  /// transformation. </param>
	  public override void setRoot(int context, object environment)
	  {
		base.setRoot(context, environment);
		m_traverser = m_cdtm.getAxisTraverser(m_axis);

	  }

	//  /**
	//   * Return the first node out of the nodeset, if this expression is 
	//   * a nodeset expression.  This is the default implementation for 
	//   * nodesets.
	//   * <p>WARNING: Do not mutate this class from this function!</p>
	//   * @param xctxt The XPath runtime context.
	//   * @return the first node out of the nodeset, or DTM.NULL.
	//   */
	//  public int asNode(XPathContext xctxt)
	//    throws javax.xml.transform.TransformerException
	//  {
	//    if(getPredicateCount() > 0)
	//      return super.asNode(xctxt);
	//      
	//    int current = xctxt.getCurrentNode();
	//    
	//    DTM dtm = xctxt.getDTM(current);
	//    DTMAxisTraverser traverser = dtm.getAxisTraverser(m_axis);
	//    
	//    String localName = getLocalName();
	//    String namespace = getNamespace();
	//    int what = m_whatToShow;
	//    
	//    // System.out.println("what: ");
	//    // NodeTest.debugWhatToShow(what);
	//    if(DTMFilter.SHOW_ALL == what
	//       || ((DTMFilter.SHOW_ELEMENT & what) == 0)
	//       || localName == NodeTest.WILD
	//       || namespace == NodeTest.WILD)
	//    {
	//      return traverser.first(current);
	//    }
	//    else
	//    {
	//      int type = getNodeTypeTest(what);
	//      int extendedType = dtm.getExpandedTypeID(namespace, localName, type);
	//      return traverser.first(current, extendedType);
	//    }
	//  }

	  /// <summary>
	  /// Get the next node via getFirstAttribute && getNextAttribute.
	  /// </summary>
	  protected internal override int NextNode
	  {
		  get
		  {
			m_lastFetched = (org.apache.xml.dtm.DTM_Fields.NULL == m_lastFetched) ? m_traverser.first(m_context) : m_traverser.next(m_context, m_lastFetched);
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
			return m_axis;
		  }
	  }

	  /// <seealso cref= Expression#deepEquals(Expression) </seealso>
	  public override bool deepEquals(Expression expr)
	  {
		  if (!base.deepEquals(expr))
		  {
			  return false;
		  }

		  if (m_axis != ((OneStepIteratorForward)expr).m_axis)
		  {
			  return false;
		  }

		  return true;
	  }


	}

}