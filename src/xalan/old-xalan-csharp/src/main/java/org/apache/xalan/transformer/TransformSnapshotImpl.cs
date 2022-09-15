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
 * $Id: TransformSnapshotImpl.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{


	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using BoolStack = org.apache.xml.utils.BoolStack;
	using IntStack = org.apache.xml.utils.IntStack;
	using NamespaceSupport2 = org.apache.xml.utils.NamespaceSupport2;
	using NodeVector = org.apache.xml.utils.NodeVector;
	using ObjectStack = org.apache.xml.utils.ObjectStack;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathContext = org.apache.xpath.XPathContext;

	using NamespaceSupport = org.xml.sax.helpers.NamespaceSupport;

	using NamespaceMappings = org.apache.xml.serializer.NamespaceMappings;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	/// <summary>
	/// This class holds a "snapshot" of it's current transformer state,
	/// which can later be restored.
	/// 
	/// This only saves state which can change over the course of the side-effect-free
	/// (i.e. no extensions that call setURIResolver, etc.).
	/// </summary>
	/// @deprecated  It doesn't look like this code, which is for tooling, has
	/// functioned propery for a while, so it doesn't look like it is being used. 
	internal class TransformSnapshotImpl : TransformSnapshot
	{

	  /// <summary>
	  /// The stack of Variable stack frames.
	  /// </summary>
	  private VariableStack m_variableStacks;

	  /// <summary>
	  /// The stack of <a href="http://www.w3.org/TR/xslt#dt-current-node">current node</a> objects.
	  ///  Not to be confused with the current node list.  
	  /// </summary>
	  private IntStack m_currentNodes;

	  /// <summary>
	  /// A stack of the current sub-expression nodes. </summary>
	  private IntStack m_currentExpressionNodes;

	  /// <summary>
	  /// The current context node lists stack.
	  /// </summary>
	  private Stack m_contextNodeLists;

	  /// <summary>
	  /// The current context node list.
	  /// </summary>
	  private DTMIterator m_contextNodeList;

	  /// <summary>
	  /// Stack of AxesIterators.
	  /// </summary>
	  private Stack m_axesIteratorStack;

	  /// <summary>
	  /// Is > 0 when we're processing a for-each.
	  /// </summary>
	  private BoolStack m_currentTemplateRuleIsNull;

	  /// <summary>
	  /// A node vector used as a stack to track the current
	  /// ElemTemplateElement.  Needed for the
	  /// org.apache.xalan.transformer.TransformState interface,
	  /// so a tool can discover the calling template. 
	  /// </summary>
	  private ObjectStack m_currentTemplateElements;

	  /// <summary>
	  /// A node vector used as a stack to track the current
	  /// ElemTemplate that was matched, as well as the node that
	  /// was matched.  Needed for the
	  /// org.apache.xalan.transformer.TransformState interface,
	  /// so a tool can discover the matched template, and matched
	  /// node. 
	  /// </summary>
	  private Stack m_currentMatchTemplates;

	  /// <summary>
	  /// A node vector used as a stack to track the current
	  /// ElemTemplate that was matched, as well as the node that
	  /// was matched.  Needed for the
	  /// org.apache.xalan.transformer.TransformState interface,
	  /// so a tool can discover the matched template, and matched
	  /// node. 
	  /// </summary>
	  private NodeVector m_currentMatchNodes;

	  /// <summary>
	  /// The table of counters for xsl:number support. </summary>
	  /// <seealso cref= ElemNumber </seealso>
	  private CountersTable m_countersTable;

	  /// <summary>
	  /// Stack for the purposes of flagging infinite recursion with
	  /// attribute sets.
	  /// </summary>
	  private Stack m_attrSetStack;

	  /// <summary>
	  /// Indicate whether a namespace context was pushed </summary>
	  internal bool m_nsContextPushed;

	  /// <summary>
	  /// Use the SAX2 helper class to track result namespaces.
	  /// </summary>
	  private NamespaceMappings m_nsSupport;

	  /// <summary>
	  /// The number of events queued </summary>
	//  int m_eventCount;

	  /// <summary>
	  /// Constructor TransformSnapshotImpl
	  /// Take a snapshot of the currently executing context.
	  /// </summary>
	  /// <param name="transformer"> Non null transformer instance </param>
	  /// @deprecated  It doesn't look like this code, which is for tooling, has
	  /// functioned propery for a while, so it doesn't look like it is being used. 
	  internal TransformSnapshotImpl(TransformerImpl transformer)
	  {

		try
		{

		  // Are all these clones deep enough?
		  SerializationHandler rtf = transformer.ResultTreeHandler;

		  {
			// save serializer fields
			m_nsSupport = (NamespaceMappings)rtf.NamespaceMappings.clone();

			// Do other fields need to be saved/restored?
		  }

		  XPathContext xpc = transformer.XPathContext;

		  m_variableStacks = (VariableStack) xpc.VarStack.clone();
		  m_currentNodes = (IntStack) xpc.CurrentNodeStack.clone();
		  m_currentExpressionNodes = (IntStack) xpc.CurrentExpressionNodeStack.clone();
		  m_contextNodeLists = (Stack) xpc.ContextNodeListsStack.clone();

		  if (m_contextNodeLists.Count > 0)
		  {
			m_contextNodeList = (DTMIterator) xpc.ContextNodeList.clone();
		  }

		  m_axesIteratorStack = (Stack) xpc.AxesIteratorStackStacks.clone();
		  m_currentTemplateRuleIsNull = (BoolStack) transformer.m_currentTemplateRuleIsNull.clone();
		  m_currentTemplateElements = (ObjectStack) transformer.m_currentTemplateElements.clone();
		  m_currentMatchTemplates = (Stack) transformer.m_currentMatchTemplates.clone();
		  m_currentMatchNodes = (NodeVector) transformer.m_currentMatchedNodes.clone();
		  m_countersTable = (CountersTable) transformer.CountersTable.clone();

		  if (transformer.m_attrSetStack != null)
		  {
			m_attrSetStack = (Stack) transformer.m_attrSetStack.clone();
		  }
		}
		catch (CloneNotSupportedException cnse)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(cnse);
		}
	  }

	  /// <summary>
	  /// This will reset the stylesheet to a given execution context
	  /// based on some previously taken snapshot where we can then start execution 
	  /// </summary>
	  /// <param name="transformer"> Non null transformer instance
	  /// </param>
	  /// @deprecated  It doesn't look like this code, which is for tooling, has
	  /// functioned propery for a while, so it doesn't look like it is being used. 
	  internal virtual void apply(TransformerImpl transformer)
	  {

		try
		{

		  // Are all these clones deep enough?
		  SerializationHandler rtf = transformer.ResultTreeHandler;

		  if (rtf != null)
		  {
			// restore serializer fields
			 rtf.NamespaceMappings = (NamespaceMappings)m_nsSupport.clone();
		  }

		  XPathContext xpc = transformer.XPathContext;

		  xpc.VarStack = (VariableStack) m_variableStacks.clone();
		  xpc.CurrentNodeStack = (IntStack) m_currentNodes.clone();
		  xpc.CurrentExpressionNodeStack = (IntStack) m_currentExpressionNodes.clone();
		  xpc.ContextNodeListsStack = (Stack) m_contextNodeLists.clone();

		  if (m_contextNodeList != null)
		  {
			xpc.pushContextNodeList((DTMIterator) m_contextNodeList.clone());
		  }

		  xpc.AxesIteratorStackStacks = (Stack) m_axesIteratorStack.clone();

		  transformer.m_currentTemplateRuleIsNull = (BoolStack) m_currentTemplateRuleIsNull.clone();
		  transformer.m_currentTemplateElements = (ObjectStack) m_currentTemplateElements.clone();
		  transformer.m_currentMatchTemplates = (Stack) m_currentMatchTemplates.clone();
		  transformer.m_currentMatchedNodes = (NodeVector) m_currentMatchNodes.clone();
		  transformer.m_countersTable = (CountersTable) m_countersTable.clone();

		  if (m_attrSetStack != null)
		  {
			transformer.m_attrSetStack = (Stack) m_attrSetStack.clone();
		  }
		}
		catch (CloneNotSupportedException cnse)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(cnse);
		}
	  }
	}

}