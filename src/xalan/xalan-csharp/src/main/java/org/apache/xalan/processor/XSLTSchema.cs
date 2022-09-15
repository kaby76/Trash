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
 * $Id: XSLTSchema.java 476466 2006-11-18 08:22:31Z minchau $
 */
namespace org.apache.xalan.processor
{

	using Constants = org.apache.xalan.templates.Constants;
	using ElemApplyImport = org.apache.xalan.templates.ElemApplyImport;
	using ElemApplyTemplates = org.apache.xalan.templates.ElemApplyTemplates;
	using ElemAttribute = org.apache.xalan.templates.ElemAttribute;
	using ElemCallTemplate = org.apache.xalan.templates.ElemCallTemplate;
	using ElemChoose = org.apache.xalan.templates.ElemChoose;
	using ElemComment = org.apache.xalan.templates.ElemComment;
	using ElemCopy = org.apache.xalan.templates.ElemCopy;
	using ElemCopyOf = org.apache.xalan.templates.ElemCopyOf;
	using ElemElement = org.apache.xalan.templates.ElemElement;
	using ElemExsltFuncResult = org.apache.xalan.templates.ElemExsltFuncResult;
	using ElemExsltFunction = org.apache.xalan.templates.ElemExsltFunction;
	using ElemExtensionDecl = org.apache.xalan.templates.ElemExtensionDecl;
	using ElemExtensionScript = org.apache.xalan.templates.ElemExtensionScript;
	using ElemFallback = org.apache.xalan.templates.ElemFallback;
	using ElemForEach = org.apache.xalan.templates.ElemForEach;
	using ElemIf = org.apache.xalan.templates.ElemIf;
	using ElemLiteralResult = org.apache.xalan.templates.ElemLiteralResult;
	using ElemMessage = org.apache.xalan.templates.ElemMessage;
	using ElemNumber = org.apache.xalan.templates.ElemNumber;
	using ElemOtherwise = org.apache.xalan.templates.ElemOtherwise;
	using ElemPI = org.apache.xalan.templates.ElemPI;
	using ElemParam = org.apache.xalan.templates.ElemParam;
	using ElemSort = org.apache.xalan.templates.ElemSort;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemText = org.apache.xalan.templates.ElemText;
	using ElemTextLiteral = org.apache.xalan.templates.ElemTextLiteral;
	using ElemUnknown = org.apache.xalan.templates.ElemUnknown;
	using ElemValueOf = org.apache.xalan.templates.ElemValueOf;
	using ElemVariable = org.apache.xalan.templates.ElemVariable;
	using ElemWhen = org.apache.xalan.templates.ElemWhen;
	using ElemWithParam = org.apache.xalan.templates.ElemWithParam;
	using QName = org.apache.xml.utils.QName;

