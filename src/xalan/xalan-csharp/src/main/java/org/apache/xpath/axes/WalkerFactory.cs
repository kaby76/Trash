using System;
using System.Text;

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
 * $Id: WalkerFactory.java 469314 2006-10-30 23:31:59Z minchau $
 */
namespace org.apache.xpath.axes
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using Axis = org.apache.xml.dtm.Axis;
	using DTMFilter = org.apache.xml.dtm.DTMFilter;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using Expression = org.apache.xpath.Expression;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using FunctionTable = org.apache.xpath.compiler.FunctionTable;
	using OpCodes = org.apache.xpath.compiler.OpCodes;
	using OpMap = org.apache.xpath.compiler.OpMap;
	using XNumber = org.apache.xpath.objects.XNumber;
	using ContextMatchStepPattern = org.apache.xpath.patterns.ContextMatchStepPattern;
	using FunctionPattern = org.apache.xpath.patterns.FunctionPattern;
	using NodeTest = org.apache.xpath.patterns.NodeTest;
	using StepPattern = org.apache.xpath.patterns.StepPattern;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// This class is both a factory for XPath location path expressions,
	/// which are built from the opcode map output, and an analysis engine
	/// for the location path expressions in order to provide optimization hints.
	/// </summary>
	public class WalkerFactory
	{

	  /// <summary>
	  /// This method is for building an array of possible levels
	  /// where the target element(s) could be found for a match. </summary>
	  /// <param name="lpi"> The owning location path iterator. </param>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="stepOpCodePos"> The opcode position for the step.
	  /// </param>
	  /// <returns> non-null AxesWalker derivative.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static AxesWalker loadOneWalker(WalkingIterator lpi, org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos) throws javax.xml.transform.TransformerException
	  internal static AxesWalker loadOneWalker(WalkingIterator lpi, Compiler compiler, int stepOpCodePos)
	  {

		AxesWalker firstWalker = null;
		int stepType = compiler.getOp(stepOpCodePos);

		if (stepType != OpCodes.ENDOP)
		{

		  // m_axesWalkers = new AxesWalker[1];
		  // As we unwind from the recursion, create the iterators.
		  firstWalker = createDefaultWalker(compiler, stepType, lpi, 0);

		  firstWalker.init(compiler, stepOpCodePos, stepType);
		}

		return firstWalker;
	  }

	  /// <summary>
	  /// This method is for building an array of possible levels
	  /// where the target element(s) could be found for a match. </summary>
	  /// <param name="lpi"> The owning location path iterator object. </param>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="stepOpCodePos"> The opcode position for the step. </param>
	  /// <param name="stepIndex"> The top-level step index withing the iterator.
	  /// </param>
	  /// <returns> non-null AxesWalker derivative.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException">
	  /// @xsl.usage advanced </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static AxesWalker loadWalkers(WalkingIterator lpi, org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos, int stepIndex) throws javax.xml.transform.TransformerException
	  internal static AxesWalker loadWalkers(WalkingIterator lpi, Compiler compiler, int stepOpCodePos, int stepIndex)
	  {

		int stepType;
		AxesWalker firstWalker = null;
		AxesWalker walker, prevWalker = null;

		int analysis = analyze(compiler, stepOpCodePos, stepIndex);

		while (OpCodes.ENDOP != (stepType = compiler.getOp(stepOpCodePos)))
		{
		  walker = createDefaultWalker(compiler, stepOpCodePos, lpi, analysis);

		  walker.init(compiler, stepOpCodePos, stepType);
		  walker.exprSetParent(lpi);

		  // walker.setAnalysis(analysis);
		  if (null == firstWalker)
		  {
			firstWalker = walker;
		  }
		  else
		  {
			prevWalker.NextWalker = walker;
			walker.PrevWalker = prevWalker;
		  }

		  prevWalker = walker;
		  stepOpCodePos = compiler.getNextStepPos(stepOpCodePos);

		  if (stepOpCodePos < 0)
		  {
			break;
		  }
		}

		return firstWalker;
	  }

	  public static bool isSet(int analysis, int bits)
	  {
		return (0 != (analysis & bits));
	  }

	  public static void diagnoseIterator(string name, int analysis, Compiler compiler)
	  {
		Console.WriteLine(compiler.ToString() + ", " + name + ", " + Convert.ToString(analysis, 2) + ", " + getAnalysisString(analysis));
	  }

	  /// <summary>
	  /// Create a new LocPathIterator iterator.  The exact type of iterator
	  /// returned is based on an analysis of the XPath operations.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="opPos"> The position of the operation code for this itterator.
	  /// </param>
	  /// <returns> non-null reference to a LocPathIterator or derivative.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static org.apache.xml.dtm.DTMIterator newDTMIterator(org.apache.xpath.compiler.Compiler compiler, int opPos, boolean isTopLevel) throws javax.xml.transform.TransformerException
	  public static DTMIterator newDTMIterator(Compiler compiler, int opPos, bool isTopLevel)
	  {

		int firstStepPos = OpMap.getFirstChildPos(opPos);
		int analysis = analyze(compiler, firstStepPos, 0);
		bool isOneStep = WalkerFactory.isOneStep(analysis);
		DTMIterator iter;

		// Is the iteration a one-step attribute pattern (i.e. select="@foo")?
		if (isOneStep && walksSelfOnly(analysis) && isWild(analysis) && !hasPredicate(analysis))
		{
		  if (DEBUG_ITERATOR_CREATION)
		  {
			diagnoseIterator("SelfIteratorNoPredicate", analysis, compiler);
		  }

		  // Then use a simple iteration of the attributes, with node test 
		  // and predicate testing.
		  iter = new SelfIteratorNoPredicate(compiler, opPos, analysis);
		}
		// Is the iteration exactly one child step?
		else if (walksChildrenOnly(analysis) && isOneStep)
		{

		  // Does the pattern specify *any* child with no predicate? (i.e. select="child::node()".
		  if (isWild(analysis) && !hasPredicate(analysis))
		  {
			if (DEBUG_ITERATOR_CREATION)
			{
			  diagnoseIterator("ChildIterator", analysis, compiler);
			}

			// Use simple child iteration without any test.
			iter = new ChildIterator(compiler, opPos, analysis);
		  }
		  else
		  {
			if (DEBUG_ITERATOR_CREATION)
			{
			  diagnoseIterator("ChildTestIterator", analysis, compiler);
			}

			// Else use simple node test iteration with predicate test.
			iter = new ChildTestIterator(compiler, opPos, analysis);
		  }
		}
		// Is the iteration a one-step attribute pattern (i.e. select="@foo")?
		else if (isOneStep && walksAttributes(analysis))
		{
		  if (DEBUG_ITERATOR_CREATION)
		  {
			diagnoseIterator("AttributeIterator", analysis, compiler);
		  }

		  // Then use a simple iteration of the attributes, with node test 
		  // and predicate testing.
		  iter = new AttributeIterator(compiler, opPos, analysis);
		}
		else if (isOneStep && !walksFilteredList(analysis))
		{
		  if (!walksNamespaces(analysis) && (walksInDocOrder(analysis) || isSet(analysis, BIT_PARENT)))
		  {
			if (false || DEBUG_ITERATOR_CREATION)
			{
			  diagnoseIterator("OneStepIteratorForward", analysis, compiler);
			}

			// Then use a simple iteration of the attributes, with node test 
			// and predicate testing.
			iter = new OneStepIteratorForward(compiler, opPos, analysis);
		  }
		  else
		  {
			if (false || DEBUG_ITERATOR_CREATION)
			{
			  diagnoseIterator("OneStepIterator", analysis, compiler);
			}

			// Then use a simple iteration of the attributes, with node test 
			// and predicate testing.
			iter = new OneStepIterator(compiler, opPos, analysis);
		  }
		}

		// Analysis of "//center":
		// bits: 1001000000001010000000000000011
		// count: 3
		// root
		// child:node()
		// BIT_DESCENDANT_OR_SELF
		// It's highly possible that we should have a seperate bit set for 
		// "//foo" patterns.
		// For at least the time being, we can't optimize patterns like 
		// "//table[3]", because this has to be analyzed as 
		// "/descendant-or-self::node()/table[3]" in order for the indexes 
		// to work right.
		else if (isOptimizableForDescendantIterator(compiler, firstStepPos, 0))
		{
		  if (DEBUG_ITERATOR_CREATION)
		  {
			diagnoseIterator("DescendantIterator", analysis, compiler);
		  }

		  iter = new DescendantIterator(compiler, opPos, analysis);
		}
		else
		{
		  if (isNaturalDocOrder(compiler, firstStepPos, 0, analysis))
		  {
			if (false || DEBUG_ITERATOR_CREATION)
			{
			  diagnoseIterator("WalkingIterator", analysis, compiler);
			}

			iter = new WalkingIterator(compiler, opPos, analysis, true);
		  }
		  else
		  {
	//        if (DEBUG_ITERATOR_CREATION)
	//          diagnoseIterator("MatchPatternIterator", analysis, compiler);
	//
	//        return new MatchPatternIterator(compiler, opPos, analysis);
			if (DEBUG_ITERATOR_CREATION)
			{
			  diagnoseIterator("WalkingIteratorSorted", analysis, compiler);
			}

			iter = new WalkingIteratorSorted(compiler, opPos, analysis, true);
		  }
		}
		if (iter is LocPathIterator)
		{
		  ((LocPathIterator)iter).IsTopLevel = isTopLevel;
		}

		return iter;
	  }

	  /// <summary>
	  /// Special purpose function to see if we can optimize the pattern for 
	  /// a DescendantIterator.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="stepOpCodePos"> The opcode position for the step.
	  /// </param>
	  /// <returns> 32 bits as an integer that give information about the location
	  /// path as a whole.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static int getAxisFromStep(org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos) throws javax.xml.transform.TransformerException
	  public static int getAxisFromStep(Compiler compiler, int stepOpCodePos)
	  {

		int stepType = compiler.getOp(stepOpCodePos);

		switch (stepType)
		{
		case OpCodes.FROM_FOLLOWING :
		  return Axis.FOLLOWING;
		case OpCodes.FROM_FOLLOWING_SIBLINGS :
		  return Axis.FOLLOWINGSIBLING;
		case OpCodes.FROM_PRECEDING :
		  return Axis.PRECEDING;
		case OpCodes.FROM_PRECEDING_SIBLINGS :
		  return Axis.PRECEDINGSIBLING;
		case OpCodes.FROM_PARENT :
		  return Axis.PARENT;
		case OpCodes.FROM_NAMESPACE :
		  return Axis.NAMESPACE;
		case OpCodes.FROM_ANCESTORS :
		  return Axis.ANCESTOR;
		case OpCodes.FROM_ANCESTORS_OR_SELF :
		  return Axis.ANCESTORORSELF;
		case OpCodes.FROM_ATTRIBUTES :
		  return Axis.ATTRIBUTE;
		case OpCodes.FROM_ROOT :
		  return Axis.ROOT;
		case OpCodes.FROM_CHILDREN :
		  return Axis.CHILD;
		case OpCodes.FROM_DESCENDANTS_OR_SELF :
		  return Axis.DESCENDANTORSELF;
		case OpCodes.FROM_DESCENDANTS :
		  return Axis.DESCENDANT;
		case OpCodes.FROM_SELF :
		  return Axis.SELF;
		case OpCodes.OP_EXTFUNCTION :
		case OpCodes.OP_FUNCTION :
		case OpCodes.OP_GROUP :
		case OpCodes.OP_VARIABLE :
		  return Axis.FILTEREDLIST;
		}

		throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, new object[]{Convert.ToString(stepType)})); //"Programmer's assertion: unknown opcode: "
								   //+ stepType);
	  }

		/// <summary>
		/// Get a corresponding BIT_XXX from an axis. </summary>
		/// <param name="axis"> One of Axis.ANCESTOR, etc. </param>
		/// <returns> One of BIT_ANCESTOR, etc. </returns>
		public static int getAnalysisBitFromAxes(int axis)
		{
		  switch (axis) // Generate new traverser
		  {
			case Axis.ANCESTOR :
			  return BIT_ANCESTOR;
			case Axis.ANCESTORORSELF :
			  return BIT_ANCESTOR_OR_SELF;
			case Axis.ATTRIBUTE :
			  return BIT_ATTRIBUTE;
			case Axis.CHILD :
			  return BIT_CHILD;
			case Axis.DESCENDANT :
			  return BIT_DESCENDANT;
			case Axis.DESCENDANTORSELF :
			  return BIT_DESCENDANT_OR_SELF;
			case Axis.FOLLOWING :
			  return BIT_FOLLOWING;
			case Axis.FOLLOWINGSIBLING :
			  return BIT_FOLLOWING_SIBLING;
			case Axis.NAMESPACE :
			case Axis.NAMESPACEDECLS :
			  return BIT_NAMESPACE;
			case Axis.PARENT :
			  return BIT_PARENT;
			case Axis.PRECEDING :
			  return BIT_PRECEDING;
			case Axis.PRECEDINGSIBLING :
			  return BIT_PRECEDING_SIBLING;
			case Axis.SELF :
			  return BIT_SELF;
			case Axis.ALLFROMNODE :
			  return BIT_DESCENDANT_OR_SELF;
			  // case Axis.PRECEDINGANDANCESTOR :
			case Axis.DESCENDANTSFROMROOT :
			case Axis.ALL :
			case Axis.DESCENDANTSORSELFFROMROOT :
			  return BIT_ANY_DESCENDANT_FROM_ROOT;
			case Axis.ROOT :
			  return BIT_ROOT;
			case Axis.FILTEREDLIST :
			  return BIT_FILTER;
			default :
			  return BIT_FILTER;
		  }
		}

	  internal static bool functionProximateOrContainsProximate(Compiler compiler, int opPos)
	  {
		int endFunc = opPos + compiler.getOp(opPos + 1) - 1;
		opPos = OpMap.getFirstChildPos(opPos);
		int funcID = compiler.getOp(opPos);
		//  System.out.println("funcID: "+funcID);
		//  System.out.println("opPos: "+opPos);
		//  System.out.println("endFunc: "+endFunc);
		switch (funcID)
		{
		  case FunctionTable.FUNC_LAST:
		  case FunctionTable.FUNC_POSITION:
			return true;
		  default:
			opPos++;
			int i = 0;
			for (int p = opPos; p < endFunc; p = compiler.getNextOpPos(p), i++)
			{
			  int innerExprOpPos = p + 2;
			  int argOp = compiler.getOp(innerExprOpPos);
			  bool prox = isProximateInnerExpr(compiler, innerExprOpPos);
			  if (prox)
			  {
				return true;
			  }
			}

		break;
		}
		return false;
	  }

	  internal static bool isProximateInnerExpr(Compiler compiler, int opPos)
	  {
		int op = compiler.getOp(opPos);
		int innerExprOpPos = opPos + 2;
		switch (op)
		{
		  case OpCodes.OP_ARGUMENT:
			if (isProximateInnerExpr(compiler, innerExprOpPos))
			{
			  return true;
			}
			break;
		  case OpCodes.OP_VARIABLE:
		  case OpCodes.OP_NUMBERLIT:
		  case OpCodes.OP_LITERAL:
		  case OpCodes.OP_LOCATIONPATH:
			break; // OK
		  case OpCodes.OP_FUNCTION:
			bool isProx = functionProximateOrContainsProximate(compiler, opPos);
			if (isProx)
			{
			  return true;
			}
			break;
		  case OpCodes.OP_GT:
		  case OpCodes.OP_GTE:
		  case OpCodes.OP_LT:
		  case OpCodes.OP_LTE:
		  case OpCodes.OP_EQUALS:
			int leftPos = OpMap.getFirstChildPos(op);
			int rightPos = compiler.getNextOpPos(leftPos);
			isProx = isProximateInnerExpr(compiler, leftPos);
			if (isProx)
			{
			  return true;
			}
			isProx = isProximateInnerExpr(compiler, rightPos);
			if (isProx)
			{
			  return true;
			}
			break;
		  default:
			return true; // be conservative...
		}
		return false;
	  }

	  /// <summary>
	  /// Tell if the predicates need to have proximity knowledge.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static boolean mightBeProximate(org.apache.xpath.compiler.Compiler compiler, int opPos, int stepType) throws javax.xml.transform.TransformerException
	  public static bool mightBeProximate(Compiler compiler, int opPos, int stepType)
	  {

		bool mightBeProximate = false;
		int argLen;

		switch (stepType)
		{
		case OpCodes.OP_VARIABLE :
		case OpCodes.OP_EXTFUNCTION :
		case OpCodes.OP_FUNCTION :
		case OpCodes.OP_GROUP :
		  argLen = compiler.getArgLength(opPos);
		  break;
		default :
		  argLen = compiler.getArgLengthOfStep(opPos);
	  break;
		}

		int predPos = compiler.getFirstPredicateOpPos(opPos);
		int count = 0;

		while (OpCodes.OP_PREDICATE == compiler.getOp(predPos))
		{
		  count++;

		  int innerExprOpPos = predPos + 2;
		  int predOp = compiler.getOp(innerExprOpPos);

		  switch (predOp)
		  {
			case OpCodes.OP_VARIABLE:
				return true; // Would need more smarts to tell if this could be a number or not!
			case OpCodes.OP_LOCATIONPATH:
			  // OK.
			  break;
			case OpCodes.OP_NUMBER:
			case OpCodes.OP_NUMBERLIT:
			  return true; // that's all she wrote!
			case OpCodes.OP_FUNCTION:
			  bool isProx = functionProximateOrContainsProximate(compiler, innerExprOpPos);
			  if (isProx)
			  {
				return true;
			  }
			  break;
			case OpCodes.OP_GT:
			case OpCodes.OP_GTE:
			case OpCodes.OP_LT:
			case OpCodes.OP_LTE:
			case OpCodes.OP_EQUALS:
			  int leftPos = OpMap.getFirstChildPos(innerExprOpPos);
			  int rightPos = compiler.getNextOpPos(leftPos);
			  isProx = isProximateInnerExpr(compiler, leftPos);
			  if (isProx)
			  {
				return true;
			  }
			  isProx = isProximateInnerExpr(compiler, rightPos);
			  if (isProx)
			  {
				return true;
			  }
			  break;
			default:
			  return true; // be conservative...
		  }

		  predPos = compiler.getNextOpPos(predPos);
		}

		return mightBeProximate;
	  }

	  /// <summary>
	  /// Special purpose function to see if we can optimize the pattern for 
	  /// a DescendantIterator.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="stepOpCodePos"> The opcode position for the step. </param>
	  /// <param name="stepIndex"> The top-level step index withing the iterator.
	  /// </param>
	  /// <returns> 32 bits as an integer that give information about the location
	  /// path as a whole.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static boolean isOptimizableForDescendantIterator(org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos, int stepIndex) throws javax.xml.transform.TransformerException
	  private static bool isOptimizableForDescendantIterator(Compiler compiler, int stepOpCodePos, int stepIndex)
	  {

		int stepType;
		int stepCount = 0;
		bool foundDorDS = false;
		bool foundSelf = false;
		bool foundDS = false;

		int nodeTestType = OpCodes.NODETYPE_NODE;

		while (OpCodes.ENDOP != (stepType = compiler.getOp(stepOpCodePos)))
		{
		  // The DescendantIterator can only do one node test.  If there's more 
		  // than one, use another iterator.
		  if (nodeTestType != OpCodes.NODETYPE_NODE && nodeTestType != OpCodes.NODETYPE_ROOT)
		  {
			return false;
		  }

		  stepCount++;
		  if (stepCount > 3)
		  {
			return false;
		  }

		  bool mightBeProximate = WalkerFactory.mightBeProximate(compiler, stepOpCodePos, stepType);
		  if (mightBeProximate)
		  {
			return false;
		  }

		  switch (stepType)
		  {
		  case OpCodes.FROM_FOLLOWING :
		  case OpCodes.FROM_FOLLOWING_SIBLINGS :
		  case OpCodes.FROM_PRECEDING :
		  case OpCodes.FROM_PRECEDING_SIBLINGS :
		  case OpCodes.FROM_PARENT :
		  case OpCodes.OP_VARIABLE :
		  case OpCodes.OP_EXTFUNCTION :
		  case OpCodes.OP_FUNCTION :
		  case OpCodes.OP_GROUP :
		  case OpCodes.FROM_NAMESPACE :
		  case OpCodes.FROM_ANCESTORS :
		  case OpCodes.FROM_ANCESTORS_OR_SELF :
		  case OpCodes.FROM_ATTRIBUTES :
		  case OpCodes.MATCH_ATTRIBUTE :
		  case OpCodes.MATCH_ANY_ANCESTOR :
		  case OpCodes.MATCH_IMMEDIATE_ANCESTOR :
			return false;
		  case OpCodes.FROM_ROOT :
			if (1 != stepCount)
			{
			  return false;
			}
			break;
		  case OpCodes.FROM_CHILDREN :
			if (!foundDS && !(foundDorDS && foundSelf))
			{
			  return false;
			}
			break;
		  case OpCodes.FROM_DESCENDANTS_OR_SELF :
			foundDS = true;
			  goto case org.apache.xpath.compiler.OpCodes.FROM_DESCENDANTS;
		  case OpCodes.FROM_DESCENDANTS :
			if (3 == stepCount)
			{
			  return false;
			}
			foundDorDS = true;
			break;
		  case OpCodes.FROM_SELF :
			if (1 != stepCount)
			{
			  return false;
			}
			foundSelf = true;
			break;
		  default :
			throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, new object[]{Convert.ToString(stepType)})); //"Programmer's assertion: unknown opcode: "
									  // + stepType);
		  }

		  nodeTestType = compiler.getStepTestType(stepOpCodePos);

		  int nextStepOpCodePos = compiler.getNextStepPos(stepOpCodePos);

		  if (nextStepOpCodePos < 0)
		  {
			break;
		  }

		  if (OpCodes.ENDOP != compiler.getOp(nextStepOpCodePos))
		  {
			if (compiler.countPredicates(stepOpCodePos) > 0)
			{
			  return false;
			}
		  }

		  stepOpCodePos = nextStepOpCodePos;
		}

		return true;
	  }

	  /// <summary>
	  /// Analyze the location path and return 32 bits that give information about
	  /// the location path as a whole.  See the BIT_XXX constants for meaning about
	  /// each of the bits.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="stepOpCodePos"> The opcode position for the step. </param>
	  /// <param name="stepIndex"> The top-level step index withing the iterator.
	  /// </param>
	  /// <returns> 32 bits as an integer that give information about the location
	  /// path as a whole.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static int analyze(org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos, int stepIndex) throws javax.xml.transform.TransformerException
	  private static int analyze(Compiler compiler, int stepOpCodePos, int stepIndex)
	  {

		int stepType;
		int stepCount = 0;
		int analysisResult = 0x00000000; // 32 bits of analysis

		while (OpCodes.ENDOP != (stepType = compiler.getOp(stepOpCodePos)))
		{
		  stepCount++;

		  // String namespace = compiler.getStepNS(stepOpCodePos);
		  // boolean isNSWild = (null != namespace) 
		  //                   ? namespace.equals(NodeTest.WILD) : false;
		  // String localname = compiler.getStepLocalName(stepOpCodePos);
		  // boolean isWild = (null != localname) ? localname.equals(NodeTest.WILD) : false;
		  bool predAnalysis = analyzePredicate(compiler, stepOpCodePos, stepType);

		  if (predAnalysis)
		  {
			analysisResult |= BIT_PREDICATE;
		  }

		  switch (stepType)
		  {
		  case OpCodes.OP_VARIABLE :
		  case OpCodes.OP_EXTFUNCTION :
		  case OpCodes.OP_FUNCTION :
		  case OpCodes.OP_GROUP :
			analysisResult |= BIT_FILTER;
			break;
		  case OpCodes.FROM_ROOT :
			analysisResult |= BIT_ROOT;
			break;
		  case OpCodes.FROM_ANCESTORS :
			analysisResult |= BIT_ANCESTOR;
			break;
		  case OpCodes.FROM_ANCESTORS_OR_SELF :
			analysisResult |= BIT_ANCESTOR_OR_SELF;
			break;
		  case OpCodes.FROM_ATTRIBUTES :
			analysisResult |= BIT_ATTRIBUTE;
			break;
		  case OpCodes.FROM_NAMESPACE :
			analysisResult |= BIT_NAMESPACE;
			break;
		  case OpCodes.FROM_CHILDREN :
			analysisResult |= BIT_CHILD;
			break;
		  case OpCodes.FROM_DESCENDANTS :
			analysisResult |= BIT_DESCENDANT;
			break;
		  case OpCodes.FROM_DESCENDANTS_OR_SELF :

			// Use a special bit to to make sure we get the right analysis of "//foo".
			if (2 == stepCount && BIT_ROOT == analysisResult)
			{
			  analysisResult |= BIT_ANY_DESCENDANT_FROM_ROOT;
			}

			analysisResult |= BIT_DESCENDANT_OR_SELF;
			break;
		  case OpCodes.FROM_FOLLOWING :
			analysisResult |= BIT_FOLLOWING;
			break;
		  case OpCodes.FROM_FOLLOWING_SIBLINGS :
			analysisResult |= BIT_FOLLOWING_SIBLING;
			break;
		  case OpCodes.FROM_PRECEDING :
			analysisResult |= BIT_PRECEDING;
			break;
		  case OpCodes.FROM_PRECEDING_SIBLINGS :
			analysisResult |= BIT_PRECEDING_SIBLING;
			break;
		  case OpCodes.FROM_PARENT :
			analysisResult |= BIT_PARENT;
			break;
		  case OpCodes.FROM_SELF :
			analysisResult |= BIT_SELF;
			break;
		  case OpCodes.MATCH_ATTRIBUTE :
			analysisResult |= (BIT_MATCH_PATTERN | BIT_ATTRIBUTE);
			break;
		  case OpCodes.MATCH_ANY_ANCESTOR :
			analysisResult |= (BIT_MATCH_PATTERN | BIT_ANCESTOR);
			break;
		  case OpCodes.MATCH_IMMEDIATE_ANCESTOR :
			analysisResult |= (BIT_MATCH_PATTERN | BIT_PARENT);
			break;
		  default :
			throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, new object[]{Convert.ToString(stepType)})); //"Programmer's assertion: unknown opcode: "
									   //+ stepType);
		  }

		  if (OpCodes.NODETYPE_NODE == compiler.getOp(stepOpCodePos + 3)) // child::node()
		  {
			analysisResult |= BIT_NODETEST_ANY;
		  }

		  stepOpCodePos = compiler.getNextStepPos(stepOpCodePos);

		  if (stepOpCodePos < 0)
		  {
			break;
		  }
		}

		analysisResult |= (stepCount & BITS_COUNT);

		return analysisResult;
	  }

	  /// <summary>
	  /// Tell if the given axis goes downword.  Bogus name, if you can think of 
	  /// a better one, please do tell.  This really has to do with inverting 
	  /// attribute axis. </summary>
	  /// <param name="axis"> One of Axis.XXX. </param>
	  /// <returns> true if the axis is not a child axis and does not go up from 
	  /// the axis root. </returns>
	  public static bool isDownwardAxisOfMany(int axis)
	  {
		return ((Axis.DESCENDANTORSELF == axis) || (Axis.DESCENDANT == axis) || (Axis.FOLLOWING == axis) || (Axis.PRECEDING == axis));
	  }

	  /// <summary>
	  /// Read a <a href="http://www.w3.org/TR/xpath#location-paths">LocationPath</a>
	  /// as a generalized match pattern.  What this means is that the LocationPath
	  /// is read backwards, as a test on a given node, to see if it matches the
	  /// criteria of the selection, and ends up at the context node.  Essentially,
	  /// this is a backwards query from a given node, to find the context node.
	  /// <para>So, the selection "foo/daz[2]" is, in non-abreviated expanded syntax,
	  /// "self::node()/following-sibling::foo/child::daz[position()=2]".
	  /// Taking this as a match pattern for a probable node, it works out to
	  /// "self::daz/parent::foo[child::daz[position()=2 and isPrevStepNode()]
	  /// precedingSibling::node()[isContextNodeOfLocationPath()]", adding magic
	  /// isPrevStepNode and isContextNodeOfLocationPath operations.  Predicates in
	  /// the location path have to be executed by the following step,
	  /// because they have to know the context of their execution.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="mpi"> The MatchPatternIterator to which the steps will be attached. </param>
	  /// <param name="compiler"> The compiler that holds the syntax tree/op map to
	  /// construct from. </param>
	  /// <param name="stepOpCodePos"> The current op code position within the opmap. </param>
	  /// <param name="stepIndex"> The top-level step index withing the iterator.
	  /// </param>
	  /// <returns> A StepPattern object, which may contain relative StepPatterns.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static org.apache.xpath.patterns.StepPattern loadSteps(MatchPatternIterator mpi, org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos, int stepIndex) throws javax.xml.transform.TransformerException
	  internal static StepPattern loadSteps(MatchPatternIterator mpi, Compiler compiler, int stepOpCodePos, int stepIndex)
	  {
		if (DEBUG_PATTERN_CREATION)
		{
		  Console.WriteLine("================");
		  Console.WriteLine("loadSteps for: " + compiler.PatternString);
		}
		int stepType;
		StepPattern step = null;
		StepPattern firstStep = null, prevStep = null;
		int analysis = analyze(compiler, stepOpCodePos, stepIndex);

		while (OpCodes.ENDOP != (stepType = compiler.getOp(stepOpCodePos)))
		{
		  step = createDefaultStepPattern(compiler, stepOpCodePos, mpi, analysis, firstStep, prevStep);

		  if (null == firstStep)
		  {
			firstStep = step;
		  }
		  else
		  {

			//prevStep.setNextWalker(step);
			step.RelativePathPattern = prevStep;
		  }

		  prevStep = step;
		  stepOpCodePos = compiler.getNextStepPos(stepOpCodePos);

		  if (stepOpCodePos < 0)
		  {
			break;
		  }
		}

		int axis = Axis.SELF;
		int paxis = Axis.SELF;
		StepPattern tail = step;
		for (StepPattern pat = step; null != pat; pat = pat.RelativePathPattern)
		{
		  int nextAxis = pat.Axis;
		  //int nextPaxis = pat.getPredicateAxis();
		  pat.Axis = axis;

		  // The predicate axis can't be moved!!!  Test Axes103
		  // pat.setPredicateAxis(paxis);

		  // If we have an attribute or namespace axis that went up, then 
		  // it won't find the attribute in the inverse, since the select-to-match
		  // axes are not invertable (an element is a parent of an attribute, but 
		  // and attribute is not a child of an element).
		  // If we don't do the magic below, then "@*/ancestor-or-self::*" gets
		  // inverted for match to "self::*/descendant-or-self::@*/parent::node()",
		  // which obviously won't work.
		  // So we will rewrite this as:
		  // "self::*/descendant-or-self::*/attribute::*/parent::node()"
		  // Child has to be rewritten a little differently:
		  // select: "@*/parent::*"
		  // inverted match: "self::*/child::@*/parent::node()"
		  // rewrite: "self::*/attribute::*/parent::node()"
		  // Axes that go down in the select, do not have to have special treatment 
		  // in the rewrite. The following inverted match will still not select 
		  // anything.
		  // select: "@*/child::*"
		  // inverted match: "self::*/parent::@*/parent::node()"
		  // Lovely business, this.
		  // -sb
		  int whatToShow = pat.WhatToShow;
		  if (whatToShow == DTMFilter.SHOW_ATTRIBUTE || whatToShow == DTMFilter.SHOW_NAMESPACE)
		  {
			int newAxis = (whatToShow == DTMFilter.SHOW_ATTRIBUTE) ? Axis.ATTRIBUTE : Axis.NAMESPACE;
			if (isDownwardAxisOfMany(axis))
			{
			  StepPattern attrPat = new StepPattern(whatToShow, pat.Namespace, pat.LocalName, newAxis, 0); // don't care about the predicate axis
			  XNumber score = pat.StaticScore;
			  pat.Namespace = null;
			  pat.LocalName = NodeTest.WILD;
			  attrPat.Predicates = pat.Predicates;
			  pat.Predicates = null;
			  pat.WhatToShow = DTMFilter.SHOW_ELEMENT;
			  StepPattern rel = pat.RelativePathPattern;
			  pat.RelativePathPattern = attrPat;
			  attrPat.RelativePathPattern = rel;
			  attrPat.StaticScore = score;

			  // This is needed to inverse a following pattern, because of the 
			  // wacky Xalan rules for following from an attribute.  See axes108.
			  // By these rules, following from an attribute is not strictly 
			  // inverseable.
			  if (Axis.PRECEDING == pat.Axis)
			  {
				pat.Axis = Axis.PRECEDINGANDANCESTOR;
			  }

			  else if (Axis.DESCENDANT == pat.Axis)
			  {
				pat.Axis = Axis.DESCENDANTORSELF;
			  }

			  pat = attrPat;
			}
			else if (Axis.CHILD == pat.Axis)
			{
			  // In this case just change the axis.
			  // pat.setWhatToShow(whatToShow);
			  pat.Axis = Axis.ATTRIBUTE;
			}
		  }
		  axis = nextAxis;
		  //paxis = nextPaxis;
		  tail = pat;
		}

		if (axis < Axis.ALL)
		{
		  StepPattern selfPattern = new ContextMatchStepPattern(axis, paxis);
		  // We need to keep the new nodetest from affecting the score...
		  XNumber score = tail.StaticScore;
		  tail.RelativePathPattern = selfPattern;
		  tail.StaticScore = score;
		  selfPattern.StaticScore = score;
		}

		if (DEBUG_PATTERN_CREATION)
		{
		  Console.WriteLine("Done loading steps: " + step.ToString());

		  Console.WriteLine("");
		}
		return step; // start from last pattern?? //firstStep;
	  }

	  /// <summary>
	  /// Create a StepPattern that is contained within a LocationPath.
	  /// 
	  /// </summary>
	  /// <param name="compiler"> The compiler that holds the syntax tree/op map to
	  /// construct from. </param>
	  /// <param name="stepOpCodePos"> The current op code position within the opmap. </param>
	  /// <param name="mpi"> The MatchPatternIterator to which the steps will be attached. </param>
	  /// <param name="analysis"> 32 bits of analysis, from which the type of AxesWalker
	  ///                 may be influenced. </param>
	  /// <param name="tail"> The step that is the first step analyzed, but the last 
	  ///                  step in the relative match linked list, i.e. the tail.
	  ///                  May be null. </param>
	  /// <param name="head"> The step that is the current head of the relative 
	  ///                 match step linked list.
	  ///                 May be null.
	  /// </param>
	  /// <returns> the head of the list.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static org.apache.xpath.patterns.StepPattern createDefaultStepPattern(org.apache.xpath.compiler.Compiler compiler, int opPos, MatchPatternIterator mpi, int analysis, org.apache.xpath.patterns.StepPattern tail, org.apache.xpath.patterns.StepPattern head) throws javax.xml.transform.TransformerException
	  private static StepPattern createDefaultStepPattern(Compiler compiler, int opPos, MatchPatternIterator mpi, int analysis, StepPattern tail, StepPattern head)
	  {

		int stepType = compiler.getOp(opPos);
		bool simpleInit = false;
		bool prevIsOneStepDown = true;

		int whatToShow = compiler.getWhatToShow(opPos);
		StepPattern ai = null;
		int axis, predicateAxis;

		switch (stepType)
		{
		case OpCodes.OP_VARIABLE :
		case OpCodes.OP_EXTFUNCTION :
		case OpCodes.OP_FUNCTION :
		case OpCodes.OP_GROUP :
		  prevIsOneStepDown = false;

		  Expression expr;

		  switch (stepType)
		  {
		  case OpCodes.OP_VARIABLE :
		  case OpCodes.OP_EXTFUNCTION :
		  case OpCodes.OP_FUNCTION :
		  case OpCodes.OP_GROUP :
			expr = compiler.compile(opPos);
			break;
		  default :
			expr = compiler.compile(opPos + 2);
		break;
		  }

		  axis = Axis.FILTEREDLIST;
		  predicateAxis = Axis.FILTEREDLIST;
		  ai = new FunctionPattern(expr, axis, predicateAxis);
		  simpleInit = true;
		  break;
		case OpCodes.FROM_ROOT :
		  whatToShow = DTMFilter.SHOW_DOCUMENT | DTMFilter.SHOW_DOCUMENT_FRAGMENT;

		  axis = Axis.ROOT;
		  predicateAxis = Axis.ROOT;
		  ai = new StepPattern(DTMFilter.SHOW_DOCUMENT | DTMFilter.SHOW_DOCUMENT_FRAGMENT, axis, predicateAxis);
		  break;
		case OpCodes.FROM_ATTRIBUTES :
		  whatToShow = DTMFilter.SHOW_ATTRIBUTE;
		  axis = Axis.PARENT;
		  predicateAxis = Axis.ATTRIBUTE;
		  // ai = new StepPattern(whatToShow, Axis.SELF, Axis.SELF);
		  break;
		case OpCodes.FROM_NAMESPACE :
		  whatToShow = DTMFilter.SHOW_NAMESPACE;
		  axis = Axis.PARENT;
		  predicateAxis = Axis.NAMESPACE;
		  // ai = new StepPattern(whatToShow, axis, predicateAxis);
		  break;
		case OpCodes.FROM_ANCESTORS :
		  axis = Axis.DESCENDANT;
		  predicateAxis = Axis.ANCESTOR;
		  break;
		case OpCodes.FROM_CHILDREN :
		  axis = Axis.PARENT;
		  predicateAxis = Axis.CHILD;
		  break;
		case OpCodes.FROM_ANCESTORS_OR_SELF :
		  axis = Axis.DESCENDANTORSELF;
		  predicateAxis = Axis.ANCESTORORSELF;
		  break;
		case OpCodes.FROM_SELF :
		  axis = Axis.SELF;
		  predicateAxis = Axis.SELF;
		  break;
		case OpCodes.FROM_PARENT :
		  axis = Axis.CHILD;
		  predicateAxis = Axis.PARENT;
		  break;
		case OpCodes.FROM_PRECEDING_SIBLINGS :
		  axis = Axis.FOLLOWINGSIBLING;
		  predicateAxis = Axis.PRECEDINGSIBLING;
		  break;
		case OpCodes.FROM_PRECEDING :
		  axis = Axis.FOLLOWING;
		  predicateAxis = Axis.PRECEDING;
		  break;
		case OpCodes.FROM_FOLLOWING_SIBLINGS :
		  axis = Axis.PRECEDINGSIBLING;
		  predicateAxis = Axis.FOLLOWINGSIBLING;
		  break;
		case OpCodes.FROM_FOLLOWING :
		  axis = Axis.PRECEDING;
		  predicateAxis = Axis.FOLLOWING;
		  break;
		case OpCodes.FROM_DESCENDANTS_OR_SELF :
		  axis = Axis.ANCESTORORSELF;
		  predicateAxis = Axis.DESCENDANTORSELF;
		  break;
		case OpCodes.FROM_DESCENDANTS :
		  axis = Axis.ANCESTOR;
		  predicateAxis = Axis.DESCENDANT;
		  break;
		default :
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, new object[]{Convert.ToString(stepType)})); //"Programmer's assertion: unknown opcode: "
									 //+ stepType);
		}
		if (null == ai)
		{
		  whatToShow = compiler.getWhatToShow(opPos); // %REVIEW%
		  ai = new StepPattern(whatToShow, compiler.getStepNS(opPos), compiler.getStepLocalName(opPos), axis, predicateAxis);
		}

		if (false || DEBUG_PATTERN_CREATION)
		{
		  Console.Write("new step: " + ai);
		  Console.Write(", axis: " + Axis.getNames(ai.Axis));
		  Console.Write(", predAxis: " + Axis.getNames(ai.Axis));
		  Console.Write(", what: ");
		  Console.Write("    ");
		  ai.debugWhatToShow(ai.WhatToShow);
		}

		int argLen = compiler.getFirstPredicateOpPos(opPos);

		ai.Predicates = compiler.getCompiledPredicates(argLen);

		return ai;
	  }

	  /// <summary>
	  /// Analyze a step and give information about it's predicates.  Right now this
	  /// just returns true or false if the step has a predicate.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="opPos"> The opcode position for the step. </param>
	  /// <param name="stepType"> The type of step, one of OP_GROUP, etc.
	  /// </param>
	  /// <returns> true if step has a predicate.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static boolean analyzePredicate(org.apache.xpath.compiler.Compiler compiler, int opPos, int stepType) throws javax.xml.transform.TransformerException
	  internal static bool analyzePredicate(Compiler compiler, int opPos, int stepType)
	  {

		int argLen;

		switch (stepType)
		{
		case OpCodes.OP_VARIABLE :
		case OpCodes.OP_EXTFUNCTION :
		case OpCodes.OP_FUNCTION :
		case OpCodes.OP_GROUP :
		  argLen = compiler.getArgLength(opPos);
		  break;
		default :
		  argLen = compiler.getArgLengthOfStep(opPos);
	  break;
		}

		int pos = compiler.getFirstPredicateOpPos(opPos);
		int nPredicates = compiler.countPredicates(pos);

		return (nPredicates > 0) ? true : false;
	  }

	  /// <summary>
	  /// Create the proper Walker from the axes type.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="opPos"> The opcode position for the step. </param>
	  /// <param name="lpi"> The owning location path iterator. </param>
	  /// <param name="analysis"> 32 bits of analysis, from which the type of AxesWalker
	  ///                 may be influenced.
	  /// </param>
	  /// <returns> non-null reference to AxesWalker derivative. </returns>
	  /// <exception cref="RuntimeException"> if the input is bad. </exception>
	  private static AxesWalker createDefaultWalker(Compiler compiler, int opPos, WalkingIterator lpi, int analysis)
	  {

		AxesWalker ai = null;
		int stepType = compiler.getOp(opPos);

		/*
		System.out.println("0: "+compiler.getOp(opPos));
		System.out.println("1: "+compiler.getOp(opPos+1));
		System.out.println("2: "+compiler.getOp(opPos+2));
		System.out.println("3: "+compiler.getOp(opPos+3));
		System.out.println("4: "+compiler.getOp(opPos+4));
		System.out.println("5: "+compiler.getOp(opPos+5));
		*/
		bool simpleInit = false;
		int totalNumberWalkers = (analysis & BITS_COUNT);
		bool prevIsOneStepDown = true;

		switch (stepType)
		{
		case OpCodes.OP_VARIABLE :
		case OpCodes.OP_EXTFUNCTION :
		case OpCodes.OP_FUNCTION :
		case OpCodes.OP_GROUP :
		  prevIsOneStepDown = false;

		  if (DEBUG_WALKER_CREATION)
		  {
			Console.WriteLine("new walker:  FilterExprWalker: " + analysis + ", " + compiler.ToString());
		  }

		  ai = new FilterExprWalker(lpi);
		  simpleInit = true;
		  break;
		case OpCodes.FROM_ROOT :
		  ai = new AxesWalker(lpi, Axis.ROOT);
		  break;
		case OpCodes.FROM_ANCESTORS :
		  prevIsOneStepDown = false;
		  ai = new ReverseAxesWalker(lpi, Axis.ANCESTOR);
		  break;
		case OpCodes.FROM_ANCESTORS_OR_SELF :
		  prevIsOneStepDown = false;
		  ai = new ReverseAxesWalker(lpi, Axis.ANCESTORORSELF);
		  break;
		case OpCodes.FROM_ATTRIBUTES :
		  ai = new AxesWalker(lpi, Axis.ATTRIBUTE);
		  break;
		case OpCodes.FROM_NAMESPACE :
		  ai = new AxesWalker(lpi, Axis.NAMESPACE);
		  break;
		case OpCodes.FROM_CHILDREN :
		  ai = new AxesWalker(lpi, Axis.CHILD);
		  break;
		case OpCodes.FROM_DESCENDANTS :
		  prevIsOneStepDown = false;
		  ai = new AxesWalker(lpi, Axis.DESCENDANT);
		  break;
		case OpCodes.FROM_DESCENDANTS_OR_SELF :
		  prevIsOneStepDown = false;
		  ai = new AxesWalker(lpi, Axis.DESCENDANTORSELF);
		  break;
		case OpCodes.FROM_FOLLOWING :
		  prevIsOneStepDown = false;
		  ai = new AxesWalker(lpi, Axis.FOLLOWING);
		  break;
		case OpCodes.FROM_FOLLOWING_SIBLINGS :
		  prevIsOneStepDown = false;
		  ai = new AxesWalker(lpi, Axis.FOLLOWINGSIBLING);
		  break;
		case OpCodes.FROM_PRECEDING :
		  prevIsOneStepDown = false;
		  ai = new ReverseAxesWalker(lpi, Axis.PRECEDING);
		  break;
		case OpCodes.FROM_PRECEDING_SIBLINGS :
		  prevIsOneStepDown = false;
		  ai = new ReverseAxesWalker(lpi, Axis.PRECEDINGSIBLING);
		  break;
		case OpCodes.FROM_PARENT :
		  prevIsOneStepDown = false;
		  ai = new ReverseAxesWalker(lpi, Axis.PARENT);
		  break;
		case OpCodes.FROM_SELF :
		  ai = new AxesWalker(lpi, Axis.SELF);
		  break;
		default :
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, new object[]{Convert.ToString(stepType)})); //"Programmer's assertion: unknown opcode: "
									 //+ stepType);
		}

		if (simpleInit)
		{
		  ai.initNodeTest(DTMFilter.SHOW_ALL);
		}
		else
		{
		  int whatToShow = compiler.getWhatToShow(opPos);

		  /*
		  System.out.print("construct: ");
		  NodeTest.debugWhatToShow(whatToShow);
		  System.out.println("or stuff: "+(whatToShow & (DTMFilter.SHOW_ATTRIBUTE
		                         | DTMFilter.SHOW_ELEMENT
		                         | DTMFilter.SHOW_PROCESSING_INSTRUCTION)));
		  */
		  if ((0 == (whatToShow & (DTMFilter.SHOW_ATTRIBUTE | DTMFilter.SHOW_NAMESPACE | DTMFilter.SHOW_ELEMENT | DTMFilter.SHOW_PROCESSING_INSTRUCTION))) || (whatToShow == DTMFilter.SHOW_ALL))
		  {
			ai.initNodeTest(whatToShow);
		  }
		  else
		  {
			ai.initNodeTest(whatToShow, compiler.getStepNS(opPos), compiler.getStepLocalName(opPos));
		  }
		}

		return ai;
	  }

	  public static string getAnalysisString(int analysis)
	  {
		StringBuilder buf = new StringBuilder();
		buf.Append("count: " + getStepCount(analysis) + " ");
		if ((analysis & BIT_NODETEST_ANY) != 0)
		{
		  buf.Append("NTANY|");
		}
		if ((analysis & BIT_PREDICATE) != 0)
		{
		  buf.Append("PRED|");
		}
		if ((analysis & BIT_ANCESTOR) != 0)
		{
		  buf.Append("ANC|");
		}
		if ((analysis & BIT_ANCESTOR_OR_SELF) != 0)
		{
		  buf.Append("ANCOS|");
		}
		if ((analysis & BIT_ATTRIBUTE) != 0)
		{
		  buf.Append("ATTR|");
		}
		if ((analysis & BIT_CHILD) != 0)
		{
		  buf.Append("CH|");
		}
		if ((analysis & BIT_DESCENDANT) != 0)
		{
		  buf.Append("DESC|");
		}
		if ((analysis & BIT_DESCENDANT_OR_SELF) != 0)
		{
		  buf.Append("DESCOS|");
		}
		if ((analysis & BIT_FOLLOWING) != 0)
		{
		  buf.Append("FOL|");
		}
		if ((analysis & BIT_FOLLOWING_SIBLING) != 0)
		{
		  buf.Append("FOLS|");
		}
		if ((analysis & BIT_NAMESPACE) != 0)
		{
		  buf.Append("NS|");
		}
		if ((analysis & BIT_PARENT) != 0)
		{
		  buf.Append("P|");
		}
		if ((analysis & BIT_PRECEDING) != 0)
		{
		  buf.Append("PREC|");
		}
		if ((analysis & BIT_PRECEDING_SIBLING) != 0)
		{
		  buf.Append("PRECS|");
		}
		if ((analysis & BIT_SELF) != 0)
		{
		  buf.Append(".|");
		}
		if ((analysis & BIT_FILTER) != 0)
		{
		  buf.Append("FLT|");
		}
		if ((analysis & BIT_ROOT) != 0)
		{
		  buf.Append("R|");
		}
		return buf.ToString();
	  }

	  /// <summary>
	  /// Set to true for diagnostics about walker creation </summary>
	  internal const bool DEBUG_PATTERN_CREATION = false;

	  /// <summary>
	  /// Set to true for diagnostics about walker creation </summary>
	  internal const bool DEBUG_WALKER_CREATION = false;

	  /// <summary>
	  /// Set to true for diagnostics about iterator creation </summary>
	  internal const bool DEBUG_ITERATOR_CREATION = false;

	  public static bool hasPredicate(int analysis)
	  {
		return (0 != (analysis & BIT_PREDICATE));
	  }

	  public static bool isWild(int analysis)
	  {
		return (0 != (analysis & BIT_NODETEST_ANY));
	  }

	  public static bool walksAncestors(int analysis)
	  {
		return isSet(analysis, BIT_ANCESTOR | BIT_ANCESTOR_OR_SELF);
	  }

	  public static bool walksAttributes(int analysis)
	  {
		return (0 != (analysis & BIT_ATTRIBUTE));
	  }

	  public static bool walksNamespaces(int analysis)
	  {
		return (0 != (analysis & BIT_NAMESPACE));
	  }

	  public static bool walksChildren(int analysis)
	  {
		return (0 != (analysis & BIT_CHILD));
	  }

	  public static bool walksDescendants(int analysis)
	  {
		return isSet(analysis, BIT_DESCENDANT | BIT_DESCENDANT_OR_SELF);
	  }

	  public static bool walksSubtree(int analysis)
	  {
		return isSet(analysis, BIT_DESCENDANT | BIT_DESCENDANT_OR_SELF | BIT_CHILD);
	  }

	  public static bool walksSubtreeOnlyMaybeAbsolute(int analysis)
	  {
		return walksSubtree(analysis) && !walksExtraNodes(analysis) && !walksUp(analysis) && !walksSideways(analysis);
	  }

	  public static bool walksSubtreeOnly(int analysis)
	  {
		return walksSubtreeOnlyMaybeAbsolute(analysis) && !isAbsolute(analysis);
	  }

	  public static bool walksFilteredList(int analysis)
	  {
		return isSet(analysis, BIT_FILTER);
	  }

	  public static bool walksSubtreeOnlyFromRootOrContext(int analysis)
	  {
		return walksSubtree(analysis) && !walksExtraNodes(analysis) && !walksUp(analysis) && !walksSideways(analysis) && !isSet(analysis, BIT_FILTER);
	  }

	  public static bool walksInDocOrder(int analysis)
	  {
		return (walksSubtreeOnlyMaybeAbsolute(analysis) || walksExtraNodesOnly(analysis) || walksFollowingOnlyMaybeAbsolute(analysis)) && !isSet(analysis, BIT_FILTER);
	  }

	  public static bool walksFollowingOnlyMaybeAbsolute(int analysis)
	  {
		return isSet(analysis, BIT_SELF | BIT_FOLLOWING_SIBLING | BIT_FOLLOWING) && !walksSubtree(analysis) && !walksUp(analysis) && !walksSideways(analysis);
	  }

	  public static bool walksUp(int analysis)
	  {
		return isSet(analysis, BIT_PARENT | BIT_ANCESTOR | BIT_ANCESTOR_OR_SELF);
	  }

	  public static bool walksSideways(int analysis)
	  {
		return isSet(analysis, BIT_FOLLOWING | BIT_FOLLOWING_SIBLING | BIT_PRECEDING | BIT_PRECEDING_SIBLING);
	  }

	  public static bool walksExtraNodes(int analysis)
	  {
		return isSet(analysis, BIT_NAMESPACE | BIT_ATTRIBUTE);
	  }

	  public static bool walksExtraNodesOnly(int analysis)
	  {
		return walksExtraNodes(analysis) && !isSet(analysis, BIT_SELF) && !walksSubtree(analysis) && !walksUp(analysis) && !walksSideways(analysis) && !isAbsolute(analysis);
	  }

	  public static bool isAbsolute(int analysis)
	  {
		return isSet(analysis, BIT_ROOT | BIT_FILTER);
	  }

	  public static bool walksChildrenOnly(int analysis)
	  {
		return walksChildren(analysis) && !isSet(analysis, BIT_SELF) && !walksExtraNodes(analysis) && !walksDescendants(analysis) && !walksUp(analysis) && !walksSideways(analysis) && (!isAbsolute(analysis) || isSet(analysis, BIT_ROOT));
	  }

	  public static bool walksChildrenAndExtraAndSelfOnly(int analysis)
	  {
		return walksChildren(analysis) && !walksDescendants(analysis) && !walksUp(analysis) && !walksSideways(analysis) && (!isAbsolute(analysis) || isSet(analysis, BIT_ROOT));
	  }

	  public static bool walksDescendantsAndExtraAndSelfOnly(int analysis)
	  {
		return !walksChildren(analysis) && walksDescendants(analysis) && !walksUp(analysis) && !walksSideways(analysis) && (!isAbsolute(analysis) || isSet(analysis, BIT_ROOT));
	  }

	  public static bool walksSelfOnly(int analysis)
	  {
		return isSet(analysis, BIT_SELF) && !walksSubtree(analysis) && !walksUp(analysis) && !walksSideways(analysis) && !isAbsolute(analysis);
	  }


	  public static bool walksUpOnly(int analysis)
	  {
		return !walksSubtree(analysis) && walksUp(analysis) && !walksSideways(analysis) && !isAbsolute(analysis);
	  }

	  public static bool walksDownOnly(int analysis)
	  {
		return walksSubtree(analysis) && !walksUp(analysis) && !walksSideways(analysis) && !isAbsolute(analysis);
	  }

	  public static bool walksDownExtraOnly(int analysis)
	  {
		return walksSubtree(analysis) && walksExtraNodes(analysis) && !walksUp(analysis) && !walksSideways(analysis) && !isAbsolute(analysis);
	  }

	  public static bool canSkipSubtrees(int analysis)
	  {
		return isSet(analysis, BIT_CHILD) | walksSideways(analysis);
	  }

	  public static bool canCrissCross(int analysis)
	  {
		// This could be done faster.  Coded for clarity.
		if (walksSelfOnly(analysis))
		{
		  return false;
		}
		else if (walksDownOnly(analysis) && !canSkipSubtrees(analysis))
		{
		  return false;
		}
		else if (walksChildrenAndExtraAndSelfOnly(analysis))
		{
		  return false;
		}
		else if (walksDescendantsAndExtraAndSelfOnly(analysis))
		{
		  return false;
		}
		else if (walksUpOnly(analysis))
		{
		  return false;
		}
		else if (walksExtraNodesOnly(analysis))
		{
		  return false;
		}
		else if (walksSubtree(analysis) && (walksSideways(analysis) || walksUp(analysis) || canSkipSubtrees(analysis)))
		{
		  return true;
		}
		else
		{
		  return false;
		}
	  }

	  /// <summary>
	  /// Tell if the pattern can be 'walked' with the iteration steps in natural 
	  /// document order, without duplicates.
	  /// </summary>
	  /// <param name="analysis"> The general analysis of the pattern.
	  /// </param>
	  /// <returns> true if the walk can be done in natural order.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public static bool isNaturalDocOrder(int analysis)
	  {
		if (canCrissCross(analysis) || isSet(analysis, BIT_NAMESPACE) || walksFilteredList(analysis))
		{
		  return false;
		}

		if (walksInDocOrder(analysis))
		{
		  return true;
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if the pattern can be 'walked' with the iteration steps in natural 
	  /// document order, without duplicates.
	  /// </summary>
	  /// <param name="compiler"> non-null reference to compiler object that has processed
	  ///                 the XPath operations into an opcode map. </param>
	  /// <param name="stepOpCodePos"> The opcode position for the step. </param>
	  /// <param name="stepIndex"> The top-level step index withing the iterator. </param>
	  /// <param name="analysis"> The general analysis of the pattern.
	  /// </param>
	  /// <returns> true if the walk can be done in natural order.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static boolean isNaturalDocOrder(org.apache.xpath.compiler.Compiler compiler, int stepOpCodePos, int stepIndex, int analysis) throws javax.xml.transform.TransformerException
	  private static bool isNaturalDocOrder(Compiler compiler, int stepOpCodePos, int stepIndex, int analysis)
	  {
		if (canCrissCross(analysis))
		{
		  return false;
		}

		// Namespaces can present some problems, so just punt if we're looking for 
		// these.
		if (isSet(analysis, BIT_NAMESPACE))
		{
		  return false;
		}

		// The following, preceding, following-sibling, and preceding sibling can 
		// be found in doc order if we get to this point, but if they occur 
		// together, they produce 
		// duplicates, so it's better for us to eliminate this case so we don't 
		// have to check for duplicates during runtime if we're using a 
		// WalkingIterator.
		if (isSet(analysis, BIT_FOLLOWING | BIT_FOLLOWING_SIBLING) && isSet(analysis, BIT_PRECEDING | BIT_PRECEDING_SIBLING))
		{
		  return false;
		}

		// OK, now we have to check for select="@*/axis::*" patterns, which 
		// can also cause duplicates to happen.  But select="axis*/@::*" patterns 
		// are OK, as are select="@foo/axis::*" patterns.
		// Unfortunately, we can't do this just via the analysis bits.

		int stepType;
		int stepCount = 0;
		bool foundWildAttribute = false;

		// Steps that can traverse anything other than down a 
		// subtree or that can produce duplicates when used in 
		// combonation are counted with this variable.
		int potentialDuplicateMakingStepCount = 0;

		while (OpCodes.ENDOP != (stepType = compiler.getOp(stepOpCodePos)))
		{
		  stepCount++;

		  switch (stepType)
		  {
		  case OpCodes.FROM_ATTRIBUTES :
		  case OpCodes.MATCH_ATTRIBUTE :
			if (foundWildAttribute) // Maybe not needed, but be safe.
			{
			  return false;
			}

			// This doesn't seem to work as a test for wild card.  Hmph.
			// int nodeTestType = compiler.getStepTestType(stepOpCodePos);  

			string localName = compiler.getStepLocalName(stepOpCodePos);
			// System.err.println("localName: "+localName);
			if (localName.Equals("*"))
			{
			  foundWildAttribute = true;
			}
			break;
		  case OpCodes.FROM_FOLLOWING :
		  case OpCodes.FROM_FOLLOWING_SIBLINGS :
		  case OpCodes.FROM_PRECEDING :
		  case OpCodes.FROM_PRECEDING_SIBLINGS :
		  case OpCodes.FROM_PARENT :
		  case OpCodes.OP_VARIABLE :
		  case OpCodes.OP_EXTFUNCTION :
		  case OpCodes.OP_FUNCTION :
		  case OpCodes.OP_GROUP :
		  case OpCodes.FROM_NAMESPACE :
		  case OpCodes.FROM_ANCESTORS :
		  case OpCodes.FROM_ANCESTORS_OR_SELF :
		  case OpCodes.MATCH_ANY_ANCESTOR :
		  case OpCodes.MATCH_IMMEDIATE_ANCESTOR :
		  case OpCodes.FROM_DESCENDANTS_OR_SELF :
		  case OpCodes.FROM_DESCENDANTS :
			if (potentialDuplicateMakingStepCount > 0)
			{
				return false;
			}
			potentialDuplicateMakingStepCount++;
			  goto case org.apache.xpath.compiler.OpCodes.FROM_ROOT;
		  case OpCodes.FROM_ROOT :
		  case OpCodes.FROM_CHILDREN :
		  case OpCodes.FROM_SELF :
			if (foundWildAttribute)
			{
			  return false;
			}
			break;
		  default :
			throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_NULL_ERROR_HANDLER, new object[]{Convert.ToString(stepType)})); //"Programmer's assertion: unknown opcode: "
									  // + stepType);
		  }

		  int nextStepOpCodePos = compiler.getNextStepPos(stepOpCodePos);

		  if (nextStepOpCodePos < 0)
		  {
			break;
		  }

		  stepOpCodePos = nextStepOpCodePos;
		}

		return true;
	  }

	  public static bool isOneStep(int analysis)
	  {
		return (analysis & BITS_COUNT) == 0x00000001;
	  }

	  public static int getStepCount(int analysis)
	  {
		return (analysis & BITS_COUNT);
	  }

	  /// <summary>
	  /// First 8 bits are the number of top-level location steps.  Hopefully
	  ///  there will never be more that 255 location steps!!!
	  /// </summary>
	  public const int BITS_COUNT = 0x000000FF;

	  /// <summary>
	  /// 4 bits are reserved for future use. </summary>
	  public const int BITS_RESERVED = 0x00000F00;

	  /// <summary>
	  /// Bit is on if the expression contains a top-level predicate. </summary>
	  public static readonly int BIT_PREDICATE = (0x00001000);

	  /// <summary>
	  /// Bit is on if any of the walkers contain an ancestor step. </summary>
	  public static readonly int BIT_ANCESTOR = (0x00001000 << 1);

	  /// <summary>
	  /// Bit is on if any of the walkers contain an ancestor-or-self step. </summary>
	  public static readonly int BIT_ANCESTOR_OR_SELF = (0x00001000 << 2);

	  /// <summary>
	  /// Bit is on if any of the walkers contain an attribute step. </summary>
	  public static readonly int BIT_ATTRIBUTE = (0x00001000 << 3);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a child step. </summary>
	  public static readonly int BIT_CHILD = (0x00001000 << 4);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a descendant step. </summary>
	  public static readonly int BIT_DESCENDANT = (0x00001000 << 5);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a descendant-or-self step. </summary>
	  public static readonly int BIT_DESCENDANT_OR_SELF = (0x00001000 << 6);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a following step. </summary>
	  public static readonly int BIT_FOLLOWING = (0x00001000 << 7);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a following-sibiling step. </summary>
	  public static readonly int BIT_FOLLOWING_SIBLING = (0x00001000 << 8);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a namespace step. </summary>
	  public static readonly int BIT_NAMESPACE = (0x00001000 << 9);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a parent step. </summary>
	  public static readonly int BIT_PARENT = (0x00001000 << 10);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a preceding step. </summary>
	  public static readonly int BIT_PRECEDING = (0x00001000 << 11);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a preceding-sibling step. </summary>
	  public static readonly int BIT_PRECEDING_SIBLING = (0x00001000 << 12);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a self step. </summary>
	  public static readonly int BIT_SELF = (0x00001000 << 13);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a filter (i.e. id(), extension
	  ///  function, etc.) step.
	  /// </summary>
	  public static readonly int BIT_FILTER = (0x00001000 << 14);

	  /// <summary>
	  /// Bit is on if any of the walkers contain a root step. </summary>
	  public static readonly int BIT_ROOT = (0x00001000 << 15);

	  /// <summary>
	  /// If any of these bits are on, the expression may likely traverse outside
	  ///  the given subtree.
	  /// </summary>
	  public static readonly int BITMASK_TRAVERSES_OUTSIDE_SUBTREE = (BIT_NAMESPACE | BIT_PRECEDING_SIBLING | BIT_PRECEDING | BIT_FOLLOWING_SIBLING | BIT_FOLLOWING | BIT_PARENT | BIT_ANCESTOR_OR_SELF | BIT_ANCESTOR | BIT_FILTER | BIT_ROOT);

	  /// <summary>
	  /// Bit is on if any of the walkers can go backwards in document
	  ///  order from the context node.
	  /// </summary>
	  public static readonly int BIT_BACKWARDS_SELF = (0x00001000 << 16);

	  /// <summary>
	  /// Found "//foo" pattern </summary>
	  public static readonly int BIT_ANY_DESCENDANT_FROM_ROOT = (0x00001000 << 17);

	  /// <summary>
	  /// Bit is on if any of the walkers contain an node() test.  This is
	  ///  really only useful if the count is 1.
	  /// </summary>
	  public static readonly int BIT_NODETEST_ANY = (0x00001000 << 18);

	  // can't go higher than 18!

	  /// <summary>
	  /// Bit is on if the expression is a match pattern. </summary>
	  public static readonly int BIT_MATCH_PATTERN = (0x00001000 << 19);
	}

}