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
 * $Id: ElemAttribute.java 469304 2006-10-30 22:29:47Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using NamespaceMappings = org.apache.xml.serializer.NamespaceMappings;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using QName = org.apache.xml.utils.QName;
	using XML11Char = org.apache.xml.utils.XML11Char;

	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Implement xsl:attribute.
	/// <pre>
	/// &amp;!ELEMENT xsl:attribute %char-template;>
	/// &amp;!ATTLIST xsl:attribute
	///   name %avt; #REQUIRED
	///   namespace %avt; #IMPLIED
	///   %space-att;
	/// &amp;
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.creating-attributes">creating-attributes in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemAttribute : ElemElement
	{
		internal new const long serialVersionUID = 8817220961566919187L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_ATTRIBUTE;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The element name  </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_ATTRIBUTE_STRING;
		  }
	  }

	  /// <summary>
	  /// Create an attribute in the result tree. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.creating-attributes">creating-attributes in XSLT Specification</a>"
	  ////>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
	//  public void execute(
	//          TransformerImpl transformer)
	//            throws TransformerException
	//  {
		//SerializationHandler rhandler = transformer.getSerializationHandler();

		// If they are trying to add an attribute when there isn't an 
		// element pending, it is an error.
		// I don't think we need this check here because it is checked in 
		// ResultTreeHandler.addAttribute.  (is)
	//    if (!rhandler.isElementPending())
	//    {
	//      // Make sure the trace event is sent.
	//      if (TransformerImpl.S_DEBUG)
	//        transformer.getTraceManager().fireTraceEvent(this);
	//
	//      XPathContext xctxt = transformer.getXPathContext();
	//      int sourceNode = xctxt.getCurrentNode();
	//      String attrName = m_name_avt.evaluate(xctxt, sourceNode, this);
	//      transformer.getMsgMgr().warn(this,
	//                                   XSLTErrorResources.WG_ILLEGAL_ATTRIBUTE_POSITION,
	//                                   new Object[]{ attrName });
	//
	//      if (TransformerImpl.S_DEBUG)
	//        transformer.getTraceManager().fireTraceEndEvent(this);
	//      return;
	//
	//      // warn(templateChild, sourceNode, "Trying to add attribute after element child has been added, ignoring...");
	//    }

	//    super.execute(transformer);

	//  }

	  /// <summary>
	  /// Resolve the namespace into a prefix.  At this level, if no prefix exists, 
	  /// then return a manufactured prefix.
	  /// </summary>
	  /// <param name="rhandler"> The current result tree handler. </param>
	  /// <param name="prefix"> The probable prefix if already known. </param>
	  /// <param name="nodeNamespace">  The namespace, which should not be null.
	  /// </param>
	  /// <returns> The prefix to be used. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected String resolvePrefix(org.apache.xml.serializer.SerializationHandler rhandler, String prefix, String nodeNamespace) throws javax.xml.transform.TransformerException
	  protected internal override string resolvePrefix(SerializationHandler rhandler, string prefix, string nodeNamespace)
	  {

		if (null != prefix && (prefix.Length == 0 || prefix.Equals("xmlns")))
		{
		  // Since we can't use default namespace, in this case we try and 
		  // see if a prefix has already been defined or this namespace.
		  prefix = rhandler.getPrefix(nodeNamespace);

		  // System.out.println("nsPrefix: "+nsPrefix);           
		  if (null == prefix || prefix.Length == 0 || prefix.Equals("xmlns"))
		  {
			if (nodeNamespace.Length > 0)
			{
				NamespaceMappings prefixMapping = rhandler.NamespaceMappings;
				prefix = prefixMapping.generateNextPrefix();
			}
			else
			{
			  prefix = "";
			}
		  }
		}
		return prefix;
	  }

	  /// <summary>
	  /// Validate that the node name is good.
	  /// </summary>
	  /// <param name="nodeName"> Name of the node being constructed, which may be null.
	  /// </param>
	  /// <returns> true if the node name is valid, false otherwise. </returns>
	   protected internal virtual bool validateNodeName(string nodeName)
	   {
		  if (null == nodeName)
		  {
			return false;
		  }
		  if (nodeName.Equals("xmlns"))
		  {
			return false;
		  }
		  return XML11Char.isXML11ValidQName(nodeName);
	   }

	  /// <summary>
	  /// Construct a node in the result tree.  This method is overloaded by 
	  /// xsl:attribute. At this class level, this method creates an element.
	  /// </summary>
	  /// <param name="nodeName"> The name of the node, which may be null. </param>
	  /// <param name="prefix"> The prefix for the namespace, which may be null. </param>
	  /// <param name="nodeNamespace"> The namespace of the node, which may be null. </param>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="sourceNode"> non-null reference to the <a href="http://www.w3.org/TR/xslt#dt-current-node">current source node</a>. </param>
	  /// <param name="mode"> reference, which may be null, to the <a href="http://www.w3.org/TR/xslt#modes">current mode</a>.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: void constructNode(String nodeName, String prefix, String nodeNamespace, org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  internal override void constructNode(string nodeName, string prefix, string nodeNamespace, TransformerImpl transformer)
	  {

		if (null != nodeName && nodeName.Length > 0)
		{
		  SerializationHandler rhandler = transformer.SerializationHandler;

		  // Evaluate the value of this attribute
		  string val = transformer.transformToString(this);
		  try
		  {
			// Let the result tree handler add the attribute and its String value.
			string localName = QName.getLocalPart(nodeName);
			if (!string.ReferenceEquals(prefix, null) && prefix.Length > 0)
			{
				rhandler.addAttribute(nodeNamespace, localName, nodeName, "CDATA", val, true);
			}
			else
			{
				rhandler.addAttribute("", localName, nodeName, "CDATA", val, true);
			}
		  }
		  catch (SAXException)
		  {
		  }
		}
	  }


	  /// <summary>
	  /// Add a child to the child list.
	  /// <!ELEMENT xsl:attribute %char-template;>
	  /// <!ATTLIST xsl:attribute
	  ///   name %avt; #REQUIRED
	  ///   namespace %avt; #IMPLIED
	  ///   %space-att;
	  /// >
	  /// </summary>
	  /// <param name="newChild"> Child to append to the list of this node's children
	  /// </param>
	  /// <returns> The node we just appended to the children list 
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		int type = ((ElemTemplateElement) newChild).XSLToken;

		switch (type)
		{

		// char-instructions 
		case Constants.ELEMNAME_TEXTLITERALRESULT :
		case Constants.ELEMNAME_APPLY_TEMPLATES :
		case Constants.ELEMNAME_APPLY_IMPORTS :
		case Constants.ELEMNAME_CALLTEMPLATE :
		case Constants.ELEMNAME_FOREACH :
		case Constants.ELEMNAME_VALUEOF :
		case Constants.ELEMNAME_COPY_OF :
		case Constants.ELEMNAME_NUMBER :
		case Constants.ELEMNAME_CHOOSE :
		case Constants.ELEMNAME_IF :
		case Constants.ELEMNAME_TEXT :
		case Constants.ELEMNAME_COPY :
		case Constants.ELEMNAME_VARIABLE :
		case Constants.ELEMNAME_MESSAGE :

		  // instructions 
		  // case Constants.ELEMNAME_PI:
		  // case Constants.ELEMNAME_COMMENT:
		  // case Constants.ELEMNAME_ELEMENT:
		  // case Constants.ELEMNAME_ATTRIBUTE:
		  break;
		default :
		  error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
	  break;
		}

		return base.appendChild(newChild);
	  }
		/// <seealso cref="ElemElement.setName(AVT)"/>
		public override AVT Name
		{
			set
			{
				if (value.Simple)
				{
					if (value.SimpleString.Equals("xmlns"))
					{
						throw new System.ArgumentException();
					}
				}
				base.Name = value;
			}
		}

	}

}