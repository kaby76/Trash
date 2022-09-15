using System;
using System.IO;
using System.Text;

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
 * $Id: Output.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using Encodings = org.apache.xml.serializer.Encodings;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Output : TopLevelElement
	{

		// TODO: use three-value variables for boolean values: true/false/default

		// These attributes are extracted from the xsl:output element. They also
		// appear as fields (with the same type, only public) in the translet
		private string _version;
		private string _method;
		private string _encoding;
		private bool _omitHeader = false;
		private string _standalone;
		private string _doctypePublic;
		private string _doctypeSystem;
		private string _cdata;
		private bool _indent = false;
		private string _mediaType;
		private string _indentamount;

		// Disables this output element (when other element has higher precedence)
		private bool _disabled = false;

		// Some global constants
		private const string STRING_SIG = "Ljava/lang/String;";
		private const string XML_VERSION = "1.0";
		private const string HTML_VERSION = "4.0";

		/// <summary>
		/// Displays the contents of this element (for debugging)
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("Output " + _method);
		}

		/// <summary>
		/// Disables this <xsl:output> element in case where there are some other
		/// <xsl:output> element (from a different imported/included stylesheet)
		/// with higher precedence.
		/// </summary>
		public void disable()
		{
		_disabled = true;
		}

		public bool enabled()
		{
		return !_disabled;
		}

		public string Cdata
		{
			get
			{
			return _cdata;
			}
		}

		public string OutputMethod
		{
			get
			{
				return _method;
			}
		}

		private void transferAttribute(Output previous, string qname)
		{
			if (!hasAttribute(qname) && previous.hasAttribute(qname))
			{
				addAttribute(qname, previous.getAttribute(qname));
			}
		}

		public void mergeOutput(Output previous)
		{
			// Transfer attributes from previous xsl:output
			transferAttribute(previous, "version");
			transferAttribute(previous, "method");
			transferAttribute(previous, "encoding");
			transferAttribute(previous, "doctype-system");
			transferAttribute(previous, "doctype-public");
			transferAttribute(previous, "media-type");
			transferAttribute(previous, "indent");
			transferAttribute(previous, "omit-xml-declaration");
			transferAttribute(previous, "standalone");

			// Merge cdata-section-elements
			if (previous.hasAttribute("cdata-section-elements"))
			{
				// addAttribute works as a setter if it already exists
				addAttribute("cdata-section-elements", previous.getAttribute("cdata-section-elements") + ' ' + getAttribute("cdata-section-elements"));
			}

			// Transfer non-standard attributes as well
			string prefix = lookupPrefix("http://xml.apache.org/xalan");
			if (!string.ReferenceEquals(prefix, null))
			{
				transferAttribute(previous, prefix + ':' + "indent-amount");
			}
			prefix = lookupPrefix("http://xml.apache.org/xslt");
			if (!string.ReferenceEquals(prefix, null))
			{
				transferAttribute(previous, prefix + ':' + "indent-amount");
			}
		}

		/// <summary>
		/// Scans the attribute list for the xsl:output instruction
		/// </summary>
		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Properties outputProperties = new java.util.Properties();
		Properties outputProperties = new Properties();

		// Ask the parser if it wants this <xsl:output> element
		parser.Output = this;

		// Do nothing if other <xsl:output> element has higher precedence
		if (_disabled)
		{
			return;
		}

		string attrib = null;

		// Get the output version
		_version = getAttribute("version");
		if (_version.Equals(Constants.EMPTYSTRING))
		{
			_version = null;
		}
		else
		{
			outputProperties.setProperty(OutputKeys.VERSION, _version);
		}

		// Get the output method - "xml", "html", "text" or <qname> (but not ncname)
		_method = getAttribute("method");
		if (_method.Equals(Constants.EMPTYSTRING))
		{
			_method = null;
		}
		if (!string.ReferenceEquals(_method, null))
		{
				_method = _method.ToLower();
				if ((_method.Equals("xml")) || (_method.Equals("html")) || (_method.Equals("text")) || ((XML11Char.isXML11ValidQName(_method) && (_method.IndexOf(":", StringComparison.Ordinal) > 0))))
				{
			   outputProperties.setProperty(OutputKeys.METHOD, _method);
				}
				else
				{
					reportError(this, parser, ErrorMsg.INVALID_METHOD_IN_OUTPUT, _method);
				}
		}

		// Get the output encoding - any value accepted here
		_encoding = getAttribute("encoding");
		if (_encoding.Equals(Constants.EMPTYSTRING))
		{
			_encoding = null;
		}
		else
		{
			try
			{
			// Create a write to verify encoding support
					string canonicalEncoding;
					canonicalEncoding = Encodings.convertMime2JavaEncoding(_encoding);
			StreamWriter writer = new StreamWriter(System.out, canonicalEncoding);
			}
			catch (java.io.UnsupportedEncodingException)
			{
			ErrorMsg msg = new ErrorMsg(ErrorMsg.UNSUPPORTED_ENCODING, _encoding, this);
			parser.reportError(Constants.WARNING, msg);
			}
			outputProperties.setProperty(OutputKeys.ENCODING, _encoding);
		}

		// Should the XML header be omitted - translate to true/false
		attrib = getAttribute("omit-xml-declaration");
		if (!attrib.Equals(Constants.EMPTYSTRING))
		{
			if (attrib.Equals("yes"))
			{
			_omitHeader = true;
			}
			outputProperties.setProperty(OutputKeys.OMIT_XML_DECLARATION, attrib);
		}

		// Add 'standalone' decaration to output - use text as is
		_standalone = getAttribute("standalone");
		if (_standalone.Equals(Constants.EMPTYSTRING))
		{
			_standalone = null;
		}
		else
		{
			outputProperties.setProperty(OutputKeys.STANDALONE, _standalone);
		}

		// Get system/public identifiers for output DOCTYPE declaration
		_doctypeSystem = getAttribute("doctype-system");
		if (_doctypeSystem.Equals(Constants.EMPTYSTRING))
		{
			_doctypeSystem = null;
		}
		else
		{
			outputProperties.setProperty(OutputKeys.DOCTYPE_SYSTEM, _doctypeSystem);
		}


		_doctypePublic = getAttribute("doctype-public");
		if (_doctypePublic.Equals(Constants.EMPTYSTRING))
		{
			_doctypePublic = null;
		}
		else
		{
			outputProperties.setProperty(OutputKeys.DOCTYPE_PUBLIC, _doctypePublic);
		}

		// Names the elements of whose text contents should be output as CDATA
		_cdata = getAttribute("cdata-section-elements");
		if (_cdata.Equals(Constants.EMPTYSTRING))
		{
			_cdata = null;
		}
		else
		{
			StringBuilder expandedNames = new StringBuilder();
			StringTokenizer tokens = new StringTokenizer(_cdata);

			// Make sure to store names in expanded form
			while (tokens.hasMoreTokens())
			{
					string qname = tokens.nextToken();
					if (!XML11Char.isXML11ValidQName(qname))
					{
						ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, qname, this);
						parser.reportError(Constants.ERROR, err);
					}
			expandedNames.Append(parser.getQName(qname).ToString()).Append(' ');
			}
			_cdata = expandedNames.ToString();
			outputProperties.setProperty(OutputKeys.CDATA_SECTION_ELEMENTS, _cdata);
		}

		// Get the indent setting - only has effect for xml and html output
		attrib = getAttribute("indent");
		if (!attrib.Equals(EMPTYSTRING))
		{
			if (attrib.Equals("yes"))
			{
			_indent = true;
			}
			outputProperties.setProperty(OutputKeys.INDENT, attrib);
		}
		else if (!string.ReferenceEquals(_method, null) && _method.Equals("html"))
		{
			_indent = true;
		}

			// indent-amount: extension attribute of xsl:output
			_indentamount = getAttribute(lookupPrefix("http://xml.apache.org/xalan"), "indent-amount");
			//  Hack for supporting Old Namespace URI.
			if (_indentamount.Equals(EMPTYSTRING))
			{
				_indentamount = getAttribute(lookupPrefix("http://xml.apache.org/xslt"), "indent-amount");
			}
			if (!_indentamount.Equals(EMPTYSTRING))
			{
				outputProperties.setProperty("indent_amount", _indentamount);
			}

		// Get the MIME type for the output file
		_mediaType = getAttribute("media-type");
		if (_mediaType.Equals(Constants.EMPTYSTRING))
		{
			_mediaType = null;
		}
		else
		{
			outputProperties.setProperty(OutputKeys.MEDIA_TYPE, _mediaType);
		}

		// Implied properties
		if (!string.ReferenceEquals(_method, null))
		{
			if (_method.Equals("html"))
			{
			if (string.ReferenceEquals(_version, null))
			{
				_version = HTML_VERSION;
			}
			if (string.ReferenceEquals(_mediaType, null))
			{
				_mediaType = "text/html";
			}
			}
			else if (_method.Equals("text"))
			{
			if (string.ReferenceEquals(_mediaType, null))
			{
				_mediaType = "text/plain";
			}
			}
		}

		// Set output properties in current stylesheet
		parser.CurrentStylesheet.setOutputProperties(outputProperties);
		}

		/// <summary>
		/// Compile code that passes the information in this <xsl:output> element
		/// to the appropriate fields in the translet
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

		// Do nothing if other <xsl:output> element has higher precedence
		if (_disabled)
		{
			return;
		}

		ConstantPoolGen cpg = classGen.getConstantPool();
		InstructionList il = methodGen.getInstructionList();

		int field = 0;
			il.append(classGen.loadTranslet());

		// Only update _version field if set and different from default
		if ((!string.ReferenceEquals(_version, null)) && (!_version.Equals(XML_VERSION)))
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_version", STRING_SIG);
			il.append(DUP);
			il.append(new PUSH(cpg, _version));
			il.append(new PUTFIELD(field));
		}

		// Only update _method field if "method" attribute used
		if (!string.ReferenceEquals(_method, null))
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_method", STRING_SIG);
			il.append(DUP);
			il.append(new PUSH(cpg, _method));
			il.append(new PUTFIELD(field));
		}

		// Only update if _encoding field is "encoding" attribute used
		if (!string.ReferenceEquals(_encoding, null))
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_encoding", STRING_SIG);
			il.append(DUP);
			il.append(new PUSH(cpg, _encoding));
			il.append(new PUTFIELD(field));
		}

		// Only update if "omit-xml-declaration" used and set to 'yes'
		if (_omitHeader)
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_omitHeader", "Z");
			il.append(DUP);
			il.append(new PUSH(cpg, _omitHeader));
			il.append(new PUTFIELD(field));
		}

		// Add 'standalone' decaration to output - use text as is
		if (!string.ReferenceEquals(_standalone, null))
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_standalone", STRING_SIG);
			il.append(DUP);
			il.append(new PUSH(cpg, _standalone));
			il.append(new PUTFIELD(field));
		}

		// Set system/public doctype only if both are set
		field = cpg.addFieldref(TRANSLET_CLASS,"_doctypeSystem",STRING_SIG);
		il.append(DUP);
		il.append(new PUSH(cpg, _doctypeSystem));
		il.append(new PUTFIELD(field));
		field = cpg.addFieldref(TRANSLET_CLASS,"_doctypePublic",STRING_SIG);
		il.append(DUP);
		il.append(new PUSH(cpg, _doctypePublic));
		il.append(new PUTFIELD(field));

		// Add 'medye-type' decaration to output - if used
		if (!string.ReferenceEquals(_mediaType, null))
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_mediaType", STRING_SIG);
			il.append(DUP);
			il.append(new PUSH(cpg, _mediaType));
			il.append(new PUTFIELD(field));
		}

		// Compile code to set output indentation on/off
		if (_indent)
		{
			field = cpg.addFieldref(TRANSLET_CLASS, "_indent", "Z");
			il.append(DUP);
			il.append(new PUSH(cpg, _indent));
			il.append(new PUTFIELD(field));
		}

			//Compile code to set indent amount.
			if (!string.ReferenceEquals(_indentamount, null) && !_indentamount.Equals(EMPTYSTRING))
			{
				field = cpg.addFieldref(TRANSLET_CLASS, "_indentamount", "I");
			il.append(DUP);
			il.append(new PUSH(cpg, int.Parse(_indentamount)));
			il.append(new PUTFIELD(field));
			}

		// Forward to the translet any elements that should be output as CDATA
		if (!string.ReferenceEquals(_cdata, null))
		{
			int index = cpg.addMethodref(TRANSLET_CLASS, "addCdataElement", "(Ljava/lang/String;)V");

			StringTokenizer tokens = new StringTokenizer(_cdata);
			while (tokens.hasMoreTokens())
			{
			il.append(DUP);
			il.append(new PUSH(cpg, tokens.nextToken()));
			il.append(new INVOKEVIRTUAL(index));
			}
		}
		il.append(POP); // Cleanup - pop last translet reference off stack
		}

	}

}