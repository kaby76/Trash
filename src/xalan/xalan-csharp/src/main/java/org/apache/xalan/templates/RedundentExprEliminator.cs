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
 * $Id: RedundentExprEliminator.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using QName = org.apache.xml.utils.QName;
	using WrappedRuntimeException = org.apache.xml.utils.WrappedRuntimeException;
	using Expression = org.apache.xpath.Expression;
	using ExpressionNode = org.apache.xpath.ExpressionNode;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPath = org.apache.xpath.XPath;
	using AxesWalker = org.apache.xpath.axes.AxesWalker;
	using FilterExprIteratorSimple = org.apache.xpath.axes.FilterExprIteratorSimple;
	using FilterExprWalker = org.apache.xpath.axes.FilterExprWalker;
	using LocPathIterator = org.apache.xpath.axes.LocPathIterator;
	using SelfIteratorNoPredicate = org.apache.xpath.axes.SelfIteratorNoPredicate;
	using WalkerFactory = org.apache.xpath.axes.WalkerFactory;
	using WalkingIterator = org.apache.xpath.axes.WalkingIterator;
	using Variable = org.apache.xpath.operations.Variable;
	using VariableSafeAbsRef = org.apache.xpath.operations.VariableSafeAbsRef;

	/// <summary>
	/// This class eleminates redundent XPaths from a given subtree, 
	/// and also collects all absolute paths within the subtree.  First 
	/// it must be called as a visitor to the subtree, and then 
	/// eleminateRedundent must be called.
	/// </summary>
	public class RedundentExprEliminator : XSLTVisitor
	{
	  internal ArrayList m_paths;
	  internal ArrayList m_absPaths;
	  internal bool m_isSameContext;
	  internal AbsPathChecker m_absPathChecker = new AbsPathChecker();

	  private static int m_uniquePseudoVarID = 1;
	  internal static readonly string PSUEDOVARNAMESPACE = Constants.S_VENDORURL + "/xalan/psuedovar";

	  public const bool DEBUG = false;
	  public const bool DIAGNOSE_NUM_PATHS_REDUCED = false;
	  public const bool DIAGNOSE_MULTISTEPLIST = false;

	  /// <summary>
	  /// So we can reuse it over and over again.
	  /// </summary>
	  internal VarNameCollector m_varNameCollector = new VarNameCollector();

	  /// <summary>
	  /// Construct a RedundentExprEliminator.
	  /// </summary>
	  public RedundentExprEliminator()
	  {
		m_isSameContext = true;
		m_absPaths = new ArrayList();
		m_paths = null;
	  }


	  /// <summary>
	  /// Method to be called after the all expressions within an  
	  /// node context have been visited.  It eliminates redundent 
	  /// expressions by creating a variable in the psuedoVarRecipient 
	  /// for each redundent expression, and then rewriting the redundent 
	  /// expression to be a variable reference.
	  /// </summary>
	  /// <param name="psuedoVarRecipient"> The recipient of the psuedo vars.  The 
	  /// variables will be inserted as first children of the element, before 
	  /// any existing variables. </param>
	  public virtual void eleminateRedundentLocals(ElemTemplateElement psuedoVarRecipient)
	  {
		eleminateRedundent(psuedoVarRecipient, m_paths);
	  }

	  /// <summary>
	  /// Method to be called after the all global expressions within a stylesheet 
	  /// have been collected.  It eliminates redundent 
	  /// expressions by creating a variable in the psuedoVarRecipient 
	  /// for each redundent expression, and then rewriting the redundent 
	  /// expression to be a variable reference.
	  /// 
	  /// </summary>
	  public virtual void eleminateRedundentGlobals(StylesheetRoot stylesheet)
	  {
		eleminateRedundent(stylesheet, m_absPaths);
	  }


	  /// <summary>
	  /// Method to be called after the all expressions within an  
	  /// node context have been visited.  It eliminates redundent 
	  /// expressions by creating a variable in the psuedoVarRecipient 
	  /// for each redundent expression, and then rewriting the redundent 
	  /// expression to be a variable reference.
	  /// </summary>
	  /// <param name="psuedoVarRecipient"> The owner of the subtree from where the 
	  ///                           paths were collected. </param>
	  /// <param name="paths"> A vector of paths that hold ExpressionOwner objects, 
	  ///              which must yield LocationPathIterators. </param>
	  protected internal virtual void eleminateRedundent(ElemTemplateElement psuedoVarRecipient, ArrayList paths)
	  {
		int n = paths.Count;
		int numPathsEliminated = 0;
		int numUniquePathsEliminated = 0;
		for (int i = 0; i < n; i++)
		{
		  ExpressionOwner owner = (ExpressionOwner) paths[i];
		  if (null != owner)
		  {
			int found = findAndEliminateRedundant(i + 1, i, owner, psuedoVarRecipient, paths);
			if (found > 0)
			{
					  numUniquePathsEliminated++;
			}
			numPathsEliminated += found;
		  }
		}

		eleminateSharedPartialPaths(psuedoVarRecipient, paths);

		if (DIAGNOSE_NUM_PATHS_REDUCED)
		{
			diagnoseNumPaths(paths, numPathsEliminated, numUniquePathsEliminated);
		}
	  }

	  /// <summary>
	  /// Eliminate the shared partial paths in the expression list.
	  /// </summary>
	  /// <param name="psuedoVarRecipient"> The recipient of the psuedo vars.
	  /// </param>
	  /// <param name="paths"> A vector of paths that hold ExpressionOwner objects, 
	  ///              which must yield LocationPathIterators. </param>
	  protected internal virtual void eleminateSharedPartialPaths(ElemTemplateElement psuedoVarRecipient, ArrayList paths)
	  {
		  MultistepExprHolder list = createMultistepExprList(paths);
		  if (null != list)
		  {
			  if (DIAGNOSE_MULTISTEPLIST)
			  {
				list.diagnose();
			  }

			bool isGlobal = (paths == m_absPaths);

			// Iterate over the list, starting with the most number of paths, 
			// trying to find the longest matches first.
			int longestStepsCount = list.m_stepCount;
			for (int i = longestStepsCount - 1; i >= 1; i--)
			{
				MultistepExprHolder next = list;
				while (null != next)
				{
					if (next.m_stepCount < i)
					{
						break;
					}
					list = matchAndEliminatePartialPaths(next, list, isGlobal, i, psuedoVarRecipient);
					next = next.m_next;
				}
			}
		  }
	  }

	  /// <summary>
	  /// For a given path, see if there are any partitial matches in the list, 
	  /// and, if there are, replace those partial paths with psuedo variable refs,
	  /// and create the psuedo variable decl.
	  /// </summary>
	  /// <returns> The head of the list, which may have changed. </returns>
	  protected internal virtual MultistepExprHolder matchAndEliminatePartialPaths(MultistepExprHolder testee, MultistepExprHolder head, bool isGlobal, int lengthToTest, ElemTemplateElement varScope)
	  {
		  if (null == testee.m_exprOwner)
		  {
			  return head;
		  }

		// Start with the longest possible match, and move down.
		WalkingIterator iter1 = (WalkingIterator) testee.m_exprOwner.Expression;
		if (partialIsVariable(testee, lengthToTest))
		{
			return head;
		}
		MultistepExprHolder matchedPaths = null;
		MultistepExprHolder matchedPathsTail = null;
		MultistepExprHolder meh = head;
		while (null != meh)
		{
		  if ((meh != testee) && (null != meh.m_exprOwner))
		  {
			  WalkingIterator iter2 = (WalkingIterator) meh.m_exprOwner.Expression;
			  if (stepsEqual(iter1, iter2, lengthToTest))
			  {
				if (null == matchedPaths)
				{
				  try
				  {
					  matchedPaths = (MultistepExprHolder)testee.clone();
					  testee.m_exprOwner = null; // So it won't be processed again.
				  }
				  catch (CloneNotSupportedException)
				  {
				  }
				  matchedPathsTail = matchedPaths;
				  matchedPathsTail.m_next = null;
				}

				try
				{
				  matchedPathsTail.m_next = (MultistepExprHolder)meh.clone();
				  meh.m_exprOwner = null; // So it won't be processed again.
				}
				catch (CloneNotSupportedException)
				{
				}
				matchedPathsTail = matchedPathsTail.m_next;
				matchedPathsTail.m_next = null;
			  }
		  }
		  meh = meh.m_next;
		}

		int matchCount = 0;
		if (null != matchedPaths)
		{
			ElemTemplateElement root = isGlobal ? varScope : findCommonAncestor(matchedPaths);
			WalkingIterator sharedIter = (WalkingIterator)matchedPaths.m_exprOwner.Expression;
			WalkingIterator newIter = createIteratorFromSteps(sharedIter, lengthToTest);
			ElemVariable var = createPseudoVarDecl(root, newIter, isGlobal);
			if (DIAGNOSE_MULTISTEPLIST)
			{
				Console.Error.WriteLine("Created var: " + var.Name + (isGlobal ? "(Global)" : ""));
			}
			while (null != matchedPaths)
			{
				ExpressionOwner owner = matchedPaths.m_exprOwner;
				WalkingIterator iter = (WalkingIterator)owner.Expression;

				if (DIAGNOSE_MULTISTEPLIST)
				{
					diagnoseLineNumber(iter);
				}

				LocPathIterator newIter2 = changePartToRef(var.Name, iter, lengthToTest, isGlobal);
				owner.Expression = newIter2;

				matchedPaths = matchedPaths.m_next;
			}
		}

		if (DIAGNOSE_MULTISTEPLIST)
		{
			diagnoseMultistepList(matchCount, lengthToTest, isGlobal);
		}
		return head;
	  }

	  /// <summary>
	  /// Check if results of partial reduction will just be a variable, in 
	  /// which case, skip it.
	  /// </summary>
	  internal virtual bool partialIsVariable(MultistepExprHolder testee, int lengthToTest)
	  {
		  if (1 == lengthToTest)
		  {
			  WalkingIterator wi = (WalkingIterator)testee.m_exprOwner.Expression;
			  if (wi.FirstWalker is FilterExprWalker)
			  {
				  return true;
			  }
		  }
		  return false;
	  }

	  /// <summary>
	  /// Tell what line number belongs to a given expression.
	  /// </summary>
	  protected internal virtual void diagnoseLineNumber(Expression expr)
	  {
		ElemTemplateElement e = getElemFromExpression(expr);
		Console.Error.WriteLine("   " + e.SystemId + " Line " + e.LineNumber);
	  }

	  /// <summary>
	  /// Given a linked list of expressions, find the common ancestor that is 
	  /// suitable for holding a psuedo variable for shared access.
	  /// </summary>
	  protected internal virtual ElemTemplateElement findCommonAncestor(MultistepExprHolder head)
	  {
		  // Not sure this algorithm is the best, but will do for the moment.
		  int numExprs = head.Length;
		  // The following could be made cheaper by pre-allocating large arrays, 
		  // but then we would have to assume a max number of reductions, 
		  // which I am not inclined to do right now.
		ElemTemplateElement[] elems = new ElemTemplateElement[numExprs];
		int[] ancestorCounts = new int[numExprs];

		// Loop through, getting the parent elements, and counting the 
		// ancestors.
		  MultistepExprHolder next = head;
		  int shortestAncestorCount = 10000;
		  for (int i = 0; i < numExprs; i++)
		  {
			  ElemTemplateElement elem = getElemFromExpression(next.m_exprOwner.Expression);
			  elems[i] = elem;
			  int numAncestors = countAncestors(elem);
			  ancestorCounts[i] = numAncestors;
			  if (numAncestors < shortestAncestorCount)
			  {
				  shortestAncestorCount = numAncestors;
			  }
			  next = next.m_next;
		  }

		  // Now loop through and "correct" the elements that have more ancestors.
		  for (int i = 0; i < numExprs; i++)
		  {
			  if (ancestorCounts[i] > shortestAncestorCount)
			  {
				  int numStepCorrection = ancestorCounts[i] - shortestAncestorCount;
				  for (int j = 0; j < numStepCorrection; j++)
				  {
					  elems[i] = elems[i].ParentElem;
				  }
			  }
		  }

		  // Now everyone has an equal number of ancestors.  Walk up from here 
		  // equally until all are equal.
		  ElemTemplateElement first = null;
		  while (shortestAncestorCount-- >= 0)
		  {
			  bool areEqual = true;
			  first = elems[0];
			  for (int i = 1; i < numExprs; i++)
			  {
				  if (first != elems[i])
				  {
					  areEqual = false;
					  break;
				  }
			  }
			  // This second check is to make sure we have a common ancestor that is not the same 
			  // as the expression owner... i.e. the var decl has to go above the expression owner.
			  if (areEqual && isNotSameAsOwner(head, first) && first.canAcceptVariables())
			  {
				  if (DIAGNOSE_MULTISTEPLIST)
				  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					  Console.Error.Write(first.GetType().FullName);
					  Console.Error.WriteLine(" at   " + first.SystemId + " Line " + first.LineNumber);
				  }
				  return first;
			  }

			  for (int i = 0; i < numExprs; i++)
			  {
				  elems[i] = elems[i].ParentElem;
			  }
		  }

		  assertion(false, "Could not find common ancestor!!!");
		  return null;
	  }

	  /// <summary>
	  /// Find out if the given ElemTemplateElement is not the same as one of 
	  /// the ElemTemplateElement owners of the expressions.
	  /// </summary>
	  /// <param name="head"> Head of linked list of expression owners. </param>
	  /// <param name="ete"> The ElemTemplateElement that is a candidate for a psuedo 
	  /// variable parent. </param>
	  /// <returns> true if the given ElemTemplateElement is not the same as one of 
	  /// the ElemTemplateElement owners of the expressions.  This is to make sure 
	  /// we find an ElemTemplateElement that is in a viable position to hold 
	  /// psuedo variables that are visible to the references. </returns>
	  protected internal virtual bool isNotSameAsOwner(MultistepExprHolder head, ElemTemplateElement ete)
	  {
		  MultistepExprHolder next = head;
		  while (null != next)
		  {
			  ElemTemplateElement elemOwner = getElemFromExpression(next.m_exprOwner.Expression);
			  if (elemOwner == ete)
			  {
				  return false;
			  }
			  next = next.m_next;
		  }
		  return true;
	  }

	  /// <summary>
	  /// Count the number of ancestors that a ElemTemplateElement has.
	  /// </summary>
	  /// <param name="elem"> An representation of an element in an XSLT stylesheet. </param>
	  /// <returns> The number of ancestors of elem (including the element itself). </returns>
	  protected internal virtual int countAncestors(ElemTemplateElement elem)
	  {
		  int count = 0;
		  while (null != elem)
		  {
			  count++;
			  elem = elem.ParentElem;
		  }
		  return count;
	  }

	  /// <summary>
	  /// Print out diagnostics about partial multistep evaluation.
	  /// </summary>
	  protected internal virtual void diagnoseMultistepList(int matchCount, int lengthToTest, bool isGlobal)
	  {
		  if (matchCount > 0)
		  {
			Console.Error.Write("Found multistep matches: " + matchCount + ", " + lengthToTest + " length");
			if (isGlobal)
			{
				  Console.Error.WriteLine(" (global)");
			}
			else
			{
				  Console.Error.WriteLine();
			}
		  }
	  }

	  /// <summary>
	  /// Change a given number of steps to a single variable reference.
	  /// </summary>
	  /// <param name="uniquePseudoVarName"> The name of the variable reference. </param>
	  /// <param name="wi"> The walking iterator that is to be changed. </param>
	  /// <param name="numSteps"> The number of steps to be changed. </param>
	  /// <param name="isGlobal"> true if this will be a global reference. </param>
	  protected internal virtual LocPathIterator changePartToRef(in QName uniquePseudoVarName, WalkingIterator wi, in int numSteps, in bool isGlobal)
	  {
		  Variable var = new Variable();
		  var.QName = uniquePseudoVarName;
		  var.IsGlobal = isGlobal;
		  if (isGlobal)
		  {
			  ElemTemplateElement elem = getElemFromExpression(wi);
			  StylesheetRoot root = elem.StylesheetRoot;
			  ArrayList vars = root.VariablesAndParamsComposed;
			  var.Index = vars.Count - 1;
		  }

		  // Walk to the first walker after the one's we are replacing.
		  AxesWalker walker = wi.FirstWalker;
		  for (int i = 0; i < numSteps; i++)
		  {
			  assertion(null != walker, "Walker should not be null!");
			  walker = walker.NextWalker;
		  }

		  if (null != walker)
		  {

			FilterExprWalker few = new FilterExprWalker(wi);
			few.InnerExpression = var;
			few.exprSetParent(wi);
			few.NextWalker = walker;
			walker.PrevWalker = few;
			wi.FirstWalker = few;
			return wi;
		  }
		  else
		  {
			FilterExprIteratorSimple feis = new FilterExprIteratorSimple(var);
			feis.exprSetParent(wi.exprGetParent());
			return feis;
		  }
	  }

	  /// <summary>
	  /// Create a new WalkingIterator from the steps in another WalkingIterator.
	  /// </summary>
	  /// <param name="wi"> The iterator from where the steps will be taken. </param>
	  /// <param name="numSteps"> The number of steps from the first to copy into the new 
	  ///                 iterator. </param>
	  /// <returns> The new iterator. </returns>
	  protected internal virtual WalkingIterator createIteratorFromSteps(in WalkingIterator wi, int numSteps)
	  {
		  WalkingIterator newIter = new WalkingIterator(wi.PrefixResolver);
		  try
		  {
			  AxesWalker walker = (AxesWalker)wi.FirstWalker.clone();
			  newIter.FirstWalker = walker;
			  walker.LocPathIterator = newIter;
			  for (int i = 1; i < numSteps; i++)
			  {
				  AxesWalker next = (AxesWalker)walker.NextWalker.clone();
				  walker.NextWalker = next;
				  next.LocPathIterator = newIter;
				  walker = next;
			  }
			  walker.NextWalker = null;
		  }
		  catch (CloneNotSupportedException cnse)
		  {
			  throw new WrappedRuntimeException(cnse);
		  }
		  return newIter;
	  }

	  /// <summary>
	  /// Compare a given number of steps between two iterators, to see if they are equal.
	  /// </summary>
	  /// <param name="iter1"> The first iterator to compare. </param>
	  /// <param name="iter2"> The second iterator to compare. </param>
	  /// <param name="numSteps"> The number of steps to compare. </param>
	  /// <returns> true If the given number of steps are equal.
	  ///  </returns>
	  protected internal virtual bool stepsEqual(WalkingIterator iter1, WalkingIterator iter2, int numSteps)
	  {
		  AxesWalker aw1 = iter1.FirstWalker;
		  AxesWalker aw2 = iter2.FirstWalker;

		  for (int i = 0; (i < numSteps); i++)
		  {
			  if ((null == aw1) || (null == aw2))
			  {
				   return false;
			  }

			  if (!aw1.deepEquals(aw2))
			  {
				  return false;
			  }

			  aw1 = aw1.NextWalker;
			  aw2 = aw2.NextWalker;
		  }

		  assertion((null != aw1) || (null != aw2), "Total match is incorrect!");

		  return true;
	  }

	  /// <summary>
	  /// For the reduction of location path parts, create a list of all 
	  /// the multistep paths with more than one step, sorted by the 
	  /// number of steps, with the most steps occuring earlier in the list.
	  /// If the list is only one member, don't bother returning it.
	  /// </summary>
	  /// <param name="paths"> Vector of ExpressionOwner objects, which may contain null entries. 
	  ///              The ExpressionOwner objects must own LocPathIterator objects. </param>
	  /// <returns> null if no multipart paths are found or the list is only of length 1, 
	  /// otherwise the first MultistepExprHolder in a linked list of these objects. </returns>
	  protected internal virtual MultistepExprHolder createMultistepExprList(ArrayList paths)
	  {
		  MultistepExprHolder first = null;
		  int n = paths.Count;
		  for (int i = 0; i < n; i++)
		  {
			  ExpressionOwner eo = (ExpressionOwner)paths[i];
			  if (null == eo)
			  {
				  continue;
			  }

			  // Assuming location path iterators should be OK.
			  LocPathIterator lpi = (LocPathIterator)eo.Expression;
			  int numPaths = countSteps(lpi);
			  if (numPaths > 1)
			  {
				  if (null == first)
				  {
					  first = new MultistepExprHolder(this, eo, numPaths, null);
				  }
				  else
				  {
					  first = first.addInSortedOrder(eo, numPaths);
				  }
			  }
		  }

		  if ((null == first) || (first.Length <= 1))
		  {
			  return null;
		  }
		  else
		  {
			  return first;
		  }
	  }

	  /// <summary>
	  /// Look through the vector from start point, looking for redundant occurances.
	  /// When one or more are found, create a psuedo variable declaration, insert 
	  /// it into the stylesheet, and replace the occurance with a reference to 
	  /// the psuedo variable.  When a redundent variable is found, it's slot in 
	  /// the vector will be replaced by null.
	  /// </summary>
	  /// <param name="start"> The position to start looking in the vector. </param>
	  /// <param name="firstOccuranceIndex"> The position of firstOccuranceOwner. </param>
	  /// <param name="firstOccuranceOwner"> The owner of the expression we are looking for. </param>
	  /// <param name="psuedoVarRecipient"> Where to put the psuedo variables.
	  /// </param>
	  /// <returns> The number of expression occurances that were modified. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int findAndEliminateRedundant(int start, int firstOccuranceIndex, org.apache.xpath.ExpressionOwner firstOccuranceOwner, ElemTemplateElement psuedoVarRecipient, java.util.Vector paths) throws org.w3c.dom.DOMException
	  protected internal virtual int findAndEliminateRedundant(int start, int firstOccuranceIndex, ExpressionOwner firstOccuranceOwner, ElemTemplateElement psuedoVarRecipient, ArrayList paths)
	  {
		MultistepExprHolder head = null;
		MultistepExprHolder tail = null;
		int numPathsFound = 0;
		int n = paths.Count;

		Expression expr1 = firstOccuranceOwner.Expression;
		if (DEBUG)
		{
			assertIsLocPathIterator(expr1, firstOccuranceOwner);
		}
		bool isGlobal = (paths == m_absPaths);
		LocPathIterator lpi = (LocPathIterator)expr1;
		int stepCount = countSteps(lpi);
		for (int j = start; j < n; j++)
		{
			ExpressionOwner owner2 = (ExpressionOwner)paths[j];
			if (null != owner2)
			{
				Expression expr2 = owner2.Expression;
				bool isEqual = expr2.deepEquals(lpi);
				if (isEqual)
				{
					LocPathIterator lpi2 = (LocPathIterator)expr2;
					if (null == head)
					{
						head = new MultistepExprHolder(this, firstOccuranceOwner, stepCount, null);
						tail = head;
						numPathsFound++;
					}
					tail.m_next = new MultistepExprHolder(this, owner2, stepCount, null);
					tail = tail.m_next;

					// Null out the occurance, so we don't have to test it again.
					paths[j] = null;

					// foundFirst = true;
					numPathsFound++;
				}
			}
		}

		// Change all globals in xsl:templates, etc, to global vars no matter what.
		if ((0 == numPathsFound) && isGlobal)
		{
		  head = new MultistepExprHolder(this, firstOccuranceOwner, stepCount, null);
		  numPathsFound++;
		}

		if (null != head)
		{
			ElemTemplateElement root = isGlobal ? psuedoVarRecipient : findCommonAncestor(head);
			LocPathIterator sharedIter = (LocPathIterator)head.m_exprOwner.Expression;
			ElemVariable var = createPseudoVarDecl(root, sharedIter, isGlobal);
			if (DIAGNOSE_MULTISTEPLIST)
			{
				Console.Error.WriteLine("Created var: " + var.Name + (isGlobal ? "(Global)" : ""));
			}
			QName uniquePseudoVarName = var.Name;
			while (null != head)
			{
				ExpressionOwner owner = head.m_exprOwner;
				if (DIAGNOSE_MULTISTEPLIST)
				{
					diagnoseLineNumber(owner.Expression);
				}
				changeToVarRef(uniquePseudoVarName, owner, paths, root);
				head = head.m_next;
			}
			// Replace the first occurance with the variable's XPath, so  
			// that further reduction may take place if needed.
			paths[firstOccuranceIndex] = var.Select;
		}

		return numPathsFound;
	  }

	  /// <summary>
	  /// To be removed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected int oldFindAndEliminateRedundant(int start, int firstOccuranceIndex, org.apache.xpath.ExpressionOwner firstOccuranceOwner, ElemTemplateElement psuedoVarRecipient, java.util.Vector paths) throws org.w3c.dom.DOMException
	  protected internal virtual int oldFindAndEliminateRedundant(int start, int firstOccuranceIndex, ExpressionOwner firstOccuranceOwner, ElemTemplateElement psuedoVarRecipient, ArrayList paths)
	  {
		QName uniquePseudoVarName = null;
		bool foundFirst = false;
		int numPathsFound = 0;
		int n = paths.Count;
		Expression expr1 = firstOccuranceOwner.Expression;
		if (DEBUG)
		{
			assertIsLocPathIterator(expr1, firstOccuranceOwner);
		}
		bool isGlobal = (paths == m_absPaths);
		LocPathIterator lpi = (LocPathIterator)expr1;
		for (int j = start; j < n; j++)
		{
			ExpressionOwner owner2 = (ExpressionOwner)paths[j];
			if (null != owner2)
			{
				Expression expr2 = owner2.Expression;
				bool isEqual = expr2.deepEquals(lpi);
				if (isEqual)
				{
					LocPathIterator lpi2 = (LocPathIterator)expr2;
					if (!foundFirst)
					{
						foundFirst = true;
						// Insert variable decl into psuedoVarRecipient
						// We want to insert this into the first legitimate 
						// position for a variable.
						ElemVariable var = createPseudoVarDecl(psuedoVarRecipient, lpi, isGlobal);
						if (null == var)
						{
							return 0;
						}
						uniquePseudoVarName = var.Name;

						changeToVarRef(uniquePseudoVarName, firstOccuranceOwner, paths, psuedoVarRecipient);

						// Replace the first occurance with the variable's XPath, so  
						// that further reduction may take place if needed.
						paths[firstOccuranceIndex] = var.Select;
						numPathsFound++;
					}

					changeToVarRef(uniquePseudoVarName, owner2, paths, psuedoVarRecipient);

					// Null out the occurance, so we don't have to test it again.
					paths[j] = null;

					// foundFirst = true;
					numPathsFound++;
				}
			}
		}

		// Change all globals in xsl:templates, etc, to global vars no matter what.
		if ((0 == numPathsFound) && (paths == m_absPaths))
		{
		  ElemVariable var = createPseudoVarDecl(psuedoVarRecipient, lpi, true);
		  if (null == var)
		  {
			return 0;
		  }
		  uniquePseudoVarName = var.Name;
		  changeToVarRef(uniquePseudoVarName, firstOccuranceOwner, paths, psuedoVarRecipient);
		  paths[firstOccuranceIndex] = var.Select;
		  numPathsFound++;
		}
		return numPathsFound;
	  }

	  /// <summary>
	  /// Count the steps in a given location path.
	  /// </summary>
	  /// <param name="lpi"> The location path iterator that owns the steps. </param>
	  /// <returns> The number of steps in the given location path. </returns>
	  protected internal virtual int countSteps(LocPathIterator lpi)
	  {
		  if (lpi is WalkingIterator)
		  {
			  WalkingIterator wi = (WalkingIterator)lpi;
			  AxesWalker aw = wi.FirstWalker;
			  int count = 0;
			  while (null != aw)
			  {
				  count++;
				  aw = aw.NextWalker;
			  }
			  return count;
		  }
		  else
		  {
			  return 1;
		  }
	  }

	  /// <summary>
	  /// Change the expression owned by the owner argument to a variable reference 
	  /// of the given name.
	  /// 
	  /// Warning: For global vars, this function relies on the variable declaration 
	  /// to which it refers having been added just prior to this function being called,
	  /// so that the reference index can be determined from the size of the global variables 
	  /// list minus one.
	  /// </summary>
	  /// <param name="varName"> The name of the variable which will be referenced. </param>
	  /// <param name="owner"> The owner of the expression which will be replaced by a variable ref. </param>
	  /// <param name="paths"> The paths list that the iterator came from, mainly to determine
	  ///              if this is a local or global reduction. </param>
	  /// <param name="psuedoVarRecipient"> The element within whose scope the variable is 
	  ///                           being inserted, possibly a StylesheetRoot. </param>
	  protected internal virtual void changeToVarRef(QName varName, ExpressionOwner owner, ArrayList paths, ElemTemplateElement psuedoVarRecipient)
	  {
		Variable varRef = (paths == m_absPaths) ? new VariableSafeAbsRef() : new Variable();
		varRef.QName = varName;
		if (paths == m_absPaths)
		{
			StylesheetRoot root = (StylesheetRoot)psuedoVarRecipient;
			ArrayList globalVars = root.VariablesAndParamsComposed;
			// Assume this operation is occuring just after the decl has 
			// been added.
			varRef.Index = globalVars.Count - 1;
			varRef.IsGlobal = true;
		}
		owner.Expression = varRef;
	  }

	  private static int PseudoVarID
	  {
		  get
		  {
			  lock (typeof(RedundentExprEliminator))
			  {
				  return m_uniquePseudoVarID++;
			  }
		  }
	  }

	  /// <summary>
	  /// Create a psuedo variable reference that will represent the 
	  /// shared redundent XPath, and add it to the stylesheet.
	  /// </summary>
	  /// <param name="psuedoVarRecipient"> The broadest scope of where the variable 
	  /// should be inserted, usually an xsl:template or xsl:for-each. </param>
	  /// <param name="lpi"> The LocationPathIterator that the variable should represent. </param>
	  /// <param name="isGlobal"> true if the paths are global. </param>
	  /// <returns> The new psuedo var element. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected ElemVariable createPseudoVarDecl(ElemTemplateElement psuedoVarRecipient, org.apache.xpath.axes.LocPathIterator lpi, boolean isGlobal) throws org.w3c.dom.DOMException
	  protected internal virtual ElemVariable createPseudoVarDecl(ElemTemplateElement psuedoVarRecipient, LocPathIterator lpi, bool isGlobal)
	  {
		QName uniquePseudoVarName = new QName(PSUEDOVARNAMESPACE, "#" + PseudoVarID);

		  if (isGlobal)
		  {
			return createGlobalPseudoVarDecl(uniquePseudoVarName, (StylesheetRoot)psuedoVarRecipient, lpi);
		  }
		  else
		  {
		  return createLocalPseudoVarDecl(uniquePseudoVarName, psuedoVarRecipient, lpi);
		  }
	  }

	  /// <summary>
	  /// Create a psuedo variable reference that will represent the 
	  /// shared redundent XPath, for a local reduction.
	  /// </summary>
	  /// <param name="uniquePseudoVarName"> The name of the new variable. </param>
	  /// <param name="stylesheetRoot"> The broadest scope of where the variable 
	  ///        should be inserted, which must be a StylesheetRoot element in this case. </param>
	  /// <param name="lpi"> The LocationPathIterator that the variable should represent. </param>
	  /// <returns> null if the decl was not created, otherwise the new Pseudo var  
	  ///              element. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected ElemVariable createGlobalPseudoVarDecl(org.apache.xml.utils.QName uniquePseudoVarName, StylesheetRoot stylesheetRoot, org.apache.xpath.axes.LocPathIterator lpi) throws org.w3c.dom.DOMException
	  protected internal virtual ElemVariable createGlobalPseudoVarDecl(QName uniquePseudoVarName, StylesheetRoot stylesheetRoot, LocPathIterator lpi)
	  {
		  ElemVariable psuedoVar = new ElemVariable();
		  psuedoVar.IsTopLevel = true;
		XPath xpath = new XPath(lpi);
		psuedoVar.Select = xpath;
		psuedoVar.Name = uniquePseudoVarName;

		ArrayList globalVars = stylesheetRoot.VariablesAndParamsComposed;
		psuedoVar.Index = globalVars.Count;
		globalVars.Add(psuedoVar);
		return psuedoVar;
	  }




	  /// <summary>
	  /// Create a psuedo variable reference that will represent the 
	  /// shared redundent XPath, for a local reduction.
	  /// </summary>
	  /// <param name="uniquePseudoVarName"> The name of the new variable. </param>
	  /// <param name="psuedoVarRecipient"> The broadest scope of where the variable 
	  /// should be inserted, usually an xsl:template or xsl:for-each. </param>
	  /// <param name="lpi"> The LocationPathIterator that the variable should represent. </param>
	  /// <returns> null if the decl was not created, otherwise the new Pseudo var  
	  ///              element. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected ElemVariable createLocalPseudoVarDecl(org.apache.xml.utils.QName uniquePseudoVarName, ElemTemplateElement psuedoVarRecipient, org.apache.xpath.axes.LocPathIterator lpi) throws org.w3c.dom.DOMException
	  protected internal virtual ElemVariable createLocalPseudoVarDecl(QName uniquePseudoVarName, ElemTemplateElement psuedoVarRecipient, LocPathIterator lpi)
	  {
			ElemVariable psuedoVar = new ElemVariablePsuedo();

			XPath xpath = new XPath(lpi);
			psuedoVar.Select = xpath;
			psuedoVar.Name = uniquePseudoVarName;

			ElemVariable var = addVarDeclToElem(psuedoVarRecipient, lpi, psuedoVar);

			lpi.exprSetParent(var);

			return var;
	  }

	  /// <summary>
	  /// Add the given variable to the psuedoVarRecipient.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected ElemVariable addVarDeclToElem(ElemTemplateElement psuedoVarRecipient, org.apache.xpath.axes.LocPathIterator lpi, ElemVariable psuedoVar) throws org.w3c.dom.DOMException
	  protected internal virtual ElemVariable addVarDeclToElem(ElemTemplateElement psuedoVarRecipient, LocPathIterator lpi, ElemVariable psuedoVar)
	  {
		// Create psuedo variable element
		ElemTemplateElement ete = psuedoVarRecipient.FirstChildElem;

		lpi.callVisitors(null, m_varNameCollector);

		// If the location path contains variables, we have to insert the 
		// psuedo variable after the reference. (Otherwise, we want to 
		// insert it as close as possible to the top, so we'll be sure 
		// it is in scope for any other vars.
		if (m_varNameCollector.VarCount > 0)
		{
		  ElemTemplateElement baseElem = getElemFromExpression(lpi);
		  ElemVariable varElem = getPrevVariableElem(baseElem);
		  while (null != varElem)
		  {
			if (m_varNameCollector.doesOccur(varElem.Name))
			{
			  psuedoVarRecipient = varElem.ParentElem;
			  ete = varElem.NextSiblingElem;
			  break;
			}
			varElem = getPrevVariableElem(varElem);
		  }
		}

		if ((null != ete) && (Constants.ELEMNAME_PARAMVARIABLE == ete.XSLToken))
		{
		  // Can't stick something in front of a param, so abandon! (see variable13.xsl)
		  if (isParam(lpi))
		  {
			return null;
		  }

		  while (null != ete)
		  {
			ete = ete.NextSiblingElem;
			if ((null != ete) && Constants.ELEMNAME_PARAMVARIABLE != ete.XSLToken)
			{
				break;
			}
		  }
		}
		psuedoVarRecipient.insertBefore(psuedoVar, ete);
		m_varNameCollector.reset();
		return psuedoVar;
	  }

	  /// <summary>
	  /// Tell if the expr param is contained within an xsl:param.
	  /// </summary>
	  protected internal virtual bool isParam(ExpressionNode expr)
	  {
		  while (null != expr)
		  {
			  if (expr is ElemTemplateElement)
			  {
				  break;
			  }
			  expr = expr.exprGetParent();
		  }
		  if (null != expr)
		  {
			  ElemTemplateElement ete = (ElemTemplateElement)expr;
			  while (null != ete)
			  {
				  int type = ete.XSLToken;
				  switch (type)
				  {
					  case Constants.ELEMNAME_PARAMVARIABLE:
						  return true;
					  case Constants.ELEMNAME_TEMPLATE:
					  case Constants.ELEMNAME_STYLESHEET:
						  return false;
				  }
				  ete = ete.ParentElem;
			  }
		  }
		  return false;

	  }

	  /// <summary>
	  /// Find the previous occurance of a xsl:variable.  Stop 
	  /// the search when a xsl:for-each, xsl:template, or xsl:stylesheet is 
	  /// encountered.
	  /// </summary>
	  /// <param name="elem"> Should be non-null template element. </param>
	  /// <returns> The first previous occurance of an xsl:variable or xsl:param, 
	  /// or null if none is found. </returns>
	  protected internal virtual ElemVariable getPrevVariableElem(ElemTemplateElement elem)
	  {
		  // This could be somewhat optimized.  since getPreviousSiblingElem is a  
		  // fairly expensive operation.
		  while (null != (elem = getPrevElementWithinContext(elem)))
		  {
			  int type = elem.XSLToken;

			  if ((Constants.ELEMNAME_VARIABLE == type) || (Constants.ELEMNAME_PARAMVARIABLE == type))
			  {
				  return (ElemVariable)elem;
			  }
		  }
		  return null;
	  }

	  /// <summary>
	  /// Get the previous sibling or parent of the given template, stopping at 
	  /// xsl:for-each, xsl:template, or xsl:stylesheet.
	  /// </summary>
	  /// <param name="elem"> Should be non-null template element. </param>
	  /// <returns> previous sibling or parent, or null if previous is xsl:for-each, 
	  /// xsl:template, or xsl:stylesheet. </returns>
	  protected internal virtual ElemTemplateElement getPrevElementWithinContext(ElemTemplateElement elem)
	  {
		  ElemTemplateElement prev = elem.PreviousSiblingElem;
		  if (null == prev)
		  {
			  prev = elem.ParentElem;
		  }
		  if (null != prev)
		  {
			int type = prev.XSLToken;
			if ((Constants.ELEMNAME_FOREACH == type) || (Constants.ELEMNAME_TEMPLATE == type) || (Constants.ELEMNAME_STYLESHEET == type))
			{
				prev = null;
			}
		  }
		  return prev;
	  }

	  /// <summary>
	  /// From an XPath expression component, get the ElemTemplateElement 
	  /// owner.
	  /// </summary>
	  /// <param name="expr"> Should be static expression with proper parentage. </param>
	  /// <returns> Valid ElemTemplateElement, or throw a runtime exception 
	  /// if it is not found. </returns>
	  protected internal virtual ElemTemplateElement getElemFromExpression(Expression expr)
	  {
		  ExpressionNode parent = expr.exprGetParent();
		  while (null != parent)
		  {
			  if (parent is ElemTemplateElement)
			  {
				  return (ElemTemplateElement)parent;
			  }
			  parent = parent.exprGetParent();
		  }
		  throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_ASSERT_NO_TEMPLATE_PARENT, null));
		  // "Programmer's error! expr has no ElemTemplateElement parent!");
	  }

	  /// <summary>
	  /// Tell if the given LocPathIterator is relative to an absolute path, i.e. 
	  /// in not dependent on the context.
	  /// </summary>
	  /// <returns> true if the LocPathIterator is not dependent on the context node. </returns>
	  public virtual bool isAbsolute(LocPathIterator path)
	  {
		  int analysis = path.AnalysisBits;
		bool isAbs = (WalkerFactory.isSet(analysis, WalkerFactory.BIT_ROOT) || WalkerFactory.isSet(analysis, WalkerFactory.BIT_ANY_DESCENDANT_FROM_ROOT));
		if (isAbs)
		{
			isAbs = m_absPathChecker.checkAbsolute(path);
		}
		return isAbs;
	  }


	  /// <summary>
	  /// Visit a LocationPath. </summary>
	  /// <param name="owner"> The owner of the expression, to which the expression can 
	  ///              be reset if rewriting takes place. </param>
	  /// <param name="path"> The LocationPath object. </param>
	  /// <returns> true if the sub expressions should be traversed. </returns>
	  public override bool visitLocationPath(ExpressionOwner owner, LocPathIterator path)
	  {
		  // Don't optimize "." or single step variable paths.
		  // Both of these cases could use some further optimization by themselves.
		  if (path is SelfIteratorNoPredicate)
		  {
			  return true;
		  }
		  else if (path is WalkingIterator)
		  {
			  WalkingIterator wi = (WalkingIterator)path;
			  AxesWalker aw = wi.FirstWalker;
			  if ((aw is FilterExprWalker) && (null == aw.NextWalker))
			  {
				  FilterExprWalker few = (FilterExprWalker)aw;
				  Expression exp = few.InnerExpression;
				  if (exp is Variable)
				  {
					  return true;
				  }
			  }
		  }

		if (isAbsolute(path) && (null != m_absPaths))
		{
		  if (DEBUG)
		  {
			validateNewAddition(m_absPaths, owner, path);
		  }
		  m_absPaths.Add(owner);
		}
		else if (m_isSameContext && (null != m_paths))
		{
		  if (DEBUG)
		  {
			validateNewAddition(m_paths, owner, path);
		  }
		  m_paths.Add(owner);
		}

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
	  public override bool visitPredicate(ExpressionOwner owner, Expression pred)
	  {
		bool savedIsSame = m_isSameContext;
		m_isSameContext = false;

		// Any further down, just collect the absolute paths.
		pred.callVisitors(owner, this);

		m_isSameContext = savedIsSame;

		// We've already gone down the subtree, so don't go have the caller 
		// go any further.
		return false;
	  }

	  /// <summary>
	  /// Visit an XSLT top-level instruction.
	  /// </summary>
	  /// <param name="elem"> The xsl instruction element object. </param>
	  /// <returns> true if the sub expressions should be traversed. </returns>
	   public override bool visitTopLevelInstruction(ElemTemplateElement elem)
	   {
		 int type = elem.XSLToken;
		 switch (type)
		 {
		   case Constants.ELEMNAME_TEMPLATE :
			 return visitInstruction(elem);
		   default:
			 return true;
		 }
	   }


	  /// <summary>
	  /// Visit an XSLT instruction.  Any element that isn't called by one 
	  /// of the other visit methods, will be called by this method.
	  /// </summary>
	  /// <param name="elem"> The xsl instruction element object. </param>
	  /// <returns> true if the sub expressions should be traversed. </returns>
	  public override bool visitInstruction(ElemTemplateElement elem)
	  {
		int type = elem.XSLToken;
		switch (type)
		{
		  case Constants.ELEMNAME_CALLTEMPLATE :
		  case Constants.ELEMNAME_TEMPLATE :
		  case Constants.ELEMNAME_FOREACH :
		  {

			  // Just get the select value.
			  if (type == Constants.ELEMNAME_FOREACH)
			  {
				ElemForEach efe = (ElemForEach) elem;

				  Expression select = efe.getSelect();
				  select.callVisitors(efe, this);
			  }

				ArrayList savedPaths = m_paths;
				m_paths = new ArrayList();

				// Visit children.  Call the superclass callChildVisitors, because 
				// we don't want to visit the xsl:for-each select attribute, or, for 
				// that matter, the xsl:template's match attribute.
				elem.callChildVisitors(this, false);
				eleminateRedundentLocals(elem);

				m_paths = savedPaths;

			  // select.callVisitors(efe, this);
			  return false;
		  }
		  case Constants.ELEMNAME_NUMBER :
		  case Constants.ELEMNAME_SORT :
			// Just collect absolute paths until and unless we can fully
			// analyze these cases.
			bool savedIsSame = m_isSameContext;
			m_isSameContext = false;
			elem.callChildVisitors(this);
			m_isSameContext = savedIsSame;
			return false;

		  default :
			return true;
		}
	  }

	  // ==== DIAGNOSTIC AND DEBUG FUNCTIONS ====

	  /// <summary>
	  /// Print out to std err the number of paths reduced.
	  /// </summary>
	  protected internal virtual void diagnoseNumPaths(ArrayList paths, int numPathsEliminated, int numUniquePathsEliminated)
	  {
			if (numPathsEliminated > 0)
			{
			  if (paths == m_paths)
			  {
				Console.Error.WriteLine("Eliminated " + numPathsEliminated + " total paths!");
				Console.Error.WriteLine("Consolodated " + numUniquePathsEliminated + " redundent paths!");
			  }
			  else
			  {
				Console.Error.WriteLine("Eliminated " + numPathsEliminated + " total global paths!");
				Console.Error.WriteLine("Consolodated " + numUniquePathsEliminated + " redundent global paths!");
			  }
			}
	  }


	  /// <summary>
	  /// Assert that the expression is a LocPathIterator, and, if 
	  /// not, try to give some diagnostic info.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private final void assertIsLocPathIterator(org.apache.xpath.Expression expr1, org.apache.xpath.ExpressionOwner eo) throws RuntimeException
	  private void assertIsLocPathIterator(Expression expr1, ExpressionOwner eo)
	  {
			if (!(expr1 is LocPathIterator))
			{
				string errMsg;
				if (expr1 is Variable)
				{
					errMsg = "Programmer's assertion: expr1 not an iterator: " + ((Variable)expr1).QName;
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					errMsg = "Programmer's assertion: expr1 not an iterator: " + expr1.GetType().FullName;
				}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				throw new Exception(errMsg + ", " + eo.GetType().FullName + " " + expr1.exprGetParent());
			}
	  }


	  /// <summary>
	  /// Validate some assumptions about the new LocPathIterator and it's 
	  /// owner and the state of the list.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void validateNewAddition(java.util.Vector paths, org.apache.xpath.ExpressionOwner owner, org.apache.xpath.axes.LocPathIterator path) throws RuntimeException
	  private static void validateNewAddition(ArrayList paths, ExpressionOwner owner, LocPathIterator path)
	  {
		  assertion(owner.Expression == path, "owner.getExpression() != path!!!");
		int n = paths.Count;
		// There should never be any duplicates in the list!
		for (int i = 0; i < n; i++)
		{
			ExpressionOwner ew = (ExpressionOwner)paths[i];
			assertion(ew != owner, "duplicate owner on the list!!!");
			assertion(ew.Expression != path, "duplicate expression on the list!!!");
		}
	  }

	  /// <summary>
	  /// Simple assertion.
	  /// </summary>
	  protected internal static void assertion(bool b, string msg)
	  {
		  if (!b)
		  {
			  throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_ASSERT_REDUNDENT_EXPR_ELIMINATOR, new object[]{msg}));
			  // "Programmer's assertion in RundundentExprEliminator: "+msg);
		  }
	  }

	  /// <summary>
	  /// Since we want to sort multistep expressions by length, use 
	  /// a linked list with elements of type MultistepExprHolder.
	  /// </summary>
	  internal class MultistepExprHolder : ICloneable
	  {
		  private readonly RedundentExprEliminator outerInstance;

		internal ExpressionOwner m_exprOwner; // Will change to null once we have processed this item.
		internal readonly int m_stepCount;
		internal MultistepExprHolder m_next;

		/// <summary>
		/// Clone this object.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
			return base.clone();
		}

		/// <summary>
		/// Create a MultistepExprHolder.
		/// </summary>
		/// <param name="exprOwner"> the owner of the expression we are holding.
		///                  It must hold a LocationPathIterator. </param>
		/// <param name="stepCount"> The number of steps in the location path. </param>
		  internal MultistepExprHolder(RedundentExprEliminator outerInstance, ExpressionOwner exprOwner, int stepCount, MultistepExprHolder next)
		  {
			  this.outerInstance = outerInstance;
			  m_exprOwner = exprOwner;
			  assertion(null != m_exprOwner, "exprOwner can not be null!");
			  m_stepCount = stepCount;
			  m_next = next;
		  }

		/// <summary>
		/// Add a new MultistepExprHolder in sorted order in the list.
		/// </summary>
		/// <param name="exprOwner"> the owner of the expression we are holding.
		///                  It must hold a LocationPathIterator. </param>
		/// <param name="stepCount"> The number of steps in the location path. </param>
		/// <returns> The new head of the linked list. </returns>
		internal virtual MultistepExprHolder addInSortedOrder(ExpressionOwner exprOwner, int stepCount)
		{
			MultistepExprHolder first = this;
			MultistepExprHolder next = this;
			MultistepExprHolder prev = null;
			while (null != next)
			{
				if (stepCount >= next.m_stepCount)
				{
					MultistepExprHolder newholder = new MultistepExprHolder(outerInstance, exprOwner, stepCount, next);
					if (null == prev)
					{
						first = newholder;
					}
					else
					{
						prev.m_next = newholder;
					}

					return first;
				}
				prev = next;
				next = next.m_next;
			}

			prev.m_next = new MultistepExprHolder(outerInstance, exprOwner, stepCount, null);
			return first;
		}

		/// <summary>
		/// Remove the given element from the list.  'this' should 
		/// be the head of the list.  If the item to be removed is not 
		/// found, an assertion will be made.
		/// </summary>
		/// <param name="itemToRemove"> The item to remove from the list. </param>
		/// <returns> The head of the list, which may have changed if itemToRemove 
		/// is the same as this element.  Null if the item to remove is the 
		/// only item in the list. </returns>
		internal virtual MultistepExprHolder unlink(MultistepExprHolder itemToRemove)
		{
			MultistepExprHolder first = this;
			MultistepExprHolder next = this;
			MultistepExprHolder prev = null;
			while (null != next)
			{
				if (next == itemToRemove)
				{
					if (null == prev)
					{
						first = next.m_next;
					}
					else
					{
						prev.m_next = next.m_next;
					}

					next.m_next = null;

					return first;
				}
				prev = next;
				next = next.m_next;
			}

			assertion(false, "unlink failed!!!");
			return null;
		}

		/// <summary>
		/// Get the number of linked list items.
		/// </summary>
		internal virtual int Length
		{
			get
			{
				int count = 0;
				MultistepExprHolder next = this;
				while (null != next)
				{
					count++;
					next = next.m_next;
				}
				return count;
			}
		}

		/// <summary>
		/// Print diagnostics out for the multistep list.
		/// </summary>
		protected internal virtual void diagnose()
		{
		  Console.Error.Write("Found multistep iterators: " + this.Length + "  ");
		  MultistepExprHolder next = this;
		  while (null != next)
		  {
			Console.Error.Write("" + next.m_stepCount);
			next = next.m_next;
			if (null != next)
			{
				  Console.Error.Write(", ");
			}
		  }
		  Console.Error.WriteLine();
		}

	  }

	}

}