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
 * $Id: ElemCopyOf.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using TreeWalker2Result = org.apache.xalan.transformer.TreeWalker2Result;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMTreeWalker = org.apache.xml.dtm.@ref.DTMTreeWalker;
	using SerializerUtils = org.apache.xalan.serialize.SerializerUtils;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using XPath = org.apache.xpath.XPath;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Implement xsl:copy-of.
	/// <pre>
	/// <!ELEMENT xsl:copy-of EMPTY>
	/// <!ATTLIST xsl:copy-of select %expr; #REQUIRED>
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#copy-of">copy-of in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemCopyOf : ElemTemplateElement
	{
		internal new const long serialVersionUID = -7433828829497411127L;

	  /// <summary>
	  /// The required select attribute contains an expression.
	  /// @serial
	  /// </summary>
	  public XPath m_selectExpression = null;

	  /// <summary>
	  /// Set the "select" attribute.
	  /// The required select attribute contains an expression.
	  /// </summary>
	  /// <param name="expr"> Expression for select attribute  </param>
	  public virtual XPath Select
	  {
		  set
		  {
			m_selectExpression = value;
		  }
		  get
		  {
			return m_selectExpression;
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
		m_selectExpression.fixupVariables(cstate.VariableNames, cstate.GlobalsSize);
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
			return Constants.ELEMNAME_COPY_OF;
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
			return Constants.ELEMNAME_COPY_OF_STRING;
		  }
	  }

	  /// <summary>
	  /// The xsl:copy-of element can be used to insert a result tree
	  /// fragment into the result tree, without first converting it to
	  /// a string as xsl:value-of does (see [7.6.1 Generating Text with
	  /// xsl:value-of]).
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
		  XPathContext xctxt = transformer.XPathContext;
		  int sourceNode = xctxt.CurrentNode;
		  XObject value = m_selectExpression.execute(xctxt, sourceNode, this);

		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireSelectedEvent(sourceNode, this, "select", m_selectExpression, value);
		  }

		  SerializationHandler handler = transformer.SerializationHandler;

		  if (null != value)
		  {
			int type = value.Type;
			string s;

			switch (type)
			{
			case XObject.CLASS_BOOLEAN :
			case XObject.CLASS_NUMBER :
			case XObject.CLASS_STRING :
			  s = value.str();

			  handler.characters(s.ToCharArray(), 0, s.Length);
			  break;
			case XObject.CLASS_NODESET :

			  // System.out.println(value);
			  DTMIterator nl = value.iter();

			  // Copy the tree.
			  DTMTreeWalker tw = new TreeWalker2Result(transformer, handler);
			  int pos;

			  while (org.apache.xml.dtm.DTM_Fields.NULL != (pos = nl.nextNode()))
			  {
				DTM dtm = xctxt.DTMManager.getDTM(pos);
				short t = dtm.getNodeType(pos);

				// If we just copy the whole document, a startDoc and endDoc get 
				// generated, so we need to only walk the child nodes.
				if (t == org.apache.xml.dtm.DTM_Fields.DOCUMENT_NODE)
				{
				  for (int child = dtm.getFirstChild(pos); child != org.apache.xml.dtm.DTM_Fields.NULL; child = dtm.getNextSibling(child))
				  {
					tw.traverse(child);
				  }
				}
				else if (t == org.apache.xml.dtm.DTM_Fields.ATTRIBUTE_NODE)
				{
				  SerializerUtils.addAttribute(handler, pos);
				}
				else
				{
				  tw.traverse(pos);
				}
			  }
			  // nl.detach();
			  break;
			case XObject.CLASS_RTREEFRAG :
			  SerializerUtils.outputResultTreeFragment(handler, value, transformer.XPathContext);
			  break;
			default :

			  s = value.str();

			  handler.characters(s.ToCharArray(), 0, s.Length);
			  break;
			}
		  }

		  // I don't think we want this.  -sb
		  //  if (transformer.getDebug())
		  //  transformer.getTraceManager().fireSelectedEvent(sourceNode, this,
		  //  "endSelect", m_selectExpression, value);

		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}
		finally
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireTraceEndEvent(this);
		  }
		}

	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// </summary>
	  /// <param name="newChild"> Child to add to this node's child list
	  /// </param>
	  /// <returns> Child just added to child list </returns>
	  public override ElemTemplateElement appendChild(ElemTemplateElement newChild)
	  {

		error(XSLTErrorResources.ER_CANNOT_ADD, new object[]{newChild.NodeName, this.NodeName}); //"Can not add " +((ElemTemplateElement)newChild).m_elemName +

		//" to " + this.m_elemName);
		return null;
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal override void callChildVisitors(XSLTVisitor visitor, bool callAttrs)
	  {
		  if (callAttrs)
		  {
			  m_selectExpression.Expression.callVisitors(m_selectExpression, visitor);
		  }
		base.callChildVisitors(visitor, callAttrs);
	  }

	}

}