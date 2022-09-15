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
 * $Id: NodeInfo.java 468639 2006-10-28 06:52:33Z minchau $
 */

namespace org.apache.xalan.lib
{

	using ExpressionContext = org.apache.xalan.extensions.ExpressionContext;
	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// <code>NodeInfo</code> defines a set of XSLT extension functions to be
	/// used from stylesheets.
	/// 
	/// @author <a href="mailto:ovidiu@cup.hp.com">Ovidiu Predescu</a>
	/// @since May 24, 2001
	/// </summary>
	public class NodeInfo
	{
	  /// <summary>
	  /// <code>systemId</code> returns the system id of the current
	  /// context node.
	  /// </summary>
	  /// <param name="context"> an <code>ExpressionContext</code> value </param>
	  /// <returns> a <code>String</code> value </returns>
	  public static string systemId(ExpressionContext context)
	  {
		Node contextNode = context.ContextNode;
		int nodeHandler = ((DTMNodeProxy)contextNode).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)contextNode).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.SystemId;
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// <code>systemId</code> returns the system id of the node passed as
	  /// argument. If a node set is passed as argument, the system id of
	  /// the first node in the set is returned.
	  /// </summary>
	  /// <param name="nodeList"> a <code>NodeList</code> value </param>
	  /// <returns> a <code>String</code> value </returns>
	  public static string systemId(NodeList nodeList)
	  {
		if (nodeList == null || nodeList.Length == 0)
		{
		  return null;
		}

		Node node = nodeList.item(0);
		int nodeHandler = ((DTMNodeProxy)node).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)node).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.SystemId;
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// <code>publicId</code> returns the public identifier of the current
	  /// context node.
	  /// 
	  /// Xalan does not currently record this value, and will return null.
	  /// </summary>
	  /// <param name="context"> an <code>ExpressionContext</code> value </param>
	  /// <returns> a <code>String</code> value </returns>
	  public static string publicId(ExpressionContext context)
	  {
		Node contextNode = context.ContextNode;
		int nodeHandler = ((DTMNodeProxy)contextNode).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)contextNode).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.PublicId;
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// <code>publicId</code> returns the public identifier of the node passed as
	  /// argument. If a node set is passed as argument, the public identifier of
	  /// the first node in the set is returned.
	  /// 
	  /// Xalan does not currently record this value, and will return null.
	  /// </summary>
	  /// <param name="nodeList"> a <code>NodeList</code> value </param>
	  /// <returns> a <code>String</code> value </returns>
	  public static string publicId(NodeList nodeList)
	  {
		if (nodeList == null || nodeList.Length == 0)
		{
		  return null;
		}

		Node node = nodeList.item(0);
		int nodeHandler = ((DTMNodeProxy)node).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)node).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.PublicId;
		}
		else
		{
		  return null;
		}
	  }

	  /// <summary>
	  /// <code>lineNumber</code> returns the line number of the current
	  /// context node.
	  /// 
	  /// NOTE: Xalan does not normally record location information for each node. 
	  /// To obtain it, you must set the custom TrAX attribute 
	  /// "http://xml.apache.org/xalan/features/source_location"
	  /// true in the TransformerFactory before generating the Transformer and executing
	  /// the stylesheet. Storage cost per node will be noticably increased in this mode.
	  /// </summary>
	  /// <param name="context"> an <code>ExpressionContext</code> value </param>
	  /// <returns> an <code>int</code> value. This may be -1 to indicate that the
	  /// line number is not known. </returns>
	  public static int lineNumber(ExpressionContext context)
	  {
		Node contextNode = context.ContextNode;
		int nodeHandler = ((DTMNodeProxy)contextNode).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)contextNode).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.LineNumber;
		}
		else
		{
		  return -1;
		}
	  }

	  /// <summary>
	  /// <code>lineNumber</code> returns the line number of the node
	  /// passed as argument. If a node set is passed as argument, the line
	  /// number of the first node in the set is returned.
	  /// 
	  /// NOTE: Xalan does not normally record location information for each node. 
	  /// To obtain it, you must set the custom TrAX attribute 
	  /// "http://xml.apache.org/xalan/features/source_location"
	  /// true in the TransformerFactory before generating the Transformer and executing
	  /// the stylesheet. Storage cost per node will be noticably increased in this mode.
	  /// </summary>
	  /// <param name="nodeList"> a <code>NodeList</code> value </param>
	  /// <returns> an <code>int</code> value. This may be -1 to indicate that the
	  /// line number is not known. </returns>
	  public static int lineNumber(NodeList nodeList)
	  {
		if (nodeList == null || nodeList.Length == 0)
		{
		  return -1;
		}

		Node node = nodeList.item(0);
		int nodeHandler = ((DTMNodeProxy)node).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)node).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.LineNumber;
		}
		else
		{
		  return -1;
		}
	  }

	  /// <summary>
	  /// <code>columnNumber</code> returns the column number of the
	  /// current context node.
	  /// 
	  /// NOTE: Xalan does not normally record location information for each node. 
	  /// To obtain it, you must set the custom TrAX attribute 
	  /// "http://xml.apache.org/xalan/features/source_location"
	  /// true in the TransformerFactory before generating the Transformer and executing
	  /// the stylesheet. Storage cost per node will be noticably increased in this mode.
	  /// </summary>
	  /// <param name="context"> an <code>ExpressionContext</code> value </param>
	  /// <returns> an <code>int</code> value. This may be -1 to indicate that the
	  /// column number is not known. </returns>
	  public static int columnNumber(ExpressionContext context)
	  {
		Node contextNode = context.ContextNode;
		int nodeHandler = ((DTMNodeProxy)contextNode).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)contextNode).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.ColumnNumber;
		}
		else
		{
		  return -1;
		}
	  }

	  /// <summary>
	  /// <code>columnNumber</code> returns the column number of the node
	  /// passed as argument. If a node set is passed as argument, the line
	  /// number of the first node in the set is returned.
	  /// 
	  /// NOTE: Xalan does not normally record location information for each node. 
	  /// To obtain it, you must set the custom TrAX attribute 
	  /// "http://xml.apache.org/xalan/features/source_location"
	  /// true in the TransformerFactory before generating the Transformer and executing
	  /// the stylesheet. Storage cost per node will be noticably increased in this mode.
	  /// </summary>
	  /// <param name="nodeList"> a <code>NodeList</code> value </param>
	  /// <returns> an <code>int</code> value. This may be -1 to indicate that the
	  /// column number is not known. </returns>
	  public static int columnNumber(NodeList nodeList)
	  {
		if (nodeList == null || nodeList.Length == 0)
		{
		  return -1;
		}

		Node node = nodeList.item(0);
		int nodeHandler = ((DTMNodeProxy)node).DTMNodeNumber;
		SourceLocator locator = ((DTMNodeProxy)node).DTM.getSourceLocatorFor(nodeHandler);

		if (locator != null)
		{
		  return locator.ColumnNumber;
		}
		else
		{
		  return -1;
		}
	  }
	}

}