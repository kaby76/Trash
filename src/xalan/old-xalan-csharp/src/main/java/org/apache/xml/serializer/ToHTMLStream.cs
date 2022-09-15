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
 * $Id: ToHTMLStream.java 1225444 2011-12-29 05:52:39Z mrglavas $
 */
namespace org.apache.xml.serializer
{


	using MsgKey = org.apache.xml.serializer.utils.MsgKey;
	using Utils = org.apache.xml.serializer.utils.Utils;
	using Attributes = org.xml.sax.Attributes;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This serializer takes a series of SAX or
	/// SAX-like events and writes its output
	/// to the given stream.
	/// 
	/// This class is not a public API, it is public
	/// because it is used from another package.
	/// 
	/// @xsl.usage internal
	/// </summary>
	public class ToHTMLStream : ToStream
	{

		/// <summary>
		/// This flag is set while receiving events from the DTD </summary>
		protected internal bool m_inDTD = false;

		/// <summary>
		/// True if the current element is a block element.  (seems like 
		///  this needs to be a stack. -sb). 
		/// </summary>
		private bool m_inBlockElem = false;

		/// <summary>
		/// Map that tells which XML characters should have special treatment, and it
		///  provides character to entity name lookup.
		/// </summary>
		private readonly CharInfo m_htmlcharInfo = CharInfo.getCharInfo(CharInfo.HTML_ENTITIES_RESOURCE, Method.HTML);
	//        new CharInfo(CharInfo.HTML_ENTITIES_RESOURCE);

		/// <summary>
		/// A digital search trie for fast, case insensitive lookup of ElemDesc objects. </summary>
		internal static readonly Trie m_elementFlags = new Trie();

