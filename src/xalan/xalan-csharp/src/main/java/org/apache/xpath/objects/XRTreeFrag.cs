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
 * $Id: XRTreeFrag.java 469368 2006-10-31 04:41:36Z minchau $
 */
namespace org.apache.xpath.objects
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XMLString = org.apache.xml.utils.XMLString;
	using Expression = org.apache.xpath.Expression;
	using ExpressionNode = org.apache.xpath.ExpressionNode;
	using XPathContext = org.apache.xpath.XPathContext;
	using RTFIterator = org.apache.xpath.axes.RTFIterator;

	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// This class represents an XPath result tree fragment object, and is capable of
	/// converting the RTF to other types, such as a string.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class XRTreeFrag : XObject, ICloneable
	{
		internal new const long serialVersionUID = -3201553822254911567L;
	  private DTMXRTreeFrag m_DTMXRTreeFrag;
	  private int m_dtmRoot = DTM.NULL;
	  protected internal bool m_allowRelease = false;


	  /// <summary>
	  /// Create an XRTreeFrag Object.
	  /// 
	  /// </summary>
	  public XRTreeFrag(int root, XPathContext xctxt, ExpressionNode parent) : base(null)
	  {
		exprSetParent(parent);
		initDTM(root, xctxt);
	  }

	  /// <summary>
	  /// Create an XRTreeFrag Object.
	  /// 
	  /// </summary>
	  public XRTreeFrag(int root, XPathContext xctxt) : base(null)
	  {
	   initDTM(root, xctxt);
	  }

	  private void initDTM(int root, XPathContext xctxt)
	  {
		m_dtmRoot = root;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xml.dtm.DTM dtm = xctxt.getDTM(root);
		DTM dtm = xctxt.getDTM(root);
		if (dtm != null)
		{
		  m_DTMXRTreeFrag = xctxt.getDTMXRTreeFrag(xctxt.getDTMIdentity(dtm));
		}
	  }

	  /// <summary>
	  /// Return a java object that's closest to the representation
	  /// that should be handed to an extension.
	  /// </summary>
	  /// <returns> The object that this class wraps </returns>
	  public override object @object()
	  {
		if (m_DTMXRTreeFrag.XPathContext != null)
		{
		  return new org.apache.xml.dtm.@ref.DTMNodeIterator((DTMIterator)(new org.apache.xpath.NodeSetDTM(m_dtmRoot, m_DTMXRTreeFrag.XPathContext.DTMManager)));
		}
		else
		{
		  return base.@object();
		}
	  }

	  /// <summary>
	  /// Create an XRTreeFrag Object.
	  /// 
	  /// </summary>
	  public XRTreeFrag(Expression expr) : base(expr)
	  {
	  }

	  /// <summary>
	  /// Specify if it's OK for detach to release the iterator for reuse.
	  /// </summary>
	  /// <param name="allowRelease"> true if it is OK for detach to release this iterator 
	  /// for pooling. </param>
	  public override void allowDetachToRelease(bool allowRelease)
	  {
		m_allowRelease = allowRelease;
	  }

	  /// <summary>
	  /// Detaches the <code>DTMIterator</code> from the set which it iterated
	  /// over, releasing any computational resources and placing the iterator
	  /// in the INVALID state. After <code>detach</code> has been invoked,
	  /// calls to <code>nextNode</code> or <code>previousNode</code> will
	  /// raise a runtime exception.
	  /// 
	  /// In general, detach should only be called once on the object.
	  /// </summary>
	  public override void detach()
	  {
		if (m_allowRelease)
		{
			m_DTMXRTreeFrag.destruct();
		  Object = null;
		}
	  }

	  /// <summary>
	  /// Tell what kind of class this is.
	  /// </summary>
	  /// <returns> type CLASS_RTREEFRAG  </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_RTREEFRAG;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#RTREEFRAG" </returns>
	  public override string TypeString
	  {
		  get
		  {
			return "#RTREEFRAG";
		  }
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> The result tree fragment as a number or NaN </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double num() throws javax.xml.transform.TransformerException
	  public override double num()
	  {

		XMLString s = xstr();

		return s.toDouble();
	  }

	  /// <summary>
	  /// Cast result object to a boolean.  This always returns true for a RTreeFrag
	  /// because it is treated like a node-set with a single root node.
	  /// </summary>
	  /// <returns> true </returns>
	  public override bool @bool()
	  {
		return true;
	  }

	  private XMLString m_xmlStr = null;

	  /// <summary>
	  /// Cast result object to an XMLString.
	  /// </summary>
	  /// <returns> The document fragment node data or the empty string.  </returns>
	  public override XMLString xstr()
	  {
		if (null == m_xmlStr)
		{
		  m_xmlStr = m_DTMXRTreeFrag.DTM.getStringValue(m_dtmRoot);
		}

		return m_xmlStr;
	  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The string this wraps or the empty string if null </returns>
	  public override void appendToFsb(org.apache.xml.utils.FastStringBuffer fsb)
	  {
		XString xstring = (XString)xstr();
		xstring.appendToFsb(fsb);
	  }


	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> The document fragment node data or the empty string.  </returns>
	  public override string str()
	  {
		string str = m_DTMXRTreeFrag.DTM.getStringValue(m_dtmRoot).ToString();

		return (null == str) ? "" : str;
	  }

	  /// <summary>
	  /// Cast result object to a result tree fragment.
	  /// </summary>
	  /// <returns> The document fragment this wraps </returns>
	  public override int rtf()
	  {
		return m_dtmRoot;
	  }

	  /// <summary>
	  /// Cast result object to a DTMIterator.
	  /// dml - modified to return an RTFIterator for
	  /// benefit of EXSLT object-type function in 
	  /// <seealso cref="org.apache.xalan.lib.ExsltCommon"/>. </summary>
	  /// <returns> The document fragment as a DTMIterator </returns>
	  public virtual DTMIterator asNodeIterator()
	  {
		return new RTFIterator(m_dtmRoot, m_DTMXRTreeFrag.XPathContext.DTMManager);
	  }

	  /// <summary>
	  /// Cast result object to a nodelist. (special function).
	  /// </summary>
	  /// <returns> The document fragment as a nodelist </returns>
	  public virtual NodeList convertToNodeset()
	  {

		if (m_obj is NodeList)
		{
		  return (NodeList) m_obj;
		}
		else
		{
		  return new org.apache.xml.dtm.@ref.DTMNodeList(asNodeIterator());
		}
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> True if the two objects are equal
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public override bool Equals(XObject obj2)
	  {

		try
		{
		  if (XObject.CLASS_NODESET == obj2.Type)
		  {

			// In order to handle the 'all' semantics of 
			// nodeset comparisons, we always call the 
			// nodeset function.
			return obj2.Equals(this);
		  }
		  else if (XObject.CLASS_BOOLEAN == obj2.Type)
		  {
			return @bool() == obj2.@bool();
		  }
		  else if (XObject.CLASS_NUMBER == obj2.Type)
		  {
			return num() == obj2.num();
		  }
		  else if (XObject.CLASS_NODESET == obj2.Type)
		  {
			return xstr().Equals(obj2.xstr());
		  }
		  else if (XObject.CLASS_STRING == obj2.Type)
		  {
			return xstr().Equals(obj2.xstr());
		  }
		  else if (XObject.CLASS_RTREEFRAG == obj2.Type)
		  {

			// Probably not so good.  Think about this.
			return xstr().Equals(obj2.xstr());
		  }
		  else
		  {
			return base.Equals(obj2);
		  }
		}
		catch (javax.xml.transform.TransformerException te)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(te);
		}
	  }

	}

}