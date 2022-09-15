using System;
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
 * $Id: ErrorMsg.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using Stylesheet = org.apache.xalan.xsltc.compiler.Stylesheet;
	using SyntaxTreeNode = org.apache.xalan.xsltc.compiler.SyntaxTreeNode;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class ErrorMsg
	{

		private string _code;
		private int _line;
		private string _message = null;
		private string _url = null;
		internal object[] _params = null;
		private bool _isWarningError;

		// Compiler error messages
		public const string MULTIPLE_STYLESHEET_ERR = "MULTIPLE_STYLESHEET_ERR";
		public const string TEMPLATE_REDEF_ERR = "TEMPLATE_REDEF_ERR";
		public const string TEMPLATE_UNDEF_ERR = "TEMPLATE_UNDEF_ERR";
		public const string VARIABLE_REDEF_ERR = "VARIABLE_REDEF_ERR";
		public const string VARIABLE_UNDEF_ERR = "VARIABLE_UNDEF_ERR";
		public const string CLASS_NOT_FOUND_ERR = "CLASS_NOT_FOUND_ERR";
		public const string METHOD_NOT_FOUND_ERR = "METHOD_NOT_FOUND_ERR";
		public const string ARGUMENT_CONVERSION_ERR = "ARGUMENT_CONVERSION_ERR";
		public const string FILE_NOT_FOUND_ERR = "FILE_NOT_FOUND_ERR";
		public const string INVALID_URI_ERR = "INVALID_URI_ERR";
		public const string FILE_ACCESS_ERR = "FILE_ACCESS_ERR";
		public const string MISSING_ROOT_ERR = "MISSING_ROOT_ERR";
		public const string NAMESPACE_UNDEF_ERR = "NAMESPACE_UNDEF_ERR";
		public const string FUNCTION_RESOLVE_ERR = "FUNCTION_RESOLVE_ERR";
		public const string NEED_LITERAL_ERR = "NEED_LITERAL_ERR";
		public const string XPATH_PARSER_ERR = "XPATH_PARSER_ERR";
		public const string REQUIRED_ATTR_ERR = "REQUIRED_ATTR_ERR";
		public const string ILLEGAL_CHAR_ERR = "ILLEGAL_CHAR_ERR";
		public const string ILLEGAL_PI_ERR = "ILLEGAL_PI_ERR";
		public const string STRAY_ATTRIBUTE_ERR = "STRAY_ATTRIBUTE_ERR";
		public const string ILLEGAL_ATTRIBUTE_ERR = "ILLEGAL_ATTRIBUTE_ERR";
		public const string CIRCULAR_INCLUDE_ERR = "CIRCULAR_INCLUDE_ERR";
		public const string RESULT_TREE_SORT_ERR = "RESULT_TREE_SORT_ERR";
		public const string SYMBOLS_REDEF_ERR = "SYMBOLS_REDEF_ERR";
		public const string XSL_VERSION_ERR = "XSL_VERSION_ERR";
		public const string CIRCULAR_VARIABLE_ERR = "CIRCULAR_VARIABLE_ERR";
		public const string ILLEGAL_BINARY_OP_ERR = "ILLEGAL_BINARY_OP_ERR";
		public const string ILLEGAL_ARG_ERR = "ILLEGAL_ARG_ERR";
		public const string DOCUMENT_ARG_ERR = "DOCUMENT_ARG_ERR";
		public const string MISSING_WHEN_ERR = "MISSING_WHEN_ERR";
		public const string MULTIPLE_OTHERWISE_ERR = "MULTIPLE_OTHERWISE_ERR";
		public const string STRAY_OTHERWISE_ERR = "STRAY_OTHERWISE_ERR";
		public const string STRAY_WHEN_ERR = "STRAY_WHEN_ERR";
		public const string WHEN_ELEMENT_ERR = "WHEN_ELEMENT_ERR";
		public const string UNNAMED_ATTRIBSET_ERR = "UNNAMED_ATTRIBSET_ERR";
		public const string ILLEGAL_CHILD_ERR = "ILLEGAL_CHILD_ERR";
		public const string ILLEGAL_ELEM_NAME_ERR = "ILLEGAL_ELEM_NAME_ERR";
		public const string ILLEGAL_ATTR_NAME_ERR = "ILLEGAL_ATTR_NAME_ERR";
		public const string ILLEGAL_TEXT_NODE_ERR = "ILLEGAL_TEXT_NODE_ERR";
		public const string SAX_PARSER_CONFIG_ERR = "SAX_PARSER_CONFIG_ERR";
		public const string INTERNAL_ERR = "INTERNAL_ERR";
		public const string UNSUPPORTED_XSL_ERR = "UNSUPPORTED_XSL_ERR";
		public const string UNSUPPORTED_EXT_ERR = "UNSUPPORTED_EXT_ERR";
		public const string MISSING_XSLT_URI_ERR = "MISSING_XSLT_URI_ERR";
		public const string MISSING_XSLT_TARGET_ERR = "MISSING_XSLT_TARGET_ERR";
		public const string NOT_IMPLEMENTED_ERR = "NOT_IMPLEMENTED_ERR";
		public const string NOT_STYLESHEET_ERR = "NOT_STYLESHEET_ERR";
		public const string ELEMENT_PARSE_ERR = "ELEMENT_PARSE_ERR";
		public const string KEY_USE_ATTR_ERR = "KEY_USE_ATTR_ERR";
		public const string OUTPUT_VERSION_ERR = "OUTPUT_VERSION_ERR";
		public const string ILLEGAL_RELAT_OP_ERR = "ILLEGAL_RELAT_OP_ERR";
		public const string ATTRIBSET_UNDEF_ERR = "ATTRIBSET_UNDEF_ERR";
		public const string ATTR_VAL_TEMPLATE_ERR = "ATTR_VAL_TEMPLATE_ERR";
		public const string UNKNOWN_SIG_TYPE_ERR = "UNKNOWN_SIG_TYPE_ERR";
		public const string DATA_CONVERSION_ERR = "DATA_CONVERSION_ERR";

		// JAXP/TrAX error messages
		public const string NO_TRANSLET_CLASS_ERR = "NO_TRANSLET_CLASS_ERR";
		public const string NO_MAIN_TRANSLET_ERR = "NO_MAIN_TRANSLET_ERR";
		public const string TRANSLET_CLASS_ERR = "TRANSLET_CLASS_ERR";
		public const string TRANSLET_OBJECT_ERR = "TRANSLET_OBJECT_ERR";
		public const string ERROR_LISTENER_NULL_ERR = "ERROR_LISTENER_NULL_ERR";
		public const string JAXP_UNKNOWN_SOURCE_ERR = "JAXP_UNKNOWN_SOURCE_ERR";
		public const string JAXP_NO_SOURCE_ERR = "JAXP_NO_SOURCE_ERR";
		public const string JAXP_COMPILE_ERR = "JAXP_COMPILE_ERR";
		public const string JAXP_INVALID_ATTR_ERR = "JAXP_INVALID_ATTR_ERR";
		public const string JAXP_SET_RESULT_ERR = "JAXP_SET_RESULT_ERR";
		public const string JAXP_NO_TRANSLET_ERR = "JAXP_NO_TRANSLET_ERR";
		public const string JAXP_NO_HANDLER_ERR = "JAXP_NO_HANDLER_ERR";
		public const string JAXP_NO_RESULT_ERR = "JAXP_NO_RESULT_ERR";
		public const string JAXP_UNKNOWN_PROP_ERR = "JAXP_UNKNOWN_PROP_ERR";
		public const string SAX2DOM_ADAPTER_ERR = "SAX2DOM_ADAPTER_ERR";
		public const string XSLTC_SOURCE_ERR = "XSLTC_SOURCE_ERR";
		public const string ER_RESULT_NULL = "ER_RESULT_NULL";
		public const string JAXP_INVALID_SET_PARAM_VALUE = "JAXP_INVALID_SET_PARAM_VALUE";
		public const string JAXP_SET_FEATURE_NULL_NAME = "JAXP_SET_FEATURE_NULL_NAME";
		public const string JAXP_GET_FEATURE_NULL_NAME = "JAXP_GET_FEATURE_NULL_NAME";
		public const string JAXP_UNSUPPORTED_FEATURE = "JAXP_UNSUPPORTED_FEATURE";

		// Command-line error messages
		public const string COMPILE_STDIN_ERR = "COMPILE_STDIN_ERR";
		public const string COMPILE_USAGE_STR = "COMPILE_USAGE_STR";
		public const string TRANSFORM_USAGE_STR = "TRANSFORM_USAGE_STR";

		// Recently added error messages
		public const string STRAY_SORT_ERR = "STRAY_SORT_ERR";
		public const string UNSUPPORTED_ENCODING = "UNSUPPORTED_ENCODING";
		public const string SYNTAX_ERR = "SYNTAX_ERR";
		public const string CONSTRUCTOR_NOT_FOUND = "CONSTRUCTOR_NOT_FOUND";
		public const string NO_JAVA_FUNCT_THIS_REF = "NO_JAVA_FUNCT_THIS_REF";
		public const string TYPE_CHECK_ERR = "TYPE_CHECK_ERR";
		public const string TYPE_CHECK_UNK_LOC_ERR = "TYPE_CHECK_UNK_LOC_ERR";
		public const string ILLEGAL_CMDLINE_OPTION_ERR = "ILLEGAL_CMDLINE_OPTION_ERR";
		public const string CMDLINE_OPT_MISSING_ARG_ERR = "CMDLINE_OPT_MISSING_ARG_ERR";
		public const string WARNING_PLUS_WRAPPED_MSG = "WARNING_PLUS_WRAPPED_MSG";
		public const string WARNING_MSG = "WARNING_MSG";
		public const string FATAL_ERR_PLUS_WRAPPED_MSG = "FATAL_ERR_PLUS_WRAPPED_MSG";
		public const string FATAL_ERR_MSG = "FATAL_ERR_MSG";
		public const string ERROR_PLUS_WRAPPED_MSG = "ERROR_PLUS_WRAPPED_MSG";
		public const string ERROR_MSG = "ERROR_MSG";
		public const string TRANSFORM_WITH_TRANSLET_STR = "TRANSFORM_WITH_TRANSLET_STR";
		public const string TRANSFORM_WITH_JAR_STR = "TRANSFORM_WITH_JAR_STR";
		public const string COULD_NOT_CREATE_TRANS_FACT = "COULD_NOT_CREATE_TRANS_FACT";
		public const string TRANSLET_NAME_JAVA_CONFLICT = "TRANSLET_NAME_JAVA_CONFLICT";
		public const string INVALID_QNAME_ERR = "INVALID_QNAME_ERR";
		public const string INVALID_NCNAME_ERR = "INVALID_NCNAME_ERR";
		public const string INVALID_METHOD_IN_OUTPUT = "INVALID_METHOD_IN_OUTPUT";

		public const string OUTLINE_ERR_TRY_CATCH = "OUTLINE_ERR_TRY_CATCH";
		public const string OUTLINE_ERR_UNBALANCED_MARKERS = "OUTLINE_ERR_UNBALANCED_MARKERS";
		public const string OUTLINE_ERR_DELETED_TARGET = "OUTLINE_ERR_DELETED_TARGET";
		public const string OUTLINE_ERR_METHOD_TOO_BIG = "OUTLINE_ERR_METHOD_TOO_BIG";

		// All error messages are localized and are stored in resource bundles.
		// This array and the following 4 strings are read from that bundle.
		private static ResourceBundle _bundle;

		public const string ERROR_MESSAGES_KEY = "ERROR_MESSAGES_KEY";
		public const string COMPILER_ERROR_KEY = "COMPILER_ERROR_KEY";
		public const string COMPILER_WARNING_KEY = "COMPILER_WARNING_KEY";
		public const string RUNTIME_ERROR_KEY = "RUNTIME_ERROR_KEY";

		static ErrorMsg()
		{
			_bundle = ResourceBundle.getBundle("org.apache.xalan.xsltc.compiler.util.ErrorMessages", Locale.getDefault());
		}

		public ErrorMsg(string code)
		{
		_code = code;
		_line = 0;
		}

		public ErrorMsg(Exception e)
		{
		   _code = null;
		_message = e.Message;
		_line = 0;
		}

		public ErrorMsg(string message, int line)
		{
		_code = null;
		_message = message;
		_line = line;
		}

		public ErrorMsg(string code, int line, object param)
		{
		_code = code;
		_line = line;
		_params = new object[] {param};
		}

		public ErrorMsg(string code, object param) : this(code)
		{
		_params = new object[1];
		_params[0] = param;
		}

		public ErrorMsg(string code, object param1, object param2) : this(code)
		{
		_params = new object[2];
		_params[0] = param1;
		_params[1] = param2;
		}

		public ErrorMsg(string code, SyntaxTreeNode node)
		{
		_code = code;
		_url = getFileName(node);
		_line = node.LineNumber;
		}

		public ErrorMsg(string code, object param1, SyntaxTreeNode node)
		{
		_code = code;
		_url = getFileName(node);
		_line = node.LineNumber;
		_params = new object[1];
		_params[0] = param1;
		}

		public ErrorMsg(string code, object param1, object param2, SyntaxTreeNode node)
		{
		_code = code;
		_url = getFileName(node);
		_line = node.LineNumber;
		_params = new object[2];
		_params[0] = param1;
		_params[1] = param2;
		}

		private string getFileName(SyntaxTreeNode node)
		{
		Stylesheet stylesheet = node.Stylesheet;
		if (stylesheet != null)
		{
			return stylesheet.SystemId;
		}
		else
		{
			return null;
		}
		}

		private string formatLine()
		{
		StringBuilder result = new StringBuilder();
		if (!string.ReferenceEquals(_url, null))
		{
			result.Append(_url);
			result.Append(": ");
		}
		if (_line > 0)
		{
			result.Append("line ");
			result.Append(Convert.ToString(_line));
			result.Append(": ");
		}
		return result.ToString();
		}

		/// <summary>
		/// This version of toString() uses the _params instance variable
		/// to format the message. If the <code>_code</code> is negative
		/// the use _message as the error string.
		/// </summary>
		public override string ToString()
		{
		string suffix = (_params == null) ? (null != _code ? ErrorMessage : _message) : MessageFormat.format(ErrorMessage, _params);
		return formatLine() + suffix;
		}

		public string toString(object obj)
		{
		object[] @params = new object[1];
		@params[0] = obj.ToString();
		string suffix = MessageFormat.format(ErrorMessage, @params);
		return formatLine() + suffix;
		}

		public string toString(object obj0, object obj1)
		{
		object[] @params = new object[2];
		@params[0] = obj0.ToString();
		@params[1] = obj1.ToString();
		string suffix = MessageFormat.format(ErrorMessage, @params);
		return formatLine() + suffix;
		}

		/// <summary>
		/// Return an ErrorMessages string corresponding to the _code
		/// This function is temporary until the three special-cased keys
		/// below are moved into ErrorMessages
		/// </summary>
		/// <returns> ErrorMessages string </returns>
		private string ErrorMessage
		{
			get
			{
			  return _bundle.getString(_code);
			}
		}

		// If the _isWarningError flag is true, the error is treated as
		// a warning by the compiler, but should be reported as an error
		// to the ErrorListener. This is a workaround for the TCK failure 
		// ErrorListener.errorTests.error001.
		public bool WarningError
		{
			set
			{
				_isWarningError = value;
			}
			get
			{
				return _isWarningError;
			}
		}

	}


}