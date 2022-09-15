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
 * $Id: ElemFallback.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;

	/// <summary>
	/// Implement xsl:fallback.
	/// <pre>
	/// <!ELEMENT xsl:fallback %template;>
	/// <!ATTLIST xsl:fallback %space-att;>
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.fallback">fallback in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemFallback : ElemTemplateElement
	{
		internal new const long serialVersionUID = 1782962139867340703L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_FALLBACK;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The Element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_FALLBACK_STRING;
		  }
	  }

	  /// <summary>
	  /// This is the normal call when xsl:fallback is instantiated.
	  /// In accordance with the XSLT 1.0 Recommendation, chapter 15,
	  /// "Normally, instantiating an xsl:fallback element does nothing."
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {
	  }

	  /// <summary>
	  /// Execute the fallback elements.  This must be explicitly called to
	  /// instantiate the content of an xsl:fallback element.
	  /// When an XSLT transformer performs fallback for an instruction
	  /// element, if the instruction element has one or more xsl:fallback
	  /// children, then the content of each of the xsl:fallback children
	  /// must be instantiated in sequence; otherwise, an error must
	  /// be signaled. The content of an xsl:fallback element is a template.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void executeFallback(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public virtual void executeFallback(TransformerImpl transformer)
	  {

		int parentElemType = m_parentNode.XSLToken;
		if (Constants.ELEMNAME_EXTENSIONCALL == parentElemType || Constants.ELEMNAME_UNDEFINED == parentElemType)
		{

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEvent(this);
		  }

		  transformer.executeChildTemplates(this, true);

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }
		}
		else
		{

		  // Should never happen
		  Console.WriteLine("Error!  parent of xsl:fallback must be an extension or unknown element!");
		}
	  }
	}

}