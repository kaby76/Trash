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
 * $Id: XPathResultImpl.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */


namespace org.apache.xpath.domapi
{

	using XPath = org.apache.xpath.XPath;
	using XObject = org.apache.xpath.objects.XObject;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XPATHMessages = org.apache.xpath.res.XPATHMessages;
	using DOMException = org.w3c.dom.DOMException;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using Event = org.w3c.dom.events.Event;
	using EventListener = org.w3c.dom.events.EventListener;
	using EventTarget = org.w3c.dom.events.EventTarget;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;
	using XPathException = org.w3c.dom.xpath.XPathException;
	using XPathResult = org.w3c.dom.xpath.XPathResult;

	/// 
	/// <summary>
	/// The class provides an implementation XPathResult according 
	/// to the DOM L3 XPath Specification, Working Group Note 26 February 2004.
	/// 
	/// <para>See also the <a href='http://www.w3.org/TR/2004/NOTE-DOM-Level-3-XPath-20040226'>Document Object Model (DOM) Level 3 XPath Specification</a>.</para>
	/// 
	/// <para>The <code>XPathResult</code> interface represents the result of the 
	/// evaluation of an XPath expression within the context of a particular 
	/// node. Since evaluation of an XPath expression can result in various 
	/// result types, this object makes it possible to discover and manipulate 
	/// the type and value of the result.</para>
	/// 
	/// <para>This implementation wraps an <code>XObject</code>.
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref="org.apache.xpath.objects.XObject"/>
	/// <seealso cref="org.w3c.dom.xpath.XPathResult"
	/// 
	/// @xsl.usage internal/>
	internal class XPathResultImpl : XPathResult, EventListener
	{

		/// <summary>
		///  The wrapped XObject
		/// </summary>
		private readonly XObject m_resultObj;

		/// <summary>
		/// The xpath object that wraps the expression used for this result.
		/// </summary>
		private readonly XPath m_xpath;

		/// <summary>
		///  This the type specified by the user during construction.  Typically
		///  the constructor will be called by org.apache.xpath.XPath.evaluate().
		/// </summary>
		private readonly short m_resultType;

		private bool m_isInvalidIteratorState = false;

		/// <summary>
		/// Only used to attach a mutation event handler when specified
		/// type is an iterator type.
		/// </summary>
		private readonly Node m_contextNode;

		/// <summary>
		///  The iterator, if this is an iterator type.
		/// </summary>
		private NodeIterator m_iterator = null;

		/// <summary>
		///  The list, if this is a snapshot type.
		/// </summary>
		private NodeList m_list = null;


		/// <summary>
		/// Constructor for XPathResultImpl.
		/// 
		/// For internal use only.
		/// </summary>
		 internal XPathResultImpl(short type, XObject result, Node contextNode, XPath xpath)
		 {
			// Check that the type is valid
			if (!isValidType(type))
			{
				string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_INVALID_XPATH_TYPE, new object[] {new int?(type)});
				throw new XPathException(XPathException.TYPE_ERR,fmsg); // Invalid XPath type argument: {0}
			}

			// Result object should never be null!
			if (null == result)
			{
				string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_EMPTY_XPATH_RESULT, null);
				throw new XPathException(XPathException.INVALID_EXPRESSION_ERR,fmsg); // Empty XPath result object
			}

			this.m_resultObj = result;
			this.m_contextNode = contextNode;
			this.m_xpath = xpath;

			// If specified result was ANY_TYPE, determine XObject type
			if (type == ANY_TYPE)
			{
				this.m_resultType = getTypeFromXObject(result);
			}
			else
			{
				this.m_resultType = type;
			}

			// If the context node supports DOM Events and the type is one of the iterator
			// types register this result as an event listener
			if (((m_resultType == XPathResult.ORDERED_NODE_ITERATOR_TYPE) || (m_resultType == XPathResult.UNORDERED_NODE_ITERATOR_TYPE)))
			{
					addEventListener();

			} // else can we handle iterator types if contextNode doesn't support EventTarget??

