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
 * $Id: ElemElement.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using QName = org.apache.xml.utils.QName;
	using XML11Char = org.apache.xml.utils.XML11Char;
	using XPathContext = org.apache.xpath.XPathContext;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// Implement xsl:element
	/// <pre>
	/// <!ELEMENT xsl:element %template;>
	/// <!ATTLIST xsl:element
	///   name %avt; #REQUIRED
	///   namespace %avt; #IMPLIED
	///   use-attribute-sets %qnames; #IMPLIED
	///   %space-att;
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Creating-Elements-with-xsl:element">XXX in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemElement : ElemUse
	{
		internal new const long serialVersionUID = -324619535592435183L;

	  /// <summary>
	  /// The name attribute is interpreted as an attribute value template.
	  /// It is an error if the string that results from instantiating the
	  /// attribute value template is not a QName.
	  /// @serial
	  /// </summary>
	  protected internal AVT m_name_avt = null;

	  /// <summary>
	  /// Set the "name" attribute.
	  /// The name attribute is interpreted as an attribute value template.
	  /// It is an error if the string that results from instantiating the
	  /// attribute value template is not a QName.
	  /// </summary>
	  /// <param name="v"> Name attribute to set for this element </param>
	  public virtual AVT Name
	  {
		  set
		  {
			m_name_avt = value;
		  }
		  get
		  {
			return m_name_avt;
		  }
	  }


	  /// <summary>
	  /// If the namespace attribute is present, then it also is interpreted
	  /// as an attribute value template. The string that results from
	  /// instantiating the attribute value template should be a URI reference.
	  /// It is not an error if the string is not a syntactically legal URI reference.
	  /// @serial
	  /// </summary>
	  protected internal AVT m_namespace_avt = null;

	  /// <summary>
	  /// Set the "namespace" attribute.
	  /// If the namespace attribute is present, then it also is interpreted
	  /// as an attribute value template. The string that results from
	  /// instantiating the attribute value template should be a URI reference.
	  /// It is not an error if the string is not a syntactically legal URI reference.
	  /// </summary>
	  /// <param name="v"> NameSpace attribute to set for this element </param>
	  public virtual AVT Namespace
	  {
		  set
		  {
			m_namespace_avt = value;
		  }
		  get
		  {
			return m_namespace_avt;
		  }
	  }


	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public override void compose(StylesheetRoot sroot)
	  {
		base.compose(sroot);

		StylesheetRoot.ComposeState cstate = sroot.getComposeState();
		ArrayList vnames = cstate.VariableNames;
		if (null != m_name_avt)
		{
		  m_name_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
		if (null != m_namespace_avt)
		{
		  m_namespace_avt.fixupVariables(vnames, cstate.GlobalsSize);
		}
	  }


	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_ELEMENT;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> This element's name  </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_ELEMENT_STRING;
		  }
	  }

	  /// <summary>
	  /// Resolve the namespace into a prefix.  Meant to be
	  /// overidded by elemAttribute if this class is derived.
	  /// </summary>
	  /// <param name="rhandler"> The current result tree handler. </param>
	  /// <param name="prefix"> The probable prefix if already known. </param>
	  /// <param name="nodeNamespace">  The namespace.
	  /// </param>
	  /// <returns> The prefix to be used. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String resolvePrefix(org.apache.xml.serializer.SerializationHandler rhandler, String prefix, String nodeNamespace) throws javax.xml.transform.TransformerException
	  protected internal virtual string resolvePrefix(SerializationHandler rhandler, string prefix, string nodeNamespace)
	  {

	//    if (null != prefix && prefix.length() == 0)
	//    {
	//      String foundPrefix = rhandler.getPrefix(nodeNamespace);
	//
	//      // System.out.println("nsPrefix: "+nsPrefix);           
	//      if (null == foundPrefix)
	//        foundPrefix = "";
	//    }
		return prefix;
	  }

	  /// <summary>
	  /// Create an element in the result tree.
	  /// The xsl:element element allows an element to be created with a
	  /// computed name. The expanded-name of the element to be created
	  /// is specified by a required name attribute and an optional namespace
	  /// attribute. The content of the xsl:element element is a template
	  /// for the attributes and children of the created element.
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

		 SerializationHandler rhandler = transformer.SerializationHandler;
		XPathContext xctxt = transformer.XPathContext;
		int sourceNode = xctxt.CurrentNode;


		string nodeName = m_name_avt == null ? null : m_name_avt.evaluate(xctxt, sourceNode, this);

		string prefix = null;
		string nodeNamespace = "";

		// Only validate if an AVT was used.
		if ((!string.ReferenceEquals(nodeName, null)) && (!m_name_avt.Simple) && (!XML11Char.isXML11ValidQName(nodeName)))
		{
		  transformer.MsgMgr.warn(this, XSLTErrorResources.WG_ILLEGAL_ATTRIBUTE_VALUE, new object[]{Constants.ATTRNAME_NAME, nodeName});

		  nodeName = null;
		}

		else if (!string.ReferenceEquals(nodeName, null))
		{
		  prefix = QName.getPrefixPart(nodeName);

		  if (null != m_namespace_avt)
		  {
			nodeNamespace = m_namespace_avt.evaluate(xctxt, sourceNode, this);
			if (null == nodeNamespace || (!string.ReferenceEquals(prefix, null) && prefix.Length > 0 && nodeNamespace.Length == 0))
			{
			  transformer.MsgMgr.error(this, XSLTErrorResources.ER_NULL_URI_NAMESPACE);
			}
			else
			{
			// Determine the actual prefix that we will use for this nodeNamespace

			prefix = resolvePrefix(rhandler, prefix, nodeNamespace);
			if (null == prefix)
			{
			  prefix = "";
			}

			if (prefix.Length > 0)
			{
			  nodeName = (prefix + ":" + QName.getLocalPart(nodeName));
			}
			else
			{
			  nodeName = QName.getLocalPart(nodeName);
			}
			}
		  }

		  // No namespace attribute was supplied. Use the namespace declarations
		  // currently in effect for the xsl:element element.
		  else
		  {
			try
			{
			  // Maybe temporary, until I get this worked out.  test: axes59
			  nodeNamespace = getNamespaceForPrefix(prefix);

			  // If we get back a null nodeNamespace, that means that this prefix could
			  // not be found in the table.  This is okay only for a default namespace
			  // that has never been declared.

			  if ((null == nodeNamespace) && (prefix.Length == 0))
			  {
				nodeNamespace = "";
			  }
			  else if (null == nodeNamespace)
			  {
				transformer.MsgMgr.warn(this, XSLTErrorResources.WG_COULD_NOT_RESOLVE_PREFIX, new object[]{prefix});

				nodeName = null;
			  }

			}
			catch (Exception)
			{
			  transformer.MsgMgr.warn(this, XSLTErrorResources.WG_COULD_NOT_RESOLVE_PREFIX, new object[]{prefix});

			  nodeName = null;
			}
		  }
		}

		constructNode(nodeName, prefix, nodeNamespace, transformer);

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }

	  /// <summary>
	  /// Construct a node in the result tree.  This method is overloaded by 
	  /// xsl:attribute. At this class level, this method creates an element.
	  /// If the node is null, we instantiate only the content of the node in accordance
	  /// with section 7.1.2 of the XSLT 1.0 Recommendation.
	  /// </summary>
	  /// <param name="nodeName"> The name of the node, which may be <code>null</code>.  If <code>null</code>,
	  ///                 only the non-attribute children of this node will be processed. </param>
	  /// <param name="prefix"> The prefix for the namespace, which may be <code>null</code>.
	  ///               If not <code>null</code>, this prefix will be mapped and unmapped. </param>
	  /// <param name="nodeNamespace"> The namespace of the node, which may be not be <code>null</code>. </param>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void constructNode(String nodeName, String prefix, String nodeNamespace, org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  internal virtual void constructNode(string nodeName, string prefix, string nodeNamespace, TransformerImpl transformer)
	  {

		bool shouldAddAttrs;

		try
		{
		  SerializationHandler rhandler = transformer.ResultTreeHandler;

		  if (null == nodeName)
		  {
			shouldAddAttrs = false;
		  }
		  else
		  {
			if (null != prefix)
			{
			  rhandler.startPrefixMapping(prefix, nodeNamespace, true);
			}

			rhandler.startElement(nodeNamespace, QName.getLocalPart(nodeName), nodeName);

			base.execute(transformer);

			shouldAddAttrs = true;
		  }

		  transformer.executeChildTemplates(this, shouldAddAttrs);

		  // Now end the element if name was valid
		  if (null != nodeName)
		  {
			rhandler.endElement(nodeNamespace, QName.getLocalPart(nodeName), nodeName);
			if (null != prefix)
			{
			  rhandler.endPrefixMapping(prefix);
			}
		  }
		}
		catch (SAXException se)
		{
		  throw new TransformerException(se);
		}
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
	  {
		  if (callAttrs)
		  {
			if (null != m_name_avt)
			{
			  m_name_avt.callVisitors(visitor);
			}

			if (null != m_namespace_avt)
			{
			  m_namespace_avt.callVisitors(visitor);
			}
		  }

		base.callChildVisitors(visitor, callAttrs);
	  }

	}

}