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
 * $Id: SerializerMessages_pl.java 471981 2006-11-07 04:28:00Z minchau $
 */

namespace org.apache.xml.serializer.utils
{


	/// <summary>
	/// An instance of this class is a ListResourceBundle that
	/// has the required getContents() method that returns
	/// an array of message-key/message associations.
	/// <para>
	/// The message keys are defined in <seealso cref="MsgKey"/>. The
	/// messages that those keys map to are defined here.
	/// </para>
	/// <para>
	/// The messages in the English version are intended to be
	/// translated.
	/// 
	/// This class is not a public API, it is only public because it is
	/// used in org.apache.xml.serializer.
	/// 
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	public class SerializerMessages_pl : ListResourceBundle
	{

		/*
		 * This file contains error and warning messages related to
		 * Serializer Error Handling.
		 *
		 *  General notes to translators:
	
		 *  1) A stylesheet is a description of how to transform an input XML document
		 *     into a resultant XML document (or HTML document or text).  The
		 *     stylesheet itself is described in the form of an XML document.
	
		 *
		 *  2) An element is a mark-up tag in an XML document; an attribute is a
		 *     modifier on the tag.  For example, in <elem attr='val' attr2='val2'>
		 *     "elem" is an element name, "attr" and "attr2" are attribute names with
		 *     the values "val" and "val2", respectively.
		 *
		 *  3) A namespace declaration is a special attribute that is used to associate
		 *     a prefix with a URI (the namespace).  The meanings of element names and
		 *     attribute names that use that prefix are defined with respect to that
		 *     namespace.
		 *
		 *
		 */

		/// <summary>
		/// The lookup table for error messages. </summary>
		public virtual object[][] Contents
		{
			get
			{
				object[][] contents = new object[][]
				{
					new object[] {MsgKey.BAD_MSGKEY, "Klucz komunikatu ''{0}'' nie znajduje si\u0119 w klasie komunikat\u00f3w ''{1}''"},
					new object[] {MsgKey.BAD_MSGFORMAT, "Nie powiod\u0142o si\u0119 sformatowanie komunikatu ''{0}'' w klasie komunikat\u00f3w ''{1}''."},
					new object[] {MsgKey.ER_SERIALIZER_NOT_CONTENTHANDLER, "Klasa szereguj\u0105ca ''{0}'' nie implementuje org.xml.sax.ContentHandler."},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_FIND, "Nie mo\u017cna znale\u017a\u0107 zasobu [ {0} ].\n {1}"},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_LOAD, "Zas\u00f3b [ {0} ] nie m\u00f3g\u0142 za\u0142adowa\u0107: {1} \n {2} \t {3}"},
					new object[] {MsgKey.ER_BUFFER_SIZE_LESSTHAN_ZERO, "Wielko\u015b\u0107 buforu <=0"},
					new object[] {MsgKey.ER_INVALID_UTF16_SURROGATE, "Wykryto niepoprawny odpowiednik UTF-16: {0} ?"},
					new object[] {MsgKey.ER_OIERROR, "B\u0142\u0105d we/wy"},
					new object[] {MsgKey.ER_ILLEGAL_ATTRIBUTE_POSITION, "Nie mo\u017cna doda\u0107 atrybutu {0} po bezpo\u015brednich w\u0119z\u0142ach potomnych ani przed wyprodukowaniem elementu.  Atrybut zostanie zignorowany."},
					new object[] {MsgKey.ER_NAMESPACE_PREFIX, "Nie zadeklarowano przestrzeni nazw dla przedrostka ''{0}''."},
					new object[] {MsgKey.ER_STRAY_ATTRIBUTE, "Atrybut ''{0}'' znajduje si\u0119 poza elementem."},
					new object[] {MsgKey.ER_STRAY_NAMESPACE, "Deklaracja przestrzeni nazw ''{0}''=''{1}'' znajduje si\u0119 poza elementem."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_RESOURCE, "Nie mo\u017cna za\u0142adowa\u0107 ''{0}'' (sprawd\u017a CLASSPATH) - u\u017cywane s\u0105 teraz warto\u015bci domy\u015blne"},
					new object[] {MsgKey.ER_ILLEGAL_CHARACTER, "Pr\u00f3ba wyprowadzenia znaku warto\u015bci ca\u0142kowitej {0}, kt\u00f3ry nie jest reprezentowany w podanym kodowaniu wyj\u015bciowym {1}."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_METHOD_PROPERTY, "Nie mo\u017cna za\u0142adowa\u0107 pliku w\u0142a\u015bciwo\u015bci ''{0}'' dla metody wyj\u015bciowej ''{1}'' (sprawd\u017a CLASSPATH)"},
					new object[] {MsgKey.ER_INVALID_PORT, "Niepoprawny numer portu"},
					new object[] {MsgKey.ER_PORT_WHEN_HOST_NULL, "Nie mo\u017cna ustawi\u0107 portu, kiedy host jest pusty"},
					new object[] {MsgKey.ER_HOST_ADDRESS_NOT_WELLFORMED, "Host nie jest poprawnie skonstruowanym adresem"},
					new object[] {MsgKey.ER_SCHEME_NOT_CONFORMANT, "Schemat nie jest zgodny."},
					new object[] {MsgKey.ER_SCHEME_FROM_NULL_STRING, "Nie mo\u017cna ustawi\u0107 schematu z pustego ci\u0105gu znak\u00f3w"},
					new object[] {MsgKey.ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "\u015acie\u017cka zawiera nieznan\u0105 sekwencj\u0119 o zmienionym znaczeniu"},
					new object[] {MsgKey.ER_PATH_INVALID_CHAR, "\u015acie\u017cka zawiera niepoprawny znak {0}"},
					new object[] {MsgKey.ER_FRAG_INVALID_CHAR, "Fragment zawiera niepoprawny znak"},
					new object[] {MsgKey.ER_FRAG_WHEN_PATH_NULL, "Nie mo\u017cna ustawi\u0107 fragmentu, kiedy \u015bcie\u017cka jest pusta"},
					new object[] {MsgKey.ER_FRAG_FOR_GENERIC_URI, "Fragment mo\u017cna ustawi\u0107 tylko dla og\u00f3lnego URI"},
					new object[] {MsgKey.ER_NO_SCHEME_IN_URI, "Nie znaleziono schematu w URI"},
					new object[] {MsgKey.ER_CANNOT_INIT_URI_EMPTY_PARMS, "Nie mo\u017cna zainicjowa\u0107 URI z pustymi parametrami"},
					new object[] {MsgKey.ER_NO_FRAGMENT_STRING_IN_PATH, "Nie mo\u017cna poda\u0107 fragmentu jednocze\u015bnie w \u015bcie\u017cce i fragmencie"},
					new object[] {MsgKey.ER_NO_QUERY_STRING_IN_PATH, "Tekstu zapytania nie mo\u017cna poda\u0107 w tek\u015bcie \u015bcie\u017cki i zapytania"},
					new object[] {MsgKey.ER_NO_PORT_IF_NO_HOST, "Nie mo\u017cna poda\u0107 portu, je\u015bli nie podano hosta"},
					new object[] {MsgKey.ER_NO_USERINFO_IF_NO_HOST, "Nie mo\u017cna poda\u0107 informacji o u\u017cytkowniku, je\u015bli nie podano hosta"},
					new object[] {MsgKey.ER_XML_VERSION_NOT_SUPPORTED, "Ostrze\u017cenie:  Wymagan\u0105 wersj\u0105 dokumentu wyj\u015bciowego jest ''{0}''.  Ta wersja XML nie jest obs\u0142ugiwana.  Wersj\u0105 dokumentu wyj\u015bciowego b\u0119dzie ''1.0''."},
					new object[] {MsgKey.ER_SCHEME_REQUIRED, "Schemat jest wymagany!"},
					new object[] {MsgKey.ER_FACTORY_PROPERTY_MISSING, "Obiekt klasy Properties przekazany do klasy SerializerFactory nie ma w\u0142a\u015bciwo\u015bci ''{0}''."},
					new object[] {MsgKey.ER_ENCODING_NOT_SUPPORTED, "Ostrze\u017cenie:  dekodowany ''{0}'' nie jest obs\u0142ugiwany przez \u015brodowisko wykonawcze Java."},
					new object[] {MsgKey.ER_FEATURE_NOT_FOUND, "Parametr ''{0}'' nie zosta\u0142 rozpoznany."},
					new object[] {MsgKey.ER_FEATURE_NOT_SUPPORTED, "Parametr ''{0}'' zosta\u0142 rozpoznany, ale nie mo\u017cna ustawi\u0107 \u017c\u0105danej warto\u015bci."},
					new object[] {MsgKey.ER_STRING_TOO_LONG, "Wynikowy \u0142a\u0144cuch jest zbyt d\u0142ugi, aby si\u0119 zmie\u015bci\u0107 w obiekcie DOMString: ''{0}''."},
					new object[] {MsgKey.ER_TYPE_MISMATCH_ERR, "Typ warto\u015bci parametru o tej nazwie jest niezgodny z oczekiwanym typem warto\u015bci. "},
					new object[] {MsgKey.ER_NO_OUTPUT_SPECIFIED, "Docelowe miejsce zapisu danych wyj\u015bciowych by\u0142o puste (null)."},
					new object[] {MsgKey.ER_UNSUPPORTED_ENCODING, "Napotkano nieobs\u0142ugiwane kodowanie."},
					new object[] {MsgKey.ER_UNABLE_TO_SERIALIZE_NODE, "Nie mo\u017cna przekszta\u0142ci\u0107 w\u0119z\u0142a do postaci szeregowej."},
					new object[] {MsgKey.ER_CDATA_SECTIONS_SPLIT, "Sekcja CDATA zawiera jeden lub kilka znacznik\u00f3w zako\u0144czenia ']]>'."},
					new object[] {MsgKey.ER_WARNING_WF_NOT_CHECKED, "Nie mo\u017cna utworzy\u0107 instancji obiektu sprawdzaj\u0105cego Well-Formedness.  Parametr well-formed ustawiono na warto\u015b\u0107 true, ale nie mo\u017cna by\u0142o dokona\u0107 sprawdzenia poprawno\u015bci konstrukcji."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER, "W\u0119ze\u0142 ''{0}'' zawiera niepoprawne znaki XML."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_COMMENT, "W komentarzu znaleziono niepoprawny znak XML (Unicode: 0x{0})."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_PI, "W danych instrukcji przetwarzania znaleziono niepoprawny znak XML (Unicode: 0x{0})."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_CDATA, "W sekcji CDATA znaleziono niepoprawny znak XML (Unicode: 0x{0})."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_TEXT, "W tre\u015bci danych znakowych w\u0119z\u0142a znaleziono niepoprawny znak XML (Unicode: 0x{0})."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, "W {0} o nazwie ''{1}'' znaleziono niepoprawne znaki XML."},
					new object[] {MsgKey.ER_WF_DASH_IN_COMMENT, "Ci\u0105g znak\u00f3w \"--\" jest niedozwolony w komentarzu."},
					new object[] {MsgKey.ER_WF_LT_IN_ATTVAL, "Warto\u015b\u0107 atrybutu \"{1}\" zwi\u0105zanego z typem elementu \"{0}\" nie mo\u017ce zawiera\u0107 znaku ''<''."},
					new object[] {MsgKey.ER_WF_REF_TO_UNPARSED_ENT, "Odwo\u0142anie do encji nieprzetwarzanej \"&{0};\" jest niedozwolone."},
					new object[] {MsgKey.ER_WF_REF_TO_EXTERNAL_ENT, "Odwo\u0142anie do zewn\u0119trznej encji \"&{0};\" jest niedozwolone w warto\u015bci atrybutu."},
					new object[] {MsgKey.ER_NS_PREFIX_CANNOT_BE_BOUND, "Nie mo\u017cna zwi\u0105za\u0107 przedrostka \"{0}\" z przestrzeni\u0105 nazw \"{1}\"."},
					new object[] {MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, "Nazwa lokalna elementu \"{0}\" jest pusta (null)."},
					new object[] {MsgKey.ER_NULL_LOCAL_ATTR_NAME, "Nazwa lokalna atrybutu \"{0}\" jest pusta (null)."},
					new object[] {MsgKey.ER_ELEM_UNBOUND_PREFIX_IN_ENTREF, "Tekst zast\u0119puj\u0105cy w\u0119z\u0142a encji \"{0}\" zawiera w\u0119ze\u0142 elementu \"{1}\" o niezwi\u0105zanym przedrostku \"{2}\"."},
					new object[] {MsgKey.ER_ATTR_UNBOUND_PREFIX_IN_ENTREF, "Tekst zast\u0119puj\u0105cy w\u0119z\u0142a encji \"{0}\" zawiera w\u0119ze\u0142 atrybutu \"{1}\" o niezwi\u0105zanym przedrostku \"{2}\"."}
				};
    
				return contents;
			}
		}
	}

}