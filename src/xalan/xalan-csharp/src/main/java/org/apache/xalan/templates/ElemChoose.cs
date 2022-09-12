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
 * $Id: ElemChoose.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Implement xsl:choose.
	/// <pre>
	/// <!ELEMENT xsl:choose (xsl:when+, xsl:otherwise?)>
	/// <!ATTLIST xsl:choose %space-att;>
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Conditional-Processing-with-xsl:choose">XXX in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemChoose : ElemTemplateElement
	{
		internal new const long serialVersionUID = -3070117361903102033L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_CHOOSE;
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
			return Constants.ELEMNAME_CHOOSE_STRING;
		  }
	  }

	  /// <summary>
	  /// Constructor ElemChoose
	  /// 
	  /// </summary>
	  public ElemChoose()
	  {
	  }

	  /// <summary>
	  /// Execute the xsl:choose transformation.
	  /// 
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

		bool found = false;

		for (ElemTemplateElement childElem = FirstChildElem; childElem != null; childElem = childElem.NextSiblingElem)
		{
		  int type = childElem.XSLToken;

		  if (Constants.ELEMNAME_WHEN == type)
		  {
			found = true;

			ElemWhen when = (ElemWhen) childElem;

			// must be xsl:when
			XPathContext xctxt = transformer.XPathContext;
			int sourceNode = xctxt.CurrentNode;

			// System.err.println("\""+when.getTest().getPatternString()+"\"");

			// if(when.getTest().getPatternString().equals("COLLECTION/icuser/ictimezone/LITERAL='GMT +13:00 Pacific/Tongatapu'"))
			// 	System.err.println("Found COLLECTION/icuser/ictimezone/LITERAL");

			if (transformer.Debug)
			{
			  XObject test = when.Test.execute(xctxt, sourceNode, when);

			  if (transformer.Debug)
			  {
				transformer.TraceManager.fireSelectedEvent(sourceNode, when, "test", when.Test, test);
			  }

			  if (test.@bool())
			  {
				transformer.TraceManager.fireTraceEvent(when);

				transformer.executeChildTemplates(when, true);

				transformer.TraceManager.fireTraceEndEvent(when);

				return;
			  }

			}
			else if (when.Test.@bool(xctxt, sourceNode, when))
			{
			  transformer.executeChildTemplates(when, true);

			  return;
			}
		  }
		  else if (Constants.ELEMNAME_OTHERWISE == type)
		  {
			found = true;

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEvent(childElem);
			}

			// xsl:otherwise                
			transformer.executeChildTemplates(childElem, true);

			if (transformer.Debug)
			{
			  transformer.TraceManager.fireTraceEndEvent(childElem);
			}
			return;
		  }
		}

		if (!found)
		{
		  transformer.MsgMgr.error(this, XSLTErrorResources.ER_CHOOSE_REQUIRES_WHEN);
		}

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// </summary>
	  /// <param name="newChild"> Child to add to this node's child list
	  /// </param>
	  /// <returns> The child that was just added to the child list
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		int type = ((ElemTemplateElement) newChild).XSLToken;

		switch (type)
		{
		case Constants.ELEMNAME_WHEN :
		case Constants.ELEMNAME_OTHERWISE :

		  // TODO: Positional checking
		  break;
		default :
		  error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
	  break;
		}

		return base.appendChild(newChild);
	  }

	  /// <summary>
	  /// Tell if this element can accept variable declarations. </summary>
	  /// <returns> true if the element can accept and process variable declarations. </returns>
	  public override bool canAcceptVariables()
	  {
		  return false;
	  }

	}

}