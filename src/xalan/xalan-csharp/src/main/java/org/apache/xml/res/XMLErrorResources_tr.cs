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
 * $Id: XMLErrorResources_tr.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.res
{


	/// <summary>
	/// Set up error messages.
	/// We build a two dimensional array of message keys and
	/// message strings. In order to add a new message here,
	/// you need to first add a String constant. And you need
	/// to enter key, value pair as part of the contents
	/// array. You also need to update MAX_CODE for error strings
	/// and MAX_WARNING for warnings ( Needed for only information
	/// purpose )
	/// </summary>
	public class XMLErrorResources_tr : ListResourceBundle
	{

	/*
	 * This file contains error and warning messages related to Xalan Error
	 * Handling.
	 *
	 *  General notes to translators:
	 *
	 *  1) Xalan (or more properly, Xalan-interpretive) and XSLTC are names of
	 *     components.
	 *     XSLT is an acronym for "XML Stylesheet Language: Transformations".
	 *     XSLTC is an acronym for XSLT Compiler.
	 *
	 *  2) A stylesheet is a description of how to transform an input XML document
	 *     into a resultant XML document (or HTML document or text).  The
	 *     stylesheet itself is described in the form of an XML document.
	 *
	 *  3) A template is a component of a stylesheet that is used to match a
	 *     particular portion of an input document and specifies the form of the
	 *     corresponding portion of the output document.
	 *
	 *  4) An element is a mark-up tag in an XML document; an attribute is a
	 *     modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *     "elem" is an element name, "attr" and "attr2" are attribute names with
	 *     the values "val" and "val2", respectively.
	 *
	 *  5) A namespace declaration is a special attribute that is used to associate
	 *     a prefix with a URI (the namespace).  The meanings of element names and
	 *     attribute names that use that prefix are defined with respect to that
	 *     namespace.
	 *
	 *  6) "Translet" is an invented term that describes the class file that
	 *     results from compiling an XML stylesheet into a Java class.
	 *
	 *  7) XPath is a specification that describes a notation for identifying
	 *     nodes in a tree-structured representation of an XML document.  An
	 *     instance of that notation is referred to as an XPath expression.
	 *
	 */

	  /*
	   * Message keys
	   */
	  public const string ER_FUNCTION_NOT_SUPPORTED = "ER_FUNCTION_NOT_SUPPORTED";
	  public const string ER_CANNOT_OVERWRITE_CAUSE = "ER_CANNOT_OVERWRITE_CAUSE";
	  public const string ER_NO_DEFAULT_IMPL = "ER_NO_DEFAULT_IMPL";
	  public const string ER_CHUNKEDINTARRAY_NOT_SUPPORTED = "ER_CHUNKEDINTARRAY_NOT_SUPPORTED";
	  public const string ER_OFFSET_BIGGER_THAN_SLOT = "ER_OFFSET_BIGGER_THAN_SLOT";
	  public const string ER_COROUTINE_NOT_AVAIL = "ER_COROUTINE_NOT_AVAIL";
	  public const string ER_COROUTINE_CO_EXIT = "ER_COROUTINE_CO_EXIT";
	  public const string ER_COJOINROUTINESET_FAILED = "ER_COJOINROUTINESET_FAILED";
	  public const string ER_COROUTINE_PARAM = "ER_COROUTINE_PARAM";
	  public const string ER_PARSER_DOTERMINATE_ANSWERS = "ER_PARSER_DOTERMINATE_ANSWERS";
	  public const string ER_NO_PARSE_CALL_WHILE_PARSING = "ER_NO_PARSE_CALL_WHILE_PARSING";
	  public const string ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED = "ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED";
	  public const string ER_ITERATOR_AXIS_NOT_IMPLEMENTED = "ER_ITERATOR_AXIS_NOT_IMPLEMENTED";
	  public const string ER_ITERATOR_CLONE_NOT_SUPPORTED = "ER_ITERATOR_CLONE_NOT_SUPPORTED";
	  public const string ER_UNKNOWN_AXIS_TYPE = "ER_UNKNOWN_AXIS_TYPE";
	  public const string ER_AXIS_NOT_SUPPORTED = "ER_AXIS_NOT_SUPPORTED";
	  public const string ER_NO_DTMIDS_AVAIL = "ER_NO_DTMIDS_AVAIL";
	  public const string ER_NOT_SUPPORTED = "ER_NOT_SUPPORTED";
	  public const string ER_NODE_NON_NULL = "ER_NODE_NON_NULL";
	  public const string ER_COULD_NOT_RESOLVE_NODE = "ER_COULD_NOT_RESOLVE_NODE";
	  public const string ER_STARTPARSE_WHILE_PARSING = "ER_STARTPARSE_WHILE_PARSING";
	  public const string ER_STARTPARSE_NEEDS_SAXPARSER = "ER_STARTPARSE_NEEDS_SAXPARSER";
	  public const string ER_COULD_NOT_INIT_PARSER = "ER_COULD_NOT_INIT_PARSER";
	  public const string ER_EXCEPTION_CREATING_POOL = "ER_EXCEPTION_CREATING_POOL";
	  public const string ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE = "ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE";
	  public const string ER_SCHEME_REQUIRED = "ER_SCHEME_REQUIRED";
	  public const string ER_NO_SCHEME_IN_URI = "ER_NO_SCHEME_IN_URI";
	  public const string ER_NO_SCHEME_INURI = "ER_NO_SCHEME_INURI";
	  public const string ER_PATH_INVALID_CHAR = "ER_PATH_INVALID_CHAR";
	  public const string ER_SCHEME_FROM_NULL_STRING = "ER_SCHEME_FROM_NULL_STRING";
	  public const string ER_SCHEME_NOT_CONFORMANT = "ER_SCHEME_NOT_CONFORMANT";
	  public const string ER_HOST_ADDRESS_NOT_WELLFORMED = "ER_HOST_ADDRESS_NOT_WELLFORMED";
	  public const string ER_PORT_WHEN_HOST_NULL = "ER_PORT_WHEN_HOST_NULL";
	  public const string ER_INVALID_PORT = "ER_INVALID_PORT";
	  public const string ER_FRAG_FOR_GENERIC_URI = "ER_FRAG_FOR_GENERIC_URI";
	  public const string ER_FRAG_WHEN_PATH_NULL = "ER_FRAG_WHEN_PATH_NULL";
	  public const string ER_FRAG_INVALID_CHAR = "ER_FRAG_INVALID_CHAR";
	  public const string ER_PARSER_IN_USE = "ER_PARSER_IN_USE";
	  public const string ER_CANNOT_CHANGE_WHILE_PARSING = "ER_CANNOT_CHANGE_WHILE_PARSING";
	  public const string ER_SELF_CAUSATION_NOT_PERMITTED = "ER_SELF_CAUSATION_NOT_PERMITTED";
	  public const string ER_NO_USERINFO_IF_NO_HOST = "ER_NO_USERINFO_IF_NO_HOST";
	  public const string ER_NO_PORT_IF_NO_HOST = "ER_NO_PORT_IF_NO_HOST";
	  public const string ER_NO_QUERY_STRING_IN_PATH = "ER_NO_QUERY_STRING_IN_PATH";
	  public const string ER_NO_FRAGMENT_STRING_IN_PATH = "ER_NO_FRAGMENT_STRING_IN_PATH";
	  public const string ER_CANNOT_INIT_URI_EMPTY_PARMS = "ER_CANNOT_INIT_URI_EMPTY_PARMS";
	  public const string ER_METHOD_NOT_SUPPORTED = "ER_METHOD_NOT_SUPPORTED";
	  public const string ER_INCRSAXSRCFILTER_NOT_RESTARTABLE = "ER_INCRSAXSRCFILTER_NOT_RESTARTABLE";
	  public const string ER_XMLRDR_NOT_BEFORE_STARTPARSE = "ER_XMLRDR_NOT_BEFORE_STARTPARSE";
	  public const string ER_AXIS_TRAVERSER_NOT_SUPPORTED = "ER_AXIS_TRAVERSER_NOT_SUPPORTED";
	  public const string ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER = "ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER";
	  public const string ER_SYSTEMID_UNKNOWN = "ER_SYSTEMID_UNKNOWN";
	  public const string ER_LOCATION_UNKNOWN = "ER_LOCATION_UNKNOWN";
	  public const string ER_PREFIX_MUST_RESOLVE = "ER_PREFIX_MUST_RESOLVE";
	  public const string ER_CREATEDOCUMENT_NOT_SUPPORTED = "ER_CREATEDOCUMENT_NOT_SUPPORTED";
	  public const string ER_CHILD_HAS_NO_OWNER_DOCUMENT = "ER_CHILD_HAS_NO_OWNER_DOCUMENT";
	  public const string ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT = "ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT";
	  public const string ER_CANT_OUTPUT_TEXT_BEFORE_DOC = "ER_CANT_OUTPUT_TEXT_BEFORE_DOC";
	  public const string ER_CANT_HAVE_MORE_THAN_ONE_ROOT = "ER_CANT_HAVE_MORE_THAN_ONE_ROOT";
	  public const string ER_ARG_LOCALNAME_NULL = "ER_ARG_LOCALNAME_NULL";
	  public const string ER_ARG_LOCALNAME_INVALID = "ER_ARG_LOCALNAME_INVALID";
	  public const string ER_ARG_PREFIX_INVALID = "ER_ARG_PREFIX_INVALID";
	  public const string ER_NAME_CANT_START_WITH_COLON = "ER_NAME_CANT_START_WITH_COLON";

	  /*
	   * Now fill in the message text.
	   * Then fill in the message text for that message code in the
	   * array. Use the new error code as the index into the array.
	   */

	  // Error messages...

	  /// <summary>
	  /// Get the lookup table for error messages
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public virtual object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ER0000", "{0}"},
				new object[] {ER_FUNCTION_NOT_SUPPORTED, "\u0130\u015flev desteklenmiyor!"},
				new object[] {ER_CANNOT_OVERWRITE_CAUSE, "Nedenin \u00fczerine yaz\u0131lamaz"},
				new object[] {ER_NO_DEFAULT_IMPL, "Varsay\u0131lan uygulama bulunamad\u0131 "},
				new object[] {ER_CHUNKEDINTARRAY_NOT_SUPPORTED, "ChunkedIntArray({0}) \u015fu an desteklenmiyor"},
				new object[] {ER_OFFSET_BIGGER_THAN_SLOT, "G\u00f6reli konum yuvadan b\u00fcy\u00fck"},
				new object[] {ER_COROUTINE_NOT_AVAIL, "Coroutine kullan\u0131lam\u0131yor, id={0}"},
				new object[] {ER_COROUTINE_CO_EXIT, "CoroutineManager co_exit() iste\u011fi ald\u0131"},
				new object[] {ER_COJOINROUTINESET_FAILED, "co_joinCoroutineSet() ba\u015far\u0131s\u0131z oldu"},
				new object[] {ER_COROUTINE_PARAM, "Coroutine de\u011fi\u015ftirgesi hatas\u0131 ({0})"},
				new object[] {ER_PARSER_DOTERMINATE_ANSWERS, "\nBEKLENMEYEN: Parser doTerminate yan\u0131t\u0131 {0}"},
				new object[] {ER_NO_PARSE_CALL_WHILE_PARSING, "Ayr\u0131\u015ft\u0131rma s\u0131ras\u0131nda parse \u00e7a\u011fr\u0131lamaz"},
				new object[] {ER_TYPED_ITERATOR_AXIS_NOT_IMPLEMENTED, "Hata: {0} ekseni i\u00e7in tip atanm\u0131\u015f yineleyici ger\u00e7ekle\u015ftirilmedi"},
				new object[] {ER_ITERATOR_AXIS_NOT_IMPLEMENTED, "Hata: {0} ekseni i\u00e7in yineleyici ger\u00e7ekle\u015ftirilmedi "},
				new object[] {ER_ITERATOR_CLONE_NOT_SUPPORTED, "Yineleyici e\u015fkopyas\u0131 desteklenmiyor"},
				new object[] {ER_UNKNOWN_AXIS_TYPE, "Bilinmeyen eksen dola\u015fma tipi: {0}"},
				new object[] {ER_AXIS_NOT_SUPPORTED, "Eksen dola\u015f\u0131c\u0131 desteklenmiyor: {0}"},
				new object[] {ER_NO_DTMIDS_AVAIL, "Kullan\u0131labilecek ba\u015fka DTM tan\u0131t\u0131c\u0131s\u0131 yok"},
				new object[] {ER_NOT_SUPPORTED, "Desteklenmiyor: {0}"},
				new object[] {ER_NODE_NON_NULL, "getDTMHandleFromNode i\u00e7in d\u00fc\u011f\u00fcm bo\u015f de\u011ferli olmamal\u0131d\u0131r"},
				new object[] {ER_COULD_NOT_RESOLVE_NODE, "D\u00fc\u011f\u00fcm tan\u0131t\u0131c\u0131 de\u011fere \u00e7\u00f6z\u00fclemedi"},
				new object[] {ER_STARTPARSE_WHILE_PARSING, "Ayr\u0131\u015ft\u0131rma s\u0131ras\u0131nda startParse \u00e7a\u011fr\u0131lamaz"},
				new object[] {ER_STARTPARSE_NEEDS_SAXPARSER, "startParse i\u00e7in bo\u015f de\u011ferli olmayan SAXParser gerekiyor"},
				new object[] {ER_COULD_NOT_INIT_PARSER, "Ayr\u0131\u015ft\u0131r\u0131c\u0131 bununla kullan\u0131ma haz\u0131rlanamad\u0131"},
				new object[] {ER_EXCEPTION_CREATING_POOL, "Havuz i\u00e7in yeni \u00f6rnek yarat\u0131l\u0131rken kural d\u0131\u015f\u0131 durum"},
				new object[] {ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "Yol ge\u00e7ersiz ka\u00e7\u0131\u015f dizisi i\u00e7eriyor"},
				new object[] {ER_SCHEME_REQUIRED, "\u015eema gerekli!"},
				new object[] {ER_NO_SCHEME_IN_URI, "URI i\u00e7inde \u015fema bulunamad\u0131: {0}"},
				new object[] {ER_NO_SCHEME_INURI, "URI i\u00e7inde \u015fema bulunamad\u0131"},
				new object[] {ER_PATH_INVALID_CHAR, "Yol ge\u00e7ersiz karakter i\u00e7eriyor: {0}"},
				new object[] {ER_SCHEME_FROM_NULL_STRING, "Bo\u015f de\u011ferli dizgiden \u015fema tan\u0131mlanamaz"},
				new object[] {ER_SCHEME_NOT_CONFORMANT, "\u015eema uyumlu de\u011fil."},
				new object[] {ER_HOST_ADDRESS_NOT_WELLFORMED, "Anasistem do\u011fru bi\u00e7imli bir adres de\u011fil"},
				new object[] {ER_PORT_WHEN_HOST_NULL, "Anasistem bo\u015f de\u011ferliyken kap\u0131 tan\u0131mlanamaz"},
				new object[] {ER_INVALID_PORT, "Kap\u0131 numaras\u0131 ge\u00e7ersiz"},
				new object[] {ER_FRAG_FOR_GENERIC_URI, "Par\u00e7a yaln\u0131zca soysal URI i\u00e7in tan\u0131mlanabilir"},
				new object[] {ER_FRAG_WHEN_PATH_NULL, "Yol bo\u015f de\u011ferliyken par\u00e7a tan\u0131mlanamaz"},
				new object[] {ER_FRAG_INVALID_CHAR, "Par\u00e7a ge\u00e7ersiz karakter i\u00e7eriyor"},
				new object[] {ER_PARSER_IN_USE, "Ayr\u0131\u015ft\u0131r\u0131c\u0131 kullan\u0131mda"},
				new object[] {ER_CANNOT_CHANGE_WHILE_PARSING, "Ayr\u0131\u015ft\u0131rma s\u0131ras\u0131nda {0} {1} de\u011fi\u015ftirilemez"},
				new object[] {ER_SELF_CAUSATION_NOT_PERMITTED, "\u00d6znedenselli\u011fe izin verilmez"},
				new object[] {ER_NO_USERINFO_IF_NO_HOST, "Anasistem belirtilmediyse kullan\u0131c\u0131 bilgisi belirtilemez"},
				new object[] {ER_NO_PORT_IF_NO_HOST, "Anasistem belirtilmediyse kap\u0131 belirtilemez"},
				new object[] {ER_NO_QUERY_STRING_IN_PATH, "Yol ve sorgu dizgisinde sorgu dizgisi belirtilemez"},
				new object[] {ER_NO_FRAGMENT_STRING_IN_PATH, "Par\u00e7a hem yolda, hem de par\u00e7ada belirtilemez"},
				new object[] {ER_CANNOT_INIT_URI_EMPTY_PARMS, "Bo\u015f de\u011fi\u015ftirgelerle URI kullan\u0131ma haz\u0131rlanamaz"},
				new object[] {ER_METHOD_NOT_SUPPORTED, "Y\u00f6ntem hen\u00fcz desteklenmiyor "},
				new object[] {ER_INCRSAXSRCFILTER_NOT_RESTARTABLE, "IncrementalSAXSource_Filter \u015fu an yeniden ba\u015flat\u0131labilir durumda de\u011fil"},
				new object[] {ER_XMLRDR_NOT_BEFORE_STARTPARSE, "XMLReader, startParse iste\u011finden \u00f6nce olmaz"},
				new object[] {ER_AXIS_TRAVERSER_NOT_SUPPORTED, "Eksen dola\u015f\u0131c\u0131 desteklenmiyor: {0}"},
				new object[] {ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, "ListingErrorHandler bo\u015f de\u011ferli PrintWriter ile yarat\u0131ld\u0131!"},
				new object[] {ER_SYSTEMID_UNKNOWN, "SystemId bilinmiyor"},
				new object[] {ER_LOCATION_UNKNOWN, "Hata yeri bilinmiyor"},
				new object[] {ER_PREFIX_MUST_RESOLVE, "\u00d6nek bir ad alan\u0131na \u00e7\u00f6z\u00fclmelidir: {0}"},
				new object[] {ER_CREATEDOCUMENT_NOT_SUPPORTED, "XPathContext i\u00e7inde createDocument() desteklenmiyor!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT, "\u00d6zniteli\u011fin alt \u00f6\u011fesinin iye belgesi yok!"},
				new object[] {ER_CHILD_HAS_NO_OWNER_DOCUMENT_ELEMENT, "\u00d6zniteli\u011fin alt \u00f6\u011fesinin iye belge \u00f6\u011fesi yok!"},
				new object[] {ER_CANT_OUTPUT_TEXT_BEFORE_DOC, "Uyar\u0131: Belge \u00f6\u011fesinden \u00f6nce metin \u00e7\u0131k\u0131\u015f\u0131 olamaz!  Yoksay\u0131l\u0131yor..."},
				new object[] {ER_CANT_HAVE_MORE_THAN_ONE_ROOT, "DOM \u00fczerinde birden fazla k\u00f6k olamaz!"},
				new object[] {ER_ARG_LOCALNAME_NULL, "'localName' ba\u011f\u0131ms\u0131z de\u011fi\u015ftirgesi bo\u015f de\u011ferli"},
				new object[] {ER_ARG_LOCALNAME_INVALID, "QNAME i\u00e7indeki yerel ad (localname) ge\u00e7erli bir NCName olmal\u0131d\u0131r"},
				new object[] {ER_ARG_PREFIX_INVALID, "QNAME i\u00e7indeki \u00f6nek ge\u00e7erli bir NCName olmal\u0131d\u0131r"},
				new object[] {ER_NAME_CANT_START_WITH_COLON, "Ad iki nokta \u00fcst \u00fcste imiyle ba\u015flayamaz"},
				new object[] {"BAD_CODE", "createMessage i\u00e7in kullan\u0131lan de\u011fi\u015ftirge s\u0131n\u0131rlar\u0131n d\u0131\u015f\u0131nda"},
				new object[] {"FORMAT_FAILED", "messageFormat \u00e7a\u011fr\u0131s\u0131 s\u0131ras\u0131nda kural d\u0131\u015f\u0131 durum yay\u0131nland\u0131"},
				new object[] {"line", "Sat\u0131r #"},
				new object[] {"column", "Kolon #"}
			};
		  }
	  }

	  /// <summary>
	  ///   Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  ///   of ResourceBundle.getBundle().
	  /// </summary>
	  ///   <param name="className"> the name of the class that implements the resource bundle. </param>
	  ///   <returns> the ResourceBundle </returns>
	  ///   <exception cref="MissingResourceException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static final XMLErrorResources loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static XMLErrorResources loadResourceBundle(string className)
	  {

		Locale locale = Locale.getDefault();
		string suffix = getResourceSuffix(locale);

		try
		{

		  // first try with the given locale
		  return (XMLErrorResources) ResourceBundle.getBundle(className + suffix, locale);
		}
		catch (MissingResourceException)
		{
		  try // try to fall back to en_US if we can't load
		  {

			// Since we can't find the localized property file,
			// fall back to en_US.
			return (XMLErrorResources) ResourceBundle.getBundle(className, new Locale("tr", "TR"));
		  }
		  catch (MissingResourceException)
		  {

			// Now we are really in trouble.
			// very bad, definitely very bad...not going to get very far
			throw new MissingResourceException("Could not load any resource bundles.", className, "");
		  }
		}
	  }

	  /// <summary>
	  /// Return the resource file suffic for the indicated locale
	  /// For most locales, this will be based the language code.  However
	  /// for Chinese, we do distinguish between Taiwan and PRC
	  /// </summary>
	  /// <param name="locale"> the locale </param>
	  /// <returns> an String suffix which canbe appended to a resource name </returns>
	  private static string getResourceSuffix(Locale locale)
	  {

		string suffix = "_" + locale.getLanguage();
		string country = locale.getCountry();

		if (country.Equals("TW"))
		{
		  suffix += "_" + country;
		}

		return suffix;
	  }

	}

}