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
 * $Id: AVTPartSimple.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{
	using FastStringBuffer = org.apache.xml.utils.FastStringBuffer;
	using XPathContext = org.apache.xpath.XPathContext;

	/// <summary>
	/// Simple string part of a complex AVT.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class AVTPartSimple : AVTPart
	{
		internal new const long serialVersionUID = -3744957690598727913L;

	  /// <summary>
	  /// Simple string value;
	  /// @serial
	  /// </summary>
	  private string m_val;

	  /// <summary>
	  /// Construct a simple AVT part. </summary>
	  /// <param name="val"> A pure string section of an AVT. </param>
	  public AVTPartSimple(string val)
	  {
		m_val = val;
	  }

	  /// <summary>
	  /// Get the AVT part as the original string.
	  /// </summary>
	  /// <returns> the AVT part as the original string. </returns>
	  public override string SimpleString
	  {
		  get
		  {
			return m_val;
		  }
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
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		// no-op
	  }


	  /// <summary>
	  /// Write the value into the buffer.
	  /// </summary>
	  /// <param name="xctxt"> An XPathContext object, providing infomation specific
	  /// to this invocation and this thread. Maintains SAX state, variables, 
	  /// error handler and  so on, so the transformation/XPath object itself
	  /// can be simultaneously invoked from multiple threads. </param>
	  /// <param name="buf"> Buffer to write into. </param>
	  /// <param name="context"> The current source tree context. </param>
	  /// <param name="nsNode"> The current namespace context (stylesheet tree context). </param>
	  public override void evaluate(XPathContext xctxt, FastStringBuffer buf, int context, org.apache.xml.utils.PrefixResolver nsNode)
	  {
		buf.append(m_val);
	  }
	  /// <seealso cref="XSLTVisitable.callVisitors(XSLTVisitor)"/>
	  public override void callVisitors(XSLTVisitor visitor)
	  {
		  // Don't do anything for the subpart for right now.
	  }

	}

}