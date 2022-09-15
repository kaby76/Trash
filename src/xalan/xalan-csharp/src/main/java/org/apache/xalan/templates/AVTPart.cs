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
 * $Id: AVTPart.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Class to hold a part, either a string or XPath,
	/// of an Attribute Value Template.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public abstract class AVTPart : XSLTVisitable
	{
		public abstract void callVisitors(XSLTVisitor visitor);
		internal const long serialVersionUID = -1747749903613916025L;

	  /// <summary>
	  /// Construct a part.
	  /// </summary>
	  public AVTPart()
	  {
	  }

	  /// <summary>
	  /// Get the AVT part as the original string.
	  /// </summary>
	  /// <returns> the AVT part as the original string. </returns>
	  public abstract string SimpleString {get;}

	  /// <summary>
	  /// Write the evaluated value into the given
	  /// string buffer.
	  /// </summary>
	  /// <param name="xctxt"> The XPath context to use to evaluate this AVT. </param>
	  /// <param name="buf"> Buffer to write into. </param>
	  /// <param name="context"> The current source tree context. </param>
	  /// <param name="nsNode"> The current namespace context (stylesheet tree context).
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void evaluate(org.apache.xpath.XPathContext xctxt, org.apache.xml.utils.FastStringBuffer buf, int context, org.apache.xml.utils.PrefixResolver nsNode) throws javax.xml.transform.TransformerException;
	  public abstract void evaluate(XPathContext xctxt, FastStringBuffer buf, int context, org.apache.xml.utils.PrefixResolver nsNode);

	  /// <summary>
	  /// Set the XPath support.
	  /// </summary>
	  /// <param name="support"> XPathContext to set.  </param>
	  public virtual XPathContext XPathSupport
	  {
		  set
		  {
		  }
	  }

	  /// <summary>
	  /// Tell if this expression or it's subexpressions can traverse outside 
	  /// the current subtree.
	  /// </summary>
	  /// <returns> true if traversal outside the context node's subtree can occur. </returns>
	   public virtual bool canTraverseOutsideSubtree()
	   {
		return false;
	   }

	  /// <summary>
	  /// This function is used to fixup variables from QNames to stack frame 
	  /// indexes at stylesheet build time. </summary>
	  /// <param name="vars"> List of QNames that correspond to variables.  This list 
	  /// should be searched backwards for the first qualified name that 
	  /// corresponds to the variable reference qname.  The position of the 
	  /// QName in the vector from the start of the vector will be its position 
	  /// in the stack frame (but variables above the globalsTop value will need 
	  /// to be offset to the current stack frame). </param>
	  public abstract void fixupVariables(ArrayList vars, int globalsSize);


	}

}