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
 * $Id: VariableSafeAbsRef.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.operations
{

	using DTMManager = org.apache.xml.dtm.DTMManager;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;


	/// <summary>
	/// This is a "smart" variable reference that is used in situations where 
	/// an absolute path is optimized into a variable reference, but may 
	/// be used in some situations where the document context may have changed.
	/// For instance, in select="document(doc/@href)//name[//salary &gt; 7250]", the 
	/// root in the predicate will be different for each node in the set.  While 
	/// this is easy to detect statically in this case, in other cases static 
	/// detection would be very hard or impossible.  So, this class does a dynamic check 
	/// to make sure the document context of the referenced variable is the same as 
	/// the current document context, and, if it is not, execute the referenced variable's 
	/// expression with the current context instead.
	/// </summary>
	[Serializable]
	public class VariableSafeAbsRef : Variable
	{
		internal new const long serialVersionUID = -9174661990819967452L;

	  /// <summary>
	  /// Dereference the variable, and return the reference value.  Note that lazy 
	  /// evaluation will occur.  If a variable within scope is not found, a warning 
	  /// will be sent to the error listener, and an empty nodeset will be returned.
	  /// 
	  /// </summary>
	  /// <param name="xctxt"> The runtime execution context.
	  /// </param>
	  /// <returns> The evaluated variable, or an empty nodeset if not found.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt, boolean destructiveOK) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt, bool destructiveOK)
	  {
		  XNodeSet xns = (XNodeSet)base.execute(xctxt, destructiveOK);
		  DTMManager dtmMgr = xctxt.DTMManager;
		  int context = xctxt.ContextNode;
		  if (dtmMgr.getDTM(xns.Root).Document != dtmMgr.getDTM(context).Document)
		  {
			  Expression expr = (Expression)xns.ContainedIter;
			  xns = (XNodeSet)expr.asIterator(xctxt, context);
		  }
		  return xns;
	  }

	}


}