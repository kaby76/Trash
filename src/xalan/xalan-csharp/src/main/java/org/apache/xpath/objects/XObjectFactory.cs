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
 * $Id: XObjectFactory.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.objects
{
	using Axis = org.apache.xml.dtm.Axis;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XPathContext = org.apache.xpath.XPathContext;
	using OneStepIterator = org.apache.xpath.axes.OneStepIterator;


	public class XObjectFactory
	{

	  /// <summary>
	  /// Create the right XObject based on the type of the object passed.  This 
	  /// function can not make an XObject that exposes DOM Nodes, NodeLists, and 
	  /// NodeIterators to the XSLT stylesheet as node-sets.
	  /// </summary>
	  /// <param name="val"> The java object which this object will wrap.
	  /// </param>
	  /// <returns> the right XObject based on the type of the object passed. </returns>
	  public static XObject create(object val)
	  {

		XObject result;

		if (val is XObject)
		{
		  result = (XObject) val;
		}
		else if (val is string)
		{
		  result = new XString((string) val);
		}
		else if (val is Boolean)
		{
		  result = new XBoolean(((bool?)val).Value);
		}
		else if (val is Double)
		{
		  result = new XNumber(((double?) val).Value);
		}
		else
		{
		  result = new XObject(val);
		}

		return result;
	  }

	  /// <summary>
	  /// Create the right XObject based on the type of the object passed.
	  /// This function <emph>can</emph> make an XObject that exposes DOM Nodes, NodeLists, and 
	  /// NodeIterators to the XSLT stylesheet as node-sets.
	  /// </summary>
	  /// <param name="val"> The java object which this object will wrap. </param>
	  /// <param name="xctxt"> The XPath context.
	  /// </param>
	  /// <returns> the right XObject based on the type of the object passed. </returns>
	  public static XObject create(object val, XPathContext xctxt)
	  {

		XObject result;

		if (val is XObject)
		{
		  result = (XObject) val;
		}
		else if (val is string)
		{
		  result = new XString((string) val);
		}
		else if (val is Boolean)
		{
		  result = new XBoolean(((bool?)val).Value);
		}
		else if (val is Number)
		{
		  result = new XNumber(((Number) val));
		}
		else if (val is DTM)
		{
		  DTM dtm = (DTM)val;
		  try
		  {
			int dtmRoot = dtm.Document;
			DTMAxisIterator iter = dtm.getAxisIterator(Axis.SELF);
			iter.StartNode = dtmRoot;
			DTMIterator iterator = new OneStepIterator(iter, Axis.SELF);
			iterator.setRoot(dtmRoot, xctxt);
			result = new XNodeSet(iterator);
		  }
		  catch (Exception ex)
		  {
			throw new org.apache.xml.utils.WrappedRuntimeException(ex);
		  }
		}
		else if (val is DTMAxisIterator)
		{
		  DTMAxisIterator iter = (DTMAxisIterator)val;
		  try
		  {
			DTMIterator iterator = new OneStepIterator(iter, Axis.SELF);
			iterator.setRoot(iter.StartNode, xctxt);
			result = new XNodeSet(iterator);
		  }
		  catch (Exception ex)
		  {
			throw new org.apache.xml.utils.WrappedRuntimeException(ex);
		  }
		}
		else if (val is DTMIterator)
		{
		  result = new XNodeSet((DTMIterator) val);
		}
		// This next three instanceofs are a little worrysome, since a NodeList 
		// might also implement a Node!
		else if (val is org.w3c.dom.Node)
		{
		  result = new XNodeSetForDOM((org.w3c.dom.Node)val, xctxt);
		}
		// This must come after org.w3c.dom.Node, since many Node implementations 
		// also implement NodeList.
		else if (val is org.w3c.dom.NodeList)
		{
		  result = new XNodeSetForDOM((org.w3c.dom.NodeList)val, xctxt);
		}
		else if (val is org.w3c.dom.traversal.NodeIterator)
		{
		  result = new XNodeSetForDOM((org.w3c.dom.traversal.NodeIterator)val, xctxt);
		}
		else
		{
		  result = new XObject(val);
		}

		return result;
	  }
	}

}