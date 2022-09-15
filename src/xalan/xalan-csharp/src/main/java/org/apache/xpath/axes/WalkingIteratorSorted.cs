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
 * $Id: WalkingIteratorSorted.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.axes
{
	using Axis = org.apache.xml.dtm.Axis;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using Compiler = org.apache.xpath.compiler.Compiler;

	/// <summary>
	/// This class iterates over set of nodes that needs to be sorted.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class WalkingIteratorSorted : WalkingIterator
	{
		internal new const long serialVersionUID = -4512512007542368213L;

	//  /** True if the nodes will be found in document order */
	//  protected boolean m_inNaturalOrder = false;

	  /// <summary>
	  /// True if the nodes will be found in document order, and this can 
	  /// be determined statically. 
	  /// </summary>
	  protected internal bool m_inNaturalOrderStatic = false;

	  /// <summary>
	  /// Create a WalkingIteratorSorted object.
	  /// </summary>
	  /// <param name="nscontext"> The namespace context for this iterator,
	  /// should be OK if null. </param>
	  public WalkingIteratorSorted(PrefixResolver nscontext) : base(nscontext)
	  {
	  }

	  /// <summary>
	  /// Create a WalkingIterator iterator, including creation
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: WalkingIteratorSorted(org.apache.xpath.compiler.Compiler compiler, int opPos, int analysis, boolean shouldLoadWalkers) throws javax.xml.transform.TransformerException
	  internal WalkingIteratorSorted(Compiler compiler, int opPos, int analysis, bool shouldLoadWalkers) : base(compiler, opPos, analysis, shouldLoadWalkers)
	  {
	  }

	  /// <summary>
	  /// Returns true if all the nodes in the iteration well be returned in document 
	  /// order.
	  /// </summary>
	  /// <returns> true as a default. </returns>
	  public override bool DocOrdered
	  {
		  get
		  {
			return m_inNaturalOrderStatic;
		  }
	  }


	  /// <summary>
	  /// Tell if the nodeset can be walked in doc order, via static analysis. 
	  /// 
	  /// </summary>
	  /// <returns> true if the nodeset can be walked in doc order, without sorting. </returns>
	  internal virtual bool canBeWalkedInNaturalDocOrderStatic()
	  {

		if (null != m_firstWalker)
		{
		  AxesWalker walker = m_firstWalker;
		  int prevAxis = -1;
		  bool prevIsSimpleDownAxis = true;

		  for (int i = 0; null != walker; i++)
		  {
			int axis = walker.Axis;

			if (walker.DocOrdered)
			{
			  bool isSimpleDownAxis = ((axis == Axis.CHILD) || (axis == Axis.SELF) || (axis == Axis.ROOT));
			  // Catching the filtered list here is only OK because
			  // FilterExprWalker#isDocOrdered() did the right thing.
			  if (isSimpleDownAxis || (axis == -1))
			  {
				walker = walker.NextWalker;
			  }
			  else
			  {
				bool isLastWalker = (null == walker.NextWalker);
				if (isLastWalker)
				{
				  if (walker.DocOrdered && (axis == Axis.DESCENDANT || axis == Axis.DESCENDANTORSELF || axis == Axis.DESCENDANTSFROMROOT || axis == Axis.DESCENDANTSORSELFFROMROOT) || (axis == Axis.ATTRIBUTE))
				  {
					return true;
				  }
				}
				return false;
			  }
			}
			else
			{
			  return false;
			}
		  }
		  return true;
		}
		return false;
	  }


	//  /**
	//   * NEEDSDOC Method canBeWalkedInNaturalDocOrder 
	//   *
	//   *
	//   * NEEDSDOC (canBeWalkedInNaturalDocOrder) @return
	//   */
	//  boolean canBeWalkedInNaturalDocOrder()
	//  {
	//
	//    if (null != m_firstWalker)
	//    {
	//      AxesWalker walker = m_firstWalker;
	//      int prevAxis = -1;
	//      boolean prevIsSimpleDownAxis = true;
	//
	//      for(int i = 0; null != walker; i++)
	//      {
	//        int axis = walker.getAxis();
	//        
	//        if(walker.isDocOrdered())
	//        {
	//          boolean isSimpleDownAxis = ((axis == Axis.CHILD)
	//                                   || (axis == Axis.SELF)
	//                                   || (axis == Axis.ROOT));
	//          // Catching the filtered list here is only OK because
	//          // FilterExprWalker#isDocOrdered() did the right thing.
	//          if(isSimpleDownAxis || (axis == -1))
	//            walker = walker.getNextWalker();
	//          else
	//          {
	//            boolean isLastWalker = (null == walker.getNextWalker());
	//            if(isLastWalker)
	//            {
	//              if(walker.isDocOrdered() && (axis == Axis.DESCENDANT || 
	//                 axis == Axis.DESCENDANTORSELF || axis == Axis.DESCENDANTSFROMROOT
	//                 || axis == Axis.DESCENDANTSORSELFFROMROOT) || (axis == Axis.ATTRIBUTE))
	//                return true;
	//            }
	//            return false;
	//          }
	//        }
	//        else
	//          return false;
	//      }
	//      return true;
	//    }
	//    return false;
	//  }

	  /// <summary>
	  /// This function is used to perform some extra analysis of the iterator.
	  /// </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		base.fixupVariables(vars, globalsSize);

		int analysis = AnalysisBits;
		if (WalkerFactory.isNaturalDocOrder(analysis))
		{
			m_inNaturalOrderStatic = true;
		}
		else
		{
			m_inNaturalOrderStatic = false;
			// System.out.println("Setting natural doc order to false: "+
			//    WalkerFactory.getAnalysisString(analysis));
		}

	  }

	}

}