			// If this is an iterator type get the iterator
			if ((m_resultType == ORDERED_NODE_ITERATOR_TYPE) || (m_resultType == UNORDERED_NODE_ITERATOR_TYPE) || (m_resultType == ANY_UNORDERED_NODE_TYPE) || (m_resultType == FIRST_ORDERED_NODE_TYPE))
			{

				try
				{
					m_iterator = m_resultObj.nodeset();
				}
				catch (TransformerException)
				{
					// probably not a node type
					string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_INCOMPATIBLE_TYPES, new object[] {m_xpath.PatternString, getTypeString(getTypeFromXObject(m_resultObj)), getTypeString(m_resultType)});
					  throw new XPathException(XPathException.TYPE_ERR, fmsg); // "The XPathResult of XPath expression {0} has an XPathResultType of {1} which cannot be coerced into the specified XPathResultType of {2}."},
				}

					// If user requested ordered nodeset and result is unordered 
					// need to sort...TODO
		//            if ((m_resultType == ORDERED_NODE_ITERATOR_TYPE) &&
		//                (!(((DTMNodeIterator)m_iterator).getDTMIterator().isDocOrdered()))) {
		// 
		//            }

			// If it's a snapshot type, get the nodelist
			}
			else if ((m_resultType == UNORDERED_NODE_SNAPSHOT_TYPE) || (m_resultType == ORDERED_NODE_SNAPSHOT_TYPE))
			{
				try
				{
				   m_list = m_resultObj.nodelist();
				}
				catch (TransformerException)
				{
					// probably not a node type 
					string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_INCOMPATIBLE_TYPES, new object[] {m_xpath.PatternString, getTypeString(getTypeFromXObject(m_resultObj)), getTypeString(m_resultType)});
					throw new XPathException(XPathException.TYPE_ERR, fmsg); // "The XPathResult of XPath expression {0} has an XPathResultType of {1} which cannot be coerced into the specified XPathResultType of {2}."},
				}
			}
		 }

		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getResultType()"/>
		public virtual short ResultType
		{
			get
			{
				return m_resultType;
			}
		}

		/// <summary>
		///  The value of this number result. </summary>
		/// <exception cref="XPathException">
		///   TYPE_ERR: raised if <code>resultType</code> is not 
		///   <code>NUMBER_TYPE</code>. </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getNumberValue()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double getNumberValue() throws org.w3c.dom.xpath.XPathException
		public virtual double NumberValue
		{
			get
			{
				if (ResultType != NUMBER_TYPE)
				{
					string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_CANT_CONVERT_XPATHRESULTTYPE_TO_NUMBER, new object[] {m_xpath.PatternString, getTypeString(m_resultType)});
					throw new XPathException(XPathException.TYPE_ERR,fmsg);
		//		"The XPathResult of XPath expression {0} has an XPathResultType of {1} which cannot be converted to a number"
				}
				else
				{
					try
					{
					   return m_resultObj.num();
					}
					catch (Exception e)
					{
						// Type check above should prevent this exception from occurring.
						throw new XPathException(XPathException.TYPE_ERR,e.Message);
					}
				}
			}
		}

		/// <summary>
		/// The value of this string result. </summary>
		/// <exception cref="XPathException">
		///   TYPE_ERR: raised if <code>resultType</code> is not 
		///   <code>STRING_TYPE</code>.
		/// </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getStringValue()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public String getStringValue() throws org.w3c.dom.xpath.XPathException
		public virtual string StringValue
		{
			get
			{
				if (ResultType != STRING_TYPE)
				{
					string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_CANT_CONVERT_TO_STRING, new object[] {m_xpath.PatternString, m_resultObj.TypeString});
					throw new XPathException(XPathException.TYPE_ERR,fmsg);
		//		"The XPathResult of XPath expression {0} has an XPathResultType of {1} which cannot be converted to a string."
				}
				else
				{
					try
					{
					   return m_resultObj.str();
					}
					catch (Exception e)
					{
						// Type check above should prevent this exception from occurring.
						throw new XPathException(XPathException.TYPE_ERR,e.Message);
					}
				}
			}
		}

		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getBooleanValue()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean getBooleanValue() throws org.w3c.dom.xpath.XPathException
		public virtual bool BooleanValue
		{
			get
			{
				if (ResultType != BOOLEAN_TYPE)
				{
					string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_CANT_CONVERT_TO_BOOLEAN, new object[] {m_xpath.PatternString, getTypeString(m_resultType)});
					throw new XPathException(XPathException.TYPE_ERR,fmsg);
		//		"The XPathResult of XPath expression {0} has an XPathResultType of {1} which cannot be converted to a boolean."			
				}
				else
				{
					try
					{
					   return m_resultObj.@bool();
					}
					catch (TransformerException e)
					{
						// Type check above should prevent this exception from occurring.
						throw new XPathException(XPathException.TYPE_ERR,e.Message);
					}
				}
			}
		}

		/// <summary>
		/// The value of this single node result, which may be <code>null</code>. </summary>
		/// <exception cref="XPathException">
		///   TYPE_ERR: raised if <code>resultType</code> is not 
		///   <code>ANY_UNORDERED_NODE_TYPE</code> or 
		///   <code>FIRST_ORDERED_NODE_TYPE</code>.
		/// </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getSingleNodeValue()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node getSingleNodeValue() throws org.w3c.dom.xpath.XPathException
		public virtual Node SingleNodeValue
		{
			get
			{
    
				if ((m_resultType != ANY_UNORDERED_NODE_TYPE) && (m_resultType != FIRST_ORDERED_NODE_TYPE))
				{
						string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_CANT_CONVERT_TO_SINGLENODE, new object[] {m_xpath.PatternString, getTypeString(m_resultType)});
						throw new XPathException(XPathException.TYPE_ERR,fmsg);
		//				"The XPathResult of XPath expression {0} has an XPathResultType of {1} which cannot be converted to a single node. 
		//				 This method applies only to types ANY_UNORDERED_NODE_TYPE and FIRST_ORDERED_NODE_TYPE."
				}
    
				NodeIterator result = null;
				try
				{
					result = m_resultObj.nodeset();
				}
				catch (TransformerException te)
				{
					throw new XPathException(XPathException.TYPE_ERR,te.Message);
				}
    
				if (null == result)
				{
					return null;
				}
    
				Node node = result.nextNode();
    
				// Wrap "namespace node" in an XPathNamespace 
				if (isNamespaceNode(node))
				{
					return new XPathNamespaceImpl(node);
				}
				else
				{
					return node;
				}
			}
		}

		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getInvalidIteratorState()"/>
		public virtual bool InvalidIteratorState
		{
			get
			{
				return m_isInvalidIteratorState;
			}
		}

		/// <summary>
		/// The number of nodes in the result snapshot. Valid values for 
		/// snapshotItem indices are <code>0</code> to 
		/// <code>snapshotLength-1</code> inclusive. </summary>
		/// <exception cref="XPathException">
		///   TYPE_ERR: raised if <code>resultType</code> is not 
		///   <code>UNORDERED_NODE_SNAPSHOT_TYPE</code> or 
		///   <code>ORDERED_NODE_SNAPSHOT_TYPE</code>.
		/// </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathResult.getSnapshotLength()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public int getSnapshotLength() throws org.w3c.dom.xpath.XPathException
		public virtual int SnapshotLength
		{
			get
			{
    
				if ((m_resultType != UNORDERED_NODE_SNAPSHOT_TYPE) && (m_resultType != ORDERED_NODE_SNAPSHOT_TYPE))
				{
						string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_CANT_GET_SNAPSHOT_LENGTH, new object[] {m_xpath.PatternString, getTypeString(m_resultType)});
						throw new XPathException(XPathException.TYPE_ERR,fmsg);
		//				"The method getSnapshotLength cannot be called on the XPathResult of XPath expression {0} because its XPathResultType is {1}.
				}
    
				return m_list.getLength();
			}
		}

		/// <summary>
		/// Iterates and returns the next node from the node set or 
		/// <code>null</code>if there are no more nodes. </summary>
		/// <returns> Returns the next node. </returns>
		/// <exception cref="XPathException">
		///   TYPE_ERR: raised if <code>resultType</code> is not 
		///   <code>UNORDERED_NODE_ITERATOR_TYPE</code> or 
		///   <code>ORDERED_NODE_ITERATOR_TYPE</code>. </exception>
		/// <exception cref="DOMException">
		///   INVALID_STATE_ERR: The document has been mutated since the result was 
		///   returned. </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathResult.iterateNext()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node iterateNext() throws XPathException, org.w3c.dom.DOMException
		public virtual Node iterateNext()
		{
			if ((m_resultType != UNORDERED_NODE_ITERATOR_TYPE) && (m_resultType != ORDERED_NODE_ITERATOR_TYPE))
			{
			  string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_NON_ITERATOR_TYPE, new object[] {m_xpath.PatternString, getTypeString(m_resultType)});
			  throw new XPathException(XPathException.TYPE_ERR, fmsg);
	//		  "The method iterateNext cannot be called on the XPathResult of XPath expression {0} because its XPathResultType is {1}. 
	//		  This method applies only to types UNORDERED_NODE_ITERATOR_TYPE and ORDERED_NODE_ITERATOR_TYPE."},
			}

			if (InvalidIteratorState)
			{
			  string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_DOC_MUTATED, null);
			  throw new DOMException(DOMException.INVALID_STATE_ERR,fmsg); // Document mutated since result was returned. Iterator is invalid.
			}

			Node node = m_iterator.nextNode();
			if (null == node)
			{
				removeEventListener(); // JIRA 1673
			}
			// Wrap "namespace node" in an XPathNamespace 
			if (isNamespaceNode(node))
			{
				return new XPathNamespaceImpl(node);
			}
			else
			{
				return node;
			}
		}

		/// <summary>
		/// Returns the <code>index</code>th item in the snapshot collection. If 
		/// <code>index</code> is greater than or equal to the number of nodes in 
		/// the list, this method returns <code>null</code>. Unlike the iterator 
		/// result, the snapshot does not become invalid, but may not correspond 
		/// to the current document if it is mutated. </summary>
		/// <param name="index"> Index into the snapshot collection. </param>
		/// <returns> The node at the <code>index</code>th position in the 
		///   <code>NodeList</code>, or <code>null</code> if that is not a valid 
		///   index. </returns>
		/// <exception cref="XPathException">
		///   TYPE_ERR: raised if <code>resultType</code> is not 
		///   <code>UNORDERED_NODE_SNAPSHOT_TYPE</code> or 
		///   <code>ORDERED_NODE_SNAPSHOT_TYPE</code>.
		/// </exception>
		/// <seealso cref="org.w3c.dom.xpath.XPathResult.snapshotItem(int)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.Node snapshotItem(int index) throws org.w3c.dom.xpath.XPathException
		public virtual Node snapshotItem(int index)
		{

			if ((m_resultType != UNORDERED_NODE_SNAPSHOT_TYPE) && (m_resultType != ORDERED_NODE_SNAPSHOT_TYPE))
			{
			   string fmsg = XPATHMessages.createXPATHMessage(XPATHErrorResources.ER_NON_SNAPSHOT_TYPE, new object[] {m_xpath.PatternString, getTypeString(m_resultType)});
			   throw new XPathException(XPathException.TYPE_ERR, fmsg);
	//		"The method snapshotItem cannot be called on the XPathResult of XPath expression {0} because its XPathResultType is {1}. 
	//		This method applies only to types UNORDERED_NODE_SNAPSHOT_TYPE and ORDERED_NODE_SNAPSHOT_TYPE."},
			}

			Node node = m_list.item(index);

			// Wrap "namespace node" in an XPathNamespace 
			if (isNamespaceNode(node))
			{
				return new XPathNamespaceImpl(node);
			}
			else
			{
				return node;
			}
		}


		/// <summary>
		/// Check if the specified type is one of the supported types. </summary>
		/// <param name="type"> The specified type
		/// </param>
		/// <returns> true If the specified type is supported; otherwise, returns false. </returns>
		internal static bool isValidType(short type)
		{
			switch (type)
			{
				case ANY_TYPE:
				case NUMBER_TYPE:
				case STRING_TYPE:
				case BOOLEAN_TYPE:
				case UNORDERED_NODE_ITERATOR_TYPE:
				case ORDERED_NODE_ITERATOR_TYPE:
				case UNORDERED_NODE_SNAPSHOT_TYPE:
				case ORDERED_NODE_SNAPSHOT_TYPE:
				case ANY_UNORDERED_NODE_TYPE:
				case FIRST_ORDERED_NODE_TYPE:
					return true;
				default:
					return false;
			}
		}

		/// <seealso cref="org.w3c.dom.events.EventListener.handleEvent(Event)"/>
		public virtual void handleEvent(Event @event)
		{

			if (@event.getType().Equals("DOMSubtreeModified"))
			{
				// invalidate the iterator
				m_isInvalidIteratorState = true;

				// deregister as a listener to reduce computational load
				removeEventListener();
			}
		}

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string  </returns>
	  private string getTypeString(int type)
	  {
		 switch (type)
		 {
		  case ANY_TYPE:
			  return "ANY_TYPE";
		  case ANY_UNORDERED_NODE_TYPE:
			  return "ANY_UNORDERED_NODE_TYPE";
		  case BOOLEAN_TYPE:
			  return "BOOLEAN";
		  case FIRST_ORDERED_NODE_TYPE:
			  return "FIRST_ORDERED_NODE_TYPE";
		  case NUMBER_TYPE:
			  return "NUMBER_TYPE";
		  case ORDERED_NODE_ITERATOR_TYPE:
			  return "ORDERED_NODE_ITERATOR_TYPE";
		  case ORDERED_NODE_SNAPSHOT_TYPE:
			  return "ORDERED_NODE_SNAPSHOT_TYPE";
		  case STRING_TYPE:
			  return "STRING_TYPE";
		  case UNORDERED_NODE_ITERATOR_TYPE:
			  return "UNORDERED_NODE_ITERATOR_TYPE";
		  case UNORDERED_NODE_SNAPSHOT_TYPE:
			  return "UNORDERED_NODE_SNAPSHOT_TYPE";
		  default:
			  return "#UNKNOWN";
		 }
	  }

	  /// <summary>
	  /// Given an XObject, determine the corresponding DOM XPath type
	  /// </summary>
	  /// <returns> type string </returns>
	  private short getTypeFromXObject(XObject @object)
	  {
		  switch (@object.Type)
		  {
			case XObject.CLASS_BOOLEAN:
				return BOOLEAN_TYPE;
			case XObject.CLASS_NODESET:
				return UNORDERED_NODE_ITERATOR_TYPE;
			case XObject.CLASS_NUMBER:
				return NUMBER_TYPE;
			case XObject.CLASS_STRING:
				return STRING_TYPE;
			// XPath 2.0 types                         
	//          case XObject.CLASS_DATE: 
	//          case XObject.CLASS_DATETIME:
	//          case XObject.CLASS_DTDURATION:
	//          case XObject.CLASS_GDAY:
	//          case XObject.CLASS_GMONTH:
	//          case XObject.CLASS_GMONTHDAY:
	//          case XObject.CLASS_GYEAR:
	//          case XObject.CLASS_GYEARMONTH: 
	//          case XObject.CLASS_TIME:
	//          case XObject.CLASS_YMDURATION: return STRING_TYPE; // treat all date types as strings?

			case XObject.CLASS_RTREEFRAG:
				return UNORDERED_NODE_ITERATOR_TYPE;
			case XObject.CLASS_NULL:
				return ANY_TYPE; // throw exception ?
			default:
				return ANY_TYPE; // throw exception ?
		  }

	  }

	/// <summary>
	/// Given a node, determine if it is a namespace node.
	/// </summary>
	/// <param name="node"> 
	/// </param>
	/// <returns> boolean Returns true if this is a namespace node; otherwise, returns false. </returns>
	  private bool isNamespaceNode(Node node)
	  {

		 if ((null != node) && (node.getNodeType() == Node.ATTRIBUTE_NODE) && (node.getNodeName().StartsWith("xmlns:") || node.getNodeName().Equals("xmlns")))
		 {
			return true;
		 }
		 else
		 {
			return false;
		 }
	  }

	/// <summary>
	/// Add m_contextNode to Event Listner to listen for Mutations Events
	/// 
	/// </summary>
	  private void addEventListener()
	  {
		  if (m_contextNode is EventTarget)
		  {
			((EventTarget)m_contextNode).addEventListener("DOMSubtreeModified",this,true);
		  }

	  }


	/// <summary>
	/// Remove m_contextNode to Event Listner to listen for Mutations Events
	/// 
	/// </summary>
	private void removeEventListener()
	{
		if (m_contextNode is EventTarget)
		{
			((EventTarget)m_contextNode).removeEventListener("DOMSubtreeModified",this,true);
		}
	}

	}

}