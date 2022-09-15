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
 * $Id: ElemUnknown.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPathContext = org.apache.xpath.XPathContext;


	/// <summary>
	/// Implement an unknown element
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class ElemUnknown : ElemLiteralResult
	{
		internal new const long serialVersionUID = -4573981712648730168L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_UNDEFINED;
		  }
	  }

	  /// <summary>
	  /// Execute the fallbacks when an extension is not available.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void executeFallbacks(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  private void executeFallbacks(TransformerImpl transformer)
	  {
		for (ElemTemplateElement child = m_firstChild; child != null; child = child.m_nextSibling)
		{
		  if (child.XSLToken == Constants.ELEMNAME_FALLBACK)
		  {
			try
			{
			  transformer.pushElemTemplateElement(child);
			  ((ElemFallback) child).executeFallback(transformer);
			}
			finally
			{
			  transformer.popElemTemplateElement();
			}
		  }
		}

	  }

	  /// <summary>
	  /// Return true if this extension element has a <xsl:fallback> child element.
	  /// </summary>
	  /// <returns> true if this extension element has a <xsl:fallback> child element. </returns>
	  private bool hasFallbackChildren()
	  {
		for (ElemTemplateElement child = m_firstChild; child != null; child = child.m_nextSibling)
		{
		  if (child.XSLToken == Constants.ELEMNAME_FALLBACK)
		  {
			return true;
		  }
		}

		return false;
	  }


	  /// <summary>
	  /// Execute an unknown element.
	  /// Execute fallback if fallback child exists or do nothing
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {


		if (transformer.Debug)
		{
			transformer.TraceManager.fireTraceEvent(this);
		}

		try
		{

			if (hasFallbackChildren())
			{
				executeFallbacks(transformer);
			}
			else
			{
				// do nothing
			}

		}
		catch (TransformerException e)
		{
			transformer.ErrorListener.fatalError(e);
		}
		if (transformer.Debug)
		{
			transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	}

}