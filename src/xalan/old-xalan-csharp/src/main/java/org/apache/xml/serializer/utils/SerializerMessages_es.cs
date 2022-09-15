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
 * $Id: SerializerMessages_es.java 471981 2006-11-07 04:28:00Z minchau $
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
	public class SerializerMessages_es : ListResourceBundle
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
					new object[] {MsgKey.BAD_MSGKEY, "La clave de mensaje ''{0}'' no est\u00e1 en la clase de mensaje ''{1}''"},
					new object[] {MsgKey.BAD_MSGFORMAT, "Se ha producido un error en el formato de mensaje ''{0}'' de la clase de mensaje ''{1}''."},
					new object[] {MsgKey.ER_SERIALIZER_NOT_CONTENTHANDLER, "La clase serializer ''{0}'' no implementa org.xml.sax.ContentHandler."},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_FIND, "No se ha podido encontrar el recurso [ {0} ].\n {1}"},
					new object[] {MsgKey.ER_RESOURCE_COULD_NOT_LOAD, "No se ha podido cargar el recurso [ {0} ]: {1} \n {2} \t {3}"},
					new object[] {MsgKey.ER_BUFFER_SIZE_LESSTHAN_ZERO, "Tama\u00f1o de almacenamiento intermedio <=0"},
					new object[] {MsgKey.ER_INVALID_UTF16_SURROGATE, "\u00bfSe ha detectado un sustituto UTF-16 no v\u00e1lido: {0}?"},
					new object[] {MsgKey.ER_OIERROR, "Error de ES"},
					new object[] {MsgKey.ER_ILLEGAL_ATTRIBUTE_POSITION, "No se puede a\u00f1adir el atributo {0} despu\u00e9s de nodos hijo o antes de que se produzca un elemento.  Se ignorar\u00e1 el atributo."},
					new object[] {MsgKey.ER_NAMESPACE_PREFIX, "No se ha declarado el espacio de nombres para el prefijo ''{0}''."},
					new object[] {MsgKey.ER_STRAY_ATTRIBUTE, "Atributo ''{0}'' fuera del elemento."},
					new object[] {MsgKey.ER_STRAY_NAMESPACE, "Declaraci\u00f3n del espacio de nombres ''{0}''=''{1}'' fuera del elemento."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_RESOURCE, "No se ha podido cargar ''{0}'' (compruebe la CLASSPATH), ahora s\u00f3lo se est\u00e1n utilizando los valores predeterminados"},
					new object[] {MsgKey.ER_ILLEGAL_CHARACTER, "Se ha intentado dar salida a un car\u00e1cter del valor integral {0} que no est\u00e1 representado en la codificaci\u00f3n de salida especificada de {1}."},
					new object[] {MsgKey.ER_COULD_NOT_LOAD_METHOD_PROPERTY, "No se ha podido cargar el archivo de propiedades ''{0}'' para el m\u00e9todo de salida ''{1}'' (compruebe la CLASSPATH)"},
					new object[] {MsgKey.ER_INVALID_PORT, "N\u00famero de puerto no v\u00e1lido"},
					new object[] {MsgKey.ER_PORT_WHEN_HOST_NULL, "No se puede establecer el puerto si el sistema principal es nulo"},
					new object[] {MsgKey.ER_HOST_ADDRESS_NOT_WELLFORMED, "El sistema principal no es una direcci\u00f3n bien formada"},
					new object[] {MsgKey.ER_SCHEME_NOT_CONFORMANT, "El esquema no es compatible."},
					new object[] {MsgKey.ER_SCHEME_FROM_NULL_STRING, "No se puede establecer un esquema de una serie nula"},
					new object[] {MsgKey.ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "La v\u00eda de acceso contiene una secuencia de escape no v\u00e1lida"},
					new object[] {MsgKey.ER_PATH_INVALID_CHAR, "La v\u00eda de acceso contiene un car\u00e1cter no v\u00e1lido: {0}"},
					new object[] {MsgKey.ER_FRAG_INVALID_CHAR, "El fragmento contiene un car\u00e1cter no v\u00e1lido"},
					new object[] {MsgKey.ER_FRAG_WHEN_PATH_NULL, "No se puede establecer el fragmento si la v\u00eda de acceso es nula"},
					new object[] {MsgKey.ER_FRAG_FOR_GENERIC_URI, "S\u00f3lo se puede establecer el fragmento para un URI gen\u00e9rico"},
					new object[] {MsgKey.ER_NO_SCHEME_IN_URI, "No se ha encontrado un esquema en el URI"},
					new object[] {MsgKey.ER_CANNOT_INIT_URI_EMPTY_PARMS, "No se puede inicializar el URI con par\u00e1metros vac\u00edos"},
					new object[] {MsgKey.ER_NO_FRAGMENT_STRING_IN_PATH, "No se puede especificar el fragmento en la v\u00eda de acceso y en el fragmento"},
					new object[] {MsgKey.ER_NO_QUERY_STRING_IN_PATH, "No se puede especificar la serie de consulta en la v\u00eda de acceso y en la serie de consulta"},
					new object[] {MsgKey.ER_NO_PORT_IF_NO_HOST, "No se puede especificar el puerto si no se ha especificado el sistema principal"},
					new object[] {MsgKey.ER_NO_USERINFO_IF_NO_HOST, "No se puede especificar la informaci\u00f3n de usuario si no se ha especificado el sistema principal"},
					new object[] {MsgKey.ER_XML_VERSION_NOT_SUPPORTED, "Aviso: la versi\u00f3n del documento de salida tiene que ser ''{0}''.  No se admite esta versi\u00f3n de XML.  La versi\u00f3n del documento de salida ser\u00e1 ''1.0''."},
					new object[] {MsgKey.ER_SCHEME_REQUIRED, "\u00a1Se necesita un esquema!"},
					new object[] {MsgKey.ER_FACTORY_PROPERTY_MISSING, "El objeto Properties pasado a SerializerFactory no tiene una propiedad ''{0}''."},
					new object[] {MsgKey.ER_ENCODING_NOT_SUPPORTED, "Aviso: La codificaci\u00f3n ''{0}'' no est\u00e1 soportada por Java Runtime."},
					new object[] {MsgKey.ER_FEATURE_NOT_FOUND, "El par\u00e1metro ''{0}'' no se reconoce."},
					new object[] {MsgKey.ER_FEATURE_NOT_SUPPORTED, "Se reconoce el par\u00e1metro ''{0}'' pero no puede establecerse el valor solicitado."},
					new object[] {MsgKey.ER_STRING_TOO_LONG, "La serie producida es demasiado larga para ajustarse a DOMString: ''{0}''."},
					new object[] {MsgKey.ER_TYPE_MISMATCH_ERR, "El tipo de valor para este nombre de par\u00e1metro es incompatible con el tipo de valor esperado."},
					new object[] {MsgKey.ER_NO_OUTPUT_SPECIFIED, "El destino de salida de escritura de los datos es nulo."},
					new object[] {MsgKey.ER_UNSUPPORTED_ENCODING, "Se ha encontrado una codificaci\u00f3n no soportada."},
					new object[] {MsgKey.ER_UNABLE_TO_SERIALIZE_NODE, "No se ha podido serializar el nodo."},
					new object[] {MsgKey.ER_CDATA_SECTIONS_SPLIT, "La secci\u00f3n CDATA contiene uno o m\u00e1s marcadores ']]>' de terminaci\u00f3n."},
					new object[] {MsgKey.ER_WARNING_WF_NOT_CHECKED, "No se ha podido crear una instancia del comprobador de gram\u00e1tica correcta.  El par\u00e1metro well-formed se ha establecido en true pero no se puede realizar la comprobaci\u00f3n de gram\u00e1tica correcta."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER, "El nodo ''{0}'' contiene caracteres XML no v\u00e1lidos."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_COMMENT, "Se ha encontrado un car\u00e1cter XML no v\u00e1lido (Unicode: 0x{0}) en el comentario."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_PI, "Se ha encontrado un car\u00e1cter XML no v\u00e1lido (Unicode: 0x{0}) en los datos de la instrucci\u00f3n de proceso."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_CDATA, "Se ha encontrado un car\u00e1cter XML no v\u00e1lido (Unicode: 0x{0}) en el contenido de CDATASection."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_TEXT, "Se ha encontrado un car\u00e1cter XML no v\u00e1lido (Unicode: 0x{0}) en el contenido de datos de caracteres del nodo."},
					new object[] {MsgKey.ER_WF_INVALID_CHARACTER_IN_NODE_NAME, "Se ha encontrado un car\u00e1cter o caracteres XML no v\u00e1lidos en el nodo {0} denominado ''{1}''."},
					new object[] {MsgKey.ER_WF_DASH_IN_COMMENT, "No se permite la serie \"--\" dentro de los comentarios."},
					new object[] {MsgKey.ER_WF_LT_IN_ATTVAL, "El valor del atributo \"{1}\" asociado a un tipo de elemento \"{0}\" no debe contener el car\u00e1cter ''''<''''."},
					new object[] {MsgKey.ER_WF_REF_TO_UNPARSED_ENT, "No se permite la referencia de entidad no analizada \"&{0};\"."},
					new object[] {MsgKey.ER_WF_REF_TO_EXTERNAL_ENT, "La referencia de entidad externa \"&{0};\" no est\u00e1 permitida en un valor de atributo."},
					new object[] {MsgKey.ER_NS_PREFIX_CANNOT_BE_BOUND, "No se puede encontrar el prefijo \"{0}\" en el espacio de nombres \"{1}\"."},
					new object[] {MsgKey.ER_NULL_LOCAL_ELEMENT_NAME, "El nombre local del elemento \"{0}\" es null."},
					new object[] {MsgKey.ER_NULL_LOCAL_ATTR_NAME, "El nombre local del atributo \"{0}\" es null."},
					new object[] {MsgKey.ER_ELEM_UNBOUND_PREFIX_IN_ENTREF, "El texto de sustituci\u00f3n del nodo de entidad \"{0}\" contiene un nodo de elemento \"{1}\" con un prefijo no enlazado \"{2}\"."},
					new object[] {MsgKey.ER_ATTR_UNBOUND_PREFIX_IN_ENTREF, "El texto de sustituci\u00f3n del nodo de entidad \"{0}\" contiene un nodo de atributo \"{1}\" con un prefijo no enlazado \"{2}\"."}
				};
    
				return contents;
			}
		}
	}

}