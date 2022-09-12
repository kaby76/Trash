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
 * $Id: ErrorMessages_tr.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_tr : ListResourceBundle
	{

	/*
	 * XSLTC compile-time error messages.
	 *
	 * General notes to translators and definitions:
	 *
	 *   1) XSLTC is the name of the product.  It is an acronym for "XSLT Compiler".
	 *      XSLT is an acronym for "XML Stylesheet Language: Transformations".
	 *
	 *   2) A stylesheet is a description of how to transform an input XML document
	 *      into a resultant XML document (or HTML document or text).  The
	 *      stylesheet itself is described in the form of an XML document.
	 *
	 *   3) A template is a component of a stylesheet that is used to match a
	 *      particular portion of an input document and specifies the form of the
	 *      corresponding portion of the output document.
	 *
	 *   4) An axis is a particular "dimension" in a tree representation of an XML
	 *      document; the nodes in the tree are divided along different axes.
	 *      Traversing the "child" axis, for instance, means that the program
	 *      would visit each child of a particular node; traversing the "descendant"
	 *      axis means that the program would visit the child nodes of a particular
	 *      node, their children, and so on until the leaf nodes of the tree are
	 *      reached.
	 *
	 *   5) An iterator is an object that traverses nodes in a tree along a
	 *      particular axis, one at a time.
	 *
	 *   6) An element is a mark-up tag in an XML document; an attribute is a
	 *      modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *      "elem" is an element name, "attr" and "attr2" are attribute names with
	 *      the values "val" and "val2", respectively.
	 *
	 *   7) A namespace declaration is a special attribute that is used to associate
	 *      a prefix with a URI (the namespace).  The meanings of element names and
	 *      attribute names that use that prefix are defined with respect to that
	 *      namespace.
	 *
	 *   8) DOM is an acronym for Document Object Model.  It is a tree
	 *      representation of an XML document.
	 *
	 *      SAX is an acronym for the Simple API for XML processing.  It is an API
	 *      used inform an XML processor (in this case XSLTC) of the structure and
	 *      content of an XML document.
	 *
	 *      Input to the stylesheet processor can come from an XML parser in the
	 *      form of a DOM tree or through the SAX API.
	 *
	 *   9) DTD is a document type declaration.  It is a way of specifying the
	 *      grammar for an XML file, the names and types of elements, attributes,
	 *      etc.
	 *
	 *  10) XPath is a specification that describes a notation for identifying
	 *      nodes in a tree-structured representation of an XML document.  An
	 *      instance of that notation is referred to as an XPath expression.
	 *
	 *  11) Translet is an invented term that refers to the class file that contains
	 *      the compiled form of a stylesheet.
	 */

		// These message should be read from a locale-specific resource bundle
		/// <summary>
		/// Get the lookup table for error messages.
		/// </summary>
		/// <returns> The message lookup table. </returns>
		public virtual object[][] Contents
		{
			get
			{
			  return new object[][]
			  {
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "Ayn\u0131 dosyada birden \u00e7ok bi\u00e7em yapra\u011f\u0131 tan\u0131mland\u0131."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "Bi\u00e7em yapra\u011f\u0131nda ''{0}'' \u015fablonu zaten tan\u0131ml\u0131."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "Bu bi\u00e7em yapra\u011f\u0131nda ''{0}'' \u015fablonu tan\u0131ml\u0131 de\u011fil."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "''{0}'' de\u011fi\u015fkeni ayn\u0131 kapsamda bir kereden \u00e7ok tan\u0131mland\u0131."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "''{0}'' de\u011fi\u015fkeni ya da de\u011fi\u015ftirgesi tan\u0131ml\u0131 de\u011fil."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "''{0}'' s\u0131n\u0131f\u0131 bulunam\u0131yor."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "''{0}'' d\u0131\u015f y\u00f6ntemi bulunam\u0131yor (public olmal\u0131)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "''{0}'' y\u00f6ntemi \u00e7a\u011fr\u0131s\u0131nda ba\u011f\u0131ms\u0131z de\u011fi\u015fken/d\u00f6n\u00fc\u015f tipi d\u00f6n\u00fc\u015ft\u00fcr\u00fclemiyor."},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Dosya ya da URI ''{0}'' bulunamad\u0131."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "Ge\u00e7ersiz URI ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "Dosya ya da URI ''{0}'' a\u00e7\u0131lam\u0131yor."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "<xsl:stylesheet> ya da <xsl:transform> \u00f6\u011fesi bekleniyor."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "Ad alan\u0131 \u00f6neki ''{0}'' bildirilmemi\u015f."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "''{0}'' i\u015flevi \u00e7a\u011fr\u0131s\u0131 \u00e7\u00f6z\u00fclemiyor."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "''{0}'' i\u015flevine ili\u015fkin ba\u011f\u0131ms\u0131z de\u011fi\u015fken bir haz\u0131r bilgi dizgisi olmal\u0131d\u0131r."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "XPath ifadesi ''{0}'' ayr\u0131\u015ft\u0131r\u0131l\u0131rken hata olu\u015ftu."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Gerekli ''{0}'' \u00f6zniteli\u011fi eksik."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "XPath ifadesinde ge\u00e7ersiz ''{0}'' karakteri var."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "\u0130\u015fleme y\u00f6nergesi i\u00e7in ''{0}'' ad\u0131 ge\u00e7ersiz."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "''{0}'' \u00f6zniteli\u011fi \u00f6\u011fenin d\u0131\u015f\u0131nda."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "''{0}'' \u00f6zniteli\u011fi ge\u00e7ersiz."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "\u00c7evrimsel import/include. ''{0}'' bi\u00e7em yapra\u011f\u0131 zaten y\u00fcklendi."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Sonu\u00e7 a\u011fac\u0131 par\u00e7alar\u0131 s\u0131ralanam\u0131yor (<xsl:sort> \u00f6\u011feleri yok say\u0131ld\u0131). D\u00fc\u011f\u00fcmleri sonu\u00e7 a\u011fac\u0131n\u0131 yarat\u0131rken s\u0131ralamal\u0131s\u0131n\u0131z."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "Onlu bi\u00e7imleme bi\u00e7emi ''{0}'' zaten tan\u0131ml\u0131."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "XSL s\u00fcr\u00fcm\u00fc ''{0}'' XSLTC taraf\u0131ndan desteklenmiyor."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "''{0}'' i\u00e7inde \u00e7evrimsel de\u011fi\u015fken/de\u011fi\u015ftirge ba\u015fvurusu."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "\u0130kili ifadede bilinmeyen i\u015fle\u00e7."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "\u0130\u015flev \u00e7a\u011fr\u0131s\u0131 i\u00e7in ge\u00e7ersiz say\u0131da ba\u011f\u0131ms\u0131z de\u011fi\u015fken."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "document() i\u015flevinin ikinci ba\u011f\u0131ms\u0131z de\u011fi\u015fkeni d\u00fc\u011f\u00fcm k\u00fcmesi olmal\u0131d\u0131r."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "<xsl:choose> i\u00e7inde en az bir <xsl:when> \u00f6\u011fesi gereklidir."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "<xsl:choose> i\u00e7inde tek bir <xsl:otherwise> \u00f6\u011fesine izin verilir."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> yaln\u0131zca <xsl:choose> i\u00e7inde kullan\u0131labilir."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> yaln\u0131zca <xsl:choose> i\u00e7inde kullan\u0131labilir."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "<xsl:choose> i\u00e7inde yaln\u0131zca <xsl:when> ve <xsl:otherwise> \u00f6\u011feleri kullan\u0131labilir."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> \u00f6\u011fesinde 'name' \u00f6zniteli\u011fi eksik."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Ge\u00e7ersiz alt \u00f6\u011fe."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Bir \u00f6\u011feye ''{0}'' ad\u0131 verilemez."},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Bir \u00f6zniteli\u011fe ''{0}'' ad\u0131 verilemez."},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "\u00dcst d\u00fczey <xsl:stylesheet> \u00f6\u011fesi d\u0131\u015f\u0131nda metin verisi."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "JAXP ayr\u0131\u015ft\u0131r\u0131c\u0131s\u0131 do\u011fru yap\u0131land\u0131r\u0131lmam\u0131\u015f"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Kurtar\u0131lamaz XSLTC i\u00e7 hatas\u0131: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "XSL \u00f6\u011fesi ''{0}'' desteklenmiyor."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "XSLTC eklentisi ''{0}'' tan\u0131nm\u0131yor."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "Giri\u015f belgesi bir bi\u00e7em yapra\u011f\u0131 de\u011fil (XSL ad alan\u0131 k\u00f6k \u00f6\u011fede bildirilmedi)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "Bi\u00e7em yapra\u011f\u0131 hedefi ''{0}'' bulunamad\u0131."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "Ger\u00e7ekle\u015ftirilmedi: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "Giri\u015f belgesi bir XSL bi\u00e7em yapra\u011f\u0131 i\u00e7ermiyor."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "''{0}'' \u00f6\u011fesi ayr\u0131\u015ft\u0131r\u0131lamad\u0131."},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "<key> ile ilgili use \u00f6zniteli\u011fi node, node-set, string ya da number olmal\u0131d\u0131r."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "\u00c7\u0131k\u0131\u015f XML belgesi s\u00fcr\u00fcm\u00fc 1.0 olmal\u0131d\u0131r."},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "\u0130li\u015fkisel ifade i\u00e7in bilinmeyen i\u015fle\u00e7"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Varolmayan ''{0}'' \u00f6znitelik k\u00fcmesini kullanma giri\u015fimi."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "\u00d6znitelik de\u011feri \u015fablonu ''{0}'' ayr\u0131\u015ft\u0131r\u0131lam\u0131yor."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "''{0}'' s\u0131n\u0131f\u0131na ili\u015fkin imzada bilinmeyen veri tipi."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "''{0}'' veri tipi ''{1}'' tipine d\u00f6n\u00fc\u015ft\u00fcr\u00fclemez."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Bu Templates ge\u00e7erli bir derleme sonucu s\u0131n\u0131f tan\u0131m\u0131 i\u00e7ermiyor."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Bu Templates ''{0}'' ad\u0131nda bir s\u0131n\u0131f i\u00e7ermiyor."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "Derleme sonucu s\u0131n\u0131f\u0131 ''{0}'' y\u00fcklenemedi."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Derleme sonucu s\u0131n\u0131f\u0131 y\u00fcklendi, ancak derleme sonucu s\u0131n\u0131f\u0131n\u0131n somut kopyas\u0131 yarat\u0131lam\u0131yor."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "''{0}'' ile ilgili ErrorListener nesnesini bo\u015f de\u011fer (null) olarak ayarlama giri\u015fimi."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "XSLTC yaln\u0131zca StreamSource, SAXSource ve DOMSource arabirimlerini destekler."},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "''{0}'' y\u00f6ntemine aktar\u0131lan Source nesnesinin i\u00e7eri\u011fi yok."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "Bi\u00e7em yapra\u011f\u0131 derlenemedi."},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory ''{0}'' \u00f6zniteli\u011fini tan\u0131m\u0131yor."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "startDocument() y\u00f6nteminden \u00f6nce setResult() \u00e7a\u011fr\u0131lmal\u0131d\u0131r."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer, derleme sonucu s\u0131n\u0131f dosyas\u0131 nesnesine ba\u015fvuru i\u00e7ermiyor."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "D\u00f6n\u00fc\u015ft\u00fcrme sonucu i\u00e7in tan\u0131ml\u0131 \u00e7\u0131k\u0131\u015f i\u015fleyicisi yok."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "''{0}'' y\u00f6ntemine aktar\u0131lan Result nesnesi ge\u00e7ersiz."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Ge\u00e7ersiz ''{0}'' Transformer \u00f6zelli\u011fine (property) eri\u015fme giri\u015fimi."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "SAX2DOM ba\u011fda\u015ft\u0131r\u0131c\u0131s\u0131 yarat\u0131lamad\u0131: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() y\u00f6ntemi systemId tan\u0131mlanmadan \u00e7a\u011fr\u0131ld\u0131."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "Sonu\u00e7 bo\u015f de\u011ferli olmamal\u0131"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "{0} de\u011fi\u015ftirgesinin de\u011feri ge\u00e7erli bir Java nesnesi olmal\u0131d\u0131r"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "-i se\u00e7ene\u011fi -o se\u00e7ene\u011fiyle birlikte kullan\u0131lmal\u0131d\u0131r."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "\u00d6ZET\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <\u00e7\u0131k\u0131\u015f>]\n      [-d <dizin>] [-j <jardosyas\u0131>] [-p <paket>]\n      [-n] [-x] [-u] [-v] [-h] { <bi\u00e7emyapra\u011f\u0131> | -i }\n\nSE\u00c7ENEKLER\n   -o <\u00e7\u0131k\u0131\u015f>    derleme sonucu s\u0131n\u0131f dosyas\u0131na <\u00e7\u0131k\u0131\u015f>\n                  ad\u0131n\u0131 atar. Varsay\u0131lan olarak, derleme sonucu s\u0131n\u0131f dosyas\u0131\n                  ad\u0131 <bi\u00e7emyapra\u011f\u0131> ad\u0131ndan al\u0131n\u0131r.  Birden \u00e7ok bi\u00e7em yapra\u011f\u0131 derleniyorsa\n                  bu se\u00e7enek dikkate al\u0131nmaz.\n   -d <dizin> derleme sonucu s\u0131n\u0131f dosyas\u0131 i\u00e7in hedef dizini belirtir.\n   -j <jardosyas\u0131>   derleme sonucu s\u0131n\u0131f dosyalar\u0131n\u0131\n                  <jardosyas\u0131> dosyas\u0131nda paketler.\n   -p <paket>   derleme sonucu \u00fcretilen t\u00fcm s\u0131n\u0131f dosyalar\u0131 i\u00e7in\n                  bir paket ad\u0131 \u00f6neki belirtir.\n   -n             \u015fablona do\u011frudan yerle\u015ftirmeyi etkinle\u015ftirir (ortalama olarak\n                  daha y\u00fcksek ba\u015far\u0131m sa\u011flar).\n   -x             ek hata ay\u0131klama iletisi \u00e7\u0131k\u0131\u015f\u0131n\u0131 etkinle\u015ftirir\n   -u             <bi\u00e7emyapra\u011f\u0131> ba\u011f\u0131ms\u0131z de\u011fi\u015fkenlerini URL olarak yorumlars\n   -i             derleyiciyi stdin'den bi\u00e7em yapra\u011f\u0131n\u0131 okumaya zorlar\n   -v             derleyici s\u00fcr\u00fcm\u00fcn\u00fc yazd\u0131r\u0131r.\n   -h             bu kullan\u0131m bilgilerini yazd\u0131r\u0131r\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "\u00d6ZET \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <jardosyas\u0131>]\n      [-x] [-n <yinelemesay\u0131s\u0131>] {-u <belge_url> | <belge>}\n      <s\u0131n\u0131f> [<de\u011fi\u015ftirge1>=<de\u011fer1> ...]\n\n   <belge> ile belirtilen XML belgesini d\u00f6n\u00fc\u015ft\u00fcrmek i\u00e7in <s\u0131n\u0131f>\n   s\u0131n\u0131f dosyas\u0131n\u0131 kullan\u0131r. <s\u0131n\u0131f> s\u0131n\u0131f dosyas\u0131\n   kullan\u0131c\u0131n\u0131n CLASSPATH de\u011fi\u015fkeninde ya da iste\u011fe ba\u011fl\u0131 olarak belirtilen <jardosyas\u0131> dosyas\u0131ndad\u0131r.\nSE\u00c7ENEKLER\n   -j <jardosyas\u0131>    derleme sonucu s\u0131n\u0131f dosyas\u0131n\u0131n hangi jar dosyas\u0131ndan y\u00fcklenece\u011fini belirtir\n   -x              ek hata ay\u0131klama iletisi \u00e7\u0131k\u0131\u015f\u0131n\u0131 etkinle\u015ftirir\n   -n <yinelemesay\u0131s\u0131> d\u00f6n\u00fc\u015ft\u00fcrmeyi <yineleme say\u0131s\u0131> ile belirtilen say\u0131 kadar \u00e7al\u0131\u015ft\u0131r\u0131r\n                   ve yakalama bilgilerini g\u00f6r\u00fcnt\u00fcler\n   -u <belge_url> XML giri\u015f belgesini URL olarak belirtir\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> yaln\u0131zca <xsl:for-each> ya da <xsl:apply-templates> i\u00e7inde kullan\u0131labilir."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "''{0}'' \u00e7\u0131k\u0131\u015f kodlamas\u0131 bu JVM \u00fczerinde desteklenmiyor."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "''{0}'' ifadesinde s\u00f6zdizimi hatas\u0131."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "D\u0131\u015f olu\u015fturucu ''{0}'' bulunam\u0131yor."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "Dura\u011fan (static) olmayan ''{0}'' Java i\u015flevine ili\u015fkin ilk ba\u011f\u0131ms\u0131z de\u011fi\u015fken ge\u00e7erli bir nesne ba\u015fvurusu de\u011fil."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "''{0}'' ifadesinin tipi denetlenirken hata saptand\u0131."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Bilinmeyen bir yerdeki bir ifadenin tipi denetlenirken hata saptand\u0131."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "Komut sat\u0131r\u0131 se\u00e7ene\u011fi ''{0}'' ge\u00e7erli de\u011fil."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "''{0}'' komut sat\u0131r\u0131 se\u00e7ene\u011finde gerekli bir ba\u011f\u0131ms\u0131z de\u011fi\u015fken eksik."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "UYARI:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "UYARI:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "ONULMAZ HATA:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "ONULMAZ HATA:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "HATA:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "HATA:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "''{0}'' s\u0131n\u0131f\u0131n\u0131 kullanarak d\u00f6n\u00fc\u015ft\u00fcr "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "''{1}'' jar dosyas\u0131ndan ''{0}'' derleme sonucu s\u0131n\u0131f dosyas\u0131n\u0131 kullanarak d\u00f6n\u00fc\u015ft\u00fcr"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "''{0}'' TransformerFactory s\u0131n\u0131f\u0131n\u0131n somut \u00f6rne\u011fi yarat\u0131lamad\u0131."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "''{0}'' ad\u0131, derleme sonucu s\u0131n\u0131f dosyas\u0131 ad\u0131 olarak kullan\u0131lamad\u0131; bir Java s\u0131n\u0131f\u0131nda kullan\u0131lmas\u0131na izin verilmeyen karakterler i\u00e7eriyor.  Onun yerine ''{1}'' ad\u0131 kullan\u0131ld\u0131."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Derleyici hatalar\u0131:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Derleyici uyar\u0131lar\u0131:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Derleme sonusu s\u0131n\u0131f dosyas\u0131 hatalar\u0131:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "De\u011ferinin bir QName ya da beyaz alanla ayr\u0131lm\u0131\u015f QName listesi olmas\u0131 gereken bir \u00f6zniteli\u011fin de\u011feri ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "De\u011ferinin bir NCName olmas\u0131 gereken \u00f6zniteli\u011fin de\u011feri ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "Bir <xsl:output> \u00f6\u011fesinin y\u00f6ntem \u00f6zniteli\u011finin de\u011feri ''{0}''.  De\u011fer ''xml'', ''html'', ''text'' ya da ncname olmayan bir qname olmal\u0131d\u0131r"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "TransformerFactory.getFeature(dizgi ad\u0131) i\u00e7inde \u00f6zellik (feature) ad\u0131 bo\u015f de\u011ferli olamaz."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "TransformerFactory.setFeature(dizgi ad\u0131, boole de\u011fer) i\u00e7inde \u00f6zellik (feature) ad\u0131 bo\u015f de\u011ferli olamaz."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "Bu TransformerFactory \u00fczerinde ''{0}'' \u00f6zelli\u011fi tan\u0131mlanamaz."}
			  };
			}
		}

	}

}