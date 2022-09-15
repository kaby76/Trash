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
 * $Id: OpMap.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.compiler
{
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using ObjectVector = org.apache.xml.utils.ObjectVector;
	using NodeTest = org.apache.xpath.patterns.NodeTest;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// This class represents the data structure basics of the XPath
	/// object.
	/// </summary>
	public class OpMap
	{

	  /// <summary>
	  /// The current pattern string, for diagnostics purposes
	  /// </summary>
	  protected internal string m_currentPattern;

	  /// <summary>
	  /// Return the expression as a string for diagnostics.
	  /// </summary>
	  /// <returns> The expression string. </returns>
	  public override string ToString()
	  {
		return m_currentPattern;
	  }

	  /// <summary>
	  /// Return the expression as a string for diagnostics.
	  /// </summary>
	  /// <returns> The expression string. </returns>
	  public virtual string PatternString
	  {
		  get
		  {
			return m_currentPattern;
		  }
	  }

	  /// <summary>
	  /// The starting size of the token queue.
	  /// </summary>
	  internal const int MAXTOKENQUEUESIZE = 500;

	  /*
	   * Amount to grow token queue when it becomes full
	   */
	  internal const int BLOCKTOKENQUEUESIZE = 500;

	  /// <summary>
	  ///  TokenStack is the queue of used tokens. The current token is the token at the
	  /// end of the m_tokenQueue. The idea is that the queue can be marked and a sequence
	  /// of tokens can be reused.
	  /// </summary>
	  internal ObjectVector m_tokenQueue = new ObjectVector(MAXTOKENQUEUESIZE, BLOCKTOKENQUEUESIZE);

	  /// <summary>
	  /// Get the XPath as a list of tokens.
	  /// </summary>
	  /// <returns> ObjectVector of tokens. </returns>
	  public virtual ObjectVector TokenQueue
	  {
		  get
		  {
			return m_tokenQueue;
		  }
	  }

	  /// <summary>
	  /// Get the XPath as a list of tokens.
	  /// </summary>
	  /// <param name="pos"> index into token queue.
	  /// </param>
	  /// <returns> The token, normally a string. </returns>
	  public virtual object getToken(int pos)
	  {
		return m_tokenQueue.elementAt(pos);
	  }

	  /// <summary>
	  /// The current size of the token queue.
	  /// </summary>
	//  public int m_tokenQueueSize = 0;

	  /// <summary>
	  /// Get size of the token queue.
	  /// </summary>
	  /// <returns> The size of the token queue. </returns>
	  public virtual int TokenQueueSize
	  {
		  get
		  {
			return m_tokenQueue.size();
    
		  }
	  }

	  /// <summary>
	  /// An operations map is used instead of a proper parse tree.  It contains
	  /// operations codes and indexes into the m_tokenQueue.
	  /// I use an array instead of a full parse tree in order to cut down
	  /// on the number of objects created.
	  /// </summary>
	  internal OpMapVector m_opMap = null;

	  /// <summary>
	  /// Get the opcode list that describes the XPath operations.  It contains
	  /// operations codes and indexes into the m_tokenQueue.
	  /// I use an array instead of a full parse tree in order to cut down
	  /// on the number of objects created.
	  /// </summary>
	  /// <returns> An IntVector that is the opcode list that describes the XPath operations. </returns>
	  public virtual OpMapVector OpMap
	  {
		  get
		  {
			return m_opMap;
		  }
	  }

	  // Position indexes

	  /// <summary>
	  /// The length is always the opcode position + 1.
	  /// Length is always expressed as the opcode+length bytes,
	  /// so it is always 2 or greater.
	  /// </summary>
	  public const int MAPINDEX_LENGTH = 1;

	  /// <summary>
	  /// Replace the large arrays
	  /// with a small array.
	  /// </summary>
	  internal virtual void shrink()
	  {

		int n = m_opMap.elementAt(MAPINDEX_LENGTH);
		m_opMap.ToSize = n + 4;

		m_opMap.setElementAt(0,n);
		m_opMap.setElementAt(0,n + 1);
		m_opMap.setElementAt(0,n + 2);


		n = m_tokenQueue.size();
		m_tokenQueue.ToSize = n + 4;

		m_tokenQueue.setElementAt(null,n);
		m_tokenQueue.setElementAt(null,n + 1);
		m_tokenQueue.setElementAt(null,n + 2);
	  }

	  /// <summary>
	  /// Given an operation position, return the current op.
	  /// </summary>
	  /// <param name="opPos"> index into op map. </param>
	  /// <returns> the op that corresponds to the opPos argument. </returns>
	  public virtual int getOp(int opPos)
	  {
		return m_opMap.elementAt(opPos);
	  }

	  /// <summary>
	  /// Set the op at index to the given int.
	  /// </summary>
	  /// <param name="opPos"> index into op map. </param>
	  /// <param name="value"> Value to set </param>
	  public virtual void setOp(int opPos, int value)
	  {
		 m_opMap.setElementAt(value,opPos);
	  }

	  /// <summary>
	  /// Given an operation position, return the end position, i.e. the
	  /// beginning of the next operation.
	  /// </summary>
	  /// <param name="opPos"> An op position of an operation for which there is a size 
	  ///              entry following. </param>
	  /// <returns> position of next operation in m_opMap. </returns>
	  public virtual int getNextOpPos(int opPos)
	  {
		return opPos + m_opMap.elementAt(opPos + 1);
	  }

	  /// <summary>
	  /// Given a location step position, return the end position, i.e. the
	  /// beginning of the next step.
	  /// </summary>
	  /// <param name="opPos"> the position of a location step. </param>
	  /// <returns> the position of the next location step. </returns>
	  public virtual int getNextStepPos(int opPos)
	  {

		int stepType = getOp(opPos);

		if ((stepType >= OpCodes.AXES_START_TYPES) && (stepType <= OpCodes.AXES_END_TYPES))
		{
		  return getNextOpPos(opPos);
		}
		else if ((stepType >= OpCodes.FIRST_NODESET_OP) && (stepType <= OpCodes.LAST_NODESET_OP))
		{
		  int newOpPos = getNextOpPos(opPos);

		  while (OpCodes.OP_PREDICATE == getOp(newOpPos))
		  {
			newOpPos = getNextOpPos(newOpPos);
		  }

		  stepType = getOp(newOpPos);

		  if (!((stepType >= OpCodes.AXES_START_TYPES) && (stepType <= OpCodes.AXES_END_TYPES)))
		  {
			return OpCodes.ENDOP;
		  }

		  return newOpPos;
		}
		else
		{
		  throw new Exception(XSLMessages.createXPATHMessage(XPATHErrorResources.ER_UNKNOWN_STEP, new object[]{stepType.ToString()}));
		  //"Programmer's assertion in getNextStepPos: unknown stepType: " + stepType);
		}
	  }

	  /// <summary>
	  /// Given an operation position, return the end position, i.e. the
	  /// beginning of the next operation.
	  /// </summary>
	  /// <param name="opMap"> The operations map. </param>
	  /// <param name="opPos"> index to operation, for which there is a size entry following. </param>
	  /// <returns> position of next operation in m_opMap. </returns>
	  public static int getNextOpPos(int[] opMap, int opPos)
	  {
		return opPos + opMap[opPos + 1];
	  }

	  /// <summary>
	  /// Given an FROM_stepType position, return the position of the
	  /// first predicate, if there is one, or else this will point
	  /// to the end of the FROM_stepType.
	  /// Example:
	  ///  int posOfPredicate = xpath.getNextOpPos(stepPos);
	  ///  boolean hasPredicates =
	  ///            OpCodes.OP_PREDICATE == xpath.getOp(posOfPredicate);
	  /// </summary>
	  /// <param name="opPos"> position of FROM_stepType op. </param>
	  /// <returns> position of predicate in FROM_stepType structure. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int getFirstPredicateOpPos(int opPos) throws javax.xml.transform.TransformerException
	  public virtual int getFirstPredicateOpPos(int opPos)
	  {

		int stepType = m_opMap.elementAt(opPos);

		if ((stepType >= OpCodes.AXES_START_TYPES) && (stepType <= OpCodes.AXES_END_TYPES))
		{
		  return opPos + m_opMap.elementAt(opPos + 2);
		}
		else if ((stepType >= OpCodes.FIRST_NODESET_OP) && (stepType <= OpCodes.LAST_NODESET_OP))
		{
		  return opPos + m_opMap.elementAt(opPos + 1);
		}
		else if (-2 == stepType)
		{
		  return -2;
		}
		else
		{
		  error(XPATHErrorResources.ER_UNKNOWN_OPCODE, new object[]{stepType.ToString()}); //"ERROR! Unknown op code: "+m_opMap[opPos]);
		  return -1;
		}
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="msg"> An error msgkey that corresponds to one of the constants found 
	  ///            in <seealso cref="org.apache.xpath.res.XPATHErrorResources"/>, which is 
	  ///            a key for a format string. </param>
	  /// <param name="args"> An array of arguments represented in the format string, which 
	  ///             may be null.
	  /// </param>
	  /// <exception cref="TransformerException"> if the current ErrorListoner determines to 
	  ///                              throw an exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void error(String msg, Object[] args) throws javax.xml.transform.TransformerException
	  public virtual void error(string msg, object[] args)
	  {

		string fmsg = XSLMessages.createXPATHMessage(msg, args);


		throw new javax.xml.transform.TransformerException(fmsg);
	  }


	  /// <summary>
	  /// Go to the first child of a given operation.
	  /// </summary>
	  /// <param name="opPos"> position of operation.
	  /// </param>
	  /// <returns> The position of the first child of the operation. </returns>
	  public static int getFirstChildPos(int opPos)
	  {
		return opPos + 2;
	  }

	  /// <summary>
	  /// Get the length of an operation.
	  /// </summary>
	  /// <param name="opPos"> The position of the operation in the op map.
	  /// </param>
	  /// <returns> The size of the operation. </returns>
	  public virtual int getArgLength(int opPos)
	  {
		return m_opMap.elementAt(opPos + MAPINDEX_LENGTH);
	  }

	  /// <summary>
	  /// Given a location step, get the length of that step.
	  /// </summary>
	  /// <param name="opPos"> Position of location step in op map.
	  /// </param>
	  /// <returns> The length of the step. </returns>
	  public virtual int getArgLengthOfStep(int opPos)
	  {
		return m_opMap.elementAt(opPos + MAPINDEX_LENGTH + 1) - 3;
	  }

	  /// <summary>
	  /// Get the first child position of a given location step.
	  /// </summary>
	  /// <param name="opPos"> Position of location step in the location map.
	  /// </param>
	  /// <returns> The first child position of the step. </returns>
	  public static int getFirstChildPosOfStep(int opPos)
	  {
		return opPos + 3;
	  }

	  /// <summary>
	  /// Get the test type of the step, i.e. NODETYPE_XXX value.
	  /// </summary>
	  /// <param name="opPosOfStep"> The position of the FROM_XXX step.
	  /// </param>
	  /// <returns> NODETYPE_XXX value. </returns>
	  public virtual int getStepTestType(int opPosOfStep)
	  {
		return m_opMap.elementAt(opPosOfStep + 3); // skip past op, len, len without predicates
	  }

	  /// <summary>
	  /// Get the namespace of the step.
	  /// </summary>
	  /// <param name="opPosOfStep"> The position of the FROM_XXX step.
	  /// </param>
	  /// <returns> The step's namespace, NodeTest.WILD, or null for null namespace. </returns>
	  public virtual string getStepNS(int opPosOfStep)
	  {

		int argLenOfStep = getArgLengthOfStep(opPosOfStep);

		// System.out.println("getStepNS.argLenOfStep: "+argLenOfStep);
		if (argLenOfStep == 3)
		{
		  int index = m_opMap.elementAt(opPosOfStep + 4);

		  if (index >= 0)
		  {
			return (string) m_tokenQueue.elementAt(index);
		  }
		  else if (OpCodes.ELEMWILDCARD == index)
		  {
			return NodeTest.WILD;
		  }
		  else
		  {
			return null;
		  }
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// Get the local name of the step. </summary>
	  /// <param name="opPosOfStep"> The position of the FROM_XXX step.
	  /// </param>
	  /// <returns> OpCodes.EMPTY, OpCodes.ELEMWILDCARD, or the local name. </returns>
	  public virtual string getStepLocalName(int opPosOfStep)
	  {

		int argLenOfStep = getArgLengthOfStep(opPosOfStep);

		// System.out.println("getStepLocalName.argLenOfStep: "+argLenOfStep);
		int index;

		switch (argLenOfStep)
		{
		case 0 :
		  index = OpCodes.EMPTY;
		  break;
		case 1 :
		  index = OpCodes.ELEMWILDCARD;
		  break;
		case 2 :
		  index = m_opMap.elementAt(opPosOfStep + 4);
		  break;
		case 3 :
		  index = m_opMap.elementAt(opPosOfStep + 5);
		  break;
		default :
		  index = OpCodes.EMPTY;
		  break; // Should assert error
		}

		// int index = (argLenOfStep == 3) ? m_opMap[opPosOfStep+5] 
		//                                  : ((argLenOfStep == 1) ? -3 : -2);
		if (index >= 0)
		{
		  return (string) m_tokenQueue.elementAt(index).ToString();
		}
		else if (OpCodes.ELEMWILDCARD == index)
		{
		  return NodeTest.WILD;
		}
		else
		{
		  return null;
		}
	  }

	}

}