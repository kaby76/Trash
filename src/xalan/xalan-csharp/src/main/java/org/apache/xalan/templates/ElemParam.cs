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
 * $Id: ElemParam.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using VariableStack = org.apache.xpath.VariableStack;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Implement xsl:param.
	/// <pre>
	/// <!ELEMENT xsl:param %template;>
	/// <!ATTLIST xsl:param
	///   name %qname; #REQUIRED
	///   select %expr; #IMPLIED
	/// >
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.variables">variables in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemParam : ElemVariable
	{
		internal new const long serialVersionUID = -1131781475589006431L;
	  internal int m_qnameID;

	  /// <summary>
	  /// Constructor ElemParam
	  /// 
	  /// </summary>
	  public ElemParam()
	  {
	  }

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID of the element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_PARAMVARIABLE;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_PARAMVARIABLE_STRING;
		  }
	  }

	  /// <summary>
	  /// Copy constructor.
	  /// </summary>
	  /// <param name="param"> Element from an xsl:param
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public ElemParam(ElemParam param) throws javax.xml.transform.TransformerException
	  public ElemParam(ElemParam param) : base(param)
	  {
	  }

	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);
		m_qnameID = sroot.ComposeState.getQNameID(m_qname);
		int parentToken = m_parentNode.XSLToken;
		if (parentToken == Constants.ELEMNAME_TEMPLATE || parentToken == Constants.EXSLT_ELEMNAME_FUNCTION)
		{
		  ((ElemTemplate)m_parentNode).m_inArgsSize++;
		}
	  }

	  /// <summary>
	  /// Execute a variable declaration and push it onto the variable stack. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.variables">variables in XSLT Specification</a>"
	  ////>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {
		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		VariableStack vars = transformer.XPathContext.getVarStack();

		if (!vars.isLocalSet(m_index))
		{

		  int sourceNode = transformer.XPathContext.getCurrentNode();
		  XObject var = getValue(transformer, sourceNode);

		  // transformer.getXPathContext().getVarStack().pushVariable(m_qname, var);
		  transformer.XPathContext.getVarStack().setLocalVariable(m_index, var);
		}

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	}

}