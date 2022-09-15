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
 * $Id: ElemTemplateElement.java 475981 2006-11-16 23:35:53Z minchau $
 */
namespace org.apache.xalan.templates
{



	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using UnImplNode = org.apache.xml.utils.UnImplNode;
	using ExpressionNode = org.apache.xpath.ExpressionNode;
	using WhitespaceStrippingElementMatcher = org.apache.xpath.WhitespaceStrippingElementMatcher;

	using DOMException = org.w3c.dom.DOMException;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	using NamespaceSupport = org.xml.sax.helpers.NamespaceSupport;

	/// <summary>
	/// An instance of this class represents an element inside
	/// an xsl:template class.  It has a single "execute" method
	/// which is expected to perform the given action on the
	/// result tree.
	/// This class acts like a Element node, and implements the
	/// Element interface, but is not a full implementation
	/// of that interface... it only implements enough for
	/// basic traversal of the tree.
	/// </summary>
	/// <seealso cref= Stylesheet
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemTemplateElement : UnImplNode, PrefixResolver, ExpressionNode, WhitespaceStrippingElementMatcher, XSLTVisitable
	{
		internal const long serialVersionUID = 4440018597841834447L;

	  /// <summary>
	  /// Construct a template element instance.
	  /// 
	  /// </summary>
	  public ElemTemplateElement()
	  {
	  }

	  /// <summary>
	  /// Tell if this template is a compiled template.
	  /// </summary>
	  /// <returns> Boolean flag indicating whether this is a compiled template    </returns>
	  public virtual bool CompiledTemplate
	  {
		  get
		  {
			return false;
		  }
	  }

	  /// <summary>
	  /// Get an integer representation of the element type.
	  /// </summary>
	  /// <returns> An integer representation of the element, defined in the
	  ///     Constants class. </returns>
	  /// <seealso cref= org.apache.xalan.templates.Constants </seealso>
	  public virtual int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_UNDEFINED;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> An invalid node name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return "Unknown XSLT Element";
		  }
	  }

	  /// <summary>
	  /// For now, just return the result of getNodeName(), which 
	  /// the local name.
	  /// </summary>
	  /// <returns> The result of getNodeName(). </returns>
	  public override string LocalName
	  {
		  get
		  {
    
			return NodeName;
		  }
	  }


	  /// <summary>
	  /// This function will be called on top-level elements
	  /// only, just before the transform begins.
	  /// </summary>
	  /// <param name="transformer"> The XSLT TransformerFactory.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void runtimeInit(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public virtual void runtimeInit(TransformerImpl transformer)
	  {
	  }

	  /// <summary>
	  /// Execute the element's primary function.  Subclasses of this
	  /// function may recursivly execute down the element tree.
	  /// </summary>
	  /// <param name="transformer"> The XSLT TransformerFactory.
	  /// </param>
	  /// <exception cref="TransformerException"> if any checked exception occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public virtual void execute(TransformerImpl transformer)
	  {
	  }

	  /// <summary>
	  /// Get the owning "composed" stylesheet.  This looks up the
	  /// inheritance chain until it calls getStylesheetComposed
	  /// on a Stylesheet object, which will Get the owning
	  /// aggregated stylesheet, or that stylesheet if it is aggregated.
	  /// </summary>
	  /// <returns> the owning "composed" stylesheet. </returns>
	  public virtual StylesheetComposed StylesheetComposed
	  {
		  get
		  {
			return m_parentNode.StylesheetComposed;
		  }
	  }

	  /// <summary>
	  /// Get the owning stylesheet.  This looks up the
	  /// inheritance chain until it calls getStylesheet
	  /// on a Stylesheet object, which will return itself.
	  /// </summary>
	  /// <returns> the owning stylesheet </returns>
	  public virtual Stylesheet Stylesheet
	  {
		  get
		  {
			return (null == m_parentNode) ? null : m_parentNode.Stylesheet;
		  }
	  }

	  /// <summary>
	  /// Get the owning root stylesheet.  This looks up the
	  /// inheritance chain until it calls StylesheetRoot
	  /// on a Stylesheet object, which will return a reference
	  /// to the root stylesheet.
	  /// </summary>
	  /// <returns> the owning root stylesheet </returns>
	  public virtual StylesheetRoot StylesheetRoot
	  {
		  get
		  {
			return m_parentNode.StylesheetRoot;
		  }
	  }

	  /// <summary>
	  /// This function is called during recomposition to
	  /// control how this element is composed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void recompose(StylesheetRoot root) throws javax.xml.transform.TransformerException
	  public virtual void recompose(StylesheetRoot root)
	  {
	  }

	  /// <summary>
	  /// This function is called after everything else has been
	  /// recomposed, and allows the template to set remaining
	  /// values that may be based on some other property that
	  /// depends on recomposition.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void compose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public virtual void compose(StylesheetRoot sroot)
	  {
		resolvePrefixTables();
		ElemTemplateElement t = FirstChildElem;
		m_hasTextLitOnly = ((t != null) && (t.XSLToken == Constants.ELEMNAME_TEXTLITERALRESULT) && (t.NextSiblingElem == null));

		StylesheetRoot.ComposeState cstate = sroot.getComposeState();
		cstate.pushStackMark();
	  }

	  /// <summary>
	  /// This after the template's children have been composed.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endCompose(StylesheetRoot sroot) throws javax.xml.transform.TransformerException
	  public virtual void endCompose(StylesheetRoot sroot)
	  {
		StylesheetRoot.ComposeState cstate = sroot.getComposeState();
		cstate.popStackMark();
	  }

	  /// <summary>
	  /// Throw a template element runtime error.  (Note: should we throw a TransformerException instead?)
	  /// </summary>
	  /// <param name="msg"> key of the error that occured. </param>
	  /// <param name="args"> Arguments to be used in the message </param>
	  public override void error(string msg, object[] args)
	  {

		string themsg = XSLMessages.createMessage(msg, args);

		throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_ELEMTEMPLATEELEM_ERR, new object[]{themsg}));
	  }

	  /*
	   * Throw an error.
	   *
	   * @param msg Message key for the error
	   *
	   */
	  public override void error(string msg)
	  {
		error(msg, null);
	  }


	  // Implemented DOM Element methods.
	  /// <summary>
	  /// Add a child to the child list.
	  /// NOTE: This presumes the child did not previously have a parent.
	  /// Making that assumption makes this a less expensive operation -- but
	  /// requires that if you *do* want to reparent a node, you use removeChild()
	  /// first to remove it from its previous context. Failing to do so will
	  /// damage the tree.
	  /// </summary>
	  /// <param name="newChild"> Child to be added to child list
	  /// </param>
	  /// <returns> Child just added to the child list </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node appendChild(org.w3c.dom.Node newChild) throws org.w3c.dom.DOMException
	  public override Node appendChild(Node newChild)
	  {

		if (null == newChild)
		{
		  error(XSLTErrorResources.ER_NULL_CHILD, null); //"Trying to add a null child!");
		}

		ElemTemplateElement elem = (ElemTemplateElement) newChild;

		if (null == m_firstChild)
		{
		  m_firstChild = elem;
		}
		else
		{
		  ElemTemplateElement last = (ElemTemplateElement) LastChild;

		  last.m_nextSibling = elem;
		}

		elem.m_parentNode = this;

		return newChild;
	  }

	  /// <summary>
	  /// Add a child to the child list.
	  /// NOTE: This presumes the child did not previously have a parent.
	  /// Making that assumption makes this a less expensive operation -- but
	  /// requires that if you *do* want to reparent a node, you use removeChild()
	  /// first to remove it from its previous context. Failing to do so will
	  /// damage the tree.
	  /// </summary>
	  /// <param name="elem"> Child to be added to child list
	  /// </param>
	  /// <returns> Child just added to the child list </returns>
	  public virtual ElemTemplateElement appendChild(ElemTemplateElement elem)
	  {

		if (null == elem)
		{
		  error(XSLTErrorResources.ER_NULL_CHILD, null); //"Trying to add a null child!");
		}

		if (null == m_firstChild)
		{
		  m_firstChild = elem;
		}
		else
		{
		  ElemTemplateElement last = LastChildElem;

		  last.m_nextSibling = elem;
		}

		elem.ParentElem = this;

		return elem;
	  }


	  /// <summary>
	  /// Tell if there are child nodes.
	  /// </summary>
	  /// <returns> True if there are child nodes </returns>
	  public override bool hasChildNodes()
	  {
		return (null != m_firstChild);
	  }

	  /// <summary>
	  /// Get the type of the node.
	  /// </summary>
	  /// <returns> Constant for this node type </returns>
	  public override short NodeType
	  {
		  get
		  {
			return Node.ELEMENT_NODE;
		  }
	  }

	  /// <summary>
	  /// Return the nodelist (same reference).
	  /// </summary>
	  /// <returns> The nodelist containing the child nodes (this) </returns>
	  public override NodeList ChildNodes
	  {
		  get
		  {
			return this;
		  }
	  }

	  /// <summary>
	  /// Remove a child.
	  /// ADDED 9/8/200 to support compilation.
	  /// TODO: ***** Alternative is "removeMe() from my parent if any"
	  /// ... which is less well checked, but more convenient in some cases.
	  /// Given that we assume only experts are calling this class, it might
	  /// be preferable. It's less DOMish, though.
	  /// </summary>
	  /// <param name="childETE"> The child to remove. This operation is a no-op
	  /// if oldChild is not a child of this node.
	  /// </param>
	  /// <returns> the removed child, or null if the specified
	  /// node was not a child of this element. </returns>
	  public virtual ElemTemplateElement removeChild(ElemTemplateElement childETE)
	  {

		if (childETE == null || childETE.m_parentNode != this)
		{
		  return null;
		}

		// Pointers to the child
		if (childETE == m_firstChild)
		{
		  m_firstChild = childETE.m_nextSibling;
		}
		else
		{
		  ElemTemplateElement prev = childETE.PreviousSiblingElem;

		  prev.m_nextSibling = childETE.m_nextSibling;
		}

		// Pointers from the child
		childETE.m_parentNode = null;
		childETE.m_nextSibling = null;

		return childETE;
	  }

	  /// <summary>
	  /// Replace the old child with a new child.
	  /// </summary>
	  /// <param name="newChild"> New child to replace with </param>
	  /// <param name="oldChild"> Old child to be replaced
	  /// </param>
	  /// <returns> The new child
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node replaceChild(org.w3c.dom.Node newChild, org.w3c.dom.Node oldChild) throws org.w3c.dom.DOMException
	  public override Node replaceChild(Node newChild, Node oldChild)
	  {

		if (oldChild == null || oldChild.ParentNode != this)
		{
		  return null;
		}

		ElemTemplateElement newChildElem = ((ElemTemplateElement) newChild);
		ElemTemplateElement oldChildElem = ((ElemTemplateElement) oldChild);

		// Fix up previous sibling.
		ElemTemplateElement prev = (ElemTemplateElement) oldChildElem.PreviousSibling;

		if (null != prev)
		{
		  prev.m_nextSibling = newChildElem;
		}

		// Fix up parent (this)
		if (m_firstChild == oldChildElem)
		{
		  m_firstChild = newChildElem;
		}

		newChildElem.m_parentNode = this;
		oldChildElem.m_parentNode = null;
		newChildElem.m_nextSibling = oldChildElem.m_nextSibling;
		oldChildElem.m_nextSibling = null;

		// newChildElem.m_stylesheet = oldChildElem.m_stylesheet;
		// oldChildElem.m_stylesheet = null;
		return newChildElem;
	  }

	  /// <summary>
	  /// Unimplemented. See org.w3c.dom.Node
	  /// </summary>
	  /// <param name="newChild"> New child node to insert </param>
	  /// <param name="refChild"> Insert in front of this child
	  /// </param>
	  /// <returns> null
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.w3c.dom.Node insertBefore(org.w3c.dom.Node newChild, org.w3c.dom.Node refChild) throws org.w3c.dom.DOMException
	  public override Node insertBefore(Node newChild, Node refChild)
	  {
		  if (null == refChild)
		  {
			  appendChild(newChild);
			  return newChild;
		  }

		  if (newChild == refChild)
		  {
			  // hmm...
			  return newChild;
		  }

		Node node = m_firstChild;
		Node prev = null;
		bool foundit = false;

		while (null != node)
		{
			// If the newChild is already in the tree, it is first removed.
			if (newChild == node)
			{
				if (null != prev)
				{
					((ElemTemplateElement)prev).m_nextSibling = (ElemTemplateElement)node.NextSibling;
				}
				else
				{
					m_firstChild = (ElemTemplateElement)node.NextSibling;
				}
				node = node.NextSibling;
				continue; // prev remains the same.
			}
			if (refChild == node)
			{
				if (null != prev)
				{
					((ElemTemplateElement)prev).m_nextSibling = (ElemTemplateElement)newChild;
				}
				else
				{
					m_firstChild = (ElemTemplateElement)newChild;
				}
				((ElemTemplateElement)newChild).m_nextSibling = (ElemTemplateElement)refChild;
				((ElemTemplateElement)newChild).ParentElem = this;
				prev = newChild;
				node = node.NextSibling;
				foundit = true;
				continue;
			}
			prev = node;
			node = node.NextSibling;
		}

		if (!foundit)
		{
			throw new DOMException(DOMException.NOT_FOUND_ERR, "refChild was not found in insertBefore method!");
		}
		else
		{
			return newChild;
		}
	  }


	  /// <summary>
	  /// Replace the old child with a new child.
	  /// </summary>
	  /// <param name="newChildElem"> New child to replace with </param>
	  /// <param name="oldChildElem"> Old child to be replaced
	  /// </param>
	  /// <returns> The new child
	  /// </returns>
	  /// <exception cref="DOMException"> </exception>
	  public virtual ElemTemplateElement replaceChild(ElemTemplateElement newChildElem, ElemTemplateElement oldChildElem)
	  {

		if (oldChildElem == null || oldChildElem.ParentElem != this)
		{
		  return null;
		}

		// Fix up previous sibling.
		ElemTemplateElement prev = oldChildElem.PreviousSiblingElem;

		if (null != prev)
		{
		  prev.m_nextSibling = newChildElem;
		}

		// Fix up parent (this)
		if (m_firstChild == oldChildElem)
		{
		  m_firstChild = newChildElem;
		}

		newChildElem.m_parentNode = this;
		oldChildElem.m_parentNode = null;
		newChildElem.m_nextSibling = oldChildElem.m_nextSibling;
		oldChildElem.m_nextSibling = null;

		// newChildElem.m_stylesheet = oldChildElem.m_stylesheet;
		// oldChildElem.m_stylesheet = null;
		return newChildElem;
	  }

	  /// <summary>
	  /// NodeList method: Count the immediate children of this node
	  /// </summary>
	  /// <returns> The count of children of this node </returns>
	  public override int Length
	  {
		  get
		  {
    
			// It is assumed that the getChildNodes call synchronized
			// the children. Therefore, we can access the first child
			// reference directly.
			int count = 0;
    
			for (ElemTemplateElement node = m_firstChild; node != null; node = node.m_nextSibling)
			{
			  count++;
			}
    
			return count;
		  }
	  } // getLength():int

	  /// <summary>
	  /// NodeList method: Return the Nth immediate child of this node, or
	  /// null if the index is out of bounds.
	  /// </summary>
	  /// <param name="index"> Index of child to find </param>
	  /// <returns> org.w3c.dom.Node: the child node at given index </returns>
	  public override Node item(int index)
	  {

		// It is assumed that the getChildNodes call synchronized
		// the children. Therefore, we can access the first child
		// reference directly.
		ElemTemplateElement node = m_firstChild;

		for (int i = 0; i < index && node != null; i++)
		{
		  node = node.m_nextSibling;
		}

		return node;
	  } // item(int):Node

	  /// <summary>
	  /// Get the stylesheet owner.
	  /// </summary>
	  /// <returns> The stylesheet owner </returns>
	  public override Document OwnerDocument
	  {
		  get
		  {
			return Stylesheet;
		  }
	  }

	  /// <summary>
	  /// Get the owning xsl:template element.
	  /// </summary>
	  /// <returns> The owning xsl:template element, this element if it is a xsl:template, or null if not found. </returns>
	  public virtual ElemTemplate OwnerXSLTemplate
	  {
		  get
		  {
			  ElemTemplateElement el = this;
			  int type = el.XSLToken;
			  while ((null != el) && (type != Constants.ELEMNAME_TEMPLATE))
			  {
				el = el.ParentElem;
				if (null != el)
				{
					  type = el.XSLToken;
				}
			  }
			  return (ElemTemplate)el;
		  }
	  }


	  /// <summary>
	  /// Return the element name.
	  /// </summary>
	  /// <returns> The element name </returns>
	  public override string TagName
	  {
		  get
		  {
			return NodeName;
		  }
	  }

	  /// <summary>
	  /// Tell if this element only has one text child, for optimization purposes. </summary>
	  /// <returns> true of this element only has one text literal child. </returns>
	  public virtual bool hasTextLitOnly()
	  {
		return m_hasTextLitOnly;
	  }

	  /// <summary>
	  /// Return the base identifier.
	  /// </summary>
	  /// <returns> The base identifier  </returns>
	  public virtual string BaseIdentifier
	  {
		  get
		  {
    
			// Should this always be absolute?
			return this.SystemId;
		  }
	  }

	  /// <summary>
	  /// line number where the current document event ends.
	  ///  @serial         
	  /// </summary>
	  private int m_lineNumber;

	  /// <summary>
	  /// line number where the current document event ends.
	  ///  @serial         
	  /// </summary>
	  private int m_endLineNumber;

	  /// <summary>
	  /// Return the line number where the current document event ends.
	  /// Note that this is the line position of the first character
	  /// after the text associated with the document event. </summary>
	  /// <returns> The line number, or -1 if none is available. </returns>
	  /// <seealso cref= #getColumnNumber </seealso>
	  public virtual int EndLineNumber
	  {
		  get
		  {
			return m_endLineNumber;
		  }
	  }

	  /// <summary>
	  /// Return the line number where the current document event ends.
	  /// Note that this is the line position of the first character
	  /// after the text associated with the document event. </summary>
	  /// <returns> The line number, or -1 if none is available. </returns>
	  /// <seealso cref= #getColumnNumber </seealso>
	  public virtual int LineNumber
	  {
		  get
		  {
			return m_lineNumber;
		  }
	  }

	  /// <summary>
	  /// the column number where the current document event ends.
	  ///  @serial        
	  /// </summary>
	  private int m_columnNumber;

	  /// <summary>
	  /// the column number where the current document event ends.
	  ///  @serial        
	  /// </summary>
	  private int m_endColumnNumber;

	  /// <summary>
	  /// Return the column number where the current document event ends.
	  /// Note that this is the column number of the first
	  /// character after the text associated with the document
	  /// event.  The first column in a line is position 1. </summary>
	  /// <returns> The column number, or -1 if none is available. </returns>
	  /// <seealso cref= #getLineNumber </seealso>
	  public virtual int EndColumnNumber
	  {
		  get
		  {
			return m_endColumnNumber;
		  }
	  }

	  /// <summary>
	  /// Return the column number where the current document event ends.
	  /// Note that this is the column number of the first
	  /// character after the text associated with the document
	  /// event.  The first column in a line is position 1. </summary>
	  /// <returns> The column number, or -1 if none is available. </returns>
	  /// <seealso cref= #getLineNumber </seealso>
	  public virtual int ColumnNumber
	  {
		  get
		  {
			return m_columnNumber;
		  }
	  }

	  /// <summary>
	  /// Return the public identifier for the current document event.
	  /// <para>This will be the public identifier
	  /// </para>
	  /// </summary>
	  /// <returns> A string containing the public identifier, or
	  ///         null if none is available. </returns>
	  /// <seealso cref= #getSystemId </seealso>
	  public virtual string PublicId
	  {
		  get
		  {
			return (null != m_parentNode) ? m_parentNode.PublicId : null;
		  }
	  }

	  /// <summary>
	  /// Return the system identifier for the current document event.
	  /// 
	  /// <para>If the system identifier is a URL, the parser must resolve it
	  /// fully before passing it to the application.</para>
	  /// </summary>
	  /// <returns> A string containing the system identifier, or null
	  ///         if none is available. </returns>
	  /// <seealso cref= #getPublicId </seealso>
	  public virtual string SystemId
	  {
		  get
		  {
			Stylesheet sheet = Stylesheet;
			return (sheet == null) ? null : sheet.Href;
		  }
	  }

	  /// <summary>
	  /// Set the location information for this element.
	  /// </summary>
	  /// <param name="locator"> Source Locator with location information for this element </param>
	  public virtual SourceLocator LocaterInfo
	  {
		  set
		  {
			m_lineNumber = value.LineNumber;
			m_columnNumber = value.ColumnNumber;
		  }
	  }

	  /// <summary>
	  /// Set the end location information for this element.
	  /// </summary>
	  /// <param name="locator"> Source Locator with location information for this element </param>
	  public virtual SourceLocator EndLocaterInfo
	  {
		  set
		  {
			m_endLineNumber = value.LineNumber;
			m_endColumnNumber = value.ColumnNumber;
		  }
	  }

	  /// <summary>
	  /// Tell if this element has the default space handling
	  /// turned off or on according to the xml:space attribute.
	  /// @serial
	  /// </summary>
	  private bool m_defaultSpace = true;

	  /// <summary>
	  /// Tell if this element only has one text child, for optimization purposes.
	  /// @serial
	  /// </summary>
	  private bool m_hasTextLitOnly = false;

	  /// <summary>
	  /// Tell if this element only has one text child, for optimization purposes.
	  /// @serial
	  /// </summary>
	  protected internal bool m_hasVariableDecl = false;

	  public virtual bool hasVariableDecl()
	  {
		return m_hasVariableDecl;
	  }

	  /// <summary>
	  /// Set the "xml:space" attribute.
	  /// A text node is preserved if an ancestor element of the text node
	  /// has an xml:space attribute with a value of preserve, and
	  /// no closer ancestor element has xml:space with a value of default. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#strip">strip in XSLT Specification</a> </seealso>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Creating-Text">section-Creating-Text in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="v">  Enumerated value, either Constants.ATTRVAL_PRESERVE 
	  /// or Constants.ATTRVAL_STRIP. </param>
	  public virtual void setXmlSpace(int v)
	  {
		m_defaultSpace = ((Constants.ATTRVAL_STRIP == v) ? true : false);
	  }

	  /// <summary>
	  /// Get the "xml:space" attribute.
	  /// A text node is preserved if an ancestor element of the text node
	  /// has an xml:space attribute with a value of preserve, and
	  /// no closer ancestor element has xml:space with a value of default. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#strip">strip in XSLT Specification</a> </seealso>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#section-Creating-Text">section-Creating-Text in XSLT Specification</a>
	  /// </seealso>
	  /// <returns> The value of the xml:space attribute </returns>
	  public virtual bool getXmlSpace()
	  {
		return m_defaultSpace;
	  }

	  /// <summary>
	  /// The list of namespace declarations for this element only.
	  /// @serial
	  /// </summary>
	  private IList m_declaredPrefixes;

	  /// <summary>
	  /// Return a table that contains all prefixes available
	  /// within this element context.
	  /// </summary>
	  /// <returns> Vector containing the prefixes available within this
	  /// element context  </returns>
	  public virtual IList DeclaredPrefixes
	  {
		  get
		  {
			return m_declaredPrefixes;
		  }
	  }

	  /// <summary>
	  /// From the SAX2 helper class, set the namespace table for
	  /// this element.  Take care to call resolveInheritedNamespaceDecls.
	  /// after all namespace declarations have been added.
	  /// </summary>
	  /// <param name="nsSupport"> non-null reference to NamespaceSupport from 
	  /// the ContentHandler.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPrefixes(org.xml.sax.helpers.NamespaceSupport nsSupport) throws javax.xml.transform.TransformerException
	  public virtual NamespaceSupport Prefixes
	  {
		  set
		  {
			setPrefixes(value, false);
		  }
	  }

	  /// <summary>
	  /// Copy the namespace declarations from the NamespaceSupport object.  
	  /// Take care to call resolveInheritedNamespaceDecls.
	  /// after all namespace declarations have been added.
	  /// </summary>
	  /// <param name="nsSupport"> non-null reference to NamespaceSupport from 
	  /// the ContentHandler. </param>
	  /// <param name="excludeXSLDecl"> true if XSLT namespaces should be ignored.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPrefixes(org.xml.sax.helpers.NamespaceSupport nsSupport, boolean excludeXSLDecl) throws javax.xml.transform.TransformerException
	  public virtual void setPrefixes(NamespaceSupport nsSupport, bool excludeXSLDecl)
	  {

		System.Collections.IEnumerator decls = nsSupport.DeclaredPrefixes;

		while (decls.MoveNext())
		{
		  string prefix = (string) decls.Current;

		  if (null == m_declaredPrefixes)
		  {
			m_declaredPrefixes = new ArrayList();
		  }

		  string uri = nsSupport.getURI(prefix);

		  if (excludeXSLDecl && uri.Equals(Constants.S_XSLNAMESPACEURL))
		  {
			continue;
		  }

		  // System.out.println("setPrefixes - "+prefix+", "+uri);
		  XMLNSDecl decl = new XMLNSDecl(prefix, uri, false);

		  m_declaredPrefixes.Add(decl);
		}
	  }

	  /// <summary>
	  /// Fullfill the PrefixResolver interface.  Calling this for this class 
	  /// will throw an error.
	  /// </summary>
	  /// <param name="prefix"> The prefix to look up, which may be an empty string ("") 
	  ///               for the default Namespace. </param>
	  /// <param name="context"> The node context from which to look up the URI.
	  /// </param>
	  /// <returns> null if the error listener does not choose to throw an exception. </returns>
	  public virtual string getNamespaceForPrefix(string prefix, Node context)
	  {
		this.error(XSLTErrorResources.ER_CANT_RESOLVE_NSPREFIX, null);

		return null;
	  }

	  /// <summary>
	  /// Given a namespace, get the corrisponding prefix.
	  /// 9/15/00: This had been iteratively examining the m_declaredPrefixes
	  /// field for this node and its parents. That makes life difficult for
	  /// the compilation experiment, which doesn't have a static vector of
	  /// local declarations. Replaced a recursive solution, which permits
	  /// easier subclassing/overriding.
	  /// </summary>
	  /// <param name="prefix"> non-null reference to prefix string, which should map 
	  ///               to a namespace URL.
	  /// </param>
	  /// <returns> The namespace URL that the prefix maps to, or null if no 
	  ///         mapping can be found. </returns>
	  public virtual string getNamespaceForPrefix(string prefix)
	  {
	//    if (null != prefix && prefix.equals("xmlns"))
	//    {
	//      return Constants.S_XMLNAMESPACEURI;
	//    }

		IList nsDecls = m_declaredPrefixes;

		if (null != nsDecls)
		{
		  int n = nsDecls.Count;
		  if (prefix.Equals(Constants.ATTRVAL_DEFAULT_PREFIX))
		  {
			prefix = "";
		  }

		  for (int i = 0; i < n; i++)
		  {
			XMLNSDecl decl = (XMLNSDecl) nsDecls[i];

			if (prefix.Equals(decl.Prefix))
			{
			  return decl.URI;
			}
		  }
		}

		// Not found; ask our ancestors
		if (null != m_parentNode)
		{
		  return m_parentNode.getNamespaceForPrefix(prefix);
		}

		// JJK: No ancestors; try implicit
		// %REVIEW% Are there literals somewhere that we should use instead?
		// %REVIEW% Is this really the best place to patch?
		if ("xml".Equals(prefix))
		{
		  return "http://www.w3.org/XML/1998/namespace";
		}

		// No parent, so no definition
		return null;
	  }

	  /// <summary>
	  /// The table of <seealso cref="XMLNSDecl"/>s for this element
	  /// and all parent elements, screened for excluded prefixes.
	  /// @serial
	  /// </summary>
	  private IList m_prefixTable;

	  /// <summary>
	  /// Return a table that contains all prefixes available
	  /// within this element context.
	  /// </summary>
	  /// <returns> reference to vector of <seealso cref="XMLNSDecl"/>s, which may be null. </returns>
	  internal virtual IList PrefixTable
	  {
		  get
		  {
			return m_prefixTable;
		  }
		  set
		  {
			  m_prefixTable = value;
		  }
	  }


	  /// <summary>
	  /// Get whether or not the passed URL is contained flagged by
	  /// the "extension-element-prefixes" property.  This method is overridden 
	  /// by <seealso cref="ElemLiteralResult#containsExcludeResultPrefix"/>. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#extension-element">extension-element in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="prefix"> non-null reference to prefix that might be excluded.
	  /// </param>
	  /// <returns> true if the prefix should normally be excluded. </returns>
	  public virtual bool containsExcludeResultPrefix(string prefix, string uri)
	  {
		ElemTemplateElement parent = this.ParentElem;
		if (null != parent)
		{
		  return parent.containsExcludeResultPrefix(prefix, uri);
		}

		return false;
	  }

	  /// <summary>
	  /// Tell if the result namespace decl should be excluded.  Should be called before
	  /// namespace aliasing (I think).
	  /// </summary>
	  /// <param name="prefix"> non-null reference to prefix. </param>
	  /// <param name="uri"> reference to namespace that prefix maps to, which is protected 
	  ///            for null, but should really never be passed as null.
	  /// </param>
	  /// <returns> true if the given namespace should be excluded.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private boolean excludeResultNSDecl(String prefix, String uri) throws javax.xml.transform.TransformerException
	  private bool excludeResultNSDecl(string prefix, string uri)
	  {

		if (!string.ReferenceEquals(uri, null))
		{
		  if (uri.Equals(Constants.S_XSLNAMESPACEURL) || Stylesheet.containsExtensionElementURI(uri))
		  {
			return true;
		  }

		  if (containsExcludeResultPrefix(prefix, uri))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Combine the parent's namespaces with this namespace
	  /// for fast processing, taking care to reference the
	  /// parent's namespace if this namespace adds nothing new.
	  /// (Recursive method, walking the elements depth-first,
	  /// processing parents before children).
	  /// Note that this method builds m_prefixTable with aliased 
	  /// namespaces, *not* the original namespaces.
	  /// </summary>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resolvePrefixTables() throws javax.xml.transform.TransformerException
	  public virtual void resolvePrefixTables()
	  {
		// Always start with a fresh prefix table!
		PrefixTable = null;

		// If we have declared declarations, then we look for 
		// a parent that has namespace decls, and add them 
		// to this element's decls.  Otherwise we just point 
		// to the parent that has decls.
		if (null != this.m_declaredPrefixes)
		{
		  StylesheetRoot stylesheet = this.StylesheetRoot;

		  // Add this element's declared prefixes to the 
		  // prefix table.
		  int n = m_declaredPrefixes.Count;

		  for (int i = 0; i < n; i++)
		  {
			XMLNSDecl decl = (XMLNSDecl) m_declaredPrefixes[i];
			string prefix = decl.Prefix;
			string uri = decl.URI;
			if (null == uri)
			{
			  uri = "";
			}
			bool shouldExclude = excludeResultNSDecl(prefix, uri);

			// Create a new prefix table if one has not already been created.
			if (null == m_prefixTable)
			{
				PrefixTable = new ArrayList();
			}

			NamespaceAlias nsAlias = stylesheet.getNamespaceAliasComposed(uri);
			if (null != nsAlias)
			{
			  // Should I leave the non-aliased element in the table as 
			  // an excluded element?

			  // The exclusion should apply to the non-aliased prefix, so 
			  // we don't calculate it here.  -sb
			  // Use stylesheet prefix, as per xsl WG
			  decl = new XMLNSDecl(nsAlias.StylesheetPrefix, nsAlias.ResultNamespace, shouldExclude);
			}
			else
			{
			  decl = new XMLNSDecl(prefix, uri, shouldExclude);
			}

			m_prefixTable.Add(decl);

		  }
		}

		ElemTemplateElement parent = this.ParentNodeElem;

		if (null != parent)
		{

		  // The prefix table of the parent should never be null!
		  IList prefixes = parent.m_prefixTable;

		  if (null == m_prefixTable && !needToCheckExclude())
		  {

			// Nothing to combine, so just use parent's table!
			PrefixTable = parent.m_prefixTable;
		  }
		  else
		  {

			// Add the prefixes from the parent's prefix table.
			int n = prefixes.Count;

			for (int i = 0; i < n; i++)
			{
			  XMLNSDecl decl = (XMLNSDecl) prefixes[i];
			  bool shouldExclude = excludeResultNSDecl(decl.Prefix, decl.URI);

			  if (shouldExclude != decl.IsExcluded)
			  {
				decl = new XMLNSDecl(decl.Prefix, decl.URI, shouldExclude);
			  }

			  //m_prefixTable.addElement(decl);
			  addOrReplaceDecls(decl);
			}
		  }
		}
		else if (null == m_prefixTable)
		{

		  // Must be stylesheet element without any result prefixes!
		  PrefixTable = new ArrayList();
		}
	  }

	  /// <summary>
	  /// Add or replace this namespace declaration in list
	  /// of namespaces in scope for this element.
	  /// </summary>
	  /// <param name="newDecl"> namespace declaration to add to list </param>
	  internal virtual void addOrReplaceDecls(XMLNSDecl newDecl)
	  {
		  int n = m_prefixTable.Count;

			for (int i = n - 1; i >= 0; i--)
			{
			  XMLNSDecl decl = (XMLNSDecl) m_prefixTable[i];

			  if (decl.Prefix.Equals(newDecl.Prefix))
			  {
				return;
			  }
			}
		  m_prefixTable.Add(newDecl);

	  }

	  /// <summary>
	  /// Return whether we need to check namespace prefixes 
	  /// against and exclude result prefixes list.
	  /// </summary>
	  internal virtual bool needToCheckExclude()
	  {
		return false;
	  }

	  /// <summary>
	  /// Send startPrefixMapping events to the result tree handler
	  /// for all declared prefix mappings in the stylesheet.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void executeNSDecls(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  internal virtual void executeNSDecls(TransformerImpl transformer)
	  {
		   executeNSDecls(transformer, null);
	  }

	  /// <summary>
	  /// Send startPrefixMapping events to the result tree handler
	  /// for all declared prefix mappings in the stylesheet.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="ignorePrefix"> string prefix to not startPrefixMapping
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void executeNSDecls(org.apache.xalan.transformer.TransformerImpl transformer, String ignorePrefix) throws javax.xml.transform.TransformerException
	  internal virtual void executeNSDecls(TransformerImpl transformer, string ignorePrefix)
	  {
		try
		{
		  if (null != m_prefixTable)
		  {
			SerializationHandler rhandler = transformer.ResultTreeHandler;
			int n = m_prefixTable.Count;

			for (int i = n - 1; i >= 0; i--)
			{
			  XMLNSDecl decl = (XMLNSDecl) m_prefixTable[i];

			  if (!decl.IsExcluded && !(null != ignorePrefix && decl.Prefix.Equals(ignorePrefix)))
			  {
				rhandler.startPrefixMapping(decl.Prefix, decl.URI, true);
			  }
			}
		  }
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}
	  }

	  /// <summary>
	  /// Send endPrefixMapping events to the result tree handler
	  /// for all declared prefix mappings in the stylesheet.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void unexecuteNSDecls(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  internal virtual void unexecuteNSDecls(TransformerImpl transformer)
	  {
		   unexecuteNSDecls(transformer, null);
	  }

	  /// <summary>
	  /// Send endPrefixMapping events to the result tree handler
	  /// for all declared prefix mappings in the stylesheet.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state. </param>
	  /// <param name="ignorePrefix"> string prefix to not endPrefixMapping
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void unexecuteNSDecls(org.apache.xalan.transformer.TransformerImpl transformer, String ignorePrefix) throws javax.xml.transform.TransformerException
	  internal virtual void unexecuteNSDecls(TransformerImpl transformer, string ignorePrefix)
	  {

		try
		{
		  if (null != m_prefixTable)
		  {
			SerializationHandler rhandler = transformer.ResultTreeHandler;
			int n = m_prefixTable.Count;

			for (int i = 0; i < n; i++)
			{
			  XMLNSDecl decl = (XMLNSDecl) m_prefixTable[i];

			  if (!decl.IsExcluded && !(null != ignorePrefix && decl.Prefix.Equals(ignorePrefix)))
			  {
				rhandler.endPrefixMapping(decl.Prefix);
			  }
			}
		  }
		}
		catch (org.xml.sax.SAXException se)
		{
		  throw new TransformerException(se);
		}
	  }

	  /// <summary>
	  /// The *relative* document order number of this element.
	  ///  @serial 
	  /// </summary>
	  protected internal int m_docOrderNumber = -1;

	  /// <summary>
	  /// Set the UID (document order index).
	  /// </summary>
	  /// <param name="i"> Index of this child. </param>
	  public virtual int Uid
	  {
		  set
		  {
			m_docOrderNumber = value;
		  }
		  get
		  {
			return m_docOrderNumber;
		  }
	  }



	  /// <summary>
	  /// Parent node.
	  /// @serial
	  /// </summary>
	  protected internal ElemTemplateElement m_parentNode;

	  /// <summary>
	  /// Get the parent as a Node.
	  /// </summary>
	  /// <returns> This node's parent node </returns>
	  public override Node ParentNode
	  {
		  get
		  {
			return m_parentNode;
		  }
	  }

	  /// <summary>
	  /// Get the parent as an ElemTemplateElement.
	  /// </summary>
	  /// <returns> This node's parent as an ElemTemplateElement </returns>
	  public virtual ElemTemplateElement ParentElem
	  {
		  get
		  {
			return m_parentNode;
		  }
		  set
		  {
			m_parentNode = value;
		  }
	  }


	  /// <summary>
	  /// Next sibling.
	  /// @serial
	  /// </summary>
	  internal ElemTemplateElement m_nextSibling;

	  /// <summary>
	  /// Get the next sibling (as a Node) or return null.
	  /// </summary>
	  /// <returns> this node's next sibling or null </returns>
	  public override Node NextSibling
	  {
		  get
		  {
			return m_nextSibling;
		  }
	  }

	  /// <summary>
	  /// Get the previous sibling (as a Node) or return null.
	  /// Note that this may be expensive if the parent has many kids;
	  /// we accept that price in exchange for avoiding the prev pointer
	  /// TODO: If we were sure parents and sibs are always ElemTemplateElements,
	  /// we could hit the fields directly rather than thru accessors.
	  /// </summary>
	  /// <returns> This node's previous sibling or null </returns>
	  public override Node PreviousSibling
	  {
		  get
		  {
    
			Node walker = ParentNode, prev = null;
    
			if (walker != null)
			{
			  for (walker = walker.FirstChild; walker != null; prev = walker, walker = walker.NextSibling)
			  {
				if (walker == this)
				{
				  return prev;
				}
			  }
			}
    
			return null;
		  }
	  }

	  /// <summary>
	  /// Get the previous sibling (as a Node) or return null.
	  /// Note that this may be expensive if the parent has many kids;
	  /// we accept that price in exchange for avoiding the prev pointer
	  /// TODO: If we were sure parents and sibs are always ElemTemplateElements,
	  /// we could hit the fields directly rather than thru accessors.
	  /// </summary>
	  /// <returns> This node's previous sibling or null </returns>
	  public virtual ElemTemplateElement PreviousSiblingElem
	  {
		  get
		  {
    
			ElemTemplateElement walker = ParentNodeElem;
			ElemTemplateElement prev = null;
    
			if (walker != null)
			{
			  for (walker = walker.FirstChildElem; walker != null; prev = walker, walker = walker.NextSiblingElem)
			  {
				if (walker == this)
				{
				  return prev;
				}
			  }
			}
    
			return null;
		  }
	  }


	  /// <summary>
	  /// Get the next sibling (as a ElemTemplateElement) or return null.
	  /// </summary>
	  /// <returns> This node's next sibling (as a ElemTemplateElement) or null  </returns>
	  public virtual ElemTemplateElement NextSiblingElem
	  {
		  get
		  {
			return m_nextSibling;
		  }
	  }

	  /// <summary>
	  /// Get the parent element.
	  /// </summary>
	  /// <returns> This node's next parent (as a ElemTemplateElement) or null  </returns>
	  public virtual ElemTemplateElement ParentNodeElem
	  {
		  get
		  {
			return m_parentNode;
		  }
	  }


	  /// <summary>
	  /// First child.
	  /// @serial
	  /// </summary>
	  internal ElemTemplateElement m_firstChild;

	  /// <summary>
	  /// Get the first child as a Node.
	  /// </summary>
	  /// <returns> This node's first child or null </returns>
	  public override Node FirstChild
	  {
		  get
		  {
			return m_firstChild;
		  }
	  }

	  /// <summary>
	  /// Get the first child as a ElemTemplateElement.
	  /// </summary>
	  /// <returns> This node's first child (as a ElemTemplateElement) or null </returns>
	  public virtual ElemTemplateElement FirstChildElem
	  {
		  get
		  {
			return m_firstChild;
		  }
	  }

	  /// <summary>
	  /// Get the last child.
	  /// </summary>
	  /// <returns> This node's last child </returns>
	  public override Node LastChild
	  {
		  get
		  {
    
			ElemTemplateElement lastChild = null;
    
			for (ElemTemplateElement node = m_firstChild; node != null; node = node.m_nextSibling)
			{
			  lastChild = node;
			}
    
			return lastChild;
		  }
	  }

	  /// <summary>
	  /// Get the last child.
	  /// </summary>
	  /// <returns> This node's last child </returns>
	  public virtual ElemTemplateElement LastChildElem
	  {
		  get
		  {
    
			ElemTemplateElement lastChild = null;
    
			for (ElemTemplateElement node = m_firstChild; node != null; node = node.m_nextSibling)
			{
			  lastChild = node;
			}
    
			return lastChild;
		  }
	  }


	  /// <summary>
	  /// DOM backpointer that this element originated from. </summary>
	  [NonSerialized]
	  private Node m_DOMBackPointer;

	  /// <summary>
	  /// If this stylesheet was created from a DOM, get the
	  /// DOM backpointer that this element originated from.
	  /// For tooling use.
	  /// </summary>
	  /// <returns> DOM backpointer that this element originated from or null. </returns>
	  public virtual Node DOMBackPointer
	  {
		  get
		  {
			return m_DOMBackPointer;
		  }
		  set
		  {
			m_DOMBackPointer = value;
		  }
	  }


	  /// <summary>
	  /// Compares this object with the specified object for precedence order.
	  /// The order is determined by the getImportCountComposed() of the containing
	  /// composed stylesheet and the getUid() of this element.
	  /// Returns a negative integer, zero, or a positive integer as this
	  /// object is less than, equal to, or greater than the specified object.
	  /// </summary>
	  /// <param name="o"> The object to be compared to this object </param>
	  /// <returns>  a negative integer, zero, or a positive integer as this object is
	  ///          less than, equal to, or greater than the specified object. </returns>
	  /// <exception cref="ClassCastException"> if the specified object's
	  ///         type prevents it from being compared to this Object. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int compareTo(Object o) throws ClassCastException
	  public virtual int compareTo(object o)
	  {

		ElemTemplateElement ro = (ElemTemplateElement) o;
		int roPrecedence = ro.StylesheetComposed.ImportCountComposed;
		int myPrecedence = this.StylesheetComposed.ImportCountComposed;

		if (myPrecedence < roPrecedence)
		{
		  return -1;
		}
		else if (myPrecedence > roPrecedence)
		{
		  return 1;
		}
		else
		{
		  return this.Uid - ro.Uid;
		}
	  }

	  /// <summary>
	  /// Get information about whether or not an element should strip whitespace. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#strip">strip in XSLT Specification</a>
	  /// </seealso>
	  /// <param name="support"> The XPath runtime state. </param>
	  /// <param name="targetElement"> Element to check
	  /// </param>
	  /// <returns> true if the whitespace should be stripped.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean shouldStripWhiteSpace(org.apache.xpath.XPathContext support, org.w3c.dom.Element targetElement) throws javax.xml.transform.TransformerException
	  public virtual bool shouldStripWhiteSpace(org.apache.xpath.XPathContext support, org.w3c.dom.Element targetElement)
	  {
		StylesheetRoot sroot = this.StylesheetRoot;
		return (null != sroot) ? sroot.shouldStripWhiteSpace(support, targetElement) :false;
	  }

	  /// <summary>
	  /// Get information about whether or not whitespace can be stripped. </summary>
	  /// <seealso cref= <a href="http://www.w3.org/TR/xslt#strip">strip in XSLT Specification</a>
	  /// </seealso>
	  /// <returns> true if the whitespace can be stripped. </returns>
	  public virtual bool canStripWhiteSpace()
	  {
		StylesheetRoot sroot = this.StylesheetRoot;
		return (null != sroot) ? sroot.canStripWhiteSpace() : false;
	  }

	  /// <summary>
	  /// Tell if this element can accept variable declarations. </summary>
	  /// <returns> true if the element can accept and process variable declarations. </returns>
	  public virtual bool canAcceptVariables()
	  {
		  return true;
	  }

	  //=============== ExpressionNode methods ================

	  /// <summary>
	  /// Set the parent of this node. </summary>
	  /// <param name="n"> Must be a ElemTemplateElement. </param>
	  public virtual void exprSetParent(ExpressionNode n)
	  {
		  // This obviously requires that only a ElemTemplateElement can 
		  // parent a node of this type.
		  ParentElem = (ElemTemplateElement)n;
	  }

	  /// <summary>
	  /// Get the ExpressionNode parent of this node.
	  /// </summary>
	  public virtual ExpressionNode exprGetParent()
	  {
		  return ParentElem;
	  }

	  /// <summary>
	  /// This method tells the node to add its argument to the node's
	  /// list of children. </summary>
	  /// <param name="n"> Must be a ElemTemplateElement.  </param>
	  public virtual void exprAddChild(ExpressionNode n, int i)
	  {
		  appendChild((ElemTemplateElement)n);
	  }

	  /// <summary>
	  /// This method returns a child node.  The children are numbered
	  ///   from zero, left to right. 
	  /// </summary>
	  public virtual ExpressionNode exprGetChild(int i)
	  {
		  return (ExpressionNode)item(i);
	  }

	  /// <summary>
	  /// Return the number of children the node has. </summary>
	  public virtual int exprGetNumChildren()
	  {
		  return Length;
	  }

	  /// <summary>
	  /// Accept a visitor and call the appropriate method 
	  /// for this class.
	  /// </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  /// <returns> true if the children of the object should be visited. </returns>
	  protected internal virtual bool accept(XSLTVisitor visitor)
	  {
		  return visitor.visitInstruction(this);
	  }

	  /// <seealso cref= XSLTVisitable#callVisitors(XSLTVisitor) </seealso>
	  public virtual void callVisitors(XSLTVisitor visitor)
	  {
		  if (accept(visitor))
		  {
			callChildVisitors(visitor);
		  }
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal virtual void callChildVisitors(XSLTVisitor visitor, bool callAttributes)
	  {
		for (ElemTemplateElement node = m_firstChild; node != null; node = node.m_nextSibling)
		{
		  node.callVisitors(visitor);
		}
	  }

	  /// <summary>
	  /// Call the children visitors. </summary>
	  /// <param name="visitor"> The visitor whose appropriate method will be called. </param>
	  protected internal virtual void callChildVisitors(XSLTVisitor visitor)
	  {
		  callChildVisitors(visitor, true);
	  }


		/// <seealso cref= PrefixResolver#handlesNullPrefixes() </seealso>
		public virtual bool handlesNullPrefixes()
		{
			return false;
		}

	}

}