		static ToHTMLStream()
		{
			initTagReference(m_elementFlags);
		}
		internal static void initTagReference(Trie m_elementFlags)
		{

			// HTML 4.0 loose DTD
			m_elementFlags.put("BASEFONT", new ElemDesc(0 | ElemDesc.EMPTY));
			m_elementFlags.put("FRAME", new ElemDesc(0 | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("FRAMESET", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("NOFRAMES", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("ISINDEX", new ElemDesc(0 | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("APPLET", new ElemDesc(0 | ElemDesc.WHITESPACESENSITIVE));
			m_elementFlags.put("CENTER", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("DIR", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("MENU", new ElemDesc(0 | ElemDesc.BLOCK));

			// HTML 4.0 strict DTD
			m_elementFlags.put("TT", new ElemDesc(0 | ElemDesc.FONTSTYLE));
			m_elementFlags.put("I", new ElemDesc(0 | ElemDesc.FONTSTYLE));
			m_elementFlags.put("B", new ElemDesc(0 | ElemDesc.FONTSTYLE));
			m_elementFlags.put("BIG", new ElemDesc(0 | ElemDesc.FONTSTYLE));
			m_elementFlags.put("SMALL", new ElemDesc(0 | ElemDesc.FONTSTYLE));
			m_elementFlags.put("EM", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("STRONG", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("DFN", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("CODE", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("SAMP", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("KBD", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("VAR", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("CITE", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("ABBR", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("ACRONYM", new ElemDesc(0 | ElemDesc.PHRASE));
			m_elementFlags.put("SUP", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL));
			m_elementFlags.put("SUB", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL));
			m_elementFlags.put("SPAN", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL));
			m_elementFlags.put("BDO", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL));
			m_elementFlags.put("BR", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("BODY", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("ADDRESS", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("DIV", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("A", new ElemDesc(0 | ElemDesc.SPECIAL));
			m_elementFlags.put("MAP", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL | ElemDesc.BLOCK));
			m_elementFlags.put("AREA", new ElemDesc(0 | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("LINK", new ElemDesc(0 | ElemDesc.HEADMISC | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("IMG", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL | ElemDesc.EMPTY | ElemDesc.WHITESPACESENSITIVE));
			m_elementFlags.put("OBJECT", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL | ElemDesc.HEADMISC | ElemDesc.WHITESPACESENSITIVE));
			m_elementFlags.put("PARAM", new ElemDesc(0 | ElemDesc.EMPTY));
			m_elementFlags.put("HR", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET | ElemDesc.EMPTY));
			m_elementFlags.put("P", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("H1", new ElemDesc(0 | ElemDesc.HEAD | ElemDesc.BLOCK));
			m_elementFlags.put("H2", new ElemDesc(0 | ElemDesc.HEAD | ElemDesc.BLOCK));
			m_elementFlags.put("H3", new ElemDesc(0 | ElemDesc.HEAD | ElemDesc.BLOCK));
			m_elementFlags.put("H4", new ElemDesc(0 | ElemDesc.HEAD | ElemDesc.BLOCK));
			m_elementFlags.put("H5", new ElemDesc(0 | ElemDesc.HEAD | ElemDesc.BLOCK));
			m_elementFlags.put("H6", new ElemDesc(0 | ElemDesc.HEAD | ElemDesc.BLOCK));
			m_elementFlags.put("PRE", new ElemDesc(0 | ElemDesc.PREFORMATTED | ElemDesc.BLOCK));
			m_elementFlags.put("Q", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL));
			m_elementFlags.put("BLOCKQUOTE", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("INS", new ElemDesc(0));
			m_elementFlags.put("DEL", new ElemDesc(0));
			m_elementFlags.put("DL", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("DT", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("DD", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("OL", new ElemDesc(0 | ElemDesc.LIST | ElemDesc.BLOCK));
			m_elementFlags.put("UL", new ElemDesc(0 | ElemDesc.LIST | ElemDesc.BLOCK));
			m_elementFlags.put("LI", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("FORM", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("LABEL", new ElemDesc(0 | ElemDesc.FORMCTRL));
			m_elementFlags.put("INPUT", new ElemDesc(0 | ElemDesc.FORMCTRL | ElemDesc.INLINELABEL | ElemDesc.EMPTY));
			m_elementFlags.put("SELECT", new ElemDesc(0 | ElemDesc.FORMCTRL | ElemDesc.INLINELABEL));
			m_elementFlags.put("OPTGROUP", new ElemDesc(0));
			m_elementFlags.put("OPTION", new ElemDesc(0));
			m_elementFlags.put("TEXTAREA", new ElemDesc(0 | ElemDesc.FORMCTRL | ElemDesc.INLINELABEL));
			m_elementFlags.put("FIELDSET", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM));
			m_elementFlags.put("LEGEND", new ElemDesc(0));
			m_elementFlags.put("BUTTON", new ElemDesc(0 | ElemDesc.FORMCTRL | ElemDesc.INLINELABEL));
			m_elementFlags.put("TABLE", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("CAPTION", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("THEAD", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("TFOOT", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("TBODY", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("COLGROUP", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("COL", new ElemDesc(0 | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("TR", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("TH", new ElemDesc(0));
			m_elementFlags.put("TD", new ElemDesc(0));
			m_elementFlags.put("HEAD", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.HEADELEM));
			m_elementFlags.put("TITLE", new ElemDesc(0 | ElemDesc.BLOCK));
			m_elementFlags.put("BASE", new ElemDesc(0 | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("META", new ElemDesc(0 | ElemDesc.HEADMISC | ElemDesc.EMPTY | ElemDesc.BLOCK));
			m_elementFlags.put("STYLE", new ElemDesc(0 | ElemDesc.HEADMISC | ElemDesc.RAW | ElemDesc.BLOCK));
			m_elementFlags.put("SCRIPT", new ElemDesc(0 | ElemDesc.SPECIAL | ElemDesc.ASPECIAL | ElemDesc.HEADMISC | ElemDesc.RAW));
			m_elementFlags.put("NOSCRIPT", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			m_elementFlags.put("HTML", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.HTMLELEM));

			// From "John Ky" <hand@syd.speednet.com.au
			// Transitional Document Type Definition ()
			// file:///C:/Documents%20and%20Settings/sboag.BOAG600E/My%20Documents/html/sgml/loosedtd.html#basefont
			m_elementFlags.put("FONT", new ElemDesc(0 | ElemDesc.FONTSTYLE));

			// file:///C:/Documents%20and%20Settings/sboag.BOAG600E/My%20Documents/html/present/graphics.html#edef-STRIKE
			m_elementFlags.put("S", new ElemDesc(0 | ElemDesc.FONTSTYLE));
			m_elementFlags.put("STRIKE", new ElemDesc(0 | ElemDesc.FONTSTYLE));

			// file:///C:/Documents%20and%20Settings/sboag.BOAG600E/My%20Documents/html/present/graphics.html#edef-U
			m_elementFlags.put("U", new ElemDesc(0 | ElemDesc.FONTSTYLE));

			// From "John Ky" <hand@syd.speednet.com.au
			m_elementFlags.put("NOBR", new ElemDesc(0 | ElemDesc.FONTSTYLE));

			// HTML 4.0, section 16.5
			m_elementFlags.put("IFRAME", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));

			// Netscape 4 extension
			m_elementFlags.put("LAYER", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));
			// Netscape 4 extension                    
			m_elementFlags.put("ILAYER", new ElemDesc(0 | ElemDesc.BLOCK | ElemDesc.BLOCKFORM | ElemDesc.BLOCKFORMFIELDSET));

			// NOW FOR ATTRIBUTE INFORMATION . . .
			ElemDesc elemDesc;


			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("a");
			elemDesc.setAttr("HREF", ElemDesc.ATTRURL);
			elemDesc.setAttr("NAME", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("area");

			elemDesc.setAttr("HREF", ElemDesc.ATTRURL);
			elemDesc.setAttr("NOHREF", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("base");

			elemDesc.setAttr("HREF", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("button");
			elemDesc.setAttr("DISABLED", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("blockquote");

			elemDesc.setAttr("CITE", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("del");
			elemDesc.setAttr("CITE", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("dir");
			elemDesc.setAttr("COMPACT", ElemDesc.ATTREMPTY);

			// ----------------------------------------------

			elemDesc = (ElemDesc) m_elementFlags.get("div");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL); // Netscape 4 extension
			elemDesc.setAttr("NOWRAP", ElemDesc.ATTREMPTY); // Internet-Explorer extension

			// ----------------------------------------------        
			elemDesc = (ElemDesc) m_elementFlags.get("dl");
			elemDesc.setAttr("COMPACT", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("form");
			elemDesc.setAttr("ACTION", ElemDesc.ATTRURL);

			// ----------------------------------------------
			// Attribution to: "Voytenko, Dimitry" <DVoytenko@SECTORBASE.COM>
			elemDesc = (ElemDesc) m_elementFlags.get("frame");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);
			elemDesc.setAttr("LONGDESC", ElemDesc.ATTRURL);
			elemDesc.setAttr("NORESIZE",ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("head");
			elemDesc.setAttr("PROFILE", ElemDesc.ATTRURL);

			// ----------------------------------------------        
			elemDesc = (ElemDesc) m_elementFlags.get("hr");
			elemDesc.setAttr("NOSHADE", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			// HTML 4.0, section 16.5
			elemDesc = (ElemDesc) m_elementFlags.get("iframe");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);
			elemDesc.setAttr("LONGDESC", ElemDesc.ATTRURL);

			// ----------------------------------------------
			// Netscape 4 extension
			elemDesc = (ElemDesc) m_elementFlags.get("ilayer");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("img");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);
			elemDesc.setAttr("LONGDESC", ElemDesc.ATTRURL);
			elemDesc.setAttr("USEMAP", ElemDesc.ATTRURL);
			elemDesc.setAttr("ISMAP", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("input");

			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);
			elemDesc.setAttr("USEMAP", ElemDesc.ATTRURL);
			elemDesc.setAttr("CHECKED", ElemDesc.ATTREMPTY);
			elemDesc.setAttr("DISABLED", ElemDesc.ATTREMPTY);
			elemDesc.setAttr("ISMAP", ElemDesc.ATTREMPTY);
			elemDesc.setAttr("READONLY", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("ins");
			elemDesc.setAttr("CITE", ElemDesc.ATTRURL);

			// ----------------------------------------------
			// Netscape 4 extension
			elemDesc = (ElemDesc) m_elementFlags.get("layer");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("link");
			elemDesc.setAttr("HREF", ElemDesc.ATTRURL);

			// ----------------------------------------------       
			elemDesc = (ElemDesc) m_elementFlags.get("menu");
			elemDesc.setAttr("COMPACT", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("object");

			elemDesc.setAttr("CLASSID", ElemDesc.ATTRURL);
			elemDesc.setAttr("CODEBASE", ElemDesc.ATTRURL);
			elemDesc.setAttr("DATA", ElemDesc.ATTRURL);
			elemDesc.setAttr("ARCHIVE", ElemDesc.ATTRURL);
			elemDesc.setAttr("USEMAP", ElemDesc.ATTRURL);
			elemDesc.setAttr("DECLARE", ElemDesc.ATTREMPTY);

			// ----------------------------------------------        
			elemDesc = (ElemDesc) m_elementFlags.get("ol");
			elemDesc.setAttr("COMPACT", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("optgroup");
			elemDesc.setAttr("DISABLED", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("option");
			elemDesc.setAttr("SELECTED", ElemDesc.ATTREMPTY);
			elemDesc.setAttr("DISABLED", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("q");
			elemDesc.setAttr("CITE", ElemDesc.ATTRURL);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("script");
			elemDesc.setAttr("SRC", ElemDesc.ATTRURL);
			elemDesc.setAttr("FOR", ElemDesc.ATTRURL);
			elemDesc.setAttr("DEFER", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("select");
			elemDesc.setAttr("DISABLED", ElemDesc.ATTREMPTY);
			elemDesc.setAttr("MULTIPLE", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("table");
			elemDesc.setAttr("NOWRAP", ElemDesc.ATTREMPTY); // Internet-Explorer extension

			// ----------------------------------------------        
			elemDesc = (ElemDesc) m_elementFlags.get("td");
			elemDesc.setAttr("NOWRAP", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			elemDesc = (ElemDesc) m_elementFlags.get("textarea");
			elemDesc.setAttr("DISABLED", ElemDesc.ATTREMPTY);
			elemDesc.setAttr("READONLY", ElemDesc.ATTREMPTY);

			// ----------------------------------------------                
			elemDesc = (ElemDesc) m_elementFlags.get("th");
			elemDesc.setAttr("NOWRAP", ElemDesc.ATTREMPTY);

			// ----------------------------------------------
			// The nowrap attribute of a tr element is both
			// a Netscape and Internet-Explorer extension                
			elemDesc = (ElemDesc) m_elementFlags.get("tr");
			elemDesc.setAttr("NOWRAP", ElemDesc.ATTREMPTY);

			// ----------------------------------------------        
			elemDesc = (ElemDesc) m_elementFlags.get("ul");
			elemDesc.setAttr("COMPACT", ElemDesc.ATTREMPTY);
		}

		/// <summary>
		/// Dummy element for elements not found.
		/// </summary>
		private static readonly ElemDesc m_dummy = new ElemDesc(0 | ElemDesc.BLOCK);

		/// <summary>
		/// True if URLs should be specially escaped with the %xx form. </summary>
		private bool m_specialEscapeURLs = true;

		/// <summary>
		/// True if the META tag should be omitted. </summary>
		private bool m_omitMetaTag = false;

		/// <summary>
		/// Tells if the formatter should use special URL escaping.
		/// </summary>
		/// <param name="bool"> True if URLs should be specially escaped with the %xx form. </param>
		public virtual bool SpecialEscapeURLs
		{
			set
			{
				m_specialEscapeURLs = value;
			}
			get
			{
				return m_specialEscapeURLs;
			}
		}

		/// <summary>
		/// Tells if the formatter should omit the META tag.
		/// </summary>
		/// <param name="bool"> True if the META tag should be omitted. </param>
		public virtual bool OmitMetaTag
		{
			set
			{
				m_omitMetaTag = value;
			}
			get
			{
				return m_omitMetaTag;
			}
		}

		/// <summary>
		/// Specifies an output format for this serializer. It the
		/// serializer has already been associated with an output format,
		/// it will switch to the new format. This method should not be
		/// called while the serializer is in the process of serializing
		/// a document.
		/// 
		/// This method can be called multiple times before starting
		/// the serialization of a particular result-tree. In principle
		/// all serialization parameters can be changed, with the exception
		/// of method="html" (it must be method="html" otherwise we
		/// shouldn't even have a ToHTMLStream object here!) 
		/// </summary>
		/// <param name="format"> The output format or serialzation parameters
		/// to use. </param>
		public override Properties OutputFormat
		{
			set
			{
				/*
				 * If "format" does not contain the property
				 * S_USE_URL_ESCAPING, then don't set this value at all,
				 * just leave as-is rather than explicitly setting it.
				 */
				string value;
				value = value.getProperty(OutputPropertiesFactory.S_USE_URL_ESCAPING);
				if (!string.ReferenceEquals(value, null))
				{
					m_specialEscapeURLs = OutputPropertyUtils.getBooleanProperty(OutputPropertiesFactory.S_USE_URL_ESCAPING, value);
				}
    
				/*
				 * If "format" does not contain the property
				 * S_OMIT_META_TAG, then don't set this value at all,
				 * just leave as-is rather than explicitly setting it.
				 */
				value = value.getProperty(OutputPropertiesFactory.S_OMIT_META_TAG);
				if (!string.ReferenceEquals(value, null))
				{
				   m_omitMetaTag = OutputPropertyUtils.getBooleanProperty(OutputPropertiesFactory.S_OMIT_META_TAG, value);
				}
    
				base.OutputFormat = value;
			}
		}



		/// <summary>
		/// Get a description of the given element.
		/// </summary>
		/// <param name="name"> non-null name of element, case insensitive.
		/// </param>
		/// <returns> non-null reference to ElemDesc, which may be m_dummy if no 
		///         element description matches the given name. </returns>
		public static ElemDesc getElemDesc(string name)
		{
			/* this method used to return m_dummy  when name was null
			 * but now it doesn't check and and requires non-null name.
			 */
			object obj = m_elementFlags.get(name);
			if (null != obj)
			{
				return (ElemDesc)obj;
			}
			return m_dummy;
		}


		/// <summary>
		/// A Trie that is just a copy of the "static" one.
		/// We need this one to be able to use the faster, but not thread-safe
		/// method Trie.get2(name)
		/// </summary>
		private Trie m_htmlInfo = new Trie(m_elementFlags);
		/// <summary>
		/// Calls to this method could be replaced with calls to
		/// getElemDesc(name), but this one should be faster.
		/// </summary>
		private ElemDesc getElemDesc2(string name)
		{
			object obj = m_htmlInfo.get2(name);
			if (null != obj)
			{
				return (ElemDesc)obj;
			}
			return m_dummy;
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ToHTMLStream() : base()
		{

			// we are just constructing this thing, no output properties
			// have been used, so we will set the right default for
			// indenting anyways
			m_doIndent = true;
			m_charInfo = m_htmlcharInfo;
			// initialize namespaces
			m_prefixMap = new NamespaceMappings();

		}

		/// <summary>
		/// The name of the current element. </summary>
	//    private String m_currentElementName = null;

		/// <summary>
		/// Receive notification of the beginning of a document.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void startDocumentInternal() throws org.xml.sax.SAXException
		protected internal override void startDocumentInternal()
		{
			base.startDocumentInternal();

			m_needToCallStartDocument = false;
			m_needToOutputDocTypeDecl = true;
			m_startNewLine = false;
			OmitXMLDeclaration = true;
		}

		/// <summary>
		/// This method should only get called once.
		/// If a DOCTYPE declaration needs to get written out, it will
		/// be written out. If it doesn't need to be written out, then
		/// the call to this method has no effect.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void outputDocTypeDecl(String name) throws org.xml.sax.SAXException
		private void outputDocTypeDecl(string name)
		{
			if (true == m_needToOutputDocTypeDecl)
			{
				string doctypeSystem = DoctypeSystem;
				string doctypePublic = DoctypePublic;
				if ((null != doctypeSystem) || (null != doctypePublic))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
					java.io.Writer writer = m_writer;
					try
					{
					writer.write("<!DOCTYPE ");
					writer.write(name);

					if (null != doctypePublic)
					{
						writer.write(" PUBLIC \"");
						writer.write(doctypePublic);
						writer.write('"');
					}

					if (null != doctypeSystem)
					{
						if (null == doctypePublic)
						{
							writer.write(" SYSTEM \"");
						}
						else
						{
							writer.write(" \"");
						}

						writer.write(doctypeSystem);
						writer.write('"');
					}

					writer.write('>');
					outputLineSep();
					}
					catch (IOException e)
					{
						throw new SAXException(e);
					}
				}
			}

			m_needToOutputDocTypeDecl = false;
		}

		/// <summary>
		/// Receive notification of the end of a document. 
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void endDocument() throws org.xml.sax.SAXException
		public void endDocument()
		{

			flushPending();
			if (m_doIndent && !m_isprevtext)
			{
				try
				{
				outputLineSep();
				}
				catch (IOException e)
				{
					throw new SAXException(e);
				}
			}

			flushWriter();
			if (m_tracer != null)
			{
				base.fireEndDoc();
			}
		}

		/// <summary>
		///  Receive notification of the beginning of an element.
		/// 
		/// </summary>
		///  <param name="namespaceURI"> </param>
		///  <param name="localName"> </param>
		///  <param name="name"> The element type name. </param>
		///  <param name="atts"> The attributes attached to the element, if any. </param>
		///  <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///             wrapping another exception. </exception>
		///  <seealso cref= #endElement </seealso>
		///  <seealso cref= org.xml.sax.AttributeList </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startElement(String namespaceURI, String localName, String name, org.xml.sax.Attributes atts) throws org.xml.sax.SAXException
		public override void startElement(string namespaceURI, string localName, string name, Attributes atts)
		{

			ElemContext elemContext = m_elemContext;

			// clean up any pending things first
			if (elemContext.m_startTagOpen)
			{
				closeStartTag();
				elemContext.m_startTagOpen = false;
			}
			else if (m_cdataTagOpen)
			{
				closeCDATA();
				m_cdataTagOpen = false;
			}
			else if (m_needToCallStartDocument)
			{
				startDocumentInternal();
				m_needToCallStartDocument = false;
			}

			if (m_needToOutputDocTypeDecl)
			{
				string n = name;
				if (string.ReferenceEquals(n, null) || n.Length == 0)
				{
					// If the lexical QName is not given
					// use the localName in the DOCTYPE
					n = localName;
				}
				outputDocTypeDecl(n);
			}


			// if this element has a namespace then treat it like XML
			if (null != namespaceURI && namespaceURI.Length > 0)
			{
				base.startElement(namespaceURI, localName, name, atts);

				return;
			}

			try
			{
				// getElemDesc2(name) is faster than getElemDesc(name)
				ElemDesc elemDesc = getElemDesc2(name);
				int elemFlags = elemDesc.Flags;

				// deal with indentation issues first
				if (m_doIndent)
				{

					bool isBlockElement = (elemFlags & ElemDesc.BLOCK) != 0;
					if (m_ispreserve)
					{
						m_ispreserve = false;
					}
					else if ((null != elemContext.m_elementName) && (!m_inBlockElem || isBlockElement)) // && !isWhiteSpaceSensitive
					{
						m_startNewLine = true;

						indent();

					}
					m_inBlockElem = !isBlockElement;
				}

				// save any attributes for later processing
				if (atts != null)
				{
					addAttributes(atts);
				}

				m_isprevtext = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				java.io.Writer writer = m_writer;
				writer.write('<');
				writer.write(name);



				if (m_tracer != null)
				{
					firePseudoAttributes();
				}

				if ((elemFlags & ElemDesc.EMPTY) != 0)
				{
					// an optimization for elements which are expected
					// to be empty.
					m_elemContext = elemContext.push();
					/* XSLTC sometimes calls namespaceAfterStartElement()
					 * so we need to remember the name
					 */
					m_elemContext.m_elementName = name;
					m_elemContext.m_elementDesc = elemDesc;
					return;
				}
				else
				{
					elemContext = elemContext.push(namespaceURI,localName,name);
					m_elemContext = elemContext;
					elemContext.m_elementDesc = elemDesc;
					elemContext.m_isRaw = (elemFlags & ElemDesc.RAW) != 0;
				}


				if ((elemFlags & ElemDesc.HEADELEM) != 0)
				{
					// This is the <HEAD> element, do some special processing
					closeStartTag();
					elemContext.m_startTagOpen = false;
					if (!m_omitMetaTag)
					{
						if (m_doIndent)
						{
							indent();
						}
						writer.write("<META http-equiv=\"Content-Type\" content=\"text/html; charset=");
						string encoding = Encoding;
						string encode = Encodings.getMimeEncoding(encoding);
						writer.write(encode);
						writer.write("\">");
					}
				}
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}
		}

		/// <summary>
		///  Receive notification of the end of an element.
		/// 
		/// </summary>
		///  <param name="namespaceURI"> </param>
		///  <param name="localName"> </param>
		///  <param name="name"> The element type name </param>
		///  <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///             wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void endElement(final String namespaceURI, final String localName, final String name) throws org.xml.sax.SAXException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public sealed override void endElement(string namespaceURI, string localName, string name)
		{
			// deal with any pending issues
			if (m_cdataTagOpen)
			{
				closeCDATA();
			}

			// if the element has a namespace, treat it like XML, not HTML
			if (null != namespaceURI && namespaceURI.Length > 0)
			{
				base.endElement(namespaceURI, localName, name);

				return;
			}

			try
			{

				ElemContext elemContext = m_elemContext;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ElemDesc elemDesc = elemContext.m_elementDesc;
				ElemDesc elemDesc = elemContext.m_elementDesc;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int elemFlags = elemDesc.getFlags();
				int elemFlags = elemDesc.Flags;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean elemEmpty = (elemFlags & ElemDesc.EMPTY) != 0;
				bool elemEmpty = (elemFlags & ElemDesc.EMPTY) != 0;

				// deal with any indentation issues
				if (m_doIndent)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isBlockElement = (elemFlags&ElemDesc.BLOCK) != 0;
					bool isBlockElement = (elemFlags & ElemDesc.BLOCK) != 0;
					bool shouldIndent = false;

					if (m_ispreserve)
					{
						m_ispreserve = false;
					}
					else if (m_doIndent && (!m_inBlockElem || isBlockElement))
					{
						m_startNewLine = true;
						shouldIndent = true;
					}
					if (!elemContext.m_startTagOpen && shouldIndent)
					{
						indent(elemContext.m_currentElemDepth - 1);
					}
					m_inBlockElem = !isBlockElement;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				java.io.Writer writer = m_writer;
				if (!elemContext.m_startTagOpen)
				{
					writer.write("</");
					writer.write(name);
					writer.write('>');
				}
				else
				{
					// the start-tag open when this method was called,
					// so we need to process it now.

					if (m_tracer != null)
					{
						base.fireStartElem(name);
					}

					// the starting tag was still open when we received this endElement() call
					// so we need to process any gathered attributes NOW, before they go away.
					int nAttrs = m_attributes.Length;
					if (nAttrs > 0)
					{
						processAttributes(m_writer, nAttrs);
						// clear attributes object for re-use with next element
						m_attributes.clear();
					}
					if (!elemEmpty)
					{
						// As per Dave/Paul recommendation 12/06/2000
						// if (shouldIndent)
						// writer.write('>');
						//  indent(m_currentIndent);

						writer.write("></");
						writer.write(name);
						writer.write('>');
					}
					else
					{
						writer.write('>');
					}
				}

				// clean up because the element has ended
				if ((elemFlags & ElemDesc.WHITESPACESENSITIVE) != 0)
				{
					m_ispreserve = true;
				}
				m_isprevtext = false;

				// fire off the end element event
				if (m_tracer != null)
				{
					base.fireEndElem(name);
				}

				// OPTIMIZE-EMPTY                
				if (elemEmpty)
				{
					// a quick exit if the HTML element had no children.
					// This block of code can be removed if the corresponding block of code
					// in startElement() also labeled with "OPTIMIZE-EMPTY" is also removed
					m_elemContext = elemContext.m_prev;
					return;
				}

				// some more clean because the element has ended. 
				if (!elemContext.m_startTagOpen)
				{
					if (m_doIndent && !m_preserves.Empty)
					{
						m_preserves.pop();
					}
				}
				m_elemContext = elemContext.m_prev;
	//            m_isRawStack.pop();
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}
		}

		/// <summary>
		/// Process an attribute. </summary>
		/// <param name="writer"> The writer to write the processed output to. </param>
		/// <param name="name">   The name of the attribute. </param>
		/// <param name="value">   The value of the attribute. </param>
		/// <param name="elemDesc"> The description of the HTML element 
		///           that has this attribute.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void processAttribute(java.io.Writer writer, String name, String value, ElemDesc elemDesc) throws java.io.IOException
		protected internal virtual void processAttribute(java.io.Writer writer, string name, string value, ElemDesc elemDesc)
		{
			writer.write(' ');

			if (((value.Length == 0) || value.Equals(name, StringComparison.CurrentCultureIgnoreCase)) && elemDesc != null && elemDesc.isAttrFlagSet(name, ElemDesc.ATTREMPTY))
			{
				writer.write(name);
			}
			else
			{
				// %REVIEW% %OPT%
				// Two calls to single-char write may NOT
				// be more efficient than one to string-write...
				writer.write(name);
				writer.write("=\"");
				if (elemDesc != null && elemDesc.isAttrFlagSet(name, ElemDesc.ATTRURL))
				{
					writeAttrURI(writer, value, m_specialEscapeURLs);
				}
				else
				{
					writeAttrString(writer, value, this.Encoding);
				}
				writer.write('"');

			}
		}

		/// <summary>
		/// Tell if a character is an ASCII digit.
		/// </summary>
		private bool isASCIIDigit(char c)
		{
			return (c >= '0' && c <= '9');
		}

		/// <summary>
		/// Make an integer into an HH hex value.
		/// Does no checking on the size of the input, since this 
		/// is only meant to be used locally by writeAttrURI.
		/// </summary>
		/// <param name="i"> must be a value less than 255.
		/// </param>
		/// <returns> should be a two character string. </returns>
		private static string makeHHString(int i)
		{
			string s = i.ToString("x").ToUpper();
			if (s.Length == 1)
			{
				s = "0" + s;
			}
			return s;
		}

		/// <summary>
		/// Dmitri Ilyin: Makes sure if the String is HH encoded sign. </summary>
		/// <param name="str"> must be 2 characters long
		/// </param>
		/// <returns> true or false </returns>
		private bool isHHSign(string str)
		{
			bool sign = true;
			try
			{
				char r = (char)((char) Convert.ToInt32(str, 16));
			}
			catch (System.FormatException)
			{
				sign = false;
			}
			return sign;
		}

		/// <summary>
		/// Write the specified <var>string</var> after substituting non ASCII characters,
		/// with <CODE>%HH</CODE>, where HH is the hex of the byte value.
		/// </summary>
		/// <param name="string">      String to convert to XML format. </param>
		/// <param name="doURLEscaping"> True if we should try to encode as 
		///                      per http://www.ietf.org/rfc/rfc2396.txt.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> if a bad surrogate pair is detected. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeAttrURI(final java.io.Writer writer, String string, boolean doURLEscaping) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public virtual void writeAttrURI(java.io.Writer writer, string @string, bool doURLEscaping)
		{
			// http://www.ietf.org/rfc/rfc2396.txt says:
			// A URI is always in an "escaped" form, since escaping or unescaping a
			// completed URI might change its semantics.  Normally, the only time
			// escape encodings can safely be made is when the URI is being created
			// from its component parts; each component may have its own set of
			// characters that are reserved, so only the mechanism responsible for
			// generating or interpreting that component can determine whether or
			// not escaping a character will change its semantics. Likewise, a URI
			// must be separated into its components before the escaped characters
			// within those components can be safely decoded.
			//
			// ...So we do our best to do limited escaping of the URL, without 
			// causing damage.  If the URL is already properly escaped, in theory, this 
			// function should not change the string value.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = string.length();
			int end = @string.Length;
			if (end > m_attrBuff.Length)
			{
			   m_attrBuff = new char[end * 2 + 1];
			}
			@string.CopyTo(0, m_attrBuff, 0, end - 0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] chars = m_attrBuff;
			char[] chars = m_attrBuff;

			int cleanStart = 0;
			int cleanLength = 0;


			char ch = (char)0;
			for (int i = 0; i < end; i++)
			{
				ch = chars[i];

				if ((ch < 32) || (ch > 126))
				{
					if (cleanLength > 0)
					{
						writer.write(chars, cleanStart, cleanLength);
						cleanLength = 0;
					}
					if (doURLEscaping)
					{
						// Encode UTF16 to UTF8.
						// Reference is Unicode, A Primer, by Tony Graham.
						// Page 92.

						// Note that Kay doesn't escape 0x20...
						//  if(ch == 0x20) // Not sure about this... -sb
						//  {
						//    writer.write(ch);
						//  }
						//  else 
						if (ch <= 0x7F)
						{
							writer.write('%');
							writer.write(makeHHString(ch));
						}
						else if (ch <= 0x7FF)
						{
							// Clear low 6 bits before rotate, put high 4 bits in low byte, 
							// and set two high bits.
							int high = (ch >> 6) | 0xC0;
							int low = (ch & 0x3F) | 0x80;
							// First 6 bits, + high bit
							writer.write('%');
							writer.write(makeHHString(high));
							writer.write('%');
							writer.write(makeHHString(low));
						}
						else if (Encodings.isHighUTF16Surrogate(ch)) // high surrogate
						{
							// I'm sure this can be done in 3 instructions, but I choose 
							// to try and do it exactly like it is done in the book, at least 
							// until we are sure this is totally clean.  I don't think performance 
							// is a big issue with this particular function, though I could be 
							// wrong.  Also, the stuff below clearly does more masking than 
							// it needs to do.

							// Clear high 6 bits.
							int highSurrogate = ((int) ch) & 0x03FF;

							// Middle 4 bits (wwww) + 1
							// "Note that the value of wwww from the high surrogate bit pattern
							// is incremented to make the uuuuu bit pattern in the scalar value 
							// so the surrogate pair don't address the BMP."
							int wwww = ((highSurrogate & 0x03C0) >> 6);
							int uuuuu = wwww + 1;

							// next 4 bits
							int zzzz = (highSurrogate & 0x003C) >> 2;

							// low 2 bits
							int yyyyyy = ((highSurrogate & 0x0003) << 4) & 0x30;

							// Get low surrogate character.
							ch = chars[++i];

							// Clear high 6 bits.
							int lowSurrogate = ((int) ch) & 0x03FF;

							// put the middle 4 bits into the bottom of yyyyyy (byte 3)
							yyyyyy = yyyyyy | ((lowSurrogate & 0x03C0) >> 6);

							// bottom 6 bits.
							int xxxxxx = (lowSurrogate & 0x003F);

							int byte1 = 0xF0 | (uuuuu >> 2); // top 3 bits of uuuuu
							int byte2 = 0x80 | (((uuuuu & 0x03) << 4) & 0x30) | zzzz;
							int byte3 = 0x80 | yyyyyy;
							int byte4 = 0x80 | xxxxxx;

							writer.write('%');
							writer.write(makeHHString(byte1));
							writer.write('%');
							writer.write(makeHHString(byte2));
							writer.write('%');
							writer.write(makeHHString(byte3));
							writer.write('%');
							writer.write(makeHHString(byte4));
						}
						else
						{
							int high = (ch >> 12) | 0xE0; // top 4 bits
							int middle = ((ch & 0x0FC0) >> 6) | 0x80;
							// middle 6 bits
							int low = (ch & 0x3F) | 0x80;
							// First 6 bits, + high bit
							writer.write('%');
							writer.write(makeHHString(high));
							writer.write('%');
							writer.write(makeHHString(middle));
							writer.write('%');
							writer.write(makeHHString(low));
						}

					}
					else if (escapingNotNeeded(ch))
					{
						writer.write(ch);
					}
					else
					{
						writer.write("&#");
						writer.write(Convert.ToString(ch));
						writer.write(';');
					}
					// In this character range we have first written out any previously accumulated 
					// "clean" characters, then processed the current more complicated character,
					// which may have incremented "i".
					// We now we reset the next possible clean character.
					cleanStart = i + 1;
				}
				// Since http://www.ietf.org/rfc/rfc2396.txt refers to the URI grammar as
				// not allowing quotes in the URI proper syntax, nor in the fragment 
				// identifier, we believe that it's OK to double escape quotes.
				else if (ch == '"')
				{
					// If the character is a '%' number number, try to avoid double-escaping.
					// There is a question if this is legal behavior.

					// Dmitri Ilyin: to check if '%' number number is invalid. It must be checked if %xx is a sign, that would be encoded
					// The encoded signes are in Hex form. So %xx my be in form %3C that is "<" sign. I will try to change here a little.

					//        if( ((i+2) < len) && isASCIIDigit(stringArray[i+1]) && isASCIIDigit(stringArray[i+2]) )

					// We are no longer escaping '%'

					if (cleanLength > 0)
					{
						writer.write(chars, cleanStart, cleanLength);
						cleanLength = 0;
					}


					// Mike Kay encodes this as &#34;, so he may know something I don't?
					if (doURLEscaping)
					{
						writer.write("%22");
					}
					else
					{
						writer.write("&quot;"); // we have to escape this, I guess.
					}

					// We have written out any clean characters, then the escaped '%' and now we
					// We now we reset the next possible clean character.
					cleanStart = i + 1;
				}
				else if (ch == '&')
				{
					// HTML 4.01 reads, "Authors should use "&amp;" (ASCII decimal 38) 
					// instead of "&" to avoid confusion with the beginning of a character 
					// reference (entity reference open delimiter). 
					if (cleanLength > 0)
					{
						writer.write(chars, cleanStart, cleanLength);
						cleanLength = 0;
					}
					writer.write("&amp;");
					cleanStart = i + 1;
				}
				else
				{
					// no processing for this character, just count how
					// many characters in a row that we have that need no processing
					cleanLength++;
				}
			}

			// are there any clean characters at the end of the array
			// that we haven't processed yet?
			if (cleanLength > 1)
			{
				// if the whole string can be written out as-is do so
				// otherwise write out the clean chars at the end of the
				// array
				if (cleanStart == 0)
				{
					writer.write(@string);
				}
				else
				{
					writer.write(chars, cleanStart, cleanLength);
				}
			}
			else if (cleanLength == 1)
			{
				// a little optimization for 1 clean character
				// (we could have let the previous if(...) handle them all)
				writer.write(ch);
			}
		}

		/// <summary>
		/// Writes the specified <var>string</var> after substituting <VAR>specials</VAR>,
		/// and UTF-16 surrogates for character references <CODE>&amp;#xnn</CODE>.
		/// </summary>
		/// <param name="string">      String to convert to XML format. </param>
		/// <param name="encoding">    CURRENTLY NOT IMPLEMENTED.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeAttrString(final java.io.Writer writer, String string, String encoding) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		public override void writeAttrString(java.io.Writer writer, string @string, string encoding)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = string.length();
			int end = @string.Length;
			if (end > m_attrBuff.Length)
			{
				m_attrBuff = new char[end * 2 + 1];
			}
			@string.CopyTo(0, m_attrBuff, 0, end - 0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] chars = m_attrBuff;
			char[] chars = m_attrBuff;



			int cleanStart = 0;
			int cleanLength = 0;

			char ch = (char)0;
			for (int i = 0; i < end; i++)
			{
				ch = chars[i];

				// System.out.println("SPECIALSSIZE: "+SPECIALSSIZE);
				// System.out.println("ch: "+(int)ch);
				// System.out.println("m_maxCharacter: "+(int)m_maxCharacter);
				// System.out.println("m_attrCharsMap[ch]: "+(int)m_attrCharsMap[ch]);
				if (escapingNotNeeded(ch) && (!m_charInfo.shouldMapAttrChar(ch)))
				{
					cleanLength++;
				}
				else if ('<' == ch || '>' == ch)
				{
					cleanLength++; // no escaping in this case, as specified in 15.2
				}
				else if (('&' == ch) && ((i + 1) < end) && ('{' == chars[i + 1]))
				{
					cleanLength++; // no escaping in this case, as specified in 15.2
				}
				else
				{
					if (cleanLength > 0)
					{
						writer.write(chars,cleanStart,cleanLength);
						cleanLength = 0;
					}
					int pos = accumDefaultEntity(writer, ch, i, chars, end, false, true);

					if (i != pos)
					{
						i = pos - 1;
					}
					else
					{
						if (Encodings.isHighUTF16Surrogate(ch))
						{

								writeUTF16Surrogate(ch, chars, i, end);
								i++; // two input characters processed
									 // this increments by one and the for()
									 // loop itself increments by another one.
						}

						// The next is kind of a hack to keep from escaping in the case 
						// of Shift_JIS and the like.

						/*
						else if ((ch < m_maxCharacter) && (m_maxCharacter == 0xFFFF)
						&& (ch != 160))
						{
						writer.write(ch);  // no escaping in this case
						}
						else
						*/
						string outputStringForChar = m_charInfo.getOutputStringForChar(ch);
						if (null != outputStringForChar)
						{
							writer.write(outputStringForChar);
						}
						else if (escapingNotNeeded(ch))
						{
							writer.write(ch); // no escaping in this case
						}
						else
						{
							writer.write("&#");
							writer.write(Convert.ToString(ch));
							writer.write(';');
						}
					}
					cleanStart = i + 1;
				}
			} // end of for()

			// are there any clean characters at the end of the array
			// that we haven't processed yet?
			if (cleanLength > 1)
			{
				// if the whole string can be written out as-is do so
				// otherwise write out the clean chars at the end of the
				// array
				if (cleanStart == 0)
				{
					writer.write(@string);
				}
				else
				{
					writer.write(chars, cleanStart, cleanLength);
				}
			}
			else if (cleanLength == 1)
			{
				// a little optimization for 1 clean character
				// (we could have let the previous if(...) handle them all)
				writer.write(ch);
			}
		}



		/// <summary>
		/// Receive notification of character data.
		/// 
		/// <para>The Parser will call this method to report each chunk of
		/// character data.  SAX parsers may return all contiguous character
		/// data in a single chunk, or they may split it into several
		/// chunks; however, all of the characters in any single event
		/// must come from the same external entity, so that the Locator
		/// provides useful information.</para>
		/// 
		/// <para>The application must not attempt to read from the array
		/// outside of the specified range.</para>
		/// 
		/// <para>Note that some parsers will report whitespace using the
		/// ignorableWhitespace() method rather than this one (validating
		/// parsers must do so).</para>
		/// </summary>
		/// <param name="chars"> The characters from the XML document. </param>
		/// <param name="start"> The start position in the array. </param>
		/// <param name="length"> The number of characters to read from the array. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///            wrapping another exception. </exception>
		/// <seealso cref= #ignorableWhitespace </seealso>
		/// <seealso cref= org.xml.sax.Locator
		/// </seealso>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void characters(char chars[], int start, int length) throws org.xml.sax.SAXException
		public sealed override void characters(char[] chars, int start, int length)
		{

			if (m_elemContext.m_isRaw)
			{
				try
				{
					// Clean up some pending issues.
					if (m_elemContext.m_startTagOpen)
					{
						closeStartTag();
						m_elemContext.m_startTagOpen = false;
					}

					m_ispreserve = true;

					writeNormalizedChars(chars, start, length, false, m_lineSepUse);

					// time to generate characters event
					if (m_tracer != null)
					{
						base.fireCharEvent(chars, start, length);
					}

					return;
				}
				catch (IOException ioe)
				{
					throw new SAXException(Utils.messages.createMessage(MsgKey.ER_OIERROR,null),ioe);
				}
			}
			else
			{
				base.characters(chars, start, length);
			}
		}

		/// <summary>
		///  Receive notification of cdata.
		/// 
		///  <para>The Parser will call this method to report each chunk of
		///  character data.  SAX parsers may return all contiguous character
		///  data in a single chunk, or they may split it into several
		///  chunks; however, all of the characters in any single event
		///  must come from the same external entity, so that the Locator
		///  provides useful information.</para>
		/// 
		///  <para>The application must not attempt to read from the array
		///  outside of the specified range.</para>
		/// 
		///  <para>Note that some parsers will report whitespace using the
		///  ignorableWhitespace() method rather than this one (validating
		///  parsers must do so).</para>
		/// </summary>
		///  <param name="ch"> The characters from the XML document. </param>
		///  <param name="start"> The start position in the array. </param>
		///  <param name="length"> The number of characters to read from the array. </param>
		///  <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///             wrapping another exception. </exception>
		///  <seealso cref= #ignorableWhitespace </seealso>
		///  <seealso cref= org.xml.sax.Locator
		/// </seealso>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void cdata(char ch[], int start, int length) throws org.xml.sax.SAXException
		public sealed override void cdata(char[] ch, int start, int length)
		{

			if ((null != m_elemContext.m_elementName) && (m_elemContext.m_elementName.Equals("SCRIPT", StringComparison.CurrentCultureIgnoreCase) || m_elemContext.m_elementName.Equals("STYLE", StringComparison.CurrentCultureIgnoreCase)))
			{
				try
				{
					if (m_elemContext.m_startTagOpen)
					{
						closeStartTag();
						m_elemContext.m_startTagOpen = false;
					}

					m_ispreserve = true;

					if (shouldIndent())
					{
						indent();
					}

					// writer.write(ch, start, length);
					writeNormalizedChars(ch, start, length, true, m_lineSepUse);
				}
				catch (IOException ioe)
				{
					throw new SAXException(Utils.messages.createMessage(MsgKey.ER_OIERROR, null), ioe);
					//"IO error", ioe);
				}
			}
			else
			{
				base.cdata(ch, start, length);
			}
		}

		/// <summary>
		///  Receive notification of a processing instruction.
		/// </summary>
		///  <param name="target"> The processing instruction target. </param>
		///  <param name="data"> The processing instruction data, or null if
		///         none was supplied. </param>
		///  <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		///             wrapping another exception.
		/// </exception>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processingInstruction(String target, String data) throws org.xml.sax.SAXException
		public virtual void processingInstruction(string target, string data)
		{

			// Process any pending starDocument and startElement first.
			flushPending();

			// Use a fairly nasty hack to tell if the next node is supposed to be 
			// unescaped text.
			if (target.Equals(Result.PI_DISABLE_OUTPUT_ESCAPING))
			{
				startNonEscaping();
			}
			else if (target.Equals(Result.PI_ENABLE_OUTPUT_ESCAPING))
			{
				endNonEscaping();
			}
			else
			{
				try
				{
					// clean up any pending things first
					if (m_elemContext.m_startTagOpen)
					{
						closeStartTag();
						m_elemContext.m_startTagOpen = false;
					}
					else if (m_cdataTagOpen)
					{
						closeCDATA();
					}
					else if (m_needToCallStartDocument)
					{
						startDocumentInternal();
					}


				/*
				 * Perhaps processing instructions can be written out in HTML before
				 * the DOCTYPE, in which case this could be emitted with the
				 * startElement call, that knows the name of the document element
				 * doing it right.
				 */
				if (true == m_needToOutputDocTypeDecl)
				{
					outputDocTypeDecl("html"); // best guess for the upcoming element
				}


				if (shouldIndent())
				{
					indent();
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				java.io.Writer writer = m_writer;
				//writer.write("<?" + target);
				writer.write("<?");
				writer.write(target);

				if (data.Length > 0 && !Character.isSpaceChar(data[0]))
				{
					writer.write(' ');
				}

				//writer.write(data + ">"); // different from XML
				writer.write(data); // different from XML
				writer.write('>'); // different from XML

				// Always output a newline char if not inside of an 
				// element. The whitespace is not significant in that
				// case.
				if (m_elemContext.m_currentElemDepth <= 0)
				{
					outputLineSep();
				}

				m_startNewLine = true;
				}
				catch (IOException e)
				{
					throw new SAXException(e);
				}
			}

			// now generate the PI event
			if (m_tracer != null)
			{
				base.fireEscapingEvent(target, data);
			}
		}

		/// <summary>
		/// Receive notivication of a entityReference.
		/// </summary>
		/// <param name="name"> non-null reference to entity name string.
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void entityReference(String name) throws org.xml.sax.SAXException
		public sealed override void entityReference(string name)
		{
			try
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
			java.io.Writer writer = m_writer;
			writer.write('&');
			writer.write(name);
			writer.write(';');

			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}
		}
		/// <seealso cref= ExtendedContentHandler#endElement(String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void endElement(String elemName) throws org.xml.sax.SAXException
		public sealed override void endElement(string elemName)
		{
			endElement(null, null, elemName);
		}

		/// <summary>
		/// Process the attributes, which means to write out the currently
		/// collected attributes to the writer. The attributes are not
		/// cleared by this method
		/// </summary>
		/// <param name="writer"> the writer to write processed attributes to. </param>
		/// <param name="nAttrs"> the number of attributes in m_attributes 
		/// to be processed
		/// </param>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processAttributes(java.io.Writer writer, int nAttrs) throws java.io.IOException,org.xml.sax.SAXException
		public override void processAttributes(java.io.Writer writer, int nAttrs)
		{
				/* 
				 * process the collected attributes
				 */
				for (int i = 0; i < nAttrs; i++)
				{
					processAttribute(writer, m_attributes.getQName(i), m_attributes.getValue(i), m_elemContext.m_elementDesc);
				}
		}

		/// <summary>
		/// For the enclosing elements starting tag write out out any attributes
		/// followed by ">". At this point we also mark if this element is
		/// a cdata-section-element.
		/// </summary>
		/// <exception cref="org.xml.sax.SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void closeStartTag() throws org.xml.sax.SAXException
		protected internal override void closeStartTag()
		{
				try
				{

				// finish processing attributes, time to fire off the start element event
				if (m_tracer != null)
				{
					base.fireStartElem(m_elemContext.m_elementName);
				}

				int nAttrs = m_attributes.Length;
				if (nAttrs > 0)
				{
					processAttributes(m_writer, nAttrs);
					// clear attributes object for re-use with next element
					m_attributes.clear();
				}

				m_writer.write('>');

				/* At this point we have the prefix mappings now, so
				 * lets determine if the current element is specified in the cdata-
				 * section-elements list.
				 */
				if (m_CdataElems != null) // if there are any cdata sections
				{
					m_elemContext.m_isCdataSection = CdataSection;
				}
				if (m_doIndent)
				{
					m_isprevtext = false;
					m_preserves.push(m_ispreserve);
				}

				}
				catch (IOException e)
				{
					throw new SAXException(e);
				}
		}



			/// <summary>
			/// This method is used when a prefix/uri namespace mapping
			/// is indicated after the element was started with a
			/// startElement() and before and endElement().
			/// startPrefixMapping(prefix,uri) would be used before the
			/// startElement() call. </summary>
			/// <param name="uri"> the URI of the namespace </param>
			/// <param name="prefix"> the prefix associated with the given URI.
			/// </param>
			/// <seealso cref= ExtendedContentHandler#namespaceAfterStartElement(String, String) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void namespaceAfterStartElement(String prefix, String uri) throws org.xml.sax.SAXException
			public override void namespaceAfterStartElement(string prefix, string uri)
			{
				// hack for XSLTC with finding URI for default namespace
				if (string.ReferenceEquals(m_elemContext.m_elementURI, null))
				{
					string prefix1 = getPrefixPart(m_elemContext.m_elementName);
					if (string.ReferenceEquals(prefix1, null) && SerializerConstants_Fields.EMPTYSTRING.Equals(prefix))
					{
						// the elements URI is not known yet, and it
						// doesn't have a prefix, and we are currently
						// setting the uri for prefix "", so we have
						// the uri for the element... lets remember it
						m_elemContext.m_elementURI = uri;
					}
				}
				startPrefixMapping(prefix,uri,false);
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void startDTD(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public override void startDTD(string name, string publicId, string systemId)
		{
			m_inDTD = true;
			base.startDTD(name, publicId, systemId);
		}

		/// <summary>
		/// Report the end of DTD declarations. </summary>
		/// <exception cref="org.xml.sax.SAXException"> The application may raise an exception. </exception>
		/// <seealso cref= #startDTD </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void endDTD() throws org.xml.sax.SAXException
		public override void endDTD()
		{
			m_inDTD = false;
			/* for ToHTMLStream the DOCTYPE is entirely output in the
			 * startDocumentInternal() method, so don't do anything here
			 */
		}
		/// <summary>
		/// This method does nothing.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void attributeDecl(String eName, String aName, String type, String valueDefault, String value) throws org.xml.sax.SAXException
		public override void attributeDecl(string eName, string aName, string type, string valueDefault, string value)
		{
			// The internal DTD subset is not serialized by the ToHTMLStream serializer
		}

		/// <summary>
		/// This method does nothing.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void elementDecl(String name, String model) throws org.xml.sax.SAXException
		public override void elementDecl(string name, string model)
		{
			// The internal DTD subset is not serialized by the ToHTMLStream serializer
		}
		/// <summary>
		/// This method does nothing.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void internalEntityDecl(String name, String value) throws org.xml.sax.SAXException
		public override void internalEntityDecl(string name, string value)
		{
			// The internal DTD subset is not serialized by the ToHTMLStream serializer
		}
		/// <summary>
		/// This method does nothing.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void externalEntityDecl(String name, String publicId, String systemId) throws org.xml.sax.SAXException
		public override void externalEntityDecl(string name, string publicId, string systemId)
		{
			// The internal DTD subset is not serialized by the ToHTMLStream serializer
		}

		/// <summary>
		/// This method is used to add an attribute to the currently open element. 
		/// The caller has guaranted that this attribute is unique, which means that it
		/// not been seen before and will not be seen again.
		/// </summary>
		/// <param name="name"> the qualified name of the attribute </param>
		/// <param name="value"> the value of the attribute which can contain only
		/// ASCII printable characters characters in the range 32 to 127 inclusive. </param>
		/// <param name="flags"> the bit values of this integer give optimization information. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addUniqueAttribute(String name, String value, int flags) throws org.xml.sax.SAXException
		public override void addUniqueAttribute(string name, string value, int flags)
		{
			try
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.Writer writer = m_writer;
				java.io.Writer writer = m_writer;
				if ((flags & ExtendedContentHandler_Fields.NO_BAD_CHARS) > 0 && m_htmlcharInfo.onlyQuotAmpLtGt)
				{
					// "flags" has indicated that the characters
					// '>'  '<'   '&'  and '"' are not in the value and
					// m_htmlcharInfo has recorded that there are no other
					// entities in the range 0 to 127 so we write out the
					// value directly
					writer.write(' ');
					writer.write(name);
					writer.write("=\"");
					writer.write(value);
					writer.write('"');
				}
				else if ((flags & ExtendedContentHandler_Fields.HTML_ATTREMPTY) > 0 && (value.Length == 0 || value.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
				{
					writer.write(' ');
					writer.write(name);
				}
				else
				{
					writer.write(' ');
					writer.write(name);
					writer.write("=\"");
					if ((flags & ExtendedContentHandler_Fields.HTML_ATTRURL) > 0)
					{
						writeAttrURI(writer, value, m_specialEscapeURLs);
					}
					else
					{
						writeAttrString(writer, value, this.Encoding);
					}
					writer.write('"');
				}
			}
			catch (IOException e)
			{
				throw new SAXException(e);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(char ch[], int start, int length) throws org.xml.sax.SAXException
		public override void comment(char[] ch, int start, int length)
		{
			// The internal DTD subset is not serialized by the ToHTMLStream serializer
			if (m_inDTD)
			{
				return;
			}

			// Clean up some pending issues, just in case
			// this call is coming right after a startElement()
			// or we are in the middle of writing out CDATA
			// or if a startDocument() call was not received
			if (m_elemContext.m_startTagOpen)
			{
				closeStartTag();
				m_elemContext.m_startTagOpen = false;
			}
			else if (m_cdataTagOpen)
			{
				closeCDATA();
			}
			else if (m_needToCallStartDocument)
			{
				startDocumentInternal();
			}

			/*
			 * Perhaps comments can be written out in HTML before the DOCTYPE.
			 * In this case we might delete this call to writeOutDOCTYPE, and
			 * it would be handled within the startElement() call.
			 */
			if (m_needToOutputDocTypeDecl)
			{
				outputDocTypeDecl("html"); // best guess for the upcoming element
			}

			base.comment(ch, start, length);
		}

		public override bool reset()
		{
			bool ret = base.reset();
			if (!ret)
			{
				return false;
			}
			resetToHTMLStream();
			return true;
		}

		private void resetToHTMLStream()
		{
			// m_htmlcharInfo remains unchanged
			// m_htmlInfo = null;  // Don't reset
			m_inBlockElem = false;
			m_inDTD = false;
			m_omitMetaTag = false;
			m_specialEscapeURLs = true;
		}

		internal class Trie
		{
			/// <summary>
			/// A digital search trie for 7-bit ASCII text
			/// The API is a subset of java.util.Hashtable
			/// The key must be a 7-bit ASCII string
			/// The value may be any Java Object
			/// One can get an object stored in a trie from its key, 
			/// but the search is either case sensitive or case 
			/// insensitive to the characters in the key, and this
			/// choice of sensitivity or insensitivity is made when
			/// the Trie is created, before any objects are put in it.
			/// 
			/// This class is a copy of the one in org.apache.xml.utils. 
			/// It exists to cut the serializers dependancy on that package.
			/// 
			/// @xsl.usage internal
			/// </summary>

			/// <summary>
			/// Size of the m_nextChar array. </summary>
			public const int ALPHA_SIZE = 128;

			/// <summary>
			/// The root node of the tree. </summary>
			internal readonly Node m_Root;

			/// <summary>
			/// helper buffer to convert Strings to char arrays </summary>
			internal char[] m_charBuffer = new char[0];

			/// <summary>
			/// true if the search for an object is lower case only with the key </summary>
			internal readonly bool m_lowerCaseOnly;

			/// <summary>
			/// Construct the trie that has a case insensitive search.
			/// </summary>
			public Trie()
			{
				m_Root = new Node();
				m_lowerCaseOnly = false;
			}

			/// <summary>
			/// Construct the trie given the desired case sensitivity with the key. </summary>
			/// <param name="lowerCaseOnly"> true if the search keys are to be loser case only,
			/// not case insensitive. </param>
			public Trie(bool lowerCaseOnly)
			{
				m_Root = new Node();
				m_lowerCaseOnly = lowerCaseOnly;
			}

			/// <summary>
			/// Put an object into the trie for lookup.
			/// </summary>
			/// <param name="key"> must be a 7-bit ASCII string </param>
			/// <param name="value"> any java object.
			/// </param>
			/// <returns> The old object that matched key, or null. </returns>
			public virtual object put(string key, object value)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = key.length();
				int len = key.Length;
				if (len > m_charBuffer.Length)
				{
					// make the biggest buffer ever needed in get(String)
					m_charBuffer = new char[len];
				}

				Node node = m_Root;

				for (int i = 0; i < len; i++)
				{
					Node nextNode = node.m_nextChar[char.ToLower(key[i])];

					if (nextNode != null)
					{
						node = nextNode;
					}
					else
					{
						for (; i < len; i++)
						{
							Node newNode = new Node();
							if (m_lowerCaseOnly)
							{
								// put this value into the tree only with a lower case key 
								node.m_nextChar[char.ToLower(key[i])] = newNode;
							}
							else
							{
								// put this value into the tree with a case insensitive key
								node.m_nextChar[char.ToUpper(key[i])] = newNode;
								node.m_nextChar[char.ToLower(key[i])] = newNode;
							}
							node = newNode;
						}
						break;
					}
				}

				object ret = node.m_Value;

				node.m_Value = value;

				return ret;
			}

			/// <summary>
			/// Get an object that matches the key.
			/// </summary>
			/// <param name="key"> must be a 7-bit ASCII string
			/// </param>
			/// <returns> The object that matches the key, or null. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Object get(final String key)
			public virtual object get(string key)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = key.length();
				int len = key.Length;

				/* If the name is too long, we won't find it, this also keeps us
				 * from overflowing m_charBuffer
				 */
				if (m_charBuffer.Length < len)
				{
					return null;
				}

				Node node = m_Root;
				switch (len) // optimize the look up based on the number of chars
				{
					// case 0 looks silly, but the generated bytecode runs
					// faster for lookup of elements of length 2 with this in
					// and a fair bit faster.  Don't know why.
					case 0 :
					{
							return null;
					}

					case 1 :
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char ch = key.charAt(0);
							char ch = key[0];
							if (ch < ALPHA_SIZE)
							{
								node = node.m_nextChar[ch];
								if (node != null)
								{
									return node.m_Value;
								}
							}
							return null;
					}
						//                comment out case 2 because the default is faster            
						//                case 2 :
						//                    {
						//                        final char ch0 = key.charAt(0);
						//                        final char ch1 = key.charAt(1);
						//                        if (ch0 < ALPHA_SIZE && ch1 < ALPHA_SIZE)
						//                        {
						//                            node = node.m_nextChar[ch0];
						//                            if (node != null)
						//                            {
						//                        
						//                                if (ch1 < ALPHA_SIZE) 
						//                                {
						//                                    node = node.m_nextChar[ch1];
						//                                    if (node != null)
						//                                        return node.m_Value;
						//                                }
						//                            }
						//                        }
						//                        return null;
						//                   }
					default :
					{
							for (int i = 0; i < len; i++)
							{
								// A thread-safe way to loop over the characters
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char ch = key.charAt(i);
								char ch = key[i];
								if (ALPHA_SIZE <= ch)
								{
									// the key is not 7-bit ASCII so we won't find it here
									return null;
								}

								node = node.m_nextChar[ch];
								if (node == null)
								{
									return null;
								}
							}

							return node.m_Value;
					}
				}
			}

			/// <summary>
			/// The node representation for the trie.
			/// @xsl.usage internal
			/// </summary>
			private class Node
			{

				/// <summary>
				/// Constructor, creates a Node[ALPHA_SIZE].
				/// </summary>
				internal Node()
				{
					m_nextChar = new Node[ALPHA_SIZE];
					m_Value = null;
				}

				/// <summary>
				/// The next nodes. </summary>
				internal readonly Node[] m_nextChar;

				/// <summary>
				/// The value. </summary>
				internal object m_Value;
			}
			/// <summary>
			/// Construct the trie from another Trie.
			/// Both the existing Trie and this new one share the same table for
			/// lookup, and it is assumed that the table is fully populated and
			/// not changing anymore.
			/// </summary>
			/// <param name="existingTrie"> the Trie that this one is a copy of. </param>
			public Trie(Trie existingTrie)
			{
				// copy some fields from the existing Trie into this one.
				m_Root = existingTrie.m_Root;
				m_lowerCaseOnly = existingTrie.m_lowerCaseOnly;

				// get a buffer just big enough to hold the longest key in the table.
				int max = existingTrie.LongestKeyLength;
				m_charBuffer = new char[max];
			}

			/// <summary>
			/// Get an object that matches the key.
			/// This method is faster than get(), but is not thread-safe.
			/// </summary>
			/// <param name="key"> must be a 7-bit ASCII string
			/// </param>
			/// <returns> The object that matches the key, or null. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Object get2(final String key)
			public virtual object get2(string key)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = key.length();
				int len = key.Length;

				/* If the name is too long, we won't find it, this also keeps us
				 * from overflowing m_charBuffer
				 */
				if (m_charBuffer.Length < len)
				{
					return null;
				}

				Node node = m_Root;
				switch (len) // optimize the look up based on the number of chars
				{
					// case 0 looks silly, but the generated bytecode runs
					// faster for lookup of elements of length 2 with this in
					// and a fair bit faster.  Don't know why.
					case 0 :
					{
							return null;
					}

					case 1 :
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char ch = key.charAt(0);
							char ch = key[0];
							if (ch < ALPHA_SIZE)
							{
								node = node.m_nextChar[ch];
								if (node != null)
								{
									return node.m_Value;
								}
							}
							return null;
					}
					default :
					{
							/* Copy string into array. This is not thread-safe because
							 * it modifies the contents of m_charBuffer. If multiple
							 * threads were to use this Trie they all would be
							 * using this same array (not good). So this 
							 * method is not thread-safe, but it is faster because
							 * converting to a char[] and looping over elements of
							 * the array is faster than a String's charAt(i).
							 */
							key.CopyTo(0, m_charBuffer, 0, len - 0);

							for (int i = 0; i < len; i++)
							{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char ch = m_charBuffer[i];
								char ch = m_charBuffer[i];
								if (ALPHA_SIZE <= ch)
								{
									// the key is not 7-bit ASCII so we won't find it here
									return null;
								}

								node = node.m_nextChar[ch];
								if (node == null)
								{
									return null;
								}
							}

							return node.m_Value;
					}
				}
			}

			/// <summary>
			/// Get the length of the longest key used in the table. 
			/// </summary>
			public virtual int LongestKeyLength
			{
				get
				{
					return m_charBuffer.Length;
				}
			}
		}
	}

}