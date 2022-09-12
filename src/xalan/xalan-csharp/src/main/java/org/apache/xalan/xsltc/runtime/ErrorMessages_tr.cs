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
 * $Id: ErrorMessages_tr.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_tr : ListResourceBundle
	{

	/*
	 * XSLTC run-time error messages.
	 *
	 * General notes to translators and definitions:
	 *
	 *   1) XSLTC is the name of the product.  It is an acronym for XML Stylesheet:
	 *      Transformations Compiler
	 *
	 *   2) A stylesheet is a description of how to transform an input XML document
	 *      into a resultant output XML document (or HTML document or text)
	 *
	 *   3) An axis is a particular "dimension" in a tree representation of an XML
	 *      document; the nodes in the tree are divided along different axes.
	 *      Traversing the "child" axis, for instance, means that the program
	 *      would visit each child of a particular node; traversing the "descendant"
	 *      axis means that the program would visit the child nodes of a particular
	 *      node, their children, and so on until the leaf nodes of the tree are
	 *      reached.
	 *
	 *   4) An iterator is an object that traverses nodes in a tree along a
	 *      particular axis, one at a time.
	 *
	 *   5) An element is a mark-up tag in an XML document; an attribute is a
	 *      modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
	 *      "elem" is an element name, "attr" and "attr2" are attribute names with
	 *      the values "val" and "val2", respectively.
	 *
	 *   6) A namespace declaration is a special attribute that is used to associate
	 *      a prefix with a URI (the namespace).  The meanings of element names and
	 *      attribute names that use that prefix are defined with respect to that
	 *      namespace.
	 *
	 *   7) DOM is an acronym for Document Object Model.  It is a tree
	 *      representation of an XML document.
	 *
	 *      SAX is an acronym for the Simple API for XML processing.  It is an API
	 *      used inform an XML processor (in this case XSLTC) of the structure and
	 *      content of an XML document.
	 *
	 *      Input to the stylesheet processor can come from an XML parser in the
	 *      form of a DOM tree or through the SAX API.
	 *
	 *   8) DTD is a document type declaration.  It is a way of specifying the
	 *      grammar for an XML file, the names and types of elements, attributes,
	 *      etc.
	 *
	 *   9) Translet is an invented term that refers to the class file that contains
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
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "''{0}'' i\u00e7inde y\u00fcr\u00fctme zaman\u0131 i\u00e7 hatas\u0131"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "<xsl:copy> y\u00fcr\u00fct\u00fcl\u00fcrken y\u00fcr\u00fctme zaman\u0131 hatas\u0131."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "''{0}'' tipinden ''{1}'' tipine d\u00f6n\u00fc\u015ft\u00fcrme ge\u00e7ersiz."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "''{0}'' d\u0131\u015f i\u015flevi XSLTC taraf\u0131ndan desteklenmiyor."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "E\u015fitlik ifadesinde bilinmeyen ba\u011f\u0131ms\u0131z de\u011fi\u015fken tipi."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "''{1}'' i\u015flevi \u00e7a\u011fr\u0131s\u0131nda ba\u011f\u0131ms\u0131z de\u011fi\u015fken tipi ''{0}'' ge\u00e7ersiz."},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "''{0}'' say\u0131s\u0131n\u0131 ''{1}'' \u00f6r\u00fcnt\u00fcs\u00fcn\u00fc kullanarak bi\u00e7imleme giri\u015fimi."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "''{0}'' yineleyicisinin e\u015fkopyas\u0131 yarat\u0131lam\u0131yor."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "''{0}'' ekseni i\u00e7in yineleyici desteklenmiyor."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Tip atanm\u0131\u015f ''{0}'' ekseni i\u00e7in yineleyici desteklenmiyor."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "''{0}'' \u00f6zniteli\u011fi \u00f6\u011fenin d\u0131\u015f\u0131nda."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "''{0}''=''{1}'' ad alan\u0131 bildirimi \u00f6\u011fenin d\u0131\u015f\u0131nda."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "''{0}'' \u00f6nekine ili\u015fkin ad alan\u0131 bildirilmedi."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "DOMAdapter, yanl\u0131\u015f tipte kaynak DOM kullan\u0131larak yarat\u0131ld\u0131."},
				  new object[] {BasisLibrary.PARSER_DTD_SUPPORT_ERR, "Kulland\u0131\u011f\u0131n\u0131z SAX ayr\u0131\u015ft\u0131r\u0131c\u0131s\u0131 DTD bildirim olaylar\u0131n\u0131 i\u015flemiyor."},
				  new object[] {BasisLibrary.NAMESPACES_SUPPORT_ERR, "Kulland\u0131\u011f\u0131n\u0131z SAX ayr\u0131\u015ft\u0131r\u0131c\u0131s\u0131n\u0131n XML ad alanlar\u0131 deste\u011fi yok."},
				  new object[] {BasisLibrary.CANT_RESOLVE_RELATIVE_URI_ERR, "''{0}'' URI ba\u015fvurusu \u00e7\u00f6z\u00fclemedi."},
				  new object[] {BasisLibrary.UNSUPPORTED_XSL_ERR, "XSL \u00f6\u011fesi ''{0}'' desteklenmiyor"},
				  new object[] {BasisLibrary.UNSUPPORTED_EXT_ERR, "XSLTC uzant\u0131s\u0131 ''{0}'' tan\u0131nm\u0131yor"},
				  new object[] {BasisLibrary.UNKNOWN_TRANSLET_VERSION_ERR, "Belirtilen derleme sonucu s\u0131n\u0131f dosyas\u0131 ''{0}'', kullan\u0131lmakta olan XSLTC s\u00fcr\u00fcm\u00fcnden daha yeni bir XSLTC s\u00fcr\u00fcm\u00fcyle yarat\u0131lm\u0131\u015f.  Bi\u00e7em yapra\u011f\u0131n\u0131 yeniden derlemeli ya da bu derleme sonucu s\u0131n\u0131f dosyas\u0131n\u0131 \u00e7al\u0131\u015ft\u0131rmak i\u00e7in daha yeni bir XSLTC s\u00fcr\u00fcm\u00fcn\u00fc kullanmal\u0131s\u0131n\u0131z."},
				  new object[] {BasisLibrary.INVALID_QNAME_ERR, "De\u011ferinin bir QName olmas\u0131 gereken \u00f6zniteli\u011fin de\u011feri ''{0}''"},
				  new object[] {BasisLibrary.INVALID_NCNAME_ERR, "De\u011ferinin bir NCName olmas\u0131 gereken \u00f6zniteli\u011fin de\u011feri ''{0}''"},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_FUNCTION_ERR, "G\u00fcvenli i\u015fleme \u00f6zelli\u011fi true de\u011ferine ayarland\u0131\u011f\u0131nda ''{0}'' eklenti i\u015flevinin kullan\u0131lmas\u0131na izin verilmez."},
				  new object[] {BasisLibrary.UNALLOWED_EXTENSION_ELEMENT_ERR, "G\u00fcvenli i\u015fleme \u00f6zelli\u011fi true de\u011ferine ayarland\u0131\u011f\u0131nda ''{0}'' eklenti \u00f6\u011fesinin kullan\u0131lmas\u0131na izin verilmez."}
			  };
			}
		}

	}

}