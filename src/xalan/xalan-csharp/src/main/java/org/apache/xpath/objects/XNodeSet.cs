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
 * $Id: XNodeSet.java 469368 2006-10-31 04:41:36Z minchau $
 */
namespace org.apache.xpath.objects
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using XMLString = org.apache.xml.utils.XMLString;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using NodeSequence = org.apache.xpath.axes.NodeSequence;

	using NodeList = org.w3c.dom.NodeList;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// This class represents an XPath nodeset object, and is capable of
	/// converting the nodeset to other types, such as a string.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class XNodeSet : NodeSequence
	{
		internal new const long serialVersionUID = 1916026368035639667L;
	  /// <summary>
	  /// Default constructor for derived objects.
	  /// </summary>
	  protected internal XNodeSet()
	  {
	  }

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> Value of the XNodeSet object </param>
	  public XNodeSet(DTMIterator val) : base()
	  {
		  if (val is XNodeSet)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XNodeSet nodeSet = (XNodeSet) val;
			XNodeSet nodeSet = (XNodeSet) val;
			Iter = nodeSet.m_iter;
			m_dtmMgr = nodeSet.m_dtmMgr;
			m_last = nodeSet.m_last;
			// First make sure the DTMIterator val has a cache,
			// so if it doesn't have one, make one.
			if (!nodeSet.hasCache())
			{
				nodeSet.ShouldCacheNodes = true;
			}

			// Get the cache from val and use it ourselves (we share it).
			Object = nodeSet.IteratorCache;
		  }
		  else
		  {
			Iter = val;
		  }
	  }

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="val"> Value of the XNodeSet object </param>
	  public XNodeSet(XNodeSet val) : base()
	  {
		Iter = val.m_iter;
		m_dtmMgr = val.m_dtmMgr;
		m_last = val.m_last;
		if (!val.hasCache())
		{
			val.ShouldCacheNodes = true;
		}
		Object = val.m_obj;
	  }


	  /// <summary>
	  /// Construct an empty XNodeSet object.  This is used to create a mutable 
	  /// nodeset to which random nodes may be added.
	  /// </summary>
	  public XNodeSet(DTMManager dtmMgr) : this(DTM.NULL,dtmMgr)
	  {
	  }

	  /// <summary>
	  /// Construct a XNodeSet object for one node.
	  /// </summary>
	  /// <param name="n"> Node to add to the new XNodeSet object </param>
	  public XNodeSet(int n, DTMManager dtmMgr) : base(new NodeSetDTM(dtmMgr))
	  {

		m_dtmMgr = dtmMgr;

		if (DTM.NULL != n)
		{
		  ((NodeSetDTM) m_obj).addNode(n);
		  m_last = 1;
		}
		else
		{
			m_last = 0;
		}
	  }

	  /// <summary>
	  /// Tell that this is a CLASS_NODESET.
	  /// </summary>
	  /// <returns> type CLASS_NODESET </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_NODESET;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#NODESET" </returns>
	  public override string TypeString
	  {
		  get
		  {
			return "#NODESET";
		  }
	  }

	  /// <summary>
	  /// Get numeric value of the string conversion from a single node.
	  /// </summary>
	  /// <param name="n"> Node to convert
	  /// </param>
	  /// <returns> numeric value of the string conversion from a single node. </returns>
	  public virtual double getNumberFromNode(int n)
	  {
		XMLString xstr = m_dtmMgr.getDTM(n).getStringValue(n);
		return xstr.toDouble();
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> numeric value of the string conversion from the 
	  /// next node in the NodeSetDTM, or NAN if no node was found </returns>
	  public override double num()
	  {

		int node = item(0);
		return (node != DTM.NULL) ? getNumberFromNode(node) : Double.NaN;
	  }

	  /// <summary>
	  /// Cast result object to a number, but allow side effects, such as the 
	  /// incrementing of an iterator.
	  /// </summary>
	  /// <returns> numeric value of the string conversion from the 
	  /// next node in the NodeSetDTM, or NAN if no node was found </returns>
	  public override double numWithSideEffects()
	  {
		int node = nextNode();

		return (node != DTM.NULL) ? getNumberFromNode(node) : Double.NaN;
	  }


	  /// <summary>
	  /// Cast result object to a boolean.
	  /// </summary>
	  /// <returns> True if there is a next node in the nodeset </returns>
	  public override bool @bool()
	  {
		return (item(0) != DTM.NULL);
	  }

	  /// <summary>
	  /// Cast result object to a boolean, but allow side effects, such as the 
	  /// incrementing of an iterator.
	  /// </summary>
	  /// <returns> True if there is a next node in the nodeset </returns>
	  public override bool boolWithSideEffects()
	  {
		return (nextNode() != DTM.NULL);
	  }


	  /// <summary>
	  /// Get the string conversion from a single node.
	  /// </summary>
	  /// <param name="n"> Node to convert
	  /// </param>
	  /// <returns> the string conversion from a single node. </returns>
	  public virtual XMLString getStringFromNode(int n)
	  {
		// %OPT%
		// I guess we'll have to get a static instance of the DTM manager...
		if (DTM.NULL != n)
		{
		  return m_dtmMgr.getDTM(n).getStringValue(n);
		}
		else
		{
		  return org.apache.xpath.objects.XString.EMPTYSTRING;
		}
	  }

	  /// <summary>
	  /// Directly call the
	  /// characters method on the passed ContentHandler for the
	  /// string-value. Multiple calls to the
	  /// ContentHandler's characters methods may well occur for a single call to
	  /// this method.
	  /// </summary>
	  /// <param name="ch"> A non-null reference to a ContentHandler.
	  /// </param>
	  /// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void dispatchCharactersEvents(org.xml.sax.ContentHandler ch) throws org.xml.sax.SAXException
	  public override void dispatchCharactersEvents(org.xml.sax.ContentHandler ch)
	  {
		int node = item(0);

		if (node != DTM.NULL)
		{
		  m_dtmMgr.getDTM(node).dispatchCharactersEvents(node, ch, false);
		}

	  }

	  /// <summary>
	  /// Cast result object to an XMLString.
	  /// </summary>
	  /// <returns> The document fragment node data or the empty string.  </returns>
	  public override XMLString xstr()
	  {
		int node = item(0);
		return (node != DTM.NULL) ? getStringFromNode(node) : XString.EMPTYSTRING;
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
	  /// <returns> the string conversion from the next node in the nodeset
	  /// or "" if there is no next node </returns>
	  public override string str()
	  {
		int node = item(0);
		return (node != DTM.NULL) ? getStringFromNode(node).ToString() : "";
	  }

	  /// <summary>
	  /// Return a java object that's closest to the representation
	  /// that should be handed to an extension.
	  /// </summary>
	  /// <returns> The object that this class wraps </returns>
	  public override object @object()
	  {
		if (null == m_obj)
		{
			return this;
		}
		else
		{
			return m_obj;
		}
	  }

	  // %REVIEW%
	  // hmmm...
	//  /**
	//   * Cast result object to a result tree fragment.
	//   *
	//   * @param support The XPath context to use for the conversion 
	//   *
	//   * @return the nodeset as a result tree fragment.
	//   */
	//  public DocumentFragment rtree(XPathContext support)
	//  {
	//    DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
	//    DocumentBuilder db = dbf.newDocumentBuilder();
	//    Document myDoc = db.newDocument();
	//    
	//    DocumentFragment docFrag = myDoc.createDocumentFragment();
	//
	//    DTMIterator nl = iter();
	//    int node;
	//
	//    while (DTM.NULL != (node = nl.nextNode()))
	//    {
	//      frag.appendChild(node, true, true);
	//    }
	//
	//    return frag.getDocument();
	//  }

	  /// <summary>
	  /// Cast result object to a nodelist.
	  /// </summary>
	  /// <returns> a NodeIterator.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.traversal.NodeIterator nodeset() throws javax.xml.transform.TransformerException
	  public override NodeIterator nodeset()
	  {
		return new org.apache.xml.dtm.@ref.DTMNodeIterator(iter());
	  }

	  /// <summary>
	  /// Cast result object to a nodelist.
	  /// </summary>
	  /// <returns> a NodeList.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.w3c.dom.NodeList nodelist() throws javax.xml.transform.TransformerException
	  public override NodeList nodelist()
	  {
		org.apache.xml.dtm.@ref.DTMNodeList nodelist = new org.apache.xml.dtm.@ref.DTMNodeList(this);
		// Creating a DTMNodeList has the side-effect that it will create a clone
		// XNodeSet with cache and run m_iter to the end. You cannot get any node
		// from m_iter after this call. As a fix, we call SetVector() on the clone's 
		// cache. See Bugzilla 14406.
		XNodeSet clone = (XNodeSet)nodelist.DTMIterator;
		SetVector(clone.Vector);
		return nodelist;
	  }


	//  /**
	//   * Return a java object that's closest to the representation
	//   * that should be handed to an extension.
	//   *
	//   * @return The object that this class wraps
	//   */
	//  public Object object()
	//  {
	//    return new org.apache.xml.dtm.ref.DTMNodeList(iter());
	//  }

	  /// <summary>
	  /// Return the iterator without cloning, etc.
	  /// </summary>
	  public virtual DTMIterator iterRaw()
	  {
		return this;
	  }

	  public virtual void release(DTMIterator iter)
	  {
	  }

	  /// <summary>
	  /// Cast result object to a nodelist.
	  /// </summary>
	  /// <returns> The nodeset as a nodelist </returns>
	  public override DTMIterator iter()
	  {
		try
		{
			if (hasCache())
			{
				  return cloneWithReset();
			}
			  else
			  {
				  return this; // don't bother to clone... won't do any good!
			  }
		}
		catch (CloneNotSupportedException cnse)
		{
		  throw new Exception(cnse.Message);
		}
	  }

	  /// <summary>
	  /// Get a fresh copy of the object.  For use with variables.
	  /// </summary>
	  /// <returns> A fresh nodelist. </returns>
	  public override XObject Fresh
	  {
		  get
		  {
			try
			{
				if (hasCache())
				{
					  return (XObject)cloneWithReset();
				}
				  else
				  {
					  return this; // don't bother to clone... won't do any good!
				  }
			}
			catch (CloneNotSupportedException cnse)
			{
			  throw new Exception(cnse.Message);
			}
		  }
	  }

	  /// <summary>
	  /// Cast result object to a mutableNodeset.
	  /// </summary>
	  /// <returns> The nodeset as a mutableNodeset </returns>
	  public override NodeSetDTM mutableNodeset()
	  {
		NodeSetDTM mnl;

		if (m_obj is NodeSetDTM)
		{
		  mnl = (NodeSetDTM) m_obj;
		}
		else
		{
		  mnl = new NodeSetDTM(iter());
		  Object = mnl;
		  CurrentPos = 0;
		}

		return mnl;
	  }

	  /// <summary>
	  /// Less than comparator </summary>
	  internal static readonly LessThanComparator S_LT = new LessThanComparator();

	  /// <summary>
	  /// Less than or equal comparator </summary>
	  internal static readonly LessThanOrEqualComparator S_LTE = new LessThanOrEqualComparator();

	  /// <summary>
	  /// Greater than comparator </summary>
	  internal static readonly GreaterThanComparator S_GT = new GreaterThanComparator();

	  /// <summary>
	  /// Greater than or equal comparator </summary>
	  internal static readonly GreaterThanOrEqualComparator S_GTE = new GreaterThanOrEqualComparator();

	  /// <summary>
	  /// Equal comparator </summary>
	  internal static readonly EqualComparator S_EQ = new EqualComparator();

	  /// <summary>
	  /// Not equal comparator </summary>
	  internal static readonly NotEqualComparator S_NEQ = new NotEqualComparator();

	  /// <summary>
	  /// Tell if one object is less than the other.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this nodeset to </param>
	  /// <param name="comparator"> Comparator to use
	  /// </param>
	  /// <returns> See the comments below for each object type comparison 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean compare(XObject obj2, Comparator comparator) throws javax.xml.transform.TransformerException
	  public virtual bool compare(XObject obj2, Comparator comparator)
	  {

		bool result = false;
		int type = obj2.Type;

		if (XObject.CLASS_NODESET == type)
		{
		  // %OPT% This should be XMLString based instead of string based...

		  // From http://www.w3.org/TR/xpath: 
		  // If both objects to be compared are node-sets, then the comparison 
		  // will be true if and only if there is a node in the first node-set 
		  // and a node in the second node-set such that the result of performing 
		  // the comparison on the string-values of the two nodes is true.
		  // Note this little gem from the draft:
		  // NOTE: If $x is bound to a node-set, then $x="foo" 
		  // does not mean the same as not($x!="foo"): the former 
		  // is true if and only if some node in $x has the string-value 
		  // foo; the latter is true if and only if all nodes in $x have 
		  // the string-value foo.
		  DTMIterator list1 = iterRaw();
		  DTMIterator list2 = ((XNodeSet) obj2).iterRaw();
		  int node1;
		  ArrayList node2Strings = null;

		  while (DTM.NULL != (node1 = list1.nextNode()))
		  {
			XMLString s1 = getStringFromNode(node1);

			if (null == node2Strings)
			{
			  int node2;

			  while (DTM.NULL != (node2 = list2.nextNode()))
			  {
				XMLString s2 = getStringFromNode(node2);

				if (comparator.compareStrings(s1, s2))
				{
				  result = true;

				  break;
				}

				if (null == node2Strings)
				{
				  node2Strings = new ArrayList();
				}

				node2Strings.Add(s2);
			  }
			}
			else
			{
			  int n = node2Strings.Count;

			  for (int i = 0; i < n; i++)
			  {
				if (comparator.compareStrings(s1, (XMLString)node2Strings[i]))
				{
				  result = true;

				  break;
				}
			  }
			}
		  }
		  list1.reset();
		  list2.reset();
		}
		else if (XObject.CLASS_BOOLEAN == type)
		{

		  // From http://www.w3.org/TR/xpath: 
		  // If one object to be compared is a node-set and the other is a boolean, 
		  // then the comparison will be true if and only if the result of 
		  // performing the comparison on the boolean and on the result of 
		  // converting the node-set to a boolean using the boolean function 
		  // is true.
		  double num1 = @bool() ? 1.0 : 0.0;
		  double num2 = obj2.num();

		  result = comparator.compareNumbers(num1, num2);
		}
		else if (XObject.CLASS_NUMBER == type)
		{

		  // From http://www.w3.org/TR/xpath: 
		  // If one object to be compared is a node-set and the other is a number, 
		  // then the comparison will be true if and only if there is a 
		  // node in the node-set such that the result of performing the 
		  // comparison on the number to be compared and on the result of 
		  // converting the string-value of that node to a number using 
		  // the number function is true. 
		  DTMIterator list1 = iterRaw();
		  double num2 = obj2.num();
		  int node;

		  while (DTM.NULL != (node = list1.nextNode()))
		  {
			double num1 = getNumberFromNode(node);

			if (comparator.compareNumbers(num1, num2))
			{
			  result = true;

			  break;
			}
		  }
		  list1.reset();
		}
		else if (XObject.CLASS_RTREEFRAG == type)
		{
		  XMLString s2 = obj2.xstr();
		  DTMIterator list1 = iterRaw();
		  int node;

		  while (DTM.NULL != (node = list1.nextNode()))
		  {
			XMLString s1 = getStringFromNode(node);

			if (comparator.compareStrings(s1, s2))
			{
			  result = true;

			  break;
			}
		  }
		  list1.reset();
		}
		else if (XObject.CLASS_STRING == type)
		{

		  // From http://www.w3.org/TR/xpath: 
		  // If one object to be compared is a node-set and the other is a 
		  // string, then the comparison will be true if and only if there 
		  // is a node in the node-set such that the result of performing 
		  // the comparison on the string-value of the node and the other 
		  // string is true. 
		  XMLString s2 = obj2.xstr();
		  DTMIterator list1 = iterRaw();
		  int node;

		  while (DTM.NULL != (node = list1.nextNode()))
		  {
			XMLString s1 = getStringFromNode(node);
			if (comparator.compareStrings(s1, s2))
			{
			  result = true;

			  break;
			}
		  }
		  list1.reset();
		}
		else
		{
		  result = comparator.compareNumbers(this.num(), obj2.num());
		}

		return result;
	  }

	  /// <summary>
	  /// Tell if one object is less than the other.
	  /// </summary>
	  /// <param name="obj2"> object to compare this nodeset to
	  /// </param>
	  /// <returns> see this.compare(...) 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean lessThan(XObject obj2) throws javax.xml.transform.TransformerException
	  public override bool lessThan(XObject obj2)
	  {
		return compare(obj2, S_LT);
	  }

	  /// <summary>
	  /// Tell if one object is less than or equal to the other.
	  /// </summary>
	  /// <param name="obj2"> object to compare this nodeset to
	  /// </param>
	  /// <returns> see this.compare(...) 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean lessThanOrEqual(XObject obj2) throws javax.xml.transform.TransformerException
	  public override bool lessThanOrEqual(XObject obj2)
	  {
		return compare(obj2, S_LTE);
	  }

	  /// <summary>
	  /// Tell if one object is less than the other.
	  /// </summary>
	  /// <param name="obj2"> object to compare this nodeset to
	  /// </param>
	  /// <returns> see this.compare(...) 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean greaterThan(XObject obj2) throws javax.xml.transform.TransformerException
	  public override bool greaterThan(XObject obj2)
	  {
		return compare(obj2, S_GT);
	  }

	  /// <summary>
	  /// Tell if one object is less than the other.
	  /// </summary>
	  /// <param name="obj2"> object to compare this nodeset to
	  /// </param>
	  /// <returns> see this.compare(...) 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean greaterThanOrEqual(XObject obj2) throws javax.xml.transform.TransformerException
	  public override bool greaterThanOrEqual(XObject obj2)
	  {
		return compare(obj2, S_GTE);
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> object to compare this nodeset to
	  /// </param>
	  /// <returns> see this.compare(...) 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public override bool Equals(XObject obj2)
	  {
		try
		{
		  return compare(obj2, S_EQ);
		}
		catch (javax.xml.transform.TransformerException te)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(te);
		}
	  }

	  /// <summary>
	  /// Tell if two objects are functionally not equal.
	  /// </summary>
	  /// <param name="obj2"> object to compare this nodeset to
	  /// </param>
	  /// <returns> see this.compare(...) 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean notEquals(XObject obj2) throws javax.xml.transform.TransformerException
	  public override bool notEquals(XObject obj2)
	  {
		return compare(obj2, S_NEQ);
	  }
	}

	/// <summary>
	/// compares nodes for various boolean operations.
	/// </summary>
	internal abstract class Comparator
	{

	  /// <summary>
	  /// Compare two strings
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare 
	  /// </param>
	  /// <returns> Whether the strings are equal or not </returns>
	  internal abstract bool compareStrings(XMLString s1, XMLString s2);

	  /// <summary>
	  /// Compare two numbers
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> Whether the numbers are equal or not </returns>
	  internal abstract bool compareNumbers(double n1, double n2);
	}

	/// <summary>
	/// Compare strings or numbers for less than.
	/// </summary>
	internal class LessThanComparator : Comparator
	{

	  /// <summary>
	  /// Compare two strings for less than.
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare 
	  /// </param>
	  /// <returns> True if s1 is less than s2 </returns>
	  internal override bool compareStrings(XMLString s1, XMLString s2)
	  {
		return (s1.toDouble() < s2.toDouble());
		// return s1.compareTo(s2) < 0;
	  }

	  /// <summary>
	  /// Compare two numbers for less than.
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> true if n1 is less than n2 </returns>
	  internal override bool compareNumbers(double n1, double n2)
	  {
		return n1 < n2;
	  }
	}

	/// <summary>
	/// Compare strings or numbers for less than or equal.
	/// </summary>
	internal class LessThanOrEqualComparator : Comparator
	{

	  /// <summary>
	  /// Compare two strings for less than or equal.
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare
	  /// </param>
	  /// <returns> true if s1 is less than or equal to s2 </returns>
	  internal override bool compareStrings(XMLString s1, XMLString s2)
	  {
		return (s1.toDouble() <= s2.toDouble());
		// return s1.compareTo(s2) <= 0;
	  }

	  /// <summary>
	  /// Compare two numbers for less than or equal.
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> true if n1 is less than or equal to n2 </returns>
	  internal override bool compareNumbers(double n1, double n2)
	  {
		return n1 <= n2;
	  }
	}

	/// <summary>
	/// Compare strings or numbers for greater than.
	/// </summary>
	internal class GreaterThanComparator : Comparator
	{

	  /// <summary>
	  /// Compare two strings for greater than.
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare
	  /// </param>
	  /// <returns> true if s1 is greater than s2 </returns>
	  internal override bool compareStrings(XMLString s1, XMLString s2)
	  {
		return (s1.toDouble() > s2.toDouble());
		// return s1.compareTo(s2) > 0;
	  }

	  /// <summary>
	  /// Compare two numbers for greater than.
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> true if n1 is greater than n2 </returns>
	  internal override bool compareNumbers(double n1, double n2)
	  {
		return n1 > n2;
	  }
	}

	/// <summary>
	/// Compare strings or numbers for greater than or equal.
	/// </summary>
	internal class GreaterThanOrEqualComparator : Comparator
	{

	  /// <summary>
	  /// Compare two strings for greater than or equal.
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare
	  /// </param>
	  /// <returns> true if s1 is greater than or equal to s2 </returns>
	  internal override bool compareStrings(XMLString s1, XMLString s2)
	  {
		return (s1.toDouble() >= s2.toDouble());
		// return s1.compareTo(s2) >= 0;
	  }

	  /// <summary>
	  /// Compare two numbers for greater than or equal.
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> true if n1 is greater than or equal to n2 </returns>
	  internal override bool compareNumbers(double n1, double n2)
	  {
		return n1 >= n2;
	  }
	}

	/// <summary>
	/// Compare strings or numbers for equality.
	/// </summary>
	internal class EqualComparator : Comparator
	{

	  /// <summary>
	  /// Compare two strings for equality.
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare
	  /// </param>
	  /// <returns> true if s1 is equal to s2 </returns>
	  internal override bool compareStrings(XMLString s1, XMLString s2)
	  {
		return s1.Equals(s2);
	  }

	  /// <summary>
	  /// Compare two numbers for equality.
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> true if n1 is equal to n2 </returns>
	  internal override bool compareNumbers(double n1, double n2)
	  {
		return n1 == n2;
	  }
	}

	/// <summary>
	/// Compare strings or numbers for non-equality.
	/// </summary>
	internal class NotEqualComparator : Comparator
	{

	  /// <summary>
	  /// Compare two strings for non-equality.
	  /// 
	  /// </summary>
	  /// <param name="s1"> First string to compare </param>
	  /// <param name="s2"> Second String to compare
	  /// </param>
	  /// <returns> true if s1 is not equal to s2 </returns>
	  internal override bool compareStrings(XMLString s1, XMLString s2)
	  {
		return !s1.Equals(s2);
	  }

	  /// <summary>
	  /// Compare two numbers for non-equality.
	  /// 
	  /// </summary>
	  /// <param name="n1"> First number to compare </param>
	  /// <param name="n2"> Second number to compare
	  /// </param>
	  /// <returns> true if n1 is not equal to n2 </returns>
	  internal override bool compareNumbers(double n1, double n2)
	  {
		return n1 != n2;
	  }
	}

}