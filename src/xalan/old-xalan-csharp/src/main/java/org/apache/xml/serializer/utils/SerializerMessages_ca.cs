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
 * $Id: SerializerMessages_ca.java 471981 2006-11-07 04:28:00Z minchau $
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
	public class SerializerMessages_ca : ListResourceBundle
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
					new object[] {MsgKey.BAD_MSGKEY, "La clau del missatge ''{0}'' no est\u00e0 a la classe del missatge ''{1}''"},
					new object[] {MsgKey.BAD_MSGFORMAT, "El format del missatge ''{0}'' a la classe del missatge ''{1}'' ha fallat."},
					new object[] {MsgKey.ER_SERIALIZER_NOT_CONTENTHANDLER, "La classe de serialitzador ''{0}'' no implementa org.xml.sax.ContentHandler."},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_FIND, "No s''ha trobat el recurs [ {0} ].\n {1}"},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_LOAD, "No s''ha pogut carregar el recurs [ {0} ]: {1} \n {2} \t {3}"},
					new object[] {MsgKey.ER_BUFFER_SIZE_LESSTHAN_ZERO, "Grand\u00e0ria del buffer <=0"},
					new object[] {MsgKey.ER_INVALID_UTF16_SURROGATE, "S''ha detectat un suplent UTF-16 no v\u00e0lid: {0} ?"},
					new object[] {MsgKey.ER_OIERROR, "Error d'E/S"},
					new object[] {MsgKey.ER_ILLEGAL_ATTRIBUTE_POSITION, "No es pot afegir l''atribut {0} despr\u00e9s dels nodes subordinats o abans que es produeixi un element. Es passar\u00e0 per alt l''atribut."},
					new object[] {MsgKey.ER_NAMESPACE_PREFIX, "No s''ha declarat l''espai de noms pel prefix ''{0}''."},
					new object[] {MsgKey.ER_STRAY_ATTRIBUTE, "L''atribut ''{0}'' es troba fora de l''element."},
					new object[] {MsgKey.ER_STRAY_NAMESPACE, "La declaraci\u00f3 de l''espai de noms ''{0}''=''{1}'' es troba fora de l''element."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_RESOURCE, "No s''ha pogut carregar ''{0}'' (comproveu CLASSPATH), ara s''est\u00e0 fent servir els valors per defecte."},
					new object[] {MsgKey.ER_ILLEGAL_CHARACTER, "S''ha intentat un car\u00e0cter de sortida del valor integral {0} que no est\u00e0 representat a una codificaci\u00f3 de sortida especificada de {1}."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_METHOD_PROPERTY, "No s''ha pogut carregar el fitxer de propietats ''{0}'' del m\u00e8tode de sortida ''{1}'' (comproveu CLASSPATH)"},
					new object[] {MsgKey.ER_INVALID_PORT, "N\u00famero de port no v\u00e0lid"},
					new object[] {MsgKey.ER_PORT_WHEN_HOST_NULL, "El port no es pot establir quan el sistema principal \u00e9s nul"},
					new object[] {MsgKey.ER_HOST_ADDRESS_NOT_WELLFORMED, "El format de l'adre\u00e7a del sistema principal no \u00e9s el correcte"},
					new object[] {MsgKey.ER_SCHEME_NOT_CONFORMANT, "L'esquema no t\u00e9 conformitat."},
					new object[] {MsgKey.ER_SCHEME_FROM_NULL_STRING, "No es pot establir un esquema des d'una cadena nul\u00b7la"},
					new object[] {MsgKey.ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "La via d'acc\u00e9s cont\u00e9 una seq\u00fc\u00e8ncia d'escapament no v\u00e0lida"},
					new object[] {MsgKey.ER_PATH_INVALID_CHAR, "La via d''acc\u00e9s cont\u00e9 un car\u00e0cter no v\u00e0lid {0}"},
					new object[] {MsgKey.ER_FRAG_INVALID_CHAR, "El fragment cont\u00e9 un car\u00e0cter no v\u00e0lid"},
					new object[] {MsgKey.ER_FRAG_WHEN_PATH_NULL, "El fragment no es pot establir si la via d'acc\u00e9s \u00e9s nul\u00b7la"},
					new object[] {MsgKey.ER_FRAG_FOR_GENERIC_URI, "El fragment nom\u00e9s es pot establir per a un URI gen\u00e8ric"},
					new object[] {MsgKey.ER_NO_SCHEME_IN_URI, "No s'ha trobat cap esquema a l'URI"},
					new object[] {MsgKey.ER_CANNOT_INIT_URI_EMPTY_PARMS, "No es pot inicialitzar l'URI amb par\u00e0metres buits"},
					new object[] {MsgKey.ER_NO_FRAGMENT_STRING_IN_PATH, "No es pot especificar un fragment tant en la via d'acc\u00e9s com en el fragment"},
					new object[] {MsgKey.ER_NO_QUERY_STRING_IN_PATH, "No es pot especificar una cadena de consulta en la via d'acc\u00e9s i la cadena de consulta"},
					new object[] {MsgKey.ER_NO_PORT_IF_NO_HOST, "No es pot especificar el port si no s'especifica el sistema principal"},
					new object[] {MsgKey.ER_NO_USERINFO_IF_NO_HOST, "No es pot especificar informaci\u00f3 de l'usuari si no s'especifica el sistema principal"},
					new object[] {MsgKey.ER_XML_VERSION_NOT_SUPPORTED, "Av\u00eds: la versi\u00f3 del document de sortida s''ha sol\u00b7licitat que sigui ''{0}''. Aquesta versi\u00f3 de XML no est\u00e0 suportada. La versi\u00f3 del document de sortida ser\u00e0 ''1.0''."},
					new object[] {MsgKey.ER_SCHEME_REQUIRED, "Es necessita l'esquema"},
					new object[] {MsgKey.ER_FACTORY_PROPERTY_MISSING, "L''objecte de propietats passat a SerializerFactory no t\u00e9 cap propietat ''{0}''."},
					new object[] {MsgKey.ER_ENCODING_NOT_SUPPORTED, "Av\u00eds: el temps d''execuci\u00f3 de Java no d\u00f3na suport a la codificaci\u00f3 ''{0}''."},
					new object[] {MsgKey.ER_FEATURE_NOT_FOUND, "El par\u00e0metre ''{0}'' no es reconeix."},
					new object[] {MsgKey.ER_FEATURE_NOT_SUPPORTED, "El par\u00e0metre ''{0}'' es reconeix per\u00f2 el valor sol\u00b7licitat no es pot establir."},
					new object[] {MsgKey.ER_STRING_TOO_LONG, "La cadena resultant \u00e9s massa llarga per cabre en una DOMString: ''{0}''."},
					new object[] {MsgKey.ER_TYPE_MISMATCH_ERR, "El tipus de valor per a aquest nom de par\u00e0metre \u00e9s incompatible amb el tipus de valor esperat."},
					new object[] {MsgKey.ER_NO_OUTPUT_SPECIFIED, "La destinaci\u00f3 de sortida per a les dades que s'ha d'escriure era nul\u00b7la."},
					new object[] {MsgKey.ER_UNSUPPORTED_ENCODING, "S'ha trobat una codificaci\u00f3 no suportada."},
					new object[] {MsgKey.ER_UNABLE_TO_SERIALIZE_NODE, "El node no s'ha pogut serialitzat."},
					new object[] {MsgKey.ER_CDATA_SECTIONS_SPLIT, "La secci\u00f3 CDATA cont\u00e9 un o m\u00e9s marcadors d'acabament ']]>'."},
					new object[] {MsgKey.ER_WARNING_WF_NOT_CHECKED, "No s'ha pogut crear cap inst\u00e0ncia per comprovar si t\u00e9 un format correcte o no. El par\u00e0metre del tipus ben format es va establir en cert, per\u00f2 la comprovaci\u00f3 de format no s'ha pogut realitzar."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER, "El node ''{0}'' cont\u00e9 car\u00e0cters XML no v\u00e0lids."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_COMMENT, "S''ha trobat un car\u00e0cter XML no v\u00e0lid (Unicode: 0x{0}) en el comentari."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_PI, "S''ha trobat un car\u00e0cter XML no v\u00e0lid (Unicode: 0x{0}) en les dades d''instrucci\u00f3 de proc\u00e9s."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_CDATA, "S''ha trobat un car\u00e0cter XML no v\u00e0lid (Unicode: 0x''{0})'' en els continguts de la CDATASection."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_TEXT, "S''ha trobat un car\u00e0cter XML no v\u00e0lid (Unicode: 0x''{0})'' en el contingut de dades de car\u00e0cter del node."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, "S''han trobat car\u00e0cters XML no v\u00e0lids al node {0} anomenat ''{1}''."},
					new object[] {MsgKey.ER_WF_DASH_IN_COMMENT, "La cadena \"--\" no est\u00e0 permesa dins dels comentaris."},
					new object[] {MsgKey.ER_WF_LT_IN_ATTVAL, "El valor d''atribut \"{1}\" associat amb un tipus d''element \"{0}\" no pot contenir el car\u00e0cter ''<''."},
					new object[] {MsgKey.ER_WF_REF_TO_UNPARSED_ENT, "La refer\u00e8ncia de l''entitat no analitzada \"&{0};\" no est\u00e0 permesa."},
					new object[] {MsgKey.ER_WF_REF_TO_EXTERNAL_ENT, "La refer\u00e8ncia externa de l''entitat \"&{0};\" no est\u00e0 permesa en un valor d''atribut."},
					new object[] {MsgKey.ER_NS_PREFIX_CANNOT_BE_BOUND, "El prefix \"{0}\" no es pot vincular a l''espai de noms \"{1}\"."},
					new object[] {MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, "El nom local de l''element \"{0}\" \u00e9s nul."},
					new object[] {MsgKey.ER_NULL_LOCAL_ATTR_NAME, "El nom local d''atr \"{0}\" \u00e9s nul."},
					new object[] {MsgKey.ER_ELEM_UNBOUND_PREFIX_IN_ENTREF, "El text de recanvi del node de l''entitat \"{0}\" cont\u00e9 un node d''element \"{1}\" amb un prefix de no enlla\u00e7at \"{2}\"."},
					new object[] {MsgKey.ER_ATTR_UNBOUND_PREFIX_IN_ENTREF, "El text de recanvi del node de l''entitat \"{0}\" cont\u00e9 un node d''atribut \"{1}\" amb un prefix de no enlla\u00e7at \"{2}\"."}
				};
    
				return contents;
			}
		}
	}

}