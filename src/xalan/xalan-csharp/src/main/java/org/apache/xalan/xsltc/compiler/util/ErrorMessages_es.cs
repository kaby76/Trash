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
 * $Id: ErrorMessages_es.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_es : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "Hay m\u00e1s de una hoja de estilos definida en el mismo archivo."},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "La plantilla ''{0}'' ya est\u00e1 definida en esta hoja de estilos."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "La plantilla ''{0}'' no est\u00e1 definida en esta hoja de estilos."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "La variable ''{0}'' se ha definido varias veces en el mismo \u00e1mbito."},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "La variable o el par\u00e1metro ''{0}'' no est\u00e1n definidos."},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "No se puede encontrar la clase ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "No se puede encontrar el m\u00e9todo externo ''{0}'' (debe ser p\u00fablico)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "No se puede convertir el argumento/tipo de devoluci\u00f3n en la llamada al m\u00e9todo ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Archivo o URI ''{0}'' no encontrado."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI ''{0}'' no v\u00e1lido."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "No se puede abrir el archivo o URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Se esperaba el elemento <xsl:stylesheet> o <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "El prefijo ''{0}'' del espacio de nombres no est\u00e1 declarado."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "No se puede resolver la llamada a la funci\u00f3n ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "El argumento para ''{0}'' debe ser una serie literal."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Error al analizar la expresi\u00f3n ''{0}'' de XPath."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "Falta el atributo necesario ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Car\u00e1cter ''{0}'' no permitido en expresi\u00f3n de XPath."},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Nombre ''{0}'' no permitido para la instrucci\u00f3n de proceso."},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Atributo ''{0}'' fuera del elemento."},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Atributo ''{0}'' no permitido."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "import/include circular. Hoja de estilos ''{0}'' ya cargada."},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Los fragmentos del \u00e1rbol de resultados no se pueden ordenar (elementos <xsl:sort> ignorados). Debe ordenar los nodos al crear el \u00e1rbol de resultados."},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "El formato decimal ''{0}'' ya est\u00e1 definido."},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "La versi\u00f3n de XSL ''{0}'' no est\u00e1 soportada por XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Referencia de variable/par\u00e1metro circular en ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Operador desconocido para expresi\u00f3n binaria."},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Argumento(s) no permitido(s) para llamada a funci\u00f3n."},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "El segundo argumento de la funci\u00f3n document() debe ser un conjunto de nodos."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Se necesita al menos un elemento <xsl:when> en <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "S\u00f3lo se permite un elemento <xsl:otherwise> en <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> s\u00f3lo puede utilizarse dentro de <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> s\u00f3lo puede utilizarse dentro de <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "S\u00f3lo est\u00e1n permitidos los elementos <xsl:when> y <xsl:otherwise> en <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "Falta el atributo 'name' en <xsl:attribute-set>."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Elemento hijo no permitido."},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "No puede llamar a un elemento ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "No puede llamar a un atributo ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Datos de texto fuera del elemento <xsl:stylesheet> de nivel superior."},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Analizador JAXP no configurado correctamente"},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Error interno de XSLTC irrecuperable: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Elemento XSL ''{0}'' no soportado."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Extensi\u00f3n XSLTC ''{0}'' no reconocida."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "El documento de entrada no es una hoja de estilos (el espacio de nombres XSL no est\u00e1 declarado en el elemento ra\u00edz)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "No se ha podido encontrar el destino de la hoja de estilos ''{0}''."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "No implementado: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "El documento de entrada no contiene una hoja de estilos XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "No se ha podido analizar el elemento ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "El atributo use de <key> debe ser node, node-set, string o number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "La versi\u00f3n del documento XML de salida deber\u00eda ser 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Operador desconocido para expresi\u00f3n relacional"},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Intento de utilizar un conjunto de atributos no existente ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "No se puede analizar la plantilla de valor del atributo ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Tipo de datos desconocido en la firma de la clase ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "No se puede convertir el tipo de datos ''{0}'' a ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Templates no contiene una definici\u00f3n de clase translet v\u00e1lida."},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Templates no contiene una clase con el nombre ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "No se ha podido cargar la clase translet ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Clase translet cargada, pero no es posible crear una instancia translet."},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Intento de establecer ErrorListener para ''{0}'' en null"},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "S\u00f3lo StreamSource, SAXSource y DOMSource est\u00e1n soportadas por XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "El objeto Source pasado a ''{0}'' no tiene contenido."},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "No se ha podido compilar la hoja de estilos"},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory no reconoce el atributo ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() debe llamarse antes de startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer no tiene un objeto translet encapsulado."},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "No se ha definido un manejador de salida para el resultado de la transformaci\u00f3n."},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "El objeto Result pasado a ''{0}'' no es v\u00e1lido."},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Intento de acceder a una propiedad de Transformer ''{0}'' no v\u00e1lida."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "No se ha podido crear adaptador SAX2DOMr: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() llamado sin establecer systemId."},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "El resultado no deber\u00eda ser nulo"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "El valor del par\u00e1metro {0} debe ser un objeto Java v\u00e1lido"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "La opci\u00f3n -i debe utilizarse con la opci\u00f3n -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SINOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <salida>]\n      [-d <directory>] [-j <archivojar>] [-p <paquete>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\nOPCIONES\n   -o <salida>    asigna el nombre <salida> al translet\n                  generado.  De forma predeterminada, el nombre del translet\n                  se deriva del nombre de la <hojaestilos>. Esta opci\u00f3n\n                  se ignora si se compilan varias hojas de estilos.\n   -d <directorio> especificar un directorio de destino para el translet\n   -j <archivojar>   empaqueta las clases translet en el archivo jar del\n                  nombre especificado por <archivojar>\n   -p <paquete>   especifica un prefijo de nombre de paquete para todas las\n                  clases translet generadas.\n   -n             habilita la inclusi\u00f3n en l\u00ednea de plantillas (comportamiento predeterminado\n                  mejor seg\u00fan promedio).\n   -x             activa la salida de mensajes de depuraci\u00f3n adicionales\n   -u             interpreta los argumentos <stylesheet>  como URL\n   -i             fuerza que el compilador lea la hoja de estilos de stdin\n   -v             imprime la versi\u00f3n del compilador\n   -h             imprime esta sentencia de uso\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SINOPSIS \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <archivojar>]\n      [-x] [-n <iteraciones>] {-u <url_documento | <documento>}\n      <clase> [<param1>=<valor1> ...]\n\n   utiliza la <clase> translet para transformar  un documento XML \n   especificado como <documento>. La <clase> translet est\u00e1 en\n   la CLASSPATH del usuario o en el <archivojar> especificado opcionalmente.\nOPCIONES\n   -j <archivojar>    especifica un archivo jar desde el que se va a cargar el translet\n   -x           activa la salida de mensajes de depuraci\u00f3n adicionales\n   -n <iteraciones> ejecuta la transformaci\u00f3n <iteraciones> veces y\n       muestra informaci\u00f3n de perfiles\n   -u <url_documento> especifica el documento de entrada XML como un URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> s\u00f3lo puede utilizarse dentro de <xsl:for-each> o <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "La codificaci\u00f3n de salida ''{0}'' no est\u00e1 soportada en esta JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Error de sintaxis en ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "No se puede encontrar el constructor externo ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "El primer argumento de la funci\u00f3n Java no est\u00e1tica ''{0}'' no es una referencia de objeto v\u00e1lida."},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Error al comprobar el tipo de la expresi\u00f3n ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Error al comprobar el tipo de una expresi\u00f3n en una ubicaci\u00f3n desconocida."},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "La opci\u00f3n ''{0}'' de la l\u00ednea de mandatos no es v\u00e1lida."},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "Falta un argumento necesario en la opci\u00f3n ''{0}'' de la l\u00ednea de mandatos."},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "AVISO:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "AVISO:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "ERROR MUY GRAVE:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "ERROR MUY GRAVE:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "ERROR:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "ERROR:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Transformaci\u00f3n con translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Transformaci\u00f3n con translet ''{0}'' del archivo jar ''{1}''"},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "No se ha podido crear una instancia de la clase TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "No se ha podido utilizar el nombre ''{0}'' como nombre de la clase translet porque contiene caracteres que no est\u00e1n permitidos en los nombres de clases Java. Se utiliza el nombre ''{1}'' en su lugar."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Errores del compilador:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Avisos del compilador:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Errores de translet:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Un atributo cuyo valor debe ser un QName o una lista de QNames separados por espacios tiene el valor ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Un atributo cuyo valor debe ser un NCName tiene el valor ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "El atributo method de un elemento <xsl:output> tiene el valor ''{0}''. El valor debe ser ''xml'', ''html'', ''text'' o qname-but-not-ncname"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "El nombre de caracter\u00edstica no puede ser null en TransformerFactory.getFeature(nombre de tipo String)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "El nombre de caracter\u00edstica no puede ser null en TransformerFactory.setFeature(nombre de tipo String, valor booleano)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "No se puede establecer la caracter\u00edstica ''{0}'' en esta TransformerFactory."}
			  };
			}
		}

	}

}