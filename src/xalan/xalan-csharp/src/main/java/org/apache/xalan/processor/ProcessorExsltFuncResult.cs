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
 * $Id: ProcessorExsltFuncResult.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{
	using ElemExsltFuncResult = org.apache.xalan.templates.ElemExsltFuncResult;
	using ElemExsltFunction = org.apache.xalan.templates.ElemExsltFunction;
	using ElemParam = org.apache.xalan.templates.ElemParam;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemVariable = org.apache.xalan.templates.ElemVariable;

	using Attributes = org.xml.sax.Attributes;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This class processes parse events for an exslt func:result element.
	/// @xsl.usage internal
	/// </summary>
	[Serializable]
	public class ProcessorExsltFuncResult : ProcessorTemplateElem
	{
		internal new const long serialVersionUID = 6451230911473482423L;

	  /// <summary>
	  /// Verify that the func:result element does not appear within a variable,
	  /// parameter, or another func:result, and that it belongs to a func:function 
	  /// element.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void startElement(StylesheetHandler handler, String uri, String localName, String rawName, org.xml.sax.Attributes attributes) throws org.xml.sax.SAXException
	  public override void startElement(StylesheetHandler handler, string uri, string localName, string rawName, Attributes attributes)
	  {
		string msg = "";

		base.startElement(handler, uri, localName, rawName, attributes);
		ElemTemplateElement ancestor = handler.ElemTemplateElement.ParentElem;
		while (ancestor != null && !(ancestor is ElemExsltFunction))
		{
		  if (ancestor is ElemVariable || ancestor is ElemParam || ancestor is ElemExsltFuncResult)
		  {
			msg = "func:result cannot appear within a variable, parameter, or another func:result.";
			handler.error(msg, new SAXException(msg));
		  }
		  ancestor = ancestor.ParentElem;
		}
		if (ancestor == null)
		{
		  msg = "func:result must appear in a func:function element";
		  handler.error(msg, new SAXException(msg));
		}
	  }
	}

}