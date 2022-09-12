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
 * $Id: ProcessorExsltFunction.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using ElemApplyImport = org.apache.xalan.templates.ElemApplyImport;
	using ElemApplyTemplates = org.apache.xalan.templates.ElemApplyTemplates;
	using ElemAttribute = org.apache.xalan.templates.ElemAttribute;
	using ElemCallTemplate = org.apache.xalan.templates.ElemCallTemplate;
	using ElemComment = org.apache.xalan.templates.ElemComment;
	using ElemCopy = org.apache.xalan.templates.ElemCopy;
	using ElemCopyOf = org.apache.xalan.templates.ElemCopyOf;
	using ElemElement = org.apache.xalan.templates.ElemElement;
	using ElemExsltFuncResult = org.apache.xalan.templates.ElemExsltFuncResult;
	using ElemExsltFunction = org.apache.xalan.templates.ElemExsltFunction;
	using ElemFallback = org.apache.xalan.templates.ElemFallback;
	using ElemLiteralResult = org.apache.xalan.templates.ElemLiteralResult;
	using ElemMessage = org.apache.xalan.templates.ElemMessage;
	using ElemNumber = org.apache.xalan.templates.ElemNumber;
	using ElemPI = org.apache.xalan.templates.ElemPI;
	using ElemParam = org.apache.xalan.templates.ElemParam;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemText = org.apache.xalan.templates.ElemText;
	using ElemTextLiteral = org.apache.xalan.templates.ElemTextLiteral;
	using ElemValueOf = org.apache.xalan.templates.ElemValueOf;
	using ElemVariable = org.apache.xalan.templates.ElemVariable;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using Attributes = org.xml.sax.Attributes;
	using SAXException = org.xml.sax.SAXException;


	/// <summary>
	/// This class processes parse events for an exslt func:function element.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class ProcessorExsltFunction : ProcessorTemplateElem
	{
		internal new const long serialVersionUID = 2411427965578315332L;
	  /// <summary>
	  /// Start an ElemExsltFunction. Verify that it is top level and that it has a name attribute with a
	  /// namespace.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {
		//System.out.println("ProcessorFunction.startElement()");
		string msg = "";
		if (!(handler.ElemTemplateElement is Stylesheet))
		{
		  msg = "func:function element must be top level.";
		  handler.error(msg, new SAXException(msg));
		}
		base.startElement(handler, uri, localName, rawName, attributes);

		string val = attributes.getValue("name");
		int indexOfColon = val.IndexOf(":", StringComparison.Ordinal);
		if (indexOfColon > 0)
		{
		  //String prefix = val.substring(0, indexOfColon);
		  //String localVal = val.substring(indexOfColon + 1);
		  //String ns = handler.getNamespaceSupport().getURI(prefix);
		  //if (ns.length() > 0)
		  //  System.out.println("fullfuncname " + ns + localVal);
		}
		else
		{
		  msg = "func:function name must have namespace";
		  handler.error(msg, new SAXException(msg));
		}
	  }

	  /// <summary>
	  /// Must include; super doesn't suffice!
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void appendAndPush(StylesheetHandler handler, org.apache.xalan.templates.ElemTemplateElement elem) throws org.xml.sax.SAXException
	  protected internal override void appendAndPush(StylesheetHandler handler, ElemTemplateElement elem)
	  {
		//System.out.println("ProcessorFunction appendAndPush()" + elem);
		base.appendAndPush(handler, elem);
		//System.out.println("originating node " + handler.getOriginatingNode());
		elem.DOMBackPointer = handler.OriginatingNode;
		handler.Stylesheet.Template = (ElemTemplate) elem;
	  }

	  /// <summary>
	  /// End an ElemExsltFunction, and verify its validity.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endElement(StylesheetHandler handler, String uri, String localName, String rawName) throws org.xml.sax.SAXException
	  public override void endElement(StylesheetHandler handler, string uri, string localName, string rawName)
	  {
	   ElemTemplateElement function = handler.ElemTemplateElement;
	   validate(function, handler); // may throw exception
	   base.endElement(handler, uri, localName, rawName);
	  }

	  /// <summary>
	  /// Non-recursive traversal of FunctionElement tree based on TreeWalker to verify that
	  /// there are no literal result elements except within a func:result element and that
	  /// the func:result element does not contain any following siblings except xsl:fallback.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void validate(org.apache.xalan.templates.ElemTemplateElement elem, StylesheetHandler handler) throws org.xml.sax.SAXException
	  public virtual void validate(ElemTemplateElement elem, StylesheetHandler handler)
	  {
		string msg = "";
		while (elem != null)
		{
		  //System.out.println("elem " + elem);
		  if (elem is ElemExsltFuncResult && elem.NextSiblingElem != null && !(elem.NextSiblingElem is ElemFallback))
		  {
			msg = "func:result has an illegal following sibling (only xsl:fallback allowed)";
			handler.error(msg, new SAXException(msg));
		  }

		  if ((elem is ElemApplyImport || elem is ElemApplyTemplates || elem is ElemAttribute || elem is ElemCallTemplate || elem is ElemComment || elem is ElemCopy || elem is ElemCopyOf || elem is ElemElement || elem is ElemLiteralResult || elem is ElemNumber || elem is ElemPI || elem is ElemText || elem is ElemTextLiteral || elem is ElemValueOf) && !(ancestorIsOk(elem)))
		  {
			msg = "misplaced literal result in a func:function container.";
			handler.error(msg, new SAXException(msg));
		  }
		  ElemTemplateElement nextElem = elem.FirstChildElem;
		  while (nextElem == null)
		  {
			nextElem = elem.NextSiblingElem;
			if (nextElem == null)
			{
			  elem = elem.ParentElem;
			}
			if (elem == null || elem is ElemExsltFunction)
			{
			  return; // ok
			}
		  }
		  elem = nextElem;
		}
	  }

	  /// <summary>
	  /// Verify that a literal result belongs to a result element, a variable, 
	  /// or a parameter.
	  /// </summary>

	  internal virtual bool ancestorIsOk(ElemTemplateElement child)
	  {
		while (child.ParentElem != null && !(child.ParentElem is ElemExsltFunction))
		{
		  ElemTemplateElement parent = child.ParentElem;
		  if (parent is ElemExsltFuncResult || parent is ElemVariable || parent is ElemParam || parent is ElemMessage)
		  {
			return true;
		  }
		  child = parent;
		}
		return false;
	  }

	}

}