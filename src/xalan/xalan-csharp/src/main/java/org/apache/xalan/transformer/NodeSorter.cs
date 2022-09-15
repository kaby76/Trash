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
 * $Id: NodeSorter.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XPathContext = org.apache.xpath.XPathContext;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// This class can sort vectors of DOM nodes according to a select pattern.
	/// @xsl.usage internal
	/// </summary>
	public class NodeSorter
	{

	  /// <summary>
	  /// Current XPath context </summary>
	  internal XPathContext m_execContext;

	  /// <summary>
	  /// Vector of NodeSortKeys </summary>
	  internal ArrayList m_keys; // vector of NodeSortKeys

	//  /**
	//   * TODO: Adjust this for locale.
	//   */
	//  NumberFormat m_formatter = NumberFormat.getNumberInstance();

	  /// <summary>
	  /// Construct a NodeSorter, passing in the XSL TransformerFactory
	  /// so it can know how to get the node data according to
	  /// the proper whitespace rules.
	  /// </summary>
	  /// <param name="p"> Xpath context to use </param>
	  public NodeSorter(XPathContext p)
	  {
		m_execContext = p;
	  }

	  /// <summary>
	  /// Given a vector of nodes, sort each node according to
	  /// the criteria in the keys. </summary>
	  /// <param name="v"> an vector of Nodes. </param>
	  /// <param name="keys"> a vector of NodeSortKeys. </param>
	  /// <param name="support"> XPath context to use
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void sort(org.apache.xml.dtm.DTMIterator v, java.util.Vector keys, org.apache.xpath.XPathContext support) throws javax.xml.transform.TransformerException
	  public virtual void sort(DTMIterator v, ArrayList keys, XPathContext support)
	  {

		m_keys = keys;

		// QuickSort2(v, 0, v.size() - 1 );
		int n = v.Length;

		// %OPT% Change mergesort to just take a DTMIterator?
		// We would also have to adapt DTMIterator to have the function 
		// of NodeCompareElem.

		// Create a vector of node compare elements
		// based on the input vector of nodes
		ArrayList nodes = new ArrayList();

		for (int i = 0; i < n; i++)
		{
		  NodeCompareElem elem = new NodeCompareElem(this, v.item(i));

		  nodes.Add(elem);
		}

		ArrayList scratchVector = new ArrayList();

		mergesort(nodes, scratchVector, 0, n - 1, support);

		// return sorted vector of nodes
		for (int i = 0; i < n; i++)
		{
		  v.setItem(((NodeCompareElem) nodes[i]).m_node, i);
		}
		v.CurrentPos = 0;

		// old code...
		//NodeVector scratchVector = new NodeVector(n);
		//mergesort(v, scratchVector, 0, n - 1, support);
	  }

	  /// <summary>
	  /// Return the results of a compare of two nodes.
	  /// TODO: Optimize compare -- cache the getStringExpr results, key by m_selectPat + hash of node.
	  /// </summary>
	  /// <param name="n1"> First node to use in compare </param>
	  /// <param name="n2"> Second node to use in compare </param>
	  /// <param name="kIndex"> Index of NodeSortKey to use for sort </param>
	  /// <param name="support"> XPath context to use
	  /// </param>
	  /// <returns> The results of the compare of the two nodes.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: int compare(NodeCompareElem n1, NodeCompareElem n2, int kIndex, org.apache.xpath.XPathContext support) throws javax.xml.transform.TransformerException
	  internal virtual int compare(NodeCompareElem n1, NodeCompareElem n2, int kIndex, XPathContext support)
	  {

		int result = 0;
		NodeSortKey k = (NodeSortKey) m_keys[kIndex];

		if (k.m_treatAsNumbers)
		{
		  double n1Num, n2Num;

		  if (kIndex == 0)
		  {
			n1Num = ((double?) n1.m_key1Value).Value;
			n2Num = ((double?) n2.m_key1Value).Value;
		  }
		  else if (kIndex == 1)
		  {
			n1Num = ((double?) n1.m_key2Value).Value;
			n2Num = ((double?) n2.m_key2Value).Value;
		  }

		  /* Leave this in case we decide to use an array later
		  if (kIndex < maxkey)
		  {
		  double n1Num = (double)n1.m_keyValue[kIndex];
		  double n2Num = (double)n2.m_keyValue[kIndex];
		  }*/
		  else
		  {

			// Get values dynamically
			XObject r1 = k.m_selectPat.execute(m_execContext, n1.m_node, k.m_namespaceContext);
			XObject r2 = k.m_selectPat.execute(m_execContext, n2.m_node, k.m_namespaceContext);
			n1Num = r1.num();

			// Can't use NaN for compare. They are never equal. Use zero instead.
			// That way we can keep elements in document order. 
			//n1Num = Double.isNaN(d) ? 0.0 : d;
			n2Num = r2.num();
			//n2Num = Double.isNaN(d) ? 0.0 : d;
		  }

		  if ((n1Num == n2Num) && ((kIndex + 1) < m_keys.Count))
		  {
			result = compare(n1, n2, kIndex + 1, support);
		  }
		  else
		  {
			double diff;
			if (double.IsNaN(n1Num))
			{
			  if (double.IsNaN(n2Num))
			  {
				diff = 0.0;
			  }
			  else
			  {
				diff = -1;
			  }
			}
			else if (double.IsNaN(n2Num))
			{
			   diff = 1;
			}
			else
			{
			  diff = n1Num - n2Num;
			}

			// process order parameter 
			result = (int)((diff < 0.0) ? (k.m_descending ? 1 : -1) : (diff > 0.0) ? (k.m_descending ? -1 : 1) : 0);
		  }
		} // end treat as numbers
		else
		{
		  CollationKey n1String, n2String;

		  if (kIndex == 0)
		  {
			n1String = (CollationKey) n1.m_key1Value;
			n2String = (CollationKey) n2.m_key1Value;
		  }
		  else if (kIndex == 1)
		  {
			n1String = (CollationKey) n1.m_key2Value;
			n2String = (CollationKey) n2.m_key2Value;
		  }

		  /* Leave this in case we decide to use an array later
		  if (kIndex < maxkey)
		  {
		    String n1String = (String)n1.m_keyValue[kIndex];
		    String n2String = (String)n2.m_keyValue[kIndex];
		  }*/
		  else
		  {

			// Get values dynamically
			XObject r1 = k.m_selectPat.execute(m_execContext, n1.m_node, k.m_namespaceContext);
			XObject r2 = k.m_selectPat.execute(m_execContext, n2.m_node, k.m_namespaceContext);

			n1String = k.m_col.getCollationKey(r1.str());
			n2String = k.m_col.getCollationKey(r2.str());
		  }

		  // Use collation keys for faster compare, but note that whitespaces 
		  // etc... are treated differently from if we were comparing Strings.
		  result = n1String.compareTo(n2String);

		  //Process caseOrder parameter
		  if (k.m_caseOrderUpper)
		  {
			string tempN1 = n1String.getSourceString().ToLower();
			string tempN2 = n2String.getSourceString().ToLower();

			if (tempN1.Equals(tempN2))
			{

			  //java defaults to upper case is greater.
			  result = result == 0 ? 0 : -result;
			}
		  }

		  //Process order parameter
		  if (k.m_descending)
		  {
			result = -result;
		  }
		} //end else

		if (0 == result)
		{
		  if ((kIndex + 1) < m_keys.Count)
		  {
			result = compare(n1, n2, kIndex + 1, support);
		  }
		}

		if (0 == result)
		{

		  // I shouldn't have to do this except that there seems to 
		  // be a glitch in the mergesort
		  // if(r1.getType() == r1.CLASS_NODESET)
		  // {
		  DTM dtm = support.getDTM(n1.m_node); // %OPT%
		  result = dtm.isNodeAfter(n1.m_node, n2.m_node) ? -1 : 1;

		  // }
		}

		return result;
	  }

	  /// <summary>
	  /// This implements a standard Mergesort, as described in
	  /// Robert Sedgewick's Algorithms book.  This is a better
	  /// sort for our purpose than the Quicksort because it
	  /// maintains the original document order of the input if
	  /// the order isn't changed by the sort.
	  /// </summary>
	  /// <param name="a"> First vector of nodes to compare </param>
	  /// <param name="b"> Second vector of  nodes to compare </param>
	  /// <param name="l"> Left boundary of  partition </param>
	  /// <param name="r"> Right boundary of  partition </param>
	  /// <param name="support"> XPath context to use
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void mergesort(java.util.Vector a, java.util.Vector b, int l, int r, org.apache.xpath.XPathContext support) throws javax.xml.transform.TransformerException
	  internal virtual void mergesort(ArrayList a, ArrayList b, int l, int r, XPathContext support)
	  {

		if ((r - l) > 0)
		{
		  int m = (r + l) / 2;

		  mergesort(a, b, l, m, support);
		  mergesort(a, b, m + 1, r, support);

		  int i, j, k;

		  for (i = m; i >= l; i--)
		  {

			// b[i] = a[i];
			// Use insert if we need to increment vector size.
			if (i >= b.Count)
			{
			  b.Insert(i, a[i]);
			}
			else
			{
			  b[i] = a[i];
			}
		  }

		  i = l;

		  for (j = (m + 1); j <= r; j++)
		  {

			// b[r+m+1-j] = a[j];
			if (r + m + 1 - j >= b.Count)
			{
			  b.Insert(r + m + 1 - j, a[j]);
			}
			else
			{
			  b[r + m + 1 - j] = a[j];
			}
		  }

		  j = r;

		  int compVal;

		  for (k = l; k <= r; k++)
		  {

			// if(b[i] < b[j])
			if (i == j)
			{
			  compVal = -1;
			}
			else
			{
			  compVal = compare((NodeCompareElem) b[i], (NodeCompareElem) b[j], 0, support);
			}

			if (compVal < 0)
			{

			  // a[k]=b[i];
			  a[k] = b[i];

			  i++;
			}
			else if (compVal > 0)
			{

			  // a[k]=b[j]; 
			  a[k] = b[j];

			  j--;
			}
		  }
		}
	  }

	  /// <summary>
	  /// This is a generic version of C.A.R Hoare's Quick Sort
	  /// algorithm.  This will handle arrays that are already
	  /// sorted, and arrays with duplicate keys.<BR>
	  /// 
	  /// If you think of a one dimensional array as going from
	  /// the lowest index on the left to the highest index on the right
	  /// then the parameters to this function are lowest index or
	  /// left and highest index or right.  The first time you call
	  /// this function it will be with the parameters 0, a.length - 1.
	  /// </summary>
	  /// <param name="v">       a vector of integers </param>
	  /// <param name="lo0">     left boundary of array partition </param>
	  /// <param name="hi0">     right boundary of array partition
	  ///  </param>

	  /*  private void QuickSort2(Vector v, int lo0, int hi0, XPathContext support)
	      throws javax.xml.transform.TransformerException,
	             java.net.MalformedURLException,
	             java.io.FileNotFoundException,
	             java.io.IOException
	    {
	      int lo = lo0;
	      int hi = hi0;
	
	      if ( hi0 > lo0)
	      {
	        // Arbitrarily establishing partition element as the midpoint of
	        // the array.
	        Node midNode = (Node)v.elementAt( ( lo0 + hi0 ) / 2 );
	
	        // loop through the array until indices cross
	        while( lo <= hi )
	        {
	          // find the first element that is greater than or equal to
	          // the partition element starting from the left Index.
	          while( (lo < hi0) && (compare((Node)v.elementAt(lo), midNode, 0, support) < 0) )
	          {
	            ++lo;
	          } // end while
	
	          // find an element that is smaller than or equal to
	          // the partition element starting from the right Index.
	          while( (hi > lo0) && (compare((Node)v.elementAt(hi), midNode, 0, support) > 0) )
	          {
	            --hi;
	          }
	
	          // if the indexes have not crossed, swap
	          if( lo <= hi )
	          {
	            swap(v, lo, hi);
	            ++lo;
	            --hi;
	          }
	        }
	
	        // If the right index has not reached the left side of array
	        // must now sort the left partition.
	        if( lo0 < hi )
	        {
	          QuickSort2( v, lo0, hi, support );
	        }
	
	        // If the left index has not reached the right side of array
	        // must now sort the right partition.
	        if( lo < hi0 )
	        {
	          QuickSort2( v, lo, hi0, support );
	        }
	      }
	    } // end QuickSort2  */

	//  /**
	//   * Simple function to swap two elements in
	//   * a vector.
	//   * 
	//   * @param v Vector of nodes to swap
	//   * @param i Index of first node to swap
	//   * @param i Index of second node to swap
	//   */
	//  private void swap(Vector v, int i, int j)
	//  {
	//
	//    int node = (Node) v.elementAt(i);
	//
	//    v.setElementAt(v.elementAt(j), i);
	//    v.setElementAt(node, j);
	//  }

	  /// <summary>
	  /// This class holds the value(s) from executing the given
	  /// node against the sort key(s). 
	  /// @xsl.usage internal
	  /// </summary>
	  internal class NodeCompareElem
	  {
		  private readonly NodeSorter outerInstance;


		/// <summary>
		/// Current node </summary>
		internal int m_node;

		/// <summary>
		/// This maxkey value was chosen arbitrarily. We are assuming that the    
		/// // maxkey + 1 keys will only hit fairly rarely and therefore, we
		/// // will get the node values for those keys dynamically.
		/// </summary>
		internal int maxkey = 2;

		// Keep this in case we decide to use an array. Right now
		// using two variables is cheaper.
		//Object[] m_KeyValue = new Object[2];

		/// <summary>
		/// Value from first sort key </summary>
		internal object m_key1Value;

		/// <summary>
		/// Value from second sort key </summary>
		internal object m_key2Value;

		/// <summary>
		/// Constructor NodeCompareElem
		/// 
		/// </summary>
		/// <param name="node"> Current node
		/// </param>
		/// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: NodeCompareElem(int node) throws javax.xml.transform.TransformerException
		internal NodeCompareElem(NodeSorter outerInstance, int node)
		{
			this.outerInstance = outerInstance;
		  m_node = node;

		  if (outerInstance.m_keys.Count > 0)
		  {
			NodeSortKey k1 = (NodeSortKey) outerInstance.m_keys[0];
			XObject r = k1.m_selectPat.execute(outerInstance.m_execContext, node, k1.m_namespaceContext);

			double d;

			if (k1.m_treatAsNumbers)
			{
			  d = r.num();

			  // Can't use NaN for compare. They are never equal. Use zero instead.  
			  m_key1Value = new double?(d);
			}
			else
			{
			  m_key1Value = k1.m_col.getCollationKey(r.str());
			}

			if (r.Type == XObject.CLASS_NODESET)
			{
			  // %REVIEW%
			  DTMIterator ni = ((XNodeSet)r).iterRaw();
			  int current = ni.CurrentNode;
			  if (DTM.NULL == current)
			  {
				current = ni.nextNode();
			  }

			  // if (ni instanceof ContextNodeList) // %REVIEW%
			  // tryNextKey = (DTM.NULL != current);

			  // else abdicate... should never happen, but... -sb
			}

			if (outerInstance.m_keys.Count > 1)
			{
			  NodeSortKey k2 = (NodeSortKey) outerInstance.m_keys[1];

			  XObject r2 = k2.m_selectPat.execute(outerInstance.m_execContext, node, k2.m_namespaceContext);

			  if (k2.m_treatAsNumbers)
			  {
				d = r2.num();
				m_key2Value = new double?(d);
			  }
			  else
			  {
				m_key2Value = k2.m_col.getCollationKey(r2.str());
			  }
			}

			/* Leave this in case we decide to use an array later
			while (kIndex <= m_keys.size() && kIndex < maxkey)
			{
			  NodeSortKey k = (NodeSortKey)m_keys.elementAt(kIndex);
			  XObject r = k.m_selectPat.execute(m_execContext, node, k.m_namespaceContext);
			  if(k.m_treatAsNumbers)
			    m_KeyValue[kIndex] = r.num();
			  else
			    m_KeyValue[kIndex] = r.str();
			} */
		  } // end if not empty
		}
	  } // end NodeCompareElem class
	}

}