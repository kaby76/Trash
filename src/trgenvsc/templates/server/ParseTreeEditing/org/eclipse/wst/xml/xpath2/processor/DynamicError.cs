using System;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     David Carver (STAR) - bug 273763 - correct error codes.
///                           bug 280106 - Correct XPDY0021 - XPST0003 
///     Jesper Steen Moeller - bug 28149 - add more fn:error info
///     Jesper Steen Moller  - bug 281159 - fix document loading and resolving URIs 
///     Jesper Steen Moller  - Bug 286062 - Add FOAR0002  
///     Jesper Steen Moller  - bug 280555 - Add pluggable collation support
///     Jesper Steen Moller  - bug 262765 - Add FORG0006
///     Jesper Steen Moller  - bug 290337 - Revisit use of ICU
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using TypeError = org.eclipse.wst.xml.xpath2.processor.@internal.TypeError;

	/// <summary>
	/// Dynamic Error like division by 0 or type errors.
	/// </summary>
	public class DynamicError : XPathException
	{
		/// 
		private const long serialVersionUID = -6146830764753685791L;

		// errorcode specified in http://www.w3.org/2004/10/xqt-errors i fink
		private string _code;

		// XXX dirty... should fix the error stuff
		// have a type error encapsulated in a dynamic error
		private TypeError _te;

		/// <summary>
		/// Constructor for Dynamic Error.
		/// </summary>
		/// <param name="code">
		///            is the code that is set. </param>
		/// <param name="err">
		///            is the reason for the error. </param>
		public DynamicError(string code, string err) : base(err)
		{
			_code = code;
			_te = null;
		}

		/// <summary>
		/// Constructor for Dynamic Error.
		/// </summary>
		/// <param name="te">
		///            is the error type. </param>
		public DynamicError(TypeError te) : base(te.reason())
		{
			_te = te;
			_code = te.code();
		}

		/// <summary>
		/// Returns the string of the code.
		/// </summary>
		/// <returns> the code. </returns>
		public virtual string code()
		{
			if (_te != null)
			{
				return _te.code();
			}
			return _code;
		}

		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <param name="err">
		///            is the error </param>
		/// <returns> the DynamicError. </returns>
		public static DynamicError cant_cast(string err)
		{
			string error = "Can't cast to required type.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new DynamicError("FORG0001", error);
		}

		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <exception cref="DynamicError">
		///             a Dynamic Error </exception>
		/// <returns> the DynamicError. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DynamicError throw_type_error() throws DynamicError
		public static DynamicError throw_type_error()
		{
			throw argument_type_error(null);
		}

		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <param name="type"> Type found </param>
		/// <returns> the DynamicError.
		/// @since 1.1 </returns>
		public static DynamicError argument_type_error(Type type)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return new DynamicError("FORG0006", type != null ? "Invalid argument type :" + type.FullName : "Invalid argument type");
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DynamicError invalidType() throws DynamicError
		public static DynamicError invalidType()
		{
			throw new DynamicError(TypeError.invalid_type(null));
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DynamicError inputToLargeForDecimal() throws DynamicError
		public static DynamicError inputToLargeForDecimal()
		{
			throw new DynamicError("FOCA0001", "Input value too large for decimal");
		}
		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <param name="desc">
		///            is the description of the error </param>
		/// <returns> the DynamicError.
		/// @since 1.1 </returns>
		public static DynamicError user_error(string ns, string code, string desc)
		{
			string error = "Error reported by fn:error.";

			if (!string.ReferenceEquals(desc, null))
			{
				error = desc + " (reported by fn:error)";
			}

			// XXX: Need to pass the namespace also...
			return new DynamicError(code, error);
		}

		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <param name="err">
		///            is the description of the error </param>
		/// <returns> the DynamicError. </returns>
		public static DynamicError user_error(string err)
		{
			string error = "Error reported by fn:error.";

			if (!string.ReferenceEquals(err, null))
			{
				error = err + " (reported by fn:error)";
			}

			// XXX: Need to pass the namespace also...
			return new DynamicError("FOER0000", error);
		}

		/// <summary>
		/// Returns the Dynamic Error for invalid flags in regular expressions </summary>
		/// <param name="err">
		/// @return
		/// @since 1.1 </param>
		public static DynamicError regex_flags_error(string err)
		{
			string error = "Invalid regular expression flag parameter.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new DynamicError("FORX0001", error);

		}

		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <param name="err">
		///            is the error </param>
		/// <returns> the DynamicError. </returns>
		public static DynamicError regex_error(string err)
		{
			string error = "Invalid regular expression.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new DynamicError("FORX0002", error);
		}

		/// <summary>
		/// Returns the dynamic error.
		/// </summary>
		/// <param name="err">
		///            is the error </param>
		/// <returns> the DynamicError.
		/// @since 1.1 </returns>
		public static DynamicError regex_match_zero_length(string err)
		{
			string error = "Invalid regular expression.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new DynamicError("FORX0003", error);
		}

		/// <summary>
		/// Returns the dynamic error for an unsupported Unicode codepoint
		/// </summary>
		/// <param name="err">
		///            is the error </param>
		/// <returns> the DynamicError.
		/// @since 1.1
		///  </returns>
		public static DynamicError unsupported_codepoint(string err)
		{
			string error = "Unsupported codepoint";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new DynamicError("FOCH0001", error);
		}

		/// <summary>
		/// Returns the dynamic error for an unsupported normalization form
		/// </summary>
		/// <param name="collationName">
		///            is the error </param>
		/// <returns> the DynamicError.
		/// @since 1.1
		///  </returns>
		public static DynamicError unsupported_collation(string collationName)
		{
			string error = "Unsupported collation URI. ";

			if (!string.ReferenceEquals(collationName, null))
			{
				error += " " + collationName;
			}

			return new DynamicError("FOCH0002", error);
		}

		/// <summary>
		/// Returns the dynamic error for an unsupported normalization form
		/// </summary>
		/// <param name="err">
		///            is the error </param>
		/// <returns> the DynamicError.
		/// @since 1.1
		///  </returns>
		public static DynamicError unsupported_normalization_form(string err)
		{
			string error = "Unsupported normalization form: ";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new DynamicError("FOCH0003", error);
		}

		/// <summary>
		/// Returns the dynamic error for an unsupported normalization form
		/// </summary>
		/// <param name="err">
		///            is the error </param>
		/// <returns> the DynamicError.
		/// @since 1.1
		///  </returns>
		public static DynamicError runtime_error(string msg, Exception err)
		{
			string error = "Error at runtime: " + msg + ": " + err.Message;

			return new DynamicError("FOER0000", error);
		}

		private static DynamicError make_error(string code, string err, string msg)
		{
			string error = err;

			if (!string.ReferenceEquals(msg, null))
			{
				error += msg;
			}

			return new DynamicError(code, error);
		}

		/// <summary>
		/// Returns the error message when reads an Invalid lexical value
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError lexical_error(string msg)
		{
			return make_error("FOCA0002", "Invalid lexical value.", msg);
		}

		/// <summary>
		/// Returns the error message when reads an Items not comparable
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError not_cmp(string msg)
		{
			return make_error("FOTY0012", "Items not comparable", msg);
		}

		/// <summary>
		/// Returns the error message
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError more_one_item(string msg)
		{
			return make_error("FORG0003", "fn:zero-or-one called with a sequence containing more than one item", msg);
		}

		/// <summary>
		/// Returns the error message
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError empty_seq(string msg)
		{
			return make_error("FORG0004", "fn:one-or-more called with a sequence containing no items", msg);
		}

		/// <summary>
		/// Returns the error message
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError not_one(string msg)
		{
			return make_error("FORG0005", "fn:exactly-one called with a sequence containing zero or more than one item", msg);
		}

		/// <summary>
		/// Returns the error message when reads Invalid argument to fn:collection
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error
		/// @since 1.1 </returns>
		public static DynamicError invalidCollectionArgument()
		{
			return make_error("FODC0004", "Invalid argument to fn:doc", null);
		}

		/// <summary>
		/// Returns the error message when reads Invalid argument to fn:doc
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError invalid_doc(string msg)
		{
			return make_error("FODC0005", "Invalid argument to fn:doc", msg);
		}

		/// <summary>
		/// Returns the error message when fn:doc cannot load its document
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error
		/// @since 1.1 </returns>
		public static DynamicError doc_not_found(string msg)
		{
			return make_error("FODC0002", "Document argument fn:doc not found", msg);
		}

		/// <summary>
		/// Returns the error message when reads a Division by zero
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error </returns>
		public static DynamicError div_zero(string msg)
		{
			return make_error("FOAR0001", "Division by zero", msg);
		}

		/// <summary>
		/// Numeric operation overflow/underflow
		/// </summary>
		/// <param name="msg">
		///            is the message </param>
		/// <returns> the make_error
		/// @since 1.1 </returns>
		public static DynamicError numeric_overflow(string msg)
		{
			return make_error("FOAR0002", "Numeric overflow/underflow", msg);
		}
		/// <summary>
		/// @since 1.1
		/// </summary>
		public static DynamicError contextUndefined()
		{
			return make_error("XPDY0002", "Context is undefined.", "");
		}

		/// <summary>
		/// Data is invalid for casting or the data type constructor. </summary>
		/// <param name="msg"> </param>
		/// <returns> FORG0001
		/// @since 1.1 </returns>
		public static DynamicError invalidForCastConstructor()
		{
			return make_error("FORG0001", "data type invalid for cast or constructor", null);
		}

		/// <summary>
		/// No namespace found for prefix.
		/// 
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError invalidPrefix()
		{
			return make_error("FONS0004", "No namespace found for prefix.", null);
		}

		/// <summary>
		/// No context document
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError noContextDoc()
		{
			return make_error("FODC0001", "No context document.", null);
		}

		/// <summary>
		/// No base-uri defined.
		/// 
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError noBaseURI()
		{
			return make_error("FONS0005", "Base-uri not defined in the static context.", null);
		}

		/// <summary>
		/// Error resolving relative uri against base-uri.
		/// 
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError errorResolvingURI()
		{
			return make_error("FORG0002", "Invalid argument to fn:resolve-uri().", null);
		}

		/// <summary>
		/// Invalid Timezone value.
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError invalidTimezone()
		{
			return make_error("FODT0003", "Invalid timezone value.", null);
		}

		/// <summary>
		/// Overflow/underflow in duration operation.
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError overflowUnderflow()
		{
			return make_error("FODT0002", "Overflow/underflow in duration operation.", null);
		}

		/// <summary>
		/// Overflow/underflow in duration operation.
		/// @return
		/// @since 1.1
		/// </summary>
		public static DynamicError nan()
		{
			return make_error("FOCA0005", "NaN supplied as float/double value.", null);
		}

		/// <summary>
		/// Invalid lexical value
		/// 
		/// @since 1.1
		/// </summary>
		public static DynamicError invalidLexicalValue()
		{
			return make_error("FOCA0002", "Invalid lexical value.", null);
		}

		/// <summary>
		/// Overflow/underflow in date/time operation 
		/// @since 1.1
		/// </summary>
		public static DynamicError overflowDateTime()
		{
			return make_error("FODT0001", "Overflow/underflow in date/time operation", null);
		}

		/// <summary>
		/// The two arguments to fn:dateTime have inconsistent timezones
		/// 
		/// @since 1.1
		/// </summary>
		public static DynamicError inconsistentTimeZone()
		{
			return make_error("FORG0008", "The two arguments to fn:dateTime have inconsistent timezones", null);
		}
	}

}