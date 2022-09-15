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
 * $Id: ChildTestIterator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisTraverser = org.apache.xml.dtm.DTMAxisTraverser;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Compiler = org.apache.xpath.compiler.Compiler;

	/// <summary>
	/// This class implements an optimized iterator for
	/// children patterns that have a node test, and possibly a predicate. </summary>
	/// <seealso cref="org.apache.xpath.axes.BasicTestIterator"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ChildTestIterator : BasicTestIterator
	{
		internal new const long serialVersionUID = -7936835957960705722L;
	  /// <summary>
	  /// The traverser to use to navigate over the descendants. </summary>
	  [NonSerialized]
	  protected internal DTMAxisTraverser m_traverser;

	  /// <summary>
	  /// The extended type ID, not set until setRoot. </summary>
	//  protected int m_extendedTypeID;


	  /// <summary>
	  /// Create a ChildTestIterator object.
	  /// </summary>
	  /// <param name="compiler"> A reference to the Compiler that contains the op map. </param>
	  /// <param name="opPos"> The position within the op map, which contains the
	  /// location path expression for this itterator.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: ChildTestIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis) throws javax.xml.transform.TransformerException
	  internal ChildTestIterator(Compiler compiler, int opPos, int analysis) : base(compiler, opPos, analysis)
	  {
	  }

	  /// <summary>
	  /// Create a ChildTestIterator object.
	  /// </summary>
	  /// <param name="traverser"> Traverser that tells how the KeyIterator is to be handled.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public ChildTestIterator(DTMAxisTraverser traverser) : base(null)
	  {


		m_traverser = traverser;
	  }

	  /// <summary>
	  /// Get the next node via getNextXXX.  Bottlenecked for derived class override. </summary>
	  /// <returns> The next node on the axis, or DTM.NULL. </returns>
	  protected internal override int NextNode
	  {
		  get
		  {
			if (true)
			{
			  m_lastFetched = (DTM.NULL == m_lastFetched) ? m_traverser.first(m_context) : m_traverser.next(m_context, m_lastFetched);
			}
		//    else
		//    {
		//      m_lastFetched = (DTM.NULL == m_lastFetched)
		//                   ? m_traverser.first(m_context, m_extendedTypeID)
		//                   : m_traverser.next(m_context, m_lastFetched, 
		//                                      m_extendedTypeID);
		//    }
    
			return m_lastFetched;
		  }
	  }


	  /// <summary>
	  ///  Get a cloned Iterator that is reset to the beginning
	  ///  of the query.
	  /// </summary>
	  ///  <returns> A cloned NodeIterator set of the start of the query.
	  /// </returns>
	  ///  <exception cref="CloneNotSupportedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xml.dtm.DTMIterator cloneWithReset() throws CloneNotSupportedException
	  public override DTMIterator cloneWithReset()
	  {

		ChildTestIterator clone = (ChildTestIterator) base.cloneWithReset();
		clone.m_traverser = m_traverser;

		return clone;
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
		m_traverser = m_cdtm.getAxisTraverser(Axis.CHILD);

	//    String localName = getLocalName();
	//    String namespace = getNamespace();
	//    int what = m_whatToShow;
	//    // System.out.println("what: ");
	//    // NodeTest.debugWhatToShow(what);
	//    if(DTMFilter.SHOW_ALL == what ||
	//       ((DTMFilter.SHOW_ELEMENT & what) == 0)
	//       || localName == NodeTest.WILD
	//       || namespace == NodeTest.WILD)
	//    {
	//      m_extendedTypeID = 0;
	//    }
	//    else
	//    {
	//      int type = getNodeTypeTest(what);
	//      m_extendedTypeID = m_cdtm.getExpandedTypeID(namespace, localName, type);
	//    }

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
			return Axis.CHILD;
		  }
	  }

	  /// <summary>
	  ///  Detaches the iterator from the set which it iterated over, releasing
	  /// any computational resources and placing the iterator in the INVALID
	  /// state. After<code>detach</code> has been invoked, calls to
	  /// <code>nextNode</code> or<code>previousNode</code> will raise the
	  /// exception INVALID_STATE_ERR.
	  /// </summary>
	  public override void detach()
	  {
		if (m_allowDetach)
		{
		  m_traverser = null;

		  // Always call the superclass detach last!
		  base.detach();
		}
	  }

	}

}