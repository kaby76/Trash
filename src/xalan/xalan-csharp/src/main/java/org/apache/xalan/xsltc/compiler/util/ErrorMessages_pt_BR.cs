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
 * $Id: ErrorMessages_pt_BR.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_pt_BR : ListResourceBundle
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
				  new object[] {ErrorMsg.MULTIPLE_STYLESHEET_ERR, "Mais de uma p\u00e1gina de estilo definida no mesmo arquivo. "},
				  new object[] {ErrorMsg.TEMPLATE_REDEF_ERR, "O template ''{0}'' j\u00e1 est\u00e1 definido nesta folha de estilo."},
				  new object[] {ErrorMsg.TEMPLATE_UNDEF_ERR, "O template ''{0}'' n\u00e3o est\u00e1 definido nesta folha de estilo."},
				  new object[] {ErrorMsg.VARIABLE_REDEF_ERR, "A vari\u00e1vel ''{0}'' tem sua multiplica\u00e7\u00e3o definida no mesmo escopo. "},
				  new object[] {ErrorMsg.VARIABLE_UNDEF_ERR, "A vari\u00e1vel ou o par\u00e2metro ''{0}'' n\u00e3o est\u00e1 definido. "},
				  new object[] {ErrorMsg.CLASS_NOT_FOUND_ERR, "N\u00e3o \u00e9 poss\u00edvel localizar a classe ''{0}''."},
				  new object[] {ErrorMsg.METHOD_NOT_FOUND_ERR, "N\u00e3o \u00e9 poss\u00edvel localizar o m\u00e9todo externo ''{0}'' (deve ser p\u00fablico)."},
				  new object[] {ErrorMsg.ARGUMENT_CONVERSION_ERR, "N\u00e3o \u00e9 poss\u00edvel converter o tipo de argumento/retorno na chamada para o m\u00e9todo ''{0}''"},
				  new object[] {ErrorMsg.FILE_NOT_FOUND_ERR, "Arquivo ou URI ''{0}'' n\u00e3o localizado."},
				  new object[] {ErrorMsg.INVALID_URI_ERR, "URI inv\u00e1lido ''{0}''."},
				  new object[] {ErrorMsg.FILE_ACCESS_ERR, "N\u00e3o \u00e9 poss\u00edvel abrir o arquivo ou URI ''{0}''."},
				  new object[] {ErrorMsg.MISSING_ROOT_ERR, "Esperado elemento <xsl:stylesheet> ou <xsl:transform>."},
				  new object[] {ErrorMsg.NAMESPACE_UNDEF_ERR, "O prefixo de espa\u00e7o de nomes ''{0}'' n\u00e3o foi declarado."},
				  new object[] {ErrorMsg.FUNCTION_RESOLVE_ERR, "Imposs\u00edvel resolver a chamada para a fun\u00e7\u00e3o ''{0}''."},
				  new object[] {ErrorMsg.NEED_LITERAL_ERR, "O argumento para ''{0}'' deve ser uma cadeia literal."},
				  new object[] {ErrorMsg.XPATH_PARSER_ERR, "Erro ao analisar a express\u00e3o XPath ''{0}''."},
				  new object[] {ErrorMsg.REQUIRED_ATTR_ERR, "O atributo requerido ''{0}'' est\u00e1 ausente. "},
				  new object[] {ErrorMsg.ILLEGAL_CHAR_ERR, "Caractere inv\u00e1lido ''{0}'' na express\u00e3o XPath. "},
				  new object[] {ErrorMsg.ILLEGAL_PI_ERR, "Nome inv\u00e1lido ''{0}'' para instru\u00e7\u00e3o de processamento. "},
				  new object[] {ErrorMsg.STRAY_ATTRIBUTE_ERR, "Atributo ''{0}'' fora do elemento. "},
				  new object[] {ErrorMsg.ILLEGAL_ATTRIBUTE_ERR, "Atributo inv\u00e1lido ''{0}''."},
				  new object[] {ErrorMsg.CIRCULAR_INCLUDE_ERR, "Import/include circular. A folha de estilo ''{0}'' j\u00e1 foi carregada. "},
				  new object[] {ErrorMsg.RESULT_TREE_SORT_ERR, "Os fragmentos da \u00e1rvore de resultados n\u00e3o podem ser ordenados (os elementos <xsl:sort> ser\u00e3o ignorados). Voc\u00ea deve ordenar os n\u00f3s quando criar a \u00e1rvore de resultados. "},
				  new object[] {ErrorMsg.SYMBOLS_REDEF_ERR, "A formata\u00e7\u00e3o decimal ''{0}'' j\u00e1 est\u00e1 definida. "},
				  new object[] {ErrorMsg.XSL_VERSION_ERR, "A vers\u00e3o de XSL ''{0}'' n\u00e3o \u00e9 suportada por XSLTC."},
				  new object[] {ErrorMsg.CIRCULAR_VARIABLE_ERR, "Refer\u00eancia \u00e0 vari\u00e1vel/par\u00e2metro circular em ''{0}''."},
				  new object[] {ErrorMsg.ILLEGAL_BINARY_OP_ERR, "Operador desconhecido para express\u00e3o bin\u00e1ria. "},
				  new object[] {ErrorMsg.ILLEGAL_ARG_ERR, "Argumentos inv\u00e1lidos para chamada de fun\u00e7\u00e3o. "},
				  new object[] {ErrorMsg.DOCUMENT_ARG_ERR, "O segundo argumento para a fun\u00e7\u00e3o document() deve ser um node-set."},
				  new object[] {ErrorMsg.MISSING_WHEN_ERR, "Pelo menos um elemento <xsl:when> \u00e9 requerido em <xsl:choose>."},
				  new object[] {ErrorMsg.MULTIPLE_OTHERWISE_ERR, "Apenas um elemento <xsl:otherwise> \u00e9 permitido em <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_OTHERWISE_ERR, "<xsl:otherwise> somente pode ser utilizado em <xsl:choose>."},
				  new object[] {ErrorMsg.STRAY_WHEN_ERR, "<xsl:when> somente pode ser utilizado em <xsl:choose>."},
				  new object[] {ErrorMsg.WHEN_ELEMENT_ERR, "Apenas os elementos <xsl:when> e <xsl:otherwise> s\u00e3o permitidos em <xsl:choose>."},
				  new object[] {ErrorMsg.UNNAMED_ATTRIBSET_ERR, "<xsl:attribute-set> n\u00e3o possui o atributo 'name'."},
				  new object[] {ErrorMsg.ILLEGAL_CHILD_ERR, "Elemento filho inv\u00e1lido. "},
				  new object[] {ErrorMsg.ILLEGAL_ELEM_NAME_ERR, "Voc\u00ea n\u00e3o pode chamar um elemento ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_ATTR_NAME_ERR, "Voc\u00ea n\u00e3o pode chamar um atributo ''{0}''"},
				  new object[] {ErrorMsg.ILLEGAL_TEXT_NODE_ERR, "Dados de texto fora do elemento <xsl:stylesheet> de n\u00edvel superior. "},
				  new object[] {ErrorMsg.SAX_PARSER_CONFIG_ERR, "Analisador JAXP n\u00e3o configurado corretamente "},
				  new object[] {ErrorMsg.INTERNAL_ERR, "Erro interno de XSLTC irrecuper\u00e1vel: ''{0}''"},
				  new object[] {ErrorMsg.UNSUPPORTED_XSL_ERR, "Elemento XSL n\u00e3o suportado ''{0}''."},
				  new object[] {ErrorMsg.UNSUPPORTED_EXT_ERR, "Extens\u00e3o XSLTC n\u00e3o reconhecida ''{0}''."},
				  new object[] {ErrorMsg.MISSING_XSLT_URI_ERR, "O documento de entrada n\u00e3o \u00e9 uma p\u00e1gina de estilo (o espa\u00e7o de nomes XSL n\u00e3o est\u00e1 declarado no elemento root)."},
				  new object[] {ErrorMsg.MISSING_XSLT_TARGET_ERR, "N\u00e3o foi poss\u00edvel localizar o destino da folha de estilo ''{0}''."},
				  new object[] {ErrorMsg.NOT_IMPLEMENTED_ERR, "N\u00e3o implementado: ''{0}''."},
				  new object[] {ErrorMsg.NOT_STYLESHEET_ERR, "O documento de entrada n\u00e3o cont\u00e9m uma p\u00e1gina de estilo XSL."},
				  new object[] {ErrorMsg.ELEMENT_PARSE_ERR, "N\u00e3o foi poss\u00edvel analisar o elemento ''{0}''"},
				  new object[] {ErrorMsg.KEY_USE_ATTR_ERR, "O atributo use de <key> deve ser node, node-set, string ou number."},
				  new object[] {ErrorMsg.OUTPUT_VERSION_ERR, "A vers\u00e3o do documento XML de sa\u00edda deve ser 1.0"},
				  new object[] {ErrorMsg.ILLEGAL_RELAT_OP_ERR, "Operador desconhecido para express\u00e3o relacional "},
				  new object[] {ErrorMsg.ATTRIBSET_UNDEF_ERR, "Tentando utilizar um conjunto de atributos n\u00e3o existente ''{0}''."},
				  new object[] {ErrorMsg.ATTR_VAL_TEMPLATE_ERR, "N\u00e3o \u00e9 poss\u00edvel analisar o template de valor de atributo ''{0}''."},
				  new object[] {ErrorMsg.UNKNOWN_SIG_TYPE_ERR, "Tipo de dados desconhecido na assinatura da classe ''{0}''."},
				  new object[] {ErrorMsg.DATA_CONVERSION_ERR, "N\u00e3o \u00e9 poss\u00edvel converter o tipo de dados ''{0}'' em ''{1}''."},
				  new object[] {ErrorMsg.NO_TRANSLET_CLASS_ERR, "Este template n\u00e3o cont\u00e9m uma defini\u00e7\u00e3o de classe translet v\u00e1lida. "},
				  new object[] {ErrorMsg.NO_MAIN_TRANSLET_ERR, "Estes Templates n\u00e3o cont\u00eam uma classe com o nome ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_CLASS_ERR, "N\u00e3o foi poss\u00edvel carregar a classe translet ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_OBJECT_ERR, "Classe translet carregada, mas \u00e9 imposs\u00edvel criar a inst\u00e2ncia de translet. "},
				  new object[] {ErrorMsg.ERROR_LISTENER_NULL_ERR, "Tentando definir ErrorListener de ''{0}'' como nulo "},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_SOURCE_ERR, "Apenas StreamSource, SAXSource e DOMSource s\u00e3o suportados por XSLTC"},
				  new object[] {ErrorMsg.JAXP_NO_SOURCE_ERR, "O objeto Source transmitido para ''{0}'' n\u00e3o possui conte\u00fado. "},
				  new object[] {ErrorMsg.JAXP_COMPILE_ERR, "N\u00e3o foi poss\u00edvel compilar a p\u00e1gina de estilo "},
				  new object[] {ErrorMsg.JAXP_INVALID_ATTR_ERR, "TransformerFactory n\u00e3o reconhece o atributo ''{0}''."},
				  new object[] {ErrorMsg.JAXP_SET_RESULT_ERR, "setResult() deve ser chamado antes de startDocument()."},
				  new object[] {ErrorMsg.JAXP_NO_TRANSLET_ERR, "Transformer n\u00e3o possui nenhum objeto translet encapsulado. "},
				  new object[] {ErrorMsg.JAXP_NO_HANDLER_ERR, "Nenhuma rotina de tratamento de sa\u00edda definida para o resultado de transforma\u00e7\u00e3o. "},
				  new object[] {ErrorMsg.JAXP_NO_RESULT_ERR, "O objeto Result transmitido para ''{0}'' \u00e9 inv\u00e1lido. "},
				  new object[] {ErrorMsg.JAXP_UNKNOWN_PROP_ERR, "Tentando acessar a propriedade Transformer inv\u00e1lida ''{0}''."},
				  new object[] {ErrorMsg.SAX2DOM_ADAPTER_ERR, "N\u00e3o foi poss\u00edvel criar o adaptador SAX2DOM: ''{0}''."},
				  new object[] {ErrorMsg.XSLTC_SOURCE_ERR, "XSLTCSource.build() foi chamado sem systemId estar definido. "},
				  new object[] {ErrorMsg.ER_RESULT_NULL, "O resultado n\u00e3o deve ser nulo"},
				  new object[] {ErrorMsg.JAXP_INVALID_SET_PARAM_VALUE, "O valor do par\u00e2metro {0} deve ser um Objeto Java v\u00e1lido"},
				  new object[] {ErrorMsg.COMPILE_STDIN_ERR, "A op\u00e7\u00e3o -i deve ser utilizada com a op\u00e7\u00e3o -o."},
				  new object[] {ErrorMsg.COMPILE_USAGE_STR, "SYNOPSIS\n   java org.apache.xalan.xsltc.cmdline.Compile [-o <sa\u00edda>]\n      [-d <directory>] [-j <arquivo_jar>] [-p <pacote>]\n      [-n] [-x] [-u] [-v] [-h] { <stylesheet> | -i }\n\nOPTIONS\n   -o <sa\u00edda>    atribui o nome <sa\u00edda> para o\n                  translet gerado.  Por padr\u00e3o, o nome convertido \u00e9\n                  derivado do nome de <folha_de_estilo>. Esta op\u00e7\u00e3o\nser\u00e1 ignorada se estiverem sendo compiladas v\u00e1rias p\u00e1ginas de estilo.\n   -d <diret\u00f3rio> especifica um diret\u00f3rio de destino para translet\n   -j <arquivo_jar>   empacota classes translet em um arquivo jar do\nnome especificado como <arquivo_jar>\n   -p <pacote>  especifica um prefixo de nome de pacote para todas as\nclasses translet geradas.\n   -n ativa a seq\u00fc\u00eancia de templates (melhor comportamento padr\u00e3o\nna m\u00e9dia).\n   -x ativa a sa\u00edda de mensagem de depura\u00e7\u00e3o adicional\n  -u interpreta argumentos <stylesheet> como URLs\n   -i for\u00e7a o compilador a ler a folha de estilo de stdin\n   -v imprime a vers\u00e3o do compilador\n   -h imprime esta instru\u00e7\u00e3o de uso\n"},
				  new object[] {ErrorMsg.TRANSFORM_USAGE_STR, "SYNOPSIS \n   java org.apache.xalan.xsltc.cmdline.Transform [-j <arquivo_jar>]\n      [-x] [-n <itera\u00e7\u00f5es>] {-u <url_de_documento> | <documento>}\n      <classe> [<param1>=<valor1> ...]\n\n   utiliza o translet <classe> para transformar um documento XML \n   especificado como <documento>. A <classe> translet no\n  CLASSPATH do usu\u00e1rio ou no <arquivo_jar> opcionalmente especificado.\nOPTIONS\n   -j <arquivo_jar> especifica um arquivo jar a partir do qual ser\u00e1 carregado o translet\n   -x ativa a sa\u00edda de mensagem de depura\u00e7\u00e3o adicional\n  -n <itera\u00e7\u00f5es> executa os hor\u00e1rios de transforma\u00e7\u00e3o <itera\u00e7\u00f5es> e\n exibe informa\u00e7\u00f5es sobre tra\u00e7ado de perfil\n   -u <url_de_documento> especifica o documento XML de entrada como uma URL\n"},
				  new object[] {ErrorMsg.STRAY_SORT_ERR, "<xsl:sort> somente pode ser utilizado em <xsl:for-each> ou <xsl:apply-templates>."},
				  new object[] {ErrorMsg.UNSUPPORTED_ENCODING, "A codifica\u00e7\u00e3o de sa\u00edda ''{0}'' n\u00e3o \u00e9 suportada nesta JVM."},
				  new object[] {ErrorMsg.SYNTAX_ERR, "Erro de sintaxe em ''{0}''."},
				  new object[] {ErrorMsg.CONSTRUCTOR_NOT_FOUND, "N\u00e3o \u00e9 poss\u00edvel localizar o construtor externo ''{0}''."},
				  new object[] {ErrorMsg.NO_JAVA_FUNCT_THIS_REF, "O primeiro argumento para a fun\u00e7\u00e3o Java n\u00e3o est\u00e1tica ''{0}'' n\u00e3o \u00e9 uma refer\u00eancia de objeto v\u00e1lida. "},
				  new object[] {ErrorMsg.TYPE_CHECK_ERR, "Erro ao verificar o tipo de express\u00e3o ''{0}''."},
				  new object[] {ErrorMsg.TYPE_CHECK_UNK_LOC_ERR, "Erro ao verificar tipo de express\u00e3o em uma localiza\u00e7\u00e3o desconhecida. "},
				  new object[] {ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, "A op\u00e7\u00e3o da linha de comandos ''{0}'' n\u00e3o \u00e9 v\u00e1lida. "},
				  new object[] {ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, "A op\u00e7\u00e3o da linha de comandos ''{0}'' n\u00e3o cont\u00e9m um argumento requerido. "},
				  new object[] {ErrorMsg.WARNING_PLUS_WRAPPED_MSG, "AVISO:  ''{0}''\n       :{1}"},
				  new object[] {ErrorMsg.WARNING_MSG, "AVISO:  ''{0}''"},
				  new object[] {ErrorMsg.FATAL_ERR_PLUS_WRAPPED_MSG, "ERRO FATAL:  ''{0}''\n           :{1}"},
				  new object[] {ErrorMsg.FATAL_ERR_MSG, "ERRO FATAL:  ''{0}''"},
				  new object[] {ErrorMsg.ERROR_PLUS_WRAPPED_MSG, "ERRO:  ''{0}''\n     :{1}"},
				  new object[] {ErrorMsg.ERROR_MSG, "ERRO:  ''{0}''"},
				  new object[] {ErrorMsg.TRANSFORM_WITH_TRANSLET_STR, "Transformar utilizando translet ''{0}'' "},
				  new object[] {ErrorMsg.TRANSFORM_WITH_JAR_STR, "Transformar utilizando translet ''{0}'' do arquivo jar ''{1}'' "},
				  new object[] {ErrorMsg.COULD_NOT_CREATE_TRANS_FACT, "N\u00e3o foi poss\u00edvel criar uma inst\u00e2ncia da classe TransformerFactory ''{0}''."},
				  new object[] {ErrorMsg.TRANSLET_NAME_JAVA_CONFLICT, "N\u00e3o foi poss\u00edvel utilizar o nome ''{0}'' como o nome da classe translet porque ele cont\u00e9m caracteres n\u00e3o permitidos no nome da classe Java. O nome ''{1}'' foi utilizado como alternativa."},
				  new object[] {ErrorMsg.COMPILER_ERROR_KEY, "Erros do compilador:"},
				  new object[] {ErrorMsg.COMPILER_WARNING_KEY, "Avisos do compilador:"},
				  new object[] {ErrorMsg.RUNTIME_ERROR_KEY, "Erros de translet:"},
				  new object[] {ErrorMsg.INVALID_QNAME_ERR, "Um atributo cujo valor deve ser um QName ou uma lista de QNames separados por espa\u00e7os em branco apresentou o valor ''{0}''"},
				  new object[] {ErrorMsg.INVALID_NCNAME_ERR, "Um atributo cujo valor deve ser um NCName apresentou o valor ''{0}''"},
				  new object[] {ErrorMsg.INVALID_METHOD_IN_OUTPUT, "O atributo de m\u00e9todo de um elemento <xsl:output> apresentou o valor ''{0}''. O valor deve ser do tipo ''xml'', ''html'', ''text'' ou qname-but-not-ncname"},
				  new object[] {ErrorMsg.JAXP_GET_FEATURE_NULL_NAME, "O nome do recurso n\u00e3o pode ser nulo em TransformerFactory.getFeature(String name)."},
				  new object[] {ErrorMsg.JAXP_SET_FEATURE_NULL_NAME, "O nome do recurso n\u00e3o pode ser nulo em TransformerFactory.setFeature(String name, boolean value)."},
				  new object[] {ErrorMsg.JAXP_UNSUPPORTED_FEATURE, "N\u00e3o \u00e9 poss\u00edvel definir o recurso ''{0}'' neste TransformerFactory."}
			  };
			}
		}

	}

}