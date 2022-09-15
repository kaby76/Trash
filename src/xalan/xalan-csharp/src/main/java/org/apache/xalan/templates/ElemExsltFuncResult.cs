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
 * $Id: ElemExsltFuncResult.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Handles the EXSLT result element within an EXSLT function element.
	/// </summary>
	[Serializable]
	public class ElemExsltFuncResult : ElemVariable
	{
		internal new const long serialVersionUID = -3478311949388304563L;
		/*
		 * To keep the binary compatibility put those three private global 
		 * variables back, although they are never used in this verison
		 */
		// A flag indicating whether the return result is set
		private bool m_isResultSet = false;

		// The return result
		private XObject m_result = null;

		// The frame size of the current caller
		private int m_callerFrameSize = 0;

	  /// <summary>
	  /// Generate the EXSLT function return value, and assign it to the variable
	  /// index slot assigned for it in ElemExsltFunction compose().
	  /// 
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {
		XPathContext context = transformer.XPathContext;

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		// Verify that result has not already been set by another result
		// element. Recursion is allowed: intermediate results are cleared 
		// in the owner ElemExsltFunction execute().
		if (transformer.currentFuncResultSeen())
		{
			throw new TransformerException("An EXSLT function cannot set more than one result!");
		}

		int sourceNode = context.CurrentNode;

		// Set the return value;
		XObject var = getValue(transformer, sourceNode);
		transformer.popCurrentFuncResult();
		transformer.pushCurrentFuncResult(var);

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	  /// <summary>
	  /// Get an integer representation of the element type.
	  /// </summary>
	  /// <returns> An integer representation of the element, defined in the
	  ///     Constants class. </returns>
	  /// <seealso cref="org.apache.xalan.templates.Constants"/>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.EXSLT_ELEMNAME_FUNCRESULT;
		  }
	  }

	  /// <summary>
	  /// Return the node name, defined in the
	  ///     Constants class. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"/>
	  /// <returns> The node name
	  ///  </returns>
	   public override string NodeName
	   {
		   get
		   {
			return Constants.EXSLT_ELEMNAME_FUNCRESULT_STRING;
		   }
	   }
	}

}