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
 * $Id: ElemVariablePsuedo.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPath = org.apache.xpath.XPath;

	[Serializable]
	public class ElemVariablePsuedo : ElemVariable
	{
		internal new const long serialVersionUID = 692295692732588486L;
	  internal XUnresolvedVariableSimple m_lazyVar;

	  /// <summary>
	  /// Set the "select" attribute.
	  /// If the variable-binding element has a select attribute,
	  /// then the value of the attribute must be an expression and
	  /// the value of the variable is the object that results from
	  /// evaluating the expression. In this case, the content
	  /// of the variable must be empty.
	  /// </summary>
	  /// <param name="v"> Value to set for the "select" attribute. </param>
	  public override XPath Select
	  {
		  set
		  {
			base.Select = value;
			m_lazyVar = new XUnresolvedVariableSimple(this);
		  }
	  }

	  /// <summary>
	  /// Execute a variable declaration and push it onto the variable stack. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#variables">variables in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		// if (TransformerImpl.S_DEBUG)
		//  transformer.getTraceManager().fireTraceEvent(this);

		// transformer.getXPathContext().getVarStack().pushVariable(m_qname, var);
		transformer.XPathContext.VarStack.setLocalVariable(m_index, m_lazyVar);
	  }

	}


}