	/// <summary>
	/// This class defines the allowed structure for a stylesheet, and the
	/// mapping between Xalan classes and the markup elements in the stylesheet. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	public class XSLTSchema : XSLTElementDef
	{

	  /// <summary>
	  /// Construct a XSLTSchema which represents the XSLT "schema".
	  /// </summary>
	  internal XSLTSchema()
	  {
		build();
	  }

	  /// <summary>
	  /// This method builds an XSLT "schema" according to http://www.w3.org/TR/xslt#dtd.  This
	  /// schema provides instructions for building the Xalan Stylesheet (Templates) structure.
	  /// </summary>
	  internal virtual void build()
	  {
		// xsl:import, xsl:include
		XSLTAttributeDef hrefAttr = new XSLTAttributeDef(null, "href", XSLTAttributeDef.T_URL, true, false,XSLTAttributeDef.ERROR);

		// xsl:preserve-space, xsl:strip-space
		XSLTAttributeDef elementsAttr = new XSLTAttributeDef(null, "elements", XSLTAttributeDef.T_SIMPLEPATTERNLIST, true, false, XSLTAttributeDef.ERROR);

		// XSLTAttributeDef anyNamespacedAttr = new XSLTAttributeDef("*", "*",
		//                                XSLTAttributeDef.T_CDATA, false);

		// xsl:output
		XSLTAttributeDef methodAttr = new XSLTAttributeDef(null, "method", XSLTAttributeDef.T_QNAME, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef versionAttr = new XSLTAttributeDef(null, "version", XSLTAttributeDef.T_NMTOKEN, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef encodingAttr = new XSLTAttributeDef(null, "encoding", XSLTAttributeDef.T_CDATA, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef omitXmlDeclarationAttr = new XSLTAttributeDef(null, "omit-xml-declaration", XSLTAttributeDef.T_YESNO, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef standaloneAttr = new XSLTAttributeDef(null, "standalone", XSLTAttributeDef.T_YESNO, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef doctypePublicAttr = new XSLTAttributeDef(null, "doctype-public", XSLTAttributeDef.T_CDATA, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef doctypeSystemAttr = new XSLTAttributeDef(null, "doctype-system", XSLTAttributeDef.T_CDATA, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef cdataSectionElementsAttr = new XSLTAttributeDef(null, "cdata-section-elements", XSLTAttributeDef.T_QNAMES_RESOLVE_NULL, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef indentAttr = new XSLTAttributeDef(null, "indent", XSLTAttributeDef.T_YESNO, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef mediaTypeAttr = new XSLTAttributeDef(null, "media-type", XSLTAttributeDef.T_CDATA, false, false,XSLTAttributeDef.ERROR);


		// Required.
		// It is an error if the name attribute is invalid on any of these elements
		// xsl:key, xsl:attribute-set, xsl:call-template, xsl:with-param, xsl:variable, xsl:param
		XSLTAttributeDef nameAttrRequired = new XSLTAttributeDef(null, "name", XSLTAttributeDef.T_QNAME, true, false,XSLTAttributeDef.ERROR);
		// Required.
		// Support AVT
		// xsl:element, xsl:attribute                                    
		XSLTAttributeDef nameAVTRequired = new XSLTAttributeDef(null, "name", XSLTAttributeDef.T_AVT_QNAME, true, true,XSLTAttributeDef.WARNING);


		// Required.
		// Support AVT
		// xsl:processing-instruction                                     
		XSLTAttributeDef nameAVT_NCNAMERequired = new XSLTAttributeDef(null, "name", XSLTAttributeDef.T_NCNAME, true, true,XSLTAttributeDef.WARNING);

		// Optional.
		// Static error if invalid
		// xsl:template, xsl:decimal-format                                      
		XSLTAttributeDef nameAttrOpt_ERROR = new XSLTAttributeDef(null, "name", XSLTAttributeDef.T_QNAME, false, false,XSLTAttributeDef.ERROR);

		// xsl:key                                 
		XSLTAttributeDef useAttr = new XSLTAttributeDef(null, "use", XSLTAttributeDef.T_EXPR, true, false,XSLTAttributeDef.ERROR);

		// xsl:element, xsl:attribute                              
		XSLTAttributeDef namespaceAVTOpt = new XSLTAttributeDef(null, "namespace",XSLTAttributeDef.T_URL, false, true,XSLTAttributeDef.WARNING);
		// xsl:decimal-format                                     
		XSLTAttributeDef decimalSeparatorAttr = new XSLTAttributeDef(null, "decimal-separator", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR, ".");
		XSLTAttributeDef infinityAttr = new XSLTAttributeDef(null, "infinity", XSLTAttributeDef.T_CDATA, false,XSLTAttributeDef.ERROR,"Infinity");
		XSLTAttributeDef minusSignAttr = new XSLTAttributeDef(null, "minus-sign", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR,"-");
		XSLTAttributeDef NaNAttr = new XSLTAttributeDef(null, "NaN", XSLTAttributeDef.T_CDATA, false,XSLTAttributeDef.ERROR, "NaN");
		XSLTAttributeDef percentAttr = new XSLTAttributeDef(null, "percent", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR, "%");
		XSLTAttributeDef perMilleAttr = new XSLTAttributeDef(null, "per-mille", XSLTAttributeDef.T_CHAR, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef zeroDigitAttr = new XSLTAttributeDef(null, "zero-digit", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR, "0");
		XSLTAttributeDef digitAttr = new XSLTAttributeDef(null, "digit", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR, "#");
		XSLTAttributeDef patternSeparatorAttr = new XSLTAttributeDef(null, "pattern-separator", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR, ";");
		// xsl:decimal-format                                         
		XSLTAttributeDef groupingSeparatorAttr = new XSLTAttributeDef(null, "grouping-separator", XSLTAttributeDef.T_CHAR, false,XSLTAttributeDef.ERROR,",");


		// xsl:element, xsl:attribute-set, xsl:copy                                           
		XSLTAttributeDef useAttributeSetsAttr = new XSLTAttributeDef(null, "use-attribute-sets", XSLTAttributeDef.T_QNAMES, false, false, XSLTAttributeDef.ERROR);

		// xsl:if, xsl:when         
		XSLTAttributeDef testAttrRequired = new XSLTAttributeDef(null, "test", XSLTAttributeDef.T_EXPR, true, false,XSLTAttributeDef.ERROR);


		// Required.                                       
		// xsl:value-of, xsl:for-each, xsl:copy-of                             
		XSLTAttributeDef selectAttrRequired = new XSLTAttributeDef(null, "select", XSLTAttributeDef.T_EXPR, true, false,XSLTAttributeDef.ERROR);

		// Optional.                                          
		// xsl:variable, xsl:param, xsl:with-param                                       
		XSLTAttributeDef selectAttrOpt = new XSLTAttributeDef(null, "select", XSLTAttributeDef.T_EXPR, false, false,XSLTAttributeDef.ERROR);

		// Optional.
		// Default: "node()"
		// xsl:apply-templates                                           
		XSLTAttributeDef selectAttrDefNode = new XSLTAttributeDef(null, "select", XSLTAttributeDef.T_EXPR, false,XSLTAttributeDef.ERROR, "node()");
		// Optional.
		// Default: "."
		// xsl:sort                                        
		XSLTAttributeDef selectAttrDefDot = new XSLTAttributeDef(null, "select", XSLTAttributeDef.T_EXPR, false,XSLTAttributeDef.ERROR, ".");
		// xsl:key                                      
		XSLTAttributeDef matchAttrRequired = new XSLTAttributeDef(null, "match", XSLTAttributeDef.T_PATTERN, true, false,XSLTAttributeDef.ERROR);
		// xsl:template                                       
		XSLTAttributeDef matchAttrOpt = new XSLTAttributeDef(null, "match", XSLTAttributeDef.T_PATTERN, false, false,XSLTAttributeDef.ERROR);
		// xsl:template                                  
		XSLTAttributeDef priorityAttr = new XSLTAttributeDef(null, "priority", XSLTAttributeDef.T_NUMBER, false, false,XSLTAttributeDef.ERROR);

		// xsl:template, xsl:apply-templates                                 
		XSLTAttributeDef modeAttr = new XSLTAttributeDef(null, "mode", XSLTAttributeDef.T_QNAME, false, false,XSLTAttributeDef.ERROR);

		XSLTAttributeDef spaceAttr = new XSLTAttributeDef(Constants.S_XMLNAMESPACEURI, "space", false, false, false, XSLTAttributeDef.WARNING, "default", Constants.ATTRVAL_STRIP, "preserve", Constants.ATTRVAL_PRESERVE);


		XSLTAttributeDef spaceAttrLiteral = new XSLTAttributeDef(Constants.S_XMLNAMESPACEURI, "space", XSLTAttributeDef.T_URL, false, true,XSLTAttributeDef.ERROR);
		// xsl:namespace-alias                                      
		XSLTAttributeDef stylesheetPrefixAttr = new XSLTAttributeDef(null, "stylesheet-prefix", XSLTAttributeDef.T_CDATA, true, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef resultPrefixAttr = new XSLTAttributeDef(null, "result-prefix", XSLTAttributeDef.T_CDATA, true, false,XSLTAttributeDef.ERROR);

		// xsl:text, xsl:value-of                                      
		XSLTAttributeDef disableOutputEscapingAttr = new XSLTAttributeDef(null, "disable-output-escaping", XSLTAttributeDef.T_YESNO, false, false,XSLTAttributeDef.ERROR);

		// xsl:number                                                   
		XSLTAttributeDef levelAttr = new XSLTAttributeDef(null, "level", false, false, false, XSLTAttributeDef.ERROR, "single", Constants.NUMBERLEVEL_SINGLE, "multiple", Constants.NUMBERLEVEL_MULTI, "any", Constants.NUMBERLEVEL_ANY);
		levelAttr.Default = "single";
		XSLTAttributeDef countAttr = new XSLTAttributeDef(null, "count", XSLTAttributeDef.T_PATTERN, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef fromAttr = new XSLTAttributeDef(null, "from", XSLTAttributeDef.T_PATTERN, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef valueAttr = new XSLTAttributeDef(null, "value", XSLTAttributeDef.T_EXPR, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef formatAttr = new XSLTAttributeDef(null, "format", XSLTAttributeDef.T_CDATA, false, true,XSLTAttributeDef.ERROR);
		formatAttr.Default = "1";

		// xsl:number, xsl:sort
		XSLTAttributeDef langAttr = new XSLTAttributeDef(null, "lang", XSLTAttributeDef.T_NMTOKEN, false, true,XSLTAttributeDef.ERROR);

		// xsl:number
		XSLTAttributeDef letterValueAttr = new XSLTAttributeDef(null, "letter-value", false, true, false, XSLTAttributeDef.ERROR, "alphabetic", Constants.NUMBERLETTER_ALPHABETIC, "traditional", Constants.NUMBERLETTER_TRADITIONAL);
		// xsl:number
		XSLTAttributeDef groupingSeparatorAVT = new XSLTAttributeDef(null, "grouping-separator", XSLTAttributeDef.T_CHAR, false, true,XSLTAttributeDef.ERROR);
		// xsl:number
		XSLTAttributeDef groupingSizeAttr = new XSLTAttributeDef(null, "grouping-size", XSLTAttributeDef.T_NUMBER, false, true,XSLTAttributeDef.ERROR);

	   // xsl:sort
		XSLTAttributeDef dataTypeAttr = new XSLTAttributeDef(null, "data-type", false, true, true, XSLTAttributeDef.ERROR, "text", Constants.SORTDATATYPE_TEXT,"number", Constants.SORTDATATYPE_TEXT);
		dataTypeAttr.Default = "text";

		// xsl:sort
		XSLTAttributeDef orderAttr = new XSLTAttributeDef(null, "order", false, true, false,XSLTAttributeDef.ERROR, "ascending", Constants.SORTORDER_ASCENDING, "descending", Constants.SORTORDER_DESCENDING);
		orderAttr.Default = "ascending";

		// xsl:sort                             
		XSLTAttributeDef caseOrderAttr = new XSLTAttributeDef(null, "case-order", false, true, false,XSLTAttributeDef.ERROR, "upper-first", Constants.SORTCASEORDER_UPPERFIRST, "lower-first", Constants.SORTCASEORDER_LOWERFIRST);

		// xsl:message                                   
		XSLTAttributeDef terminateAttr = new XSLTAttributeDef(null, "terminate", XSLTAttributeDef.T_YESNO, false, false,XSLTAttributeDef.ERROR);
		terminateAttr.Default = "no";

		// top level attributes
		XSLTAttributeDef xslExcludeResultPrefixesAttr = new XSLTAttributeDef(Constants.S_XSLNAMESPACEURL, "exclude-result-prefixes", XSLTAttributeDef.T_PREFIXLIST, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef xslExtensionElementPrefixesAttr = new XSLTAttributeDef(Constants.S_XSLNAMESPACEURL, "extension-element-prefixes", XSLTAttributeDef.T_PREFIX_URLLIST, false, false,XSLTAttributeDef.ERROR);
		// result-element-atts                       
		XSLTAttributeDef xslUseAttributeSetsAttr = new XSLTAttributeDef(Constants.S_XSLNAMESPACEURL, "use-attribute-sets", XSLTAttributeDef.T_QNAMES, false, false,XSLTAttributeDef.ERROR);
		XSLTAttributeDef xslVersionAttr = new XSLTAttributeDef(Constants.S_XSLNAMESPACEURL, "version", XSLTAttributeDef.T_NMTOKEN, false, false,XSLTAttributeDef.ERROR);

		XSLTElementDef charData = new XSLTElementDef(this, null, "text()", null, null, null, new ProcessorCharacters(), typeof(ElemTextLiteral));

		charData.Type = XSLTElementDef.T_PCDATA;

		XSLTElementDef whiteSpaceOnly = new XSLTElementDef(this, null, "text()", null, null, null, null, typeof(ElemTextLiteral));

		charData.Type = XSLTElementDef.T_PCDATA;

		XSLTAttributeDef resultAttr = new XSLTAttributeDef(null, "*", XSLTAttributeDef.T_AVT, false, true,XSLTAttributeDef.WARNING);
		XSLTAttributeDef xslResultAttr = new XSLTAttributeDef(Constants.S_XSLNAMESPACEURL, "*", XSLTAttributeDef.T_CDATA, false, false,XSLTAttributeDef.WARNING);

		XSLTElementDef[] templateElements = new XSLTElementDef[23];
		XSLTElementDef[] templateElementsAndParams = new XSLTElementDef[24];
		XSLTElementDef[] templateElementsAndSort = new XSLTElementDef[24];
		//exslt
		XSLTElementDef[] exsltFunctionElements = new XSLTElementDef[24];

		XSLTElementDef[] charTemplateElements = new XSLTElementDef[15];
		XSLTElementDef resultElement = new XSLTElementDef(this, null, "*", null, templateElements, new XSLTAttributeDef[]{spaceAttrLiteral, xslExcludeResultPrefixesAttr, xslExtensionElementPrefixesAttr, xslUseAttributeSetsAttr, xslVersionAttr, xslResultAttr, resultAttr}, new ProcessorLRE(), typeof(ElemLiteralResult), 20, true);
		XSLTElementDef unknownElement = new XSLTElementDef(this, "*", "unknown", null, templateElementsAndParams, new XSLTAttributeDef[]{xslExcludeResultPrefixesAttr, xslExtensionElementPrefixesAttr, xslUseAttributeSetsAttr, xslVersionAttr, xslResultAttr, resultAttr}, new ProcessorUnknown(), typeof(ElemUnknown), 20, true);
		XSLTElementDef xslValueOf = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "value-of", null, null, new XSLTAttributeDef[]{selectAttrRequired, disableOutputEscapingAttr}, new ProcessorTemplateElem(), typeof(ElemValueOf), 20, true);
		XSLTElementDef xslCopyOf = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "copy-of", null, null, new XSLTAttributeDef[]{selectAttrRequired}, new ProcessorTemplateElem(), typeof(ElemCopyOf), 20, true);
		XSLTElementDef xslNumber = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "number", null, null, new XSLTAttributeDef[]{levelAttr, countAttr, fromAttr, valueAttr, formatAttr, langAttr, letterValueAttr, groupingSeparatorAVT, groupingSizeAttr}, new ProcessorTemplateElem(), typeof(ElemNumber), 20, true);

		// <!-- xsl:sort cannot occur after any other elements or
		// any non-whitespace character -->
		XSLTElementDef xslSort = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "sort", null, null, new XSLTAttributeDef[]{selectAttrDefDot, langAttr, dataTypeAttr, orderAttr, caseOrderAttr}, new ProcessorTemplateElem(), typeof(ElemSort), 19, true);
		XSLTElementDef xslWithParam = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "with-param", null, templateElements, new XSLTAttributeDef[]{nameAttrRequired, selectAttrOpt}, new ProcessorTemplateElem(), typeof(ElemWithParam), 19, true);
		XSLTElementDef xslApplyTemplates = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "apply-templates", null, new XSLTElementDef[]{xslSort, xslWithParam}, new XSLTAttributeDef[]{selectAttrDefNode, modeAttr}, new ProcessorTemplateElem(), typeof(ElemApplyTemplates), 20, true);
		XSLTElementDef xslApplyImports = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "apply-imports", null, null, new XSLTAttributeDef[]{}, new ProcessorTemplateElem(), typeof(ElemApplyImport));
		XSLTElementDef xslForEach = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "for-each", null, templateElementsAndSort, new XSLTAttributeDef[]{selectAttrRequired, spaceAttr}, new ProcessorTemplateElem(), typeof(ElemForEach), true, false, true, 20, true);
		XSLTElementDef xslIf = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "if", null, templateElements, new XSLTAttributeDef[]{testAttrRequired, spaceAttr}, new ProcessorTemplateElem(), typeof(ElemIf), 20, true);
		XSLTElementDef xslWhen = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "when", null, templateElements, new XSLTAttributeDef[]{testAttrRequired, spaceAttr}, new ProcessorTemplateElem(), typeof(ElemWhen), false, true, 1, true);
		XSLTElementDef xslOtherwise = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "otherwise", null, templateElements, new XSLTAttributeDef[]{spaceAttr}, new ProcessorTemplateElem(), typeof(ElemOtherwise), false, false, 2, false);
		XSLTElementDef xslChoose = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "choose", null, new XSLTElementDef[]{xslWhen, xslOtherwise}, new XSLTAttributeDef[]{spaceAttr}, new ProcessorTemplateElem(), typeof(ElemChoose), true, false, true, 20, true);
		XSLTElementDef xslAttribute = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "attribute", null, charTemplateElements, new XSLTAttributeDef[]{nameAVTRequired, namespaceAVTOpt, spaceAttr}, new ProcessorTemplateElem(), typeof(ElemAttribute), 20, true);
		XSLTElementDef xslCallTemplate = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "call-template", null, new XSLTElementDef[]{xslWithParam}, new XSLTAttributeDef[]{nameAttrRequired}, new ProcessorTemplateElem(), typeof(ElemCallTemplate), 20, true);
		XSLTElementDef xslVariable = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "variable", null, templateElements, new XSLTAttributeDef[]{nameAttrRequired, selectAttrOpt}, new ProcessorTemplateElem(), typeof(ElemVariable), 20, true);
		XSLTElementDef xslParam = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "param", null, templateElements, new XSLTAttributeDef[]{nameAttrRequired, selectAttrOpt}, new ProcessorTemplateElem(), typeof(ElemParam), 19, true);
		XSLTElementDef xslText = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "text", null, new XSLTElementDef[]{charData}, new XSLTAttributeDef[]{disableOutputEscapingAttr}, new ProcessorText(), typeof(ElemText), 20, true);
		XSLTElementDef xslProcessingInstruction = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "processing-instruction", null, charTemplateElements, new XSLTAttributeDef[]{nameAVT_NCNAMERequired, spaceAttr}, new ProcessorTemplateElem(), typeof(ElemPI), 20, true);
		XSLTElementDef xslElement = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "element", null, templateElements, new XSLTAttributeDef[]{nameAVTRequired, namespaceAVTOpt, useAttributeSetsAttr, spaceAttr}, new ProcessorTemplateElem(), typeof(ElemElement), 20, true);
		XSLTElementDef xslComment = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "comment", null, charTemplateElements, new XSLTAttributeDef[]{spaceAttr}, new ProcessorTemplateElem(), typeof(ElemComment), 20, true);
		XSLTElementDef xslCopy = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "copy", null, templateElements, new XSLTAttributeDef[]{spaceAttr, useAttributeSetsAttr}, new ProcessorTemplateElem(), typeof(ElemCopy), 20, true);
		XSLTElementDef xslMessage = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "message", null, templateElements, new XSLTAttributeDef[]{terminateAttr}, new ProcessorTemplateElem(), typeof(ElemMessage), 20, true);
		XSLTElementDef xslFallback = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "fallback", null, templateElements, new XSLTAttributeDef[]{spaceAttr}, new ProcessorTemplateElem(), typeof(ElemFallback), 20, true);
		//exslt
		XSLTElementDef exsltFunction = new XSLTElementDef(this, Constants.S_EXSLT_FUNCTIONS_URL, "function", null, exsltFunctionElements, new XSLTAttributeDef[]{nameAttrRequired}, new ProcessorExsltFunction(), typeof(ElemExsltFunction));
		XSLTElementDef exsltResult = new XSLTElementDef(this, Constants.S_EXSLT_FUNCTIONS_URL, "result", null, templateElements, new XSLTAttributeDef[]{selectAttrOpt}, new ProcessorExsltFuncResult(), typeof(ElemExsltFuncResult));


		int i = 0;

		templateElements[i++] = charData; // #PCDATA

		// char-instructions
		templateElements[i++] = xslApplyTemplates;
		templateElements[i++] = xslCallTemplate;
		templateElements[i++] = xslApplyImports;
		templateElements[i++] = xslForEach;
		templateElements[i++] = xslValueOf;
		templateElements[i++] = xslCopyOf;
		templateElements[i++] = xslNumber;
		templateElements[i++] = xslChoose;
		templateElements[i++] = xslIf;
		templateElements[i++] = xslText;
		templateElements[i++] = xslCopy;
		templateElements[i++] = xslVariable;
		templateElements[i++] = xslMessage;
		templateElements[i++] = xslFallback;

		// instructions
		templateElements[i++] = xslProcessingInstruction;
		templateElements[i++] = xslComment;
		templateElements[i++] = xslElement;
		templateElements[i++] = xslAttribute;
		templateElements[i++] = resultElement;
		templateElements[i++] = unknownElement;
		templateElements[i++] = exsltFunction;
		templateElements[i++] = exsltResult;

		Array.Copy(templateElements, 0, templateElementsAndParams, 0, i);
		Array.Copy(templateElements, 0, templateElementsAndSort, 0, i);
		Array.Copy(templateElements, 0, exsltFunctionElements, 0, i);

		templateElementsAndParams[i] = xslParam;
		templateElementsAndSort[i] = xslSort;
		exsltFunctionElements[i] = xslParam;

		i = 0;
		charTemplateElements[i++] = charData; // #PCDATA

		// char-instructions
		charTemplateElements[i++] = xslApplyTemplates;
		charTemplateElements[i++] = xslCallTemplate;
		charTemplateElements[i++] = xslApplyImports;
		charTemplateElements[i++] = xslForEach;
		charTemplateElements[i++] = xslValueOf;
		charTemplateElements[i++] = xslCopyOf;
		charTemplateElements[i++] = xslNumber;
		charTemplateElements[i++] = xslChoose;
		charTemplateElements[i++] = xslIf;
		charTemplateElements[i++] = xslText;
		charTemplateElements[i++] = xslCopy;
		charTemplateElements[i++] = xslVariable;
		charTemplateElements[i++] = xslMessage;
		charTemplateElements[i++] = xslFallback;

		XSLTElementDef importDef = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "import", null, null, new XSLTAttributeDef[]{hrefAttr}, new ProcessorImport(), null, 1, true);
		XSLTElementDef includeDef = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "include", null, null, new XSLTAttributeDef[]{hrefAttr}, new ProcessorInclude(), null, 20, true);

		XSLTAttributeDef[] scriptAttrs = new XSLTAttributeDef[]
		{
			new XSLTAttributeDef(null, "lang", XSLTAttributeDef.T_NMTOKEN, true, false,XSLTAttributeDef.WARNING),
			new XSLTAttributeDef(null, "src", XSLTAttributeDef.T_URL, false, false,XSLTAttributeDef.WARNING)
		};

		XSLTAttributeDef[] componentAttrs = new XSLTAttributeDef[]
		{
			new XSLTAttributeDef(null, "prefix", XSLTAttributeDef.T_NMTOKEN, true, false,XSLTAttributeDef.WARNING),
			new XSLTAttributeDef(null, "elements", XSLTAttributeDef.T_STRINGLIST, false, false,XSLTAttributeDef.WARNING),
			new XSLTAttributeDef(null, "functions", XSLTAttributeDef.T_STRINGLIST, false, false,XSLTAttributeDef.WARNING)
		};

		XSLTElementDef[] topLevelElements = new XSLTElementDef[] {includeDef, importDef, whiteSpaceOnly, unknownElement, new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "strip-space", null, null, new XSLTAttributeDef[]{elementsAttr}, new ProcessorStripSpace(), null, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "preserve-space", null, null, new XSLTAttributeDef[]{elementsAttr}, new ProcessorPreserveSpace(), null, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "output", null, null, new XSLTAttributeDef[]{methodAttr, versionAttr, encodingAttr, omitXmlDeclarationAttr, standaloneAttr, doctypePublicAttr, doctypeSystemAttr, cdataSectionElementsAttr, indentAttr, mediaTypeAttr, XSLTAttributeDef.m_foreignAttr}, new ProcessorOutputElem(), null, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "key", null, null, new XSLTAttributeDef[]{nameAttrRequired, matchAttrRequired, useAttr}, new ProcessorKey(), null, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "decimal-format", null, null, new XSLTAttributeDef[]{nameAttrOpt_ERROR, decimalSeparatorAttr, groupingSeparatorAttr, infinityAttr, minusSignAttr, NaNAttr, percentAttr, perMilleAttr, zeroDigitAttr, digitAttr, patternSeparatorAttr}, new ProcessorDecimalFormat(), null, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "attribute-set", null, new XSLTElementDef[]{xslAttribute}, new XSLTAttributeDef[]{nameAttrRequired, useAttributeSetsAttr}, new ProcessorAttributeSet(), null, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "variable", null, templateElements, new XSLTAttributeDef[]{nameAttrRequired, selectAttrOpt}, new ProcessorGlobalVariableDecl(), typeof(ElemVariable), 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "param", null, templateElements, new XSLTAttributeDef[]{nameAttrRequired, selectAttrOpt}, new ProcessorGlobalParamDecl(), typeof(ElemParam), 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "template", null, templateElementsAndParams, new XSLTAttributeDef[]{matchAttrOpt, nameAttrOpt_ERROR, priorityAttr, modeAttr, spaceAttr}, new ProcessorTemplate(), typeof(ElemTemplate), true, 20, true), new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "namespace-alias", null, null, new XSLTAttributeDef[]{stylesheetPrefixAttr, resultPrefixAttr}, new ProcessorNamespaceAlias(), null, 20, true), new XSLTElementDef(this, Constants.S_BUILTIN_EXTENSIONS_URL, "component", null, new XSLTElementDef[]{new XSLTElementDef(this, Constants.S_BUILTIN_EXTENSIONS_URL, "script", null, new XSLTElementDef[]{charData}, scriptAttrs, new ProcessorLRE(), typeof(ElemExtensionScript), 20, true)}, componentAttrs, new ProcessorLRE(), typeof(ElemExtensionDecl)), new XSLTElementDef(this, Constants.S_BUILTIN_OLD_EXTENSIONS_URL, "component", null, new XSLTElementDef[]{new XSLTElementDef(this, Constants.S_BUILTIN_OLD_EXTENSIONS_URL, "script", null, new XSLTElementDef[]{charData}, scriptAttrs, new ProcessorLRE(), typeof(ElemExtensionScript), 20, true)}, componentAttrs, new ProcessorLRE(), typeof(ElemExtensionDecl)), exsltFunction};

		XSLTAttributeDef excludeResultPrefixesAttr = new XSLTAttributeDef(null, "exclude-result-prefixes", XSLTAttributeDef.T_PREFIXLIST, false,false,XSLTAttributeDef.WARNING);
		XSLTAttributeDef extensionElementPrefixesAttr = new XSLTAttributeDef(null, "extension-element-prefixes", XSLTAttributeDef.T_PREFIX_URLLIST, false,false,XSLTAttributeDef.WARNING);
		XSLTAttributeDef idAttr = new XSLTAttributeDef(null, "id", XSLTAttributeDef.T_CDATA, false,false,XSLTAttributeDef.WARNING);
		XSLTAttributeDef versionAttrRequired = new XSLTAttributeDef(null, "version", XSLTAttributeDef.T_NMTOKEN, true,false,XSLTAttributeDef.WARNING);
		XSLTElementDef stylesheetElemDef = new XSLTElementDef(this, Constants.S_XSLNAMESPACEURL, "stylesheet", "transform", topLevelElements, new XSLTAttributeDef[]{extensionElementPrefixesAttr, excludeResultPrefixesAttr, idAttr, versionAttrRequired, spaceAttr}, new ProcessorStylesheetElement(), null, true, -1, false);

		importDef.Elements = new XSLTElementDef[]{stylesheetElemDef, resultElement, unknownElement};
		includeDef.Elements = new XSLTElementDef[]{stylesheetElemDef, resultElement, unknownElement};
		build(null, null, null, new XSLTElementDef[]{stylesheetElemDef, whiteSpaceOnly, resultElement, unknownElement}, null, new ProcessorStylesheetDoc(), null);
	  }

	  /// <summary>
	  /// A hashtable of all available built-in elements for use by the element-available
	  /// function.
	  /// TODO:  When we convert to Java2, this should be a Set.
	  /// </summary>
	  private Hashtable m_availElems = new Hashtable();

	  /// <summary>
	  /// Get the table of available elements.
	  /// </summary>
	  /// <returns> table of available elements, keyed by qualified names, and with 
	  /// values of the same qualified names. </returns>
	  public virtual Hashtable ElemsAvailable
	  {
		  get
		  {
			return m_availElems;
		  }
	  }

	  /// <summary>
	  /// Adds a new element name to the Hashtable of available elements. </summary>
	  /// <param name="elemName"> The name of the element to add to the Hashtable of available elements. </param>
	  internal virtual void addAvailableElement(QName elemName)
	  {
		m_availElems[elemName] = elemName;
	  }

	  /// <summary>
	  /// Determines whether the passed element name is present in the list of available elements. </summary>
	  /// <param name="elemName"> The name of the element to look up.
	  /// </param>
	  /// <returns> true if an element corresponding to elemName is available. </returns>
	  public virtual bool elementAvailable(QName elemName)
	  {
		return m_availElems.ContainsKey(elemName);
	  }
	}


}